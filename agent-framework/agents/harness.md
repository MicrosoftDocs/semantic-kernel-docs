---
title: Agent Harnesses
description: Learn what an agent harness is and how to use the batteries-included HarnessAgent (C#) and create_harness_agent (Python) to build capable, autonomous agents.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: article
ms.author: westey
ms.date: 06/26/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                          | C# | Python | Notes                         |
  |----------------------------------|:--:|:------:|-------------------------------|
  | What is an agent harness?        | ✅ |   ✅   | Shared concept                |
  | What makes up a harness          | ✅ |   ✅   | Shared concept                |
  | Creating a harness agent         | ✅ |   ✅   |                               |
  | Enabling compaction              | ✅ |   ✅   |                               |
  | Customizing and disabling        | ✅ |   ✅   |                               |
  | Plan and execute workflow        | ✅ |   ✅   | Shared concept                |
  | Looping until done               | ✅ |   ✅   |                               |
  | Shell and background agents      | ✅ |   ✅   |                               |
  | A sample terminal UX             | ✅ |   ✅   |                               |
-->

# Agent harnesses

An *agent harness* is the scaffolding that turns a language model into an agent that can actually *do* things. A model on its own can only generate text. To have it call tools, work through multi-step tasks, remember what it has done, and keep going until the job is finished, you need a runtime wrapped around the model — and that runtime is the harness.

A harness drives the agent: it runs the loop that calls the model and executes the tools the model asks for, manages conversation history and context so the model stays within its limits, applies approval and safety policies before actions are taken, and keeps the agent progressing toward task completion. Coding assistants and autonomous agents are all built on some form of harness — it's the engine wrapped around the model.

Agent Framework provides a ready-made harness so you don't have to build this scaffolding yourself. It is an opinionated, batteries-included agent that wraps a chat client with a complete agentic pipeline — function invocation, context management, and a curated set of tools and providers — tuned for long-running, autonomous work such as research, coding, data analysis, and general task automation.

You still supply your own chat client and only configure the parts you want to change. Everything else has a sensible default that you can disable or customize.

Internally, the Agent Framework harness is a chat client based agent (`Agent` in Python and `ChatClientAgent` in C#) with a set of Agent Framework features added. All these features are also available as standalone features in Agent Framework.

## What makes up the Agent Framework harness

The Agent Framework harness bundles the following capabilities into a single agent. Each one is enabled by default (unless noted as optional) and can be individually disabled or customized.

| Capability | Description |
|---|---|
| **Function invocation** | Automatic tool-calling loop with a configurable iteration limit. |
| **Per-service-call history persistence** | Chat history is persisted after every individual model call, enabling crash recovery and inspection mid-run. |
| **Compaction** | Context-window compaction keeps long tool-calling loops from overflowing the context window. Active when a token budget (or a custom strategy) is provided. |
| **Todo provider** | A persistent todo list the agent uses to track multi-step plans. |
| **Agent mode provider** | Plan/execute/custom mode tracking that structures how the agent works. |
| **File memory provider** | File-based session memory for notes and artifacts that persist across turns. |
| **File access provider** | Read/write file tools scoped to a working directory. |
| **Tool approval** | "Don't ask again" standing approval rules plus heuristic auto-approval for safe, unattended execution. |
| **OpenTelemetry** | Built-in observability following the generative-AI semantic conventions. |
| **Web search** | A hosted web search tool added by default. |
| **Skills provider** *(optional)* | Discovers and progressively loads [Agent Skills](./skills.md) from the file system. |
| **Background agents** *(optional)* | Delegate parallel work to background sub-agents. |
| **Shell environment** *(optional)* | Shell command execution plus OS/shell/working-directory probing. |
| **Looping** *(optional)* | Re-invoke the agent until a completion condition is satisfied. |

::: zone pivot="programming-language-csharp"

## Creating a harness agent

The harness is exposed as the `HarnessAgent` class in the `Microsoft.Agents.AI` namespace (the `Microsoft.Agents.AI.Harness` package). The simplest way to create one is from any [`IChatClient`](/dotnet/ai/microsoft-extensions-ai#the-ichatclient-interface) using the `AsHarnessAgent` extension method:

```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// chatClient is any IChatClient implementation (Foundry, Azure OpenAI, OpenAI, Anthropic, ...).
AIAgent agent = chatClient.AsHarnessAgent();

AgentResponse response = await agent.RunAsync("Plan a weekend trip to Seattle.");
Console.WriteLine(response.Text);
```

You can also construct the agent directly:

```csharp
AIAgent agent = new HarnessAgent(chatClient);
```

Provide a `HarnessAgentOptions` to supply instructions and tools. Harness-level instructions (`HarnessAgentOptions.HarnessInstructions`) describe general operating guidelines, while task-specific instructions go on `ChatOptions.Instructions`.
The `HarnessAgent` comes with default harness-level instructions (`HarnessAgent.DefaultInstructions`), but you can override with your own via `HarnessAgentOptions.HarnessInstructions`.

```csharp
AIAgent agent = chatClient.AsHarnessAgent(new HarnessAgentOptions
{
    Name = "research-agent",
    ChatOptions = new ChatOptions
    {
        Instructions = "You are a research assistant focused on academic sources.",
        Tools = [AIFunctionFactory.Create(GetStockPrice)],
    },
});
```

## Enabling compaction

Compaction prevents long tool-calling loops from overflowing the context window.
When not using inference-service-stored chat history, the default `InMemoryChatHistoryProvider` is also provided the same compaction provider so that session stored chat history is also compacted.
Supply both a maximum context-window size and a maximum output size to enable the default token-budget-aware strategy:

```csharp
AIAgent agent = chatClient.AsHarnessAgent(new HarnessAgentOptions
{
    MaxContextWindowTokens = 128_000,
    MaxOutputTokens = 16_384,
});
```

To use your own strategy, set `HarnessAgentOptions.CompactionStrategy`; to turn compaction off, set `DisableCompaction = true`.

## Customizing and disabling features

Every default capability has a corresponding disable flag on `HarnessAgentOptions`, so you can keep the pipeline you want and drop the rest:

```csharp
AIAgent agent = chatClient.AsHarnessAgent(new HarnessAgentOptions
{
    HarnessInstructions = "Custom operating guidelines here.",
    DisableTodoProvider = true,      // No todo list
    DisableAgentModeProvider = true, // No plan/execute modes
    DisableWebSearch = true,         // No hosted web search tool
    DisableFileMemory = true,        // No file-based session memory
});
```

Other flags include `DisableFileAccess`, `DisableAgentSkillsProvider`, `DisableToolAutoApproval`, and `DisableOpenTelemetry`. You can also add your own context providers via `AIContextProviders` and point the skills provider at custom locations via `AgentSkillsSource`.

## Looping until done

By default the harness runs once per invocation. Supply one or more `LoopEvaluator` instances to re-invoke the agent automatically until the evaluators decide it is finished (for example, when a completion marker appears, a predicate is satisfied, or an AI judge approves):

```csharp
AIAgent agent = chatClient.AsHarnessAgent(new HarnessAgentOptions
{
    LoopEvaluators = [new CompletionMarkerLoopEvaluator("DONE")],
});
```

The loop is applied as the outermost Agent decorator, so each iteration is a complete, independently tool-approved and traced agent run.

## Shell and background agents

To let the agent run shell commands, pass a `ShellExecutor`. This adds an approval-gated shell-execution tool and a provider that injects OS, shell, and working-directory information into context:

```csharp
using Microsoft.Agents.AI.Tools.Shell;

// A shell confined to a working directory. Commands require approval by default;
// the deny-list is a UX pre-filter, not a security boundary.
await using var shell = new LocalShellExecutor(new LocalShellExecutorOptions
{
    WorkingDirectory = workingDir,
    ConfineWorkingDirectory = true,
    Policy = new ShellPolicy(denyList: [@"\brm\s+-rf\b", @"\bsudo\b"]),
});

AIAgent agent = chatClient.AsHarnessAgent(new HarnessAgentOptions
{
    ShellExecutor = shell,
});
```

To enable parallel delegation, pass a set of background agents. The agent can hand off sub-tasks for concurrent execution:

```csharp
AIAgent agent = chatClient.AsHarnessAgent(new HarnessAgentOptions
{
    BackgroundAgents = [webSearchAgent, codeAgent],
});
```

::: zone-end

::: zone pivot="programming-language-python"

## Creating a harness agent

The harness is exposed as the `create_harness_agent` factory function, which assembles a fully configured `Agent` from a chat client. The simplest form requires only a client:

```python
from agent_framework import create_harness_agent
from agent_framework.openai import OpenAIChatClient

agent = create_harness_agent(
    OpenAIChatClient(model="gpt-4o"),
)

session = agent.create_session()
response = await agent.run("Plan a weekend trip to Seattle.", session=session)
print(response.text)
```

Harness-level instructions describe general operating guidelines, while task-specific instructions go in `agent_instructions`. The harness ships with default harness-level instructions (`DEFAULT_HARNESS_INSTRUCTIONS`), which you can override via `harness_instructions`. You can also pass extra tools:

```python
agent = create_harness_agent(
    client=client,
    name="research-agent",
    agent_instructions="You are a research assistant focused on academic sources.",
    tools=get_stock_price,
)
```

## Enabling compaction

Compaction prevents long tool-calling loops from overflowing the context window. Supply both the model's maximum context-window size and a maximum output size to enable the default token-budget-aware strategies:

```python
agent = create_harness_agent(
    client=client,
    max_context_window_tokens=128_000,
    max_output_tokens=16_384,
)
```

When neither token parameter nor a custom strategy is provided, compaction is automatically disabled. To use your own strategies, pass `before_compaction_strategy` and/or `after_compaction_strategy`; to turn compaction off explicitly, set `disable_compaction=True`.

## Customizing and disabling features

Every default capability has a corresponding `disable_*` keyword argument, so you can keep the parts you want and drop the rest:

```python
agent = create_harness_agent(
    client=client,
    harness_instructions="Custom operating guidelines here.",
    disable_todo=True,         # No todo list
    disable_mode=True,         # No plan/execute modes
    disable_web_search=True,   # No hosted web search tool
    disable_file_memory=True,  # No file-based session memory
)
```

Other flags include `disable_file_access`, `disable_tool_auto_approval`, and `disable_compaction`. You can point skill discovery at custom locations with `skills_paths` and add your own providers with `context_providers`.

## Looping until done

By default the harness runs once per invocation. Pass a `loop_should_continue` predicate to re-invoke the agent automatically until the predicate decides it is finished. Use `loop_next_message` to control the prompt for each follow-up iteration and `loop_max_iterations` to cap the number of passes:

```python
from agent_framework import create_harness_agent, todos_remaining

agent = create_harness_agent(
    client=client,
    loop_should_continue=todos_remaining(),
    loop_max_iterations=10,
)
```

The predicate is invoked with keyword arguments (`iteration`, `last_result`, `session`, `agent`, and so on); `todos_remaining` re-runs the agent while its todo list still has open items. To write your own, accept those keyword arguments — for example, `lambda *, last_result, **kwargs: "DONE" not in last_result.text`.

## Shell and background agents

To let the agent run shell commands, pass a `shell_executor` (for example, `LocalShellTool` from `agent-framework-tools`). This adds an approval-gated shell-execution tool plus a provider that probes the OS and shell environment. The caller owns the executor's lifecycle:

```python
from agent_framework_tools.shell import LocalShellTool, ShellPolicy

# A shell confined to a working directory. Commands require approval by default;
# the deny-list is a UX pre-filter, not a security boundary.
async with LocalShellTool(
    workdir="./working",
    confine_workdir=True,
    policy=ShellPolicy(denylist=[r"\brm\s+-rf\b", r"\bsudo\b"]),
) as shell:
    agent = create_harness_agent(
        client=client,
        shell_executor=shell,
    )
```

To enable parallel delegation, pass a sequence of background agents. The agent can hand off sub-tasks for concurrent execution:

```python
agent = create_harness_agent(
    client=client,
    background_agents=[web_search_agent, code_agent],
)
```

::: zone-end

## Plan and execute workflow

The agent mode provider enables a two-phase working style that pairs naturally with the todo list:

1. **Plan mode** — interactive. The agent asks clarifying questions, drafts a todo list and plan, and gets your approval before doing significant work.
2. **Execute mode** — autonomous. The agent works through the todos independently, reporting progress as it goes.

While the mode provider comes with plan and execute modes as the default modes, these can be replaced with other modes and custom instructions for each mode if required.

## A sample terminal UX

The harness gives you a capable agent but doesn't prescribe how people interact with it. To demonstrate the harness end to end, we include a **sample terminal UX** — an interactive console (TUI) that streams the agent's output, shows its todo list and current mode, surfaces tool-approval prompts, and supports slash commands such as `/todos`, `/mode`, and `/exit`.

> [!IMPORTANT]
> These console projects are **samples**, not part of the shipped framework. They're intentionally self-contained so you can run them as-is to explore the harness, or copy them into your own project as a starting point for building your own terminal experience.

::: zone pivot="programming-language-csharp"

The .NET sample console is the `Harness.Shared.Console` project. Its entry point is `HarnessConsole.RunAgentAsync`, taking your agent, a placeholder prompt, and an optional `HarnessConsoleOptions` (observers, slash-command handlers, mode colors):

```csharp
using Harness.Shared.Console;

await HarnessConsole.RunAgentAsync(agent, userPrompt: "Ask me anything to get started.");
```

Customize it with your own observers, tool formatters, and command handlers — or fork it as a base for your own terminal experience. See the [.NET harness samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/02-agents/Harness).

::: zone-end

::: zone pivot="programming-language-python"

The Python sample console is the `console` package next to the harness samples. Its entry point is `run_agent_async`, which runs a [Textual](https://textual.textualize.io/)-based app:

```python
from console import run_agent_async

await run_agent_async(agent)
```

It's organized around observers, UI components, and slash commands, all extensible via the `ConsoleObserver`, `ToolCallFormatter`, and `CommandHandler` base classes (depends on `textual` and `rich`). Run it as-is, or copy it as a base for your own terminal experience. See the [Python harness samples](https://github.com/microsoft/agent-framework/tree/main/python/samples/02-agents/harness).

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Agent Skills](skills.md)

### Go deeper

- [Context providers](./conversations/context-providers.md)
- [Compaction](./conversations/compaction.md)
- [Observability](./observability.md)
- [Tool approval](./tools/tool-approval.md)
