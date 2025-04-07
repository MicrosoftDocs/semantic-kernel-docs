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

The `Agent Framework` supports streamed responses when using [`AgentChat`](./agent-chat.md) or when directly invoking a [`ChatCompletionAgent`](./chat-completion-agent.md) or [`OpenAIAssistantAgent`](./assistant-agent.md). In either mode, the framework delivers responses asynchronously as they are streamed. Alongside the streamed response, a consistent, non-streamed history is maintained to track the conversation. This ensures both real-time interaction and a reliable record of the conversation's flow.

### Streamed response from `ChatCompletionAgent`

When invoking a streamed response from a [`ChatCompletionAgent`](./chat-completion-agent.md), the `ChatHistory` in the `AgentThread` is updated after the full response is received. Although the response is streamed incrementally, the history records only the complete message. This ensures that the `ChatHistory` reflects fully formed responses for consistency.

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

> Agents are currently unavailable in Java.

::: zone-end

### Streamed response from `OpenAIAssistantAgent`

When invoking a streamed response from an [`OpenAIAssistantAgent`](./assistant-agent.md), the assistant maintains the conversation state as a remote thread. It is possible to read the messages from the remote thread if required.

::: zone pivot="programming-language-csharp"
```csharp
// Define agent
OpenAIAssistantAgent agent = ...;

// Create a thread for the agent conversation.
OpenAIAssistantAgentThread agentThread = new(assistantClient);

// Cerate a user message
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

// Cerate a user message
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

> Agents are currently unavailable in Java.

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

    for user_input in USER_INPUTS:
        print(f"# User: '{user_input}'")
        first_chunk = True
        # 4. Invoke the agent for the current message and print the response
        async for response in agent.invoke_stream(messages=user_input, thread=thread):
            thread = response.thread
            if first_chunk:
                print(f"# {response.name}: ", end="", flush=True)
                first_chunk = False
            print(response.content, end="", flush=True)
        print()

    # Print the intermediate steps
    print("\nIntermediate Steps:")
    for msg in intermediate_steps:
        if any(isinstance(item, FunctionResultContent) for item in msg.items):
            for fr in msg.items:
                if isinstance(fr, FunctionResultContent):
                    print(f"Function Result:> {fr.result} for function: {fr.name}")
        elif any(isinstance(item, FunctionCallContent) for item in msg.items):
            for fcc in msg.items:
                if isinstance(fcc, FunctionCallContent):
                    print(f"Function Call:> {fcc.name} with arguments: {fcc.arguments}")
        else:
            print(f"{msg.role}: {msg.content}")

if __name__ == "__main__":
    asyncio.run(main())
```

The following demonstrates sample output from the agent invocation process:

```bash
Sample Output:

# AuthorRole.USER: 'Hello'
# Host: Hi there! How can I assist you with the menu today?
# AuthorRole.USER: 'What is the special soup?'
# Host: The special soup is Clam Chowder.
# AuthorRole.USER: 'What is the special drink?'
# Host: The special drink is Chai Tea.
# AuthorRole.USER: 'How much is that?'
# Host: Could you please specify the menu item you are asking about?
# AuthorRole.USER: 'Thank you'
# Host: You're welcome! If you have any questions about the menu or need assistance, feel free to ask.

Intermediate Steps:
AuthorRole.ASSISTANT: Hi there! How can I assist you with the menu today?
AuthorRole.ASSISTANT: 
Function Result:> 
        Special Soup: Clam Chowder
        Special Salad: Cobb Salad
        Special Drink: Chai Tea
        for function: MenuPlugin-get_specials
AuthorRole.ASSISTANT: The special soup is Clam Chowder.
AuthorRole.ASSISTANT: 
Function Result:> 
        Special Soup: Clam Chowder
        Special Salad: Cobb Salad
        Special Drink: Chai Tea
        for function: MenuPlugin-get_specials
AuthorRole.ASSISTANT: The special drink is Chai Tea.
AuthorRole.ASSISTANT: Could you please specify the menu item you are asking about?
AuthorRole.ASSISTANT: You're welcome! If you have any questions about the menu or need assistance, feel free to ask.
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
        // Process beginning of agent response
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
