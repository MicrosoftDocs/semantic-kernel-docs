---
title: Creating and managing a chat history object
description: Use chat history to maintain a record of messages in a chat session
zone_pivot_groups: programming-languages
author: evanmattson
ms.topic: conceptual
ms.author: evmattso
ms.date: 01/20/2025
ms.service: semantic-kernel
---

# Chat history

The chat history object is used to maintain a record of messages in a chat session. It is used to store messages from different authors, such as users, assistants, tools, or the system. As the primary mechanism for sending and receiving messages, the chat history object is essential for maintaining context and continuity in a conversation.

## Creating a chat history object

A chat history object is a list under the hood, making it easy to create and add messages to.

::: zone pivot="programming-language-csharp"

```csharp
using Microsoft.SemanticKernel.ChatCompletion;

// Create a chat history object
ChatHistory chatHistory = [];

chatHistory.AddSystemMessage("You are a helpful assistant.");
chatHistory.AddUserMessage("What's available to order?");
chatHistory.AddAssistantMessage("We have pizza, pasta, and salad available to order. What would you like to order?");
chatHistory.AddUserMessage("I'd like to have the first option, please.");
```

::: zone-end

::: zone pivot="programming-language-python"

```python
# Create a chat history object
chat_history = ChatHistory()

chat_history.add_system_message("You are a helpful assistant.")
chat_history.add_user_message("What's available to order?")
chat_history.add_assistant_message("We have pizza, pasta, and salad available to order. What would you like to order?")
chat_history.add_user_message("I'd like to have the first option, please.")
```

::: zone-end

::: zone pivot="programming-language-java"

```java
import com.microsoft.semantickernel.services.chatcompletion.ChatHistory;

// Create a chat history object
ChatHistory chatHistory = new ChatHistory();

chatHistory.addSystemMessage("You are a helpful assistant.");
chatHistory.addUserMessage("What's available to order?");
chatHistory.addAssistantMessage("We have pizza, pasta, and salad available to order. What would you like to order?");
chatHistory.addUserMessage("I'd like to have the first option, please.");
```

::: zone-end

## Adding richer messages to a chat history

The easiest way to add messages to a chat history object is to use the methods above. However, you can also add messages manually by creating a new `ChatMessage` object. This allows you to provide additional information, like names and images content.

::: zone pivot="programming-language-csharp"
```csharp
using Microsoft.SemanticKernel.ChatCompletion;

// Add system message
chatHistory.Add(
    new() {
        Role = AuthorRole.System,
        Content = "You are a helpful assistant"
    }
);

// Add user message with an image
chatHistory.Add(
    new() {
        Role = AuthorRole.User,
        AuthorName = "Laimonis Dumins",
        Items = [
            new TextContent { Text = "What available on this menu" },
            new ImageContent { Uri = new Uri("https://example.com/menu.jpg") }
        ]
    }
);

// Add assistant message
chatHistory.Add(
    new() {
        Role = AuthorRole.Assistant,
        AuthorName = "Restaurant Assistant",
        Content = "We have pizza, pasta, and salad available to order. What would you like to order?"
    }
);

// Add additional message from a different user
chatHistory.Add(
    new() {
        Role = AuthorRole.User,
        AuthorName = "Ema Vargova",
        Content = "I'd like to have the first option, please."
    }
);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
from semantic_kernel.contents.chat_history import ChatHistory
from semantic_kernel.contents import ChatMessageContent, TextContent, ImageContent
from semantic_kernel.contents.utils.author_role import AuthorRole

# Add system message
chat_history.add_message(
    ChatMessage(
        role=AuthorRole.System,
        content="You are a helpful assistant"
    )
)

# Add user message with an image
chat_history.add_message(
    ChatMessageContent(
        role=AuthorRole.USER,
        name="Laimonis Dumins",
        items=[
            TextContent(text="What available on this menu"),
            ImageContent(uri="https://example.com/menu.jpg")
        ]
    )
)

# Add assistant message
chat_history.add_message(
    ChatMessageContent(
        role=AuthorRole.ASSISTANT,
        name="Restaurant Assistant",
        content="We have pizza, pasta, and salad available to order. What would you like to order?"
    )
)

# Add additional message from a different user
chat_history.add_message(
    ChatMessageContent(
        role=AuthorRole.USER,
        name="Ema Vargova",
        content="I'd like to have the first option, please."
    )
)
```
::: zone-end

::: zone pivot="programming-language-java"

```java
import com.microsoft.semantickernel.services.chatcompletion.message.ChatMessageImageContent;
import com.microsoft.semantickernel.services.chatcompletion.message.ChatMessageTextContent;

// Add system message
chatHistory.addSystemMessage(
    "You are a helpful assistant"
);

// Add user message with an image
chatHistory.addUserMessage(
    "What available on this menu"
);

chatHistory.addMessage(
    ChatMessageImageContent.builder()
            .withImageUrl("https://example.com/menu.jpg")
            .build()
);

// Add assistant message
chatHistory.addAssistantMessage(
    "We have pizza, pasta, and salad available to order. What would you like to order?"
);

// Add additional message from a different user
chatHistory.addUserMessage(
    "I'd like to have the first option, please."
);
```

::: zone-end

## Simulating function calls

In addition to user, assistant, and system roles, you can also add messages from the tool role to simulate function calls. This is useful for teaching the AI how to use plugins and to provide additional context to the conversation.

For example, to inject information about the current user in the chat history without requiring the user to provide the information or having the LLM waste time asking for it, you can use the tool role to provide the information directly.

Below is an example of how we're able to provide user allergies to the assistant by simulating a function call to the `User` plugin.

> [!TIP]
> Simulated function calls is particularly helpful for providing details about the current user(s). Today's LLMs have been trained to be particularly sensitive to user information. Even if you provide user details in a system message, the LLM may still choose to ignore it. If you provide it via a user message, or tool message, the LLM is more likely to use it.

::: zone pivot="programming-language-csharp"
```csharp
// Add a simulated function call from the assistant
chatHistory.Add(
    new() {
        Role = AuthorRole.Assistant,
        Items = [
            new FunctionCallContent(
                functionName: "get_user_allergies",
                pluginName: "User",
                id: "0001",
                arguments: new () { {"username", "laimonisdumins"} }
            ),
            new FunctionCallContent(
                functionName: "get_user_allergies",
                pluginName: "User",
                id: "0002",
                arguments: new () { {"username", "emavargova"} }
            )
        ]
    }
);

// Add a simulated function results from the tool role
chatHistory.Add(
    new() {
        Role = AuthorRole.Tool,
        Items = [
            new FunctionResultContent(
                functionName: "get_user_allergies",
                pluginName: "User",
                id: "0001",
                result: "{ \"allergies\": [\"peanuts\", \"gluten\"] }"
            )
        ]
    }
);
chatHistory.Add(
    new() {
        Role = AuthorRole.Tool,
        Items = [
            new FunctionResultContent(
                functionName: "get_user_allergies",
                pluginName: "User",
                id: "0002",
                result: "{ \"allergies\": [\"dairy\", \"soy\"] }"
            )
        ]
    }
);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
from semantic_kernel.contents import ChatMessageContent, FunctionCallContent, FunctionResultContent

# Add a simulated function call from the assistant
chat_history.add_message(
    ChatMessageContent(
        role=AuthorRole.ASSISTANT,
        items=[
            FunctionCallContent(
                name="get_user_allergies-User",
                id="0001",
                arguments=str({"username": "laimonisdumins"})
            ),
            FunctionCallContent(
                name="get_user_allergies-User",
                id="0002",
                arguments=str({"username": "emavargova"})
            )
        ]
    )
)

# Add a simulated function results from the tool role
chat_history.add_message(
    ChatMessageContent(
        role=AuthorRole.TOOL,
        items=[
            FunctionResultContent(
                name="get_user_allergies-User",
                id="0001",
                result="{ \"allergies\": [\"peanuts\", \"gluten\"] }"
            )
        ]
    )
)
chat_history.add_message(
    ChatMessageContent(
        role=AuthorRole.TOOL,
        items=[
            FunctionResultContent(
                name="get_user_allergies-User",
                id="0002",
                result="{ \"allergies\": [\"dairy\", \"gluten\"] }"
            )
        ]
    )
)
```
::: zone-end

::: zone pivot="programming-language-java"

```java
This functionality is not supported in the current version of Semantic Kernel for Java. 
```

::: zone-end

> [!IMPORTANT]
> When simulating tool results, you must always provide the `id` of the function call that the result corresponds to. This is important for the AI to understand the context of the result. Some LLMs, like OpenAI, will throw an error if the `id` is missing or if the `id` does not correspond to a function call.

## Inspecting a chat history object

Whenever you pass a chat history object to a chat completion service with auto function calling enabled, the chat history object will be manipulated so that it includes the function calls and results. This allows you to avoid having to manually add these messages to the chat history object and also allows you to inspect the chat history object to see the function calls and results.

You must still, however, add the final messages to the chat history object. Below is an example of how you can inspect the chat history object to see the function calls and results.

::: zone pivot="programming-language-csharp"
```csharp
using Microsoft.SemanticKernel.ChatCompletion;

ChatHistory chatHistory = [
    new() {
        Role = AuthorRole.User,
        Content = "Please order me a pizza"
    }
];

// Get the current length of the chat history object
int currentChatHistoryLength = chatHistory.Count;

// Get the chat message content
ChatMessageContent results = await chatCompletionService.GetChatMessageContentAsync(
    chatHistory,
    kernel: kernel
);

// Get the new messages added to the chat history object
for (int i = currentChatHistoryLength; i < chatHistory.Count; i++)
{
    Console.WriteLine(chatHistory[i]);
}

// Print the final message
Console.WriteLine(results);

// Add the final message to the chat history object
chatHistory.Add(results);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
from semantic_kernel.contents import ChatMessageContent

chat_history = ChatHistory([
    ChatMessageContent(
        role=AuthorRole.USER,
        content="Please order me a pizza"
    )
])

# Get the current length of the chat history object
current_chat_history_length = len(chat_history)

# Get the chat message content
results = await chat_completion.get_chat_message_content(
    chat_history=history,
    settings=execution_settings,
    kernel=kernel,
)

# Get the new messages added to the chat history object
for i in range(current_chat_history_length, len(chat_history)):
    print(chat_history[i])

# Print the final message
print(results)

# Add the final message to the chat history object
chat_history.add_message(results)
```
::: zone-end

::: zone pivot="programming-language-java"

```java
import com.microsoft.semantickernel.services.chatcompletion.ChatHistory;

ChatHistory chatHistory = new ChatHistory();
chatHistory.addUserMessage("Please order me a pizza");

// Get the chat message content
List<ChatMessageContent> results = chatCompletionService.getChatMessageContentsAsync(
    chatHistory,
    kernel,
    null
).block();

results.forEach(result -> System.out.println(result.getContent());

// Get the new messages added to the chat history object. By default, 
// the ChatCompletionService returns new messages only. 
chatHistory.addAll(results);
```

::: zone-end

## Chat History Reduction

Managing chat history is essential for maintaining context-aware conversations while ensuring efficient performance. As a conversation progresses, the history object can grow beyond the limits of a model’s context window, affecting response quality and slowing down processing. A structured approach to reducing chat history ensures that the most relevant information remains available without unnecessary overhead.

### Why Reduce Chat History?
- Performance Optimization: Large chat histories increase processing time. Reducing their size helps maintain fast and efficient interactions.
- Context Window Management: Language models have a fixed context window. When the history exceeds this limit, older messages are lost. Managing chat history ensures that the most important context remains accessible.
- Memory Efficiency: In resource-constrained environments such as mobile applications or embedded systems, unbounded chat history can lead to excessive memory usage and slow performance.
- Privacy and Security: Retaining unnecessary conversation history increases the risk of exposing sensitive information. A structured reduction process minimizes data retention while maintaining relevant context.

### Strategies for Reducing Chat History

Several approaches can be used to keep chat history manageable while preserving essential information:

- Truncation: The oldest messages are removed when the history exceeds a predefined limit, ensuring only recent interactions are retained.
- Summarization: Older messages are condensed into a summary, preserving key details while reducing the number of stored messages.
- Token-Based: Token-based reduction ensures chat history stays within a model’s token limit by measuring total token count and removing or summarizing older messages when the limit is exceeded.

A Chat History Reducer automates these strategies by evaluating the history’s size and reducing it based on configurable parameters such as target count (the desired number of messages to retain) and threshold count (the point at which reduction is triggered). By integrating these reduction techniques, chat applications can remain responsive and performant without compromising conversational context.

::: zone pivot="programming-language-csharp"

> Content About Chat History Reduction in C# is Coming Soon.

::: zone-end

::: zone pivot="programming-language-python"

In this section, we cover the implementation details of chat history reduction in Python. The approach involves creating a ChatHistoryReducer that integrates seamlessly with the ChatHistory object, allowing it to be used and passed wherever a chat history is required.

- Integration: In Python, the `ChatHistoryReducer` is designed to be a subclass of the `ChatHistory` object. This inheritance allows the reducer to be interchangeable with standard chat history instances.
- Reduction Logic: Users can invoke the `reduce` method on the chat history object. The reducer evaluates whether the current message count exceeds `target_count` plus `threshold_count` (if set). If it does, the history is reduced to `target_count` either by truncation or summarization.
- Configuration: The reduction behavior is configurable through parameters like `target_count` (the desired number of messages to retain) and threshold_count (the message count that triggers the reduction process).

The supported Semantic Kernel Python history reducers are `ChatHistorySummarizationReducer` and `ChatHistoryTruncationReducer`. As part of the reducer configuration, `auto_reduce` can be enabled to automatically apply history reduction when used with `add_message_async`, ensuring the chat history stays within the configured limits.

The following example demonstrates how to use ChatHistoryTruncationReducer to retain only the last two messages while maintaining conversation flow.

```python
import asyncio

from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.contents import ChatHistoryTruncationReducer
from semantic_kernel.kernel import Kernel


async def main():
    kernel = Kernel()
    kernel.add_service(AzureChatCompletion())

    # Keep the last two messages
    truncation_reducer = ChatHistoryTruncationReducer(
        target_count=2,
    )
    truncation_reducer.add_system_message("You are a helpful chatbot.")

    is_reduced = False

    while True:
        user_input = input("User:> ")

        if user_input.lower() == "exit":
            print("\n\nExiting chat...")
            break

        is_reduced = await truncation_reducer.reduce()
        if is_reduced:
            print(f"@ History reduced to {len(truncation_reducer.messages)} messages.")

        response = await kernel.invoke_prompt(
            prompt="{{$chat_history}}{{$user_input}}", user_input=user_input, chat_history=truncation_reducer
        )

        if response:
            print(f"Assistant:> {response}")
            truncation_reducer.add_user_message(str(user_input))
            truncation_reducer.add_message(response.value[0])

    if is_reduced:
        for msg in truncation_reducer.messages:
            print(f"{msg.role} - {msg.content}\n")
        print("\n")


if __name__ == "__main__":
    asyncio.run(main())
```

::: zone-end

::: zone pivot="programming-language-java"

> Chat History Reduction is currently unavailable in Java.

::: zone-end

## Next steps
Now that you know how to create and manage a chat history object, you can learn more about function calling in the [Function calling](./function-calling/index.md) topic.

> [!div class="nextstepaction"]
> [Learn how function calling works](./function-calling/index.md)