---
title: How to quickly start with Semantic Kernel
description: Follow along with Semantic Kernel's guides to quickly learn how to use the SDK
zone_pivot_groups: programming-languages
author: matthewbolanos
ms.topic: quickstart
ms.author: mabolan
ms.date: 07/11/2023
ms.service: semantic-kernel
---
# Getting started with Semantic Kernel


In just a few steps, you can build your first AI agent with Semantic Kernel in either Python, .NET, or Java. This guid will show you how to...
- Install the necessary packages
- Create a back-and-forth conversation with an AI
- Give an AI agent the ability to run your code
- Watch the AI create plans on the fly

## Installing the SDK

::: zone pivot="programming-language-csharp"

Semantic Kernel has several NuGet packages available. For most scenarios, however, you typically only need `Microsoft.SemanticKernel`.

You can install it using the following command:

```bash
dotnet add package Microsoft.SemanticKernel
```

For the full list of Nuget packages, please refer to the [supported languages article](./supported-languages.md).

::: zone-end  


:::zone pivot="programming-language-python"

Instructions for accessing the `SemanticKernel` Python package is available [here](https://pypi.org/project/semantic-kernel/). It's as easy as:

```PyPI
pip install semantic-kernel
```

::: zone-end

::: zone pivot="programming-language-java"

The `SemanticKernel` bom can be found on [maven](https://repo1.maven.org/maven2/com/microsoft/semantic-kernel/semantickernel-bom/). Using the package is as easy as adding the following to your _pom.xml_ file:

```xml
<dependencyManagement>
    <dependencies>
        <dependency>
            <groupId>com.microsoft.semantic-kernel</groupId>
            <artifactId>semantickernel-bom</artifactId>
            <version>${semantickernel.version}</version>
            <scope>import</scope>
            <type>pom</type>
        </dependency>
    </dependencies>
</dependencyManagement>

<dependency>
    <groupId>com.microsoft.semantic-kernel</groupId>
    <artifactId>semantickernel-api</artifactId>
</dependency>
```
::: zone-end


::: zone pivot="programming-language-python,programming-language-csharp"
## Quickly get started with notebooks
If you're a Python or C# developer, you can quickly get started with our notebooks. These notebooks provide step-by-step guides on how to use Semantic Kernel to build AI agents.

![Semantic Kernel notebooks](../media/notebook-image.png)

::: zone-end

::: zone pivot="programming-language-python"
To get started, follow these steps:
1. Clone the [Semantic Kernel repo](https://github.com/microsoft/semantic-kernel)
2. Open the repo in Visual Studio Code
3. Navigate to [_/python/samples/getting_started](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/getting_started)
4. Open _00-getting-started.ipynb_ to get started setting your environment and creating your first AI agent!
::: zone-end

::: zone pivot="programming-language-python"
To get started, follow these steps:
1. Clone the [Semantic Kernel repo](https://github.com/microsoft/semantic-kernel)
2. Open the repo in Visual Studio Code
3. Navigate to [_/dotnet/notebooks](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/notebooks)
4. Open _00-getting-started.ipynb_ to get started setting your environment and creating your first AI agent!
::: zone-end

## Writing your first console app
To make it easier to get started building enterprise apps with Semantic Kernel, we've created a series of "maps" that guide you through the process of creating a kernel and using it to interact with AI services. These maps are available in Python, .NET, and Java.

![Semantic Kernel map](../media/map.png)

We recommend printing out these maps on at least 11x17 paper so you can easily follow along with the steps, not _all_ you need to following to build your first AI agent. In the following sections, we'll walk through steps **1**, **2**, **3**, **4**, **6**, **9**, and **10**. Everything you need to build a simple agent that is powered by an AI service and can run your code.

1. [Import packages](#1-import-packages)
2. [Add AI services](#2-add-ai-services)
3. [Enterprise components](#3-add-enterprise-services)
4. [Build the kernel](#4-build-the-kernel)
5. Add memory (skipped)
6. [Add plugins](#6-add-plugins)
7. Create kernel arguments (skipped)
8. Create prompts (skipped)
9. [Planning](#9-planning)
10. [Invoke](#10-invoke)

Once we show you how to implement these steps, we then show you how to [put it all together](#putting-it-all-together) to create a simple turn-based conversation.

### 1) Import packages
For this package, you'll want to import the following packages at the top of your file:

::: zone pivot="programming-language-csharp"

```csharp
using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
```

::: zone-end

### 2) Add AI services
The most important part of creating a kernel is adding the AI services that you want to use. In this example, we'll add an Azure OpenAI chat completion service to a kernel builder.


> [!Note]
> In this example, we'll use Azure OpenAI, but you can use any other chat completion service. To see the full list of supported services, refer to the [supported languages article](./supported-languages.md). If you need help creating a different service, refer to the [AI services article](../concepts/ai-services.md). There, you'll find guidance on how to use OpenAI or Azure OpenAI models as services.

::: zone pivot="programming-language-csharp"
```csharp
// Create kernel
var builder = Kernel.CreateBuilder()
builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
```
::: zone-end

### 3) Add enterprise services
One of the main benefits of using Semantic Kernel is that it supports enterprise-grade services. For example, you can add logging services to your kernel to help you debug your AI agent.

```csharp
builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace)); 
```

### 4) Build the kernel
Once you've added the services you need, you can build the kernel.

```csharp
Kernel kernel = builder.Build();

// Retrieve the chat completion service
var chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();
```

### 6) Add plugins
With plugins, can give your AI agent the ability to run your code to retrieve information from external sources or to perform actions.

Before adding a plugin, however, you first need to _create_ a plugin.

#### Create a native plugin

Below, you can see that creating a native plugin is as simple as creating a new class.

In this example, we've created a plugin that can manipulate a light bulb. While this is a simple example, this plugin quickly demonstrates how you can support both...

1. [Retrieval Augmented Generation (RAG)](../concepts/plugins/using-data-retrieval-functions-for-rag.md) by providing the AI agent with the state of the light bulb
2. And [task automation](../concepts/plugins/using-task-automation-functions.md) by allowing the AI agent to turn the light bulb on or off.

In your own code, you can create a plugin that interacts with any external service or API to achieve similar results.

::: zone pivot="programming-language-csharp"
```csharp
public class LightsPlugin
{
   // Mock data for the lights
   private readonly List<LightModel> lights = new()
   {
      new LightModel { Id = 1, Name = "Table Lamp", IsOn = false },
      new LightModel { Id = 2, Name = "Porch light", IsOn = false },
      new LightModel { Id = 3, Name = "Chandelier", IsOn = true }
   };

   [KernelFunction("get_lights")]
   [Description("Gets a list of lights and their current state")]
   [return: Description("An array of lights")]
   public async Task<List<LightModel>> GetLightsAsync()
   {
      return lights
   }

   [KernelFunction("change_state")]
   [Description("Changes the state of the light")]
   [return: Description("The updated state of the light; will return null if the light does not exist")]
   public async Task<LightModel?> ChangeStateAsync(int id, bool isOn)
   {
      var light = lights.FirstOrDefault(light => light.Id == id);

      if (light == null)
      {
         return null;
      }

      // Update the light with the new state
      light.IsOn = isOn;

      return light;
   }
}

public class LightModel
{
   [JsonPropertyName("id")]
   public int Id { get; set; }

   [JsonPropertyName("name")]
   public string Name { get; set; }

   [JsonPropertyName("is_on")]
   public bool? IsOn { get; set; }
}
```
::: zone-end

#### Add the plugin to the kernel

Once you've created your plugin, you can add it to the kernel so the AI agent can access it. We can alter the `Kernel` object to include the plugin within its plugin collection by adding an additional line of code to the builder.

::: zone pivot="programming-language-csharp"
```csharp
// Add the plugin to the kernel
kernel.Plugins.AddFromType<LightsPlugin>();
```
::: zone-end

### 9) Planning

Semantic Kernel leverages [function calling](../concepts/ai-services/chat-completion/function-calling.md), a native feature of most of the latest LLMs, to provide [planning](../concepts/planners.md). With function calling, LLMs can request (or call) a particular function to satisfy a user's request. Semantic Kernel then marshals the request to the appropriate function in your codebase and returns the results back to the LLM so the AI agent can generate a final response.

To enable automatic function calling, you'll first need to create the appropriate execution settings.

::: zone pivot="programming-language-csharp"
```csharp
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};
```
::: zone-end

### 10) Invoke

Finally, you can invoke the AI agent with the plugin. The following code demonstrates how to generate a [non-streaming response](../concepts/ai-services/chat-completion.md#non-streaming-completion), but you can also generate a [streaming response](../concepts/ai-services/chat-completion.md#streaming-completion) by using the `GetStreamingChatMessageContentAsync` method.

::: zone pivot="programming-language-csharp"
```csharp
// Create chat history
var history = new ChatHistory();

// Get the response from the AI
var result = await chatCompletionService.GetChatMessageContentAsync(
    history,
    executionSettings: openAIPromptExecutionSettings,
    kernel: kernel
);
```
::: zone-end  

### Final code for a simple turn-based conversation

Once you've implemented these steps, you're final code should look like the following:
    
::: zone pivot="programming-language-csharp"
```csharp
// 1. Import packages
using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// 2. Create kernel with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

// 3. Add enterprise components
builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

// 4. Build the kernel
Kernel kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// 6. Add plugins
kernel.Plugins.AddFromType<LightsPlugin>();

// 9. Planning
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

// 10. Invoke
var history = new ChatHistory();

string? userInput;
do {
    // Collect user input
    Console.Write("User > ");
    userInput = Console.ReadLine();

    // Add user input
    history.AddUserMessage(userInput);

    // Get the response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Print the results
    Console.WriteLine("Assistant > " + result);

    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not null)
```
::: zone-end

The following back-and-forth chat should be similar to what you see in the console. The function calls have been added below to demonstrate how the AI leverages the plugin behind the scenes.

| Role                                             | Message                       |
| ------------------------------------------------ | ----------------------------- |
| 🔵&nbsp;**User**                                | Please toggle the light       |
| 🔴&nbsp;**Assistant&nbsp;(function&nbsp;call)** | `LightsPlugin.GetState()`          |
| 🟢&nbsp;**Tool**                                | `off`                           |
| 🔴&nbsp;**Assistant&nbsp;(function&nbsp;call)** | `LightsPlugin.ChangeState(true)` |
| 🟢&nbsp;**Tool**                                | `on`                            |
| 🔴&nbsp;**Assistant**                           | The light is now on           |