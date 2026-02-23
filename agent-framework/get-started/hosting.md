---
title: "Step 6: Host Your Agent"
description: "Deploy your agent so users and other agents can interact with it."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: tutorial
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Step 6: Host Your Agent

Once you've built your agent, you need to host it so users and other agents can interact with it.

## Hosting Options

| Option | Description | Best For |
|--------|-------------|----------|
| [A2A Protocol](../integrations/a2a.md) | Expose agents via the Agent-to-Agent protocol | Multi-agent systems |
| [OpenAI-Compatible Endpoints](../integrations/openai-endpoints.md) | Expose agents via Chat Completions or Responses APIs | OpenAI-compatible clients |
| [Azure Functions (Durable)](../integrations/azure-functions.md) | Run agents as durable Azure Functions | Serverless, long-running tasks |
| [AG-UI Protocol](../integrations/ag-ui/index.md) | Build web-based AI agent applications | Web frontends |

:::zone pivot="programming-language-csharp"

## Hosting in ASP.NET Core

The Agent Framework provides hosting libraries that enable you to integrate AI agents into ASP.NET Core applications. These libraries simplify registering, configuring, and exposing agents through various protocols.

As described in the [Agents Overview](../agents/index.md), `AIAgent` is the fundamental concept of the Agent Framework. It defines an "LLM wrapper" that processes user inputs, makes decisions, calls tools, and performs additional work to execute actions and generate responses. Exposing AI agents from your ASP.NET Core application is not trivial. The hosting libraries solve this by registering AI agents in a dependency injection container, allowing you to resolve and use them in your application services. They also enable you to manage agent dependencies, such as tools and session storage, from the same container. Agents can be hosted alongside your application infrastructure, independent of the protocols they use. Similarly, workflows can be hosted and leverage your application's common infrastructure.

### Core Hosting Library

The `Microsoft.Agents.AI.Hosting` library is the foundation for hosting AI agents in ASP.NET Core. It provides extensions for `IHostApplicationBuilder` to register and configure AI agents and workflows. In ASP.NET Core, `IHostApplicationBuilder` is the fundamental type that represents the builder for hosted applications and services, managing configuration, logging, lifetime, and more.

Before configuring agents or workflows, register an `IChatClient` in the dependency injection container. In the examples below, it is registered as a keyed singleton under the name `chat-model`:

```csharp
// endpoint is of 'https://<your-own-foundry-endpoint>.openai.azure.com/' format
// deploymentName is 'gpt-4o-mini' for example

IChatClient chatClient = new AzureOpenAIClient(
        new Uri(endpoint),
        new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();
builder.Services.AddSingleton(chatClient);
```

#### AddAIAgent

Register an AI agent with dependency injection:

```csharp
var pirateAgent = builder.AddAIAgent(
    "pirate",
    instructions: "You are a pirate. Speak like a pirate",
    description: "An agent that speaks like a pirate.",
    chatClientServiceKey: "chat-model");
```

The `AddAIAgent()` method returns an `IHostedAgentBuilder`, which provides extension methods for configuring the agent. For example, you can add tools to the agent:

```csharp
var pirateAgent = builder.AddAIAgent("pirate", instructions: "You are a pirate. Speak like a pirate")
    .WithAITool(new MyTool()); // MyTool is a custom type derived from AITool
```

You can also configure the session store (storage for conversation data):

```csharp
var pirateAgent = builder.AddAIAgent("pirate", instructions: "You are a pirate. Speak like a pirate")
    .WithInMemorySessionStore();
```

#### AddWorkflow

Register workflows that coordinate multiple agents. A workflow is essentially a "graph" where each node is an `AIAgent`, and the agents communicate with each other.

In this example, two agents work sequentially. The user input is first sent to `agent-1`, which produces a response and sends it to `agent-2`. The workflow then outputs the final response. There is also a `BuildConcurrent` method that creates a concurrent agent workflow.

```csharp
builder.AddAIAgent("agent-1", instructions: "you are agent 1!");
builder.AddAIAgent("agent-2", instructions: "you are agent 2!");

var workflow = builder.AddWorkflow("my-workflow", (sp, key) =>
{
    var agent1 = sp.GetRequiredKeyedService<AIAgent>("agent-1");
    var agent2 = sp.GetRequiredKeyedService<AIAgent>("agent-2");
    return AgentWorkflowBuilder.BuildSequential(key, [agent1, agent2]);
});
```

#### Expose Workflow as AIAgent

To use protocol integrations (such as A2A or OpenAI) with a workflow, convert it into a standalone agent. Currently, workflows do not provide similar integration capabilities on their own, so this conversion step is required:

```csharp
var workflowAsAgent = builder
    .AddWorkflow("science-workflow", (sp, key) => { ... })
    .AddAsAIAgent();  // Now the workflow can be used as an agent
```

### Implementation Details

The hosting libraries act as protocol adapters that bridge external communication protocols and the Agent Framework's internal `AIAgent` implementation. When you use a hosting integration library, the library retrieves the registered `AIAgent` from dependency injection, wraps it with protocol-specific middleware to translate incoming requests and outgoing responses, and invokes the `AIAgent` to process requests. This architecture keeps your agent implementation protocol-agnostic.

For example, using the ASP.NET Core hosting library with the A2A protocol adapter:

```csharp
// Register the agent
var pirateAgent = builder.AddAIAgent("pirate",
    instructions: "You are a pirate. Speak like a pirate",
    description: "An agent that speaks like a pirate.");

// Expose via a protocol (e.g. A2A)
builder.Services.AddA2AServer();
var app = builder.Build();
app.MapA2AServer();
app.Run();
```

> [!TIP]
> See the [Durable Azure Functions samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/Durable/Agents/AzureFunctions) for serverless hosting examples.

:::zone-end

:::zone pivot="programming-language-python"

Install the Azure Functions hosting package:

```bash
pip install agent-framework-azurefunctions --pre
```

Create an agent:

:::code language="python" source="~/../agent-framework-code/python/samples/04-hosting/azure_functions/01_single_agent/function_app.py" range="13-27" highlight="12-15":::

Register the agent with `AgentFunctionApp`:

:::code language="python" source="~/../agent-framework-code/python/samples/04-hosting/azure_functions/01_single_agent/function_app.py" range="30-31" highlight="2":::

Run locally with [Azure Functions Core Tools](/azure/azure-functions/functions-run-local):

```bash
func start
```

Then invoke:

```bash
curl -X POST http://localhost:7071/api/agents/Joker/run \
  -H "Content-Type: text/plain" \
  -d "Tell me a short joke about cloud computing."
```

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/04-hosting/azure_functions/01_single_agent/function_app.py) for the complete runnable file, and the [Azure Functions hosting samples](https://github.com/microsoft/agent-framework/tree/main/python/samples/04-hosting/azure_functions) for more patterns.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Agents Overview](../agents/index.md)

**Go deeper:**

- [A2A Protocol](../integrations/a2a.md) — expose and consume agents via A2A
- [Azure Functions](../integrations/azure-functions.md) — serverless agent hosting
- [AG-UI Protocol](../integrations/ag-ui/index.md) — web-based agent UIs
- [Foundry Hosted Agents docs](/azure/ai-foundry/agents/concepts/hosted-agents) — understand hosted agents in Azure AI Foundry
- [Foundry Hosted Agents sample (Python)](https://github.com/microsoft-foundry/foundry-samples/tree/main/samples/python/hosted-agents/agent-framework) — run an end-to-end Agent Framework hosted-agent sample

## See also

- [Agents Overview](../agents/index.md)
- [Workflows](../workflows/index.md)
