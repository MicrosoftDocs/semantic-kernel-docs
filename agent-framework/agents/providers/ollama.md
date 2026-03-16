---
title: "Ollama"
description: "Learn how to use Ollama as a provider for Agent Framework agents."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: reference
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Ollama

Ollama allows you to run open-source models locally and use them with Agent Framework. This is ideal for development, testing, and scenarios where you need to keep data on-premises.

:::zone pivot="programming-language-csharp"

The following example shows how to create an agent using Ollama:

```csharp
using System;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// Create an Ollama agent using Microsoft.Extensions.AI.Ollama
// Requires: dotnet add package Microsoft.Extensions.AI.Ollama --prerelease
var chatClient = new OllamaChatClient(
    new Uri("http://localhost:11434"),
    modelId: "llama3.2");

AIAgent agent = chatClient.AsAIAgent(
    instructions: "You are a helpful assistant running locally via Ollama.");

Console.WriteLine(await agent.RunAsync("What is the largest city in France?"));
```

:::zone-end

:::zone pivot="programming-language-python"

## Prerequisites

Ensure [Ollama](https://ollama.com/) is installed and running locally with a model downloaded before running any examples:

```bash
ollama pull llama3.2
```

> [!NOTE]
> Not all models support function calling. For tool usage, try `llama3.2` or `qwen3:4b`.

## Installation

# [Native Ollama](#tab/ollama-native)

```bash
pip install agent-framework-ollama --pre
```

# [OpenAI Compatible](#tab/ollama-openai)

```bash
pip install agent-framework --pre
```

---

## Configuration

# [Native Ollama](#tab/ollama-native)

```bash
OLLAMA_MODEL_ID="llama3.2"
```

The native client connects to `http://localhost:11434` by default. You can override this by passing `host` to the client.

# [OpenAI Compatible](#tab/ollama-openai)

```bash
OLLAMA_ENDPOINT="http://localhost:11434/v1/"
OLLAMA_MODEL="llama3.2"
```

---

## Create Ollama Agents

# [Native Ollama](#tab/ollama-native)

`OllamaChatClient` provides native Ollama integration with full support for function tools and streaming.

```python
import asyncio
from agent_framework.ollama import OllamaChatClient

async def main():
    agent = OllamaChatClient().as_agent(
        name="HelpfulAssistant",
        instructions="You are a helpful assistant running locally via Ollama.",
    )
    result = await agent.run("What is the largest city in France?")
    print(result)

asyncio.run(main())
```

# [OpenAI Compatible](#tab/ollama-openai)

You can also use `OpenAIChatClient` with a custom base URL pointing to your Ollama instance.

```python
import asyncio
import os
from agent_framework.openai import OpenAIChatClient

async def main():
    agent = OpenAIChatClient(
        api_key="ollama",  # Placeholder, Ollama doesn't require an API key
        base_url=os.environ["OLLAMA_ENDPOINT"],
        model_id=os.environ["OLLAMA_MODEL"],
    ).as_agent(
        name="HelpfulAssistant",
        instructions="You are a helpful assistant running locally via Ollama.",
    )
    result = await agent.run("What is the largest city in France?")
    print(result)

asyncio.run(main())
```

---

## Function Tools

# [Native Ollama](#tab/ollama-native)

```python
import asyncio
from datetime import datetime
from agent_framework.ollama import OllamaChatClient

def get_time(location: str) -> str:
    """Get the current time."""
    return f"The current time in {location} is {datetime.now().strftime('%I:%M %p')}."

async def main():
    agent = OllamaChatClient().as_agent(
        name="TimeAgent",
        instructions="You are a helpful time agent.",
        tools=get_time,
    )
    result = await agent.run("What time is it in Seattle?")
    print(result)

asyncio.run(main())
```

# [OpenAI Compatible](#tab/ollama-openai)

```python
import asyncio
import os
from datetime import datetime
from agent_framework.openai import OpenAIChatClient

def get_time(location: str) -> str:
    """Get the current time."""
    return f"The current time in {location} is {datetime.now().strftime('%I:%M %p')}."

async def main():
    agent = OpenAIChatClient(
        api_key="ollama",
        base_url=os.environ["OLLAMA_ENDPOINT"],
        model_id=os.environ["OLLAMA_MODEL"],
    ).as_agent(
        name="TimeAgent",
        instructions="You are a helpful time agent.",
        tools=get_time,
    )
    result = await agent.run("What time is it in Seattle?")
    print(result)

asyncio.run(main())
```

---

## Streaming

```python
async def streaming_example():
    agent = OllamaChatClient().as_agent(
        instructions="You are a helpful assistant.",
    )
    print("Agent: ", end="", flush=True)
    async for chunk in agent.run("Tell me about Python.", stream=True):
        if chunk.text:
            print(chunk.text, end="", flush=True)
    print()
```

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Providers Overview](./index.md)
