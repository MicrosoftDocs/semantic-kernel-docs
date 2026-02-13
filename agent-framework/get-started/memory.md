---
title: "Step 4: Memory & Persistence"
description: "Add context providers and persistent memory to your agent."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: tutorial
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Step 4: Memory & Persistence

Add context to your agent so it can remember user preferences, past interactions, or external knowledge.

:::zone pivot="programming-language-csharp"

Set up memory with a custom `ChatHistoryProvider`:

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
    .AsAIAgent(instructions: "You are a friendly assistant. Keep your answers brief.", name: "MemoryAgent");
```

Use a session to persist context across runs:

```csharp
AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine(await agent.RunAsync("Hello! What's the square root of 9?", session));
Console.WriteLine(await agent.RunAsync("My name is Alice", session));
Console.WriteLine(await agent.RunAsync("What is my name?", session));
```

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/01-get-started/04_Memory.cs) for the complete runnable file.

:::zone-end

:::zone pivot="programming-language-python"

Define a context provider that injects additional context into every agent call:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/04_memory.py" id="context_provider" highlight="7,19-22,33-37":::

Create an agent with the context provider:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/04_memory.py" id="create_agent" highlight="13-14":::

> [!NOTE]
> In Python, persistence/memory is handled by history providers. A `BaseHistoryProvider` is also a `BaseContextProvider`, and `InMemoryHistoryProvider` is the built-in local implementation.
> `RawAgent` may auto-add `InMemoryHistoryProvider("memory")` in specific cases (for example, when using a session with no configured context providers and no service-side storage indicators), but this is not guaranteed in all scenarios.
> If you always want local persistence, add an `InMemoryHistoryProvider` explicitly. Also make sure only one history provider has `load_messages=True`, so you don't replay multiple stores into the same invocation.
>
> You can also add an audit store by appending another history provider at the end of `context_providers` with `store_context_messages=True`:
>
> ```python
> from agent_framework import InMemoryHistoryProvider
>
> memory_store = InMemoryHistoryProvider("memory", load_messages=True)
> audit_store = InMemoryHistoryProvider(
>     "audit",
>     load_messages=False,
>     store_context_messages=True,  # include context added by other providers
> )
>
> agent = client.as_agent(
>     name="MemoryAgent",
>     instructions="You are a friendly assistant.",
>     context_providers=[memory, memory_store, audit_store],  # audit store last
> )
> ```

Run it — the agent now has access to the context:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/04_memory.py" id="run_with_memory" highlight="1,4,8,12,15":::

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/01-get-started/04_memory.py) for the complete runnable file.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Step 5: Workflows](./workflows.md)

**Go deeper:**

- [Persistent storage](../agents/conversations/storage.md) — store conversations in databases
- [Chat history](../agents/conversations/context-providers.md) — manage chat history and memory
