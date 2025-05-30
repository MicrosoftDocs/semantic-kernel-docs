---
title: Contextual Function Selection with Semantic Kernel Agents
description: An overview of contextual function selection using RAG to dynamically choose the most relevant functions for agent interactions.
zone_pivot_groups: programming-languages
author: sergeymenshykh
ms.topic: conceptual
ms.author: semenshi
ms.date: 12/30/2024
ms.service: semantic-kernel
---

# Contextual Function Selection with Agents

> [!IMPORTANT]
> This feature is in the experimental stage. Features at this stage are under active development and may change significantly before advancing to the preview or release candidate stage.
## Overview

Contextual Function Selection is an advanced capability in the Semantic Kernel Agent Framework that enables agents to dynamically select the most relevant functions based on the current conversation context. Instead of advertising all available functions to the AI model, this feature uses Retrieval-Augmented Generation (RAG) to intelligently filter and present only the functions that are most pertinent to the current interaction.

This approach addresses the challenge of function selection when dealing with large numbers of available functions, where AI models may struggle to choose the appropriate function, leading to confusion and suboptimal performance.