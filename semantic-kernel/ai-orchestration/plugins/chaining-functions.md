---
title: How to chain prompts and native functions together
description: Learn how to seamlessly combine multiple functions within Semantic Kernel.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---
# Chaining functions together

[!INCLUDE [pat_large.md](../../includes/pat_large.md)]

In previous articles, we showed how you could invoke a Semantic Kernel function (whether semantic or native) individually. Oftentimes, however, you may want to string multiple functions together into a single pipeline to simplify your code. [In this article](./chaining-functions.md#passing-more-than-just-input-with-native-functions), we'll put this knowledge to use by demonstrating how you could refactor the code from the [calling nested functions](./native-functions/calling-nested-functions.md) article to make it more readable and maintainable.

If you want to see the final solution to this article, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/09-Calling-Nested-Functions-in-Native-Functions)  | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/10-Chaining-Functions) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/09-Calling-Nested-Functions-in-Native-Functions)  |  [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/10-Chaining-Functions) |


## Passing data to semantic functions with `input`
Semantic Kernel was designed in the spirit of UNIX's piping and filtering capabilities. To replicate this behavior, we've added a special variable called `input` into the kernel's context object that allows you to stream output from one semantic function to the next.

![Passing data with $input in Semantic Kernel](../../media/semantic-kernel-chains.png)

For example we can make three inline semantic functions and string their outputs into the next by adding the `input` variable into each prompt.

# [C#](#tab/Csharp)
Create and register the semantic functions.

```csharp
string myJokePrompt = """
Tell a short joke about {{$input}}.
""";
string myPoemPrompt = """
Take this "{{$input}}" and convert it to a nursery rhyme.
""";
string myMenuPrompt = """
Make this poem "{{$input}}" influence the three items in a coffee shop menu. 
The menu reads in enumerated form:

""";

var myJokeFunction = kernel.CreateSemanticFunction(myJokePrompt, maxTokens: 500);
var myPoemFunction = kernel.CreateSemanticFunction(myPoemPrompt, maxTokens: 500);
var myMenuFunction = kernel.CreateSemanticFunction(myMenuPrompt, maxTokens: 500);
```

Run the functions sequentially. Notice how all of the functions share the same context.

```csharp
var context = kernel.CreateNewContext("Charlie Brown");
await myJokeFunction.InvokeAsync(context);
await myPoemFunction.InvokeAsync(context);
await myMenuFunction.InvokeAsync(context);

Console.WriteLine(context);
```

# [Python](#tab/python)

Create and register the semantic functions.

```python
my_joke_prompt = """Tell a short joke about {{$INPUT}}."""
my_poem_prompt = """Take this "{{$INPUT}}" and convert it to a nursery rhyme."""
my_menu_prompt = """Make this poem "{{$INPUT}}" influence the three items in a coffee shop menu. 
The menu reads in enumerated form:
"""
```

Run the functions sequentially. Notice how all of the functions share the same context.

```python
my_joke_function = kernel.create_semantic_function(myJokePrompt, max_tokens=500)
my_poem_function = kernel.create_semantic_function(myPoemPrompt, max_tokens=500)
my_menu_function = kernel.create_semantic_function(myMenuPrompt, max_tokens=500)

context = kernel.create_new_context("Charlie Brown")
await my_joke_function(context=context)
await my_poem_function(context=context)
await my_menu_function(context=context)

print(context)
```

---

Which would result in something like:

```output
1. Colossus of Memnon Latte - A creamy latte with a hint of sweetness, just like the awe-inspiring statue.

2. Gasp and Groan Mocha - A rich and indulgent mocha that will make you gasp and groan with delight.

3. Heart Skipping a Beat Frappuccino - A refreshing frappuccino with a hint of sweetness that will make your heart skip a beat.
```

### Using the `RunAsync` method to simplify your code
Running each function individually can be very verbose, so Semantic Kernel also provides the `RunAsync` method in C# or `run_async` method in Python that automatically calls a series of functions sequentially, all with the same context object. 

# [C#](#tab/Csharp)
```csharp
var myOutput = await kernel.RunAsync(
    new ContextVariables("Charlie Brown"),
    myJokeFunction,
    myPoemFunction,
    myMenuFunction);

Console.WriteLine(myOutput);
```

# [Python](#tab/python)

```python
myOutput = await kernel.run_async(
    my_joke_function,
    my_poem_function, 
    my_menu_function,
    input_str = input_text)

print(myOutput)
```

---

## Passing more than just `input` with native functions
In the previous articles, we've already seen how you can update and retrieve additional properties from the context object within native functions. We can use this same technique to pass additional data between functions within a pipeline.

We'll demonstrate this by updating the code written in the [calling nested functions](./native-functions/calling-nested-functions.md) article to use the `RunAsync` method with multiple functions. Use the link to the previous completed solution at the top of the page if you want to follow along.

### Adding a function that changes variables during runtime
In the previous example, we used the `RouteRequest` function to individually call each of the Semantic Kernel functions, and in between calls, we manually updated the variables before running the next function.

We can simplify this code by creating a new native function that performs the same context update operations as part of a chain. We'll call this function `ExtractNumbersFromJson` and it will take the JSON string from the `input` variable and extract the numbers from it into the context object.

Add the following code to your `OrchestratorPlugin` class.

# [C#](#tab/Csharp)
:::code language="csharp" source="~/../samples/dotnet/10-Chaining-Functions/plugins/OrchestratorPlugin/Orchestrator.cs" range="80-97":::

# [Python](#tab/python)
:::code language="python" source="~/../samples/python/10-Chaining-Functions/plugins/OrchestratorPlugin/Orchestrator.py" range="75-91":::

---

### Using the `RunAsync` method to chain our functions
Now that we have a function that can extracts numbers, we can update our `RouteRequest` function to use the `RunAsync` method to call the functions in a pipeline. Update the `RouteRequest` function to the following. Notice how we can now call all of our functions in a single call to `RunAsync`.

# [C#](#tab/Csharp)
:::code language="csharp" source="~/../samples/dotnet/10-Chaining-Functions/plugins/OrchestratorPlugin/Orchestrator.cs" range="19-62,77-78" highlight="36-42":::

# [Python](#tab/python)
:::code language="python" source="~/../samples/python/10-Chaining-Functions/plugins/OrchestratorPlugin/Orchestrator.py" range="13-58,73" highlight="40-45":::

---

After making these changes, you should be able to run the code again and see the same results as before. Only now, the `RouteRequest` is easier to read and you've created a new native function that can be reused in other pipelines.

## Starting a pipeline with additional context variables
So far, we've only passed in a string to the `RunAsync` method. You can, however, also pass in a context object to start the pipeline with additional information.

This is helpful because it can allow us to persist the initial `$input` variable across all functions in the pipeline without it being overwritten. For example, in our current pipeline, the user's original request is overwritten by the output of the `GetNumbers` function. This makes it difficult to retrieve the original request later in the pipeline to create a natural sounding response. By storing the original request as another variable, we can retrieve it later in the pipeline.


### Passing a context object to `RunAsync`
To pass a context object to `RunAsync`, you can create a new context object and pass it as the first parameter. This will start the pipeline with the variables in the context object. We'll be creating a new variable called `original_request` to store the original request. Later, we'll show where to add this code in the `RouteRequest` function.

# [C#](#tab/Csharp)
:::code language="csharp" source="~/../samples/dotnet/10-Chaining-Functions/plugins/OrchestratorPlugin/Orchestrator.cs" range="65-66":::


# [Python](#tab/python)
:::code language="python" source="~/../samples/python/10-Chaining-Functions/plugins/OrchestratorPlugin/Orchestrator.py" range="59-62":::

---

### Creating a semantic function that uses the new context variables
Now that we have a variable with the original request, we can use it to create a more natural sounding response. We'll create a new semantic function called `CreateResponse` that will use the `original_request` variable to create a response in the `OrchestratorPlugin`.

Start by creating a new folder called _CreateResponse_ in your _OrchestratorPlugin_ folder. Then create the _config.json_ and _skprompt.txt_ files and paste the following code into the _config.json_ file. Notice how we now have two input variables, `input` and `original_request`.


:::code language="json" source="~/../samples/dotnet/10-Chaining-Functions/plugins/OrchestratorPlugin/CreateResponse/config.json":::

Next, copy and paste the following prompt into _skprompt.txt_.


:::code language="txt" source="~/../samples/dotnet/10-Chaining-Functions/plugins/OrchestratorPlugin/CreateResponse/skprompt.txt":::

You can now update the `RouteRequest` function to include the `CreateResponse` function in the pipeline. Update the `RouteRequest` function to the following:


# [C#](#tab/Csharp)
:::code language="csharp" source="~/../samples/dotnet/10-Chaining-Functions/plugins/OrchestratorPlugin/Orchestrator.cs" range="19-53,63-66,55,69-78":::

# [Python](#tab/python)
:::code language="python" source="~/../samples/python/10-Chaining-Functions/plugins/OrchestratorPlugin/Orchestrator.py" range="13-50,59-73":::

---

### Testing the new pipeline
Now that we've updated the pipeline, we can test it out. Run the following code in your main file.

# [C#](#tab/Csharp)
:::code language="csharp" source="~/../samples/dotnet/10-Chaining-Functions/program.cs" range="4-7,16-21,24-42":::

# [Python](#tab/python)
:::code language="python" source="~/../samples/python/10-Chaining-Functions/main.py" range="1-4,6-13,15-53":::

---

You should get a response like the following. Notice how the response is now more natural sounding.

```output
The square root of 524 is 22.891046284519195.
The room would be approximately 212.2925 square feet.
```

## Take the next step
You are now becoming familiar with orchestrating both semantic and non-semantic functions. Up until now, however, you've had to manually orchestrate the functions. In the next section, you'll learn how to use planner to orchestrate functions automatically.

> [!div class="nextstepaction"]
> [Automatically create chains with planners](../planners/index.md)
