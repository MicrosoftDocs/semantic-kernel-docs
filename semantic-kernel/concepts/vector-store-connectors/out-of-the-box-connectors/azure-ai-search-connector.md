---
title: Using the Semantic Kernel Azure AI Search Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Azure AI Search.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Using the Azure AI Search Vector Store connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

## Overview

The Azure AI Search Vector Store connector can be used to access and manage data in Azure AI Search. The connector has the following characteristics.

::: zone pivot="programming-language-csharp"

| Feature Area                          | Support                                                                                                                                                             |
| ------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Collection maps to                    | Azure AI Search Index                                                                                                                                               |
| Supported key property types          | string                                                                                                                                                              |
| Supported data property types         | <ul><li>string</li><li>int</li><li>long</li><li>double</li><li>float</li><li>bool</li><li>DateTimeOffset</li><li>*and enumerables of each of these types*</li></ul> |
| Supported vector property types       | ReadOnlyMemory\<float\>                                                                                                                                             |
| Supported index types                 | <ul><li>Hnsw</li><li>Flat</li></ul>                                                                                                                                 |
| Supported distance functions          | <ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanDistance</li></ul>                                                                           |
| Supported filter clauses              | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                                                                                     |
| Supports multiple vectors in a record | Yes                                                                                                                                                                 |
| IsIndexed supported?                  | Yes                                                                                                                                                                 |
| IsFullTextIndexed supported?          | Yes                                                                                                                                                                 |
| StoragePropertyName supported?        | No, use `JsonSerializerOptions` and `JsonPropertyNameAttribute` instead. [See here for more info.](#data-mapping)                                                   |
| HybridSearch supported?               | Yes                                                                                                                                                                 |

::: zone-end
::: zone pivot="programming-language-python"

| Feature Area                          | Support                                                                                                                                                           |
| ------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Collection maps to                    | Azure AI Search Index                                                                                                                                             |
| Supported key property types          | string                                                                                                                                                            |
| Supported data property types         | <ul><li>string</li><li>int</li><li>long</li><li>double</li><li>float</li><li>bool</li><li>DateTimeOffset</li><li>*and iterables of each of these types*</li></ul> |
| Supported vector property types       | <ul><li>list[float]</li><li>list[int]</li><li>numpy array</li></ul>                                                                                               |
| Supported index types                 | <ul><li>Hnsw</li><li>Flat</li></ul>                                                                                                                               |
| Supported distance functions          | <ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanDistance</li><li>Hamming</li></ul>                                                         |
| Supported filter clauses              | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                                                                                   |
| Supports multiple vectors in a record | Yes                                                                                                                                                               |
| IsFilterable supported?               | Yes                                                                                                                                                               |
| IsFullTextSearchable supported?       | Yes                                                                                                                                                               |

::: zone-end
::: zone pivot="programming-language-java"

| Feature Area                          | Support                                                                                                                                                             |
| ------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Collection maps to                    | Azure AI Search Index                                                                                                                                               |
| Supported key property types          | string                                                                                                                                                              |
| Supported data property types         | <ul><li>string</li><li>int</li><li>long</li><li>double</li><li>float</li><li>bool</li><li>DateTimeOffset</li><li>*and enumerables of each of these types*</li></ul> |
| Supported vector property types       | ReadOnlyMemory\<float\>                                                                                                                                             |
| Supported index types                 | <ul><li>Hnsw</li><li>Flat</li></ul>                                                                                                                                 |
| Supported distance functions          | <ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanDistance</li></ul>                                                                           |
| Supported filter clauses              | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                                                                                     |
| Supports multiple vectors in a record | Yes                                                                                                                                                                 |
| IsFilterable supported?               | Yes                                                                                                                                                                 |
| IsFullTextSearchable supported?       | Yes                                                                                                                                                                 |
| StoragePropertyName supported?        | No, use `JsonSerializerOptions` and `JsonPropertyNameAttribute` instead. [See here for more info.](#data-mapping)                                                   |

::: zone-end

## Limitations

Notable Azure AI Search connector functionality limitations.

| Feature Area                                                                        | Workaround                                                          |
| ----------------------------------------------------------------------------------- | ------------------------------------------------------------------- |
| Configuring full text search analyzers during collection creation is not supported. | Use the Azure AI Search Client SDK directly for collection creation |

::: zone pivot="programming-language-csharp"

## Getting started

Add the Azure AI Search Vector Store connector NuGet package to your project.

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Connectors.AzureAISearch --prerelease
```

You can add the vector store to the dependency injection container available on the `KernelBuilder` or to the `IServiceCollection` dependency injection container using extension methods provided by Semantic Kernel.

```csharp
using Azure;
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel
    .CreateBuilder()
    .AddAzureAISearchVectorStore(new Uri(azureAISearchUri), new AzureKeyCredential(secret));
```

```csharp
using Azure;
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAzureAISearchVectorStore(new Uri(azureAISearchUri), new AzureKeyCredential(secret));
```

Extension methods that take no parameters are also provided. These require an instance of the Azure AI Search `SearchIndexClient` to be separately registered with the dependency injection container.

```csharp
using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using Kernel Builder.
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton<SearchIndexClient>(
    sp => new SearchIndexClient(
        new Uri(azureAISearchUri),
        new AzureKeyCredential(secret)));
kernelBuilder.AddAzureAISearchVectorStore();
```

```csharp
using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// Using IServiceCollection with ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SearchIndexClient>(
    sp => new SearchIndexClient(
        new Uri(azureAISearchUri),
        new AzureKeyCredential(secret)));
builder.Services.AddAzureAISearchVectorStore();
```

You can construct an Azure AI Search Vector Store instance directly.

```csharp
using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;

var vectorStore = new AzureAISearchVectorStore(
    new SearchIndexClient(
        new Uri(azureAISearchUri),
        new AzureKeyCredential(secret)));
```

It is possible to construct a direct reference to a named collection.

```csharp
using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;

var collection = new AzureAISearchVectorStoreRecordCollection<string, Hotel>(
    new SearchIndexClient(new Uri(azureAISearchUri), new AzureKeyCredential(secret)),
    "skhotels");
```

::: zone-end
::: zone pivot="programming-language-python"

## Getting started

Install semantic kernel with the azure extras, which includes the Azure AI Search SDK.

```cli
pip install semantic-kernel[azure]
```

You can then create a vector store instance using the `AzureAISearchStore` class, this will use the environment variables `AZURE_AI_SEARCH_ENDPOINT` and `AZURE_AI_SEARCH_API_KEY` to connect to the Azure AI Search instance, those values can also be supplied directly. You can also supply azure credentials or token credentials instead of a API key.

```python

from semantic_kernel.connectors.memory.azure_ai_search import AzureAISearchStore

vector_store = AzureAISearchStore()
```

You can also create the vector store with your own instance of the azure search client.

```python
from azure.search.documents.indexes import SearchIndexClient
from semantic_kernel.connectors.memory.azure_ai_search import AzureAISearchStore

search_client = SearchIndexClient(endpoint="https://<your-search-service-name>.search.windows.net", credential="<your-search-service-key>")
vector_store = AzureAISearchStore(search_index_client=search_client)
```

You can also create a collection directly.

```python
from semantic_kernel.connectors.memory.azure_ai_search import AzureAISearchCollection

collection = AzureAISearchCollection(collection_name="skhotels", data_model_type=hotel)
```

## Serialization

Since the Azure AI Search connector needs a simple dict with the fields corresponding to the index as the input, the serialization is quite easy, just return a dict with the values with the keys corresponding to the index fields, the built-in step from dict to the the store model is a straight passthrough of the created dict.

For more details on this concept see the [serialization documentation](./../serialization.md).

::: zone-end
::: zone pivot="programming-language-java"

## Getting started

Include the latest version of the Semantic Kernel Azure AI Search data connector in your Maven project by adding the following dependency to your `pom.xml`:

```xml
<dependency>
    <groupId>com.microsoft.semantic-kernel</groupId>
    <artifactId>semantickernel-data-azureaisearch</artifactId>
    <version>[LATEST]</version>
</dependency>
```

You can then create a vector store instance using the `AzureAISearchVectorStore` class, having the AzureAISearch client as a parameter.

```java
import com.azure.core.credential.AzureKeyCredential;
import com.azure.search.documents.indexes.SearchIndexClientBuilder;
import com.microsoft.semantickernel.data.azureaisearch.AzureAISearchVectorStore;
import com.microsoft.semantickernel.data.azureaisearch.AzureAISearchVectorStoreOptions;
import com.microsoft.semantickernel.data.azureaisearch.AzureAISearchVectorStoreRecordCollection;
import com.microsoft.semantickernel.data.azureaisearch.AzureAISearchVectorStoreRecordCollectionOptions;

public class Main {
    public static void main(String[] args) {
        // Build the Azure AI Search client
        var searchClient = new SearchIndexClientBuilder()
                .endpoint("https://<your-search-service-name>.search.windows.net")
                .credential(new AzureKeyCredential("<your-search-service-key>"))
                .buildAsyncClient();

        // Build an Azure AI Search Vector Store
        var vectorStore = AzureAISearchVectorStore.builder()
                .withSearchIndexAsyncClient(searchClient)
                .withOptions(new AzureAISearchVectorStoreOptions())
                .build();
    }
}
```

You can also create a collection directly.

```java
var collection = new AzureAISearchVectorStoreRecordCollection<>(searchClient, "skhotels",
        AzureAISearchVectorStoreRecordCollectionOptions.<Hotel>builder()
                .withRecordClass(Hotel.class)
                .build());
```

::: zone-end

::: zone pivot="programming-language-csharp"

## Data mapping

The default mapper used by the Azure AI Search connector when mapping data from the data model to storage is the one provided by the Azure AI Search SDK.

This mapper does a direct conversion of the list of properties on the data model to the fields in Azure AI Search and uses `System.Text.Json.JsonSerializer`
to convert to the storage schema. This means that usage of the `JsonPropertyNameAttribute` is supported if a different storage name to the
data model property name is required.

It is also possible to use a custom `JsonSerializerOptions` instance with a customized property naming policy. To enable this, the `JsonSerializerOptions`
must be passed to both the `SearchIndexClient` and the `AzureAISearchVectorStoreRecordCollection` on construction.

```csharp
var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper };
var collection = new AzureAISearchVectorStoreRecordCollection<string, Hotel>(
    new SearchIndexClient(
        new Uri(azureAISearchUri),
        new AzureKeyCredential(secret),
        new() { Serializer = new JsonObjectSerializer(jsonSerializerOptions) }),
    "skhotels",
    new() { JsonSerializerOptions = jsonSerializerOptions });
```

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end
