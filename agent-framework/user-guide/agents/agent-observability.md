---
title: Agent Observability
description: Learn how to use observability with Agent Framework
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: reference
ms.author: edvan
ms.date: 09/24/2025
ms.service: agent-framework
---

# Agent Observability

Observability is a key aspect of building reliable and maintainable systems. Agent Framework provides built-in support for observability, allowing you to monitor the behavior of your agents.

This guide will walk you through the steps to enable observability with Agent Framework to help you understand how your agents are performing and diagnose any issues that may arise.

## OpenTelemetry Integration

Agent Framework integrates with [OpenTelemetry](https://opentelemetry.io/), and more specifically Agent Framework emits traces, logs, and metrics according to the [OpenTelemetry GenAI Semantic Conventions](https://opentelemetry.io/docs/specs/semconv/gen-ai/).

::: zone pivot="programming-language-csharp"

## Enable Observability (C#)

To enable observability for your chat client, you need to build the chat client as follows:

```csharp
// Using the Azure OpenAI client as an example
var instrumentedChatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient() // Converts a native OpenAI SDK ChatClient into a Microsoft.Extensions.AI.IChatClient
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "MyApplication", configure: (cfg) => cfg.EnableSensitiveData = true)    // Enable OpenTelemetry instrumentation with sensitive data
    .Build();
```

To enable observability for your agent, you need to build the agent as follows:

```csharp
var agent = new ChatClientAgent(
    instrumentedChatClient,
    name: "OpenTelemetryDemoAgent",
    instructions: "You are a helpful assistant that provides concise and informative responses.",
    tools: [AIFunctionFactory.Create(GetWeatherAsync)]
).WithOpenTelemetry(sourceName: "MyApplication", enableSensitiveData: true);    // Enable OpenTelemetry instrumentation with sensitive data
```

> [!IMPORTANT]
> When you enable observability for your chat clients and agents, you may see duplicated information, especially when sensitive data is enabled. The chat context (including prompts and responses) that is captured by both the chat client and the agent will be included in both spans. Depending on your needs, you may choose to enable observability only on the chat client or only on the agent to avoid duplication. See the [GenAI Semantic Conventions](https://opentelemetry.io/docs/specs/semconv/gen-ai/) for more details on the attributes captured for LLM and Agents.

> [!NOTE]
> Only enable sensitive data in development or testing environments, as it may expose user information in production logs and traces. Sensitive data includes prompts, responses, function call arguments, and results.

### Configuration

Now that your chat client and agent are instrumented, you can configure the OpenTelemetry exporters to send the telemetry data to your desired backend.

#### Traces

To export traces to the desired backend, you can configure the OpenTelemetry SDK in your application startup code. For example, to export traces to an Azure Monitor resource:

```csharp
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System;

var SourceName = "MyApplication";

var applicationInsightsConnectionString = Environment.GetEnvironmentVariable("APPLICATION_INSIGHTS_CONNECTION_STRING")
    ?? throw new InvalidOperationException("APPLICATION_INSIGHTS_CONNECTION_STRING is not set.");

var resourceBuilder = ResourceBuilder
    .CreateDefault()
    .AddService(ServiceName);

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource(SourceName)
    .AddSource("*Microsoft.Extensions.AI") // Listen to the Experimental.Microsoft.Extensions.AI source for chat client telemetry
    .AddSource("*Microsoft.Extensions.Agents*") // Listen to the Experimental.Microsoft.Extensions.Agents source for agent telemetry
    .AddAzureMonitorTraceExporter(options => options.ConnectionString = applicationInsightsConnectionString)
    .Build();
```

> [!TIP]
> Depending on your backend, you can use different exporters, see the [OpenTelemetry .NET documentation](https://opentelemetry.io/docs/instrumentation/net/exporters/) for more information. For local development, consider using the [Aspire Dashboard](#aspire-dashboard).

#### Metrics

Similarly, to export metrics to the desired backend, you can configure the OpenTelemetry SDK in your application startup code. For example, to export metrics to an Azure Monitor resource:

```csharp
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System;

var applicationInsightsConnectionString = Environment.GetEnvironmentVariable("APPLICATION_INSIGHTS_CONNECTION_STRING")
    ?? throw new InvalidOperationException("APPLICATION_INSIGHTS_CONNECTION_STRING is not set.");

var resourceBuilder = ResourceBuilder
    .CreateDefault()
    .AddService(ServiceName);

using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource(SourceName)
    .AddMeter("*Microsoft.Agents.AI") // Agent Framework metrics
    .AddAzureMonitorMetricExporter(options => options.ConnectionString = applicationInsightsConnectionString)
    .Build();
```

#### Logs

Logs are captured via the logging framework you are using, for example `Microsoft.Extensions.Logging`. To export logs to an Azure Monitor resource, you can configure the logging provider in your application startup code:

```csharp
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Logging;

var applicationInsightsConnectionString = Environment.GetEnvironmentVariable("APPLICATION_INSIGHTS_CONNECTION_STRING")
    ?? throw new InvalidOperationException("APPLICATION_INSIGHTS_CONNECTION_STRING is not set.");

using var loggerFactory = LoggerFactory.Create(builder =>
{
    // Add OpenTelemetry as a logging provider
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(resourceBuilder);
        options.AddAzureMonitorLogExporter(options => options.ConnectionString = applicationInsightsConnectionString);
        // Format log messages. This is default to false.
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
    })
    .SetMinimumLevel(LogLevel.Debug);
});

// Create a logger instance for your application
var logger = loggerFactory.CreateLogger<Program>();
```

## Aspire Dashboard

Consider using the Aspire Dashboard as a quick way to visualize your traces and metrics during development. To Learn more, see [Aspire Dashboard documentation](/dotnet/aspire/fundamentals/dashboard/overview). The Aspire Dashboard receives data via an OpenTelemetry Collector, which you can add to your tracer provider as follows:

```csharp
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource(SourceName)
    .AddSource("*Microsoft.Extensions.AI") // Listen to the Experimental.Microsoft.Extensions.AI source for chat client telemetry
    .AddSource("*Microsoft.Extensions.Agents*") // Listen to the Experimental.Microsoft.Extensions.Agents source for agent telemetry
    .AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"))
    .Build();
```

## Getting started

See a full example of an agent with OpenTelemetry enabled in the [Agent Framework repository](https://github.com/microsoft/agent-framework/tree/taochen/dotnet-workflow-observability-samples/dotnet/samples/GettingStarted/AgentOpenTelemetry).

::: zone-end

::: zone pivot="programming-language-python"

## Enable Observability (Python)

To enable observability in your python application, in most cases you do not need to install anything extra, by default the following package are installed:

```text
"opentelemetry-api",
"opentelemetry-sdk",
"opentelemetry-exporter-otlp-proto-grpc",
"opentelemetry-semantic-conventions-ai",
```

The easiest way to enable observability is to setup using the environment variables below. After those have been set, all you need to do is call at the start of your program:

```python
from agent_framework.observability import setup_observability

setup_observability()
```

This will take the environment variables into account and setup observability accordingly, it will set the global tracer provider and meter provider, so you can start using it right away, for instance to create custom spans or metrics:

```python
from agent_framework.observability import get_tracer, get_meter

tracer = get_tracer()
meter = get_meter()
with tracer.start_as_current_span("my_custom_span"):
    # do something
    pass
counter = meter.create_counter("my_custom_counter")
counter.add(1, {"key": "value"})
```

Those are wrappers of the OpenTelemetry API, that will return a tracer or meter from the global provider, but with `agent_framework` set as the instrumentation library name, unless you override the name.

For `otlp_endpoints`, these will be created as a OTLPExporter, one each for span, metrics and logs. The `connection_string` for Application Insights will be used to create an AzureMonitorTraceExporter, AzureMonitorMetricExporter and AzureMonitorLogExporter.

### Environment variables

The easiest way to enable observability for your application is to set the following environment variables:

- ENABLE_OTEL
    Default is `false`, set to `true` to enable OpenTelemetry
    This is needed for the basic setup, but also to visualize the workflows.
- ENABLE_SENSITIVE_DATA
    Default is `false`, set to `true` to enable logging of sensitive data, such as prompts, responses, function call arguments and results.
    This is needed if you want to see the actual prompts and responses in your traces.
    Be careful with this setting, as it may expose sensitive data in your logs.
- OTLP_ENDPOINT
    Default is `None`, set to your host for otel, often: `http://localhost:4317`
    This can be used for any compliant OTLP endpoint, such as [OpenTelemetry Collector](https://opentelemetry.io/docs/collector/), [Aspire Dashboard](/dotnet/aspire/fundamentals/dashboard/overview?tabs=bash) or any other OTLP compliant endpoint.
- APPLICATIONINSIGHTS_CONNECTION_STRING
    Default is `None`, set to your Application Insights connection string to export to Azure Monitor.
    You can find the connection string in the Azure portal, in the "Overview" section of your Application Insights resource. This will require the `azure-monitor-opentelemetry-exporter` package to be installed.
- VS_CODE_EXTENSION_PORT
    Default is `4317`, set to the port the AI Toolkit or AzureAI Foundry VS Code extension is running on.

### Programmatic setup

If you prefer to set up observability programmatically, you can do so by calling the `setup_observability` function with the desired configuration options:

```python
from agent_framework.observability import setup_observability

setup_observability(
    enable_sensitive_data=True,
    otlp_endpoint="http://localhost:4317",
    applicationinsights_connection_string="InstrumentationKey=your_instrumentation_key",
    vs_code_extension_port=4317
)
```

This will take the provided configuration options and set up observability accordingly. It will assume you mean to enable the tracing, so `enable_otel` is implicitly set to `True`. If you also have endpoints or connection strings set via environment variables, those will also be created, and we check if there is no doubling.

### Custom exporters

If you want to have different exporters, then the standard ones above, or if you want to customize the setup further, you can do so by creating your own tracer provider and meter provider, and then passing those to the `setup_observability` function, for example:

```python
from agent_framework.observability import setup_observability
from opentelemetry.exporter.otlp.proto.grpc.trace_exporter import OTLPSpanExporter

custom_span_exporter = OTLPSpanExporter(endpoint="http://localhost:4317", timeout=5, compression=Compression.Gzip)

setup_observability(exporters=[custom_span_exporter])
```

### Azure AI Foundry setup

Azure AI Foundry has built-in support for tracing, with a really great visualization for your spans.

When you have a Azure AI Foundry project setup with a Application Insights resource, you can do the following:

1) Install the `azure-monitor-opentelemetry-exporter` package:

```bash
pip install azure-monitor-opentelemetry-exporter>=1.0.0b41
```

2) Then you can setup observability for your Azure AI Foundry project as follows:

```python
from agent_framework.azure import AzureAIAgentClient
from agent_framework.observability import setup_observability
from azure.ai.projects.aio import AIProjectClient
from azure.identity import AzureCliCredential

async def main():
     async with AIProjectClient(credential=AzureCliCredential(), project_endpoint="https://<your-project>.foundry.azure.com") as project_client:
        try:
            conn_string = await project_client.telemetry.get_application_insights_connection_string()
            setup_observability(applicationinsights_connection_string=conn_string, enable_sensitive_data=True)
        except ResourceNotFoundError:
            print("No Application Insights connection string found for the Azure AI Project.")
```

This is a convenience method, that will use the project client, to get the Application Insights connection string, and then call `setup_observability` with that connection string, overriding any existing connection string set via environment variable.

### Zero-code instrumentation

Because we use the standard OpenTelemetry SDK, you can also use zero-code instrumentation to instrument your application, run you code like this:

```bash
opentelemetry-instrument \
    --traces_exporter console,otlp \
    --metrics_exporter console \
    --service_name your-service-name \
    --exporter_otlp_endpoint 0.0.0.0:4317 \
    python agent_framework_app.py
```

See the [OpenTelemetry Zero-code Python documentation](https://opentelemetry.io/docs/zero-code/python/) for more information and details of the environment variables used.

## Spans and metrics

Once everything is setup, you will start seeing spans and metrics being created automatically for you, the spans are:

- `invoke_agent <agent_name>`: This is the top level span for each agent invocation, it will contain all other spans as children.
- `chat <model_name>`: This span is created when the agent calls the underlying chat model, it will contain the prompt and response as attributes, if `enable_sensitive_data` is set to `True`.
- `execute_tool <function_name>`: This span is created when the agent calls a function tool, it will contain the function arguments and result as attributes, if `enable_sensitive_data` is set to `True`.

The metrics that are created are:

- For the chat client and `chat` operations:
    - `gen_ai.client.operation.duration` (histogram): This metric measures the duration of each operation, in seconds.
    - `gen_ai.client.token.usage` (histogram): This metric measures the token usage, in number of tokens.

- For function invocation during the `execute_tool` operations:
    - `agent_framework.function.invocation.duration` (histogram): This metric measures the duration of each function execution, in seconds.

### Example trace output

When you run an agent with observability enabled, you'll see trace data similar to the following console output:

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

This trace shows:

- **Trace and span identifiers**: For correlating related operations
- **Timing information**: When the operation started and ended
- **Agent metadata**: Agent ID, name, and instructions
- **Model information**: The AI system used (OpenAI) and response ID
- **Token usage**: Input and output token counts for cost tracking

## Samples

We have a number of samples in our repository that demonstrate these capabilities, see the [observability samples folder](https://github.com/microsoft/agent-framework/tree/main/python/samples/getting_started/observability) on Github. That includes samples for using zero-code telemetry as well.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Background Responses](./agent-background-responses.md)
