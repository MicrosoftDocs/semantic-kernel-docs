---
title: Semantic Kernel Agent Architecture
description: An overview of the architecture of the Semantic Kernel Agent Framework and how it aligns with core Semantic Kernel features.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# An Overview of the Agent Architecture

> [!IMPORTANT]
> `AgentChat` patterns are in the experimental stage. These patterns are under active development and may change significantly before advancing to the preview or release candidate stage.

This article covers key concepts in the architecture of the Agent Framework, including foundational principles, design objectives, and strategic goals.


## Goals

The `Agent Framework` was developed with the following key priorities in mind:

- The Semantic Kernel agent framework serves as the core foundation for implementing agent functionalities.
- Multiple agents can collaborate within a single conversation, while integrating human input.
- An agent can engage in and manage multiple concurrent conversations simultaneously.
- Different types of agents can participate in the same conversation, each contributing their unique capabilities.


## Agent

The abstract `Agent` class serves as the core abstraction for all types of agents, providing a foundational structure that can be extended to create more specialized agents. One key subclass is _Kernel Agent_, which establishes a direct association with a [`Kernel`](../../concepts/kernel.md) object. This relationship forms the basis for more specific agent implementations, such as the [`ChatCompletionAgent`](./chat-completion-agent.md), [`OpenAIAssistantAgent`](./assistant-agent.md), [`AzureAIAgent`](./azure-ai-agent.md), or [`OpenAIResponsesAgent`](./responses-agent.md), all of which leverage the Kernel's capabilities to execute their respective functions.

::: zone pivot="programming-language-csharp"

- [`Agent`](/dotnet/api/microsoft.semantickernel.agents.agent)
- [`KernelAgent`](/dotnet/api/microsoft.semantickernel.agents.kernelagent)

::: zone-end

::: zone pivot="programming-language-python"

The underlying Semantic Kernel `Agent` abstraction can be found here:

- [`Agent`](/python/api/semantic-kernel/semantic_kernel.agents.agent)

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

Agents can either be invoked directly to perform tasks or orchestrated within an [`AgentChat`](./agent-chat.md), where multiple agents may collaborate or interact dynamically with user inputs. This flexible structure allows agents to adapt to various conversational or task-driven scenarios, providing developers with robust tools for building intelligent, multi-agent systems.

#### Deep Dive:

- [`ChatCompletionAgent`](./chat-completion-agent.md)
- [`OpenAIAssistantAgent`](./assistant-agent.md)
- [`AzureAIAgent`](./azure-ai-agent.md)
- [`OpenAIResponsesAgent`](./responses-agent.md)


<!-- FUTURE
## Agent Exensibility
  -->

## Agent Thread

The abstract `AgentThread` class serves as the core abstraction for threads or conversation state.
It abstracts away the different ways in which conversation state may be managed for different agents.

Stateful agent services often store conversation state in the service, and you can interact with it via an id.
Other agents may require the entire chat history to be passed to the agent on each invocation, in which
case the conversation state is managed locally in the application.

Stateful agents typically only work with a matching `AgentThread` implementation, while other types of agents could work with more than one `AgentThread` type.
For example, `AzureAIAgent` requires a matching `AzureAIAgentThread`.
This is because the Azure AI Agent service stores conversations in the service, and requires specific service calls to create a thread and update it.
If a different agent thread type is used with the `AzureAIAgent`, we fail fast due to an unexpected thread type and raise an exception to alert the caller.

## Agent Chat

The  [`AgentChat`](./agent-chat.md) class serves as the foundational component that enables agents of any type to engage in a specific conversation. This class provides the essential capabilities for managing agent interactions within a chat environment. Building on this, the [`AgentGroupChat`](./agent-chat.md#creating-an-agentgroupchat) class extends these capabilities by offering a strategy-based container, which allows multiple agents to collaborate across numerous interactions within the same conversation.

> [!IMPORTANT]
> The current `OpenAIResponsesAgent` is not supported as part of Semantic Kernel's `AgentGroupChat` patterns. Stayed tuned for updates.

This structure facilitates more complex, multi-agent scenarios where different agents can work together, share information, and dynamically respond to evolving conversations, making it an ideal solution for advanced use cases such as customer support, multi-faceted task management, or collaborative problem-solving environments.

#### Deep Dive:
- [`AgentChat`](./agent-chat.md)


## Agent Channel

The Agent Channel class enables agents of various types to participate in an [`AgentChat`](./agent-chat.md). This functionality is completely hidden from users of the `Agent Framework` and only needs to be considered by developers creating a custom [`Agent`](#agent).

::: zone pivot="programming-language-csharp"

- [`AgentChannel`](/dotnet/api/microsoft.semantickernel.agents.agentchannel)

::: zone-end

::: zone pivot="programming-language-python"

- [`agent_channel`](/python/api/semantic-kernel/semantic_kernel.agents.channels.agent_channel)

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Agent Alignment with Semantic Kernel Features

The `Agent Framework` is built on the foundational concepts and features that many developers have come to know within the Semantic Kernel ecosystem. These core principles serve as the building blocks for the Agent Framework’s design. By leveraging the familiar structure and capabilities of the Semantic Kernel, the Agent Framework extends its functionality to enable more advanced, autonomous agent behaviors, while maintaining consistency with the broader Semantic Kernel architecture. This ensures a smooth transition for developers, allowing them to apply their existing knowledge to create intelligent, adaptable agents within the framework.


### The `Kernel`

::: zone pivot="programming-language-csharp"

At the heart of the Semantic Kernel ecosystem is the [`Kernel`](../../concepts/kernel.md), which serves as the core object that drives AI operations and interactions. To create any agent within this framework, a Kernel instance is required as it provides the foundational context and capabilities for the agent’s functionality. The `Kernel` acts as the engine for processing instructions, managing state, and invoking the necessary AI services that power the agent's behavior.

::: zone-end

::: zone pivot="programming-language-python"

At the core of the Semantic Kernel ecosystem is the [`Kernel`](../../concepts/kernel.md), the primary object responsible for managing AI operations and interactions. To simplify onboarding, the `Kernel` is optional—if none is supplied when constructing an agent, a new `Kernel` instance is automatically created for the caller. For more advanced scenarios, such as applying filters, the caller must configure the desired filters on a `Kernel` instance and explicitly pass it to the agent.

::: zone-end

::: zone pivot="programming-language-java"

At the heart of the Semantic Kernel ecosystem is the [`Kernel`](../../concepts/kernel.md), which serves as the core object that drives AI operations and interactions. To create any agent within this framework, a Kernel instance is required as it provides the foundational context and capabilities for the agent’s functionality. The `Kernel` acts as the engine for processing instructions, managing state, and invoking the necessary AI services that power the agent's behavior.

::: zone-end

The [`AzureAIAgent`](./azure-ai-agent.md), [`ChatCompletionAgent`](./chat-completion-agent.md), [`OpenAIAssistantAgent`](./assistant-agent.md), and [`OpenAIResponsesAgent`](./responses-agent.md) articles provide specific details on how to create each type of agent.

These resources offer step-by-step instructions and highlight the key configurations needed to tailor the agents to different conversational or task-based applications, demonstrating how the Kernel enables dynamic and intelligent agent behaviors across diverse use cases.

#### Related API's:

::: zone pivot="programming-language-csharp"

- [`IKernelBuilder`](/dotnet/api/microsoft.semantickernel)
- [`Kernel`](/dotnet/api/microsoft.semantickernel.kernel)
- [`KernelBuilderExtensions`](/dotnet/api/microsoft.semantickernel.kernelbuilderextensions)
- [`KernelExtensions`](/dotnet/api/microsoft.semantickernel.kernelextensions)

::: zone-end

::: zone pivot="programming-language-python"

- [`kernel`](/python/api/semantic-kernel/semantic_kernel.kernel.kernel)

::: zone-end

::: zone pivot="programming-language-java"

- [`Kernel`](/java/api/com.microsoft.semantickernel.kernel)

::: zone-end


### [Plugins and Function Calling](./agent-functions.md)

Plugins are a fundamental aspect of the Semantic Kernel, enabling developers to integrate custom functionalities and extend the capabilities of an AI application. These plugins offer a flexible way to incorporate specialized features or business-specific logic into the core AI workflows. Additionally, agent capabilities within the framework can be significantly enhanced by utilizing [Plugins](../../concepts/plugins/index.md) and leveraging [Function Calling](../../concepts/ai-services/chat-completion/function-calling/index.md). This allows agents to dynamically interact with external services or execute complex tasks, further expanding the scope and versatility of the AI system within diverse applications.

#### Example:

- [How-To: `ChatCompletionAgent`](./examples/example-chat-agent.md)

#### Related API's:

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

- [`KernelFunction`](/java/api/com.microsoft.semantickernel.semanticfunctions.kernelfunction)
- [`KernelPlugin`](/java/api/com.microsoft.semantickernel.plugin.kernelplugin)
- [`KernelPluginFactory`](/java/api/com.microsoft.semantickernel.plugin.kernelpluginfactory)

::: zone-end


### [Agent Messages](../../concepts/ai-services/chat-completion/chat-history.md)

Agent messaging, including both input and response, is built upon the core content types of the Semantic Kernel, providing a unified structure for communication. This design choice simplifies the process of transitioning from traditional chat-completion patterns to more advanced agent-driven patterns in your application development. By leveraging familiar Semantic Kernel content types, developers can seamlessly integrate agent capabilities into their applications without needing to overhaul existing systems. This streamlining ensures that as you evolve from basic conversational AI to more autonomous, task-oriented agents, the underlying framework remains consistent, making development faster and more efficient.

> Note: The [`OpenAIAssistantAgent`](./assistant-agent.md) introduced content types specific to its usage for File References and Content Annotation: 

#### Related API's:

::: zone pivot="programming-language-csharp"

- [`ChatHistory`](/dotnet/api/microsoft.semantickernel.chatcompletion.chathistory)
- [`ChatMessageContent`](/dotnet/api/microsoft.semantickernel.chatmessagecontent)
- [`KernelContent`](/dotnet/api/microsoft.semantickernel.kernelcontent)
- [`StreamingKernelContent`](/dotnet/api/microsoft.semantickernel.streamingkernelcontent)
- [`FileReferenceContent`](/dotnet/api/microsoft.semantickernel.filereferencecontent)
- [`AnnotationContent`](/dotnet/api/microsoft.semantickernel.agents.openai.annotationcontent)


::: zone-end

::: zone pivot="programming-language-python"

- [`chat_history`](/python/api/semantic-kernel/semantic_kernel.contents.chat_history)
- [`chat_message_content`](/python/api/semantic-kernel/semantic_kernel.contents.chat_message_content)
- [`kernel_content`](/python/api/semantic-kernel/semantic_kernel.contents.kernel_content)
- [`streaming_chat_message_content`](/python/api/semantic-kernel/semantic_kernel.contents.streaming_chat_message_content)
- [`file_reference_content`](/python/api/semantic-kernel/semantic_kernel.contents.file_reference_content)
- [`annotation_content`](/python/api/semantic-kernel/semantic_kernel.contents.annotation_content)

::: zone-end

::: zone pivot="programming-language-java"

- [`ChatHistory`](/java/api/com.microsoft.semantickernel.services.chatcompletion.chathistory)
- [`ChatMessageContent`](/java/api/com.microsoft.semantickernel.services.chatcompletion.chatmessagecontent)
- [`KernelContent`](/java/api/com.microsoft.semantickernel.services.kernelcontent)
- [`StreamingKernelContent`](/java/api/com.microsoft.semantickernel.services.streamingkernelcontent)

::: zone-end


### [Templating](./agent-templates.md)

An agent's role is primarily shaped by the instructions it receives, which dictate its behavior and actions. Similar to invoking a `Kernel` [prompt](../../concepts/prompts/index.md), an agent's instructions can include templated parameters—both values and functions—that are dynamically substituted during execution. This enables flexible, context-aware responses, allowing the agent to adjust its output based on real-time input.

Additionally, an agent can be configured directly using a Prompt Template Configuration, providing developers with a structured and reusable way to define its behavior. This approach offers a powerful tool for standardizing and customizing agent instructions, ensuring consistency across various use cases while still maintaining dynamic adaptability.

#### Example:

- [How-To: `ChatCompletionAgent`](./examples/example-chat-agent.md)

#### Related API's:

::: zone pivot="programming-language-csharp"

- [`PromptTemplateConfig`](/dotnet/api/microsoft.semantickernel.prompttemplateconfig)
- [`KernelFunctionYaml.FromPromptYaml`](/dotnet/api/microsoft.semantickernel.kernelfunctionyaml.frompromptyaml#microsoft-semantickernel-kernelfunctionyaml-frompromptyaml(system-string-microsoft-semantickernel-iprompttemplatefactory-microsoft-extensions-logging-iloggerfactory))
- [`IPromptTemplateFactory`](/dotnet/api/microsoft.semantickernel.iprompttemplatefactory)
- [`KernelPromptTemplateFactory`](/dotnet/api/microsoft.semantickernel.kernelprompttemplatefactory)
- [`Handlebars`](/dotnet/api/microsoft.semantickernel.prompttemplates.handlebars)
- [`Prompty`](/dotnet/api/microsoft.semantickernel.prompty)
- [`Liquid`](/dotnet/api/microsoft.semantickernel.prompttemplates.liquid)

::: zone-end

::: zone pivot="programming-language-python"

- [`prompt_template_config`](/python/api/semantic-kernel/semantic_kernel.prompt_template.prompt_template_config)
- [`kernel_prompt_template`](/python/api/semantic-kernel/semantic_kernel.prompt_template.kernel_prompt_template)
- [`jinja2_prompt_template`](/python/api/semantic-kernel/semantic_kernel.prompt_template.jinja2_prompt_template)
- [`handlebars_prompt_template`](/python/api/semantic-kernel/semantic_kernel.prompt_template.handlebars_prompt_template)

::: zone-end

::: zone pivot="programming-language-java"

- [`PromptTemplateConfig`](/java/api/com.microsoft.semantickernel.semanticfunctions.prompttemplateconfig)
- [`KernelFunctionYaml.FromPromptYaml`](/java/api/com.microsoft.semantickernel.semanticfunctions.kernelfunctionyaml#com-microsoft-semantickernel-semanticfunctions-kernelfunctionyaml-(t)frompromptyaml(java-lang-string))
- [`PromptTemplateFactory`](/java/api/com.microsoft.semantickernel.semanticfunctions.prompttemplatefactory)
- [`KernelPromptTemplateFactory`](/java/api/com.microsoft.semantickernel.semanticfunctions.kernelprompttemplatefactory)
- [`Handlebars`](/java/api/com.microsoft.semantickernel.templateengine.handlebars.handlebarsprompttemplate)

::: zone-end


### [Chat Completion](./chat-completion-agent.md)

The [`ChatCompletionAgent`](./chat-completion-agent.md) is designed around any Semantic Kernel [AI service](../../concepts/ai-services/chat-completion/index.md), offering a flexible and convenient persona encapsulation that can be seamlessly integrated into a wide range of applications. This agent allows developers to easily bring conversational AI capabilities into their systems without having to deal with complex implementation details. It mirrors the features and patterns found in the underlying [AI service](../../concepts/ai-services/chat-completion/index.md), ensuring that all functionalities—such as natural language processing, dialogue management, and contextual understanding—are fully supported within the [`ChatCompletionAgent`](./chat-completion-agent.md), making it a powerful tool for building conversational interfaces.

#### Related API's:

::: zone pivot="programming-language-csharp"

- [`IChatCompletionService`](/dotnet/api/microsoft.semantickernel.chatcompletion.ichatcompletionservice)
- [`Microsoft.SemanticKernel.Connectors.AzureOpenAI`](/dotnet/api/microsoft.semantickernel.connectors.azureopenai)
- [`Microsoft.SemanticKernel.Connectors.OpenAI`](/dotnet/api/microsoft.semantickernel.connectors.openai)
- [`Microsoft.SemanticKernel.Connectors.Google`](/dotnet/api/microsoft.semantickernel.connectors.google)
- [`Microsoft.SemanticKernel.Connectors.HuggingFace`](/dotnet/api/microsoft.semantickernel.connectors.huggingface)
- [`Microsoft.SemanticKernel.Connectors.MistralAI`](/dotnet/api/microsoft.semantickernel.connectors.mistralai)
- [`Microsoft.SemanticKernel.Connectors.Onnx`](/dotnet/api/microsoft.semantickernel.connectors.onnx)

::: zone-end

::: zone pivot="programming-language-python"

- [`azure_chat_completion`](/python/api/semantic-kernel/semantic_kernel.connectors.ai.open_ai.services.azure_chat_completion)
- [`open_ai_chat_completion`](/python/api/semantic-kernel/semantic_kernel.connectors.ai.open_ai.services.open_ai_chat_completion)

::: zone-end

::: zone pivot="programming-language-java"

- [`semantickernel.aiservices.openai`](/java/api/com.microsoft.semantickernel.services.openai)
- [`semantickernel.aiservices.google`](/java/api/com.microsoft.semantickernel.aiservices.google)
- [`semantickernel.aiservices.huggingface`](/java/api/com.microsoft.semantickernel.aiservices.huggingface)

::: zone-end


> [!div class="nextstepaction"]
> [Explore the Common Agent Invocation API](./agent-api.md)

