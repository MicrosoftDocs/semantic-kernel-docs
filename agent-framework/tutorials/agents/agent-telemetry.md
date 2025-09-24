---
title: Adding telemetry to Agents
description: Enable OpenTelemetry for an agent so agent interactions are automatically logged
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/18/2025
ms.service: semantic-kernel
---

# Adding telemetry to Agents

::: zone pivot="programming-language-csharp"

This tutorial shows how to enable OpenTelemetry on an agent so that interactions with the agent are automatically logged and exported.
In this tutorial, output is written to the console using the OpenTelemetry console exporter.

## Prerequisites

For prerequisites, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Installing Nuget packages

To use the AgentFramework with Azure OpenAI, you need to install the following NuGet packages:

```powershell
dotnet add package Azure.Identity
dotnet add package Azure.AI.OpenAI
dotnet add package Microsoft.Extensions.AI.OpenAI
dotnet add package Microsoft.Agents.OpenAI
```

To also add OpenTelemetry support, with support for writing to the console, install these additional packages:

```powershell
dotnet add package OpenTelemetry
dotnet add package OpenTelemetry.Exporter.Console
```

## Enable OpenTelemetry in your app

Enable the agent framework telemetry and create an OpenTelemetry TracerProvider that exports to the console.
Note that the tracerProvider must remain alive while you run the agent so traces are exported.

```csharp
using System;
using OpenTelemetry;
using OpenTelemetry.Trace;

// Enable agent framework telemetry
AppContext.SetSwitch("Microsoft.Extensions.AI.Agents.EnableTelemetry", true);

// Create a TracerProvider that exports to the console
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("agent-telemetry-source")
    .AddConsoleExporter()
    .Build();
```

## Create and instrument the agent

Create an agent, then call `WithOpenTelemetry` to provide a source name.
Note that the string literal "agent-telemetry-source" is the OpenTelemetry source name
that we used above, when we created the tracer provider.

```csharp
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI.Agents;

// Create the agent and enable OpenTelemetry instrumentation
AIAgent agent = new AzureOpenAIClient(
    new Uri("https://<myresource>.openai.azure.com"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o-mini")
        .CreateAIAgent(instructions: "You are good at telling jokes.", name: "Joker")
        .WithOpenTelemetry(sourceName: "agent-telemetry-source");
```

Run the agent and print the text response. The console exporter will show trace data on the console.

```csharp
Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));
```

The expected output will be something like this, where the agent invocation trace is shown first, followed by the text response from the agent.

```powershell
Activity.TraceId:            f2258b51421fe9cf4c0bd428c87b1ae4
Activity.SpanId:             2cad6fc139dcf01d
Activity.TraceFlags:         Recorded
Activity.DisplayName:        invoke_agent Joker
Activity.Kind:               Client
Activity.StartTime:          2025-09-18T11:00:48.6636883Z
Activity.Duration:           00:00:08.6077009
Activity.Tags:
    gen_ai.operation.name: invoke_agent
    gen_ai.system: openai
    gen_ai.agent.id: e1370f89-3ca8-4278-bce0-3a3a2b22f407
    gen_ai.agent.name: Joker
    gen_ai.request.instructions: You are good at telling jokes.
    gen_ai.response.id: chatcmpl-CH6fgKwMRGDtGNO3H88gA3AG2o7c5
    gen_ai.usage.input_tokens: 26
    gen_ai.usage.output_tokens: 29
Instrumentation scope (ActivitySource):
    Name: c8aeb104-0ce7-49b3-bf45-d71e5bf782d1
Resource associated with Activity:
    telemetry.sdk.name: opentelemetry
    telemetry.sdk.language: dotnet
    telemetry.sdk.version: 1.12.0
    service.name: unknown_service:Agent_Step08_Telemetry

Why did the pirate go to school?

Because he wanted to improve his "arrr-ticulation"! ?????
```

## Next steps

> [!div class="nextstepaction"]
> [Having a multi-turn conversation with an agent](./multi-turn-conversation.md)

::: zone-end
::: zone pivot="programming-language-python"

Tutorial coming soon.

::: zone-end
