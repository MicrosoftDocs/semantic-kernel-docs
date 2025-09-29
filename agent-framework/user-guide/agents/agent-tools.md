---
title: Agent Tools
description: Learn how to use tools with Agent Framework
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: reference
ms.author: markwallace
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# Agent Tools

Tooling support may vary considerably between different agent types. Some agents may allow developers to customize the agent at construction time by providing external function tools or by choosing to activate specific built-in tools that are supported by the agent. On the other hand, some custom agents may support no customization via providing external or activating built-in tools, if they already provide defined features that shouldn't be changed.

::: zone pivot="programming-language-csharp"

Therefore, the base abstraction does not provide any direct tooling support, however each agent can choose whether it accepts tooling customization at construction time.

## Tooling support with ChatClientAgent

The `ChatClientAgent` is an agent class that can be used to build agentic capabilities on top of any inference service. It comes with support for:

1. Using your own function tools with the agent
1. Using built-in tools that the underlying service may support.

> [!TIP]
> For more information on `ChatClientAgent` and information on supported services, see [Simple agents based on inference services](./agent-types/index.md#simple-agents-based-on-inference-services)

### Provide `AIFunction` instances during agent construction

There are various ways to construct a `ChatClientAgent`, e.g. directly or via factory helper methods on various service clients, but all support passing tools.

```csharp
// Sample function tool.
[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";

// When calling the ChatClientAgent constructor.
new ChatClientAgent(
    chatClient,
    instructions: "You are a helpful assistant",
    tools: [AIFunctionFactory.Create(GetWeather)]);

// When using one of the helper factory methods.
openAIResponseClient.CreateAIAgent(
    instructions: "You are a helpful assistant",
    tools: [AIFunctionFactory.Create(GetWeather)]);
```

### Provide `AIFunction` instances when running the agent

While the base `AIAgent` abstraction accepts `AgentRunOptions` on its run methods, subclasses of `AIAgent` can accept
subclasses of `AgentRunOptions`. This allows specific agent implementations to accept agent specific per-run options.

The underlying `IChatClient` of the `ChatClientAgent` can be customized via the `ChatOptions` class for any invocation.
The `ChatClientAgent` can accept a `ChatClientAgentRunOptions` which allows the caller to provide `ChatOptions` for the underlying
`IChatClient.GetResponse` method. Where any option clashes with options provided to the agent at construction time, the per run options
will take precedence.

Using this mechanism we can provide per-run tools.

```csharp
// Create the chat options class with the per-run tools.
var chatOptions = new ChatOptions()
{
    Tools = [AIFunctionFactory.Create(GetWeather)]
};
// Run the agent, with the per-run chat options.
await agent.RunAsync(
    "What is the weather like in Amsterdam?",
    options: new ChatClientAgentRunOptions(chatOptions));
```

> [!NOTE]
> Not all agents support tool calling, so providing tools per run requires providing an agent specific options class.

### Using built-in tools

Where the underlying service supports built-in tools, they can be provided using the same mechanisms as described above.

The IChatClient implementation for the underlying service should expose an `AITool` derived class that can be used to
configure the built-in tool.

E.g, when creating an Azure AI Foundry Agent, you can provide a `CodeInterpreterToolDefinition` to enable the code interpreter
tool that is built into the Azure AI Foundry service.

```csharp
var agent = await azureAgentClient.CreateAIAgentAsync(
    deploymentName,
    instructions: "You are a helpful assistant",
    tools: [new CodeInterpreterToolDefinition()]);
```

::: zone-end
::: zone pivot="programming-language-python"

::: zone-end
