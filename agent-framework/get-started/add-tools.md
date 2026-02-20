---
title: "Step 2: Add Tools"
description: "Give your agent the ability to call functions and interact with the world."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: tutorial
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Step 2: Add Tools

Tools let your agent call custom functions — like fetching weather data, querying a database, or calling an API.

:::zone pivot="programming-language-csharp"

Define a tool as any method with a `[Description]` attribute:

```csharp
using System.ComponentModel;

[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";
```

Create an agent with the tool:

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("Set AZURE_OPENAI_ENDPOINT");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a helpful assistant.", tools: [AIFunctionFactory.Create(GetWeather)]);
```

The agent will automatically call your tool when relevant:

```csharp
Console.WriteLine(await agent.RunAsync("What is the weather like in Amsterdam?"));
```

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/01-get-started/02_AddTools.cs) for the complete runnable file.

:::zone-end

:::zone pivot="programming-language-python"

Define a tool with the `@tool` decorator:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/02_add_tools.py" id="define_tool" highlight="3":::

Create an agent with the tool:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/02_add_tools.py" id="create_agent_with_tools" highlight="4":::

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/01-get-started/02_add_tools.py) for the complete runnable file.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Step 3: Multi-Turn Conversations](./multi-turn.md)

**Go deeper:**

- [Tools overview](../agents/tools/index.md) — learn about all available tool types
- [Function tools](../agents/tools/function-tools.md) — advanced function tool patterns
- [Tool approval](../agents/tools/tool-approval.md) — human-in-the-loop for tool calls
