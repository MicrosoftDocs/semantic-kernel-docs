---
title: Agents in Workflows
description: Learn how to integrate agents into workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 03/09/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                        | C# | Python | Notes                              |
  |--------------------------------|:--:|:------:|----------------------------------  |
  | What You'll Build              | ✅ |   ✅   |                                    |
  | Concepts Covered               | ✅ |   ✅   |                                    |
  | Prerequisites                  | ✅ |   ✅   |                                    |
  | Client/Agent Setup             | ✅ |   ✅   | Different Azure services           |
  | Create Specialized Agents      | ✅ |   ✅   |                                    |
  | Build the Workflow             | ✅ |   ✅   |                                    |
  | Shared Session Pattern         | ❌ |   ❌   | Planned for future tutorial        |
  | Execute with Streaming         | ✅ |   ✅   |                                    |
  | Resource Cleanup               | ✅ |   ❌   | C#-specific (Azure Foundry agents) |
  | How It Works                   | ✅ |   ✅   |                                    |
  | Key Concepts                   | ✅ |   ✅   |                                    |
  | Complete Implementation        | ✅ |   ✅   |                                    |
-->

# Agents in Workflows

This tutorial demonstrates how to integrate AI agents into workflows using Agent Framework. You'll learn to create workflows that leverage the power of specialized AI agents for content creation, review, and other collaborative tasks.

::: zone pivot="programming-language-csharp"

## What You'll Build

You'll create a workflow that:

- Uses Azure Foundry Agent Service to create intelligent agents
- Implements a French translation agent that translates input to French
- Implements a Spanish translation agent that translates French to Spanish
- Implements an English translation agent that translates Spanish back to English
- Connects agents in a sequential workflow pipeline
- Streams real-time updates as agents process requests
- Demonstrates proper resource cleanup for Azure Foundry agents

### Concepts Covered

- [Agents in Workflows](./agents-in-workflows.md)
- [Direct Edges](./edges.md#direct-edges)
- [Workflow Builder](./index.md)

## Prerequisites

- [.NET 8.0 SDK or later](https://dotnet.microsoft.com/download)
- An Azure Foundry project endpoint and model configured
- [Azure CLI installed](/cli/azure/install-azure-cli) and [authenticated (for Azure credential authentication)](/cli/azure/authenticate-azure-cli)
- A new console application

## Step 1: Install NuGet packages

First, install the required packages for your .NET project:

```dotnetcli
dotnet add package Azure.AI.Agents.Persistent --prerelease
dotnet add package Azure.Identity
dotnet add package Microsoft.Agents.AI.AzureAI --prerelease
dotnet add package Microsoft.Agents.AI.Workflows --prerelease
```

## Step 2: Set Up Azure Foundry Client

Configure the Azure Foundry client with environment variables and authentication:

```csharp
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

public static class Program
{
    private static async Task Main()
    {
        // Set up the Azure OpenAI client
        var endpoint = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT")
            ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set.");
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_AI_MODEL_DEPLOYMENT_NAME") ?? "gpt-4o-mini";
        var persistentAgentsClient = new PersistentAgentsClient(endpoint, new AzureCliCredential());
```

## Step 3: Create Agent Factory Method

Implement a helper method to create Azure Foundry agents with specific instructions:

```csharp
    /// <summary>
    /// Creates a translation agent for the specified target language.
    /// </summary>
    /// <param name="targetLanguage">The target language for translation</param>
    /// <param name="persistentAgentsClient">The PersistentAgentsClient to create the agent</param>
    /// <param name="model">The model to use for the agent</param>
    /// <returns>A ChatClientAgent configured for the specified language</returns>
    private static async Task<ChatClientAgent> GetTranslationAgentAsync(
        string targetLanguage,
        PersistentAgentsClient persistentAgentsClient,
        string model)
    {
        var agentMetadata = await persistentAgentsClient.Administration.CreateAgentAsync(
            model: model,
            name: $"{targetLanguage} Translator",
            instructions: $"You are a translation assistant that translates the provided text to {targetLanguage}.");

        return await persistentAgentsClient.GetAIAgentAsync(agentMetadata.Value.Id);
    }
}
```

## Step 4: Create Specialized Azure Foundry Agents

Create three translation agents using the helper method:

```csharp
        // Create agents
        AIAgent frenchAgent = await GetTranslationAgentAsync("French", persistentAgentsClient, deploymentName);
        AIAgent spanishAgent = await GetTranslationAgentAsync("Spanish", persistentAgentsClient, deploymentName);
        AIAgent englishAgent = await GetTranslationAgentAsync("English", persistentAgentsClient, deploymentName);
```

## Step 5: Build the Workflow

Connect the agents in a sequential workflow using the WorkflowBuilder:

```csharp
        // Build the workflow by adding executors and connecting them
        var workflow = new WorkflowBuilder(frenchAgent)
            .AddEdge(frenchAgent, spanishAgent)
            .AddEdge(spanishAgent, englishAgent)
            .Build();
```

## Step 6: Execute with Streaming

Run the workflow with streaming to observe real-time updates from all agents:

```csharp
        // Execute the workflow
        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, new ChatMessage(ChatRole.User, "Hello World!"));

        // Must send the turn token to trigger the agents.
        // The agents are wrapped as executors. When they receive messages,
        // they will cache the messages and only start processing when they receive a TurnToken.
        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
        await foreach (WorkflowEvent evt in run.WatchStreamAsync())
        {
            if (evt is AgentResponseUpdateEvent executorComplete)
            {
                Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
            }
        }
```

## Step 7: Resource Cleanup

Properly clean up the Azure Foundry agents after use:

```csharp
        // Cleanup the agents created for the sample.
        await persistentAgentsClient.Administration.DeleteAgentAsync(frenchAgent.Id);
        await persistentAgentsClient.Administration.DeleteAgentAsync(spanishAgent.Id);
        await persistentAgentsClient.Administration.DeleteAgentAsync(englishAgent.Id);
    }
```

## How It Works

1. **Azure Foundry Client Setup**: Uses `PersistentAgentsClient` with Azure CLI credentials for authentication
2. **Agent Creation**: Creates persistent agents on Azure Foundry with specific instructions for translation
3. **Sequential Processing**: French agent translates input first, then Spanish agent, then English agent
4. **Turn Token Pattern**: Agents cache messages and only process when they receive a `TurnToken`
5. **Streaming Updates**: `AgentResponseUpdateEvent` provides real-time token updates as agents generate responses
6. **Resource Management**: Proper cleanup of Azure Foundry agents using the Administration API

## Key Concepts

- **Azure Foundry Agent Service**: Cloud-based AI agents with advanced reasoning capabilities
- **PersistentAgentsClient**: Client for creating and managing agents on Azure Foundry
- **WorkflowEvent**: Output events (`type="output"`) contain agent output data (`AgentResponseUpdate` for streaming, `AgentResponse` for non-streaming)
- **TurnToken**: Signal that triggers agent processing after message caching
- **Sequential Workflow**: Agents connected in a pipeline where output flows from one to the next

## Complete Implementation

For the complete working implementation of this Azure Foundry agents workflow, see the [FoundryAgent Program.cs](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/03-workflows/Agents/FoundryAgent/Program.cs) sample in the Agent Framework repository.

::: zone-end

::: zone pivot="programming-language-python"

## What You'll Build

You'll create a workflow that:

- Uses `AzureOpenAIResponsesClient` to create intelligent agents
- Implements a Writer agent that creates content based on prompts
- Implements a Reviewer agent that provides feedback on the content
- Connects agents in a sequential workflow pipeline
- Streams real-time updates as agents process requests

### Concepts Covered

- [Agents in Workflows](./agents-in-workflows.md)
- [Direct Edges](./edges.md#direct-edges)
- [Workflow Builder](./index.md)

## Prerequisites

- Python 3.10 or later
- Agent Framework installed: `pip install agent-framework --pre`
- Azure OpenAI Responses configured with proper environment variables
- Azure CLI authentication: `az login`

## Step 1: Import Required Dependencies

Start by importing the necessary components for workflows and Azure OpenAI Responses agents:

```python
import asyncio
import os

from agent_framework import AgentResponseUpdate, WorkflowBuilder
from agent_framework.azure import AzureOpenAIResponsesClient
from azure.identity import AzureCliCredential
```

## Step 2: Create Azure OpenAI Responses Client

Create one shared client that you can use to construct multiple agents:

```python
async def main() -> None:
    client = AzureOpenAIResponsesClient(
        project_endpoint=os.environ["AZURE_AI_PROJECT_ENDPOINT"],
        deployment_name=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
        credential=AzureCliCredential(),
    )
```

## Step 3: Create Specialized Agents

Create two specialized agents for content creation and review:

```python
    # Create a Writer agent that generates content
    writer_agent = client.as_agent(
        name="Writer",
        instructions=(
            "You are an excellent content writer. You create new content and edit contents based on the feedback."
        ),
    )

    # Create a Reviewer agent that provides feedback
    reviewer_agent = client.as_agent(
        name="Reviewer",
        instructions=(
            "You are an excellent content reviewer. "
            "Provide actionable feedback to the writer about the provided content. "
            "Provide the feedback in the most concise manner possible."
        ),
    )
```

## Step 4: Build the Workflow

Connect the agents in a sequential workflow using the builder:

```python
        # Build the workflow with agents as executors
        workflow = WorkflowBuilder(start_executor=writer_agent).add_edge(writer_agent, reviewer_agent).build()
```

## Step 5: Execute with Streaming

Run the workflow with streaming to observe real-time updates from both agents:

```python
    last_author: str | None = None

    events = workflow.run("Create a slogan for a new electric SUV that is affordable and fun to drive.", stream=True)
    async for event in events:
        if event.type == "output" and isinstance(event.data, AgentResponseUpdate):
            update = event.data
            author = update.author_name
            if author != last_author:
                if last_author is not None:
                    print()
                print(f"{author}: {update.text}", end="", flush=True)
                last_author = author
            else:
                print(update.text, end="", flush=True)
```

## Step 6: Complete Main Function

Wrap everything in the main function with proper async execution:

```python
if __name__ == "__main__":
    asyncio.run(main())
```

## How It Works

1. **Client Setup**: Uses one `AzureOpenAIResponsesClient` with Azure CLI credentials for authentication.
2. **Agent Creation**: Creates Writer and Reviewer agents from the same client configuration.
3. **Sequential Processing**: Writer agent generates content first, then passes it to the Reviewer agent.
4. **Streaming Updates**: Output events (`type="output"`) with `AgentResponseUpdate` data provide real-time token updates as agents generate responses.

## Key Concepts

- **AzureOpenAIResponsesClient**: Shared client used to create workflow agents with consistent configuration.
- **WorkflowEvent**: Output events (`type="output"`) contain agent output data (`AgentResponseUpdate` for streaming, `AgentResponse` for non-streaming).
- **Sequential Workflow**: Agents connected in a pipeline where output flows from one to the next.

## Complete Implementation

For the complete working implementation, see [azure_ai_agents_streaming.py](https://github.com/microsoft/agent-framework/blob/main/python/samples/03-workflows/agents/azure_ai_agents_streaming.py) in the Agent Framework repository.

::: zone-end

## Next Steps

> [!div class="nextstepaction"]
> [Human-in-the-Loop](./human-in-the-loop.md)
