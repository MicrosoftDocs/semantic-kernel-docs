---
title: OpenAI Responses Agents
description: Learn how to use the Microsoft Agent Framework with OpenAI Responses service.
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# OpenAI Responses Agents

The Microsoft Agent Framework supports creating agents that use the [OpenAI responses](https://platform.openai.com/docs/api-reference/responses/create) service.

::: zone pivot="programming-language-csharp"

## Getting Started

Add the required NuGet packages to your project.

```powershell
dotnet add package Microsoft.Extensions.AI.OpenAI --prerelease
```

## Creating an OpenAI Responses Agent

As a first step you need to create a client to connect to the OpenAI service.

```csharp
using System;
using Microsoft.Extensions.AI.Agents;
using OpenAI;

OpenAIClient client = new OpenAIClient("<your_api_key>");
```

OpenAI supports multiple services that all provide model calling capabilities.
We need to pick the Responses service to create a Responses based agent.

```csharp
var responseClient = client.GetOpenAIResponseClient("gpt-4o-mini");
```

Finally, create the agent using the `CreateAIAgent` extension method on the `ResponseClient`.

```csharp
AIAgent agent = responseClient.CreateAIAgent(
    instructions: "You are good at telling jokes.",
    name: "Joker");
```

::: zone-end
::: zone pivot="programming-language-python"

Documentation coming soon.

::: zone-end

## Using the Agent

The agent is a standard `AIAgent` and supports all standard `AIAgent` operations.

See the [Agent getting started tutorials](../../../tutorials/index.md#agent-getting-started-tutorials) for more information on how to run and interact with agents.
