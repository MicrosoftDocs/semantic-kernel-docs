---
title: Producing Structured Output with agents
description: Learn how to use structured output with an agent
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 02/10/2026
ms.service: agent-framework
---

# Producing Structured Output with Agents

::: zone pivot="programming-language-csharp"

This tutorial step shows you how to produce structured output with an agent, where the agent is built on the Azure OpenAI Chat Completion service.

> [!IMPORTANT]
> Not all agent types support structured output natively. The `ChatClientAgent` supports structured output when used with compatible chat clients. For agents without native structured output support, you can use the `StructuredOutputAgent` decorator.

## Prerequisites

For prerequisites and installing NuGet packages, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Define a type for the structured output

First, define a type that represents the structure of the output you want from the agent.

```csharp
public class PersonInfo
{
    public string? Name { get; set; }
    public int? Age { get; set; }
    public string? Occupation { get; set; }
}
```

## Create the agent

Create a `ChatClientAgent` using the Azure OpenAI Chat Client.

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;

AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o-mini")
        .AsAIAgent(name: "HelpfulAssistant", instructions: "You are a helpful assistant.");
```

## Structured output with RunAsync\<T\>

The `RunAsync<T>` method is available on the `AIAgent` base class. It accepts a generic type parameter that specifies the structured output type.
This approach is applicable when the structured output type is known at compile time and a typed result instance is needed. It supports primitives, arrays, and complex types.

```csharp
AgentResponse<PersonInfo> response = await agent.RunAsync<PersonInfo>("Please provide information about John Smith, who is a 35-year-old software engineer.");

Console.WriteLine($"Name: {response.Result.Name}, Age: {response.Result.Age}, Occupation: {response.Result.Occupation}");
```

## Structured output with ResponseFormat

Structured output can be configured by setting the `ResponseFormat` property on `AgentRunOptions` at invocation time, or at agent initialization time for agents that support it, such as `ChatClientAgent` and Foundry Agent.

This approach is applicable when:

- The structured output type is not known at compile time.
- The schema is represented as raw JSON.
- Structured output can only be configured at agent creation time.
- Only the raw JSON text is needed without deserialization.
- Inter-agent collaboration is used.

Various options for `ResponseFormat` are available:

- A built-in <xref:Microsoft.Extensions.AI.ChatResponseFormat.Text?displayProperty=nameWithType> property: The response will be plain text.
- A built-in <xref:Microsoft.Extensions.AI.ChatResponseFormat.Json?displayProperty=nameWithType> property: The response will be a JSON object without any particular schema.
- A custom <xref:Microsoft.Extensions.AI.ChatResponseFormatJson> instance: The response will be a JSON object that conforms to a specific schema.

> [!NOTE]
> Primitives and arrays are not supported by the `ResponseFormat` approach. If you need to work with primitives or arrays, use the `RunAsync<T>` approach or create a wrapper type.
>
> ```csharp
> // Instead of using List<string> directly, create a wrapper type:
> public class MovieListWrapper
> {
>     public List<string> Movies { get; set; }
> }
> ```

```csharp
using System.Text.Json;
using Microsoft.Extensions.AI;

AgentRunOptions runOptions = new()
{
    ResponseFormat = ChatResponseFormat.ForJsonSchema<PersonInfo>()
};

AgentResponse response = await agent.RunAsync("Please provide information about John Smith, who is a 35-year-old software engineer.", options: runOptions);

PersonInfo personInfo = JsonSerializer.Deserialize<PersonInfo>(response.Text, JsonSerializerOptions.Web)!;

Console.WriteLine($"Name: {personInfo.Name}, Age: {personInfo.Age}, Occupation: {personInfo.Occupation}");
```

The `ResponseFormat` can also be specified using a raw JSON schema string, which is useful when there is no corresponding .NET type available, such as for declarative agents or schemas loaded from external configuration:

```csharp
string jsonSchema = """
{
    "type": "object",
    "properties": {
        "name": { "type": "string" },
        "age": { "type": "integer" },
        "occupation": { "type": "string" }
    },
    "required": ["name", "age", "occupation"]
}
""";

AgentRunOptions runOptions = new()
{
    ResponseFormat = ChatResponseFormat.ForJsonSchema(JsonElement.Parse(jsonSchema), "PersonInfo", "Information about a person")
};

AgentResponse response = await agent.RunAsync("Please provide information about John Smith, who is a 35-year-old software engineer.", options: runOptions);

Console.WriteLine(response.Text);
```

## Structured output with streaming

When streaming, the agent response is streamed as a series of updates, and you can only deserialize the response once all the updates have been received.
You must assemble all the updates into a single response before deserializing it.

```csharp
using System.Text.Json;
using Microsoft.Extensions.AI;

AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o-mini")
        .AsAIAgent(new ChatClientAgentOptions()
        {
            Name = "HelpfulAssistant",
            Instructions = "You are a helpful assistant.",
            ChatOptions = new() { ResponseFormat = ChatResponseFormat.ForJsonSchema<PersonInfo>() }
        });

IAsyncEnumerable<AgentResponseUpdate> updates = agent.RunStreamingAsync("Please provide information about John Smith, who is a 35-year-old software engineer.");

AgentResponse response = await updates.ToAgentResponseAsync();

PersonInfo personInfo = JsonSerializer.Deserialize<PersonInfo>(response.Text)!;

Console.WriteLine($"Name: {personInfo.Name}, Age: {personInfo.Age}, Occupation: {personInfo.Occupation}");
```

## Structured output with agents with no structured output capabilities

Some agents don't natively support structured output, either because it's not part of the protocol (for example, an A2A agent) or because the agents use language models without structured output capabilities.
To address this, you can use the `StructuredOutputAgent` decorator, which wraps any `AIAgent` and converts the agent's text response into structured JSON using a configured chat client. The decorator can be applied using the `UseStructuredOutput` method on the agent builder.

```csharp
using Microsoft.Extensions.AI;

IChatClient chatClient = chatClient.AsIChatClient();

// The base agent does not have structured output capabilities.
AIAgent baseAgent = chatClient.AsAIAgent(name: "HelpfulAssistant", instructions: "You are a helpful assistant.");

AIAgent agent = baseAgent
    .AsBuilder()
    .UseStructuredOutput(chatClient)
    .Build();

AgentResponse<PersonInfo> response = await agent.RunAsync<PersonInfo>("Please provide information about John Smith, who is a 35-year-old software engineer.");

Console.WriteLine($"Name: {response.Result.Name}, Age: {response.Result.Age}, Occupation: {response.Result.Occupation}");
```

::: zone-end
::: zone pivot="programming-language-python"

This tutorial step shows you how to produce structured output with an agent, where the agent is built on the Azure OpenAI Chat Completion service.

> [!IMPORTANT]
> Not all agent types support structured output. The `ChatAgent` supports structured output when used with compatible chat clients.

## Prerequisites

For prerequisites and installing packages, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Create the agent with structured output

The `ChatAgent` is built on top of any chat client implementation that supports structured output.
The `ChatAgent` uses the `response_format` parameter to specify the desired output schema.

When creating or running the agent, you can provide a Pydantic model that defines the structure of the expected output.

Various response formats are supported based on the underlying chat client capabilities.

This example creates an agent that produces structured output in the form of a JSON object that conforms to a Pydantic model schema.

First, define a Pydantic model that represents the structure of the output you want from the agent:

```python
from pydantic import BaseModel

class PersonInfo(BaseModel):
    """Information about a person."""
    name: str | None = None
    age: int | None = None
    occupation: str | None = None
```

Now you can create an agent using the Azure OpenAI Chat Client:

```python
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity import AzureCliCredential

# Create the agent using Azure OpenAI Chat Client
agent = AzureOpenAIChatClient(credential=AzureCliCredential()).as_agent(
    name="HelpfulAssistant",
    instructions="You are a helpful assistant that extracts person information from text."
)
```

Now you can run the agent with some textual information and specify the structured output format using the `response_format` parameter:

```python
response = await agent.run(
    "Please provide information about John Smith, who is a 35-year-old software engineer.",
    response_format=PersonInfo
)
```

The agent response will contain the structured output in the `value` property, which can be accessed directly as a Pydantic model instance:

```python
if response.value:
    person_info = response.value
    print(f"Name: {person_info.name}, Age: {person_info.age}, Occupation: {person_info.occupation}")
else:
    print("No structured data found in response")
```

When streaming, the agent response is streamed as a series of updates. To get the structured output, you must collect all the updates and then access the final response value:

```python
from agent_framework import AgentResponse

# Get structured response from streaming agent using AgentResponse.from_agent_response_generator
# This method collects all streaming updates and combines them into a single AgentResponse
final_response = await AgentResponse.from_agent_response_generator(
    agent.run_stream(query, response_format=PersonInfo),
    output_format_type=PersonInfo,
)

if final_response.value:
    person_info = final_response.value
    print(f"Name: {person_info.name}, Age: {person_info.age}, Occupation: {person_info.occupation}")
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Using an agent as a function tool](./agent-as-function-tool.md)
