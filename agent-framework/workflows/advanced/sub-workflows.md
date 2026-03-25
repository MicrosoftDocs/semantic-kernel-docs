---
title: Sub-Workflows
description: Deep dive into composing workflows by nesting them as executors within parent workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: conceptual
ms.author: taochen
ms.date: 03/23/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                        | C# | Python | Notes                              |
  |--------------------------------|:--:|:------:|-------------------------------------|
  | Overview                       | ✅ |   ✅   |                                    |
  | Creating a Sub-Workflow        | ✅ |   ✅   |                                    |
  | Implicit vs Explicit Wrapping  | ❌ |   ✅   | Python-specific (WorkflowExecutor) |
  | Input and Output Types         | ✅ |   ✅   |                                    |
  | Output Behavior                | ✅ |   ✅   |                                    |
  | Requests and Responses         | ✅ |   ✅   |                                    |
  | How It Works                   | ✅ |   ✅   |                                    |
  | Multi-Level Nesting            | ✅ |   ✅   |                                    |
  | Error Handling                 | ✅ |   ✅   |                                    |
  | Checkpointing                  | ✅ |   ✅   |                                    |
-->

# Sub-Workflows

A sub-workflow is a complete workflow that runs as an executor within a parent workflow. This enables you to compose complex systems from smaller, reusable workflow building blocks — each with its own isolated execution context, state management, and message routing.

## Overview

Sub-workflows are useful when you want to:

- **Decompose complexity** — break a large workflow into smaller, independently testable units.
- **Reuse workflow logic** — embed the same sub-workflow in multiple parent workflows.
- **Isolate state** — keep each sub-workflow's internal state separate from the parent.
- **Control data flow** — messages enter and leave the sub-workflow only through its edges, with no broadcasting across levels.

When a sub-workflow is added to a parent workflow, it behaves like any other executor: it receives input messages, runs its internal graph to completion, and produces output messages for downstream executors.

::: zone pivot="programming-language-csharp"

## Creating a Sub-Workflow

In C#, you compose sub-workflows in two ways:

- **Direct binding** — use `BindAsExecutor()` to embed a workflow directly as an executor in the parent workflow. This preserves the sub-workflow's native input/output types.
- **Agent wrapping** — use `AsAIAgent()` to convert a workflow into an agent, then add the agent to the parent workflow. This is useful when the parent workflow uses agent-based executors.

### Direct Binding with BindAsExecutor

The `BindAsExecutor()` extension method converts a workflow into an `ExecutorBinding` that can be added directly to a parent workflow:

```csharp
using Microsoft.Agents.AI.Workflows;

// Create executors for the inner workflow
UppercaseExecutor uppercase = new();
ReverseExecutor reverse = new();
AppendSuffixExecutor append = new(" [PROCESSED]");

// Build the inner workflow
var innerWorkflow = new WorkflowBuilder(uppercase)
    .AddEdge(uppercase, reverse)
    .AddEdge(reverse, append)
    .WithOutputFrom(append)
    .Build();

// Bind the inner workflow as an executor
ExecutorBinding subWorkflowExecutor = innerWorkflow.BindAsExecutor("TextProcessingSubWorkflow");

// Build the parent workflow using the sub-workflow executor
PrefixExecutor prefix = new("INPUT: ");
PostProcessExecutor postProcess = new();

var parentWorkflow = new WorkflowBuilder(prefix)
    .AddEdge(prefix, subWorkflowExecutor)
    .AddEdge(subWorkflowExecutor, postProcess)
    .WithOutputFrom(postProcess)
    .Build();
```

With `BindAsExecutor`, the sub-workflow's typed input and output types are preserved — the parent workflow routes messages based on the actual types the sub-workflow expects and produces.

### Agent Wrapping with AsAIAgent

When the parent workflow uses agent-based executors, convert the inner workflow to an agent using `AsAIAgent()`. The `WorkflowBuilder` automatically wraps the agent in an executor:

```csharp
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

// Create agents for the inner workflow
AIAgent specialist1 = chatClient.AsAIAgent("You are specialist 1. Analyze the data.");
AIAgent specialist2 = chatClient.AsAIAgent("You are specialist 2. Validate the analysis.");

// Build the inner workflow
var innerWorkflow = new WorkflowBuilder(specialist1)
    .AddEdge(specialist1, specialist2)
    .Build();

// Convert the inner workflow to an agent
AIAgent innerWorkflowAgent = innerWorkflow.AsAIAgent(
    id: "analysis-pipeline",
    name: "Analysis Pipeline",
    description: "A sub-workflow that analyzes and validates data"
);

// Create agents for the parent workflow
AIAgent coordinator = chatClient.AsAIAgent("You are a coordinator. Delegate tasks to the team.");
AIAgent reviewer = chatClient.AsAIAgent("You are a reviewer. Review the final output.");

// Build the parent workflow with the sub-workflow
var parentWorkflow = new WorkflowBuilder(coordinator)
    .AddEdge(coordinator, innerWorkflowAgent)
    .AddEdge(innerWorkflowAgent, reviewer)
    .Build();
```

The inner workflow runs as a single step from the parent workflow's perspective. The coordinator sends messages to the analysis pipeline, which internally runs `specialist1 → specialist2`, and then forwards the result to the reviewer.

> [!TIP]
> Use `BindAsExecutor()` when working with typed executors and `AsAIAgent()` when working with agent-based workflows. For details on configuring the workflow-to-agent conversion, see [Workflows as Agents](../as-agents.md).

## Input and Output Types

When a workflow is used as a sub-workflow, it preserves the type contracts of its internal executors.

With `BindAsExecutor`, the sub-workflow executor accepts the same input types as the inner workflow's start executor, and sends the same output types that the inner workflow produces. The parent workflow's edges must connect executors whose output types match the sub-workflow's expected input types, and the sub-workflow's output types must match downstream executors' expected inputs.

With `AsAIAgent`, the sub-workflow is wrapped as an agent and follows the [Agent Executor](./agent-executor.md) input/output contracts (`string`, `ChatMessage`, `IEnumerable<ChatMessage>`).

## Output Behavior

By default, when a sub-workflow produces outputs (via `YieldOutputAsync`), those outputs are forwarded as messages to connected executors in the parent workflow. This enables downstream executors to process sub-workflow results.

The `ExecutorOptions` class controls this behavior:

| Option | Default | Description |
|--------|---------|-------------|
| `AutoSendMessageHandlerResultObject` | `true` | Forward sub-workflow outputs as messages to connected executors in the parent graph. |
| `AutoYieldOutputHandlerResultObject` | `false` | Yield sub-workflow outputs directly to the parent workflow's output event stream. |

When `AutoYieldOutputHandlerResultObject` is enabled, sub-workflow outputs bypass the parent's internal routing and are delivered directly to the caller of the parent workflow.

```csharp
var options = new ExecutorOptions
{
    AutoYieldOutputHandlerResultObject = true,
};

ExecutorBinding subWorkflowExecutor = innerWorkflow.BindAsExecutor("SubWorkflow", options);
```

## Requests and Responses

Sub-workflows fully support the [request and response](../human-in-the-loop.md) mechanism. When an executor inside the sub-workflow sends a request (for example, to request human input), the `WorkflowHostExecutor` forwards the `RequestInfoEvent` to the parent workflow with a **qualified port ID** — the sub-workflow executor's ID is prepended to the port ID (for example, `SubWorkflow.GuessNumber`).

This qualification ensures that when the parent workflow receives a response, it can route the response back to the correct sub-workflow instance. The parent workflow handles sub-workflow requests using the same response mechanism as any other request:

```csharp
await using StreamingRun handle = await InProcessExecution.RunStreamingAsync(parentWorkflow, input);
await foreach (WorkflowEvent evt in handle.WatchStreamAsync())
{
    switch (evt)
    {
        case RequestInfoEvent requestInfoEvt:
            // The request may originate from the sub-workflow
            // Handle it and send the response back
            var response = requestInfoEvt.Request.CreateResponse(myResponseData);
            await handle.SendResponseAsync(response);
            break;

        case WorkflowOutputEvent outputEvt:
            Console.WriteLine($"Output: {outputEvt.Data}");
            break;
    }
}
```

> [!NOTE]
> From the parent workflow caller's perspective, there is no difference between a request from a top-level executor and a request from a sub-workflow. The framework handles the routing transparently.

## How It Works

When the parent workflow routes a message to the sub-workflow executor:

1. **Input delivery** — the message is forwarded to the inner workflow's start executor. With `BindAsExecutor`, the message type must match the start executor's expected types. With `AsAIAgent`, messages are normalized to `ChatMessage` format.
2. **Inner execution** — the inner workflow runs its own superstep loop.
3. **Output collection** — the inner workflow's output events are collected. With `BindAsExecutor`, outputs retain their original types. With `AsAIAgent`, outputs are converted to agent response messages.
4. **Request forwarding** — if the inner workflow has pending requests, they are forwarded to the parent workflow for handling (see [Requests and Responses](#requests-and-responses)).
5. **Downstream dispatch** — the resulting messages are sent to the next executor in the parent workflow.

Because the inner workflow maintains its own execution context, its state is independent from the parent workflow.

> [!TIP]
> For details on configuring the workflow-to-agent conversion, including streaming behavior and exception handling, see [Workflows as Agents](../as-agents.md).

## Multi-Level Nesting

Sub-workflows can be nested to arbitrary depth. Each level maintains its own execution context:

```csharp
// Level 1: Data preparation pipeline
var dataPipeline = new WorkflowBuilder(fetcher)
    .AddEdge(fetcher, cleaner)
    .Build();

AIAgent dataPipelineAgent = dataPipeline.AsAIAgent(
    id: "data-pipeline",
    name: "Data Pipeline"
);

// Level 2: Analysis pipeline (contains the data pipeline)
var analysisPipeline = new WorkflowBuilder(dataPipelineAgent)
    .AddEdge(dataPipelineAgent, analyzer)
    .Build();

AIAgent analysisPipelineAgent = analysisPipeline.AsAIAgent(
    id: "analysis-pipeline",
    name: "Analysis Pipeline"
);

// Level 3: Top-level orchestration
var topWorkflow = new WorkflowBuilder(coordinator)
    .AddEdge(coordinator, analysisPipelineAgent)
    .AddEdge(analysisPipelineAgent, reporter)
    .Build();
```

> [!NOTE]
> Each nesting level adds execution overhead because the inner workflow runs its own superstep loop. Keep nesting depth reasonable for performance-sensitive scenarios.

## Error Handling

When a sub-workflow fails, the error is propagated to the parent workflow as a `SubworkflowErrorEvent`. The parent workflow can observe these errors through its event stream:

```csharp
await foreach (WorkflowEvent evt in handle.WatchStreamAsync())
{
    if (evt is SubworkflowErrorEvent subError)
    {
        Console.WriteLine($"Sub-workflow '{subError.ExecutorId}' failed: {subError.Data}");
    }
}
```

If the sub-workflow encounters an unhandled exception, the parent workflow's execution continues but the sub-workflow executor stops processing further messages.

## Checkpointing

When a checkpoint is taken on the parent workflow, the sub-workflow agent's session state is serialized as part of the parent executor's checkpoint data. On restore, the session state is deserialized, allowing the parent workflow to resume with the sub-workflow's state intact.

```csharp
CheckpointManager checkpointManager = CheckpointManager.CreateInMemory();

// Run the parent workflow with checkpointing
StreamingRun run = await InProcessExecution
    .RunStreamingAsync(parentWorkflow, input, checkpointManager);

await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
    // Process events, including those from sub-workflows
}

// Resume from a checkpoint
CheckpointInfo checkpoint = run.Checkpoints[^1];
StreamingRun resumedRun = await InProcessExecution
    .ResumeStreamingAsync(parentWorkflow, checkpoint, checkpointManager);
```

::: zone-end

::: zone pivot="programming-language-python"

## Creating a Sub-Workflow

In Python, you create a sub-workflow by wrapping a `Workflow` in a `WorkflowExecutor` and adding it to a parent workflow.

```python
from agent_framework import WorkflowBuilder, WorkflowExecutor

# Create agents for the inner workflow
specialist1 = client.as_agent(name="Specialist1", instructions="Analyze the data.")
specialist2 = client.as_agent(name="Specialist2", instructions="Validate the analysis.")

# Build the inner workflow
inner_workflow = (
    WorkflowBuilder(start_executor=specialist1)
    .add_edge(specialist1, specialist2)
    .build()
)

# Wrap as an executor
inner_workflow_executor = WorkflowExecutor(
    workflow=inner_workflow,
    id="analysis-pipeline",
)

# Create agents for the parent workflow
coordinator = client.as_agent(name="Coordinator", instructions="Delegate tasks to the team.")
reviewer = client.as_agent(name="Reviewer", instructions="Review the final output.")

# Build the parent workflow with the sub-workflow
parent_workflow = (
    WorkflowBuilder(start_executor=coordinator)
    .add_edge(coordinator, inner_workflow_executor)
    .add_edge(inner_workflow_executor, reviewer)
    .build()
)
```

The inner workflow runs as a single step from the parent workflow's perspective. The coordinator sends messages to the analysis pipeline, which internally runs `specialist1 → specialist2`, and then forwards the result to the reviewer.

### WorkflowExecutor Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `workflow` | `Workflow` | — | The workflow instance to wrap as an executor. |
| `id` | `str` | — | Unique identifier for this executor. |
| `allow_direct_output` | `bool` | `False` | When `True`, sub-workflow outputs are yielded directly to the parent workflow's event stream instead of being sent as messages to connected executors. |
| `propagate_request` | `bool` | `False` | When `True`, requests from the sub-workflow are propagated to the parent workflow's event stream as regular request info events. When `False`, requests are wrapped in `SubWorkflowRequestMessage` for interception by parent executors. |

## Implicit vs Explicit Wrapping

The `WorkflowBuilder` can automatically wrap `Workflow` instances in a `WorkflowExecutor` when you pass them directly. This is similar to how `Agent` instances are automatically wrapped in `AgentExecutor`.

```python
# Implicit wrapping — WorkflowBuilder detects the Workflow and wraps it
parent_workflow = (
    WorkflowBuilder(start_executor=coordinator)
    .add_edge(coordinator, inner_workflow)    # Workflow auto-wrapped
    .add_edge(inner_workflow, reviewer)
    .build()
)
```

Use explicit wrapping when you need to:

- Assign a specific executor ID for reference in multiple edges.
- Reuse the same `WorkflowExecutor` instance across the graph.

```python
# Explicit wrapping — create the WorkflowExecutor yourself
inner_workflow_executor = WorkflowExecutor(
    workflow=inner_workflow,
    id="analysis-pipeline",
)

parent_workflow = (
    WorkflowBuilder(start_executor=coordinator)
    .add_edge(coordinator, inner_workflow_executor)
    .add_edge(inner_workflow_executor, reviewer)
    .build()
)
```

## Input and Output Types

The `WorkflowExecutor` inherits its type signature from the wrapped workflow:

- **Input types** match the wrapped workflow's start executor input types (plus `SubWorkflowResponseMessage` for handling responses to forwarded requests).
- **Output types** match the wrapped workflow's output types. If any executor in the sub-workflow is request-response capable, `SubWorkflowRequestMessage` is also included as an output type.

This means the parent workflow's edges must connect executors whose output types match the sub-workflow's expected input types. Similarly, downstream executors must accept the types that the sub-workflow produces:

```python
# The sub-workflow's start executor accepts TextProcessingRequest
# So the parent executor must send TextProcessingRequest
class Orchestrator(Executor):
    @handler
    async def start(self, texts: list[str], ctx: WorkflowContext[TextProcessingRequest]) -> None:
        for text in texts:
            await ctx.send_message(TextProcessingRequest(text=text))

# The sub-workflow yields TextProcessingResult
# So the downstream executor must handle TextProcessingResult
class ResultCollector(Executor):
    @handler
    async def collect(self, result: TextProcessingResult, ctx: WorkflowContext) -> None:
        print(f"Received: {result}")
```

## Output Behavior

By default (`allow_direct_output=False`), when a sub-workflow produces outputs via `yield_output`, those outputs are forwarded as messages to connected executors in the parent workflow using `send_message`. This enables downstream executors to process sub-workflow results as part of the parent graph.

When `allow_direct_output=True`, sub-workflow outputs are yielded directly to the parent workflow's event stream. The outputs of the sub-workflow become outputs of the parent workflow, bypassing the parent's internal executor routing:

```python
# Outputs go directly to parent's event stream
sub_workflow_executor = WorkflowExecutor(
    workflow=inner_workflow,
    id="analysis-pipeline",
    allow_direct_output=True,
)

# The caller receives sub-workflow outputs directly
async for event in parent_workflow.run(input_data, stream=True):
    if event.type == "output":
        # This output came from the sub-workflow
        print(event.data)
```

## Requests and Responses

Sub-workflows fully support the [request and response](../human-in-the-loop.md) mechanism. When an executor inside a sub-workflow calls `ctx.request_info()`, the `WorkflowExecutor` intercepts the request and handles it based on the `propagate_request` setting.

### Intercepting Requests in the Parent Workflow (Default)

With `propagate_request=False` (the default), requests from the sub-workflow are wrapped in a `SubWorkflowRequestMessage` and sent to connected executors in the parent workflow. This allows parent executors to handle the request locally:

```python
from agent_framework import (
    SubWorkflowRequestMessage,
    SubWorkflowResponseMessage,
)


class ParentHandler(Executor):
    @handler
    async def handle_request(
        self,
        request: SubWorkflowRequestMessage,
        ctx: WorkflowContext[SubWorkflowResponseMessage],
    ) -> None:
        # Inspect the original request from the sub-workflow
        original_data = request.source_event.data

        # Create and send a response back to the sub-workflow
        response = request.create_response(my_response_data)
        await ctx.send_message(response, target_id=request.executor_id)
```

The `create_response()` method validates that the response data type matches the expected type from the original request. If the types don't match, a `TypeError` is raised.

> [!IMPORTANT]
> When sending the response back, use `target_id=request.executor_id` to route the `SubWorkflowResponseMessage` to the correct `WorkflowExecutor` instance.

### Propagating Requests to External Callers

With `propagate_request=True`, requests from the sub-workflow are propagated to the parent workflow's event stream using the standard `request_info` mechanism. The parent workflow's caller handles these requests the same way as any other human-in-the-loop request:

```python
sub_workflow_executor = WorkflowExecutor(
    workflow=inner_workflow,
    id="analysis-pipeline",
    propagate_request=True,
)

# Run the parent workflow and handle propagated requests
result = await parent_workflow.run(input_data)
request_info_events = result.get_request_info_events()
if request_info_events:
    responses = {}
    for event in request_info_events:
        # Handle each request (e.g., ask a human)
        responses[event.request_id] = get_human_response(event.data)
    result = await parent_workflow.run(responses=responses)
```

## How It Works

When the parent workflow routes a message to the `WorkflowExecutor`:

1. **Input delivery** — the message is forwarded to the inner workflow's start executor. The message type must match the start executor's expected input types.
2. **Inner execution** — the inner workflow runs its own superstep loop to completion, or until it needs external input.
3. **Output collection** — the inner workflow's output events are collected and forwarded based on the `allow_direct_output` setting.
4. **Request forwarding** — if the inner workflow has pending requests, they are forwarded based on the `propagate_request` setting (see [Requests and Responses](#requests-and-responses)).
5. **Response accumulation** — the `WorkflowExecutor` collects responses and resumes the sub-workflow only when all expected responses for a given execution have been received.
6. **Downstream dispatch** — outputs are sent to the next executor in the parent workflow.

The sub-workflow maintains its own internal state independently from the parent. Messages are routed only through the edges connecting the `WorkflowExecutor` to the rest of the parent graph — there is no message broadcasting across nesting levels.

## Multi-Level Nesting

Sub-workflows can be nested to arbitrary depth. Each level maintains its own execution context:

```python
# Level 1: Data preparation pipeline
data_pipeline = (
    WorkflowBuilder(start_executor=fetcher)
    .add_edge(fetcher, cleaner)
    .build()
)

# Level 2: Analysis pipeline (contains the data pipeline)
analysis_pipeline = (
    WorkflowBuilder(start_executor=data_pipeline)  # Implicit wrapping
    .add_edge(data_pipeline, analyzer)
    .build()
)

# Level 3: Top-level orchestration
top_workflow = (
    WorkflowBuilder(start_executor=coordinator)
    .add_edge(coordinator, analysis_pipeline)       # Implicit wrapping
    .add_edge(analysis_pipeline, reporter)
    .build()
)
```

> [!NOTE]
> Each nesting level adds execution overhead because the inner workflow runs its own superstep loop. Keep nesting depth reasonable for performance-sensitive scenarios.

> [!WARNING]
> All concurrent executions of a `WorkflowExecutor` share the same underlying workflow instance. Executors inside the sub-workflow should be stateless to avoid interference between concurrent executions.

## Error Handling

When a sub-workflow fails, the error is propagated to the parent workflow. The `WorkflowExecutor` captures the failed event from the sub-workflow and converts it into an error event in the parent context:

```python
async for event in parent_workflow.run(input_data, stream=True):
    if event.type == "failed":
        print(f"Sub-workflow failed: {event.details.message}")
    elif event.type == "output":
        print(event.data)
```

If the sub-workflow encounters an unhandled exception, the parent workflow receives an error event with the exception details, including the sub-workflow's ID.

## Checkpointing

Sub-workflows support checkpointing. When a checkpoint is taken on the parent workflow, the `WorkflowExecutor` serializes its internal state, including the inner workflow's execution progress and any cached messages. On restore, this state is deserialized, allowing the parent workflow to resume with the sub-workflow intact.

```python
from agent_framework import FileCheckpointStorage, WorkflowBuilder

checkpoint_storage = FileCheckpointStorage(storage_path="./checkpoints")

# Build the parent workflow with checkpointing
parent_workflow = (
    WorkflowBuilder(
        start_executor=coordinator,
        checkpoint_storage=checkpoint_storage,
    )
    .add_edge(coordinator, inner_workflow_executor)
    .add_edge(inner_workflow_executor, reviewer)
    .build()
)

# Run with automatic checkpointing
async for event in parent_workflow.run("Analyze the dataset", stream=True):
    if event.type == "output":
        print(event.data)

# Resume from a checkpoint
checkpoints = await checkpoint_storage.list_checkpoints()
async for event in parent_workflow.run(
    checkpoint_id=checkpoints[-1].checkpoint_id,
    checkpoint_storage=checkpoint_storage,
    stream=True,
):
    if event.type == "output":
        print(event.data)
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Workflows as Agents](../as-agents.md)
