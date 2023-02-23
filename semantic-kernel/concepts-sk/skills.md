---
title: Skills in Semantic Kernel
description: What are Skills in Semantic Kernel
author: johnmaeda
ms.topic: concepts
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# What are Skills?

| ASK⇾ | [Kernel](/semantic-kernel/concepts-sk/kernel) | [Planner](/semantic-kernel/concepts-sk/planner) | Skills |  |[Connectors](/semantic-kernel/concepts-sk/Connectors) | >>>|  ⇾GET | 
|---|---|---|---|---|---|---|---|

[!INCLUDE [fullview.md](../includes/fullview.md)]

A _skill_ refers to a domain of expertise made available to the [kernel](/semantic-kernel/concepts-sk/kernel) as a single function, or as a group of functions related to the skill* The design of SK skills has prioritized maximum flexibility for the developer to be both lightweight and extensible.  

> [!TIP]
> Try the [Simple chat summary sample app](/semantic-kernel/samples/simplechatsummary) to quickly see **Skills** in action.

## What is a Function?

![Diagram showing how skills can work](/semantic-kernel/media/skillsdiagram.png)

A **function** is the basic building block for a skill. A function can be expressed as either:

1. an LLM AI [prompt](/semantic-kernel/concepts-ai/prompt) — also called a ["semantic" function](/semantic-kernel/concepts-sk//howto/semanticfunctions)
2. native computer code -- also called a ["native" function](/semantic-kernel/concepts-sk//howto/nativefunctions)

When using native computer code, it's also possible to invoke an LLM AI prompt — which means that there can be functions that are hybrid LLM AI × native code as well. 

Functions can be [connected end-to-end, or "chained together,"](/semantic-kernel/concepts-sk//howto/chainingfunctions) to create more powerful capabilities. When they are represented as pure LLM AI prompts in semantic functions, the word "function" and "prompt" can be used interchangeably. 

## What is the relationship between semantic functions and skills?

A skill is the container in which functions live. You can think of a semantic skill as a directory of folders that contain multiple directories of semantic functions or a single directory as well.

```Semantic-Skills-Are-Folders-Of-Functions
SkillName (directory name)
│
└─── Function1Name (directory name)
│   
└─── Function2Name (directory name)
```

Each function directory will have an `skprompt.txt` file and a `config.json` file. There's much more to learn about semantic functions in [Building Semantic Functions](/semantic-kernel/howto/semanticfunctions) if you wish to go deeper.

## What is the relationship between native functions and skills?

Native functions are loosely inspired by Azure Functions and exist as individual native skill files as in `MyNativeSkill.cs` below:

```Your-App-And-Skills
MyAppSource
│
└───MySkillsDirectory
    │
    └─── MySemanticSkill (a directory)
    |   │
    |   └─── MyFirstSemanticFunction (a directory)
    |   └─── MyOtherSemanticFunction (a directory)
    │
    └─── MyNativeSkill.cs (a file)
    └─── MyOtherNativeSkill.cs (a file)
```

Each file will contain multiple native functions that are associated with a skill.

## Where to find skills in the GitHub repo

Skills are stored in one of three places:

1. Core Skills: these are skills available at any time to the kernel that embody a few standard capabilities like working with time, text, files, http requests, and the [Planner](/semantic-kernel/concepts-sk/planner).

> `semantic-kernel/dotnet/src/SemanticKernel/CoreSkills`

2. Semantic Skills: these skills are managed by you in a directory of your choice.

3. Native Skills: these skills are also managed by you in a directory of your choice.

For more examples of skills, and the ones that we use in our sample apps, look inside:

> `semantic-kernel/samples/skills`

## Take the next step

Now that you know about the _kernel_, _planner_, _skills_, you can take on _connectors.__

> [!div class="nextstepaction"]
> [Learn about Connectors](/semantic-kernel/concepts-sk/Connectors)


[!INCLUDE [footer.md](../includes/footer.md)]
