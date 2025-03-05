---
title: Exploring the Semantic Kernel OpenAI Assistant Agent
description: An exploration of the definition, behaviors, and usage patterns for a `OpenAIAssistantAgent`
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# Exploring the _Semantic Kernel_ `OpenAIAssistantAgent`

> [!IMPORTANT]
> This feature is in the release candidate stage. Features at this stage are nearly complete and generally stable, though they may undergo minor refinements or optimizations before reaching full general availability.

Detailed API documentation related to this discussion is available at:

::: zone pivot="programming-language-csharp"
- [`OpenAIAssistantAgent`](/dotnet/api/microsoft.semantickernel.agents.openai.openaiassistantagent)
- [`OpenAIAssistantDefinition`](/dotnet/api/microsoft.semantickernel.agents.openai.openaiassistantdefinition)
- [`OpenAIClientProvider`](/dotnet/api/microsoft.semantickernel.agents.openai.openaiclientprovider)

::: zone-end

::: zone pivot="programming-language-python"

- [`azure_assistant_agent`](/python/api/semantic-kernel/semantic_kernel.agents.open_ai.azure_assistant_agent)
- [`open_ai_assistant_agent`](/python/api/semantic-kernel/semantic_kernel.agents.open_ai.open_ai_assistant_agent)

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## What is an Assistant?

The _OpenAI Assistant API_ is a specialized interface designed for more advanced and interactive AI capabilities, enabling developers to create personalized and multi-step task-oriented agents. Unlike the Chat Completion API, which focuses on simple conversational exchanges, the Assistant API allows for dynamic, goal-driven interactions with additional features like code-interpreter and file-search.

- [OpenAI Assistant Guide](https://platform.openai.com/docs/assistants)
- [OpenAI Assistant API](https://platform.openai.com/docs/api-reference/assistants)
- [Assistant API in Azure](/azure/ai-services/openai/assistants-quickstart)


## Creating an `OpenAIAssistantAgent`

Creating an `OpenAIAssistant` requires invoking a remote service, which is handled asynchronously. To manage this, the `OpenAIAssistantAgent` is instantiated through a static factory method, ensuring the process occurs in a non-blocking manner. This method abstracts the complexity of the asynchronous call, returning a promise or future once the assistant is fully initialized and ready for use.

::: zone pivot="programming-language-csharp"
```csharp
OpenAIAssistantAgent agent =
    await OpenAIAssistantAgent.CreateAsync(
        OpenAIClientProvider.ForAzureOpenAI(/*<...service configuration>*/),
        new OpenAIAssistantDefinition("<model name>")
        {
          Name = "<agent name>",
          Instructions = "<agent instructions>",
        },
        new Kernel());
```
::: zone-end

::: zone pivot="programming-language-python"
```python
from semantic_kernel.agents.open_ai import AzureAssistantAgent, OpenAIAssistantAgent

# Set up the client and model using Azure OpenAI Resources
client, model = AzureAssistantAgent.setup_resources()

# Define the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    instructions="<instructions>",
    name="<agent name>",
)

# Create the AzureAssistantAgent instance using the client and the assistant definition
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
)

# or

# Set up the client and model using OpenAI Resources
client, model = OpenAIAssistantAgent.setup_resources()

# Define the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    instructions="<instructions>",
    name="<agent name>",
)

# Create the OpenAIAssistantAgent instance using the client and the assistant definition
agent = OpenAIAssistantAgent(
    client=client,
    definition=definition,
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Retrieving an `OpenAIAssistantAgent`

Once created, the identifier of the assistant may be access via its identifier.  This identifier may be used to create an `OpenAIAssistantAgent` from an existing assistant definition.

::: zone pivot="programming-language-csharp"

For .NET, the agent identifier is exposed as a `string` via the  property defined by any agent.

```csharp
OpenAIAssistantAgent agent =
    await OpenAIAssistantAgent.RetrieveAsync(
        OpenAIClientProvider.ForAzureOpenAI(/*<...service configuration>*/),
        "<your agent id>",
        new Kernel());
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Using Azure OpenAI Resources

# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    name="<agent name>",
    instructions="<instructions>",
)

# Store the assistant ID
assistant_id = definition.id

# Retrieve the assistant definition from the server based on the assistant ID
new_asst_definition = await client.beta.assistants.retrieve(assistant_id)

# Create the AzureAssistantAgent instance using the client and the assistant definition
agent = AzureAssistantAgent(
    client=client,
    definition=new_asst_definition,
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Using an `OpenAIAssistantAgent`

As with all aspects of the _Assistant API_, conversations are stored remotely. Each conversation is referred to as a _thread_ and identified by a unique `string` identifier. Interactions with your `OpenAIAssistantAgent` are tied to this specific thread identifier which must be specified when calling the agent/

::: zone pivot="programming-language-csharp"
```csharp
// Define agent
OpenAIAssistantAgent agent = ...;

// Create a thread for the agent conversation.
string threadId = await agent.CreateThreadAsync();

// Add a user message to the conversation
chat.Add(threadId, new ChatMessageContent(AuthorRole.User, "<user input>"));

// Generate the agent response(s)
await foreach (ChatMessageContent response in agent.InvokeAsync(threadId))
{
  // Process agent response(s)...
}

// Delete the thread when it is no longer needed
await agent.DeleteThreadAsync(threadId);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Define agent
openai_agent = await ...

# Create a thread for the agent conversation
thread_id = await agent.create_thread()

# Add a user message to the conversation
await agent.add_chat_message(
  thread_id=thread_id, 
  message=ChatMessageContent(role=AuthorRole.USER, content="<user input>"),
)

# Generate the agent response(s)
async for response in agent.invoke(thread_id=thread_id):
  # process agent response(s)...

# Delete the thread when it is no longer needed
await agent.delete_thread(thread_id)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Deleting an `OpenAIAssistantAgent`

Since the assistant's definition is stored remotely, it supports the capability to self-delete. This enables the agent to be removed from the system when it is no longer needed.

> Note: Attempting to use an agent instance after being deleted results in an exception.

::: zone pivot="programming-language-csharp"

For .NET, the agent identifier is exposed as a `string` via the [`Agent.Id`](/dotnet/api/microsoft.semantickernel.agents.agent.id) property defined by any agent.

```csharp
// Perform the deletion
await agent.DeleteAsync();

// Inspect whether an agent has been deleted
bool isDeleted = agent.IsDeleted();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
await agent.delete()

is_deleted = agent._is_deleted
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## How-To

For an end-to-end example for a `OpenAIAssistantAgent`, see:

- [How-To: `OpenAIAssistantAgent` Code Interpreter](./examples/example-assistant-code.md)
- [How-To: `OpenAIAssistantAgent` File Search](./examples/example-assistant-search.md)


> [!div class="nextstepaction"]
> [Agent Collaboration in `AgentChat`](./agent-chat.md)

