---
title: OpenAI Responses Agents
description: Learn how to use the Microsoft Agent Framework with OpenAI Responses service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# OpenAI Responses Agents

The Microsoft Agent Framework supports creating agents that use the [OpenAI responses](https://platform.openai.com/docs/api-reference/responses/create) service.

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```powershell
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

## Creating an OpenAI Responses Agent

As a first step you need to create a client to connect to the OpenAI service.

```csharp
using System;
using Microsoft.Agents.AI;
using OpenAI;

OpenAIClient client = new OpenAIClient("<your_api_key>");
```

OpenAI supports multiple services that all provide model calling capabilities.
We need to pick the Responses service to create a Responses based agent.

```csharp
var responseClient = client.GetOpenAIResponseClient("gpt-4o-mini");
```

Finally, create the agent using the `CreateAIAgent` extension method on the `ResponseClient`.

```csharp
AIAgent agent = responseClient.CreateAIAgent(
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
OPENAI_RESPONSES_MODEL_ID="gpt-4o"  # or your preferred Responses-compatible model
```

Alternatively, you can use a `.env` file in your project root:

```env
OPENAI_API_KEY=your-openai-api-key
OPENAI_RESPONSES_MODEL_ID=gpt-4o
```

## Getting Started

Import the required classes from the Agent Framework:

```python
import asyncio
from agent_framework.openai import OpenAIResponsesClient
```

## Creating an OpenAI Responses Agent

### Basic Agent Creation

The simplest way to create a responses agent:

```python
async def basic_example():
    # Create an agent using OpenAI Responses
    agent = OpenAIResponsesClient().create_agent(
        name="WeatherBot",
        instructions="You are a helpful weather assistant.",
    )

    result = await agent.run("What's a good way to check the weather?")
    print(result.text)
```

### Using Explicit Configuration

You can provide explicit configuration instead of relying on environment variables:

```python
async def explicit_config_example():
    agent = OpenAIResponsesClient(
        ai_model_id="gpt-4o",
        api_key="your-api-key-here",
    ).create_agent(
        instructions="You are a helpful assistant.",
    )

    result = await agent.run("Tell me about AI.")
    print(result.text)
```

## Basic Usage Patterns

### Streaming Responses

Get responses as they are generated for better user experience:

```python
async def streaming_example():
    agent = OpenAIResponsesClient().create_agent(
        instructions="You are a creative storyteller.",
    )

    print("Assistant: ", end="", flush=True)
    async for chunk in agent.run_stream("Tell me a short story about AI."):
        if chunk.text:
            print(chunk.text, end="", flush=True)
    print()  # New line after streaming
```

## Agent Features

### Reasoning Models

Use advanced reasoning capabilities with models like GPT-5:

```python
from agent_framework import HostedCodeInterpreterTool, TextContent, TextReasoningContent

async def reasoning_example():
    agent = OpenAIResponsesClient(ai_model_id="gpt-5").create_agent(
        name="MathTutor",
        instructions="You are a personal math tutor. When asked a math question, "
                    "write and run code to answer the question.",
        tools=HostedCodeInterpreterTool(),
        reasoning={"effort": "high", "summary": "detailed"},
    )

    print("Assistant: ", end="", flush=True)
    async for chunk in agent.run_stream("Solve: 3x + 11 = 14"):
        if chunk.contents:
            for content in chunk.contents:
                if isinstance(content, TextReasoningContent):
                    # Reasoning content in gray text
                    print(f"\033[97m{content.text}\033[0m", end="", flush=True)
                elif isinstance(content, TextContent):
                    print(content.text, end="", flush=True)
    print()
```

### Structured Output

Get responses in structured formats:

```python
from pydantic import BaseModel
from agent_framework import AgentRunResponse

class CityInfo(BaseModel):
    """A structured output for city information."""
    city: str
    description: str

async def structured_output_example():
    agent = OpenAIResponsesClient().create_agent(
        name="CityExpert",
        instructions="You describe cities in a structured format.",
    )

    # Non-streaming structured output
    result = await agent.run("Tell me about Paris, France", response_format=CityInfo)
    
    if result.value:
        city_data = result.value
        print(f"City: {city_data.city}")
        print(f"Description: {city_data.description}")

    # Streaming structured output
    structured_result = await AgentRunResponse.from_agent_response_generator(
        agent.run_stream("Tell me about Tokyo, Japan", response_format=CityInfo),
        output_format_type=CityInfo,
    )
    
    if structured_result.value:
        tokyo_data = structured_result.value
        print(f"City: {tokyo_data.city}")
        print(f"Description: {tokyo_data.description}")
```

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
    agent = OpenAIResponsesClient().create_agent(
        instructions="You are a helpful weather assistant.",
        tools=get_weather,
    )

    result = await agent.run("What's the weather like in Tokyo?")
    print(result.text)
```

### Image Generation

Generate images using the Responses API:

```python
from agent_framework import DataContent, UriContent

async def image_generation_example():
    agent = OpenAIResponsesClient().create_agent(
        instructions="You are a helpful AI that can generate images.",
        tools=[{
            "type": "image_generation",
            "size": "1024x1024",
            "quality": "low",
        }],
    )

    result = await agent.run("Generate an image of a sunset over the ocean.")
    
    # Check for generated images in the response
    for content in result.contents:
        if isinstance(content, (DataContent, UriContent)):
            print(f"Image generated: {content.uri}")
```

### Code Interpreter

Enable your assistant to execute Python code:

```python
from agent_framework import HostedCodeInterpreterTool

async def code_interpreter_example():
    agent = OpenAIResponsesClient().create_agent(
        instructions="You are a helpful assistant that can write and execute Python code.",
        tools=HostedCodeInterpreterTool(),
    )

    result = await agent.run("Calculate the factorial of 100 using Python code.")
    print(result.text)
```

## Using the Agent

The agent is a standard `BaseAgent` and supports all standard agent operations.

See the [Agent getting started tutorials](../../../tutorials/overview.md) for more information on how to run and interact with agents.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [OpenAI Assistant Agents](./openai-assistants-completion-agent.md)
