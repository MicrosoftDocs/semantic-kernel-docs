---
title: Supported languages for Semantic Kernel
description: Learn which features are available for C#, Python, and Java.
author: matthewbolanos
ms.topic: reference
ms.author: mabolan
ms.date: 07/11/2023
ms.service: mssearch
---

# Supported Semantic Kernel languages

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

> [!Note]
> Skills are currently being renamed to plugins. This article has been updated to reflect the latest terminology, but some images and code samples may still refer to skills.

Semantic Kernel plans on providing support to the following languages:
> [!div class="checklist"]
> * C#
> * Python
> * Java ([coming soon](https://github.com/microsoft/semantic-kernel/tree/experimental-java))

While the overall architecture of the kernel is consistent across all languages, we made sure the SDK for each language follows common paradigms and styles in each language to make it feel native and easy to use.

## Available features

Today, not all features are available in all languages. The following tables show which features are available in each language. The __*️__ symbol indicates that the feature is partially implemented, please see the associated note column for more details. The ❌ symbol indicates that the feature is not yet available in that language; if you would like to see a feature implemented in a language, please consider [contributing to the project](./contributing.md) or [opening an issue](./contributing.md#reporting-issues).


### AI Services

| Services                          |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| TextGeneration                    | ✅ | ✅ | ✅ | Example: Text-Davinci-003 |
| TextEmbeddings                    | ✅ | ✅ | ✅ | Example: Text-Embeddings-Ada-002 |
| ChatCompletion                    | ✅ | ✅ | ❌ | Example: GPT4, Chat-GPT |
| Image Generation                  | ✅ | ❌ | ❌ | Example: Dall-E |

### AI service endpoints

| Endpoints                         |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| OpenAI                            | ✅ | ✅ | ✅ | |
| AzureOpenAI                       | ✅ | ✅ | ✅ | |
| Hugging Face Inference API        | *️ | ❌ | ❌ | Coming soon to Python, not all scenarios are covered for .NET |
| Hugging Face Local                | ❌ | ✅ | ❌ | |
| Custom                            | ✅ | *️ | ❌ | Requires the user to define the service schema in their application |

### Tokenizers

| Tokenizers                        |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| GPT2                              | ✅ | ✅ | ✅ | |
| GPT3                              | ✅ | ❌ | ❌ | |
| tiktoken                          | *️ | ❌ | ❌ | Coming soon to Python and C#. Can be manually added to Python via `pip install tiktoken` |

### Core plugins

| Plugins                           |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| TextMemorySkill                   | ✅ | ✅ | *️ | |
| ConversationSummarySkill          | ✅ | ✅ | ❌ | |
| FileIOSkill                       | ✅ | ✅ | ❌ | |
| HttpSkill                         | ✅ | ✅ | ❌ | |
| MathSkill                         | ✅ | ✅ | ❌ | |
| TextSkill                         | ✅ | ✅ | *️ | |
| TimeSkill                         | ✅ | ✅ | *️ | |
| WaitSkill                         | ✅ | ❌ | ❌ | |

### Planners

| Planners                          |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| Plan                              | ✅ | ✅ | ❌ | Need to port the Plan object model |
| BasicPlanner                      | ❌ | ✅ | ❌ | |
| ActionPlanner                     | ✅ | ❌ | ❌ | |
| SequentialPlanner                 | ✅ | ❌ | ❌ | |

### Connectors

| Connectors                        |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| Qdrant (Memory)                   | ✅ | ❌ | ❌ | |
| ChromaDb (Memory)                 | ❌ | *️ | ❌ | |
| Milvus (Memory)                   | ❌ | ❌ | ❌ | |
| Pinecone (Memory)                 | ✅ | ❌ | ❌ | |
| Weaviate (Memory)                 | ❌ | ❌ | ❌ | |
| CosmosDB (Memory)                 | ✅ | ❌ | ❌ | CosmosDB is not optimized for vector storage |
| Sqlite (Memory)                   | ✅ | ❌ | ❌ | Sqlite is not optimized for vector storage |
| Postgres (Memory)                 | ✅ | ❌ | ❌ | Vector optimized (required the [pgvector](https://github.com/pgvector/pgvector) extension) |
| Azure Cognitive Search            | ✅ | *️ | ❌ | |

### Plugins
| Plugins                        |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| MsGraph                           | ✅ | ❌ | ❌ | Contains plugins for OneDrive, Outlook, ToDos, and Organization Hierarchies |
| Document and data loading plugins (i.e. pdf, csv, docx, pptx)  | ✅ | ❌ | ❌ | Currently only supports Word documents |
| OpenAPI                           | ✅ | ❌ | ❌ | |
| Web search plugins (i.e. Bing, Google) | ✅ | ❌ | ❌ | |
| Text chunkers                     | *️ | *️ | ❌ | |


## Notes about the Python SDK

During the initial development phase, many Python best practices have been ignored in the interest of velocity and feature parity. The project is now going through a refactoring exercise to increase code quality.

To make the Kernel as lightweight as possible, the core pip package should have a minimal set of external dependencies. On the other hand, the SDK should not reinvent mature solutions already available, unless of major concerns.