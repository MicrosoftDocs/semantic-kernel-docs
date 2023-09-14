---
title: Introduction to Context
description: Run a simple a chat application to learn about context using the Semantic Kernel.
author: momuno
ms.topic: sample
ms.author: momuno
ms.date: 09/01/2023
ms.service: semantic-kernel
---

# Introduction to Context

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

This article will introduce you to context and its importance in LLM prompts. You will run a simple chat application and learn how to implement context with the Semantic Kernel SDK. 

## What is context?

**Context** is information that helps clarify meaning, oftentimes relating to the surrounding circumstances.

In a conversation between two people, **context** could be what was previously said or information about where the converstion is taking place. The folks conversing will likely store this knowledge in their own short-term memory, and it will help them understand the dialogue as it progresses.

An LLM also needs **context** to more clearly understand what you are telling it. Unlike humans, however, current LLMs don't immediately store this information. They are *stateless*. This means an LLM will not recall information you provide it from one exchange to the next, *unless* you provide it *again*. Therefore, a best practice is to include context in each prompt.

To demonstrate, let's look at a simple chat application.

## Context in a chat sample application
Clone the GitHub sample code below in your preferred language. Follow the instructions in the sample README to run the chat console app.

When the default chat application is run, the conversation history is included in each prompt. This is the added context. You can also run the application without this context. Try it and see what happens!

| Language  | Sample Chat Application |
| --------- | ----------------------- |
| C#        | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/Chat101) |
| Python    | Coming in the future |

## Context with the Semantic Kernel SDK

The Semantic Kernel (SK) chat sample app above uses *context variables* and *semantic functions* to include context in each prompt. This section will explain the code so you can implement it in your own app. Ultimately, the SK function you will call to send off your completed prompt (with context) is: 

# [C#](#tab/Csharp)

IKernel.RunAsync Method, specifically [RunAsync(ISKFunction, ContextVariables, CancellationToken)](/dotnet/api/microsoft.semantickernel.ikernel.runasync?view=semantic-kernel-dotnet#microsoft-semantickernel-ikernel-runasync):

```csharp
public Task<SKContext> RunAsync(ISKFunction skFunction, ContextVariables? variables = null, CancellationToken cancellationToken = default)
```

In the sample, this looks like:

```csharp
var chatCompletion = await kernel.RunAsync(chatFunction, chatFunctionVariables);
```

---

To begin, let's build the prompt.

In a chat, a prompt to the LLM will ideally contain the conversation history (the added context) as well as the user's new input. These two parts should be mutable so we will define them as *context variables*. A *context variable* is a key-value pair whose mapping is stored in a Dictionary called `ContextVariables`. In the sample, the `ContextVariables` Dictionary is initialized and called `chatFunctionVariables`, the keys are `"history"` and `"userInput"`, and their values are initialized as empty strings.

# [C#](#tab/Csharp)

```csharp
// Initialize the context variables (semantic function input).
var chatFunctionVariables = new ContextVariables
{
    [ContextVariableKeyHistory] = string.Empty,
    [ContextVariableKeyUserInput] = string.Empty,
};
```

After dereferencing the `const string`s, this becomes:
        
```csharp
// Initialize the context variables (semantic function input).
var chatFunctionVariables = new ContextVariables
{
    ["history"] = string.Empty,
    ["userInput"] = string.Empty,
};
```

---

When we construct the prompt string, we will include these *context variables* in a way so the kernel can render them as their values. The default SK approach is to use `{{$<key-string>}}`, so we will use `{{$history}}` and `{{$userInput}}`. 

In the sample code, the prompt is stored directly in a `string` called `chatFunctionPrompt`.

# [C#](#tab/Csharp)

```csharp
// Initialize the prompt string.
string chatFunctionPrompt = 
    @$"{{{{${ContextVariableKeyHistory}}}}}
    {PromptCueUser} {{{{${ContextVariableKeyUserInput}}}}}
    {PromptCueChatBot}";
```

After dereferencing the `const string`s, this becomes:
        
```csharp
// Initialize the prompt string.
string chatFunctionPrompt = 
    @"{{$history}}
    User: {{$userInput}}
    ChatBot: ";
```

---

When the prompt is sent to the AI Service, however, the LLM needs a bit more information to generate the completion. So we must also define a prompt configuration. You can learn more about the `PromptTemplateConfig`` and its properties [here](/dotnet/api/microsoft.semantickernel.semanticfunctions.prompttemplateconfig?view=semantic-kernel-dotnet).

# [C#](#tab/Csharp)

```csharp
// Initialize the prompt configuration.
var chatFunctionPromptConfig = new PromptTemplateConfig
{
    Completion = 
    {
        MaxTokens = 2000,
        Temperature = 0.7,
        TopP = 0.5,
    }
};
```

---

With both the prompt and the prompt configuration now defined, we can register the prompt as a *semantic function* with the kernel. By registering it, the kernel will know about the function and can use it. 

# [C#](#tab/Csharp)

```csharp
// Register the semantic function with your semantic kernel.
// (NOTE: This is not the standard approach. Used here for simplicity.)
var chatPromptTemplate = new PromptTemplate(chatFunctionPrompt, chatFunctionPromptConfig, kernel);
var chatFunctionConfig = new SemanticFunctionConfig(chatFunctionPromptConfig, chatPromptTemplate);
var chatFunction = kernel.RegisterSemanticFunction(FunctionNameChat, chatFunctionConfig);
```

---

At this point, you have a *semantic function* called `chatFunction` (composed of a prompt and a prompt config) and *context variables* defined by `chatFunctionVariables`. To inject the values of the *context variables* into your prompt and send it off to the LLM, the kernel will run the semantic function.

# [C#](#tab/Csharp)

```csharp
var chatCompletion = await kernel.RunAsync(chatFunction, chatFunctionVariables);
```

---

This code calls the *semantic function* `chatFunction` with the input `chatFunctionVariables` and returns the output `chatCompletion`.

To have a complete chat experience, however, the *context variables* should be updated in between each prompt submission to the LLM. Otherwise, the same prompt will be sent over and over! To do this, the value for the key `"history"` is updated with the latest prompt sent, and the value for the key `"userInput"` is updated with what was read from the console.

# [C#](#tab/Csharp)

```csharp
history += 
    @$"{PromptCueUser}{userInput}
    {PromptCueChatBot}{chatCompletion}
    ";
chatFunctionVariables.Set(ContextVariableKeyHistory, history);
chatFunctionVariables.Set(ContextVariableKeyUserInput, userInput);

chatCompletion = await kernel.RunAsync(chatFunction, chatFunctionVariables);
```

After dereferencing the `const string`s, this becomes:

```csharp
history += 
    @"User: ${userInput}
    Chatbot: ${chatCompletion}
    ";
chatFunctionVariables.Set("history", history);
chatFunctionVariables.Set("userInput", userInput);

chatCompletion = await kernel.RunAsync(chatFunction, chatFunctionVariables);
```
-----