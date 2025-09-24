---
title: Using MCP Tools
description: Using MCP tools with agents
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: reference
ms.author: markwallace
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# Using MCP tools with Agents

The Microsoft Agent Framework supports integration with Model Context Protocol (MCP) servers, allowing your agents to access external tools and services. This guide shows how to connect to an MCP server and use its tools within your agent.

::: zone pivot="programming-language-csharp"

The .Net version of Agent Framework can be used together with the [official MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk) to allow your agent to call MCP tools.

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

### Creating an Agent with MCP Tools

Create your agent and provide the MCP tools during initialization:

```csharp
AIAgent agent = new AzureOpenAIClient(
    new Uri(endpoint),
    new AzureCliCredential())
     .GetChatClient(deploymentName)
     .CreateAIAgent(
         instructions: "You answer questions related to GitHub repositories only.", 
         tools: [.. mcpTools.Cast<AITool>()]);

```

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


The full source code and instructions to run this sample is available [here](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/GettingStarted/ModelContextProtocol/Agent_MCP_Server).

::: zone-end
::: zone pivot="programming-language-python"

Coming soon.

::: zone-end
