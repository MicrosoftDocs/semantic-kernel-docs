---
title: Using the Semantic Kernel Chroma Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in ChromaDB.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: article
ms.author: edvan
ms.date: 10/09/2026
ms.service: semantic-kernel
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                  | C# | Python | Java | Notes                                  |
  |--------------------------|:--:|:------:|:----:|----------------------------------------|
  | Overview                 | ✅ |   ✅   |  ❌  | No Java connector                      |
  | Limitations              | ✅ |   ✅   |  ❌  |                                        |
  | Getting started          | ✅ |   ✅   |  ❌  |                                        |
  | Data mapping             | ✅ |   ❌   |  ❌  | C#-specific, Python has Serialization  |
  | Property name override   | ✅ |   ❌   |  ❌  | C#-specific                            |
  | Hybrid search            | ✅ |   ❌   |  ❌  | C#-specific                            |
  | Serialization            | ❌ |   ✅   |  ❌  | Python-specific                        |
-->

# Using the Chroma connector (Preview)

::: zone pivot="programming-language-csharp"

> [!WARNING]
> The Chroma Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

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

The Chroma Vector Store connector can be used to access and manage data in Chroma and Chroma Cloud. The connector has the following characteristics.

| Feature Area                          | Support                                                                                                                          |
| ------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| Collection maps to                    | Chroma collection                                                                                                                |
| Supported key property types          | <ul><li>string</li><li>Guid</li></ul>                                                                                            |
| Supported data property types         | <ul><li>string</li><li>int</li><li>long</li><li>double</li><li>float</li><li>bool</li><li>DateTime</li><li>DateTimeOffset</li><li>DateOnly</li><li>*and arrays and lists of each of these types*</li></ul> |
| Supported vector property types       | <ul><li>ReadOnlyMemory\<float\></li><li>Embedding\<float\></li><li>float[]</li></ul>                                             |
| Supported index types                 | Hnsw                                                                                                                             |
| Supported distance functions          | <ul><li>CosineSimilarity (default)</li><li>CosineDistance</li><li>DotProductSimilarity</li><li>NegativeDotProductSimilarity</li><li>EuclideanDistance</li><li>EuclideanSquaredDistance</li></ul> |
| Supported filter clauses              | <ul><li>==</li><li>!=</li><li><, <=, >, >= <ul><li>Only on numbers</li></ul></li><li>&&, \|\|, !</li><li>List.Contains() <ul><li>When checking if the model property is in the list</li><li>When checking if an array or list model property contains the value</li></ul></li><li>Enumerable.Any() <ul><li>Only with List.Contains(), when checking if an array or list model property contains any of the values in the list</li></ul></li><li>string.Contains() <ul><li>Only on the property stored as the Chroma document, see [Data mapping](#data-mapping)</li><li>Only joined to the other conditions with &&</li></ul></li><li>Filters on the key <ul><li>Only == and List.Contains()</li><li>Only joined to the other conditions with &&</li></ul></li></ul> |
| Supports multiple vectors in a record | No                                                                                                                               |
| IsIndexed supported?                  | Not needed, every data property can be used in filters                                                                           |
| IsFullTextIndexed supported?          | Yes                                                                                                                              |
| StorageName supported?                | Yes                                                                                                                              |
| HybridSearch supported?               | Yes, on Chroma Cloud only. See [Hybrid search](#hybrid-search).                                                                  |

## Limitations

Notable Chroma connector functionality limitations.

- Chroma metadata has no null values. A data property with a null value is not stored, and is read back as null.
- Chroma does not store empty arrays and lists, so they are read back as null.
- Array and list data properties need Chroma 1.5.0 or later.
- `GetAsync` with a filter does not support ordering.
- On Chroma Cloud, `top` and `Skip` of a vector search can add up to at most 300, the default quota of Chroma Cloud. Beyond that, Chroma Cloud returns a quota error.

## Getting started

Add the Chroma Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package CommunityToolkit.VectorData.Chroma --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the `IServiceCollection` dependency injection container using extension methods provided by the connector package.

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

The string is a connection string. To connect to Chroma Cloud, it also contains an API key, and the tenant and the database shown in the Chroma Cloud dashboard.

```csharp
using Microsoft.Extensions.DependencyInjection;

// Using IServiceCollection with ASP.NET Core.
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
kernelBuilder.Services.AddSingleton<ChromaClient>(sp => new ChromaClient(new ChromaConfigurationOptions("http://localhost:8000")));
kernelBuilder.Services.AddChromaVectorStore();
```

```csharp
using ChromaDB.Client;
using Microsoft.Extensions.DependencyInjection;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ChromaClient>(sp => new ChromaClient(new ChromaConfigurationOptions("http://localhost:8000")));
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
Chroma stores each record as an id, an embedding, metadata and a document.
The default mapper uses the model annotations or record definition to determine the type of each property and to do this mapping.

- The data model property annotated as a key will be mapped to the Chroma record id.
- The data model properties annotated as data will be mapped to the Chroma record metadata.
- The data model property annotated as a vector will be mapped to the Chroma record embedding.
- If exactly one `string` data property has `IsFullTextIndexed` set, it will also be mapped to the Chroma record document. `string.Contains` filters on this property search the document. On Chroma Cloud, a text longer than 8,182 bytes is stored in the document only, since Chroma Cloud does not accept longer metadata values.

### Property name override

For data properties, you can provide override field names to use in storage that are different from the property names on the data model. This is not supported for keys, since a key is stored as the Chroma record id, or for vectors, since a Chroma record has a single unnamed embedding.

The property name override is done by setting the `StorageName` option via the data model attributes or record definition.

Here is an example of a data model with `StorageName` set on its attributes and how that will be represented in Chroma.

```csharp
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreKey]
    public string HotelId { get; set; }

    [VectorStoreData(StorageName = "hotel_name")]
    public string HotelName { get; set; }

    [VectorStoreData(IsFullTextIndexed = true, StorageName = "hotel_description")]
    public string Description { get; set; }

    [VectorStoreVector(4, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```json
{
    "ids": ["h1"],
    "embeddings": [[0.9, 0.1, 0.1, 0.1]],
    "metadatas": [{ "hotel_name": "Hotel Happy", "hotel_description": "A place where everyone can be happy." }],
    "documents": ["A place where everyone can be happy."]
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
