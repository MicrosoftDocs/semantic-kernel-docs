---
title: Azure AI Foundry Models ChatCompletion Agents
description: Learn how to use the Microsoft Agent Framework with Azure AI Foundry Models service via OpenAI ChatCompletion API.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 10/07/2025
ms.service: agent-framework
---

# Azure AI Foundry Models Agents

The Microsoft Agent Framework supports creating agents using models deployed with Azure AI Foundry Models via an OpenAI Chat Completion compatible API, and therefore the OpenAI client libraries can be used to access Foundry models.

[Azure AI Foundry supports deploying](/azure/ai-foundry/foundry-models/how-to/create-model-deployments?pivots=ai-foundry-portal) a wide range of models, including open source models.

> [!NOTE]
> The capabilities of these models may limit the functionality of the agents. For example, many open source models do not support function calling and therefore any agent based on such models will not be able to use function tools.

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```powershell
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

## Creating an OpenAI ChatCompletion Agent with Foundry Models

As a first step you need to create a client to connect to the OpenAI service.

Since the code is not using the default OpenAI service, the URI of the OpenAI compatible Foundry service, needs to be provided via `OpenAIClientOptions`.

```csharp
using System;
using System.ClientModel.Primitives;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI;

var clientOptions = new OpenAIClientOptions() { Endpoint = new Uri("https://<myresource>.services.ai.azure.com/openai/v1/") };

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
OpenAIClient client = new OpenAIClient(new BearerTokenPolicy(new AzureCliCredential(), "https://ai.azure.com/.default"), clientOptions);
#pragma warning restore OPENAI001
// You can optionally authenticate with an API key
// OpenAIClient client = new OpenAIClient(new ApiKeyCredential("<your_api_key>"), clientOptions);
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

// Invoke the agent and output the text result.
Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));
```

## Using the Agent

The agent is a standard `AIAgent` and supports all standard `AIAgent` operations.

See the [Agent getting started tutorials](../../../tutorials/overview.md) for more information on how to run and interact with agents.

::: zone-end
::: zone pivot="programming-language-python"

More docs coming soon.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Azure OpenAI ChatCompletion Agents](./azure-ai-foundry-models-responses-agent.md)
