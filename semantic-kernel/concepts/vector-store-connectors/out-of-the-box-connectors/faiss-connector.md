---
title: Using the Semantic Kernel Faiss Vector Store connector (Preview)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in an in-memory Faiss vector store.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 03/13/2025
ms.service: semantic-kernel
---
# Using the Faiss connector (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone pivot="programming-language-csharp"

## Not supported at this time

::: zone-end
::: zone pivot="programming-language-python"

## Overview

The Faiss Vector Store connector is a Vector Store implementation provided by Semantic Kernel that uses no external database and stores data in memory and vectors in a Faiss Index. It uses the [`InMemoryVectorCollection`](./inmemory-connector.md) for the other parts of the records, while using the Faiss indexes for search.
This Vector Store is useful for prototyping scenarios or where high-speed in-memory operations are required.

The connector has the following characteristics.

| Feature Area                          | Support                                                                                                                                             |
| ------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| Collection maps to                    | In-memory and Faiss indexes dictionary                                                                                                              |
| Supported key property types          | Any that is allowed to be a dict key, see the python documentation for details [here](https://docs.python.org/3/library/stdtypes.html#typesmapping) |
| Supported data property types         | Any type                                                                                                                                            |
| Supported vector property types       | <li>list[float \| int]</li><li>numpy array</li>                                                                                                     |
| Supported index types                 | Flat (see [custom indexes](#custom-indexes))                                                                                                        |
| Supported distance functions          | <li>Dot Product Similarity</li><li>Euclidean Squared Distance</li>                                                                                  |
| Supports multiple vectors in a record | Yes                                                                                                                                                 |
| is_filterable supported?              | Yes                                                                                                                                                 |
| is_full_text_searchable supported?    | Yes                                                                                                                                                 |

## Getting started

Add the Semantic Kernel package to your project.

```cmd
pip install semantic-kernel[faiss]
```

In the snippets below, it is assumed that you have a data model class defined named 'DataModel'.

```python
from semantic_kernel.connectors.memory.faiss import FaissStore

vector_store = FaissStore()
vector_collection = vector_store.get_collection("collection_name", DataModel)
```

It is possible to construct a direct reference to a named collection.

```python
from semantic_kernel.connectors.memory.faiss import FaissCollection

vector_collection = FaissCollection("collection_name", DataModel)
```

## Custom indexes

The Faiss connector is limited to the Flat index type.

Given the complexity of Faiss indexes, you are free to create your own index(es), including building the faiss-gpu package and using indexes from that. When doing this, any metrics defined on a vector field is ignored. If you have multiple vectors in your datamodel, you can pass in custom indexes only for the ones you want and let the built-in indexes be created, with a flat index and the metric defined in the model.

Important to note, if the index requires training, then make sure to do that as well, whenever we use the index, a check is done on the `is_trained` attribute of the index.

The index is always available (custom or built-in) in the `indexes` property of the collection. You can use this to get the index and do any operations you want on it, so you can do training afterwards, just make sure to do that before you want to use any CRUD against it.

To pass in your custom index, use either:

```python

import faiss

from semantic_kernel.connectors.memory.faiss import FaissCollection

index = faiss.IndexHNSW(d=768, M=16, efConstruction=200) # or some other index
vector_collection = FaissCollection(
    collection_name="collection_name", 
    data_model_type=DataModel, 
    indexes={"vector_field_name": index}
)
```

or:

```python

import faiss

from semantic_kernel.connectors.memory.faiss import FaissCollection

index = faiss.IndexHNSW(d=768, M=16, efConstruction=200) # or some other index
vector_collection = FaissCollection(
    collection_name="collection_name", 
    data_model_type=DataModel,
)
await vector_collection.create_collection(
    indexes={"vector_field_name": index}
)
# or when you have only one vector field:
await vector_collection.create_collection(
    index=index
)

```

::: zone-end
::: zone pivot="programming-language-java"

## Not supported at this time

::: zone-end
