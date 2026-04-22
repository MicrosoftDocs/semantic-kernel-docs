---
title: Session
description: Learn what AgentSession contains and how to create, restore, and serialize sessions.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 04/22/2026
ms.service: agent-framework
---

# Session

`AgentSession` is the conversation state container used across agent runs.

## What `AgentSession` contains

:::zone pivot="programming-language-csharp"

| Field | Purpose |
|---|---|
| `StateBag` | Arbitrary state container for this session |

The C# `AgentSession` is an abstract base class. Concrete implementations (created via `CreateSessionAsync()`) may add additional state e.g. an id for remote chat history storage, when service-managed history is used.

:::zone-end

:::zone pivot="programming-language-python"

| Field | Purpose |
|---|---|
| `session_id` | Local unique identifier for this session |
| `service_session_id` | Remote service conversation identifier (when service-managed history is used) |
| `state` | Mutable dictionary shared with context/history providers |

:::zone-end

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

:::zone pivot="programming-language-csharp"

Create a new session from an existing conversation id varies by agent type. Here are some examples.

When using `ChatClientAgent`

```csharp
AgentSession session = await chatClientAgent.CreateSessionAsync(conversationId);
```

When using an `A2AAgent`

```csharp
AgentSession session = await a2aAgent.CreateSessionAsync(contextId, taskId);
```

:::zone-end

:::zone pivot="programming-language-python"

Use this when the backing service already has conversation state.

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

:::zone pivot="programming-language-go"
## Sessions

The `memory.Session` type provides thread-safe state management for conversations.

### Create and use a session

```go
session, err := a.CreateSession(ctx)
if err != nil {
    panic(err)
}

// Attach the session to runs
resp, _ := a.RunText(ctx, "Hello!", agentopt.Session(session)).Collect()
resp, _ = a.RunText(ctx, "Follow-up question.", agentopt.Session(session)).Collect()
```

### Store and retrieve values

Sessions provide typed key-value storage:

```go
type UserPrefs struct {
    Theme    string `json:"theme"`
    Language string `json:"language"`
}

// Store a value
session.Set("user_prefs", UserPrefs{Theme: "dark", Language: "en"})

// Retrieve a value
var prefs UserPrefs
session.Get("user_prefs", &prefs)

// Delete a value
session.Delete("user_prefs")
```

### Serialize sessions for persistence

```go
// Serialize to JSON
data, err := a.MarshalSession(ctx, session)

// Save to disk, database, etc.
os.WriteFile("session.json", data, 0644)

// Later, restore the session
loaded, _ := os.ReadFile("session.json")
resumedSession, err := a.UnmarshalSession(ctx, loaded)

// Continue the conversation
resp, _ := a.RunText(ctx, "Continue from where we left off.", agentopt.Session(resumedSession)).Collect()
```

> [!TIP]
> See the [persisted conversation sample](https://github.com/microsoft/agent-framework-go/blob/main/examples/02-agents/agents/step06_persisted_conversation/main.go) for a complete example.

:::zone-end
> [!IMPORTANT]
> Sessions are agent/service-specific. Reusing a session with a different agent configuration or provider can lead to invalid context.

## Next steps

> [!div class="nextstepaction"]
> [Context Providers](./context-providers.md)
