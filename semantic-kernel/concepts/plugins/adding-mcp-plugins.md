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

> [!NOTE]
> Make sure to install Semantic Kernel with the `mcp` extra, for instance:
> ```bash
> pip install semantic-kernel[mcp]
> ```
>

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

All the MCP plugins have additional options:
- `load_tools`: Whether or not to load the tools from the MCP server, this is useful when you know there are no tools available, default is True.
- `load_prompts`: Whether or not to load the prompts from the MCP server, this is useful when you know there are no prompts available, default is True. We have also heard cases where the call to load prompts hangs when it there are no prompts, so this is a good option to set to `False` if you know there are no prompts available.
- `request_timeout`: The timeout for the requests to the MCP server, this is useful when you know the server is sometimes non-responding, and you do not want your app to hang.

::: zone-end
::: zone pivot="programming-language-csharp"

The simplest way to add a plugin using MCP with Semantic Kernel is using the `ModelContextProtocol-SemanticKernel` package. 

### Add the package
```
dotnet add package ModelContextProtocol-SemanticKernel --version 0.3.0-preview-01
```

### Register a single function or tool
Use the extension method to register a specific MCP function/tool.
```
// Stdio
await kernel.Plugins.AddMcpFunctionsFromStdioServerAsync("GitHub", "npx", ["-y", "@modelcontextprotocol/server-github"]);

// SSE
await kernel.Plugins.AddMcpFunctionsFromSseServerAsync("GitHub", new Uri("http://localhost:12345"));
```

### Register MCP Server(s) from Claude Desktop configuration
It's also possible to register all Stdio MCP Servers which are registered in Claude Desktop:
```
// Stdio MCP Tools defined in claude_desktop_config.json
await kernel.Plugins.AddToolsFromClaudeDesktopConfigAsync();
```

### Full STDIO example
```
var builder = Kernel.CreateBuilder();
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));

builder.Services.AddOpenAIChatCompletion(
    serviceId: "openai",
    modelId: "gpt-4o-mini",
    apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);

var kernel = builder.Build();

// Add this line to enable MCP functions from a Stdio server named "Everything"
await kernel.Plugins.AddMcpFunctionsFromStdioServerAsync("Everything", "npx", ["-y", "@modelcontextprotocol/server-github"]);

var executionSettings = new OpenAIPromptExecutionSettings
{
    Temperature = 0,
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var prompt = "Please call the echo tool with the string 'Hello Stef!' and give me the response as-is.";
var result = await kernel.InvokePromptAsync(prompt, new(executionSettings)).ConfigureAwait(false);
Console.WriteLine($"\n\n{prompt}\n{result}");
```
Result:
```
Please call the echo tool with the string 'Hello Stef!' and give me the response as-is.
Echo: Hello Stef!
```

### Full SSE example
```
var builder = Kernel.CreateBuilder();
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));

builder.Services.AddOpenAIChatCompletion(
    serviceId: "openai",
    modelId: "gpt-4o-mini",
    apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);

var kernel = builder.Build();

// Add this line to enable MCP functions from a Sse server named "Github"
// - Note that a server must be running at the specified URL
await kernel.Plugins.AddMcpFunctionsFromSseServerAsync("GitHub", "http://localhost:12345");

var executionSettings = new OpenAIPromptExecutionSettings
{
    Temperature = 0,
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var prompt = "Summarize the last 3 commits to the StefH/FluentBuilder repository.";
var result = await kernel.InvokePromptAsync(prompt, new(executionSettings)).ConfigureAwait(false);
Console.WriteLine($"\n\n{prompt}\n{result}");
```

Results:
```
Summarize the last 3 commits to the StefH/FluentBuilder repository.
Here are the summaries of the last three commits to the `StefH/FluentBuilder` repository:

1. **Commit [2293880](https://github.com/StefH/FluentBuilder/commit/229388090f50a39f489e30cb535f67f3705cf61f)** (January 30, 2025)
   - **Author:** Stef Heyenrath
   - **Message:** Update README.md
   - **Details:** This commit updates the README.md file. The commit was verified and is valid.

2. **Commit [ae27064](https://github.com/StefH/FluentBuilder/commit/ae2706424c3b75613bf5625091aa2649fb33ecde)** (November 6, 2024)
   - **Author:** Stef Heyenrath
   - **Message:** Update README.md
   - **Details:** This commit also updates the README.md file. The commit was verified and is valid.

3. **Commit [53096a8](https://github.com/StefH/FluentBuilder/commit/53096a8b54a1029532425bc727fdd831e9ed0092)** (October 20, 2024)
   - **Author:** Stef Heyenrath
   - **Message:** Update README.md
   - **Details:** This commit updates the README.md file as well. The commit was verified and is valid.

All three commits involve updates to the README.md file, reflecting ongoing improvements or changes to the documentation.
```

::: zone-end
::: zone pivot="programming-language-java"

> [!NOTE]
> MCP Documentation is coming soon for Java.

::: zone-end
