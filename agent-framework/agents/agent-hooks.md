---
title: Agent hooks
description: Add fail-closed governance and runtime controls to agents with the Agent Hooks interception contract.
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: article
ms.author: evmattso
ms.date: 08/07/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                              | C# | Python | Go | Notes                     |
  |--------------------------------------|:--:|:------:|:--:|---------------------------|
  | Availability guidance                | ✅ |   ✅   | ✅ |                           |
  | Feature implementation               | ❌ |   ✅   | ❌ | Python-only               |
  | When to use Agent Hooks              | ❌ |   ✅   | ❌ | Python-specific            |
  | Core enforcement guarantees          | ❌ |   ✅   | ❌ | Python-specific            |
  | Installation and quickstart          | ❌ |   ✅   | ❌ | Python-specific            |
  | Interception points and verdicts     | ❌ |   ✅   | ❌ | Python-specific            |
  | Tool approval interaction            | ❌ |   ✅   | ❌ | Python-specific            |
  | Streaming and persistence            | ❌ |   ✅   | ❌ | Python-specific            |
  | Sessions, records, and configuration | ❌ |   ✅   | ❌ | Python-specific            |
  | Composition and limitations          | ❌ |   ✅   | ❌ | Python-specific            |
-->

# Agent hooks

Agent Hooks is a first-class Agent Framework capability for applying governance and runtime controls at well-defined points in an agent's execution. It implements the framework-neutral [AGENT-HOOKS-0.1 contract](https://github.com/responsibleai/agent-hooks/blob/main/spec/AGENT-HOOKS-0.1.md), so policy engines, approval gateways, budget guards, content filters, and egress controls can target one common control surface.

> [!IMPORTANT]
> Agent Hooks is a control plane, not a telemetry plane. Every interceptor returns a verdict. In `enforce` mode, the framework acts on that verdict; in `evaluate_only` mode, it records the verdict without changing execution. Use [observability](./observability.md) for passive tracing, metrics, and logs.

::: zone pivot="programming-language-csharp"

Agent Hooks isn't yet available for .NET. Use [agent middleware](../concepts/agents/middleware/index.md), [tool approval](./tools/tool-approval.md), and [agent safety](../concepts/agents/safety.md) to add runtime controls to .NET agents.

::: zone-end

::: zone pivot="programming-language-python"

Agent Hooks is experimental in Python. The factory emits an `ExperimentalWarning` when first used, and its API can change before general availability.

## When to use Agent Hooks

Use Agent Hooks when independently developed controls need one shared, enforceable contract across agent input, model calls, tool calls, and final output.

| Capability | Use it for |
|---|---|
| **Agent Hooks** | Standardized policy decisions, transforms, approvals, budgets, and egress controls across the agent lifecycle. |
| [Agent middleware](../concepts/agents/middleware/index.md) | Application-specific cross-cutting behavior that doesn't need the Agent Hooks contract or its core runtime guarantees. |
| [Agent Security with FIDES](./security.md) | Deterministic information-flow labels and policies for untrusted or confidential content. |
| [Tool approval](./tools/tool-approval.md) | Human confirmation of individual function-tool calls. |
| [Observability](./observability.md) | Passive traces, metrics, and logs that don't control execution. |

## What Agent Framework enforces

When you add Agent Hooks to an agent, Agent Framework applies a coordinated enforcement boundary across agent runs, model calls, and tool calls. The runtime provides the following guarantees:

- **Fail closed:** A deny blocks the guarded action. Invalid contexts, invalid verdicts, interceptor failures, and enforcement failures don't silently bypass controls.
- **Transform write-back:** A transform changes the native messages, tool arguments, tool results, or final response that execution actually uses. If a transform can't be applied, the run fails closed.
- **Buffered streaming:** No response update reaches the caller until the complete model response and final output pass their interception points.
- **Verdict-gated persistence:** Persistence waits for the verdict that covers it. Standard after-run persistence waits for `output`; per-service-call history persistence waits for each `post_model_call`.
- **Complete bundle installation:** The agent, chat, and function parts are installed as one unit, so an incomplete enforcement boundary can't be configured accidentally.

The contract is cooperative rather than a process isolation boundary. Interceptors run in the host process and receive the content needed to make decisions. Only register interceptors you trust.

## Install Agent Hooks

Install the optional `agent-hooks` extra for the core package:

```bash
pip install "agent-framework-core[agent-hooks]"
```

If you use `uv`:

```bash
uv add "agent-framework-core[agent-hooks]"
```

The `agent-hooks-sdk` dependency is lazy-imported. Importing `agent_framework` doesn't load the SDK unless you create an Agent Hooks middleware bundle.

> [!NOTE]
> The `agent-hooks` extra is intentionally not included in `agent-framework-core[all]`. Install it explicitly when you want to enable this experimental control surface.

## Add an interceptor

An interceptor receives an `agent_hooks.AgentContext` (the specification's context mapping, not the `agent_framework.AgentContext` used by agent middleware) and returns a verdict. The following interceptor blocks final output containing the word `secret`. The example assumes `client` is an already configured Agent Framework chat client.

```python
from agent_framework import Agent, create_agent_hooks_middleware
from agent_hooks import ALLOW, AgentContext, InterceptionBlocked, Verdict


class SecretEgressGuard:
    def intercept(self, context: AgentContext) -> Verdict:
        if (
            context["interception_point"] == "output"
            and "secret" in str(context["target"]).lower()
        ):
            return Verdict.deny(
                reason="secret_in_output",
                message="The final response contains restricted content.",
            )
        return ALLOW


hooks = create_agent_hooks_middleware(
    {"secret-egress": SecretEgressGuard()},
)

agent = Agent(
    client=client,
    instructions="You are a helpful assistant.",
    middleware=[hooks],
)

try:
    response = await agent.run("Summarize the account details.")
except InterceptionBlocked as exc:
    print(f"Blocked: {exc.result.verdict.reason}")
```

Pass the bundle as one element of the agent's `middleware` list. Install exactly one Agent Hooks bundle on each agent.

## Interception points

Agent Framework emits the applicable interception points automatically:

| Interception point | When it's emitted | Transform target |
|---|---|---|
| `agent_startup` | Before the first input in an Agent Hooks session | Not transformable |
| `input` | When an external request enters the agent | Input content and role |
| `pre_model_call` | Before each model request | Messages sent to the model |
| `post_model_call` | After each complete model response | Response content, framework-executed tool calls, and finish reason |
| `pre_tool_call` | Before each framework-executed tool invocation | Tool arguments |
| `post_tool_call` | After a tool succeeds or fails | Tool result |
| `output` | Before the final response reaches the caller | Final response content |
| `agent_shutdown` | When the Agent Hooks session completes, fails, or is canceled | Not transformable |

A run that calls a tool typically emits:

`agent_startup` → `input` → `pre_model_call` → `post_model_call` → `pre_tool_call` → `post_tool_call` → `pre_model_call` → `post_model_call` → `output` → `agent_shutdown`

## Verdicts

The contract has three decisions: `allow`, `deny`, and `transform`. The Python SDK also provides helpers for warnings and liftable denies.

| Result | Python API | Behavior |
|---|---|---|
| Allow | `ALLOW` or `Verdict(decision=Decision.ALLOW)` | Continue with the target unchanged. |
| Allow with warning | `Verdict.warn(...)` | Continue and include the warning in the interception record. |
| Deny | `Verdict.deny(...)` | Block the guarded action. |
| Deny pending approval | `Verdict.escalate(...)` | Block unless the configured approval resolver returns a permit verdict. |
| Transform | `Verdict(decision=Decision.TRANSFORM, transform=Transform(...))` | Rewrite a value under `$target`, then continue with the rewritten value. |

Run-level and model-level denies raise `InterceptionBlocked` and prevent the guarded result from reaching the caller or the next stage. At a tool seam, a policy deny prevents the tool action or discards its result and returns a control error containing the policy reason, without the denied target payload, to the model. This allows the agent loop to continue. A host or enforcement failure halts the run.

### Apply a transform

A transform path must start at `$target`. For example, an interceptor can replace final response content:

```python
from agent_hooks import ALLOW, AgentContext, Decision, Transform, Verdict


class OutputRedactor:
    def intercept(self, context: AgentContext) -> Verdict:
        if context["interception_point"] != "output":
            return ALLOW

        return Verdict(
            decision=Decision.TRANSFORM,
            reason="redacted_output",
            transform=Transform(
                path="$target.content",
                value="[Response removed by policy]",
            ),
        )
```

Transforms are applied to Agent Framework `Content` values, preserving supported rich content rather than reducing every value to plain text. A malformed path or incompatible replacement fails closed instead of continuing with the original value.

### Tool approval and argument transforms

Agent Framework tool approval and the Agent Hooks approval seam are separate mechanisms. For a function tool with `approval_mode="always_require"`, Agent Framework creates the human approval request before function middleware runs. A `pre_tool_call` transform can therefore change arguments after the user approved the original values.

> [!WARNING]
> Don't transform arguments at `pre_tool_call` for tools that use `approval_mode="always_require"`. Transform the tool call at `post_model_call` so the framework approval request contains the transformed values, or return `Verdict.escalate(...)` at `pre_tool_call` and resolve approval through the Agent Hooks `resolver`.

## Streaming and persistence

Agent Hooks keeps the streaming API but uses buffered-output semantics. Agent Framework assembles the complete model response, emits `post_model_call`, assembles the final agent response, and emits `output` before releasing any updates. If either point denies the response, the caller receives no partial updates.

This behavior trades token-by-token latency for fail-closed output enforcement. An output transform is also reflected in the updates eventually released to the caller.

Persistence is gated by the interception point that covers the persistence operation:

- By default, history and other after-run provider work wait for the `output` verdict. A denied output isn't persisted, and an output transform is persisted after transformation.
- When you set `require_per_service_call_history_persistence=True` on the `Agent` constructor or `client.as_agent(...)`, each model exchange is persisted after its `post_model_call` verdict permits it. A later `output` deny doesn't roll back that already permitted history.
- For default after-run persistence, retry attempts remain behind the final `output` decision. Per-service-call mode instead persists each model response that passes `post_model_call`.

> [!IMPORTANT]
> If model content must not become durable, enforce that policy at `post_model_call` when `require_per_service_call_history_persistence=True`. An output-only egress policy protects what reaches the caller, but it doesn't retroactively remove model exchanges already permitted and persisted at `post_model_call`.

## Sessions and audit records

By default, each agent run creates one Agent Hooks session. `agent_startup` and `agent_shutdown` bracket the run, and records receive one session ID with a monotonically increasing sequence.

Use `record_sink` to receive each `InterceptionRecord`:

```python
records = []

hooks = create_agent_hooks_middleware(
    {"secret-egress": SecretEgressGuard()},
    record_sink=records.append,
)
```

Interception records capture the decision, reason, interceptor summary, mode, identity, and sequence without copying the intercepted payload into the audit record. The interceptor itself still receives the full context.

### Span multiple runs with one session

Use `create_agent_hooks_middleware_from_emitter()` when the application owns a longer-lived Agent Hooks session, such as a conversation with one approval ledger:

```python
from agent_framework import Agent, create_agent_hooks_middleware_from_emitter
from agent_hooks import AgentContextBuilder, InterceptionEmitter


emitter = InterceptionEmitter().register(SecretEgressGuard())
builder = AgentContextBuilder(
    agent_id="support-agent",
    framework="agent-framework",
    session_id="conversation-42",
)

hooks = create_agent_hooks_middleware_from_emitter(emitter, builder)
agent = Agent(client=client, middleware=[hooks])

await emitter.emit(builder.agent_startup(tools_registered=[]))
await agent.run("First turn")
await agent.run("Second turn")
await emitter.emit(builder.agent_shutdown(reason="completed"))
```

In this form, the application configures the emitter and owns startup, shutdown, and error cleanup. The middleware emits the per-run points from `input` through `output`.

## Configure enforcement

`create_agent_hooks_middleware()` accepts the following controls:

| Parameter | Purpose |
|---|---|
| `interceptors` | A sequence of interceptors or a name-to-interceptor mapping. At least one is required. |
| `resolver` | Resolves liftable denies through an approval channel. Without a resolver, the deny remains in effect. |
| `mode` | `"enforce"` applies verdicts. `"evaluate_only"` records what would happen but allows every action. |
| `composition` | Selects how multiple interceptor verdicts are combined. |
| `identity_provider` | Produces content-bound context identities. The default is `"jcs-sha256"`. |
| `timeout` | Per-interceptor and resolver timeout for awaitable calls. The default is five seconds. A synchronous interceptor or resolver that blocks the event loop can't be preempted by this timeout. |
| `record_sink` | Receives each payload-free interception record. |

The default composition is sequential `first_deny` with approval configured to stop the fold. Interceptor order therefore matters: put controls that must always run before controls that can request approval. See the Agent Hooks [production checklist](https://github.com/responsibleai/agent-hooks/blob/main/docs/PRODUCTION.md) before selecting another composition profile.

### Roll out with evaluate-only mode

Use `evaluate_only` to measure policy behavior before enforcement:

```python
hooks = create_agent_hooks_middleware(
    {"secret-egress": SecretEgressGuard()},
    mode="evaluate_only",
    record_sink=records.append,
)
```

In this mode, interceptors run and records include their verdicts, but no action is blocked or transformed. Don't describe an `evaluate_only` deployment as enforced governance.

## Composition rules

Place the bundle first in the agent's middleware list so it forms the outermost enforcement boundary:

```python
agent = Agent(
    client=client,
    middleware=[
        create_agent_hooks_middleware([SecretEgressGuard()]),
        application_middleware,
    ],
)
```

Follow these rules:

- Install exactly one Agent Hooks bundle per agent. Stacked bundles are rejected.
- Keep the bundle intact. Its agent, chat, and function middleware can't be installed separately.
- Install the bundle on `Agent`, not directly on a chat client or through a context provider.
- Middleware placed before the bundle is outside the enforcement boundary. Treat outer position as outer trust.
- Give each nested agent its own bundle when its internal model and tool activity also needs interception.

## Current limitations

- **Python only:** Agent Hooks isn't yet implemented in the .NET or Go SDKs.
- **Experimental API:** Factory signatures and behavior can change before general availability.
- **Buffered streaming:** Updates aren't released token by token because output must be complete before a fail-closed verdict.
- **Hosted tools:** Tools executed by a model provider don't pass through Agent Framework's function-invocation seam. Their calls and outputs are surfaced in `post_model_call`, but `pre_tool_call` and `post_tool_call` can't block the provider's server-side execution.
- **Cooperative boundary:** Agent Hooks doesn't sandbox interceptors or protect against a hostile host. Code paths that bypass the guarded agent pipeline aren't covered.
- **Interceptor availability affects agent availability:** In enforce mode, an interceptor failure or timeout blocks the guarded action by design.

For production rollout, failure reasons, and alerting guidance, see the Agent Hooks [operations runbook](https://github.com/responsibleai/agent-hooks/blob/main/docs/OPERATIONS.md).

::: zone-end

::: zone pivot="programming-language-go"

Agent Hooks isn't yet available for Go. Use [agent middleware](../concepts/agents/middleware/index.md), [tool approval](./tools/tool-approval.md), and [agent safety](../concepts/agents/safety.md) to add runtime controls to Go agents.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Understand the agent pipeline](../concepts/agents/agent-pipeline.md)

### Related content

- [Agent middleware](../concepts/agents/middleware/index.md)
- [Agent safety](../concepts/agents/safety.md)
- [Tool approval](./tools/tool-approval.md)
- [Agent Security with FIDES](./security.md)
- [Observability](./observability.md)
- [AGENT-HOOKS-0.1 specification](https://github.com/responsibleai/agent-hooks/blob/main/spec/AGENT-HOOKS-0.1.md)
