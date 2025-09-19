---
title: Exploring the Semantic Kernel OpenAI Responses Agent
description: An exploration of the definition, behaviors, and usage patterns for a `OpenAIResponsesAgent`
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 04/02/2025
ms.service: semantic-kernel
---

# Exploring the Semantic Kernel `OpenAIResponsesAgent`

> [!IMPORTANT]
> This feature is in the experimental stage. Features at this stage are under development and subject to change before advancing to the preview or release candidate stage.

::: zone pivot="programming-language-csharp"

> The `OpenAIResponsesAgent` is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

> [!TIP]
> Detailed API documentation related to this discussion is available at:
>
> - [`AzureResponsesAgent`](/python/api/semantic-kernel/semantic_kernel.agents.open_ai.azure_responses_agent.azureresponsesagent)
> - [`OpenAIResponsesAgent`](/python/api/semantic-kernel/semantic_kernel.agents.open_ai.open_ai_responses_agent.openairesponsesagent)

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## What is a Responses Agent?

The OpenAI Responses API is OpenAI's most advanced interface for generating model responses. It supports text and image inputs, and text outputs. You are able to create stateful interactions with the model, using the output of previous responses as input. It is also possible to extend the model's capabilities with built-in tools for file search, web search, computer use, and more.

- [OpenAI Responses API](https://platform.openai.com/docs/api-reference/responses)
- [Responses API in Azure](/azure/ai-services/openai/how-to/responses?tabs=python-secure)

## Preparing Your Development Environment

To proceed with developing an `OpenAIResponsesAgent`, configure your development environment with the appropriate packages.

::: zone pivot="programming-language-csharp"

> The `OpenAIResponsesAgent` is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

Install the `semantic-kernel` package:

```bash
pip install semantic-kernel
```

> [!Important]
> The `OpenAIResponsesAgent` is supported in Semantic Kernel Python packages 1.27.0 and later.

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Creating an `OpenAIResponsesAgent`

Creating an `OpenAIResponsesAgent` requires first creating a client to be able to talk a remote service.

::: zone pivot="programming-language-csharp"

> The `OpenAIResponsesAgent` is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

To configure the model used by the OpenAI or Azure OpenAI Responses API, new environment variables are introduced:

```bash
OPENAI_RESPONSES_MODEL_ID=""
AZURE_OPENAI_RESPONSES_DEPLOYMENT_NAME=""
```

Set the appropriate variable depending on which provider you're using.

> [!TIP]
> The minimum allowed Azure OpenAI api version is `2025-03-01-preview`. Please visit the following [link](/azure/ai-services/openai/how-to/responses) to view region availability, model support, and further details.

To create an `AzureResponsesAgent` to use with Azure OpenAI models:

```python
from semantic_kernel.agents import AzureResponsesAgent
from semantic_kernel.connectors.ai.open_ai import AzureOpenAISettings, OpenAISettings


# Set up the client and model using Azure OpenAI Resources
client = AzureResponsesAgent.create_client()

# Create the AzureResponsesAgent instance using the client and the model
agent = AzureResponsesAgent(
    ai_model_id=AzureOpenAISettings().responses_deployment_name,
    client=client,
    instructions="your instructions",
    name="name",
)
```

Alternatively, to create an `OpenAIResponsesAgent` to use with OpenAI models:

```python
from semantic_kernel.agents import OpenAIResponsesAgent

# Set up the client and model using OpenAI Resources
client = OpenAIResponsesAgent.create_client()

# Create the OpenAIResponsesAgent instance using the client and the model
agent = OpenAIResponsesAgent(
    ai_model_id=OpenAISettings().responses_model_id,
    client=client,
    instructions="your instructions",
    name="name",
)
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Using an `OpenAIResponsesAgent`

::: zone pivot="programming-language-csharp"

> The `OpenAIResponsesAgent` is coming soon.

::: zone-end

::: zone pivot="programming-language-python"
The OpenAI Responses API supports optional remote storage of conversations. By default, when using a `ResponsesAgentThread`, responses are stored remotely. This enables the use of the Responses API's `previous_response_id` for maintaining context across invocations.

Each conversation is treated as a thread, identified by a unique string ID. All interactions with your `OpenAIResponsesAgent` are scoped to this thread identifier.

The underlying mechanics of the Responses API thread are abstracted by the `ResponsesAgentThread` class, which implements the `AgentThread` interface.

The `OpenAIResponsesAgent` currently only supports threads of type `ResponsesAgentThread`.

You can invoke the `OpenAIResponsesAgent` without specifying an `AgentThread`, to start a new thread and a new `AgentThread` will be returned as part of the response.

```python
from semantic_kernel.agents import AzureResponsesAgent

# Set up the client and model using Azure OpenAI Resources
client = AzureResponsesAgent.create_client()

# Create the AzureResponsesAgent instance using the client and the model
agent = AzureResponsesAgent(
    ai_model_id=AzureOpenAISettings().responses_deployment_name,
    client=client,
    instructions="your instructions",
    name="name",
)

USER_INPUTS = [
    "My name is John Doe.",
    "Tell me a joke",
    "Explain why this is funny.",
    "What have we been talking about?",
]

thread = None

# Generate the agent response(s)
for user_input in USER_INPUTS:
    print(f"# User: '{user_input}'")
    # Invoke the agent for the current message and print the response
    response = await agent.get_response(messages=user_input, thread=thread)
    print(f"# {response.name}: {response.content}")
    # Update the thread so the previous response id is used
    thread = response.thread

# Delete the thread when it is no longer needed
await thread.delete() if thread else None
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Handling Intermediate Messages with an `OpenAIResponsesAgent`

The Semantic Kernel `OpenAIResponsesAgent` is designed to invoke an agent that fulfills user queries or questions. During invocation, the agent may execute tools to derive the final answer. To access intermediate messages produced during this process, callers can supply a callback function that handles instances of `FunctionCallContent` or `FunctionResultContent`.

::: zone pivot="programming-language-csharp"

> The `OpenAIResponsesAgent` is coming soon.

::: zone-end

::: zone pivot="programming-language-python"

Configuring the `on_intermediate_message` callback within `agent.invoke(...)` or `agent.invoke_stream(...)` allows the caller to receive intermediate messages generated during the process of formulating the agent's final response.

```python
import asyncio
from typing import Annotated

from semantic_kernel.agents import AzureResponsesAgent
from semantic_kernel.contents import AuthorRole, FunctionCallContent, FunctionResultContent
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


async def main():
    # 1. Create the client using Azure OpenAI resources and configuration
    client = AzureResponsesAgent.create_client()

    # 2. Create a Semantic Kernel agent for the OpenAI Responses API
    agent = AzureResponsesAgent(
        ai_model_id=AzureOpenAISettings().responses_deployment_name,
        client=client,
        name="Host",
        instructions="Answer questions about the menu.",
        plugins=[MenuPlugin()],
    )

    # 3. Create a thread for the agent
    # If no thread is provided, a new thread will be
    # created and returned with the initial response
    thread = None

    user_inputs = ["Hello", "What is the special soup?", "What is the special drink?", "How much is that?", "Thank you"]

    try:
        for user_input in user_inputs:
            print(f"# {AuthorRole.USER}: '{user_input}'")
            async for response in agent.invoke(
                messages=user_input,
                thread=thread,
                on_intermediate_message=handle_intermediate_steps,
            ):
                thread = response.thread
                print(f"# {response.name}: {response.content}")
    finally:
        await thread.delete() if thread else None


if __name__ == "__main__":
    asyncio.run(main())
```

The following demonstrates sample output from the agent invocation process:

```bash
AuthorRole.USER: 'Hello'
Host: Hi there! How can I assist you with the menu today?
AuthorRole.USER: 'What is the special soup?'
Function Call:> MenuPlugin-get_specials with arguments: {}
Function Result:>
        Special Soup: Clam Chowder
        Special Salad: Cobb Salad
        Special Drink: Chai Tea
        for function: MenuPlugin-get_specials
Host: The special soup is Clam Chowder. Would you like to know more about any other specials?
AuthorRole.USER: 'What is the special drink?'
Host: The special drink is Chai Tea. Would you like any more information?
AuthorRole.USER: 'How much is that?'
Function Call:> MenuPlugin-get_item_price with arguments: {"menu_item":"Chai Tea"}
Function Result:> $9.99 for function: MenuPlugin-get_item_price
Host: The Chai Tea is $9.99. Is there anything else you would like to know?
AuthorRole.USER: 'Thank you'
Host: You're welcome! If you have any more questions, feel free to ask. Enjoy your day!
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

The `OpenAIResponsesAgent` supports instantiation from a YAML declarative specification. The declarative approach allows you to define the agent's properties, instructions, model configuration, tools, and other options in a single, auditable document. This makes agent composition portable and easily managed across environments.

> [!NOTE]
> Any tools, functions, or plugins listed in the declarative YAML must be available to the agent at construction time. For kernel-based plugins, this means they must be registered in the Kernel. For built-in tools such as Code Interpreter or File Search, the correct configuration and credentials must be supplied. The agent loader will not create functions from scratch. If a required component is missing, agent creation will fail.

### How to Use the Declarative Spec

Rather than enumerate every possible YAML configuration, this section outlines the key principles and provides links to concept samples that show complete code for each tool type. Refer to these concept samples for end-to-end implementations of an `OpenAIResponsesAgent` with declarative specs:

`AzureResponsesAgent` samples:

- [File Search](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/openai_responses/azure_openai_responses_agent_declarative_file_search.py)
- [Function Plugin from a File](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/openai_responses/azure_openai_responses_agent_declarative_function_calling_from_file.py)
- [Prompt Template](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/openai_responses/azure_openai_responses_agent_declarative_templating.py)

`OpenAIResponsesAgent` samples:

- [File Search](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/openai_responses/openai_responses_agent_declarative_file_search.py)
- [Function Plugin](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/getting_started_with_agents/openai_responses/step8_responses_agent_declarative.py)
- [Function Plugin from a File](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/openai_responses/openai_responses_agent_declarative_function_calling_from_file.py)
- [Prompt Template](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/openai_responses/openai_responses_agent_declarative_templating.py)
- [Web Search](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/openai_responses/openai_responses_agent_declarative_web_search.py)

#### Example: Creating an AzureAIAgent from YAML

A minimal YAML declarative spec might look like the following:

```yaml
type: openai_responses
name: Host
instructions: Respond politely to the user's questions.
model:
  id: ${OpenAI:ChatModelId}
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
> [Explore Agent Orchestration](../agent-orchestration/index.md)