---
title: Vector Store changes - May 2025
description: Describes the changes included in the May 2025 Vector Store release and how to migrate
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 03/06/2025
ms.service: semantic-kernel
---
::: zone pivot="programming-language-csharp"

# Vector Store changes - May 2025

## Nuget Package Renames

The following nuget packages have been renamed for clarity and length.

| Old Package Name | new Package Name |
|-|-|
| Microsoft.SemanticKernel.Connectors.AzureCosmosDBMongoDB | Microsoft.SemanticKernel.Connectors.CosmosMongoDB |
| Microsoft.SemanticKernel.Connectors.AzureCosmosDBNoSQL | Microsoft.SemanticKernel.Connectors.CosmosNoSql |
| Microsoft.SemanticKernel.Connectors.Postgres | Microsoft.SemanticKernel.Connectors.PgVector |
| Microsoft.SemanticKernel.Connectors.Sqlite | Microsoft.SemanticKernel.Connectors.SqliteVec |

## Type Renames

As part of our formal API review before GA various naming changes were proposed and adopted, resulting in the following name changes.
These should help improve clarity, consistency and reduce type name length.

| Old Namespace | Old TypeName | New Namespace | New TypeName |
|-|-|-|-|
| Microsoft.Extensions.VectorData | VectorStoreRecordDefinition | Microsoft.Extensions.VectorData | **VectorStoreCollectionDefinition** |
| Microsoft.Extensions.VectorData | VectorStoreRecordKeyAttribute | Microsoft.Extensions.VectorData | **VectorStoreKeyAttribute** |
| Microsoft.Extensions.VectorData | VectorStoreRecordDataAttribute | Microsoft.Extensions.VectorData | **VectorStoreDataAttribute** |
| Microsoft.Extensions.VectorData | VectorStoreRecordVectorAttribute | Microsoft.Extensions.VectorData | **VectorStoreVectorAttribute** |
| Microsoft.Extensions.VectorData | VectorStoreRecordProperty | Microsoft.Extensions.VectorData | **VectorStoreProperty** |
| Microsoft.Extensions.VectorData | VectorStoreRecordKeyProperty | Microsoft.Extensions.VectorData | **VectorStoreKeyProperty** |
| Microsoft.Extensions.VectorData | VectorStoreRecordDataProperty | Microsoft.Extensions.VectorData | **VectorStoreDataProperty** |
| Microsoft.Extensions.VectorData | VectorStoreRecordVectorProperty | Microsoft.Extensions.VectorData | **VectorStoreVectorProperty** |
| Microsoft.Extensions.VectorData | GetRecordOptions | Microsoft.Extensions.VectorData | **RecordRetrievalOptions** |
| Microsoft.Extensions.VectorData | GetFilteredRecordOptions&lt;TRecord&gt; | Microsoft.Extensions.VectorData | **FilteredRecordRetrievalOptions&lt;TRecord&gt;** |
| Microsoft.Extensions.VectorData | IVectorSearch&lt;TRecord&gt; | Microsoft.Extensions.VectorData | **IVectorSearchable&lt;TRecord&gt;** |
| Microsoft.Extensions.VectorData | IKeywordHybridSearch&lt;TRecord&gt; | Microsoft.Extensions.VectorData | **IKeywordHybridSearchable&lt;TRecord&gt;** |
| Microsoft.Extensions.VectorData | HybridSearchOptions&lt;TRecord&gt; | Microsoft.Extensions.VectorData | ??? |
| Microsoft.SemanticKernel.Connectors.AzureCosmosDBMongoDB | AzureCosmosDBMongoDBVectorStore | **Microsoft.SemanticKernel.Connectors.CosmosMongoDB** | **CosmosMongoVectorStore** |
| Microsoft.SemanticKernel.Connectors.AzureCosmosDBMongoDB | AzureCosmosDBMongoDBVectorStoreRecordCollection | **Microsoft.SemanticKernel.Connectors.CosmosMongoDB** | **CosmosMongoCollection** |
| Microsoft.SemanticKernel.Connectors.AzureCosmosDBNoSQL | AzureCosmosDBNoSQLVectorStore | **Microsoft.SemanticKernel.Connectors.CosmosNoSql** | **CosmosNoSqlVectorStore** |
| Microsoft.SemanticKernel.Connectors.AzureCosmosDBNoSQL | AzureCosmosDBNoSQLVectorStoreRecordCollection | **Microsoft.SemanticKernel.Connectors.CosmosNoSql** | **CosmosNoSqlCollection** |
| Microsoft.SemanticKernel.Connectors.MongoDB | MongoDBVectorStore | Microsoft.SemanticKernel.Connectors.MongoDB | **MongoVectorStore** |
| Microsoft.SemanticKernel.Connectors.MongoDB | MongoDBVectorStoreRecordCollection | Microsoft.SemanticKernel.Connectors.MongoDB | **MongoCollection** |

All names of the various Semantic Kernel supported implementations of `VectorStoreCollection` have been renamed to shorter names using a consistent pattern.

`*VectorStoreRecordCollection` is now `*Collection`. E.g. `PostgresVectorStoreRecordCollection` -> `PostgresCollection`.

Similary all related options classes have also changd.

`*VectorStoreRecordCollectionOptions` is now `*CollectionOptions`. E.g. `PostgresVectorStoreRecordCollectionOptions` -> `PostgresCollectionOptions`.

## Property Renames

| Namespace | Class | Old Property Name | New Property Name |
|-|-|-|-|
| Microsoft.Extensions.VectorData | VectorStoreKeyAttribute | StoragePropertyName | **StorageName** |
| Microsoft.Extensions.VectorData | VectorStoreDataAttribute | StoragePropertyName | **StorageName** |
| Microsoft.Extensions.VectorData | VectorStoreVectorAttribute | StoragePropertyName | **StorageName** |
| Microsoft.Extensions.VectorData | VectorStoreKeyProperty | DataModelPropertyName | **Name** |
| Microsoft.Extensions.VectorData | VectorStoreKeyProperty | StoragePropertyName | **StorageName** |
| Microsoft.Extensions.VectorData | VectorStoreKeyProperty | PropertyType | **Type** |
| Microsoft.Extensions.VectorData | VectorStoreDataProperty | DataModelPropertyName | **Name** |
| Microsoft.Extensions.VectorData | VectorStoreDataProperty | StoragePropertyName | **StorageName** |
| Microsoft.Extensions.VectorData | VectorStoreDataProperty | PropertyType | **Type** |
| Microsoft.Extensions.VectorData | VectorStoreVectorProperty | DataModelPropertyName | **Name** |
| Microsoft.Extensions.VectorData | VectorStoreVectorProperty | StoragePropertyName | **StorageName** |
| Microsoft.Extensions.VectorData | VectorStoreVectorProperty | PropertyType | **Type** |
| Microsoft.Extensions.VectorData | DistanceFunction | Hamming | **HammingDistance** |

The `VectorStoreRecordDefinition` property on collection options classes have been renamed to just `Definition`.

## Method Renames

The `CreateCollectionIfNotExistsAsync` method on the `Collection` has been renamed to `EnsureCollectionExistsAsync`.

The `DeleteAsync` method on the `*Collection` and `VectorStore` has been renamed to `EnsureCollectionDeletedAsync`.
This more closely aligns with the behavior of the method, which will delete a collection if it exists. If the collection does not exist it will do nothing and succeed.

## Interface to base abstract class

The following interfaces have been changed to base abstract classes.

| Namespace | Old Interface Name | New Type Name |
|-|-|-|
| Microsoft.Extensions.VectorData | IVectorStore | VectorStore |
| Microsoft.Extensions.VectorData | IVectorStoreRecordCollection | VectorStoreCollection |

Whereever you were using IVectorStore or IVectorStoreRecordCollection before, you can now use VectorStore and VectorStoreCollection instead.

## Merge of `SearchAsync` and `SearchEmbeddingAsync`

The `SearchAsync` and `SearchEmbeddingAsync` methods on the Collection have been merged into a single method: `SearchAsync`.

Previously `SearchAsync` allowed doing vector searches using source data that would be vectorized inside the collection or in the service while
`SearchEmbeddingAsync` allowed doing vector searches by providing a vector.

Now, both scenarios are supported using the single `SearchAsync` method, which can take as input both source data and vectors.

The mechanism for determining what to do is as follows:

1. If the provided value is one of the supported vector types for the connector, search uses that.
1. If the provided value is not one of supported vector types, the connector checks if an `IEmbeddingGenerator` is
   registered, with the vector store, that supports converting from the provided value to the vector type supported by the database.
1. Finally, if no compatible `IEmbeddingGenerator` is available, the method will throw an `InvalidOperationException`.

## Support for Dictionary<string, object?> models using `*DynamicCollection` and `VectorStore.GetDynamicCollection`

To allow support for NativeOAT and Trimming, where possible and when using the dynamic data model, the way in which
dynamic data models are supported has changed.
Specifically, how you request or construct the collection has changed.

Previously when using Dictionary<string, object?> as your data model, you could request this using `VectorStore.GetCollection`, but now
you will need to use `VectorStore.GetDynamicCollection`

```csharp
// Before
PostgresVectorStore vectorStore = new PostgresVectorStore(myNpgsqlDataSource)
vectorStore.GetCollection<string, Dictionary<string, object?>>("collectionName", definition);

// After
PostgresVectorStore vectorStore = new PostgresVectorStore(myNpgsqlDataSource, ownsDataSource: true)
vectorStore.GetDynamicCollection<string, Dictionary<string, object?>>("collectionName", definition);
```

## VectorStore and VectorStoreRecordCollection is now Disposable

Both `VectorStore` and `VectorStoreRecordCollection` is now disposable to ensure that database clients owned by these are disposed properly.

When passing a database client to your vector store or collection, you have the option to specify whether the vector store or collection should own
the client and therefore also dispose it when the vector store or collection is disposed.

For example, when passing a datasource to the `PostgresVectorStore` passing `true` for `ownsDataSource` will
cause the `PostgresVectorStore` to dispose the datasource when it is disposed.

```csharp
new PostgresVectorStore(dataSource, ownsDataSource: true, options);
```

## `CreateCollection` is not supported anymore, use `EnsureCollectionExistsAsync`

The `CreateCollection` method on the `Collection` has been removed, and only `EnsureCollectionExistsAsync` is now supported.
`EnsureCollectionExistsAsync` is idempotent and will create a collection if it does not exist, and do nothing if it already exists.

## `VectorStoreOperationException` and `VectorStoreRecordMappingException` is not supported anymore, use `VectorStoreException`

`VectorStoreOperationException` and `VectorStoreRecordMappingException` has been removed, and only `VectorStoreException` is now supported.
All database request failures resulting in a database specific exception are wrapped and thrown as a `VectorStoreException`, allowing
consuming code to catch a single exception type instead of a different one for each implementation.

## Dependency Injection helper changes



::: zone-end
::: zone pivot="programming-language-python"

## Not Applicable

These changes are currently only applicable in C#

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

These changes are currently only applicable in C#

::: zone-end
