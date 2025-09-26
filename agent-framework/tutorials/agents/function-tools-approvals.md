---
title: Using function tools with human in the loop approvals
description: Learn how to use function tools with human in the loop approvals
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/15/2025
ms.service: semantic-kernel
---

# Using function tools with human in the loop approvals

::: zone pivot="programming-language-csharp"

This tutorial step shows you how to use function tools that require human approval with an agent, where the agent is built on the Azure OpenAI Chat Completion service.

When agents require any user input, for example to approve a function call, this is referred to as a human-in-the-loop pattern.
An agent run that requires user input, will complete with a response that indicates what input is required from the user, instead of completing with a final answer.
The caller of the agent is then responsible for getting the required input from the user, and passing it back to the agent as part of a new agent run.

## Prerequisites

For prerequisites and installing nuget packages, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Creating the agent with function tools

When using functions, it's possible to indicate for each function, whether it requires human approval before being executed.
This is done by wrapping the `AIFunction` instance in an `ApprovalRequiredAIFunction` instance.

Here is an example of a simple function tool that fakes getting the weather for a given location.

```csharp
[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";
```

To create an `AIFunction` and then wrap it in an `ApprovalRequiredAIFunction`, you can do the following:

```csharp
AIFunction weatherFunction = AIFunctionFactory.Create(GetWeather);
AIFunction approvalRequiredWeatherFunction = new ApprovalRequiredAIFunction(weatherFunction);
```

When creating the agent, we can now provide the approval requiring function tool to the agent, by passing a list of tools to the `CreateAIAgent` method.

```csharp
AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
     .GetChatClient("gpt-4o-mini")
     .CreateAIAgent(instructions: "You are a helpful assistant", tools: [approvalRequiredWeatherFunction]);
```

Since we now have a function that requires approval, the agent may respond with a request for approval, instead of executing the function directly and returning the result.
We can check the response content for any `FunctionApprovalRequestContent` instances, which indicates that the agent requires user approval for a function.

```csharp
AgentThread thread = agent.GetNewThread();
AgentRunResponse response = await agent.RunAsync("What is the weather like in Amsterdam?", thread);

var functionApprovalRequests = response.Messages
    .SelectMany(x => x.Contents)
    .OfType<FunctionApprovalRequestContent>()
    .ToList();
```

If there are any function approval requests, the detail of the function call including name and arguments can be found in the `FunctionCall` property on the `FunctionApprovalRequestContent` instance.
This can be shown to the user, so that they can decide whether to approve or reject the function call.
For our example, we will assume there is one request.

```csharp
FunctionApprovalRequestContent requestContent = functionApprovalRequests.First();
Console.WriteLine($"We require approval to execute '{requestContent.FunctionCall.Name}'");
```

Once the user has provided their input, we can create a `FunctionApprovalResponseContent` instance using the `CreateResponse` method on the `FunctionApprovalRequestContent`.
Pass `true` to approve the function call, or `false` to reject it.

The response content can then be passed to the agent in a new `User` `ChatMessage`, along with the same thread object to get the result back from the agent.

```csharp
var approvalMessage = new ChatMessage(ChatRole.User, [requestContent.CreateResponse(true)]);
Console.WriteLine(await agent.RunAsync(approvalMessage, thread));
```

Whenever you are using function tools with human in the loop approvals, remember to check for `FunctionApprovalRequestContent` instances in the response, after each agent run, until all function calls have been approved or rejected.

::: zone-end
::: zone pivot="programming-language-python"

This tutorial step shows you how to use function tools that require human approval with an agent, where the agent is built on the Azure OpenAI Chat Completion service.

When agents require any user input, for example to approve a function call, this is referred to as a human-in-the-loop pattern.
An agent run that requires user input, will complete with a response that indicates what input is required from the user, instead of completing with a final answer.
The caller of the agent is then responsible for getting the required input from the user, and passing it back to the agent as part of a new agent run.

## Prerequisites

For prerequisites and installing Python packages, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Creating the agent with function tools

When using functions, it's possible to indicate for each function, whether it requires human approval before being executed.
This can be configured at the agent or service level depending on the underlying service capabilities.

Here is an example of a simple function tool that fakes getting the weather for a given location.

```python
from typing import Annotated
from pydantic import Field

def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
) -> str:
    """Get the weather for a given location."""
    return f"The weather in {location} is cloudy with a high of 15°C."
```

When creating the agent, we can provide the function tool to the agent:

```python
import asyncio
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity import AzureCliCredential

agent = AzureOpenAIChatClient(credential=AzureCliCredential()).create_agent(
    instructions="You are a helpful assistant",
    tools=get_weather
)
```

Since the underlying service may require approval for function calls, the agent may respond with a request for approval, instead of executing the function directly and returning the result.
We can check the response for any `user_input_requests`, which indicates that the agent requires user approval for a function.

```python
thread = agent.get_new_thread()
response = await agent.run("What is the weather like in Amsterdam?", thread=thread)

if response.user_input_requests:
    print(f"Approval required for {len(response.user_input_requests)} function(s)")
```

If there are any function approval requests, the detail of the function call including name and arguments can be found in the `function_call` property on the request.
This can be shown to the user, so that they can decide whether to approve or reject the function call.
For our example, we will assume there is one request.

```python
from agent_framework import ChatMessage, Role

approval_request = response.user_input_requests[0]
print(f"We require approval to execute '{approval_request.function_call.name}'")
print(f"Arguments: {approval_request.function_call.arguments}")
```

Once the user has provided their input, we can create a response using the `create_response` method on the request.
Pass `True` to approve the function call, or `False` to reject it.

The response content can then be passed to the agent in a new `User` `ChatMessage`, along with the same thread object to get the result back from the agent.

```python
async def main():
    # Assume user approved the function call
    user_approved = True  # This would come from user input in a real application
    
    approval_response = approval_request.create_response(user_approved)
    approval_message = ChatMessage(role=Role.USER, contents=[approval_response])
    
    final_response = await agent.run(approval_message, thread=thread)
    print(final_response.text)

asyncio.run(main())
```

Whenever you are using function tools with human in the loop approvals, remember to check for `user_input_requests` in the response, after each agent run, until all function calls have been approved or rejected.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Producing Structured Output with agents](./structured-output.md)
