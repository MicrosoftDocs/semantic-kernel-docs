---
title: Configuring Agents with Semantic Kernel Plugins. (Experimental)
description: Describes how to use Semantic Kernal plugins and function calling with agents.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# Configuring Agents with Semantic Kernel Plugins (Experimental)

> [!WARNING]
> The _Semantic Kernel Agent Framework_ is experimental, still in development and is subject to change.

## Functions and Plugins in Semantic Kernel

Function calling is a powerful tool that allows developers to add custom functionalities and expand the capabilities of AI applications. The _Semantic Kernel_ [Plugin](../../concepts/plugins/index.md) architecture offers a flexible framework to support [Function Calling](../../concepts/ai-services/chat-completion/function-calling/index.md). For an _Agent_, integrating [Plugins](../../concepts/plugins/index.md) and [Function Calling](../../concepts/ai-services/chat-completion/function-calling/index.md) is built on this foundational _Semantic Kernel_ feature.

Once configured, an agent will choose when and how to call an available function, as it would in any usage outside of the _Agent Framework_.

::: zone pivot="programming-language-csharp"

- [`KernelFunctionFactory`](https://learn.microsoft.com/dotnet/api/microsoft.semantickernel.kernelfunctionfactory)
- [`KernelFunction`](https://learn.microsoft.com/dotnet/api/microsoft.semantickernel.kernelfunction)
- [`KernelPluginFactory`](https://learn.microsoft.com/dotnet/api/microsoft.semantickernel.kernelpluginfactory)
- [`KernelPlugin`](https://learn.microsoft.com/dotnet/api/microsoft.semantickernel.kernelplugin)
- [`Kernel.Plugins`](https://learn.microsoft.com/dotnet/api/microsoft.semantickernel.kernel.plugins)

::: zone-end

::: zone pivot="programming-language-python"

- [`kernel_function`](https://learn.microsoft.com/python/api/semantic-kernel/semantic_kernel.functions.kernel_function)
- [`kernel_function_extension`](https://learn.microsoft.com/python/api/semantic-kernel/semantic_kernel.functions.kernel_function_extension)
- [`kernel_plugin`](https://learn.microsoft.com/python/api/semantic-kernel/semantic_kernel.functions.kernel_plugin)

::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## Adding Plugins to an Agent

Any [Plugin](../../concepts/plugins/index.md) available to an _Agent_ is managed within its respective _Kernel_ instance. This setup enables each _Agent_ to access distinct functionalities based on its specific role.

[Plugins](../../concepts/plugins/index.md) can be added to the _Kernel_ either before or after the _Agent_ is created. The process of initializing [Plugins](../../concepts/plugins/index.md) follows the same patterns used for any _Semantic Kernel_ implementation, allowing for consistency and ease of use in managing AI capabilities.

> Note: For a [_Chat Completion Agent_](./chat-completion-agent.md), the function calling mode must be explicitly enabled.  [_Open AI Assistant_](./assistant-agent.md) agent is always based on automatic function calling.

::: zone pivot="programming-language-csharp"
```csharp
// Factory method to product an agent with a specific role.
// Could be incorporated into DI initialization.
ChatCompletionAgent CreateSpecificAgent(Kernel kernel, string credentials)
{
    // Clone kernel instance to allow for agent specific plug-in definition
    Kernel agentKernel = kernel.Clone();

    // Initialize plug-in from type
    agentKernel.CreatePluginFromType<StatelessPlugin>();

    // Initialize plug-in from object
    agentKernel.CreatePluginFromObject(new StatefulPlugin(credentials));

    // Create the agent
    return 
        new ChatCompletionAgent()
        {
            Name = "<agent name>",
            Instructions = "<agent instructions>",
            Kernel = agentKernel,
            Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings() 
                { 
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() 
                })
        };
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Create the instance of the Kernel
kernel = Kernel()

# Define the service ID
service_id = "<service ID>"

# Add the chat completion service to the Kernel
kernel.add_service(AzureChatCompletion(service_id=service_id))

# Get the AI Service settings for the specified service_id
settings = kernel.get_prompt_execution_settings_from_service_id(service_id=service_id)

# Configure the function choice behavior to auto invoke kernel functions
settings.function_choice_behavior = FunctionChoiceBehavior.Auto()

# Add the Plugin to the Kernel
kernel.add_plugin(SamplePlugin(), plugin_name="<plugin name>")

# Create the agent
agent = ChatCompletionAgent(
    service_id=service_id, 
    kernel=kernel, 
    name=<agent name>, 
    instructions=<agent instructions>, 
    execution_settings=settings,
)
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## Adding Functions to an Agent

A [Plugin](../../concepts/plugins/index.md) is the most common approach for configuring [Function Calling](../../concepts/ai-services/chat-completion/function-calling/index.md). However, individual functions can also be supplied independently including _prompt functions_.

::: zone pivot="programming-language-csharp"
```csharp
// Factory method to product an agent with a specific role.
// Could be incorporated into DI initialization.
ChatCompletionAgent CreateSpecificAgent(Kernel kernel)
{
    // Clone kernel instance to allow for agent specific plug-in definition
    Kernel agentKernel = kernel.Clone();

    // Initialize plug-in from a static function
    agentKernel.CreateFunctionFromMethod(StatelessPlugin.AStaticMethod);

    // Initialize plug-in from a prompt
    agentKernel.CreateFunctionFromPrompt("<your prompt instructiosn>");
    
    // Create the agent
    return 
        new ChatCompletionAgent()
        {
            Name = "<agent name>",
            Instructions = "<agent instructions>",
            Kernel = agentKernel,
            Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings() 
                { 
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() 
                })
        };
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Create the instance of the Kernel
kernel = Kernel()

# Define the service ID
service_id = "<service ID>"

# Add the chat completion service to the Kernel
kernel.add_service(AzureChatCompletion(service_id=service_id))

# Get the AI Service settings for the specified service_id
settings = kernel.get_prompt_execution_settings_from_service_id(service_id=service_id)

# Configure the function choice behavior to auto invoke kernel functions
settings.function_choice_behavior = FunctionChoiceBehavior.Auto()

# Add the Plugin to the Kernel
kernel.add_plugin(SamplePlugin(), plugin_name="<plugin name>")

# Create the agent
agent = ChatCompletionAgent(
    service_id=service_id, 
    kernel=kernel, 
    name=<agent name>, 
    instructions=<agent instructions>, 
    execution_settings=settings,
)
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## Limitations for Agent Function Calling

When directly invoking a[_Chat Completion Agent_](./chat-completion-agent.md), all _Function Choice Behaviors_ are supported. However, when using an [_Open AI Assistant_](./assistant-agent.md) or [_Agent Chat_](./agent-chat.md), only _Automatic_ [Function Calling](../../concepts/ai-services/chat-completion/function-calling/index.md) is currently available.


## How-To

For an end-to-end example for using function calling, see:

- [How-To: _Chat Completion Agent_](./examples/example-chat-agent.md)


> [!div class="nextstepaction"]
> [How to Stream Agent Responses](./agent-streaming.md)

