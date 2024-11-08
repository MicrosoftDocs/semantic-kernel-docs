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

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

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

### 1. Implement the core interfaces

1.1 The three core interfaces that need to be implemented are:

- Microsoft.Extensions.VectorData.IVectorStore
- Microsoft.Extensions.VectorData.IVectorStoreRecordCollection\<TKey, TRecord\>
- Microsoft.Extensions.VectorData.IVectorizedSearch\<TRecord\>

Note that `IVectorStoreRecordCollection<TKey, TRecord>` inherits from `IVectorizedSearch<TRecord>`, so only
two classes are required to implement the three interfaces. The following naming convention should be used:

- {database type}VectorStore : IVectorStore
- {database type}VectorStoreRecordCollection<TKey, TRecord\> : IVectorStoreRecordCollection\<TKey, TRecord\>

E.g.

- MyDbVectorStore : IVectorStore
- MyDbVectorStoreRecordCollection<TKey, TRecord\> : IVectorStoreRecordCollection\<TKey, TRecord\>

The `VectorStoreRecordCollection` implementation should accept the name of the collection as a construtor parameter
and each instance of it is therefore tied to a specific collection instance in the database.

Here follows specific requirements for individual methods on these interfaces.

1.2 *`IVectorStore.GetCollection`* implementations should not do any checks to verify whether a collection exists or not.
The method should simply construct a collection object and return it. The user can optionally use the
`CollectionExistsAsync` method to check if the collection exists in cases where this is not known.
Doing checks on each invocation of `GetCollection` may add unwanted overhead for users when they are
working with a collection that they know exists.

1.3 *`IVectorStoreRecordCollection<TKey, TRecord>.UpsertAsync`* and *`IVectorStoreRecordCollection<TKey, TRecord>.UpsertBatchAsync`*
should return the keys of the upserted records. This allows for the case where a database supports generating
keys automatically. In this case the keys on the record(s) passed to the upsert method can be null, and the
generated key(s) will be returned.

1.4 *`IVectorStoreRecordCollection<TKey, TRecord>.DeleteAsync`* should succeed if the record does not exist and
for any other failures an exception should be thrown.
See the [standard exceptions](#11-standard-exceptions) section for requirements on the exception types to throw.

1.5 *`IVectorStoreRecordCollection<TKey, TRecord>.DeleteBatchAsync`* should succeed if any of the requested records
do not exist and for any other failures an exception should be thrown.
See the [standard exceptions](#11-standard-exceptions) section for requirements on the exception types to throw.

1.6 *`IVectorStoreRecordCollection<TKey, TRecord>.GetAsync`* should return null and not throw if a record is not found.
For any other failures an exception should be thrown.
See the [standard exceptions](#11-standard-exceptions) section for requirements on the exception types to throw.

1.7 *`IVectorStoreRecordCollection<TKey, TRecord>.GetBatchAsync`* should return the subset of records that were found
and not throw if any of the requested records were not found. For any other failures an exception should be thrown.
See the [standard exceptions](#11-standard-exceptions) section for requirements on the exception types to throw.

1.8 *`IVectorStoreRecordCollection<TKey, TRecord>.GetAsync`* implementations should
respect the `IncludeVectors` option provided via `GetRecordOptions` where possible.
Vectors are often most useful in the database itself, since that is where vector
comparison happens during vector searches and downloading them can be costly due to their size.
There may be cases where the database doesn't support excluding vectors in which case
returning them is acceptable.

1.9 *`IVectorizedSearch<TRecord>.VectorizedSearchAsync<TVector>`* implementations should also
respect the `IncludeVectors` option provided via `VectorSearchOptions` where possible.

1.10 *`IVectorizedSearch<TRecord>.VectorizedSearchAsync<TVector>`* implementations should simulate
the `Top` and `Skip` functionality requested via `VectorSearchOptions` if the database
does not support this natively. To simulate this behavior, the implementation should
fetch a number of results equal to Top + Skip, and then skip the first Skip number of results
before returning the remaining results.

1.11 *`IVectorizedSearch<TRecord>.VectorizedSearchAsync<TVector>`* implementations should ignore
the `IncludeTotalCount` option provided via `VectorSearchOptions` if the database
does not support this natively.

1.12 *`IVectorizedSearch<TRecord>.VectorizedSearchAsync<TVector>`* implementations should default
to the first vector if the `VectorPropertyName` option was not provided via `VectorSearchOptions`.
If a user does provide this value, the expected name should be the property name from the data model
and not any customized name that the property may be stored under in the database. E.g. let's say
the user has a data model property called `TextEmbedding` and they decorated the property with a
`JsonPropertyNameAttribute` that indicates that it should be serialized as `text_embedding`. Assuming
that the database is json based, it means that the property should be stored in the database with the
name `text_embedding`. When specifying the `VectorPropertyName` option, the user should always provide
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

If the user does not provide a `VectorStoreRecordDefinition`, this information should
be read from the data model attributes using reflection. If the user did provide a
`VectorStoreRecordDefinition`, the data model should not be used as the source of truth.
The `VectorStoreRecordDefinition` may have been provided with a custom mapper, in order
for the database schema and data model to differ. In this case the `VectorStoreRecordDefinition`
should match the database schema, but the data model may be deliberately different.

> [!TIP]
> Refer to [Defining your data model](../defining-your-data-model.md) for a detailed list of
> all attributes and settings that need to be supported.

### 3. Support record definitions

As mentioned in [Support data model attributes](#2-support-data-model-attributes) we need
information about each property to build out a connector. This information can also
be supplied via a `VectorStoreRecordDefinition` and if supplied, the connector should
avoid trying to read this information from the data model or try and validate that the
data model matches the definition in any way.

The user should be able to provide a `VectorStoreRecordDefinition` to the
`IVectorStoreRecordCollection` implementation via options.

> [!TIP]
> Refer to [Defining your storage schema using a record definition](../schema-with-record-definition.md) for a detailed list of
> all record definition settings that need to be supported.

### 4. Collection / Index Creation

4.1 A user can optionally choose an index kind and distance function for each vector property.
These are specified via string based settings, but where available a connector should expect
the strings that are provided as string consts on `Microsoft.Extensions.VectorData.IndexKind`
and `Microsoft.Extensions.VectorData.DistanceFunction`. Where the connector requires
index kinds and distance functions that are not available on the abovementioned static classes
additional custom strings may be accepted.

E.g. the goal is for a user to be able to specify a standard distance function, like `DotProductSimilarity`
for any connector that supports this distance function, without needing to use different
naming for each connector.

```csharp
    [VectorStoreRecordVector(1536, DistanceFunction.DotProductSimilarity]
    public ReadOnlyMemory<float>? Embedding { get; set; }
```

4.2 A user can optionally choose whether each data property should be filterable or full text searchable.
In some databases, all properties may already be filterable or full text searchable by default, however
in many databases, special indexing is required to achieve this. If special indexing is required
this also means that adding this indexing will most likely incur extra cost.
The `IsFilterable` and `IsFullTextSearchable` settings allow a user to control whether to enable
this additional indexing per property.

### 5. Data model validation

Every database doesn't support every data type. To improve the user experience it's important to validate
the data types of any record properties and to do so early, e.g. when an `IVectorStoreRecordCollection`
instance is constructed. This way the user will be notified of any potential failures before starting to use the database.

The type of validation required will also depend on the type of mapper used by the user. E.g. The user may have supplied
a custom data model, a custom mapper and a `VectorStoreRecordDefinition`. They may want the data model to differ significantly
from the storage schema, and the custom mapper would map between the two. In this case, we want to avoid doing any checks
on the data model, but focus on the `VectorStoreRecordDefinition` only, to ensure the data types requested are allowed
by the underlying database.

Let's consider each scenario.

|Data model type|VectorStore RecordDefinition supplied|Custom mapper supplied| Combination Supported | Validation required                 |
|---------------|-------------------------------------|----------------------|-----------------------|-------------------------------------|
|Custom         |Yes                                  |Yes                   |Yes                    | Validate definition nly            |
|Custom         |Yes                                  |No                    |Yes                    | Validate definition and check data model for matching properties |
|Custom         |No                                   |Yes                   |Yes                    | Validate data model properties      |
|Custom         |No                                   |No                    |Yes                    | Validate data model properties      |
|Generic        |Yes                                  |Yes                   |Yes                    | Validate definition only            |
|Generic        |Yes                                  |No                    |Yes                    | Validate definition and data type of GenericDataModel Key |
|Generic        |No                                   |Yes                   |No - Definition required for collection create |             |
|Generic        |No                                   |No                    |No - Definition required for collection create and mapper |  |

### 6. Storage property naming

The naming conventions used for properties in code doesn't always match the prefered naming
for matching fields in a database.
It is therefore valueable to support customized storage names for properties.
Some databases may support storage formats that already have their own mechanism for
specifying storage names, e.g. when using JSON as the storage format you can use
a `JsonPropertyNameAttribute` to provide a custom name.

6.1 Where the database has a storage format that supports its own mechanism for specifying storage
names, the connector should preferably use that mechanism.

6.2 Where the database does not use a storage format that supports its own mechanism for specifying
storage names, the connector must support the `StoragePropertyName` settings from the data model
attributes or the `VectorStoreRecordDefinition`.

### 7. Mapper support

Connectors should provide the ability to map between the user supplied data model and the
storage model that the database requires, but should also provide some flexibility in how
that mapping is done. Most connectors would typically need to support the following three
mappers.

7.1 All connectors should come with a built in mapper that can map between the user supplied
data model and the storage model required by the underlying database.

7.2 To allow users the ability to support data models that vary significantly from
the storage models of the underlying database, or to customize the mapping behavior
between the two, each connector must support custom mappers.

The `IVectorStoreRecordCollection` implementation should allow a user to provide a custom
mapper via options. E.g.

```csharp
public IVectorStoreRecordMapper<TRecord, MyDBRecord>? MyDBRecordCustomMapper { get; init; } = null;
```

Mappers should all use the same standard interface
`IMicrosoft.Extensions.VectorData.VectorStoreRecordMapper<TRecordDataModel, TStorageModel>`.
`TRecordDataModel` should be the data model chosen by the user, while `TStorageModel` should be whatever
data type the database client requires.

7.3. All connectors should have a built in mapper that works with the `VectorStoreGenericDataModel`.
See [Support GenericDataModel](#8-support-genericdatamodel) for more information.

### 8. Support GenericDataModel

While it is very useful for users to be able to define their own data model, in some cases
it may not be desirable, e.g. when the database schema is not known at coding time and driven
by configuration.

To support this scenario, connectors should have out of the box support for the generic data
model supplied by the abstraction package: `Microsoft.Extensions.VectorData.VectorStoreGenericDataModel<TKey>`.

In practice this means that the connector must implement a special mapper
to support the generic data model. The connector should automatically use this mapper
if the user specified the generic data model as their data model and they did not provide
their own custom mapper.

### 9. Support divergent data model and database schema

In most cases there will be a logical default mapping between the data model
and storage model. E.g. property x on the data model maps to property x on the
storage model. The built in mapper provided by the connector should support
this default case.

There may be scenarios where the user wants to do something more complex, e.g.
use a data model that has complex properties, where sub properties of a property
on the data model are mapped to individual properties on the storage model. In
this scenario the user would need to supply both a custom mapper and a
`VectorStoreRecordDefinition`. The `VectorStoreRecordDefinition` is required
to describe the database schema for collection / index create scenarios, while
the custom mapper is required to map between the data and storage models.

To support this scenario, the connector must fulfil the following requirements:

- Allow a user to supply a custom mapper and `VectorStoreRecordDefinition`.
- Use the `VectorStoreRecordDefinition` to create collections / indexes.
- Avoid doing reflection on the data model if a custom mapper and `VectorStoreRecordDefinition` is supplied

### 10. Support Vector Store Record Collection factory

The `IVectorStore.GetCollection` method can be used to create instances of `IVectorStoreRecordCollection`.
Some connectors however may allow or require users to provide additional configuration options
on a per collection basis, that is specific to the underlying database.
E.g. Qdrant allows two modes, one where a single unnamed vector is allowed per record, and another
where zero or more named vectors are allowed per record. The mode can be different for each
collection.

When constructing an `IVectorStoreRecordCollection` instance directly, these settings can be passed directly
to the constructor of the concrete implementation as an option. If a user is using the
`IVectorStore.GetCollection` method, this is not possible, since these settings are database specific and will
therefore break the abstraction if passed here.

To allow customization of these settings when using `IVectorStore.GetCollection`, it is important
that each connector supports an optional `VectorStoreRecordCollectionFactory` that can be passed to the concrete
implementation of `IVectorStore` as an option. Each connector should therefore provide an interface, similar to the
following sample. If a user passes an implementation of this to the `VectorStore` as an option, this
can be used by the `IVectorStore.GetCollection` method to consruct the `IVectorStoreRecordCollection` instance.

```csharp
public sealed class MyDBVectorStore : IVectorStore
{
    public IVectorStoreRecordCollection<TKey, TRecord> GetCollection<TKey, TRecord>(string name, VectorStoreRecordDefinition? vectorStoreRecordDefinition = null)
        where TKey : notnull
    {
        if (typeof(TKey) != typeof(string))
        {
            throw new NotSupportedException("Only string keys are supported by MyDB.");
        }

        if (this._options.VectorStoreCollectionFactory is not null)
        {
            return this._options.VectorStoreCollectionFactory.CreateVectorStoreRecordCollection<TKey, TRecord>(this._myDBClient, name, vectorStoreRecordDefinition);
        }

        var recordCollection = new MyDBVectorStoreRecordCollection<TRecord>(
            this._myDBClient,
            name,
            new MyDBVectorStoreRecordCollectionOptions<TRecord>()
            {
                VectorStoreRecordDefinition = vectorStoreRecordDefinition
            }) as IVectorStoreRecordCollection<TKey, TRecord>;

        return recordCollection!;
    }
}

public sealed class MyDBVectorStoreOptions
{
    public IMyDBVectorStoreRecordCollectionFactory? VectorStoreCollectionFactory { get; init; }
}

public interface IMyDBVectorStoreRecordCollectionFactory
{
    /// <summary>
    /// Constructs a new instance of the <see cref="IVectorStoreRecordCollection{TKey, TRecord}"/>.
    /// </summary>
    /// <typeparam name="TKey">The data type of the record key.</typeparam>
    /// <typeparam name="TRecord">The data model to use for adding, updating and retrieving data from storage.</typeparam>
    /// <param name="myDBClient">Database Client.</param>
    /// <param name="name">The name of the collection to connect to.</param>
    /// <param name="vectorStoreRecordDefinition">An optional record definition that defines the schema of the record type. If not present, attributes on <typeparamref name="TRecord"/> will be used.</param>
    /// <returns>The new instance of <see cref="IVectorStoreRecordCollection{TKey, TRecord}"/>.</returns>
    IVectorStoreRecordCollection<TKey, TRecord> CreateVectorStoreRecordCollection<TKey, TRecord>(
        MyDBClient myDBClient,
        string name,
        VectorStoreRecordDefinition? vectorStoreRecordDefinition)
            where TKey : notnull;
}
```

### 11. Standard Exceptions

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

### 12. Batching

The `IVectorStoreRecordCollection` interface includes batching overloads for Get, Upsert and Delete.
Not all underlying database clients may have the same level of support for batching, so let's consider
each option.

Firstly, if the database client doesn't support batching. In this case the connector should simulate
batching by executing all provided requests in parallel. Assume that the user has broken up the requests
into small enough batches already so that parallel requests will succeed without throttling.

E.g. here is an exmaple where batching is simulated with requests happening in parallel.

```csharp
public Task DeleteBatchAsync(IEnumerable<string> keys, DeleteRecordOptions? options = default, CancellationToken cancellationToken = default)
{
    if (keys == null)
    {
        throw new ArgumentNullException(nameof(keys));
    }

    // Remove records in parallel.
    var tasks = keys.Select(key => this.DeleteAsync(key, options, cancellationToken));
    return Task.WhenAll(tasks);
}
```

Secondly, if the database client does support batching, pass all requests directly to the underlying
client so that it may send the entire set in one request.

## Recommended common patterns and pratices

1. Always use options classes for optional settings with smart defaults.
1. Keep required parameters on the main signature and move optional parameters to options.

Here is an example of an `IVectorStoreRecordCollection` constructor following this pattern.

```csharp
public sealed class MyDBVectorStoreRecordCollection<TRecord> : IVectorStoreRecordCollection<string, TRecord>
{
    public MyDBVectorStoreRecordCollection(MyDBClient myDBClient, string collectionName, MyDBVectorStoreRecordCollectionOptions<TRecord>? options = default)
    {
    }

    ...
}

public sealed class MyDBVectorStoreRecordCollectionOptions<TRecord>
{
    public VectorStoreRecordDefinition? VectorStoreRecordDefinition { get; init; } = null;
    public IVectorStoreRecordMapper<TRecord, MyDbRecord>? MyDbRecordCustomMapper { get; init; } = null;
}
```

## Documentation

To share the features and limitations of your implementation, you can contribute a documentation page to the
Microsoft Learn website. See [here](https://learn.microsoft.com/en-us/semantic-kernel/concepts/vector-store-connectors/out-of-the-box-connectors/)
for the documentation on the existing connectors.

To create your page, create a pull request on the [Semantic Kernel docs Github repository](https://github.com/MicrosoftDocs/semantic-kernel-docs).
Use the pages in the following folder as examples: [Out-of-the-box connectors](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/semantic-kernel/concepts/vector-store-connectors/out-of-the-box-connectors)

Areas to cover:

1. An `Overview` with a standard table describing the main features of the connector.
1. An optional `Limitations` section with any limitations for your connector.
1. A `Getting started` section that describes how to import your nuget and construct your `VectorStore` and `VectorStoreRecordCollection`
1. A `Data mapping` section that shows how your connector maps data by default from a data model to the database storage model including any property renaming it may support.
1. Any other information about features that your connector supports.

::: zone-end
::: zone pivot="programming-language-python"

## Coming soon

More info coming soon.

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end
