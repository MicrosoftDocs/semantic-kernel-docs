---
title: Python 2026 Significant Changes Guide
description: Guide to significant changes in Python releases for Microsoft Agent Framework in 2026, including breaking changes and important enhancements.
author: eavanvalkenburg
ms.topic: upgrade-and-migration-article
ms.author: edvan
ms.date: 03/30/2026
ms.service: agent-framework
---
# Python 2026 Significant Changes Guide

This document lists all significant changes in Python releases since the start of 2026, including breaking changes and important enhancements that may affect your code. Each change is marked as:

- 🔴 **Breaking** — Requires code changes to upgrade
- 🟡 **Enhancement** — New capability or improvement; existing code continues to work

This document will be removed once we reach the 1.0.0 stable release, so please refer to it when upgrading between versions in 2026 to ensure you don't miss any important changes. For detailed upgrade instructions on specific topics (e.g., options migration), refer to the linked upgrade guides or the linked PR's.

---

## python-1.0.0rc6

This section captures the significant Python changes currently being tracked for `python-1.0.0rc6`.

### 🔴 Provider-leading client design and package split

**PR:** [#4818](https://github.com/microsoft/agent-framework/pull/4818)

PR `#4818` reorganizes the Python provider surface around provider-specific packages and namespaces.

- OpenAI clients now live in the `agent-framework-openai` package, while still importing from the `agent_framework.openai` namespace.
- Microsoft Foundry clients now live in the `agent-framework-foundry` package and the `agent_framework.foundry` namespace.
- Foundry Local is also exposed from `agent_framework.foundry` as `FoundryLocalClient`.
- `OpenAIResponsesClient` is renamed to `OpenAIChatClient`.
- `OpenAIChatClient` is renamed to `OpenAIChatCompletionClient`.
- Client configuration is standardized on `model`, replacing older parameters such as `model_id`, `deployment_name`, and `model_deployment_name`.
- For new Azure OpenAI code, use the `agent_framework.openai` clients. The older `AzureOpenAI*` classes still exist under `agent_framework.azure` as deprecated compatibility shims.
- For new Foundry code, use `FoundryChatClient` for direct project inference, `FoundryAgent` for Prompt Agents and HostedAgents, and `FoundryLocalClient` for local runtimes.
- `AzureAIClient`, `AzureAIProjectAgentProvider`, `AzureAIAgentClient`, `AzureAIAgentsProvider`, and `OpenAIAssistantsClient` now sit on deprecated or compatibility paths. `AzureAIAgentClient` and related classes targets the v1 Agent Service surface and will be removed before GA.
- Sample coverage was reorganized to match the new provider-leading layout, including Foundry samples under `samples/02-agents/providers/foundry/`.

### Package mapping

| Scenario | Install | Primary namespace |
|---|---|---|
| OpenAI and Azure OpenAI | `pip install agent-framework-openai --pre` | `agent_framework.openai` |
| Microsoft Foundry project endpoints and Agent Service | `pip install agent-framework-foundry --pre` | `agent_framework.foundry` |
| Foundry Local | `pip install agent-framework-foundry-local --pre` | `agent_framework.foundry` |
| Legacy/lower-level Azure AI clients | `pip install agent-framework-azure-ai --pre` | `agent_framework.azure` |

**Before:**
```python
from agent_framework.openai import OpenAIResponsesClient

client = OpenAIResponsesClient(model_id="gpt-5.4")
```

**After:**
```python
from agent_framework.openai import OpenAIChatClient

client = OpenAIChatClient(model="gpt-5.4")
```

If you previously used Azure OpenAI directly, map the old dedicated classes to the new provider-leading OpenAI classes:

- `AzureOpenAIResponsesClient` → `OpenAIChatClient`
- `AzureOpenAIChatClient` → `OpenAIChatCompletionClient`
- `AzureOpenAIEmbeddingClient` → `OpenAIEmbeddingClient`
- `AzureOpenAIAssistantsClient` → `OpenAIAssistantsClient` or `OpenAIAssistantProvider`

The code change is mostly a class-name move plus `deployment_name` → `model`. For Azure OpenAI compatibility, use explicit Azure inputs on the new OpenAI clients. `credential=` is now the preferred Azure auth surface, while a callable `api_key` remains a compatibility path:

**Before (`AzureOpenAIResponsesClient`):**
```python
from agent_framework.azure import AzureOpenAIResponsesClient

client = AzureOpenAIResponsesClient(
    endpoint=azure_endpoint,
    deployment_name=deployment_name,
    credential=credential,
)
```

**After (`OpenAIChatClient`):**
```python
from agent_framework.openai import OpenAIChatClient
from azure.identity import AzureCliCredential

api_version = "your-azure-openai-api-version"

client = OpenAIChatClient(
    azure_endpoint=azure_endpoint,
    model=deployment_name,
    credential=AzureCliCredential(),
    api_version=api_version,
)
```

**Before (`AzureOpenAIChatClient`):**
```python
from agent_framework.azure import AzureOpenAIChatClient

client = AzureOpenAIChatClient(
    endpoint=azure_endpoint,
    deployment_name=deployment_name,
    credential=credential,
)
```

**After (`OpenAIChatCompletionClient`):**
```python
from agent_framework.openai import OpenAIChatCompletionClient
from azure.identity import AzureCliCredential

api_version = "your-azure-openai-api-version"

client = OpenAIChatCompletionClient(
    azure_endpoint=azure_endpoint,
    model=deployment_name,
    credential=AzureCliCredential(),
    api_version=api_version,
)
```

If you want to move from Azure OpenAI endpoints to a Microsoft Foundry project endpoint, use the Foundry-oriented surface instead:

**Before (Azure OpenAI endpoint):**
```python
from agent_framework.azure import AzureOpenAIResponsesClient
from azure.identity import AzureCliCredential

client = AzureOpenAIResponsesClient(
    deployment_name="gpt-4.1",
    credential=AzureCliCredential(),
)
```

**After (Foundry project):**
```python
from agent_framework import Agent
from agent_framework.foundry import FoundryChatClient
from azure.identity import AzureCliCredential

client = FoundryChatClient(
    project_endpoint="https://your-project.services.ai.azure.com",
    model="gpt-4.1",
    credential=AzureCliCredential(),
)

agent = Agent(client=client)
```

For local Microsoft Foundry runtimes, use the Foundry namespace plus the local connector:

```python
from agent_framework.foundry import FoundryLocalClient

client = FoundryLocalClient(model="phi-4-mini")
```

If you omit `model`, set `FOUNDRY_LOCAL_MODEL` in your environment.

Also update environment/configuration names where applicable:

- `OPENAI_RESPONSES_MODEL_ID` / `OPENAI_CHAT_MODEL_ID` → `OPENAI_MODEL`
- Azure OpenAI deployment names now map to the `model` argument on `OpenAIChatClient` / `OpenAIChatCompletionClient`
- Use `azure_endpoint` for Azure OpenAI resource URLs, or `base_url` if you already have a full `.../openai/v1` URL, and set `api_version` for the Azure OpenAI API surface you are using
- Adopt Foundry-specific settings such as `FOUNDRY_PROJECT_ENDPOINT`, `FOUNDRY_MODEL`, `FOUNDRY_AGENT_NAME`, and `FOUNDRY_AGENT_VERSION` for cloud Foundry clients
- Use `FOUNDRY_LOCAL_MODEL` for Foundry Local

This change is currently tracked under `python-1.0.0rc6`; add the release-notes link once the corresponding Python release is published.

---

### 🔴 Core dependencies are now intentionally slim

**PR:** [#4904](https://github.com/microsoft/agent-framework/pull/4904)

PR `#4904` follows the provider package split from `#4818` by slimming down `agent-framework-core` and removing more transitive provider dependencies from the core package.

- `agent-framework-core` is now intentionally minimal.
- If you import `agent_framework.openai`, install `agent-framework-openai --pre`.
- If you import Azure compatibility or lower-level Azure AI clients from `agent_framework.azure`, install `agent-framework-azure-ai --pre`.
- If you use MCP tools, `Agent.as_mcp_server()`, or other MCP integrations on a minimal install, install `mcp --pre` manually. For WebSocket MCP support, install `mcp[ws] --pre`.
- If you want the broad "everything included" experience, install the meta package `agent-framework --pre`.

This does **not** redesign the provider surface again; it changes what is installed by default when you only bring in core.

**Before (core-only installs often brought in more provider functionality transitively):**
```bash
pip install agent-framework-core --pre
```

**After (install the provider package you actually use):**
```bash
pip install agent-framework-core --pre
pip install agent-framework-openai --pre
```

or:

```bash
pip install agent-framework-core --pre
pip install agent-framework-azure-ai --pre
```

If you upgrade an existing project that previously depended on core plus lazy provider imports, audit your imports and make the provider packages explicit in your environment or dependency files. Do the same for MCP dependencies if you rely on MCP tools or MCP server hosting.

---

### 🔴 Generic OpenAI clients now prefer explicit routing signals

**PR:** [#4925](https://github.com/microsoft/agent-framework/pull/4925)

PR `#4925` changes how the generic `agent_framework.openai` clients decide between OpenAI and Azure OpenAI.

- Generic OpenAI clients no longer switch to Azure just because `AZURE_OPENAI_*` environment variables are present.
- If `OPENAI_API_KEY` is configured, the generic clients stay on OpenAI unless you pass an explicit Azure routing signal such as `credential` or `azure_endpoint`.
- If only `AZURE_OPENAI_*` settings are present, the generic clients can still fall back to Azure environment-based routing.
- The preferred Azure OpenAI pattern is now to pass explicit Azure settings plus `credential=AzureCliCredential()` on `OpenAIChatClient`, `OpenAIChatCompletionClient`, and the embedding client.
- Deprecated `AzureOpenAI*` wrappers preserve their compatibility behavior, so existing wrapper-based code does not follow the new generic-client precedence rules.

**Before (`OpenAIChatClient` could route to Azure because Azure env vars were present):**
```python
import os
from agent_framework.openai import OpenAIChatClient

os.environ["OPENAI_API_KEY"] = "sk-openai"
os.environ["AZURE_OPENAI_ENDPOINT"] = "https://your-resource.openai.azure.com"
os.environ["AZURE_OPENAI_DEPLOYMENT_NAME"] = "gpt-4o-mini"

client = OpenAIChatClient(model="gpt-4o-mini")
```

**After (generic OpenAI stays on OpenAI; pass explicit Azure inputs to force Azure routing):**
```python
import os
from agent_framework.openai import OpenAIChatClient
from azure.identity import AzureCliCredential

client = OpenAIChatClient(
    model=os.environ["AZURE_OPENAI_DEPLOYMENT_NAME"],
    azure_endpoint=os.environ["AZURE_OPENAI_ENDPOINT"],
    api_version=os.getenv("AZURE_OPENAI_API_VERSION"),
    credential=AzureCliCredential(),
)
```

If your environment contains both `OPENAI_*` and `AZURE_OPENAI_*` values, audit any generic `agent_framework.openai` client construction and make the provider choice explicit. The Azure provider samples were updated to pass Azure inputs directly for this reason.

Azure embeddings now follow the same routing model:

```python
import os
from agent_framework.openai import OpenAIEmbeddingClient
from azure.identity import AzureCliCredential

client = OpenAIEmbeddingClient(
    model=os.environ["AZURE_OPENAI_EMBEDDING_DEPLOYMENT_NAME"],
    azure_endpoint=os.environ["AZURE_OPENAI_ENDPOINT"],
    api_version=os.getenv("AZURE_OPENAI_API_VERSION"),
    credential=AzureCliCredential(),
)
```

For embedding scenarios, map:

- `AzureOpenAIEmbeddingClient` → `OpenAIEmbeddingClient`
- `AZURE_OPENAI_EMBEDDING_DEPLOYMENT_NAME` → `model`
- `OPENAI_EMBEDDING_MODEL` remains the OpenAI-side embedding environment variable

---

## python-1.0.0rc5 / python-1.0.0b260319 (March 19, 2026)

### 🔴 Chat client pipeline reordered: FunctionInvocation now wraps ChatMiddleware

**PR:** [#4746](https://github.com/microsoft/agent-framework/pull/4746)

The ChatClient pipeline ordering has changed. `FunctionInvocation` is now the outermost layer and wraps `ChatMiddleware`, which means chat middleware runs **per model call** (including each iteration of the tool calling loop) instead of once around the entire function invocation sequence.

**Old pipeline order:**
```
ChatMiddleware → FunctionInvocation → RawChatClient
```

**New pipeline order:**
```
FunctionInvocation → ChatMiddleware → ChatTelemetry → RawChatClient
```

If you have custom chat middleware that assumed it ran only once per agent invocation (wrapping the entire tool calling loop), update it to be safe for repeated execution. Chat middleware is now invoked for each individual LLM request, including requests that send tool results back to the model.

Additionally, `ChatTelemetry` is now a separate layer from `ChatMiddleware` in the pipeline, running closest to `RawChatClient`.

### 🔴 Public runtime kwargs split into explicit buckets

**PR:** [#4581](https://github.com/microsoft/agent-framework/pull/4581)

Public Python agent and chat APIs no longer treat blanket public `**kwargs` forwarding as the primary runtime-data mechanism. Runtime values are now split by purpose:

- Use `function_invocation_kwargs` for values that only tools or function middleware should see.
- Use `client_kwargs` for client-layer kwargs and client middleware configuration.
- Access tool/runtime data through `FunctionInvocationContext` (`ctx.kwargs` and `ctx.session`).
- Define tools with an injected context parameter instead of `**kwargs`; injected context parameters are not shown in the schema the model sees.
- When delegating to a sub-agent as a tool, use `agent.as_tool(propagate_session=True)` if the child agent must share the caller's session.

**Before:**
```python
from typing import Any

from agent_framework import tool


@tool
def send_email(address: str, **kwargs: Any) -> str:
    return f"Queued email for {kwargs['user_id']}"


response = await agent.run(
    "Send the update to finance@example.com",
    user_id="user-123",
    request_id="req-789",
)
```

**After:**
```python
from agent_framework import FunctionInvocationContext, tool


@tool
def send_email(address: str, ctx: FunctionInvocationContext) -> str:
    user_id = ctx.kwargs["user_id"]
    session_id = ctx.session.session_id if ctx.session else "no-session"
    return f"Queued email for {user_id} in {session_id}"


response = await agent.run(
    "Send the update to finance@example.com",
    session=agent.create_session(),
    function_invocation_kwargs={
        "user_id": "user-123",
        "request_id": "req-789",
    },
)
```

If you implement custom public `run()` or `get_response()` methods, add `function_invocation_kwargs` and `client_kwargs` to those signatures. For tools, prefer a parameter annotated as `FunctionInvocationContext` — it can be named `ctx`, `context`, or any other annotated name. If you provide an explicit schema/input model, a plain unannotated parameter named `ctx` is also recognized. The same context object is available to function middleware, and it is where runtime function kwargs and session state now live. Tool definitions that still rely on `**kwargs` only use a legacy compatibility path and will be removed.

---

## python-1.0.0rc4 / python-1.0.0b260311 (March 11, 2026)

**Release Notes:** [python-1.0.0rc4](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc4)

### 🔴 Azure AI integrations now target `azure-ai-projects` 2.0 GA

**PR:** [#4536](https://github.com/microsoft/agent-framework/pull/4536)

The Python Azure AI integrations now assume the GA 2.0 `azure-ai-projects` surface.

- The supported dependency range is now `azure-ai-projects>=2.0.0,<3.0`.
- `foundry_features` passthrough was removed from Azure AI agent creation.
- Preview behavior now uses `allow_preview=True` on the supported clients/providers.
- Mixed beta/GA compatibility shims were removed, so update any imports and type names to the 2.0 GA SDK surface.

---

### 🔴 GitHub Copilot tool handlers now use `ToolInvocation` / `ToolResult` and Python 3.11+

**PR:** [#4551](https://github.com/microsoft/agent-framework/pull/4551)

`agent-framework-github-copilot` now tracks `github-copilot-sdk>=0.1.32`.

- Tool handlers receive a `ToolInvocation` dataclass instead of a raw `dict`.
- Return `ToolResult` using snake_case fields such as `result_type` and `text_result_for_llm`.
- The `agent-framework-github-copilot` package now requires Python 3.11+.

**Before:**
```python
from typing import Any


def handle_tool(invocation: dict[str, Any]) -> dict[str, Any]:
    args = invocation.get("arguments", {})
    return {
        "resultType": "success",
        "textResultForLlm": f"Handled {args.get('city', 'request')}",
    }
```

**After:**
```python
from copilot.types import ToolInvocation, ToolResult


def handle_tool(invocation: ToolInvocation) -> ToolResult:
    args = invocation.arguments
    return ToolResult(
        result_type="success",
        text_result_for_llm=f"Handled {args.get('city', 'request')}",
    )
```

---

## python-1.0.0rc3 / python-1.0.0b260304 (March 4, 2026)

**Release Notes:** [python-1.0.0rc3](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc3)

### 🔴 Skills provider finalized around code-defined `Skill` / `SkillResource`

**PR:** [#4387](https://github.com/microsoft/agent-framework/pull/4387)

Python Agent Skills now support code-defined `Skill` and `SkillResource` objects alongside file-based skills, and the public provider surface is standardized on `SkillsProvider`.

- If you still import the older preview/internal `FileAgentSkillsProvider`, switch to `SkillsProvider`.
- File-based resource lookup no longer relies on backtick-quoted references in `SKILL.md`; resources are discovered from the skill directory instead.

If you had preview/internal code that imported `FileAgentSkillsProvider`, switch to the current public surface:

```python
from agent_framework import Skill, SkillResource, SkillsProvider
```

---

## python-1.0.0rc2 / python-1.0.0b260226 (February 26, 2026)

**Release Notes:** [python-1.0.0rc2](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc2)

### 🔴 Declarative workflows replace `InvokeTool` with `InvokeFunctionTool`

**PR:** [#3716](https://github.com/microsoft/agent-framework/pull/3716)

Declarative Python workflows no longer use the old `InvokeTool` action kind. Replace it with `InvokeFunctionTool` and register Python callables with `WorkflowFactory.register_tool()`.

**Before:**
```yaml
actions:
  - kind: InvokeTool
    toolName: send_email
```

**After:**
```python
factory = WorkflowFactory().register_tool("send_email", send_email)
```

```yaml
actions:
  - kind: InvokeFunctionTool
    functionName: send_email
```

---

## python-1.0.0rc1 / python-1.0.0b260219 (February 19, 2026)

**Release:** `agent-framework-core` and `agent-framework-azure-ai` promoted to `1.0.0rc1`. All other packages updated to `1.0.0b260219`.

### 🔴 Unified Azure credential handling across all packages

**PR:** [#4088](https://github.com/microsoft/agent-framework/pull/4088)

The `ad_token`, `ad_token_provider`, and `get_entra_auth_token` parameters/helpers have been replaced with a unified `credential` parameter across all Azure-related Python packages. The new approach uses `azure.identity.get_bearer_token_provider` for automatic token caching and refresh.

**Affected classes:** `AzureOpenAIChatClient`, `AzureOpenAIResponsesClient`, `AzureOpenAIAssistantsClient`, `AzureAIClient`, `AzureAIAgentClient`, `AzureAIProjectAgentProvider`, `AzureAIAgentsProvider`, `AzureAISearchContextProvider`, `PurviewClient`, `PurviewPolicyMiddleware`, `PurviewChatPolicyMiddleware`.

**Before:**
```python
from azure.identity import AzureCliCredential, get_bearer_token_provider

token_provider = get_bearer_token_provider(
    AzureCliCredential(), "https://cognitiveservices.azure.com/.default"
)

client = AzureOpenAIResponsesClient(
    azure_ad_token_provider=token_provider,
    ...
)
```

**After:**
```python
from azure.identity import AzureCliCredential

client = AzureOpenAIResponsesClient(
    credential=AzureCliCredential(),
    ...
)
```

The `credential` parameter accepts `TokenCredential`, `AsyncTokenCredential`, or a callable token provider. Token caching and refresh are handled automatically.

---

### 🔴 Redesigned Python exception hierarchy

**PR:** [#4082](https://github.com/microsoft/agent-framework/pull/4082)

The flat `ServiceException` family has been replaced with domain-scoped exception branches under a single `AgentFrameworkException` root. This gives callers precise `except` targets and clear error semantics.

**New hierarchy:**

```
AgentFrameworkException
├── AgentException
│   ├── AgentInvalidAuthException
│   ├── AgentInvalidRequestException
│   ├── AgentInvalidResponseException
│   └── AgentContentFilterException
├── ChatClientException
│   ├── ChatClientInvalidAuthException
│   ├── ChatClientInvalidRequestException
│   ├── ChatClientInvalidResponseException
│   └── ChatClientContentFilterException
├── IntegrationException
│   ├── IntegrationInitializationError
│   ├── IntegrationInvalidAuthException
│   ├── IntegrationInvalidRequestException
│   ├── IntegrationInvalidResponseException
│   └── IntegrationContentFilterException
├── ContentError
├── WorkflowException
│   ├── WorkflowRunnerException
│   ├── WorkflowValidationError
│   └── WorkflowActionError
├── ToolExecutionException
├── MiddlewareTermination
└── SettingNotFoundError
```

**Removed exceptions:** `ServiceException`, `ServiceInitializationError`, `ServiceResponseException`, `ServiceContentFilterException`, `ServiceInvalidAuthError`, `ServiceInvalidExecutionSettingsError`, `ServiceInvalidRequestError`, `ServiceInvalidResponseError`, `AgentExecutionException`, `AgentInvocationError`, `AgentInitializationError`, `AgentSessionException`, `ChatClientInitializationError`, `CheckpointDecodingError`.

**Before:**
```python
from agent_framework.exceptions import ServiceException, ServiceResponseException

try:
    result = await agent.run("Hello")
except ServiceResponseException:
    ...
except ServiceException:
    ...
```

**After:**
```python
from agent_framework.exceptions import AgentException, AgentInvalidResponseException, AgentFrameworkException

try:
    result = await agent.run("Hello")
except AgentInvalidResponseException:
    ...
except AgentException:
    ...
except AgentFrameworkException:
    # catch-all for any Agent Framework error
    ...
```

> [!NOTE]
> Init validation errors now use built-in `ValueError`/`TypeError` instead of custom exceptions. Agent Framework exceptions are reserved for domain-level failures.

---

### 🔴 Provider state scoped by `source_id`

**PR:** [#3995](https://github.com/microsoft/agent-framework/pull/3995)

Provider hooks now receive a provider-scoped state dictionary (`state.setdefault(provider.source_id, {})`) instead of the full session state. This means provider implementations that previously accessed nested state via `state[self.source_id]["key"]` must now access `state["key"]` directly.

Additionally, `InMemoryHistoryProvider` default `source_id` changed from `"memory"` to `"in_memory"`.

**Before:**
```python
# In a custom provider hook:
async def on_before_agent(self, state: dict, **kwargs):
    my_data = state[self.source_id]["my_key"]

# InMemoryHistoryProvider default source_id
provider = InMemoryHistoryProvider("memory")
```

**After:**
```python
# Provider hooks receive scoped state — no nested access needed:
async def on_before_agent(self, state: dict, **kwargs):
    my_data = state["my_key"]

# InMemoryHistoryProvider default source_id changed
provider = InMemoryHistoryProvider("in_memory")
```

---

### 🔴 Chat/agent message typing alignment (`run` vs `get_response`)

**PR:** [#3920](https://github.com/microsoft/agent-framework/pull/3920)

Chat-client `get_response` implementations now consistently receive `Sequence[Message]`.
`agent.run(...)` remains flexible (`str`, `Content`, `Message`, or sequences of those), and normalizes inputs before calling chat clients.

**Before:**
```python
async def get_response(self, messages: str | Message | list[Message], **kwargs): ...
```

**After:**
```python
from collections.abc import Sequence
from agent_framework import Message

async def get_response(self, messages: Sequence[Message], **kwargs): ...
```

---

### 🔴 `FunctionTool[Any]` generic setup removed for schema passthrough

**PR:** [#3907](https://github.com/microsoft/agent-framework/pull/3907)

Schema-based tool paths no longer rely on the previous `FunctionTool[Any]` generic behavior.
Use `FunctionTool` directly and supply either a pydantic BaseModel or explicit schemas where needed (for example, with `@tool(schema=...)`).

**Before:**
```python
placeholder: FunctionTool[Any] = FunctionTool(...)
```

**After:**
```python
placeholder: FunctionTool = FunctionTool(...)
```

---

### 🔴 Pydantic Settings replaced with `TypedDict` + `load_settings()`

**PRs:** [#3843](https://github.com/microsoft/agent-framework/pull/3843), [#4032](https://github.com/microsoft/agent-framework/pull/4032)

The `pydantic-settings`-based `AFBaseSettings` class has been replaced with a lightweight, function-based settings system using `TypedDict` and `load_settings()`. The `pydantic-settings` dependency was removed entirely.

All settings classes (e.g., `OpenAISettings`, `AzureOpenAISettings`, `AnthropicSettings`) are now `TypedDict` definitions, and settings values are accessed via dictionary syntax instead of attribute access.

**Before:**
```python
from agent_framework.openai import OpenAISettings

settings = OpenAISettings()  # pydantic-settings auto-loads from env
api_key = settings.api_key
model_id = settings.model_id
```

**After:**
```python
from agent_framework.openai import OpenAISettings, load_settings

settings = load_settings(OpenAISettings, env_prefix="OPENAI_")
api_key = settings["api_key"]
model_id = settings["model_id"]
```

> [!IMPORTANT]
> Agent Framework does **not** automatically load values from `.env` files. You must explicitly opt in to `.env` loading by either:
>
> - Calling `load_dotenv()` from the `python-dotenv` package at the start of your application
> - Passing `env_file_path=".env"` to `load_settings()`
> - Setting environment variables directly in your shell or IDE
>
> The `load_settings` resolution order is: explicit overrides → `.env` file values (when `env_file_path` is provided) → environment variables → defaults. If you specify `env_file_path`, the file must exist or a `FileNotFoundError` is raised.

---

### 🟡 Fix reasoning model workflow handoff and history serialization

**PR:** [#4083](https://github.com/microsoft/agent-framework/pull/4083)

Fixes multiple failures when using reasoning models (e.g., gpt-5-mini, gpt-5.2) in multi-agent workflows. Reasoning items from the Responses API are now correctly serialized and only included in history when a `function_call` is also present, preventing API errors. Encrypted/hidden reasoning content is now properly emitted, and the `summary` field format is corrected. The `service_session_id` is also cleared on handoff to prevent cross-agent state leakage.

---

### 🟡 Bedrock added to `core[all]` and tool-choice defaults fixed

**PR:** [#3953](https://github.com/microsoft/agent-framework/pull/3953)

Amazon Bedrock is now included in the `agent-framework-core[all]` extras and is available via the `agent_framework.amazon` lazy import surface. Tool-choice behavior was also fixed: unset tool-choice values now remain unset so providers use their service defaults, while explicitly set values are preserved.

```python
from agent_framework.amazon import BedrockClient
```

---

### 🟡 AzureAIClient warns on unsupported runtime overrides

**PR:** [#3919](https://github.com/microsoft/agent-framework/pull/3919)

`AzureAIClient` now logs a warning when runtime `tools` or `structured_output` differ from the agent's creation-time configuration. The Azure AI Agent Service does not support runtime tool or response format changes — use `AzureOpenAIResponsesClient` instead if you need dynamic overrides.

---

### 🟡 `workflow.as_agent()` now defaults local history when providers are unset

**PR:** [#3918](https://github.com/microsoft/agent-framework/pull/3918)

When `workflow.as_agent()` is created without `context_providers`, it now adds `InMemoryHistoryProvider("memory")` by default.
If context providers are explicitly supplied, that list is preserved unchanged.

```python
workflow_agent = workflow.as_agent(name="MyWorkflowAgent")
# Default local history provider is injected when none are provided.
```

---

### 🟡 OpenTelemetry trace context propagated to MCP requests

**PR:** [#3780](https://github.com/microsoft/agent-framework/pull/3780)

When OpenTelemetry is installed, trace context (e.g., W3C `traceparent`) is automatically injected into MCP requests via `params._meta`. This enables end-to-end distributed tracing across agent → MCP server calls. No code changes needed — this is additive behavior that activates when a valid span context exists.

---

### 🟡 Durable workflow support for Azure Functions

**PR:** [#3630](https://github.com/microsoft/agent-framework/pull/3630)

The `agent-framework-azurefunctions` package now supports running `Workflow` graphs on Azure Durable Functions. Pass a `workflow` parameter to `AgentFunctionApp` to automatically register agent entities, activity functions, and HTTP endpoints.

```python
from agent_framework.azurefunctions import AgentFunctionApp

app = AgentFunctionApp(workflow=my_workflow)
# Automatically registers:
#   POST /api/workflow/run          — start a workflow
#   GET  /api/workflow/status/{id}  — check status
#   POST /api/workflow/respond/{id}/{requestId} — HITL response
```

Supports fan-out/fan-in, shared state, and human-in-the-loop patterns with configurable timeout and automatic rejection on expiry.

---

## python-1.0.0b260212 (February 12, 2026)

**Release Notes:** [python-1.0.0b260212](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260212)

### 🔴 `Hosted*Tool` classes replaced by client `get_*_tool()` methods

**PR:** [#3634](https://github.com/microsoft/agent-framework/pull/3634)

The hosted tool classes were removed in favor of client-scoped factory methods. This makes tool availability explicit by provider.

| Removed class | Replacement |
|---|---|
| `HostedCodeInterpreterTool` | `client.get_code_interpreter_tool()` |
| `HostedWebSearchTool` | `client.get_web_search_tool()` |
| `HostedFileSearchTool` | `client.get_file_search_tool(...)` |
| `HostedMCPTool` | `client.get_mcp_tool(...)` |
| `HostedImageGenerationTool` | `client.get_image_generation_tool(...)` |

**Before:**
```python
from agent_framework import HostedCodeInterpreterTool, HostedWebSearchTool

tools = [HostedCodeInterpreterTool(), HostedWebSearchTool()]
```

**After:**
```python
from agent_framework.openai import OpenAIResponsesClient

client = OpenAIResponsesClient()
tools = [client.get_code_interpreter_tool(), client.get_web_search_tool()]
```

---

### 🔴 Session/context provider pipeline finalized (`AgentSession`, `context_providers`)

**PR:** [#3850](https://github.com/microsoft/agent-framework/pull/3850)

The Python session and context-provider migration was completed. `AgentThread` and the old context-provider types were removed.

- `AgentThread` → `AgentSession`
- `agent.get_new_thread()` → `agent.create_session()`
- `agent.get_new_thread(service_thread_id=...)` → `agent.get_session(service_session_id=...)`
- `context_provider=` / `chat_message_store_factory=` patterns are replaced by `context_providers=[...]`

**Before:**
```python
thread = agent.get_new_thread()
response = await agent.run("Hello", thread=thread)
```

**After:**
```python
session = agent.create_session()
response = await agent.run("Hello", session=session)
```

---

### 🔴 Checkpoint model and storage behavior refactored

**PR:** [#3744](https://github.com/microsoft/agent-framework/pull/3744)

Checkpoint internals were redesigned, which affects persisted checkpoint compatibility and custom storage implementations:

- `WorkflowCheckpoint` now stores live objects (serialization happens in checkpoint storage)
- `FileCheckpointStorage` now uses pickle serialization
- `workflow_id` was removed and `previous_checkpoint_id` was added
- Deprecated checkpoint hooks were removed

If you persist checkpoints between versions, regenerate or migrate existing checkpoint artifacts before resuming workflows.

---

### 🟡 `AzureOpenAIResponsesClient` supports Microsoft Foundry project endpoints

**PR:** [#3814](https://github.com/microsoft/agent-framework/pull/3814)

You can now create `AzureOpenAIResponsesClient` with a Foundry project endpoint or `AIProjectClient`, not only direct Azure OpenAI endpoints.

```python
from azure.identity import DefaultAzureCredential
from agent_framework.azure import AzureOpenAIResponsesClient

client = AzureOpenAIResponsesClient(
    project_endpoint="https://<your-project>.services.ai.azure.com",
    deployment_name="gpt-4o-mini",
    credential=DefaultAzureCredential(),
)
```

---

### 🔴 Middleware `call_next` no longer accepts `context`

**PR:** [#3829](https://github.com/microsoft/agent-framework/pull/3829)

Middleware continuation now takes no arguments. If your middleware still calls `call_next(context)`, update it to `call_next()`.

**Before:**
```python
async def telemetry_middleware(context, call_next):
    # ...
    return await call_next(context)
```

**After:**
```python
async def telemetry_middleware(context, call_next):
    # ...
    return await call_next()
```

---

## python-1.0.0b260210 (February 10, 2026)

**Release Notes:** [python-1.0.0b260210](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210)

### 🔴 Workflow factory methods removed from `WorkflowBuilder`

**PR:** [#3781](https://github.com/microsoft/agent-framework/pull/3781)

`register_executor()` and `register_agent()` have been removed from `WorkflowBuilder`. All builder methods (`add_edge`, `add_fan_out_edges`, `add_fan_in_edges`, `add_chain`, `add_switch_case_edge_group`, `add_multi_selection_edge_group`) and `start_executor` no longer accept string names — they require executor or agent instances directly.

For state isolation, wrap executor/agent instantiation and workflow building inside a helper method so each call produces fresh instances.

#### `WorkflowBuilder` with executors

**Before:**
```python
workflow = (
    WorkflowBuilder(start_executor="UpperCase")
    .register_executor(lambda: UpperCaseExecutor(id="upper"), name="UpperCase")
    .register_executor(lambda: ReverseExecutor(id="reverse"), name="Reverse")
    .add_edge("UpperCase", "Reverse")
    .build()
)
```

**After:**
```python
upper = UpperCaseExecutor(id="upper")
reverse = ReverseExecutor(id="reverse")

workflow = WorkflowBuilder(start_executor=upper).add_edge(upper, reverse).build()
```

#### `WorkflowBuilder` with agents

**Before:**
```python
builder = WorkflowBuilder(start_executor="writer_agent")
builder.register_agent(factory_func=create_writer_agent, name="writer_agent")
builder.register_agent(factory_func=create_reviewer_agent, name="reviewer_agent")
builder.add_edge("writer_agent", "reviewer_agent")

workflow = builder.build()
```

**After:**
```python
writer_agent = create_writer_agent()
reviewer_agent = create_reviewer_agent()

workflow = WorkflowBuilder(start_executor=writer_agent).add_edge(writer_agent, reviewer_agent).build()
```

#### State isolation with helper methods

For workflows that need isolated state per invocation, wrap construction in a helper method:

```python
def create_workflow() -> Workflow:
    """Each call produces fresh executor instances with independent state."""
    upper = UpperCaseExecutor(id="upper")
    reverse = ReverseExecutor(id="reverse")

    return WorkflowBuilder(start_executor=upper).add_edge(upper, reverse).build()

workflow_a = create_workflow()
workflow_b = create_workflow()
```

---

### 🔴 `ChatAgent` renamed to `Agent`, `ChatMessage` renamed to `Message`

**PR:** [#3747](https://github.com/microsoft/agent-framework/pull/3747)

Core Python types have been simplified by removing the redundant `Chat` prefix. No backward-compatibility aliases are provided.

| Before | After |
|--------|-------|
| `ChatAgent` | `Agent` |
| `RawChatAgent` | `RawAgent` |
| `ChatMessage` | `Message` |
| `ChatClientProtocol` | `SupportsChatGetResponse` |

#### Update imports

**Before:**
```python
from agent_framework import ChatAgent, ChatMessage
```

**After:**
```python
from agent_framework import Agent, Message
```

#### Update type references

**Before:**
```python
agent = ChatAgent(
    chat_client=client,
    name="assistant",
    instructions="You are a helpful assistant.",
)

message = ChatMessage(role="user", contents=[Content.from_text("Hello")])
```

**After:**
```python
agent = Agent(
    client=client,
    name="assistant",
    instructions="You are a helpful assistant.",
)

message = Message(role="user", contents=[Content.from_text("Hello")])
```

> [!NOTE]
> `ChatClient`, `ChatResponse`, `ChatOptions`, and `ChatMessageStore` are **not** renamed by this change.

---

### 🔴 Types API review updates across response/message models

**PR:** [#3647](https://github.com/microsoft/agent-framework/pull/3647)

This release includes a broad, breaking cleanup of message/response typing and helper APIs.

- `Role` and `FinishReason` are now `NewType` wrappers over `str` with `RoleLiteral`/`FinishReasonLiteral` for known values. Treat them as strings (no `.value` usage).
- `Message` construction is standardized on `Message(role, contents=[...])`; strings in `contents` are auto-converted to text content.
- `ChatResponse` and `AgentResponse` constructors now center on `messages=` (single `Message` or sequence); legacy `text=` constructor usage was removed from responses.
- `ChatResponseUpdate` and `AgentResponseUpdate` no longer accept `text=`; use `contents=[Content.from_text(...)]`.
- Update-combining helper names were simplified.
- `try_parse_value` was removed from `ChatResponse` and `AgentResponse`.

#### Helper method renames

| Before | After |
|---|---|
| `ChatResponse.from_chat_response_updates(...)` | `ChatResponse.from_updates(...)` |
| `ChatResponse.from_chat_response_generator(...)` | `ChatResponse.from_update_generator(...)` |
| `AgentResponse.from_agent_run_response_updates(...)` | `AgentResponse.from_updates(...)` |

#### Update response-update construction

**Before:**
```python
update = AgentResponseUpdate(text="Processing...", role="assistant")
```

**After:**
```python
from agent_framework import AgentResponseUpdate, Content

update = AgentResponseUpdate(
    contents=[Content.from_text("Processing...")],
    role="assistant",
)
```

#### Replace `try_parse_value` with `try/except` on `.value`

**Before:**
```python
if parsed := response.try_parse_value(MySchema):
    print(parsed.name)
```

**After:**
```python
from pydantic import ValidationError

try:
    parsed = response.value
    if parsed:
        print(parsed.name)
except ValidationError as err:
    print(f"Validation failed: {err}")
```

---

### 🔴 Unified `run`/`get_response` model and `ResponseStream` usage

**PR:** [#3379](https://github.com/microsoft/agent-framework/pull/3379)

Python APIs were consolidated around `agent.run(...)` and `client.get_response(...)`, with streaming represented by `ResponseStream`.

**Before:**
```python
async for update in agent.run_stream("Hello"):
    print(update)
```

**After:**
```python
stream = agent.run("Hello", stream=True)
async for update in stream:
    print(update)
```

---

### 🔴 Core context/protocol type renames

**PRs:** [#3714](https://github.com/microsoft/agent-framework/pull/3714), [#3717](https://github.com/microsoft/agent-framework/pull/3717)

| Before | After |
|---|---|
| `AgentRunContext` | `AgentContext` |
| `AgentProtocol` | `SupportsAgentRun` |

Update imports and type annotations accordingly.

---

### 🔴 Middleware continuation parameter renamed to `call_next`

**PR:** [#3735](https://github.com/microsoft/agent-framework/pull/3735)

Middleware signatures should now use `call_next` instead of `next`.

**Before:**
```python
async def my_middleware(context, next):
    return await next(context)
```

**After:**
```python
async def my_middleware(context, call_next):
    return await call_next(context)
```

---

### 🔴 TypeVar names standardized (`TName` → `NameT`)

**PR:** [#3770](https://github.com/microsoft/agent-framework/pull/3770)

The codebase now follows a consistent TypeVar naming style where suffix `T` is used.

**Before:**
```python
TMessage = TypeVar("TMessage")
```

**After:**
```python
MessageT = TypeVar("MessageT")
```

If you maintain custom wrappers around framework generics, align your local TypeVar names with the new convention to reduce annotation churn.

---

### 🔴 Workflow-as-agent output and streaming changes

**PR:** [#3649](https://github.com/microsoft/agent-framework/pull/3649)

`workflow.as_agent()` behavior was updated to align output and streaming with standard agent response patterns. Review workflow-as-agent consumers that depend on legacy output/update handling and update them to the current `AgentResponse`/`AgentResponseUpdate` flow.

---

### 🔴 Fluent builder methods moved to constructor parameters

**PR:** [#3693](https://github.com/microsoft/agent-framework/pull/3693)

Single-config fluent methods across 6 builders (`WorkflowBuilder`, `SequentialBuilder`, `ConcurrentBuilder`, `GroupChatBuilder`, `MagenticBuilder`, `HandoffBuilder`) have been migrated to constructor parameters. Fluent methods that were the sole configuration path for a setting are removed in favor of constructor arguments.

#### `WorkflowBuilder`

`set_start_executor()`, `with_checkpointing()`, and `with_output_from()` are removed. Use constructor parameters instead.

**Before:**
```python
upper = UpperCaseExecutor(id="upper")
reverse = ReverseExecutor(id="reverse")

workflow = (
    WorkflowBuilder(start_executor=upper)
    .add_edge(upper, reverse)
    .set_start_executor(upper)
    .with_checkpointing(storage)
    .build()
)
```

**After:**
```python
upper = UpperCaseExecutor(id="upper")
reverse = ReverseExecutor(id="reverse")

workflow = (
    WorkflowBuilder(start_executor=upper, checkpoint_storage=storage)
    .add_edge(upper, reverse)
    .build()
)
```

#### `SequentialBuilder` / `ConcurrentBuilder`

`participants()`, `register_participants()`, `with_checkpointing()`, and `with_intermediate_outputs()` are removed. Use constructor parameters instead.

**Before:**
```python
workflow = SequentialBuilder().participants([agent_a, agent_b]).with_checkpointing(storage).build()
```

**After:**
```python
workflow = SequentialBuilder(participants=[agent_a, agent_b], checkpoint_storage=storage).build()
```

#### `GroupChatBuilder`

`participants()`, `register_participants()`, `with_orchestrator()`, `with_termination_condition()`, `with_max_rounds()`, `with_checkpointing()`, and `with_intermediate_outputs()` are removed. Use constructor parameters instead.

**Before:**
```python
workflow = (
    GroupChatBuilder()
    .with_orchestrator(selection_func=selector)
    .participants([agent1, agent2])
    .with_termination_condition(lambda conv: len(conv) >= 4)
    .with_max_rounds(10)
    .build()
)
```

**After:**
```python
workflow = GroupChatBuilder(
    participants=[agent1, agent2],
    selection_func=selector,
    termination_condition=lambda conv: len(conv) >= 4,
    max_rounds=10,
).build()
```

#### `MagenticBuilder`

`participants()`, `register_participants()`, `with_manager()`, `with_plan_review()`, `with_checkpointing()`, and `with_intermediate_outputs()` are removed. Use constructor parameters instead.

**Before:**
```python
workflow = (
    MagenticBuilder()
    .participants([researcher, coder])
    .with_manager(agent=manager_agent)
    .with_plan_review()
    .build()
)
```

**After:**
```python
workflow = MagenticBuilder(
    participants=[researcher, coder],
    manager_agent=manager_agent,
    enable_plan_review=True,
).build()
```

#### `HandoffBuilder`

`with_checkpointing()` and `with_termination_condition()` are removed. Use constructor parameters instead.

**Before:**
```python
workflow = (
    HandoffBuilder(participants=[triage, specialist])
    .with_start_agent(triage)
    .with_termination_condition(lambda conv: len(conv) > 5)
    .with_checkpointing(storage)
    .build()
)
```

**After:**
```python
workflow = (
    HandoffBuilder(
        participants=[triage, specialist],
        termination_condition=lambda conv: len(conv) > 5,
        checkpoint_storage=storage,
    )
    .with_start_agent(triage)
    .build()
)
```

#### Validation changes

- `WorkflowBuilder` now requires `start_executor` as a constructor argument (previously set via fluent method)
- `SequentialBuilder`, `ConcurrentBuilder`, `GroupChatBuilder`, and `MagenticBuilder` now require either `participants` or `participant_factories` at construction time — passing neither raises `ValueError`

> [!NOTE]
> `HandoffBuilder` already accepted `participants`/`participant_factories` as constructor parameters and was not changed in this regard.

---

### 🔴 Workflow events unified into single `WorkflowEvent` with `type` discriminator

**PR:** [#3690](https://github.com/microsoft/agent-framework/pull/3690)

All individual workflow event subclasses have been replaced by a single generic `WorkflowEvent[DataT]` class. Instead of using `isinstance()` checks to identify event types, you now check the `event.type` string literal (e.g., `"output"`, `"request_info"`, `"status"`). This follows the same pattern as the `Content` class consolidation from `python-1.0.0b260123`.

#### Removed event classes

The following exported event subclasses no longer exist:

| Old Class | New `event.type` Value |
|-----------|----------------------|
| `WorkflowOutputEvent` | `"output"` |
| `RequestInfoEvent` | `"request_info"` |
| `WorkflowStatusEvent` | `"status"` |
| `WorkflowStartedEvent` | `"started"` |
| `WorkflowFailedEvent` | `"failed"` |
| `ExecutorInvokedEvent` | `"executor_invoked"` |
| `ExecutorCompletedEvent` | `"executor_completed"` |
| `ExecutorFailedEvent` | `"executor_failed"` |
| `SuperStepStartedEvent` | `"superstep_started"` |
| `SuperStepCompletedEvent` | `"superstep_completed"` |

#### Update imports

**Before:**
```python
from agent_framework import (
    WorkflowOutputEvent,
    RequestInfoEvent,
    WorkflowStatusEvent,
    ExecutorCompletedEvent,
)
```

**After:**
```python
from agent_framework import WorkflowEvent
# Individual event classes no longer exist; use event.type to discriminate
```

#### Update event type checks

**Before:**
```python
async for event in workflow.run_stream(input_message):
    if isinstance(event, WorkflowOutputEvent):
        print(f"Output from {event.executor_id}: {event.data}")
    elif isinstance(event, RequestInfoEvent):
        requests[event.request_id] = event.data
    elif isinstance(event, WorkflowStatusEvent):
        print(f"Status: {event.state}")
```

**After:**
```python
async for event in workflow.run_stream(input_message):
    if event.type == "output":
        print(f"Output from {event.executor_id}: {event.data}")
    elif event.type == "request_info":
        requests[event.request_id] = event.data
    elif event.type == "status":
        print(f"Status: {event.state}")
```

#### Streaming with `AgentResponseUpdate`

**Before:**
```python
from agent_framework import AgentResponseUpdate, WorkflowOutputEvent

async for event in workflow.run_stream("Write a blog post about AI agents."):
    if isinstance(event, WorkflowOutputEvent) and isinstance(event.data, AgentResponseUpdate):
        print(event.data, end="", flush=True)
    elif isinstance(event, WorkflowOutputEvent):
        print(f"Final output: {event.data}")
```

**After:**
```python
from agent_framework import AgentResponseUpdate

async for event in workflow.run_stream("Write a blog post about AI agents."):
    if event.type == "output" and isinstance(event.data, AgentResponseUpdate):
        print(event.data, end="", flush=True)
    elif event.type == "output":
        print(f"Final output: {event.data}")
```

#### Type annotations

**Before:**
```python
pending_requests: list[RequestInfoEvent] = []
output: WorkflowOutputEvent | None = None
```

**After:**
```python
from typing import Any
from agent_framework import WorkflowEvent

pending_requests: list[WorkflowEvent[Any]] = []
output: WorkflowEvent | None = None
```

> [!NOTE]
> `WorkflowEvent` is generic (`WorkflowEvent[DataT]`), but for collections of mixed events, use `WorkflowEvent[Any]` or unparameterized `WorkflowEvent`.

---

### 🔴 `workflow.send_responses*` removed; use `workflow.run(responses=...)`

**PR:** [#3720](https://github.com/microsoft/agent-framework/pull/3720)

`send_responses()` and `send_responses_streaming()` were removed from `Workflow`. Continue paused workflows by passing responses directly to `run()`.

**Before:**
```python
async for event in workflow.send_responses_streaming(
    checkpoint_id=checkpoint_id,
    responses=[approved_response],
):
    ...
```

**After:**
```python
async for event in workflow.run(
    checkpoint_id=checkpoint_id,
    responses=[approved_response],
):
    ...
```

---

### 🔴 `SharedState` renamed to `State`; workflow state APIs are synchronous

**PR:** [#3667](https://github.com/microsoft/agent-framework/pull/3667)

State APIs no longer require `await`, and naming was standardized:

| Before | After |
|---|---|
| `ctx.shared_state` | `ctx.state` |
| `await ctx.get_shared_state("k")` | `ctx.get_state("k")` |
| `await ctx.set_shared_state("k", v)` | `ctx.set_state("k", v)` |
| `checkpoint.shared_state` | `checkpoint.state` |

---

### 🔴 Orchestration builders moved to `agent_framework.orchestrations`

**PR:** [#3685](https://github.com/microsoft/agent-framework/pull/3685)

Orchestration builders are now in a dedicated package namespace.

**Before:**
```python
from agent_framework import SequentialBuilder, GroupChatBuilder
```

**After:**
```python
from agent_framework.orchestrations import SequentialBuilder, GroupChatBuilder
```

---

### 🟡 Long-running background responses and continuation tokens

**PR:** [#3808](https://github.com/microsoft/agent-framework/pull/3808)

Background responses are now supported for Python agent runs through `options={"background": True}` and `continuation_token`.

```python
response = await agent.run("Long task", options={"background": True})
while response.continuation_token is not None:
    response = await agent.run(options={"continuation_token": response.continuation_token})
```

---

### 🟡 Session/context provider preview types added side-by-side

**PR:** [#3763](https://github.com/microsoft/agent-framework/pull/3763)

New session/context pipeline types were introduced alongside legacy APIs for incremental migration, including `SessionContext` and `BaseContextProvider`.

---

### 🟡 Code interpreter streaming now includes incremental code deltas

**PR:** [#3775](https://github.com/microsoft/agent-framework/pull/3775)

Streaming code-interpreter runs now surface code delta updates in the streamed content so UIs can render generated code progressively.

---

### 🟡 `@tool` supports explicit schema handling

**PR:** [#3734](https://github.com/microsoft/agent-framework/pull/3734)

Tool definitions can now use explicit schema handling when inferred schema output needs customization.

---

## python-1.0.0b260130 (January 30, 2026)

**Release Notes:** [python-1.0.0b260130](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260130)

### 🟡 `ChatOptions` and `ChatResponse`/`AgentResponse` now generic over response format

**PR:** [#3305](https://github.com/microsoft/agent-framework/pull/3305)

`ChatOptions`, `ChatResponse`, and `AgentResponse` are now generic types parameterized by the response format type. This enables better type inference when using structured outputs with `response_format`.

**Before:**
```python
from agent_framework import ChatOptions, ChatResponse
from pydantic import BaseModel

class MyOutput(BaseModel):
    name: str
    score: int

options: ChatOptions = {"response_format": MyOutput}  # No type inference
response: ChatResponse = await client.get_response("Query", options=options)
result = response.value  # Type: Any
```

**After:**
```python
from agent_framework import ChatOptions, ChatResponse
from pydantic import BaseModel

class MyOutput(BaseModel):
    name: str
    score: int

options: ChatOptions[MyOutput] = {"response_format": MyOutput}  # Generic parameter
response: ChatResponse[MyOutput] = await client.get_response("Query", options=options)
result = response.value  # Type: MyOutput | None (inferred!)
```

> [!TIP]
> This is a non-breaking enhancement. Existing code without type parameters continues to work.
> You do not need to specify the types in the code snippet above for the options and response; they are shown here for clarity.

---

### 🟡 `BaseAgent` support added for Claude Agent SDK

**PR:** [#3509](https://github.com/microsoft/agent-framework/pull/3509)

The Python SDK now includes a `BaseAgent` implementation for the Claude Agent SDK, enabling first-class adapter-based usage in Agent Framework.

---

## python-1.0.0b260128 (January 28, 2026)

**Release Notes:** [python-1.0.0b260128](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260128)

### 🔴 `AIFunction` renamed to `FunctionTool` and `@ai_function` renamed to `@tool`

**PR:** [#3413](https://github.com/microsoft/agent-framework/pull/3413)

The class and decorator have been renamed for clarity and consistency with industry terminology.

**Before:**
```python
from agent_framework.core import ai_function, AIFunction

@ai_function
def get_weather(city: str) -> str:
    """Get the weather for a city."""
    return f"Weather in {city}: Sunny"

# Or using the class directly
func = AIFunction(get_weather)
```

**After:**
```python
from agent_framework.core import tool, FunctionTool

@tool
def get_weather(city: str) -> str:
    """Get the weather for a city."""
    return f"Weather in {city}: Sunny"

# Or using the class directly
func = FunctionTool(get_weather)
```

---

### 🔴 Factory pattern added to GroupChat and Magentic; API renames

**PR:** [#3224](https://github.com/microsoft/agent-framework/pull/3224)

Added participant factory and orchestrator factory to group chat. Also includes renames:
- `with_standard_manager` → `with_manager`
- `participant_factories` → `register_participant`

**Before:**
```python
from agent_framework.workflows import MagenticBuilder

builder = MagenticBuilder()
builder.with_standard_manager(manager)
builder.participant_factories(factory1, factory2)
```

**After:**
```python
from agent_framework.workflows import MagenticBuilder

builder = MagenticBuilder()
builder.with_manager(manager)
builder.register_participant(factory1)
builder.register_participant(factory2)
```

---

### 🔴 `Github` renamed to `GitHub`

**PR:** [#3486](https://github.com/microsoft/agent-framework/pull/3486)

Class and package names updated to use correct casing.

**Before:**
```python
from agent_framework_github_copilot import GithubCopilotAgent

agent = GithubCopilotAgent(...)
```

**After:**
```python
from agent_framework_github_copilot import GitHubCopilotAgent

agent = GitHubCopilotAgent(...)
```

---

## python-1.0.0b260127 (January 27, 2026)

**Release Notes:** [python-1.0.0b260127](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260127)

### 🟡 `BaseAgent` support added for GitHub Copilot SDK

**PR:** [#3404](https://github.com/microsoft/agent-framework/pull/3404)

The Python SDK now includes a `BaseAgent` implementation for GitHub Copilot SDK integrations.

---

## python-1.0.0b260123 (January 23, 2026)

**Release Notes:** [python-1.0.0b260123](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260123)

### 🔴 Content types simplified to a single class with classmethod constructors

**PR:** [#3252](https://github.com/microsoft/agent-framework/pull/3252)

Replaced all old Content types (derived from `BaseContent`) with a single `Content` class with classmethods to create specific types.

#### Full Migration Reference

| Old Type | New Method |
|----------|------------|
| `TextContent(text=...)` | `Content.from_text(text=...)` |
| `DataContent(data=..., media_type=...)` | `Content.from_data(data=..., media_type=...)` |
| `UriContent(uri=..., media_type=...)` | `Content.from_uri(uri=..., media_type=...)` |
| `ErrorContent(message=...)` | `Content.from_error(message=...)` |
| `HostedFileContent(file_id=...)` | `Content.from_hosted_file(file_id=...)` |
| `FunctionCallContent(name=..., arguments=..., call_id=...)` | `Content.from_function_call(name=..., arguments=..., call_id=...)` |
| `FunctionResultContent(call_id=..., result=...)` | `Content.from_function_result(call_id=..., result=...)` |
| `FunctionApprovalRequestContent(...)` | `Content.from_function_approval_request(...)` |
| `FunctionApprovalResponseContent(...)` | `Content.from_function_approval_response(...)` |

Additional new methods (no direct predecessor):
- `Content.from_text_reasoning(...)` — For reasoning/thinking content
- `Content.from_hosted_vector_store(...)` — For vector store references
- `Content.from_usage(...)` — For usage/token information
- `Content.from_mcp_server_tool_call(...)` / `Content.from_mcp_server_tool_result(...)` — For MCP server tools
- `Content.from_code_interpreter_tool_call(...)` / `Content.from_code_interpreter_tool_result(...)` — For code interpreter
- `Content.from_image_generation_tool_call(...)` / `Content.from_image_generation_tool_result(...)` — For image generation

#### Type Checking

Instead of `isinstance()` checks, use the `type` property:

**Before:**
```python
from agent_framework.core import TextContent, FunctionCallContent

if isinstance(content, TextContent):
    print(content.text)
elif isinstance(content, FunctionCallContent):
    print(content.name)
```

**After:**
```python
from agent_framework.core import Content

if content.type == "text":
    print(content.text)
elif content.type == "function_call":
    print(content.name)
```

#### Basic Example

**Before:**
```python
from agent_framework.core import TextContent, DataContent, UriContent

text = TextContent(text="Hello world")
data = DataContent(data=b"binary", media_type="application/octet-stream")
uri = UriContent(uri="https://example.com/image.png", media_type="image/png")
```

**After:**
```python
from agent_framework.core import Content

text = Content.from_text("Hello world")
data = Content.from_data(data=b"binary", media_type="application/octet-stream")
uri = Content.from_uri(uri="https://example.com/image.png", media_type="image/png")
```

---

### 🔴 Annotation types simplified to `Annotation` and `TextSpanRegion` TypedDicts

**PR:** [#3252](https://github.com/microsoft/agent-framework/pull/3252)

Replaced class-based annotation types with simpler `TypedDict` definitions.

| Old Type | New Type |
|----------|----------|
| `CitationAnnotation` (class) | `Annotation` (TypedDict with `type="citation"`) |
| `BaseAnnotation` (class) | `Annotation` (TypedDict) |
| `TextSpanRegion` (class with `SerializationMixin`) | `TextSpanRegion` (TypedDict) |
| `Annotations` (type alias) | `Annotation` |
| `AnnotatedRegions` (type alias) | `TextSpanRegion` |

**Before:**
```python
from agent_framework import CitationAnnotation, TextSpanRegion

region = TextSpanRegion(start_index=0, end_index=25)
citation = CitationAnnotation(
    annotated_regions=[region],
    url="https://example.com/source",
    title="Source Title"
)
```

**After:**
```python
from agent_framework import Annotation, TextSpanRegion

region: TextSpanRegion = {"start_index": 0, "end_index": 25}
citation: Annotation = {
    "type": "citation",
    "annotated_regions": [region],
    "url": "https://example.com/source",
    "title": "Source Title"
}
```

> [!NOTE]
> Since `Annotation` and `TextSpanRegion` are now `TypedDict`s, you create them as dictionaries rather than class instances.

---

### 🔴 `response_format` validation errors now visible to users

**PR:** [#3274](https://github.com/microsoft/agent-framework/pull/3274)

`ChatResponse.value` and `AgentResponse.value` now raise `ValidationError` when schema validation fails instead of silently returning `None`.

**Before:**
```python
response = await agent.run(query, options={"response_format": MySchema})
if response.value:  # Returns None on validation failure - no error details
    print(response.value.name)
```

**After:**
```python
from pydantic import ValidationError

# Option 1: Catch validation errors
try:
    print(response.value.name)  # Raises ValidationError on failure
except ValidationError as e:
    print(f"Validation failed: {e}")

# Option 2: Safe parsing (returns None on failure)
if result := response.try_parse_value(MySchema):
    print(result.name)
```

---

### 🔴 AG-UI run logic simplified; MCP and Anthropic client fixes

**PR:** [#3322](https://github.com/microsoft/agent-framework/pull/3322)

The `run` method signature and behavior in AG-UI has been simplified.

**Before:**
```python
from agent_framework.ag_ui import AGUIEndpoint

endpoint = AGUIEndpoint(agent=agent)
result = await endpoint.run(
    request=request,
    run_config={"streaming": True, "timeout": 30}
)
```

**After:**
```python
from agent_framework.ag_ui import AGUIEndpoint

endpoint = AGUIEndpoint(agent=agent)
result = await endpoint.run(
    request=request,
    streaming=True,
    timeout=30
)
```

---

### 🟡 Anthropic client now supports `response_format` structured outputs

**PR:** [#3301](https://github.com/microsoft/agent-framework/pull/3301)

You can now use structured output parsing with Anthropic clients via `response_format`, similar to OpenAI and Azure clients.

---

### 🟡 Azure AI configuration expanded (`reasoning`, `rai_config`)

**PRs:** [#3403](https://github.com/microsoft/agent-framework/pull/3403), [#3265](https://github.com/microsoft/agent-framework/pull/3265)

Azure AI support was expanded with reasoning configuration support and `rai_config` during agent creation.

---

## python-1.0.0b260116 (January 16, 2026)

**Release Notes:** [python-1.0.0b260116](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260116)

### 🔴 `create_agent` renamed to `as_agent`

**PR:** [#3249](https://github.com/microsoft/agent-framework/pull/3249)

Method renamed for better clarity on its purpose.

**Before:**
```python
from agent_framework.core import ChatClient

client = ChatClient(...)
agent = client.create_agent()
```

**After:**
```python
from agent_framework.core import ChatClient

client = ChatClient(...)
agent = client.as_agent()
```

---

### 🔴 `WorkflowOutputEvent.source_executor_id` renamed to `executor_id`

**PR:** [#3166](https://github.com/microsoft/agent-framework/pull/3166)

Property renamed for API consistency.

**Before:**
```python
async for event in workflow.run_stream(...):
    if isinstance(event, WorkflowOutputEvent):
        executor = event.source_executor_id
```

**After:**
```python
async for event in workflow.run_stream(...):
    if isinstance(event, WorkflowOutputEvent):
        executor = event.executor_id
```

---

### 🟡 AG-UI supports service-managed session continuity

**PR:** [#3136](https://github.com/microsoft/agent-framework/pull/3136)

AG-UI now preserves service-managed conversation identity (for example, Foundry-managed sessions/threads) to maintain multi-turn continuity.

---

## python-1.0.0b260114 (January 14, 2026)

**Release Notes:** [python-1.0.0b260114](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260114)

### 🔴 Orchestrations refactored

**PR:** [#3023](https://github.com/microsoft/agent-framework/pull/3023)

Extensive refactor and simplification of orchestrations in Agent Framework Workflows:

- **Group Chat**: Split orchestrator executor into dedicated agent-based and function-based (`BaseGroupChatOrchestrator`, `GroupChatOrchestrator`, `AgentBasedGroupChatOrchestrator`). Simplified to star topology with broadcasting model.
- **Handoff**: Removed single tier, coordinator, and custom executor support. Moved to broadcasting model with `HandoffAgentExecutor`.
- **Sequential & Concurrent**: Simplified request info mechanism to rely on sub-workflows via `AgentApprovalExecutor` and `AgentRequestInfoExecutor`.

**Before:**
```python
from agent_framework.workflows import GroupChat, HandoffOrchestrator

# Group chat with custom coordinator
group = GroupChat(
    participants=[agent1, agent2],
    coordinator=my_coordinator
)

# Handoff with single tier
handoff = HandoffOrchestrator(
    agents=[agent1, agent2],
    tier="single"
)
```

**After:**
```python
from agent_framework.workflows import (
    GroupChatOrchestrator,
    HandoffAgentExecutor,
    AgentApprovalExecutor
)

# Group chat with star topology
group = GroupChatOrchestrator(
    participants=[agent1, agent2]
)

# Handoff with executor-based approach
handoff = HandoffAgentExecutor(
    agents=[agent1, agent2]
)
```

---

### 🔴 Options introduced as TypedDict and Generic

**PR:** [#3140](https://github.com/microsoft/agent-framework/pull/3140)

Options are now typed using `TypedDict` for better type safety and IDE autocomplete.

**📖 For complete migration instructions, see the [Typed Options Guide](typed-options-guide-python.md).**

**Before:**
```python
response = await client.get_response(
    "Hello!",
    model_id="gpt-4",
    temperature=0.7,
    max_tokens=1000,
)
```

**After:**
```python
response = await client.get_response(
    "Hello!",
    options={
        "model_id": "gpt-4",
        "temperature": 0.7,
        "max_tokens": 1000,
    },
)
```

---

### 🔴 `display_name` removed; `context_provider` to singular; `middleware` must be list

**PR:** [#3139](https://github.com/microsoft/agent-framework/pull/3139)

- `display_name` parameter removed from agents
- `context_providers` (plural, accepting list) changed to `context_provider` (singular, only 1 allowed)
- `middleware` now requires a list (no longer accepts single instance)
- `AggregateContextProvider` removed from code (use sample implementation if needed)

**Before:**
```python
from agent_framework.core import Agent, AggregateContextProvider

agent = Agent(
    name="my-agent",
    display_name="My Agent",
    context_providers=[provider1, provider2],
    middleware=my_middleware,  # single instance was allowed
)

aggregate = AggregateContextProvider([provider1, provider2])
```

**After:**
```python
from agent_framework.core import Agent

# Only one context provider allowed; combine manually if needed
agent = Agent(
    name="my-agent",  # display_name removed
    context_provider=provider1,  # singular, only 1
    middleware=[my_middleware],  # must be a list now
)

# For multiple context providers, create your own aggregate
class MyAggregateProvider:
    def __init__(self, providers):
        self.providers = providers
    # ... implement aggregation logic
```

---

### 🔴 `AgentRunResponse*` renamed to `AgentResponse*`

**PR:** [#3207](https://github.com/microsoft/agent-framework/pull/3207)

`AgentRunResponse` and `AgentRunResponseUpdate` were renamed to `AgentResponse` and `AgentResponseUpdate`.

**Before:**
```python
from agent_framework import AgentRunResponse, AgentRunResponseUpdate
```

**After:**
```python
from agent_framework import AgentResponse, AgentResponseUpdate
```

---

### 🟡 Declarative workflow runtime added for YAML-defined workflows

**PR:** [#2815](https://github.com/microsoft/agent-framework/pull/2815)

A graph-based runtime was added for executing declarative YAML workflows, enabling multi-agent orchestration without custom runtime code.

---

### 🟡 MCP loading/reliability improvements

**PR:** [#3154](https://github.com/microsoft/agent-framework/pull/3154)

MCP integrations gained improved connection-loss behavior, pagination support when loading, and representation control options.

---

### 🟡 Foundry `A2ATool` now supports connections without a target URL

**PR:** [#3127](https://github.com/microsoft/agent-framework/pull/3127)

`A2ATool` can now resolve Foundry-backed A2A connections via project connection metadata even when a direct target URL is not configured.

---

## python-1.0.0b260107 (January 7, 2026)

**Release Notes:** [python-1.0.0b260107](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260107)

No significant changes in this release.

---

## python-1.0.0b260106 (January 6, 2026)

**Release Notes:** [python-1.0.0b260106](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260106)

No significant changes in this release.

---

## Summary Table

| Release | Release Notes | Type | Change | PR |
|---------|---------------|------|--------|-----|
| 1.0.0rc6 | PR only | 🔴 Breaking | Provider-leading refactor: split `agent-framework-openai`, `agent-framework-foundry`, and `agent-framework-foundry-local`; rename OpenAI clients; move Foundry to `agent_framework.foundry`; deprecate Azure AI and Assistants compatibility paths | [#4818](https://github.com/microsoft/agent-framework/pull/4818) |
| 1.0.0rc6 | PR only | 🔴 Breaking | `agent-framework-core` is now intentionally slim; install explicit provider packages such as `agent-framework-openai` or `agent-framework-azure-ai`, and install `mcp` manually for MCP tooling on minimal installs, or use the `agent-framework` meta package for the broader default experience | [#4904](https://github.com/microsoft/agent-framework/pull/4904) |
| 1.0.0rc6 | PR only | 🔴 Breaking | Generic `agent_framework.openai` clients now prefer explicit routing signals; OpenAI stays on OpenAI when `OPENAI_API_KEY` is set, and Azure scenarios should pass explicit Azure routing inputs such as `credential` or `azure_endpoint`, then configure `api_version` | [#4925](https://github.com/microsoft/agent-framework/pull/4925) |
| 1.0.0rc5 / 1.0.0b260318 | N/A (scheduled) | 🔴 Breaking | Public runtime kwargs split into `function_invocation_kwargs` and `client_kwargs`; tools now use `FunctionInvocationContext` / `ctx.session` | [#4581](https://github.com/microsoft/agent-framework/pull/4581) |
| 1.0.0rc4 / 1.0.0b260311 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc4) | 🔴 Breaking | Azure AI integrations now target `azure-ai-projects` 2.0 GA; `foundry_features` was removed and `allow_preview` is the preview opt-in | [#4536](https://github.com/microsoft/agent-framework/pull/4536) |
| 1.0.0rc4 / 1.0.0b260311 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc4) | 🔴 Breaking | GitHub Copilot integration now uses `ToolInvocation` / `ToolResult`; `agent-framework-github-copilot` requires Python 3.11+ | [#4551](https://github.com/microsoft/agent-framework/pull/4551) |
| 1.0.0rc3 / 1.0.0b260304 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc3) | 🔴 Breaking | Skills provider adds code-defined `Skill` / `SkillResource`; older `FileAgentSkillsProvider` imports and backtick resource references must be updated | [#4387](https://github.com/microsoft/agent-framework/pull/4387) |
| 1.0.0rc2 / 1.0.0b260226 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc2) | 🔴 Breaking | Declarative workflows replace `InvokeTool` with `InvokeFunctionTool` and `WorkflowFactory.register_tool()` | [#3716](https://github.com/microsoft/agent-framework/pull/3716) |
| 1.0.0rc1 / 1.0.0b260219 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc1) | 🔴 Breaking | Unified Azure credential handling across Azure packages | [#4088](https://github.com/microsoft/agent-framework/pull/4088) |
| 1.0.0rc1 / 1.0.0b260219 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc1) | 🔴 Breaking | Python exception hierarchy redesigned under `AgentFrameworkException` | [#4082](https://github.com/microsoft/agent-framework/pull/4082) |
| 1.0.0rc1 / 1.0.0b260219 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc1) | 🔴 Breaking | Provider state is now scoped by `source_id` | [#3995](https://github.com/microsoft/agent-framework/pull/3995) |
| 1.0.0rc1 / 1.0.0b260219 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc1) | 🔴 Breaking | Custom `get_response()` implementations must accept `Sequence[Message]` | [#3920](https://github.com/microsoft/agent-framework/pull/3920) |
| 1.0.0rc1 / 1.0.0b260219 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc1) | 🔴 Breaking | `FunctionTool[Any]` schema passthrough shim removed | [#3907](https://github.com/microsoft/agent-framework/pull/3907) |
| 1.0.0rc1 / 1.0.0b260219 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc1) | 🔴 Breaking | Settings moved from `AFBaseSettings` / pydantic-settings to `TypedDict` + `load_settings()` | [#3843](https://github.com/microsoft/agent-framework/pull/3843), [#4032](https://github.com/microsoft/agent-framework/pull/4032) |
| 1.0.0rc1 / 1.0.0b260219 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc1) | 🟡 Enhancement | Reasoning-model workflow handoff and history serialization fixed | [#4083](https://github.com/microsoft/agent-framework/pull/4083) |
| 1.0.0rc1 / 1.0.0b260219 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc1) | 🟡 Enhancement | Bedrock added to `core[all]`; tool-choice defaults fixed | [#3953](https://github.com/microsoft/agent-framework/pull/3953) |
| 1.0.0rc1 / 1.0.0b260219 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc1) | 🟡 Enhancement | `AzureAIClient` warns on unsupported runtime overrides | [#3919](https://github.com/microsoft/agent-framework/pull/3919) |
| 1.0.0rc1 / 1.0.0b260219 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc1) | 🟡 Enhancement | `workflow.as_agent()` injects local history when providers are unset | [#3918](https://github.com/microsoft/agent-framework/pull/3918) |
| 1.0.0rc1 / 1.0.0b260219 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc1) | 🟡 Enhancement | OpenTelemetry trace context propagates to MCP requests | [#3780](https://github.com/microsoft/agent-framework/pull/3780) |
| 1.0.0rc1 / 1.0.0b260219 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0rc1) | 🟡 Enhancement | Durable workflow support added for Azure Functions | [#3630](https://github.com/microsoft/agent-framework/pull/3630) |
| 1.0.0b260212 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260212) | 🔴 Breaking | `Hosted*Tool` classes removed; create hosted tools via client `get_*_tool()` methods | [#3634](https://github.com/microsoft/agent-framework/pull/3634) |
| 1.0.0b260212 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260212) | 🔴 Breaking | Session/context provider pipeline finalized: `AgentThread` removed, use `AgentSession` + `context_providers` | [#3850](https://github.com/microsoft/agent-framework/pull/3850) |
| 1.0.0b260212 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260212) | 🔴 Breaking | Checkpoint model/storage refactor (`workflow_id` removed, `previous_checkpoint_id` added, storage behavior changed) | [#3744](https://github.com/microsoft/agent-framework/pull/3744) |
| 1.0.0b260212 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260212) | 🟡 Enhancement | `AzureOpenAIResponsesClient` can be created from Foundry project endpoint or `AIProjectClient` | [#3814](https://github.com/microsoft/agent-framework/pull/3814) |
| 1.0.0b260212 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260212) | 🔴 Breaking | Middleware continuation no longer accepts `context`; update `call_next(context)` to `call_next()` | [#3829](https://github.com/microsoft/agent-framework/pull/3829) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | `send_responses()`/`send_responses_streaming()` removed; use `workflow.run(responses=...)` | [#3720](https://github.com/microsoft/agent-framework/pull/3720) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | `SharedState` → `State`; workflow state APIs are synchronous and checkpoint state field renamed | [#3667](https://github.com/microsoft/agent-framework/pull/3667) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | Orchestration builders moved to `agent_framework.orchestrations` package | [#3685](https://github.com/microsoft/agent-framework/pull/3685) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🟡 Enhancement | Background responses and `continuation_token` support added to Python agent responses | [#3808](https://github.com/microsoft/agent-framework/pull/3808) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🟡 Enhancement | Session/context preview types added side-by-side (`SessionContext`, `BaseContextProvider`) | [#3763](https://github.com/microsoft/agent-framework/pull/3763) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🟡 Enhancement | Streaming code-interpreter updates now include incremental code deltas | [#3775](https://github.com/microsoft/agent-framework/pull/3775) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🟡 Enhancement | `@tool` decorator adds explicit schema handling support | [#3734](https://github.com/microsoft/agent-framework/pull/3734) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | `register_executor()`/`register_agent()` removed from `WorkflowBuilder`; use instances directly, helper methods for state isolation | [#3781](https://github.com/microsoft/agent-framework/pull/3781) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | `ChatAgent` → `Agent`, `ChatMessage` → `Message`, `RawChatAgent` → `RawAgent`, `ChatClientProtocol` → `SupportsChatGetResponse` | [#3747](https://github.com/microsoft/agent-framework/pull/3747) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | Types API review: `Role`/`FinishReason` type changes, response/update constructor tightening, helper renames to `from_updates`, and removal of `try_parse_value` | [#3647](https://github.com/microsoft/agent-framework/pull/3647) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | APIs unified around `run`/`get_response` and `ResponseStream` | [#3379](https://github.com/microsoft/agent-framework/pull/3379) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | `AgentRunContext` renamed to `AgentContext` | [#3714](https://github.com/microsoft/agent-framework/pull/3714) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | `AgentProtocol` renamed to `SupportsAgentRun` | [#3717](https://github.com/microsoft/agent-framework/pull/3717) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | Middleware `next` parameter renamed to `call_next` | [#3735](https://github.com/microsoft/agent-framework/pull/3735) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | TypeVar naming standardized (`TName` → `NameT`) | [#3770](https://github.com/microsoft/agent-framework/pull/3770) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | Workflow-as-agent output/stream behavior aligned with current agent response flow | [#3649](https://github.com/microsoft/agent-framework/pull/3649) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | Fluent builder methods moved to constructor parameters across 6 builders | [#3693](https://github.com/microsoft/agent-framework/pull/3693) |
| 1.0.0b260210 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260210) | 🔴 Breaking | Workflow events unified into single `WorkflowEvent` with `type` discriminator; `isinstance()` → `event.type == "..."` | [#3690](https://github.com/microsoft/agent-framework/pull/3690) |
| 1.0.0b260130 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260130) | 🟡 Enhancement | `ChatOptions`/`ChatResponse`/`AgentResponse` generic over response format | [#3305](https://github.com/microsoft/agent-framework/pull/3305) |
| 1.0.0b260130 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260130) | 🟡 Enhancement | `BaseAgent` support added for Claude Agent SDK integrations | [#3509](https://github.com/microsoft/agent-framework/pull/3509) |
| 1.0.0b260128 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260128) | 🔴 Breaking | `AIFunction` → `FunctionTool`, `@ai_function` → `@tool` | [#3413](https://github.com/microsoft/agent-framework/pull/3413) |
| 1.0.0b260128 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260128) | 🔴 Breaking | Factory pattern for GroupChat/Magentic; `with_standard_manager` → `with_manager`, `participant_factories` → `register_participant` | [#3224](https://github.com/microsoft/agent-framework/pull/3224) |
| 1.0.0b260128 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260128) | 🔴 Breaking | `Github` → `GitHub` | [#3486](https://github.com/microsoft/agent-framework/pull/3486) |
| 1.0.0b260127 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260127) | 🟡 Enhancement | `BaseAgent` support added for GitHub Copilot SDK integrations | [#3404](https://github.com/microsoft/agent-framework/pull/3404) |
| 1.0.0b260123 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260123) | 🔴 Breaking | Content types consolidated to single `Content` class with classmethods | [#3252](https://github.com/microsoft/agent-framework/pull/3252) |
| 1.0.0b260123 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260123) | 🔴 Breaking | `response_format` validation errors now raise `ValidationError` | [#3274](https://github.com/microsoft/agent-framework/pull/3274) |
| 1.0.0b260123 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260123) | 🔴 Breaking | AG-UI run logic simplified | [#3322](https://github.com/microsoft/agent-framework/pull/3322) |
| 1.0.0b260123 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260123) | 🟡 Enhancement | Anthropic client adds `response_format` support for structured outputs | [#3301](https://github.com/microsoft/agent-framework/pull/3301) |
| 1.0.0b260123 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260123) | 🟡 Enhancement | Azure AI configuration expanded with `reasoning` and `rai_config` support | [#3403](https://github.com/microsoft/agent-framework/pull/3403), [#3265](https://github.com/microsoft/agent-framework/pull/3265) |
| 1.0.0b260116 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260116) | 🔴 Breaking | `create_agent` → `as_agent` | [#3249](https://github.com/microsoft/agent-framework/pull/3249) |
| 1.0.0b260116 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260116) | 🔴 Breaking | `source_executor_id` → `executor_id` | [#3166](https://github.com/microsoft/agent-framework/pull/3166) |
| 1.0.0b260116 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260116) | 🟡 Enhancement | AG-UI supports service-managed session/thread continuity | [#3136](https://github.com/microsoft/agent-framework/pull/3136) |
| 1.0.0b260114 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260114) | 🔴 Breaking | Orchestrations refactored (GroupChat, Handoff, Sequential, Concurrent) | [#3023](https://github.com/microsoft/agent-framework/pull/3023) |
| 1.0.0b260114 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260114) | 🔴 Breaking | Options as TypedDict and Generic | [#3140](https://github.com/microsoft/agent-framework/pull/3140) |
| 1.0.0b260114 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260114) | 🔴 Breaking | `display_name` removed; `context_providers` → `context_provider` (singular); `middleware` must be list | [#3139](https://github.com/microsoft/agent-framework/pull/3139) |
| 1.0.0b260114 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260114) | 🔴 Breaking | `AgentRunResponse`/`AgentRunResponseUpdate` renamed to `AgentResponse`/`AgentResponseUpdate` | [#3207](https://github.com/microsoft/agent-framework/pull/3207) |
| 1.0.0b260114 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260114) | 🟡 Enhancement | Declarative workflow runtime added for YAML-defined workflows | [#2815](https://github.com/microsoft/agent-framework/pull/2815) |
| 1.0.0b260114 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260114) | 🟡 Enhancement | MCP loading/reliability improvements (connection-loss handling, pagination, representation controls) | [#3154](https://github.com/microsoft/agent-framework/pull/3154) |
| 1.0.0b260114 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260114) | 🟡 Enhancement | Foundry `A2ATool` supports connections without explicit target URL | [#3127](https://github.com/microsoft/agent-framework/pull/3127) |
| 1.0.0b260107 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260107) | — | No significant changes | — |
| 1.0.0b260106 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260106) | — | No significant changes | — |

## Next steps

> [!div class="nextstepaction"]
> [Support overview](../index.md)
