---
title: Function Invocation
description: Describes function invocation types SK supports.
zone_pivot_groups: programming-languages
author: SergeyMenshykh
ms.topic: conceptual
ms.author: SergeyMenshykh
ms.date: 12/09/2024
ms.service: semantic-kernel
---
::: zone pivot="programming-language-csharp"
# Overview
After AI model receives a prompt containing a list of functions, it may decide to call one or more of them to complete the prompt. When a function is called by the model, it needs be **invoked** by Semantic Kernel.

Function calling model in Semantic Kernel has two types of function invocation: **auto** and **manual**. 

Depending on the invocation type Semantic Kernel either do end-to-end function invocation or give the caller control over the function invocation process.

## Auto Function Invocation
The auto function invocation is the default behavior of the Semantic Kernel function-calling model. When the AI model decides to call one or more functions, Semantic Kernel automatically invokes the chosen functions. 
The results of these function invocations are added to the chat history and sent to the model automatically in subsequent requests. 
The model then reasons about the chat history, calls functions again if needed, or generates the final response. 
This approach is fully automated and requires no manual intervention from the caller.

This example demonstrates how to use the auto function invocation in Semantic Kernel. AI model decides which functions to call to complete the prompt and Semantic Kernel does the rest and invokes them automatically.
```csharp
using Microsoft.SemanticKernel;

Kernel kernel = new Kernel();
kernel.ImportPluginFromType<DateTimeUtils>();
kernel.ImportPluginFromType<WeatherForecastUtils>();

// By default, functions are set to be automatically invoked.  
// If you want to explicitly enable this behavior, you can do so with the following code:  
// PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(autoInvoke: true) };  
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }; 

await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new(settings));
```

## Manual Function Invocation
In cases when the caller wants to have more control over the function invocation process, manual function invocation can be used. 

When manual function invocation is enabled, Semantic Kernel does not automatically invoke the functions chosen by the AI model. 
Instead, it returns a list of functions to the caller, who can then decide which functions to invoke, invoke them sequentially or in parallel, handle exceptions, and so on. 
The function invocation results need to be added to the chat history and returned to the model, which reasons about them and decides whether to call more functions or generate the final response.
The caller then adds the function results or exceptions to the chat history and returns it to the model, which reasons about it.

In the example below, 
```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

Kernel kernel = new Kernel();
kernel.ImportPluginFromType<DateTimeUtils>();
kernel.ImportPluginFromType<WeatherForecastUtils>();

IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Manual funciton invocation needs to be enabled explicitly by setting autoInvoke to false.
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = Microsoft.SemanticKernel.FunctionChoiceBehavior.Auto(autoInvoke: false) };

ChatHistory chatHistory = [];
chatHistory.AddUserMessage("Given the current time of day and weather, what is the likely color of the sky in Boston?");

while (true)
{
    ChatMessageContent result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);
    
    // Check if the AI model has generated a response.
    if (result.Content is not null)
    {
        Console.Write(result.Content);
        // Sample output: "Considering the current weather conditions in Boston with a tornado watch in effect resulting in potential severe thunderstorms,
        // the sky color is likely unusual such as green, yellow, or dark gray. Please stay safe and follow instructions from local authorities."
        break;
    }

    // Adding AI model response containing function calls to chat history as it's required by the models to preserve the context.
    chatHistory.Add(result); 

    // Check if the AI model has called any functions.
    IEnumerable<FunctionCallContent> functionCalls = FunctionCallContent.GetFunctionCalls(result);
    if (!functionCalls.Any())
    {
        break;
    }

    // Sequentially iterating over each function call, invoke it, and add the result to the chat history.
    foreach (FunctionCallContent functionCall in functionCalls)
    {
        FunctionResultContent resultContent = await functionCall.InvokeAsync(kernel);

        chatHistory.Add(resultContent.ToChatMessage());
    }
}

```
> [!NOTE]
> The FunctionCallContent and FunctionResultContent classes are used to represent AI model function calls and Semantic Kernel function invocation results, respectively. 
> They contain information about function calls, such as the function ID, name, and arguments, and function invocation results, such as function call ID and result.

::: zone-end

::: zone pivot="programming-language-python"
## Coming soon
More info coming soon.
::: zone-end
::: zone pivot="programming-language-java"
## Coming soon
More info coming soon.
::: zone-end