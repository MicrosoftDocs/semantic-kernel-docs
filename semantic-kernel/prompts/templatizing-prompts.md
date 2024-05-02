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
| C# | [Open example in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/LearnResources/MicrosoftLearn/Templates.cs) | [Open solution in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example31_SerializingPrompts.cs) |
| Java |  | [Open solution in GitHub](https://github.com/microsoft/semantic-kernel/blob/java-v1/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Templates.java) |
| Python | [Open solution in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/documentation_examples/serializing_prompts.py) | [Open solution in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/documentation_examples/templates.py) |

## Adding variables to the prompt
With Semantic Kernel's templating language, we can add tokens that will be automatically replaced with input parameters. To begin, let's build a super simple prompt that uses the Semantic Kernel template syntax language to include enough information for an agent to respond back to the user.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Templates.cs" range="40-44":::

# [Java](#tab/Java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Templates.java" id="create_chat":::

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/templates.py" range="28-49":::

---

The new prompt uses the `request` and `history` variables so that we can include these values when we run our prompt.
To test our prompt, we can create a chat loop so we can begin talking back-and-forth with our agent.
When we invoke the prompt, we can pass in the `request` and `history` variables as arguments.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Templates.cs" range="6-7":::

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Templates.cs" range="35-38,91-100,119-146" highlight="20-21":::

# [Java](#tab/Java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Templates.java" id="use_chat":::

# [Python](#tab/python)

In the Python template, we just need to provide the value for the `history` variable.

1. Import Semantic Kernel.
    :::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/templates.py" range="7-10" :::

2. Create the kernel.
    :::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/templates.py" range="15" :::

3. Add the service to the kernel.

    :::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="39-46":::

4. Run the prompt in a chat loop.

    :::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/templates.py" range="51-77" highlight="19-23":::

---

## Using the Handlebars template engine
In addition to the core template syntax, Semantic Kernel also comes with support for the Handlebars templating language in the C# and Java SDK. To use Handlebars, you'll first want to add the Handlebars package to your project.

# [C#](#tab/Csharp)

```console
dotnet add package Microsoft.SemanticKernel.PromptTemplate.Handlebars --prerelease
```

Then import the Handlebars template engine package.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Templates.cs" range="8":::

Afterwards, you can create a new prompt using the `HandlebarsPromptTemplateFactory`. Because Handlebars supports loops, we can use it to loop over elements like examples and chat history. This makes it a great fit for the `getIntent` prompt we created in the [previous article](./your-first-prompt.md).

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Templates.cs" range="66-90" highlight="10-18":::

We can then create the choice and example objects that will be used by the template. In this example, we can use our prompt to end the conversation once it's over. To do this, we'll just provide two valid intents: `ContinueConversation` and `EndConversation`.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Templates.cs" range="46-64":::

Finally, you can run the prompt using the kernel. Add the following code within your main chat loop so the loop can be terminated once the intent is `EndConversation`.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Templates.cs" range="101-117" highlight="14":::

# [Java](#tab/Java)

Functions can be created from handlebars templates:

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Templates.java" id="handlebars_prompt":::

This template requires the following variables:
- `choices` -  A list containing `[ContinueConversation, EndConversation]` that are the possible intents of a users request.
- `fewShotExamples` - A list of examples demonstraiting how the AI should classify a statement.
- `history` - The conversation the AI and user has had so far.
- `request` - The users current request

These can be added to the arguments as:

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Templates.java" id="handlebars_add_variables_1":::

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Templates.java" id="handlebars_add_variables_2":::

The function can then be invoked as normal:

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Templates.java" id="handlebars_invoke":::

# [Python](#tab/python)

<!-- empty for now -->

---

## Take the next step
Now that you can templatize your prompt, you can now learn how to call functions from within
a prompt to help break up the prompt into smaller pieces.

> [!div class="nextstepaction"]
> [Call nested functions](./calling-nested-functions.md)
