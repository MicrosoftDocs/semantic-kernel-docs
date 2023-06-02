---
title: Plugins in Semantic Kernel
description: What are plugins in Semantic Kernel
author: johnmaeda
ms.topic: ai-plugins
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# What are Plugins?

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

A _plugin_ refers to a domain of expertise made available to the [kernel](/semantic-kernel/concepts-sk/kernel) as a single function, or as a group of functions related to the plugin. The design of Semantic Kernel plugins has prioritized maximum flexibility for the developer to be both lightweight and extensible.  

> [!TIP]
> Try the [Simple chat summary sample app](/semantic-kernel/samples/simplechatsummary) to quickly see **Plugins** in action.

## What is a Function?

![Diagram showing how plugins can work](/semantic-kernel/media/skillsdiagram.png)

A **function** is the basic building block for a plugin. A function can be expressed as either:

1. an LLM AI [prompt](/semantic-kernel/concepts-ai/prompts) — also called a ["semantic" function](/semantic-kernel/howto/semanticfunctions)
2. native computer code -- also called a ["native" function](/semantic-kernel/howto/nativefunctions)

When using native computer code, it's also possible to invoke an LLM AI prompt — which means that there can be functions that are hybrid LLM AI × native code as well. 

Functions can be [connected end-to-end, or "chained together,"](/semantic-kernel/howto/chainingfunctions) to create more powerful capabilities. When they are represented as pure LLM AI prompts in semantic functions, the word "function" and "prompt" can be used interchangeably. 

## What is the relationship between semantic functions and plugins?

A plugin is the container in which functions live. You can think of a semantic plugin as a directory of folders that contain multiple directories of semantic functions or a single directory as well.

```Semantic-Plugins-Are-Folders-Of-Functions
PluginName (directory name)
│
└─── Function1Name (directory name)
│   
└─── Function2Name (directory name)
```

Each function directory will have an `skprompt.txt` file and a `config.json` file. There's much more to learn about semantic functions in [Building Semantic Functions](/semantic-kernel/howto/semanticfunctions) if you wish to go deeper.

## What is the relationship between native functions and plugins?

Native functions are loosely inspired by Azure Functions and exist as individual native plugin files as in `MyNativePlugin.cs` below:

```Your-App-And-Plugins
MyAppSource
│
└───MyPluginsDirectory
    │
    └─── MySemanticPlugin (a directory)
    |   │
    |   └─── MyFirstSemanticFunction (a directory)
    |   └─── MyOtherSemanticFunction (a directory)
    │
    └─── MyNativePlugin.cs (a file)
    └─── MyOtherNativePlugin.cs (a file)
```

Each file will contain multiple native functions that are associated with a plugin.

## Where to find plugins in the GitHub repo

Plugins are stored in one of three places:

1. Core Plugins: these are plugins available at any time to the kernel that embody a few standard capabilities like working with time, text, files, http requests, and the [Planner](/semantic-kernel/concepts-sk/planner).

> `semantic-kernel/dotnet/src/SemanticKernel/CoreSkills`

2. Semantic Plugins: these plugins are managed by you in a directory of your choice.

3. Native Plugins: these plugins are also managed by you in a directory of your choice.

For more examples of plugins, and the ones that we use in our sample apps, look inside:

> `semantic-kernel/samples/skills`

## Take the next step

> [!div class="nextstepaction"]
> [Learn about out-of-the-box plugins](./out-of-the-box-plugins.md)

