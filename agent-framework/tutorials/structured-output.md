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

# Producing Structured Output with agents

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
This `ChatOptions` instance allows us to pick a preferred [`ChatResponseFormat`](https://learn.microsoft.com/dotnet/api/microsoft.extensions.ai.chatresponseformat).

Various options are supported:

- `ChatResponseFormat.Text`: The response will be plain text.
- `ChatResponseFormat.Json`: The response will be a JSON object without any particular schema.
- `ChatResponseFormatJson.ForJsonSchema`: The response will be a JSON object that conforms to the provided schema.

Let's look at an example of creating an agent that produces structured output in the form of a JSON object that conforms to a specific schema.

The easiest way to produce the schema is to define a C# class that represents the structure of the output you want from the agent, and then use the `AIJsonUtilities.CreateJsonSchema` method to create a schema from the type.

```csharp
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
var personInfo = response.Deserialize<PersonInfo>(JsonSerializerOptions.Web);
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

Tutorial coming soon.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Using an agent as a function tool](./agent-as-function-tool.md)
