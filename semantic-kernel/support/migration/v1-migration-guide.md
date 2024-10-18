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

> [!NOTE]
> This document is not final and will get increasingly better!

This guide is intended to help you upgrade from a pre-v1 version of the .NET Semantic Kernel SDK to v1+.
The pre-v1 version used as a reference for this document was the `0.26.231009` version which was the last version before the first beta release where the majority of the changes started to happen.

## Package Changes

As a result of many packages being redefined, removed and renamed, also considering that we did a good cleanup and namespace simplification many of our old packages needed to be renamed, deprecated and removed. The table below shows the changes in our packages.

All packages that start with `Microsoft.SemanticKernel` were truncated with a `..` prefix for brevity.

| Previous Name                            | V1 Name                                     | Version | Reason                                                    |
| ---------------------------------------- | ------------------------------------------- | ------- | --------------------------------------------------------- |
| ..Connectors.AI.HuggingFace              | ..Connectors.HuggingFace                    | preview |                                                           |
| ..Connectors.AI.OpenAI                   | ..Connectors.OpenAI                         | v1      |                                                           |
| ..Connectors.AI.Oobabooga                | MyIA.SemanticKernel.Connectors.AI.Oobabooga | alpha   | Community driven connector <br/> ⚠️ Not ready for v1+ yet |
| ..Connectors.Memory.Kusto                | ..Connectors.Kusto                          | alpha   |                                                           |
| ..Connectors.Memory.DuckDB               | ..Connectors.DuckDB                         | alpha   |                                                           |
| ..Connectors.Memory.Pinecone             | ..Connectors.Pinecone                       | alpha   |                                                           |
| ..Connectors.Memory.Redis                | ..Connectors.Redis                          | alpha   |                                                           |
| ..Connectors.Memory.Qdrant               | ..Connectors.Qdrant                         | alpha   |                                                           |
| --                                       | ..Connectors.Postgres                       | alpha   |                                                           |
| ..Connectors.Memory.AzureCognitiveSearch | ..Connectors.Memory.AzureAISearch           | alpha   |                                                           |
| ..Functions.Semantic                     | - Removed -                                 |         | Merged in Core                                            |
| ..Reliability.Basic                      | - Removed -                                 |         | Replaced by .NET Dependency Injection                     |
| ..Reliability.Polly                      | - Removed -                                 |         | Replaced by .NET Dependency Injection                     |
| ..TemplateEngine.Basic                   | - Removed -                                 |         | Merged in Core                                            |
| ..Planners.Core                          | ..Planners.OpenAI<br>Planners.Handlebars    | preview |                                                           |
| --                                       | ..Experimental.Agents                       | alpha   |                                                           |
| --                                       | ..Experimental.Orchestration.Flow           | v1      |                                                           |

### Reliability Packages - Replaced by .NET Dependency Injection

The Reliability Basic and Polly packages now can be achieved using the .net dependency injection `ConfigureHttpClientDefaults` service collection extension to inject the desired resiliency policies to the `HttpClient` instances.

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

## Package Removal and Changes Needed

Ensure that if you use any of the packages below you match the latest version that V1 uses:

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

## Convention Name Changes

Many of our internal naming conventions were changed to better reflect how the AI community names things. As OpenAI started the massive shift and terms like Prompt, Plugins, Models, RAG were taking shape it was clear that we needed to align with those terms to make it easier for the community to understand use the SDK.

| Previous Name     | V1 Name                   |
| ----------------- | ------------------------- |
| Semantic Function | Prompt Function           |
| Native Function   | Method Function           |
| Context Variable  | Kernel Argument           |
| Request Settings  | Prompt Execution Settings |
| Text Completion   | Text Generation           |
| Image Generation  | Text to Image             |
| Skill             | Plugin                    |

## Code Name Changes

Following the convetion name changes, many of the code names were also changed to better reflect the new naming conventions. Abbreaviations were also removed to make the code more readable.

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
| SKContext                                   | FunctionResult (output)                |

## Namespace Simplifications

The old namespaces before had a deep hierarchy matching 1:1 the directory names in the projects. This is a common practice but did mean that consumers of the Semantic Kernel packages had to add a lot of different `using`'s in their code. We decided to reduce the number of namespaces in the Semantic Kernel packages so the majority of the functionality is in the main `Microsoft.SemanticKernel` namespace. See below for more details.

| Previous Name                                         | V1 Name                                         |
| ----------------------------------------------------- | ----------------------------------------------- |
| Microsoft.SemanticKernel.Orchestration                | Microsoft.SemanticKernel                        |
| Microsoft.SemanticKernel.Connectors.AI.\*             | Microsoft.SemanticKernel.Connectors.\*          |
| Microsoft.SemanticKernel.SemanticFunctions            | Microsoft.SemanticKernel                        |
| Microsoft.SemanticKernel.Events                       | Microsoft.SemanticKernel                        |
| Microsoft.SemanticKernel.AI.\*                        | Microsoft.SemanticKernel.\*                     |
| Microsoft.SemanticKernel.Connectors.AI.OpenAI.\*      | Microsoft.SemanticKernel.Connectors.OpenAI      |
| Microsoft.SemanticKernel.Connectors.AI.HuggingFace.\* | Microsoft.SemanticKernel.Connectors.HuggingFace |

## Kernel

The code to create and use a `Kernel` instance has been simplified. The `IKernel` interface has been eliminated as developers should not need to create their own `Kernel` implementation. The `Kernel` class represents a collection of services and plugins. The current `Kernel` instance is available everywhere which is consistent with the design philosophy behind the Semantic Kernel.

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

Many changes were made to our KernelBuilder to make it more intuitive and easier to use, as well as to make it simpler and more aligned with the .NET builders approach.

- Creating a `KernelBuilder` can now be only created using the `Kernel.CreateBuilder()` method.

  This change make it simpler and easier to use the KernelBuilder in any code-base ensureing one main way of using the builder instead of multiple ways that adds complexity and maintenance overhead.

  ```csharp
  // Before
  IKernel kernel = new KernelBuilder().Build();

  // After
  var builder = Kernel.CreateBuilder().Build();
  ```

- `KernelBuilder.With...` was renamed to `KernelBuilder.Add...`

  - `WithOpenAIChatCompletionService` was renamed to `AddOpenAIChatCompletionService`
  - `WithAIService<ITextCompletion>`

- `KernelBuilder.WithLoggerFactory` is not more used, instead use dependency injection approach to add the logger factory.

  ```csharp
  IKernelBuilder builder = Kernel.CreateBuilder();
  builder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Information));
  ```

- `WithAIService<T>` Dependency Injection

  Previously the `KernelBuilder` had a method `WithAIService<T>` that was removed and a new `ServiceCollection Services` property is exposed to allow the developer to add services to the dependency injection container. i.e.:

  ```csharp
  builder.Services.AddSingleton<ITextGenerationService>()
  ```

## Kernel Result

As the Kernel became just a container for the plugins and now executes just one function there was not more need to have a `KernelResult` entity and all function invocations from Kernel now return a `FunctionResult`.

## SKContext

After a lot of discussions and feedback internally and from the community, to simplify the API and make it more intuitive, the `SKContext` concept was dilluted in different entities: `KernelArguments` for function inputs and `FunctionResult` for function outputs.

With the important decision to make `Kernel` a required argument of a function calling, the `SKContext` was removed and the `KernelArguments` and `FunctionResult` were introduced.

`KernelArguments` is a dictionary that holds the input arguments for the function invocation that were previously held in the `SKContext.Variables` property.

`FunctionResult` is the output of the `Kernel.InvokeAsync` method and holds the result of the function invocation that was previously held in the `SKContext.Result` property.

## New Plugin Abstractions

- **KernelPlugin Entity**: Before V1 there was no concept of a plugin centric entity. This changed in V1 and for any function you add to a Kernel you will get a Plugin that it belongs to.

### Plugins Immutability

Plugins are created by default as immutable by our out-of-the-box `DefaultKernelPlugin` implementation, which means that they cannot be modified or changed after creation.

Also attempting to import the plugins that share the same name in the kernel will give you a key violation exception.

The addition of the `KernelPlugin` abstraction allows dynamic implementations that may support mutability and we provided an example on how to implement a mutable plugin in the [Example 69](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example69_MutableKernelPlugin.cs).

### Combining multiple plugins into one

Attempting to create a plugin from directory and adding Method functions afterwards for the same plugin will not work unless you use another approach like creating both plugins separately and then combining them into a single plugin iterating over its functions to aggregate into the final plugin using `kernel.ImportPluginFromFunctions("myAggregatePlugin", myAggregatedFunctions)` extension.

## Usage of Experimental Attribute Feature.

This features was introduced to mark some functionalities in V1 that we can possibly change or completely remove.

For mode details one the list of current released experimental features [check here](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/docs/EXPERIMENTS.md).

## Prompt Configuration Files

Major changes were introduced to the Prompt Configuration files including default and multiple service/model configurations.

Other naming changes to note:

- `completion` was renamed to `execution_settings`
- `input` was renamed to `input_variables`
- `defaultValue` was renamed to `default`
- `parameters` was renamed to `input_variables`
- Each property name in the `execution_settings` once matched to the `service_id` will be used to configure the service/model execution settings. i.e.:
  ```csharp
  // The "service1" execution settings will be used to configure the OpenAIChatCompletion service
  Kernel kernel = Kernel.CreateBuilder()
      .AddOpenAIChatCompletion(serviceId: "service1", modelId: "gpt-4")
  ```

Before

```json
{
  "schema": 1,
  "description": "Given a text input, continue it with additional text.",
  "type": "completion",
  "completion": {
    "max_tokens": 4000,
    "temperature": 0.3,
    "top_p": 0.5,
    "presence_penalty": 0.0,
    "frequency_penalty": 0.0
  },
  "input": {
    "parameters": [
      {
        "name": "input",
        "description": "The text to continue.",
        "defaultValue": ""
      }
    ]
  }
}
```

After

```json
{
  "schema": 1,
  "description": "Given a text input, continue it with additional text.",
  "execution_settings": {
    "default": {
      "max_tokens": 4000,
      "temperature": 0.3,
      "top_p": 0.5,
      "presence_penalty": 0.0,
      "frequency_penalty": 0.0
    },
    "service1": {
      "model_id": "gpt-4",
      "max_tokens": 200,
      "temperature": 0.2,
      "top_p": 0.0,
      "presence_penalty": 0.0,
      "frequency_penalty": 0.0,
      "stop_sequences": ["Human", "AI"]
    },
    "service2": {
      "model_id": "gpt-3.5_turbo",
      "max_tokens": 256,
      "temperature": 0.3,
      "top_p": 0.0,
      "presence_penalty": 0.0,
      "frequency_penalty": 0.0,
      "stop_sequences": ["Human", "AI"]
    }
  },
  "input_variables": [
    {
      "name": "input",
      "description": "The text to continue.",
      "default": ""
    }
  ]
}
```
