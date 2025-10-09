---
title: Azure AI Foundry Models Agents
description: Learn how to use the Microsoft Agent Framework with Azure AI Foundry Models service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 10/07/2025
ms.service: agent-framework
---

# Azure AI Foundry Models Agents

[Azure AI Foundry supports deploying](/azure/ai-foundry/foundry-models/how-to/create-model-deployments?pivots=ai-foundry-portal) a wide range of models, including open source models.
Microsoft Agent Framework can create agents that use these models.

> [!NOTE]
> The capabilities of these models may limit the functionality of the agents. For example, many open source models do not support function calling and therefore any agent based on such models will not be able to use function tools.

::: zone pivot="programming-language-csharp"

## Getting Started

Foundry supports accessing models via an OpenAI Chat Completion compatible API, and therefore the OpenAI client libraries can be used to access Foundry models.

Add the required NuGet packages to your project.

```powershell
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

## Creating an OpenAI ChatCompletion Agent with Foundry Models

As a first step you need to create a client to connect to the OpenAI service.

Since the code is not using the default OpenAI service, the URI of the OpenAI compatible Foundry service, needs to be provided via `OpenAIClientOptions`.

```csharp
using System;
using Microsoft.Agents.AI;
using OpenAI;

var clientOptions = new OpenAIClientOptions() { Endpoint = new Uri("https://ai-foundry-<your-resource>.services.ai.azure.com/openai/v1/") };
```

There are different options for constructing the `OpenAIClient` depending on the desired authentication method. Let's look at two common options.

The first option uses an API key.

```csharp
OpenAIClient client = new OpenAIClient(new ApiKeyCredential("<your_api_key>"), clientOptions);
```

The second option uses token based authentication, and here it is using the Azure CLI credential to get a token.

```csharp
OpenAIClient client = new OpenAIClient(new BearerTokenPolicy(new AzureCliCredential(), "https://ai.azure.com/.default"), clientOptions);
```

A client for chat completions can then be created using the model deployment name.

```csharp
var chatCompletionClient = client.GetChatClient("gpt-4o-mini");
```

Finally, the agent can be created using the `CreateAIAgent` extension method on the `ChatCompletionClient`.

```csharp
AIAgent agent = chatCompletionClient.CreateAIAgent(
    instructions: "You are good at telling jokes.",
    name: "Joker");
```

## Using the Agent

The agent is a standard `AIAgent` and supports all standard `AIAgent` operations.

For more information on how to run and interact with agents, see the [Agent getting started tutorials](../../../tutorials/overview.md)

::: zone-end
::: zone pivot="programming-language-python"

More docs coming soon.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Azure OpenAI ChatCompletion Agents](./azure-openai-chat-completion-agent.md)
