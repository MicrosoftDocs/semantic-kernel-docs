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

::: zone-end
::: zone pivot="programming-language-python"

## Coming soon

More coming soon.

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More coming soon.

::: zone-end
