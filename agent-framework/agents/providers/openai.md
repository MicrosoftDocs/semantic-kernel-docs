---
title: OpenAI Agents
description: Learn how to use Microsoft Agent Framework with OpenAI services — Chat Completions, Responses, and Assistants APIs.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 03/26/2026
ms.service: agent-framework
---

# OpenAI Agents

Microsoft Agent Framework supports three distinct OpenAI client types, each targeting a different API surface with different tool capabilities:

| Client Type | API | Best For |
|---|---|---|
| **Chat Completion** | [Chat Completions API](https://platform.openai.com/docs/api-reference/chat/create) | Simple agents, broad model support |
| **Responses** | [Responses API](https://platform.openai.com/docs/api-reference/responses) | Full-featured agents with hosted tools (code interpreter, file search, web search, hosted MCP) |
| **Assistants** | [Assistants API](https://platform.openai.com/docs/api-reference/assistants) | Server-managed agents with code interpreter and file search |


::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```dotnetcli
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

## Chat Completion Client

The Chat Completion client provides a straightforward way to create agents using the ChatCompletion API.

```csharp
using Microsoft.Agents.AI;
using OpenAI;

OpenAIClient client = new OpenAIClient("<your_api_key>");
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: "You are good at telling jokes.",
    name: "Joker");

Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));
```

**Supported tools:** Function tools, web search, local MCP tools.

## Responses Client

The Responses client provides the richest tool support including code interpreter, file search, web search, and hosted MCP.

```csharp
using Microsoft.Agents.AI;
using OpenAI;

OpenAIClient client = new OpenAIClient("<your_api_key>");
var responsesClient = client.GetResponseClient("gpt-4o-mini");

AIAgent agent = responsesClient.AsAIAgent(
    instructions: "You are a helpful coding assistant.",
    name: "CodeHelper");

Console.WriteLine(await agent.RunAsync("Write a Python function to sort a list."));
```

**Supported tools:** Function tools, tool approval, code interpreter, file search, web search, hosted MCP, local MCP tools.

## Assistants Client

The Assistants client creates server-managed agents with built-in code interpreter and file search.

```csharp
using Microsoft.Agents.AI;
using OpenAI;

OpenAIClient client = new OpenAIClient("<your_api_key>");
var assistantsClient = client.GetAssistantClient();

// Assistants are managed server-side
AIAgent agent = assistantsClient.AsAIAgent(
    instructions: "You are a data analysis assistant.",
    name: "DataHelper");

Console.WriteLine(await agent.RunAsync("Analyze trends in the uploaded data."));
```

**Supported tools:** Function tools, code interpreter, file search, local MCP tools.

> [!TIP]
> See the [.NET samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples) for complete runnable examples.

## Using the Agent

All three client types produce a standard `AIAgent` that supports the same agent operations (streaming, threads, middleware).

For more information, see the [Get Started tutorials](../../get-started/your-first-agent.md).

::: zone-end
::: zone pivot="programming-language-python"

> [!TIP]
> In Python, Azure OpenAI now uses the same `agent_framework.openai` clients shown here. Pass explicit Azure routing inputs such as `credential` or `azure_endpoint` when you want Azure routing, then set `api_version` for the Azure API surface you want to use. If `OPENAI_API_KEY` is configured, the generic clients stay on OpenAI even when `AZURE_OPENAI_*` variables are also present. If you already have a full `.../openai/v1` URL, use `base_url` instead of `azure_endpoint`. For Microsoft Foundry project endpoints and the Foundry Agent Service, see the [Microsoft Foundry provider page](./microsoft-foundry.md). For local runtimes, see [Foundry Local](./foundry-local.md).


## Installation

```bash
pip install agent-framework agent-framework-openai --pre
```

`agent-framework-openai` is the optional Python provider package for both direct OpenAI and Azure OpenAI usage.

## Configuration

Each client type uses different environment variables:

# [Chat Completion](#tab/oai-chat-completion)

```bash
OPENAI_API_KEY="your-openai-api-key"
OPENAI_MODEL="gpt-4o-mini"
```

# [Responses](#tab/oai-responses)

```bash
OPENAI_API_KEY="your-openai-api-key"
OPENAI_MODEL="gpt-4o-mini"
```

# [Assistants](#tab/oai-assistants)

```bash
OPENAI_API_KEY="your-openai-api-key"
OPENAI_MODEL="gpt-4o-mini"
```

---

### Azure OpenAI with the same clients

Azure OpenAI now uses the same Python OpenAI clients as direct OpenAI. The preferred and clearest Azure pattern is to pass explicit Azure routing inputs such as `credential` or `azure_endpoint`, then set `api_version` for Azure once routing is selected. If `OPENAI_API_KEY` is set, the generic clients stay on OpenAI unless you pass those Azure routing inputs. If you only have `AZURE_OPENAI_*` settings, Azure environment fallback still works.

```bash
AZURE_OPENAI_ENDPOINT="https://<resource>.openai.azure.com"
AZURE_OPENAI_DEPLOYMENT_NAME="gpt-4o-mini"
AZURE_OPENAI_API_VERSION="your-api-version"
```

```python
import asyncio
import os
from agent_framework.openai import OpenAIChatClient
from azure.identity import AzureCliCredential

async def main():
    agent = OpenAIChatClient(
        model=os.environ["AZURE_OPENAI_DEPLOYMENT_NAME"],
        azure_endpoint=os.environ["AZURE_OPENAI_ENDPOINT"],
        api_version=os.getenv("AZURE_OPENAI_API_VERSION"),
        credential=AzureCliCredential(),
    ).as_agent(
        name="AzureOpenAIResponsesAgent",
        instructions="You are a helpful assistant.",
    )

    result = await agent.run("Hello!")
    print(result)

asyncio.run(main())
```

If you already have a full Azure OpenAI URL that ends with `/openai/v1`, pass it as `base_url` instead of `azure_endpoint`. Keep `api_version` aligned to the Azure OpenAI API surface you are using. If `OPENAI_API_KEY` is also set in your environment, these explicit Azure inputs keep the client on Azure.

> [!NOTE]
> Use `OpenAIChatClient` for the Responses API. For Azure key auth, you can still pass `api_key`, but `credential=` is now the preferred Azure auth surface.

### Azure embeddings with the same client family

`OpenAIEmbeddingClient` follows the same routing rules as the chat clients. For Azure embeddings, pass the deployment name as `model` and prefer explicit Azure inputs:

```python
import os
from agent_framework.openai import OpenAIEmbeddingClient
from azure.identity import AzureCliCredential

client = OpenAIEmbeddingClient(
    model=os.environ["AZURE_OPENAI_EMBEDDING_DEPLOYMENT_NAME"],
    azure_endpoint=os.environ["AZURE_OPENAI_ENDPOINT"],
    api_version=os.getenv("AZURE_OPENAI_API_VERSION"),
    credential=AzureCliCredential(),
)
```

## Create OpenAI Agents

# [Chat Completion](#tab/oai-chat-completion)

`OpenAIChatCompletionClient` uses the Chat Completions API — the simplest option with broad model support.

```python
import asyncio
from agent_framework.openai import OpenAIChatCompletionClient

async def main():
    agent = OpenAIChatCompletionClient().as_agent(
        name="HelpfulAssistant",
        instructions="You are a helpful assistant.",
    )
    result = await agent.run("Hello, how can you help me?")
    print(result)

asyncio.run(main())
```

**Supported tools:** Function tools, web search, local MCP tools.

### Web Search with Chat Completion

```python
async def web_search_example():
    client = OpenAIChatCompletionClient()
    web_search = client.get_web_search_tool()

    agent = client.as_agent(
        name="SearchBot",
        instructions="You can search the web for current information.",
        tools=web_search,
    )
    result = await agent.run("What are the latest developments in AI?")
    print(result)
```

# [Responses](#tab/oai-responses)

`OpenAIChatClient` uses the Responses API — the most feature-rich option with hosted tools.

```python
import asyncio
from agent_framework.openai import OpenAIChatClient

async def main():
    agent = OpenAIChatClient().as_agent(
        name="FullFeaturedAgent",
        instructions="You are a helpful assistant with access to many tools.",
    )
    result = await agent.run("Write and run a Python script that calculates fibonacci numbers.")
    print(result)

asyncio.run(main())
```

**Supported tools:** Function tools, tool approval, code interpreter, file search, web search, hosted MCP, local MCP tools.

### Hosted Tools with Responses Client

The Responses client provides `get_*_tool()` methods for each hosted tool type:

```python
async def hosted_tools_example():
    client = OpenAIChatClient()

    # Each tool is created via a client method
    code_interpreter = client.get_code_interpreter_tool()
    web_search = client.get_web_search_tool()
    file_search = client.get_file_search_tool(vector_store_ids=["vs_abc123"])
    mcp_tool = client.get_mcp_tool(
        name="GitHub",
        url="https://api.githubcopilot.com/mcp/",
        approval_mode="never_require",
    )

    agent = client.as_agent(
        name="PowerAgent",
        instructions="You have access to code execution, web search, files, and GitHub.",
        tools=[code_interpreter, web_search, file_search, mcp_tool],
    )
    result = await agent.run("Search the web for Python best practices, then write a summary.")
    print(result)
```

# [Assistants](#tab/oai-assistants)

`OpenAIAssistantProvider` uses the Assistants API — server-managed agents with built-in code interpreter and file search. The provider manages assistant lifecycle automatically.

> [!WARNING]
> Python Assistants API support (`OpenAIAssistantProvider` / `OpenAIAssistantsClient`) is a compatibility path and will be removed before GA. For new Python code, prefer the Responses API via `OpenAIChatClient`.

```python
import asyncio
from agent_framework.openai import OpenAIAssistantProvider
from openai import AsyncOpenAI

async def main():
    client = AsyncOpenAI()
    provider = OpenAIAssistantProvider(client)

    agent = await provider.create_agent(
        name="DataAnalyst",
        model="gpt-4o-mini",
        instructions="You analyze data using code execution.",
    )

    try:
        result = await agent.run("Calculate the first 20 prime numbers.")
        print(result)
    finally:
        await provider.delete_agent(agent.id)

asyncio.run(main())
```

**Supported tools:** Function tools, code interpreter, file search, local MCP tools.

---

## Common Features

All three client types support these standard agent features:

### Function Tools

```python
from agent_framework import tool

@tool
def get_weather(location: str) -> str:
    """Get the weather for a given location."""
    return f"The weather in {location} is sunny, 25°C."

async def example():
    agent = OpenAIChatClient().as_agent(
        instructions="You are a weather assistant.",
        tools=get_weather,
    )
    result = await agent.run("What's the weather in Tokyo?")
    print(result)
```

### Multi-Turn Conversations

```python
async def thread_example():
    agent = OpenAIChatClient().as_agent(
        instructions="You are a helpful assistant.",
    )
    session = await agent.create_session()

    result1 = await agent.run("My name is Alice", session=session)
    print(result1)
    result2 = await agent.run("What's my name?", session=session)
    print(result2)  # Remembers "Alice"
```

### Streaming

```python
async def streaming_example():
    agent = OpenAIChatClient().as_agent(
        instructions="You are a creative storyteller.",
    )
    print("Agent: ", end="", flush=True)
    async for chunk in agent.run("Tell me a short story about AI.", stream=True):
        if chunk.text:
            print(chunk.text, end="", flush=True)
    print()
```

## Using the Agent

All client types produce a standard `Agent` that supports the same operations.

For more information, see the [Get Started tutorials](../../get-started/your-first-agent.md).

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Microsoft Foundry](./microsoft-foundry.md)
