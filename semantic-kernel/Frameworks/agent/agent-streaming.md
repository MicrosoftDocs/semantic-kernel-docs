---
title: How to Stream Agent Responses.
description: Describes how to utilize streamed responses for agents and agent chat.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# How to Stream Agent Responses

> [!IMPORTANT]
> This feature is in the release candidate stage. Features at this stage are nearly complete and generally stable, though they may undergo minor refinements or optimizations before reaching full general availability.

## What is a Streamed Response?

A streamed response delivers the message content in small, incremental chunks. This approach enhances the user experience by allowing them to view and engage with the message as it unfolds, rather than waiting for the entire response to load. Users can begin processing information immediately, improving the sense of responsiveness and interactivity. As a result, it minimizes delays and keeps users more engaged throughout the communication process.

#### Streaming References:

- [OpenAI Streaming Guide](https://platform.openai.com/docs/api-reference/streaming)
- [OpenAI Chat Completion Streaming](https://platform.openai.com/docs/api-reference/chat/create#chat-create-stream)
- [OpenAI Assistant Streaming](https://platform.openai.com/docs/api-reference/assistants-streaming)
- [Azure OpenAI Service REST API](/azure/ai-services/openai/reference)


## Streaming in Semantic Kernel

[AI Services](../../concepts/ai-services/index.md) that support streaming in Semantic Kernel use different content types compared to those used for fully-formed messages. These content types are specifically designed to handle the incremental nature of streaming data. The same content types are also utilized within the Agent Framework for similar purposes. This ensures consistency and efficiency across both systems when dealing with streaming information.

::: zone pivot="programming-language-csharp"
- [`StreamingChatMessageContent`](/dotnet/api/microsoft.semantickernel.streamingchatmessagecontent)
- [`StreamingTextContent`](/dotnet/api/microsoft.semantickernel.streamingtextcontent)
- [`StreamingFileReferenceContent`](/dotnet/api/microsoft.semantickernel.streamingfilereferencecontent)
- [`StreamingAnnotationContent`](/dotnet/api/microsoft.semantickernel.agents.openai.streamingannotationcontent)

::: zone-end

::: zone pivot="programming-language-python"
- [`streaming_chat_message_content`](/python/api/semantic-kernel/semantic_kernel.contents.streaming_chat_message_content)
- [`streaming_text_content`](/python/api/semantic-kernel/semantic_kernel.contents.streaming_text_content)
- [`streaming_file_reference_content`](/python/api/semantic-kernel/semantic_kernel.contents.streaming_file_reference_content)
- [`streaming_annotation_content`](/python/api/semantic-kernel/semantic_kernel.contents.streaming_annotation_content)

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Streaming Agent Invocation

The `Agent Framework` supports _streamed_ responses when using [`AgentChat`](./agent-chat.md) or when directly invoking a [`ChatCompletionAgent`](./chat-completion-agent.md) or [`OpenAIAssistantAgent`](./assistant-agent.md). In either mode, the framework delivers responses asynchronously as they are streamed. Alongside the streamed response, a consistent, non-streamed history is maintained to track the conversation. This ensures both real-time interaction and a reliable record of the conversation's flow.

### Streamed response from `ChatCompletionAgent`

When invoking a streamed response from a [`ChatCompletionAgent`](./chat-completion-agent.md), the `ChatHistory` is updated after the full response is received. Although the response is streamed incrementally, the history records only the complete message. This ensures that the `ChatHistory` reflects fully formed responses for consistency.

::: zone pivot="programming-language-csharp"
```csharp
// Define agent
ChatCompletionAgent agent = ...;

// Create a ChatHistory object to maintain the conversation state.
ChatHistory chat = [];

// Add a user message to the conversation
chat.Add(new ChatMessageContent(AuthorRole.User, "<user input>"));

// Generate the streamed agent response(s)
await foreach (StreamingChatMessageContent response in agent.InvokeStreamingAsync(chat))
{
  // Process streamed response(s)...
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Define agent
agent = ChatCompletionAgent(...)

# Create a ChatHistory object to maintain the conversation state.
chat = ChatHistory()

# Add a user message to the conversation
chat.add_message(ChatMessageContent(AuthorRole.USER, "<user input>"))

# Generate the streamed agent response(s)
async for response in agent.invoke_stream(chat)
{
  # Process streamed response(s)...
}
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

### Streamed response from `OpenAIAssistantAgent`

When invoking a streamed response from an [`OpenAIAssistantAgent`](./assistant-agent.md), an optional `ChatHistory` can be provided to capture the complete messages for further analysis if needed. Since the assistant maintains the conversation state as a remote thread, capturing these messages is not always necessary. The decision to store and analyze the full response depends on the specific requirements of the interaction.

::: zone pivot="programming-language-csharp"
```csharp
// Define agent
OpenAIAssistantAgent agent = ...;

// Create a thread for the agent conversation.
string threadId = await agent.CreateThreadAsync();

// Add a user message to the conversation
chat.Add(threadId, new ChatMessageContent(AuthorRole.User, "<user input>"));

// Generate the streamed agent response(s)
await foreach (StreamingChatMessageContent response in agent.InvokeStreamingAsync(threadId))
{
  // Process streamed response(s)...
}

// Delete the thread when it is no longer needed
await agent.DeleteThreadAsync(threadId);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Define agent
agent = OpenAIAssistantAgent(...)

# Create a thread for the agent conversation.
thread_id = await agent.create_thread()

# Add user message to the conversation
await agent.add_chat_message(message="<user input>")

# Generate the streamed agent response(s)
async for response in agent.invoke_stream(thread_id=thread_id):
  # Process streamed response(s)...
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Streaming with `AgentChat`

When using [`AgentChat`](./agent-chat.md), the full conversation history is always preserved and can be accessed directly through the [`AgentChat`](./agent-chat.md) instance. Therefore, the key difference between streamed and non-streamed invocations lies in the delivery method and the resulting content type. In both cases, users can still access the complete history, but streamed responses provide real-time updates as the conversation progresses. This allows for greater flexibility in handling interactions, depending on the application's needs.

::: zone pivot="programming-language-csharp"
```csharp
// Define agents
ChatCompletionAgent agent1 = ...;
OpenAIAssistantAgent agent2 = ...;

// Create chat with participating agents.
AgentGroupChat chat =
  new(agent1, agent2)
  {
    // Override default execution settings
    ExecutionSettings =
    {
        TerminationStrategy = { MaximumIterations = 10 }
    }
  };

// Invoke agents
string lastAgent = string.Empty;
await foreach (StreamingChatMessageContent response in chat.InvokeStreamingAsync())
{
    if (!lastAgent.Equals(response.AuthorName, StringComparison.Ordinal))
    {
        // Process begining of agent response
        lastAgent = response.AuthorName;
    }

    // Process streamed content...
} 
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Define agents
agent1 = ChatCompletionAgent(...)
agent2 = OpenAIAssistantAgent(...)

# Create chat with participating agents
chat = AgentGroupChat(
  agents=[agent1, agent2],
  termination_strategy=DefaultTerminationStrategy(maximum_iterations=10),
)

await chat.add_chat_message("<input>")

# Invoke agents
last_agent = None
async for response in chat.invoke_stream():
    if message.content is not None:
        if last_agent != response.name:
            # Process beginning of agent response
            last_agent = message.name
        # Process streamed content
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end
