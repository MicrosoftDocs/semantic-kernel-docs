---
title: Out-of-the-box plugins available in Semantic Kernel
description: View the plugins available in Semantic Kernel for semantic and native functions.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---
# Use the out-of-the-box plugins in the kernel

[!INCLUDE [pat_large.md](../includes/pat_large.md)]


> [!Note]
> Skills are currently being renamed to plugins. This article has been updated to reflect the latest terminology, but some images and code samples may still refer to skills.

To provide a degree of standardization across Semantic Kernel implementations, the GitHub repo has several plugins available out-of-the-box depending on the language you are using. These plugins are often referred to as **Core plugins**. Additionally, each library also includes a handful of other plugins that you can use. The following section covers each set of plugins in more detail.

## Core plugins
The core plugins are planned to be available in all languages since they are core to using Semantic Kernel. Below are the core plugins currently available in Semantic Kernel along with their current support for each language. The ❌ symbol indicates that the feature is not yet available in that language; if you would like to see a feature implemented in a language, please consider [contributing to the project](../get-started/contributing.md) or [opening an issue](../get-started/contributing.md#reporting-issues).

| Plugin | Description | C# | Python | Java |
| --- | --- | :------:|:----: | :----: |
| `ConversationSummarySkill` | To summarize a conversation | ✅ | ✅ | * |
| `FileIOSkill` | To read and write to the filesystem | ✅ | ✅ | ❌ |
| `HttpSkill` | To call APIs | ✅ | ✅ | ❌ |
| `MathSkill` | To perform mathematical operations | ✅ | ✅ | ❌ |
| `TextMemorySkill` | To store and retrieve text in memory | ✅ | ✅ | ❌ |
| `TextSkill` | To deterministically manipulating text strings | ✅ | ✅ | * |
| `TimeSkill` | To acquire the time of day and any other temporal information | ✅ | ✅ | * |
| `WaitSkill` | To pause execution for a specified amount of time | ✅ | ❌ | ❌ |

You can find the full list of core plugins for each language by following the links below:
- [C# core plugins](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Skills/Skills.Core)
- [Python core plugins](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/core_skills)

### Using core plugins in Semantic Kernel
If you want to use one of the core plugins, you can easily import them into your project. For example, if you want to use the `TimeSkill` in either C# or Python, you can import it as follows.

# [C#](#tab/Csharp)

When using a core plugin, be sure to include a `using Microsoft.SemanticKernel.CoreSkills`:

```csharp
using Microsoft.SemanticKernel.CoreSkills;

// ... instantiate a kernel and configure it first

kernel.ImportSkill(new TimeSkill(), "time");

const string ThePromptTemplate = @"
Today is: {{time.Date}}
Current time is: {{time.Time}}

Answer to the following questions using JSON syntax, including the data used.
Is it morning, afternoon, evening, or night (morning/afternoon/evening/night)?
Is it weekend time (weekend/not weekend)?";

var myKindOfDay = kernel.CreateSemanticFunction(ThePromptTemplate, maxTokens: 150);

var myOutput = await myKindOfDay.InvokeAsync();
Console.WriteLine(myOutput);
```


# [Python](#tab/python)

```python
from semantic_kernel.core_skills.time_skill import TimeSkill

# ... instantiate a kernel and configure it first

kernel.import_skill(TimeSkill(), "time")

ThePromptTemplate = """
Today is: {{time.Date}}
Current time is: {{time.Time}}

Answer to the following questions using JSON syntax, including the data used.
Is it morning, afternoon, evening, or night (morning/afternoon/evening/night)?
Is it weekend time (weekend/not weekend)?
"""

myKindOfDay = kernel.create_semantic_function(ThePromptTemplate, max_tokens=150)

myOutput = await myKindOfDay.invoke_async()
print(myOutput)

```
---

The output should be similar to the following:

```resulting-output
{
  "date": "Wednesday, 21 June, 2023",
  "time": "12:17:02 AM",
  "period": "night",
  "weekend": "not weekend"
}
```

### Chaining core plugins together in Semantic Kernel
Most of the core plugins were built so that they can be easily chained together. For example, the `TextSkill` can be used to trim whitespace from a string, convert it to uppercase, and then convert it to lowercase.


# [C#](#tab/Csharp)

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.CoreSkills;

var kernel = Kernel.Builder.Build();

var myText = kernel.ImportSkill(new TextSkill());

SKContext myOutput = await kernel.RunAsync(
    "    i n f i n i t e     s p a c e     ",
    myText["TrimStart"],
    myText["TrimEnd"],
    myText["Uppercase"]);

Console.WriteLine(myOutput);
```


# [Python](#tab/python)

```python
import semantic_kernel as sk
from semantic_kernel.core_skills.text_skill import TextSkill

my_text = kernel.import_skill(TextSkill(), "time")

myOutput = await kernel.run_async(
    my_text["trim_start"],
    my_text["trim_end"],
    my_text["uppercase"],
    input_str="    i n f i n i t e     s p a c e     ",
)

print(myOutput)
```
---


Note how the input streams through a pipeline of three functions executed serially. Expressed sequentially as in a chain of functions:

| "   i n f i n i t e    s p a c e    " → | TextSkill.TrimStart → | TextSkill.TrimEnd → | TextSkill.Uppercase → |
|---|---|---|---|

The output reads as:

`I N F I N I T E     S P A C E`


### Take the next step

> [!div class="nextstepaction"]
> [Deploy your plugins to Azure](../deploy/deploy-to-azure.md)
