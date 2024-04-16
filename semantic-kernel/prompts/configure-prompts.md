---
title: Configure prompts within Semantic Kernel
description: Learn how to configure prompts for your functions in Semantic Kernel
author: johnmaeda
ms.topic: prompt-engineering
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: semantic-kernel
---
# Configuring prompts

When creating a prompt, you can adjust parameters that control how the prompt behaves. In Semantic Kernel, these parameters both control how a function is run by an AI model and how it used by function calling and [planners](../agents/planners/index.md).

For example, you could add settings to the chat prompt from the previous article with the following code

# [C#](#tab/Csharp)

In C#, you can define the following properties of a prompt:
- **Name** - the name of the prompt
- **Description** - a description of what the prompt does
- **Template format** - the format of the prompt template (e.g., `semantic-kernel`, `handlebars`)
- **Input variables** - the variables that are used inside of the prompt (e.g., `request`)
- **Execution settings** - the settings for different models that can be used to execute the prompt

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/ConfiguringPrompts.cs" id="FunctionFromPrompt":::

# [Java](#tab/Java)

In Java, you can define the following properties of a prompt:
- **Name** - the name of the prompt
- **Description** - a description of what the prompt does
- **Template format** - the format of the prompt template (e.g., `semantic-kernel`, `handlebars`)
- **Input variables** - the variables that are used inside of the prompt (e.g., `request`)
- **Input variables** - the variables that are used inside of the prompt (e.g., `request`)
- **Output variable** - the type of value that returned by the prompt (e.g., `java.lang.String`)
- **Execution settings** - the settings for different models that can be used to execute the prompt

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/ConfiguringPrompts.java" id="CreateFromPrompt":::

# [Python](#tab/python)

In Python, you can define the following properties of a prompt:
- **Name** - the name of the prompt
- **Description** - a description of what the prompt does
- **Execution settings** - the settings used to execute the prompt (e.g., `max_tokens`, `temperature`)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/configuring_prompts.py" range="43-67":::

---

## Parameters used by planner
The `description` field `input_variables` array are leveraged by [planners](/semantic-kernel/concepts-sk/planner) to determine how to use a function. The `description` tells planner what the function does, and the `input_variables` tells planner how to populate the input parameters.

Because these parameters impact the behavior of planner, we recommend running tests on the values you provide to ensure they are used by planner correctly.

When writing `description` and `input_variables`, we recommend using the following guidelines:
- The `description` fields should be short and concise so that it does not consume too many tokens when used in planner prompt (but not so short that it is not descriptive enough).
- Consider the `description`s of other functions in the same plugin to ensure that they are sufficiently unique. If they are not, planner may not be able to distinguish between them.
- If you have trouble getting planner to use a function, try adding recommendations or examples for when to use the function.

## Execution settings used by AI models
In addition to providing parameters for planner, the execution settings also allows you to control how a function is run by an AI model. The following table describes the many of the commonly available settings for models:

| Completion Parameter | Type | Required? | Default | Description |
|---|---|---|---|
| `max_tokens` | integer| Optional |16 |	The maximum number of tokens to generate in the completion. The token count of your prompt plus max_tokens can't exceed the model's context length. Most models have a context length of 2048 tokens (except davinci-codex, which supports 4096).|
| `temperature`	| number	| Optional	| 1	| What sampling temperature to use. Higher values means the model will take more risks. Try 0.9 for more creative applications, and 0 (argmax sampling) for ones with a well-defined answer. We generally recommend altering this or `top_p` but not both. |
| `top_p`	| number	| Optional	| 1	| An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered. We generally recommend altering this or temperature but not both. |
| `presence_penalty` | number	| Optional	| 0	| Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics. |
| `frequency_penalty` |	number	| Optional	|0 |	Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim. |

To learn more about the various parameters available for OpenAI and Azure OpenAI models, visit the [Azure OpenAI reference](/azure/cognitive-services/openai/reference) article.

### Default setting for OpenAI and Azure OpenAI
If you do not provide completion parameters, Semantic Kernel will use the default parameters for the OpenAI API. Learn more about the current defaults by reading the [Azure OpenAI API reference](/azure/cognitive-services/openai/reference) article.

## Take the next step
> [!div class="nextstepaction"]
> [Saving your prompts as files](./saving-prompts-as-files.md)

