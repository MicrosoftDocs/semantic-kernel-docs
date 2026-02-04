---
title: Microsoft Agent Framework Workflows Core Concepts - Executors
description: In-depth look at Executors in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/12/2025
ms.service: agent-framework
---

# Microsoft Agent Framework Workflows Core Concepts - Executors

This document provides an in-depth look at the **Executors** component of the Microsoft Agent Framework Workflow system.

## Overview

Executors are the fundamental building blocks that process messages in a workflow. They are autonomous processing units that receive typed messages, perform operations, and can produce output messages or events.

::: zone pivot="programming-language-csharp"

> [!IMPORTANT]
> Starting with this release, the recommended way to define executor message handlers in C# is to use the `[MessageHandler]` attribute on methods within a `partial` class that derives from `Executor`. This replaces the previous patterns of inheriting from `Executor<TInput>`, `Executor<TInput, TOutput>`, or `ReflectingExecutor<T>` with `IMessageHandler<T>` interfaces. The new approach uses compile-time source generation for handler registration, providing better performance, compile-time validation, and Native AOT compatibility. The previous patterns are deprecated and will be removed in a future version.

Executors derive from the `Executor` base class and use the `[MessageHandler]` attribute to declare handler methods. The class must be marked `partial` to enable source generation. Each executor has a unique identifier and can handle specific message types.

### Basic Executor Structure

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

It is possible to send messages manually without returning a value:

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

It is also possible to handle multiple input types by defining multiple `[MessageHandler]` methods:

```csharp
internal sealed partial class SampleExecutor() : Executor("SampleExecutor")
{
    /// <summary>
    /// Converts input string to uppercase
    /// </summary>
    [MessageHandler]
    private ValueTask<string> HandleStringAsync(string message, IWorkflowContext context)
    {
        return ValueTask.FromResult(message.ToUpperInvariant());
    }

    /// <summary>
    /// Doubles the input integer
    /// </summary>
    [MessageHandler]
    private ValueTask<int> HandleIntAsync(int message, IWorkflowContext context)
    {
        return ValueTask.FromResult(message * 2);
    }
}
```

It is also possible to create an executor from a function by using the `BindExecutor` extension method:

```csharp
Func<string, string> uppercaseFunc = s => s.ToUpperInvariant();
var uppercase = uppercaseFunc.BindExecutor("UppercaseExecutor");
```

::: zone-end

::: zone pivot="programming-language-python"

Executors inherit from the `Executor` base class. Each executor has a unique identifier and can handle specific message types using methods decorated with the `@handler` decorator. Handlers must have the proper annotation to specify the type of messages they can process.

### Basic Executor Structure

```python
from agent_framework import (
    Executor,
    WorkflowContext,
    handler,
)

class UpperCase(Executor):

    @handler
    async def to_upper_case(self, text: str, ctx: WorkflowContext[str]) -> None:
        """Convert the input to uppercase and forward it to the next node.

        Note: The WorkflowContext is parameterized with the type this handler will
        emit. Here WorkflowContext[str] means downstream nodes should expect str.
        """
        await ctx.send_message(text.upper())
```

It is possible to create an executor from a function by using the `@executor` decorator:

```python
from agent_framework import (
    WorkflowContext,
    executor,
)

@executor(id="upper_case_executor")
async def upper_case(text: str, ctx: WorkflowContext[str]) -> None:
    """Convert the input to uppercase and forward it to the next node.

    Note: The WorkflowContext is parameterized with the type this handler will
    emit. Here WorkflowContext[str] means downstream nodes should expect str.
    """
    await ctx.send_message(text.upper())
```

It is also possible to handle multiple input types by defining multiple handlers:

```python
class SampleExecutor(Executor):

    @handler
    async def to_upper_case(self, text: str, ctx: WorkflowContext[str]) -> None:
        """Convert the input to uppercase and forward it to the next node.

        Note: The WorkflowContext is parameterized with the type this handler will
        emit. Here WorkflowContext[str] means downstream nodes should expect str.
        """
        await ctx.send_message(text.upper())

    @handler
    async def double_integer(self, number: int, ctx: WorkflowContext[int]) -> None:
        """Double the input integer and forward it to the next node.

        Note: The WorkflowContext is parameterized with the type this handler will
        emit. Here WorkflowContext[int] means downstream nodes should expect int.
        """
        await ctx.send_message(number * 2)
```

### The `WorkflowContext` Object

The `WorkflowContext` object provides methods for the handler to interact with the workflow during execution. The `WorkflowContext` is parameterized with the type of messages the handler will emit and the type of outputs it can yield.

The most commonly used method is `send_message`, which allows the handler to send messages to connected executors.

```python
from agent_framework import WorkflowContext

class SomeHandler(Executor):

    @handler
    async def some_handler(message: str, ctx: WorkflowContext[str]) -> None:
        await ctx.send_message("Hello, World!")
```

A handler can use `yield_output` to produce outputs that will be considered as workflow outputs and be returned/streamed to the caller as an output event:

```python
from agent_framework import WorkflowContext

class SomeHandler(Executor):

    @handler
    async def some_handler(message: str, ctx: WorkflowContext[Never, str]) -> None:
        await ctx.yield_output("Hello, World!")
```

If a handler neither sends messages nor yields outputs, no type parameter is needed for `WorkflowContext`:

```python
from agent_framework import WorkflowContext

class SomeHandler(Executor):

    @handler
    async def some_handler(message: str, ctx: WorkflowContext) -> None:
        print("Doing some work...")
```

::: zone-end

## Next Step

- [Learn about Edges](./edges.md) to understand how executors are connected in a workflow.
