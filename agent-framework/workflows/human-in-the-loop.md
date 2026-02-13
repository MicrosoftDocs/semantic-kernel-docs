---
title: "Human-in-the-Loop Workflows"
description: "Learn how to add human approval, review, and input steps to Agent Framework workflows."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Human-in-the-Loop Workflows

Add human review, approval, or input steps into your workflow execution.

Human-in-the-loop (HITL) patterns let you pause a workflow at key decision points, collect human input, and then resume execution. This is essential for scenarios where AI outputs need review before proceeding.

:::zone pivot="programming-language-csharp"

```csharp
using System;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

// Create the agent
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You guess a number between 1 and 10.");

// Build a workflow with human-in-the-loop approval
var workflow = AgentWorkflowBuilder.BuildSequential([agent]);

// Run the workflow, pausing for human input at each step
await foreach (var update in workflow.RunStreamingAsync("Guess a number between 1 and 10."))
{
    Console.Write(update);

    // Prompt the user for feedback before continuing
    Console.Write("\nYour feedback (higher/lower/correct): ");
    var feedback = Console.ReadLine();

    if (feedback?.Equals("correct", StringComparison.OrdinalIgnoreCase) == true)
    {
        Console.WriteLine("Guessed correctly!");
        break;
    }
}
```

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/03-workflows/human-in-the-loop/HumanInTheLoop.cs) for the complete runnable file.

:::zone-end

:::zone pivot="programming-language-python"

```python
# Copyright (c) Microsoft. All rights reserved.

import asyncio
from collections.abc import AsyncIterable
from dataclasses import dataclass

from agent_framework import (
    AgentExecutorRequest,
    AgentExecutorResponse,
    AgentResponseUpdate,
    Executor,
    Message,
    WorkflowBuilder,
    WorkflowContext,
    WorkflowEvent,
    handler,
    response_handler,
)
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity import AzureCliCredential
from pydantic import BaseModel

"""
Sample: Human in the loop guessing game

An agent guesses a number, then a human guides it with higher, lower, or
correct. The loop continues until the human confirms correct, at which point
the workflow completes when idle with no pending work.

Purpose:
Show how to integrate a human step in the middle of an LLM workflow by using
`request_info` and `run(responses=..., stream=True)`.

Demonstrate:
- Alternating turns between an AgentExecutor and a human, driven by events.
- Using Pydantic response_format to enforce structured JSON output from the agent instead of regex parsing.
- Driving the loop in application code with run and responses parameter.

Prerequisites:
- Azure OpenAI configured for AzureOpenAIChatClient with required environment variables.
- Authentication via azure-identity. Use AzureCliCredential and run az login before executing the sample.
- Basic familiarity with WorkflowBuilder, executors, edges, events, and streaming runs.
"""

# How human-in-the-loop is achieved via `request_info` and `run(responses=..., stream=True)`:
# - An executor (TurnManager) calls `ctx.request_info` with a payload (HumanFeedbackRequest).
# - The workflow run pauses and emits a  with the payload and the request_id.
# - The application captures the event, prompts the user, and collects replies.
# - The application calls `run(stream=True, responses=...)` with a map of request_ids to replies.
# - The workflow resumes, and the response is delivered to the executor method decorated with @response_handler.
# - The executor can then continue the workflow, e.g., by sending a new message to the agent.


@dataclass
class HumanFeedbackRequest:
    """Request sent to the human for feedback on the agent's guess."""

    prompt: str


class GuessOutput(BaseModel):
    """Structured output from the agent. Enforced via response_format for reliable parsing."""

    guess: int


class TurnManager(Executor):
    """Coordinates turns between the agent and the human.

    Responsibilities:
    - Kick off the first agent turn.
    - After each agent reply, request human feedback with a HumanFeedbackRequest.
    - After each human reply, either finish the game or prompt the agent again with feedback.
    """

    def __init__(self, id: str | None = None):
        super().__init__(id=id or "turn_manager")

    @handler
    async def start(self, _: str, ctx: WorkflowContext[AgentExecutorRequest]) -> None:
        """Start the game by asking the agent for an initial guess.

        Contract:
        - Input is a simple starter token (ignored here).
        - Output is an AgentExecutorRequest that triggers the agent to produce a guess.
        """
        user = Message("user", text="Start by making your first guess.")
        await ctx.send_message(AgentExecutorRequest(messages=[user], should_respond=True))

    @handler
    async def on_agent_response(
        self,
        result: AgentExecutorResponse,
        ctx: WorkflowContext,
    ) -> None:
        """Handle the agent's guess and request human guidance.

        Steps:
        1) Parse the agent's JSON into GuessOutput for robustness.
        2) Request info with a HumanFeedbackRequest as the payload.
        """
        # Parse structured model output
        text = result.agent_response.text
        last_guess = GuessOutput.model_validate_json(text).guess

        # Craft a precise human prompt that defines higher and lower relative to the agent's guess.
        prompt = (
            f"The agent guessed: {last_guess}. "
            "Type one of: higher (your number is higher than this guess), "
            "lower (your number is lower than this guess), correct, or exit."
        )
        # Send a request with a prompt as the payload and expect a string reply.
        await ctx.request_info(
            request_data=HumanFeedbackRequest(prompt=prompt),
            response_type=str,
        )

    @response_handler
    async def on_human_feedback(
        self,
        original_request: HumanFeedbackRequest,
        feedback: str,
        ctx: WorkflowContext[AgentExecutorRequest, str],
    ) -> None:
        """Continue the game or finish based on human feedback."""
        reply = feedback.strip().lower()

        if reply == "correct":
            await ctx.yield_output("Guessed correctly!")
            return

        # Provide feedback to the agent to try again.
        # response_format=GuessOutput on the agent ensures JSON output, so we just need to guide the logic.
        last_guess = original_request.prompt.split(": ")[1].split(".")[0]
        feedback_text = (
            f"Feedback: {reply}. Your last guess was {last_guess}. "
            f"Use this feedback to adjust and make your next guess (1-10)."
        )
        user_msg = Message("user", text=feedback_text)
        await ctx.send_message(AgentExecutorRequest(messages=[user_msg], should_respond=True))


async def process_event_stream(stream: AsyncIterable[WorkflowEvent]) -> dict[str, str] | None:
    """Process events from the workflow stream to capture human feedback requests."""
    # Track the last author to format streaming output.
    last_response_id: str | None = None

    requests: list[tuple[str, HumanFeedbackRequest]] = []
    async for event in stream:
        if event.type == "request_info" and isinstance(event.data, HumanFeedbackRequest):
            requests.append((event.request_id, event.data))
        elif event.type == "output":
            if isinstance(event.data, AgentResponseUpdate):
                update = event.data
                response_id = update.response_id
                if response_id != last_response_id:
                    if last_response_id is not None:
                        print()  # Newline between different responses
                    print(f"{update.author_name}: {update.text}", end="", flush=True)
                    last_response_id = response_id
                else:
                    print(update.text, end="", flush=True)
            else:
                print(f"\n{event.executor_id}: {event.data}")

    # Handle any pending human feedback requests.
    if requests:
        responses: dict[str, str] = {}
        for request_id, request in requests:
            print(f"\nHITL: {request.prompt}")
            # Instructional print already appears above. The input line below is the user entry point.
            # If desired, you can add more guidance here, but keep it concise.
            answer = input("Enter higher/lower/correct/exit: ").lower()  # noqa: ASYNC250
            if answer == "exit":
                print("Exiting...")
                return None
            responses[request_id] = answer
        return responses

    return None


async def main() -> None:
    """Run the human-in-the-loop guessing game workflow."""
    # Create agent and executor
    guessing_agent = AzureOpenAIChatClient(credential=AzureCliCredential()).as_agent(
        name="GuessingAgent",
        instructions=(
            "You guess a number between 1 and 10. "
            "If the user says 'higher' or 'lower', adjust your next guess. "
            'You MUST return ONLY a JSON object exactly matching this schema: {"guess": <integer 1..10>}. '
            "No explanations or additional text."
        ),
        # response_format enforces that the model produces JSON compatible with GuessOutput.
        default_options={"response_format": GuessOutput},
    )
    turn_manager = TurnManager(id="turn_manager")

    # Build a simple loop: TurnManager <-> AgentExecutor.
    workflow = (
        WorkflowBuilder(start_executor=turn_manager)
        .add_edge(turn_manager, guessing_agent)  # Ask agent to make/adjust a guess
        .add_edge(guessing_agent, turn_manager)  # Agent's response comes back to coordinator
    ).build()

    # Initiate the first run of the workflow.
    # Runs are not isolated; state is preserved across multiple calls to run.
    stream = workflow.run("start", stream=True)

    pending_responses = await process_event_stream(stream)
    while pending_responses is not None:
        # Run the workflow until there is no more human feedback to provide,
        # in which case this workflow completes.
        stream = workflow.run(stream=True, responses=pending_responses)
        pending_responses = await process_event_stream(stream)

    """
    Sample Output:

    HITL> The agent guessed: 5. Type one of: higher (your number is higher than this guess), lower (your number is lower than this guess), correct, or exit.
    Enter higher/lower/correct/exit: higher
    HITL> The agent guessed: 8. Type one of: higher (your number is higher than this guess), lower (your number is lower than this guess), correct, or exit.
    Enter higher/lower/correct/exit: higher
    HITL> The agent guessed: 10. Type one of: higher (your number is higher than this guess), lower (your number is lower than this guess), correct, or exit.
    Enter higher/lower/correct/exit: lower
    HITL> The agent guessed: 9. Type one of: higher (your number is higher than this guess), lower (your number is lower than this guess), correct, or exit.
    Enter higher/lower/correct/exit: correct
    Workflow output: Guessed correctly: 9
    """  # noqa: E501


if __name__ == "__main__":
    asyncio.run(main())
```

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/03-workflows/human-in-the-loop/human_in_the_loop.py) for the complete runnable file.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [State Management](./state.md)

**Go deeper:**

- [Checkpoints & Resuming](./checkpoints.md) — persist and resume workflows
- [Agents in Workflows](./agents-in-workflows.md) — use agents as workflow steps
- [Tool Approval](../agents/tools/tool-approval.md) — human approval for tool calls
