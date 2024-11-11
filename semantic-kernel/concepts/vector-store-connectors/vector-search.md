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

## Vector Search

There are three searches currently supported in the Semantic Kernel Vector Store abstractions:
1. `vectorized_search`
   1. This is search based on a vector created in your code, the vector is passed in and used to search.
2. `vectorizable_text_search`
   1. This is search based on a text string that is vectorized by the vector store as part of the search, this is not always supported and often requires a specific setup of either the vector store or the index.
3. `vector_text_search`
   1. This is text search directly against the vector store, most stores support this and depending on the store it can be as simple as comparing values or more advanced keyword search.

All searches can take a optional `VectorSearchOptions` instance as input. Each of the three searches have a Mixin class that needs to be part of it to surface the search methods and this always should be combined with the `VectorSearchBase[TKey, TRecord]` parent. 

Note that `VectorSearchBase` inherits from `VectorStoreRecordCollection`, as it uses some of the same methods, for instance for serialization and deserialization.

Assuming you have a collection that already contains data, you can easily search it. Here is an example using Azure AI Search.

```python
from semantic_kernel.connectors.memory.azure_ai_search import AzureAISearchCollection, AzureAISearchStore
from semantic_kernel.data.vector_search import VectorSearchOptions

# Create a Azure AI Search VectorStore object and choose an existing collection that already contains records.
# Hotels is the data model decorated class.
store = AzureAISearchStore()
collection: AzureAISearchCollection = store.get_collection("skhotels", Hotels)

# Generate a vector for your search text.
# Just showing a placeholder method here for brevity.
vector = await generate_vector("I'm looking for a hotel where customer happiness is the priority.")

search_results = await collection.vectorized_search(
    vector=vector, options=VectorSearchOptions(vector_field_name="vector")
)
hotels = [record.record async for record in search_results.results]
print(f"Found hotels: {hotels}")
```

> [!TIP]
> For more information on how to generate embeddings see [embedding generation](./embedding-generation.md).

## Vector Search Options

The following options can be provided using the `VectorSearchOptions` class.

### Vector Field Name

The `vector_field_name` option can be used to specify the name of the vector property to target during the search.
If none is provided, the first vector found on the data model or specified in the record definition will be used.

Note that when specifying the `vector_field_name`, use the name of the property as defined on the data model or in the record definition.
Use this property name even if the property may be stored under a different name in the vector store. The storage name may e.g. be different
because of custom serialization settings.

```Python

from semantic_kernel.data.vector_search import VectorSearchOptions
from semantic_kernel.connectors.memory.in_memory import InMemoryVectorStore

vector_store = InMemoryVectorStore()
collection = vector_store.get_collection("skproducts", Product)

# Create the vector search options and indicate that we want to search the FeatureListEmbedding property.
vector_search_options = VectorSearchOptions(vector_field_name="feature_list_embedding")

# This snippet assumes search_vector is already provided, having been created using the embedding model of your choice.
search_result = await collection.vectorized_search(vector=search_vector, options=vector_search_options)
products = [record async for record in search_result.results]

```

### Top and Skip

The `top` and `skip` options allow you to limit the number of results to the Top n results and
to skip a number of results from the top of the resultset.
Top and Skip can be used to do paging if you wish to retrieve a large number of results using separate calls.

```python
# Create the vector search options and indicate that we want to skip the first 40 results and then get the next 20.
vector_search_options = VectorSearchOptions(top=20, skip=40)

# This snippet assumes `vector` is already provided, having been created using the embedding model of your choice.
search_result = await collection.vectorized_search(vector=vector, options=vector_search_options)
async for result in search_result.results:
    print(result.record)
```

The default values for `top` is 3 and `skip` is 0.

### Include Vectors

The `include_vectors` option allows you to specify whether you wish to return vectors in the search results.
If `false`, the vector properties on the returned model will be left null.
Using `false` can significantly reduce the amount of data retrieved from the vector store during search,
making searches more efficient.

The default value for `include_vectors` is `false`.

> [!TIP]
> Make sure your data model allows the vector fields to be None! If not and you keep this to the default it might raise an error.

### Filter

The `filter` option can be used to provide a filter for filtering the records in the chosen collection
before applying the vector search.

This has multiple benefits:

- Reduce latency and processing cost, since only records remaining after filtering need to be compared with the search vector and therefore fewer vector comparisons have to be done.
- Limit the resultset for e.g. access control purposes, by excluding data that the user shouldn't have access to.

Note that in order for fields to be used for filtering, many vector stores require those fields to be indexed first.
Some vector stores will allow filtering using any field, but may optionally allow indexing to improve filtering performance.

If creating a collection via the Semantic Kernel vector store abstractions and you wish to enable filtering on a field,
set the `is_filterable` property to true when defining your data model or when creating your record definition.

> [!TIP]
> For more information on how to set the `is_filterable` property, refer to [VectorStoreRecordDataAttribute parameters](./defining-your-data-model.md#vectorstorerecorddataattribute-parameters) or [VectorStoreRecordDataProperty configuration settings](./schema-with-record-definition.md#vectorstorerecorddataproperty-configuration-settings).

To create a filter use the `VectorSearchFilter` class. You can combine multiple filter clauses together in one `VectorSearchFilter`.
All filter clauses are combined with `and`.
Note that when providing a property name when constructing the filter, use the name of the property as defined on the data model or in the record definition.
Use this property name even if the property may be stored under a different name in the vector store. The storage name may e.g. be different
because of custom serialization settings.

```python
# Filter where category == 'External Definitions' and tags contain 'memory'.
filter = VectorSearchFilter.equal_to('category', "External Definitions").any_tag_equal_to('tags', "memory")

# Create the vector search options and set the filter on the options.
vector_search_options = VectorSearchOptions(filter=filter)

# This snippet assumes search_vector is already provided, having been created using the embedding model of your choice.
search_result = await collection.vectorized_search(vector=search_vector, options=vector_search_options)
```
You can chain any number of filters together as shown above, and they will be combined with `and`.

#### EqualTo filter clause

Use `EqualTo` (class) or `.equal_to` ((class)method on the filter object) for a direct comparison between property and value.

#### AnyTagEqualTo filter clause

Use `AnyTagEqualTo`/`.any_tag_equal_to` to check if any of the strings, stored in a tag property in the vector store, contains a provided value.
For a property to be considered a tag property, it needs to be a List, array or other enumerable of string.

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
> For more information on how to set the `IsFilterable` field, refer to [VectorStoreRecordDataAttribute parameters](./defining-your-data-model.md#vectorstorerecorddataattribute-parameters) or [VectorStoreRecordDataProperty configuration settings](./schema-with-record-definition.md#vectorstorerecorddataproperty-configuration-settings).

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
