---
title: Concepts Overview for Semantic Kernel
description: Concepts Overview for Semantic Kernel
author: johnmaeda
ms.topic: concepts
ms.author: johnmaeda
ms.date: 02/07/2023
ms.prod: semantic-kernel
---

# Discover Semantic Kernel

| ASK⇾ | [Kernel](kernel) | [Planner](planner) | [Skills](skills)| |[Connectors](Connectors) | >>>|  ⇾GET | 
|---|---|---|---|---|---|---|---|

[!INCLUDE [fullview.md](../includes/fullview.md)]

Semantic Kernel (SK) builds upon these concepts:

| Concept | Short Description |
|---|---|
| [Kernel](kernel) | The kernel orchestrates a user's ASK expressed as a goal |
| [Planner](planner)| The planner breaks it down into steps based upon resources that are available |
| [Skills](skills)| Skills are customizable resources built from LLM AI [prompts](../concepts-ai/prompts) and native code |
| [Connectors](Connectors)| Connectors are customizable resources that enable external data access |

## Why is the word "semantic" used in SK?

The second "L" in LLM says it all: **Language**. Because instead of an operating kernel that's been built atop just conventional computer code, it's been built with LLM AI as the driving force for how it functions, like with the [planner](planner). A simple way to think of the shift that's happening with SK is how we're moving from syntax to semantics, or:

| |  FROM Syntax | TO Semantics |
|------------------|------------------|----------------------|
|Natural language  | The rules and patterns of how words and sentences are formed and structured. | The meaning and interpretation of words and sentences, and how they relate to the real world. |
| Programming language | The rules and symbols of how code and commands are written and structured. | The meaning and execution of code and commands, and how they affect the system state. |

## Take the next step

> [!div class="nextstepaction"]
> [Learn about the Kernel](kernel)

[!INCLUDE [footer.md](../includes/footer.md)]
