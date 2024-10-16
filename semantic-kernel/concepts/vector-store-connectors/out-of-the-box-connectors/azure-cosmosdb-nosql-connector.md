---
title: Using the Semantic Kernel Azure CosmosDB NoSQL Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Azure CosmosDB NoSQL.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 09/23/2024
ms.service: semantic-kernel
---
# Using the Azure CosmosDB NoSQL Vector Store connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone pivot="programming-language-csharp"

## Overview

The Azure CosmosDB NoSQL Vector Store connector can be used to access and manage data in Azure CosmosDB NoSQL. The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | Azure Cosmos DB NoSQL Container                                                                                                  |
| Supported key property types      | <ul><li>string</li><li>AzureCosmosDBNoSQLCompositeKey</li></ul>                                                                  |
| Supported data property types     | <ul><li>string</li><li>int</li><li>long</li><li>double</li><li>float</li><li>bool</li><li>DateTimeOffset</li><li>*and enumerables of each of these types*</li></ul> |
| Supported vector property types   | <ul><li>ReadOnlyMemory\<float\></li><li>ReadOnlyMemory\<byte\></li><li>ReadOnlyMemory\<sbyte\></li><li>ReadOnlyMemory\<Half\></li></ul> |
| Supported index types             | <ul><li>Flat</li><li>QuantizedFlat</li><li>DiskAnn</li></ul>                                                                     |
| Supported distance functions      | <ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanDistance</li></ul>                                        |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsFilterable supported?           | Yes                                                                                                                              |
| IsFullTextSearchable supported?   | Yes                                                                                                                              |
| StoragePropertyName supported?    | No, use `JsonSerializerOptions` and `JsonPropertyNameAttribute` instead. [See here for more info.](#data-mapping)                |

## Getting started

Add the Azure CosmosDB NoSQL Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.AzureCosmosDBNoSQL --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

```csharp
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder()
    .AddAzureCosmosDBNoSQLVectorStore(connectionString, databaseName);
```

```csharp
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAzureCosmosDBNoSQLVectorStore(connectionString, databaseName);
```

Extension methods that take no parameters are also provided. These require an instance of `Microsoft.Azure.Cosmos.Database` to be separately registered with the dependency injection container.

```csharp
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton<Database>(
    sp =>
    {
        var cosmosClient = new CosmosClient(connectionString);
        return cosmosClient.GetDatabase(databaseName);
    });
kernelBuilder.AddAzureCosmosDBNoSQLVectorStore();
```

```csharp
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Database>(
    sp =>
    {
        var cosmosClient = new CosmosClient(connectionString);
        return cosmosClient.GetDatabase(databaseName);
    });
builder.Services.AddAzureCosmosDBNoSQLVectorStore();
```

You can construct an Azure CosmosDB NoSQL Vector Store instance directly.

```csharp
using Microsoft.Azure.Cosmos;
using Microsoft.SemanticKernel.Connectors.AzureCosmosDBNoSQL;

var cosmosClient = new CosmosClient(connectionString);
var database = cosmosClient.GetDatabase(databaseName);
var vectorStore = new AzureCosmosDBNoSQLVectorStore(database);
```

It is possible to construct a direct reference to a named collection.

```csharp
using Microsoft.Azure.Cosmos;
using Microsoft.SemanticKernel.Connectors.AzureCosmosDBNoSQL;

var cosmosClient = new CosmosClient(connectionString);
var database = cosmosClient.GetDatabase(databaseName);
var collection = new AzureCosmosDBNoSQLVectorStoreRecordCollection<Hotel>(
    database,
    "skhotels");
```

## Data mapping

The Azure CosmosDB NoSQL Vector Store connector provides a default mapper when mapping from the data model to storage.

This mapper does a direct conversion of the list of properties on the data model to the fields in Azure CosmosDB NoSQL and uses `System.Text.Json.JsonSerializer`
to convert to the storage schema. This means that usage of the `JsonPropertyNameAttribute` is supported if a different storage name to the
data model property name is required. The only exception is the key of the record which is mapped to a database field named `id`, since all CosmosDB NoSQL
records must use this name for ids.

It is also possible to use a custom `JsonSerializerOptions` instance with a customized property naming policy. To enable this, the `JsonSerializerOptions`
must be passed to the `AzureCosmosDBNoSQLVectorStoreRecordCollection` on construction.

```csharp
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.SemanticKernel.Connectors.AzureCosmosDBNoSQL;

var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper };

var cosmosClient = new CosmosClient(connectionString);

var database = cosmosClient.GetDatabase(databaseName);
var collection = new AzureCosmosDBNoSQLVectorStoreRecordCollection<Hotel>(
    database,
    "skhotels",
    new() { JsonSerializerOptions = jsonSerializerOptions });
```

Using the above custom `JsonSerializerOptions` which is using `SnakeCaseUpper`, the following data model will be mapped to the below json.

```csharp
using System.Text.Json.Serialization;
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreRecordKey]
    public ulong HotelId { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public string HotelName { get; set; }

    [VectorStoreRecordData(IsFullTextSearchable = true)]
    public string Description { get; set; }

    [JsonPropertyName("HOTEL_DESCRIPTION_EMBEDDING")]
    [VectorStoreRecordVector(4, DistanceFunction.EuclideanDistance, IndexKind.QuantizedFlat)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```json
{
    "id": 1,
    "HOTEL_NAME": "Hotel Happy",
    "DESCRIPTION": "A place where everyone can be happy.",
    "HOTEL_DESCRIPTION_EMBEDDING": [0.9, 0.1, 0.1, 0.1],
}
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
