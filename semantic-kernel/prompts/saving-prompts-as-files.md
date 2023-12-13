---
title: Saving your prompts as files
description: Learn how to serialize your prompts in Semantic Kernel.
author: matthewbolanos
ms.topic: tutorial
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Saving and sharing prompts

In previous articles, we demonstrated how to [create and run prompts inline](./templatizing-prompts.md). However, in most cases, you'll want to create your prompts in a separate file so you can easily import them into Semantic Kernel across multiple projects and share them with others.

In this article, we'll demonstrate how to create the files necessary for a prompt so you can easily import them into Semantic Kernel. As an example in this article, we will build on the [previous tutorial](./templatizing-prompts.md) by showing how to serialize the chat prompt. This prompt will be called `chat`.

If you want to see the final solution, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/05-Nested-Functions-In-Prompts) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/07-Serializing-Prompts) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/05-Nested-Functions-In-Prompts) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/07-Serializing-Prompts) |


## Creating a home for your prompts
Before creating the files for the `chat` function, you must first define a folder that will hold all of your plugins. This will make it easier to import them into Semantic Kernel later. We recommend putting this folder at the root of your project and calling it _Prompts_.

Within your _Prompts_ folder, you can create a nested folder called _chat_ for your function.

```directory
Prompts
│
└─── chat
```

## Creating the files for your prompt
Once inside of a prompts folder, you'll need to create two new files: _skprompt.txt_ and _config.json_. The _skprompt.txt_ file contains the prompt that will be sent to the AI service and the _config.json_ file contains the configuration along with semantic descriptions that can be used by planners.

Go ahead and create these two files in the _chat_ folder.

```directory
Prompts
│
└─── chat
     |
     └─── config.json
     └─── skprompt.txt
```

### Writing a prompt in the _skprompt.txt_ file
The _skprompt.txt_ file contains the request that will be sent to the AI service. Since we've already written the prompt in the [previous tutorial](./templatizing-prompts.md), we can simply copy it over to the _skprompt.txt_ file.

:::code language="txt" source="~/../samples/dotnet/07-Serializing-Prompts/prompts/chat/skprompt.txt":::


### Configuring the function in the _config.json_ file
Next, we need to define the configuration for the `chat` function. When serializing the configuration, all you need to do is define the same properties in a JSON file:

- `type` – The type of prompt. In this case, we're using the `completion` type.
- `description` – A description of what the prompt does. This is used by planner to automatically orchestrate plans with the function.
- `completion` – The settings for completion models. For OpenAI models, this includes the `max_tokens` and `temperature` properties.
- `input` – Defines the variables that are used inside of the prompt (e.g., `input`).

For the `chat` function, we can use the same configuration [as before](./configure-prompts.md).


# [C#](#tab/Csharp)
:::code language="json" source="~/../samples/dotnet/07-Serializing-Prompts/prompts/chat/config.json":::

# [Python](#tab/python)

:::code language="json" source="~/../samples/python/07-Serializing-Prompts/prompts/chat/config.json":::

---


### Testing your prompt
At this point, you can import and test your function with the kernel by updating your _Program.cs_ or _main.py_ file to the following.

# [C#](#tab/Csharp)
:::code language="csharp" source="~/../samples/dotnet/07-Serializing-Prompts/Program.cs" range="6-8,10-17,20-25,33-35, 53-59,77-99":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/07-Serializing-Prompts/main.py" range="2-3,5-10,12-41":::

---

## Using YAML to serialize your prompt
In addition to the _skprompt.txt_ and _config.json_ files, you can also serialize your prompt using a single YAML file while using the C# SDK. This is useful if you want to use a single file to define your prompt. Additionally, this is the same format that is used by Azure AI Studio, making it easier to share prompts between the two platforms.

Let's try creating a YAML serialization file for the `getIntent` prompt. To get started, you first need to install the necessary packages.

```console
dotnet add package Microsoft.SemanticKernel.Yaml --prerelease
```

This walkthrough also uses the Handlebars template engine, so you'll need to install the Handlebars package as well.

```console
dotnet add package Microsoft.SemanticKernel.PromptTemplate.Handlebars --prerelease
```

Next, create a new file called _getIntent.prompt.yaml_ in the _Prompts_ folder and copy the following YAML into the file.

:::code language="yaml" source="~/../samples/dotnet/07-Serializing-Prompts/prompts/getIntent.prompt.yaml":::

You should notice that all of the same properties that were defined in the _config.json_ file are now defined in the YAML file. Additionally, the `template` property is used to define the prompt template.

As a best practice, we recommend adding your prompts as an embedded resource. To do this, you'll need to update your _csproj_ file to include the following:

```xml
<ItemGroup>
     <EmbeddedResource Include="Prompts\**\*.yaml" />
</ItemGroup>
```

Finally, you can import your prompt in the _Program.cs_ file.

:::code language="csharp" source="~/../samples/dotnet/07-Serializing-Prompts/Program.cs" range="26-31":::

To call the prompt, you can use the following code:

:::code language="csharp" source="~/../samples/dotnet/07-Serializing-Prompts/Program.cs" range="60-69":::



## Take the next step
Now that you know how to save your prompts, you can begin learning how to create an agent.

> [!div class="nextstepaction"]
> [Create your first agent](../agents/index.md)