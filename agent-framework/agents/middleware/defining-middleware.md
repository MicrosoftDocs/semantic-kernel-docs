---
title: Adding middleware to agents
description: How to add middleware to an agent
zone_pivot_groups: programming-languages
author: dmytrostruk
ms.topic: tutorial
ms.author: dmytrostruk
ms.date: 03/16/2026
ms.service: agent-framework
---

# Adding Middleware to Agents

Learn how to add middleware to your agents in a few simple steps. Middleware allows you to intercept and modify agent interactions for logging, security, and other cross-cutting concerns.

::: zone pivot="programming-language-csharp"

## Prerequisites

For prerequisites and installing NuGet packages, see the [Create and run a simple agent](../running-agents.md) step in this tutorial.

## Step 1: Create a Simple Agent

First, create a basic agent with a function tool.

```csharp
using System;
using System.ComponentModel;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

[Description("The current datetime offset.")]
static string GetDateTime()
    => DateTimeOffset.Now.ToString();

AIAgent baseAgent = new AIProjectClient(
    new Uri("<your-foundry-project-endpoint>"),
    new DefaultAzureCredential())
        .AsAIAgent(
            model: "gpt-4o-mini",
            instructions: "You are an AI assistant that helps people find information.",
            tools: [AIFunctionFactory.Create(GetDateTime, name: nameof(GetDateTime))]);
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

## Step 2: Create Your Agent Run Middleware

Next, create a function that will get invoked for each agent run.
It allows you to inspect the input and output from the agent.

Unless the intention is to use the middleware to stop executing the run, the function
should call `RunAsync` on the provided `innerAgent`.

This sample middleware just inspects the input and output from the agent run and
outputs the number of messages passed into and out of the agent.

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

async Task<AgentResponse> CustomAgentRunMiddleware(
    IEnumerable<ChatMessage> messages,
    AgentSession? session,
    AgentRunOptions? options,
    AIAgent innerAgent,
    CancellationToken cancellationToken)
{
    Console.WriteLine($"Input: {messages.Count()}");
    var response = await innerAgent.RunAsync(messages, session, options, cancellationToken).ConfigureAwait(false);
    Console.WriteLine($"Output: {response.Messages.Count}");
    return response;
}
```

## Step 3: Add Agent Run Middleware to Your Agent

To add this middleware function to the `baseAgent` you created in step 1, use the builder pattern.
This creates a new agent that has the middleware applied.
The original `baseAgent` is not modified.

```csharp
var middlewareEnabledAgent = baseAgent
    .AsBuilder()
        .Use(runFunc: CustomAgentRunMiddleware, runStreamingFunc: null)
    .Build();
```

Now, when executing the agent with a query, the middleware should get invoked,
outputting the number of input messages and the number of response messages.

```csharp
Console.WriteLine(await middlewareEnabledAgent.RunAsync("What's the current time?"));
```

## Step 4: Create Function calling Middleware

> [!NOTE]
> Function calling middleware is currently only supported with an `AIAgent` that uses <xref:Microsoft.Extensions.AI.FunctionInvokingChatClient>, for example, `ChatClientAgent`.

You can also create middleware that gets called for each function tool that's invoked.
Here's an example of function-calling middleware that can inspect and/or modify the function being called and the result from the function call.

Unless the intention is to use the middleware to not execute the function tool, the middleware should call the provided `next` `Func`.

```csharp
using System.Threading;
using System.Threading.Tasks;

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

Same as with adding agent-run middleware, you can add function calling middleware as follows:

```csharp
var middlewareEnabledAgent = baseAgent
    .AsBuilder()
        .Use(CustomFunctionCallingMiddleware)
    .Build();
```

Now, when executing the agent with a query that invokes a function, the middleware should get invoked,
outputting the function name and call result.

```csharp
Console.WriteLine(await middlewareEnabledAgent.RunAsync("What's the current time?"));
```

## Step 6: Create Chat Client Middleware

For agents that are built using <xref:Microsoft.Extensions.AI.IChatClient>, you might want to intercept calls going from the agent to the `IChatClient`.
In this case, it's possible to use middleware for the `IChatClient`.

Here is an example of chat client middleware that can inspect and/or modify the input and output for the request to the inference service that the chat client provides.

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

async Task<ChatResponse> CustomChatClientMiddleware(
    IEnumerable<ChatMessage> messages,
    ChatOptions? options,
    IChatClient innerChatClient,
    CancellationToken cancellationToken)
{
    Console.WriteLine($"Input: {messages.Count()}");
    var response = await innerChatClient.GetResponseAsync(messages, options, cancellationToken);
    Console.WriteLine($"Output: {response.Messages.Count}");

    return response;
}
```

> [!NOTE]
> For more information about `IChatClient` middleware, see [Custom IChatClient middleware](/dotnet/ai/microsoft-extensions-ai#custom-ichatclient-middleware).

## Step 7: Add Chat client Middleware to an `IChatClient`

To add middleware to your <xref:Microsoft.Extensions.AI.IChatClient>, you can use the builder pattern.
After adding the middleware, you can use the `IChatClient` with your agent as usual.

```csharp
var chatClient = new AIProjectClient(
    new Uri("<your-foundry-project-endpoint>"),
    new DefaultAzureCredential())
        .GetProjectOpenAIClient()
        .GetResponsesClient()
        .AsIChatClient("gpt-4o-mini");

var middlewareEnabledChatClient = chatClient
    .AsBuilder()
        .Use(getResponseFunc: CustomChatClientMiddleware, getStreamingResponseFunc: null)
    .Build();

var agent = new ChatClientAgent(middlewareEnabledChatClient, instructions: "You are a helpful assistant.");
```

`IChatClient` middleware can also be registered using a factory method when constructing
 an agent via one of the helper methods on SDK clients.

```csharp
var agent = new AIProjectClient(
    new Uri("<your-foundry-project-endpoint>"),
    new DefaultAzureCredential())
        .AsAIAgent(
            model: "gpt-4o-mini",
            instructions: "You are a helpful assistant.",
            clientFactory: (chatClient) => chatClient
                .AsBuilder()
                    .Use(getResponseFunc: CustomChatClientMiddleware, getStreamingResponseFunc: null)
                .Build());
```

::: zone-end
::: zone pivot="programming-language-python"

## Step 1: Create a Simple Agent

First, create a basic agent:

```python
import asyncio
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

async def main():
    credential = AzureCliCredential()

    async with AzureAIAgentClient(credential=credential).as_agent(
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
from collections.abc import Awaitable, Callable

from agent_framework import AgentContext

async def logging_agent_middleware(
    context: AgentContext,
    call_next: Callable[[], Awaitable[None]],
) -> None:
    """Simple middleware that logs agent execution."""
    print("Agent starting...")

    # Continue to agent execution
    await call_next()

    print("Agent finished!")
```

## Step 3: Add Middleware to Your Agent

Add the middleware when creating your agent:

```python
async def main():
    credential = AzureCliCredential()

    async with AzureAIAgentClient(credential=credential).as_agent(
        name="GreetingAgent",
        instructions="You are a friendly greeting assistant.",
        middleware=[logging_agent_middleware],  # Add your middleware here
    ) as agent:
        result = await agent.run("Hello!")
        print(result.text)
```

## Step 4: Create Function Middleware

If your agent uses functions, you can intercept function calls and set tool-only runtime values before the tool executes:

```python
from collections.abc import Awaitable, Callable

from agent_framework import FunctionInvocationContext

def get_time(ctx: FunctionInvocationContext) -> str:
    """Get the current time."""
    from datetime import datetime
    source = ctx.kwargs.get("request_source", "direct")
    return f"[{source}] {datetime.now().strftime('%H:%M:%S')}"

async def inject_function_kwargs(
    context: FunctionInvocationContext,
    call_next: Callable[[], Awaitable[None]],
) -> None:
    """Middleware that adds tool-only runtime values before execution."""
    context.kwargs.setdefault("request_source", "middleware")

    await call_next()

# Add both the function and middleware to your agent
async with AzureAIAgentClient(credential=credential).as_agent(
    name="TimeAgent",
    instructions="You can tell the current time.",
    tools=[get_time],
    middleware=[inject_function_kwargs],
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

For more advanced scenarios, see the [Agent Middleware User Guide](../../agents/middleware/index.md), which covers:

- Different types of middleware (agent, function, chat).
- Class-based middleware for complex scenarios.
- Middleware termination and result overrides.
- Advanced middleware patterns and best practices.

### Complete examples

#### Class-based middleware

```python
# Copyright (c) Microsoft. All rights reserved.

import asyncio
import time
from collections.abc import Awaitable, Callable
from random import randint
from typing import Annotated

from agent_framework import (
    AgentContext,
    AgentMiddleware,
    AgentResponse,
    FunctionInvocationContext,
    FunctionMiddleware,
    Message,
    tool,
)
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential
from pydantic import Field

"""
Class-based MiddlewareTypes Example

This sample demonstrates how to implement middleware using class-based approach by inheriting
from AgentMiddleware and FunctionMiddleware base classes. The example includes:

- SecurityAgentMiddleware: Checks for security violations in user queries and blocks requests
  containing sensitive information like passwords or secrets
- LoggingFunctionMiddleware: Logs function execution details including timing and parameters

This approach is useful when you need stateful middleware or complex logic that benefits
from object-oriented design patterns.
"""


# NOTE: approval_mode="never_require" is for sample brevity. Use "always_require" in production; see samples/02-agents/tools/function_tool_with_approval.py and samples/02-agents/tools/function_tool_with_approval_and_sessions.py.
@tool(approval_mode="never_require")
def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
) -> str:
    """Get the weather for a given location."""
    conditions = ["sunny", "cloudy", "rainy", "stormy"]
    return f"The weather in {location} is {conditions[randint(0, 3)]} with a high of {randint(10, 30)}°C."


class SecurityAgentMiddleware(AgentMiddleware):
    """Agent middleware that checks for security violations."""

    async def process(
        self,
        context: AgentContext,
        call_next: Callable[[], Awaitable[None]],
    ) -> None:
        # Check for potential security violations in the query
        # Look at the last user message
        last_message = context.messages[-1] if context.messages else None
        if last_message and last_message.text:
            query = last_message.text
            if "password" in query.lower() or "secret" in query.lower():
                print("[SecurityAgentMiddleware] Security Warning: Detected sensitive information, blocking request.")
                # Override the result with warning message
                context.result = AgentResponse(
                    messages=[Message("assistant", ["Detected sensitive information, the request is blocked."])]
                )
                # Simply don't call call_next() to prevent execution
                return

        print("[SecurityAgentMiddleware] Security check passed.")
        await call_next()


class LoggingFunctionMiddleware(FunctionMiddleware):
    """Function middleware that logs function calls."""

    async def process(
        self,
        context: FunctionInvocationContext,
        call_next: Callable[[], Awaitable[None]],
    ) -> None:
        function_name = context.function.name
        print(f"[LoggingFunctionMiddleware] About to call function: {function_name}.")

        start_time = time.time()

        await call_next()

        end_time = time.time()
        duration = end_time - start_time

        print(f"[LoggingFunctionMiddleware] Function {function_name} completed in {duration:.5f}s.")


async def main() -> None:
    """Example demonstrating class-based middleware."""
    print("=== Class-based MiddlewareTypes Example ===")

    # For authentication, run `az login` command in terminal or replace AzureCliCredential with preferred
    # authentication option.
    async with (
        AzureCliCredential() as credential,
        AzureAIAgentClient(credential=credential).as_agent(
            name="WeatherAgent",
            instructions="You are a helpful weather assistant.",
            tools=get_weather,
            middleware=[SecurityAgentMiddleware(), LoggingFunctionMiddleware()],
        ) as agent,
    ):
        # Test with normal query
        print("\n--- Normal Query ---")
        query = "What's the weather like in Seattle?"
        print(f"User: {query}")
        result = await agent.run(query)
        print(f"Agent: {result.text}\n")

        # Test with security-related query
        print("--- Security Test ---")
        query = "What's the password for the weather service?"
        print(f"User: {query}")
        result = await agent.run(query)
        print(f"Agent: {result.text}\n")


if __name__ == "__main__":
    asyncio.run(main())
```

#### Function-based middleware

```python
# Copyright (c) Microsoft. All rights reserved.

import asyncio
import time
from collections.abc import Awaitable, Callable
from random import randint
from typing import Annotated

from agent_framework import (
    AgentContext,
    AgentMiddleware,
    AgentResponse,
    FunctionInvocationContext,
    FunctionMiddleware,
    Message,
    tool,
)
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential
from pydantic import Field

"""
Class-based MiddlewareTypes Example

This sample demonstrates how to implement middleware using class-based approach by inheriting
from AgentMiddleware and FunctionMiddleware base classes. The example includes:

- SecurityAgentMiddleware: Checks for security violations in user queries and blocks requests
  containing sensitive information like passwords or secrets
- LoggingFunctionMiddleware: Logs function execution details including timing and parameters

This approach is useful when you need stateful middleware or complex logic that benefits
from object-oriented design patterns.
"""


# NOTE: approval_mode="never_require" is for sample brevity. Use "always_require" in production; see samples/02-agents/tools/function_tool_with_approval.py and samples/02-agents/tools/function_tool_with_approval_and_sessions.py.
@tool(approval_mode="never_require")
def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
) -> str:
    """Get the weather for a given location."""
    conditions = ["sunny", "cloudy", "rainy", "stormy"]
    return f"The weather in {location} is {conditions[randint(0, 3)]} with a high of {randint(10, 30)}°C."


class SecurityAgentMiddleware(AgentMiddleware):
    """Agent middleware that checks for security violations."""

    async def process(
        self,
        context: AgentContext,
        call_next: Callable[[], Awaitable[None]],
    ) -> None:
        # Check for potential security violations in the query
        # Look at the last user message
        last_message = context.messages[-1] if context.messages else None
        if last_message and last_message.text:
            query = last_message.text
            if "password" in query.lower() or "secret" in query.lower():
                print("[SecurityAgentMiddleware] Security Warning: Detected sensitive information, blocking request.")
                # Override the result with warning message
                context.result = AgentResponse(
                    messages=[Message("assistant", ["Detected sensitive information, the request is blocked."])]
                )
                # Simply don't call call_next() to prevent execution
                return

        print("[SecurityAgentMiddleware] Security check passed.")
        await call_next()


class LoggingFunctionMiddleware(FunctionMiddleware):
    """Function middleware that logs function calls."""

    async def process(
        self,
        context: FunctionInvocationContext,
        call_next: Callable[[], Awaitable[None]],
    ) -> None:
        function_name = context.function.name
        print(f"[LoggingFunctionMiddleware] About to call function: {function_name}.")

        start_time = time.time()

        await call_next()

        end_time = time.time()
        duration = end_time - start_time

        print(f"[LoggingFunctionMiddleware] Function {function_name} completed in {duration:.5f}s.")


async def main() -> None:
    """Example demonstrating class-based middleware."""
    print("=== Class-based MiddlewareTypes Example ===")

    # For authentication, run `az login` command in terminal or replace AzureCliCredential with preferred
    # authentication option.
    async with (
        AzureCliCredential() as credential,
        AzureAIAgentClient(credential=credential).as_agent(
            name="WeatherAgent",
            instructions="You are a helpful weather assistant.",
            tools=get_weather,
            middleware=[SecurityAgentMiddleware(), LoggingFunctionMiddleware()],
        ) as agent,
    ):
        # Test with normal query
        print("\n--- Normal Query ---")
        query = "What's the weather like in Seattle?"
        print(f"User: {query}")
        result = await agent.run(query)
        print(f"Agent: {result.text}\n")

        # Test with security-related query
        print("--- Security Test ---")
        query = "What's the password for the weather service?"
        print(f"User: {query}")
        result = await agent.run(query)
        print(f"Agent: {result.text}\n")


if __name__ == "__main__":
    asyncio.run(main())
```

#### Decorator-based middleware

```python
# Copyright (c) Microsoft. All rights reserved.

import asyncio
import time
from collections.abc import Awaitable, Callable
from random import randint
from typing import Annotated

from agent_framework import (
    AgentContext,
    AgentMiddleware,
    AgentResponse,
    FunctionInvocationContext,
    FunctionMiddleware,
    Message,
    tool,
)
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential
from pydantic import Field

"""
Class-based MiddlewareTypes Example

This sample demonstrates how to implement middleware using class-based approach by inheriting
from AgentMiddleware and FunctionMiddleware base classes. The example includes:

- SecurityAgentMiddleware: Checks for security violations in user queries and blocks requests
  containing sensitive information like passwords or secrets
- LoggingFunctionMiddleware: Logs function execution details including timing and parameters

This approach is useful when you need stateful middleware or complex logic that benefits
from object-oriented design patterns.
"""


# NOTE: approval_mode="never_require" is for sample brevity. Use "always_require" in production; see samples/02-agents/tools/function_tool_with_approval.py and samples/02-agents/tools/function_tool_with_approval_and_sessions.py.
@tool(approval_mode="never_require")
def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
) -> str:
    """Get the weather for a given location."""
    conditions = ["sunny", "cloudy", "rainy", "stormy"]
    return f"The weather in {location} is {conditions[randint(0, 3)]} with a high of {randint(10, 30)}°C."


class SecurityAgentMiddleware(AgentMiddleware):
    """Agent middleware that checks for security violations."""

    async def process(
        self,
        context: AgentContext,
        call_next: Callable[[], Awaitable[None]],
    ) -> None:
        # Check for potential security violations in the query
        # Look at the last user message
        last_message = context.messages[-1] if context.messages else None
        if last_message and last_message.text:
            query = last_message.text
            if "password" in query.lower() or "secret" in query.lower():
                print("[SecurityAgentMiddleware] Security Warning: Detected sensitive information, blocking request.")
                # Override the result with warning message
                context.result = AgentResponse(
                    messages=[Message("assistant", ["Detected sensitive information, the request is blocked."])]
                )
                # Simply don't call call_next() to prevent execution
                return

        print("[SecurityAgentMiddleware] Security check passed.")
        await call_next()


class LoggingFunctionMiddleware(FunctionMiddleware):
    """Function middleware that logs function calls."""

    async def process(
        self,
        context: FunctionInvocationContext,
        call_next: Callable[[], Awaitable[None]],
    ) -> None:
        function_name = context.function.name
        print(f"[LoggingFunctionMiddleware] About to call function: {function_name}.")

        start_time = time.time()

        await call_next()

        end_time = time.time()
        duration = end_time - start_time

        print(f"[LoggingFunctionMiddleware] Function {function_name} completed in {duration:.5f}s.")


async def main() -> None:
    """Example demonstrating class-based middleware."""
    print("=== Class-based MiddlewareTypes Example ===")

    # For authentication, run `az login` command in terminal or replace AzureCliCredential with preferred
    # authentication option.
    async with (
        AzureCliCredential() as credential,
        AzureAIAgentClient(credential=credential).as_agent(
            name="WeatherAgent",
            instructions="You are a helpful weather assistant.",
            tools=get_weather,
            middleware=[SecurityAgentMiddleware(), LoggingFunctionMiddleware()],
        ) as agent,
    ):
        # Test with normal query
        print("\n--- Normal Query ---")
        query = "What's the weather like in Seattle?"
        print(f"User: {query}")
        result = await agent.run(query)
        print(f"Agent: {result.text}\n")

        # Test with security-related query
        print("--- Security Test ---")
        query = "What's the password for the weather service?"
        print(f"User: {query}")
        result = await agent.run(query)
        print(f"Agent: {result.text}\n")


if __name__ == "__main__":
    asyncio.run(main())
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Chat-Level Middleware](chat-middleware.md)
