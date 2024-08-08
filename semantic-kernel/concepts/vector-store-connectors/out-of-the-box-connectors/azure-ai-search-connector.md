---
title: Using the Semantic Kernel Azure AI Search Vector Store connector (Experimental)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Azure AI Search.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Using the Azure AI Search Vector Store connector (Experimental)

## Overview

The Azure AI Search Vector Store connector can be used to access and manage data in Azure AI Search. The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | Azure AI Search Index                                                                                                            |
| Supported key property types      | string                                                                                                                           |
| Supported data property types     | string<br>int<br>long<br>double<br>float<br>bool<br>DateTimeOffset<br>*and enumerables of each of these types*                   |
| Supported vector property types   | ReadOnlyMemory\<float\>                                                                                                          |
| Supported index types             | Hnsw<br>Flat                                                                                                                     |
| Supported distance functions      | CosineSimilarity<br>DotProductSimilarity<br>EuclideanDistance                                                                    |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| IsFilterable supported?           | Yes                                                                                                                              |
| IsFullTextSearchable supported?   | Yes                                                                                                                              |
| StoragePropertyName supported?    | No                                                                                                                               |

## Getting started

::: zone pivot="programming-language-csharp"

Add the Azure AI Search Vector Store connector nuget package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.AzureAISearch --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the to the `IServiceCollection` dependency injection container using extention methods provided by Semantic Kernel.

```csharp
using Azure;
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder()
    .AddAzureAISearchVectorStore(new Uri(azureAISearchUri), new AzureKeyCredential(secret));

// Using IServiceCollection.
serviceCollection.AddAzureAISearchVectorStore(new Uri(azureAISearchUri), new AzureKeyCredential(secret));
```

Extension methods are also provided that take no parameters. These require an instance of the Azure AI Search `SearchIndexClient` to be separately registered with the dependency injection container.

```csharp
using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton<SearchIndexClient>(sp => new SearchIndexClient(new Uri(azureAISearchUri), new AzureKeyCredential(secret)));
kernelBuilder.AddAzureAISearchVectorStore();

// Using IServiceCollection.
serviceCollection.AddSingleton<SearchIndexClient>(sp => new SearchIndexClient(new Uri(azureAISearchUri), new AzureKeyCredential(secret)));
serviceCollection.AddAzureAISearchVectorStore();
```

You can construct an Azure AI Search Vector Store instance directly.

```csharp
using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;

var vectorStore = new AzureAISearchVectorStore(new SearchIndexClient(new Uri(azureAISearchUri), new AzureKeyCredential(secret));
```

It is possible to construct a direct reference to a named collection.

```csharp
using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;

var collection = new AzureAISearchVectorStoreRecordCollection<Hotel>(
    new SearchIndexClient(new Uri(azureAISearchUri), new AzureKeyCredential(secret),
    "skhotels");
```

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end

## Data mapping

::: zone pivot="programming-language-csharp"
The default mapper used by the Azure AI Search connector when mapping data from the data model to storage is the one provided by the Azure AI Search SDK.

This mapper does a direct conversion of the list of properties on the data model to the fields in Azure AI Search and uses JSON serialization
to convert to the storage schema. This means that usage of the `JsonPropertyNameAttribute` is supported if a different storage name to the
data model property name is required.

It is also possible to use a custom `JsonSerializerOptions` instance with a customized property naming policy. To enable this, the `JsonSerializerOptions`
must be passed to both the `SearchIndexClient` and the `AzureAISearchVectorStoreRecordCollection` on construction.

```csharp
var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper };
var collection = new AzureAISearchVectorStoreRecordCollection<Hotel>(
    new SearchIndexClient(new Uri(azureAISearchUri), new AzureKeyCredential(secret), new() { Serializer = new JsonObjectSerializer(jsonSerializerOptions) }),
    "skhotels",
    new() { JsonSerializerOptions = jsonSerializerOptions });
```

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end
