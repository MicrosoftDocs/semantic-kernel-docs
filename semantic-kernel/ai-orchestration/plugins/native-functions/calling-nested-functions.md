---
title: Calling functions within a native function
description: Leverage native and semantic functions within a Semantic Kernel native function.
author: matthewbolanos
ms.topic: tutorial
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Calling functions within a native function

[!INCLUDE [pat_large.md](../../../includes/pat_large.md)]

In the [previous tutorials](./multiple-parameters.md), we demonstrated how to create functions with one or more input parameters. In this tutorial, we'll demonstrate how to call other functions _within_ a native function. This will allow you to build more complex functions that consist of both native and semantic functions.

If you want to see the final solution to this article, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/08-Native-Functions-with-Context) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/09-Calling-Nested-Functions-in-Native-Functions) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/08-Native-Functions-with-Context) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/09-Calling-Nested-Functions-in-Native-Functions) |

> [!Important]
> This article uses the `GetIntent` from the [semantic functions](../semantic-functions/calling-nested-functions.md#calling-a-nested-function) section. We recommend using the final solution from the [previous tutorial](./multiple-parameters.md) as a starting point for this tutorial so you can follow along.

## Using native and semantic functions together
Typically, you'll want to use semantic functions to extract data from the user's request and native functions to perform the actual work. [Previously](../semantic-functions/calling-nested-functions.md#calling-a-nested-function), we created a `GetIntent` semantic function that extracts the user's intent from their request. In this tutorial, we'll leverage this semantic function in a new native function called `RouteRequest` that will call the appropriate function based on the user's intent.

### Finding a home for the `RouteRequest` function
Since the `RouteRequest` function helps with orchestration, we can add it to the `OrchestratorPlugin` plugin we created in the [semantic function](../semantic-functions/serializing-semantic-functions.md) section.

# [C#](#tab/Csharp)
 To start, create a new file called _OrchestratorPlugin.cs_ in the same directory as your semantic functions for `OrchestratorPlugin`.

```directory
Plugins
│
└─── OrchestratorPlugin
|    │
|    └─── GetIntent
|    │    └─── skprompt.txt
|    │    └─── config.json
|    └─── OrchestratorPlugin.cs
|
└─── MathPlugin
     │
     └─── Math.cs
```

# [Python](#tab/python)
To start, create a new file called _OrchestratorPlugin.py_ in the same directory as your semantic functions for `OrchestratorPlugin`.

```directory
Plugins
│
└─── OrchestratorPlugin
|    │
|    └─── GetIntent
|    │    └─── skprompt.txt
|    │    └─── config.json
|    └─── OrchestratorPlugin.py
|
└─── MathPlugin
     │
     └─── Math.py
```
---


### Accessing the kernel within a native function
We can now create the class for our orchestrator plugin where all its native functions will live.

Since this plugin will run other functions, we'll need to pass the kernel to the plugin during initialization. Add the following code to your `Orchestrator` class to get started.


# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/09-Calling-Nested-Functions-in-Native-Functions/plugins/OrchestratorPlugin/Orchestrator.cs" range="3-16,63":::


# [Python](#tab/python)

:::code language="python" source="~/../samples/python/09-Calling-Nested-Functions-in-Native-Functions/plugins/OrchestratorPlugin/Orchestrator.py" range="7-9":::

---

### Creating the `RouteRequest` function
We can now create the `RouteRequest` function which will perform the following steps:
1. Get the user's intent from their request.
2. Call the appropriate math function based on the user's intent.

Add the following code to your `Orchestrator` class to get started creating the routing function.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/09-Calling-Nested-Functions-in-Native-Functions/plugins/OrchestratorPlugin/Orchestrator.cs" range="18-33,40-45,49-50,59-62":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/09-Calling-Nested-Functions-in-Native-Functions/plugins/OrchestratorPlugin/Orchestrator.py" range="11-29,38-40,47-48,58-59":::

---

Notice how we're able to use the readonly `kernel` property of our Orchestrator class to run the `GetIntent` function. Later, we can also use this kernel to run the `Sqrt` and `Multiply` functions.

Unfortunately, however, we have a challenge. Despite knowing the user's intent, we don't know which numbers to pass to the `Sqrt` or `Multiply` functions. We'll need to add _another_ semantic function to the orchestrator plugin to extract the necessary numbers from the user's input.

### Using semantic functions to extract data for native functions
To pull the numbers from the user's input, we'll create a semantic function called `GetNumbers`. Create a new folder under the _OrchestratorPlugin_ folder named _GetNumbers_. Then create a _skprompt.txt_ and _config.json_ file with the following content.

```directory
Plugins
│
└─── OrchestratorPlugin
|    │
|    └─── GetIntent
|    │    └─── skprompt.txt
|    │    └─── config.json
|    └─── GetNumbers
|    │    └─── skprompt.txt
|    │    └─── config.json
|    └─── OrchestratorPlugin.cs
|
└─── MathPlugin
     │
     └─── Math.cs
```

Add the following to the _skprompt.txt_ file:

:::code language="txt" source="~/../samples/dotnet/09-Calling-Nested-Functions-in-Native-Functions/plugins/OrchestratorPlugin/GetNumbers/skprompt.txt":::

Add the following code to the _config.json_ file:

:::code language="json" source="~/../samples/dotnet/09-Calling-Nested-Functions-in-Native-Functions/plugins/OrchestratorPlugin/GetNumbers/config.json":::

This semantic function uses few-shot learning to demonstrate to the LLM how to correctly extract the numbers from the user's request and output them in JSON format. This will allow us to easily pass the numbers to the `Sqrt` and `Multiply` functions.

### Putting it all together
We can now call the `GetNumbers` function from the `RouteRequest` function. Replace the `switch` statement in the `RouteRequest` function with the following code to run the `GetNumbers` function and extract the numbers from the JSON output.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/09-Calling-Nested-Functions-in-Native-Functions/plugins/OrchestratorPlugin/Orchestrator.cs" range="35-61":::

# [Python](#tab/python)
:::code language="python" source="~/../samples/python/09-Calling-Nested-Functions-in-Native-Functions/plugins/OrchestratorPlugin/Orchestrator.py" range="30-58":::

---

### Running the `RouteRequest` function

Finally, you can invoke the `RouteRequest` function from your main file using the code below. Notice how we're passing in the kernel to the `Orchestrator` object when we instantiate it. This will allow the `Orchestrator` object to access the kernel so it can run the `GetIntent`, `GetNumbers`, `Sqrt`, and `Multiply` functions.


# [C#](#tab/Csharp)
:::code language="csharp" source="~/../samples/dotnet/09-Calling-Nested-Functions-in-Native-Functions/Program.cs" range="3-6,14-20,23-41"  highlight="22":::

# [Python](#tab/python)
:::code language="python" source="~/../samples/python/09-Calling-Nested-Functions-in-Native-Functions/main.py" range="1-4,6-13,15-49" highlight="25":::

---

Also notice how in the main file we load the kernel with _all_ the functions that are needed by the `RouteRequest` function. If we do not appropriately load the `GetIntent`, `GetNumbers`, `Sqrt`, and `Multiply` functions, the `RouteRequest` function will fail when it tries to call them.

## Take the next step
You now have the skills necessary to create both semantic and native functions to create custom plugins, but up until now, we've only called one function at a time. In the next section, you'll learn how to chain multiple functions together.

> [!div class="nextstepaction"]
> [Chaining functions](../chaining-functions.md)