---
title: List of all sample apps
description: List of all sample apps
author: johnmaeda
ms.topic: samples
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
ms.custom: build-2023, build-2023-dataai
---
# Overview of sample apps 

[!INCLUDE [subheader.md](../includes/pat_large.md)]

Multiple learning samples are provided in the [Semantic Kernel GitHub repository](https://github.com/microsoft/semantic-kernel/tree/main/samples) to help you learn core concepts of Semantic Kernel.

## Requirements to run the apps

> [!div class="checklist"]
> * [Azure Functions Core Tools](/azure/azure-functions/functions-run-local) - used for running the kernel as a local API
> * [Yarn](https://yarnpkg.com/getting-started/install) - used for installing the app's dependencies

## Try the TypeScript/React sample apps

> [!IMPORTANT]
> [The local API service](../samples-and-solutions/local-api-service.md) must be active for the sample apps to run.

| Sample App | Illustrates |
|---|---|
| [Simple chat summary](../samples-and-solutions/simple-chat-summary.md) | Use ready-to-use [plugins](../create-plugins/out-of-the-box-plugins.md) and get those plugins into **your** app easily. _Be sure that the [local API service](../samples-and-solutions/local-api-service.md) is running for this sample to work._ |
| [Book creator](../samples-and-solutions/book-creator.md) | Use [planner](../create-chains/planner.md) to deconstruct a complex goal and envision using the planner in **your** app. _Be sure that the [local API service](../samples-and-solutions/local-api-service.md) is running for this sample to work._ |
| [Authentication and APIs](../samples-and-solutions/authentication-api.md) | Use a basic [connector](../create-chains/connectors.md) pattern to authenticate and connect to an API and imagine integrating external data into **your** app's LLM AI. _Be sure that the [local API service](../samples-and-solutions/local-api-service.md) is running for this sample to work._ |
| [GitHub Repo Q&A Bot](../samples-and-solutions/github-repo-qa-bot.md) | Use [embeddings](../memories/index.md) to store local data and functions to question the embedded data. _Be sure that the [local API service](../samples-and-solutions/local-api-service.md) is running for this sample to work._ |
| [Copilot Chat Sample App](../samples-and-solutions/copilot-chat.md) | Build your own chatbot based on the Semantic Kernel. |

## Next step

> [!div class="nextstepaction"]
> [Run the simple chat summary app](../samples-and-solutions/simple-chat-summary.md)
