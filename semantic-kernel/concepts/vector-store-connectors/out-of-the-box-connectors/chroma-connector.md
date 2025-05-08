---
title: Using the Semantic Kernel Chroma Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in ChromaDB.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 02/26/2025
ms.service: semantic-kernel
---

# Using the Chroma connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone pivot="programming-language-csharp"

## Not supported

Not supported.

::: zone-end
::: zone pivot="programming-language-python"

## Overview

The Chroma Vector Store connector can be used to access and manage data in Chroma. The connector has the
following characteristics.

| Feature Area                          | Support                                                                                           |
| ------------------------------------- | ------------------------------------------------------------------------------------------------- |
| Collection maps to                    | Chroma collection                                                                                 |
| Supported key property types          | string                                                                                            |
| Supported data property types         | All types that are supported by System.Text.Json (either built-in or by using a custom converter) |
| Supported vector property types       | <ul><li>list[float]</li><li>list[int]</li><li>ndarray</li></ul>                                   |
| Supported index types                 | <ul><li>HNSW</li></ul>                                                                            |
| Supported distance functions          | <ul><li>CosineSimilarity</li><li>DotProductSimilarity</li><li>EuclideanSquaredDistance</li></ul>  |
| Supported filter clauses              | <ul><li>AnyTagEqualTo</li><li>EqualTo</li></ul>                                                   |
| Supports multiple vectors in a record | No                                                                                                |
| IsFilterable supported?               | Yes                                                                                               |
| IsFullTextSearchable supported?       | Yes                                                                                               |

## Limitations

Notable Chroma connector functionality limitations.

| Feature Area       | Workaround                                                                                                                |
| ------------------ | ------------------------------------------------------------------------------------------------------------------------- |
| Client-server mode | Use the client.HttpClient and pass the result to the `client` parameter, we do not support a AsyncHttpClient at this time |
| Chroma Cloud       | Unclear at this time, as Chroma Cloud is still in private preview                                                         |

## Getting Started

Add the Chroma Vector Store connector dependencies to your project.

```bash
pip install semantic-kernel[chroma]
```

You can then create the vector store.

```python
from semantic_kernel.connectors.memory.chroma import ChromaStore

store = ChromaStore()
```

Alternatively, you can also pass in your own mongodb client if you want to have more control over the client construction:

```python
from chromadb import Client
from semantic_kernel.connectors.memory.chroma import ChromaStore

client = Client(...)
store = ChromaStore(client=client)
```

You can also create a collection directly, without the store.

```python
from semantic_kernel.connectors.memory.chroma import ChromaCollection

# `hotel` is a class created with the @vectorstoremodel decorator
collection = ChromaCollection(
    collection_name="my_collection",
    data_model_type=hotel
)
```

## Serialization

The Chroma client returns both `get` and `search` results in tabular form, this means that there are between 3 and 5 lists being returned in a dict, the lists are 'keys', 'documents', 'embeddings', and optionally 'metadatas' and 'distances'. The Semantic Kernel Chroma connector will automatically convert this into a list of `dict` objects, which are then parsed back to your data model.

It could be very interesting performance wise to do straight serialization from this format into a dataframe-like structure as that saves a lot of rebuilding of the data structure. This is not done for you, even when using container mode, you would have to specify this yourself, for more details on this concept see the [serialization documentation](./../serialization.md).

::: zone-end
::: zone pivot="programming-language-java"

## Not supported

Not supported.

::: zone-end
