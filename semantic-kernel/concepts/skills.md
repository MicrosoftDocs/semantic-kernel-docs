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

![](../media/skills.png)

A **skill** refers to a domain of expertise made available to the [kernel](kernel) as a single function, or as a group of functions related to the **skill**. 

![](../media/skills01.png)

A **function** is expressed as either:

1. an LLM AI prompt
2. native computer code

Note that when using native computer code, it's also possible to call an LLM AI prompt — which means that hybrid LLM AI × native **functions** can be created. In addition, prompts can be connected end-to-end, or "chained together," to create more powerful capabilities.

## What are "prompt templates"?

## How are functions packaged in code?

With a text file "skprompt.txt" that's using SK's Prompt Template format ...

## Take the next step

Now that you know about the **kernel**, **planner**, **skills**, you can take on **memories**.

> [!div class="nextstepaction"]
> [Learn about Memories](memories.md)

## Concepts Glossary

| Semantic Kernel | | LLM AI |
|---|---|---|
| [Connectors](connectors) || [Embeddings](embeddings) |
| [Kernel](kernel) || [Models](models) |
| [Planner](planner) || [Prompts](prompts) |
| [Memories](memories) || [Tokens](tokens) |
| [Skills](skills) ||  |

[!INCLUDE [glossary.md](./includes)]