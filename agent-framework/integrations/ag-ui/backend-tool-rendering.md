---
title: Backend Tool Rendering with AG-UI
description: Learn how to add function tools that execute on the backend with results streamed to clients
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 11/07/2025
ms.service: agent-framework
---

# Backend Tool Rendering with AG-UI

::: zone pivot="programming-language-csharp"

This tutorial shows you how to add function tools to your AG-UI agents. Function tools are custom C# methods that the agent can call to perform specific tasks like retrieving data, performing calculations, or interacting with external systems. With AG-UI, these tools execute on the backend and their results are automatically streamed to the client.

## Prerequisites

Before you begin, ensure you have completed the [Getting Started](getting-started.md) tutorial and have:

- .NET 8.0 or later
- `Microsoft.Agents.AI.Hosting.AGUI.AspNetCore` package installed
- Azure OpenAI service configured
- Basic understanding of AG-UI server and client setup

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

You can turn any C# method into a tool using `AIFunctionFactory.Create`:

```csharp
using System.ComponentModel;
using Microsoft.Extensions.AI;

[Description("Get the current weather for a location.")]
static string GetWeather([Description("The city")] string location)
{
    // In a real application, you would call a weather API
    return $"The weather in {location} is sunny with a temperature of 22°C.";
}

var weatherTool = AIFunctionFactory.Create(GetWeather);
```

### Key Concepts

- **`AIFunctionFactory.Create`**: Converts a method into an `AIFunction` tool
- **`[Description]` attributes**: Provide descriptions to help the agent understand the function and parameters
- **Type annotations**: Provide type information for parameters
- **Return value**: The result returned to the agent (and streamed to the client)

### Multiple Function Tools

You can provide multiple tools to give the agent more capabilities:

```csharp
using System.ComponentModel;
using Microsoft.Extensions.AI;

// Simple tool returning a string
[Description("Get the current weather for a location.")]
static string GetWeather([Description("The city")] string location)
{
    return $"The weather in {location} is sunny with a temperature of 22°C.";
}

// Complex tool with request/response types
internal sealed class WeatherForecastRequest
{
    public string Location { get; set; } = string.Empty;
}

internal sealed class WeatherForecastResponse
{
    public string Location { get; set; } = string.Empty;
    public string Conditions { get; set; } = string.Empty;
    public int TemperatureC { get; set; }
}

[Description("Get the weather forecast for a location.")]
static WeatherForecastResponse GetForecast(
    [Description("The weather forecast request")] WeatherForecastRequest request)
{
    return new WeatherForecastResponse
    {
        Location = request.Location,
        Conditions = "Sunny",
        TemperatureC = 22
    };
}

AITool[] tools =
[
    AIFunctionFactory.Create(GetWeather),
    AIFunctionFactory.Create(
        GetForecast,
        serializerOptions: Step3SampleJsonSerializerContext.Default.Options)
];
```

## Creating an AG-UI Server with Function Tools

Here's a complete server implementation with function tools that demonstrates both simple (string return) and complex (typed object return) patterns:

```csharp
// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel;
using System.Text.Json.Serialization;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.AI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient().AddLogging();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.TypeInfoResolverChain.Add(Step3SampleJsonSerializerContext.Default));
builder.Services.AddAGUI();

WebApplication app = builder.Build();

string endpoint = builder.Configuration["AZURE_OPENAI_ENDPOINT"]
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
string deploymentName = builder.Configuration["AZURE_OPENAI_DEPLOYMENT_NAME"]
    ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME is not set.");

// Define request/response types for tools
internal sealed class WeatherForecastRequest
{
    public string Location { get; set; } = string.Empty;
}

internal sealed class WeatherForecastResponse
{
    public string Location { get; set; } = string.Empty;
    public string Conditions { get; set; } = string.Empty;
    public int TemperatureC { get; set; }
}

internal sealed class RestaurantSearchRequest
{
    public string Location { get; set; } = string.Empty;
    public string Cuisine { get; set; } = "any";
}

internal sealed class RestaurantSearchResponse
{
    public string Location { get; set; } = string.Empty;
    public string Cuisine { get; set; } = string.Empty;
    public RestaurantInfo[] Results { get; set; } = [];
}

internal sealed class RestaurantInfo
{
    public string Name { get; set; } = string.Empty;
    public string Cuisine { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Address { get; set; } = string.Empty;
}

// JSON serialization context for source generation
[JsonSerializable(typeof(WeatherForecastRequest))]
[JsonSerializable(typeof(WeatherForecastResponse))]
[JsonSerializable(typeof(RestaurantSearchRequest))]
[JsonSerializable(typeof(RestaurantSearchResponse))]
internal sealed partial class Step3SampleJsonSerializerContext : JsonSerializerContext;

// Define function tools

// Simple tool that returns a string
[Description("Get the current weather for a location.")]
static string GetWeather([Description("The city")] string location)
{
    // Simulated weather data
    return $"The weather in {location} is sunny with a temperature of 22°C.";
}

// Complex tool with typed request/response
[Description("Search for restaurants in a location.")]
static RestaurantSearchResponse SearchRestaurants(
    [Description("The restaurant search request")] RestaurantSearchRequest request)
{
    // Simulated restaurant data
    string cuisine = request.Cuisine == "any" ? "Italian" : request.Cuisine;

    return new RestaurantSearchResponse
    {
        Location = request.Location,
        Cuisine = request.Cuisine,
        Results =
        [
            new RestaurantInfo
            {
                Name = "The Golden Fork",
                Cuisine = cuisine,
                Rating = 4.5,
                Address = $"123 Main St, {request.Location}"
            },
            new RestaurantInfo
            {
                Name = "Spice Haven",
                Cuisine = cuisine == "Italian" ? "Indian" : cuisine,
                Rating = 4.7,
                Address = $"456 Oak Ave, {request.Location}"
            },
            new RestaurantInfo
            {
                Name = "Green Leaf",
                Cuisine = "Vegetarian",
                Rating = 4.3,
                Address = $"789 Elm Rd, {request.Location}"
            }
        ]
    };
}

// Another tool with typed request/response
[Description("Get the weather forecast for a location.")]
static WeatherForecastResponse GetForecast(
    [Description("The weather forecast request")] WeatherForecastRequest request)
{
    // Simulated forecast data
    return new WeatherForecastResponse
    {
        Location = request.Location,
        Conditions = "Sunny",
        TemperatureC = 22
    };
}

// Create tools array
AITool[] tools =
[
    AIFunctionFactory.Create(GetWeather),
    AIFunctionFactory.Create(
        SearchRestaurants,
        serializerOptions: Step3SampleJsonSerializerContext.Default.Options),
    AIFunctionFactory.Create(
        GetForecast,
        serializerOptions: Step3SampleJsonSerializerContext.Default.Options)
];

// Create the AI agent with tools
IChatClient chatClient = new AzureOpenAIClient(
        new Uri(endpoint),
        new DefaultAzureCredential())
    .GetChatClient(deploymentName);

AIAgent agent = chatClient.CreateAIAgent(
    name: "AGUIAssistant",
    instructions: "You are a helpful assistant with access to weather and restaurant information.",
    tools: tools);

// Map the AG-UI agent endpoint
app.MapAGUI("/", agent);

await app.RunAsync();
```

### Key Concepts

- **Tool Definition**: Methods decorated with `[Description]` attributes
- **Request/Response Types**: Explicit classes for type safety with complex tools
- **JSON Serialization Context**: Source-generated serialization for performance and trimming
- **`ConfigureHttpJsonOptions`**: Registers the serialization context with ASP.NET Core
- **`AIFunctionFactory.Create`**: Converts methods into tool definitions
- **`serializerOptions` parameter**: Required for tools with complex types
- **`CreateAIAgent` with tools**: Registers tools with the agent
- **Server-side execution**: Tools execute in the server process
- **Automatic streaming**: Tool calls and results are streamed to clients

### Running the Server

Set environment variables and run:

```bash
export AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
export AZURE_OPENAI_DEPLOYMENT_NAME="gpt-4o-mini"
dotnet run --urls http://localhost:8888
```

## Observing Tool Calls in the Client

When using the AG-UI client from the Getting Started tutorial, tool execution is automatically handled by the agent. The client displays the final results:

```csharp
User (:q or quit to exit): What's the weather like in Amsterdam?

[Run Started - Thread: thread_abc123, Run: run_xyz789]
The weather in Amsterdam is sunny with a temperature of 22°C.
[Run Finished - Thread: thread_abc123]
```

The client doesn't need special handling for tools - it just receives the agent's final response after tool execution completes.

## How Tool Rendering Works

### Server-Side Flow

1. Client sends message: "What's the weather in Amsterdam?"
2. Agent analyzes request and decides to call `GetWeather`
3. Server executes `GetWeather("Amsterdam")`
4. Server receives tool result: "The weather in Amsterdam is sunny..."
5. Agent formulates final response incorporating the tool result
6. Final response is streamed to client as text

### Client-Side Experience

From the client's perspective:

1. Sends user message to server
2. Receives streaming text response
3. Displays final answer (no tool-specific handling needed)

The AG-UI protocol handles all tool execution details internally, providing a seamless experience.

## Common Patterns

### Async Function Tools

For I/O-bound operations, use async tools:

```csharp
[Description("Fetch weather data from an API.")]
static async Task<string> GetWeatherAsync([Description("The city")] string location)
{
    // Simulate async API call
    await Task.Delay(100);
    return $"The weather in {location} is sunny with a temperature of 22°C.";
}

var weatherTool = AIFunctionFactory.Create(GetWeatherAsync);
```

### Tools with Complex Return Types

Return structured data using explicit types for better type safety:

```csharp
internal sealed class WeatherDetails
{
    public string Location { get; set; } = string.Empty;
    public CurrentConditions Current { get; set; } = new();
    public ForecastDay[] Forecast { get; set; } = [];
}

internal sealed class CurrentConditions
{
    public int Temperature { get; set; }
    public string Conditions { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public int WindSpeed { get; set; }
}

internal sealed class ForecastDay
{
    public string Day { get; set; } = string.Empty;
    public int High { get; set; }
    public int Low { get; set; }
}

[Description("Get detailed weather information.")]
static WeatherDetails GetDetailedWeather(
    [Description("The weather request")] WeatherRequest request)
{
    return new WeatherDetails
    {
        Location = request.Location,
        Current = new CurrentConditions
        {
            Temperature = 22,
            Conditions = "Sunny",
            Humidity = 65,
            WindSpeed = 15
        },
        Forecast =
        [
            new ForecastDay { Day = "Monday", High = 24, Low = 18 },
            new ForecastDay { Day = "Tuesday", High = 22, Low = 17 }
        ]
    };
}

// Remember to add types to serialization context
[JsonSerializable(typeof(WeatherDetails))]
[JsonSerializable(typeof(WeatherRequest))]
internal sealed partial class Step3SampleJsonSerializerContext : JsonSerializerContext;
```

### Error Handling in Tools

Handle errors gracefully within tools:

```csharp
[Description("Get weather with error handling.")]
static string GetWeatherSafe([Description("The city")] string location)
{
    try
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return "Error: Location cannot be empty.";
        }

        // Simulate weather lookup
        return $"The weather in {location} is sunny.";
    }
    catch (Exception ex)
    {
        return $"Error retrieving weather: {ex.Message}";
    }
}
```

## Testing Your Tools

You can test the agent with tools using curl:

```bash
curl -X POST http://127.0.0.1:8888/ \
  -H "Content-Type: application/json" \
  -d '{
    "messages": [
      {
        "role": "user",
        "content": "What is the weather in Amsterdam and can you find Italian restaurants there?"
      }
    ]
  }'
```

Expected response (streamed as SSE events):

```
event: run_started
data: {"threadId":"thread_abc","runId":"run_123"}

event: text_message_content
data: {"delta":"The weather in Amsterdam is sunny with a temperature of 22°C."}

event: text_message_content  
data: {"delta":" I found some Italian restaurants: The Golden Fork (4.5 stars) at 123 Main St."}

event: run_finished
data: {"threadId":"thread_abc","runId":"run_123"}
```

## Troubleshooting

### Tools Not Being Called

1. Ensure descriptions are clear and relevant
2. Verify the agent's instructions encourage tool usage
3. Check that tool signatures match expected parameter types

### Tool Execution Errors

1. Add try-catch blocks in tool implementations
2. Return error messages as strings rather than throwing exceptions
3. Log errors on the server for debugging

### Complex Return Values Not Working

1. Use anonymous objects or serializable types
2. Avoid circular references in return values
3. Test tools independently before integrating

## Next Steps

Now that you can add function tools, you can:

- **[Implement Human-in-the-Loop](human-in-the-loop.md)**: Add approval workflows for sensitive operations
- **[Manage State](state-management.md)**: Implement shared state for generative UI applications
- **[Test with Dojo](testing-with-dojo.md)**: Use AG-UI's Dojo app to test your agents

## Additional Resources

- [AG-UI Overview](index.md)
- [Getting Started Tutorial](getting-started.md)
- [Agent Framework Documentation](../../overview/agent-framework-overview.md)

::: zone-end

::: zone pivot="programming-language-python"

This tutorial shows you how to add function tools to your AG-UI agents. Function tools are custom Python functions that the agent can call to perform specific tasks like retrieving data, performing calculations, or interacting with external systems. With AG-UI, these tools execute on the backend and their results are automatically streamed to the client.

## Prerequisites

Before you begin, ensure you have completed the [Getting Started](getting-started.md) tutorial and have:

- Python 3.10 or later
- `agent-framework-ag-ui`, `agent-framework-core`, `fastapi`, and `uvicorn` installed
- Azure OpenAI service configured
- Basic understanding of AG-UI server and client setup

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

You can turn any Python function into a tool using the `@ai_function` decorator:

```python
from typing import Annotated
from pydantic import Field
from agent_framework import ai_function


@ai_function
def get_weather(
    location: Annotated[str, Field(description="The city")],
) -> str:
    """Get the current weather for a location."""
    # In a real application, you would call a weather API
    return f"The weather in {location} is sunny with a temperature of 22°C."
```

### Key Concepts

- **`@ai_function` decorator**: Marks a function as available to the agent
- **Type annotations**: Provide type information for parameters
- **`Annotated` and `Field`**: Add descriptions to help the agent understand parameters
- **Docstring**: Describes what the function does (helps the agent decide when to use it)
- **Return value**: The result returned to the agent (and streamed to the client)

### Multiple Function Tools

You can provide multiple tools to give the agent more capabilities:

```python
from typing import Any
from agent_framework import ai_function


@ai_function
def get_weather(
    location: Annotated[str, Field(description="The city.")],
) -> str:
    """Get the current weather for a location."""
    return f"The weather in {location} is sunny with a temperature of 22°C."


@ai_function
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

from agent_framework import ChatAgent, ai_function
from agent_framework.azure import AzureOpenAIChatClient
from agent_framework_ag_ui import add_agent_framework_fastapi_endpoint
from fastapi import FastAPI
from pydantic import Field


# Define function tools
@ai_function
def get_weather(
    location: Annotated[str, Field(description="The city")],
) -> str:
    """Get the current weather for a location."""
    # Simulated weather data
    return f"The weather in {location} is sunny with a temperature of 22°C."


@ai_function
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


# Read configuration
endpoint = os.environ.get("AZURE_OPENAI_ENDPOINT")
deployment_name = os.environ.get("AZURE_OPENAI_DEPLOYMENT_NAME")

if not endpoint or not deployment_name:
    raise ValueError("AZURE_OPENAI_ENDPOINT and AZURE_OPENAI_DEPLOYMENT_NAME are required")

# Create agent with tools
agent = ChatAgent(
    name="TravelAssistant",
    instructions="You are a helpful travel assistant. Use the available tools to help users plan their trips.",
    chat_client=AzureOpenAIChatClient(
        endpoint=endpoint,
        deployment_name=deployment_name,
    ),
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

Here's an enhanced client that displays tool execution:

```python
"""AG-UI client with tool event handling."""

import asyncio
import json
import os

import httpx


class AGUIClient:
    """AG-UI client with tool event support."""

    def __init__(self, server_url: str):
        self.server_url = server_url
        self.thread_id: str | None = None

    async def send_message(self, message: str):
        """Send a message and handle streaming response with tool events."""
        request_data = {
            "messages": [
                {"role": "system", "content": "You are a helpful assistant."},
                {"role": "user", "content": message},
            ]
        }

        if self.thread_id:
            request_data["thread_id"] = self.thread_id

        async with httpx.AsyncClient(timeout=60.0) as client:
            async with client.stream(
                "POST",
                self.server_url,
                json=request_data,
                headers={"Accept": "text/event-stream"},
            ) as response:
                response.raise_for_status()

                current_tool_args = {}

                async for line in response.aiter_lines():
                    if line.startswith("data: "):
                        data = line[6:]
                        try:
                            event = json.loads(data)
                            yield event

                            # Capture thread_id
                            if event.get("type") == "RUN_STARTED" and not self.thread_id:
                                self.thread_id = event.get("threadId")

                            # Track tool arguments
                            if event.get("type") == "TOOL_CALL_ARGS":
                                tool_id = event.get("toolCallId")
                                if tool_id not in current_tool_args:
                                    current_tool_args[tool_id] = ""
                                current_tool_args[tool_id] += event.get("delta", "")

                        except json.JSONDecodeError:
                            continue


async def main():
    """Main client loop with tool event display."""
    server_url = os.environ.get("AGUI_SERVER_URL", "http://127.0.0.1:8888/")
    print(f"Connecting to AG-UI server at: {server_url}\n")

    client = AGUIClient(server_url)

    try:
        while True:
            message = input("\nUser (:q or quit to exit): ")
            if not message.strip():
                continue

            if message.lower() in (":q", "quit"):
                break

            print()
            async for event in client.send_message(message):
                event_type = event.get("type", "")

                if event_type == "RUN_STARTED":
                    print(f"\033[93m[Run Started]\033[0m")

                elif event_type == "TEXT_MESSAGE_CONTENT":
                    print(f"\033[96m{event.get('delta', '')}\033[0m", end="", flush=True)

                elif event_type == "TOOL_CALL_START":
                    tool_name = event.get("toolCallName", "unknown")
                    print(f"\n\033[95m[Tool Call: {tool_name}]\033[0m")

                elif event_type == "TOOL_CALL_RESULT":
                    content = event.get("content", "")
                    print(f"\033[94m[Tool Result: {content}]\033[0m")

                elif event_type == "RUN_FINISHED":
                    print(f"\n\033[92m[Run Finished]\033[0m")

                elif event_type == "RUN_ERROR":
                    error_msg = event.get("message", "Unknown error")
                    print(f"\n\033[91m[Error: {error_msg}]\033[0m")

            print()

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
@ai_function
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
@ai_function
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
@ai_function
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
from agent_framework import ai_function


class WeatherTools:
    """Collection of weather-related tools."""
    
    def __init__(self, api_key: str):
        self.api_key = api_key
    
    @ai_function
    def get_current_weather(
        self,
        location: Annotated[str, Field(description="The city.")],
    ) -> str:
        """Get current weather for a location."""
        # Use self.api_key to call API
        return f"Current weather in {location}: Sunny, 22°C"
    
    @ai_function
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
agent = ChatAgent(
    name="WeatherAgent",
    instructions="You are a weather assistant.",
    chat_client=AzureOpenAIChatClient(...),
    tools=[
        weather_tools.get_current_weather,
        weather_tools.get_forecast,
    ],
)
```

## Next Steps

Now that you understand backend tool rendering, you can:

- **[Add Human-in-the-Loop](human-in-the-loop.md)**: Require user approval before executing sensitive tools
- **[Manage State](state-management.md)**: Share state between client and server for richer interactions
- **[Create Advanced Tools](../../tutorials/agents/function-tools.md)**: Learn more about creating function tools with Agent Framework

## Additional Resources

- [AG-UI Overview](index.md)
- [Getting Started with AG-UI](getting-started.md)
- [Function Tools Tutorial](../../tutorials/agents/function-tools.md)

::: zone-end
