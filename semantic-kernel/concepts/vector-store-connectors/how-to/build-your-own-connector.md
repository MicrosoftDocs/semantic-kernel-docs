---
title: How to build your own Vector Store connector (Preview)
description: Describes how to build your own Vector Store connector connector
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# How to build your own Vector Store connector (Preview)

::: zone pivot="programming-language-csharp"

::: zone-end
::: zone pivot="programming-language-python"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end
::: zone pivot="programming-language-java"

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

::: zone-end

::: zone pivot="programming-language-csharp"

This article provides guidance for anyone who wishes to build their own Vector Store connector.
This article can be used by database providers who wish to build and maintain their own implementation,
or for anyone who wishes to build and maintain an unofficial connector for a database that lacks support.

If you wish to contribute your connector to the Semantic Kernel code base:

1. Create an issue in the [Semantic Kernel Github repository](https://github.com/microsoft/semantic-kernel/issues).
1. Review the [Semantic Kernel contribution guidelines](https://github.com/microsoft/semantic-kernel/blob/main/CONTRIBUTING.md).

## Overview

Vector Store connectors are implementations of the [Vector Store abstraction](https://www.nuget.org/packages/Microsoft.Extensions.VectorData.Abstractions). Some of the decisions that
were made when designing the Vector Store abstraction mean that a Vector Store connector requires certain
features to provide users with a good experience.

A key design decision is that the Vector Store abstraction takes a strongly typed approach to working with database records.
This means that `UpsertAsync` takes a strongly typed record as input, while `GetAsync` returns a strongly typed record.
The design uses C# generics to achieve the strong typing.
This means that a connector has to be able to map from this data model to the storage model used by the underlying database.
It also means that a connector may need to find out certain information about the record properties in order to know how
to map each of these properties. E.g. some vector databases (such as Chroma, Qdrant and Weaviate) require vectors to be stored in a specific structure and non-vectors
in a different structure, or require record keys to be stored in a specific field.

At the same time, the Vector Store abstraction also provides a generic data model that allows a developer to work
with a database without needing to create a custom data model.

It is important for connectors to support different types of model and provide developers with flexibility around
how they use the connector. The following section deep dives into each of these requirements.

## Requirements

In order to be considered a full implementation of the Vector Store abstractions, the following set of requirements must be met.

### 1. Implement the core abstract base clases and interfaces

1.1 The three core abstract base classes and interfaces that need to be implemented are:

- Microsoft.Extensions.VectorData.VectorStore
- Microsoft.Extensions.VectorData.VectorStoreCollection\<TKey, TRecord\>
- Microsoft.Extensions.VectorData.IVectorSearchable\<TRecord\>

Note that `VectorStoreCollection<TKey, TRecord>` implements `IVectorSearchable<TRecord>`, so only
two inheriting classes are required. The following naming convention should be used:

- {database type}VectorStore : VectorStore
- {database type}Collection<TKey, TRecord\> : VectorStoreCollection\<TKey, TRecord\>

E.g.

- MyDbVectorStore : VectorStore
- MyDbCollection<TKey, TRecord\> : VectorStoreCollection\<TKey, TRecord\>

The `VectorStoreCollection` implementation should accept the name of the collection as a constructor parameter
and each instance of it is therefore tied to a specific collection instance in the database.

Here follows specific requirements for individual methods on these abstract base classes and interfaces.

1.2 *`VectorStore.GetCollection`* implementations should not do any checks to verify whether a collection exists or not.
The method should simply construct a collection object and return it. The user can optionally use the
`CollectionExistsAsync` method to check if the collection exists in cases where this is not known.
Doing checks on each invocation of `GetCollection` may add unwanted overhead for users when they are
working with a collection that they know exists.

1.3 *`VectorStoreCollection<TKey, TRecord>.DeleteAsync`* that takes a single key as input should succeed if the record does not exist and
for any other failures an exception should be thrown.
See the [standard exceptions](#10-standard-exceptions) section for requirements on the exception types to throw.

1.4 *`VectorStoreCollection<TKey, TRecord>.DeleteAsync`* that takes multiple keys as input should succeed if any of the requested records
do not exist and for any other failures an exception should be thrown.
See the [standard exceptions](#10-standard-exceptions) section for requirements on the exception types to throw.

1.5 *`VectorStoreCollection<TKey, TRecord>.GetAsync`* that takes a single key as input should return null and not throw if a record is not found.
For any other failures an exception should be thrown.
See the [standard exceptions](#10-standard-exceptions) section for requirements on the exception types to throw.

1.6 *`VectorStoreCollection<TKey, TRecord>.GetAsync`* that takes multiple keys as input should return the subset of records that were found
and not throw if any of the requested records were not found. For any other failures an exception should be thrown.
See the [standard exceptions](#10-standard-exceptions) section for requirements on the exception types to throw.

1.7 *`VectorStoreCollection<TKey, TRecord>.GetAsync`* implementations should
respect the `IncludeVectors` option provided via `RecordRetrievalOptions` where possible.
Vectors are often most useful in the database itself, since that is where vector
comparison happens during vector searches and downloading them can be costly due to their size.
There may be cases where the database doesn't support excluding vectors in which case
returning them is acceptable.

1.8 *`IVectorSearchable<TRecord>.SearchAsync<TVector>`* implementations should also
respect the `IncludeVectors` option provided via `VectorSearchOptions<TRecord>` where possible.

1.9 *`IVectorSearchable<TRecord>.SearchAsync<TVector>`* implementations should simulate
the `Top` and `Skip` functionality requested via `VectorSearchOptions<TRecord>` if the database
does not support this natively. To simulate this behavior, the implementation should
fetch a number of results equal to Top + Skip, and then skip the first Skip number of results
before returning the remaining results.

1.10 *`IVectorSearchable<TRecord>.SearchAsync<TVector>`* implementations should not require
`VectorPropertyName` or `VectorProperty` to be specified if only one vector exists on the data model.
In this case that single vector should automatically become the search target. If no vector or
multiple vectors exists on the data model, and no `VectorPropertyName` or `VectorProperty` is provided
the search method should throw.

When using `VectorPropertyName`, if a user does provide this value, the expected name should be the
property name from the data model and not any customized name that the property may be stored under
in the database. E.g. let's say the user has a data model property called `TextEmbedding` and they
decorated the property with a `JsonPropertyNameAttribute` that indicates that it should be serialized
as `text_embedding`. Assuming that the database is json based, it means that the property should be
stored in the database with the name `text_embedding`.
 When specifying the `VectorPropertyName` option, the user should always provide
`TextEmbedding` as the value. This is to ensure that where different connectors are used with the
same data model, the user can always use the same property names, even though the storage name
of the property may be different.

### 2. Support data model attributes

The Vector Store abstraction allows a user to use attributes to decorate their data model
to indicate the type of each property and to configure the type of indexing required
for each vector property.

This information is typically required for

1. Mapping between a data model and the underlying database's storage model
1. Creating a collection / index
1. Vector Search

If the user does not provide a `VectorStoreCollectionDefinition`, this information should
be read from the data model attributes using reflection. If the user did provide a
`VectorStoreCollectionDefinition`, the data model should not be used as the source of truth.

> [!TIP]
> Refer to [Defining your data model](../defining-your-data-model.md) for a detailed list of
> all attributes and settings that need to be supported.

### 3. Support record definitions

As mentioned in [Support data model attributes](#2-support-data-model-attributes) we need
information about each property to build out a connector. This information can also
be supplied via a `VectorStoreCollectionDefinition` and if supplied, the connector should
avoid trying to read this information from the data model or try and validate that the
data model matches the definition in any way.

The user should be able to provide a `VectorStoreCollectionDefinition` to the
`VectorStoreCollection` implementation via options.

> [!TIP]
> Refer to [Defining your storage schema using a record definition](../schema-with-record-definition.md) for a detailed list of
> all record definition settings that need to be supported.

### 4. Collection / Index Creation

4.1 A user can optionally choose an index kind and distance function for each vector property.
These are specified via string based settings, but where available a connector should expect
the strings that are provided as string consts on `Microsoft.Extensions.VectorData.IndexKind`
and `Microsoft.Extensions.VectorData.DistanceFunction`. Where the connector requires
index kinds and distance functions that are not available on the above mentioned static classes
additional custom strings may be accepted.

E.g. the goal is for a user to be able to specify a standard distance function, like `DotProductSimilarity`
for any connector that supports this distance function, without needing to use different
naming for each connector.

```csharp
    [VectorStoreVector(1536, DistanceFunction = DistanceFunction.DotProductSimilarity]
    public ReadOnlyMemory<float>? Embedding { get; set; }
```

4.2 A user can optionally choose whether each data property should be indexed or full text indexed.
In some databases, all properties may already be filterable or full text searchable by default, however
in many databases, special indexing is required to achieve this. If special indexing is required
this also means that adding this indexing will most likely incur extra cost.
The `IsIndexed` and `IsFullTextIndexed` settings allow a user to control whether to enable
this additional indexing per property.

### 5. Data model validation

Every database doesn't support every data type. To improve the user experience it's important to validate
the data types of any record properties and to do so early, e.g. when an `VectorStoreCollection`
instance is constructed. This way the user will be notified of any potential failures before starting to use the database.

### 6. Storage property naming

The naming conventions used for properties in code doesn't always match the preferred naming
for matching fields in a database.
It is therefore valuable to support customized storage names for properties.
Some databases may support storage formats that already have their own mechanism for
specifying storage names, e.g. when using JSON as the storage format you can use
a `JsonPropertyNameAttribute` to provide a custom name.

6.1 Where the database has a storage format that supports its own mechanism for specifying storage
names, the connector should preferably use that mechanism.

6.2 Where the database does not use a storage format that supports its own mechanism for specifying
storage names, the connector must support the `StorageName` settings from the data model
attributes or the `VectorStoreCollectionDefinition`.

### 7. Mapper support

Connectors should provide the ability to map between the user supplied data model and the
storage model that the database requires, but should also provide some flexibility in how
that mapping is done. Most connectors would typically need to support the following two
mappers.

7.1 All connectors should come with a built in mapper that can map between the user supplied
data model and the storage model required by the underlying database.

7.2. All connectors should have a built in mapper that works with the `VectorStoreGenericDataModel`.
See [Support GenericDataModel](#8-support-genericdatamodel) for more information.

### 8. Support GenericDataModel

While it is very useful for users to be able to define their own data model, in some cases
it may not be desirable, e.g. when the database schema is not known at coding time and driven
by configuration.

To support this scenario, connectors should have out of the box support for the generic data
model supplied by the abstraction package: `Microsoft.Extensions.VectorData.VectorStoreGenericDataModel<TKey>`.

In practice this means that the connector must implement a special mapper
to support the generic data model. The connector should automatically use this mapper
if the user specified the generic data model as their data model.

### 9. Divergent data model and database schema

The only divergence required to be supported by connector implementations are
customizing storage property names for any properties.

Any more complex divergence is not supported, since this causes additional
complexity for filtering. E.g. if the user has a filter expression that references
the data model, but the underlying schema is different to the data model, the
filter expression cannot be used against the underlying schema.

### 10. Standard Exceptions

The database operation methods provided by the connector should throw a set of standard
exceptions so that users of the abstraction know what exceptions they need to handle,
instead of having to catch a different set for each provider. E.g. if the underlying
database client throws a `MyDBClientException` when a call to the database fails, this
should be caught and wrapped in a `VectorStoreOperationException`, preferably preserving
the original exception as an inner exception.

11.1 For failures relating to service call or database failures the connector should throw:
`Microsoft.Extensions.VectorData.VectorStoreOperationException`

11.2 For mapping failures, the connector should throw:
`Microsoft.Extensions.VectorData.VectorStoreRecordMappingException`

11.3 For cases where a certain setting or feature is not supported, e.g. an unsupported index type, use:
`System.NotSupportedException`.

11.4 In addition, use `System.ArgumentException`, `System.ArgumentNullException` for argument validation.

### 11. Batching

The `VectorStoreCollection` abstract base class includes batching overloads for Get, Upsert and Delete.
Not all underlying database clients may have the same level of support for batching.

The base batch method implementations on `VectorStoreCollection` calls the abstract non-batch implementations in serial.
If the database supports batching natively, these base batch implementations should be overridden and implemented
using the native database support.

## Recommended common patterns and practices

1. Keep `VectorStore` and `VectorStoreCollection` implementations sealed. It is recommended to use a decorator pattern to override a default vector store behaviour.
1. Always use options classes for optional settings with smart defaults.
1. Keep required parameters on the main signature and move optional parameters to options.

Here is an example of an `VectorStoreCollection` constructor following this pattern.

```csharp
public sealed class MyDBCollection<TRecord> : VectorStoreCollection<string, TRecord>
{
    public MyDBCollection(MyDBClient myDBClient, string collectionName, MyDBCollectionOptions<TRecord>? options = default)
    {
    }

    ...
}

public class MyDBCollectionOptions<TRecord> : VectorStoreCollectionOptions
{
}
```

## SDK Changes

Please also see the following articles for a history of changes to the SDK and therefore implementation requirements:

1. [Vector Store Changes March 2025](../../../support/migration/vectorstore-march-2025.md)
1. [Vector Store Changes April 2025](../../../support/migration/vectorstore-april-2025.md)
1. [Vector Store Changes May 2025](../../../support/migration/vectorstore-may-2025.md)

::: zone-end
::: zone pivot="programming-language-python"

This article provides guidance for anyone who wishes to build their own Vector Store connector.
This article can be used by database providers who wish to build and maintain their own implementation,
or for anyone who wishes to build and maintain an unofficial connector for a database that lacks support.

In June 2025, the setup was updated see:
1. [Vector Store Changes for Python June 2025](../../../support/migration/vectorstore-python-june-2025.md)

If you wish to contribute your connector to the Semantic Kernel code base:

1. Create an issue in the [Semantic Kernel Github repository](https://github.com/microsoft/semantic-kernel/issues).
1. Review the [Semantic Kernel contribution guidelines](https://github.com/microsoft/semantic-kernel/blob/main/CONTRIBUTING.md).

## Overview

Vector Store connectors are implementations of the [Vector Store base classes](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/data/vector). Some of the decisions that
were made when designing the Vector Store abstraction mean that a Vector Store connector requires certain
features to provide users with a good experience.

A key design decision is that the Vector Store abstraction takes a strongly typed approach to working with database records.
This means that `upsert` takes a strongly typed record as input, while `get` returns a strongly typed record.
The design uses python generics to achieve the strong typing.
This means that a connector has to be able to map from this data model to the storage model used by the underlying database.
It also means that a connector may need to find out certain information about the record properties in order to know how
to map each of these properties. E.g. some vector databases (such as Chroma, Qdrant and Weaviate) require vectors to be stored in a specific structure and non-vectors
in a different structure, or require record keys to be stored in a specific field.

It is important for connectors to support different types of model and provide developers with flexibility around
how they use the connector. The following section deep dives into each of these requirements.

## Requirements

In order to be considered a full implementation of the Vector Store abstractions, the following set of requirements must be met.

### Implement the core classes

The two core classes that need to be implemented are:
1. VectorStore
1. VectorStoreCollection[TKey, TModel] and optionally VectorSearch[TKey, TModel]

The naming convention should be:
- {database type}Store
- {database type}Collection

E.g.
- MyDBStore
- MyDBCollection

A `VectorStoreCollection` is tied to a specific collection/index name in the database, that collection name should be passed to the constructor, or the `get_collection` method of the VectorStore.

These are the methods that need to be implemented:

1. VectorStore
   1. `VectorStore.get_collection` - This is a factory method that should return a new instance of the VectorStoreCollection for the given collection name, it should not do checks to verify whether a collection exists or not. It should also store the collection in the internal dict `vector_record_collections` so that it can be retrieved later.
   1. `VectorStore.list_collection_names` - This method should return a list of collection names that are available in the database.
1. VectorStoreCollection
   1. `VectorStoreCollection._inner_upsert` - This method takes a list of records and returns a list of keys that were updated or inserted, this method is called from the `upsert` method, those methods takes care of serialization.
   2. `VectorStoreCollection._inner_get` - This method takes a list of keys and returns a list of records, this method is called from the `get` method.
   3. `VectorStoreCollection._inner_delete` - This method takes a list of keys and deletes them from the database, this method is called from the `delete` method.
   4. `VectorStoreCollection._serialize_dicts_to_store_models` - This method takes a list of dicts and returns a list of objects ready to be upserted, this method is called from the `upsert` method, check the [Serialization docs for more info](../serialization.md), by this point a embedding is already generated when applicable.
   5. `VectorStoreCollection._deserialize_store_models_to_dicts` - This method takes a list of objects from the store and returns a list of dicts, this method is called from the `get` and optionally `search` methods, check the [Serialization docs for more info](../serialization.md)
   6. `VectorStoreCollection.ensure_collection_exists` - This method should create a collection/index in the database, it should be able to parse a `VectorStoreRecordDefinition` and create the collection/index accordingly and also allow the user to supply their own definition, ready for that store, this allows the user to leverage every feature of the store, even ones we don't. This method should first check if the collection exists, if it does not, it should create it, if it does exist, it should do nothing.
   7. `VectorStoreCollection.collection_exists` - This method should return a boolean indicating whether the collection exists or not.
   8. `VectorStoreCollection.ensure_collection_deleted` - This method should delete the collection/index from the database.
2. VectorSearch
   1. `VectorSearch._inner_search` - This method should take the query values or vector and options and search_type and return a `KernelSearchResults` with a `VectorSearchResult` as the internal content, the `KernelSearchResults` is a Async Iterable to allow support for paging results, as search can return a large number of results (there is a helper util to take a list of results and return a `AsyncIterable`). The search_type can be `vector` or `keyword_hybrid`, the first one is a pure vector search, the second one is a hybrid search that combines keyword and vector search, this is not supported in all vector stores, and the below mentioned `supported_search_types` class variable is be used to validate the search type and can also be inspected by users. There is a convenience function `_generate_vector_from_values` that can be used to generate a vector from the query values, for both search types.
   2. `VectorSearch._get_record_from_result` - This method should take the search result from the store and extract the actual content, this can also be as simple as returning the result.
   3. `VectorSearch._get_score_from_result` - This method should take the search result from the store and extract the score, this is not always present as some databases don't return a score.
   4. `VectorSearch._lambda_parser` - This method should take a lambda expression as AST (`abstract syntax tree`) and parse it into a filter expression that can be used by the store, this is called from a built-in method called `_build_filter` which takes care of parsing a lambda expression or string into a AST, and returns the results, the _inner_search method will then use the results of `_build_filter` to filter the results from the store. If you do not want to use `_build_filter` you can just implement `_lambda_parser` with a `pass`. The best way to understand more about this method is to look at the ones built, for instance in Azure AI Search or MongoDB, as they are fairly complete.

Some other optional items that can be implemented are:
1. `VectorStoreCollection._validate_data_model` - This method validates the data model, there is a default implementation that takes the `VectorStoreRecordDefinition` and validates the data model against it, with the values from the supported types (see below), but this can be overwritten to provide custom validation. A additional step can be added by doing `super()._validate_data_model()` to run the default validation first.
1. `VectorStoreCollection.supported_key_types` - This is a `classvar`, that should be a list of supported key types, this is used to validate the key type when creating a collection.
2. `VectorStoreCollection.supported_vector_types` - This is a `classvar`, that should be a list of supported vector types, this is used to validate the vector type when creating a collection.
3. `VectorSearch.supported_search_types` - This is a `classvar`, that should be a list of supported search types, `vector` or `keyword_hybrid`, this is used to validate the search type when searching.
3. `VectorStoreCollection.__aenter__` and `VectorStoreCollection.__aexit__` - These methods should be implemented to allow the use of the `async with` statement, this is useful for managing connections and resources.
4. `VectorStoreCollection.managed_client` - This is a helper property that can be used to indicate whether the current instance is managing the client or not, this is useful for the `__aenter__` and `__aexit__` methods and should be set based on the constructor arguments.
5. `VectorSearch.options_class` - This is a property that returns the search options class, by default this is the `VectorSearchOptions` but can be overwritten to provide a custom options class. The public methods perform a check of the options type and do a cast if needed.

### Collection / Index Creation
Every store has it's own quirks when it comes to the way indexes/collections are defined and which features are supported. Most implementation use some kind of helper or util function to parse the `VectorStoreRecordDefinition` and create the collection/index definition. This includes mapping from the Semantic Kernel IndexKind and DistanceFunction to the store specific ones, and raising an error when a unsupported index or distance function is used. It is advised to use a dict to map between these so that it is easy to update and maintain over time.

There are features in Semantic Kernel that are not available in the store and vice versa, for instance a data field might be marked as full text searchable in Semantic Kernel but the store might not make that distinction, in this case that setting is ignored. The inverse where there are settings available in the store but not in Semantic Kernel, a sensible default, with a clear docstring or comment on why that default is chosen, should be used and this is exactly the type of thing that a user might want to leverage the break glass feature for (supplying their own definition to the `ensure_collection_exists` method).

### Exceptions
Most exceptions are raised with the Semantic Kernel specific types by the public methods, so the developer of the connector should not worry about it, this also makes sure that a user does not have to think about very specific exceptions from each connector. You should also not catch things only to re-raise, that is done once so that the stack trace does not become overly long.

The vector store exceptions are all coming from the [vector_store_exceptions](https://github.com/microsoft/semantic-kernel/blob/main/python/semantic_kernel/exceptions/vector_store_exceptions.py).

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end

## Documentation

To share the features and limitations of your implementation, you can contribute a documentation page to this
Microsoft Learn website. See [here](../out-of-the-box-connectors/index.md)
for the documentation on the existing connectors.

To create your page, create a pull request on the [Semantic Kernel docs Github repository](https://github.com/MicrosoftDocs/semantic-kernel-docs).
Use the pages in the following folder as examples: [Out-of-the-box connectors](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/semantic-kernel/concepts/vector-store-connectors/out-of-the-box-connectors)

Areas to cover:

1. An `Overview` with a standard table describing the main features of the connector.
1. An optional `Limitations` section with any limitations for your connector.
1. A `Getting started` section that describes how to import your nuget and construct your `VectorStore` and `VectorStoreCollection`
1. A `Data mapping` section showing the connector's default data mapping mechanism to the database storage model, including any property renaming it may support.
1. Information about additional features your connector supports.
   