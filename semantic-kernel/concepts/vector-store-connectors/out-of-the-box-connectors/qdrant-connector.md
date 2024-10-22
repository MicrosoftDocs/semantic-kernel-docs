---
title: Using the Semantic Kernel Qdrant Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Qdrant.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Using the Qdrant connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

## Overview

The Qdrant Vector Store connector can be used to access and manage data in Qdrant. The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | Qdrant collection with payload indices for filterable data fields                                                                |
| Supported key property types      | <ul><li>ulong</li><li>Guid</li></ul>                                                                                             |
| Supported data property types     | <ul><li>string</li><li>int</li><li>long</li><li>double</li><li>float</li><li>bool</li><li>*and enumerables of each of these types*</li></ul> |
| Supported vector property types   | ReadOnlyMemory\<float\>                                                                                                          |
| Supported index types             | Hnsw                                                                                                                             |
| Supported distance functions      | <ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanDistance</li><li>ManhattanDistance</li></ul>              |
| Supports multiple vectors in a record | Yes (configurable)                                                                                                           |
| IsFilterable supported?           | Yes                                                                                                                              |
| IsFullTextSearchable supported?   | Yes                                                                                                                              |
| StoragePropertyName supported?    | Yes                                                                                                                              |

::: zone pivot="programming-language-csharp"

## Getting started

Add the Qdrant Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.Qdrant --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

```csharp
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder()
    .AddQdrantVectorStore("localhost");
```

```csharp
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddQdrantVectorStore("localhost");
```

Extension methods that take no parameters are also provided. These require an instance of the `Qdrant.Client.QdrantClient` class to be separately registered with the dependency injection container.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Qdrant.Client;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton<QdrantClient>(sp => new QdrantClient("localhost"));
kernelBuilder.AddQdrantVectorStore();
```

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Qdrant.Client;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<QdrantClient>(sp => new QdrantClient("localhost"));
builder.Services.AddQdrantVectorStore();
```

You can construct a Qdrant Vector Store instance directly.

```csharp
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

var vectorStore = new QdrantVectorStore(new QdrantClient("localhost"));
```

It is possible to construct a direct reference to a named collection.

```csharp
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

var collection = new QdrantVectorStoreRecordCollection<Hotel>(
    new QdrantClient("localhost"),
    "skhotels");
```

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

The property name override is done by setting the `StoragePropertyName` option via the data model attributes or record definition.

Here is an example of a data model with `StoragePropertyName` set on its attributes and how that will be represented in Qdrant.

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

    [VectorStoreRecordVector(4, DistanceFunction.CosineDistance, IndexKind.Hnsw, StoragePropertyName = "hotel_description_embedding")]
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

## Getting started

Install semantic kernel with the qdrant extras, which includes the [qdrant client](https://github.com/qdrant/qdrant-client).

```cli
pip install semantic-kernel[qdrant]
```

You can then create a vector store instance using the `QdrantStore` class, this will create a AsyncQdrantClient using the environment variables `QDRANT_URL`, `QDRANT_API_KEY`, `QDRANT_HOST`, `QDRANT_PORT`, `QDRANT_GRPC_PORT`, `QDRANT_PATH`, `QDRANT_LOCATION` and `QDRANT_PREFER_GRPS` to connect to the Qdrant instance, those values can also be supplied directly. If nothing is supplied it falls back to `location=:memory:`.

```python

from semantic_kernel.connectors.memory.qdrant import QdrantStore

vector_store = QdrantStore()
```

You can also create the vector store with your own instance of the qdrant client.

```python
from qdrant_client.async_qdrant_client import AsyncQdrantClient
from semantic_kernel.connectors.memory.qdrant import QdrantStore

client = AsyncQdrantClient(host='localhost', port=6333)
vector_store = QdrantStore(client=client)
```

You can also create a collection directly.

```python
from semantic_kernel.connectors.memory.qdrant import QdrantCollection

collection = QdrantCollection(collection_name="skhotels", data_model_type=hotel)
```

## Serialization

The Qdrant connector uses a model called `PointStruct` for reading and writing to the store. This can be imported from `from qdrant_client.models import PointStruct`. The serialization methods expects a output of a list of PointStruct objects, and the deserialization method recieves a list of PointStruct objects.

There are some special considerations for this that have to do with named or unnamed vectors, see below.

For more details on this concept see the [serialization documentation](./../serialization.md).

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
    DescriptionEmbedding = new float[4] { 0.9f, 0.1f, 0.1f, 0.1f }
};
```

```json
{
    "id": 1,
    "payload": { "HotelName": "Hotel Happy", "Description": "A place where everyone can be happy." },
    "vector": [0.9, 0.1, 0.1, 0.1]
}
```

::: zone-end
::: zone pivot="programming-language-python"

```python
Hotel(
    hotel_id = 1,
    hotel_name = "Hotel Happy",
    description = "A place where everyone can be happy.",
    description_embedding = [0.9f, 0.1f, 0.1f, 0.1f],
)
```

```python
from qdrant_client.models import PointStruct

PointStruct(
    id=1,
    payload={ "hotel_name": "Hotel Happy", "description": "A place where everyone can be happy." },
    vector=[0.9, 0.1, 0.1, 0.1],
)
```
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end


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
    HotelNameEmbedding = new float[4] { 0.9f, 0.5f, 0.5f, 0.5f }
    DescriptionEmbedding = new float[4] { 0.9f, 0.1f, 0.1f, 0.1f }
};
```

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

::: zone-end
::: zone pivot="programming-language-python"

```python
Hotel(
    hotel_id = 1,
    hotel_name = "Hotel Happy",
    description = "A place where everyone can be happy.",
    hotel_name_embedding = [0.9f, 0.5f, 0.5f, 0.5f],
    description_embedding = [0.9f, 0.1f, 0.1f, 0.1f],
)
```

```python
from qdrant_client.models import PointStruct

PointStruct(
    id=1,
    payload={ "hotel_name": "Hotel Happy", "description": "A place where everyone can be happy." },
    vector={
        "hotel_name_embedding": [0.9, 0.5, 0.5, 0.5],
        "description_embedding": [0.9, 0.1, 0.1, 0.1],
    },
)
```

::: zone-end
::: zone pivot="programming-language-java"
::: zone-end

To enable named vectors mode, pass this as an option when constructing a Vector Store or collection.
The same options can also be passed to any of the provided dependency injection container extension methods.

::: zone pivot="programming-language-csharp"

```csharp
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

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

In python the default value for `named_vectors` is True, but you can also disable this as shown below.

```python
from semantic_kernel.connectors.memory.qdrant import QdrantCollection

collection = QdrantCollection(
    collection_name="skhotels", 
    data_model_type=Hotel, 
    named_vectors=False,
)
```
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end
