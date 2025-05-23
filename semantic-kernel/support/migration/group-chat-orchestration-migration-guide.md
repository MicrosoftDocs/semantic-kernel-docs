---
title: Group Chat Orchestration Migration Guide
description: Describes the steps for developers to update their AgentGroupChat to the new GroupChatOrchestration.
zone_pivot_groups: programming-languages
author: taochen
ms.topic: tutorial
ms.author: taochen
ms.date: 05/21/2025
ms.service: semantic-kernel
---

# `AgentGroupChat` Orchestration Migration Guide

This is a migration guide for developers who have been using the [`AgentGroupChat`](../archive/agent-chat.md) in Semantic Kernel and want to transition to the new [`GroupChatOrchestration`](../../Frameworks/agent/agent-orchestration/group-chat.md). The new class provides a more flexible and powerful way to manage group chat interactions among agents.

::: zone pivot="programming-language-csharp"

## Migrating from `AgentGroupChat` to `GroupChatOrchestration`

The new `GroupChatOrchestration` class replaces the `AgentGroupChat` with a unified, extensible orchestration model. Here’s how to migrate your C# code:

### Step 1: Replace Usings and Class References

- Remove any `using` statements or references to `AgentChat` and `AgentGroupChat`. For example, remove:

  ```csharp
  using Microsoft.SemanticKernel.Agents.Chat;
  ```

- Add a reference to the new orchestration namespace:

  ```csharp
  using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
  ```

### Step 2: Update Initialization

**Before:**

```csharp
AgentGroupChat chat = new(agentWriter, agentReviewer)
{
    ExecutionSettings = new()
    {
        SelectionStrategy = new CustomSelectionStrategy(),
        TerminationStrategy = new CustomTerminationStrategy(),
    }
};
```

**After:**

```csharp
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;

GroupChatOrchestration orchestration = new(
    new RoundRobinGroupChatManager(),
    agentWriter,
    agentReviewer);
```

### Step 3: Start the Group Chat

**Before:**

```csharp
chat.AddChatMessage(input);
await foreach (var response in chat.InvokeAsync())
{
    // handle response
}
```

**After:**

```csharp
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;

InProcessRuntime runtime = new();
await runtime.StartAsync();

OrchestrationResult<string> result = await orchestration.InvokeAsync(input, runtime);
string text = await result.GetValueAsync(TimeSpan.FromSeconds(timeout));
```

### Step 4: Customizing Orchestration

The new orchestration model allows you to create custom strategies for termination, agent selection, and more by sub-classing `GroupChatManager` and overriding its methods. Please refer to the [GroupChatOrchestration documentation](../../Frameworks/agent/agent-orchestration/group-chat.md#customize-the-group-chat-manager) for more details.

### Step 5: Remove Deprecated APIs

Remove any code that directly manipulates `AgentGroupChat`-specific properties or methods, as they are no longer maintained.

### Step 6: Review and Test

- Review your code for any remaining references to the old classes.
- Test your group chat scenarios to ensure the new orchestration behaves as expected.

## Full Example

This guide demonstrates how to migrate the core logic of [`Step03_Chat.cs`](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/Step03_Chat.cs) from `AgentGroupChat` to the new `GroupChatOrchestration`, including a custom group chat manager that implements the approval-based termination strategy.

### Step 1: Agent Definition

There are no changes needed in the agent definition. You can continue using the same `AgentWriter` and `AgentReviewer` as before.

### Step 2: Implement a Custom Group Chat Manager

Create a custom `GroupChatManager` that terminates the chat when the last message contains "approve" and only the reviewer can approve:

```csharp
private sealed class ApprovalGroupChatManager : RoundRobinGroupChatManager
{
    private readonly string _approverName;
    public ApprovalGroupChatManager(string approverName)
    {
        _approverName = approverName;
    }

    public override ValueTask<GroupChatManagerResult<bool>> ShouldTerminate(ChatHistory history, CancellationToken cancellationToken = default)
    {
        var last = history.LastOrDefault();
        bool shouldTerminate = last?.AuthorName == _approverName &&
            last.Content?.Contains("approve", StringComparison.OrdinalIgnoreCase) == true;
        return ValueTask.FromResult(new GroupChatManagerResult<bool>(shouldTerminate)
        {
            Reason = shouldTerminate ? "Approved by reviewer." : "Not yet approved."
        });
    }
}
```

### Step 3: Initialize the Orchestration

Replace the `AgentGroupChat` initialization with:

```csharp
var orchestration = new GroupChatOrchestration(
    new ApprovalGroupChatManager(ReviewerName)
    {
        MaximumInvocationCount = 10
    },
    agentWriter,
    agentReviewer);
```

### Step 4: Run the Orchestration

Replace the message loop with:

```csharp
var runtime = new InProcessRuntime();
await runtime.StartAsync();

var result = await orchestration.InvokeAsync("concept: maps made out of egg cartons.", runtime);
string text = await result.GetValueAsync(TimeSpan.FromSeconds(60));
Console.WriteLine($"\n# RESULT: {text}");

await runtime.RunUntilIdleAsync();
```

## Summary

- Use a custom `GroupChatManager` for approval-based termination.
- Replace the chat loop with orchestration invocation.
- The rest of your agent setup and message formatting can remain unchanged.

::: zone-end

::: zone pivot="programming-language-python"

## Migrating from `AgentGroupChat` to `GroupChatOrchestration`

The new `GroupChatOrchestration` API in Python replaces the older `AgentGroupChat` pattern, providing a more flexible and extensible way to manage multi-agent conversations. Here’s how to migrate your code:

### Step 1: Replace Imports and Class References

- Remove any imports or references to `AgentGroupChat` and related strategies. For example, remove:

    ```python
    from semantic_kernel.agents import AgentGroupChat
    ```

- Import the new orchestration classes:

  ```python
  from semantic_kernel.agents import GroupChatOrchestration, RoundRobinGroupChatManager
  from semantic_kernel.agents.runtime import InProcessRuntime
  ```

### Step 2: Update Initialization

Replace `AgentGroupChat` with `GroupChatOrchestration` and a `GroupChatManager` (e.g., `RoundRobinGroupChatManager` or a custom one) to control the flow.

**Before:**

```python
group_chat = AgentGroupChat(
    agents=[agent_writer, agent_reviewer],
    termination_strategy=CustomTerminationStrategy(),
    selection_strategy=CustomSelectionStrategy(),
)
```

**After:**

```python
from semantic_kernel.agents import GroupChatOrchestration, RoundRobinGroupChatManager

orchestration = GroupChatOrchestration(
    members=[agent_writer, agent_reviewer],
    manager=RoundRobinGroupChatManager(),
)
```

### Step 3: Start the Group Chat

**Before:**

```python
await group_chat.add_chat_message(message=TASK)
async for content in group_chat.invoke():
    # handle response
```

**After:**

```python
from semantic_kernel.agents.runtime import InProcessRuntime

runtime = InProcessRuntime()
runtime.start()

orchestration_result = await group_chat_orchestration.invoke(task=TASK, runtime=runtime)
value = await orchestration_result.get()
```

### Step 4: Customizing Orchestration

The new orchestration model allows you to create custom strategies for termination, agent selection, and more by subclassing `GroupChatManager` and overriding its methods. Please refer to the [GroupChatOrchestration documentation](../../Frameworks/agent/agent-orchestration/group-chat.md#customize-the-group-chat-manager) for more details.

### Step 5: Remove Deprecated APIs

Remove any code that directly manipulates `AgentGroupChat`-specific properties or methods, as they are no longer maintained.

### Step 6: Review and Test

- Review your code for any remaining references to the old classes.
- Test your group chat scenarios to ensure the new orchestration behaves as expected.

## Full Example

This guide demonstrates how to migrate the core logic of [`step06_chat_completion_agent_group_chat.py`](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/getting_started_with_agents/chat_completion/step06_chat_completion_agent_group_chat.py) from `AgentGroupChat` to the new `GroupChatOrchestration`, including a custom group chat manager that implements the approval-based termination strategy.

### Step 1: Agent Definition

There are no changes needed in the agent definition. You can continue using the same `AgentWriter` and `AgentReviewer` as before.

### Step 2: Implement a Custom Group Chat Manager

Create a custom `GroupChatManager` that terminates the chat when the last message contains "approved" and only the reviewer can approve:

```python
from semantic_kernel.agents import RoundRobinGroupChatManager, BooleanResult

class ApprovalGroupChatManager(RoundRobinGroupChatManager):
    def __init__(self, approver_name: str, max_rounds: int = 10):
        super().__init__(max_rounds=max_rounds)
        self._approver_name = approver_name

    async def should_terminate(self, chat_history):
        last = chat_history[-1] if chat_history else None
        should_terminate = (
            last is not None and
            getattr(last, 'name', None) == self._approver_name and
            'approved' in (last.content or '').lower()
        )
        return BooleanResult(result=should_terminate, reason="Approved by reviewer." if should_terminate else "Not yet approved.")
```

### Step 3: Initialize the Orchestration

Replace the `AgentGroupChat` initialization with:

```python
from semantic_kernel.agents import GroupChatOrchestration
from semantic_kernel.agents.runtime import InProcessRuntime

orchestration = GroupChatOrchestration(
    members=[agent_writer, agent_reviewer],
    manager=ApprovalGroupChatManager(approver_name=REVIEWER_NAME, max_rounds=10),
)
```

### Step 4: Run the Orchestration

Replace the message loop with:

```python
runtime = InProcessRuntime()
runtime.start()

orchestration_result = await orchestration.invoke(
    task="a slogan for a new line of electric cars.",
    runtime=runtime,
)

value = await orchestration_result.get()
print(f"***** Result *****\n{value}")

await runtime.stop_when_idle()
```

## Summary

- Use a custom `GroupChatManager` for approval-based termination.
- Replace the chat loop with orchestration invocation.
- The rest of your agent setup and message formatting can remain unchanged.

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Agent orchestration is not yet available in Java SDK.

::: zone-end
