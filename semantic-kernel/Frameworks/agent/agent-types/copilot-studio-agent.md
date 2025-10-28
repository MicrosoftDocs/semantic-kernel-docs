---
title: Exploring the Semantic Kernel Copilot Studio Agent
description: An exploration of the definition, behaviors, and usage patterns for an Copilot Studio Agent
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 05/20/2025
ms.service: semantic-kernel
---

# Exploring the Semantic Kernel `CopilotStudioAgent`

> [!IMPORTANT]
> This feature is in the experimental stage. Features at this stage are under development and subject to change before advancing to the preview or release candidate stage.

Detailed API documentation related to this discussion is available at:

::: zone pivot="programming-language-csharp"

> The CopilotStudioAgent for .NET is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

> Updated API docs are coming soon.

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## What is a `CopilotStudioAgent`?

A `CopilotStudioAgent` is an integration point within the Semantic Kernel framework that enables seamless interaction with [Microsoft Copilot Studio](https://copilotstudio.microsoft.com) agents using programmatic APIs. This agent allows you to:

- Automate conversations and invoke existing Copilot Studio agents from Python code.
- Maintain rich conversational history using threads, preserving context across messages.
- Leverage advanced knowledge retrieval, web search, and data integration capabilities made available within Microsoft Copilot Studio.

> [!NOTE]
> Knowledge sources/tools must be configured **within** Microsoft Copilot Studio before they can be accessed via the agent.

## Preparing Your Development Environment

To develop with the `CopilotStudioAgent`, you must have your environment and authentication set up correctly.

::: zone pivot="programming-language-csharp"

> The CopilotStudioAgent for .NET is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

### Prerequisites

1. **Python 3.10 or higher**
2. **Semantic Kernel** with Copilot Studio dependencies:

```bash
pip install semantic-kernel[copilotstudio]
```

3. **Microsoft Copilot Studio** agent:

   - Create an agent at [Microsoft Copilot Studio](https://copilotstudio.microsoft.com).
   - Publish your agent, and under `Settings → Advanced → Metadata` obtain:
     - `Schema Name` (used as `agent_identifier`)
     - `Environment ID`

4. **Azure Entra ID Application Registration** (“Native app”, for interactive login), with `CopilotStudio.Copilots.Invoke` delegated permission.

### Environment Variables

Set the following variables in your environment or `.env` file:

```env
COPILOT_STUDIO_AGENT_APP_CLIENT_ID=<your-app-client-id>
COPILOT_STUDIO_AGENT_TENANT_ID=<your-tenant-id>
COPILOT_STUDIO_AGENT_ENVIRONMENT_ID=<your-env-id>
COPILOT_STUDIO_AGENT_AGENT_IDENTIFIER=<your-agent-id>
COPILOT_STUDIO_AGENT_AUTH_MODE=interactive
```

> [!TIP]
> See [Power Platform API Authentication](/power-platform/admin/programmability-authentication-v2) for help with permissions.

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Creating and Configuring a `CopilotStudioAgent` Client

You may rely on environment variables for most configuration, but can explicitly create and customize the agent client as needed.

::: zone pivot="programming-language-csharp"

> The CopilotStudioAgent for .NET is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

### Basic Usage — Environment Variable Driven

```python
from semantic_kernel.agents import CopilotStudioAgent

agent = CopilotStudioAgent(
    name="PhysicsAgent",
    instructions="You help answer questions about physics.",
)
```

No explicit client setup is required if your environment variables are set.

### Explicit Client Construction

Override config or use custom identity:

```python
from semantic_kernel.agents import CopilotStudioAgent

client = CopilotStudioAgent.create_client(
    auth_mode="interactive",  # or use CopilotStudioAgentAuthMode.INTERACTIVE
    agent_identifier="<schema-name>",
    app_client_id="<client-id>",
    tenant_id="<tenant-id>",
    environment_id="<env-id>",
)

agent = CopilotStudioAgent(
    client=client,
    name="CustomAgent",
    instructions="You help answer custom questions.",
)
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Interacting with a `CopilotStudioAgent`

The core workflow is similar to other Semantic Kernel agents: provide user input(s), receive responses, maintain context via threads.

::: zone pivot="programming-language-csharp"

> The CopilotStudioAgent for .NET is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

### Basic Example

```python
import asyncio
from semantic_kernel.agents import CopilotStudioAgent

async def main():
    agent = CopilotStudioAgent(
        name="PhysicsAgent",
        instructions="You help answer questions about physics.",
    )

    USER_INPUTS = [
        "Why is the sky blue?",
        "What is the speed of light?",
    ]

    for user_input in USER_INPUTS:
        print(f"# User: {user_input}")
        response = await agent.get_response(messages=user_input)
        print(f"# {response.name}: {response}")

asyncio.run(main())
```

### Maintaining Context with Threads

To keep the conversation stateful:

```python
import asyncio
from semantic_kernel.agents import CopilotStudioAgent, CopilotStudioAgentThread

async def main():
    agent = CopilotStudioAgent(
        name="PhysicsAgent",
        instructions="You help answer questions about physics.",
    )

    USER_INPUTS = [
        "Hello! Who are you? My name is John Doe.",
        "What is the speed of light?",
        "What have we been talking about?",
        "What is my name?",
    ]

    thread: CopilotStudioAgentThread | None = None

    for user_input in USER_INPUTS:
        print(f"# User: {user_input}")
        response = await agent.get_response(messages=user_input, thread=thread)
        print(f"# {response.name}: {response}")
        thread = response.thread

    if thread:
        await thread.delete()

asyncio.run(main())
```

### Using Arguments and Prompt Templates

```python
import asyncio
from semantic_kernel.agents import CopilotStudioAgent, CopilotStudioAgentThread
from semantic_kernel.contents import ChatMessageContent
from semantic_kernel.functions import KernelArguments
from semantic_kernel.prompt_template import PromptTemplateConfig

async def main():
    agent = CopilotStudioAgent(
        name="JokeAgent",
        instructions="You are a joker. Tell kid-friendly jokes.",
        prompt_template_config=PromptTemplateConfig(template="Craft jokes about {{$topic}}"),
    )

    USER_INPUTS = [
        ChatMessageContent(role="user", content="Tell me a joke to make me laugh.")
    ]

    thread: CopilotStudioAgentThread | None = None

    for user_input in USER_INPUTS:
        print(f"# User: {user_input}")
        response = await agent.get_response(
            messages=user_input,
            thread=thread,
            arguments=KernelArguments(topic="pirate"),
        )
        print(f"# {response.name}: {response}")
        thread = response.thread

    if thread:
        await thread.delete()

asyncio.run(main())
```

### Iterating over Streaming (Not Supported)

> [!NOTE]
> Streaming responses are not currently supported by `CopilotStudioAgent`.

::: zone-end

## Using Plugins with a `CopilotStudioAgent`

Semantic Kernel allows composition of agents and plugins. Although the primary extensibility for Copilot Studio comes via the Studio itself, you can compose plugins as with other agents.

::: zone pivot="programming-language-csharp"

> The CopilotStudioAgent for .NET is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

```python
from semantic_kernel.functions import kernel_function
from semantic_kernel.agents import CopilotStudioAgent

class SamplePlugin:
    @kernel_function(description="Provides sample data.")
    def get_data(self) -> str:
        return "Sample data from custom plugin"

agent = CopilotStudioAgent(
    name="PluggedInAgent",
    instructions="Demonstrate plugins.",
    plugins=[SamplePlugin()],
)
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Advanced Features

A `CopilotStudioAgent` can leverage advanced Copilot Studio-enhanced abilities, depending on how the target agent is configured in the Studio environment:

- **Knowledge Retrieval** — responds based on pre-configured knowledge sources in the Studio.
- **Web Search** — if web search is enabled in your Studio agent, queries will use Bing Search.
- **Custom Auth or APIs** — via Power Platform and Studio plug-ins; direct OpenAPI binding is not currently first-class in SK integration.

::: zone pivot="programming-language-csharp"

> The CopilotStudioAgent for .NET is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

### Knowledge Retrieval

No specific Python code is needed; knowledge sources must be configured in Copilot Studio. When user messages require facts from these sources, the agent will return information as appropriate.

### Web Search

Configure your Copilot within Studio to allow Bing Search. Then use as above. For more information about configuring Bing Search see the following [guide](/microsoft-copilot-studio/nlu-generative-answers-bing).

```python
from semantic_kernel.agents import CopilotStudioAgent, CopilotStudioAgentThread

agent = CopilotStudioAgent(
    name="WebSearchAgent",
    instructions="Help answer the user's questions by searching the web.",
)

USER_INPUTS = ["Which team won the 2025 NCAA Basketball championship?"]

thread: CopilotStudioAgentThread | None = None

for user_input in USER_INPUTS:
    print(f"# User: {user_input}")
    # Note: Only non-streamed responses are supported
    response = await agent.get_response(messages=user_input, thread=thread)
    print(f"# {response.name}: {response}")
    thread = response.thread

if thread:
    await thread.delete()
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## How-To

For practical examples of using a `CopilotStudioAgent`, see our code samples on GitHub:

::: zone pivot="programming-language-csharp"

> The CopilotStudioAgent for .NET is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

- [Getting Started with Copilot Studio Agents](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/getting_started_with_agents/copilot_studio)

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

---

**Notes:**

- For more information or troubleshooting, see [Microsoft Copilot Studio documentation](/microsoft-365-copilot/extensibility/copilot-studio-agent-builder-build).
- Only features and tools separately enabled and published in your Studio agent will be available via the Semantic Kernel interface.
- Streaming, plugin deployment, and programmatic tool addition are planned for future releases.

---

## Next Steps

> [!div class="nextstepaction"]
> [Explore the OpenAI Assistant Agent](./assistant-agent.md)
