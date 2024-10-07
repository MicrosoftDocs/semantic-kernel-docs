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

Additionally, copy the GitHub plug-in and models (`GitHubPlugin.cs` and `GitHubModels.cs`) from [_Semantic Kernel_ `LearnResources` Project](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/LearnResources/Plugins/GitHub).  Add these files in your project folder.

::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
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
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## Coding

The coding process for this sample involves:

1. [Setup](#setup) - Initializing settings and the plug-in.
2. [Agent Definition](#agent-definition) - Create the _Chat Completion Agent_ with templatized instructions and plug-in.
3. [The _Chat_ Loop](#the-chat-loop) - Write the loop that drives user / agent interaction.

The full example code is provided in the [Final](#final) section. Refer to that section for the complete implementation.

### Setup

Prior to creating a _Chat Completion Agent_, the configuration settings, plugins, and _Kernel_ must be initialized.

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

TBD $$$

::: zone pivot="programming-language-csharp"

Next initialize the plug-in using its settings.  Here, a message is displaying to indicate progress.

```csharp
Console.WriteLine("Initialize plugins...");
GitHubSettings githubSettings = settings.GetSettings<GitHubSettings>();
GitHubPlugin githubPlugin = new(githubSettings);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
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
```
::: zone-end

::: zone pivot="programming-language-java"
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
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

### The _Chat_ Loop

At last, we are able to coordinate the interaction between the user and the _Agent_.  Start by creating a _Chat History_ object to maintain the conversation state and creating an empty loop.

::: zone pivot="programming-language-csharp"
```csharp
ChatHistory history = [];
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

Now let's capture user input within the previous loop.  In this case, empty input will be ignored and the term `EXIT` will signal that the conversation is completed.  Valid nput will be added to the _Chat History_ as a _User_ message.

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
```
::: zone-end

::: zone pivot="programming-language-java"
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
```python
```
::: zone-end

::: zone pivot="programming-language-java"
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
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end



> [!div class="nextstepaction"]
> [How-To: _Open AI Assistant Agent_ Code Interpreter](./example-assistant-code.md)


