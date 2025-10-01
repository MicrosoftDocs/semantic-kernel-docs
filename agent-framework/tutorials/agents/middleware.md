---
title: Adding middleware to agents
description: How to add middleware to an agent
zone_pivot_groups: programming-languages
author: dmytrostruk
ms.topic: tutorial
ms.author: dmytrostruk
ms.date: 09/29/2025
ms.service: semantic-kernel
---

# Adding Middleware to Agents

Learn how to add middleware to your agents in a few simple steps. Middleware allows you to intercept and modify agent interactions for logging, security, and other cross-cutting concerns.

::: zone pivot="programming-language-csharp"

## Prerequisites

For prerequisites and installing nuget packages, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Step 1: Create a Simple Agent

First, let's create a basic agent with a function tool.

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

[Description("The current datetime offset.")]
static string GetDateTime()
    => DateTimeOffset.Now.ToString();

AIAgent baseAgent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o-mini")
        .CreateAIAgent(
            instructions: "You are an AI assistant that helps people find information.",
            tools: [AIFunctionFactory.Create(GetDateTime, name: nameof(GetDateTime))]);
```

## Step 2: Create Your Agent Run Middleware

Next, we'll create a function that will get invoked for each agent run.
It allows us to inspect the input and output from the agent.

Unless the intention is to use the middleware to stop executing the run, the function
should call `RunAsync` on the provided `innerAgent`.

This sample middleware just inspects the input and output from the agent run and
outputs the number of messages passed into and out of the agent.

```csharp
async Task<AgentRunResponse> CustomAgentRunMiddleware(
    IEnumerable<ChatMessage> messages,
    AgentThread? thread,
    AgentRunOptions? options,
    AIAgent innerAgent,
    CancellationToken cancellationToken)
{
    Console.WriteLine(messages.Count());
    var response = await innerAgent.RunAsync(messages, thread, options, cancellationToken).ConfigureAwait(false);
    Console.WriteLine(response.Messages.Count);
    return response;
}
```

## Step 3: Add Agent Run Middleware to Your Agent

To add this middleware function to the `baseAgent` we created in step 1,
we should use the builder pattern.
This creates a new agent that has the middleware applied.
The original `baseAgent` is not modified.

```csharp
var middlewareEnabledAgent = baseAgent
    .AsBuilder()
        .Use(CustomAgentRunMiddleware)
    .Build();
```

## Step 4: Create Function calling Middleware

> [!NOTE]
> Function calling middleware is currently only supported with an `AIAgent` that uses `Microsoft.Extensions.AI.FunctionInvokingChatClient`, e.g. `ChatClientAgent`.

We can also create middleware that gets called for each function tool that is invoked.
Here is an example of function calling middleware, that can inspect and/or modify the function being called, and the result from the function call.

Unless the intention is to use the middleware to not execute the function tool, the middleware
should call the provided `next` `Func`.

```csharp
async ValueTask<object?> CustomFunctionCallingMiddleware(
    AIAgent agent,
    FunctionInvocationContext context,
    Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
    CancellationToken cancellationToken)
{
    Console.WriteLine($"Function Name: {context!.Function.Name}");
    var result = await next(context, cancellationToken);
    Console.WriteLine($"Function Call Result: {result}");

    return result;
}
```

## Step 5: Add Function calling Middleware to Your Agent

Same as with adding agent run middleware, we can add function calling middleware as follows:

```csharp
var middlewareEnabledAgent = baseAgent
    .AsBuilder()
        .Use(CustomFunctionCallingMiddleware)
    .Build();
```

Now, when executing the agent with a query that invokes a function, the middleware should get invoked,
outputting the function name and call result.

```csharp
await middlewareEnabledAgent.RunAsync("What's the current time?");
```

## Step 6: Create Chat Client Middleware

For agents that are built using `IChatClient` developers may want to intercept calls going from the agent to the `IChatClient`.
In this case it is possible to use middleware for the `IChatClient`.

Here is an example of chat client middleware, that can inspect and/or modify the input and output for the request to the inference service that the chat client provides.

```csharp
async Task<ChatResponse> CustomChatClientMiddleware(
    IEnumerable<ChatMessage> messages,
    ChatOptions? options,
    IChatClient innerChatClient,
    CancellationToken cancellationToken)
{
    Console.WriteLine(messages.Count());
    var response = await innerChatClient.GetResponseAsync(messages, options, cancellationToken);
    Console.WriteLine(response.Messages.Count);

    return response;
}
```

> [!NOTE]
> For more information about `IChatClient` middleware, see [Custom IChatClient middleware](/dotnet/ai/microsoft-extensions-ai#custom-ichatclient-middleware)
> in the Microsoft.Extensions.AI documentation.

## Step 7: Add Chat client Middleware to an `IChatClient`

To add middleware to your `IChatClient`, you can use the builder pattern.
After adding the middleware, you can use the `IChatClient` with your agent as usual.

```csharp
var chatClient = new AzureOpenAIClient(new Uri("https://<myresource>.openai.azure.com"), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();

var middlewareEnabledChatClient = chatClient
    .AsBuilder()
        .Use(getResponseFunc: CustomChatClientMiddleware, getStreamingResponseFunc: null)
    .Build();

var agent = new ChatClientAgent(middlewareEnabledChatClient, instructions: "You are a helpful assistant.");
```

`IChatClient` middleware can also be registered using a factory method when constructing
 an agent via one of the helper methods on SDK clients.

```csharp
var agent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .CreateAIAgent("You are a helpful assistant.", clientFactory: (chatClient) => chatClient
        .AsBuilder()
            .Use(getResponseFunc: CustomChatClientMiddleware, getStreamingResponseFunc: null)
        .Build());
```

::: zone-end
::: zone pivot="programming-language-python"

## Step 1: Create a Simple Agent

First, let's create a basic agent:

```python
import asyncio
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def main():
    credential = AzureCliCredential()

    async with AzureAIAgentClient(async_credential=credential).create_agent(
        name="GreetingAgent",
        instructions="You are a friendly greeting assistant.",
    ) as agent:
        result = await agent.run("Hello!")
        print(result.text)

if __name__ == "__main__":
    asyncio.run(main())
```

## Step 2: Create Your Middleware

Create a simple logging middleware to see when your agent runs:

```python
from agent_framework import AgentRunContext

async def logging_agent_middleware(
    context: AgentRunContext,
    next: Callable[[AgentRunContext], Awaitable[None]],
) -> None:
    """Simple middleware that logs agent execution."""
    print("Agent starting...")

    # Continue to agent execution
    await next(context)

    print("Agent finished!")
```

## Step 3: Add Middleware to Your Agent

Add the middleware when creating your agent:

```python
async def main():
    credential = AzureCliCredential()

    async with AzureAIAgentClient(async_credential=credential).create_agent(
        name="GreetingAgent",
        instructions="You are a friendly greeting assistant.",
        middleware=logging_agent_middleware,  # Add your middleware here
    ) as agent:
        result = await agent.run("Hello!")
        print(result.text)
```

## Step 4: Create Function Middleware

If your agent uses functions, you can intercept function calls:

```python
from agent_framework import FunctionInvocationContext

def get_time():
    """Get the current time."""
    from datetime import datetime
    return datetime.now().strftime("%H:%M:%S")

async def logging_function_middleware(
    context: FunctionInvocationContext,
    next: Callable[[FunctionInvocationContext], Awaitable[None]],
) -> None:
    """Middleware that logs function calls."""
    print(f"Calling function: {context.function.name}")

    await next(context)

    print(f"Function result: {context.result}")

# Add both the function and middleware to your agent
async with AzureAIAgentClient(async_credential=credential).create_agent(
    name="TimeAgent",
    instructions="You can tell the current time.",
    tools=[get_time],
    middleware=[logging_function_middleware],
) as agent:
    result = await agent.run("What time is it?")
```

## Step 5: Use Run-Level Middleware

You can also add middleware for specific runs:

```python
# Use middleware for this specific run only
result = await agent.run(
    "This is important!",
    middleware=[logging_function_middleware]
)
```

## What's Next?

For more advanced scenarios, check out the [Agent Middleware User Guide](../../user-guide/agents/agent-middleware.md) which covers:

- Different types of middleware (agent, function, chat)
- Class-based middleware for complex scenarios
- Middleware termination and result overrides
- Advanced middleware patterns and best practices

::: zone-end
