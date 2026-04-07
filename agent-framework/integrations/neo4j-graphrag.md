---
title: Neo4j GraphRAG Context Provider for Agent Framework
description: Learn how to use the Neo4j GraphRAG Context Provider to add knowledge graph retrieval capabilities to your Agent Framework agents
zone_pivot_groups: programming-languages
author: retroryan
ms.topic: article
ms.author: westey
ms.date: 04/01/2026
ms.service: agent-framework
---

# Neo4j GraphRAG Context Provider

The Neo4j GraphRAG Context Provider adds Retrieval Augmented Generation (RAG) capabilities to Agent Framework agents using a Neo4j knowledge graph. It supports vector, fulltext, and hybrid search modes, with optional graph traversal to enrich results with related entities via custom Cypher queries.

For knowledge graph scenarios where relationships between entities matter, this provider retrieves relevant subgraphs rather than isolated text chunks, giving agents richer context for generating responses.

## Why use Neo4j for GraphRAG?

- **Graph enhanced retrieval**: Standard vector search returns isolated chunks; graph traversal follows connections to surface related entities, giving agents richer context.
- **Flexible search modes**: Combine vector similarity, keyword/BM25, and graph traversal in a single query.
- **Custom retrieval queries**: Cypher queries let you control exactly which relationships to traverse and what context to return.

> [!NOTE]
> Neo4j offers two separate integrations for Agent Framework. This provider is for **GraphRAG** — searching an existing knowledge graph to ground agent responses. For **persistent memory** that learns from conversations and builds a knowledge graph over time, see the [Neo4j Memory Provider](./neo4j-memory.md).

::: zone pivot="programming-language-csharp"

## Prerequisites

- A Neo4j instance (self-hosted or [Neo4j AuraDB](https://neo4j.com/cloud/aura/)) with a vector or fulltext index configured
- An Azure AI Foundry project with a deployed chat model and an embedding model (e.g. `text-embedding-3-small`)
- Environment variables set: `NEO4J_URI`, `NEO4J_USERNAME`, `NEO4J_PASSWORD`, `AZURE_AI_SERVICES_ENDPOINT`, `AZURE_AI_EMBEDDING_NAME`
- Azure CLI credentials configured (`az login`)
- .NET 8.0 or later

## Installation

```bash
dotnet add package Neo4j.AgentFramework.GraphRAG
```

## Usage

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.OpenAI;
using Microsoft.Extensions.AI;
using Neo4j.AgentFramework.GraphRAG;
using Neo4j.Driver;

// Read connection details from environment variables
var neo4jSettings = new Neo4jSettings();
var azureEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_SERVICES_ENDPOINT")!;

// Create embedding generator
var credential = new DefaultAzureCredential();
var azureClient = new AzureOpenAIClient(new Uri(azureEndpoint), credential);

IEmbeddingGenerator<string, Embedding<float>> embedder = azureClient
    .GetEmbeddingClient("text-embedding-3-small")
    .AsIEmbeddingGenerator();

// Create Neo4j driver
await using var driver = GraphDatabase.Driver(
    neo4jSettings.Uri, AuthTokens.Basic(neo4jSettings.Username, neo4jSettings.Password!));

// Create the Neo4j context provider
await using var provider = new Neo4jContextProvider(driver, new Neo4jContextProviderOptions
{
    IndexName = "chunkEmbeddings",
    IndexType = IndexType.Vector,
    EmbeddingGenerator = embedder,
    TopK = 5,
    RetrievalQuery = """
        MATCH (node)-[:FROM_DOCUMENT]->(doc:Document)
        OPTIONAL MATCH (doc)<-[:FILED]-(company:Company)
        RETURN node.text AS text, score, doc.title AS title, company.name AS company
        ORDER BY score DESC
        """,
});

// Create an agent with the provider
AIAgent agent = azureClient
    .GetChatClient("gpt-4o")
    .AsIChatClient()
    .AsBuilder()
    .UseAIContextProviders(provider)
    .BuildAIAgent(new ChatClientAgentOptions
    {
        ChatOptions = new ChatOptions
        {
            Instructions = "You are a financial analyst assistant.",
        },
    });

var session = await agent.CreateSessionAsync();
Console.WriteLine(await agent.RunAsync("What risks does Acme Corp face?", session));
```

## Key features

- **Index-driven**: Works with any Neo4j vector or fulltext index
- **Graph traversal**: Custom Cypher queries enrich search results with related entities
- **Search modes**: Vector (semantic similarity), fulltext (keyword/BM25), or hybrid (both combined)

## Resources

- [Neo4j Context Provider repository](https://github.com/neo4j-labs/neo4j-maf-provider)
- [NuGet package page](https://www.nuget.org/packages/Neo4j.AgentFramework.GraphRAG)
- [Workshop: Neo4j Context Providers for Agent Framework](https://github.com/neo4j-partners/maf-context-providers-lab)

::: zone-end

::: zone pivot="programming-language-python"

## Prerequisites

- A Neo4j instance (self-hosted or [Neo4j AuraDB](https://neo4j.com/cloud/aura/)) with a vector or fulltext index configured
- An Azure AI Foundry project with a deployed chat model and an embedding model (e.g. `text-embedding-ada-002`)
- Environment variables set: `NEO4J_URI`, `NEO4J_USERNAME`, `NEO4J_PASSWORD`, `FOUNDRY_PROJECT_ENDPOINT`, `FOUNDRY_MODEL`, `AZURE_AI_EMBEDDING_NAME`
- Azure CLI credentials configured (`az login`)
- Python 3.10 or later

## Installation

```bash
pip install agent-framework-neo4j
```

## Usage

```python
import os

from agent_framework import Agent
from agent_framework.foundry import FoundryChatClient
from agent_framework_neo4j import Neo4jContextProvider, Neo4jSettings, AzureAISettings, AzureAIEmbedder
from azure.identity import DefaultAzureCredential
from azure.identity.aio import AzureCliCredential

# Reads NEO4J_URI, NEO4J_USERNAME, NEO4J_PASSWORD from environment variables
neo4j_settings = Neo4jSettings()

# Reads FOUNDRY_PROJECT_ENDPOINT, AZURE_AI_EMBEDDING_NAME from environment variables
azure_settings = AzureAISettings()

sync_credential = DefaultAzureCredential()
embedder = AzureAIEmbedder(
    endpoint=azure_settings.inference_endpoint,
    credential=sync_credential,
    model=azure_settings.embedding_model,
)

neo4j_provider = Neo4jContextProvider(
    uri=neo4j_settings.uri,
    username=neo4j_settings.username,
    password=neo4j_settings.get_password(),
    index_name=neo4j_settings.vector_index_name,
    index_type="vector",
    embedder=embedder,
    top_k=5,
    retrieval_query="""
        MATCH (node)-[:FROM_DOCUMENT]->(doc:Document)
        OPTIONAL MATCH (doc)<-[:FILED]-(company:Company)
        RETURN node.text AS text, score, doc.title AS title, company.name AS company
        ORDER BY score DESC
    """,
)

async with (
    neo4j_provider,
    AzureCliCredential() as credential,
    Agent(
        client=FoundryChatClient(
            credential=credential,
            project_endpoint=azure_settings.project_endpoint,
            model=os.environ["FOUNDRY_MODEL"],
        ),
        instructions="You are a financial analyst assistant.",
        context_providers=[neo4j_provider],
    ) as agent,
):
    session = agent.create_session()
    response = await agent.run("What risks does Acme Corp face?", session=session)
```

## Key features

- **Index-driven**: Works with any Neo4j vector or fulltext index
- **Graph traversal**: Custom Cypher queries enrich search results with related entities
- **Search modes**: Vector (semantic similarity), fulltext (keyword/BM25), or hybrid (both combined)

## Resources

- [Neo4j Context Provider repository](https://github.com/neo4j-labs/neo4j-maf-provider)
- [PyPI package page](https://pypi.org/project/agent-framework-neo4j/)
- [Workshop: Neo4j Context Providers for Agent Framework](https://github.com/neo4j-partners/maf-context-providers-lab)

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Neo4j Memory Provider](./neo4j-memory.md)
