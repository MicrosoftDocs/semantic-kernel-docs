---
title: Frontend Tool Rendering with AG-UI
description: Learn how to register client-side tools that execute in the browser or client application
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
  | Frontend tool declarations | ✅ |   ✅   | ❌ | Go zone covers server-side forwarding only |
  | Client execution flow      | ✅ |   ✅   | ❌ | Not documented for Go |
-->

# Frontend Tool Rendering with AG-UI

::: zone pivot="programming-language-csharp"

Frontend tools are declared and executed by the AG-UI client. The server receives their schemas so the model can request them, but it doesn't receive their implementations.

## Register a frontend tool

Create the tool and pass it to the agent backed by `AGUIChatClient`:

```csharp
using System.ComponentModel;
using AGUI.Client;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

[Description("Get the user's current location from the client device.")]
static string GetUserLocation() => "Amsterdam, Netherlands";

AITool locationTool = AIFunctionFactory.Create(
    GetUserLocation,
    name: "get_user_location");

using HttpClient httpClient = new() { BaseAddress = new Uri("http://localhost:8888") };
AGUIChatClient chatClient = new(new AGUIChatClientOptions(httpClient, "/"));
AIAgent agent = chatClient.AsAIAgent(tools: [locationTool]);
```

`AGUIChatClient` handles the continuation flow:

1. Sends the frontend tool declaration with the run request.
2. Receives the model's tool call from the server.
3. Executes the matching function locally.
4. Sends the result back to the server.
5. Continues the run and streams the final response.

> [!TIP]
> See the [.NET frontend-tools sample](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/02-agents/AGUI/Step03_FrontendTools) for a complete client and server.

> [!WARNING]
> Tool declarations and results supplied by an untrusted client are untrusted input. Authorize which client tools may influence server-side agent execution, and validate results before using them for privileged operations.

For general tool-authoring guidance, see [Use function tools with an agent](../../../../agents/tools/function-tools.md).

## Next steps

> [!div class="nextstepaction"]
> [Use human approval with AG-UI](./human-in-the-loop.md)

::: zone-end

::: zone pivot="programming-language-python"

This tutorial shows you how to add frontend function tools to your AG-UI clients. Frontend tools are functions that execute on the client side, allowing the AI agent to interact with the user's local environment, access client-specific data, or perform UI operations.

## Prerequisites

Before you begin, ensure you have completed the [Getting Started](getting-started.md) tutorial and have:

- Python 3.10 or later
- `httpx` installed for HTTP client functionality
- Basic understanding of AG-UI client setup
- Azure OpenAI service configured

## What are Frontend Tools?

Frontend tools are function tools that:

- Are defined and registered on the client
- Execute in the client's environment (not on the server)
- Allow the AI agent to interact with client-specific resources
- Provide results back to the server for the agent to incorporate into responses

Common use cases:
- Reading local sensor data
- Accessing client-side storage or preferences
- Performing UI operations
- Interacting with device-specific features

## Creating Frontend Tools

Frontend tools in Python are defined similarly to backend tools but are registered with the client:

```python
from typing import Annotated
from pydantic import BaseModel, Field


class SensorReading(BaseModel):
    """Sensor reading from client device."""
    temperature: float
    humidity: float
    air_quality_index: int


def read_climate_sensors(
    include_temperature: Annotated[bool, Field(description="Include temperature reading")] = True,
    include_humidity: Annotated[bool, Field(description="Include humidity reading")] = True,
) -> SensorReading:
    """Read climate sensor data from the client device."""
    # Simulate reading from local sensors
    return SensorReading(
        temperature=22.5 if include_temperature else 0.0,
        humidity=45.0 if include_humidity else 0.0,
        air_quality_index=75,
    )


def change_background_color(color: Annotated[str, Field(description="Color name")] = "blue") -> str:
    """Change the console background color."""
    # Simulate UI change
    print(f"\n🎨 Background color changed to {color}")
    return f"Background changed to {color}"
```

## Creating an AG-UI Client with Frontend Tools

Here's a complete client implementation with frontend tools:

```python
"""AG-UI client with frontend tools."""

import asyncio
import json
import os
from typing import Annotated, AsyncIterator

import httpx
from pydantic import BaseModel, Field


class SensorReading(BaseModel):
    """Sensor reading from client device."""
    temperature: float
    humidity: float
    air_quality_index: int


# Define frontend tools
def read_climate_sensors(
    include_temperature: Annotated[bool, Field(description="Include temperature")] = True,
    include_humidity: Annotated[bool, Field(description="Include humidity")] = True,
) -> SensorReading:
    """Read climate sensor data from the client device."""
    return SensorReading(
        temperature=22.5 if include_temperature else 0.0,
        humidity=45.0 if include_humidity else 0.0,
        air_quality_index=75,
    )


def get_user_location() -> dict:
    """Get the user's current GPS location."""
    # Simulate GPS reading
    return {
        "latitude": 52.3676,
        "longitude": 4.9041,
        "accuracy": 10.0,
        "city": "Amsterdam",
    }


# Tool registry maps tool names to functions
FRONTEND_TOOLS = {
    "read_climate_sensors": read_climate_sensors,
    "get_user_location": get_user_location,
}


class AGUIClientWithTools:
    """AG-UI client with frontend tool support."""

    def __init__(self, server_url: str, tools: dict):
        self.server_url = server_url
        self.tools = tools
        self.thread_id: str | None = None

    async def send_message(self, message: str) -> AsyncIterator[dict]:
        """Send a message and handle streaming response with tool execution."""
        # Prepare tool declarations for the server
        tool_declarations = []
        for name, func in self.tools.items():
            tool_declarations.append({
                "name": name,
                "description": func.__doc__ or "",
                # Add parameter schema from function signature
            })

        request_data = {
            "messages": [
                {"role": "system", "content": "You are a helpful assistant with access to client tools."},
                {"role": "user", "content": message},
            ],
            "tools": tool_declarations,  # Send tool declarations to server
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

                async for line in response.aiter_lines():
                    if line.startswith("data: "):
                        data = line[6:]
                        try:
                            event = json.loads(data)
                            
                            # Tool calls arrive as TOOL_CALL_START/ARGS/END events
                            # and results are streamed back as TOOL_CALL_RESULT events.
                            yield event

                            # Capture thread_id
                            if event.get("type") == "RUN_STARTED" and not self.thread_id:
                                self.thread_id = event.get("threadId")

                        except json.JSONDecodeError:
                            continue

    async def _handle_tool_call(self, event: dict, client: httpx.AsyncClient):
        """Execute frontend tool and send result back to server."""
        tool_name = event.get("toolName")
        tool_call_id = event.get("toolCallId")
        arguments = event.get("arguments", {})

        print(f"\n\033[95m[Client Tool Call: {tool_name}]\033[0m")
        print(f"  Arguments: {arguments}")

        try:
            # Execute the tool
            tool_func = self.tools.get(tool_name)
            if not tool_func:
                raise ValueError(f"Unknown tool: {tool_name}")

            result = tool_func(**arguments)

            # Convert Pydantic models to dict
            if hasattr(result, "model_dump"):
                result = result.model_dump()

            print(f"\033[94m[Client Tool Result: {result}]\033[0m")

            # In current Python AG-UI, frontend tool declarations are sent with
            # the run request. Tool-call lifecycle events are streamed back over SSE.
            print(f"Tool result for {tool_call_id}: {result}")

        except Exception as e:
            print(f"\033[91m[Tool Error: {e}]\033[0m")
            print(f"Tool error for {tool_call_id}: {e}")


async def main():
    """Main client loop with frontend tools."""
    server_url = os.environ.get("AGUI_SERVER_URL", "http://127.0.0.1:8888/")
    print(f"Connecting to AG-UI server at: {server_url}\n")

    client = AGUIClientWithTools(server_url, FRONTEND_TOOLS)

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

## How Frontend Tools Work

### Protocol Flow

1. **Client Registration**: Client sends tool declarations (names, descriptions, parameters) to server
2. **Server Orchestration**: AI agent decides when to call frontend tools based on user request
3. **Tool Call Events**: Server streams `TOOL_CALL_START`, `TOOL_CALL_ARGS`, and `TOOL_CALL_END` events to the client
4. **Client Execution**: Client executes the tool locally
5. **Result Events**: Tool results are represented as `TOOL_CALL_RESULT` events in the stream
6. **Agent Processing**: Server incorporates result and continues response

### Key Events

- **`TOOL_CALL_START` / `TOOL_CALL_ARGS` / `TOOL_CALL_END`**: Server requests and streams tool-call details
- **`TOOL_CALL_RESULT`**: Tool execution result event

## Expected Output

```
User (:q or quit to exit): What's the temperature reading from my sensors?

[Run Started]

[Client Tool Call: read_climate_sensors]
  Arguments: {'include_temperature': True, 'include_humidity': True}
[Client Tool Result: {'temperature': 22.5, 'humidity': 45.0, 'air_quality_index': 75}]

Based on your sensor readings, the current temperature is 22.5°C and the 
humidity is at 45%. These are comfortable conditions!
[Run Finished]
```

## Server Setup

The standard AG-UI server from the Getting Started tutorial automatically supports frontend tools. No changes needed on the server side - it handles tool orchestration automatically.

## Best Practices

### Security

```python
def access_sensitive_data() -> str:
    """Access user's sensitive data."""
    # Always check permissions first
    if not has_permission():
        return "Error: Permission denied"
    
    try:
        # Access data
        return "Data retrieved"
    except Exception as e:
        # Don't expose internal errors
        return "Unable to access data"
```

### Error Handling

```python
def read_file(path: str) -> str:
    """Read a local file."""
    try:
        with open(path, "r") as f:
            return f.read()
    except FileNotFoundError:
        return f"Error: File not found: {path}"
    except PermissionError:
        return f"Error: Permission denied: {path}"
    except Exception as e:
        return f"Error reading file: {str(e)}"
```

### Async Operations

```python
async def capture_photo() -> str:
    """Capture a photo from device camera."""
    # Simulate camera access
    await asyncio.sleep(1)
    return "photo_12345.jpg"
```

## Troubleshooting

### Tools Not Being Called

1. Ensure tool declarations are sent to server
2. Verify tool descriptions clearly indicate purpose
3. Check server logs for tool registration

### Execution Errors

1. Add comprehensive error handling
2. Validate parameters before processing
3. Return user-friendly error messages
4. Log errors for debugging

### Type Issues

1. Use Pydantic models for complex types
2. Convert models to dicts before serialization
3. Handle type conversions explicitly

## Next Steps

- **[Backend Tool Rendering](backend-tool-rendering.md)**: Combine with server-side tools
<!-- - **[Human-in-the-Loop](human-in-the-loop.md)**: Add approval workflows -->
<!-- - **[State Management](state-management.md)**: Share state between client and server -->

## Additional Resources

- [AG-UI Overview](index.md)
- [Getting Started Tutorial](getting-started.md)
- [Agent Framework Documentation](../../../../overview/index.md)

::: zone-end

::: zone pivot="programming-language-go"

Go AG-UI servers can leave tool calls for the frontend by disabling automatic function calling on the hosted agent.

```go
a := foundryprovider.NewAgent(endpoint, token, foundryprovider.ModelDeployment(model), foundryprovider.AgentConfig{
    Instructions: "You are a helpful assistant.",
    Config: agent.Config{
        Name:                "AGUIAssistant",
        DisableFuncAutoCall: true,
    },
})

mux := http.NewServeMux()
mux.Handle("/", aguiprovider.NewJSONHTTPHandler(a, aguiprovider.HandlerConfig{}))
```

> [!TIP]
> See the [AG-UI frontend tools sample](https://github.com/microsoft/agent-framework-go/blob/main/examples/02-agents/agui/step03_frontend_tools/server/main.go) for a complete runnable example.

::: zone-end