---
title: Foundry Local
description: Learn how to run Microsoft Foundry models locally with Agent Framework and Foundry Local.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: reference
ms.author: edvan
ms.date: 03/25/2026
ms.service: agent-framework
---

# Foundry Local

Foundry Local lets you run supported Microsoft Foundry models on your local machine while still using the standard Agent Framework Python `Agent` experience.

::: zone pivot="programming-language-csharp"

> [!NOTE]
> Foundry Local is not currently supported in .NET.

::: zone-end

::: zone pivot="programming-language-python"

## Prerequisites

Install Foundry Local and its local runtime components before running Agent Framework against a local model. The first run can take a while because the selected model may need to be downloaded and loaded.

## Installation

```bash
pip install agent-framework-foundry-local --pre
```

## Configuration

Set the default local model with:

```bash
FOUNDRY_LOCAL_MODEL="phi-4-mini"
```

You can also pass the model explicitly with `FoundryLocalClient(model="phi-4-mini")`.

> [!NOTE]
> `FoundryLocalClient` lives in the `agent_framework.foundry` namespace. It is a local chat client, so you typically pair it with a standard `Agent`.

## Create a local agent

```python
import asyncio

from agent_framework import Agent
from agent_framework.foundry import FoundryLocalClient

async def main():
    agent = Agent(
        client=FoundryLocalClient(model="phi-4-mini"),
        name="LocalAgent",
        instructions="You are a helpful local assistant.",
    )
    result = await agent.run("What's the weather like in Seattle?")
    print(result)

asyncio.run(main())
```

## Model capabilities

Not every local model supports the same features. Function calling and structured output depend on the selected model. The `FoundryLocalClient.manager` helper can be used to inspect the local catalog and supported capabilities before you run an agent.

For additional runtime controls, `FoundryLocalClient` also supports options such as `device`, `bootstrap`, and `prepare_model`.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Microsoft Foundry Provider](./microsoft-foundry.md)
