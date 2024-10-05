
---
title: How-To&colon; _Open AI Assistant Agent_ File Search (Experimental)
description: A step-by-step walk-through of defining and utilizing the features of an Open AI Assistant Agent.
zone_pivot_groups: programming-languages
author: crickman, evan.mattson
ms.topic: tutorial
ms.author: crickman, evan.mattson
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# How-To: _Open AI Assistant Agent_ File Search (Experimental)

> [!WARNING] The _Semantic Kernel Agent Framework_ is experimental, still in development and is subject to change.

## Overview

In this sample, we will explore how to use the _file-search_ tool of an [_Open AI Assistant Agent_](../assistant-agent.md) to complete comprehension tasks. The approach will be step-by-step, ensuring clarity and precision throughout the process. As part of the task, the agent will provide document citations within the response.

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
    <PackageReference Include="Microsoft.SemanticKernel.Agents.OpenAI" Version="<latest>" />
  </ItemGroup>
```

Additionally, copy the `Grimms-The-King-of-the-Golden-Mountain.txt`, `Grimms-The-Water-of-Life.txt` and `Grimms-The-White-Snake.txt` public domain content from [_Semantic Kernel_ `LearnResources` Project](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/LearnResources/Resources).  Add these files in your project folder and configure to have them copied to the output directory:

```xml
  <ItemGroup>
    <None Include="Grimms-The-King-of-the-Golden-Mountain.txt">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
    <None Include="Grimms-The-Water-of-Life.txt">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
    <None Include="Grimms-The-White-Snake.txt">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
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
```python
```
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

Prior to creating a _Open AI Assistant Agent_, the configuration settings, plugins, and _Client Provider_ must be initialized.

First initialize settings:

::: zone pivot="programming-language-csharp"

Instantiate the `Settings` class referenced in the previous [Configuration](#configuration) section.  Use the settings to also create an `OpenAIClientProvider` that will be used for the [Agent Definition](#agent-definition) as well as file-upload and the creation of a `VectorStore`.

```csharp

Settings settings = new();

OpenAIClientProvider clientProvider =
    OpenAIClientProvider.ForAzureOpenAI(
        new AzureCliCredential(),
        new Uri(settings.AzureOpenAI.Endpoint));
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Now create an empty _Vector Store for use with the _File Search_ tool:

::: zone pivot="programming-language-csharp"

Use the `OpenAIClientProvider` to access a `VectorStoreClient` and create a `VectorStore`.

```csharp
Console.WriteLine("Creating store...");
VectorStoreClient storeClient = clientProvider.Client.GetVectorStoreClient();
VectorStore store = await storeClient.CreateVectorStoreAsync();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Let's declare the the three content-files described in the previous [Configuration](#configuration) section:

::: zone pivot="programming-language-csharp"

```csharp
private static readonly string[] _fileNames =
    [
        "Grimms-The-King-of-the-Golden-Mountain.txt",
        "Grimms-The-Water-of-Life.txt",
        "Grimms-The-White-Snake.txt",
    ];
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Now upload those files and add them to the _Vector Store_:

::: zone pivot="programming-language-csharp"

Use the previously created `FileClient` and `VectorStore` client to upload each file and add it to the _Vector Store_, preserving the resulting _File References_.

```csharp
Dictionary<string, OpenAIFileInfo> fileReferences = [];

Console.WriteLine("Uploading files...");
FileClient fileClient = clientProvider.Client.GetFileClient();
foreach (string fileName in _fileNames)
{
    OpenAIFileInfo fileInfo = await fileClient.UploadFileAsync(fileName, FileUploadPurpose.Assistants);
    await storeClient.AddFileToVectorStoreAsync(store.Id, fileInfo.Id);
    fileReferences.Add(fileInfo.Id, fileInfo);
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

We are now ready to instantiate an _OpenAI Assistant Agent_ using the _Client Provider_ again. The agent is configured with its target model, _Instructions_, and the _File Search_ tool enabled. Additionally, we explicitly associate the _Vector Store_ with the _File Search_ tool.

::: zone pivot="programming-language-csharp"
```csharp
Console.WriteLine("Defining agent...");
OpenAIAssistantAgent agent =
    await OpenAIAssistantAgent.CreateAsync(
        clientProvider,
        new OpenAIAssistantDefinition(settings.AzureOpenAI.ChatModelDeployment)
        {
            Name = "SampleAssistantAgent",
            Instructions =
                """
                The document store contains the text of fictional stories.
                Always analyze the document store to provide an answer to the user's question.
                Never rely on your knowledge of stories not included in the document store.
                Always format response using markdown.
                """,
            EnableFileSearch = true,
            VectorStoreId = store.Id,
        },
        new Kernel());
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

### The _Chat_ Loop

At last, we are able to coordinate the interaction between the user and the _Agent_.  Start by creating an _Assistant Thread_ to maintain the conversation state and creating an empty loop.

Let's also ensure the resources are removed at the end of execution to minimize unnecessary charges.

::: zone pivot="programming-language-csharp"
```csharp
Console.WriteLine("Creating thread...");
string threadId = await agent.CreateThreadAsync();

Console.WriteLine("Ready!");

try
{
    bool isComplete = false;
    do
    {
    } while (!isComplete);
}
finally
{
    Console.WriteLine();
    Console.WriteLine("Cleaning-up...");
    await Task.WhenAll(
        [
            agent.DeleteThreadAsync(threadId),
            agent.DeleteAsync(),
            storeClient.DeleteVectorStoreAsync(store.Id),
            ..fileReferences.Select(fileReference => fileClient.DeleteFileAsync(fileReference.Key))
        ]);
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Now let's capture user input within the previous loop.  In this case, empty input will be ignored and the term `EXIT` will signal that the conversation is completed.  Valid nput will be added to the _Assistant Thread_ as a _User_ message.

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

await agent.AddChatMessageAsync(threadId, new ChatMessageContent(AuthorRole.User, input));
Console.WriteLine();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Before invoking the _Agent_ response, let's add a helper method to reformat the unicode annotation brackets to ANSI brackets.

::: zone pivot="programming-language-csharp"
```csharp
private static string ReplaceUnicodeBrackets(this string content) =>
    content?.Replace('【', '[').Replace('】', ']');
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

To generate an _Agent_ response to user input, invoke the agent by specifying the _Assistant Thread_. In this example, we choose a streamed response and capture any associated _Citation Annotations_ for display at the end of the response cycle. Note each streamed chunk is being reformatted using the previous helper method.

::: zone pivot="programming-language-csharp"
```csharp
List<StreamingAnnotationContent> footnotes = [];
await foreach (StreamingChatMessageContent chunk in agent.InvokeStreamingAsync(threadId))
{
    // Capture annotations for footnotes
    footnotes.AddRange(chunk.Items.OfType<StreamingAnnotationContent>());

    // Render chunk with replacements for unicode brackets.
    Console.Write(chunk.Content.ReplaceUnicodeBrackets());
}

Console.WriteLine();

// Render footnotes for captured annotations.
if (footnotes.Count > 0)
{
    Console.WriteLine();
    foreach (StreamingAnnotationContent footnote in footnotes)
    {
        Console.WriteLine($"#{footnote.Quote.ReplaceUnicodeBrackets()} - {fileReferences[footnote.FileId!].Filename} (Index: {footnote.StartIndex} - {footnote.EndIndex})");
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Files;
using OpenAI.VectorStores;

namespace AgentsSample;

public static class Program
{
    private static readonly string[] _fileNames =
        [
            "Grimms-The-King-of-the-Golden-Mountain.txt",
            "Grimms-The-Water-of-Life.txt",
            "Grimms-The-White-Snake.txt",
        ];

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Main()
    {
        // Load configuration from environment variables or user secrets.
        Settings settings = new();

        OpenAIClientProvider clientProvider =
            OpenAIClientProvider.ForAzureOpenAI(
                new AzureCliCredential(),
                new Uri(settings.AzureOpenAI.Endpoint));

        Console.WriteLine("Creating store...");
        VectorStoreClient storeClient = clientProvider.Client.GetVectorStoreClient();
        VectorStore store = await storeClient.CreateVectorStoreAsync();

        // Retain file references.
        Dictionary<string, OpenAIFileInfo> fileReferences = [];

        Console.WriteLine("Uploading files...");
        FileClient fileClient = clientProvider.Client.GetFileClient();
        foreach (string fileName in _fileNames)
        {
            OpenAIFileInfo fileInfo = await fileClient.UploadFileAsync(fileName, FileUploadPurpose.Assistants);
            await storeClient.AddFileToVectorStoreAsync(store.Id, fileInfo.Id);
            fileReferences.Add(fileInfo.Id, fileInfo);
        }


        Console.WriteLine("Defining agent...");
        OpenAIAssistantAgent agent =
            await OpenAIAssistantAgent.CreateAsync(
                clientProvider,
                new OpenAIAssistantDefinition(settings.AzureOpenAI.ChatModelDeployment)
                {
                    Name = "SampleAssistantAgent",
                    Instructions =
                        """
                        The document store contains the text of fictional stories.
                        Always analyze the document store to provide an answer to the user's question.
                        Never rely on your knowledge of stories not included in the document store.
                        Always format response using markdown.
                        """,
                    EnableFileSearch = true,
                    VectorStoreId = store.Id,
                },
                new Kernel());

        Console.WriteLine("Creating thread...");
        string threadId = await agent.CreateThreadAsync();

        Console.WriteLine("Ready!");

        try
        {
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

                await agent.AddChatMessageAsync(threadId, new ChatMessageContent(AuthorRole.User, input));
                Console.WriteLine();

                List<StreamingAnnotationContent> footnotes = [];
                await foreach (StreamingChatMessageContent chunk in agent.InvokeStreamingAsync(threadId))
                {
                    // Capture annotations for footnotes
                    footnotes.AddRange(chunk.Items.OfType<StreamingAnnotationContent>());

                    // Render chunk with replacements for unicode brackets.
                    Console.Write(chunk.Content.ReplaceUnicodeBrackets());
                }

                Console.WriteLine();

                // Render footnotes for captured annotations.
                if (footnotes.Count > 0)
                {
                    Console.WriteLine();
                    foreach (StreamingAnnotationContent footnote in footnotes)
                    {
                        Console.WriteLine($"#{footnote.Quote.ReplaceUnicodeBrackets()} - {fileReferences[footnote.FileId!].Filename} (Index: {footnote.StartIndex} - {footnote.EndIndex})");
                    }
                }
            } while (!isComplete);
        }
        finally
        {
            Console.WriteLine();
            Console.WriteLine("Cleaning-up...");
            await Task.WhenAll(
                [
                    agent.DeleteThreadAsync(threadId),
                    agent.DeleteAsync(),
                    storeClient.DeleteVectorStoreAsync(store.Id),
                    ..fileReferences.Select(fileReference => fileClient.DeleteFileAsync(fileReference.Key))
                ]);
        }
    }

    private static string ReplaceUnicodeBrackets(this string content) =>
        content?.Replace('【', '[').Replace('】', ']');
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
> [How to Coordinate Agent Collaboration using _Agent Group Chat_](./example-agent-collaboration.md)

