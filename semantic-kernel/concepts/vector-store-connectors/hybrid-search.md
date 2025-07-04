---
title: Hybrid search using Semantic Kernel Vector Store connectors (Preview)
description: Describes the different options you can use when doing a hybrid search using Semantic Kernel vector store connectors.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 03/06/2025
ms.service: semantic-kernel
---
# Hybrid search using Semantic Kernel Vector Store connectors (Preview)

::: zone pivot="programming-language-csharp"

::: zone-end
::: zone pivot="programming-language-python"


There are two searches currently supported in the Semantic Kernel Vector Store abstractions:
1. `search` -> see [Search](./vector-search.md)
2. `hybrid_search`
   1. This is search based on a text value and a vector, if the vector is not supplied, it will be generated using the `embedding_generator` field on the data model or record definition, or by the vector store itself.

All searches can take a optional set of parameters:
- `vector`: A vector used to search, can be supplied instead of the values, or in addition to the values for hybrid.
- `top`: The number of results to return, defaults to 3.
- `skip`: The number of results to skip, defaults to 0.
- `include_vectors`: Whether to include the vectors in the results, defaults to `false`.
- `filter`: A filter to apply to the results before the vector search is applied, defaults to `None`, in the form of a lambda expression: `lambda record: record.property == "value"`.
- `vector_property_name`: The name of the vector property to use for the search, defaults to the first vector property found on the data model or record definition.
- `additional_property_name`: The name of the additional field to use for the text search of the hybrid search.
- `include_total_count`: Whether to include the total count of results in the search result, defaults to `false`.

Assuming you have a collection that already contains data, you can easily search it. Here is an example using Azure AI Search.

```python
from semantic_kernel.connectors.azure_ai_search import AzureAISearchCollection, AzureAISearchStore

# Create a Azure AI Search VectorStore object and choose an existing collection that already contains records.
# Hotels is the data model decorated class.
store = AzureAISearchStore()
collection: AzureAISearchCollection[str, Hotels] = store.get_collection(Hotels, collection_name="skhotels")

search_results = await collection.hybrid_search(
    query, vector_property_name="vector", additional_property_name="description"
)
hotels = [record.record async for record in search_results.results]
print(f"Found hotels: {hotels}")
```

> [!TIP]
> For more information on how to generate embeddings see [embedding generation](./embedding-generation.md).

### Filters

The `filter` parameter can be used to provide a filter for filtering the records in the chosen collection. It is defined as a lambda expression, or a string of a lambda expression, e.g. `lambda record: record.property == "value"`.

It is important to understand that these are not executed directly, rather they are parsed into the syntax matching the vector stores, the only exception to this is the `InMemoryCollection` which does execute the filter directly.

Given this flexibility, it is important to review the documentation of a specific store to understand which filters are supported, for instance not all vector stores support negative filters (i.e. `lambda x: not x.value`), and that won't become apparent until the search is executed.

::: zone-end
::: zone pivot="programming-language-java"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end

::: zone pivot="programming-language-csharp"

Semantic Kernel provides hybrid search capabilities as part of its Vector Store abstractions. This supports filtering and many other options, which this article will explain in more detail.

Currently the type of hybrid search supported is based on a vector search, plus a keyword search, both of which are executed in parallel, after which a union of the two result sets
are returned. Sparse vector based hybrid search is not currently supported.

To execute a hybrid search, your database schema needs to have a vector field and a string field with full text search capabilities enabled.
If you are creating a collection using the Semantic Kernel vector storage connectors, make sure to enable the `IsFullTextIndexed` option
on the string field that you want to target for the keyword search.

> [!TIP]
> For more information on how to enable `IsFullTextIndexed` refer to [VectorStoreDataAttribute parameters](./defining-your-data-model.md#vectorstoredataattribute-parameters) or [VectorStoreDataProperty configuration settings](./schema-with-record-definition.md#vectorstoredataproperty-configuration-settings)

## Hybrid Search

The `HybridSearchAsync` method allows searching using a vector and an `ICollection` of string keywords. It also takes an optional `HybridSearchOptions<TRecord>` class as input.
This method is available on the following interface:

1. `IKeywordHybridSearchable<TRecord>`

Only connectors for databases that currently support vector plus keyword hybrid search are implementing this interface.

Assuming you have a collection that already contains data, you can easily do a hybrid search on it. Here is an example using Qdrant.

```csharp
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.Extensions.VectorData;
using Qdrant.Client;

// Placeholder embedding generation method.
async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string textToVectorize)
{
    // your logic here
}

// Create a Qdrant VectorStore object and choose an existing collection that already contains records.
VectorStore vectorStore = new QdrantVectorStore(new QdrantClient("localhost"), ownsClient: true);
IKeywordHybridSearchable<Hotel> collection = (IKeywordHybridSearchable<Hotel>)vectorStore.GetCollection<ulong, Hotel>("skhotels");

// Generate a vector for your search text, using your chosen embedding generation implementation.
ReadOnlyMemory<float> searchVector = await GenerateEmbeddingAsync("I'm looking for a hotel where customer happiness is the priority.");

// Do the search, passing an options object with a Top value to limit results to the single top match.
var searchResult = collection.HybridSearchAsync(searchVector, ["happiness", "hotel", "customer"], top: 1);

// Inspect the returned hotel.
await foreach (var record in searchResult)
{
    Console.WriteLine("Found hotel description: " + record.Record.Description);
    Console.WriteLine("Found record score: " + record.Score);
}
```

> [!TIP]
> For more information on how to generate embeddings see [embedding generation](./embedding-generation.md).

## Supported Vector Types

`HybridSearchAsync` takes a generic type as the vector parameter.
The types of vectors supported by each data store vary.
See [the documentation for each connector](./out-of-the-box-connectors/index.md) for the list of supported vector types.

It is also important for the search vector type to match the target vector that is being searched, e.g. if you have two vectors
on the same record with different vector types, make sure that the search vector you supply matches the type of the specific vector
you are targeting.
See [VectorProperty and AdditionalProperty](#vectorproperty-and-additionalproperty) for how to pick a target vector if you have more than one per record.

## Hybrid Search Options

The following options can be provided using the `HybridSearchOptions<TRecord>` class.

### VectorProperty and AdditionalProperty

The `VectorProperty` and `AdditionalProperty` options can be used to specify the vector property and full text search property to target during the search.

If no `VectorProperty` is provided and the data model contains only one vector, that vector will be used.
If the data model contains no vector or multiple vectors and `VectorProperty` is not provided, the search method will throw.

If no `AdditionalProperty` is provided and the data model contains only one full text search property, that property will be used.
If the data model contains no full text search property or multiple full text search properties and `AdditionalProperty` is not provided, the search method will throw.

```csharp
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.Extensions.VectorData;
using Qdrant.Client;

var vectorStore = new QdrantVectorStore(new QdrantClient("localhost"), ownsClient: true);
var collection = (IKeywordHybridSearchable<Product>)vectorStore.GetCollection<ulong, Product>("skproducts");

// Create the hybrid search options and indicate that we want
// to search the DescriptionEmbedding vector property and the
// Description full text search property.
var hybridSearchOptions = new HybridSearchOptions<Product>
{
    VectorProperty = r => r.DescriptionEmbedding,
    AdditionalProperty = r => r.Description
};

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var searchResult = collection.HybridSearchAsync(searchVector, ["happiness", "hotel", "customer"], top: 3, hybridSearchOptions);

public sealed class Product
{
    [VectorStoreKey]
    public int Key { get; set; }

    [VectorStoreData(IsFullTextIndexed = true)]
    public string Name { get; set; }

    [VectorStoreData(IsFullTextIndexed = true)]
    public string Description { get; set; }

    [VectorStoreData]
    public List<string> FeatureList { get; set; }

    [VectorStoreVector(1536)]
    public ReadOnlyMemory<float> DescriptionEmbedding { get; set; }

    [VectorStoreVector(1536)]
    public ReadOnlyMemory<float> FeatureListEmbedding { get; set; }
}
```

### Top and Skip

The `Top` and `Skip` options allow you to limit the number of results to the Top n results and
to skip a number of results from the top of the resultset.
Top and Skip can be used to do paging if you wish to retrieve a large number of results using separate calls.

```csharp
// Create the vector search options and indicate that we want to skip the first 40 results and then pass 20 to search to get the next 20.
var hybridSearchOptions = new HybridSearchOptions<Product>
{
    Skip = 40
};

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var searchResult = collection.HybridSearchAsync(searchVector, ["happiness", "hotel", "customer"], top: 20, hybridSearchOptions);

// Iterate over the search results.
await foreach (var result in searchResult)
{
    Console.WriteLine(result.Record.Description);
}
```

The default values for `Skip` is 0.

### IncludeVectors

The `IncludeVectors` option allows you to specify whether you wish to return vectors in the search results.
If `false`, the vector properties on the returned model will be left null.
Using `false` can significantly reduce the amount of data retrieved from the vector store during search,
making searches more efficient.

The default value for `IncludeVectors` is `false`.

```csharp
// Create the hybrid search options and indicate that we want to include vectors in the search results.
var hybridSearchOptions = new HybridSearchOptions<Product>
{
    IncludeVectors = true
};

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var searchResult = collection.HybridSearchAsync(searchVector, ["happiness", "hotel", "customer"], top: 3, hybridSearchOptions);

// Iterate over the search results.
await foreach (var result in searchResult)
{
    Console.WriteLine(result.Record.FeatureList);
}
```

### Filter

The vector search filter option can be used to provide a filter for filtering the records in the chosen collection
before applying the vector search.

This has multiple benefits:

- Reduce latency and processing cost, since only records remaining after filtering need to be compared with the search vector and therefore fewer vector comparisons have to be done.
- Limit the resultset for e.g. access control purposes, by excluding data that the user shouldn't have access to.

Note that in order for fields to be used for filtering, many vector stores require those fields to be indexed first.
Some vector stores will allow filtering using any field, but may optionally allow indexing to improve filtering performance.

If creating a collection via the Semantic Kernel vector store abstractions and you wish to enable filtering on a field,
set the `IsFilterable` property to true when defining your data model or when creating your record definition.

> [!TIP]
> For more information on how to set the `IsFilterable` property, refer to [VectorStoreRecordDataAttribute parameters](./defining-your-data-model.md#vectorstorerecorddatafield-parameters) or [VectorStoreRecordDataField configuration settings](./schema-with-record-definition.md).

Filters are expressed using LINQ expressions based on the type of the data model.
The set of LINQ expressions supported will vary depending on the functionality supported
by each database, but all databases support a broad base of common expressions, e.g. equals,
not equals, and, or, etc.

```csharp
// Create the hybrid search options and set the filter on the options.
var hybridSearchOptions = new HybridSearchOptions<Glossary>
{
    Filter = r => r.Category == "External Definitions" && r.Tags.Contains("memory")
};

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var searchResult = collection.HybridSearchAsync(searchVector, ["happiness", "hotel", "customer"], top: 3, hybridSearchOptions);

// Iterate over the search results.
await foreach (var result in searchResult)
{
    Console.WriteLine(result.Record.Definition);
}

sealed class Glossary
{
    [VectorStoreKey]
    public ulong Key { get; set; }

    // Category is marked as indexed, since we want to filter using this property.
    [VectorStoreData(IsIndexed = true)]
    public string Category { get; set; }

    // Tags is marked as indexed, since we want to filter using this property.
    [VectorStoreData(IsIndexed = true)]
    public List<string> Tags { get; set; }

    [VectorStoreData]
    public string Term { get; set; }

    [VectorStoreData(IsFullTextIndexed = true)]
    public string Definition { get; set; }

    [VectorStoreVector(1536)]
    public ReadOnlyMemory<float> DefinitionEmbedding { get; set; }
}
```

::: zone-end
::: zone pivot="programming-language-python"

## Coming soon

More information coming soon.

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More information coming soon.

::: zone-end
