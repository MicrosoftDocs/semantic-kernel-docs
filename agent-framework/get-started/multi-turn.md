---
title: "Step 3: Multi-Turn Conversations"
description: "Maintain context across multiple exchanges with AgentSession."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: tutorial
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Step 3: Multi-Turn Conversations

Use a session to maintain conversation context so the agent remembers what was said earlier.

:::zone pivot="programming-language-csharp"

Use `AgentSession` to maintain context across multiple calls:

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("Set AZURE_OPENAI_ENDPOINT");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a friendly assistant. Keep your answers brief.", name: "ConversationAgent");

// Create a session to maintain conversation history
AgentSession session = await agent.CreateSessionAsync();

// First turn
Console.WriteLine(await agent.RunAsync("My name is Alice and I love hiking.", session));

// Second turn — the agent remembers the user's name and hobby
Console.WriteLine(await agent.RunAsync("What do you remember about me?", session));
```

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/01-get-started/03_MultiTurn.cs) for the complete runnable file.

:::zone-end

:::zone pivot="programming-language-python"

Use `AgentSession` to maintain context across multiple calls:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/03_multi_turn.py" id="create_agent":::

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/03_multi_turn.py" id="multi_turn" highlight="2,5,9":::

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/01-get-started/03_multi_turn.py) for the complete runnable file.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Step 4: Memory & Persistence](./memory.md)

**Go deeper:**

- [Multi-turn conversations](../agents/conversations/session.md) — advanced conversation patterns
- [Middleware](../agents/middleware/index.md) — intercept and modify agent interactions
