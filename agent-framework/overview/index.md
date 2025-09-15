---
title: Introduction to Microsoft Agent Framework
description: Learn about Microsoft Agent Framework
author: markwallace
ms.topic: reference
ms.author: markwallace
ms.date: 09/15/2015
ms.service: agent-framework
---

# Introduction to Agent Framework
The Microsoft Agent Framework is a lightweight, open-source development kit that lets you easily build **AI agents** and **multi-agent workflows**.

There are two concepts that will be introduced here:

1. AI Agents
1. Multi-Agent Workflows

## AI Agents

### What is an AI agent?

An **AI agent** is a software entity designed to perform tasks autonomously or semi-autonomously by receiving input, processing information, and taking actions to achieve specific goals.

Agents can send and receive messages, generating responses using a combination of models, tools, human inputs, or other customizable components.

Agents are designed to work collaboratively, enabling complex workflows by interacting with each other.  The `Agent Framework` allows for the creation of both simple and sophisticated agents, enhancing modularity and ease of maintenance.

### What problems do AI agents solve?

AI agents offers several advantages for application development, particularly by enabling the creation of modular AI components that are able to collaborate to reduce manual intervention in complex tasks. AI agents can operate autonomously or semi-autonomously, making them powerful tools for a range of applications.

Here are some of the key benefits:

- **Modular Components**: Allows developers to define various types of agents for specific tasks (e.g., data scraping, API interaction, or natural language processing). This makes it easier to adapt the application as requirements evolve or new technologies emerge.

- **Collaboration**: Multiple agents may "collaborate" on tasks. For example, one agent might handle data collection while another analyzes it and yet another uses the results to make decisions, creating a more sophisticated system with distributed intelligence.

- **Human-Agent Collaboration**: Human-in-the-loop interactions allow agents to work alongside humans to augment decision-making processes. For instance, agents might prepare data analyses that humans can review and fine-tune, thus improving productivity.

- **Process Orchestration**: Agents can coordinate different tasks across systems, tools, and APIs, helping to automate end-to-end processes like application deployments, cloud orchestration, or even creative processes like writing and design.

### When to use an AI agent?

Using an agent framework for application development provides advantages that are especially beneficial for certain types of applications. While traditional AI models are often used as tools to perform specific tasks (e.g., classification, prediction, or recognition), agents introduce more autonomy, flexibility, and interactivity into the development process.

- **Autonomy and Decision-Making**: If your application requires entities that can make independent decisions and adapt to changing conditions (e.g., robotic systems, autonomous vehicles, smart environments), an agent framework is preferable.

- **Multi-Agent Collaboration**: If your application involves complex systems that require multiple independent components to work together (e.g., supply chain management, distributed computing, or swarm robotics), agents provide built-in mechanisms for coordination and communication.

- **Interactive and Goal-Oriented**: If your application involves goal-driven behavior (e.g., completing tasks autonomously or interacting with users to achieve specific objectives), agent-based frameworks are a better choice. Examples include virtual assistants, game AI, and task planners.

## Multi-Agent Workflows