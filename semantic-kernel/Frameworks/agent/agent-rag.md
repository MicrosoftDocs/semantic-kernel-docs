---
title: Adding Retrieval Augmented Generation (RAG) to Semantic Kernel Agents
description: How to use the TextSearchProvider for Retrieval Augmented Generation (RAG) with Semantic Kernel Agents
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 05/22/2025
ms.service: semantic-kernel
---

# Adding Retrieval Augmented Generation (RAG) to Semantic Kernel Agents

::: zone pivot="programming-language-csharp"

> [!WARNING]
> The Semantic Kernel Agent RAG functionality is experimental, subject to change, and will only be finalized based on feedback and evaluation.

## Using the TextSearchProvider for RAG

The `Microsoft.SemanticKernel.Data.TextSearchProvider` allows agents to retrieve relevant documents based on user input and inject them into the agent's context for more informed responses.
It integrates an `Microsoft.SemanticKernel.Data.ITextSearch` instance with Semantic Kernel agents.
Multiple `ITextSearch` implementations exist, supporting similarity searches on vector stores and search engine integration.
More information can be found [here](../../concepts/text-search/index.md).

We also provide a `Microsoft.SemanticKernel.Data.TextSearchStore`, which provides simple, opinionated vector storage of textual data for the purposes of Retrieval Augmented Generation.
`TextSearchStore` has a built-in schema for storing and retrieving textual data in a vector store. If you wish to use your own schema for storage, check out [VectorStoreTextSearch](../../concepts/text-search/text-search-vector-stores.md).

## Setting Up the TextSearchProvider

The `TextSearchProvider` can be used with a `VectorStore` and `TextSearchStore` to store and search text documents.

The following example demonstrates how to set up and use the `TextSearchProvider` with a `TextSearchStore` and `InMemoryVectorStore` for an agent to perform simple RAG over text.

```csharp
// Create an embedding generator using Azure OpenAI.
var embeddingGenerator = new AzureOpenAIClient(new Uri("<Your_Azure_OpenAI_Endpoint>"), new AzureCliCredential())
    .GetEmbeddingClient("<Your_Deployment_Name>")
    .AsIEmbeddingGenerator(1536);

// Create a vector store to store documents.
var vectorStore = new InMemoryVectorStore(new() { EmbeddingGenerator = embeddingGenerator });

// Create a TextSearchStore for storing and searching text documents.
using var textSearchStore = new TextSearchStore<string>(vectorStore, collectionName: "FinancialData", vectorDimensions: 1536);

// Upsert documents into the store.
await textSearchStore.UpsertTextAsync(new[]
{
    "The financial results of Contoso Corp for 2024 is as follows:\nIncome EUR 154 000 000\nExpenses EUR 142 000 000",
    "The Contoso Corporation is a multinational business with its headquarters in Paris."
});

// Create an agent.
Kernel kernel = new Kernel();
ChatCompletionAgent agent = new()
{
    Name = "FriendlyAssistant",
    Instructions = "You are a friendly assistant",
    Kernel = kernel,
    // This setting must be set to true when using the on-demand RAG feature
    UseImmutableKernel = true
};

// Create an agent thread and add the TextSearchProvider.
ChatHistoryAgentThread agentThread = new();
var textSearchProvider = new TextSearchProvider(textSearchStore);
agentThread.AIContextProviders.Add(textSearchProvider);

// Use the agent with RAG capabilities.
ChatMessageContent response = await agent.InvokeAsync("Where is Contoso based?", agentThread).FirstAsync();
Console.WriteLine(response.Content);
```

## Advanced Features: Citations and Filtering

The `TextSearchStore` supports advanced features such as filtering results by namespace and including citations in responses.

### Including Citations

Documents in the `TextSearchStore` can include metadata like source names and links, enabling citation generation in agent responses.

```csharp
await textSearchStore.UpsertDocumentsAsync(new[]
{
    new TextSearchDocument
    {
        Text = "The financial results of Contoso Corp for 2023 is as follows:\nIncome EUR 174 000 000\nExpenses EUR 152 000 000",
        SourceName = "Contoso 2023 Financial Report",
        SourceLink = "https://www.contoso.com/reports/2023.pdf",
        Namespaces = ["group/g2"]
    }
});
```

When the `TextSearchProvider` retrieves this document, it will by default include the source name and link in its response.

### Filtering by Namespace

When upserting documents you can optionally provide one or more namespaces for each document.
Namespaces can be any string that defines the scope of a document.
You can then configure the `TextSearchStore` to limit search results to only those records that match the requested namespace.

```csharp
using var textSearchStore = new TextSearchStore<string>(
    vectorStore,
    collectionName: "FinancialData",
    vectorDimensions: 1536,
    new() { SearchNamespace = "group/g2" }
);
```

### Automatic vs on-demand RAG

The `TextSearchProvider` can perform searches automatically during each agent invocation or allow on-demand searches via tool calls when the agent needs additional information.

The default setting is `BeforeAIInvoke`, which means that searches will be performed before each agent invocation using the message passed to the agent.
This can be changed to `OnDemandFunctionCalling`, which will allow the Agent to make a tool call to do searches using a search string of the agent's choice.

```csharp
var options = new TextSearchProviderOptions
{
    SearchTime = TextSearchProviderOptions.RagBehavior.OnDemandFunctionCalling,
};

var provider = new TextSearchProvider(mockTextSearch.Object, options: options);
```

> [!WARNING]
> When using the `TextSearchProvider` with `OnDemandFunctionCalling`, the `UseImmutableKernel` setting on the agent has to be set to `true` as the feature requires cloning the kernel when invoking the agent.
> Note that setting `UseImmutableKernel` to `true` will mean that any kernel data modifications done during the agent invocation by e.g. plugins, will not be retained after the invocation completes.

## TextSearchProvider options

The `TextSearchProvider` can be configured with various options to customize its behavior. Options are provided using the `TextSearchProviderOptions` class to the `TextSearchProvider` constructor.

### Top

Specifies the maximum number of results to return from the similarity search.

- **Default**: 3

### SearchTime

Controls when the text search is performed. Options include:

- **BeforeAIInvoke**: A search is performed each time the model/agent is invoked, just before invocation, and the results are provided to the model/agent via the invocation context.
- **OnDemandFunctionCalling**: A search may be performed by the model/agent on demand via function calling.

### PluginFunctionName

Specifies the name of the plugin method that will be made available for searching if `SearchTime` is set to `OnDemandFunctionCalling`.

- **Default**: "Search"

### PluginFunctionDescription

Provides a description of the plugin method that will be made available for searching if `SearchTime` is set to `OnDemandFunctionCalling`.

- **Default**: "Allows searching for additional information to help answer the user question."

### ContextPrompt

When providing the text chunks to the AI model on invocation, a prompt is required to indicate to the AI model what the text chunks are for and how they should be used.
This setting allows overriding the default messaging that is built into the `TextSearchProvider`.

### IncludeCitationsPrompt

When providing the text chunks to the AI model on invocation, a prompt is required to tell to the AI model whether and how to do citations.
This setting allows overriding the default messaging that is built into the `TextSearchProvider`.

### ContextFormatter

This optional callback can be used to completely customize the text that is produced by the `TextSearchProvider`.
By default the `TextSearchProvider` will produce text that includes

1. A prompt telling the AI model what the text chunks are for.
2. The list of text chunks with source links and names.
3. A prompt instructing the AI model about including citations.

You can write your own output by implementing and providing this callback.

**Note**: If this delegate is provided, the `ContextPrompt` and `IncludeCitationsPrompt` settings will not be used.

## Combining RAG with Other Providers

The `TextSearchProvider` can be combined with other providers, such as `mem0` or `WhiteboardProvider`, to create agents with both memory and retrieval capabilities.

```csharp
// Add both mem0 and TextSearchProvider to the agent thread.
agentThread.AIContextProviders.Add(mem0Provider);
agentThread.AIContextProviders.Add(textSearchProvider);

// Use the agent with combined capabilities.
ChatMessageContent response = await agent.InvokeAsync("What was Contoso's income for 2023?", agentThread).FirstAsync();
Console.WriteLine(response.Content);
```

By combining these features, agents can deliver a more personalized and context-aware experience.

## Next steps

> [!div class="nextstepaction"]
> [Explore the Agent with RAG sample](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/ChatCompletion_Rag.cs)

::: zone-end

::: zone pivot="programming-language-python"

## Coming Soon

More information coming soon.

::: zone-end

::: zone pivot="programming-language-java"

## Coming Soon

More information coming soon.

::: zone-end