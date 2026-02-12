---
title: Agent Chat History and Memory
description: Learn how to use chat history and memory with Agent Framework
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: reference
ms.author: markwallace
ms.date: 09/24/2025
ms.service: agent-framework
---

# Agent Chat History and Memory

Agent chat history and memory are crucial capabilities that allow agents to maintain context across conversations, remember user preferences, and provide personalized experiences. The Agent Framework provides multiple features to suit different use cases, from simple in-memory chat message storage to persistent databases and specialized memory services.

::: zone pivot="programming-language-csharp"

## Chat History

Various chat history storage options are supported by Agent Framework. The available options vary by agent type and the underlying service(s) used to build the agent.

The two main supported scenarios are:

- **In-memory storage**: Agent is built on a service that doesn't support in-service storage of chat history (for example, OpenAI Chat Completion). By default, Agent Framework stores the full chat history in-memory in the `AgentSession` object, but developers can provide a custom `ChatHistoryProvider` implementation to store chat history in a third-party store if required.
- **In-service storage**: Agent is built on a service that requires in-service storage of chat history (for example, Azure AI Foundry Persistent Agents). Agent Framework stores the ID of the remote chat history in the `AgentSession` object, and no other chat history storage options are supported.

### In-memory chat history storage

When using a service that doesn't support in-service storage of chat history, Agent Framework defaults to storing chat history in-memory in the `AgentSession` object. In this case, the full chat history that's stored in the session object, plus any new messages, will be provided to the underlying service on each agent run. This design allows for a natural conversational experience with the agent. The caller only provides the new user message, and the agent only returns new answers. But the agent has access to the full conversation history and will use it when generating its response.

When using OpenAI Chat Completion as the underlying service for agents, the following code results in the session object containing the chat history from the agent run.

```csharp
AIAgent agent = new OpenAIClient("<your_api_key>")
     .GetChatClient(modelName)
     .AsAIAgent(JokerInstructions, JokerName);
AgentSession session = await agent.CreateSessionAsync();
Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate.", session));
```

Where messages are stored in memory, it's possible to retrieve the list of messages from the session and manipulate the messages directly if required.

```csharp
IList<ChatMessage>? messages = session.GetService<IList<ChatMessage>>();
```

> [!NOTE]
> Retrieving messages from the `AgentSession` object in this way only works if in-memory storage is being used.

#### Chat history reduction with in-memory storage

The built-in `InMemoryChatHistoryProvider` that's used by default when the underlying service does not support in-service storage,
can be configured with a reducer to manage the size of the chat history.
This is useful to avoid exceeding the context size limits of the underlying service.

The `InMemoryChatHistoryProvider` can take an optional `Microsoft.Extensions.AI.IChatReducer` implementation to reduce the size of the chat history.
It also allows you to configure the event during which the reducer is invoked, either after a message is added to the chat history
or before the chat history is returned for the next invocation.

To configure the `InMemoryChatHistoryProvider` with a reducer, you can provide a factory to construct a new `InMemoryChatHistoryProvider`
for each new `AgentSession` and pass it a reducer of your choice. The `InMemoryChatHistoryProvider` can also be passed an optional trigger event
which can be set to either `InMemoryChatHistoryProvider.ChatReducerTriggerEvent.AfterMessageAdded` or `InMemoryChatHistoryProvider.ChatReducerTriggerEvent.BeforeMessagesRetrieval`.

The factory is an async function that receives a context object and a cancellation token.

```csharp
AIAgent agent = new OpenAIClient("<your_api_key>")
    .GetChatClient(modelName)
    .AsAIAgent(new ChatClientAgentOptions
    {
        Name = JokerName,
        ChatOptions = new() { Instructions = JokerInstructions },
        ChatHistoryProviderFactory = (ctx, ct) => new ValueTask<ChatHistoryProvider>(
            new InMemoryChatHistoryProvider(
                new MessageCountingChatReducer(2),
                ctx.SerializedState,
                ctx.JsonSerializerOptions,
                InMemoryChatHistoryProvider.ChatReducerTriggerEvent.AfterMessageAdded))
    });
```

> [!NOTE]
> This feature is only supported when using the `InMemoryChatHistoryProvider`. When a service has in-service chat history storage, it is up to the service itself to manage the size of the chat history. Similarly, when using 3rd party storage (see below), it is up to the 3rd party storage solution to manage the chat history size. If you provide a `ChatHistoryProviderFactory` for a chat history provider but you use a service with built-in chat history storage, the factory will not be used.

### Inference service chat history storage

When using a service that requires in-service storage of chat history, Agent Framework stores the ID of the remote chat history in the `AgentSession` object.

For example, when using OpenAI Responses with store=true as the underlying service for agents, the following code will result in the session object containing the last response ID returned by the service.

```csharp
AIAgent agent = new OpenAIClient("<your_api_key>")
     .GetOpenAIResponseClient(modelName)
     .AsAIAgent(JokerInstructions, JokerName);
AgentSession session = await agent.CreateSessionAsync();
Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate.", session));
```

> [!NOTE]
> Some services, for example, OpenAI Responses support either in-service storage of chat history (store=true), or providing the full chat history on each invocation (store=false).
> Therefore, depending on the mode that the service is used in, Agent Framework will either default to storing the full chat history in memory, or storing an ID reference to the service stored chat history.

### Third-party chat history storage

When using a service that does not support in-service storage of chat history, Agent Framework allows developers to replace the default in-memory storage of chat history with third-party chat history storage. The developer is required to provide a subclass of the base abstract `ChatHistoryProvider` class.

The `ChatHistoryProvider` class defines the interface for storing and retrieving chat messages. Developers must implement the `InvokedAsync` and `InvokingAsync` methods to add messages to the remote store as they are generated, and retrieve messages from the remote store before invoking the underlying service.

The agent will use all messages returned by `InvokingAsync` when processing a user query. It is up to the implementer of `ChatHistoryProvider` to ensure that the size of the chat history does not exceed the context window of the underlying service.

When implementing a custom `ChatHistoryProvider` which stores chat history in a remote store, the chat history for that session should be stored under a key that is unique to that session. The `ChatHistoryProvider` implementation should generate this key and keep it in its state. `ChatHistoryProvider` has a `Serialize` method that can be overridden to serialize its state when the session is serialized. The `ChatHistoryProvider` should also provide a constructor that takes a <xref:System.Text.Json.JsonElement> as input to support deserialization of its state.

To supply a custom `ChatHistoryProvider` to a `ChatClientAgent`, you can use the `ChatHistoryProviderFactory` option when creating the agent.
Here is an example showing how to pass the custom implementation of `ChatHistoryProvider` to a `ChatClientAgent` that is based on Azure OpenAI Chat Completion.

The factory is an async function that receives a context object and a cancellation token.

```csharp
AIAgent agent = new AzureOpenAIClient(
    new Uri(endpoint),
    new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(new ChatClientAgentOptions
    {
        Name = JokerName,
        ChatOptions = new() { Instructions = JokerInstructions },
        ChatHistoryProviderFactory = (ctx, ct) => new ValueTask<ChatHistoryProvider>(
            // Create a new chat history provider for this agent that stores the messages in a custom store.
            // Each session must get its own copy of the CustomChatHistoryProvider, since the provider
            // also contains the ID that the session is stored under.
            new CustomChatHistoryProvider(
                vectorStore,
                ctx.SerializedState,
                ctx.JsonSerializerOptions))
    });
```

> [!TIP]
> For a detailed example on how to create a custom message store, see the [Storing Chat History in 3rd Party Storage](../../agents/conversations/persistent-storage.md) tutorial.

## Long term memory

The Agent Framework allows developers to provide custom components that can extract memories or provide memories to an agent.

To implement such a memory component, the developer needs to subclass the `AIContextProvider` abstract base class. This class has two core methods, `InvokingAsync` and `InvokedAsync`. When overridden, `InvokedAsync` allows developers to inspect all messages provided by users or generated by the agent. `InvokingAsync` allows developers to inject additional context for a specific agent run. System instructions, additional messages and additional functions can be provided.

> [!TIP]
> For a detailed example on how to create a custom memory component, see the [Adding Memory to an Agent](../../get-started/memory.md) tutorial.

## AgentSession Serialization

It is important to be able to persist an `AgentSession` object between agent invocations. This allows for situations where a user might ask a question of the agent, and take a long time to ask follow up questions. This allows the `AgentSession` state to survive service or app restarts.

Even if the chat history is stored in a remote store, the `AgentSession` object still contains an ID referencing the remote chat history. Losing the `AgentSession` state will therefore result in also losing the ID of the remote chat history.

The `AIAgent` provides the `SerializeSession` method to serialize session state, as well as a `DeserializeSessionAsync` method that re-creates a session from the serialized state. The `DeserializeSessionAsync` method re-creates the session with the `ChatHistoryProvider` and `AIContextProvider` configured on the agent.

```csharp
// Serialize the session state to a JsonElement, so it can be stored for later use.
JsonElement serializedSessionState = agent.SerializeSession(session);

// Re-create the session from the JsonElement.
AgentSession resumedSession = await agent.DeserializeSessionAsync(serializedSessionState);
```

> [!NOTE]
> `AgentSession` objects may contain more than just chat history, e.g. context providers may also store state in the session object. Therefore, it is important to always serialize, store and deserialize the entire `AgentSession` object to ensure that all state is preserved.
> [!IMPORTANT]
> Always treat `AgentSession` objects as opaque objects, unless you are very sure of the internals. The contents may vary not just by agent type, but also by service type and configuration.
> [!WARNING]
> Deserializing a session with a different agent than that which originally created it, or with an agent that has a different configuration than the original agent, might result in errors or unexpected behavior.

> [!TIP]
> See the [.NET samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples) for complete runnable examples.

::: zone-end
::: zone pivot="programming-language-python"

## Memory Types

The Agent Framework supports several types of memory to accommodate different use cases, including managing chat history as part of short term memory and providing extension points for extracting, storing and injecting long term memories into agents.

### In-Memory Storage (Default)

The simplest form of memory where conversation history is stored in memory during the application runtime. This is the default behavior and requires no additional configuration.

```python
from agent_framework import Agent
from agent_framework.openai import OpenAIChatClient

# Default behavior - uses in-memory storage
agent = Agent(
    chat_client=OpenAIChatClient(),
    instructions="You are a helpful assistant."
)

# Conversation history is maintained in memory for this thread
thread = agent.get_new_thread()

response = await agent.run("Hello, my name is Alice", thread=thread)
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

agent = Agent(
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

agent = Agent(
    chat_client=OpenAIChatClient(),
    instructions="You are a helpful assistant.",
    chat_message_store_factory=create_redis_store
)
```

#### Custom Message Store
You can implement your own storage backend by implementing the `ChatMessageStoreProtocol`:

```python
from agent_framework import Message, ChatMessageStoreProtocol
from typing import Any
from collections.abc import Sequence

class DatabaseMessageStore(ChatMessageStoreProtocol):
    def __init__(self, connection_string: str):
        self.connection_string = connection_string
        self._messages: list[Message] = []

    async def add_messages(self, messages: Sequence[Message]) -> None:
        """Add messages to database."""
        # Implement database insertion logic
        self._messages.extend(messages)

    async def list_messages(self) -> list[Message]:
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

> [!TIP]
> For a detailed example on how to create a custom message store, see the [Storing Chat History in 3rd Party Storage](../../agents/conversations/persistent-storage.md) tutorial.

### Context Providers (Dynamic Memory)
Context providers enable sophisticated memory patterns by injecting relevant context before each agent invocation:

#### Basic Context Provider
```python
from agent_framework import ContextProvider, Context, Message
from collections.abc import MutableSequence
from typing import Any

class UserPreferencesMemory(ContextProvider):
    def __init__(self):
        self.preferences = {}

    async def invoking(self, messages: Message | MutableSequence[Message], **kwargs: Any) -> Context:
        """Provide user preferences before each invocation."""
        if self.preferences:
            preferences_text = ", ".join([f"{k}: {v}" for k, v in self.preferences.items()])
            instructions = f"User preferences: {preferences_text}"
            return Context(instructions=instructions)
        return Context()

    async def invoked(
        self,
        request_messages: Message | Sequence[Message],
        response_messages: Message | Sequence[Message] | None = None,
        invoke_exception: Exception | None = None,
        **kwargs: Any,
    ) -> None:
        """Extract and store user preferences from the conversation."""
        # Implement preference extraction logic
        pass
```

> [!TIP]
> For a detailed example on how to create a custom memory component, see the [Adding Memory to an Agent](../../get-started/memory.md) tutorial.

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

agent = Agent(
    chat_client=OpenAIChatClient(),
    instructions="You are a helpful assistant with memory.",
    context_provider=memory_provider
)
```

### Thread Serialization and Persistence
The framework supports serializing entire thread states for persistence across application restarts:

```python
import json

# Create agent and thread
agent = Agent(chat_client=OpenAIChatClient())
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

### Complete Redis example

```python
# Copyright (c) Microsoft. All rights reserved.

import asyncio
import os
from uuid import uuid4

from agent_framework import AgentThread
from agent_framework.openai import OpenAIChatClient
from agent_framework.redis import RedisChatMessageStore

"""
Redis Chat Message Store Thread Example

This sample demonstrates how to use Redis as a chat message store for thread
management, enabling persistent conversation history storage across sessions
with Redis as the backend data store.
"""


async def example_manual_memory_store() -> None:
    """Basic example of using Redis chat message store."""
    print("=== Basic Redis Chat Message Store Example ===")

    # Create Redis store with auto-generated thread ID
    redis_store = RedisChatMessageStore(
        redis_url="redis://localhost:6379",
        # thread_id will be auto-generated if not provided
    )

    print(f"Created store with thread ID: {redis_store.thread_id}")

    # Create thread with Redis store
    thread = AgentThread(message_store=redis_store)

    # Create agent
    agent = OpenAIChatClient().as_agent(
        name="RedisBot",
        instructions="You are a helpful assistant that remembers our conversation using Redis.",
    )

    # Have a conversation
    print("\n--- Starting conversation ---")
    query1 = "Hello! My name is Alice and I love pizza."
    print(f"User: {query1}")
    response1 = await agent.run(query1, thread=thread)
    print(f"Agent: {response1.text}")

    query2 = "What do you remember about me?"
    print(f"User: {query2}")
    response2 = await agent.run(query2, thread=thread)
    print(f"Agent: {response2.text}")

    # Show messages are stored in Redis
    messages = await redis_store.list_messages()
    print(f"\nTotal messages in Redis: {len(messages)}")

    # Cleanup
    await redis_store.clear()
    await redis_store.aclose()
    print("Cleaned up Redis data\n")


async def example_user_session_management() -> None:
    """Example of managing user sessions with Redis."""
    print("=== User Session Management Example ===")

    user_id = "alice_123"
    session_id = f"session_{uuid4()}"

    # Create Redis store for specific user session
    def create_user_session_store():
        return RedisChatMessageStore(
            redis_url="redis://localhost:6379",
            thread_id=f"user_{user_id}_{session_id}",
            max_messages=10,  # Keep only last 10 messages
        )

    # Create agent with factory pattern
    agent = OpenAIChatClient().as_agent(
        name="SessionBot",
        instructions="You are a helpful assistant. Keep track of user preferences.",
        chat_message_store_factory=create_user_session_store,
    )

    # Start conversation
    thread = agent.get_new_thread()

    print(f"Started session for user {user_id}")
    if hasattr(thread.message_store, "thread_id"):
        print(f"Thread ID: {thread.message_store.thread_id}")  # type: ignore[union-attr]

    # Simulate conversation
    queries = [
        "Hi, I'm Alice and I prefer vegetarian food.",
        "What restaurants would you recommend?",
        "I also love Italian cuisine.",
        "Can you remember my food preferences?",
    ]

    for i, query in enumerate(queries, 1):
        print(f"\n--- Message {i} ---")
        print(f"User: {query}")
        response = await agent.run(query, thread=thread)
        print(f"Agent: {response.text}")

    # Show persistent storage
    if thread.message_store:
        messages = await thread.message_store.list_messages()  # type: ignore[union-attr]
        print(f"\nMessages stored for user {user_id}: {len(messages)}")

    # Cleanup
    if thread.message_store:
        await thread.message_store.clear()  # type: ignore[union-attr]
        await thread.message_store.aclose()  # type: ignore[union-attr]
    print("Cleaned up session data\n")


async def example_conversation_persistence() -> None:
    """Example of conversation persistence across application restarts."""
    print("=== Conversation Persistence Example ===")

    conversation_id = "persistent_chat_001"

    # Phase 1: Start conversation
    print("--- Phase 1: Starting conversation ---")
    store1 = RedisChatMessageStore(
        redis_url="redis://localhost:6379",
        thread_id=conversation_id,
    )

    thread1 = AgentThread(message_store=store1)
    agent = OpenAIChatClient().as_agent(
        name="PersistentBot",
        instructions="You are a helpful assistant. Remember our conversation history.",
    )

    # Start conversation
    query1 = "Hello! I'm working on a Python project about machine learning."
    print(f"User: {query1}")
    response1 = await agent.run(query1, thread=thread1)
    print(f"Agent: {response1.text}")

    query2 = "I'm specifically interested in neural networks."
    print(f"User: {query2}")
    response2 = await agent.run(query2, thread=thread1)
    print(f"Agent: {response2.text}")

    print(f"Stored {len(await store1.list_messages())} messages in Redis")
    await store1.aclose()

    # Phase 2: Resume conversation (simulating app restart)
    print("\n--- Phase 2: Resuming conversation (after 'restart') ---")
    store2 = RedisChatMessageStore(
        redis_url="redis://localhost:6379",
        thread_id=conversation_id,  # Same thread ID
    )

    thread2 = AgentThread(message_store=store2)

    # Continue conversation - agent should remember context
    query3 = "What was I working on before?"
    print(f"User: {query3}")
    response3 = await agent.run(query3, thread=thread2)
    print(f"Agent: {response3.text}")

    query4 = "Can you suggest some Python libraries for neural networks?"
    print(f"User: {query4}")
    response4 = await agent.run(query4, thread=thread2)
    print(f"Agent: {response4.text}")

    print(f"Total messages after resuming: {len(await store2.list_messages())}")

    # Cleanup
    await store2.clear()
    await store2.aclose()
    print("Cleaned up persistent data\n")


async def example_thread_serialization() -> None:
    """Example of thread state serialization and deserialization."""
    print("=== Thread Serialization Example ===")

    # Create initial thread with Redis store
    original_store = RedisChatMessageStore(
        redis_url="redis://localhost:6379",
        thread_id="serialization_test",
        max_messages=50,
    )

    original_thread = AgentThread(message_store=original_store)

    agent = OpenAIChatClient().as_agent(
        name="SerializationBot",
        instructions="You are a helpful assistant.",
    )

    # Have initial conversation
    print("--- Initial conversation ---")
    query1 = "Hello! I'm testing serialization."
    print(f"User: {query1}")
    response1 = await agent.run(query1, thread=original_thread)
    print(f"Agent: {response1.text}")

    # Serialize thread state
    serialized_thread = await original_thread.serialize()
    print(f"\nSerialized thread state: {serialized_thread}")

    # Close original connection
    await original_store.aclose()

    # Deserialize thread state (simulating loading from database/file)
    print("\n--- Deserializing thread state ---")

    # Create a new thread with the same Redis store type
    # This ensures the correct store type is used for deserialization
    restored_store = RedisChatMessageStore(redis_url="redis://localhost:6379")
    restored_thread = await AgentThread.deserialize(serialized_thread, message_store=restored_store)

    # Continue conversation with restored thread
    query2 = "Do you remember what I said about testing?"
    print(f"User: {query2}")
    response2 = await agent.run(query2, thread=restored_thread)
    print(f"Agent: {response2.text}")

    # Cleanup
    if restored_thread.message_store:
        await restored_thread.message_store.clear()  # type: ignore[union-attr]
        await restored_thread.message_store.aclose()  # type: ignore[union-attr]
    print("Cleaned up serialization test data\n")


async def example_message_limits() -> None:
    """Example of automatic message trimming with limits."""
    print("=== Message Limits Example ===")

    # Create store with small message limit
    store = RedisChatMessageStore(
        redis_url="redis://localhost:6379",
        thread_id="limits_test",
        max_messages=3,  # Keep only 3 most recent messages
    )

    thread = AgentThread(message_store=store)
    agent = OpenAIChatClient().as_agent(
        name="LimitBot",
        instructions="You are a helpful assistant with limited memory.",
    )

    # Send multiple messages to test trimming
    messages = [
        "Message 1: Hello!",
        "Message 2: How are you?",
        "Message 3: What's the weather?",
        "Message 4: Tell me a joke.",
        "Message 5: This should trigger trimming.",
    ]

    for i, query in enumerate(messages, 1):
        print(f"\n--- Sending message {i} ---")
        print(f"User: {query}")
        response = await agent.run(query, thread=thread)
        print(f"Agent: {response.text}")

        stored_messages = await store.list_messages()
        print(f"Messages in store: {len(stored_messages)}")
        if len(stored_messages) > 0:
            print(f"Oldest message: {stored_messages[0].text[:30]}...")

    # Final check
    final_messages = await store.list_messages()
    print(f"\nFinal message count: {len(final_messages)} (should be <= 6: 3 messages × 2 per exchange)")

    # Cleanup
    await store.clear()
    await store.aclose()
    print("Cleaned up limits test data\n")


async def main() -> None:
    """Run all Redis chat message store examples."""
    print("Redis Chat Message Store Examples")
    print("=" * 50)
    print("Prerequisites:")
    print("- Redis server running on localhost:6379")
    print("- OPENAI_API_KEY environment variable set")
    print("=" * 50)

    # Check prerequisites
    if not os.getenv("OPENAI_API_KEY"):
        print("ERROR: OPENAI_API_KEY environment variable not set")
        return

    try:
        # Test Redis connection
        test_store = RedisChatMessageStore(redis_url="redis://localhost:6379")
        connection_ok = await test_store.ping()
        await test_store.aclose()
        if not connection_ok:
            raise Exception("Redis ping failed")
        print("✓ Redis connection successful\n")
    except Exception as e:
        print(f"ERROR: Cannot connect to Redis: {e}")
        print("Please ensure Redis is running on localhost:6379")
        return

    try:
        # Run all examples
        await example_manual_memory_store()
        await example_user_session_management()
        await example_conversation_persistence()
        await example_thread_serialization()
        await example_message_limits()

        print("All examples completed successfully!")

    except Exception as e:
        print(f"Error running examples: {e}")
        raise


if __name__ == "__main__":
    asyncio.run(main())
```

```python
# Copyright (c) Microsoft. All rights reserved.

import asyncio
import os
from uuid import uuid4

from agent_framework import AgentThread
from agent_framework.openai import OpenAIChatClient
from agent_framework.redis import RedisChatMessageStore

"""
Redis Chat Message Store Thread Example

This sample demonstrates how to use Redis as a chat message store for thread
management, enabling persistent conversation history storage across sessions
with Redis as the backend data store.
"""


async def example_manual_memory_store() -> None:
    """Basic example of using Redis chat message store."""
    print("=== Basic Redis Chat Message Store Example ===")

    # Create Redis store with auto-generated thread ID
    redis_store = RedisChatMessageStore(
        redis_url="redis://localhost:6379",
        # thread_id will be auto-generated if not provided
    )

    print(f"Created store with thread ID: {redis_store.thread_id}")

    # Create thread with Redis store
    thread = AgentThread(message_store=redis_store)

    # Create agent
    agent = OpenAIChatClient().as_agent(
        name="RedisBot",
        instructions="You are a helpful assistant that remembers our conversation using Redis.",
    )

    # Have a conversation
    print("\n--- Starting conversation ---")
    query1 = "Hello! My name is Alice and I love pizza."
    print(f"User: {query1}")
    response1 = await agent.run(query1, thread=thread)
    print(f"Agent: {response1.text}")

    query2 = "What do you remember about me?"
    print(f"User: {query2}")
    response2 = await agent.run(query2, thread=thread)
    print(f"Agent: {response2.text}")

    # Show messages are stored in Redis
    messages = await redis_store.list_messages()
    print(f"\nTotal messages in Redis: {len(messages)}")

    # Cleanup
    await redis_store.clear()
    await redis_store.aclose()
    print("Cleaned up Redis data\n")


async def example_user_session_management() -> None:
    """Example of managing user sessions with Redis."""
    print("=== User Session Management Example ===")

    user_id = "alice_123"
    session_id = f"session_{uuid4()}"

    # Create Redis store for specific user session
    def create_user_session_store():
        return RedisChatMessageStore(
            redis_url="redis://localhost:6379",
            thread_id=f"user_{user_id}_{session_id}",
            max_messages=10,  # Keep only last 10 messages
        )

    # Create agent with factory pattern
    agent = OpenAIChatClient().as_agent(
        name="SessionBot",
        instructions="You are a helpful assistant. Keep track of user preferences.",
        chat_message_store_factory=create_user_session_store,
    )

    # Start conversation
    thread = agent.get_new_thread()

    print(f"Started session for user {user_id}")
    if hasattr(thread.message_store, "thread_id"):
        print(f"Thread ID: {thread.message_store.thread_id}")  # type: ignore[union-attr]

    # Simulate conversation
    queries = [
        "Hi, I'm Alice and I prefer vegetarian food.",
        "What restaurants would you recommend?",
        "I also love Italian cuisine.",
        "Can you remember my food preferences?",
    ]

    for i, query in enumerate(queries, 1):
        print(f"\n--- Message {i} ---")
        print(f"User: {query}")
        response = await agent.run(query, thread=thread)
        print(f"Agent: {response.text}")

    # Show persistent storage
    if thread.message_store:
        messages = await thread.message_store.list_messages()  # type: ignore[union-attr]
        print(f"\nMessages stored for user {user_id}: {len(messages)}")

    # Cleanup
    if thread.message_store:
        await thread.message_store.clear()  # type: ignore[union-attr]
        await thread.message_store.aclose()  # type: ignore[union-attr]
    print("Cleaned up session data\n")


async def example_conversation_persistence() -> None:
    """Example of conversation persistence across application restarts."""
    print("=== Conversation Persistence Example ===")

    conversation_id = "persistent_chat_001"

    # Phase 1: Start conversation
    print("--- Phase 1: Starting conversation ---")
    store1 = RedisChatMessageStore(
        redis_url="redis://localhost:6379",
        thread_id=conversation_id,
    )

    thread1 = AgentThread(message_store=store1)
    agent = OpenAIChatClient().as_agent(
        name="PersistentBot",
        instructions="You are a helpful assistant. Remember our conversation history.",
    )

    # Start conversation
    query1 = "Hello! I'm working on a Python project about machine learning."
    print(f"User: {query1}")
    response1 = await agent.run(query1, thread=thread1)
    print(f"Agent: {response1.text}")

    query2 = "I'm specifically interested in neural networks."
    print(f"User: {query2}")
    response2 = await agent.run(query2, thread=thread1)
    print(f"Agent: {response2.text}")

    print(f"Stored {len(await store1.list_messages())} messages in Redis")
    await store1.aclose()

    # Phase 2: Resume conversation (simulating app restart)
    print("\n--- Phase 2: Resuming conversation (after 'restart') ---")
    store2 = RedisChatMessageStore(
        redis_url="redis://localhost:6379",
        thread_id=conversation_id,  # Same thread ID
    )

    thread2 = AgentThread(message_store=store2)

    # Continue conversation - agent should remember context
    query3 = "What was I working on before?"
    print(f"User: {query3}")
    response3 = await agent.run(query3, thread=thread2)
    print(f"Agent: {response3.text}")

    query4 = "Can you suggest some Python libraries for neural networks?"
    print(f"User: {query4}")
    response4 = await agent.run(query4, thread=thread2)
    print(f"Agent: {response4.text}")

    print(f"Total messages after resuming: {len(await store2.list_messages())}")

    # Cleanup
    await store2.clear()
    await store2.aclose()
    print("Cleaned up persistent data\n")


async def example_thread_serialization() -> None:
    """Example of thread state serialization and deserialization."""
    print("=== Thread Serialization Example ===")

    # Create initial thread with Redis store
    original_store = RedisChatMessageStore(
        redis_url="redis://localhost:6379",
        thread_id="serialization_test",
        max_messages=50,
    )

    original_thread = AgentThread(message_store=original_store)

    agent = OpenAIChatClient().as_agent(
        name="SerializationBot",
        instructions="You are a helpful assistant.",
    )

    # Have initial conversation
    print("--- Initial conversation ---")
    query1 = "Hello! I'm testing serialization."
    print(f"User: {query1}")
    response1 = await agent.run(query1, thread=original_thread)
    print(f"Agent: {response1.text}")

    # Serialize thread state
    serialized_thread = await original_thread.serialize()
    print(f"\nSerialized thread state: {serialized_thread}")

    # Close original connection
    await original_store.aclose()

    # Deserialize thread state (simulating loading from database/file)
    print("\n--- Deserializing thread state ---")

    # Create a new thread with the same Redis store type
    # This ensures the correct store type is used for deserialization
    restored_store = RedisChatMessageStore(redis_url="redis://localhost:6379")
    restored_thread = await AgentThread.deserialize(serialized_thread, message_store=restored_store)

    # Continue conversation with restored thread
    query2 = "Do you remember what I said about testing?"
    print(f"User: {query2}")
    response2 = await agent.run(query2, thread=restored_thread)
    print(f"Agent: {response2.text}")

    # Cleanup
    if restored_thread.message_store:
        await restored_thread.message_store.clear()  # type: ignore[union-attr]
        await restored_thread.message_store.aclose()  # type: ignore[union-attr]
    print("Cleaned up serialization test data\n")


async def example_message_limits() -> None:
    """Example of automatic message trimming with limits."""
    print("=== Message Limits Example ===")

    # Create store with small message limit
    store = RedisChatMessageStore(
        redis_url="redis://localhost:6379",
        thread_id="limits_test",
        max_messages=3,  # Keep only 3 most recent messages
    )

    thread = AgentThread(message_store=store)
    agent = OpenAIChatClient().as_agent(
        name="LimitBot",
        instructions="You are a helpful assistant with limited memory.",
    )

    # Send multiple messages to test trimming
    messages = [
        "Message 1: Hello!",
        "Message 2: How are you?",
        "Message 3: What's the weather?",
        "Message 4: Tell me a joke.",
        "Message 5: This should trigger trimming.",
    ]

    for i, query in enumerate(messages, 1):
        print(f"\n--- Sending message {i} ---")
        print(f"User: {query}")
        response = await agent.run(query, thread=thread)
        print(f"Agent: {response.text}")

        stored_messages = await store.list_messages()
        print(f"Messages in store: {len(stored_messages)}")
        if len(stored_messages) > 0:
            print(f"Oldest message: {stored_messages[0].text[:30]}...")

    # Final check
    final_messages = await store.list_messages()
    print(f"\nFinal message count: {len(final_messages)} (should be <= 6: 3 messages × 2 per exchange)")

    # Cleanup
    await store.clear()
    await store.aclose()
    print("Cleaned up limits test data\n")


async def main() -> None:
    """Run all Redis chat message store examples."""
    print("Redis Chat Message Store Examples")
    print("=" * 50)
    print("Prerequisites:")
    print("- Redis server running on localhost:6379")
    print("- OPENAI_API_KEY environment variable set")
    print("=" * 50)

    # Check prerequisites
    if not os.getenv("OPENAI_API_KEY"):
        print("ERROR: OPENAI_API_KEY environment variable not set")
        return

    try:
        # Test Redis connection
        test_store = RedisChatMessageStore(redis_url="redis://localhost:6379")
        connection_ok = await test_store.ping()
        await test_store.aclose()
        if not connection_ok:
            raise Exception("Redis ping failed")
        print("✓ Redis connection successful\n")
    except Exception as e:
        print(f"ERROR: Cannot connect to Redis: {e}")
        print("Please ensure Redis is running on localhost:6379")
        return

    try:
        # Run all examples
        await example_manual_memory_store()
        await example_user_session_management()
        await example_conversation_persistence()
        await example_thread_serialization()
        await example_message_limits()

        print("All examples completed successfully!")

    except Exception as e:
        print(f"Error running examples: {e}")
        raise


if __name__ == "__main__":
    asyncio.run(main())
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Suspend and Resume](./threads.md)
