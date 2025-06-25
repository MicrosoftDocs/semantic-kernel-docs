---
title: Migrating from Stepwise Planner to Auto Function Calling
description: Describes the steps for SK caller code to migrate from Stepwise Planner to Auto Function Calling.
zone_pivot_groups: programming-languages
author: dmytrostruk
ms.topic: conceptual
ms.author: dmytrostruk
ms.date: 06/10/2025
ms.service: semantic-kernel
---

# Stepwise Planner Migration Guide
This migration guide shows how to migrate from `FunctionCallingStepwisePlanner` to a new recommended approach for planning capability - [Auto Function Calling](../../concepts/ai-services/chat-completion/function-calling/index.md). The new approach produces the results more reliably and uses fewer tokens compared to `FunctionCallingStepwisePlanner`.

## Plan generation

::: zone pivot="programming-language-csharp"

Following code shows how to generate a new plan with Auto Function Calling by using `FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()`. After sending a request to AI model, the plan will be located in `ChatHistory` object where a message with `Assistant` role will contain a list of functions (steps) to call.

Old approach:
```csharp
Kernel kernel = Kernel
    .CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4", Environment.GetEnvironmentVariable("OpenAI__ApiKey"))
    .Build();

FunctionCallingStepwisePlanner planner = new();

FunctionCallingStepwisePlannerResult result = await planner.ExecuteAsync(kernel, "Check current UTC time and return current weather in Boston city.");

ChatHistory generatedPlan = result.ChatHistory;
```

New approach:
```csharp
Kernel kernel = Kernel
    .CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4", Environment.GetEnvironmentVariable("OpenAI__ApiKey"))
    .Build();

IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

ChatHistory chatHistory = [];
chatHistory.AddUserMessage("Check current UTC time and return current weather in Boston city.");

OpenAIPromptExecutionSettings executionSettings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings, kernel);

ChatHistory generatedPlan = chatHistory;
```

## Execution of the new plan
Following code shows how to execute a new plan with Auto Function Calling by using `FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()`. This approach is useful when only result is needed without plan steps. In this case, `Kernel` object can be used to pass a goal to `InvokePromptAsync` method. The result of plan execution will be located in `FunctionResult` object.

Old approach:
```csharp
Kernel kernel = Kernel
    .CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4", Environment.GetEnvironmentVariable("OpenAI__ApiKey"))
    .Build();

FunctionCallingStepwisePlanner planner = new();

FunctionCallingStepwisePlannerResult result = await planner.ExecuteAsync(kernel, "Check current UTC time and return current weather in Boston city.");

string planResult = result.FinalAnswer;
```

New approach:
```csharp
Kernel kernel = Kernel
    .CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4", Environment.GetEnvironmentVariable("OpenAI__ApiKey"))
    .Build();

OpenAIPromptExecutionSettings executionSettings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

FunctionResult result = await kernel.InvokePromptAsync("Check current UTC time and return current weather in Boston city.", new(executionSettings));

string planResult = result.ToString();
```

## Execution of the existing plan
Following code shows how to execute an existing plan with Auto Function Calling by using `FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()`. This approach is useful when `ChatHistory` is already present (e.g. stored in cache) and it should be re-executed again and final result should be provided by AI model.

Old approach:
```csharp
Kernel kernel = Kernel
    .CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4", Environment.GetEnvironmentVariable("OpenAI__ApiKey"))
    .Build();

FunctionCallingStepwisePlanner planner = new();
ChatHistory existingPlan = GetExistingPlan(); // plan can be stored in database  or cache for reusability.

FunctionCallingStepwisePlannerResult result = await planner.ExecuteAsync(kernel, "Check current UTC time and return current weather in Boston city.", existingPlan);

string planResult = result.FinalAnswer;
```

New approach:
```csharp
Kernel kernel = Kernel
    .CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4", Environment.GetEnvironmentVariable("OpenAI__ApiKey"))
    .Build();

IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

ChatHistory existingPlan = GetExistingPlan(); // plan can be stored in database or cache for reusability.

OpenAIPromptExecutionSettings executionSettings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

ChatMessageContent result = await chatCompletionService.GetChatMessageContentAsync(existingPlan, executionSettings, kernel);

string planResult = result.Content;
```

::: zone-end

::: zone pivot="programming-language-python"

The following code shows how to generate a new plan with Auto Function Calling by using `function_choice_behavior = FunctionChoiceBehavior.Auto()`. After sending a request to AI model, the plan will be located in `ChatHistory` object where a message with `Assistant` role will contain a list of functions (steps) to call.

Old approach:
```python
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.planners.function_calling_stepwise_planner import (
    FunctionCallingStepwisePlanner, 
    FunctionCallingStepwisePlannerResult,
)

kernel = Kernel()
kernel.add_service(AzureChatCompletion())

# Add any plugins to the kernel that the planner will leverage
kernel.add_plugins(...)

planner = FunctionCallingStepwisePlanner(service_id="service_id")

result: FunctionCallingStepwisePlannerResult = await planner.invoke(
    kernel=kernel, 
    question="Check current UTC time and return current weather in Boston city.",
)

generated_plan = result.chat_history
```

New approach:

```python
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion, AzureChatPromptExecutionSettings
from semantic_kernel.contents import ChatHistory

chat_completion_service = AzureChatCompletion()

chat_history = ChatHistory()
chat_hitory.add_user_message("Check current UTC time and return current weather in Boston city.")

request_settings = AzureChatPromptExecutionSettings(function_choice_behavior=FunctionChoiceBehavior.Auto())

# Add any plugins to the kernel that the planner will leverage
kernel = Kernel()
kernel.add_plugins(...)

response = await chat_completion_service.get_chat_message_content(
    chat_history=chat_history,
    settings=request_settings,
    kernel=kernel,
)
print(response)

# The generated plan is now contained inside of `chat_history`.
```

## Execution of the new plan
Following code shows how to execute a new plan with Auto Function Calling by using `function_choice_behavior = FunctionChoiceBehavior.Auto()`. This approach is useful when only the result is needed without plan steps. In this case, the `Kernel` object can be used to pass a goal to the `invoke_prompt` method. The result of plan execution will be located in a `FunctionResult` object.

Old approach:
```python
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.planners.function_calling_stepwise_planner import (
    FunctionCallingStepwisePlanner, 
    FunctionCallingStepwisePlannerResult,
)

kernel = Kernel()
kernel.add_service(AzureChatCompletion())

# Add any plugins to the kernel that the planner will leverage
kernel.add_plugins(...)

planner = FunctionCallingStepwisePlanner(service_id="service_id")

result: FunctionCallingStepwisePlannerResult = await planner.invoke(
    kernel=kernel, 
    question="Check current UTC time and return current weather in Boston city.",
)

print(result.final_answer)
```

New approach:

```python
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion, AzureChatPromptExecutionSettings
from semantic_kernel.contents import ChatHistory
from semantic_kernel.functions import KernelArguments

kernel = Kernel()
kernel.add_service(AzureChatCompletion())
# Add any plugins to the kernel that the planner will leverage
kernel.add_plugins(...)

chat_history = ChatHistory()
chat_hitory.add_user_message("Check current UTC time and return current weather in Boston city.")

request_settings = AzureChatPromptExecutionSettings(function_choice_behavior=FunctionChoiceBehavior.Auto())

response = await kernel.invoke_prompt(
    "Check current UTC time and return current weather in Boston city.", 
    arguments=KernelArguments(settings=request_settings),
)
print(response)
```

::: zone-end

::: zone pivot="programming-language-java"

> Planners were not available in SK Java. Please use function calling directly.

::: zone-end

The code snippets above demonstrate how to migrate your code that uses Stepwise Planner to use Auto Function Calling. Learn more about [Function Calling with chat completion](../../concepts/ai-services/chat-completion/function-calling/index.md).

