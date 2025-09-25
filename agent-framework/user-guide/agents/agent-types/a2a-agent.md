---
title: A2A Agents
description: Learn how to use the Microsoft Agent Framework with a remote A2A service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# A2A Agents

The Microsoft Agent Framework supports using a remote agent that is exposed via the A2A protocol in your application using the same `AIAgent` abstraction as any other agent.

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```powershell
dotnet add package Microsoft.Extensions.AI.Agents.A2A --prerelease
```

## Creating an A2A Agent

You can create an agent that connects to a remote A2A service using the `A2ACardResolver` class.

First, create an `A2ACardResolver` with the URI of the remote A2A agent host.

```csharp
using System;
using A2A;
using Microsoft.Extensions.AI.Agents;
using Microsoft.Extensions.AI.Agents.A2A;

A2ACardResolver agentCardResolver = new(new Uri("https://a2a.example.com/agent/card"));
```

Create an instance of the `AIAgent` for the remote A2A agent using the `GetAIAgentAsync` helper method.

```csharp
AIAgent agent = await agentCardResolver.GetAIAgentAsync();
```

::: zone-end
::: zone pivot="programming-language-python"

Documentation coming soon.

::: zone-end

## Using the Agent

The agent is a standard `AIAgent` and supports all standard agent operations.

See the [Agent getting started tutorials](../../../tutorials/index.md#agent-getting-started-tutorials) for more information on how to run and interact with agents.
