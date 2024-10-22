---
title: Vector seearch using Semantic Kernel Vector Store connectors (Preview)
description: Describes the different options you can use when doing a vector search using Semantic Kernel vector store connectors.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 09/23/2024
ms.service: semantic-kernel
---
# Vector search using Semantic Kernel Vector Store connectors (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

Semantic Kernel provides vector search capabilities as part of its Vector Store abstractions. This supports filtering and many other options, which this article will explain in more detail.

::: zone pivot="programming-language-csharp"

## Vector Search

The `VectorizedSearchAsync` method allows searching using data that has already been vectorized. This method takes a vector and an optional `VectorSearchOptions` class as input.
This method is available on the following interfaces:

1. `IVectorizedSearch<TRecord>`
2. `IVectorStoreRecordCollection<TKey, TRecord>`

Note that `IVectorStoreRecordCollection<TKey, TRecord>` inherits from `IVectorizedSearch<TRecord>`.

Assuming you have a collection that already contains data, you can easily search it. Here is an example using Qdrant.

```csharp
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.Extensions.VectorData;
using Qdrant.Client;

// Create a Qdrant VectorStore object and choose an existing collection that already contains records.
IVectorStore vectorStore = new QdrantVectorStore(new QdrantClient("localhost"));
IVectorStoreRecordCollection<ulong, Hotel> collection = vectorStore.GetCollection<ulong, Hotel>("skhotels");

// Generate a vector for your search text, using your chosen embedding generation implementation.
// Just showing a placeholder method here for brevity.
var searchVector = await GenerateEmbeddingAsync("I'm looking for a hotel where customer happiness is the priority.");

// Do the search, passing an options object with a Top value to limit resulst to the single top match.
var searchResult = await collection.VectorizedSearchAsync(searchVector, new() { Top = 1 }).Results.ToListAsync();

// Inspect the returned hotel.
Hotel hotel = searchResult.First().Record;
Console.WriteLine("Found hotel description: " + hotel.Description);
```

> [!TIP]
> For more information on how to generate embeddings see [embedding generation](./embedding-generation.md).

## Supported Vector Types

`VectorizedSearchAsync` takes a generic type as the vector parameter.
The types of vectors supported y each data store vary.
See [the documentation for each connector](./out-of-the-box-connectors/index.md) for the list of supported vector types.

It is also important for the search vector type to match the target vector that is being searched, e.g. if you have two vectors
on the same record with different vector types, make sure that the search vector you supply matches the type of the specific vector
you are targeting.
See [VectorPropertyName](#vectorpropertyname) for how to pick a target vector if you have more than one per record.

## Vector Search Options

The following options can be provided using the `VectorSearchOptions` class.

### VectorPropertyName

The `VectorPropertyName` option can be used to specify the name of the vector property to target during the search.
If none is provided, the first vector found on the data model or specified in the record definition will be used.

Note that when specifying the `VectorPropertyName`, use the name of the property as defined on the data model or in the record definition.
Use this property name even if the property may be stored under a different name in the vector store. The storage name may e.g. be different
because of custom serialization settings.

```csharp
using Microsoft.Extensions.VectorData;
using Microsoft.Connectors.Memory.InMemory;

var vectorStore = new InMemoryVectorStore();
var collection = vectorStore.GetCollection<int, Product>("skproducts");

// Create the vector search options and indicate that we want to search the FeatureListEmbedding property.
var vectorSearchOptions = new VectorSearchOptions
{
    VectorPropertyName = nameof(Product.FeatureListEmbedding)
};

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var searchResult = await collection.VectorizedSearchAsync(searchVector, vectorSearchOptions).Results.ToListAsync();

public sealed class Product
{
    [VectorStoreRecordKey]
    public int Key { get; set; }

    [VectorStoreRecordData]
    public string Description { get; set; }

    [VectorStoreRecordData]
    public List<string> FeatureList { get; set; }

    [VectorStoreRecordVector(1536)]
    public ReadOnlyMemory<float> DescriptionEmbedding { get; set; }

    [VectorStoreRecordVector(1536)]
    public ReadOnlyMemory<float> FeatureListEmbedding { get; set; }
}
```

### Top and Skip

The `Top` and `Skip` options allow you to limit the number of results to the Top n results and
to skip a number of results from the top of the resultset.
Top and Skip can be used to do paging if you wish to retrieve a large number of results using separate calls.

```csharp
// Create the vector search options and indicate that we want to skip the first 40 results and then get the next 20.
var vectorSearchOptions = new VectorSearchOptions
{
    Top = 20,
    Skip = 40
};

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var searchResult = await collection.VectorizedSearchAsync(searchVector, vectorSearchOptions).Results.ToListAsync();
```

The default values for `Top` is 3 and `Skip` is 0.

### IncludeVectors

The `IncludeVectors` option allows you to specify whether you wish to return vectors in the search results.
If `false`, the vector properties on the returned model will be left null.
Using `false` can significantly reduce the amount of data retrieved from the vector store during search,
making searches more efficient.

The default value for `IncludeVectors` is `false`.

```csharp
// Create the vector search options and indicate that we want to include vectors in the search results.
var vectorSearchOptions = new VectorSearchOptions
{
    IncludeVectors = true
}
// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var searchResult = await collection.VectorizedSearchAsync(searchVector, vectorSearchOptions).Results.ToListAsync()
```

### VectorSearchFilter

The `VectorSearchFilter` option can be used to provide a filter for filtering the records in the chosen collection
before applying the vector search.

This has multiple benefits:

- Reduce latency and processing cost, since only records remaining after filtering need to be compared with the search vector and therefore fewer vector comparisons have to be done.
- Limit the resultset for e.g. access control purposes, by excluding data that the user shouldn't have access to.

Note that in order for fields to be used for filtering, many vector stores require those fields to be indexed first.
Some vector stores will allow filtering using any field, but may optionally allow indexing to improve filtering performance.

If creating a collection via the Semantic Kernel vector store abstractions and you wish to enable filtering on a field,
set the `IsFilterable` property to true when defining your data model or when creating your record definition.

> [!TIP]
> For more information on how to set the `IsFilterable` property, refer to [VectorStoreRecordDataAttribute parameters](./defining-your-data-model.md#vectorstorerecorddataattribute-parameters) or [VectorStoreRecordDataProperty configuration settings](./schema-with-record-definition.md#vectorstorerecorddataproperty-configuration-settings).

To create a filter use the `VectorSearchFilter` class. You can combine multiple filter clauses together in one `VectorSearchFilter`.
All filter clauses are combined with `and`.
Note that when providing a property name when constructing the filter, use the name of the property as defined on the data model or in the record definition.
Use this property name even if the property may be stored under a different name in the vector store. The storage name may e.g. be different
because of custom serialization settings.

```csharp
// Filter where Category == 'External Definitions' and Tags contain 'memory'.
var filter = new VectorSearchFilter()
    .EqualTo(nameof(Glossary.Category), "External Definitions")
    .AnyTagEqualTo(nameof(Glossary.Tags), "memory");

// Create the vector search options and set the filter on the options.
var vectorSearchOptions = new VectorSearchOptions
{
    Filter = filter
};

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
searchResult = await collection.VectorizedSearchAsync(searchVector, vectorSearchOptions).Results.ToListAsync();

private sealed class Glossary
{
    [VectorStoreRecordKey]
    public ulong Key { get; set; }

    // Category is marked as filterable, since we want to filter using this property.
    [VectorStoreRecordData(IsFilterable = true)]
    public string Category { get; set; }

    // Tags is marked as filterable, since we want to filter using this property.
    [VectorStoreRecordData(IsFilterable = true)]
    public List<string> Tags { get; set; }

    [VectorStoreRecordData]
    public string Term { get; set; }

    [VectorStoreRecordData]
    public string Definition { get; set; }

    [VectorStoreRecordVector(1536)]
    public ReadOnlyMemory<float> DefinitionEmbedding { get; set; }
}
```

#### EqualTo filter clause

Use `EqualTo` for a direct comparison between property and value.

#### AnyTagEqualTo filter clause

Use `AnyTagEqualTo` to check if any of the strings, stored in a tag property in the vector store, contains a provided value.
For a property to be considered a tag property, it needs to be a List, array or other enumerable of string.

::: zone-end
::: zone pivot="programming-language-python"

## Coming soon

More info coming soon.

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end
