---
title: Using the Semantic Kernel Milvus Vector Store connector (Preview)
description: Learn how to use the .NET Community Toolkit Milvus Vector Store connector to access and manage data in Milvus.
author: westey-m
ms.topic: article
ms.author: westey
ms.date: 10/30/2026
ms.service: semantic-kernel
---

# Using the Milvus connector (Preview)

The Milvus Vector Store connector for C# is provided by the [.NET Community Toolkit](https://github.com/CommunityToolkit/AI) and is built on the [Milvus.Client](https://www.nuget.org/packages/Milvus.Client) SDK. It implements the `Microsoft.Extensions.VectorData` abstractions and requires **Milvus 2.6 or later**.

> [!WARNING]
> The Milvus connector is in preview. Breaking changes may occur before release.

## Overview

| Feature Area                          | Support |
|---------------------------------------|---------|
| Collection maps to                    | Milvus collection |
| Supported key property types          | `long`, `string`, `Guid` |
| Supported data property types         | `string`, `int`, `long`, `float`, `double`, `bool`, nullable versions of these value types, and arrays or `List<T>` of these scalar types |
| Supported vector property types       | `ReadOnlyMemory<float>`, `Embedding<float>`, `float[]` |
| Supported index types                 | Automatic (default), Flat, Hnsw |
| Supported distance functions          | CosineSimilarity (default), DotProductSimilarity, EuclideanDistance, EuclideanSquaredDistance |
| Supported filter clauses              | LINQ expressions: `==`, `!=`, `<`, `<=`, `>`, `>=`, `&&`, `\|\|`, `!`, null checks, `Contains` over constant collections or array properties, and `Any` with `Contains` over array properties |
| Supports multiple vectors in a record | Yes |
| IsIndexed supported?                  | All scalar data fields can be filtered. The setting does not create scalar indexes. |
| IsFullTextIndexed supported?          | Yes (string properties only) |
| StorageName supported?                | Yes (key, data, and vector properties) |
| HybridSearch supported?               | Yes, requires a string data property with `IsFullTextIndexed = true` |

## Limitations

| Feature Area | Details |
|--------------|---------|
| Server version | Milvus 2.5 is not supported. Use Milvus 2.6 or later. |
| Models | Every record model must have at least one vector property. |
| Key generation | Only `Guid` keys support automatic generation. The connector generates these keys before upsert. |
| Data properties | Date/time types and whole-array equality filters are not supported. Array or list properties can be null or empty, but their elements cannot be null. |
| Ordering | Ordered retrieval is not supported. |
| Storage names | Collection and field names must use ASCII letters, digits, and underscores, starting with a letter or underscore, and contain at most 255 characters. Field names cannot be reserved Milvus keywords. |
| Full-text indexing | Each full-text indexed property reserves a hidden sparse field named `<StorageName>_bm25`. No other property can use that name, and the original storage name can contain at most 250 characters. |
| Strings and arrays | String fields allow up to 65,535 bytes. Collections created by the connector configure array fields for at most 4,096 elements. |
| Schema changes | Schema changes on existing collections require explicit management through the Milvus SDK. |
| NativeAOT | Typed collections use reflection. Use dynamic dictionary collections with explicit definitions for NativeAOT. The `Milvus.Client` dependency emits trimming and NativeAOT warnings. |

On .NET Framework, the Milvus client requires a TLS endpoint with a certificate trusted by Windows and Windows 11 or later. Plaintext gRPC connections aren't supported on .NET Framework. See the [provider documentation](https://github.com/CommunityToolkit/AI/tree/main/MEVD/src/Milvus#limitations) for details.

## Getting started

Install the preview connector package:

```dotnetcli
dotnet add package CommunityToolkit.VectorData.Milvus --prerelease
```

Start a Milvus 2.6 or later instance by following the [Milvus standalone installation instructions](https://milvus.io/docs/install_standalone-docker.md).

### Dependency injection

Register the vector store with an `IServiceCollection`:

```csharp
using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new();
services.AddMilvusVectorStore("localhost", https: false);
```

Host-based registrations default to port `19530` and `https: true`. Use `https: false` only for a local instance serving plaintext gRPC. Overloads also accept a database name, an API key or username/password authentication, options, and a service lifetime. API-key and username/password authentication cannot be combined. Keyed registrations and `AddMilvusCollection<TKey, TRecord>` are also available.

Alternatively, register a `MilvusClient` separately and call `AddMilvusVectorStore()` without a host. Host-based registrations own their SDK client; supplied clients remain caller-owned.

### Direct construction

You can construct a store and get a named collection directly. This example connects to a local plaintext instance and uses the `Hotel` model defined in the next section.

```csharp
using CommunityToolkit.VectorData.Milvus;
using Microsoft.Extensions.VectorData;
using Milvus.Client;

using MilvusVectorStore vectorStore = new(
    new MilvusClient("localhost", port: 19530, ssl: false),
    ownsClient: true);

using VectorStoreCollection<long, Hotel> collection =
    vectorStore.GetCollection<long, Hotel>("skhotels");

await collection.EnsureCollectionExistsAsync();
```

To construct a collection without a store, use `MilvusCollection<long, Hotel>` with a `MilvusClient`, a collection name, and the `ownsClient` argument. `MilvusCollectionOptions` provides `LoadTimeout`, `HnswM`, and `HnswEfConstruction` settings.

## Data mapping

The connector maps model properties directly to Milvus fields. A `long` key becomes an Int64 primary key; `string` and `Guid` keys become VarChar primary keys. Vectors are stored as dense float vector fields. Use `StorageName` to override the field name for a key, data, or vector property.

```csharp
using System;
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreKey(StorageName = "hotel_id")]
    public long HotelId { get; set; }

    [VectorStoreData(IsIndexed = true, StorageName = "hotel_name")]
    public string HotelName { get; set; } = string.Empty;

    [VectorStoreData(IsFullTextIndexed = true, StorageName = "hotel_description")]
    public string Description { get; set; } = string.Empty;

    [VectorStoreVector(4, DistanceFunction = DistanceFunction.CosineSimilarity,
        IndexKind = IndexKind.Hnsw, StorageName = "description_embedding")]
    public ReadOnlyMemory<float> DescriptionEmbedding { get; set; }
}
```

The full-text indexed `hotel_description` field uses Milvus's default text analyzer and a BM25 function. The connector stores its sparse vectors in the hidden `hotel_description_bm25` field, which is not included in records read back from the collection.

## Hybrid search

`HybridSearchAsync` combines dense vector search with BM25 keyword search on a full-text indexed string property, applying the filter to both searches. The connector fuses the results with Reciprocal Rank Fusion (RRF, `k = 60`). Higher scores rank higher, and score thresholds are lower bounds. The keywords are analyzed together as one text.

Each search ranks at least 10 candidates, or `top + Skip` if that is larger. Hybrid search supports at most 16,384 candidates, including skipped results. The collection exposes `IKeywordHybridSearchable<TRecord>` through `GetService` only when its model contains a full-text indexed property.

## Next steps

Learn more about [defining a data model](../defining-your-data-model.md) and [using Vector Store connectors](../index.md).
