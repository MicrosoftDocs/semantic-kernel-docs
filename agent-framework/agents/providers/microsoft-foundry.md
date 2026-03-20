---
title: Microsoft Foundry Agents
description: Learn how to use Microsoft Agent Framework with Foundry Agent Service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 02/17/2026
ms.service: agent-framework
---

# Microsoft Foundry Agents

Microsoft Agent Framework supports creating agents that use the [Foundry Agent Service](/azure/ai-foundry/agents/overview). You can create persistent service-based agent instances with service-managed chat history.

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```dotnetcli
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.AzureAI.Persistent --prerelease
```

## Create Foundry Agents

As a first step you need to create a client to connect to the Agent Service.

```csharp
using System;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Agents.AI;

var persistentAgentsClient = new PersistentAgentsClient(
    "https://<myresource>.services.ai.azure.com/api/projects/<myproject>",
    new DefaultAzureCredential());
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

To use the Agent Service, you need create an agent resource in the service.
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

// Invoke the agent and output the text result.
Console.WriteLine(await agent1.RunAsync("Tell me a joke about a pirate."));
```

### Using Agent Framework helpers

You can also create and return an `AIAgent` in one step:

```csharp
AIAgent agent2 = await persistentAgentsClient.CreateAIAgentAsync(
    model: "gpt-4o-mini",
    name: "Joker",
    instructions: "You are good at telling jokes.");
```

## Reusing Foundry Agents

You can reuse existing Foundry Agents by retrieving them using their IDs.

```csharp
AIAgent agent3 = await persistentAgentsClient.GetAIAgentAsync("<agent-id>");
```

> [!TIP]
> See the [.NET samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples) for complete runnable examples.

## Using the agent

The agent is a standard `AIAgent` and supports all standard `AIAgent` operations.

For more information on how to run and interact with agents, see the [Agent getting started tutorials](../../get-started/your-first-agent.md).

::: zone-end
::: zone pivot="programming-language-python"

## Configuration

### Environment Variables

> [!IMPORTANT]
> `AzureAIAgentClient` (Foundry Agent Service v1) and `AzureAIClient` (Foundry Agent Service v2) both require an **Azure AI Foundry project** endpoint (format: `https://<your-project>.services.ai.azure.com/api/projects/<project-id>`), **not** an Azure OpenAI resource endpoint. You must have an [Azure AI Foundry project](/azure/ai-foundry/what-is-ai-foundry) to use this provider. If you have a standalone Azure OpenAI resource instead, see the [Azure OpenAI provider page](./azure-openai.md).

Before using Foundry Agents, you need to set up these environment variables:

```bash
export AZURE_AI_PROJECT_ENDPOINT="https://<your-project>.services.ai.azure.com/api/projects/<project-id>"
export AZURE_AI_MODEL_DEPLOYMENT_NAME="gpt-4o-mini"
```

Alternatively, you can provide these values directly in your code.

### Installation

Add the Agent Framework Azure AI package to your project:

```bash
pip install agent-framework-azure-ai --pre
```

## Getting Started

### Authentication

Foundry Agents use Azure credentials for authentication. The simplest approach is to use `AzureCliCredential` after running `az login`. All Azure AI clients accept a unified `credential` parameter that supports `TokenCredential`, `AsyncTokenCredential`, or a callable token provider — token caching and refresh are handled automatically.

```python
from azure.identity.aio import AzureCliCredential

async with AzureCliCredential() as credential:
    # Use credential with Azure AI agent client
```

## Create Foundry Agents

### Basic Agent Creation

# [Foundry Agent Service v1](#tab/foundry-v1)

The simplest way to create an agent is using the `AzureAIAgentClient` with environment variables:

```python
import asyncio
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        AzureAIAgentClient(credential=credential).as_agent(
            name="HelperAgent",
            instructions="You are a helpful assistant."
        ) as agent,
    ):
        result = await agent.run("Hello!")
        print(result.text)

asyncio.run(main())
```

# [Foundry Agent Service v2](#tab/foundry-v2)

The simplest way to create an agent is using the `AzureAIClient` with environment variables:

```python
import asyncio
from agent_framework.azure import AzureAIClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        AzureAIClient(credential=credential).as_agent(
            name="HelperAgent",
            instructions="You are a helpful assistant."
        ) as agent,
    ):
        result = await agent.run("Hello!")
        print(result.text)

asyncio.run(main())
```

---

### Explicit Configuration

# [Foundry Agent Service v1](#tab/foundry-v1)

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
            credential=credential,
            agent_name="HelperAgent"
        ).as_agent(
            instructions="You are a helpful assistant."
        ) as agent,
    ):
        result = await agent.run("Hello!")
        print(result.text)

asyncio.run(main())
```

# [Foundry Agent Service v2](#tab/foundry-v2)

You can also provide configuration explicitly instead of using environment variables:

```python
import asyncio
from agent_framework.azure import AzureAIClient
from azure.ai.projects.aio import AIProjectClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        AIProjectClient(
            endpoint="https://<your-project>.services.ai.azure.com/api/projects/<project-id>",
            credential=credential
        ) as project_client,
        AzureAIClient(
            project_client=project_client,
            model_deployment_name="gpt-4o-mini",
        ).as_agent(
            name="HelperAgent",
            instructions="You are a helpful assistant."
        ) as agent,
    ):
        result = await agent.run("Hello!")
        print(result.text)

asyncio.run(main())
```

---

## Using Existing Foundry Agents

### Using an Existing Agent

# [Foundry Agent Service v1](#tab/foundry-v1)

If you have an existing agent in Foundry, you can use it by providing its ID:

```python
import asyncio
from agent_framework import Agent
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        Agent(
            chat_client=AzureAIAgentClient(
                credential=credential,
                agent_id="<existing-agent-id>"
            ),
            instructions="You are a helpful assistant."
        ) as agent,
    ):
        result = await agent.run("Hello!")
        print(result.text)

asyncio.run(main())
```

# [Foundry Agent Service v2](#tab/foundry-v2)

If you have an existing agent in Foundry, you can retrieve it by name using the project agent provider:

```python
import asyncio
import os
from agent_framework.azure import AzureAIProjectAgentProvider
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
        provider = AzureAIProjectAgentProvider(project_client=project_client)
        agent = await provider.get_agent(name="<existing-agent-name>")

        result = await agent.run("Hello!")
        print(result)

asyncio.run(main())
```

---

### Create and Manage Persistent Agents

# [Foundry Agent Service v1](#tab/foundry-v1)

For more control over agent lifecycle, you can create persistent agents using the Azure AI Projects client:

```python
import asyncio
import os
from agent_framework import Agent
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
            async with Agent(
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

# [Foundry Agent Service v2](#tab/foundry-v2)

For more control over agent lifecycle, you can create persistent agents using the Azure AI Projects client:

```python
import asyncio
import os
from agent_framework.azure import AzureAIProjectAgentProvider
from azure.ai.projects.aio import AIProjectClient
from azure.ai.projects.models import PromptAgentDefinition
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
        created_agent = await project_client.agents.create_version(
            agent_name="PersistentAgent",
            definition=PromptAgentDefinition(
                model=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
                instructions="You are a helpful assistant."
            ),
        )

        try:
            # Use the agent
            provider = AzureAIProjectAgentProvider(project_client=project_client)
            agent = provider.as_agent(created_agent)

            result = await agent.run("Hello!")
            print(result)
        finally:
            # Clean up the agent
            await project_client.agents.delete_version(
                agent_name=created_agent.name,
                agent_version=created_agent.version
            )

asyncio.run(main())
```

---

## Agent Features

### Reasoning and content filtering options

When creating agents through Azure AI project providers, you can set `default_options` to enable model reasoning and Responsible AI content filtering.

Use `reasoning` for reasoning-capable models:

:::code language="python" source="~/../agent-framework-code/python/samples/02-agents/providers/azure_ai/azure_ai_with_reasoning.py" range="5-7,31-35,60-64":::

Use `rai_config` to apply a configured RAI policy:

:::code language="python" source="~/../agent-framework-code/python/samples/02-agents/providers/azure_ai/azure_ai_with_content_filtering.py" range="6-7,32-46":::

### Function Tools

# [Foundry Agent Service v1](#tab/foundry-v1)

You can provide custom function tools to Foundry agents:

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
        AzureAIAgentClient(credential=credential).as_agent(
            name="WeatherAgent",
            instructions="You are a helpful weather assistant.",
            tools=get_weather
        ) as agent,
    ):
        result = await agent.run("What's the weather like in Seattle?")
        print(result.text)

asyncio.run(main())
```

# [Foundry Agent Service v2](#tab/foundry-v2)

You can provide custom function tools to Foundry agents:

```python
import asyncio
from typing import Annotated
from agent_framework.azure import AzureAIClient
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
        AzureAIClient(credential=credential).as_agent(
            name="WeatherAgent",
            instructions="You are a helpful weather assistant.",
            tools=get_weather
        ) as agent,
    ):
        result = await agent.run("What's the weather like in Seattle?")
        print(result.text)

asyncio.run(main())
```

---

### Code Interpreter

# [Foundry Agent Service v1](#tab/foundry-v1)

Foundry agents support code execution through the hosted code interpreter:

```python
import asyncio
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        AzureAIAgentClient(credential=credential) as client,
        client.as_agent(
            name="CodingAgent",
            instructions="You are a helpful assistant that can write and execute Python code.",
            tools=client.get_code_interpreter_tool(),
        ) as agent,
    ):
        result = await agent.run("Calculate the factorial of 20 using Python code.")
        print(result.text)

asyncio.run(main())
```

# [Foundry Agent Service v2](#tab/foundry-v2)

Foundry agents support code execution through the hosted code interpreter:

```python
import asyncio
from agent_framework.azure import AzureAIClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        AzureAIClient(credential=credential) as client,
        client.as_agent(
            name="CodingAgent",
            instructions="You are a helpful assistant that can write and execute Python code.",
            tools=client.get_code_interpreter_tool(),
        ) as agent,
    ):
        result = await agent.run("Calculate the factorial of 20 using Python code.")
        print(result.text)

asyncio.run(main())
```

---

### Streaming Responses

# [Foundry Agent Service v1](#tab/foundry-v1)

Get responses as they are generated using streaming:

```python
import asyncio
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        AzureAIAgentClient(credential=credential).as_agent(
            name="StreamingAgent",
            instructions="You are a helpful assistant."
        ) as agent,
    ):
        print("Agent: ", end="", flush=True)
        async for chunk in agent.run("Tell me a short story", stream=True):
            if chunk.text:
                print(chunk.text, end="", flush=True)
        print()

asyncio.run(main())
```

# [Foundry Agent Service v2](#tab/foundry-v2)

Get responses as they are generated using streaming:

```python
import asyncio
from agent_framework.azure import AzureAIClient
from azure.identity.aio import AzureCliCredential

async def main():
    async with (
        AzureCliCredential() as credential,
        AzureAIClient(credential=credential).as_agent(
            name="StreamingAgent",
            instructions="You are a helpful assistant."
        ) as agent,
    ):
        print("Agent: ", end="", flush=True)
        async for chunk in agent.run("Tell me a short story", stream=True):
            if chunk.text:
                print(chunk.text, end="", flush=True)
        print()

asyncio.run(main())
```

---

## Using the Agent

The agent is a standard `BaseAgent` and supports all standard agent operations.

For more information on how to run and interact with agents, see the [Agent getting started tutorials](../../get-started/your-first-agent.md).

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Foundry Models based Agents](./microsoft-foundry.md)
