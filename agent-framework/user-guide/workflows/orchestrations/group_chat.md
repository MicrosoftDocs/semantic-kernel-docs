---
title: Microsoft Agent Framework Workflows Orchestrations - Group Chat
description: In-depth look at Group Chat Orchestrations in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 11/12/2025
ms.service: agent-framework
---

# Microsoft Agent Framework Workflows Orchestrations - Group Chat

Group chat orchestration models a collaborative conversation among multiple agents, coordinated by a manager that determines speaker selection and conversation flow. This pattern is ideal for scenarios requiring iterative refinement, collaborative problem-solving, or multi-perspective analysis.

## Differences Between Group Chat and Other Patterns

Group chat orchestration has distinct characteristics compared to other multi-agent patterns:

- **Centralized Coordination**: Unlike handoff patterns where agents directly transfer control, group chat uses a manager to coordinate who speaks next
- **Iterative Refinement**: Agents can review and build upon each other's responses in multiple rounds
- **Flexible Speaker Selection**: The manager can use various strategies (round-robin, prompt-based, custom logic) to select speakers
- **Shared Context**: All agents see the full conversation history, enabling collaborative refinement

## What You'll Learn

- How to create specialized agents for group collaboration
- How to configure speaker selection strategies
- How to build workflows with iterative agent refinement
- How to customize conversation flow with custom managers

::: zone pivot="programming-language-csharp"

## Set Up the Azure OpenAI Client

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;

// Set up the Azure OpenAI client
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ??
    throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";
var client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();
```

## Define Your Agents

Create specialized agents for different roles in the group conversation:

```csharp
// Create a copywriter agent
ChatClientAgent writer = new(client,
    "You are a creative copywriter. Generate catchy slogans and marketing copy. Be concise and impactful.",
    "CopyWriter",
    "A creative copywriter agent");

// Create a reviewer agent
ChatClientAgent reviewer = new(client,
    "You are a marketing reviewer. Evaluate slogans for clarity, impact, and brand alignment. " +
    "Provide constructive feedback or approval.",
    "Reviewer",
    "A marketing review agent");
```

## Configure Group Chat with Round-Robin Manager

Build the group chat workflow using `AgentWorkflowBuilder`:

```csharp
// Build group chat with round-robin speaker selection
// The manager factory receives the list of agents and returns a configured manager
var workflow = AgentWorkflowBuilder
    .CreateGroupChatBuilderWith(agents => 
        new RoundRobinGroupChatManager(agents) 
        { 
            MaximumIterationCount = 5  // Maximum number of turns
        })
    .AddParticipants(writer, reviewer)
    .Build();
```

## Run the Group Chat Workflow

Execute the workflow and observe the iterative conversation:

```csharp
// Start the group chat
var messages = new List<ChatMessage> { 
    new(ChatRole.User, "Create a slogan for an eco-friendly electric vehicle.") 
};

StreamingRun run = await InProcessExecution.StreamAsync(workflow, messages);
await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
{
    if (evt is AgentRunUpdateEvent update)
    {
        // Process streaming agent responses
        AgentRunResponse response = update.AsResponse();
        foreach (ChatMessage message in response.Messages)
        {
            Console.WriteLine($"[{update.ExecutorId}]: {message.Text}");
        }
    }
    else if (evt is WorkflowOutputEvent output)
    {
        // Workflow completed
        var conversationHistory = output.As<List<ChatMessage>>();
        Console.WriteLine("\n=== Final Conversation ===");
        foreach (var message in conversationHistory)
        {
            Console.WriteLine($"{message.AuthorName}: {message.Text}");
        }
        break;
    }
}
```

## Sample Interaction

```plaintext
[CopyWriter]: "Green Dreams, Zero Emissions" - Drive the future with style and sustainability.

[Reviewer]: The slogan is good, but "Green Dreams" might be a bit abstract. Consider something 
more direct like "Pure Power, Zero Impact" to emphasize both performance and environmental benefit.

[CopyWriter]: "Pure Power, Zero Impact" - Experience electric excellence without compromise.

[Reviewer]: Excellent! This slogan is clear, impactful, and directly communicates the key benefits. 
The tagline reinforces the message perfectly. Approved for use.

[CopyWriter]: Thank you! The final slogan is: "Pure Power, Zero Impact" - Experience electric 
excellence without compromise.
```

::: zone-end

::: zone pivot="programming-language-python"

## Set Up the Chat Client

```python
from agent_framework.openai import OpenAIChatClient

# Initialize the OpenAI chat client
chat_client = OpenAIChatClient(model_id="gpt-4o-mini")
```

## Define Your Agents

Create specialized agents with distinct roles:

```python
from agent_framework import ChatAgent

# Create a researcher agent
researcher = ChatAgent(
    name="Researcher",
    description="Collects relevant background information.",
    instructions="Gather concise facts that help answer the question. Be brief and factual.",
    chat_client=chat_client,
)

# Create a writer agent
writer = ChatAgent(
    name="Writer",
    description="Synthesizes polished answers using gathered information.",
    instructions="Compose clear, structured answers using any notes provided. Be comprehensive.",
    chat_client=chat_client,
)
```

## Configure Group Chat with Simple Selector

Build a group chat with custom speaker selection logic:

```python
from agent_framework import GroupChatBuilder, GroupChatStateSnapshot

def select_next_speaker(state: GroupChatStateSnapshot) -> str | None:
    """Alternate between researcher and writer for collaborative refinement.
    
    Args:
        state: Contains task, participants, conversation, history, and round_index
        
    Returns:
        Name of next speaker, or None to finish
    """
    round_idx = state["round_index"]
    history = state["history"]
    
    # Finish after 4 turns (researcher → writer → researcher → writer)
    if round_idx >= 4:
        return None
    
    # Alternate speakers
    last_speaker = history[-1].speaker if history else None
    if last_speaker == "Researcher":
        return "Writer"
    return "Researcher"

# Build the group chat workflow
workflow = (
    GroupChatBuilder()
    .select_speakers(select_next_speaker, display_name="Orchestrator")
    .participants([researcher, writer])
    .build()
)
```

## Configure Group Chat with Prompt-Based Manager

Alternatively, use an AI-powered manager for dynamic speaker selection:

```python
# Build group chat with prompt-based manager
workflow = (
    GroupChatBuilder()
    .set_prompt_based_manager(
        chat_client=chat_client,
        display_name="Coordinator"
    )
    .participants(researcher=researcher, writer=writer)
    .build()
)
```

## Run the Group Chat Workflow

Execute the workflow and process events:

```python
from agent_framework import AgentRunUpdateEvent, WorkflowOutputEvent

task = "What are the key benefits of async/await in Python?"

print(f"Task: {task}\n")
print("=" * 80)

# Run the workflow
async for event in workflow.run_stream(task):
    if isinstance(event, AgentRunUpdateEvent):
        # Print streaming agent updates
        print(f"[{event.executor_id}]: {event.data}", end="", flush=True)
    elif isinstance(event, WorkflowOutputEvent):
        # Workflow completed
        final_message = event.data
        author = getattr(final_message, "author_name", "System")
        text = getattr(final_message, "text", str(final_message))
        print(f"\n\n[{author}]\n{text}")
        print("-" * 80)

print("\nWorkflow completed.")
```

## Sample Interaction

```plaintext
Task: What are the key benefits of async/await in Python?

================================================================================

[Researcher]: Async/await in Python provides non-blocking I/O operations, enabling 
concurrent execution without threading overhead. Key benefits include improved 
performance for I/O-bound tasks, better resource utilization, and simplified 
concurrent code structure using native coroutines.

[Writer]: The key benefits of async/await in Python are:

1. **Non-blocking Operations**: Allows I/O operations to run concurrently without 
   blocking the main thread, significantly improving performance for network 
   requests, file I/O, and database queries.

2. **Resource Efficiency**: Avoids the overhead of thread creation and context 
   switching, making it more memory-efficient than traditional threading.

3. **Simplified Concurrency**: Provides a clean, synchronous-looking syntax for 
   asynchronous code, making concurrent programs easier to write and maintain.

4. **Scalability**: Enables handling thousands of concurrent connections with 
   minimal resource consumption, ideal for high-performance web servers and APIs.

--------------------------------------------------------------------------------

Workflow completed.
```

::: zone-end

## Key Concepts

::: zone pivot="programming-language-csharp"

- **Centralized Manager**: Group chat uses a manager to coordinate speaker selection and flow
- **AgentWorkflowBuilder.CreateGroupChatBuilderWith()**: Creates workflows with a manager factory function
- **RoundRobinGroupChatManager**: Built-in manager that alternates speakers in round-robin fashion
- **MaximumIterationCount**: Controls the maximum number of agent turns before termination
- **Custom Managers**: Extend `RoundRobinGroupChatManager` or implement custom logic
- **Iterative Refinement**: Agents review and improve each other's contributions
- **Shared Context**: All participants see the full conversation history

::: zone-end

::: zone pivot="programming-language-python"

- **Flexible Manager Strategies**: Choose between simple selectors, prompt-based managers, or custom logic
- **GroupChatBuilder**: Creates workflows with configurable speaker selection
- **select_speakers()**: Define custom Python functions for speaker selection
- **set_prompt_based_manager()**: Use AI-powered coordination for dynamic speaker selection
- **GroupChatStateSnapshot**: Provides conversation state for selection decisions
- **Iterative Collaboration**: Agents build upon each other's contributions
- **Event Streaming**: Process agent updates and outputs in real-time

::: zone-end

## Advanced: Custom Speaker Selection

::: zone pivot="programming-language-csharp"

You can implement custom manager logic by creating a custom group chat manager:

```csharp
public class ApprovalBasedManager : RoundRobinGroupChatManager
{
    private readonly string _approverName;
    
    public ApprovalBasedManager(IReadOnlyList<AIAgent> agents, string approverName) 
        : base(agents)
    {
        _approverName = approverName;
    }
    
    // Override to add custom termination logic
    protected override ValueTask<bool> ShouldTerminateAsync(
        IReadOnlyList<ChatMessage> history, 
        CancellationToken cancellationToken = default)
    {
        var last = history.LastOrDefault();
        bool shouldTerminate = last?.AuthorName == _approverName &&
            last.Text?.Contains("approve", StringComparison.OrdinalIgnoreCase) == true;
            
        return ValueTask.FromResult(shouldTerminate);
    }
}

// Use custom manager in workflow
var workflow = AgentWorkflowBuilder
    .CreateGroupChatBuilderWith(agents => 
        new ApprovalBasedManager(agents, "Reviewer") 
        { 
            MaximumIterationCount = 10 
        })
    .AddParticipants(writer, reviewer)
    .Build();
```

::: zone-end

::: zone pivot="programming-language-python"

You can implement sophisticated selection logic based on conversation state:

```python
def smart_selector(state: GroupChatStateSnapshot) -> str | None:
    """Select speakers based on conversation content and context."""
    round_idx = state["round_index"]
    conversation = state["conversation"]
    history = state["history"]
    
    # Maximum 10 rounds
    if round_idx >= 10:
        return None
    
    # First round: always start with researcher
    if round_idx == 0:
        return "Researcher"
    
    # Check last message content
    last_message = conversation[-1] if conversation else None
    last_text = getattr(last_message, "text", "").lower()
    
    # If researcher asked a question, let writer respond
    if "?" in last_text and history[-1].speaker == "Researcher":
        return "Writer"
    
    # If writer provided info, let researcher validate or extend
    if history[-1].speaker == "Writer":
        return "Researcher"
    
    # Default alternation
    return "Writer" if history[-1].speaker == "Researcher" else "Researcher"

workflow = (
    GroupChatBuilder()
    .select_speakers(smart_selector, display_name="SmartOrchestrator")
    .participants([researcher, writer])
    .build()
)
```

::: zone-end

## When to Use Group Chat

Group chat orchestration is ideal for:

- **Iterative Refinement**: Multiple rounds of review and improvement
- **Collaborative Problem-Solving**: Agents with complementary expertise working together
- **Content Creation**: Writer-reviewer workflows for document creation
- **Multi-Perspective Analysis**: Getting diverse viewpoints on the same input
- **Quality Assurance**: Automated review and approval processes

**Consider alternatives when:**
- You need strict sequential processing (use Sequential orchestration)
- Agents should work completely independently (use Concurrent orchestration)
- Direct agent-to-agent handoffs are needed (use Handoff orchestration)
- Complex dynamic planning is required (use Magentic orchestration)

## Next steps

> [!div class="nextstepaction"]
> [Handoff Orchestration](./handoff.md)
