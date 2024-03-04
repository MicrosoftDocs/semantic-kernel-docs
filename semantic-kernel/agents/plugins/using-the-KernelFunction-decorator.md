---
title: Give your agents native functions to call
description: Learn how to write native functions inside of plugins for Semantic Kernel.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Creating native functions for AI to call


In the [how to create prompts](../../prompts/index.md) section, we showed how you could create a prompt that retrieves a user's intent, but what do you do once you have the intent? In _most_ cases, you want to perform some sort of task based on the intent. For example, if the user wants to send an email, you'll need to make the necessary API calls to actually send an email.

Automating tasks like these are the primary purpose of AI apps. In this section, we'll show how you can create a simple native function that can perform a task LLMs cannot do easily on their own: arithmetic.

If you want to see the final solution to this article, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- | --- |
| C# | [Open example in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/DocumentationExamples/SerializingPrompts.cs) | [Open solution in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/DocumentationExamples/CreatingFunctons.cs) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/07-Serializing-Prompts) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/08-Creating-Functions-For-AI) |

## Why should you create functions for your AI?
Large language models are great at generating text, but there are several tasks they cannot perform on their own. These include, but are not limited to:
- Retrieve data from external data sources
- Knowing what time it is
- Performing complex math
- Completing tasks in the real world
- Memorizing and recalling information

### Augmenting AI with native functions
Thankfully, these tasks can already be completed by computers using native code. With native functions, you can author these features as functions that can later be called by the kernel. This allows you to combine the power of large language models with the power of native code.

For example, if you simply asked a large language model `What is the square root of 634?`, it would likely return back a number that is _close_ to the square root of 634, but not the _exact_ answer. This is because large language models are trained to predict the next word in a sequence, not to perform math.

### Giving your agent the ability to perform math

To solve this problem, we'll demonstrate how to create native functions that can perform arithmetic based on a user's intent. At the end of this section you will have the following supported functions that your AI can call.

- `Sqrt` – Takes the square root of a number
- `Add` – Adds two numbers together
- `Subtract` – Subtracts two numbers
- `Multiply` – Multiplies two numbers
- `Divide` – Divides two numbers

## Finding a home for your native functions

We recommend that you create a new folder for your plugins at the root of your project. We recommend putting this folder at the root of your project and calling it _Plugins_.

Since we're giving our kernel the ability to perform math, we'll create a new plugin called `MathPlugin`. To do this, we'll create a _MathPlugin_ folder along with a file to store all its native functions. Depending on the language you're using, you'll create either a C# or Python file.

# [C#](#tab/Csharp)

```directory
Plugins
│
└─── MathPlugin.cs
```

# [Python](#tab/python)

```directory
Plugins
│
└─── Math.py
```
---

## Creating your native functions
Open up the _MathPlugin.cs_ or _Math.py_ file you created earlier and follow the instructions below to create the `Sqrt` function. This function will take a single number as an input and return the square root of that number.

### Defining the class for your plugin
All native functions must be defined as public methods of a class that represents your plugin. To begin, create a class called `Math` in your _MathPlugin.cs_ or _Math.py_ file. 

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Plugins/MathPlugin.cs" range="4-10":::

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/plugins/MathPlugin/native_function.py" range="1-12":::

---

### Use the KernelFunction decorator to define a native function

Now that you have a class for your plugin, you can add the `Sqrt` function. To make sure Semantic Kernel knows this is a native function, use the `KernelFunction` decorator above your new method. This decorator will tell the kernel that this method is a native function and will automatically register it with the kernel when the plugin is loaded.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Plugins/MathPlugin.cs" range="11-17" highlight="1":::

Notice how we've added a description to the function and each of its parameters with the `Description` attribute. This description will be used by function calling and by [planners](../planners/index.md) to automatically create a plan using these functions. In our case, we're telling planner that this function can `Take the square root of a number`.

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/plugins/MathPlugin/native_function.py" range="48-56" highlight="1-4":::

Notice that the input and and return types are strings. This is because the kernel passes all parameters as strings so they can work seamlessly with prompts. While inside of a function, you can convert the input to any type you want. In our case, we convert the string into a number so we can perform math on it before converting it back to a string.

Also notice how we've added a description to each function with the `Description` attribute. This description will be used in the future by the [planner](../planners/index.md) to automatically create a plan using these functions. In our case, we're telling planner that this function can `Take the square root of a number`.

---

### Creating the remaining math functions
Now that you've created the `Sqrt` function, you can create the remaining math functions. To do this, you can copy the `Sqrt` function and update the code to perform the correct math operation. Below is the entire `MathPlugin` class with all the functions implemented.


# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Plugins/MathPlugin.cs":::

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/plugins/MathPlugin/native_function.py":::

---

## Running your native function
Now that you've created your first native function, you can import it and run it using the following code. Notice how calling a native function is the same as calling a prompt. This is one of the benefits of using the kernel, both semantic and native functions are treated identically.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/CreatingFunctions.cs" range="4-7,34,36-47" highlight="12":::

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/creating_functions.py"  highlight="24-28":::

---

The code should output `3.4641016151377544` since it's the square root of `12`.

### Allow the AI to automatically call your function
Now that you've created your first native function, you can now allow the AI to automatically call it within the C# version of the SDK. To do this, let's go ahead and create a chat loop that will allow us to talk back-and-forth with our agent.

While in the chat loop, we'll configure the OpenAI connection to automatically call any functions that are registered with the kernel. To do this, we'll set the `ToolCallBehavior` property to `ToolCallBehavior.AutoInvokeKernelFunctions` on the `OpenAIPromptExecutionSettings` object.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/CreatingFunctions.cs" range="50-51,57-97" highlight="14":::

When you run this code, you'll be able to ask the AI to perform math for you. For example, you can ask the AI to `Take the square root of 12` and it will return back the correct answer.

## Take the next step
Now that you can create a simple native function, you can now learn how to create native functions that accept multiple input parameters.
This will be helpful to create functions like addition, multiplication, subtraction, and division which all require multiple inputs.

> [!div class="nextstepaction"]
> [Using OpenAI plugins](./openai-plugins.md)