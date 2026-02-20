---
title: Microsoft Agent Framework Overview
description: "Build AI agents and multi-agent workflows in .NET and Python with Microsoft Agent Framework."
zone_pivot_groups: programming-languages
ms.topic: overview
ms.date: 02/09/2026
ms.service: agent-framework
author: markwallace-microsoft
ms.author: markwallace
ms.reviewer: ssalgado
---

# Microsoft Agent Framework

Agent Framework offers two primary categories of capabilities:

| | Description |
|---|---|
| **[Agents](../agents/index.md)** | Individual agents that use LLMs to process inputs, call [tools](../agents/tools/index.md) and [MCP servers](../agents/tools/hosted-mcp-tools.md), and generate responses. Supports Azure OpenAI, OpenAI, Anthropic, Ollama, and [more](../agents/providers/index.md). |
| **[Workflows](../workflows/index.md)** | Graph-based workflows that connect agents and functions for multi-step tasks with type-safe routing, checkpointing, and human-in-the-loop support. |

The framework also provides foundational building
blocks, including model clients (chat completions and responses), an agent session for state management, context providers for agent memory,
middleware for intercepting agent actions, and MCP clients for tool integration.
Together, these components give you the flexibility and power to build
interactive, robust, and safe AI applications.


## Get started

:::zone pivot="programming-language-csharp"

```dotnetcli
dotnet add package Azure.AI.OpenAI --prerelease
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;

AIAgent agent = new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!),
        new AzureCliCredential())
    .GetChatClient("gpt-4o-mini")
    .AsAIAgent(instructions: "You are a friendly assistant. Keep your answers brief.");

Console.WriteLine(await agent.RunAsync("What is the largest city in France?"));
```

:::zone-end

:::zone pivot="programming-language-python"

```bash
pip install agent-framework --pre
```

```python
    credential = AzureCliCredential()
    client = AzureOpenAIResponsesClient(
        project_endpoint=os.environ["AZURE_AI_PROJECT_ENDPOINT"],
        deployment_name=os.environ["AZURE_OPENAI_RESPONSES_DEPLOYMENT_NAME"],
        credential=credential,
    )

    agent = client.as_agent(
        name="HelloAgent",
        instructions="You are a friendly assistant. Keep your answers brief.",
    )
```

```python
    # Non-streaming: get the complete response at once
    result = await agent.run("What is the largest city in France?")
    print(f"Agent: {result}")
```
:::zone-end

That's it — an agent that calls an LLM and returns a response. From here you can [add tools](../agents/tools/index.md), [multi-turn conversations](../agents/conversations/session.md), [middleware](../agents/middleware/index.md), and [workflows](#when-to-use-agents-vs-workflows) to build production applications.

:::zone pivot="programming-language-python"

> [!NOTE]
> Agent Framework does **not** automatically load `.env` files. To use a `.env` file, call `load_dotenv()` at the start of your application, or set environment variables directly in your shell or IDE.

:::zone-end

> [!div class="nextstepaction"]
> [Get Started — full tutorial](../get-started/your-first-agent.md)

## When to use agents vs workflows

| Use an agent when… | Use a workflow when… |
|---|---|
| The task is open-ended or conversational | The process has well-defined steps |
| You need autonomous tool use and planning | You need explicit control over execution order |
| A single LLM call (possibly with tools) suffices | Multiple agents or functions must coordinate |

_If you can write a function to handle the task, do that instead of using an AI agent._

## Why Agent Framework?

Agent Framework combines AutoGen's simple agent abstractions with Semantic Kernel's enterprise features — session-based state management, type safety, middleware, telemetry — and adds graph-based workflows for explicit multi-agent orchestration.

[Semantic Kernel](https://github.com/microsoft/semantic-kernel)
and [AutoGen](https://github.com/microsoft/autogen) pioneered the concepts of AI agents and multi-agent orchestration.
The Agent Framework is the direct successor, created by the same teams. It combines AutoGen's simple abstractions for single- and multi-agent patterns with Semantic Kernel's enterprise-grade features such as session-based state management, type safety, filters,
telemetry, and extensive model and embedding support. Beyond merging the two,
Agent Framework introduces workflows that give developers explicit control over
multi-agent execution paths, plus a robust state management system
for long-running and human-in-the-loop scenarios.
In short, Agent Framework is the next generation of
both Semantic Kernel and AutoGen.

To learn more about migrating from either Semantic Kernel or AutoGen,
see the [Migration Guide from Semantic Kernel](../migration-guide/from-semantic-kernel/index.md)
and [Migration Guide from AutoGen](../migration-guide/from-autogen/index.md).

Both Semantic Kernel and AutoGen have benefited significantly from the open-source community,
and the same is expected for Agent Framework. Microsoft Agent Framework welcomes contributions and will keep improving with new features and capabilities.

> [!NOTE]
> Microsoft Agent Framework is currently in public preview. Please submit any feedback or issues on the [GitHub repository](https://github.com/microsoft/agent-framework).

> [!IMPORTANT]
> If you use Microsoft Agent Framework to build applications that operate with third-party servers or agents, you do so at your own risk. We recommend reviewing all data being shared with third-party servers or agents and being cognizant of third-party practices for retention and location of data. It is your responsibility to manage whether your data will flow outside of your organization's Azure compliance and geographic boundaries and any related implications.

## Next steps

> [!div class="nextstepaction"]
> [Step 1: Your First Agent](../get-started/your-first-agent.md)

**Go deeper:**

- [Agents overview](../agents/index.md) — architecture, providers, tools
- [Workflows overview](../workflows/index.md) — sequential, concurrent, branching
- [Integrations](../integrations/index.md) — A2A, AG-UI, Azure Functions, M365
