---
title: Understanding AI plugins in Semantic Kernel and beyond
description: Learn how to use AI plugins and how they relate to AI plugins in ChatGPT, Bing, and Microsoft 365.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Understanding AI plugins in Semantic Kernel



Plugins are the fundamental building blocks of Semantic Kernel and can interoperate with plugins in ChatGPT, Bing, and Microsoft 365.
With plugins, you can encapsulate capabilities into a single unit of functionality that can then be run by the kernel. Plugins can consist of both native code and requests to AI services via prompts. 

## What is a plugin?
To drive alignment across the industry, we've adopted the [OpenAI plugin specification](https://platform.openai.com/docs/plugins/getting-started/) as the standard for plugins. This will help create an ecosystem of interoperable plugins that can be used across all of the major AI apps and services like ChatGPT, Bing, and Microsoft 365.

:::row:::
   :::column span="2":::
        ![Semantic Kernel can orchestrate AI plugins from any provider](../../media/cross-platform-plugins.png)
   :::column-end:::
   :::column span="1":::
      For developers using Semantic Kernel, this means any plugins you build can be exported so they are usable in ChatGPT, Bing, and Microsoft 365. This allows you to increase the reach of your AI capabilities without rewriting code. It also means that plugins built for ChatGPT, Bing, and Microsoft 365 can be imported into Semantic Kernel seamlessly.
   :::column-end:::
:::row-end:::

To show how to make interoperable plugins, we've created an in-depth walkthrough on how to export a Semantic Kernel plugin as an OpenAI plugin using OpenAI's specification. You can find the walkthrough in the [Create and run OpenAI plugins](./openai-plugins.md) section.

### What does a plugin look like?
At a high-level, a plugin is a group of functions that can be exposed to AI apps and services. The functions within plugins can then be orchestrated by an AI application to accomplish user requests. Within Semantic Kernel, you can invoke these functions either manually or automatically with function calling or [planners](../planners/index.md).

_Just_ providing functions, however, is not enough to make a plugin. To power automatic orchestration with a [planner](../planners/index.md), plugins _also_ need to provide details that semantically describe how they behave. Everything from the function's input, output, and side effects need to be described in a way that the AI can understand, otherwise, the planner will provide unexpected results.

:::row:::
   :::column span="1":::
      For example, in the [`WriterPlugin` plugin](https://github.com/microsoft/semantic-kernel/tree/main/samples/plugins/WriterPlugin), each function has a semantic description that describes what the function does. A planner can then use these descriptions to choose the best functions to call to fulfill a user's ask.
      
      In the picture on the right, a planner would likely use the `ShortPoem` and `StoryGen` functions to satisfy the users ask thanks to the provided semantic descriptions.
   :::column-end:::
   :::column span="3":::
        ![Semantic description within the WriterPlugin plugin](../../media/writer-plugin-example.png)
   :::column-end:::
:::row-end:::

## Adding functions to plugins
Now that you know what a plugin is, let's take a look at how to create one. Within a plugin, you can create two types of functions: prompts and native functions. The following sections describe how to create each type. For further details, please refer to the [Creating prompts](../../prompts/index.md) and [Creating native functions](./using-the-KernelFunction-decorator.md) sections.

### Native functions

:::row:::
   :::column span="2":::
      With native functions, you can have the kernel call C# or Python code directly so you can manipulate data or perform other operations. In this way, native functions are like the hands of your AI app. They can be used to save data, retrieve data, and perform any other operation that you can do in code that is ill-suited for LLMs (e.g., performing calculations).
   :::column-end:::
   :::column span="3":::
        ![Prompts are the hands of your app](../../media/native-function-explainer.png)
   :::column-end:::
:::row-end:::


Instead of providing a separate configuration file with semantic descriptions, planners are able to use annotations in the code to understand how the function behaves. Below are examples of the annotations used by planner in both C# and Python for out-of-the-box native functions.

# [C#](#tab/Csharp)
The following code is an excerpt from the `DocumentSkill` plugin, which can be found in the [document plugin](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Skills/Skills.Document) folder in the GitHub repository. It demonstrates how you can use the `SKFunction` and `SKFunctionInput` attributes to describe the function's input and output to planner.

```csharp
[SKFunction, Description("Read all text from a document")]
public async Task<string> ReadTextAsync(
   [Description("Path to the file to read")] string filePath
)
{
    this._logger.LogInformation("Reading text from {0}", filePath);
    using var stream = await this._fileSystemConnector.GetFileContentStreamAsync(filePath).ConfigureAwait(false);
    return this._documentConnector.ReadText(stream);
}
```

# [Java](#tab/Java)
The following code is an excerpt from the `MathPlugin` plugin, which can be found in the [examples](https://github.com/microsoft/semantic-kernel/blob/java-v1/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/plugins/MathPlugin.java) folder in the GitHub repository. It demonstrates how you can use the `kernel_function` and `Annotated` decorators to describe the function's input and output to planner.

```java
@DefineKernelFunction(name = "add", description = "Add two numbers")
public static double add(
      @KernelFunctionParameter(name = "number1", description = "The first number to add", type = double.class) double number1,
      @KernelFunctionParameter(name = "number2", description = "The second number to add", type = double.class) double number2) {
   return number1 + number2;
}
```

# [Python](#tab/python)

The following code is an excerpt from the `MathPlugin` plugin, which can be found in the [core skills](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/core_plugins) folder in the GitHub repository. It demonstrates how you can use the `kernel_function` and `Annotated` decorators to describe the function's input and output to planner.

```python
@kernel_function(name="Add")
def add(
   self,
   number1: Annotated[float, "the first number to add"],
   number2: Annotated[float, "the second number to add"],
) -> Annotated[float, "the output is a float"]:
   return float(number1) + float(number2)
```

---

You can learn more about creating native functions in the [Creating native functions](./using-the-KernelFunction-decorator.md) section. In this article you'll learn the best practices for the following:
> [!div class="checklist"]
> * How to create simple native functions with the `SKFunction` decorator
> * Using multiple input parameters with native functions
> * Calling nested functions from within native functions

:::row:::
   :::column span="2":::
      ### Prompts

      If plugins represent the "[body](../../overview/index.md#semantic-kernel-makes-ai-development-extensible)" of your AI app, then prompts would represent the ears and mouth of your AI. They allow your AI app to listen to users asks and respond back with a natural language response.
      
      To connect the ears and the mouth to the "brain," Semantic Kernel uses connectors. This allows you to easily swap out the AI services without rewriting code.
   :::column-end:::
   :::column span="3":::
        ![Prompts are the ears and mouth of your AI apps](../../media/semantic-function-explainer.png)
   :::column-end:::
:::row-end:::

Below is an sample called `Summarize` that can be found in the [samples folder](https://github.com/microsoft/semantic-kernel/tree/main/samples/plugins/SummarizePlugin/Summarize) in the GitHub repository.

:::code language="txt" source="~/../semantic-kernel-samples/samples/plugins/SummarizePlugin/Summarize/skprompt.txt":::

To semantically describe this function (as well as define the configuration for the AI service), you must also create a _config.json_ file in the same folder as the prompt. This file describes the function's input parameters and description. Below is the _config.json_ file for the `Summarize` function.


:::code language="json" source="~/../semantic-kernel-samples/samples/plugins/SummarizePlugin/Summarize/config.json":::


Both `description` fields are used by [planner](../planners/index.md), so it's important to provide a detailed, yet concise, description so the planner can make the best decision when orchestrating functions together. We recommend testing multiple descriptions to see which one works best for the widest range of scenarios.

You can learn more about creating prompts in the [Creating prompts](../../prompts/index.md) section. In this section you'll learn the best practices for the following:
> [!div class="checklist"]
> * How to create prompts
> * Adding input parameters with prompt templates
> * Calling nested functions in prompts
> * How to create files for your prompts

## Take the next step
Now that you understand the basics of plugins, you can now go deeper into the details of creating semantic and native functions for your plugin.

> [!div class="nextstepaction"]
> [Create a plugin with native functions](./using-the-KernelFunction-decorator.md)

