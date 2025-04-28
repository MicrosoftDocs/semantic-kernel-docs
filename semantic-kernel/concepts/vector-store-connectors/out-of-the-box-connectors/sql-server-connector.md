---
title: Using the Semantic Kernel SQL Server Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Azure SQL Server.
zone_pivot_groups: programming-languages
author: rwjdk
ms.topic: conceptual
ms.author: 
ms.date: 03/22/2025
ms.service: semantic-kernel
---
# Using the SQL Server Vector Store connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

## Overview

The Azure SQL Server Vector Store connector can be used to access and manage data in Azure SQL Server. The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | SQL Server Tabel                                                                                                                 |
| Supported key property types      | <ul><li>ulong</li><li>string</li></ul>                                                                                           |
| Supported data property types     | <ul><li>string</li><li>int</li><li>long</li><li>double</li><li>float</li><li>bool</li><li>DateTimeOffset</li></ul>               |
| Supported vector property types   | ReadOnlyMemory\<float\>                                                                                                          |
| Supported index types             | <ul><li>Flat</li></ul>                                                                                                           |
| Supported distance functions      | <ul><li>CosineDistance</li><li>EuclideanDistance</li><li>EuclideanDistance</li><li>NegativeDotProductSimilarity</li></ul>        |
| Supported filter clauses          | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                                                  |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsFilterable supported?           | Yes                                                                                                                              |
| IsFullTextSearchable supported?   | No                                                                                                                               |
| StoragePropertyName supported?    | Yes                                                                                                                              |

## Limitations

Only Azure SQL is supported. Not on-premise SQL Server.

::: zone pivot="programming-language-csharp"

## Getting started

Add the SQL Server Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.SqlServer --prerelease
```

Create your SqlServerVectorStore instance.

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.SqlServer;

IVectorStore sqlServerVectorStore = new SqlServerVectorStore("sql-server-connection-string");

```

::: zone-end
::: zone pivot="programming-language-python"

## Getting started

More info coming soon.

::: zone-end
::: zone pivot="programming-language-java"

## Getting started

More info coming soon.
::: zone-end

::: zone pivot="programming-language-csharp"

## Data mapping

SQL Server Vector Store provides mapping data from the data model to storage via properties to SQL tabel columns.

### Property name override

You can override property names to use in storage that is different to the property names on the data model.
The property name override is done by setting the `StoragePropertyName` option via the data model property attributes or record definition.

Here is an example of a data model with `StoragePropertyName` set on its attributes and how that will be represented in SQL Server query.

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

    [VectorStoreRecordVector(Dimensions: 1536, DistanceFunction.CosineDistance, IndexKind.Flat)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```
CREATE TABLE Hotels (
    HotelId INT PRIMARY KEY,
    hotel_name NVARCHAR,
    hotel_description NVARCHAR
);
```
::: zone-end
::: zone pivot="programming-language-python"
## Coming soon

More info coming soon.

::: zone-end
::: zone pivot="programming-language-java"
## Coming soon

More info coming soon.

::: zone-end
