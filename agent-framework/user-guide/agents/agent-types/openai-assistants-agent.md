---
title: OpenAI Assistants Agents
description: Learn how to use the Microsoft Agent Framework with OpenAI Assistants service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# OpenAI Assistants Agents

The Microsoft Agent Framework supports creating agents that use the [OpenAI Assistants](https://platform.openai.com/docs/api-reference/assistants/createAssistant) service.

> [!WARNING]
> The OpenAI Assistants API is deprecated and will be shut down. For more information see the [OpenAI documentation](https://platform.openai.com/docs/assistants/migration).

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```powershell
dotnet add package Microsoft.Extensions.AI.OpenAI --prerelease
```

## Creating an OpenAI Assistants Agent

As a first step you need to create a client to connect to the OpenAI service.

```csharp
using System;
using Microsoft.Extensions.AI.Agents;
using OpenAI;

OpenAIClient client = new OpenAIClient("<your_api_key>");
```

OpenAI supports multiple services that all provide model calling capabilities.
We need to pick the Assistants service to create an Assistants based agent.

```csharp
var assistantClient = client.GetAssistantClient();
```

To use the OpenAI Assistants service, you need create an assistant resource in the service.
This can be done using either the OpenAI SDK or using Microsoft Agent Framework helpers.

### Using the OpenAI SDK

Create an assistant and retrieve it as an `AIAgent` using the client.

```csharp
// Create a server-side assistant
var createResult = await assistantClient.CreateAssistantAsync(
    "gpt-4o-mini",
    new() { Name = "Joker", Instructions = "You are good at telling jokes." });

// Retrieve the assistant as an AIAgent
AIAgent agent1 = await assistantClient.GetAIAgentAsync(createResult.Value.Id);
```

### Using the Agent Framework helpers

You can also create and return an `AIAgent` in one step:

```csharp
AIAgent agent2 = await assistantClient.CreateAIAgentAsync(
    model: "gpt-4o-mini",
    name: "Joker",
    instructions: "You are good at telling jokes.");
```

## Reusing OpenAI Assistants

You can reuse existing OpenAI Assistants by retrieving them using their IDs.

```csharp
AIAgent agent3 = await assistantClient.GetAIAgentAsync("<agent-id>");
```

::: zone-end
::: zone pivot="programming-language-python"

Documentation coming soon.

::: zone-end

## Using the Agent

The agent is a standard `AIAgent` and supports all standard agent operations.

See the [Agent getting started tutorials](../../../tutorials/overview.md) for more information on how to run and interact with agents.
