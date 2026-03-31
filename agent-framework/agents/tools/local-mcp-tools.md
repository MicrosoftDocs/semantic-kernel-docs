---
title: Using MCP Tools
description: Using MCP tools with agents
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: reference
ms.author: markwallace
ms.date: 03/30/2026
ms.service: agent-framework
---

# Using MCP tools with Agents

Model Context Protocol is an open standard that defines how applications provide tools and contextual data to large language models (LLMs). It enables consistent, scalable integration of external tools into model workflows.

Microsoft Agent Framework supports integration with Model Context Protocol (MCP) servers, allowing your agents to access external tools and services. This guide shows how to connect to an MCP server and use its tools within your agent.

## Considerations for using third-party MCP servers

Your use of Model Context Protocol servers is subject to the terms between you and the service provider. When you connect to a non-Microsoft service, some of your data (such as prompt content) is passed to the non-Microsoft service, or your application might receive data from the non-Microsoft service. You're responsible for your use of non-Microsoft services and data, along with any charges associated with that use.

The remote MCP servers that you decide to use with the MCP tool described in this article were created by third parties, not Microsoft. Microsoft hasn't tested or verified these servers. Microsoft has no responsibility to you or others in relation to your use of any remote MCP servers.

We recommend that you carefully review and track what MCP servers you add to your Agent Framework based applications. We also recommend that you rely on servers hosted by trusted service providers themselves rather than proxies.

The MCP tool allows you to pass custom headers, such as authentication keys or schemas, that a remote MCP server might need. We recommend that you review all data that's shared with remote MCP servers and that you log the data for auditing purposes. Be cognizant of non-Microsoft practices for retention and location of data.

> [!IMPORTANT]
> You can specify headers only by including them in tool_resources at each run. In this way, you can put API keys, OAuth access tokens, or other credentials directly in your request. Headers that you pass in are available only for the current run and aren't persisted.

For more information on MCP security, see:

- [Security Best Practices](https://modelcontextprotocol.io/specification/draft/basic/security_best_practices) on the Model Context Protocol website.
- [Understanding and mitigating security risks in MCP implementations](https://techcommunity.microsoft.com/blog/microsoft-security-blog/understanding-and-mitigating-security-risks-in-mcp-implementations/4404667) in the Microsoft Security Community Blog.

::: zone pivot="programming-language-csharp"

The .NET version of Agent Framework can be used together with the [official MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk) to allow your agent to call MCP tools.

The following sample shows how to:

1. Set up and MCP server
1. Retrieve the list of available tools from the MCP Server
1. Convert the MCP tools to `AIFunction`'s so they can be added to an agent
1. Invoke the tools from an agent using function calling

### Setting Up an MCP Client

First, create an MCP client that connects to your desired MCP server:

```csharp
// Create an MCPClient for the GitHub server
await using var mcpClient = await McpClientFactory.CreateAsync(new StdioClientTransport(new()
{
    Name = "MCPServer",
    Command = "npx",
    Arguments = ["-y", "--verbose", "@modelcontextprotocol/server-github"],
}));
```

In this example:

- **Name**: A friendly name for your MCP server connection
- **Command**: The executable to run the MCP server (here using npx to run a Node.js package)
- **Arguments**: Command-line arguments passed to the MCP server

### Retrieving Available Tools

Once connected, retrieve the list of tools available from the MCP server:

```csharp
// Retrieve the list of tools available on the GitHub server
var mcpTools = await mcpClient.ListToolsAsync().ConfigureAwait(false);
```

The `ListToolsAsync()` method returns a collection of tools that the MCP server exposes. These tools are automatically converted to AITool objects that can be used by your agent.

### Create an Agent with MCP Tools

Create your agent and provide the MCP tools during initialization:

```csharp
AIAgent agent = new AzureOpenAIClient(
    new Uri(endpoint),
    new DefaultAzureCredential())
     .GetChatClient(deploymentName)
     .AsAIAgent(
         instructions: "You answer questions related to GitHub repositories only.",
         tools: [.. mcpTools.Cast<AITool>()]);

```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

Key points:

- **Instructions**: Provide clear instructions that align with the capabilities of your MCP tools
- **Tools**: Cast the MCP tools to `AITool` objects and spread them into the tools array
- The agent will automatically have access to all tools provided by the MCP server

### Using the Agent

Once configured, your agent can automatically use the MCP tools to fulfill user requests:

```csharp
// Invoke the agent and output the text result
Console.WriteLine(await agent.RunAsync("Summarize the last four commits to the microsoft/semantic-kernel repository?"));
```

The agent will:

1. Analyze the user's request
1. Determine which MCP tools are needed
1. Call the appropriate tools through the MCP server
1. Synthesize the results into a coherent response

### Environment Configuration

Make sure to set up the required environment variables:

```csharp
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ??
    throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";
```

### Resource Management

Always properly dispose of MCP client resources:

```csharp
await using var mcpClient = await McpClientFactory.CreateAsync(...);
```

Using `await using` ensures the MCP client connection is properly closed when it goes out of scope.

### Common MCP Servers

Popular MCP servers include:

- `@modelcontextprotocol/server-github`: Access GitHub repositories and data
- `@modelcontextprotocol/server-filesystem`: File system operations
- `@modelcontextprotocol/server-sqlite`: SQLite database access

Each server provides different tools and capabilities that extend your agent's functionality.
This integration allows your agents to seamlessly access external data and services while maintaining the security and standardization benefits of the Model Context Protocol.

The full source code and instructions to run this sample is available at <https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/02-agents/ModelContextProtocol/Agent_MCP_Server>.

> [!TIP]
> See the [.NET samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples) for complete runnable examples.

::: zone-end
::: zone pivot="programming-language-python"

This allows your agents to access external tools and services seamlessly.

> [!NOTE]
> On minimal Python installs, MCP support might need to be installed manually. Install `mcp --pre` to use `MCPStdioTool`, `MCPStreamableHTTPTool`, or `Agent.as_mcp_server()`. Install `mcp[ws] --pre` if you also need `MCPWebsocketTool`.

## MCP Tool Types

The Agent Framework supports three types of MCP connections:

### MCPStdioTool - Local MCP Servers

Use `MCPStdioTool` to connect to MCP servers that run as local processes using standard input/output:

```python
import asyncio
from agent_framework import Agent, MCPStdioTool
from agent_framework.openai import OpenAIChatClient

async def local_mcp_example():
    """Example using a local MCP server via stdio."""
    async with (
        MCPStdioTool(
            name="calculator",
            command="uvx",
            args=["mcp-server-calculator"]
        ) as mcp_server,
        Agent(
            chat_client=OpenAIChatClient(),
            name="MathAgent",
            instructions="You are a helpful math assistant that can solve calculations.",
        ) as agent,
    ):
        result = await agent.run(
            "What is 15 * 23 + 45?",
            tools=mcp_server
        )
        print(result)

if __name__ == "__main__":
    asyncio.run(local_mcp_example())
```

### MCPStreamableHTTPTool - HTTP/SSE MCP Servers

Use `MCPStreamableHTTPTool` to connect to MCP servers over HTTP with Server-Sent Events:

```python
import asyncio
from agent_framework import Agent, MCPStreamableHTTPTool
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def http_mcp_example():
    """Example using an HTTP-based MCP server."""
    async with (
        AzureCliCredential() as credential,
        MCPStreamableHTTPTool(
            name="Microsoft Learn MCP",
            url="https://learn.microsoft.com/api/mcp",
            headers={"Authorization": "Bearer your-token"},
        ) as mcp_server,
        Agent(
            chat_client=AzureAIAgentClient(credential=credential),
            name="DocsAgent",
            instructions="You help with Microsoft documentation questions.",
        ) as agent,
    ):
        result = await agent.run(
            "How to create an Azure storage account using az cli?",
            tools=mcp_server
        )
        print(result)

if __name__ == "__main__":
    asyncio.run(http_mcp_example())
```

### MCPWebsocketTool - WebSocket MCP Servers

Use `MCPWebsocketTool` to connect to MCP servers over WebSocket connections:

```python
import asyncio
from agent_framework import Agent, MCPWebsocketTool
from agent_framework.openai import OpenAIChatClient

async def websocket_mcp_example():
    """Example using a WebSocket-based MCP server."""
    async with (
        MCPWebsocketTool(
            name="realtime-data",
            url="wss://api.example.com/mcp",
        ) as mcp_server,
        Agent(
            chat_client=OpenAIChatClient(),
            name="DataAgent",
            instructions="You provide real-time data insights.",
        ) as agent,
    ):
        result = await agent.run(
            "What is the current market status?",
            tools=mcp_server
        )
        print(result)

if __name__ == "__main__":
    asyncio.run(websocket_mcp_example())
```

## Popular MCP Servers

Common MCP servers you can use with Python Agent Framework:

- **Calculator**: `uvx mcp-server-calculator` - Mathematical computations
- **Filesystem**: `uvx mcp-server-filesystem` - File system operations
- **GitHub**: `npx @modelcontextprotocol/server-github` - GitHub repository access
- **SQLite**: `uvx mcp-server-sqlite` - Database operations

Each server provides different tools and capabilities that extend your agent's functionality while maintaining the security and standardization benefits of the Model Context Protocol.

### Complete example

```python
# Copyright (c) Microsoft. All rights reserved.

import os

from agent_framework import Agent, MCPStreamableHTTPTool
from agent_framework.openai import OpenAIChatClient
from httpx import AsyncClient

"""
MCP Authentication Example

This example demonstrates how to authenticate with MCP servers using API key headers.

For more authentication examples including OAuth 2.0 flows, see:
- https://github.com/modelcontextprotocol/python-sdk/tree/main/examples/clients/simple-auth-client
- https://github.com/modelcontextprotocol/python-sdk/tree/main/examples/servers/simple-auth
"""


async def api_key_auth_example() -> None:
    """Example of using API key authentication with MCP server."""
    # Configuration
    mcp_server_url = os.getenv("MCP_SERVER_URL", "your-mcp-server-url")
    api_key = os.getenv("MCP_API_KEY")

    # Create authentication headers
    # Common patterns:
    # - Bearer token: "Authorization": f"Bearer {api_key}"
    # - API key header: "X-API-Key": api_key
    # - Custom header: "Authorization": f"ApiKey {api_key}"
    auth_headers = {
        "Authorization": f"Bearer {api_key}",
    }

    # Create HTTP client with authentication headers
    http_client = AsyncClient(headers=auth_headers)

    # Create MCP tool with the configured HTTP client
    async with (
        MCPStreamableHTTPTool(
            name="MCP tool",
            description="MCP tool description",
            url=mcp_server_url,
            http_client=http_client,  # Pass HTTP client with authentication headers
        ) as mcp_tool,
        Agent(
            client=OpenAIChatClient(),
            name="Agent",
            instructions="You are a helpful assistant.",
            tools=mcp_tool,
        ) as agent,
    ):
        query = "What tools are available to you?"
        print(f"User: {query}")
        result = await agent.run(query)
        print(f"Agent: {result.text}")
```

::: zone-end

## Exposing an Agent as an MCP Server

You can expose an agent as an MCP server, allowing it to be used as a tool by any MCP-compatible client (such as VS Code GitHub Copilot Agents or other agents). The agent's name and description become the MCP server metadata.

::: zone pivot="programming-language-csharp"

Wrap the agent in a function tool using `.AsAIFunction()`, create an `McpServerTool`, and register it with an MCP server:

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

// Create the agent
AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o-mini")
        .AsAIAgent(instructions: "You are good at telling jokes.", name: "Joker");

// Convert the agent to an MCP tool
McpServerTool tool = McpServerTool.Create(agent.AsAIFunction());

// Set up the MCP server over stdio
HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools([tool]);

await builder.Build().RunAsync();
```

Install the required NuGet packages:

```dotnetcli
dotnet add package Microsoft.Extensions.Hosting --prerelease
dotnet add package ModelContextProtocol --prerelease
```

::: zone-end
::: zone pivot="programming-language-python"

Call `.as_mcp_server()` on an agent to expose it as an MCP server:

> [!NOTE]
> Python `agent.as_mcp_server()` also depends on the optional `mcp` package. If you use a slim/core-based install, run `pip install mcp --pre` first.

```python
from agent_framework.openai import OpenAIChatClient
from typing import Annotated

def get_specials() -> Annotated[str, "Returns the specials from the menu."]:
    return "Special Soup: Clam Chowder, Special Salad: Cobb Salad"

# Create an agent with tools
agent = OpenAIChatClient().as_agent(
    name="RestaurantAgent",
    description="Answer questions about the menu.",
    tools=[get_specials],
)

# Expose the agent as an MCP server
server = agent.as_mcp_server()
```

Set up the MCP server to listen over standard input/output:

```python
import anyio
from mcp.server.stdio import stdio_server

async def run():
    async with stdio_server() as (read_stream, write_stream):
        await server.run(read_stream, write_stream, server.create_initialization_options())

if __name__ == "__main__":
    anyio.run(run)
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Conversations & Memory](../conversations/index.md)
