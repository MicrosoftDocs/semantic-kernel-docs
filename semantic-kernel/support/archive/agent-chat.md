---
title: Exploring Agent Collaboration in Agent Chat (Experimental)
description: An exploration of defining and managing agent collaboration via Agent Chat.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# Exploring Agent Collaboration in `AgentChat`

> [!IMPORTANT]
> This feature is in the experimental stage. Features at this stage are under development and subject to change before advancing to the preview or release candidate stage.

Detailed API documentation related to this discussion is available at:

::: zone pivot="programming-language-csharp"
- [`AgentChat`](/dotnet/api/microsoft.semantickernel.agents.agentchat)
- [`AgentGroupChat`](/dotnet/api/microsoft.semantickernel.agents.agentgroupchat)
- [`Microsoft.SemanticKernel.Agents.Chat`](/dotnet/api/microsoft.semantickernel.agents.chat)

::: zone-end

::: zone pivot="programming-language-python"

- [`AgentChat`](/python/api/semantic-kernel/semantic_kernel.agents.group_chat.agent_chat.agentchat)
- [`AgentGroupChat`](/python/api/semantic-kernel/semantic_kernel.agents.group_chat.agent_group_chat.agentgroupchat)

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end


## What is `AgentChat`?

`AgentChat` provides a framework that enables interaction between multiple agents, even if they are of different types. This makes it possible for a [`ChatCompletionAgent`](./../../Frameworks/agent/agent-types/chat-completion-agent.md) and an [`OpenAIAssistantAgent`](./../../Frameworks/agent/agent-types/assistant-agent.md) to work together within the same conversation. `AgentChat` also defines entry points for initiating collaboration between agents, whether through multiple responses or a single agent response.

As an abstract class, `AgentChat` can be subclassed to support custom scenarios.

One such subclass, `AgentGroupChat`, offers a concrete implementation of `AgentChat`, using a strategy-based approach to manage conversation dynamics.


## Creating an `AgentGroupChat`

To create an `AgentGroupChat`, you may either specify the participating agents or create an empty chat and subsequently add agent participants.  Configuring the Chat-Settings and Strategies is also performed during `AgentGroupChat` initialization. These settings define how the conversation dynamics will function within the group.

> Note: The default Chat-Settings result in a conversation that is limited to a single response.  See [`AgentChat` Behavior](#defining-agentgroupchat-behavior) for details on configuring Chat-Settings.

#### Creating an `AgentGroupChat` with an `Agent`:

::: zone pivot="programming-language-csharp"
```csharp
// Define agents
ChatCompletionAgent agent1 = ...;
OpenAIAssistantAgent agent2 = ...;

// Create chat with participating agents.
AgentGroupChat chat = new(agent1, agent2);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Define agents
agent1 = ChatCompletionAgent(...)
agent2 = OpenAIAssistantAgent(...)

# Create chat with participating agents
chat = AgentGroupChat(agents=[agent1, agent2])
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

#### Adding an `Agent` to an `AgentGroupChat`:

::: zone pivot="programming-language-csharp"
```csharp
// Define agents
ChatCompletionAgent agent1 = ...;
OpenAIAssistantAgent agent2 = ...;

// Create an empty chat.
AgentGroupChat chat = new();

// Add agents to an existing chat.
chat.AddAgent(agent1);
chat.AddAgent(agent2);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Define agents
agent1 = ChatCompletionAgent(...)
agent2 = OpenAIAssistantAgent(...)

# Create an empty chat
chat = AgentGroupChat()

# Add agents to an existing chat
chat.add_agent(agent=agent1)
chat.add_agent(agent=agent2)
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end


## Using `AgentGroupChat`

`AgentChat` supports two modes of operation: `Single-Turn` and `Multi-Turn`.  In `single-turn`, a specific agent is designated to provide a response. In `multi-turn`, all agents in the conversation take turns responding until a termination criterion is met. In both modes, agents can collaborate by responding to one another to achieve a defined goal.

### Providing Input

Adding an input message to an `AgentChat` follows the same pattern as whit a `ChatHistory` object.

::: zone pivot="programming-language-csharp"
```csharp
AgentGroupChat chat = new();

chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, "<message content>"));
```
::: zone-end

::: zone pivot="programming-language-python"
```python
chat = AgentGroupChat()

await chat.add_chat_message(message="<message content>")
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

### Single-Turn Agent Invocation

In a multi-turn invocation, the system must decide which agent responds next and when the conversation should end. In contrast, a single-turn invocation simply returns a response from the specified agent, allowing the caller to directly manage agent participation.

After an agent participates in the `AgentChat` through a single-turn invocation, it is added to the set of agents eligible for multi-turn invocation.

::: zone pivot="programming-language-csharp"
```csharp
// Define an agent
ChatCompletionAgent agent = ...;

// Create an empty chat.
AgentGroupChat chat = new();

// Invoke an agent for its response
ChatMessageContent[] messages = await chat.InvokeAsync(agent).ToArrayAsync();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Define an agent
agent = ChatCompletionAgent(...)

# Create an empty chat
chat = AgentGroupChat()

# Invoke an agent for its response(s)
async for message in chat.invoke(agent)
    # process message response(s)
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

### Multi-Turn Agent Invocation

While agent collaboration requires that a system must be in place that not only determines which agent should respond during each turn but also assesses when the conversation has achieved its intended goal, initiating multi-turn collaboration remains straightforward.

Agent responses are returned asynchronously as they are generated, allowing the conversation to unfold in real-time.

> Note: In following sections, [Agent Selection](#agent-selection) and [Chat Termination](#chat-termination), will delve into the Execution Settings in detail.  The default Execution Settings employs sequential or round-robin selection and limits agent participation to a single turn.

::: zone pivot="programming-language-csharp"

.NET Execution Settings API: [`AgentGroupChatSettings`](/dotnet/api/microsoft.semantickernel.agents.chat.agentgroupchatsettings)

```csharp
// Define agents
ChatCompletionAgent agent1 = ...;
OpenAIAssistantAgent agent2 = ...;

// Create chat with participating agents.
AgentGroupChat chat =
  new(agent1, agent2)
  {
    // Override default execution settings
    ExecutionSettings =
    {
        TerminationStrategy = { MaximumIterations = 10 }
    }
  };

// Invoke agents
await foreach (ChatMessageContent response in chat.InvokeAsync())
{
  // Process agent response(s)...
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Define agents
agent1 = ChatCompletionAgent(...)
agent2 = OpenAIAssistantAgent(...)

# Create chat with participating agents
chat = AgentGroupChat(
    agents=[agent1, agent2],
    termination_strategy=DefaultTerminationStrategy(maximum_iterations=10),
)

async for response in chat.invoke():
    # process agent response(s)
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

### Accessing Chat History

The `AgentChat` conversation history is always accessible, even though messages are delivered through the invocation pattern. This ensures that past exchanges remain available throughout the conversation.

> Note: The most recent message is provided first (descending order: newest to oldest).

::: zone pivot="programming-language-csharp"
```csharp
// Define and use a chat
AgentGroupChat chat = ...;

// Access history for a previously utilized AgentGroupChat
ChatMessageContent[] history = await chat.GetChatMessagesAsync().ToArrayAsync();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Define a group chat
chat = AgentGroupChat(...)

# Access history for a previously utilized AgentGroupChat
history = await chat.get_chat_messages()
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

Since different agent types or configurations may maintain their own version of the conversation history, agent specific history is also available by specifying an agent.  (For example: [`OpenAIAssistant`](./../../Frameworks/agent/agent-types/assistant-agent.md) versus [`ChatCompletionAgent`](./../../Frameworks/agent/agent-types/chat-completion-agent.md).)

::: zone pivot="programming-language-csharp"
```csharp
// Agents to participate in chat
ChatCompletionAgent agent1 = ...;
OpenAIAssistantAgent agent2 = ...;

// Define a group chat
AgentGroupChat chat = ...;

// Access history for a previously utilized AgentGroupChat
ChatMessageContent[] history1 = await chat.GetChatMessagesAsync(agent1).ToArrayAsync();
ChatMessageContent[] history2 = await chat.GetChatMessagesAsync(agent2).ToArrayAsync();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Agents to participate in a chat
agent1 = ChatCompletionAgent(...)
agent2 = OpenAIAssistantAgent(...)

# Define a group chat
chat = AgentGroupChat(...)

# Access history for a previously utilized AgentGroupChat
history1 = await chat.get_chat_messages(agent=agent1)
history2 = await chat.get_chat_messages(agent=agent2)
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end


## Defining `AgentGroupChat` Behavior

Collaboration among agents to solve complex tasks is a core agentic pattern. To use this pattern effectively, a system must be in place that not only determines which agent should respond during each turn but also assesses when the conversation has achieved its intended goal. This requires managing agent selection and establishing clear criteria for conversation termination, ensuring seamless cooperation between agents toward a solution. Both of these aspects are governed by the Execution Settings property.

The following sections, [Agent Selection](#agent-selection) and [Chat Termination](#chat-termination), will delve into these considerations in detail.

### Agent Selection

In multi-turn invocation, agent selection is guided by a Selection Strategy. This strategy is defined by a base class that can be extended to implement custom behaviors tailored to specific needs. For convenience, two predefined concrete Selection Strategies are also available, offering ready-to-use approaches for handling agent selection during conversations.

If known, an initial agent may be specified to always take the first turn.  A history reducer may also be employed to limit token usage when using a strategy based on a `KernelFunction`.

::: zone pivot="programming-language-csharp"

.NET Selection Strategy API:
- [`SelectionStrategy`](/dotnet/api/microsoft.semantickernel.agents.chat.selectionstrategy)
- [`SequentialSelectionStrategy`](/dotnet/api/microsoft.semantickernel.agents.chat.sequentialselectionstrategy)
- [`KernelFunctionSelectionStrategy`](/dotnet/api/microsoft.semantickernel.agents.chat.kernelfunctionselectionstrategy)

```csharp
// Define the agent names for use in the function template
const string WriterName = "Writer";
const string ReviewerName = "Reviewer";

// Initialize a Kernel with a chat-completion service
Kernel kernel = ...;

// Create the agents
ChatCompletionAgent writerAgent =
    new()
    {
        Name = WriterName,
        Instructions = "<writer instructions>",
        Kernel = kernel
    };

ChatCompletionAgent reviewerAgent =
    new()
    {
        Name = ReviewerName,
        Instructions = "<reviewer instructions>",
        Kernel = kernel
    };

// Define a kernel function for the selection strategy
KernelFunction selectionFunction =
    AgentGroupChat.CreatePromptFunctionForStrategy(
        $$$"""
        Determine which participant takes the next turn in a conversation based on the the most recent participant.
        State only the name of the participant to take the next turn.
        No participant should take more than one turn in a row.
        
        Choose only from these participants:
        - {{{ReviewerName}}}
        - {{{WriterName}}}
        
        Always follow these rules when selecting the next participant:
        - After {{{WriterName}}}, it is {{{ReviewerName}}}'s turn.
        - After {{{ReviewerName}}}, it is {{{WriterName}}}'s turn.

        History:
        {{$history}}
        """,
        safeParameterNames: "history");

// Define the selection strategy
KernelFunctionSelectionStrategy selectionStrategy = 
  new(selectionFunction, kernel)
  {
      // Always start with the writer agent.
      InitialAgent = writerAgent,
      // Parse the function response.
      ResultParser = (result) => result.GetValue<string>() ?? WriterName,
      // The prompt variable name for the history argument.
      HistoryVariableName = "history",
      // Save tokens by not including the entire history in the prompt
      HistoryReducer = new ChatHistoryTruncationReducer(3),
  };   

// Create a chat using the defined selection strategy.
AgentGroupChat chat =
    new(writerAgent, reviewerAgent)
    {
        ExecutionSettings = new() { SelectionStrategy = selectionStrategy }
    };
```
::: zone-end

::: zone pivot="programming-language-python"

Python Selection Strategy API:
- [`SelectionStrategy` Base Class](/python/api/semantic-kernel/semantic_kernel.agents.strategies.selection.selection_strategy.selectionstrategy)
- [`SequentialSelectionStrategy`](/python/api/semantic-kernel/semantic_kernel.agents.strategies.selection.sequential_selection_strategy.sequentialselectionstrategy)
- [`KernelFunctionSelectionStrategy`](/python/api/semantic-kernel/semantic_kernel.agents.strategies.kernelfunctionselectionstrategy)

```python
REVIEWER_NAME = "Reviewer"
WRITER_NAME = "Writer"

agent_reviewer = ChatCompletionAgent(
    kernel=kernel,
    name=REVIEWER_NAME,
    instructions="<instructions>",
)

agent_writer = ChatCompletionAgent(
    kernel=kernel,
    name=WRITER_NAME,
    instructions="<instructions>",
)

selection_function = KernelFunctionFromPrompt(
    function_name="selection",
    prompt=f"""
    Determine which participant takes the next turn in a conversation based on the the most recent participant.
    State only the name of the participant to take the next turn.
    No participant should take more than one turn in a row.
    
    Choose only from these participants:
    - {REVIEWER_NAME}
    - {WRITER_NAME}
    
    Always follow these rules when selecting the next participant:
    - After user input, it is {WRITER_NAME}'s turn.
    - After {WRITER_NAME} replies, it is {REVIEWER_NAME}'s turn.
    - After {REVIEWER_NAME} provides feedback, it is {WRITER_NAME}'s turn.

    History:
    {{{{$history}}}}
    """,
)

chat = AgentGroupChat(
    agents=[agent_writer, agent_reviewer],
    selection_strategy=KernelFunctionSelectionStrategy(
        function=selection_function,
        kernel=_create_kernel_with_chat_completion("selection"),
        result_parser=lambda result: str(result.value[0]) if result.value is not None else COPYWRITER_NAME,
        agent_variable_name="agents",
        history_variable_name="history",
    ),
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end


### Chat Termination

In multi-turn invocation, the Termination Strategy dictates when the final turn takes place. This strategy ensures the conversation ends at the appropriate point.

This strategy is defined by a base class that can be extended to implement custom behaviors tailored to specific needs. For convenience, several predefined concrete Selection Strategies are also available, offering ready-to-use approaches for defining termination criteria for an `AgentChat` conversations.

::: zone pivot="programming-language-csharp"

.NET Termination Strategy API:
- [`TerminationStrategy`](/dotnet/api/microsoft.semantickernel.agents.chat.terminationstrategy)
- [`RegexTerminationStrategy`](/dotnet/api/microsoft.semantickernel.agents.chat.regexterminationstrategy)
- [`KernelFunctionSelectionStrategy`](/dotnet/api/microsoft.semantickernel.agents.chat.kernelfunctionselectionstrategy)
- [`KernelFunctionTerminationStrategy`](/dotnet/api/microsoft.semantickernel.agents.chat.kernelfunctionterminationstrategy)
- [`AggregatorTerminationStrategy`](/dotnet/api/microsoft.semantickernel.agents.chat.aggregatorterminationstrategy)

```csharp
// Initialize a Kernel with a chat-completion service
Kernel kernel = ...;

// Create the agents
ChatCompletionAgent writerAgent =
    new()
    {
        Name = "Writer",
        Instructions = "<writer instructions>",
        Kernel = kernel
    };

ChatCompletionAgent reviewerAgent =
    new()
    {
        Name = "Reviewer",
        Instructions = "<reviewer instructions>",
        Kernel = kernel
    };

// Define a kernel function for the selection strategy
KernelFunction terminationFunction =
    AgentGroupChat.CreatePromptFunctionForStrategy(
        $$$"""
        Determine if the reviewer has approved.  If so, respond with a single word: yes

        History:
        {{$history}}
        """,
        safeParameterNames: "history");

// Define the termination strategy
KernelFunctionTerminationStrategy terminationStrategy = 
  new(terminationFunction, kernel)
  {
      // Only the reviewer may give approval.
      Agents = [reviewerAgent],
      // Parse the function response.
      ResultParser = (result) => 
        result.GetValue<string>()?.Contains("yes", StringComparison.OrdinalIgnoreCase) ?? false,
      // The prompt variable name for the history argument.
      HistoryVariableName = "history",
      // Save tokens by not including the entire history in the prompt
      HistoryReducer = new ChatHistoryTruncationReducer(1),
      // Limit total number of turns no matter what
      MaximumIterations = 10,
};

// Create a chat using the defined termination strategy.
AgentGroupChat chat =
    new(writerAgent, reviewerAgent)
    {
        ExecutionSettings = new() { TerminationStrategy = terminationStrategy }
    };

```
::: zone-end

::: zone pivot="programming-language-python"

Python Termination Strategy API:
- [`TerminationStrategy` Base Class](/python/api/semantic-kernel/semantic_kernel.agents.strategies.termination.termination_strategy.terminationstrategy)
- [`KernelFunctionTerminationStrategy`](/python/api/semantic-kernel/semantic_kernel.agents.strategies.termination.kernel_function_termination_strategy.kernelfunctionterminationstrategy)
- [`AggregatorTerminationStrategy`](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/agents/strategies/termination/aggregator_termination_strategy.py#L27)


```python
REVIEWER_NAME = "Reviewer"
WRITER_NAME = "Writer"

agent_reviewer = ChatCompletionAgent(
    kernel=kernel,
    name=REVIEWER_NAME,
    instructions="<instructions>",
)

agent_writer = ChatCompletionAgent(
    kernel=kernel,
    name=WRITER_NAME,
    instructions="<instructions>",
)

termination_function = KernelFunctionFromPrompt(
    function_name="termination",
    prompt="""
    Determine if the copy has been approved.  If so, respond with a single word: yes

    History:
    {{$history}}
    """,
)

chat = AgentGroupChat(
    agents=[agent_writer, agent_reviewer],
    termination_strategy=KernelFunctionTerminationStrategy(
        agents=[agent_reviewer],
        function=termination_function,
        kernel=_create_kernel_with_chat_completion("termination"),
        result_parser=lambda result: str(result.value[0]).lower() == "yes",
        history_variable_name="history",
        maximum_iterations=10,
    ),
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end


### Resetting Chat Completion State

Regardless of whether `AgentGroupChat` is invoked using the single-turn or multi-turn approach, the state of the `AgentGroupChat` is updated to indicate it is completed once the termination criteria is met. This ensures that the system recognizes when a conversation has fully concluded. To continue using an `AgentGroupChat` instance after it has reached the _Completed_ state, this state must be reset to allow further interactions. Without resetting, additional interactions or agent responses will not be possible.

In the case of a multi-turn invocation that reaches the maximum turn limit, the system will cease agent invocation but will not mark the instance as completed. This allows for the possibility of extending the conversation without needing to reset the Completion state.

::: zone pivot="programming-language-csharp"
```csharp
// Define an use chat
AgentGroupChat chat = ...;

// Evaluate if completion is met and reset.
if (chat.IsComplete) 
{
  // Opt to take action on the chat result...

  // Reset completion state to continue use
  chat.IsComplete = false;
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Define a group chat
chat = AgentGroupChat()

# Evaluate if completion is met and reset
if chat.is_complete:
    # Reset completion state to continue use
    chat.is_complete = False
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end


### Clear Full Conversation State

When done using an `AgentChat` where an [`OpenAIAssistant`](./../../Frameworks/agent/agent-types/assistant-agent.md) participated, it may be necessary to delete the remote thread associated with the assistant. `AgentChat` supports resetting or clearing the entire conversation state, which includes deleting any remote thread definition. This ensures that no residual conversation data remains linked to the assistant once the chat concludes.

A full reset does not remove the agents that had joined the `AgentChat` and leaves the `AgentChat` in a state where it can be reused. This allows for the continuation of interactions with the same agents without needing to reinitialize them, making future conversations more efficient.

::: zone pivot="programming-language-csharp"
```csharp
// Define an use chat
AgentGroupChat chat = ...;

// Clear the all conversation state
await chat.ResetAsync();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Define a group chat
chat = AgentGroupChat()

# Clear the conversation state
await chat.reset()
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end


## How-To

For an end-to-end example for using `AgentGroupChat` for `Agent` collaboration, see:

- [How to Coordinate Agent Collaboration using `AgentGroupChat`](./agent-chat-example.md)
