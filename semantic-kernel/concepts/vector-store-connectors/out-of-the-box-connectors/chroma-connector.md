---
title: Using the Semantic Kernel Chroma Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in ChromaDB.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: article
ms.author: edvan
ms.date: 10/30/2026
ms.service: semantic-kernel
---

# Using the Chroma connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone pivot="programming-language-csharp"

## Overview

The Chroma Vector Store connector can be used to access and manage data in Chroma and Chroma Cloud. The connector is provided by the [.NET Community Toolkit](https://github.com/CommunityToolkit/AI) and is built on the [ChromaDotNet.Client](https://www.nuget.org/packages/ChromaDotNet.Client) library. The connector has the following characteristics.

| Feature Area                          | Support                                                                                                                                                                  |
| ------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Collection maps to                    | Chroma collection                                                                                                                                                        |
| Supported key property types          | <ul><li>string</li><li>Guid</li></ul>                                                                                                                                    |
| Supported data property types         | <ul><li>string</li><li>int</li><li>long</li><li>double</li><li>float</li><li>bool</li><li>DateTime</li><li>DateTimeOffset</li><li>DateOnly</li><li>*and arrays or lists of each of these types*</li></ul> |
| Supported vector property types       | <ul><li>ReadOnlyMemory\<float\></li><li>Embedding\<float\></li><li>float[]</li></ul>                                                                                     |
| Supported index types                 | Hnsw (any other index kind throws when the collection is constructed)                                                                                                    |
| Supported distance functions          | <ul><li>CosineSimilarity (default)</li><li>CosineDistance</li><li>DotProductSimilarity</li><li>NegativeDotProductSimilarity</li><li>EuclideanSquaredDistance</li><li>EuclideanDistance</li></ul> |
| Supported filter clauses              | LINQ expressions: `==`, `!=`, `<`, `<=`, `>`, `>=` (numbers only), `&&`, `\|\|`, `!`, `Contains` over a list and `Any` with `Contains` over array properties. `string.Contains` only works on the full-text indexed document property. Vector properties cannot be filtered. |
| Supports multiple vectors in a record | No                                                                                                                                                                       |
| IsIndexed supported?                  | Yes (all data properties can be used in filters, so the setting has no effect)                                                                                           |
| IsFullTextIndexed supported?          | Yes (string properties only)                                                                                                                                             |
| StorageName supported?                | Yes                                                                                                                                                                      |
| HybridSearch supported?               | Yes (Chroma Cloud only, requires a full-text indexed string property)                                                                                                    |

## Limitations

Notable Chroma connector functionality limitations.

| Feature Area                | Details                                                                                                                                           |
| --------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| Empty arrays and lists      | Chroma does not store empty lists, so an empty array or list is returned as `null`.                                                               |
| Array and list properties   | Require Chroma 1.5.0 or later.                                                                                                                    |
| Chroma Cloud search results | A vector search returns at most 300 results (`Skip` included), which is the default Chroma Cloud quota. Requesting more results fails with a quota error. |
| Ordering                    | Ordering of results is not supported.                                                                                                             |
| Null values                 | Chroma metadata has no null values, so a data property set to `null` is not stored, and is read back as `null`.                                   |
| Storage names               | A storage name cannot be empty, or start with `#`, `$` or `chroma:`.                                                                              |

## Getting started

Add the Chroma Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package CommunityToolkit.VectorData.Chroma --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the `IServiceCollection` dependency injection container using extension methods provided by the connector package.
The connection string is either the URI of the Chroma server, or `Endpoint=...;Token=...;Tenant=...;Database=...` (for example, for Chroma Cloud).

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder();
kernelBuilder.Services
    .AddChromaVectorStore("http://localhost:8000");
```

```csharp
using Microsoft.Extensions.DependencyInjection;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddChromaVectorStore("http://localhost:8000");
```

To connect to Chroma Cloud, the connection string has the API key, and the tenant and the database shown in the Chroma Cloud dashboard.

```csharp
using Microsoft.Extensions.DependencyInjection;

// Using IServiceCollection with ASP.NET Core and Chroma Cloud.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddChromaVectorStore("Endpoint=https://api.trychroma.com;Token=<api key>;Tenant=<tenant>;Database=<database>");
```

Extension methods that take no parameters are also provided. These require an instance of the `ChromaDB.Client.ChromaClient` class to be separately registered with the dependency injection container.

```csharp
using ChromaDB.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton<ChromaClient>(sp =>
    new ChromaClient(new ChromaConfigurationOptions("http://localhost:8000")));
kernelBuilder.Services.AddChromaVectorStore();
```

```csharp
using ChromaDB.Client;
using Microsoft.Extensions.DependencyInjection;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ChromaClient>(sp =>
    new ChromaClient(new ChromaConfigurationOptions("http://localhost:8000")));
builder.Services.AddChromaVectorStore();
```

You can construct a Chroma Vector Store instance directly.

```csharp
using ChromaDB.Client;
using CommunityToolkit.VectorData.Chroma;

var vectorStore = new ChromaVectorStore(
    new ChromaClient(new ChromaConfigurationOptions("http://localhost:8000")),
    ownsClient: true);
```

It is possible to construct a direct reference to a named collection.

```csharp
using ChromaDB.Client;
using CommunityToolkit.VectorData.Chroma;

var collection = new ChromaCollection<string, Hotel>(
    new ChromaClient(new ChromaConfigurationOptions("http://localhost:8000")),
    "skhotels",
    ownsClient: true);
```

## Data mapping

The Chroma connector provides a default mapper when mapping data from the data model to storage.
Chroma stores each record as an id, an embedding, optional metadata and an optional document.
The default mapper uses the model annotations or record definition to determine the type of each property and to do this mapping.

- The data model property annotated as a key will be mapped to the Chroma record id.
- The data model properties annotated as data will be mapped to the Chroma record metadata, except one: if exactly one `string` data property has `IsFullTextIndexed` set, it is stored as the Chroma record document instead. `string.Contains` filters on this property search the document.
- The data model property annotated as a vector will be mapped to the Chroma record embedding.

### Property name override

For data properties, you can provide override field names to use in storage that are different to the property names on the data model.
This is not supported for keys and vectors, since they have fixed names in Chroma.

The property name override is done by setting the `StorageName` option via the data model attributes or record definition.

Here is an example of a data model with `StorageName` set on its attributes and how that will be represented in Chroma.

```csharp
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreKey]
    public string HotelId { get; set; }

    [VectorStoreData(IsIndexed = true, StorageName = "hotel_name")]
    public string HotelName { get; set; }

    [VectorStoreData(IsFullTextIndexed = true, StorageName = "hotel_description")]
    public string Description { get; set; }

    [VectorStoreVector(4, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```json
{
    "id": "1",
    "metadata": { "hotel_name": "Hotel Happy" },
    "document": "A place where everyone can be happy.",
    "embedding": [0.9, 0.1, 0.1, 0.1]
}
```

## Hybrid search

On Chroma Cloud, the connector supports [hybrid search](../hybrid-search.md). It combines a vector search with a BM25 search of the keywords in a full-text indexed `string` property, and fuses the two rankings with reciprocal rank fusion.

When the connector creates a collection on Chroma Cloud, it also creates a BM25 index for each full-text indexed `string` property. A Chroma server outside Chroma Cloud has no BM25 indexes. There, `GetService` does not return the collection as `IKeywordHybridSearchable<TRecord>`, and `HybridSearchAsync` throws.

```csharp
using ChromaDB.Client;
using CommunityToolkit.VectorData.Chroma;
using Microsoft.Extensions.VectorData;

var vectorStore = new ChromaVectorStore(
    new ChromaClient(ChromaConfigurationOptions.FromConnectionString(
        "Endpoint=https://api.trychroma.com;Token=<api key>;Tenant=<tenant>;Database=<database>")),
    ownsClient: true);
var collection = vectorStore.GetCollection<string, Hotel>("skhotels");
await collection.EnsureCollectionExistsAsync();

// This snippet assumes searchVector is already provided, having been created using the embedding model of your choice.
var hybridSearchCollection = (IKeywordHybridSearchable<Hotel>)collection;
var searchResult = hybridSearchCollection.HybridSearchAsync(searchVector, ["happy", "place"], top: 3);

await foreach (var result in searchResult)
{
    Console.WriteLine($"{result.Record.HotelName}: {result.Score}");
}
```

::: zone-end
::: zone pivot="programming-language-python"

## Overview

The Chroma Vector Store connector can be used to access and manage data in Chroma. The connector has the
following characteristics.

| Feature Area                          | Support                                                                                          |
| ------------------------------------- | ------------------------------------------------------------------------------------------------ |
| Collection maps to                    | Chroma collection                                                                                |
| Supported key property types          | string                                                                                           |
| Supported data property types         | All types                                                                                        |
| Supported vector property types       | <ul><li>list[float]</li><li>list[int]</li><li>ndarray</li></ul>                                  |
| Supported index types                 | <ul><li>HNSW</li></ul>                                                                           |
| Supported distance functions          | <ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanSquaredDistance</li></ul> |
| Supported filter clauses              | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                  |
| Supports multiple vectors in a record | No                                                                                               |
| IsFilterable supported?               | Yes                                                                                              |
| IsFullTextSearchable supported?       | Yes                                                                                              |

## Limitations

Notable Chroma connector functionality limitations.

| Feature Area       | Workaround                                                                                                                |
| ------------------ | ------------------------------------------------------------------------------------------------------------------------- |
| Client-server mode | Use the client.HttpClient and pass the result to the `client` parameter, we do not support a AsyncHttpClient at this time |
| Chroma Cloud       | Unclear at this time, as Chroma Cloud is still in private preview                                                         |

## Getting Started

Add the Chroma Vector Store connector dependencies to your project.

```bash
pip install semantic-kernel[chroma]
```

You can then create the vector store.

```python
from semantic_kernel.connectors.chroma import ChromaStore

store = ChromaStore()
```

Alternatively, you can also pass in your own mongodb client if you want to have more control over the client construction:

```python
from chromadb import Client
from semantic_kernel.connectors.chroma import ChromaStore

client = Client(...)
store = ChromaStore(client=client)
```

You can also create a collection directly, without the store.

```python
from semantic_kernel.connectors.chroma import ChromaCollection

# `hotel` is a class created with the @vectorstoremodel decorator
collection = ChromaCollection(
    record_type=hotel,
    collection_name="my_collection",
)
```

## Serialization

The Chroma client returns both `get` and `search` results in tabular form, this means that there are between 3 and 5 lists being returned in a dict, the lists are 'keys', 'documents', 'embeddings', and optionally 'metadatas' and 'distances'. The Semantic Kernel Chroma connector will automatically convert this into a list of `dict` objects, which are then parsed back to your data model.

It could be very interesting performance wise to do straight serialization from this format into a dataframe-like structure as that saves a lot of rebuilding of the data structure. This is not done for you, even when using container mode, you would have to specify this yourself, for more details on this concept see the [serialization documentation](./../serialization.md).

::: zone-end
::: zone pivot="programming-language-java"

## Not supported

Not supported.

::: zone-end
