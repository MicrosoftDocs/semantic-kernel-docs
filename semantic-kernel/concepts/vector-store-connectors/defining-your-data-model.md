---
title: Defining your Semantic Kernel Vector Store data model (Experimental)
description: Describes how to create a data model with Semantic Kernel to use when writing to or reading from a Vector Store.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: reference
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Defining your data model (Experimental)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is experimental, still in development and is subject to change.

## Overview

The Semantic Kernel Vector Store connectors use a model first approach to interacting with databases.

::: zone pivot="programming-language-csharp"
All methods to upsert or get records use strongly typed model classes.
The properties on these classes are decorated with attributes that indicate the purpose of each property.

> [!TIP]
> For an alternative to using attributes, refer to [definining your schema with a record definition](./schema-with-record-definition.md).

Here is an example of a model that is decorated with these attributes.

```csharp
using Microsoft.SemanticKernel.Data;

#pragma warning disable SKEXP0001

public class Hotel
{
    [VectorStoreRecordKey]
    public ulong HotelId { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public string HotelName { get; set; }

    [VectorStoreRecordData(IsFullTextSearchable = true)]
    public string Description { get; set; }

    [VectorStoreRecordVector(4, IndexKind.Hnsw, DistanceFunction.CosineDistance)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public string[] Tags { get; set; }
}
```

## Attributes

### VectorStoreRecordKeyAttribute

Use this attribute to incidate that your property is the key of the record.

```csharp
[VectorStoreRecordKey]
public ulong HotelId { get; set; }
```

#### VectorStoreRecordKeyAttribute parameters

| Parameter                 | Required | Description                                                                                                                                                                                                     |
|---------------------------|:--------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| StoragePropertyName       | No       | Can be used to supply an alternative name for the property in the database. Note that this parameter is not supported by all connectors, e.g. where alternatives like `JsonPropertyNameAttribute` is supported. |

> [!TIP]
> For more information on which connectors support StoragePropertyName and what alternatives are available, refer to [the documentation for each connector](./out-of-the-box-connectors/index.md).

### VectorStoreRecordDataAttribute

Use this attribute to incidate that your property contains general data that is not a key or a vector.

```csharp
[VectorStoreRecordData(IsFilterable = true)]
public string HotelName { get; set; }
```

#### VectorStoreRecordDataAttribute parameters

| Parameter                 | Required | Description                                                                                                                                                                                                     |
|---------------------------|:--------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| IsFilterable              | No       | Indicates whether the property should be indexed for filtering in cases where a database requires opting in to indexing per property. Default is false.                                                         |
| IsFullTextSearchable      | No       | Indicates whether the property should be indexed for full text search for databases that support full text search. Default is false.                                                                            |
| StoragePropertyName       | No       | Can be used to supply an alternative name for the property in the database. Note that this parameter is not supported by all connectors, e.g. where alternatives like `JsonPropertyNameAttribute` is supported. |

> [!TIP]
> For more information on which connectors support StoragePropertyName and what alternatives are available, refer to [the documentation for each connector](./out-of-the-box-connectors/index.md).

### VectorStoreRecordVectorAttribute

Use this attribute to indicate that your property contains a vector.

```csharp
[VectorStoreRecordVector(Dimensions: 4, IndexKind.Hnsw, DistanceFunction.CosineDistance)]
public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
```

#### VectorStoreRecordVectorAttribute parameters

| Parameter                 | Required | Description                                                                                                                                                                                                     |
|---------------------------|:--------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Dimensions                | Yes for collection create, optional otherwise | The number of dimensions that the vector has. This is typically required when creating a vector index for a collection.                                                    |
| IndexKind                 | No       | The type of index to index the vector with. Default varies by vector store type.                                                                                                                                |
| DistanceFunction          | No       | The type of distance function to use when doing vector comparison during vector search over this vector. Default varies by vector store type.                                                                   |
| StoragePropertyName       | No       | Can be used to supply an alternative name for the property in the database. Note that this parameter is not supported by all connectors, e.g. where alternatives like `JsonPropertyNameAttribute` is supported. |

Common index kinds and distance function types are supplied as static values on the `Microsoft.SemanticKernel.Data.IndexKind` and `Microsoft.SemanticKernel.Data.DistanceFunction` classes.
Individual Vector Store implementations may also use their own index kinds and distance functions, where the database supports unusual types.

> [!TIP]
> For more information on which connectors support StoragePropertyName and what alternatives are available, refer to [the documentation for each connector](./out-of-the-box-connectors/index.md).

::: zone-end

::: zone pivot="programming-language-python"

All methods to upsert or get records use a class and a vector store record definition.

This can be done by defining your own class with annotations for the fields, or by using a class/type in combination with a record definition. Two things need to be done for a class, the first is to add the annotations with the field types, the second is to decorate the class with the `vectorstoremodel` decorator.

> [!TIP]
> For the alternative approach using a record definition, refer to [definining your schema with a record definition](./schema-with-record-definition.md).

Here is an example of a model that is decorated with these annotations.

```python
from dataclasses import dataclass, field
from typing import Annotated
from semantic_kernel.data import (
    DistanceFunction,
    IndexKind,
    VectorStoreRecordDataField,
    VectorStoreRecordDefinition,
    VectorStoreRecordKeyField,
    VectorStoreRecordVectorField,
    vectorstoremodel,
)

@vectorstoremodel
@dataclass
class Hotel:
    hotel_id: Annotated[str, VectorStoreRecordKeyField()] = field(default_factory=lambda: str(uuid4()))
    hotel_name: Annotated[str, VectorStoreRecordDataField(is_filterable=True)]
    description: Annotated[str, VectorStoreRecordDataField(is_full_text_searchable=True)]
    description_embedding: Annotated[list[float], VectorStoreRecordVectorField(dimensions=4, distance_function=DistanceFunction.COSINE, index_kind=IndexKind.HNSW)]
    tags: Annotated[list[str], VectorStoreRecordDataField(is_filterable=True)]
```

> [!TIP]
> Defining a class with these annotations can be done in multiple ways, one of which is using the `dataclasses` module in Python, shown here. This [sample](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/memory/data_models.py) shows other approaches (using Pydantic BaseModels and vanilla python classes) as well.

## Annotations

There are three types of annotations to be used, and they have a common base class.

### VectorStoreRecordField

This is the base class for all annotations, it is not meant to be used directly.

### VectorStoreRecordField parameters

| Parameter                 | Required | Description                                                                                                                                                                                                     |
|---------------------------|:--------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| name                      | No       | Can be added directly but will be set during parsing of the model. |
| property_type       | No       | Should be a string, will also be derived during parsing. |

> [!TIP]
> The annotations are parsed by the `vectorstoremodel` decorator and one of the things it does is to create a record definition for the class, for that it is not even necessary to instantiate a field class, so when no parameters need to be set manually, the field can be created with just the name of the class:
> ```python
> hotel_id: Annotated[str, VectorStoreRecordKeyField]
> ```

### VectorStoreRecordKeyField

Use this annotation to indicate that this attribute is the key of the record.

```python
VectorStoreRecordKeyField()
```

#### VectorStoreRecordKeyField parameters
No other parameters outside of the base class are defined.


### VectorStoreRecordDataField

Use this annotation to indicate that your attribute contains general data that is not a key or a vector.

```python
VectorStoreRecordDataField(is_filterable=True)
```

#### VectorStoreRecordDataAttribute parameters

| Parameter                 | Required | Description                                                                                                                                                                                                     |
|---------------------------|:--------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| has_embedding             | No       | Indicates whether the property has a embedding associated with it, default is None.                                                         |
| embedding_property_name   | No       | The name of the property that contains the embedding, default is None.                                                                            |
| is_filterable              | No       | Indicates whether the property should be indexed for filtering in cases where a database requires opting in to indexing per property. Default is false.                                                         |
| is_full_text_searchable      | No       | Indicates whether the property should be indexed for full text search for databases that support full text search. Default is false.                                                                            |


### VectorStoreRecordVectorField

Use this annotation to indicate that your attribute contains a vector.

```python
VectorStoreRecordVectorField(dimensions=4, distance_function=DistanceFunction.COSINE, index_kind=IndexKind.HNSW)
```

#### VectorStoreRecordVectorAttribute parameters

| Parameter                 | Required | Description                                                                                                                                                                                                     |
|---------------------------|:--------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| dimensions                | Yes for collection create, optional otherwise | The number of dimensions that the vector has. This is typically required when creating a vector index for a collection.                                                    |
| index_kind                 | No       | The type of index to index the vector with. Default varies by vector store type.                                                                                                                                |
| distance_function          | No       | The type of distance function to use when doing vector comparison during vector search over this vector. Default varies by vector store type.                                                                   |
| local_embedding            | No       | Indicates whether the property has a local embedding associated with it, default is None.                                                         |
| embedding_settings        | No       | The settings for the embedding, in the form of a dict with service_id as key and PromptExecutionSettings as value, default is None.                                                                            |
| serialize_function        | No       | The function to use to serialize the vector, if the type is not a list[float | int] this function is needed, or the whole model needs to be serialized.                                                                            |
| deserialize_function        | No       | The function to use to deserialize the vector, if the type is not a list[float | int] this function is needed, or the whole model needs to be deserialized.                                                                            |


Common index kinds and distance function types are supplied as static values on the `semantic_kernel.data.IndexKind` and `semantic_kernel.data.DistanceFunction` classes.
Individual Vector Store implementations may also use their own index kinds and distance functions, where the database supports unusual types.


::: zone-end

::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end
