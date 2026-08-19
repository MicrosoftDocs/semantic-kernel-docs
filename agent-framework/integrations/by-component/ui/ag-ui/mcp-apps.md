---
title: MCP Apps Compatibility with AG-UI
description: Learn how Agent Framework Python AG-UI endpoints work with CopilotKit's MCPAppsMiddleware for MCP Apps integration
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: article
ms.author: evmattso
ms.date: 08/11/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                         | C# | Python | Go | Notes |
  |---------------------------------|:--:|:------:|:--:|-------|
  | MAF-specific MCP Apps behavior  | ❌ |   ❌   | ❌ | MCP Apps is implemented by external middleware |
  | External middleware setup       | ❌ |   ✅   | ❌ | Python zone documents CopilotKit middleware |
-->

# MCP Apps Compatibility with AG-UI

::: zone pivot="programming-language-csharp"

MAF doesn't provide MCP Apps-specific configuration or runtime behavior. MCP Apps support is implemented by middleware outside the MAF AG-UI endpoint, which continues to receive standard AG-UI requests.

For middleware setup and compatibility requirements, use the documentation for the selected AG-UI client or middleware.

::: zone-end

::: zone pivot="programming-language-python"

Agent Framework Python AG-UI endpoints are compatible with the AG-UI ecosystem's [MCP Apps](https://docs.ag-ui.com/agentic-protocols) feature. MCP Apps allows frontend applications to embed MCP-powered tools and resources alongside your AG-UI agent — no changes needed on the Python side.

## Architecture

MCP Apps support is provided by CopilotKit's TypeScript `MCPAppsMiddleware` (`@ag-ui/mcp-apps-middleware`), which sits between the frontend and your Agent Framework backend:

```
┌─────────────────────────┐
│  Frontend               │
│  (CopilotKit / AG-UI)   │
└────────┬────────────────┘
         │
         ▼
┌─────────────────────────┐
│  CopilotKit Runtime /   │
│  Node.js Proxy          │
│  + MCPAppsMiddleware    │
└────────┬────────────────┘
         │ AG-UI protocol
         ▼
┌─────────────────────────┐
│  Agent Framework        │
│  FastAPI AG-UI Endpoint │
└─────────────────────────┘
```

The middleware layer handles MCP tool discovery, iframe-proxied resource requests, and `ui/resourceUri` resolution. Your Python AG-UI endpoint receives standard AG-UI requests and is unaware of the MCP Apps layer.

## No Python-Side Changes Required

MCP Apps integration is entirely handled by the TypeScript middleware. Your existing `add_agent_framework_fastapi_endpoint()` setup works as-is:

```python
from agent_framework import Agent
from agent_framework.ag_ui import add_agent_framework_fastapi_endpoint
from fastapi import FastAPI

app = FastAPI()
agent = Agent(name="my-agent", instructions="...", client=chat_client)

# This endpoint is MCP Apps-compatible with no additional configuration
add_agent_framework_fastapi_endpoint(app, agent, "/")
```

This approach is consistent with how MCP Apps works with all other AG-UI Python integrations — the MCP Apps layer is always in the TypeScript middleware, not in the Python backend.

## Setting Up the Middleware

To use MCP Apps with your Agent Framework backend, set up a CopilotKit Runtime or Node.js proxy that includes `MCPAppsMiddleware` and points at your Python endpoint:

```typescript
// Example Node.js proxy configuration (TypeScript)
import { MCPAppsMiddleware } from "@ag-ui/mcp-apps-middleware";

const middleware = new MCPAppsMiddleware({
  agents: [
    {
      name: "my-agent",
      url: "http://localhost:8888/",  // Your MAF AG-UI endpoint
    },
  ],
  mcpApps: [
    // MCP app configurations
  ],
});
```

For full setup instructions, see the [CopilotKit MCP Apps documentation](https://docs.copilotkit.ai/built-in-agent/generative-ui/mcp-apps) and the [AG-UI agentic protocols documentation](https://docs.ag-ui.com/agentic-protocols).

## What Is Not in Scope

The following are explicitly **not** part of the Python AG-UI integration:

- **No Python `MCPAppsMiddleware`**: MCP Apps middleware runs in the TypeScript layer only.
- **No FastAPI handling of iframe-proxied MCP requests**: Resource proxying is handled by the Node.js middleware.
- **No Python-side `ui/resourceUri` discovery**: Resource URI resolution is a middleware concern.

If your application doesn't need the MCP Apps middleware layer, your Agent Framework AG-UI endpoint works directly with any AG-UI-compatible client.

## Next steps

> [!div class="nextstepaction"]
> [State Management](./state-management.md)

## Additional Resources

- [AG-UI Agentic Protocols Documentation](https://docs.ag-ui.com/agentic-protocols)
- [CopilotKit MCP Apps Documentation](https://docs.copilotkit.ai/built-in-agent/generative-ui/mcp-apps)
- [Agent Framework GitHub Repository](https://github.com/microsoft/agent-framework)

::: zone-end


::: zone pivot="programming-language-go"

MAF Go doesn't provide MCP Apps-specific configuration or runtime behavior. MCP Apps support is implemented by middleware outside the MAF AG-UI endpoint.

::: zone-end