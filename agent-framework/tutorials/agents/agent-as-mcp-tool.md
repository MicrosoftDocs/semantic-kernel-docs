---
title: Exposing an agent as an MCP tool
description: Learn how to expose an agent as a tool over the MCP protocol
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# Exposing an agent as an MCP tool

This tutorial shows you how to expose an agent as a tool over the Model Context Protocol (MCP), so it can be used by other systems that support MCP tools.

## Prerequisites

For prerequisites see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Installing Nuget packages

To use the Microsoft Agent Framework with Azure OpenAI, you need to install the following NuGet packages:

```powershell
dotnet add package Azure.Identity
dotnet add package Azure.AI.OpenAI
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

To add support for hosting a tool over the Model Context Protocol (MCP), add the following Nuget packages

```powershell
dotnet add package Microsoft.Extensions.Hosting --prerelease
dotnet add package ModelContextProtocol --prerelease
```

## Exposing an agent as an MCP tool

You can expose an `AIAgent` as an MCP tool by wrapping it in a function and using `McpServerTool`. You then need to register it with an MCP server. This allows the agent to be invoked as a tool by any MCP-compatible client.

First, create an agent that we will expose as an MCP tool.

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI;

AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o-mini")
        .CreateAIAgent(instructions: "You are good at telling jokes.", name: "Joker");
```

Turn the agent into a function tool and then an MCP tool. The agent name and description will be used as the mcp tool name and description.

```csharp
using ModelContextProtocol.Server;

McpServerTool tool = McpServerTool.Create(agent.AsAIFunction());
```

Setup the MCP server to listen for incoming requests over standard input/output and expose the MCP tool:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools([tool]);

await builder.Build().RunAsync();
```

This will start an MCP server that exposes the agent as a tool over the MCP protocol.

## Next steps

> [!div class="nextstepaction"]
> [Enabling observability for agents](./enable-observability.md)
