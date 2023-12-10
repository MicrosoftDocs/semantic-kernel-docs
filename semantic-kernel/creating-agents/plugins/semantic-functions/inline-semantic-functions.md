---
title: Creating semantic functions inline
description: Learn how to create and run prompts in code using Semantic Kernel.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Creating semantic functions inline

[!INCLUDE [pat_large.md](../../../includes/pat_large.md)]

The simplest way to create a semantic function is to create it inline within your application. This is useful for prototyping and testing. Later, you can [serialize it](./serializing-semantic-functions.md) so that its more easily reusable.

If you want to see the final solution to this tutorial, you can check out the following samples in the public documentation repository.

| Language  | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/03-Inline-Semantic-Functions) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/03-Inline-Semantic-Functions) |


## Defining a semantic function in code
At its core, a semantic function is just a prompt that is sent to an AI service along with any additional settings the AI service requires. By combining both prompts and settings, you can declaratively define everything necessary to execute a prompt into a single function.

### Defining the prompt
The first step is to define the prompt. The [prompt engineering](../../../prompt-engineering/index.md) section of the documentation provides a detailed overview of how to write successful prompts, but at a high level, prompts are requests written in natural language that are sent to an AI service like OpenAI.

In most cases, you'll send your prompt to a text or chat completion service which will return back a response that attempts to complete the prompt. For example, if you sent the prompt `"I want to go to the "`, the AI service might return back `"beach"` to complete the sentence. This is a very simple example, but it demonstrates the basic idea of how completion prompts work.

In the following sample, we'll demonstrate how you could create a simple function called `GetIntent` that takes a user request and returns back the intent of the user. This is useful for scenarios where you want to understand the intent of a user's input and then take some action based on that intent.

The following code shows how you could define the prompt for the `GetIntent` function.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/03-Inline-Semantic-Functions/Program.cs" range="8-13":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/03-Inline-Semantic-Functions/main.py" range="8-13":::

---

Notice that we're using a variable called `input` in the prompt. This variable is defined in the settings of the semantic function and is used to pass the user's input to the AI service.

### Configuring the function
Now that you have a prompt, you need to define the settings necessary for your prompt to work correctly with your AI service. The [configuring prompts](../../../prompt-engineering/configure-prompts.md) article provides a detailed overview of how to configure semantic functions, but at a high level, you need to provide the following information:

- `type` – The type of prompt. In this case, we're using the `completion` type.
- `description` – A description of what the prompt does. This is used by planner to automatically orchestrate plans with the function.
- `completion` – The settings for completion models. For OpenAI models, this includes the `max_tokens` and `temperature` properties.
- `input` – Defines the variables that are used inside of the prompt (e.g., `input`).

The following sample shows how you could define the settings for the `GetIntent` function.


# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/03-Inline-Semantic-Functions/Program.cs" range="16-25":::

# [Python](#tab/python)


:::code language="python" source="~/../samples/python/03-Inline-Semantic-Functions/main.py" range="16-27":::

---

### Importing the semantic function into the kernel
Now that you have a prompt and its settings, you can import it into a kernel. The following code demonstrates how to register the `GetIntent` function with a new kernel.

# [C#](#tab/Csharp)
:::code language="csharp" source="~/../samples/dotnet/03-Inline-Semantic-Functions/Program.cs" range="35-40, 43-46":::

# [Python](#tab/python)
To register the function with the kernel, you first need to create a SemanticFunctionConfig object. 

:::code language="python" source="~/../samples/python/03-Inline-Semantic-Functions/main.py" range="29-33,35-42":::

Afterwards, you can pass it into the kernel using the `register_semantic_function` method. The first parameter is the name of the plugin it belongs to, the second is the name of the function, and the last is the configuration object.

:::code language="python" source="~/../samples/python/03-Inline-Semantic-Functions/main.py" range="45-49":::

---

### Testing the semantic function
Now that you have a prompt and its settings, you can test the function by running it with the kernel.

The following sample shows how you could run the `GetIntent` function with the input "I want to send an email to the marketing team celebrating their recent milestone."

# [C#](#tab/Csharp)
:::code language="csharp" source="~/../samples/dotnet/03-Inline-Semantic-Functions/Program.cs" range="49-54":::

# [Python](#tab/python)
:::code language="python" source="~/../samples/python/03-Inline-Semantic-Functions/main.py" range="52-57":::

---

You should get an output that looks like the following:

```output
Send congratulatory email.
```

## Next steps
Now that you know how to create a semantic function inline, you can learn how to serialize it so its easier to reuse.

> [!div class="nextstepaction"]
> [Serialize your semantic function](./serializing-semantic-functions.md)