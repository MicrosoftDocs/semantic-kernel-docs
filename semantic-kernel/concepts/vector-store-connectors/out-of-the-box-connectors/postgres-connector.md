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

::: zone pivot="programming-language-csharp"

> [!WARNING]
> The Postgres Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

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

The Postgres Vector Store connector can be used to access and manage data in Postgres and also supports [Neon Serverless Postgres](https://azuremarketplace.microsoft.com/en-us/marketplace/apps/neon1722366567200.neon_serverless_postgres_azure_prod).

The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | Postgres table                                                                                                                   |
| Supported key property types      | <ul><li>short</li><li>int</li><li>long</li><li>string</li><li>Guid</li></ul>                                                     |
| Supported data property types     | <ul><li>bool</li><li>short</li><li>int</li><li>long</li><li>float</li><li>double</li><li>decimal</li><li>string</li><li>DateTime</li><li>DateTimeOffset</li><li>Guid</li><li>byte[]</li><li>bool Enumerables</li><li>short Enumerables</li><li>int Enumerables</li><li>long Enumerables</li><li>float Enumerables</li><li>double Enumerables</li><li>decimal Enumerables</li><li>string Enumerables</li><li>DateTime Enumerables</li><li>DateTimeOffset Enumerables</li><li>Guid Enumerables</li></ul> |
| Supported vector property types   | <ul><li>ReadOnlyMemory\<float\></li><li>Embedding\<float\></li><li>float[]</li><li>ReadOnlyMemory\<Half\></li><li>Embedding\<Half\></li><li>Half[]</li><li>BitArray</li><li>Pgvector.SparseVector</li></ul>                                             |
| Supported index types             | Hnsw                                                                                                                             |
| Supported distance functions      | <ul><li>CosineDistance</li><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanDistance</li><li>ManhattanDistance</li></ul>|
| Supported filter clauses          | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                                                  |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsIndexed supported?              | No                                                                                                                               |
| IsFullTextIndexed supported?      | No                                                                                                                               |
| StorageName supported?            | Yes                                                                                                                              |
| HybridSearch supported?           | No                                                                                                                               |

## Limitations

> [!IMPORTANT]
> When initializing `NpgsqlDataSource` manually, it is necessary to call `UseVector` on the `NpgsqlDataSourceBuilder`. This enables vector support. Without this, usage of the VectorStore implementation will fail.

Here is an example of how to call `UseVector`.

```csharp
NpgsqlDataSourceBuilder dataSourceBuilder = new("Host=localhost;Port=5432;Username=postgres;Password=example;Database=postgres;");
dataSourceBuilder.UseVector();
NpgsqlDataSource dataSource = dataSourceBuilder.Build();
```

When using the `AddPostgresVectorStore` dependency injection registration method with a connection string, the datasource will be constructed by this method and will automatically have `UseVector` applied.

## Getting started

Add the Postgres Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.PgVector --prerelease
```

You can add the vector store to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

In this case, an instance of the `Npgsql.NpgsqlDataSource` class, which has vector capabilities enabled, will also be registered with the container.

```csharp
using Microsoft.Extensions.DependencyInjection;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddPostgresVectorStore("Host=localhost;Port=5432;Username=postgres;Password=example;Database=postgres;");
```

Extension methods that take no parameters are also provided. These require an instance of the `Npgsql.NpgsqlDataSource` class to be separately registered with the dependency injection container.

```csharp
using Microsoft.Extensions.DependencyInjection;
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

You can construct a Postgres Vector Store instance directly with a custom data source or with a connection string.

```csharp
using Microsoft.SemanticKernel.Connectors.PgVector;
using Npgsql;

NpgsqlDataSourceBuilder dataSourceBuilder = new("Host=localhost;Port=5432;Username=postgres;Password=example;Database=postgres;");
dataSourceBuilder.UseVector();
var dataSource = dataSourceBuilder.Build();

var connection = new PostgresVectorStore(dataSource, ownsDataSource: true);
```

```csharp
using Microsoft.SemanticKernel.Connectors.PgVector;

var connection = new PostgresVectorStore("Host=localhost;Port=5432;Username=postgres;Password=example;Database=postgres;");
```

It is possible to construct a direct reference to a named collection with a custom data source or with a connection string.

```csharp
using Microsoft.SemanticKernel.Connectors.PgVector;
using Npgsql;

NpgsqlDataSourceBuilder dataSourceBuilder = new("Host=localhost;Port=5432;Username=postgres;Password=example;Database=postgres;");
dataSourceBuilder.UseVector();
var dataSource = dataSourceBuilder.Build();

var collection = new PostgresCollection<string, Hotel>(dataSource, "skhotels", ownsDataSource: true);
```

```csharp
using Microsoft.SemanticKernel.Connectors.PgVector;

var collection = new PostgresCollection<string, Hotel>("Host=localhost;Port=5432;Username=postgres;Password=example;Database=postgres;", "skhotels");
```

## Data mapping

The Postgres Vector Store connector provides a default mapper when mapping from the data model to storage.
This mapper does a direct conversion of the list of properties on the data model to the columns in Postgres.

### Property name override

You can provide override property names to use in storage that is different to the property names on the data model.
The property name override is done by setting the `StorageName` option via the data model property attributes or record definition.

Here is an example of a data model with `StorageName` set on its attributes and how that will be represented in a Postgres command.

```csharp
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreKey]
    public int HotelId { get; set; }

    [VectorStoreData(StorageName = "hotel_name")]
    public string? HotelName { get; set; }

    [VectorStoreData(StorageName = "hotel_description")]
    public string? Description { get; set; }

    [VectorStoreVector(Dimensions: 4, DistanceFunction = DistanceFunction.CosineDistance)]
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
