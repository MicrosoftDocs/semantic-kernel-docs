---
title: Azure AI Foundry Agents
description: Learn how to use the Microsoft Agent Framework with Azure AI Foundry Agents service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/15/2025
ms.service: semantic-kernel
---

# Azure AI Foundry Agents

The Microsoft Agent Framework supports creating agents that use the [Azure AI Foundry Agents](/azure/ai-foundry/agents/overview) service.

::: zone pivot="programming-language-csharp"

## Getting Started

Add the Agents Azure AI NuGet package to your project.

```powershell
dotnet add package Microsoft.Extensions.AI.Agents.AzureAI --prerelease
```

## Creating Azure AI Foundry Agents

As a first step you need to create a client to connect to the Azure AI Foundry Agents service.

```csharp
using System;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Extensions.AI.Agents;

var persistentAgentsClient = new PersistentAgentsClient(
    "https://<myresource>.services.ai.azure.com/api/projects/<myproject>",
    new AzureCliCredential());
```

To use the Azure AI Foundry Agents service, you need create an agent resource in the service.
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
```

### Using the Agent Framework helpers

You can also create and return an `AIAgent` in one step:

```csharp
AIAgent agent2 = await persistentAgentsClient.CreateAIAgentAsync(
    model: "gpt-4o-mini",
    name: "Joker",
    instructions: "You are good at telling jokes.");
```

## Reusing Azure AI Foundry Agents

You can reuse existing Azure AI Foundry Agents by retrieving them using their IDs.

```csharp
AIAgent agent3 = await persistentAgentsClient.GetAIAgentAsync("<agent-id>");
```

## Using the agent

The agent is a standard `AIAgent` and supports all standard `AIAgent` operations.

See the [Agent getting started tutorials](../../../tutorials/index.md#agent-getting-started-tutorials) for more information on how to run and interact with agents.

::: zone-end
::: zone pivot="programming-language-python"

Documentation coming soon.

::: zone-end
