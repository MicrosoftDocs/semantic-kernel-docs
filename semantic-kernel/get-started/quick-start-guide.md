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


## Create a simple turn-based conversation

Now that you have the SDK installed in your preferred language, you can create a simple conversation with an AI agent. Using Semantic Kernel always starts with following these steps:

1. [Import the necessary packages](#1-import-the-necessary-packages)
2. [Create a kernel with your AI services](#2-create-a-kernel-with-your-ai-services)
3. [Create a chat history object to store the conversation](#3-create-a-chat-history-object-to-store-messages)
4. [Collect the user's input and add it to the chat history](#4-collect-the-users-input-and-add-to-the-history)
5. [Pass the chat history to the AI services to generate a response](#5-as-the-ai-to-generate-a-response)
6. [Print the response and add it to the chat history](#6-print-the-response-and-add-it-back-to-the-history)

Below is an example of how you can implement these steps.

> [!Note]
> In this example, we'll use Azure OpenAI, but you can use any other chat completion service. To see the full list of supported services, refer to the [supported languages article](./supported-languages.md).

### 1) Import the necessary packages
For this package, you'll want to import the following packages at the top of your file:

::: zone pivot="programming-language-csharp"
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="3-6":::
::: zone-end

### 2) Create a kernel with your AI services

> [!Tip]
> If you need help creating a service, refer to the [AI services article](../concepts/ai-services.md). There, you'll find guidance on how to use OpenAI or Azure OpenAI models as services.

::: zone pivot="programming-language-csharp"
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="35,37-38,40,47-49":::
::: zone-end

### 3) Create a chat history object to store messages

> [!Tip]
> For more information on the chat history object, refer to the [chat history section](../concepts/ai-services/chat-completion.md#creating-a-chat-history-object) in the chat completion service article.

::: zone pivot="programming-language-csharp"
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="45-46":::
::: zone-end

### 4) Collect the user's input and add to the history

::: zone pivot="programming-language-csharp"
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="51-57,76-79" highlight="7":::
::: zone-end

### 5) Ask the AI service to generate a response

Once you've prepared the chat history object, you can then pass it to the AI services to generate a response. The final message from the AI is returned back as a result so you can use it later. The following code demonstrates how to generate a [non-streaming response](../concepts/ai-services/chat-completion.md#non-streaming-completion), but you can also generate a [streaming response](../concepts/ai-services/chat-completion.md#streaming-completion) by using the `GetStreamingChatMessageContentAsync` method.

::: zone pivot="programming-language-csharp"
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="65-67,69":::
::: zone-end

### 6) Print the response and add it back to the history

Finally, you'll take the results of the LLM model and print it to the screen. To support multi-turn scenarios, you'll also want to add it to the chat history object so the AI can refer to it in future responses.

::: zone pivot="programming-language-csharp"
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="71-75":::
::: zone-end

### Final code for a simple turn-based conversation

Once you've implemented these steps, you're final code should look like the following:
    
::: zone pivot="programming-language-csharp"
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="35, 37,38,40, 44-57,64-67,69-79":::
::: zone-end

## Allowing your AI to invoke your native code

In the previous section, you saw how to create a simple conversation with an AI agent. The challenge with this code, however, is that the AI agent can only respond with information baked into its model. With [plugins](../concepts/plugins.md), however, you can give your AI agent the ability to run your code to retrieve information from external sources or to perform actions.

> [!Tip]
> Behind the scenes, Semantic Kernel leverages [function calling](https://platform.openai.com/docs/guides/function-calling), a native feature of most of the latest LLMs, to provide [planning](../concepts/planners.md). With function calling, LLMs can request (or call) a particular function to satisfy a user's request. Semantic Kernel then marshals the request to the appropriate function in your codebase and returns the results back to the LLM so the LLM can generate a final response.

To allow our AI to call our native code, we'll complete the following:
7. [Create a plugin with the code you want the AI to invoke](#7-create-a-plugin)
8. [Add the plugin to the kernel so the AI can access it](#8-add-the-plugin-to-the-kernel)
9. [Invoke the AI with the plugin](#9-invoke-the-ai-with-your-plugin)

### 7) Create a plugin

Below, you can see that creating a native plugin is as simple as creating a new class.

In this example, we've created a plugin that can manipulate a light bulb. While this is a simple example, this plugin quickly demonstrates how you can support Retrieval Augmented Generation (RAG) by providing the AI with the state of the light bulb and task automation by allowing the AI to turn the light bulb on or off. In your own code, you can create a plugin that interacts with any external service or API to achieve similar results.

::: zone pivot="programming-language-csharp"
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="85-88,90-92,94-107":::
::: zone-end

### 8) Add the plugin to the kernel

Once you've created your plugin, you can add it to the kernel so the AI can access it. We can alter the `Kernel` object to include the plugin within its plugin collection by adding an additional line of code to the builder.

::: zone pivot="programming-language-csharp"
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="35,37-40" highlight="4":::
::: zone-end

### 9) Invoke the AI with your plugin

Finally, we can alter the request to the LLM so that it automatically uses the plugin to satisfy the user's request.

::: zone pivot="programming-language-csharp"
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="59-69" highlight="10":::
::: zone-end

### Final code for a turn-based conversation with a plugin

Once you've implemented these steps, you're final code should look like the following:

::: zone pivot="programming-language-csharp"
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="35, 37,38,40, 44-79":::
::: zone-end

The following back-and-forth chat should be similar to what you see in the console. The function calls have been added below to demonstrate how the AI leverages the plugin behind the scenes.

| Role                          | Message                       |
| ----------------------------- | ----------------------------- |
| 🔵 **User**                      | Please toggle the light       |
| 🔴 **Assistant (function call)** | LightPlugin.GetState          |
| 🟢 **Tool**                      | off                           |
| 🔴 **Assistant (function call)** | LightPlugin.ChangeState(true) |
| 🟢 **Tool**                      | on                            |
| 🔴 **Assistant**                 | The light is now on           |