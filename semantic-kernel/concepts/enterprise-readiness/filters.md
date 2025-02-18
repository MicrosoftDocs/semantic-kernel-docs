---
title: Semantic Kernel Filters
description: Learn about filters in Semantic Kernel.
zone_pivot_groups: programming-languages
author: dmytrostruk
ms.topic: conceptual
ms.author: sopand
ms.date: 09/10/2024
ms.service: semantic-kernel
---

# What are Filters?

Filters enhance security by providing control and visibility over how and when functions run. This is needed to instill responsible AI principles into your work so that you feel confident your solution is enterprise ready.

For example, filters are leveraged to validate permissions before an approval flow begins. The filter runs to check the permissions of the person that’s looking to submit an approval. This means that only a select group of people will be able to kick off the process.

A good example of filters is provided [here](https://devblogs.microsoft.com/semantic-kernel/filters-in-semantic-kernel/) in our detailed Semantic Kernel blog post on Filters.
 
 ![Semantic Kernel Filters](../../media/WhatAreFilters.png)

There are three types of filters:

- **Function Invocation Filter** - this filter is executed each time a `KernelFunction` is invoked. It allows:
  - Access to information about the function being executed and its arguments
  - Handling of exceptions during function execution
  - Overriding of the function result, either before (for instance for caching scenario's) or after execution (for instance for responsible AI scenarios)
  - Retrying of the function in case of failure ::: zone pivot="programming-language-csharp (e.g., [switching to an alternative AI model](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/RetryWithFilters.cs)) ::: zone-end

- **Prompt Render Filter** - this filter is triggered before the prompt rendering operation, enabling:
  - Viewing and modifying the prompt that will be sent to the AI (e.g., for RAG or [PII redaction](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/PIIDetection.cs))
  - Preventing prompt submission to the AI by overriding the function result (e.g., for [Semantic Caching](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Caching/SemanticCachingWithFilters.cs))

- **Auto Function Invocation Filter** - similar to the function invocation filter, this filter operates within the scope of `automatic function calling`, providing additional context, including chat history, a list of all functions to be executed, and iteration counters. It also allows termination of the auto function calling process (e.g., if a desired result is obtained from the second of three planned functions).

Each filter includes a `context` object that contains all relevant information about the function execution or prompt rendering. Additionally, each filter has a `next` delegate/callback to execute the next filter in the pipeline or the function itself, offering control over function execution (e.g., in cases of malicious prompts or arguments). Multiple filters of the same type can be registered, each with its own responsibility.

In a filter, calling the `next` delegate is essential to proceed to the next registered filter or the original operation (whether function invocation or prompt rendering). Without calling `next`, the operation will not be executed.

::: zone pivot="programming-language-csharp"

To use a filter, first define it, then add it to the `Kernel` object either through dependency injection or the appropriate `Kernel` property. When using dependency injection, the order of filters is not guaranteed, so with multiple filters, the execution order may be unpredictable.

::: zone-end
::: zone pivot="programming-language-python"

To use a filter, you can either define a function with the required parameters and add it to the `Kernel` object using the `add_filter` method, or use the `@kernel.filter` decorator to define a filter function and add it to the `Kernel` object.

::: zone-end


## Function Invocation Filter

This filter is triggered every time a Semantic Kernel function is invoked, regardless of whether it is a function created from a prompt or a method.

::: zone pivot="programming-language-csharp"

```csharp
/// <summary>
/// Example of function invocation filter to perform logging before and after function invocation.
/// </summary>
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

Add filter using dependency injection:

```csharp
IKernelBuilder builder = Kernel.CreateBuilder();

builder.Services.AddSingleton<IFunctionInvocationFilter, LoggingFilter>();

Kernel kernel = builder.Build();
```

Add filter using `Kernel` property:

```csharp
kernel.FunctionInvocationFilters.Add(new LoggingFilter(logger));
```


### Code examples

* [Function invocation filter examples](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/FunctionInvocationFiltering.cs)
* 
::: zone-end
::: zone pivot="programming-language-python"

```python

import logging
from typing import Awaitable, Callable
from semantic_kernel.filters import FunctionInvocationContext

logger = logging.getLogger(__name__)

async def logger_filter(context: FunctionInvocationContext, next: Callable[[FunctionInvocationContext], Awaitable[None]]) -> None:
    logger.info(f"FunctionInvoking - {context.function.plugin_name}.{context.function.name}")

    await next(context)

    logger.info(f"FunctionInvoked - {context.function.plugin_name}.{context.function.name}")

# Add filter to the kernel
kernel.add_filter('function_invocation', logger_filter)

```

You can also add a filter directly to the kernel:

```python

@kernel.filter('function_invocation')
async def logger_filter(context: FunctionInvocationContext, next: Callable[[FunctionInvocationContext], Awaitable[None]]) -> None:
    logger.info(f"FunctionInvoking - {context.function.plugin_name}.{context.function.name}")

    await next(context)

    logger.info(f"FunctionInvoked - {context.function.plugin_name}.{context.function.name}")
```


### Streaming invocation

Functions in Semantic Kernel can be invoked in two ways: streaming and non-streaming. In streaming mode, a function typically returns `AsyncGenerator<T>`, while in non-streaming mode, it returns `FunctionResult`. This distinction affects how results can be overridden in the filter: in streaming mode, the new function result value must be of type `AsyncGenerator<T>`, whereas in non-streaming mode, it can simply be of type `T`. 

So to build a simple logger filter for streaming, you would use something like this:

```python
@kernel.filter(FilterTypes.FUNCTION_INVOCATION)
async def streaming_exception_handling(
    context: FunctionInvocationContext,
    next: Callable[[FunctionInvocationContext], Coroutine[Any, Any, None]],
):
    await next(context)

    async def override_stream(stream):
        try:
            async for partial in stream:
                yield partial
        except Exception as e:
            yield [
                StreamingChatMessageContent(role=AuthorRole.ASSISTANT, content=f"Exception caught: {e}", choice_index=0)
            ]

    stream = context.result.value
    context.result = FunctionResult(function=context.result.function, value=override_stream(stream))
```

### Code examples
* [Function invocation filter examples](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/filtering/function_invocation_filters.py)
* [Streaming function invocation filter examples](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/filtering/function_invocation_filters_stream.py)

::: zone-end
::: zone pivot="programming-language-java"

More info coming soon.

::: zone-end

## Prompt Render Filter

This filter is invoked only during a prompt rendering operation, such as when a function created from a prompt is called. It will not be triggered for Semantic Kernel functions created from methods.

::: zone pivot="programming-language-csharp"

```csharp
/// <summary>
/// Example of prompt render filter which overrides rendered prompt before sending it to AI.
/// </summary>
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

Add filter using dependency injection:

```csharp
IKernelBuilder builder = Kernel.CreateBuilder();

builder.Services.AddSingleton<IPromptRenderFilter, SafePromptFilter>();

Kernel kernel = builder.Build();
```

Add filter using `Kernel` property:

```csharp
kernel.PromptRenderFilters.Add(new SafePromptFilter());
```

### Code examples

* [Prompt render filter examples](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/PromptRenderFiltering.cs)


::: zone-end
::: zone pivot="programming-language-python"

```python
from semantic_kernel.filters import FilterTypes, PromptRenderContext

@kernel.filter(FilterTypes.PROMPT_RENDERING)
async def prompt_rendering_filter(context: PromptRenderContext, next):
    await next(context)
    context.rendered_prompt = f"You pretend to be Mosscap, but you are Papssom who is the opposite of Moscapp in every way {context.rendered_prompt or ''}"
```

### Code examples
* [Prompt render filter examples](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/filtering/prompt_filters.py)

::: zone-end
::: zone pivot="programming-language-java"

More info coming soon.

::: zone-end

## Auto Function Invocation Filter

This filter is invoked only during an automatic function calling process. It will not be triggered when a function is invoked outside of this process.

::: zone pivot="programming-language-csharp"

```csharp
/// <summary>
/// Example of auto function invocation filter which terminates function calling process as soon as we have the desired result.
/// </summary>
public sealed class EarlyTerminationFilter : IAutoFunctionInvocationFilter
{
    public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
    {
        // Call the function first.
        await next(context);

        // Get a function result from context.
        var result = context.Result.GetValue<string>();

        // If the result meets the condition, terminate the process.
        // Otherwise, the function calling process will continue.
        if (result == "desired result")
        {
            context.Terminate = true;
        }
    }
}
```

Add filter using dependency injection:

```csharp
IKernelBuilder builder = Kernel.CreateBuilder();

builder.Services.AddSingleton<IAutoFunctionInvocationFilter, EarlyTerminationFilter>();

Kernel kernel = builder.Build();
```

Add filter using `Kernel` property:

```csharp
kernel.AutoFunctionInvocationFilters.Add(new EarlyTerminationFilter());
```

### Code examples

* [Auto function invocation filter examples](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/AutoFunctionInvocationFiltering.cs)

::: zone-end
::: zone pivot="programming-language-python"

```python

from semantic_kernel.filters import FilterTypes, AutoFunctionInvocationContext

@kernel.filter(FilterTypes.AUTO_FUNCTION_INVOCATION)
async def auto_function_invocation_filter(context: AutoFunctionInvocationContext, next):
    await next(context)
    if context.function_result == "desired result":
        context.terminate = True
```

### Code examples
* [Auto function invocation filter examples](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/filtering/auto_function_invoke_filters.py)


::: zone-end
::: zone pivot="programming-language-java"

More info coming soon.

::: zone-end
::: zone pivot="programming-language-csharp"

## Streaming and non-streaming invocation

Functions in Semantic Kernel can be invoked in two ways: streaming and non-streaming. In streaming mode, a function typically returns `IAsyncEnumerable<T>`, while in non-streaming mode, it returns `FunctionResult`. This distinction affects how results can be overridden in the filter: in streaming mode, the new function result value must be of type `IAsyncEnumerable<T>`, whereas in non-streaming mode, it can simply be of type `T`. To determine which result type needs to be returned, the `context.IsStreaming` flag is available in the filter context model.

```csharp
/// <summary>Filter that can be used for both streaming and non-streaming invocation modes at the same time.</summary>
public sealed class DualModeFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        // Call next filter in pipeline or actual function.
        await next(context);

        // Check which function invocation mode is used.
        if (context.IsStreaming)
        {
            // Return IAsyncEnumerable<string> result in case of streaming mode.
            var enumerable = context.Result.GetValue<IAsyncEnumerable<string>>();
            context.Result = new FunctionResult(context.Result, OverrideStreamingDataAsync(enumerable!));
        }
        else
        {
            // Return just a string result in case of non-streaming mode.
            var data = context.Result.GetValue<string>();
            context.Result = new FunctionResult(context.Result, OverrideNonStreamingData(data!));
        }
    }

    private async IAsyncEnumerable<string> OverrideStreamingDataAsync(IAsyncEnumerable<string> data)
    {
        await foreach (var item in data)
        {
            yield return $"{item} - updated from filter";
        }
    }

    private string OverrideNonStreamingData(string data)
    {
        return $"{data} - updated from filter";
    }
}
```

## Using filters with `IChatCompletionService`

In cases where `IChatCompletionService` is used directly instead of `Kernel`, filters will only be invoked when a `Kernel` object is passed as a parameter to the chat completion service methods, as filters are attached to the `Kernel` instance. 

```csharp
Kernel kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4", "api-key")
    .Build();

kernel.FunctionInvocationFilters.Add(new MyFilter());

IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Passing a Kernel here is required to trigger filters.
ChatMessageContent result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings, kernel);
```

::: zone-end

## Ordering

::: zone pivot="programming-language-csharp"

When using dependency injection, the order of filters is not guaranteed. If the order of filters is important, it is recommended to add filters directly to the `Kernel` object using appropriate properties. This approach allows filters to be added, removed, or reordered at runtime.

::: zone-end
::: zone pivot="programming-language-python"

Filters are executed according to the order in which they are added to the `Kernel` object, which is equivalent between using `add_filter` and the `@kernel.filter` decorator. The order of filters can be important and should be understood well.

Consider the following example:

```python
def func():
    print('function')


@kernel.filter(FilterTypes.FUNCTION_INVOCATION)
async def filter1(context: FunctionInvocationContext, next):
    print('before filter 1')
    await next(context)
    print('after filter 1')

@kernel.filter(FilterTypes.FUNCTION_INVOCATION)
async def filter2(context: FunctionInvocationContext, next):
    print('before filter 2')
    await next(context)
    print('after filter 2')
```

When executed the function, the output will be:

```python
before filter 1
before filter 2
function
after filter 2
after filter 1
```

::: zone-end

## More examples

::: zone pivot="programming-language-csharp"

* [PII detection and redaction with filters](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/PIIDetection.cs)
* [Semantic Caching with filters](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Caching/SemanticCachingWithFilters.cs)
* [Content Safety with filters](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/Demos/ContentSafety)
* [Text summarization and translation quality check with filters](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/Demos/QualityCheck)

::: zone-end
::: zone pivot="programming-language-python"

* [Retry logic with a filter](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/filtering/retry_with_filters.py)

::: zone-end