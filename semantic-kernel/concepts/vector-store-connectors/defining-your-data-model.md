---
title: Defining your Semantic Kernel Vector Store data model (Preview)
description: Describes how to create a data model with Semantic Kernel to use when writing to or reading from a Vector Store.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: reference
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Defining your data model (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

## Overview

The Semantic Kernel Vector Store connectors use a model first approach to interacting with databases.

::: zone pivot="programming-language-csharp"
All methods to upsert or get records use strongly typed model classes.
The properties on these classes are decorated with attributes that indicate the purpose of each property.

> [!TIP]
> For an alternative to using attributes, refer to [defining your schema with a record definition](./schema-with-record-definition.md).
> [!TIP]
> For an alternative to defining your own data model, refer to [using Vector Store abstractions without defining your own data model](./generic-data-model.md).

Here is an example of a model that is decorated with these attributes.

```csharp
using Microsoft.Extensions.VectorData;

public class Hotel
{
    [VectorStoreRecordKey]
    public ulong HotelId { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public string HotelName { get; set; }

    [VectorStoreRecordData(IsFullTextSearchable = true)]
    public string Description { get; set; }

    [VectorStoreRecordVector(4, DistanceFunction.CosineSimilarity, IndexKind.Hnsw)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public string[] Tags { get; set; }
}
```

## Attributes

### VectorStoreRecordKeyAttribute

Use this attribute to indicate that your property is the key of the record.

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

Use this attribute to indicate that your property contains general data that is not a key or a vector.

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
[VectorStoreRecordVector(Dimensions: 4, DistanceFunction.CosineSimilarity, IndexKind.Hnsw)]
public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
```

#### VectorStoreRecordVectorAttribute parameters

| Parameter                 | Required | Description                                                                                                                                                                                                     |
|---------------------------|:--------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Dimensions                | Yes for collection create, optional otherwise | The number of dimensions that the vector has. This is typically required when creating a vector index for a collection.                                                    |
| IndexKind                 | No       | The type of index to index the vector with. Default varies by vector store type.                                                                                                                                |
| DistanceFunction          | No       | The type of function to use when doing vector comparison during vector search over this vector. Default varies by vector store type.                                                                   |
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
> For the alternative approach using a record definition, refer to [defining your schema with a record definition](./schema-with-record-definition.md).

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
> The annotations are parsed by the `vectorstoremodel` decorator and one of the things it does is to create a record definition for the class, it is therefore not necessary to instantiate a field class when no parameters are set, the field can be annotated with just the class, like this:
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

#### VectorStoreRecordDataField parameters

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

#### VectorStoreRecordVectorField parameters

| Parameter                 | Required | Description       |
|---------------------------|:--------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| dimensions                | Yes for collection create, optional otherwise | The number of dimensions that the vector has. This is typically required when creating a vector index for a collection.                                                    |
| index_kind                 | No       | The type of index to index the vector with. Default varies by vector store type.                                                                                                                                |
| distance_function          | No       | The type of function to use when doing vector comparison during vector search over this vector. Default varies by vector store type.                                                                   |
| local_embedding            | No       | Indicates whether the property has a local embedding associated with it, default is None.                                                         |
| embedding_settings        | No       | The settings for the embedding, in the form of a dict with service_id as key and PromptExecutionSettings as value, default is None.                                                                            |
| serialize_function        | No       | The function to use to serialize the vector, if the type is not a list[float \| int] this function is needed, or the whole model needs to be serialized.                                                                            |
| deserialize_function        | No       | The function to use to deserialize the vector, if the type is not a list[float \| int] this function is needed, or the whole model needs to be deserialized.                                                                            |


Common index kinds and distance function types are supplied as static values on the `semantic_kernel.data.IndexKind` and `semantic_kernel.data.DistanceFunction` classes.
Individual Vector Store implementations may also use their own index kinds and distance functions, where the database supports unusual types.


::: zone-end

::: zone pivot="programming-language-java"

All methods to upsert or get records use strongly typed model classes.
The fields on these classes are decorated with annotations that indicate the purpose of each field.

> [!TIP]
> For an alternative to using attributes, refer to [defining your schema with a record definition](./schema-with-record-definition.md).

Here is an example of a model that is decorated with these annotations. By default, most out of the box vector stores use Jackson, thus is a good practice to ensure the model object can be serialized by Jackson, i.e the class is visible, has getters, constructor, annotations, etc.

```java
import com.microsoft.semantickernel.data.vectorstorage.annotations.VectorStoreRecordData;
import com.microsoft.semantickernel.data.vectorstorage.annotations.VectorStoreRecordKey;
import com.microsoft.semantickernel.data.vectorstorage.annotations.VectorStoreRecordVector;
import com.microsoft.semantickernel.data.vectorstorage.definition.DistanceFunction;
import com.microsoft.semantickernel.data.vectorstorage.definition.IndexKind;

import java.util.List;

public class Hotel {
    @VectorStoreRecordKey
    private String hotelId;

    @VectorStoreRecordData(isFilterable = true)
    private String name;

    @VectorStoreRecordData(isFullTextSearchable = true)
    private String description;

    @VectorStoreRecordVector(dimensions = 4, indexKind = IndexKind.HNSW, distanceFunction = DistanceFunction.COSINE_DISTANCE)
    private List<Float> descriptionEmbedding;

    @VectorStoreRecordData(isFilterable = true)
    private List<String> tags;

    public Hotel() { }

    public Hotel(String hotelId, String name, String description, List<Float> descriptionEmbedding, List<String> tags) {
        this.hotelId = hotelId;
        this.name = name;
        this.description = description;
        this.descriptionEmbedding = descriptionEmbedding;
        this.tags = tags;
    }

    public String getHotelId() { return hotelId; }
    public String getName() { return name; }
    public String getDescription() { return description; }
    public List<Float> getDescriptionEmbedding() { return descriptionEmbedding; }
    public List<String> getTags() { return tags; }
}
```

## Annotations

### VectorStoreRecordKey

Use this annotation to indicate that your field is the key of the record.

```java
@VectorStoreRecordKey
private String hotelId;
```

#### VectorStoreRecordKey parameters

| Parameter                 | Required | Description                                                                                                                                                                                                     |
|---------------------------|:--------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| storageName       | No       | Can be used to supply an alternative name for the field in the database. Note that this parameter is not supported by all connectors, e.g. where Jackson is used, in that case the storage name can be specified using Jackson annotations. |

> [!TIP]
> For more information on which connectors support storageName and what alternatives are available, refer to [the documentation for each connector](./out-of-the-box-connectors/index.md).

### VectorStoreRecordData

Use this annotation to indicate that your field contains general data that is not a key or a vector.

```java
@VectorStoreRecordData(isFilterable = true)
private String name;
```

#### VectorStoreRecordData parameters

| Parameter                 | Required | Description                                                                                                                                                                                                     |
|---------------------------|:--------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| isFilterable              | No       | Indicates whether the field should be indexed for filtering in cases where a database requires opting in to indexing per field. Default is false.                                                         |
| isFullTextSearchable      | No       | Indicates whether the field should be indexed for full text search for databases that support full text search. Default is false.                                                                            |
| storageName       | No       | Can be used to supply an alternative name for the field in the database. Note that this parameter is not supported by all connectors, e.g. where Jackson is used, in that case the storage name can be specified using Jackson annotations. |

> [!TIP]
> For more information on which connectors support storageName and what alternatives are available, refer to [the documentation for each connector](./out-of-the-box-connectors/index.md).

### VectorStoreRecordVector

Use this annotation to indicate that your field contains a vector.

```java
@VectorStoreRecordVector(dimensions = 4, indexKind = IndexKind.HNSW, distanceFunction = DistanceFunction.COSINE_DISTANCE)
private List<Float> descriptionEmbedding;
```

#### VectorStoreRecordVector parameters

| Parameter                 | Required | Description                                                                                                                                                                                                     |
|---------------------------|:--------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| dimensions                | Yes for collection create, optional otherwise | The number of dimensions that the vector has. This is typically required when creating a vector index for a collection.                                                    |
| indexKind                 | No       | The type of index to index the vector with. Default varies by vector store type.                                                                                                                                |
| distanceFunction          | No       | The type of function to use when doing vector comparison during vector search over this vector. Default varies by vector store type.                                                                   |
| storageName       | No       | Can be used to supply an alternative name for the field in the database. Note that this parameter is not supported by all connectors, e.g. where Jackson is used, in that case the storage name can be specified using Jackson annotations. |

Common index kinds and distance function types are supplied on the `com.microsoft.semantickernel.data.vectorstorage.definition.IndexKind` and `com.microsoft.semantickernel.data.vectorstorage.definition.DistanceFunction` enums.
Individual Vector Store implementations may also use their own index kinds and distance functions, where the database supports unusual types.

> [!TIP]
> For more information on which connectors support storageName and what alternatives are available, refer to [the documentation for each connector](./out-of-the-box-connectors/index.md).


More info coming soon.

::: zone-end
