---
title: Microsoft Foundry
description: Learn how to use Microsoft Agent Framework with Microsoft Foundry project endpoints and the Foundry Agent Service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 04/02/2026
ms.service: agent-framework
---

# Microsoft Foundry

Microsoft Agent Framework supports both direct model inference from Microsoft Foundry project endpoints and service-managed agents in the [Foundry Agent Service](/azure/ai-foundry/agents/overview).

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```dotnetcli
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.Foundry --prerelease
```

## Two agent types

The Microsoft Foundry integration exposes two distinct usage patterns:

| Type | Produced type | Description | Use when |
|---|---|---|---|
| **Responses Agent** | `ChatClientAgent` | Your app programmatically provides a model, instructions, and tools at runtime via `AIProjectClient.AsAIAgent(...)`. No server-side agent resource is created. | You own the agent definition and want a simple, flexible setup. This is the pattern used in most samples. |
| **Foundry Agent** (versioned) | `FoundryAgent` | Server-managed — agent definitions are created and versioned either through the Foundry portal or programmatically via `AIProjectClient.AgentAdministrationClient`. Pass a `ProjectsAgentVersion` or `ProjectsAgentRecord` or `AgentReference` to `AIProjectClient.AsAIAgent(...)`. | You need strict, versioned agent definitions managed in the Foundry portal, through service APIs |

## Responses Agent (direct inference)

Use `AsAIAgent` on `AIProjectClient` directly with a model and instructions. This is the recommended starting point for most scenarios.

```csharp
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;

AIAgent agent = new AIProjectClient(
    new Uri("<your-foundry-project-endpoint>"),
    new DefaultAzureCredential())
        .AsAIAgent(
            model: "gpt-4o-mini",
            name: "Joker",
            instructions: "You are good at telling jokes.");

Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

This path is code-first and does not create a server-managed agent resource.

## Foundry Agent (versioned)

Use the native `AIProjectClient.AgentAdministrationClient` APIs from the AI Projects SDK to retrieve versioned agent resources, then wrap them with `AsAIAgent`. Agents can be created and configured directly in the Foundry portal or programmatically via `AIProjectClient.AgentAdministrationClient`.

```csharp
using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry;

var aiProjectClient = new AIProjectClient(
    new Uri("<your-foundry-project-endpoint>"),
    new DefaultAzureCredential());

// Retrieve an existing agent by name (uses the latest version automatically)
ProjectsAgentRecord jokerRecord = await aiProjectClient.AgentAdministrationClient.GetAgentAsync("Joker");
FoundryAgent agent = aiProjectClient.AsAIAgent(jokerRecord);

Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));
```

> [!IMPORTANT]
> Foundry Agents tools and instructions are strict to the ones it was created with, attempting to modify tooling or instructions at runtime is not supported.

## Using the agent

Both `ChatClientAgent` (Responses) and `FoundryAgent` (versioned) are standard `AIAgent` instances and support all standard operations including sessions, tools, middleware, and streaming.

```csharp
AgentSession session = await agent.CreateSessionAsync();
Console.WriteLine(await agent.RunAsync("Tell me a joke.", session));
Console.WriteLine(await agent.RunAsync("Now make it funnier.", session));
```

For more information on how to run and interact with agents, see the [Agent getting started tutorials](../../get-started/your-first-agent.md).

::: zone-end
::: zone pivot="programming-language-python"

## Foundry in Python

In Python, all Foundry-specific clients now live under `agent_framework.foundry`.

- `agent-framework-foundry` provides the cloud Foundry connectors: `FoundryChatClient`, `FoundryAgent`, `FoundryEmbeddingClient`, and `FoundryMemoryProvider`.
- `agent-framework-foundry-local` provides `FoundryLocalClient` for local model execution.

> [!IMPORTANT]
> This page covers the current Python clients for Microsoft Foundry project endpoints, models endpoints, and the Foundry Agent Service. If you have a standalone Azure OpenAI resource endpoint (`https://<your-resource>.openai.azure.com`), use the Python guidance on the [OpenAI provider page](./openai.md). If you want to run supported models locally, see the [Foundry Local provider page](./foundry-local.md).

## Foundry chat and agent patterns in Python

| Scenario | Python shape | Use when |
|---|---|---|
| Plain inference with the Foundry Responses endpoint | `Agent(client=FoundryChatClient(...))` | Your app owns the agent definition, tools, and conversation loop, and you want a model deployed in a Foundry project. |
| Service-managed agents in the Foundry Agent Service | `FoundryAgent(...)` | You want to connect to a PromptAgent or HostedAgent that is created and configured in the Foundry portal or through the service APIs. |

## Installation

```bash
pip install agent-framework-foundry
pip install azure-identity
```

The same `agent-framework-foundry` package also includes `FoundryEmbeddingClient` for Foundry models-endpoint embeddings.

## Configuration

### `FoundryChatClient`

```bash
FOUNDRY_PROJECT_ENDPOINT="https://<your-project>.services.ai.azure.com"
FOUNDRY_MODEL="gpt-4o-mini"
```

### `FoundryAgent`

```bash
FOUNDRY_PROJECT_ENDPOINT="https://<your-project>.services.ai.azure.com"
FOUNDRY_AGENT_NAME="my-agent"
FOUNDRY_AGENT_VERSION="1.0"
```

Use `FOUNDRY_AGENT_VERSION` for Prompt Agents. Hosted agents can omit it.

### `FoundryEmbeddingClient`

```bash
FOUNDRY_MODELS_ENDPOINT="https://<apim-instance>.azure-api.net/<foundry-instance>/models"
FOUNDRY_MODELS_API_KEY="<api-key>"
FOUNDRY_EMBEDDING_MODEL="text-embedding-3-small"
FOUNDRY_IMAGE_EMBEDDING_MODEL="Cohere-embed-v3-english"  # optional
```

`FoundryChatClient` and `FoundryAgent` use the project endpoint. `FoundryEmbeddingClient` uses the separate models endpoint.

### Choose the right Python client

| Scenario | Preferred client | Notes |
|---|---|---|
| Azure OpenAI resource | `OpenAIChatCompletionClient` / `OpenAIChatClient` | Use the [OpenAI provider page](./openai.md). |
| Microsoft Foundry project inference | `Agent(client=FoundryChatClient(...))` | Uses the Foundry Responses endpoint. |
| Microsoft Foundry service-managed agent | `FoundryAgent` | Recommended for Prompt Agents and HostedAgents. |
| Microsoft Foundry models-endpoint embeddings | `FoundryEmbeddingClient` | Uses `FOUNDRY_MODELS_ENDPOINT` plus `FOUNDRY_EMBEDDING_MODEL` / `FOUNDRY_IMAGE_EMBEDDING_MODEL`. |
| Foundry Local runtime | `Agent(client=FoundryLocalClient(...))` | See [Foundry Local](./foundry-local.md). |

## Create an agent with `FoundryChatClient`

`FoundryChatClient` connects to a deployed model in a Foundry project and uses the Responses endpoint. Pair it with a standard `Agent` when your app should own instructions, tools, and session handling.

```python
from agent_framework import Agent
from agent_framework.foundry import FoundryChatClient
from azure.identity import AzureCliCredential

agent = Agent(
    client=FoundryChatClient(
        project_endpoint="https://your-project.services.ai.azure.com",
        model="gpt-4o-mini",
        credential=AzureCliCredential(),
    ),
    name="FoundryWeatherAgent",
    instructions="You are a helpful assistant.",
)
```

`FoundryChatClient` is the Foundry-first Python path for direct inference and supports tools, structured output, and streaming.

## Create embeddings with `FoundryEmbeddingClient`

Use `FoundryEmbeddingClient` when you want text or image embeddings from a Foundry models endpoint.

```python
from agent_framework.foundry import FoundryEmbeddingClient

async with FoundryEmbeddingClient() as client:
    result = await client.get_embeddings(["hello from Agent Framework"])
    print(result[0].dimensions)
```

## Connect to a service-managed agent with `FoundryAgent`

Use `FoundryAgent` when the agent definition lives in Foundry. This is the recommended Python API for Prompt Agents and HostedAgents.

```python
from agent_framework.foundry import FoundryAgent
from azure.identity import AzureCliCredential

agent = FoundryAgent(
    project_endpoint="https://your-project.services.ai.azure.com",
    agent_name="my-prompt-agent",
    agent_version="1.0",
    credential=AzureCliCredential(),
)
```

For a HostedAgent, omit `agent_version` and use the hosted agent name instead.

> [!WARNING]
> The older Python `AzureAIClient`, `AzureAIProjectAgentProvider`, `AzureAIAgentClient`, `AzureAIAgentsProvider`, and Azure AI embedding compatibility surfaces were removed from the current `agent_framework.azure` namespace. For current Python code, use `FoundryChatClient` when your app owns instructions and tools, `FoundryAgent` when the agent definition lives in Foundry, and `FoundryEmbeddingClient` for Foundry models-endpoint embeddings.

## Using the agent

Both `FoundryChatClient` and `FoundryAgent` integrate with the standard Python `Agent` experience, including tool calling, sessions, and streaming responses. For local runtimes, use the separate [Foundry Local provider page](./foundry-local.md).
::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Foundry Local](./foundry-local.md)
