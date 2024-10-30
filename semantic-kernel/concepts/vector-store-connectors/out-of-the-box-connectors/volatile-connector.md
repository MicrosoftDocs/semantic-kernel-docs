---
title: Using the Semantic Kernel Volatile (In-Memory) Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data using the Volatile (in-memory) Semantic Kernel supplied vector store.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Using the Volatile (In-Memory) connector (Preview)

::: zone pivot="programming-language-csharp"

> [!WARNING]
> The C# VolatileVectorStore is obsolete and has been replaced with a new package.  See [InMemory Connector](./inmemory-connector.md)

::: zone-end
::: zone pivot="programming-language-python"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end
::: zone pivot="programming-language-java"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end

## Overview

The Volatile Vector Store connector is a Vector Store implementation provided by Semantic Kernel that uses no external database and stores data in memory.
This Vector Store is useful for prototyping scenarios or where high-speed in-memory operations are required.

The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | In-memory dictionary                                                                                                             |
| Supported key property types      | Any type that can be compared                                                                                                    |
| Supported data property types     | Any type                                                                                                                         |
| Supported vector property types   | ReadOnlyMemory\<float\>                                                                                                          |
| Supported index types             | N/A                                                                                                                              |
| Supported distance functions      | N/A                                                                                                                              |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsFilterable supported?           | Yes                                                                                                                              |
| IsFullTextSearchable supported?   | Yes                                                                                                                              |
| StoragePropertyName supported?    | No, since storage is volatile and data reuse is therefore not possible, custom naming is not useful and not supported.           |

::: zone pivot="programming-language-csharp"

## Getting started

Add the Semantic Kernel Core nuget package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Core
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

```csharp
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder()
    .AddVolatileVectorStore();
```

```csharp
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddVolatileVectorStore();
```

You can construct a Volatile Vector Store instance directly.

```csharp
using Microsoft.SemanticKernel.Data;

var vectorStore = new VolatileVectorStore();
```

It is possible to construct a direct reference to a named collection.

```csharp
using Microsoft.SemanticKernel.Data;

var collection = new VolatileVectorStoreRecordCollection<string, Hotel>("skhotels");
```

::: zone-end
::: zone pivot="programming-language-python"

## Getting started

Install semantic kernel.

```cli
pip install semantic-kernel
```

You can then create a vector store instance using the `VolatileStore` class.

```python

from semantic_kernel.connectors.memory.volatile import VolatileStore

vector_store = VolatileStore()
```

You can also create a collection directly.

```python
from semantic_kernel.connectors.memory.volatile import VolatileCollection

collection = VolatileCollection(collection_name="skhotels", data_model_type=Hotel)
```

## Serialization

Since the Volatile connector has a simple dict as the internal storage mechanism it can store any data model that can be serialized to a dict.

For more details on this concept see the [serialization documentation](./../serialization.md).

::: zone-end
::: zone pivot="programming-language-java"
::: zone-end
