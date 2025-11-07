---
title: AG-UI Integration with Agent Framework
description: Learn how to integrate Agent Framework with AG-UI protocol for building web-based AI agent applications
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: overview
ms.author: evmattso
ms.date: 11/07/2025
ms.service: agent-framework
---

# AG-UI Integration with Agent Framework

[AG-UI](https://docs.ag-ui.com/introduction) is a protocol that enables you to build web-based AI agent applications with advanced features like real-time streaming, state management, and interactive UI components. The Agent Framework AG-UI integration provides seamless connectivity between your agents and web clients.

## What is AG-UI?

AG-UI is a standardized protocol for building AI agent interfaces that provides:

- **Remote Agent Hosting**: Deploy AI agents as web services accessible by multiple clients
- **Real-time Streaming**: Stream agent responses using Server-Sent Events (SSE) for immediate feedback
- **Standardized Communication**: Consistent message format for reliable agent interactions
- **Thread Management**: Maintain conversation context across multiple requests
- **Advanced Features**: Human-in-the-loop approvals, state synchronization, and custom UI rendering

## When to Use AG-UI

Consider using AG-UI when you need to:

- Build web or mobile applications that interact with AI agents
- Deploy agents as services accessible by multiple concurrent users
- Stream agent responses in real-time to provide immediate user feedback
- Implement approval workflows where users confirm actions before execution
- Synchronize state between client and server for interactive experiences
- Render custom UI components based on agent tool calls

## Supported Features

The Agent Framework AG-UI integration supports all 7 AG-UI protocol features:

1. **Agentic Chat**: Basic streaming chat with automatic tool calling
2. **Backend Tool Rendering**: Tools executed on backend with results streamed to client
3. **Human in the Loop**: Function approval requests for user confirmation
4. **Agentic Generative UI**: Async tools for long-running operations with progress updates
5. **Tool-based Generative UI**: Custom UI components rendered based on tool calls
6. **Shared State**: Bidirectional state synchronization between client and server
7. **Predictive State Updates**: Stream tool arguments as optimistic state updates

::: zone pivot="programming-language-csharp"

Coming soon.

::: zone-end

::: zone pivot="programming-language-python"

## AG-UI vs. Direct Agent Usage

While you can run agents directly in your application using Agent Framework's `run` and `run_streaming` methods, AG-UI provides additional capabilities:

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
│  ChatAgent              │
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

- **FastAPI Endpoint**: HTTP endpoint that handles SSE streaming and request routing
- **AgentFrameworkAgent**: Lightweight wrapper that adapts Agent Framework agents to AG-UI protocol
- **Orchestrators**: Handle different execution flows (default, human-in-the-loop, state management)
- **Event Bridge**: Converts Agent Framework events to AG-UI protocol events
- **Message Adapters**: Bidirectional conversion between AG-UI and Agent Framework message formats
- **Confirmation Strategies**: Extensible strategies for domain-specific confirmation messages

## How Agent Framework Translates to AG-UI

Understanding how Agent Framework concepts map to AG-UI helps you build effective integrations:

| Agent Framework Concept | AG-UI Equivalent | Description |
|------------------------|------------------|-------------|
| `ChatAgent` | Agent Endpoint | Each agent becomes an HTTP endpoint |
| `agent.run()` | HTTP POST Request | Client sends messages via HTTP |
| `agent.run_streaming()` | Server-Sent Events | Streaming responses via SSE |
| Agent response updates | AG-UI Events | `TEXT_MESSAGE_CONTENT`, `TOOL_CALL_START`, etc. |
| Function tools (`@ai_function`) | Backend Tools | Executed on server, results streamed to client |
| Tool approval mode | Human-in-the-Loop | Approval requests/responses via protocol |
| Conversation history | Thread Management | `threadId` maintains context across requests |

## Installation

Install the AG-UI integration package:

```bash
pip install agent-framework-ag-ui
```

This installs both the core agent framework and AG-UI integration components.

## Next Steps

To get started with AG-UI integration:

1. **[Getting Started](getting-started.md)**: Build your first AG-UI server and client
2. **[Backend Tool Rendering](backend-tool-rendering.md)**: Add function tools to your agents
3. **[Human-in-the-Loop](human-in-the-loop.md)**: Implement approval workflows
4. **[State Management](state-management.md)**: Synchronize state between client and server

## Additional Resources

- [Agent Framework Documentation](../../overview/agent-framework-overview.md)
- [AG-UI Protocol Documentation](https://docs.ag-ui.com/introduction)
- [AG-UI Dojo App](https://github.com/ag-oss/ag-ui/tree/main/apps/dojo) - Example application demonstrating Agent Framework integration
- [Agent Framework GitHub Repository](https://github.com/microsoft/agent-framework)

::: zone-end
