---
title: Microsoft Agent Framework Workflows - Request and Response
description: In-depth look at Request and Response handling in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/12/2025
ms.service: agent-framework
---

# Microsoft Agent Framework Workflows - Request and Response

This page provides an overview of how **Request and Response** handling works in the Microsoft Agent Framework Workflow system.

## Overview

Executors in a workflow can send requests to outside of the workflow and wait for responses. This is useful for scenarios where an executor needs to interact with external systems, such as human-in-the-loop interactions, or any other asynchronous operations.

::: zone pivot="programming-language-csharp"

## Enable Request and Response Handling in a Workflow

Requests and responses are handled via a special type called `InputPort`.

```csharp
// Create an input port that receives requests of type CustomRequestType and responses of type CustomResponseType.
var inputPort = InputPort.Create<CustomRequestType, CustomResponseType>("input-port");
```

Add the input port to a workflow.

```csharp
var executorA = new SomeExecutor();
var workflow = new WorkflowBuilder(inputPort)
    .AddEdge(inputPort, executorA)
    .AddEdge(executorA, inputPort)
    .Build<CustomRequestType>();
```

Now, because in the workflow we have `executorA` connected to the `inputPort` in both directions, `executorA` needs to be able to send requests and receive responses via the `inputPort`. Here is what we need to do in `SomeExecutor` to send a request and receive a response.

```csharp
internal sealed class SomeExecutor() : ReflectingExecutor<SomeExecutor>("SomeExecutor"), IMessageHandler<CustomResponseType>
{
    public async ValueTask HandleAsync(CustomResponseType message, IWorkflowContext context)
    {
        // Process the response...
        ...
        // Send a request
        await context.SendMessageAsync(new CustomRequestType(...)).ConfigureAwait(false);
    }
}
```

Alternatively, `SomeExecutor` can separate the request sending and response handling into two handlers.

```csharp
internal sealed class SomeExecutor() : ReflectingExecutor<SomeExecutor>("SomeExecutor"), IMessageHandler<CustomResponseType>, IMessageHandler<OtherDataType>
{
    public async ValueTask HandleAsync(CustomResponseType message, IWorkflowContext context)
    {
        // Process the response...
        ...
    }

    public async ValueTask HandleAsync(OtherDataType message, IWorkflowContext context)
    {
        // Process the message...
        ...
        // Send a request
        await context.SendMessageAsync(new CustomRequestType(...)).ConfigureAwait(false);
    }
}

```

::: zone-end

::: zone pivot="programming-language-python"

Requests and responses are handled via a special built-in executor called `RequestInfoExecutor`.

```python
from agent_framework import RequestInfoExecutor

# Create a RequestInfoExecutor with an ID
request_info_executor = RequestInfoExecutor(id="request-info-executor")
```

Add the `RequestInfoExecutor` to a workflow.

```python
from agent_framework import WorkflowBuilder

executor_a = SomeExecutor()
workflow_builder = WorkflowBuilder()
workflow_builder.set_start_executor(request_info_executor)
workflow_builder.add_edge(request_info_executor, executor_a)
workflow = workflow_builder.build()
```

Now, because in the workflow we have `executor_a` connected to the `request_info_executor` in both directions, `executor_a` needs to be able to send requests and receive responses via the `request_info_executor`. Here is what we need to do in `SomeExecutor` to send a request and receive a response.

```python
from agent_framework import (
    Executor,
    RequestResponse,
    WorkflowContext,
    handler,
)

class SomeExecutor(Executor):

    @handler
    async def handle(
        self,
        request: RequestResponse[CustomRequestType, CustomResponseType],
        context: WorkflowContext[CustomResponseType],
    ):
        # Process the response...
        ...
        # Send a request
        await context.send_message(CustomRequestType(...))
```

Alternatively, `SomeExecutor` can separate the request sending and response handling into two handlers.

```python
class SomeExecutor(Executor):

    @handler
    async def handle_response(
        self,
        response: CustomResponseType[CustomRequestType, CustomResponseType],
        context: WorkflowContext,
    ):
        # Process the response...
        ...

    @handler
    async def handle_other_data(
        self,
        data: OtherDataType,
        context: WorkflowContext[CustomRequestType],
    ):
        # Process the message...
        ...
        # Send a request
        await context.send_message(CustomRequestType(...))
```

::: zone-end

## Handling Requests and Responses

::: zone pivot="programming-language-csharp"

An `InputPort` emits a `RequestInfoEvent` when it receives a request. You can subscribe to these events to handle incoming requests from the workflow. When you receive a response from an external system, send it back to the workflow using the response mechanism. The framework automatically routes the response to the executor that sent the original request.

```csharp
StreamingRun handle = await InProcessExecution.StreamAsync(workflow, input).ConfigureAwait(false);
await foreach (WorkflowEvent evt in handle.WatchStreamAsync().ConfigureAwait(false))
{
    switch (evt)
    {
        case RequestInfoEvent requestInputEvt:
            // Handle `RequestInfoEvent` from the workflow
            ExternalResponse response = requestInputEvt.Request.CreateResponse<CustomResponseType>(...);
            await handle.SendResponseAsync(response).ConfigureAwait(false);
            break;

        case WorkflowCompletedEvent workflowCompleteEvt:
            // The workflow has completed successfully
            Console.WriteLine($"Workflow completed with result: {workflowCompleteEvt.Data}");
            return;
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

The `RequestInfoExecutor` emits a `RequestInfoEvent` when it receives a request. You can subscribe to these events to handle incoming requests from the workflow. When you receive a response from an external system, send it back to the workflow using the response mechanism. The framework automatically routes the response to the executor that sent the original request.

```python
from agent_framework import RequestInfoEvent

while True:
    request_info_events : list[RequestInfoEvent] = []
    pending_responses : dict[str, CustomResponseType] = {}

    stream = workflow.run_stream(input) if not pending_responses else workflow.send_responses_streaming(pending_responses)

    async for event in stream:
        if isinstance(event, RequestInfoEvent):
            # Handle `RequestInfoEvent` from the workflow
            request_info_events.append(event)

    if not request_info_events:
        break

    for request_info_event in request_info_events:
        # Handle `RequestInfoEvent` from the workflow
        response = CustomResponseType(...)
        pending_responses[request_info_event.request_id] = response
```

::: zone-end

## Checkpoints and Requests

To learn more about checkpoints, please refer to this [page](./checkpoints.md).

When a checkpoint is created, pending requests are also saved as part of the checkpoint state. When you restore from a checkpoint, any pending requests will be re-emitted, allowing the workflow to continue processing from where it left off.

## Next Steps

- [Learn how to use agents in workflows](./using-agents.md) to build intelligent workflows.
- [Learn how to use workflows as agents](./as-agents.md).
- [Learn how to manage state](./shared-states.md) in workflows.
- [Learn how to create checkpoints and resume from them](./checkpoints.md).
