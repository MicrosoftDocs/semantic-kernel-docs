---
title: Using the Semantic Kernel Oracle Database Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Oracle Database.
zone_pivot_groups: programming-languages
author: minal-agashe-oracle
ms.topic: conceptual
ms.author: westey
ms.date: 08/14/2025
ms.service: semantic-kernel
---
# Using the Oracle Database Vector Store connector (Preview)

::: zone pivot="programming-language-csharp"

> [!WARNING]
> The Oracle Database Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end
::: zone pivot="programming-language-python"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end
::: zone pivot="programming-language-java"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end

## Overview

The Oracle Database Vector Store Connector can be used to access and manage data in Oracle Database. The connector has the following characteristics.

::: zone pivot="programming-language-csharp"

| Feature Area                          | Support                                                                                                                                                                       |
| ------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Collection maps to                    | Oracle database table                                                                                                                                                    |
| Supported key property types          |      <ul><li>short</li><li>int</li><li>long</li><li>string</li><li>Guid</li></ul>                                                                                                                                                                   |
| Supported data property types         | <ul><li>bool</li><li>byte</li><li>short</li><li>int</li><li>decimal</li><li>long</li><li>float</li><li>double</li><li>DateTime</li><li>DateTimeOffset</li><li>TimeSpan</li><li>char</li><li>char[]</li><li>byte[]</li><li>String</li><li>Guid</li><li>*and nullable type of the above types*</i></li></ul> |
| Supported vector property types       | <ul><li>ReadOnlyMemory\<float\></li><li>Embedding\<float\></li><li>float[]</li><li>ReadOnlyMemory\<double\></li><li>Embedding\<double\></li><li>double[]</li><li>ReadOnlyMemory\<short\></li><li>Embedding\<short\></li><li>short[]</li><li>ReadOnlyMemory\<byte\></li><li>Embedding\<byte\></li><li>byte[]</li><li>BitArray</li><li>BinaryEmbedding</li></ul>                                                                                          |
| Supported index types                 | <ul><li>Flat (default)</li><li>HNSW</li><li>IVF</li></ul>                                                                                                                                                                      |
| Supported distance functions          | <ul><li>CosineDistance</li><ul><li>FLOAT32, FLOAT64, and INT8 vector default</li></ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>NegativeDotProductSimilarity</li><li>EuclideanDistance</li><li>EuclideanSquaredDistance</li><li>HammingDistance</li><ul><li>BINARY vector default</li></ul><li>ManhattanDistance</li><li>JaccardSimilarity</li></ul>                                                                                     |
| Supported filter clauses              | <ul><li>==</li><li>!=</li><li><</li><li><=</li><li>></li><li>>=</li><li>List.Contains() <ul><li>Only when checking if the model property is in the list</li></ul></li></ul>                                                                                                                                                     |
| Supports zero, one, or multiple vectors in a record | Yes                                                                                                                                                                           |
| IsIndexed supported?                  | Yes                                                                                                                                                                           |
| IsFullTextSearchable supported?          | No                                                                                                                                                                            |
| StorageName supported?                | Yes                                                                                              |
| HybridSearch supported?               | No                                                                                                                                                                         |


> [!IMPORTANT]
> Vector data searches require Oracle Database 23ai. All other Oracle connector features are available using Oracle Database 19c or higher.

::: zone-end
::: zone pivot="programming-language-python"
More information coming soon.
::: zone-end
::: zone pivot="programming-language-java"

More information coming soon.

::: zone-end

::: zone pivot="programming-language-csharp"

## Getting started

Add the Oracle Database Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Oracle.VectorData --prerelease
```

You can add the vector store to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel. In this case, an instance of the `Oracle.VectorData.OracleVectorStore` class also gets registered with the container.

```csharp
using Microsoft.SemanticKernel;
using Oracle.VectorData;
using Microsoft.Extensions.DependencyInjection;
 
// Using Kernel Builder.
var builder = Kernel.CreateBuilder();
builder.Services.AddOracleVectorStore("<connection string>");
```

```csharp
using Microsoft.AspNetCore.Builder;
using Oracle.VectorData;
using Microsoft.Extensions.DependencyInjection;
 
// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOracleVectorStore("<connection string>");
```

Extension methods that take no parameters are also available. These require an instance of the `Oracle.ManagedDataAccess.Client.OracleDataSource` class to be separately registered with the dependency injection container.

```csharp
using Microsoft.SemanticKernel;
using Oracle.VectorData;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
 
// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
builder.Services.AddSingleton<OracleDataSource>(sp =>
{
    OracleDataSourceBuilder dataSourceBuilder = new("<connection string>");  
    return dataSourceBuilder.Build();
});
 
builder.Services.AddOracleVectorStore();
```

```csharp
using Microsoft.AspNetCore.Builder;
using Oracle.VectorData;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
 
// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<OracleDataSource>(sp =>
{
    OracleDataSourceBuilder dataSourceBuilder = new("<connection string>");  
    return dataSourceBuilder.Build();
});
 
builder.Services.AddOracleVectorStore();
```

You can construct an Oracle Database Vector Store instance directly with a custom data source or with a connection string.

```csharp
using Oracle.VectorData;
using Oracle.ManagedDataAccess.Client;
 
OracleDataSourceBuilder dataSourceBuilder = new("<connection string>");
var dataSource = dataSourceBuilder.Build();
 
var connection = new OracleVectorStore(dataSource);
```

```csharp
using Oracle.VectorData;
 
var connection = new OracleVectorStore("<connection string>");
```

It is possible to construct a direct reference to a named collection with a custom data source or with a connection string.

```csharp
using Oracle.VectorData;
using Oracle.ManagedDataAccess.Client;
 
OracleDataSourceBuilder dataSourceBuilder = new("<connection string>");
var dataSource = dataSourceBuilder.Build();
 
var collection = new OracleCollection<string, Hotel>(dataSource, "skhotels");
```

```csharp
using Oracle.VectorData;
 
var collection = new OracleCollection<string, Hotel>("<connection string>", "skhotels");
```

::: zone-end

::: zone pivot="programming-language-python"

## Getting started

More information coming soon.
::: zone-end
::: zone pivot="programming-language-java"

## Getting started

More information coming soon.
::: zone-end
::: zone pivot="programming-language-csharp"

## Data mapping

The Oracle Database Vector Store connector provides a default mapper when mapping data from the data model to storage. This mapper does a direct conversion of the data model properties list to the Oracle database columns to convert to the storage schema.   

The Oracle Database Vector Store connector allows the application to provide a custom mapper in combination with a `VectorStoreCollectionDefinition`. In this case, the `VectorStoreCollectionDefinition` can differ from the supplied data model. The `VectorStoreCollectionDefinition` is used to define the database schema, while the data model is used to interact with the vector store. A custom mapper is required in this case to map from the data model to the custom database schema defined by the `VectorStoreCollectionDefinition`.

The following table shows the default primary key data type mapping between Oracle database and C#:

| C# Data Type                          | Database Type                                                                                                                                                                       |
| ------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| short/int16       | NUMBER(5)       |
| int/int32         | NUMBER(10)      |
| long/int64        | NUMBER(19)      |
| string            | NVARCHAR2(2000) |
| Guid              | RAW(16)         |

The following table shows the default data property type mapping, including nullable types:

| C# Data Type                          | Database Type                                                                                                                                                                       |
| ------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| bool              | BOOLEAN for Oracle Database 23ai and higher <br> NUMBER(1) for earlier versions        |
| byte              | NUMBER(3)       |
| short/int16       | NUMBER(5)       |
| int/int32         | NUMBER(10)      |
| decimal           | NUMBER(18,2)    |
| long/int64        | NUMBER(19)      |
| float             | BINARY_FLOAT    |
| double            | BINARY_DOUBLE   |
| DateTime          | TIMESTAMP(7)    |
| DateTimeOffset    | TIMESTAMP(7) WITH TIME ZONE    |
| TimeSpan          | INTERVAL DAY(8) TO SECOND(7)   |
| char              | NVARCHAR2(1)    |
| char[]            | NVARCHAR2(2000) |
| byte[]            | RAW(2000)       |
| string            | NVARCHAR2(2000) |
| Guid              | RAW(16)         |

Starting with Oracle Database 23ai, database vectors can be mapped to .NET. data types. Multiple vector columns are supported. The following table shows the default vector property type mapping, including nullable types:

| C# Data Type                          | Database Type                                                                                                                                                                       |
| ------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| <ul><li>ReadOnlyMemory\<byte></li><li>Embedding\<System.Byte></li> <li>BinaryEmbedding</li><li>Embedding\<byte></li><li>byte[]</li><li>System.Byte[]</li><li>BitArray</li></ul>      | VECTOR(dimensions, BINARY)       |
| <ul><li>ReadOnlyMemory\<short></li><li>ReadOnlyMemory\<System.Int16></li><li>Embedding\<short></li><li>Embedding\<System.Int16></li><li>short[]</li><li>System.Int16[]</li></ul>          | VECTOR(dimensions, INT8)       |
| <ul><li>ReadOnlyMemory\<double></li><li>ReadOnlyMemory\<System.Double></li><li>Embedding\<System.Double></li><li>Embedding\<double></li><li>double[]</li><li>System.Double[]</li></ul>       | VECTOR(dimensions, FLOAT64)        |
| <ul><li>ReadOnlyMemory\<float></li><li>ReadOnlyMemory\<System.Float></li><li>Embedding\<float></li><li>Embedding\<System.Float></li><li>float[]</li><li>System.Float[]</li></ul>            | VECTOR(dimensions, FLOAT32)   |

### Property name override

For data properties and vector properties, you can override names to use in storage that are different from the data model property names. The property name override occurs when setting the `StorageName` option either in the data model properties or record definition.

Here is a data model with `StorageName` set code sample and how that will be represented in an Oracle SQL command.

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
 
    [VectorStoreVector(Dimensions: 384, DistanceFunction = DistanceFunction.CosineDistance)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```SQL
CREATE TABLE "MYSCHEMA"."Hotels"
  ("HotelId" NUMBER(10),
   "hotel_name" NVARCHAR2(2000),
   "hotel_description" NVARCHAR2(2000),
   "DescriptionEmbedding" VECTOR(384, FLOAT32),
   PRIMARY KEY ( "HotelId" )
);
```

## Learn More
Refer to the following Oracle Database Vector Store connector resources to learn more:
- [Documentation: Oracle Database Vector Store Connector Classes for Semantic Kernel (.NET) APIs](https://docs.oracle.com/en/database/oracle/oracle-database/23/odpnt/oracle-database-vector-store-connector-semantic-kernel-classes.html)  
Contains information on Oracle Database Vector Store connector classes for adding data, retrieving data, and performing vector search in the Oracle vector database.  
-  Sample Code: Oracle Database Vector Store Connector for Semantic Kernel (.NET) 
    - Coming soon
- [Documentation: Oracle Data Provider for .NET](https://docs.oracle.com/en/database/oracle/oracle-database/23/odpnt/intro.html)  
Contains information on Oracle Data Provider for .NET (ODP.NET), the ADO.NET data provider for Oracle Database Vector Store connector.

::: zone-end
