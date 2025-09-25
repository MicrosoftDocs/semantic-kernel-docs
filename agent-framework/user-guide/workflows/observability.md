---
title: Microsoft Agent Framework Workflows - Observability
description: In-depth look at Observability in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/12/2025
ms.service: semantic-kernel
---

# Microsoft Agent Framework Workflows - Observability

Observability provides insights into the internal state and behavior of workflows during execution. This includes logging, metrics, and tracing capabilities that help monitor and debug workflows.

Aside from the standard [GenAI telemetry](https://opentelemetry.io/docs/specs/semconv/gen-ai/), Agent Framework Workflows emits additional spans, logs, and metrics to provide deeper insights into workflow execution. These observability features help developers understand the flow of messages, the performance of executors, and any errors that may occur.

::: zone pivot="programming-language-csharp"

Coming soon...

::: zone-end

::: zone pivot="programming-language-python"

## Enable Observability

Observability is enabled framework-wide by setting the `ENABLE_OTEL=true` environment variable or calling `setup_observability()` at the beginning of your application.

```env
# This is not required if you run `setup_observability()` in your code
ENABLE_OTEL=true
# Sensitive data (e.g., message content) will be included in logs and traces if this is set to true
ENABLE_SENSITIVE_DATA=true
```

```python
from agent_framework.observability import setup_observability

setup_observability(enable_sensitive_data=True)
```

## Workflow Spans

| Span Name                        | Description                                      |
|----------------------------------|--------------------------------------------------|
| `workflow.build`                 | For each workflow build                          |
| `workflow.run`                   | For each workflow execution                      |
| `message.send`                   | For each message sent to an executor             |
| `executor.process`               | For each executor processing a message           |
| `edge_group.process`             | For each edge group processing a message         |

### Links between Spans

When an executor sends a message to another executor, the `message.send` span is created as a child of the `executor.process` span. However, the `executor.process` span of the target executor will not be a child of the `message.send` span because the execution is not nested. Instead, the `executor.process` span of the target executor is linked to the `message.send` span of the source executor. This creates a traceable path through the workflow execution.

For example:

![Span Relationships](./resources/images/workflow-trace.png)

## Next Steps

- [Learn how to use agents in workflows](./using-agents.md) to build intelligent workflows.
- [Learn how to handle requests and responses](./request-and-response.md) in workflows.
- [Learn how to manage state](./shared-states.md) in workflows.
- [Learn how to create checkpoints and resume from them](./checkpoints.md).