---
title: Using function tools with an agent
description: Learn how to use function tools with an agent
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 02/17/2026
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
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new DefaultAzureCredential())
     .GetChatClient("gpt-4o-mini")
     .AsAIAgent(instructions: "You are a helpful assistant", tools: [AIFunctionFactory.Create(GetWeather)]);
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

### Create declaration-only tools

If a tool is implemented outside the framework (for example, client-side in a UI), you can declare it without an implementation using `FunctionTool(..., func=None)`.
The model can still reason about and call the tool, and your application can provide the result later.

:::code language="python" source="~/../agent-framework-code/python/samples/03-workflows/human-in-the-loop/agents_with_declaration_only_tools.py" range="33-46":::

When creating the agent, you can now provide the function tool to the agent, by passing it to the `tools` parameter.

```python
import asyncio
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity import AzureCliCredential

agent = AzureOpenAIChatClient(credential=AzureCliCredential()).as_agent(
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

You can also create a class that contains multiple function tools as methods.
This can be useful for organizing related functions together or when you want to pass state between them.

```python

class WeatherTools:
    def __init__(self):
        self.last_location = None

    def get_weather(
        self,
        location: Annotated[str, Field(description="The location to get the weather for.")],
    ) -> str:
        """Get the weather for a given location."""
        return f"The weather in {location} is cloudy with a high of 15°C."

    def get_weather_details(self) -> int:
        """Get the detailed weather for the last requested location."""
        if self.last_location is None:
            return "No location specified yet."
        return f"The detailed weather in {self.last_location} is cloudy with a high of 15°C, low of 7°C, and 60% humidity."

```

When creating the agent, you can now provide all the methods of the class as functions:

```python
tools = WeatherTools()
agent = AzureOpenAIChatClient(credential=AzureCliCredential()).as_agent(
    instructions="You are a helpful assistant",
    tools=[tools.get_weather, tools.get_weather_details]
)
```

You can also decorate the functions with the same `@tool` decorator as before.

### Complete example

```python
# Copyright (c) Microsoft. All rights reserved.

import asyncio
from typing import Annotated, Any

from agent_framework import tool
from agent_framework.openai import OpenAIResponsesClient
from pydantic import Field

"""
AI Function with kwargs Example

This example demonstrates how to inject custom keyword arguments (kwargs) into an AI function
from the agent's run method, without exposing them to the AI model.

This is useful for passing runtime information like access tokens, user IDs, or
request-specific context that the tool needs but the model shouldn't know about
or provide.
"""


# Define the function tool with **kwargs to accept injected arguments
# NOTE: approval_mode="never_require" is for sample brevity. Use "always_require" in production; see samples/02-agents/tools/function_tool_with_approval.py and samples/02-agents/tools/function_tool_with_approval_and_sessions.py.
@tool(approval_mode="never_require")
def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
    **kwargs: Any,
) -> str:
    """Get the weather for a given location."""
    # Extract the injected argument from kwargs
    user_id = kwargs.get("user_id", "unknown")

    # Simulate using the user_id for logging or personalization
    print(f"Getting weather for user: {user_id}")

    return f"The weather in {location} is cloudy with a high of 15°C."


async def main() -> None:
    agent = OpenAIResponsesClient().as_agent(
        name="WeatherAgent",
        instructions="You are a helpful weather assistant.",
        tools=[get_weather],
    )

    # Pass the injected argument when running the agent
    # The 'user_id' kwarg will be passed down to the tool execution via **kwargs
    response = await agent.run("What is the weather like in Amsterdam?", user_id="user_123")

    print(f"Agent: {response.text}")


if __name__ == "__main__":
    asyncio.run(main())
```

```python
# Copyright (c) Microsoft. All rights reserved.

import asyncio
from typing import Annotated, Any

from agent_framework import tool
from agent_framework.openai import OpenAIResponsesClient
from pydantic import Field

"""
AI Function with kwargs Example

This example demonstrates how to inject custom keyword arguments (kwargs) into an AI function
from the agent's run method, without exposing them to the AI model.

This is useful for passing runtime information like access tokens, user IDs, or
request-specific context that the tool needs but the model shouldn't know about
or provide.
"""


# Define the function tool with **kwargs to accept injected arguments
# NOTE: approval_mode="never_require" is for sample brevity. Use "always_require" in production; see samples/02-agents/tools/function_tool_with_approval.py and samples/02-agents/tools/function_tool_with_approval_and_sessions.py.
@tool(approval_mode="never_require")
def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
    **kwargs: Any,
) -> str:
    """Get the weather for a given location."""
    # Extract the injected argument from kwargs
    user_id = kwargs.get("user_id", "unknown")

    # Simulate using the user_id for logging or personalization
    print(f"Getting weather for user: {user_id}")

    return f"The weather in {location} is cloudy with a high of 15°C."


async def main() -> None:
    agent = OpenAIResponsesClient().as_agent(
        name="WeatherAgent",
        instructions="You are a helpful weather assistant.",
        tools=[get_weather],
    )

    # Pass the injected argument when running the agent
    # The 'user_id' kwarg will be passed down to the tool execution via **kwargs
    response = await agent.run("What is the weather like in Amsterdam?", user_id="user_123")

    print(f"Agent: {response.text}")


if __name__ == "__main__":
    asyncio.run(main())
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Using function tools with human in the loop approvals](./tool-approval.md)
