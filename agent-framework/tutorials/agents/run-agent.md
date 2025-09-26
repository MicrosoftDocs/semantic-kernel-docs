---
title: Create and run an agent with Agent Framework
description: Learn how to create and run an AI agent using Agent Framework
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/15/2025
ms.service: semantic-kernel
---

# Create and run an agent with Agent Framework

::: zone pivot="programming-language-csharp"

This tutorial shows you how to create and run an agent with the Agent Framework, based on the Azure OpenAI Chat Completion service.

> [!IMPORTANT]
> The agent framework supports many different types of agents. This tutorial uses an agent based on a Chat Completion service, but all other agent types are run in the same way. See the [Agent Framework user guide](../../user-guide/overview.md) for more information on other agent types and how to construct them.

## Prerequisites

Before you begin, ensure you have the following prerequisites:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Azure OpenAI service endpoint and deployment configured](/azure/ai-foundry/openai/how-to/create-resource)
- [Azure CLI installed](/cli/azure/install-azure-cli) and [authenticated (for Azure credential authentication)](/cli/azure/authenticate-azure-cli)
- [User has the `Cognitive Services OpenAI User` or `Cognitive Services OpenAI Contributor` roles, depending on need, for the Azure OpenAI resource.](/azure/ai-foundry/openai/how-to/role-based-access-control)

> [!IMPORTANT]
> For this tutorial we are using Azure OpenAI for the Chat Completion service, but you can use any inference service that is compatible with [Microsoft.Extensions.AI.IChatClient](/dotnet/api/microsoft.extensions.ai.ichatclient).

## Installing Nuget packages

To use the Microsoft Agent Framework with Azure OpenAI, you need to install the following NuGet packages:

```powershell
dotnet add package Azure.Identity
dotnet add package Azure.AI.OpenAI
dotnet add package Microsoft.Extensions.AI.OpenAI
dotnet add package Microsoft.Agents.OpenAI
```

## Creating the agent

- First we create create a client for Azure OpenAI, by providing the Azure OpenAI endpoint and using the same login as was used when authenticating with the Azure CLI in the [Prerequisites](#prerequisites) step.
- Then we get a chat client for communicating with the chat completion service, where we also specify the specific model deployment to use. Use one of the deployments that you created in the [Prerequisites](#prerequisites) step.
- Finally we create the agent, providing instructions and a name for the agent.

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using OpenAI;

AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o-mini")
        .CreateAIAgent(instructions: "You are good at telling jokes.", name: "Joker");
```

## Running the agent

To run the agent, call the `RunAsync` method on the agent instance, providing the user input.
The agent will return a response object, and calling `.ToString()` or `.Text` on this response object, provides the text result from the agent.

```csharp
Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));
```

## Running the agent with streaming

To run the agent with streaming, call the `RunStreamingAsync` method on the agent instance, providing the user input.
The agent will stream a list of update objects, and calling `.ToString()` or `.Text` on each update object provides the part of the text result contained in that update.

```csharp
await foreach (var update in agent.RunStreamingAsync("Tell me a joke about a pirate."))
{
    Console.WriteLine(update);
}
```

## Running the agent with a ChatMessage

Instead of a simple string, you can also provide one or more `ChatMessage` objects to the `RunAsync` and `RunStreamingAsync` methods.

```csharp
ChatMessage message = new(ChatRole.User, [
    new TextContent("Tell me a joke about this image?"),
    new UriContent("https://samplesite.org/clown.jpg", "image/jpeg")
]);

Console.WriteLine(await agent.RunAsync(message));
```

::: zone-end
::: zone pivot="programming-language-python"

This tutorial shows you how to create and run an agent with the Agent Framework, based on the Azure OpenAI Chat Completion service.

> [!IMPORTANT]
> The agent framework supports many different types of agents. This tutorial uses an agent based on a Chat Completion service, but all other agent types are run in the same way. See the [Agent Framework user guide](../../user-guide/overview.md) for more information on other agent types and how to construct them.

## Prerequisites

Before you begin, ensure you have the following prerequisites:

- [Python 3.10 or later](https://www.python.org/downloads/)
- [Azure OpenAI service endpoint and deployment configured](/azure/ai-foundry/openai/how-to/create-resource)
- [Azure CLI installed](/cli/azure/install-azure-cli) and [authenticated (for Azure credential authentication)](/cli/azure/authenticate-azure-cli)
- [User has the `Cognitive Services OpenAI User` or `Cognitive Services OpenAI Contributor` roles, depending on need, for the Azure OpenAI resource.](/azure/ai-foundry/openai/how-to/role-based-access-control)

> [!IMPORTANT]
> For this tutorial we are using Azure OpenAI for the Chat Completion service, but you can use any inference service that is compatible with the Agent Framework's chat client protocol.

## Installing Python packages

To use the Microsoft Agent Framework with Azure OpenAI, you need to install the following Python packages:

```bash
pip install agent-framework azure-identity
```

## Creating the agent

- First we create a chat client for communicating with Azure OpenAI, where we use the same login as was used when authenticating with the Azure CLI in the [Prerequisites](#prerequisites) step.
- Then we create the agent, providing instructions and a name for the agent.

```python
import asyncio
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity import AzureCliCredential

agent = AzureOpenAIChatClient(credential=AzureCliCredential()).create_agent(
    instructions="You are good at telling jokes.",
    name="Joker"
)
```

## Running the agent

To run the agent, call the `run` method on the agent instance, providing the user input.
The agent will return a response object, and accessing the `.text` property provides the text result from the agent.

```python
async def main():
    result = await agent.run("Tell me a joke about a pirate.")
    print(result.text)

asyncio.run(main())
```

## Running the agent with streaming

To run the agent with streaming, call the `run_stream` method on the agent instance, providing the user input.
The agent will stream a list of update objects, and accessing the `.text` property on each update object provides the part of the text result contained in that update.

```python
async def main():
    async for update in agent.run_stream("Tell me a joke about a pirate."):
        if update.text:
            print(update.text, end="", flush=True)
    print()  # New line after streaming is complete

asyncio.run(main())
```

## Running the agent with a ChatMessage

Instead of a simple string, you can also provide one or more `ChatMessage` objects to the `run` and `run_stream` methods.

```python
from agent_framework import ChatMessage, TextContent, UriContent, Role

message = ChatMessage(
    role=Role.USER, 
    contents=[
        TextContent(text="Tell me a joke about this image?"),
        UriContent(uri="https://samplesite.org/clown.jpg", media_type="image/jpeg")
    ]
)

async def main():
    result = await agent.run(message)
    print(result.text)

asyncio.run(main())
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Using images with an agent](./images.md)
