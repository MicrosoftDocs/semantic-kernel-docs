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

```Your-App-And-Semantic-Skills
MyAppSource
│
└───MySkillsDirectory
    │
    └─── TestSkillImproved
    |   │
    |   └─── SloganMakerFlex
    |   │    └─── skprompt.txt
    |   │    └─── config.json
    |   └─── SummarizeBlurbFlex
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

And then use the native skill via:

```csharp
var mySkill = myKernel.ImportSkill ( new MyCSharpSkill(), "MyCSharpSkill");

var myContext = new ContextVariables(); 
myContext.Set("INPUT","This is input.");

var myOutput = await myKernel.RunAsync(myContext,mySkill["DupDup"]);
Console.WriteLine(myOutput);
```

The output will look like: `This is input.This is input.`

## Take the next step

> [!div class="nextstepaction"]
> [Run the samples](../samples)

[!INCLUDE [footer.md](../includes/footer.md)]