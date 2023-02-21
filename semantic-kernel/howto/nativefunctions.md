---
title: How to write native skills in Semantic Kernel
description: How to write native skills in Semantic Kernel
author: johnmaeda
ms.topic: skills
ms.author: johnmaeda
ms.date: 02/07/2023
ms.prod: semantic-kernel
---
# It all starts with a little C# knowledge

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

> [!CAUTION]
> This section is still under construction

If you're new to C#, here's a few quick tips to get you started in the context of Semantic Kernel (SK):

* With semantic functions, they went into folders with an `skprompt.txt` file and a `config.json` file; with native C# functions they live as a `.cs` file like _MyCSharpSkill.cs._
* Strings in C# can span multiple lines with the `@` sign and enclosing `""`
 or by being enclosed within a pair of three double quotes `"""`.

## Sample C# skill in a file _MyCSharpSkill.cs_

A C# sharp skill goes into the same directory as the other semantic skills:

```Your-App-And-Skills
MyAppSource
│
└───MySkillsDirectory
    │
    └─── MySemanticSkill
    |   │
    |   └─── MyFirstSemanticFunction
    |   │    └─── skprompt.txt
    |   │    └─── config.json
    |   └─── MyOtherSemanticFunctions
    |        |...
    │
    └─── MyCSharpSkill.cs
```

Set up the `namespace` to match the enclosing skills directory:

```csharp
using Microsoft.SemanticKernel.Registry;
namespace MySkillsDirectory;

public class MyCSharpSkill
{
    [SKFunction(description: "Return the first row of a qwerty keyboard")]
    public string Qwerty(string input)
    {
        return "qwertyuiop";
    }

    [SKFunction(description: "Return a string that's duplicated")]
    public string DupDup(string text)
    {
        return text + text;
    }
}
```

And then use the native skill in your C# project:

```csharp
var mySkill = myKernel.ImportSkill ( ew MyCSharpSkill(), "MyCSharpSkill");

var myContext = new ContextVariables(); 
myContext.Set("INPUT","This is input.");

var myOutput = await myKernel.RunAsync(myContext,mySkill["DupDup"]);
Console.WriteLine(myOutput);
```

The output will look like: `This is input.This is input.`

This might seem like a lot of extra work to simply get native C# code to work within a C# codebase itself, but that's not really the point. The magic is in how you can easily call a native function from a semantic function. It's cool!

## Invoking a native function from a semantic function

We start with our native function `MyCSharpSkill` that has the simplistic `DupDup` function within a mixed directory of semantic and native skills:

```Your-App-And-Skills
MyAppSource
│
└───MySkillsDirectory
    │
    └─── MySemanticSkill
    |   │
    |   └─── MyFirstSemanticFunction
    |   │    └─── skprompt.txt
    |   │    └─── config.json
    |   └─── MyOtherSemanticFunctions
    |        |...
    │
    └─── MyCSharpSkill.cs
```

The semantic skill uses `DupDup` as a 100% deterministic in its function `DoubleTrouble`:

```Semantic-Skill-Calling-Native-Skill
There's nothing like an athlete's slogan that includes "{{MyCSharpSkill.DupDup $INPUT}}" so do your best to pitch a terrific running shoe with this key fact.
Earth-shattering slogan:
```

And then to use the skill we simply make sure that both the semantic skills and native skills are imported before we ask the kernel to do its thing:

```csharp
using MySkillsDirectory;

var myContext = new ContextVariables("*Twinnify"); 
var myCshSkill = myKernel.ImportSkill ( new MyCSharpSkill(), "MyCSharpSkill");
var mySemSkill = myKernel.ImportSemanticSkillFromDirectory("MySkillsDirectory", "MySemanticSkill");
var myOutput = await myKernel.RunAsync(myContext,mySemSkill["MySemanticFunction"]);

Console.WriteLine(myOutput);
```

The output will look similar to this:

```output
 "Twinnify, Twinnify - Run Faster, Run Further, with Earth-Shattering Comfort!"
```

## Take the next step

> [!div class="nextstepaction"]
> [Run the samples](../samples)

[!INCLUDE [footer.md](../includes/footer.md)]