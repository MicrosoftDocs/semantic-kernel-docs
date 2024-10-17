---
title: Defining your Semantic Kernel storage schema using a record definition (Preview)
description: Describes how to create a record definition with Semantic Kernel to use when writing to or reading from a Vector Store.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: reference
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Defining your storage schema using a record definition (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

## Overview

The Semantic Kernel Vector Store connectors use a model first approach to interacting with databases and allows annotating data
models with information that is needed for creating indexes or mapping data to the database schema.

Another way of providing this information is via record definitions, that can be defined and supplied separately to the data model.
This can be useful in multiple scenarios:

- There may be a case where a developer wants to use the same data model with more than one configuration.
- There may be a case where the developer wants to store data using a very different schema to the model and wants to supply a custom mapper for converting between the data model and storage schema.
- There may be a case where a developer wants to use a built-in type, like a dict, or a optimized format like a dataframe and still wants to leverage the vector store functionality.

::: zone pivot="programming-language-csharp"

Here is an example of how to create a record definition.

```csharp
using Microsoft.Extensions.VectorData;

var hotelDefinition = new VectorStoreRecordDefinition
{
    Properties = new List<VectorStoreRecordProperty>
    {
        new VectorStoreRecordKeyProperty("HotelId", typeof(ulong)),
        new VectorStoreRecordDataProperty("HotelName", typeof(string)) { IsFilterable = true },
        new VectorStoreRecordDataProperty("Description", typeof(string)) { IsFullTextSearchable = true },
        new VectorStoreRecordVectorProperty("DescriptionEmbedding", typeof(float)) { Dimensions = 4, DistanceFunction = DistanceFunction.CosineDistance, IndexKind = IndexKind.Hnsw },
    }
};
```

When creating a definition you always have to provide a name and type for each property in your schema, since this is required for index creation and data mapping.

To use the definition, pass it to the GetCollection method.

```csharp
var collection = vectorStore.GetCollection<ulong, Hotel>("skhotels", hotelDefinition);
```

## Record Property configuration classes

### VectorStoreRecordKeyProperty

Use this class to incidate that your property is the key of the record.

```csharp
new VectorStoreRecordKeyProperty("HotelId", typeof(ulong)),
```

#### VectorStoreRecordKeyProperty configuration settings

| Parameter                 | Required | Description                                                                                                                                                       |
|---------------------------|:--------:|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| DataModelPropertyName     | Yes      | The name of the property on the data model. Used by the built in mappers to automatically map between the storage schema and data model and for creating indexes. |
| PropertyType              | Yes      | The type of the property on the data model. Used by the built in mappers to automatically map between the storage schema and data model and for creating indexes. |
| StoragePropertyName       | No       | Can be used to supply an alternative name for the property in the database. Note that this parameter is not supported by all connectors, e.g. where alternatives like `JsonPropertyNameAttribute` is supported. |

> [!TIP]
> For more information on which connectors support StoragePropertyName and what alternatives are available, refer to [the documentation for each connector](./out-of-the-box-connectors/index.md).

### VectorStoreRecordDataProperty

Use this class to incidate that your property contains general data that is not a key or a vector.

```csharp
new VectorStoreRecordDataProperty("HotelName", typeof(string)) { IsFilterable = true },
```

#### VectorStoreRecordDataProperty configuration settings

| Parameter                 | Required | Description                                                                                                                                                       |
|---------------------------|:--------:|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| DataModelPropertyName     | Yes      | The name of the property on the data model. Used by the built in mappers to automatically map between the storage schema and data model and for creating indexes. |
| PropertyType              | Yes      | The type of the property on the data model. Used by the built in mappers to automatically map between the storage schema and data model and for creating indexes. |
| IsFilterable              | No       | Indicates whether the property should be indexed for filtering in cases where a database requires opting in to indexing per property. Default is false.           |
| IsFullTextSearchable      | No       | Indicates whether the property should be indexed for full text search for databases that support full text search. Default is false.                              |
| StoragePropertyName       | No       | Can be used to supply an alternative name for the property in the database. Note that this parameter is not supported by all connectors, e.g. where alternatives like `JsonPropertyNameAttribute` is supported. |

> [!TIP]
> For more information on which connectors support StoragePropertyName and what alternatives are available, refer to [the documentation for each connector](./out-of-the-box-connectors/index.md).

### VectorStoreRecordVectorProperty

Use this class to incidate that your property contains a vector.

```csharp
new VectorStoreRecordVectorProperty("DescriptionEmbedding", typeof(float)) { Dimensions = 4, DistanceFunction = DistanceFunction.CosineDistance, IndexKind = IndexKind.Hnsw },
```

#### VectorStoreRecordVectorProperty configuration settings

| Parameter                 | Required | Description                                                                                                                                                       |
|---------------------------|:--------:|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| DataModelPropertyName     | Yes      | The name of the property on the data model. Used by the built in mappers to automatically map between the storage schema and data model and for creating indexes. |
| PropertyType              | Yes      | The type of the property on the data model. Used by the built in mappers to automatically map between the storage schema and data model and for creating indexes. |
| Dimensions                | Yes for collection create, optional otherwise | The number of dimensions that the vector has. This is typically required when creating a vector index for a collection.      |
| IndexKind                 | No       | The type of index to index the vector with. Default varies by vector store type.                                                                                  |
| DistanceFunction          | No       | The type of distance function to use when doing vector comparison during vector search over this vector. Default varies by vector store type.                     |
| StoragePropertyName       | No       | Can be used to supply an alternative name for the property in the database. Note that this parameter is not supported by all connectors, e.g. where alternatives like `JsonPropertyNameAttribute` is supported. |

> [!TIP]
> For more information on which connectors support StoragePropertyName and what alternatives are available, refer to [the documentation for each connector](./out-of-the-box-connectors/index.md).

::: zone-end
::: zone pivot="programming-language-python"

Here is an example of how to create a record definition, for use with a [pandas DataFrame](https://pandas.pydata.org/docs/reference/frame.html).

> [!Note]
> The same fields as in the [data model definition](./defining-your-data-model.md) are used here, for a datamodel they are added as annotations, here as a dict with the name.

There are a couple of important things to note, other then the fields definitions themselves. The first is the `container_mode` parameter. When set to True, this indicates that the data model is a container type, like a DataFrame, and that the data model is therefore a container of records, instead of a single one, a container record can be used in the exact same way, the main difference is that `get` and `get_batch` will return the same data type, with a single record for a `get` and one or more for a `get_batch`. When you want to do a upsert, `upsert` and `upsert_batch` can be used interchangeably, in other words, passing a container to `upsert` will result in multiple upserts, instead of a single one.

The second is the addition of the `to_dict` and `from_dict` methods, which are used to convert between the data model and the storage schema. In this case, the `to_dict` method is used to convert the DataFrame to a list of records, and the `from_dict` method is used to convert a list of records to a DataFrame. There can also be a `serialize` and `deserialize` method (not shown in the example below), for details on the difference between those see the [serialization documentation](./serialization.md).

```python
from semantic_kernel.data import (
    VectorStoreRecordDataField,
    VectorStoreRecordDefinition,
    VectorStoreRecordKeyField,
    VectorStoreRecordVectorField,
)

hotel_definition = VectorStoreRecordDefinition(
    fields={
        "hotel_id": VectorStoreRecordKeyField(property_type="str"),
        "hotel_name": VectorStoreRecordDataField(property_type="str", is_filterable=True),
        "description": VectorStoreRecordDataField(
            property_type="str", has_embedding=True, embedding_property_name="description_embedding"
        ),
        "description_embedding": VectorStoreRecordVectorField(property_type="list[float]"),
    },
    container_mode=True,
    to_dict=lambda record, **_: record.to_dict(orient="records"),
    from_dict=lambda records, **_: DataFrame(records),
)
```

When creating a definition you always have to provide a name (as the key in the `fields` dict) and type for each property in your schema, since this is required for index creation and data mapping.

To use the definition, pass it to the GetCollection method or a collection constructor, together with the data model type.

```python
collection = vector_store.get_collection(
    collection_name="skhotels", 
    data_model_type=pd.DataFrame, 
    data_model_definition=hotel_definition,
)
```

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end
