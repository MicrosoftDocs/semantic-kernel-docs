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

## Overview

The Semantic Kernel Vector Store connectors use a model first approach to interacting with databases.

::: zone pivot="programming-language-csharp"
All methods to upsert or get records use strongly typed model classes.
The properties on these classes are decorated with attributes that indicate the pupose of each property.

> [!TIP]
> For an alternative to using attributes, refer to [definining your schema with a record definition](./schema-with-record-definition.md).

## Attributes

### VectorStoreRecordKeyAttribute

Use this attribute to incidate that your property is the key of the record.

```csharp
[VectorStoreRecordKey]
public ulong HotelId { get; set; }
```

#### VectorStoreRecordKeyAttribute parameters

| Parameter                 | Required | Description                                                                                                                              |
|---------------------------|:--------:|------------------------------------------------------------------------------------------------------------------------------------------|
| StoragePropertyName       | No       | Can be used to supply an alternative name for the property in the database. Note that this parameter is not supported by all connectors. |

### VectorStoreRecordDataAttribute

Use this attribute to incidate that your property contains general data that is not a key or a vector.

```csharp
[VectorStoreRecordData(IsFilterable = true)]
public string HotelName { get; set; }
```

#### VectorStoreRecordDataAttribute parameters

| Parameter                 | Required | Description                                                                                                                              |
|---------------------------|:--------:|------------------------------------------------------------------------------------------------------------------------------------------|
| StoragePropertyName       | No       | Can be used to supply an alternative name for the property in the database. Note that this parameter is not supported by all connectors. |
| IsFilterable              | No       | Indicates whether the property should be indexed for filtering in cases where a database requires opting in to indexing per property. Default is false. |
| IsFullTextSearchable      | No       | Indicates whether the property should be indexed for full text search for databases that support full text search. Default is false.     |

### VectorStoreRecordVectorAttribute

Use this attribute to incidate that your property contains a vector.

```csharp
[VectorStoreRecordVector(4, IndexKind.Hnsw, DistanceFunction.CosineDistance)]
public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
```

#### VectorStoreRecordVectorAttribute parameters

| Parameter                 | Required | Description                                                                                                                              |
|---------------------------|:--------:|------------------------------------------------------------------------------------------------------------------------------------------|
| StoragePropertyName       | No       | Can be used to supply an alternative name for the property in the database. Note that this parameter is not supported by all connectors. |
| Dimensions                | Yes for collection create, optional otherwise | The number of dimensions that the vector has. This is typically required when creating a vector index for a collection.                  |
| IndexKind                 | No       | The type of index to index the vector with. Default varies by vector store type.                                                         |
| DistanceFunction          | No       | The type of distance function to use when doing vector comparison during vector search over this vector. Default varies by vector store type. |

Common index kinds and distance function types are supplied as static values on the `Microsoft.SemanticKernel.Data.IndexKind` and `Microsoft.SemanticKernel.Data.DistanceFunction` classes.
Individual Vector Store implementations may also use their own index kinds and distance functions, where the database supports unusual types.

::: zone-end

::: zone pivot="programming-language-python"
::: zone-end
