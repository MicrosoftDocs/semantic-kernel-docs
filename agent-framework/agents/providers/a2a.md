---
title: A2A
description: Learn how to connect to remote A2A agents using the A2AAgent in Microsoft Agent Framework.
zone_pivot_groups: programming-languages
author: sergeymenshykh
ms.topic: reference
ms.author: semenshi
ms.date: 04/22/2026
ms.service: agent-framework
---

# A2A Protocol

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

> [!NOTE]
> Documentation for Python A2A agents is coming soon.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Custom Provider](./custom.md)
