---
title: Microsoft Agent Framework Workflows - Checkpoints
description: In-depth look at Checkpoints in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/12/2025
ms.service: agent-framework
---

# Microsoft Agent Framework Workflows - Checkpoints

This page provides an overview of **Checkpoints** in the Microsoft Agent Framework Workflow system.

## Overview

Checkpoints allow you to save the state of a workflow at specific points during its execution, and resume from those points later. This feature is particularly useful for the following scenarios:

- Long-running workflows where you want to avoid losing progress in case of failures.
- Long-running workflows where you want to pause and resume execution at a later time.
- Workflows that require periodic state saving for auditing or compliance purposes.
- Workflows that need to be migrated across different environments or instances.

## When Are Checkpoints Created?

Remember that workflows are executed in **supersteps**, as documented in the [core concepts](./core-concepts/workflows.md#execution-model). Checkpoints are created at the end of each superstep, after all executors in that superstep have completed their execution. A checkpoint captures the entire state of the workflow, including:

- The current state of all executors
- All pending messages in the workflow for the next superstep
- Pending requests and responses
- Shared states

## Capturing Checkpoints

::: zone pivot="programming-language-csharp"

To enable check pointing, a `CheckpointManager` needs to be provided when creating a workflow run. A checkpoint then can be accessed via a `SuperStepCompletedEvent`.

```csharp
using Microsoft.Agents.Workflows;

// Create a checkpoint manager to manage checkpoints
var checkpointManager = new CheckpointManager();
// List to store checkpoint info for later use
var checkpoints = new List<CheckpointInfo>();

// Run the workflow with checkpointing enabled
Checkpointed<StreamingRun> checkpointedRun = await InProcessExecution
    .StreamAsync(workflow, input, checkpointManager)
    .ConfigureAwait(false);
await foreach (WorkflowEvent evt in checkpointedRun.Run.WatchStreamAsync().ConfigureAwait(false))
{
    if (evt is SuperStepCompletedEvent superStepCompletedEvt)
    {
        // Access the checkpoint and store it
        CheckpointInfo? checkpoint = superStepCompletedEvt.CompletionInfo!.Checkpoint;
        if (checkpoint != null)
        {
            checkpoints.Add(checkpoint);
        }
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

To enable check pointing, a `CheckpointStorage` needs to be provided when creating a workflow. A checkpoint then can be accessed via the storage.

```python
from agent_framework import (
    InMemoryCheckpointStorage,
    WorkflowBuilder,
)

# Create a checkpoint storage to manage checkpoints
# There are different implementations of CheckpointStorage, such as InMemoryCheckpointStorage and FileCheckpointStorage.
checkpoint_storage = InMemoryCheckpointStorage()

# Build a workflow with checkpointing enabled
builder = WorkflowBuilder()
builder.set_start_executor(start_executor)
builder.add_edge(start_executor, executor_b)
builder.add_edge(executor_b, executor_c)
builder.add_edge(executor_b, end_executor)
workflow = builder.with_checkpointing(checkpoint_storage).build()

# Run the workflow
async for event in workflow.run_streaming(input):
    ...

# Access checkpoints from the storage
checkpoints = await checkpoint_storage.list_checkpoints()
```

::: zone-end

## Resuming from Checkpoints

::: zone pivot="programming-language-csharp"

You can resume a workflow from a specific checkpoint directly on the same run.

```csharp
// Assume we want to resume from the 6th checkpoint
CheckpointInfo savedCheckpoint = checkpoints[5];
// Note that we are restoring the state directly to the same run instance.
await checkpointedRun.RestoreCheckpointAsync(savedCheckpoint, CancellationToken.None).ConfigureAwait(false);
await foreach (WorkflowEvent evt in checkpointedRun.Run.WatchStreamAsync().ConfigureAwait(false))
{
    if (evt is WorkflowCompletedEvent workflowCompletedEvt)
    {
        Console.WriteLine($"Workflow completed with result: {workflowCompletedEvt.Data}");
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

You can resume a workflow from a specific checkpoint directly on the same workflow instance.

```python
# Assume we want to resume from the 6th checkpoint
saved_checkpoint = checkpoints[5]
async for event in workflow.run_stream_from_checkpoint(saved_checkpoint.checkpoint_id):
    ...
```

::: zone-end

## Rehydrating from Checkpoints

::: zone pivot="programming-language-csharp"

Or you can rehydrate a workflow from a checkpoint into a new run instance.

```csharp
// Assume we want to resume from the 6th checkpoint
CheckpointInfo savedCheckpoint = checkpoints[5];
Checkpointed<StreamingRun> newCheckpointedRun = await InProcessExecution
    .ResumeStreamAsync(newWorkflow, savedCheckpoint, checkpointManager)
    .ConfigureAwait(false);
await foreach (WorkflowEvent evt in newCheckpointedRun.Run.WatchStreamAsync().ConfigureAwait(false))
{
    if (evt is WorkflowCompletedEvent workflowCompletedEvt)
    {
        Console.WriteLine($"Workflow completed with result: {workflowCompletedEvt.Data}");
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

Or you can rehydrate a new workflow instance from a checkpoint.

```python
from agent_framework import WorkflowBuilder

builder = WorkflowBuilder()
builder.set_start_executor(start_executor)
builder.add_edge(start_executor, executor_b)
builder.add_edge(executor_b, executor_c)
builder.add_edge(executor_b, end_executor)
# This workflow instance doesn't require checkpointing enabled.
workflow = builder.build()

# Assume we want to resume from the 6th checkpoint
saved_checkpoint = checkpoints[5]
async for event in workflow.run_stream_from_checkpoint(
    saved_checkpoint.checkpoint_id,
    checkpoint_storage,
):
    ...
```

::: zone-end

## Save Executor States

::: zone pivot="programming-language-csharp"

To ensure that the state of an executor is captured in a checkpoint, the executor must override the `OnCheckpointingAsync` method and save its state to the workflow context.

```csharp
using Microsoft.Agents.Workflows;
using Microsoft.Agents.Workflows.Reflection;

internal sealed class CustomExecutor() : ReflectingExecutor<CustomExecutor>("CustomExecutor"), IMessageHandler<string>
{
    private const string StateKey = "CustomExecutorState";

    private List<string> messages = new();

    public async ValueTask HandleAsync(string message, IWorkflowContext context)
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

Coming soon...

::: zone-end

## Next Steps

- [Learn how to use agents in workflows](./using-agents.md) to build intelligent workflows.
- [Learn how to use workflows as agents](./as-agents.md).
- [Learn how to handle requests and responses](./request-and-response.md) in workflows.
- [Learn how to manage state](./shared-states.md) in workflows.
