---
title: Claude Agents
description: Learn how to use Microsoft Agent Framework with the Claude Agent SDK.
zone_pivot_groups: programming-languages
author: dmytrostruk
ms.topic: tutorial
ms.author: dmytrostruk
ms.date: 01/30/2026
ms.service: agent-framework
---

# Claude Agents

Microsoft Agent Framework supports creating agents that use the [Claude Agent SDK](https://github.com/anthropics/claude-agent-sdk-python) as their backend. Claude Agent SDK agents provide access to Claude's full agentic capabilities through the Claude Code CLI, including file editing, code execution, and advanced tool workflows.

> [!IMPORTANT]
> Claude Agent SDK agents require the Claude Code CLI to be installed and authenticated. This is different from the [Anthropic Agent](./anthropic-agent.md) which uses the Anthropic API directly. The Claude Agent SDK provides full agentic capabilities via the CLI.

::: zone pivot="programming-language-csharp"

.NET support for Claude Agent SDK is not currently available. Please use Python for Claude Agent SDK agents.

::: zone-end
::: zone pivot="programming-language-python"

## Prerequisites

Install the Microsoft Agent Framework Claude package.

```bash
pip install agent-framework-claude --pre
```

## Configuration

The agent can be optionally configured using the following environment variables:

| Variable | Description |
|----------|-------------|
| `CLAUDE_AGENT_CLI_PATH` | Path to the Claude CLI executable |
| `CLAUDE_AGENT_MODEL` | Model to use ("sonnet", "opus", "haiku") |
| `CLAUDE_AGENT_CWD` | Working directory for Claude CLI |
| `CLAUDE_AGENT_PERMISSION_MODE` | Permission mode (default, acceptEdits, plan, bypassPermissions) |
| `CLAUDE_AGENT_MAX_TURNS` | Maximum conversation turns |
| `CLAUDE_AGENT_MAX_BUDGET_USD` | Budget limit in USD |

## Getting Started

Import the required classes from Agent Framework:

```python
import asyncio
from agent_framework_claude import ClaudeAgent
```

## Create a Claude Agent

### Basic Agent Creation

The simplest way to create a Claude agent using the async context manager:

```python
async def basic_example():
    async with ClaudeAgent(
        instructions="You are a helpful assistant.",
    ) as agent:
        response = await agent.run("Hello, how can you help me?")
        print(response.text)
```

### With Explicit Configuration

You can provide explicit configuration through `default_options`:

```python
async def explicit_config_example():
    async with ClaudeAgent(
        instructions="You are a helpful assistant.",
        default_options={
            "model": "sonnet",
            "permission_mode": "default",
            "max_turns": 10,
        },
    ) as agent:
        response = await agent.run("What can you do?")
        print(response.text)
```

## Agent Features

### Built-in Tools

Claude Agent SDK provides access to built-in tools by passing tool names as strings. These include file operations, shell commands, and more:

```python
async def builtin_tools_example():
    async with ClaudeAgent(
        instructions="You are a helpful coding assistant.",
        tools=["Read", "Write", "Bash", "Glob"],
    ) as agent:
        response = await agent.run("List all Python files in the current directory")
        print(response.text)
```

### Function Tools

Equip your agent with custom functions alongside built-in tools:

```python
from typing import Annotated
from pydantic import Field

def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
) -> str:
    """Get the weather for a given location."""
    return f"The weather in {location} is sunny with a high of 25C."

async def tools_example():
    async with ClaudeAgent(
        instructions="You are a helpful weather agent.",
        tools=[get_weather],
    ) as agent:
        response = await agent.run("What's the weather like in Seattle?")
        print(response.text)
```

### Streaming Responses

Get responses as they are generated for better user experience:

```python
async def streaming_example():
    async with ClaudeAgent(
        instructions="You are a helpful assistant.",
    ) as agent:
        print("Agent: ", end="", flush=True)
        async for chunk in agent.run_stream("Tell me a short story."):
            if chunk.text:
                print(chunk.text, end="", flush=True)
        print()
```

### Thread Management

Maintain conversation context across multiple interactions using threads:

```python
async def thread_example():
    async with ClaudeAgent(
        instructions="You are a helpful assistant. Keep your answers short.",
    ) as agent:
        thread = agent.get_new_thread()

        # First turn
        await agent.run("My name is Alice.", thread=thread)

        # Second turn - agent remembers the context via session resumption
        response = await agent.run("What is my name?", thread=thread)
        print(response.text)  # Should mention "Alice"
```

### Permission Modes

Control how the agent handles permission requests for file operations and command execution:

```python
async def permission_mode_example():
    async with ClaudeAgent(
        instructions="You are a coding assistant that can edit files.",
        tools=["Read", "Write", "Bash"],
        default_options={
            "permission_mode": "acceptEdits",  # Auto-accept file edits
        },
    ) as agent:
        response = await agent.run("Create a hello.py file that prints 'Hello, World!'")
        print(response.text)
```

### MCP Servers

Connect to local (stdio) or remote (HTTP) MCP servers for additional capabilities:

```python
async def mcp_example():
    async with ClaudeAgent(
        instructions="You are a helpful assistant with access to the filesystem and Microsoft Learn.",
        default_options={
            "mcp_servers": {
                # Local stdio server
                "filesystem": {
                    "command": "npx",
                    "args": ["-y", "@modelcontextprotocol/server-filesystem", "."],
                },
                # Remote HTTP server
                "microsoft-learn": {
                    "type": "http",
                    "url": "https://learn.microsoft.com/api/mcp",
                },
            },
        },
    ) as agent:
        response = await agent.run("Search Microsoft Learn for 'Azure Functions' and summarize the top result")
        print(response.text)
```

## Using the Agent

The agent is a standard `BaseAgent` and supports all standard agent operations.

For more information on how to run and interact with agents, see the [Agent getting started tutorials](../../../tutorials/overview.md).

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Custom Agents](./custom-agent.md)
