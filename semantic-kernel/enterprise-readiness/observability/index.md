---
title: Observability in Semantic Kernel
description: Introduction to observability in Semantic Kernel
author: taochen
ms.topic: conceptual
ms.author: taochen
ms.date: 09/11/2024
ms.service: semantic-kernel
---

# Brief introduction to observability

When you build AI solutions, you want to be able to observe the behavior of your services. Observability is the ability to monitor and analyze the internal state of components within a distributed system. It is a key requirement for building enterprise-ready AI solutions.

Observability is typically achieved through logging, metrics, and tracing. They are often referred to as the three pillars of observability. You will also hear the term "telemetry" used to describe the data collected by these three pillars. Unlike debugging, observability provides an ongoing overview of the system's health and performance.

Useful materials for further reading:
- [Observability defined by Cloud Native Computing Foundation](https://glossary.cncf.io/observability/)
- [Observability in .Net](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel)
- [OpenTelemetry](https://opentelemetry.io/docs/what-is-opentelemetry/)

# Observability in Semantic Kernel

Semantic Kernel is designed to be observable. It emits logs, metrics, and traces that are compatible to the OpenTelemetry standard. You can use your favorite observability tools to monitor and analyze the behavior of your services built on Semantic Kernel.

Specifically, Semantic Kernel provides the following observability features:
- **Logging**: Semantic Kernel logs meaningful events and errors from the kernel, kernel plugins and functions, as well as the AI connectors.
- **Metrics**: Semantic Kernel emits metrics from kernel functions and AI connectors. You will be able to monitor metrics such as the kernel function execution time, the token consumption of AI connectors, etc.
- **Tracing**: Semantic Kernel supports distributed tracing. You can track activities across different services and within Semantic Kernel.

# [OpenTelemetry Semantic Convention](https://opentelemetry.io/docs/concepts/semantic-conventions/)

Semantic Kernel follows the OpenTelemetry Semantic Convention for Observability. This means that the logs, metrics, and traces emitted by Semantic Kernel are structured and follow a common schema. This ensures that you can more effectively analyze the telemetry data emitted by Semantic Kernel.

> Currently, the [Semantic Conventions for Generative AI](https://github.com/open-telemetry/semantic-conventions/blob/main/docs/gen-ai/README.md) are in experimental status. Semantic Kernel strives to follow the OpenTelemetry Semantic Convention as closely as possible, and provide a consistent and meaningful observability experience for AI solutions.

# Output telemetry to the console
TODO


# Next steps
Now that you have a basic understanding of observability in Semantic Kernel, and have seen the raw telemetry data outputted to the console, you can learn more about how to use APM tools to visualize and analyze the telemetry data:
- [Application Insights](telemetry-with-app-insights.md)
- [Aspire Dashboard](telemetry-with-aspire-dashboard.md)