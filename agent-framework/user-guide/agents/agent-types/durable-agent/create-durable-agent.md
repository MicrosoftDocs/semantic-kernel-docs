---
title: Durable Agents
description: Learn how to use the durable task extension for Microsoft Agent Framework to build stateful AI agents with automatic state management.
zone_pivot_groups: programming-languages
author: anthonychu
ms.topic: tutorial
ms.author: antchu
ms.date: 11/05/2025
ms.service: agent-framework
---

# Durable Agents

The durable task extension for Microsoft Agent Framework enables you to build stateful AI agents and multi-agent deterministic orchestrations. Durable agents support flexible hosting options: use Azure Functions or Azure Container Apps for serverless scenarios with automatic HTTP endpoints, or integrate with any hosting environment such as Kubernetes or your own application servers.

The durable task extension provides durable state management, meaning your agent's conversation history and execution state are reliably persisted and survive failures, restarts, and long-running operations.

The extension manages agent thread state and orchestration coordination, allowing you to focus on your agent logic instead of infrastructure concerns for reliability.

## Key Features

The durable task extension provides the following key features:

- **Flexible hosting**: Deploy and host agents on Azure Functions or Azure Container Apps for serverless scenarios, or integrate with any hosting environment such as Kubernetes or your own application servers
- **Stateful agent threads**: Maintain persistent threads with conversation history that survive across multiple interactions
- **Deterministic orchestrations**: Coordinate multiple agents reliably with fault-tolerant workflows that can run for days or weeks, supporting sequential, parallel, and human-in-the-loop patterns
- **Observability and debugging**: Visualize agent conversations, orchestration flows, and execution history through the built-in Durable Task Scheduler dashboard

## Getting Started

::: zone pivot="programming-language-csharp"

Choose the package based on your hosting environment:

### Azure Functions Extension

For serverless hosting with Azure Functions, add the following NuGet packages:

```bash
dotnet add package Azure.AI.OpenAI --prerelease
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
dotnet add package Microsoft.Agents.AI.Hosting.AzureFunctions --prerelease
```

### Durable Task package

For hosting outside of Azure Functions (such as ASP.NET Core, console applications, or other hosting environments), add the core Durable Task package:

```bash
dotnet add package Azure.AI.OpenAI --prerelease
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
dotnet add package Microsoft.Agents.AI.DurableTask --prerelease
```

The `Microsoft.Agents.AI.DurableTask` package provides the core durable agent functionality that works with any Durable Task backend.

::: zone-end

::: zone pivot="programming-language-python"

Choose the package based on your hosting environment:

### Azure Functions Extension

For serverless hosting with Azure Functions, install the Azure Functions-specific package:

```bash
pip install azure-identity
pip install agent-framework-azurefunctions --pre
```

### Durable Task package

For hosting outside of Azure Functions (such as Azure Container Apps, Azure Kubernetes Services, App Services, or other hosting environments), install the core Durable Task package:

```bash
pip install azure-identity
pip install agent-framework-durabletask --pre
```

The `agent-framework-durabletask` package provides the core durable agent functionality that works with any Durable Task backend.

::: zone-end

## Hosting Options

The durable task extension supports multiple hosting environments. Choose the option that best fits your deployment requirements.

### Azure Functions Extension

With the Azure Functions Extension package, you can deploy and host Microsoft Agent Framework agents in Azure Functions with built-in HTTP endpoints and orchestration-based invocation. Azure Functions provides event-driven, pay-per-invocation pricing with automatic scaling and minimal infrastructure management.

When you configure a durable agent with the Azure Functions Extension, it automatically creates HTTP endpoints for your agent and manages all the underlying infrastructure for storing conversation state, handling concurrent requests, and coordinating multi-agent workflows.

::: zone pivot="programming-language-csharp"

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AzureFunctions;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT") ?? "gpt-4o-mini";

// Create an AI agent following the standard Microsoft Agent Framework pattern
AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(
        instructions: "You are good at telling jokes.",
        name: "Joker");

// Configure the function app to host the agent with durable thread management
// This automatically creates HTTP endpoints and manages state persistence
using IHost app = FunctionsApplication
    .CreateBuilder(args)
    .ConfigureFunctionsWebApplication()
    .ConfigureDurableAgents(options => options.AddAIAgent(agent))
    .Build();
app.Run();
```

::: zone-end

::: zone pivot="programming-language-python"

```python
from agent_framework.azure import AgentFunctionApp, AzureOpenAIChatClient
from azure.identity import DefaultAzureCredential

# Create an AI agent following the standard Microsoft Agent Framework pattern
agent = AzureOpenAIChatClient(credential=DefaultAzureCredential()).as_agent(
    name="Joker",
    instructions="You are good at telling jokes.",
)

# Configure the function app to host the agent with durable thread management
# This automatically creates HTTP endpoints and manages state persistence
app = AgentFunctionApp(agents=[agent])
```

::: zone-end

### Durable Task Package

For scenarios where you want to host durable agents outside of Azure Functions, use the Durable Task package. This gives you the same durable state management capabilities while allowing you to integrate with any hosting environment such as Azure Container Apps, Kubernetes, or custom applications.

::: zone pivot="programming-language-csharp"

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DurableTask;
using Microsoft.DurableTask.Client.AzureManaged;
using Microsoft.DurableTask.Worker.AzureManaged;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT") ?? "gpt-4o-mini";
var dtsConnectionString = Environment.GetEnvironmentVariable("DURABLE_TASK_SCHEDULER_CONNECTION_STRING");

// Create an AI agent following the standard Microsoft Agent Framework pattern
AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(
        instructions: "You are good at telling jokes.",
        name: "Joker");

// Configure the host to run the durable agent worker
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.ConfigureDurableAgents(
            options => options.AddAIAgent(agent),
            workerBuilder: builder => builder.UseDurableTaskScheduler(dtsConnectionString),
            clientBuilder: builder => builder.UseDurableTaskScheduler(dtsConnectionString));
    })
    .Build();

await host.RunAsync();
```

::: zone-end

::: zone pivot="programming-language-python"

```python
import os
from agent_framework.azure import AzureOpenAIChatClient, DurableAIAgentWorker
from azure.identity import DefaultAzureCredential
from durabletask.azuremanaged.worker import DurableTaskSchedulerWorker

# Create an AI agent following the standard Microsoft Agent Framework pattern
agent = AzureOpenAIChatClient(credential=DefaultAzureCredential()).as_agent(
    name="Joker",
    instructions="You are good at telling jokes.",
)

# Configure the Durable Task Scheduler worker
dts_endpoint = os.environ["DURABLE_TASK_SCHEDULER_ENDPOINT"]
dts_taskhub = os.environ.get("DURABLE_TASK_SCHEDULER_TASKHUB", "default")

worker = DurableTaskSchedulerWorker(
    host_address=dts_endpoint,
    secure_channel=True,
    taskhub=dts_taskhub,
    credential=DefaultAzureCredential(),
)

# Wrap with the agent worker and register the agent
agent_worker = DurableAIAgentWorker(worker)
agent_worker.add_agent(agent)

# Start the worker
await agent_worker.start()
```

::: zone-end

With the core Durable Task package, you need to implement your own HTTP endpoints or other interfaces for agent interaction, but you gain full control over your hosting environment.

### When to Use Each Hosting Option

**Choose Azure Functions or Azure Container Apps hosting when you need:**

- Serverless, pay-per-invocation pricing
- Automatic HTTP endpoints without custom code
- Automatic scaling to zero when not in use
- Minimal infrastructure management

**Choose the core Durable Task package when you need:**

- Custom hosting environments (Kubernetes, VMs, other containers)
- Integration with existing web frameworks (ASP.NET Core, Flask, FastAPI)
- Full control over HTTP routing and middleware

### When to Use Durable Agents

Choose durable agents when you need:

- **Full code control**: Deploy and manage your own compute environment with your choice of hosting platform
- **Complex orchestrations**: Coordinate multiple agents with deterministic, reliable workflows that can run for days or weeks
- **Event-driven orchestration**: Integrate with triggers (HTTP, timers, queues, etc.) for event-driven agent workflows
- **Automatic conversation state**: Agent conversation history is automatically managed and persisted without requiring explicit state handling in your code

Durable agents are ideal when you need the flexibility of code-first deployment combined with the reliability of durable state management. Whether you choose serverless hosting with Azure Functions or self-hosted deployments with the core Durable Task package, you get the same powerful state management capabilities.

When hosted in the [Azure Functions Flex Consumption](/azure/azure-functions/flex-consumption-plan) hosting plan, agents can scale to thousands of instances or to zero instances when not in use, allowing you to pay only for the compute you need.

## Stateful Agent Threads with Conversation History

Agents maintain persistent threads that survive across multiple interactions. Each thread is identified by a unique thread ID and stores the complete conversation history in durable storage managed by the [Durable Task Scheduler](/azure/azure-functions/durable/durable-task-scheduler/durable-task-scheduler).

This pattern enables conversational continuity where agent state is preserved through process crashes and restarts, allowing full conversation history to be maintained across user threads. The durable storage ensures that even if your hosting instance restarts or scales to a different instance, the conversation seamlessly continues from where it left off.

The following example demonstrates multiple HTTP requests to the same thread when using Azure Functions hosting, showing how conversation context persists:

```bash
# First interaction - start a new thread
curl -X POST https://your-function-app.azurewebsites.net/api/agents/Joker/run \
  -H "Content-Type: text/plain" \
  -d "Tell me a joke about pirates"

# Response includes thread ID in x-ms-thread-id header and joke as plain text
# HTTP/1.1 200 OK
# Content-Type: text/plain
# x-ms-thread-id: @dafx-joker@263fa373-fa01-4705-abf2-5a114c2bb87d
#
# Why don't pirates shower before they walk the plank? Because they'll just wash up on shore later!

# Second interaction - continue the same thread with context
curl -X POST "https://your-function-app.azurewebsites.net/api/agents/Joker/run?thread_id=@dafx-joker@263fa373-fa01-4705-abf2-5a114c2bb87d" \
  -H "Content-Type: text/plain" \
  -d "Tell me another one about the same topic"

# Agent remembers the pirate context from the first message and responds with plain text
# What's a pirate's favorite letter? You'd think it's R, but it's actually the C!
```

Agent state is maintained in durable storage, enabling distributed execution across multiple instances. Any instance can resume an agent's execution after interruptions or failures, ensuring continuous operation.

## Next Steps

Learn about advanced capabilities of the durable task extension:

> [!div class="nextstepaction"]
> [Durable Agent Features](features.md)

For a step-by-step tutorial on building and running a durable agent:

> [!div class="nextstepaction"]
> [Create and run a durable agent](../../../../tutorials/agents/create-and-run-durable-agent.md)

## Related Content

- [Durable Task Scheduler Overview](/azure/azure-functions/durable/durable-task-scheduler/durable-task-scheduler)
- [Azure Functions Flex Consumption Plan](/azure/azure-functions/flex-consumption-plan)
- [Microsoft Agent Framework Overview](../../../../overview/agent-framework-overview.md)
