---
title: Foundry Hosted Agents
description: Learn how to host Agent Framework agents in Microsoft Foundry Agent Service as containerized, managed hosted agents.
zone_pivot_groups: programming-languages
author: taochen
ms.topic: conceptual
ms.author: taochen
ms.date: 04/27/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                         | C# | Python | Notes |
  |---------------------------------|:--:|:------:|-------|
  | Overview                        | ✅ |   ✅   |       |
  | Prerequisites                   | ✅ |   ✅   |       |
  | Responses protocol              | ✅ |   ✅   |       |
  | Invocations protocol            | ✅ |   ✅   |       |
  | Running locally                 | ✅ |   ✅   |       |
  | Deploying to Foundry            | ✅ |   ✅   |       |
-->

# Foundry Hosted Agents

[Hosted agents](https://learn.microsoft.com/azure/foundry/agents/concepts/hosted-agents) in Microsoft Foundry Agent Service let you deploy Agent Framework agents as containerized applications to Microsoft-managed infrastructure. The platform handles scaling, session state persistence, security, and lifecycle management so you can focus on your agent's logic.

With the Agent Framework hosting integration, you can take any `Agent` or workflow and expose it through the Foundry Responses or Invocations protocol with minimal code.

## When to use hosted agents

Choose Foundry hosted agents when you want:

- **Managed infrastructure** — no need to configure containers, web servers, or scaling rules yourself.
- **Built-in session management** — the platform persists `$HOME` and uploaded files across turns and idle periods.
- **Dedicated agent identity** — every deployed agent gets its own Entra identity for secure access to models, tools, and downstream services.
- **OpenAI-compatible endpoints** — clients can interact with your agent using any OpenAI-compatible SDK through the Responses protocol.

> [!NOTE]
> Foundry hosted agents are currently in preview. See the [Foundry hosted agents documentation](https://learn.microsoft.com/azure/foundry/agents/concepts/hosted-agents#limits-pricing-and-availability-preview) for the latest availability, limits, and pricing.

## Prerequisites

- An Azure subscription
- [Azure Developer CLI (`azd`)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) with the AI agent extension: `azd ext install azure.ai.agents`

For local testing, you also need:

- A [Microsoft Foundry](https://learn.microsoft.com/azure/foundry/) project with a model deployment (for example, `gpt-4o`)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) installed and authenticated (`az login`)

:::zone pivot="programming-language-csharp"

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

Install the hosting NuGet package:

```dotnetcli
dotnet add package Microsoft.Agents.AI.Foundry.Hosting --prerelease
dotnet add package Azure.AI.Projects --prerelease
```

:::zone-end

:::zone pivot="programming-language-python"

- Python 3.10 or later

Install the hosting Python package:

```bash
pip install agent-framework agent-framework-foundry-hosting
```

:::zone-end

## Responses protocol

The **Responses** protocol is the recommended starting point for most agents. It exposes an OpenAI-compatible `/responses` endpoint, and the platform manages conversation history, streaming, and session lifecycle automatically.

:::zone pivot="programming-language-csharp"

```csharp
using Azure.AI.AgentServer.Core;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry.Hosting;

var projectEndpoint = new Uri(Environment.GetEnvironmentVariable("FOUNDRY_PROJECT_ENDPOINT")
    ?? throw new InvalidOperationException("FOUNDRY_PROJECT_ENDPOINT is not set."));
var deployment = Environment.GetEnvironmentVariable("AZURE_AI_MODEL_DEPLOYMENT_NAME") ?? "gpt-4o";

AIAgent agent = new AIProjectClient(projectEndpoint, new DefaultAzureCredential())
    .AsAIAgent(
        model: deployment,
        instructions: "You are a helpful AI assistant.",
        name: "my-agent");

var builder = AgentHost.CreateBuilder(args);
builder.Services.AddFoundryResponses(agent);
builder.RegisterProtocol("responses", endpoints => endpoints.MapFoundryResponses());

var app = builder.Build();
app.Run();
```

The `AgentHost.CreateBuilder` creates an application host preconfigured for the Foundry hosting environment. `AddFoundryResponses` registers your agent with the Responses protocol handler, and `MapFoundryResponses` maps the `/responses` HTTP endpoint.

:::zone-end

:::zone pivot="programming-language-python"

```python
import os

from agent_framework import Agent
from agent_framework.foundry import FoundryChatClient
from agent_framework_foundry_hosting import ResponsesHostServer
from azure.identity import DefaultAzureCredential

client = FoundryChatClient(
    project_endpoint=os.environ["FOUNDRY_PROJECT_ENDPOINT"],
    model=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
    credential=DefaultAzureCredential(),
)

agent = Agent(
    client=client,
    instructions="You are a helpful AI assistant.",
    default_options={"store": False},
)

server = ResponsesHostServer(agent)
server.run()
```

The `ResponsesHostServer` wraps your agent and exposes it through the Foundry Responses protocol. Setting `store` to `False` in `default_options` avoids duplicating conversation history, since the hosting infrastructure manages history automatically.

:::zone-end

## Invocations protocol

The **Invocations** protocol gives you full control over the HTTP request and response. Use it when you need custom payloads, non-conversational processing, or streaming protocols that aren't OpenAI-compatible.

:::zone pivot="programming-language-csharp"

With the Invocations protocol in C#, you implement a custom `InvocationHandler` to process incoming requests:

```csharp
using Azure.AI.AgentServer.Core;
using Azure.AI.AgentServer.Invocations;
using Microsoft.Agents.AI;

var builder = AgentHost.CreateBuilder(args);

builder.Services.AddSingleton<AIAgent, MyAgent>();
builder.Services.AddInvocationsServer();
builder.Services.AddScoped<InvocationHandler, MyInvocationHandler>();

builder.RegisterProtocol("invocations", endpoints => endpoints.MapInvocationsServer());

var app = builder.Build();
app.Run();
```

The `AddInvocationsServer` method registers the Invocations protocol services. You implement `InvocationHandler` to define how your agent processes each request.

:::zone-end

:::zone pivot="programming-language-python"

For a lightweight setup, use `InvocationsHostServer` from the `agent_framework_foundry_hosting` package. It wraps your agent similarly to `ResponsesHostServer` and handles session management automatically:

```python
import os

from agent_framework import Agent
from agent_framework.foundry import FoundryChatClient
from agent_framework_foundry_hosting import InvocationsHostServer
from azure.identity import DefaultAzureCredential

client = FoundryChatClient(
    project_endpoint=os.environ["FOUNDRY_PROJECT_ENDPOINT"],
    model=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
    credential=DefaultAzureCredential(),
)

agent = Agent(
    client=client,
    instructions="You are a friendly assistant. Keep your answers brief.",
    default_options={"store": False},
)

server = InvocationsHostServer(agent)
server.run()
```

For full control over request handling, use `InvocationAgentServerHost` from the `azure.ai.agentserver.invocations` package directly and implement your own invoke handler:

```python
import os
from collections.abc import AsyncGenerator

from agent_framework import Agent, AgentSession
from agent_framework.foundry import FoundryChatClient
from azure.ai.agentserver.invocations import InvocationAgentServerHost
from azure.identity import DefaultAzureCredential
from starlette.requests import Request
from starlette.responses import JSONResponse, Response, StreamingResponse

_sessions: dict[str, AgentSession] = {}

client = FoundryChatClient(
    project_endpoint=os.environ["FOUNDRY_PROJECT_ENDPOINT"],
    model=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
    credential=DefaultAzureCredential(),
)

agent = Agent(
    client=client,
    instructions="You are a friendly assistant. Keep your answers brief.",
    default_options={"store": False},
)

app = InvocationAgentServerHost()


@app.invoke_handler
async def handle_invoke(request: Request):
    """Handle streaming multi-turn chat."""
    data = await request.json()
    session_id = request.state.session_id
    stream = data.get("stream", False)
    user_message = data.get("message", None)

    if user_message is None:
        return Response(content="Missing 'message' in request", status_code=400)

    session = _sessions.setdefault(session_id, AgentSession(session_id=session_id))

    if stream:

        async def stream_response() -> AsyncGenerator[str]:
            async for update in agent.run(user_message, session=session, stream=True):
                yield update.text

        return StreamingResponse(
            stream_response(),
            media_type="text/event-stream",
            headers={"Cache-Control": "no-cache", "Connection": "keep-alive"},
        )

    response = await agent.run([user_message], session=session, stream=stream)
    return JSONResponse({"response": response.text})


if __name__ == "__main__":
    app.run()
```

> [!WARNING]
> The in-memory session store in the custom handler example is lost on restart. Use durable storage (for example, Cosmos DB) in production.

:::zone-end

> [!TIP]
> Refer the [Python samples](https://github.com/microsoft-foundry/foundry-samples/tree/main/samples/python/hosted-agents/agent-framework) or the [C# samples](https://github.com/microsoft-foundry/foundry-samples/tree/main/samples/csharp/hosted-agents/agent-framework) for examples of a hosted agent project. Or use the `azd ai agent init` command to scaffold a new hosted agent project from scratch. Refer to this [quickstart guide](https://learn.microsoft.com/azure/foundry/agents/quickstarts/quickstart-hosted-agent?pivots=azd) for step-by-step instructions.

## Running locally

The Azure Developer CLI (`azd`) provides the easiest way to run and test your hosted agent locally.

### Initialize a project

Create a new folder and initialize from a sample manifest:

```bash
mkdir my-hosted-agent && cd my-hosted-agent
azd ai agent init -m <path-to-agent.manifest.yaml>
```

> [!TIP]
> The manifest can be a path to a local YAML file or a URL to a remote manifest.

### Set environment variables

```bash
export FOUNDRY_PROJECT_ENDPOINT="https://<account>.services.ai.azure.com/api/projects/<project>"
export AZURE_AI_MODEL_DEPLOYMENT_NAME="<your-model-deployment>"
```

### Run the agent host

```bash
azd ai agent run
```

The agent host starts on `http://localhost:8088`.

### Invoke the agent

```bash
azd ai agent invoke --local "Hello!"
```

Or use `curl`:

```bash
curl -X POST http://localhost:8088/responses \
  -H "Content-Type: application/json" \
  -d '{"input": "Hello!"}'
```

Or in PowerShell:

```powershell
(Invoke-WebRequest -Uri http://localhost:8088/responses -Method POST -ContentType "application/json" -Body '{"input": "Hello!"}').Content
```

## Deploying to Foundry

Once you've verified your agent locally, deploy it to Microsoft Foundry:

1. **Provision resources** (if you don't already have a Foundry project):

   ```bash
   azd provision
   ```

   This creates a resource group with a Foundry instance, project, model deployment, Application Insights, and a container registry.

2. **Deploy the agent:**

   ```bash
   azd deploy
   ```

   This packages your agent as a container image, pushes it to Azure Container Registry, and deploys it to Foundry Agent Service.

The Foundry hosting infrastructure automatically injects the following environment variables into your agent container at runtime:

| Variable | Description |
|----------|-------------|
| `FOUNDRY_PROJECT_ENDPOINT` | The endpoint URL for the Foundry project. |
| `AZURE_AI_MODEL_DEPLOYMENT_NAME` | The model deployment name (configured during `azd ai agent init`). |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | The Application Insights connection string for telemetry. |

Once deployed, your agent is accessible through its dedicated Foundry endpoint and can also be tested from the Foundry portal.

## Next steps

> [!div class="nextstepaction"]
> [Hosted agents concepts](https://learn.microsoft.com/azure/foundry/agents/concepts/hosted-agents)

- [Deploy a hosted agent with the Foundry SDK](https://learn.microsoft.com/azure/foundry/agents/how-to/deploy-hosted-agent)
- [Manage hosted agents](https://learn.microsoft.com/azure/foundry/agents/how-to/manage-hosted-agent)
- [Azure Functions (Durable) hosting](../integrations/azure-functions.md)
- [A2A Hosting](agent-to-agent.md)
- [Python samples](https://github.com/microsoft-foundry/foundry-samples/tree/main/samples/python/hosted-agents/agent-framework)
- [C# samples](https://github.com/microsoft-foundry/foundry-samples/tree/main/samples/csharp/hosted-agents/agent-framework)
