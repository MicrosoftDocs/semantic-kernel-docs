---
title: DevUI Samples
description: Browse sample agents and workflows for use with DevUI.
author: moonbox3
ms.topic: reference
ms.author: evmattso
ms.date: 12/10/2025
ms.service: agent-framework
zone_pivot_groups: programming-languages
---

# Samples

This page provides links to sample agents and workflows designed for use with DevUI.

::: zone pivot="programming-language-csharp"

## Coming Soon

DevUI samples for C# are coming soon. Please check back later or refer to the Python samples for guidance.

::: zone-end

::: zone pivot="programming-language-python"

## Getting Started Samples

The Agent Framework repository includes sample agents and workflows in the `python/samples/02-agents/devui/` directory:

| Sample | Description |
|--------|-------------|
| [weather_agent_azure](https://github.com/microsoft/agent-framework/tree/main/python/samples/02-agents/devui/weather_agent_azure) | A weather agent using Azure OpenAI |
| [foundry_agent](https://github.com/microsoft/agent-framework/tree/main/python/samples/02-agents/devui/foundry_agent) | Agent using Azure AI Foundry |
| [azure_responses_agent](https://github.com/microsoft/agent-framework/tree/main/python/samples/02-agents/devui/azure_responses_agent) | Agent using Azure Responses API |
| [fanout_workflow](https://github.com/microsoft/agent-framework/tree/main/python/samples/02-agents/devui/fanout_workflow) | Workflow demonstrating fan-out pattern |
| [spam_workflow](https://github.com/microsoft/agent-framework/tree/main/python/samples/02-agents/devui/spam_workflow) | Workflow for spam detection |
| [workflow_agents](https://github.com/microsoft/agent-framework/tree/main/python/samples/02-agents/devui/workflow_agents) | Multiple agents in a workflow |

## Running the Samples

### Clone and Navigate

```bash
git clone https://github.com/microsoft/agent-framework.git
cd agent-framework/python/samples/02-agents/devui
```

### Set Up Environment

Each sample may require environment variables. Check for `.env.example` files:

```bash
# Copy and edit the example file
cp weather_agent_azure/.env.example weather_agent_azure/.env
# Edit .env with your credentials
```

### Launch DevUI

```bash
# Discover all samples
devui .

# Or run a specific sample
devui ./weather_agent_azure
```

## In-Memory Mode

The `in_memory_mode.py` script demonstrates running agents without directory discovery:

```bash
python in_memory_mode.py
```

This opens the browser with pre-configured agents and a basic workflow, showing how to use `serve()` programmatically.

## Sample Gallery

When DevUI starts with no discovered entities, it displays a **sample gallery** with curated examples. From the gallery, you can:

1. Browse available samples
2. View sample descriptions and requirements
3. Download samples to your local machine
4. Run samples directly

## Creating Your Own Samples

Follow the [Directory Discovery](./directory-discovery.md) guide to create your own agents and workflows compatible with DevUI.

### Minimal Agent Template

```python
# my_agent/__init__.py
from agent_framework import Agent
from agent_framework.openai import OpenAIChatClient

agent = Agent(
    name="my_agent",
    chat_client=OpenAIChatClient(),
    instructions="You are a helpful assistant."
)
```

### Minimal Workflow Template

```python
# my_workflow/__init__.py
from agent_framework.workflows import WorkflowBuilder

# Define your workflow
workflow = (
    WorkflowBuilder(start_executor="my_executor")
    # Add executors and edges
    .build()
)
```

## Related Resources

- [DevUI Package README](https://github.com/microsoft/agent-framework/tree/main/python/packages/devui) - Full package documentation
- [Agent Framework Samples](https://github.com/microsoft/agent-framework/tree/main/python/samples) - All Python samples
- [Workflow Samples](https://github.com/microsoft/agent-framework/tree/main/python/samples/03-workflows) - Workflow-specific samples

::: zone-end

## Next Steps

- [Overview](./index.md) - Return to DevUI overview
- [Directory Discovery](./directory-discovery.md) - Learn about directory structure
- [API Reference](./api-reference.md) - Explore the API
