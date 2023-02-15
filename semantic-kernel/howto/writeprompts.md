---
title: How to write prompts in Semantic Kernel
description: How to write prompts in Semantic Kernel
author: johnmaeda
ms.topic: skills
ms.author: johnmaeda
ms.date: 02/07/2023
ms.prod: semantic-kernel
---
# It all starts with an ASK

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

To write an LLM AI prompt that Semantic Kernel (SK) is uniquely fit for, all you need is a concrete goal in mind — something you would like an AI to get done for you. For example:

> I want to make a cake. Give me the best chocolate cake recipe you can think of.

Congratulations! You have imagined a delicious ASK for SK to run to completion. This ASK can be given to the Planner to get decomposed into steps. Although to make the Planner work reliably, you'll need to use the most advanced model available to you. So let's start from writing basic prompts to begin with.

## Writing a simple prompt

Writing prompts is like making a wish. Let's imagine we are entrepreneurs trying to make it in downtown Manhattan and we need to drive more leads to our store. We write the prompt:

```Plain-Prompt
Write me a marketing slogan for my apparel shop in New 
York City with a focus on how affordable we are without 
sacrificing quality.
```

The result of this prompt from an actual LLM AI model is:

```Response-From-LLM-AI-Model
New York Style, Low-Cost Smile: 
Shop at NYC's Best Apparel Store!
```

Let's try another example where we are eager to play with the summarizing capability of LLM AIs and want to show off its superpower when applied to text that we explicitly define:

```Plain-Prompt
Summarize the following text in two sentences or less. 
[BEGIN TEXT]
Jan had always wanted to be a writer, ever since they 
were a kid. They spent hours reading books, writing 
stories, and imagining worlds. They grew up and pursued 
their passion, studying literature and journalism, and 
submitting their work to magazines and publishers. They 
faced rejection after rejection, but they never gave up 
hope. Jan finally got their breakthrough, when a famous 
editor discovered their manuscript and offered them a 
book deal.
[END TEXT]
```

The result of this prompt from an actual LLM AI model is:

```Response-From-LLM-AI-Model
A possible summary is:

Jan's lifelong dream of becoming a writer came true 
when a famous editor offered them a book deal, after 
years of rejection and perseverance.
```

And there we have it. Two simple prompts that aren't asking the model for too much: 1/ we're asking it to give us a marketing slogan, and 2/ we're asking it to summarize a body of text down to two sentences.

Both of these simple prompts qualify as "functions" that can be packaged as part of an SK skill. The only problem is that they can do only one thing — as defined by the prompt — and with no flexibility. We set up the first plain prompt in SK within a directory named `SloganMaker` into a file named `skprompt.txt`:

```sloganmaker/skprompt.txt
Write me a marketing slogan for my apparel shop in New 
York City with a focus on how affordable we are without 
sacrificing quality.
```

Similarly, we place the second plain prompt into a directory named `SummarizeBlurb` as a file named into a file named `skprompt.txt`. Each of these directories comprise a SK function. When both of the directories are placed inside an enclosing directory called `TestSkill` the result is a brand new skill. 

```File-Structure-For-Skill-Definition-With-Functions
TestSkill
│
└─── SloganMaker
|    |
│    └─── skprompt.txt
│   
└─── SummarizeBlurb
     |
     └─── skprompt.txt
```

This skill can do one of two things by calling one of its two functions:

* `TestSkill.SloganMaker()` generates a slogan for a specific kind of shop in NYC
* `TestSkill.SummmarizeBlurb()` creates a short summary of a specific blurb

Next, we'll show you how to make a more powerful skill by introducing SK prompt templates.

## Writing a more powerful, "templated" prompt

Let's say we want to go into the advertising business with AI powering the slogan-side of our offerings. We'd like to encapsulate how we create slogans to be repeatable and across any industry. To do so, we take our first prompt and write it
as such:

```Templated-Prompt
Write me a marketing slogan for my {{$INPUT}} in New 
York City with a focus on how affordable we are without 
sacrificing quality.
```

The double curly braces signify to SK that there's something special for it to notice within the LLM AI prompt. All prompting variables that are passed to SK will begin with a dollar sign `$` — with `$INPUT` being a reserved name for the first passed variable. 

We can do the same for how we summarize text into two sentences by removing the body of the text we want to summarize, and replacing it with "{{$input}}" to pass into the prompt at runtime.

```Templated-Prompt
Summarize the following text in two sentences or less. 
[BEGIN TEXT]
{{$input}}
[END TEXT]
```

This function would be named `SloganMakerGeneral` and the other would be named `SummarizeBlurbGeneral` — as two new SK functions that can belong to a `TestSkillImproved` skill. To package these two function to be used by SK in the context of a skill, we do the same as we did before:

```File-Structure-For-Skill-Definition-With-Functions
TestSkillImproved
│
└─── SloganMakerGeneral
|    |
│    └─── skprompt.txt
│   
└─── SummarizeBlurbGeneral
     |
     └─── skprompt.txt
```

The difference is that we can now pass an input parameter to the function — which enables it to become a generalized machine for generating outputs:

* `TestSkill.sloganmaker('flatware')` generates a slogan for a 'flatware' shop in NYC
* `TestSkill.summmarizeblurb('<insert long text here>')` creates a short summary of given blurb

Templated prompts can be further customized to call on more kinds of inputs to have even more generalized capabilities.

## Take the next step

> [!div class="nextstepaction"]
> [Learn how to configure models](configuremodels)

[!INCLUDE [footer.md](../includes/footer.md)]