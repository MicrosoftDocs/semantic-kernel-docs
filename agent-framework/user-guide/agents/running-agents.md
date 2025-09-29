---
title: Running Agents
description: Learn how to run agents with Agent Framework
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: reference
ms.author: markwallace
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# Running Agents

The base Agent abstraction exposes various options for running the agent. Callers can choose to supply zero, one or many input messages. Callers can also choose between streaming and non-streaming. Let's dig into the different usage scenarios.

## Streaming and non-streaming

The Microsoft Agent Framework supports both streaming and non-streaming methods for running an agent.

::: zone pivot="programming-language-csharp"

For non-streaming, use the `RunAsync` method.

```csharp
Console.WriteLine(await agent.RunAsync("What is the weather like in Amsterdam?"));
```

For streaming, use the `RunStreamingAsync` method.

```csharp
await foreach (var update in agent.RunStreamingAsync("What is the weather like in Amsterdam?"))
{
    Console.Write(update);
}
```

::: zone-end
::: zone pivot="programming-language-python"

::: zone-end

## Agent run options

The base agent abstraction does allow passing an options object for each agent run, however the ability to customize a run at the abstraction level is quite limited.
Agents can vary significantly and therefore there aren't really common customization options.

::: zone pivot="programming-language-csharp"

For cases where the caller knows the type of the agent they are working with, it is possible to pass type specific options to allow customizing the run.

For example, here the agent is a `ChatClientAgent` and it is possible to pass a `ChatClientAgentRunOptions` object that inherits from `AgentRunOptions`.
This allows the caller to provide custom `ChatOptions` that are merged with any agent level options before being passed to the `IChatClient` that
the `ChatClientAgent` is built on.

```csharp
var chatOptions = new ChatOptions() { Tools = [AIFunctionFactory.Create(GetWeather)] };
Console.WriteLine(await agent.RunAsync("What is the weather like in Amsterdam?", options: new ChatClientAgentRunOptions(chatOptions)));
```

::: zone-end
::: zone pivot="programming-language-python"

::: zone-end

## Response types

Both streaming and non-streaming responses from agents contain all content produced by the agent.
Content may include data that is not the result (i.e. the answer to the user question) from the agent.
Examples of other data returned include function tool calls, results from function tool calls, reasoning text, status updates, and many more.

Since not all content returned is the result, it's important to look for specific content types when trying to isolate the result from the other content.

::: zone pivot="programming-language-csharp"

To extract the text result from a response, all `TextContent` items from all `ChatMessages` items need to be aggregated.
To simplify this, we provide a `Text` property on all response types that aggregate this all `TextContent`.

For the non-streaming case, everything is returned in one `AgentRunResponse` object.
`AgentRunResponse` allows access to the produced messages via the `Messages` property.

```csharp
var response = await agent.RunAsync("What is the weather like in Amsterdam?");
Console.WriteLine(response.Text);
Console.WriteLine(response.Messages.Count);
```

For the streaming case, `AgentRunResponseUpdate` objects are streamed as they are produced.
Each update may contain a part of the result from the agent, and also various other content items.
Similar to the non-streaming case, it is possible to use the `Text` property to get the portion
of the result contained in the update, and drill into the detail via the `Contents` property.

```csharp
await foreach (var update in agent.RunStreamingAsync("What is the weather like in Amsterdam?"))
{
    Console.WriteLine(update.Text);
    Console.WriteLine(update.Contents.Count);
}
```

::: zone-end
::: zone pivot="programming-language-python"

::: zone-end

## Message types

Input and output from agents are represented as messages. Messages are subdivided into content items.

::: zone pivot="programming-language-csharp"

The Microsoft Agent Framework uses the message and content types provided by the `Microsoft.Extensions.AI` abstractions.
Messages are represented by the `ChatMessage` class and all content classes inherit from the base `AIContent` class.

Various `AIContent` subclasses exist that are used to represent different types of content. Some are provided as
part of the base `Microsoft.Extensions.AI` abstractions, but providers can also add their own types, where needed.

Here are some popular types from `Microsoft.Extensions.AI`:

|Type|Description|
|---|---|
|TextContent|Textual content that can be both input, e.g. from a user or developer, and output from the agent. Typically contains the text result from an agent.|
|DataContent|Binary content that can be both input and output. Can be used to pass image, audio or video data to and from the agent (where supported).|
|UriContent|A url that typically points at hosted content such as an image, audio or video.|
|FunctionCallContent|A request by an inference service to invoke a function tool.|
|FunctionResultContent|The result of a function tool invocation.|

::: zone-end
::: zone pivot="programming-language-python"

::: zone-end
