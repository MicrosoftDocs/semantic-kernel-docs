---
title: Function Choice Behavior
description: Describes function choice behavior types Semantic Kernel supports.
zone_pivot_groups: programming-languages
author: SergeyMenshykh
ms.topic: conceptual
ms.author: semenshi
ms.service: semantic-kernel
---

::: zone pivot="programming-language-csharp"
# Function Choice Behaviors

Function choice behaviors are bits of configuration that allows a developer to configure:
1. Which functions are advertised to AI models.
2. How the models should choose them for invocation.
3. How Semantic Kernel might invoke those functions.

As of today, the function choice behaviors are represented by three static methods of the `FunctionChoiceBehavior` class:
- **Auto**: Allows the AI model to decide to choose from zero or more of the provided function(s) for invocation.
- **Required**: Forces the AI model to choose provided function(s).
- **None**: Instructs the AI model not to choose any function(s).

> [!WARNING]
> The function-calling model is experimental and subject to change. It is expected to reach general availability (GA) by mid-November 2024, at which point the other function-calling model, represented by the TollCallBehavior class, will be deprecated.
> Please refer to the [migration guide](../../../../support/function-calling-migration-guide.md) to migrate your code to the latest function-calling model.

> [!NOTE]
> The function-calling model is only supported by a few AI connectors so far, see the [Supported AI Connectors](./function-choice-behaviors.md#supported-ai-connectors) section below for more details.

## Function Advertising

Function advertising is the process of providing functions to AI models for further calling and invocation. All three function choice behaviors accept a list of functions to advertise as a `functions` parameter. By default, it is null, which means all functions from plugins registered on the Kernel are provided to the AI model.

```csharp
using Microsoft.SemanticKernel;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();
builder.Plugins.AddFromType<DateTimeUtils>(); 

Kernel kernel = builder.Build();

// All functions from the DateTimeUtils and WeatherForecastUtils plugins will be sent to AI model together with the prompt.
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }; 

await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new(settings));
```

If a list of functions is provided, only those functions are sent to the AI model:
```csharp
using Microsoft.SemanticKernel;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();
builder.Plugins.AddFromType<DateTimeUtils>(); 

Kernel kernel = builder.Build();

KernelFunction getWeatherForCity = kernel.Plugins.GetFunction("WeatherForecastUtils", "GetWeatherForCity");
KernelFunction getCurrentTime = kernel.Plugins.GetFunction("DateTimeUtils", "GetCurrentUtcDateTime");

// Only the specified getWeatherForCity and getCurrentTime functions will be sent to AI model alongside the prompt.
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions: [getWeatherForCity, getCurrentTime]) }; 

await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new(settings));
```

An empty list of functions means no functions are provided to the AI model, which is equivalent to disabling function calling.
```csharp
using Microsoft.SemanticKernel;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();
builder.Plugins.AddFromType<DateTimeUtils>(); 

Kernel kernel = builder.Build();

// Disables function calling. Equivalent to var settings = new() { FunctionChoiceBehavior = null } or var settings = new() { }.
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions: []) }; 

await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new(settings));
```
## Using Auto Function Choice Behavior

The `Auto` function choice behavior instructs the AI model to decide to choose from zero or more of the provided function(s) for invocation.

In this example, all the functions from the `DateTimeUtils` and `WeatherForecastUtils` plugins will be provided to the AI model alongside the prompt. 
The model will first choose `GetCurrentTime` function for invocation to obtain the current date and time, as this information is needed as input for the `GetWeatherForCity` function. 
Next, it will choose `GetWeatherForCity` function for invocation to get the weather forecast for the city of Boston using the obtained date and time. 
With this information, the model will be able to determine the likely color of the sky in Boston.
```csharp
using Microsoft.SemanticKernel;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();
builder.Plugins.AddFromType<DateTimeUtils>(); 

Kernel kernel = builder.Build();

// All functions from the DateTimeUtils and WeatherForecastUtils plugins will be provided to AI model alongside the prompt.
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }; 

await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new(settings));
```

The same example can be easily modeled in a YAML prompt template configuration:
```csharp
using Microsoft.SemanticKernel;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();
builder.Plugins.AddFromType<DateTimeUtils>(); 

Kernel kernel = builder.Build();

string promptTemplateConfig = """
    template_format: semantic-kernel
    template: Given the current time of day and weather, what is the likely color of the sky in Boston?
    execution_settings:
      default:
        function_choice_behavior:
          type: auto
    """;

KernelFunction promptFunction = KernelFunctionYaml.FromPromptYaml(promptTemplateConfig);

Console.WriteLine(await kernel.InvokeAsync(promptFunction));
```

## Using Required Function Choice Behavior

The `Required` behavior forces the model to choose the provided function(s) for for invocation. This is useful for scenarios when the AI model must obtain required information from the specified 
functions rather than from it's own knowledge.

> [!NOTE]
> The behavior advertises functions in the first request to the AI model only and stops sending them in subsequent requests to prevent an infinite loop where the model keeps choosing the same functions for invocation repeatedly.

Here, we specify that the AI model must choose the `GetWeatherForCity` function for invocation to obtain the weather forecast for the city of Boston, rather than guessing it based on its own knowledge. 
The model will first choose the `GetWeatherForCity` function for invocation to retrieve the weather forecast. 
With this information, the model can then determine the likely color of the sky in Boston using the response from the call to `GetWeatherForCity`.
```csharp
using Microsoft.SemanticKernel;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();

Kernel kernel = builder.Build();

KernelFunction getWeatherForCity = kernel.Plugins.GetFunction("WeatherForecastUtils", "GetWeatherForCity");

PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Required(functions: [getWeatherFunction]) };

await kernel.InvokePromptAsync("Given that it is now the 10th of September 2024, 11:29 AM, what is the likely color of the sky in Boston?", new(settings));
```

An identical example in a YAML template configuration:
```csharp
using Microsoft.SemanticKernel;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();

Kernel kernel = builder.Build();

string promptTemplateConfig = """
    template_format: semantic-kernel
    template: Given that it is now the 10th of September 2024, 11:29 AM, what is the likely color of the sky in Boston?
    execution_settings:
      default:
        function_choice_behavior:
          type: auto
          functions:
            - WeatherForecastUtils.GetWeatherForCity
    """;

KernelFunction promptFunction = KernelFunctionYaml.FromPromptYaml(promptTemplateConfig);

Console.WriteLine(await kernel.InvokeAsync(promptFunction));
```

Alternatively, all functions registered in the kernel can be provided to the AI model as required. However, only the ones chosen by the AI model as a result of the first request will be invoked by the Semantic Kernel.
The functions will not be sent to the AI model in subsequent requests to prevent an infinite loop, as mentioned above.
```csharp
using Microsoft.SemanticKernel;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();

Kernel kernel = builder.Build();

PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Required() };

await kernel.InvokePromptAsync("Given that it is now the 10th of September 2024, 11:29 AM, what is the likely color of the sky in Boston?", new(settings));
```

## Using None Function Choice Behavior

The `None` behavior instructs the AI model to use the provided function(s) without choosing any of them for invocation to generate a response. This is useful for dry runs when the caller may want to see which functions the model would choose without actually invoking them.
It is also useful when you want the AI model to extract information from a user ask e.g. in the sample below the AI model correctly worked out that Boston was the city name.

Here, we advertise all functions from the `DateTimeUtils` and `WeatherForecastUtils` plugins to the AI model but instruct it not to choose any of them. 
Instead, the model will provide a response describing which functions it would choose to determine the color of the sky in Boston on a specified date.
```csharp
using Microsoft.SemanticKernel;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();
builder.Plugins.AddFromType<DateTimeUtils>(); 

Kernel kernel = builder.Build();

KernelFunction getWeatherForCity = kernel.Plugins.GetFunction("WeatherForecastUtils", "GetWeatherForCity");

PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.None() };

await kernel.InvokePromptAsync("Specify which provided functions are needed to determine the color of the sky in Boston on a specified date.", new(settings))

// Sample response: To determine the color of the sky in Boston on a specified date, first call the DateTimeUtils-GetCurrentUtcDateTime function to obtain the 
// current date and time in UTC. Next, use the WeatherForecastUtils-GetWeatherForCity function, providing 'Boston' as the city name and the retrieved UTC date and time. 
// These functions do not directly provide the sky's color, but the GetWeatherForCity function offers weather data, which can be used to infer the general sky condition (e.g., clear, cloudy, rainy).
```

A corresponding example in a YAML prompt template configuration:
```csharp
using Microsoft.SemanticKernel;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();
builder.Plugins.AddFromType<DateTimeUtils>(); 

Kernel kernel = builder.Build();

string promptTemplateConfig = """
    template_format: semantic-kernel
    template: Specify which provided functions are needed to determine the color of the sky in Boston on a specified date.
    execution_settings:
      default:
        function_choice_behavior:
          type: none
    """;

KernelFunction promptFunction = KernelFunctionYaml.FromPromptYaml(promptTemplateConfig);

Console.WriteLine(await kernel.InvokeAsync(promptFunction));
```

## Function Invocation

Function invocation is the process whereby Sematic Kernel invokes functions chosen by the AI model. For more details on function invocation see [function invocation article](./function-invocation.md).

## Supported AI Connectors

As of today, the following AI connectors in Semantic Kernel support the function calling model:

| AI Connector           | FunctionChoiceBehavior | ToolCallBehavior |  
|------------------------|------------------------|------------------|  
| Anthropic              | Planned                | ❌               |  
| AzureAIInference       | Coming soon            | ❌               |  
| AzureOpenAI            | ✔️                     | ✔️               |  
| Gemini                 | Planned                | ✔️               |  
| HuggingFace            | Planned                | ❌               |  
| Mistral                | Planned                | ✔️               |  
| Ollama                 | Coming soon            | ❌               |  
| Onnx                   | Coming soon            | ❌               |  
| OpenAI                 | ✔️                     | ✔️               |  
::: zone-end

::: zone pivot="programming-language-python"
## Coming soon
More info coming soon.
::: zone-end
::: zone pivot="programming-language-java"
## Coming soon
More info coming soon.
::: zone-end