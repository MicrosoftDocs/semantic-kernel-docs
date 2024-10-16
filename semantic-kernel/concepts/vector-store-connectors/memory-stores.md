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

Semantic Kernel provides a set of Memory Store abstractions where the primary interface is `Microsoft.SemanticKernel.Memory.IMemoryStore`.

## Memory Store vs Vector Store abstractions

As part of an effort to evolve and expand the vector storage and search capbilities of Semantic Kernel, we have released a new set of abstractions to replace the Memory Store abstractions.
We are calling the replacement abstractions Vector Store abstractions.
The purpose of both are similar, but their interfaces differ and the Vector Store abstractions provide expanded functionality.

::: zone pivot="programming-language-csharp"

|Characteristic|Legacy Memory Stores|Vector Stores|
|-|-|-|
|Main Interface|IMemoryStore|IVectorStore|
|Abstractions nuget package|Microsoft.SemanticKernel.Abstractions|Microsoft.Extensions.VectorData.Abstractions|
|Naming Convention|{Provider}MemoryStore, e.g. RedisMemoryStore|{Provider}VectorStore, e.g. RedisVectorStore|
|Supports record upsert, get and delete|Yes|Yes|
|Supports collection create and delete|Yes|Yes|
|Supports vector search|Yes|Yes|
|Supports choosing your preferred vector search index and distance function|No|Yes|
|Supports multiple vectors per record|No|Yes|
|Supports custom schemas|No|Yes|
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
| Vector Database in Azure Cosmos DB for NoSQL | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.AzureCosmosDBNoSQL) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/azure_cosmosdb_no_sql)
| Vector Database in vCore-based Azure Cosmos DB for MongoDB | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.AzureCosmosDBMongoDB) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/azure_cosmosdb) |
| Azure AI Search   | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.AzureAISearch) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/azure_cognitive_search) |
| Azure PostgreSQL Server  | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Postgres) |
| Azure SQL Database       | [C#](https://github.com/kbeaugrand/SemanticKernel.Connectors.Memory.SqlServer) |
| Chroma                   | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Chroma) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/chroma) |
| DuckDB                   | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.DuckDB) |  |
| Milvus                   | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Milvus) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/milvus) |
| MongoDB Atlas Vector Search | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.MongoDB) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/mongodb_atlas) |
| Pinecone                 | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Pinecone) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/pinecone) |
| Postgres                 | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Postgres) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/postgres) |
| Qdrant                   | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Qdrant) |  |
| Redis                    | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Redis) |  |
| Sqlite                   | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Sqlite) |  |
| Weaviate                 | [C#](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Connectors/Connectors.Memory.Weaviate) | [Python](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/memory/weaviate) |
