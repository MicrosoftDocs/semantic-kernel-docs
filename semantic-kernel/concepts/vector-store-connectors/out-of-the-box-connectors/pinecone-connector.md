---
title: Using the Semantic Kernel Pinecone Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in Pinecone.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: article
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Using the Pinecone connector (Preview)

::: zone pivot="programming-language-python"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end
::: zone pivot="programming-language-java"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end

::: zone pivot="programming-language-csharp"

The .NET SDK for Pinecone has been archived and is no longer supported. The Pinecone connector is not available for C#.

::: zone-end
::: zone pivot="programming-language-python"

## Overview

The Pinecone Vector Store connector can be used to access and manage data in Pinecone. The connector has the following characteristics.

| Feature Area                          | Support                                                                                                                                                                     |
| ------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Collection maps to                    | Pinecone serverless Index                                                                                                                                                   |
| Supported key property types          | string                                                                                                                                                                      |
| Supported data property types         | <ul><li>string</li><li>int</li><li>long</li><li>double</li><li>float</li><li>decimal</li><li>bool</li><li>DateTime</li><li>*and iterables of each of these types*</li></ul> |
| Supported vector property types       | <ul><li>list[float]</li><li>list[int]</li><li>numpy array</li></ul>                                                                                                         |
| Supported index types                 | PGA (Pinecone Graph Algorithm)                                                                                                                                              |
| Supported distance functions          | <ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanSquaredDistance</li></ul>                                                                            |
| Supported filter clauses              | <ul><li>EqualTo</li></ul><ul><li>AnyTagEqualTo</li></ul>                                                                                                                    |
| Supports multiple vectors in a record | No                                                                                                                                                                          |
| IsFilterable supported?               | Yes                                                                                                                                                                         |
| IsFullTextSearchable supported?       | No                                                                                                                                                                          |
| Integrated Embeddings supported?      | Yes, see [here](#integrated-embeddings)                                                                                                                                     |
| GRPC Supported?                       | Yes, see [here](#grpc-support)                                                                                                                                              |

## Getting started

Add the Pinecone Vector Store connector extra  to your project.

```bash
pip install semantic-kernel[pinecone]
```

You can then create a PineconeStore instance and use it to create a collection.
This will read the Pinecone API key from the environment variable `PINECONE_API_KEY`.

```python
from semantic_kernel.connectors.pinecone import PineconeStore

store = PineconeStore()
collection = store.get_collection(collection_name="collection_name", record_type=DataModel)
```

It is possible to construct a direct reference to a named collection.

```python
from semantic_kernel.connectors.pinecone import PineconeCollection

collection = PineconeCollection(collection_name="collection_name", record_type=DataModel)
```

You can also create your own Pinecone client and pass it into the constructor.
The client needs to be either `PineconeAsyncio` or `PineconeGRPC` (see [GRPC Support](#grpc-support)).

```python
from semantic_kernel.connectors.pinecone import PineconeStore, PineconeCollection
from pinecone import PineconeAsyncio

client = PineconeAsyncio(api_key="your_api_key") 
store = PineconeStore(client=client)
collection = store.get_collection(collection_name="collection_name", record_type=DataModel)
```

### GRPC support

We also support two options on the collection constructor, the first is to enable GRPC support:

```python
from semantic_kernel.connectors.pinecone import PineconeCollection

collection = PineconeCollection(collection_name="collection_name", record_type=DataModel, use_grpc=True)
```

Or with your own client:

```python
from semantic_kernel.connectors.pinecone import PineconeStore
from pinecone.grpc import PineconeGRPC

client = PineconeGRPC(api_key="your_api_key")
store = PineconeStore(client=client)
collection = store.get_collection(collection_name="collection_name", record_type=DataModel)
```

### Integrated Embeddings

The second is to use the integrated embeddings of Pinecone, this will check for a environment variable called `PINECONE_EMBED_MODEL` with the model name, or you can pass in a `embed_settings` dict, which can contain just the model key, or the full settings for the embedding model. In the former case, the other settings will be derived from the data model definition.

See [Pinecone docs](https://docs.pinecone.io/guides/indexes/create-an-index) and then the `Use integrated embeddings` sections.

```python
from semantic_kernel.connectors.pinecone import PineconeCollection

collection = PineconeCollection(collection_name="collection_name", record_type=DataModel)
```

Alternatively, when not settings the environment variable, you can pass the embed settings into the constructor:

```python
from semantic_kernel.connectors.pinecone import PineconeCollection

collection = PineconeCollection(collection_name="collection_name", record_type=DataModel, embed_settings={"model": "multilingual-e5-large"})
```

This can include other details about the vector setup, like metric and field mapping.
You can also pass the embed settings into the `ensure_collection_exists` method, this will override the default settings set during initialization.

```python
from semantic_kernel.connectors.pinecone import PineconeCollection

collection = PineconeCollection(collection_name="collection_name", record_type=DataModel)
await collection.ensure_collection_exists(embed_settings={"model": "multilingual-e5-large"})
```

> Important: GRPC and Integrated embeddings cannot be used together.

## Index Namespace

The Vector Store abstraction does not support a multi tiered record grouping mechanism. Collections in the abstraction map to a Pinecone serverless index
and no second level exists in the abstraction. Pinecone does support a second level of grouping called namespaces.

By default the Pinecone connector will pass `''` as the namespace for all operations. However it is possible to pass a single namespace to the
Pinecone collection when constructing it and use this instead for all operations.

```python
from semantic_kernel.connectors.pinecone import PineconeCollection

collection = PineconeCollection(
    collection_name="collection_name", 
    record_type=DataModel, 
    namespace="seasidehotels"
)
```

::: zone-end
::: zone pivot="programming-language-java"

The Pinecone connector is not yet available in Java.

::: zone-end
