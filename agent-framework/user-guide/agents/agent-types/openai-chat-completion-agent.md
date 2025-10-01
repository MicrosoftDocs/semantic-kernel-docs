---
title: OpenAI ChatCompletion Agents
description: Learn how to use the Microsoft Agent Framework with OpenAI ChatCompletion service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# OpenAI ChatCompletion Agents

The Microsoft Agent Framework supports creating agents that use the [OpenAI ChatCompletion](https://platform.openai.com/docs/api-reference/chat/create) service.

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```powershell
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

## Creating an OpenAI ChatCompletion Agent

As a first step you need to create a client to connect to the OpenAI service.

```csharp
using System;
using Microsoft.Agents.AI;
using OpenAI;

OpenAIClient client = new OpenAIClient("<your_api_key>");
```

OpenAI supports multiple services that all provide model calling capabilities.
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

## Using the Agent

The agent is a standard `AIAgent` and supports all standard `AIAgent` operations.

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
from agent_framework.openai import OpenAIChatClient
```

## Creating an OpenAI ChatCompletion Agent

### Basic Agent Creation

The simplest way to create a chat completion agent:

```python
async def basic_example():
    # Create an agent using OpenAI ChatCompletion
    agent = OpenAIChatClient().create_agent(
        name="HelpfulAssistant",
        instructions="You are a helpful assistant.",
    )

    result = await agent.run("Hello, how can you help me?")
    print(result.text)
```

### Using Explicit Configuration

You can provide explicit configuration instead of relying on environment variables:

```python
async def explicit_config_example():
    agent = OpenAIChatClient(
        ai_model_id="gpt-4o-mini",
        api_key="your-api-key-here",
    ).create_agent(
        instructions="You are a helpful assistant.",
    )

    result = await agent.run("What can you do?")
    print(result.text)
```

## Agent Features

### Function Tools

Equip your agent with custom functions:

```python
from typing import Annotated
from pydantic import Field

def get_weather(
    location: Annotated[str, Field(description="The location to get weather for")]
) -> str:
    """Get the weather for a given location."""
    # Your weather API implementation here
    return f"The weather in {location} is sunny with 25°C."

async def tools_example():
    agent = ChatAgent(
        chat_client=OpenAIChatClient(),
        instructions="You are a helpful weather assistant.",
        tools=get_weather,  # Add tools to the agent
    )

    result = await agent.run("What's the weather like in Tokyo?")
    print(result.text)
```

### Streaming Responses

Get responses as they are generated for better user experience:

```python
async def streaming_example():
    agent = OpenAIChatClient().create_agent(
        name="StoryTeller",
        instructions="You are a creative storyteller.",
    )

    print("Assistant: ", end="", flush=True)
    async for chunk in agent.run_stream("Tell me a short story about AI."):
        if chunk.text:
            print(chunk.text, end="", flush=True)
    print()  # New line after streaming
```

## Using the Agent

The agent is a standard `BaseAgent` and supports all standard agent operations.

See the [Agent getting started tutorials](../../../tutorials/overview.md) for more information on how to run and interact with agents.

::: zone-end
