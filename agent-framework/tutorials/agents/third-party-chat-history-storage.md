---
title: Storing Chat History in 3rd Party Storage
description: How to store agent chat history in external storage using a custom ChatMessageStore.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/25/2025
ms.service: semantic-kernel
---

# Storing Chat History in 3rd Party Storage

This tutorial shows how to store agent chat history in external storage by implementing a custom `ChatMessageStore` and using it with a `ChatClientAgent`.

::: zone pivot="programming-language-csharp"

By default, when using `ChatClientAgent`, chat history is stored either in memory in the `AgentThread` object or the underlying inference service, if the service supports it.

Where services do not require chat history to be stored in the service, it is possible to provide a custom store for persisting chat history instead of relying on the default in-memory behavior.

## Prerequisites

For prerequisites, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Installing Nuget packages

To use the Microsoft Agent Framework with Azure OpenAI, you need to install the following NuGet packages:

```powershell
dotnet add package Azure.Identity
dotnet add package Azure.AI.OpenAI
dotnet add package Microsoft.Extensions.AI.OpenAI
dotnet add package Microsoft.Agents.OpenAI
```

In addition to this, we will use the in-memory vector store to store chat messages and a utility package for async LINQ operations.

```powershell
dotnet add package Microsoft.SemanticKernel.Connectors.InMemory --prerelease
dotnet add package System.Linq.Async
```

## Creating a custom ChatMessage Store

To create a custom `ChatMessageStore`, you need to implement the abstract `ChatMessageStore` class and provide implementations for the required methods.

### Message storage and retrieval methods

The most important methods to implement are:

- `AddMessagesAsync` - called to add new messages to the store.
- `GetMessagesAsync` - called to retrieve the messages from the store.

`GetMessagesAsync` should return the messages in ascending chronological order. All messages returned by it will be used by the `ChatClientAgent` when making calls to the underlying `IChatClient`.  It's therefore important that this method considers the limits of the underlying model, and only returns as many messages as can be handled by the model.

Any chat history reduction logic, such as summarization or trimming, should be done before returning messages from `GetMessagesAsync`.

### Serialization

`ChatMessageStore` instances are created and attached to an `AgentThread` when the thread is created, and when a thread is resumed from a serialized state.

While the actual messages making up the chat history are stored externally, the `ChatMessageStore` instance may need to store keys or other state to identify the chat history in the external store.

To allow persisting threads, you need to implement the `SerializeStateAsync` method of the `ChatMessageStore` class. You also need to provide a constructor that takes a `JsonElement` parameter, which can be used to deserialize the state when resuming a thread.

### Sample ChatMessageStore implementation

Let's look at a sample implementation that stores chat messages in a vector store.

In `AddMessagesAsync` it upserts messages into the vector store, using a unique key for each message.

`GetMessagesAsync` retrieves the messages for the current thread from the vector store, orders them by timestamp, and returns them in ascending order.

When the first message is received, the store generates a unique key for the thread, which is then used to identify the chat history in the vector store for subsequent calls.

The unique key is stored in the `ThreadDbKey` property, which is serialized and deserialized using the `SerializeStateAsync` method and the constructor that takes a `JsonElement`.
This key will therefore be persisted as part of the `AgentThread` state, allowing the thread to be resumed later and continue using the same chat history.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;

internal sealed class VectorChatMessageStore : ChatMessageStore
{
    private readonly VectorStore _vectorStore;

    public VectorChatMessageStore(
        VectorStore vectorStore,
        JsonElement serializedStoreState,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        this._vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        if (serializedStoreState.ValueKind is JsonValueKind.String)
        {
            this.ThreadDbKey = serializedStoreState.Deserialize<string>();
        }
    }

    public string? ThreadDbKey { get; private set; }

    public override async Task AddMessagesAsync(
        IEnumerable<ChatMessage> messages,
        CancellationToken cancellationToken)
    {
        this.ThreadDbKey ??= Guid.NewGuid().ToString("N");
        var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        await collection.UpsertAsync(messages.Select(x => new ChatHistoryItem()
        {
            Key = this.ThreadDbKey + x.MessageId,
            Timestamp = DateTimeOffset.UtcNow,
            ThreadId = this.ThreadDbKey,
            SerializedMessage = JsonSerializer.Serialize(x),
            MessageText = x.Text
        }), cancellationToken);
    }

    public override async Task<IEnumerable<ChatMessage>> GetMessagesAsync(
        CancellationToken cancellationToken)
    {
        var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        var records = await collection
            .GetAsync(
                x => x.ThreadId == this.ThreadDbKey, 10,
                new() { OrderBy = x => x.Descending(y => y.Timestamp) },
                cancellationToken)
            .ToListAsync(cancellationToken);
        var messages = records.ConvertAll(x => JsonSerializer.Deserialize<ChatMessage>(x.SerializedMessage!)!);
        messages.Reverse();
        return messages;
    }

    public override ValueTask<JsonElement?> SerializeStateAsync(
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken cancellationToken = default) =>
            new(JsonSerializer.SerializeToElement(this.ThreadDbKey));

    private sealed class ChatHistoryItem
    {
        [VectorStoreKey]
        public string? Key { get; set; }
        [VectorStoreData]
        public string? ThreadId { get; set; }
        [VectorStoreData]
        public DateTimeOffset? Timestamp { get; set; }
        [VectorStoreData]
        public string? SerializedMessage { get; set; }
        [VectorStoreData]
        public string? MessageText { get; set; }
    }
}
```

## Using the custom ChatMessageStore with a ChatClientAgent

To use the custom `ChatMessageStore`, you need to provide a `ChatMessageStoreFactory` when creating the agent. This factory allows the agent to create a new instance of the desired `ChatMessageStore` for each thread.

When creating a `ChatClientAgent` it is possible to provide a `ChatClientAgentOptions` object that allows providing the `ChatMessageStoreFactory` in addition to all other agent options.

```csharp
AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
     .GetChatClient("gpt-4o-mini")
     .CreateAIAgent(new ChatClientAgentOptions
     {
         Name = "Joker",
         Instructions = "You are good at telling jokes.",
         ChatMessageStoreFactory = ctx =>
         {
             // Create a new chat message store for this agent that stores the messages in a vector store.
             return new VectorChatMessageStore(
                new InMemoryVectorStore(),
                ctx.SerializedState,
                ctx.JsonSerializerOptions);
         }
     });
```

::: zone-end
::: zone pivot="programming-language-python"

By default, when using `ChatAgent`, chat history is stored either in memory in the `AgentThread` object or in the underlying inference service, if the service supports it.

Where services do not require chat history to be stored in the service, it is possible to provide a custom store for persisting chat history instead of relying on the default in-memory behavior.

## Prerequisites

For prerequisites, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Installing Python packages

To use the Agent Framework with OpenAI, you need to install the following packages:

```bash
pip install agent-framework
```

In addition to this, we will use Redis to store chat messages:

```bash
pip install agent-framework[redis]
```

## Creating a custom ChatMessage Store

To create a custom chat message store, you need to implement the `ChatMessageStoreProtocol` protocol and provide implementations for the required methods.

### Message storage and retrieval methods

The most important methods to implement are:

- `add_messages` - called to add new messages to the store.
- `list_messages` - called to retrieve the messages from the store.

`list_messages` should return the messages in ascending chronological order. All messages returned by it will be used by the `ChatAgent` when making calls to the underlying chat client. It's therefore important that this method considers the limits of the underlying model, and only returns as many messages as can be handled by the model.

Any chat history reduction logic, such as summarization or trimming, should be done before returning messages from `list_messages`.

### Serialization

`ChatMessageStore` instances are created and attached to an `AgentThread` when the thread is created, and when a thread is resumed from a serialized state.

While the actual messages making up the chat history are stored externally, the `ChatMessageStore` instance may need to store keys or other state to identify the chat history in the external store.

To allow persisting threads, you need to implement the `serialize` method and optionally the `update_from_state` method of the `ChatMessageStoreProtocol`. You can also provide a class method `deserialize` which can be used to create a new instance from serialized state when resuming a thread.

### Sample ChatMessageStore implementation

Let's look at a sample implementation that stores chat messages in Redis.

The framework includes a production-ready Redis implementation that we can use directly. When the first message is received, the store generates a unique key for the thread, which is then used to identify the chat history in Redis for subsequent calls.

The unique key is stored in the `thread_id` property, which is serialized and deserialized automatically. This key will therefore be persisted as part of the `AgentThread` state, allowing the thread to be resumed later and continue using the same chat history.

```python
from agent_framework.redis import RedisChatMessageStore

store = RedisChatMessageStore(
    redis_url="redis://localhost:6379",
    thread_id="my_conversation",  # Optional, auto-generated if not provided
    max_messages=10,
    key_prefix="chat_messages"
)
```

## Using the custom ChatMessageStore with a ChatAgent

To use the custom `ChatMessageStore`, you need to provide a `chat_message_store_factory` when creating the agent.

### Basic Usage

```python
from agent_framework import ChatAgent
from agent_framework.openai import OpenAIChatClient
from agent_framework.redis import RedisChatMessageStore

# Create agent with Redis store
def create_redis_store():
    return RedisChatMessageStore(
        redis_url="redis://localhost:6379",
        max_messages=10
    )

agent = ChatAgent(
    chat_client=OpenAIChatClient(),
    instructions="You are a helpful assistant with persistent memory.",
    chat_message_store_factory=create_redis_store
)

# Use the agent
thread = agent.get_new_thread()
response = await agent.run("Hello, remember that I like Python!", thread=thread)
```

### Thread Persistence

When using custom stores, thread serialization works seamlessly:

```python
import json

# Have conversation
thread = agent.get_new_thread()
await agent.run("Hello, I'm working on a Python project.", thread=thread)

# Serialize thread (includes store state)
serialized_thread = await thread.serialize()

# Save to file
with open("thread_state.json", "w") as f:
    json.dump(serialized_thread, f)

# Later, restore the thread
with open("thread_state.json", "r") as f:
    thread_data = json.load(f)

resumed_thread = await agent.deserialize_thread(thread_data)
await agent.run("What was I working on?", thread=resumed_thread)
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Adding Memory to an Agent](memory.md)
