---
title: Using the Semantic Kernel Postgres Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Postgres.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 10/24/2024
ms.service: semantic-kernel
---
# Using the Postgres Vector Store connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone pivot="programming-language-csharp"

## Overview

The Postgres Vector Store connector can be used to access and manage data in Postgres. The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | Postgres table                                                                                                                   |
| Supported key property types      | <ul><li>short</li><li>int</li><li>long</li><li>string</li><li>Guid</li></ul>                                                     |
| Supported data property types     | <ul><li>bool</li><li>short</li><li>int</li><li>long</li><li>float</li><li>double</li><li>decimal</li><li>string</li><li>DateTime</li><li>DateTimeOffset</li><li>Guid</li><li>byte[]</li><li>bool Enumerables</li><li>short Enumerables</li><li>int Enumerables</li><li>long Enumerables</li><li>float Enumerables</li><li>double Enumerables</li><li>decimal Enumerables</li><li>string Enumerables</li><li>DateTime Enumerables</li><li>DateTimeOffset Enumerables</li><li>Guid Enumerables</li></ul> |
| Supported vector property types   | ReadOnlyMemory\<float\>                                                                                                          |
| Supported index types             | Hnsw                                                                                                                             |
| Supported distance functions      | <ul><li>CosineDistance</li><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanDistance</li><li>ManhattanDistance</li></ul>|
| Supported filter clauses          | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                                                  |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsFilterable supported?           | No                                                                                                                               |
| IsFullTextSearchable supported?   | No                                                                                                                               |
| StoragePropertyName supported?    | Yes                                                                                                                              |

## Getting started

Add the Postgres Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.Postgres --prerelease
```

You can add the vector store to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

In this case, an instance of the `Npgsql.NpgsqlDataSource` class, which has vector capabilities enabled, will also be registered with the container.

```csharp
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddPostgresVectorStore("Host=localhost;Port=5432;Username=postgres;Password=example;Database=postgres;");
```

Extension methods that take no parameters are also provided. These require an instance of the `Npgsql.NpgsqlDataSource` class to be separately registered with the dependency injection container.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Npgsql;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<NpgsqlDataSource>(sp => 
{
    NpgsqlDataSourceBuilder dataSourceBuilder = new("Host=localhost;Port=5432;Username=postgres;Password=example;Database=postgres;");
    dataSourceBuilder.UseVector();
    return dataSourceBuilder.Build();
});

builder.Services.AddPostgresVectorStore();
```

You can construct a Postgres Vector Store instance directly.

```csharp
using Microsoft.SemanticKernel.Connectors.Postgres;
using Npgsql;

NpgsqlDataSourceBuilder dataSourceBuilder = new("Host=localhost;Port=5432;Username=postgres;Password=example;Database=postgres;");
dataSourceBuilder.UseVector();
var dataSource = dataSourceBuilder.Build();

var connection = new PostgresVectorStore(dataSource);
```

It is possible to construct a direct reference to a named collection.

```csharp
using Microsoft.SemanticKernel.Connectors.Postgres;
using Npgsql;

NpgsqlDataSourceBuilder dataSourceBuilder = new("Host=localhost;Port=5432;Username=postgres;Password=example;Database=postgres;");
dataSourceBuilder.UseVector();
var dataSource = dataSourceBuilder.Build();

var collection = new PostgresVectorStoreRecordCollection<string, Hotel>(dataSource, "skhotels");
```

## Data mapping

The Postgres Vector Store connector provides a default mapper when mapping from the data model to storage.
This mapper does a direct conversion of the list of properties on the data model to the columns in Postgres.

It's also possible to override the default mapper behavior by providing a custom mapper via the `PostgresVectorStoreRecordCollectionOptions<TRecord>.DictionaryCustomMapper` property.

### Property name override

You can override property names to use in storage that is different to the property names on the data model.
The property name override is done by setting the `StoragePropertyName` option via the data model property attributes or record definition.

Here is an example of a data model with `StoragePropertyName` set on its attributes and how that will be represented in Postgres query.

```csharp
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreRecordKey]
    public ulong HotelId { get; set; }

    [VectorStoreRecordData(StoragePropertyName = "hotel_name")]
    public string? HotelName { get; set; }

    [VectorStoreRecordData(StoragePropertyName = "hotel_description")]
    public string? Description { get; set; }

    [VectorStoreRecordVector(Dimensions: 4, DistanceFunction.CosineDistance)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```sql
CREATE TABLE public."Hotels" (
    "HotelId" INTEGER NOT NULL,
    "hotel_name" TEXT ,
    "hotel_description" TEXT ,
    "DescriptionEmbedding" VECTOR(4) ,
    PRIMARY KEY ("HotelId")
);
```

::: zone-end
::: zone pivot="programming-language-python"

## Coming soon

More info coming soon.

::: zone-end
::: zone pivot="programming-language-java"

## JDBC

The [JDBC](./jdbc-connector.md) connector can be used to connect to Postgres.

::: zone-end
