---
title: How to automatically orchestrate AI with planners
description: Learn how to automatically create chains of functions with planner.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Automatically orchestrate AI with planners

So far, we have orchestrated functions using function calling. This, however, doesn't always work well. Take for example if you wanted to add all of the numbers between 1 and 100. With function calling, you'd need to make a call to the LLM for every number. That's an expensive request!

With planners, however, you can use alternative strategies to optimize the number of functions you can call per LLM request. For example, with the Handlebars planner, you can generate an entire plan with loops and if statements all with a single LLM call. For complex request, this means you can save both time and money.

If you want to see the final solution, you can check out the following samples in the public documentation repository.

| Language | Link to final solution |
| --- |
| C# | [Open example in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/DocumentationExamples/Planner.cs) |
| Python | [Open solution in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/documentation_examples/planner.py) |

> [!Tip]
> If you previously used the Action, Sequential, or Stepwise planners, please upgrade to the latest planners: Handlebars and Function calling stepwise planner. You can follow the migration guide on our blog to learn how to make the move.

> [!Important]
> All planners are currently marked experimental in the C# SDK until we finalize the API surface. If you would like to use them, add `<NoWarn>SKEXP0060</NoWarn>` in your _.csproj_ file for the Handlebars planner or `<NoWarn>SKEXP0061</NoWarn>` in your _.csproj_ file for the stepwise planner. Alternatively, you can add `#pragma warning disable SKEXP0060` or `#pragma warning disable SKEXP0061` in the file that uses the planners.

## What is a planner?
Planner is a function that takes a user's ask and returns back a plan on how to accomplish the request. It does so by using AI to mix-and-match the plugins registered in the kernel so that it can recombine them into a series of steps that complete a goal.

:::row:::
   :::column span="2":::
      This is a powerful concept because it allows you to create atomic functions that can be used in ways that you as a developer may not have thought of.
      
      For example, if you had task and calendar event plugins, planner could combine them to create workflows like "remind me to buy milk when I go to the store" or "remind me to call my mom tomorrow" without you explicitly having to write code for those scenarios.
   :::column-end:::
   :::column span="3":::
        ![Planner automatically combines functions](../../media/the-planner.png)
   :::column-end:::
:::row-end:::

### Instantiating a planner
To instantiate a planner, all you need to do is pass in a configuration object.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugins/MathSolver.cs" range="31":::

# [Python](#tab/python)
In Python, you currently need to pass in the kernel as well.

1. Import Semantic Kernel.
    :::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/planner.py" range="7-8" :::

2. Create the kernel.
    :::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/planner.py" range="13" :::

3. Add the service to the kernel.
    :::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="39-46":::

4. Create the planner.
    :::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/planner.py" range="23-26":::

---


### Creating and running a plan
Now that we have planner, we can use it to create a plan for a user's ask and then invoke the plan to get a result. The following code asks our planner to solve a math problem that is difficult for an LLM to solve on its own because it requires multiple steps and it has numbers with decimal points.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugins/MathSolver.cs" range="38-44":::

# [Python](#tab/python)
:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/planner.py" range="28-34":::

---

After running this code, with the ask "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent $5 on a latte?" you should get the correct answer of `2615.18` back, but how?

## How do planners work?
Behind the scenes, planner uses an LLM prompt to generate a plan. You can see the prompt that is used by the `HandlebarsPlanner` by navigating to its [prompt file](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Planners/Planners.Handlebars/Handlebars/CreatePlanPrompt.handlebars) in the Semantic Kernel repository.

### Understanding the prompts powering planners

The last few lines of the prompt are the most important to understanding how planner works. They look like this:

```handlebars
## Start
Now take a deep breath and accomplish the task:
1. Keep the template short and sweet. Be as efficient as possible.
2. Do not make up helpers or functions that were not provided to you, and be especially careful to NOT assume or use any helpers or operations that were not explicitly defined already.
3. If you can't fully accomplish the goal with the available helpers, just print "{{insufficientFunctionsErrorMessage}}".
4. Always start by identifying any important values in the goal. Then, use the `\{{set}}` helper to create variables for each of these values.
5. The template should use the \{{json}} helper at least once to output the result of the final step.
6. Don't forget to use the tips and tricks otherwise the template will not work.
7. Don't close the ``` handlebars block until you're done with all the steps.
```

With these steps, the planner is given a set of rules that it can use to generate a plan using Handlebars. Also inside of the prompt is what we call the "function manual"

```handlebars
{{#each functions}}
### `{{doubleOpen}}{{PluginName}}{{../nameDelimiter}}{{Name}}{{doubleClose}}`
Description: {{Description}}
Inputs:
  {{#each Parameters}}
    - {{Name}}:
    {{~#if ParameterType}} {{ParameterType.Name}} -
    {{~else}}
        {{~#if Schema}} {{getSchemaTypeName this}} -{{/if}}
    {{~/if}}
    {{~#if Description}} {{Description}}{{/if}}
    {{~#if IsRequired}} (required){{else}} (optional){{/if}}
  {{/each}}
Output:
{{~#if ReturnParameter}}
  {{~#if ReturnParameter.ParameterType}} {{ReturnParameter.ParameterType.Name}}
  {{~else}}
    {{~#if ReturnParameter.Schema}} {{getSchemaReturnTypeName ReturnParameter}}
    {{else}} string{{/if}}
  {{~/if}}
  {{~#if ReturnParameter.Description}} - {{ReturnParameter.Description}}{{/if}}
{{/if}}
{{/each}}
```

The "function manual" describes all of the function that are available for the LLM to create a plan with.

## Giving planner the _best_ data
When you render the prompt, one of the main things you might notice is that all of the descriptions we provided for our functions are included in the prompt. For example, the description for `MathPlugin.Add` is included in the prompt as `Add two numbers`.

```txt
[AVAILABLE FUNCTIONS]

### `{{MathPlugin-Add}}`
Description: Add two numbers
Inputs:
  - number1 double - The first number to add (required)
  - number2 double - The second number to add (required)
Output: double

### `{{MathPlugin.Divide}}`
Description: Divide two numbers
Inputs:
  - number1: double - The first number to divide from (required)
  - number2: double - The second number to divide by (required)
Output: double
```

Because of this, it's incredibly important to provide the best descriptions you can for your functions. If you don't, planner will not be able to generate a plan that uses your functions correctly.

You can also use the descriptions to provide explicit instructions to the model on how to use your functions. Below are some techniques you can use to improve the use of your functions by planner.

- **Provide help text** – It's not always clear when or how to use a function, so giving advice helps. For example, the description for `MathPlugin.Multiply` reminds the bot to add 1 whenever it increases a number by a percentage.
- **Describe the output.** – While there is not an explicit way to tell planner what the output of a function is, you can describe the output in the description.
- **State if inputs are required.** – If a function requires an input, you can state that in the input's description so the model knows to provide an input. Conversely, you can tell the model that an input is optional so it knows it can skip it if necessary.


### Viewing the plan produced by a planner
Because the plan is returned as plain text (either as XML or JSON), we can print the results to inspect what plan planner actually created. The following code shows how to print the plan in C#.

```csharp
Console.WriteLine(plan);
```

```output
Plugins.MathSolver: Information: Plan: {{!-- Step 1: Set the initial investment amount --}}
{{set "initialInvestment" 2130.23}}

{{!-- Step 2: Calculate the increase percentage --}}
{{set "increasePercentage" 0.23}}

{{!-- Step 3: Calculate the final amount after the increase --}}
{{set "finalAmount" (MathPlugin-Multiply (get "initialInvestment") (MathPlugin-Add 1 (get "increasePercentage")))}}

{{!-- Step 4: Output the final amount --}}
{{json (get "finalAmount")}}
```

Notice how in the example, planner can string together functions and pass parameters to them. Once the plan is rendered by Handlebars, the final result is the following:

```
2620.1829
```

## When to use a planner?
As demonstrated by this example, planners are extremely powerful because they can automatically recombine functions you have already defined, and as AI models improve and as the community develops better planners, you will be able to rely on them to achieve increasingly more sophisticated user scenarios.

There are, however, considerations you should make before using a planner. The following table describes the top considerations you should make along with mitigations you can take to reduce their impact.

| Considerations | Description | Mitigation |
| --- | --- | --- |
| **Performance** | It takes time for a planner to consume the full list of tokens and to generate a plan for a user, if you rely on the planner after a user provides input, you may unintentionally hang the UI while waiting for a plan. | While building UI, it's important to provide feedback to the user to let them know something is happening with loading experiences. You can also use LLMs to stall for time by generating an initial response for the user while the planner completes a plan. Lastly, you can use [predefined plans](./index.md#using-predefined-plans) for common scenarios to avoid waiting for a new plan. |
| **Cost** | both the prompt and generated plan consume many tokens. To generate a very complex plan, you may need to consume _all_ of the tokens provided by a model. This can result in high costs for your service if you're not careful, especially since planning typically requires more advanced models like GPT 3.5 or GPT 4. | The more atomic your functions are, the more tokens you'll require. By authoring higher order functions, you can provide planner with fewer functions that use fewer tokens. Lastly, you can use [predefined plans](./index.md#using-predefined-plans) for common scenarios to avoid spending money on new plans. |
| **Correctness** | Planner can generate faulty plans. For example, it may pass variables incorrectly, return malformed schema, or perform steps that don't make sense. | To make planner robust, you should provide error handling. Some errors, like malformed schema or improperly returned schema, can be recovered by asking planner to "fix" the plan. |

### Using predefined plans
There are likely common scenarios that your users will frequently ask for. To avoid the performance hit and the costs associated with planner, you can pre-create plans and serve them up to a user.

This is similar to the front-end development adage coined by Aaron Swartz: "[Bake, don't fry](http://www.aaronsw.com/weblog/000404)." By pre-creating, or "baking," your plans, you can avoid generating them on the fly (i.e., "frying"). You won't be able to get rid of "frying" entirely when creating AI apps, but you can reduce your reliance on it so you can use healthier alternatives instead.

To achieve this, you can generate plans for common scenarios offline, and store them as XML in your project. Based on the intent of the user, you can then serve the plan back up so it can be executed. By "baking" your plans, you also have the opportunity to create additional optimizations to improve speed or lower costs.

## Next steps
You now have the skills necessary to automatically generate plans for your users. You can use these skills to create more advanced AI apps that can handle increasingly complex scenarios. In the next section, you'll learn how to evaluate  your planners with Prompt flow.

> [!div class="nextstepaction"]
> [Evaluate your planners with Prompt flow](./evaluate-and-deploy-planners/index.md)
