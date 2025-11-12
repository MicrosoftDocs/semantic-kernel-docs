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
> Not all agent types support structured output. In this step we are using a `ChatClientAgent`, which does support structured output.

## Prerequisites

For prerequisites and installing nuget packages, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Creating the agent with structured output

The `ChatClientAgent` is built on top of any `Microsoft.Extensions.AI.IChatClient` implementation.
The `ChatClientAgent` uses the support for structured output that is provided by the underlying chat client.

When creating the agent, we have the option to provide the default `ChatOptions` instance to use for the underlying chat client.
This `ChatOptions` instance allows us to pick a preferred [`ChatResponseFormat`](/dotnet/api/microsoft.extensions.ai.chatresponseformat).

Various options are supported:

- `ChatResponseFormat.Text`: The response will be plain text.
- `ChatResponseFormat.Json`: The response will be a JSON object without any particular schema.
- `ChatResponseFormatJson.ForJsonSchema`: The response will be a JSON object that conforms to the provided schema.

Let's look at an example of creating an agent that produces structured output in the form of a JSON object that conforms to a specific schema.

The easiest way to produce the schema is to define a C# class that represents the structure of the output you want from the agent, and then use the `AIJsonUtilities.CreateJsonSchema` method to create a schema from the type.

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;

public class PersonInfo
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("age")]
    public int? Age { get; set; }

    [JsonPropertyName("occupation")]
    public string? Occupation { get; set; }
}

JsonElement schema = AIJsonUtilities.CreateJsonSchema(typeof(PersonInfo));
```

We can then create a `ChatOptions` instance that uses this schema for the response format.

```csharp
using Microsoft.Extensions.AI;

ChatOptions chatOptions = new()
{
    ResponseFormat = ChatResponseFormatJson.ForJsonSchema(
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
    new AzureCliCredential())
        .GetChatClient("gpt-4o-mini")
        .CreateAIAgent(new ChatClientAgentOptions()
        {
            Name = "HelpfulAssistant",
            Instructions = "You are a helpful assistant.",
            ChatOptions = chatOptions
        });
```

Now we can just run the agent with some textual information that the agent can use to fill in the structured output.

```csharp
var response = await agent.RunAsync("Please provide information about John Smith, who is a 35-year-old software engineer.");
```

The agent response can then be deserialized into the `PersonInfo` class using the `Deserialize<T>` method on the response object.

```csharp
var personInfo = JsonSerializer.Deserialize<PersonInfo>(response.ToString(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
Console.WriteLine($"Name: {personInfo.Name}, Age: {personInfo.Age}, Occupation: {personInfo.Occupation}");
```

When streaming, the agent response is streamed as a series of updates, and we can only deserialize the response once we have received all the updates.
We therefore need to assemble all the updates into a single response, before deserializing it.

```csharp
var updates = agent.RunStreamingAsync("Please provide information about John Smith, who is a 35-year-old software engineer.");
personInfo = (await updates.ToAgentRunResponseAsync()).Deserialize<PersonInfo>(JsonSerializerOptions.Web);
```

::: zone-end
::: zone pivot="programming-language-python"

This tutorial step shows you how to produce structured output with an agent, where the agent is built on the Azure OpenAI Chat Completion service.

> [!IMPORTANT]
> Not all agent types support structured output. The `ChatAgent` supports structured output when used with compatible chat clients.

## Prerequisites

For prerequisites and installing packages, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Creating the agent with structured output

The `ChatAgent` is built on top of any chat client implementation that supports structured output.
The `ChatAgent` uses the `response_format` parameter to specify the desired output schema.

When creating or running the agent, we can provide a Pydantic model that defines the structure of the expected output.

Various response formats are supported based on the underlying chat client capabilities.

Let's look at an example of creating an agent that produces structured output in the form of a JSON object that conforms to a Pydantic model schema.

First, define a Pydantic model that represents the structure of the output you want from the agent:

```python
from pydantic import BaseModel

class PersonInfo(BaseModel):
    """Information about a person."""
    name: str | None = None
    age: int | None = None
    occupation: str | None = None
```

Now we can create an agent using the Azure OpenAI Chat Client:

```python
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity import AzureCliCredential

# Create the agent using Azure OpenAI Chat Client
agent = AzureOpenAIChatClient(credential=AzureCliCredential()).create_agent(
    name="HelpfulAssistant",
    instructions="You are a helpful assistant that extracts person information from text."
)
```

Now we can run the agent with some textual information and specify the structured output format using the `response_format` parameter:

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

When streaming, the agent response is streamed as a series of updates. To get the structured output, we need to collect all the updates and then access the final response value:

```python
from agent_framework import AgentRunResponse

# Get structured response from streaming agent using AgentRunResponse.from_agent_response_generator
# This method collects all streaming updates and combines them into a single AgentRunResponse
final_response = await AgentRunResponse.from_agent_response_generator(
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
