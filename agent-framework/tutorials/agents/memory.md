---
title: Adding Memory to an Agent
description: How to add memory to an agent using an AIContextProvider.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/25/2025
ms.service: semantic-kernel
---

# Adding Memory to an Agent

This tutorial shows how to add memory to an agent by implementing an `AIContextProvider` and attaching it to the agent.

> [!IMPORTANT]
> Not all agent types support `AIContextProvider`. In this step we are using a `ChatClientAgent`, which does support `AIContextProvider`.

::: zone pivot="programming-language-csharp"

## Prerequisites

For prerequisites and installing nuget packages, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Creating an AIContextProvider

`AIContextProvider` is an abstract class that you can inherit from, and which can be associated with the `AgentThread` for a `ChatClientAgent`.
It allows you to:

1. run custom logic before and after the agent invokes the underlying inference service
1. provide additional context to the agent before it invokes the underlying inference service
1. inspect all messages provided to and produced by the agent

### Pre and post invocation events

The `AIContextProvider` class has two methods that you can override to run custom logic before and after the agent invokes the underlying inference service:

- `InvokingAsync` - called before the agent invokes the underlying inference service. You can provide additional context to the agent by returning an `AIContext` object. This context will be merged with the agent's existing context before invoking the underlying service. It is possible to provide instructions, tools, and messages to add to the request.
- `InvokedAsync` - called after the agent has received a response from the underlying inference service. You can inspect the request and response messages, and update the state of the context provider.

### Serialization

`AIContextProvider` instances are created and attached to an `AgentThread` when the thread is created, and when a thread is resumed from a serialized state.

The `AIContextProvider` instance may have its own state that needs to be persisted between invocations of the agent. E.g. a memory component that remembers information about the user may have memories as part of its state.

To allow persisting threads, you need to implement the `SerializeAsync` method of the `AIContextProvider` class. You also need to provide a constructor that takes a `JsonElement` parameter, which can be used to deserialize the state when resuming a thread.

### Sample AIContextProvider implementation

Let's look at an example of a custom memory component that remembers a user's name and age, and provides it to the agent before each invocation.

First we'll create a model class to hold the memories.

```csharp
internal sealed class UserInfo
{
    public string? UserName { get; set; }
    public int? UserAge { get; set; }
}
```

Then we can implement the `AIContextProvider` to manage the memories.
The `UserInfoMemory` class below contains the following behavior:

1. It uses a `IChatClient` to look for the user's name and age in user messages when new messages are added to the thread at the end of each run.
1. It provides any current memories to the agent before each invocation.
1. If not memories are available, it instructs the agent to ask the user for the missing information, and not to answer any questions until the information is provided.
1. It also implements serialization to allow persisting the memories as part of the thread state.

```csharp
internal sealed class UserInfoMemory : AIContextProvider
{
    private readonly IChatClient _chatClient;
    public UserInfoMemory(IChatClient chatClient, UserInfo? userInfo = null)
    {
        this._chatClient = chatClient;
        this.UserInfo = userInfo ?? new UserInfo();
    }

    public UserInfoMemory(IChatClient chatClient, JsonElement serializedState, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        this._chatClient = chatClient;
        this.UserInfo = serializedState.ValueKind == JsonValueKind.Object ?
            serializedState.Deserialize<UserInfo>(jsonSerializerOptions)! :
            new UserInfo();
    }

    public UserInfo UserInfo { get; set; }

    public override async ValueTask InvokedAsync(
        InvokedContext context,
        CancellationToken cancellationToken = default)
    {
        if ((this.UserInfo.UserName is null || this.UserInfo.UserAge is null) && context.RequestMessages.Any(x => x.Role == ChatRole.User))
        {
            var result = await this._chatClient.GetResponseAsync<UserInfo>(
                context.RequestMessages,
                new ChatOptions()
                {
                    Instructions = "Extract the user's name and age from the message if present. If not present return nulls."
                },
                cancellationToken: cancellationToken);
            this.UserInfo.UserName ??= result.Result.UserName;
            this.UserInfo.UserAge ??= result.Result.UserAge;
        }
    }

    public override ValueTask<AIContext> InvokingAsync(
        InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        StringBuilder instructions = new();
        instructions
            .AppendLine(
                this.UserInfo.UserName is null ?
                    "Ask the user for their name and politely decline to answer any questions until they provide it." :
                    $"The user's name is {this.UserInfo.UserName}.")
            .AppendLine(
                this.UserInfo.UserAge is null ?
                    "Ask the user for their age and politely decline to answer any questions until they provide it." :
                    $"The user's age is {this.UserInfo.UserAge}.");
        return new ValueTask<AIContext>(new AIContext
        {
            Instructions = instructions.ToString()
        });
    }

    public override ValueTask<JsonElement?> SerializeAsync(
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        return new ValueTask<JsonElement?>(JsonSerializer.SerializeToElement(this.UserInfo, jsonSerializerOptions));
    }
}
```

## Using the AIContextProvider with an agent

To use the custom `AIContextProvider`, you need to provide an `AIContextProviderFactory` when creating the agent. This factory allows the agent to create a new instance of the desired `AIContextProvider` for each thread.

When creating a `ChatClientAgent` it is possible to provide a `ChatClientAgentOptions` object that allows providing the `AIContextProviderFactory` in addition to all other agent options.

```csharp
ChatClient chatClient = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
    .GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.CreateAIAgent(new ChatClientAgentOptions()
{
    Instructions = "You are a friendly assistant. Always address the user by their name.",
    AIContextProviderFactory = ctx => new UserInfoMemory(
        chatClient.AsIChatClient(),
        ctx.SerializedState,
        ctx.JsonSerializerOptions)
});
```

When creating a new thread, the `AIContextProvider` will be created by `GetNewThread`
and attached to the thread. Once memories are extracted it is therefore possible to access the memory component via the thread's `GetService` method and inspect the memories.

```csharp
// Create a new thread for the conversation.
AgentThread thread = agent.GetNewThread();

Console.WriteLine(await agent.RunAsync("Hello, what is the square root of 9?", thread));
Console.WriteLine(await agent.RunAsync("My name is Ruaidhrí", thread));
Console.WriteLine(await agent.RunAsync("I am 20 years old", thread));

// Access the memory component via the thread's GetService method.
var userInfo = deserializedThread.GetService<UserInfoMemory>()?.UserInfo;
Console.WriteLine($"MEMORY - User Name: {userInfo?.UserName}");
Console.WriteLine($"MEMORY - User Age: {userInfo?.UserAge}");
```

::: zone-end
::: zone pivot="programming-language-python"

Tutorial coming soon.

::: zone-end
