---
title: What are Semantic Kernel Vector Store connectors? (Preview)
description: Describes what a Semantic Kernel Vector Store is, and provides a basic example of how to use one and how to get started.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# What are Semantic Kernel Vector Store connectors? (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.
> [!TIP]
> If you are looking for information about the legacy Memory Store connectors, refer to the [Memory Stores page](./memory-stores.md).

Vector databases have many use cases across different domains and applications that involve natural language processing (NLP), computer vision (CV), recommendation systems (RS), and other areas that require semantic understanding and matching of data.

One use case for storing information in a vector database is to enable large language models (LLMs) to generate more relevant and coherent responses. Large language models often face challenges such as generating inaccurate or irrelevant information; lacking factual consistency or common sense; repeating or contradicting themselves; being biased or offensive. To help overcome these challenges, you can use a vector database to store information about different topics, keywords, facts, opinions, and/or sources related to your desired domain or genre. The vector database allows you to efficiently find the subset of information related to a specific question or topic. You can then pass information from the vector database with your prompt to your large language model to generate more accurate and relevant content.

For example, if you want to write a blog post about the latest trends in AI, you can use a vector database to store the latest information about that topic and pass the information along with the ask to a LLM in order to generate a blog post that leverages the latest information.

Semantic Kernel and .net provides an abstraction for interacting with Vector Stores and a list of out-of-the-box connectors that implement these abstractions. Features include creating, listing and deleting collections of records, and uploading, retrieving and deleting records. The abstraction makes it easy to experiment with a free or locally hosted Vector Store and then switch to a service when needing to scale up.

::: zone pivot="programming-language-csharp"

## Retrieval Augmented Generation (RAG) with Vector Stores

The vector store abstractions are a low level api for adding and retrieving data from vector stores.
Semantic Kernel has built-in support for using any one of the Vector Store implementations for RAG.
This is achieved by wrapping `IVectorSearch<TRecord>` and exposing it as a Text Search implementation.

> [!TIP]
> To learn more about how to use vector stores for RAG see [How to use Vector Stores with Semantic Kernel Text Search](../text-search/text-search-vector-stores.md).
> [!TIP]
> To learn more about text search see [What is Semantic Kernel Text Search?](../text-search/index.md)

## The Vector Store Abstraction

The main interfaces in the Vector Store abstraction are the following.

### Microsoft.Extensions.VectorData.IVectorStore

`IVectorStore` contains operations that spans across all collections in the vector store, e.g. ListCollectionNames.
It also provides the ability to get `IVectorStoreRecordCollection<TKey, TRecord>` instances.

### Microsoft.Extensions.VectorData.IVectorStoreRecordCollection\<TKey, TRecord\>

`IVectorStoreRecordCollection<TKey, TRecord>` represents a collection.
This collection may or may not exist, and the interface provides methods to check if the collection exists, create it or delete it.
The interface also provides methods to upsert, get and delete records.
Finally, the interface inherits from `IVectorSearch<TRecord>` providing vector search capabilities.

### Microsoft.Extensions.VectorData.IVectorSearch\<TRecord\>

- `SearchAsync<TRecord>` contains a method for doing vector searches taking some input that can be vectorized by a registered embedding generator or by the vector database where the database supports this.
- `SearchEmbeddingAsync<TRecord>` contains a method for doing vector searches taking a vector as input.

::: zone-end
::: zone pivot="programming-language-python"

## Retrieval Augmented Generation (RAG) with Vector Stores

The vector store abstractions are a low level api for adding and retrieving data from vector stores.
Semantic Kernel has built-in support for using any one of the Vector Store implementations for RAG.
This is achieved by wrapping `VectorSearchBase[TKey, TModel]` with either `VectorizedSearchMixin[Tmodel]`, `VectorizableTextSearchMixin[TModel]` or `VectorTextSearch[TModel]` and exposing it as a Text Search implementation.

> [!TIP]
> To learn more about how to use vector stores for RAG see [How to use Vector Stores with Semantic Kernel Text Search](../text-search/text-search-vector-stores.md).
> [!TIP]
> To learn more about text search see [What is Semantic Kernel Text Search?](../text-search/index.md)

::: zone-end
::: zone pivot="programming-language-java"

## The Vector Store Abstraction

The main interfaces in the Vector Store abstraction are the following.

### com.microsoft.semantickernel.data.vectorstorage.VectorStore

`VectorStore` contains operations that spans across all collections in the vector store, e.g. listCollectionNames.
It also provides the ability to get `VectorStoreRecordCollection<Key, Record>` instances.

### com.microsoft.semantickernel.data.vectorstorage.VectorStoreRecordCollection\<Key, Record\>

`VectorStoreRecordCollection<Key, Record>` represents a collection.
This collection may or may not exist, and the interface provides methods to check if the collection exists, create it or delete it.
The interface also provides methods to upsert, get and delete records.
Finally, the interface inherits from `VectorizedSearch<Record>` providing vector search capabilities.

### com.microsoft.semantickernel.data.vectorsearch.VectorizedSearch\<Record\>

`VectorizedSearch<Record>` contains a method for doing vector searches.
`VectorStoreRecordCollection<Key, Record>` inherits from `VectorizedSearch<Record>` making it possible to use
`VectorizedSearch<Record>` on its own in cases where only search is needed and no record or collection management is needed.

### com.microsoft.semantickernel.data.vectorsearch.VectorizableTextSearch\<Record\>

`VectorizableTextSearch<Record>` contains a method for doing vector searches where the vector database has the ability to
generate embeddings automatically. E.g. you can call this method with a text string and the database will generate the embedding
for you and search against a vector field. This is not supported by all vector databases and is therefore only implemented
by select connectors.

::: zone-end

## Getting started with Vector Store connectors

::: zone pivot="programming-language-csharp"

### Import the necessary nuget packages

All the vector store interfaces and any abstraction related classes are available in the `Microsoft.Extensions.VectorData.Abstractions` nuget package.
Each vector store implementation is available in its own nuget package. For a list of known implementations, see the [Out-of-the-box connectors page](./out-of-the-box-connectors/index.md).

The abstractions package can be added like this.

```dotnetcli
dotnet add package Microsoft.Extensions.VectorData.Abstractions --prerelease
```

> [!WARNING]
> From version 1.23.0 of Semantic Kernel, the Vector Store abstractions have been removed from `Microsoft.SemanticKernel.Abstractions`
> and are available in the new dedicated `Microsoft.Extensions.VectorData.Abstractions` package.
>
> Note that from version 1.23.0, `Microsoft.SemanticKernel.Abstractions` has a dependency on `Microsoft.Extensions.VectorData.Abstractions`,
> therefore there is no need to reference additional packages.
> The abstractions will however now be in the new `Microsoft.Extensions.VectorData` namespace.
>
> When upgrading from 1.22.0 or earlier to 1.23.0 or later, you will need to add an additional `using Microsoft.Extensions.VectorData;`
> clause in files where any of the Vector Store abstraction types are used e.g. `IVectorStore`, `IVectorStoreRecordCollection`, `VectorStoreRecordDataAttribute`, `VectorStoreRecordKeyProperty`, etc.
>
> This change has been made to support vector store providers when creating their own implementations. A provider only has to reference
> the `Microsoft.Extensions.VectorData.Abstractions` package. This reduces potential version conflicts and allows Semantic Kernel
> to continue to evolve fast without impacting vector store providers.

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end

### Define your data model

The Semantic Kernel Vector Store connectors use a model first approach to interacting with databases. This means that the first step is to define a data model that maps to the storage schema. To help the connectors create collections of records and map to the storage schema, the model can be annotated to indicate the function of each property.

::: zone pivot="programming-language-csharp"

```csharp
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreRecordKey]
    public ulong HotelId { get; set; }

    [VectorStoreRecordData(IsIndexed = true)]
    public string HotelName { get; set; }

    [VectorStoreRecordData(IsFullTextIndexed = true)]
    public string Description { get; set; }

    [VectorStoreRecordVector(Dimensions: 4, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }

    [VectorStoreRecordData(IsIndexed = true)]
    public string[] Tags { get; set; }
}
```

::: zone-end
::: zone pivot="programming-language-python"
```python
from dataclasses import dataclass, field
from typing import Annotated
from semantic_kernel.data import (
    DistanceFunction,
    IndexKind,
    VectorStoreRecordDataField,
    VectorStoreRecordDefinition,
    VectorStoreRecordKeyField,
    VectorStoreRecordVectorField,
    vectorstoremodel,
)

@vectorstoremodel
@dataclass
class Hotel:
    hotel_id: Annotated[str, VectorStoreRecordKeyField()] = field(default_factory=lambda: str(uuid4()))
    hotel_name: Annotated[str, VectorStoreRecordDataField(is_filterable=True)]
    description: Annotated[str, VectorStoreRecordDataField(is_full_text_searchable=True)]
    description_embedding: Annotated[list[float], VectorStoreRecordVectorField(dimensions=4, distance_function=DistanceFunction.COSINE, index_kind=IndexKind.HNSW)]
    tags: Annotated[list[str], VectorStoreRecordDataField(is_filterable=True)]
```
::: zone-end

::: zone pivot="programming-language-java"
```java
import com.microsoft.semantickernel.data.vectorstorage.annotations.VectorStoreRecordData;
import com.microsoft.semantickernel.data.vectorstorage.annotations.VectorStoreRecordKey;
import com.microsoft.semantickernel.data.vectorstorage.annotations.VectorStoreRecordVector;
import com.microsoft.semantickernel.data.vectorstorage.definition.DistanceFunction;
import com.microsoft.semantickernel.data.vectorstorage.definition.IndexKind;

import java.util.Collections;
import java.util.List;

public class Hotel {
    @VectorStoreRecordKey
    private String hotelId;

    @VectorStoreRecordData(isFilterable = true)
    private String name;

    @VectorStoreRecordData(isFullTextSearchable = true)
    private String description;

    @VectorStoreRecordVector(dimensions = 4, indexKind = IndexKind.HNSW, distanceFunction = DistanceFunction.COSINE_DISTANCE)
    private List<Float> descriptionEmbedding;

    @VectorStoreRecordData(isFilterable = true)
    private List<String> tags;

    public Hotel() { }

    public Hotel(String hotelId, String name, String description, List<Float> descriptionEmbedding, List<String> tags) {
        this.hotelId = hotelId;
        this.name = name;
        this.description = description;
        this.descriptionEmbedding = Collections.unmodifiableList(descriptionEmbedding);
        this.tags = Collections.unmodifiableList(tags);
    }

    public String getHotelId() { return hotelId; }
    public String getName() { return name; }
    public String getDescription() { return description; }
    public List<Float> getDescriptionEmbedding() { return descriptionEmbedding; }
    public List<String> getTags() { return tags; }
}
```
::: zone-end

> [!TIP]
> For more information on how to annotate your data model, refer to [defining your data model](./defining-your-data-model.md).
> [!TIP]
> For an alternative to annotating your data model, refer to [defining your schema with a record definition](./schema-with-record-definition.md).

### Connect to your database and select a collection

Once you have defined your data model, the next step is to create a VectorStore instance for the database of your choice and select a collection of records.

::: zone pivot="programming-language-csharp"

In this example, we'll use Qdrant. You will therefore need to import the Qdrant nuget package.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.Qdrant --prerelease
```

If you want to run Qdrant locally using Docker, use the following command to start the Qdrant container
with the settings used in this example.

```cli
docker run -d --name qdrant -p 6333:6333 -p 6334:6334 qdrant/qdrant:latest
```

To verify that your Qdrant instance is up and running correctly, visit the Qdrant dashboard that is
built into the Qdrant docker container: [http://localhost:6333/dashboard](http://localhost:6333/dashboard)

Since databases support many different types of keys and records, we allow you to specify the type of the key and record for your collection using generics.
In our case, the type of record will be the `Hotel` class we already defined, and the type of key will be `ulong`, since the `HotelId` property is a `ulong` and Qdrant only supports `Guid` or `ulong` keys.

```csharp
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

// Create a Qdrant VectorStore object
var vectorStore = new QdrantVectorStore(new QdrantClient("localhost"));

// Choose a collection from the database and specify the type of key and record stored in it via Generic parameters.
var collection = vectorStore.GetCollection<ulong, Hotel>("skhotels");
```

::: zone-end
::: zone pivot="programming-language-python"

Since databases support many different types of keys and records, we allow you to specify the type of the key and record for your collection using generics.
In our case, the type of record will be the `Hotel` class we already defined, and the type of key will be `str`, since the `HotelId` property is a `str` and Qdrant only supports `str` or `int` keys.

```python
from semantic_kernel.connectors.memory.qdrant import QdrantStore

# Create a Qdrant VectorStore object, this will look in the environment for Qdrant related settings, and will fall back to the default, which is to run in-memory.
vector_store = QdrantStore()

# Choose a collection from the database and specify the type of key and record stored in it via Generic parameters.
collection = vector_store.get_collection(
    collection_name="skhotels", 
    data_model_type=Hotel
)
```
::: zone-end
::: zone pivot="programming-language-java"
Since databases support many different types of keys and records, we allow you to specify the type of the key and record for your collection using generics.
In our case, the type of record will be the `Hotel` class we already defined, and the type of key will be `String`, since the `hotelId` property is a `String` and JDBC store only supports `String` keys.

```java
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStore;
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStoreOptions;
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStoreRecordCollectionOptions;
import com.microsoft.semantickernel.data.jdbc.mysql.MySQLVectorStoreQueryProvider;
import com.mysql.cj.jdbc.MysqlDataSource;

import java.util.List;

public class Main {
    public static void main(String[] args) {
        // Create a MySQL data source
        var dataSource = new MysqlDataSource();
        dataSource.setUrl("jdbc:mysql://localhost:3306/sk");
        dataSource.setPassword("root");
        dataSource.setUser("root");

        // Create a JDBC vector store
        var vectorStore = JDBCVectorStore.builder()
            .withDataSource(dataSource)
            .withOptions(
                JDBCVectorStoreOptions.builder()
                    .withQueryProvider(MySQLVectorStoreQueryProvider.builder()
                        .withDataSource(dataSource)
                        .build())
                    .build()
            )
            .build();

        // Get a collection from the vector store
        var collection = vectorStore.getCollection("skhotels",
            JDBCVectorStoreRecordCollectionOptions.<Hotel>builder()
                .withRecordClass(Hotel.class)
                .build()
        );
    }
}
```
::: zone-end

> [!TIP]
> For more information on what key and field types each Vector Store connector supports, refer to [the documentation for each connector](./out-of-the-box-connectors/index.md).

::: zone pivot="programming-language-csharp"

### Create the collection and add records

```csharp
// Placeholder embedding generation method.
async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string textToVectorize)
{
    // your logic here
}

// Create the collection if it doesn't exist yet.
await collection.CreateCollectionIfNotExistsAsync();

// Upsert a record.
string descriptionText = "A place where everyone can be happy.";
ulong hotelId = 1;

// Create a record and generate a vector for the description using your chosen embedding generation implementation.
await collection.UpsertAsync(new Hotel
{
    HotelId = hotelId,
    HotelName = "Hotel Happy",
    Description = descriptionText,
    DescriptionEmbedding = await GenerateEmbeddingAsync(descriptionText),
    Tags = new[] { "luxury", "pool" }
});

// Retrieve the upserted record.
Hotel? retrievedHotel = await collection.GetAsync(hotelId);
```

::: zone-end
::: zone pivot="programming-language-python"

### Create the collection and add records

```python
# Create the collection if it doesn't exist yet.
await collection.create_collection_if_not_exists()

# Upsert a record.
description = "A place where everyone can be happy."
hotel_id = "1"

await collection.upsert(Hotel(
    hotel_id = hotel_id,
    hotel_name = "Hotel Happy",
    description = description,
    description_embedding = await GenerateEmbeddingAsync(description),
    tags = ["luxury", "pool"]
))

# Retrieve the upserted record.
retrieved_hotel = await collection.get(hotel_id)
```

::: zone-end
::: zone pivot="programming-language-java"

```java
// Create the collection if it doesn't exist yet.
collection.createCollectionAsync().block();

// Upsert a record.
var description = "A place where everyone can be happy";
var hotelId = "1";
var hotel = new Hotel(
    hotelId, 
    "Hotel Happy", 
    description, 
    generateEmbeddingsAsync(description).block(), 
    List.of("luxury", "pool")
);

collection.upsertAsync(hotel, null).block();

// Retrieve the upserted record.
var retrievedHotel = collection.getAsync(hotelId, null).block();
```
::: zone-end

::: zone pivot="programming-language-csharp"

> [!TIP]
> For more information on how to generate embeddings see [embedding generation](./embedding-generation.md).

### Do a vector search

```csharp
// Placeholder embedding generation method.
async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string textToVectorize)
{
    // your logic here
}

// Generate a vector for your search text, using your chosen embedding generation implementation.
ReadOnlyMemory<float> searchVector = await GenerateEmbeddingAsync("I'm looking for a hotel where customer happiness is the priority.");

// Do the search.
var searchResult = await collection.SearchEmbeddingAsync(searchVector, new() { Top = 1 });

// Inspect the returned hotel.
await foreach (var record in searchResult)
{
    Console.WriteLine("Found hotel description: " + record.Record.Description);
    Console.WriteLine("Found record score: " + record.Score);
}
```

::: zone-end
::: zone pivot="programming-language-python"

### Do a vector search

```python
# Generate a vector for your search text, using your chosen embedding generation implementation.
# Just showing a placeholder method here for brevity.
search_vector = await GenerateEmbedding("I'm looking for a hotel where customer happiness is the priority.");
# Do the search.
search_result = await collection.vectorized_search(vector=searchVector, VectorSearchOptions(top = 1 ))

# Inspect the returned hotels.
async for result in search_result.results:
    print(f"Found hotel description: {result.record.description}")
```

::: zone-end
::: zone pivot="programming-language-java"
```java
// Generate a vector for your search text, using your chosen embedding generation implementation.
// Just showing a placeholder method here for brevity.
var searchVector = generateEmbeddingsAsync("I'm looking for a hotel where customer happiness is the priority.").block();

// Do the search.
var searchResult = collection.searchAsync(searchVector, VectorSearchOptions.builder()
    .withTop(1).build()
).block();

Hotel record = searchResult.getResults().get(0).getRecord();
System.out.printf("Found hotel description: %s\n", record.getDescription());
```
::: zone-end

> [!TIP]
> For more information on how to generate embeddings see [embedding generation](./embedding-generation.md).

## Next steps

> [!div class="nextstepaction"]
> [Learn about the Vector Store data architecture](./data-architecture.md)
> [How to ingest data into a Vector Store](./how-to/vector-store-data-ingestion.md)
