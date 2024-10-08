---
title: Exploring the Semantic Kernel Open AI Assistant Agent (Experimental)
description: An exploration of the definition, behaviors, and usage patterns for a `OpenAIAssistantAgent`
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# Exploring the _Semantic Kernel_ _Open AI Assistant Agent_ (Experimental)

> [!WARNING] 
> The _Semantic Kernel Agent Framework_ is experimental, still in development and is subject to change.

Detailed API documentation related to this discussion is available at:

::: zone pivot="programming-language-csharp"
- [`OpenAIAssistantAgent`](/dotnet/api/microsoft.semantickernel.agents.openai.openaiassistantagent)
- [`OpenAIAssistantDefinition`](/dotnet/api/microsoft.semantickernel.agents.openai.openaiassistantdefinition)
- [`OpenAIClientProvider`](/dotnet/api/microsoft.semantickernel.agents.openai.openaiclientprovider)

::: zone-end

::: zone pivot="programming-language-python"

- [`open_ai_assistant_base`](/python/api/semantic-kernel/semantic_kernel.agents.open_ai.open_ai_assistant_base)
- [`azure_assistant_agent`](/python/api/semantic-kernel/semantic_kernel.agents.open_ai.azure_assistant_agent)
- [`open_ai_assistant_agent`](/python/api/semantic-kernel/semantic_kernel.agents.open_ai.open_ai_assistant_agent)

::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## What is an Assistant?

The _OpenAI Assistant API_ is a specialized interface designed for more advanced and interactive AI capabilities, enabling developers to create personalized and multi-step task-oriented agents. Unlike the Chat Completion API, which focuses on simple conversational exchanges, the Assistant API allows for dynamic, goal-driven interactions with additional features like code-interpreter and file-search.

- [Open AI Assistant Guide](https://platform.openai.com/docs/assistants)
- [Open AI Assistant API](https://platform.openai.com/docs/api-reference/assistants)
- [Assistant API in Azure](/azure/ai-services/openai/assistants-quickstart)


## Creating an _Open AI Assistant Agent_

Creating an _Open AI Assistant_ requires invoking a remote service, which is handled asynchronously. To manage this, the _Open AI Assistant Agent_ is instantiated through a static factory method, ensuring the process occurs in a non-blocking manner. This method abstracts the complexity of the asynchronous call, returning a promise or future once the assistant is fully initialized and ready for use.

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
azure_agent = await AzureAssistantAgent.create(
    kernel=kernel, 
    service_id=service_id, 
    name="<agent name>", 
    instructions="<agent instructions>"
)

# or

openai_agent = await OpenAIAssistantAgent.create(
    kernel=kernel, 
    service_id=service_id, 
    name="<agent name>", 
    instructions="<agent instructions>"
)
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## Retrieving an _Open AI Assistant Agent_

Once created, the identifier of the assistant may be access via its identifier.  This identifier may be used to create an _Open AI Assistant Agent_ from an existing assistant definition.

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
agent = await OpenAIAssistantAgent.retrieve(id=agent_id, kernel=kernel)

# or

agent = await AzureAssistantAgent.retrieve(id=agent_id, kernel=kernel)
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## Using an _Open AI Assistant Agent_

As with all aspects of the _Assistant API_, conversations are stored remotely. Each conversation is referred to as a _thread_ and identified by a unique `string` identifier. Interactions with your _OpenAI Assistant Agent_ are tied to this specific thread identifier which must be specified when calling the agent/

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
::: zone-end


## Deleting an _Open AI Assistant Agent_

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
::: zone-end


## How-To

For an end-to-end example for a _Open AI Assistant Agent_, see:

- [How-To: _Open AI Assistant Agent_ Code Interpreter](./examples/example-assistant-code.md)
- [How-To: _Open AI Assistant Agent_ File Search](./examples/example-assistant-search.md)


> [!div class="nextstepaction"]
> [Agent Collaboration in _Agent Chat_](./agent-chat.md)

