---
title: Give agents access to OpenAPI APIs
description: Learn how to add plugins from OpenAPI specifications to your agents in Semantic Kernel.
author: sophialagerkranspandey
ms.topic: conceptual
ms.author: sopand
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Add plugins from OpenAPI specifications

Often in an enterprise, you already have a set of APIs that perform real work. These could be used by other automation services or power front-end applications that humans interact with. In Semantic Kernel, you can add these exact same APIs as plugins so your agents can also use them.

## An example OpenAPI specification

Take for example an API that allows you to alter the 