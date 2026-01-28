---
title: GitHub Copilot Agents
description: Learn how to use Microsoft Agent Framework with the GitHub Copilot SDK.
zone_pivot_groups: programming-languages
author: dmytrostruk
ms.topic: tutorial
ms.author: dmytrostruk
ms.date: 01/26/2026
ms.service: agent-framework
---

# GitHub Copilot Agents

Microsoft Agent Framework supports creating agents that use the [GitHub Copilot SDK](https://github.com/github/copilot-sdk) as their backend. GitHub Copilot agents provide access to powerful coding-oriented AI capabilities, including shell command execution, file operations, URL fetching, and Model Context Protocol (MCP) server integration.

> [!IMPORTANT]
> GitHub Copilot agents require the GitHub Copilot CLI to be installed and authenticated. For security, it is recommended to run agents with shell or file permissions in a containerized environment (Docker/Dev Container).

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```dotnetcli
dotnet add package Microsoft.Agents.AI.GitHub.Copilot --prerelease
```

## Create a GitHub Copilot Agent

As a first step, create a `CopilotClient` and start it. Then use the `AsAIAgent` extension method to create an agent.

```csharp
using GitHub.Copilot.SDK;
using Microsoft.Agents.AI;

await using CopilotClient copilotClient = new();
await copilotClient.StartAsync();

AIAgent agent = copilotClient.AsAIAgent();

Console.WriteLine(await agent.RunAsync("What is Microsoft Agent Framework?"));
```

### With Tools and Instructions

You can provide function tools and custom instructions when creating the agent:

```csharp
using GitHub.Copilot.SDK;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

AIFunction weatherTool = AIFunctionFactory.Create((string location) =>
{
    return $"The weather in {location} is sunny with a high of 25C.";
}, "GetWeather", "Get the weather for a given location.");

await using CopilotClient copilotClient = new();
await copilotClient.StartAsync();

AIAgent agent = copilotClient.AsAIAgent(
    tools: [weatherTool],
    instructions: "You are a helpful weather agent.");

Console.WriteLine(await agent.RunAsync("What's the weather like in Seattle?"));
```

## Agent Features

### Streaming Responses

Get responses as they are generated:

```csharp
await using CopilotClient copilotClient = new();
await copilotClient.StartAsync();

AIAgent agent = copilotClient.AsAIAgent();

await foreach (AgentResponseUpdate update in agent.RunStreamingAsync("Tell me a short story."))
{
    Console.Write(update);
}

Console.WriteLine();
```

### Session Management

Maintain conversation context across multiple interactions using sessions:

```csharp
await using CopilotClient copilotClient = new();
await copilotClient.StartAsync();

await using GitHubCopilotAgent agent = new(
    copilotClient,
    instructions: "You are a helpful assistant. Keep your answers short.");

AgentSession session = await agent.GetNewSessionAsync();

// First turn
await agent.RunAsync("My name is Alice.", session);

// Second turn - agent remembers the context
AgentResponse response = await agent.RunAsync("What is my name?", session);
Console.WriteLine(response); // Should mention "Alice"
```

### Permissions

By default, the agent cannot execute shell commands, read/write files, or fetch URLs. To enable these capabilities, provide a permission handler via `SessionConfig`:

```csharp
static Task<PermissionRequestResult> PromptPermission(
    PermissionRequest request, PermissionInvocation invocation)
{
    Console.WriteLine($"\n[Permission Request: {request.Kind}]");
    Console.Write("Approve? (y/n): ");

    string? input = Console.ReadLine()?.Trim().ToUpperInvariant();
    string kind = input is "Y" or "YES" ? "approved" : "denied-interactively-by-user";

    return Task.FromResult(new PermissionRequestResult { Kind = kind });
}

await using CopilotClient copilotClient = new();
await copilotClient.StartAsync();

SessionConfig sessionConfig = new()
{
    OnPermissionRequest = PromptPermission,
};

AIAgent agent = copilotClient.AsAIAgent(sessionConfig);

Console.WriteLine(await agent.RunAsync("List all files in the current directory"));
```

### MCP Servers

Connect to local (stdio) or remote (HTTP) MCP servers for extended capabilities:

```csharp
await using CopilotClient copilotClient = new();
await copilotClient.StartAsync();

SessionConfig sessionConfig = new()
{
    OnPermissionRequest = PromptPermission,
    McpServers = new Dictionary<string, object>
    {
        // Local stdio server
        ["filesystem"] = new McpLocalServerConfig
        {
            Type = "stdio",
            Command = "npx",
            Args = ["-y", "@modelcontextprotocol/server-filesystem", "."],
            Tools = ["*"],
        },
        // Remote HTTP server
        ["microsoft-learn"] = new McpRemoteServerConfig
        {
            Type = "http",
            Url = "https://learn.microsoft.com/api/mcp",
            Tools = ["*"],
        },
    },
};

AIAgent agent = copilotClient.AsAIAgent(sessionConfig);

Console.WriteLine(await agent.RunAsync("Search Microsoft Learn for 'Azure Functions' and summarize the top result"));
```

## Using the Agent

The agent is a standard `AIAgent` and supports all standard `AIAgent` operations.

For more information on how to run and interact with agents, see the [Agent getting started tutorials](../../../tutorials/overview.md).

::: zone-end
::: zone pivot="programming-language-python"

## Prerequisites

Install the Microsoft Agent Framework GitHub Copilot package.

```bash
pip install agent-framework-github-copilot --pre
```

## Configuration

The agent can be optionally configured using the following environment variables:

| Variable | Description |
|----------|-------------|
| `GITHUB_COPILOT_CLI_PATH` | Path to the Copilot CLI executable |
| `GITHUB_COPILOT_MODEL` | Model to use (e.g., `gpt-5`, `claude-sonnet-4`) |
| `GITHUB_COPILOT_TIMEOUT` | Request timeout in seconds |
| `GITHUB_COPILOT_LOG_LEVEL` | CLI log level |

## Getting Started

Import the required classes from Agent Framework:

```python
import asyncio
from agent_framework.github import GitHubCopilotAgent, GitHubCopilotOptions
```

## Create a GitHub Copilot Agent

### Basic Agent Creation

The simplest way to create a GitHub Copilot agent:

```python
async def basic_example():
    agent = GitHubCopilotAgent(
        default_options={"instructions": "You are a helpful assistant."},
    )

    async with agent:
        result = await agent.run("What is Microsoft Agent Framework?")
        print(result)
```

### With Explicit Configuration

You can provide explicit configuration through `default_options`:

```python
async def explicit_config_example():
    agent = GitHubCopilotAgent(
        default_options={
            "instructions": "You are a helpful assistant.",
            "model": "gpt-5",
            "timeout": 120,
        },
    )

    async with agent:
        result = await agent.run("What can you do?")
        print(result)
```

## Agent Features

### Function Tools

Equip your agent with custom functions:

```python
from typing import Annotated
from pydantic import Field

def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
) -> str:
    """Get the weather for a given location."""
    return f"The weather in {location} is sunny with a high of 25C."

async def tools_example():
    agent = GitHubCopilotAgent(
        default_options={"instructions": "You are a helpful weather agent."},
        tools=[get_weather],
    )

    async with agent:
        result = await agent.run("What's the weather like in Seattle?")
        print(result)
```

### Streaming Responses

Get responses as they are generated for better user experience:

```python
async def streaming_example():
    agent = GitHubCopilotAgent(
        default_options={"instructions": "You are a helpful assistant."},
    )

    async with agent:
        print("Agent: ", end="", flush=True)
        async for chunk in agent.run_stream("Tell me a short story."):
            if chunk.text:
                print(chunk.text, end="", flush=True)
        print()
```

### Thread Management

Maintain conversation context across multiple interactions:

```python
async def thread_example():
    agent = GitHubCopilotAgent(
        default_options={"instructions": "You are a helpful assistant."},
    )

    async with agent:
        thread = agent.get_new_thread()

        # First interaction
        result1 = await agent.run("My name is Alice.", thread=thread)
        print(f"Agent: {result1}")

        # Second interaction - agent remembers the context
        result2 = await agent.run("What's my name?", thread=thread)
        print(f"Agent: {result2}")  # Should remember "Alice"
```

### Permissions

By default, the agent cannot execute shell commands, read/write files, or fetch URLs. To enable these capabilities, provide a permission handler:

```python
from copilot.types import PermissionRequest, PermissionRequestResult

def prompt_permission(
    request: PermissionRequest, context: dict[str, str]
) -> PermissionRequestResult:
    kind = request.get("kind", "unknown")
    print(f"\n[Permission Request: {kind}]")

    response = input("Approve? (y/n): ").strip().lower()
    if response in ("y", "yes"):
        return PermissionRequestResult(kind="approved")
    return PermissionRequestResult(kind="denied-interactively-by-user")

async def permissions_example():
    agent = GitHubCopilotAgent(
        default_options={
            "instructions": "You are a helpful assistant that can execute shell commands.",
            "on_permission_request": prompt_permission,
        },
    )

    async with agent:
        result = await agent.run("List the Python files in the current directory")
        print(result)
```

### MCP Servers

Connect to local (stdio) or remote (HTTP) MCP servers for extended capabilities:

```python
from copilot.types import MCPServerConfig

async def mcp_example():
    mcp_servers: dict[str, MCPServerConfig] = {
        # Local stdio server
        "filesystem": {
            "type": "stdio",
            "command": "npx",
            "args": ["-y", "@modelcontextprotocol/server-filesystem", "."],
            "tools": ["*"],
        },
        # Remote HTTP server
        "microsoft-learn": {
            "type": "http",
            "url": "https://learn.microsoft.com/api/mcp",
            "tools": ["*"],
        },
    }

    agent = GitHubCopilotAgent(
        default_options={
            "instructions": "You are a helpful assistant with access to the filesystem and Microsoft Learn.",
            "on_permission_request": prompt_permission,
            "mcp_servers": mcp_servers,
        },
    )

    async with agent:
        result = await agent.run("Search Microsoft Learn for 'Azure Functions' and summarize the top result")
        print(result)
```

## Using the Agent

The agent is a standard `BaseAgent` and supports all standard agent operations.

For more information on how to run and interact with agents, see the [Agent getting started tutorials](../../../tutorials/overview.md).

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Custom Agents](./custom-agent.md)
