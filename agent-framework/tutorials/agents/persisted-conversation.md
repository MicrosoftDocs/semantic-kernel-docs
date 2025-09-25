---
title: Persisting and Resuming Agent Conversations
description: How to persist an agent thread to storage and reload it later
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/25/2025
ms.service: semantic-kernel
---

# Persisting and Resuming Agent Conversations

::: zone pivot="programming-language-csharp"

This tutorial shows how to persist an agent conversation (AgentThread) to storage and reload it later.

When hosting an agent in a service or even in a client application, you often want to maintain conversation state across multiple requests or sessions. By persisting the `AgentThread`, you can save the conversation context and reload it later.

## Prerequisites

For prerequisites and installing nuget packages, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Persisting and resuming the conversation

Create an agent and obtain a new thread that will hold the conversation state.

```csharp
AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
     .GetChatClient("gpt-4o-mini")
     .CreateAIAgent(instructions: "You are a helpful assistant.", name: "Assistant");

AgentThread thread = agent.GetNewThread();
```

Run the agent, passing in the thread, so that the `AgentThread` includes this exchange.

```csharp
// Run the agent and append the exchange to the thread
Console.WriteLine(await agent.RunAsync("Tell me a short pirate joke.", thread));
```

Call the SerializeAsync method on the thread to serialize it to a JsonElement.
It can then be converted to a string for storage and saved to a database, blob storage, or file.

```csharp
// Serialize the thread state
JsonElement serializedThread = await thread.SerializeAsync();
string serializedJson = JsonSerializer.Serialize(serializedThread, JsonSerializerOptions.Web);

// Example: save to a local file (replace with DB or blob storage in production)
string filePath = Path.Combine(Path.GetTempPath(), "agent_thread.json");
await File.WriteAllTextAsync(filePath, serializedJson);
```

Load the persisted JSON from storage and recreate the AgentThread instance from it.
Note that the thread must be deserialized using an agent instance. This should be the
same agent type that was used to create the original thread.
This is because agents may have their own thread types and may construct threads with
additional functionality that is specific to that agent type.

```csharp
// Read persisted JSON
string loadedJson = await File.ReadAllTextAsync(filePath);
JsonElement reloaded = JsonSerializer.Deserialize<JsonElement>(loadedJson);

// Deserialize the thread into an AgentThread tied to the same agent type
AgentThread resumedThread = agent.DeserializeThread(reloaded);
```

Use the resumed thread to continue the conversation.

```csharp
// Continue the conversation with resumed thread
Console.WriteLine(await agent.RunAsync("Now tell that joke in the voice of a pirate.", resumedThread));
```

::: zone-end
::: zone pivot="programming-language-python"

Tutorial coming soon.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Third Party chat history storage](./third-party-chat-history-storage.md)
