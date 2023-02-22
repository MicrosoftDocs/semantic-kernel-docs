---
title: Kernel in Semantic Kernel
description: Kernel in Semantic Kernel
author: johnmaeda
ms.topic: concepts
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# What is the Kernel?

| ASK⇾ | Kernel | [Planner](/semantic-kernel/concepts-sk/Planner) | [Skills](/semantic-kernel/concepts-sk/skills)| |[Connectors](/semantic-kernel/concepts-sk/Connectors) | >>>|  ⇾GET | 
|---|---|---|---|---|---|---|---|

[!INCLUDE [fullview.md](../includes/fullview.md)]

The **kernel** in Semantic Kernel (SK) is the orchestrator of a user's ASK. The **kernel** fulfill the user's desired goal using its available [skills](/semantic-kernel/concepts-sk/skills), [memories](/semantic-kernel/concepts-sk/memories), and [connectors](/semantic-kernel/concepts-sk/connectors). Key features of the **kernel** that facilitate faster development include:

* [Skills](/semantic-kernel/concepts-sk/skills): Package your most useful prompts as fully reusable [skills](/semantic-kernel/concepts-sk/skills)
* Hybrid Development: Fluidly mix your AI prompts with conventional native code
* Orchestration: Manage complicated LLM AI prompts with complete control
* Future Proof: Use multiple LLM AI models and configurations with specificity

The **kernel** is designed to encourage "function composition" which allows developers to combine and interconnect the input and outputs of skills into a single pipeline.

## Why is the Kernel called a "kernel"?

> **Kernel**: "The core, center, or essence of an object or system." —[Wiktionary](/semantic-kernel/support/bibliography#kernel)

The term "kernel" can have different meanings in different contexts, but in the case of the Semantic Kernel, the **kernel** refers to an instance of the processing engine that processes an ASK all the way through to fulfillment. The **kernel** is the seed around which everything else in the Semantic Kernel world grows.

## How does the Kernel appear in code?

In code you will see the **kernel** instantiated with `Kernel.Build()`:

```csharp
using Microsoft.SemanticKernel;

var myKernel = Kernel.Build();
```

There are a variety of things that you can do with `myKernel` that include:

* Configuring the kernel to use OpenAI or Azure OpenAI
* Letting it know where to source a collection of [skills](/semantic-kernel/concepts-sk/skills)
* Choosing to provide [skills](/semantic-kernel/concepts-sk/skills) to it as inline expressions
* Feeding a desired [skill](/semantic-kernel/concepts-sk/skills) with inputs to drive towards output
* Pipelining multiple [skills](/semantic-kernel/concepts-sk/skills)' inputs and outputs

## Take the next step

> [!TIP]
> Try the [Simple chat summary sample app](/semantic-kernel/samples/simplechatsummary) to quickly see the **Kernel** in action.

> [!div class="nextstepaction"]
> [Learn about the Planner](/semantic-kernel/concepts-sk/planner)


[!INCLUDE [footer.md](../includes/footer.md)]
