---
title: How to use core skills in Semantic Kernel
description: How to use core skills in Semantic Kernel
author: johnmaeda
ms.topic: skills
ms.author: johnmaeda
ms.date: 02/07/2023
ms.prod: semantic-kernel
---
# Core skills are always ready to be accessed

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

> [!CAUTION]
> This section is still under construction

To provide a degree of standardization across Semantic Kernel (SK) implementations, the GitHub repo has several skills available to any SK prompt which you can browse at:

`/semantic-kernel/dotnet/src/CoreSkills`

The core skills currently supported include:

* TimeSkill: To acquire the time of day and any other temporal information
* TextSkill: To deterministically manipulating text strings
* FileIOSkill: To read and write to the filesystem
* HttpSkill: To call APIs
* PlannerSkill: To create and execute plans

# Example of how a core skill is used in SK

When using a core skill, be sure to include a `using Microsoft.SemanticKernel.CoreSkills`:

```csharp
using Microsoft.SemanticKernel.CoreSkills;

// ( You want to instantiate a kernel and configure it first )

myKernel.ImportSkill(new TimeSkill(), "time");

const string ThePromptTemplate = @"
Today is: {{time.Date}}
Current time is: {{time.Time}}

Answer to the following questions using JSON syntax, including the data used.
Is it morning, afternoon, evening, or night (morning/afternoon/evening/night)?
Is it weekend time (weekend/not weekend)?";

var myKindOfDay = myKernel.CreateSemanticFunction(ThePromptTemplate, maxTokens: 150);

var myOutput = await myKindOfDay.InvokeAsync();
Console.WriteLine(myOutput);
```

The output is what you would expect when you read `ThePromptTemplate` to be:

```resulting-output
{
  "date": "Monday, February 20, 2023",
  "time": "01:27:44 PM",
  "period": "afternoon",
  "weekend": "not weekend"
}
```

## Pipelining example with core skillls

Using the core `TextSkill` it's easy to transform text by modifying it sequentially:

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.CoreSkills;

var myKernel = Kernel.Build();

var myText = myKernel.ImportSkill(new TextSkill());

SKContext myOutput = await myKernel.RunAsync(
    "    i n f i n i t e     s p a c e     ",
    myText["TrimStart"],
    myText["TrimEnd"],
    myText["Uppercase"]);

Console.WriteLine(myOutput);
```

Note how the input streams through a pipeline of three functions executed serially. Expressed in an exaggerated manner, that's like:

| "   i n f i n i t e    s p a c e    " → | TextSkill.TrimStart → | TextSkill.TrimEnd → | TextSkill.Uppercase → |
|---|---|---|---|

The output reads as:

`I N F I N I T E     S P A C E`

## Take the next step

> [!div class="nextstepaction"]
> [Run the samples](../samples)
