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

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

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
| Supported vector property types   | ReadOnlyMemory\<float\>                                                                                                          |
| Supported index types             | Flat                                                                                                                             |
| Supported distance functions      | <ul><li>CosineSimilarity</li><li>CosineDistance</li><li>DotProductSimilarity</li><li>EuclideanDistance</li></ul>                 |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsFilterable supported?           | Yes                                                                                                                              |
| IsFullTextSearchable supported?   | Yes                                                                                                                              |
| StoragePropertyName supported?    | No, since storage is in-memory and data reuse is therefore not possible, custom naming is not applicable.                        |

## Getting started

Add the Semantic Kernel Core nuget package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.InMemory --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

```csharp
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder()
    .AddInMemoryVectorStore();
```

```csharp
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

var collection = new InMemoryVectorStoreRecordCollection<string, Hotel>("skhotels");
```

## Key and Vector property lookup

By default the In-Memory Vector Store connector will read the values of keys and vectors using
reflection. The keys and vectors are assumed to be direct properties on the data model.

If a data model is required that has a structure where keys and vectors are not direct properties
of the data model, it is possible to supply functions to read the values of these.

When using this, it is also required to supply a `VectorStoreRecordDefinition` so that information
about vector dimension size and distance function can be communicated to the In-Memory vector store.

```csharp
var collection = new InMemoryVectorStoreRecordCollection<string, MyDataModel>(
    "mydata",
    new()
    {
        VectorStoreRecordDefinition = vectorStoreRecordDefinition,
        KeyResolver = (record) => record.Key,
        VectorResolver = (vectorName, record) => record.Vectors[vectorName]
    });

private class MyDataModel
{
    public string Key { get; set; }
    public Dictionary<string, ReadOnlyMemory<float>> Vectors { get; set; }
}
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
from semantic_kernel.connectors.memory.in_memory import InMemoryVectorStore

vector_store = InMemoryVectorStore()
vector_collection = vector_store.get_collection("collection_name", DataModel)
```

It is possible to construct a direct reference to a named collection.

```python
from semantic_kernel.connectors.memory.in_memory import InMemoryCollection

vector_collection = InMemoryCollection("collection_name", DataModel)
```

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end
