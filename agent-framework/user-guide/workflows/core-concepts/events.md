---
title: Microsoft Agent Framework Workflows Core Concepts - Events
description: In-depth look at Events in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/12/2025
ms.service: agent-framework
---

# Microsoft Agent Framework Workflows Core Concepts - Events

This document provides an in-depth look at the **Events** system of Workflows in the Microsoft Agent Framework.

## Overview

There are built-in events that provide observability into the workflow execution.

## Built-in Event Types

::: zone pivot="programming-language-csharp"

```csharp
// Workflow lifecycle events
WorkflowStartedEvent    // Workflow execution begins
WorkflowCompletedEvent  // Workflow reaches completion
WorkflowErrorEvent      // Workflow encounters an error

// Executor events  
ExecutorInvokeEvent     // Executor starts processing
ExecutorCompleteEvent   // Executor finishes processing
ExecutorFailureEvent    // Executor encounters an error

// Superstep events
SuperStepStartedEvent   // Superstep begins
SuperStepCompletedEvent // Superstep completes

// Request events
RequestInfoEvent        // A request is issued
```

::: zone-end

::: zone pivot="programming-language-python"

```python
# Workflow lifecycle events
WorkflowStartedEvent    # Workflow execution begins
WorkflowCompletedEvent  # Workflow reaches completion
WorkflowErrorEvent      # Workflow encounters an error

# Executor events
ExecutorInvokeEvent     # Executor starts processing
ExecutorCompleteEvent   # Executor finishes processing

# Request events
RequestInfoEvent        # A request is issued
```

::: zone-end

### Consuming Events

::: zone pivot="programming-language-csharp"

```csharp
await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
    switch (evt)
    {
        case ExecutorInvokeEvent invoke:
            Console.WriteLine($"Starting {invoke.ExecutorId}");
            break;
            
        case ExecutorCompleteEvent complete:
            Console.WriteLine($"Completed {complete.ExecutorId}: {complete.Data}");
            break;
            
        case WorkflowCompletedEvent finished:
            Console.WriteLine($"Workflow finished: {finished.Data}");
            return;
            
        case WorkflowErrorEvent error:
            Console.WriteLine($"Workflow error: {error.Exception}");
            return;
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

```python
async for event in workflow.run_stream(input_message):
    match event:
        case ExecutorInvokeEvent() as invoke:
            print(f"Starting {invoke.executor_id}")
        case ExecutorCompleteEvent() as complete:
            print(f"Completed {complete.executor_id}: {complete.data}")
        case WorkflowCompletedEvent() as finished:
            print(f"Workflow finished: {finished.data}")
            return
        case WorkflowErrorEvent() as error:
            print(f"Workflow error: {error.exception}")
            return
```

::: zone-end

## Custom Events

Users can define and emit custom events during workflow execution for enhanced observability.

::: zone pivot="programming-language-csharp"

```csharp
internal sealed class CustomEvent(string message) : WorkflowEvent(message) { }

internal sealed class CustomExecutor() : ReflectingExecutor<CustomExecutor>("CustomExecutor"), IMessageHandler<string>
{
    public async ValueTask HandleAsync(string message, IWorkflowContext context)
    {
        await context.AddEventAsync(new CustomEvent($"Processing message: {message}"));
        // Executor logic...
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

```python
class CustomEvent(WorkflowEvent):
    def __init__(self, message: str):
        super().__init__(message)

class CustomExecutor(Executor):

    @handler
    async def handle(self, message: str, ctx: WorkflowContext[Any]) -> None:
        await ctx.add_event(CustomEvent(f"Processing message: {message}"))
        # Executor logic...
```

::: zone-end

## Next Steps

- [Learn how to use agents in workflows](./../using-agents.md) to build intelligent workflows.
- [Learn how to use workflows as agents](./../as-agents.md).
- [Learn how to handle requests and responses](./../request-and-response.md) in workflows.
- [Learn how to manage state](./../shared-states.md) in workflows.
- [Learn how to create checkpoints and resume from them](./../checkpoints.md).
