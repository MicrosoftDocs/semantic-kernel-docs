---
title: Semantic Kernel Telemetry
description: Learn about Telemetry in Semantic Kernel.
author: sophialagerkranspandey
ms.topic: conceptual
ms.author: sopand
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# What is Telemetry?
Telemetry in Semantic Kernel (SK) .NET implementation includes logging, metering and tracing. The code is instrumented using native .NET instrumentation tools, which means that it's possible to use different monitoring platforms (e.g. Application Insights, Aspire dashboard, Prometheus, Grafana etc.).

Code example using Application Insights can be found [here](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Demos/TelemetryWithAppInsights).

# Logging
The logging mechanism in this project relies on the ILogger interface from the Microsoft.Extensions.Logging namespace. Recent updates have introduced enhancements to the logger creation process. Instead of directly using the ILogger interface, instances of ILogger are now recommended to be created through an ILoggerFactory provided to components using the WithLoggerFactory method.

By employing the WithLoggerFactory approach, logger instances are generated with precise type information, facilitating more accurate logging and streamlined control over log filtering across various classes.

Log levels used in SK:

- Trace - this type of logs should not be enabled in production environments, since it may contain sensitive data. It can be useful in test environments for better observability. Logged information includes:
    - Goal/Ask to create a plan
    - Prompt (template and rendered version) for AI to create a plan
    - Created plan with function arguments (arguments may contain sensitive data)
    - Prompt (template and rendered version) for AI to execute a function
    - Arguments to functions (arguments may contain sensitive data)
- Debug - contains more detailed messages without sensitive data. Can be enabled in production environments.
- Information (default) - log level that is enabled by default and provides information about general flow of the application. Contains following data:
    - AI model used to create a plan
    - Plan creation status (Success/Failed)
    - Plan creation execution time (in seconds)
    - Created plan without function arguments
    - AI model used to execute a function
    - Function execution status (Success/Failed)
    - Function execution time (in seconds)
- Warning - includes information about unusual events that don't cause the application to fail.
- Error - used for logging exception details.

Examples
Enable logging for Kernel instance:

```csharp
var kernel = new KernelBuilder().WithLoggerFactory(loggerFactory);
```
All kernel functions and planners will be instrumented. It includes logs, metering and tracing.

# Log Filtering Configuration
Log filtering configuration has been refined to strike a balance between visibility and relevance:

```csharp
// Add OpenTelemetry as a logging provider
builder.AddOpenTelemetry(options =>
{
  options.AddAzureMonitorLogExporter(options => options.ConnectionString = connectionString);
  // Format log messages. This is default to false.
  options.IncludeFormattedMessage = true;
});
builder.AddFilter("Microsoft", LogLevel.Warning);
builder.AddFilter("Microsoft.SemanticKernel", LogLevel.Critical);
builder.AddFilter("Microsoft.SemanticKernel.Reliability", LogLevel.Information);
```

Read more at: https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/logs/customizing-the-sdk/README.md

# Metering
Metering is implemented with Meter class from System.Diagnostics.Metrics namespace.

Available meters:

- Microsoft.SemanticKernel.Planning - contains all metrics related to planning. List of metrics:
    - semantic_kernel.planning.create_plan.duration (Histogram) - execution time of plan creation (in seconds)
    - semantic_kernel.planning.invoke_plan.duration (Histogram) - execution time of plan execution (in seconds)
- Microsoft.SemanticKernel - captures metrics for KernelFunction. List of metrics:
    - semantic_kernel.function.invocation.duration (Histogram) - function execution time (in seconds)
    - semantic_kernel.function.streaming.duration (Histogram) - function streaming execution time (in seconds)
    - semantic_kernel.function.invocation.token_usage.prompt (Histogram) - number of prompt token usage (only for KernelFunctionFromPrompt)
    - semantic_kernel.function.invocation.token_usage.completion (Histogram) - number of completion token usage (only for KernelFunctionFromPrompt)
- Microsoft.SemanticKernel.Connectors.OpenAI - captures metrics for OpenAI functionality. List of metrics:
    - semantic_kernel.connectors.openai.tokens.prompt (Counter) - number of prompt tokens used.
    - semantic_kernel.connectors.openai.tokens.completion (Counter) - number of completion tokens used.
    - semantic_kernel.connectors.openai.tokens.total (Counter) - total number of tokens used.

Measurements will be associated with tags that will allow data to be categorized for analysis:

```csharp
TagList tags = new() { { "semantic_kernel.function.name", this.Name } };
s_invocationDuration.Record(duration.TotalSeconds, in tags);
```
Depending on monitoring tool, there are different ways how to subscribe to available meters. Following example shows how to subscribe to available meters and export metrics to Application Insights using OpenTelemetry.Sdk:

```csharp
using var meterProvider = Sdk.CreateMeterProviderBuilder()
  .AddMeter("Microsoft.SemanticKernel*")
  .AddAzureMonitorMetricExporter(options => options.ConnectionString = connectionString)
  .Build();
  ```
Read more at: [https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable?tabs=net](https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable?tabs=net)

Read more at: [https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/metrics/customizing-the-sdk/README.md](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/metrics/customizing-the-sdk/README.md)

# Tracing
Tracing is implemented with Activity class from System.Diagnostics namespace.

Available activity sources:

- Microsoft.SemanticKernel.Planning - creates activities for all planners. 
- Microsoft.SemanticKernel - creates activities for KernelFunction as well as requests to models.

Examples

Subscribe to available activity sources using OpenTelemetry.Sdk:

```csharp
using var traceProvider = Sdk.CreateTracerProviderBuilder()
  .AddSource("Microsoft.SemanticKernel*")
  .AddAzureMonitorTraceExporter(options => options.ConnectionString = connectionString)
  .Build();
  ```
Read more at: https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/trace/customizing-the-sdk/README.md

# Additional Learning
For more information, please refer to the following articles:

1. [Observability](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel)
2. [OpenTelemetry](https://opentelemetry.io/docs/)
3. [Enable Azure Monitor OpenTelemetry for .Net](https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable?tabs=net)
4. [Configure Azure Monitor OpenTelemetry for .Net](https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-configuration?tabs=net)
5. [Add, modify, and filter Azure Monitor OpenTelemetry](https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-add-modify?tabs=net)
6. [Customizing OpenTelemetry .NET SDK for Metrics](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/metrics/customizing-the-sdk/README.md)
7. [Customizing OpenTelemetry .NET SDK for Logs](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/logs/customizing-the-sdk/README.md)

# Open Telemetry
Semantic Kernel is also committed to provide the best developer experience while complying with the industry standards for observability. For more information, please review [ADR](https://github.com/microsoft/semantic-kernel/blob/main/docs/decisions/0044-OTel-semantic-convention.md).

The OTel GenAI semantic conventions are experimental. There are two options to enable the feature:

1. AppContext switch:
    - Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnostics
    - Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive

2. Environment variable
    - SEMANTICKERNEL_EXPERIMENTAL_GENAI_ENABLE_OTEL_DIAGNOSTICS
    - SEMANTICKERNEL_EXPERIMENTAL_GENAI_ENABLE_OTEL_DIAGNOSTICS_SENSITIVE

Enabling the collection of sensitive data including prompts and responses will implicitly enable the feature.

# Enterprise Readiness
When using Semantic Kernel telemetry in your enterprise, ensure that the data you're using is adhering to proper data storage. 