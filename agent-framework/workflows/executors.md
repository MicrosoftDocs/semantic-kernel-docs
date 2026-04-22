---
title: Microsoft Agent Framework Workflows - Executors
description: In-depth look at Executors in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: conceptual
ms.author: taochen
ms.date: 04/22/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                    | C# | Python | Notes            |
  |----------------------------|:--:|:------:|------------------|
  | Basic Executor Structure   | ✅ |   ✅   |                  |
  | Resettable Executors (TIP) | ✅ |   ❌   | C#-specific; links to advanced page |
  | Multiple Input Types       | ✅ |   ✅   |                  |
  | Function-Based Executors   | ✅ |   ✅   |                  |
  | Explicit Type Parameters   | ❌ |   ✅   | Python-specific  |
  | The WorkflowContext Object | ✅ |   ✅   |                  |
-->

# Executors

Executors are the fundamental building blocks that process messages in a workflow. They are autonomous processing units that receive typed messages, perform operations, and can produce output messages or events.

## Overview

Each executor has a unique identifier and can handle specific message types. Executors can be:

- **Custom logic components** — process data, call APIs, or transform messages
- **AI agents** — use LLMs to generate responses (see [Agents in Workflows](./agents-in-workflows.md))

::: zone pivot="programming-language-csharp"

> [!IMPORTANT]
> The recommended way to define executor message handlers in C# is to use the `[MessageHandler]` attribute on methods within a `partial` class that derives from `Executor`. This uses compile-time source generation for handler registration, providing better performance, compile-time validation, and Native AOT compatibility.

## Basic Executor Structure

Executors derive from the `Executor` base class and use the `[MessageHandler]` attribute to declare handler methods. The class must be marked `partial` to enable source generation.

```csharp
using Microsoft.Agents.AI.Workflows;

internal sealed partial class UppercaseExecutor() : Executor("UppercaseExecutor")
{
    [MessageHandler]
    private ValueTask<string> HandleAsync(string message, IWorkflowContext context)
    {
        string result = message.ToUpperInvariant();
        return ValueTask.FromResult(result); // Return value is automatically sent to connected executors
    }
}
```

You can also send messages manually without returning a value:

```csharp
internal sealed partial class UppercaseExecutor() : Executor("UppercaseExecutor")
{
    [MessageHandler]
    private async ValueTask HandleAsync(string message, IWorkflowContext context)
    {
        string result = message.ToUpperInvariant();
        await context.SendMessageAsync(result); // Manually send messages to connected executors
    }
}
```

> [!TIP]
> Executors can hold mutable state. If a stateful executor is shared across workflow runs, it must implement `IResettableExecutor` to clear stale state between runs. See [Resettable Executors](./advanced/resettable-executors.md) for details.

## Multiple Input Types

Handle multiple input types by defining multiple `[MessageHandler]` methods:

```csharp
internal sealed partial class SampleExecutor() : Executor("SampleExecutor")
{
    [MessageHandler]
    private ValueTask<string> HandleStringAsync(string message, IWorkflowContext context)
    {
        return ValueTask.FromResult(message.ToUpperInvariant());
    }

    [MessageHandler]
    private ValueTask<int> HandleIntAsync(int message, IWorkflowContext context)
    {
        return ValueTask.FromResult(message * 2);
    }
}
```

## Function-Based Executors

Create an executor from a function using the `BindExecutor` extension method:

```csharp
Func<string, string> uppercaseFunc = s => s.ToUpperInvariant();
var uppercase = uppercaseFunc.BindExecutor("UppercaseExecutor");
```

## The IWorkflowContext Object

The `IWorkflowContext` provides methods for interacting with the workflow during execution:

- **`SendMessageAsync`** — send messages to connected executors
- **`YieldOutputAsync`** — produce workflow outputs returned/streamed to the caller

```csharp
internal sealed partial class OutputExecutor() : Executor("OutputExecutor")
{
    [MessageHandler]
    private async ValueTask HandleAsync(string message, IWorkflowContext context)
    {
        await context.YieldOutputAsync("Hello, World!");
    }
}
```

If a handler neither sends messages nor yields outputs, it can simply perform side effects:

```csharp
internal sealed partial class LogExecutor() : Executor("LogExecutor")
{
    [MessageHandler]
    private void Handle(string message, IWorkflowContext context)
    {
        Console.WriteLine("Doing some work...");
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

## Basic Executor Structure

Executors inherit from the `Executor` base class. Each executor uses methods decorated with the `@handler` decorator. Handlers must have proper type annotations to specify the message types they process.

```python
from agent_framework import (
    Executor,
    WorkflowContext,
    handler,
)

class UpperCase(Executor):

    @handler
    async def to_upper_case(self, text: str, ctx: WorkflowContext[str]) -> None:
        """Convert the input to uppercase and forward it to the next node."""
        await ctx.send_message(text.upper())
```

## Function-Based Executors

Create an executor from a function using the `@executor` decorator:

```python
from agent_framework import (
    WorkflowContext,
    executor,
)

@executor(id="upper_case_executor")
async def upper_case(text: str, ctx: WorkflowContext[str]) -> None:
    """Convert the input to uppercase and forward it to the next node."""
    await ctx.send_message(text.upper())
```

## Multiple Input Types

Handle multiple input types by defining multiple handlers:

```python
class SampleExecutor(Executor):

    @handler
    async def to_upper_case(self, text: str, ctx: WorkflowContext[str]) -> None:
        await ctx.send_message(text.upper())

    @handler
    async def double_integer(self, number: int, ctx: WorkflowContext[int]) -> None:
        await ctx.send_message(number * 2)
```

## Explicit Type Parameters

As an alternative to type annotations, you can specify types explicitly via decorator parameters:

> [!IMPORTANT]
> When using explicit type parameters, you must specify **all** types via the decorator — you cannot mix explicit parameters with type annotations. The `input` parameter is required; `output` and `workflow_output` are optional.

```python
class ExplicitTypesExecutor(Executor):

    @handler(input=str, output=str)
    async def to_upper_case(self, text, ctx) -> None:
        await ctx.send_message(text.upper())

    @handler(input=str | int, output=str)
    async def handle_mixed(self, message, ctx) -> None:
        await ctx.send_message(str(message).upper())

    @handler(input=str, output=int, workflow_output=bool)
    async def process_with_workflow_output(self, message, ctx) -> None:
        await ctx.send_message(len(message))
        await ctx.yield_output(True)
```

## The WorkflowContext Object

The `WorkflowContext` provides methods for interacting with the workflow during execution:

- **`send_message`** — send messages to connected executors
- **`yield_output`** — produce workflow outputs returned/streamed to the caller

```python
class OutputExecutor(Executor):

    @handler
    async def handle(self, message: str, ctx: WorkflowContext[Never, str]) -> None:
        await ctx.yield_output("Hello, World!")
```

If a handler neither sends messages nor yields outputs, no type parameter is needed:

```python
class LogExecutor(Executor):

    @handler
    async def handle(self, message: str, ctx: WorkflowContext) -> None:
        print("Doing some work...")
```

::: zone-end

::: zone pivot="programming-language-go"
## Executors

Executors are the processing units in a workflow. They receive input, perform work, and produce output.

### Function executors

The simplest way to create an executor is with `workflow.BindFunc`:

```go
uppercase := workflow.BindFunc("UppercaseExecutor", true, func(input string) string {
    return strings.ToUpper(input)
})
```

The second parameter (`true`) indicates that the executor auto-completes (returns a result immediately).

### Agent executors

Agents can be used as workflow executors via `agent.Bind`:

```go
agentExecutor := myAgent.Bind(false) // false = manual completion via events
```

### Executor lifecycle

Executors support lifecycle hooks through `workflow.ExecutorConfig`:

| Hook | Purpose |
|---|---|
| `ConfigureRoutes` | Set up message routing |
| `Initialize` | Setup when workflow starts |
| `Reset` | Reset state between runs |
| `OnCheckpoint` | Save state at checkpoint |
| `OnCheckpointRestored` | Restore state from checkpoint |

::: zone-end
## Next steps

> [!div class="nextstepaction"]
> [Edges](./edges.md)
