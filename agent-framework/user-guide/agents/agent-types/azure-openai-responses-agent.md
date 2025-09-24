---
title: Azure OpenAI Responses Agents
description: Learn how to use the Microsoft Agent Framework with Azure OpenAI Responses service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# Azure OpenAI Responses Agents

The Microsoft Agent Framework supports creating agents that use the [Azure OpenAI responses](/azure/ai-foundry/openai/how-to/responses) service.

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```powershell
dotnet add package Microsoft.Extensions.AI.OpenAI --prerelease
dotnet add package Azure.AI.OpenAI
dotnet add package Azure.Identity
```

## Creating an Azure OpenAI Responses Agent

As a first step you need to create a client to connect to the Azure OpenAI service.

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI.Agents;
using OpenAI;

AzureOpenAIClient client = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential());
```

Azure OpenAI supports multiple services that all provide model calling capabilities.
We need to pick the Responses service to create a Responses based agent.

```csharp
var responseClient = client.GetOpenAIResponseClient("gpt-4o-mini");
```

Finally, create the agent using the `CreateAIAgent` extension method on the `ResponseClient`.

```csharp
AIAgent agent = responseClient.CreateAIAgent(
    instructions: "You are good at telling jokes.",
    name: "Joker");
```

::: zone-end
::: zone pivot="programming-language-python"

Documentation coming soon.

::: zone-end

## Using the Agent

The agent is a standard `AIAgent` and supports all standard `AIAgent` operations.

See the [Agent getting started tutorials](../../../tutorials/index.md#agent-getting-started-tutorials) for more information on how to run and interact with agents.
