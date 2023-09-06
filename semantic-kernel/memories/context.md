---
title: Introduction to Context
description: Run a simple a chat application to learn about context using the Semantic Kernel.
author: molliemunoz
ms.topic: sample
ms.author: momuno
ms.date: 09/01/2023
ms.service: semantic-kernel
---

# Introduction

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

This article will introduce you to context and its importance in LLM prompts.  You will run a simple chat application and learn how to implement context with the Semantic Kernel SDK. 

# What is context?

**Context** is information that helps clarify meaning, oftentimes relating to the surrounding circumstances.

In a conversation between two people, **context** could be what was previously said or information about where the converstion is taking place. The folks conversing will likely store this knowledge in their own short-term memory, and it will help them understand the dialogue as it progresses.

An LLM also needs **context** to more clearly understand what you are telling it. Unlike humans, however, current LLMs don't immediately store this information. They are *stateless*. This means an LLM will not recall information you provide it from one exchange to the next, *unless* you provide it *again*. Therefore, a best practice is to include context in each prompt.

To demonstrate, let's look at a simple chat application.

# Chat sample application
Clone the GitHub sample code below in your preferred language. Follow the instructions in the sample README to run the chat console app.

| Language  | Sample Chat Application |
| --- | --- |
| C# | [Open solution in GitHub](tbd) |
| Python | [Open solution in GitHub](tbd) |

# Context with the Semantic Kernel SDK





