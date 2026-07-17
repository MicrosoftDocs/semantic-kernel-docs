---
title: Neo4j Memory Provider for Agent Framework
description: Learn how to use the Neo4j Memory Provider to add persistent knowledge graph memory to your Agent Framework agents
zone_pivot_groups: programming-languages
author: retroryan
ms.topic: article
ms.author: westey
ms.date: 07/16/2026
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

> [!NOTE]
> The .NET package (`AgentMemory`) is an independent, community-maintained .NET port of the Neo4j Labs memory provider — it is not an official Neo4j Labs package. See the [AgentMemory (.NET) repository](https://github.com/joslat/agent-memory-dotnet) for source and details.

## Prerequisites

- A Neo4j instance (self-hosted or [Neo4j AuraDB](https://neo4j.com/cloud/aura/))
- An Azure OpenAI or Microsoft Foundry deployment (a chat model + an embedding model)
- Environment variables set: `NEO4J_URI`, `NEO4J_USERNAME`, `NEO4J_PASSWORD`, `AZURE_OPENAI_ENDPOINT`
- Azure CLI credentials configured (`az login`), or an API key
- .NET 8.0 or later

## Installation

```bash
dotnet add package AgentMemory
dotnet add package AgentMemory.AgentFramework
```

## Usage

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AgentMemory;
using AgentMemory.Abstractions.Services;
using AgentMemory.AgentFramework;
using AgentMemory.AgentFramework.Tools;

var builder = Host.CreateApplicationBuilder(args);

// Registers Core + Neo4j infrastructure in one call (reads NEO4J_URI / NEO4J_USERNAME /
// NEO4J_PASSWORD, falling back to local-dev defaults). Passing configureLlm opts in to
// LLM-backed entity/fact/preference extraction, using the IChatClient registered below.
builder.Services.AddNeo4jAgentMemory(
    configureMemory: _ => { },
    configureNeo4j: neo4j =>
    {
        neo4j.Uri = Environment.GetEnvironmentVariable("NEO4J_URI") ?? "bolt://localhost:7687";
        neo4j.Username = Environment.GetEnvironmentVariable("NEO4J_USERNAME") ?? "neo4j";
        neo4j.Password = Environment.GetEnvironmentVariable("NEO4J_PASSWORD") ?? "password";
    },
    configureLlm: _ => { });

// Any Microsoft.Extensions.AI-compatible chat + embedding client works
var azureClient = new AzureOpenAIClient(
    new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!), new DefaultAzureCredential());
builder.Services.AddSingleton(azureClient.GetChatClient("gpt-4o-mini").AsIChatClient());
builder.Services.AddSingleton(azureClient.GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator());

// AutoExtractOnPersist builds the knowledge graph from every conversation turn
builder.Services.AddAgentMemoryFramework(options =>
{
    options.AutoExtractOnPersist = true;
    options.ContextFormat.IncludeEntities = true;
    options.ContextFormat.IncludeFacts = true;
    options.ContextFormat.IncludePreferences = true;
});

using var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();
var services = scope.ServiceProvider;

// Bootstraps Neo4j schema/indexes on first run (idempotent)
await services.GetRequiredService<ISchemaBootstrapper>().BootstrapAsync();

var memoryProvider = services.GetRequiredService<Neo4jMemoryContextProvider>();
var memoryTools = services.GetRequiredService<MemoryToolFactory>().CreateAIFunctions();

// WithMemoryOwnerScoping wraps the whole invocation — recall, the tool-calling loop, and
// persistence — in the owner scope set by WithMemoryIdentity below, so no manual
// BeginOwnerScope call is needed around RunAsync.
AIAgent agent = services.GetRequiredService<IChatClient>().AsAIAgent(new ChatClientAgentOptions
{
    ChatOptions = new ChatOptions
    {
        Instructions = "You are a helpful assistant with persistent memory.",
        Tools = [.. memoryTools],
    },
    AIContextProviders = [memoryProvider],
}).WithMemoryOwnerScoping(services);

var session = (await agent.CreateSessionAsync())
    .WithMemoryIdentity(userId: "user-123", sessionId: "session-1", applicationId: "my-app");

var response = await agent.RunAsync("Remember that I prefer window seats on flights.", session);
```

## Key features

- **Bidirectional**: `Neo4jMemoryContextProvider` recalls relevant memory before each run and persists new memory after — no manual wiring needed
- **Entity extraction**: builds a knowledge graph from conversations with a configurable extraction pipeline (`AutoExtractOnPersist`)
- **Preference learning**: infers and stores user preferences, facts, and entities, recalled automatically by a brand-new `AgentSession` for the same user
- **Memory tools**: `MemoryToolFactory` exposes `AIFunction`s so the model can explicitly search, remember, and recall
- **Dependency-injection first**: registers via `AddNeo4jAgentMemory` (wires up Core + Neo4j internally) and `AddAgentMemoryFramework`, fitting naturally into Generic Host and ASP.NET Core apps
- **Beyond Agent Framework**: the same library also integrates with Semantic Kernel and MCP clients, and includes built-in OpenTelemetry observability

## Resources

- [AgentMemory (.NET) repository](https://github.com/joslat/agent-memory-dotnet)
- [NuGet package page](https://www.nuget.org/packages/AgentMemory)
- [Sample: Retail Assistant with AgentMemory](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/02-agents/AgentWithMemory/AgentWithMemory_Step06_MemoryUsingAgentMemory)

::: zone-end

::: zone pivot="programming-language-python"

## Prerequisites

- A Neo4j instance (self-hosted or [Neo4j AuraDB](https://neo4j.com/cloud/aura/))
- An Azure AI Foundry project with a deployed chat model
- An OpenAI API key or Azure OpenAI deployment (for embeddings and entity extraction)
- Environment variables set: `NEO4J_URI`, `NEO4J_PASSWORD`, `FOUNDRY_PROJECT_ENDPOINT`, `FOUNDRY_MODEL`, `OPENAI_API_KEY`
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
from agent_framework.foundry import FoundryChatClient
from azure.identity.aio import AzureCliCredential
from neo4j_agent_memory import MemoryClient, MemorySettings
from neo4j_agent_memory.integrations.microsoft_agent import (
    Neo4jMicrosoftMemory,
    create_memory_tools,
)

# Pass Neo4j and embedding configuration directly via constructor arguments.
# MemorySettings also supports loading from environment variables or .env files
# using the NAM_ prefix (e.g. NAM_NEO4J__URI, NAM_EMBEDDING__MODEL).
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

    async with AzureCliCredential() as credential, Agent(
            client=FoundryChatClient(
            credential=credential,
            project_endpoint=os.environ["FOUNDRY_PROJECT_ENDPOINT"],
            model=os.environ["FOUNDRY_MODEL"],
        ),
        instructions="You are a helpful assistant with persistent memory.",
        tools=tools,
        context_providers=[memory.context_provider],
    ) as agent:
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

::: zone pivot="programming-language-go"

> [!NOTE]
> Go support for this feature is coming soon. See the [Agent Framework Go repository](https://github.com/microsoft/agent-framework-go) for the latest status.

::: zone-end
## Next steps

> [!div class="nextstepaction"]
> [Neo4j GraphRAG Context Provider](./neo4j-graphrag.md)
