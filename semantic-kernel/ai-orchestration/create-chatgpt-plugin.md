---
title: How to create a plugin for ChatGPT with Semantic Kernel
description: Learn how to take a Semantic Kernel plugin and expose it to ChatGPT with Azure Functions.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/8/2023
ms.service: mssearch
---


# Expose your plugins to ChatGPT and Bing

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

So far, we've demonstrated how to create plugins that can be used natively in Semantic Kernel. This is great if you are building a custom application that only uses Semantic Kernel, but what if you want to use your plugins in applications that _don't_ use Semantic Kernel, like ChatGPT, or Bing?

In this article, we'll show you how to take a Semantic Kernel plugin and expose it to ChatGPT with Azure Functions. As an example, we'll demonstrate how to transform the `MathPlugin` we created in previous articles into a ChatGPT plugin.

Once we're done, you'll have an Azure Function that exposes each of your plugin's native functions as HTTP endpoints so they can be used by Semantic Kernel _or_ ChatGPT. If you want to see the final solution, you can check out the sample in the public documentation repository.


| Language  | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/05-Create-ChatGPT-Plugin) |
| Python | _Coming soon_ |

## Prerequisites
To complete this tutorial, you'll need the following:
- [Azure Functions Core Tools](https://github.com/Azure/azure-functions-core-tools)

You do _not_ need to have access to OpenAI's plugin preview to complete this tutorial. If you do have access, however, you can upload your final plugin to OpenAI and use it in ChatGPT.


## How are ChatGPT plugins different?
In short, they aren't! In the [plugin article](./plugins#what-is-a-plugin) we described how all plugins are moving towards the common standard defined by OpenAI. This standard uses a plugin manifest file that points to an accompanying [OpenAPI specification](https://swagger.io/specification/). Plugins defined in this way can then be used by any application that supports the OpenAI specification, including Semantic Kernel and ChatGPT.

> [!Note]
> OpenAPI is different than OpenAI. OpenAPI is a specification for describing REST APIs, while OpenAI is a company that develops AI models and APIs. While the two are not related, OpenAI has adopted the OpenAPI specification for describing plugin APIs.

So far, however, we've only shown how to create plugins that are _natively_ loaded into Semantic Kernel instead of being exposed through an OpenAPI specification. This has helped us demonstrate the core concepts of plugins without adding the additional complexity of standing up an HTTP endpoint. With minimal changes, however, we can take the plugins we've already created and expose them to ChatGPT.

### Transforming `MathPlugin` into a ChatGPT plugin

:::row:::
   :::column span="2":::
        At a high level, there are three steps required to take our existing `MathPlugin` and turn it into a ChatGPT plugin:
        1. Create HTTP endpoints for each of the plugin's native functions.
        2. Create and expose a ChatGPT plugin manifest and OpenAPI specification file for the plugin.
        3. Test the ChatGPT plugin by importing the manifest into Semantic Kernel.
   :::column-end:::
   :::column span="3":::
      ![The Math plugin, before and after ](../media/plugin-before-and-after.png)
   :::column-end:::
:::row-end:::

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

