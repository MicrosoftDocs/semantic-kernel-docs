---
title: Using function tools with human in the loop approvals
description: Learn how to use function tools with human in the loop approvals
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/15/2025
ms.service: agent-framework
---

# Using function tools with human in the loop approvals

::: zone pivot="programming-language-csharp"

This tutorial step shows you how to use function tools that require human approval with an agent, where the agent is built on the Azure OpenAI Chat Completion service.

When agents require any user input, for example to approve a function call, this is referred to as a human-in-the-loop pattern.
An agent run that requires user input, will complete with a response that indicates what input is required from the user, instead of completing with a final answer.
The caller of the agent is then responsible for getting the required input from the user, and passing it back to the agent as part of a new agent run.

## Prerequisites

For prerequisites and installing NuGet packages, see the [Create and run a simple agent](../running-agents.md) step in this tutorial.

## Create the agent with function tools

When using functions, it's possible to indicate for each function, whether it requires human approval before being executed.
This is done by wrapping the `AIFunction` instance in an `ApprovalRequiredAIFunction` instance.

Here is an example of a simple function tool that fakes getting the weather for a given location.

```csharp
using System;
using System.ComponentModel;
using System.Linq;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";
```

To create an `AIFunction` and then wrap it in an `ApprovalRequiredAIFunction`, you can do the following:

```csharp
AIFunction weatherFunction = AIFunctionFactory.Create(GetWeather);
AIFunction approvalRequiredWeatherFunction = new ApprovalRequiredAIFunction(weatherFunction);
```

When creating the agent, you can now provide the approval requiring function tool to the agent, by passing a list of tools to the `AsAIAgent` method.

```csharp
AIAgent agent = new AIProjectClient(
    new Uri("<your-foundry-project-endpoint>"),
    new DefaultAzureCredential())
     .AsAIAgent(
        model: "gpt-4o-mini",
        instructions: "You are a helpful assistant",
        tools: [approvalRequiredWeatherFunction]);
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

Since you now have a function that requires approval, the agent might respond with a request for approval instead of executing the function directly and returning the result.
You can check the response content for any `FunctionApprovalRequestContent` instances, which indicates that the agent requires user approval for a function.

```csharp
AgentSession session = await agent.CreateSessionAsync();
AgentResponse response = await agent.RunAsync("What is the weather like in Amsterdam?", session);

var functionApprovalRequests = response.Messages
    .SelectMany(x => x.Contents)
    .OfType<FunctionApprovalRequestContent>()
    .ToList();
```

If there are any function approval requests, the detail of the function call including name and arguments can be found in the `FunctionCall` property on the `FunctionApprovalRequestContent` instance.
This can be shown to the user, so that they can decide whether to approve or reject the function call.
For this example, assume there is one request.

```csharp
FunctionApprovalRequestContent requestContent = functionApprovalRequests.First();
Console.WriteLine($"We require approval to execute '{requestContent.FunctionCall.Name}'");
```

Once the user has provided their input, you can create a `FunctionApprovalResponseContent` instance using the `CreateResponse` method on the `FunctionApprovalRequestContent`.
Pass `true` to approve the function call, or `false` to reject it.

The response content can then be passed to the agent in a new `User` `ChatMessage`, along with the same session object to get the result back from the agent.

```csharp
var approvalMessage = new ChatMessage(ChatRole.User, [requestContent.CreateResponse(true)]);
Console.WriteLine(await agent.RunAsync(approvalMessage, session));
```

Whenever you are using function tools with human in the loop approvals, remember to check for `FunctionApprovalRequestContent` instances in the response, after each agent run, until all function calls have been approved or rejected.

> [!TIP]
> See the [.NET Agents Step 01: Using Function Tools with Approvals](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/02-agents/Agents/Agent_Step01_UsingFunctionToolsWithApprovals) sample for a complete, runnable example.

::: zone-end
::: zone pivot="programming-language-python"

This tutorial step shows you how to use function tools that require human approval with an agent.

When agents require any user input, for example to approve a function call, this is referred to as a human-in-the-loop pattern.
An agent run that requires user input, will complete with a response that indicates what input is required from the user, instead of completing with a final answer.
The caller of the agent is then responsible for getting the required input from the user, and passing it back to the agent as part of a new agent run.

## Prerequisites

For prerequisites and installing Python packages, see the [Create and run a simple agent](../running-agents.md) step in this tutorial.

## Create the agent with function tools requiring approval

When using functions, it's possible to indicate for each function, whether it requires human approval before being executed.
This is done by setting the `approval_mode` parameter to `"always_require"` when using the `@tool` decorator.

Here is an example of a simple function tool that fakes getting the weather for a given location.

```python
from typing import Annotated
from agent_framework import tool

@tool
def get_weather(location: Annotated[str, "The city and state, e.g. San Francisco, CA"]) -> str:
    """Get the current weather for a given location."""
    return f"The weather in {location} is cloudy with a high of 15°C."
```

To create a function that requires approval, you can use the `approval_mode` parameter:

```python
@tool(approval_mode="always_require")
def get_weather_detail(location: Annotated[str, "The city and state, e.g. San Francisco, CA"]) -> str:
    """Get detailed weather information for a given location."""
    return f"The weather in {location} is cloudy with a high of 15°C, humidity 88%."
```

When creating the agent, you can now provide the approval requiring function tool to the agent, by passing a list of tools to the `Agent` constructor.

```python
from agent_framework import Agent
from agent_framework.openai import OpenAIChatClient

async with Agent(
    chat_client=OpenAIChatClient(),
    name="WeatherAgent",
    instructions="You are a helpful weather assistant.",
    tools=[get_weather, get_weather_detail],
) as agent:
    # Agent is ready to use
```

Since you now have a function that requires approval, the agent might respond with a request for approval instead of executing the function directly and returning the result.
You can check the response for any user input requests, which indicates that the agent requires user approval for a function.

```python
result = await agent.run("What is the detailed weather like in Amsterdam?")

if result.user_input_requests:
    for user_input_needed in result.user_input_requests:
        print(f"Function: {user_input_needed.function_call.name}")
        print(f"Arguments: {user_input_needed.function_call.arguments}")
```

If there are any function approval requests, the detail of the function call including name and arguments can be found in the `function_call` property on the user input request.
This can be shown to the user, so that they can decide whether to approve or reject the function call.

Once the user has provided their input, you can create a response using the `create_response` method on the user input request.
Pass `True` to approve the function call, or `False` to reject it.

The response can then be passed to the agent in a new `Message`, to get the result back from the agent.

```python
from agent_framework import Message

# Get user approval (in a real application, this would be interactive)
user_approval = True  # or False to reject

# Create the approval response
approval_message = Message(
    role="user",
    contents=[user_input_needed.create_response(user_approval)]
)

# Continue the conversation with the approval
final_result = await agent.run([
    "What is the detailed weather like in Amsterdam?",
    Message(role="assistant", contents=[user_input_needed]),
    approval_message
])
print(final_result.text)
```

## Handling approvals in a loop

When working with multiple function calls that require approval, you may need to handle approvals in a loop until all functions are approved or rejected:

```python
async def handle_approvals(query: str, agent) -> str:
    """Handle function call approvals in a loop."""
    current_input = query

    while True:
        result = await agent.run(current_input)

        if not result.user_input_requests:
            # No more approvals needed, return the final result
            return result.text

        # Build new input with all context
        new_inputs = [query]

        for user_input_needed in result.user_input_requests:
            print(f"Approval needed for: {user_input_needed.function_call.name}")
            print(f"Arguments: {user_input_needed.function_call.arguments}")

            # Add the assistant message with the approval request
            new_inputs.append(Message(role="assistant", contents=[user_input_needed]))

            # Get user approval (in practice, this would be interactive)
            user_approval = True  # Replace with actual user input

            # Add the user's approval response
            new_inputs.append(
                Message(role="user", contents=[user_input_needed.create_response(user_approval)])
            )

        # Continue with all the context
        current_input = new_inputs

# Usage
result_text = await handle_approvals("Get detailed weather for Seattle and Portland", agent)
print(result_text)
```

Whenever you are using function tools with human in the loop approvals, remember to check for user input requests in the response, after each agent run, until all function calls have been approved or rejected.

### Complete example

```python
# Copyright (c) Microsoft. All rights reserved.

import asyncio
from random import randrange
from typing import TYPE_CHECKING, Annotated, Any

from agent_framework import Agent, AgentResponse, Message, tool
from agent_framework.openai import OpenAIChatClient

if TYPE_CHECKING:
    from agent_framework import SupportsAgentRun

"""
Demonstration of a tool with approvals.

This sample demonstrates using AI functions with user approval workflows.
It shows how to handle function call approvals without using threads.
"""

conditions = ["sunny", "cloudy", "raining", "snowing", "clear"]


# NOTE: approval_mode="never_require" is for sample brevity. Use "always_require" in production; see samples/02-agents/tools/function_tool_with_approval.py and samples/02-agents/tools/function_tool_with_approval_and_sessions.py.
@tool(approval_mode="never_require")
def get_weather(location: Annotated[str, "The city and state, e.g. San Francisco, CA"]) -> str:
    """Get the current weather for a given location."""
    # Simulate weather data
    return f"The weather in {location} is {conditions[randrange(0, len(conditions))]} and {randrange(-10, 30)}°C."


# Define a simple weather tool that requires approval
@tool(approval_mode="always_require")
def get_weather_detail(location: Annotated[str, "The city and state, e.g. San Francisco, CA"]) -> str:
    """Get the current weather for a given location."""
    # Simulate weather data
    return (
        f"The weather in {location} is {conditions[randrange(0, len(conditions))]} and {randrange(-10, 30)}°C, "
        "with a humidity of 88%. "
        f"Tomorrow will be {conditions[randrange(0, len(conditions))]} with a high of {randrange(-10, 30)}°C."
    )


async def handle_approvals(query: str, agent: "SupportsAgentRun") -> AgentResponse:
    """Handle function call approvals.

    When we don't have a thread, we need to ensure we include the original query,
    the approval request, and the approval response in each iteration.
    """
    result = await agent.run(query)
    while len(result.user_input_requests) > 0:
        # Start with the original query
        new_inputs: list[Any] = [query]

        for user_input_needed in result.user_input_requests:
            print(
                f"\nUser Input Request for function from {agent.name}:"
                f"\n  Function: {user_input_needed.function_call.name}"
                f"\n  Arguments: {user_input_needed.function_call.arguments}"
            )

            # Add the assistant message with the approval request
            new_inputs.append(Message("assistant", [user_input_needed]))

            # Get user approval
            user_approval = await asyncio.to_thread(input, "\nApprove function call? (y/n): ")

            # Add the user's approval response
            new_inputs.append(
                Message("user", [user_input_needed.to_function_approval_response(user_approval.lower() == "y")])
            )

        # Run again with all the context
        result = await agent.run(new_inputs)

    return result


async def handle_approvals_streaming(query: str, agent: "SupportsAgentRun") -> None:
    """Handle function call approvals with streaming responses.

    When we don't have a thread, we need to ensure we include the original query,
    the approval request, and the approval response in each iteration.
    """
    current_input: str | list[Any] = query
    has_user_input_requests = True
    while has_user_input_requests:
        has_user_input_requests = False
        user_input_requests: list[Any] = []

        # Stream the response
        async for chunk in agent.run(current_input, stream=True):
            if chunk.text:
                print(chunk.text, end="", flush=True)

            # Collect user input requests from the stream
            if chunk.user_input_requests:
                user_input_requests.extend(chunk.user_input_requests)

        if user_input_requests:
            has_user_input_requests = True
            # Start with the original query
            new_inputs: list[Any] = [query]

            for user_input_needed in user_input_requests:
                print(
                    f"\n\nUser Input Request for function from {agent.name}:"
                    f"\n  Function: {user_input_needed.function_call.name}"
                    f"\n  Arguments: {user_input_needed.function_call.arguments}"
                )

                # Add the assistant message with the approval request
                new_inputs.append(Message("assistant", [user_input_needed]))

                # Get user approval
                user_approval = await asyncio.to_thread(input, "\nApprove function call? (y/n): ")

                # Add the user's approval response
                new_inputs.append(
                    Message("user", [user_input_needed.to_function_approval_response(user_approval.lower() == "y")])
                )

            # Update input with all the context for next iteration
            current_input = new_inputs


async def run_weather_agent_with_approval(stream: bool) -> None:
    """Example showing AI function with approval requirement."""
    print(f"\n=== Weather Agent with Approval Required ({'Streaming' if stream else 'Non-Streaming'}) ===\n")

    async with Agent(
        client=OpenAIChatClient(),
        name="WeatherAgent",
        instructions=("You are a helpful weather assistant. Use the get_weather tool to provide weather information."),
        tools=[get_weather, get_weather_detail],
    ) as agent:
        query = "Can you give me an update of the weather in LA and Portland and detailed weather for Seattle?"
        print(f"User: {query}")

        if stream:
            print(f"\n{agent.name}: ", end="", flush=True)
            await handle_approvals_streaming(query, agent)
            print()
        else:
            result = await handle_approvals(query, agent)
            print(f"\n{agent.name}: {result}\n")


async def main() -> None:
    print("=== Demonstration of a tool with approvals ===\n")

    await run_weather_agent_with_approval(stream=False)
    await run_weather_agent_with_approval(stream=True)


if __name__ == "__main__":
    asyncio.run(main())
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Code Interpreter](./code-interpreter.md)
