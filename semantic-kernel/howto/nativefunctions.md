---
title: How to write native skills in Semantic Kernel
description: How to write native skills in Semantic Kernel
author: johnmaeda
ms.topic: skills
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# It all starts with a little C# knowledge

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

If you're new to C#, here's a few quick tips to get you started in the context of Semantic Kernel (SK):

* With semantic functions, they went into folders with an `skprompt.txt` file and a `config.json` file; with native C# functions they live as a `.cs` file like _MyCSharpSkill.cs._
* Strings in C# can span multiple lines with the `@` sign and enclosing `""`
 or by being enclosed within a pair of three double quotes `"""`.

## C# skill as file _MyCSharpSkill.cs_

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
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Orchestration;

namespace MySkillsDirectory;

public class MyCSharpSkill
{
    [SKFunction("Return the first row of a qwerty keyboard")]
    public string Qwerty(string input)
    {
        return "qwertyuiop";
    }

    [SKFunction("Return a string that's duplicated")]
    public string DupDup(string text)
    {
        return text + text;
    }
}
```

And then use the native skill in your C# project:

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

using MySkillsDirectory;

// ... instantiate a kernel as myKernel

var mySkill = myKernel.ImportSkill (new MyCSharpSkill(), "MyCSharpSkill");

var myContext = new ContextVariables(); 
myContext.Set("INPUT","This is input.");

var myOutput = await myKernel.RunAsync(myContext,mySkill["DupDup"]);
Console.WriteLine(myOutput);
```

The output will look like: `This is input.This is input.`

This might seem like a lot of extra work to simply get native C# code to work within a C# codebase itself, but that's not really the point. The magic is in how you can easily call a native function from a semantic function. 

## Calling a native function from a semantic function

We start with our native function `MyCSharpSkill` that has the simplistic `DupDup` function within a mixed directory of semantic and native skills:

```Your-App-And-Skills
MyAppSource
│
└───MySkillsDirectory
    │
    └─── MySemanticSkill
    |   │
    |   └─── MySemanticFunction
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
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration;

using MySkillsDirectory;

// ... instantiate a kernel as myKernel

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

## Adding extra input parameters to a native function

Use an `SKContext` as input to the native function to extract the context variables:

```csharp
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Orchestration;

namespace MySkillsDirectory;

public class MyCSharpSkill
{
    [SKFunction("Return a string that's duplicated")]
    public string DupDup(string text)
    {
        return text + text;
    }

    [SKFunction("Joins a first and last name together")]
    [SKFunctionContextParameter(Name = "firstname", Description = "Informal name you use")]
    [SKFunctionContextParameter(Name = "lastname", Description = "More formal name you use")]
    public string FullNamer(SKContext context)
    {
        return context["firstname"] + " " + context["lastname"];
    }
}
```

The context parameters are pushed into the native function in a similar manner to how semantic functions work:

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

using MySkillsDirectory;

// ... instantiate a kernel as myKernel

var myContext = new ContextVariables(); 
myContext.Set("firstname","Sam");
myContext.Set("lastname","Appdev");

var myCshSkill = myKernel.ImportSkill ( new MyCSharpSkill(), "MyCSharpSkill");
var myOutput = await myKernel.RunAsync(myContext,myCshSkill["FullNamer"]);

Console.WriteLine(myOutput);
```

The output is `"Sam AppDev"`. 

## Advanced: Building a native asynchronous function in a skill

Recall our first function example `Qwerty` and compare it with a new function called `Asdfg`:

```csharp
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Orchestration;

public class MyCSharpSkill
{
    [SKFunction("Return the first row of a qwerty keyboard")]
    public string Qwerty(string input)
    {
        return "qwertyuiop";
    }

    [SKFunction("Return the second row of a qwerty keyboard")]
    [SKFunctionName("Asdfg")]
    public async Task<string> AsdfgAsync(string input)
    {
        await ...do something asynchronous...
        
        return "asdfghjkl";
    }
```

All semantic functions run asynchronously by default. But native functions can run synchronous or asynchronous. In the above example, `Qwerty` runs synchronously (i.e. it returns immediately) where as `Asdfg` runs asynchronously (i.e. it may be calling an API). Two things to note about this syntax:

1. We use the convention `<functionname>Async` to identify the C# function as running asynchronously. As a result, the function needs to be called `MyCSharpSkill.AsdfgAsync` explicitly — which isn't great. 

2. So to fix that, we write `[SKFunctionName("Asdfg")]` to rename how SK accesses the function as `MyCSharpSkill.Asdfg` that is more readily legible to humans. 

## Advanced: Using a semantic function from within a native C# skill

To access a semantic function `funSkill.joker` from a native function, there are one of two ways to achieve that goal: 

```csharp
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Orchestration;

namespace MySkillsDirectory;

public class MyCSharpSkill
{
    [SKFunction("Tell me a joke in one line of text")]
    [SKFunctionName("TellAJokeInOneLine")]
    public async Task<string> TellAJokeInOneLineAsync(SKContext context)
    {
        // Fetch a semantic function previously loaded into the kernel, 2 equivalent ways
        ISKFunction joker1 = context.Func("funSkill", "joker");
        ISKFunction joker2 = context.Skills.GetSemanticFunction("funSkill", "joker");

        var joke = await joker1.InvokeAsync();

        return joke.Result.ReplaceLineEndings(" ");
    }
}
```

Or also:

```csharp
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Orchestration;

namespace MySkillsDirectory;

public class MyCSharpSkill
{
    [SKFunction("Tell me a joke in one line of text")]
    [SKFunctionName("TellAJokeInOneLine")]
    public async Task<string> TellAJokeInOneLineAsync(SKContext context)
    {
        // Fetch a semantic function previously loaded into the kernel, 2 equivalent ways
        ISKFunction joker2 = context.Skills.GetSemanticFunction("funSkill", "joker");

        var joke = await joker2.InvokeAsync();

        return joke.Result.ReplaceLineEndings(" ");
    }
}
```

## Take the next step

Running the app samples will give you the quickest sense of what you can do with SK. 

> [!div class="nextstepaction"]
> [Run the app samples](/semantic-kernel/samples)