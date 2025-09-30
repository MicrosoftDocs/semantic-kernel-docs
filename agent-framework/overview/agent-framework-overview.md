---
title: Introduction to Microsoft Agent Framework
description: Learn about Microsoft Agent Framework
author: markwallace
ms.topic: reference
ms.author: markwallace
ms.date: 09/15/2025
ms.service: semantic-kernel
---

# Introduction to Agent Framework

The Microsoft Agent Framework is a lightweight, open-source development kit that lets you easily build **AI agents** and **multi-agent workflows**.

There are two concepts that will be introduced here:

1. [AI Agents](#ai-agents)
2. [Workflows](#workflows)

> [!IMPORTANT]
> If you use the Microsoft Agent Framework to build applications that operate with third-party servers or agents, you do so at your own risk. We recommend reviewing all data being shared with third-party servers or agents and being cognizant of third-party practices for retention and location of data. It is your responsibility to manage whether your data will flow outside of your organization’s Azure compliance and geographic boundaries and any related implications. 

## AI Agents

### What is an AI agent?

An **AI agent** is a software entity designed to perform tasks autonomously or semi-autonomously by receiving input, processing information, and taking actions to achieve specific goals.

Agents can send and receive messages, generating responses using a combination of models, tools, human inputs, or other customizable components.

Agents are designed to work collaboratively, enabling complex workflows by interacting with each other.  The `Agent Framework` allows for the creation of both simple and sophisticated agents, enhancing modularity and ease of maintenance.

### What problems do AI agents solve?

AI agents offer several advantages for application development, particularly by enabling the creation of modular AI components that are able to collaborate to reduce manual intervention in complex tasks. AI agents can operate autonomously or semi-autonomously, making them powerful tools for a range of applications.

Here are some of the key benefits:

- **Modular Components**: Allows developers to separate agents for specific tasks (e.g., data scraping, API interaction, or natural language processing). This separation makes it easier to adapt the application as requirements evolve or new technologies emerge.

- **Collaboration**: Multiple agents may "collaborate" on tasks. For example, one agent might handle data collection while another analyzes it and yet another uses the results to make decisions, creating a more sophisticated system with distributed intelligence.

- **Human-Agent Collaboration**: Human-in-the-loop interactions allow agents to work alongside humans to augment decision-making processes. For instance, agents might prepare data analyses that humans can review and fine-tune, thus improving productivity.

- **Process Orchestration**: Agents can coordinate different tasks across systems, tools, and APIs, helping to automate end-to-end processes like application deployments, cloud orchestration, or even creative processes like writing and design.

### When to use an AI agent?

Using an AI agent for application development provides advantages that are especially beneficial for certain types of applications. While LLM's are often used as tools to perform specific tasks (e.g., classification, prediction, or recognition), agents introduce more autonomy, flexibility, and interactivity into the development process.

- **Autonomy and Decision-Making**: If your application requires entities that can make independent decisions and adapt to changing conditions (e.g., robotic systems, autonomous vehicles, smart environments), an AI agent is preferable.

- **Multi-Agent Collaboration**: If your application involves complex systems that require multiple independent components to work together (e.g., supply chain management, distributed computing, or swarm robotics), agents provide built-in mechanisms for coordination and communication.

- **Interactive and Goal-Oriented**: If your application involves goal-driven behavior (e.g., completing tasks autonomously or interacting with users to achieve specific objectives), agent-based frameworks are a better choice. Examples include virtual assistants, game AI, and task planners.

## Workflows

### What is a Workflow?

A **workflow** is a predefined sequence of operations that can include AI agents as components while maintaining consistency and reliability. Workflows are designed to handle complex and long-running business processes that may involve multiple agents, human interactions, and integrations with external systems.

The flow of a workflow is explicitly defined, allowing for more control over the execution path. Workflows can include conditional routing, parallel processing, and dynamic execution paths.

### What problems do Workflows solve?

Workflows provide a structured way to manage complex processes that involve multiple steps, decision points, and interactions with various systems or agents. The types of tasks workflows are designed to handle often require more than one AI agent.

Here are some of the key benefits of `Agent Framework` workflows:

- **Modularity**: Workflows can be broken down into smaller, reusable components, making it easier to manage and update individual parts of the process.
- **Agent Integration**: Workflows can incorporate multiple AI agents with non-agentic components, allowing for sophisticated orchestration of tasks.
- **Type Safety**: Strong typing ensures messages flow correctly between components, with comprehensive validation that prevents runtime errors.
- **Flexible Control Flow**: Graph-based architecture allows for intuitive modeling of complex workflows with `executors` and `edges`. Conditional routing, parallel processing, and dynamic execution paths are all supported.
- **External Integration**: Built-in request/response patterns for seamless integration with external APIs, and human-in-the-loop scenarios.
- **Checkpointing**: Save workflow states via checkpoints, enabling recovery and resumption of long-running processes on server sides.
- **Multi-Agent Orchestration**: Built-in patterns for coordinating multiple AI agents, including sequential, concurrent, hand-off, and magentic.
- **Composability**: Workflows can be nested or combined to create more complex processes, allowing for scalability and adaptability.

## Next steps

> [!div class="nextstepaction"]
> [Quickstart](../tutorials/quick-start.md)
>[Migration Guide from Semantic Kernel](../migration-guide/from-semantic-kernel/index.md)
>[Migration Guide from AutoGen](../migration-guide/from-auto-genl/index.md)
