---
title: "Runtime Context"
description: "Learn how to use runtime context in middleware."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: reference
ms.author: edvan
ms.date: 03/16/2026
ms.service: agent-framework
---

# Runtime Context

Runtime context provides middleware with access to information about the current execution environment and request. This enables patterns such as per-session configuration, user-specific behavior, and dynamic middleware behavior based on runtime conditions.

:::zone pivot="programming-language-csharp"

In C#, runtime context is typically passed through `AgentRunOptions` or custom session state. Middleware can access session properties and run options to make runtime decisions.

> [!TIP]
> See the [Agent vs Run Scope](./agent-vs-run-scope.md) page for information on how middleware scope affects access to runtime context.

:::zone-end

:::zone pivot="programming-language-python"

Python runtime context is split across three public surfaces:

- `session=` for conversation state and history.
- `function_invocation_kwargs=` for values that only tools or function middleware should see.
- `client_kwargs=` for chat-client-specific data or client middleware configuration.

Use the smallest surface that fits the data. This keeps tool inputs explicit and avoids leaking client-only metadata into tool execution.

> [!TIP]
> Treat `function_invocation_kwargs` as the replacement for the old pattern of passing arbitrary public `**kwargs` to `agent.run()` or `get_response()`.

### Choose the right runtime bucket

| Use case | API surface | Accessed from |
|---|---|---|
| Share conversation state, service session IDs, or history | `session=` | `ctx.session`, `AgentContext.session` |
| Pass runtime values only tools or function middleware need | `function_invocation_kwargs=` | `FunctionInvocationContext.kwargs` |
| Pass client-specific runtime values or client middleware configuration | `client_kwargs=` | custom `get_response(..., client_kwargs=...)` implementations |

### Pass tool-only runtime values

```python
from typing import Annotated

from agent_framework import FunctionInvocationContext, tool
from agent_framework.openai import OpenAIResponsesClient


@tool(approval_mode="never_require")
def send_email(
    address: Annotated[str, "Recipient email address."],
    ctx: FunctionInvocationContext,
) -> str:
    user_id = ctx.kwargs["user_id"]
    tenant = ctx.kwargs.get("tenant", "default")
    return f"Queued email for {address} from {user_id} ({tenant})"


agent = OpenAIResponsesClient().as_agent(
    name="Notifier",
    instructions="Send email updates.",
    tools=[send_email],
)

response = await agent.run(
    "Email the launch update to finance@example.com",
    function_invocation_kwargs={
        "user_id": "user-123",
        "tenant": "contoso",
    },
)

print(response.text)
```

Use `ctx.kwargs` inside the tool instead of declaring blanket `**kwargs` on the tool callable. Legacy `**kwargs` tools still work for compatibility, but will be removed before GA.

Any parameter annotated as `FunctionInvocationContext` is treated as the injected runtime context parameter, regardless of its name, and it is not exposed in the JSON schema shown to the model. If you provide an explicit schema/input model, a plain unannotated parameter named `ctx` is also recognized as the injected context parameter.

If the value is long-lived tool state or a dependency rather than per-invocation data, keep it on a tool class instance instead of passing it through `function_invocation_kwargs`. For that pattern, see [Create a class with multiple function tools](../tools/function-tools.md#create-a-class-with-multiple-function-tools).

### Function middleware receives the same context

Function middleware uses the same `FunctionInvocationContext` object that tools receive. That means middleware can inspect `context.arguments`, `context.kwargs`, `context.session`, and `context.result`.

```python
from collections.abc import Awaitable, Callable

from agent_framework import FunctionInvocationContext
from agent_framework.openai import OpenAIResponsesClient


async def enrich_tool_runtime_context(
    context: FunctionInvocationContext,
    call_next: Callable[[], Awaitable[None]],
) -> None:
    context.kwargs.setdefault("tenant", "contoso")
    context.kwargs.setdefault("request_source", "middleware")
    await call_next()


agent = OpenAIResponsesClient().as_agent(
    name="Notifier",
    instructions="Send email updates.",
    tools=[send_email],
    middleware=[enrich_tool_runtime_context],
)
```

The middleware contract uses `call_next()` with no arguments. Mutate `context.kwargs` before calling it, and the selected tool sees those values through its injected `FunctionInvocationContext`.

### Use `session=` for shared runtime state

```python
from typing import Annotated

from agent_framework import FunctionInvocationContext, tool
from agent_framework.openai import OpenAIResponsesClient


@tool(approval_mode="never_require")
def remember_topic(
    topic: Annotated[str, "Topic to remember."],
    ctx: FunctionInvocationContext,
) -> str:
    if ctx.session is None:
        return "No session available."

    ctx.session.state["topic"] = topic
    return f"Stored {topic!r} in session state."


agent = OpenAIResponsesClient().as_agent(
    name="MemoryAgent",
    instructions="Remember important topics.",
    tools=[remember_topic],
)

session = agent.create_session()
await agent.run("Remember that the budget review is on Friday.", session=session)
print(session.state["topic"])
```

Pass the session explicitly with `session=` and read it from `ctx.session`. Session access no longer needs to travel through runtime kwargs.

### Share session state with delegated agents

When an agent is exposed as a tool via `as_tool()`, runtime function kwargs already flow through `ctx.kwargs`. Add `propagate_session=True` only when the sub-agent should share the caller's `AgentSession`.

```python
from agent_framework import FunctionInvocationContext, tool
from agent_framework.openai import OpenAIResponsesClient


@tool(description="Store findings for later steps.")
def store_findings(findings: str, ctx: FunctionInvocationContext) -> None:
    if ctx.session is not None:
        ctx.session.state["findings"] = findings


client = OpenAIResponsesClient()

research_agent = client.as_agent(
    name="ResearchAgent",
    instructions="Research the topic and store findings.",
    tools=[store_findings],
)

research_tool = research_agent.as_tool(
    name="research",
    description="Research a topic and store findings.",
    arg_name="query",
    propagate_session=True,
)
```

With `propagate_session=True`, the delegated agent sees the same `ctx.session` state as the caller. Leave it `False` to isolate the child agent in its own session.

### Custom chat clients and agents

If you implement custom public `run()` or `get_response()` methods, add the explicit runtime buckets to the signature.

```python
from collections.abc import Mapping, Sequence
from typing import Any

from agent_framework import ChatOptions, Message


async def get_response(
    self,
    messages: Sequence[Message],
    *,
    options: ChatOptions[Any] | None = None,
    function_invocation_kwargs: Mapping[str, Any] | None = None,
    client_kwargs: Mapping[str, Any] | None = None,
    **kwargs: Any,
):
    ...
```

Use `function_invocation_kwargs` for tool-invocation flows and `client_kwargs` for client-specific behavior. Passing client-specific values directly through public `**kwargs` is only a compatibility path and should be treated as deprecated. Likewise, defining new tools with `**kwargs` is migration-only compatibility — consume runtime data through the injected context object instead.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Middleware Overview](./index.md)
