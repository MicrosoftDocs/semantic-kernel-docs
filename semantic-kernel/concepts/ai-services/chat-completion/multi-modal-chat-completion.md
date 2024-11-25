---
title: Multi-modal chat completion
description: Doing chat completion with images
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 11/14/2024
ms.service: semantic-kernel
---

# Multi-modal chat completion

Many AI services support input using images, text and potentially more at the same time, allowing developers to blend together
these different inputs. This allows for scenarios such as passing an image and asking the AI model a specific question about
the image.

## Using images with chat completion

The Semantic Kernel chat completion connectors support passing both images and text at the same time to a chat completion AI model.
Note that not all AI models or AI services support this behavior.

After you have constructed a chat completion service using the steps outlined in the [Chat completion](./index.md) article,
you can provide images and text in the following way.

::: zone pivot="programming-language-csharp"

```csharp
// Load an image from disk.
byte[] bytes = File.ReadAllBytes("path/to/image.jpg");

// Create a chat history with a system message instructing
// the LLM on its required role.
var chatHistory = new ChatHistory("Your job is describing images.");

// Add a user message with both the image and a question
// about the image.
chatHistory.AddUserMessage(
[
    new TextContent("What’s in this image?"),
    new ImageContent(bytes, "image/jpeg"),
]);

// Invoke the chat completion model.
var reply = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
Console.WriteLine(reply.Content);
```

::: zone-end

::: zone pivot="programming-language-python"

```python
from semantic_kernel.contents import ChatHistory, ChatMessageContent, ImageContent, TextContent

chat_history = ChatHistory("Your job is describing images.")

# If you have an image that is accessible via a URI, you can use the following code.
chat_history.add_message(
    ChatMessageContent(
        role="user",
        items=[
            TextContent("What’s in this image?"),
            ImageContent(uri=uri),
        ]
    )
)

# If you have an image that is accessible via a local file path, you can use the following code.
chat_history.add_message(
    ChatMessageContent(
        role="user",
        items=[
            TextContent("What’s in this image?"),
            ImageContent.from_image_file(path="path/to/image.jpg"),
        ]
    )
)

# Invoke the chat completion model.
response = await chat_completion_service.get_chat_message_content(chat_history)
print(response)
```

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Multi-modal chat completion is coming soon for Java.

::: zone-end
