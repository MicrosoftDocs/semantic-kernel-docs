---
title: How to add native code to your AI apps with Semantic Kernel
description: Learn how to write native functions inside of AI plugins for Semantic Kernel.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: mssearch
---

# Adding native functions to the kernel

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

In the [how to create semantic functions](./semantic-functions.md) article, we showed how you could create a semantic function that retrieves a user's intent, but what do you do once you have the intent? In this article we'll show how to create native functions that can route the intent and perform a task.

As an example, we'll add an additional function to the `OrchestratorPlugin`  we created in the [semantic functions](./semantic-functions.md) article to route the user's intent. We'll also add a new plugin called `MathPlugin` that will perform simple arithmetic for the user.

By the end of this article, you'll have a kernel that can correctly answer user questions like `What is the square root of 634?` and `What is 42 plus 1513?`. If you want to see the final solution, you can check out the following samples in the public documentation repository.

| Language  | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/02-Native-Functions) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/02-Native-Functions) |


> [!Note]
> Skills are currently being renamed to plugins. This article has been updated to reflect the latest terminology, but some images and code samples may still refer to skills.

## Finding a home for your native function

You can place plugins in the same directory as the other plugins. For example, to create functions for a plugin called `MyNewPlugin`, you can create a new file called _MyCSharpPlugin.cs_ in the same directory as your semantic functions.

```directory
MyPluginsDirectory
│
└─── MyNewPlugin
     │
     └─── MyFirstSemanticFunction
     │    └─── skprompt.txt
     │    └─── config.json
     └─── MyOtherSemanticFunctions
     |    | ...  
     │
     └─── MyNewPlugin.cs
```

In our example, we'll create two files one for the native `OrchestratorPlugin` functions and another for the `MathPlugin`. Depending on the language you're using, you'll create either C# or Python files for each.

# [C#](#tab/Csharp)

```directory
Plugins
│
└─── OrchestratorPlugin
|    │
|    └─── GetIntent
|         └─── skprompt.txt
|         └─── config.json
|         └─── OrchestratorPlugin.cs
|
└─── MathPlugin
     │
     └─── MathPlugin.cs
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
|         └─── OrchestratorPlugin.py
|
└─── MathPlugin
     │
     └─── MathPlugin.py
```
---

It's ok if you have a plugin folder with native functions and semantic functions. The kernel will load both functions into the same plugin namespace. What's important is that you don't have two functions with the same name within the same plugin namespace. If you do, the last function loaded will overwrite the previous function.

We'll begin by creating the `MathPlugin` functions. Afterwards, we'll call the `MathPlugin` functions from within the `OrchestratorPlugin`. At the end of this example you will have the following supported functions.

| Plugin | Function | Type | Description |
| --- | --- | --- | --- |
| OrchestratorPlugin | GetIntent | Semantic | Gets the intent of the user |
| OrchestratorPlugin | GetNumbers | Semantic | Gets the numbers from a user's request |
| OrchestratorPlugin | RouteRequest | Native | Routes the request to the appropriate function |
| MathPlugin | Sqrt | Native | Takes the square root of a number |
| MathPlugin | Add | Native | Adds two numbers together |

## Creating simple native functions
Open up the _MathPlugin.cs_ or _MathPlugin.py_ file you created earlier and follow the instructions below to create the two necessary functions: `Sqrt` and `Add`.

### Defining a function that takes a single input
Add the following code to your file to create a function that takes the square root of a number.

# [C#](#tab/Csharp)

```csharp
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Orchestration;

namespace Plugins;

public class MathPlugin
{
    [SKFunction, Description("Takes the square root of a number")]
    public string Sqrt(string number)
    {
        return Math.Sqrt(Convert.ToDouble(number)).ToString();
    }
}
```

# [Python](#tab/python)

```python
import math
from semantic_kernel.skill_definition import (
    sk_function,
    sk_function_context_parameter,
)
from semantic_kernel.orchestration.sk_context import SKContext


class MathPlugin:
    @sk_function(
        description="Takes the square root of a number",
        name="square_root",
        input_description="The value to take the square root of",
    )
    def square_root(self, number: str) -> str:
        return str(math.sqrt(float(number)))
```
---

Notice that the input and and return types are strings. This is because the kernel will pass the input as a string and expect a string to be returned. You can convert the input to any type you want within the function.

Also notice how we've added a description to each function with attributes. This description will be used by the [planner](./planner.md) to automatically create a plan using these functions. In our case, we're telling planner that this function `Takes the square root of a number`.

### Using context parameters to take multiple inputs
Adding numbers together requires multiple numbers as input. Since we cannot pass multiple numbers into a native function, we'll need to use context parameters instead.

Add the following code to your `MathPlugin` class to create a function that adds two numbers together.

# [C#](#tab/Csharp)

```csharp
[SKFunction, Description("Adds two numbers together")]
[SKParameter("input", "The first number to add")]
[SKParameter("number2", "The second number to add")]
public string Add(SKContext context)
{
    return (
        Convert.ToDouble(context["input"]) + Convert.ToDouble(context["number2"])
    ).ToString();
}
```

# [Python](#tab/python)

```python
@sk_function(
    description="Adds two numbers together",
    name="add",
)
@sk_function_context_parameter(
    name="input",
    description="The first number to add",
)
@sk_function_context_parameter(
    name="number2",
    description="The second number to add",
)
def add(self, context: SKContext) -> str:
    return str(float(context["input"]) + float(context["number2"]))
```
---

Notice that instead of taking a string as input, this function takes an `SKContext` object as input. This object contains all of the variables in the Semantic Kernel's context. We can use this object to retrieve the two numbers we want to add. Also notice how we provide descriptions for each of the context parameters. These descriptions will be used by the [planner](./planner.md) to automatically provide inputs to this function.

The `SKContext` object only supports strings, so we'll need to convert the strings to doubles before we add them together.

### Running your native functions
You can now run your functions using the code below. Notice how we pass in the multiple numbers required for the `Add` function using a `SKContext` object.

# [C#](#tab/Csharp)

```csharp
using Microsoft.SemanticKernel;
using Plugins;

// ... instantiate your kernel

var mathPlugin = kernel.ImportSkill(new MathPlugin(), "MathPlugin");

// Run the Sqrt function
var result1 = await mathPlugin["Sqrt"].InvokeAsync("64");
Console.WriteLine(result1);

// Run the Add function with multiple inputs
var context = kernel.CreateNewContext();
context["input"] = "3";
context["number2"] = "7";
var result2 = await mathPlugin["Add"].InvokeAsync(context);
Console.WriteLine(result2);
```

# [Python](#tab/python)

```python
import semantic_kernel as sk
from plugins.MathPlugin.MathPlugin import MathPlugin

async def main():
    # Instantiate your kernel and register your skill
    kernel = sk.Kernel()
    math_plugin = kernel.import_skill(MathPlugin(), skill_name="math_plugin")
    sqrt = math_plugin["square_root"]
    add = math_plugin["add"]

    # Run the square root function
    print(await sqrt.invoke_async(64))

    # Run the add function with multiple inputs
    context = kernel.create_new_context()
    context["input"] = "3"
    context["number2"] = "7"
    print(await add.invoke_async(context=context))

# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
```
---

The code should output `8` since it's the square root of `64` and `10` since it's the sum of `3` and `7`.

## Creating a more complex native function
We can now create the native routing function for the `OrchestratorPlugin`. This function will be responsible for calling the right `MathPlugin` function based on the user's request.

### Calling Semantic Kernel functions within a native function
In order to call the right `MathPlugin` function, we'll use the `GetIntent` semantic function we defined in the [previous tutorial](./semantic-functions.md). Add the following code to your `OrchestratorPlugin` class to get started creating the routing function.

# [C#](#tab/Csharp)

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Newtonsoft.Json.Linq;

namespace Plugins;

public class OrchestratorPlugin
{
    IKernel _kernel;

    public OrchestratorPlugin(IKernel kernel)
    {
        _kernel = kernel;
    }

    [SKFunction, Description("Routes the request to the appropriate function.")]
    public async Task<string> RouteRequest(SKContext context)
    {
        // Save the original user request
        string request = context["input"];

        // Add the list of available functions to the context
        context["options"] = "Sqrt, Add";

        // Retrieve the intent from the user request
        var GetIntent = _kernel.Skills.GetFunction("OrchestratorPlugin", "GetIntent");
        await GetIntent.InvokeAsync(context);
        string intent = context["input"].Trim();

        // Call the appropriate function
        switch (intent)
        {
            case "Sqrt":
                // Call the Sqrt function
            case "Add":
                // Call the Add function
            default:
                return "I'm sorry, I don't understand.";
        }
    }
}
```

# [Python](#tab/python)

```python
import json
from semantic_kernel import Kernel
from semantic_kernel.skill_definition import (sk_function)
from semantic_kernel.orchestration.sk_context import SKContext


class OrchestratorPlugin:
    def __init__(self, kernel: Kernel):
        self._kernel = kernel

    @sk_function(
        description="Routes the request to the appropriate function",
        name="route_request",
    )
    async def RouteRequest(self, context: SKContext) -> str:
        # Save the original user request
        request = context["input"]

        # Add the list of available functions to the context
        context["options"] = "Sqrt, Add"

        # Retrieve the intent from the user request
        GetIntent = self._kernel.skills.get_function("OrchestratorPlugin", "GetIntent")
        GetIntent(context=context)
        intent = context["input"].strip()

        # Call the appropriate function
        if intent == "Sqrt":
            # Call the sqrt function
        elif intent == "Add":
            # Call the add function
        else:
            return "I'm sorry, I don't understand."

```
---

From the code above, we can see that the `OrchestratorPlugin` function does the following:
1. Saves the kernel to a private variable during initialization so it can be used later to call the `GetIntent` function.
2. Sets the list of available functions to the context so it can be passed to the `GetIntent` function.
3. Uses a `switch` statement to call the appropriate function based on the user's intent.

Unfortunately, we have a challenge. Despite knowing the user's intent, we don't know which numbers to pass to the `MathPlugin` functions. We'll need to add another semantic function to the `OrchestratorPlugin` to extract the necessary numbers from the user's input.

### Using semantic functions to extract data for native functions
To pull the numbers from the user's input, we'll create a semantic function called `GetNumbers`. Create a new folder under the _OrchestratorPlugin_ folder named _GetNumbers_. Then create a _skprompt.txt_ and _config.json_ file within the folder. Add the following code to the _skprompt.txt_ file.

```txt
Extract the numbers from the input and output them in JSON format.

-------------------

INPUT: Take the square root of 4
OUTPUT: {"number1":4}

INPUT: Subtract 3 dollars from 2 dollars
OUTPUT: {"number1":2,"number2":3}

INPUT: I have a 2x4 that is 3 feet long. Can you cut it in half?
OUTPUT: {"number1":3, "number2":2}

INPUT: {{$input}}
OUTPUT: 
```

Add the following code to the _config.json_ file.

```json
{
     "schema": 1,
     "type": "completion",
     "description": "Gets the numbers from a user's request.",
     "completion": {
          "max_tokens": 500,
          "temperature": 0.0,
          "top_p": 0.0,
          "presence_penalty": 0.0,
          "frequency_penalty": 0.0
     },
     "input": {
          "parameters": [
               {
               "name": "input",
               "description": "The user's request.",
               "defaultValue": ""
               }
          ]
     }
}
```

### Putting it all together
We can now call the `GetNumbers` function from the `OrchestratorPlugin` function. Replace the `switch` statement in the `OrchestratorPlugin` function with the following code.

# [C#](#tab/Csharp)

```csharp
var GetNumbers = _kernel.Skills.GetFunction("OrchestratorPlugin", "GetNumbers");
SKContext getNumberContext = await GetNumbers.InvokeAsync(request);
JObject numbers = JObject.Parse(getNumberContext["input"]);

// Call the appropriate function
switch (intent)
{
    case "Sqrt":
        // Call the Sqrt function with the first number
        var Sqrt = _kernel.Skills.GetFunction("MathPlugin", "Sqrt");
        SKContext sqrtResults = await Sqrt.InvokeAsync(numbers["number1"]!.ToString());

        return sqrtResults["input"];
    case "Add":
        // Call the Add function with both numbers
        var Add = _kernel.Skills.GetFunction("MathPlugin", "Add");
        context["input"] = numbers["number1"]!.ToString();
        context["number2"] = numbers["number2"]!.ToString();
        SKContext addResults = await Add.InvokeAsync(context);

        return addResults["input"];
    default:
        return "I'm sorry, I don't understand.";
}
```

# [Python](#tab/python)

```python
GetNumbers = self._kernel.skills.get_function(
    "OrchestratorPlugin", "GetNumbers"
)
getNumberContext = GetNumbers(request)
numbers = json.loads(getNumberContext["input"])

# Call the appropriate function
if intent == "Sqrt":
    # Call the Sqrt function with the first number
    square_root = self._kernel.skills.get_function("MathPlugin", "square_root")
    sqrtResults = await square_root.invoke_async(numbers["number1"])

    return sqrtResults["input"]
elif intent == "Add":
    # Call the Add function with both numbers
    add = self._kernel.skills.get_function("MathPlugin", "add")
    context["input"] = numbers["number1"]
    context["number2"] = numbers["number2"]
    addResults = await add.invoke_async(context=context)

    return addResults["input"]
else:
    return "I'm sorry, I don't understand."
```
---

Finally, you can invoke the `OrchestratorPlugin` function from your main file using the code below.


# [C#](#tab/Csharp)

```csharp
using Microsoft.SemanticKernel;
using Plugins;

// ... instantiate your kernel

var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins");

// Import the semantic functions
kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "OrchestratorPlugin");
kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "SummarizeSkill");

// Import the native functions 
var mathPlugin = kernel.ImportSkill(new MathPlugin(), "MathPlugin");
var orchestratorPlugin = kernel.ImportSkill(new OrchestratorPlugin(kernel), "OrchestratorPlugin");

// Make a request that runs the Sqrt function
var result1 = await orchestratorPlugin["RouteRequest"].InvokeAsync("What is the square root of 634?");
Console.WriteLine(result1);

// Make a request that runs the Add function
var result2 = await orchestratorPlugin["RouteRequest"].InvokeAsync("What is 42 plus 1513?");
Console.WriteLine(result2);

```

# [Python](#tab/python)

```python
import semantic_kernel as sk
from semantic_kernel.connectors.ai.open_ai import (
    OpenAITextCompletion,
    AzureChatCompletion,
)
from plugins.OrchestratorPlugin.OrchestratorPlugin import OrchestratorPlugin
from plugins.MathPlugin.MathPlugin import MathPlugin


async def main():
    # ... Instantiate your kernel

    pluginsDirectory = "./plugins"

    # Import the semantic functions
    kernel.import_semantic_skill_from_directory(pluginsDirectory, "OrchestratorPlugin")
    kernel.import_semantic_skill_from_directory(pluginsDirectory, "SummarizeSkill")

    # Import the native functions
    mathPlugin = kernel.import_skill(MathPlugin(), "MathPlugin")
    orchestratorPlugin = kernel.import_skill(
        OrchestratorPlugin(kernel), "OrchestratorPlugin"
    )

    # Make a request that runs the Sqrt function
    result1 = await orchestratorPlugin["route_request"].invoke_async(
        "What is the square root of 634?"
    )
    print(result1["input"])

    # Make a request that runs the Add function
    result2 = await orchestratorPlugin["route_request"].invoke_async(
        "What is 42 plus 1513?"
    )
    print(result2["input"])


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
```
---

## Take the next step
You now have the skills necessary to create both semantic and native functions to create custom plugins, but up until now, we've only called one function at a time. In the next article, you'll learn how to chain multiple functions together.

> [!div class="nextstepaction"]
> [Chaining functions](./chaining-functions.md)