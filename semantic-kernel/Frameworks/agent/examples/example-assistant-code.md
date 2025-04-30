---
title: How-To use `OpenAIAssistantAgent` - Code Interpreter
description: A step-by-step walk-through of defining and utilizing the code-interpreter tool of an OpenAI Assistant Agent.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# How-To: `OpenAIAssistantAgent` Code Interpreter

> [!IMPORTANT]
> This feature is in the release candidate stage. Features at this stage are nearly complete and generally stable, though they may undergo minor refinements or optimizations before reaching full general availability.

## Overview

In this sample, we will explore how to use the code-interpreter tool of an [`OpenAIAssistantAgent`](../assistant-agent.md) to complete data-analysis tasks. The approach will be broken down step-by-step to high-light the key parts of the coding process. As part of the task, the agent will generate both image and text responses. This will demonstrate the versatility of this tool in performing quantitative analysis.

Streaming will be used to deliver the agent's responses. This will provide real-time updates as the task progresses.


## Getting Started

Before proceeding with feature coding, make sure your development environment is fully set up and configured.

::: zone pivot="programming-language-csharp"

Start by creating a Console project. Then, include the following package references to ensure all required dependencies are available.

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

> If managing NuGet packages in Visual Studio, ensure `Include prerelease` is checked.

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

Additionally, copy the `PopulationByAdmin1.csv` and `PopulationByCountry.csv` data files from [Semantic Kernel `LearnResources` Project](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/LearnResources/Resources).  Add these files in your project folder and configure to have them copied to the output directory:

```xml
  <ItemGroup>
    <None Include="PopulationByAdmin1.csv">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
    <None Include="PopulationByCountry.csv">
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

from semantic_kernel.agents import AssistantAgentThread, AzureAssistantAgent
from semantic_kernel.contents import StreamingFileReferenceContent
```

Additionally, copy the `PopulationByAdmin1.csv` and `PopulationByCountry.csv` data files from the [Semantic Kernel `learn_resources/resources` directory](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/learn_resources/resources). Add these files to your working directory.

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

## Configuration

This sample requires configuration setting in order to connect to remote services.  You will need to define settings for either OpenAI or Azure OpenAI.

::: zone pivot="programming-language-csharp"

```powershell
# OpenAI
dotnet user-secrets set "OpenAISettings:ApiKey" "<api-key>"
dotnet user-secrets set "OpenAISettings:ChatModel" "gpt-4o"

# Azure OpenAI
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
2. [Agent Definition](#agent-definition) - Create the _OpenAI_Assistant`Agent` with templatized instructions and plug-in.
3. [The Chat Loop](#the-chat-loop) - Write the loop that drives user / agent interaction.

The full example code is provided in the [Final](#final) section. Refer to that section for the complete implementation.

### Setup

::: zone pivot="programming-language-csharp"

Prior to creating an `OpenAIAssistantAgent`, ensure the configuration settings are available and prepare the file resources.

Instantiate the `Settings` class referenced in the previous [Configuration](#configuration) section.  Use the settings to also create an `AzureOpenAIClient` that will be used for the [Agent Definition](#agent-definition) as well as file-upload.

```csharp
Settings settings = new();

AzureOpenAIClient client = OpenAIAssistantAgent.CreateAzureOpenAIClient(new AzureCliCredential(), new Uri(settings.AzureOpenAI.Endpoint));
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

::: zone pivot="programming-language-csharp"

Use the `AzureOpenAIClient` to access an `OpenAIFileClient` and upload the two data-files described in the previous [Configuration](#configuration) section, preserving the _File Reference_ for final clean-up.

```csharp
Console.WriteLine("Uploading files...");
OpenAIFileClient fileClient = client.GetOpenAIFileClient();
OpenAIFile fileDataCountryDetail = await fileClient.UploadFileAsync("PopulationByAdmin1.csv", FileUploadPurpose.Assistants);
OpenAIFile fileDataCountryList = await fileClient.UploadFileAsync("PopulationByCountry.csv", FileUploadPurpose.Assistants);
```
::: zone-end

::: zone pivot="programming-language-python"

Prior to creating an `AzureAssistantAgent` or an `OpenAIAssistantAgent`, ensure the configuration settings are available and prepare the file resources.

> [!TIP]
> You may need to adjust the file paths depending upon where your files are located.

```python
# Let's form the file paths that we will use as part of file upload
csv_file_path_1 = os.path.join(
    os.path.dirname(os.path.dirname(os.path.realpath(__file__))),
    "resources",
    "PopulationByAdmin1.csv",
)

csv_file_path_2 = os.path.join(
    os.path.dirname(os.path.dirname(os.path.realpath(__file__))),
    "resources",
    "PopulationByCountry.csv",
)
```

```python
# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

# Upload the files to the client
file_ids: list[str] = []
for path in [csv_file_path_1, csv_file_path_2]:
    with open(path, "rb") as file:
        file = await client.files.create(file=file, purpose="assistants")
        file_ids.append(file.id)

# Get the code interpreter tool and resources
code_interpreter_tools, code_interpreter_tool_resources = AzureAssistantAgent.configure_code_interpreter_tool(
    file_ids=file_ids
)

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    instructions="""
        Analyze the available data to provide an answer to the user's question.
        Always format response using markdown.
        Always include a numerical index that starts at 1 for any lists or tables.
        Always sort lists in ascending order.
        """,
    name="SampleAssistantAgent",
    tools=code_interpreter_tools,
    tool_resources=code_interpreter_tool_resources,
)
```

We first set up the Azure OpenAI resources to obtain the client and model. Next, we upload the CSV files from the specified paths using the client's Files API. We then configure the `code_interpreter_tool` using the uploaded file IDs, which are linked to the assistant upon creation along with the model, instructions, and name.

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

### Agent Definition

::: zone pivot="programming-language-csharp"

We are now ready to instantiate an `OpenAIAssistantAgent` by first creating an assistant definition. The assistant is configured with its target model, _Instructions_, and the _Code Interpreter_ tool enabled. Additionally, we explicitly associate the two data files with the _Code Interpreter_ tool.

```csharp
Console.WriteLine("Defining agent...");
AssistantClient assistantClient = client.GetAssistantClient();
        Assistant assistant =
            await assistantClient.CreateAssistantAsync(
                settings.AzureOpenAI.ChatModelDeployment,
                name: "SampleAssistantAgent",
                instructions:
                        """
                        Analyze the available data to provide an answer to the user's question.
                        Always format response using markdown.
                        Always include a numerical index that starts at 1 for any lists or tables.
                        Always sort lists in ascending order.
                        """,
                enableCodeInterpreter: true,
                codeInterpreterFileIds: [fileDataCountryList.Id, fileDataCountryDetail.Id]);

// Create agent
OpenAIAssistantAgent agent = new(assistant, assistantClient);
```
::: zone-end

::: zone pivot="programming-language-python"

We are now ready to instantiate an `AzureAssistantAgent`. The agent is configured with the client and the assistant definition.

```python
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

### The Chat Loop

At last, we are able to coordinate the interaction between the user and the `Agent`.  Start by creating an `AgentThread` to maintain the conversation state and creating an empty loop.

Let's also ensure the resources are removed at the end of execution to minimize unnecessary charges.

::: zone pivot="programming-language-csharp"
```csharp
Console.WriteLine("Creating thread...");
AssistantAgentThread agentThread = new();

Console.WriteLine("Ready!");

try
{
    bool isComplete = false;
    List<string> fileIds = [];
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
            agentThread.DeleteAsync(),
            assistantClient.DeleteAssistantAsync(assistant.Id),
            fileClient.DeleteFileAsync(fileDataCountryList.Id),
            fileClient.DeleteFileAsync(fileDataCountryDetail.Id),
        ]);
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
thread: AssistantAgentThread = None

try:
    is_complete: bool = False
    file_ids: list[str] = []
    while not is_complete:
        # agent interaction logic here
finally:
    print("\nCleaning up resources...")
    [await client.files.delete(file_id) for file_id in file_ids]
    await thread.delete() if thread else None
    await client.beta.assistants.delete(agent.id)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

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

> Agents are currently unavailable in Java.

::: zone-end

Before invoking the `Agent` response, let's add some helper methods to download any files that may be produced by the `Agent`.

::: zone pivot="programming-language-csharp"
Here we're place file content in the system defined temporary directory and then launching the system defined viewer application.

```csharp
private static async Task DownloadResponseImageAsync(OpenAIFileClient client, ICollection<string> fileIds)
{
    if (fileIds.Count > 0)
    {
        Console.WriteLine();
        foreach (string fileId in fileIds)
        {
            await DownloadFileContentAsync(client, fileId, launchViewer: true);
        }
    }
}

private static async Task DownloadFileContentAsync(OpenAIFileClient client, string fileId, bool launchViewer = false)
{
    OpenAIFile fileInfo = client.GetFile(fileId);
    if (fileInfo.Purpose == FilePurpose.AssistantsOutput)
    {
        string filePath =
            Path.Combine(
                Path.GetTempPath(),
                Path.GetFileName(Path.ChangeExtension(fileInfo.Filename, ".png")));

        BinaryData content = await client.DownloadFileAsync(fileId);
        await using FileStream fileStream = new(filePath, FileMode.CreateNew);
        await content.ToStream().CopyToAsync(fileStream);
        Console.WriteLine($"File saved to: {filePath}.");

        if (launchViewer)
        {
            Process.Start(
                new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C start {filePath}"
                });
        }
    }
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
import os

async def download_file_content(agent, file_id: str):
    try:
        # Fetch the content of the file using the provided method
        response_content = await agent.client.files.content(file_id)

        # Get the current working directory of the file
        current_directory = os.path.dirname(os.path.abspath(__file__))

        # Define the path to save the image in the current directory
        file_path = os.path.join(
            current_directory,  # Use the current directory of the file
            f"{file_id}.png"  # You can modify this to use the actual filename with proper extension
        )

        # Save content to a file asynchronously
        with open(file_path, "wb") as file:
            file.write(response_content.content)

        print(f"File saved to: {file_path}")
    except Exception as e:
        print(f"An error occurred while downloading file {file_id}: {str(e)}")

async def download_response_image(agent, file_ids: list[str]):
    if file_ids:
        # Iterate over file_ids and download each one
        for file_id in file_ids:
            await download_file_content(agent, file_id)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

To generate an `Agent` response to user input, invoke the agent by providing the message and the `AgentThread`. In this example, we choose a streamed response and capture any generated _File References_ for download and review at the end of the response cycle. It's important to note that generated code is identified by the presence of a _Metadata_ key in the response message, distinguishing it from the conversational reply.

::: zone pivot="programming-language-csharp"
```csharp
bool isCode = false;
await foreach (StreamingChatMessageContent response in agent.InvokeStreamingAsync(message, agentThread))
{
    if (isCode != (response.Metadata?.ContainsKey(OpenAIAssistantAgent.CodeInterpreterMetadataKey) ?? false))
    {
        Console.WriteLine();
        isCode = !isCode;
    }

    // Display response.
    Console.Write($"{response.Content}");

    // Capture file IDs for downloading.
    fileIds.AddRange(response.Items.OfType<StreamingFileReferenceContent>().Select(item => item.FileId));
}
Console.WriteLine();

// Download any files referenced in the response.
await DownloadResponseImageAsync(fileClient, fileIds);
fileIds.Clear();
```
::: zone-end

::: zone pivot="programming-language-python"
```python
is_code = False
last_role = None
async for response in agent.invoke_stream(messages=user_input, thread=thread):
    current_is_code = response.metadata.get("code", False)

    if current_is_code:
        if not is_code:
            print("\n\n```python")
            is_code = True
        print(response.content, end="", flush=True)
    else:
        if is_code:
            print("\n```")
            is_code = False
            last_role = None
        if hasattr(response, "role") and response.role is not None and last_role != response.role:
            print(f"\n# {response.role}: ", end="", flush=True)
            last_role = response.role
        print(response.content, end="", flush=True)
    file_ids.extend([
        item.file_id for item in response.items if isinstance(item, StreamingFileReferenceContent)
    ])
    thread = response.thread
if is_code:
    print("```\n")
print()

await download_response_image(agent, file_ids)
file_ids.clear()
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Final

Bringing all the steps together, we have the final code for this example. The complete implementation is provided below.

Try using these suggested inputs:

1. Compare the files to determine the number of countries do not have a state or province defined compared to the total count
2. Create a table for countries with state or province defined.  Include the count of states or provinces and the total population
3. Provide a bar chart for countries whose names start with the same letter and sort the x axis by highest count to lowest (include all countries)

::: zone pivot="programming-language-csharp"
```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Assistants;
using OpenAI.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AgentsSample;

public static class Program
{
    public static async Task Main()
    {
        // Load configuration from environment variables or user secrets.
        Settings settings = new();

        // Initialize the clients
        AzureOpenAIClient client = OpenAIAssistantAgent.CreateAzureOpenAIClient(new AzureCliCredential(), new Uri(settings.AzureOpenAI.Endpoint));
        //OpenAIClient client = OpenAIAssistantAgent.CreateOpenAIClient(new ApiKeyCredential(settings.OpenAI.ApiKey)));
        AssistantClient assistantClient = client.GetAssistantClient();
        OpenAIFileClient fileClient = client.GetOpenAIFileClient();

        // Upload files
        Console.WriteLine("Uploading files...");
        OpenAIFile fileDataCountryDetail = await fileClient.UploadFileAsync("PopulationByAdmin1.csv", FileUploadPurpose.Assistants);
        OpenAIFile fileDataCountryList = await fileClient.UploadFileAsync("PopulationByCountry.csv", FileUploadPurpose.Assistants);

        // Define assistant
        Console.WriteLine("Defining assistant...");
        Assistant assistant =
            await assistantClient.CreateAssistantAsync(
                settings.AzureOpenAI.ChatModelDeployment,
                name: "SampleAssistantAgent",
                instructions:
                        """
                        Analyze the available data to provide an answer to the user's question.
                        Always format response using markdown.
                        Always include a numerical index that starts at 1 for any lists or tables.
                        Always sort lists in ascending order.
                        """,
                enableCodeInterpreter: true,
                codeInterpreterFileIds: [fileDataCountryList.Id, fileDataCountryDetail.Id]);

        // Create agent
        OpenAIAssistantAgent agent = new(assistant, assistantClient);

        // Create the conversation thread
        Console.WriteLine("Creating thread...");
        AssistantAgentThread agentThread = new();

        Console.WriteLine("Ready!");

        try
        {
            bool isComplete = false;
            List<string> fileIds = [];
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

                bool isCode = false;
                await foreach (StreamingChatMessageContent response in agent.InvokeStreamingAsync(message, agentThread))
                {
                    if (isCode != (response.Metadata?.ContainsKey(OpenAIAssistantAgent.CodeInterpreterMetadataKey) ?? false))
                    {
                        Console.WriteLine();
                        isCode = !isCode;
                    }

                    // Display response.
                    Console.Write($"{response.Content}");

                    // Capture file IDs for downloading.
                    fileIds.AddRange(response.Items.OfType<StreamingFileReferenceContent>().Select(item => item.FileId));
                }
                Console.WriteLine();

                // Download any files referenced in the response.
                await DownloadResponseImageAsync(fileClient, fileIds);
                fileIds.Clear();

            } while (!isComplete);
        }
        finally
        {
            Console.WriteLine();
            Console.WriteLine("Cleaning-up...");
            await Task.WhenAll(
                [
                    agentThread.DeleteAsync(),
                    assistantClient.DeleteAssistantAsync(assistant.Id),
                    fileClient.DeleteFileAsync(fileDataCountryList.Id),
                    fileClient.DeleteFileAsync(fileDataCountryDetail.Id),
                ]);
        }
    }

    private static async Task DownloadResponseImageAsync(OpenAIFileClient client, ICollection<string> fileIds)
    {
        if (fileIds.Count > 0)
        {
            Console.WriteLine();
            foreach (string fileId in fileIds)
            {
                await DownloadFileContentAsync(client, fileId, launchViewer: true);
            }
        }
    }

    private static async Task DownloadFileContentAsync(OpenAIFileClient client, string fileId, bool launchViewer = false)
    {
        OpenAIFile fileInfo = client.GetFile(fileId);
        if (fileInfo.Purpose == FilePurpose.AssistantsOutput)
        {
            string filePath =
                Path.Combine(
                    Path.GetTempPath(),
                    Path.GetFileName(Path.ChangeExtension(fileInfo.Filename, ".png")));

            BinaryData content = await client.DownloadFileAsync(fileId);
            await using FileStream fileStream = new(filePath, FileMode.CreateNew);
            await content.ToStream().CopyToAsync(fileStream);
            Console.WriteLine($"File saved to: {filePath}.");

            if (launchViewer)
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C start {filePath}"
                    });
            }
        }
    }
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
import asyncio
import logging
import os

from semantic_kernel.agents import AssistantAgentThread, AzureAssistantAgent
from semantic_kernel.contents import StreamingFileReferenceContent

logging.basicConfig(level=logging.ERROR)

"""
The following sample demonstrates how to create a simple,
OpenAI assistant agent that utilizes the code interpreter
to analyze uploaded files.
"""

# Let's form the file paths that we will later pass to the assistant
csv_file_path_1 = os.path.join(
    os.path.dirname(os.path.dirname(os.path.realpath(__file__))),
    "resources",
    "PopulationByAdmin1.csv",
)

csv_file_path_2 = os.path.join(
    os.path.dirname(os.path.dirname(os.path.realpath(__file__))),
    "resources",
    "PopulationByCountry.csv",
)


async def download_file_content(agent: AzureAssistantAgent, file_id: str):
    try:
        # Fetch the content of the file using the provided method
        response_content = await agent.client.files.content(file_id)

        # Get the current working directory of the file
        current_directory = os.path.dirname(os.path.abspath(__file__))

        # Define the path to save the image in the current directory
        file_path = os.path.join(
            current_directory,  # Use the current directory of the file
            f"{file_id}.png",  # You can modify this to use the actual filename with proper extension
        )

        # Save content to a file asynchronously
        with open(file_path, "wb") as file:
            file.write(response_content.content)

        print(f"File saved to: {file_path}")
    except Exception as e:
        print(f"An error occurred while downloading file {file_id}: {str(e)}")


async def download_response_image(agent: AzureAssistantAgent, file_ids: list[str]):
    if file_ids:
        # Iterate over file_ids and download each one
        for file_id in file_ids:
            await download_file_content(agent, file_id)


async def main():
    # Create the client using Azure OpenAI resources and configuration
    client, model = AzureAssistantAgent.setup_resources()

    # Upload the files to the client
    file_ids: list[str] = []
    for path in [csv_file_path_1, csv_file_path_2]:
        with open(path, "rb") as file:
            file = await client.files.create(file=file, purpose="assistants")
            file_ids.append(file.id)

    # Get the code interpreter tool and resources
    code_interpreter_tools, code_interpreter_tool_resources = AzureAssistantAgent.configure_code_interpreter_tool(
        file_ids=file_ids
    )

    # Create the assistant definition
    definition = await client.beta.assistants.create(
        model=model,
        instructions="""
            Analyze the available data to provide an answer to the user's question.
            Always format response using markdown.
            Always include a numerical index that starts at 1 for any lists or tables.
            Always sort lists in ascending order.
            """,
        name="SampleAssistantAgent",
        tools=code_interpreter_tools,
        tool_resources=code_interpreter_tool_resources,
    )

    # Create the agent using the client and the assistant definition
    agent = AzureAssistantAgent(
        client=client,
        definition=definition,
    )

    thread: AssistantAgentThread = None

    try:
        is_complete: bool = False
        file_ids: list[str] = []
        while not is_complete:
            user_input = input("User:> ")
            if not user_input:
                continue

            if user_input.lower() == "exit":
                is_complete = True
                break

            is_code = False
            last_role = None
            async for response in agent.invoke_stream(messages=user_input, thread=thread):
                current_is_code = response.metadata.get("code", False)

                if current_is_code:
                    if not is_code:
                        print("\n\n```python")
                        is_code = True
                    print(response.content, end="", flush=True)
                else:
                    if is_code:
                        print("\n```")
                        is_code = False
                        last_role = None
                    if hasattr(response, "role") and response.role is not None and last_role != response.role:
                        print(f"\n# {response.role}: ", end="", flush=True)
                        last_role = response.role
                    print(response.content, end="", flush=True)
                file_ids.extend([
                    item.file_id for item in response.items if isinstance(item, StreamingFileReferenceContent)
                ])
                thread = response.thread
            if is_code:
                print("```\n")
            print()

            await download_response_image(agent, file_ids)
            file_ids.clear()

    finally:
        print("\nCleaning up resources...")
        [await client.files.delete(file_id) for file_id in file_ids]
        await thread.delete() if thread else None
        await client.beta.assistants.delete(agent.id)


if __name__ == "__main__":
    asyncio.run(main())
```

You may find the full [code](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/learn_resources/agent_docs/assistant_code.py), as shown above, in our repo.
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


> [!div class="nextstepaction"]
> [How-To: `OpenAIAssistantAgent` Code File Search](./example-assistant-search.md)

