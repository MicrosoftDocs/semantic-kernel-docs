---
title: Add inputs and outputs to prompts
description: Learn how to templatize semantic functions in Semantic Kernel.
author: matthewbolanos
ms.topic: tutorial
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Templatizing your semantic functions

[!INCLUDE [pat_large.md](../../../includes/pat_large.md)]

In the [previous article](./serializing-semantic-functions.md) we created a semantic function that could be used to get the intent of the user. This function, however, is not very reusable. For example, if we wanted to run specific code based on the user intent, it would be difficult to use the output of the `GetIntent` function to choose which code to actually run.

We need to find a way to _constrain_ the output of our function so that we can later use the output in a switch statement inside of a native function.

By following this example, you'll learn how to templatize a semantic function. If you want to see the final solution, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/04-Serializing-Semantic-Functions) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/05-Templatizing-Semantic-Functions) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/04-Serializing-Semantic-Functions) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/05-Templatizing-Semantic-Functions) |

## Adding variables to the prompt
One way to constrain the output of a semantic function is to provide a list of options for it to choose from. A naive approach would be to hard code these options into the prompt, but this would be difficult to maintain and would not scale well. Instead, we can use Semantic Kernel's templating language to dynamically generate the prompt.

The [prompt template syntax](../../../prompt-engineering/prompt-template-syntax.md) article in the [prompt engineering](../../../prompt-engineering/index.md) section of the documentation provides a detailed overview of how to use the templating language. In this article, we'll show you just enough to get started.

To begin, open the _skprompt.txt_ file in the _GetIntent_ folder from the previous solution and update it to the following prompt.

:::code language="txt" source="~/../samples/dotnet/05-Templatizing-Semantic-Functions/plugins/OrchestratorPlugin/GetIntent/skprompt.txt":::

:::row:::
   :::column span="2":::
      The new prompt uses the `options` variable to provide a list of options for the LLM to choose from. We've also added a `history` variable to the prompt so that the previous conversation is included.
      
      By including these variables, we are able to help the LLM choose the correct intent by providing it with more context and a constrained list of options to choose from.
   :::column-end:::
   :::column span="3":::
      ![Consuming context variables within a semantic function](../../../media/using-context-in-templates.png)
   :::column-end:::
:::row-end:::


When you add a new variable to the prompt, you must also update the _config.json_ file to include the new variables and their descriptions. While these properties aren't used now, it's good to get into the practice of adding them so they can be used by the [planner](../../planners/index.md) later. The following configuration adds the `options` and `history` variable to the `input` section of the configuration.

:::code language="json" source="~/../samples/dotnet/05-Templatizing-Semantic-Functions/plugins/OrchestratorPlugin/GetIntent/config.json":::

## Passing in context variables

You can now update your _Program.cs_ file to provide a list of options to the `GetIntent` function. To do this, you'll need to complete the following steps:

1. Create a [ContextVariables](/dotnet/api/microsoft.semantickernel.orchestration.contextvariables) object that will store the variables,
2. Set the `input`, `history`, and `options` variables,
3. And finally pass the object into the kernel's `RunAsync` function.

You can see how to do this in the code snippets below.

# [C#](#tab/Csharp)

Initialize the kernel and import the plugins.

:::code language="csharp" source="~/../samples/dotnet/05-Templatizing-Semantic-Functions/Program.cs" range="4-6,15-20,23-28":::

Create a new context and set the input, history, and options variables.

:::code language="csharp" source="~/../samples/dotnet/05-Templatizing-Semantic-Functions/Program.cs" range="31-38":::

Run the GetIntent function with the context variables.

:::code language="csharp" source="~/../samples/dotnet/05-Templatizing-Semantic-Functions/Program.cs" range="41-43":::

# [Python](#tab/python)

Initialize the kernel and import the plugins.

:::code language="python" source="~/../samples/python/05-Templatizing-Semantic-Functions/main.py" range="1, 3-10,12-18":::

Create a new context and set the input, history, and options variables.

:::code language="python" source="~/../samples/python/05-Templatizing-Semantic-Functions/main.py" range="21-28":::

Run the GetIntent function with the context variables.

:::code language="python" source="~/../samples/python/05-Templatizing-Semantic-Functions/main.py" range="31-36":::

Call your main function.

:::code language="python" source="~/../samples/python/05-Templatizing-Semantic-Functions/main.py" range="39-43":::


---

Now, instead of getting an output like `Send congratulatory email`, we'll get an output like `SendEmail`. This output could then be used within a switch statement in native code to execute the next appropriate step.

## Take the next step
Now that you can templatize your semantic function, you can now learn how to call functions from within
a semantic function to help break up the prompt into smaller pieces.

> [!div class="nextstepaction"]
> [Call nested functions](./calling-nested-functions.md)