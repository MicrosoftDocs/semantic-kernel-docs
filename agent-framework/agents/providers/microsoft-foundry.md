---
title: Microsoft Foundry
description: Learn how to use Microsoft Agent Framework with Microsoft Foundry project endpoints and the Foundry Agent Service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 07/01/2026
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

## Tools

Foundry agents created from `AIProjectClient.AsAIAgent(...)` (the Responses path) support the standard Agent Framework tool surface — see the [Tools overview](../tools/index.md) for the full list and supported feature matrix. For Foundry agents loaded from a versioned agent definition (`FoundryAgent`), the agent's tools are owned by the Foundry agent definition, not by the client.

| Tool | Notes |
|---|---|
| [Function Tools](../tools/function-tools.md) | Supported. |
| [Tool Approval](../tools/tool-approval.md) | Supported. Provided by the framework's function-invoking chat client. |
| [Code Interpreter](../tools/code-interpreter.md) | Supported. |
| [File Search](../tools/file-search.md) | Supported. |
| [Hosted MCP Tools](../tools/hosted-mcp-tools.md) | Supported. |
| [Local MCP Tools](../tools/local-mcp-tools.md) | Supported. |
| [Foundry Toolboxes](#toolboxes) | Supported. |

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

## Tools

`FoundryChatClient` ships static factory methods for each hosted Foundry tool. The factories return SDK tool objects you pass to `tools=` on `Agent` or directly to `client.get_response(..., tools=[...])`. For `FoundryAgent`, the agent's tools live on the Foundry agent definition itself — see [What works and what doesn't with `FoundryAgent`](#what-works-and-what-doesnt-with-foundryagent).

The factories are class methods, so you do not need an instance to create a tool:

```python
from agent_framework import Agent
from agent_framework.foundry import FoundryChatClient
from azure.identity import AzureCliCredential

agent = Agent(
    client=FoundryChatClient(credential=AzureCliCredential()),
    instructions="You can search the web and run code.",
    tools=[
        FoundryChatClient.get_web_search_tool(),
        FoundryChatClient.get_code_interpreter_tool(),
    ],
)
```

### Tool support

The table below lists every tool the Python `FoundryChatClient` exposes today. `FoundryAgent` works with the same tools, but they must be configured on the Foundry agent definition rather than passed in code.

| Tool | Factory on `FoundryChatClient` | Status | Detail |
|---|---|---|---|
| [Function Tools](../tools/function-tools.md) | n/a — pass any Python callable or `@ai_function` | GA | Invoked locally in your Python process. |
| [Tool Approval](../tools/tool-approval.md) | n/a — wraps existing tools | GA | Works with hosted MCP and function tools. |
| [Code Interpreter](../tools/code-interpreter.md) | `get_code_interpreter_tool` | GA | Sandboxed code execution on Foundry. |
| [File Search](../tools/file-search.md) | `get_file_search_tool` | GA | Search uploaded files via Foundry vector stores. |
| [Web Search](../tools/web-search.md) | `get_web_search_tool` | GA | Bing-backed web grounding managed by Microsoft. Azure OpenAI models only. |
| [Image Generation](#image-generation) | `get_image_generation_tool` | GA | Image generation hosted on Foundry. |
| [Hosted MCP](../tools/hosted-mcp-tools.md) | `get_mcp_tool` | GA | Remote MCP server invoked by Foundry. |
| [Local MCP](../tools/local-mcp-tools.md) | n/a — use `MCPStreamableHTTPTool` / `MCPStdioTool` | GA | Runs in your process; works with any client. |
| [Foundry Toolboxes](#toolboxes) | `MCPStreamableHTTPTool` to the toolbox MCP endpoint | GA | Consumed over MCP from `FoundryChatClient`; attached server-side on `FoundryAgent`. |
| [Bing Grounding](#bing-grounding) | `get_bing_grounding_tool` | Experimental | Bring-your-own Grounding with Bing Search resource. |
| [Bing Custom Search](#bing-custom-search) | `get_bing_custom_search_tool` | Preview | Bing grounding restricted to a curated domain list. |
| [Azure AI Search](#azure-ai-search) | `get_azure_ai_search_tool` | Experimental | Search an Azure AI Search index via a Foundry connection. |
| [SharePoint](#sharepoint) | `get_sharepoint_tool` | Preview | Ground answers in SharePoint content. |
| [Microsoft Fabric](#microsoft-fabric) | `get_fabric_tool` | Preview | Query a Fabric data agent. |
| [Memory Search](#memory-search) | `get_memory_search_tool` | Preview | Search a Foundry-managed memory store. |
| [Computer Use](#computer-use) | `get_computer_use_tool` | Preview | Let the agent drive a desktop or browser environment. |
| [Browser Automation](#browser-automation) | `get_browser_automation_tool` | Preview | Drive a browser via an Azure Playwright connection. |
| [Agent-to-Agent (A2A)](#agent-to-agent-a2a) | `get_a2a_tool` | Preview | Call another A2A agent as a tool. |

> [!NOTE]
> **Experimental** factories wrap GA Foundry SDK types but the wrappers themselves may change before GA. **Preview** factories wrap Foundry SDK types whose underlying capability is in preview and may change or be removed. Both emit an `ExperimentalWarning` the first time they are used in a process.

### Web search variants

Foundry exposes three Bing-backed grounding options. Pick the one that matches your scenario:

- `get_web_search_tool` (GA) — zero-setup default; Bing resource managed by Microsoft. Azure OpenAI models only. Limited to `user_location` and `search_context_size`.
- `get_bing_grounding_tool` (experimental) — bring your own Grounding with Bing Search Azure resource. Supports `count`, `freshness`, `market`, `set_lang`, and non-OpenAI Foundry models.
- `get_bing_custom_search_tool` (preview) — bring your own Bing Custom Search instance to restrict grounding to a curated set of domains.

All three send search data outside the Azure compliance boundary. See the [web grounding overview](/azure/foundry/agents/how-to/tools/web-overview) for the full comparison.

```python
client = FoundryChatClient(credential=AzureCliCredential())

# Default (GA): minimal configuration
web_search = client.get_web_search_tool(
    user_location={"city": "Amsterdam", "country": "NL"},
    search_context_size="medium",
)
```

### Image generation

`get_image_generation_tool` configures Foundry's hosted image generation tool. The model produces image content in the response — there are no extra files to manage.

```python
image_gen = FoundryChatClient.get_image_generation_tool(
    model="gpt-image-1",
    size="1024x1024",
    output_format="png",
    quality="high",
)
```

### Bing grounding

`get_bing_grounding_tool` wraps the Grounding with Bing Search Foundry tool. You create the Grounding with Bing Search resource yourself and add it as a Foundry project connection, then pass the connection ID.

```python
bing = FoundryChatClient.get_bing_grounding_tool(
    connection_id="/subscriptions/.../connections/my-bing",
    market="en-US",
    freshness="Day",
    count=10,
)
```

### Bing custom search

`get_bing_custom_search_tool` restricts grounding to the allow-list defined on a Bing Custom Search resource.

```python
bing_custom = FoundryChatClient.get_bing_custom_search_tool(
    connection_id="/subscriptions/.../connections/my-bing-custom",
    instance_name="docs-only",
    market="en-US",
)
```

### Azure AI Search

`get_azure_ai_search_tool` lets the agent query an Azure AI Search index through a Foundry project connection.

```python
ai_search = FoundryChatClient.get_azure_ai_search_tool(
    index_connection_id="/subscriptions/.../connections/my-search",
    index_name="product-docs",
    query_type="vector_semantic_hybrid",
    top_k=5,
)
```

### SharePoint

`get_sharepoint_tool` grounds answers in SharePoint content reachable through a Foundry SharePoint connection.

```python
sharepoint = FoundryChatClient.get_sharepoint_tool(
    connection_id="/subscriptions/.../connections/my-sharepoint",
)
```

### Microsoft Fabric

`get_fabric_tool` connects the agent to a Microsoft Fabric data agent via a Foundry connection so the agent can answer questions over your Fabric data.

```python
fabric = FoundryChatClient.get_fabric_tool(
    connection_id="/subscriptions/.../connections/my-fabric",
)
```

### Memory search

`get_memory_search_tool` lets the agent search a Foundry-managed memory store, optionally scoped to a user or tenant.

```python
memory = FoundryChatClient.get_memory_search_tool(
    memory_store_name="user-preferences",
    scope="{{$userId}}",
)
```

### Computer use

`get_computer_use_tool` configures the Computer Use preview tool — the model can drive a desktop or browser environment by issuing pointer and keyboard actions.

```python
computer = FoundryChatClient.get_computer_use_tool(
    environment="browser",
    display_width=1280,
    display_height=800,
)
```

### Browser automation

`get_browser_automation_tool` wires the agent into an Azure Playwright Testing resource via a Foundry connection. The agent can drive a real browser through Playwright.

```python
browser = FoundryChatClient.get_browser_automation_tool(
    connection_id="/subscriptions/.../connections/my-playwright",
)
```

### Agent-to-Agent (A2A)

`get_a2a_tool` exposes a remote A2A agent as a tool so a Foundry agent can call it. Provide either a `base_url` (and optionally `agent_card_path`) or a `project_connection_id` for a stored A2A connection.

```python
a2a = FoundryChatClient.get_a2a_tool(
    base_url="https://remote-agent.example.com",
    agent_card_path="/.well-known/agent-card.json",
)
```

For the general A2A guidance — discovery, sessions, streaming — see the [Agent-to-Agent provider page](./agent-to-agent.md).

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

### Setting a custom HTTP timeout

By default, `FoundryAgent` (and `RawFoundryAgent`) inherits the OpenAI SDK's built-in timeout (5 s connect / 600 s total). On multi-turn conversations or slow networks this can surface as a `ConnectTimeout`. Pass `timeout=` (in seconds) at construction time to override it:

```python
from agent_framework.foundry import FoundryAgent
from azure.identity import AzureCliCredential

agent = FoundryAgent(
    project_endpoint="https://your-project.services.ai.azure.com",
    agent_name="my-prompt-agent",
    credential=AzureCliCredential(),
    timeout=120.0,  # seconds; set to None to use the SDK default
)
```

The value is applied via `with_options(timeout=...)` on a per-agent copy of the HTTP client, so it does not affect other agents or clients that share the same `AIProjectClient`.

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
| **FoundryChatClient** (direct inference) | Use `MCPStreamableHTTPTool` against the toolbox's MCP endpoint. |

### Two consumption patterns

| Pattern | Description |
|---|---|
| **Hosted agent attachment** | Tool configs execute on the Foundry runtime. Attach the toolbox to the hosted Foundry agent. |
| **MCP** | Use `MCPStreamableHTTPTool` against the toolbox's MCP endpoint. Works with any chat client, not just `FoundryChatClient`. |

### Direct inference with the toolbox MCP endpoint

For `FoundryChatClient` direct inference, point `MCPStreamableHTTPTool` at the toolbox's MCP endpoint. Use `allowed_tools` on `MCPStreamableHTTPTool` when the toolbox exposes tools that a specific agent should not call.

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
    header_provider=lambda _kwargs: {"Authorization": f"Bearer {token_provider()}"},
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
| [foundry_chat_client_with_toolbox.py](https://github.com/microsoft/agent-framework/tree/main/python/samples/02-agents/providers/foundry/foundry_chat_client_with_toolbox.py) | Toolbox MCP consumption with `MCPStreamableHTTPTool` |
| [foundry_chat_client_with_toolbox_skills.py](https://github.com/microsoft/agent-framework/tree/main/python/samples/02-agents/providers/foundry/foundry_chat_client_with_toolbox_skills.py) | Toolbox-backed skills configuration |
| [invoke_foundry_toolbox_mcp](https://github.com/microsoft/agent-framework/tree/main/python/samples/03-workflows/declarative/invoke_foundry_toolbox_mcp) | MCP consumption path with `MCPStreamableHTTPTool` |

::: zone-end

::: zone pivot="programming-language-go"

## Foundry in Go

The Go SDK provides Microsoft Foundry agents through `github.com/microsoft/agent-framework-go/provider/foundryprovider`.

The package supports two agent targets:

| Target | Go shape | Use when |
|---|---|---|
| Project-backed model deployment | `foundryprovider.ModelDeployment("gpt-4o-mini")` | Your app owns instructions, tools, and conversation flow. |
| Existing server-side Foundry agent | `foundryprovider.ServerAgent("my-agent")` | The agent definition is already configured in Foundry. |

## Configuration

Set your Foundry project endpoint and model deployment:

```bash
FOUNDRY_PROJECT_ENDPOINT="https://<your-project>.services.ai.azure.com/api/projects/<project-id>"
FOUNDRY_MODEL="gpt-4o-mini"
```

## Project-backed Foundry agent

Use `ModelDeployment` when you want to create an Agent Framework agent in code and pass instructions, tools, middleware, and context providers from your Go application.

```go
import (
    "context"
    "os"

    "github.com/Azure/azure-sdk-for-go/sdk/azidentity"
    "github.com/microsoft/agent-framework-go/agent"
    "github.com/microsoft/agent-framework-go/provider/foundryprovider"
)

endpoint := os.Getenv("FOUNDRY_PROJECT_ENDPOINT")
model := os.Getenv("FOUNDRY_MODEL")

token, err := azidentity.NewDefaultAzureCredential(nil)
if err != nil {
    panic(err)
}

a := foundryprovider.NewAgent(
    endpoint,
    token,
    foundryprovider.ModelDeployment(model),
    foundryprovider.AgentConfig{
        Instructions: "You are good at telling jokes.",
        Config: agent.Config{
            Name: "Joker",
        },
    },
)

resp, err := a.RunText(context.Background(), "Tell me a joke about a pirate.").Collect()
```

## Existing server-side Foundry agent

Use `ServerAgent` when you want to invoke an agent already configured in Foundry. The server-side agent owns its instructions and tools, so `AgentConfig.Instructions` is ignored for this target.

```go
a := foundryprovider.NewAgent(
    endpoint,
    token,
    foundryprovider.ServerAgent("my-agent"),
    foundryprovider.AgentConfig{
        Config: agent.Config{
            Name: "my-agent",
        },
    },
)

resp, err := a.RunText(ctx, "Summarize the current project status.").Collect()
```

## Tools

Project-backed Foundry agents support the standard Go Agent Framework tool surface for local tools and supported hosted tool declarations.

| Tool | Status | Notes |
|---|---|---|
| [Function Tools](../tools/function-tools.md) | Supported | Functions run in your Go process. |
| [Tool Approval](../tools/tool-approval.md) | Supported | Works with local function tools through the tool auto-call loop. |
| [Code Interpreter](../tools/code-interpreter.md) | Supported | Use `&hostedtool.CodeInterpreter{}`. |
| [Web Search](../tools/web-search.md) | Supported | Use `&hostedtool.WebSearch{}`. |
| [Local MCP Tools](../tools/local-mcp-tools.md) | Supported | Use `tool/mcptool` to connect to an MCP server and expose its tools locally. |
| [Hosted MCP Tools](../tools/hosted-mcp-tools.md) | Not currently documented for Go Foundry | Use local MCP tools when you need MCP servers with Go Foundry agents. |
| Foundry Toolboxes | Not currently exposed through a Go helper. |

For local function tools, add `tool.Tool` values through `agent.Config.Tools`:

```go
a := foundryprovider.NewAgent(
    endpoint,
    token,
    foundryprovider.ModelDeployment(model),
    foundryprovider.AgentConfig{
        Instructions: "You are a helpful assistant.",
        Config: agent.Config{
            Tools: []tool.Tool{weatherTool},
        },
    },
)
```

For hosted code execution, pass the hosted tool declaration:

```go
a := foundryprovider.NewAgent(
    endpoint,
    token,
    foundryprovider.ModelDeployment(model),
    foundryprovider.AgentConfig{
        Instructions: "You solve problems with code.",
        Config: agent.Config{
            Tools: []tool.Tool{&hostedtool.CodeInterpreter{}},
        },
    },
)
```

## Client headers and served model

Foundry accepts `x-client-*` headers per run. Add them with `foundryprovider.WithClientHeader` or `foundryprovider.WithClientHeaders`:

```go
resp, err := a.RunText(
    ctx,
    "Hello!",
    foundryprovider.WithClientHeader("x-client-scenario", "docs"),
).Collect()
```

When Foundry returns the `x-ms-served-model` response header, the Go provider adds it to response/update additional properties as `ServedModel`.

```go
if servedModel, ok := resp.AdditionalProperties["ServedModel"].(string); ok {
    fmt.Println(servedModel)
}
```

## Foundry memory provider

Use `foundryprovider.NewMemoryProvider` when you want an Agent Framework agent to retrieve from and update a Foundry-managed memory store around each run.

```go
import (
    "log/slog"

    "github.com/microsoft/agent-framework-go/agent"
    "github.com/microsoft/agent-framework-go/provider/foundryprovider"
)

memoryProvider := foundryprovider.NewMemoryProvider(
    endpoint,
    tokenCredential,
    "memory-store-sample",
    func(*agent.Session) string { return "user-123" },
    foundryprovider.MemoryProviderConfig{
        Logger: slog.Default(),
    },
)

a := foundryprovider.NewAgent(
    endpoint,
    tokenCredential,
    foundryprovider.ModelDeployment(model),
    foundryprovider.AgentConfig{
        Instructions: "Use known memories about the user when responding.",
        Config: agent.Config{
            Name:             "FoundryMemoryAgent",
            ContextProviders: []agent.ContextProvider{memoryProvider},
        },
    },
)
```

The endpoint must be a project-scoped Microsoft Foundry endpoint, and the memory store must already exist in that project. The scope callback should return a stable user, tenant, or conversation partition key.

> [!TIP]
> See the [Foundry memory Go sample](https://github.com/microsoft/agent-framework-go/blob/main/examples/02-agents/agents/step22_foundry_memory/main.go) for a complete runnable example.

## Current Go gaps

Go support does not currently include Foundry hosted deployment/lifecycle/admin APIs, embeddings clients, or Go-specific helpers for Foundry Toolboxes. Use the Foundry portal or service SDKs for those operations.

::: zone-end
## Next steps

> [!div class="nextstepaction"]
> [Foundry Local](./foundry-local.md)
