---
title: Agent Framework Release Candidate Migration Guide
description: Describes the steps for developers to update their Agent Framework code to the latest abstractions.
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: conceptual
ms.author: evmattso
ms.date: 02/26/2025
ms.service: semantic-kernel
---

# Agent Framework Release Candidate Migration Guide

As we transition some agents from the experimental stage to the release candidate stage, we have updated the APIs to simplify and streamline their use. Refer to the specific scenario guide to learn how to update your existing code to work with the latest available APIs.

::: zone pivot="programming-language-csharp"

## Common Agent Invocation API

In version 1.43.0 we are releasing a new common agent invocation API, that will allow all agent types to be invoked via a common API.

To enable this new API we are introducing the concept of an `AgentThread`, which represents a conversation thread and abstracts away the different thread management requirements of different agent types. For some agent types it will also, in future, allow different thread imlementations to be used with the same agent.

The common `Invoke` methods that we are introducing allow you to provide the message(s) that you want to pass to the agent and an optional `AgentThread`. If an `AgentThread` is provided, this will continue the conversation already on the `AgentThread`. If no `AgentThread` is provided, a new default thread will be created and returned as part of the response.

It is also possible to manually create an `AgentThread` instance, for example in cases where you may have a thread id from the underlying agent service, and you want to continue that thread. You may also want to customize the options for the thread, e.g. associate tools.

Here is a simple example of how any agent can now be used with agent agnostic code.

```csharp
private async Task UseAgentAsync(Agent agent, AgentThread? agentThread = null)
{
    // Invoke the agent, and continue the existing thread if provided.
    var responses = agent.InvokeAsync(new ChatMessageContent(AuthorRole.User, "Hi"), agentThread);

    // Output results.
    await foreach (AgentResponseItem<ChatMessageContent> response in responses)
    {
        Console.WriteLine(response);
        agentThread = response.Thread;
    }

    // Delete the thread if required.
    if (agentThread is not null)
    {
        await agentThread.DeleteAsync();
    }
}
```

These changes were applied in:

- [PR #11116](https://github.com/microsoft/semantic-kernel/pull/11116)

### Azure AI Agent Thread Options

The `AzureAIAgent` currently only supports threads of type `AzureAIAgentThread`.

In addition to allowing a thread to be created for you automatically on agent invocation, you can also manually
construct an instance of an `AzureAIAgentThread`.

`AzureAIAgentThread` supports being created with customized tools and metadata, plus messages to seed the conversation with.

```csharp
AgentThread thread = new AzureAIAgentThread(
    agentsClient,
    messages: seedMessages,
    toolResources: tools,
    metadata: metadata);
```

You can also construct an instance of an `AzureAIAgentThread` that continues an existing conversation.

```csharp
AgentThread thread = new AzureAIAgentThread(
    agentsClient,
    id: "my-existing-thread-id");
```

### Bedrock Agent Thread Options

The `BedrockAgent` currently only supports threads of type `BedrockAgentThread`.

In addition to allowing a thread to be created for you automatically on agent invocation, you can also manually
construct an instance of an `BedrockAgentThread`.

```csharp
AgentThread thread = new BedrockAgentThread(amazonBedrockAgentRuntimeClient);
```

You can also construct an instance of an `BedrockAgentThread` that continues an existing conversation.

```csharp
AgentThread thread = new BedrockAgentThread(
    amazonBedrockAgentRuntimeClient,
    sessionId: "my-existing-session-id");
```

### Chat Completion Agent Thread Options

The `ChatCompletionAgent` currently only supports threads of type `ChatHistoryAgentThread`.
`ChatHistoryAgentThread` uses an in-memory `ChatHistory` object to store the messages on the thread.

In addition to allowing a thread to be created for you automatically on agent invocation, you can also manually
construct an instance of an `ChatHistoryAgentThread`.

```csharp
AgentThread thread = new ChatHistoryAgentThread();
```

You can also construct an instance of an `ChatHistoryAgentThread` that continues an existing conversation
by passing in a `ChatHistory` object with the existing messages.

```csharp
ChatHistory chatHistory = new([new ChatMessageContent(AuthorRole.User, "Hi")]);

AgentThread thread = new ChatHistoryAgentThread(chatHistory: chatHistory);
```

### OpenAI Assistant Thread Options

The `OpenAIAssistantAgent` currently only supports threads of type `OpenAIAssistantAgentThread`.

In addition to allowing a thread to be created for you automatically on agent invocation, you can also manually
construct an instance of an `OpenAIAssistantAgentThread`.

`OpenAIAssistantAgentThread` supports being created with customized tools and metadata, plus messages to seed the conversation with.

```csharp
AgentThread thread = new OpenAIAssistantAgentThread(
    assistantClient,
    messages: seedMessages,
    codeInterpreterFileIds: fileIds,
    vectorStoreId: "my-vector-store",
    metadata: metadata);
```

You can also construct an instance of an `OpenAIAssistantAgentThread` that continues an existing conversation.

```csharp
AgentThread thread = new OpenAIAssistantAgentThread(
    assistantClient,
    id: "my-existing-thread-id");
```

## OpenAIAssistantAgent C# Migration Guide

We recently applied a significant shift around the [`OpenAIAssistantAgent`](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Agents/OpenAI/OpenAIAssistantAgent.cs) in the _Semantic Kernel Agent Framework_.

These changes were applied in:

- [PR #10583](https://github.com/microsoft/semantic-kernel/pull/10583)
- [PR #10616](https://github.com/microsoft/semantic-kernel/pull/10616)
- [PR #10633](https://github.com/microsoft/semantic-kernel/pull/10633)

These changes are intended to:

- Align with the pattern for using for our [`AzureAIAgent`](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Agents/AzureAI/AzureAIAgent.cs).
- Fix bugs around static initialization pattern.
- Avoid limiting features based on our abstraction of the underlying SDK.

This guide provides step-by-step instructions for migrating your C# code from the old implementation to the new one. Changes include updates for creating assistants, managing the assistant lifecycle, handling threads, files, and vector stores.

## 1. Client Instantiation

Previously, `OpenAIClientProvider` was required for creating any `OpenAIAssistantAgent`. This dependency has been simplified.

#### **New Way**
```csharp
OpenAIClient client = OpenAIAssistantAgent.CreateAzureOpenAIClient(new AzureCliCredential(), new Uri(endpointUrl));
AssistantClient assistantClient = client.GetAssistantClient();
```

#### **Old Way (Deprecated)**
```csharp
var clientProvider = new OpenAIClientProvider(...);
```

## 2. Assistant Lifecycle

### **Creating an Assistant**
You may now directly instantiate an `OpenAIAssistantAgent` using an existing or new Assistant definition from `AssistantClient`.

##### **New Way**
```csharp
Assistant definition = await assistantClient.GetAssistantAsync(assistantId);
OpenAIAssistantAgent agent = new(definition, client);
```

Plugins can be directly included during initialization:
```csharp
KernelPlugin plugin = KernelPluginFactory.CreateFromType<YourPlugin>();
Assistant definition = await assistantClient.GetAssistantAsync(assistantId);
OpenAIAssistantAgent agent = new(definition, client, [plugin]);
```

Creating a new assistant definition using an extension method:
```csharp
Assistant assistant = await assistantClient.CreateAssistantAsync(
    model,
    name,
    instructions: instructions,
    enableCodeInterpreter: true);
```

##### **Old Way (Deprecated)**
Previously, assistant definitions were managed indirectly.

## 3. Invoking the Agent

You may specify `RunCreationOptions` directly, enabling full access to underlying SDK capabilities.

#### **New Way**
```csharp
RunCreationOptions options = new(); // configure as needed
var result = await agent.InvokeAsync(options);
```

#### **Old Way (Deprecated)**
```csharp
var options = new OpenAIAssistantInvocationOptions();
```

## 4. Assistant Deletion

You can directly manage assistant deletion with `AssistantClient`.

```csharp
await assistantClient.DeleteAssistantAsync(agent.Id);
```

## 5. Thread Lifecycle

### **Creating a Thread**

Threads are now managed via `AssistantAgentThread`.

##### **New Way**

```csharp
var thread = new AssistantAgentThread(assistantClient);
// Calling CreateAsync is an optional step.
// A thread will be created automatically on first use if CreateAsync was not called.
// Note that CreateAsync is not on the AgentThread base implementation since not all
// agent services support explicit thread creation.
await thread.CreateAsync();
```

##### **Old Way (Deprecated)**

Previously, thread management was indirect or agent-bound.

### **Thread Deletion**

```csharp
var thread = new AssistantAgentThread(assistantClient, "existing-thread-id");
await thread.DeleteAsync();
```

## 6. File Lifecycle

File creation and deletion now utilize `OpenAIFileClient`.

### **File Upload**
```csharp
string fileId = await client.UploadAssistantFileAsync(stream, "<filename>");
```

### **File Deletion**
```csharp
await client.DeleteFileAsync(fileId);
```

## 7. Vector Store Lifecycle

Vector stores are managed directly via `VectorStoreClient` with convenient extension methods.

### **Vector Store Creation**
```csharp
string vectorStoreId = await client.CreateVectorStoreAsync([fileId1, fileId2], waitUntilCompleted: true);
```

### **Vector Store Deletion**
```csharp
await client.DeleteVectorStoreAsync(vectorStoreId);
```

## Backwards Compatibility

Deprecated patterns are marked with `[Obsolete]`. To suppress obsolete warnings (`CS0618`), update your project file as follows:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);CS0618</NoWarn>
</PropertyGroup>
```

This migration guide helps you transition smoothly to the new implementation, simplifying client initialization, resource management, and integration with the **Semantic Kernel .NET SDK**.

::: zone-end

::: zone pivot="programming-language-python"

> [!IMPORTANT]
> For developers upgrading to Semantic Kernel Python 1.26.1 or later, significant updates and breaking changes have been introduced to improve our agent framework as we approach GA.

These changes were applied in:

- [PR #11116](https://github.com/microsoft/semantic-kernel/pull/11116)

Previous changes were applied in:

- [PR #10666](https://github.com/microsoft/semantic-kernel/pull/10666)
- [PR #10667](https://github.com/microsoft/semantic-kernel/pull/10667)
- [PR #10701](https://github.com/microsoft/semantic-kernel/pull/10701)
- [PR #10707](https://github.com/microsoft/semantic-kernel/pull/10707)

This guide provides step-by-step instructions for migrating your Python code from the old implementation to the new implementation.

## Agent Imports

All agent import paths have been consolidated under `semantic_kernel.agents`.

#### Updated import style

```python
from semantic_kernel.agents import (
    AutoGenConversableAgent,
    AzureAIAgent,
    AzureAssistantAgent,
    BedrockAgent,
    ChatCompletionAgent,
    OpenAIAssistantAgent,
)
```

#### Previous import style (deprecated):

```
from semantic_kernel.agents import ChatCompletionAgent
from semantic_kernel.agents.autogen import AutoGenConversableAgent
from semantic_kernel.agents.azure_ai import AzureAIAgent
from semantic_kernel.agents.bedrock import BedrockAgent
from semantic_kernel.agents.open_ai import AzureAssistantAgent, OpenAIAssistantAgent
```

## Common Agent Invocation API

As of Semantic Kernel Python 1.26.0 and later, we introduced a new common abstraction to manage threads for all agents. For each agent we now expose a thread class that implements the `AgentThread` base class, allowing context management via methods like `create()` and `delete()`.

Agent responses `get_response(...)`, `invoke(...)`, `invoke_stream(...)` now return an `AgentResponseItem[ChatMessageContent]`, which has two attributes:

```python
message: TMessage  # Usually ChatMessageContent
thread: AgentThread  # Contains the concrete type for the given agent
```

### Adding Messages to a Thread

Messages should be added to a thread via the `messages` argument as part of the agent's  `get_response(...)`, `invoke(...)` or `invoke_stream(...)` methods. 

### Azure AI Agent Thread

An `AzureAIAgentThread` can be created as follows:

```python
from semantic_kernel.agents import AzureAIAgentThread

thread = AzureAIAgentThread(
    client: AIProjectClient,  # required
    messages: list[ThreadMessageOptions] | None = None,  # optional
    metadata: dict[str, str] | None = None,  # optional
    thread_id: str | None = None,  # optional
    tool_resources: "ToolResources | None" = None,  # optional
)
```

Providing a `thread_id` (string) allows you to continue an existing conversation. If omitted, a new thread is created and returned as part of the agent response.

A complete implementation example:

```python
import asyncio

from azure.identity.aio import DefaultAzureCredential

from semantic_kernel.agents import AzureAIAgent, AzureAIAgentSettings, AzureAIAgentThread

USER_INPUTS = [
    "Why is the sky blue?",
    "What are we talking about?",
]

async def main() -> None:
    ai_agent_settings = AzureAIAgentSettings.create()

    async with (
        DefaultAzureCredential() as creds,
        AzureAIAgent.create_client(credential=creds) as client,
    ):
        # 1. Create an agent on the Azure AI agent service
        agent_definition = await client.agents.create_agent(
            model=ai_agent_settings.model_deployment_name,
            name="Assistant",
            instructions="Answer the user's questions.",
        )

        # 2. Create a Semantic Kernel agent for the Azure AI agent
        agent = AzureAIAgent(
            client=client,
            definition=agent_definition,
        )

        # 3. Create a thread for the agent
        # If no thread is provided, a new thread will be
        # created and returned with the initial response
        thread: AzureAIAgentThread = None

        try:
            for user_input in USER_INPUTS:
                print(f"# User: {user_input}")
                # 4. Invoke the agent with the specified message for response
                response = await agent.get_response(messages=user_input, thread=thread)
                print(f"# {response.content}: {response}")
                thread = response.thread
        finally:
            # 6. Cleanup: Delete the thread and agent
            await thread.delete() if thread else None
            await client.agents.delete_agent(agent.id)

if __name__ == "__main__":
    asyncio.run(main())
```

### Bedrock Agent Thread

A `BedrockAgent` uses a `BedrockAgentThread` to manage conversation history and context. You may provide a `session_id` to either continue or initiate a fresh conversation context.

```python
from semantic_kernel.agents import BedrockAgentThread

thread = BedrockAgentThread(
    bedrock_runtime_client: Any,
    session_id: str | None = None,
)
```

If no `session_id` is provided, a new context is created automatically.

A complete implementation example:

```python
import asyncio

from semantic_kernel.agents import BedrockAgent, BedrockAgentThread

async def main():
    bedrock_agent = await BedrockAgent.create_and_prepare_agent(
        "semantic-kernel-bedrock-agent",
        instructions="You are a friendly assistant. You help people find information.",
    )

    # Create a thread for the agent
    # If no thread is provided, a new thread will be
    # created and returned with the initial response
    thread: BedrockAgentThread = None

    try:
        while True:
            user_input = input("User:> ")
            if user_input == "exit":
                print("\n\nExiting chat...")
                break

            # Invoke the agent
            # The chat history is maintained in the session
            response = await bedrock_agent.get_response(
                input_text=user_input,
                thread=thread,
            )
            print(f"Bedrock agent: {response}")
            thread = response.thread
    except KeyboardInterrupt:
        print("\n\nExiting chat...")
        return False
    except EOFError:
        print("\n\nExiting chat...")
        return False
    finally:
        # Delete the agent
        await bedrock_agent.delete_agent()
        await thread.delete() if thread else None


if __name__ == "__main__":
    asyncio.run(main())
```

### Chat History Agent Thread

A `ChatCompletionAgent` uses `ChatHistoryAgentThread` to manage conversation history. It can be initialized as follows:

```python
from semantic_kernel.agents import ChatHistoryAgentThread

thread = ChatHistoryAgentThread(
    chat_history: ChatHistory | None = None, 
    thread_id: str | None = None
)
```

Providing a `thread_id` allows continuing existing conversations. Omitting it creates a new thread. Serialization and rehydration of thread state are supported for persistent conversation contexts.

A complete implementation example:

```python
import asyncio

from semantic_kernel.agents import ChatCompletionAgent, ChatHistoryAgentThread
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion

# Simulate a conversation with the agent
USER_INPUTS = [
    "Hello, I am John Doe.",
    "What is your name?",
    "What is my name?",
]


async def main():
    # 1. Create the agent by specifying the service
    agent = ChatCompletionAgent(
        service=AzureChatCompletion(),
        name="Assistant",
        instructions="Answer the user's questions.",
    )

    # 2. Create a thread to hold the conversation
    # If no thread is provided, a new thread will be
    # created and returned with the initial response
    thread: ChatHistoryAgentThread = None

    for user_input in USER_INPUTS:
        print(f"# User: {user_input}")
        # 3. Invoke the agent for a response
        response = await agent.get_response(
            messages=user_input,
            thread=thread,
        )
        print(f"# {response.name}: {response}")
        # 4. Store the thread, which allows the agent to
        # maintain conversation history across multiple messages.
        thread = response.thread

    # 5. Cleanup: Clear the thread
    await thread.delete() if thread else None

if __name__ == "__main__":
    asyncio.run(main())
```

### OpenAI Assistant Thread

The `AzureAssistantAgent` and `OpenAIAssistantAgent` use `AssistantAgentThread` to manage conversation history and context:

```python
from semantic_kernel.agents import ChatHistoryAgentThread

thread = AssistantAgentThread(
    client: AsyncOpenAI,
    thread_id: str | None = None,
    messages: Iterable["ThreadCreateMessage"] | NotGiven = NOT_GIVEN,
    metadata: dict[str, Any] | NotGiven = NOT_GIVEN,
    tool_resources: ToolResources | NotGiven = NOT_GIVEN,
)
```

Providing a `thread_id` continues an existing conversation; otherwise, a new thread is created.

A complete implementation example:

```python
# Copyright (c) Microsoft. All rights reserved.
import asyncio

from semantic_kernel.agents import AzureAssistantAgent


# Simulate a conversation with the agent
USER_INPUTS = [
    "Why is the sky blue?",
    "What is the speed of light?",
    "What have we been talking about?",
]


async def main():
    # 1. Create the client using Azure OpenAI resources and configuration
    client, model = AzureAssistantAgent.setup_resources()

    # 2. Create the assistant on the Azure OpenAI service
    definition = await client.beta.assistants.create(
        model=model,
        instructions="Answer questions about the world in one sentence.",
        name="Assistant",
    )

    # 3. Create a Semantic Kernel agent for the Azure OpenAI assistant
    agent = AzureAssistantAgent(
        client=client,
        definition=definition,
    )

    # 4. Create a new thread for use with the assistant
    # If no thread is provided, a new thread will be
    # created and returned with the initial response
    thread = None

    try:
        for user_input in USER_INPUTS:
            print(f"# User: '{user_input}'")
            # 6. Invoke the agent for the current thread and print the response
            response = await agent.get_response(messages=user_input, thread=thread)
            print(f"# {response.name}: {response}")
            thread = response.thread

    finally:
        # 7. Clean up the resources
        await thread.delete() if thread else None
        await agent.client.beta.assistants.delete(assistant_id=agent.id)


if __name__ == "__main__":
    asyncio.run(main())

```

## Message Inputs for Agent Invocation

Previous implementations allowed only a single message input to methods like `get_response(...)`, `invoke(...)`, and `invoke_stream(...)`. We've now updated these methods to support multiple `messages (str | ChatMessageContent | list[str | ChatMessageContent])`. Message inputs need to be passed in with the `messages` keyword argument, such as `agent.get_response(messages="user input")` or `agent.invoke(messages="user input")`.

Agent invocation methods need updates as follows:

### Old Way

```python
response = await agent.get_response(message="some user input", thread=thread)
```

### New Way

```python
response = await agent.get_response(messages=["some initial inputer", "other input"], thread=thread)
```

## `AzureAIAgent`

In Semantic Kernel Python 1.26.0+, `AzureAIAgent` thread creation is now managed via the `AzureAIAgentThread` object, not directly on the client.

### Old Way

```python
thread = await client.agents.create_thread()
```

### New Way

```python
from semantic_kernel.agents import AzureAIAgentThread

thread = AzureAIAgentThread(
    client: AIProjectClient,  # required
    messages: list[ThreadMessageOptions] | None = None,  # optional
    metadata: dict[str, str] | None = None,  # optional
    thread_id: str | None = None,  # optional
    tool_resources: "ToolResources | None" = None,  # optional
)
```

If no `thread_id` is provided initially, a new thread is created and returned in the agent response.

## `ChatCompletionAgent`

The `ChatCompletionAgent` has been updated to simplify service configuration, plugin handling, and function calling behaviors. Below are the key changes you should consider when migrating.

### 1. Specifying the Service

You can now specify the service directly as part of the agent constructor:

#### New Way

```python
from semantic_kernel.agents import ChatCompletionAgent
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion

agent = ChatCompletionAgent(
    service=AzureChatCompletion(),
    name="<name>",
    instructions="<instructions>",
)
```

Note: If both a kernel and a service are provided, the service will take precedence if it shares the same service_id or ai_model_id. Otherwise, if they are separate, the first AI service registered on the kernel will be used.

#### Old Way (Still Valid)

Previously, you would first add a service to a kernel and then pass the kernel to the agent:

```python
from semantic_kernel.agents import ChatCompletionAgent
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion

kernel = Kernel()
kernel.add_service(AzureChatCompletion())

agent = ChatCompletionAgent(
    kernel=kernel,
    name="<name>",
    instructions="<instructions>",
)
```

### 2. Adding Plugins

Plugins can now be supplied directly through the constructor:

#### New Way

```python
from semantic_kernel.agents import ChatCompletionAgent
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion

agent = ChatCompletionAgent(
    service=AzureChatCompletion(),
    name="<name>",
    instructions="<instructions>",
    plugins=[SamplePlugin()],
)
```

#### Old Way (Still Valid)

Plugins previously had to be added to the kernel separately:

```python
from semantic_kernel.agents import ChatCompletionAgent
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion

kernel = Kernel()
kernel.add_plugin(SamplePlugin())

agent = ChatCompletionAgent(
    kernel=kernel,
    name="<name>",
    instructions="<instructions>",
)
```

Note: Both approaches are valid, but directly specifying plugins simplifies initialization.

### 3. Invoking the Agent

You now have two ways to invoke the agent. The new method directly retrieves a single response, while the old method supports streaming.

#### New Way (No Conversation Thread/Context)

```python
response = await agent.get_response(messages="user input")
# response is of type AgentResponseItem[ChatMessageContent]
```
Note: if the next response does not use the returned thread, the conversation will use a new thread and thus will not continue with previous context.

#### New Way (Single Response with Context)

```python
thread = ChatHistoryAgentThread()

for user_input in ["First user input", "Second User Input"]:
    response = await agent.get_response(messages=user_input, thread=thread)
    # response is of type AgentResponseItem[ChatMessageContent]
    thread = response.thread
```

#### Old Way (No Longer Valid)

```python
chat_history = ChatHistory()
chat_history.add_user_message("<user_input>")
response = agent.get_response(message="user input", chat_history=chat_history)
```

### 4. Controlling Function Calling

Function calling behavior can now be controlled directly when specifying the service within the agent constructor:

```python
agent = ChatCompletionAgent(
    service=AzureChatCompletion(),
    name="<name>",
    instructions="<instructions>",
    plugins=[MenuPlugin()],
    function_choice_behavior=FunctionChoiceBehavior.Auto(
        filters={"included_functions": ["get_specials", "get_item_price"]}
    ),
)
```

Note: Previously, function calling configuration required separate setup on the kernel or service object. If execution settings specify the same `service_id` or `ai_model_id` as the AI service configuration, the function calling behavior defined in the execution settings (via `KernelArguments`) will take precedence over the function choice behavior set in the constructor.

These updates enhance simplicity and configurability, making the ChatCompletionAgent easier to integrate and maintain.

## `OpenAIAssistantAgent`

The `AzureAssistantAgent` and `OpenAIAssistantAgent` changes include updates for creating assistants, creating threads, handling plugins, using the code interpreter tool, working with the file search tool, and adding chat messages to a thread.

## Setting up Resources

### Old Way

The `AsyncAzureOpenAI` client was created as part of creating the Agent object.

```python
agent = await AzureAssistantAgent.create(
    deployment_name="optional-deployment-name",
    api_key="optional-api-key",
    endpoint="optional-endpoint",
    ad_token="optional-ad-token",
    ad_token_provider=optional_callable,
    default_headers={"optional_header": "optional-header-value"},
    env_file_path="optional-env-file-path",
    env_file_encoding="optional-env-file-encoding",
    ...,
)
```

### New Way

The agent provides a static method to create the required client for the specified resources, where method-level keyword arguments take precedence over environment variables and values in an existing `.env` file.

```python
client, model = AzureAssistantAgent.setup_resources(
    ad_token="optional-ad-token",
    ad_token_provider=optional_callable,
    api_key="optional-api-key",
    api_version="optional-api-version",
    base_url="optional-base-url",
    default_headers="optional-default-headers",
    deployment_name="optional-deployment-name",
    endpoint="optional-endpoint",
    env_file_path="optional-env-file-path",
    env_file_encoding="optional-env-file-encoding",
    token_scope="optional-token-scope",
)
```

## 1. Creating an Assistant

### Old Way
```python
agent = await AzureAssistantAgent.create(
    kernel=kernel,
    service_id=service_id,
    name=AGENT_NAME,
    instructions=AGENT_INSTRUCTIONS,
    enable_code_interpreter=True,
)
```
or
```python
agent = await OpenAIAssistantAgent.create(
    kernel=kernel,
    service_id=service_id,
    name=<name>,
    instructions=<instructions>,
    enable_code_interpreter=True,
)
```

### New Way
```python
# Azure AssistantAgent 

# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    instructions="<instructions>",
    name="<name>",
)

# Create the agent using the client and the assistant definition
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
)
```
or
```python
# OpenAI Assistant Agent

# Create the client using OpenAI resources and configuration
client, model = OpenAIAssistantAgent.setup_resources()

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    instructions="<instructions>",
    name="<name>",
)

# Create the agent using the client and the assistant definition
agent = OpenAIAssistantAgent(
    client=client,
    definition=definition,
)
```

## 2. Creating a Thread

### Old Way
```python
thread_id = await agent.create_thread()
```

### New Way
```python
from semantic_kernel.agents AssistantAgentThread, AzureAssistantAgent

client, model = AzureAssistantAgent.setup_resources()

# You may create a thread based on an existing thread id
# thread = AssistantAgentThread(client=client, thread_id="existing-thread-id")
# Otherwise, if not specified, a thread will be created during the first invocation
# and returned as part of the response
thread = None

async for response in agent.invoke(messages="user input", thread=thread):
    # handle response
    print(response)
    thread = response.thread
```

## 3. Handling Plugins

### Old Way
```python
# Create the instance of the Kernel
kernel = Kernel()

# Add the sample plugin to the kernel
kernel.add_plugin(plugin=MenuPlugin(), plugin_name="menu")

agent = await AzureAssistantAgent.create(
    kernel=kernel, 
    name="<name>", 
    instructions="<instructions>"
)
```
*Note: It is still possible to manage plugins via the kernel. If you do not supply a kernel, a kernel is automatically created at agent creation time and the plugins will be added to that instance.*

### New Way
```python
# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    instructions="<instructions>",
    name="<name>",
)

# Create the agent with plugins passed in as a list
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
    plugins=[MenuPlugin()],
)
```

Refer to the [sample implementation](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/getting_started_with_agents/openai_assistant/step2_plugins.py) for full details.

## 4. Using the Code Interpreter Tool

### Old Way
```python
csv_file_path = ...

agent = await AzureAssistantAgent.create(
    kernel=kernel,
    name="<name>",
    instructions="<instructions>",
    enable_code_interpreter=True,
    code_interpreter_filenames=[csv_file_path],
)
```

### New Way
```python
# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

csv_file_path = ...

# Load the CSV file as a FileObject
with open(csv_file_path, "rb") as file:
    file = await client.files.create(file=file, purpose="assistants")

# Get the code interpreter tool and resources
code_interpreter_tool, code_interpreter_tool_resource = AzureAssistantAgent.configure_code_interpreter_tool(file.id)

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    name="<name>",
    instructions="<instructions>.",
    tools=code_interpreter_tool,
    tool_resources=code_interpreter_tool_resource,
)

# Create the AzureAssistantAgent instance using the client and the assistant definition
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
)
```

Refer to the [sample implementation](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/openai_assistant/openai_assistant_file_manipulation.py) for full details.

## 5. Working with the File Search Tool

### Old Way
```python
pdf_file_path = ...

agent = await AzureAssistantAgent.create(
    kernel=kernel,
    service_id=service_id,
    name=AGENT_NAME,
    instructions=AGENT_INSTRUCTIONS,
    enable_file_search=True,
    vector_store_filenames=[pdf_file_path],
)
```

### New Way

```python
# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

pdf_file_path = ...

# Load the employees PDF file as a FileObject
with open(pdf_file_path, "rb") as file:
    file = await client.files.create(file=file, purpose="assistants")

# Create a vector store specifying the file ID to be used for file search
vector_store = await client.beta.vector_stores.create(
    name="step4_assistant_file_search",
    file_ids=[file.id],
)

file_search_tool, file_search_tool_resources = AzureAssistantAgent.configure_file_search_tool(vector_store.id)

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    instructions="Find answers to the user's questions in the provided file.",
    name="FileSearch",
    tools=file_search_tool,
    tool_resources=file_search_tool_resources,
)

# Create the AzureAssistantAgent instance using the client and the assistant definition
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
)
```

Refer to the [sample implementation](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/getting_started_with_agents/openai_assistant/step4_assistant_tool_file_search.py) for full details.

## 6. Adding Chat Messages to a Thread

### Old Way
```python
await agent.add_chat_message(
    thread_id=thread_id, 
    message=ChatMessageContent(role=AuthorRole.USER, content=user_input)
)
```

### New Way
*Note: The old method still works if you pass in a `ChatMessageContent`, but you can now also pass a simple string.*
```python
await agent.add_chat_message(
    thread_id=thread_id, 
    message=user_input,
)
```

## 7. Cleaning Up Resources

### Old Way
```python
await agent.delete_file(file_id)
await agent.delete_thread(thread_id)
await agent.delete()
```

### New Way
```python
await client.files.delete(file_id)
await thread.delete()
await client.beta.assistants.delete(agent.id)
```

## Handling Structured Outputs

### Old Way
*Unavailable in the old way*

### New Way
```python
# Define a Pydantic model that represents the structured output from the OpenAI service
class ResponseModel(BaseModel):
    response: str
    items: list[str]

# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    name="<name>",
    instructions="<instructions>",
    response_format=AzureAssistantAgent.configure_response_format(ResponseModel),
)

# Create the AzureAssistantAgent instance using the client and the assistant definition
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
)
```
Refer to the [sample implementation](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/openai_assistant/openai_assistant_structured_outputs.py) for full details.

This migration guide should help you update your code to the new implementation, leveraging client-based configuration and enhanced features.

::: zone-end
::: zone pivot="programming-language-java"
> Agents are unavailable in Java.
::: zone-end
