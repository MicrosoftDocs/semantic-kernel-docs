---
title: Exploring the Semantic Kernel Azure AI Agent Agent
description: An exploration of the definition, behaviors, and usage patterns for an Azure AI Agent
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 03/05/2025
ms.service: semantic-kernel
---
# Exploring the Semantic Kernel `AzureAIAgent`

> [!IMPORTANT]
> This feature is in the experimental stage. Features at this stage are still under development and subject to change before advancing to the preview or release candidate stage.

Detailed API documentation related to this discussion is available at:

::: zone pivot="programming-language-csharp"

- [`AzureAIAgent`](/dotnet/api/microsoft.semantickernel.agents.azureai)

::: zone-end

::: zone pivot="programming-language-python"

- [`AzureAIAgent`](python/api/semantic-kernel/semantic_kernel.agents.azure_ai.azure_ai_agent.azureaiagent)

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## What is an `AzureAIAgent`?

An `AzureAIAgent` is a specialized agent within the Semantic Kernel framework, designed to provide advanced conversational capabilities with seamless tool integration. It automates tool calling, eliminating the need for manual parsing and invocation. The agent also securely manages conversation history using threads, reducing the overhead of maintaining state. Additionally, the `AzureAIAgent` supports a variety of built-in tools, including file retrieval, code execution, and data interaction via Bing, Azure AI Search, Azure Functions, and OpenAPI.

To use an `AzureAIAgent`, an Azure AI Foundry Project must be utilized.  The following articles provide an overview of the Azure AI Foundry, how to create and configure a project, and the agent service:

- [What is Azure AI Foundry?](/azure/ai-foundry/what-is-ai-foundry)
- [The Azure AI Foundry SDK](/azure/ai-foundry/how-to/develop/sdk-overview)
- [What is Azure AI Agent Service](/azure/ai-services/agents/overview)
- [Quickstart: Create a new agent](/azure/ai-services/agents/quickstart)


## Preparing Your Development Environment

To proceed with developing an `AzureAIAgent`, configure your development environment with the appropriate packages.

::: zone pivot="programming-language-csharp"

Add the `Microsoft.SemanticKernel.Agents.AzureAI` package to your project:

```pwsh
dotnet add package Microsoft.SemanticKernel.Agents.AzureAI --prerelease
```

You may also want to include the `Azure.Identity` package:

```pwsh
dotnet add package Azure.Identity
```

::: zone-end

::: zone pivot="programming-language-python"

Install the `semantic-kernel` package with the optional Azure dependencies:

```bash
pip install semantic-kernel[azure]
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Configuring the AI Project Client

Accessing an `AzureAIAgent` first requires the creation of a project client that is configured for a specific Foundry Project, most commonly by providing a connection string ([The Azure AI Foundry SDK: Getting Started with Projects](/azure/ai-foundry/how-to/develop/sdk-overview#get-started-with-projects)).

::: zone pivot="programming-language-csharp"

```c#
AIProjectClient client = AzureAIAgent.CreateAzureAIClient("<your connection-string>", new AzureCliCredential());
```

The `AgentsClient` may be accessed from the `AIProjectClient`:

```c#
AgentsClient agentsClient = client.GetAgentsClient();
```

::: zone-end

::: zone pivot="programming-language-python"

Modify your the `.env` file in the root directory to include:

```bash
AZURE_AI_AGENT_PROJECT_CONNECTION_STRING = "<example-connection-string>"
AZURE_AI_AGENT_MODEL_DEPLOYMENT_NAME = "<example-model-deployment-name>"
```

or

```bash
AZURE_AI_AGENT_ENDPOINT = "<example-endpoint>"
AZURE_AI_AGENT_SUBSCRIPTION_ID = "<example-subscription-id>"
AZURE_AI_AGENT_RESOURCE_GROUP_NAME = "<example-resource-group-name>"
AZURE_AI_AGENT_PROJECT_NAME = "<example-project-name>"
AZURE_AI_AGENT_MODEL_DEPLOYMENT_NAME = "<example-model-deployment-name>"
```

Once the configuration is defined, the client may be created:

```python
async with (
    DefaultAzureCredential() as creds,
    AzureAIAgent.create_client(credential=creds) as client,
):
    # Your operational code here
```

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Creating an `AzureAIAgent`

To create an `AzureAIAgent`, you start by configuring and initializing the agent project through the Azure AI service and then integrate it with Semantic Kernel:

::: zone pivot="programming-language-csharp"

```c#
AIProjectClient client = AzureAIAgent.CreateAzureAIClient("<your connection-string>", new AzureCliCredential());
AgentsClient agentsClient = client.GetAgentsClient();

// 1. Define an agent on the Azure AI agent service
Agent definition = agentsClient.CreateAgentAsync(
    "<name of the the model used by the agent>",
    name: "<agent name>",
    description: "<agent description>",
    instructions: "<agent instructions>");

// 2. Create a Semantic Kernel agent based on the agent definition
AzureAIAgent agent = new(definition, agentsClient);
```
::: zone-end

::: zone pivot="programming-language-python"

```python
from azure.identity.aio import DefaultAzureCredential
from semantic_kernel.agents.azure_ai import AzureAIAgent, AzureAIAgentSettings

ai_agent_settings = AzureAIAgentSettings.create()

async with (
    DefaultAzureCredential() as creds,
    AzureAIAgent.create_client(credential=creds) as client,
):
    # 1. Define an agent on the Azure AI agent service
    agent_definition = await client.agents.create_agent(
        model=ai_agent_settings.model_deployment_name,
        name="<name>",
        instructions="<instructions>",
    )

    # 2. Create a Semantic Kernel agent based on the agent definition
    agent = AzureAIAgent(
        client=client,
        definition=agent_definition,
    )
```

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Interacting with an `AzureAIAgent`

Interaction with the `AzureAIAgent` is straightforward. The agent maintains the conversation history automatically using a thread:

::: zone pivot="programming-language-csharp"
```c#
AgentThread thread = await agentsClient.CreateThreadAsync();
try
{
    ChatMessageContent message = new(AuthorRole.User, "<your user input>");
    await agent.AddChatMessageAsync(threadId, message);
    await foreach (ChatMessageContent response in agent.InvokeAsync(thread.Id))
    {
        Console.WriteLine(response.Content);
    }
}
finally
{
    await this.AgentsClient.DeleteThreadAsync(thread.Id);
    await this.AgentsClient.DeleteAgentAsync(agent.Id);
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
USER_INPUTS = ["Hello", "What's your name?"]

thread = await client.agents.create_thread()

try:
    for user_input in USER_INPUTS:
        await agent.add_chat_message(thread_id=thread.id, message=user_input)
        response = await agent.get_response(thread_id=thread.id)
        print(response)
finally:
    await client.agents.delete_thread(thread.id)
```

Optionally, an agent may be invoked as: 

```python
for user_input in USER_INPUTS:
    await agent.add_chat_message(thread_id=thread.id, message=user_input)
    async for content in agent.invoke(thread_id=thread.id):
        print(content.content)
```

::: zone-end

An agent may also produce a streamed response:

::: zone pivot="programming-language-csharp"
```c#
ChatMessageContent message = new(AuthorRole.User, "<your user input>");
await agent.AddChatMessageAsync(threadId, message);
await foreach (StreamingChatMessageContent response in agent.InvokeStreamingAsync(thread.Id))
{
    Console.Write(response.Content);
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
for user_input in USER_INPUTS:
    await agent.add_chat_message(thread_id=thread.id, message=user_input)
    async for content in agent.invoke_stream(thread_id=thread.id):
        print(content.content, end="", flush=True)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Using Plugins with an `AzureAIAgent`

Semantic Kernel supports extending an `AzureAIAgent` with custom plugins for enhanced functionality:

::: zone pivot="programming-language-csharp"
```c#
Plugin plugin = KernelPluginFactory.CreateFromType<YourPlugin>();
AIProjectClient client = AzureAIAgent.CreateAzureAIClient("<your connection-string>", new AzureCliCredential());
AgentsClient agentsClient = client.GetAgentsClient();

Agent definition = agentsClient.CreateAgentAsync(
    "<name of the the model used by the agent>",
    name: "<agent name>",
    description: "<agent description>",
    instructions: "<agent instructions>");

AzureAIAgent agent = new(definition, agentsClient, plugins: [plugin]);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
from semantic_kernel.functions import kernel_function

class SamplePlugin:
    @kernel_function(description="Provides sample data.")
    def get_data(self) -> str:
        return "Sample data"

ai_agent_settings = AzureAIAgentSettings.create()

async with (
        DefaultAzureCredential() as creds,
        AzureAIAgent.create_client(credential=creds) as client,
    ):
        agent_definition = await client.agents.create_agent(
            model=ai_agent_settings.model_deployment_name,
        )

        agent = AzureAIAgent(
            client=client,
            definition=agent_definition,
            plugins=[SamplePlugin()]
        )
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Advanced Features

An `AzureAIAgent` can leverage advanced tools such as:

- [Code Interpreter](#code-interpreter)
- [File Search](#file-search)
- [OpenAPI integration](#openapi-integration)
- [Azure AI Search integration](#azureai-search-integration)

### Code Interpreter

Code Interpreter allows the agents to write and run Python code in a sandboxed execution environment ([Azure AI Agent Service Code Interpreter](/azure/ai-services/agents/how-to/tools/code-interpreter)).

::: zone pivot="programming-language-csharp"
```c#
AIProjectClient client = AzureAIAgent.CreateAzureAIClient("<your connection-string>", new AzureCliCredential());
AgentsClient agentsClient = client.GetAgentsClient();

Agent definition = agentsClient.CreateAgentAsync(
    "<name of the the model used by the agent>",
    name: "<agent name>",
    description: "<agent description>",
    instructions: "<agent instructions>",
    tools: [new CodeInterpreterToolDefinition()],
    toolResources:
        new()
        {
            CodeInterpreter = new()
            {
                FileIds = { ... },
            }
        }));

AzureAIAgent agent = new(definition, agentsClient);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
from azure.ai.projects.models import CodeInterpreterTool

async with (
        DefaultAzureCredential() as creds,
        AzureAIAgent.create_client(credential=creds) as client,
    ):
        code_interpreter = CodeInterpreterTool()
        agent_definition = await client.agents.create_agent(
            model=ai_agent_settings.model_deployment_name,
            tools=code_interpreter.definitions,
            tool_resources=code_interpreter.resources,
        )
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

### File Search

File search augments agents with knowledge from outside its model ([Azure AI Agent Service File Search Tool](/azure/ai-services/agents/how-to/tools/file-search)).

::: zone pivot="programming-language-csharp"

```c#
AIProjectClient client = AzureAIAgent.CreateAzureAIClient("<your connection-string>", new AzureCliCredential());
AgentsClient agentsClient = client.GetAgentsClient();

Agent definition = agentsClient.CreateAgentAsync(
    "<name of the the model used by the agent>",
    name: "<agent name>",
    description: "<agent description>",
    instructions: "<agent instructions>",
    tools: [new FileSearchToolDefinition()],
    toolResources:
        new()
        {
            FileSearch = new()
            {
                VectorStoreIds = { ... },
            }
        }));

AzureAIAgent agent = new(definition, agentsClient);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
from azure.ai.projects.models import FileSearchTool

async with (
        DefaultAzureCredential() as creds,
        AzureAIAgent.create_client(credential=creds) as client,
    ):
        file_search = FileSearchTool(vector_store_ids=[vector_store.id])
        agent_definition = await client.agents.create_agent(
            model=ai_agent_settings.model_deployment_name,
            tools=file_search.definitions,
            tool_resources=file_search.resources,
        )
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

### OpenAPI Integration

Connects your agent to an external API ([How to use Azure AI Agent Service with OpenAPI Specified Tools](/azure/ai-services/agents/how-to/tools/openapi-spec)).

::: zone pivot="programming-language-csharp"
```c#
AIProjectClient client = AzureAIAgent.CreateAzureAIClient("<your connection-string>", new AzureCliCredential());
AgentsClient agentsClient = client.GetAgentsClient();

string apiJsonSpecification = ...; // An Open API JSON specification

Agent definition = agentsClient.CreateAgentAsync(
    "<name of the the model used by the agent>",
    name: "<agent name>",
    description: "<agent description>",
    instructions: "<agent instructions>",
    tools: [
        new OpenApiToolDefinition(
            "<api name>", 
            "<api description>", 
            BinaryData.FromString(apiJsonSpecification), 
            new OpenApiAnonymousAuthDetails())
    ],
);

AzureAIAgent agent = new(definition, agentsClient);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
from azure.ai.projects.models import OpenApiTool, OpenApiAnonymousAuthDetails

async with (
        DefaultAzureCredential() as creds,
        AzureAIAgent.create_client(credential=creds) as client,
    ):
        openapi_spec_file_path = "sample/filepath/..."
        with open(os.path.join(openapi_spec_file_path, "spec_one.json")) as file_one:
            openapi_spec_one = json.loads(file_one.read())
        with open(os.path.join(openapi_spec_file_path, "spec_two.json")) as file_two:
            openapi_spec_two = json.loads(file_two.read())

        # Note that connection or managed identity auth setup requires additional setup in Azure
        auth = OpenApiAnonymousAuthDetails()
        openapi_tool_one = OpenApiTool(
            name="<name>",
            spec=openapi_spec_one,
            description="<description>",
            auth=auth,
        )
        openapi_tool_two = OpenApiTool(
            name="<name>",
            spec=openapi_spec_two,
            description="<description>",
            auth=auth,
        )

        agent_definition = await client.agents.create_agent(
            model=ai_agent_settings.model_deployment_name,
            tools=openapi_tool_one.definitions + openapi_tool_two.definitions,
        )
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

### AzureAI Search Integration

Use an existing Azure AI Search index with with your agent ([Use an existing AI Search index](/azure/ai-services/agents/how-to/tools/azure-ai-search)).

::: zone pivot="programming-language-csharp"
```c#
AIProjectClient client = AzureAIAgent.CreateAzureAIClient("<your connection-string>", new AzureCliCredential());
AgentsClient agentsClient = client.GetAgentsClient();

ConnectionsClient cxnClient = client.GetConnectionsClient();
ListConnectionsResponse searchConnections = await cxnClient.GetConnectionsAsync(AzureAIP.ConnectionType.AzureAISearch);
ConnectionResponse searchConnection = searchConnections.Value[0];

Agent definition = agentsClient.CreateAgentAsync(
    "<name of the the model used by the agent>",
    name: "<agent name>",
    description: "<agent description>",
    instructions: "<agent instructions>",
    tools: [new AzureAIP.AzureAISearchToolDefinition()],
    toolResources: new()
    {
        AzureAISearch = new()
        {
            IndexList = { new AzureAIP.IndexResource(searchConnection.Id, "<your index name>") }
        }
    });

AzureAIAgent agent = new(definition, agentsClient);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
from azure.ai.projects.models import AzureAISearchTool, ConnectionType

async with (
        DefaultAzureCredential() as creds,
        AzureAIAgent.create_client(credential=creds) as client,
    ):
        conn_list = await client.connections.list()

        ai_search_conn_id = ""
        for conn in conn_list:
            if conn.connection_type == ConnectionType.AZURE_AI_SEARCH:
                ai_search_conn_id = conn.id
                break

        ai_search = AzureAISearchTool(
            index_connection_id=ai_search_conn_id, 
            index_name=AZURE_AI_SEARCH_INDEX_NAME,
        )

        agent_definition = await client.agents.create_agent(
            model=ai_agent_settings.model_deployment_name,
            instructions="Answer questions using your index.",
            tools=ai_search.definitions,
            tool_resources=ai_search.resources,
            headers={"x-ms-enable-preview": "true"},
        )
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

### Retrieving an Existing `AzureAIAgent`

An existing agent can be retrieved and reused by specifying its assistant ID:

::: zone pivot="programming-language-csharp"

```c#
Agent definition = agentsClient.GetAgentAsync("<your agent id>");
AzureAIAgent agent = new(definition, agentsClient);
```

::: zone-end

::: zone pivot="programming-language-python"
```python
agent_definition = await client.agents.get_agent(assistant_id="your-agent-id")
agent = AzureAIAgent(client=client, definition=agent_definition)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Deleting an `AzureAIAgent`

Agents and their associated threads can be deleted when no longer needed:

::: zone pivot="programming-language-csharp"

```c#
await agentsClient.DeleteThreadAsync(thread.Id);
await agentsClient.DeleteAgentAsync(agent.Id);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
await client.agents.delete_thread(thread.id)
await client.agents.delete_agent(agent.id)
```
::: zone-end

If working with a vector store or files, they may be deleted as well:

::: zone pivot="programming-language-csharp"
```c#
await agentsClient.DeleteVectorStoreAsync("<your store id>");
await agentsClient.DeleteFileAsync("<your file id>");
```
::: zone-end

::: zone pivot="programming-language-python"
```python
await client.agents.delete_file(file_id=file.id)
await client.agents.delete_vector_store(vector_store_id=vector_store.id)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

> More information on the _file search_ tool is described in the [Azure AI Agent Service file search tool](/azure/ai-services/agents/how-to/tools/file-search) article.

## How-To

For practical examples of using an `AzureAIAgent`, see our code samples on GitHub:

::: zone pivot="programming-language-csharp"

- [Getting Started with Azure AI Agents](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/GettingStartedWithAgents/AzureAIAgent)
- [Advanced Azure AI Agent Code Samples](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/Concepts/Agents)

::: zone-end

::: zone pivot="programming-language-python"

- [Getting Started with Azure AI Agents](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/getting_started_with_agents/azure_ai_agent)
- [Advanced Azure AI Agent Code Samples](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/concepts/agents/azure_ai_agent)

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

> [!div class="nextstepaction"]
> [Agent Collaboration in Agent Chat](./agent-chat.md)