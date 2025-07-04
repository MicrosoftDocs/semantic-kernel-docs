---
title: Vector search using Semantic Kernel Vector Store connectors (Preview)
description: Describes the different options you can use when doing a vector search using Semantic Kernel vector store connectors.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 09/23/2024
ms.service: semantic-kernel
---
# Vector search using Semantic Kernel Vector Store connectors (Preview)

::: zone pivot="programming-language-csharp"

::: zone-end
::: zone pivot="programming-language-python"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end
::: zone pivot="programming-language-java"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end

Semantic Kernel provides vector search capabilities as part of its Vector Store abstractions. This supports filtering and many other options, which this article will explain in more detail.

::: zone pivot="programming-language-csharp"

> [!TIP]
> To see how you can search without generating embeddings yourself, see [Letting the Vector Store generate embeddings](./embedding-generation.md#letting-the-vector-store-generate-embeddings).

## Vector Search

The `SearchAsync` method allows searching using data that has already been vectorized. This method takes a vector and an optional `VectorSearchOptions<TRecord>` class as input.
This method is available on the following types:

1. `IVectorSearchable<TRecord>`
2. `VectorStoreCollection<TKey, TRecord>`

Note that `VectorStoreCollection<TKey, TRecord>` implements from `IVectorSearchable<TRecord>`.

Assuming you have a collection that already contains data, you can easily search it. Here is an example using Qdrant.

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
VectorStoreCollection<ulong, Hotel> collection = vectorStore.GetCollection<ulong, Hotel>("skhotels");

// Generate a vector for your search text, using your chosen embedding generation implementation.
ReadOnlyMemory<float> searchVector = await GenerateEmbeddingAsync("I'm looking for a hotel where customer happiness is the priority.");

// Do the search, passing an options object with a Top value to limit results to the single top match.
var searchResult = collection.SearchAsync(searchVector, top: 1);

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

`SearchAsync` takes a generic type as the vector parameter.
The types of vectors supported by each data store vary.
See [the documentation for each connector](./out-of-the-box-connectors/index.md) for the list of supported vector types.

It is also important for the search vector type to match the target vector that is being searched, e.g. if you have two vectors
on the same record with different vector types, make sure that the search vector you supply matches the type of the specific vector
you are targeting.
See [VectorProperty](#vectorproperty) for how to pick a target vector if you have more than one per record.

## Vector Search Options

The following options can be provided using the `VectorSearchOptions<TRecord>` class.

### VectorProperty

The `VectorProperty` option can be used to specify the vector property to target during the search.
If none is provided and the data model contains only one vector, that vector will be used.
If the data model contains no vector or multiple vectors and `VectorProperty` is not provided, the search method will throw.

```csharp
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;

var vectorStore = new InMemoryVectorStore();
var collection = vectorStore.GetCollection<int, Product>("skproducts");

// Create the vector search options and indicate that we want to search the FeatureListEmbedding property.
var vectorSearchOptions = new VectorSearchOptions<Product>
{
    VectorProperty = r => r.FeatureListEmbedding
};

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var searchResult = collection.SearchAsync(searchVector, top: 3, vectorSearchOptions);

public sealed class Product
{
    [VectorStoreKey]
    public int Key { get; set; }

    [VectorStoreData]
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
// Create the vector search options and indicate that we want to skip the first 40 results.
var vectorSearchOptions = new VectorSearchOptions<Product>
{
    Skip = 40
};

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
// Here we pass top: 20 to indicate that we want to retrieve the next 20 results after skipping
// the first 40
var searchResult = collection.SearchAsync(searchVector, top: 20, vectorSearchOptions);

// Iterate over the search results.
await foreach (var result in searchResult)
{
    Console.WriteLine(result.Record.FeatureList);
}
```

The default value `Skip` is 0.

### IncludeVectors

The `IncludeVectors` option allows you to specify whether you wish to return vectors in the search results.
If `false`, the vector properties on the returned model will be left null.
Using `false` can significantly reduce the amount of data retrieved from the vector store during search,
making searches more efficient.

The default value for `IncludeVectors` is `false`.

```csharp
// Create the vector search options and indicate that we want to include vectors in the search results.
var vectorSearchOptions = new VectorSearchOptions<Product>
{
    IncludeVectors = true
};

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var searchResult = collection.SearchAsync(searchVector, top: 3, vectorSearchOptions);

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
> For more information on how to set the `IsFilterable` property, refer to [VectorStoreDataAttribute parameters](./defining-your-data-model.md#vectorstoredataattribute-parameters) or [VectorStoreDataProperty configuration settings](./schema-with-record-definition.md#vectorstoredataproperty-configuration-settings).

Filters are expressed using LINQ expressions based on the type of the data model.
The set of LINQ expressions supported will vary depending on the functionality supported
by each database, but all databases support a broad base of common expressions, e.g. equals,
not equals, and, or, etc.

```csharp
// Create the vector search options and set the filter on the options.
var vectorSearchOptions = new VectorSearchOptions<Glossary>
{
    Filter = r => r.Category == "External Definitions" && r.Tags.Contains("memory")
};

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var searchResult = collection.SearchAsync(searchVector, top: 3, vectorSearchOptions);

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

    [VectorStoreData]
    public string Definition { get; set; }

    [VectorStoreVector(1536)]
    public ReadOnlyMemory<float> DefinitionEmbedding { get; set; }
}
```

::: zone-end
::: zone pivot="programming-language-python"

## Vector Search

There are two searches currently supported in the Semantic Kernel Vector Store abstractions:
1. `search` (vector search)
   1. This is search based on a value that can be vectorized, by the `embedding_generator` field on the data model or record definition, or by the vector store itself. Or by directly supplying a vector.
2. `hybrid_search` -> see [Hybrid Search](./hybrid-search.md)

All searches can take a optional set of parameters:
- `vector`: A vector used to search, can be supplied instead of the values, or in addition to the values for hybrid.
- `top`: The number of results to return, defaults to 3.
- `skip`: The number of results to skip, defaults to 0.
- `include_vectors`: Whether to include the vectors in the results, defaults to `false`.
- `filter`: A filter to apply to the results before the vector search is applied, defaults to `None`, in the form of a lambda expression: `lambda record: record.property == "value"`.
- `vector_property_name`: The name of the vector property to use for the search, defaults to the first vector property found on the data model or record definition.
- `include_total_count`: Whether to include the total count of results in the search result, defaults to `false`.

Assuming you have a collection that already contains data, you can easily search it. Here is an example using Azure AI Search.

```python
from semantic_kernel.connectors.azure_ai_search import AzureAISearchCollection, AzureAISearchStore

# Create a Azure AI Search VectorStore object and choose an existing collection that already contains records.
# Hotels is the data model decorated class.
store = AzureAISearchStore()
collection: AzureAISearchCollection[str, Hotels] = store.get_collection(Hotels, collection_name="skhotels")

search_results = await collection.search(
    query, vector_property_name="vector"
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

## Vector Search

The `searchAsync` method allows searching using data that has already been vectorized. This method takes a vector and an optional `VectorSearchOptions` class as input.
This method is available on the following interfaces:

1. `VectorizedSearch<Record>`
2. `VectorStoreRecordCollection<Key, Record>`

Note that `VectorStoreRecordCollection<Key, Record>` inherits from `VectorizedSearch<Record>`.

Assuming you have a collection that already contains data, you can easily search it. Here is an example using JDBC with PostgreSQL.

```java
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStore;
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStoreOptions;
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStoreRecordCollectionOptions;
import com.microsoft.semantickernel.data.jdbc.postgres.PostgreSQLVectorStoreQueryProvider;
import com.microsoft.semantickernel.data.vectorstorage.options.VectorSearchOptions;
import org.postgresql.ds.PGSimpleDataSource;

import java.util.List;

public class Main {
    public static void main(String[] args) {
        // Configure the data source
        PGSimpleDataSource dataSource = new PGSimpleDataSource();
        dataSource.setUrl("jdbc:postgresql://localhost:5432/sk");
        dataSource.setUser("postgres");
        dataSource.setPassword("root");

        // Create a JDBC vector store and choose an existing collection that already contains records.
        var vectorStore = new JDBCVectorStore(dataSource, JDBCVectorStoreOptions.builder()
                .withQueryProvider(PostgreSQLVectorStoreQueryProvider.builder()
                        .withDataSource(dataSource)
                        .build())
                .build());
        var collection = vectorStore.getCollection("skhotels", JDBCVectorStoreRecordCollectionOptions.<Hotel>builder()
                .withRecordClass(Hotel.class)
                .build());

        // Generate a vector for your search text, using your chosen embedding generation implementation.
        // Just showing a placeholder method here for brevity.
        var searchVector = generateEmbeddingsAsync("I'm looking for a hotel where customer happiness is the priority.").block();

        // Do the search, passing an options object with a Top value to limit results to the single top match.
        var searchResult = collection.searchAsync(searchVector, VectorSearchOptions.builder()
                .withTop(1).build()
        ).block();

        // Inspect the returned hotel.
        Hotel hotel = searchResult.getResults().get(0).getRecord();
        System.out.printf("Found hotel description: %s\n", hotel.getDescription());
    }
}
```

> [!TIP]
> For more information on how to generate embeddings see [embedding generation](./embedding-generation.md).

## Vector Search Options

The following options can be provided using the `VectorSearchOptions` class.

### VectorFieldName

The `VectorFieldName` option can be used to specify the name of the vector field to target during the search.
If none is provided, the first vector found on the data model or specified in the record definition will be used.

Note that when specifying the `VectorFieldName`, use the name of the field as defined on the data model or in the record definition.
Use this field name even if the field may be stored under a different name in the vector store. The storage name may e.g. be different
because of custom serialization settings.

```java
import com.microsoft.semantickernel.data.VolatileVectorStore;
import com.microsoft.semantickernel.data.VolatileVectorStoreRecordCollectionOptions;
import com.microsoft.semantickernel.data.vectorstorage.annotations.VectorStoreRecordData;
import com.microsoft.semantickernel.data.vectorstorage.annotations.VectorStoreRecordKey;
import com.microsoft.semantickernel.data.vectorstorage.annotations.VectorStoreRecordVector;
import com.microsoft.semantickernel.data.vectorstorage.options.VectorSearchOptions;

import java.util.List;

public class Main {
    public static void main(String[] args) {
        // Build a query provider
        var vectorStore = new VolatileVectorStore();
        var collection = vectorStore.getCollection("skproducts", VolatileVectorStoreRecordCollectionOptions.<Product>builder()
                .withRecordClass(Product.class)
                .build());

        // Create the vector search options and indicate that we want to search the FeatureListEmbedding field.
        var searchOptions = VectorSearchOptions.builder()
                .withVectorFieldName("featureListEmbedding")
                .build();

        // Generate a vector for your search text, using the embedding model of your choice
        var searchVector = generateEmbeddingsAsync().block();

        // Do the search
        var searchResult = collection.searchAsync(searchVector, searchOptions).block();
    }

    public static class Product {
        @VectorStoreRecordKey
        private int key;

        @VectorStoreRecordData
        private String description;

        @VectorStoreRecordData
        private List<String> featureList;

        @VectorStoreRecordVector(dimensions = 1536)
        public List<Float> descriptionEmbedding;

        @VectorStoreRecordVector(dimensions = 1536)
        public List<Float> featureListEmbedding;

        public Product() {
        }

        public Product(int key, String description, List<String> featureList, List<Float> descriptionEmbedding, List<Float> featureListEmbedding) {
            this.key = key;
            this.description = description;
            this.featureList = featureList;
            this.descriptionEmbedding = Collections.unmodifiableList(descriptionEmbedding);
            this.featureListEmbedding = Collections.unmodifiableList(featureListEmbedding);
        }

        public int getKey() { return key; }
        public String getDescription() { return description; }
        public List<String> getFeatureList() { return featureList; }
        public List<Float> getDescriptionEmbedding() { return descriptionEmbedding; }
        public List<Float> getFeatureListEmbedding() { return featureListEmbedding; }
    }
}
```

### Top and Skip

The `Top` and `Skip` options allow you to limit the number of results to the Top n results and
to skip a number of results from the top of the resultset.
Top and Skip can be used to do paging if you wish to retrieve a large number of results using separate calls.

```java
// Create the vector search options and indicate that we want to skip the first 40 results and then get the next 20.
var searchOptions = VectorSearchOptions.builder()
        .withTop(20)
        .withSkip(40)
        .build();

// Generate a vector for your search text, using the embedding model of your choice
var searchVector = generateEmbeddingsAsync().block();

// Do the search
var searchResult = collection.searchAsync(searchVector, searchOptions).block();
```

The default values for `Top` is 3 and `Skip` is 0.

### IncludeVectors

The `IncludeVectors` option allows you to specify whether you wish to return vectors in the search results.
If `false`, the vector properties on the returned model will be left null.
Using `false` can significantly reduce the amount of data retrieved from the vector store during search,
making searches more efficient.

The default value for `IncludeVectors` is `false`.

```java
// Create the vector search options and indicate that we want to include vectors in the search results.
var searchOptions = VectorSearchOptions.builder()
        .withIncludeVectors(true)
        .build();

// Generate a vector for your search text, using the embedding model of your choice
var searchVector = generateEmbeddingsAsync().block();

// Do the search
var searchResult = collection.searchAsync(searchVector, searchOptions).block();
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
set the `IsFilterable` field to true when defining your data model or when creating your record definition.

> [!TIP]
> For more information on how to set the `IsFilterable` field, refer to [VectorStoreRecordData parameters](./defining-your-data-model.md#vectorstorerecorddata-parameters) or [VectorStoreRecordDataField configuration settings](./schema-with-record-definition.md#vectorstorerecorddatafield-configuration-settings).

To create a filter use the `VectorSearchFilter` class. You can combine multiple filter clauses together in one `VectorSearchFilter`.
All filter clauses are combined with `and`.
Note that when providing a field name when constructing the filter, use the name of the field as defined on the data model or in the record definition.
Use this field name even if the field may be stored under a different name in the vector store. The storage name may e.g. be different
because of custom serialization settings.

```java
// Filter where category == 'External Definitions' and tags contain 'memory'.
var filter = VectorSearchFilter.builder()
        .equalTo("category", "External Definitions")
        .anyTagEqualTo("tags", "memory")
        .build();

// Create the vector search options and indicate that we want to filter the search results by a specific field.
var searchOptions = VectorSearchOptions.builder()
        .withVectorSearchFilter(filter)
        .build();

// Generate a vector for your search text, using the embedding model of your choice
var searchVector = generateEmbeddingsAsync().block();

// Do the search
var searchResult = collection.searchAsync(searchVector, searchOptions).block();


public static class Glossary {
    @VectorStoreRecordKey
    private String key;

    @VectorStoreRecordData(isFilterable = true)
    private String category;

    @VectorStoreRecordData(isFilterable = true)
    private List<String> tags;

    @VectorStoreRecordData
    private String term;

    @VectorStoreRecordData
    private String definition;

    @VectorStoreRecordVector(dimensions = 1536)
    private List<Float> definitionEmbedding;

    public Glossary() {
    }

    public Glossary(String key, String category, List<String> tags, String term, String definition, List<Float> definitionEmbedding) {
        this.key = key;
        this.category = category;
        this.tags = tags;
        this.term = term;
        this.definition = definition;
        this.definitionEmbedding = Collections.unmodifiableList(definitionEmbedding);
    }

    public String getKey() { return key; }
    public String getCategory() { return category; }
    public List<String> getTags() { return tags; }
    public String getTerm() { return term; }
    public String getDefinition() { return definition; }
    public List<Float> getDefinitionEmbedding() { return definitionEmbedding; }
}
```

#### EqualTo filter clause

Use `equalTo` for a direct comparison between field and value.

#### AnyTagEqualTo filter clause

Use `anyTagEqualTo` to check if any of the strings, stored in a tag field in the vector store, contains a provided value.
For a field to be considered a tag field, it needs to be a `List<String>`.

::: zone-end
