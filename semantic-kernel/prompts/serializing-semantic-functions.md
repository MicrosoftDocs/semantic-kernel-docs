---
title: Save your prompts
description: Learn how to serialize your prompts in Semantic Kernel.
author: matthewbolanos
ms.topic: tutorial
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Serializing prompts


In previous articles, we demonstrated [create and run prompts inline](./templatizing-prompts.md). However, in most cases, you'll want to create your prompts in a separate file so you can easily import them into Semantic Kernel across multiple projects.

In this article, we'll demonstrate how to create the files necessary for a prompt so you can easily import them into Semantic Kernel. As an example in this article, we will build on the [previous tutorial](./templatizing-prompts.md) by showing how to create a prompt that gathers the intent of the user. This prompt will be called `getIntent`.

If you want to see the final solution, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/05-Nested-Functions-In-Prompts) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/06-Serializing-Prompts) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/05-Nested-Functions-In-Prompts) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/06-Serializing-Prompts) |


## Creating a home for your prompts
Before creating the files for the `getIntent` function, you must first define a folder that will hold all of your plugins. This will make it easier to import them into Semantic Kernel later. We recommend putting this folder at the root of your project and calling it _prompts_.

Within your _prompts_ folder, you can create a nested folder called _getIntent_ for your function.

```directory
Prompts
│
└─── getIntent
```

## Creating the files for your prompt
Once inside of a prompts folder, you'll need to create two new files: _skprompt.txt_ and _config.json_. The _skprompt.txt_ file contains the prompt that will be sent to the AI service and the _config.json_ file contains the configuration along with semantic descriptions that can be used by planners.

Go ahead and create these two files in the _getIntent_ folder.

```directory
Plugins
│
└─── getIntent
     |
     └─── config.json
     └─── skprompt.txt
```

### Writing a prompt in the _skprompt.txt_ file
The _skprompt.txt_ file contains the request that will be sent to the AI service. Since we've already written the prompt in the [previous tutorial](./inline-semantic-functions.md#defining-the-prompt), we can simply copy it over to the _skprompt.txt_ file.

:::code language="txt" source="~/../samples/dotnet/06-Serializing-Prompts/plugins/OrchestratorPlugin/getIntent/skprompt.txt":::


### Configuring the function in the _config.json_ file
Next, we need to define the configuration for the `getIntent` function. When serializing the configuration, all you need to do is define the same properties in a JSON file:

- `type` – The type of prompt. In this case, we're using the `completion` type.
- `description` – A description of what the prompt does. This is used by planner to automatically orchestrate plans with the function.
- `completion` – The settings for completion models. For OpenAI models, this includes the `max_tokens` and `temperature` properties.
- `input` – Defines the variables that are used inside of the prompt (e.g., `input`).

For the `getIntent` function, we can use the same configuration [as before](./inline-semantic-functions.md#configuring-the-function).

:::code language="json" source="~/../samples/dotnet/06-Serializing-Prompts/plugins/OrchestratorPlugin/getIntent/config.json":::

### Testing your prompt
At this point, you can import and test your function with the kernel by updating your _Program.cs_ or _main.py_ file to the following.

# [C#](#tab/Csharp)
:::code language="csharp" source="~/../samples/dotnet/06-Serializing-Prompts/Program.cs" range="3-5,14-19,22-36":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/06-Serializing-Prompts/main.py" range="1,3-10,12-33":::

---

You should get an output that looks like the following:

```output
Send congratulatory email.
```


## Take the next step
Now that you can create a prompt, you can now learn how to templatize your prompt so
that it can be used for even more scenarios.

> [!div class="nextstepaction"]
> [Templatize your prompt](./templatizing-semantic-functions.md)