---
title: Using the Semantic Kernel Vector Store text search (Preview)
description: Contains information on how to use a Semantic Kernel Text Search with Vector Store.
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: conceptual
ms.author: markwallace
ms.date: 10/21/2024
ms.service: semantic-kernel
---
# Using the Vector Store Text Search (Preview)

> [!WARNING]
> The Semantic Kernel Text Search functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone pivot="programming-language-csharp"

## Overview

The Vector Store Text Search implementation uses the [Vector Store Connectors](../../vector-store-connectors/out-of-the-box-connectors/index.md) to retrieve search results. This means you can use Vector Store Text Search with any Vector Store which Semantic Kernel supports and any implementation of [Microsoft.Extensions.VectorData.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions).

## Limitations

See the limitations listed for the [Vector Store connector](../../vector-store-connectors/out-of-the-box-connectors/index.md) you are using.

## Getting started

The sample below shows how to use an in-memory vector store to create a `VectorStoreTextSearch` and use it to perform a text search.

```csharp
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Embeddings;

// Create an embedding generation service.
var textEmbeddingGeneration = new OpenAITextEmbeddingGenerationService(
        modelId: TestConfiguration.OpenAI.EmbeddingModelId,
        apiKey: TestConfiguration.OpenAI.ApiKey);

// Construct an InMemory vector store.
var vectorStore = new InMemoryVectorStore();
var collectionName = "records";

// Get and create collection if it doesn't exist.
var recordCollection = vectorStore.GetCollection<TKey, TRecord>(collectionName);
await recordCollection.EnsureCollectionExistsAsync().ConfigureAwait(false);

// TODO populate the record collection with your test data
// Example https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Search/VectorStore_TextSearch.cs

// Create a text search instance using the InMemory vector store.
var textSearch = new VectorStoreTextSearch<DataModel>(recordCollection, textEmbeddingGeneration);

// Search and return results as TextSearchResult items
var query = "What is the Semantic Kernel?";
KernelSearchResults<TextSearchResult> textResults = await textSearch.GetTextSearchResultsAsync(query, new() { Top = 2, Skip = 0 });
Console.WriteLine("\n--- Text Search Results ---\n");
await foreach (TextSearchResult result in textResults.Results)
{
    Console.WriteLine($"Name:  {result.Name}");
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Link:  {result.Link}");
}
```

::: zone-end
::: zone pivot="programming-language-python"

## Coming soon

More coming soon.

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More coming soon.

::: zone-end

## Next steps

The following sections of the documentation show you how to:

1. Create a [plugin](../text-search-plugins.md) and use it for Retrieval Augmented Generation (RAG).
2. Use text search together with [function calling](../text-search-function-calling.md).
3. Learn more about using [vector stores](../text-search-vector-stores.md) for text search.

> [!div class="nextstepaction"]
> [Text Search Abstractions](../text-search-abstractions.md)
> [Text Search Plugins](../text-search-plugins.md)
> [Text Search Function Calling](../text-search-function-calling.md)
> [Text Search with Vector Stores](../text-search-vector-stores.md)
