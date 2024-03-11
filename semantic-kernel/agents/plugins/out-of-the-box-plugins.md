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

To provide a degree of standardization across Semantic Kernel implementations, the GitHub repo has several plugins available out-of-the-box depending on the language you are using. These plugins are often referred to as **Core plugins**. Additionally, each library also includes a handful of other plugins that you can use. The following section covers each set of plugins in more detail.

## Core plugins
The core plugins are planned to be available in all languages since they are core to using Semantic Kernel. Below are the core plugins currently available in Semantic Kernel along with their current support for each language. The ❌ symbol indicates that the feature is not yet available in that language; if you would like to see a feature implemented in a language, please consider [contributing to the project](../../get-started/contributing.md) or [opening an issue](../../get-started/contributing.md#reporting-issues).

| Plugin | Description | C# | Python | Java |
| --- | --- | :------:|:----: | :----: |
| `ConversationSummaryPlugin` | To summarize a conversation | ✅ | ✅ | * |
| `FileIOPlugin` | To read and write to the filesystem | ✅ | ✅ | ❌ |
| `HttpPlugin` | To call APIs | ✅ | ✅ | ❌ |
| `MathPlugin` | To perform mathematical operations | ✅ | ✅ | ❌ |
| `TextMemoryPlugin` | To store and retrieve text in memory | ✅ | ✅ | ❌ |
| `TextPlugin` | To deterministically manipulating text strings | ✅ | ✅ | * |
| `TimePlugin` | To acquire the time of day and any other temporal information | ✅ | ✅ | * |
| `WaitPlugin` | To pause execution for a specified amount of time | ✅ | ✅ | ❌ |

You can find the full list of core plugins for each language by following the links below:
- [C# core plugins](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Plugins/Plugins.Core)
- [Python core plugins](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/core_plugins)

### Using core plugins in Semantic Kernel
If you want to use one of the core plugins, you can easily import them into your project. For example, if you want to use the `TimePlugin` in either C# or Python, you can import it as follows.

# [C#](#tab/Csharp)

When using a core plugin, be sure to install the [Microsoft.SemanticKernel.Plugins.Core](https://www.nuget.org/packages/Microsoft.SemanticKernel.Plugins.Core/) nuget package, and include a `using Microsoft.SemanticKernel.Plugins.Core`:

```csharp
using Microsoft.SemanticKernel.Plugins.Core;

// Instantiate a kernelBuilder and configure it

kernelBuilder.Plugins.AddFromType<TimePlugin>();
var kernel = kernelBuilder.Build();

const string promptTemplate = @"
Today is: {{time.Date}}
Current time is: {{time.Time}}

Answer to the following questions using JSON syntax, including the data used.
Is it morning, afternoon, evening, or night (morning/afternoon/evening/night)?
Is it weekend time (weekend/not weekend)?";

var results = await kernel.InvokePromptAsync(promptTemplate);
Console.WriteLine(results);
```


# [Python](#tab/python)

```python
from semantic_kernel.core_skills.time_skill import TimePlugin

# ... instantiate a kernel and configure it first

kernel.import_skill(TimePlugin(), "time")

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

### Take the next step

> [!div class="nextstepaction"]
> [Learn about Chat Copilot](../../chat-copilot/index.md)
