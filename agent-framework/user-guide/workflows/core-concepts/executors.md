---
title: Microsoft Agent Framework Workflows Core Concepts - Executors
description: In-depth look at Executors in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/12/2025
ms.service: semantic-kernel
---

# Microsoft Agent Framework Workflows Core Concepts - Executors

This document provides an in-depth look at the **Executors** component of the Microsoft Agent Framework Workflow system.

## Overview

Executors are the fundamental building blocks that process messages in a workflow. They are autonomous processing units that receive typed messages, perform operations, and can produce output messages or events.

::: zone pivot="programming-language-csharp"

Executors implement the `IMessageHandler<TInput>` or `IMessageHandler<TInput, TOutput>` interfaces and inherit from the `ReflectingExecutor<T>` base class. Each executor has a unique identifier and can handle specific message types.

### Basic Executor Structure

```csharp
using Microsoft.Agents.Workflows;
using Microsoft.Agents.Workflows.Reflection;

internal sealed class UppercaseExecutor() : ReflectingExecutor<UppercaseExecutor>("UppercaseExecutor"),
    IMessageHandler<string, string>
{
    public async ValueTask<string> HandleAsync(string message, IWorkflowContext context)
    {
        string result = message.ToUpperInvariant();
        return result; // Return value is automatically sent to connected executors
    }
}
```

It is possible to send messages manually without returning a value:

```csharp
internal sealed class UppercaseExecutor() : ReflectingExecutor<UppercaseExecutor>("UppercaseExecutor"),
    IMessageHandler<string>
{
    public async ValueTask HandleAsync(string message, IWorkflowContext context)
    {
        string result = message.ToUpperInvariant();
        await context.SendMessageAsync(result); // Manually send messages to connected executors
    }
}
```

It is also possible to handle multiple input types by implementing multiple interfaces:

```csharp
internal sealed class SampleExecutor() : ReflectingExecutor<SampleExecutor>("SampleExecutor"),
    IMessageHandler<string, string>, IMessageHandler<int, int>
{
    /// <summary>
    /// Converts input string to uppercase
    /// </summary>
    public async ValueTask<string> HandleAsync(string message, IWorkflowContext context)
    {
        string result = message.ToUpperInvariant();
        return result;
    }

    /// <summary>
    /// Doubles the input integer
    /// </summary>
    public async ValueTask<int> HandleAsync(int message, IWorkflowContext context)
    {
        int result = message * 2;
        return result;
    }
}
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

::: zone-end

## Next Step

- [Learn about Edges](./edges.md) to understand how executors are connected in a workflow.
