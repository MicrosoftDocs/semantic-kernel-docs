---
title: Quick Start
description: Quick start guide for the Agent Framework.
ms.service: semantic-kernel
ms.topic: tutorial
ms.date: 09/04/2025
ms.reviewer: ssalgado
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.author: taochen
---

# Microsoft Agent Framework Quick Start

This guide will help you get up and running quickly with a basic agent using the Agent Framework and Azure OpenAI.

::: zone pivot="programming-language-csharp"

## Prerequisites

Before you begin, ensure you have the following:

- [.NET 8.0 SDK or later](https://dotnet.microsoft.com/download)
- [Azure OpenAI resource](/azure/ai-foundry/openai/how-to/create-resource) with a deployed model (e.g., `gpt-4o-mini`)
- [Azure CLI installed](/cli/azure/install-azure-cli) and [authenticated](/cli/azure/authenticate-azure-cli) (`az login`)
- [User has the `Cognitive Services OpenAI User` or `Cognitive Services OpenAI Contributor` roles, depending on need, for the Azure OpenAI resource.](/azure/ai-foundry/openai/how-to/role-based-access-control)

**Note**: This demo uses Azure CLI credentials for authentication. Make sure you're logged in with `az login` and have access to the Azure OpenAI resource. For more information, see the [Azure CLI documentation](/cli/azure/authenticate-azure-cli-interactively). It is also possible to replace the `AzureCliCredential` with an `ApiKeyCredential` if you
have an api key and do not wish to use role based authentication, in which case `az login` is not required.

## Installing nuget packages

Install the following nuget packages into your application.

```powershell
dotnet add package Azure.AI.OpenAI
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.OpenAI
```

## Running a Basic Agent Sample

This sample demonstrates how to create and use a simple AI agent with Azure OpenAI Chat Completion as the backend. It will create a basic agent using `AzureOpenAIClient` with `gpt-4o-mini` and custom instructions.

Make sure to replace `https://your-resource.openai.azure.com/` with the endpoint of your Azure OpenAI resource.

### Sample Code

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI.Agents;
using OpenAI;

AIAgent agent = new AzureOpenAIClient(
  new Uri("https://your-resource.openai.azure.com/"),
  new AzureCliCredential())
    .GetChatClient("gpt-4o-mini")
    .CreateAIAgent(instructions: "You are good at telling jokes.");

Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));
```

::: zone-end

::: zone pivot="programming-language-python"

## Prerequisites

Before you begin, ensure you have the following:

- [Python 3.10 or later](https://www.python.org/downloads/)
- An [Azure AI](/azure/ai-foundry/) project with a deployed model (e.g., `gpt-4o-mini`)
- [Azure CLI](/cli/azure/install-azure-cli) installed and authenticated (`az login`)

**Note**: This demo uses Azure CLI credentials for authentication. Make sure you're logged in with `az login` and have access to the Azure AI project. For more information, see the [Azure CLI documentation](/cli/azure/authenticate-azure-cli-interactively).

## Running a Basic Agent Sample

This sample demonstrates how to create and use a simple AI agent with Azure AI as the backend. It will create a basic agent using `ChatAgent` with `AzureAIAgentClient` and custom instructions.

Make sure to set the following environment variables:
- `AZURE_AI_PROJECT_ENDPOINT`: Your Azure AI project endpoint
- `AZURE_AI_MODEL_DEPLOYMENT_NAME`: The name of your model deployment


### Sample Code

```python
import asyncio
from agent_framework import ChatAgent
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        ChatAgent(
            chat_client=AzureAIAgentClient(async_credential=credential),
            instructions="You are good at telling jokes."
        ) as agent,
    ):
        result = await agent.run("Tell me a joke about a pirate.")
        print(result.text)

if __name__ == "__main__":
    asyncio.run(main())
```

## More Examples

For more detailed examples and advanced scenarios, see the [Azure AI Agent Examples](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/agents/azure_ai/README.md).


::: zone-end
