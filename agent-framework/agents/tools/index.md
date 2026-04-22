---
title: Tools Overview
description: Overview of tool types available in Agent Framework and provider support matrix.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: reference
ms.author: edvan
ms.date: 04/22/2026
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

| Tool Type | [Chat Completion](../providers/azure-openai.md) | [Responses](../providers/azure-openai.md) | [Assistants](../providers/azure-openai.md) | [Foundry](../providers/microsoft-foundry.md) | [Anthropic](../providers/anthropic.md) | [Ollama](../providers/ollama.md) | [GitHub Copilot](../providers/github-copilot.md) | [Copilot Studio](../providers/copilot-studio.md) |
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

| Tool Type | [Chat Completion](../providers/azure-openai.md) | [Responses](../providers/azure-openai.md) | [Assistants](../providers/azure-openai.md) | [Foundry](../providers/microsoft-foundry.md) | [Anthropic](../providers/anthropic.md) | [Claude Agent](../providers/anthropic.md) | [Ollama](../providers/ollama.md) | [GitHub Copilot](../providers/github-copilot.md) |
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
AIAgent weatherAgent = new AIProjectClient(
    new Uri("<your-foundry-project-endpoint>"),
    new DefaultAzureCredential())
     .AsAIAgent(
        model: "gpt-4o-mini",
        instructions: "You answer questions about the weather.",
        name: "WeatherAgent",
        description: "An agent that answers questions about the weather.",
        tools: [AIFunctionFactory.Create(GetWeather)]);

// Create the main agent and provide the inner agent as a function tool
AIAgent agent = new AIProjectClient(
    new Uri("<your-foundry-project-endpoint>"),
    new DefaultAzureCredential())
     .AsAIAgent(
        model: "gpt-4o-mini",
        instructions: "You are a helpful assistant.",
        tools: [weatherAgent.AsAIFunction()]);

// The main agent can now call the weather agent as a tool
Console.WriteLine(await agent.RunAsync("What is the weather like in Amsterdam?"));
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

:::zone-end

:::zone pivot="programming-language-python"

Call `.as_tool()` on an agent to convert it to a function tool that can be provided to another agent:

```python
import os
from agent_framework.openai import OpenAIChatCompletionClient
from azure.identity import AzureCliCredential

# Create the inner agent with its own tools
weather_agent = OpenAIChatCompletionClient(
    model=os.environ["AZURE_OPENAI_CHAT_COMPLETION_MODEL"],
    azure_endpoint=os.environ["AZURE_OPENAI_ENDPOINT"],
    api_version=os.getenv("AZURE_OPENAI_API_VERSION"),
    credential=AzureCliCredential(),
).as_agent(
    name="WeatherAgent",
    description="An agent that answers questions about the weather.",
    instructions="You answer questions about the weather.",
    tools=get_weather
)

# Create the main agent and provide the inner agent as a function tool
main_agent = OpenAIChatCompletionClient(
    model=os.environ["AZURE_OPENAI_CHAT_COMPLETION_MODEL"],
    azure_endpoint=os.environ["AZURE_OPENAI_ENDPOINT"],
    api_version=os.getenv("AZURE_OPENAI_API_VERSION"),
    credential=AzureCliCredential(),
).as_agent(
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

:::zone pivot="programming-language-go"
## Tools overview

Go agents support several tool types through the `tool` package:

| Tool Type | Package | Description |
|---|---|---|
| Function tools | `tool/functool` | Custom Go functions the agent can call |
| MCP tools | `tool/mcptool` | Tools from Model Context Protocol servers |
| Hosted tools | `tool/hostedtool` | Service-side tools (web search, file search, code interpreter) |

### Tool interface

All tools implement the `tool.Tool` interface:

```go
type Tool interface {
    Name() string
    Description() string
}
```

Function tools additionally implement `tool.FuncTool`:

```go
type FuncTool interface {
    Tool
    Schema() map[string]any
    ReturnSchema() map[string]any
    Call(ctx Context, arguments string) (any, error)
}
```

### Registering tools

Pass tools to the agent via `agent.Config.Tools`:

```go
a := openaichatagent.New(client, openaichatagent.Config{
    Model: deployment,
    Config: agent.Config{
        Instructions: "You are a helpful assistant.",
        Tools: []tool.Tool{weatherTool, calculatorTool},
    },
})
```

Or add tools per-run:

```go
resp, err := a.RunText(ctx, "What's the weather?", agentopt.Tool(weatherTool)).Collect()
```

:::zone-end
## Next steps

> [!div class="nextstepaction"]
> [Function Tools](./function-tools.md)
