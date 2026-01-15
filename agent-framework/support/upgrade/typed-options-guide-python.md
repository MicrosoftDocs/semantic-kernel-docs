---
title: Upgrade Guide - Chat Client and Chat Agent options through TypedDicts
description: Guide on upgrading chat client and chat agent options to use TypedDicts in the Agent Framework.
author: eavanvalkenburg
ms.topic: tutorial
ms.author: edvan
ms.date: 01/15/2026
ms.service: agent-framework
---

# Upgrade Guide: Chat Options as TypedDict with Generics

This guide helps you upgrade your Python code to the new TypedDict-based `Options` system introduced in version [1.0.0b260114](https://github.com/microsoft/agent-framework/releases/tag/python-1.0.0b260114) of the Microsoft Agent Framework. This is a **breaking change** that provides improved type safety, IDE autocomplete, and runtime extensibility.

## Overview of Changes

This release introduces a major refactoring of how options are passed to chat clients and chat agents.

### How It Worked Before

Previously, options were passed as **direct keyword arguments** on methods like `get_response()`, `get_streaming_response()`, `run()`, and agent constructors:

```python
# Options were individual keyword arguments
response = await client.get_response(
    "Hello!",
    model_id="gpt-4",
    temperature=0.7,
    max_tokens=1000,
)

# For provider-specific options not in the base set, you used additional_properties
response = await client.get_response(
    "Hello!",
    model_id="gpt-4",
    additional_properties={"reasoning_effort": "medium"},
)
```

### How It Works Now

Most options are now passed through a single `options` parameter as a typed dictionary:

```python
# Most options go in a single typed dict
response = await client.get_response(
    "Hello!",
    options={
        "model_id": "gpt-4",
        "temperature": 0.7,
        "max_tokens": 1000,
        "reasoning_effort": "medium",  # Provider-specific options included directly
    },
)
```

> **Note:** For **Agents**, the `instructions` and `tools` parameters remain available as direct keyword arguments on `ChatAgent.__init__()` and `client.create_agent()`. For `agent.run()`, only `tools` is available as a keyword argument:
>
> ```python
> # Agent creation accepts both tools and instructions as keyword arguments
> agent = ChatAgent(
>     chat_client=client,
>     tools=[my_function],
>     instructions="You are a helpful assistant.",
>     default_options={"model_id": "gpt-4", "temperature": 0.7},
> )
>
> # agent.run() only accepts tools as a keyword argument
> response = await agent.run(
>     "Hello!",
>     tools=[another_function],  # Can override tools per-run
> )
> ```

### Key Changes

1. **Consolidated Options Parameter**: Most keyword arguments (`model_id`, `temperature`, etc.) are now passed via a single `options` dict
2. **Exception for Agent Creation**: `instructions` and `tools` remain available as direct keyword arguments on `ChatAgent.__init__()` and `create_agent()`
3. **Exception for Agent Run**: `tools` remains available as a direct keyword argument on `agent.run()`
4. **TypedDict-based Options**: Options are defined as `TypedDict` classes for type safety
5. **Generic Type Support**: Chat clients and agents support generics for provider-specific options, to allow runtime overloads
6. **Provider-specific Options**: Each provider has its own default TypedDict (e.g., `OpenAIChatOptions`, `OllamaChatOptions`)
7. **No More additional_properties**: Provider-specific parameters are now first-class typed fields

### Benefits

- **Type Safety**: IDE autocomplete and type checking for all options
- **Provider Flexibility**: Support for provider-specific parameters on day one
- **Cleaner Code**: Consistent dict-based parameter passing
- **Easier Extension**: Create custom options for specialized use cases (e.g., reasoning models or other API backends)

## Migration Guide

### 1. Convert Keyword Arguments to Options Dict

The most common change is converting individual keyword arguments to the `options` dictionary.

**Before (keyword arguments):**

```python
from agent_framework.openai import OpenAIChatClient

client = OpenAIChatClient()

# Options passed as individual keyword arguments
response = await client.get_response(
    "Hello!",
    model_id="gpt-4",
    temperature=0.7,
    max_tokens=1000,
)

# Streaming also used keyword arguments
async for chunk in client.get_streaming_response(
    "Tell me a story",
    model_id="gpt-4",
    temperature=0.9,
):
    print(chunk.text, end="")
```

**After (options dict):**

```python
from agent_framework.openai import OpenAIChatClient

client = OpenAIChatClient()

# All options now go in a single 'options' parameter
response = await client.get_response(
    "Hello!",
    options={
        "model_id": "gpt-4",
        "temperature": 0.7,
        "max_tokens": 1000,
    },
)

# Same pattern for streaming
async for chunk in client.get_streaming_response(
    "Tell me a story",
    options={
        "model_id": "gpt-4",
        "temperature": 0.9,
    },
):
    print(chunk.text, end="")
```

If you pass options that are not appropriate for that client, you will get a type error in your IDE.

### 2. Using Provider-Specific Options (No More additional_properties)

Previously, to pass provider-specific parameters that weren't part of the base set of keyword arguments, you had to use the `additional_properties` parameter:

**Before (using additional_properties):**

```python
from agent_framework.openai import OpenAIChatClient

client = OpenAIChatClient()
response = await client.get_response(
    "What is 2 + 2?",
    model_id="gpt-4",
    temperature=0.7,
    additional_properties={
        "reasoning_effort": "medium",  # No type checking or autocomplete
    },
)
```

**After (direct options with TypedDict):**

```python
from agent_framework.openai import OpenAIChatClient

# Provider-specific options are now first-class citizens with full type support
client = OpenAIChatClient()
response = await client.get_response(
    "What is 2 + 2?",
    options={
        "model_id": "gpt-4",
        "temperature": 0.7,
        "reasoning_effort": "medium",  # Type checking or autocomplete
    },
)
```

**After (custom subclassing for new parameters):**

Or if it is a parameter that is not yet part of Agent Framework (because it is new, or because it is custom for a OpenAI compatible backend), you can now subclass the options and use the generic support:

```python
from typing import Literal
from agent_framework.openai import OpenAIChatOptions, OpenAIChatClient

class MyCustomOpenAIChatOptions(OpenAIChatOptions, total=False):
    """Custom OpenAI chat options with additional parameters."""

    # New or custom parameters
    custom_param: str

# Use with the client
client = OpenAIChatClient[MyCustomOpenAIChatOptions]()
response = await client.get_response(
    "Hello!",
    options={
        "model_id": "gpt-4",
        "temperature": 0.7,
        "custom_param": "my_value",  # IDE autocomplete works!
    },
)
```

The key benefit is that most provider-specific parameters are now part of the typed options dictionary, giving you:
- **IDE autocomplete** for all available options
- **Type checking** to catch invalid keys or values
- **No need for additional_properties** for known provider parameters
- **Easy extension** for custom or new parameters

### 3. Update ChatAgent Configuration

ChatAgent initialization and run methods follow the same pattern:

**Before (keyword arguments on constructor and run):**

```python
from agent_framework import ChatAgent
from agent_framework.openai import OpenAIChatClient

client = OpenAIChatClient()

# Default options as keyword arguments on constructor
agent = ChatAgent(
    chat_client=client,
    name="assistant",
    model_id="gpt-4",
    temperature=0.7,
)

# Run also took keyword arguments
response = await agent.run(
    "Hello!",
    max_tokens=1000,
)
```

**After:**

```python
from agent_framework import ChatAgent
from agent_framework.openai import OpenAIChatClient, OpenAIChatOptions

client = OpenAIChatClient()
agent = ChatAgent(
    chat_client=client,
    name="assistant",
    default_options={ # <- type checkers will verify this dict
        "model_id": "gpt-4",
        "temperature": 0.7,
    },
)

response = await agent.run("Hello!", options={ # <- and this dict too
    "max_tokens": 1000,
})
```

### 4. Provider-Specific Options

Each provider now has its own TypedDict for options, these are enabled by default. This allows you to use provider-specific parameters with full type safety:

**OpenAI Example:**

```python
from agent_framework.openai import OpenAIChatClient

client = OpenAIChatClient()
response = await client.get_response(
    "Hello!",
    options={
        "model_id": "gpt-4",
        "temperature": 0.7,
        "reasoning_effort": "medium",
    },
)
```

But you can also make it explicit:

```python
from agent_framework_anthropic import AnthropicClient, AnthropicChatOptions

client = AnthropicClient[AnthropicChatOptions]()
response = await client.get_response(
    "Hello!",
    options={
        "model_id": "claude-3-opus-20240229",
        "max_tokens": 1000,
    },
)
```


### 5. Creating Custom Options for Specialized Models

One powerful feature of the new system is the ability to create custom TypedDict options for specialized models. This is particularly useful for models that have unique parameters, such as reasoning models with OpenAI:

```python
from typing import Literal
from agent_framework.openai import OpenAIChatOptions, OpenAIChatClient

class OpenAIReasoningChatOptions(OpenAIChatOptions, total=False):
    """Chat options for OpenAI reasoning models (o1, o3, o4-mini, etc.)."""

    # Reasoning-specific parameters
    reasoning_effort: Literal["none", "minimal", "low", "medium", "high", "xhigh"]

    # Unsupported parameters for reasoning models (override with None)
    temperature: None
    top_p: None
    frequency_penalty: None
    presence_penalty: None
    logit_bias: None
    logprobs: None
    top_logprobs: None
    stop: None


# Use with the client
client = OpenAIChatClient[OpenAIReasoningChatOptions]()
response = await client.get_response(
    "What is 2 + 2?",
    options={
        "model_id": "o3",
        "max_tokens": 100,
        "allow_multiple_tool_calls": True,
        "reasoning_effort": "medium",  # IDE autocomplete works!
        # "temperature": 0.7,  # Would raise a type error, because the value is not None
    },
)
```

### 6. Chat Agents with Options

The generic setup has also been extended to Chat Agents:

```python
from agent_framework import ChatAgent
from agent_framework.openai import OpenAIChatClient

agent = ChatAgent(
    chat_client=OpenAIChatClient[OpenAIReasoningChatOptions](),
    default_options={
        "model_id": "o3",
        "max_tokens": 100,
        "allow_multiple_tool_calls": True,
        "reasoning_effort": "medium",
    },
)
```
and you can specify the generic on both the client and the agent, so this is also valid:

```python
from agent_framework import ChatAgent
from agent_framework.openai import OpenAIChatClient

agent = ChatAgent[OpenAIReasoningChatOptions](
    chat_client=OpenAIChatClient(),
    default_options={
        "model_id": "o3",
        "max_tokens": 100,
        "allow_multiple_tool_calls": True,
        "reasoning_effort": "medium",
    },
)
```

### 6. Update Custom Chat Client Implementations

If you have implemented a custom chat client by extending `BaseChatClient`, update the internal methods:

**Before:**

```python
from agent_framework import BaseChatClient, ChatMessage, ChatOptions, ChatResponse

class MyCustomClient(BaseChatClient):
    async def _inner_get_response(
        self,
        *,
        messages: MutableSequence[ChatMessage],
        chat_options: ChatOptions,
        **kwargs: Any,
    ) -> ChatResponse:
        # Access options via class attributes
        model = chat_options.model_id
        temp = chat_options.temperature
        # ...
```

**After:**

```python
from typing import Generic
from agent_framework import BaseChatClient, ChatMessage, ChatOptions, ChatResponse

# Define your provider's options TypedDict
class MyCustomChatOptions(ChatOptions, total=False):
    my_custom_param: str

# This requires the TypeVar from Python 3.13+ or from typing_extensions, so for Python 3.13+:
from typing import TypeVar

TOptions = TypeVar("TOptions", bound=TypedDict, default=MyCustomChatOptions, covariant=True)

class MyCustomClient(BaseChatClient[TOptions], Generic[TOptions]):
    async def _inner_get_response(
        self,
        *,
        messages: MutableSequence[ChatMessage],
        options: dict[str, Any],  # Note: parameter renamed and just a dict
        **kwargs: Any,
    ) -> ChatResponse:
        # Access options via dict access
        model = options.get("model_id")
        temp = options.get("temperature")
        # ...
```

## Common Migration Patterns

### Pattern 1: Simple Parameter Update

```python
# Before - keyword arguments
await client.get_response("Hello", temperature=0.7)

# After - options dict
await client.get_response("Hello", options={"temperature": 0.7})
```

### Pattern 2: Multiple Parameters

```python
# Before - multiple keyword arguments
await client.get_response(
    "Hello",
    model_id="gpt-4",
    temperature=0.7,
    max_tokens=1000,
)

# After - all in options dict
await client.get_response(
    "Hello",
    options={
        "model_id": "gpt-4",
        "temperature": 0.7,
        "max_tokens": 1000,
    },
)
```

### Pattern 3: Chat Client with Tools

For chat clients, `tools` now goes in the options dict:

```python
# Before - tools as keyword argument on chat client
await client.get_response(
    "What's the weather?",
    model_id="gpt-4",
    tools=[my_function],
    tool_choice="auto",
)

# After - tools in options dict for chat clients
await client.get_response(
    "What's the weather?",
    options={
        "model_id": "gpt-4",
        "tools": [my_function],
        "tool_choice": "auto",
    },
)
```

### Pattern 4: Agent with Tools and Instructions

For agent creation, `tools` and `instructions` can remain as keyword arguments. For `run()`, only `tools` is available:

```python
# Before
agent = ChatAgent(
    chat_client=client,
    name="assistant",
    tools=[my_function],
    instructions="You are helpful.",
    model_id="gpt-4",
)

# After - tools and instructions stay as keyword args on creation
agent = ChatAgent(
    chat_client=client,
    name="assistant",
    tools=[my_function],  # Still a keyword argument!
    instructions="You are helpful.",  # Still a keyword argument!
    default_options={"model_id": "gpt-4"},
)

# For run(), only tools is available as keyword argument
response = await agent.run(
    "Hello!",
    tools=[another_function],  # Can override tools
    options={"max_tokens": 100},
)
```

```python
# Before - using additional_properties
await client.get_response(
    "Solve this problem",
    model_id="o3",
    additional_properties={"reasoning_effort": "high"},
)

# After - directly in options
await client.get_response(
    "Solve this problem",
    options={
        "model_id": "o3",
        "reasoning_effort": "high",
    },
)
```

### Pattern 5: Provider-Specific Parameters

```python
# Define reusable options
my_options: OpenAIChatOptions = {
    "model_id": "gpt-4",
    "temperature": 0.7,
}

# Use with different messages
await client.get_response("Hello", options=my_options)
await client.get_response("Goodbye", options=my_options)

# Extend options using dict merge
extended_options = {**my_options, "max_tokens": 500}
```

## Summary of Breaking Changes

| Aspect | Before | After |
|--------|--------|-------|
| Chat client options | Individual keyword arguments (`temperature=0.7`) | Single `options` dict (`options={"temperature": 0.7}`) |
| Chat client tools | `tools=[...]` keyword argument | `options={"tools": [...]}` |
| Agent creation `tools` and `instructions` | Keyword arguments | **Still keyword arguments** (unchanged) |
| Agent `run()` `tools` | Keyword argument | **Still keyword argument** (unchanged) |
| Agent `run()` `instructions` | Keyword argument | Moved to `options={"instructions": ...}` |
| Provider-specific options | `additional_properties={...}` | Included directly in `options` dict |
| Agent default options | Keyword arguments on constructor | `default_options={...}` |
| Agent run options | Keyword arguments on `run()` | `options={...}` parameter |
| Client typing | `OpenAIChatClient()` | `OpenAIChatClient[CustomOptions]()` (optional) |
| Agent typing | `ChatAgent(...)` | `ChatAgent[CustomOptions](...)` (optional) |

## Testing Your Migration

### ChatClient Updates

1. Find all calls to `get_response()` and `get_streaming_response()` that use keyword arguments like `model_id=`, `temperature=`, `tools=`, etc.
2. Move all keyword arguments into an `options={...}` dictionary
3. Move any `additional_properties` values directly into the `options` dict

### ChatAgent Updates

1. Find all `ChatAgent` constructors and `run()` calls that use keyword arguments
2. Move keyword arguments on constructors to `default_options={...}`
3. Move keyword arguments on `run()` to `options={...}`
4. **Exception**: `tools` and `instructions` can remain as keyword arguments on `ChatAgent.__init__()` and `create_agent()`
5. **Exception**: `tools` can remain as a keyword argument on `run()`

### Custom Chat Client Updates

1. Update the `_inner_get_response()` and `_inner_get_streaming_response()` method signatures: change `chat_options: ChatOptions` parameter to `options: dict[str, Any]`
2. Update attribute access (e.g., `chat_options.model_id`) to dict access (e.g., `options.get("model_id")`)
3. **(Optional)** If using non-standard parameters: Define a custom TypedDict
4. Add generic type parameters to your client class

### For All

1. **Run Type Checker**: Use `mypy` or `pyright` to catch type errors
2. **Test End-to-End**: Run your application to verify functionality

## IDE Support

The new TypedDict-based system provides excellent IDE support:

- **Autocomplete**: Get suggestions for all available options
- **Type Checking**: Catch invalid option keys at development time
- **Documentation**: Hover over keys to see descriptions
- **Provider-specific**: Each provider's options show only relevant parameters

## Next Steps

To see the typed dicts in action for the case of using OpenAI Reasoning Models with the Chat Completion API, explore [this sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/chat_client/typed_options.py)

After completing the migration:

1. Explore provider-specific options in the [API documentation](../../api-docs/TOC.yml)
2. Review updated [samples](https://github.com/microsoft/agent-framework/tree/main/python/samples)
3. Learn about creating [custom chat clients](../../user-guide/agents/agent-types/custom-agent.md)

For additional help, refer to the [Agent Framework documentation](../../overview/agent-framework-overview.md) or reach out to the community.
