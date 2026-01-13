---
title: Telemetry with MLflow Tracing
description: Collect Semantic Kernel traces in MLflow using autologging.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: conceptual
ms.author: taochen
ms.date: 11/25/2025
ms.service: semantic-kernel
---

# Inspection of telemetry data with MLflow

[MLflow](https://mlflow.org/) provides tracing for LLM applications and includes a built‑in integration for Microsoft Semantic Kernel. With a single line of code, you can capture spans from Semantic Kernel and browse them in the MLflow UI alongside parameters, metrics, and artifacts.

## Prerequisites

- Python 3.10, 3.11, or 3.12.
- An LLM provider. The example below uses Azure OpenAI chat completions.
- MLflow UI or Tracking Server (local UI shown below).

## Setup

::: zone pivot="programming-language-python"

### 1) Install packages

```bash
pip install semantic-kernel mlflow
```

### 2) Start the MLflow Tracking Server (local)

```bash
mlflow sever --port 5000 --backend-store-uri sqlite:///mlflow.db
```

### 3) Create a simple Semantic Kernel script and enable MLflow autologging

Create `telemetry_mlflow_quickstart.py` with the content below and fill in the environment variables for your Azure OpenAI deployment.

```python
import os
import asyncio
import mlflow

from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion

# One-line enablement of MLflow tracing for Semantic Kernel
mlflow.semantic_kernel.autolog()

# Set the tracking URI and experiment name (optional)
mlflow.set_tracking_uri("http://127.0.0.1:5000")
mlflow.set_experiment("telemetry-mlflow-quickstart")


async def main():
    # Configure the kernel and add an Azure OpenAI chat service
    kernel = Kernel()
    kernel.add_service(AzureChatCompletion(
        api_key=os.environ.get("AZURE_OPENAI_API_KEY"),
        endpoint=os.environ.get("AZURE_OPENAI_ENDPOINT"),
        deployment_name=os.environ.get("AZURE_OPENAI_CHAT_DEPLOYMENT_NAME"),
    ))

    # Issue a simple prompt; MLflow records spans automatically
    answer = await kernel.invoke_prompt("Why is the sky blue in one sentence?")
    print(answer)


if __name__ == "__main__":
    asyncio.run(main())
```

Run the script:

```bash
python telemetry_mlflow_quickstart.py
```

### 4) Inspect traces in MLflow

Open the MLflow UI (default at `http://127.0.0.1:5000`). Navigate to the Traces view to see spans emitted by Semantic Kernel, including function execution and model calls.

![MLflow Traces](https://mlflow.org/docs/latest/images/llms/tracing/semantic-kernel-tracing.png)

::: zone-end

## Next steps

- Explore the [Observability overview](./index.md) for additional exporters and patterns.
- Review [Advanced telemetry with Semantic Kernel](./telemetry-advanced.md) to customize signals and attributes.
- Visit [MLflow Semantic Kernel integration](https://mlflow.org/docs/latest/genai/tracing/integrations/listing/semantic-kernel/) for more detailed information on how to use MLflow with Semantic Kernel.
