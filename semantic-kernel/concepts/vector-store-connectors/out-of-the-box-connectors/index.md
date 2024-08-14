---
title: Out-of-the-box Vector Store connectors (Experimental)
description: Out-of-the-box Vector Store connectors (Experimental)
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Out-of-the-box Vector Store connectors (Experimental)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is experimental, still in development and is subject to change.

Semantic Kernel provides a number of out-of-the-box Vector Store integrations making it easy to get started with using Vector Stores. It also allows you to experiment with a free or locally hosted Vector Store and then easily switch to a service when scale requires it.

> [!IMPORTANT]
> Some connectors are using SDKs that are not officially supported by Microsoft or by the Database provider. The *Officially supported database SDK* column lists which are using officially supported SDKs and which are not.

| Vector Store Connectors                             |  C#            | Python          | Java           | Officially supported database SDK |
|-----------------------------------------------------|:--------------:|:---------------:|:--------------:|:---------------------------------:|
| [Azure AI Search](./azure-ai-search-connector.md)   | ✅             | ✅             | In Development | ✅                                |
| Cosmos DB No SQL                                    | In Development | In Development  | In Development | ✅                                |
| Cosmos DB MongoDB                                   | In Development | In Development  | In Development | ✅                                |
| [Pinecone](./pinecone-connector.md)                 | ✅             | In Development | In Development | ❌                                |
| [Qdrant](./qdrant-connector.md)                     | ✅             | ✅             | In Development | ✅                                |
| [Redis](./redis-connector.md)                       | ✅             | ✅             | In Development | ✅                                |
| [Volatile (In-Memory)](./volatile-connector.md)     | ✅             | ✅             | In Development | N/A                               |
