---
title: Upgrade Guide - Requests and Responses in Python Workflows from Version 1.0.0b251028
description: Guide on upgrading Python workflows to support Requests and Responses in Microsoft Agent Framework.
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 10/30/2025
ms.service: agent-framework
---

# Upgrade Guide: Requests and Responses in Python Workflows

This guide helps you upgrade your Python workflows from the previous `RequestInfoExecutor` pattern to the new integrated request-response API introduced in version [1.0.0b251104](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b251104).

## Overview of Changes

The request-response system has been significantly simplified:

- **No more `RequestInfoExecutor`**: Executors can now send requests directly
- **New `@response_handler` decorator**: Replace `RequestResponse` message handlers
- **Simplified request types**: No inheritance from `RequestInfoMessage` required
- **Built-in capabilities**: All executors automatically support request-response functionality
- **Cleaner workflow graphs**: Remove `RequestInfoExecutor` nodes from your workflows

## Migration Steps

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

## Key Benefits of the New Pattern

1. **Simplified Architecture**: No need for separate `RequestInfoExecutor` components
2. **Type Safety**: Direct type specification in `request_info()` calls
3. **Cleaner Code**: Fewer imports and simpler workflow graphs
4. **Better Performance**: Reduced message routing overhead
5. **Enhanced Debugging**: Clearer execution flow and error handling

## Important: Changes to Checkpoint Resume Behavior

### Checkpoint Resume with Pending Requests

**Breaking Change**: The way workflows resume from checkpoints with pending requests has also changed.

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
    responses=responses  # This is no longer supported
):
    print(f"Event: {event}")
```

**After (New Behavior):**

```python
# NEW: Pending requests are re-emitted, must be captured and responded to
requests_info_events = []
async for event in workflow.run_stream_from_checkpoint(
    checkpoint_id="checkpoint-id",
    checkpoint_storage=checkpoint_storage
):
    if isinstance(event, RequestInfoEvent):
        # Capture re-emitted pending requests
        print(f"Pending request re-emitted: {event.request_id}")
        requests_info_events.append(event)

# Handle the request and provide response
# If responses are already provided, no need to handle them again
responses = {}
for event in requests_info_events:
    response = handle_request(event.data)
    responses[event.request_id] = response

# Send response back to workflow using standard mechanism
async for event in workflow.send_responses_streaming(responses):
    if isinstance(event, WorkflowOutputEvent):
        print(f"Workflow completed: {event.data}")
```

**What Changed:**

- You can no longer supply responses as a parameter to `run_stream_from_checkpoint()` or `run_from_checkpoint()`
- When resuming from a checkpoint with pending requests, those requests will be re-emitted as `RequestInfoEvent` objects
- You must capture these re-emitted events and respond using the standard `send_responses_streaming()` method or equivalent `send_response()` calls
- If resuming from a checkpoint with pending requests that have already been responded to, you still need to call `run_stream_from_checkpoint()` to continue the workflow followed by `send_responses_streaming()` with the pre-supplied responses

**Migration Steps for Checkpoint Resume:**

1. Remove any `responses` parameter from `run_stream_from_checkpoint()` calls
2. Add event handling logic to capture re-emitted `RequestInfoEvent` objects
3. Use `send_responses_streaming()` to provide responses to re-emitted requests
4. Test the resume flow to ensure proper request handling

### Testing Your Migration

1. **Verify Imports**: Ensure no old imports remain
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
