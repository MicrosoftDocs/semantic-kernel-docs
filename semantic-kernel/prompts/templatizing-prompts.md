---
title: Making your prompts reusable
description: Learn how to templatize prompts in Semantic Kernel.
author: matthewbolanos
ms.topic: tutorial
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Templatizing your prompts

In the [previous article](./your-first-prompt.md) we created a prompt that could be used to get the intent of the user. This function, however, is not very reusable. Because the options are hard coded in. We could dynamically create the prompts string, but there's a better way: prompt templates. function.

By following this example, you'll learn how to templatize a prompt. If you want to see the final solution, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- |
| C# | [Open example in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/DocumentationExamples/Templates.cs) | [Open solution in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example31_SerializingPrompts.cs) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/07-Serializing-Prompts) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/04-Templatizing-Prompts) |

## Adding variables to the prompt
With Semantic Kernel's templating language, we can add tokens that will be automatically replaced with input parameters. To begin, let's build a super simple prompt that uses the Semantic Kernel template syntax language to include enough information for an agent to respond back to the user.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" range="41-45":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/04-Templatizing-Prompts/main.py" range="15-17":::

---

The new prompt uses the `request` and `history` variables so that we can include these values when we run our prompt.
To test our prompt, we can create a chat loop so we can begin talking back-and-forth with our agent.
When we invoke the prompt, we can pass in the `request` and `history` variables as arguments.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" range="7-8,36-39,92-101,120-147" highlight="21-22":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/04-Templatizing-Prompts/main.py" range="5-14, 19-37" highlight="14-16,22":::

---

## Using the Handlebars template engine
In addition to the core template syntax, Semantic Kernel also comes with support for the Handlebars templating language in the C# SDK. To use Handlebars, you'll first want to add the Handlebars package to your project.

```console
dotnet add package Microsoft.SemanticKernel.PromptTemplate.Handlebars --prerelease
```

Then import the Handlebars template engine package.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" range="41-45":::

Afterwards, you can create a new prompt using the `HandlebarsPromptTemplateFactory`. Because Handlebars supports loops, we can use it to loop over elements like examples and chat history. This makes it a great fit for the `getIntent` prompt we created in the [previous article](./your-first-prompt.md).

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" range="41-45":::

We can then create the choice and example objects that will be used by the template. In this example, we can use our prompt to end the conversation once it's over. To do this, we'll just provide two valid intents: `ContinueConversation` and `EndConversation`.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" range="41-45":::

Finally, you can run the prompt using the kernel. Add the following code within your main chat loop so the loop can be terminated once the intent is `EndConversation`.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" range="41-45":::

## Take the next step
Now that you can templatize your prompt, you can now learn how to call functions from within
a prompt to help break up the prompt into smaller pieces.

> [!div class="nextstepaction"]
> [Call nested functions](./calling-nested-functions.md)