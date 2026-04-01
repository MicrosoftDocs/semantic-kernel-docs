---
title: Providers Overview
description: Overview of agent providers available in Agent Framework and their supported features.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: reference
ms.author: edvan
ms.date: 03/25/2026
ms.service: agent-framework
---

# Providers Overview

Microsoft Agent Framework supports several types of agents to accommodate different use cases and requirements. All agents are derived from a common base class (`AIAgent` in .NET, `BaseAgent` in Python), which provides a consistent interface for all agent types.

## Provider Comparison

| Provider | Function Tools | Structured Output | Code Interpreter | File Search | MCP Tools | Background Responses |
|----------|:---:|:---:|:---:|:---:|:---:|:---:|
| [Azure OpenAI](./azure-openai.md) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| [OpenAI](./openai.md) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| [Microsoft Foundry](./microsoft-foundry.md) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| [Anthropic](./anthropic.md) | ✅ | ✅ | ✅ | ❌ | ✅ | ❌ |
| [Ollama](./ollama.md) | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| [Foundry Local](./foundry-local.md) | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| [GitHub Copilot](./github-copilot.md) | ✅ | ❌ | ❌ | ❌ | ✅ | ❌ |
| [Copilot Studio](./copilot-studio.md) | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| [Custom](./custom.md) | Varies | Varies | Varies | Varies | Varies | Varies |

> [!IMPORTANT]
> If you use Microsoft Agent Framework to build applications that operate with third-party servers or agents, you do so at your own risk. We recommend reviewing all data being shared with third-party servers or agents.

:::zone pivot="programming-language-csharp"

## Simple agents based on inference services

Agent Framework makes it easy to create simple agents based on many different inference services. Any inference service that provides a `Microsoft.Extensions.AI.IChatClient` implementation can be used to build these agents.

The following providers are available for .NET:

- **[Azure OpenAI](./azure-openai.md)** — Full-featured provider with chat completion, responses API, and tool support.
- **[OpenAI](./openai.md)** — Direct OpenAI API access with chat completion and responses API.
- **[Foundry](./microsoft-foundry.md)** — Persistent server-side agents with managed chat history.
- **[Anthropic](./anthropic.md)** — Claude models with function tools and streaming support.
- **[Ollama](./ollama.md)** — Run open-source models locally.
- **[GitHub Copilot](./github-copilot.md)** — GitHub Copilot SDK integration with shell and file access.
- **[Copilot Studio](./copilot-studio.md)** — Integration with Microsoft Copilot Studio agents.
- **[Custom](./custom.md)** — Build your own provider by implementing the `AIAgent` base class.

:::zone-end

:::zone pivot="programming-language-python"

## Agent providers

Agent Framework supports many different inference services through chat clients. Each provider offers a different set of features:

- **[Azure OpenAI](./azure-openai.md)** — Full-featured provider with Azure identity support.
- **[OpenAI](./openai.md)** — Direct OpenAI API access.
- **[Foundry](./microsoft-foundry.md)** — Microsoft Foundry project inference and service-managed agents.
- **[Foundry Local](./foundry-local.md)** — Run supported Foundry models locally with `FoundryLocalClient` (Python only).
- **[Anthropic](./anthropic.md)** — Claude models with extended thinking and hosted tools support.
- **[Ollama](./ollama.md)** — Run open-source models locally.
- **[GitHub Copilot](./github-copilot.md)** — GitHub Copilot SDK integration.
- **[Copilot Studio](./copilot-studio.md)** — Integration with Microsoft Copilot Studio agents.
- **[Custom](./custom.md)** — Build your own provider by implementing the `BaseAgent` class.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Azure OpenAI Provider](./azure-openai.md)
