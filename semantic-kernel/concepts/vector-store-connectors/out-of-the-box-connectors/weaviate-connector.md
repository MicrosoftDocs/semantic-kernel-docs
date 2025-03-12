---
title: Using the Semantic Kernel Weaviate Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Weaviate.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 09/23/2024
ms.service: semantic-kernel
---
# Using the Weaviate Vector Store connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

## Overview

The Weaviate Vector Store connector can be used to access and manage data in Weaviate. The connector has the following characteristics.

::: zone pivot="programming-language-csharp"

| Feature Area                          | Support                                                                                                                                                                               |
| ------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Collection maps to                    | Weaviate Collection                                                                                                                                                                   |
| Supported key property types          | Guid                                                                                                                                                                                  |
| Supported data property types         | <ul><li>string</li><li>byte</li><li>short</li><li>int</li><li>long</li><li>double</li><li>float</li><li>decimal</li><li>bool</li><li>DateTime</li><li>DateTimeOffset</li><li>Guid</li><li>*and enumerables of each of these types*</li></ul> |
| Supported vector property types       | <ul><li>ReadOnlyMemory\<float\></li><li>ReadOnlyMemory\<double\></li></ul>                                                                                                            |
| Supported index types                 | <ul><li>Hnsw</li><li>Flat</li><li>Dynamic</li></ul>                                                                                                                                   |
| Supported distance functions          | <ul><li>CosineDistance</li><li>NegativeDotProductSimilarity</li><li>EuclideanSquaredDistance</li><li>Hamming</li><li>ManhattanDistance</li></ul>                                      |
| Supported filter clauses              | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                                                                                                       |
| Supports multiple vectors in a record | Yes                                                                                                                                                                                   |
| IsFilterable supported?               | Yes                                                                                                                                                                                   |
| IsFullTextSearchable supported?       | Yes                                                                                                                                                                                   |
| StoragePropertyName supported?        | No, use `JsonSerializerOptions` and `JsonPropertyNameAttribute` instead. [See here for more info.](#data-mapping)                                                                     |
| HybridSearch supported?               | Yes                                                                                                                                                                                   |

::: zone-end
::: zone pivot="programming-language-python"

| Feature Area                          | Support                                                                                                                                                                               |
| ------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Collection maps to                    | Weaviate Collection                                                                                                                                                                   |
| Supported key property types          | Guid                                                                                                                                                                                  |
| Supported data property types         | <ul><li>string</li><li>byte</li><li>short</li><li>int</li><li>long</li><li>double</li><li>float</li><li>decimal</li><li>bool</li><li>*and iterables of each of these types*</li></ul> |
| Supported vector property types       | <ul><li>list[float]</li><li>list[int]</li><li>ndarray</li></ul>                                                                                                                       |
| Supported index types                 | <ul><li>Hnsw</li><li>Flat</li><li>Dynamic</li></ul>                                                                                                                                   |
| Supported distance functions          | <ul><li>CosineDistance</li><li>NegativeDotProductSimilarity</li><li>EuclideanSquaredDistance</li><li>Hamming</li><li>ManhattanDistance</li></ul>                                      |
| Supported filter clauses              | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                                                                                                       |
| Supports multiple vectors in a record | Yes                                                                                                                                                                                   |
| IsFilterable supported?               | Yes                                                                                                                                                                                   |
| IsFullTextSearchable supported?       | Yes                                                                                                                                                                                   |

::: zone-end
::: zone pivot="programming-language-java"
Coming soon.
::: zone-end

## Limitations

Notable Weaviate connector functionality limitations.

| Feature Area                                                           | Workaround                                          |
| ---------------------------------------------------------------------- | --------------------------------------------------- |
| Using the 'vector' property for single vector objects is not supported | Use of the 'vectors' property is supported instead. |

> [!WARNING]
> Weaviate requires collection names to start with an upper case letter. If you do not provide a collection name with an upper case letter, Weaviate will return an error when you try and create your collection. The error that you will see is `Cannot query field "mycollection" on type "GetObjectsObj". Did you mean "Mycollection"?` where `mycollection` is your collection name. In this example, if you change your collection name to `Mycollection` instead, this will fix the error.

## Getting started

Add the Weaviate Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.Weaviate --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.
The Weaviate vector store uses an `HttpClient` to communicate with the Weaviate service. There are two options for providing the URL/endpoint for the Weaviate service.
It can be provided via options or by setting the base address of the `HttpClient`.

This first example shows how to set the service URL via options.
Also note that these methods will retrieve an `HttpClient` instance for making calls to the Weaviate service from the dependency injection service provider.

```csharp
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder()
    .AddWeaviateVectorStore(options: new() { Endpoint = new Uri("http://localhost:8080/v1/") });
```

```csharp
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWeaviateVectorStore(options: new() { Endpoint = new Uri("http://localhost:8080/v1/") });
```

Overloads where you can specify your own `HttpClient` are also provided.
In this case it's possible to set the service url via the `HttpClient` `BaseAddress` option.

```csharp
using System.Net.Http;
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
using HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/v1/") };
kernelBuilder.AddWeaviateVectorStore(client);
```

```csharp
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
using HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/v1/") };
builder.Services.AddWeaviateVectorStore(client);
```

You can construct a Weaviate Vector Store instance directly as well.

```csharp
using System.Net.Http;
using Microsoft.SemanticKernel.Connectors.Weaviate;

var vectorStore = new WeaviateVectorStore(
    new HttpClient { BaseAddress = new Uri("http://localhost:8080/v1/") });
```

It is possible to construct a direct reference to a named collection.

```csharp
using System.Net.Http;
using Microsoft.SemanticKernel.Connectors.Weaviate;

var collection = new WeaviateVectorStoreRecordCollection<Hotel>(
    new HttpClient { BaseAddress = new Uri("http://localhost:8080/v1/") },
    "Skhotels");
```

If needed, it is possible to pass an Api Key, as an option, when using any of the above mentioned mechanisms, e.g.

```csharp
using Microsoft.SemanticKernel;

var kernelBuilder = Kernel
    .CreateBuilder()
    .AddWeaviateVectorStore(options: new() { Endpoint = new Uri("http://localhost:8080/v1/"), ApiKey = secretVar });
```

## Data mapping

The Weaviate Vector Store connector provides a default mapper when mapping from the data model to storage.
Weaviate requires properties to be mapped into id, payload and vectors groupings.
The default mapper uses the model annotations or record definition to determine the type of each property and to do this mapping.

- The data model property annotated as a key will be mapped to the Weaviate `id` property.
- The data model properties annotated as data will be mapped to the Weaviate `properties` object.
- The data model properties annotated as vectors will be mapped to the Weaviate `vectors` object.

The default mapper uses `System.Text.Json.JsonSerializer` to convert to the storage schema.
This means that usage of the `JsonPropertyNameAttribute` is supported if a different storage name to the
data model property name is required.

Here is an example of a data model with `JsonPropertyNameAttribute` set and how that will be represented in Weaviate.

```csharp
using System.Text.Json.Serialization;
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreRecordKey]
    public ulong HotelId { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public string HotelName { get; set; }

    [VectorStoreRecordData(IsFullTextSearchable = true)]
    public string Description { get; set; }

    [JsonPropertyName("HOTEL_DESCRIPTION_EMBEDDING")]
    [VectorStoreRecordVector(4, DistanceFunction.CosineDistance, IndexKind.QuantizedFlat)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```json
{
    "id": 1,
    "properties": { "HotelName": "Hotel Happy", "Description": "A place where everyone can be happy." },
    "vectors": {
        "HOTEL_DESCRIPTION_EMBEDDING": [0.9, 0.1, 0.1, 0.1],
    }
}
```

::: zone-end
::: zone pivot="programming-language-python"

## Getting Started

Add the Weaviate Vector Store connector dependencies to your project.

```bash
pip install semantic-kernel[weaviate]
```

You can then create the vector store, it uses environment settings to connect:

For using Weaviate Cloud:

- url: WEAVIATE_URL
- api_key: WEAVIATE_API_KEY

For using Weaviate Local (i.e. Weaviate in a Docker container):

- local_host: WEAVIATE_LOCAL_HOST
- local_port: WEAVIATE_LOCAL_PORT
- local_grpc_port: WEAVIATE_LOCAL_GRPC_PORT

If you want to use embedded:

- use_embed: WEAVIATE_USE_EMBED

These should be set exclusively, so only one set of the above is present, otherwise it will raise an exception.

```python
from semantic_kernel.connectors.memory.weaviate import WeaviateStore

store = WeaviateStore()
```

Alternatively, you can also pass in your own mongodb client if you want to have more control over the client construction:

```python
import weaviate
from semantic_kernel.connectors.memory.weaviate import WeaviateStore

client = weaviate.WeaviateAsyncClient(...)
store = WeaviateStore(async_client=client)
```

You can also create a collection directly, without the store.

```python
from semantic_kernel.connectors.memory.weaviate import WeaviateCollection

# `hotel` is a class created with the @vectorstoremodel decorator
collection = WeaviateCollection(
    collection_name="my_collection",
    data_model_type=hotel
)
```

## Serialization

The Weaviate client returns it's own objects which are parsed and turned into dicts in the regular flow, for more details on this concept see the [serialization documentation](./../serialization.md).

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end
