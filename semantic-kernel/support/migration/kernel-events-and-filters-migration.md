---
title: Kernel Events and Filters Migration
description: Describes the steps for SK caller code to migrate from Kernel Events and use latest version of Filters
author: dmytrostruk
ms.topic: conceptual
ms.author: dmytrostruk
ms.date: 11/18/2024
ms.service: semantic-kernel
---

# Kernel Events and Filters Migration

> [!NOTE]
> This document addresses functionality from Semantic Kernel versions prior to v1.10.0. For the latest information about Filters, refer to this [documentation](../../concepts/enterprise-readiness/filters.md).

Semantic Kernel enables control over function execution using Filters. Over time, multiple versions of the filtering logic have been introduced: starting with Kernel Events, followed by the first version of Filters (`IFunctionFilter`, `IPromptFilter`), and culminating in the latest version (`IFunctionInvocationFilter`, `IPromptRenderFilter`). This guide explains how to migrate from Kernel Events and the first version of Filters to the latest implementation.

## Migration from Kernel Events

Kernel Events were the initial implementation for intercepting function operations in Semantic Kernel. They were deprecated in version 1.2.0. The examples below illustrate how to transition to the new function filtering logic.

### Function Invocation

Old implementation with Kernel Events:

```csharp
Kernel kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(
        modelId: "model-id",
        apiKey: "api-key")
    .Build();

void PreHandler(object? sender, FunctionInvokingEventArgs e)
{
    Console.WriteLine($"Function {e.Function.Name} is about to be invoked.");
}

void PostHandler(object? sender, FunctionInvokedEventArgs e)
{
    Console.WriteLine($"Function {e.Function.Name} was invoked.");
}

kernel.FunctionInvoking += PreHandler;
kernel.FunctionInvoked += PostHandler;

var result = await kernel.InvokePromptAsync("Write a random paragraph about universe.");

Console.WriteLine($"Function Result: {result}");
```

New implementation with function invocation filter:

```csharp
public sealed class FunctionInvocationFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        Console.WriteLine($"Function {context.Function.Name} is about to be invoked.");
        await next(context);
        Console.WriteLine($"Function {context.Function.Name} was invoked.");
    }
}

IKernelBuilder kernelBuilder = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(
        modelId: "model-id",
        apiKey: "api-key");

// Option 1: Add filter via Dependency Injection (DI)
kernelBuilder.Services.AddSingleton<IFunctionInvocationFilter, FunctionInvocationFilter>();

Kernel kernel = kernelBuilder.Build();

// Option 2: Add filter directly to the Kernel instance
kernel.FunctionInvocationFilters.Add(new FunctionInvocationFilter());

var result = await kernel.InvokePromptAsync("Write a random paragraph about universe.");

Console.WriteLine($"Function Result: {result}");
```

Alternate implementation with inline logic:

```csharp
public sealed class FunctionInvocationFilter(Func<FunctionInvocationContext, Func<FunctionInvocationContext, Task>, Task> onFunctionInvocation) : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        await onFunctionInvocation(context, next);
    }
}

Kernel kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(
        modelId: "model-id",
        apiKey: "api-key")
    .Build();

kernel.FunctionInvocationFilters.Add(new FunctionInvocationFilter(async (context, next) =>
{
    Console.WriteLine($"Function {context.Function.Name} is about to be invoked.");
    await next(context);
    Console.WriteLine($"Function {context.Function.Name} was invoked.");
}));

var result = await kernel.InvokePromptAsync("Write a random paragraph about universe.");

Console.WriteLine($"Function Result: {result}");
```

### Prompt Rendering

Old implementation with Kernel Events:

```csharp
Kernel kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(
        modelId: "model-id",
        apiKey: "api-key")
    .Build();

void RenderingHandler(object? sender, PromptRenderingEventArgs e)
{
    Console.WriteLine($"Prompt rendering for function {e.Function.Name} is about to be started.");
}

void RenderedHandler(object? sender, PromptRenderedEventArgs e)
{
    Console.WriteLine($"Prompt rendering for function {e.Function.Name} has completed.");
    e.RenderedPrompt += " USE SHORT, CLEAR, COMPLETE SENTENCES.";
}

kernel.PromptRendering += RenderingHandler;
kernel.PromptRendered += RenderedHandler;

var result = await kernel.InvokePromptAsync("Write a random paragraph about universe.");

Console.WriteLine($"Function Result: {result}");
```

New implementation with prompt render filter:

```csharp
public sealed class PromptRenderFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        Console.WriteLine($"Prompt rendering for function {context.Function.Name} is about to be started.");
        await next(context);
        Console.WriteLine($"Prompt rendering for function {context.Function.Name} has completed.");

        context.RenderedPrompt += " USE SHORT, CLEAR, COMPLETE SENTENCES.";
    }
}

IKernelBuilder kernelBuilder = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(
        modelId: "model-id",
        apiKey: "api-key");

// Option 1: Add filter via DI
kernelBuilder.Services.AddSingleton<IPromptRenderFilter, PromptRenderFilter>();

Kernel kernel = kernelBuilder.Build();

// Option 2: Add filter directly to the Kernel instance
kernel.PromptRenderFilters.Add(new PromptRenderFilter());

var result = await kernel.InvokePromptAsync("Write a random paragraph about universe.");

Console.WriteLine($"Function Result: {result}");
```

Inline logic example:

```csharp
public sealed class PromptRenderFilter(Func<PromptRenderContext, Func<PromptRenderContext, Task>, Task> onPromptRender) : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        await onPromptRender(context, next);
    }
}

Kernel kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(
        modelId: "model-id",
        apiKey: "api-key")
    .Build();

kernel.PromptRenderFilters.Add(new PromptRenderFilter(async (context, next) =>
{
    Console.WriteLine($"Prompt rendering for function {context.Function.Name} is about to be started.");
    await next(context);
    Console.WriteLine($"Prompt rendering for function {context.Function.Name} has completed.");

    context.RenderedPrompt += " USE SHORT, CLEAR, COMPLETE SENTENCES.";
}));

var result = await kernel.InvokePromptAsync("Write a random paragraph about universe.");

Console.WriteLine($"Function Result: {result}");
```

## Migration from Filters v1

The first version of Filters introduced a structured approach for function and prompt interception but lacked support for asynchronous operations and consolidated pre/post-operation handling. These limitations were addressed in Semantic Kernel v1.10.0.

### Function Invocation

Filters v1 syntax:

```csharp
public sealed class MyFilter : IFunctionFilter
{
    public void OnFunctionInvoking(FunctionInvokingContext context)
    {
        // Method which is executed before function invocation.
    }

    public void OnFunctionInvoked(FunctionInvokedContext context)
    {
        // Method which is executed after function invocation.
    }
}
```

Updated syntax:

```csharp
public sealed class FunctionInvocationFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        // Perform some actions before function invocation
        await next(context);
        // Perform some actions after function invocation
    }
}
```

### Prompt Rendering

Filters v1 syntax:

```csharp
public sealed class PromptFilter : IPromptFilter
{
    public void OnPromptRendering(PromptRenderingContext context)
    {
        // Perform some actions before prompt rendering
    }

    public void OnPromptRendered(PromptRenderedContext context)
    {
        // Perform some actions after prompt rendering
    }
}
```

Updated syntax:

```csharp
public class PromptFilter: IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        // Perform some actions before prompt rendering
        await next(context);
        // Perform some actions after prompt rendering
    }
}
```

For the latest information about Filters, refer to this [documentation](../../concepts/enterprise-readiness/filters.md).
