---
title: Microsoft Agent Framework Workflows Orchestrations - Handoff
description: In-depth look at Handoff Orchestrations in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/12/2025
ms.service: agent-framework
---

# Microsoft Agent Framework Workflows Orchestrations - Handoff

Handoff orchestration allows agents to transfer control to one another based on the context or user request. Each agent can "handoff" the conversation to another agent with the appropriate expertise, ensuring that the right agent handles each part of the task. This is particularly useful in customer support, expert systems, or any scenario requiring dynamic delegation.

![Handoff Orchestration](../resources/images/orchestration-handoff.png)

## Differences Between Handoff and Agent-as-Tools

While agent-as-tools is commonly considered as a multi-agent pattern and it may look similar to handoff at first glance, there are fundamental differences between the two:

- **Control Flow**: In handoff orchestration, control is explicitly passed between agents based on defined rules. Each agent can decide to hand off the entire task to another agent. There is no central authority managing the workflow. In contrast, agent-as-tools involves a primary agent that delegates sub tasks to other agents and once the agent completes the sub task, control returns to the primary agent.
- **Task Ownership**: In handoff, the agent receiving the handoff takes full ownership of the task. In agent-as-tools, the primary agent retains overall responsibility for the task, while other agents are treated as tools to assist in specific subtasks.
- **Context Management**: In handoff orchestration, the conversation is handed off to another agent entirely. The receiving agent has full context of what has been done so far. In agent-as-tools, the primary agent manages the overall context and may provide only relevant information to the tool agents as needed.

## What You'll Learn

- How to create specialized agents for different domains
- How to configure handoff rules between agents
- How to build interactive workflows with dynamic agent routing
- How to handle multi-turn conversations with agent switching

## Define Your Specialized Agents

::: zone pivot="programming-language-csharp"

In handoff orchestration, agents can transfer control to one another based on context. Here's how to create specialized agents and configure handoff rules:

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Agents;

// Set up the Azure OpenAI client
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ??
    throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";
var client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();

// Create specialized agents
ChatClientAgent historyTutor = new(client,
    "You provide assistance with historical queries. Explain important events and context clearly. Only respond about history.",
    "history_tutor",
    "Specialist agent for historical questions");

ChatClientAgent mathTutor = new(client,
    "You provide help with math problems. Explain your reasoning at each step and include examples. Only respond about math.",
    "math_tutor",
    "Specialist agent for math questions");

ChatClientAgent triageAgent = new(client,
    "You determine which agent to use based on the user's homework question. ALWAYS handoff to another agent.",
    "triage_agent",
    "Routes messages to the appropriate specialist agent");
```

## Configure Handoff Rules

Use `AgentWorkflowBuilder` to define which agents can hand off to which other agents:

```csharp
// Build handoff workflow with routing rules
var workflow = AgentWorkflowBuilder.StartHandoffWith(triageAgent)
    .WithHandoff(triageAgent, [mathTutor, historyTutor])  // Triage can route to either specialist
    .WithHandoff(mathTutor, triageAgent)                 // Math tutor can return to triage
    .WithHandoff(historyTutor, triageAgent)              // History tutor can return to triage
    .Build();
```

## Run Interactive Handoff Workflow

Handle multi-turn conversations where agents can dynamically hand off control:

```csharp
List<ChatMessage> messages = new();

while (true)
{
    Console.Write("Q: ");
    string userInput = Console.ReadLine()!;
    messages.Add(new(ChatRole.User, userInput));

    // Process the conversation through the handoff workflow
    var result = await RunWorkflowAsync(workflow, messages);
    messages.AddRange(result);
}

// Helper method to run workflow and handle events
static async Task<List<ChatMessage>> RunWorkflowAsync(
    Workflow<List<ChatMessage>> workflow,
    List<ChatMessage> messages)
{
    StreamingRun run = await InProcessExecution.StreamAsync(workflow, messages);
    await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

    await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
    {
        if (evt is AgentRunUpdateEvent e)
        {
            Console.WriteLine($"{e.ExecutorId}: {e.Data}");
        }
        else if (evt is WorkflowCompletedEvent completed)
        {
            return (List<ChatMessage>)completed.Data!;
        }
    }

    return new List<ChatMessage>();
}
```

## Key Features

- **AgentWorkflowBuilder.StartHandoffWith()**: Defines the initial agent for the workflow
- **WithHandoff()**: Configures handoff rules between agents
- **Dynamic Routing**: Agents can decide which agent should handle the next interaction
- **Context Preservation**: Full conversation history is maintained across handoffs
- **Multi-turn Support**: Supports ongoing conversations with agent switching

## Sample Interaction

```plaintext
Q: What is the derivative of x^2?
triage_agent: This is a math question. I'll hand this off to the math tutor.
math_tutor: The derivative of x^2 is 2x. Using the power rule, we bring down the exponent...

Q: Tell me about World War 2
triage_agent: This is a history question. I'll hand this off to the history tutor.
history_tutor: World War 2 was a global conflict from 1939 to 1945...

Q: Can you help me with calculus integration?
triage_agent: This is another math question. I'll route this to the math tutor.
math_tutor: I'd be happy to help with calculus integration...
```

::: zone-end

::: zone pivot="programming-language-python"

Coming soon...

::: zone-end
