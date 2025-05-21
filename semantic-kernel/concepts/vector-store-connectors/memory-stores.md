---
title: Legacy Semantic Kernel Memory Stores
description: Describes the legacy Semantic Kernel Memory Stores and the benefits of moving to Vector Stores
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 10/15/2024
ms.service: semantic-kernel
---
# Legacy Semantic Kernel Memory Stores

> [!TIP]
> We recommend using the Vector Store abstractions instead of the legacy Memory Stores. For more information on how to use the Vector Store abstractions start [here](./index.md).

Semantic Kernel provides a set of Memory Store abstractions where the primary interface is `Microsoft.SemanticKernel.Memory.IMemoryStore`.

## Memory Store vs Vector Store abstractions

As part of an effort to evolve and expand the vector storage and search capabilities of Semantic Kernel, we have released a new set of abstractions to replace the Memory Store abstractions.
We are calling the replacement abstractions Vector Store abstractions.
The purpose of both are similar, but their interfaces differ and the Vector Store abstractions provide expanded functionality.

::: zone pivot="programming-language-csharp"

|Characteristic|Legacy Memory Stores|Vector Stores|
|-|-|-|
|Main Interface|IMemoryStore|VectorStore|
|Abstractions nuget package|Microsoft.SemanticKernel.Abstractions|Microsoft.Extensions.VectorData.Abstractions|
|Naming Convention|{Provider}MemoryStore, e.g. RedisMemoryStore|{Provider}VectorStore, e.g. RedisVectorStore|
|Supports record upsert, get and delete|Yes|Yes|
|Supports collection create and delete|Yes|Yes|
|Supports vector search|Yes|Yes|
|Supports choosing your preferred vector search index and distance function|No|Yes|
|Supports multiple vectors per record|No|Yes|
|Supports custom schemas|No|Yes|
|Supports multiple vector types|No|Yes|
|Supports metadata pre-filtering for vector search|No|Yes|
|Supports vector search on non-vector databases by downloading the entire dataset onto the client and doing a local vector search|Yes|No|

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end

## Available Memory Store connectors

Semantic Kernel offers several Memory Store connectors to vector databases that you can use to store and retrieve information. These include:

| Service                  | C# | Python |
|--------------------------|:----:|:------:|
| Vector Database in Azure Cosmos DB for NoSQL | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.CosmosNoSql) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/azure_cosmosdb_no_sql) |
| Vector Database in vCore-based Azure Cosmos DB for MongoDB | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.CosmosMongoDB) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/azure_cosmosdb) |
| Azure AI Search   | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.AzureAISearch) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/azure_cognitive_search) |
| Azure PostgreSQL Server  | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Postgres) | |
| Azure SQL Database       | [C#](https://github.com/kbeaugrand/SemanticKernel.Connectors.Memory.SqlServer) | |
| Chroma                   | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Chroma) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/chroma) |
| DuckDB                   | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.DuckDB) |  |
| Milvus                   | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Milvus) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/milvus) |
| MongoDB Atlas Vector Search | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.MongoDB) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/mongodb_atlas) |
| Pinecone                 | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Pinecone) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/pinecone) |
| Postgres                 | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Postgres) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/postgres) |
| Qdrant                   | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Qdrant) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/qdrant) |
| Redis                    | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Redis) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/redis) |
| Sqlite                   | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Sqlite) |  |
| Weaviate                 | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Weaviate) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/weaviate) |

::: zone pivot="programming-language-csharp"

## Migrating from Memory Stores to Vector Stores

If you wanted to migrate from using the Memory Store abstractions to the Vector Store abstractions there are various ways in which you can do this.

### Use the existing collection with the Vector Store abstractions

The simplest way in many cases could be to just use the Vector Store abstractions to access a collection that was created using the Memory Store abstractions.
In many cases this is possible, since the Vector Store abstraction allows you to choose the schema that you would like to use.
The main requirement is to create a data model that matches the schema that the legacy Memory Store implementation used.

E.g. to access a collection created by the Azure AI Search Memory Store, you can use the following Vector Store data model.

```csharp
using Microsoft.Extensions.VectorData;

class VectorStoreRecord
{
    [VectorStoreKey]
    public string Id { get; set; }

    [VectorStoreData]
    public string Description { get; set; }

    [VectorStoreData]
    public string Text { get; set; }

    [VectorStoreData]
    public bool IsReference { get; set; }

    [VectorStoreData]
    public string ExternalSourceName { get; set; }

    [VectorStoreData]
    public string AdditionalMetadata { get; set; }

    [VectorStoreVector(VectorSize)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}
```

> [!TIP]
> For more detailed examples on how to use the Vector Store abstractions to access collections created using a Memory Store, see [here](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_ConsumeFromMemoryStore_Common.cs).

### Create a new collection

In some cases migrating to a new collection may be preferable than using the existing collection directly. The schema that was chosen by the Memory Store may not match your requirements, especially with regards to filtering.

E.g. The Redis Memory store uses a schema with three fields:

- string metadata
- long timestamp
- float[] embedding

All data other than the embedding or timestamp is stored as a serialized json string in the Metadata field. This means that it is not possible to index the individual values and filter on them.
E.g. perhaps you may want to filter using the ExternalSourceName, but this is not possible while it is inside a json string.

In this case, it may be better to migrate the data to a new collection with a flat schema.
There are two options here. You could create a new collection from your source data or simply map and copy the data from the old to the new.
The first option may be more costly as you will need to regenerate the embeddings from the source data.

> [!TIP]
> For an example using Redis showing how to copy data from a collection created using the Memory Store abstractions to one created using the Vector Store abstractions see [here](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_MigrateFromMemoryStore_Redis.cs).

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end
