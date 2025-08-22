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

This article covers key concepts in the architecture of the Agent Framework, including foundational principles, design objectives, and strategic goals.

## Goals

The `Agent Framework` was developed with the following key priorities in mind:

- The Semantic Kernel agent framework serves as the core foundation for implementing agent functionalities.
- Multiple agents of different types can collaborate within a single conversation, each contributing their unique capabilities, while integrating human input.
- An agent can engage in and manage multiple concurrent conversations simultaneously.

## Agent

The abstract `Agent` class serves as the core abstraction for all types of agents, providing a foundational structure that can be extended to create more specialized agents. This base class forms the basis for more specific agent implementations, all of which leverage the Kernel's capabilities to execute their respective functions. See all the available agent types in the [Agent Types](#agent-types-in-semantic-kernel) section.

::: zone pivot="programming-language-csharp"

The underlying Semantic Kernel `Agent` abstraction can be found [here](/dotnet/api/microsoft.semantickernel.agents.agent).

::: zone-end

::: zone pivot="programming-language-python"

The underlying Semantic Kernel `Agent` abstraction can be found [here](/python/api/semantic-kernel/semantic_kernel.agents.agent).

::: zone-end

Agents can either be invoked directly to perform tasks or be orchestrated by different patterns. This flexible structure allows agents to adapt to various conversational or task-driven scenarios, providing developers with robust tools for building intelligent, multi-agent systems.

### Agent Types in Semantic Kernel

- [`ChatCompletionAgent`](./agent-types/chat-completion-agent.md)
- [`OpenAIAssistantAgent`](./agent-types/assistant-agent.md)
- [`AzureAIAgent`](./agent-types/azure-ai-agent.md)
- [`OpenAIResponsesAgent`](./agent-types/responses-agent.md)
- [`CopilotStudioAgent`](./agent-types/copilot-studio-agent.md)

## Agent Thread

The abstract `AgentThread` class serves as the core abstraction for threads or conversation state. It abstracts away the different ways in which conversation state may be managed for different agents.

Stateful agent services often store conversation state in the service, and you can interact with it via an id. Other agents may require the entire chat history to be passed to the agent on each invocation, in which case the conversation state is managed locally in the application.

Stateful agents typically only work with a matching `AgentThread` implementation, while other types of agents could work with more than one `AgentThread` type. For example, `AzureAIAgent` requires a matching `AzureAIAgentThread`. This is because the Azure AI Agent service stores conversations in the service, and requires specific service calls to create a thread and update it. If a different agent thread type is used with the `AzureAIAgent`, we fail fast due to an unexpected thread type and raise an exception to alert the caller.

## Agent Orchestration

> [!IMPORTANT]
> Agent Orchestration features in the Agent Framework are in the experimental stage. They are under active development and may change significantly before advancing to the preview or release candidate stage.

> [!NOTE]
> If you have been using the [`AgentGroupChat`](../../support/archive/agent-chat.md) orchestration pattern, please note that it is no longer maintained. We recommend developers to use the new [`GroupChatOrchestration`](./agent-orchestration/group-chat.md) pattern. A migration guide is provided [here](../../support/migration/group-chat-orchestration-migration-guide.md).

The [Agent Orchestration](./agent-orchestration/index.md) framework in Semantic Kernel enables the coordination of multiple agents to solve complex tasks collaboratively. It provides a flexible structure for defining how agents interact, share information, and delegate responsibilities. The core components and concepts include:

- **Orchestration Patterns:** Pre-built patterns such as Concurrent, Sequential, Handoff, Group Chat, and Magentic allow developers to choose the most suitable collaboration model for their scenario. Each pattern defines a different way for agents to communicate and process tasks (see the [Orchestration patterns](./agent-orchestration/index.md#supported-orchestration-patterns) table for details).
- **Data Transform Logic:** Input and output transforms allow orchestration flows to adapt data between agents and external systems, supporting both simple and complex data types.
- **Human-in-the-loop:** Some patterns support human-in-the-loop, enabling human agents to participate in the orchestration process. This is particularly useful for scenarios where human judgment or expertise is required.

This architecture empowers developers to build intelligent, multi-agent systems that can tackle real-world problems through collaboration, specialization, and dynamic coordination.

## Agent Alignment with Semantic Kernel Features

The `Agent Framework` is built on the foundational concepts and features that many developers have come to know within the Semantic Kernel ecosystem. These core principles serve as the building blocks for the Agent Framework’s design. By leveraging the familiar structure and capabilities of the Semantic Kernel, the Agent Framework extends its functionality to enable more advanced, autonomous agent behaviors, while maintaining consistency with the broader Semantic Kernel architecture. This ensures a smooth transition for developers, allowing them to apply their existing knowledge to create intelligent, adaptable agents within the framework.

### Plugins and Function Calling

[Plugins](./../../concepts/plugins/index.md) are a fundamental aspect of the Semantic Kernel, enabling developers to integrate custom functionalities and extend the capabilities of an AI application. These plugins offer a flexible way to incorporate specialized features or business-specific logic into the core AI workflows. Additionally, agent capabilities within the framework can be significantly enhanced by utilizing [Plugins](../../concepts/plugins/index.md) and leveraging [Function Calling](../../concepts/ai-services/chat-completion/function-calling/index.md). This allows agents to dynamically interact with external services or execute complex tasks, further expanding the scope and versatility of the AI system within diverse applications.

Learn how to configure agents to use plugins [here](./agent-functions.md).

### Agent Messages

Agent messaging, including both input and response, is built upon the core content types of the Semantic Kernel, providing a unified structure for communication. This design choice simplifies the process of transitioning from traditional chat-completion patterns to more advanced agent-driven patterns in your application development. By leveraging familiar Semantic Kernel content types, developers can seamlessly integrate agent capabilities into their applications without needing to overhaul existing systems. This streamlining ensures that as you evolve from basic conversational AI to more autonomous, task-oriented agents, the underlying framework remains consistent, making development faster and more efficient.

::: zone pivot="programming-language-csharp"

> [!TIP]
> API reference:
>
> - [`ChatHistory`](/dotnet/api/microsoft.semantickernel.chatcompletion.chathistory)
> - [`ChatMessageContent`](/dotnet/api/microsoft.semantickernel.chatmessagecontent)
> - [`KernelContent`](/dotnet/api/microsoft.semantickernel.kernelcontent)
> - [`StreamingKernelContent`](/dotnet/api/microsoft.semantickernel.streamingkernelcontent)
> - [`FileReferenceContent`](/dotnet/api/microsoft.semantickernel.filereferencecontent)
> - [`AnnotationContent`](/dotnet/api/microsoft.semantickernel.agents.openai.annotationcontent)

::: zone-end

::: zone pivot="programming-language-python"

> [!TIP]
> API reference:
>
> - [`chat_history`](/python/api/semantic-kernel/semantic_kernel.contents.chat_history)
> - [`chat_message_content`](/python/api/semantic-kernel/semantic_kernel.contents.chat_message_content)
> - [`kernel_content`](/python/api/semantic-kernel/semantic_kernel.contents.kernel_content)
> - [`streaming_chat_message_content`](/python/api/semantic-kernel/semantic_kernel.contents.streaming_chat_message_content)
> - [`file_reference_content`](/python/api/semantic-kernel/semantic_kernel.contents.file_reference_content)
> - [`annotation_content`](/python/api/semantic-kernel/semantic_kernel.contents.annotation_content)

::: zone-end

::: zone pivot="programming-language-java"

> [!TIP]
> API reference:
>
> - [`ChatHistory`](/java/api/com.microsoft.semantickernel.services.chatcompletion.chathistory)
> - [`ChatMessageContent`](/java/api/com.microsoft.semantickernel.services.chatcompletion.chatmessagecontent)
> - [`KernelContent`](/java/api/com.microsoft.semantickernel.services.kernelcontent)
> - [`StreamingKernelContent`](/java/api/com.microsoft.semantickernel.services.streamingkernelcontent)

::: zone-end

### Templating

An agent's role is primarily shaped by the instructions it receives, which dictate its behavior and actions. Similar to invoking a `Kernel` [prompt](../../concepts/prompts/index.md), an agent's instructions can include templated parameters—both values and functions—that are dynamically substituted during execution. This enables flexible, context-aware responses, allowing the agent to adjust its output based on real-time input.

Additionally, an agent can be configured directly using a Prompt Template Configuration, providing developers with a structured and reusable way to define its behavior. This approach offers a powerful tool for standardizing and customizing agent instructions, ensuring consistency across various use cases while still maintaining dynamic adaptability.

Learn more about how to create an agent with Semantic Kernel template [here](./agent-templates.md).

## Declarative Spec

::: zone pivot="programming-language-csharp"

> The documentation on using declarative specs is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

> [!IMPORTANT]
> This feature is in the experimental stage. Features at this stage are under development and subject to change before advancing to the preview or release candidate stage.

### Registering Custom Agent Types

To use a custom agent with the declarative YAML spec system, you must first register your agent class with the agent registry. This is required so that the `AgentRegistry` can recognize and construct your agent when parsing the `type:` field in the YAML spec.

To register a custom agent type, use the `@register_agent_type` decorator:

```python
from semantic_kernel.agents import register_agent_type, Agent, DeclarativeSpecMixin

@register_agent_type("custom_agent")
class CustomAgent(DeclarativeSpecMixin, Agent):
    ...
```

The string provided to the decorator (for example, `"custom_agent"`) must match the type: field in your YAML specification.

Once registered, your custom agent can be instantiated using the declarative pattern, for example via `AgentRegistry.create_from_yaml(...)`.

The `DeclarativeSpecMixin` adds support for methods such as `from_yaml`, `from_dict`, and `resolve_placeholders`, which allow your agent to be constructed from a YAML or dictionary specification:

```python
@classmethod
async def from_yaml(cls, yaml_str: str, *, kernel=None, plugins=None, prompt_template_config=None, settings=None, extras=None, **kwargs):
    # Resolves placeholders and loads YAML, then delegates to from_dict.
    ...

@classmethod
async def from_dict(cls, data: dict, *, kernel=None, plugins=None, prompt_template_config=None, settings=None, **kwargs):
    # Normalizes and passes spec fields to _from_dict.
    ...

@classmethod
@abstractmethod
async def _from_dict(cls, data: dict, *, kernel, prompt_template_config=None, **kwargs):
    # Subclasses implement this to create the agent from a dict.
    ...

@classmethod
def resolve_placeholders(cls, yaml_str: str, settings=None, extras=None) -> str:
    # Optional: override this to customize how environment or runtime placeholders are resolved in YAML.
    return yaml_str
```
> [!TIP]
> Any custom agent must inherit from `DeclarativeSpecMixin` to enable YAML-based construction and must be registered with the registry using `@register_agent_type`.

::: zone-end

::: zone pivot="programming-language-java"

> This feature is unavailable.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Explore the Common Agent Invocation API](./agent-api.md)
