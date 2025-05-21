---
title: How-To use `ChatCompletionAgent`
description: A step-by-step walk-through of defining and utilizing the features of a Chat Completion Agent.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# How-To: `ChatCompletionAgent` 

> [!IMPORTANT]
> This feature is in the experimental stage. Features at this stage are under development and subject to change before advancing to the preview or release candidate stage.

## Overview

In this sample, we will explore configuring a plugin to access GitHub API and provide templatized instructions to a [`ChatCompletionAgent`](../chat-completion-agent.md) to answer questions about a GitHub repository.  The approach will be broken down step-by-step to high-light the key parts of the coding process.  As part of the task, the agent will provide document citations within the response.

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

> If managing NuGet packages in Visual Studio, ensure `Include prerelease` is checked.

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

The `Agent Framework` is experimental and requires warning suppression.  This may addressed in as a property in the project file (`.csproj`):

```xml
  <PropertyGroup>
    <NoWarn>$(NoWarn);CA2007;IDE1006;SKEXP0001;SKEXP0110;OPENAI001</NoWarn>
  </PropertyGroup>
```

Additionally, copy the GitHub plug-in and models (`GitHubPlugin.cs` and `GitHubModels.cs`) from [Semantic Kernel `LearnResources` Project](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/LearnResources/Plugins/GitHub).  Add these files in your project folder.

::: zone-end

::: zone pivot="programming-language-python"
Start by creating a folder that will hold your script (`.py` file) and the sample resources. Include the following imports at the top of your `.py` file:
```python
import asyncio
import os
import sys
from datetime import datetime

from semantic_kernel.agents import ChatCompletionAgent, ChatHistoryAgentThread
from semantic_kernel.connectors.ai import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.functions import KernelArguments
from semantic_kernel.kernel import Kernel

# Adjust the sys.path so we can use the GitHubPlugin and GitHubSettings classes
# This is so we can run the code from the samples/learn_resources/agent_docs directory
# If you are running code from your own project, you may not need need to do this.
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), "..")))

from plugins.GithubPlugin.github import GitHubPlugin, GitHubSettings  # noqa: E402
```

Additionally, copy the GitHub plug-in and models (`github.py`) from [Semantic Kernel `LearnResources` Project](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/learn_resources/plugins/GithubPlugin).  Add these files in your project folder.
::: zone-end

::: zone pivot="programming-language-java"

Start by creating a Maven console project. Then, include the following package references to ensure all required dependencies are available.

The project `pom.xml` should contain the following dependencies:

```xml
<dependencyManagement>
    <dependencies>
        <dependency>
            <groupId>com.microsoft.semantic-kernel</groupId>
            <artifactId>semantickernel-bom</artifactId>
            <version>[LATEST]</version>
            <type>pom</type>
            <scope>import</scope>
        </dependency>
    </dependencies>
</dependencyManagement>

<dependencies>
    <dependency>
        <groupId>com.microsoft.semantic-kernel</groupId>
        <artifactId>semantickernel-agents-core</artifactId>
    </dependency>

    <dependency>
        <groupId>com.microsoft.semantic-kernel</groupId>
        <artifactId>semantickernel-aiservices-openai</artifactId>
    </dependency>
</dependencies>
```

Additionally, copy the GitHub plug-in and models (`GitHubPlugin.java` and `GitHubModels.java`) from [Semantic Kernel `LearnResources` Project](https://github.com/microsoft/semantic-kernel-java/tree/main/samples/semantickernel-learn-resources/src/main/java/com/microsoft/semantickernel/samples/plugins/github).  Add these files in your project folder.

::: zone-end

## Configuration

This sample requires configuration setting in order to connect to remote services.  You will need to define settings for either OpenAI or Azure OpenAI and also for GitHub.

> Note: For information on GitHub Personal Access Tokens, see: [Managing your personal access tokens](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens).

::: zone pivot="programming-language-csharp"

```powershell
# OpenAI
dotnet user-secrets set "OpenAISettings:ApiKey" "<api-key>"
dotnet user-secrets set "OpenAISettings:ChatModel" "gpt-4o"

# Azure OpenAI
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
AZURE_OPENAI_ENDPOINT="https://<resource-name>.openai.azure.com/"
AZURE_OPENAI_CHAT_DEPLOYMENT_NAME="..."
AZURE_OPENAI_API_VERSION="..."

OPENAI_API_KEY="sk-..."
OPENAI_ORG_ID=""
OPENAI_CHAT_MODEL_ID=""
```

Once configured, the respective AI service classes will pick up the required variables and use them during instantiation.
::: zone-end

::: zone pivot="programming-language-java"

Define the following environment variables in your system.

```bash
# Azure OpenAI
AZURE_OPENAI_API_KEY=""
AZURE_OPENAI_ENDPOINT="https://<resource-name>.openai.azure.com/"
AZURE_CHAT_MODEL_DEPLOYMENT=""

# OpenAI
OPENAI_API_KEY=""
OPENAI_MODEL_ID=""
```

At the top of the file you can retrieve their values as following.

```java
// Azure OpenAI
private static final String AZURE_OPENAI_API_KEY = System.getenv("AZURE_OPENAI_API_KEY");
private static final String AZURE_OPENAI_ENDPOINT = System.getenv("AZURE_OPENAI_ENDPOINT");
private static final String AZURE_CHAT_MODEL_DEPLOYMENT = System.getenv().getOrDefault("AZURE_CHAT_MODEL_DEPLOYMENT", "gpt-4o");

// OpenAI
private static final String OPENAI_API_KEY = System.getenv("OPENAI_API_KEY");
private static final String OPENAI_MODEL_ID = System.getenv().getOrDefault("OPENAI_MODEL_ID", "gpt-4o");
```

::: zone-end


## Coding

The coding process for this sample involves:

1. [Setup](#setup) - Initializing settings and the plug-in.
2. [`Agent` Definition](#agent-definition) - Create the `ChatCompletionAgent` with templatized instructions and plug-in.
3. [The Chat Loop](#the-chat-loop) - Write the loop that drives user / agent interaction.

The full example code is provided in the [Final](#final) section. Refer to that section for the complete implementation.

### Setup

Prior to creating a `ChatCompletionAgent`, the configuration settings, plugins, and `Kernel` must be initialized.

::: zone pivot="programming-language-csharp"

Initialize the `Settings` class referenced in the previous [Configuration](#configuration) section.

```csharp
Settings settings = new();
```
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

```java
var githubPlugin = new GitHubPlugin(GITHUB_PAT);
```

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

```java
OpenAIAsyncClient client = new OpenAIClientBuilder()
    .credential(new AzureKeyCredential(AZURE_OPENAI_API_KEY))
    .endpoint(AZURE_OPENAI_ENDPOINT)
    .buildAsyncClient();

ChatCompletionService chatCompletion = OpenAIChatCompletion.builder()
    .withModelId(AZURE_CHAT_MODEL_DEPLOYMENT)
    .withOpenAIAsyncClient(client)
    .build();

Kernel kernel = Kernel.builder()
    .withAIService(ChatCompletionService.class, chatCompletion)
    .withPlugin(KernelPluginFactory.createFromObject(githubPlugin, "GitHubPlugin"))
    .build();
```

::: zone-end

### Agent Definition

Finally we are ready to instantiate a `ChatCompletionAgent` with its Instructions, associated `Kernel`, and the default Arguments and Execution Settings.  In this case, we desire to have the any plugin functions automatically executed.

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
    kernel=kernel,
    name="SampleAssistantAgent",
    instructions=f"""
        You are an agent designed to query and retrieve information from a single GitHub repository in a read-only 
        manner.
        You are also able to access the profile of the active user.

        Use the current date and time to provide up-to-date details or time-sensitive responses.
        
        The repository you are querying is a public repository with the following name: microsoft/semantic-kernel

        The current date and time is: {{$now}}. 
        """,
    arguments=KernelArguments(
        settings=settings,
    ),
)
```
::: zone-end

::: zone pivot="programming-language-java"

```java
// Invocation context for the agent
InvocationContext invocationContext = InvocationContext.builder()
    .withFunctionChoiceBehavior(FunctionChoiceBehavior.auto(true))
    .build()

ChatCompletionAgent agent = ChatCompletionAgent.builder()
    .withName("SampleAssistantAgent")
    .withKernel(kernel)
    .withInvocationContext(invocationContext)
    .withTemplate(
        DefaultPromptTemplate.build(
            PromptTemplateConfig.builder()
                .withTemplate(
                    """
                    You are an agent designed to query and retrieve information from a single GitHub repository in a read-only manner.
                    You are also able to access the profile of the active user.

                    Use the current date and time to provide up-to-date details or time-sensitive responses.

                    The repository you are querying is a public repository with the following name: {{$repository}}

                    The current date and time is: {{$now}}.
                    """)
                .build()))
    .withKernelArguments(
        KernelArguments.builder()
            .withVariable("repository", "microsoft/semantic-kernel-java")
            .withExecutionSettings(PromptExecutionSettings.builder()
                    .build())
            .build())
    .build();
```


::: zone-end

### The Chat Loop

At last, we are able to coordinate the interaction between the user and the `Agent`.  Start by creating a `ChatHistoryAgentThread` object to maintain the conversation state and creating an empty loop.

::: zone pivot="programming-language-csharp"
```csharp
ChatHistoryAgentThread agentThread = new();
bool isComplete = false;
do
{
    // processing logic here
} while (!isComplete);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
thread: ChatHistoryAgentThread = None
is_complete: bool = False
while not is_complete:
    # processing logic here
```
::: zone-end

::: zone pivot="programming-language-java"

```java
AgentThread agentThread = new ChatHistoryAgentThread();
boolean isComplete = false;

while (!isComplete) {
    // processing logic here
}
```


::: zone-end

Now let's capture user input within the previous loop.  In this case, empty input will be ignored and the term `EXIT` will signal that the conversation is completed.

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

var message = new ChatMessageContent(AuthorRole.User, input);

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
```
::: zone-end

::: zone pivot="programming-language-java"

```java
Scanner scanner = new Scanner(System.in);

while (!isComplete) {
    System.out.print("> ");

    String input = scanner.nextLine();
    if (input.isEmpty()) {
        continue;
    }

    if (input.equalsIgnoreCase("exit")) {
        isComplete = true;
        break;
    }

}
```

::: zone-end

To generate a `Agent` response to user input, invoke the agent using _Arguments_ to provide the final template parameter that specifies the current date and time.

The `Agent` response is then then displayed to the user.

::: zone pivot="programming-language-csharp"
```csharp
DateTime now = DateTime.Now;
KernelArguments arguments =
    new()
    {
        { "now", $"{now.ToShortDateString()} {now.ToShortTimeString()}" }
    };
await foreach (ChatMessageContent response in agent.InvokeAsync(message, agentThread, options: new() { KernelArguments = arguments }))
{
    Console.WriteLine($"{response.Content}");
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
arguments = KernelArguments(
    now=datetime.now().strftime("%Y-%m-%d %H:%M")
)

async for response in agent.invoke(messages=user_input, thread=thread, arguments=arguments):
    print(f"{response.content}")
    thread = response.thread
```
::: zone-end

::: zone pivot="programming-language-java"

```java
var options = AgentInvokeOptions.builder()
    .withKernelArguments(KernelArguments.builder()
            .withVariable("now", OffsetDateTime.now())
            .build())
    .build();

for (var response : agent.invokeAsync(message, agentThread, options).block()) {
    System.out.println(response.getMessage());
    agentThread = response.getThread();
}
```


::: zone-end

## Final

Bringing all the steps together, we have the final code for this example. The complete implementation is provided below.

Try using these suggested inputs:

1. What is my username?
2. Describe the repo.
3. Describe the newest issue created in the repo.
4. List the top 10 issues closed within the last week.
5. How were these issues labeled?
6. List the 5 most recently opened issues with the "Agents" label

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

        ChatHistoryAgentThread agentThread = new();
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

            var message = new ChatMessageContent(AuthorRole.User, input);

            Console.WriteLine();

            DateTime now = DateTime.Now;
            KernelArguments arguments =
                new()
                {
                    { "now", $"{now.ToShortDateString()} {now.ToShortTimeString()}" }
                };
            await foreach (ChatMessageContent response in agent.InvokeAsync(message, agentThread, options: new() { KernelArguments = arguments }))
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

from semantic_kernel.agents import ChatCompletionAgent, ChatHistoryAgentThread
from semantic_kernel.connectors.ai import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.functions import KernelArguments
from semantic_kernel.kernel import Kernel

# Adjust the sys.path so we can use the GitHubPlugin and GitHubSettings classes
# This is so we can run the code from the samples/learn_resources/agent_docs directory
# If you are running code from your own project, you may not need need to do this.
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), "..")))

from plugins.GithubPlugin.github import GitHubPlugin, GitHubSettings  # noqa: E402

async def main():
    kernel = Kernel()

    # Add the AzureChatCompletion AI Service to the Kernel
    service_id = "agent"
    kernel.add_service(AzureChatCompletion(service_id=service_id))

    settings = kernel.get_prompt_execution_settings_from_service_id(service_id=service_id)
    # Configure the function choice behavior to auto invoke kernel functions
    settings.function_choice_behavior = FunctionChoiceBehavior.Auto()

    # Set your GitHub Personal Access Token (PAT) value here
    gh_settings = GitHubSettings(token="")  # nosec
    kernel.add_plugin(plugin=GitHubPlugin(gh_settings), plugin_name="GithubPlugin")

    current_time = datetime.now().isoformat()

    # Create the agent
    agent = ChatCompletionAgent(
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
        arguments=KernelArguments(settings=settings),
    )

    thread: ChatHistoryAgentThread = None
    is_complete: bool = False
    while not is_complete:
        user_input = input("User:> ")
        if not user_input:
            continue

        if user_input.lower() == "exit":
            is_complete = True
            break

        arguments = KernelArguments(now=datetime.now().strftime("%Y-%m-%d %H:%M"))

        async for response in agent.invoke(messages=user_input, thread=thread, arguments=arguments):
            print(f"{response.content}")
            thread = response.thread


if __name__ == "__main__":
    asyncio.run(main())
```

You may find the full [code](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/learn_resources/agent_docs/chat_agent.py), as shown above, in our repo.
::: zone-end

::: zone pivot="programming-language-java"

```java
import com.microsoft.semantickernel.Kernel;
import com.microsoft.semantickernel.agents.AgentInvokeOptions;
import com.microsoft.semantickernel.agents.AgentThread;
import com.microsoft.semantickernel.agents.chatcompletion.ChatCompletionAgent;
import com.microsoft.semantickernel.agents.chatcompletion.ChatHistoryAgentThread;
import com.microsoft.semantickernel.aiservices.openai.chatcompletion.OpenAIChatCompletion;
import com.microsoft.semantickernel.contextvariables.ContextVariableTypeConverter;
import com.microsoft.semantickernel.functionchoice.FunctionChoiceBehavior;
import com.microsoft.semantickernel.implementation.templateengine.tokenizer.DefaultPromptTemplate;
import com.microsoft.semantickernel.orchestration.InvocationContext;
import com.microsoft.semantickernel.orchestration.PromptExecutionSettings;
import com.microsoft.semantickernel.plugin.KernelPluginFactory;
import com.microsoft.semantickernel.samples.plugins.github.GitHubModel;
import com.microsoft.semantickernel.samples.plugins.github.GitHubPlugin;
import com.microsoft.semantickernel.semanticfunctions.KernelArguments;
import com.microsoft.semantickernel.semanticfunctions.PromptTemplateConfig;
import com.microsoft.semantickernel.services.chatcompletion.AuthorRole;
import com.microsoft.semantickernel.services.chatcompletion.ChatCompletionService;
import com.microsoft.semantickernel.services.chatcompletion.ChatMessageContent;
import com.azure.ai.openai.OpenAIAsyncClient;
import com.azure.ai.openai.OpenAIClientBuilder;
import com.azure.core.credential.AzureKeyCredential;

import java.time.OffsetDateTime;
import java.util.Scanner;

public class CompletionAgent {
    // Azure OpenAI
    private static final String AZURE_OPENAI_API_KEY = System.getenv("AZURE_OPENAI_API_KEY");
    private static final String AZURE_OPENAI_ENDPOINT = System.getenv("AZURE_OPENAI_ENDPOINT");
    private static final String AZURE_CHAT_MODEL_DEPLOYMENT = System.getenv().getOrDefault("AZURE_CHAT_MODEL_DEPLOYMENT", "gpt-4o");

    // GitHub Personal Access Token
    private static final String GITHUB_PAT = System.getenv("GITHUB_PAT");

    public static void main(String[] args) {
        System.out.println("======== ChatCompletion Agent ========");

        OpenAIAsyncClient client = new OpenAIClientBuilder()
                .credential(new AzureKeyCredential(AZURE_OPENAI_API_KEY))
                .endpoint(AZURE_OPENAI_ENDPOINT)
                .buildAsyncClient();

        var githubPlugin = new GitHubPlugin(GITHUB_PAT);

        ChatCompletionService chatCompletion = OpenAIChatCompletion.builder()
                .withModelId(AZURE_CHAT_MODEL_DEPLOYMENT)
                .withOpenAIAsyncClient(client)
                .build();

        Kernel kernel = Kernel.builder()
            .withAIService(ChatCompletionService.class, chatCompletion)
            .withPlugin(KernelPluginFactory.createFromObject(githubPlugin, "GitHubPlugin"))
            .build();

        InvocationContext invocationContext = InvocationContext.builder()
            .withFunctionChoiceBehavior(FunctionChoiceBehavior.auto(true))
            .withContextVariableConverter(new ContextVariableTypeConverter<>(
                    GitHubModel.Issue.class,
                    o -> (GitHubModel.Issue) o,
                    o -> o.toString(),
                    s -> null))
            .build();

        ChatCompletionAgent agent = ChatCompletionAgent.builder()
            .withName("SampleAssistantAgent")
            .withKernel(kernel)
            .withInvocationContext(invocationContext)
            .withTemplate(
                DefaultPromptTemplate.build(
                    PromptTemplateConfig.builder()
                        .withTemplate(
                            """
                            You are an agent designed to query and retrieve information from a single GitHub repository in a read-only manner.
                            You are also able to access the profile of the active user.
        
                            Use the current date and time to provide up-to-date details or time-sensitive responses.
        
                            The repository you are querying is a public repository with the following name: {{$repository}}
        
                            The current date and time is: {{$now}}.
                            """)
                        .build()))
            .withKernelArguments(
                KernelArguments.builder()
                    .withVariable("repository", "microsoft/semantic-kernel-java")
                    .withExecutionSettings(PromptExecutionSettings.builder()
                            .build())
                    .build())
            .build();

        AgentThread agentThread = new ChatHistoryAgentThread();
        boolean isComplete = false;

        Scanner scanner = new Scanner(System.in);

        while (!isComplete) {
            System.out.print("> ");

            String input = scanner.nextLine();
            if (input.isEmpty()) {
                continue;
            }

            if (input.equalsIgnoreCase("EXIT")) {
                isComplete = true;
                break;
            }

            var message = new ChatMessageContent<>(AuthorRole.USER, input);

            var options = AgentInvokeOptions.builder()
                .withKernelArguments(KernelArguments.builder()
                        .withVariable("now", OffsetDateTime.now())
                        .build())
                .build();

            for (var response : agent.invokeAsync(message, agentThread, options).block()) {
                System.out.println(response.getMessage());
                agentThread = response.getThread();
            }
        }
    }
}
```

::: zone-end



> [!div class="nextstepaction"]
> [How-To: `OpenAIAssistantAgent` Code Interpreter](./example-assistant-code.md)


