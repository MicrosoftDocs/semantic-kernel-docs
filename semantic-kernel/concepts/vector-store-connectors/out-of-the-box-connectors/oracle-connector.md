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
| Supported distance functions          | <ul><li>CosineDistance</li><ul><li>FLOAT32, FLOAT64, and INT8 vector default</li></ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>NegativeDotProductSimilarity</li><li>EuclideanDistance</li><li>EuclideanSquaredDistance</li><li>HammingDistance</li><ul><li>BINARY vector default</li></ul><li>ManhattanDistance</li><li>JaccardSimilarity<br> To use Jaccard similarity, set the DistanceFunction string to "JACCARD" or "JACCARDSIMILARITY" (for example, DistanceFunction = "JACCARDSIMILARITY"). This value is case sensitive. Jaccard similarity requires BINARY numeric format vectors. </li></ul>                                                                                     |
| Supported filter clauses              | <ul><li>==</li><li>!=</li><li><</li><li><=</li><li>></li><li>>=</li><li>List.Contains() <ul><li>Only when checking if the model property is in the list</li></ul></li></ul>                                                                                                                                                     |
| Supports zero, one, or multiple vectors in a record | Yes                                                                                                                                                                           |
| IsIndexed supported?                  | Yes                                                                                                                                                                           |
| IsFullTextSearchable supported?          | No                                                                                                                                                                            |
| StorageName supported?                | Yes                                                                                              |
| HybridSearch supported?               | No                                                                                                                                                                         |

> [!IMPORTANT]
> Vector data searches require Oracle Database 23ai or higher. All other Oracle connector features are available using Oracle Database 19c or higher.

::: zone-end
::: zone pivot="programming-language-python"
More information coming soon.
::: zone-end
::: zone pivot="programming-language-java"

| Feature Area        | Support           |
| ------------- |-------------|
|Supported filter clauses|<ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>|
|Collection maps to|SQL database table|
|Supported key property types|<ul><li>short/Short</li><li>int/Integer</li><li>long/Long</li><li>String</li><li>UUID</li></ul>|
|Supported data property types|<ul><li>byte/Byte</li><li>short/Short</li><li>int/Integer</li><li>long/Long</li><li>float/Float</li><li>double/Double</li><li>decimal/Decimal</li><li>DateTime</li><li>OffsetDataTime</li><li>Timestamp</li><li>String</li><li>UUID</li><li>List`<of all above types>`</li></ul>|
|Supported vector property types|<ul><li>String</li><li>Collection`<Float>`</li><li>List`<Float>`</li><li>Float[]</li><li>float[]</li></ul>|
|Supported index types|<ul><li>HNSW</li><li>IVF</li></ul>|
|Supported distance functions|<ul><li>DOT_PRODUCT</li><li>COSINE_SIMILARITY</li><li>COSINE_DISTANCE</li><li>EUCLIDEAN_DISTANCE</li></ul>|
|Supports multiple vectors in a record|Yes|
|IsIndexed support?|Yes|
|IsFullTextSearchable supported?| No|
|StoragePropertyName supported?|No, use `@JsonProperty` instead|
|HybridSearch supported?|No|

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

Set up Oracle Database Vector Store connector.

```java
// Copyright (c) Microsoft. All rights reserved.
package com.microsoft.semantickernel.samples.syntaxexamples.memory;

import com.microsoft.semantickernel.data.jdbc.JDBCVectorStore;
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStoreOptions;
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStoreRecordCollection;
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStoreRecordCollectionOptions;
import com.microsoft.semantickernel.data.jdbc.oracle.OracleVectorStoreQueryProvider;
import com.microsoft.semantickernel.data.vectorstorage.VectorStoreRecordCollection;
import com.microsoft.semantickernel.samples.documentationexamples.data.index.Hotel;
import java.sql.SQLException;
import java.util.Collections;
import oracle.jdbc.datasource.impl.OracleDataSource;

public class VectorStoreWithOracle {

    public static void main(String[] args) throws SQLException {
        System.out.println("==============================================================");
        System.out.println("============== Oracle Vector Store Example ===================");
        System.out.println("==============================================================");

        // Configure the data source
        OracleDataSource dataSource = new OracleDataSource();
        dataSource.setURL("jdbc:oracle:thin:@localhost:1521/FREEPDB1");
        dataSource.setUser("scott");
        dataSource.setPassword("tiger");

        // Build a query provider
        OracleVectorStoreQueryProvider queryProvider = OracleVectorStoreQueryProvider.builder()
            .withDataSource(dataSource)
            .build();

        // Build a vector store
        JDBCVectorStore vectorStore = JDBCVectorStore.builder()
            .withDataSource(dataSource)
            .withOptions(JDBCVectorStoreOptions.builder()
                .withQueryProvider(queryProvider)
                .build())
            .build();

        // Get a collection from the vector store
        VectorStoreRecordCollection<String, Hotel> collection = vectorStore.getCollection(
            "skhotels",
            JDBCVectorStoreRecordCollectionOptions.<Hotel>builder()
                .withRecordClass(Hotel.class)
                .build());

        // Create the collection if it doesn't exist yet.
        collection.createCollectionAsync().block();

        collection.upsertAsync(new Hotel("1",
            "HotelOne",
            "Desc for HotelOne",
            Collections.emptyList(), Collections.emptyList()),
            null)
            .block();

    }

}
```

::: zone-end
::: zone pivot="programming-language-csharp"

## Data mapping

The Oracle Database Vector Store connector provides a default mapper when mapping data from the data model to storage. This mapper does a direct conversion of the data model properties list to the Oracle database columns to convert to the storage schema.

The Oracle Database Vector Store connector supports data model annotations and record definitions.Using annotations, the information can be provided to the data model for creating indexes and database column mapping. Using [record definitions](../schema-with-record-definition.md), the information can be defined and supplied separately from the data model.

The following table shows the default primary key data type mapping between Oracle Database and C#:

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

- [Introducing the Oracle Database Vector Store Connector for Semantic Kernel](https://medium.com/oracledevs/announcing-the-oracle-database-vector-store-connector-for-semantic-kernel-adb83e806d4e)  
Describes key connector features, classes, and guides the reader through a sample AI vector search application using the connector.
- [Documentation: Oracle Database Vector Store Connector Classes for Semantic Kernel (.NET) APIs](https://docs.oracle.com/en/database/oracle/oracle-database/23/odpnt/VSConnector4SKClasses.html)  
Contains information on Oracle Database Vector Store connector classes for adding data, retrieving data, and performing vector search in the Oracle vector database.  
- [Documentation: Oracle Data Provider for .NET](https://docs.oracle.com/en/database/oracle/oracle-database/23/odpnt/intro.html)  
Contains information on Oracle Data Provider for .NET (ODP.NET), the ADO.NET data provider for Oracle Database Vector Store connector.

::: zone-end
::: zone pivot="programming-language-java"

## Data mapping

The Oracle Database Vector Store connector provides a default mapper when mapping data from the data model to storage. This mapper does a direct conversion of the data model properties list to the Oracle database columns to convert to the storage schema.

The Oracle Database Vector Store connector supports data model annotations and record definitions.Using annotations, the information can be provided to the data model for creating indexes and database column mapping. Using [record definitions](../schema-with-record-definition.md), the information can be defined and supplied separately from the data model.

The following table shows the default primary key data type mapping between Oracle Database and Java, along with the corresponding methods to retrieve data from a `ResultSet`:

| Java Type        | Database Type          | ResultSet Getter Method  |
| ------------- |-------------| -----|
| byte/Byte      | NUMBER(3) | `resultSet.getByte(name)`|
| short/Short    | NUMBER(5)  |`resultSet.getShort(name)`|
|int/Integer     | NUMBER(10) |`resultSet.getInt(name)`|
|long/Long       |NUMBER(19)  |`resultSet.getLong(name)`|
|String          |NVARCHAR2(2000)|`resultSet.getString(name)`|
|UUID            |RAW(16)  |  `resultSet.getObject(name, java_type)`|

The following table shows the default data property type mapping along with the corresponding methods to retrieve data from a `ResultSet`:

| Java Type        | Database Type          | ResultSet Getter Method  |
| ------------- |-------------| -----|
| boolean     | BOOLEAN | `resultSet.getByte(name)`|
|byte/Byte    |NUMBER(3)|`resultSet.getByte(name)`|
|byte[]       |RAW(2000)|`resultSet.getBytes(name)`|
|short/Short  |NUMBER(5)|`resultSet.getShort(name)`|
|int/Integer  |NUMBER(10)|`resultSet.getInt(name)`|
|long/Long    |NUMBER(19)|`resultSet.getLong(name)`|
|float/Float  |BINARY_FLOAT|`resultSet.getFloat(name)`|
|double/Double|BINARY_DOUBLE|`resultSet.getDouble(name)`|
|BigDecimal   |NUMBER(18,2)|`resultSet.getBigDecimal(name)`|
|OffsetDateTime|TIMESTAMP(7) WITH TIME ZONE|`resultSet.getTIMESTAMPTZ(name).offsetDateTimeValue()`|
|String        |CLOB/NVARCHAR2(%s)|`resultSet.getString(name)`|
|UUID          |RAW(16)           |`resultSet.getObject(name, java_type)`|
|List`<T>`       |JSON              |`resultSet.getObject(name, java_type)`  Using `ojdbc-extensions-jackson-oson`|

 Starting with Oracle Database 23ai, database vectors can be mapped to Java data types. Multiple vector columns are supported. The following table shows the default vector property type mapping:

| Java Type        | Database Type|
| ------------- |-------------|
| String          | VECTOR(%d, FLOAT32) |  
|Collection`<Float>`|VECTOR(%d, FLOAT32)  |  
|List`<Float>`      |VECTOR(%d, FLOAT32)  |  
|Float[]          |VECTOR(%d, FLOAT32)  |  
|float[]          |VECTOR(%d, FLOAT32)  |  

::: zone-end
