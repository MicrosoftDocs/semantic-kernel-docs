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

## Reading data that was ingested using Langchain

It's possible to use the Vector Store abstractions to access data that was ingested using a different sytem, e.g. Langchain.
The main requirement is to create a data model that matches the storage schema that the Langchain implementation used.
In cases where this is not possible, it's also possible to use a custom mapper instead.

The following two examples, show how to use the Azure AI Search and Redis vector stores to read data that was ingested using Langchain.

- [Azure AI Search](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_Langchain_Interop_AzureAISearch.cs)
- [Redis](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_Langchain_Interop_Redis.cs)

The next example shows how to use the Qdrant vector store with a custom mapper to read data that was ingested using Langchain.
A custom mapper is required in this case, because the built in Qdrant mapper does not support complex types and the metadata field
produced by the Langchain ingestion is a complex field.

- [Qdrant](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/VectorStore_Langchain_Interop_Qdrant.cs)

::: zone-end
::: zone pivot="programming-language-python"

## Coming soon

More coming soon.

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More coming soon.

::: zone-end
