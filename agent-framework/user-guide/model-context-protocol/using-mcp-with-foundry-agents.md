---
title: MCP and Foundry Agents
description: Using MCP with Foundry Agents
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: reference
ms.author: markwallace
ms.date: 09/24/2025
ms.service: semantic-kernel
---

# Using MCP tools with Foundry Agents

You can extend the capabilities of your Azure AI Foundry agent by connecting it to tools hosted on remote [Model Context Protocol (MCP)](/azure/ai-foundry/agents/how-to/tools/model-context-protocol) servers (bring your own MCP server endpoint).

## How to use the Model Context Protocol tool

This section explains how to create an AI agent using Azure Foundry (Azure AI) with a hosted Model Context Protocol (MCP) server integration. The agent can utilize MCP tools that are managed and executed by the Azure Foundry service, allowing for secure and controlled access to external resources.

### Key Features

- **Hosted MCP Server**: The MCP server is hosted and managed by Azure AI Foundry, eliminating the need to manage server infrastructure
- **Persistent Agents**: Agents are created and stored server-side, allowing for stateful conversations
- **Tool Approval Workflow**: Configurable approval mechanisms for MCP tool invocations

### How It Works

::: zone pivot="programming-language-csharp"

#### 1. Environment Setup

The sample requires two environment variables:
- `AZURE_FOUNDRY_PROJECT_ENDPOINT`: Your Azure AI Foundry project endpoint URL
- `AZURE_FOUNDRY_PROJECT_MODEL_ID`: The model deployment name (defaults to "gpt-4.1-mini")

```csharp
var endpoint = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_ENDPOINT") 
    ?? throw new InvalidOperationException("AZURE_FOUNDRY_PROJECT_ENDPOINT is not set.");
var model = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_MODEL_ID") ?? "gpt-4.1-mini";
```

#### 2. Agent Configuration

The agent is configured with specific instructions and metadata:

```csharp
const string AgentName = "MicrosoftLearnAgent";
const string AgentInstructions = "You answer questions by searching the Microsoft Learn content only.";
```

This creates an agent specialized for answering questions using Microsoft Learn documentation.

#### 3. MCP Tool Definition

The sample creates an MCP tool definition that points to a hosted MCP server:

```csharp
var mcpTool = new MCPToolDefinition(
    serverLabel: "microsoft_learn",
    serverUrl: "https://learn.microsoft.com/api/mcp");
mcpTool.AllowedTools.Add("microsoft_docs_search");
```

**Key Components:**
- **serverLabel**: A unique identifier for the MCP server instance
- **serverUrl**: The URL of the hosted MCP server
- **AllowedTools**: Specifies which tools from the MCP server the agent can use

#### 4. Persistent Agent Creation

The agent is created server-side using the Azure AI Foundry Persistent Agents SDK:

```csharp
var persistentAgentsClient = new PersistentAgentsClient(endpoint, new AzureCliCredential());

var agentMetadata = await persistentAgentsClient.Administration.CreateAgentAsync(
    model: model,
    name: AgentName,
    instructions: AgentInstructions,
    tools: [mcpTool]);
```

This creates a persistent agent that:
- Lives on the Azure AI Foundry service
- Has access to the specified MCP tools
- Can maintain conversation state across multiple interactions

#### 5. Agent Retrieval and Execution

The created agent is retrieved as an `AIAgent` instance:

```csharp
AIAgent agent = await persistentAgentsClient.GetAIAgentAsync(agentMetadata.Value.Id);
```

#### 6. Tool Resource Configuration

The sample configures tool resources with approval settings:

```csharp
var runOptions = new ChatClientAgentRunOptions()
{
    ChatOptions = new()
    {
        RawRepresentationFactory = (_) => new ThreadAndRunOptions()
        {
            ToolResources = new MCPToolResource(serverLabel: "microsoft_learn")
            {
                RequireApproval = new MCPApproval("never"),
            }.ToToolResources()
        }
    }
};
```

**Key Configuration:**
- **MCPToolResource**: Links the MCP server instance to the agent execution
- **RequireApproval**: Controls when user approval is needed for tool invocations
  - `"never"`: Tools execute automatically without approval
  - `"always"`: All tool invocations require user approval
  - Custom approval rules can also be configured

#### 7. Agent Execution

The agent is invoked with a question and executes using the configured MCP tools:

```csharp
AgentThread thread = agent.GetNewThread();
var response = await agent.RunAsync(
    "Please summarize the Azure AI Agent documentation related to MCP Tool calling?", 
    thread, 
    runOptions);
Console.WriteLine(response);
```

#### 8. Cleanup

The sample demonstrates proper resource cleanup:

```csharp
await persistentAgentsClient.Administration.DeleteAgentAsync(agent.Id);
```

::: zone-end
::: zone pivot="programming-language-python"

Coming soon.

::: zone-end

To learn more refer to the [Azure AI Foundry Model Context Protocol documentation](/azure/ai-foundry/agents/how-to/tools/model-context-protocol).

## Next steps

> [!div class="nextstepaction"]
> [Using workflows as Agents](../workflows/as-agents.md)
