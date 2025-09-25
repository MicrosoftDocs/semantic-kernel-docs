---
title: Custom Agents
description: Learn how to build custom agents with the Microsoft Agent Framework.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/25/2025
ms.service: semantic-kernel
---

# Custom Agents

The Microsoft Agent Framework supports building custom agents by inheriting from the `AIAgent` class and implementing the required methods.

::: zone pivot="programming-language-csharp"

This document shows how to build a simple custom agent that parrots back user input in upper case.
In most cases building your own agent will involve more complex logic and integration with an AI service.

## Getting Started

Add the required NuGet packages to your project.

```powershell
dotnet add package Microsoft.Extensions.AI.Agents
```

## Creating a Custom Agent

### The Agent Thread

To create a custom agent you also need a thread, which is used to keep track of the state
of a single conversation, including message history, and any other state the agent needs to maintain.

To make it easy to get started, you can inherit from various base classes that implement common thread storage mechanisms.

1. `InMemoryAgentThread` - stores the chat history in memory and can be serialized to JSON.
1. `ServiceIdAgentThread` - doesn't store any chat history, but allows you to associate an id with the thread, under which the chat history can be stored externally.

For this example, we will use the `InMemoryAgentThread` as the base class for our custom thread.

```csharp
internal sealed class CustomAgentThread : InMemoryAgentThread
{
    internal CustomAgentThread() : base() { }
    internal CustomAgentThread(JsonElement serializedThreadState, JsonSerializerOptions? jsonSerializerOptions = null)
        : base(serializedThreadState, jsonSerializerOptions) { }
}
```

### The Agent class

Next, we want to create the agent class itself by inheriting from the `AIAgent` class.

```csharp
internal sealed class UpperCaseParrotAgent : AIAgent
{
}
```

### Constructing threads

Threads are always created via two factory methods on the agent class.
This allows for the agent to control how threads are created and deserialized.
Agents can therefore attach any additional state or behaviors needed to the thread when constructed.

Two methods are required to be implemented:

```csharp
    public override AgentThread GetNewThread() => new CustomAgentThread();

    public override AgentThread DeserializeThread(JsonElement serializedThread, JsonSerializerOptions? jsonSerializerOptions = null)
        => new CustomAgentThread(serializedThread, jsonSerializerOptions);
```

### Core agent logic

The core logic of the agent, is to take any input messages, convert their text to upper case, and return them as response messages.

We want to add the following method to contain this logic.
We are cloning the input messages, since various aspects of the input messages have to be modified to be valid response messages.  E.g. the role has to be changed to `Assistant`.

```csharp
    private static IEnumerable<ChatMessage> CloneAndToUpperCase(IEnumerable<ChatMessage> messages, string agentName) => messages.Select(x =>
        {
            var messageClone = x.Clone();
            messageClone.Role = ChatRole.Assistant;
            messageClone.MessageId = Guid.NewGuid().ToString();
            messageClone.AuthorName = agentName;
            messageClone.Contents = x.Contents.Select(c => c is TextContent tc ? new TextContent(tc.Text.ToUpperInvariant())
            {
                AdditionalProperties = tc.AdditionalProperties,
                Annotations = tc.Annotations,
                RawRepresentation = tc.RawRepresentation
            } : c).ToList();
            return messageClone;
        });
```

### Agent run methods

Finally we need to implement the two core methods that are used to run the agent.
One for non-streaming and one for streaming.

For both methods, we need to ensure that a thread is provided, and if not we create a new thread.
The thread can then be updated with the new messages by calling `NotifyThreadOfNewMessagesAsync`.
If we don't do this, the user will not be able to have a multi-turn conversation with the agent and each run will be a fresh interaction.

```csharp
    public override async Task<AgentRunResponse> RunAsync(IEnumerable<ChatMessage> messages, AgentThread? thread = null, AgentRunOptions? options = null, CancellationToken cancellationToken = default)
    {
        thread ??= this.GetNewThread();
        List<ChatMessage> responseMessages = CloneAndToUpperCase(messages, this.DisplayName).ToList();
        await NotifyThreadOfNewMessagesAsync(thread, messages.Concat(responseMessages), cancellationToken);
        return new AgentRunResponse
        {
            AgentId = this.Id,
            ResponseId = Guid.NewGuid().ToString(),
            Messages = responseMessages
        };
    }

    public override async IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(IEnumerable<ChatMessage> messages, AgentThread? thread = null, AgentRunOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        thread ??= this.GetNewThread();
        List<ChatMessage> responseMessages = CloneAndToUpperCase(messages, this.DisplayName).ToList();
        await NotifyThreadOfNewMessagesAsync(thread, messages.Concat(responseMessages), cancellationToken);
        foreach (var message in responseMessages)
        {
            yield return new AgentRunResponseUpdate
            {
                AgentId = this.Id,
                AuthorName = this.DisplayName,
                Role = ChatRole.Assistant,
                Contents = message.Contents,
                ResponseId = Guid.NewGuid().ToString(),
                MessageId = Guid.NewGuid().ToString()
            };
        }
    }
```

::: zone-end
::: zone pivot="programming-language-python"

Documentation coming soon.

::: zone-end

## Using the Agent

If the `AIAgent` methods are all implemented correctly, the agent would be a standard `AIAgent` and support standard agent operations.

See the [Agent getting started tutorials](../../../tutorials/overview.md) for more information on how to run and interact with agents.
