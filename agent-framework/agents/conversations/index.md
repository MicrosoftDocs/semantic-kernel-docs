---
title: Conversations & Memory overview in Agent Framework
description: Learn the core AgentSession usage pattern and how to navigate sessions, context providers, and storage.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 02/13/2026
ms.service: agent-framework
---

# Conversations & Memory overview

Use `AgentSession` to keep conversation context between invocations.

## Core usage pattern

Most applications follow the same flow:

:::zone pivot="programming-language-csharp"

1. Create a session (`CreateSessionAsync()`)
2. Pass that session to each `RunAsync(...)`
3. Rehydrate from serialized state (`DeserializeSessionAsync(...)`)
4. Continue with a service conversation ID (varies by agent, e.g. `myChatClientAgent.CreateSessionAsync("existing-id")`)

:::zone-end

:::zone pivot="programming-language-python"

1. Create a session (`create_session()`)
2. Pass that session to each `run(...)`
3. Rehydrate by service conversation ID (`get_session(...)`) or from serialized state

:::zone-end

:::zone pivot="programming-language-csharp"

```csharp
// Create and reuse a session
AgentSession session = await agent.CreateSessionAsync();

var first = await agent.RunAsync("My name is Alice.", session);
var second = await agent.RunAsync("What is my name?", session);

// Persist and restore later
var serialized = agent.SerializeSession(session);
AgentSession resumed = await agent.DeserializeSessionAsync(serialized);
```

:::zone-end

:::zone pivot="programming-language-python"

```python
# Create and reuse a session
session = agent.create_session()

first = await agent.run("My name is Alice.", session=session)
second = await agent.run("What is my name?", session=session)

# Rehydrate by service conversation ID when needed
service_session = agent.get_session(service_session_id="<service-conversation-id>")

# Persist and restore later
serialized = session.to_dict()
resumed = AgentSession.from_dict(serialized)
```

:::zone-end

## Guide map

| Page | Focus |
|---|---|
| [Session](./session.md) | `AgentSession` structure and serialization |
| [Context Providers](./context-providers.md) | Built-in and custom context/history provider patterns |
| [Context Compaction](./compaction.md) | Efficiently manage conversation growth |
| [Storage](./storage.md) | Built-in storage modes and external persistence strategies |

## Next steps

> [!div class="nextstepaction"]
> [Session](./session.md)
