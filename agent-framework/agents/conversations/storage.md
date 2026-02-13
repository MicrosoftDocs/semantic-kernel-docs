---
title: Storage
description: Learn built-in storage modes and how to persist session state or plug in external storage.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 02/13/2026
ms.service: agent-framework
---

# Storage

Storage controls where conversation history lives, how much history is loaded, and how reliably sessions can be resumed.

## Built-in storage modes

Agent Framework supports two regular storage modes:

| Mode | What is stored | Typical usage |
|---|---|---|
| Local session state | Full chat history in `AgentSession.state` (for example via `InMemoryHistoryProvider`) | Services that don't require server-side conversation persistence |
| Service-managed storage | Conversation state in the service; `AgentSession.service_session_id` points to it | Services with native persistent conversation support |

## In-memory chat history storage

When a provider doesn't require server-side chat history, Agent Framework keeps history locally in the session and sends relevant messages on each run.

:::zone pivot="programming-language-csharp"

```csharp
AIAgent agent = new OpenAIClient("<your_api_key>")
    .GetChatClient(modelName)
    .AsAIAgent(instructions: "You are a helpful assistant.", name: "Assistant");

AgentSession session = await agent.CreateSessionAsync();
Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate.", session));

// Works when in-memory storage is active.
IList<ChatMessage>? messages = session.GetService<IList<ChatMessage>>();
```

:::zone-end

:::zone pivot="programming-language-python"

```python
from agent_framework import InMemoryHistoryProvider
from agent_framework.openai import OpenAIChatClient

agent = OpenAIChatClient().as_agent(
    name="StorageAgent",
    instructions="You are a helpful assistant.",
    context_providers=[InMemoryHistoryProvider("memory", load_messages=True)],
)

session = agent.create_session()
await agent.run("Remember that I like Italian food.", session=session)
```

:::zone-end

## Reducing in-memory history size

If history grows too large for model limits, apply a reducer.

:::zone pivot="programming-language-csharp"

```csharp
AIAgent agent = new OpenAIClient("<your_api_key>")
    .GetChatClient(modelName)
    .AsAIAgent(new ChatClientAgentOptions
    {
        Name = "Assistant",
        ChatOptions = new() { Instructions = "You are a helpful assistant." },
        ChatHistoryProviderFactory = (ctx, ct) => new ValueTask<ChatHistoryProvider>(
            new InMemoryChatHistoryProvider(
                new MessageCountingChatReducer(20),
                ctx.SerializedState,
                ctx.JsonSerializerOptions,
                InMemoryChatHistoryProvider.ChatReducerTriggerEvent.AfterMessageAdded))
    });
```

:::zone-end

> [!NOTE]
> Reducer configuration applies to in-memory history providers. For service-managed history, reduction behavior is provider/service specific.

## Service-managed storage

When the service manages conversation history, the session stores a remote conversation identifier.

:::zone pivot="programming-language-csharp"

```csharp
AIAgent agent = new OpenAIClient("<your_api_key>")
    .GetOpenAIResponseClient(modelName)
    .AsAIAgent(instructions: "You are a helpful assistant.", name: "Assistant");

AgentSession session = await agent.CreateSessionAsync();
Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate.", session));
```

:::zone-end

:::zone pivot="programming-language-python"

```python
# Rehydrate when the service already has the conversation state.
session = agent.get_session(service_session_id="<service-conversation-id>")
response = await agent.run("Continue this conversation.", session=session)
```

:::zone-end

## Third-party storage pattern

For database/Redis/blob-backed history, implement a custom history provider.

Key guidance:

- Store messages under a session-scoped key.
- Keep returned history within model context limits.
- Persist provider-specific identifiers in `session.state`.
- In Python, only one history provider should use `load_messages=True`.

:::zone pivot="programming-language-python"

```python
from agent_framework.openai import OpenAIChatClient

history = DatabaseHistoryProvider(db_client)
agent = OpenAIChatClient().as_agent(
    name="StorageAgent",
    instructions="You are a helpful assistant.",
    context_providers=[history],
)

session = agent.create_session()
await agent.run("Store this conversation.", session=session)
```

:::zone-end

## Persisting sessions across restarts

Persist the full `AgentSession`, not only message text.

:::zone pivot="programming-language-csharp"

```csharp
JsonElement serialized = agent.SerializeSession(session);
// Store serialized payload in durable storage.
AgentSession resumed = await agent.DeserializeSessionAsync(serialized);
```

:::zone-end

:::zone pivot="programming-language-python"

```python
serialized = session.to_dict()
# Store serialized payload in durable storage.
resumed = AgentSession.from_dict(serialized)
```

:::zone-end

> [!IMPORTANT]
> Treat `AgentSession` as an opaque state object and restore it with the same agent/provider configuration that created it.

:::zone pivot="programming-language-python"
> [!TIP]
> Use an additional audit/eval history provider (`load_messages=False`, `store_context_messages=True`) to capture enriched context plus input/output without affecting primary history loading.
:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Running Agents](../running-agents.md)
