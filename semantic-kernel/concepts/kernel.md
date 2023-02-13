---
title: Kernel in Semantic Kernel
description: Kernel in Semantic Kernel
author: johnmaeda
ms.topic: concepts
ms.author: johnmaeda
ms.date: 02/07/2023
ms.prod: semantic-kernel
---
# What is the Kernel?

![](../media/kernelsm.png)

The **kernel** in Semantic Kernel (SK) is the orchestrator of a user's ASK. The **kernel** fulfill the user's desired goal using its available [skills](skills), [memories](memories), and [connectors](connectors). Key features of the **kernel** that facilitate faster development include:

* [Skills](skills): Package your most useful prompts as fully reusable [skills](skills)
* Hybrid Development: Mix your AI prompts with conventional native code fluidly
* Orchestration: Manage complicated LLM AI prompts with complete control
* Future Proof: Use multiple LLM AI models and configurations with specificity

The **kernel** is designed to encourage "function composition" which allows developers to combine and interconnect the input and outputs of skills into a single pipeline.

## Why is the Kernel called a "kernel"?

> **Kernel**: "The core, center, or essence of an object or system." —[Wiktionary](https://en.wiktionary.org/wiki/kernel)

The term "kernel" can have different meanings in different contexts, but in the case of the Semantic Kernel, the **kernel** refers to an instance of the processing engine that processes an ASK all the way through to fulfillment. The **kernel** is the seed around which everything else in the Semantic Kernel world grows.

## How does the Kernel appear in code?

In code you will see the **kernel** instantiated as `ISemanticKernel`:

```csharp
// Simple instance
ISemanticKernel my_kernel = SemanticKernel.Build();
```

There are a variety of things that you can do with `my_kernel` that include:

* Configuring the kernel to use OpenAI or Azure OpenAI
* Letting it know where to source a collection of [skills](skills)
* Choosing to provide [skills] to it as inline expressions
* Feeding a desired skill with inputs to drive towards output

## Take the next step

Now that you know a bit about the **kernel** you will definitely want to learn about the **planner**.

> [!div class="nextstepaction"]
> [Learn about Planner](planner.md)

## Concepts Glossary

| Semantic Kernel | | LLM AI |
|---|---|---|
| [Connectors](connectors) || [Embeddings](embeddings) |
| [Kernel](kernel) || [Models](models) |
| [Planner](planner) || [Prompts](prompts) |
| [Memories](memories) || [Tokens](tokens) |
| [Skills](skills) ||  |

[!INCLUDE [glossary.md](./includes)]
