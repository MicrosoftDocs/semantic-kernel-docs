---
title: Backend Tool Rendering with AG-UI
description: Learn how to add function tools that execute on the backend with results streamed to clients
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 08/11/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                    | C# | Python | Go | Notes |
  |----------------------------|:--:|:------:|:--:|-------|
  | Backend tool registration  | ✅ |   ✅   | ✅ |       |
  | AG-UI tool event mapping   | ✅ |   ✅   | ❌ | Not documented for Go |
  | Complex JSON serialization | ✅ |   ✅   | ❌ | Not documented for Go |
-->

# Backend Tool Rendering with AG-UI

::: zone pivot="programming-language-csharp"

Backend tools use the normal MAF tool pipeline. AG-UI adds transport events so a client can observe the call and result; it doesn't introduce a separate tool abstraction.

## Add a backend tool

Define and register the tool as you would for any MAF agent:

```csharp
using System.ComponentModel;
using Microsoft.Extensions.AI;

[Description("Get the weather for a location.")]
static string GetWeather(
    [Description("The city to look up.")] string location) =>
    $"The weather in {location} is sunny.";

AITool getWeather = AIFunctionFactory.Create(GetWeather, name: "get_weather");
AIAgent agent = chatClient.AsAIAgent(tools: [getWeather]);

app.MapAGUIServer("/", agent);
```

For complex request or response types, configure the same `JsonSerializerOptions` for ASP.NET Core and `AIFunctionFactory.Create`.

> [!TIP]
> See the [.NET backend-tools sample](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/02-agents/AGUI/Step02_BackendTools) for a complete implementation.

For tool schemas, dependency injection, error handling, and general tool design, see [Use function tools with an agent](../../../../agents/tools/function-tools.md).

## AG-UI event mapping

When the agent calls the tool:

- `FunctionCallContent` is emitted as AG-UI `TOOL_CALL_START`, `TOOL_CALL_ARGS`, and `TOOL_CALL_END` events.
- `FunctionResultContent` is emitted as a `TOOL_CALL_RESULT` event.
- Text and other agent content continue to stream normally.

A .NET client receives the translated content as `FunctionCallContent` and `FunctionResultContent`:

```csharp
await foreach (AgentResponseUpdate update in agent.RunStreamingAsync(messages, session))
{
    foreach (AIContent content in update.Contents)
    {
        if (content is FunctionCallContent call)
        {
            Console.WriteLine($"Calling {call.Name}");
        }
        else if (content is FunctionResultContent result)
        {
            Console.WriteLine($"Result: {result.Result}");
        }
    }
}
```

Tool results are model-facing values that AG-UI also exposes to the client. To emit shared UI state in addition to a tool result, use the explicit mappings described in [State management](./state-management.md).

## Next steps

> [!div class="nextstepaction"]
> [Use frontend tools with AG-UI](./frontend-tools.md)

::: zone-end

::: zone pivot="programming-language-python"

This tutorial shows you how to add function tools to your AG-UI agents. Function tools are custom Python functions that the agent can call to perform specific tasks like retrieving data, performing calculations, or interacting with external systems. With AG-UI, these tools execute on the backend and their results are automatically streamed to the client.

## Prerequisites

Before you begin, ensure you have completed the [Getting Started](getting-started.md) tutorial and have:

- Python 3.10 or later
- `agent-framework-ag-ui` installed
- Azure OpenAI service configured
- Basic understanding of AG-UI server and client setup

> [!NOTE]
> These samples use `DefaultAzureCredential` for authentication. Make sure you're authenticated with Azure (e.g., via `az login`). For more information, see the [Azure Identity documentation](/python/api/azure-identity/azure.identity.defaultazurecredential).

## What is Backend Tool Rendering?

Backend tool rendering means:

- Function tools are defined on the server
- The AI agent decides when to call these tools
- Tools execute on the backend (server-side)
- Tool call events and results are streamed to the client in real-time
- The client receives updates about tool execution progress

This approach provides:

- **Security**: Sensitive operations stay on the server
- **Consistency**: All clients use the same tool implementations
- **Transparency**: Clients can display tool execution progress
- **Flexibility**: Update tools without changing client code

## Creating Function Tools

### Basic Function Tool

You can turn any Python function into a tool using the `@tool` decorator:

```python
from typing import Annotated
from pydantic import Field
from agent_framework import tool


@tool
def get_weather(
    location: Annotated[str, Field(description="The city")],
) -> str:
    """Get the current weather for a location."""
    # In a real application, you would call a weather API
    return f"The weather in {location} is sunny with a temperature of 22°C."
```

### Key Concepts

- **`@tool` decorator**: Marks a function as available to the agent
- **Type annotations**: Provide type information for parameters
- **`Annotated` and `Field`**: Add descriptions to help the agent understand parameters
- **Docstring**: Describes what the function does (helps the agent decide when to use it)
- **Return value**: The result returned to the agent (and streamed to the client)

### Multiple Function Tools

You can provide multiple tools to give the agent more capabilities:

```python
from typing import Any
from agent_framework import tool


@tool
def get_weather(
    location: Annotated[str, Field(description="The city.")],
) -> str:
    """Get the current weather for a location."""
    return f"The weather in {location} is sunny with a temperature of 22°C."


@tool
def get_forecast(
    location: Annotated[str, Field(description="The city.")],
    days: Annotated[int, Field(description="Number of days to forecast")] = 3,
) -> dict[str, Any]:
    """Get the weather forecast for a location."""
    return {
        "location": location,
        "days": days,
        "forecast": [
            {"day": 1, "weather": "Sunny", "high": 24, "low": 18},
            {"day": 2, "weather": "Partly cloudy", "high": 22, "low": 17},
            {"day": 3, "weather": "Rainy", "high": 19, "low": 15},
        ],
    }
```

## Creating an AG-UI Server with Function Tools

Here's a complete server implementation with function tools:

```python
"""AG-UI server with backend tool rendering."""

import os
from typing import Annotated, Any

from agent_framework import Agent, tool
from agent_framework.openai import OpenAIChatCompletionClient
from agent_framework_ag_ui import add_agent_framework_fastapi_endpoint
from azure.identity import AzureCliCredential
from fastapi import FastAPI
from pydantic import Field


# Define function tools
@tool
def get_weather(
    location: Annotated[str, Field(description="The city")],
) -> str:
    """Get the current weather for a location."""
    # Simulated weather data
    return f"The weather in {location} is sunny with a temperature of 22°C."


@tool
def search_restaurants(
    location: Annotated[str, Field(description="The city to search in")],
    cuisine: Annotated[str, Field(description="Type of cuisine")] = "any",
) -> dict[str, Any]:
    """Search for restaurants in a location."""
    # Simulated restaurant data
    return {
        "location": location,
        "cuisine": cuisine,
        "results": [
            {"name": "The Golden Fork", "rating": 4.5, "price": "$$"},
            {"name": "Bella Italia", "rating": 4.2, "price": "$$$"},
            {"name": "Spice Garden", "rating": 4.7, "price": "$$"},
        ],
    }


# Read required configuration
endpoint = os.environ.get("AZURE_OPENAI_ENDPOINT")
deployment_name = os.environ.get("AZURE_OPENAI_CHAT_COMPLETION_MODEL")

if not endpoint:
    raise ValueError("AZURE_OPENAI_ENDPOINT environment variable is required")
if not deployment_name:
    raise ValueError("AZURE_OPENAI_CHAT_COMPLETION_MODEL environment variable is required")

chat_client = OpenAIChatCompletionClient(
    model=deployment_name,
    azure_endpoint=endpoint,
    api_version=os.getenv("AZURE_OPENAI_API_VERSION"),
    credential=AzureCliCredential(),
)

# Create agent with tools
agent = Agent(
    name="TravelAssistant",
    instructions="You are a helpful travel assistant. Use the available tools to help users plan their trips.",
    client=chat_client,
    tools=[get_weather, search_restaurants],
)

# Create FastAPI app
app = FastAPI(title="AG-UI Travel Assistant")
add_agent_framework_fastapi_endpoint(app, agent, "/")

if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="127.0.0.1", port=8888)
```

## Understanding Tool Events

When the agent calls a tool, the client receives several events:

### Tool Call Events

```python
# 1. TOOL_CALL_START - Tool execution begins
{
    "type": "TOOL_CALL_START",
    "toolCallId": "call_abc123",
    "toolCallName": "get_weather"
}

# 2. TOOL_CALL_ARGS - Tool arguments (may stream in chunks)
{
    "type": "TOOL_CALL_ARGS",
    "toolCallId": "call_abc123",
    "delta": "{\"location\": \"Paris, France\"}"
}

# 3. TOOL_CALL_END - Arguments complete
{
    "type": "TOOL_CALL_END",
    "toolCallId": "call_abc123"
}

# 4. TOOL_CALL_RESULT - Tool execution result
{
    "type": "TOOL_CALL_RESULT",
    "toolCallId": "call_abc123",
    "content": "The weather in Paris, France is sunny with a temperature of 22°C."
}
```

## Enhanced Client for Tool Events

Here's an enhanced client using `AGUIChatClient` that displays tool execution:

```python
"""AG-UI client with tool event handling."""

import asyncio
import os

from agent_framework import Agent
from agent_framework_ag_ui import AGUIChatClient


async def main():
    """Main client loop with tool event display."""
    server_url = os.environ.get("AGUI_SERVER_URL", "http://127.0.0.1:8888/")
    print(f"Connecting to AG-UI server at: {server_url}\n")

    # Create AG-UI chat client
    chat_client = AGUIChatClient(endpoint=server_url)
    
    # Create agent with the chat client
    agent = Agent(
        name="ClientAgent",
        client=chat_client,
        instructions="You are a helpful assistant.",
    )

    # Get a thread for conversation continuity
    thread = agent.create_session()

    try:
        while True:
            message = input("\nUser (:q or quit to exit): ")
            if not message.strip():
                continue

            if message.lower() in (":q", "quit"):
                break

            print("\nAssistant: ", end="", flush=True)
            async for update in agent.run(message, session=thread, stream=True):
                # Display text content
                if update.text:
                    print(f"\033[96m{update.text}\033[0m", end="", flush=True)
                
                # Display tool calls and results
                for content in update.contents:
                    if content.type == "function_call":
                        print(f"\n\033[95m[Calling tool: {content.name}]\033[0m")
                    elif content.type == "function_result":
                        result_text = content.result if isinstance(content.result, str) else str(content.result)
                        print(f"\033[94m[Tool result: {result_text}]\033[0m")

            print("\n")

    except KeyboardInterrupt:
        print("\n\nExiting...")
    except Exception as e:
        print(f"\n\033[91mError: {e}\033[0m")


if __name__ == "__main__":
    asyncio.run(main())
```

## Example Interaction

With the enhanced server and client running:

```
User (:q or quit to exit): What's the weather like in Paris and suggest some Italian restaurants?

[Run Started]
[Tool Call: get_weather]
[Tool Result: The weather in Paris, France is sunny with a temperature of 22°C.]
[Tool Call: search_restaurants]
[Tool Result: {"location": "Paris", "cuisine": "Italian", "results": [...]}]
Based on the current weather in Paris (sunny, 22°C) and your interest in Italian cuisine,
I'd recommend visiting Bella Italia, which has a 4.2 rating. The weather is perfect for
outdoor dining!
[Run Finished]
```

## Tool Implementation Best Practices

### Error Handling

Handle errors gracefully in your tools:

```python
@tool
def get_weather(
    location: Annotated[str, Field(description="The city.")],
) -> str:
    """Get the current weather for a location."""
    try:
        # Call weather API
        result = call_weather_api(location)
        return f"The weather in {location} is {result['condition']} with temperature {result['temp']}°C."
    except Exception as e:
        return f"Unable to retrieve weather for {location}. Error: {str(e)}"
```

### Rich Return Types

Return structured data when appropriate:

```python
@tool
def analyze_sentiment(
    text: Annotated[str, Field(description="The text to analyze")],
) -> dict[str, Any]:
    """Analyze the sentiment of text."""
    # Perform sentiment analysis
    return {
        "text": text,
        "sentiment": "positive",
        "confidence": 0.87,
        "scores": {
            "positive": 0.87,
            "neutral": 0.10,
            "negative": 0.03,
        },
    }
```

### Descriptive Documentation

Provide clear descriptions to help the agent understand when to use tools:

```python
@tool
def book_flight(
    origin: Annotated[str, Field(description="Departure city and airport code, e.g., 'New York, JFK'")],
    destination: Annotated[str, Field(description="Arrival city and airport code, e.g., 'London, LHR'")],
    date: Annotated[str, Field(description="Departure date in YYYY-MM-DD format")],
    passengers: Annotated[int, Field(description="Number of passengers")] = 1,
) -> dict[str, Any]:
    """
    Book a flight for specified passengers from origin to destination.
    
    This tool should be used when the user wants to book or reserve airline tickets.
    Do not use this for searching flights - use search_flights instead.
    """
    # Implementation
    pass
```

## Tool Organization with Classes

For related tools, organize them in a class:

```python
from agent_framework import tool


class WeatherTools:
    """Collection of weather-related tools."""
    
    def __init__(self, api_key: str):
        self.api_key = api_key
    
    @tool
    def get_current_weather(
        self,
        location: Annotated[str, Field(description="The city.")],
    ) -> str:
        """Get current weather for a location."""
        # Use self.api_key to call API
        return f"Current weather in {location}: Sunny, 22°C"
    
    @tool
    def get_forecast(
        self,
        location: Annotated[str, Field(description="The city.")],
        days: Annotated[int, Field(description="Number of days")] = 3,
    ) -> dict[str, Any]:
        """Get weather forecast for a location."""
        # Use self.api_key to call API
        return {"location": location, "forecast": [...]}


# Create tools instance
weather_tools = WeatherTools(api_key="your-api-key")

# Create agent with class-based tools
agent = Agent(
    name="WeatherAgent",
    instructions="You are a weather assistant.",
    client=OpenAIChatCompletionClient(...),
    tools=[
        weather_tools.get_current_weather,
        weather_tools.get_forecast,
    ],
)
```

## Next Steps

Now that you understand backend tool rendering, you can:

<!-- - **[Add Human-in-the-Loop](human-in-the-loop.md)**: Require user approval before executing sensitive tools -->
<!-- - **[Manage State](state-management.md)**: Share state between client and server for richer interactions -->
- **[Create Advanced Tools](../../../../agents/tools/function-tools.md)**: Learn more about creating function tools with Agent Framework

## Additional Resources

- [AG-UI Overview](index.md)
- [Getting Started with AG-UI](getting-started.md)
- [Function Tools Tutorial](../../../../agents/tools/function-tools.md)

::: zone-end

::: zone pivot="programming-language-go"

Go AG-UI servers can expose normal Agent Framework function tools. Create tools with `tool/functool`, attach them to the hosted agent, and serve the agent with `aguiprovider.NewJSONHTTPHandler`.

```go
searchRestaurants := functool.MustNew(functool.Config{
    Name:        "search_restaurants",
    Description: "Search for restaurants in a location.",
}, func(ctx context.Context, in restaurantSearchRequest) (restaurantSearchResponse, error) {
    return restaurantSearchResponse{
        Location: in.Location,
        Cuisine:  in.Cuisine,
        Results:  []restaurantInfo{{Name: "The Golden Fork", Cuisine: in.Cuisine}},
    }, nil
})

a := foundryprovider.NewAgent(endpoint, token, foundryprovider.ModelDeployment(model), foundryprovider.AgentConfig{
    Config: agent.Config{
        Tools: []tool.Tool{searchRestaurants},
    },
})
```

> [!TIP]
> See the [AG-UI backend tools sample](https://github.com/microsoft/agent-framework-go/blob/main/examples/02-agents/agui/step02_backend_tools/server/main.go) for a complete runnable example.

::: zone-end
