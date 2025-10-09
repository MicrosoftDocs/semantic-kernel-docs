---
title: Checkpointing and Resuming Workflows
description: Learn how to implement checkpointing and resuming in workflows using Agent Framework.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/29/2025
ms.service: agent-framework
---

# Checkpointing and Resuming Workflows

Checkpointing allows workflows to save their state at specific points and resume execution later, even after process restarts. This is crucial for long-running workflows, error recovery, and human-in-the-loop scenarios.

::: zone pivot="programming-language-csharp"

## Key Components

### CheckpointManager

The `CheckpointManager` provides checkpoint storage and retrieval functionality:

```csharp
using Microsoft.Agents.AI.Workflows;

// Use the default in-memory checkpoint manager
var checkpointManager = CheckpointManager.Default;

// Or create a custom checkpoint manager with JSON serialization
var checkpointManager = CheckpointManager.CreateJson(store, customOptions);
```

### Enabling Checkpointing

Enable checkpointing when executing workflows using `InProcessExecution`:

```csharp
using Microsoft.Agents.AI.Workflows;

// Create workflow with checkpointing support
var workflow = await WorkflowHelper.GetWorkflowAsync();
var checkpointManager = CheckpointManager.Default;

// Execute with checkpointing enabled
Checkpointed<StreamingRun> checkpointedRun = await InProcessExecution
    .StreamAsync(workflow, NumberSignal.Init, checkpointManager);
```

## State Persistence

### Executor State

Executors can persist local state that survives checkpoints using the `ReflectingExecutor` base class:

```csharp
internal sealed class GuessNumberExecutor : ReflectingExecutor<GuessNumberExecutor>, IMessageHandler<NumberSignal>
{
    private static readonly StateKey StateKey = new("GuessNumberExecutor.State");

    public int LowerBound { get; private set; }
    public int UpperBound { get; private set; }

    public async ValueTask HandleAsync(NumberSignal message, IWorkflowContext context)
    {
        int guess = (LowerBound + UpperBound) / 2;
        await context.SendMessageAsync(guess);
    }

    /// <summary>
    /// Checkpoint the current state of the executor.
    /// This must be overridden to save any state that is needed to resume the executor.
    /// </summary>
    protected override ValueTask OnCheckpointingAsync(IWorkflowContext context, CancellationToken cancellationToken = default) =>
        context.QueueStateUpdateAsync(StateKey, (LowerBound, UpperBound));

    /// <summary>
    /// Restore the state of the executor from a checkpoint.
    /// This must be overridden to restore any state that was saved during checkpointing.
    /// </summary>
    protected override async ValueTask OnCheckpointRestoredAsync(IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        var state = await context.ReadStateAsync<(int, int)>(StateKey);
        (LowerBound, UpperBound) = state;
    }
}
```

### Automatic Checkpoint Creation

Checkpoints are automatically created at the end of each super step when a checkpoint manager is provided:

```csharp
var checkpoints = new List<CheckpointInfo>();

await foreach (WorkflowEvent evt in checkpointedRun.Run.WatchStreamAsync())
{
    switch (evt)
    {
        case SuperStepCompletedEvent superStepCompletedEvt:
            // Checkpoints are automatically created at super step boundaries
            CheckpointInfo? checkpoint = superStepCompletedEvt.CompletionInfo!.Checkpoint;
            if (checkpoint is not null)
            {
                checkpoints.Add(checkpoint);
                Console.WriteLine($"Checkpoint created at step {checkpoints.Count}.");
            }
            break;

        case WorkflowOutputEvent workflowOutputEvt:
            Console.WriteLine($"Workflow completed with result: {workflowOutputEvt.Data}");
            break;
    }
}
```

## Working with Checkpoints

### Accessing Checkpoint Information

Access checkpoint metadata from completed runs:

```csharp
// Get all checkpoints from a checkpointed run
var allCheckpoints = checkpointedRun.Checkpoints;

// Get the latest checkpoint
var latestCheckpoint = checkpointedRun.LatestCheckpoint;

// Access checkpoint details
foreach (var checkpoint in checkpoints)
{
    Console.WriteLine($"Checkpoint ID: {checkpoint.CheckpointId}");
    Console.WriteLine($"Step Number: {checkpoint.StepNumber}");
    Console.WriteLine($"Parent ID: {checkpoint.Parent?.CheckpointId ?? "None"}");
}
```

### Checkpoint Storage

Checkpoints are managed through the `CheckpointManager` interface:

```csharp
// Commit a checkpoint (usually done automatically)
CheckpointInfo checkpointInfo = await checkpointManager.CommitCheckpointAsync(runId, checkpoint);

// Retrieve a checkpoint
Checkpoint restoredCheckpoint = await checkpointManager.LookupCheckpointAsync(runId, checkpointInfo);
```

## Resuming from Checkpoints

### Streaming Resume

Resume execution from a checkpoint and stream events in real-time:

```csharp
// Resume from a specific checkpoint with streaming
CheckpointInfo savedCheckpoint = checkpoints[checkpointIndex];

Checkpointed<StreamingRun> resumedRun = await InProcessExecution
    .ResumeStreamAsync(workflow, savedCheckpoint, checkpointManager, runId);

await foreach (WorkflowEvent evt in resumedRun.Run.WatchStreamAsync())
{
    switch (evt)
    {
        case ExecutorCompletedEvent executorCompletedEvt:
            Console.WriteLine($"Executor {executorCompletedEvt.ExecutorId} completed.");
            break;

        case WorkflowOutputEvent workflowOutputEvt:
            Console.WriteLine($"Workflow completed with result: {workflowOutputEvt.Data}");
            return;
    }
}
```

### Non-Streaming Resume

Resume and wait for completion:

```csharp
// Resume from checkpoint without streaming
Checkpointed<Run> resumedRun = await InProcessExecution
    .ResumeAsync(workflow, savedCheckpoint, checkpointManager, runId);

// Wait for completion and get final result
var result = await resumedRun.Run.WaitForCompletionAsync();
```

### In-Place Restoration

Restore a checkpoint directly to an existing run instance:

```csharp
// Restore checkpoint to the same run instance
await checkpointedRun.RestoreCheckpointAsync(savedCheckpoint);

// Continue execution from the restored state
await foreach (WorkflowEvent evt in checkpointedRun.Run.WatchStreamAsync())
{
    // Handle events as normal
    if (evt is WorkflowOutputEvent outputEvt)
    {
        Console.WriteLine($"Resumed workflow result: {outputEvt.Data}");
        break;
    }
}
```

### New Workflow Instance (Rehydration)

Create a new workflow instance from a checkpoint:

```csharp
// Create a completely new workflow instance
var newWorkflow = await WorkflowHelper.GetWorkflowAsync();

// Resume with the new instance from a saved checkpoint
Checkpointed<StreamingRun> newCheckpointedRun = await InProcessExecution
    .ResumeStreamAsync(newWorkflow, savedCheckpoint, checkpointManager, originalRunId);

await foreach (WorkflowEvent evt in newCheckpointedRun.Run.WatchStreamAsync())
{
    if (evt is WorkflowOutputEvent workflowOutputEvt)
    {
        Console.WriteLine($"Rehydrated workflow result: {workflowOutputEvt.Data}");
        break;
    }
}
```

## Human-in-the-Loop with Checkpointing

Combine checkpointing with human-in-the-loop workflows:

```csharp
var checkpoints = new List<CheckpointInfo>();

await foreach (WorkflowEvent evt in checkpointedRun.Run.WatchStreamAsync())
{
    switch (evt)
    {
        case RequestInfoEvent requestInputEvt:
            // Handle external requests
            ExternalResponse response = HandleExternalRequest(requestInputEvt.Request);
            await checkpointedRun.Run.SendResponseAsync(response);
            break;

        case SuperStepCompletedEvent superStepCompletedEvt:
            // Save checkpoint after each interaction
            CheckpointInfo? checkpoint = superStepCompletedEvt.CompletionInfo!.Checkpoint;
            if (checkpoint is not null)
            {
                checkpoints.Add(checkpoint);
                Console.WriteLine($"Checkpoint created after human interaction.");
            }
            break;

        case WorkflowOutputEvent workflowOutputEvt:
            Console.WriteLine($"Workflow completed: {workflowOutputEvt.Data}");
            return;
    }
}

// Later, resume from any checkpoint
if (checkpoints.Count > 0)
{
    var selectedCheckpoint = checkpoints[1]; // Select specific checkpoint
    await checkpointedRun.RestoreCheckpointAsync(selectedCheckpoint);

    // Continue from that point
    await foreach (WorkflowEvent evt in checkpointedRun.Run.WatchStreamAsync())
    {
        // Handle remaining workflow execution
    }
}
```

## Complete Example Pattern

Here's a comprehensive checkpointing workflow pattern:

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Agents.AI.Workflows;

public static class CheckpointingExample
{
    public static async Task RunAsync()
    {
        // Create workflow and checkpoint manager
        var workflow = await WorkflowHelper.GetWorkflowAsync();
        var checkpointManager = CheckpointManager.Default;
        var checkpoints = new List<CheckpointInfo>();

        Console.WriteLine("Starting workflow with checkpointing...");

        // Execute workflow with checkpointing
        Checkpointed<StreamingRun> checkpointedRun = await InProcessExecution
            .StreamAsync(workflow, NumberSignal.Init, checkpointManager);

        // Monitor execution and collect checkpoints
        await foreach (WorkflowEvent evt in checkpointedRun.Run.WatchStreamAsync())
        {
            switch (evt)
            {
                case ExecutorCompletedEvent executorEvt:
                    Console.WriteLine($"Executor {executorEvt.ExecutorId} completed.");
                    break;

                case SuperStepCompletedEvent superStepEvt:
                    var checkpoint = superStepEvt.CompletionInfo!.Checkpoint;
                    if (checkpoint is not null)
                    {
                        checkpoints.Add(checkpoint);
                        Console.WriteLine($"Checkpoint {checkpoints.Count} created.");
                    }
                    break;

                case WorkflowOutputEvent outputEvt:
                    Console.WriteLine($"Workflow completed: {outputEvt.Data}");
                    goto FinishExecution;
            }
        }

        FinishExecution:
        Console.WriteLine($"Total checkpoints created: {checkpoints.Count}");

        // Demonstrate resuming from a checkpoint
        if (checkpoints.Count > 5)
        {
            var selectedCheckpoint = checkpoints[5];
            Console.WriteLine($"Resuming from checkpoint 6...");

            // Restore to same instance
            await checkpointedRun.RestoreCheckpointAsync(selectedCheckpoint);

            await foreach (WorkflowEvent evt in checkpointedRun.Run.WatchStreamAsync())
            {
                if (evt is WorkflowOutputEvent resumedOutputEvt)
                {
                    Console.WriteLine($"Resumed workflow result: {resumedOutputEvt.Data}");
                    break;
                }
            }
        }

        // Demonstrate rehydration with new workflow instance
        if (checkpoints.Count > 3)
        {
            var newWorkflow = await WorkflowHelper.GetWorkflowAsync();
            var rehydrationCheckpoint = checkpoints[3];

            Console.WriteLine("Rehydrating from checkpoint 4 with new workflow instance...");

            Checkpointed<StreamingRun> newRun = await InProcessExecution
                .ResumeStreamAsync(newWorkflow, rehydrationCheckpoint, checkpointManager, checkpointedRun.Run.RunId);

            await foreach (WorkflowEvent evt in newRun.Run.WatchStreamAsync())
            {
                if (evt is WorkflowOutputEvent rehydratedOutputEvt)
                {
                    Console.WriteLine($"Rehydrated workflow result: {rehydratedOutputEvt.Data}");
                    break;
                }
            }
        }
    }
}
```

## Key Benefits

- **Fault Tolerance**: Workflows can recover from failures by resuming from the last checkpoint
- **Long-Running Processes**: Break long workflows into manageable segments with automatic checkpoint boundaries
- **Human-in-the-Loop**: Pause for external input and resume later from saved state
- **Debugging**: Inspect workflow state at specific points and resume execution for testing
- **Portability**: Checkpoints can be restored to new workflow instances (rehydration)
- **Automatic Management**: Checkpoints are created automatically at super step boundaries

### Running the Example

For the complete working implementation, see the [CheckpointAndResume sample](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/GettingStarted/Workflows/Checkpoint/CheckpointAndResume).

::: zone-end

::: zone pivot="programming-language-python"

## Key Components

### FileCheckpointStorage

The `FileCheckpointStorage` class provides persistent checkpoint storage using JSON files:

```python
from agent_framework import FileCheckpointStorage
from pathlib import Path

# Initialize checkpoint storage
checkpoint_storage = FileCheckpointStorage(storage_path="./checkpoints")
```

### Enabling Checkpointing

Enable checkpointing when building your workflow:

```python
from agent_framework import WorkflowBuilder

workflow = (
    WorkflowBuilder(max_iterations=5)
    .add_edge(executor1, executor2)
    .set_start_executor(executor1)
    .with_checkpointing(checkpoint_storage=checkpoint_storage)  # Enable checkpointing
    .build()
)
```

## State Persistence

### Executor State

Executors can persist local state that survives checkpoints:

```python
from agent_framework import Executor, WorkflowContext, handler

class UpperCaseExecutor(Executor):
    @handler
    async def to_upper_case(self, text: str, ctx: WorkflowContext[str]) -> None:
        result = text.upper()

        # Persist executor-local state for checkpoints
        prev = await ctx.get_state() or {}
        count = int(prev.get("count", 0)) + 1
        await ctx.set_state({
            "count": count,
            "last_input": text,
            "last_output": result,
        })

        # Send result to next executor
        await ctx.send_message(result)
```

### Shared State

Use shared state for data that multiple executors need to access:

```python
class ProcessorExecutor(Executor):
    @handler
    async def process(self, text: str, ctx: WorkflowContext[str]) -> None:
        # Write to shared state for cross-executor visibility
        await ctx.set_shared_state("original_input", text)
        await ctx.set_shared_state("processed_output", text.upper())

        await ctx.send_message(text.upper())
```

## Working with Checkpoints

### Listing Checkpoints

Retrieve and inspect available checkpoints:

```python
# List all checkpoints
all_checkpoints = await checkpoint_storage.list_checkpoints()

# List checkpoints for a specific workflow
workflow_checkpoints = await checkpoint_storage.list_checkpoints(workflow_id="my-workflow")

# Sort by creation time
sorted_checkpoints = sorted(all_checkpoints, key=lambda cp: cp.timestamp)
```

### Checkpoint Information

Access checkpoint metadata and state:

```python
from agent_framework import RequestInfoExecutor

for checkpoint in checkpoints:
    # Get human-readable summary
    summary = RequestInfoExecutor.checkpoint_summary(checkpoint)

    print(f"Checkpoint: {summary.checkpoint_id}")
    print(f"Iteration: {summary.iteration_count}")
    print(f"Status: {summary.status}")
    print(f"Messages: {len(checkpoint.messages)}")
    print(f"Shared State: {checkpoint.shared_state}")
    print(f"Executor States: {list(checkpoint.executor_states.keys())}")
```

## Resuming from Checkpoints

### Streaming Resume

Resume execution and stream events in real-time:

```python
# Resume from a specific checkpoint
async for event in workflow.run_stream_from_checkpoint(
    checkpoint_id="checkpoint-id",
    checkpoint_storage=checkpoint_storage
):
    print(f"Resumed Event: {event}")

    if isinstance(event, WorkflowOutputEvent):
        print(f"Final Result: {event.data}")
        break
```

### Non-Streaming Resume

Resume and get all results at once:

```python
# Resume and wait for completion
result = await workflow.run_from_checkpoint(
    checkpoint_id="checkpoint-id",
    checkpoint_storage=checkpoint_storage
)

# Access final outputs
outputs = result.get_outputs()
print(f"Final outputs: {outputs}")
```

### Resume with Responses

For workflows with pending requests, provide responses during resume:

```python
# Resume with pre-supplied responses for RequestInfoExecutor
responses = {
    "request-id-1": "user response data",
    "request-id-2": "another response"
}

async for event in workflow.run_stream_from_checkpoint(
    checkpoint_id="checkpoint-id",
    checkpoint_storage=checkpoint_storage,
    responses=responses  # Inject responses during resume
):
    print(f"Event: {event}")
```

## Interactive Checkpoint Selection

Build user-friendly checkpoint selection:

```python
async def select_and_resume_checkpoint(workflow, storage):
    # Get available checkpoints
    checkpoints = await storage.list_checkpoints()
    if not checkpoints:
        print("No checkpoints available")
        return

    # Sort and display options
    sorted_cps = sorted(checkpoints, key=lambda cp: cp.timestamp)
    print("Available checkpoints:")
    for i, cp in enumerate(sorted_cps):
        summary = RequestInfoExecutor.checkpoint_summary(cp)
        print(f"[{i}] {summary.checkpoint_id[:8]}... iter={summary.iteration_count}")

    # Get user selection
    try:
        idx = int(input("Enter checkpoint index: "))
        selected = sorted_cps[idx]

        # Resume from selected checkpoint
        print(f"Resuming from checkpoint: {selected.checkpoint_id}")
        async for event in workflow.run_stream_from_checkpoint(
            selected.checkpoint_id,
            checkpoint_storage=storage
        ):
            print(f"Event: {event}")

    except (ValueError, IndexError):
        print("Invalid selection")
```

## Complete Example Pattern

Here's a typical checkpointing workflow pattern:

```python
import asyncio
from pathlib import Path
from agent_framework import (
    WorkflowBuilder, FileCheckpointStorage,
    WorkflowOutputEvent, RequestInfoExecutor
)

async def main():
    # Setup checkpoint storage
    checkpoint_dir = Path("./checkpoints")
    checkpoint_dir.mkdir(exist_ok=True)
    storage = FileCheckpointStorage(checkpoint_dir)

    # Build workflow with checkpointing
    workflow = (
        WorkflowBuilder()
        .add_edge(executor1, executor2)
        .set_start_executor(executor1)
        .with_checkpointing(storage)
        .build()
    )

    # Initial run
    print("Running workflow...")
    async for event in workflow.run_stream("input data"):
        print(f"Event: {event}")

    # List and inspect checkpoints
    checkpoints = await storage.list_checkpoints()
    for cp in sorted(checkpoints, key=lambda c: c.timestamp):
        summary = RequestInfoExecutor.checkpoint_summary(cp)
        print(f"Checkpoint: {summary.checkpoint_id[:8]}... iter={summary.iteration_count}")

    # Resume from a checkpoint
    if checkpoints:
        latest = max(checkpoints, key=lambda cp: cp.timestamp)
        print(f"Resuming from: {latest.checkpoint_id}")

        async for event in workflow.run_stream_from_checkpoint(latest.checkpoint_id):
            print(f"Resumed: {event}")

if __name__ == "__main__":
    asyncio.run(main())
```

## Key Benefits

- **Fault Tolerance**: Workflows can recover from failures by resuming from the last checkpoint
- **Long-Running Processes**: Break long workflows into manageable segments with checkpoint boundaries
- **Human-in-the-Loop**: Pause for human input and resume later with responses
- **Debugging**: Inspect workflow state at specific points and resume execution for testing
- **Resource Management**: Stop and restart workflows based on resource availability

### Running the Example

For the complete working implementation, see the [Checkpoint with Resume sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/workflows/checkpoint/checkpoint_with_resume.py).

::: zone-end

## Next Steps

> [!div class="nextstepaction"]
> [Learn about Workflow Visualization](visualization.md)
