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

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/05_first_workflow.py" id="create_workflow" highlight="24-26":::

Build and run the workflow:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/05_first_workflow.py" id="run_workflow" highlight="3-5":::

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
