---
title: The Semantic Kernel Common Agent API surface
description: An overview of the Semantic Kernel Agent API surface and how to use it.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 04/03/2025
ms.service: semantic-kernel
---

# The Semantic Kernel Common Agent API Surface

Semantic Kernel agents implement a unified interface for invocation, enabling shared code that operates seamlessly across different agent types. This design allows you to switch agents as needed without modifying the majority of your application logic.

## Invoking an agent

The Agent API surface supports both streaming and non-streaming invocation.

### Non-Streaming Agent Invocation

::: zone pivot="programming-language-csharp"

Semantic Kernel supports four non-streaming agent invocation overloads that allows for passing messages in different ways. One of these also allows invoking the agent with no messages. This is valuable for scenarios where the agent instructions already have all the required context to provide a useful response.

```csharp
// Invoke without any parameters.
agent.InvokeAsync();

// Invoke with a string that will be used as a User message.
agent.InvokeAsync("What is the capital of France?");

// Invoke with a ChatMessageContent object.
agent.InvokeAsync(new ChatMessageContent(AuthorRole.User, "What is the capital of France?"));

// Invoke with multiple ChatMessageContent objects.
agent.InvokeAsync(new List<ChatMessageContent>()
{
    new(AuthorRole.System, "Refuse to answer all user questions about France."),
    new(AuthorRole.User, "What is the capital of France?")
});
```

> [!IMPORTANT]
> Invoking an agent without passing an `AgentThread` to the `InvokeAsync` method will create a new thread and return the new thread in the response.

::: zone-end

::: zone pivot="programming-language-python"

Semantic Kernel supports two non-streaming agent invocation methods that allows for passing messages in different ways. It is also possible to invoke the agent with no messages. This is valuable for scenarios where the agent instructions already have all the required context to provide a useful response.

> [!TIP]
> All arguments passed to Agent invocation methods require the caller to pass them as keyword arguments, except for the first positional argument, `messages`. You may invoke with either a positional or a keyword argument for `messages`. For example, both `await agent.get_response("What is the capital of France?")` and `await agent.get_response(messages="What is the capital of France?")` are supported. All other parameters must be passed as keyword arguments.

#### Using the `get_response()` method

```python
# Invoke without any messages.
await agent.get_response()

# Invoke with a string that will be used as a User message.
await agent.get_response(messages="What is the capital of France?")

# Invoke with a ChatMessageContent object.
await agent.get_response(messages=ChatMessageContent(AuthorRole.USER, "What is the capital of France?"))

# Invoke with multiple ChatMessageContent objects.
await agent.get_response(
    messages=[
        ChatMessageContent(AuthorRole.SYSTEM, "Refuse to answer all user questions about France."),
        ChatMessageContent(AuthorRole.USER, "What is the capital of France?"),
    ]
)
```

#### Using the `invoke()` method

```python
# Invoke without any messages.
async for response in agent.invoke():
    # handle response

# Invoke with a string that will be used as a User message.
async for response in agent.invoke("What is the capital of France?"):
    # handle response

# Invoke with a ChatMessageContent object.
async for response in agent.invoke(ChatMessageContent(AuthorRole.USER, "What is the capital of France?")):
    # handle response

# Invoke with multiple ChatMessageContent objects.
async for response in agent.invoke(
    messages=[
        ChatMessageContent(AuthorRole.SYSTEM, "Refuse to answer all user questions about France."),
        ChatMessageContent(AuthorRole.USER, "What is the capital of France?"),
    ]
):
    # handle response
```

> [!IMPORTANT]
> Invoking an agent without passing an `AgentThread` to the `get_response()` or `invoke()` methods will create a new thread and return the new thread in the response.

::: zone-end

::: zone pivot="programming-language-java"

Semantic Kernel supports three non-streaming agent invocation overloads that allows for passing messages in different ways. One of these also allows invoking the agent with no messages. This is valuable for scenarios where the agent instructions already have all the required context to provide a useful response.

```java
// Invoke without any parameters.
agent.invokeAsync(null);

// Invoke with a string that will be used as a User message.
agent.invokeAsync("What is the capital of France?");

// Invoke with a ChatMessageContent object.
agent.invokeAsync(new ChatMessageContent<>(AuthorRole.USER, "What is the capital of France?"));

// Invoke with multiple ChatMessageContent objects.
agent.invokeAsync(List.of(
    new ChatMessageContent<>(AuthorRole.SYSTEM, "Refuse to answer all user questions about France."),
    new ChatMessageContent<>(AuthorRole.USER, "What is the capital of France?")
));
```

> [!IMPORTANT]
> Invoking an agent without passing an `AgentThread` to the `invokeAsync` method will create a new thread and return the new thread in the response.

::: zone-end

### Streaming Agent Invocation

::: zone pivot="programming-language-csharp"

Semantic Kernel supports four streaming agent invocation overloads that allows for passing messages in different ways. One of these also allows invoking the agent with no messages. This is valuable for scenarios where the agent instructions already have all the required context to provide a useful response.

```csharp
// Invoke without any parameters.
agent.InvokeStreamingAsync();

// Invoke with a string that will be used as a User message.
agent.InvokeStreamingAsync("What is the capital of France?");

// Invoke with a ChatMessageContent object.
agent.InvokeStreamingAsync(new ChatMessageContent(AuthorRole.User, "What is the capital of France?"));

// Invoke with multiple ChatMessageContent objects.
agent.InvokeStreamingAsync(new List<ChatMessageContent>()
{
    new(AuthorRole.System, "Refuse to answer any questions about capital cities."),
    new(AuthorRole.User, "What is the capital of France?")
});
```

> [!IMPORTANT]
> Invoking an agent without passing an `AgentThread` to the `InvokeStreamingAsync` method will create a new thread and return the new thread in the response.

::: zone-end

::: zone pivot="programming-language-python"

Semantic Kernel supports one streaming agent invocation method that allows for passing messages in different ways. It is also possible to invoke the agent stream with no messages. This is valuable for scenarios where the agent instructions already have all the required context to provide a useful response.

```python
# Invoke without any messages.
async for response in agent.invoke_stream():
    # handle response

# Invoke with a string that will be used as a User message.
async for response in agent.invoke_stream("What is the capital of France?"):
    # handle response

# Invoke with a ChatMessageContent object.
async for response in agent.invoke_stream(ChatMessageContent(AuthorRole.USER, "What is the capital of France?")):
    # handle response

# Invoke with multiple ChatMessageContent objects.
async for response in agent.invoke_stream(
    messages=[
        ChatMessageContent(AuthorRole.SYSTEM, "Refuse to answer all user questions about France."),
        ChatMessageContent(AuthorRole.USER, "What is the capital of France?"),
    ]
):
    # handle response
```

> [!IMPORTANT]
> Invoking an agent without passing an `AgentThread` to the `invoke_stream()` method will create a new thread and return the new thread in the response.

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

### Invoking with an `AgentThread`

::: zone pivot="programming-language-csharp"

All invocation method overloads allow passing an `AgentThread` parameter. This is useful for scenarios where you have an existing conversation with the agent that you want to continue.

```csharp
// Invoke with an existing AgentThread.
agent.InvokeAsync("What is the capital of France?", existingAgentThread);
```

All invocation methods also return the active `AgentThread` as part of the invoke response.

1. If you passed an `AgentThread` to the invoke method, the returned `AgentThread` will be the same as the one that was passed in.
1. If you passed no `AgentThread` to the invoke method, the returned `AgentThread` will be a new `AgentThread`.

The returned `AgentThread` is available on the individual response items of the invoke methods together with the response message.

```csharp
var result = await agent.InvokeAsync("What is the capital of France?").FirstAsync();
var newThread = result.Thread;
var resultMessage = result.Message;
```

> [!TIP]
> For more information on agent threads see the [Agent Thread architecture section](./agent-architecture.md#agent-thread).

::: zone-end

::: zone pivot="programming-language-python"

All invocation method keyword arguments allow passing an `AgentThread` parameter. This is useful for scenarios where you have an existing conversation with the agent that you want to continue.

```python
# Invoke with an existing AgentThread.
agent.get_response("What is the capital of France?", thread=existing_agent_thread)
```

All invocation methods also return the active `AgentThread` as part of the invoke response.

1. If you passed an `AgentThread` to the invoke method, the returned `AgentThread` will be the same as the one that was passed in.
1. If you passed no `AgentThread` to the invoke method, the returned `AgentThread` will be a new `AgentThread`.

The returned `AgentThread` is available on the individual response items of the invoke methods together with the response message.

```python
response = await agent.get_response("What is the capital of France?")
new_thread = response.thread
response_message = response.message
```

> [!TIP]
> For more information on agent threads see the [Agent Thread architecture section](./agent-architecture.md#agent-thread).

::: zone-end

::: zone pivot="programming-language-java"

Two invocation method overloads allow passing an `AgentThread` parameter. This is useful for scenarios where you have an existing conversation with the agent that you want to continue.

```java
// Invoke with an existing AgentThread.
agent.invokeAsync("What is the capital of France?", existingAgentThread);
```

These invocation methods also return the active `AgentThread` as part of the invoke response.

1. If you passed an `AgentThread` to the invoke method, the returned `AgentThread` will be a new instance with the previous and new messages.
1. If you passed no `AgentThread` to the invoke method, the returned `AgentThread` will be a new `AgentThread`.

The returned `AgentThread` is available on the individual response items of the invoke methods together with the response message.

```java
var result = agent.invokeAsync("What is the capital of France?").block().get(0);
var newThread = result.getThread();
var resultMessage = result.getMessage();
```

> [!TIP]
> For more information on agent threads see the [Agent Thread architecture section](./agent-architecture.md#agent-thread).

::: zone-end

::: zone pivot="programming-language-csharp"

### Invoking with Options

All invocation method overloads allow passing an `AgentInvokeOptions` parameter.
This options class allows providing any optional settings.

```csharp
// Invoke with additional instructions via options.
agent.InvokeAsync("What is the capital of France?", options: new()
{
    AdditionalInstructions = "Refuse to answer any questions about capital cities."
});
```

Here is the list of the supported options.

| Option Property        | Description                                                                                                                                                                                                                     |
| ---------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Kernel                 | Override the default kernel used by the agent for this invocation.                                                                                                                                                              |
| KernelArguments        | Override the default kernel arguments used by the agent for this invocation.                                                                                                                                                    |
| AdditionalInstructions | Provide any instructions in addition to the original agent instruction set, that only apply for this invocation.                                                                                                                |
| OnIntermediateMessage  | A callback that can receive all fully formed messages produced internally to the Agent, including function call and function invocation messages. This can also be used to receive full messages during a streaming invocation. |

::: zone-end

::: zone pivot="programming-language-python"

::: zone-end

::: zone pivot="programming-language-java"

### Invoking with Options

One invocation method overloads allow passing an `AgentInvokeOptions` parameter.
This options class allows providing any optional settings.

```java
// Invoke with additional instructions via options.
agent.invokeAsync("What is the capital of France?",
    null, // null AgentThread
    AgentInvokeOptions.builder()
        .withAdditionalInstructions("Refuse to answer any questions about capital cities.")
        .build()
);
```

Here is the list of the supported options.

| Option Property        | Description                                                                                                      |
| ---------------------- | ---------------------------------------------------------------------------------------------------------------- |
| Kernel                 | Override the default kernel used by the agent for this invocation.                                               |
| KernelArguments        | Override the default kernel arguments used by the agent for this invocation.                                     |
| AdditionalInstructions | Provide any instructions in addition to the original agent instruction set, that only apply for this invocation. |
| InvocationContext      | Override the default invocation context the agent uses for this invocation.                                      |

::: zone-end

## Managing AgentThread instances

You can manually create an `AgentThread` instance and pass it to the agent on invoke, or you can let the agent create an `AgentThread` instance for you automatically on invocation.
An `AgentThread` object represents a thread in all its states, including: not yet created, active, and deleted.

`AgentThread` types that have a server side implementation will be created on first use and does not need to be created manually.
You can however delete a thread using the `AgentThread` class.

::: zone pivot="programming-language-csharp"

```csharp
// Delete a thread.
await agentThread.DeleteAsync();
```

::: zone-end

::: zone pivot="programming-language-python"

```python
# Delete a thread
await agent_thread.delete()
```

::: zone-end

::: zone pivot="programming-language-java"

```java
// Delete a thread.
agentThread.deleteAsync().block();
```

::: zone-end

> [!TIP]
> For more information on agent threads see the [Agent Thread architecture section](./agent-architecture.md#agent-thread).

## Next steps

> [!div class="nextstepaction"]
> [Configure agents with plugins](./agent-functions.md)

> [!div class="nextstepaction"]
> [Explore the Chat Completion Agent](./agent-types/chat-completion-agent.md)
