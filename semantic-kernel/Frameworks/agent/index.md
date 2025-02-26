---
title: Semantic Kernel Agent Framework (Experimental)
description: Introducing the Semantic Kernel Agent Framework
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# Semantic Kernel Agent Framework 

> [!WARNING]
> The _Semantic Kernel Agent Framework_ is in preview and is subject to change.

The _Semantic Kernel Agent Framework_ provides a platform within the Semantic Kernel eco-system that allow for the creation of AI **agents** and the ability to  incorporate **agentic patterns** into any application based on the same patterns and features that exist in the core _Semantic Kernel_ framework.

## What is an AI agent?

An **AI agent** is a software entity designed to perform tasks autonomously or semi-autonomously by recieving input, processing information, and taking actions to achieve specific goals.

Agents can send and receive messages, generating responses using a combination of models, tools, human inputs, or other customizable components. 

Agents are designed to work collaboratively, enabling complex workflows by interacting with each other.  The _Agent Framework_ allows for the creation of both simple and sophisticated agents, enhancing modularity and ease of maintenance


## What problems do AI agents solve?

AI agents offers several advantages for application development, particularly by enabling the creation of modular AI components that are able to collaborate to reduce manual intervention in complex tasks. AI agents can operate autonomously or semi-autonomously, making them powerful tools for a range of applications. 

Here are some of the key benefits:

- **Modular Components**: Allows  developers to define various types of agents for specific tasks (e.g., data scraping, API interaction, or natural language processing). This makes it easier to adapt the application as requirements evolve or new technologies emerge.

- **Collaboration**: Multiple agents may "collaborate" on tasks. For example, one agent might handle data collection while another analyzes it and yet another uses the results to make decisions, creating a more sophisticated system with distributed intelligence.

- **Human-Agent Collaboration**: Human-in-the-loop interactions allow agents to work alongside humans to augment decision-making processes. For instance, agents might prepare data analyses that humans can review and fine-tune, thus improving productivity.

- **Process Orchestration**: Agents can coordinate different tasks across systems, tools, and APIs, helping to automate end-to-end processes like application deployments, cloud orchestration, or even creative processes like writing and design.


## When to use an AI agent?

Using an agent framework for application development provides advantages that are especially beneficial for certain types of applications. While traditional AI models are often used as tools to perform specific tasks (e.g., classification, prediction, or recognition), agents introduce more autonomy, flexibility, and interactivity into the development process.

- **Autonomy and Decision-Making**: If your application requires entities that can make independent decisions and adapt to changing conditions (e.g., robotic systems, autonomous vehicles, smart environments), an agent framework is preferable.

- **Multi-Agent Collaboration**: If your application involves complex systems that require multiple independent components to work together (e.g., supply chain management, distributed computing, or swarm robotics), agents provide built-in mechanisms for coordination and communication.

- **Interactive and Goal-Oriented**: If your application involves goal-driven behavior (e.g., completing tasks autonomously or interacting with users to achieve specific objectives), agent-based frameworks are a better choice. Examples include virtual assistants, game AI, and task planners.


## How do I install the _Semantic Kernel Agent Framework_?

Installing the _Agent Framework SDK_ is specific to the distribution channel associated with your programming language.

::: zone pivot="programming-language-csharp"

For .NET SDK, serveral NuGet packages are available.  

> Note: The core _Semantic Kernel SDK_ is required in addition to any agent packages.


Package|Description
--|--
[Microsoft.SemanticKernel](https://www.nuget.org/packages/Microsoft.SemanticKernel)|This contains the core _Semantic Kernel_ libraries for getting started with the _Agent Framework_.  This must be explicitly referenced by your application.
[Microsoft.SemanticKernel.Agents.Abstractions](https://www.nuget.org/packages/Microsoft.SemanticKernel.Agents.Abstractions)|Defines the core agent abstractions for the _Agent Framework_.  Generally not required to be specified as it is included in both the `Microsoft.SemanticKernel.Agents.Core` and `Microsoft.SemanticKernel.Agents.OpenAI` packages.
[Microsoft.SemanticKernel.Agents.Core](https://www.nuget.org/packages/Microsoft.SemanticKernel.Agents.Core)|Includes the [`ChatCompletionAgent`](./chat-completion-agent.md) and [`AgentGroupChat`](./agent-chat.md) classes.
[Microsoft.SemanticKernel.Agents.OpenAI](https://www.nuget.org/packages/Microsoft.SemanticKernel.Agents.OpenAI)|Provides ability to use the [OpenAI Assistant API](https://platform.openai.com/docs/assistants) via the [`OpenAIAssistantAgent`](./assistant-agent.md).

::: zone-end

::: zone pivot="programming-language-python"

Module|Description
--|--
[semantic-kernel.agents](https://pypi.org/project/semantic-kernel/)|This is the _Semantic Kernel_ library for getting started with the _Agent Framework_.  This must be explicitly referenced by your application. This module contains the [`ChatCompletionAgent`](./chat-completion-agent.md) and [`AgentGroupChat`](./agent-chat.md) classes, as well as the ability to use the [OpenAI Assistant API](https://platform.openai.com/docs/assistants) via the [`OpenAIAssistantAgent` or `AzureOpenAssistant`](./assistant-agent.md).

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


> [!div class="nextstepaction"]
> [Agent Architecture](./agent-architecture.md)
