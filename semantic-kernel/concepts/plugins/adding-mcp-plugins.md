---
title: Give agents access to MCP Servers
description: Learn how to add plugins from a MCP Server to your agents in Semantic Kernel.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 04/15/2025
ms.service: semantic-kernel
---

# Add plugins from a MCP Server

MCP is the Model Context Protocol, it is an open protocol that is designed to allow additional capabilities to be added to AI applications with ease, see [the documentation](https://modelcontextprotocol.io/introduction) for more info.
Semantic Kernel allows you to add plugins from a MCP Server to your agents. This is useful when you want to use plugins that are made available as a MCP Server.

Semantic Kernel supports both local MCP Servers, through Stdio, or servers that connect through SSE over HTTPS.

## Add plugins from a local MCP Server

To add a locally running MCP server, you can use the familiar MCP commands, like `npx`, `docker` or `uvx`, so if you want to run one of those, make sure those are installed.

For instance when you look into your claude desktop config, or the vscode settings.json, you would see something like this:

```json
{
    "mcpServers": {
        "github": {
           "command": "docker",
           "args": [
                 "run",
                 "-i",
                 "--rm",
                 "-e",
                 "GITHUB_PERSONAL_ACCESS_TOKEN",
                 "ghcr.io/github/github-mcp-server"
           ],
           "env": {
                 "GITHUB_PERSONAL_ACCESS_TOKEN": "..."
           }
        }
    }
}
```

In order to make the same plugin available to your kernel or agent, you would do this:

::: zone pivot="programming-language-python"

```python
import os
from semantic_kernel import Kernel
from semantic_kernel.connectors.mcp import MCPStdioPlugin

async def main():
    async with MCPStdioPlugin(
        name="Github",
        description="Github Plugin",
        command="docker",
        args=["run", "-i", "--rm", "-e", "GITHUB_PERSONAL_ACCESS_TOKEN", "ghcr.io/github/github-mcp-server"],
        env={"GITHUB_PERSONAL_ACCESS_TOKEN": os.getenv("GITHUB_PERSONAL_ACCESS_TOKEN")},
    ) as github_plugin:
        kernel = Kernel()
        kernel.add_plugin(github_plugin)
        # Do something with the kernel
```

An SSE-based MCP server is even simpler as it just needs the URL:

```python
import os
from semantic_kernel import Kernel
from semantic_kernel.connectors.mcp import MCPSsePlugin

async def main():
    async with MCPSsePlugin(
        name="Github",
        description="Github Plugin",
        url="http://localhost:8080",
    ) as github_plugin:
        kernel = Kernel()
        kernel.add_plugin(github_plugin)
        # Do something with the kernel
```

In both case the async context manager is used to setup the connection and close it, you can also do this manually:

```python
import os
from semantic_kernel import Kernel
from semantic_kernel.connectors.mcp import MCPSsePlugin

async def main():
    plugin = MCPSsePlugin(
        name="Github",
        description="Github Plugin",
        url="http://localhost:8080",
    )
    await plugin.connect()   
    kernel = Kernel()
    kernel.add_plugin(github_plugin)
    # Do something with the kernel
    await plugin.close()
```

::: zone-end
::: zone pivot="programming-language-csharp"

> [!NOTE]
> MCP Documentation is coming soon for .Net.

::: zone-end
::: zone pivot="programming-language-java"

> [!NOTE]
> MCP Documentation is coming soon for Java.

::: zone-end
