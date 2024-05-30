---
title: Plugins in Semantic Kernel
description: Learn how to use AI plugins in Semantic Kernel
author: sophialagerkranspandey
ms.topic: conceptual
ms.author: sopand
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# What is a Plugin?

:::row:::
   :::column span="1":::
    Plugins are a key component of Semantic Kernel. If you have already used plugins from ChatGPT or Copilot extensions in Microsoft 365, you’re already familiar with them. With plugins, you can encapsulate your existing APIs into a collection that can be used by an AI. This allows you to give your AI the ability to perform actions that it wouldn’t be able to do otherwise.

    Behind the scenes, Semantic Kernel leverages [function calling](https://platform.openai.com/docs/guides/function-calling), a native feature of most of the latest LLMs to allow LLMs to invoke your APIs. With function calling, LLMs can request (i.e., call) a particular function. Semantic Kernel can then marshal the request to the appropriate function in your codebase and return the results back to the LLM so the LLM can generate a final response.
   :::column-end:::
   :::column span="1":::
        ![Semantic Kernel Plugin](../media/Designed-for-modular-extensibility.png)
   :::column-end:::
:::row-end:::

Not all AI SDKs have an analogous concept to plugins (most just have functions or tools). In enterprise scenarios, however, plugins are valuable because they encapsulate a set of functionality that mirrors how enterprise developers already develop services and APIs. Plugins also play nicely with dependency injection. Within a plugin's constructor, you can inject services that are necessary to perform the work of the plugin (e.g., database connections, HTTP clients, etc.). This is difficult to accomplish with other SDKs that lack plugins.

## Anatomy of a plugin
At a high-level, a plugin is a group of [functions](#types-of-plugin-functions) that can be exposed to AI apps and services. The functions within plugins can then be orchestrated by an AI application to accomplish user requests. Within Semantic Kernel, you can invoke these functions either manually or automatically with function calling or planners.

> [!Tip]
> In other platforms, functions are often referred to as "tools" or "actions". In Semantic Kernel, we use the term "functions" since they are typically defined as native functions in your codebase.

Just providing functions, however, is not enough to make a plugin. To power automatic orchestration with function calling, plugins also need to provide details that semantically describe how they behave. Everything from the function's input, output, and side effects need to be described in a way that the AI can understand, otherwise, the AI will not correctly call the function.


For example, the sample `WriterPlugin` plugin on the right has functions with semantic descriptions that describe what each function does. An LLM can then use these descriptions to choose the best functions to call to fulfill a user's ask.

:::row:::
   :::column span="1":::
      
      In the picture on the right, an LLM would likely call the `ShortPoem` and `StoryGen` functions to satisfy the users ask thanks to the provided semantic descriptions.
   :::column-end:::
   :::column span="3":::
        ![Semantic description within the WriterPlugin plugin](../media/writer-plugin-example.png)
   :::column-end:::
:::row-end:::

## Types of plugin functions
Most plugin functions fall into one of two categories:
1. [Data retrieval](#data-retrieval) (for RAG)
2. [Task automation](#task-automation)

### Data retrieval

These functions are used to retrieve data from a database or external API so that an AI can gather additional context to generate a response. This is also known as Retrieval Augmented Generation (RAG). Examples include the following.

| Plugin | Description |
|--------|-------------|
| Web search | Allows an AI to search the web for current information that might not exist in its training data. |
| Time | Gives an AI the ability to see the current time so it can provide time-sensitive information. |
| CRM | Allows an AI to retrieve information about customers. |
| Inventory | Gives an AI the ability to see what is in stock so it can give recommendations to employees and customers alike. |
| Semantic search | Allows an AI to search for information within a specific domain (e.g., internal legal documents); typically powered by a vector DB like Azure AI Search. |

> [!Tip]
> When developing plugins for Retrieval Augmented Generation (RAG), it’s important to note that you don't always need a vector DB. Often your existing APIs can be used by an AI to retrieve the necessary information. We recommend starting with your existing APIs and then moving to a vector DB for semantic search if necessary.

### Task automation

These functions are used to perform actions on behalf of a user. These are helpful when you want to automate business processes with AI.
Examples include the following.

| Plugin | Description |
|--------|-------------|
| Email | Allows an AI to send and receive emails on behalf of a user. |
| Microsoft 365 | Allows an AI to use Microsoft 365 applications, such as Microsoft Word so that it can edit documents on behalf of a user. |
| Home automation | Allows an AI to control smart home devices like lights, thermostats, and security cameras. |
| Inventory | Allows an AI to update inventory levels based on sales or new shipments. |

> [!Tip]
> When developing plugins for task automation, it’s important to ensure that you have the necessary safeguards in place to ensure the AI cannot perform actions without user consent. To learn more about building approval workflows, refer to the [hooks and filters article](./hooks-and-filters.md) article.

## Using plugins with Semantic Kernel
