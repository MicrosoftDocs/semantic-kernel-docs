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

> [!IMPORTANT]
> This feature is in the experimental stage. Features at this stage are under active development and may change significantly before advancing to the preview or release candidate stage.

## Overview

Contextual Function Selection is an advanced capability in the Semantic Kernel Agent Framework that enables agents to dynamically select and advertise only the most relevant functions based on the current conversation context. Instead of exposing all available functions to the AI model, this feature uses Retrieval-Augmented Generation (RAG) to intelligently filter and present only those functions that are most pertinent to the user’s request.

This approach addresses the challenge of function selection when dealing with large numbers of available functions, where AI models may otherwise struggle to choose the appropriate function, leading to confusion and suboptimal performance.

## How Contextual Function Selection Works

When an agent is configured with contextual function selection, it leverages a vector store and an embedding generator to semantically match the current conversation context (including previous messages, and user input) with the descriptions and names of available functions. The most relevant functions, up to the specified limit, are then advertised to the AI model for invocation.

This mechanism is especially useful for agents that have access to a broad set of plugins or tools, ensuring that only contextually appropriate actions are considered at each step.


## Usage Example

The following example demonstrates how an agent can be configured to use contextual function selection. The agent is set up to summarize customer reviews, but only the most relevant functions are advertised to the AI model for each invocation. The `GetAvailableFunctions` method intentionally includes a mix of relevant and irrelevant functions to highlight the benefits of contextual selection.


```csharp
// Create an embedding generator for function vectorization
var embeddingGenerator = new AzureOpenAIClient(new Uri("<endpoint>"), new ApiKeyCredential("<api-key>"))
    .GetEmbeddingClient("<deployment-name>")
    .AsIEmbeddingGenerator();

// Create kernel and register AzureOpenAI chat completion service
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion("<deployment-name>", "<endpoint>", "<api-key>");
var kernel = builder.Build();

// Create a chat completion agent
ChatCompletionAgent agent = new()
{
    Name = "ReviewGuru",
    Instructions = "You are a friendly assistant that summarizes key points and sentiments from customer reviews. For each response, list available functions.",
    Kernel = kernel,
    Arguments = new(new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new FunctionChoiceBehaviorOptions { RetainArgumentTypes = true }) })
};

// Create the agent thread and register the contextual function provider
ChatHistoryAgentThread agentThread = new();

agentThread.AIContextProviders.Add(
    new ContextualFunctionProvider(
        vectorStore: new InMemoryVectorStore(new InMemoryVectorStoreOptions() { EmbeddingGenerator = embeddingGenerator }),
        vectorDimensions: 1536,
        functions: AvailableFunctions(),
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

> [!TIP]
> See a complete sample: [ChatCompletion_ContextualFunctionSelection.cs](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/ChatCompletion_ContextualFunctionSelection.cs)

## Context Size

The context size determines how many recent messages from previous agent invocations are included when forming the context for a new invocation. 
The provider collects all messages from previous invocations, up to the specified number, and prepends them to the new messages of the new invocation to form the context.

Using recent messages together with new messages is especially useful for tasks that require information from earlier steps in a conversation. 
For example, if an agent provisions a resource in one invocation and deploys it in the next, the deployment step can access details from the provisioning step by including 
recent messages in the context.

The default value for the number of recent messages in context is 2, but this can be configured as needed by specifying the `NumberOfRecentMessagesInContext` property in the `ContextualFunctionProviderOptions`:

```csharp
ContextualFunctionProviderOptions options = new ()
{
    NumberOfRecentMessagesInContext = 1 // Only the last message will be included in the context
};
```

## Customizing Context Embedding Value

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