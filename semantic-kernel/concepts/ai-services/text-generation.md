---
title: Generate text in Semantic Kernel
description: Learn to use text generation to autocomplete text in Semantic Kernel.
zone_pivot_groups: programming-languages
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---


# Text generation

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