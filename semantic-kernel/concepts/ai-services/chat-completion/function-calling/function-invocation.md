---
title: Function Invocation
description: Describes function invocation types SK supports.
zone_pivot_groups: programming-languages
author: SergeyMenshykh
ms.topic: conceptual
ms.author: semenshi
ms.service: semantic-kernel
---
::: zone pivot="programming-language-csharp"
# Function Invocation Modes
When the AI model receives a prompt containing a list of functions, it may choose one or more of them for invocation to complete the prompt. When a function is chosen by the model, it needs be **invoked** by Semantic Kernel.

The function calling subsystem in Semantic Kernel has two modes of function invocation: **auto** and **manual**. 

Depending on the invocation mode, Semantic Kernel either does end-to-end function invocation or gives the caller control over the function invocation process.

## Auto Function Invocation
Auto function invocation is the default mode of the Semantic Kernel function-calling subsystem. When the AI model chooses one or more functions, Semantic Kernel automatically invokes the chosen functions. 
The results of these function invocations are added to the chat history and sent to the model automatically in subsequent requests. 
The model then reasons about the chat history, chooses additional functions if needed, or generates the final response. 
This approach is fully automated and requires no manual intervention from the caller.

This example demonstrates how to use the auto function invocation in Semantic Kernel. AI model decides which functions to call to complete the prompt and Semantic Kernel does the rest and invokes them automatically.
```csharp
using Microsoft.SemanticKernel;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();
builder.Plugins.AddFromType<DateTimeUtils>(); 

Kernel kernel = builder.Build();

// By default, functions are set to be automatically invoked.  
// If you want to explicitly enable this behavior, you can do so with the following code:  
// PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(autoInvoke: true) };  
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }; 

await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new(settings));
```

Some AI models support parallel function calling, where the model chooses multiple functions for invocation. This can be useful in cases when invoking chosen functions takes a long time. For example, the AI may choose to retrieve the latest news and the current time simultaneously, rather than making a round trip per function.

Semantic Kernel can invoke these functions in two different ways:
- **Sequentially**: The functions are invoked one after another. This is the default behavior.
- **Concurrently**: The functions are invoked at the same time. This can be enabled by setting the `FunctionChoiceBehaviorOptions.AllowConcurrentInvocation` property to `true`, as shown in the example below.
```csharp
using Microsoft.SemanticKernel;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<NewsUtils>();
builder.Plugins.AddFromType<DateTimeUtils>(); 

Kernel kernel = builder.Build();

// Enable concurrent invocation of functions to get the latest news and the current time.
FunctionChoiceBehaviorOptions options = new() { AllowConcurrentInvocation = true };

PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: options) }; 

await kernel.InvokePromptAsync("Good morning! What is the current time and latest news headlines?", new(settings));
```

## Manual Function Invocation
In cases when the caller wants to have more control over the function invocation process, manual function invocation can be used. 

When manual function invocation is enabled, Semantic Kernel does not automatically invoke the functions chosen by the AI model. 
Instead, it returns a list of chosen functions to the caller, who can then decide which functions to invoke, invoke them sequentially or in parallel, handle exceptions, and so on. 
The function invocation results need to be added to the chat history and returned to the model, which reasons about them and decides whether to choose additional functions or generate the final response.

The example below demonstrates how to use manual function invocation.
```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();
builder.Plugins.AddFromType<DateTimeUtils>(); 

Kernel kernel = builder.Build();

IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Manual function invocation needs to be enabled explicitly by setting autoInvoke to false.
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

    // Adding AI model response containing chosen functions to chat history as it's required by the models to preserve the context.
    chatHistory.Add(result); 

    // Check if the AI model has chosen any function for invocation.
    IEnumerable<FunctionCallContent> functionCalls = FunctionCallContent.GetFunctionCalls(result);
    if (!functionCalls.Any())
    {
        break;
    }

    // Sequentially iterating over each chosen function, invoke it, and add the result to the chat history.
    foreach (FunctionCallContent functionCall in functionCalls)
    {
        try
        {
            // Invoking the function
            FunctionResultContent resultContent = await functionCall.InvokeAsync(kernel);

            // Adding the function result to the chat history
            chatHistory.Add(resultContent.ToChatMessage());
        }
        catch (Exception ex)
        {
            // Adding function exception to the chat history.
            chatHistory.Add(new FunctionResultContent(functionCall, ex).ToChatMessage());
            // or
            //chatHistory.Add(new FunctionResultContent(functionCall, "Error details that the AI model can reason about.").ToChatMessage());
        }
    }
}

```
> [!NOTE]
> The FunctionCallContent and FunctionResultContent classes are used to represent AI model function calls and Semantic Kernel function invocation results, respectively. 
> They contain information about chosen function, such as the function ID, name, and arguments, and function invocation results, such as function call ID and result.

The following example demonstrates how to use manual function invocation with the streaming chat completion API. Note the usage of the `FunctionCallContentBuilder` class to build function calls from the streaming content. 
Due to the streaming nature of the API, function calls are also streamed. This means that the caller must build the function calls from the streaming content before invoking them.  

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

IKernelBuilder builder = Kernel.CreateBuilder(); 
builder.AddOpenAIChatCompletion("<model-id>", "<api-key>");
builder.Plugins.AddFromType<WeatherForecastUtils>();
builder.Plugins.AddFromType<DateTimeUtils>(); 

Kernel kernel = builder.Build();

IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Manual function invocation needs to be enabled explicitly by setting autoInvoke to false.
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = Microsoft.SemanticKernel.FunctionChoiceBehavior.Auto(autoInvoke: false) };

ChatHistory chatHistory = [];
chatHistory.AddUserMessage("Given the current time of day and weather, what is the likely color of the sky in Boston?");

while (true)
{
    AuthorRole? authorRole = null;
    FunctionCallContentBuilder fccBuilder = new ();

    // Start or continue streaming chat based on the chat history
    await foreach (StreamingChatMessageContent streamingContent in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, settings, kernel))
    {
        // Check if the AI model has generated a response.
        if (streamingContent.Content is not null)
        {
            Console.Write(streamingContent.Content);
            // Sample streamed output: "The color of the sky in Boston is likely to be gray due to the rainy weather."
        }
        authorRole ??= streamingContent.Role;

        // Collect function calls details from the streaming content
        fccBuilder.Append(streamingContent);
    }

    // Build the function calls from the streaming content and quit the chat loop if no function calls are found
    IReadOnlyList<FunctionCallContent> functionCalls = fccBuilder.Build();
    if (!functionCalls.Any())
    {
        break;
    }

    // Creating and adding chat message content to preserve the original function calls in the chat history.
    // The function calls are added to the chat message a few lines below.
    ChatMessageContent fcContent = new ChatMessageContent(role: authorRole ?? default, content: null);
    chatHistory.Add(fcContent);

    // Iterating over the requested function calls and invoking them.
    // The code can easily be modified to invoke functions concurrently if needed.
    foreach (FunctionCallContent functionCall in functionCalls)
    {
        // Adding the original function call to the chat message content
        fcContent.Items.Add(functionCall);

        // Invoking the function
        FunctionResultContent functionResult = await functionCall.InvokeAsync(kernel);

        // Adding the function result to the chat history
        chatHistory.Add(functionResult.ToChatMessage());
    }
}
```

::: zone-end
::: zone pivot="programming-language-python"
## Coming soon
More info coming soon.
::: zone-end
::: zone pivot="programming-language-java"
## Coming soon
More info coming soon.
::: zone-end