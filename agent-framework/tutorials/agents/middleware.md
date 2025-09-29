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

Tutorial coming soon.

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

## Step 2: Create Your First Middleware

Create a simple logging middleware to see when your agent runs:

```python
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
        middleware=[logging_agent_middleware],  # Add your middleware here
    ) as agent:
        result = await agent.run("Hello!")
        print(result.text)
```

## Step 4: Create Function Middleware

If your agent uses functions, you can intercept function calls:

```python
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
