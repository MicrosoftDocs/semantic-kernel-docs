---
title: Python 2026 Significant Changes Guide
description: Guide to significant changes in Python releases for Microsoft Agent Framework in 2026, including breaking changes and important enhancements.
author: eavanvalkenburg
ms.topic: upgrade-and-migration-article
ms.author: edvan
ms.date: 02/04/2026
ms.service: agent-framework
---
# Python 2026 Significant Changes Guide

This document lists all significant changes in Python releases since the start of 2026, including breaking changes and important enhancements that may affect your code. Each change is marked as:

- 🔴 **Breaking** — Requires code changes to upgrade
- 🟡 **Enhancement** — New capability or improvement; existing code continues to work

This document will be removed once we reach the 1.0.0 stable release, so please refer to it when upgrading between versions in 2026 to ensure you don't miss any important changes. For detailed upgrade instructions on specific topics (e.g., options migration), refer to the linked upgrade guides or the linked PR's.

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

No significant changes in this release.

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
from agent_framework.core import ChatAgent, AggregateContextProvider

agent = ChatAgent(
    name="my-agent",
    display_name="My Agent",
    context_providers=[provider1, provider2],
    middleware=my_middleware,  # single instance was allowed
)

aggregate = AggregateContextProvider([provider1, provider2])
```

**After:**
```python
from agent_framework.core import ChatAgent

# Only one context provider allowed; combine manually if needed
agent = ChatAgent(
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
| 1.0.0b260130 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260130) | 🟡 Enhancement | `ChatOptions`/`ChatResponse`/`AgentResponse` generic over response format | [#3305](https://github.com/microsoft/agent-framework/pull/3305) |
| 1.0.0b260128 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260128) | 🔴 Breaking | `AIFunction` → `FunctionTool`, `@ai_function` → `@tool` | [#3413](https://github.com/microsoft/agent-framework/pull/3413) |
| 1.0.0b260128 | | 🔴 Breaking | Factory pattern for GroupChat/Magentic; `with_standard_manager` → `with_manager`, `participant_factories` → `register_participant` | [#3224](https://github.com/microsoft/agent-framework/pull/3224) |
| 1.0.0b260128 | | 🔴 Breaking | `Github` → `GitHub` | [#3486](https://github.com/microsoft/agent-framework/pull/3486) |
| 1.0.0b260127 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260127) | — | No significant changes | — |
| 1.0.0b260123 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260123) | 🔴 Breaking | Content types consolidated to single `Content` class with classmethods | [#3252](https://github.com/microsoft/agent-framework/pull/3252) |
| 1.0.0b260123 | | 🔴 Breaking | `response_format` validation errors now raise `ValidationError` | [#3274](https://github.com/microsoft/agent-framework/pull/3274) |
| 1.0.0b260123 | | 🔴 Breaking | AG-UI run logic simplified | [#3322](https://github.com/microsoft/agent-framework/pull/3322) |
| 1.0.0b260116 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260116) | 🔴 Breaking | `create_agent` → `as_agent` | [#3249](https://github.com/microsoft/agent-framework/pull/3249) |
| 1.0.0b260116 | | 🔴 Breaking | `source_executor_id` → `executor_id` | [#3166](https://github.com/microsoft/agent-framework/pull/3166) |
| 1.0.0b260114 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260114) | 🔴 Breaking | Orchestrations refactored (GroupChat, Handoff, Sequential, Concurrent) | [#3023](https://github.com/microsoft/agent-framework/pull/3023) |
| 1.0.0b260114 | | 🔴 Breaking | Options as TypedDict and Generic | [#3140](https://github.com/microsoft/agent-framework/pull/3140) |
| 1.0.0b260114 | | 🔴 Breaking | `display_name` removed; `context_providers` → `context_provider` (singular); `middleware` must be list | [#3139](https://github.com/microsoft/agent-framework/pull/3139) |
| 1.0.0b260107 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260107) | — | No significant changes | — |
| 1.0.0b260106 | [Notes](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260106) | — | No significant changes | — |
