---
title: Using the Semantic Kernel SQL Server Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in SQL Server.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 03/21/2024
ms.service: semantic-kernel
---
# Using the SQL Server Vector Store connector (Preview)

::: zone pivot="programming-language-csharp"

> [!WARNING]
> The Sql Server Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

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

The SQL Server Vector Store connector can be used to access and manage data in SQL Server. The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | SQL Server table                                                                                                                 |
| Supported key property types      | <ul><li>int</li><li>long</li><li>string</li><li>Guid</li><li>DateTime</li><li>byte[]</li></ul>                                   |
| Supported data property types     | <ul><li>int</li><li>short</li><li>byte</li><li>long</li><li>Guid</li><li>string</li><li>bool</li><li>float</li><li>double</li><li>decimal</li><li>byte[]</li><li>DateTime</li><li>TimeOnly</li></ul> |
| Supported vector property types   | <ul><li>ReadOnlyMemory\<float\></li><li>Embedding\<float\></li><li>float[]</li></ul>                                             |
| Supported index types             | <ul><li>Flat</li></ul>                                                                                                           |
| Supported distance functions      | <ul><li>CosineDistance</li><li>NegativeDotProductSimilarity</li><li>EuclideanDistance</li></ul>                                  |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsIndexed supported?              | Yes                                                                                                                              |
| IsFullTextIndexed supported?      | No                                                                                                                               |
| StorageName supported?            | Yes                                                                                                                              |
| HybridSearch supported?           | No                                                                                                                               |

## Getting started

Add the SQL Sever Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.SqlServer --prerelease
```

You can add the vector store to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

```csharp
using Microsoft.Extensions.DependencyInjection;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServerVectorStore(_ => "<connectionstring>");
```

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServerVectorStore(_ => "<connectionstring>")
```

You can construct a Sql Server Vector Store instance directly.

```csharp
using Microsoft.SemanticKernel.Connectors.SqlServer;

var vectorStore = new SqlServerVectorStore("<connectionstring>");
```

It is possible to construct a direct reference to a named collection.

```csharp
using Microsoft.SemanticKernel.Connectors.SqlServer;

var collection = new SqlServerCollection<string, Hotel>("<connectionstring>", "skhotels");
```

## Data mapping

The SQL Server Vector Store connector provides a default mapper when mapping from the data model to storage.
This mapper does a direct conversion of the list of properties on the data model to the columns in SQL Server.

### Property name override

You can provide override property names to use in storage that is different to the property names on the data model.
The property name override is done by setting the `StorageName` option via the data model property attributes or record definition.

Here is an example of a data model with `StorageName` set on its attributes and how that will be represented in a SQL Server command.

```csharp
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreKey]
    public ulong HotelId { get; set; }

    [VectorStoreData(StorageName = "hotel_name")]
    public string? HotelName { get; set; }

    [VectorStoreData(StorageName = "hotel_description")]
    public string? Description { get; set; }

    [VectorStoreVector(Dimensions: 4, DistanceFunction = DistanceFunction.CosineDistance)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```sql
CREATE TABLE Hotel (
[HotelId] BIGINT NOT NULL,
[hotel_name] NVARCHAR(MAX),
[hotel_description] NVARCHAR(MAX),
[DescriptionEmbedding] VECTOR(4),
PRIMARY KEY ([HotelId])
);
```

::: zone-end
::: zone pivot="programming-language-python"

## Overview

The [SQL Server](/sql) Vector Store connector is a Vector Store implementation provided by Semantic Kernel that uses Azure SQL as a vector store. Once SQL Server on-prem supports vectors it can also be used with that.

The connector has the following characteristics.

| Feature Area                          | Support                                                                                     |
| ------------------------------------- | ------------------------------------------------------------------------------------------- |
| Collection maps to                    | Table dictionary                                                                            |
| Supported key property types          | <ul><li>str</li><li>int</li></ul>                                                           |
| Supported data property types         | Any type                                                                                    |
| Supported vector property types       | <ul><li>list[float]</li><li>numpy array</li></ul>                                           |
| Supported index types                 | <ul><li>Flat</li></ul>                                                                      |
| Supported distance functions          | <ul><li>Cosine Distance</li><li>Dot Product Similarity</li><li>Euclidean Distance</li></ul> |
| Supports multiple vectors in a record | Yes                                                                                         |
| is_filterable supported?              | Yes                                                                                         |
| is_full_text_searchable supported?    | No                                                                                          |

## Getting started

Add the Semantic Kernel package to your project.

```bash
pip install semantic-kernel[sql]
```

The SQL Server connector uses the [pyodbc](https://pypi.org/project/pyodbc/) package to connect to SQL Server. The extra will install the package, but you will need to install the ODBC driver for SQL Server separately, this differs by platform, see the [Azure SQL Documentation](/azure/azure-sql/database/azure-sql-python-quickstart) for details.

In order for the store and collection to work, it needs a connection string, this can be passed to the constructor or be set in the environment variable `SQL_SERVER_CONNECTION_STRING`. In order to properly deal with vectors, the `LongAsMax=yes` option will be added if not found. It also can use both username/password or integrated security, for the latter, the `DefaultAzureCredential` is used.

In the snippets below, it is assumed that you have a data model class defined named 'DataModel'.

```python
from semantic_kernel.connectors.sql_server import SqlServerStore

vector_store = SqlServerStore()

# OR

vector_store = SqlServerStore(connection_string="Driver={ODBC Driver 18 for SQL Server};Server=server_name;Database=database_name;UID=user;PWD=password;LongAsMax=yes;")

vector_collection = vector_store.get_collection("dbo.table_name", DataModel)
```

It is possible to construct a direct reference to a named collection.

```python
from semantic_kernel.connectors.sql_server import SqlServerCollection

vector_collection = SqlServerCollection("dbo.table_name", DataModel)
```

> Note: The collection name can be specified as a simple string (e.g. `table_name`) or as a fully qualified name (e.g. `dbo.table_name`). The latter is recommended to avoid ambiguity, if no schema is specified, the default schema (`dbo`) will be used.

When you have specific requirements for the connection, you can also pass in a `pyodbc.Connection` object to the `SqlServerStore` constructor. This allows you to use a custom connection string or other connection options:

```python
from semantic_kernel.connectors.sql_server import SqlServerStore
import pyodbc

# Create a connection to the SQL Server database
connection = pyodbc.connect("Driver={ODBC Driver 18 for SQL Server};Server=server_name;Database=database_name;UID=user;PWD=password;LongAsMax=yes;")
# Create a SqlServerStore with the connection
vector_store = SqlServerStore(connection=connection)
```

You will have to make sure to close the connection yourself, as the store or collection will not do that for you.

## Custom create queries

The SQL Server connector is limited to the Flat index type.

The `ensure_collection_exists` method on the `SqlServerCollection` allows you to pass in a single or multiple custom queries to create the collection. The queries are executed in the order they are passed in, no results are returned.

If this is done, there is no guarantee that the other methods still work as expected. The connector is not aware of the custom queries and will not validate them.

If the `DataModel` has `id`, `content`, and `vector` as fields, then for instance you could create the table like this in order to also create a index on the content field:

```python
from semantic_kernel.connectors.sql_server import SqlServerCollection

# Create a collection with a custom query
async with SqlServerCollection("dbo.table_name", DataModel) as collection:    
    collection.ensure_collection_exists(
        queries=["CREATE TABLE dbo.table_name (id INT PRIMARY KEY, content NVARCHAR(3000) NULL, vector VECTOR(1536) NULL ) PRIMARY KEY (id);",
        "CREATE INDEX idx_content ON dbo.table_name (content);"]
    )
```

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end
