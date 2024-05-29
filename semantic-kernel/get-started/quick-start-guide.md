---
title: How to quickly start with Semantic Kernel
description: Follow along with Semantic Kernel's guides to quickly learn how to use the SDK.
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

# [C#](#tab/Csharp)

Semantic Kernel has several NuGet packages available. For most scenarios, however, you typically only need `Microsoft.SemanticKernel`. This includes the base abstractions in `Microsoft.SemanticKernel.Abstractions` as well as the connectors for Azure OpenAI and Azure OpenAI.

You can install it using the following command:

```bash
dotnet add package Microsoft.SemanticKernel
```

For the full list of Nuget packages, please refer to the [supported languages article](./supported-languages.md).

# [Python](#tab/python)

Instructions for accessing the `SemanticKernel` Python package is available [here](https://pypi.org/project/semantic-kernel/). It's as easy as:

```PyPI
pip install semantic-kernel
```

# [Java](#tab/Java)

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

---


## Create a simple turn-based conversation

Now that you have the SDK installed in your preferred language, you can create a simple conversation with an AI agent. Using Semantic Kernel always starts with following these steps:

1. Create a kernel with your AI services.
2. Create a chat history object to store the conversation.
3. Collect the user's input and add it to the chat history.
4. Pass the chat history to the AI services to generate a response.
5. Print the response and add it to the chat history.

Below is an example of how you can implement these steps.


# [C#](#tab/Csharp)

1. Create a kernel with your AI services. In this example, we'll use Azure OpenAI, but you can use any other chat completion service. To see the full list of supported services, refer to the [supported languages article](./supported-languages.md).

    :::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="35,37-38,40,47-49":::

2. Create a chat history object to store the conversation.

    :::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="45-46":::

3. Collect the user's input and add it to the chat history.

    :::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="51-57,76-79" highlight="7":::

4. Pass the chat history to the AI services to generate a response.

    :::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="65-67,69":::

5. Print the response and add it to the chat history.

    :::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="71-75":::

# [Python](#tab/python)



# [Java](#tab/Java)


---

> [!Note]
> For a simple scenario like the one above, use of the `Kernel` is not entirely necessary, but in the next section, you'll see how encapsulating your services and plugins in a single `Kernel` object can make your code more modular and easier to maintain.


## Giving your agent the ability to invoke your native code

In the previous section, you saw how to create a simple conversation with an AI agent. The challenge with this code, however, is that the AI agent can only respond with information baked into its model. With plugins, however, you can give your AI agent the ability to run your code to retrieve additional information from external sources and perform actions.

To do this, we'll first need to create a plugin with the code you want the AI to invoke. Afterwards, we'll add the plugin to the kernel so the AI can access it.

# [C#](#tab/Csharp)

1. Create a plugin with the code you want the AI to invoke.

    :::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/Plugin.cs" range="1-33":::