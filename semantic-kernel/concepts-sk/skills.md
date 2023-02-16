---
title: Skills in Semantic Kernel
description: What are Skills in Semantic Kernel
author: johnmaeda
ms.topic: concepts
ms.author: johnmaeda
ms.date: 02/07/2023
ms.prod: semantic-kernel
---
# What are Skills?

| ASK⇾ | [Kernel](kernel) | [Planner](planner) | Skills |  Memories |[Connectors](Connectors) | >>>|  ⇾GET | 
|---|---|---|---|---|---|---|---|

![Journey of an ask to a get in Semantic Kernel visualized as phases as annotated immediately below](../media/fullview.png)

A **skill** refers to a domain of expertise made available to the [kernel](kernel) as a single function, or as a group of functions related to the skill* The design of SK skills has prioritized maximum flexibility for the developer to be both lightweight and extensible.  

## What is a Function?

![](../media/skills01.png)

A **function** is the basic building block for a skill. A function can be expressed as either:

1. an LLM AI [prompt](prompt) — also called a "semantic" function
2. native computer code -- also called a "native" function

When using native computer code, it's also possible to invoke an LLM AI prompt — which means that there can be **functions** that are hybrid LLM AI × native code as well. 

**Functions** can be connected end-to-end, or "chained together," to create more powerful capabilities. When they are represented as pure LLM AI prompts in semantic functions, the word "function" and "prompt" can be used interchangeably. 

## What is the relationship between semantic functions and skills?

A skill is the container in which functions live. You can think of a semantic skill as a directory of folders that contain multiple directories of semantic functions or a single directory as well.

```Skills-Are-Folders-Of-Functions
SkillName
│
└─── Function1Name
│   
└─── Function2Name
```

A function can 

## What are SK "prompt templates"?

SK Prompt templates are prompts that include variables and function calls that can dynamically change the content and the behavior of an otherwise plain prompt. Prompt templates can help you to generate more diverse, relevant, and effective prompts, and to reuse and combine them for different tasks and domains. 

## To go deeper into skills ...

There are a set of "core" skills in SK that reside in:

`semantic-kernel/dotnet/src/SemanticKernel/CoreSkills`

These skills embody a few standard capabilities like working with time, text, files, http requests, and the [Planner](planner).

For more examples of skills, and the ones that we use in our sample apps, look inside:

`semantic-kernel/samples/skills`

## Take the next step

Now that you know about the **kernel**, **planner**, **skills**, you can take on **memories**.

> [!div class="nextstepaction"]
> [Learn about Memories](memories.md)

[!INCLUDE [footer.md](../includes/footer.md)]
