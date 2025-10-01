---
title: Semantic Kernel to Microsoft Agent Framework Migration Guide
description: Learn how to migrate from the Semantic Kernel Agent Framework to Microsoft Agent Framework
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: reference
ms.author: westey
ms.date: 09/25/2025
ms.service: agent-framework
---

# Semantic Kernel to Agent Framework Migration Guide

## Benefits of Microsoft Agent Framework compared to Semantic Kernel Agent Framework

- **Simplified API**: Reduced complexity and boilerplate code
- **Better Performance**: Optimized object creation and memory usage
- **Unified Interface**: Consistent patterns across different AI providers
- **Enhanced Developer Experience**: More intuitive and discoverable APIs

::: zone pivot="programming-language-csharp"

## Key differences

Here is a summary of the key differences between the Semantic Kernel Agent Framework and the Microsoft Agent Framework to help you migrate your code.

### 1. Namespace Updates

#### Semantic Kernel

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
```

#### Agent Framework

Agent Framework namespaces are under `Microsoft.Agents.AI`.
Agent Framework uses the core AI message and content types from `Microsoft.Extensions.AI` for communication between components.

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
```

### 2. Agent Creation Simplification

#### Semantic Kernel

Every agent in Semantic Kernel depends on a `Kernel` instance and will have
an empty `Kernel` if not provided.

```csharp
 Kernel kernel = Kernel
    .AddOpenAIChatClient(modelId, apiKey)
    .Build();

 ChatCompletionAgent agent = new() { Instructions = ParrotInstructions, Kernel = kernel };
```

Azure AI Foundry requires an agent resource to be created in the cloud before creating a local agent class that uses it.

```csharp
PersistentAgentsClient azureAgentClient = AzureAIAgent.CreateAgentsClient(azureEndpoint, new AzureCliCredential());

PersistentAgent definition = await azureAgentClient.Administration.CreateAgentAsync(
    deploymentName,
    instructions: ParrotInstructions);

AzureAIAgent agent = new(definition, azureAgentClient);
 ```

#### Agent Framework

Agent creation in Agent Framework is made simpler with extensions provided by all main providers.

```csharp
AIAgent openAIAgent = chatClient.CreateAIAgent(instructions: ParrotInstructions);
AIAgent azureFoundryAgent = await persistentAgentsClient.CreateAIAgentAsync(instructions: ParrotInstructions);
AIAgent openAIAssistantAgent = await assistantClient.CreateAIAgentAsync(instructions: ParrotInstructions);
```

Additionally for hosted agent providers you can also use the `GetAIAgent` to retrieve an agent from an existing hosted agent.

```csharp
AIAgent azureFoundryAgent = await persistentAgentsClient.GetAIAgentAsync(agentId);
```

### 3. Agent Thread Creation

#### Semantic Kernel

The caller has to know the thread type and create it manually.

```csharp
// Create a thread for the agent conversation.
AgentThread thread = new OpenAIAssistantAgentThread(this.AssistantClient);
AgentThread thread = new AzureAIAgentThread(this.Client);
AgentThread thread = new OpenAIResponseAgentThread(this.Client);
```

#### Agent Framework

The agent is responsible for creating the thread.

```csharp
// New
AgentThread thread = agent.GetNewThread();
```

### 4. Hosted Agent Thread Cleanup

This case applies exclusively to a few AI providers that still provide hosted threads.

#### Semantic Kernel

Threads have a `self` deletion method

i.e: OpenAI Assistants Provider
```csharp
await thread.DeleteAsync();
```

#### Agent Framework

> [!NOTE]
> OpenAI Responses introduced a new conversation model that simplifies how conversations are handled. This simplifies hosted thread management compared to the now deprecated OpenAI Assistants model. For more information see the [OpenAI Assistants migration guide](https://platform.openai.com/docs/assistants/migration).

Agent Framework doesn't have a thread deletion API in the `AgentThread` type as not all providers support hosted threads or thread deletion and this will become more common as more providers shift to responses based architectures.

If you require thread deletion and the provider allows this, the caller **should** keep track of the created threads and delete them later when necessary via the provider's sdk.

i.e: OpenAI Assistants Provider
```csharp
await assistantClient.DeleteThreadAsync(thread.ConversationId);
```

### 5. Tool Registration

#### Semantic Kernel

In semantic kernel to expose a function as a tool you must:

1. Decorate the function with a `[KernelFunction]` attribute.
2. Have a `Plugin` class or use the `KernelPluginFactory` to wrap the function.
3. Have a `Kernel` to add your plugin to.
4. Pass the `Kernel` to the agent.

```csharp
KernelFunction function = KernelFunctionFactory.CreateFromMethod(GetWeather);
KernelPlugin plugin = KernelPluginFactory.CreateFromFunctions("KernelPluginName", [function]);
Kernel kernel = ... // Create kernel
kernel.Plugins.Add(plugin);

ChatCompletionAgent agent = new() { Kernel = kernel, ... };
```

#### Agent Framework

In agent framework in a single call you can register tools directly in the agent creation process.

```csharp
AIAgent agent = chatClient.CreateAIAgent(tools: [AIFunctionFactory.Create(GetWeather)]);
```

### 6. Agent Non-Streaming Invocation

Key differences can be seen in the method names from `Invoke` to `Run`, return types and parameters `AgentRunOptions`.

#### Semantic Kernel

The Non-Streaming uses a streaming pattern `IAsyncEnumerable<AgentResponseItem<ChatMessageContent>>` for returning multiple agent messages.

```csharp
await foreach (AgentResponseItem<ChatMessageContent> result in agent.InvokeAsync(userInput, thread, agentOptions))
{
    Console.WriteLine(result.Message);
}
```

#### Agent Framework

The Non-Streaming returns a single `AgentRunResponse` with the agent response that can contain multiple messages.
The text result of the run is available in `AgentRunResponse.Text` or `AgentRunResponse.ToString()`.
All messages created as part of the response is returned in the `AgentRunResponse.Messages` list.
This may include tool call messages, function results, reasoning updates and final results.

```csharp
AgentRunResponse agentResponse = await agent.RunAsync(userInput, thread);
```

### 7. Agent Streaming Invocation

Key differences in the method names from `Invoke` to `Run`, return types and parameters `AgentRunOptions`.

#### Semantic Kernel

```csharp
await foreach (StreamingChatMessageContent update in agent.InvokeStreamingAsync(userInput, thread))
{
    Console.Write(update);
}
```

#### Agent Framework

Similar streaming API pattern with the key difference being that it returns `AgentRunResponseUpdate` objects including more agent related information per update.

All updates produced by any service underlying the AIAgent is returned. The textual result of the agent is available by concatenating the `AgentRunResponse.Text` values.

```csharp
await foreach (AgentRunResponseUpdate update in agent.RunStreamingAsync(userInput, thread))
{
    Console.Write(update); // Update is ToString() friendly
}
```

### 8. Tool Function Signatures

**Problem**: SK plugin methods need `[KernelFunction]` attributes

```csharp
public class MenuPlugin
{
    [KernelFunction] // Required for SK
    public static MenuItem[] GetMenu() => ...;
}
```

**Solution**: AF can use methods directly without attributes

```csharp
public class MenuTools
{
    [Description("Get menu items")] // Optional description
    public static MenuItem[] GetMenu() => ...;
}
```

### 9. Options Configuration

**Problem**: Complex options setup in SK

```csharp
OpenAIPromptExecutionSettings settings = new() { MaxTokens = 1000 };
AgentInvokeOptions options = new() { KernelArguments = new(settings) };
```

**Solution**: Simplified options in AF

```csharp
ChatClientAgentRunOptions options = new(new() { MaxOutputTokens = 1000 });
```

> [!IMPORTANT]
> This example shows passing implementation specific options to a `ChatClientAgent`. Not all `AIAgents` support `ChatClientAgentRunOptions`.
> `ChatClientAgent` is provided to build agents based on underlying inference services, and therefore supports inference options like `MaxOutputTokens`.

### 10. Dependency Injection

#### Semantic Kernel

A `Kernel` registration is required in the service container to be able to create an agent
as every agent abstractions needs to be initialized with a `Kernel` property.

Semantic Kernel uses the `Agent` type as the base abstraction class for agents.

```csharp
services.AddKernel().AddProvider(...);
serviceContainer.AddKeyedSingleton<SemanticKernel.Agents.Agent>(
    TutorName,
    (sp, key) =>
        new ChatCompletionAgent()
        {
            // Passing the kernel is required
            Kernel = sp.GetRequiredService<Kernel>(),
        });
```

#### Agent Framework

The Agent framework provides the `AIAgent` type as the base abstraction class.

```csharp
services.AddKeyedSingleton<AIAgent>(() => client.CreateAIAgent(...));
```

### 11. **Agent Type Consolidation**

#### Semantic Kernel

Semantic kernel provides specific agent classes for various services, e.g.

- `ChatCompletionAgent` for use with chat-completion-based inference services.
- `OpenAIAssistantAgent` for use with the OpenAI Assistants service.
- `AzureAIAgent` for use with the Azure AI Foundry Agents service.

#### Agent Framework

The agent framework supports all the above mentioned services via a single agent type, `ChatClientAgent`.

`ChatClientAgent` can be used to build agents using any underlying service that provides an SDK implementing the `Microsoft.Extensions.AI.IChatClient` interface.

::: zone-end
::: zone pivot="programming-language-python"

## Key differences

Here is a summary of the key differences between the Semantic Kernel Agent Framework and the Microsoft Agent Framework to help you migrate your code.

### 1. Package and Import Updates

#### Semantic Kernel

Semantic Kernel packages are installed as `semantic-kernel` and imported as `semantic_kernel`. The package also has a number of `extras` that you can install to install the different dependencies for different AI providers and other features.

```python
from semantic_kernel import Kernel
from semantic_kernel.agents import ChatCompletionAgent
```

#### Agent Framework

Agent Framework package is installed as `agent-framework` and imported as `agent_framework`.
Agent Framework is built up differently, it has a core package `agent-framework-core` that contains the core functionality, and then there are multiple packages that rely on that core package, such as `agent-framework-azure-ai`, `agent-framework-mem0`, `agent-framework-copilotstudio`, etc. When you run `pip install agent-framework` it will install the core package and *all* packages, so that you can get started with all the features quickly. When you are ready to reduce the number of packages because you know what you need, you can install only the packages you need, so for instance if you only plan to use Azure AI Foundry and Mem0 you can install only those two packages: `pip install agent-framework-azure-ai agent-framework-mem0`, `agent-framework-core` is a dependency to those two, so will automatically be installed.

Even though the packages are split up, the imports are all from `agent_framework`, or it's modules. So for instance to import the client for Azure AI Foundry you would do:

```python
from agent_framework.azure import AzureAIAgentClient
```

Many of the most commonly used types are imported directly from `agent_framework`:

```python
from agent_framework import ChatMessage, ChatAgent
```

### 2. Agent Type Consolidation

#### Semantic Kernel
Semantic Kernel provides specific agent classes for various services, e.g. ChatCompletionAgent, AzureAIAgent, OpenAIAssistantAgent, etc. See [Agent types in Semantic Kernel](../../../semantic-kernel/frameworks/agent/agent-types/azure-ai-agent.md).

#### Agent Framework
In Agent Framework the majority of agents are built using the `ChatAgent` which can be used with all the `ChatClient` based services, such as Azure AI Foundry, OpenAI ChatCompletion and OpenAI Responses. We currently have two other agents, `CopilotStudioAgent` for use with Copilot Studio and `A2AAgent` for use with A2A.

All the built-in agents are based on the BaseAgent (`from agent_framework import BaseAgent`). And all agents are consistent with the `AgentProtocol` (`from agent_framework import AgentProtocol`) interface.

### 2. Agent Creation Simplification

#### Semantic Kernel

Every agent in Semantic Kernel depends on a `Kernel` instance and will have
an empty `Kernel` if not provided.

```python
from semantic_kernel.agents import ChatCompletionAgent
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion

agent = ChatCompletionAgent(
    service=OpenAIChatCompletion(),
    name="Support",
    instructions="Answer in one sentence.",
)
```


#### Agent Framework

Agent creation in Agent Framework can be done in two ways, directly:

```python
from agent_framework.azure import AzureAIAgentClient
from agent_framework import ChatMessage, ChatAgent

agent = ChatAgent(chat_client=AzureAIAgentClient(credential=AzureCliCredential()), instructions="You are a helpful assistant")
```
or, with the convenience methods provided by chat clients:

```python
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity import AzureCliCredential
agent = AzureOpenAIChatClient(credential=AzureCliCredential()).create_agent(instructions="You are a helpful assistant")
```

The direct method, exposes all possible parameters you can set for your agent, while the convenience method has a subset, you can still pass in the same set of parameters, because internally we call the direct method.

### 3. Agent Thread Creation

#### Semantic Kernel

The caller has to know the thread type and create it manually.

```python
from semantic_kernel.agents import ChatHistoryAgentThread

thread = ChatHistoryAgentThread()
```

#### Agent Framework

The agent can be asked to create a new thread for you.

```python
agent = ...
thread = agent.get_new_thread()
```

a thread is then created in one of three ways:
1. if the agent has a thread_id (or conversation_id or something similar) set, it will create a thread in the underlying service with that id.
    Once a thread has a `service_thread_id`, you can no longer use it to store messages in memory.
    And this only applies to agents that have a service-side thread concept. such as Azure AI Foundry Agents and OpenAI Assistants.
2. if the agent has a `chat_message_store_factory` set, it will use that factory to create a message store and use that to create an in-memory thread.
    It can then no longer be used with a agent with the `store` parameter set to `True`.
3. if neither of the above is set, we consider it `uninitialized` and depending on how it is used, it will either become a in-memory thread or a service thread.

#### Agent Framework

> [!NOTE]
> OpenAI Responses introduced a new conversation model that simplifies how conversations are handled. This simplifies hosted thread management compared to the now deprecated OpenAI Assistants model. For more information see the [OpenAI Assistants migration guide](https://platform.openai.com/docs/assistants/migration).

Agent Framework doesn't have a thread deletion API in the `AgentThread` type as not all providers support hosted threads or thread deletion and this will become more common as more providers shift to responses based architectures.

If you require thread deletion and the provider allows this, the caller **should** keep track of the created threads and delete them later when necessary via the provider's sdk.

i.e: OpenAI Assistants Provider
```python
# OpenAI Assistants threads have self-deletion method in SK
await thread.delete_async()
```

### 5. Tool Registration

#### Semantic Kernel

In semantic kernel to expose a function as a tool you must:

1. Decorate the function with a `@kernel_function` decorator.
2. Have a `Plugin` class or use the kernel plugin factory to wrap the function.
3. Have a `Kernel` to add your plugin to.
4. Pass the `Kernel` to the agent.

```python
from semantic_kernel.functions import kernel_function

class SpecialsPlugin:
    @kernel_function(name="specials", description="List daily specials")
    def specials(self) -> str:
        return "Clam chowder, Cobb salad, Chai tea"

agent = ChatCompletionAgent(
    service=OpenAIChatCompletion(),
    name="Host",
    instructions="Answer menu questions accurately.",
    plugins=[SpecialsPlugin()],
)
```

#### Agent Framework

In agent framework in a single call you can register tools directly in the agent creation process. But we no longer have the concept of a plugin, to wrap multiple functions, but you can still do that if you want to.

The simplest way to create a tool is just to create a python function:

```python
def get_weather(location: str) -> str:
    """Get the weather for a given location."""
    return f"The weather in {location} is sunny."

agent = chat_client.create_agent(tools=get_weather)
```
> Note: the `tools` parameter is present on both the agent creation, the `run` and `run_stream` methods, as well as the `get_response` and `get_streaming_response` methods, it allows you to supply tools both as a list or a single function.

The name of the function will then become the name of the tool, and the docstring will become the description of the tool, you can also add a description to the parameters:

```python
from typing import Annotated

def get_weather(location: Annotated[str, "The location to get the weather for."]) -> str:
    """Get the weather for a given location."""
    return f"The weather in {location} is sunny."
```

Finally, you can use the decorator to further customize the name and description of the tool:

```python
from typing import Annotated
from agent_framework import ai_function

@ai_function(name="weather_tool", description="Retrieves weather information for any location")
def get_weather(location: Annotated[str, "The location to get the weather for."])
    """Get the weather for a given location."""
    return f"The weather in {location} is sunny."
```

This also works when you create a class with multiple tools as methods.

When creating the agent, we can now provide the function tool to the agent, by passing it to the `tools` parameter.

```python
class Plugin:

    def __init__(self, initial_state: str):
        self.state: list[str] = [initial_state]

    def get_weather(self, location: Annotated[str, "The location to get the weather for."]) -> str:
        """Get the weather for a given location."""
        self.state.append(f"Requested weather for {location}. ")
        return f"The weather in {location} is sunny."

    def get_weather_details(self, location: Annotated[str, "The location to get the weather details for."]) -> str:
        """Get detailed weather for a given location."""
        self.state.append(f"Requested detailed weather for {location}. ")
        return f"The weather in {location} is sunny with a high of 25°C and a low of 15°C."

plugin = Plugin("Initial state")
agent = chat_client.create_agent(tools=[plugin.get_weather, plugin.get_weather_details])

... # use the agent

print("Plugin state:", plugin.state)
```
> Note: the functions within the class can also be decorated with `@ai_function` to customize the name and description of the tools.

This mechanism is also useful for tools that need additional input that cannot be supplied by the LLM, such as connections, secrets, etc.

### 6. Agent Non-Streaming Invocation

Key differences can be seen in the method names from `invoke` to `run`, return types (e.g. `AgentRunResponse`) and parameters.

#### Semantic Kernel

The Non-Streaming invoke uses an async iterator pattern for returning multiple agent messages.

```python
async for response in agent.invoke(
    messages=user_input,
    thread=thread,
):
    print(f"# {response.role}: {response}")
    thread = response.thread
```
And we had a convenience method to get the final response:
```python
response = await agent.get_response(messages="How do I reset my bike tire?", thread=thread)
print(f"# {response.role}: {response}")
```

#### Agent Framework

The Non-Streaming run returns a single `AgentRunResponse` with the agent response that can contain multiple messages.
The text result of the run is available in `response.text` or `str(response)`.
All messages created as part of the response are returned in the `response.messages` list.
This may include tool call messages, function results, reasoning updates and final results.

```python
agent = ...

response = await agent.run(user_input, thread)
print("Agent response:", response.text)

```

### 7. Agent Streaming Invocation

Key differences in the method names from `invoke` to `run_stream`, return types (`AgentRunResponseUpdate`) and parameters.

#### Semantic Kernel

```python
async for update in agent.invoke_stream(
    messages="Draft a 2 sentence blurb.",
    thread=thread,
):
    if update.message:
        print(update.message.content, end="", flush=True)
```

#### Agent Framework

Similar streaming API pattern with the key difference being that it returns `AgentRunResponseUpdate` objects including more agent related information per update.

All contents produced by any service underlying the Agent are returned. The final result of the agent is available by combining the `update` values into a single response.

```python
from agent_framework import AgentRunResponse
agent = ...
updates = []
async for update in agent.run_stream(user_input, thread):
    updates.append(update)
    print(update.text)

full_response = AgentRunResponse.from_agent_run_response_updates(updates)
print("Full agent response:", full_response.text)
```

You can even do that directly:

```python
from agent_framework import AgentRunResponse
agent = ...
full_response = AgentRunResponse.from_agent_response_generator(agent.run_stream(user_input, thread))
print("Full agent response:", full_response.text)
```


### 9. Options Configuration

**Problem**: Complex options setup in SK

```python
from semantic_kernel.connectors.ai.open_ai import OpenAIPromptExecutionSettings

settings = OpenAIPromptExecutionSettings(max_tokens=1000)
arguments = KernelArguments(settings)

response = await agent.get_response(user_input, thread=thread, arguments=arguments)
```

**Solution**: Simplified options in AF

In agent framework, we allow the passing of all parameters directly to the relevant methods, so that you do not have to import anything extra, or create any options objects, unless you want to. Internally we use a `ChatOptions` object for `ChatClients` and `ChatAgents`, that you can also create and pass in if you want to. This is also created in a `ChatAgent` to hold the options and can be overridden per call.

```python
agent = ...

response = await agent.run(user_input, thread, max_tokens=1000, frequency_penalty=0.5)
```

> Note: The above is specific to a `ChatAgent`, because other agents may have different options, they should all accepts `messages` as a parameter, since that is defined in the `AgentProtocol`.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Quickstart Guide](../../tutorials/quick-start.md)
