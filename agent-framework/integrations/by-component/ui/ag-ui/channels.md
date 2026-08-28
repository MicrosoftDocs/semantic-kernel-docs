---
title: Run Agents in Microsoft Teams with CopilotKit Channels
description: Connect an Agent Framework AG-UI endpoint to Microsoft Teams and other messaging platforms with the CopilotKit Channels SDK
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: how-to
ms.author: evmattso
ms.date: 08/28/2026
ms.service: agent-framework
---

# Run Agents in Microsoft Teams with CopilotKit Channels

The agent you exposed over AG-UI doesn't have to live behind a web app: the same endpoint can power a bot in Microsoft Teams. The CopilotKit Channels SDK connects any AG-UI agent to messaging platforms, with threads, tool calls, and rich interactive messages handled natively in the channel. A web frontend and a Teams bot are just two clients of the same AG-UI endpoint.

::: zone pivot="programming-language-csharp"

> [!NOTE]
> Platform setup (creating the Channel, connecting Teams) is maintained in the [CopilotKit Channels documentation for Agent Framework (.NET)](https://docs.copilotkit.ai/teams/ms-agent-dotnet), including [configuring the Channel in Intelligence](https://docs.copilotkit.ai/teams/ms-agent-dotnet/intelligence). This page shows how a channel fits together with an Agent Framework agent.

::: zone-end

::: zone pivot="programming-language-python"

> [!NOTE]
> Platform setup (creating the Channel, connecting Teams) is maintained in the [CopilotKit Channels documentation for Agent Framework (Python)](https://docs.copilotkit.ai/teams/ms-agent-python), including [configuring the Channel in Intelligence](https://docs.copilotkit.ai/teams/ms-agent-python/intelligence). This page shows how a channel fits together with an Agent Framework agent.

::: zone-end

::: zone pivot="programming-language-go"

> [!NOTE]
> Platform setup (creating the Channel, connecting Teams) is maintained in the [CopilotKit Channels documentation](https://docs.copilotkit.ai/teams), including [configuring the Channel in Intelligence](https://docs.copilotkit.ai/teams/intelligence). This page shows how a channel fits together with an Agent Framework agent.

::: zone-end

## How It Fits Together

Nothing about your Agent Framework server changes: it keeps serving the agent over AG-UI exactly as shown in [Getting Started](getting-started.md). What you add is a separate long-running Node process built with [`@copilotkit/channels`](https://www.npmjs.com/package/@copilotkit/channels), connected through CopilotKit Intelligence, a required surface for Channels, by design (a free tier is available). Intelligence holds the platform connection and credentials, receives each platform event, and delivers the turn to your channel process over a persistent gateway connection; your process runs the agent over AG-UI and the reply goes back as native platform content. Platform credentials never enter your process: you configure Teams once in the Intelligence dashboard, and the channel's `name` ties your declaration to that Channel.

```
┌─────────────────┐
│  Microsoft Teams│
└────────┬────────┘
         │ platform events
         ▼
┌────────────────────────────┐
│  CopilotKit Intelligence   │
│  (platform credentials)    │
└────────┬───────────────────┘
         │ persistent gateway connection
         ▼
┌────────────────────────────┐
│  Channel process (Node)    │
│  @copilotkit/channels      │
└────────┬───────────────────┘
         │ HTTP POST + SSE (AG-UI)
         ▼
┌────────────────────────────┐
│  Agent Framework server    │
└────────────────────────────┘
```

## Step 1: Serve the Agent over AG-UI

::: zone pivot="programming-language-csharp"

Serve your agent with `MapAGUIServer` exactly as shown in [Getting Started](getting-started.md):

```csharp
WebApplication app = builder.Build();

// Map the agent to an AG-UI endpoint (HTTP POST + SSE streaming).
app.MapAGUIServer("/", agent);

await app.RunAsync();
```

Run it on the port the channel will connect to:

```bash
dotnet run --urls http://localhost:8888
```

::: zone-end

::: zone pivot="programming-language-python"

Serve your agent with `add_agent_framework_fastapi_endpoint` exactly as shown in [Getting Started](getting-started.md):

```python
from fastapi import FastAPI
from agent_framework_ag_ui import add_agent_framework_fastapi_endpoint

app = FastAPI(title="AG-UI Server")

# Register the AG-UI endpoint
add_agent_framework_fastapi_endpoint(app, agent, "/")

if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="127.0.0.1", port=8888)
```

::: zone-end

::: zone pivot="programming-language-go"

Serve your agent with `aguiprovider.NewJSONHTTPHandler` as shown on the [AG-UI overview](index.md):

```go
import "github.com/microsoft/agent-framework-go/provider/aguiprovider"

mux := http.NewServeMux()
mux.Handle("/", aguiprovider.NewJSONHTTPHandler(myAgent, aguiprovider.HandlerConfig{}))

if err := http.ListenAndServe(":8888", mux); err != nil {
    log.Fatal(err)
}
```

::: zone-end

## Step 2: Point a Channel at It

The channel process is the same Node program regardless of which language serves your agent: Agent Framework endpoints speak standard AG-UI over HTTP, so the channel drives them with `HttpAgent` from `@ag-ui/client`, the same client the [AG-UI Dojo](testing-with-dojo.md) uses for Agent Framework.

In a separate Node project, install the Channels SDK, the CopilotKit runtime, and the AG-UI client:

```bash
npm install @copilotkit/channels @copilotkit/runtime @ag-ui/client
```

Build the agent as a per-thread factory so each conversation gets its own instance keyed by thread:

```typescript
// channel.ts
import { createChannel } from "@copilotkit/channels";
import { CopilotRuntime, CopilotKitIntelligence } from "@copilotkit/runtime/v2";
import { createCopilotNodeListener } from "@copilotkit/runtime/v2/node";
import { HttpAgent } from "@ag-ui/client";

const required = (name: string) => {
  const value = process.env[name];
  if (!value) throw new Error(`Missing ${name}`);
  return value;
};

const channel = createChannel({
  // The Code shown for the Channel you created in Intelligence; Teams
  // credentials live there, never in this process.
  name: required("CHANNEL_CODE"),
  identifyUser: "platform",
  // A fresh agent per conversation, pointed at your AG-UI endpoint.
  agent: (threadId) => {
    const agent = new HttpAgent({ url: "http://localhost:8888/" });
    agent.threadId = threadId;
    return agent;
  },
});

// A mention subscribes the thread and runs the agent; afterwards every message
// in a subscribed thread runs it without needing another mention.
channel.onMention(async ({ thread }) => {
  await thread.subscribe();
  await thread.runAgent();
});
channel.onMessage(async ({ thread }) => {
  if (await thread.isSubscribed()) await thread.runAgent();
});

// The runtime owns the channel's lifecycle; there is no `channel.start()`.
const runtime = new CopilotRuntime({
  agents: {}, // the channel supplies its own agent; no web-facing agents needed
  intelligence: new CopilotKitIntelligence({
    apiKey: required("INTELLIGENCE_API_KEY"), // free tier available
  }),
  channels: [channel],
});

// Creating the listener starts the channel's connection.
const listener = createCopilotNodeListener({ runtime });
await listener.channels.ready({ timeoutMs: 15_000 });
```

## Step 3: Run It

Set `INTELLIGENCE_API_KEY` and `CHANNEL_CODE` (both from the CopilotKit Intelligence dashboard), start your AG-UI server as in Step 1, then start the channel process:

```bash
npx tsx channel.ts
```

Mention the bot in Teams and it runs your agent, streaming the reply back into the thread; the thread stays subscribed, so follow-up messages run without another mention. The agent receives ordinary AG-UI run input and emits ordinary AG-UI events; the platform mechanics stay behind the channel, so the same Agent Framework agent runs unchanged across every platform. Rich messages are written as JSX and rendered to each platform's native format, [Adaptive Cards](/adaptive-cards/) in Teams, so an interactive card degrades gracefully where a platform has no equivalent.

## Microsoft Teams

The Teams connection is configured in CopilotKit Intelligence, which walks you through creating the Teams app and holds its credentials. That leaves two environment variables for the channel process, both from the Intelligence dashboard:

- `INTELLIGENCE_API_KEY`: Authenticates the runtime with Intelligence (free tier available)
- `CHANNEL_CODE`: The Code shown for the Channel you created in Intelligence, passed as `createChannel({ name })`

The channel process holds a persistent connection to the Intelligence gateway, which delivers each turn to it. It needs a long-running host; a serverless request handler cannot own that connection.

::: zone pivot="programming-language-csharp"

For the full walkthrough, see the [CopilotKit Teams guide for Agent Framework (.NET)](https://docs.copilotkit.ai/teams/ms-agent-dotnet/connect).

::: zone-end

::: zone pivot="programming-language-python"

For the full walkthrough, see the [CopilotKit Teams guide for Agent Framework (Python)](https://docs.copilotkit.ai/teams/ms-agent-python/connect).

::: zone-end

::: zone pivot="programming-language-go"

For the full walkthrough, see the [CopilotKit Teams guide](https://docs.copilotkit.ai/teams/connect).

::: zone-end

## Other Platforms

Other messaging platforms, including Discord, WhatsApp, and Telegram, connect the same way: a managed connection configured in Intelligence, with your channel code unchanged.

::: zone pivot="programming-language-csharp"

See the [CopilotKit Channels documentation for Agent Framework (.NET)](https://docs.copilotkit.ai/teams/ms-agent-dotnet) for the current platform list and per-platform setup.

::: zone-end

::: zone pivot="programming-language-python"

See the [CopilotKit Channels documentation for Agent Framework (Python)](https://docs.copilotkit.ai/teams/ms-agent-python) for the current platform list and per-platform setup.

::: zone-end

::: zone pivot="programming-language-go"

See the [CopilotKit Channels documentation](https://docs.copilotkit.ai/teams) for the current platform list and per-platform setup.

::: zone-end

## Message Pipeline and Architecture

Channels keeps a deliberate credential split, useful when your Agent Framework agent holds its own model keys and tools:

- **You keep** the agent logic, model credentials, tools, and the channel process
- **CopilotKit Intelligence holds** the platform credentials, message delivery, registration, health, and reconnects

A turn flows through five stages: a user messages the app, Intelligence receives the platform event using the credentials configured for that channel, a persistent gateway connection delivers the turn to your running channel process, your Agent Framework agent runs and renders a reply, and Intelligence sends it back as native platform content. Platform credentials never enter the agent process.

::: zone pivot="programming-language-csharp"

> [!NOTE]
> By default, interactive actions and per-thread state live in memory and reset when the channel process restarts. Back the channel with a durable action and state store so buttons and per-thread state survive restarts and span multiple instances. See [Persistence and scaling](https://docs.copilotkit.ai/teams/ms-agent-dotnet/persistence-and-scaling).

::: zone-end

::: zone pivot="programming-language-python"

> [!NOTE]
> By default, interactive actions and per-thread state live in memory and reset when the channel process restarts. Back the channel with a durable action and state store so buttons and per-thread state survive restarts and span multiple instances. See [Persistence and scaling](https://docs.copilotkit.ai/teams/ms-agent-python/persistence-and-scaling).

::: zone-end

::: zone pivot="programming-language-go"

> [!NOTE]
> By default, interactive actions and per-thread state live in memory and reset when the channel process restarts. Back the channel with a durable action and state store so buttons and per-thread state survive restarts and span multiple instances. See [Persistence and scaling](https://docs.copilotkit.ai/teams/persistence-and-scaling).

::: zone-end

## Next Steps

- [Getting Started](getting-started.md): Build the AG-UI server this page connects to
- [Human-in-the-Loop](human-in-the-loop.md): Approval workflows on the same endpoint
- [Security Considerations](security-considerations.md): Trust boundaries for AG-UI deployments

::: zone pivot="programming-language-csharp"

For platform setup, rich messages, and approvals in the channel, see the [CopilotKit Teams documentation for Agent Framework (.NET)](https://docs.copilotkit.ai/teams/ms-agent-dotnet).

::: zone-end

::: zone pivot="programming-language-python"

For platform setup, rich messages, and approvals in the channel, see the [CopilotKit Teams documentation for Agent Framework (Python)](https://docs.copilotkit.ai/teams/ms-agent-python).

::: zone-end

::: zone pivot="programming-language-go"

For platform setup, rich messages, and approvals in the channel, see the [CopilotKit Teams documentation](https://docs.copilotkit.ai/teams).

::: zone-end
