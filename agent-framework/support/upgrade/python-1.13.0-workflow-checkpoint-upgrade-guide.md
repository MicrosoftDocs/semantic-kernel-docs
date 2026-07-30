---
title: Upgrade Python workflow checkpoints to 1.13.0
description: Learn how to update Python workflow event, iteration, message source, and checkpoint handling for Agent Framework 1.13.0.
author: TaoChenOSU
ms.topic: upgrade-and-migration-article
ms.author: taochen
ms.date: 07/30/2026
ms.service: agent-framework
---

# Upgrade Python workflow checkpoints to 1.13.0

Agent Framework 1.13.0 contains minor breaking changes to Python workflow execution. Most applications **don't** require changes. The changes affect applications that depend on exact superstep counts or iteration numbers, set `max_iterations` at the convergence boundary, inspect the initial message source ID, or make assumptions about checkpoint placement and ordering.

## Background

Before 1.13.0, checkpointing didn't fully meet its promise of capturing the workflow state needed to resume execution from any recorded boundary. The start executor ran before the superstep and checkpoint loop, so the earliest checkpoint contained the start executor's output and updated state, but not the original workflow input. Similarly, responses to request events were delivered and processed without first being recorded in a checkpoint. As a result, no checkpoint could replay the start executor from the original input or reproduce a human-in-the-loop continuation from the delivered response.

## Behavior changes

Version 1.13.0 closes these gaps. The start executor now runs in the first superstep, an entry checkpoint records the initial input before that superstep, and a response-entry checkpoint records delivered responses before they are processed. Together, these changes make a checkpointed workflow run fully replayable from its input, including human-in-the-loop continuations.

> [!IMPORTANT]
> These changes don't affect checkpoints created before version 1.13.0. Existing checkpoints remain supported and can still be restored after upgrading.

### Changes that might require action

| Area | Before 1.13.0 | In 1.13.0 and later | User impact |
|---|---|---|---|
| Start executor | The start executor ran before the superstep loop. | The input is queued for the start executor, which runs in the first superstep. | Each fresh run emits one additional `superstep_started` and `superstep_completed` event. |
| Iteration count | Iteration 1 represented the first superstep after the start executor ran. | Iteration 1 runs the start executor. Later work shifts by one iteration. | A workflow that previously needed $N$ iterations now needs $N + 1$. |
| Input message source | The initial message had the hardcoded source ID `"Workflow"`. | The initial message is delivered through the start executor's internal edge and has source ID `INTERNAL_SOURCE_ID(start_executor.id)`. | Code that reads or filters the initial message source ID must use the new value. |

### Replayability improvements

| Area | Before 1.13.0 | In 1.13.0 and later | Improvement |
|---|---|---|---|
| Initial checkpoint | The iteration-0 checkpoint was created after the start executor ran. It captured the executor's output messages and updated state, but not the original input. | An entry checkpoint is created before superstep 1. It records the original input queued for the start executor. | Restoring the entry checkpoint replays the complete run, including the start executor. |
| Response checkpoint | A response to a request event was delivered without first being recorded in a checkpoint. | A response-entry checkpoint is created after the response is delivered and before its consuming superstep runs. | Restoring the response-entry checkpoint replays the continuation that consumes the response. |

## Update superstep event handling

A fresh workflow run now produces one more pair of superstep events because the start executor runs in superstep 1:

- `superstep_started` with `iteration == 1`
- `superstep_completed` with `iteration == 1`

Subsequent executor work shifts by one superstep. Update tests, telemetry, progress indicators, or other code that assumes an exact event count or maps a particular executor to a fixed iteration.

Code that responds to event types without relying on their count or iteration doesn't need to change.

## Review the maximum iteration limit

The `max_iterations` limit now includes the superstep that runs the start executor. If a workflow previously used its full limit, increase the configured value by one:

```python
from agent_framework import WorkflowBuilder

workflow = WorkflowBuilder(
    start_executor=start_executor,
    max_iterations=previous_max_iterations + 1,
).build()
```

No change is needed if the workflow already converges before reaching the configured limit.

## Update initial message source checks

If a start executor consumes the source ID of the initial message, replace the hardcoded `"Workflow"` value with the source ID for the start executor's internal edge.

**Before 1.13.0:**

```python
is_workflow_input = ctx.source_executor_ids != ["Workflow"]
```

**In 1.13.0 and later:**

```python
from agent_framework import INTERNAL_SOURCE_ID

is_workflow_input = ctx.source_executor_ids != [INTERNAL_SOURCE_ID(self.id)]
```

`INTERNAL_SOURCE_ID(executor_id)` currently returns `"internal:<executor_id>"`. Use the helper instead of constructing this string so your code follows the framework's source ID format.

## Update checkpoint handling

### Initial input checkpoints

When checkpointing is enabled, every fresh run now creates an entry checkpoint at `iteration_count == 0`. This checkpoint contains the original input as an in-flight message addressed to the start executor. Restoring it reruns the start executor and reproduces the complete workflow run.

After each completed superstep, the framework continues to create a checkpoint. For a run with $N$ supersteps, expect $N + 1$ checkpoints: the entry checkpoint followed by one checkpoint for each completed superstep.

Review code that assumes the iteration-0 checkpoint contains state produced by the start executor. That state now appears in the checkpoint created after superstep 1.

### Request-response checkpoints

When you continue a workflow with `workflow.run(responses=...)`, the framework now creates a response-entry checkpoint after queuing the responses and before running the superstep that consumes them. Restoring this checkpoint re-delivers the recorded responses and replays the rest of the workflow.

The response-entry checkpoint has the same `iteration_count` as the preceding checkpoint that contains the pending request. It is a separate checkpoint whose `previous_checkpoint_id` points to that pending-request checkpoint.

> [!IMPORTANT]
> An `iteration_count` isn't guaranteed to be unique in a human-in-the-loop checkpoint history. Follow the `previous_checkpoint_id` chain to determine checkpoint order. If you need the latest checkpoint, use the checkpoint storage API instead of selecting the largest `iteration_count`.

## Migration checklist

- Update assertions and event consumers that depend on exact superstep counts or iteration numbers.
- Increase `max_iterations` by one only for workflows that reached the previous limit.
- Replace initial source ID checks for `"Workflow"` with `INTERNAL_SOURCE_ID(start_executor.id)`.
- Treat the iteration-0 checkpoint as the pre-execution input checkpoint.
- Order human-in-the-loop checkpoints by lineage rather than assuming `iteration_count` is unique.
- Verify that replaying an entry checkpoint and a response-entry checkpoint produces the expected output and side effects.

For implementation details, see [Allow workflow checkpoint full replayability](https://github.com/microsoft/agent-framework/pull/7374).
