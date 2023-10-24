---
title: Save your semantic functions
description: Learn how to serialize your semantic functions in Semantic Kernel.
author: matthewbolanos
ms.topic: tutorial
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Serializing semantic functions

[!INCLUDE [pat_large.md](../../../includes/pat_large.md)]

In previous articles, we demonstrated [how to load a semantic function inline](./inline-semantic-functions.md). However, in most cases, you'll want to create your semantic functions in a separate file so you can easily import them into Semantic Kernel across multiple projects.

In this article, we'll demonstrate how to create the files necessary for a semantic function so you can easily import them into Semantic Kernel. As an example in this article, we will build on the [previous tutorial](./inline-semantic-functions.md) by showing how to create a semantic function that gathers the intent of the user. This semantic function will be called `GetIntent` and will be part of a plugin called `OrchestratorPlugin`.

If you want to see the final solution, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/03-Inline-Semantic-Functions) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/04-Serializing-Semantic-Functions) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/03-Inline-Semantic-Functions) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/04-Serializing-Semantic-Functions) |


> [!TIP]
> We recommend using the [Semantic Kernel Tools](../../../vs-code-tools/index.md) extension for Visual Studio Code to help you create semantic functions. This extension provides an easy way to create and test functions directly from within VS Code.

## Creating a home for your semantic functions
Before creating the files for the `GetIntent` function, you must first define a folder that will hold all of your plugins. This will make it easier to import them into Semantic Kernel later. We recommend putting this folder at the root of your project and calling it _plugins_.

Within your _plugins_ folder, you can then create a folder called _OrchestratorPlugin_ for your plugin and a nested folder called _GetIntent_ for your function.

```directory
Plugins
│
└─── OrchestratorPlugin
     |
     └─── GetIntent
```

To see a more complete example of a plugins directory, check out the [Semantic Kernel sample plugins](https://github.com/microsoft/semantic-kernel/tree/main/samples/plugins) folder in the GitHub repository.

## Creating the files for your semantic function
Once inside of a semantic functions folder, you'll need to create two new files: _skprompt.txt_ and _config.json_. The _skprompt.txt_ file contains the prompt that will be sent to the AI service and the _config.json_ file contains the configuration along with semantic descriptions that can be used by planners.

Go ahead and create these two files in the _GetIntent_ folder.

```directory
Plugins
│
└─── OrchestratorPlugin
     |
     └─── GetIntent
          |
          └─── config.json
          └─── skprompt.txt
```

### Writing a prompt in the _skprompt.txt_ file
The _skprompt.txt_ file contains the request that will be sent to the AI service. Since we've already written the prompt in the [previous tutorial](./inline-semantic-functions.md#defining-the-prompt), we can simply copy it over to the _skprompt.txt_ file.

:::code language="txt" source="~/../samples/dotnet/04-Serializing-Semantic-Functions/plugins/OrchestratorPlugin/GetIntent/skprompt.txt":::


### Configuring the function in the _config.json_ file
Next, we need to define the configuration for the `GetIntent` function. When serializing the configuration, all you need to do is define the same properties in a JSON file:

- `type` – The type of prompt. In this case, we're using the `completion` type.
- `description` – A description of what the prompt does. This is used by planner to automatically orchestrate plans with the function.
- `completion` – The settings for completion models. For OpenAI models, this includes the `max_tokens` and `temperature` properties.
- `input` – Defines the variables that are used inside of the prompt (e.g., `input`).

For the `GetIntent` function, we can use the same configuration [as before](./inline-semantic-functions.md#configuring-the-function).

:::code language="json" source="~/../samples/dotnet/04-Serializing-Semantic-Functions/plugins/OrchestratorPlugin/GetIntent/config.json":::

### Testing your semantic function
At this point, you can import and test your function with the kernel by updating your _Program.cs_ or _main.py_ file to the following.

# [C#](#tab/Csharp)
:::code language="csharp" source="~/../samples/dotnet/04-Serializing-Semantic-Functions/Program.cs" range="3-5,14-19,22-36":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/04-Serializing-Semantic-Functions/main.py" range="1,3-10,12-33":::

---

You should get an output that looks like the following:

```output
Send congratulatory email.
```


## Take the next step
Now that you can create a semantic function, you can now learn how to templatize your prompt so
that it can be used for even more scenarios.

> [!div class="nextstepaction"]
> [Templatize your semantic function](./templatizing-semantic-functions.md)