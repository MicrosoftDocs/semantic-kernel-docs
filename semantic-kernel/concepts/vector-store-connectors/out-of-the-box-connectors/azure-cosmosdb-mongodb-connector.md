---
title: Using the Semantic Kernel Azure CosmosDB MongoDB (vCore) Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Azure CosmosDB MongoDB (vCore).
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 09/23/2024
ms.service: semantic-kernel
---
# Using the Azure CosmosDB MongoDB (vCore) Vector Store connector (Preview)

::: zone pivot="programming-language-csharp"

> [!WARNING]
> The Azure CosmosDB MongoDB (vCore) Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

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

The Azure CosmosDB MongoDB Vector Store connector can be used to access and manage data in Azure CosmosDB MongoDB (vCore). The connector has the following characteristics.

::: zone pivot="programming-language-csharp"

| Feature Area                          | Support                                                                                                                                                                       |
| ------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Collection maps to                    | Azure Cosmos DB MongoDB (vCore) Collection + Index                                                                                                                            |
| Supported key property types          | string                                                                                                                                                                        |
| Supported data property types         | <ul><li>string</li><li>int</li><li>long</li><li>double</li><li>float</li><li>decimal</li><li>bool</li><li>DateTime</li><li>*and enumerables of each of these types*</li></ul> |
| Supported vector property types       | <ul><li>ReadOnlyMemory\<float\></li><li>Embedding\<float\></li><li>float[]</li></ul>                                                                                                    |
| Supported index types                 | <ul><li>Hnsw</li><li>IvfFlat</li></ul>                                                                                                                                        |
| Supported distance functions          | <ul><li>CosineDistance</li><li>DotProductSimilarity</li><li>EuclideanDistance</li></ul>                                                                                       |
| Supported filter clauses              | <ul><li>EqualTo</li></ul>                                                                                                                                                     |
| Supports multiple vectors in a record | Yes                                                                                                                                                                           |
| IsIndexed supported?                  | Yes                                                                                                                                                                           |
| IsFullTextIndexed supported?          | No                                                                                                                                                                            |
| StorageName supported?                | No, use BsonElementAttribute instead. [See here for more info.](#data-mapping)                                                                                                |
| HybridSearch supported?               | No                                                                                                                                                                            |

::: zone-end
::: zone pivot="programming-language-python"

| Feature Area                          | Support                                                                                                                                                                     |
| ------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Collection maps to                    | Azure Cosmos DB MongoDB (vCore) Collection + Index                                                                                                                          |
| Supported key property types          | string                                                                                                                                                                      |
| Supported data property types         | <ul><li>string</li><li>int</li><li>long</li><li>double</li><li>float</li><li>decimal</li><li>bool</li><li>DateTime</li><li>*and iterables of each of these types*</li></ul> |
| Supported vector property types       | <ul><li>list[float]</li><li>list[int]</li><li>ndarray</li></ul>                                                                                                             |
| Supported index types                 | <ul><li>Hnsw</li><li>IvfFlat</li></ul>                                                                                                                                      |
| Supported distance functions          | <ul><li>CosineDistance</li><li>DotProductSimilarity</li><li>EuclideanDistance</li></ul>                                                                                     |
| Supported filter clauses              | <ul><li>EqualTo</li></ul><ul><li>AnyTagsEqualTo</li></ul>                                                                                                                   |
| Supports multiple vectors in a record | Yes                                                                                                                                                                         |
| IsFilterable supported?               | Yes                                                                                                                                                                         |
| IsFullTextSearchable supported?       | No                                                                                                                                                                          |

::: zone-end
::: zone pivot="programming-language-java"
More info coming soon.
::: zone-end

## Limitations

This connector is compatible with Azure Cosmos DB MongoDB (vCore) and is *not* designed to be compatible with Azure Cosmos DB MongoDB (RU).

::: zone pivot="programming-language-csharp"

## Getting started

Add the Azure CosmosDB MongoDB Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.CosmosMongoDB --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder();
kernelBuilder.Services
    .AddCosmosMongoVectorStore(connectionString, databaseName);
```

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCosmosMongoVectorStore(connectionString, databaseName);
```

Extension methods that take no parameters are also provided. These require an instance of `MongoDB.Driver.IMongoDatabase` to be separately registered with the dependency injection container.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using MongoDB.Driver;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton<IMongoDatabase>(
    sp =>
    {
        var mongoClient = new MongoClient(connectionString);
        return mongoClient.GetDatabase(databaseName);
    });
kernelBuilder.Services.AddCosmosMongoVectorStore();
```

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using MongoDB.Driver;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IMongoDatabase>(
    sp =>
    {
        var mongoClient = new MongoClient(connectionString);
        return mongoClient.GetDatabase(databaseName);
    });
builder.Services.AddCosmosMongoVectorStore();
```

You can construct an Azure CosmosDB MongoDB Vector Store instance directly.

```csharp
using Microsoft.SemanticKernel.Connectors.CosmosMongoDB;
using MongoDB.Driver;

var mongoClient = new MongoClient(connectionString);
var database = mongoClient.GetDatabase(databaseName);
var vectorStore = new CosmosMongoVectorStore(database);
```

It is possible to construct a direct reference to a named collection.

```csharp
using Microsoft.SemanticKernel.Connectors.CosmosMongoDB;
using MongoDB.Driver;

var mongoClient = new MongoClient(connectionString);
var database = mongoClient.GetDatabase(databaseName);
var collection = new CosmosMongoCollection<ulong, Hotel>(
    database,
    "skhotels");
```

## Data mapping

The Azure CosmosDB MongoDB Vector Store connector provides a default mapper when mapping data from the data model to storage.

This mapper does a direct conversion of the list of properties on the data model to the fields in Azure CosmosDB MongoDB and uses `MongoDB.Bson.Serialization`
to convert to the storage schema. This means that usage of the `MongoDB.Bson.Serialization.Attributes.BsonElement` is supported if a different storage name to the
data model property name is required. The only exception is the key of the record which is mapped to a database field named `_id`, since all CosmosDB MongoDB
records must use this name for ids.

### Property name override

For data properties and vector properties, you can provide override field names to use in storage that is different to the
property names on the data model. This is not supported for keys, since a key has a fixed name in MongoDB.

The property name override is done by setting the `BsonElement` attribute on the data model properties.

Here is an example of a data model with `BsonElement` set.

```csharp
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreKey]
    public ulong HotelId { get; set; }

    [BsonElement("hotel_name")]
    [VectorStoreData(IsIndexed = true)]
    public string HotelName { get; set; }

    [BsonElement("hotel_description")]
    [VectorStoreData(IsFullTextIndexed = true)]
    public string Description { get; set; }

    [BsonElement("hotel_description_embedding")]
    [VectorStoreVector(4, DistanceFunction = DistanceFunction.CosineDistance, IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

::: zone-end
::: zone pivot="programming-language-python"

## Getting started

Add the Azure CosmosDB MongoDB Vector Store dependencies to your environment. Because the Azure CosmosDB MongoDB connector is built on the MongoDB Atlas connector and uses the same client as that one, you need to install with these extras:

```bash
pip install semantic-kernel[azure, mongo]
```

You can then create the vector store.

```python
from semantic_kernel.connectors.memory.azure_cosmos_db import AzureCosmosDBforMongoDBStore

# If the right environment settings are set, namely AZURE_COSMOS_DB_MONGODB_CONNECTION_STRING and optionally AZURE_COSMOS_DB_MONGODB_DATABASE_NAME, this is enough to create the Store:
store = AzureCosmosDBforMongoDBStore()
```

Alternatively, you can also pass in your own mongodb client if you want to have more control over the client construction:

```python
from pymongo import AsyncMongoClient
from semantic_kernel.connectors.memory.azure_cosmos_db import AzureCosmosDBforMongoDBStore

client = AsyncMongoClient(...)
store = AzureCosmosDBforMongoDBStore(mongo_client=client)
```

When a client is passed in, Semantic Kernel will not close the connection for you, so you need to ensure to close it, for instance with a `async with` statement.

You can also create a collection directly, without the store.

```python
from semantic_kernel.connectors.memory.azure_cosmos_db import AzureCosmosDBforMongoDBCollection

# `hotel` is a class created with the @vectorstoremodel decorator
collection = AzureCosmosDBforMongoDBCollection(
    collection_name="my_collection",
    data_model_type=hotel
)
```

## Serialization

Since the Azure CosmosDB for MongoDB  connector needs a simple dict with the fields corresponding to the index as the input, the serialization is quite easy, it only uses a predetermined key `_id`, so we replace the key of the data model with that if it is not already `_id`.

For more details on this concept see the [serialization documentation](./../serialization.md).

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end
