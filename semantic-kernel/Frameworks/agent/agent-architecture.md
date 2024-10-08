---
title: Semantic Kernel Agent Architecture (Experimental)
description: An overview of the architecture of the Semantic Kernel Agent Framework and how it aligns with core Semantic Kernel features.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# An Overview of the Agent Architecture (Experimental)

> [!WARNING]
> The _Semantic Kernel Agent Framework_ is experimental, still in development and is subject to change.

This article explores the key concepts and priorities that shaped the development of the _Agent Framework_. It delves into the foundational principles, design objectives, and strategic goals that influenced its creation, providing insight into how these elements come together to drive the framework's functionality and effectiveness.

## Goals

The _Agent Framework_ was developed with the following key priorities in mind:

- The _Semantic Kernel_ framework serves as the core foundation for implementing agent functionalities.
- Multiple agents can collaborate within a single conversation, while integrating human input.
- An agent can engage in and manage multiple concurrent conversations simultaneously.
- Different types of agents can participate in the same conversation, each contributing their unique capabilities.


## Agent

The _Agent_ class serves as the core abstraction for all types of agents, providing a foundational structure that can be extended to create more specialized agents. One key subclass is _Kernel Agent_, which establishes a direct association with a [_Kernel_](../../concepts/kernel.md) object. This relationship forms the basis for more specific agent implementations, such as the [_Chat Completion Agent_](./chat-completion-agent.md) and the [_Open AI Assistant Agent_](./assistant-agent.md), both of which leverage the Kernel's capabilities to execute their respective functions.

::: zone pivot="programming-language-csharp"

- [`Agent`](/dotnet/api/microsoft.semantickernel.agents.agent)
- [`KernelAgent`](/dotnet/api/microsoft.semantickernel.agents.kernelagent)


::: zone-end

::: zone pivot="programming-language-python"

- [`agent`](python/api/semantic-kernel/semantic_kernel.agents.agent)

::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Agents can either be invoked directly to perform tasks or orchestrated within an [_Agent Chat_](./agent-chat.md), where multiple agents may collaborate or interact dynamically with user inputs. This flexible structure allows agents to adapt to various conversational or task-driven scenarios, providing developers with robust tools for building intelligent, multi-agent systems.

#### Deep Dive:

- [`ChatCompletionAgent`](./chat-completion-agent.md)
- [`OpenAIAssistantAgent`](./assistant-agent.md)


<!-- FUTURE
## Agent Exensibility
  -->


## Agent Chat

The  [_Agent Chat_](./agent-chat.md) class serves as the foundational component that enables agents of any type to engage in a specific conversation. This class provides the essential capabilities for managing agent interactions within a chat environment. Building on this, the [_Agent Group Chat_](./agent-chat.md#creating-an-agent-group-chat) class extends these capabilities by offering a stategy-based container, which allows multiple agents to collaborate across numerous interactions within the same conversation. 

This structure facilitates more complex, multi-agent scenarios where different agents can work together, share information, and dynamically respond to evolving conversations, making it an ideal solution for advanced use cases such as customer support, multi-faceted task management, or collaborative problem-solving environments.

#### Deep Dive:
- [`AgentChat`](./agent-chat.md)


## Agent Channel

The _Agent Channel_ class enables agents of various types to participate in an [_Agent Chat_](./agent-chat.md). This functionality is completely hidden from users of the _Agent Framework_ and only needs to be considered by developers creating a custom [_Agent_](#agent).

::: zone pivot="programming-language-csharp"

- [`AgentChannel`](/dotnet/api/microsoft.semantickernel.agents.agentchannel)

::: zone-end

::: zone pivot="programming-language-python"

- [`agent_channel](/python/api/semantic-kernel/semantic_kernel.agents.channels.agent_channel)

::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

## Agent Alignment with _Semantic Kernel_ Features

The __Agent Framework_ is built on the foundational concepts and features that many developers have come to know within the _Semantic Kernel_ ecosystem. These core principles serve as the building blocks for the Agent Framework’s design. By leveraging the familiar structure and capabilities of the _Semantic Kernel_, the Agent Framework extends its functionality to enable more advanced, autonomous agent behaviors, while maintaining consistency with the broader _Semantic Kernel_ architecture. This ensures a smooth transition for developers, allowing them to apply their existing knowledge to create intelligent, adaptable agents within the framework.


### [The Kernel](../../concepts/kernel.md)

At the heart of the _Semantic Kernel_ ecosystem is the _Kernel_, which serves as the core object that drives AI operations and interactions. To create any agent within this framework, a _Kernel instance_ is required as it provides the foundational context and capabilities for the agent’s functionality. The _Kernel_ acts as the engine for processing instructions, managing state, and invoking the necessary AI services that power the agent's behavior.

The [_Chat Completion Agent_](./chat-completion-agent.md) and [_Open AI Assistant Agent_](./assistant-agent.md) articles provide specific details on how to create each type of agent.
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
::: zone-end


### [Plugins and Function Calling](./agent-functions.md)

Plugins are a fundamental aspect of the _Semantic Kernel_, enabling developers to integrate custom functionalities and extend the capabilities of an AI application. These plugins offer a flexible way to incorporate specialized features or business-specific logic into the core AI workflows. Additionally, agent capabilities within the framework can be significantly enhanced by utilizing [Plugins](../../concepts/plugins/index.md) and leveraging [Function Calling](../../concepts/ai-services/chat-completion/function-calling/index.md). This allows agents to dynamically interact with external services or execute complex tasks, further expanding the scope and versatility of the AI system within diverse applications.

#### Example:

- [How-To: _Chat Completion Agent_](./examples/example-chat-agent.md)

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
::: zone-end


### [Agent Messages](../../concepts/ai-services/chat-completion/chat-history.md)

Agent messaging, including both input and response, is built upon the core content types of the _Semantic Kernel_, providing a unified structure for communication. This design choice simplifies the process of transitioning from traditional chat-completion patterns to more advanced agent-driven patterns in your application development. By leveraging familiar _Semantic Kernel_ content types, developers can seamlessly integrate agent capabilities into their applications without needing to overhaul existing systems. This streamlining ensures that as you evolve from basic conversational AI to more autonomous, task-oriented agents, the underlying framework remains consistent, making development faster and more efficient.

> Note: The [_Open AI Assistant Agent_`_](./assistant-agent.md) introduced content types specific to its usage for _File References_ and _Content Annotation_: 

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
::: zone-end


### [Templating](./agent-templates.md)

An agent's role is primarily shaped by the instructions it receives, which dictate its behavior and actions. Similar to invoking a _Kernel_ [prompt](../../concepts/prompts/index.md), an agent's instructions can include templated parameters—both values and functions—that are dynamically substituted during execution. This enables flexible, context-aware responses, allowing the agent to adjust its output based on real-time input.

Additionally, an agent can be configured directly using a _Prompt Template Configuration_, providing developers with a structured and reusable way to define its behavior. This approach offers a powerful tool for standardizing and customizing agent instructions, ensuring consistency across various use cases while still maintaining dynamic adaptability.

#### Example:

- [How-To: _Chat Completion Agent_](./examples/example-chat-agent.md)

#### Related API's:

::: zone pivot="programming-language-csharp"

- [`PromptTemplateConfig`](/dotnet/api/microsoft.semantickernel.prompttemplateconfig)
- [`KernelFunctionYaml.FromPromptYaml`](/dotnet/api/microsoft.semantickernel.kernelfunctionyaml.frompromptyaml#microsoft-semantickernel-kernelfunctionyaml-frompromptyaml(system-string-microsoft-semantickernel-iprompttemplatefactory-microsoft-extensions-logging-iloggerfactory))
- [`IPromptTemplateFactory`](/dotnet/api/microsoft.semantickernel.iprompttemplatefactory)
- [`KernelPromptTemplateFactory`](/dotnet/api/microsoft.semantickernel.kernelprompttemplatefactory)
- [_Handlebars_](/dotnet/api/microsoft.semantickernel.prompttemplates.handlebars)
- [_Prompty_](/dotnet/api/microsoft.semantickernel.prompty)
- [_Liquid_](/dotnet/api/microsoft.semantickernel.prompttemplates.liquid)

::: zone-end

::: zone pivot="programming-language-python"

- [`prompt_template_config`](/python/api/semantic-kernel/semantic_kernel.prompt_template.prompt_template_config)
- [`kernel_prompt_template`](/python/api/semantic-kernel/semantic_kernel.prompt_template.kernel_prompt_template)
- [`jinja2_prompt_template`](/python/api/semantic-kernel/semantic_kernel.prompt_template.jinja2_prompt_template)
- [`handlebars_prompt_teplate`](/python/api/semantic-kernel/semantic_kernel.prompt_template.handlebars_prompt_template)

::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


### [Chat Completion](./chat-completion-agent.md)

The [_Chat Completion Agent_](./chat-completion-agent.md) is designed around any _Semantic Kernel_ [AI service](../../concepts/ai-services/chat-completion/index.md), offering a flexible and convenient persona encapsulation that can be seamlessly integrated into a wide range of applications. This agent allows developers to easily bring conversational AI capabilities into their systems without having to deal with complex implementation details. It mirrors the features and patterns found in the underlying [AI service](../../concepts/ai-services/chat-completion/index.md), ensuring that all functionalities—such as natural language processing, dialogue management, and contextual understanding—are fully supported within the [_Chat Completion Agent_](./chat-completion-agent.md), making it a powerful tool for building conversational interfaces.

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
::: zone-end


> [!div class="nextstepaction"]
> [Exploring Chat Completion Agent](./chat-completion-agent.md)

