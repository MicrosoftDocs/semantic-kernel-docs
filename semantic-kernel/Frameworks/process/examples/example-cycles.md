---
title: How-To&colon; Using Cycles
description: A step-by-step walk-through for using Cycles
zone_pivot_groups: programming-languages
author: bentho
ms.topic: tutorial
ms.author: bentho
ms.date: 01/13/2025
ms.service: semantic-kernel
---
# How-To: Using Cycles

> [!WARNING]
> The _Semantic Kernel Process Framework_ is experimental, still in development and is subject to change.

## Overview

In the previous section we build a simple Process to help us automate the creation of documentation for our now product. In this section we will improve on that process by adding a quality assurance step. This step will use and LLM to grade the generated documentation and provide recommended changes as well as a pass/fail grade. By taking advantage of the Process Frameworks' support for cycles, we can go one step further and automatically apply the recommended changes (if any) and then start the cycle over, repeating this until the content meets our quality bar. 