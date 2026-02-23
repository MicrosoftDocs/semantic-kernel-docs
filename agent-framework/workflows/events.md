---
title: Microsoft Agent Framework Workflows - Events
description: In-depth look at Events in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: conceptual
ms.author: taochen
ms.date: 02/12/2026
ms.service: agent-framework
---

# Events

The workflow event system provides observability into workflow execution. Events are emitted at key points during execution and can be consumed in real-time via streaming.

## Built-in Event Types

::: zone pivot="programming-language-csharp"

```csharp
// Workflow lifecycle events
WorkflowStartedEvent     // Workflow execution begins
WorkflowOutputEvent      // Workflow outputs data
WorkflowErrorEvent       // Workflow encounters an error
WorkflowWarningEvent     // Workflow encountered a warning

// Executor events
ExecutorInvokedEvent     // Executor starts processing
ExecutorCompletedEvent   // Executor finishes processing
ExecutorFailedEvent      // Executor encounters an error
AgentResponseEvent       // An agent run produces output
AgentResponseUpdateEvent // An agent run produces a streaming update

// Superstep events
SuperStepStartedEvent    // Superstep begins
SuperStepCompletedEvent  // Superstep completes

// Request events
RequestInfoEvent         // A request is issued
```

::: zone-end

::: zone pivot="programming-language-python"

```python
# All events use the unified WorkflowEvent class with a type discriminator:
WorkflowEvent.type == "started"             # Workflow execution begins
WorkflowEvent.type == "output"              # Workflow produces an output
WorkflowEvent.type == "error"               # Workflow encounters an error
WorkflowEvent.type == "warning"             # Workflow encountered a warning

WorkflowEvent.type == "executor_invoked"    # Executor starts processing
WorkflowEvent.type == "executor_completed"  # Executor finishes processing
WorkflowEvent.type == "executor_failed"     # Executor encounters an error

WorkflowEvent.type == "superstep_started"   # Superstep begins
WorkflowEvent.type == "superstep_completed" # Superstep completes

WorkflowEvent.type == "request_info"        # A request is issued
```

::: zone-end

## Consuming Events

::: zone pivot="programming-language-csharp"

```csharp
using Microsoft.Agents.AI.Workflows;

await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
    switch (evt)
    {
        case ExecutorInvokedEvent invoke:
            Console.WriteLine($"Starting {invoke.ExecutorId}");
            break;

        case ExecutorCompletedEvent complete:
            Console.WriteLine($"Completed {complete.ExecutorId}: {complete.Data}");
            break;

        case WorkflowOutputEvent output:
            Console.WriteLine($"Workflow output: {output.Data}");
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
from agent_framework import WorkflowEvent

async for event in workflow.run_stream(input_message):
    if event.type == "executor_invoked":
        print(f"Starting {event.executor_id}")
    elif event.type == "executor_completed":
        print(f"Completed {event.executor_id}: {event.data}")
    elif event.type == "output":
        print(f"Workflow produced output: {event.data}")
        return
    elif event.type == "error":
        print(f"Workflow error: {event.data}")
        return
```

::: zone-end

## Custom Events

Define and emit custom events during workflow execution for enhanced observability.

::: zone pivot="programming-language-csharp"

```csharp
using Microsoft.Agents.AI.Workflows;

internal sealed class CustomEvent(string message) : WorkflowEvent(message) { }

internal sealed partial class CustomExecutor() : Executor("CustomExecutor")
{
    [MessageHandler]
    private async ValueTask HandleAsync(string message, IWorkflowContext context)
    {
        await context.AddEventAsync(new CustomEvent($"Processing message: {message}"));
        // Executor logic...
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

```python
from agent_framework import (
    handler,
    Executor,
    WorkflowContext,
    WorkflowEvent,
)

class CustomExecutor(Executor):

    @handler
    async def handle(self, message: str, ctx: WorkflowContext[str]) -> None:
        await ctx.add_event(WorkflowEvent("data", data=f"Processing message: {message}"))
        # Executor logic...
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Workflow Builder & Execution](./workflows.md)

**Related topics:**

- [Agents in Workflows](./agents-in-workflows.md)
- [State Management](./state.md)
- [Checkpoints & Resuming](./checkpoints.md)
- [Observability](./observability.md)
