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

```csharp
using Microsoft.SemanticKernel;

var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddPostgresVectorStore("<Connection String>");
```

Where `<Connection String>` is a connection string to the Postgres instance, in the format that [Npgsql](https://www.npgsql.org/) expects, for example `Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres`.

Extension methods that take no parameters are also provided. These require an instance of [NpgsqlDataSource](https://www.npgsql.org/doc/api/Npgsql.NpgsqlDataSource.html) to be separately registered with the dependency injection container. Note that `UseVector` must be called on the builder to enable vector support via [pgvector-dotnet](https://github.com/pgvector/pgvector-dotnet?tab=readme-ov-file#npgsql-c):

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Npgsql;

var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton<NpgsqlDataSource>(sp => 
{
    NpgsqlDataSourceBuilder dataSourceBuilder = new("<Connection String>");
    dataSourceBuilder.UseVector();
    return dataSourceBuilder.Build();
});
kernelBuilder.Services.AddPostgresVectorStore();
```

You can construct a Postgres Vector Store instance directly.

```csharp
using Microsoft.SemanticKernel.Connectors.Postgres;
using Npgsql;

NpgsqlDataSourceBuilder dataSourceBuilder = new("<Connection String>");
dataSourceBuilder.UseVector();
using NpgsqlDataSource dataSource = dataSourceBuilder.Build();
var vectorStore = new PostgresVectorStore(dataSource);
```

It is possible to construct a direct reference to a named collection.

```csharp
using Microsoft.SemanticKernel.Connectors.Postgres;
using Npgsql;

NpgsqlDataSourceBuilder dataSourceBuilder = new("<Connection String>");
dataSourceBuilder.UseVector();
using NpgsqlDataSource dataSource = dataSourceBuilder.Build();
var collection = new PostgresVectorStoreRecordCollection<int, Hotel>(dataSource, "skhotels");
```

## Data mapping

The Postgres connector provides a default mapper when mapping data from the data model to storage.
The default mapper uses the model annotations or record definition to determine the type of each property and to map the model
into a Dictionary that can be serialized to Postgres.

- The data model property annotated as a key will be mapped to the PRIMARY KEY in the Postgres table.
- The data model properties annotated as data will be mapped to a table column in Postgres.
- The data model properties annotated as vectors will be mapped to a table column that has the pgvector `VECTOR` type in Postgres.

It's also possible to override the default mapper behavior by providing a custom mapper via the `PostgresVectorStoreRecordCollectionOptions<TRecord>.DictionaryCustomMapper` property.

### Property name override

For data properties and vector properties (if using named vectors mode), you can provide override field names to use in storage that is different from the
property names on the data model.

The property name override is done by setting the `StoragePropertyName` option via the data model attributes or record definition.

Here is an example of a data model with `StoragePropertyName` set on its attributes and how it will be represented in Postgres as a table, assuming the Collection name is `Hotels`.

```csharp
using System;
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreRecordKey(StoragePropertyName = "hotel_id")]
    public int HotelId { get; set; }

    [VectorStoreRecordData(StoragePropertyName = "hotel_name")]
    public string HotelName { get; set; }

    [VectorStoreRecordData(StoragePropertyName = "hotel_description")]
    public string Description { get; set; }

    [VectorStoreRecordVector(Dimensions: 4, DistanceFunction: DistanceFunction.CosineDistance, IndexKind: IndexKind.Hnsw, StoragePropertyName = "hotel_description_embedding")]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```sql
CREATE TABLE IF NOT EXISTS public."Hotels" (
    "hotel_id" INTEGER PRIMARY KEY NOT NULL,
    hotel_name TEXT,
    hotel_description TEXT,
    hotel_description_embedding VECTOR(4)
);
```

## Vector indexing

The `hotel_description_embedding` in the above `Hotel` model is a vector property with `IndexKind.HNSW` 
indexing. This index will be created automatically when the collection is created. 
HNSW is the only index type supported for index creation. IVFFlat index building requires that data already 
exist in the table at index creation time, and so it is not appropriate for the creation of an empty table. 
You are free to create and modify indexes on tables outside of the connector, which will 
be used by the connector when performing queries.

## Using with Entra Authentication

Azure Database for PostgreSQL provides the ability to connect to your database using [Entra authentication](/azure/postgresql/flexible-server/concepts-azure-ad-authentication).
This removes the need to store a username and password in your connection string.
To use Entra authentication for an Azure DB for PostgreSQL database, you can use the following Npgsql extension method and set a connection string that does not have a username or password:

```csharp
using System.Text;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using Npgsql;

namespace Program;

public static class NpgsqlDataSourceBuilderExtensions
{
    private static readonly TokenRequestContext s_azureDBForPostgresTokenRequestContext = new(["https://ossrdbms-aad.database.windows.net/.default"]);

    public static NpgsqlDataSourceBuilder UseEntraAuthentication(this NpgsqlDataSourceBuilder dataSourceBuilder, TokenCredential? credential = default)
    {
        credential ??= new DefaultAzureCredential();

        if (dataSourceBuilder.ConnectionStringBuilder.Username == null)
        {
            var token = credential.GetToken(s_azureDBForPostgresTokenRequestContext, default);
            SetUsernameFromToken(dataSourceBuilder, token.Token);
        }

        SetPasswordProvider(dataSourceBuilder, credential, s_azureDBForPostgresTokenRequestContext);

        return dataSourceBuilder;
    }

    public static async Task<NpgsqlDataSourceBuilder> UseEntraAuthenticationAsync(this NpgsqlDataSourceBuilder dataSourceBuilder, TokenCredential? credential = default, CancellationToken cancellationToken = default)
    {
        credential ??= new DefaultAzureCredential();

        if (dataSourceBuilder.ConnectionStringBuilder.Username == null)
        {
            var token = await credential.GetTokenAsync(s_azureDBForPostgresTokenRequestContext, cancellationToken).ConfigureAwait(false);
            SetUsernameFromToken(dataSourceBuilder, token.Token);
        }

        SetPasswordProvider(dataSourceBuilder, credential, s_azureDBForPostgresTokenRequestContext);

        return dataSourceBuilder;
    }

    private static void SetPasswordProvider(NpgsqlDataSourceBuilder dataSourceBuilder, TokenCredential credential, TokenRequestContext tokenRequestContext)
    {
        dataSourceBuilder.UsePasswordProvider(_ =>
        {
            var token = credential.GetToken(tokenRequestContext, default);
            return token.Token;
        }, async (_, ct) =>
        {
            var token = await credential.GetTokenAsync(tokenRequestContext, ct).ConfigureAwait(false);
            return token.Token;
        });
    }

    private static void SetUsernameFromToken(NpgsqlDataSourceBuilder dataSourceBuilder, string token)
    {
        var username = TryGetUsernameFromToken(token);

        if (username != null)
        {
            dataSourceBuilder.ConnectionStringBuilder.Username = username;
        }
        else
        {
            throw new Exception("Could not determine username from token claims");
        }
    }

    private static string? TryGetUsernameFromToken(string jwtToken)
    {
        // Split the token into its parts (Header, Payload, Signature)
        var tokenParts = jwtToken.Split('.');
        if (tokenParts.Length != 3)
        {
            return null;
        }

        // The payload is the second part, Base64Url encoded
        var payload = tokenParts[1];

        // Add padding if necessary
        payload = AddBase64Padding(payload);

        // Decode the payload from Base64Url
        var decodedBytes = Convert.FromBase64String(payload);
        var decodedPayload = Encoding.UTF8.GetString(decodedBytes);

        // Parse the decoded payload as JSON
        var payloadJson = JsonSerializer.Deserialize<JsonElement>(decodedPayload);

        // Try to get the username from 'upn', 'preferred_username', or 'unique_name' claims
        if (payloadJson.TryGetProperty("upn", out var upn))
        {
            return upn.GetString();
        }
        else if (payloadJson.TryGetProperty("preferred_username", out var preferredUsername))
        {
            return preferredUsername.GetString();
        }
        else if (payloadJson.TryGetProperty("unique_name", out var uniqueName))
        {
            return uniqueName.GetString();
        }

        return null;
    }

    private static string AddBase64Padding(string base64) => (base64.Length % 4) switch
    {
        2 => base64 + "==",
        3 => base64 + "=",
        _ => base64,
    };
}
```

Now you can use the `UseEntraAuthentication` method to set up the connection string for the Postgres connector:

```csharp
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.UseEntraAuthentication();
dataSourceBuilder.UseVector();
using var dataSource = dataSourceBuilder.Build();

var vectorStore = new PostgresVectorStore(dataSource);
```

By default, the `UseEntraAuthentication` method uses the [DefaultAzureCredential](/dotnet/api/azure.identity.defaultazurecredential) to authenticate with Azure AD. You can also provide a custom `TokenCredential` implementation if needed.

::: zone-end
::: zone pivot="programming-language-python"

## Getting started

Install semantic kernel with the Postgres extras, which includes the [Postgres client](https://github.com/Postgres/Postgres-client).

```cli
pip install semantic-kernel[postgres]
```

You can then create a vector store instance using the `PostgresStore` class.
You can pass a `psycopg_pool` [AsyncConnectionPool](https://www.psycopg.org/psycopg3/docs/api/pool.html#psycopg_pool.AsyncConnectionPool) directly,
or use the `PostgresSettings` to create a connection pool from environment variables.

```python
from semantic_kernel.connectors.memory.postgres import PostgresStore, PostgresSettings

settings = PostgresSettings()

pool = await settings.create_connection_pool()
async with pool:
    vector_store = PostgresStore(connection_pool=pool)
    ...
```

You can set `POSTGRES_CONNECTION_STRING` in your environment, or the environment variables `PGHOST`, `PGPORT`, `PGUSER`, `PGPASSWORD`, `PGDATABASE`, and optionally `PGSSLMODE` as defined for [libpq](https://www.postgresql.org/docs/current/libpq-envars.html).
These values will be used by the `PostgresSettings` class to create a connection pool.

You can also create a collection directly. The Collection itself is a context manager, so you can use it in a `with` block. If you don't pass
in a connection pool, the collection will create one using the `PostgresSettings` class.

```python
from semantic_kernel.connectors.memory.postgres import PostgresCollection

collection = PostgresCollection(collection_name="skhotels", data_model_type=Hotel)
async with collection:  # This will create a connection pool using PostgresSettings
    ...
```

## Data mapping

The Postgres connector provides a default mapper when mapping data from the data model to storage.
The default mapper uses the model annotations or record definition to determine the type of each property and to map the model
into a `dict` that can be serialized to Postgres rows.

- The data model property annotated as a key will be mapped to the PRIMARY KEY in the Postgres table.
- The data model properties annotated as data will be mapped to a table column in Postgres.
- The data model properties annotated as vectors will be mapped to a table column that has the pgvector `VECTOR` type in Postgres.

```python
from pydantic import BaseModel

from semantic_kernel.connectors.memory.postgres import PostgresCollection
from semantic_kernel.data import (
    DistanceFunction,
    IndexKind,
    VectorStoreRecordDataField,
    VectorStoreRecordKeyField,
    VectorStoreRecordVectorField,
    vectorstoremodel,
)

@vectorstoremodel
class Hotel(BaseModel):
    hotel_id: Annotated[int, VectorStoreRecordKeyField()]
    hotel_name: Annotated[str, VectorStoreRecordDataField()]
    hotel_description: Annotated[str, VectorStoreRecordDataField(has_embedding=True, embedding_property_name="hotel_description_embedding")]
    hotel_description_embedding: Annotated[
        list[float] | None,
        VectorStoreRecordVectorField(
            index_kind=IndexKind.HNSW,
            dimensions=4,
            distance_function=DistanceFunction.COSINE_SIMILARITY,
        ),
    ] = None

collection = PostgresCollection(collection_name="Hotels", data_model_type=Hotel)

async with collection:
    await collection.create_collection_if_not_exists()
```

```sql
CREATE TABLE IF NOT EXISTS public."Hotels" (
    "hotel_id" INTEGER PRIMARY KEY NOT NULL,
    "hotel_name" TEXT,
    "hotel_description" TEXT,
    "hotel_description_embedding" VECTOR(4)
);
```

## Vector indexing

The `hotel_description_embedding` in the above `Hotel` model is a vector property with `IndexKind.HNSW` 
indexing. This index will be created automatically when the collection is created. 
HNSW is the only index type supported for index creation. IVFFlat index building requires that data already 
exist in the table at index creation time, and so it is not appropriate for the creation of an empty table. 
You are free to create and modify indexes on tables outside of the connector, which will 
be used by the connector when performing queries.

## Using with Entra Authentication

Azure Database for PostgreSQL provides the ability to connect to your database using [Entra authentication](/azure/postgresql/flexible-server/concepts-azure-ad-authentication).
This removes the need to store a username and password in your connection string.
To use Entra authentication for an Azure DB for PostgreSQL database, you can use the following custom AsyncConnection class:

```python
import base64
import json
import logging
from functools import lru_cache

from azure.core.credentials import TokenCredential
from azure.core.credentials_async import AsyncTokenCredential
from azure.identity import DefaultAzureCredential
from psycopg import AsyncConnection

AZURE_DB_FOR_POSTGRES_SCOPE = "https://ossrdbms-aad.database.windows.net/.default"

logger = logging.getLogger(__name__)

async def get_entra_token_async(credential: AsyncTokenCredential) -> str:
    """Get the password from Entra using the provided credential."""
    logger.info("Acquiring Entra token for postgres password")

    async with credential:
        cred = await credential.get_token(AZURE_DB_FOR_POSTGRES_SCOPE)
        return cred.token

def get_entra_token(credential: TokenCredential | None) -> str:
    """Get the password from Entra using the provided credential."""
    logger.info("Acquiring Entra token for postgres password")
    credential = credential or get_default_azure_credentials()

    return credential.get_token(AZURE_DB_FOR_POSTGRES_SCOPE).token

@lru_cache(maxsize=1)
def get_default_azure_credentials() -> DefaultAzureCredential:
    """Get the default Azure credentials.

    This method caches the credentials to avoid creating new instances.
    """
    return DefaultAzureCredential()

def decode_jwt(token):
    """Decode the JWT payload to extract claims."""
    payload = token.split(".")[1]
    padding = "=" * (4 - len(payload) % 4)
    decoded_payload = base64.urlsafe_b64decode(payload + padding)
    return json.loads(decoded_payload)

async def get_entra_conninfo(credential: TokenCredential | AsyncTokenCredential | None) -> dict[str, str]:
    """Fetches a token returns the username and token."""
    # Fetch a new token and extract the username
    if isinstance(credential, AsyncTokenCredential):
        token = await get_entra_token_async(credential)
    else:
        token = get_entra_token(credential)
    claims = decode_jwt(token)
    username = claims.get("upn") or claims.get("preferred_username") or claims.get("unique_name")
    if not username:
        raise ValueError("Could not extract username from token. Have you logged in?")

    return {"user": username, "password": token}

class AsyncEntraConnection(AsyncConnection):
    """Asynchronous connection class for using Entra auth with Azure DB for PostgreSQL."""

    @classmethod
    async def connect(cls, *args, **kwargs):
        """Establish an asynchronous connection using Entra auth with Azure DB for PostgreSQL."""
        credential = kwargs.pop("credential", None)
        if credential and not isinstance(credential, (TokenCredential, AsyncTokenCredential)):
            raise ValueError("credential must be a TokenCredential or AsyncTokenCredential")
        if not kwargs.get("user") or not kwargs.get("password"):
            credential = credential or get_default_azure_credentials()
            entra_conninfo = await get_entra_conninfo(credential)
            if kwargs.get("user"):
                entra_conninfo.pop("user", None)
            kwargs.update(entra_conninfo)
        return await super().connect(*args, **kwargs)
```

You can use the custom connection class with the `PostgresSettings.get_connection_pool` method to create a connection pool.

```python
from semantic_kernel.connectors.memory.postgres import PostgresSettings, PostgresStore


pool = await PostgresSettings().create_connection_pool(connection_class=AsyncEntraConnection)
async with pool:
    vector_store = PostgresStore(connection_pool=pool)
    ...
```

By default, the `AsyncEntraConnection` class uses the [DefaultAzureCredential](/python/api/azure-identity/azure.identity.defaultazurecredential) to authenticate with Azure AD. 
You can also provide another `TokenCredential` in the kwargs if needed:

```python
from azure.identity import ManagedIdentityCredential


pool = await PostgresSettings().create_connection_pool(
    connection_class=AsyncEntraConnection, credential=ManagedIdentityCredential()
)
async with pool:
    vector_store = PostgresStore(connection_pool=pool)
    ...
```

::: zone-end
::: zone pivot="programming-language-java"

## JDBC

The [JDBC](./jdbc-connector.md) connector can be used to connect to Postgres.

::: zone-end
