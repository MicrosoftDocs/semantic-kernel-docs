---
title: Using the Semantic Kernel Qdrant Vector Store connector (Experimental)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Qdrant.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Using the Qdrant connector (Experimental)

## Overview

The Qdrant Vector Store connector can be used to access and manage data in Qdrant. The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | Qdrant collection with payload indices for filterable data fields                                                                |
| Supported key property types      | ulong<br>Guid                                                                                                                    |
| Supported data property types     | string<br>int<br>long<br>double<br>float<br>bool<br>*and enumerables of each of these types*                                     |
| Supported vector property types   | ReadOnlyMemory\<float\><br>ReadOnlyMemory\<double\>                                                                              |
| Supported index types             | Hnsw                                                                                                                             |
| Supported distance functions      | CosineSimilarity<br>DotProductSimilarity<br>EuclideanDistance<br>ManhattanDistance                                               |
| Supports multiple vectors in a record | Yes (configurable)                                                                                                           |
| IsFilterable supported?           | Yes                                                                                                                              |
| IsFullTextSearchable supported?   | Yes                                                                                                                              |
| StoragePropertyName supported?    | Yes                                                                                                                              |

::: zone pivot="programming-language-csharp"

## Getting started

Add the Qdrant Vector Store connector nuget package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.Qdrant --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the to the `IServiceCollection` dependency injection container using extention methods provided by Semantic Kernel.

```csharp
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder()
    .AddQdrantVectorStore("localhost");

// Using IServiceCollection.
serviceCollection.AddQdrantVectorStore("localhost");
```

Extension methods are also provided that take no parameters. These require an instance of the `Qdrant.Client.QdrantClient` class to be separately registered with the dependency injection container.

```csharp
using Microsoft.SemanticKernel;
using Qdrant.Client;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton<QdrantClient>(sp => new QdrantClient("localhost"));
kernelBuilder.AddQdrantVectorStore();

// Using IServiceCollection.
serviceCollection.AddSingleton<QdrantClient>(sp => new QdrantClient("localhost"));
serviceCollection.AddQdrantVectorStore();
```

You can construct a Qdrant Vector Store instance directly.

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

var vectorStore = new QdrantVectorStore(new QdrantClient("localhost"));
```

It is possible to construct a direct reference to a named collection.

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

var collection = new QdrantVectorStoreRecordCollection<Hotel>(
    new QdrantClient("localhost"),
    "skhotels");
```

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end

## Data mapping

The Qdrant connector provides a default mapper when mapping data from the data model to storage.
Qdrant requires properties to be mapped into id, payload and vector(s) groupings.
The default mapper uses the model annotations or record definition to determine the type of each property and to do this mapping.

- The data model property annotated as a key will be mapped to the Qdrant point id.
- The data model properties annotated as data will be mapped to the Qdrant point payload object.
- The data model properties annotated as vectors will be mapped to the Qdrant point vector object.

### Property name override

For data properties and vector properties (if using named vectors mode), you can provide override field names to use in storage that is different to the
property names on the data model. This is not supported for keys, since a key has a fixed name in Qdrant. It is also not supported for vectors in *single
unnamed vector* mode, since the vector is stored under a fixed name.
::: zone pivot="programming-language-csharp"
The property name override is done by setting the `StoragePropertyName` option via the data model attributes or record definition.

Here is an example of a data model with `StoragePropertyName` set on its attributes and how that will be represented in Qdrant.

```csharp
using Microsoft.SemanticKernel;

public class Hotel
{
    [VectorStoreRecordKey]
    public ulong HotelId { get; set; }

    [VectorStoreRecordData(IsFilterable = true) { StoragePropertyName = "hotel_name" }]
    public string HotelName { get; set; }

    [VectorStoreRecordData(IsFullTextSearchable = true) { StoragePropertyName = "hotel_description" }]
    public string Description { get; set; }

    [VectorStoreRecordVector(4, IndexKind.Hnsw, DistanceFunction.CosineDistance) { StoragePropertyName = "hotel_description_embedding" }]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```json
{
    "id": 1,
    "payload": { "hotel_name": "Hotel Happy", "hotel_description": "A place where everyone can be happy." },
    "vector": {
        "hotel_description_embedding": [0.9, 0.1, 0.1, 0.1],
    }
}
```

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end

### Qdrant vector modes

Qdrant supports two modes for vector storage and the Qdrant Connector with default mapper supports both modes.
The default mode is *single unnamed vector*.

#### Single unnamed vector

With this option a collection may only contain a single vector and it will be unnamed in the storage model in Qdrant.
Here is an example of how an object is represented in Qdrant when using *single unnamed vector* mode:

::: zone pivot="programming-language-csharp"

```csharp
new Hotel
{
    HotelId = 1,
    HotelName = "Hotel Happy",
    Description = "A place where everyone can be happy.",
    DescriptionEmbedding = new float[3] { 0.9f, 0.1f, 0.1f, 0.1f }
}
```

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end

```json
{
    "id": 1,
    "payload": { "HotelName": "Hotel Happy", "Description": "A place where everyone can be happy." },
    "vector": [0.9, 0.1, 0.1, 0.1]
}
```

#### Named vectors

If using the named vectors mode, it means that each point in a collection may contain more than one vector, and each will be named.
Here is an example of how an object is represented in Qdrant when using *named vectors* mode:

::: zone pivot="programming-language-csharp"

```csharp
new Hotel
{
    HotelId = 1,
    HotelName = "Hotel Happy",
    Description = "A place where everyone can be happy.",
    HotelNameEmbedding = new float[3] { 0.9f, 0.5f, 0.5f, 0.5f }
    DescriptionEmbedding = new float[3] { 0.9f, 0.1f, 0.1f, 0.1f }
}
```

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end

```json
{
    "id": 1,
    "payload": { "HotelName": "Hotel Happy", "Description": "A place where everyone can be happy." },
    "vector": {
        "HotelNameEmbedding": [0.9, 0.5, 0.5, 0.5],
        "DescriptionEmbedding": [0.9, 0.1, 0.1, 0.1],
    }
}
```

To enable named vectors mode, pass this as an option when constructing a Vector Store or collection.
The same options can also be passed to any of the provided dependency injection container extensions methods.

::: zone pivot="programming-language-csharp"

```csharp
var vectorStore = new QdrantVectorStore(
    new QdrantClient("localhost"),
    new() { HasNamedVectors = true });

var collection = new QdrantVectorStoreRecordCollection<Hotel>(
    new QdrantClient("localhost"),
    "skhotels",
    new() { HasNamedVectors = true });
```

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end
