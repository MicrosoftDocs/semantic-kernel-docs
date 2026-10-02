---
title: Using the Semantic Kernel SingleStore Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in SingleStore.
zone_pivot_groups: programming-languages
author: AdalbertMemSQL
ms.topic: article
ms.author: westey
ms.date: 02/11/2026
ms.service: semantic-kernel
---
# Using the SingleStore connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone pivot="programming-language-csharp"

## Overview

The SingleStore Vector Store connector can be used to access and manage data in SingleStore.

The connector has the following characteristics.

| Feature Area                          | Support                                                                                                                                                                                                                                                                                                                                                                                                                                   |
|---------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                    | SingleStore table                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Supported key property types          | <ul><li>short</li><li>int</li><li>long</li><li>string</li><li>Guid</li></ul>                                                                                                                                                                                                                                                                                                                                                              |
| Supported data property types         | <ul><li>bool</li><li>byte</li><li>sbyte</li><li>short</li><li>ushort</li><li>int</li><li>uint</li><li>long</li><li>ulong</li><li>float</li><li>double</li><li>decimal</li><li>string</li><li>DateTime</li><li>DateTimeOffset</li><li>DateOnly (.NET 8 and later only)</li><li>TimeOnly (.NET 8 and later only)</li><li>Guid</li><li>byte[]</li><li>string[]</li><li>List\<string\></li><li>*and nullable variants of the above*</li></ul> |
| Supported vector property types       | <ul><li>ReadOnlyMemory\<float\></li><li>Embedding\<float\></li><li>float[]</li></ul>                                                                                                                                                                                                                                                                                                                                                      |
| Supported index types                 | <ul><li>Dynamic</li><li>Flat</li><li>IvfFlat</li><li>Hnsw</li></ul>                                                                                                                                                                                                                                                                                                                                                                       |
| Supported distance functions          | <ul><li>EuclideanDistance</li><li>EuclideanSquaredDistance</li><li>NegativeDotProductSimilarity</li><li>DotProductSimilarity</li></ul>                                                                                                                                                                                                                                                                                                    |
| Supported filter clauses              | <ul><li>`==`, `!=`</li><li>`<`, `<=`, `>`, `>=`</li><li>`&&`, `\|\|`, `!`</li><li>`Contains()` over an inline or captured collection of values</li><li>`Contains()` and `Any()` over a `string[]` or `List<string>` data property</li></ul>                                                                                                                                                                                               |
| Supports multiple vectors in a record | Yes                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| IsIndexed supported?                  | Yes                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| IsFullTextIndexed supported?          | Yes                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| StorageName supported?                | Yes                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| HybridSearch supported?               | Yes                                                                                                                                                                                                                                                                                                                                                                                                                                       |

## Limitations

> [!IMPORTANT]
> When initializing `SingleStoreDataSource` manually, it is necessary to set `AllowLoadLocalInfile=true` and
`ConnectionAttributes`.
> `AllowLoadLocalInfile=true` enables LOAD DATA LOCAL INFILE support. Without this, record uploading will fail.
> Set `ConnectionAttributes` to `_connector_name:SingleStore Semantic Kernel .NET Connector`.
> SingleStore uses `ConnectionAttributes` to collect usage information and prioritize connector development.

Here is an example of how to set `AllowLoadLocalInfile` and `ConnectionAttributes`.

```csharp
SingleStoreDataSource dataSource = new("Host=localhost;Port=3306;Username=root;Password=example;Database=db;AllowLoadLocalInfile=true;ConnectionAttributes=_connector_name:SingleStore Semantic Kernel .NET Connector");
```

When using the `AddSingleStoreVectorStore` dependency injection registration method with a connection string,
`AllowLoadLocalInfile` and `ConnectionAttributes` are set automatically.

## Getting started

Add the SingleStore Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package SingleStore.SemanticKernel --prerelease
```

You can add the vector store to the `IServiceCollection` dependency injection container using extension methods provided
by the connector package.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using SingleStore.SemanticKernel;

var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleStoreVectorStore("<Connection String>");
```

Where `<Connection String>` is a connection string to the SingleStore instance, in the format
that [SingleStore .NET Connector](https://docs.singlestore.com/cloud/csharp/)
expects, for example `Host=localhost;Port=3306;Database=db;Username=root;Password=secret`.

Extension methods that take no parameters are also provided. These require an instance of `SingleStoreDataSource` to be
separately registered with the dependency injection container. To upload records, ensure that `AllowLoadLocalInfile` is
enabled and `ConnectionAttributes` is set:

```csharp
using Microsoft.Extensions.DependencyInjection;
using SingleStore.SemanticKernel;
using SingleStoreConnector;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SingleStoreDataSource>(sp =>
    new SingleStoreDataSource("<Connection String>;AllowLoadLocalInfile=true;ConnectionAttributes=_connector_name:SingleStore Semantic Kernel .NET Connector"));
builder.Services.AddSingleStoreVectorStore();
```

You can construct a SingleStore Vector Store instance directly with a custom data source or with a connection string.

```csharp
using SingleStore.SemanticKernel;
using SingleStoreConnector;

SingleStoreDataSource dataSource = new("<Connection String>;AllowLoadLocalInfile=true;ConnectionAttributes=_connector_name:SingleStore Semantic Kernel .NET Connector");
var vectorStore = new SingleStoreVectorStore(dataSource, ownsDataSource: true);
```

```csharp
using SingleStore.SemanticKernel;

var vectorStore = new SingleStoreVectorStore("<Connection String>");
```

It is possible to construct a direct reference to a named collection with a custom data source or with a connection
string.

```csharp
using SingleStore.SemanticKernel;
using SingleStoreConnector;

SingleStoreDataSource dataSource = new("<Connection String>;AllowLoadLocalInfile=true;ConnectionAttributes=_connector_name:SingleStore Semantic Kernel .NET Connector");

var collection = new SingleStoreCollection<int, Hotel>(dataSource, "skhotels", ownsDataSource: true);
```

```csharp
using SingleStore.SemanticKernel;

var collection = new SingleStoreCollection<int, Hotel>("<Connection String>", "skhotels");
```

## Data mapping

The SingleStore Vector Store connector provides a default mapper when mapping from the data model to storage. This
mapper directly converts the list of properties defined in the data model to columns in SingleStore.

The following table shows the default key and data property type mapping:

| C# Data Type   | SingleStore Type  |
|----------------|-------------------|
| bool           | TINYINT           |
| byte           | TINYINT UNSIGNED  |
| sbyte          | TINYINT           |
| short          | SMALLINT          |
| ushort         | SMALLINT UNSIGNED |
| int            | INT               |
| uint           | INT UNSIGNED      |
| long           | BIGINT            |
| ulong          | BIGINT UNSIGNED   |
| float          | FLOAT             |
| double         | DOUBLE            |
| decimal        | DECIMAL(65,30)    |
| DateTime       | DATETIME(6)       |
| DateTimeOffset | DATETIME(6)       |
| DateOnly       | DATE              |
| TimeOnly       | TIME(6)           |
| string         | LONGTEXT          |
| byte[]         | LONGBLOB          |
| Guid           | CHAR(36)          |
| string[]       | JSON              |
| `List<string>` | JSON              |

Vector properties are mapped to `VECTOR(dimensions, F32)`.

### Property name override

You can specify a storage field name that differs from the corresponding property name in the data model. This allows
you to match table column names even if they don't match the property names on the data model.

The property name override is done by setting the `StorageName` option via the data model attributes or record
definition.

Here is an example of a data model with `StorageName` set on its attributes and how it will be represented in
SingleStore as a table, assuming the Collection name is `Hotels`.

```csharp
using System;
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreKey(StorageName = "hotel_id")]
    public int HotelId { get; set; }

    [VectorStoreData(StorageName = "hotel_name")]
    public string HotelName { get; set; }

    [VectorStoreData(StorageName = "hotel_description")]
    public string Description { get; set; }

    [VectorStoreVector(dimensions: 4, DistanceFunction = DistanceFunction.EuclideanDistance, IndexKind = IndexKind.Hnsw, StorageName = "hotel_description_embedding")]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```sql
CREATE TABLE IF NOT EXISTS `db`.`Hotels`
(
    `hotel_id` INT NOT NULL,
    `hotel_name` LONGTEXT NOT NULL,
    `hotel_description` LONGTEXT NOT NULL,
    `hotel_description_embedding` VECTOR(4, F32) NULL,
    PRIMARY KEY (`hotel_id`),
    VECTOR KEY (`hotel_description_embedding`) INDEX_OPTIONS '{ "metric_type":"EUCLIDEAN_DISTANCE", "index_type":"HNSW_FLAT" }'
);
```

::: zone-end

::: zone pivot="programming-language-python"

## Not supported

Not supported.

::: zone-end
::: zone pivot="programming-language-java"

## Not supported

Not supported.

::: zone-end
