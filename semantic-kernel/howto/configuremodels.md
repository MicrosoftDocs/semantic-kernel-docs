---
title: How to configure models in Semantic Kernel
description: How to configure models in Semantic Kernel
author: johnmaeda
ms.topic: skills
ms.author: johnmaeda
ms.date: 02/07/2023
ms.prod: semantic-kernel
---
## Configuring models 101

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

LLM AI [models](../concepts-ai/models) have a variety of parameters associated them that can alter their behavior. SK enables the developer to have complete control over how a model is to be configured by using a `config.json` file placed in the same directory as the `skprompt.txt` file.

```config.json-example
{
  "schema": 1,
  "type": "completion",
  "description": "a function that generates marketing slogans for shops in NYC",
  "completion": {
    "max_tokens": 1000,
    "temperature": 0.0,
    "top_p": 0.0,
    "presence_penalty": 0.0,
    "frequency_penalty": 0.0
  },
  "default_backends": [
    "text-davinci-003"
  ]
}
```

The `config.json` file is optional, but if you wish to exercise precise control of a function's behavior be sure to include it inside each function directory.

```File-Structure-For-Skill-Definition-With-Functions
TestSkillImproved
│
└─── SloganMakerGeneral
│    └─── skprompt.txt
│    └─── config.json
│   
└─── SummarizeBlurbGeneral
     └─── skprompt.txt
     └─── config.json

```

## Take the next step

> [!div class="nextstepaction"]
> [Learn how to think deeper about LLM AI](schillacelaws)

[!INCLUDE [footer.md](../includes/footer.md)]