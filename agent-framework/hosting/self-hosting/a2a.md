---
title: Self-host A2A agents
description: Expose Agent Framework agents from an application-owned A2A server.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: article
ms.author: edvan
ms.date: 07/14/2026
ms.service: agent-framework
---

# Self-host A2A agents

:::zone pivot="programming-language-csharp"

> [!NOTE]
> Self-hosting A2A support in .NET is coming soon.

:::zone-end

:::zone pivot="programming-language-go"

> [!NOTE]
> Self-hosting A2A support is not currently available for Go.

:::zone-end

:::zone pivot="programming-language-python"

Use `agent-framework-a2a` to expose an Agent Framework agent through the Agent-to-Agent (A2A) protocol. The package's `A2AExecutor` adapts the agent to the A2A server SDK; your application still owns the server, routes, agent card, task storage, authentication, and deployment.

```bash
pip install --pre agent-framework-a2a
```

## Build an A2A server

The A2A SDK request handler hosts an `A2AExecutor` and serves both the agent card and A2A routes. The Starlette server in this sample is one implementation; you can host the A2A SDK integration in another compatible application server.

:::code language="python" source="~/../agent-framework-code/python/samples/04-hosting/a2a/agent_framework_to_a2a.py" range="39-72":::

`A2AExecutor` maps the A2A `context_id` to the Agent Framework session. Unlike the Responses and Telegram helpers, it manages this protocol-specific session mapping directly rather than through `AgentState`.

For a complete runnable server and multi-agent examples, see the [A2A hosting samples](https://github.com/microsoft/agent-framework/tree/main/python/samples/04-hosting/a2a). For A2A clients and protocol capabilities, see [A2A integration](../../integrations/a2a.md).

> [!IMPORTANT]
> The A2A SDK's default task store is in-memory. Replace it with a durable, tenant-aware store in production, and derive ownership from your host's authenticated identity rather than an untrusted protocol value.

## Next steps

> [!div class="nextstepaction"]
> [Learn about A2A integration](../../integrations/a2a.md)

**Go deeper:**

- [Self-hosting overview](index.md)
- [OpenAI Responses](responses.md)
- [Telegram](telegram.md)

:::zone-end
