---
title: How to compose functions in Semantic Kernel
description: How to compose functions in Semantic Kernel
author: johnmaeda
ms.topic: skills
ms.author: johnmaeda
ms.date: 02/07/2023
ms.prod: semantic-kernel
---
# Connecting multiple functions together

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

Semantic Kernel (SK) was designed in the spirit of UNIX's piping capability to take one command and stream its output to the next command in the sequence. You can see that legacy design built-in to the use of the `$INPUT` parameter as a default intake for a function to stream its output into the next `$INPUT`-ready function.

Let's illustrate that approach with the core skill `TextSkill` that lets us do some basic string manipulation:

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

Semantic functions can be composed similarly -- as well as any mixture of semantic or native functions.

## Take the next step

> [!div class="nextstepaction"]
> [Run the samples](../samples)