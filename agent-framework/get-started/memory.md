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

By default, agents will store chat history in an `InMemoryChatHistoryProvider` or in the underlying AI service,
depending on what the underlying service requires.

The following agent uses OpenAI Chat Completion, which neither supports nor requires in-service chat history storage
so therefore automatically creates and uses an `InMemoryChatHistoryProvider`.

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

To use a custom `ChatHistoryProvider` you can pass one to the agent options:

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
    .AsAIAgent(new ChatClientAgentOptions()
    {
        ChatOptions = new() { Instructions = "You are a helpful assistant." },
        ChatHistoryProvider = new CustomChatHistoryProvider()
    });
```

Use a session to share context across runs:

```csharp
AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine(await agent.RunAsync("Hello! What's the square root of 9?", session));
Console.WriteLine(await agent.RunAsync("My name is Alice", session));
Console.WriteLine(await agent.RunAsync("What is my name?", session));
```

> [!TIP]
> See [here](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/GettingStarted/Agents/Agent_Step07_3rdPartyChatHistoryStorage) for a full runnable sample application.

:::zone-end

:::zone pivot="programming-language-python"

Define a context provider that stores user info in session state and injects personalization instructions:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/04_memory.py" id="context_provider" highlight="4,15-20,39":::

Create an agent with the context provider:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/04_memory.py" id="create_agent" highlight="11":::

Run it — the agent now has access to the context:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/04_memory.py" id="run_with_memory" highlight="1,4,8,12,16":::

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/01-get-started/04_memory.py) for the complete runnable file.

> [!NOTE]
> In Python, persistence/memory is handled by Context and History providers. A `BaseHistoryProvider` is also a `BaseContextProvider`, and `InMemoryHistoryProvider` is the built-in local, in-memory implementation.
> `RawAgent` may auto-add `InMemoryHistoryProvider()` in specific cases (for example, when using a session with no configured context providers and no service-side storage indicators), but this is not guaranteed in all scenarios.
> If you always want local persistence, add an `InMemoryHistoryProvider` explicitly. Also make sure only one history provider has `load_messages=True`, so you don't replay multiple stores into the same invocation.
>
> You can also add an audit store by appending another history provider at the end of the list of `context_providers` with `store_context_messages=True`:
>
> ```python
> from agent_framework import InMemoryHistoryProvider
> from agent_framework.mem0 import Mem0ContextProvider
>
> memory_store = InMemoryHistoryProvider(load_messages=True) # add a history provider for persistence across sessions
> agent_memory = Mem0ContextProvider("user-memory", api_key=..., agent_id="my-agent")  # add Mem0 provider for agent memory
> audit_store = InMemoryHistoryProvider(
>     "audit",
>     load_messages=False,
>     store_context_messages=True,  # include context added by other providers
> )
>
> agent = client.as_agent(
>     name="MemoryAgent",
>     instructions="You are a friendly assistant.",
>     context_providers=[memory_store, agent_memory, audit_store],  # audit store last
> )
> ```

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Step 5: Workflows](./workflows.md)

**Go deeper:**

- [Persistent storage](../agents/conversations/storage.md) — store conversations in databases
- [Chat history](../agents/conversations/context-providers.md) — manage chat history and memory
