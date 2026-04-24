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
Func<string, string> uppercaseFunc = s => s.ToUpperInvariant();
var uppercase = uppercaseFunc.BindAsExecutor("UppercaseExecutor");

// Step 2: Reverse the string and yield output
class ReverseTextExecutor() : Executor<string, string>("ReverseTextExecutor")
{
    public override ValueTask<string> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(string.Concat(message.Reverse()));
    }
}
ReverseTextExecutor reverse = new();
```

Build and run the workflow:

```csharp
WorkflowBuilder builder = new(uppercase);
builder.AddEdge(uppercase, reverse).WithOutputFrom(reverse);
var workflow = builder.Build();

await using Run run = await InProcessExecution.RunAsync(workflow, "Hello, World!");
foreach (WorkflowEvent evt in run.NewEvents)
{
    if (evt is ExecutorCompletedEvent executorComplete)
    {
        Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
    }
}
```

> [!TIP]
> See [here](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/01-get-started/05_first_workflow) for a full runnable sample application.

:::zone-end

:::zone pivot="programming-language-python"

Define workflow steps (executors) and connect them with edges:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/05_functional_workflow_with_agents.py" id="create_workflow" highlight="26":::

Build and run the workflow:

:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/05_first_workflow.py" id="run_workflow" highlight="3":::

> [!TIP]
> See the [full sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/01-get-started/05_functional_workflow_with_agents.py) for the complete runnable file.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Step 6: Host Your Agent](./hosting.md)

**Go deeper:**

- [Workflows overview](../workflows/index.md) — understand workflow architecture
- [Sequential workflows](../workflows/orchestrations/sequential.md) — linear step-by-step patterns
- [Agents in workflows](../workflows/agents-in-workflows.md) — using agents as workflow steps
