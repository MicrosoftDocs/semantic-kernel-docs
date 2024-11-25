---
title: Add AI services to Semantic Kernel
description: Learn how to bring multiple AI services to your Semantic Kernel project.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Adding AI services to Semantic Kernel

One of the main features of Semantic Kernel is its ability to add different AI services to the kernel. This allows you to easily swap out different AI services to compare their performance and to leverage the best model for your needs. In this section, we will provide sample code for adding different AI services to the kernel.

Within Semantic Kernel, there are interfaces for the most popular AI tasks. In the table below, you can see the services that are supported by each of the SDKs.

| Services                          |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| [Chat completion](./chat-completion/index.md)                    | ✅ | ✅ | ✅ |
| Text generation                  | ✅ | ✅ | ✅ |
| Embedding generation (Experimental)     | ✅ | ✅ | ✅ |
| Text-to-image  (Experimental)       | ✅ | ✅ | ❌ |
| Image-to-text (Experimental)       | ✅ | ❌ | ❌ |
| Text-to-audio (Experimental)       | ✅ | ✅ | ❌ | 
| Audio-to-text (Experimental)       | ✅ | ✅ | ❌ | 

> [!TIP]
> In most scenarios, you will only need to add chat completion to your kernel, but to support multi-modal AI, you can add any of the above services to your kernel.

## Next steps
To learn more about each of the services, please refer to the specific articles for each service type. In each of the articles we provide sample code for adding the service to the kernel across multiple AI service providers.

> [!div class="nextstepaction"]
> [Learn about chat completion](./chat-completion/index.md)