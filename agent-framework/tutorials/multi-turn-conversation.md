---
title: Multi-turn conversations with an agent
description: Learn how to have a multi-turn conversation with an agent
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/15/2025
ms.service: agent-framework
---

# Multi-turn conversations with an agent

::: zone pivot="programming-language-csharp"

This tutorial step shows you how to have a multi-turn conversation with an agent, where the agent is built on the Azure OpenAI Chat Completion service.

> [!IMPORTANT]
> The agent framework supports many different types of agents. This tutorial uses an agent based on a Chat Completion service, but all other agent types are run in the same way. See the [Agent Framework user guide](../user-guide/index.md) for more information on other agent types and how to construct them.

## Prerequisites

For prerequisites, installing nuget packages, and creating the agent, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Running the agent with a multi-turn conversation

Agents are stateless and do not maintain any state internally between calls.
To have a multi-turn conversation with an agent, you need to create an object to hold the conversation state and pass this object to the agent when running it.

To create the conversation state object, call the `GetNewThread` method on the agent instance.

```csharp
AgentThread thread = agent.GetNewThread();
```

You can then pass this thread object to the `RunAsync` and `RunStreamingAsync` methods on the agent instance, along with the user input.

```csharp
Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate.", thread));
Console.WriteLine(await agent.RunAsync("Now add some emojis to the joke and tell it in the voice of a pirate's parrot.", thread));
```

This will maintain the conversation state between the calls, and the agent will be able to refer to previous input and response messages in the conversation when responding to new input.

> [!IMPORTANT]
> The type of service that is used by the `AIAgent` will determine how conversation history is stored. E.g. when using a ChatCompletion service, like in this example, the conversation history is stored in the AgentThread object and sent to the service on each call. When using the Foundry Agent service on the other hand, the conversation history is stored in the Foundry Agent service and only a reference to the conversation is sent to the service on each call.

## Single agent with multiple conversations

It is possible to have multiple, independent conversations with the same agent instance, by creating multiple `AgentThread` objects.
These threads can then be used to maintain separate conversation states for each conversation.
The conversations will be fully independent of each other, since the agent does not maintain any state internally.

```csharp
AgentThread thread1 = agent.GetNewThread();
AgentThread thread2 = agent.GetNewThread();
Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate.", thread1));
Console.WriteLine(await agent.RunAsync("Tell me a joke about a robot.", thread2));
Console.WriteLine(await agent.RunAsync("Now add some emojis to the joke and tell it in the voice of a pirate's parrot.", thread1));
Console.WriteLine(await agent.RunAsync("Now add some emojis to the joke and tell it in the voice of a robot.", thread2));
```

::: zone-end
::: zone pivot="programming-language-python"

Tutorial coming soon.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Using function tools with an agent](./function-tools.md)