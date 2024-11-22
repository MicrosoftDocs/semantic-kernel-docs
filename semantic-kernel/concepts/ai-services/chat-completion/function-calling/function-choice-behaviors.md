---
title: Function Choice Behavior
description: Describes function choice behavior types Semantic Kernel supports.
zone_pivot_groups: programming-languages
author: SergeyMenshykh
ms.topic: conceptual
ms.author: semenshi
ms.service: semantic-kernel
---


# Function Choice Behaviors

Function choice behaviors are bits of configuration that allows a developer to configure:

1. Which functions are advertised to AI models.
2. How the models should choose them for invocation.
3. How Semantic Kernel might invoke those functions.

::: zone pivot="programming-language-csharp"

As of today, the function choice behaviors are represented by three static methods of the `FunctionChoiceBehavior` class:

- **Auto**: Allows the AI model to choose from zero or more function(s) from the provided function(s) for invocation.
- **Required**: Forces the AI model to choose one or more function(s) from the provided function(s) for invocation.
- **None**: Instructs the AI model not to choose any function(s).

::: zone-end

::: zone pivot="programming-language-python"

As of today, the function choice behaviors are represented by three class methods of the `FunctionChoiceBehavior` class:

- **Auto**: Allows the AI model to choose from zero or more function(s) from the provided function(s) for invocation.
- **Required**: Forces the AI model to choose one or more function(s) from the provided function(s) for invocation.
- **NoneInvoke**: Instructs the AI model not to choose any function(s).

> [!NOTE]
> You may be more familiar with the `None` behavior from other literatures. We use `NoneInvoke` to avoid confusion with the Python `None` keyword.

::: zone-end

::: zone pivot="programming-language-java"

More info coming soon.

::: zone-end

> [!NOTE]
> If your code uses the function-calling capabilities represented by the ToolCallBehavior class, please refer to the [migration guide](../../../../support/migration/function-calling-migration-guide.md) to update the code to the latest function-calling model.

> [!NOTE]
> The function-calling capabilities is only supported by a few AI connectors so far, see the [Supported AI Connectors](./function-choice-behaviors.md#supported-ai-connectors) section below for more details.

## Function Advertising

::: zone pivot="programming-language-csharp"

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

::: zone-end

::: zone pivot="programming-language-python"

Function advertising is the process of providing functions to AI models for further calling and invocation. By default, all functions from plugins registered on the Kernel are provided to the AI model unless **filters** are specified. **Filters** is a dictionary with the following keys: `excluded_plugins`, `included_plugins`, `excluded_functions`, `included_functions`. They allow you to specify which functions should be advertised to the AI model.

> [!IMPORTANT]
> It is not allowed to specify both `excluded_plugins` and `included_plugins` or `excluded_functions` and `included_functions` at the same time.

```python
from semantic_kernel.connectors.ai.function_choice_behavior import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.connectors.ai.prompt_execution_settings import PromptExecutionSettings
from semantic_kernel.functions.kernel_arguments import KernelArguments
from semantic_kernel.kernel import Kernel

kernel = Kernel()
kernel.add_service(OpenAIChatCompletion())

# Assuming that WeatherPlugin, DateTimePlugin, and LocationPlugin are already implemented
kernel.add_plugin(WeatherPlugin(), "WeatherPlugin")
kernel.add_plugin(DateTimePlugin(), "DateTimePlugin")
kernel.add_plugin(LocationPlugin(), "LocationPlugin")

query = "What is the weather in my current location today?"
arguments = KernelArguments(
    settings=PromptExecutionSettings(
        # Advertise all functions from the WeatherPlugin, DateTimePlugin, and LocationPlugin plugins to the AI model.
        function_choice_behavior=FunctionChoiceBehavior.Auto(),
    )
)

response = await kernel.invoke_prompt(query, arguments=arguments)
```

If a filter is provided, only those pass the filter are sent to the AI model:

```python
from semantic_kernel.connectors.ai.function_choice_behavior import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.connectors.ai.prompt_execution_settings import PromptExecutionSettings
from semantic_kernel.functions.kernel_arguments import KernelArguments
from semantic_kernel.kernel import Kernel

kernel = Kernel()
kernel.add_service(OpenAIChatCompletion())

# Assuming that WeatherPlugin, DateTimePlugin, and LocationPlugin are already implemented
kernel.add_plugin(WeatherPlugin(), "WeatherPlugin")
kernel.add_plugin(DateTimePlugin(), "DateTimePlugin")
kernel.add_plugin(LocationPlugin(), "LocationPlugin")

query = "What is the weather in Seattle today?"
arguments = KernelArguments(
    settings=PromptExecutionSettings(
        # Advertise all functions from the WeatherPlugin and DateTimePlugin plugins to the AI model.
        function_choice_behavior=FunctionChoiceBehavior.Auto(filters={"included_plugins": ["WeatherPlugin", "DateTimePlugin"]}),
    )
)

response = await kernel.invoke_prompt(query, arguments=arguments)
```

> [!IMPORTANT]
> Providing an empty list to `included_plugins` or `included_functions` does not take any effect. If you want to disable function calling, you should set `function_choice_behavior` to `NoneInvoke`.

::: zone-end

::: zone pivot="programming-language-java"

More info coming soon.

::: zone-end

## Using Auto Function Choice Behavior

The `Auto` function choice behavior instructs the AI model to choose from zero or more function(s) from the provided function(s) for invocation.

::: zone pivot="programming-language-csharp"

In this example, all functions from the `DateTimeUtils` and `WeatherForecastUtils` plugins will be provided to the AI model alongside the prompt.
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

::: zone-end

::: zone pivot="programming-language-python"

In this example, all functions from the `WeatherPlugin` and `DateTimePlugin` plugins will be provided to the AI model alongside the prompt. The model will first choose the `GetCurrentUtcDateTime` function from the `DateTimePlugin` plugin for invocation to obtain the current date and time, as this information is needed as input for the `GetWeatherForCity` function from the `WeatherPlugin` plugin. Next, it will choose the `GetWeatherForCity` function for invocation to get the weather forecast for the city of Seattle using the obtained date and time. With this information, the model will be able to answer the user query in natural language.

```python
from semantic_kernel.connectors.ai.function_choice_behavior import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.connectors.ai.prompt_execution_settings import PromptExecutionSettings
from semantic_kernel.functions.kernel_arguments import KernelArguments
from semantic_kernel.kernel import Kernel

kernel = Kernel()
kernel.add_service(OpenAIChatCompletion())

# Assuming that WeatherPlugin and DateTimePlugin are already implemented
kernel.add_plugin(WeatherPlugin(), "WeatherPlugin")
kernel.add_plugin(DateTimePlugin(), "DateTimePlugin")

query = "What is the weather in Seattle today?"
arguments = KernelArguments(
    settings=PromptExecutionSettings(
        # Advertise all functions from the WeatherPlugin and DateTimePlugin plugins to the AI model.
        function_choice_behavior=FunctionChoiceBehavior.Auto(),
    )
)

response = await kernel.invoke_prompt(query, arguments=arguments)
```

The same example can be easily modeled in a YAML prompt template configuration:

```python
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.functions.kernel_function_from_prompt import KernelFunctionFromPrompt
from semantic_kernel.kernel import Kernel

kernel = Kernel()
kernel.add_service(OpenAIChatCompletion())

# Assuming that WeatherPlugin and DateTimePlugin are already implemented
kernel.add_plugin(WeatherPlugin(), "WeatherPlugin")
kernel.add_plugin(DateTimePlugin(), "DateTimePlugin")

prompt_template_config = """
    name: Weather
    template_format: semantic-kernel
    template: What is the weather in Seattle today?
    execution_settings:
      default:
        function_choice_behavior:
          type: auto
"""
prompt_function = KernelFunctionFromPrompt.from_yaml(prompt_template_config)

response = await kernel.invoke(prompt_function)
```

::: zone-end

::: zone pivot="programming-language-java"

More info coming soon.

::: zone-end

## Using Required Function Choice Behavior

The `Required` behavior forces the model to choose one or more function(s) from the provided function(s) for invocation. This is useful for scenarios when the AI model must obtain required information from the specified
functions rather than from it's own knowledge.

::: zone pivot="programming-language-csharp"

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

::: zone-end

::: zone pivot="programming-language-python"

Here, we provide only one function, `get_weather_for_city`, to the AI model and force it to choose this function for invocation to obtain the weather forecast.

```python
from semantic_kernel.connectors.ai.function_choice_behavior import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.connectors.ai.prompt_execution_settings import PromptExecutionSettings
from semantic_kernel.functions.kernel_arguments import KernelArguments
from semantic_kernel.kernel import Kernel

kernel = Kernel()
kernel.add_service(OpenAIChatCompletion())

# Assuming that WeatherPlugin is already implemented with a
# get_weather_for_city function
kernel.add_plugin(WeatherPlugin(), "WeatherPlugin")

query = "What is the weather in Seattle on September 10, 2024, at 11:29 AM?"
arguments = KernelArguments(
    settings=PromptExecutionSettings(
        # Force the AI model to choose the get_weather_for_city function for invocation.
        function_choice_behavior=FunctionChoiceBehavior.Required(filters={"included_functions": ["get_weather_for_city"]}),
    )
)

response = await kernel.invoke_prompt(query, arguments=arguments)
```

An identical example in a YAML template configuration:

```python
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.functions.kernel_function_from_prompt import KernelFunctionFromPrompt
from semantic_kernel.kernel import Kernel

kernel = Kernel()
kernel.add_service(OpenAIChatCompletion())

# Assuming that WeatherPlugin is already implemented with a
# get_weather_for_city function
kernel.add_plugin(WeatherPlugin(), "WeatherPlugin")

prompt_template_config = """
    name: Weather
    template_format: semantic-kernel
    template: What is the weather in Seattle on September 10, 2024, at 11:29 AM?
    execution_settings:
      default:
        function_choice_behavior:
          type: auto
          filters:
            included_functions:
              - get_weather_for_city
"""
prompt_function = KernelFunctionFromPrompt.from_yaml(prompt_template_config)

response = await kernel.invoke(prompt_function)
```

::: zone-end

::: zone pivot="programming-language-java"

More info coming soon.

::: zone-end

## Using None Function Choice Behavior

::: zone pivot="programming-language-csharp"

The `None` behavior instructs the AI model to use the provided function(s) without choosing any of them for invocation and instead generate a message response. This is useful for dry runs when the caller may want to see which functions the model would choose without actually invoking them. For instance in the sample below the AI model correctly lists the functions it would choose to determine the color of the sky in Boston.

```csharp

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

::: zone-end

::: zone pivot="programming-language-python"

The `NoneInvoke` behavior instructs the AI model to use the provided function(s) without choosing any of them for invocation and instead generate a message response. This is useful for dry runs when the caller may want to see which functions the model would choose without actually invoking them. For instance in the sample below the AI model correctly lists the functions it would choose to determine the color of the sky in Boston.

```python
from semantic_kernel.connectors.ai.function_choice_behavior import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.connectors.ai.prompt_execution_settings import PromptExecutionSettings
from semantic_kernel.functions.kernel_arguments import KernelArguments
from semantic_kernel.kernel import Kernel

kernel = Kernel()
kernel.add_service(OpenAIChatCompletion())

# Assuming that WeatherPlugin and DateTimePlugin are already implemented
kernel.add_plugin(WeatherPlugin(), "WeatherPlugin")
kernel.add_plugin(DateTimePlugin(), "DateTimePlugin")

query = "Specify which provided functions are needed to determine the color of the sky in Boston on the current date."
arguments = KernelArguments(
    settings=PromptExecutionSettings(
        # Force the AI model to choose the get_weather_for_city function for invocation.
        function_choice_behavior=FunctionChoiceBehavior.NoneInvoke(),
    )
)

response = await kernel.invoke_prompt(query, arguments=arguments)
# To determine the color of the sky in Boston on the current date, you would need the following functions:
# 1. **functions.DateTimePlugin-get_current_date**: This function is needed to get the current date.
# 2. **functions.WeatherPlugin-get_weather_for_city**: After obtaining the current date,
#    this function will allow you to get the weather for Boston, which will indicate the sky conditions
#    such as clear, cloudy, etc., helping you infer the color of the sky.
```

An identical example in a YAML template configuration:

```python
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.functions.kernel_function_from_prompt import KernelFunctionFromPrompt
from semantic_kernel.kernel import Kernel

kernel = Kernel()
kernel.add_service(OpenAIChatCompletion())

# Assuming that WeatherPlugin and DateTimePlugin are already implemented
kernel.add_plugin(WeatherPlugin(), "WeatherPlugin")
kernel.add_plugin(DateTimePlugin(), "DateTimePlugin")

prompt_template_config = """
    name: BostonSkyColor
    template_format: semantic-kernel
    template: Specify which provided functions are needed to determine the color of the sky in Boston on the current date.
    execution_settings:
      default:
        function_choice_behavior:
          type: none
"""
prompt_function = KernelFunctionFromPrompt.from_yaml(prompt_template_config)

response = await kernel.invoke(prompt_function)
# To determine the color of the sky in Boston on the current date, you would need the following functions:
# 1. **functions.DateTimePlugin-get_current_date**: This function is needed to get the current date.
# 2. **functions.WeatherPlugin-get_weather_for_city**: After obtaining the current date,
#    this function will allow you to get the weather for Boston, which will indicate the sky conditions
#    such as clear, cloudy, etc., helping you infer the color of the sky.
```

::: zone-end

::: zone pivot="programming-language-java"

More info coming soon.

::: zone-end

::: zone pivot="programming-language-csharp"

## Function Choice Behavior Options

Certain aspects of the function choice behaviors can be configured through options that each function choice behavior class accepts via the `options` constructor parameter of the `FunctionChoiceBehaviorOptions` type. The following options are available:

- **AllowConcurrentInvocation**: This option enables the concurrent invocation of functions by the Semantic Kernel. By default, it is set to false,
    meaning that functions are invoked sequentially. Concurrent invocation is only possible if the AI model can choose multiple functions for invocation in a single request;
    otherwise, there is no distinction between sequential and concurrent invocation
- **AllowParallelCalls**: This option allows the AI model to choose multiple functions in one request. Some AI models may not support this feature; in such cases, the option will have no effect.
    By default, this option is set to null, indicating that the AI model's default behavior will be used.
  
      The following table summarizes the effects of various combinations of the AllowParallelCalls and AllowConcurrentInvocation options:

      | AllowParallelCalls  | AllowConcurrentInvocation | # of functions chosen per AI roundtrip  | Concurrent Invocation by SK |
      |---------------------|---------------------------|-----------------------------------------|-----------------------|
      | false               | false                     | one                                     | false                 |
      | false               | true                      | one                                     | false*                |
      | true                | false                     | multiple                                | false                 |
      | true                | true                      | multiple                                | true                  |

      `*` There's only one function to invoke

::: zone-end

## Function Invocation

Function invocation is the process whereby Sematic Kernel invokes functions chosen by the AI model. For more details on function invocation see [function invocation article](./function-invocation.md).

## Supported AI Connectors

::: zone pivot="programming-language-csharp"

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

As of today, the following AI connectors in Semantic Kernel support the function calling model:

| AI Connector           | FunctionChoiceBehavior | ToolCallBehavior |
|------------------------|------------------------|------------------|
| Anthropic              | ✔️                    | ❌               |
| AzureAIInference       | ✔️                    | ❌               |
| Bedrock                | ✔️                    | ❌               |
| Google AI              | ✔️                    | ❌               |
| Vertex AI              | ✔️                    | ❌               |
| HuggingFace            | Planned                | ❌               |
| Mistral AI             | ✔️                    | ❌               |
| Ollama                 | ✔️                    | ❌               |
| Onnx                   | ❌                    | ❌               |
| OpenAI                 | ✔️                    | ✔️               |
| Azure OpenAI           | ✔️                    | ✔️               |

> [!WARNING]
> Not all models support function calling while some models only supports function calling in non-streaming mode. Please understand the limitations of the model you are using before attempting to use function calling.

::: zone-end

::: zone pivot="programming-language-java"

More info coming soon.

::: zone-end
