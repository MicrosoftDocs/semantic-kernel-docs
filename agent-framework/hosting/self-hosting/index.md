---
title: Self-host Agent Framework applications
description: Build an application-owned server and add one or more Agent Framework protocols.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: article
ms.author: edvan
ms.date: 07/14/2026
ms.service: agent-framework
---

# Self-host Agent Framework applications

:::zone pivot="programming-language-csharp"

> [!NOTE]
> Self-hosting protocol helpers for .NET are coming soon. The hosting model will let your application own its server, state, and protocol integrations.

:::zone-end

:::zone pivot="programming-language-go"

> [!NOTE]
> Self-hosting protocol helpers are not currently available for Go.

:::zone-end

:::zone pivot="programming-language-python"

Self-hosting lets you run an Agent Framework agent or workflow in your own web application, container, service, or runtime. Your application controls routing, identity, authorization, request policy, storage, deployment, and scaling. Add one or more protocol integrations to that server based on the clients you need to support.

Use this option when you need to integrate an agent endpoint with your existing application infrastructure. If you want Microsoft Foundry to run the agent for you, see [Foundry Hosted Agents](../foundry-hosted-agent.md). If you need Azure Functions triggers or durable execution, see [Durable Extension](../../integrations/durable-extension.md).

> [!IMPORTANT]
> `agent-framework-hosting`, `agent-framework-hosting-responses`, and `agent-framework-hosting-telegram` are prerelease Python packages. Install prerelease versions explicitly and review release notes before updating a production deployment.

```bash
pip install --pre agent-framework agent-framework-hosting
```

## What the hosting helpers provide

The generic hosting package provides shared execution state for an application-owned server:

- `AgentState` pairs an agent target with a `SessionStore` and creates sessions when the application selects a new key.
- `SessionStore` stores, retrieves, and deletes sessions by an application-selected ID. Its default store is process-local and has no eviction policy.
- `WorkflowState` resolves a workflow target. Your application owns checkpoint storage and any mapping from a client continuation ID to a checkpoint.

`AgentState` is not a server or protocol registry. Your application selects an authorized session key, resolves the target, and saves the post-run state. It can use the same target and shared application infrastructure for one or several protocol endpoints.

## Bring your own framework or client library

The hosting packages aren't tied to a web framework or client library. The samples use FastAPI and `aiogram` because they provide concise runnable examples, not because the helpers require them.

- For HTTP endpoints, use the routing and request/response APIs of your application framework, such as FastAPI, Starlette, Django, Flask, Azure Functions, or another framework.
- For protocol clients such as Telegram, use any client library that can supply a protocol update and execute the operations produced by the helper.

The application selects its framework and client library; the Agent Framework packages only convert protocol data and manage optional execution state. They don't register routes, authenticate callers, authorize access to state, choose allowed model options, or provide durable storage.

## Add protocols to your server

Choose one or more protocol integrations:

| Protocol | Package and integration | Use it when |
|---|---|---|
| [OpenAI Responses](responses.md) | `agent-framework-hosting-responses` | Clients need an application-owned OpenAI Responses-compatible endpoint. |
| [Telegram](telegram.md) | `agent-framework-hosting-telegram` | Your server receives Telegram updates and sends Bot API operations. |
| [A2A](a2a.md) | `agent-framework-a2a` | Other agents need to discover and invoke your agent through the A2A protocol. |

Each protocol page describes its own conversion helpers or server adapter. A2A uses its native `A2AExecutor`, which maps the A2A context to the agent session; the Responses and Telegram helpers use application-selected session keys with `AgentState`.

## Secure session continuation

Treat every protocol-provided identifier as untrusted input. Before using an ID to load a session, checkpoint, task, or other state:

1. Authenticate the caller.
2. Authorize the caller to access the referenced state.
3. Partition durable state by the authenticated tenant, user, or workspace.
4. Persist session and checkpoint state only after the run or stream has completed.

This self-hosting pattern lets your application implement only the protocol endpoints and policies it needs; it doesn't attempt to implement the complete API surface of every supported protocol.

## Next steps

> [!div class="nextstepaction"]
> [Add the OpenAI Responses protocol](responses.md)

**Go deeper:**

- [Telegram](telegram.md)
- [A2A](a2a.md)
- [Foundry Hosted Agents](../foundry-hosted-agent.md)

:::zone-end
