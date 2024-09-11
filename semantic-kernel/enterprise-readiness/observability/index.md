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
