---
title: Semantic Kernel Agent Orchestration
description: An overview on orchestrating agents in Semantic Kernel
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 05/19/2025
ms.service: semantic-kernel
---

# Semantic Kernel Agent Orchestration

> [!IMPORTANT]
> Agent Orchestration features in the Agent Framework are in the experimental stage. They are under active development and may change significantly before advancing to the preview or release candidate stage.

Semantic Kernel’s Agent Orchestration framework enables developers to build, manage, and scale complex agent workflows with ease.

## Why Multi-agent Orchestration?

Traditional single-agent systems are limited in their ability to handle complex, multi-faceted tasks. By orchestrating multiple agents, each with specialized skills or roles, we can create systems that are more robust, adaptive, and capable of solving real-world problems collaboratively. Multi-agent orchestration in Semantic Kernel provides a flexible foundation for building such systems, supporting a variety of coordination patterns.

## Orchestration Patterns

Semantic Kernel supports several orchestration patterns, each designed for different collaboration scenarios. These patterns are available as part of the framework and can be easily extended or customized.

## Supported Orchestration Patterns

| Pattern                       | Description                                                                                                                                                                         | Typical Use Case                                                      |
| ----------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------- |
| [Concurrent](./concurrent.md) | Broadcasts a task to all agents, collects results independently.                                                                                                                    | Parallel analysis, independent subtasks, ensemble decision making.    |
| [Sequential](./sequential.md) | Passes the result from one agent to the next in a defined order.                                                                                                                    | Step-by-step workflows, pipelines, multi-stage processing.            |
| [Handoff](./handoff.md)       | Dynamically passes control between agents based on context or rules.                                                                                                                | Dynamic workflows, escalation, fallback, or expert handoff scenarios. |
| [Group Chat](./group-chat.md) | All agents participate in a group conversation, coordinated by a group manager.                                                                                                     | Brainstorming, collaborative problem solving, consensus building.     |
| [Magentic](./magentic.md)     | Group chat-like orchestration inspired by [MagenticOne](https://www.microsoft.com/en-us/research/articles/magentic-one-a-generalist-multi-agent-system-for-solving-complex-tasks/). | Complex, generalist multi-agent collaboration.                        |

## Simplicity and Developer-friendly

All orchestration patterns share a unified interface for construction and invocation. No matter which orchestration you choose, you:

- Define your agents and their capabilities, see Semantic Kernel Agents.
- Create an orchestration by passing the agents (and, if needed, a manager).
- Optionally provide callbacks or transforms for custom input/output handling.
- Start a runtime and invoke the orchestration with a task.
- Await the result in a consistent, asynchronous manner.

This unified approach means you can easily switch between orchestration patterns, without learning new APIs or rewriting your agent logic. The framework abstracts away the complexity of agent communication, coordination, and result aggregation, letting you focus on your application’s goals.

::: zone pivot="programming-language-csharp"

```csharp
// Choose an orchestration pattern with your agents
SequentialOrchestration orchestration = new(agentA, agentB)
{
    LoggerFactory = this.LoggerFactory
};  // or ConcurrentOrchestration, GroupChatOrchestration, HandoffOrchestration, MagenticOrchestration, ...

// Start the runtime
InProcessRuntime runtime = new();
await runtime.StartAsync();

// Invoke the orchestration and get the result
OrchestrationResult<string> result = await orchestration.InvokeAsync(task, runtime);
string text = await result.GetValueAsync();

await runtime.RunUntilIdleAsync();
```

::: zone-end

::: zone pivot="programming-language-python"

```python
# Choose an orchestration pattern with your agents
orchestration = SequentialOrchestration(members=[agent_a, agent_b])
# or ConcurrentOrchestration, GroupChatOrchestration, HandoffOrchestration, MagenticOrchestration, ...

# Start the runtime
runtime = InProcessRuntime()
runtime.start()

# Invoke the orchestration
result = await orchestration.invoke(task="Your task here", runtime=runtime)

# Get the result
final_output = await result.get()

await runtime.stop_when_idle()
```

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Agent orchestration is not yet available in Java SDK.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Concurrent Orchestration](./concurrent.md)
