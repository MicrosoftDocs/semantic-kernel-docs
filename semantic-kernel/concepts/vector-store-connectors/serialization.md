---
title: Serialization of your data model to and from different stores (Preview)
description: How does Semantic Kernel serialize your data model to and from different stores
author: edvan
zone_pivot_groups: programming-languages
ms.topic: conceptual
ms.author: edvan
ms.date: 08/15/2024
ms.service: semantic-kernel
---
# Serialization of your data model to and from different stores (Preview)

::: zone pivot="programming-language-csharp"

In order for your data model to be stored in a database, it needs to be converted to a format that the database can understand.
Different databases require different storage schemas and formats. Some have a strict schema that needs to be adhered to, while
others allow the schema to be defined by the user.

## Mapping options

The vector store connectors provided by Semantic Kernel provide multiple ways to achieve this mapping.

### Built-in mappers

The vector store connectors provided by Semantic Kernel have built-in mappers that will map your data model to and from the
database schemas. See the [page for each connector](./out-of-the-box-connectors/index.md) for more information on how the built-in
mappers map data for each database.

### Custom mappers

The vector store connectors provided by Semantic Kernel support the ability to provide custom mappers in combination with
a `VectorStoreRecordDefinition`. In this case, the `VectorStoreRecordDefinition` can differ from the supplied data model.
The `VectorStoreRecordDefinition` is used to define the database schema, while the data model is used by the developer
to interact with the vector store.
A custom mapper is required in this case to map from the data model to the custom database schema defined by the `VectorStoreRecordDefinition`.

> [!TIP]
> See [How to build a custom mapper for a Vector Store connector](./how-to/vector-store-custom-mapper.md) for an example on how to create your own custom mapper.

::: zone-end
::: zone pivot="programming-language-python"

In order for your data model defined either as a [class](./defining-your-data-model.md) or a [definition](./schema-with-record-definition.md) to be stored in a database, it needs to be serialized to a format that the database can understand.

There are two ways that can be done, either by using the built-in serialization provided by the Semantic Kernel or by providing your own serialization logic.

## Serialization options

### Built-in serialization

The built-in serialization is done by first converting the data model to a dictionary and then serializing it to the model that that store understands, for each store that is different and defined as part of the built-in connector. Deserialization is done in the reverse order.

#### Custom to and from dict methods

The built-in serialization can also use custom methods to go from the data model to a dictionary and from a dictionary to the data model. This can be done by implementing methods from the `VectorStoreModelToDictFromDictProtocol` for a class or functions following the `ToDictProtocol` and `FromDictProtocol` protocols in your record definition, both can be found in `semantic_kernel/data/vector_store_model_protocols.py`.

This is especially useful when you want to use a optimized, container format in your code, but still want to be able to move between stores easily.

#### Pydantic models
When you define you model using a Pydantic BaseModel, it will use the `model_dump` and `model_validate` methods to serialize and deserialize the data model to and from a dict.

### Custom serialization
You can also define the serialization to be done directly from your model into the model of the data store. 

This can be done by implementing the `VectorStoreModelFunctionSerdeProtocol` protocol, or by adding functions that follow the `SerializeProtocol` and `DeserializeProtocol` in your record definition, both can be found in `semantic_kernel/data/vector_store_model_protocols.py`.

## Serialization of vectors

When you have a vector in your data model, it needs to either be a list of floats or list of ints, since that is what most stores need, if you want your class to store the vector in a different format, you can use the `serialize_function` and `deserialize_function` defined in the `VectorStoreRecordVectorField` annotation. For instance for a numpy array you can use the following annotation:

```python
import numpy as np

vector: Annotated[
    np.ndarray | None,
    VectorStoreRecordVectorField(
        dimensions=1536,
        serialize_function=np.ndarray.tolist,
        deserialize_function=np.array,
    ),
] = None
```

If you do use a vector store that can handle native numpy arrays and you don't want to have them converted back and forth, you should setup the direct serialization and deserialization for the model and that store.

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end

