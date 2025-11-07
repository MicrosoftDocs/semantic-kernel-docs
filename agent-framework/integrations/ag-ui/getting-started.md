---
title: Getting Started with AG-UI
description: Step-by-step tutorial to build your first AG-UI server and client with Agent Framework
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 11/07/2025
ms.service: agent-framework
---

# Getting Started with AG-UI

This tutorial demonstrates how to build both server and client applications using the AG-UI protocol with Python and Agent Framework. You'll learn how to create an AG-UI server that hosts an AI agent and a client that connects to it for interactive conversations.

## What You'll Build

By the end of this tutorial, you'll have:

- An AG-UI server hosting an AI agent accessible via HTTP
- A client application that connects to the server and streams responses
- Understanding of how the AG-UI protocol works with Agent Framework

::: zone pivot="programming-language-csharp"

## Prerequisites

Before you begin, ensure you have the following:

- .NET 8.0 or later
- [Azure OpenAI service endpoint and deployment configured](/azure/ai-foundry/openai/how-to/create-resource)
- [Azure CLI installed](/cli/azure/install-azure-cli) and [authenticated](/cli/azure/authenticate-azure-cli)
- User has the `Cognitive Services OpenAI Contributor` role for the Azure OpenAI resource

> [!NOTE]
> These samples use Azure OpenAI models. For more information, see [how to deploy Azure OpenAI models with Azure AI Foundry](/azure/ai-foundry/how-to/deploy-models-openai).

> [!NOTE]
> These samples use `DefaultAzureCredential` for authentication. Make sure you're authenticated with Azure (e.g., via `az login`). For more information, see the [Azure Identity documentation](/dotnet/api/overview/azure/identity-readme).

> [!WARNING]
> The AG-UI protocol is still under development and subject to change. We will keep these samples updated as the protocol evolves.

## Step 1: Creating an AG-UI Server

The AG-UI server hosts your AI agent and exposes it via HTTP endpoints using ASP.NET Core.

### Install Required Packages

Install the necessary packages for the server:

```bash
dotnet add package Microsoft.Agents.AI.Hosting.AGUI.AspNetCore
```

This will automatically install the required dependencies including `Microsoft.Extensions.AI`.

### Server Code

Create a file named `Program.cs`:

```csharp
// Copyright (c) Microsoft. All rights reserved.

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient().AddLogging();
var app = builder.Build();

string endpoint = builder.Configuration["AZURE_OPENAI_ENDPOINT"] 
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
string deploymentName = builder.Configuration["AZURE_OPENAI_DEPLOYMENT_NAME"] 
    ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME is not set.");

// Create the AI agent
var agent = new AzureOpenAIClient(
        new Uri(endpoint),
        new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .CreateAIAgent(
        name: "AGUIAssistant",
        instructions: "You are a helpful assistant.");

// Map the AG-UI agent endpoint
app.MapAGUI("/", agent);

await app.RunAsync();
```

### Key Concepts

- **`MapAGUI`**: Extension method that registers the AG-UI endpoint with automatic request/response handling and SSE streaming
- **`CreateAIAgent`**: Creates an Agent Framework agent from a chat client
- **ASP.NET Core Integration**: Uses ASP.NET Core's native async support for streaming responses
- **Instructions**: The agent is created with default instructions, which can be overridden by client messages
- **Configuration**: `AzureOpenAIClient` with `DefaultAzureCredential` provides secure authentication

### Configure and Run the Server

Set the required environment variables:

```bash
export AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
export AZURE_OPENAI_DEPLOYMENT_NAME="gpt-4o-mini"
```

Run the server:

```bash
dotnet run --urls "http://127.0.0.1:8888"
```

The server will start listening on `http://127.0.0.1:8888`.

## Step 2: Creating an AG-UI Client

The AG-UI client connects to the remote server and displays streaming responses.

### Install Required Packages

Install the AG-UI client library:

```bash
dotnet add package Microsoft.Agents.AI.AGUI
```

### Client Code

Create a file named `Program.cs`:

```csharp
// Copyright (c) Microsoft. All rights reserved.

using System.CommandLine;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.AGUI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var rootCommand = new RootCommand("AGUIClient");
rootCommand.SetAction((_, ct) => HandleCommandsAsync(ct));
return await rootCommand.Parse(args).InvokeAsync();

static async Task HandleCommandsAsync(CancellationToken cancellationToken)
{
    // Set up logging
    using var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Information);
    });
    var logger = loggerFactory.CreateLogger("AGUIClient");

    // Get server URL from configuration or use default
    var configRoot = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();
    
    string serverUrl = configRoot["AGUI_SERVER_URL"] ?? "http://127.0.0.1:8888";
    
    logger.LogInformation("Connecting to AG-UI server at: {ServerUrl}", serverUrl);

    // Create the AG-UI client agent
    using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
    
    var agent = new AGUIAgent(
        id: "agui-client",
        description: "AG-UI Client Agent",
        httpClient: httpClient,
        endpoint: serverUrl);

    var thread = agent.GetNewThread();
    var messages = new List<ChatMessage>
    {
        new(ChatRole.System, "You are a helpful assistant.")
    };

    try
    {
        while (true)
        {
            // Get user input
            Console.Write("\nUser (:q or quit to exit): ");
            string? message = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine("Request cannot be empty.");
                continue;
            }

            if (message is ":q" or "quit")
            {
                break;
            }

            messages.Add(new ChatMessage(ChatRole.User, message));

            // Stream the response
            bool isFirstUpdate = true;
            string? threadId = null;
            
            await foreach (var update in agent.RunStreamingAsync(messages, thread, cancellationToken: cancellationToken))
            {
                var chatUpdate = update.AsChatResponseUpdate();
                
                // First update indicates run started
                if (isFirstUpdate)
                {
                    threadId = chatUpdate.ConversationId;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[Run Started - Thread: {chatUpdate.ConversationId}, Run: {chatUpdate.ResponseId}]");
                    Console.ResetColor();
                    isFirstUpdate = false;
                }
                
                // Display streaming text content
                foreach (var content in update.Contents)
                {
                    if (content is TextContent textContent)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(textContent.Text);
                        Console.ResetColor();
                    }
                    else if (content is ErrorContent errorContent)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\n[Error: {errorContent.Message}]");
                        Console.ResetColor();
                    }
                }
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[Run Finished - Thread: {threadId}]");
            Console.ResetColor();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred");
    }
}
```

### Key Concepts

- **Server-Sent Events (SSE)**: The protocol uses SSE for streaming responses
- **AGUIAgent**: Client class that connects to AG-UI servers
- **RunStreamingAsync**: Streams responses as `AgentRunResponseUpdate` objects
- **AsChatResponseUpdate**: Extension method to access chat-specific properties like `ConversationId` and `ResponseId`
- **Thread Management**: The `AGUIAgentThread` maintains conversation context across requests
- **Content Types**: Responses include `TextContent` for messages and `ErrorContent` for errors

### Configure and Run the Client

Optionally set a custom server URL:

```bash
export AGUI_SERVER_URL="http://127.0.0.1:8888"
```

Run the client (in a separate terminal):

```bash
dotnet run
```

## Step 3: Testing the Complete System

With both the server and client running, you can now test the complete system.

### Expected Output

```
$ dotnet run
info: AGUIClient[0]
      Connecting to AG-UI server at: http://127.0.0.1:8888

User (:q or quit to exit): What is 2 + 2?

[Run Started - Thread: thread_abc123, Run: run_xyz789]
2 + 2 equals 4.
[Run Finished - Thread: thread_abc123]

User (:q or quit to exit): Tell me a fun fact about space

[Run Started - Thread: thread_abc123, Run: run_def456]
Here's a fun fact: A day on Venus is longer than its year! Venus takes
about 243 Earth days to rotate once on its axis, but only about 225 Earth
days to orbit the Sun.
[Run Finished - Thread: thread_abc123]

User (:q or quit to exit): :q
```

### Color-Coded Output

The client displays different content types with distinct colors:

- **Yellow**: Run started notifications
- **Cyan**: Agent text responses (streamed in real-time)
- **Green**: Run completion notifications
- **Red**: Error messages

## How It Works

### Server-Side Flow

1. Client sends HTTP POST request with messages
2. ASP.NET Core endpoint receives the request via `MapAGUI`
3. Agent processes the messages using Agent Framework
4. Responses are converted to AG-UI events
5. Events are streamed back as Server-Sent Events (SSE)
6. Connection closes when the run completes

### Client-Side Flow

1. `AGUIAgent` sends HTTP POST request to server endpoint
2. Server responds with SSE stream
3. Client parses incoming events into `AgentRunResponseUpdate` objects
4. Each update is displayed based on its content type
5. `ConversationId` is captured for conversation continuity
6. Stream completes when run finishes

### Protocol Details

The AG-UI protocol uses:

- HTTP POST for sending requests
- Server-Sent Events (SSE) for streaming responses
- JSON for event serialization
- Thread IDs (as `ConversationId`) for maintaining conversation context
- Run IDs (as `ResponseId`) for tracking individual executions

## Common Patterns

### Custom Server Configuration

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add CORS for web clients
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors();

app.MapAGUI("/agent", agent);
```

### Multiple Agents

```csharp
var weatherAgent = weatherChatClient.CreateAIAgent(name: "weather", instructions: "...");
var financeAgent = financeChatClient.CreateAIAgent(name: "finance", instructions: "...");

app.MapAGUI("/weather", weatherAgent);
app.MapAGUI("/finance", financeAgent);
```

### Error Handling

```csharp
try
{
    await foreach (var update in agent.RunStreamingAsync(messages, thread))
    {
        foreach (var content in update.Contents)
        {
            if (content is ErrorContent errorContent)
            {
                Console.WriteLine($"Error: {errorContent.Message}");
                // Handle error appropriately
            }
        }
    }
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"HTTP error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## Troubleshooting

### Connection Refused

Ensure the server is running before starting the client:

```bash
# Terminal 1
dotnet run --urls "http://127.0.0.1:8888"

# Terminal 2 (after server starts)
dotnet run
```

### Authentication Errors

Make sure you're authenticated with Azure:

```bash
az login
```

Verify you have the correct role assignment on the Azure OpenAI resource.

### Streaming Not Working

Check that your client timeout is sufficient:

```csharp
var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
```

For long-running agents, increase the timeout accordingly.

### Thread Context Lost

The client automatically manages thread continuity. If context is lost:

1. Check that `ConversationId` is being captured from response updates
2. Ensure the same thread instance is used across messages
3. Verify the server is receiving the thread information in subsequent requests

## Next Steps

Now that you understand the basics of AG-UI, you can:

- **[Add Backend Tools](backend-tool-rendering.md)**: Create custom function tools for your domain
- **[Implement Human-in-the-Loop](human-in-the-loop.md)**: Add approval workflows for sensitive operations
- **[Manage State](state-management.md)**: Implement shared state for generative UI applications

## Additional Resources

- [AG-UI Overview](index.md)
- [Agent Framework Documentation](../../overview/agent-framework-overview.md)
- [AG-UI Protocol Specification](https://docs.ag-ui.com/)

::: zone-end

::: zone pivot="programming-language-python"

## Prerequisites

Before you begin, ensure you have the following:

- Python 3.10 or later
- [Azure OpenAI service endpoint and deployment configured](/azure/ai-foundry/openai/how-to/create-resource)
- [Azure CLI installed](/cli/azure/install-azure-cli) and [authenticated](/cli/azure/authenticate-azure-cli)
- User has the `Cognitive Services OpenAI Contributor` role for the Azure OpenAI resource

> [!NOTE]
> These samples use Azure OpenAI models. For more information, see [how to deploy Azure OpenAI models with Azure AI Foundry](/azure/ai-foundry/how-to/deploy-models-openai).

> [!NOTE]
> These samples use `DefaultAzureCredential` for authentication. Make sure you're authenticated with Azure (e.g., via `az login`). For more information, see the [Azure Identity documentation](/python/api/azure-identity/azure.identity.defaultazurecredential).

> [!WARNING]
> The AG-UI protocol is still under development and subject to change. We will keep these samples updated as the protocol evolves.

## Step 1: Creating an AG-UI Server

The AG-UI server hosts your AI agent and exposes it via HTTP endpoints using FastAPI.

### Install Required Packages

Install the necessary packages for the server:

```bash
pip install agent-framework-ag-ui
```

Or using uv:

```bash
uv pip install agent-framework-ag-ui
```

This will automatically install `agent-framework-core`, `fastapi`, and `uvicorn` as dependencies.

### Server Code

Create a file named `server.py`:

```python
"""AG-UI server example."""

import os

from agent_framework import ChatAgent
from agent_framework.azure import AzureOpenAIChatClient
from agent_framework_ag_ui import add_agent_framework_fastapi_endpoint
from fastapi import FastAPI

# Read required configuration
endpoint = os.environ.get("AZURE_OPENAI_ENDPOINT")
deployment_name = os.environ.get("AZURE_OPENAI_DEPLOYMENT_NAME")
# Note: a token may also be used in place of the api_key using:
# from azure.identity import AzureCliCredential
# chat_client = AzureOpenAIChatClient(credential=AzureCliCredential())
api_key = os.environ.get("AZURE_OPENAI_API_KEY")

if not endpoint:
    raise ValueError("AZURE_OPENAI_ENDPOINT environment variable is required")
if not deployment_name:
    raise ValueError("AZURE_OPENAI_DEPLOYMENT_NAME environment variable is required")
if not api_key:
    raise ValueError("AZURE_OPENAI_API_KEY environment variable is required")

# Create the AI agent
agent = ChatAgent(
    name="AGUIAssistant",
    instructions="You are a helpful assistant.",
    chat_client=AzureOpenAIChatClient(
        endpoint=endpoint,
        deployment_name=deployment_name,
        api_key=api_key,
    ),
)

# Create FastAPI app
app = FastAPI(title="AG-UI Server")

# Register the AG-UI endpoint
add_agent_framework_fastapi_endpoint(app, agent, "/")

if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="127.0.0.1", port=8888)
```

### Key Concepts

- **`add_agent_framework_fastapi_endpoint`**: Registers the AG-UI endpoint with automatic request/response handling and SSE streaming
- **`ChatAgent`**: The Agent Framework agent that will handle incoming requests
- **FastAPI Integration**: Uses FastAPI's native async support for streaming responses
- **Instructions**: The agent is created with default instructions, which can be overridden by client messages
- **Configuration**: `AzureOpenAIChatClient` reads from environment variables or accepts parameters directly

### Configure and Run the Server

Set the required environment variables:

```bash
export AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
export AZURE_OPENAI_DEPLOYMENT_NAME="gpt-4o-mini"
```

Run the server:

```bash
python server.py
```

Or using uvicorn directly:

```bash
uvicorn server:app --host 127.0.0.1 --port 8888
```

The server will start listening on `http://127.0.0.1:8888`.

## Step 2: Creating an AG-UI Client

The AG-UI client connects to the remote server and displays streaming responses.

### Install Required Packages

Install the HTTP client library:

```bash
pip install httpx
```

### Client Code

Create a file named `client.py`:

```python
"""AG-UI client example."""

import asyncio
import json
import os
from typing import AsyncIterator

import httpx


class AGUIClient:
    """Simple AG-UI protocol client."""

    def __init__(self, server_url: str):
        """Initialize the client.

        Args:
            server_url: The AG-UI server endpoint URL
        """
        self.server_url = server_url
        self.thread_id: str | None = None

    async def send_message(self, message: str) -> AsyncIterator[dict]:
        """Send a message and stream the response.

        Args:
            message: The user message to send

        Yields:
            AG-UI events from the server
        """
        # Prepare the request
        request_data = {
            "messages": [
                {"role": "system", "content": "You are a helpful assistant."},
                {"role": "user", "content": message},
            ]
        }

        # Include thread_id if we have one (for conversation continuity)
        if self.thread_id:
            request_data["thread_id"] = self.thread_id

        # Stream the response
        async with httpx.AsyncClient(timeout=60.0) as client:
            async with client.stream(
                "POST",
                self.server_url,
                json=request_data,
                headers={"Accept": "text/event-stream"},
            ) as response:
                response.raise_for_status()

                async for line in response.aiter_lines():
                    # Parse Server-Sent Events format
                    if line.startswith("data: "):
                        data = line[6:]  # Remove "data: " prefix
                        try:
                            event = json.loads(data)
                            yield event

                            # Capture thread_id from RUN_STARTED event
                            if event.get("type") == "RUN_STARTED" and not self.thread_id:
                                self.thread_id = event.get("threadId")
                        except json.JSONDecodeError:
                            continue


async def main():
    """Main client loop."""
    # Get server URL from environment or use default
    server_url = os.environ.get("AGUI_SERVER_URL", "http://127.0.0.1:8888/")
    print(f"Connecting to AG-UI server at: {server_url}\n")

    client = AGUIClient(server_url)

    try:
        while True:
            # Get user input
            message = input("\nUser (:q or quit to exit): ")
            if not message.strip():
                print("Request cannot be empty.")
                continue

            if message.lower() in (":q", "quit"):
                break

            # Send message and display streaming response
            print("\n", end="")
            async for event in client.send_message(message):
                event_type = event.get("type", "")

                if event_type == "RUN_STARTED":
                    thread_id = event.get("threadId", "")
                    run_id = event.get("runId", "")
                    print(f"\033[93m[Run Started - Thread: {thread_id}, Run: {run_id}]\033[0m")

                elif event_type == "TEXT_MESSAGE_CONTENT":
                    # Stream text content in cyan
                    print(f"\033[96m{event.get('delta', '')}\033[0m", end="", flush=True)

                elif event_type == "RUN_FINISHED":
                    thread_id = event.get("threadId", "")
                    run_id = event.get("runId", "")
                    print(f"\n\033[92m[Run Finished - Thread: {thread_id}, Run: {run_id}]\033[0m")

                elif event_type == "RUN_ERROR":
                    error_message = event.get("message", "Unknown error")
                    print(f"\n\033[91m[Run Error - Message: {error_message}]\033[0m")

            print()

    except KeyboardInterrupt:
        print("\n\nExiting...")
    except Exception as e:
        print(f"\n\033[91mAn error occurred: {e}\033[0m")


if __name__ == "__main__":
    asyncio.run(main())
```

### Key Concepts

- **Server-Sent Events (SSE)**: The protocol uses SSE format (`data: {json}\n\n`)
- **Event Types**: Different events provide metadata and content (UPPERCASE with underscores):
  - `RUN_STARTED`: Agent has started processing
  - `TEXT_MESSAGE_START`: Start of a text message from the agent
  - `TEXT_MESSAGE_CONTENT`: Incremental text streamed from the agent (with `delta` field)
  - `TEXT_MESSAGE_END`: End of a text message
  - `RUN_FINISHED`: Successful completion
  - `RUN_ERROR`: Error information
- **Field Naming**: Event fields use camelCase (e.g., `threadId`, `runId`, `messageId`)
- **Thread Management**: The `threadId` maintains conversation context across requests
- **Client-Side Instructions**: System messages are sent from the client

### Configure and Run the Client

Optionally set a custom server URL:

```bash
export AGUI_SERVER_URL="http://127.0.0.1:8888/"
```

Run the client (in a separate terminal):

```bash
python client.py
```

## Step 3: Testing the Complete System

With both the server and client running, you can now test the complete system.

### Expected Output

```
$ python client.py
Connecting to AG-UI server at: http://127.0.0.1:8888/

User (:q or quit to exit): What is 2 + 2?

[Run Started - Thread: abc123, Run: xyz789]
2 + 2 equals 4.
[Run Finished - Thread: abc123, Run: xyz789]

User (:q or quit to exit): Tell me a fun fact about space

[Run Started - Thread: abc123, Run: def456]
Here's a fun fact: A day on Venus is longer than its year! Venus takes
about 243 Earth days to rotate once on its axis, but only about 225 Earth
days to orbit the Sun.
[Run Finished - Thread: abc123, Run: def456]

User (:q or quit to exit): :q
```

### Color-Coded Output

The client displays different content types with distinct colors:

- **Yellow**: Run started notifications
- **Cyan**: Agent text responses (streamed in real-time)
- **Green**: Run completion notifications
- **Red**: Error messages

## Testing with curl (Optional)

Before running the client, you can test the server manually using curl:

```bash
curl -N http://127.0.0.1:8888/ \
  -H "Content-Type: application/json" \
  -H "Accept: text/event-stream" \
  -d '{
    "messages": [
      {"role": "user", "content": "What is 2 + 2?"}
    ]
  }'
```

You should see Server-Sent Events streaming back:

```
data: {"type":"RUN_STARTED","threadId":"...","runId":"..."}

data: {"type":"TEXT_MESSAGE_START","messageId":"...","role":"assistant"}

data: {"type":"TEXT_MESSAGE_CONTENT","messageId":"...","delta":"The"}

data: {"type":"TEXT_MESSAGE_CONTENT","messageId":"...","delta":" answer"}

...

data: {"type":"TEXT_MESSAGE_END","messageId":"..."}

data: {"type":"RUN_FINISHED","threadId":"...","runId":"..."}
```

## How It Works

### Server-Side Flow

1. Client sends HTTP POST request with messages
2. FastAPI endpoint receives the request
3. `AgentFrameworkAgent` wrapper orchestrates the execution
4. Agent processes the messages using Agent Framework
5. `AgentFrameworkEventBridge` converts agent updates to AG-UI events
6. Responses are streamed back as Server-Sent Events (SSE)
7. Connection closes when the run completes

### Client-Side Flow

1. Client sends HTTP POST request to server endpoint
2. Server responds with SSE stream
3. Client parses incoming `data:` lines as JSON events
4. Each event is displayed based on its type
5. `threadId` is captured for conversation continuity
6. Stream completes when `RUN_FINISHED` event arrives

### Protocol Details

The AG-UI protocol uses:

- HTTP POST for sending requests
- Server-Sent Events (SSE) for streaming responses
- JSON for event serialization
- Thread IDs for maintaining conversation context
- Run IDs for tracking individual executions
- Event type naming: UPPERCASE with underscores (e.g., `RUN_STARTED`, `TEXT_MESSAGE_CONTENT`)
- Field naming: camelCase (e.g., `threadId`, `runId`, `messageId`)

## Common Patterns

### Custom Server Configuration

```python
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI()

# Add CORS for web clients
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

add_agent_framework_fastapi_endpoint(app, agent, "/agent")
```

### Multiple Agents

```python
app = FastAPI()

weather_agent = ChatAgent(name="weather", ...)
finance_agent = ChatAgent(name="finance", ...)

add_agent_framework_fastapi_endpoint(app, weather_agent, "/weather")
add_agent_framework_fastapi_endpoint(app, finance_agent, "/finance")
```

### Error Handling

```python
try:
    async for event in client.send_message(message):
        if event.get("type") == "RUN_ERROR":
            error_msg = event.get("message", "Unknown error")
            print(f"Error: {error_msg}")
            # Handle error appropriately
except httpx.HTTPError as e:
    print(f"HTTP error: {e}")
except Exception as e:
    print(f"Unexpected error: {e}")
```

## Troubleshooting

### Connection Refused

Ensure the server is running before starting the client:

```bash
# Terminal 1
python server.py

# Terminal 2 (after server starts)
python client.py
```

### Authentication Errors

Make sure you're authenticated with Azure:

```bash
az login
```

Verify you have the correct role assignment on the Azure OpenAI resource.

### Streaming Not Working

Check that your client timeout is sufficient:

```python
httpx.AsyncClient(timeout=60.0)  # 60 seconds should be enough
```

For long-running agents, increase the timeout accordingly.

### Thread Context Lost

The client automatically manages thread continuity. If context is lost:

1. Check that `threadId` is being captured from `RUN_STARTED` events
2. Ensure the same client instance is used across messages
3. Verify the server is receiving the `thread_id` in subsequent requests

## Next Steps

Now that you understand the basics of AG-UI, you can:

- **[Add Backend Tools](backend-tool-rendering.md)**: Create custom function tools for your domain
- **[Implement Human-in-the-Loop](human-in-the-loop.md)**: Add approval workflows for sensitive operations
- **[Manage State](state-management.md)**: Implement shared state for generative UI applications

## Additional Resources

- [AG-UI Overview](index.md)
- [Agent Framework Documentation](../../overview/agent-framework-overview.md)
- [AG-UI Protocol Specification](https://docs.ag-ui.com/)

::: zone-end
