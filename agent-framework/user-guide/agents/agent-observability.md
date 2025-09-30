---
title: Agent Observability
description: Learn how to use observability with Agent Framework
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: reference
ms.author: edvan
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# Agent Observability

::: zone pivot="programming-language-csharp"

Documentation coming soon.


::: zone-end
::: zone pivot="programming-language-python"

Observability is a key concern for any agent framework, and so we decided to ensure we make it easy to setup and completely follow the latest OpenTelemetry standards for GenAI, for more info on those standards, see the [OpenTelemetry for AI](https://opentelemetry.io/docs/specs/semconv/gen-ai/) documentation.

To enable OpenTelemetry in your python application, you do not need to install anything extra, by default the following package are installed:
```text
"opentelemetry-api",
"opentelemetry-sdk",
"azure-monitor-opentelemetry",
"azure-monitor-opentelemetry-exporter",
"opentelemetry-exporter-otlp-proto-grpc",
"opentelemetry-semantic-conventions-ai",
```
You can then enable OpenTelemetry in your application by following the steps below.

## Enable OpenTelemetry in your app

The easiest way to enable OpenTelemetry is to setup using the environment variables below. After those have been set, all you need to do is call at the start of your program:
```python
from agent_framework.observability import setup_observability

setup_observability()
```

This will take the environment variables into account and setup OpenTelemetry accordingly, it will set the global tracer provider and meter provider, so you can start using it right away, for instance to create custom spans or metrics:

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
The easiest way to enable OpenTelemetry is to set the following environment variables:

- ENABLE_OTEL
    Default is `false`, set to `true` to enable OpenTelemetry
    This is needed for the basic setup, but also to visualize the workflows.
- ENABLE_SENSITIVE_DATA
    Default is `false`, set to `true` to enable logging of sensitive data, such as prompts, responses, function call arguments and results.
    This is needed if you want to see the actual prompts and responses in your traces.
    Be careful with this setting, as it may expose sensitive data in your logs.
- OTLP_ENDPOINT
    Default is `None`, set to your host for otel, often: `http://localhost:4317`
    This can be used for any compliant OTLP endpoint, such as [OpenTelemetry Collector](https://opentelemetry.io/docs/collector/), [Aspire Dashboard](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/overview?tabs=bash) or any other OTLP compliant endpoint.
- APPLICATIONINSIGHTS_CONNECTION_STRING
    Default is `None`, set to your Application Insights connection string to export to Azure Monitor.
    You can find the connection string in the Azure portal, in the "Overview" section of your Application Insights resource.
- VS_CODE_EXTENSION_PORT
    Default is `4317`, set to the port the AI Toolkit or AzureAI Foundry VS Code extension is running on.

### Programmatic setup

If you prefer to set up OpenTelemetry programmatically, you can do so by calling the `setup_observability` function with the desired configuration options:

```python
from agent_framework.observability import setup_observability

setup_observability(
    enable_sensitive_data=True,
    otlp_endpoint="http://localhost:4317",
    applicationinsights_connection_string="InstrumentationKey=your_instrumentation_key",
    vs_code_extension_port=4317
)
```

This will take the provided configuration options and set up OpenTelemetry accordingly. It will assume you mean to enable the tracing, so `enable_otel` is implicitly set to `True`. If you also have endpoints or connection strings set via environment variables, those will also be created, and we check if there is no doubling.

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

```python
from agent_framework.azure import AzureAIAgentClient
from azure.identity import AzureCliCredential

agent_client = AzureAIAgentClient(credential=AzureCliCredential(), project_endpoint="https://<your-project>.foundry.azure.com")

await agent_client.setup_azure_ai_observability()
```

This is a convenience method, that will use the project client, to get the Application Insights connection string, and then call `setup_observability` with that connection string.

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

# Getting started

We have a number of samples in our repository that demonstrate these capabilities, see the [observability samples folder ](https://github.com/microsoft/agent-framework/tree/main/python/samples/getting_started/observability) on Github. That includes samples for using zero-code telemetry as well.

::: zone-end
