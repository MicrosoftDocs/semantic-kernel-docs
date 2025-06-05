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

## What is a Streamed Response?

A streamed response delivers the message content in small, incremental chunks. This approach enhances the user experience by allowing them to view and engage with the message as it unfolds, rather than waiting for the entire response to load. Users can begin processing information immediately, improving the sense of responsiveness and interactivity. As a result, it minimizes delays and keeps users more engaged throughout the communication process.

### Streaming References

- [OpenAI Streaming Guide](https://platform.openai.com/docs/api-reference/streaming)
- [OpenAI Chat Completion Streaming](https://platform.openai.com/docs/api-reference/chat/create#chat-create-stream)
- [OpenAI Assistant Streaming](https://platform.openai.com/docs/api-reference/assistants-streaming)
- [Azure OpenAI Service REST API](/azure/ai-services/openai/reference)

## Streaming in Semantic Kernel

[AI Services](../../concepts/ai-services/index.md) that support streaming in Semantic Kernel use different content types compared to those used for fully-formed messages. These content types are specifically designed to handle the incremental nature of streaming data. The same content types are also utilized within the Agent Framework for similar purposes. This ensures consistency and efficiency across both systems when dealing with streaming information.

::: zone pivot="programming-language-csharp"

> [!TIP]
> API reference:
>
> - [`StreamingChatMessageContent`](/dotnet/api/microsoft.semantickernel.streamingchatmessagecontent)
> - [`StreamingTextContent`](/dotnet/api/microsoft.semantickernel.streamingtextcontent)
> - [`StreamingFileReferenceContent`](/dotnet/api/microsoft.semantickernel.streamingfilereferencecontent)
> - [`StreamingAnnotationContent`](/dotnet/api/microsoft.semantickernel.agents.openai.streamingannotationcontent)

::: zone-end

::: zone pivot="programming-language-python"

> [!TIP]
> API reference:
>
> - [`streaming_chat_message_content`](/python/api/semantic-kernel/semantic_kernel.contents.streaming_chat_message_content)
> - [`streaming_text_content`](/python/api/semantic-kernel/semantic_kernel.contents.streaming_text_content)
> - [`streaming_file_reference_content`](/python/api/semantic-kernel/semantic_kernel.contents.streaming_file_reference_content)
> - [`streaming_annotation_content`](/python/api/semantic-kernel/semantic_kernel.contents.streaming_annotation_content)

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

### Streamed response from `ChatCompletionAgent`

When invoking a streamed response from a [`ChatCompletionAgent`](./agent-types/chat-completion-agent.md), the `ChatHistory` in the `AgentThread` is updated after the full response is received. Although the response is streamed incrementally, the history records only the complete message. This ensures that the `ChatHistory` reflects fully formed responses for consistency.

::: zone pivot="programming-language-csharp"

```csharp
// Define agent
ChatCompletionAgent agent = ...;

ChatHistoryAgentThread agentThread = new();

// Create a user message
var message = ChatMessageContent(AuthorRole.User, "<user input>");

// Generate the streamed agent response(s)
await foreach (StreamingChatMessageContent response in agent.InvokeStreamingAsync(message, agentThread))
{
  // Process streamed response(s)...
}

// It's also possible to read the messages that were added to the ChatHistoryAgentThread.
await foreach (ChatMessageContent response in agentThread.GetMessagesAsync())
{
  // Process messages...
}
```

::: zone-end

::: zone pivot="programming-language-python"

```python
from semantic_kernel.agents import ChatCompletionAgent, ChatHistoryAgentThread

# Define agent
agent = ChatCompletionAgent(...)

# Create a thread object to maintain the conversation state.
# If no thread is provided one will be created and returned with
# the initial response.
thread: ChatHistoryAgentThread = None

# Generate the streamed agent response(s)
async for response in agent.invoke_stream(messages="user input", thread=thread)
{
  # Process streamed response(s)...
  thread = response.thread
}
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

### Streamed response from `OpenAIAssistantAgent`

When invoking a streamed response from an [`OpenAIAssistantAgent`](./agent-types/assistant-agent.md), the assistant maintains the conversation state as a remote thread. It is possible to read the messages from the remote thread if required.

::: zone pivot="programming-language-csharp"

```csharp
// Define agent
OpenAIAssistantAgent agent = ...;

// Create a thread for the agent conversation.
OpenAIAssistantAgentThread agentThread = new(assistantClient);

// Create a user message
var message = new ChatMessageContent(AuthorRole.User, "<user input>");

// Generate the streamed agent response(s)
await foreach (StreamingChatMessageContent response in agent.InvokeStreamingAsync(message, agentThread))
{
  // Process streamed response(s)...
}

// It's possible to read the messages from the remote thread.
await foreach (ChatMessageContent response in agentThread.GetMessagesAsync())
{
  // Process messages...
}

// Delete the thread when it is no longer needed
await agentThread.DeleteAsync();
```

To create a thread using an existing `Id`, pass it to the constructor of `OpenAIAssistantAgentThread`:

```csharp
// Define agent
OpenAIAssistantAgent agent = ...;

// Create a thread for the agent conversation.
OpenAIAssistantAgentThread agentThread = new(assistantClient, "your-existing-thread-id");

// Create a user message
var message = new ChatMessageContent(AuthorRole.User, "<user input>");

// Generate the streamed agent response(s)
await foreach (StreamingChatMessageContent response in agent.InvokeStreamingAsync(message, agentThread))
{
  // Process streamed response(s)...
}

// It's possible to read the messages from the remote thread.
await foreach (ChatMessageContent response in agentThread.GetMessagesAsync())
{
  // Process messages...
}

// Delete the thread when it is no longer needed
await agentThread.DeleteAsync();
```

::: zone-end

::: zone pivot="programming-language-python"

```python
from semantic_kernel.agents import AssistantAgentThread, AzureAssistantAgent, OpenAIAssistantAgent

# Define agent
agent = OpenAIAssistantAgent(...)  # or = AzureAssistantAgent(...)

# Create a thread for the agent conversation.
# If no thread is provided one will be created and returned with
# the initial response.
thread: AssistantAgentThread = None

# Generate the streamed agent response(s)
async for response in agent.invoke_stream(messages="user input", thread=thread):
  # Process streamed response(s)...
  thread = response.thread

# Read the messages from the remote thread
async for response in thread.get_messages():
  # Process messages

# Delete the thread
await thread.delete()
```

To create a thread using an existing `thread_id`, pass it to the constructor of `AssistantAgentThread`:

```python
from semantic_kernel.agents import AssistantAgentThread, AzureAssistantAgent, OpenAIAssistantAgent

# Define agent
agent = OpenAIAssistantAgent(...)  # or = AzureAssistantAgent(...)

# Create a thread for the agent conversation.
# If no thread is provided one will be created and returned with
# the initial response.
thread = AssistantAgentThread(client=client, thread_id="your-existing-thread-id")

# Generate the streamed agent response(s)
async for response in agent.invoke_stream(messages="user input", thread=thread):
  # Process streamed response(s)...
  thread = response.thread

# Delete the thread
await thread.delete()
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Handling Intermediate Messages with a Streaming Response

The nature of streaming responses allows LLM models to return incremental chunks of text, enabling quicker rendering in a UI or console without waiting for the entire response to complete. Additionally, a caller might want to handle intermediate content, such as results from function calls. This can be achieved by supplying a callback function when invoking the streaming response. The callback function receives complete messages encapsulated within `ChatMessageContent`.

::: zone pivot="programming-language-csharp"

> Callback documentation for the `AzureAIAgent` is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

Configuring the `on_intermediate_message` callback within `agent.invoke_stream(...)` allows the caller to receive intermediate messages generated during the process of formulating the agent's final response.

```python
import asyncio
from typing import Annotated

from semantic_kernel.agents import AzureResponsesAgent
from semantic_kernel.contents import ChatMessageContent, FunctionCallContent, FunctionResultContent
from semantic_kernel.functions import kernel_function


# Define a sample plugin for the sample
class MenuPlugin:
    """A sample Menu Plugin used for the concept sample."""

    @kernel_function(description="Provides a list of specials from the menu.")
    def get_specials(self, menu_item: str) -> Annotated[str, "Returns the specials from the menu."]:
        return """
        Special Soup: Clam Chowder
        Special Salad: Cobb Salad
        Special Drink: Chai Tea
        """

    @kernel_function(description="Provides the price of the requested menu item.")
    def get_item_price(
        self, menu_item: Annotated[str, "The name of the menu item."]
    ) -> Annotated[str, "Returns the price of the menu item."]:
        return "$9.99"

# This callback function will be called for each intermediate message,
# which will allow one to handle FunctionCallContent and FunctionResultContent.
# If the callback is not provided, the agent will return the final response
# with no intermediate tool call steps.
async def handle_streaming_intermediate_steps(message: ChatMessageContent) -> None:
    for item in message.items or []:
        if isinstance(item, FunctionResultContent):
            print(f"Function Result:> {item.result} for function: {item.name}")
        elif isinstance(item, FunctionCallContent):
            print(f"Function Call:> {item.name} with arguments: {item.arguments}")
        else:
            print(f"{item}")

# Simulate a conversation with the agent
USER_INPUTS = [
    "Hello",
    "What is the special soup?",
    "What is the special drink?",
    "How much is it?",
    "Thank you",
]


async def main():
    # 1. Create the client using OpenAI resources and configuration
    client, model = AzureResponsesAgent.setup_resources()

    # 2. Create a Semantic Kernel agent for the OpenAI Responses API
    agent = AzureResponsesAgent(
        ai_model_id=model,
        client=client,
        instructions="Answer questions about the menu.",
        name="Host",
        plugins=[MenuPlugin()],
    )

    # 3. Create a thread for the agent
    # If no thread is provided, a new thread will be
    # created and returned with the initial response
    thread = None

    try:
        for user_input in user_inputs:
            print(f"# {AuthorRole.USER}: '{user_input}'")

            first_chunk = True
            async for response in agent.invoke_stream(
                messages=user_input,
                thread=thread,
                on_intermediate_message=handle_streaming_intermediate_steps,
            ):
                thread = response.thread
                if first_chunk:
                    print(f"# {response.name}: ", end="", flush=True)
                    first_chunk = False
                print(response.content, end="", flush=True)
            print()
    finally:
        await thread.delete() if thread else None

if __name__ == "__main__":
    asyncio.run(main())
```

The following demonstrates sample output from the agent invocation process:

```bash
Sample Output:

# AuthorRole.USER: 'Hello'
# Host: Hello! How can I assist you with the menu today?
# AuthorRole.USER: 'What is the special soup?'
Function Call:> MenuPlugin-get_specials with arguments: {}
Function Result:>
        Special Soup: Clam Chowder
        Special Salad: Cobb Salad
        Special Drink: Chai Tea
        for function: MenuPlugin-get_specials
# Host: The special soup today is Clam Chowder. Would you like to know more about it or hear about other specials?
# AuthorRole.USER: 'What is the special drink?'
# Host: The special drink today is Chai Tea. Would you like more details or are you interested in ordering it?
# AuthorRole.USER: 'How much is that?'
Function Call:> MenuPlugin-get_item_price with arguments: {"menu_item":"Chai Tea"}
Function Result:> $9.99 for function: MenuPlugin-get_item_price
# Host: The special drink, Chai Tea, is $9.99. Would you like to order one or need information on something else?
# AuthorRole.USER: 'Thank you'
# Host: You're welcome! If you have any more questions or need help with the menu, just let me know. Enjoy your day!
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Next Steps

> [!div class="nextstepaction"]
> [Using templates with agents](./agent-templates.md)

> [!div class="nextstepaction"]
> [Agent orchestration](./agent-orchestration/index.md)
