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

In this sample, we will explore how to use _Agent Group Chat_ to coordinate collboration of two different agents working to review and rewrite user provided content.  Each agent is assigned a distinct role:

- **Reviewer**: Reviews and provides direction to _Writer_.
- **Writer**: Updates user content based on _Reviewer_ input.

The approach will be broken down step-by-step to high-light the key parts of the coding process.

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
dotnet user-secrets set "AzureOpenAISettings:Endpoint" "<model-endpoint>"
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
2. [_Agent_ Definition](#agent-definition) - Create the two _Chat Completion Agent_ instances (_Reviewer_ and _Writer_).
3. [_Chat_ Definition](#chat-definition) - Create the _Agent Group Chat_ and associated strategies.
4. [The _Chat_ Loop](#the-chat-loop) - Write the loop that drives user / agent interaction.

The full example code is provided in the [Final](#final) section. Refer to that section for the complete implementation.

### Setup


Prior to creating any _Chat Completion Agent_, the configuration settings, plugins, and _Kernel_ must be initialized.

::: zone pivot="programming-language-csharp"

First, simply initialize the `Settings` class referenced in the previous [Configuration](#configuration) section.

```csharp
Settings settings = new();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Now initialize a `Kernel` instance with an `IChatCompletionService`.

::: zone pivot="programming-language-csharp"
```csharp
IKernelBuilder builder = Kernel.CreateBuilder();

builder.AddAzureOpenAIChatCompletion(
	settings.AzureOpenAI.ChatModelDeployment,
	settings.AzureOpenAI.Endpoint,
	new AzureCliCredential());

Kernel kernel = builder.Build();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Let's also create a second _Kernel_ instance via _cloning_ and add a plug-in that will allow the reivew to place updated content on the clip-board.

::: zone pivot="programming-language-csharp"
```csharp
Kernel toolKernel = kernel.Clone();
toolKernel.Plugins.AddFromType<ClipboardAccess>();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

The _Clipboard_ plugin may be defined as part of the sample.

::: zone pivot="programming-language-csharp"
```csharp
private sealed class ClipboardAccess
{
    [KernelFunction]
    [Description("Copies the provided content to the clipboard.")]
    public static void SetClipboard(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        using Process clipProcess = Process.Start(
            new ProcessStartInfo
            {
                FileName = "clip",
                RedirectStandardInput = true,
                UseShellExecute = false,
            });

        clipProcess.StandardInput.Write(content);
        clipProcess.StandardInput.Close();
    }
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

### Agent Definition

Let's declare the agent names as `const` so they might be referenced in _Agent Group Chat_ strategies:

::: zone pivot="programming-language-csharp"
```csharp
const string ReviewerName = "Reviewer";
const string WriterName = "Writer";
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Defining the _Reviewer_ agent uses the pattern explored in [How-To: Chat Completion Agent](./example-chat-agent.md).

Here the _Reviewer_ is given the role of responding to user input, providing direction to the _Writer_ agent, and verifying result of the _Writer_ agent.

::: zone pivot="programming-language-csharp"
```csharp
ChatCompletionAgent agentReviewer =
    new()
    {
        Name = ReviewerName,
        Instructions =
            """
            Your responsiblity is to review and identify how to improve user provided content.
            If the user has providing input or direction for content already provided, specify how to address this input.
            Never directly perform the correction or provide example.
            Once the content has been updated in a subsequent response, you will review the content again until satisfactory.
            Always copy satisfactory content to the clipboard using available tools and inform user.

            RULES:
            - Only identify suggestions that are specific and actionable.
            - Verify previous suggestions have been addressed.
            - Never repeat previous suggestions.
            """,
        Kernel = toolKernel,
        Arguments =
            new KernelArguments(
                new AzureOpenAIPromptExecutionSettings() 
                { 
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() 
                })
    };
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

The _Writer_ agent is is similiar, but doesn't require the specification of _Execution Settings_ since it isn't configured with a plug-in.

Here the _Writer_ is given a single-purpose task, follow direction and rewrite the content.

::: zone pivot="programming-language-csharp"
```csharp
ChatCompletionAgent agentWriter =
    new()
    {
        Name = WriterName,
        Instructions =
            """
            Your sole responsiblity is to rewrite content according to review suggestions.

            - Always apply all review direction.
            - Always revise the content in its entirety without explanation.
            - Never address the user.
            """,
        Kernel = kernel,
    };
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

### Chat Definition

Defining the _Agent Group Chat_ requires considering the strategies for selecting the _Agent_ turn and determining when to exit the _Chat_ loop.  For both of these considerations, we will define a _Kernel Prompt Function_.

The first to reason over _Agent_ selection:

::: zone pivot="programming-language-csharp"

Using `AgentGroupChat.CreatePromptFunctionForStrategy` provides a convenient mechanism to avoid _HTML encoding_ the message paramter.

```csharp
KernelFunction selectionFunction =
    AgentGroupChat.CreatePromptFunctionForStrategy(
        $$$"""
        Examine the provided RESPONSE and choose the next participant.
        State only the name of the chosen participant without explanation.
        Never choose the participant named in the RESPONSE.
        
        Choose only from these participants:
        - {{{ReviewerName}}}
        - {{{WriterName}}}

        Always follow these rules when choosing the next participant:
        - If RESPONSE is user input, it is {{{ReviewerName}}}'s turn.
        - If RESPONSE is by {{{ReviewerName}}}, it is {{{WriterName}}}'s turn.
        - If RESPONSE is by {{{WriterName}}}, it is {{{ReviewerName}}}'s turn.

        RESPONSE:
        {{$lastmessage}}
        """,
        safeParameterNames: "lastmessage");
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

The second will evaluate when to exit the _Chat_ loop:

::: zone pivot="programming-language-csharp"
```csharp
const string TerminationToken = "yes";

KernelFunction terminationFunction =
    AgentGroupChat.CreatePromptFunctionForStrategy(
        $$$"""
        Examine the RESPONSE and determine whether the content has been deemed satisfactory.
        If content is satisfactory, respond with a single word without explanation: {{{TerminationToken}}}.
        If specific suggestions are being provided, it is not satisfactory.
        If no correction is suggested, it is satisfactory.

        RESPONSE:
        {{$lastmessage}}
        """,
        safeParameterNames: "lastmessage");
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Both of these _Strategies_ will only require knowledge of the most recent _Chat_ message.  This will reduce token usage and help improve performance:

::: zone pivot="programming-language-csharp"
```csharp
ChatHistoryTruncationReducer historyReducer = new(1);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Finally we are ready to bring everything together in our _Agent Group Chat_ definition.

::: zone pivot="programming-language-csharp"

Creating `AgentGroupChat` involves:

1. Include both agents in the constructor.
2. Define a `KernelFunctionSelectionStrategy` using the previously defined `KernelFunction` and `Kernel` instance.
3. Define a `KernelFunctionTerminationStrategy` using the previously defined `KernelFunction` and `Kernel` instance.

Notice that each strategy is responsible for parsing the `KernelFunction` result.

```csharp
AgentGroupChat chat =
    new(agentReviewer, agentWriter)
    {
        ExecutionSettings = new AgentGroupChatSettings
        {
            SelectionStrategy =
                new KernelFunctionSelectionStrategy(selectionFunction, kernel)
                {
                    // Always start with the editor agent.
                    InitialAgent = agentReviewer,
                    // Save tokens by only including the final response
                    HistoryReducer = historyReducer,
                    // The prompt variable name for the history argument.
                    HistoryVariableName = "lastmessage",
                    // Returns the entire result value as a string.
                    ResultParser = (result) => result.GetValue<string>() ?? agentReviewer.Name
                },
            TerminationStrategy =
                new KernelFunctionTerminationStrategy(terminationFunction, kernel)
                {
                    // Only evaluate for editor's response
                    Agents = [agentReviewer],
                    // Save tokens by only including the final response
                    HistoryReducer = historyReducer,
                    // The prompt variable name for the history argument.
                    HistoryVariableName = "lastmessage",
                    // Limit total number of turns
                    MaximumIterations = 12,
                    // Customer result parser to determine if the response is "yes"
                    ResultParser = (result) => result.GetValue<string>()?.Contains(TerminationToken, StringComparison.OrdinalIgnoreCase) ?? false
                }
        }
    };

Console.WriteLine("Ready!");
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

### The _Chat_ Loop

At last, we are able to coordinate the interaction between the user and the _Agent Group Chat_.  Start by creating creating an empty loop.

> Note: Unlike the other examples, no external history or _thread_ is managed.  _Agent Group Chat_ manages the conversation history internally.

::: zone pivot="programming-language-csharp"
```csharp
bool isComplete = false;
do
{

} while (!isComplete);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Now let's capture user input within the previous loop.  In this case:
- Empty input will be ignored 
- The term `EXIT` will signal that the conversation is completed
- The term `RESET` will clear the _Agent Group Chat_ history
- Any term starting with `@` will be treated as a file-path whose content will be provided as input
- Valid input will be added to the _Agent Group Chaty_ as a _User_ message.


::: zone pivot="programming-language-csharp"
```csharp
Console.WriteLine();
Console.Write("> ");
string input = Console.ReadLine();
if (string.IsNullOrWhiteSpace(input))
{
    continue;
}
input = input.Trim();
if (input.Equals("EXIT", StringComparison.OrdinalIgnoreCase))
{
    isComplete = true;
    break;
}

if (input.Equals("RESET", StringComparison.OrdinalIgnoreCase))
{
    await chat.ResetAsync();
    Console.WriteLine("[Converation has been reset]");
    continue;
}

if (input.StartsWith("@", StringComparison.Ordinal) && input.Length > 1)
{
    string filePath = input.Substring(1);
    try
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Unable to access file: {filePath}");
            continue;
        }
        input = File.ReadAllText(filePath);
    }
    catch (Exception)
    {
        Console.WriteLine($"Unable to access file: {filePath}");
        continue;
    }
}

chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, input));
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

To initate the _Agent_ collaboration in response to user input and display the _Agent_ responses, invoke the _Agent Group Chat_; however, first be sure to reset the _Completion_ state from any prior invocation.

> Note: Service failures are being caught and displayed to avoid crashing the conversation loop.

::: zone pivot="programming-language-csharp"
```csharp
chat.IsComplete = false;

try
{
    await foreach (ChatMessageContent response in chat.InvokeAsync())
    {
        Console.WriteLine();
        Console.WriteLine($"{response.AuthorName.ToUpperInvariant()}:{Environment.NewLine}{response.Content}");
    }
}
catch (HttpOperationException exception)
{
    Console.WriteLine(exception.Message);
    if (exception.InnerException != null)
    {
        Console.WriteLine(exception.InnerException.Message);
        if (exception.InnerException.Data.Count > 0)
        {
            Console.WriteLine(JsonSerializer.Serialize(exception.InnerException.Data, new JsonSerializerOptions() { WriteIndented = true }));
        }
    }
}
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
// Copyright (c) Microsoft. All rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Agents.History;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace AgentsSample;

public static class Program
{
    public static async Task Main()
    {
        // Load configuration from environment variables or user secrets.
        Settings settings = new();

        Console.WriteLine("Creating kernel...");
        IKernelBuilder builder = Kernel.CreateBuilder();

        builder.AddAzureOpenAIChatCompletion(
            settings.AzureOpenAI.ChatModelDeployment,
            settings.AzureOpenAI.Endpoint,
            new AzureCliCredential());

        Kernel kernel = builder.Build();

        Kernel toolKernel = kernel.Clone();
        toolKernel.Plugins.AddFromType<ClipboardAccess>();


        Console.WriteLine("Defining agents...");

        const string ReviewerName = "Reviewer";
        const string WriterName = "Writer";

        ChatCompletionAgent agentReviewer =
            new()
            {
                Name = ReviewerName,
                Instructions =
                    """
                    Your responsiblity is to review and identify how to improve user provided content.
                    If the user has providing input or direction for content already provided, specify how to address this input.
                    Never directly perform the correction or provide example.
                    Once the content has been updated in a subsequent response, you will review the content again until satisfactory.
                    Always copy satisfactory content to the clipboard using available tools and inform user.

                    RULES:
                    - Only identify suggestions that are specific and actionable.
                    - Verify previous suggestions have been addressed.
                    - Never repeat previous suggestions.
                    """,
                Kernel = toolKernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
            };

        ChatCompletionAgent agentWriter =
            new()
            {
                Name = WriterName,
                Instructions =
                    """
                    Your sole responsiblity is to rewrite content according to review suggestions.

                    - Always apply all review direction.
                    - Always revise the content in its entirety without explanation.
                    - Never address the user.
                    """,
                Kernel = kernel,
            };

        KernelFunction selectionFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                Examine the provided RESPONSE and choose the next participant.
                State only the name of the chosen participant without explanation.
                Never choose the participant named in the RESPONSE.
                
                Choose only from these participants:
                - {{{ReviewerName}}}
                - {{{WriterName}}}

                Always follow these rules when choosing the next participant:
                - If RESPONSE is user input, it is {{{ReviewerName}}}'s turn.
                - If RESPONSE is by {{{ReviewerName}}}, it is {{{WriterName}}}'s turn.
                - If RESPONSE is by {{{WriterName}}}, it is {{{ReviewerName}}}'s turn.

                RESPONSE:
                {{$lastmessage}}
                """,
                safeParameterNames: "lastmessage");

        const string TerminationToken = "yes";

        KernelFunction terminationFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                Examine the RESPONSE and determine whether the content has been deemed satisfactory.
                If content is satisfactory, respond with a single word without explanation: {{{TerminationToken}}}.
                If specific suggestions are being provided, it is not satisfactory.
                If no correction is suggested, it is satisfactory.

                RESPONSE:
                {{$lastmessage}}
                """,
                safeParameterNames: "lastmessage");

        ChatHistoryTruncationReducer historyReducer = new(1);

        AgentGroupChat chat =
            new(agentReviewer, agentWriter)
            {
                ExecutionSettings = new AgentGroupChatSettings
                {
                    SelectionStrategy =
                        new KernelFunctionSelectionStrategy(selectionFunction, kernel)
                        {
                            // Always start with the editor agent.
                            InitialAgent = agentReviewer,
                            // Save tokens by only including the final response
                            HistoryReducer = historyReducer,
                            // The prompt variable name for the history argument.
                            HistoryVariableName = "lastmessage",
                            // Returns the entire result value as a string.
                            ResultParser = (result) => result.GetValue<string>() ?? agentReviewer.Name
                        },
                    TerminationStrategy =
                        new KernelFunctionTerminationStrategy(terminationFunction, kernel)
                        {
                            // Only evaluate for editor's response
                            Agents = [agentReviewer],
                            // Save tokens by only including the final response
                            HistoryReducer = historyReducer,
                            // The prompt variable name for the history argument.
                            HistoryVariableName = "lastmessage",
                            // Limit total number of turns
                            MaximumIterations = 12,
                            // Customer result parser to determine if the response is "yes"
                            ResultParser = (result) => result.GetValue<string>()?.Contains(TerminationToken, StringComparison.OrdinalIgnoreCase) ?? false
                        }
                }
            };

        Console.WriteLine("Ready!");

        bool isComplete = false;
        do
        {
            Console.WriteLine();
            Console.Write("> ");
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            input = input.Trim();
            if (input.Equals("EXIT", StringComparison.OrdinalIgnoreCase))
            {
                isComplete = true;
                break;
            }

            if (input.Equals("RESET", StringComparison.OrdinalIgnoreCase))
            {
                await chat.ResetAsync();
                Console.WriteLine("[Converation has been reset]");
                continue;
            }

            if (input.StartsWith("@", StringComparison.Ordinal) && input.Length > 1)
            {
                string filePath = input.Substring(1);
                try
                {
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($"Unable to access file: {filePath}");
                        continue;
                    }
                    input = File.ReadAllText(filePath);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Unable to access file: {filePath}");
                    continue;
                }
            }

            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, input));

            chat.IsComplete = false;

            try
            {
                await foreach (ChatMessageContent response in chat.InvokeAsync())
                {
                    Console.WriteLine();
                    Console.WriteLine($"{response.AuthorName.ToUpperInvariant()}:{Environment.NewLine}{response.Content}");
                }
            }
            catch (HttpOperationException exception)
            {
                Console.WriteLine(exception.Message);
                if (exception.InnerException != null)
                {
                    Console.WriteLine(exception.InnerException.Message);
                    if (exception.InnerException.Data.Count > 0)
                    {
                        Console.WriteLine(JsonSerializer.Serialize(exception.InnerException.Data, new JsonSerializerOptions() { WriteIndented = true }));
                    }
                }
            }
        } while (!isComplete);
    }

    private sealed class ClipboardAccess
    {
        [KernelFunction]
        [Description("Copies the provided content to the clipboard.")]
        public static void SetClipboard(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            using Process clipProcess = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "clip",
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                });

            clipProcess.StandardInput.Write(content);
            clipProcess.StandardInput.Close();
        }
    }
}

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