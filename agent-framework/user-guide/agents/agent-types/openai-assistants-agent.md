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
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

## Creating an OpenAI Assistants Agent

As a first step you need to create a client to connect to the OpenAI service.

```csharp
using System;
using Microsoft.Agents.AI;
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

## Using the Agent

The agent is a standard `AIAgent` and supports all standard agent operations.

See the [Agent getting started tutorials](../../../tutorials/overview.md) for more information on how to run and interact with agents.

::: zone-end
::: zone pivot="programming-language-python"

## Prerequisites

Install the Microsoft Agent Framework package.

```bash
pip install agent-framework
```

## Configuration

### Environment Variables

Set up the required environment variables for OpenAI authentication:

```bash
# Required for OpenAI API access
OPENAI_API_KEY="your-openai-api-key"
OPENAI_CHAT_MODEL_ID="gpt-4o-mini"  # or your preferred model
```

Alternatively, you can use a `.env` file in your project root:

```env
OPENAI_API_KEY=your-openai-api-key
OPENAI_CHAT_MODEL_ID=gpt-4o-mini
```

## Getting Started

Import the required classes from the Agent Framework:

```python
import asyncio
from agent_framework import ChatAgent
from agent_framework.openai import OpenAIAssistantsClient
```

## Creating an OpenAI Assistants Agent

### Basic Agent Creation

The simplest way to create an agent is by using the `OpenAIAssistantsClient` which automatically creates and manages assistants:

```python
async def basic_example():
    # Create an agent with automatic assistant creation and cleanup
    async with OpenAIAssistantsClient().create_agent(
        instructions="You are a helpful assistant.",
        name="MyAssistant"
    ) as agent:
        result = await agent.run("Hello, how are you?")
        print(result.text)
```

### Using Explicit Configuration

You can provide explicit configuration instead of relying on environment variables:

```python
async def explicit_config_example():
    async with OpenAIAssistantsClient(
        ai_model_id="gpt-4o-mini",
        api_key="your-api-key-here",
    ).create_agent(
        instructions="You are a helpful assistant.",
    ) as agent:
        result = await agent.run("What's the weather like?")
        print(result.text)
```

### Using an Existing Assistant

You can reuse existing OpenAI assistants by providing their IDs:

```python
from openai import AsyncOpenAI

async def existing_assistant_example():
    # Create OpenAI client directly
    client = AsyncOpenAI()
    
    # Create or get an existing assistant
    assistant = await client.beta.assistants.create(
        model="gpt-4o-mini",
        name="WeatherAssistant",
        instructions="You are a weather forecasting assistant."
    )
    
    try:
        # Use the existing assistant with Agent Framework
        async with ChatAgent(
            chat_client=OpenAIAssistantsClient(
                async_client=client,
                assistant_id=assistant.id
            ),
            instructions="You are a helpful weather agent.",
        ) as agent:
            result = await agent.run("What's the weather like in Seattle?")
            print(result.text)
    finally:
        # Clean up the assistant
        await client.beta.assistants.delete(assistant.id)
```

## Agent Features

### Function Tools

You can equip your assistant with custom functions:

```python
from typing import Annotated
from pydantic import Field

def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")]
) -> str:
    """Get the weather for a given location."""
    return f"The weather in {location} is sunny with 25°C."

async def tools_example():
    async with ChatAgent(
        chat_client=OpenAIAssistantsClient(),
        instructions="You are a helpful weather assistant.",
        tools=get_weather,  # Provide tools to the agent
    ) as agent:
        result = await agent.run("What's the weather like in Tokyo?")
        print(result.text)
```

### Code Interpreter

Enable your assistant to execute Python code:

```python
from agent_framework import HostedCodeInterpreterTool

async def code_interpreter_example():
    async with ChatAgent(
        chat_client=OpenAIAssistantsClient(),
        instructions="You are a helpful assistant that can write and execute Python code.",
        tools=HostedCodeInterpreterTool(),
    ) as agent:
        result = await agent.run("Calculate the factorial of 100 using Python code.")
        print(result.text)
```

### Streaming Responses

Get responses as they are generated for better user experience:

```python
async def streaming_example():
    async with OpenAIAssistantsClient().create_agent(
        instructions="You are a helpful assistant.",
    ) as agent:
        print("Assistant: ", end="", flush=True)
        async for chunk in agent.run_stream("Tell me a story about AI."):
            if chunk.text:
                print(chunk.text, end="", flush=True)
        print()  # New line after streaming is complete
```

## Using the Agent

The agent is a standard `BaseAgent` and supports all standard agent operations.

See the [Agent getting started tutorials](../../../tutorials/overview.md) for more information on how to run and interact with agents.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Chat Client Agents](./chat-client-agent.md)
