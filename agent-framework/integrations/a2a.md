---
title: A2A Integration
description: Learn how to expose Microsoft Agent Framework agents using the Agent-to-Agent (A2A) protocol for inter-agent communication.
zone_pivot_groups: programming-languages
author: dmkorolev
ms.service: agent-framework
ms.topic: tutorial
ms.date: 02/11/2026
ms.author: dmkorolev
---

# A2A Integration

The Agent-to-Agent (A2A) protocol enables standardized communication between agents, allowing agents built with different frameworks and technologies to communicate seamlessly.

## What is A2A?

A2A is a standardized protocol that supports:

- **Agent discovery** through agent cards
- **Message-based communication** between agents
- **Long-running agentic processes** via tasks
- **Cross-platform interoperability** between different agent frameworks

For more information, see the [A2A protocol specification](https://a2a-protocol.org/latest/).

::: zone pivot="programming-language-csharp"

The `Microsoft.Agents.AI.Hosting.A2A.AspNetCore` library provides ASP.NET Core integration for exposing your agents via the A2A protocol.

**NuGet Packages:**
- [Microsoft.Agents.AI.Hosting.A2A](https://www.nuget.org/packages/Microsoft.Agents.AI.Hosting.A2A)
- [Microsoft.Agents.AI.Hosting.A2A.AspNetCore](https://www.nuget.org/packages/Microsoft.Agents.AI.Hosting.A2A.AspNetCore)

## Example

This minimal example shows how to expose an agent via A2A. The sample includes OpenAPI and Swagger dependencies to simplify testing.

#### 1. Create an ASP.NET Core Web API project

Create a new ASP.NET Core Web API project or use an existing one.

#### 2. Install required dependencies

Install the following packages:

  ## [.NET CLI](#tab/dotnet-cli)
  
  Run the following commands in your project directory to install the required NuGet packages:
  
  ```bash
  # Hosting.A2A.AspNetCore for A2A protocol integration
  dotnet add package Microsoft.Agents.AI.Hosting.A2A.AspNetCore --prerelease

  # Libraries to connect to Microsoft Foundry
  dotnet add package Azure.AI.Projects --prerelease
  dotnet add package Azure.Identity
  dotnet add package Microsoft.Agents.AI.Foundry --prerelease

  # Swagger to test app
  dotnet add package Microsoft.AspNetCore.OpenApi
  dotnet add package Swashbuckle.AspNetCore
  ```

  ---

#### 3. Configure Azure OpenAI connection

The application requires an Azure OpenAI connection. Configure the endpoint and deployment name using `dotnet user-secrets` or environment variables.
You can also simply edit the `appsettings.json`, but that's not recommended for the apps deployed in production since some of the data can be considered to be secret.

  ## [User-Secrets](#tab/user-secrets)
  ```bash
  dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "https://<your-openai-resource>.openai.azure.com/"
  dotnet user-secrets set "AZURE_OPENAI_DEPLOYMENT_NAME" "gpt-4o-mini"
  ```
  ## [ENV Windows](#tab/env-windows)
  ```powershell
  $env:AZURE_OPENAI_ENDPOINT = "https://<your-openai-resource>.openai.azure.com/"
  $env:AZURE_OPENAI_DEPLOYMENT_NAME = "gpt-4o-mini"
  ```
  ## [ENV unix](#tab/env-unix)
  ```bash
  export AZURE_OPENAI_ENDPOINT="https://<your-openai-resource>.openai.azure.com/"
  export AZURE_OPENAI_DEPLOYMENT_NAME="gpt-4o-mini"
  ```
  ## [appsettings](#tab/appsettings)
  ```json
    "AZURE_OPENAI_ENDPOINT": "https://<your-openai-resource>.openai.azure.com/",
    "AZURE_OPENAI_DEPLOYMENT_NAME": "gpt-4o-mini"
  ```

  ---


#### 4. Add the code to Program.cs

Replace the contents of `Program.cs` with the following code and run the application:
```csharp
using A2A.AspNetCore;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

string endpoint = builder.Configuration["AZURE_OPENAI_ENDPOINT"]
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
string deploymentName = builder.Configuration["AZURE_OPENAI_DEPLOYMENT_NAME"]
    ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME is not set.");

// Register the chat client
IChatClient chatClient = new AIProjectClient(
        new Uri(endpoint),
        new DefaultAzureCredential())
        .GetProjectOpenAIClient()
        .GetResponsesClient()
        .AsIChatClient(deploymentName);

builder.Services.AddSingleton(chatClient);

// Register an agent
var pirateAgent = builder.AddAIAgent("pirate", instructions: "You are a pirate. Speak like a pirate.");

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

// Expose the agent via A2A protocol. You can also customize the agentCard
app.MapA2A(pirateAgent, path: "/a2a/pirate", agentCard: new()
{
    Name = "Pirate Agent",
    Description = "An agent that speaks like a pirate.",
    Version = "1.0"
});

app.Run();
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

### Testing the Agent

Once the application is running, you can test the A2A agent using the following `.http` file or through Swagger UI.

The input format complies with the A2A specification. You can provide values for:
- `messageId` - A unique identifier for this specific message. You can create your own ID (e.g., a GUID) or set it to `null` to let the agent generate one automatically.
- `contextId` - The conversation identifier. Provide your own ID to start a new conversation or continue an existing one by reusing a previous `contextId`. The agent will maintain conversation history for the same `contextId`. Agent will generate one for you as well, if none is provided.

```http
# Send A2A request to the pirate agent
POST {{baseAddress}}/a2a/pirate/v1/message:stream
Content-Type: application/json
{
  "message": {
    "kind": "message",
    "role": "user",
    "parts": [
      {
        "kind": "text",
        "text": "Hey pirate! Tell me where have you been",
        "metadata": {}
      }
    ],
	"messageId": null,
    "contextId": "foo"
  }
}
```
_Note: Replace `{{baseAddress}}` with your server endpoint._

This request returns the following JSON response:
```json
{
	"kind": "message",
	"role": "agent",
	"parts": [
		{
			"kind": "text",
			"text": "Arrr, ye scallywag! Ye’ll have to tell me what yer after, or be I walkin’ the plank? 🏴‍☠️"
		}
	],
	"messageId": "chatcmpl-CXtJbisgIJCg36Z44U16etngjAKRk",
	"contextId": "foo"
}
```

The response includes the `contextId` (conversation identifier), `messageId` (message identifier), and the actual content from the pirate agent.

## AgentCard Configuration

The `AgentCard` provides metadata about your agent for discovery and integration:
```csharp
app.MapA2A(agent, "/a2a/my-agent", agentCard: new()
{
    Name = "My Agent",
    Description = "A helpful agent that assists with tasks.",
    Version = "1.0",
});
```

You can access the agent card by sending this request:
```http
# Send A2A request to the pirate agent
GET {{baseAddress}}/a2a/pirate/v1/card
```
_Note: Replace `{{baseAddress}}` with your server endpoint._

### AgentCard Properties

- **Name**: Display name of the agent
- **Description**: Brief description of the agent
- **Version**: Version string for the agent
- **Url**: Endpoint URL (automatically assigned if not specified)
- **Capabilities**: Optional metadata about streaming, push notifications, and other features

## Exposing Multiple Agents

You can expose multiple agents in a single application, as long as their endpoints don't collide. Here's an example:

```csharp
var mathAgent = builder.AddAIAgent("math", instructions: "You are a math expert.");
var scienceAgent = builder.AddAIAgent("science", instructions: "You are a science expert.");

app.MapA2A(mathAgent, "/a2a/math");
app.MapA2A(scienceAgent, "/a2a/science");
```

::: zone-end

::: zone pivot="programming-language-python"

The `agent-framework-a2a` package lets you connect to and communicate with external A2A-compliant agents.

```bash
pip install agent-framework-a2a --pre
```

## Connecting to an A2A Agent

Use `A2AAgent` to wrap any remote A2A endpoint. The agent resolves the remote agent's capabilities via its AgentCard and handles all protocol details.

```python
import asyncio
import httpx
from a2a.client import A2ACardResolver
from agent_framework.a2a import A2AAgent

async def main():
    a2a_host = "https://your-a2a-agent.example.com"

    # 1. Discover the remote agent's capabilities
    async with httpx.AsyncClient(timeout=60.0) as http_client:
        resolver = A2ACardResolver(httpx_client=http_client, base_url=a2a_host)
        agent_card = await resolver.get_agent_card()
        print(f"Found agent: {agent_card.name}")

    # 2. Create an A2AAgent and send a message
    async with A2AAgent(
        name=agent_card.name,
        agent_card=agent_card,
        url=a2a_host,
    ) as agent:
        response = await agent.run("What are your capabilities?")
        for message in response.messages:
            print(message.text)

asyncio.run(main())
```

### Streaming Responses

A2A naturally supports streaming via Server-Sent Events — updates arrive in real time as the remote agent works:

```python
async with A2AAgent(name="remote", url="https://a2a-agent.example.com") as agent:
    async with agent.run("Tell me about yourself", stream=True) as stream:
        async for update in stream:
            for content in update.contents:
                if content.text:
                    print(content.text, end="", flush=True)

        final = await stream.get_final_response()
        print(f"\n({len(final.messages)} message(s))")
```

### Long-Running Tasks

By default, `A2AAgent` waits for the remote agent to finish before returning. For long-running tasks, set `background=True` to get a continuation token you can use to poll or resubscribe later:

```python
async with A2AAgent(name="worker", url="https://a2a-agent.example.com") as agent:
    # Start a long-running task
    response = await agent.run("Process this large dataset", background=True)

    if response.continuation_token:
        # Poll for completion later
        result = await agent.poll_task(response.continuation_token)
        print(result)
```

### Authentication

Use an `AuthInterceptor` for secured A2A endpoints:

```python
from a2a.client.auth.interceptor import AuthInterceptor

class BearerAuth(AuthInterceptor):
    def __init__(self, token: str):
        self.token = token

    async def intercept(self, request):
        request.headers["Authorization"] = f"Bearer {self.token}"
        return request

async with A2AAgent(
    name="secure-agent",
    url="https://secure-a2a-agent.example.com",
    auth_interceptor=BearerAuth("your-token"),
) as agent:
    response = await agent.run("Hello!")
```

::: zone-end

## See Also

- [Integrations Overview](./index.md)
- [OpenAI Integration](./openai-endpoints.md)
- [A2A Protocol Specification](https://a2a-protocol.org/latest/)
- [Agent Discovery](https://github.com/a2aproject/A2A/blob/main/docs/topics/agent-discovery.md)

## Next steps

> [!div class="nextstepaction"]
> [AG-UI Protocol](ag-ui/index.md)
