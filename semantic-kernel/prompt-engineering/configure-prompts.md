---
title: Configure prompts 
description: Learn how to configure prompts for your functions in Semantic Kernel
author: johnmaeda
ms.topic: prompt-engineering
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# Configuring prompts


[!INCLUDE [pat_large.md](../includes/pat_large.md)]


When creating a prompt, there are many parameters that can be set to control how the prompt behaves. In Semantic Kernel, these parameters both control how a function is used by [planner](/semantic-kernel/concepts-sk/planner) and how it is run by an [LLM AI model](../prompt-engineering/llm-models.md).

Semantic Kernel allows a developer to have complete control over these parameters by using a _config.json_ file placed in the same directory as the `skprompt.txt` file.

For example, if you were to create a plugin called `TestPlugin` with two semantic functions called `SloganMaker` and `OtherFunction`, the file structure would look like this:

```File-Structure-For-Semantic-Plugins
TestPlugin
│
└─── SloganMaker
|    |
│    └─── skprompt.txt
│    └─── config.json
│   
└─── OtherFunction
     |
     └─── skprompt.txt
     └─── config.json
```

The _config.json_ file for the `SloganMaker` function would look like this:

```config.json-example
{
  "schema": 1,
  "type": "completion",
  "description": "a function that generates marketing slogans",
  "completion": {
    "max_tokens": 1000,
    "temperature": 0.0,
    "top_p": 0.0,
    "presence_penalty": 0.0,
    "frequency_penalty": 0.0
  }
  "input": {
    "parameters": [
      {
        "name": "input",
        "description": "The product to generate a slogan for",
        "defaultValue": ""
      }
    ]
  }
}
```

> [!NOTE]
> The _config.json_ file is currently optional, but if you wish to exercise precise control of a function's behavior be sure to include it inside each function directory. 

## Parameters used by planner
The `description` field in the root object and `input` object are used by [planner](/semantic-kernel/concepts-sk/planner) to determine how to use a function. The root `description` tells planner what the function does, and the input `description` tells planner how to populate the input parameters.

Because these parameters impact the behavior of planner, we recommend running tests on the values you provide to ensure  they are used by planner correctly.

When writing `description` and `input`, we recommend using the following guidelines:
- The `description` fields should be short and concise so that it does not consume too many tokens when used in planner prompt.
- Consider the `description`s of other functions in the same plugin to ensure that they are sufficiently unique. If they are not, planner may not be able to distinguish between them.
- If you have trouble getting planner to use a function, try adding recommendations or examples for when to use the function.

## Completion parameters in config.json
In addition to providing parameters for planner, the _config.json_ file also allows you to control how a function is run by an [LLM AI model](../prompt-engineering/llm-models.md). The `completion` object in the root object of the _config.json_ file allows you to set the parameters used by the model.

The following table describes the parameters available for use in the `completion` object for the OpenAI and Azure OpenAI APIs:

| Completion Parameter | Type | Required? | Default | Description |
|---|---|---|---|
| `max_tokens` | integer| Optional |16 |	The maximum number of tokens to generate in the completion. The token count of your prompt plus max_tokens can't exceed the model's context length. Most models have a context length of 2048 tokens (except davinci-codex, which supports 4096).|
| `temperature`	| number	| Optional	| 1	| What sampling temperature to use. Higher values means the model will take more risks. Try 0.9 for more creative applications, and 0 (argmax sampling) for ones with a well-defined answer. We generally recommend altering this or `top_p` but not both. |
| `top_p`	| number	| Optional	| 1	| An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered. We generally recommend altering this or temperature but not both. |
| `presence_penalty` | number	| Optional	| 0	| Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics. |
| `frequency_penalty` |	number	| Optional	|0 |	Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim. |

To learn more about the various parameters available for tuning how a function works, visit the [Azure OpenAI reference](/azure/cognitive-services/openai/reference).

### Default setting for OpenAI and Azure OpenAI
If you do not provide completion parameters in the _config.json_ file, Semantic Kernel will use the default parameters for the OpenAI API. Learn more about the current defaults by reading the [Azure OpenAI API reference](/azure/cognitive-services/openai/reference).

## Take the next step
> [!div class="nextstepaction"]
> [Understanding tokens](./tokens.md)

