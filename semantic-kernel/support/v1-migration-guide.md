---
title: Upgrading to v1+ of the .NET Semantic Kernel SDK
description: Learn how to upgrade from a pre-v1 version of the .NET Semantic Kernel SDK to v1+.
author: rogerbarreto
ms.topic: tutorial
ms.author: rbarreto
ms.date: 02/15/2024
ms.service: semantic-kernel
---

# Semantic Kernel - .Net V1 Migration Guide

This guide is intended to help you upgrade from a pre-v1 version of the .NET Semantic Kernel SDK to v1+. The pre-v1 version used as a reference was the

## Package Changes

.. = Microsoft.SemanticKernel

| Previous Name                            | V1 Name                                     |
| ---------------------------------------- | ------------------------------------------- |
| ..Connectors.AI.HuggingFace              | ..Connectors.HuggingFace                    |
| ..Connectors.AI.OpenAI                   | ..Connectors.OpenAI                         |
| ..Connectors.AI.Oobabooga                | MyIA.SemanticKernel.Connectors.AI.Oobabooga |
| ..Connectors.Memory.Kusto                | ..Connectors.Kusto                          |
| ..Connectors.Memory.DuckDB               | ..Connectors.DuckDB                         |
| ..Connectors.Memory.Pinecone             | ..Connectors.Pinecone                       |
| ..Connectors.Memory.Redis                | ..Connectors.Redis                          |
| ..Connectors.Memory.Qdrant               | ..Connectors.Qdrant                         |
| ..Connectors.Memory.AzureCognitiveSearch | ..Connectors.Memory.AzureAISearch           |
| ..Functions.Semantic                     | - Removed -                                 |
| ..Reliability.Basic                      | - Removed -                                 |
| ..Reliability.Polly                      | - Removed -                                 |
| ..TemplateEngine.Basic                   | Merged in Core                              |
| ..Planners.Core                          | ..Planners.OpenAI<br>Planners.Handlebars    |
| --                                       | ..Experimental.Agents                       |
| --                                       | ..Experimental.Orchestration.Flow           |

## Package Removal and Changes needed

Ensure that if you use any of the packages bellow you match the latest version that V1 uses:

| Package Name                                            | Version |
| ------------------------------------------------------- | ------- |
| Microsoft.Extensions.Configuration                      | 8.0.0   |
| Microsoft.Extensions.Configuration.Binder               | 8.0.0   |
| Microsoft.Extensions.Configuration.EnvironmentVariables | 8.0.0   |
| Microsoft.Extensions.Configuration.Json                 | 8.0.0   |
| Microsoft.Extensions.Configuration.UserSecrets          | 8.0.0   |
| Microsoft.Extensions.DependencyInjection                | 8.0.0   |
| Microsoft.Extensions.DependencyInjection.Abstractions   | 8.0.0   |
| Microsoft.Extensions.Http                               | 8.0.0   |
| Microsoft.Extensions.Http.Resilience                    | 8.0.0   |
| Microsoft.Extensions.Logging                            | 8.0.0   |
| Microsoft.Extensions.Logging.Abstractions               | 8.0.0   |
| Microsoft.Extensions.Logging.Console                    | 8.0.0   |

### Reliability.Basic Package

The Reliability Basic package was removed and the same functionality can be achieved using the .net dependency injection `ConfigureHttpClientDefaults` service collection extension to inject the desired resiliency policies to the `HttpClient` instances.

```csharp
// Before
var retryConfig = new BasicRetryConfig
{
    MaxRetryCount = 3,
    UseExponentialBackoff = true,
};
retryConfig.RetryableStatusCodes.Add(HttpStatusCode.Unauthorized);
var kernel = new KernelBuilder().WithRetryBasic(retryConfig).Build();
```

```csharp
// After
builder.Services.ConfigureHttpClientDefaults(c =>
{
    // Use a standard resiliency policy, augmented to retry on 401 Unauthorized for this example
    c.AddStandardResilienceHandler().Configure(o =>
    {
        o.Retry.ShouldHandle = args => ValueTask.FromResult(args.Outcome.Result?.StatusCode is HttpStatusCode.Unauthorized);
    });
});

```

## Convention name changes

| Previous Name     | V1 Name         |
| ----------------- | --------------- |
| Semantic Function | Prompt Function |
| Native Function   | Method Function |
| Context Variable  | Kernel Argument |

## Code name changes

| Previous Name                               | V1 Name                                |
| ------------------------------------------- | -------------------------------------- |
| ContextVariables                            | KernelArguments                        |
| ContextVariables.Set                        | KernelArguments.Add                    |
| IImageGenerationService                     | ITextToImageService                    |
| ITextCompletionService                      | ITextGenerationService                 |
| Kernel.CreateSemanticFunction               | Kernel.CreateFunctionFromPrompt        |
| Kernel.ImportFunctions                      | Kernel.ImportPluginFrom\_\_\_\_        |
| Kernel.ImportSemanticFunctionsFromDirectory | Kernel.ImportPluginFromPromptDirectory |
| Kernel.RunAsync                             | Kernel.InvokeAsync                     |
| NativeFunction                              | MethodFunction                         |
| OpenAIRequestSettings                       | OpenAIPromptExecutionSettings          |
| RequestSettings                             | PromptExecutionSettings                |
| SKException                                 | KernelException                        |
| SKFunction                                  | KernelFunction                         |
| SKFunctionMetadata                          | KernelFunctionAttribute                |
| SKJsonSchema                                | KernelJsonSchema                       |
| SKParameterMetadata                         | KernelParameterMetadata                |
| SKPluginCollection                          | KernelPluginCollection                 |
| SKReturnParameterMetadata                   | KernelReturnParameterMetadata          |
| SemanticFunction                            | PromptFunction                         |

## Namespace Simplifications

| Previous Name                          | V1 Name                  |
| -------------------------------------- | ------------------------ |
| Microsoft.SemanticKernel.Orchestration | Microsoft.SemanticKernel |

## New Concepts

- **Plugin Entity**: Before V1 there was no concept of a plugin centric entity. This changed in V1 and for any function you add to a Kernel you will get a Plugin that it belongs to.

## Kernel

- `IKernel` interface was changed to `Kernel` class.

- `Kernel.ImportFunctions` was removed and replaced by `Kernel.ImportPluginFrom____`, where `____` can be `Functions`, `Object`, `PromptDirectory`, `Type`, `Grp` or `OpenAIAsync`, etc.

  ```csharp
  // Before
  var textFunctions = kernel.ImportFunctions(new StaticTextPlugin(), "text");

  // After
  var textFunctions = kernel.ImportPluginFromObject(new StaticTextPlugin(), "text");
  ```

- `Kernel.RunAsync` was removed and replaced by `Kernel.InvokeAsync`. Order of parameters shifted, where function is the first.

  ```csharp
  // Before
  KernelResult result = kernel.RunAsync(textFunctions["Uppercase"], "Hello World!");

  // After
  FunctionResult result = kernel.InvokeAsync(textFunctions["Uppercase"], new() { ["input"] = "Hello World!" });
  ```

- `Kernel.InvokeAsync` now returns a `FunctionResult` instead of a `KernelResult`.

- `Kernel.InvokeAsync` only targets one function per call as first parameter. Pipelining is not supported, use the [Example 60](https://github.com/microsoft/semantic-kernel/blob/b9c1adc4834c0e560c429fa035054ab51d738bdf/dotnet/samples/KernelSyntaxExamples/Example60_AdvancedMethodFunctions.cs#L48) to achieve a chaining behavior.

  ❌ Not supported

  ```csharp
  KernelResult result = await kernel.RunAsync(" Hello World! ",
      textFunctions["TrimStart"],
      textFunctions["TrimEnd"],
      textFunctions["Uppercase"]);
  ```

  ✔️ One function per call

  ```csharp
  var trimStartResult = await kernel.InvokeAsync(textFunctions["TrimStart"], new() { ["input"] = " Hello World! " });
  var trimEndResult = await kernel.InvokeAsync(textFunctions["TrimEnd"], new() { ["input"] = trimStartResult.GetValue<string>() });
  var finalResult = await kernel.InvokeAsync(textFunctions["Uppercase"], new() { ["input"] = trimEndResult.GetValue<string>() });
  ```

  ✔️ Chaining using plugin Kernel injection

  ```csharp
  // Plugin using Kernel injection
  public class MyTextPlugin
  {
      [KernelFunction]
      public async Task<string> Chain(Kernel kernel, string input)
      {
          var trimStartResult = await kernel.InvokeAsync("textFunctions", "TrimStart", new() { ["input"] = input });
          var trimEndResult = await kernel.InvokeAsync("textFunctions", "TrimEnd", new() { ["input"] = trimStartResult.GetValue<string>() });
          var finalResult = await kernel.InvokeAsync("textFunctions", "Uppercase", new() { ["input"] = trimEndResult.GetValue<string>() });

          return finalResult.GetValue<string>();
      }
  }

  var plugin = kernel.ImportPluginFromObject(new MyTextPlugin(), "textFunctions");
  var finalResult = await kernel.InvokeAsync(plugin["Chain"], new() { ["input"] = " Hello World! "});
  ```

- `Kernel.InvokeAsync` does not accept string as input anymore, use a `KernelArguments` instance instead. The function now is the first argument and the input argument needs to be provided as a `KernelArguments` instance.

  ```csharp
  // Before
  var result = await kernel.RunAsync("I missed the F1 final race", excuseFunction);

  // After
  var result = await kernel.InvokeAsync(excuseFunction, new() { ["input"] = "I missed the F1 final race" });
  ```

- `Kernel.ImportSemanticFunctionsFromDirectory` was removed and replaced by `Kernel.ImportPluginFromPromptDirectory`.

- `Kernel.CreateSemanticFunction` was removed and replaced by `Kernel.CreateFunctionFromPrompt`.
  - Arguments: `OpenAIRequestSettings` is now `OpenAIPromptExecutionSettings`

## Context Variables

`ContextVariables` was redefined as`KernelArguments` and is now a dictionary, where the key is the name of the argument and the value is the value of the argument. Methods like `Set` and `Get` were removed and the common dictionary Add or the indexer `[]` to set and get values should be used instead.

```csharp
// Before
var variables = new ContextVariables("Today is: ");
variables.Set("day", DateTimeOffset.Now.ToString("dddd", CultureInfo.CurrentCulture));

// After
var arguments = new KernelArguments() {
  ["input"] = "Today is: ",
  ["day"] = DateTimeOffset.Now.ToString("dddd", CultureInfo.CurrentCulture)
};

// Initialize directly or use the dictionary indexer below
arguments["day"] = DateTimeOffset.Now.ToString("dddd", CultureInfo.CurrentCulture);
```

## Kernel Builder

- Creating a `KernelBuilder` can now be only created using the `Kernel.CreateBuilder()` method.

  ```csharp
  // Before
  IKernel kernel = new KernelBuilder().Build();

  // After
  var builder = Kernel.CreateBuilder().Build();
  ```

- `KernelBuilder.WithOpenAIChatCompletionService` was renamed to `KernelBuilder.AddOpenAIChatCompletion`

- `KernelBuilder.WithLoggerFactory` is not more used, instead use dependency injection approach to add the logger factory.

  ```csharp
  IKernelBuilder builder = Kernel.CreateBuilder();
  builder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Information));
  ```

## Kernel Result

There is no more specific `KernelResult`, use the `FunctionResult` instead.

## SKContext

SKContext has been replaced in all of the method signatures that required it as an input parameter with your Kernel instance.

## Usage of Experimental Attribute Feature.

This features was introduced to mark some functionalities in V1 that we can possibly change or completely remove.

For mode details one the list of current released experimental features [check here](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/docs/EXPERIMENTS.md).
