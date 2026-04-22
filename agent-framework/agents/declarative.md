---
title: Declarative Agents
description: Learn how to define agents declaratively using configuration files in Agent Framework.
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: reference
ms.author: edvan
ms.date: 02/09/2026
ms.service: agent-framework
---

# Declarative Agents

Declarative agents allow you to define agent configuration using YAML or JSON files instead of writing programmatic code. This approach makes agents easier to define, modify, and share across teams.

:::zone pivot="programming-language-csharp"

The following example shows how to create a declarative agent from a YAML configuration:

```csharp
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;

// Create the chat client
IChatClient chatClient = new AIProjectClient(
    new Uri("<your-foundry-project-endpoint>"),
    new DefaultAzureCredential())
        .GetProjectOpenAIClient()
        .GetProjectResponsesClient()
        .AsIChatClient("gpt-4o-mini");

// Define the agent using a YAML definition.
var yamlDefinition =
    """
    kind: Prompt
    name: Assistant
    description: Helpful assistant
    instructions: You are a helpful assistant. You answer questions in the language specified by the user. You return your answers in a JSON format.
    model:
        options:
            temperature: 0.9
            topP: 0.95
    outputSchema:
        properties:
            language:
                type: string
                required: true
                description: The language of the answer.
            answer:
                type: string
                required: true
                description: The answer text.
    """;

// Create the agent from the YAML definition.
var agentFactory = new ChatClientPromptAgentFactory(chatClient);
var agent = await agentFactory.CreateFromYamlAsync(yamlDefinition);

// Invoke the agent and output the text result.
Console.WriteLine(await agent!.RunAsync("Tell me a joke about a pirate in English."));

// Invoke the agent with streaming support.
await foreach (var update in agent!.RunStreamingAsync("Tell me a joke about a pirate in French."))
{
    Console.WriteLine(update);
}
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

:::zone-end

:::zone pivot="programming-language-python"

### Define an agent inline with YAML

You can define the full YAML specification as a string directly in your code:

```python
import asyncio

from agent_framework.declarative import AgentFactory
from azure.identity.aio import AzureCliCredential


async def main():
    """Create an agent from an inline YAML definition and run it."""
    yaml_definition = """kind: Prompt
name: DiagnosticAgent
displayName: Diagnostic Assistant
instructions: Specialized diagnostic and issue detection agent for systems with critical error protocol and automatic handoff capabilities
description: An agent that performs diagnostics on systems and can escalate issues when critical errors are detected.

model:
  id: =Env.AZURE_OPENAI_MODEL
  connection:
    kind: remote
    endpoint: =Env.FOUNDRY_PROJECT_ENDPOINT
"""
    async with (
        AzureCliCredential() as credential,
        AgentFactory(client_kwargs={"credential": credential}).create_agent_from_yaml(yaml_definition) as agent,
    ):
        response = await agent.run("What can you do for me?")
        print("Agent response:", response.text)


if __name__ == "__main__":
    asyncio.run(main())
```

### Load an agent from a YAML file

You can also load the YAML definition from a file:

```python
import asyncio
from pathlib import Path

from agent_framework.declarative import AgentFactory
from azure.identity import AzureCliCredential


async def main():
    """Create an agent from a declarative YAML file and run it."""
    yaml_path = Path(__file__).parent / "agent-config.yaml"

    with yaml_path.open("r") as f:
        yaml_str = f.read()

    agent = AgentFactory(client_kwargs={"credential": AzureCliCredential()}).create_agent_from_yaml(yaml_str)
    response = await agent.run("Why is the sky blue?")
    print("Agent response:", response.text)


if __name__ == "__main__":
    asyncio.run(main())
```

:::zone-end

:::zone pivot="programming-language-go"

> [!NOTE]
> Go support for this feature is coming soon. See the [Agent Framework Go repository](https://github.com/microsoft/agent-framework-go) for the latest status.

:::zone-end
## Next steps

> [!div class="nextstepaction"]
> [Observability](./observability.md)
