---
title: Microsoft Agent Framework Workflows - Checkpoints
description: In-depth look at Checkpoints in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 03/11/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                        | C# | Python | Notes |
  |--------------------------------|:--:|:------:|-------|
  | Overview                       | ✅ |   ✅   |       |
  | When Are Checkpoints Created?  | ✅ |   ✅   |       |
  | Capturing Checkpoints          | ✅ |   ✅   |       |
  | Resuming from Checkpoints      | ✅ |   ✅   |       |
  | Rehydrating from Checkpoints   | ✅ |   ✅   |       |
  | Save Executor States           | ✅ |   ✅   |       |
  | Security Considerations        | ✅ |   ✅   |       |
  | Next Steps                     | ✅ |   ✅   |       |
-->

# Microsoft Agent Framework Workflows - Checkpoints

This page provides an overview of **Checkpoints** in the Microsoft Agent Framework Workflow system.

## Overview

Checkpoints allow you to save the state of a workflow at specific points during its execution, and resume from those points later. This feature is particularly useful for the following scenarios:

- Long-running workflows where you want to avoid losing progress in case of failures.
- Long-running workflows where you want to pause and resume execution at a later time.
- Workflows that require periodic state saving for auditing or compliance purposes.
- Workflows that need to be migrated across different environments or instances.

## When Are Checkpoints Created?

Remember that workflows are executed in **supersteps**, as documented in the [core concepts](./index.md#core-concepts). Checkpoints are created at the end of each superstep, after all executors in that superstep have completed their execution. A checkpoint captures the entire state of the workflow, including:

- The current state of all executors
- All pending messages in the workflow for the next superstep
- Pending requests and responses
- Shared states

## Capturing Checkpoints

::: zone pivot="programming-language-csharp"

To enable checkpointing, a `CheckpointManager` needs to be provided when running the workflow. A checkpoint can then be accessed via a `SuperStepCompletedEvent`, or through the `Checkpoints` property on the run.

```csharp
using Microsoft.Agents.AI.Workflows;

// Create a checkpoint manager to manage checkpoints
CheckpointManager checkpointManager = CheckpointManager.CreateInMemory();

// Run the workflow with checkpointing enabled
StreamingRun run = await InProcessExecution
    .RunStreamingAsync(workflow, input, checkpointManager)
    .ConfigureAwait(false);
await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
{
    if (evt is SuperStepCompletedEvent superStepCompletedEvt)
    {
        // Access the checkpoint
        CheckpointInfo? checkpoint = superStepCompletedEvt.CompletionInfo?.Checkpoint;
    }
}

// Checkpoints can also be accessed from the run directly
IReadOnlyList<CheckpointInfo> checkpoints = run.Checkpoints;
```

::: zone-end

::: zone pivot="programming-language-python"

To enable checkpointing, a `CheckpointStorage` needs to be provided when creating a workflow. A checkpoint can then be accessed via the storage.

```python
from agent_framework import (
    InMemoryCheckpointStorage,
    WorkflowBuilder,
)

# Create a checkpoint storage to manage checkpoints
# There are different implementations of CheckpointStorage, such as InMemoryCheckpointStorage and FileCheckpointStorage.
checkpoint_storage = InMemoryCheckpointStorage()

# Build a workflow with checkpointing enabled
builder = WorkflowBuilder(start_executor=start_executor, checkpoint_storage=checkpoint_storage)
builder.add_edge(start_executor, executor_b)
builder.add_edge(executor_b, executor_c)
builder.add_edge(executor_b, end_executor)
workflow = builder.build()

# Run the workflow
async for event in workflow.run(input, stream=True):
    ...

# Access checkpoints from the storage
checkpoints = await checkpoint_storage.list_checkpoints(workflow_name=workflow.name)
```

::: zone-end

## Resuming from Checkpoints

::: zone pivot="programming-language-csharp"

You can resume a workflow from a specific checkpoint directly on the same run.

```csharp
// Assume we want to resume from the 6th checkpoint
CheckpointInfo savedCheckpoint = run.Checkpoints[5];
// Restore the state directly on the same run instance.
await run.RestoreCheckpointAsync(savedCheckpoint).ConfigureAwait(false);
await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
{
    if (evt is WorkflowOutputEvent workflowOutputEvt)
    {
        Console.WriteLine($"Workflow completed with result: {workflowOutputEvt.Data}");
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

You can resume a workflow from a specific checkpoint directly on the same workflow instance.

```python
# Assume we want to resume from the 6th checkpoint
saved_checkpoint = checkpoints[5]
async for event in workflow.run(checkpoint_id=saved_checkpoint.checkpoint_id, stream=True):
    ...
```

::: zone-end

## Rehydrating from Checkpoints

::: zone pivot="programming-language-csharp"

Or you can rehydrate a workflow from a checkpoint into a new run instance.

```csharp
// Assume we want to resume from the 6th checkpoint
CheckpointInfo savedCheckpoint = run.Checkpoints[5];
StreamingRun newRun = await InProcessExecution
    .ResumeStreamingAsync(newWorkflow, savedCheckpoint, checkpointManager)
    .ConfigureAwait(false);
await foreach (WorkflowEvent evt in newRun.WatchStreamAsync().ConfigureAwait(false))
{
    if (evt is WorkflowOutputEvent workflowOutputEvt)
    {
        Console.WriteLine($"Workflow completed with result: {workflowOutputEvt.Data}");
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

Or you can rehydrate a new workflow instance from a checkpoint.

```python
from agent_framework import WorkflowBuilder

builder = WorkflowBuilder(start_executor=start_executor)
builder.add_edge(start_executor, executor_b)
builder.add_edge(executor_b, executor_c)
builder.add_edge(executor_b, end_executor)
# This workflow instance doesn't require checkpointing enabled.
workflow = builder.build()

# Assume we want to resume from the 6th checkpoint
saved_checkpoint = checkpoints[5]
async for event in workflow.run(
    checkpoint_id=saved_checkpoint.checkpoint_id,
    checkpoint_storage=checkpoint_storage,
    stream=True,
):
    ...
```

::: zone-end

## Save Executor States

::: zone pivot="programming-language-csharp"

To ensure that the state of an executor is captured in a checkpoint, the executor must override the `OnCheckpointingAsync` method and save its state to the workflow context.

```csharp
using Microsoft.Agents.AI.Workflows;

internal sealed partial class CustomExecutor() : Executor("CustomExecutor")
{
    private const string StateKey = "CustomExecutorState";

    private List<string> messages = new();

    [MessageHandler]
    private async ValueTask HandleAsync(string message, IWorkflowContext context)
    {
        this.messages.Add(message);
        // Executor logic...
    }

    protected override ValueTask OnCheckpointingAsync(IWorkflowContext context, CancellationToken cancellation = default)
    {
        return context.QueueStateUpdateAsync(StateKey, this.messages);
    }
}
```

Also, to ensure the state is correctly restored when resuming from a checkpoint, the executor must override the `OnCheckpointRestoredAsync` method and load its state from the workflow context.

```csharp
protected override async ValueTask OnCheckpointRestoredAsync(IWorkflowContext context, CancellationToken cancellation = default)
{
    this.messages = await context.ReadStateAsync<List<string>>(StateKey).ConfigureAwait(false);
}
```

::: zone-end

::: zone pivot="programming-language-python"

To ensure that the state of an executor is captured in a checkpoint, the executor must override the `on_checkpoint_save` method and return its state as a dictionary.

```python
class CustomExecutor(Executor):
    def __init__(self, id: str) -> None:
        super().__init__(id=id)
        self._messages: list[str] = []

    @handler
    async def handle(self, message: str, ctx: WorkflowContext):
        self._messages.append(message)
        # Executor logic...

    async def on_checkpoint_save(self) -> dict[str, Any]:
        return {"messages": self._messages}
```

Also, to ensure the state is correctly restored when resuming from a checkpoint, the executor must override the `on_checkpoint_restore` method and restore its state from the provided state dictionary.

```python
async def on_checkpoint_restore(self, state: dict[str, Any]) -> None:
    self._messages = state.get("messages", [])
```

::: zone-end

## Security Considerations

> [!IMPORTANT]
> Checkpoint storage is a trust boundary. Whether you use the built-in storage implementations or a custom one, the storage backend must be treated as trusted, private infrastructure. **Never load checkpoints from untrusted or potentially tampered sources.** Loading a malicious checkpoint can execute arbitrary code.

::: zone pivot="programming-language-csharp"

Ensure that the storage location used for checkpoints is secured appropriately. Only authorized services and users should have read or write access to checkpoint data.

::: zone-end

::: zone pivot="programming-language-python"

### Pickle serialization

`FileCheckpointStorage` uses Python's [`pickle`](https://docs.python.org/3/library/pickle.html) module to serialize non-JSON-native state such as dataclasses, datetimes, and custom objects. Because `pickle.loads()` can execute arbitrary code during deserialization, a compromised checkpoint file can run malicious code when loaded. The post-deserialization type check performed by the framework cannot prevent this.

If your threat model does not permit pickle-based serialization, use `InMemoryCheckpointStorage` or implement a custom `CheckpointStorage` with an alternative serialization strategy.

### Storage location responsibility

`FileCheckpointStorage` requires an explicit `storage_path` parameter — there is no default directory. While the framework validates against path traversal attacks, securing the storage directory itself (file permissions, encryption at rest, access controls) is the developer's responsibility. Only authorized processes should have read or write access to the checkpoint directory.

::: zone-end

## Next Steps

- [Learn how to monitor workflows](./observability.md).
- [Learn about state isolation in workflows](./state.md).
- [Learn how to visualize workflows](./visualization.md).
