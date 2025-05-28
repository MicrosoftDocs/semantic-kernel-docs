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
> This feature is in the experimental stage. Features at this stage are under development and subject to change before advancing to the preview or release candidate stage.

::: zone pivot="programming-language-csharp"

> [!TIP]
> Detailed API documentation related to this discussion is available at:
>
> - [`AzureAIAgent`](/dotnet/api/microsoft.semantickernel.agents.azureai)

::: zone-end

::: zone pivot="programming-language-python"

> [!TIP]
> Detailed API documentation related to this discussion is available at:
>
> - [`AzureAIAgent`](/python/api/semantic-kernel/semantic_kernel.agents.azure_ai.azure_ai_agent.azureaiagent)

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

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

> Feature currently unavailable in Java.

::: zone-end

## Configuring the AI Project Client

Accessing an `AzureAIAgent` first requires the creation of a client that is configured for a specific Foundry Project, most commonly by providing your project endpoint ([The Azure AI Foundry SDK: Getting Started with Projects](/azure/ai-foundry/how-to/develop/sdk-overview#get-started-with-projects)).

::: zone pivot="programming-language-csharp"

```csharp
PersistentAgentsClient client = AzureAIAgent.CreateAgentsClient("<your endpoint>", new AzureCliCredential());
```

::: zone-end

::: zone pivot="programming-language-python"

Modify your the `.env` file in the root directory to include:

```bash
AZURE_AI_AGENT_ENDPOINT = "<example-endpoint>"
AZURE_AI_AGENT_MODEL_DEPLOYMENT_NAME = "<example-model-deployment-name>"
```

Once the configuration is defined, the client may be created:

```python
from semantic_kernel.agents import AzureAIAgent

async with (
    DefaultAzureCredential() as creds,
    AzureAIAgent.create_client(credential=creds) as client,
):
    # Your operational code here
```

The underlying `endpoint` will be picked up by Pydantic Settings if configured. Otherwise you may explicitly pass it in to the `create_client()` method:

```python
from semantic_kernel.agents import AzureAIAgent

async with (
    DefaultAzureCredential() as creds,
    AzureAIAgent.create_client(credential=creds, endpoint="<your-endpoint>") as client,
):
    # Your operational code here
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Creating an `AzureAIAgent`

To create an `AzureAIAgent`, you start by configuring and initializing the Foundry project through the Azure Agent service and then integrate it with Semantic Kernel:

::: zone pivot="programming-language-csharp"

```csharp
PersistentAgentsClient client = AzureAIAgent.CreateAgentsClient("<your endpoint>", new AzureCliCredential());

// 1. Define an agent on the Azure AI agent service
PersistentAgent definition = await agentsClient.Administration.CreateAgentAsync(
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
from semantic_kernel.agents import AzureAIAgent, AzureAIAgentSettings

async with (
    DefaultAzureCredential() as creds,
    AzureAIAgent.create_client(credential=creds) as client,
):
    # 1. Define an agent on the Azure AI agent service
    agent_definition = await client.agents.create_agent(
        model=AzureAIAgentSettings().model_deployment_name,
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

> Feature currently unavailable in Java.

::: zone-end

## Interacting with an `AzureAIAgent`

Interaction with the `AzureAIAgent` is straightforward. The agent maintains the conversation history automatically using a thread.

::: zone pivot="programming-language-csharp"
The specifics of the _Azure AI Agent thread_ is abstracted away via the `Microsoft.SemanticKernel.Agents.AzureAI.AzureAIAgentThread` class, which is an implementation of `Microsoft.SemanticKernel.Agents.AgentThread`.

> [!IMPORTANT]
> Note that the Azure AI Agents SDK has the `PersistentAgentThread` class. It should not be confused with `Microsoft.SemanticKernel.Agents.AgentThread`, which is the common Semantic Kernel Agents abstraction for all thread types.

The `AzureAIAgent` currently only supports threads of type `AzureAIAgentThread`.

```csharp
AzureAIAgentThread agentThread = new(agent.Client);
try
{
    ChatMessageContent message = new(AuthorRole.User, "<your user input>");
    await foreach (ChatMessageContent response in agent.InvokeAsync(message, agentThread))
    {
        Console.WriteLine(response.Content);
    }
}
finally
{
    await agentThread.DeleteAsync();
    await agent.Client.DeleteAgentAsync(agent.Id);
}
```

::: zone-end

::: zone pivot="programming-language-python"
The specifics of the _Azure AI Agent thread_ is abstracted away via the `AzureAIAgentThread` class, which is an implementation of `AgentThread`.

```python
USER_INPUTS = ["Hello", "What's your name?"]

thread: AzureAIAgentThread = AzureAIAgentThread()

try:
    for user_input in USER_INPUTS:
        response = await agent.get_response(messages=user_inputs, thread=thread)
        print(response)
        thread = response.thread
finally:
    await thread.delete() if thread else None
```

Optionally, an agent may be invoked as:

```python
for user_input in USER_INPUTS:
    async for content in agent.invoke(messages=user_input, thread=thread):
        print(content.content)
        thread = response.thread
```

You may also pass in a list of messages to the `get_response(...)`, `invoke(...)`, or `invoke_stream(...)` methods:

```python
USER_INPUTS = ["Hello", "What's your name?"]

thread: AzureAIAgentThread = AzureAIAgentThread()

try:
    for user_input in USER_INPUTS:
        response = await agent.get_response(messages=USER_INPUTS, thread=thread)
        print(response)
        thread = response.thread
finally:
    await thread.delete() if thread else None
```

::: zone-end

An agent may also produce a streamed response:

::: zone pivot="programming-language-csharp"

```csharp
ChatMessageContent message = new(AuthorRole.User, "<your user input>");
await foreach (StreamingChatMessageContent response in agent.InvokeStreamingAsync(message, agentThread))
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

> Feature currently unavailable in Java.

::: zone-end

## Using Plugins with an `AzureAIAgent`

Semantic Kernel supports extending an `AzureAIAgent` with custom plugins for enhanced functionality:

::: zone pivot="programming-language-csharp"

```csharp
KernelPlugin plugin = KernelPluginFactory.CreateFromType<YourPlugin>();
PersistentAgentsClient client = AzureAIAgent.CreateAgentsClient("<your endpoint>", new AzureCliCredential());

PersistentAgent definition = await agentsClient.Administration.CreateAgentAsync(
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

async with (
        DefaultAzureCredential() as creds,
        AzureAIAgent.create_client(credential=creds) as client,
    ):
        agent_definition = await client.agents.create_agent(
            model=AzureAIAgentSettings().model_deployment_name,
        )

        agent = AzureAIAgent(
            client=client,
            definition=agent_definition,
            plugins=[SamplePlugin()]
        )
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Advanced Features

An `AzureAIAgent` can leverage advanced tools such as:

- [Code Interpreter](#code-interpreter)
- [File Search](#file-search)
- [OpenAPI integration](#openapi-integration)
- [Azure AI Search integration](#azureai-search-integration)
- [Bing Grounding](#bing-grounding)

### Code Interpreter

Code Interpreter allows the agents to write and run Python code in a sandboxed execution environment ([Azure AI Agent Service Code Interpreter](/azure/ai-services/agents/how-to/tools/code-interpreter)).

::: zone pivot="programming-language-csharp"

```csharp
PersistentAgentsClient client = AzureAIAgent.CreateAgentsClient("<your endpoint>", new AzureCliCredential());

PersistentAgent definition = await agentsClient.CreateAgentAsync(
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

> Feature currently unavailable in Java.

::: zone-end

### File Search

File search augments agents with knowledge from outside its model ([Azure AI Agent Service File Search Tool](/azure/ai-services/agents/how-to/tools/file-search)).

::: zone pivot="programming-language-csharp"

```csharp
PersistentAgentsClient client = AzureAIAgent.CreateAgentsClient("<your endpoint>", new AzureCliCredential());

PersistentAgent definition = await agentsClient.CreateAgentAsync(
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
        });

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

> Feature currently unavailable in Java.

::: zone-end

### OpenAPI Integration

Connects your agent to an external API ([How to use Azure AI Agent Service with OpenAPI Specified Tools](/azure/ai-services/agents/how-to/tools/openapi-spec)).

::: zone pivot="programming-language-csharp"

```csharp
PersistentAgentsClient client = AzureAIAgent.CreateAgentsClient("<your endpoint>", new AzureCliCredential());

string apiJsonSpecification = ...; // An Open API JSON specification

PersistentAgent definition = await agentsClient.CreateAgentAsync(
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
    ]
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

> Feature currently unavailable in Java.

::: zone-end

### AzureAI Search Integration

Use an existing Azure AI Search index with with your agent ([Use an existing AI Search index](/azure/ai-services/agents/how-to/tools/azure-ai-search)).

::: zone pivot="programming-language-csharp"

```csharp
PersistentAgentsClient client = AzureAIAgent.CreateAgentsClient("<your endpoint>", new AzureCliCredential());

PersistentAgent definition = await agentsClient.CreateAgentAsync(
    "<name of the the model used by the agent>",
    name: "<agent name>",
    description: "<agent description>",
    instructions: "<agent instructions>",
    tools: [new AzureAISearchToolDefinition()],
    toolResources: new()
    {
        AzureAISearch = new()
        {
            IndexList = { new AISearchIndexResource("<your connection id>", "<your index name>") }
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

> Feature currently unavailable in Java.

::: zone-end

### Bing Grounding

::: zone pivot="programming-language-csharp"

> Example coming soon.

::: zone-end

::: zone pivot="programming-language-python"

```python
from azure.ai.agents.models import BingGroundingTool
from azure.identity.aio import DefaultAzureCredential

from semantic_kernel.agents import AzureAIAgent, AzureAIAgentSettings

async with (
    DefaultAzureCredential() as creds,
    AzureAIAgent.create_client(credential=creds) as client,
):
    # 1. Enter your Bing Grounding Connection Name
    bing_connection = await client.connections.get(connection_name="<your-bing-grounding-connection-name>")
    conn_id = bing_connection.id

    # 2. Initialize agent bing tool and add the connection id
    bing_grounding = BingGroundingTool(connection_id=conn_id)

    # 3. Create an agent with Bing grounding on the Azure AI agent service
    agent_definition = await client.agents.create_agent(
        name="BingGroundingAgent",
        instructions="Use the Bing grounding tool to answer the user's question.",
        model=AzureAIAgentSettings().model_deployment_name,
        tools=bing_grounding.definitions,
    )

    # 4. Create a Semantic Kernel agent for the Azure AI agent
    agent = AzureAIAgent(
        client=client,
        definition=agent_definition,
    )
```

When using the Bing Grounding tool, the `FunctionCallContent` passed to the `on_intermediate_message` callback will have its function name set to `"bing_grounding"`. After the run completes, the `ChatMessageContent.items` list will include either `AnnotationContent` or `StreamingAnnotationContent`, depending on whether the invocation is standard or streaming. These annotation items contain information about the links the agent visited during the response, similar to the information present in the `FunctionCallContent`.

For more information, see the following concept samples:
- [AzureAIAgent with Bing Grounding](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/azure_ai_agent/azure_ai_agent_bing_grounding.py)
- [AzureAIAgent Streaming with Bing Grounding](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/azure_ai_agent/azure_ai_agent_bing_grounding_streaming_with_message_callback.py)


::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

### Retrieving an Existing `AzureAIAgent`

An existing agent can be retrieved and reused by specifying its assistant ID:

::: zone pivot="programming-language-csharp"

```csharp
PersistentAgent definition = await agentsClient.Administration.GetAgentAsync("<your agent id>");
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

> Feature currently unavailable in Java.

::: zone-end

## Deleting an `AzureAIAgent`

Agents and their associated threads can be deleted when no longer needed:

::: zone pivot="programming-language-csharp"

```csharp
await agentThread.DeleteAsync();
await agentsClient.Administration.DeleteAgentAsync(agent.Id);
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

```csharp
await agentsClient.VectorStores.DeleteVectorStoreAsync("<your store id>");
await agentsClient.Files.DeleteFileAsync("<your file id>");
```

::: zone-end

::: zone pivot="programming-language-python"

```python
await client.agents.files.delete(file_id=file.id)
await client.agents.vector_stores.delete(vector_store_id=vector_store.id)
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

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

> Feature currently unavailable in Java.

::: zone-end

## Handling Intermediate Messages with an `AzureAIAgent`

The Semantic Kernel `AzureAIAgent` is designed to invoke an agent that fulfills user queries or questions. During invocation, the agent may execute tools to derive the final answer. To access intermediate messages produced during this process, callers can supply a callback function that handles instances of `FunctionCallContent` or `FunctionResultContent`.

::: zone pivot="programming-language-csharp"
> Callback documentation for the `AzureAIAgent` is coming soon.
::: zone-end

::: zone pivot="programming-language-python"

Configuring the `on_intermediate_message` callback within `agent.invoke(...)` or `agent.invoke_stream(...)` allows the caller to receive intermediate messages generated during the process of formulating the agent's final response.

```python
import asyncio
from typing import Annotated

from azure.identity.aio import DefaultAzureCredential

from semantic_kernel.agents import AzureAIAgent, AzureAIAgentSettings, AzureAIAgentThread
from semantic_kernel.contents import FunctionCallContent, FunctionResultContent
from semantic_kernel.contents.chat_message_content import ChatMessageContent
from semantic_kernel.functions import kernel_function


# Define a sample plugin for the sample
class MenuPlugin:
    """A sample Menu Plugin used for the concept sample."""

    @kernel_function(description="Provides a list of specials from the menu.")
    def get_specials(self) -> Annotated[str, "Returns the specials from the menu."]:
        return """
        Special Soup: Clam Chowder
        Special Salad: Cobb Salad
        Special Drink: Chai Tea
        """

    @kernel_function(description="Provides the price of the requested menu item.")
    def get_item_price(
        self, menu_item: Annotated[str, "The name of the menu item."]
    ) -> Annotated[str, "Returns the price of the menu item."]:
        return "$9.99"


# This callback function will be called for each intermediate message,
# which will allow one to handle FunctionCallContent and FunctionResultContent.
# If the callback is not provided, the agent will return the final response
# with no intermediate tool call steps.
async def handle_intermediate_steps(message: ChatMessageContent) -> None:
    for item in message.items or []:
        if isinstance(item, FunctionResultContent):
            print(f"Function Result:> {item.result} for function: {item.name}")
        elif isinstance(item, FunctionCallContent):
            print(f"Function Call:> {item.name} with arguments: {item.arguments}")
        else:
            print(f"{item}")


async def main() -> None:
    ai_agent_settings = AzureAIAgentSettings()

    async with (
        DefaultAzureCredential() as creds,
        AzureAIAgent.create_client(credential=creds, endpoint=ai_agent_settings.endpoint) as client,
    ):
        AGENT_NAME = "Host"
        AGENT_INSTRUCTIONS = "Answer questions about the menu."

        # Create agent definition
        agent_definition = await client.agents.create_agent(
            model=ai_agent_settings.deployment_name,
            name=AGENT_NAME,
            instructions=AGENT_INSTRUCTIONS,
        )

        # Create the AzureAI Agent
        agent = AzureAIAgent(
            client=client,
            definition=agent_definition,
            plugins=[MenuPlugin()],  # add the sample plugin to the agent
        )

        # Create a thread for the agent
        # If no thread is provided, a new thread will be
        # created and returned with the initial response
        thread: AzureAIAgentThread = None

        user_inputs = [
            "Hello",
            "What is the special soup?",
            "How much does that cost?",
            "Thank you",
        ]

        try:
            for user_input in user_inputs:
                print(f"# User: '{user_input}'")
                async for response in agent.invoke(
                    messages=user_input,
                    thread=thread,
                    on_intermediate_message=handle_intermediate_steps,
                ):
                    print(f"# Agent: {response}")
                    thread = response.thread
        finally:
            # Cleanup: Delete the thread and agent
            await thread.delete() if thread else None
            await client.agents.delete_agent(agent.id)


if __name__ == "__main__":
    asyncio.run(main())
```

The following demonstrates sample output from the agent invocation process:

```bash
User: 'Hello'
Agent: Hi there! How can I assist you today?
User: 'What is the special soup?'
Function Call:> MenuPlugin-get_specials with arguments: {}
Function Result:> 
        Special Soup: Clam Chowder
        Special Salad: Cobb Salad
        Special Drink: Chai Tea
        for function: MenuPlugin-get_specials
Agent: The special soup is Clam Chowder. Would you like to know anything else about the menu?
User: 'How much does that cost?'
Function Call:> MenuPlugin-get_item_price with arguments: {"menu_item":"Clam Chowder"}
Function Result:> $9.99 for function: MenuPlugin-get_item_price
Agent: The Clam Chowder costs $9.99. Let me know if you would like assistance with anything else!
User: 'Thank you'
Agent: You're welcome! Enjoy your meal! 😊
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Declarative Spec

::: zone pivot="programming-language-csharp"

> The documentation on using declarative specs is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

> [!IMPORTANT]
> This feature is in the experimental stage. Features at this stage are under development and subject to change before advancing to the preview or release candidate stage.

The `AzureAIAgent` supports instantiation from a YAML declarative specification. The declarative approach allows you to define the agent's properties, instructions, model configuration, tools, and other options in a single, auditable document. This makes agent composition portable and easily managed across environments.

> [!NOTE]
> Any tools, functions, or plugins listed in the declarative YAML must be available to the agent at construction time. For kernel-based plugins, this means they must be registered in the Kernel. For built-in tools such as Bing Grounding, File Search, or OpenAPI tools, the correct configuration and credentials must be supplied. The agent loader will not create functions from scratch. If a required component is missing, agent creation will fail.

### How to Use the Declarative Spec

Rather than enumerate every possible YAML configuration, this section outlines the key principles and provides links to concept samples that show complete code for each tool type. Refer to these concept samples for end-to-end implementations of an `AzureAIAgent` with declarative specs:

- [Function Plugin](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/getting_started_with_agents/azure_ai_agent/step8_azure_ai_agent_declarative.py)
- [Function Plugin from a File](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/azure_ai_agent/azure_ai_agent_declarative_function_calling_from_file.py)
- [AI Search](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/azure_ai_agent/azure_ai_agent_declarative_azure_ai_search.py)
- [Bing Grounding](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/azure_ai_agent/azure_ai_agent_declarative_bing_grounding.py)
- [Code Interpreter](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/azure_ai_agent/azure_ai_agent_declarative_code_interpreter.py)
- [File Search](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/azure_ai_agent/azure_ai_agent_declarative_file_search.py)
- [OpenAPI](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/azure_ai_agent/azure_ai_agent_declarative_openapi.py)
- [Prompt Template](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/azure_ai_agent/azure_ai_agent_declarative_templating.py)
- [Load from Existing Agent ID](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/azure_ai_agent/azure_ai_agent_declarative_with_existing_agent_id.py)

#### Example: Creating an AzureAIAgent from YAML

A minimal YAML declarative spec might look like the following:

```yaml
type: foundry_agent
name: MyAgent
instructions: Respond politely to the user's questions.
model:
  id: ${AzureAI:ChatModelId}
tools:
  - id: MenuPlugin.get_specials
    type: function
  - id: MenuPlugin.get_item_price
    type: function
```

For details on how to wire up the agent, refer to the full code samples above.

### Key Points
- Declarative specs allow defining agent structure, tools, and behavior in YAML.
- All referenced tools and plugins must be registered or accessible at runtime.
- Built-in tools such as Bing, File Search, and Code Interpreter require proper configuration and credentials (often via environment variables or explicit arguments).
- For comprehensive examples, see the provided sample links which demonstrate practical scenarios, including plugin registration, Azure identity configuration, and advanced tool use.

::: zone-end

::: zone pivot="programming-language-java"

> This feature is unavailable.

::: zone-end


## Next Steps

> [!div class="nextstepaction"]
> [Explore the OpenAI Responses Agent](./responses-agent.md)