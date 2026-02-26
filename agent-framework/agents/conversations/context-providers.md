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

Configure providers through constructor options when creating an agent. `AIContextProvider` is the built-in extension point for memory/context enrichment.

```csharp
AIAgent agent = new OpenAIClient("<your_api_key>")
    .GetChatClient(modelName)
    .AsAIAgent(new ChatClientAgentOptions()
    {
        ChatOptions = new() { Instructions = "You are a helpful assistant." },
        AIContextProviders = [
            new MyCustomMemoryProvider()
        ],
    });

AgentSession session = await agent.CreateSessionAsync();
Console.WriteLine(await agent.RunAsync("Remember my name is Alice.", session));
```

> [!TIP]
> For a list of pre-built `AIContextProvider` implementations see [Integrations](../../integrations/index.md)

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

Use custom context providers when you need to inject dynamic instructions/messages/tools or extract state after runs.

:::zone pivot="programming-language-csharp"

The base class for context providers is `Microsoft.Agents.AI.AIContextProvider`.
Context providers participate in the agent pipeline, have the ability to contribute to or override agent input messages
and can extract information from new messages.
`AIContextProvider` has various virtual methods that can be overridden to implement your own custom context provider.
See the different implementation options below for more information on what to override.

### `AIContextProvider` state

An `AIContextProvider` instance is attached to an agent and the same instance would be used for all sessions.
This means that the `AIContextProvider` should not store any session specific state in the provider instance.
The `AIContextProvider` may have a reference to a memory service client in a field, but shouldn't have an id for
the specific set of memories in a field.

Instead, the `AIContextProvider` can store any session specific values, like memory ids, messages, or anything else that is relevant
in the `AgentSession` itself.  The virtual methods on `AIContextProvider` are all passed a reference to the current `AIAgent` and `AgentSession`.

To enable easily storing typed state in the `AgentSession`, a utility class is provided:

```csharp
// First define a type containing the properties to store in state
internal class MyCustomState
{
    public string? MemoryId { get; set; }
}

// Create the helper
var sessionStateHelper = new ProviderSessionState<MyCustomState>(
    // stateInitializer is called when there is no state in the session for this AIContextProvider yet
    stateInitializer: currentSession => new MyCustomState() { MemoryId = Guid.NewGuid().ToString() },
    // The key under which to store state in the session for this provider. Make sure it does not clash with the keys of other providers.
    stateKey: this.GetType().Name,
    // An optional jsonSerializerOptions to control the serialization/deserialization of the custom state object
    jsonSerializerOptions: myJsonSerializerOptions);

// Using the helper you can read state:
MyCustomState state = sessionStateHelper.GetOrInitializeState(session);
Console.WriteLine(state.MemoryId);

// And write state:
sessionStateHelper.SaveState(session, state);
```

### Simple `AIContextProvider` implementation

The simplest `AIContextProvider` implementation would typically override two methods:

- **AIContextProvider.ProvideAIContextAsync** - Load relevant data and return additional instructions, messages or tools.
- **AIContextProvider.StoreAIContextAsync** - Extract any relevant data from new messages and store.

Here is an example of a simple `AIContextProvider` that integrates with a memory service.

```csharp
internal sealed class SimpleServiceMemoryProvider : AIContextProvider
{
    private readonly ProviderSessionState<State> _sessionState;
    private readonly ServiceClient _client;

    public SimpleServiceMemoryProvider(ServiceClient client, Func<AgentSession?, State>? stateInitializer = null)
        : base(null, null)
    {
        this._sessionState = new ProviderSessionState<State>(
            stateInitializer ?? (_ => new State()),
            this.GetType().Name);
        this._client = client;
    }

    public override string StateKey => this._sessionState.StateKey;

    protected override ValueTask<AIContext> ProvideAIContextAsync(InvokingContext context, CancellationToken cancellationToken = default)
    {
        var state = this._sessionState.GetOrInitializeState(context.Session);

        if (state.MemoriesId == null)
        {
            // No stored memories yet.
            return new ValueTask<AIContext>(new AIContext());
        }

        // Find memories that match the current user input.
        var memories = this._client.LoadMemories(state.MemoriesId, string.Join("\n", context.AIContext.Messages?.Select(x => x.Text) ?? []));

        // Return a new message that contains the text from any memories that were found.
        return new ValueTask<AIContext>(new AIContext
        {
            Messages = [new ChatMessage(ChatRole.User, "Here are some memories to help answer the user question: " + string.Join("\n", memories.Select(x => x.Text)))]
        });
    }

    protected override async ValueTask StoreAIContextAsync(InvokedContext context, CancellationToken cancellationToken = default)
    {
        var state = this._sessionState.GetOrInitializeState(context.Session);
        // Create a memory container in the service for this session
        // and save the returned id in the session.
        state.MemoriesId ??= this._client.CreateMemoryContainer();
        this._sessionState.SaveState(context.Session, state);

        // Use the service to extract memories from the user input and agent response.
        await this._client.StoreMemoriesAsync(state.MemoriesId, context.RequestMessages.Concat(context.ResponseMessages ?? []), cancellationToken);
    }

    public class State
    {
        public string? MemoriesId { get; set; }
    }
}
```

### Advanced `AIContextProvider` implementation

A more advanced implementation could choose to override the following methods:

- **AIContextProvider.InvokingCoreAsync** - Called before the agent invokes the LLM and allows the request message list, tools and instructions to be modified.
- **AIContextProvider.InvokedCoreAsync** - Called after the agent had invoked the LLM and allows access to all request and response messages.

`AIContextProvider` provides base implementations of `InvokingCoreAsync` and `InvokedCoreAsync`.

The `InvokingCoreAsync` base implementation does the following:

- filters the input message list to only messages passed into the agent by the caller. Note that this filter can be overridden via the `provideInputMessageFilter` parameter on the `AIContextProvider` constructor.
- calls `ProvideAIContextAsync` with the filtered request messages, existing tools and instructions.
- stamps all messages returned by `ProvideAIContextAsync` with source information, indicating that these messages are coming from this context provider.
- merges the messages, tools and instructions returned by `ProvideAIContextAsync` with the existing ones, to produce the input that will be used by the agent. Messages, tools and instructions are appended to existing ones.

The `InvokedCoreAsync` base does the following:

- checks if the run failed and if so, returns without doing any further processing.
- filters the input message list to only messages passed into the agent by the caller. Note that this filter can be overridden via the `storeInputMessageFilter` parameter on the `AIContextProvider` constructor.
- passes the filtered request messages and all response messages to `StoreAIContextAsync` for storage.

It's possible to override these methods to implement an `AIContextProvider`, however this requires the implementer to implement the base functionality themself as appropriate.
Here is an example of such an implementation.

```csharp
internal sealed class AdvancedServiceMemoryProvider : AIContextProvider
{
    private readonly ProviderSessionState<State> _sessionState;
    private readonly ServiceClient _client;

    public AdvancedServiceMemoryProvider(ServiceClient client, Func<AgentSession?, State>? stateInitializer = null)
        : base(null, null)
    {
        this._sessionState = new ProviderSessionState<State>(
            stateInitializer ?? (_ => new State()),
            this.GetType().Name);
        this._client = client;
    }

    public override string StateKey => this._sessionState.StateKey;

    protected override async ValueTask<AIContext> InvokingCoreAsync(InvokingContext context, CancellationToken cancellationToken = default)
    {
        var state = this._sessionState.GetOrInitializeState(context.Session);

        if (state.MemoriesId == null)
        {
            // No stored memories yet.
            return new AIContext();
        }

        // We only want to search for memories based on user input, and exclude chat history or other AI context provider messages.
        var filteredInputMessages = context.AIContext.Messages?.Where(m => m.GetAgentRequestMessageSourceType() == AgentRequestMessageSourceType.External);

        // Find memories that match the current user input.
        var memories = this._client.LoadMemories(state.MemoriesId, string.Join("\n", filteredInputMessages?.Select(x => x.Text) ?? []));

        // Create a message for the memories, and stamp it to indicate where it came from.
        var memoryMessages =
            [new ChatMessage(ChatRole.User, "Here are some memories to help answer the user question: " + string.Join("\n", memories.Select(x => x.Text)))]
            .Select(m => m.WithAgentRequestMessageSource(AgentRequestMessageSourceType.AIContextProvider, this.GetType().FullName!));

        // Return a new merged AIContext.
        return new AIContext
        {
            Instructions = context.AIContext.Instructions,
            Messages = context.AIContext.Messages.Concat(memoryMessages),
            Tools = context.AIContext.Tools
        };
    }

    protected override async ValueTask InvokedCoreAsync(InvokedContext context, CancellationToken cancellationToken = default)
    {
        if (context.InvokeException is not null)
        {
            return;
        }

        var state = this._sessionState.GetOrInitializeState(context.Session);
        // Create a memory container in the service for this session
        // and save the returned id in the session.
        state.MemoriesId ??= this._client.CreateMemoryContainer();
        this._sessionState.SaveState(context.Session, state);

        // We only want to store memories based on user input and agent output, and exclude messages from chat history or other AI context providers to avoid feedback loops.
        var filteredRequestMessages = context.RequestMessages.Where(m => m.GetAgentRequestMessageSourceType() == AgentRequestMessageSourceType.External);

        // Use the service to extract memories from the user input and agent response.
        await this._client.StoreMemoriesAsync(state.MemoriesId, filteredRequestMessages.Concat(context.ResponseMessages ?? []), cancellationToken);
    }

    public class State
    {
        public string? MemoriesId { get; set; }
    }
}
```

:::zone-end

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

:::zone pivot="programming-language-python"

## Custom history provider

History providers are context providers specialized for loading/storing messages.

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
