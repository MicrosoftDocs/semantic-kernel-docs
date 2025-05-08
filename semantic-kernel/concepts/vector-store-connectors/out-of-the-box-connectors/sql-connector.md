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

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone pivot="programming-language-csharp"

## Coming soon

More info coming soon.

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
from semantic_kernel.connectors.memory.sql_server import SqlServerStore

vector_store = SqlServerStore()

# OR

vector_store = SqlServerStore(connection_string="Driver={ODBC Driver 18 for SQL Server};Server=server_name;Database=database_name;UID=user;PWD=password;LongAsMax=yes;")

vector_collection = vector_store.get_collection("dbo.table_name", DataModel)
```

It is possible to construct a direct reference to a named collection.

```python
from semantic_kernel.connectors.memory.sql_server import SqlServerCollection

vector_collection = SqlServerCollection("dbo.table_name", DataModel)
```

> Note: The collection name can be specified as a simple string (e.g. `table_name`) or as a fully qualified name (e.g. `dbo.table_name`). The latter is recommended to avoid ambiguity, if no schema is specified, the default schema (`dbo`) will be used.

When you have specific requirements for the connection, you can also pass in a `pyodbc.Connection` object to the `SqlServerStore` constructor. This allows you to use a custom connection string or other connection options:

```python
from semantic_kernel.connectors.memory.sql_server import SqlServerStore
import pyodbc

# Create a connection to the SQL Server database
connection = pyodbc.connect("Driver={ODBC Driver 18 for SQL Server};Server=server_name;Database=database_name;UID=user;PWD=password;LongAsMax=yes;")
# Create a SqlServerStore with the connection
vector_store = SqlServerStore(connection=connection)
```

You will have to make sure to close the connection yourself, as the store or collection will not do that for you.

## Custom create queries

The SQL Server connector is limited to the Flat index type.

The `create_collection` method on the `SqlServerCollection` allows you to pass in a single or multiple custom queries to create the collection. The queries are executed in the order they are passed in, no results are returned.

If this is done, there is no guarantee that the other methods still work as expected. The connector is not aware of the custom queries and will not validate them.

If the `DataModel` has `id`, `content`, and `vector` as fields, then for instance you could create the table like this in order to also create a index on the content field:

```python
from semantic_kernel.connectors.memory.sql_server import SqlServerCollection

# Create a collection with a custom query
async with SqlServerCollection("dbo.table_name", DataModel) as collection:    
    collection.create_collection(
        queries=["CREATE TABLE dbo.table_name (id INT PRIMARY KEY, content NVARCHAR(3000) NULL, vector VECTOR(1536) NULL ) PRIMARY KEY (id);",
        "CREATE INDEX idx_content ON dbo.table_name (content);"]
    )
```

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end
