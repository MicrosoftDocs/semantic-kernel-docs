---
title: Self-host Agent Framework applications
description: Build an application-owned server and add one or more Agent Framework protocols.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: article
ms.author: edvan
ms.date: 08/17/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                       | C# | Python | Go | Notes                                  |
  |-------------------------------|:--:|:------:|:--:|----------------------------------------|
  | Self-hosting responsibilities | ✅ |   ✅   | ❌ |                                        |
  | Shared hosting helpers        | ✅ |   ✅   | ❌ | SDK-specific APIs                      |
  | Framework integration         | ✅ |   ✅   | ❌ | ASP.NET Core for C#; framework-neutral Python helpers |
  | Protocol selection            | ✅ |   ✅   | ❌ | Available protocol adapters differ     |
  | Session persistence           | ✅ |   ✅   | ❌ | SDK-specific stores                    |
  | History storage distinction   | ✅ |   ✅   | ❌ | SDK-specific providers                 |
  | Secure session continuation   | ✅ |   ✅   | ❌ | SDK-specific isolation mechanisms      |
-->

# Self-host Agent Framework applications

:::zone pivot="programming-language-csharp"

Self-hosting lets you run an Agent Framework agent or workflow in your own ASP.NET Core application, container, service, or runtime. Your application controls routing, identity, authorization, request policy, storage, deployment, and scaling. Add protocol integrations to the host based on the clients you need to support.

Use this option when you need to integrate an agent endpoint with your existing application infrastructure. If you want Microsoft Foundry to run the agent for you, see [Foundry Hosted Agents](../foundry-hosted-agent.md). If you need Azure Functions triggers or durable execution, see [Durable Extension](../azure-functions.md).

> [!IMPORTANT]
> The .NET hosting packages are prerelease. Install prerelease versions explicitly and review release notes before updating a production deployment.

```dotnetcli
dotnet add package Microsoft.Agents.AI.Hosting --prerelease
```

## What the hosting helpers provide

The `Microsoft.Agents.AI.Hosting` package integrates agents and workflows with the .NET generic host:

- `AddAIAgent` registers a named `AIAgent` with dependency injection.
- `AddWorkflow` registers a named workflow. Chain `AddAsAIAgent` to make the workflow available to protocol integrations through the standard agent interface.
- `IHostedAgentBuilder` configures hosting services associated with that agent.
- `AgentSessionStore` optionally loads and saves `AgentSession` instances by an application- or protocol-supplied continuation ID.

The hosting package isn't an HTTP server or protocol registry. Your application selects the hosted agents and workflows, configures their services, and adds the protocol endpoints it needs.

## Integrate with ASP.NET Core

The shared hosting package uses the .NET generic host and dependency injection. For an HTTP server, create an ASP.NET Core application and add the protocol-specific packages for the endpoints you want to expose. Those packages resolve named `AIAgent` instances from dependency injection and add ASP.NET Core route mappings.

For example, the OpenAI hosting package can expose a configured agent through a Responses endpoint:

```dotnetcli
dotnet add package Microsoft.Agents.AI.Hosting.OpenAI --prerelease
```

```csharp
using Microsoft.Agents.AI.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var hostedAgent = builder.AddAIAgent("weather-agent", (_, _) => agent);

WebApplication app = builder.Build();
app.MapOpenAIResponses(hostedAgent);
app.Run();
```

See [OpenAI-compatible endpoints](openai-endpoints.md) for complete configuration.

Your application remains responsible for its middleware pipeline, authentication, authorization, request validation, allowed model options, and durable storage. A non-HTTP host can use the shared hosting services without adding ASP.NET Core protocol endpoints.

## Add protocols to your server

Choose the protocol integrations your application needs:

| Protocol | Integration |
|---|---|
| [OpenAI-compatible endpoints](openai-endpoints.md) | Chat Completions and Responses-compatible HTTP endpoints |
| [A2A](a2a/server.md) | Agent-to-agent discovery, messaging, and task endpoints |
| [AG-UI](../../integrations/by-component/ui/ag-ui/index.md) | Event-streaming endpoints for web agent applications |

## Persist hosted sessions

`AgentSessionStore` persistence is opt-in for hosting integrations that use it. Without a configured store, those integrations can create a new session for each request but can't recover server-owned session state from an earlier request.

> [!IMPORTANT]
> MAF doesn't include a general-purpose durable session store. For production, provide an `AgentSessionStore` implementation backed by storage appropriate for your application.

Register your durable implementation with dependency injection and pass it to the hosted agent. You can use the in-memory store conditionally during development:

```csharp
builder.Services.AddSingleton<AgentSessionStore, MyAgentSessionStore>();

var hostedAgent = builder.AddAIAgent("weather-agent", (_, _) => agent);

if (builder.Environment.IsDevelopment())
{
    hostedAgent.WithInMemorySessionStore(withIsolation: false);
}
else
{
    hostedAgent.WithSessionStore((services, _) =>
        services.GetRequiredService<AgentSessionStore>());
}
```

In this example, `MyAgentSessionStore` is your application-provided durable implementation. The development branch assumes a local environment with one trusted user and is the only path that disables isolation. The production branch keeps the default isolation behavior; configure an isolation key provider as described in [Secure session continuation](#secure-session-continuation).

`InMemoryAgentSessionStore` loses all sessions when the process exits and doesn't share state across application instances. Implement your own `AgentSessionStore` with persistent storage to retain sessions.

An `AgentSessionStore` implements asynchronous save, get, and delete operations. It receives the owning `AIAgent` and an opaque continuation ID selected by a hosting integration or application-owned route, and it must return an independent `AgentSession` instance from each get operation. Treat the continuation ID as an opaque key in custom stores; how the ID is interpreted is protocol-specific.

A durable implementation has the following structure. Replace each stub with operations for your chosen storage system:

```csharp
public sealed class MyAgentSessionStore : AgentSessionStore
{
    public override ValueTask SaveSessionAsync(
        AIAgent agent,
        string sessionStoreId,
        AgentSession session,
        CancellationToken cancellationToken = default)
    {
        // Persist the session using your storage system.
        throw new NotImplementedException();
    }

    public override ValueTask<AgentSession> GetSessionAsync(
        AIAgent agent,
        string sessionStoreId,
        CancellationToken cancellationToken = default)
    {
        // Restore an independent session, or create one when no state exists.
        throw new NotImplementedException();
    }

    public override ValueTask DeleteSessionAsync(
        AIAgent agent,
        string sessionStoreId,
        CancellationToken cancellationToken = default)
    {
        // Delete the stored session if it exists.
        throw new NotImplementedException();
    }
}
```

Key records by both `agent.Id` and the opaque `sessionStoreId`. `GetSessionAsync` must return an independent session instance on every call; use the owning agent's session serialization APIs when storing serialized state. Persisted sessions can contain sensitive data, so protect them with appropriate access controls and encryption.

`AgentSessionStore` persists the complete `AgentSession` selected by a hosted request, not only conversation messages. Depending on the agent stack, a session can contain a service-managed conversation ID, framework-managed chat history, memory or context-provider state, queued messages, pending approvals, and other state that must survive across runs.

[History providers](../../concepts/agents/conversations/storage.md) control where conversation messages are stored. When history is held in session state, persisting the session also persists that history. An external history provider stores messages separately; the session may retain a reference or related provider state.

## Secure session continuation

A continuation ID identifies a session to resume; it doesn't prove that the caller owns that session. Scope persisted sessions by an authenticated user, tenant, or other authorization boundary before accepting client-supplied IDs. The `IsolationKeyScopedAgentSessionStore` gets an isolation key from `AgentIsolationKeyProvider`, combines it with the protocol continuation ID, and passes the resulting scoped ID to the underlying store. As a result, the same continuation ID under two different isolation keys resolves to two different stored sessions, and a caller can retrieve only sessions saved with that caller's isolation key.

For ASP.NET Core applications that use claims-based authentication, install the prerelease `Microsoft.Agents.AI.Hosting.AspNetCore` package, register the claims-based isolation provider, and keep isolation enabled on the session store:

```dotnetcli
dotnet add package Microsoft.Agents.AI.Hosting.AspNetCore --prerelease
```

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.UseClaimsBasedAgentIsolation();
```

By default, `UseClaimsBasedAgentIsolation` uses the `ClaimTypes.NameIdentifier` claim. Configure another claim only when it is stable and unique across every caller served by the store. The isolation provider doesn't authenticate requests; configure ASP.NET Core authentication and authorization separately. With the default strict isolation behavior, session access fails when the current principal doesn't provide the configured claim.

For a non-HTTP host or another tenancy model, register a custom `AgentIsolationKeyProvider`. The default `WithInMemorySessionStore()` and `WithSessionStore(...)` overloads wrap the configured store in `IsolationKeyScopedAgentSessionStore`.

## Next steps

> [!div class="nextstepaction"]
> [Add an OpenAI-compatible endpoint](openai-endpoints.md)

**Go deeper:**

- [Host agents with A2A](a2a/server.md)
- [Build web agent applications with AG-UI](../../integrations/by-component/ui/ag-ui/index.md)
- [Foundry Hosted Agents](../foundry-hosted-agent.md)

:::zone-end

:::zone pivot="programming-language-go"

> [!NOTE]
> Self-hosting protocol helpers are not currently available for Go.

:::zone-end

:::zone pivot="programming-language-python"

Self-hosting lets you run an Agent Framework agent or workflow in your own web application, container, service, or runtime. Your application controls routing, identity, authorization, request policy, storage, deployment, and scaling. Add one or more protocol integrations to that server based on the clients you need to support.

Use this option when you need to integrate an agent endpoint with your existing application infrastructure. If you want Microsoft Foundry to run the agent for you, see [Foundry Hosted Agents](../foundry-hosted-agent.md). If you need Azure Functions triggers or durable execution, see [Durable Extension](../azure-functions.md).

The design of these packages is such that is allows for maximum flexibility for the developer. This means that if you want to build a host that exposes a agent with the Responses API, and abuse the parameters for other purposes (i.e. map `temperature` to `top_p`), you can do that. If you don't want to store sessions, you can do that, if you want to allow the caller to control the full agent run, you can do that too. We will not get in the way, we provide helpers for the common cases, and make you responsible for the rest, to allow you to build the exact host that you need.

> [!IMPORTANT]
> `agent-framework-hosting`, `agent-framework-hosting-responses`, `agent-framework-hosting-telegram`, `agent-framework-a2a`, `agent-framework-hosting-a2a`, and `agent-framework-hosting-mcp` are prerelease Python packages. Install prerelease versions explicitly and review release notes before updating a production deployment.

```bash
pip install --pre agent-framework-hosting
```

## What the hosting helpers provide

The generic hosting package provides shared execution state for an application-owned server:

- `AgentState` pairs an agent target with a `SessionStore` and creates sessions when the application selects a new key.
- `SessionStore` stores, retrieves, and deletes sessions by an application-selected ID. Its default store is process-local and has no eviction policy.
- `WorkflowState` resolves a workflow target. Your application owns checkpoint storage and any mapping from a client continuation ID to a checkpoint.

`AgentState` is not a server or protocol registry. Your application selects an authorized session key, resolves the target, and saves the post-run state. It can use the same target and shared application infrastructure for one or several protocol endpoints.

## Customize session storage

`SessionStore` is a small async storage class with `get`, `set`, and `delete` methods. The default implementation keeps sessions in process memory. Subclass it and override those methods to store `AgentSession` objects in Redis, a database, blob storage, or another application-owned store, then pass the instance to `AgentState(session_store=...)`.

`SessionStore` and [history providers](../../concepts/agents/conversations/storage.md) persist separate parts of an agent conversation. A session store saves one session object per session ID, including session metadata and provider state. A dedicated `HistoryProvider` stores the conversation separately, typically as one record per message. This separation is recommended for durable hosts because appending individual messages is generally more efficient than rewriting a growing session object after every turn. A history provider is defined per agent, by passing the desired history provider class to the `context_providers` parameter.

> [!NOTE]
> The default history provider: `InMemoryHistoryProvider` is the exception: it stores the full conversation in `AgentSession.state`. When that provider is used, `SessionStore` persists the conversation inside the session object. For longer conversations or production storage, use a dedicated history provider so the session store can remain focused on lightweight session state.

## Bring your own framework or client library

The hosting packages aren't tied to a web framework or client library. The samples use FastAPI and `aiogram` because they provide concise runnable examples, not because the helpers require them.

- For HTTP endpoints, use the routing and request/response APIs of your application framework, such as FastAPI, Starlette, Django, Flask, Azure Functions, or another framework.
- For protocol clients such as Telegram, use any client library that can supply a protocol update and execute the operations produced by the helper.

The application selects its framework and client library; the Agent Framework packages only convert protocol data and manage optional execution state. They don't register routes, authenticate callers, authorize access to state, choose allowed model options, or provide durable storage.

## Add protocols to your server

Choose one or more protocol integrations:

| Protocol | Package and integration |
|---|---|---|
| [OpenAI Responses](responses.md) | `agent-framework-hosting-responses` |
| [Telegram](telegram.md) | `agent-framework-hosting-telegram` |
| [A2A](a2a/index.md) | `agent-framework-a2a` or `agent-framework-hosting-a2a` |
| [MCP](mcp.md) | `agent-framework-hosting-mcp` |

Each protocol page describes its setup. However they are designed to allow you to build a single host with one or more protocols enabled and a callable target; either an agent or a workflow. Since we do not limit you to one web framework, you can choose the one you want, and setup the host with those protocols with ease.

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
- [A2A](a2a/index.md)
- [MCP](mcp.md)
- [Foundry Hosted Agents](../foundry-hosted-agent.md)

:::zone-end
