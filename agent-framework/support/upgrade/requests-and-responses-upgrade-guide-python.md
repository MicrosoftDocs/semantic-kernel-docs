---
title: Upgrade Guide - Workflow APIs and Request-Response System in Python
description: Guide on upgrading to consolidated workflow APIs and the new request-response system in Microsoft Agent Framework.
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 11/06/2025
ms.service: agent-framework
---

# Upgrade Guide: Workflow APIs and Request-Response System

This guide helps you upgrade your Python workflows to the latest API changes introduced in version [1.0.0b251104](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b251104).

## Overview of Changes

This release includes two major improvements to the workflow system:

### 1. Consolidated Workflow Execution APIs

The workflow execution methods have been unified for simplicity:

- **Unified `run_stream()` and `run()` methods**: Replace separate checkpoint-specific methods (`run_stream_from_checkpoint()`, `run_from_checkpoint()`)
- **Single interface**: Use `checkpoint_id` parameter to resume from checkpoints instead of separate methods
- **Flexible checkpointing**: Configure checkpoint storage at build time or override at runtime
- **Clearer semantics**: Mutually exclusive `message` (new run) and `checkpoint_id` (resume) parameters

### 2. Simplified Request-Response System

The request-response system has been streamlined:

- **No more `RequestInfoExecutor`**: Executors can now send requests directly
- **New `@response_handler` decorator**: Replace `RequestResponse` message handlers
- **Simplified request types**: No inheritance from `RequestInfoMessage` required
- **Built-in capabilities**: All executors automatically support request-response functionality
- **Cleaner workflow graphs**: Remove `RequestInfoExecutor` nodes from your workflows

## Part 1: Unified Workflow Execution APIs

We recommend migrating to the consolidated workflow APIs first, as this forms the foundation for all workflow execution patterns.

### Resuming from Checkpoints

**Before (Old API):**

```python
# OLD: Separate method for checkpoint resume
async for event in workflow.run_stream_from_checkpoint(
    checkpoint_id="checkpoint-id",
    checkpoint_storage=checkpoint_storage
):
    print(f"Event: {event}")
```

**After (New API):**

```python
# NEW: Unified method with checkpoint_id parameter
async for event in workflow.run_stream(
    checkpoint_id="checkpoint-id",
    checkpoint_storage=checkpoint_storage  # Optional if configured at build time
):
    print(f"Event: {event}")
```

**Key differences:**

- Use `checkpoint_id` parameter instead of separate method
- Cannot provide both `message` and `checkpoint_id` (mutually exclusive)
- Must provide either `message` (new run) or `checkpoint_id` (resume)
- `checkpoint_storage` is optional if checkpointing was configured at build time

### Non-Streaming API

The non-streaming `run()` method follows the same pattern:

**Old:**

```python
result = await workflow.run_from_checkpoint(
    checkpoint_id="checkpoint-id",
    checkpoint_storage=checkpoint_storage
)
```

**New:**

```python
result = await workflow.run(
    checkpoint_id="checkpoint-id",
    checkpoint_storage=checkpoint_storage  # Optional if configured at build time
)
```

### Checkpoint Resume with Pending Requests

**Important Breaking Change**: When resuming from a checkpoint that has pending `RequestInfoEvent` objects, the new API re-emits these events automatically. You must capture and respond to them.

**Before (Old Behavior):**

```python
# OLD: Could provide responses directly during resume
responses = {
    "request-id-1": "user response data",
    "request-id-2": "another response"
}

async for event in workflow.run_stream_from_checkpoint(
    checkpoint_id="checkpoint-id",
    checkpoint_storage=checkpoint_storage,
    responses=responses  # No longer supported
):
    print(f"Event: {event}")
```

**After (New Behavior):**

```python
# NEW: Capture re-emitted pending requests
requests: dict[str, Any] = {}

async for event in workflow.run_stream(checkpoint_id="checkpoint-id"):
    if isinstance(event, RequestInfoEvent):
        # Pending requests are automatically re-emitted
        print(f"Pending request re-emitted: {event.request_id}")
        requests[event.request_id] = event.data

# Collect user responses
responses: dict[str, Any] = {}
for request_id, request_data in requests.items():
    response = handle_request(request_data)  # Your logic here
    responses[request_id] = response

# Send responses back to workflow
async for event in workflow.send_responses_streaming(responses):
    if isinstance(event, WorkflowOutputEvent):
        print(f"Workflow output: {event.data}")
```

### Complete Human-in-the-Loop Example

Here's a complete example showing checkpoint resume with pending human approval:

```python
from agent_framework import (
    Executor,
    FileCheckpointStorage,
    RequestInfoEvent,
    WorkflowBuilder,
    WorkflowOutputEvent,
    WorkflowStatusEvent,
    handler,
    response_handler,
)

# ... (Executor definitions omitted for brevity)

async def run_interactive_session(
    workflow: Workflow,
    initial_message: str | None = None,
    checkpoint_id: str | None = None,
) -> str:
    """Run workflow until completion, handling human input interactively."""
    
    requests: dict[str, HumanApprovalRequest] = {}
    responses: dict[str, str] | None = None
    completed_output: str | None = None

    while True:
        # Determine which API to call
        if responses:
            # Send responses from previous iteration
            event_stream = workflow.send_responses_streaming(responses)
            requests.clear()
            responses = None
        else:
            # Start new run or resume from checkpoint
            if initial_message:
                event_stream = workflow.run_stream(initial_message)
            elif checkpoint_id:
                event_stream = workflow.run_stream(checkpoint_id=checkpoint_id)
            else:
                raise ValueError("Either initial_message or checkpoint_id required")

        # Process events
        async for event in event_stream:
            if isinstance(event, WorkflowStatusEvent):
                print(event)
            if isinstance(event, WorkflowOutputEvent):
                completed_output = event.data
            if isinstance(event, RequestInfoEvent):
                if isinstance(event.data, HumanApprovalRequest):
                    requests[event.request_id] = event.data

        # Check completion
        if completed_output:
            break

        # Prompt for user input if we have pending requests
        if requests:
            responses = prompt_for_responses(requests)
            continue

        raise RuntimeError("Workflow stopped without completing or requesting input")

    return completed_output
```

## Part 2: Simplified Request-Response System

After migrating to the unified workflow APIs, update your request-response patterns to use the new integrated system.

### 1. Update Imports

**Before:**

```python
from agent_framework import (
    RequestInfoExecutor,
    RequestInfoMessage,
    RequestResponse,
    # ... other imports
)
```

**After:**

```python
from agent_framework import (
    response_handler,
    # ... other imports
    # Remove: RequestInfoExecutor, RequestInfoMessage, RequestResponse
)
```

### 2. Update Request Types

**Before:**

```python
from dataclasses import dataclass
from agent_framework import RequestInfoMessage

@dataclass
class UserApprovalRequest(RequestInfoMessage):
    """Request for user approval."""
    prompt: str = ""
    context: str = ""
```

**After:**

```python
from dataclasses import dataclass

@dataclass
class UserApprovalRequest:
    """Request for user approval."""
    prompt: str = ""
    context: str = ""
```

### 3. Update Workflow Graph

**Before:**

```python
# Old pattern: Required RequestInfoExecutor in workflow
approval_executor = ApprovalRequiredExecutor(id="approval")
request_info_executor = RequestInfoExecutor(id="request_info")

workflow = (
    WorkflowBuilder()
    .set_start_executor(approval_executor)
    .add_edge(approval_executor, request_info_executor)
    .add_edge(request_info_executor, approval_executor)
    .build()
)
```

**After:**

```python
# New pattern: Direct request-response capabilities
approval_executor = ApprovalRequiredExecutor(id="approval")

workflow = (
    WorkflowBuilder()
    .set_start_executor(approval_executor)
    .build()
)
```

### 4. Update Request Sending

**Before:**

```python
class ApprovalRequiredExecutor(Executor):
    @handler
    async def process(self, message: str, ctx: WorkflowContext[UserApprovalRequest]) -> None:
        request = UserApprovalRequest(
            prompt=f"Please approve: {message}",
            context="Important operation"
        )
        await ctx.send_message(request)
```

**After:**

```python
class ApprovalRequiredExecutor(Executor):
    @handler
    async def process(self, message: str, ctx: WorkflowContext) -> None:
        request = UserApprovalRequest(
            prompt=f"Please approve: {message}",
            context="Important operation"
        )
        await ctx.request_info(request_data=request, response_type=bool)
```

### 5. Update Response Handling

**Before:**

```python
class ApprovalRequiredExecutor(Executor):
    @handler
    async def handle_approval(
        self,
        response: RequestResponse[UserApprovalRequest, bool],
        ctx: WorkflowContext[Never, str]
    ) -> None:
        if response.data:
            await ctx.yield_output("Approved!")
        else:
            await ctx.yield_output("Rejected!")
```

**After:**

```python
class ApprovalRequiredExecutor(Executor):
    @response_handler
    async def handle_approval(
        self,
        original_request: UserApprovalRequest,
        approved: bool,
        ctx: WorkflowContext
    ) -> None:
        if approved:
            await ctx.yield_output("Approved!")
        else:
            await ctx.yield_output("Rejected!")
```

## Summary of Benefits

### Unified Workflow APIs

1. **Simplified Interface**: Single method for initial runs and checkpoint resume
2. **Clearer Semantics**: Mutually exclusive parameters make intent explicit
3. **Flexible Checkpointing**: Configure at build time or override at runtime
4. **Reduced Cognitive Load**: Fewer methods to remember and maintain

### Request-Response System

1. **Simplified Architecture**: No need for separate `RequestInfoExecutor` components
2. **Type Safety**: Direct type specification in `request_info()` calls
3. **Cleaner Code**: Fewer imports and simpler workflow graphs
4. **Better Performance**: Reduced message routing overhead
5. **Enhanced Debugging**: Clearer execution flow and error handling

## Testing Your Migration

### Part 1 Checklist: Workflow APIs

1. **Update API Calls**: Replace `run_stream_from_checkpoint()` with `run_stream(checkpoint_id=...)`
2. **Update API Calls**: Replace `run_from_checkpoint()` with `run(checkpoint_id=...)`
3. **Remove `responses` parameter**: Delete any `responses` arguments from checkpoint resume calls
4. **Add event capture**: Implement logic to capture re-emitted `RequestInfoEvent` objects
5. **Test checkpoint resume**: Verify pending requests are re-emitted and handled correctly

### Part 2 Checklist: Request-Response System

1. **Verify Imports**: Ensure no old imports remain (`RequestInfoExecutor`, `RequestInfoMessage`, `RequestResponse`)
2. **Check Request Types**: Confirm removal of `RequestInfoMessage` inheritance
3. **Test Workflow Graph**: Verify removal of `RequestInfoExecutor` nodes
4. **Validate Handlers**: Ensure `@response_handler` decorators are applied
5. **Test End-to-End**: Run complete workflow scenarios

## Next Steps

After completing the migration:

1. Review the updated [Requests and Responses Tutorial](../../tutorials/workflows/requests-and-responses.md)
2. Explore advanced patterns in the [User Guide](../../user-guide/workflows/requests-and-responses.md)
3. Check out updated samples in the [repository](https://github.com/microsoft/agent-framework/tree/main/python/samples)

For additional help, refer to the [Agent Framework documentation](../../overview/agent-framework-overview.md) or reach out to the team and community.
