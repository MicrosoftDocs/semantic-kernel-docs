---
title: Compaction
description: Learn how to manage conversation history size with compaction strategies that keep context within token limits.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: conceptual
ms.author: crickman
ms.date: 03/05/2026
ms.service: agent-framework
---

# Compaction

As conversations grow, the token count of the chat history can exceed model context windows or drive up costs. Compaction strategies reduce the size of conversation history while preserving important context, so agents can continue functioning over long-running interactions.

:::zone pivot="programming-language-csharp"

> [!IMPORTANT]
> The compaction framework is currently experimental. To use it, you will need to add `#pragma warning disable MAAI001`.

:::zone-end

:::zone pivot="programming-language-python"

> [!NOTE]
> Compaction strategies are not yet available in the Python SDK. This page documents the C# implementation. Python support is planned for a future release.

:::zone-end

## Why compaction matters

Every call to an LLM includes the full conversation history. Without compaction:

- **Token limits** — Conversations eventually exceed the model's context window, causing errors.
- **Cost** — Larger prompts consume more tokens, increasing API costs.
- **Latency** — More input tokens means slower response times.

Compaction solves these problems by selectively removing, collapsing, or summarizing older portions of the conversation.

## Core concepts

:::zone pivot="programming-language-csharp"

Compaction operates on a **`MessageIndex`** — a structured view of the flat message list that groups messages into atomic units called **`MessageGroup`** instances. Each group tracks its message count, byte count, and estimated token count.

### Message groups

A `MessageGroup` represents logically related messages that must be kept or removed together. For example, an assistant message containing tool calls and its corresponding tool result messages form an atomic group — removing one without the other would cause LLM API errors.

Each group has a `MessageGroupKind`:

| Kind | Description |
|---|---|
| `System` | One or more system messages. Always preserved during compaction. |
| `User` | A single user message that starts a new turn. |
| `AssistantText` | A plain assistant text response (no tool calls). |
| `ToolCall` | An assistant message with tool calls and the corresponding tool result messages, treated as an atomic unit. |
| `Summary` | A condensed message produced by summarization compaction. |

### Triggers

A `CompactionTrigger` is a delegate that evaluates whether compaction should proceed based on current `MessageIndex` metrics:

```csharp
public delegate bool CompactionTrigger(MessageIndex index);
```

The `CompactionTriggers` class provides common factory methods:

| Trigger | Fires when |
|---|---|
| `CompactionTriggers.Always` | Every time (unconditionally). |
| `CompactionTriggers.Never` | Never (disables compaction). |
| `CompactionTriggers.TokensExceed(maxTokens)` | Included token count exceeds the threshold. |
| `CompactionTriggers.MessagesExceed(maxMessages)` | Included message count exceeds the threshold. |
| `CompactionTriggers.TurnsExceed(maxTurns)` | Included user turn count exceeds the threshold. |
| `CompactionTriggers.GroupsExceed(maxGroups)` | Included group count exceeds the threshold. |
| `CompactionTriggers.HasToolCalls()` | At least one non-excluded tool call group exists. |

Combine triggers with `CompactionTriggers.All(...)` (logical AND) or `CompactionTriggers.Any(...)` (logical OR):

```csharp
// Compact only when there are tool calls AND tokens exceed 2000
CompactionTrigger trigger = CompactionTriggers.All(
    CompactionTriggers.HasToolCalls(),
    CompactionTriggers.TokensExceed(2000));
```

### Trigger vs. target

Every strategy has two predicates:

- **Trigger** — Controls *when* compaction begins. If the trigger returns `false`, the strategy is skipped entirely.
- **Target** — Controls *when* compaction stops. Strategies incrementally exclude groups and re-evaluate the target after each step, stopping as soon as the target returns `true`.

When no target is specified, it defaults to the inverse of the trigger — compaction stops as soon as the trigger condition would no longer fire.

:::zone-end

:::zone pivot="programming-language-python"

> [!NOTE]
> Compaction strategies are not yet available in the Python SDK. This page documents the C# implementation. Python support is planned for a future release.

:::zone-end

## Compaction strategies

:::zone pivot="programming-language-csharp"

All strategies inherit from the abstract `CompactionStrategy` base class. Each strategy preserves system messages and respects a `MinimumPreserved` floor that protects the most-recent non-system groups from removal.

### TruncationCompactionStrategy

The most straightforward approach: removes the oldest non-system message groups until the target condition is met.

- Respects atomic group boundaries (tool call + result messages are removed together).
- `MinimumPreserved` defaults to `32`.
- Best for hard token-budget backstops.

```csharp
// Drop oldest groups when tokens exceed 32K, keeping at least 10 recent groups
TruncationCompactionStrategy truncation = new(
    trigger: CompactionTriggers.TokensExceed(0x8000),
    minimumPreserved: 10);
```

### SlidingWindowCompactionStrategy

Removes the oldest user **turns** and their associated response groups. Unlike truncation, this strategy operates on logical turn boundaries rather than individual groups.

- A turn starts with a user message and includes all subsequent assistant and tool-call groups until the next user message.
- `MinimumPreserved` defaults to `1` (preserves at least the most recent non-system group).
- Best for bounding conversation length predictably.

```csharp
// Keep only the last 4 user turns
SlidingWindowCompactionStrategy slidingWindow = new(
    trigger: CompactionTriggers.TurnsExceed(4));
```

### ToolResultCompactionStrategy

The gentlest strategy: collapses old tool call groups into single concise assistant messages while leaving user messages and plain assistant responses untouched.

- Replaces multi-message tool call groups (assistant call + tool results) with a short summary like `[Tool calls: get_weather, search_docs]`.
- Does not remove any user messages or plain assistant responses.
- `MinimumPreserved` defaults to `2`, ensuring the current turn's tool interactions remain visible.
- Best as a first-pass compaction to reclaim space from verbose tool results.

```csharp
// Collapse old tool results when tokens exceed 512
ToolResultCompactionStrategy toolCompaction = new(
    trigger: CompactionTriggers.TokensExceed(0x200));
```

### SummarizationCompactionStrategy

Uses an LLM to summarize older portions of the conversation, replacing them with a single summary message.

- Protects system messages and the most recent `MinimumPreserved` non-system groups (default: `4`).
- Sends the older messages to a separate `IChatClient` with a summarization prompt, then inserts the summary as a `MessageGroupKind.Summary` group.
- The default prompt preserves key facts, decisions, user preferences, and tool call outcomes.
- Requires a separate `IChatClient` instance for summarization — a smaller, faster model is recommended.
- Best for preserving conversational context while significantly reducing token count.

```csharp
// Summarize older messages when tokens exceed 1280, keeping the last 4 groups
SummarizationCompactionStrategy summarization = new(
    chatClient: summarizerChatClient,
    trigger: CompactionTriggers.TokensExceed(0x500),
    minimumPreserved: 4);
```

You can provide a custom summarization prompt:

```csharp
SummarizationCompactionStrategy summarization = new(
    chatClient: summarizerChatClient,
    trigger: CompactionTriggers.TokensExceed(0x500),
    summarizationPrompt: "Summarize the key decisions and user preferences only.");
```

### PipelineCompactionStrategy

Composes multiple strategies into a sequential pipeline. Each strategy operates on the result of the previous one, enabling layered compaction from gentle to aggressive.

- The pipeline's own trigger is `CompactionTriggers.Always` — each child strategy evaluates its own trigger independently.
- Strategies execute in order, so put the gentlest strategies first.

```csharp
PipelineCompactionStrategy pipeline = new(
    new ToolResultCompactionStrategy(CompactionTriggers.TokensExceed(0x200)),
    new SummarizationCompactionStrategy(summarizerChatClient, CompactionTriggers.TokensExceed(0x500)),
    new SlidingWindowCompactionStrategy(CompactionTriggers.TurnsExceed(4)),
    new TruncationCompactionStrategy(CompactionTriggers.TokensExceed(0x8000)));
```

This pipeline:

1. Collapses old tool results (gentle).
2. Summarizes older conversation spans (moderate).
3. Keeps only the last 4 user turns (aggressive).
4. Drops oldest groups if still over budget (emergency backstop).

:::zone-end

:::zone pivot="programming-language-python"

> [!NOTE]
> Compaction strategies are not yet available in the Python SDK. This page documents the C# implementation. Python support is planned for a future release.

:::zone-end

## Using compaction with an agent

:::zone pivot="programming-language-csharp"

Assign a compaction strategy via the `CompactionStrategy` property on `ChatClientAgentOptions`. The framework automatically applies compaction before each LLM call during the tool loop.

```csharp
IChatClient agentChatClient = openAIClient.GetChatClient(deploymentName).AsIChatClient();
IChatClient summarizerChatClient = openAIClient.GetChatClient(deploymentName).AsIChatClient();

PipelineCompactionStrategy compactionPipeline =
    new(
        new ToolResultCompactionStrategy(CompactionTriggers.TokensExceed(0x200)),
        new SummarizationCompactionStrategy(summarizerChatClient, CompactionTriggers.TokensExceed(0x500)),
        new SlidingWindowCompactionStrategy(CompactionTriggers.TurnsExceed(4)),
        new TruncationCompactionStrategy(CompactionTriggers.TokensExceed(0x8000)));

AIAgent agent =
    agentChatClient.AsAIAgent(
        new ChatClientAgentOptions
        {
            Name = "ShoppingAssistant",
            ChatOptions = new()
            {
                Instructions = "You are a helpful shopping assistant.",
                Tools = [AIFunctionFactory.Create(LookupPrice)],
            },
            CompactionStrategy = compactionPipeline,
        });

AgentSession session = await agent.CreateSessionAsync();
Console.WriteLine(await agent.RunAsync("What's the price of a laptop?", session));
```

> [!TIP]
> Use a smaller, cheaper model (such as `gpt-4o-mini`) for the summarization chat client to reduce costs while maintaining summary quality.

:::zone-end

:::zone pivot="programming-language-python"

> [!NOTE]
> Compaction strategies are not yet available in the Python SDK. This page documents the C# implementation. Python support is planned for a future release.

:::zone-end

:::zone pivot="programming-language-csharp"

## Choosing a strategy

| Strategy | Aggressiveness | Preserves context | Requires LLM | Best for |
|---|---|---|---|---|
| `ToolResultCompactionStrategy` | Low | High — only collapses tool results | No | Reclaiming space from verbose tool output |
| `SummarizationCompactionStrategy` | Medium | Medium — replaces history with a summary | Yes | Long conversations where context matters |
| `SlidingWindowCompactionStrategy` | High | Low — drops entire turns | No | Hard turn-count limits |
| `TruncationCompactionStrategy` | High | Low — drops oldest groups | No | Emergency token-budget backstops |
| `PipelineCompactionStrategy` | Configurable | Depends on child strategies | Depends | Layered compaction with multiple fallbacks |

:::zone-end

:::zone pivot="programming-language-python"

> [!NOTE]
> Compaction strategies are not yet available in the Python SDK. This page documents the C# implementation. Python support is planned for a future release.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Context Providers](context-providers.md)

> [!div class="nextstepaction"]
> [Storage](storage.md)
