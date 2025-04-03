---
title: Exploring the Semantic Kernel ChatCompletionAgent
description: An exploration of the definition, behaviors, and usage patterns for a Chat Completion Agent
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# Exploring the Semantic Kernel `ChatCompletionAgent`

Detailed API documentation related to this discussion is available at:

::: zone pivot="programming-language-csharp"

- [`ChatCompletionAgent`](/dotnet/api/microsoft.semantickernel.agents.chatcompletionagent)
- [`Microsoft.SemanticKernel.Agents`](/dotnet/api/microsoft.semantickernel.agents)
- [`IChatCompletionService`](/dotnet/api/microsoft.semantickernel.chatcompletion.ichatcompletionservice)
- [`Microsoft.SemanticKernel.ChatCompletion`](/dotnet/api/microsoft.semantickernel.chatcompletion )

::: zone-end

::: zone pivot="programming-language-python"

- [`ChatCompletionAgent`](/python/api/semantic-kernel/semantic_kernel.agents.chat_completion.chat_completion_agent.chatcompletionagent)

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Chat Completion in Semantic Kernel

[Chat Completion](../../concepts/ai-services/chat-completion/index.md) is fundamentally a protocol for a chat-based interaction with an AI model where the chat-history is maintained and presented to the model with each request.  Semantic Kernel [AI services](../../concepts/ai-services/index.md) offer a unified framework for integrating the chat-completion capabilities of various AI models.

A `ChatCompletionAgent` can leverage any of these [AI services](../../concepts/ai-services/chat-completion/index.md) to generate responses, whether directed to a user or another agent.

## Preparing Your Development Environment

To proceed with developing an `ChatCompletionAgent`, configure your development environment with the appropriate packages.

::: zone pivot="programming-language-csharp"

Add the `Microsoft.SemanticKernel.Agents.Core` package to your project:

```pwsh
dotnet add package Microsoft.SemanticKernel.Agents.Core --prerelease
```

::: zone-end

::: zone pivot="programming-language-python"

Install the `semantic-kernel` package:

```bash
pip install semantic-kernel
```

> [!IMPORTANT]
> Depending upon which AI Service you use as part of the `ChatCompletionAgent`, you may need to install extra packages. Please check for the required extra on the following [page](../../concepts/ai-services/chat-completion/index.md#creating-a-chat-completion-service)


::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Creating a `ChatCompletionAgent`

A `ChatCompletionAgent` is fundamentally based on an [AI services](../../concepts/ai-services/index.md).  As such, creating a `ChatCompletionAgent` starts with creating a [`Kernel`](../../concepts/kernel.md) instance that contains one or more chat-completion services and then instantiating the agent with a reference to that [`Kernel`](../../concepts/kernel.md) instance.

::: zone pivot="programming-language-csharp"

```csharp
// Initialize a Kernel with a chat-completion service
IKernelBuilder builder = Kernel.CreateBuilder();

builder.AddAzureOpenAIChatCompletion(/*<...configuration parameters>*/);

Kernel kernel = builder.Build();

// Create the agent
ChatCompletionAgent agent =
    new()
    {
        Name = "SummarizationAgent",
        Instructions = "Summarize user input",
        Kernel = kernel
    };
```

::: zone-end

::: zone pivot="programming-language-python"
There are two ways to create a `ChatCompletionAgent`:

### 1. By providing the chat completion service directly

```python
from semantic_kernel.agents import ChatCompletionAgent

# Create the agent by directly providing the chat completion service
agent = ChatCompletionAgent(
    service=AzureChatCompletion(),  # your chat completion service instance
    name="<agent name>",
    instructions="<agent instructions>",
)
```

### 2. By creating a Kernel first, adding the service to it, then providing the kernel

```python
# Define the kernel
kernel = Kernel()

# Add the chat completion service to the kernel
kernel.add_service(AzureChatCompletion())

# Create the agent using the kernel
agent = ChatCompletionAgent(
  kernel=kernel, 
  name="<agent name>", 
  instructions="<agent instructions>",
)
```

The first method is useful when you already have a chat completion service ready. The second method is beneficial when you need a kernel that manages multiple services or additional functionalities.
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## AI Service Selection

No different from using Semantic Kernel [AI services](../../concepts/ai-services/index.md) directly, a `ChatCompletionAgent` supports the specification of a service-selector.  A service-selector identifies which [AI service](../../concepts/ai-services/index.md) to target when the [`Kernel`](../../concepts/kernel.md) contains more than one.

> Note: If multiple [AI services](../../concepts/ai-services/index.md) are present and no service-selector is provided, the same default logic is applied for the agent that you'd find when using an [AI services](../../concepts/ai-services/index.md) outside of the `Agent Framework`

::: zone pivot="programming-language-csharp"

```csharp
IKernelBuilder builder = Kernel.CreateBuilder();

// Initialize multiple chat-completion services.
builder.AddAzureOpenAIChatCompletion(/*<...service configuration>*/, serviceId: "service-1");
builder.AddAzureOpenAIChatCompletion(/*<...service configuration>*/, serviceId: "service-2");

Kernel kernel = builder.Build();

ChatCompletionAgent agent =
    new()
    {
        Name = "<agent name>",
        Instructions = "<agent instructions>",
        Kernel = kernel,
        Arguments = // Specify the service-identifier via the KernelArguments
          new KernelArguments(
            new OpenAIPromptExecutionSettings() 
            { 
              ServiceId = "service-2" // The target service-identifier.
            });
    };
```

::: zone-end

::: zone pivot="programming-language-python"

```python
from semantic_kernel.connectors.ai.open_ai import (
    AzureChatCompletion,
    AzureChatPromptExecutionSettings,
)

# Define the Kernel
kernel = Kernel()

# Add the AzureChatCompletion AI Service to the Kernel
kernel.add_service(AzureChatCompletion(service_id="service1"))
kernel.add_service(AzureChatCompletion(service_id="service2"))

settings = AzureChatPromptExecutionSettings(service_id="service2")

# Create the agent
agent = ChatCompletionAgent(
  kernel=kernel, 
  name="<agent name>", 
  instructions="<agent instructions>",
  arguments=KernelArguments(settings=settings)
)
```

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Conversing with `ChatCompletionAgent`

::: zone pivot="programming-language-csharp"

Conversing with your `ChatCompletionAgent` is based on a `ChatHistory` instance, no different from interacting with a Chat Completion [AI service](../../concepts/ai-services/index.md).

You can simply invoke the agent with your user message.

```csharp
// Define agent
ChatCompletionAgent agent = ...;

// Generate the agent response(s)
await foreach (ChatMessageContent response in agent.InvokeAsync(new ChatMessageContent(AuthorRole.User, "<user input>")))
{
  // Process agent response(s)...
}
```

You can also use an `AgentThread` to have a conversation with your agent.
Here we are using a `ChatHistoryAgentThread`.

The `ChatHistoryAgentThread` can also take an optional `ChatHistory`
object as input, via its constructor, if resuming a previous conversation. (not shown)

```csharp
// Define agent
ChatCompletionAgent agent = ...;

AgentThread thread = new ChatHistoryAgentThread();

// Generate the agent response(s)
await foreach (ChatMessageContent response in agent.InvokeAsync(new ChatMessageContent(AuthorRole.User, "<user input>"), thread))
{
  // Process agent response(s)...
}
```

::: zone-end

::: zone pivot="programming-language-python"

There are multiple ways to converse with a `ChatCompletionAgent`.

The easiest is to call and await `get_response`:

```python
# Define agent
agent = ChatCompletionAgent(...)

# Generate the agent response
response = await agent.get_response(messages="user input")
# response is an `AgentResponseItem[ChatMessageContent]` object
```

If you want the agent to maintain conversation history between invocations, you can pass it a `ChatHistoryAgentThread` as follows:

```python

# Define agent
agent = ChatCompletionAgent(...)

# Generate the agent response(s)
response = await agent.get_response(messages="user input")

# Generate another response, continuing the conversation thread from the first response.
response2 = await agent.get_response(messages="user input", thread=response.thread)
# process agent response(s)

```

Calling the `invoke` method returns an `AsyncIterable` of `AgentResponseItem[ChatMessageContent]`.

```python
# Define agent
agent = ChatCompletionAgent(...)
  
# Define the thread
thread = ChatHistoryAgentThread()

# Generate the agent response(s)
async for response in agent.invoke(messages="user input", thread=thread):
  # process agent response(s)
```

The `ChatCompletionAgent` also supports streaming in which the `invoke_stream` method returns an `AsyncIterable` of `StreamingChatMessageContent`:

```python
# Define agent
agent = ChatCompletionAgent(...)
  
# Define the thread
thread = ChatHistoryAgentThread()

# Generate the agent response(s)
async for response in agent.invoke_stream(messages="user input", thread=thread):
  # process agent response(s)
```

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Handling Intermediate Messages with a `ChatCompletionAgent`

The Semantic Kernel `ChatCompletionAgent` is designed to invoke an agent that fulfills user queries or questions. During invocation, the agent may execute tools to derive the final answer. To access intermediate messages produced during this process, callers can supply a callback function that handles instances of `FunctionCallContent` or `FunctionResultContent`.

::: zone pivot="programming-language-csharp"
> Callback documentation for the `ChatCompletionAgent` is coming soon.
::: zone-end

::: zone pivot="programming-language-python"

Configuring the `on_intermediate_message` callback within `agent.invoke(...)` or `agent.invoke_stream(...)` allows the caller to receive intermediate messages generated during the process of formulating the agent's final response.

```python
import asyncio
from typing import Annotated

from semantic_kernel.agents import ChatCompletionAgent
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
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
  # Create the ChatCompletionAgent instance
  agent = ChatCompletionAgent(
      service=AzureChatCompletion(),
      instructions="your instructions",
      name="name",
      plugins=[MenuPlugin()],
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

### How-To

For an end-to-end example for a `ChatCompletionAgent`, see:

- [How-To: `ChatCompletionAgent`](./examples/example-chat-agent.md)

> [!div class="nextstepaction"]
> [Exploring the OpenAI Assistant Agent](./assistant-agent.md)
