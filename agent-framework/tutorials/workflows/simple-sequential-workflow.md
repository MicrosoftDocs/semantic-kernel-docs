---
title: Create a Simple Sequential Workflow
description: Learn how to create a simple sequential workflow using the Agent Framework.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/29/2025
ms.service: agent-framework
---

# Create a Simple Sequential Workflow

This tutorial demonstrates how to create a simple sequential workflow using the Agent Framework Workflows.

Sequential workflows are the foundation of building complex AI agent systems. This tutorial shows how to create a simple two-step workflow where each step processes data and passes it to the next step.

::: zone pivot="programming-language-csharp"

## Overview

In this tutorial, you'll create a workflow with two executors:

1. **Uppercase Executor** - Converts input text to uppercase
2. **Reverse Text Executor** - Reverses the text and outputs the final result

The workflow demonstrates core concepts like:

- Creating custom executors that implement `IMessageHandler<TInput, TOutput>`
- Using `WorkflowBuilder` to connect executors with edges
- Processing data through sequential steps
- Observing workflow execution through events

## Prerequisites

- .NET 9.0 or later
- Microsoft.Agents.AI.Workflows NuGet package
- No external AI services required for this basic example

## Step-by-Step Implementation

Let's build the sequential workflow step by step.

### Step 1: Add Required Using Statements

First, add the necessary using statements:

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
```

### Step 2: Create the Uppercase Executor

Create an executor that converts text to uppercase:

```csharp
/// <summary>
/// First executor: converts input text to uppercase.
/// </summary>
internal sealed class UppercaseExecutor() : ReflectingExecutor<UppercaseExecutor>("UppercaseExecutor"), 
    IMessageHandler<string, string>
{
    public ValueTask<string> HandleAsync(string input, IWorkflowContext context)
    {
        // Convert input to uppercase and pass to next executor
        return ValueTask.FromResult(input.ToUpper());
    }
}
```

**Key Points:**

- Inherits from `ReflectingExecutor<T>` for basic executor functionality
- Implements `IMessageHandler<string, string>` - takes string input, produces string output
- The `HandleAsync` method processes the input and returns the result
- Result is automatically passed to the next connected executor

### Step 3: Create the Reverse Text Executor

Create an executor that reverses the text:

```csharp
/// <summary>
/// Second executor: reverses the input text and completes the workflow.
/// </summary>
internal sealed class ReverseTextExecutor() : ReflectingExecutor<ReverseTextExecutor>("ReverseTextExecutor"), 
    IMessageHandler<string, string>
{
    public ValueTask<string> HandleAsync(string input, IWorkflowContext context)
    {
        // Reverse the input text
        return ValueTask.FromResult(new string(input.Reverse().ToArray()));
    }
}
```

**Key Points:**

- Same pattern as the first executor
- Reverses the string using LINQ's `Reverse()` method
- This will be the final executor in our workflow

### Step 4: Build and Connect the Workflow

Connect the executors using `WorkflowBuilder`:

```csharp
// Create the executors
UppercaseExecutor uppercase = new();
ReverseTextExecutor reverse = new();

// Build the workflow by connecting executors sequentially
WorkflowBuilder builder = new(uppercase);
builder.AddEdge(uppercase, reverse).WithOutputFrom(reverse);
var workflow = builder.Build();
```

**Key Points:**

- `WorkflowBuilder` constructor takes the starting executor
- `AddEdge()` creates a directed connection from uppercase to reverse
- `WithOutputFrom()` specifies which executor produces the final workflow output
- `Build()` creates the immutable workflow

### Step 5: Execute the Workflow

Run the workflow and observe the results:

```csharp
// Execute the workflow with input data
Run run = await InProcessExecution.RunAsync(workflow, "Hello, World!");
foreach (WorkflowEvent evt in run.NewEvents)
{
    if (evt is ExecutorCompletedEvent executorComplete)
    {
        Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
    }
}
```

### Step 6: Understanding the Workflow Output

When you run the workflow, you'll see output like:

```text
UppercaseExecutor: HELLO, WORLD!
ReverseTextExecutor: !DLROW ,OLLEH
```

The input "Hello, World!" is first converted to uppercase ("HELLO, WORLD!"), then reversed ("!DLROW ,OLLEH").

## Key Concepts Explained

### Executor Interface

Executors implement `IMessageHandler<TInput, TOutput>`:

- **TInput**: The type of data this executor accepts
- **TOutput**: The type of data this executor produces
- **HandleAsync**: The method that processes the input and returns the output

### Workflow Builder Pattern

The `WorkflowBuilder` provides a fluent API for constructing workflows:

- **Constructor**: Takes the starting executor
- **AddEdge()**: Creates directed connections between executors
- **WithOutputFrom()**: Specifies which executors produce workflow outputs
- **Build()**: Creates the final immutable workflow

### .NET Event Types

During execution, you can observe these event types:

- `ExecutorCompletedEvent` - When an executor finishes processing
- `WorkflowOutputEvent` - Contains the final workflow result (for streaming execution)

### .NET Workflow Builder Pattern

## Running the .NET Example

1. Create a new console application
2. Install the `Microsoft.Agents.AI.Workflows` NuGet package
3. Combine all the code snippets from the steps above into your `Program.cs`
4. Run the application

The workflow will process your input through both executors and display the results.

## Complete .NET Example

For the complete, ready-to-run implementation, see the [01_ExecutorsAndEdges sample](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/GettingStarted/Workflows/_Foundational/01_ExecutorsAndEdges/Program.cs) in the Agent Framework repository.

This sample includes:

- Full implementation with all using statements and class structure
- Additional comments explaining the workflow concepts
- Complete project setup and configuration

::: zone-end

::: zone pivot="programming-language-python"

## Overview

In this tutorial, you'll create a workflow with two executors:

1. **Upper Case Executor** - Converts input text to uppercase
2. **Reverse Text Executor** - Reverses the text and outputs the final result

The workflow demonstrates core concepts like:

- Using the `@executor` decorator to create workflow nodes
- Connecting executors with `WorkflowBuilder`
- Passing data between steps with `ctx.send_message()`
- Yielding final output with `ctx.yield_output()`
- Streaming events for real-time observability

## Prerequisites

- Python 3.10 or later
- Agent Framework Core Python package installed: `pip install agent-framework-core`
- No external AI services required for this basic example

## Step-by-Step Implementation

Let's build the sequential workflow step by step.

### Step 1: Import Required Modules

First, import the necessary modules from the Agent Framework:

```python
import asyncio
from typing_extensions import Never
from agent_framework import WorkflowBuilder, WorkflowContext, WorkflowOutputEvent, executor
```

### Step 2: Create the First Executor

Create an executor that converts text to uppercase using the `@executor` decorator:

```python
@executor(id="upper_case_executor")
async def to_upper_case(text: str, ctx: WorkflowContext[str]) -> None:
    """Transform the input to uppercase and forward it to the next step."""
    result = text.upper()
    
    # Send the intermediate result to the next executor
    await ctx.send_message(result)
```

**Key Points:**

- The `@executor` decorator registers this function as a workflow node
- `WorkflowContext[str]` indicates this executor sends a string downstream
- `ctx.send_message()` passes data to the next step

### Step 3: Create the Second Executor

Create an executor that reverses the text and yields the final output:

```python
@executor(id="reverse_text_executor")
async def reverse_text(text: str, ctx: WorkflowContext[Never, str]) -> None:
    """Reverse the input and yield the workflow output."""
    result = text[::-1]
    
    # Yield the final output for this workflow run
    await ctx.yield_output(result)
```

**Key Points:**

- `WorkflowContext[Never, str]` indicates this is a terminal executor
- `ctx.yield_output()` provides the final workflow result
- The workflow completes when it becomes idle

### Step 4: Build the Workflow

Connect the executors using `WorkflowBuilder`:

```python
workflow = (
    WorkflowBuilder()
    .add_edge(to_upper_case, reverse_text)
    .set_start_executor(to_upper_case)
    .build()
)
```

**Key Points:**

- `add_edge()` creates directed connections between executors
- `set_start_executor()` defines the entry point
- `build()` finalizes the workflow

### Step 5: Run the Workflow with Streaming

Execute the workflow and observe events in real-time:

```python
async def main():
    # Run the workflow and stream events
    async for event in workflow.run_stream("hello world"):
        print(f"Event: {event}")
        if isinstance(event, WorkflowOutputEvent):
            print(f"Workflow completed with result: {event.data}")

if __name__ == "__main__":
    asyncio.run(main())
```

### Step 6: Understanding the Output

When you run the workflow, you'll see events like:

```text
Event: ExecutorInvokedEvent(executor_id=upper_case_executor)
Event: ExecutorCompletedEvent(executor_id=upper_case_executor)
Event: ExecutorInvokedEvent(executor_id=reverse_text_executor)
Event: ExecutorCompletedEvent(executor_id=reverse_text_executor)
Event: WorkflowOutputEvent(data='DLROW OLLEH', source_executor_id=reverse_text_executor)
Workflow completed with result: DLROW OLLEH
```

## Key Concepts Explained

### Workflow Context Types

The `WorkflowContext` generic type defines what data flows between executors:

- `WorkflowContext[str]` - Sends a string to the next executor
- `WorkflowContext[Never, str]` - Terminal executor that yields workflow output of type string

### Event Types

During streaming execution, you'll observe these event types:

- `ExecutorInvokedEvent` - When an executor starts processing
- `ExecutorCompletedEvent` - When an executor finishes processing
- `WorkflowOutputEvent` - Contains the final workflow result

### Python Workflow Builder Pattern

The `WorkflowBuilder` provides a fluent API for constructing workflows:

- **add_edge()**: Creates directed connections between executors
- **set_start_executor()**: Defines the workflow entry point
- **build()**: Finalizes and returns an immutable workflow object

## Running the Example

1. Combine all the code snippets from the steps above into a single Python file
2. Save it as `sequential_workflow.py`
3. Run with: `python sequential_workflow.py`

The workflow will process the input "hello world" through both executors and display the streaming events.

## Complete Example

For the complete, ready-to-run implementation, see the [sequential_streaming.py sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/workflow/control-flow/sequential_streaming.py) in the Agent Framework repository.

This sample includes:

- Full implementation with all imports and documentation
- Additional comments explaining the workflow concepts
- Sample output showing the expected results

::: zone-end

## Next Steps

> [!div class="nextstepaction"]
> [Learn about creating a simple concurrent workflow](simple-concurrent-workflow.md)
