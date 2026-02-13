---
title: "Step 5: Workflows"
description: "Chain multiple steps together in a sequential workflow."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: tutorial
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Step 5: Workflows

Workflows let you chain multiple steps together — each step processes data and passes it to the next.

:::zone pivot="programming-language-csharp"

Define workflow steps (executors):

```csharp
using Microsoft.Agents.AI.Workflows;

// Step 1: Convert text to uppercase
class UpperCase : Executor
{
    [Handler]
    public async Task ToUpperCase(string text, WorkflowContext<string> ctx)
    {
        await ctx.SendMessageAsync(text.ToUpper());
    }
}

// Step 2: Reverse the string and yield output
[Executor(Id = "reverse_text")]
static async Task ReverseText(string text, WorkflowContext<Never, string> ctx)
{
    var reversed = new string(text.Reverse().ToArray());
    await ctx.YieldOutputAsync(reversed);
}
```

Build and run the workflow:

```csharp
var upper = new UpperCase();
var workflow = new AgentWorkflowBuilder(startExecutor: upper)
    .AddEdge(upper, ReverseText)
    .Build();

var result = await workflow.RunAsync("hello world");
Console.WriteLine($"Output: {string.Join(", ", result.GetOutputs())}");
// Output: DLROW OLLEH
```

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/01-get-started/05_FirstWorkflow.cs) for the complete runnable file.

:::zone-end

:::zone pivot="programming-language-python"

Define workflow steps (executors) and connect them with edges:

```python
from agent_framework import (
    Executor, WorkflowBuilder, WorkflowContext, executor, handler,
)
from typing_extensions import Never

# Step 1: A class-based executor that converts text to uppercase
class UpperCase(Executor):
    def __init__(self, id: str):
        super().__init__(id=id)

    @handler
    async def to_upper_case(self, text: str, ctx: WorkflowContext[str]) -> None:
        await ctx.send_message(text.upper())

# Step 2: A function-based executor that reverses the string
@executor(id="reverse_text")
async def reverse_text(text: str, ctx: WorkflowContext[Never, str]) -> None:
    await ctx.yield_output(text[::-1])
```

Build and run the workflow:

```python
upper = UpperCase(id="upper_case")
workflow = (
    WorkflowBuilder(start_executor=upper)
    .add_edge(upper, reverse_text)
    .build()
)

events = await workflow.run("hello world")
print(f"Output: {events.get_outputs()}")
# Output: ['DLROW OLLEH']
```

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/01-get-started/05_first_workflow.py) for the complete runnable file.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Step 6: Host Your Agent](./hosting.md)

**Go deeper:**

- [Workflows overview](../workflows/index.md) — understand workflow architecture
- [Sequential workflows](../workflows/orchestrations/sequential.md) — linear step-by-step patterns
- [Agents in workflows](../workflows/agents-in-workflows.md) — using agents as workflow steps
