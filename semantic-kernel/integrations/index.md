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

## Available features

Today, not all features are available in all languages. The following tables show which features are available in each language. The 🔄 symbol indicates that the feature is partially implemented, please see the associated note column for more details. The ❌ symbol indicates that the feature is not yet available in that language; if you would like to see a feature implemented in a language, please consider [contributing to the project](./contributing.md) or [opening an issue](./contributing.md#reporting-issues).


### AI Services

| Services                          |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| TextGeneration                    | ✅ | ✅ | ✅ | Example: Text-Davinci-003 |
| TextEmbeddings                    | ✅ | ✅ | ✅ | Example: Text-Embeddings-Ada-002 |
| ChatCompletion                    | ✅ | ✅ | ✅ | Example: GPT4, Chat-GPT |
| Image Generation                  | ✅ | ❌ | ❌ | Example: Dall-E |

### AI service endpoints

| Endpoints                         |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| OpenAI                            | ✅ | ✅ | ✅ | |
| AzureOpenAI                       | ✅ | ✅ | ✅ | |
| Hugging Face Inference API        | 🔄 | ❌ | ❌ | Coming soon to Python, not all scenarios are covered for .NET |
| Hugging Face Local                | ❌ | ✅ | ❌ | |
| Custom                            | ✅ | 🔄 | ❌ | Requires the user to define the service schema in their application |

### Tokenizers

| Tokenizers                        |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| GPT2                              | ✅ | ✅ | ✅ | |
| GPT3                              | ✅ | ❌ | ❌ | |
| tiktoken                          | 🔄 | ❌ | ❌ | Coming soon to Python and C#. Can be manually added to Python via `pip install tiktoken` |

### Core plugins

| Plugins                           |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| TextMemoryPlugin                   | ✅ | ✅ | 🔄 | |
| ConversationSummaryPlugin          | ✅ | ✅ | ✅ | |
| FileIOPlugin                       | ✅ | ✅ | ✅ | |
| HttpPlugin                         | ✅ | ✅ | ✅ | |
| MathPlugin                         | ✅ | ✅ | ✅ | |
| TextPlugin                         | ✅ | ✅ | ✅ | |
| TimePlugin                         | ✅ | ✅ | ✅ | |
| WaitPlugin                         | ✅ | ✅ | ✅ | |

### Planners

| Planners                          |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| Plan Object Model                 | ✅ | ✅ | 🔄 | |
| BasicPlanner                      | ❌ | ✅ | ❌ | |
| ActionPlanner                     | ✅ | ✅ | 🔄 | In development|
| SequentialPlanner                 | ✅ | ✅ | 🔄 | In development|
| StepwisePlanner                   | ✅ | ✅ | ❌ | |

### Connectors

| Memory Connectors        |  C#  | Python | Java | Notes |
|--------------------------|:----:|:------:|:----:|-------|
| Azure AI Search   | ✅ | ✅ | ✅ | |
| Chroma                   | ✅ | ✅ | ❌ | |
| DuckDB                   | ✅ | ❌ | ❌ | |
| Milvus                   | 🔄 | ✅ | ❌ | |
| Pinecone                 | ✅ | ✅ | ❌ | |
| Postgres                 | ✅ | ✅ | ❌ | Vector optimization requires [pgvector](https://github.com/pgvector/pgvector) |
| Qdrant                   | ✅ | 🔄 | ❌ | In feature branch for review |
| Redis                    | ✅ | 🔄 | ❌ | Vector optimization requires [RediSearch](https://redis.io/docs/stack/search) |
| Sqlite                   | ✅ | ❌ | 🔄 | Vector optimization requires [sqlite-vss](https://github.com/asg017/sqlite-vss) |
| Weaviate                 | ✅ | ✅ | ❌ | Currently supported on Python 3.9+, 3.8 coming soon |

### Plugins
| Plugins                           |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| MsGraph                           | ✅ | ❌ | ❌ | Contains plugins for OneDrive, Outlook, ToDos, and Organization Hierarchies |
| Document and data loading plugins (i.e. pdf, csv, docx, pptx)  | ✅ | ❌ | ❌ | Currently only supports Word documents |
| OpenAPI                           | ✅ | ❌ | ❌ | |
| Web search plugins (i.e. Bing, Google) | ✅ | ✅ | ❌ | |
| Text chunkers                     | 🔄 | 🔄 | ❌ | |


