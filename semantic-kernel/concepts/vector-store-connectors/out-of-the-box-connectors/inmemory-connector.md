---
title: Using the Semantic Kernel In-Memory Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in an in-memory Semantic Kernel supplied vector store.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 11/10/2024
ms.service: semantic-kernel
---
# Using the In-Memory connector (Preview)

::: zone pivot="programming-language-csharp"

> [!WARNING]
> The In-Memory Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end
::: zone pivot="programming-language-python"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end
::: zone pivot="programming-language-java"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end

::: zone pivot="programming-language-csharp"

## Overview

The In-Memory Vector Store connector is a Vector Store implementation provided by Semantic Kernel that uses no external database and stores data in memory.
This Vector Store is useful for prototyping scenarios or where high-speed in-memory operations are required.

The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | In-memory dictionary                                                                                                             |
| Supported key property types      | Any type that can be compared                                                                                                    |
| Supported data property types     | Any type                                                                                                                         |
| Supported vector property types   | <ul><li>ReadOnlyMemory\<float\></li><li>Embedding\<float\></li><li>float[]</li></ul>                                             |
| Supported index types             | Flat                                                                                                                             |
| Supported distance functions      | <ul><li>CosineSimilarity</li><li>CosineDistance</li><li>DotProductSimilarity</li><li>EuclideanDistance</li></ul>                 |
| Supported filter clauses          | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                                                  |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsIndexed supported?              | Yes                                                                                                                              |
| IsFullTextIndexed supported?      | Yes                                                                                                                              |
| StorageName supported?            | No, since storage is in-memory and data reuse is therefore not possible, custom naming is not applicable.                        |
| HybridSearch supported?           | No                                                                                                                               |

## Getting started

Add the Semantic Kernel Core nuget package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.InMemory --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder();
kernelBuilder.Services
    .AddInMemoryVectorStore();
```

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInMemoryVectorStore();
```

You can construct an InMemory Vector Store instance directly.

```csharp
using Microsoft.SemanticKernel.Connectors.InMemory;

var vectorStore = new InMemoryVectorStore();
```

It is possible to construct a direct reference to a named collection.

```csharp
using Microsoft.SemanticKernel.Connectors.InMemory;

var collection = new InMemoryCollection<string, Hotel>("skhotels");
```

::: zone-end
::: zone pivot="programming-language-python"

## Overview

The In-Memory Vector Store connector is a Vector Store implementation provided by Semantic Kernel that uses no external database and stores data in memory.
This Vector Store is useful for prototyping scenarios or where high-speed in-memory operations are required.

The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | In-memory dictionary                                                                                                             |
| Supported key property types      | Any that is allowed to be a dict key, see the python documentation for details [here](https://docs.python.org/3/library/stdtypes.html#typesmapping)                                                                                                    |
| Supported data property types     | Any type                                                                                                                         |
| Supported vector property types   | list[float \| int] \| numpy array                                                                                                       |
| Supported index types             | Flat                                                                                                                              |
| Supported distance functions      | <ul><li>Cosine Similarity</li><li>Cosine Distance</li><li>Dot Product Similarity</li><li>Euclidean Distance</li><li>Euclidean Squared Distance</li><li>Manhattan Distance</li><li>Hamming Distance</li></ul> |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| is_filterable supported?           | Yes                                                                                                                              |
| is_full_text_searchable supported?   | Yes                                                                                                                              |


## Getting started

Add the Semantic Kernel package to your project.

```cmd
pip install semantic-kernel
```

You can create the store and collections from there, or create the collections directly.

In the snippets below, it is assumed that you have a data model class defined named 'DataModel'.

```python
from semantic_kernel.connectors.in_memory import InMemoryVectorStore

vector_store = InMemoryVectorStore()
vector_collection = vector_store.get_collection(record_type=DataModel, collection_name="collection_name")
```

It is possible to construct a direct reference to a named collection.

```python
from semantic_kernel.connectors.in_memory import InMemoryCollection

vector_collection = InMemoryCollection(record_type=DataModel, collection_name="collection_name")
```

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end
