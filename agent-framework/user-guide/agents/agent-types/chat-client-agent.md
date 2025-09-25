---
title: Agent based on any IChatClient
description: Learn how to use the Microsoft Agent Framework with any IChatClient implementation.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/25/2025
ms.service: semantic-kernel
---

# Agent based on any IChatClient

::: zone pivot="programming-language-csharp"

The Microsoft Agent Framework supports creating agents for any inference service that provides an `IChatClient` implementation. This means that there is a very broad range of services that can be used to create agents, including open source models that can be run locally.

In this document, we will use Ollama as an example.

## Getting Started

Add the required NuGet packages to your project.

```powershell
dotnet add package Microsoft.Extensions.AI.Agents
```

You will also need to add the package for the specific `IChatClient` implementation you want to use. In this example, we will use [OllamaSharp](https://www.nuget.org/packages/OllamaSharp/).

```powershell
dotnet add package OllamaSharp
```

## Creating a ChatClientAgent

To create an agent based on the `IChatClient` interface, you can use the `ChatClientAgent` class.
The `ChatClientAgent` class takes `IChatClient` as a constructor parameter.

First, create an `OllamaApiClient` to access the Ollama service.

```csharp
using System;
using Microsoft.Extensions.AI.Agents;
using OllamaSharp;

using OllamaApiClient chatClient = new(new Uri("http://localhost:11434"), "phi3");
```

The `OllamaApiClient` implements the `IChatClient` interface, so you can use it to create a `ChatClientAgent`.

```csharp
AIAgent agent = new ChatClientAgent(
    chatClient,
    instructions: "You are good at telling jokes.",
    name: "Joker");
```

> [!IMPORTANT]
> To ensure that you get the most out of your agent, make sure to choose a service and model that is well-suited for conversational tasks and supports function calling.

::: zone-end
::: zone pivot="programming-language-python"

Documentation coming soon.

::: zone-end

## Using the Agent

The agent is a standard `AIAgent` and supports all standard agent operations.

See the [Agent getting started tutorials](../../../tutorials/overview.md) for more information on how to run and interact with agents.
