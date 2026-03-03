---
title: Microsoft Agent Framework Workflows - Human-in-the-loop (HITL)
description: In-depth look at Human-in-the-loop interactions in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 03/03/2026
ms.service: agent-framework
---

# Microsoft Agent Framework Workflows - Human-in-the-loop (HITL)

This page provides an overview of **Human-in-the-loop (HITL)** interactions in the Microsoft Agent Framework Workflow system. HILT is achieved through the **request and response** handling mechanism in workflows, which allows executors to send requests to external systems (such as human operators) and wait for their responses before proceeding with the workflow execution.

## Overview

Executors in a workflow can send requests to outside of the workflow and wait for responses. This is useful for scenarios where an executor needs to interact with external systems, such as human-in-the-loop interactions, or any other asynchronous operations.

::: zone pivot="programming-language-csharp"

Let's build a workflow that asks a human operator to guess a number and uses an executor to judge whether the guess is correct.

## Enable Request and Response Handling in a Workflow

Requests and responses are handled via a special type called `RequestPort`.

A `RequestPort` is a communication channel that allows executors to send requests and receive responses. When an executor sends a message to a `RequestPort`, the request port emits a `RequestInfoEvent` that contains the details of the request. External systems can listen for these events, process the requests, and send responses back to the workflow. The framework automatically routes the responses back to the appropriate executor based on the original request.

```csharp
// Create a request port that receives requests of type NumberSignal and responses of type int.
var numberRequestPort = RequestPort.Create<NumberSignal, int>("GuessNumber");
```

Add the input port to a workflow.

```csharp
JudgeExecutor judgeExecutor = new(42);
var workflow = new WorkflowBuilder(numberRequestPort)
    .AddEdge(numberRequestPort, judgeExecutor)
    .AddEdge(judgeExecutor, numberRequestPort)
    .WithOutputFrom(judgeExecutor)
    .Build();
```

The definition of `JudgeExecutor` needs a target number and be able to judge whether the guess is correct. If it is not correct, it will send another request to ask for a new guess through the `RequestPort`.

```csharp
internal enum NumberSignal
{
    Init,
    Above,
    Below,
}

internal sealed class JudgeExecutor() : Executor<int>("Judge")
{
    private readonly int _targetNumber;
    private int _tries;

    public JudgeExecutor(int targetNumber) : this()
    {
        this._targetNumber = targetNumber;
    }

    public override async ValueTask HandleAsync(int message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        this._tries++;
        if (message == this._targetNumber)
        {
            await context.YieldOutputAsync($"{this._targetNumber} found in {this._tries} tries!", cancellationToken);
        }
        else if (message < this._targetNumber)
        {
            await context.SendMessageAsync(NumberSignal.Below, cancellationToken: cancellationToken);
        }
        else
        {
            await context.SendMessageAsync(NumberSignal.Above, cancellationToken: cancellationToken);
        }
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

Executors can send requests using `ctx.request_info()` and handle responses with `@response_handler`.

```python
from agent_framework import response_handler, WorkflowBuilder

executor_a = SomeExecutor()
executor_b = SomeOtherExecutor()
workflow_builder = WorkflowBuilder(start_executor=executor_a)
workflow_builder.add_edge(executor_a, executor_b)
workflow = workflow_builder.build()
```

`executor_a` can send requests and receive responses directly using built-in capabilities.

```python
from agent_framework import (
    Executor,
    WorkflowContext,
    handler,
    response_handler,
)

class SomeExecutor(Executor):

    @handler
    async def handle_data(
        self,
        data: OtherDataType,
        context: WorkflowContext,
    ):
        # Process the message...
        ...
        # Send a request using the API
        await context.request_info(
            request_data=CustomRequestType(...),
            response_type=CustomResponseType
        )

    @response_handler
    async def handle_response(
        self,
        original_request: CustomRequestType,
        response: CustomResponseType,
        context: WorkflowContext,
    ):
        # Process the response...
        ...
```

The `@response_handler` decorator automatically registers the method to handle responses for the specified request and response types.

::: zone-end

## Handling Requests and Responses

::: zone pivot="programming-language-csharp"

An `RequestPort` emits a `RequestInfoEvent` when it receives a request. You can subscribe to these events to handle incoming requests from the workflow. When you receive a response from an external system, send it back to the workflow using the response mechanism. The framework automatically routes the response to the executor that sent the original request.

```csharp
await using StreamingRun handle = await InProcessExecution.RunStreamingAsync(workflow, NumberSignal.Init);
await foreach (WorkflowEvent evt in handle.WatchStreamAsync())
{
    switch (evt)
    {
        case RequestInfoEvent requestInputEvt:
            // Handle `RequestInfoEvent` from the workflow
            int guess = ...; // Get the guess from the human operator or any external system
            await handle.SendResponseAsync(requestInputEvt.request.CreateResponse(guess));
            break;

        case WorkflowOutputEvent outputEvt:
            // The workflow has yielded output
            Console.WriteLine($"Workflow completed with result: {outputEvt.Data}");
            return;
    }
}
```

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/03-workflows/HumanInTheLoop/HumanInTheLoopBasic) for the complete runnable project.

::: zone-end

::: zone pivot="programming-language-python"

Executors can send requests directly without needing a separate component. When an executor calls `ctx.request_info()`, the workflow emits a `RequestInfoEvent`. You can subscribe to these events to handle incoming requests from the workflow. When you receive a response from an external system, send it back to the workflow using the response mechanism. The framework automatically routes the response to the executor's `@response_handler` method.

```python
from agent_framework import RequestInfoEvent


async def process_event_stream(stream: AsyncIterable[WorkflowEvent]) -> dict[str, str] | None:
    """Process events from the workflow stream to capture requests."""
    requests: list[tuple[str, HumanFeedbackRequest]] = []
    async for event in stream:
        if event.type == "request_info":
            requests.append((event.request_id, event.data))

    # Handle any pending human feedback requests.
    if requests:
        responses: dict[str, str] = {}
        for request_id, request in requests:
            responses[request_id] = ...  # Get the response for the request from the human operator or any external system.
        return responses

    return None

# Initiate the first run of the workflow.
# Runs are not isolated; state is preserved across multiple calls to run.
stream = workflow.run("start", stream=True)

pending_responses = await process_event_stream(stream)
while pending_responses is not None:
    # Run the workflow until there is no more human feedback to provide,
    # in which case this workflow completes.
    stream = workflow.run(stream=True, responses=pending_responses)
    pending_responses = await process_event_stream(stream)
```

> [!TIP]
> See this [full sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/03-workflows/human-in-the-loop/guessing_game_with_human_input.py) for a complete runnable file.

::: zone-end

## Checkpoints and Requests

To learn more about checkpoints, see [Checkpoints](./checkpoints.md).

When a checkpoint is created, pending requests are also saved as part of the checkpoint state. When you restore from a checkpoint, any pending requests will be re-emitted as `RequestInfoEvent` objects, allowing you to capture and respond to them. You cannot provide responses directly during the resume operation - instead, you must listen for the re-emitted events and respond using the standard response mechanism.

## Next Steps

- [Learn how to manage state](./state.md) in workflows.
- [Learn how to create checkpoints and resume from them](./checkpoints.md).
- [Learn how to monitor workflows](./observability.md).
- [Learn how to visualize workflows](./visualization.md).
