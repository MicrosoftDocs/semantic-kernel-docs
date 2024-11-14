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

::: zone pivot="programming-language-csharp"

## Using images with chat completion

The Semantic Kernel chat completion connectors support passing both images and text at the same time to a chat completion AI model.
Note that not all AI models or AI services support this behavior.

After you have constructed a chat completion service using the steps outlined in the [Chat completion](./index.md) article,
you can provide images and text in the following way.

```csharp
// Load an image from disk.
byte[] bytes = LoadImage("sample_image.jpg");

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

## Coming soon

More info coming soon.

::: zone pivot="programming-language-python"
::: zone-end

## Coming soon

More info coming soon.

::: zone pivot="programming-language-java"
::: zone-end
