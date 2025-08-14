---
title: Using the Semantic Kernel Couchbase Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Couchbase.
zone_pivot_groups: programming-languages
author: azaddhirajkumar
ms.topic: conceptual
ms.author: westey
ms.date: 01/14/2025
ms.service: semantic-kernel
---

# Using the Couchbase connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone pivot="programming-language-csharp"

## Overview

The Couchbase Vector Store connector can be used to access and manage data in Couchbase. The connector has the
following characteristics.

| Feature Area                          | Support                                                                                                           |
|---------------------------------------|-------------------------------------------------------------------------------------------------------------------|
| Collection maps to                    | Couchbase collection                                                                                              |
| Supported key property types          | string                                                                                                            |
| Supported data property types         | All types that are supported by System.Text.Json (either built-in or by using a custom converter)                 |
| Supported vector property types       | <ul><li>ReadOnlyMemory\<float\></li></ul>                                                                         |
| Supported index types                 | N/A                                                                                                               |
| Supported distance functions          | <ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanDistance</li></ul>                         |
| Supported filter clauses              | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                                   |
| Supports multiple vectors in a record | Yes                                                                                                               |
| IsFilterable supported?               | No                                                                                                                |
| IsFullTextSearchable supported?       | No                                                                                                                |
| StoragePropertyName supported?        | No, use `JsonSerializerOptions` and `JsonPropertyNameAttribute` instead. [See here for more info.](#data-mapping) |
| HybridSearch supported?               | No                                                                                                                |

## Getting Started

Add the Couchbase Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package CouchbaseConnector.SemanticKernel --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to
the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

```csharp
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder()
    .AddCouchbaseVectorStore(
        connectionString: "couchbases://your-cluster-address",
        username: "username",
        password: "password",
        bucketName: "bucket-name",
        scopeName: "scope-name");
```

```csharp
using Microsoft.Extensions.DependencyInjection;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCouchbaseVectorStore(
    connectionString: "couchbases://your-cluster-address",
    username: "username",
    password: "password",
    bucketName: "bucket-name",
    scopeName: "scope-name");
```

Extension methods that take no parameters are also provided. These require an instance of the `IScope` class to be
separately registered with the dependency injection container.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Couchbase;
using Couchbase.KeyValue;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton<ICluster>(sp =>
{
    var clusterOptions = new ClusterOptions
    {
        ConnectionString = "couchbases://your-cluster-address",
        UserName = "username",
        Password = "password"
    };

    return Cluster.ConnectAsync(clusterOptions).GetAwaiter().GetResult();
});

kernelBuilder.Services.AddSingleton<IScope>(sp =>
{
    var cluster = sp.GetRequiredService<ICluster>();
    var bucket = cluster.BucketAsync("bucket-name").GetAwaiter().GetResult();
    return bucket.Scope("scope-name");
});

// Add Couchbase Vector Store
kernelBuilder.Services.AddCouchbaseVectorStore();
```

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Couchbase.KeyValue;
using Couchbase;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ICluster>(sp =>
{
    var clusterOptions = new ClusterOptions
    {
        ConnectionString = "couchbases://your-cluster-address",
        UserName = "username",
        Password = "password"
    };

    return Cluster.ConnectAsync(clusterOptions).GetAwaiter().GetResult();
});

builder.Services.AddSingleton<IScope>(sp =>
{
    var cluster = sp.GetRequiredService<ICluster>();
    var bucket = cluster.BucketAsync("bucket-name").GetAwaiter().GetResult();
    return bucket.Scope("scope-name");
});

// Add Couchbase Vector Store
builder.Services.AddCouchbaseVectorStore();
```

You can construct a Couchbase Vector Store instance directly.

```csharp
using Couchbase;
using Couchbase.KeyValue;
using Couchbase.SemanticKernel;

var clusterOptions = new ClusterOptions
{
    ConnectionString = "couchbases://your-cluster-address",
    UserName = "username",
    Password = "password"
};

var cluster = await Cluster.ConnectAsync(clusterOptions);

var bucket = await cluster.BucketAsync("bucket-name");
var scope = bucket.Scope("scope-name");

var vectorStore = new CouchbaseVectorStore(scope);
```

It is possible to construct a direct reference to a named collection.

```csharp
using Couchbase;
using Couchbase.KeyValue;
using Couchbase.SemanticKernel;

var clusterOptions = new ClusterOptions
{
    ConnectionString = "couchbases://your-cluster-address",
    UserName = "username",
    Password = "password"
};

var cluster = await Cluster.ConnectAsync(clusterOptions);
var bucket = await cluster.BucketAsync("bucket-name");
var scope = bucket.Scope("scope-name");

var collection = new CouchbaseFtsVectorStoreRecordCollection<Hotel>(
    scope,
    "hotelCollection");
```
## Data mapping

The Couchbase connector uses `System.Text.Json.JsonSerializer` for data mapping. Properties in the data model are serialized into a JSON object and mapped to Couchbase storage.

Use the `JsonPropertyName` attribute to map a property to a different name in Couchbase storage. Alternatively, you can configure `JsonSerializerOptions` for advanced customization.
```csharp
using Couchbase.SemanticKernel;
using Couchbase.KeyValue;
using System.Text.Json;

var jsonSerializerOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

var options = new CouchbaseFtsVectorStoreRecordCollectionOptions<Hotel>
{
    JsonSerializerOptions = jsonSerializerOptions
};

var collection = new CouchbaseFtsVectorStoreRecordCollection<Hotel>(scope, "hotels", options);
```
Using the above custom `JsonSerializerOptions` which is using `CamelCase`, the following data model will be mapped to the below json.

```csharp
using System.Text.Json.Serialization;
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [JsonPropertyName("hotelId")]
    [VectorStoreRecordKey]
    public string HotelId { get; set; }

    [JsonPropertyName("hotelName")]
    [VectorStoreRecordData]
    public string HotelName { get; set; }

    [JsonPropertyName("description")]
    [VectorStoreRecordData]
    public string Description { get; set; }

    [JsonPropertyName("descriptionEmbedding")]
    [VectorStoreRecordVector(Dimensions: 4, DistanceFunction.DotProductSimilarity)]
    public ReadOnlyMemory<float> DescriptionEmbedding { get; set; }
}
```

```json
{
  "hotelId": "h1",
  "hotelName": "Hotel Happy",
  "description": "A place where everyone can be happy",
  "descriptionEmbedding": [0.9, 0.1, 0.1, 0.1]
}
```

::: zone-end

::: zone pivot="programming-language-python"

## Not supported

Not supported.

::: zone-end
::: zone pivot="programming-language-java"

## Not supported

Not supported.

::: zone-end