---
title: Migrating from Memory Stores to Vector Stores
description: Describes how to migrate from using memory stores to vector stores in semantic kernel.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.service: semantic-kernel
---
::: zone pivot="programming-language-csharp"

# Migrating from Memory Stores to Vector Stores

Semantic Kernel provides two distinct abstractions for interacting with vector stores.

1. A set of legacy abstractions where the primary interface is `Microsoft.SemanticKernel.Memory.IMemoryStore`.
2. A new and improved set of abstractions where the primary interface is `Microsoft.Extensions.VectorData.IVectorStore`.

The Vector Store abstractions provide more functionality than what the Memory Store abstractions provide, e.g. being
able to define your own schema, supporting multiple vectors per record (database permitting), supporting more
vector types than `ReadOnlyMemory<float>`, etc. We recommend using the Vector Store abstractions instead of the
Memory Store abstractions.

> [!TIP]
> For a more detailed comparison of the Memory Store and Vector Store abstractions see [here](../../concepts/vector-store-connectors/memory-stores.md#memory-store-vs-vector-store-abstractions).

## Migrating from Memory Stores to Vector Stores

See the [Legacy Semantic Kernel Memory Stores](../../concepts/vector-store-connectors/memory-stores.md#migrating-from-memory-stores-to-vector-stores) page for instructions on how to migrate.

::: zone-end
::: zone pivot="programming-language-python"

## Coming soon

More info coming soon.

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More info coming soon.

::: zone-end
