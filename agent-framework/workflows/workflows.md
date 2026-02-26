---
title: Microsoft Agent Framework Workflows - Workflow Builder & Execution
description: Building and executing workflows with the WorkflowBuilder.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: conceptual
ms.author: taochen
ms.date: 02/12/2026
ms.service: agent-framework
---

# Workflow Builder & Execution

A Workflow ties [executors](./executors.md) and [edges](./edges.md) together into a directed graph and manages execution. It coordinates executor invocation, message routing, and event streaming.

## Building Workflows

::: zone pivot="programming-language-csharp"

Workflows are constructed using the `WorkflowBuilder` class, which provides a fluent API for defining the workflow structure:

```csharp
using Microsoft.Agents.AI.Workflows;

var processor = new DataProcessor();
var validator = new Validator();
var formatter = new Formatter();

// Build workflow
WorkflowBuilder builder = new(processor); // Set starting executor
builder.AddEdge(processor, validator);
builder.AddEdge(validator, formatter);
var workflow = builder.Build<string>(); // Specify input message type
```

::: zone-end

::: zone pivot="programming-language-python"

Workflows are constructed using the `WorkflowBuilder` class:

```python
from agent_framework import WorkflowBuilder

processor = DataProcessor()
validator = Validator()
formatter = Formatter()

# Build workflow
builder = WorkflowBuilder(start_executor=processor)
builder.add_edge(processor, validator)
builder.add_edge(validator, formatter)
workflow = builder.build()
```

::: zone-end

## Workflow Execution

Workflows support both streaming and non-streaming execution modes:

::: zone pivot="programming-language-csharp"

```csharp
using Microsoft.Agents.AI.Workflows;

// Streaming execution — get events as they happen
StreamingRun run = await InProcessExecution.StreamAsync(workflow, inputMessage);
await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
    if (evt is ExecutorCompleteEvent executorComplete)
    {
        Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
    }

    if (evt is WorkflowOutputEvent outputEvt)
    {
        Console.WriteLine($"Workflow completed: {outputEvt.Data}");
    }
}

// Non-streaming execution — wait for completion
Run result = await InProcessExecution.RunAsync(workflow, inputMessage);
foreach (WorkflowEvent evt in result.NewEvents)
{
    if (evt is WorkflowOutputEvent outputEvt)
    {
        Console.WriteLine($"Final result: {outputEvt.Data}");
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

```python
# Streaming execution — get events as they happen
async for event in workflow.run_stream(input_message):
    if event.type == "output":
        print(f"Workflow completed: {event.data}")

# Non-streaming execution — wait for completion
events = await workflow.run(input_message)
print(f"Final result: {events.get_outputs()}")
```

::: zone-end

## Workflow Validation

The framework performs comprehensive validation when building workflows:

- **Type Compatibility**: Ensures message types are compatible between connected executors
- **Graph Connectivity**: Verifies all executors are reachable from the start executor
- **Executor Binding**: Confirms all executors are properly bound and instantiated
- **Edge Validation**: Checks for duplicate edges and invalid connections

## Execution Model: Supersteps

The framework uses a modified [Pregel](https://kowshik.github.io/JPregel/pregel_paper.pdf) execution model — a Bulk Synchronous Parallel (BSP) approach with superstep-based processing.

### How Supersteps Work

Workflow execution is organized into discrete supersteps. Each superstep:

1. Collects all pending messages from the previous superstep
2. Routes messages to target executors based on edge definitions
3. Runs all target executors concurrently within the superstep
4. Waits for all executors to complete before advancing (synchronization barrier)
5. Queues any new messages emitted by executors for the next superstep

```text
Superstep N:
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  Collect All    │───▶│  Route Messages │───▶│  Execute All    │
│  Pending        │    │  Based on Type  │    │  Target         │
│  Messages       │    │  & Conditions   │    │  Executors      │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                                       │
                                                       │ (barrier: wait for all)
┌─────────────────┐    ┌─────────────────┐             │
│  Start Next     │◀───│  Emit Events &  │◀────────────┘
│  Superstep      │    │  New Messages   │
└─────────────────┘    └─────────────────┘
```

### Synchronization Barrier

The most important characteristic is the synchronization barrier between supersteps. Within a single superstep, all triggered executors run in parallel, but the workflow does not advance to the next superstep until every executor completes.

This affects fan-out patterns: if you fan out to multiple paths — one with a chain of executors and another with a single long-running executor — the chained path cannot advance until the long-running executor completes.

### Why Supersteps?

The BSP model provides important guarantees:

- **Deterministic execution**: Given the same input, the workflow always executes in the same order
- **Reliable checkpointing**: State can be saved at superstep boundaries for fault tolerance
- **Simpler reasoning**: No race conditions between supersteps; each sees a consistent view of messages

### Working with the Superstep Model

If you need truly independent parallel paths that don't block each other, consolidate sequential steps into a single executor. Instead of chaining `step1 → step2 → step3`, combine that logic into one executor. Both parallel paths then execute within a single superstep.

## Next steps

> [!div class="nextstepaction"]
> [Agents in Workflows](./agents-in-workflows.md)

**Related topics:**

- [Executors](./executors.md) — processing units in a workflow
- [Edges](./edges.md) — connections between executors
- [Events](./events.md) — workflow observability
- [State Management](./state.md)
