---
title: A2A Agent
description: Learn how to connect to remote A2A agents using the A2AAgent in Microsoft Agent Framework.
zone_pivot_groups: programming-languages
author: sergeymenshykh
ms.topic: reference
ms.author: semenshi
ms.date: 04/22/2026
ms.service: agent-framework
---

# A2A Agent

The `A2AAgent` enables your application to connect to remote agents that are exposed via the [Agent-to-Agent (A2A) protocol](https://a2a-protocol.org/latest/). It wraps any A2A-compliant endpoint as a standard `AIAgent`, so you can use familiar methods like `RunAsync` and `RunStreamingAsync` to interact with remote agents regardless of what framework or technology they were built with.

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet package to your project:

```dotnetcli
dotnet add package Microsoft.Agents.AI.A2A --prerelease
```

## Agent Discovery

Before communicating with a remote A2A agent, you need to discover it and create an `AIAgent` instance. The A2A protocol defines three [discovery strategies](https://a2a-protocol.org/latest/topics/agent-discovery/), each supported by the Agent Framework.

### Well-Known URI

A2A agents can make their [Agent Card](https://a2a-protocol.org/latest/specification/#5-agent-discovery-the-agent-card) discoverable at a standardized path: `https://{domain}/.well-known/agent-card.json`. Use the `A2ACardResolver` to fetch the card and create an agent in a single call:

```csharp
using A2A;
using Microsoft.Agents.AI;

// Initialize a resolver pointing at the remote agent's host.
A2ACardResolver resolver = new(new Uri("https://a2a-agent.example.com"));

// Resolve the agent card and create an AIAgent in one step.
AIAgent agent = await resolver.GetAIAgentAsync();

// Use the agent.
Console.WriteLine(await agent.RunAsync("Hello!"));
```

> [!TIP]
> `GetAIAgentAsync` also accepts an optional `A2AClientOptions` parameter for [protocol selection](#protocol-selection).

### Catalog-Based Discovery

In enterprise environments or public marketplaces, Agent Cards are often managed by a central registry. If you already have an `AgentCard` obtained from such a registry, convert it directly to an `AIAgent`:

```csharp
using A2A;
using Microsoft.Agents.AI;

// Assume agentCard was retrieved from a registry or catalog.
AgentCard agentCard = await GetAgentCardFromRegistryAsync("travel-planner");

AIAgent agent = agentCard.AsAIAgent();

Console.WriteLine(await agent.RunAsync("Plan a trip to Paris."));
```

### Direct Configuration

For tightly coupled systems or development scenarios where the agent endpoint is known ahead of time, create an `A2AClient` directly and convert it to an `AIAgent`:

```csharp
using A2A;
using Microsoft.Agents.AI;

// Create a client pointing at the known agent endpoint.
A2AClient a2aClient = new(new Uri("https://a2a-agent.example.com"));

AIAgent agent = a2aClient.AsAIAgent(name: "my-agent", description: "A helpful assistant.");

Console.WriteLine(await agent.RunAsync("What can you help me with?"));
```

## Protocol Selection

A2A agents can expose multiple protocol bindings such as HTTP+JSON and JSON-RPC. By default, HTTP+JSON is preferred over JSON-RPC. Use `A2AClientOptions.PreferredBindings` to explicitly control which protocol binding is used:

> [!NOTE]
> The remote A2A agent must be available at an endpoint that supports the selected protocol binding.

```csharp
using A2A;
using Microsoft.Agents.AI;

A2ACardResolver agentCardResolver = new(new Uri("https://a2a-agent.example.com"));

AgentCard agentCard = await agentCardResolver.GetAgentCardAsync();

// Prefer HTTP+JSON protocol binding. For JSON-RPC, set PreferredBindings = [ProtocolBindingNames.JsonRpc]
A2AClientOptions options = new()
{
    PreferredBindings = [ProtocolBindingNames.HttpJson]
};

AIAgent agent = agentCard.AsAIAgent(options: options);

Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));
```

## Streaming

A2A supports streaming responses via Server-Sent Events. Use `RunStreamingAsync` to receive updates in real time as the remote agent processes the request:

```csharp
using A2A;
using Microsoft.Agents.AI;

A2ACardResolver resolver = new(new Uri("https://a2a-agent.example.com"));
AIAgent agent = await resolver.GetAIAgentAsync();

await foreach (var update in agent.RunStreamingAsync("Write a short story about a robot."))
{
    if (!string.IsNullOrEmpty(update.Text))
    {
        Console.Write(update.Text);
    }
}
```

## Background Responses

A2A agents support [background responses](../background-responses.md) for handling long-running operations. When a remote A2A agent returns a task instead of an immediate message, the Agent Framework provides a continuation token that you can use to poll for results or reconnect to interrupted streams.

### Polling for Task Completion

For non-streaming scenarios, use `AllowBackgroundResponses` to receive a continuation token and poll until the task completes:

```csharp
using A2A;
using Microsoft.Agents.AI;

A2ACardResolver resolver = new(new Uri("https://a2a-agent.example.com"));
AIAgent agent = await resolver.GetAIAgentAsync();

AgentSession session = await agent.CreateSessionAsync();

// AllowBackgroundResponses must be true so the server returns immediately with a continuation token
// instead of blocking until the task is complete.
AgentRunOptions options = new() { AllowBackgroundResponses = true };

// Start the initial run with a long-running task.
AgentResponse response = await agent.RunAsync(
    "Conduct a comprehensive analysis of quantum computing applications in cryptography.",
    session,
    options: options);

// Poll until the response is complete.
while (response.ContinuationToken is { } token)
{
    // Wait before polling again.
    await Task.Delay(TimeSpan.FromSeconds(2));

    // Continue with the token.
    response = await agent.RunAsync(session, options: new AgentRunOptions { ContinuationToken = token });
}

Console.WriteLine(response);
```

### Stream Reconnection

In streaming scenarios, each update may include a continuation token. If the stream is interrupted, use the token to reconnect and obtain the response stream from the beginning:

```csharp
using A2A;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

A2ACardResolver resolver = new(new Uri("https://a2a-agent.example.com"));
AIAgent agent = await resolver.GetAIAgentAsync();

AgentSession session = await agent.CreateSessionAsync();

ResponseContinuationToken? continuationToken = null;

await foreach (var update in agent.RunStreamingAsync(
    "Conduct a comprehensive analysis of quantum computing applications in cryptography.",
    session))
{
    // Save the continuation token to reconnect later if the stream is interrupted.
    // Continuation tokens are only returned for long-running tasks. If the A2A agent
    // returns a message instead of a task, the continuation token will not be initialized.
    if (update.ContinuationToken is { } token)
    {
        continuationToken = token;
    }
}

// If the stream was interrupted and a continuation token was captured,
// reconnect to the response stream using the saved continuation token.
if (continuationToken is not null)
{
    await foreach (var update in agent.RunStreamingAsync(
        session,
        options: new() { ContinuationToken = continuationToken }))
    {
        if (!string.IsNullOrEmpty(update.Text))
        {
            Console.WriteLine(update.Text);
        }
    }
}
```

> [!NOTE]
> A2A agents support stream reconnection (obtaining the same response stream from the beginning), not stream resumption from a specific point in the stream.

::: zone-end

::: zone pivot="programming-language-python"

## Getting Started

Install the A2A package:

```bash
pip install agent-framework-a2a --pre
```

## Initialization

`A2AAgent` can be initialized in three ways depending on how much you know about the remote agent ahead of time.

### Direct URL

For development or tightly coupled systems where the endpoint is known:

```python
from agent_framework.a2a import A2AAgent

async with A2AAgent(name="remote", url="https://a2a-agent.example.com") as agent:
    response = await agent.run("Hello!")
    print(response.messages[0].text)
```

When only a URL is provided, `A2AAgent` creates a minimal agent card internally and connects using JSON-RPC.

### Agent Card

If you have an `AgentCard` from a registry or catalog, pass it directly:

```python
from agent_framework.a2a import A2AAgent

async with A2AAgent(agent_card=agent_card) as agent:
    response = await agent.run("Plan a trip to Paris.")
    print(response.messages[0].text)
```

When an `AgentCard` is provided, `A2AAgent` defaults `name` and `description` from the card. It negotiates transport using the card's `supported_interfaces`.

### Well-Known URI (A2ACardResolver)

Use `A2ACardResolver` from the `a2a-sdk` to discover the remote agent at the standard well-known path (`/.well-known/agent.json`):

```python
import httpx
from a2a.client import A2ACardResolver
from agent_framework.a2a import A2AAgent

async with httpx.AsyncClient(timeout=60.0) as http_client:
    resolver = A2ACardResolver(httpx_client=http_client, base_url="https://a2a-agent.example.com")
    agent_card = await resolver.get_agent_card()

async with A2AAgent(agent_card=agent_card) as agent:
    response = await agent.run("What can you help me with?")
    print(response.messages[0].text)
```

## Streaming

Use `stream=True` to receive updates in real time as the remote agent processes the request:

```python
from agent_framework.a2a import A2AAgent

async with A2AAgent(name="remote", url="https://a2a-agent.example.com") as agent:
    async with agent.run("Write a short story about a robot.", stream=True) as stream:
        async for update in stream:
            for content in update.contents:
                if content.text:
                    print(content.text, end="", flush=True)

        final = await stream.get_final_response()
        print(f"\n({len(final.messages)} message(s))")
```

## Long-Running Tasks

By default, `A2AAgent` waits for the remote agent to finish before returning. For long-running tasks, set `background=True` to surface a continuation token you can use to poll or subscribe later:

```python
from agent_framework.a2a import A2AAgent

async with A2AAgent(name="worker", url="https://a2a-agent.example.com") as agent:
    # Start a long-running task
    response = await agent.run("Process this large dataset", background=True)

    if response.continuation_token:
        # Poll for completion later
        result = await agent.poll_task(response.continuation_token)
        print(result)
```

You can also resubscribe to the SSE stream instead of polling:

```python
# Resubscribe to the task's event stream
response = await agent.run(continuation_token=response.continuation_token)
```

## Conversation Identity (context_id)

When you call `A2AAgent.run()` with an `AgentSession`, the agent automatically derives the A2A `context_id` from `session.service_session_id` if the outgoing message does not already carry one. This lets you maintain conversation continuity across multiple A2A calls:

```python
from agent_framework import AgentSession
from agent_framework.a2a import A2AAgent

async with A2AAgent(name="remote", url="https://a2a-agent.example.com") as agent:
    session = AgentSession(service_session_id="my-conversation-1")

    # context_id is automatically set to "my-conversation-1"
    response = await agent.run("Hello!", session=session)

    # Subsequent calls with the same session continue the conversation
    response = await agent.run("Follow-up question", session=session)
```

If a message has an explicit `context_id` in its `additional_properties`, that value takes precedence over the session-derived fallback.

## Authentication

Use an `AuthInterceptor` for secured A2A endpoints:

```python
from a2a.client.auth.interceptor import AuthInterceptor
from agent_framework.a2a import A2AAgent

class BearerAuth(AuthInterceptor):
    def __init__(self, token: str):
        self.token = token

    async def intercept(self, request):
        request.headers["Authorization"] = f"Bearer {self.token}"
        return request

async with A2AAgent(
    name="secure-agent",
    url="https://secure-a2a-agent.example.com",
    auth_interceptor=BearerAuth("your-token"),
) as agent:
    response = await agent.run("Hello!")
```

## Timeout Configuration

`A2AAgent` accepts a `timeout` parameter for controlling request timeouts:

```python
import httpx
from agent_framework.a2a import A2AAgent

# Simple timeout (applies to all components)
async with A2AAgent(name="remote", url="https://example.com", timeout=120.0) as agent:
    ...

# Fine-grained timeout
async with A2AAgent(
    name="remote",
    url="https://example.com",
    timeout=httpx.Timeout(connect=10.0, read=120.0, write=10.0, pool=5.0),
) as agent:
    ...
```

When no timeout is specified, the defaults are: 10s connect, 60s read, 10s write, 5s pool.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Custom Provider](./custom.md)
