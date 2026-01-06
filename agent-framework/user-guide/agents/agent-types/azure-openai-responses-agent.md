---
title: Azure OpenAI Responses Agents
description: Learn how to use Microsoft Agent Framework with Azure OpenAI Responses service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/24/2025
ms.service: agent-framework
---

# Azure OpenAI Responses Agents

Microsoft Agent Framework supports creating agents that use the [Azure OpenAI Responses](/azure/ai-foundry/openai/how-to/responses) service.

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```dotnetcli
dotnet add package Azure.AI.OpenAI --prerelease
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

## Create an Azure OpenAI Responses Agent

As a first step you need to create a client to connect to the Azure OpenAI service.

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI;

AzureOpenAIClient client = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com/"),
    new AzureCliCredential());
```

Azure OpenAI supports multiple services that all provide model calling capabilities.
Pick the Responses service to create a Responses based agent.

```csharp
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
var responseClient = client.GetOpenAIResponseClient("gpt-4o-mini");
#pragma warning restore OPENAI001
```

Finally, create the agent using the `CreateAIAgent` extension method on the `ResponseClient`.

```csharp
AIAgent agent = responseClient.CreateAIAgent(
    instructions: "You are good at telling jokes.",
    name: "Joker");

// Invoke the agent and output the text result.
Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));
```

## Using the Agent

The agent is a standard `AIAgent` and supports all standard `AIAgent` operations.

For more information on how to run and interact with agents, see the [Agent getting started tutorials](../../../tutorials/overview.md).

::: zone-end
::: zone pivot="programming-language-python"

## Configuration

### Environment Variables

Before using Azure OpenAI Responses agents, you need to set up these environment variables:

```bash
export AZURE_OPENAI_ENDPOINT="https://<myresource>.openai.azure.com"
export AZURE_OPENAI_RESPONSES_DEPLOYMENT_NAME="gpt-4o-mini"
```

Optionally, you can also set:

```bash
export AZURE_OPENAI_API_VERSION="preview"  # Required for Responses API
export AZURE_OPENAI_API_KEY="<your-api-key>"  # If not using Azure CLI authentication
```

### Installation

Add the Agent Framework package to your project:

```bash
pip install agent-framework --pre
```

## Getting Started

### Authentication

Azure OpenAI Responses agents use Azure credentials for authentication. The simplest approach is to use `AzureCliCredential` after running `az login`:

```python
from azure.identity import AzureCliCredential

credential = AzureCliCredential()
```

## Create an Azure OpenAI Responses Agent

### Basic Agent Creation

The simplest way to create an agent is using the `AzureOpenAIResponsesClient` with environment variables:

```python
import asyncio
from agent_framework.azure import AzureOpenAIResponsesClient
from azure.identity import AzureCliCredential

async def main():
    agent = AzureOpenAIResponsesClient(credential=AzureCliCredential()).create_agent(
        instructions="You are good at telling jokes.",
        name="Joker"
    )

    result = await agent.run("Tell me a joke about a pirate.")
    print(result.text)

asyncio.run(main())
```

### Explicit Configuration

You can also provide configuration explicitly instead of using environment variables:

```python
import asyncio
from agent_framework.azure import AzureOpenAIResponsesClient
from azure.identity import AzureCliCredential

async def main():
    agent = AzureOpenAIResponsesClient(
        endpoint="https://<myresource>.openai.azure.com",
        deployment_name="gpt-4o-mini",
        api_version="preview",
        credential=AzureCliCredential()
    ).create_agent(
        instructions="You are good at telling jokes.",
        name="Joker"
    )

    result = await agent.run("Tell me a joke about a pirate.")
    print(result.text)

asyncio.run(main())
```

## Agent Features

### Function Tools

You can provide custom function tools to Azure OpenAI Responses agents:

```python
import asyncio
from typing import Annotated
from agent_framework.azure import AzureOpenAIResponsesClient
from azure.identity import AzureCliCredential
from pydantic import Field

def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
) -> str:
    """Get the weather for a given location."""
    return f"The weather in {location} is sunny with a high of 25°C."

async def main():
    agent = AzureOpenAIResponsesClient(credential=AzureCliCredential()).create_agent(
        instructions="You are a helpful weather assistant.",
        tools=get_weather
    )

    result = await agent.run("What's the weather like in Seattle?")
    print(result.text)

asyncio.run(main())
```

### Code Interpreter

Azure OpenAI Responses agents support code execution through the hosted code interpreter:

```python
import asyncio
from agent_framework import ChatAgent, HostedCodeInterpreterTool
from agent_framework.azure import AzureOpenAIResponsesClient
from azure.identity import AzureCliCredential

async def main():
    async with ChatAgent(
        chat_client=AzureOpenAIResponsesClient(credential=AzureCliCredential()),
        instructions="You are a helpful assistant that can write and execute Python code.",
        tools=HostedCodeInterpreterTool()
    ) as agent:
        result = await agent.run("Calculate the factorial of 20 using Python code.")
        print(result.text)

asyncio.run(main())
```

### Streaming Responses

Get responses as they are generated using streaming:

```python
import asyncio
from agent_framework.azure import AzureOpenAIResponsesClient
from azure.identity import AzureCliCredential

async def main():
    agent = AzureOpenAIResponsesClient(credential=AzureCliCredential()).create_agent(
        instructions="You are a helpful assistant."
    )

    print("Agent: ", end="", flush=True)
    async for chunk in agent.run_stream("Tell me a short story about a robot"):
        if chunk.text:
            print(chunk.text, end="", flush=True)
    print()

asyncio.run(main())
```

## Using the Agent

The agent is a standard `BaseAgent` and supports all standard agent operations.

For more information on how to run and interact with agents, see the [Agent getting started tutorials](../../../tutorials/overview.md).

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [OpenAI Chat Completion Agents](./openai-chat-completion-agent.md)
