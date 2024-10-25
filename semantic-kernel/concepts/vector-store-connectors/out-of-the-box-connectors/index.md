---
title: Out-of-the-box Vector Store connectors (Preview)
description: Out-of-the-box Vector Store connectors
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Out-of-the-box Vector Store connectors (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

Semantic Kernel provides a number of out-of-the-box Vector Store integrations making it easy to get started with using Vector Stores. It also allows you to experiment with a free or locally hosted Vector Store and then easily switch to a service when scale requires it.

> [!IMPORTANT]
> Some connectors are using SDKs that are not officially supported by Microsoft or by the Database provider. The *Officially supported SDK* column lists which are using officially supported SDKs and which are not.

| Vector Store Connectors                                    |  C#            | Python          | Java           | Officially supported SDK           |
|------------------------------------------------------------|:--------------:|:---------------:|:--------------:|:----------------------------------:|
| [Azure AI Search](./azure-ai-search-connector.md)          | ✅             | ✅             | In Development | ✅                                |
| [Cosmos DB MongoDB](./azure-cosmosdb-mongodb-connector.md) | ✅             | In Development  | In Development | ✅                                |
| [Cosmos DB No SQL](./azure-cosmosdb-nosql-connector.md)    | ✅             | In Development  | In Development | ✅                                |
| [In-Memory](./inmemory-connector.md)                       | ✅             | In Development  | In Development | N/A                               |
| [Pinecone](./pinecone-connector.md)                        | ✅             | In Development  | In Development | C#: ❌ Python: ✅                |
| [Qdrant](./qdrant-connector.md)                            | ✅             | ✅             | In Development | ✅                                |
| [Redis](./redis-connector.md)                              | ✅             | ✅             | In Development | ✅                                |
| [SQLite](./sqlite-connector.md)                            | ✅             | In Development  | In Development | ✅                               |
| [Volatile (In-Memory)](./volatile-connector.md)            | Deprecated (use In-Memory) | ✅             | In Development | N/A                                |
| [Weaviate](./weaviate-connector.md)                        | ✅             | In Development  | In Development | N/A                               |
