---
title: Azure OpenAI ChatCompletion Agents
description: Learn how to use the Microsoft Agent Framework with Azure OpenAI ChatCompletion service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# Azure OpenAI ChatCompletion Agents

The Microsoft Agent Framework supports creating agents that use the [Azure OpenAI ChatCompletion](/azure/ai-foundry/openai/how-to/chatgpt) service.

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```powershell
dotnet add package Microsoft.Extensions.AI.OpenAI --prerelease
dotnet add package Azure.AI.OpenAI
dotnet add package Azure.Identity
```

## Creating an Azure OpenAI ChatCompletion Agent

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
We need to pick the ChatCompletion service to create a ChatCompletion based agent.

```csharp
var chatCompletionClient = client.GetChatClient("gpt-4o-mini");
```

Finally, create the agent using the `CreateAIAgent` extension method on the `ChatCompletionClient`.

```csharp
AIAgent agent = chatCompletionClient.CreateAIAgent(
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
