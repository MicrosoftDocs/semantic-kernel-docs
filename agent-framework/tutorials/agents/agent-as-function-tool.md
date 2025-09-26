---
title: Using an agent as a function tool
description: Learn how to use an agent as a function tool
zone_pivot_groups: programming-languages
author: westey-m, dmytrostruk
ms.topic: tutorial
ms.author: westey, dmytrostruk
ms.date: 09/24/2025
ms.service: semantic-kernel
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

This tutorial shows you how to use an agent as a function tool, so that one agent can call another agent as a tool.

## Prerequisites

For prerequisites and installing packages, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Creating and using an agent as a function tool

You can use a `ChatAgent` as a function tool by calling `.as_tool()` on the agent and providing it as a tool to another agent. This allows you to compose agents and build more advanced workflows.

First, create a function tool that will be used by our agent that is exposed as a function.

```python
from typing import Annotated
from pydantic import Field

def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
) -> str:
    """Get the weather for a given location."""
    return f"The weather in {location} is cloudy with a high of 15°C."
```

Create a `ChatAgent` that uses the function tool.

```python
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity import AzureCliCredential

weather_agent = AzureOpenAIChatClient(credential=AzureCliCredential()).create_agent(
    name="WeatherAgent",
    description="An agent that answers questions about the weather.",
    instructions="You answer questions about the weather.",
    tools=get_weather
)
```

Now, create a main agent and provide the `weather_agent` as a function tool by calling `.as_tool()` to convert `weather_agent` to a function tool.

```python
main_agent = AzureOpenAIChatClient(credential=AzureCliCredential()).create_agent(
    instructions="You are a helpful assistant who responds in French.",
    tools=weather_agent.as_tool()
)
```

Invoke the main agent as normal. It can now call the weather agent as a tool, and should respond in French.

```python
result = await main_agent.run("What is the weather like in Amsterdam?")
print(result.text)
```

You can also customize the tool name, description, and argument name when converting an agent to a tool:

```python
# Convert agent to tool with custom parameters
weather_tool = weather_agent.as_tool(
    name="WeatherLookup",
    description="Look up weather information for any location",
    arg_name="query", 
    arg_description="The weather query or location"
)

main_agent = AzureOpenAIChatClient(credential=AzureCliCredential()).create_agent(
    instructions="You are a helpful assistant who responds in French.",
    tools=weather_tool
)
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Exposing an agent as an MCP tool](./agent-as-mcp-tool.md)
