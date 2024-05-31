---
title: Convert audio to text in Semantic Kernel
description: Use models from Azure OpenAI, OpenAI, and Hugging Face to convert audio to text in Semantic Kernel.
zone_pivot_groups: programming-languages
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

::: zone pivot="programming-language-csharp"

# Audio-to-text (Experimental)

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
