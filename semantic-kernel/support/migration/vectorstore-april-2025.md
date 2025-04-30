---
title: Vector Store changes - April 2025
description: Describes the changes included in the April 2025 Vector Store release and how to migrate
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 03/06/2025
ms.service: semantic-kernel
---
::: zone pivot="programming-language-csharp"

# Vector Store changes - Preview 2 - April 2025

## Built-in Support for Embedding Generation in the Vector Store

The April 2025 update introduces built-in support for embedding generation directly within the vector store. By configuring an embedding generator, you can now automatically generate embeddings for vector properties without needing to precompute them externally. This feature simplifies workflows and reduces the need for additional preprocessing steps.

### Configuring an Embedding Generator

Embedding generators implementing the `Microsoft.Extensions.AI` abstractions are supported and can be configured at various levels:

1. **On the Vector Store**:
    You can set a default embedding generator for the entire vector store. This generator will be used for all collections and properties unless overridden.

    ```csharp
    using Microsoft.Extensions.AI;
    using Microsoft.SemanticKernel.Connectors.Qdrant;
    using OpenAI;
    using Qdrant.Client;
    
    var embeddingGenerator = new OpenAIClient("your key")
        .GetEmbeddingClient("your chosen model")
        .AsIEmbeddingGenerator();

    var vectorStore = new QdrantVectorStore(new QdrantClient("localhost"), new QdrantVectorStoreOptions
    {
         EmbeddingGenerator = embeddingGenerator
    });
    ```

2. **On a Collection**:
    You can configure an embedding generator for a specific collection, overriding the store-level generator.

    ```csharp
    using Microsoft.Extensions.AI;
    using Microsoft.SemanticKernel.Connectors.Qdrant;
    using OpenAI;
    using Qdrant.Client;
    
    var embeddingGenerator = new OpenAIClient("your key")
        .GetEmbeddingClient("your chosen model")
        .AsIEmbeddingGenerator();

    var collectionOptions = new QdrantVectorStoreRecordCollectionOptions<MyRecord>
    {
        EmbeddingGenerator = embeddingGenerator
    };
    var collection = new QdrantVectorStoreRecordCollection<ulong, MyRecord>(new QdrantClient("localhost"), "myCollection", collectionOptions);
    ```

3. **On a Record Definition**:
    When defining properties programmatically using `VectorStoreRecordDefinition`, you can specify an embedding generator for all properties.

    ```csharp
    using Microsoft.Extensions.AI;
    using Microsoft.Extensions.VectorData;
    using Microsoft.SemanticKernel.Connectors.Qdrant;
    using OpenAI;
    using Qdrant.Client;
    
    var embeddingGenerator = new OpenAIClient("your key")
        .GetEmbeddingClient("your chosen model")
        .AsIEmbeddingGenerator();

    var recordDefinition = new VectorStoreRecordDefinition
    {
        EmbeddingGenerator = embeddingGenerator,
        Properties = new List<VectorStoreRecordProperty>
        {
            new VectorStoreRecordKeyProperty("Key", typeof(ulong)),
            new VectorStoreRecordVectorProperty("DescriptionEmbedding", typeof(string), dimensions: 1536)
        }
    };

    var collectionOptions = new QdrantVectorStoreRecordCollectionOptions<MyRecord>
    {
        VectorStoreRecordDefinition = recordDefinition
    };
    var collection = new QdrantVectorStoreRecordCollection<ulong, MyRecord>(new QdrantClient("localhost"), "myCollection", collectionOptions);
    ```

4. **On a Vector Property Definition**:
    When defining properties programmatically, you can set an embedding generator directly on the property.

    ```csharp
    using Microsoft.Extensions.AI;
    using Microsoft.Extensions.VectorData;
    using OpenAI;
    
    var embeddingGenerator = new OpenAIClient("your key")
        .GetEmbeddingClient("your chosen model")
        .AsIEmbeddingGenerator();

    var vectorProperty = new VectorStoreRecordVectorProperty("DescriptionEmbedding", typeof(string), dimensions: 1536)
    {
         EmbeddingGenerator = embeddingGenerator
    };
    ```

### Example Usage

The following example demonstrates how to use the embedding generator to automatically generate vectors during both upsert and search operations. This approach simplifies workflows by eliminating the need to precompute embeddings manually.

```csharp

// The data model
internal class FinanceInfo
{
    [VectorStoreRecordKey]
    public string Key { get; set; } = string.Empty;

    [VectorStoreRecordData]
    public string Text { get; set; } = string.Empty;

    // Note that the vector property is typed as a string, and
    // its value is derived from the Text property. The string
    // value will however be converted to a vector on upsert and
    // stored in the database as a vector.
    [VectorStoreRecordVector(1536)]
    public string Embedding => this.Text;
}

// Create an OpenAI embedding generator.
var embeddingGenerator = new OpenAIClient("your key")
    .GetEmbeddingClient("your chosen model")
    .AsIEmbeddingGenerator();

// Use the embedding generator with the vector store.
var vectorStore = new InMemoryVectorStore(new() { EmbeddingGenerator = embeddingGenerator });
var collection = vectorStore.GetCollection<string, FinanceInfo>("finances");
await collection.CreateCollectionAsync();

// Create some test data.
string[] budgetInfo =
{
    "The budget for 2020 is EUR 100 000",
    "The budget for 2021 is EUR 120 000",
    "The budget for 2022 is EUR 150 000",
    "The budget for 2023 is EUR 200 000",
    "The budget for 2024 is EUR 364 000"
};
    
// Embeddings are generated automatically on upsert.
var records = budgetInfo.Select((input, index) => new FinanceInfo { Key = index.ToString(), Text = input });
await collection.UpsertAsync(records);

// Embeddings for the search is automatically generated on search.
var searchResult = collection.SearchAsync(
    "What is my budget for 2024?",
    top: 1);

// Output the matching result.
await foreach (var result in searchResult)
{
    Console.WriteLine($"Key: {result.Record.Key}, Text: {result.Record.Text}");
}
```

## Transition from `IVectorizableTextSearch` and `IVectorizedSearch` to `IVectorSearch`

The `IVectorizableTextSearch` and `IVectorizedSearch` interfaces have been marked as obsolete and replaced by the more unified and flexible `IVectorSearch` interface.
This change simplifies the API surface and provides a more consistent approach to vector search operations.

### Key Changes

1. **Unified Interface**:
    - The `IVectorSearch` interface consolidates the functionality of both `IVectorizableTextSearch` and `IVectorizedSearch` into a single interface.

2. **Method Renaming**:
    - `VectorizableTextSearchAsync` from `IVectorizableTextSearch` has been replaced by `SearchAsync` in `IVectorSearch`.
    - `VectorizedSearchAsync` from `IVectorizedSearch` has been replaced by `SearchEmbeddingAsync` in `IVectorSearch`.

3. **Improved Flexibility**:
    - The `SearchAsync` method in `IVectorSearch` handles embedding generation, supporting either local embedding, if an embedding generator is configured, or server side embedding.
    - The `SearchEmbeddingAsync` method in `IVectorSearch` allows for direct embedding-based searches, providing a low-level API for advanced use cases.

## Return type change for search methods

In addition to the change in naming for search methods, the return type for all search methods have been changed to simplify usage.
The result type of search methods is now `IAsyncEnumerable<VectorSearchResult<TRecord>>`, which allows for looping through the results
directly. Previously the returned object contained an IAsyncEnumerable property.

## Support for search without vectors / filtered get

The April 2025 update introduces support for finding records using a filter and returning the results with a configurable sort order.
This allows enumerating records in a predictable order, which is particularly useful when needing to sync the vector store with an external data source.

### Example: Using filtered `GetAsync`

The following example demonstrates how to use the `GetAsync` method with a filter and options to retrieve records from a vector store collection. This approach allows you to apply filtering criteria and sort the results in a predictable order.

```csharp
// Define a filter to retrieve products priced above $600
Expression<Func<ProductInfo, bool>> filter = product => product.Price > 600;

// Define the options with a sort order
var options = new GetFilteredRecordOptions<ProductInfo>();
options.OrderBy.Descending(product => product.Price);

// Use GetAsync with the filter and sort order
var filteredProducts = await collection.GetAsync(filter, top: 10, options)
    .ToListAsync();
```

This example demonstrates how to use the `GetAsync` method to retrieve filtered records and sort them based on specific criteria, such as price.

## New methods on IVectorStore

Some new methods are available on the `IVectorStore` interface that allow you to perform more operations directly without needing a VectorStoreRecordCollection object.

### Check if a Collection Exists

You can now verify whether a collection exists in the vector store without having to create a VectorStoreRecordCollection object.

```csharp
// Example: Check if a collection exists
bool exists = await vectorStore.CollectionExistsAsync("myCollection", cancellationToken);
if (exists)
{
    Console.WriteLine("The collection exists.");
}
else
{
    Console.WriteLine("The collection does not exist.");
}
```

### Delete a Collection

A new method allows you to delete a collection from the vector store without having to create a VectorStoreRecordCollection object.

```csharp
// Example: Delete a collection
await vectorStore.DeleteCollectionAsync("myCollection", cancellationToken);
Console.WriteLine("The collection has been deleted.");
```

## Change in Batch method naming convention

The `IVectorStoreRecordCollection` interface has been updated to improve consistency in method naming conventions.
Specifically, the batch methods have been renamed to remove the "Batch" part of their names. This change aligns with a more concise naming convention.

### Renaming Examples

- **Old Method:** `GetBatchAsync(IEnumerable<TKey> keys, ...)`  
    **New Method:** `GetAsync(IEnumerable<TKey> keys, ...)`

- **Old Method:** `DeleteBatchAsync(IEnumerable<TKey> keys, ...)`  
    **New Method:** `DeleteAsync(IEnumerable<TKey> keys, ...)`

- **Old Method:** `UpsertBatchAsync(IEnumerable<TRecord> records, ...)`  
    **New Method:** `UpsertAsync(IEnumerable<TRecord> records, ...)`

## Return type change for batch Upsert method

The return type for the batch upsert method has been changed from `IAsyncEnumerable<TKey>` to `Task<IReadOnlyList<TKey>>`.
This change impacts how the method is consumed. You can now simply await the result and get back a list of keys.
Previously, to ensure that all upserts were completed, the IAsyncEnumerable had to be completely enumerated.
This simplifies the developer experience when using the batch upsert method.

## The CollectionName property has been changed to Name

The `CollectionName` property on the `IVectorStoreRecordCollection` interface has renamed to `Name`.

## IsFilterable and IsFullTextSearchable renamed to IsIndexed and IsFullTextIndexed

The `IsFilterable` and `IsFullTextSearchable` properties on the `VectorStoreRecordDataAttribute` and `VectorStoreRecordDataProperty` classes have been renamed to
`IsIndexed` and `IsFullTextIndexed` respectively.

## Dimensions are now required for vector attributes and definitions

In the April 2025 update, specifying the number of dimensions has become mandatory when using vector attributes or vector property definitions. This ensures that the vector store always has the necessary information to handle embeddings correctly.

### Changes to `VectorStoreRecordVectorAttribute`

Previously, the `VectorStoreRecordVectorAttribute` allowed you to omit the `Dimensions` parameter. This is no longer allowed, and the `Dimensions` parameter must now be explicitly provided.

**Before:**

```csharp
[VectorStoreRecordVector]
public ReadOnlyMemory<float> DefinitionEmbedding { get; set; }
```

**After:**

```csharp
[VectorStoreRecordVector(Dimensions: 1536)]
public ReadOnlyMemory<float> DefinitionEmbedding { get; set; }
```

### Changes to `VectorStoreRecordVectorProperty`

Similarly, when defining a vector property programmatically using `VectorStoreRecordVectorProperty`, the `dimensions` parameter is now required.

**Before:**

```csharp
var vectorProperty = new VectorStoreRecordVectorProperty("DefinitionEmbedding", typeof(ReadOnlyMemory<float>));
```

**After:**

```csharp
var vectorProperty = new VectorStoreRecordVectorProperty("DefinitionEmbedding", typeof(ReadOnlyMemory<float>), dimensions: 1536);
```

## All collections require the key type to be passed as a generic type parameter

When constructing a collection directly, it is now required to provide the `TKey` generic type parameter.
Previously, where some databases allowed only one key type, it was now a required parameter, but to allow using
collections with a `Dictionary<string, object?>` type and an `object` key type, `TKey` must now always be provided.

::: zone-end
::: zone pivot="programming-language-python"

## Not Applicable

These changes are currently only applicable in C#

::: zone-end
::: zone pivot="programming-language-java"

## Not Applicable

These changes are currently only applicable in C#

::: zone-end
