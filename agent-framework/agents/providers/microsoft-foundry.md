---
title: Microsoft Foundry
description: Learn how to use Microsoft Agent Framework with Microsoft Foundry project endpoints and the Foundry Agent Service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 03/25/2026
ms.service: agent-framework
---

# Microsoft Foundry

Microsoft Agent Framework supports both direct model inference from Microsoft Foundry project endpoints and service-managed agents in the [Foundry Agent Service](/azure/ai-foundry/agents/overview).

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```dotnetcli
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.AzureAI.Persistent --prerelease
```

## Create Foundry Agents

As a first step you need to create a client to connect to the Agent Service.

```csharp
using System;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Agents.AI;

var persistentAgentsClient = new PersistentAgentsClient(
    "https://<myresource>.services.ai.azure.com/api/projects/<myproject>",
    new DefaultAzureCredential());
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

To use the Agent Service, you need create an agent resource in the service.
This can be done using either the Azure.AI.Agents.Persistent SDK or using Microsoft Agent Framework helpers.

### Using the Persistent SDK

Create a persistent agent and retrieve it as an `AIAgent` using the `PersistentAgentsClient`.

```csharp
// Create a persistent agent
var agentMetadata = await persistentAgentsClient.Administration.CreateAgentAsync(
    model: "gpt-4o-mini",
    name: "Joker",
    instructions: "You are good at telling jokes.");

// Retrieve the agent that was just created as an AIAgent using its ID
AIAgent agent1 = await persistentAgentsClient.GetAIAgentAsync(agentMetadata.Value.Id);

// Invoke the agent and output the text result.
Console.WriteLine(await agent1.RunAsync("Tell me a joke about a pirate."));
```

### Using Agent Framework helpers

You can also create and return an `AIAgent` in one step:

```csharp
AIAgent agent2 = await persistentAgentsClient.CreateAIAgentAsync(
    model: "gpt-4o-mini",
    name: "Joker",
    instructions: "You are good at telling jokes.");
```

## Reusing Foundry Agents

You can reuse existing Foundry Agents by retrieving them using their IDs.

```csharp
AIAgent agent3 = await persistentAgentsClient.GetAIAgentAsync("<agent-id>");
```

> [!TIP]
> See the [.NET samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples) for complete runnable examples.

## Using the agent

The agent is a standard `AIAgent` and supports all standard `AIAgent` operations.

For more information on how to run and interact with agents, see the [Agent getting started tutorials](../../get-started/your-first-agent.md).

::: zone-end
::: zone pivot="programming-language-python"

## Foundry in Python

In Python, all Foundry-specific clients now live under `agent_framework.foundry`.

- `agent-framework-foundry` provides the cloud Foundry connectors: `FoundryChatClient`, `FoundryAgent`, and `FoundryMemoryProvider`.
- `agent-framework-foundry-local` provides `FoundryLocalClient` for local model execution.

> [!IMPORTANT]
> This page is for Microsoft Foundry project endpoints and the Foundry Agent Service. If you have a standalone Azure OpenAI resource endpoint (`https://<your-resource>.openai.azure.com`), use the Python guidance on the [OpenAI provider page](./openai.md). If you want to run supported models locally, see the [Foundry Local provider page](./foundry-local.md).

## Two ways to use Foundry in Python

| Scenario | Python shape | Use when |
|---|---|---|
| Plain inference with the Foundry Responses endpoint | `Agent(client=FoundryChatClient(...))` | Your app owns the agent definition, tools, and conversation loop, and you want a model deployed in a Foundry project. |
| Service-managed agents in the Foundry Agent Service | `FoundryAgent(...)` | You want to connect to a PromptAgent or HostedAgent that is created and configured in the Foundry portal or through the service APIs. |

## Installation

```bash
pip install agent-framework-foundry --pre
pip install azure-identity
```

If you still rely on the older Azure AI compatibility classes, install `agent-framework-azure-ai --pre` separately.

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

### Choose the right Python client

| Scenario | Preferred client | Notes |
|---|---|---|
| Azure OpenAI resource | `OpenAIChatCompletionClient` / `OpenAIChatClient` | Use the [OpenAI provider page](./openai.md). |
| Microsoft Foundry project inference | `Agent(client=FoundryChatClient(...))` | Uses the Foundry Responses endpoint. |
| Microsoft Foundry service-managed agent | `FoundryAgent` | Recommended for Prompt Agents and HostedAgents. |
| Foundry Local runtime | `Agent(client=FoundryLocalClient(...))` | See [Foundry Local](./foundry-local.md). |
| Legacy/lower-level Azure AI agent APIs | `AzureAIClient`, `AzureAIProjectAgentProvider`, `AzureAIAgentClient` | Available under `agent_framework.azure` as deprecated or compatibility paths. |

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
> `AzureAIClient`, `AzureAIProjectAgentProvider`, and `AzureAIAgentsProvider` are older Azure AI compatibility or lower-level paths. `AzureAIAgentClient` targets the v1 Agent Service compatibility surface and will be removed before GA. For new Python code, prefer `FoundryAgent`.

## Using the agent

Both `FoundryChatClient` and `FoundryAgent` integrate with the standard Python `Agent` experience, including tool calling, sessions, and streaming responses. For local runtimes, use the separate [Foundry Local provider page](./foundry-local.md).
::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Foundry Local](./foundry-local.md)
