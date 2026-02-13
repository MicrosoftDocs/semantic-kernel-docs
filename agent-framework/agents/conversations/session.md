---
title: Session
description: Learn what AgentSession contains and how to create, restore, and serialize sessions.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 02/13/2026
ms.service: agent-framework
---

# Session

`AgentSession` is the conversation state container used across agent runs.

## What `AgentSession` contains

| Field | Purpose |
|---|---|
| `session_id` | Local unique identifier for this session |
| `service_session_id` | Remote service conversation identifier (when service-managed history is used) |
| `state` | Mutable dictionary shared with context/history providers |

## Built-in usage pattern

:::zone pivot="programming-language-csharp"

```csharp
AgentSession session = await agent.CreateSessionAsync();

var first = await agent.RunAsync("My name is Alice.", session);
var second = await agent.RunAsync("What is my name?", session);
```

:::zone-end

:::zone pivot="programming-language-python"

```python
session = agent.create_session()

first = await agent.run("My name is Alice.", session=session)
second = await agent.run("What is my name?", session=session)
```

:::zone-end

## Creating a session from an existing service conversation ID

Use this when the backing service already has conversation state.

:::zone pivot="programming-language-python"

```python
session = agent.get_session(service_session_id="<service-conversation-id>")
response = await agent.run("Continue this conversation.", session=session)
```

:::zone-end

## Serialization and restoration

:::zone pivot="programming-language-csharp"

```csharp
var serialized = agent.SerializeSession(session);
AgentSession resumed = await agent.DeserializeSessionAsync(serialized);
```

:::zone-end

:::zone pivot="programming-language-python"

```python
serialized = session.to_dict()
resumed = AgentSession.from_dict(serialized)
```

:::zone-end

> [!IMPORTANT]
> Sessions are agent/service-specific. Reusing a session with a different agent configuration or provider can lead to invalid context.

## Next steps

> [!div class="nextstepaction"]
> [Context Providers](./context-providers.md)
