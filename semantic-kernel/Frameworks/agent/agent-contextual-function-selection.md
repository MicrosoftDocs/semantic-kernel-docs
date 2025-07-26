---
title: Contextual Function Selection with Semantic Kernel Agents
description: An overview of contextual function selection using RAG to dynamically choose the most relevant functions for agent interactions.
zone_pivot_groups: programming-languages
author: sergeymenshykh
ms.topic: conceptual
ms.author: semenshi
ms.date: 12/30/2024
ms.service: semantic-kernel
---

# Contextual Function Selection with Agents

::: zone pivot="programming-language-csharp"

> [!IMPORTANT]
> This feature is in the experimental stage. Features at this stage are under active development and may change significantly before advancing to the preview or release candidate stage.

## Overview

Contextual Function Selection is an advanced capability in the Semantic Kernel Agent Framework that enables agents to dynamically select and advertise only the most relevant functions based on the current conversation context. Instead of exposing all available functions to the AI model, this feature uses Retrieval-Augmented Generation (RAG) to filter and present only those functions most pertinent to the user’s request.

This approach addresses the challenge of function selection when dealing with a large number of available functions, where AI models may otherwise struggle to choose the appropriate function, leading to confusion and suboptimal performance.

> [!WARNING]
> When using the `ContextualFunctionProvider`, the `UseImmutableKernel` setting on the agent has to be set to `true` as the feature requires cloning the kernel when invoking the agent.
> Note that setting `UseImmutableKernel` to `true` will mean that any kernel data modifications done during the agent invocation by e.g. plugins, will not be retained after the invocation completes.

## How Contextual Function Selection Works

When an agent is configured with contextual function selection, it leverages a vector store and an embedding generator to semantically match the current conversation context (including previous messages and user input) with the descriptions and names of available functions. The most relevant functions, up to the specified limit, are then advertised to the AI model for invocation.

This mechanism is especially useful for agents that have access to a broad set of plugins or tools, ensuring that only contextually appropriate actions are considered at each step.

## Usage Example

The following example demonstrates how an agent can be configured to use contextual function selection. The agent is set up to summarize customer reviews, but only the most relevant functions are advertised to the AI model for each invocation. The `GetAvailableFunctions` method intentionally includes both relevant and irrelevant functions to highlight the benefits of contextual selection.


```csharp
// Create an embedding generator for function vectorization
var embeddingGenerator = new AzureOpenAIClient(new Uri("<endpoint>"), new ApiKeyCredential("<api-key>"))
    .GetEmbeddingClient("<deployment-name>")
    .AsIEmbeddingGenerator();

// Create kernel and register AzureOpenAI chat completion service
var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion("<deployment-name>", "<endpoint>", "<api-key>");
    .Build();

// Create a chat completion agent
ChatCompletionAgent agent = new()
{
    Name = "ReviewGuru",
    Instructions = "You are a friendly assistant that summarizes key points and sentiments from customer reviews. For each response, list available functions.",
    Kernel = kernel,
    Arguments = new(new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new FunctionChoiceBehaviorOptions { RetainArgumentTypes = true }) }),
    // This setting must be set to true when using the ContextualFunctionProvider
    UseImmutableKernel = true
};

// Create the agent thread and register the contextual function provider
ChatHistoryAgentThread agentThread = new();

agentThread.AIContextProviders.Add(
    new ContextualFunctionProvider(
        vectorStore: new InMemoryVectorStore(new InMemoryVectorStoreOptions() { EmbeddingGenerator = embeddingGenerator }),
        vectorDimensions: 1536,
        functions: GetAvailableFunctions(),
        maxNumberOfFunctions: 3, // Only the top 3 relevant functions are advertised
        loggerFactory: LoggerFactory
    )
);


// Invoke the agent
ChatMessageContent message = await agent.InvokeAsync("Get and summarize customer review.", agentThread).FirstAsync();
Console.WriteLine(message.Content);

// Output
/*
    Customer Reviews:
    -----------------
    1. John D. - ★★★★★
       Comment: Great product and fast shipping!
       Date: 2023-10-01

    Summary:
    --------
    The reviews indicate high customer satisfaction,
    highlighting product quality and shipping speed.

    Available functions:
    --------------------
    - Tools-GetCustomerReviews
    - Tools-Summarize
    - Tools-CollectSentiments
*/

IReadOnlyList<AIFunction> GetAvailableFunctions()
{
    // Only a few functions are directly related to the prompt; the majority are unrelated to demonstrate the benefits of contextual filtering.
    return new List<AIFunction>
    {
        // Relevant functions
        AIFunctionFactory.Create(() => "[ { 'reviewer': 'John D.', 'date': '2023-10-01', 'rating': 5, 'comment': 'Great product and fast shipping!' } ]", "GetCustomerReviews"),
        AIFunctionFactory.Create((string text) => "Summary generated based on input data: key points include customer satisfaction.", "Summarize"),
        AIFunctionFactory.Create((string text) => "The collected sentiment is mostly positive.", "CollectSentiments"),

        // Irrelevant functions
        AIFunctionFactory.Create(() => "Current weather is sunny.", "GetWeather"),
        AIFunctionFactory.Create(() => "Email sent.", "SendEmail"),
        AIFunctionFactory.Create(() => "The current stock price is $123.45.", "GetStockPrice"),
        AIFunctionFactory.Create(() => "The time is 12:00 PM.", "GetCurrentTime")
    };
}
```

## Vector Store

The provider is primarily designed to work with in-memory vector stores, which offer simplicity. However, if other types of vector stores are used, it is important to note that the responsibility for handling data synchronization and consistency falls on the hosting application.

Synchronization is necessary whenever the list of functions changes or when the source of function embeddings is modified. For example, if an agent initially has three functions (f1, f2, f3) that are vectorized and stored in a cloud vector store, and later f3 is removed from the agent's list of functions, the vector store must be updated to reflect only the current functions the agent has (f1 and f2). Failing to update the vector store may result in irrelevant functions being returned as results. Similarly, if the data used for vectorization such as function names, descriptions, etc. changes, the vector store should be purged and repopulated with new embeddings based on the updated information.

Managing data synchronization in external or distributed vector stores can be complex and prone to errors, especially in distributed applications where different services or instances 
may operate independently and require consistent access to the same data. In contrast, using an in-memory store simplifies this process: when the function list or vectorization source 
changes, the in-memory store can be easily recreated with the new set of functions and their embeddings, ensuring consistency with minimal effort.

## Specifying Functions

The contextual function provider must be supplied with a list of functions from which it can select the most relevant ones based on the current context. This is accomplished by providing a list of functions to the `functions` parameter of the `ContextualFunctionProvider` constructor.
   
In addition to the functions, you must also specify the maximum number of relevant functions to return using the `maxNumberOfFunctions` parameter.
This parameter determines how many functions the provider will consider when selecting the most relevant ones for the current context.
The specified number is not meant to be precise; rather, it serves as an upper limit that depends on the specific scenario.  
   
Setting this value too low may prevent the agent from accessing all the necessary functions for a scenario, potentially leading to the scenario failure.
Conversely, setting it too high may overwhelm the agent with too many functions, which can result in hallucinations, excessive input token consumption, and suboptimal performance.

```csharp
// Create the provider with a list of functions and a maximum number of functions to return
ContextualFunctionProvider provider = new (
    vectorStore: new InMemoryVectorStore(new InMemoryVectorStoreOptions { EmbeddingGenerator = embeddingGenerator }),
    vectorDimensions: 1536,
    functions: [AIFunctionFactory.Create((string text) => $"Echo: {text}", "Echo"), <other functions>]
    maxNumberOfFunctions: 3 // Only the top 3 relevant functions are advertised
);
```

## Contextual Function Provider Options

The provider can be configured using the `ContextualFunctionProviderOptions` class, which allows you to customize various aspects of how the provider operates:

```csharp
// Create options for the contextual function provider
ContextualFunctionProviderOptions options = new ()
{
    ...
};

// Create the provider with options
ContextualFunctionProvider provider = new (
    ...
    options: options // Pass the options
);
```

### Context Size

The context size determines how many recent messages from previous agent invocations are included when forming the context for a new invocation. The provider collects all messages from previous invocations, up to the specified number, and prepends them to the new messages to form the context.

Using recent messages together with new messages is especially useful for tasks that require information from earlier steps in a conversation. 
For example, if an agent provisions a resource in one invocation and deploys it in the next, the deployment step can access details from the provisioning step to get provisioned resource information for the deployment.

The default value for the number of recent messages in context is 2, but this can be configured as needed by specifying the `NumberOfRecentMessagesInContext` property in the `ContextualFunctionProviderOptions`:

```csharp
ContextualFunctionProviderOptions options = new ()
{
    NumberOfRecentMessagesInContext = 1 // Only the last message will be included in the context
};
```

### Context Embedding Source Value

To perform contextual function selection, the provider needs to vectorize the current context so it can be compared with available functions in the vector store. By default, the provider creates this context embedding by concatenating all non-empty recent and new messages into a single string, which is then vectorized and used to search for relevant functions.

In some scenarios, you may want to customize this behavior to:

- Focus on specific message types (e.g., only user messages)
- Exclude certain information from context
- Preprocess or summarize the context before vectorization (e.g., apply prompt rewriting)

To do this, you can assign a custom delegate to `ContextEmbeddingValueProvider`. This delegate receives the recent and new messages, and returns a string value to be used as a source for the context embedding:

```csharp
ContextualFunctionProviderOptions options = new()
{
    ContextEmbeddingValueProvider = async (recentMessages, newMessages, cancellationToken) =>
    {
        // Example: Only include user messages in the embedding
        var allUserMessages = recentMessages.Concat(newMessages)
            .Where(m => m.Role == "user")
            .Select(m => m.Content)
            .Where(content => !string.IsNullOrWhiteSpace(content));
        return string.Join("\n", allUserMessages);
    }
};
```

Customizing the context embedding can improve the relevance of function selection, especially in complex or highly specialized agent scenarios.

### Function Embedding Source Value

The provider needs to vectorize each available function in order to compare it with the context and select the most relevant ones. By default, the provider creates a function embedding by concatenating the function's name and description into a single string, which is then vectorized and stored in the vector store.

You can customize this behavior using the `EmbeddingValueProvider` property of `ContextualFunctionProviderOptions`. This property allows you to specify a callback that receives the function and a cancellation token, and returns a string to be used as the source for the function embedding. This is useful if you want to:

- Add additional function metadata to the embedding source
- Preprocess, filter, or reformat the function information before vectorization

```csharp
ContextualFunctionProviderOptions options = new()
{
    EmbeddingValueProvider = async (function, cancellationToken) =>
    {
        // Example: Use only the function name for embedding
        return function.Name;
    }
};
```

Customizing the function embedding source value can improve the accuracy of function selection, especially when your functions have rich, context-relevant metadata or when you want to focus the search on specific aspects of each function.

## Next steps
[Explore the contextual function selection samples](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/ChatCompletion_ContextualFunctionSelection.cs)

::: zone-end

::: zone pivot="programming-language-python"

## Coming Soon

More information coming soon.

::: zone-end

::: zone pivot="programming-language-java"

## Coming Soon

More information coming soon.

::: zone-end
