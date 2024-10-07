---
title: How-To&colon; Coordinate Agent Collaboration using Agent Group Chat (Experimental)
description: TBD $$$
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# How-To: Coordinate Agent Collaboration using Agent Group Chat (Experimental)

> [!WARNING]
> The _Semantic Kernel Agent Framework_ is experimental, still in development and is subject to change.

## Overview

In this sample, we will explore how to use _Agent Group Chat_ to coordinate collboration of three different agents working to author content in response to user direction.  Each agent is assigned a distinct role:

- **Planner**: Responsible for creating outline and providing strucural input
- **Reviewer**: Reviews and provides critical feedback for both _Planner_ and _Writer_ output.
- **Writer**: Authors content based on _Planner_ and _Reviewer_ input.

## Getting Started

Before proceeding with feature coding, make sure your development environment is fully set up and configured.

::: zone pivot="programming-language-csharp"

Start by creating a _Console_ project. Then, include the following package references to ensure all required dependencies are available.

```xml
  <ItemGroup>
    <PackageReference Include="Azure.Identity" Version="<stable>" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="<stable>" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="<stable>" />
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="<stable>" />
    <PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="<stable>" />
    <PackageReference Include="Microsoft.SemanticKernel.Agents.Core" Version="<latest>" />
    <PackageReference Include="Microsoft.SemanticKernel.Agents.OpenAI" Version="<latest>" />
    <PackageReference Include="Microsoft.SemanticKernel.Connectors.AzureOpenAI" Version="<latest>" />
  </ItemGroup>
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## Configuration

This sample requires configuration setting in order to connect to remote services.  You will need to define settings for either _Open AI_ or _Azure Open AI_.

::: zone pivot="programming-language-csharp"

```powershell
# Open AI
dotnet user-secrets set "OpenAISettings:ApiKey" "<api-key>"
dotnet user-secrets set "OpenAISettings:ChatModel" "gpt-4o"

# Azure Open AI
dotnet user-secrets set "AzureOpenAISettings:ApiKey" "<api-key>" # Not required if using token-credential
dotnet user-secrets set "AzureOpenAISettings:Endpoint" "https://lightspeed-team-shared-openai-eastus.openai.azure.com/"
dotnet user-secrets set "AzureOpenAISettings:ChatModelDeployment" "gpt-4o"
```

The following class is used in all of the Agent examples. Be sure to include it in your project to ensure proper functionality. This class serves as a foundational component for the examples that follow.

```c#
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace AgentsSample;

public class Settings
{
    private readonly IConfigurationRoot configRoot;

    private AzureOpenAISettings azureOpenAI;
    private OpenAISettings openAI;

    public AzureOpenAISettings AzureOpenAI => this.azureOpenAI ??= this.GetSettings<Settings.AzureOpenAISettings>();
    public OpenAISettings OpenAI => this.openAI ??= this.GetSettings<Settings.OpenAISettings>();

    public class OpenAISettings
    {
        public string ChatModel { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }

    public class AzureOpenAISettings
    {
        public string ChatModelDeployment { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }

    public TSettings GetSettings<TSettings>() =>
        this.configRoot.GetRequiredSection(typeof(TSettings).Name).Get<TSettings>()!;

    public Settings()
    {
        this.configRoot =
            new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
                .Build();
    }
}
```
::: zone-end

::: zone pivot="programming-language-python"
The quickest way to get started with the proper configuration to run the sample code is to create a `.env` file at the root of your project (where your script is run). 

Configure the following settings in your `.env` file for either Azure OpenAI or OpenAI:

```python
AZURE_OPENAI_API_KEY="..."
AZURE_OPENAI_ENDPOINT="https://..."
AZURE_OPENAI_CHAT_DEPLOYMENT_NAME="..."
AZURE_OPENAI_TEXT_DEPLOYMENT_NAME="..."
AZURE_OPENAI_EMBEDDING_DEPLOYMENT_NAME="..."
AZURE_OPENAI_API_VERSION="..."

OPENAI_API_KEY="sk-..."
OPENAI_ORG_ID=""
OPENAI_CHAT_MODEL_ID=""
```

Once configured, the respective AI service classes will pick up the required variables and use them during instantiation.
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## Coding

The coding process for this sample involves:

1. [Setup](#setup) - Initializing settings and the plug-in.
2. [Agent Definition](#agent-definition) - Create the _Chat_Completion_Agent_ with templatized instructions and plug-in.
3. [The _Chat_ Loop](#the-chat-loop) - Write the loop that drives user / agent interaction.

The full example code is provided in the [Final](#final) section. Refer to that section for the complete implementation.

### Setup

::: zone pivot="programming-language-csharp"
```csharp
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

### Agent Definition

::: zone pivot="programming-language-csharp"
```csharp
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

### The _Chat_ Loop

::: zone pivot="programming-language-csharp"
```csharp
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## Final

Bringing all the steps together, we have the final code for this example. The complete implementation is provided below.

::: zone pivot="programming-language-csharp"
```csharp
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Copyright (c) Microsoft. All rights reserved.

import asyncio
import os

from semantic_kernel.agents import AgentGroupChat, ChatCompletionAgent
from semantic_kernel.agents.strategies.selection.kernel_function_selection_strategy import (
    KernelFunctionSelectionStrategy,
)
from semantic_kernel.agents.strategies.termination.kernel_function_termination_strategy import (
    KernelFunctionTerminationStrategy,
)
from semantic_kernel.agents.strategies.termination.termination_strategy import TerminationStrategy
from semantic_kernel.connectors.ai.open_ai.services.azure_chat_completion import AzureChatCompletion
from semantic_kernel.contents.chat_message_content import ChatMessageContent
from semantic_kernel.contents.utils.author_role import AuthorRole
from semantic_kernel.functions.kernel_function_from_prompt import KernelFunctionFromPrompt
from semantic_kernel.kernel import Kernel

###################################################################
# The following sample demonstrates how to create a simple,       #
# agent group chat that utilizes a Reviewer Chat Completion       #
# Agent along with a Writer Chat Completion Agent to              #
# complete a user's task.                                         #
###################################################################


REVIEWER_NAME = "Reviewer"
COPYWRITER_NAME = "Writer"


def _create_kernel_with_chat_completion(service_id: str) -> Kernel:
    kernel = Kernel()
    kernel.add_service(AzureChatCompletion(service_id=service_id))
    return kernel


async def main():
    agent_reviewer = ChatCompletionAgent(
        service_id=REVIEWER_NAME,
        kernel=_create_kernel_with_chat_completion(REVIEWER_NAME),
        name=REVIEWER_NAME,
        instructions="""
            Your responsiblity is to review and identify how to improve user provided content.
            If the user has providing input or direction for content already provided, specify how to 
            address this input.
            Never directly perform the correction or provide example.
            Once the content has been updated in a subsequent response, you will review the content 
            again until satisfactory.
            Always copy satisfactory content to the clipboard using available tools and inform user.

            RULES:
            - Only identify suggestions that are specific and actionable.
            - Verify previous suggestions have been addressed.
            - Never repeat previous suggestions.
            """,
    )

    agent_writer = ChatCompletionAgent(
        service_id=COPYWRITER_NAME,
        kernel=_create_kernel_with_chat_completion(COPYWRITER_NAME),
        name=COPYWRITER_NAME,
        instructions="""
            Your sole responsiblity is to rewrite content according to review suggestions.

            - Always apply all review direction.
            - Always revise the content in its entirety without explanation.
            - Never address the user.
            """,
    )

    selection_function = KernelFunctionFromPrompt(
        function_name="selection",
        prompt=f"""
        Determine which participant takes the next turn in a conversation based on the the most recent participant.
        State only the name of the participant to take the next turn.
        No participant should take more than one turn in a row.
        
        Choose only from these participants:
        - {REVIEWER_NAME}
        - {COPYWRITER_NAME}
        
        Always follow these rules when selecting the next participant:
        - After user input, it is {COPYWRITER_NAME}'s turn.
        - After {COPYWRITER_NAME} replies, it is {REVIEWER_NAME}'s turn.
        - After {REVIEWER_NAME} provides feedback, it is {COPYWRITER_NAME}'s turn.

        History:
        {{{{$history}}}}
        """,
    )

    TERMINATION_KEYWORD = "yes"

    termination_function = KernelFunctionFromPrompt(
        function_name="termination",
        prompt=f"""
            Examine the RESPONSE and determine whether the content has been deemed satisfactory.
            If content is satisfactory, respond with a single word without explanation: {TERMINATION_KEYWORD}.
            If specific suggestions are being provided, it is not satisfactory.
            If no correction is suggested, it is satisfactory.

            RESPONSE:
            {{{{$history}}}}
            """,
    )

    chat = AgentGroupChat(
        agents=[agent_writer, agent_reviewer],
        selection_strategy=KernelFunctionSelectionStrategy(
            function=selection_function,
            kernel=_create_kernel_with_chat_completion("selection"),
            result_parser=lambda result: str(result.value[0]) if result.value is not None else COPYWRITER_NAME,
            agent_variable_name="agents",
            history_variable_name="history",
        ),
        termination_strategy=KernelFunctionTerminationStrategy(
            agents=[agent_reviewer],
            function=termination_function,
            kernel=_create_kernel_with_chat_completion("termination"),
            result_parser=lambda result: TERMINATION_KEYWORD in str(result.value[0]).lower(),
            history_variable_name="history",
            maximum_iterations=10,
        ),
    )

    is_complete: bool = False
    while not is_complete:
        user_input = input("User:> ")
        if not user_input:
            continue

        if user_input.lower() == "exit":
            is_complete = True
            break

        if user_input.lower() == "reset":
            await chat.reset()
            print("[Conversation has been reset]")
            continue

        if user_input.startswith("@") and len(input) > 1:
            file_path = input[1:]
            try:
                if not os.path.exists(file_path):
                    print(f"Unable to access file: {file_path}")
                    continue
                with open(file_path) as file:
                    user_input = file.read()
            except Exception:
                print(f"Unable to access file: {file_path}")
                continue

        await chat.add_chat_message(ChatMessageContent(role=AuthorRole.USER, content=user_input))

        async for response in chat.invoke():
            print(f"# {response.role} - {response.name or '*'}: '{response.content}'")

        if chat.is_complete:
            is_complete = True
            break


if __name__ == "__main__":
    asyncio.run(main())
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end