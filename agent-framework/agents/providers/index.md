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

| Provider | Function Tools | Structured Outputs | Code Interpreter | File Search | MCP Tools | Background Responses |
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
> If you use Microsoft Agent Framework to build applications that operate with any third-party servers, agents, code, or non-Azure Direct models ("Third-Party Systems"), you do so at your own risk. Third-Party Systems are Non-Microsoft Products under the Microsoft Product Terms and are governed by their own third-party license terms. You are responsible for any usage and associated costs.
>
> We recommend reviewing all data being shared with and received from Third-Party Systems and being cognizant of third-party practices for handling, sharing, retention and location of data. It is your responsibility to manage whether your data will flow outside of your organization's Azure compliance and geographic boundaries and any related implications, and that appropriate permissions, boundaries and approvals are provisioned.
>
> You are responsible for carefully reviewing and testing applications you build using Microsoft Agent Framework in the context of your specific use cases, and making all appropriate decisions and customizations. This includes implementing your own responsible AI mitigations such as metaprompt, content filters, or other safety systems, and ensuring your applications meet appropriate quality, reliability, security, and trustworthiness standards. See also: [Transparency FAQ](https://github.com/microsoft/agent-framework/blob/main/TRANSPARENCY_FAQS.md)

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
- **[A2A](./agent-to-agent.md)** — Connect to remote agents via the Agent-to-Agent (A2A) protocol.
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
