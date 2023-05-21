---
title: Configure prompts 
description: Learn how to configure prompts for your functions in Semantic Kernel
author: johnmaeda
ms.topic: skills
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# Configuring templates

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

```File-Structure-For-Semantic-Skills
TestSkill
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

LLM AI [models](/semantic-kernel/concepts-ai/models) have a variety of parameters associated them that can alter their behavior. SK enables the developer to have complete control over how a model is to be configured by using a `config.json` file placed in the same directory as the `skprompt.txt` file.

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
}
```

The text used in `description` is arguably the most important parameter to consider because it's used by the [planner](/semantic-kernel/concepts-sk/planner) to get a quick read on what the function can do for a user.

> [!NOTE]
> The `config.json` file is currently optional, but if you wish to exercise precise control of a function's behavior be sure to include it inside each function directory. 

To learn more about the various parameters available for tuning how a function works, visit the [Azure OpenAI reference](/azure/cognitive-services/openai/reference).

## Default backends setting for OpenAI and Azure OpenAI

Learn more about [available GPT-3](/azure/cognitive-services/openai/concepts/models) models besides `text-davinci-003` for completion.

## Completion parameters that can be set in config.json

| Completion Parameter | Type | Required? | Default | Description |
|---|---|---|---|
| `max_tokens` | integer| Optional |16 |	The maximum number of tokens to generate in the completion. The token count of your prompt plus max_tokens can't exceed the model's context length. Most models have a context length of 2048 tokens (except davinci-codex, which supports 4096).|
| `temperature`	| number	| Optional	| 1	| What sampling temperature to use. Higher values means the model will take more risks. Try 0.9 for more creative applications, and 0 (argmax sampling) for ones with a well-defined answer. We generally recommend altering this or `top_p` but not both. |
| `top_p`	| number	| Optional	| 1	| An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered. We generally recommend altering this or temperature but not both. |
| `presence_penalty` | number	| Optional	| 0	| Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics. |
| `frequency_penalty` |	number	| Optional	|0 |	Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim. |

## Take the next step

Running the app samples will give you the quickest sense of what you can do with SK. 

> [!div class="nextstepaction"]
> [Run the app samples](/semantic-kernel/samples)


---