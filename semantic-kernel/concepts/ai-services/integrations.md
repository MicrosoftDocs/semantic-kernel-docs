---
title: integrations
description: Learn which features are available for C#, Python, and Java through integrations.
author: sophialagerkranspandey
ms.topic: reference
ms.author: sopand
ms.date: 07/11/2023
ms.service: semantic-kernel
---

# Integrations for Semantic Kernel 

Semantic Kernel provides a wide range of integrations to help you build powerful AI agents. These integrations include AI services, memory connectors. Additionally, Semantic Kernel integrates with other Microsoft services to provide additional functionality via plugins.

## Out-of-the-box integrations

With the available AI and memory connectors, developers can easily build AI agents with swappable components. This allows you to experiment with different AI services and memory connectors to find the best combination for your use case.

### AI Services

| Services                          |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| Text Generation                    | ✅ | ✅ | ✅ | Example: Text-Davinci-003 |
| Chat Completion                    | ✅ | ✅ | ✅ | Example: GPT4, Chat-GPT |
| Text Embeddings (Experimental)     | ✅ | ✅ | ✅ | Example: Text-Embeddings-Ada-002 |
| Text to Image (Experimental)       | ✅ | ❌ | ❌ | Example: Dall-E |
| Image to Text (Experimental)       | ✅ | ❌ | ❌ | Example: Pix2Struct |
| Text to Audio (Experimental)       | ✅ | ❌ | ❌ | Example: Text-to-speech |
| Audio to Text (Experimental)       | ✅ | ❌ | ❌ | Example: Whisper |


### Memory Connectors (Experimental)

Vector databases have many use cases across different domains and applications that involve natural language processing (NLP), computer vision (CV), recommendation systems (RS), and other areas that require semantic understanding and matching of data.

One use case for storing information in a vector database is to enable large language models (LLMs) to generate more relevant and coherent responses based on an [AI plugin](../create-plugins/index.md).

However, large language models often face challenges such as generating inaccurate or irrelevant information; lacking factual consistency or common sense; repeating or contradicting themselves; being biased or offensive. To overcome these challenges, you can use a vector database to store information about different topics, keywords, facts, opinions, and/or sources related to your desired domain or genre.
Then, you can use a large language model and pass information from the vector database with your AI prompt to generate more accurate and relevant content.

For example, if you want to write a blog post about the latest trends in AI, you can use a vector database to store the latest information about that topic and pass the information along with the ask to a LLM in order to generate a blog post that leverages the latest information.

## Available connectors to vector databases
Today, Semantic Kernel offers several connectors to vector databases that you can use to store and retrieve information. These include:


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

### Vector database solutions
- [Azure Cosmos DB for NoSQL](/azure/cosmos-db/nosql/vector-search) Integrated Vector Database with DiskANN
- [Azure Cosmos DB for MongoDB](/azure/cosmos-db/mongodb/vcore/vector-search) Integrated Vector Database
- [Azure SQL Database](/azure/azure-sql/database/ai-artificial-intelligence-intelligent-applications?&preserve-view=true#vector-search)
- [Azure PostgreSQL Server pgvector Extension](/azure/postgresql/flexible-server/how-to-use-pgvector)
- [Azure AI Search](/azure/search/search-what-is-azure-search)
- [Open-source vector databases](/azure/cosmos-db/mongodb/vcore/vector-search-ai)

:::image type="content" source="../media/decision-guide-databases-and-ai-search.png" lightbox="../media/decision-guide-databases-and-ai-search.png" alt-text="Vector indexing service decision guide":::


## Additional plugins

If you want to extend the functionality of your AI agent, you can use plugins to integrate with other Microsoft services. Here are some of the plugins that are available for Semantic Kernel:

| Plugin     | C#  | Python | Java | Description |
| ---------- | :-: | :----: | :--: | ----------- |
| Logic Apps | ✅  |   ✅   |  ✅  | Build workflows within Logic Apps using its available connectors and import them as plugins in Semantic Kernel. [Learn more](../concepts/plugins/adding-logic-apps-as-plugins.md). |
| Azure Container Apps Dynamic Sessions | ✅  |   ✅   |  ❌  | With dynamic sessions, you can recreate the Code Interpreter experience from the Assistants API by effortlessly spinning up Python containers where AI agents can execute Python code. [Learn more](/azure/container-apps/sessions). |
