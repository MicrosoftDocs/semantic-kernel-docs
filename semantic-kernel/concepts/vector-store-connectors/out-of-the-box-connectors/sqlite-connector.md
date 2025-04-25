---
title: Using the Semantic Kernel SQLite Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in SQLite.
zone_pivot_groups: programming-languages
author: dmytrostruk
ms.topic: conceptual
ms.author: dmytrostruk
ms.date: 10/24/2024
ms.service: semantic-kernel
---
# Using the SQLite Vector Store connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone pivot="programming-language-csharp"

## Overview

The SQLite Vector Store connector can be used to access and manage data in SQLite. The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | SQLite table                                                                                                                     |
| Supported key property types      | <ul><li>ulong</li><li>string</li></ul>                                                                                           |
| Supported data property types     | <ul><li>int</li><li>long</li><li>ulong</li><li>short</li><li>ushort</li><li>string</li><li>bool</li><li>float</li><li>double</li><li>decimal</li><li>byte[]</li></ul> |
| Supported vector property types   | ReadOnlyMemory\<float\>                                                                                                          |
| Supported index types             | N/A                                                                                                                              |
| Supported distance functions      | <ul><li>CosineDistance</li><li>ManhattanDistance</li><li>EuclideanDistance</li></ul>                                             |
| Supported filter clauses          | <ul><li>EqualTo</li></ul>                                                                                                        |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsFilterable supported?           | No                                                                                                                               |
| IsFullTextSearchable supported?   | No                                                                                                                               |
| StoragePropertyName supported?    | Yes                                                                                                                              |
| HybridSearch supported?           | No                                                                                                                               |

## Limitations

SQLite doesn't support vector search out-of-the-box. The SQLite extension should be loaded first to enable vector search capability. The current implementation of the SQLite connector is compatible with the [sqlite-vec](https://github.com/asg017/sqlite-vec) vector search extension.

In order to install the extension, use one of the [releases](https://github.com/asg017/sqlite-vec/releases) with the specific extension version of your choice. It's possible to get a pre-compiled version with the `install.sh` script. This script will produce `vec0.dll`, which must be located in the same folder as the running application. This will allow the application to call the `SqliteConnection.LoadExtension("vec0")` method and load the vector search extension.

## Getting started

Add the SQLite Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.Sqlite --prerelease
```

You can add the vector store to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

In this case, an instance of the `Microsoft.Data.Sqlite.SqliteConnection` class will be initialized, the connection will be opened and the vector search extension will be loaded. The default vector search extension name is `vec0`, but it can be overridden by using the `SqliteVectorStoreOptions.VectorSearchExtensionName` property.

```csharp
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqliteVectorStore("Data Source=:memory:");
```

Extension methods that take no parameters are also provided. These require an instance of the `Microsoft.Data.Sqlite.SqliteConnection` class to be separately registered with the dependency injection container.

In this case, the connection will be opened only if it wasn't opened before and the extension method will assume that the vector search extension was already loaded for the registered `Microsoft.Data.Sqlite.SqliteConnection` instance.

```csharp
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SqliteConnection>(sp => 
{
    var connection = new SqliteConnection("Data Source=:memory:");
    
    connection.LoadExtension("vector-search-extension-name");

    return connection;
});

builder.Services.AddSqliteVectorStore();
```

You can construct a SQLite Vector Store instance directly.

```csharp
using Microsoft.Data.Sqlite;
using Microsoft.SemanticKernel.Connectors.Sqlite;

var connection = new SqliteConnection("Data Source=:memory:");

connection.LoadExtension("vector-search-extension-name");

var vectorStore = new SqliteVectorStore(connection);
```

It is possible to construct a direct reference to a named collection.

```csharp
using Microsoft.Data.Sqlite;
using Microsoft.SemanticKernel.Connectors.Sqlite;

var connection = new SqliteConnection("Data Source=:memory:");

connection.LoadExtension("vector-search-extension-name");

var collection = new SqliteVectorStoreRecordCollection<Hotel>(connection, "skhotels");
```

## Data mapping

The SQLite Vector Store connector provides a default mapper when mapping from the data model to storage.
This mapper does a direct conversion of the list of properties on the data model to the columns in SQLite.

With the vector search extension, vectors are stored in virtual tables, separately from key and data properties.
By default, the virtual table with vectors will use the same name as the table with key and data properties, but with a `vec_` prefix. For example, if the collection name in `SqliteVectorStoreRecordCollection` is `skhotels`, the name of the virtual table with vectors will be `vec_skhotels`. It's possible to override the virtual table name by using the `SqliteVectorStoreOptions.VectorVirtualTableName` or `SqliteVectorStoreRecordCollectionOptions<TRecord>.VectorVirtualTableName` properties.

### Property name override

You can override property names to use in storage that is different to the property names on the data model.
The property name override is done by setting the `StoragePropertyName` option via the data model property attributes or record definition.

Here is an example of a data model with `StoragePropertyName` set on its attributes and how that will be represented in SQLite query.

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

```
CREATE TABLE Hotels (
    HotelId INTEGER PRIMARY KEY,
    hotel_name TEXT,
    hotel_description TEXT
);

CREATE VIRTUAL TABLE vec_Hotels (
    HotelId INTEGER PRIMARY KEY,
    DescriptionEmbedding FLOAT[4] distance_metric=cosine
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
