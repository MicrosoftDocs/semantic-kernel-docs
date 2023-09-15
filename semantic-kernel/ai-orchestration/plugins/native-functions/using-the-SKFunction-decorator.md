---
title: How to add native code to your AI apps with Semantic Kernel
description: Learn how to write native functions inside of AI plugins for Semantic Kernel.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Run native code with Semantic Kernel

[!INCLUDE [pat_large.md](../../../includes/pat_large.md)]

In the [how to create semantic functions](../semantic-functions/inline-semantic-functions.md) section, we showed how you could create a semantic function that retrieves a user's intent, but what do you do once you have the intent? In _most_ cases, you want to perform some sort of task based on the intent. For example, if the user wants to send an email, you'll need to make the necessary API calls to actually send an email.

Automating tasks like these are the primary purpose of AI apps. In this section, we'll show how you can create a simple native function that can perform a task LLMs cannot do easily on their own: arithmetic. In a [subsequent tutorial](./calling-nested-functions.md) we'll demonstrate how to combine native functions with semantic functions to correctly answer word problems like `What is the square root of 634?` and `What is 42 plus 1513?`

If you want to see the final solution to this article, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/06-Nested-Functions-In-Semantic-Functions) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/07-Simple-Native-Functions) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/06-Nested-Functions-In-Semantic-Functions) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/07-Simple-Native-Functions) |


## Why use native functions?
Large language models are great at generating text, but there are several tasks they cannot perform on their own. These include, but are not limited to:
- Retrieve data from external data sources
- Knowing what time it is
- Performing complex math
- Completing tasks in the real world
- Memorizing and recalling information

### Augmenting large language models with native functions
Thankfully, these tasks can already be completed by computers using native code. With native functions, you can author these features as functions that can later be called by the kernel. This allows you to combine the power of large language models with the power of native code.

For example, if you simply asked a large language model `What is the square root of 634?`, it would likely return back a number that is _close_ to the square root of 634, but not the _exact_ answer. This is because large language models are trained to predict the next word in a sequence, not to perform math.

### Giving the kernel the ability to perform math

To solve this problem, we'll demonstrate how to create native functions that can perform arithmetic based on a user's intent. At the end of this section you will have the following supported functions.

| Plugin | Function | Type | Description |
| --- | --- | --- | --- |
| Orchestrator Plugin | GetIntent | Semantic | Gets the intent of the user |
| Orchestrator Plugin | GetNumbers | Semantic | Gets the numbers from a user's request |
| Orchestrator Plugin | RouteRequest | Native | Routes the request to the appropriate function |
| Math Plugin | Sqrt | Native | Takes the square root of a number |
| Math Plugin | Multiple | Native | Multiplies two numbers together |

In this article, we'll start with a simple example by demonstrating how to create a `Sqrt` function. In the [Using multiple inputs and outputs](./multiple-parameters.md) article, we'll then show how to create functions that require multiple inputs (like the `Multiply` function). Finally, in the [Calling nested functions](./calling-nested-functions.md) article, we'll show how to create the `RouteRequest` and `GetNumbers` functions which combine native and semantic functions together.

## Finding a home for your native functions

You can place native functions in the same plugin directory as your semantic functions. For example, to create native functions for a plugin called `MyPlugin`, you can create a new file called _MyPlugin.cs_ in the same directory as your semantic functions.

```directory
MyPluginsDirectory
│
└─── MyPlugin
     │
     └─── MyFirstSemanticFunction
     │    └─── skprompt.txt
     │    └─── config.json
     └─── MyOtherSemanticFunctions
     |    | ...  
     │
     └─── MyPlugin.cs
```

> [!Tip]
> It's ok if you have a plugin folder with both native and semantic functions. The kernel will load both functions into the same plugin namespace. What's important is that you don't have two functions with the same name within the same plugin namespace. If you do, the last function loaded will take precedence.

### Creating the folder for the Math plugin

Since we're giving our kernel the ability to perform math, we'll create a new plugin called `MathPlugin`. To do this, we'll create a _MathPlugin_ folder along with a file to store all its native functions. Depending on the language you're using, you'll create either a C# or Python file.

# [C#](#tab/Csharp)

```directory
Plugins
│
└─── OrchestratorPlugin
|    │
|    └─── GetIntent
|         └─── skprompt.txt
|         └─── config.json
|
└─── MathPlugin
     │
     └─── Math.cs
```

# [Python](#tab/python)

```directory
Plugins
│
└─── OrchestratorPlugin
|    │
|    └─── GetIntent
|         └─── skprompt.txt
|         └─── config.json
|
└─── MathPlugin
     │
     └─── Math.py
```
---

## Creating your native functions
Open up the _Math.cs_ or _Math.py_ file you created earlier and follow the instructions below to create the `Sqrt` function. This function will take a single number as an input and return the square root of that number.

### Defining the class for your plugin
All native functions must be defined as public methods of a class that represents your plugin. To begin, create a class called `Math` in your _Math.cs_ or _Math.py_ file. 

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/07-Simple-Native-Functions/plugins/MathPlugin/Math.cs" range="3-10,16" highlight="7":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/07-Simple-Native-Functions/plugins/MathPlugin/Math.py" range="1-5" highlight="5":::

---

### Use the SKFunction decorator to define a native function

Now that you have a class for your plugin, you can add the `Sqrt` function. To make sure Semantic Kernel knows this is a native function, use the `SKFunction` decorator above your new method. This decorator will tell the kernel that this method is a native function and will automatically register it with the kernel when the plugin is loaded.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/07-Simple-Native-Functions/plugins/MathPlugin/Math.cs" range="3-16"  highlight="9":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/07-Simple-Native-Functions/plugins/MathPlugin/Math.py" range="1-12" highlight="6-10":::

---

Notice that the input and and return types are strings. This is because the kernel passes all parameters as strings so they can work seamlessly with semantic functions. While inside of a function, you can convert the input to any type you want. In our case, we convert the string into a number so we can perform math on it before converting it back to a string.

Also notice how we've added a description to each function with the `Description` attribute. This description will be used in the future by the [planner](../../planners/index.md) to automatically create a plan using these functions. In our case, we're telling planner that this function can `Take the square root of a number`.

### Running your native function
Now that you've created your first native function, you can import it and run it using the following code. Notice how calling a native function is the same as calling a semantic function. This is one of the benefits of using the kernel, both semantic and native functions are treated identically.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/07-Simple-Native-Functions/program.cs" range="3-5,14-19,22-29" highlight="16":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/07-Simple-Native-Functions/main.py" range="1-2,4-11,13-30" highlight="16-19":::

---

The code should output `3.4641016151377544` since it's the square root of `12`.


## Take the next step
Now that you can create a simple native function, you can now learn how to create native functions that accept multiple input parameters.
This will be helpful to create functions like addition, multiplication, subtraction, and division which all require multiple inputs.

> [!div class="nextstepaction"]
> [Passing multiple parameters into native functions](./multiple-parameters.md)