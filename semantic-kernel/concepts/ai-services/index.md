---
title: Add AI services to Semantic Kernel
description: Learn how to bring multiple AI services to your Semantic Kernel project.
zone_pivot_groups: programming-languages
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Adding AI services to Semantic Kernel

One of the main features of Semantic Kernel is its ability to add different AI services to the kernel. This allows you to easily swap out different AI services to compare their performance and to leverage the best model for your needs. In this article, we will provide sample code for adding different AI services to the kernel.

Within Semantic Kernel, there are interfaces for the most popular AI tasks. These include:

| Services                          |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| [Chat completion](#chat-completion)                    | ✅ | ✅ | ✅ |
| [Text generation](#text-generation)                    | ✅ | ✅ | ✅ |
| [Embedding generation](#embedding-generation) (Experimental)     | ✅ | ✅ | ✅ |
| [Text-to-image](#text-to-image)  (Experimental)       | ✅ | ❌ | ❌ |
| [Image-to-text](#image-to-text) (Experimental)       | ✅ | ❌ | ❌ |
| [Text-to-audio](#text-to-audio) (Experimental)       | ✅ | ❌ | ❌ | 
| [Audio-to-text](#audio-to-text) (Experimental)       | ✅ | ❌ | ❌ | 

> [!TIP]
> In most scenarios, you will only need to add chat completion to your kernel, but to support multi-modal AI, you can add any of the above services to your kernel.

The rest of this article will describe what each interface does and how to add them from different AI providers.

## Text generation

Text generation is the process of predicting the next set of words from an initial sentence.

> [!TIP]
> In _most_ cases, you will likely use a chat completion models instead of a text generation model because they're better suited to generating discrete responses. If you have autocomplete scenarios, however, text generation models can be useful.

::: zone pivot="programming-language-csharp"
To add a text generation service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Mistral](#tab/Mistral)

# [Google](#tab/Mistral)

# [Hugging Face](#tab/HuggingFace)

# [Ollama](#tab/Ollama)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Mistral](#tab/Mistral)

# [Google](#tab/Mistral)

# [Hugging Face](#tab/HuggingFace)

# [Ollama](#tab/Ollama)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Mistral](#tab/Mistral)

# [Google](#tab/Mistral)

# [Hugging Face](#tab/HuggingFace)

# [Ollama](#tab/Ollama)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

---

::: zone-end

::: zone pivot="programming-language-python"
To add a chat completion service, you can use the following code to add it to the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

---

You can also create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

---

::: zone-end

::: zone pivot="programming-language-java"

To add a chat completion service, you can use the following code to add it to the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

::: zone-end

::: zone pivot="programming-language-csharp"
## Text-to-image

Text-to-image is the process of generating an image from a text prompt. This is useful for generating images for chat bots, creating images for reports, and more. Today's chat completion models currently do not support text-to-image. To recreate the experience in ChatGPT, you can wrap a text-to-image model in a plugin so that the chat completion model can call it.


To add a text-to-image service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

::: zone-end

::: zone pivot="programming-language-csharp"
## Image-to-text

Image-to-text is the process of generating text from an image. This is useful for extracting text from images, creating captions for images, and more. Some chat completion models support images as input, so you can use them to generate text from images. Often, however, using a dedicated image-to-text can provide cheaper and more targeted results.

Text-to-image is the process of generating an image from a text prompt. This is useful for generating images for chat bots, creating images for reports, and more. Today's chat completion models currently do not support text-to-image. To recreate the experience in ChatGPT, you can wrap a text-to-image model in a plugin so that the chat completion model can call it.

To add an image-to-text service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

::: zone-end

::: zone pivot="programming-language-csharp"
## Text-to-audio

Text-to-audio is the process of generating audio from a text prompt. Today's chat completion models currently do not support text-to-audio. To recreate the experience in ChatGPT, you can post-process any of the messages generated by the model into audio.

> [!NOTE] There are several different types of text-to-audio models, including those that generate speech, music, and more. In future versions of Semantic Kernel, we plan on making this interface more specific to the type of audio you want to generate to make it easier to request the right model through dependency injection.


To add a text-to-audio service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

::: zone-end

::: zone pivot="programming-language-csharp"

## Audio-to-text

Audio-to-text is the process of generating text from an audio prompt. Today's chat completion models currently do not support audio-to-text. To recreate the experience in ChatGPT, you can generate text from a user's audio input using a dedicated audio-to-text model and then pass that text to the chat completion model.

> [!NOTE] There are several different types of audio-to-text models, including those that transcribe speech, music, and more. In future versions of Semantic Kernel, we plan on making this interface more specific to the type of audio you want to transcribe to make it easier to request the right model through dependency injection.

To add an audio-to-text service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

::: zone-end

::: zone pivot="programming-language-csharp,programming-language-python"

## Embedding generation

Embedding generation is the process of generating a vector representation of a text prompt. This is useful for retrieving text from memory stores.

> [!NOTE] As part of the enhancements coming to Semantic Kernel's memory connectors, we plan on making the embedding interface more generic to support additional embedding models.

::: zone-end

::: zone pivot="programming-language-csharp"

To add an embedding generation service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

::: zone-end

::: zone pivot="programming-language-python"

::: zone-end

## Next steps
Now that you know how to add different AI services to your Semantic Kernel project, you can learn now to add plugins to your project so that you can start automating tasks with AI.

> [!div class="nextstepaction"]
> [Learn about plugins in Semantic Kernel](./plugins.md)

