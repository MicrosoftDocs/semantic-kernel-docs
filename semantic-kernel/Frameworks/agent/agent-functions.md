---
title: Configuring Agents with Semantic Kernel Plugins.
description: Describes how to use Semantic Kernel plugins and function calling with agents.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# Configuring Agents with Semantic Kernel Plugins

> [!IMPORTANT]
> This feature is in the release candidate stage. Features at this stage are nearly complete and generally stable, though they may undergo minor refinements or optimizations before reaching full general availability.

## Functions and Plugins in Semantic Kernel

Function calling is a powerful tool that allows developers to add custom functionalities and expand the capabilities of AI applications. The Semantic Kernel [Plugin](../../concepts/plugins/index.md) architecture offers a flexible framework to support [Function Calling](../../concepts/ai-services/chat-completion/function-calling/index.md). For an `Agent`, integrating [Plugins](../../concepts/plugins/index.md) and [Function Calling](../../concepts/ai-services/chat-completion/function-calling/index.md) is built on this foundational Semantic Kernel feature.

Once configured, an agent will choose when and how to call an available function, as it would in any usage outside of the `Agent Framework`.

::: zone pivot="programming-language-csharp"

- [`KernelFunctionFactory`](/dotnet/api/microsoft.semantickernel.kernelfunctionfactory)
- [`KernelFunction`](/dotnet/api/microsoft.semantickernel.kernelfunction)
- [`KernelPluginFactory`](/dotnet/api/microsoft.semantickernel.kernelpluginfactory)
- [`KernelPlugin`](/dotnet/api/microsoft.semantickernel.kernelplugin)
- [`Kernel.Plugins`](/dotnet/api/microsoft.semantickernel.kernel.plugins)

::: zone-end

::: zone pivot="programming-language-python"

- [`kernel_function`](/python/api/semantic-kernel/semantic_kernel.functions.kernel_function)
- [`kernel_function_extension`](/python/api/semantic-kernel/semantic_kernel.functions.kernel_function_extension)
- [`kernel_plugin`](/python/api/semantic-kernel/semantic_kernel.functions.kernel_plugin)

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end


## Adding Plugins to an Agent

Any [Plugin](../../concepts/plugins/index.md) available to an `Agent` is managed within its respective `Kernel` instance. This setup enables each `Agent` to access distinct functionalities based on its specific role.

[Plugins](../../concepts/plugins/index.md) can be added to the `Kernel` either before or after the `Agent` is created. The process of initializing [Plugins](../../concepts/plugins/index.md) follows the same patterns used for any Semantic Kernel implementation, allowing for consistency and ease of use in managing AI capabilities.

> Note: For a [`ChatCompletionAgent`](./chat-completion-agent.md), the function calling mode must be explicitly enabled.  [`OpenAIAssistant`](./assistant-agent.md) agent is always based on automatic function calling.

::: zone pivot="programming-language-csharp"
```csharp
// Factory method to product an agent with a specific role.
// Could be incorporated into DI initialization.
ChatCompletionAgent CreateSpecificAgent(Kernel kernel, string credentials)
{
    // Clone kernel instance to allow for agent specific plug-in definition
    Kernel agentKernel = kernel.Clone();

    // Import plug-in from type
    agentKernel.ImportPluginFromType<StatelessPlugin>();

    // Import plug-in from object
    agentKernel.ImportPluginFromObject(new StatefulPlugin(credentials));

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

There are two ways to create a `ChatCompletionAgent` with plugins.

#### Method 1: Specify Plugins via the Constructor

You can directly pass a list of plugins to the constructor:

```python
from semantic_kernel.agents import ChatCompletionAgent

# Create the Chat Completion Agent instance by specifying a list of plugins
agent = ChatCompletionAgent(
    service=AzureChatCompletion(),
    instructions="<instructions>",
    plugins=[SamplePlugin()]
)
```

> [!TIP]
> By default, auto-function calling is enabled. To disable it, set the `function_choice_behavior` argument to `function_choice_behavior=FunctionChoiceBehavior.Auto(auto_invoke=False)` in the constructor. With this setting, plugins are broadcast to the model, but they are not automatically invoked. If execution settings specify the same `service_id` or `ai_model_id` as the AI service configuration, the function calling behavior defined in the execution settings (via `KernelArguments`) will take precedence over the function choice behavior set in the constructor.

#### Method 2: Configure the Kernel Manually

If no kernel is provided via the constructor, one is automatically created during model validation. Any plugins passed in take precedence and are added to the kernel. For more fine-grained control over the kernel's state, follow these steps:

```python
from semantic_kernel.agents import ChatCompletionAgent
from semantic_kernel.connectors.ai import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion, AzureChatPromptExecutionSettings
from semantic_kernel.functions import KernelFunctionFromPrompt
from semantic_kernel.kernel import Kernel

# Create the instance of the Kernel
kernel = Kernel()

# Add the chat completion service to the Kernel
kernel.add_service(AzureChatCompletion())

# Get the AI Service settings
settings = kernel.get_prompt_execution_settings_from_service_id()

# Configure the function choice behavior to auto invoke kernel functions
settings.function_choice_behavior = FunctionChoiceBehavior.Auto()

# Add the Plugin to the Kernel
kernel.add_plugin(SamplePlugin(), plugin_name="<plugin name>")

# Create the agent
agent = ChatCompletionAgent(
    kernel=kernel, 
    name=<agent name>, 
    instructions=<agent instructions>, 
    arguments=KernelArguments(settings=settings),
)
```

> [!TIP]
> If a `service_id` is not specified when adding a service to the kernel, it defaults to `default`. When configuring multiple AI services on the kernel, it’s recommended to differentiate them using the `service_id` argument. This allows you to retrieve execution settings for a specific `service_id` and tie those settings to the desired service.

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end


## Adding Functions to an Agent

A [Plugin](../../concepts/plugins/index.md) is the most common approach for configuring [Function Calling](../../concepts/ai-services/chat-completion/function-calling/index.md). However, individual functions can also be supplied independently including prompt functions.

::: zone pivot="programming-language-csharp"
```csharp
// Factory method to product an agent with a specific role.
// Could be incorporated into DI initialization.
ChatCompletionAgent CreateSpecificAgent(Kernel kernel)
{
    // Clone kernel instance to allow for agent specific plug-in definition
    Kernel agentKernel = kernel.Clone();

    // Create plug-in from a static function
    var functionFromMethod = agentKernel.CreateFunctionFromMethod(StatelessPlugin.AStaticMethod);

    // Create plug-in from a prompt
    var functionFromPrompt = agentKernel.CreateFunctionFromPrompt("<your prompt instructions>");

    // Add to the kernel
    agentKernel.ImportPluginFromFunctions("my_plugin", [functionFromMethod, functionFromPrompt]);

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
from semantic_kernel.agents import ChatCompletionAgent
from semantic_kernel.connectors.ai import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion, AzureChatPromptExecutionSettings
from semantic_kernel.functions import KernelFunctionFromPrompt
from semantic_kernel.kernel import Kernel

# Create the instance of the Kernel
kernel = Kernel()

# Add the chat completion service to the Kernel
kernel.add_service(AzureChatCompletion())

# Create the AI Service settings
settings = AzureChatPromptExecutionSettings()

# Configure the function choice behavior to auto invoke kernel functions
settings.function_choice_behavior = FunctionChoiceBehavior.Auto()

# Add the Plugin to the Kernel
kernel.add_function(
    plugin_name="<plugin_name>",
    function=KernelFunctionFromPrompt(
        function_name="<function_name>",
        prompt="<your prompt instructions>",
    )
)

# Create the agent
agent = ChatCompletionAgent(
    kernel=kernel, 
    name=<agent name>, 
    instructions=<agent instructions>, 
    arguments=KernelArguments(settings=settings),
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end


## Limitations for Agent Function Calling

When directly invoking a[`ChatCompletionAgent`](./chat-completion-agent.md), all Function Choice Behaviors are supported. However, when using an [`OpenAIAssistant`](./assistant-agent.md) or [`AgentChat`](./agent-chat.md), only Automatic [Function Calling](../../concepts/ai-services/chat-completion/function-calling/index.md) is currently available.


## How-To

For an end-to-end example for using function calling, see:

- [How-To: `ChatCompletionAgent`](./examples/example-chat-agent.md)


> [!div class="nextstepaction"]
> [How to Stream Agent Responses](./agent-streaming.md)

