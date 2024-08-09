---
title: How to create a custom mapper for a Semantic Kernel Vector Store connector (Experimental)
description: Describes how to create a custom mapper for a Semantic Kernel Vector Store connector
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# How to create a custom mapper for a Vector Store connector (Experimental)

In this how to, we will show how you can replace the default mapper with your own mapper, and how you can use a record definition to define a storage
schema that does not resemble your data model. We will use Qdrant, but the concepts will be similar for other connectors.

## Background

Each Vector Store connector includes a default mapper that can map from the provided data model to the storage schema supported by the underlying store.
Some stores allow a lot of freedom with regards to how data is stored while other stores require a more structured approach, e.g. where all vectors have
to be added to a dictionary of vectors and all non-vector fields to a dictionary of data fields. Therefore, mapping is an important part of abstracting
away the differences of each data store implementation.

In some cases, the developer may want to replace the default mapper if e.g. they do not want their data model and storage schema to match, or they want
to build an optimized mapper for their scenario.
All Vector Store connector implementations allow you to provide a custom mapper.

## Differences by vector store type

The underlying data stores of each Vector Store connector have different ways of storing data. Therefore what you are mapping to on the storage side may
differ for each connector.

::: zone pivot="programming-language-csharp"
E.g. if using the Qdrant connector, the storage type is a `PointStruct` class provided by the Qdrant SDK. If using the Redis JSON connector, the storage type
is a `string` key and a `JsonNode`, while if using a JSON HashSet connector, the storage type is a `string` key and a `HashEntry` array.
::: zone-end
::: zone pivot="programming-language-python"
::: zone-end
::: zone pivot="programming-language-java"
::: zone-end

If you want to do custom mapping, and you want to use multiple connector types, you will therefore need to implement a mapper for each connector type.

::: zone pivot="programming-language-csharp"

## Creating the data model

Our first step is to create a data model. In this case we will not annotate the data model with attributes, since we will provide a separate record definition
that describes what the database schema will look like.

Also note that this model is complex, with seperate classes for vectors and additional product info.

```csharp
public class Product
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProductVectors Vectors { get; set; }
    public ProductInfo ProductInfo { get; set; }
}

public class ProductInfo
{
    public double Price { get; set; }
    public string SupplierId { get; set; }
}

public class ProductVectors
{
    public ReadOnlyMemory<float> NameEmbedding { get; set; }
    public ReadOnlyMemory<float> DescriptionEmbedding { get; set; }
}
```

## Creating the record definition

We need to create a record definition instance to define what the database schema will look like. Normally a connector will require this information to do
mapping when using the default mapper. Since we are creating a custom mapper, this is not required for mapping, however, the connector will still require
this information for creating collections in the data store.

Note that the definition here is different to the data model above. To store `ProductInfo` we have a string property called `ProductInfoJson`, and
the two vectors are defined at the same level as the `Id`, `Name` and `Description` fields.

```csharp
using Microsoft.SemanticKernel.Data;

var productDefinition = new VectorStoreRecordDefinition
{
    Properties = new List<VectorStoreRecordProperty>
    {
        new VectorStoreRecordKeyProperty("Id", typeof(ulong)),
        new VectorStoreRecordDataProperty("Name", typeof(string)) { IsFilterable = true },
        new VectorStoreRecordDataProperty("Description", typeof(string)),
        new VectorStoreRecordDataProperty("ProductInfoJson", typeof(string)),
        new VectorStoreRecordVectorProperty("NameEmbedding", typeof(ReadOnlyMemory<float>)) { Dimensions = 1536 },
        new VectorStoreRecordVectorProperty("DescriptionEmbedding", typeof(ReadOnlyMemory<float>)) { Dimensions = 1536 }
    }
};
```

## Creating the custom mapper

All mappers implement the generic interface `Microsoft.SemanticKernel.Data.IVectorStoreRecordMapper<TRecordDataModel, TStorageModel>`.
`TRecordDataModel` will differ depending on what data model the developer wants to use, and `TStorageModel` will be determined by the type of Vector Store.

For Qdrant `TStorageModel` is `Qdrant.Client.Grpc.PointStruct`.

We therefore have to implement a mapper that will map between our `Product` data model and a Qdrant `PointStruct`.

```csharp
using Microsoft.SemanticKernel.Data;
using Qdrant.Client.Grpc;

public class ProductMapper : IVectorStoreRecordMapper<Product, PointStruct>
{
    public PointStruct MapFromDataToStorageModel(Product dataModel)
    {
        // Create a Qdrant PointStruct to map our data to.
        var pointStruct = new PointStruct
        {
            Id = new PointId { Num = dataModel.Id },
            Vectors = new Vectors(),
            Payload = { },
        };

        // Add the data fields to the payload dictionary and serialize the product info into a json string.
        pointStruct.Payload.Add("Name", dataModel.Name);
        pointStruct.Payload.Add("Description", dataModel.Description);
        pointStruct.Payload.Add("ProductInfoJson", JsonSerializer.Serialize(dataModel.ProductInfo));

        // Add the vector fields to the vector dictionary.
        var namedVectors = new NamedVectors();
        namedVectors.Vectors.Add("NameEmbedding", dataModel.Vectors.NameEmbedding.ToArray());
        namedVectors.Vectors.Add("DescriptionEmbedding", dataModel.Vectors.DescriptionEmbedding.ToArray());
        pointStruct.Vectors.Vectors_ = namedVectors;

        return pointStruct;
    }

    public Product MapFromStorageToDataModel(PointStruct storageModel, StorageToDataModelMapperOptions options)
    {
        var product = new Product
        {
            Id = storageModel.Id.Num,

            // Retrieve the data fields from the payload dictionary and deserialize the product info from the json string that it was stored as.
            Name = storageModel.Payload["Name"].StringValue,
            Description = storageModel.Payload["Description"].StringValue,
            ProductInfo = JsonSerializer.Deserialize<ProductInfo>(storageModel.Payload["ProductInfoJson"].StringValue)!,

            // Retrieve the vector fields from the vector dictionary.
            Vectors = new ProductVectors
            {
                NameEmbedding = new ReadOnlyMemory<float>(storageModel.Vectors.Vectors_.Vectors["NameEmbedding"].Data.ToArray()),
                DescriptionEmbedding = new ReadOnlyMemory<float>(storageModel.Vectors.Vectors_.Vectors["DescriptionEmbedding"].Data.ToArray())
            }
        };

        return product;
    }
}
```

## Constructing the collection

To use the custom mapper that we have created, we need to pass it to the collection at construction time.
We also need to pass the record definition that we created earlier, so that collections are created in the
data store using the right schema.
One more setting that is important here, is Qdrant's named vectors mode, since we have more than one
vector. Without this mode switched on, only one vector is supported.

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

var productMapper = new ProductMapper();
var collection = new QdrantVectorStoreRecordCollection<Product>(
    new QdrantClient("localhost"),
    "skproducts",
    new()
    {
        HasNamedVectors = true,
        PointStructCustomMapper = productMapper,
        VectorStoreRecordDefinition = productDefinition
    });
```

## Using a custom mapper with IVectorStore

When using `IVectorStore` to get `IVectorStoreRecordCollection` object instances, it is not possible to provide a custom mapper directly to
the `GetCollection` method. This is because custom mappers differ for each Vector Store type, and would make it impossible to use `IVectorStore`
to communicate with any vector store implementation.

It is however possible to provide a factory when constructing a Vector Store implementation. This can be used to customize `IVectorStoreRecordCollection`
instances as they are created.

Here is an example of such a factory, which checks if `CreateCollection` was called with the product definition and data type, and if so
injects the custom mapper and switches on named vectors mode.

```csharp
public class QdrantCollectionFactory(VectorStoreRecordDefinition productDefinition) : IQdrantVectorStoreRecordCollectionFactory
{
    public IVectorStoreRecordCollection<TKey, TRecord> CreateVectorStoreRecordCollection<TKey, TRecord>(QdrantClient qdrantClient, string name, VectorStoreRecordDefinition? vectorStoreRecordDefinition)
        where TKey : notnull
        where TRecord : class
    {
        // If the record definition is the product definition and the record type is the product data model, inject the custom mapper into the collection options.
        if (vectorStoreRecordDefinition == productDefinition && typeof(TRecord) == typeof(Product))
        {
            var customCollection = new QdrantVectorStoreRecordCollection<Product>(
                qdrantClient,
                name,
                new()
                {
                    HasNamedVectors = true,
                    PointStructCustomMapper = new ProductMapper(),
                    VectorStoreRecordDefinition = vectorStoreRecordDefinition
                }) as IVectorStoreRecordCollection<TKey, TRecord>;
            return customCollection!;
        }

        // Otherwise, just create a standard collection with the default mapper.
        var collection = new QdrantVectorStoreRecordCollection<TRecord>(
            qdrantClient,
            name,
            new()
            {
                VectorStoreRecordDefinition = vectorStoreRecordDefinition
            }) as IVectorStoreRecordCollection<TKey, TRecord>;
        return collection!;
    }
}
```

To use the collection factory, pass it to the Vector Store when constructing it, or when registering it with the dependency injection container.

```csharp
// When registering with the dependency injection container on the kernel builder.
kernelBuilder.AddQdrantVectorStore(
    "localhost",
    options: new()
    {
        VectorStoreCollectionFactory = new QdrantCollectionFactory(productDefinition)
    });

// When constructing the Vector Store instance directly.
var vectorStore = new QdrantVectorStore(
    new QdrantClient("localhost"),
    new()
    {
        VectorStoreCollectionFactory = new QdrantCollectionFactory(productDefinition)
    });
```

Then just use the vector store as normal to get a collection.

```csharp
var collection = vectorStore.GetCollection<ulong, Product>("skproducts", productDefinition);
```

::: zone-end
::: zone pivot="programming-language-python"

## Coming Soon

More info coming soon.

::: zone-end
::: zone pivot="programming-language-java"

## Coming Soon

More info coming soon.

::: zone-end
