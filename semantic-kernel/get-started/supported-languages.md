---
title: Supported languages for Semantic Kernel
description: Learn which features are available for C#, Python, and Java.
author: matthewbolanos
ms.topic: reference
ms.author: mabolan
ms.date: 07/11/2023
ms.service: semantic-kernel
---

# Supported Semantic Kernel languages

Semantic Kernel plans on providing support to the following languages:
> [!div class="checklist"]
> * C#
> * Python
> * Java

While the overall architecture of the kernel is consistent across all languages, we made sure the SDK for each language follows common paradigms and styles in each language to make it feel native and easy to use.

## Available features

Today, not all features are available in all languages. The following tables show which features are available in each language. The 🔄 symbol indicates that the feature is partially implemented, please see the associated note column for more details. The ❌ symbol indicates that the feature is not yet available in that language; if you would like to see a feature implemented in a language, please consider [contributing to the project](./contributing.md) or [opening an issue](./contributing.md#reporting-issues).


### AI Services

| Services                          |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| Text Generation                    | ✅ | ✅ | ✅ | Example: Text-Davinci-003 |
| Chat Completion                    | ✅ | ✅ | ✅ | Example: GPT4, Chat-GPT |
| Text Embeddings (Experimental)                   | ✅ | ✅ | ✅ | Example: Text-Embeddings-Ada-002 |
| Text to Image (Experimental)                 | ✅ | ❌ | ❌ | Example: Dall-E |
| Image to Text (Experimental)                 | ✅ | ❌ | ❌ | Example: Pix2Struct |
| Text to Audio (Experimental)                 | ✅ | ❌ | ❌ | Example: Text-to-speech |
| Audio to Text (Experimental)                 | ✅ | ❌ | ❌ | Example: Whisper |

### AI service endpoints

| Endpoints                         |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| OpenAI                            | ✅ | ✅ | ✅ | |
| Azure OpenAI                      | ✅ | ✅ | ✅ | |
| Other endpoints that suppoprt OpenAI APIs | ✅ | ✅ | ✅ | Includes Ollama, LLM Studio, Azure Model-as-a-service, etc. |
| Hugging Face Inference API        | 🔄 | ❌ | ❌ | Coming soon to Python, not all scenarios are covered for .NET |

### Memory Connectors

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

