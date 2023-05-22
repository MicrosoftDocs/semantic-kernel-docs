---
title: Kernel in Semantic Kernel
description: Kernel in Semantic Kernel
author: johnmaeda
ms.topic: creating-chains
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# What is the Kernel?

| ASK⇾ | Kernel | [Planner](/semantic-kernel/concepts-sk/Planner) | [Skills](/semantic-kernel/concepts-sk/skills)| [Memories](/semantic-kernel/concepts-sk/memories) |[Connectors](/semantic-kernel/concepts-sk/Connectors) | >>>|  ⇾GET | 
|---|---|---|---|---|---|---|---|

[!INCLUDE [fullview.md](../includes/fullview.md)]

The _kernel_ in Semantic Kernel is the orchestrator of a user's ask. The _kernel_ fulfills the user's desired goal using its available [plugins](/semantic-kernel/concepts-sk/skills), memories, and [connectors](/semantic-kernel/concepts-sk/connectors). Key features of the _kernel_ that facilitate faster development include:

* [Plugins](/semantic-kernel/concepts-sk/skills): Package your most useful prompts as fully reusable components
* Hybrid Development: Fluidly mix your AI prompts with conventional native code
* Orchestration: Manage complicated LLM AI prompts with complete control
* Future Proof: Use multiple LLM AI models and configurations with specificity

The _kernel_ is designed to encourage "function composition" which allows developers to combine and interconnect the input and outputs of plugins into a single pipeline.

> [!TIP]
> Try the [Simple chat summary sample app](/semantic-kernel/samples/simplechatsummary) to quickly see the _Kernel_ in action.

## Why is the Kernel called a "kernel"?

> _Kernel_: "The core, center, or essence of an object or system." —[Wiktionary](/semantic-kernel/support/bibliography#kernel)

The term "kernel" can have different meanings in different contexts, but in the case of the Semantic Kernel, the _kernel_ refers to an instance of the processing engine that processes an ASK all the way through to fulfillment. The _kernel_ is the seed around which everything else in the Semantic Kernel world grows.

## How does the Kernel appear in code?

In code you will see the _kernel_ instantiated with `Kernel.Builder.Build()`:

```csharp
using Microsoft.SemanticKernel;

var myKernel = Kernel.Builder.Build();
```

There are a variety of things that you can do with `myKernel` that include:

* Configuring the _kernel_ to use OpenAI or Azure OpenAI
* Sourcing a collection of [plugins](/semantic-kernel/concepts-sk/skills)
* Chaining multiple [plugins](/semantic-kernel/concepts-sk/skills)' together
* Customize how the _kernel_ works to fit your exact needs

## Take the next step

Now that you know about the _kernel_, you are ready to learn about the _planner_.

> [!div class="nextstepaction"]
> [Learn about the Planner](/semantic-kernel/concepts-sk/planner)

[!INCLUDE [footer.md](../includes/footer.md)]
