---
title: Azure OpenAI Agents
description: Learn how to use Microsoft Agent Framework with Azure OpenAI services — Chat Completions and Responses APIs.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 04/01/2026
ms.service: agent-framework
---

# Azure OpenAI Agents

Microsoft Agent Framework supports two Azure OpenAI client types, each targeting a different API surface with different tool capabilities. **Responses is the recommended primary client**: it supports the full set of hosted tools. Use Chat Completion when you need broad model compatibility or have an existing Chat Completions integration to keep.

| Client Type | API | Best For |
|---|---|---|
| **Responses** (recommended) | [Responses API](/azure/ai-services/openai/how-to/responses) | Full-featured agents with hosted tools (code interpreter, file search, web search, hosted MCP) |
| **Chat Completion** | [Chat Completions API](/azure/ai-services/openai/how-to/chatgpt) | Simple agents, broad model support |

> [!TIP]
> For direct OpenAI equivalents (`OpenAIChatClient`, `OpenAIChatCompletionClient`), see the [OpenAI provider page](./openai.md). The tool support is identical.

::: zone pivot="programming-language-csharp"

> [!NOTE]
> The Azure OpenAI Assistants API is deprecated. New code should use the Responses client. If you are migrating from an existing Assistants-based app, see the [Semantic Kernel migration guide](../../migration-guide/from-semantic-kernel/index.md).

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

## Responses Client

The Responses client is the recommended primary client and provides the richest tool support including code interpreter, file search, web search, and hosted MCP.

```csharp
var responsesClient = client.GetResponsesClient();

AIAgent agent = responsesClient.AsAIAgent(
    model: "gpt-4o-mini",
    instructions: "You are a helpful coding assistant.",
    name: "CodeHelper");

Console.WriteLine(await agent.RunAsync("Write a Python function to sort a list."));
```

**Supported tools:** Function tools, tool approval, code interpreter, file search, web search, hosted MCP, local MCP tools.

## Chat Completion Client

The Chat Completion client provides a straightforward way to create agents using the Chat Completions API. Use it when you need broad model compatibility or have an existing Chat Completions integration.

```csharp
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: "You are good at telling jokes.",
    name: "Joker");

Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));
```

**Supported tools:** Function tools, web search, local MCP tools.

## Assistants Client

> [!NOTE]
> The Azure OpenAI Assistants API is deprecated. The Agent Framework no longer documents an Assistants client — use the Responses client above for new code. For migrating an existing app, see the [Semantic Kernel migration guide](../../migration-guide/from-semantic-kernel/index.md).

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

Both client types produce a standard `AIAgent` that supports the same agent operations (streaming, threads, middleware).

For more information, see the [Get Started tutorials](../../get-started/your-first-agent.md).

## Tools

The Azure OpenAI .NET clients share their tool surface with the matching OpenAI clients. See the [OpenAI provider page](./openai.md#tools) for the full per-client matrix — the Responses and Chat Completion Azure variants mirror their direct-OpenAI equivalents.

| Tool | Responses | Chat Completion |
|---|:---:|:---:|
| [Function Tools](../tools/function-tools.md) | ✅ | ✅ |
| [Tool Approval](../tools/tool-approval.md) | ✅ | ✅ |
| [Code Interpreter](../tools/code-interpreter.md) | ✅ | ❌ |
| [File Search](../tools/file-search.md) | ✅ | ❌ |
| [Web Search](../tools/web-search.md) | ✅ | ✅ |
| [Hosted MCP Tools](../tools/hosted-mcp-tools.md) | ✅ | ❌ |
| [Local MCP Tools](../tools/local-mcp-tools.md) | ✅ | ✅ |

> [!NOTE]
> **Tool Approval** is provided by the framework's function-invoking chat client, so it works with any function-tool call regardless of the underlying API.

::: zone-end
::: zone pivot="programming-language-python"

## Python guidance

> [!IMPORTANT]
> Python Azure OpenAI guidance now lives on the [OpenAI provider page](./openai.md). Use that page for `OpenAIChatCompletionClient`, `OpenAIChatClient`, and `OpenAIEmbeddingClient`, deployment-name-to-`model` mapping, explicit Azure routing inputs such as `credential` or `azure_endpoint`, `api_version` configuration after Azure is selected, plus `base_url` guidance for full `.../openai/v1` URLs. If `OPENAI_API_KEY` is also present, the generic clients stay on OpenAI unless you pass explicit Azure routing inputs. If only `AZURE_OPENAI_*` settings are present, Azure environment fallback still works. The old Python `AzureOpenAI*` compatibility classes were removed from the current `agent_framework.azure` namespace, so migrate older code to `agent_framework.openai`. For new Python solutions, we recommend deploying models with Microsoft Foundry and connecting to them with `FoundryChatClient` instead of staying on the Azure OpenAI-specific path. If you need Foundry project endpoints or the Foundry Agent Service instead, see the [Foundry provider page](./microsoft-foundry.md). For a broader migration checklist, see the [Python significant changes guide](../../support/upgrade/python-2026-significant-changes.md).

## Tools

Python Azure OpenAI uses the same `agent_framework.openai` clients as direct OpenAI, so the tool surface is identical. See the [Tools section on the OpenAI provider page](./openai.md#tools) for the full per-client matrix.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [OpenAI Provider](./openai.md)
