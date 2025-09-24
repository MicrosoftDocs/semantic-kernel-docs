---
title: Microsoft Agent Framework Workflows Orchestrations - Magentic
description: In-depth look at Magentic Orchestrations in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/12/2025
ms.service: semantic-kernel
---

# Microsoft Agent Framework Workflows Orchestrations - Magentic

Magentic orchestration is designed based on the [Magentic-One](https://microsoft.github.io/autogen/stable/user-guide/agentchat-user-guide/magentic-one.html) system invented by AutoGen. It is a flexible, general-purpose multi-agent pattern designed for complex, open-ended tasks that require dynamic collaboration. In this pattern, a dedicated Magentic manager coordinates a team of specialized agents, selecting which agent should act next based on the evolving context, task progress, and agent capabilities.

The Magentic manager maintains a shared context, tracks progress, and adapts the workflow in real time. This enables the system to break down complex problems, delegate subtasks, and iteratively refine solutions through agent collaboration. The orchestration is especially well-suited for scenarios where the solution path is not known in advance and may require multiple rounds of reasoning, research, and computation.

![Magentic Orchestration](../resources/images/orchestration-magentic.png)

## What You'll Learn

- How to set up a Magentic manager to coordinate multiple specialized agents
- How to configure callbacks for streaming and event handling
- How to implement human-in-the-loop plan review
- How to track agent collaboration and progress through complex tasks

## Define Your Specialized Agents

::: zone pivot="programming-language-csharp"

Coming soon...

::: zone-end

::: zone pivot="programming-language-python"

In Magentic orchestration, you define specialized agents that the manager can dynamically select based on task requirements:

```python
from agent_framework import ChatAgent, HostedCodeInterpreterTool
from agent_framework.openai import OpenAIChatClient, OpenAIResponsesClient

researcher_agent = ChatAgent(
    name="ResearcherAgent",
    description="Specialist in research and information gathering",
    instructions=(
        "You are a Researcher. You find information without additional computation or quantitative analysis."
    ),
    # This agent requires the gpt-4o-search-preview model to perform web searches
    chat_client=OpenAIChatClient(ai_model_id="gpt-4o-search-preview"),
)

coder_agent = ChatAgent(
    name="CoderAgent",
    description="A helpful assistant that writes and executes code to process and analyze data.",
    instructions="You solve questions using code. Please provide detailed analysis and computation process.",
    chat_client=OpenAIResponsesClient(),
    tools=HostedCodeInterpreterTool(),
)
```

## Set Up Event Callbacks

Magentic orchestration provides rich event callbacks to monitor the workflow progress in real-time:

```python
from agent_framework import (
    MagenticAgentDeltaEvent,
    MagenticAgentMessageEvent,
    MagenticCallbackEvent,
    MagenticFinalResultEvent,
    MagenticOrchestratorMessageEvent,
)

# Unified callback for all events
async def on_event(event: MagenticCallbackEvent) -> None:
    if isinstance(event, MagenticOrchestratorMessageEvent):
        # Manager's planning and coordination messages
        print(f"\n[ORCH:{event.kind}]\n\n{getattr(event.message, 'text', '')}\n{'-' * 26}")

    elif isinstance(event, MagenticAgentDeltaEvent):
        # Streaming tokens from agents
        print(event.text, end="", flush=True)

    elif isinstance(event, MagenticAgentMessageEvent):
        # Complete agent responses
        msg = event.message
        if msg is not None:
            response_text = (msg.text or "").replace("\n", " ")
            print(f"\n[AGENT:{event.agent_id}] {msg.role.value}\n\n{response_text}\n{'-' * 26}")

    elif isinstance(event, MagenticFinalResultEvent):
        # Final synthesized result
        print("\n" + "=" * 50)
        print("FINAL RESULT:")
        print("=" * 50)
        if event.message is not None:
            print(event.message.text)
        print("=" * 50)
```

## Build the Magentic Workflow

Use `MagenticBuilder` to configure the workflow with a standard manager:

```python
from agent_framework import MagenticBuilder, MagenticCallbackMode

workflow = (
    MagenticBuilder()
    .participants(researcher=researcher_agent, coder=coder_agent)
    .on_event(on_event, mode=MagenticCallbackMode.STREAMING)
    .with_standard_manager(
        chat_client=OpenAIChatClient(),
        max_round_count=10,  # Maximum collaboration rounds
        max_stall_count=3,   # Maximum rounds without progress
        max_reset_count=2,   # Maximum plan resets allowed
    )
    .build()
)
```

## Run the Workflow

Execute a complex task that requires multiple agents working together:

```python
from agent_framework import WorkflowCompletedEvent

task = (
    "I am preparing a report on the energy efficiency of different machine learning model architectures. "
    "Compare the estimated training and inference energy consumption of ResNet-50, BERT-base, and GPT-2 "
    "on standard datasets (e.g., ImageNet for ResNet, GLUE for BERT, WebText for GPT-2). "
    "Then, estimate the CO2 emissions associated with each, assuming training on an Azure Standard_NC6s_v3 "
    "VM for 24 hours. Provide tables for clarity, and recommend the most energy-efficient model "
    "per task type (image classification, text classification, and text generation)."
)

completion_event = None
async for event in workflow.run_stream(task):
    if isinstance(event, WorkflowCompletedEvent):
        completion_event = event

if completion_event is not None:
    data = getattr(completion_event, "data", None)
    preview = getattr(data, "text", None) or (str(data) if data is not None else "")
    print(f"Workflow completed with result:\n\n{preview}")
```

## Advanced: Human-in-the-Loop Plan Review

Enable human review and approval of the manager's plan before execution:

### Configure Plan Review

```python
from agent_framework import (
    MagenticPlanReviewDecision,
    MagenticPlanReviewReply,
    MagenticPlanReviewRequest,
    RequestInfoEvent,
)

workflow = (
    MagenticBuilder()
    .participants(researcher=researcher_agent, coder=coder_agent)
    .on_event(on_event, mode=MagenticCallbackMode.STREAMING)
    .with_standard_manager(
        chat_client=OpenAIChatClient(),
        max_round_count=10,
        max_stall_count=3,
        max_reset_count=2,
    )
    .with_plan_review()  # Enable plan review
    .build()
)
```

### Handle Plan Review Requests

```python
completion_event: WorkflowCompletedEvent | None = None
pending_request: RequestInfoEvent | None = None

while True:
    # Run until completion or review request
    if pending_request is None:
        async for event in workflow.run_stream(task):
            if isinstance(event, WorkflowCompletedEvent):
                completion_event = event

            if isinstance(event, RequestInfoEvent) and event.request_type is MagenticPlanReviewRequest:
                pending_request = event
                review_req = cast(MagenticPlanReviewRequest, event.data)
                if review_req.plan_text:
                    print(f"\n=== PLAN REVIEW REQUEST ===\n{review_req.plan_text}\n")

    # Check if completed
    if completion_event is not None:
        break

    # Respond to plan review
    if pending_request is not None:
        # Collect human decision (approve/reject/modify)
        # For demo, we auto-approve:
        reply = MagenticPlanReviewReply(decision=MagenticPlanReviewDecision.APPROVE)

        # Or modify the plan:
        # reply = MagenticPlanReviewReply(
        #     decision=MagenticPlanReviewDecision.APPROVE,
        #     edited_plan="Modified plan text here..."
        # )

        async for event in workflow.send_responses_streaming({pending_request.request_id: reply}):
            if isinstance(event, WorkflowCompletedEvent):
                completion_event = event
            elif isinstance(event, RequestInfoEvent):
                # Another review cycle if needed
                pending_request = event
            else:
                pending_request = None
```

## Key Concepts

- **Dynamic Coordination**: The Magentic manager dynamically selects which agent should act next based on the evolving context
- **Iterative Refinement**: The system can break down complex problems and iteratively refine solutions through multiple rounds
- **Progress Tracking**: Built-in mechanisms to detect stalls and reset the plan if needed
- **Flexible Collaboration**: Agents can be called multiple times in any order as determined by the manager
- **Human Oversight**: Optional human-in-the-loop review allows manual intervention and plan modification

## Workflow Execution Flow

The Magentic orchestration follows this execution pattern:

1. **Planning Phase**: The manager analyzes the task and creates an initial plan
2. **Agent Selection**: The manager selects the most appropriate agent for each subtask
3. **Execution**: The selected agent executes their portion of the task
4. **Progress Assessment**: The manager evaluates progress and updates the plan
5. **Iteration**: Steps 2-4 repeat until the task is complete or limits are reached
6. **Final Synthesis**: The manager synthesizes all agent outputs into a final result

## Error Handling

Add error handling to make your workflow robust:

```python
def on_exception(exception: Exception) -> None:
    print(f"Exception occurred: {exception}")
    logger.exception("Workflow exception", exc_info=exception)

workflow = (
    MagenticBuilder()
    .participants(researcher=researcher_agent, coder=coder_agent)
    .on_exception(on_exception)
    .on_event(on_event, mode=MagenticCallbackMode.STREAMING)
    .with_standard_manager(
        chat_client=OpenAIChatClient(),
        max_round_count=10,
        max_stall_count=3,
        max_reset_count=2,
    )
    .build()
)
```

## Complete Example

Here's a full example that brings together all the concepts:

```python
import asyncio
import logging
from typing import cast

from agent_framework import (
    ChatAgent,
    HostedCodeInterpreterTool,
    MagenticAgentDeltaEvent,
    MagenticAgentMessageEvent,
    MagenticBuilder,
    MagenticCallbackEvent,
    MagenticCallbackMode,
    MagenticFinalResultEvent,
    MagenticOrchestratorMessageEvent,
    WorkflowCompletedEvent,
)
from agent_framework.openai import OpenAIChatClient, OpenAIResponsesClient

logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger(__name__)

async def main() -> None:
    # Define specialized agents
    researcher_agent = ChatAgent(
        name="ResearcherAgent",
        description="Specialist in research and information gathering",
        instructions=(
            "You are a Researcher. You find information without additional "
            "computation or quantitative analysis."
        ),
        chat_client=OpenAIChatClient(ai_model_id="gpt-4o-search-preview"),
    )

    coder_agent = ChatAgent(
        name="CoderAgent",
        description="A helpful assistant that writes and executes code to process and analyze data.",
        instructions="You solve questions using code. Please provide detailed analysis and computation process.",
        chat_client=OpenAIResponsesClient(),
        tools=HostedCodeInterpreterTool(),
    )

    # State for streaming callback
    last_stream_agent_id: str | None = None
    stream_line_open: bool = False

    # Unified callback for all events
    async def on_event(event: MagenticCallbackEvent) -> None:
        nonlocal last_stream_agent_id, stream_line_open

        if isinstance(event, MagenticOrchestratorMessageEvent):
            print(f"\n[ORCH:{event.kind}]\n\n{getattr(event.message, 'text', '')}\n{'-' * 26}")

        elif isinstance(event, MagenticAgentDeltaEvent):
            if last_stream_agent_id != event.agent_id or not stream_line_open:
                if stream_line_open:
                    print()
                print(f"\n[STREAM:{event.agent_id}]: ", end="", flush=True)
                last_stream_agent_id = event.agent_id
                stream_line_open = True
            print(event.text, end="", flush=True)

        elif isinstance(event, MagenticAgentMessageEvent):
            if stream_line_open:
                print(" (final)")
                stream_line_open = False
                print()
            msg = event.message
            if msg is not None:
                response_text = (msg.text or "").replace("\n", " ")
                print(f"\n[AGENT:{event.agent_id}] {msg.role.value}\n\n{response_text}\n{'-' * 26}")

        elif isinstance(event, MagenticFinalResultEvent):
            print("\n" + "=" * 50)
            print("FINAL RESULT:")
            print("=" * 50)
            if event.message is not None:
                print(event.message.text)
            print("=" * 50)

    # Build the workflow
    print("\nBuilding Magentic Workflow...")

    workflow = (
        MagenticBuilder()
        .participants(researcher=researcher_agent, coder=coder_agent)
        .on_event(on_event, mode=MagenticCallbackMode.STREAMING)
        .with_standard_manager(
            chat_client=OpenAIChatClient(),
            max_round_count=10,
            max_stall_count=3,
            max_reset_count=2,
        )
        .build()
    )

    # Define the task
    task = (
        "I am preparing a report on the energy efficiency of different machine learning model architectures. "
        "Compare the estimated training and inference energy consumption of ResNet-50, BERT-base, and GPT-2 "
        "on standard datasets (e.g., ImageNet for ResNet, GLUE for BERT, WebText for GPT-2). "
        "Then, estimate the CO2 emissions associated with each, assuming training on an Azure Standard_NC6s_v3 "
        "VM for 24 hours. Provide tables for clarity, and recommend the most energy-efficient model "
        "per task type (image classification, text classification, and text generation)."
    )

    print(f"\nTask: {task}")
    print("\nStarting workflow execution...")

    # Run the workflow
    try:
        completion_event = None
        async for event in workflow.run_stream(task):
            print(f"Event: {event}")

            if isinstance(event, WorkflowCompletedEvent):
                completion_event = event

        if completion_event is not None:
            data = getattr(completion_event, "data", None)
            preview = getattr(data, "text", None) or (str(data) if data is not None else "")
            print(f"Workflow completed with result:\n\n{preview}")

    except Exception as e:
        print(f"Workflow execution failed: {e}")
        logger.exception("Workflow exception", exc_info=e)

if __name__ == "__main__":
    asyncio.run(main())
```

## Configuration Options

### Manager Parameters
- `max_round_count`: Maximum number of collaboration rounds (default: 10)
- `max_stall_count`: Maximum rounds without progress before reset (default: 3)
- `max_reset_count`: Maximum number of plan resets allowed (default: 2)

### Callback Modes
- `MagenticCallbackMode.STREAMING`: Receive incremental token updates
- `MagenticCallbackMode.COMPLETE`: Receive only complete messages

### Plan Review Decisions
- `APPROVE`: Accept the plan as-is
- `REJECT`: Reject and request a new plan
- `APPROVE` with `edited_plan`: Accept with modifications

## Sample Output

Coming soon...

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Handoff Orchestration](./handoff.md)
