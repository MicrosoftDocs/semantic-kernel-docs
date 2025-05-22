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
> The Semantic Kernel Agent Memory functionality is experimental, is subject to change and will only be graduated based on feedback and evaluation.

## Using mem0 for Agent memory

[mem0](https://mem0.ai) is a self-improving memory layer for LLM applications, enabling personalized AI experiences.

The **mem0** memory provider integrates with the mem0 service allowing agents to remember user preferences and context across multiple threads, enabling a seamless user experience.

Each message added to the thread is passed to the mem0 service to allow it to extract memories.
For each agent invocation mem0 is queried for memories matching the provided user request and any memories are added to the agent context for that invocation.

The **mem0** memory provider can be configured with a user id to allow storing memories about the user, long term, across multiple threads.
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
var mem0Provider = new Mem0Provider(httpClient, new()
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

## Using Whiteboard Memory for Short-Term Context

The whiteboard memory feature allows agents to capture the most relevant information from a conversation, even when the chat history is truncated.

Each message that is added to the converstion will be processed by the `WhiteboardProvider` to extract requriements, proposals, decisions, actions.
These will be kept on a whiteboard that is provided to the agent as additional context on each invocation.

Setting Up Whiteboard Memory

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

- Short-Term Context: Retains key information about the goal of the ongoing conversations.
- Efficient Truncation: Allows chat history to be reduced without losing critical context.

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

::: zone-end

::: zone pivot="programming-language-python"

## Coming Soon

More information coming soon.

::: zone-end

::: zone pivot="programming-language-java"

## Coming Soon

More information coming soon.

::: zone-end
