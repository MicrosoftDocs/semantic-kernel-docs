---
title: Azure OpenAI Agents
description: Learn how to use Microsoft Agent Framework with Azure OpenAI services — Chat Completions, Responses, and Assistants APIs.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 02/12/2026
ms.service: agent-framework
---

# Azure OpenAI Agents

Microsoft Agent Framework supports three distinct Azure OpenAI client types, each targeting a different API surface with different tool capabilities:

| Client Type | API | Best For |
|---|---|---|
| **Chat Completion** | [Chat Completions API](/azure/ai-services/openai/how-to/chatgpt) | Simple agents, broad model support |
| **Responses** | [Responses API](/azure/ai-services/openai/how-to/responses) | Full-featured agents with hosted tools (code interpreter, file search, web search, hosted MCP) |
| **Assistants** | [Assistants API](/azure/ai-services/openai/how-to/assistant) | Server-managed agents with code interpreter and file search |

> [!TIP]
> For direct OpenAI equivalents (`OpenAIChatClient`, `OpenAIResponsesClient`, `OpenAIAssistantsClient`), see the [OpenAI provider page](./openai.md). The tool support is identical.

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```dotnetcli
dotnet add package Azure.AI.OpenAI --prerelease
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

All Azure OpenAI client types start by creating an `AzureOpenAIClient`:

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;

AzureOpenAIClient client = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new DefaultAzureCredential());
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

## Chat Completion Client

The Chat Completion client provides a straightforward way to create agents using the ChatCompletion API.

```csharp
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
var assistantsClient = client.GetAssistantClient();

AIAgent agent = assistantsClient.AsAIAgent(
    instructions: "You are a data analysis assistant.",
    name: "DataHelper");

Console.WriteLine(await agent.RunAsync("Analyze trends in the uploaded data."));
```

**Supported tools:** Function tools, code interpreter, file search, local MCP tools.

### Function Tools

You can provide custom function tools to any Azure OpenAI agent:

```csharp
using System.ComponentModel;
using Microsoft.Extensions.AI;

[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";

AIAgent agent = new AzureOpenAIClient(
    new Uri(endpoint),
    new DefaultAzureCredential())
     .GetChatClient(deploymentName)
     .AsAIAgent(instructions: "You are a helpful assistant", tools: [AIFunctionFactory.Create(GetWeather)]);

Console.WriteLine(await agent.RunAsync("What is the weather like in Amsterdam?"));
```

### Streaming Responses

```csharp
await foreach (var update in agent.RunStreamingAsync("Tell me a joke about a pirate."))
{
    Console.Write(update);
}
```

> [!TIP]
> See the [.NET samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples) for complete runnable examples.

## Using the Agent

All three client types produce a standard `AIAgent` that supports the same agent operations (streaming, threads, middleware).

For more information, see the [Get Started tutorials](../../get-started/your-first-agent.md).

::: zone-end
::: zone pivot="programming-language-python"

## Installation

```bash
pip install agent-framework --pre
```

## Configuration

Each client type uses different environment variables:

# [Chat Completion](#tab/aoai-chat-completion)

```bash
AZURE_OPENAI_ENDPOINT="https://<myresource>.openai.azure.com"
AZURE_OPENAI_CHAT_DEPLOYMENT_NAME="gpt-4o-mini"
```

# [Responses](#tab/aoai-responses)

```bash
AZURE_OPENAI_ENDPOINT="https://<myresource>.openai.azure.com"
AZURE_OPENAI_RESPONSES_DEPLOYMENT_NAME="gpt-4o-mini"
```

# [Assistants](#tab/aoai-assistants)

```bash
AZURE_OPENAI_ENDPOINT="https://<myresource>.openai.azure.com"
AZURE_OPENAI_CHAT_DEPLOYMENT_NAME="gpt-4o-mini"
```

---

Optionally, you can also set:

```bash
AZURE_OPENAI_API_VERSION="2024-10-21"  # Default API version
AZURE_OPENAI_API_KEY="<your-api-key>"  # If not using Azure CLI authentication
```

All clients use Azure credentials for authentication. The simplest approach is `AzureCliCredential` after running `az login`. All Azure clients accept a unified `credential` parameter that supports `TokenCredential`, `AsyncTokenCredential`, or a callable token provider — token caching and refresh are handled automatically.

## Create Azure OpenAI Agents

# [Chat Completion](#tab/aoai-chat-completion)

`AzureOpenAIChatClient` uses the Chat Completions API — the simplest option with broad model support.

```python
import asyncio
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity import AzureCliCredential

async def main():
    agent = AzureOpenAIChatClient(credential=AzureCliCredential()).as_agent(
        name="Joker",
        instructions="You are good at telling jokes.",
    )
    result = await agent.run("Tell me a joke about a pirate.")
    print(result)

asyncio.run(main())
```

**Supported tools:** Function tools, web search, local MCP tools.

# [Responses](#tab/aoai-responses)

`AzureOpenAIResponsesClient` uses the Responses API — the most feature-rich option with hosted tools.

```python
import asyncio
from agent_framework.azure import AzureOpenAIResponsesClient
from azure.identity import AzureCliCredential

async def main():
    agent = AzureOpenAIResponsesClient(credential=AzureCliCredential()).as_agent(
        name="FullFeaturedAgent",
        instructions="You are a helpful assistant with access to many tools.",
    )
    result = await agent.run("Write and run a Python script that calculates fibonacci numbers.")
    print(result)

asyncio.run(main())
```

### Responses Client with Microsoft Foundry project endpoint

`AzureOpenAIResponsesClient` can also be created from a Foundry project endpoint:

```python
from agent_framework.azure import AzureOpenAIResponsesClient
from azure.identity import AzureCliCredential

client = AzureOpenAIResponsesClient(
    project_endpoint="https://<your-project>.services.ai.azure.com/api/projects/<project-id>",
    deployment_name="gpt-4o-mini",
    credential=AzureCliCredential(),
)
agent = client.as_agent(
    name="FoundryResponsesAgent",
    instructions="You are a helpful assistant.",
)
```

**Supported tools:** Function tools, tool approval, code interpreter, file search, web search, hosted MCP, local MCP tools.

### Hosted Tools with Responses Client

The Responses client provides `get_*_tool()` methods for each hosted tool type:

```python
async def hosted_tools_example():
    client = AzureOpenAIResponsesClient(credential=AzureCliCredential())

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

# [Assistants](#tab/aoai-assistants)

`AzureOpenAIAssistantsClient` uses the Assistants API — server-managed agents with built-in code interpreter and file search. Note the `async with` context manager for automatic assistant lifecycle management.

```python
import asyncio
from agent_framework.azure import AzureOpenAIAssistantsClient
from azure.identity import AzureCliCredential

async def main():
    async with AzureOpenAIAssistantsClient(credential=AzureCliCredential()).as_agent(
        name="DataAnalyst",
        instructions="You analyze data using code execution.",
    ) as agent:
        result = await agent.run("Calculate the first 20 prime numbers.")
        print(result)

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
    agent = AzureOpenAIResponsesClient(credential=AzureCliCredential()).as_agent(
        instructions="You are a weather assistant.",
        tools=get_weather,
    )
    result = await agent.run("What's the weather in Tokyo?")
    print(result)
```

### Multi-Turn Conversations

```python
async def thread_example():
    agent = AzureOpenAIResponsesClient(credential=AzureCliCredential()).as_agent(
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
    agent = AzureOpenAIResponsesClient(credential=AzureCliCredential()).as_agent(
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
> [OpenAI Provider](./openai.md)
