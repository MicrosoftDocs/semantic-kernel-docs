---
title: Anthropic Agents
description: Learn how to use the Microsoft Agent Framework with Anthropic's Claude models.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: tutorial
ms.author: edvan
ms.date: 11/05/2025
ms.service: agent-framework
---

# Anthropic Agents

The Microsoft Agent Framework supports creating agents that use [Anthropic's Claude models](https://www.anthropic.com/claude).

::: zone pivot="programming-language-csharp"

Coming soon...

::: zone-end
::: zone pivot="programming-language-python"

## Prerequisites

Install the Microsoft Agent Framework Anthropic package.

```bash
pip install agent-framework-anthropic --pre
```

## Configuration

### Environment Variables

Set up the required environment variables for Anthropic authentication:

```bash
# Required for Anthropic API access
ANTHROPIC_API_KEY="your-anthropic-api-key"
ANTHROPIC_CHAT_MODEL_ID="claude-sonnet-4-5-20250929"  # or your preferred model
```

Alternatively, you can use a `.env` file in your project root:

```env
ANTHROPIC_API_KEY=your-anthropic-api-key
ANTHROPIC_CHAT_MODEL_ID=claude-sonnet-4-5-20250929
```

You can get an API key from the [Anthropic Console](https://console.anthropic.com/).

## Getting Started

Import the required classes from the Agent Framework:

```python
import asyncio
from agent_framework.anthropic import AnthropicClient
```

## Creating an Anthropic Agent

### Basic Agent Creation

The simplest way to create an Anthropic agent:

```python
async def basic_example():
    # Create an agent using Anthropic
    agent = AnthropicClient().create_agent(
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
    agent = AnthropicClient(
        model_id="claude-sonnet-4-5-20250929",
        api_key="your-api-key-here",
    ).create_agent(
        name="HelpfulAssistant",
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

def get_weather(
    location: Annotated[str, "The location to get the weather for."],
) -> str:
    """Get the weather for a given location."""
    conditions = ["sunny", "cloudy", "rainy", "stormy"]
    return f"The weather in {location} is {conditions[randint(0, 3)]} with a high of {randint(10, 30)}°C."

async def tools_example():
    agent = AnthropicClient().create_agent(
        name="WeatherAgent",
        instructions="You are a helpful weather assistant.",
        tools=get_weather,  # Add tools to the agent
    )

    result = await agent.run("What's the weather like in Seattle?")
    print(result.text)
```

### Streaming Responses

Get responses as they are generated for better user experience:

```python
async def streaming_example():
    agent = AnthropicClient().create_agent(
        name="WeatherAgent",
        instructions="You are a helpful weather agent.",
        tools=get_weather,
    )

    query = "What's the weather like in Portland and in Paris?"
    print(f"User: {query}")
    print("Agent: ", end="", flush=True)
    async for chunk in agent.run_stream(query):
        if chunk.text:
            print(chunk.text, end="", flush=True)
    print()
```

### Hosted Tools

Anthropic agents support hosted tools such as web search, MCP (Model Context Protocol), and code execution:

```python
from agent_framework import HostedMCPTool, HostedWebSearchTool

async def hosted_tools_example():
    agent = AnthropicClient().create_agent(
        name="DocsAgent",
        instructions="You are a helpful agent for both Microsoft docs questions and general questions.",
        tools=[
            HostedMCPTool(
                name="Microsoft Learn MCP",
                url="https://learn.microsoft.com/api/mcp",
            ),
            HostedWebSearchTool(),
        ],
        max_tokens=20000,
    )

    result = await agent.run("Can you compare Python decorators with C# attributes?")
    print(result.text)
```

### Extended Thinking (Reasoning)

Anthropic supports extended thinking capabilities through the `thinking` feature, which allows the model to show its reasoning process:

```python
from agent_framework import TextReasoningContent, UsageContent

async def thinking_example():
    agent = AnthropicClient().create_agent(
        name="DocsAgent",
        instructions="You are a helpful agent.",
        tools=[HostedWebSearchTool()],
        max_tokens=20000,
        additional_chat_options={
            "thinking": {"type": "enabled", "budget_tokens": 10000}
        },
    )

    query = "Can you compare Python decorators with C# attributes?"
    print(f"User: {query}")
    print("Agent: ", end="", flush=True)

    async for chunk in agent.run_stream(query):
        for content in chunk.contents:
            if isinstance(content, TextReasoningContent):
                # Display thinking in a different color
                print(f"\033[32m{content.text}\033[0m", end="", flush=True)
            if isinstance(content, UsageContent):
                print(f"\n\033[34m[Usage: {content.details}]\033[0m\n", end="", flush=True)
        if chunk.text:
            print(chunk.text, end="", flush=True)
    print()
```

## Using the Agent

The agent is a standard `BaseAgent` and supports all standard agent operations.

See the [Agent getting started tutorials](../../../tutorials/overview.md) for more information on how to run and interact with agents.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Azure AI Agents](./azure-ai-foundry-agent.md)
