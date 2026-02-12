---
title: Conversations & memory in Agent Framework
description: Learn about multi-turn conversations, chat history, persistent storage, and threads in Agent Framework.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 02/12/2026
ms.service: agent-framework
---

# Conversations & memory

Agents are most useful when they can remember what was said earlier in a conversation. Agent Framework provides `AgentThread` (Python) and `AgentSession` (.NET) to maintain conversation state across multiple exchanges, along with options for persistent storage and chat history management.

## How it works

When you create a thread and pass it to each `run` call, the agent automatically:

1. **Stores messages** — both user inputs and agent responses are tracked
2. **Provides context** — previous messages are sent to the model on each turn
3. **Maintains identity** — each thread has a unique ID for tracking

:::zone pivot="programming-language-csharp"

```csharp
AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a friendly assistant. Keep your answers brief.", name: "ConversationAgent");

// Create a session to maintain conversation history
AgentSession session = await agent.CreateSessionAsync();

// First turn
Console.WriteLine(await agent.RunAsync("My name is Alice and I love hiking.", session));

// Second turn — the agent remembers the user's name and hobby
Console.WriteLine(await agent.RunAsync("What do you remember about me?", session));
```

:::zone-end

:::zone pivot="programming-language-python"

```python
agent = client.as_agent(
    name="ConversationAgent",
    instructions="You are a friendly assistant. Keep your answers brief.",
)

# Create a thread to maintain conversation history
thread = agent.get_new_thread()

# First turn
result = await agent.run("My name is Alice and I love hiking.", thread=thread)
print(f"Agent: {result}\n")

# Second turn — the agent remembers the user's name and hobby
result = await agent.run("What do you remember about me?", thread=thread)
print(f"Agent: {result}")
```

:::zone-end

Without a thread, each call to `run` is independent — the agent has no memory of previous exchanges.

## Topics

| Topic | Description |
|-------|-------------|
| [Multi-Turn Conversations](multi-turn.md) | Maintain context across multiple exchanges |
| [Chat History](chat-history.md) | Access and manage the conversation history |
| [Persistent Storage](persistent-storage.md) | Store conversations across sessions |
| [Threads](threads.md) | Organize conversations into logical threads |

## Next steps

> [!div class="nextstepaction"]
> [Multi-Turn Conversations](multi-turn.md)
