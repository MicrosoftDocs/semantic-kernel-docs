---
title: Create a Simple Concurrent Workflow
description: Learn how to create a simple concurrent workflow using the Agent Framework.
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/29/2025
ms.service: semantic-kernel
---

# Create a Simple Concurrent Workflow

This tutorial demonstrates how to create a concurrent workflow using the Agent Framework. You'll learn to implement fan-out and fan-in patterns that enable parallel processing, allowing multiple executors or agents to work simultaneously and then aggregate their results.

::: zone pivot="programming-language-csharp"

## What You'll Build

You'll create a workflow that:

- Takes a question as input (e.g., "What is temperature?")
- Sends the same question to two expert AI agents simultaneously (Physicist and Chemist)
- Collects and combines responses from both agents into a single output
- Demonstrates concurrent execution with AI agents using fan-out/fan-in patterns

## Prerequisites

- .NET 8.0 or later
- Agent Framework NuGet package: `Microsoft.Agents.AI.Workflows`
- Azure OpenAI access with an endpoint and deployment configured

## Step 1: Setup Dependencies and Azure OpenAI

Start by setting up your project with the required NuGet packages and Azure OpenAI client:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

public static class Program
{
    private static async Task Main()
    {
        // Set up the Azure OpenAI client
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? 
            throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";
        var chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
            .GetChatClient(deploymentName).AsIChatClient();
```

## Step 2: Create Expert AI Agents

Create two specialized AI agents that will provide expert perspectives:

```csharp
        // Create the AI agents with specialized expertise
        ChatClientAgent physicist = new(
            chatClient,
            name: "Physicist",
            instructions: "You are an expert in physics. You answer questions from a physics perspective."
        );
        
        ChatClientAgent chemist = new(
            chatClient,
            name: "Chemist",
            instructions: "You are an expert in chemistry. You answer questions from a chemistry perspective."
        );
```

## Step 3: Create the Start Executor

Create an executor that initiates the concurrent processing by sending input to multiple agents:

```csharp
        var startExecutor = new ConcurrentStartExecutor();
```

The `ConcurrentStartExecutor` implementation:

```csharp
/// <summary>
/// Executor that starts the concurrent processing by sending messages to the agents.
/// </summary>
internal sealed class ConcurrentStartExecutor() :
    ReflectingExecutor<ConcurrentStartExecutor>("ConcurrentStartExecutor"),
    IMessageHandler<string>
{
    /// <summary>
    /// Handles the input string and forwards it to connected agents.
    /// </summary>
    /// <param name="message">The input message to process</param>
    /// <param name="context">Workflow context for sending messages</param>
    public async ValueTask HandleAsync(string message, IWorkflowContext context)
    {
        await context.SendMessageAsync(new ChatMessage(ChatRole.User, message));
    }
}
```

## Step 4: Create the Aggregation Executor

Create an executor that collects and combines responses from multiple agents:

```csharp
        var aggregationExecutor = new ConcurrentAggregationExecutor();
```

The `ConcurrentAggregationExecutor` implementation:

```csharp
/// <summary>
/// Executor that aggregates the results from the concurrent agents.
/// </summary>
internal sealed class ConcurrentAggregationExecutor() :
    ReflectingExecutor<ConcurrentAggregationExecutor>("ConcurrentAggregationExecutor"),
    IMessageHandler<ChatMessage>
{
    private readonly List<ChatMessage> _messages = [];

    /// <summary>
    /// Handles incoming messages from the agents and aggregates their responses.
    /// </summary>
    /// <param name="message">The message from the agent</param>
    /// <param name="context">Workflow context for accessing workflow services and adding events</param>
    public async ValueTask HandleAsync(ChatMessage message, IWorkflowContext context)
    {
        this._messages.Add(message);

        if (this._messages.Count == 2)
        {
            var formattedMessages = string.Join(Environment.NewLine, 
                this._messages.Select(m => $"{m.AuthorName}: {m.Text}"));
            await context.YieldOutputAsync(formattedMessages);
        }
    }
}
```

## Step 5: Build the Workflow

Connect the executors and agents using fan-out and fan-in edge patterns:

```csharp
        // Build the workflow by adding executors and connecting them
        var workflow = new WorkflowBuilder(startExecutor)
            .AddFanOutEdge(startExecutor, targets: [physicist, chemist])
            .AddFanInEdge(aggregationExecutor, sources: [physicist, chemist])
            .WithOutputFrom(aggregationExecutor)
            .Build();
```

## Step 6: Execute the Workflow

Run the workflow and capture the streaming output:

```csharp
        // Execute the workflow in streaming mode
        StreamingRun run = await InProcessExecution.StreamAsync(workflow, "What is temperature?");
        await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
        {
            if (evt is WorkflowOutputEvent output)
            {
                Console.WriteLine($"Workflow completed with results:\n{output.Data}");
            }
        }
    }
}
```

## How It Works

1. **Fan-Out**: The `ConcurrentStartExecutor` receives the input question and the fan-out edge sends it to both the Physicist and Chemist agents simultaneously
2. **Parallel Processing**: Both AI agents process the same question concurrently, each providing their expert perspective
3. **Fan-In**: The `ConcurrentAggregationExecutor` collects `ChatMessage` responses from both agents
4. **Aggregation**: Once both responses are received, the aggregator combines them into a formatted output

## Key Concepts

- **Fan-Out Edges**: Use `AddFanOutEdge()` to distribute the same input to multiple executors or agents
- **Fan-In Edges**: Use `AddFanInEdge()` to collect results from multiple source executors
- **AI Agent Integration**: AI agents can be used directly as executors in workflows
- **ReflectingExecutor**: Base class for creating custom executors with automatic message handling
- **Streaming Execution**: Use `StreamAsync()` to get real-time updates as the workflow progresses

## Complete Implementation

For the complete working implementation of this concurrent workflow with AI agents, see the [Concurrent/Program.cs](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/GettingStarted/Workflows/Concurrent/Program.cs) sample in the Agent Framework repository.

::: zone-end

::: zone pivot="programming-language-python"

In the Python implementation, you'll build a concurrent workflow that processes data through multiple parallel executors and aggregates results of different types. This example demonstrates how the framework handles mixed result types from concurrent processing.

## What You'll Build

You'll create a workflow that:

- Takes a list of numbers as input
- Distributes the list to two parallel executors (one calculating average, one calculating sum)
- Aggregates the different result types (float and int) into a final output
- Demonstrates how the framework handles different result types from concurrent executors

## Prerequisites

- Python 3.10 or later
- Agent Framework Core installed: `pip install agent-framework`

## Step 1: Import Required Dependencies

Start by importing the necessary components from the Agent Framework:

```python
import asyncio
import random

from agent_framework import Executor, WorkflowBuilder, WorkflowContext, WorkflowOutputEvent, handler
from typing_extensions import Never
```

## Step 2: Create the Dispatcher Executor

The dispatcher is responsible for distributing the initial input to multiple parallel executors:

```python
class Dispatcher(Executor):
    """
    The sole purpose of this executor is to dispatch the input of the workflow to
    other executors.
    """

    @handler
    async def handle(self, numbers: list[int], ctx: WorkflowContext[list[int]]):
        if not numbers:
            raise RuntimeError("Input must be a valid list of integers.")

        await ctx.send_message(numbers)
```

## Step 3: Create Parallel Processing Executors

Create two executors that will process the data concurrently:

```python
class Average(Executor):
    """Calculate the average of a list of integers."""

    @handler
    async def handle(self, numbers: list[int], ctx: WorkflowContext[float]):
        average: float = sum(numbers) / len(numbers)
        await ctx.send_message(average)


class Sum(Executor):
    """Calculate the sum of a list of integers."""

    @handler
    async def handle(self, numbers: list[int], ctx: WorkflowContext[int]):
        total: int = sum(numbers)
        await ctx.send_message(total)
```

## Step 4: Create the Aggregator Executor

The aggregator collects results from the parallel executors and yields the final output:

```python
class Aggregator(Executor):
    """Aggregate the results from the different tasks and yield the final output."""

    @handler
    async def handle(self, results: list[int | float], ctx: WorkflowContext[Never, list[int | float]]):
        """Receive the results from the source executors.

        The framework will automatically collect messages from the source executors
        and deliver them as a list.

        Args:
            results (list[int | float]): execution results from upstream executors.
                The type annotation must be a list of union types that the upstream
                executors will produce.
            ctx (WorkflowContext[Never, list[int | float]]): A workflow context that can yield the final output.
        """
        await ctx.yield_output(results)
```

## Step 5: Build the Workflow

Connect the executors using fan-out and fan-in edge patterns:

```python
async def main() -> None:
    # 1) Create the executors
    dispatcher = Dispatcher(id="dispatcher")
    average = Average(id="average")
    summation = Sum(id="summation")
    aggregator = Aggregator(id="aggregator")

    # 2) Build a simple fan out and fan in workflow
    workflow = (
        WorkflowBuilder()
        .set_start_executor(dispatcher)
        .add_fan_out_edges(dispatcher, [average, summation])
        .add_fan_in_edges([average, summation], aggregator)
        .build()
    )
```

## Step 6: Run the Workflow

Execute the workflow with sample data and capture the output:

```python
    # 3) Run the workflow
    output: list[int | float] | None = None
    async for event in workflow.run_stream([random.randint(1, 100) for _ in range(10)]):
        if isinstance(event, WorkflowOutputEvent):
            output = event.data

    if output is not None:
        print(output)

if __name__ == "__main__":
    asyncio.run(main())
```

## How It Works

1. **Fan-Out**: The `Dispatcher` receives the input list and sends it to both the `Average` and `Sum` executors simultaneously
2. **Parallel Processing**: Both executors process the same input concurrently, producing different result types:
   - `Average` executor produces a `float` result
   - `Sum` executor produces an `int` result
3. **Fan-In**: The `Aggregator` receives results from both executors as a list containing both types
4. **Type Handling**: The framework automatically handles the different result types using union types (`int | float`)

## Key Concepts

- **Fan-Out Edges**: Use `add_fan_out_edges()` to send the same input to multiple executors
- **Fan-In Edges**: Use `add_fan_in_edges()` to collect results from multiple source executors
- **Union Types**: Handle different result types using type annotations like `list[int | float]`
- **Concurrent Execution**: Multiple executors process data simultaneously, improving performance

## Complete Implementation

For the complete working implementation of this concurrent workflow, see the [aggregate_results_of_different_types.py](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/workflow/parallelism/aggregate_results_of_different_types.py) sample in the Agent Framework repository.

::: zone-end

## Next Steps

> [!div class="nextstepaction"]
> [Learn about using agents in workflows](agents-in-workflows.md)
