---
title: Producing Structured Output with agents
description: Learn how to use produce structured output with an agent
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/15/2025
ms.service: agent-framework
---

# Producing Structured Output with Agents

::: zone pivot="programming-language-csharp"

This tutorial step shows you how to produce structured output with an agent, where the agent is built on the Azure OpenAI Chat Completion service.

> [!IMPORTANT]
> Not all agent types support structured output. This step uses a `ChatClientAgent`, which does support structured output.

## Prerequisites

For prerequisites and installing NuGet packages, see the [Create and run a simple agent](./running-agents.md) step in this tutorial.

## Create the agent with structured output

The `ChatClientAgent` is built on top of any <xref:Microsoft.Extensions.AI.IChatClient> implementation.
The `ChatClientAgent` uses the support for structured output that's provided by the underlying chat client.

When creating the agent, you have the option to provide the default <xref:Microsoft.Extensions.AI.ChatOptions> instance to use for the underlying chat client.
This `ChatOptions` instance allows you to pick a preferred <xref:Microsoft.Extensions.AI.ChatResponseFormat>.

Various options for `ResponseFormat` are available:

- A built-in <xref:Microsoft.Extensions.AI.ChatResponseFormat.Text?displayProperty=nameWithType> property: The response will be plain text.
- A built-in <xref:Microsoft.Extensions.AI.ChatResponseFormat.Json?displayProperty=nameWithType> property: The response will be a JSON object without any particular schema.
- A custom <xref:Microsoft.Extensions.AI.ChatResponseFormatJson> instance: The response will be a JSON object that conforms to a specific schema.

This example creates an agent that produces structured output in the form of a JSON object that conforms to a specific schema.

The easiest way to produce the schema is to define a type that represents the structure of the output you want from the agent, and then use the `AIJsonUtilities.CreateJsonSchema` method to create a schema from the type.

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;

public class PersonInfo
{
    public string? Name { get; set; }
    public int? Age { get; set; }
    public string? Occupation { get; set; }
}

JsonElement schema = AIJsonUtilities.CreateJsonSchema(typeof(PersonInfo));
```

You can then create a <xref:Microsoft.Extensions.AI.ChatOptions> instance that uses this schema for the response format.

```csharp
using Microsoft.Extensions.AI;

ChatOptions chatOptions = new()
{
    ResponseFormat = ChatResponseFormat.ForJsonSchema(
        schema: schema,
        schemaName: "PersonInfo",
        schemaDescription: "Information about a person including their name, age, and occupation")
};
```

This `ChatOptions` instance can be used when creating the agent.

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI;

AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new DefaultAzureCredential())
        .GetChatClient("gpt-4o-mini")
        .AsAIAgent(new ChatClientAgentOptions()
        {
            Name = "HelpfulAssistant",
            Instructions = "You are a helpful assistant.",
            ChatOptions = chatOptions
        });
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

Now you can just run the agent with some textual information that the agent can use to fill in the structured output.

```csharp
var response = await agent.RunAsync("Please provide information about John Smith, who is a 35-year-old software engineer.");
```

The agent response can then be deserialized into the `PersonInfo` class using the `Deserialize<T>` method on the response object.

```csharp
var personInfo = response.Deserialize<PersonInfo>(JsonSerializerOptions.Web);
Console.WriteLine($"Name: {personInfo.Name}, Age: {personInfo.Age}, Occupation: {personInfo.Occupation}");
```

When streaming, the agent response is streamed as a series of updates, and you can only deserialize the response once all the updates have been received.
You must assemble all the updates into a single response before deserializing it.

```csharp
var updates = agent.RunStreamingAsync("Please provide information about John Smith, who is a 35-year-old software engineer.");
personInfo = (await updates.ToAgentResponseAsync()).Deserialize<PersonInfo>(JsonSerializerOptions.Web);
```

> [!TIP]
> See the [.NET samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples) for complete runnable examples.

### Streaming example

> [!TIP]
> See the [.NET samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples) for complete runnable examples.

::: zone-end
::: zone pivot="programming-language-python"

This tutorial step shows you how to produce structured output with an agent, where the agent is built on the Azure OpenAI Chat Completion service.

> [!IMPORTANT]
> Not all agent types support structured output. The `Agent` supports structured output when used with compatible chat clients.

## Prerequisites

For prerequisites and installing packages, see the [Create and run a simple agent](./running-agents.md) step in this tutorial.

## Create the agent with structured output

The `Agent` is built on top of any chat client implementation that supports structured output.
The `Agent` uses the `response_format` parameter to specify the desired output schema.

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

When streaming, `agent.run(..., stream=True)` returns a `ResponseStream`. The stream's built-in finalizer automatically handles structured output parsing, so you can iterate for real-time updates and then call `get_final_response()` to get the parsed result:

```python
# Stream updates in real time, then get the structured result
stream = agent.run(query, stream=True, options={"response_format": PersonInfo})
async for update in stream:
    print(update.text, end="", flush=True)

# get_final_response() returns the AgentResponse with the parsed value
final_response = await stream.get_final_response()

if final_response.value:
    person_info = final_response.value
    print(f"Name: {person_info.name}, Age: {person_info.age}, Occupation: {person_info.occupation}")
```

If you don't need to process individual streaming updates, you can skip iteration entirely — `get_final_response()` will automatically consume the stream:

```python
stream = agent.run(query, stream=True, options={"response_format": PersonInfo})
final_response = await stream.get_final_response()

if final_response.value:
    person_info = final_response.value
    print(f"Name: {person_info.name}, Age: {person_info.age}, Occupation: {person_info.occupation}")
```

### Complete example

```python
# Copyright (c) Microsoft. All rights reserved.

import asyncio

from agent_framework.openai import OpenAIResponsesClient
from pydantic import BaseModel

"""
OpenAI Responses Client with Structured Output Example

This sample demonstrates using structured output capabilities with OpenAI Responses Client,
showing Pydantic model integration for type-safe response parsing and data extraction.
"""


class OutputStruct(BaseModel):
    """A structured output for testing purposes."""

    city: str
    description: str


async def non_streaming_example() -> None:
    print("=== Non-streaming example ===")

    agent = OpenAIResponsesClient().as_agent(
        name="CityAgent",
        instructions="You are a helpful agent that describes cities in a structured format.",
    )

    query = "Tell me about Paris, France"
    print(f"User: {query}")

    result = await agent.run(query, options={"response_format": OutputStruct})

    if structured_data := result.value:
        print("Structured Output Agent:")
        print(f"City: {structured_data.city}")
        print(f"Description: {structured_data.description}")
    else:
        print(f"Failed to parse response: {result.text}")


async def streaming_example() -> None:
    print("=== Streaming example ===")

    agent = OpenAIResponsesClient().as_agent(
        name="CityAgent",
        instructions="You are a helpful agent that describes cities in a structured format.",
    )

    query = "Tell me about Tokyo, Japan"
    print(f"User: {query}")

    # Stream updates in real time using ResponseStream
    stream = agent.run(query, stream=True, options={"response_format": OutputStruct})
    async for update in stream:
        if update.text:
            print(update.text, end="", flush=True)
    print()

    # get_final_response() returns the AgentResponse with structured output parsed
    result = await stream.get_final_response()

    if structured_data := result.value:
        print("Structured Output (from streaming with ResponseStream):")
        print(f"City: {structured_data.city}")
        print(f"Description: {structured_data.description}")
    else:
        print(f"Failed to parse response: {result.text}")


async def main() -> None:
    print("=== OpenAI Responses Agent with Structured Output ===")

    await non_streaming_example()
    await streaming_example()


if __name__ == "__main__":
    asyncio.run(main())
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Background Responses](./background-responses.md)
