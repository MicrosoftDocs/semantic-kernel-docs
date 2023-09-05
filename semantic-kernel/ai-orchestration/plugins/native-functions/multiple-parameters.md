---
title: Using multiple variables in native functions
description: Consume and output multiple variables using Semantic Kernel context.
author: matthewbolanos
ms.topic: tutorial
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Using multiple inputs in native functions


[!INCLUDE [pat_large.md](../../../includes/pat_large.md)]

In the [previous tutorial](./using-the-SKFunction-decorator.md), we demonstrated how to use the `SKFunction` decorator to create a native function that performs a square root operation. For other mathematical operations, however, we'll need to use _multiple_ inputs, so in this tutorial, we'll demonstrate how to use and consume `ContextVariables` objects within native functions.

If you want to see the final solution to this article, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/07-Simple-Native-Functions) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/08-Native-Functions-with-Context) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/07-Simple-Native-Functions) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/08-Native-Functions-with-Context) |


## Using context parameters to pass multiple inputs
Adding numbers together requires multiple numbers as input. To achieve this, we'll need to use context parameters.

Update your `Math` class so that it also includes a function that multiplies two numbers together. Instead of accepting a string as an input, we'll create a method signature that accepts an `SKContext` object as an input. This object contains all of the inputs passed into the function.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/08-Native-Functions-with-Context/plugins/MathPlugin/Math.cs" range="3-28" highlight="19":::

# [Python](#tab/python)
TODO: Add python

---

To access the multiple inputs within the `SKContext` object, we can use the `Variables` property. This property is a dictionary that contains all of the inputs passed into the function. We can then use these inputs to perform the multiplication. 

Also notice how we are now using the `SKParameter` decorator to define the two inputs of the function. The first parameter is the name of the parameter and the second is the description of the parameter. Both are used by the [planner](../planner.md) to automatically provide inputs to this function.


### Running your native functions
You can now run your functions using the code below. Notice how we pass in the multiple numbers required for the `Multiply` function into the kernel using a `ContextVariables` object. This object is what populates the `Variables` property of the `SKContext` object.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/08-Native-Functions-with-Context/program.cs" range="3-6,14-19,22-36" highlight="17-22":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/08-Native-Functions-with-Context/main.py" range="1-2,4-11,13-34" highlight="15-17":::

---

The code should output `700.6652` since that's the product of `12.34` and `56.78`.

## Take the next step
Now that you can pass in multiple parameters, you can now learn how to call other functions _within_ a native function using the kernel.

> [!div class="nextstepaction"]
> [Calling nested functions in native functions](./calling-nested-functions.md)