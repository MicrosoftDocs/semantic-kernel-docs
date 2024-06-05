---
title: Creating and managing a chat history object
description: Use chat history to maintain a record of messages in a chat session
zone_pivot_groups: programming-languages
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Chat history

The chat history object is used to maintain a record of messages in a chat session. It is used to store messages from different authors, such as users, assistants, tools, or the system. As the primary mechanism for sending and receiving messages, the chat history object is essential for maintaining context and continuity in a conversation.

## Creating a chat history object

A chat history object is a list under the hood, making it easy to create and add messages to.

```csharp
// Create a chat history object
ChatHistory chatHistory = [];

chatHistory.AddSystemMessage("You are a helpful assistant.");
chatHistory.AddUserMessage("What's available to order?");
chatHistory.AddAssistantMessage("We have pizza, pasta, and salad available to order. What would you like to order?");
chatHistory.AddUserMessage("I'd like to have the first option, please.");
```

## Adding messages to a chat history object

The easiest way to add messages to a chat history object is to use the methods above. However, you can also add messages manually by creating a new `ChatMessage` object. This allows you to provide additional information, like names and images content.

> [!NOTE]
> For more details about sending multi-modal messages to the AI, see the [Multi-modal messages](./multi-modal-messages.md) topic.

```csharp
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

In addition to user, assistant, and system roles, you can also add messages from the tool role to simulate function calls. This is useful for teaching the AI how to use plugins and to provide additional context to the conversation.

For example, to inject information about the current user in the chat history without requiring the user to provide the information or having the LLM waste time asking for it, you can use the tool role to provide the information directly.

Below is an example of how we're able to provide user allergies to the assistant by simulating a function call to the `UserPlugin` plugin.

> [!TIP]
> Simulated function calls is particularly helpful for providing details about the current user(s). Today's LLMs have been trained to be particularly sensitive to user information. Even if you provide user details in a system message, the LLM may still choose to ignore it. If you provide it via a user message, or tool message, the LLM is more likely to use it.

```csharp
// Add a simulated function call from the assistant
chatHistory.Add(
    new() {
        Role = AuthorRole.Assistant,
        Items = [
            new FunctionCallContent(
                functionName: "get_user_allergies",
                pluginName: "UserPlugin",
                id: "0001",
                arguments: new () { {"username", "laimonisdumins"} }
            )
            new FunctionCallContent(
                functionName: "get_user_allergies",
                pluginName: "UserPlugin",
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
                pluginName: "UserPlugin",
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
                pluginName: "UserPlugin",
                id: "0002",
                result: "{ \"allergies\": [\"dairy\", \"soy\"] }"
            )
        ]
    }
);
```

> [!IMPORTANT]
> When simulating tool results, you must always provide the `id` of the function call that the result corresponds to. This is important for the AI to understand the context of the result. Some LLMs, like OpenAI, will throw an error if the `id` is missing or if the `id` does not correspond to a function call.

## Inspecting a chat history object

Whenever you pass a chat history object to a chat completion service with auto function calling enabled, the chat history object will be manipulated so that it includes the function calls and results. This allows you to avoid having to manually add these messages to the chat history object and also allows you to inspect the chat history object to see the function calls and results.

You must still, however, add the final messages to the chat history object. Below is an example of how you can inspect the chat history object to see the function calls and results.

```csharp
ChatHistory chatHistory = [
    new() {
        Role = AuthorRole.User,
        Content = "Please order me a pizza"
    }
];

// Get the current length of the chat history object
int currentChatHistoryLength = chatHistory.Count;

// Get the chat message content
ChatMessageContent results = await chatCompletionService.GetChatMessageContentAsync(chatHistory, kernel: kernel);

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

## Next steps
Now that you know how to create and manage a chat history object, you can learn more about function calling in the [Function calling](./function-calling.md) topic.

> [!div class="nextstepaction"]
> [Function calling](./function-calling.md)