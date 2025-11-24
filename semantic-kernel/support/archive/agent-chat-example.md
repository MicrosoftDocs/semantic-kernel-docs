---
title: How-To Coordinate Agent Collaboration using Agent Group Chat (Experimental)
description: A step-by-step walk-through for coordinating agent collaboration using Agent Group Chat.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# How-To: Coordinate Agent Collaboration using Agent Group Chat

> [!IMPORTANT]
> This is an archived document.

> [!IMPORTANT]
> This feature is in the experimental stage but no longer maintained. For a replacement, see the [Group Chat Orchestration](./../../Frameworks/agent/agent-orchestration/group-chat.md) and the migration guide [Migrating from AgentChat to Group Chat Orchestration](./../migration/group-chat-orchestration-migration-guide.md).

## Overview

In this sample, we will explore how to use `AgentGroupChat` to coordinate collaboration of two different agents working to review and rewrite user provided content.  Each agent is assigned a distinct role:

- **Reviewer**: Reviews and provides direction to Writer.
- **Writer**: Updates user content based on Reviewer input.

The approach will be broken down step-by-step to high-light the key parts of the coding process.

## Getting Started

Before proceeding with feature coding, make sure your development environment is fully set up and configured.

::: zone pivot="programming-language-csharp"

> [!TIP]
> This sample uses an optional text file as part of processing. If you'd like to use it, you may download it [here](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/LearnResources/Resources/WomensSuffrage.txt). Place the file in your code working directory.

Start by creating a Console project. Then, include the following package references to ensure all required dependencies are available.

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
::: zone-end

::: zone pivot="programming-language-python"

> [!TIP]
> This sample uses an optional text file as part of processing. If you'd like to use it, you may download it [here](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/learn_resources/resources/WomensSuffrage.txt). Place the file in your code working directory.

Start by installing the Semantic Kernel Python package.

```bash
pip install semantic-kernel
```

Next add the required imports.

```python
import asyncio
import os

from semantic_kernel import Kernel
from semantic_kernel.agents import AgentGroupChat, ChatCompletionAgent
from semantic_kernel.agents.strategies import (
    KernelFunctionSelectionStrategy,
    KernelFunctionTerminationStrategy,
)
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.contents import ChatHistoryTruncationReducer
from semantic_kernel.functions import KernelFunctionFromPrompt
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Configuration

::: zone pivot="programming-language-csharp"

This sample requires configuration setting in order to connect to remote services.  You will need to define settings for either OpenAI or Azure OpenAI.

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
The quickest way to get started with the proper configuration to run the sample code is to create a `.env` file at the root of your project (where your script is run). The sample requires that you have Azure OpenAI or OpenAI resources available.

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

> Feature currently unavailable in Java.

::: zone-end

## Coding

The coding process for this sample involves:

1. [Setup](#setup) - Initializing settings and the plug-in.
2. [`Agent` Definition](#agent-definition) - Create the two `ChatCompletionAgent` instances (_Reviewer_ and _Writer_).
3. [_Chat_ Definition](#chat-definition) - Create the `AgentGroupChat` and associated strategies.
4. [The _Chat_ Loop](#the-chat-loop) - Write the loop that drives user / agent interaction.

The full example code is provided in the [Final](#final) section. Refer to that section for the complete implementation.

### Setup

Prior to creating any `ChatCompletionAgent`, the configuration settings, plugins, and `Kernel` must be initialized.

::: zone pivot="programming-language-csharp"

Instantiate the the `Settings` class referenced in the previous [Configuration](#configuration) section.

```csharp
Settings settings = new();
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

::: zone pivot="programming-language-csharp"

Now initialize a `Kernel` instance with an `IChatCompletionService`.
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
Initialize the kernel object:

```python
kernel = Kernel()
```
::: zone-end

::: zone pivot="programming-language-java"
> Feature currently unavailable in Java.
::: zone-end

::: zone pivot="programming-language-csharp"
Let's also create a second `Kernel` instance via _cloning_ and add a plug-in that will allow the review to place updated content on the clip-board.

```csharp
Kernel toolKernel = kernel.Clone();
toolKernel.Plugins.AddFromType<ClipboardAccess>();
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

::: zone pivot="programming-language-csharp"
The _Clipboard_ plugin may be defined as part of the sample.

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

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

### Agent Definition

::: zone pivot="programming-language-csharp"
Let's declare the agent names as `const` so they might be referenced in `AgentGroupChat` strategies:

```csharp
const string ReviewerName = "Reviewer";
const string WriterName = "Writer";
```
::: zone-end

::: zone pivot="programming-language-python"

We will declare the agent names as "Reviewer" and "Writer."

```python
REVIEWER_NAME = "Reviewer"
COPYWRITER_NAME = "Writer"
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

Defining the _Reviewer_ agent uses the pattern explored in [How-To: Chat Completion Agent](./../../Frameworks/agent/examples/example-chat-agent.md).

Here the _Reviewer_ is given the role of responding to user input, providing direction to the _Writer_ agent, and verifying result of the _Writer_ agent.

::: zone pivot="programming-language-csharp"
```csharp
ChatCompletionAgent agentReviewer =
    new()
    {
        Name = ReviewerName,
        Instructions =
            """
            Your responsibility is to review and identify how to improve user provided content.
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
agent_reviewer = ChatCompletionAgent(
        kernel=kernel,
        name=REVIEWER_NAME,
        instructions="""
Your responsibility is to review and identify how to improve user provided content.
If the user has provided input or direction for content already provided, specify how to address this input.
Never directly perform the correction or provide an example.
Once the content has been updated in a subsequent response, review it again until it is satisfactory.

RULES:
- Only identify suggestions that are specific and actionable.
- Verify previous suggestions have been addressed.
- Never repeat previous suggestions.
""",
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

::: zone pivot="programming-language-csharp"
The _Writer_ agent is similar, but doesn't require the specification of Execution Settings since it isn't configured with a plug-in.

Here the _Writer_ is given a single-purpose task, follow direction and rewrite the content.

```csharp
ChatCompletionAgent agentWriter =
    new()
    {
        Name = WriterName,
        Instructions =
            """
            Your sole responsibility is to rewrite content according to review suggestions.

            - Always apply all review direction.
            - Always revise the content in its entirety without explanation.
            - Never address the user.
            """,
        Kernel = kernel,
    };
```
::: zone-end

::: zone pivot="programming-language-python"
The _Writer_ agent is similar. It is given a single-purpose task, follow direction and rewrite the content.
```python
agent_writer = ChatCompletionAgent(
        kernel=kernel,
        name=WRITER_NAME,
        instructions="""
Your sole responsibility is to rewrite content according to review suggestions.
- Always apply all review directions.
- Always revise the content in its entirety without explanation.
- Never address the user.
""",
    )
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

### Chat Definition

Defining the `AgentGroupChat` requires considering the strategies for selecting the `Agent` turn and determining when to exit the _Chat_ loop.  For both of these considerations, we will define a _Kernel Prompt Function_.

The first to reason over `Agent` selection:

::: zone pivot="programming-language-csharp"

Using `AgentGroupChat.CreatePromptFunctionForStrategy` provides a convenient mechanism to avoid _HTML encoding_ the message parameter.

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
selection_function = KernelFunctionFromPrompt(
    function_name="selection", 
    prompt=f"""
Examine the provided RESPONSE and choose the next participant.
State only the name of the chosen participant without explanation.
Never choose the participant named in the RESPONSE.

Choose only from these participants:
- {REVIEWER_NAME}
- {WRITER_NAME}

Rules:
- If RESPONSE is user input, it is {REVIEWER_NAME}'s turn.
- If RESPONSE is by {REVIEWER_NAME}, it is {WRITER_NAME}'s turn.
- If RESPONSE is by {WRITER_NAME}, it is {REVIEWER_NAME}'s turn.

RESPONSE:
{{{{$lastmessage}}}}
"""
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

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
termination_keyword = "yes"

termination_function = KernelFunctionFromPrompt(
    function_name="termination", 
    prompt=f"""
Examine the RESPONSE and determine whether the content has been deemed satisfactory.
If the content is satisfactory, respond with a single word without explanation: {termination_keyword}.
If specific suggestions are being provided, it is not satisfactory.
If no correction is suggested, it is satisfactory.

RESPONSE:
{{{{$lastmessage}}}}
"""
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

Both of these _Strategies_ will only require knowledge of the most recent _Chat_ message.  This will reduce token usage and help improve performance:

::: zone pivot="programming-language-csharp"
```csharp
ChatHistoryTruncationReducer historyReducer = new(1);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
history_reducer = ChatHistoryTruncationReducer(target_count=1)
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

Finally we are ready to bring everything together in our `AgentGroupChat` definition.

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
Creating `AgentGroupChat` involves:

1. Include both agents in the constructor.
2. Define a `KernelFunctionSelectionStrategy` using the previously defined `KernelFunction` and `Kernel` instance.
3. Define a `KernelFunctionTerminationStrategy` using the previously defined `KernelFunction` and `Kernel` instance.

Notice that each strategy is responsible for parsing the `KernelFunction` result.
```python
chat = AgentGroupChat(
    agents=[agent_reviewer, agent_writer],
    selection_strategy=KernelFunctionSelectionStrategy(
        initial_agent=agent_reviewer,
        function=selection_function,
        kernel=kernel,
        result_parser=lambda result: str(result.value[0]).strip() if result.value[0] is not None else WRITER_NAME,
        history_variable_name="lastmessage",
        history_reducer=history_reducer,
    ),
    termination_strategy=KernelFunctionTerminationStrategy(
        agents=[agent_reviewer],
        function=termination_function,
        kernel=kernel,
        result_parser=lambda result: termination_keyword in str(result.value[0]).lower(),
        history_variable_name="lastmessage",
        maximum_iterations=10,
        history_reducer=history_reducer,
    ),
)
```

The `lastmessage` `history_variable_name` corresponds with the `KernelFunctionSelectionStrategy` and the `KernelFunctionTerminationStrategy` prompt that was defined above. This is where the last message is placed when rendering the prompt.
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

### The _Chat_ Loop

At last, we are able to coordinate the interaction between the user and the `AgentGroupChat`.  Start by creating creating an empty loop.

> Note: Unlike the other examples, no external history or _thread_ is managed.  `AgentGroupChat` manages the conversation history internally.

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
is_complete: bool = False
while not is_complete:
    # operational logic
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

::: zone pivot="programming-language-csharp"
Now let's capture user input within the previous loop.  In this case:
- Empty input will be ignored 
- The term `EXIT` will signal that the conversation is completed
- The term `RESET` will clear the `AgentGroupChat` history
- Any term starting with `@` will be treated as a file-path whose content will be provided as input
- Valid input will be added to the `AgentGroupChat` as a _User_ message.

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
    Console.WriteLine("[Conversation has been reset]");
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
Now let's capture user input within the previous loop.  In this case:
- Empty input will be ignored.
- The term `exit` will signal that the conversation is complete.
- The term `reset` will clear the `AgentGroupChat` history.
- Any term starting with `@` will be treated as a file-path whose content will be provided as input.
- Valid input will be added to the `AgentGroupChat` as a _User_ message.

The operation logic inside the while loop looks like:

```python
print()
user_input = input("User > ").strip()
if not user_input:
    continue

if user_input.lower() == "exit":
    is_complete = True
    break

if user_input.lower() == "reset":
    await chat.reset()
    print("[Conversation has been reset]")
    continue

# Try to grab files from the script's current directory
if user_input.startswith("@") and len(user_input) > 1:
    file_name = user_input[1:]
    script_dir = os.path.dirname(os.path.abspath(__file__))
    file_path = os.path.join(script_dir, file_name)
    try:
        if not os.path.exists(file_path):
            print(f"Unable to access file: {file_path}")
            continue
        with open(file_path, "r", encoding="utf-8") as file:
            user_input = file.read()
    except Exception:
        print(f"Unable to access file: {file_path}")
        continue

# Add the current user_input to the chat
await chat.add_chat_message(message=user_input)
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

To initiate the `Agent` collaboration in response to user input and display the `Agent` responses, invoke the `AgentGroupChat`; however, first be sure to reset the _Completion_ state from any prior invocation.

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
try:
    async for response in chat.invoke():
        if response is None or not response.name:
            continue
        print()
        print(f"# {response.name.upper()}:\n{response.content}")
except Exception as e:
    print(f"Error during chat invocation: {e}")

# Reset the chat's complete flag for the new conversation round.
chat.is_complete = False
```
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Final

::: zone pivot="programming-language-csharp"

Bringing all the steps together, we have the final code for this example. The complete implementation is provided below.

Try using these suggested inputs:

1. Hi
2. {"message: "hello world"}
3. {"message": "hello world"}
4. Semantic Kernel (SK) is an open-source SDK that enables developers to build and orchestrate complex AI workflows that involve natural language processing (NLP) and machine learning models. It provides a flexible platform for integrating AI capabilities such as semantic search, text summarization, and dialogue systems into applications. With SK, you can easily combine different AI services and models, define their relationships, and orchestrate interactions between them.
5. make this two paragraphs
6. thank you
7. @.\WomensSuffrage.txt
8. its good, but is it ready for my college professor? 

```csharp
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
                    Your responsibility is to review and identify how to improve user provided content.
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
                    Your sole responsibility is to rewrite content according to review suggestions.

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
                Console.WriteLine("[Conversation has been reset]");
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

Bringing all the steps together, we now have the final code for this example. The complete implementation is shown below.  

You can try using one of the suggested inputs. As the agent chat begins, the agents will exchange messages for several iterations until the reviewer agent is satisfied with the copywriter's work. The `while` loop ensures the conversation continues, even if the chat is initially considered complete, by resetting the `is_complete` flag to `False`.

1. Rozes are red, violetz are blue.
2. Semantic Kernel (SK) is an open-source SDK that enables developers to build and orchestrate complex AI workflows that involve natural language processing (NLP) and machine learning models. It provides a flexible platform for integrating AI capabilities such as semantic search, text summarization, and dialogue systems into applications. With SK, you can easily combine different AI services and models, define their relationships, and orchestrate interactions between them.
4. Make this two paragraphs
5. thank you
7. @WomensSuffrage.txt
8. It's good, but is it ready for my college professor? 

> [!TIP]  
> You can reference any file by providing `@<file_path_to_file>`. To reference the "WomensSuffrage" text from above, download it [here](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/learn_resources/resources/WomensSuffrage.txt) and place it in your current working directory. You can then reference it with `@WomensSuffrage.txt`.

```python
import asyncio
import os

from semantic_kernel import Kernel
from semantic_kernel.agents import AgentGroupChat, ChatCompletionAgent
from semantic_kernel.agents.strategies import (
    KernelFunctionSelectionStrategy,
    KernelFunctionTerminationStrategy,
)
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.contents import ChatHistoryTruncationReducer
from semantic_kernel.functions import KernelFunctionFromPrompt

"""
The following sample demonstrates how to create a simple,
agent group chat that utilizes a Reviewer Chat Completion
Agent along with a Writer Chat Completion Agent to
complete a user's task.
"""

# Define agent names
REVIEWER_NAME = "Reviewer"
WRITER_NAME = "Writer"


def create_kernel() -> Kernel:
    """Creates a Kernel instance with an Azure OpenAI ChatCompletion service."""
    kernel = Kernel()
    kernel.add_service(service=AzureChatCompletion())
    return kernel


async def main():
    # Create a single kernel instance for all agents.
    kernel = create_kernel()

    # Create ChatCompletionAgents using the same kernel.
    agent_reviewer = ChatCompletionAgent(
        kernel=kernel,
        name=REVIEWER_NAME,
        instructions="""
Your responsibility is to review and identify how to improve user provided content.
If the user has provided input or direction for content already provided, specify how to address this input.
Never directly perform the correction or provide an example.
Once the content has been updated in a subsequent response, review it again until it is satisfactory.

RULES:
- Only identify suggestions that are specific and actionable.
- Verify previous suggestions have been addressed.
- Never repeat previous suggestions.
""",
    )

    agent_writer = ChatCompletionAgent(
        kernel=kernel,
        name=WRITER_NAME,
        instructions="""
Your sole responsibility is to rewrite content according to review suggestions.
- Always apply all review directions.
- Always revise the content in its entirety without explanation.
- Never address the user.
""",
    )

    # Define a selection function to determine which agent should take the next turn.
    selection_function = KernelFunctionFromPrompt(
        function_name="selection",
        prompt=f"""
Examine the provided RESPONSE and choose the next participant.
State only the name of the chosen participant without explanation.
Never choose the participant named in the RESPONSE.

Choose only from these participants:
- {REVIEWER_NAME}
- {WRITER_NAME}

Rules:
- If RESPONSE is user input, it is {REVIEWER_NAME}'s turn.
- If RESPONSE is by {REVIEWER_NAME}, it is {WRITER_NAME}'s turn.
- If RESPONSE is by {WRITER_NAME}, it is {REVIEWER_NAME}'s turn.

RESPONSE:
{{{{$lastmessage}}}}
""",
    )

    # Define a termination function where the reviewer signals completion with "yes".
    termination_keyword = "yes"

    termination_function = KernelFunctionFromPrompt(
        function_name="termination",
        prompt=f"""
Examine the RESPONSE and determine whether the content has been deemed satisfactory.
If the content is satisfactory, respond with a single word without explanation: {termination_keyword}.
If specific suggestions are being provided, it is not satisfactory.
If no correction is suggested, it is satisfactory.

RESPONSE:
{{{{$lastmessage}}}}
""",
    )

    history_reducer = ChatHistoryTruncationReducer(target_count=5)

    # Create the AgentGroupChat with selection and termination strategies.
    chat = AgentGroupChat(
        agents=[agent_reviewer, agent_writer],
        selection_strategy=KernelFunctionSelectionStrategy(
            initial_agent=agent_reviewer,
            function=selection_function,
            kernel=kernel,
            result_parser=lambda result: str(result.value[0]).strip() if result.value[0] is not None else WRITER_NAME,
            history_variable_name="lastmessage",
            history_reducer=history_reducer,
        ),
        termination_strategy=KernelFunctionTerminationStrategy(
            agents=[agent_reviewer],
            function=termination_function,
            kernel=kernel,
            result_parser=lambda result: termination_keyword in str(result.value[0]).lower(),
            history_variable_name="lastmessage",
            maximum_iterations=10,
            history_reducer=history_reducer,
        ),
    )

    print(
        "Ready! Type your input, or 'exit' to quit, 'reset' to restart the conversation. "
        "You may pass in a file path using @<path_to_file>."
    )

    is_complete = False
    while not is_complete:
        print()
        user_input = input("User > ").strip()
        if not user_input:
            continue

        if user_input.lower() == "exit":
            is_complete = True
            break

        if user_input.lower() == "reset":
            await chat.reset()
            print("[Conversation has been reset]")
            continue

        # Try to grab files from the script's current directory
        if user_input.startswith("@") and len(user_input) > 1:
            file_name = user_input[1:]
            script_dir = os.path.dirname(os.path.abspath(__file__))
            file_path = os.path.join(script_dir, file_name)
            try:
                if not os.path.exists(file_path):
                    print(f"Unable to access file: {file_path}")
                    continue
                with open(file_path, "r", encoding="utf-8") as file:
                    user_input = file.read()
            except Exception:
                print(f"Unable to access file: {file_path}")
                continue

        # Add the current user_input to the chat
        await chat.add_chat_message(message=user_input)

        try:
            async for response in chat.invoke():
                if response is None or not response.name:
                    continue
                print()
                print(f"# {response.name.upper()}:\n{response.content}")
        except Exception as e:
            print(f"Error during chat invocation: {e}")

        # Reset the chat's complete flag for the new conversation round.
        chat.is_complete = False


if __name__ == "__main__":
    asyncio.run(main())
```

You may find the full [code](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/learn_resources/agent_docs/agent_collaboration.py), as shown above, in our repo.
::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end