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
# Exploring the Semantic Kernel `OpenAIAssistantAgent`

> [!IMPORTANT]
> This feature is in the release candidate stage. Features at this stage are nearly complete and generally stable, though they may undergo minor refinements or optimizations before reaching full general availability.

Detailed API documentation related to this discussion is available at:

::: zone pivot="programming-language-csharp"
- [`OpenAIAssistantAgent`](/dotnet/api/microsoft.semantickernel.agents.openai.openaiassistantagent)

::: zone-end

::: zone pivot="programming-language-python"

- [`AzureAssistantAgent`](/python/api/semantic-kernel/semantic_kernel.agents.open_ai.azure_assistant_agent.azureassistantagent)
- [`OpenAIAssistantAgent`](/python/api/semantic-kernel/semantic_kernel.agents.open_ai.open_ai_assistant_agent.openaiassistantagent)

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## What is an Assistant?

The _OpenAI Assistant API_ is a specialized interface designed for more advanced and interactive AI capabilities, enabling developers to create personalized and multi-step task-oriented agents. Unlike the Chat Completion API, which focuses on simple conversational exchanges, the Assistant API allows for dynamic, goal-driven interactions with additional features like code-interpreter and file-search.

- [OpenAI Assistant Guide](https://platform.openai.com/docs/assistants)
- [OpenAI Assistant API](https://platform.openai.com/docs/api-reference/assistants)
- [Assistant API in Azure](/azure/ai-services/openai/assistants-quickstart)


## Preparing Your Development Environment

To proceed with developing an `OpenAIAIAssistantAgent`, configure your development environment with the appropriate packages.

::: zone pivot="programming-language-csharp"

Add the `Microsoft.SemanticKernel.Agents.OpenAI` package to your project:

```pwsh
dotnet add package Microsoft.SemanticKernel.Agents.AzureAI --prerelease
```

You may also want to include the `Azure.Identity` package:

```pwsh
dotnet add package Azure.Identity
```
::: zone-end

::: zone pivot="programming-language-python"

Install the `semantic-kernel` package with the optional _Azure_ dependencies:

```bash
pip install semantic-kernel[azure]
```

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Creating an `OpenAIAssistantAgent`

Creating an `OpenAIAssistant` requires invoking a remote service, which is handled asynchronously. To manage this, the `OpenAIAssistantAgent` is instantiated through a static factory method, ensuring the process occurs in a non-blocking manner. This method abstracts the complexity of the asynchronous call, returning a promise or future once the assistant is fully initialized and ready for use.

::: zone pivot="programming-language-csharp"
```csharp
AssistantClient client = OpenAIAssistantAgent.CreateAzureOpenAIClient(...).GetAssistantClient();
Assistant assistant =
    await this.AssistantClient.CreateAssistantAsync(
        "<model name>",
        "<agent name>",
        instructions: "<agent instructions>");
OpenAIAssistantAgent agent = new(assistant, client);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
from semantic_kernel.agents import AssistantAgentThread, AzureAssistantAgent, OpenAIAssistantAgent

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
AssistantClient client = OpenAIAssistantAgent.CreateAzureOpenAIClient(...).GetAssistantClient();
Assistant assistant = await this.AssistantClient.GetAssistantAsync("<assistant id>");
OpenAIAssistantAgent agent = new(assistant, client);
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

As with all aspects of the _Assistant API_, conversations are stored remotely. Each conversation is referred to as a _thread_ and identified by a unique `string` identifier. Interactions with your `OpenAIAssistantAgent` are tied to this specific thread identifier. The specifics of the _Assistant API thread_ is abstracted away via the `OpenAIAssistantAgentThread` class, which is an implementation of `AgentThread`.

The `OpenAIAssistantAgent` currently only supports threads of type `OpenAIAssistantAgentThread`.

You can invoke the `OpenAIAssistantAgent` without specifying an `AgentThread`, to start a new thread and a new `AgentThread` will be returned as part of the response.

::: zone pivot="programming-language-csharp"
```csharp

// Define agent
OpenAIAssistantAgent agent = ...;
AgentThread? agentThread = null;

// Generate the agent response(s)
await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(new ChatMessageContent(AuthorRole.User, "<user input>")))
{
  // Process agent response(s)...
  agentThread = response.Thread;
}

// Delete the thread if no longer needed
if (agentThread is not null)
{
    await agentThread.DeleteAsync();
}
```

You can also invoke the `OpenAIAssistantAgent` with an `AgentThread` that you created.

```csharp
// Define agent
OpenAIAssistantAgent agent = ...;

// Create a thread with some custom metadata.
AgentThread agentThread = new OpenAIAssistantAgentThread(this.AssistantClient, metadata: myMetadata);

// Generate the agent response(s)
await foreach (ChatMessageContent response in agent.InvokeAsync(new ChatMessageContent(AuthorRole.User, "<user input>"), agentThread))
{
  // Process agent response(s)...
}

// Delete the thread when it is no longer needed
await agentThread.DeleteAsync();
```

You can also create an `OpenAIAssistantAgentThread` that resumes an earlier conversation by id.

```csharp
// Create a thread with an existing thread id.
AgentThread agentThread = new OpenAIAssistantAgentThread(this.AssistantClient, "existing-thread-id");
```

::: zone-end

::: zone pivot="programming-language-python"
```python
from semantic_kernel.agents import AssistantAgentThread, AzureAssistantAgent

# Define agent
openai_agent = await ...

# Create a thread for the agent conversation
thread: AssistantAgentThread = None

# Generate the agent response(s)
async for response in agent.invoke(messages="user input", thread=thread):
  # process agent response(s)...
  thread = response.thread

# Delete the thread when it is no longer needed
await thread.delete() if thread else None
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Deleting an `OpenAIAssistantAgent`

Since the assistant's definition is stored remotely, it will persist if not deleted.  
Deleting an assistant definition may be performed directly with the `AssistantClient`.

> Note: Attempting to use an agent instance after being deleted will result in a service exception.

::: zone pivot="programming-language-csharp"

For .NET, the agent identifier is exposed as a `string` via the [`Agent.Id`](/dotnet/api/microsoft.semantickernel.agents.agent.id) property defined by any agent.

```csharp
AssistantClient client = OpenAIAssistantAgent.CreateAzureOpenAIClient(...).GetAssistantClient();
Assistant assistant = await this.AssistantClient.DeleteAssistantAsync("<assistant id>");
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
> [Exploring the Azure AI Agent](./azure-ai-agent.md)

