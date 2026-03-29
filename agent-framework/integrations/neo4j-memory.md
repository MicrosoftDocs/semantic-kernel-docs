---
title: Neo4j Memory Provider for Agent Framework
description: Learn how to use the Neo4j Memory Provider to add persistent knowledge graph memory to your Agent Framework agents
zone_pivot_groups: programming-languages
author: retroryan
ms.topic: article
ms.author: ryanknight
ms.date: 03/29/2026
ms.service: agent-framework
---

# Neo4j Memory Provider

The Neo4j Memory Provider gives Agent Framework agents persistent memory backed by a knowledge graph. Unlike RAG providers that retrieve from static knowledge bases, the memory provider stores and recalls agent interactions, automatically extracting entities and building a knowledge graph over time.

The provider manages three types of memory:

- **Short-term memory**: Conversation history and recent context
- **Long-term memory**: Entities, preferences, and facts extracted from interactions
- **Reasoning memory**: Past reasoning traces and tool usage patterns

## Why use Neo4j for agent memory?

- **Knowledge graph persistence**: Memories are stored as connected entities, not flat records, so the agent can reason about relationships between things it remembers.
- **Automatic entity extraction**: Conversations are parsed into structured entities and relationships without manual schema design.
- **Cross-session recall**: Preferences, facts, and reasoning traces persist across sessions and surface automatically via context providers.

> [!NOTE]
> Neo4j offers two separate integrations for Agent Framework. This provider (`neo4j-agent-memory`) is for **persistent memory** — storing and recalling agent interactions, extracting entities, and building a knowledge graph over time. For **GraphRAG** from an existing knowledge graph using vector, fulltext, or hybrid search, see the [Neo4j GraphRAG Context Provider](./neo4j-graphrag.md).

::: zone pivot="programming-language-csharp"

This provider is not yet available for C#. See the Python tab for usage examples.

::: zone-end

::: zone pivot="programming-language-python"

## Prerequisites

- A Neo4j instance (self-hosted or [Neo4j AuraDB](https://neo4j.com/cloud/aura/))
- An Azure AI Foundry project with a deployed chat model
- An OpenAI API key or Azure OpenAI deployment (for embeddings and entity extraction)
- Environment variables set: `NEO4J_URI`, `NEO4J_PASSWORD`, `AZURE_AI_PROJECT_ENDPOINT`
- Azure CLI credentials configured (`az login`)
- Python 3.10 or later

## Installation

```bash
pip install neo4j-agent-memory[microsoft-agent]
```

## Usage

```python
import os
from pydantic import SecretStr
from agent_framework import Agent
from agent_framework.azure import AzureAIClient
from azure.identity.aio import AzureCliCredential
from neo4j_agent_memory import MemoryClient, MemorySettings
from neo4j_agent_memory.integrations.microsoft_agent import (
    Neo4jMicrosoftMemory,
    create_memory_tools,
)

# MemorySettings accepts nested configuration for Neo4j, embedding, and LLM providers.
# Environment variables use the NAM_ prefix with __ as nested delimiter
# (e.g. NAM_NEO4J__URI, NAM_NEO4J__PASSWORD, NAM_EMBEDDING__MODEL).
settings = MemorySettings(
    neo4j={
        "uri": os.environ["NEO4J_URI"],
        "username": os.environ.get("NEO4J_USERNAME", "neo4j"),
        "password": SecretStr(os.environ["NEO4J_PASSWORD"]),
    },
    embedding={
        "provider": "openai",
        "model": "text-embedding-3-small",
    },
)

memory_client = MemoryClient(settings)

async with memory_client:
    memory = Neo4jMicrosoftMemory.from_memory_client(
        memory_client=memory_client,
        session_id="user-123",
    )
    tools = create_memory_tools(memory)

    async with (
        AzureAIClient(
            credential=AzureCliCredential(),
            project_endpoint=os.environ["AZURE_AI_PROJECT_ENDPOINT"],
        ) as client,
        Agent(
            client=client,
            instructions="You are a helpful assistant with persistent memory.",
            tools=tools,
            context_providers=[memory.context_provider],
        ) as agent,
    ):
        session = agent.create_session()
        response = await agent.run("Remember that I prefer window seats on flights.", session=session)
```

## Key features

- **Bidirectional**: Automatically retrieves relevant context before invocation and saves new memories after responses
- **Entity extraction**: Builds a knowledge graph from conversations using a multi-stage extraction pipeline
- **Preference learning**: Infers and stores user preferences across sessions
- **Memory tools**: Agents can explicitly search memory, remember preferences, and find entity connections

## Resources

- [Neo4j Agent Memory repository](https://github.com/neo4j-labs/agent-memory)
- [PyPI package page](https://pypi.org/project/neo4j-agent-memory/)
- [Sample: Retail Assistant with Neo4j Agent Memory](https://github.com/neo4j-labs/agent-memory/tree/main/examples/microsoft_agent_retail_assistant)
- [Workshop: Neo4j Context Providers for Agent Framework](https://github.com/neo4j-partners/maf-context-providers-lab)

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Neo4j GraphRAG Context Provider](./neo4j-graphrag.md)
