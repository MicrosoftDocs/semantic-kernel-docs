---
title: Agent Executor
description: Deep dive into the AgentExecutor, the built-in executor that adapts AI agents for use in workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: conceptual
ms.author: taochen
ms.date: 03/16/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                        | C# | Python | Notes                                     |
  |--------------------------------|:--:|:------:|-------------------------------------------|
  | Overview                       | ✅ |   ✅   |                                           |
  | How It Works                   | ✅ |   ✅   |                                           |
  | Implicit vs Explicit Creation  | ✅ |   ✅   |                                           |
  | Input Types                    | ✅ |   ✅   | Different supported types per language     |
  | Output and Chaining            | ✅ |   ✅   |                                           |
  | Streaming Behavior             | ✅ |   ✅   |                                           |
  | Shared Sessions                | ✅ |   ✅   |                                           |
  | Configuration Options          | ✅ |   ❌   | C#-specific (AIAgentHostOptions)           |
  | Checkpointing                  | ✅ |   ✅   |                                           |
-->

# Agent Executor

When you add an AI agent to a workflow, it needs to be wrapped in an executor so the workflow engine can route messages to it, manage its session state, and handle its output. The **Agent Executor** is the built-in executor that handles this adaptation.

## Overview

The Agent Executor bridges the gap between the agent abstraction and the workflow execution model. It:

- Receives typed messages from the workflow graph and forwards them to the underlying agent.
- Manages the agent's session and conversation state between runs.
- Adapts its behavior based on the workflow execution mode (streaming or non-streaming).
- Yields output events (`AgentResponse` or `AgentResponseUpdate`) to the workflow caller for observation.
- Sends messages to connected downstream executors for continued processing within the graph.
- Supports checkpointing for long-running workflows.

::: zone pivot="programming-language-csharp"

## How It Works

In C#, the workflow engine internally creates an `AIAgentHostExecutor` for each `AIAgent` added to a workflow. This specialized executor extends `ChatProtocolExecutor` and uses a **turn token** pattern:

1. **Message caching** — as messages arrive from other executors, the agent executor collects them. If `ForwardIncomingMessages` is enabled (the default), the incoming messages are also forwarded to downstream executors.
2. **Turn token trigger** — the agent processes its cached messages only after receiving a `TurnToken`.
3. **Agent invocation** — the executor calls `RunAsync` (non-streaming) or `RunStreamingAsync` (streaming) on the underlying agent.
4. **Output yielding** — if streaming events are enabled, each incremental `AgentResponseUpdate` is yielded as a workflow output. If `EmitAgentResponseEvents` is enabled, the aggregated `AgentResponse` is also yielded as a workflow output.
5. **Downstream messaging** — the agent's response messages are sent to connected downstream executors.
6. **Turn token pass-through** — after completing its turn, the executor sends a new `TurnToken` downstream so that the next agent in the chain can begin processing.

> [!TIP]
> Some scenarios may require a more specialized agent executor; for example, [handoff orchestrations](../orchestrations/handoff.md) use a dedicated `HandoffAgentExecutor` with custom routing logic.

## Implicit vs Explicit Creation

When you pass an `AIAgent` to `WorkflowBuilder`, the framework automatically wraps it in an `AIAgentBinding`, which creates the underlying `AIAgentHostExecutor`. You do not need to instantiate the agent executor directly.

```csharp
AIAgent writerAgent = /* create your agent */;
AIAgent reviewerAgent = /* create your agent */;

// Agents are automatically wrapped — no manual executor creation required
var workflow = new WorkflowBuilder(writerAgent)
    .AddEdge(writerAgent, reviewerAgent)
    .Build();
```

You can also use the helper methods on `AgentWorkflowBuilder` for common patterns:

```csharp
// Build a sequential pipeline of agents
var workflow = AgentWorkflowBuilder.BuildSequential(writerAgent, reviewerAgent);
```

### Custom Configuration

To customize how the agent executor behaves, use `BindAsExecutor` with `AIAgentHostOptions`:

```csharp
var options = new AIAgentHostOptions
{
    EmitAgentUpdateEvents = true,
    EmitAgentResponseEvents = true,
    ReassignOtherAgentsAsUsers = true,
    ForwardIncomingMessages = true,
};

ExecutorBinding writerBinding = writerAgent.BindAsExecutor(options);
var workflow = new WorkflowBuilder(writerBinding)
    .AddEdge(writerBinding, reviewerAgent)
    .Build();
```

## Input Types

The agent executor in C# accepts multiple input types: `string`, `ChatMessage`, and `IEnumerable<ChatMessage>`. String inputs are automatically converted to `ChatMessage` instances with the `User` role. All incoming messages are accumulated until a `TurnToken` is received, at which point the executor processes the batch. When `ReassignOtherAgentsAsUsers` is enabled (the default), messages from other agents are reassigned to the `User` role so the underlying model treats them as user inputs, while messages from the current agent retain the `Assistant` role.

## Output and Chaining

After the agent completes its turn, the executor:

1. Sends the agent's response messages to all connected downstream executors.
2. Forwards a new `TurnToken` so the next agent in the chain can begin processing.

This makes chaining agents straightforward — simply connect them with edges:

```csharp
var workflow = new WorkflowBuilder(frenchTranslator)
    .AddEdge(frenchTranslator, spanishTranslator)
    .AddEdge(spanishTranslator, englishTranslator)
    .Build();
```

## Streaming Behavior

Streaming behavior is controlled by the `EmitAgentUpdateEvents` option on `AIAgentHostOptions`, or dynamically via the `TurnToken`:

- **When enabled** — the executor calls `RunStreamingAsync` on the agent and yields each `AgentResponseUpdate` as a workflow output event. This provides real-time token-by-token updates.
- **When disabled** — the executor calls `RunAsync` and produces a single complete response.

```csharp
// Enable streaming events at the configuration level
var options = new AIAgentHostOptions
{
    EmitAgentUpdateEvents = true,
};

// Or enable streaming dynamically via TurnToken
await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
```

## Shared Sessions

Each agent executor maintains its own session by default. To share a session between agents, configure the agents with a common session provider before adding them to the workflow.

## Configuration Options

`AIAgentHostOptions` controls the agent executor's behavior:

| Option | Default | Description |
|--------|---------|-------------|
| `EmitAgentUpdateEvents` | `null` | Emit streaming update events during execution. `TurnToken` takes precedence if set. If both are `null`, streaming is disabled. |
| `EmitAgentResponseEvents` | `false` | Emit the aggregated agent response as a workflow output event. |
| `InterceptUserInputRequests` | `false` | Intercept `UserInputRequestContent` and route it as a workflow message for handling. |
| `InterceptUnterminatedFunctionCalls` | `false` | Intercept `FunctionCallContent` without a corresponding result and route it as a workflow message. |
| `ReassignOtherAgentsAsUsers` | `true` | Reassign messages from other agents to the `User` role so the model treats them as user inputs. |
| `ForwardIncomingMessages` | `true` | Forward incoming messages to downstream executors before the agent's generated messages. |

## Checkpointing

The agent executor supports checkpointing for long-running workflows. When a checkpoint is taken, the executor serializes:

- The agent's session state (via `SerializeSessionAsync`).
- The current turn's event emission configuration (only present while requests are pending and the executor has not yet yielded its incoming `TurnToken`).
- Any pending user input requests and function call requests.

On restore, the executor deserializes the session and pending request state, allowing the workflow to resume from where it left off.

::: zone-end

::: zone pivot="programming-language-python"

## How It Works

The `AgentExecutor` class wraps an agent that implements the `SupportsAgentRun` protocol. When the executor receives a message:

1. **Message normalization** — the input is normalized into a list of `Message` objects and added to the executor's internal cache. The executor accepts multiple input types — `str`, `Message`, `list[str | Message]`, `AgentExecutorRequest`, and `AgentExecutorResponse` — each routed to a dedicated handler that normalizes the input before caching.
2. **Agent invocation** — the executor calls `agent.run()` with the cached messages, automatically selecting streaming or non-streaming mode based on the workflow execution mode.
3. **Output emission** — in streaming mode, each `AgentResponseUpdate` is yielded as a workflow output event. In non-streaming mode, a single `AgentResponse` is yielded.
4. **Downstream dispatch** — after the agent completes, the executor sends an `AgentExecutorResponse` to all connected downstream executors. This response includes the full conversation history, enabling seamless chaining.
5. **Cache reset** — the executor's internal message cache is cleared after the agent is invoked, ensuring that each agent invocation processes only new messages received since the last invocation.

> [!TIP]
> Some scenarios may require a more specialized agent executor; for example, [handoff orchestrations](../orchestrations/handoff.md) use a dedicated executor with custom routing logic.

## Implicit vs Explicit Creation

The `WorkflowBuilder` automatically wraps agents in `AgentExecutor` instances when you pass an agent directly. For most workflows, implicit creation is sufficient:

```python
from agent_framework import WorkflowBuilder

writer_agent = client.as_agent(name="Writer", instructions="...")
reviewer_agent = client.as_agent(name="Reviewer", instructions="...")

# Agents are automatically wrapped — no manual AgentExecutor creation required
workflow = (
    WorkflowBuilder(start_executor=writer_agent)
    .add_edge(writer_agent, reviewer_agent)
    .build()
)
```

### Explicit Creation

Create an `AgentExecutor` explicitly when you need to:

- Share a session between multiple agents.
- Provide a custom executor ID.
- Reference the same executor instance in multiple edges.

```python
from agent_framework import AgentExecutor

writer_executor = AgentExecutor(writer_agent, id="my-writer")
reviewer_executor = AgentExecutor(reviewer_agent, id="my-reviewer")

workflow = (
    WorkflowBuilder(start_executor=writer_executor)
    .add_edge(writer_executor, reviewer_executor)
    .build()
)
```

**Constructor parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `agent` | `SupportsAgentRun` | The agent to wrap. |
| `session` | `AgentSession \| None` | Session to use for agent runs. If `None`, a new session is created from the agent. |
| `id` | `str \| None` | Unique executor ID. Defaults to the agent's name if available. |

## Input Types

The `AgentExecutor` defines multiple handler methods, each accepting a different input type. The workflow engine automatically dispatches the correct handler based on the message type. All input types trigger the agent to run immediately, except for `AgentExecutorRequest` where the `should_respond` flag controls whether the agent runs or simply caches the messages:

| Input Type | Handler | Triggers Agent | Description |
|------------|---------|:--------------:|-------------|
| `AgentExecutorRequest` | `run` | Conditional | The canonical input type. Contains a list of messages and a `should_respond` flag that controls whether the agent runs. |
| `str` | `from_str` | Always | Accepts a raw string prompt. |
| `Message` | `from_message` | Always | Accepts a single `Message` object. |
| `list[str \| Message]` | `from_messages` | Always | Accepts a list of strings or `Message` objects as conversation context. |
| `AgentExecutorResponse` | `from_response` | Always | Accepts a prior agent executor's response, enabling direct chaining. |

### Using AgentExecutorRequest

`AgentExecutorRequest` is the canonical input type and provides the most control:

```python
from agent_framework import AgentExecutorRequest, Message

# Create a request with messages
request = AgentExecutorRequest(
    messages=[Message("user", text="Hello, world!")],
    should_respond=True,
)

# Run the workflow
result = await workflow.run(request)
```

The `should_respond` flag controls whether the agent processes the messages immediately or simply caches them for later:

- `True` (default) — the agent runs and produces a response.
- `False` — the messages are added to the cache but the agent does not run. This is useful for preloading conversation context before triggering a response.

## Output and Chaining

After the agent completes, the executor sends an `AgentExecutorResponse` downstream. This dataclass contains:

| Field | Type | Description |
|-------|------|-------------|
| `executor_id` | `str` | The ID of the executor that produced the response. |
| `agent_response` | `AgentResponse` | The underlying agent response (unaltered from the client). |
| `full_conversation` | `list[Message] \| None` | The full conversation context (prior inputs + agent outputs) for chaining. |

When chaining agent executors, the downstream executor receives the `AgentExecutorResponse` via the `from_response` handler. It uses the `full_conversation` field to preserve the complete conversation history, preventing downstream agents from losing prior context:

```python
spam_detector = AgentExecutor(create_spam_detector_agent())
email_assistant = AgentExecutor(create_email_assistant_agent())

# The email_assistant receives the spam_detector's full conversation context
workflow = (
    WorkflowBuilder(start_executor=spam_detector)
    .add_edge(spam_detector, email_assistant)
    .build()
)
```

## Streaming Behavior

The `AgentExecutor` automatically adapts to the workflow execution mode:

- **`stream=True`** — calls `agent.run(stream=True)` and yields each `AgentResponseUpdate` as a workflow output event. After streaming completes, the updates are aggregated into a full `AgentResponse` for downstream dispatch.
- **`stream=False`** (default) — calls `agent.run(stream=False)` and yields a single `AgentResponse` as a workflow output event.

```python
# Streaming mode — receive incremental updates
events = workflow.run("Write a story about a cat.", stream=True)
async for event in events:
    if event.type == "output" and isinstance(event.data, AgentResponseUpdate):
        print(event.data.text, end="", flush=True)

# Non-streaming mode — receive complete response
result = await workflow.run("Write a story about a cat.")

# Retrieve AgentResponse objects from the result
outputs = result.get_outputs()
for output in outputs:
    if isinstance(output, AgentResponse):
        print(output.text)
```

## Shared Sessions

By default, each `AgentExecutor` creates its own session. To share a session between multiple agents (for example, to maintain a common conversation thread), create a session explicitly and pass it to each executor:

```python
from agent_framework import AgentExecutor

# Create a shared session from one agent
shared_session = writer_agent.create_session()

# Both executors share the same session
writer_executor = AgentExecutor(writer_agent, session=shared_session)
reviewer_executor = AgentExecutor(reviewer_agent, session=shared_session)
```

> [!NOTE]
> Not all agents support shared sessions. Typically, only agents of the same provider type can share a session.

## Checkpointing

The `AgentExecutor` supports checkpointing for saving and restoring state in long-running workflows. When a checkpoint is taken, the executor serializes:

- The internal message cache.
- The full conversation history.
- The agent session state.
- Any pending user input requests and responses.

On restore, the executor deserializes this state, allowing the workflow to resume from where it left off.

> [!WARNING]
> Checkpointing with agents that use server-side sessions (such as `AzureAIAgentClient`) has limitations. Server-side session state is not captured in checkpoints and can be modified by subsequent runs. Consider implementing a custom executor if you need reliable checkpointing with server-side sessions.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Agents in Workflows](../agents-in-workflows.md)
