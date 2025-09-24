---
title: Using an agent as a function tool
description: Learn how to use an agent as a function tool
author: westey-m
ms.topic: tutorial
ms.date: 09/24/2025
ms.service: semantic-kernel
zone_pivot_groups: programming-languages
---

# Using an agent as a function tool

::: zone pivot="programming-language-csharp"

This tutorial shows you how to use an agent as a function tool, so that one agent can call another agent as a tool.

## Prerequisites

For prerequisites and installing nuget packages, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Creating and using an agent as a function tool

You can use an `AIAgent` as a function tool by calling `.AsAIFunction()` on the agent and providing it as a tool to another agent. This allows you to compose agents and build more advanced workflows.

First, create a function tool as a C# method, and decorate it with descriptions if needed.
This tool will be used by our agent that is exposed as a function.

```csharp
[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";
```

Create an `AIAgent` that uses the function tool.

```csharp
AIAgent weatherAgent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
     .GetChatClient("gpt-4o-mini")
     .CreateAIAgent(
        instructions: "You answer questions about the weather.",
        name: "WeatherAgent",
        description: "An agent that answers questions about the weather.",
        tools: [AIFunctionFactory.Create(GetWeather)]);
```

Now, create a main agent and provide the `weatherAgent` as a function tool by calling `.AsAIFunction()` to convert `weatherAgent` to a function tool.

```csharp
AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
     .GetChatClient("gpt-4o-mini")
     .CreateAIAgent(instructions: "You are a helpful assistant who responds in French.", tools: [weatherAgent.AsAIFunction()]);
```

Invoke the main agent as normal. It can now call the weather agent as a tool, and should respond in French.

```csharp
Console.WriteLine(await agent.RunAsync("What is the weather like in Amsterdam?"));
```

::: zone-end
::: zone pivot="programming-language-python"

Tutorial coming soon.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Exposing an agent as an MCP tool](./agent-as-mcp-tool.md)
