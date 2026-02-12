---
title: "Step 4: Memory & Persistence"
description: "Add context providers and persistent memory to your agent."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: tutorial
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Step 4: Memory & Persistence

Add context to your agent so it can remember user preferences, past interactions, or external knowledge.

:::zone pivot="programming-language-csharp"

Set up memory with a custom `ChatHistoryProvider`:

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("Set AZURE_OPENAI_ENDPOINT");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a friendly assistant. Keep your answers brief.", name: "MemoryAgent");
```

Use a session to persist context across runs:

```csharp
AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine(await agent.RunAsync("Hello! What's the square root of 9?", session));
Console.WriteLine(await agent.RunAsync("My name is Alice", session));
Console.WriteLine(await agent.RunAsync("What is my name?", session));
```

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/01-get-started/04_Memory.cs) for the complete runnable file.

:::zone-end

:::zone pivot="programming-language-python"

Define a context provider that injects additional context into every agent call:

```python
class UserNameProvider(ContextProvider):
    """A simple context provider that remembers the user's name."""

    def __init__(self) -> None:
        self.user_name: str | None = None

    async def invoking(self, messages: Message | MutableSequence[Message], **kwargs: Any) -> Context:
        """Called before each agent invocation — add extra instructions."""
        if self.user_name:
            return Context(instructions=f"The user's name is {self.user_name}. Always address them by name.")
        return Context(instructions="You don't know the user's name yet. Ask for it politely.")

    async def invoked(
        self,
        request_messages: Message | "list[Message] | Message | None" = None,
        response_messages: "Message | list[Message] | None" = None,
        invoke_exception: Exception | None = None,
        **kwargs: Any,
    ) -> None:
        """Called after each agent invocation — extract information."""
        msgs = [request_messages] if isinstance(request_messages, Message) else list(request_messages or [])
        for msg in msgs:
            text = msg.text if hasattr(msg, "text") else ""
            if isinstance(text, str) and "my name is" in text.lower():
                self.user_name = text.lower().split("my name is")[-1].strip().split()[0].capitalize()
```

Create an agent with the context provider:

```python
credential = AzureCliCredential()
client = AzureOpenAIResponsesClient(
    project_endpoint=os.environ["AZURE_AI_PROJECT_ENDPOINT"],
    deployment_name=os.environ["AZURE_OPENAI_RESPONSES_DEPLOYMENT_NAME"],
    credential=credential,
)

memory = UserNameProvider()

agent = client.as_agent(
    name="MemoryAgent",
    instructions="You are a friendly assistant.",
    context_provider=memory,
)
```

Run it — the agent now has access to the context:

```python
thread = agent.get_new_thread()

result = await agent.run("Hello! What's the square root of 9?", thread=thread)
print(f"Agent: {result}\n")

# Now provide the name — the provider extracts and stores it
result = await agent.run("My name is Alice", thread=thread)
print(f"Agent: {result}\n")

# Subsequent calls are personalized
result = await agent.run("What is 2 + 2?", thread=thread)
print(f"Agent: {result}\n")

print(f"[Memory] Stored user name: {memory.user_name}")
```

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/01-get-started/04_memory.py) for the complete runnable file.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Step 5: Workflows](./workflows.md)

**Go deeper:**

- [Persistent storage](../agents/conversations/persistent-storage.md) — store conversations in databases
- [Chat history](../agents/conversations/chat-history.md) — manage chat history and memory
