---
title: Concepts Overview for LLM AI
description: Concepts Overview for LLM AI
author: johnmaeda
ms.topic: concepts
ms.author: johnmaeda
ms.date: 02/07/2023
ms.prod: semantic-kernel
---

# Learn LLM AI

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

Large Language Model (LLM) AI is a term that refers to AI models that can generate natural language texts from large amounts of data. Large language models use deep neural networks, such as transformers, to learn from billions or trillions of words, and to produce texts on any topic or domain. Large language models can also perform various natural language tasks, such as classification, summarization, translation, generation, and dialogue. Some examples of large language models are GPT-3, BERT, XLNet, T5, and EleutherAI.

Semantic Kernel builds upon four core ideas in LLM AI:

1. [Models](models)
2. [Prompts](prompts)
3. [Tokens](tokens)
4. [Embeddings](embeddings)

## Pick apart those letters "LLM" for me

Here's a simple decoder ring:

|L|L|M|
|---|---|---|
| "Large" in the case of LLM means really, really, really large — like on the scale of millions and billions. | "Language" refers to the fact that words, sentences, paragraphs — the building blocks of language — live at the core of how this kind of "semantic" AI works. | "Models" is discussed [in detail](models) but in a nutshell, a model is a high-dimensional, mathematical representation of a large amount of written information. | 

## How does LLM AI relate to ChatGPT?

The popular ChatGPT system is powered by an LLM AI model invented at OpenAI based upon the GPT-3 [model](models). You can think of ChatGPT as an application built on top of LLM AI that has been specifically tuned to engage in interactive chats. 

As a user of Semantic Kernel, you are likely to have an existing app to which you want to add LLM AI — instead of building your own ChatGPT-like system. But if you are interested in building your own chat system, we have a [code sample](../samples/simplechatsummary) that gets you going asap.

## Take the next step

> [!div class="nextstepaction"]
> [Learn about models](models)

[!INCLUDE [footer.md](../includes/footer.md)]