---
title: Hyperlight CodeAct
description: Use the Hyperlight connector to add CodeAct and sandboxed Python execution to Agent Framework.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 04/21/2026
ms.service: agent-framework
---
<!--
  Language parity table - keep in sync when adding/removing sections.

  | Section                         | C# | Python | Notes                             |
  |---------------------------------|:--:|:------:|-----------------------------------|
  | Why Hyperlight CodeAct          | ✅ |   ✅   | Shared integration overview       |
  | Getting started                 | ✅ |   ✅   | C# tab is currently a placeholder |
  | Package installation            | ❌ |   ✅   | Python-only today                 |
  | HyperlightCodeActProvider       | ❌ |   ✅   | Python-only today                 |
  | Approvals and host tools        | ❌ |   ✅   | Python-only today                 |
  | HyperlightExecuteCodeTool       | ❌ |   ✅   | Python-only today                 |
  | Filesystem and network settings | ❌ |   ✅   | Python-only today                 |
  | Output guidance                 | ❌ |   ✅   | Python-only today                 |
  | Benchmark framing               | ❌ |   ✅   | Python-only today                 |
  | Current limitations             | ❌ |   ✅   | Python-only today                 |
-->

# Hyperlight CodeAct

Hyperlight is the currently documented backend for CodeAct in Agent Framework. It exposes an `execute_code` tool backed by an isolated sandbox runtime and can call provider-owned host tools through `call_tool(...)`.

For the pattern-level overview, see [CodeAct](../agents/code_act.md).

## Why Hyperlight CodeAct

Modern agents are often limited more by tool-calling overhead than by the model itself. A task that reads data, performs light computation, and assembles a result can easily turn into a chain of model -> tool -> model -> tool interactions, even when each individual step is simple.

Hyperlight-backed CodeAct collapses that loop. The model writes one short Python program, the sandbox executes it once, and provider-owned tools are reached from inside the sandbox with `call_tool(...)`. In representative tool-heavy workloads, that shift can cut latency roughly in half and token usage by more than 60%, while keeping the execution isolated and auditable.

::: zone pivot="programming-language-csharp"

## Get started

Coming soon.

::: zone-end

::: zone pivot="programming-language-python"

## Install the package

```bash
pip install agent-framework-hyperlight --pre
```

`agent-framework-hyperlight` ships separately from `agent-framework-core`, so you only take on the sandbox runtime when you need it.

> [!NOTE]
> The package depends on Hyperlight sandbox components. If the backend is not published for your current platform yet, `execute_code` fails when it tries to create the sandbox.

## Use `HyperlightCodeActProvider`

`HyperlightCodeActProvider` is the recommended entry point when you want CodeAct added automatically for each run. It injects run-scoped CodeAct instructions plus the `execute_code` tool, while keeping provider-owned tools off the direct agent tool surface.

:::code language="python" source="~/../agent-framework-code/python/packages/hyperlight/samples/codeact_context_provider.py" range="125-157":::

Tools registered on the provider are available inside the sandbox through `call_tool(...)`, but they are not exposed as direct agent tools. The provider also exposes CRUD-style management for tools, file mounts, and outbound allow-list entries through methods such as `add_tools(...)`, `remove_tool(...)`, `add_file_mounts(...)`, and `add_allowed_domains(...)`.

## How approvals and host tools work

Agent Framework tools carry an `approval_mode` that controls whether they can be auto-invoked or must pause for user approval.

The main difference between registering a tool on `HyperlightCodeActProvider` and registering it directly on `Agent(tools=...)` is **how the tool is invoked**, not where the Python function ultimately runs:

- Tools registered on `HyperlightCodeActProvider(tools=...)` are hidden from the model as direct tools. The model reaches them by writing code that calls `call_tool("name", ...)` inside `execute_code`.
- Tools registered on `Agent(tools=...)` are surfaced to the model as first-class tools, and each direct call honors that tool's own `approval_mode`.

`call_tool(...)` is a bridge back to host callbacks; it is not an in-sandbox reimplementation of the tool. That means provider-owned tools still execute in the host process, with whatever filesystem, network, and credentials the host process itself can access.

As a rule of thumb:

- Put cheap, deterministic, safe-to-chain tools on the provider so the model can compose many calls inside one `execute_code` turn.
- Keep side-effecting or approval-gated operations as direct agent tools, often with `approval_mode="always_require"`, so each invocation stays individually visible and approvable.

Because host tools run outside the sandbox, `file_mounts` and `allowed_domains` constrain the sandboxed code itself, not the host callback behind `call_tool(...)`. When you need controlled access to a sensitive resource, prefer a narrow host tool over broadening sandbox permissions.

## Use `HyperlightExecuteCodeTool` for direct wiring

When you need to mix `execute_code` with direct-only tools on the same agent, use `HyperlightExecuteCodeTool` instead of the provider. For fixed configurations, you can build the CodeAct instructions once and wire the tool directly:

```python
from agent_framework_hyperlight import HyperlightExecuteCodeTool

execute_code = HyperlightExecuteCodeTool(
    tools=[compute],
    approval_mode="never_require",
)

codeact_instructions = execute_code.build_instructions(tools_visible_to_model=False)
```

This pattern is useful when the CodeAct surface is fixed and you do not need the provider lifecycle on every run. Unlike `HyperlightCodeActProvider`, the standalone tool does not inject prompt guidance automatically, so you are responsible for adding the `build_instructions(...)` output to the agent instructions yourself.

## Configure files and outbound access

Hyperlight can expose a read-only `/input` tree plus a writable `/output` area for generated artifacts.

- Use `workspace_root` to make a workspace available under `/input/`.
- Use `file_mounts` to map specific host paths into the sandbox.
- Use `allowed_domains` to enable outbound access only for specific targets or methods.

`file_mounts` accepts a shorthand string, an explicit `(host_path, mount_path)` pair, or a `FileMount` named tuple. `allowed_domains` accepts a string target, an explicit `(target, method-or-methods)` pair, or an `AllowedDomain` named tuple.

```python
from agent_framework_hyperlight import HyperlightCodeActProvider

codeact = HyperlightCodeActProvider(
    tools=[compute],
    file_mounts=[
        "/host/data",
        ("/host/models", "/sandbox/models"),
    ],
    allowed_domains=[
        "api.github.com",
        ("internal.api.example.com", "GET"),
    ],
)
```

## Output guidance

To surface text from `execute_code`, end the code with `print(...)`; Hyperlight does not return the value of the last expression automatically.

When filesystem access is enabled, write larger artifacts to `/output/<filename>` instead. Returned files are attached to the tool result, while files under `/input` are available for reading inside the sandbox.

## Compare CodeAct and direct tool calling

The benchmark sample runs the same task with the same client, model, tools, prompt, and structured output schema once through traditional tool calling and once through Hyperlight-backed CodeAct. The only difference is the wiring: direct tools versus a single `execute_code` tool backed by `HyperlightCodeActProvider`.

:::code language="python" source="~/../agent-framework-code/python/packages/hyperlight/samples/codeact_benchmark.py" range="166-195":::

In that sample, the agent computes grand totals across a dataset of users and orders by repeatedly looking up data and performing light computation. That is exactly the kind of many-small-steps workflow where CodeAct can remove orchestration overhead. The full sample prints elapsed time and token usage for both runs so you can compare the execution shape in your own environment.

## Current limitations

This package is still alpha, and a few constraints are worth planning around:

1. Platform support follows the published Hyperlight backend packages. Today that means supported Linux and Windows environments; unsupported platforms will fail when creating the sandbox.
2. The current integration executes Python guest code. The .NET documentation is still coming soon.
3. In-memory interpreter state does not persist across separate `execute_code` calls. Use mounted files and `/output` artifacts when data needs to survive across calls.
4. Approval applies to the `execute_code` invocation as a whole, not to each individual `call_tool(...)` inside the same code block.
5. Tool descriptions, parameter annotations, and return shapes matter more here because the model is writing code against that contract rather than choosing isolated direct tool calls.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Purview](purview.md)

### Related content

- [CodeAct](../agents/code_act.md)
- [CodeAct paper](https://arxiv.org/abs/2402.01030)
- [Context Providers](../agents/conversations/context-providers.md)
- [Tool Approval](../agents/tools/tool-approval.md)
- [Hyperlight provider sample](https://github.com/microsoft/agent-framework/blob/main/python/packages/hyperlight/samples/codeact_context_provider.py)
- [Hyperlight benchmark sample](https://github.com/microsoft/agent-framework/blob/main/python/packages/hyperlight/samples/codeact_benchmark.py)
