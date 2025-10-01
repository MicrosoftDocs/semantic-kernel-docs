---
title: Enabling observability for Agents
description: Enable OpenTelemetry for an agent so agent interactions are automatically logged
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 09/18/2025
ms.service: semantic-kernel
---

# Enabling observability for Agents

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
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
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
using Microsoft.Agents.AI;
using OpenAI;

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
> [Persisting conversations](./persisted-conversation.md)

::: zone-end
::: zone pivot="programming-language-python"

This tutorial shows how to enable OpenTelemetry on an agent so that interactions with the agent are automatically logged and exported.
In this tutorial, output is written to the console using the OpenTelemetry console exporter.

## Prerequisites

For prerequisites, see the [Create and run a simple agent](./run-agent.md) step in this tutorial.

## Installing packages

To use the Agent Framework with Azure OpenAI, you need to install the following packages. The agent framework automatically includes all necessary OpenTelemetry dependencies:

```bash
pip install agent-framework
```

The following OpenTelemetry packages are included by default:
```text
opentelemetry-api
opentelemetry-sdk
azure-monitor-opentelemetry
azure-monitor-opentelemetry-exporter
opentelemetry-exporter-otlp-proto-grpc
opentelemetry-semantic-conventions-ai
```

## Enable OpenTelemetry in your app

The agent framework provides a convenient `setup_observability` function that configures OpenTelemetry with sensible defaults.
By default, it exports to the console if no specific exporter is configured.

```python
import asyncio
from agent_framework.observability import setup_observability

# Enable agent framework telemetry with console output (default behavior)
setup_observability(enable_sensitive_data=True)
```

### Understanding `setup_observability` parameters

The `setup_observability` function accepts the following parameters to customize your observability configuration:

- **`enable_otel`** (bool, optional): Enables OpenTelemetry tracing and metrics. Default is `False` when using environment variables only, but is assumed `True` when calling `setup_observability()` programmatically. When using environment variables, set `ENABLE_OTEL=true`.

- **`enable_sensitive_data`** (bool, optional): Controls whether sensitive data like prompts, responses, function call arguments, and results are included in traces. Default is `False`. Set to `True` to see actual prompts and responses in your traces. **Warning**: Be careful with this setting as it may expose sensitive data in your logs. Can also be set via `ENABLE_SENSITIVE_DATA=true` environment variable.

- **`otlp_endpoint`** (str, optional): The OTLP endpoint URL for exporting telemetry data. Default is `None`. Commonly set to `http://localhost:4317`. This creates an OTLPExporter for spans, metrics, and logs. Can be used with any OTLP-compliant endpoint such as [OpenTelemetry Collector](https://opentelemetry.io/docs/collector/), [Aspire Dashboard](/dotnet/aspire/fundamentals/dashboard/overview?tabs=bash), or other OTLP endpoints. Can also be set via `OTLP_ENDPOINT` environment variable.

- **`applicationinsights_connection_string`** (str, optional): Azure Application Insights connection string for exporting to Azure Monitor. Default is `None`. Creates AzureMonitorTraceExporter, AzureMonitorMetricExporter, and AzureMonitorLogExporter. You can find this connection string in the Azure portal under the "Overview" section of your Application Insights resource. Can also be set via `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable.

- **`vs_code_extension_port`** (int, optional): Port number for the AI Toolkit or Azure AI Foundry VS Code extension. Default is `4317`. Allows integration with VS Code extensions for local development and debugging. Can also be set via `VS_CODE_EXTENSION_PORT` environment variable.

- **`exporters`** (list, optional): Custom list of OpenTelemetry exporters for advanced scenarios. Default is `None`. Allows you to provide your own configured exporters when the standard options don't meet your needs.

### Setup options

You can configure observability in three ways:

**1. Environment variables** (simplest approach):
```bash
export ENABLE_OTEL=true
export ENABLE_SENSITIVE_DATA=true
export OTLP_ENDPOINT=http://localhost:4317
```

Then in your code:
```python
from agent_framework.observability import setup_observability

setup_observability()  # Reads from environment variables
```

**2. Programmatic configuration**:
```python
from agent_framework.observability import setup_observability

setup_observability(
    enable_sensitive_data=True,
    otlp_endpoint="http://localhost:4317",
    applicationinsights_connection_string="InstrumentationKey=your_key"
)
```

**3. Custom exporters** (for advanced scenarios):
```python
from agent_framework.observability import setup_observability
from opentelemetry.exporter.otlp.proto.grpc.trace_exporter import OTLPSpanExporter
from opentelemetry.sdk.trace.export import ConsoleSpanExporter

custom_exporters = [
    OTLPSpanExporter(endpoint="http://localhost:4317"),
    ConsoleSpanExporter()
]

setup_observability(exporters=custom_exporters, enable_sensitive_data=True)
```

The `setup_observability` function sets the global tracer provider and meter provider, allowing you to create custom spans and metrics:

```python
from agent_framework.observability import get_tracer, get_meter

tracer = get_tracer()
meter = get_meter()

with tracer.start_as_current_span("my_custom_span"):
    # Your code here
    pass

counter = meter.create_counter("my_custom_counter")
counter.add(1, {"key": "value"})
```

## Create and run the agent

Create an agent using the agent framework. The observability will be automatically enabled for the agent once `setup_observability` has been called.

```python
from agent_framework import ChatAgent
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity import AzureCliCredential

# Create the agent - telemetry is automatically enabled
agent = ChatAgent(
    chat_client=AzureOpenAIChatClient(
        credential=AzureCliCredential(),
        model="gpt-4o-mini"
    ),
    name="Joker",
    instructions="You are good at telling jokes."
)

# Run the agent
result = await agent.run("Tell me a joke about a pirate.")
print(result.text)
```

The console exporter will show trace data on the console similar to the following:

```text
{
    "name": "invoke_agent Joker",
    "context": {
        "trace_id": "0xf2258b51421fe9cf4c0bd428c87b1ae4",
        "span_id": "0x2cad6fc139dcf01d",
        "trace_state": "[]"
    },
    "kind": "SpanKind.CLIENT",
    "parent_id": null,
    "start_time": "2025-09-25T11:00:48.663688Z",
    "end_time": "2025-09-25T11:00:57.271389Z",
    "status": {
        "status_code": "UNSET"
    },
    "attributes": {
        "gen_ai.operation.name": "invoke_agent",
        "gen_ai.system": "openai",
        "gen_ai.agent.id": "Joker",
        "gen_ai.agent.name": "Joker",
        "gen_ai.request.instructions": "You are good at telling jokes.",
        "gen_ai.response.id": "chatcmpl-CH6fgKwMRGDtGNO3H88gA3AG2o7c5",
        "gen_ai.usage.input_tokens": 26,
        "gen_ai.usage.output_tokens": 29
    }
}
```

Followed by the text response from the agent:

```text
Why did the pirate go to school?

Because he wanted to improve his "arrr-ticulation"! ⛵
```

## Understanding the telemetry output

Once observability is enabled, the agent framework automatically creates the following spans:

- **`invoke_agent <agent_name>`**: The top-level span for each agent invocation. Contains all other spans as children and includes metadata like agent ID, name, and instructions.

- **`chat <model_name>`**: Created when the agent calls the underlying chat model. Includes the prompt and response as attributes when `enable_sensitive_data` is `True`, along with token usage information.

- **`execute_tool <function_name>`**: Created when the agent calls a function tool. Contains function arguments and results as attributes when `enable_sensitive_data` is `True`.

The following metrics are also collected:

**For chat operations:**
- `gen_ai.client.operation.duration` (histogram): Duration of each operation in seconds
- `gen_ai.client.token.usage` (histogram): Token usage in number of tokens

**For function invocations:**
- `agent_framework.function.invocation.duration` (histogram): Duration of each function execution in seconds

## Azure AI Foundry integration

If you're using Azure AI Foundry, there's a convenient method for automatic setup:

```python
from agent_framework.azure import AzureAIAgentClient
from azure.identity import AzureCliCredential

agent_client = AzureAIAgentClient(
    credential=AzureCliCredential(),
    project_endpoint="https://<your-project>.foundry.azure.com"
)

# Automatically configures observability with Application Insights
await agent_client.setup_azure_ai_observability()
```

This method retrieves the Application Insights connection string from your Azure AI Foundry project and calls `setup_observability` automatically.

## Next steps

For more advanced observability scenarios and examples, see the [Agent Observability user guide](../../user-guide/agents/agent-observability.md) and the [observability samples](https://github.com/microsoft/agent-framework/tree/main/python/samples/getting_started/observability) in the GitHub repository.

> [!div class="nextstepaction"]
> [Persisting conversations](./persisted-conversation.md)

::: zone-end

