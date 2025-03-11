---
title: Vector Store changes - March 2025
description: Describes the changes included in the March 2025 Vector Store release and how to migrate
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 03/06/2025
ms.service: semantic-kernel
---
::: zone pivot="programming-language-csharp"

# Vector Store changes - March 2025

## LINQ based filtering

When doing vector searches it is possible to create a filter (in addition to the vector similarity)
that act on data properties to constrain the list of records matched.

This filter is changing to support more filtering options. Previously the filter would
have been expressed using a custom `VectorSearchFilter` type, but with this update the filter
would be expressed using LINQ expressions.

The old filter clause is still preserved in a property called OldFilter, and will be removed in future.

```csharp
// Before
var searchResult = await collection.VectorizedSearchAsync(
    searchVector,
    new() { Filter = new VectorSearchFilter().EqualTo(nameof(Glossary.Category), "External Definitions") });

// After
var searchResult = await collection.VectorizedSearchAsync(
    searchVector,
    new() { Filter = g => g.Category == "External Definitions" });

// The old filter option is still available
var searchResult = await collection.VectorizedSearchAsync(
    searchVector,
    new() { OldFilter = new VectorSearchFilter().EqualTo(nameof(Glossary.Category), "External Definitions") });
```

## Target Property Selection for Search

When doing a vector search, it is possible to choose the vector property that the search should
be executed against.
Previously this was done via an option on the `VectorSearchOptions` class called `VectorPropertyName`.
`VectorPropertyName` was a string that could contain the name of the target property.

`VectorPropertyName` is being obsoleted in favour of a new property called `VectorProperty`.
`VectorProperty` is an expression that references the required property directly.

```csharp
// Before
var options = new VectorSearchOptions() { VectorPropertyName = "DescriptionEmbedding" };

// After
var options = new VectorSearchOptions<MyRecord>() { VectorProperty = r => r.DescriptionEmbedding };
```

Specifying `VectorProperty` will remain optional just like `VectorPropertyName` was optional.
The behavior when not specifying the property name is changing.
Previously if not specifying a target property, and more than one vector property existed on the
data model, the search would target the first available vector property in the schema.

Since the property which is 'first' can change in many circumstances unrelated to the search code, using this
strategy is risky. We are therefore changing this behavior, so that if there are more than
one vector property, one must be chosen.

## `VectorSearchOptions` change to generic type

The `VectorSearchOptions` class is changing to `VectorSearchOptions<TRecord>`, to accomodate the
LINQ based filtering and new property selectors metioned above.

If you are currently constructing the options class without providing the name of the options class
there will be no change.  E.g. `VectorizedSearchAsync(embedding, new() { Top = 5 })`.

On the other hand if you are using `new` with the type name, you will need to add the record type as a
generic parameter.

```csharp
// Before
var options = new VectorSearchOptions() { Top = 5 };

// After
var options = new VectorSearchOptions<MyRecord>() { Top = 5 };
```

## Removal of collection factories in favour of inheritance/decorator pattern

Each VectorStore implementation allows you to pass a custom factory to use for
constructing collections. This pattern is being removed and the recommended approach
is now to inherit from the VectorStore where you want custom construction and override
the GetCollection method.

```csharp
// Before
var vectorStore = new QdrantVectorStore(
    new QdrantClient("localhost"),
    new()
    {
        VectorStoreCollectionFactory = new CustomQdrantCollectionFactory(productDefinition)
    });

// After
public class QdrantCustomCollectionVectorStore(QdrantClient qdrantClient) : QdrantVectorStore(qdrantClient)
{
    public override IVectorStoreRecordCollection<TKey, TRecord> GetCollection<TKey, TRecord>(string name, VectorStoreRecordDefinition? vectorStoreRecordDefinition = null)
    {
        // custom construction logic...
    }
}

var vectorStore = new QdrantCustomCollectionVectorStore(new QdrantClient("localhost"));
```

## Removal of DeleteRecordOptions and UpsertRecordOptions

The `DeleteRecordOptions` and `UpsertRecordOptions` parameters have been removed from the
`DeleteAsync`, `DeleteBatchAsync`, `UpsertAsync` and `UpsertBatchAsync` methods on the
`IVectorStoreRecordCollection<TKey, TRecord>` interface.

These parameters were all optional and the options classes did not contain any options to set.

If you were passing these options in the past, you will need to remove these with this update.

```csharp
// Before
collection.DeleteAsync("mykey", new DeleteRecordOptions(), cancellationToken);

// After
collection.DeleteAsync("mykey", cancellationToken);
```

::: zone-end
::: zone pivot="programming-language-python"

## Not Applicable

These changes are currently only applicable in C#

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

These changes are currently only applicable in C#

::: zone-end
