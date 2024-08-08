---
title: Using the Semantic Kernel Redis Vector Store connector (Experimental)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Redis.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Using the Redis connector (Experimental)

## Overview

The Redis Vector Store connector can be used to access and manage data in Redis. The connector supports both Hashes and JSON modes and which mode you pick will determine what other features are supported.

The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | Redis index with prefix set to `<collectionname>:`                                                                               |
| Supported key property types      | string                                                                                                                           |
| Supported data property types     | **When using Hashes:**<br>string<br>int<br>uint<br>long<br>ulong<br>double<br>float<br>bool<br><br>**When using JSON:**<br>Any types serializable to JSON|
| Supported vector property types   | ReadOnlyMemory\<float\><br>ReadOnlyMemory\<double\>                                                                              |
| Supported index types             | Hnsw<br>Flat                                                                                                                     |
| Supported distance functions      | CosineSimilarity<br>DotProductSimilarity<br>EuclideanDistance                                                                    |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsFilterable supported?           | Yes                                                                                                                              |
| IsFullTextSearchable supported?   | Yes                                                                                                                              |
| StoragePropertyName supported?    | **When using Hashes:** Yes<br>**When using JSON:** No, use `JsonSerializerOptions` and `JsonPropertyNameAttribute` instead. [See here for more info.](#data-mapping) |

::: zone pivot="programming-language-csharp"

## Getting started

Add the Redis Vector Store connector nuget package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.Redis --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the to the `IServiceCollection` dependency injection container using extention methods provided by Semantic Kernel.

```csharp
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder()
    .AddRedisVectorStore("localhost:6379");

// Using IServiceCollection.
serviceCollection.AddRedisVectorStore("localhost:6379");
```

Extension methods are also provided that take no parameters. These require an instance of the Redis `IDatabase` to be separately registered with the dependency injection container.

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Redis;
using StackExchange.Redis;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton<IDatabase>(sp => ConnectionMultiplexer.Connect("localhost:6379").GetDatabase());
kernelBuilder.AddRedisVectorStore();

// Using IServiceCollection.
serviceCollection.AddSingleton<IDatabase>(sp => ConnectionMultiplexer.Connect("localhost:6379").GetDatabase());
serviceCollection.AddRedisVectorStore();
```

You can construct a Redis Vector Store instance directly.

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Redis;
using StackExchange.Redis;

var vectorStore = new RedisVectorStore(ConnectionMultiplexer.Connect("localhost:6379").GetDatabase());
```

It is possible to construct a direct reference to a named collection.
When doing so, you have to choose between the JSON or Hashes instance depending on how you wish to store data in redis.

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Redis;
using StackExchange.Redis;

// Using Hashes.
var hashesCollection = new RedisHashSetVectorStoreRecordCollection<Hotel>(
    ConnectionMultiplexer.Connect("localhost:6379").GetDatabase(),
    "skhotelshashes");

// Using JSON.
var jsonCollection = new RedisJsonVectorStoreRecordCollection<Hotel>(
    ConnectionMultiplexer.Connect("localhost:6379").GetDatabase(),
    "skhotelsjson");
```

When constructing a `RedisVectorStore` or registering it with the dependency injection container, it's possible to pass a `RedisVectorStoreOptions` instance
that configures the preferred storage type / mode used: Hashes or JSON. If not specified, the default is JSON.

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Redis;
using StackExchange.Redis;

var vectorStore = new RedisVectorStore(
    ConnectionMultiplexer.Connect("localhost:6379").GetDatabase(),
    new() { StorageType = RedisStorageType.HashSet });
```

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end

## Index prefixes

Redis uses a system of key prefixing to associate a record with an index.
When creating an index you can specify one or more prefixes to use with that index.
If you want to associate a record with that index, you have to add the prefix to the key of that record.

E.g. If you create a index called `skhotelsjson` with a prefix of `skhotelsjson:`, when setting a record
with key `h1`, the record key will need to be prefixed like this `skhotelsjson:h1` to be added to the index.

When creating a new collection using the Redis connector, the connector will create an index in redis with a
prefix consisting of the collection name and a colon, like this `<collectionname>:`.
By default, the connector will also prefix all keys with the this prefix when doing record operations like Get, Upsert, and Delete.

If you didn't want to use a prefix consisting of the collection name and a colon, it is possible to switch
off the prefixing behavior and pass in the fully prefixed key to the record operations.

::: zone pivot="programming-language-csharp"

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Redis;
using StackExchange.Redis;

var collection = new RedisJsonVectorStoreRecordCollection<Hotel>(
    ConnectionMultiplexer.Connect("localhost:6379").GetDatabase(),
    "skhotelsjson",
    new() { PrefixCollectionNameToKeyNames = false });

await collection.GetAsync("myprefix_h1");
```

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end

## Data mapping

Redis supports two modes for storing data: JSON and Hashes. The Redis connector supports both storage types, and mapping differs depending on the chosen storage type.

::: zone pivot="programming-language-csharp"

### Data mapping when using the JSON storage type

When using the JSON storage type, the Redis connector will use `System.Text.Json.JsonSerializer` to do mapping.
Since Redis stores records with a separate key and value, the mapper will serialize all properties except for the key to a JSON object
and use that as the value.

Usage of the `JsonPropertyNameAttribute` is supported if a different storage name to the
data model property name is required. It is also possible to use a custom `JsonSerializerOptions` instance with a customized property naming policy. To enable this, the `JsonSerializerOptions`
must be passed to the `RedisJsonVectorStoreRecordCollection` on construction.

```csharp
var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper };
var collection = new RedisJsonVectorStoreRecordCollection<Hotel>(
    ConnectionMultiplexer.Connect("localhost:6379").GetDatabase(),
    "skhotelsjson",
    new() { JsonSerializerOptions = jsonSerializerOptions });
```

Since a naming policy of snake case upper was chosen, here is an example of how this data type will be set in Redis.
Also note the use of `JsonPropertyNameAttribute` on the `Description` property to further customize the storage naming.

```csharp
using Microsoft.SemanticKernel;

public class Hotel
{
    [VectorStoreRecordKey]
    public ulong HotelId { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public string HotelName { get; set; }

    [JsonPropertyNameAttribute("HOTEL_DESCRIPTION")]
    [VectorStoreRecordData(IsFullTextSearchable = true)]
    public string Description { get; set; }

    [VectorStoreRecordVector(4, IndexKind.Hnsw, DistanceFunction.CosineDistance)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```
JSON.SET skhotelsjson:h1 $ '{ "HOTEL_NAME": "Hotel Happy", "HOTEL_DESCRIPTION": "A place where everyone can be happy.", "DESCRIPTION_EMBEDDING": [0.9, 0.1, 0.1, 0.1] }'
```

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end

### Data mapping when using the Hashes storage type

When using the Hashes storage type, the Redis connector provides its own mapper to do mapping.
This mapper will map each property to a field-value pair as supported by the Redis `HSET` command.

For data properties and vector properties, you can provide override field names to use in storage that is different to the
property names on the data model. This is not supported for keys, since keys cannot be named in Redis.
::: zone pivot="programming-language-csharp"
Property name overriding is done by setting the `StoragePropertyName` option via the data model attributes or record definition.

Here is an example of a data model with `StoragePropertyName` set on its attributes and how these are set in Redis.

```csharp
using Microsoft.SemanticKernel;

public class Hotel
{
    [VectorStoreRecordKey]
    public ulong HotelId { get; set; }

    [VectorStoreRecordData(IsFilterable = true) { StoragePropertyName = "hotel_name" }]
    public string HotelName { get; set; }

    [VectorStoreRecordData(IsFullTextSearchable = true) { StoragePropertyName = "hotel_description" }]
    public string Description { get; set; }

    [VectorStoreRecordVector(4, IndexKind.Hnsw, DistanceFunction.CosineDistance) { StoragePropertyName = "hotel_description_embedding" }]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```
HSET skhotelshashes:h1 hotel_name "Hotel Happy" hotel_description 'A place where everyone can be happy.' hotel_description_embedding <vector_bytes>
```

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end
