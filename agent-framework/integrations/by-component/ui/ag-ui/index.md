---
title: AG-UI Integration with Agent Framework
description: Learn how to integrate Agent Framework with AG-UI protocol for building web-based AI agent applications
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: overview
ms.author: evmattso
ms.date: 08/11/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                        | C# | Python | Go | Notes |
  |--------------------------------|:--:|:------:|:--:|-------|
  | Supported integration behavior | ✅ |   ✅   | ✅ | SDK capabilities differ |
  | Architecture                   | ✅ |   ✅   | ❌ | Not documented for Go |
  | Installation                   | ✅ |   ✅   | ❌ | Go zone links to runnable samples |
-->

# AG-UI Integration with Agent Framework

[AG-UI](https://docs.ag-ui.com/introduction) is a protocol that enables you to build web-based AI agent applications with advanced features like real-time streaming, state management, and interactive UI components. The Agent Framework AG-UI integration provides seamless connectivity between your agents and web clients.

## What is AG-UI?

AG-UI is a standardized protocol for building AI agent interfaces that provides:

- **Remote Agent Hosting**: Deploy AI agents as web services accessible by multiple clients
- **Real-time Streaming**: Stream agent responses using Server-Sent Events (SSE) for immediate feedback
- **Standardized Communication**: Consistent message format for reliable agent interactions
- **Session Management**: Maintain conversation context across multiple requests
- **Advanced Features**: Human-in-the-loop approvals, state synchronization, and custom UI rendering

## When to Use AG-UI

Consider using AG-UI when you need to:

- Build web or mobile applications that interact with AI agents
- Deploy agents as services accessible by multiple concurrent users
- Stream agent responses in real-time to provide immediate user feedback
- Implement approval workflows where users confirm actions before execution
- Synchronize state between client and server for interactive experiences
- Render custom UI components based on agent tool calls

## AG-UI scenarios

AG-UI defines seven showcase scenarios. MAF support varies by SDK; use the language-specific section on this page for the current support level and implementation guidance.

1. **Agentic Chat**: Basic streaming chat with automatic tool calling
2. **Backend Tool Rendering**: Tools executed on backend with results streamed to client
3. **Human in the Loop**: Function approval requests for user confirmation
4. **Agentic Generative UI**: Async tools for long-running operations with progress updates
5. **Tool-based Generative UI**: Custom UI components rendered based on tool calls
6. **Shared State**: Bidirectional state synchronization between client and server
7. **Predictive State Updates**: Stream tool arguments as optimistic state updates

## Build agent UIs with CopilotKit

[CopilotKit](https://copilotkit.ai/) provides rich UI components for building agent user interfaces based on the standard AG-UI protocol. CopilotKit supports streaming chat interfaces, frontend & backend tool calling, human-in-the-loop interactions, generative UI, shared state, and much more. You can see examples of the various agent UI scenarios that CopilotKit supports in the [AG-UI Dojo](https://dojo.ag-ui.com/microsoft-agent-framework-dotnet) sample application.

To connect a CopilotKit React frontend to an Agent Framework AG-UI backend, register your endpoint as an `HttpAgent` in the CopilotKit runtime. This allows CopilotKit's frontend tools to flow through as AG-UI client tools, and all AG-UI features (streaming, approvals, state sync) work automatically.

CopilotKit helps you focus on your agent’s capabilities while delivering a polished user experience without reinventing the wheel.
To learn more about getting started with Microsoft Agent Framework and CopilotKit, see the [Microsoft Agent Framework integration for CopilotKit](https://docs.copilotkit.ai/microsoft-agent-framework) documentation.

::: zone pivot="programming-language-csharp"

## .NET integration

The .NET integration exposes a MAF `AIAgent` as an AG-UI HTTP endpoint. The hosting adapter converts the agent's response stream into AG-UI events; core agent behavior such as tool execution and approval remains part of MAF.

Use the .NET integration to:

- Stream agent text over Server-Sent Events (SSE).
- Surface [backend](./backend-tool-rendering.md) and [frontend](./frontend-tools.md) tool calls as AG-UI events.
- Send [MAF tool approval](./human-in-the-loop.md) requests to the client and return the decision.
- Exchange [client state, state snapshots and deltas, and forwarded properties](./state-management.md).
- [Resume persisted hosted sessions](./getting-started.md#conversation-continuity) using the AG-UI `threadId`.
- Expose [workflows converted to agents](./workflows.md) through the same endpoint.

AG-UI clients decide how to render text, tool, approval, and state events.

## Architecture

The C# hosting package adds an ASP.NET Core endpoint around an ordinary MAF agent:

```text
AG-UI client -- HTTP POST / SSE --> MapAGUIServer --> AIAgent
```

`MapAGUIServer` adapts the AG-UI request to MAF messages and run options. It then converts the agent's streaming response to AG-UI events using the AG-UI .NET SDK.

## Installation

```dotnetcli
dotnet add package Microsoft.Agents.AI.Hosting.AGUI.AspNetCore --prerelease
```

## Next steps

> [!div class="nextstepaction"]
> [Get started with AG-UI](./getting-started.md)

## Related resources

- [Agent Framework overview](../../../../overview/index.md)
- [AG-UI protocol documentation](https://docs.ag-ui.com/introduction)
- [Microsoft Agent Framework repository](https://github.com/microsoft/agent-framework)

::: zone-end

::: zone pivot="programming-language-python"

## AG-UI vs. Direct Agent Usage

While you can run agents directly in your application using Agent Framework's `run` and `run(..., stream=True)` methods, AG-UI provides additional capabilities:

| Feature | Direct Agent Usage | AG-UI Integration |
|---------|-------------------|-------------------|
| Deployment | Embedded in application | Remote service via HTTP |
| Client Access | Single application | Multiple clients (web, mobile) |
| Streaming | In-process async iteration | Server-Sent Events (SSE) |
| State Management | Application-managed | Bidirectional protocol-level sync |
| Thread Context | Application-managed | Protocol-managed thread IDs |
| Approval Workflows | Custom implementation | Built-in protocol support |

## Architecture Overview

The AG-UI integration uses a clean, modular architecture:

```
┌─────────────────┐
│  Web Client     │
│  (Browser/App)  │
└────────┬────────┘
         │ HTTP POST + SSE
         ▼
┌─────────────────────────┐
│  FastAPI Endpoint       │
│  (add_agent_framework_  │
│   fastapi_endpoint)     │
└────────┬────────────────┘
         │
         ▼
┌─────────────────────────┐
│  AgentFrameworkAgent    │
│  (Protocol Wrapper)     │
└────────┬────────────────┘
         │
         ▼
┌─────────────────────────┐
│  Orchestrators          │
│  (Execution Flow Logic) │
└────────┬────────────────┘
         │
         ▼
┌─────────────────────────┐
│  Agent              │
│  (Agent Framework)      │
└────────┬────────────────┘
         │
         ▼
┌─────────────────────────┐
│  Chat Client            │
│  (Azure OpenAI, etc.)   │
└─────────────────────────┘
```

### Key Components

- **FastAPI Endpoint**: HTTP endpoint that handles SSE streaming, configurable keepalive comments, and request routing
- **AgentFrameworkAgent**: Lightweight wrapper that adapts Agent Framework agents to AG-UI protocol
- **Orchestrators**: Handle different execution flows (default, human-in-the-loop, state management)
- **Event Bridge**: Converts Agent Framework events to AG-UI protocol events
- **Message Adapters**: Bidirectional conversion between AG-UI and Agent Framework message formats
- **Confirmation Strategies**: Extensible strategies for domain-specific confirmation messages

## How Agent Framework Translates to AG-UI

Understanding how Agent Framework concepts map to AG-UI helps you build effective integrations:

| Agent Framework Concept | AG-UI Equivalent | Description |
|------------------------|------------------|-------------|
| `Agent` | Agent Endpoint | Each agent becomes an HTTP endpoint |
| `agent.run()` | HTTP POST Request | Client sends messages via HTTP |
| `agent.run(..., stream=True)` | Server-Sent Events | Streaming responses via SSE |
| Agent response updates | AG-UI Events | `TEXT_MESSAGE_CONTENT`, `TOOL_CALL_START`, etc. |
| Function tools (`@tool`) | Backend Tools | Executed on server, results streamed to client |
| Tool approval mode | Human-in-the-Loop | Approval requests/responses via protocol |
| Conversation history | Thread Management | `threadId` maintains context across requests |

## Installation

Install the AG-UI integration package:

```bash
pip install agent-framework-ag-ui --pre
```

This installs both the core agent framework and AG-UI integration components.

## Next Steps

To get started with AG-UI integration:

1. **[Getting Started](getting-started.md)**: Build your first AG-UI server and client
2. **[Backend Tool Rendering](backend-tool-rendering.md)**: Add function tools to your agents
3. **[Workflows](workflows.md)**: Expose multi-agent workflows through AG-UI
4. **[Human-in-the-Loop](human-in-the-loop.md)**: Implement approval workflows
5. **[MCP Apps Compatibility](mcp-apps.md)**: Use MCP Apps with your AG-UI endpoint
6. **[State Management](state-management.md)**: Synchronize state between client and server

## Additional Resources

- [Agent Framework Documentation](../../../../overview/index.md)
- [AG-UI Protocol Documentation](https://docs.ag-ui.com/introduction)
- [AG-UI Dojo App](https://dojo.ag-ui.com/) - Example application demonstrating Agent Framework integration
- [CopilotKit MAF Integration](https://docs.copilotkit.ai/microsoft-agent-framework) - Connect CopilotKit React frontends to AG-UI backends
- [Agent Framework GitHub Repository](https://github.com/microsoft/agent-framework)

::: zone-end

::: zone pivot="programming-language-go"

Go supports AG-UI through `provider/aguiprovider` for both servers and clients.

```go
import "github.com/microsoft/agent-framework-go/provider/aguiprovider"

mux := http.NewServeMux()
mux.Handle("/", aguiprovider.NewJSONHTTPHandler(myAgent, aguiprovider.HandlerConfig{}))

if err := http.ListenAndServe(":8888", mux); err != nil {
    log.Fatal(err)
}
```

> [!TIP]
> See the [AG-UI Go examples](https://github.com/microsoft/agent-framework-go/tree/main/examples/02-agents/agui) for complete server and client samples.

::: zone-end