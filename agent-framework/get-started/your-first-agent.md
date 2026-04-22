---
title: "Step 1: Your First Agent"
description: "Create and run your first AI agent with Agent Framework in under 5 minutes."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: tutorial
ms.author: edvan
ms.date: 04/22/2026
ms.service: agent-framework
---

# Step 1: Your First Agent

Create an agent and get a response — in just a few lines of code.

:::zone pivot="programming-language-csharp"

```dotnetcli
dotnet add package Azure.AI.Projects --prerelease
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.Foundry --prerelease
```

Create the agent:

```csharp
using System;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("Set AZURE_OPENAI_ENDPOINT");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

AIAgent agent = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential())
    .AsAIAgent(
        model: deploymentName,
        instructions: "You are a friendly assistant. Keep your answers brief.",
        name: "HelloAgent");
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

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
pip install agent-framework
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

:::zone pivot="programming-language-go"

```bash
go get github.com/microsoft/agent-framework-go
```

Create and run an agent:

```go
package main

import (
	"cmp"
	"context"
	"fmt"
	"os"

	"github.com/Azure/azure-sdk-for-go/sdk/azidentity"
	"github.com/microsoft/agent-framework-go/agent"
	"github.com/microsoft/agent-framework-go/agent/provider/openaichatagent"
	openai "github.com/openai/openai-go/v3"
	"github.com/openai/openai-go/v3/azure"
)

func main() {
	endpoint := os.Getenv("AZURE_OPENAI_ENDPOINT")
	deployment := cmp.Or(os.Getenv("AZURE_OPENAI_DEPLOYMENT_NAME"), "gpt-4o-mini")
	apiVersion := cmp.Or(os.Getenv("AZURE_OPENAI_API_VERSION"), "2025-01-01-preview")

	token, err := azidentity.NewDefaultAzureCredential(nil)
	if err != nil {
		panic(err)
	}

	a := openaichatagent.New(
		openai.NewClient(
			azure.WithEndpoint(endpoint, apiVersion),
			azure.WithTokenCredential(token),
		),
		openaichatagent.Config{
			Model: deployment,
			Config: agent.Config{
				Instructions: "You are a friendly assistant. Keep your answers brief.",
				Name:         "HelloAgent",
			},
		},
	)

	ctx := context.Background()

	// Invoke the agent and collect the text result.
	resp, err := a.RunText(ctx, "What is the largest city in France?").Collect()
	if err != nil {
		panic(err)
	}
	fmt.Println(resp)
}
```

Or stream the response:

```go
for update, err := range a.RunText(ctx, "Tell me a one-sentence fun fact.", agentopt.Stream(true)) {
	if err != nil {
		panic(err)
	}
	fmt.Print(update)
}
```

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework-go/blob/main/examples/01-get-started/01_hello_agent/main.go) for the complete runnable file.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Step 2: Add Tools](./add-tools.md)

**Go deeper:**

- [Agents overview](../agents/index.md) — understand agent architecture
- [Providers](../agents/providers/index.md) — see all supported providers
