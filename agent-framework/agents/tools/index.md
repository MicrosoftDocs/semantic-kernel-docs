---
title: Tools Overview
description: Overview of tool types available in Agent Framework and provider support matrix.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: reference
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Tools Overview

Agent Framework supports many different types of tools that extend agent capabilities. Tools allow agents to interact with external systems, execute code, search data, and more.

## Tool Types

| Tool Type | Description |
|-----------|-------------|
| [Function Tools](./function-tools.md) | Custom code that agents can call during conversations |
| [Tool Approval](./tool-approval.md) | Human-in-the-loop approval for tool invocations |
| [Code Interpreter](./code-interpreter.md) | Execute code in a sandboxed environment |
| [File Search](./file-search.md) | Search through uploaded files |
| [Web Search](./web-search.md) | Search the web for information |
| [Hosted MCP Tools](./hosted-mcp-tools.md) | MCP tools hosted by Microsoft Foundry |
| [Local MCP Tools](./local-mcp-tools.md) | MCP tools running locally or on custom servers |

:::zone pivot="programming-language-csharp"

## Provider Support Matrix

The OpenAI and Azure OpenAI providers each offer multiple client types with different tool capabilities. Azure OpenAI clients mirror their OpenAI equivalents.

| Tool Type | [Chat Completion](../providers/azure-openai.md) | [Responses](../providers/azure-openai.md) | [Assistants](../providers/azure-openai.md) | [Foundry](../providers/azure-ai-foundry.md) | [Anthropic](../providers/anthropic.md) | [Ollama](../providers/ollama.md) | [GitHub Copilot](../providers/github-copilot.md) | [Copilot Studio](../providers/copilot-studio.md) |
|-----------|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| [Function Tools](./function-tools.md) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| [Tool Approval](./tool-approval.md) | ❌ | ✅ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Code Interpreter](./code-interpreter.md) | ❌ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [File Search](./file-search.md) | ❌ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Web Search](./web-search.md) | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| [Hosted MCP Tools](./hosted-mcp-tools.md) | ❌ | ✅ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| [Local MCP Tools](./local-mcp-tools.md) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ |

> [!NOTE]
> The **Chat Completion**, **Responses**, and **Assistants** columns apply to both OpenAI and Azure OpenAI — the Azure variants mirror the same tool support as their OpenAI counterparts.

:::zone-end

:::zone pivot="programming-language-python"

## Provider Support Matrix

The OpenAI and Azure OpenAI providers each offer multiple client types with different tool capabilities. Azure OpenAI clients mirror their OpenAI equivalents.

| Tool Type | [Chat Completion](../providers/azure-openai.md) | [Responses](../providers/azure-openai.md) | [Assistants](../providers/azure-openai.md) | [Foundry](../providers/azure-ai-foundry.md) | [Anthropic](../providers/anthropic.md) | [Claude Agent](../providers/anthropic.md) | [Ollama](../providers/ollama.md) | [GitHub Copilot](../providers/github-copilot.md) |
|-----------|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| [Function Tools](./function-tools.md) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| [Tool Approval](./tool-approval.md) | ❌ | ✅ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Code Interpreter](./code-interpreter.md) | ❌ | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |
| [File Search](./file-search.md) | ❌ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Web Search](./web-search.md) | ✅ | ✅ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| [Image Generation](./code-interpreter.md) | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Hosted MCP Tools](./hosted-mcp-tools.md) | ❌ | ✅ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| [Local MCP Tools](./local-mcp-tools.md) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |

> [!NOTE]
> The **Chat Completion**, **Responses**, and **Assistants** columns apply to both OpenAI and Azure OpenAI — the Azure variants mirror the same tool support as their OpenAI counterparts. Local MCP Tools work with any provider that supports function tools.

:::zone-end

## Using an Agent as a Function Tool

You can use an agent as a function tool for another agent, enabling agent composition and more advanced workflows. The inner agent is converted to a function tool and provided to the outer agent, which can then call it as needed.

:::zone pivot="programming-language-csharp"

Call `.AsAIFunction()` on an `AIAgent` to convert it to a function tool that can be provided to another agent:

```csharp
// Create the inner agent with its own tools
AIAgent weatherAgent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
     .GetChatClient("gpt-4o-mini")
     .AsAIAgent(
        instructions: "You answer questions about the weather.",
        name: "WeatherAgent",
        description: "An agent that answers questions about the weather.",
        tools: [AIFunctionFactory.Create(GetWeather)]);

// Create the main agent and provide the inner agent as a function tool
AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
     .GetChatClient("gpt-4o-mini")
     .AsAIAgent(instructions: "You are a helpful assistant.", tools: [weatherAgent.AsAIFunction()]);

// The main agent can now call the weather agent as a tool
Console.WriteLine(await agent.RunAsync("What is the weather like in Amsterdam?"));
```

:::zone-end

:::zone pivot="programming-language-python"

Call `.as_tool()` on an agent to convert it to a function tool that can be provided to another agent:

```python
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity import AzureCliCredential

# Create the inner agent with its own tools
weather_agent = AzureOpenAIChatClient(credential=AzureCliCredential()).as_agent(
    name="WeatherAgent",
    description="An agent that answers questions about the weather.",
    instructions="You answer questions about the weather.",
    tools=get_weather
)

# Create the main agent and provide the inner agent as a function tool
main_agent = AzureOpenAIChatClient(credential=AzureCliCredential()).as_agent(
    instructions="You are a helpful assistant.",
    tools=weather_agent.as_tool()
)

# The main agent can now call the weather agent as a tool
result = await main_agent.run("What is the weather like in Amsterdam?")
print(result.text)
```

You can also customize the tool name, description, and argument name:

```python
weather_tool = weather_agent.as_tool(
    name="WeatherLookup",
    description="Look up weather information for any location",
    arg_name="query",
    arg_description="The weather query or location"
)
```

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Function Tools](./function-tools.md)
