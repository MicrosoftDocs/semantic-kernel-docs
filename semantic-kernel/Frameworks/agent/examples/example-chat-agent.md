---
title: How-To&colon; _Chat Completion Agent_ (Experimental)
description: A step-by-step walk-through of defining and utilizing the features of a Chat Completion Agent.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# How-To: _Chat Completion Agent_ (Experimental)

> [!WARNING]
> The _Semantic Kernel Agent Framework_ is experimental, still in development and is subject to change.

## Overview

In this sample, we will explore configuring a plugin to access _GitHub_ API and provide templatized instructions to a [_Chat Completion Agent_](../chat-completion-agent.md) to answer questions about a _GitHub_ repository.  The approach will be broken down step-by-step to high-light the key parts of the coding process.  As part of the task, the agent will provide document citations within the response.

Streaming will be used to deliver the agent's responses. This will provide real-time updates as the task progresses.

## Getting Started

Before proceeding with feature coding, make sure your development environment is fully set up and configured.

::: zone pivot="programming-language-csharp"

Start by creating a _Console_ project. Then, include the following package references to ensure all required dependencies are available.

To add package dependencies from the command-line use the `dotnet` command:

```powershell
dotnet add package Azure.Identity
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Binder
dotnet add package Microsoft.Extensions.Configuration.UserSecrets
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
dotnet add package Microsoft.SemanticKernel.Connectors.AzureOpenAI
dotnet add package Microsoft.SemanticKernel.Agents.Core --prerelease
```

> If managing _NuGet_ packages in _Visual Studio_, ensure `Include prerelease` is checked.

The project file (`.csproj`) should contain the following `PackageReference` definitions:

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

The _Agent Framework_ is experimental and requires warning suppression.  This may addressed in as a property in the project file (`.csproj`):

```xml
  <PropertyGroup>
    <NoWarn>$(NoWarn);CA2007;IDE1006;SKEXP0001;SKEXP0110;OPENAI001</NoWarn>
  </PropertyGroup>
```

Additionally, copy the GitHub plug-in and models (`GitHubPlugin.cs` and `GitHubModels.cs`) from [_Semantic Kernel_ `LearnResources` Project](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/LearnResources/Plugins/GitHub).  Add these files in your project folder.

::: zone-end

::: zone pivot="programming-language-python"
Start by creating a folder that will hold your script (`.py` file) and the sample resources. Include the following imports at the top of your `.py` file:
```python
import asyncio
import os
import sys
from datetime import datetime

from semantic_kernel.agents import ChatCompletionAgent
from semantic_kernel.connectors.ai.function_choice_behavior import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.contents.chat_history import ChatHistory
from semantic_kernel.contents.chat_message_content import ChatMessageContent
from semantic_kernel.contents.utils.author_role import AuthorRole
from semantic_kernel.kernel import Kernel

# Adjust the sys.path so we can use the GitHubPlugin and GitHubSettings classes
# This is so we can run the code from the samples/learn_resources/agent_docs directory
# If you are running code from your own project, you may not need need to do this.
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), "..")))

from plugins.GithubPlugin.github import GitHubPlugin, GitHubSettings  # noqa: E402
```

Additionally, copy the GitHub plug-in and models (`github.py`) from [_Semantic Kernel_ `LearnResources` Project](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/learn_resources/plugins/GithubPlugin).  Add these files in your project folder.
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Configuration

This sample requires configuration setting in order to connect to remote services.  You will need to define settings for either _Open AI_ or _Azure Open AI_ and also for _GitHub_.

> Note: For information on GitHub _Personal Access Tokens_, see: [Managing your personal access tokens](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens).

::: zone pivot="programming-language-csharp"

```powershell
# Open AI
dotnet user-secrets set "OpenAISettings:ApiKey" "<api-key>"
dotnet user-secrets set "OpenAISettings:ChatModel" "gpt-4o"

# Azure Open AI
dotnet user-secrets set "AzureOpenAISettings:ApiKey" "<api-key>" # Not required if using token-credential
dotnet user-secrets set "AzureOpenAISettings:Endpoint" "<model-endpoint>"
dotnet user-secrets set "AzureOpenAISettings:ChatModelDeployment" "gpt-4o"

# GitHub
dotnet user-secrets set "GitHubSettings:BaseUrl" "https://api.github.com"
dotnet user-secrets set "GitHubSettings:Token" "<personal access token>"
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
AZURE_OPENAI_API_VERSION="..."

OPENAI_API_KEY="sk-..."
OPENAI_ORG_ID=""
OPENAI_CHAT_MODEL_ID=""
```

Once configured, the respective AI service classes will pick up the required variables and use them during instantiation.
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Coding

The coding process for this sample involves:

1. [Setup](#setup) - Initializing settings and the plug-in.
2. [_Agent_ Definition](#agent-definition) - Create the _Chat Completion Agent_ with templatized instructions and plug-in.
3. [The _Chat_ Loop](#the-chat-loop) - Write the loop that drives user / agent interaction.

The full example code is provided in the [Final](#final) section. Refer to that section for the complete implementation.

### Setup

Prior to creating a _Chat Completion Agent_, the configuration settings, plugins, and _Kernel_ must be initialized.

::: zone pivot="programming-language-csharp"

Initialize the `Settings` class referenced in the previous [Configuration](#configuration) section.

```csharp
Settings settings = new();
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

Initialize the plug-in using its settings.

::: zone pivot="programming-language-csharp"

Here, a message is displaying to indicate progress.

```csharp
Console.WriteLine("Initialize plugins...");
GitHubSettings githubSettings = settings.GetSettings<GitHubSettings>();
GitHubPlugin githubPlugin = new(githubSettings);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
gh_settings = GitHubSettings(
    token="<PAT value>"
)
kernel.add_plugin(GitHubPlugin(settings=gh_settings), plugin_name="github")
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

Now initialize a `Kernel` instance with an `IChatCompletionService` and the `GitHubPlugin` previously created.

::: zone pivot="programming-language-csharp"
```csharp
Console.WriteLine("Creating kernel...");
IKernelBuilder builder = Kernel.CreateBuilder();

builder.AddAzureOpenAIChatCompletion(
    settings.AzureOpenAI.ChatModelDeployment,
    settings.AzureOpenAI.Endpoint,
    new AzureCliCredential());

builder.Plugins.AddFromObject(githubPlugin);

Kernel kernel = builder.Build();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
kernel = Kernel()

# Add the AzureChatCompletion AI Service to the Kernel
service_id = "agent"
kernel.add_service(AzureChatCompletion(service_id=service_id))

settings = kernel.get_prompt_execution_settings_from_service_id(service_id=service_id)
# Configure the function choice behavior to auto invoke kernel functions
settings.function_choice_behavior = FunctionChoiceBehavior.Auto()
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

### Agent Definition

Finally we are ready to instantiate a _Chat Completion Agent_ with its _Instructions_, associated _Kernel_, and the default _Arguments_ and _Execution Settings_.  In this case, we desire to have the any plugin functions automatically executed.

::: zone pivot="programming-language-csharp"
```csharp
Console.WriteLine("Defining agent...");
ChatCompletionAgent agent =
    new()
    {
        Name = "SampleAssistantAgent",
        Instructions =
            """
            You are an agent designed to query and retrieve information from a single GitHub repository in a read-only manner.
            You are also able to access the profile of the active user.

            Use the current date and time to provide up-to-date details or time-sensitive responses.
            
            The repository you are querying is a public repository with the following name: {{$repository}}

            The current date and time is: {{$now}}. 
            """,
        Kernel = kernel,
        Arguments =
            new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
            {
                { "repository", "microsoft/semantic-kernel" }
            }
    };

Console.WriteLine("Ready!");
```
::: zone-end

::: zone pivot="programming-language-python"
```python
agent = ChatCompletionAgent(
    service_id="agent",
    kernel=kernel,
    name="SampleAssistantAgent",
    instructions=f"""
        You are an agent designed to query and retrieve information from a single GitHub repository in a read-only 
        manner.
        You are also able to access the profile of the active user.

        Use the current date and time to provide up-to-date details or time-sensitive responses.
        
        The repository you are querying is a public repository with the following name: microsoft/semantic-kernel

        The current date and time is: {current_time}. 
        """,
    execution_settings=settings,
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

### The _Chat_ Loop

At last, we are able to coordinate the interaction between the user and the _Agent_.  Start by creating a _Chat History_ object to maintain the conversation state and creating an empty loop.

::: zone pivot="programming-language-csharp"
```csharp
ChatHistory history = [];
bool isComplete = false;
do
{
    // processing logic here
} while (!isComplete);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
history = ChatHistory()
is_complete: bool = False
while not is_complete:
    # processing logic here
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

Now let's capture user input within the previous loop.  In this case, empty input will be ignored and the term `EXIT` will signal that the conversation is completed.  Valid input will be added to the _Chat History_ as a _User_ message.

::: zone pivot="programming-language-csharp"
```csharp
Console.WriteLine();
Console.Write("> ");
string input = Console.ReadLine();
if (string.IsNullOrWhiteSpace(input))
{
    continue;
}
if (input.Trim().Equals("EXIT", StringComparison.OrdinalIgnoreCase))
{
    isComplete = true;
    break;
}

history.Add(new ChatMessageContent(AuthorRole.User, input));

Console.WriteLine();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
user_input = input("User:> ")
if not user_input:
    continue

if user_input.lower() == "exit":
    is_complete = True
    break

history.add_message(ChatMessageContent(role=AuthorRole.USER, content=user_input))
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

To generate a _Agent_ response to user input, invoke the agent using _Arguments_ to provide the final template parameter that specifies the current date and time.

The _Agent_ response is then then displayed to the user.

::: zone pivot="programming-language-csharp"
```csharp
DateTime now = DateTime.Now;
KernelArguments arguments =
    new()
    {
        { "now", $"{now.ToShortDateString()} {now.ToShortTimeString()}" }
    };
await foreach (ChatMessageContent response in agent.InvokeAsync(history, arguments))
{
    Console.WriteLine($"{response.Content}");
}
```
::: zone-end

::: zone pivot="programming-language-python"
**Coming soon**
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Final

Bringing all the steps together, we have the final code for this example. The complete implementation is provided below.

::: zone pivot="programming-language-csharp"
```csharp
using System;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Plugins;

namespace AgentsSample;

public static class Program
{
    public static async Task Main()
    {
        // Load configuration from environment variables or user secrets.
        Settings settings = new();

        Console.WriteLine("Initialize plugins...");
        GitHubSettings githubSettings = settings.GetSettings<GitHubSettings>();
        GitHubPlugin githubPlugin = new(githubSettings);

        Console.WriteLine("Creating kernel...");
        IKernelBuilder builder = Kernel.CreateBuilder();

        builder.AddAzureOpenAIChatCompletion(
            settings.AzureOpenAI.ChatModelDeployment,
            settings.AzureOpenAI.Endpoint,
            new AzureCliCredential());

        builder.Plugins.AddFromObject(githubPlugin);

        Kernel kernel = builder.Build();

        Console.WriteLine("Defining agent...");
        ChatCompletionAgent agent =
            new()
            {
                Name = "SampleAssistantAgent",
                Instructions =
                        """
                        You are an agent designed to query and retrieve information from a single GitHub repository in a read-only manner.
                        You are also able to access the profile of the active user.

                        Use the current date and time to provide up-to-date details or time-sensitive responses.
                        
                        The repository you are querying is a public repository with the following name: {{$repository}}

                        The current date and time is: {{$now}}. 
                        """,
                Kernel = kernel,
                Arguments =
                    new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
                    {
                        { "repository", "microsoft/semantic-kernel" }
                    }
            };

        Console.WriteLine("Ready!");

        ChatHistory history = [];
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
            if (input.Trim().Equals("EXIT", StringComparison.OrdinalIgnoreCase))
            {
                isComplete = true;
                break;
            }

            history.Add(new ChatMessageContent(AuthorRole.User, input));

            Console.WriteLine();

            DateTime now = DateTime.Now;
            KernelArguments arguments =
                new()
                {
                    { "now", $"{now.ToShortDateString()} {now.ToShortTimeString()}" }
                };
            await foreach (ChatMessageContent response in agent.InvokeAsync(history, arguments))
            {
                // Display response.
                Console.WriteLine($"{response.Content}");
            }

        } while (!isComplete);
    }
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
import asyncio
import os
import sys
from datetime import datetime

from semantic_kernel.agents import ChatCompletionAgent
from semantic_kernel.connectors.ai.function_choice_behavior import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.contents.chat_history import ChatHistory
from semantic_kernel.contents.chat_message_content import ChatMessageContent
from semantic_kernel.contents.utils.author_role import AuthorRole
from semantic_kernel.kernel import Kernel

# Adjust the sys.path so we can use the GitHubPlugin and GitHubSettings classes
# This is so we can run the code from the samples/learn_resources/agent_docs directory
# If you are running code from your own project, you may not need need to do this.
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), "..")))

from plugins.GithubPlugin.github import GitHubPlugin, GitHubSettings  # noqa: E402

###################################################################
# The following sample demonstrates how to create a simple,       #
# ChatCompletionAgent to use a GitHub plugin to interact          #
# with the GitHub API.                                            #
###################################################################


async def main():
    kernel = Kernel()

    # Add the AzureChatCompletion AI Service to the Kernel
    service_id = "agent"
    kernel.add_service(AzureChatCompletion(service_id=service_id))

    settings = kernel.get_prompt_execution_settings_from_service_id(service_id=service_id)
    # Configure the function choice behavior to auto invoke kernel functions
    settings.function_choice_behavior = FunctionChoiceBehavior.Auto()

    # Set your GitHub Personal Access Token (PAT) value here
    gh_settings = GitHubSettings(token="<PAT value>")
    kernel.add_plugin(plugin=GitHubPlugin(gh_settings), plugin_name="GithubPlugin")

    current_time = datetime.now().isoformat()

    # Create the agent
    agent = ChatCompletionAgent(
        service_id="agent",
        kernel=kernel,
        name="SampleAssistantAgent",
        instructions=f"""
            You are an agent designed to query and retrieve information from a single GitHub repository in a read-only 
            manner.
            You are also able to access the profile of the active user.

            Use the current date and time to provide up-to-date details or time-sensitive responses.
            
            The repository you are querying is a public repository with the following name: microsoft/semantic-kernel

            The current date and time is: {current_time}. 
            """,
        execution_settings=settings,
    )

    history = ChatHistory()
    is_complete: bool = False
    while not is_complete:
        user_input = input("User:> ")
        if not user_input:
            continue

        if user_input.lower() == "exit":
            is_complete = True
            break

        history.add_message(ChatMessageContent(role=AuthorRole.USER, content=user_input))

        async for response in agent.invoke(history=history):
            print(f"{response.content}")


if __name__ == "__main__":
    asyncio.run(main())
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end



> [!div class="nextstepaction"]
> [How-To: _Open AI Assistant Agent_ Code Interpreter](./example-assistant-code.md)


