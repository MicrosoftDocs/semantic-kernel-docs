---
title: Code Interpreter
description: Learn how to use the Code Interpreter tool with Agent Framework agents.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: reference
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Code Interpreter

Code Interpreter allows agents to write and execute code in a sandboxed environment. This is useful for data analysis, mathematical computations, file processing, and other tasks that benefit from code execution.

> [!NOTE]
> Code Interpreter availability depends on the underlying agent provider. See [Providers Overview](../providers/index.md) for provider-specific support.

:::zone pivot="programming-language-csharp"

The following example shows how to create an agent with the Code Interpreter tool and read the generated output:

### Create an agent with Code Interpreter

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// Requires: dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

// Create an agent with the code interpreter hosted tool
AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(
        instructions: "You are a helpful assistant that can write and execute Python code.",
        tools: [new CodeInterpreterToolDefinition()]);

var response = await agent.RunAsync("Calculate the factorial of 100 using code.");
Console.WriteLine(response);
```

### Read code output

```csharp
// Inspect code interpreter output from the response
foreach (var message in response.Messages)
{
    foreach (var content in message.Contents)
    {
        if (content is CodeInterpreterContent codeContent)
        {
            Console.WriteLine($"Code:\n{codeContent.Code}");
            Console.WriteLine($"Output:\n{codeContent.Output}");
        }
    }
}
```

:::zone-end

:::zone pivot="programming-language-python"

The following example shows how to create an agent with the Code Interpreter tool:

```python
# Copyright (c) Microsoft. All rights reserved.

import asyncio

from agent_framework import (
    Agent,
    Content,
)
from agent_framework.openai import OpenAIResponsesClient

"""
OpenAI Responses Client with Code Interpreter Example

This sample demonstrates using get_code_interpreter_tool() with OpenAI Responses Client
for Python code execution and mathematical problem solving.
"""


async def main() -> None:
    """Example showing how to use the code interpreter tool with OpenAI Responses."""
    print("=== OpenAI Responses Agent with Code Interpreter Example ===")

    client = OpenAIResponsesClient()
    agent = Agent(
        client=client,
        instructions="You are a helpful assistant that can write and execute Python code to solve problems.",
        tools=client.get_code_interpreter_tool(),
    )

    query = "Use code to get the factorial of 100?"
    print(f"User: {query}")
    result = await agent.run(query)
    print(f"Result: {result}\n")

    for message in result.messages:
        code_blocks = [c for c in message.contents if c.type == "code_interpreter_tool_call"]
        outputs = [c for c in message.contents if c.type == "code_interpreter_tool_result"]

        if code_blocks:
            code_inputs = code_blocks[0].inputs or []
            for content in code_inputs:
                if isinstance(content, Content) and content.type == "text":
                    print(f"Generated code:\n{content.text}")
                    break
        if outputs:
            print("Execution outputs:")
            for out in outputs[0].outputs or []:
                if isinstance(out, Content) and out.type == "text":
                    print(out.text)


if __name__ == "__main__":
    asyncio.run(main())
```

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [File Search](./file-search.md)
