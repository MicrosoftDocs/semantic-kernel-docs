---
title: Agent Middleware
description: Learn how to create middleware with Agent Framework
zone_pivot_groups: programming-languages
author: dmytrostruk
ms.topic: reference
ms.author: dmytrostruk
ms.date: 09/29/2025
ms.service: agent-framework
---

# Agent Middleware

Middleware in the Agent Framework provides a powerful way to intercept, modify, and enhance agent interactions at various stages of execution. You can use middleware to implement cross-cutting concerns such as logging, security validation, error handling, and result transformation without modifying your core agent or function logic.

::: zone pivot="programming-language-csharp"

The Agent Framework can be customized using three different types of middleware:

1. Agent Run middleware: Allows interception of all agent runs, so that input and output can be inspected and/or modified as needed.
1. Function calling middleware: Allows interception of all function calls executed by the agent, so that input and output can be inspected and modified as needed.
1. `IChatClient` middleware: Allows interception of calls to an `IChatClient` implementation, where an agent is using `IChatClient` for inference calls, e.g. when using `ChatClientAgent`.

All the types of middleware are implemented via a function callback, and when multiple middleware instances of the same type are registered, they form a chain,
where each middleware instance is expected to call the next in the chain, via a provided `next` `Func`.

Agent run and function calling middleware types can be registered on an agent, by using the agent builder with an existing agent object.

```csharp
var middlewareEnabledAgent = originalAgent
    .AsBuilder()
        .Use(CustomAgentRunMiddleware)
        .Use(CustomFunctionCallingMiddleware)
    .Build();
```

`IChatClient` middleware can be registered on an `IChatClient` before it is used with a `ChatClientAgent`, by using the chat client builder pattern.

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

## Agent Run Middleware

Here is an example of agent run middleware, that can inspect and/or modify the input and output from the agent run.

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

## Function calling middleware

> [!NOTE]
> Function calling middleware is currently only supported with an `AIAgent` that uses `Microsoft.Extensions.AI.FunctionInvokingChatClient`, e.g. `ChatClientAgent`.

Here is an example of function calling middleware, that can inspect and/or modify the function being called, and the result from the function call.

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

It is possible to terminate the function call loop with function calling middleware by setting the provided `FunctionInvocationContext.Terminate` to true.
This will prevent the function calling loop from issuing a request to the inference service containing the function call results after function invocation.
If there were more than one function available for invocation during this iteration, it may also prevent any remaining functions from being executed.

> [!WARNING]
> Terminating the function call loop may result in your thread being left in an inconsistent state, e.g. containing function call content with no function result content.
> This may result in the thread being unusable for further runs.

## IChatClient middleware

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

::: zone-end
::: zone pivot="programming-language-python"

## Function-Based Middleware

Function-based middleware is the simplest way to implement middleware using async functions. This approach is ideal for stateless operations and provides a lightweight solution for common middleware scenarios.

### Agent Middleware

Agent middleware intercepts and modifies agent run execution. It uses the `AgentRunContext` which contains:

- `agent`: The agent being invoked
- `messages`: List of chat messages in the conversation
- `is_streaming`: Boolean indicating if the response is streaming
- `metadata`: Dictionary for storing additional data between middleware
- `result`: The agent's response (can be modified)
- `terminate`: Flag to stop further processing
- `kwargs`: Additional keyword arguments passed to the agent run method

The `next` callable continues the middleware chain or executes the agent if it's the last middleware.

Here's a simple logging example with logic before and after `next` callable:

```python
async def logging_agent_middleware(
    context: AgentRunContext,
    next: Callable[[AgentRunContext], Awaitable[None]],
) -> None:
    """Agent middleware that logs execution timing."""
    # Pre-processing: Log before agent execution
    print("[Agent] Starting execution")
    
    # Continue to next middleware or agent execution
    await next(context)
    
    # Post-processing: Log after agent execution
    print("[Agent] Execution completed")
```

### Function Middleware

Function middleware intercepts function calls within agents. It uses the `FunctionInvocationContext` which contains:

- `function`: The function being invoked
- `arguments`: The validated arguments for the function
- `metadata`: Dictionary for storing additional data between middleware
- `result`: The function's return value (can be modified)
- `terminate`: Flag to stop further processing
- `kwargs`: Additional keyword arguments passed to the chat method that invoked this function

The `next` callable continues to the next middleware or executes the actual function.

Here's a simple logging example with logic before and after `next` callable:

```python
async def logging_function_middleware(
    context: FunctionInvocationContext,
    next: Callable[[FunctionInvocationContext], Awaitable[None]],
) -> None:
    """Function middleware that logs function execution."""
    # Pre-processing: Log before function execution
    print(f"[Function] Calling {context.function.name}")
    
    # Continue to next middleware or function execution
    await next(context)
    
    # Post-processing: Log after function execution
    print(f"[Function] {context.function.name} completed")
```

### Chat Middleware

Chat middleware intercepts chat requests sent to AI models. It uses the `ChatContext` which contains:

- `chat_client`: The chat client being invoked
- `messages`: List of messages being sent to the AI service
- `chat_options`: The options for the chat request
- `is_streaming`: Boolean indicating if this is a streaming invocation
- `metadata`: Dictionary for storing additional data between middleware
- `result`: The chat response from the AI (can be modified)
- `terminate`: Flag to stop further processing
- `kwargs`: Additional keyword arguments passed to the chat client

The `next` callable continues to the next middleware or sends the request to the AI service.

Here's a simple logging example with logic before and after `next` callable:

```python
async def logging_chat_middleware(
    context: ChatContext,
    next: Callable[[ChatContext], Awaitable[None]],
) -> None:
    """Chat middleware that logs AI interactions."""
    # Pre-processing: Log before AI call
    print(f"[Chat] Sending {len(context.messages)} messages to AI")
    
    # Continue to next middleware or AI service
    await next(context)
    
    # Post-processing: Log after AI response
    print("[Chat] AI response received")
```

### Function Middleware Decorators

Decorators provide explicit middleware type declaration without requiring type annotations. They're helpful when:

- You don't use type annotations
- You need explicit middleware type declaration
- You want to prevent type mismatches

```python
from agent_framework import agent_middleware, function_middleware, chat_middleware

@agent_middleware  # Explicitly marks as agent middleware
async def simple_agent_middleware(context, next):
    """Agent middleware with decorator - types are inferred."""
    print("Before agent execution")
    await next(context)
    print("After agent execution")

@function_middleware  # Explicitly marks as function middleware
async def simple_function_middleware(context, next):
    """Function middleware with decorator - types are inferred."""
    print(f"Calling function: {context.function.name}")
    await next(context)
    print("Function call completed")

@chat_middleware  # Explicitly marks as chat middleware
async def simple_chat_middleware(context, next):
    """Chat middleware with decorator - types are inferred."""
    print(f"Processing {len(context.messages)} chat messages")
    await next(context)
    print("Chat processing completed")
```

## Class-Based Middleware

Class-based middleware is useful for stateful operations or complex logic that benefits from object-oriented design patterns.

### Agent Middleware Class

Class-based agent middleware uses a `process` method that has the same signature and behavior as function-based middleware. The `process` method receives the same `context` and `next` parameters and is invoked in exactly the same way.

```python
from agent_framework import AgentMiddleware, AgentRunContext

class LoggingAgentMiddleware(AgentMiddleware):
    """Agent middleware that logs execution."""
    
    async def process(
        self,
        context: AgentRunContext,
        next: Callable[[AgentRunContext], Awaitable[None]],
    ) -> None:
        # Pre-processing: Log before agent execution
        print("[Agent Class] Starting execution")
        
        # Continue to next middleware or agent execution
        await next(context)
        
        # Post-processing: Log after agent execution
        print("[Agent Class] Execution completed")
```

### Function Middleware Class

Class-based function middleware also uses a `process` method with the same signature and behavior as function-based middleware. The method receives the same `context` and `next` parameters.

```python
from agent_framework import FunctionMiddleware, FunctionInvocationContext

class LoggingFunctionMiddleware(FunctionMiddleware):
    """Function middleware that logs function execution."""
    
    async def process(
        self,
        context: FunctionInvocationContext,
        next: Callable[[FunctionInvocationContext], Awaitable[None]],
    ) -> None:
        # Pre-processing: Log before function execution
        print(f"[Function Class] Calling {context.function.name}")
        
        # Continue to next middleware or function execution
        await next(context)
        
        # Post-processing: Log after function execution
        print(f"[Function Class] {context.function.name} completed")
```

### Chat Middleware Class

Class-based chat middleware follows the same pattern with a `process` method that has identical signature and behavior to function-based chat middleware.

```python
from agent_framework import ChatMiddleware, ChatContext

class LoggingChatMiddleware(ChatMiddleware):
    """Chat middleware that logs AI interactions."""
    
    async def process(
        self,
        context: ChatContext,
        next: Callable[[ChatContext], Awaitable[None]],
    ) -> None:
        # Pre-processing: Log before AI call
        print(f"[Chat Class] Sending {len(context.messages)} messages to AI")
        
        # Continue to next middleware or AI service
        await next(context)
        
        # Post-processing: Log after AI response
        print("[Chat Class] AI response received")
```

## Middleware Registration

Middleware can be registered at two levels with different scopes and behaviors.

### Agent-Level vs Run-Level Middleware

```python
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential

# Agent-level middleware: Applied to ALL runs of the agent
async with AzureAIAgentClient(async_credential=credential).create_agent(
    name="WeatherAgent",
    instructions="You are a helpful weather assistant.",
    tools=get_weather,
    middleware=[
        SecurityAgentMiddleware(),  # Applies to all runs
        TimingFunctionMiddleware(),  # Applies to all runs
    ],
) as agent:
    
    # This run uses agent-level middleware only
    result1 = await agent.run("What's the weather in Seattle?")
    
    # This run uses agent-level + run-level middleware
    result2 = await agent.run(
        "What's the weather in Portland?",
        middleware=[  # Run-level middleware (this run only)
            logging_chat_middleware,
        ]
    )
    
    # This run uses agent-level middleware only (no run-level)
    result3 = await agent.run("What's the weather in Vancouver?")
```

**Key Differences:**
- **Agent-level**: Persistent across all runs, configured once when creating the agent
- **Run-level**: Applied only to specific runs, allows per-request customization
- **Execution Order**: Agent middleware (outermost) → Run middleware (innermost) → Agent execution

## Middleware Termination

Middleware can terminate execution early using `context.terminate`. This is useful for security checks, rate limiting, or validation failures.

```python
async def blocking_middleware(
    context: AgentRunContext,
    next: Callable[[AgentRunContext], Awaitable[None]],
) -> None:
    """Middleware that blocks execution based on conditions."""
    # Check for blocked content
    last_message = context.messages[-1] if context.messages else None
    if last_message and last_message.text:
        if "blocked" in last_message.text.lower():
            print("Request blocked by middleware")
            context.terminate = True
            return
    
    # If no issues, continue normally
    await next(context)
```

**What termination means:**
- Setting `context.terminate = True` signals that processing should stop
- You can provide a custom result before terminating to give users feedback
- The agent execution is completely skipped when middleware terminates

## Middleware Result Override

Middleware can override results in both non-streaming and streaming scenarios, allowing you to modify or completely replace agent responses.

The result type in `context.result` depends on whether the agent invocation is streaming or non-streaming:
- **Non-streaming**: `context.result` contains an `AgentRunResponse` with the complete response
- **Streaming**: `context.result` contains an async generator that yields `AgentRunResponseUpdate` chunks

You can use `context.is_streaming` to differentiate between these scenarios and handle result overrides appropriately.

```python
async def weather_override_middleware(
    context: AgentRunContext, 
    next: Callable[[AgentRunContext], Awaitable[None]]
) -> None:
    """Middleware that overrides weather results for both streaming and non-streaming."""
    
    # Execute the original agent logic
    await next(context)
    
    # Override results if present
    if context.result is not None:
        custom_message_parts = [
            "Weather Override: ",
            "Perfect weather everywhere today! ",
            "22°C with gentle breezes. ",
            "Great day for outdoor activities!"
        ]
        
        if context.is_streaming:
            # Streaming override
            async def override_stream() -> AsyncIterable[AgentRunResponseUpdate]:
                for chunk in custom_message_parts:
                    yield AgentRunResponseUpdate(contents=[TextContent(text=chunk)])
            
            context.result = override_stream()
        else:
            # Non-streaming override
            custom_message = "".join(custom_message_parts)
            context.result = AgentRunResponse(
                messages=[ChatMessage(role=Role.ASSISTANT, text=custom_message)]
            )
```

This middleware approach allows you to implement sophisticated response transformation, content filtering, result enhancement, and streaming customization while keeping your agent logic clean and focused.

::: zone-end


## Next steps

> [!div class="nextstepaction"]
> [Agent Memory](./agent-memory.md)
