---
title: Using the Semantic Kernel SQLite Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in SQLite.
zone_pivot_groups: programming-languages
author: dmytrostruk
ms.topic: article
ms.author: dmytrostruk
ms.date: 10/24/2024
ms.service: semantic-kernel
---
# Using the SQLite Vector Store connector (Preview)

::: zone pivot="programming-language-csharp"

> [!WARNING]
> The Sqlite Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

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

The SQLite Vector Store connector can be used to access and manage data in SQLite. The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | SQLite table                                                                                                                     |
| Supported key property types      | <ul><li>int</li><li>long</li><li>string</li><li>Guid</li></ul>                                                                   |
| Supported data property types     | <ul><li>int</li><li>long</li><li>short</li><li>string</li><li>bool</li><li>float</li><li>double</li><li>byte[]</li><li>Guid</li><li>DateTime</li><li>DateTimeOffset</li><li>DateOnly (.NET only)</li><li>TimeOnly (.NET only)</li></ul> |
| Supported vector property types   | <ul><li>ReadOnlyMemory\<float\></li><li>Embedding\<float\></li><li>float[]</li></ul>                                             |
| Supported index types             | N/A                                                                                                                              |
| Supported distance functions      | <ul><li>CosineDistance (default)</li><li>ManhattanDistance</li><li>EuclideanDistance</li></ul>                                   |
| Supported filter clauses          | LINQ expressions: comparisons, `&&`, `\|\|`, `!`, and `Contains` over an inline or captured array. `Contains` over an array property and `Any` are not supported. |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsIndexed    supported?           | Yes                                                                                                                              |
| IsFullTextIndexed supported?      | No                                                                                                                               |
| StorageName supported?            | Yes                                                                                                                              |
| HybridSearch supported?           | No                                                                                                                               |

## Getting started

Add the SQLite Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package CommunityToolkit.VectorData.SqliteVec --prerelease
```

You can add the vector store to the `IServiceCollection` dependency injection container using extension methods provided by the connector package.

```csharp
using Microsoft.Extensions.DependencyInjection;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqliteVectorStore(_ => "Data Source=:memory:");
```

Overloads that accept `SqliteVectorStoreOptions`, a `ServiceLifetime` and keyed registrations are also provided, as well as `AddSqliteCollection<TKey, TRecord>` for registering a collection directly. If an `IEmbeddingGenerator` is registered in the container, the vector store and collections use it automatically. The sqlite-vec extension is loaded automatically, so no manual loading is needed.

You can construct a SQLite Vector Store instance directly.

```csharp
using CommunityToolkit.VectorData.SqliteVec;

var vectorStore = new SqliteVectorStore("Data Source=:memory:");
```

It is possible to construct a direct reference to a named collection.

```csharp
using CommunityToolkit.VectorData.SqliteVec;

var collection = new SqliteCollection<long, Hotel>("Data Source=:memory:", "skhotels");
```

## Data mapping

The SQLite Vector Store connector provides a default mapper when mapping from the data model to storage.
This mapper does a direct conversion of the list of properties on the data model to the columns in SQLite.

With the vector search extension, vectors are stored in virtual tables, separately from key and data properties.
By default, the virtual table with vectors will use the same name as the table with key and data properties, but with a `vec_` prefix. For example, if the collection name in `SqliteCollection` is `skhotels`, the name of the virtual table with vectors will be `vec_skhotels`. It's possible to override the virtual table name by using the `SqliteVectorStoreOptions.VectorVirtualTableName` or `SqliteCollectionOptions.VectorVirtualTableName` properties.

### Property name override

You can provide override property names to use in storage that is different to the property names on the data model.
The property name override is done by setting the `StorageName` option via the data model property attributes or record definition.

Here is an example of a data model with `StorageName` set on its attributes and how that will be represented in a SQLite command.

```csharp
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreKey]
    public long HotelId { get; set; }

    [VectorStoreData(StorageName = "hotel_name")]
    public string? HotelName { get; set; }

    [VectorStoreData(StorageName = "hotel_description")]
    public string? Description { get; set; }

    [VectorStoreVector(4, DistanceFunction = DistanceFunction.CosineDistance)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```sql
CREATE TABLE "Hotels" (
    "HotelId" INTEGER PRIMARY KEY,
    "hotel_name" TEXT,
    "hotel_description" TEXT
);

CREATE VIRTUAL TABLE "vec_Hotels" USING vec0(
    "HotelId" INTEGER PRIMARY KEY,
    "DescriptionEmbedding" FLOAT[4] distance_metric=cosine
);
```

::: zone-end
::: zone pivot="programming-language-python"

## Coming soon

More info coming soon.

::: zone-end
::: zone pivot="programming-language-java"

## JDBC

The [JDBC](./jdbc-connector.md) connector can be used to connect to SQLite.

::: zone-end
