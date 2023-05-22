---
title: What is Semantic Kernel?
description: Overview of Semantic Kernel
author: johnmaeda
ms.topic: quickstart
ms.author: johnmaeda
ms.date: 05/21/2023
ms.service: mssearch
---
# What is Semantic Kernel?


[!INCLUDE [pat_large.md](../includes/pat_large.md)]

Semantic Kernel is a lightweight open-source orchestration SDK that lets you easily mix-and-match AI prompts with conventional programming languages like C# and Python.

## Semantic Kernel simplifies development of AI apps
Semantic Kernel has been engineered to allow developers to flexibly integrate AI into their existing apps. To do so, Semantic Kernel provides a set of abstractions that make it easy to create and manage prompts, native functions, memories, and connectors. You can then orchestrate these components using Semantic Kernel pipelines to complete users' requests or automate actions.

Because of the abstractions Semantic Kernel provides, you can use it to orchestrate AI from any provider. For example, you can use Semantic Kernel to orchestrate AI from OpenAI, Azure, or even Hugging Face.

As a developer, you can then use these pieces individually. For example, if you just need an abstraction over OpenAI and Azure OpenAI services, you could use the SDK to run pre-configured prompts.

### Orchestrating AI with Semantic Kernel
The real power of Semantic Kernel, however, comes from its ability to combine these components together. By using multiple AI models, native functions, and memory all together within Semantic Kernel, you can create sophisticated pipelines that use AI to automate complex tasks.

For example, with Semantic Kernel, you could create a pipeline that helps a user send a customer email. With memory, you could retrieve information about the customer and then use GPT-4 to generate a response. Finally, you could use a native function to automatically send the response to a user's email address.

The Semantic Kernel documentation will explain how to use each of the core components so that you can orchestrate them together to create flows like the following.

![Technical perspective of what's happening](../media/flowdiagram.png)

| Step | Description |
|:-|:-|
| **Ask** | A user's goal is sent to Semantic Kernel as an ask |
| **Kernel** | [The kernel](../create-chains/kernel.md) orchestrates a user's ask |
| **Planner** | [The planner](../create-chains/planner.md) breaks it down into steps based upon resources that are available |
| **Resources** | Planning involves leveraging available [plugins](../create-plugins/index.md), [memories](../memories/index.md), and [connectors](../create-chains/connectors.md) |
| **Steps** | A plan is a series of steps for the kernel to execute |
| **Pipeline** | Executing the steps results in fulfilling the user's ask |
| **Response** | The output sent back to the user |

## Semantic Kernel is open-source
You may be familiar with the [Microsoft 365 Copilot System](https://www.youtube.com/watch?v=E5g20qmeKpg), the steps Microsoft uses to power its new Copilot experiences on top of GPT-4. This SDK formalizes patterns like these so building LLM-powered apps can be easier. To make sure all developers can take advantage of our learnings, we have released Semantic Kernel as an [open-source project](https://aka.ms/skrepo) on GitHub. 

:::image type="content" source="../media/github.png" alt-text="GitHub repo of Semantic Kernel":::

Given that new breakthroughs in LLM AIs are landing on a daily basis, you should expect this SDK evolve. We're excited to see what you build with Semantic Kernel and we look forward to your feedback and contributions so we can build the best practices together in the SDK.

> [!div class="nextstepaction"]
> [Open the Semantic Kernel repo](https://aka.ms/skrepo)

### Contribute to Semantic Kernel
We welcome contributions and suggestions from the Semantic Kernel community! One of the easiest ways to participate is to engage in discussions in the GitHub repository. Bug reports and fixes are welcome!

For new features, components, or extensions, please open an issue and discuss with us before sending a PR. This will help avoid rejections since it will allow us to discuss the impact to the larger ecosystem.

<!-- ## Semantic Kernel is one part of the entire AI ecosystem -->

## Get started using the Semantic Kernel SDK
Now that you know what Semantic Kernel is, follow the [get started](../get-started/index.md) link to try it out. Within minutes you can create prompts and chain them with out-of-the-box plugins and native code. Soon afterwards, you can give your apps memories with embeddings and summon even more power from external APIs.

> [!div class="nextstepaction"]
> [Get started with Semantic Kernel](../get-started/index.md)