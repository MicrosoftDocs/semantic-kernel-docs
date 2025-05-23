---
title: Semantic Kernel Agent Orchestration Advanced Topics
description: Advanced topics for agent orchestration
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 05/19/2025
ms.service: semantic-kernel
---

# Semantic Kernel Agent Orchestration Advanced Topics

> [!IMPORTANT]
> Agent Orchestrations are in the experimental stage. These patterns are under active development and may change significantly before advancing to the preview or release candidate stage.

## Runtime

The runtime is the foundational component that manages the lifecycle, communication, and execution of agents and orchestrations. It acts as the message bus and execution environment for all actors (agents and orchestration-specific actors) in the system.

### Role of the Runtime

- **Message Routing:** The runtime is responsible for delivering messages between agents and orchestration actors, using a pub-sub or direct messaging model depending on the orchestration pattern.
- **Actor Lifecycle Management:** It creates, registers, and manages the lifecycle of all actors involved in an orchestration, ensuring isolation and proper resource management.
- **Execution Context:** The runtime provides the execution context for orchestrations, allowing multiple orchestrations (and their invocations) to run independently and concurrently.

### Relationship Between Runtime and Orchestrations

Think of an orchestration as a graph that defines how agents interact with each other. The runtime is the engine that executes this graph, managing the flow of messages and the lifecycle of agents. Developers can execute this graph multiple times with different inputs on the same runtime instance, and the runtime will ensure that each execution is isolated and independent.

## Timeouts

When an orchestration is invoked, the orchestration returns immediately with a handler that can be used to get the result later. This asynchronous pattern allows a more flexible and responsive design, especially in scenarios where the orchestration may take a long time to complete.

> [!IMPORTANT]
> If a timeout occurs, the invocation of the orchestration will not be cancelled. The orchestration will continue to run in the background until it completes. Developers can still retrieve the result later.

::: zone pivot="programming-language-csharp"

Developers can get the result of the an orchestration invocation later by calling the `GetValueAsync` method on the result object. When the application is ready to process the result, the invocation may or may not have completed. Therefore, developers can optionally specify a timeout for the `GetValueAsync` method. If the orchestration does not complete within the specified timeout, a timeout exception will be thrown.

```csharp
string output = await result.GetValueAsync(TimeSpan.FromSeconds(60));
```

If the orchestration does not complete within the specified timeout, a timeout exception will be thrown.

::: zone-end

::: zone pivot="programming-language-python"

Developers can get the result of the an orchestration invocation later by calling the `get` method on the result object. When the application is ready to process the result, the invocation may or may not have completed. Therefore, developers can optionally specify a timeout for the `get` method. If the orchestration does not complete within the specified timeout, a timeout exception will be thrown.

```python
value = await orchestration_result.get(timeout=60)
```

If the orchestration does not complete within the specified timeout, a timeout exception will be raised.

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Agent orchestration is not yet available in Java SDK.

::: zone-end

## Human-in-the-loop

### Agent Response Callback

::: zone pivot="programming-language-csharp"

To see agent responses inside an invocation, developers can provide a `ResponseCallback` to the orchestration. This allows developers to observe the responses from each agent during the orchestration process. Developers can use this callback for UI updates, logging, or other purposes.

```csharp
public ValueTask ResponseCallback(ChatMessageContent response)
{
    Console.WriteLine($"# {response.AuthorName}\n{response.Content}");
    return ValueTask.CompletedTask;
}

SequentialOrchestration orchestration = new SequentialOrchestration(
    analystAgent, writerAgent, editorAgent)
{
    ResponseCallback = ResponseCallback,
};
```

::: zone-end

::: zone pivot="programming-language-python"

To see agent responses inside an invocation, developers can provide an `agent_response_callback` to the orchestration. This allows developers to observe the responses from each agent during the orchestration process. Developers can use this callback for UI updates, logging, or other purposes.

```python
def agent_response_callback(message: ChatMessageContent) -> None:
    print(f"# {message.name}\n{message.content}")

sequential_orchestration = SequentialOrchestration(
    members=agents,
    agent_response_callback=agent_response_callback,
)
```

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Agent orchestration is not yet available in Java SDK.

::: zone-end

### Human Response Function

::: zone pivot="programming-language-csharp"

For orchestrations that supports user input (e.g., handoff and group chat), provide an `InteractiveCallback` that returns a `ChatMessageContent` from the user. By using this callback, developers can implement custom logic to gather user input, such as displaying a UI prompt or integrating with other systems.

```csharp
HandoffOrchestration orchestration = new(...)
{
    InteractiveCallback = () =>
    {
        Console.Write("User: ");
        string input = Console.ReadLine();
        return new ChatMessageContent(AuthorRole.User, input);
    }
};
```

::: zone-end

::: zone pivot="programming-language-python"

For orchestrations that supports user input (e.g., handoff and group chat), provide an `human_response_function` that returns a `ChatMessageContent` from the user. By using this callback, developers can implement custom logic to gather user input, such as displaying a UI prompt or integrating with other systems.

```python
def human_response_function() -> ChatMessageContent:
    user_input = input("User: ")
    return ChatMessageContent(role=AuthorRole.USER, content=user_input)

handoff_orchestration = HandoffOrchestration(
    ...,
    agent_response_callback=agent_response_callback,
)
```

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Agent orchestration is not yet available in Java SDK.

::: zone-end

## Structured Data

We believe that structured data is a key part in building agentic workflows. By using structured data, developers can create more reuseable orchestrations and the development experience is improved. The Semantic Kernel SDK provides a way to pass structured data as input to orchestrations and return structured data as output.

> [!IMPORTANT]
> Internally, the orchestrations still process data as `ChatMessageContent`.

### Structured Inputs

::: zone pivot="programming-language-csharp"

Developers can pass structured data as input to orchestrations by using a strongly-typed input class and specifying it as the generic parameter for the orchestration. This enables type safety and more flexibility for orchestrations to handle complex data structures. For example, to triage GitHub issues, define a class for the structured input:

```csharp
public sealed class GithubIssue
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string[] Labels { get; set; } = [];
}
```

Developers can then use this type as the input to an orchestration by providing it as the generic parameter:

```csharp
HandoffOrchestration<GithubIssue, string> orchestration =
    new(...);

GithubIssue input = new GithubIssue { ... };
var result = await orchestration.InvokeAsync(input, runtime);
```

#### Custom Input Transforms

By default, the orchestration will use the built-in input transform, which serializes the object to JSON and wraps it in a `ChatMessageContent`. If you want to customize how your structured input is converted to the underlying message type, you can provide your own input transform function via the `InputTransform` property:

```csharp
HandoffOrchestration<GithubIssue, string> orchestration =
    new(...)
    {
        InputTransform = (issue, cancellationToken) =>
        {
            // For example, create a chat message with a custom format
            var message = new ChatMessageContent(AuthorRole.User, $"[{issue.Id}] {issue.Title}\n{issue.Body}");
            return ValueTask.FromResult<IEnumerable<ChatMessageContent>>([message]);
        },
    };
```

This allows you to control exactly how your typed input is presented to the agents, enabling advanced scenarios such as custom formatting, field selection, or multi-message input.

> [!TIP]
> See the full sample in [Step04a_HandoffWithStructuredInput.cs](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/Orchestration/Step04a_HandoffWithStructuredInput.cs)

::: zone-end

::: zone pivot="programming-language-python"

Developers can pass structured data as input to orchestrations by specifying a Pydantic model (or any custom class) as the generic parameter for the orchestration. This enables type safety and lets the orchestrations handle complex data structures.

For example, to triage GitHub issues, define a Pydantic model for the structured input:

```python
from pydantic import BaseModel

class GithubIssue(BaseModel):
    id: str
    title: str
    body: str
    labels: list[str] = []
```

You can then use this type as the input to your orchestration by providing it as the generic parameter:

```python
from semantic_kernel.agents import HandoffOrchestration

def custom_input_transform(input_message: GithubIssue) -> ChatMessageContent:
    return ChatMessageContent(role=AuthorRole.USER, content=f"[{input_message.id}] {input_message.title}\n{input_message.body}")


handoff_orchestration = HandoffOrchestration[GithubIssue, ChatMessageContent](
    ...,
    input_transform=custom_input_transform,
)

GithubIssueSample = GithubIssue(
    id="12345",
    title="Bug: ...",
    body="Describe the bug...",
    labels=[],
)

orchestration_result = await handoff_orchestration.invoke(
    task=GithubIssueSample,
    runtime=runtime,
)
```

> [!TIP]
> See the full sample in [step4a_handoff_structured_inputs.py](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/getting_started_with_agents/multi_agent_orchestration/step4a_handoff_structured_inputs.py)

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Agent orchestration is not yet available in Java SDK.

::: zone-end

### Structured Outputs

::: zone pivot="programming-language-csharp"

Agents and orchestrations can return structured outputs by specifying a strongly-typed output class as the generic parameter for the orchestration. This enables you to work with rich, structured results in your application, rather than just plain text.

For example, suppose you want to analyze an article and extract themes, sentiments, and entities. Define a class for the structured output:

```csharp
public sealed class Analysis
{
    public IList<string> Themes { get; set; } = [];
    public IList<string> Sentiments { get; set; } = [];
    public IList<string> Entities { get; set; } = [];
}
```

You can then use this type as the output for your orchestration by providing it as the generic parameter:

```csharp
ConcurrentOrchestration<string, Analysis> orchestration =
    new(agent1, agent2, agent3)
    {
        ResultTransform = outputTransform.TransformAsync, // see below
    };

// ...
OrchestrationResult<Analysis> result = await orchestration.InvokeAsync(input, runtime);
Analysis output = await result.GetValueAsync(TimeSpan.FromSeconds(60));
```

#### Custom Output Transforms

By default, the orchestration will use the built-in output transform, which attempts to deserialize the agent's response content to your output type. For more advanced scenarios, you can provide a custom output transform (for example, with structured output by some models).

```csharp
StructuredOutputTransform<Analysis> outputTransform =
    new(chatCompletionService, new OpenAIPromptExecutionSettings { ResponseFormat = typeof(Analysis) });

ConcurrentOrchestration<string, Analysis> orchestration =
    new(agent1, agent2, agent3)
    {
        ResultTransform = outputTransform.TransformAsync,
    };
```

This approach allows you to receive and process structured data directly from the orchestration, making it easier to build advanced workflows and integrations.

> [!TIP]
> See the full sample in [Step01a_ConcurrentWithStructuredOutput.cs](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/Orchestration/Step01a_ConcurrentWithStructuredOutput.cs)

::: zone-end

::: zone pivot="programming-language-python"

Agents and orchestrations can return structured outputs by specifying a Pydantic model (or any custom class) as the generic output type for the orchestration. This enables you to work with rich, structured results in your application, rather than just plain text.

For example, suppose you want to analyze an article and extract themes, sentiments, and entities. Define a Pydantic model for the structured output:

```python
from pydantic import BaseModel

class ArticleAnalysis(BaseModel):
    themes: list[str]
    sentiments: list[str]
    entities: list[str]
```

You can then use this type as the output for your orchestration by providing it as the generic parameter and specifying an output transform:

```python
from semantic_kernel.agents import ConcurrentOrchestration
from semantic_kernel.agents.orchestration.tools import structured_outputs_transform
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion

# `structured_outputs_transform` is a built-in transform that uses the structured output

concurrent_orchestration = ConcurrentOrchestration[str, ArticleAnalysis](
    members=agents,
    output_transform=structured_outputs_transform(ArticleAnalysis, AzureChatCompletion()),
)

...

orchestration_result = await concurrent_orchestration.invoke(
    task="article text",
    runtime=runtime,
)

value = await orchestration_result.get(timeout=20)

# `value` is now an instance of ArticleAnalysis
```

This approach allows you to receive and process structured data directly from the orchestration, making it easier to build advanced workflows and integrations.

> [!TIP]
> See the full sample in [step1a_concurrent_structured_outputs.py](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/getting_started_with_agents/multi_agent_orchestration/step1a_concurrent_structured_outputs.py)

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Agent orchestration is not yet available in Java SDK.

::: zone-end

## Cancellation

> [!IMPORTANT]
> Cancellation will stop the agents from processing any further messages, but it will not stop agents that are already processing messages.

> [!IMPORTANT]
> Cancellation will not stop the runtime.

::: zone pivot="programming-language-csharp"

You can cancel an orchestration by calling the `Cancel` method on the result handler. This will stop the orchestration by propagating the signal to all agents, and they will stop processing any further messages.

```csharp
var resultTask = orchestration.InvokeAsync(input, runtime);
resultTask.Cancel();
```

::: zone-end

::: zone pivot="programming-language-python"

Developers can cancel an orchestration by calling the `cancel` method on the result handler. This will stop the orchestration by propagating the signal to all agents, and they will stop processing any further messages.

```python
orchestration_result = await orchestration.invoke(task=task, runtime=runtime)
orchestration_result.cancel()
```

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Agent orchestration is not yet available in Java SDK.

::: zone-end

## Next steps

::: zone pivot="programming-language-csharp"

> [!div class="nextstepaction"]
> [More Code Samples](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/GettingStartedWithAgents/Orchestration)

::: zone-end

::: zone pivot="programming-language-python"

> [!div class="nextstepaction"]
> [More Code Samples](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/getting_started_with_agents/multi_agent_orchestration)

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Agent orchestration is not yet available in Java SDK.

::: zone-end
