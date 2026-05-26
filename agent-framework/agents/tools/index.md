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

:::zone pivot="programming-language-csharp"

| Tool Type | Description |
|-----------|-------------|
| [Function Tools](./function-tools.md) | Custom code that agents can call during conversations |
| [Code Interpreter](./code-interpreter.md) | Execute code in a sandboxed environment |
| [File Search](./file-search.md) | Search through uploaded files |
| [Web Search](./web-search.md) | Search the web for information |
| [Hosted MCP Tools](./hosted-mcp-tools.md) | MCP servers invoked by the provider runtime |
| [Local MCP Tools](./local-mcp-tools.md) | MCP servers running locally or on custom hosts |
| [Foundry Toolboxes](../providers/microsoft-foundry.md#toolboxes) | Named, versioned bundles of hosted tool configurations managed in a Foundry project |

:::zone-end

:::zone pivot="programming-language-python"

| Tool Type | Description |
|-----------|-------------|
| [Function Tools](./function-tools.md) | Custom code that agents can call during conversations |
| [Code Interpreter](./code-interpreter.md) | Execute code in a sandboxed environment |
| [File Search](./file-search.md) | Search through uploaded files |
| [Web Search](./web-search.md) | Search the web for information |
| [Hosted MCP Tools](./hosted-mcp-tools.md) | MCP servers invoked by the provider runtime |
| [Local MCP Tools](./local-mcp-tools.md) | MCP servers running locally or on custom hosts |
| [Foundry Toolboxes](../providers/microsoft-foundry.md#toolboxes) | Named, versioned bundles of hosted tool configurations managed in a Foundry project |
| [Image Generation](../providers/microsoft-foundry.md#image-generation) | Hosted image generation on the Foundry / OpenAI Responses runtime |
| [Shell](../providers/openai.md#tools) | Hosted shell execution on the OpenAI Responses runtime — distinct from the GitHub Copilot CLI's built-in shell/file/URL runtime tools |
| [Bing Grounding](../providers/microsoft-foundry.md#bing-grounding) | Web grounding via your own Grounding with Bing Search resource — experimental |
| [Bing Custom Search](../providers/microsoft-foundry.md#bing-custom-search) | Bing grounding restricted to a curated domain list — preview |
| [Azure AI Search](../providers/microsoft-foundry.md#azure-ai-search) | Query an Azure AI Search index through a Foundry connection — experimental |
| [SharePoint](../providers/microsoft-foundry.md#sharepoint) | Ground answers in SharePoint content — preview |
| [Microsoft Fabric](../providers/microsoft-foundry.md#microsoft-fabric) | Query a Fabric data agent — preview |
| [Memory Search](../providers/microsoft-foundry.md#memory-search) | Search a Foundry-managed memory store — preview |
| [Computer Use](../providers/microsoft-foundry.md#computer-use) | Drive a desktop or browser environment — preview |
| [Browser Automation](../providers/microsoft-foundry.md#browser-automation) | Drive a browser via Azure Playwright — preview |
| [Agent-to-Agent (A2A) tool](../providers/microsoft-foundry.md#agent-to-agent-a2a) | Call a remote A2A agent as a tool from a Foundry agent — preview |

> [!NOTE]
> Tools marked **experimental** or **preview** are documented on the relevant provider page and emit an `ExperimentalWarning` the first time they are used in a process.

:::zone-end

## Tool Approval

[Tool Approval](./tool-approval.md) is a framework feature that lets you gate every tool invocation — function tools, hosted tools, MCP tool calls — through a human-in-the-loop decision before the model receives the result. It is handled by the framework's function-invoking chat client in both .NET and Python, so it works with any provider whose client invokes tools locally; it is not a per-provider capability. See the [Tool Approval](./tool-approval.md) page for the full pattern, including how approvals interact with sessions, streaming, and middleware.

:::zone pivot="programming-language-csharp"

## Provider Support Matrix

The OpenAI and Azure OpenAI providers each offer two client types — Responses and Chat Completion — with different tool capabilities. Azure OpenAI clients mirror their OpenAI equivalents. [Copilot Studio](../providers/copilot-studio.md) and [A2A](../providers/agent-to-agent.md) agents run on a remote service so their capabilities are configured on the remote agent rather than through the Agent Framework client — they are not listed in the matrix.

| Tool Type | [Responses](../providers/openai.md#tools) | [Chat Completion](../providers/openai.md#tools) | [Foundry](../providers/microsoft-foundry.md#tools) | [Anthropic](../providers/anthropic.md#tools) | [Ollama](../providers/ollama.md#tools) | [GitHub Copilot](../providers/github-copilot.md#tools) |
|-----------|:---:|:---:|:---:|:---:|:---:|:---:|
| [Function Tools](./function-tools.md) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| [Code Interpreter](./code-interpreter.md) | ✅ | ❌ | ✅ | ❌ | ❌ | ❌ |
| [File Search](./file-search.md) | ✅ | ❌ | ✅ | ❌ | ❌ | ❌ |
| [Web Search](./web-search.md) | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Hosted MCP Tools](./hosted-mcp-tools.md) | ✅ | ❌ | ✅ | ✅ | ❌ | ✅ |
| [Local MCP Tools](./local-mcp-tools.md) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |

> [!NOTE]
> The **Responses** and **Chat Completion** columns apply to both OpenAI and Azure OpenAI — the Azure variants mirror the same tool support as their OpenAI counterparts. The deprecated OpenAI **Assistants** API is no longer documented; for migration guidance see the [Semantic Kernel migration guide](../../migration-guide/from-semantic-kernel/index.md).

:::zone-end

:::zone pivot="programming-language-python"

## Provider Support Matrix

The OpenAI and Azure OpenAI providers each offer multiple client types with different tool capabilities. Azure OpenAI clients mirror their OpenAI equivalents. The Foundry column applies to `FoundryChatClient` — for `FoundryAgent`, the tools are configured on the Foundry agent definition (see [What works and what doesn't with `FoundryAgent`](../providers/microsoft-foundry.md#what-works-and-what-doesnt-with-foundryagent)). [Copilot Studio](../providers/copilot-studio.md) and [A2A](../providers/agent-to-agent.md) agents run on a remote service so their capabilities are configured on the remote agent rather than through the Agent Framework client — they are not listed in the matrix.

| Tool Type | [Responses](../providers/openai.md#tools) | [Chat Completion](../providers/openai.md#tools) | [Foundry](../providers/microsoft-foundry.md#tools) | [Anthropic](../providers/anthropic.md#tools) | [Ollama](../providers/ollama.md#tools) | [Foundry Local](../providers/foundry-local.md#tools) | [GitHub Copilot](../providers/github-copilot.md#tools) |
|-----------|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| [Function Tools](./function-tools.md) | ✅ | ✅ | ✅ | ✅ | ⚠️¹ | ⚠️¹ | ✅ |
| [Code Interpreter](./code-interpreter.md) | ✅ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| [File Search](./file-search.md) | ✅ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Web Search](./web-search.md) | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |
| [Image Generation](../providers/microsoft-foundry.md#image-generation) | ✅ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| Hosted Shell (`get_shell_tool`) | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Built-in shell / file system / URL fetch | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅² |
| [Hosted MCP Tools](./hosted-mcp-tools.md) | ✅ | ❌ | ✅ | ✅ | ❌ | ❌ | ✅ |
| [Local MCP Tools](./local-mcp-tools.md) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| [Foundry Toolboxes](../providers/microsoft-foundry.md#toolboxes) | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Bing Grounding](../providers/microsoft-foundry.md#bing-grounding) (experimental) | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Bing Custom Search](../providers/microsoft-foundry.md#bing-custom-search) (preview) | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Azure AI Search](../providers/microsoft-foundry.md#azure-ai-search) (experimental) | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [SharePoint](../providers/microsoft-foundry.md#sharepoint) (preview) | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Microsoft Fabric](../providers/microsoft-foundry.md#microsoft-fabric) (preview) | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Memory Search](../providers/microsoft-foundry.md#memory-search) (preview) | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Computer Use](../providers/microsoft-foundry.md#computer-use) (preview) | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Browser Automation](../providers/microsoft-foundry.md#browser-automation) (preview) | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Agent-to-Agent (A2A) tool](../providers/microsoft-foundry.md#agent-to-agent-a2a) (preview) | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |

¹ Depends on the chosen local model supporting function calling.
² Built into the GitHub Copilot CLI runtime, gated by a permission handler. Different surface from OpenAI's `get_shell_tool`.

> [!NOTE]
> The **Responses** and **Chat Completion** columns apply to both OpenAI and Azure OpenAI — the Azure variants mirror the same tool support as their OpenAI counterparts. Local MCP Tools work with any provider that supports function tools.

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

## Next steps

> [!div class="nextstepaction"]
> [Function Tools](./function-tools.md)
