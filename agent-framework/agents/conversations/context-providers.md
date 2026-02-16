---
title: Context Providers
description: Learn built-in and custom context provider patterns, including history provider guidance.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 02/13/2026
ms.service: agent-framework
---

# Context Providers

Context providers run around each invocation to add context before execution and process data after execution.

## Built-in pattern

:::zone pivot="programming-language-csharp"

Configure providers through constructor options when creating an agent. `ChatHistoryProvider` and `AIContextProvider` are the built-in extension points for chat history and memory/context enrichment respectively.

`InMemoryChatHistoryProvider` is the built-in provider used for local in-memory chat history.

```csharp
AIAgent agent = new OpenAIClient("<your_api_key>")
    .GetChatClient(modelName)
    .AsAIAgent(new ChatClientAgentOptions()
    {
        ChatOptions = new() { Instructions = "You are a helpful assistant." },
        ChatHistoryProvider = new InMemoryChatHistoryProvider(),
        AIContextProviders = [
            new MyCustomMemoryProvider()
        ],
    });

AgentSession session = await agent.CreateSessionAsync();
Console.WriteLine(await agent.RunAsync("Remember my name is Alice.", session));
```

:::zone-end

:::zone pivot="programming-language-python"

The regular pattern is to configure providers through `context_providers=[...]` when creating an agent.

`InMemoryHistoryProvider` is the built-in history provider used for local conversational memory.

```python
from agent_framework import InMemoryHistoryProvider
from agent_framework.openai import OpenAIChatClient

agent = OpenAIChatClient().as_agent(
    name="MemoryBot",
    instructions="You are a helpful assistant.",
    context_providers=[InMemoryHistoryProvider("memory", load_messages=True)],
)

session = agent.create_session()
await agent.run("Remember that I prefer vegetarian food.", session=session)
```

`RawAgent` may auto-add `InMemoryHistoryProvider("memory")` in specific cases, but add it explicitly when you want deterministic local memory behavior.

:::zone-end

## Custom context provider

Use custom context providers when you need to inject dynamic instructions/messages or extract state after runs.

:::zone pivot="programming-language-python"

```python
from typing import Any

from agent_framework import AgentSession, BaseContextProvider, SessionContext


class UserPreferenceProvider(BaseContextProvider):
    def __init__(self) -> None:
        super().__init__("user-preferences")

    async def before_run(
        self,
        *,
        agent: Any,
        session: AgentSession,
        context: SessionContext,
        state: dict[str, Any],
    ) -> None:
        if favorite := state.get("favorite_food"):
            context.extend_instructions(self.source_id, f"User's favorite food is {favorite}.")

    async def after_run(
        self,
        *,
        agent: Any,
        session: AgentSession,
        context: SessionContext,
        state: dict[str, Any],
    ) -> None:
        for message in context.input_messages:
            text = (message.text or "") if hasattr(message, "text") else ""
            if isinstance(text, str) and "favorite food is" in text.lower():
                state["favorite_food"] = text.split("favorite food is", 1)[1].strip().rstrip(".")
```

:::zone-end

## Custom history provider

History providers are context providers specialized for loading/storing messages.

:::zone pivot="programming-language-python"

```python
from collections.abc import Sequence
from typing import Any

from agent_framework import BaseHistoryProvider, Message


class DatabaseHistoryProvider(BaseHistoryProvider):
    def __init__(self, db: Any) -> None:
        super().__init__("db-history", load_messages=True)
        self._db = db

    async def get_messages(
        self,
        session_id: str | None,
        *,
        state: dict[str, Any] | None = None,
        **kwargs: Any,
    ) -> list[Message]:
        key = (state or {}).get(self.source_id, {}).get("history_key", session_id or "default")
        rows = await self._db.load_messages(key)
        return [Message.from_dict(row) for row in rows]

    async def save_messages(
        self,
        session_id: str | None,
        messages: Sequence[Message],
        *,
        state: dict[str, Any] | None = None,
        **kwargs: Any,
    ) -> None:
        if not messages:
            return
        if state is not None:
            key = state.setdefault(self.source_id, {}).setdefault("history_key", session_id or "default")
        else:
            key = session_id or "default"
        await self._db.save_messages(key, [m.to_dict() for m in messages])
```

> [!IMPORTANT]
> In Python, you can configure multiple history providers, but **only one** should use `load_messages=True`.
> Use additional providers for diagnostics/evals with `load_messages=False` and `store_context_messages=True` so they capture context from other providers alongside input/output.
>
> Example pattern:
>
> ```python
> primary = DatabaseHistoryProvider(db)
> audit = InMemoryHistoryProvider("audit", load_messages=False, store_context_messages=True)
> agent = OpenAIChatClient().as_agent(context_providers=[primary, audit])
> ```

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Storage](./storage.md)
