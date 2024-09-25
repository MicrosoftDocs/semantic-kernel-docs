---
title: Migrating to the new Function Calling capabilities
description: Describes the steps for SK caller code to migrate from the current function calling capabilities, represented by the `ToolCallBehavior` class, to the new one represented by the `FunctionChoiceBehavior` class.
zone_pivot_groups: programming-languages
author: SergeyMenshykh
ms.topic: conceptual
ms.author: semenshi
ms.service: semantic-kernel
---
::: zone pivot="programming-language-csharp"
# Function Calling Migration Guide
Semantic Kernel is gradually transitioning from the current function calling capabilities, represented by the `ToolCallBehavior` class, to the new enhanced capabilities, represented by the `FunctionChoiceBehavior` class.
The new capability is service-agnostic and is not tied to any specific AI service, unlike the current model. Therefore, it resides in Semantic Kernel abstractions and will be used by all AI connectors working with function-calling capable AI models. 


This guide is intended to help you to migrate your code to the new function calling capabilities.

## Migrate ToolCallBehavior.AutoInvokeKernelFunctions behavior
The `ToolCallBehavior.AutoInvokeKernelFunctions` behavior is equivalent to the `FunctionChoiceBehavior.Auto` behavior in the new model. 
```csharp
// Before
var executionSettings = new OpenAIPromptExecutionSettings { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

// After
var executionSettings = new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
```

## Migrate ToolCallBehavior.EnableKernelFunctions behavior
The `ToolCallBehavior.EnableKernelFunctions` behavior is equivalent to the `FunctionChoiceBehavior.Auto` behavior with disabled auto invocation. 
```csharp
// Before
var executionSettings = new OpenAIPromptExecutionSettings { ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions };

// After
var executionSettings = new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(autoInvoke: false) };
```

## Migrate ToolCallBehavior.EnableFunctions behavior
The `ToolCallBehavior.EnableFunctions` behavior is equivalent to the `FunctionChoiceBehavior.Auto` behavior that configured with list of functions with disabled auto invocation. 
```csharp
var function = kernel.CreateFunctionFromMethod(() => DayOfWeek.Friday, "GetDayOfWeek", "Returns the current day of the week.");

// Before
var executionSettings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.EnableFunctions(functions: [function.Metadata.ToOpenAIFunction()]) };

// After
var executionSettings = new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions: [function], autoInvoke: false) };
```

## Migrate ToolCallBehavior.RequireFunction behavior
The `ToolCallBehavior.RequireFunction` behavior is equivalent to the `FunctionChoiceBehavior.Required` behavior that configured with list of functions with disabled auto invocation.
```csharp
var function = kernel.CreateFunctionFromMethod(() => DayOfWeek.Friday, "GetDayOfWeek", "Returns the current day of the week.");

// Before
var executionSettings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.RequireFunction(functions: [function.Metadata.ToOpenAIFunction()]) };

// After
var executionSettings = new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Required(functions: [function], autoInvoke: false) };
```

## Replace the usage of connector-specific function call classes
Function calling functionality in Semantic Kernel allows developers to access a list of functions chosen by the AI model in two ways:  
- Using connector-specific function call classes like `ChatToolCall` or `ChatCompletionsFunctionToolCall`, available via the `ToolCalls` property of the OpenAI-specific `OpenAIChatMessageContent` item in chat history.  
- Using connector-agnostic function call classes like `FunctionCallContent`, available via the `Items` property of the connector-agnostic `ChatMessageContent` item in chat history.

Both ways are supported at the moment by the current and new models. However, we strongly recommend using the connector-agnostic approach to access function calls, as it is more flexible and allows your code to work with any AI connector that supports the new function-calling model. 
Moreover, considering that the current model will be deprecated soon, now is a good time to migrate your code to the new model to avoid breaking changes in the future.

So, if you use [Manual Function Invocation](../concepts/ai-services/chat-completion/function-calling/function-invocation.md#manual-function-invocation) with the connector-specific function call classes like in this code snippet:
```csharp
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;

var chatHistory = new ChatHistory();

var settings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions };

var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);

// Current way of accessing function calls using connector specific classes.
var toolCalls = ((OpenAIChatMessageContent)result).ToolCalls.OfType<ChatToolCall>().ToList();

while (toolCalls.Count > 0)
{
    // Adding function call from AI model to chat history
    chatHistory.Add(result);

    // Iterating over the requested function calls and invoking them
    foreach (var toolCall in toolCalls)
    {
        string content = kernel.Plugins.TryGetFunctionAndArguments(toolCall, out KernelFunction? function, out KernelArguments? arguments) ?
            JsonSerializer.Serialize((await function.InvokeAsync(kernel, arguments)).GetValue<object>()) :
            "Unable to find function. Please try again!";

        // Adding the result of the function call to the chat history
        chatHistory.Add(new ChatMessageContent(
            AuthorRole.Tool,
            content,
            metadata: new Dictionary<string, object?>(1) { { OpenAIChatMessageContent.ToolIdProperty, toolCall.Id } }));
    }

    // Sending the functions invocation results back to the AI model to get the final response
    result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);
    toolCalls = ((OpenAIChatMessageContent)result).ToolCalls.OfType<ChatToolCall>().ToList();
}
```

You can refactor it to use the connector-agnostic classes:
```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var chatHistory = new ChatHistory();

var settings = new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(autoInvoke: false) };

var messageContent = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);

// New way of accessing function calls using connector agnostic function calling model classes.
var functionCalls = FunctionCallContent.GetFunctionCalls(messageContent).ToArray();

while (functionCalls.Length != 0)
{
    // Adding function call from AI model to chat history
    chatHistory.Add(messageContent);

    // Iterating over the requested function calls and invoking them
    foreach (var functionCall in functionCalls)
    {
        var result = await functionCall.InvokeAsync(kernel);

        chatHistory.Add(result.ToChatMessage());
    }

    // Sending the functions invocation results to the AI model to get the final response
    messageContent = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);
    functionCalls = FunctionCallContent.GetFunctionCalls(messageContent).ToArray();
}
```

The code snippets above demonstrate how to migrate your code that uses the OpenAI AI connector.
A similar migration process can be applied to the Gemini and Mistral AI connectors when they are updated to support the new function calling model.

## Next steps
Now after you have migrated your code to the new function calling model, you can proceed to learn how to configure various aspects of the model that might better correspond to your specific scenarios by referring to the [function choice behaviors article](../concepts/ai-services/chat-completion/function-calling/function-choice-behaviors.md)



::: zone-end
::: zone pivot="programming-language-python"
## Coming soon
More info coming soon.
::: zone-end
::: zone pivot="programming-language-java"
## Coming soon
More info coming soon.
::: zone-end
