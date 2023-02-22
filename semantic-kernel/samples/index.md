---
title: List of all sample apps
description: List of all sample apps
author: johnmaeda
ms.topic: samples
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# Overview of sample apps

[!INCLUDE [subheader.md](../includes/pat_medium.md)]

Multiple learning samples are provided in the [Semantic Kernel GitHub repository](/semantic-kernel/getting-started/setup) to help you learn core concepts of Semantic Kernel.

## Requirements to run the sample apps

> [!div class="checklist"]
> * [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local) - used for running the kernel as a local API
> * [Yarn](https://yarnpkg.com/getting-started/install) - used installing dependencies of the React sample apps and running the sample apps

## Try the TypeScript/React sample apps

> [!IMPORTANT]
> [The local API service](/semantic-kernel/samples/localapiservice) must be active for the sample apps to run.

| Sample App | Illustrates |
|---|---|
| [Simple chat summary](/semantic-kernel/samples/simplechatsummary) | Use ready-to-use [skills](/semantic-kernel/concepts-sk/skills) and get those skills into **your** app easily. Be sure that the [local API service](/semantic-kernel/samples/localapiservice) is running for this sample to work. |
| [Book creator](/semantic-kernel/samples/bookcreator) | Use [planner](/semantic-kernel/concepts-sk/planner) to deconstruct a complex goal and envision using the planner in **your** app. Be sure that the [local API service](/semantic-kernel/samples/localapiservice) is running for this sample to work. |
| [Authentication and APIs](/semantic-kernel/samples/authapi) | Use a basic [connector](/semantic-kernel/concepts-sk/connectors) pattern to authenticate and connect to an API and imagine integrating external data into **your** app's LLM AI. Be sure that the [local API service](/semantic-kernel/samples/localapiservice) is running for this sample to work. |


## Take the next step

> [!div class="nextstepaction"]
> [Run the simple chat summary app](/semantic-kernel/samples/simplechatsummary)

[!INCLUDE [footer.md](../includes/footer.md)]
