---
title: "Ollama"
description: "Learn how to use Ollama as a provider for Agent Framework agents."
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: reference
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Ollama

Ollama allows you to run open-source models locally and use them with Agent Framework. This is ideal for development, testing, and scenarios where you need to keep data on-premises.

:::zone pivot="programming-language-csharp"

The following example shows how to create an agent using Ollama:

```csharp
using System;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// Create an Ollama agent using Microsoft.Extensions.AI.Ollama
// Requires: dotnet add package Microsoft.Extensions.AI.Ollama --prerelease
var chatClient = new OllamaChatClient(
    new Uri("http://localhost:11434"),
    modelId: "llama3.2");

AIAgent agent = chatClient.AsAIAgent(
    instructions: "You are a helpful assistant running locally via Ollama.");

Console.WriteLine(await agent.RunAsync("What is the largest city in France?"));
```

:::zone-end

:::zone pivot="programming-language-python"

> [!NOTE]
> Python support for Ollama is available through the OpenAI-compatible API. Use the `OpenAIChatClient` with a custom base URL pointing to your Ollama instance.

:::zone-end

## Next steps

> [!div class="nextstepaction"]
> [Providers Overview](./index.md)
