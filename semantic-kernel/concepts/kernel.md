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

The **kernel** in Semantic Kernel is the orchestrator of a user's Ask. It fulfill the user's desired goal using its available skills, memories, and connectors. Key features of the **kernel** that can make a developer faster include:

* Integrating native code with prompts
* Packaging of useful prompts as reusable skills
* Packaging of hybrid prompts as reusable skills
* Orchestrating complicated LLM AI prompts
* Using multiple LLM AI models and configurations

## Get familiar with SK's concepts

Kernel | [Skills](skills) | [Planner](planner) | Memories | Embeddings | Connectors

## History of the Kernel

We began constructing Semantic Kernel in 2022 under the leadership of Sam Schillace, Deputy CTO at Microsoft. To quote Sam's early motivation for advancing the SK project:

>Advanced LLMs and other AI models are powerful but are limited – without ongoing memory, or the ability to interact with outside code, they are essentially purely stochastic functions (not idempotent but without side effects). In order to get beyond "dancing bear" demonstrations and one-offs, it's important to build more robust capabilities: flow control and memory.

The **kernel** is the result of multiple engineering iterations — resulting in a lightweight design built in the C# language. If you are not a C# developer, then no worries! Our [sample code](../samples/samplelist) demonstrates how to utilize the kernel from examples written in TypeScript, to start.