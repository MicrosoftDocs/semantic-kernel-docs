---
title: Semantic Kernel Vector Store code samples (Preview)
description: Lists code samples for the Semantic Kernel Vector Store abstractions and implementations
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Semantic Kernel Vector Store code samples (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone pivot="programming-language-csharp"

## End to end RAG sample with Vector Stores

This example is a standalone console application that demonstrates RAG using Semantic Kernel. The sample has the following characteristics:

1. Allows a choice of chat and embedding services
1. Allows a choice of vector databases
1. Reads the contents of one or more PDF files and creates a chunks for each section
1. Generates embeddings for each text chunk and upserts it to the chosen vector database
1. Registers the Vector Store as a Text Search plugin with the kernel
1. Invokes the plugin to augment the prompt provided to the AI model with more context

- [End to end RAG demo](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Demos/VectorStoreRAG/README.md)

## Simple Data Ingestion and Vector Search

For two very simple examples of how to do data ingestion into a vector store and do vector search, check out these
two examples, which use Qdrant and InMemory vector stores to demonstrate their usage.

- [Simple Vector Search](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_VectorSearch_Simple.cs)
- [Simple Data Ingestion](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_DataIngestion_Simple.cs)

## Common code with multiple stores

Vector stores may different in certain aspects, e.g. with regards to the types of their keys or the types of fields each support.
Even so, it is possible to write code that is agnostic to these differences.

For a data ingestion sample that demonstrates this, see:
- [MultiStore Data Ingestion](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_DataIngestion_MultiStore.cs)

For a vector search sample demonstrating the same concept see the following samples.
Each of these samples are referencing the same common code, and just differ on the type of
vector store they create to use with the common code.

- [Azure AI Search vector search with common code](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_VectorSearch_MultiStore_AzureAISearch.cs)
- [InMemory vector search with common code](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_VectorSearch_MultiStore_InMemory.cs)
- [Qdrant vector search with common code](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_VectorSearch_MultiStore_Qdrant.cs)
- [Redis vector search with common code](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_VectorSearch_MultiStore_Redis.cs)

## Supporting multiple vectors in the same record

The Vector Store abstractions support multiple vectors in the same record, for vector databases that support this.
The following sample shows how to create some records with multiple vectors, and pick the desired target vector
when doing a vector search.

- [Choosing a vector for search on a record with multiple vectors](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_VectorSearch_MultiVector.cs)

## Vector search with paging

When doing vector search with the Vector Store abstractions it's possible to use Top and Skip parameters to support paging, where e.g.
you need to build a service that reponds with a small set of results per request.

- [Vector search with paging](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_VectorSearch_Paging.cs)

> [!WARNING]
> Not all vector databases support Skip functionality natively for vector searches, so some connectors may have to fetch Skip + Top records and skip
> on the client side to simulate this behavior.

## Using the generic data model vs using a custom data model

It's possible to use the Vector Store abstractions without defining a data model and defining your schema via a record definition instead.
This example shows how you can create a vector store using a custom model and read using the generic data model or vice versa.

- [Generic data model interop](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_GenericDataModel_Interop.cs)

> [!TIP]
> For more information about using the generic data model, refer to [using Vector Store abstractions without defining your own data model](./generic-data-model.md).

## Using collections that were created and ingested using Langchain

It's possible to use the Vector Store abstractions to access collections that were created and ingested using a different sytem, e.g. Langchain.
There are various approaches that can be followed to make the interop work correctly. E.g.

1. Creating a data model that matches the storage schema that the Langchain implemenation used.
1. Using a custom mapper to map between the storage schema and data model.
1. Using a record definition with special storage property names for fields.

In the following sample, we show how to use these approaches to construct Langchain compatible Vector Store implementations.

- [VectorStore Langchain Interop](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_Langchain_Interop.cs)

For each vector store, there is a factory class that shows how to contruct the Langchain compatible Vector Store. See e.g.

- [AzureAISearchFactory](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStoreLangchainInterop/AzureAISearchFactory.cs)
- [PineconeFactory](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStoreLangchainInterop/PineconeFactory.cs)
- [QdrantFactory](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStoreLangchainInterop/QdrantFactory.cs)
- [RedisFactory](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStoreLangchainInterop/RedisFactory.cs)

In this sample, we also demonstrate a technique for having a single unified data model across different Vector Stores, where each Vector Store supports
different key types and may require different storage schemas.

We use a decorator class [MappingVectorStoreRecordCollection](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStoreLangchainInterop/MappingVectorStoreRecordCollection.cs)
that allows converting data models and key types. E.g. Qdrant only supports `Guid` and `ulong` key types, and Langchain uses the `Guid` key type when creating
a collection. Azure AI Search, Pinecone and Redis all support `string` keys. In the sample, we use the `MappingVectorStoreRecordCollection` to expose the Qdrant
Vector Store with a `string` key containing a guid instead of the key being a `Guid` type. This allows us to easily use all databases with
[one data model](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStoreLangchainInterop/LangchainDocument.cs).
Note that supplying `string` keys that do not contain guids to the decorated Qdrant Vector Store will not work, since the underlying database still
requires `Guid` keys.

::: zone-end
::: zone pivot="programming-language-python"

## End to end RAG sample with Azure AI Search Vector Store

This example is set of two scripts, the first showing the basics of settings up the Azure AI Search Vector Store and the second showing how to create a plugin from it and use that to perform RAG.

1. [Explication of the data model and how to setup Azure AI Search for this sample](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/memory/azure_ai_search_hotel_samples/step_0_data_model.py)
2. [Creating records, adding vectors, and upserting records in Azure AI Search](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/memory/azure_ai_search_hotel_samples/step_1_interact_with_the_collection.py)
3. [Use the same connection and data model to create custom functions that can then be used with auto function calling for advanced RAG](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/memory/azure_ai_search_hotel_samples/step_2_use_as_a_plugin.py)

## Simple Data Ingestion and Vector Search

We also have a sample that shows the basics from creating the collection, to adding records, to finally doing search, this can be started with different vector stores.

- [Simple Vector Search](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/memory/new_memory.py)

::: zone-end
::: zone pivot="programming-language-java"

## Simple Data Ingestion and Vector Search

For simple examples of how to do data ingestion into a vector store and do vector search, check out these examples, which make use of Azure AI Search, JDBC with PostgreSQL, Redis and In Memory vector stores.

- [Vector Search with Azure AI Search](https://github.com/microsoft/semantic-kernel-java/blob/main/samples/semantickernel-concepts/semantickernel-syntax-examples/src/main/java/com/microsoft/semantickernel/samples/syntaxexamples/memory/VectorStoreWithAzureAISearch.java)
- [Vector Search with JDBC](https://github.com/microsoft/semantic-kernel-java/blob/main/samples/semantickernel-concepts/semantickernel-syntax-examples/src/main/java/com/microsoft/semantickernel/samples/syntaxexamples/memory/VectorStoreWithJDBC.java)
- [Vector Search with Redis](https://github.com/microsoft/semantic-kernel-java/blob/main/samples/semantickernel-concepts/semantickernel-syntax-examples/src/main/java/com/microsoft/semantickernel/samples/syntaxexamples/memory/VectorStoreWithRedis.java)
- [Vector Search with in memory store](https://github.com/microsoft/semantic-kernel-java/blob/main/samples/semantickernel-concepts/semantickernel-syntax-examples/src/main/java/com/microsoft/semantickernel/samples/syntaxexamples/memory/InMemoryVolatileVectorStore.java)


::: zone-end
