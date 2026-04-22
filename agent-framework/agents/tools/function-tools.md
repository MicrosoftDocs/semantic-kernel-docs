---
title: Using function tools with an agent
description: Learn how to use function tools with an agent
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 04/22/2026
ms.service: agent-framework
---

# Using function tools with an agent

This tutorial step shows you how to use function tools with an agent, where the agent is built on the Azure OpenAI Chat Completion service.

::: zone pivot="programming-language-csharp"

> [!IMPORTANT]
> Not all agent types support function tools. Some might only support custom built-in tools, without allowing the caller to provide their own functions. This step uses a `ChatClientAgent`, which does support function tools.

## Prerequisites

For prerequisites and installing NuGet packages, see the [Create and run a simple agent](../running-agents.md) step in this tutorial.

## Create the agent with function tools

Function tools are just custom code that you want the agent to be able to call when needed.
You can turn any C# method into a function tool, by using the `AIFunctionFactory.Create` method to create an `AIFunction` instance from the method.

If you need to provide additional descriptions about the function or its parameters to the agent, so that it can more accurately choose between different functions, you can use the `System.ComponentModel.DescriptionAttribute` attribute on the method and its parameters.

Here is an example of a simple function tool that fakes getting the weather for a given location.
It is decorated with description attributes to provide additional descriptions about itself and its location parameter to the agent.

```csharp
using System.ComponentModel;

[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";
```

When creating the agent, you can now provide the function tool to the agent, by passing a list of tools to the `AsAIAgent` method.

```csharp
using System;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

AIAgent agent = new AIProjectClient(
    new Uri("<your-foundry-project-endpoint>"),
    new DefaultAzureCredential())
     .AsAIAgent(
        model: "gpt-4o-mini",
        instructions: "You are a helpful assistant",
        tools: [AIFunctionFactory.Create(GetWeather)]);
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

Now you can just run the agent as normal, and the agent will be able to call the `GetWeather` function tool when needed.

```csharp
Console.WriteLine(await agent.RunAsync("What is the weather like in Amsterdam?"));
```

> [!TIP]
> See the [.NET samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples) for complete runnable examples.

::: zone-end
::: zone pivot="programming-language-python"

> [!IMPORTANT]
> Not all agent types support function tools. Some might only support custom built-in tools, without allowing the caller to provide their own functions. This step uses agents created via chat clients, which do support function tools.

## Prerequisites

For prerequisites and installing Python packages, see the [Create and run a simple agent](../running-agents.md) step in this tutorial.

## Create the agent with function tools

Function tools are just custom code that you want the agent to be able to call when needed.
You can turn any Python function into a function tool by passing it to the agent's `tools` parameter when creating the agent.

If you need to provide additional descriptions about the function or its parameters to the agent, so that it can more accurately choose between different functions, you can use Python's type annotations with `Annotated` and Pydantic's `Field` to provide descriptions.

Here is an example of a simple function tool that fakes getting the weather for a given location.
It uses type annotations to provide additional descriptions about the function and its location parameter to the agent.

```python
from typing import Annotated
from pydantic import Field

def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
) -> str:
    """Get the weather for a given location."""
    return f"The weather in {location} is cloudy with a high of 15°C."
```

You can also use the `@tool` decorator to explicitly specify the function's name and description:

```python
from typing import Annotated
from pydantic import Field
from agent_framework import tool

@tool(name="weather_tool", description="Retrieves weather information for any location")
def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
) -> str:
    return f"The weather in {location} is cloudy with a high of 15°C."
```

If you don't specify the `name` and `description` parameters in the `@tool` decorator, the framework will automatically use the function's name and docstring as fallbacks.

### Use explicit schemas with `@tool`

When you need full control over the schema exposed to the model, pass the `schema` parameter to `@tool`.
You can provide either a Pydantic model or a raw JSON schema dictionary.

:::code language="python" source="~/../agent-framework-code/python/samples/02-agents/tools/function_tool_with_explicit_schema.py" range="25-41,44-59":::

### Pass runtime-only context to a tool

Use normal function parameters for values the model should supply. Use `FunctionInvocationContext` for runtime-only values such as `function_invocation_kwargs` or the current session. The injected context parameter is hidden from the schema exposed to the model.

:::code language="python" source="~/../agent-framework-code/python/samples/02-agents/tools/function_tool_with_kwargs.py" range="3-9,28-56":::

For more detail on `ctx.kwargs`, `ctx.session`, and function middleware, see [Runtime Context](../middleware/runtime-context.md).

### Create declaration-only tools

If a tool is implemented outside the framework (for example, client-side in a UI), you can declare it without an implementation using `FunctionTool(..., func=None)`.
The model can still reason about and call the tool, and your application can provide the result later.

:::code language="python" source="~/../agent-framework-code/python/samples/03-workflows/human-in-the-loop/agents_with_declaration_only_tools.py" range="33-46":::

When creating the agent, you can now provide the function tool to the agent, by passing it to the `tools` parameter.

```python
import asyncio
import os
from agent_framework.openai import OpenAIChatCompletionClient
from azure.identity import AzureCliCredential

agent = OpenAIChatCompletionClient(
    model=os.environ["AZURE_OPENAI_CHAT_COMPLETION_MODEL"],
    azure_endpoint=os.environ["AZURE_OPENAI_ENDPOINT"],
    api_version=os.getenv("AZURE_OPENAI_API_VERSION"),
    credential=AzureCliCredential(),
).as_agent(
    instructions="You are a helpful assistant",
    tools=get_weather
)
```

Now you can just run the agent as normal, and the agent will be able to call the `get_weather` function tool when needed.

```python
async def main():
    result = await agent.run("What is the weather like in Amsterdam?")
    print(result.text)

asyncio.run(main())
```

## Create a class with multiple function tools

When several tools share dependencies or mutable state, wrap them in a class and pass bound methods to the agent. Use class attributes for values the model should not provide, such as service clients, feature flags, or cached state.

:::code language="python" source="~/../agent-framework-code/python/samples/02-agents/tools/tool_in_class.py" range="3-8,21-68":::

This pattern is a good fit for long-lived tool state. Use `FunctionInvocationContext` instead when the value changes per invocation.

::: zone-end

::: zone pivot="programming-language-go"
## Function tools

Function tools let agents call custom Go functions. The `functool` package provides a simple way to define type-safe tools with automatic schema generation.

### Define a function tool

```go
import (
    "github.com/microsoft/agent-framework-go/tool"
    "github.com/microsoft/agent-framework-go/tool/functool"
)

var weatherTool = functool.MustNew(&functool.Func{
    Name:        "weather",
    Description: "Get the current weather for a given location",
}, func(_ tool.Context, location string) (string, error) {
    return fmt.Sprintf("The weather in %s is cloudy with a high of 15°C.", location), nil
})
```

The function signature determines the tool's input schema. The `tool.Context` parameter is injected by the framework and is not exposed to the model.

### Structured input types

For tools with multiple parameters, define a struct:

```go
type WeatherInput struct {
    Location string `json:"location" jsonschema:"description=The city to check weather for"`
    Unit     string `json:"unit" jsonschema:"description=Temperature unit (celsius or fahrenheit),enum=celsius,enum=fahrenheit"`
}

var weatherTool = functool.MustNew(&functool.Func{
    Name:        "weather",
    Description: "Get weather for a location",
}, func(_ tool.Context, input WeatherInput) (string, error) {
    return fmt.Sprintf("Weather in %s: 15°%s", input.Location, input.Unit), nil
})
```

### Create an agent with tools

```go
a := openaichatagent.New(client, openaichatagent.Config{
    Model: deployment,
    Config: agent.Config{
        Instructions: "You are a helpful assistant.",
        Tools:        []tool.Tool{weatherTool},
    },
})

resp, err := a.RunText(ctx, "What is the weather like in Amsterdam?").Collect()
```

### Use an agent as a function tool

Any agent can be wrapped as a function tool for use by another agent:

```go
weatherAgent := openaichatagent.New(client, openaichatagent.Config{
    Model: deployment,
    Config: agent.Config{
        Instructions: "You answer questions about the weather.",
        Name:         "WeatherAgent",
        Description:  "An agent that answers weather questions.",
        Tools:        []tool.Tool{weatherTool},
    },
})

mainAgent := openaichatagent.New(client, openaichatagent.Config{
    Model: deployment,
    Config: agent.Config{
        Instructions: "You are a helpful assistant who responds in French.",
        Tools:        []tool.Tool{weatherAgent.AsFuncTool()},
    },
})
```

> [!TIP]
> See the [function tools sample](https://github.com/microsoft/agent-framework-go/blob/main/examples/02-agents/agents/step03_using_function_tools/main.go) and the [agent as tool sample](https://github.com/microsoft/agent-framework-go/blob/main/examples/02-agents/agents/step12_as_function_tool/main.go) for complete examples.

::: zone-end
## Next steps

> [!div class="nextstepaction"]
> [Using function tools with human in the loop approvals](./tool-approval.md)
