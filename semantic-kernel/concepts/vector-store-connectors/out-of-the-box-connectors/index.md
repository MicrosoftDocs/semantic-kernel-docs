---
title: Out-of-the-box Vector Store connectors (Preview)
description: Out-of-the-box Vector Store connectors
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: article
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Out-of-the-box Vector Store connectors (Preview)

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

Semantic Kernel provides a number of out-of-the-box Vector Store integrations making it easy to get started with using Vector Stores. It also allows you to experiment with a free or locally hosted Vector Store and then easily switch to a service when scale requires it.

> [!IMPORTANT]
> Semantic Kernel Vector Store connectors are built by a variety of sources. Not all connectors are maintained as part of the Microsoft Semantic Kernel Project. When considering a connector, be sure to evaluate quality, licensing, support, etc. to ensure they meet your requirements. Also make sure you review each provider's documentation for detailed version compatibility information.
> [!IMPORTANT]
> Some connectors are internally using Database SDKs that are not officially supported by Microsoft or by the Database provider. The *Uses Officially supported SDK* column lists which are using officially supported SDKs and which are not.

::: zone pivot="programming-language-csharp"

| Vector Store Connectors                                            |             C#               | Uses officially supported SDK  |        Maintainer / Vendor        |
| ------------------------------------------------------------------ | :--------------------------: | :----------------------------: | :-------------------------------: |
| [Azure AI Search](./azure-ai-search-connector.md)                  |             ✅              |               ✅               | Microsoft Semantic Kernel Project |
| [Cosmos DB MongoDB (vCore)](./azure-cosmosdb-mongodb-connector.md) |             ✅              |               ✅               | Microsoft Semantic Kernel Project |
| [Cosmos DB No SQL](./azure-cosmosdb-nosql-connector.md)            |             ✅              |               ✅               | Microsoft Semantic Kernel Project |
| [Couchbase](./couchbase-connector.md)                              |             ✅              |               ✅               |             Couchbase             |
| [Elasticsearch](./elasticsearch-connector.md)                      |             ✅              |               ✅               |              Elastic              |
| Chroma                                                             |          Planned             |                                |                                   |
| [In-Memory](./inmemory-connector.md)                               |             ✅              |              N/A                | Microsoft Semantic Kernel Project |
| Milvus                                                             |          Planned             |                                |                                   |
| [MongoDB](./mongodb-connector.md)                                  |             ✅              |               ✅               | Microsoft Semantic Kernel Project |
| [Neon Serverless Postgres](https://azuremarketplace.microsoft.com/en-us/marketplace/apps/neon1722366567200.neon_serverless_postgres_azure_prod) |Use [Postgres Connector](./postgres-connector.md)|      ✅               | Microsoft Semantic Kernel Project |
| [Oracle](./oracle-connector.md)                                    |             ✅              |               ✅               |              Oracle               |
| [Pinecone](./pinecone-connector.md)                                |             ✅              |               ❌               | Microsoft Semantic Kernel Project |
| [Postgres](./postgres-connector.md)                                |             ✅              |               ✅               | Microsoft Semantic Kernel Project |
| [Qdrant](./qdrant-connector.md)                                    |             ✅              |               ✅               | Microsoft Semantic Kernel Project |
| [Redis](./redis-connector.md)                                      |             ✅              |               ✅               | Microsoft Semantic Kernel Project |
| [SQL Server](./sql-connector.md)                                   |             ✅              |               ✅               | Microsoft Semantic Kernel Project |
| [SQLite](./sqlite-connector.md)                                    |             ✅              |               ✅               | Microsoft Semantic Kernel Project |
| [Volatile (In-Memory)](./volatile-connector.md)                    | Deprecated (use In-Memory)   |              N/A               | Microsoft Semantic Kernel Project |
| [Weaviate](./weaviate-connector.md)                                |             ✅              |               ✅               | Microsoft Semantic Kernel Project |

::: zone-end
::: zone pivot="programming-language-python"

| Vector Store Connectors                                            | Python  |       Uses officially supported SDK        |        Maintainer / Vendor        |
| ------------------------------------------------------------------ | :-----: | :----------------------------------------: | :-------------------------------: |
| [Azure AI Search](./azure-ai-search-connector.md)                  |    ✅    |                     ✅                      | Microsoft Semantic Kernel Project |
| [Cosmos DB MongoDB (vCore)](./azure-cosmosdb-mongodb-connector.md) |    ✅    |                     ✅                      | Microsoft Semantic Kernel Project |
| [Cosmos DB No SQL](./azure-cosmosdb-nosql-connector.md)            |    ✅    |                     ✅                      | Microsoft Semantic Kernel Project |
| [Chroma](./chroma-connector.md)                                    |    ✅    |                     ✅                      | Microsoft Semantic Kernel Project |
| [Elasticsearch](./elasticsearch-connector.md)                      | Planned   |                                             |                                   |
| [Faiss](./faiss-connector.md)                                      |    ✅    |                     ✅                      | Microsoft Semantic Kernel Project |
| [In-Memory](./inmemory-connector.md)                               |    ✅    |                    N/A                       | Microsoft Semantic Kernel Project |
| [MongoDB](./mongodb-connector.md)                                  |    ✅    |                     ✅                      | Microsoft Semantic Kernel Project |
| [Neon Serverless Postgres](https://azuremarketplace.microsoft.com/en-us/marketplace/apps/neon1722366567200.neon_serverless_postgres_azure_prod) |Use [Postgres Connector](./postgres-connector.md)| ✅              | Microsoft Semantic Kernel Project |
| [Oracle](./oracle-connector.md)                                    |    ✅    |        ✅              |   Oracle               |
| [Pinecone](./pinecone-connector.md)                                |    ✅    |                     ✅                      | Microsoft Semantic Kernel Project |
| [Postgres](./postgres-connector.md)                                |    ✅    |                     ✅                      | Microsoft Semantic Kernel Project |
| [Qdrant](./qdrant-connector.md)                                    |    ✅    |                     ✅                      | Microsoft Semantic Kernel Project |
| [Redis](./redis-connector.md)                                      |    ✅    |                     ✅                      | Microsoft Semantic Kernel Project |
| [SQL Server](./sql-connector.md)                                   |    ✅    | [pyodbc](https://pypi.org/project/pyodbc/)  | Microsoft Semantic Kernel Project |
| SQLite                                                             | Planned   |                                             | Microsoft Semantic Kernel Project |
| [Weaviate](./weaviate-connector.md)                                |    ✅    |                     ✅                      | Microsoft Semantic Kernel Project |

::: zone-end
::: zone pivot="programming-language-java"

| Vector Store Connectors                           |              Java               | Uses officially supported SDK |        Maintainer / Vendor        |
| ------------------------------------------------- | :-----------------------------: | :---------------------------: | :-------------------------------: |
| [Azure AI Search](./azure-ai-search-connector.md) |                ✅                |               ✅               | Microsoft Semantic Kernel Project |
| HSQLDB                                            | Use [JDBC](./jdbc-connector.md) |               ✅               | Microsoft Semantic Kernel Project |
| [JDBC](./jdbc-connector.md)                       |                ✅                |               ✅               | Microsoft Semantic Kernel Project |
| MySQL                                             | Use [JDBC](./jdbc-connector.md) |               ✅               | Microsoft Semantic Kernel Project |
| [Oracle](./oracle-connector.md)                   |             ✅                  |               ✅               |              Oracle               |
| Postgres                                          | Use [JDBC](./jdbc-connector.md) |                               | Microsoft Semantic Kernel Project |
| [Redis](./redis-connector.md)                     |                ✅                |               ✅               | Microsoft Semantic Kernel Project |
| SQLite                                            | Use [JDBC](./jdbc-connector.md) |               ✅               | Microsoft Semantic Kernel Project |
| [Volatile (In-Memory)](./volatile-connector.md)   |                ✅                |              N/A              | Microsoft Semantic Kernel Project |

::: zone-end
