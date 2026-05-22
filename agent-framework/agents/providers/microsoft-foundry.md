---
title: Microsoft Foundry
description: Learn how to use Microsoft Agent Framework with Microsoft Foundry project endpoints and the Foundry Agent Service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 04/02/2026
ms.service: agent-framework
---

# Microsoft Foundry

Microsoft Agent Framework supports both direct model inference from Microsoft Foundry project endpoints and service-managed agents in the [Foundry Agent Service](/azure/ai-foundry/agents/overview).

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```dotnetcli
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.Foundry --prerelease
```

## Two agent types

The Microsoft Foundry integration exposes two distinct usage patterns:

| Type | Produced type | Description | Use when |
|---|---|---|---|
| **Responses Agent** | `ChatClientAgent` | Your app programmatically provides a model, instructions, and tools at runtime via `AIProjectClient.AsAIAgent(...)`. No server-side agent resource is created. | You own the agent definition and want a simple, flexible setup. This is the pattern used in most samples. |
| **Foundry Agent** (versioned) | `FoundryAgent` | Server-managed — agent definitions are created and versioned either through the Foundry portal or programmatically via `AIProjectClient.AgentAdministrationClient`. Pass a `ProjectsAgentVersion` or `ProjectsAgentRecord` or `AgentReference` to `AIProjectClient.AsAIAgent(...)`. | You need strict, versioned agent definitions managed in the Foundry portal, through service APIs |

## Responses Agent (direct inference)

Use `AsAIAgent` on `AIProjectClient` directly with a model and instructions. This is the recommended starting point for most scenarios.

```csharp
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;

AIAgent agent = new AIProjectClient(
    new Uri("<your-foundry-project-endpoint>"),
    new DefaultAzureCredential())
        .AsAIAgent(
            model: "gpt-4o-mini",
            name: "Joker",
            instructions: "You are good at telling jokes.");

Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

This path is code-first and does not create a server-managed agent resource.

## Foundry Agent (versioned)

Use the native `AIProjectClient.AgentAdministrationClient` APIs from the AI Projects SDK to retrieve versioned agent resources, then wrap them with `AsAIAgent`. Agents can be created and configured directly in the Foundry portal or programmatically via `AIProjectClient.AgentAdministrationClient`.

```csharp
using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry;

var aiProjectClient = new AIProjectClient(
    new Uri("<your-foundry-project-endpoint>"),
    new DefaultAzureCredential());

// Retrieve an existing agent by name (uses the latest version automatically)
ProjectsAgentRecord jokerRecord = await aiProjectClient.AgentAdministrationClient.GetAgentAsync("Joker");
FoundryAgent agent = aiProjectClient.AsAIAgent(jokerRecord);

Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));
```

> [!IMPORTANT]
> Foundry Agents tools and instructions are strict to the ones it was created with, attempting to modify tooling or instructions at runtime is not supported.

## Using the agent

Both `ChatClientAgent` (Responses) and `FoundryAgent` (versioned) are standard `AIAgent` instances and support all standard operations including sessions, tools, middleware, and streaming.

```csharp
AgentSession session = await agent.CreateSessionAsync();
Console.WriteLine(await agent.RunAsync("Tell me a joke.", session));
Console.WriteLine(await agent.RunAsync("Now make it funnier.", session));
```

For more information on how to run and interact with agents, see the [Agent getting started tutorials](../../get-started/your-first-agent.md).

## Toolboxes

> [!NOTE]
> Foundry Toolbox .NET docs are coming soon.

::: zone-end
::: zone pivot="programming-language-python"

## Foundry in Python

In Python, all Foundry-specific clients now live under `agent_framework.foundry`.

- `agent-framework-foundry` provides the cloud Foundry connectors: `FoundryChatClient`, `FoundryAgent`, `FoundryEmbeddingClient`, and `FoundryMemoryProvider`.
- `agent-framework-foundry-local` provides `FoundryLocalClient` for local model execution.

> [!IMPORTANT]
> This page covers the current Python clients for Microsoft Foundry project endpoints, models endpoints, and the Foundry Agent Service. If you have a standalone Azure OpenAI resource endpoint (`https://<your-resource>.openai.azure.com`), use the Python guidance on the [OpenAI provider page](./openai.md). If you want to run supported models locally, see the [Foundry Local provider page](./foundry-local.md).

## Foundry chat and agent patterns in Python

| Scenario | Python shape | Use when |
|---|---|---|
| Plain inference with the Foundry Responses endpoint | `Agent(client=FoundryChatClient(...))` | Your app owns the agent definition, tools, and conversation loop, and you want a model deployed in a Foundry project. |
| Service-managed agents in the Foundry Agent Service | `FoundryAgent(...)` | You want to connect to a PromptAgent or HostedAgent that is created and configured in the Foundry portal or through the service APIs. |

## Installation

```bash
pip install agent-framework-foundry
pip install azure-identity
```

The same `agent-framework-foundry` package also includes `FoundryEmbeddingClient` for Foundry models-endpoint embeddings.

## Configuration

### `FoundryChatClient`

```bash
FOUNDRY_PROJECT_ENDPOINT="https://<your-project>.services.ai.azure.com"
FOUNDRY_MODEL="gpt-4o-mini"
```

### `FoundryAgent`

```bash
FOUNDRY_PROJECT_ENDPOINT="https://<your-project>.services.ai.azure.com"
FOUNDRY_AGENT_NAME="my-agent"
FOUNDRY_AGENT_VERSION="1.0"
```

Use `FOUNDRY_AGENT_VERSION` for Prompt Agents. Hosted agents can omit it.

### `FoundryEmbeddingClient`

```bash
FOUNDRY_MODELS_ENDPOINT="https://<apim-instance>.azure-api.net/<foundry-instance>/models"
FOUNDRY_MODELS_API_KEY="<api-key>"
FOUNDRY_EMBEDDING_MODEL="text-embedding-3-small"
FOUNDRY_IMAGE_EMBEDDING_MODEL="Cohere-embed-v3-english"  # optional
```

`FoundryChatClient` and `FoundryAgent` use the project endpoint. `FoundryEmbeddingClient` uses the separate models endpoint.

### Choose the right Python client

| Scenario | Preferred client | Notes |
|---|---|---|
| Azure OpenAI resource | `OpenAIChatCompletionClient` / `OpenAIChatClient` | Use the [OpenAI provider page](./openai.md). |
| Microsoft Foundry project inference | `Agent(client=FoundryChatClient(...))` | Uses the Foundry Responses endpoint. |
| Microsoft Foundry service-managed agent | `FoundryAgent` | Recommended for Prompt Agents and HostedAgents. |
| Microsoft Foundry models-endpoint embeddings | `FoundryEmbeddingClient` | Uses `FOUNDRY_MODELS_ENDPOINT` plus `FOUNDRY_EMBEDDING_MODEL` / `FOUNDRY_IMAGE_EMBEDDING_MODEL`. |
| Foundry Local runtime | `Agent(client=FoundryLocalClient(...))` | See [Foundry Local](./foundry-local.md). |

## Create an agent with `FoundryChatClient`

`FoundryChatClient` connects to a deployed model in a Foundry project and uses the Responses endpoint. Pair it with a standard `Agent` when your app should own instructions, tools, and session handling.

```python
from agent_framework import Agent
from agent_framework.foundry import FoundryChatClient
from azure.identity import AzureCliCredential

agent = Agent(
    client=FoundryChatClient(
        project_endpoint="https://your-project.services.ai.azure.com",
        model="gpt-4o-mini",
        credential=AzureCliCredential(),
    ),
    name="FoundryWeatherAgent",
    instructions="You are a helpful assistant.",
)
```

`FoundryChatClient` is the Foundry-first Python path for direct inference and supports tools, structured outputs, and streaming.

## Create embeddings with `FoundryEmbeddingClient`

Use `FoundryEmbeddingClient` when you want text or image embeddings from a Foundry models endpoint.

```python
from agent_framework.foundry import FoundryEmbeddingClient

async with FoundryEmbeddingClient() as client:
    result = await client.get_embeddings(["hello from Agent Framework"])
    print(result[0].dimensions)
```

## Connect to a service-managed agent with `FoundryAgent`

Use `FoundryAgent` when the agent definition lives in Foundry. This is the recommended Python API for Prompt Agents and HostedAgents.

```python
from agent_framework.foundry import FoundryAgent
from azure.identity import AzureCliCredential

agent = FoundryAgent(
    project_endpoint="https://your-project.services.ai.azure.com",
    agent_name="my-prompt-agent",
    agent_version="1.0",
    credential=AzureCliCredential(),
)
```

For a HostedAgent, omit `agent_version` and use the hosted agent name instead.

### What works and what doesn't with `FoundryAgent`

`FoundryAgent` connects to an agent that already exists in Foundry (a Prompt Agent or a Hosted Agent). The agent's definition — its instructions and its tool configuration — lives in Foundry, not in your Python code. This means several `Agent`-level features behave differently than they do with `Agent(client=FoundryChatClient(...))` or other chat-client–backed agents.

#### Tools

| Tool type passed to `FoundryAgent(...)` | Behavior |
|---|---|
| `FunctionTool` (a local Python callable) | **Supported, but only if the matching function definition already exists on the Foundry agent.** The Foundry runtime decides which tools to expose to the model based on the agent definition. When the model calls a function, Foundry returns a tool call to the client and the framework invokes your local Python callable **in your process** (not in Foundry), then sends the result back. Passing a `FunctionTool` client-side just supplies that local implementation — if the function is not declared on the Foundry agent, the model will never call it. |
| Hosted tools (web search, code interpreter, file search, MCP, image generation, etc.) | **Ignored.** These must be configured on the Foundry agent definition itself, either in the Foundry portal or via the service APIs. Passing them client-side has no effect because the Foundry runtime only knows about tools attached to the agent definition. |

In short: **you cannot add new tools at construction time.** Every tool the model can call — including local Python functions — must already be part of the agent definition in Foundry. Passing a `FunctionTool` to `FoundryAgent(...)` only provides the local implementation that runs in your Python process when the Foundry-defined function is called; it does not register a new tool with the agent.

#### Context providers

`context_providers=[...]` is partially supported. Whether a context provider works depends on *what* the provider tries to do:

| Context provider behavior | Works with `FoundryAgent`? |
|---|---|
| Adds extra context as messages (for example, retrieved memory, RAG snippets, user profile information) | **Yes.** The injected context is forwarded with the request. |
| Persists or observes the conversation (for example, writing turns to an external store) | **Yes.** Runs locally around the request/response. |
| Adds tools dynamically (for example, `SkillsProvider`, or any provider that returns tools from `invoking()`) | **No, unless the tools are already part of the Foundry agent definition.** The Foundry runtime executes the model against the tools attached to the agent in Foundry; tools that only exist locally are not exposed to the model and will not be invoked. |

If you need dynamic tool selection, skill loading, or any other behavior that relies on tools being added at runtime, use `Agent(client=FoundryChatClient(...))` instead — that path owns the model loop locally and supports the full set of tool types and tool-adding context providers.

#### Run options (`default_options` and `agent.run(...)` options)

Options you pass to `FoundryAgent(default_options=...)` or to `agent.run(..., **options)` (such as `temperature`, `top_p`, `max_tokens`, `instructions`, `tool_choice`, `response_format`, `metadata`, etc.) are **not all honored**. Because the agent definition in Foundry is the source of truth, many options are silently ignored.

For **Prompt Agents**, the framework explicitly removes or overrides the following before sending the request to the Foundry Responses API:

| Option | Behavior with `FoundryAgent` |
|---|---|
| `model` | **Ignored.** The model is taken from the Foundry agent definition. |
| `tools`, `tool_choice`, `parallel_tool_calls` | **Stripped from the request body.** Tools must be declared on the Foundry agent definition (see the previous section). `FunctionTool` callables are still wired up locally for function invocation, but the tool list itself is not sent to the service. |
| `instructions` and system/developer messages | **Ignored.** The Foundry agent's own instructions are authoritative. System/developer messages are stripped from the message list before the request is sent. |
| `conversation_id` | **Used**, and mapped to the Foundry agent session when it refers to one. |
| `extra_body` | **Forwarded**, merged with the framework-set `agent_reference` payload. |
| Sampling parameters (`temperature`, `top_p`, `max_tokens`, `seed`, `frequency_penalty`, `presence_penalty`, `stop`, …), `metadata`, `user`, `store`, `response_format`, etc. | **Forwarded** to the Responses API. Whether Foundry actually applies them depends on the agent and model configuration — the agent definition can override or constrain them — so do not rely on them taking effect for a Prompt Agent. |

For **Hosted Agents**, the same client-side stripping applies, but everything beyond that depends on what the specific hosted agent implements. A hosted agent may accept, ignore, or reinterpret any option that is forwarded. Treat run-time options as advisory and verify the actual behavior against the hosted agent you are calling.

> [!TIP]
> If you need precise control over generation parameters, instructions, or tool selection per run, configure them on the Foundry agent definition, or switch to `Agent(client=FoundryChatClient(...))`, which honors `ChatOptions` end-to-end.

> [!TIP]
> A good rule of thumb: if a feature depends on changing the agent's instructions or tools per run, it belongs on `Agent(client=FoundryChatClient(...))`. If the agent's definition is fixed in Foundry and you only need local function invocation plus message-level context, `FoundryAgent` is the right choice.

### Connecting to a deployed (hosted) Foundry agent

For HostedAgents that run service-side sessions (`/agents/{name}/sessions`), use `FoundryAgent` with `allow_preview=True` to opt into the preview Responses surface:

```python
from agent_framework.foundry import FoundryAgent
from azure.identity import AzureCliCredential

agent = FoundryAgent(
    agent_name="my-hosted-agent",
    credential=AzureCliCredential(),
    allow_preview=True,
)
```

When you need to manage the underlying service session yourself — for example to bind a session to a specific tenant or user — create the session through the preview `AIProjectClient` API and wrap it with `agent.get_session(...)`:

```python
from azure.ai.projects.aio import AIProjectClient
from azure.ai.projects.models import VersionRefIndicator

service_session = await project_client.beta.agents.create_session(
    agent_name="my-hosted-agent",
    isolation_key="user-123",
    version_indicator=VersionRefIndicator(agent_version="1.0"),
)
session = agent.get_session(service_session.agent_session_id)

response = await agent.run("Hello!", session=session)
```

> [!TIP]
> See the [`using_deployed_agent.py` sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/04-hosting/foundry-hosted-agents/responses/using_deployed_agent.py) for a complete example, including resolving the latest version automatically.

> [!WARNING]
> The older Python `AzureAIClient`, `AzureAIProjectAgentProvider`, `AzureAIAgentClient`, `AzureAIAgentsProvider`, and Azure AI embedding compatibility surfaces were removed from the current `agent_framework.azure` namespace. For current Python code, use `FoundryChatClient` when your app owns instructions and tools, `FoundryAgent` when the agent definition lives in Foundry, and `FoundryEmbeddingClient` for Foundry models-endpoint embeddings.

## Using the agent

Both `FoundryChatClient` and `FoundryAgent` integrate with the standard Python `Agent` experience, including tool calling, sessions, and streaming responses. For local runtimes, use the separate [Foundry Local provider page](./foundry-local.md).

## Toolboxes

> [!IMPORTANT]
> Toolbox APIs are experimental. The surface may change in future releases.

A **Foundry toolbox** is a named, versioned server-side bundle of hosted tool configurations (code interpreter, file search, image generation, MCP, web search) configured in a Microsoft Foundry project. Toolboxes let you manage tool configuration once in the Foundry portal and reuse it across agents.

Agent Framework covers **consumption** only — creating and updating toolbox versions is done through the Foundry portal or the raw `azure-ai-projects` SDK (`azure-ai-projects>=2.1.0`).

### FoundryAgent vs FoundryChatClient

| Agent type | Toolbox behavior |
|---|---|
| **FoundryAgent** (hosted) | Toolbox attachment happens server-side. No client-side wiring is required. |
| **FoundryChatClient** (direct inference) | Fetch the toolbox with `get_toolbox()` and pass it as `tools=`. |

### Two consumption patterns

| Pattern | Description |
|---|---|
| **Native (hosted tools)** | Tool configs execute on the Foundry runtime. Pass the toolbox directly as `tools=`. |
| **MCP** | Use `MCPStreamableHTTPTool` against the toolbox's MCP endpoint. Works with any chat client, not just `FoundryChatClient`. |

### Fetching a toolbox

Use `FoundryChatClient.get_toolbox()` to retrieve a toolbox:

```python
from agent_framework import Agent
from agent_framework.foundry import FoundryChatClient
from azure.identity.aio import AzureCliCredential

async with AzureCliCredential() as credential:
    client = FoundryChatClient(credential=credential)
    toolbox = await client.get_toolbox("research_toolbox")

    async with Agent(client=client, name="ResearchAgent", tools=toolbox) as agent:
        result = await agent.run("Summarize recent findings.")
        print(result.text)
```

When `version` is omitted, `get_toolbox` resolves the default version in two requests. Pin a specific version to avoid the extra round trip:

```python
toolbox = await client.get_toolbox("research_toolbox", version="v3")
```

> [!NOTE]
> Each `get_toolbox()` call hits the network — there is no framework-side cache, because default versions can change server-side. Caching is caller-owned.

### Implicit flattening

You do not need to write `toolbox.tools`. The framework's `normalize_tools` recognizes `ToolboxVersionObject` and flattens automatically. All of these work:

```python
# Single toolbox
agent = Agent(client=client, tools=toolbox)

# Toolbox in a list
agent = Agent(client=client, tools=[toolbox])

# Mix local function tools with a toolbox
agent = Agent(client=client, tools=[get_internal_metrics, toolbox])

# Combine multiple toolboxes
agent = Agent(client=client, tools=[toolbox_a, toolbox_b])
```

### Filtering tools with `select_toolbox_tools`

If your toolbox bundles several tools but an agent only needs a subset, use `select_toolbox_tools` to narrow the set after fetching. This avoids sending unnecessary tool definitions to the model, which reduces token usage and prevents the model from invoking tools you do not intend to expose:

```python
from agent_framework.foundry import select_toolbox_tools, get_toolbox_tool_name

# Filter by tool name
tools = select_toolbox_tools(toolbox, include_names=["web_search", "code_interpreter"])

# Filter by tool type
tools = select_toolbox_tools(toolbox, include_types=["mcp", "web_search"])

# Filter with a custom predicate
tools = select_toolbox_tools(toolbox, predicate=lambda t: "search" in (get_toolbox_tool_name(t) or ""))
```

Helper functions `get_toolbox_tool_name(tool)` and `get_toolbox_tool_type(tool)` return the selection name and raw type of a tool entry, respectively. `FoundryHostedToolType` is a `TypeAlias` (`Literal["code_interpreter", "file_search", "image_generation", "mcp", "web_search"] | str`) for IDE-guided completion on `include_types` / `exclude_types`.

### MCP consumption path

You can also consume a toolbox as an MCP server by pointing `MCPStreamableHTTPTool` at the toolbox's MCP endpoint URL.

The MCP endpoint URL is shown on the Foundry Portal or follows the format:

`https://<account>.services.ai.azure.com/api/projects/<project>/toolsets/<name>/mcp?api-version=v1`

Because the client connects to the Foundry toolbox endpoint directly, you must authenticate with an Entra ID bearer token via `header_provider`:

```python
from azure.identity.aio import DefaultAzureCredential
from azure.identity.aio import get_bearer_token_provider
from agent_framework import Agent, MCPStreamableHTTPTool

credential = DefaultAzureCredential()
token_provider = get_bearer_token_provider(credential, "https://ai.azure.com/.default")

mcp_tool = MCPStreamableHTTPTool(
    name="research_mcp",
    url="https://<your-toolbox-mcp-endpoint>",
    header_provider=lambda: {"Authorization": f"Bearer {token_provider()}"},
)

async with Agent(client=client, name="MCPAgent", tools=[mcp_tool]) as agent:
    result = await agent.run("Search for recent papers on LLM agents.")
    print(result.text)
```

### Limitations

- **MCP tools inside a toolbox use server-side authentication.** Authentication to the upstream MCP server is handled via `project_connection_id` (an OAuth connection configured in the Foundry project). The client never holds bearer tokens for the upstream server.
- **Consuming a toolbox as an MCP server requires client-side authentication.** When you point `MCPStreamableHTTPTool` at a toolbox's MCP endpoint, you must supply an Entra ID bearer token (for example, via `get_bearer_token_provider(credential, "https://ai.azure.com/.default")`) through `header_provider`.
- **Consent-flow handling is a runtime concern.** If a toolbox MCP tool triggers `CONSENT_REQUIRED` during `agent.run()`, it is handled at run time, not during toolbox fetch.

### Samples

| Sample | Description |
|---|---|
| [foundry_chat_client_with_toolbox.py](https://github.com/microsoft/agent-framework/tree/main/python/samples/02-agents/providers/foundry/foundry_chat_client_with_toolbox.py) | Basic toolbox fetch, version pinning, combining toolboxes, and filtering |
| [foundry_chat_client_with_toolbox_mcp.py](https://github.com/microsoft/agent-framework/tree/main/python/samples/02-agents/providers/foundry/foundry_chat_client_with_toolbox_mcp.py) | MCP consumption path with `MCPStreamableHTTPTool` |
| [foundry_toolbox_context_provider.py](https://github.com/microsoft/agent-framework/tree/main/python/samples/02-agents/context_providers/foundry_toolbox_context_provider.py) | Dynamic per-turn tool selection via a context provider |

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Foundry Local](./foundry-local.md)
