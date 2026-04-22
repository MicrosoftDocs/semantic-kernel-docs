---
title: "Copilot Studio"
description: "Learn how to use Copilot Studio with Agent Framework."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: reference
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Copilot Studio

Copilot Studio integration enables you to use Copilot Studio agents within the Agent Framework.

:::zone pivot="programming-language-csharp"

The following example shows how to create an agent using Copilot Studio:

```csharp
using System;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.CopilotStudio;

// Create a Copilot Studio agent using the IChatClient pattern
// Requires: dotnet add package Microsoft.Agents.AI.CopilotStudio --prerelease
var copilotClient = new CopilotStudioChatClient(
    environmentId: "<your-environment-id>",
    agentIdentifier: "<your-agent-id>",
    credential: new AzureCliCredential());

AIAgent agent = copilotClient.AsAIAgent(
    instructions: "You are a helpful enterprise assistant.");

Console.WriteLine(await agent.RunAsync("What are our company policies on remote work?"));
```

:::zone-end

:::zone pivot="programming-language-python"

> [!NOTE]
> Python support for Copilot Studio agents is available through the `agent-framework-copilotstudio` package.

## Installation

```bash
pip install agent-framework-copilotstudio --pre
```

## Configuration

Set the following environment variables for automatic configuration:

```bash
COPILOTSTUDIOAGENT__ENVIRONMENTID="<your-environment-id>"
COPILOTSTUDIOAGENT__SCHEMANAME="<your-agent-schema-name>"
COPILOTSTUDIOAGENT__AGENTAPPID="<your-client-id>"
COPILOTSTUDIOAGENT__TENANTID="<your-tenant-id>"
```

## Create a Copilot Studio Agent

`CopilotStudioAgent` reads connection settings from environment variables automatically:

```python
import asyncio
from agent_framework.microsoft import CopilotStudioAgent

async def main():
    agent = CopilotStudioAgent()

    result = await agent.run("What are our company policies on remote work?")
    print(result)

asyncio.run(main())
```

## Streaming

```python
async def streaming_example():
    agent = CopilotStudioAgent()

    print("Agent: ", end="", flush=True)
    async for chunk in agent.run("What is the largest city in France?", stream=True):
        if chunk.text:
            print(chunk.text, end="", flush=True)
    print()
```

:::zone-end

:::zone pivot="programming-language-go"

> [!NOTE]
> Go support for this feature is coming soon. See the [Agent Framework Go repository](https://github.com/microsoft/agent-framework-go) for the latest status.

:::zone-end
## Next steps

> [!div class="nextstepaction"]
> [Custom Provider](./custom.md)
