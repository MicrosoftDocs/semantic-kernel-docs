---
title: Defining your Semantic Kernel storage schema using a record definition (Experimental)
description: Describes how to create a record definition with Semantic Kernel to use when writing to or reading from a Vector Store.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: reference
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# Defining your storage schema using a record definition (Experimental)

## Overview

The Semantic Kernel Vector Store connectors use a model first approach to interacting with databases and allows annotating data
models with information that is needed for creating indexes or mapping data to the database schema.

Another way of providing this information is via record definitions, that can be defined and supplied separately to the data model.
This can be useful in multiple scenarios:

- There may be a case where a developer wants to use the same data model with more than one configuration.
- There may be a case where the developer wants to store data using a very different schema to the model and wants to supply a custom mapper for converting between the data model and storage schema.

::: zone pivot="programming-language-csharp"

Here is an example of how to create a record definition.

```csharp
using Microsoft.SemanticKernel;

var hotelDefinition = new VectorStoreRecordDefinition
{
    Properties = new List<VectorStoreRecordProperty>
    {
        new VectorStoreRecordKeyProperty("HotelId", typeof(ulong)),
        new VectorStoreRecordDataProperty("HotelName", typeof(string)) { IsFilterable = true },
        new VectorStoreRecordDataProperty("Description", typeof(string)) { IsFullTextSearchable = true },
        new VectorStoreRecordVectorProperty("DescriptionEmbedding", typeof(float)) { Dimensions = 4, IndexKind = IndexKind.Hnsw, DistanceFunction = DistanceFunction.CosineDistance },
    }
};
```

When creating a definition you always have to provide a name and type for each property in your schema, since this is required for index creation and data mapping.

To use the definition, pass it to the GetCollection method.

```csharp
var collection = vectorStore.GetCollection<ulong, Glossary>("skhotels", hotelDefinition);
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
| StoragePropertyName       | No       | Can be used to supply an alternative name for the property in the database. Note that this parameter is not supported by all connectors.                          |

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
| StoragePropertyName       | No       | Can be used to supply an alternative name for the property in the database. Note that this parameter is not supported by all connectors.                          |
| IsFilterable              | No       | Indicates whether the property should be indexed for filtering in cases where a database requires opting in to indexing per property. Default is false.           |
| IsFullTextSearchable      | No       | Indicates whether the property should be indexed for full text search for databases that support full text search. Default is false.                              |

### VectorStoreRecordVectorProperty

Use this class to incidate that your property contains a vector.

```csharp
new VectorStoreRecordVectorProperty("DescriptionEmbedding", typeof(float)) { Dimensions = 4, IndexKind = IndexKind.Hnsw, DistanceFunction = DistanceFunction.CosineDistance },
```

#### VectorStoreRecordVectorProperty configuration settings

| Parameter                 | Required | Description                                                                                                                                                       |
|---------------------------|:--------:|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| DataModelPropertyName     | Yes      | The name of the property on the data model. Used by the built in mappers to automatically map between the storage schema and data model and for creating indexes. |
| PropertyType              | Yes      | The type of the property on the data model. Used by the built in mappers to automatically map between the storage schema and data model and for creating indexes. |
| StoragePropertyName       | No       | Can be used to supply an alternative name for the property in the database. Note that this parameter is not supported by all connectors.                          |
| Dimensions                | Yes for collection create, optional otherwise | The number of dimensions that the vector has. This is typically required when creating a vector index for a collection.      |
| IndexKind                 | No       | The type of index to index the vector with. Default varies by vector store type.                                                                                  |
| DistanceFunction          | No       | The type of distance function to use when doing vector comparison during vector search over this vector. Default varies by vector store type.                     |

::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end
