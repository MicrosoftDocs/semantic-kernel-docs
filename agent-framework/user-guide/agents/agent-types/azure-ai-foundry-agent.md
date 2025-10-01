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
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.AzureAI --prerelease
```

## Creating Azure AI Foundry Agents

As a first step you need to create a client to connect to the Azure AI Foundry Agents service.

```csharp
using System;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Agents.AI;

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

See the [Agent getting started tutorials](../../../tutorials/overview.md) for more information on how to run and interact with agents.

::: zone-end
::: zone pivot="programming-language-python"

## Configuration

### Environment Variables

Before using Azure AI Foundry Agents, you need to set up these environment variables:

```bash
export AZURE_AI_PROJECT_ENDPOINT="https://<your-project>.services.ai.azure.com/api/projects/<project-id>"
export AZURE_AI_MODEL_DEPLOYMENT_NAME="gpt-4o-mini"
```

Alternatively, you can provide these values directly in your code.

### Installation

Add the Agent Framework Azure AI package to your project:

```bash
pip install agent-framework[azure-ai]
```

## Getting Started

### Authentication

Azure AI Foundry Agents use Azure credentials for authentication. The simplest approach is to use `AzureCliCredential` after running `az login`:

```python
from azure.identity.aio import AzureCliCredential

async with AzureCliCredential() as credential:
    # Use credential with Azure AI agent client
```

## Creating Azure AI Foundry Agents

### Basic Agent Creation

The simplest way to create an agent is using the `AzureAIAgentClient` with environment variables:

```python
import asyncio
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        AzureAIAgentClient(async_credential=credential).create_agent(
            name="HelperAgent",
            instructions="You are a helpful assistant."
        ) as agent,
    ):
        result = await agent.run("Hello!")
        print(result.text)

asyncio.run(main())
```

### Explicit Configuration

You can also provide configuration explicitly instead of using environment variables:

```python
import asyncio
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        AzureAIAgentClient(
            project_endpoint="https://<your-project>.services.ai.azure.com/api/projects/<project-id>",
            model_deployment_name="gpt-4o-mini",
            async_credential=credential,
            agent_name="HelperAgent"
        ).create_agent(
            instructions="You are a helpful assistant."
        ) as agent,
    ):
        result = await agent.run("Hello!")
        print(result.text)

asyncio.run(main())
```

## Using Existing Azure AI Foundry Agents

### Using an Existing Agent by ID

If you have an existing agent in Azure AI Foundry, you can use it by providing its ID:

```python
import asyncio
from agent_framework import ChatAgent
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        ChatAgent(
            chat_client=AzureAIAgentClient(
                async_credential=credential,
                agent_id="<existing-agent-id>"
            ),
            instructions="You are a helpful assistant."
        ) as agent,
    ):
        result = await agent.run("Hello!")
        print(result.text)

asyncio.run(main())
```

### Creating and Managing Persistent Agents

For more control over agent lifecycle, you can create persistent agents using the Azure AI Projects client:

```python
import asyncio
import os
from agent_framework import ChatAgent
from agent_framework.azure import AzureAIAgentClient
from azure.ai.projects.aio import AIProjectClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        AIProjectClient(
            endpoint=os.environ["AZURE_AI_PROJECT_ENDPOINT"], 
            credential=credential
        ) as project_client,
    ):
        # Create a persistent agent
        created_agent = await project_client.agents.create_agent(
            model=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
            name="PersistentAgent",
            instructions="You are a helpful assistant."
        )

        try:
            # Use the agent
            async with ChatAgent(
                chat_client=AzureAIAgentClient(
                    project_client=project_client,
                    agent_id=created_agent.id
                ),
                instructions="You are a helpful assistant."
            ) as agent:
                result = await agent.run("Hello!")
                print(result.text)
        finally:
            # Clean up the agent
            await project_client.agents.delete_agent(created_agent.id)

asyncio.run(main())
```

## Agent Features

### Function Tools

You can provide custom function tools to Azure AI Foundry agents:

```python
import asyncio
from typing import Annotated
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential
from pydantic import Field

def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
) -> str:
    """Get the weather for a given location."""
    return f"The weather in {location} is sunny with a high of 25°C."

async def main():
    async with (
        AzureCliCredential() as credential,
        AzureAIAgentClient(async_credential=credential).create_agent(
            name="WeatherAgent",
            instructions="You are a helpful weather assistant.",
            tools=get_weather
        ) as agent,
    ):
        result = await agent.run("What's the weather like in Seattle?")
        print(result.text)

asyncio.run(main())
```

### Code Interpreter

Azure AI Foundry agents support code execution through the hosted code interpreter:

```python
import asyncio
from agent_framework import HostedCodeInterpreterTool
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        AzureAIAgentClient(async_credential=credential).create_agent(
            name="CodingAgent",
            instructions="You are a helpful assistant that can write and execute Python code.",
            tools=HostedCodeInterpreterTool()
        ) as agent,
    ):
        result = await agent.run("Calculate the factorial of 20 using Python code.")
        print(result.text)

asyncio.run(main())
```

### Streaming Responses

Get responses as they are generated using streaming:

```python
import asyncio
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        AzureAIAgentClient(async_credential=credential).create_agent(
            name="StreamingAgent",
            instructions="You are a helpful assistant."
        ) as agent,
    ):
        print("Agent: ", end="", flush=True)
        async for chunk in agent.run_stream("Tell me a short story"):
            if chunk.text:
                print(chunk.text, end="", flush=True)
        print()

asyncio.run(main())
```

## Using the Agent

The agent is a standard `BaseAgent` and supports all standard agent operations.

See the [Agent getting started tutorials](../../../tutorials/overview.md) for more information on how to run and interact with agents.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [OpenAI ChatCompletion Agents](./azure-openai-chat-completion-agent.md)
