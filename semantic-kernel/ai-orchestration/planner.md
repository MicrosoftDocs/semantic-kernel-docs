---
title: How to automatically orchestrate AI with planner
description: Learn how to automatically create chains of functions with planner.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: mssearch
---

# Automatically orchestrate AI with planner

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

So far, we have manually orchestrated all of the functions on behalf of the user. This, however, is not a scalable solution because it would require the app developer to predict all possible requests that could be made by the user. So instead, we will learn how to automatically orchestrate functions on the fly using planner. If you want to see the final solution, you can check out the following samples in the public documentation repository.

| Language  | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/04-Planner) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/04-Planner) |


## What is planner?
Planner is a function that takes a user's ask and returns back a plan on how to accomplish the request. It does so by using AI to mix-and-match the plugins registered in the kernel so that it can recombine them into a series of steps that complete a goal.

:::row:::
   :::column span="2":::
      This is a powerful concept because it allows you to create atomic functions that can be used in ways that you as a developer may not have thought of.
      
      For example, if you had task and calendar event plugins, planner could combine them to create workflows like "remind me to buy milk when I go to the store" or "remind me to call my mom tomorrow" without you explicitly having to write code for those scenarios.
   :::column-end:::
   :::column span="3":::
        ![Planner automatically combines functions](../media/the-planner.png)
   :::column-end:::
:::row-end:::


With great power comes great responsibility, however. Because planner can combine functions in ways that you may not have thought of, it is important to make sure that you only expose functions that you want to be used in this way. It's also important to make sure that you apply [responsible AI](https://www.microsoft.com/en-us/ai/responsible-ai) principles to your functions so that they are used in a way that is fair, reliable, safe, private, and secure.

Planner is an extensible part of Semantic Kernel. This means we have several planners to choose from and that you could create a custom planner if you had specific needs. Below is a table of the out-of-the-box planners provided by Semantic Kernel and their language support. The ❌ symbol indicates that the feature is not yet available in that language; if you would like to see a feature implemented in a language, please consider [contributing to the project](../get-started/contributing.md) or [opening an issue](../get-started/contributing.md#reporting-issues). 

| Planner | Description | C# | Python | Java |
| --- | --- | :------:|:----: | :----: |
| BasicPlanner                      | A simplified version of SequentialPlanner that strings together a set of functions. | ❌ | ✅ | ❌ |
| ActionPlanner                     | Creates a plan with a single step. | ✅ | ❌ | ❌ |
| SequentialPlanner                 | Creates a plan with a series of steps that are interconnected with custom generated input and output variables. | ✅ | ❌ | ❌ |

## Testing out planner
For the purposes of this article, we'll build upon the same code we wrote in the [previous section](./chaining-functions.md). Only this time, instead of relying on our own `OrchestratorPlugin` to chain the `MathPlugin` functions, we'll use planner to do it for us!

At the end of this section, we'll have built a natural language calculator that can answer simple word problems for users.

### Adding more functions to `MathPlugin`
Before we use planner, let's add a few more functions to our `MathPlugin` class so we can have more options for our planner to choose from. The following code adds a `Subtract`, `Multiply`, and `Divide` function to our plugin.

# [C#](#tab/Csharp)
```csharp
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Plugins;

public class MathPlugin
{
  [SKFunction, Description("Take the square root of a number")]
  public string Sqrt(string input)
  {
      return Math.Sqrt(Convert.ToDouble(input, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
  }

  [SKFunction, Description("Add two numbers")]
  [SKParameter("input", "The first number to add")]
  [SKParameter("number2", "The second number to add")]
  public string Add(SKContext context)
  {
      return (
          Convert.ToDouble(context["input"], CultureInfo.InvariantCulture) +
          Convert.ToDouble(context["number2"], CultureInfo.InvariantCulture)
      ).ToString(CultureInfo.InvariantCulture);
  }

  [SKFunction, Description("Subtract two numbers")]
  [SKParameter("input", "The first number to subtract from")]
  [SKParameter("number2", "The second number to subtract away")]
  public string Subtract(SKContext context)
  {
      return (
          Convert.ToDouble(context["input"], CultureInfo.InvariantCulture) -
          Convert.ToDouble(context["number2"], CultureInfo.InvariantCulture)
      ).ToString(CultureInfo.InvariantCulture);
  }

  [SKFunction, Description("Multiply two numbers. When increasing by a percentage, don't forget to add 1 to the percentage.")]
  [SKParameter("input", "The first number to multiply")]
  [SKParameter("number2", "The second number to multiply")]
  public string Multiply(SKContext context)
  {
      return (
          Convert.ToDouble(context["input"], CultureInfo.InvariantCulture) *
          Convert.ToDouble(context["number2"], CultureInfo.InvariantCulture)
      ).ToString(CultureInfo.InvariantCulture);
  }

  [SKFunction, Description("Divide two numbers")]
  [SKParameter("input", "The first number to divide from")]
  [SKParameter("number2", "The second number to divide by")]
  public string Divide(SKContext context)
  {
      return (
          Convert.ToDouble(context["input"], CultureInfo.InvariantCulture) /
          Convert.ToDouble(context["number2"], CultureInfo.InvariantCulture)
      ).ToString(CultureInfo.InvariantCulture);
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
    
    @sk_function(
        description="Subtract two numbers",
        name="subtract",
    )
    @sk_function_context_parameter(
        name="input",
        description="The first number to subtract from",
    )
    @sk_function_context_parameter(
        name="number2",
        description="The second number to subtract away",
    )
    def subtract(self, context: SKContext) -> str:
        return str(float(context["input"]) - float(context["number2"]))

    @sk_function(
        description="Multiply two numbers",
        name="multiply",
    )
    @sk_function_context_parameter(
        name="input",
        description="The first number to multiply",
    )
    @sk_function_context_parameter(
        name="number2",
        description="The second number to multiply",
    )
    def multiply(self, context: SKContext) -> str:
        return str(float(context["input"]) * float(context["number2"]))

    @sk_function(
        description="Divide two numbers",
        name="divide",
    )
    @sk_function_context_parameter(
        name="input",
        description="The first number to divide from",
    )
    @sk_function_context_parameter(
        name="number2",
        description="The second number to divide by",
    )
    def divide(self, context: SKContext) -> str:
        return str(float(context["input"]) / float(context["number2"]))
```
---

### Instantiating planner
To instantiate planner, all you need to do is pass it a kernel object. Planner will then automatically discover all of the plugins registered in the kernel and use them to create plans. The following code initializes both a kernel and a `SequentialPlanner`. At the end of this article we'll review the other types of Planners that are available in Semantic Kernel.

# [C#](#tab/Csharp)
```csharp
using Microsoft.SemanticKernel;
using Plugins;

// ... instantiate your kernel

// Add the math plugin
var mathPlugin = kernel.ImportSkill(new MathPlugin(), "MathPlugin");

// Create planner
var planner = new SequentialPlanner(kernel);
```

# [Python](#tab/python)

```python
import semantic_kernel as sk
from plugins.MathPlugin.MathPlugin import MathPlugin
from semantic_kernel.planning.basic_planner import BasicPlanner


async def main():
    # ... Initialize the kernel

    kernel = sk.Kernel()
    math_plugin = kernel.import_skill(MathPlugin(), skill_name="math_plugin")

    planner = BasicPlanner()

if __name__ == "__main__":
    import asyncio

    asyncio.run(main())

```

---

### Creating and running a plan
Now that we have planner, we can use it to create a plan for a user's ask and then invoke the plan to get a result. The following code asks our planner to solve a math problem that is difficult for an LLM to solve on its own because it requires multiple steps and it has numbers with decimal points.

# [C#](#tab/Csharp)
```csharp
// Create a plan for the ask
var ask = "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent $5 on a latte?";
var plan = await planner.CreatePlanAsync(ask);

// Execute the plan
var result = await plan.InvokeAsync();

Console.WriteLine("Plan results:");
Console.WriteLine(result.Result);
```

# [Python](#tab/python)

```python
ask = "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent $5 on a latte?"
plan = await planner.create_plan_async(ask, kernel)

# Execute the plan
result = await planner.execute_plan_async(plan, kernel)

print("Plan results:")
print(result)
```

---

After running this code, you should get the correct answer of `2615.1829` back, but how?

## How does planner work?
Behind the scenes, planner uses an LLM prompt to generate a plan. You can see the prompt that is used by `SequentialPlanner` by navigating to the [_skprompt.txt_ file](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Extensions/Planning.SequentialPlanner/skprompt.txt) in the Semantic Kernel repository. You can also view the prompt used by the [basic planner](https://github.com/microsoft/semantic-kernel/blob/main/python/semantic_kernel/planning/basic_planner.py) in Python.

### Understanding the prompt powering planner

The first few lines of the prompt are the most important to understanding how planner works. They look like this:

```txt
Create an XML plan step by step, to satisfy the goal given.
To create a plan, follow these steps:
0. The plan should be as short as possible.
1. From a <goal> create a <plan> as a series of <functions>.
2. Before using any function in a plan, check that it is present in the most recent [AVAILABLE FUNCTIONS] list. If it is not, do not use it. Do not assume that any function that was previously defined or used in another plan or in [EXAMPLES] is automatically available or compatible with the current plan.
3. Only use functions that are required for the given goal.
4. A function has a single 'input' and a single 'output' which are both strings and not objects.
5. The 'output' from each function is automatically passed as 'input' to the subsequent <function>.
6. 'input' does not need to be specified if it consumes the 'output' of the previous function.
7. To save an 'output' from a <function>, to pass into a future <function>, use <function.{FunctionName} ... setContextVariable: "<UNIQUE_VARIABLE_KEY>"/>
8. To save an 'output' from a <function>, to return as part of a plan result, use <function.{FunctionName} ... appendToResult: "RESULT__<UNIQUE_RESULT_KEY>"/>
9. Append an "END" XML comment at the end of the plan.
```

With these steps, planner is given a set of rules that it can use to generate a plan in XML. Afterwards, the prompt provides a few examples of valid plans before finally providing the `$available_functions` and user's goal.

```txt
[AVAILABLE FUNCTIONS]

{{$available_functions}}

[END AVAILABLE FUNCTIONS]

<goal>{{$input}}</goal>
```

## Giving planner the _best_ data
When you render the prompt, one of the main things you might notice is that all of the descriptions we provided for our functions are included in the prompt. For example, the description for `MathPlugin.Add` is included in the prompt as `Add two numbers`.

```txt
[AVAILABLE FUNCTIONS]

MathPlugin.Add:
  description: Add two numbers
  inputs:
    - input: The first number to add
  - number2: The second number to add

MathPlugin.Divide:
  description: Divide two numbers
  inputs:
    - input: The first number to divide from
  - number2: The second number to divide by
```

Because of this, it's incredibly important to provide the best descriptions you can for your functions. If you don't, planner will not be able to generate a plan that uses your functions correctly.

You can also use the descriptions to provide explicit instructions to the model on how to use your functions. Below are some techniques you can use to improve the use of your functions by planner.

- **Provide help text** – It's not always clear when or how to use a function, so giving advice helps. For example, the description for `MathPlugin.Multiply` reminds the bot to add 1 whenever it increases a number by a percentage.
- **Describe the output.** – While there is not an explicit way to tell planner what the output of a function is, you can describe the output in the description.
- **State if inputs are required.** – If a function requires an input, you can state that in the input's description so the model knows to provide an input. Conversely, you can tell the model that an input is optional so it knows it can skip it if necessary.


### Viewing the plan produced by planner
Because the plan is returned as plain text (either as XML or JSON), we can print the results to inspect what plan planner actually created. The following code shows how to print the plan and the output for C# and Python.

# [C#](#tab/Csharp)
```csharp
Console.WriteLine(plan);
```

```output
{
  "state": [
    {
      "Key": "INPUT",
      "Value": ""
    }
  ],
  "steps": [
    {
      "state": [
        {
          "Key": "INPUT",
          "Value": ""
        }
      ],
      "steps": [],
      "parameters": [
        {
          "Key": "number2",
          "Value": "1.23"
        },
        {
          "Key": "INPUT",
          "Value": "2130.23"
        }
      ],   
      "outputs": [
        "INVESTMENT_INCREASE"
      ],
      "next_step_index": 0,
      "name": "Multiply",
      "skill_name": "MathPlugin",
      "description": "Multiply two numbers"
    },
    {
      "state": [
        {
          "Key": "INPUT",
          "Value": ""
        }
      ],
      "steps": [],
      "parameters": [
        {
          "Key": "number2",
          "Value": "5"
        },
        {
          "Key": "INPUT",
          "Value": "$INVESTMENT_INCREASE"
        }
      ],
      "outputs": [
        "RESULT__FINAL_AMOUNT"
      ],
      "next_step_index": 0,
      "name": "Subtract",
      "skill_name": "MathPlugin",
      "description": "Subtract two numbers"
    }
  ],
  "parameters": [
    {
      "Key": "INPUT",
      "Value": ""
    }
  ],
  "outputs": [
    "RESULT__FINAL_AMOUNT"
  ],
  "next_step_index": 0,
  "name": "",
  "skill_name": "Microsoft.SemanticKernel.Planning.Plan",
  "description": "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent $5 on a latte?"
}
```

# [Python](#tab/python)

```python
print(plan.generated_plan)
```

```output
{
  "input": 2130.23,
  "subtasks": [
    {"function": "math_plugin.multiply", "args": {"number2": 1.23}},
    {"function": "math_plugin.subtract", "args": {"number2": 5}}
  ]
}
```

---

Notice how in the example, planner can string together functions and pass parameters to them. This effectively allows us to deprecate the `OrchestratorPlugin` we created previously because we no longer need the `RouteRequest` native function or the `GetNumbers` semantic function. Planner does both.

## When to use planner?
As demonstrated by this example, planner is extremely powerful because it can automatically recombine functions you have already defined, and as AI models improve and as the community developers better planners, you will be able to rely on them to achieve increasingly more sophisticated user scenarios.

There are, however, considerations you should make before using a planner. The following table describes the top considerations you should make along with mitigations you can take to reduce their impact.

| Considerations | Description | Mitigation |
| --- | --- | --- |
| **Performance** | It takes time for a planner to consume the full list of tokens and to generate a plan for a user, if you rely on the planner after a user provides input, you may unintentionally hang the UI while waiting for a plan. | While building UI, it's important to provide feedback to the user to let them know something is happening with loading experiences. You can also use LLMs to stall for time by generating an initial response for the user while the planner completes a plan. Lastly, you can use [predefined plans](./planner.md#using-predefined-plans) for common scenarios to avoid waiting for a new plan. |
| **Cost** | both the prompt and generated plan consume many tokens. To generate a very complex plan, you may need to consume _all_ of the tokens provided by a model. This can result in high costs for your service if you're not careful, especially since planning typically requires more advanced models like GPT 3.5 or GPT 4. | The more atomic your functions are, the more tokens you'll require. By authoring higher order functions, you can provide planner with fewer functions that use fewer tokens. Lastly, you can use [predefined plans](./planner.md#using-predefined-plans) for common scenarios to avoid spending money on new plans. |
| **Correctness** | Planner can generate faulty plans. For example, it may pass variables incorrectly, return malformed schema, or perform steps that don't make sense. | To make planner robust, you should provide error handling. Some errors, like malformed schema or improperly returned schema, can be recovered by asking planner to "fix" the plan. |

### Using predefined plans
There are likely common scenarios that your users will frequently ask for. To avoid the performance hit and the costs associated with planner, you can pre-create plans and serve them up to a user.

This is similar to the front-end development adage coined by Aaron Swartz: "[Bake, don't fry](http://www.aaronsw.com/weblog/000404)." By pre-creating, or "baking," your plans, you can avoid generating them on the fly (i.e., "frying"). You won't be able to get rid of "frying" entirely when creating AI apps, but you can reduce your reliance on it so you can use healthier alternatives instead.

To achieve this, you can generate plans for common scenarios offline, and store them as XML in your project. Based on the intent of the user, you can then serve the plan back up so it can be executed. By "baking" your plans, you also have the opportunity to create additional optimizations to improve speed or lower costs.

## Next steps
You now have the skills necessary to automatically generate plans for your users. You can use these skills to create more advanced AI apps that can handle increasingly complex scenarios. In the next section, you'll learn how to author plugins that can be used by planner _and_ ChatGPT.

> [!div class="nextstepaction"]
> [Create and run ChatGPT plugins](./chatgpt-plugins.md)