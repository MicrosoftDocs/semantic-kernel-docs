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

# What is context?

**Context** is information that helps clarify meaning, oftentimes relating to the surrounding circumstances.

In a conversation between two people, **context** could be what was previously said or information about where the converstion is taking place. The folks conversing will likely store this knowledge in their own short-term memory, and it will help them understand the dialogue as it progresses.

An LLM also needs **context** to more clearly understand what you are telling it. Unlike humans, however, current LLMs don't immediately store this information. They are *stateless*. This means an LLM will not recall information you provide it from one exchange to the next, *unless* you provide it *again*. Therefore, a best practice is to include context in each prompt.

To demonstrate, let's look at a simple chat application.

# Chat sample application
Clone the GitHub sample code below in your preferred language. Follow the instructions in the sample README to run the chat console app.

| Language  | Sample Chat Application |
| --- | --- |
| C# | [Open solution in GitHub](tbd) |
| Python | [Open solution in GitHub](tbd) |

When the default application is run, the conversation history is included in each prompt. This is the added context. You can also run the application without this context. Try it and see what happens!

# Context with the Semantic Kernel SDK

The Semantic Kernel (SK) includes context in a prompt via `ContextVariables` and `semantic functions`. Ultimately, you will send your ptompt (run your semantic function) using context variables (input) and receive the chat completion (output). 

```csharp
var chatCompletion = await kernel.RunAsync(chatFunction, chatFunctionVariables);
```

Let's begin with the prompt.

In the Chat101 sample, each prompt includes the history of the conversation, the user's input, and placement for the LLM's completion. This prompt is stored directly in a `string` called `chatFunctionPrompt`.  
        
```csharp
// Initialize the prompt.
// (NOTE: This is not the standard approach. Used here for simplicity.)
string chatFunctionPrompt = 
    @$"{{{{${ContextVariableKeyHistory}}}}}
    {PromptStringUser} {{{{${ContextVariableKeyUserInput}}}}}
    {PromptStringChatBot}";
```

After dereferencing the `const string`s, this becomes:
        
```csharp
// Initialize the prompt.
// (NOTE: This is not the standard approach. Used here for simplicity.)
string chatFunctionPrompt = 
    @"{{$history}}
    User: {{$userInput}}
    ChatBot: ";
```

The `$history` and `$userInput` are context variables, represented as key-value pairs and stored in a Dictionary called `Context ariables`. In the sample, the `ContextVariables` Dictionary is initialized and called `chatFunctionVariables`.

```csharp
// Initialize the context variables that will be used.
var chatFunctionVariables = new ContextVariables
{
    [ContextVariableKeyHistory] = string.Empty,
    [ContextVariableKeyUserInput] = string.Empty,
};
```

After dereferencing the `const string`s, this becomes:
        
```csharp
// Initialize the context variables that will be used.
var chatFunctionVariables = new ContextVariables
{
    ["history"] = string.Empty,
    ["userInput"] = string.Empty,
};
```

`chatFunctionVariables` will need to be updated and injected into the prompt string each time the Semantic Kernel sends the prompt to the LLM. To accomplish this, SK uses `Semantic Functions`.

A `Semantic Function` is simply a prompt string (above) and a prompt configuration json (not shown here). The input to the semantic function is the `ContextVariables` Dictionary (above), and the output from the `Semantic Function` is the LLM completion. Once the semantic function is registered to your semantic kernel (so it knows about it), you will have a complete function that you can run!

```csharp
// Register the semantic function with your semantic kernel.
// (NOTE: This is not the standard approach. Used here for simplicity.)
var chatPromptTemplate = new PromptTemplate(chatFunctionPrompt, chatFunctionPromptConfig, kernel);
var chatFunctionConfig = new SemanticFunctionConfig(chatFunctionPromptConfig, chatPromptTemplate);
var chatFunction = kernel.RegisterSemanticFunction(FunctionNameChat, chatFunctionConfig);
```

You can send the prompt (run the semantic function `chatFunction`) using context variables (the input `chatFunctionVariables`) and receive the LLM chat completion (output). Use the following:

```csharp
var chatCompletion = await kernel.RunAsync(chatFunction, chatFunctionVariables);
```

The last step is to update your context variables before sending off the prompt again. Otherwise, the same prompt will be sent over and over! To do this, history is updated with the latest prompt sent, and userInput is updated with what was read from the console.

```csharp
history += 
    @$"{PromptStringUser}{userInput}
    {PromptStringChatBot}{chatCompletion}
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

# API Documentation
- dotnet: https://learn.microsoft.com/en-us/dotnet/api/microsoft.semantickernel.orchestration?view=semantic-kernel-dotnet
