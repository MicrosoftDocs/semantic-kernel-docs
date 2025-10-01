---
title: Microsoft Agent Framework Agent Types
titleSuffix: Azure AI Foundry
description: Learn different Agent Framework agent types.
ms.service: agent-framework
ms.topic: tutorial
ms.date: 09/04/2025
ms.reviewer: ssalgado
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.author: taochen
---

# Microsoft Agent Framework Agent Types

The Microsoft Agent Framework provides support for several types of agents to accommodate different use cases and requirements.

All agents are derived from a common base class, `AIAgent`, which provides a consistent interface for all agent types. This allows for building common, agent agnostic, higher level functionality such as multi-agent orchestrations.

> [!IMPORTANT]
> If you use the Microsoft Agent Framework to build applications that operate with third-party servers or agents, you do so at your own risk. We recommend reviewing all data being shared with third-party servers or agents and being cognizant of third-party practices for retention and location of data. It is your responsibility to manage whether your data will flow outside of your organization’s Azure compliance and geographic boundaries and any related implications.

::: zone pivot="programming-language-csharp"

## Simple agents based on inference services

The agent framework makes it easy to create simple agents based on many different inference services.
Any inference service that provides an [`Microsoft.Extensions.AI.IChatClient`](/dotnet/ai/microsoft-extensions-ai#the-ichatclient-interface) implementation can be used to build these agents. The `Microsoft.Agents.AI.ChatClientAgent` is the agent class used to provide an agent for any `IChatClient` implementation.

These agents support a wide range of functionality out of the box:

1. Function calling
1. Multi-turn conversations with local chat history management or service provided chat history management
1. Custom service provided tools (e.g. MCP, Code Execution)
1. Structured output

To create one of these agents, simply construct a `ChatClientAgent` using the ChatClient implementation of your choice.

```csharp
using Microsoft.Agents.AI;

var agent = new ChatClientAgent(chatClient, instructions: "You are a helpful assistant");
```

For many popular services, we also have helpers to make creating these agents even easier.
See the documentation for each service, for more information:

|Underlying Inference Service|Description|Service Chat History storage supported|Custom Chat History storage supported|
|---|---|---|---|
|[Azure AI Foundry Agent](./azure-ai-foundry-agent.md)|An agent that uses the Azure AI Foundry Agents Service as its backend.|Yes|No|
|[Azure OpenAI ChatCompletion](./azure-openai-chat-completion-agent.md)|An agent that uses the Azure OpenAI ChatCompletion service.|No|Yes|
|[Azure OpenAI Responses](./azure-openai-responses-agent.md)|An agent that uses the Azure OpenAI Responses service.|Yes|Yes|
|[OpenAI ChatCompletion](./openai-chat-completion-agent.md)|An agent that uses the OpenAI ChatCompletion service.|No|Yes|
|[OpenAI Responses](./openai-responses-agent.md)|An agent that uses the OpenAI Responses service.|Yes|Yes|
|[OpenAI Assistants](./openai-assistants-agent.md)|An agent that uses the OpenAI Assistants service.|Yes|No|
|[Any other ChatClient](./chat-client-agent.md)|You can also use any other [`Microsoft.Extensions.AI.IChatClient`](/dotnet/ai/microsoft-extensions-ai#the-ichatclient-interface) implementation to create an agent.|Varies|Varies|

## Complex custom agents

It is also possible to create fully custom agents, that are not just wrappers around a ChatClient.
The agent framework provides the `AIAgent` base type.
This base type is the core abstraction for all agents, which when subclassed allows for complete control over the agent's behavior and capabilities.

See the documentation for [Custom Agents](./custom-agent.md) for more information.

## Proxies for remote agents

The agent framework provides out of the box `AIAgent` implementations for common service hosted agent protocols,
such as A2A. This way you can easily connect to and use remote agents from your application.

See the documentation for each agent type, for more information:

|Protocol|Description|
|---|---|
|[A2A](./a2a-agent.md)|An agent that serves as a proxy to a remote agent via the A2A protocol.|

::: zone-end

::: zone pivot="programming-language-python"

## Simple agents based on inference services

The agent framework makes it easy to create simple agents based on many different inference services.
Any inference service that provides a chat client implementation can be used to build these agents.

These agents support a wide range of functionality out of the box:

1. Function calling
1. Multi-turn conversations with local chat history management or service provided chat history management
1. Custom service provided tools (e.g. MCP, Code Execution)
1. Structured output
1. Streaming responses

To create one of these agents, simply construct a `ChatAgent` using the chat client implementation of your choice.

```python
from agent_framework import ChatAgent
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import DefaultAzureCredential

async with (
    DefaultAzureCredential() as credential,
    ChatAgent(
        chat_client=AzureAIAgentClient(async_credential=credential),
        instructions="You are a helpful assistant"
    ) as agent
):
    response = await agent.run("Hello!")
```

Alternatively, you can use the convenience method on the chat client:

```python
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import DefaultAzureCredential

async with DefaultAzureCredential() as credential:
    agent = AzureAIAgentClient(async_credential=credential).create_agent(
        instructions="You are a helpful assistant"
    )
```

For detailed examples, see the agent-specific documentation sections below.

### Supported Agent Types

|Underlying Inference Service|Description|Service Chat History storage supported|Custom Chat History storage supported|
|---|---|---|---|
|[Azure AI Agent](./azure-ai-foundry-agent.md)|An agent that uses the Azure AI Agents Service as its backend.|Yes|No|
|[Azure OpenAI Chat Completion](./azure-openai-chat-completion-agent.md)|An agent that uses the Azure OpenAI Chat Completion service.|No|Yes|
|[Azure OpenAI Responses](./azure-openai-responses-agent.md)|An agent that uses the Azure OpenAI Responses service.|Yes|Yes|
|[OpenAI Chat Completion](./openai-chat-completion-agent.md)|An agent that uses the OpenAI Chat Completion service.|No|Yes|
|[OpenAI Responses](./openai-responses-agent.md)|An agent that uses the OpenAI Responses service.|Yes|Yes|
|[OpenAI Assistants](./openai-assistants-agent.md)|An agent that uses the OpenAI Assistants service.|Yes|No|
|[Any other ChatClient](./chat-client-agent.md)|You can also use any other chat client implementation to create an agent.|Varies|Varies|

### Function Tools

You can provide function tools to agents for enhanced capabilities:

```python
from typing import Annotated
from pydantic import Field
from azure.identity.aio import DefaultAzureCredential
from agent_framework.azure import AzureAIAgentClient

def get_weather(location: Annotated[str, Field(description="The location to get the weather for.")]) -> str:
    """Get the weather for a given location."""
    return f"The weather in {location} is sunny with a high of 25°C."

async with (
    DefaultAzureCredential() as credential,
    AzureAIAgentClient(async_credential=credential).create_agent(
        instructions="You are a helpful weather assistant.",
        tools=get_weather
    ) as agent
):
    response = await agent.run("What's the weather in Seattle?")
```

For complete examples with function tools, see:

- [Azure AI with function tools](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/agents/azure_ai/azure_ai_with_function_tools.py)
- [Azure OpenAI with function tools](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/agents/azure_openai/azure_openai_with_function_tools.py)
- [OpenAI with function tools](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/agents/openai/openai_with_function_tools.py)

### Streaming Responses

Agents support both regular and streaming responses:

```python
# Regular response (wait for complete result)
response = await agent.run("What's the weather like in Seattle?")
print(response.text)

# Streaming response (get results as they are generated)
async for chunk in agent.run_stream("What's the weather like in Portland?"):
    if chunk.text:
        print(chunk.text, end="", flush=True)
```

For streaming examples, see:

- [Azure AI streaming examples](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/agents/azure_ai/azure_ai_basic.py)
- [Azure OpenAI streaming examples](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/agents/azure_openai/azure_openai_basic.py)
- [OpenAI streaming examples](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/agents/openai/openai_basic.py)

### Code Interpreter Tools

Azure AI agents support hosted code interpreter tools for executing Python code:

```python
from agent_framework import ChatAgent, HostedCodeInterpreterTool
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import DefaultAzureCredential

async with (
    DefaultAzureCredential() as credential,
    ChatAgent(
        chat_client=AzureAIAgentClient(async_credential=credential),
        instructions="You are a helpful assistant that can execute Python code.",
        tools=HostedCodeInterpreterTool()
    ) as agent
):
    response = await agent.run("Calculate the factorial of 100 using Python")
```

For code interpreter examples, see:

- [Azure AI with code interpreter](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/agents/azure_ai/azure_ai_with_code_interpreter.py)
- [Azure OpenAI Assistants with code interpreter](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/agents/azure_assistants_client/azure_assistants_with_code_interpreter.py)
- [OpenAI Assistants with code interpreter](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/agents/openai_assistants_client/openai_assistants_with_code_interpreter.py)

## Custom agents

It is also possible to create fully custom agents that are not just wrappers around a chat client.
Agent Framework provides the `AgentProtocol` protocol and `BaseAgent` base class, which when implemented/subclassed allows for complete control over the agent's behavior and capabilities.

```python
from agent_framework import BaseAgent, AgentRunResponse, AgentRunResponseUpdate, AgentThread, ChatMessage
from collections.abc import AsyncIterable

class CustomAgent(BaseAgent):
    async def run(
        self,
        messages: str | ChatMessage | list[str] | list[ChatMessage] | None = None,
        *,
        thread: AgentThread | None = None,
        **kwargs: Any,
    ) -> AgentRunResponse:
        # Custom agent implementation
        pass

    def run_stream(
        self,
        messages: str | ChatMessage | list[str] | list[ChatMessage] | None = None,
        *,
        thread: AgentThread | None = None,
        **kwargs: Any,
    ) -> AsyncIterable[AgentRunResponseUpdate]:
        # Custom streaming implementation
        pass
```

::: zone-end
