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
> Python support for Copilot Studio agents is coming soon.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Custom Provider](./custom.md)
