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

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

::: zone-end

::: zone pivot="programming-language-python"

> Updated Semantic Kernel Python API Docs are coming soon.

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## What is an `AzureAIAgent`?

An `AzureAIAgent` is a specialized agent within the Semantic Kernel framework, designed to provide advanced conversational capabilities with seamless tool integration. It automates tool calling, eliminating the need for manual parsing and invocation. The agent also securely manages conversation history using threads, reducing the overhead of maintaining state. Additionally, the `AzureAIAgent` supports a variety of built-in tools, including file retrieval, code execution, and data interaction via Bing, Azure AI Search, Azure Functions, and OpenAPI.

::: zone pivot="programming-language-csharp"

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

::: zone-end

::: zone pivot="programming-language-python"

To set up the required resources, follow the "Quickstart: Create a new agent" guide [here](/azure/ai-services/agents/quickstart?pivots=programming-language-python-azure).

You will need to install the optional Semantic Kernel azure dependencies if you haven't already via:

```bash
pip install semantic-kernel[azure]
```

Before running an `AzureAIAgent`, modify your .env file to include:

```bash
AZURE_AI_AGENT_PROJECT_CONNECTION_STRING = "<example-connection-string>"
AZURE_AI_AGENT_MODEL_DEPLOYMENT_NAME = "<example-model-deployment-name>"
```

or

```bash
AZURE_AI_AGENT_MODEL_DEPLOYMENT_NAME = "<example-model-deployment-name>"
AZURE_AI_AGENT_ENDPOINT = "<example-endpoint>"
AZURE_AI_AGENT_SUBSCRIPTION_ID = "<example-subscription-id>"
AZURE_AI_AGENT_RESOURCE_GROUP_NAME = "<example-resource-group-name>"
AZURE_AI_AGENT_PROJECT_NAME = "<example-project-name>"
```

The project connection string is of the following format: `<HostName>;<AzureSubscriptionId>;<ResourceGroup>;<ProjectName>`. See here for information on obtaining the values to populate the connection string.

The `.env` should be placed in the root directory.

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Configuring the AI Project Client

Ensure that your `AzureAIAgent` resources are configured with at least a Basic or Standard SKU (the Standard SKU is required to do more advanced operations like AI Search).

To begin, create the project client as follows:

::: zone pivot="programming-language-csharp"

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

::: zone-end

::: zone pivot="programming-language-python"

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

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

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
    # 1. Create an agent on the Azure AI agent service
    agent_definition = await client.agents.create_agent(
        model=ai_agent_settings.model_deployment_name,
        name="<name>",
        instructions="<instructions>",
    )

    # 2. Create a Semantic Kernel agent to use the Azure AI agent
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

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

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

Python also supports invoking an agent in a streaming and a non-streaming fashion:

```python
# Streaming
for user_input in USER_INPUTS:
    await agent.add_chat_message(thread_id=thread.id, message=user_input)
    async for content in agent.invoke_stream(thread_id=thread.id):
        print(content.content, end="", flush=True)
```

```python
# Non-streaming
for user_input in USER_INPUTS:
    await agent.add_chat_message(thread_id=thread.id, message=user_input)
    async for content in agent.invoke(thread_id=thread.id):
        print(content.content)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Using Plugins with an `AzureAIAgent`

Semantic Kernel supports extending an `AzureAIAgent` with custom plugins for enhanced functionality:

::: zone pivot="programming-language-csharp"

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

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

An `AzureAIAgent` can leverage advanced tools such as code interpreters, file search, OpenAPI and Azure AI Search integration for dynamic and powerful interactions:

### Code Interpreter

::: zone pivot="programming-language-csharp"

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

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

::: zone pivot="programming-language-csharp"

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

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

::: zone pivot="programming-language-csharp"

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

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

### AzureAI Search

::: zone pivot="programming-language-csharp"

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

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

### Retrieving Existing `AzureAIAgent`

An existing agent can be retrieved and reused by specifying its assistant ID:

::: zone pivot="programming-language-csharp"

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

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

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

::: zone-end

::: zone pivot="programming-language-python"
```python
await client.agents.delete_thread(thread.id)
await client.agents.delete_agent(agent.id)
```

If working with a vector store or files, they can be deleted as well:

```python
await client.agents.delete_file(file_id=file.id)
await client.agents.delete_vector_store(vector_store_id=vector_store.id)
```

> [!TIP]
> To remove a file from a vector store, use:
> ```python
> await client.agents.delete_vector_store_file(vector_store_id=vector_store.id, file_id=file.id)
> ```
> This operation detaches the file from the vector store but does not permanently delete it.
> To fully delete the file, call:
> ```python
> await client.agents.delete_file(file_id=file.id)
> ```

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## How-To

For practical examples of using an `AzureAIAgent`, see our code samples on GitHub:

::: zone pivot="programming-language-csharp"

> TODO(crickman) Azure AI Agents are currently unavailable in .NET.

::: zone-end

::: zone pivot="programming-language-python"

- [Getting Started with Azure AI Agents](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/getting_started_with_agents/azure_ai_agent)
- [Advanced Azure AI Agent Code Samples](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/concepts/agents/azure_ai_agent)

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

> [!div class="nextstepaction"]
> [Agent Collaboration in `AgentChat`](./agent-chat.md)