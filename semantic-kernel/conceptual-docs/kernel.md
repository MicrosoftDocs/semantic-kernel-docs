---
title: Understanding the kernel in Semantic Kernel
description: Learn about the central component of Semantic Kernel and how it works
zone_pivot_groups: programming-languages
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Understanding the kernel

The kernel is the central component of Semantic Kernel. At its simplest, the kernel is a Dependency Injection container that manages all of the services and plugins necessary to run your AI application. If you provide all of your services and plugins to the kernel, they will then be seamlessly used by the AI as needed.

## The kernel is at the center of _everything_
Because the kernel has all of the services and plugins necessary to run both native code and AI services, it is used by nearly every component within the Semantic Kernel SDK. This means that if you run any prompt or code in Semantic Kernel, it will always go through a kernel.

This is extremely powerful, because it means you as a developer have a single place where you can configure, and most importantly monitor, your AI application. Take for example, when you invoke a prompt from the kernel. When you do so, the kernel will...
1. Select the best AI service to run the prompt.
2. Build the prompt using the provided prompt template.
3. Send the prompt to the AI service.
4. Receive and parse the response.
5. Before finally returning the response to your application.

Throughout this entire process, you can create events and middleware that are triggered at each of these steps. This means you can perform actions like logging, provide status updates to users, and most importantly responsible AI. All from a single place.

![The kernel is at the center of everything in Semantic Kernel](../media/the-kernel-is-at-the-center-of-everything.png)

## Building a kernel
Before building a kernel, you should first understand the two types of components that exist within a kernel: services and plugins. Services consist of both AI services and other services that are necessary to run your application (e.g., logging, telemetry, etc.). Plugins, meanwhile, are _any_ code you want AI to call or leverage within a prompt.


::: zone pivot="programming-language-csharp"
In the following examples, you can see how to add a logger, chat completion service, and plugin to the kernel.

Import the necessary packages:
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/UsingTheKernel.cs" id="NecessaryPackages":::

If you are using a Azure OpenAI, you can use the `AddAzureOpenAIChatCompletion` method.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/UsingTheKernel.cs" id="KernelCreation":::

If you are using OpenAI, you can use the `AddOpenAIChatCompletionService` method instead.

# [Python](#tab/python)
Import the necessary packages:
:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="5-7,9,11":::

Create a kernel.

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/using_the_kernel.py" range="14" :::

If you are using a Azure OpenAI, you can use the `AzureChatCompletion` class.

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="39-46" highlight="2":::

If you are using OpenAI, you can use the `OpenAIChatCompletion` class.

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="48-55" highlight="2":::

# [Java](#tab/Java)

To build a kernel that uses OpenAI for chat completion, it can be created as follows.

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/UsingTheKernel.java" id="build_client":::

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/UsingTheKernel.java" id="build_kernel":::

---

## Invoking plugins from the kernel
Semantic Kernel makes it easy to run prompts alongside native code because they are both expressed as `KernelFunction`s. This means you can invoke them in exactly same way.

To run `KernelFunction`s, Semantic Kernel provides the `InvokeAsync` method. Simply pass in the function you want to run, its arguments, and the kernel will handle the rest.

# [C#](#tab/Csharp)
Run the `UtcNow` function from `TimePlugin`:
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/UsingTheKernel.cs" id="InvokeUtcNow":::

Run the `ShortPoem` function from `WriterPlugin` while using the current time as an argument:
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/UsingTheKernel.cs" id="InvokeShortPoem":::

This should return a response similar to the following (except specific to your current time):
```
There once was a sun in the sky
That shone so bright, it caught the eye
But on December tenth
It decided to vent
And took a break, said "Bye bye!"
```


# [Python](#tab/python)
Import the necessary packages:
:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/using_the_kernel.py" range="9":::

Run the today function from the time plugin:
:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/using_the_kernel.py" range="20-22,31-33" highlight="5":::

Run the `ShortPoem` function from `WriterPlugin` while using the current time as an argument:
:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/using_the_kernel.py" range="23-30, 35-37" highlight="10":::

# [Java](#tab/Java)

Run the native plugin `MathPlugin`

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/UsingTheKernel.java" id="run_math":::

Run the poem plugin defined by a prompt `PoemPlugin`

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/UsingTheKernel.java" id="run_poem":::

---


## Going further with the kernel
For more details on how to configure and leverage these properties, please refer to the following articles:

| Article | Description |
|---------|-------------|
| [Adding AI services](./adding-services.md) | Learn how to add additional AI services from OpenAI, Azure OpenAI, Hugging Face, and more to the kernel. |
| [Adding telemetry and logs](https://devblogs.microsoft.com/semantic-kernel/unlock-the-power-of-telemetry-in-semantic-kernel-sdk/) | Gain visibility into what Semantic Kernel is doing by adding telemetry to the kernel. |

## Next steps
Once you're done configuring the kernel, you can learn how to create prompts to run AI services from the kernel.

# Adding AI services to Semantic Kernel

One of the main features of Semantic Kernel is its ability to add different AI services to the kernel. This allows you to easily swap out different AI services to compare their performance and to leverage the best model for your needs. In this article, we will provide sample code for adding different AI services to the kernel.

If you want to see any of these samples in a complete solution, you can check them out in the public documentation repository.

| Language  | Link to final solution |
| --- | --- |
| C# | [Open example in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/DocumentationExamples/AIServices.cs) |
| Java | [Open example in GitHub](https://github.com/microsoft/semantic-kernel/blob/java-v1/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/AIServices.java) |
| Python | [Open solution in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/documentation_examples/service_configurator.py) |

## Azure OpenAI
With Azure OpenAI, you can deploy most of the OpenAI models to the cloud and use them in your Semantic Kernel project. Depending on the model that you want to use, however, you will either need to add the model as a text generation service or as a chat completion service.

The following table shows which service you should use for each model.

| Model type      | Model            |
| --------------- | ---------------- |
| Text generation | text-ada-001     |
| Text generation | text-babbage-001 |
| Text generation | text-curie-001   |
| Text generation | text-davinci-001 |
| Text generation | text-davinci-002 |
| Text generation | text-davinci-003 |
| Chat Completion | gpt-3.5-turbo    |
| Chat Completion | gpt-4            |

### Chat completion deployments

# [C#](#tab/Csharp)
To add an Azure OpenAI chat completion service to your Semantic Kernel project, you will need to use the AddAzureChatCompletionService method.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/AIServices.cs" id="TypicalKernelCreation":::

# [Java](#tab/Java)
To add an Azure OpenAI chat completion service to your Semantic Kernel project, you will need to create a `ChatCompletionService`.

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/AIServices.java" id="CreateChatCompletionService":::

Then use the `withAIService` method of the `Kernel.Builder` to add the service to the kernel.

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/AIServices.java" id="CreateKernel":::

# [Python](#tab/python)
To add an Azure OpenAI chat completion service to your Semantic Kernel project, you will need to use the `AzureChatCompletion` class.

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="48-55" highlight="2":::

---

### Text generation deployments

# [C#](#tab/Csharp)
To add an Azure OpenAI text generation service to your Semantic Kernel project, you will need to use the AddAzureTextCompletionService method.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/AIServices.cs" id="TextCompletionKernelCreation":::

# [Java](#tab/Java)
 
To add an Azure OpenAI text generation service to your Semantic Kernel project, you will need to create a `TextGenerationService`.

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/AIServices.java" id="CreateTextGenerationService":::

Then use the `withAIService` method of the `Kernel.Builder` to add the service to the kernel.

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/AIServices.java" id="CreateKernel":::

# [Python](#tab/python)
To add an Azure OpenAI text generation service to your Semantic Kernel project, you will need to use the `AzureTextCompletion` class.

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="57-64" highlight="2":::

---

## OpenAI
Similar to Azure OpenAI, depending on the model that you want to use from OpenAI, you will either need to add the model as a text generation service or as a chat completion service.

The following table shows which service you should use for each model.

| Model type      | Model            |
| --------------- | ---------------- |
| Text generation | text-ada-001     |
| Text generation | text-babbage-001 |
| Text generation | text-curie-001   |
| Text generation | text-davinci-001 |
| Text generation | text-davinci-002 |
| Text generation | text-davinci-003 |
| Chat Completion | gpt-3.5-turbo    |
| Chat Completion | gpt-4            |

### Chat completion models

# [C#](#tab/Csharp)
To add an OpenAI text generation service to your Semantic Kernel project, you will need to use the AddOpenAIChatCompletionService method.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/AIServices.cs" id="OpenAIKernelCreation":::

# [Java](#tab/Java)
To add an Azure OpenAI chat completion service to your Semantic Kernel project, you will need to create a `ChatCompletionService`.

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/AIServices.java" id="CreateChatCompletionService":::

Then use the `withAIService` method of the `Kernel.Builder` to add the service to the kernel.

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/AIServices.java" id="CreateKernel":::

# [Python](#tab/python)
To add an OpenAI text generation service to your Semantic Kernel project, you will need to use the `OpenAIChatCompletion` class.

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="77-84" highlight="2":::

---

### Text generation models

# [C#](#tab/Csharp)
To add an OpenAI text generation service to your Semantic Kernel project, you will need to use the AddOpenAITextCompletionService method.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/AIServices.cs" id="OpenAITextCompletionKernelCreation":::

# [Java](#tab/Java)
 
To add an Azure OpenAI text generation service to your Semantic Kernel project, you will need to create a `TextGenerationService`.

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/AIServices.java" id="CreateTextGenerationService":::

Then use the `withAIService` method of the `Kernel.Builder` to add the service to the kernel.

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/AIServices.java" id="CreateKernel":::

# [Python](#tab/python)
To add an OpenAI text generation service to your Semantic Kernel project, you will need to use the `OpenAITextCompletion` class.

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="86-93" highlight="2":::

---

## Next steps
Now that you know how to add different AI services to your Semantic Kernel project, you can learn now to add telemetry and logging to the kernel.

