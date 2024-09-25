---
title: Vector seearch using Semantic Kernel Vector Store connectors (Experimental)
description: Describes the different options you can use when doing a vector search using Semantic Kernel vector store connectors.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 23/09/2024
ms.service: semantic-kernel
---
# Vector seearch using Semantic Kernel Vector Store connectors (Experimental)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is experimental, still in development and is subject to change.

Semantic Kernel provides vector search capabilities as part of its Vector Store abstractions. This supports filtering and many other options, which this article will explain in more detail.

## Vector Search

The `VectorizedSearchAsync` method allows searching using data that has already been vectorized. This method takes a vector and an optional `VectorSearchOptions` class as input.
This method is available on `IVectorizedSearch<TRecord>` interface. `IVectorStoreRecordCollection<TKey, TRecord>` inherits from `IVectorizedSearch<TRecord>`, so also includes this method.

Assuming you have a collection that already contains data, you can easily search it. Here is an example using Qdrant.

```csharp
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

// Create a Qdrant VectorStore object and choose an existing collection that already contains records.
var vectorStore = new QdrantVectorStore(new QdrantClient("localhost"));
var collection = vectorStore.GetCollection<ulong, Hotel>("skhotels");

// Generate a vector for your search text, using your chosen embedding generation implementation.
// Just showing a placeholder method here for brevity.
var searchVector = await GenerateEmbeddingAsync("I'm looking for a hotel where customer happiness is the priority.");

// Do the search, passing an options object with a Top value to limit resulst to the single top match.
var searchResult = await collection.VectorizedSearchAsync(searchVector, new() { Top = 1 }).ToListAsync()

// Inspect the returned hotel.
Hotel hotel = searchResult.First().Record;
Console.WriteLine("Found hotel description: " + hotel.Description);
```

> [!TIP]
> For more information on how to generate embeddings see [embedding generation.](./embedding-generation.md).

## Supported Vector Types

`VectorizedSearchAsync` takes a generic type as the vector parameter.
The types of vectors supported vary by each data store.
See [the documentation for each connector](./out-of-the-box-connectors/index.md) for the list of supported vector types.

It is also important for the search vector type to match the target vector that is being searched, e.g. if you have two vectors
on the same record with different vector types.
See [VectorPropertyName](#vectorpropertyname) for how to to pick a target vector if you have more than one per record.

## Vector Search Options

The following options can be provided using the `VectorSearchOptions` class.

### VectorPropertyName

The `VectorPropertyName` option can be used to provide the name of the specific vector property to target during the search.
If none is provided, the first vector found on the data model or specified in the record definition will be used.

Note that when specifying the `VectorPropertyName`, use the name of the property as defined on the data model or in the record definition.
Use this property name even if the property may be stored under a different name in the vector store because of custom serialization settings or any similar mechanism.

```csharp
using Microsoft.SemanticKernel.Data;

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

var vectorStore = new VolatileVectorStore();
var collection = vectorStore.GetCollection<int, Hotel>("skproducts");

// Create the vector search options and indicate that we want to search the FeatureListEmbedding property.
var vectorSearchOptions = new VectorSearchOptions
{
    VectorPropertyName = nameof(Product.FeatureListEmbedding)
}
// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var searchResult = await collection.VectorizedSearchAsync(searchVector, vectorSearchOptions).ToListAsync()
```

### Top and Skip

The `Top` and `Skip` options allow you to limit the number of results to the Top n results and
to skip a number of results from the start of the resultset.
This can be used to do paging if you wish to retrieve a large number of results using separate calls.

```csharp
// Create the vector search options and indicate that we want to skip the first 40 results and then get the next 20.
var vectorSearchOptions = new VectorSearchOptions
{
    Top = 20,
    Skip = 40
}
// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var searchResult = await collection.VectorizedSearchAsync(searchVector, vectorSearchOptions).ToListAsync()
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
var searchResult = await collection.VectorizedSearchAsync(searchVector, vectorSearchOptions).ToListAsync()
```

### VectorSearchFilter

The `VectorSearchFilter` option can be used to provide a filter for filtering the records in the chosen collection
before applying the vector search.

This has multiple benefits:

- Reduce latency and processing cost, since only remaining records have to be compared with the search vector and therefore fewer vector comparisons have to be done.
- Limit the resultset for e.g. access control purposes, by excluding data that the user shouldn't have access to.

Note that many vector stores require fields to be indexed for these to be used for filtering.
In some other cases filtering performance may be optionally improved by indexing these fields.

If creating a collection via the Semantic Kernel vector store abstractions and you wish to enable filtering on a field,
set the `IsFilterable` property to true when defining your data model or when creating your record definition.

> [!TIP]
> For more information on how to set the `IsFilterable` property, refer to [VectorStoreRecordDataAttribute parameters](./defining-your-data-model.md#vectorstorerecorddataattribute-parameters) or [VectorStoreRecordDataProperty configuration settings](./schema-with-record-definition.md#vectorstorerecorddataproperty-configuration-settings).

To create a filter use the `VectorSearchFilter` class. You can combine multiple filter clauses together in one `VectorSearchFilter`.
All filter clauses are combined with `and`.
Note that when providing a property name when constructing the filter, use the name of the property as defined on the data model or in the record definition.
Use this property name even if the property may be stored under a different name in the vector store because of custom serialization settings or any similar mechanism.

```csharp

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

// Filter where Category == 'External Definitions' and Tags contain 'memory'.
var filter = new VectorSearchFilter()
    .EqualTo(nameof(Glossary.Category), "External Definitions")
    .AnyTagEqualTo(nameof(Glossary.Tags), "memory");

// Create the vector search options and set the filter on the options.
var vectorSearchOptions = new VectorSearchOptions
{
    Filter = filter
}

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
searchResult = await collection.VectorizedSearchAsync(searchVector, vectorSearchOptions).ToListAsync();
```

#### EqualTo filter clause

Use `EqualTo` for a direct comparison between property and value.

#### AnyTagEqualTo filter clause

Use `AnyTagEqualTo` to check if any of the strings, stored in a tag property in the vector store, contains a provided value.
For a property to be considered a tag property, it needs to be a List, array or other enumerable of string.
