---
title: Inspection of telemetry data with the console
description: Output telemetry data to the console for inspection
zone_pivot_groups: programming-languages
author: taochen
ms.topic: conceptual
ms.author: taochen
ms.date: 09/12/2024
ms.service: semantic-kernel
---

# Inspection of telemetry data with the console

Although the console is not a recommended way to inspect telemetry data, it is a simple and quick way to get started. This article shows you how to output telemetry data to the console for inspection with a minimal Kernel setup.

## Exporter

Exporters are responsible for sending telemetry data to a destination. Read more about exporters [here](https://opentelemetry.io/docs/concepts/components/#exporters). In this example, we use the console exporter to output telemetry data to the console.

## Prerequisites

::: zone pivot="programming-language-csharp"

- The latest [.Net SDK](https://dotnet.microsoft.com/download/dotnet) for your operating system.

::: zone-end

::: zone pivot="programming-language-python"

- [Python 3.10, 3.11, or 3.12](https://www.python.org/downloads/) installed on your machine.

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Semantic Kernel Observability is not yet available for Java.

::: zone-end

## Setup

::: zone pivot="programming-language-csharp"

### Create a new console application

```console
dotnet new console -n TelemetryConsoleQuickstart
```

Navigate to the newly created project directory after the command completes.


### Install required packages

- Semantic Kernel
    ```console
    dotnet add package Microsoft.SemanticKernel
    ```

- OpenTelemetry Console Exporter
    ```console
    dotnet add package OpenTelemetry.Exporter.Console
    ```

::: zone-end

::: zone pivot="programming-language-python"

### Create a new Python virtual environment

```console
python -m venv telemetry-console-quickstart
```

Activate the virtual environment.
```console
telemetry-console-quickstart\Scripts\activate
```

### Install required packages

```console
pip install semantic-kernel opentelemetry-sdk opentelemetry-exporter-console
```

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Semantic Kernel Observability is not yet available for Java.
 
::: zone-end

## Run

## Next steps