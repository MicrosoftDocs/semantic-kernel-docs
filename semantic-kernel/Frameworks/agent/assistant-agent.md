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
> Single-agent features, such as `OpenAIAssistantAgent`, are in the release candidate stage. These features are nearly complete and generally stable, though they may undergo minor refinements or optimizations before reaching full general availability.

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

The OpenAI Assistants API is a specialized interface designed for more advanced and interactive AI capabilities, enabling developers to create personalized and multi-step task-oriented agents. Unlike the Chat Completion API, which focuses on simple conversational exchanges, the Assistant API allows for dynamic, goal-driven interactions with additional features like code-interpreter and file-search.

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

Install the `semantic-kernel` package with the optional Azure dependencies:

```bash
pip install semantic-kernel[azure]
```

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Creating an `OpenAIAssistantAgent`

Creating an `OpenAIAssistant` requires first creating a client to be able to talk a remote service.

::: zone pivot="programming-language-csharp"
```csharp
AssistantClient client = OpenAIAssistantAgent.CreateAzureOpenAIClient(...).GetAssistantClient();
Assistant assistant =
    await client.CreateAssistantAsync(
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
Assistant assistant = await client.GetAssistantAsync("<assistant id>");
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

As with all aspects of the Assistant API, conversations are stored remotely. Each conversation is referred to as a thread and identified by a unique `string` identifier. Interactions with your `OpenAIAssistantAgent` are tied to this specific thread identifier. The specifics of the Assistant API thread is abstracted away via the `OpenAIAssistantAgentThread` class, which is an implementation of `AgentThread`.

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
AgentThread agentThread = new OpenAIAssistantAgentThread(client, metadata: myMetadata);

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
AgentThread agentThread = new OpenAIAssistantAgentThread(client, "existing-thread-id");
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
await client.DeleteAssistantAsync("<assistant id>");
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

## Handling Intermediate Messages with an `OpenAIAssistantAgent`

The Semantic Kernel `OpenAIAssistantAgent` is designed to invoke an agent that fulfills user queries or questions. During invocation, the agent may execute tools to derive the final answer. To access intermediate messages produced during this process, callers can supply a callback function that handles instances of `FunctionCallContent` or `FunctionResultContent`.

::: zone pivot="programming-language-csharp"
> Callback documentation for the `OpenAIAssistantAgent` is coming soon.
::: zone-end

::: zone pivot="programming-language-python"

Configuring the `on_intermediate_message` callback within `agent.invoke(...)` or `agent.invoke_stream(...)` allows the caller to receive intermediate messages generated during the process of formulating the agent's final response.

```python
import asyncio
from typing import Annotated

from semantic_kernel.agents import AzureAssistantAgent
from semantic_kernel.contents import AuthorRole, ChatMessageContent, FunctionCallContent, FunctionResultContent
from semantic_kernel.functions import kernel_function

class MenuPlugin:
    """A sample Menu Plugin used for the concept sample."""

    @kernel_function(description="Provides a list of specials from the menu.")
    def get_specials(self) -> Annotated[str, "Returns the specials from the menu."]:
        return """
        Special Soup: Clam Chowder
        Special Salad: Cobb Salad
        Special Drink: Chai Tea
        """

    @kernel_function(description="Provides the price of the requested menu item.")
    def get_item_price(
        self, menu_item: Annotated[str, "The name of the menu item."]
    ) -> Annotated[str, "Returns the price of the menu item."]:
        return "$9.99"

# Define a list to hold callback message content
intermediate_steps: list[ChatMessageContent] = []

# Define an async method to handle the `on_intermediate_message` callback
async def handle_intermediate_steps(message: ChatMessageContent) -> None:
    intermediate_steps.append(message)

async def main():
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
        plugins=[MenuPlugin()]
    )

    user_inputs = [
        "Hello", 
        "What is the special soup?", 
        "What is the special drink?", 
        "How much is that?", 
        "Thank you",
    ]

    thread = None

    # Generate the agent response(s)
    for user_input in user_inputs:
        print(f"# {AuthorRole.USER}: '{user_input}'")
        async for response in agent.invoke(
            messages=user_input,
            thread=thread,
            on_intermediate_message=handle_intermediate_steps,
        ):
            thread = response.thread
            print(f"# {response.name}: {response.content}")

    # Delete the thread when it is no longer needed
    await thread.delete() if thread else None

    # Print the intermediate steps
    print("\nIntermediate Steps:")
    for msg in intermediate_steps:
        if any(isinstance(item, FunctionResultContent) for item in msg.items):
            for fr in msg.items:
                if isinstance(fr, FunctionResultContent):
                    print(f"Function Result:> {fr.result} for function: {fr.name}")
        elif any(isinstance(item, FunctionCallContent) for item in msg.items):
            for fcc in msg.items:
                if isinstance(fcc, FunctionCallContent):
                    print(f"Function Call:> {fcc.name} with arguments: {fcc.arguments}")
        else:
            print(f"{msg.role}: {msg.content}")

if __name__ == "__main__":
    asyncio.run(main())
```

The following demonstrates sample output from the agent invocation process:

```bash
Sample Output:

# AuthorRole.USER: 'Hello'
# Host: Hi there! How can I assist you with the menu today?
# AuthorRole.USER: 'What is the special soup?'
# Host: The special soup is Clam Chowder.
# AuthorRole.USER: 'What is the special drink?'
# Host: The special drink is Chai Tea.
# AuthorRole.USER: 'How much is that?'
# Host: Could you please specify the menu item you are asking about?
# AuthorRole.USER: 'Thank you'
# Host: You're welcome! If you have any questions about the menu or need assistance, feel free to ask.

Intermediate Steps:
AuthorRole.ASSISTANT: Hi there! How can I assist you with the menu today?
AuthorRole.ASSISTANT: 
Function Result:> 
        Special Soup: Clam Chowder
        Special Salad: Cobb Salad
        Special Drink: Chai Tea
        for function: MenuPlugin-get_specials
AuthorRole.ASSISTANT: The special soup is Clam Chowder.
AuthorRole.ASSISTANT: 
Function Result:> 
        Special Soup: Clam Chowder
        Special Salad: Cobb Salad
        Special Drink: Chai Tea
        for function: MenuPlugin-get_specials
AuthorRole.ASSISTANT: The special drink is Chai Tea.
AuthorRole.ASSISTANT: Could you please specify the menu item you are asking about?
AuthorRole.ASSISTANT: You're welcome! If you have any questions about the menu or need assistance, feel free to ask.
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
> [Explore the Azure AI Agent](./azure-ai-agent.md)

