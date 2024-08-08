---
title: The Semantic Kernel Vector Store data architecture (Experimental)
description: Defines the data architecture for Semantic Kernel, including the relationship between vector stores, collections and records.
author: westey-m
ms.topic: conceptual
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# The Semantic Kernel Vector Store data architecture (Experimental)

Vector Store abstractions in Semantic Kernel are based on three main components: vector stores, collections and records.
Records are contained by collections, and collections are contained by vector stores.

- A vector store maps to an instance of a database
- A collection is a collection of records including any index required to query or filter those records
- A record is an individual data entry in the database

## Collections in different databases

The underlying implementation of what a collection is, will vary by connector and is influenced by how each database groups and indexes records.
Most databases have a concept of a collection of records and there is a natural mapping between this concept and the Vector Store abstraction collection.
Note that this concept may not always be referred to as a `collection` in the underlying database.

> [!TIP]
> For more information on what the underlying implementation of a collection is per connector, refer to [the documentation for each connector](./out-of-the-box-connectors/index.md).
