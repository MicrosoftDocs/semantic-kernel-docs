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

Multiple learning samples are provided in the [Semantic Kernel GitHub repository](https://github.com/microsoft/semantic-kernel/tree/main/samples) to help you learn core concepts of Semantic Kernel.

## Requirements to run the apps

> [!div class="checklist"]
> * [Azure Functions Core Tools](/azure/azure-functions/functions-run-local) - used for running the kernel as a local API
> * [Yarn](https://yarnpkg.com/getting-started/install) - used for installing the app's dependencies

## Try the TypeScript/React sample apps

> [!IMPORTANT]
> [The local API service](/semantic-kernel/samples-and-solutions/local-api-service) must be active for the sample apps to run.

| Sample App | Illustrates |
|---|---|
| [Simple chat summary](/semantic-kernel/samples-and-solutions/simple-chat-summary) | Use ready-to-use [plugins](/semantic-kernel/create-plugins/out-of-the-box-plugins) and get those plugins into **your** app easily. _Be sure that the [local API service](/semantic-kernel/samples-and-solutions/local-api-service) is running for this sample to work._ |
| [Book creator](/semantic-kernel/samples-and-solutions/book-creator) | Use [planner](/semantic-kernel/create-chains/planner) to deconstruct a complex goal and envision using the planner in **your** app. _Be sure that the [local API service](/semantic-kernel/samples-and-solutions/local-api-service) is running for this sample to work._ |
| [Authentication and APIs](/semantic-kernel/samples-and-solutions/authentication-api) | Use a basic [connector](/semantic-kernel/create-chains/connectors) pattern to authenticate and connect to an API and imagine integrating external data into **your** app's LLM AI. _Be sure that the [local API service](/semantic-kernel/samples-and-solutions/local-api-service) is running for this sample to work._ |
| [GitHub Repo Q&A Bot](/semantic-kernel/samples-and-solutions/github-repo-qa-bot) | Use [embeddings](/semantic-kernel/memories/index) to store local data and functions to question the embedded data. _Be sure that the [local API service](/semantic-kernel/samples-and-solutions/local-api-service) is running for this sample to work._ |
| [Copilot Chat Sample App](/semantic-kernel/samples-and-solutions/copilot-chat) | Build your own chatbot based on the Semantic Kernel. |

## Next step

> [!div class="nextstepaction"]
> [Run the simple chat summary app](/semantic-kernel/samples-and-solutions/simple-chat-summary)

[!INCLUDE [footer.md](../includes/footer.md)]
