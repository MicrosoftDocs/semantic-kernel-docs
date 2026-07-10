---
title: Session
description: Learn what AgentSession contains and how to create, restore, and serialize sessions.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: article
ms.author: edvan
ms.date: 02/13/2026
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
| `service_session_id` | Remote service session identifier, such as a conversation or response ID, when service-managed history is used |
| `state` | Mutable dictionary shared with context/history providers |

:::zone-end

## Service session ID scoping

When service-managed history is used, a session can contain a service-issued session identifier. For example, OpenAI Responses may use a `resp_*` response ID as `previous_response_id`, and the OpenAI Conversations API may use a `conv_*` conversation ID as the conversation.

OpenAI scopes these IDs to the backing API key or project by default. This is usually enough when that key or project already matches the application boundary, such as a single-user app or a separate key/project per tenant. The risky hosted pattern is using one backing key or project for multiple end users, echoing raw service-side IDs to clients, and accepting those IDs back without checking ownership. In hosted or multi-user apps that reuse one backing key or project, do not treat `service_session_id`, `previous_response_id`, or `conversation`/`conversation_id` as end-user authorization boundaries. Store service-side IDs in trusted application storage, map client-visible session IDs to those service-side IDs, and verify the authenticated user or tenant before resuming a conversation.

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

In hosted apps, resolve `<service-conversation-id>` from application-owned storage after checking the current user or tenant. Avoid accepting raw service-side IDs from a client unless you first verify that the caller owns the conversation.

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
> Sessions are agent/service-specific. Reusing a session with a different agent configuration or provider can lead to invalid context. If the serialized session contains a service-side session ID, restore it only for the application user or tenant that owns that ID.

## Next steps

> [!div class="nextstepaction"]
> [Context Providers](./context-providers.md)
