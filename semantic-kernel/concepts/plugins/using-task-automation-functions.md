---
title: Allow agents to automate tasks
description: Learn how you safely allow agents to automate tasks on behalf of users in Semantic Kernel.
author: sophialagerkranspandey
ms.topic: conceptual
ms.author: sopand
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Task automation with agents

Most AI agents today simply retrieve data and respond to user queries. AI agents, however, can achieve much more by using plugins to automate tasks on behalf of users. This allows users to delegate tasks to AI agents, freeing up time for more important work.

Once AI Agents start performing actions, however, it's important to ensure that they are acting in the best interest of the user. This is why we provide hooks / filters to allow you to control what actions the AI agent can take.

## Requiring user consent

When an AI agent is about to perform an action on behalf of a user, it should first ask for the user's consent. This is especially important when the action involves sensitive data or financial transactions.

In Semantic Kernel, you can use the function invocation filter. This filter is always called whenever a function is invoked from an AI agent. To create a filter, you need to implement the `IFunctionInvocationFilter` interface and then add it as a service to the kernel.

Here's an example of a function invocation filter that requires user consent:

```csharp
public class ApprovalFilterExample() : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        if (context.Function.PluginName == "DynamicsPlugin" && context.Function.Name == "create_order")
        {
            Console.WriteLine("System > The agent wants to create an approval, do you want to proceed? (Y/N)");
            string shouldProceed = Console.ReadLine()!;

            if (shouldProceed != "Y")
            {
                context.Result = new FunctionResult(context.Result, "The order creation was not approved by the user");
                return;
            }
            
            await next(context);
        }
    }
}
```

You can then add the filter as a service to the kernel:

```csharp
IKernelBuilder builder = Kernel.CreateBuilder();
builder.Services.AddSingleton<IFunctionInvocationFilter, ApprovalFilterExample>();
Kernel kernel = builder.Build();
```

Now, whenever the AI agent tries to create an order using the `DynamicsPlugin`, the user will be prompted to approve the action.

> [!TIP]
> Whenever a function is cancelled or fails, you should provide the AI agent with a meaningful error message so it can respond appropriately. For example, if we didn't let the AI agent know that the order creation was not approved, it would assume that the order failed due to a technical issue and would try to create the order again.

## Next steps

Now that you've learned how to allow agents to automate tasks, you can learn how to allow agents to automatically create plans to address user needs.

> [!div class="nextstepaction"]
> [Automate planning with agents](../planning.md)
