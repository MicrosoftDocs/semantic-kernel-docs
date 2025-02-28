---
title: How-To&colon; `OpenAIAssistantAgent` File Search
description: A step-by-step walk-through of defining and utilizing the file-search tool of an OpenAI Assistant Agent.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# How-To: `OpenAIAssistantAgent` File Search 

> [!IMPORTANT]
> This feature is in the release candidate stage. Features at this stage are nearly complete and generally stable, though they may undergo minor refinements or optimizations before reaching full general availability.

## Overview

In this sample, we will explore how to use the _file-search_ tool of an [`OpenAIAssistantAgent`](../assistant-agent.md) to complete comprehension tasks. The approach will be step-by-step, ensuring clarity and precision throughout the process. As part of the task, the agent will provide document citations within the response.

Streaming will be used to deliver the agent's responses. This will provide real-time updates as the task progresses.


## Getting Started

Before proceeding with feature coding, make sure your development environment is fully set up and configured.

::: zone pivot="programming-language-csharp"

To add package dependencies from the command-line use the `dotnet` command:

```powershell
dotnet add package Azure.Identity
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Binder
dotnet add package Microsoft.Extensions.Configuration.UserSecrets
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.SemanticKernel.Agents.OpenAI --prerelease
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
    <PackageReference Include="Microsoft.SemanticKernel" Version="<latest>" />
    <PackageReference Include="Microsoft.SemanticKernel.Agents.OpenAI" Version="<latest>" />
  </ItemGroup>
```

The `Agent Framework` is experimental and requires warning suppression.  This may addressed in as a property in the project file (`.csproj`):

```xml
  <PropertyGroup>
    <NoWarn>$(NoWarn);CA2007;IDE1006;SKEXP0001;SKEXP0110;OPENAI001</NoWarn>
  </PropertyGroup>
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
Start by creating a folder that will hold your script (`.py` file) and the sample resources. Include the following imports at the top of your `.py` file:

```python
import asyncio
import os

from semantic_kernel.agents.open_ai import AzureAssistantAgent
from semantic_kernel.contents import StreamingAnnotationContent
```

Additionally, copy the `Grimms-The-King-of-the-Golden-Mountain.txt`, `Grimms-The-Water-of-Life.txt` and `Grimms-The-White-Snake.txt` public domain content from [_Semantic Kernel_ `LearnResources` Project](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/learn_resources/resources).  Add these files in your project folder.

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Configuration

This sample requires configuration setting in order to connect to remote services.  You will need to define settings for either _OpenAI_ or _Azure OpenAI_.

::: zone pivot="programming-language-csharp"

```powershell
# OpenAI
dotnet user-secrets set "OpenAISettings:ApiKey" "<api-key>"
dotnet user-secrets set "OpenAISettings:ChatModel" "gpt-4o"

# Azure OpenAI
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
AZURE_OPENAI_ENDPOINT="https://<resource-name>.openai.azure.com/"
AZURE_OPENAI_CHAT_DEPLOYMENT_NAME="..."
AZURE_OPENAI_API_VERSION="..."

OPENAI_API_KEY="sk-..."
OPENAI_ORG_ID=""
OPENAI_CHAT_MODEL_ID=""
```

> [!TIP]
> Azure Assistants require an API version of at least 2024-05-01-preview. As new features are introduced, API versions are updated accordingly. As of this writing, the latest version is 2025-01-01-preview. For the most up-to-date versioning details, refer to the [Azure OpenAI API preview lifecycle](/azure/ai-services/openai/api-version-deprecation).

Once configured, the respective AI service classes will pick up the required variables and use them during instantiation.
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Coding

The coding process for this sample involves:

1. [Setup](#setup) - Initializing settings and the plug-in.
2. [Agent Definition](#agent-definition) - Create the _Chat_Completion`Agent` with templatized instructions and plug-in.
3. [The _Chat_ Loop](#the-chat-loop) - Write the loop that drives user / agent interaction.

The full example code is provided in the [Final](#final) section. Refer to that section for the complete implementation.

### Setup

Prior to creating an `OpenAIAssistantAgent`, ensure the configuration settings are available and prepare the file resources.

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
The class method `setup_resources()` on the Assistant Agent handles creating the client and returning it and the model to use based on the desired configuration. Pydantic settings are used to load environment variables first from environment variables or from the `.env` file. One may pass in the `api_key`, `api_version`, `deployment_name` or `endpoint`, which will take precedence over any environment variables configured.

```python
# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

Now create an empty _Vector Store for use with the _File Search_ tool:

::: zone pivot="programming-language-csharp"

Use the `OpenAIClientProvider` to access a `VectorStoreClient` and create a `VectorStore`.

```csharp
Console.WriteLine("Creating store...");
VectorStoreClient storeClient = clientProvider.Client.GetVectorStoreClient();
CreateVectorStoreOperation operation = await storeClient.CreateVectorStoreAsync(waitUntilCompleted: true);
string storeId = operation.VectorStoreId;
```
::: zone-end

::: zone pivot="programming-language-python"
```python
def get_filepath_for_filename(filename: str) -> str:
    base_directory = os.path.join(
        os.path.dirname(os.path.dirname(os.path.realpath(__file__))),
        "resources",
    )
    return os.path.join(base_directory, filename)

# Upload the files to the client
file_ids: list[str] = []
for path in [get_filepath_for_filename(filename) for filename in filenames]:
    with open(path, "rb") as file:
        file = await client.files.create(file=file, purpose="assistants")
        file_ids.append(file.id)

# Get the file search tool and resources
file_search_tools, file_search_tool_resources = AzureAssistantAgent.configure_file_search_tool(file_ids=file_ids)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

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
filenames = [
    "Grimms-The-King-of-the-Golden-Mountain.txt",
    "Grimms-The-Water-of-Life.txt",
    "Grimms-The-White-Snake.txt",
]
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

::: zone pivot="programming-language-csharp"

Now upload those files and add them to the _Vector Store_ by using the previously created `VectorStoreClient` clients to upload each file with a `OpenAIFileClient` and add it to the _Vector Store_, preserving the resulting _File References_.

```csharp
Dictionary<string, OpenAIFile> fileReferences = [];

Console.WriteLine("Uploading files...");
OpenAIFileClient fileClient = clientProvider.Client.GetOpenAIFileClient();
foreach (string fileName in _fileNames)
{
    OpenAIFile fileInfo = await fileClient.UploadFileAsync(fileName, FileUploadPurpose.Assistants);
    await storeClient.AddFileToVectorStoreAsync(storeId, fileInfo.Id, waitUntilCompleted: true);
    fileReferences.Add(fileInfo.Id, fileInfo);
}
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

### Agent Definition

We are now ready to instantiate an `OpenAIAssistantAgent`. The agent is configured with its target model, _Instructions_, and the _File Search_ tool enabled. Additionally, we explicitly associate the _Vector Store_ with the _File Search_ tool.

::: zone pivot="programming-language-csharp"

We will utilize the `OpenAIClientProvider` again as part of creating the `OpenAIAssistantAgent`:

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
            VectorStoreId = storeId,
        },
        new Kernel());
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    instructions="""
        The document store contains the text of fictional stories.
        Always analyze the document store to provide an answer to the user's question.
        Never rely on your knowledge of stories not included in the document store.
        Always format response using markdown.
        """,
    name="SampleAssistantAgent",
    tools=file_search_tools,
    tool_resources=file_search_tool_resources,
)

# Create the agent using the client and the assistant definition
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

### The _Chat_ Loop

At last, we are able to coordinate the interaction between the user and the `Agent`.  Start by creating an _Assistant Thread_ to maintain the conversation state and creating an empty loop.

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
        // Processing occurrs here
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
            storeClient.DeleteVectorStoreAsync(storeId),
            ..fileReferences.Select(fileReference => fileClient.DeleteFileAsync(fileReference.Key))
        ]);
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
print("Creating thread...")
thread_id = await agent.create_thread()

try:
    is_complete: bool = False
    while not is_complete:
        # Processing occurs here

finally:
    print("Cleaning up resources...")
    if agent is not None:
        [await agent.delete_file(file_id) for file_id in agent.file_search_file_ids]
        await agent.delete_thread(thread_id)
        await agent.delete()
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

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
user_input = input("User:> ")
if not user_input:
    continue

if user_input.lower() == "exit":
    is_complete = True
    break

await agent.add_chat_message(
    thread_id=thread_id, message=ChatMessageContent(role=AuthorRole.USER, content=user_input)
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

Before invoking the `Agent` response, let's add a helper method to reformat the unicode annotation brackets to ANSI brackets.

::: zone pivot="programming-language-csharp"
```csharp
private static string ReplaceUnicodeBrackets(this string content) =>
    content?.Replace('【', '[').Replace('】', ']');
```
::: zone-end

::: zone pivot="programming-language-python"
```python
# No special handling required.
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

To generate an `Agent` response to user input, invoke the agent by specifying the _Assistant Thread_. In this example, we choose a streamed response and capture any associated _Citation Annotations_ for display at the end of the response cycle. Note each streamed chunk is being reformatted using the previous helper method.

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
footnotes: list[StreamingAnnotationContent] = []
async for response in agent.invoke_stream(thread_id=thread_id):
    footnotes.extend([item for item in response.items if isinstance(item, StreamingAnnotationContent)])

    print(f"{response.content}", end="", flush=True)

print()

if len(footnotes) > 0:
    for footnote in footnotes:
        print(
            f"\n`{footnote.quote}` => {footnote.file_id} "
            f"(Index: {footnote.start_index} - {footnote.end_index})"
        )
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Final

Bringing all the steps together, we have the final code for this example. The complete implementation is provided below.

Try using these suggested inputs:

1. What is the paragraph count for each of the stories?
2. Create a table that identifies the protagonist and antagonist for each story.
3. What is the moral in The White Snake?

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
        CreateVectorStoreOperation operation = await storeClient.CreateVectorStoreAsync(waitUntilCompleted: true);
        string storeId = operation.VectorStoreId;

        // Retain file references.
        Dictionary<string, OpenAIFile> fileReferences = [];

        Console.WriteLine("Uploading files...");
        OpenAIFileClient fileClient = clientProvider.Client.GetOpenAIFileClient();
        foreach (string fileName in _fileNames)
        {
            OpenAIFile fileInfo = await fileClient.UploadFileAsync(fileName, FileUploadPurpose.Assistants);
            await storeClient.AddFileToVectorStoreAsync(storeId, fileInfo.Id, waitUntilCompleted: true);
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
                    VectorStoreId = storeId,
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
                    storeClient.DeleteVectorStoreAsync(storeId),
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
# Copyright (c) Microsoft. All rights reserved.

import asyncio
import os

from semantic_kernel.agents.open_ai import AzureAssistantAgent
from semantic_kernel.contents import StreamingAnnotationContent

"""
The following sample demonstrates how to create a simple,
OpenAI assistant agent that utilizes the vector store
to answer questions based on the uploaded documents.
"""


def get_filepath_for_filename(filename: str) -> str:
    base_directory = os.path.join(
        os.path.dirname(os.path.dirname(os.path.realpath(__file__))),
        "resources",
    )
    return os.path.join(base_directory, filename)


filenames = [
    "Grimms-The-King-of-the-Golden-Mountain.txt",
    "Grimms-The-Water-of-Life.txt",
    "Grimms-The-White-Snake.txt",
]


async def main():
    # Create the client using Azure OpenAI resources and configuration
    client, model = AzureAssistantAgent.setup_resources()

    # Upload the files to the client
    file_ids: list[str] = []
    for path in [get_filepath_for_filename(filename) for filename in filenames]:
        with open(path, "rb") as file:
            file = await client.files.create(file=file, purpose="assistants")
            file_ids.append(file.id)

    vector_store = await client.beta.vector_stores.create(
        name="assistant_search",
        file_ids=file_ids,
    )

    # Get the file search tool and resources
    file_search_tools, file_search_tool_resources = AzureAssistantAgent.configure_file_search_tool(
        vector_store_ids=vector_store.id
    )

    # Create the assistant definition
    definition = await client.beta.assistants.create(
        model=model,
        instructions="""
            The document store contains the text of fictional stories.
            Always analyze the document store to provide an answer to the user's question.
            Never rely on your knowledge of stories not included in the document store.
            Always format response using markdown.
            """,
        name="SampleAssistantAgent",
        tools=file_search_tools,
        tool_resources=file_search_tool_resources,
    )

    # Create the agent using the client and the assistant definition
    agent = AzureAssistantAgent(
        client=client,
        definition=definition,
    )

    print("Creating thread...")
    thread = await client.beta.threads.create()

    try:
        is_complete: bool = False
        while not is_complete:
            user_input = input("User:> ")
            if not user_input:
                continue

            if user_input.lower() == "exit":
                is_complete = True
                break

            await agent.add_chat_message(thread_id=thread.id, message=user_input)

            footnotes: list[StreamingAnnotationContent] = []
            async for response in agent.invoke_stream(thread_id=thread.id):
                footnotes.extend([item for item in response.items if isinstance(item, StreamingAnnotationContent)])

                print(f"{response.content}", end="", flush=True)

            print()

            if len(footnotes) > 0:
                for footnote in footnotes:
                    print(
                        f"\n`{footnote.quote}` => {footnote.file_id} "
                        f"(Index: {footnote.start_index} - {footnote.end_index})"
                    )

    finally:
        print("\nCleaning up resources...")
        [await client.files.delete(file_id) for file_id in file_ids]
        await client.beta.threads.delete(thread.id)
        await client.beta.assistants.delete(agent.id)


if __name__ == "__main__":
    asyncio.run(main())
```

You may find the full [code](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/learn_resources/agent_docs/assistant_search.py), as shown above, in our repo.
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


> [!div class="nextstepaction"]
> [How to Coordinate Agent Collaboration using `AgentGroupChat`](./example-agent-collaboration.md)

