---
title: Exploring the Semantic Kernel Chat Completion Agent (Experimental)
description: An exploration of the definition, behaviors, and usage patterns for a Chat Completion Agent
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# Exploring the _Semantic Kernel_ Chat Completion Agent

> [!WARNING]
> The *Semantic Kernel Agent Framework* is in preview and is subject to change.

Detailed API documentation related to this discussion is available at:

::: zone pivot="programming-language-csharp"
- [`ChatCompletionAgent`](/dotnet/api/microsoft.semantickernel.agents.chatcompletionagent)
- [`Microsoft.SemanticKernel.Agents`](/dotnet/api/microsoft.semantickernel.agents)
- [`IChatCompletionService`](/dotnet/api/microsoft.semantickernel.chatcompletion.ichatcompletionservice)
- [`Microsoft.SemanticKernel.ChatCompletion`](/dotnet/api/microsoft.semantickernel.chatcompletion )

::: zone-end

::: zone pivot="programming-language-python"

- [`chat_completion_agent`](/python/api/semantic-kernel/semantic_kernel.agents.chat_completion.chat_completion_agent)
- [`chat_completion_client_base`](/python/api/semantic-kernel/semantic_kernel.connectors.ai.chat_completion_client_base)

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Chat Completion in _Semantic Kernel_

[_Chat Completion_](../../concepts/ai-services/chat-completion/index.md) is fundamentally a protocol for a chat-based interaction with an AI model where the chat-history maintained and presented to the model with each request.  _Semantic Kernel_ [AI services](../../concepts/ai-services/index.md) offer a unified framework for integrating the chat-completion capabilities of various AI models.

A _chat completion agent_ can leverage any of these [AI services](../../concepts/ai-services/index.md) to generate responses, whether directed to a user or another agent.

::: zone pivot="programming-language-csharp"

For .NET, _chat-completion_ AI Services are based on the [`IChatCompletionService`](/dotnet/api/microsoft.semantickernel.chatcompletion.ichatcompletionservice) interface.

For .NET, some of AI services that support models with chat-completion include:

Model|_Semantic Kernel_ AI Service
--|--
Azure OpenAI|[`Microsoft.SemanticKernel.Connectors.AzureOpenAI`](/dotnet/api/microsoft.semantickernel.connectors.azureopenai)
Gemini|[`Microsoft.SemanticKernel.Connectors.Google`](/dotnet/api/microsoft.semantickernel.connectors.google)
HuggingFace|[`Microsoft.SemanticKernel.Connectors.HuggingFace`](/dotnet/api/microsoft.semantickernel.connectors.huggingface)
Mistral|[`Microsoft.SemanticKernel.Connectors.MistralAI`](/dotnet/api/microsoft.semantickernel.connectors.mistralai)
OpenAI|[`Microsoft.SemanticKernel.Connectors.OpenAI`](/dotnet/api/microsoft.semantickernel.connectors.openai)
Onnx|[`Microsoft.SemanticKernel.Connectors.Onnx`](/dotnet/api/microsoft.semantickernel.connectors.onnx)

::: zone-end

::: zone pivot="programming-language-python"

- [`azure_chat_completion`](/python/api/semantic-kernel/semantic_kernel.connectors.ai.open_ai.services.azure_chat_completion)
- [`open_ai_chat_completion`](/python/api/semantic-kernel/semantic_kernel.connectors.ai.open_ai.services.open_ai_chat_completion)

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Creating a Chat Completion Agent

A _chat completion agent_ is fundamentally based on an [AI services](../../concepts/ai-services/index.md).  As such, creating an _chat completion agent_ starts with creating a [_Kernel_](../../concepts/kernel.md) instance that contains one or more chat-completion services and then instantiating the agent with a reference to that [_Kernel_](../../concepts/kernel.md) instance.

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
```python
# Define the Kernel
kernel = Kernel()

# Add the AzureChatCompletion AI Service to the Kernel
kernel.add_service(AzureChatCompletion(service_id="<service_id>"))

# Create the agent
agent = ChatCompletionAgent(
  service_id="agent", 
  kernel=kernel, 
  name="<agent name>", 
  instructions="<agent instructions>",
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## AI Service Selection

No different from using _Semantic Kernel_ [AI services](../../concepts/ai-services/index.md) directly, a _chat completion agent_ support the specification of a _service-selector_.  A _service-selector_ indentifies which [AI service](../../concepts/ai-services/index.md) to target when the [_Kernel_](../../concepts/kernel.md) contains more than one.

> Note: If multiple [AI services](../../concepts/ai-services/index.md) are present and no _service-selector_ is provided, the same _default_ logic is applied for the agent that you'd find when using an [AI services](../../concepts/ai-services/index.md) outside of the _Agent Framework_

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
# Define the Kernel
kernel = Kernel()

# Add the AzureChatCompletion AI Service to the Kernel
kernel.add_service(AzureChatCompletion(service_id="<service_id>"))

# Create the agent
agent = ChatCompletionAgent(
  service_id="agent", 
  kernel=kernel, 
  name="<agent name>", 
  instructions="<agent instructions>",
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Conversing with _Chat Completion Agent_

Conversing with your _Chat Completion Agent_ is based on a _Chat History_ instance, no different from interacting with a _Chat Completion_ [AI service](../../concepts/ai-services/index.md).

::: zone pivot="programming-language-csharp"
```csharp
// Define agent
ChatCompletionAgent agent = ...;

// Create a ChatHistory object to maintain the conversation state.
ChatHistory chat = [];

// Add a user message to the conversation
chat.Add(new ChatMessageContent(AuthorRole.User, "<user input>"));

// Generate the agent response(s)
await foreach (ChatMessageContent response in agent.InvokeAsync(chat))
{
  // Process agent response(s)...
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Define agent
agent = ChatCompletionAgent(...)
  
# Define the chat history
chat = ChatHistory()

# Add the user message
chat.add_message(ChatMessageContent(role=AuthorRole.USER, content=input))

# Generate the agent response(s)
async for response in agent.invoke(chat):
  # process agent response(s)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


#### How-To:

For an end-to-end example for a _Chat Completion Agent_, see:

- [How-To: _Chat Completion Agent_](./examples/example-chat-agent.md)


> [!div class="nextstepaction"]
> [Exploring _OpenAI Assistant Agent_](./assistant-agent.md)
