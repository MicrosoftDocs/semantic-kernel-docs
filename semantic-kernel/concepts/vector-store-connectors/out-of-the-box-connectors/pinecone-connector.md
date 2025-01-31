---
title: Using the Semantic Kernel Pinecone Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Pinecone.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Using the Pinecone connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone pivot="programming-language-csharp"

## Overview

The Pinecone Vector Store connector can be used to access and manage data in Pinecone. The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | Pinecone serverless Index                                                                                                        |
| Supported key property types      | string                                                                                                                           |
| Supported data property types     | <ul><li>string</li><li>int</li><li>long</li><li>double</li><li>float</li><li>bool</li><li>decimal</li><li>*enumerables of type* string</li></ul> |
| Supported vector property types   | ReadOnlyMemory\<float\>                                                                                                          |
| Supported index types             | PGA (Pinecone Graph Algorithm)                                                                                                   |
| Supported distance functions      | <ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanSquaredDistance</li></ul>                                 |
| Supported filter clauses          | <ul><li>EqualTo</li></ul>                                                                                                        |
| Supports multiple vectors in a record | No                                                                                                                           |
| IsFilterable supported?           | Yes                                                                                                                              |
| IsFullTextSearchable supported?   | No                                                                                                                               |
| StoragePropertyName supported?    | Yes                                                                                                                              |

## Getting started

Add the Pinecone Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.Pinecone --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

```csharp
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder()
    .AddPineconeVectorStore(pineconeApiKey);
```

```csharp
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddPineconeVectorStore(pineconeApiKey);
```

Extension methods that take no parameters are also provided. These require an instance of the `PineconeClient` to be separately registered with the dependency injection container.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using PineconeClient = Pinecone.PineconeClient;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton<PineconeClient>(
    sp => new PineconeClient(pineconeApiKey));
kernelBuilder.AddPineconeVectorStore();
```

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using PineconeClient = Pinecone.PineconeClient;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<PineconeClient>(
    sp => new PineconeClient(pineconeApiKey));
builder.Services.AddPineconeVectorStore();
```

You can construct a Pinecone Vector Store instance directly.

```csharp
using Microsoft.SemanticKernel.Connectors.Pinecone;
using PineconeClient = Pinecone.PineconeClient;

var vectorStore = new PineconeVectorStore(
    new PineconeClient(pineconeApiKey));
```

It is possible to construct a direct reference to a named collection.

```csharp
using Microsoft.SemanticKernel.Connectors.Pinecone;
using PineconeClient = Pinecone.PineconeClient;

var collection = new PineconeVectorStoreRecordCollection<Hotel>(
    new PineconeClient(pineconeApiKey),
    "skhotels");
```

## Index Namespace

The Vector Store abstraction does not support a multi tiered record grouping mechanism. Collections in the abstraction map to a Pinecone serverless index
and no second level exists in the abstraction. Pinecone does support a second level of grouping called namespaces.

By default the Pinecone connector will pass null as the namespace for all operations. However it is possible to pass a single namespace to the
Pinecone collection when constructing it and use this instead for all operations.

```csharp
using Microsoft.SemanticKernel.Connectors.Pinecone;
using PineconeClient = Pinecone.PineconeClient;

var collection = new PineconeVectorStoreRecordCollection<Hotel>(
    new PineconeClient(pineconeApiKey),
    "skhotels",
    new() { IndexNamespace = "seasidehotels" });
```

## Data mapping

The Pinecone connector provides a default mapper when mapping data from the data model to storage.
Pinecone requires properties to be mapped into id, metadata and values groupings.
The default mapper uses the model annotations or record definition to determine the type of each property and to do this mapping.

- The data model property annotated as a key will be mapped to the Pinecone id property.
- The data model properties annotated as data will be mapped to the Pinecone metadata object.
- The data model property annotated as a vector will be mapped to the Pinecone vector property.

### Property name override

For data properties, you can provide override field names to use in storage that is different to the
property names on the data model. This is not supported for keys, since a key has a fixed name in Pinecone.
It is also not supported for vectors, since the vector is stored under a fixed name `values`.
The property name override is done by setting the `StoragePropertyName` option via the data model attributes or record definition.

Here is an example of a data model with `StoragePropertyName` set on its attributes and how that will be represented in Pinecone.

```csharp
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreRecordKey]
    public ulong HotelId { get; set; }

    [VectorStoreRecordData(IsFilterable = true, StoragePropertyName = "hotel_name")]
    public string HotelName { get; set; }

    [VectorStoreRecordData(IsFullTextSearchable = true, StoragePropertyName = "hotel_description")]
    public string Description { get; set; }

    [VectorStoreRecordVector(Dimensions: 4, DistanceFunction.CosineDistance, IndexKind.Hnsw)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```json
{
    "id": "h1", 
    "values": [0.9, 0.1, 0.1, 0.1], 
    "metadata": { "hotel_name": "Hotel Happy", "hotel_description": "A place where everyone can be happy." }
}
```

::: zone-end
::: zone pivot="programming-language-python"

The Pinecone connector is not yet available in Python.

::: zone-end
::: zone pivot="programming-language-java"

The Pinecone connector is not yet available in Java.

::: zone-end
