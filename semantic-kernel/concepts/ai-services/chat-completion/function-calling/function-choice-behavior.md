---
title: Function Choice Behavior
description: Describes function choice behavior types Semantic Kernel supports.
zone_pivot_groups: programming-languages
author: SergeyMenshykh
ms.topic: conceptual
ms.author: SergeyMenshykh
ms.date: 12/09/2024
ms.service: semantic-kernel
---

# Overview
   
The new function-calling model in Semantic Kernel is represented by the `FunctionChoiceBehavior` class. The class provides a set of static methods that return instances of the `FunctionChoiceBehavior` class, each representing a specific function choice behavior. As of today, there are three function choice behaviors available in Semantic Kernel:
- **Auto**: The AI model decides whether to call provided function(s) and, if so, which one to call.
- **Required**: The AI model must call the provided function(s).
- **None**: The AI model must not call any function(s).

Each of the behaviors enable configuration of the following aspects of function calling that a caller might need to change according to the modeled scenario(s):
- **Function advertising**: The process of advertising or sending of all Semantic Kernel functions or specified ones to the AI model.  
- **Function choice behavior**: The process of instructing the AI model on how to choose functions for calling.
- **Function invocation**: Invocation of functions called by the AI model.

> [!WARNING]
> The function-calling model is experimental and subject to change. It is expected to reach general availability (GA) by mid-November 2024. The other function-calling model, based on the TollCallBehavior class, will be deprecated at the same time and is planned to be completely removed by the end of 2024.  

> [!NOTE]
> The function-calling model is a general-purpose model that is not tied to any specific AI model. It can be used with any AI model that supports function calling. At the moment, it's supported by the `AzureOpenAI` and `OpenAI` connectors only, with plans to be supported by other connectors for Olama, Onix, and other function-calling-capable models in the future.

## Function Advertising
::: zone pivot="programming-language-csharp"
All three behaviors accept list of functions of `KernelFunction` type as a parameter. 
By default, it is null, which means all kernel functions are provided to the AI model. 

```csharp
using Microsoft.SemanticKernel;

Kernel kernel = new Kernel();
kernel.ImportPluginFromType<DateTimeUtils>();
kernel.ImportPluginFromType<WeatherForecastUtils>();

// All functions from the DateTimeUtils and WeatherForecastUtils plugins will be sent to AI model together with the prompt.
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }; 

await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new(settings));
```

If a list of functions is provided, only those functions are sent to the AI model:
```csharp
using Microsoft.SemanticKernel;

Kernel kernel = new Kernel();
kernel.ImportPluginFromType<DateTimeUtils>();
kernel.ImportPluginFromType<WeatherForecastUtils>();

KernelFunction getWeatherForCity = kernel.Plugins.GetFunction("WeatherForecastUtils", "GetWeatherForCity");
KernelFunction getCurrentTime = kernel.Plugins.GetFunction("DateTimeUtils", "GetCurrentUtcDateTime");

// Only the specified getWeatherForCity and getCurrentTime functions will be sent to AI model alongside the prompt.
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions: [getWeatherForCity, getCurrentTime]) }; 

await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new(settings));
```

An empty list of functions means no functions are provided to the AI model, which is equivalent to disabling function calling.
```csharp
using Microsoft.SemanticKernel;

Kernel kernel = new Kernel();
kernel.ImportPluginFromType<DateTimeUtils>();
kernel.ImportPluginFromType<WeatherForecastUtils>();

// Disables function calling. Equivalent to var settings = new() { FunctionChoiceBehavior = null } or var settings = new() { }.
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions: []) }; 

await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new(settings));
```
::: zone-end

## Auto Function Choice Behavior
The "Auto" function choice behavior instructs the AI model to decide whether to call provided function(s) and, if so, which one to call.

::: zone pivot="programming-language-csharp"
In this example, all the functions from the DateTimeUtils and WeatherForecastUtils plugins will be provided to the AI model alongside the prompt. 
The model will first call `GetCurrentTime` to obtain the current date and time, as this information is needed as input for the `GetWeatherForCity` function. 
Next, it will call `GetWeatherForCity` to get the weather forecast for the city of Boston using the obtained date and time. 
With this information, the model will be able to determine the likely color of the sky in Boston.
```csharp
using Microsoft.SemanticKernel;

Kernel kernel = new Kernel();
kernel.ImportPluginFromType<DateTimeUtils>();
kernel.ImportPluginFromType<WeatherForecastUtils>();

// All functions from the DateTimeUtils and WeatherForecastUtils plugins will be provided to AI model alongside the prompt.
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }; 

await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new(settings));
```
::: zone-end

## Required Function Choice Behavior
The "Required"" behavior forces the model to call the provided functions. This is useful for scenarios when the AI model must call specific functions to obtain the required information from 
specified functions rather than from it's own knowledge.

> [!NOTE]
The behavior advertise functions in the first request to the AI model only and stops sending them in subsequent requests to prevent an infinite loop where the model keeps calling functions repeatedly.

::: zone pivot="programming-language-csharp"
Here, we specify that the AI model must call the `GetWeatherForCity` function to obtain the weather forecast for the city of Boston, rather than guessing it based on its own knowledge. 
The model will first call the `GetWeatherForCity` function to retrieve the weather forecast. 
With this information, the model can then determine the likely color of the sky in Boston using its own knowledge.
```csharp
using Microsoft.SemanticKernel;

Kernel kernel = new Kernel();
kernel.ImportPluginFromType<WeatherForecastUtils>();

KernelFunction getWeatherForCity = kernel.Plugins.GetFunction("WeatherForecastUtils", "GetWeatherForCity");

PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Required(functions: [getWeatherFunction]) };

await kernel.InvokePromptAsync("Given that it is now the 10th of September 2024, 11:29 AM, what is the likely color of the sky in Boston?", new(settings));
```
Alternatively, all functions registered in the kernel can be provided to the AI model as required:
```csharp
using Microsoft.SemanticKernel;

Kernel kernel = new Kernel();
kernel.ImportPluginFromType<WeatherForecastUtils>();

PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Required() };

await kernel.InvokePromptAsync("Given that it is now the 10th of September 2024, 11:29 AM, what is the likely color of the sky in Boston?", new(settings));
```
::: zone-end

## None Function Choice Behavior
The "None" behavior instructs the AI model to use the provided functions without executing them to generate a response. This is useful for dry runs when the SK caller wants to see which functions the model would choose without actually invoking them.
::: zone pivot="programming-language-csharp"
```csharp
using Microsoft.SemanticKernel;

Kernel kernel = new Kernel();
kernel.ImportPluginFromType<DateTimeUtils>();
kernel.ImportPluginFromType<WeatherForecastUtils>();

KernelFunction getWeatherForCity = kernel.Plugins.GetFunction("WeatherForecastUtils", "GetWeatherForCity");

PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.None() };

await kernel.InvokePromptAsync("Specify which provided functions are needed to determine the color of the sky in Boston on a specified date.", new(settings))

// Sample response: To determine the color of the sky in Boston on a specified date, first call the DateTimeUtils-GetCurrentUtcDateTime function to obtain the 
// current date and time in UTC. Next, use the WeatherForecastUtils-GetWeatherForCity function, providing 'Boston' as the city name and the retrieved UTC date and time. 
// These functions do not directly provide the sky's color, but the GetWeatherForCity function offers weather data, which can be used to infer the general sky condition (e.g., clear, cloudy, rainy).
```
::: zone-end

## Function Invocation
Function invocation is a process of invoking functions by SK chosen or called by AI model. For more details on function invocation see [Function Invocation](./function-invocation.md).

::: zone pivot="programming-language-python"

## Coming soon

More info coming soon.

::: zone-end

::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end