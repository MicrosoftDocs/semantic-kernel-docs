---
title: Adding memory to Semantic Kernel Agents
description: How to add memory to Semantic Kernel Agents
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 05/21/2025
ms.service: semantic-kernel
---

# Using memory with Agents

::: zone pivot="programming-language-csharp"

> [!WARNING]
> The Semantic Kernel Agent Memory functionality is experimental, is subject to change and will only be finalized based on feedback and evaluation.

## Using mem0 for Agent memory

[mem0](https://mem0.ai) is a self-improving memory layer for LLM applications, enabling personalized AI experiences.

The `Microsoft.SemanticKernel.Memory.Mem0Provider` integrates with the mem0 service allowing agents to remember user preferences and context across multiple threads, enabling a seamless user experience.

Each message added to the thread is sent to the mem0 service to extract memories.
For each agent invocation, mem0 is queried for memories matching the provided user request, and any memories are added to the agent context for that invocation.

The mem0 memory provider can be configured with a user id to allow storing memories about the user, long term, across multiple threads.
It can also be configured with a thread id or to use the thread id of the agent thread, to allow for short term memories that are only attached to a single thread.

Here is an example of how to use this component.

```csharp
// Create an HttpClient for the mem0 service.
using var httpClient = new HttpClient()
{
    BaseAddress = new Uri("https://api.mem0.ai")
};
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", "<Your_Mem0_API_Key>");

// Create a mem0 provider for the current user.
var mem0Provider = new Mem0Provider(httpClient, options: new()
{
    UserId = "U1"
});

// Clear any previous memories (optional).
await mem0Provider.ClearStoredMemoriesAsync();

// Add the mem0 provider to the agent thread.
ChatHistoryAgentThread agentThread = new();
agentThread.AIContextProviders.Add(mem0Provider);

// Use the agent with mem0 memory.
ChatMessageContent response = await agent.InvokeAsync("Please retrieve my company report", agentThread).FirstAsync();
Console.WriteLine(response.Content);
```

### Mem0Provider options

The `Mem0Provider` can be configured with various options to customize its behavior.
Options are provided using the `Mem0ProviderOptions` class to the `Mem0Provider` constructor.

#### Scoping Options

mem0 provides the ability to scope memories by Application, Agent, Thread and User.

Options are available to provide ids for these scopes, so that the memories can be stored in mem0 under these ids.
See the `ApplicationId`, `AgentId`, `ThreadId` and `UserId` properties on `Mem0ProviderOptions`.

In some cases you may want to use the thread id of the server side agent thread, when using a service based agent.
The thread may however not have been created yet when the `Mem0Provider` object is being constructed.
In this case, you can set the `ScopeToPerOperationThreadId` option to `true`, and the `Mem0Provider` will
use the id of the `AgentThread` when it is available.

#### Context Prompt

The `ContextPrompt` option allows you to override the default prompt that is prefixed to memories.
The prompt is used to contextualize the memories provided to the AI model, so that the AI model knows what they are and how to use them.

## Using Whiteboard Memory for Short-Term Context

The whiteboard memory feature allows agents to capture and retain the most relevant information from a conversation, even when the chat history is truncated.

Each message added to the conversation is processed by the `Microsoft.SemanticKernel.Memory.WhiteboardProvider` to extract requirements, proposals, decisions, actions.
These are stored on a whiteboard and provided to the agent as additional context on each invocation.

Here is an example of how to set up Whiteboard Memory:

```csharp
// Create a whiteboard provider.
var whiteboardProvider = new WhiteboardProvider(chatClient);

// Add the whiteboard provider to the agent thread.
ChatHistoryAgentThread agentThread = new();
agentThread.AIContextProviders.Add(whiteboardProvider);

// Simulate a conversation with the agent.
await agent.InvokeAsync("I would like to book a trip to Paris.", agentThread);

// Whiteboard should now contain a requirement that the user wants to book a trip to Paris.
```

Benefits of Whiteboard Memory

- Short-Term Context: Retains key information about the goals of ongoing conversations.
- Allows Chat History Truncation: Supports maintaining critical context if the chat history is truncated.

### WhiteboardProvider options

The `WhiteboardProvider` can be configured with various options to customize its behavior.
Options are provided using the `WhiteboardProviderOptions` class to the `WhiteboardProvider` constructor.

#### MaxWhiteboardMessages

Specifies a maximum number of messages to retain on the whiteboard.
When the maximum is reached, less valuable messages will be removed.

#### ContextPrompt

When providing the whiteboard contents to the AI model it's important to describe what the messages are for.
This setting allows overriding the default messaging that is built into the `WhiteboardProvider`.

#### WhiteboardEmptyPrompt

When the whiteboard is empty, the `WhiteboardProvider` outputs a message saying that it is empty.
This setting allows overriding the default messaging that is built into the `WhiteboardProvider`.

#### MaintenancePromptTemplate

The `WhiteboardProvider` uses an AI model to add/update/remove messages on the whiteboard.
It has a built in prompt for making these updates.
This setting allows overriding this built-in prompt.

The following parameters can be used in the template:

- `{{$maxWhiteboardMessages}}`: The maximum number of messages allowed on the whiteboard.
- `{{$inputMessages}}`: The input messages to be added to the whiteboard.
- `{{$currentWhiteboard}}`: The current state of the whiteboard.

## Combining mem0 and Whiteboard Memory

You can use both mem0 and whiteboard memory in the same agent to achieve a balance between long-term and short-term memory capabilities.

```csharp
// Add both mem0 and whiteboard providers to the agent thread.
agentThread.AIContextProviders.Add(mem0Provider);
agentThread.AIContextProviders.Add(whiteboardProvider);

// Use the agent with combined memory capabilities.
ChatMessageContent response = await agent.InvokeAsync("Please retrieve my company report", agentThread).FirstAsync();
Console.WriteLine(response.Content);
```

By combining these memory features, agents can provide a more personalized and context-aware experience for users.

## Next steps

> [!div class="nextstepaction"]
> [Explore the Agent with mem0 sample](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/ChatCompletion_Mem0.cs)
> [Explore the Agent with Whiteboard sample](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/ChatCompletion_Whiteboard.cs)

::: zone-end

::: zone pivot="programming-language-python"

## Coming Soon

More information coming soon.

::: zone-end

::: zone pivot="programming-language-java"

## Coming Soon

More information coming soon.

::: zone-end
