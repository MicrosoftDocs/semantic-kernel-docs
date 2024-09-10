---
title: Semantic Kernel Filters
description: Learn about filters in Semantic Kernel.
author: sophialagerkranspandey, dmytrostruk
ms.topic: conceptual
ms.author: sopand, dmytrostruk
ms.date: 09/10/2024
ms.service: semantic-kernel
---

# What are Filters?

Filters enhance security by providing control and visibility over how and when functions run. This is needed to instill responsible AI principles into your work so that you feel confident your solution is enterprise ready.

For example, filters are leveraged to validate permissions before an approval flow begins. The `IFunctionInvocationFilter` is run to check the permissions of the person that’s looking to submit an approval. This means that only a select group of people will be able to kick off the process.

A good example of filters is provided [here](https://devblogs.microsoft.com/semantic-kernel/filters-in-semantic-kernel/) in our detailed Semantic Kernel blog post on Filters.
 
 ![Semantic Kernel Filters](../media/WhatAreFilters.png)

There are 3 types of filters:

- Function invocation filter - it's executed every time `KernelFunction` is invoked. Allows to get information about function which is going to be executed, its arguments, catch an exception during function execution, override function result, retry function execution in case of failure (can be used to [switch to other AI model](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/RetryWithFilters.cs)).
- Prompt render filter - it's executed before prompt rendering operation. Allows to see what prompt is going to be sent to AI, modify prompt (e.g. RAG, [PII redaction](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/PIIDetection.cs) scenarios) and prevent the prompt from being sent to AI with function result override (can be used for [Semantic Caching](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Caching/SemanticCachingWithFilters.cs)).
- Auto function invocation filter - similar to function invocation filter, but it is executed in a scope of `automatic function calling` operation, so it has more information available in a context, including chat history, list of all functions that will be executed and request iteration counters. It also allows to terminate auto function calling process (e.g. there are 3 functions to execute, but there is already the desired result from the second function).

Each filter has `context` object that contains all information related to function execution or prompt rendering. Together with context, there is also a `next` delegate/callback, which executes next filter in pipeline or function itself. This provides more control, and it is useful in case there are some reasons to avoid function execution (e.g. malicious prompt or function arguments). It is possible to register multiple filters of the same type, where each filter will have different responsibility.

Example of function invocation filter to perform logging before and after function invocation:

```csharp
public sealed class LoggingFilter(ILogger logger) : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        logger.LogInformation("FunctionInvoking - {PluginName}.{FunctionName}", context.Function.PluginName, context.Function.Name);

        await next(context);

        logger.LogInformation("FunctionInvoked - {PluginName}.{FunctionName}", context.Function.PluginName, context.Function.Name);
    }
}
```

Example of prompt render filter which overrides rendered prompt before sending it to AI:

```csharp
public class SafePromptFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        // Example: get function information
        var functionName = context.Function.Name;

        await next(context);

        // Example: override rendered prompt before sending it to AI
        context.RenderedPrompt = "Safe prompt";
    }
}
```

Example of auto function invocation filter which terminates function calling process as soon as we have the desired result:

```csharp
public sealed class EarlyTerminationFilter : IAutoFunctionInvocationFilter
{
    public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
    {
        await next(context);

        var result = context.Result.GetValue<string>();

        if (result == "desired result")
        {
            context.Terminate = true;
        }
    }
}
```

## More information

C#/.NET:
* [Function invocation filter examples](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/FunctionInvocationFiltering.cs)
* [Prompt render filter examples](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/PromptRenderFiltering.cs)
* [Auto function invocation filter examples](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/AutoFunctionInvocationFiltering.cs)
* [PII detection and redaction with filters](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/PIIDetection.cs)
* [Semantic Caching with filters](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Caching/SemanticCachingWithFilters.cs)
* [Content Safety with filters](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/Demos/ContentSafety)
* [Text summarization and translation quality check with filters](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/Demos/QualityCheck)