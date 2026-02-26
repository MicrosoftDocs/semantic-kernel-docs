---
title: "Step 1: Your First Agent"
description: "Create and run your first AI agent with Agent Framework in under 5 minutes."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: tutorial
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Step 1: Your First Agent

Create an agent and get a response — in just a few lines of code.

:::zone pivot="programming-language-csharp"

```dotnetcli
dotnet add package Azure.AI.OpenAI --prerelease
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
```

Create the agent:

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("Set AZURE_OPENAI_ENDPOINT");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a friendly assistant. Keep your answers brief.", name: "HelloAgent");
```

Run it:

```csharp
Console.WriteLine(await agent.RunAsync("What is the largest city in France?"));
```

Or stream the response:

```csharp
await foreach (var update in agent.RunStreamingAsync("Tell me a one-sentence fun fact."))
{
    Console.Write(update);
}
```

> [!TIP]
> See [here](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/01-get-started/01_hello_agent) for a full runnable sample application.

:::zone-end

:::zone pivot="programming-language-python"

```bash
pip install agent-framework --pre
```

Create and run an agent:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/01_hello_agent.py" id="create_agent" highlight="8-11":::

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/01_hello_agent.py" id="run_agent" highlight="2":::

Or stream the response:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/01_hello_agent.py" id="run_agent_streaming" highlight="3-5":::

> [!NOTE]
> Agent Framework does **not** automatically load `.env` files. To use a `.env` file for configuration, call `load_dotenv()` at the start of your script:
>
> ```python
> from dotenv import load_dotenv
> load_dotenv()
> ```
>
> Alternatively, set environment variables directly in your shell or IDE. See the [settings migration note](../support/upgrade/python-2026-significant-changes.md#-pydantic-settings-replaced-with-typeddict--load_settings) for details.

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/01-get-started/01_hello_agent.py) for the complete runnable file.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Step 2: Add Tools](./add-tools.md)

**Go deeper:**

- [Agents overview](../agents/index.md) — understand agent architecture
- [Providers](../agents/providers/index.md) — see all supported providers
