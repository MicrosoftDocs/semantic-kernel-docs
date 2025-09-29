---
title: Agent Memory
description: Learn how to use memory with Agent Framework
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: reference
ms.author: markwallace
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# Agent Memory

Agent memory is a crucial capability that allows agents to maintain context across conversations, remember user preferences, and provide personalized experiences. The Agent Framework provides multiple memory mechanisms to suit different use cases, from simple in-memory storage to persistent databases and specialized memory services.

::: zone pivot="programming-language-csharp"

Documentation coming soon.

::: zone-end
::: zone pivot="programming-language-python"

## Memory Types

The Agent Framework supports several types of memory to accommodate different use cases:

### In-Memory Storage (Default)
The simplest form of memory where conversation history is stored in memory during the application runtime. This is the default behavior and requires no additional configuration.

```python
from agent_framework import ChatAgent
from agent_framework.openai import OpenAIChatClient

# Default behavior - uses in-memory storage
agent = ChatAgent(
    chat_client=OpenAIChatClient(),
    instructions="You are a helpful assistant."
)

thread = agent.get_new_thread()
# Conversation history is maintained in memory for this thread
```

### Persistent Message Stores
For applications that need to persist conversation history across sessions, the framework provides `ChatMessageStore` implementations:

#### Built-in ChatMessageStore
The default in-memory implementation that can be serialized:

```python
from agent_framework import ChatMessageStore

# Create a custom message store
def create_message_store():
    return ChatMessageStore()

agent = ChatAgent(
    chat_client=OpenAIChatClient(),
    instructions="You are a helpful assistant.",
    chat_message_store_factory=create_message_store
)
```

#### Redis Message Store
For production applications requiring persistent storage:

```python
from agent_framework.redis import RedisChatMessageStore

def create_redis_store():
    return RedisChatMessageStore(
        redis_url="redis://localhost:6379",
        thread_id="user_session_123",
        max_messages=100  # Keep last 100 messages
    )

agent = ChatAgent(
    chat_client=OpenAIChatClient(),
    instructions="You are a helpful assistant.",
    chat_message_store_factory=create_redis_store
)
```

#### Custom Message Store
You can implement your own storage backend by implementing the `ChatMessageStoreProtocol`:

```python
from agent_framework import ChatMessage, ChatMessageStoreProtocol
from typing import Any
from collections.abc import Sequence

class DatabaseMessageStore(ChatMessageStoreProtocol):
    def __init__(self, connection_string: str):
        self.connection_string = connection_string
        self._messages: list[ChatMessage] = []
    
    async def add_messages(self, messages: Sequence[ChatMessage]) -> None:
        """Add messages to database."""
        # Implement database insertion logic
        self._messages.extend(messages)
    
    async def list_messages(self) -> list[ChatMessage]:
        """Retrieve messages from database."""
        # Implement database query logic
        return self._messages
    
    async def serialize(self, **kwargs: Any) -> Any:
        """Serialize store state for persistence."""
        return {"connection_string": self.connection_string}
    
    async def update_from_state(self, serialized_store_state: Any, **kwargs: Any) -> None:
        """Update store from serialized state."""
        if serialized_store_state:
            self.connection_string = serialized_store_state["connection_string"]
```

### Context Providers (Dynamic Memory)
Context providers enable sophisticated memory patterns by injecting relevant context before each agent invocation:

#### Basic Context Provider
```python
from agent_framework import ContextProvider, Context, ChatMessage
from collections.abc import MutableSequence
from typing import Any

class UserPreferencesMemory(ContextProvider):
    def __init__(self):
        self.preferences = {}
    
    async def invoking(self, messages: ChatMessage | MutableSequence[ChatMessage], **kwargs: Any) -> Context:
        """Provide user preferences before each invocation."""
        if self.preferences:
            preferences_text = ", ".join([f"{k}: {v}" for k, v in self.preferences.items()])
            instructions = f"User preferences: {preferences_text}"
            return Context(instructions=instructions)
        return Context()
    
    async def invoked(
        self,
        request_messages: ChatMessage | Sequence[ChatMessage],
        response_messages: ChatMessage | Sequence[ChatMessage] | None = None,
        invoke_exception: Exception | None = None,
        **kwargs: Any,
    ) -> None:
        """Extract and store user preferences from the conversation."""
        # Implement preference extraction logic
        pass
```

#### External Memory Services
The framework supports integration with specialized memory services like Mem0:

```python
from agent_framework.mem0 import Mem0Provider

# Using Mem0 for advanced memory capabilities
memory_provider = Mem0Provider(
    api_key="your-mem0-api-key",
    user_id="user_123",
    application_id="my_app"
)

agent = ChatAgent(
    chat_client=OpenAIChatClient(),
    instructions="You are a helpful assistant with memory.",
    context_providers=memory_provider
)
```

### Thread Serialization and Persistence
The framework supports serializing entire thread states for persistence across application restarts:

```python
import json

# Create agent and thread
agent = ChatAgent(chat_client=OpenAIChatClient())
thread = agent.get_new_thread()

# Have conversation
await agent.run("Hello, my name is Alice", thread=thread)

# Serialize thread state
serialized_thread = await thread.serialize()
# Save to file/database
with open("thread_state.json", "w") as f:
    json.dump(serialized_thread, f)

# Later, restore the thread
with open("thread_state.json", "r") as f:
    thread_data = json.load(f)

restored_thread = await agent.deserialize_thread(thread_data)
# Continue conversation with full context
await agent.run("What's my name?", thread=restored_thread)
```

::: zone-end
