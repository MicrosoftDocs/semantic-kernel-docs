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

## Creating an A2A Agent using the well known agent card location

First, let's look at a scenarios where we use the well known agent card location.
We pass the root URI of the A2A agent host to the `A2ACardResolver` constructor
and the resolver will look for the agent card at `https://your-a2a-agent-host/.well-known/agent-card.json`.

First, create an `A2ACardResolver` with the URI of the remote A2A agent host.

```csharp
using System;
using A2A;
using Microsoft.Extensions.AI.Agents;
using Microsoft.Extensions.AI.Agents.A2A;

A2ACardResolver agentCardResolver = new(new Uri("https://your-a2a-agent-host"));
```

Create an instance of the `AIAgent` for the remote A2A agent using the `GetAIAgentAsync` helper method.

```csharp
AIAgent agent = await agentCardResolver.GetAIAgentAsync();
```

## Creating an A2A Agent using the Direct Configuration / Private Discovery mechanism

It is also possible to point directly at the agent URL if it's known to us. This can be useful for tightly coupled systems, private agents, or development purposes, where clients are directly configured with Agent Card information and agent URL."

In this case we construct an `A2AClient` directly with the URL of the agent.

```csharp
A2AClient a2aClient = new(new Uri("https://your-a2a-agent-host/echo"));
```

And then we can create an instance of the `AIAgent` using the `GetAIAgent` method.

```csharp
AIAgent agent = a2aClient.GetAIAgent();
```

::: zone-end
::: zone pivot="programming-language-python"

Documentation coming soon.

::: zone-end

## Using the Agent

The agent is a standard `AIAgent` and supports all standard agent operations.

See the [Agent getting started tutorials](../../../tutorials/overview.md) for more information on how to run and interact with agents.
