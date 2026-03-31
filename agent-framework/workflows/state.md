---
title: Microsoft Agent Framework Workflows - State
description: In-depth look at State in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: conceptual
ms.author: taochen
ms.date: 03/25/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                                    | C# | Python | Notes |
  |--------------------------------------------|:--:|:------:|-------|
  | Writing to State                           | ✅ |   ✅   |       |
  | Accessing State                            | ✅ |   ✅   |       |
  | State Isolation – Mutable vs Immutable     | ✅ |   ✅   | Prose only, no code needed |
  | State Isolation – Helper Methods           | ❌ |   ✅   | C# coming soon |
  | State Isolation – Resetting Shared Executors| ✅ |   ❌   | C#-specific; links to advanced page |
  | Agent State Management                     | ❌ |   ✅   | C# coming soon |
-->

# Microsoft Agent Framework Workflows - State

This document provides an overview of **State** in the Microsoft Agent Framework Workflow system.

## Overview

State allows multiple executors within a workflow to access and modify common data. This feature is essential for scenarios where different parts of the workflow need to share information where direct message passing is not feasible or efficient.

## Writing to State

::: zone pivot="programming-language-csharp"

```csharp
using Microsoft.Agents.AI.Workflows;

internal sealed class FileReadExecutor() : Executor<string, string>("FileReadExecutor")
{
    public override async ValueTask<string> HandleAsync(
        string message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        // Read file content from embedded resource
        string fileContent = File.ReadAllText(message);
        // Store file content in a shared state for access by other executors
        string fileID = Guid.NewGuid().ToString("N");
        await context.QueueStateUpdateAsync(fileID, fileContent, scopeName: "FileContent", cancellationToken);

        return fileID;
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

```python
import uuid

from agent_framework import (
    Executor,
    WorkflowContext,
    handler,
)

class FileReadExecutor(Executor):

    @handler
    async def handle(self, file_path: str, ctx: WorkflowContext[str]):
        # Read file content from embedded resource
        with open(file_path, 'r') as file:
            file_content = file.read()
        # Store file content in state for access by other executors
        file_id = str(uuid.uuid4())
        ctx.set_state(file_id, file_content)

        await ctx.send_message(file_id)
```

::: zone-end

## Accessing State

::: zone pivot="programming-language-csharp"

```csharp
using Microsoft.Agents.AI.Workflows;

internal sealed class WordCountingExecutor() : Executor<string, int>("WordCountingExecutor")
{
    public override async ValueTask<int> HandleAsync(
        string message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        // Retrieve the file content from the shared state
        var fileContent = await context.ReadStateAsync<string>(message, scopeName: "FileContent", cancellationToken)
            ?? throw new InvalidOperationException("File content state not found");

        return fileContent.Split([' ', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

```python
from agent_framework import (
    Executor,
    WorkflowContext,
    handler,
)

class WordCountingExecutor(Executor):

    @handler
    async def handle(self, file_id: str, ctx: WorkflowContext[int]):
        # Retrieve the file content from state
        file_content = ctx.get_state(file_id)
        if file_content is None:
            raise ValueError("File content state not found")

        await ctx.send_message(len(file_content.split()))
```

::: zone-end

## State Isolation

In real-world applications, properly managing state is critical when handling multiple tasks or requests. Without proper isolation, shared state between different workflow executions can lead to unexpected behavior, data corruption, and race conditions. This section explains how to ensure state isolation within Microsoft Agent Framework Workflows, providing insights into best practices and common pitfalls.

### Mutable Workflow Builders vs Immutable Workflows

Workflows are created by workflow builders. Workflow builders are generally considered mutable, where one can add, modify start executor or other configurations after the builder is created or even after a workflow has been built. On the other hand, workflows are immutable in that once a workflow is built, it cannot be modified (no public API to modify a workflow).

This distinction is important because it affects how state is managed across different workflow executions. It is not recommended to reuse a single workflow instance for multiple tasks or requests, as this can lead to unintended state sharing. Instead, it is recommended to create a new workflow instance from the builder for each task or request to ensure proper state isolation and thread safety.

### Ensuring State Isolation with Helper Methods

When executor instances are created once and shared across multiple workflow builds, their internal state is shared across all workflow executions. This can lead to issues if an executor contains mutable state that should be isolated per workflow. To ensure proper state isolation and thread safety, wrap executor instantiation and workflow building inside a helper method so that each call produces fresh, independent instances.

::: zone pivot="programming-language-csharp"

Coming soon...

::: zone-end

::: zone pivot="programming-language-python"

Non-isolated example (shared state):

```python
executor_a = CustomExecutorA()
executor_b = CustomExecutorB()

# executor_a and executor_b are shared across all workflows built from this builder
workflow_builder = WorkflowBuilder(start_executor=executor_a).add_edge(executor_a, executor_b)

workflow_a = workflow_builder.build()
workflow_b = workflow_builder.build()
# workflow_a and workflow_b share the same executor instances and their mutable state
```

Isolated example (helper method):

```python
def create_workflow() -> Workflow:
    """Create a fresh workflow with isolated state.

    Each call produces independent executor instances, ensuring no state
    leaks between workflow runs.
    """
    executor_a = CustomExecutorA()
    executor_b = CustomExecutorB()

    return WorkflowBuilder(start_executor=executor_a).add_edge(executor_a, executor_b).build()

# Each workflow has its own executor instances with independent state
workflow_a = create_workflow()
workflow_b = create_workflow()
```

::: zone-end

> [!TIP]
> To ensure proper state isolation and thread safety, also make sure that executor instances created inside the helper method do not share external mutable state.

::: zone pivot="programming-language-csharp"

### Resetting Shared Executors

If you need to share executor instances across workflow runs — for example, when executor construction is expensive or when a workflow is exposed as an agent — stateful executors must implement `IResettableExecutor`. This interface provides a `ResetAsync()` method that the workflow runtime calls automatically between runs to clear stale state.

For details on when and how to implement `IResettableExecutor`, see [Resettable Executors](./advanced/resettable-executors.md).

::: zone-end

### Agent State Management

Agent context is managed via agent threads. By default, each agent in a workflow will get its own thread unless the agent is managed by a custom executor. For more information, refer to [Working with Agents](./agents-in-workflows.md).

Agent threads are persisted across workflow runs. This means that if an agent is invoked in the first run of a workflow, content generated by the agent will be available in subsequent runs of the same workflow instance. While this can be useful for maintaining continuity within a single task, it can also lead to unintended state sharing if the same workflow instance is reused for different tasks or requests. To ensure each task has isolated agent state, wrap agent and workflow creation inside a helper method so that each call produces new agent instances with their own threads.

::: zone pivot="programming-language-csharp"

Coming soon...

::: zone-end

::: zone pivot="programming-language-python"

Non-isolated example (shared agent state):

```python
writer_agent = FoundryChatClient(
    project_endpoint=os.environ["AZURE_AI_PROJECT_ENDPOINT"],
    model=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
    credential=AzureCliCredential(),
).as_agent(
    instructions=(
        "You are an excellent content writer. You create new content and edit contents based on the feedback."
    ),
    name="writer_agent",
)
reviewer_agent = FoundryChatClient(
    project_endpoint=os.environ["AZURE_AI_PROJECT_ENDPOINT"],
    model=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
    credential=AzureCliCredential(),
).as_agent(
    instructions=(
        "You are an excellent content reviewer. "
        "Provide actionable feedback to the writer about the provided content. "
        "Provide the feedback in the most concise manner possible."
    ),
    name="reviewer_agent",
)

# writer_agent and reviewer_agent are shared across all workflows
workflow = WorkflowBuilder(start_executor=writer_agent).add_edge(writer_agent, reviewer_agent).build()
```

Isolated example (helper method):

```python
def create_workflow() -> Workflow:
    """Create a fresh workflow with isolated agent state.

    Each call produces new agent instances with their own threads,
    ensuring no conversation history leaks between workflow runs.
    """
    writer_agent = FoundryChatClient(
        project_endpoint=os.environ["AZURE_AI_PROJECT_ENDPOINT"],
        model=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
        credential=AzureCliCredential(),
    ).as_agent(
        instructions=(
            "You are an excellent content writer. You create new content and edit contents based on the feedback."
        ),
        name="writer_agent",
    )
    reviewer_agent = FoundryChatClient(
        project_endpoint=os.environ["AZURE_AI_PROJECT_ENDPOINT"],
        model=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
        credential=AzureCliCredential(),
    ).as_agent(
        instructions=(
            "You are an excellent content reviewer. "
            "Provide actionable feedback to the writer about the provided content. "
            "Provide the feedback in the most concise manner possible."
        ),
        name="reviewer_agent",
    )

    return WorkflowBuilder(start_executor=writer_agent).add_edge(writer_agent, reviewer_agent).build()

# Each workflow has its own agent instances and threads
workflow_a = create_workflow()
workflow_b = create_workflow()
```

::: zone-end

## Summary

State isolation in Microsoft Agent Framework Workflows can be effectively managed by wrapping executor and agent instantiation along with workflow building inside helper methods. By calling the helper method each time you need a new workflow, you ensure each instance has fresh, independent state and avoid unintended state sharing between different workflow executions.

## Next Steps

- [Learn how to create checkpoints and resume from them](./checkpoints.md).
- [Learn how to monitor workflows](./observability.md).
- [Learn how to visualize workflows](./visualization.md).
