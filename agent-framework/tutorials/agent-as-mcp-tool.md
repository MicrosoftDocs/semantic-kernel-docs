---
title: Exposing an agent as an MCP tool
description: Learn how to expose an agent as a tool over the MCP protocol
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/24/2025
ms.service: agent-framework
---

# Exposing an agent as an MCP tool

::: zone pivot="programming-language-csharp"

This tutorial shows you how to expose an agent as a tool over the Model Context Protocol (MCP), so it can be used by other systems that support MCP tools.

## Prerequisites

For prerequisites and installing nuget packages, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Exposing an agent as an MCP tool

You can expose an `AIAgent` as an MCP tool by wrapping it in a function and using `McpServerTool`. You then need to register it with an MCP server. This allows the agent to be invoked as a tool by any MCP-compatible client.

First, create an agent that we will expose as an MCP tool.

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using OpenAI;

AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o-mini")
        .CreateAIAgent(instructions: "You are good at telling jokes.", name: "Joker");
```

Turn the agent into a function tool. This function tool will be used as the implementation of the MCP tool.

```csharp
var agentAsAIFunction = agent.AsAIFunction();
```

Setup the MCP server to listen for incoming requests over standard input/output and expose the function tool as an MCP tool:

```csharp
HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools([
        McpServerTool.Create(
            agentAsAIFunction,
            new McpServerToolCreateOptions()
            {
                Title = agent.Name,
                Name = agent.Name,
                Description = agent.Description
            })
    ]);

await builder.Build().RunAsync();
```

This will start an MCP server that exposes the agent as a tool over the MCP protocol.

::: zone-end
::: zone pivot="programming-language-python"

Tutorial coming soon.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Persisting Conversations](./persisted-conversation.md)
