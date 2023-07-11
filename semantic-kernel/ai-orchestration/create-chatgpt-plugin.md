---
title: How to create a plugin for ChatGPT
description: Learn how to take a Semantic Kernel plugin and expose it to ChatGPT.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/8/2023
ms.service: mssearch
---


# Expose your plugins to ChatGPT

[!INCLUDE [pat_large.md](../includes/pat_large.md)]


// TODO: state what is necessary to create a ChatGPT plugin

// TODO: state what the end result will be

| Language  | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/01-Semantic-Functions) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/01-Semantic-Functions) |


> [!Note]
> Skills are currently being renamed to plugins. This article has been updated to reflect the latest terminology, but some images and code samples may still refer to skills.

## Prerequisites

## Create HTTP endpoints for your native functions

### Create a new Azure Function

### Add your native functions to the Azure Function

### Add an OpenAPI document to your Azure Function

### Test your native functions

## Create the ChatGPT plugin manifest

### Create the manifest file

### Expose the manifest from the Azure Function

## Test your ChatGPT plugin in Semantic Kernel

dotnet add package Microsoft.SemanticKernel.Skills.OpenAPI --version 0.17.230704.3-preview

