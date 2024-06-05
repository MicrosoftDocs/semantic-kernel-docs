---
title: Generate images from text in Semantic Kernel
description: Learn how to generate images from text in Semantic Kernel using Hugging Face.
zone_pivot_groups: programming-languages
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---


::: zone pivot="programming-language-csharp"
# Text-to-image (Experimental)

Text-to-image is the process of generating an image from a text prompt. This is useful for generating images from AI agents, creating images for reports, and more. Today's chat completion models currently do not support text-to-image. To recreate the Dall-e experiences in ChatGPT, you can wrap a text-to-image model in a plugin so that the chat completion model can call it.


To add a text-to-image service, you can use the following code to add it to the kernel's inner service provider.

# [Hugging Face](#tab/HuggingFace)

---

If you're working directly with a service provider, you can also use the following methods.

# [Hugging Face](#tab/HuggingFace)

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Hugging Face](#tab/HuggingFace)

---

::: zone-end
