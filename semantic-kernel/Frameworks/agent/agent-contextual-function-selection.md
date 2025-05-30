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

When an agent is configured with contextual function selection, it leverages a vector store and an embedding generator to semantically match the current conversation context (including previous messages, and user input) with the descriptions and names of available functions. The most relevant functions are then advertised to the AI model for invocation.

This mechanism is especially useful for agents that have access to a broad set of plugins or tools, ensuring that only contextually appropriate actions are considered at each step.

## Main Example

The following conceptual example demonstrates how an agent can be configured to use contextual function selection. The agent is set up to summarize customer reviews, but only the most relevant functions are advertised to the AI model for each invocation.

```csharp
// Create an embedding generator for function vectorization
var embeddingGenerator = new AzureOpenAIClient(new Uri("<endpoint>"), new ApiKeyCredential("<api-key>"))
    .GetEmbeddingClient("<deployment-name>")
    .AsIEmbeddingGenerator();

// Create kernel
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion("<deployment-name>", "<endpoint>", "<api-key>");
var kernel = builder.Build();

// Create the agent
ChatCompletionAgent agent = new()
{
    Name = "ReviewGuru",
    Instructions = "You are a friendly assistant that summarizes key points and sentiments from customer reviews. For each response, list available functions.",
    Kernel = kernel,
    Arguments = new(new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new FunctionChoiceBehaviorOptions { RetainArgumentTypes = true }) })
};

// Register the contextual function provider
ChatHistoryAgentThread agentThread = new();
var allAvailableFunctions = GetAvailableFunctions();

agentThread.AIContextProviders.Add(
    new ContextualFunctionProvider(
        vectorStore: new InMemoryVectorStore(new InMemoryVectorStoreOptions() { EmbeddingGenerator = embeddingGenerator }),
        vectorDimensions: 1536,
        functions: allAvailableFunctions,
        maxNumberOfFunctions: 3, // Only the top 3 relevant functions are advertised
        loggerFactory: LoggerFactory
    )
);

// Invoke the agent
ChatMessageContent message = await agent.InvokeAsync("Get and summarize customer review.", agentThread).FirstAsync();
Console.WriteLine(message.Content);
```

> [!TIP]
> See a complete sample: [ChatCompletion_ContextualFunctionSelection.cs](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/ChatCompletion_ContextualFunctionSelection.cs)

## Configuration Options

