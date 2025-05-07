---
title: Functions.Markdown NuGet Package Migration Guide
description: Describes the steps to migrate from the APIs of the Functions.Markdown NuGet package to the APIs of the Functions.Yaml package.
author: SergeyMenshykh
ms.topic: conceptual
ms.author: semenshi
ms.date: 05/07/2025
ms.service: semantic-kernel
---

# Functions.Markdown NuGet Package Migration Guide

The Functions.Markdown NuGet package is deprecated and will be removed in a future release as part of the clean-up initiative. The recommended replacement is the Functions.Yaml package.

## Markdown Prompt Templates

Before migrating your code to the new APIs from the Functions.Yaml package, consider migrating your markdown prompt templates to the new YAML format first. So, if you have a Markdown prompt template like this:

````markdown
This is a semantic kernel prompt template
```sk.prompt
Hello AI, tell me about {{$input}}
```
```sk.execution_settings
{
    "service1" : {
        "model_id": "gpt4",
        "temperature": 0.7,
        "function_choice_behavior": {
            "type": "auto",
        }
    }
}
```
```sk.execution_settings
{
    "service2" : {
        "model_id": "gpt-4o-mini",
        "temperature": 0.7
    }
}
````

the YAML equivalent prompt template would look like this:
```yaml
name: TellMeAbout
description: This is a semantic kernel prompt template
template: Hello AI, tell me about {{$input}}
template_format: semantic-kernel
execution_settings:
  service1:
    model_id: gpt4
    temperature: 0.7
    function_choice_behavior:
      type: auto
  service2:
    model_id: gpt-4o-mini
    temperature: 0.7
```

## KernelFunctionMarkdown.FromPromptMarkdown method

If your code uses the `KernelFunctionMarkdown.FromPromptMarkdown` method to create a Kernel Function from prompt, replace it with the `KernelFunctionYaml.FromPromptYaml` method:

````csharp
// Before
string promptTemplateConfig = """
This is a semantic kernel prompt template
```sk.prompt
Hello AI, tell me about {{$input}}
```
""";

KernelFunction function = KernelFunctionMarkdown.FromPromptMarkdown(promptTemplateConfig, "TellMeAbout");

//After
string promptTemplateConfig = 
"""
name: TellMeAbout
description: This is a semantic kernel prompt template
template: Hello AI, tell me about {{$input}}
""";

KernelFunction function = KernelFunctionYaml.FromPromptYaml(promptTemplateConfig);
````
Notice that the `KernelFunctionYaml.FromPromptYaml` method does not accept function name as a parameter. The function name is now part of the YAML configuration.

## MarkdownKernelExtensions.CreateFunctionFromMarkdown method

Similarly, if your code uses the `MarkdownKernelExtensions.CreateFunctionFromMarkdown` Kernel extension method to create a Kernel Function from prompt, replace it with the `PromptYamlKernelExtensions.CreateFunctionFromPromptYaml` method:

````csharp
// Before
string promptTemplateConfig = """
This is a semantic kernel prompt template
```sk.prompt
Hello AI, tell me about {{$input}}
```
""";

Kernel kernel = new Kernel();

KernelFunction function = kernel.CreateFunctionFromMarkdown(promptTemplateConfig, "TellMeAbout");

//After
string promptTemplateConfig = 
"""
name: TellMeAbout
description: This is a semantic kernel prompt template
template: Hello AI, tell me about {{$input}}
""";

Kernel kernel = new Kernel();

KernelFunction function = kernel.CreateFunctionFromPromptYaml(promptTemplateConfig);
````
Notice that the `PromptYamlKernelExtensions.CreateFunctionFromPromptYaml` method does not accept function name as a parameter. The function name is now part of the YAML configuration.