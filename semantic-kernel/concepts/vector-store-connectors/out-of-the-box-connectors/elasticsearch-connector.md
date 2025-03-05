---
title: Using the Semantic Kernel Elasticsearch Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Elasticsearch.
zone_pivot_groups: programming-languages
author: flobernd
ms.topic: conceptual
ms.author: westey
ms.date: 11/04/2024
ms.service: semantic-kernel
---
# Using the Elasticsearch connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

## Overview

The Elasticsearch Vector Store connector can be used to access and manage data in Elasticsearch. The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | Elasticsearch index                                                                                                              |
| Supported key property types      | string                                                                                                                           |
| Supported data property types     | All types that are supported by System.Text.Json (either built-in or by using a custom converter)                                |
| Supported vector property types   | <ul><li>ReadOnlyMemory\<float\></li><li>IEnumerable\<float\></li></ul>                                                           |
| Supported index types             | <ul><li>HNSW (32, 8, or 4 bit)</li><li>FLAT (32, 8, or 4 bit)</li></ul>                                                          |
| Supported distance functions      | <ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanDistance</li><li>MaxInnerProduct</li></ul>                |
| Supported filter clauses          | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                                                  |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsFilterable supported?           | Yes                                                                                                                              |
| IsFullTextSearchable supported?   | Yes                                                                                                                              |
| StoragePropertyName supported?    | No, use `JsonSerializerOptions` and `JsonPropertyNameAttribute` instead. [See here for more info.](#data-mapping)                |

::: zone pivot="programming-language-csharp"

## Getting started

To [run Elasticsearch locally](https://www.elastic.co/guide/en/elasticsearch/reference/current/run-elasticsearch-locally.html) for local development or testing run the `start-local` script with one command: 

```bash
curl -fsSL https://elastic.co/start-local | sh
```

Add the Elasticsearch Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Elastic.SemanticKernel.Connectors.Elasticsearch --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

```csharp
using Microsoft.SemanticKernel;
using Elastic.Clients.Elasticsearch;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder()
    .AddElasticsearchVectorStore(new ElasticsearchClientSettings(new Uri("http://localhost:9200")));
```

```csharp
using Microsoft.SemanticKernel;
using Elastic.Clients.Elasticsearch;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddElasticsearchVectorStore(new ElasticsearchClientSettings(new Uri("http://localhost:9200")));
```

Extension methods that take no parameters are also provided. These require an instance of the `Elastic.Clients.Elasticsearch.ElasticsearchClient` class to be separately registered with the dependency injection container.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Elastic.Clients.Elasticsearch;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton<ElasticsearchClient>(sp =>
    new ElasticsearchClient(new ElasticsearchClientSettings(new Uri("http://localhost:9200"))));
kernelBuilder.AddElasticsearchVectorStore();
```

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Elastic.Clients.Elasticsearch;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ElasticsearchClient>(sp =>
    new ElasticsearchClient(new ElasticsearchClientSettings(new Uri("http://localhost:9200"))));
builder.Services.AddElasticsearchVectorStore();
```

You can construct an Elasticsearch Vector Store instance directly.

```csharp
using Elastic.SemanticKernel.Connectors.Elasticsearch;
using Elastic.Clients.Elasticsearch;

var vectorStore = new ElasticsearchVectorStore(
    new ElasticsearchClient(new ElasticsearchClientSettings(new Uri("http://localhost:9200"))));
```

It is possible to construct a direct reference to a named collection.

```csharp
using Elastic.SemanticKernel.Connectors.Elasticsearch;
using Elastic.Clients.Elasticsearch;

var collection = new ElasticsearchVectorStoreRecordCollection<Hotel>(
    new ElasticsearchClient(new ElasticsearchClientSettings(new Uri("http://localhost:9200"))),
    "skhotels");
```

## Data mapping

The Elasticsearch connector will use `System.Text.Json.JsonSerializer` to do mapping.
Since Elasticsearch stores documents with a separate key/id and value, the mapper will serialize all properties except for the key to a JSON object
and use that as the value.

Usage of the `JsonPropertyNameAttribute` is supported if a different storage name to the
data model property name is required. It is also possible to use a custom `JsonSerializerOptions` instance with a customized property naming policy. To enable this, 
a custom source serializer must be configured.

```csharp
using Elastic.SemanticKernel.Connectors.Elasticsearch;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Serialization;
using Elastic.Transport;

var nodePool = new SingleNodePool(new Uri("http://localhost:9200"));
var settings = new ElasticsearchClientSettings(
    nodePool,
    sourceSerializer: (defaultSerializer, settings) =>
        new DefaultSourceSerializer(settings, options => 
            options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper));
var client = new ElasticsearchClient(settings);

var collection = new ElasticsearchVectorStoreRecordCollection<Hotel>(
    client,
    "skhotelsjson");
```

As an alternative, the `DefaultFieldNameInferrer` lambda function can be configured to achieve the same result or to even further customize property naming based on dynamic conditions.

```csharp
using Elastic.SemanticKernel.Connectors.Elasticsearch;
using Elastic.Clients.Elasticsearch;

var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"));
settings.DefaultFieldNameInferrer(name => JsonNamingPolicy.SnakeCaseUpper.ConvertName(name));
var client = new ElasticsearchClient(settings);

var collection = new ElasticsearchVectorStoreRecordCollection<Hotel>(
    client,
    "skhotelsjson");
```

Since a naming policy of snake case upper was chosen, here is an example of how this data type will be set in Elasticsearch.
Also note the use of `JsonPropertyNameAttribute` on the `Description` property to further customize the storage naming.

```csharp
using System.Text.Json.Serialization;
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreRecordKey]
    public string HotelId { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public string HotelName { get; set; }

    [JsonPropertyName("HOTEL_DESCRIPTION")]
    [VectorStoreRecordData(IsFullTextSearchable = true)]
    public string Description { get; set; }

    [VectorStoreRecordVector(Dimensions: 4, DistanceFunction.CosineSimilarity, IndexKind.Hnsw)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
```

```json
{
  "_index" : "skhotelsjson",
  "_id" : "h1",
  "_source" : {
    "HOTEL_NAME" : "Hotel Happy",
    "HOTEL_DESCRIPTION" : "A place where everyone can be happy.",
    "DESCRIPTION_EMBEDDING" : [
      0.9,
      0.1,
      0.1,
      0.1
    ]
  }
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
