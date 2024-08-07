# What are Semantic Kernel Vector Store connectors? (Experimental)

Vector databases have many use cases across different domains and applications that involve natural language processing (NLP), computer vision (CV), recommendation systems (RS), and other areas that require semantic understanding and matching of data.

One use case for storing information in a vector database is to enable large language models (LLMs) to generate more relevant and coherent text. Large language models often face challenges such as generating inaccurate or irrelevant information; lacking factual consistency or common sense; repeating or contradicting themselves; being biased or offensive. To help overcome these challenges, you can use a vector database to store information about different topics, keywords, facts, opinions, and/or sources related to your desired domain or genre. The vector database allows you to efficiently find the subset of information related to a specific question or topic. You can then pass information from the vector database with your AI plugin to your large language model to generate more informative and engaging content that matches your intent and style.

For example, if you want to write a blog post about the latest trends in AI, you can use a vector database to store the latest information about that topic and pass the information along with the ask to a LLM in order to generate a blog post that leverages the latest information.

Semantic Kernel provides an abstraction for interacting with Vector Stores and a list of out-of-the-box connectors that implement these abstractions. Features include creating, listing and deleting collections of records, and uploading, retrieving and deleting records. The abstraction makes it easy to experiment with a free or locally hosted Vector Store and then switch to a service when needing to scale up.

## Getting started with Vector Store connectors

### Define your data model

The Semantic Kernel Vector Store connectors use a model first approach to interacting with databases. This means that the first step is to define a data model that maps to the storage schema. To help the connectors create collections of records and map to the storage schema, the model can be annotated to indicate the function of each property.

::: zone pivot="programming-language-csharp"

```csharp
using Microsoft.SemanticKernel;

public class Hotel
{
    [VectorStoreRecordKey]
    public ulong HotelId { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public string HotelName { get; set; }

    [VectorStoreRecordData(IsFullTextSearchable = true)]
    public string Description { get; set; }

    [VectorStoreRecordVector(4, IndexKind.Hnsw, DistanceFunction.CosineDistance)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public string[] Tags { get; set; }
}
```

::: zone-end

::: zone pivot="programming-language-python"

```python
```

::: zone-end

> [!TIP]
> For more information on how to annotate your data model, refer to [definining your data model](./defining-your-data-model.md).
> [!TIP]
> For an alternative to annotating your data model, refer to [definining your schema with a record definition](./schema-with-record-definition.md).

### Connect to your database and select a collection

Once you have defined your data model, the next step is to create a VectorStore instance for the database of your choice and select a collection of records.

::: zone pivot="programming-language-csharp"

Since databases support many different types of keys and records, we allow you to specify the type of the key and record for your collection using generics.
In our case, the type of record will be the `Hotel` class we already defined, and the type of key will be `ulong`, since the `HotelId` property is a `ulong` and Qdrant only supports `Guid` or `ulong` keys.

```csharp
using Microsoft.SemanticKernel.Connectors.Qdrant;

// Create a Qdrant VectorStore object
var vectorStore = new QdrantVectorStore(new QdrantClient("localhost"));

// Choose a collection from the database and specify the type of key and record stored in it via Generic parameters.
var collection = vectorStore.GetCollection<ulong, Hotel>("skglossary");
```

::: zone-end

::: zone pivot="programming-language-python"

```python
```

::: zone-end

> [!TIP]
> For more information on what key and field types each Vector Store connector supports, refer to [the documentation for each connector](./out-of-the-box-connectors.md).

### Create the collection and add records

::: zone pivot="programming-language-csharp"

```csharp
// Create the collection if it doesn't exist yet.
await collection.CreateCollectionIfNotExistsAsync();

// Upsert a record.
var descriptionText = "A place where everyone can be happy.";
await collection.UpsertAsync(new Hotel
{
    HotelId = 1,
    HotelName = "Hotel Happy",
    Description = descriptionText,
    DescriptionEmbedding = await GenerateEmbeddingAsync(descriptionText),
    Tags = new[] { "luxury", "pool" }
});

// Retrieve the upserted record.
var retrievedHotel = await collection.GetAsync(1);
```

::: zone-end

::: zone pivot="programming-language-python"

```python
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Learn about the Vector Store data architecture](./data-architecture.md)
