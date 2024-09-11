---
title: Migrating to the new Function Calling model
description: Describes the steps for SK caller code to migrate from the current function calling model, represented by the `ToolCallBehavior` class, to the new one represented by the `FunctionChoiceBehavior` class.
zone_pivot_groups: programming-languages
author: SergeyMenshykh
ms.topic: conceptual
ms.author: SergeyMenshykh
ms.date: 12/09/2024
ms.service: semantic-kernel
---
::: zone pivot="programming-language-csharp"
# Overview
Semantic Kernel is gradually transitioning from the current function calling model, represented by the `ToolCallBehavior` class, to the new one, represented by the `FunctionChoiceBehavior` class. 
The new model provides more flexibility and control over the function calling process and is expected to reach general availability (GA) by mid-November 2024. The current model will be deprecated at the same time and is planned to be completely removed by the end of 2024.

This guide is intended to help you to migrate your code to the new function calling model.

## ToolCallBehavior.AutoInvokeKernelFunctions
The `ToolCallBehavior.AutoInvokeKernelFunctions` behavior is equivalent to the `FunctionChoiceBehavior.Auto` behavior in the new model. 
```csharp
// Before
var executionSettings = new OpenAIPromptExecutionSettings { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

// After
var executionSettings = new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
```

## ToolCallBehavior.EnableKernelFunctions
The `ToolCallBehavior.EnableKernelFunctions` behavior is equivalent to the `FunctionChoiceBehavior.Auto` behavior with disabled auto invocation. 
```csharp
// Before
var executionSettings = new OpenAIPromptExecutionSettings { ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions };

// After
var executionSettings = new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(autoInvoke: false) };
```

## ToolCallBehavior.EnableFunctions
The `ToolCallBehavior.EnableFunctions` behavior is equivalent to the `FunctionChoiceBehavior.Auto` behavior that configured with list of functions with disabled auto invocation. 
```csharp
var function = kernel.CreateFunctionFromMethod(() => DayOfWeek.Friday, "GetDayOfWeek", "Retunrs the current day of the week.");

// Before
var executionSettings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.EnableFunctions(functions: [function.Metadata.ToOpenAIFunction()]) };

// After
var executionSettings = new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions: [function], autoInvoke: false) };
```

## ToolCallBehavior.RequireFunction
The `ToolCallBehavior.RequireFunction` behavior is equivalent to the `FunctionChoiceBehavior.Required` behavior that configured with list of functions with disabled auto invocation.
```csharp
var function = kernel.CreateFunctionFromMethod(() => DayOfWeek.Friday, "GetDayOfWeek", "Retunrs the current day of the week.");

// Before
var executionSettings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.RequireFunction(functions: [function.Metadata.ToOpenAIFunction()]) };

// After
var executionSettings = new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Required(functions: [function], autoInvoke: false) };
```

## Next steps
Now after you have migrated your code to the new function calling model, you can proceed to learn how to configure various aspects of the model that might better correspond to your specific scenarios by referring to the [function choice behavior article](../concepts/ai-services/chat-completion/function-calling/function-choice-behavior.md)

> [!div class="nextstepaction"]
> [Function Choice Behavior](../concepts/ai-services/chat-completion/function-calling/function-choice-behavior.md)


::: zone-end
::: zone pivot="programming-language-python"
## Coming soon
More info coming soon.
::: zone-end
::: zone pivot="programming-language-java"
## Coming soon
More info coming soon.
::: zone-end