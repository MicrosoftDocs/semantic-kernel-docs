---
title: Getting Started with AG-UI
description: Step-by-step tutorial to build your first AG-UI server and client with Agent Framework
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 08/11/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                 | C# | Python | Go | Notes |
  |-------------------------|:--:|:------:|:--:|-------|
  | Server setup            | ✅ |   ✅   | ✅ | Language-specific hosting APIs |
  | Client setup            | ✅ |   ✅   | ✅ |       |
  | Conversation continuity | ✅ |   ✅   | ❌ | Not documented for Go |
-->

# Getting Started with AG-UI

This tutorial demonstrates how to build server and client applications using the AG-UI protocol with Agent Framework. You'll learn how to host an agent behind an AG-UI endpoint and connect a client for interactive conversations.

## What You'll Build

By the end of this tutorial, you'll have:

- An AG-UI server hosting an AI agent accessible via HTTP
- A client application that connects to the server and streams responses
- Understanding of how the AG-UI protocol works with Agent Framework

::: zone pivot="programming-language-csharp"

## Prerequisites

- .NET 8 or later
- An ASP.NET Core project
- A configured MAF `AIAgent`

The example uses Azure OpenAI, but `MapAGUIServer` works with any MAF agent.

## Create an AG-UI server

Install the hosting package:

```dotnetcli
dotnet add package Microsoft.Agents.AI.Hosting.AGUI.AspNetCore --prerelease
```

Register AG-UI hosting and map your agent:

```csharp
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddAGUIServer();

AIAgent agent = CreateAgent();

WebApplication app = builder.Build();
app.MapAGUIServer("/", agent);
await app.RunAsync();
```

`MapAGUIServer` accepts AG-UI `RunAgentInput` requests and streams the agent's response as AG-UI events over server-sent events (SSE).

Run the server on the URL used by the client example:

```dotnetcli
dotnet run --urls http://localhost:8888
```

> [!TIP]
> See the [.NET getting-started sample](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/02-agents/AGUI/Step01_GettingStarted) for a complete server and console client.

## Connect with a .NET client

The AG-UI .NET SDK provides `AGUIChatClient`, which implements `IChatClient` and can be adapted to a MAF agent:

```dotnetcli
dotnet add package AGUI.Client --prerelease
dotnet add package Microsoft.Agents.AI --prerelease
```

```csharp
using AGUI.Abstractions;
using AGUI.Client;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

using HttpClient httpClient = new() { BaseAddress = new Uri("http://localhost:8888") };
AGUIChatClient chatClient = new(new AGUIChatClientOptions(httpClient, "/"));
AIAgent remoteAgent = chatClient.AsAIAgent();
AgentSession session = await remoteAgent.CreateSessionAsync();

List<AgentResponseUpdate> firstTurnUpdates = [];
await foreach (AgentResponseUpdate update in
    remoteAgent.RunStreamingAsync("Hello", session))
{
    firstTurnUpdates.Add(update);

    foreach (TextContent text in update.Contents.OfType<TextContent>())
    {
        Console.Write(text.Text);
    }
}
```

You can also connect with any client that implements the AG-UI protocol.

## Conversation continuity

AG-UI uses `threadId` and `parentRunId` to identify continuation requests. These identifiers are protocol data, not authorization credentials.

`AGUIChatClient` is stateless. To continue a server-owned conversation, get the identifiers from the first turn's `RunStartedEvent`, then include the same `threadId` and the previous `runId` as `parentRunId` on the next request:

```csharp
RunStartedEvent started = firstTurnUpdates
    .Select(update => update.AsChatResponseUpdate().RawRepresentation)
    .OfType<RunStartedEvent>()
    .FirstOrDefault()
    ?? throw new InvalidOperationException("The server didn't return a run-started event.");

ChatMessage nextMessage = new(ChatRole.User, "What did I just say?");
ChatClientAgentRunOptions continuationOptions = new()
{
    ChatOptions = new ChatOptions
    {
        RawRepresentationFactory = _ => new RunAgentInput
        {
            ThreadId = started.ThreadId,
            ParentRunId = started.RunId,
            Messages = new[] { nextMessage }.AsAGUIMessages().ToList(),
        },
    },
};

await foreach (AgentResponseUpdate update in
    remoteAgent.RunStreamingAsync([nextMessage], session, continuationOptions))
{
    // Process the continued response.
}
```

Send only the new messages in a continuation request. `MapAGUIServer` uses `threadId` to select the hosted agent session and `parentRunId` to identify the run being continued. Without hosted session persistence, each request receives a new server session; the client can instead resend conversation history.

To retain server-owned `AgentSession` state across requests, configure [hosted session persistence and isolation](../../../../hosting/self-hosting/index.md#persist-hosted-sessions), then map the named hosted agent with `MapAGUIServer`. For the AG-UI-specific trust boundary, see [Production and security considerations](./security-considerations.md).

## Next steps

> [!div class="nextstepaction"]
> [Use backend tools with AG-UI](./backend-tool-rendering.md)

## Related resources

- [AG-UI overview](./index.md)
- [MAF hosting](../../../../hosting/index.md)
- [AG-UI protocol documentation](https://docs.ag-ui.com/)

::: zone-end

::: zone pivot="programming-language-python"

## Prerequisites

Before you begin, ensure you have the following:

- Python 3.10 or later
- [Azure OpenAI service endpoint and deployment configured](/azure/ai-foundry/openai/how-to/create-resource)
- [Azure CLI installed](/cli/azure/install-azure-cli) and [authenticated](/cli/azure/authenticate-azure-cli)
- User has the `Cognitive Services OpenAI Contributor` role for the Azure OpenAI resource

> [!NOTE]
> These samples use Azure OpenAI models. For more information, see [how to deploy Azure OpenAI models with Foundry](/azure/ai-foundry/how-to/deploy-models-openai).

> [!NOTE]
> These samples use `DefaultAzureCredential` for authentication. Make sure you're authenticated with Azure (e.g., via `az login`). For more information, see the [Azure Identity documentation](/python/api/azure-identity/azure.identity.defaultazurecredential).

> [!WARNING]
> The AG-UI protocol is still under development and subject to change. We will keep these samples updated as the protocol evolves.

## Step 1: Creating an AG-UI Server

The AG-UI server hosts your AI agent and exposes it via HTTP endpoints using FastAPI.

### Install Required Packages

Install the necessary packages for the server:

```bash
pip install agent-framework-ag-ui --pre
```

Or using uv:

```bash
uv pip install agent-framework-ag-ui --prerelease=allow
```

This will automatically install `agent-framework-core`, `fastapi`, `uvicorn`, and `sse-starlette` as dependencies.

### Server Code

Create a file named `server.py`:

```python
"""AG-UI server example."""

import os

from agent_framework import Agent
from agent_framework.openai import OpenAIChatCompletionClient
from agent_framework_ag_ui import add_agent_framework_fastapi_endpoint
from azure.identity import AzureCliCredential
from fastapi import FastAPI

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

# Create the AI agent
agent = Agent(
    name="AGUIAssistant",
    instructions="You are a helpful assistant.",
    client=chat_client,
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
- **`Agent`**: The Agent Framework agent that will handle incoming requests
- **FastAPI Integration**: Uses FastAPI's native async support for streaming responses
- **Instructions**: The agent is created with default instructions, which can be overridden by client messages
- **Configuration**: `OpenAIChatCompletionClient` accepts explicit Azure routing inputs such as `model`, `azure_endpoint`, `api_version`, and `credential`, and can also read from environment variables

### Configure and Run the Server

Set the required environment variables:

```bash
export AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
export AZURE_OPENAI_CHAT_COMPLETION_MODEL="gpt-4o-mini"
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

The AG-UI package is already installed, which includes the `AGUIChatClient`:

```bash
# Already installed with agent-framework-ag-ui
pip install agent-framework-ag-ui --pre
```

### Client Code

Create a file named `client.py`:

```python
"""AG-UI client example."""

import asyncio
import os

from agent_framework import Agent
from agent_framework_ag_ui import AGUIChatClient


async def main():
    """Main client loop."""
    # Get server URL from environment or use default
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
            # Get user input
            message = input("\nUser (:q or quit to exit): ")
            if not message.strip():
                print("Request cannot be empty.")
                continue

            if message.lower() in (":q", "quit"):
                break

            # Stream the agent response
            print("\nAssistant: ", end="", flush=True)
            async for update in agent.run(message, session=thread, stream=True):
                # Print text content as it streams
                if update.text:
                    print(f"\033[96m{update.text}\033[0m", end="", flush=True)

            print("\n")

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

For an idle stream, curl may also display `: keepalive` comment lines. These are SSE transport comments, not AG-UI events.

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
- SSE keepalive comments every 15 seconds while a stream is idle. Clients that process only `data:` lines ignore
  these comments automatically.

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

add_agent_framework_fastapi_endpoint(
    app,
    agent,
    "/agent",
    keepalive_seconds=30,  # Defaults to 15; set to None to disable
)
```

`keepalive_seconds` must be a positive number or `None`.

### Multiple Agents

```python
app = FastAPI()

weather_agent = Agent(name="weather", ...)
finance_agent = Agent(name="finance", ...)

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

Idle streams emit an SSE keepalive comment every 15 seconds by default. If a proxy closes idle connections sooner,
configure a smaller positive `keepalive_seconds` value when registering the endpoint.

### Thread Context Lost

The client automatically manages thread continuity. If context is lost:

1. Check that `threadId` is being captured from `RUN_STARTED` events
2. Ensure the same client instance is used across messages
3. Verify the server is receiving the `thread_id` in subsequent requests

## Next Steps

Now that you understand the basics of AG-UI, you can:

- **[Add Backend Tools](backend-tool-rendering.md)**: Create custom function tools for your domain
<!-- - **[Implement Human-in-the-Loop](human-in-the-loop.md)**: Add approval workflows for sensitive operations -->
<!-- - **[Manage State](state-management.md)**: Implement shared state for generative UI applications -->

## Additional Resources

- [AG-UI Overview](index.md)
- [Agent Framework Documentation](../../../../overview/index.md)
- [AG-UI Protocol Specification](https://docs.ag-ui.com/)

::: zone-end

::: zone pivot="programming-language-go"

Go supports AG-UI through `provider/aguiprovider` for both servers and clients.

```go
import "github.com/microsoft/agent-framework-go/provider/aguiprovider"

mux := http.NewServeMux()
mux.Handle("/", aguiprovider.NewJSONHTTPHandler(myAgent, aguiprovider.HandlerConfig{}))

if err := http.ListenAndServe(":8888", mux); err != nil {
    log.Fatal(err)
}
```

Use `aguiprovider.NewAgent` when your Go app needs to call an AG-UI server as an agent:

```go
import aguiSSEClient "github.com/ag-ui-protocol/ag-ui/sdks/community/go/pkg/client/sse"

a := aguiprovider.NewAgent(
    aguiSSEClient.NewClient(aguiSSEClient.Config{Endpoint: serverURL}),
    aguiprovider.AgentConfig{},
)
```

> [!TIP]
> See the [AG-UI getting started server](https://github.com/microsoft/agent-framework-go/blob/main/examples/02-agents/agui/step01_getting_started/server/main.go) and [client](https://github.com/microsoft/agent-framework-go/blob/main/examples/02-agents/agui/step01_getting_started/client/main.go) samples for complete runnable examples.

::: zone-end