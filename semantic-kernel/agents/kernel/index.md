---
title: Understanding the kernel in Semantic Kernel
description: Learn about the central component of Semantic Kernel and how it works.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Understanding the kernel in Semantic Kernel

Similar to operating system, the kernel is responsible for managing resources that are necessary to run "code" in an AI application. This includes managing the AI models, services, and plugins that are necessary for both native code and AI services to run together.

If you want to see the code demonstrated in this article in a complete solution, check out the following samples in the public documentation repository.

| Language  | Link to final solution |
| --- | --- |
| C# | [Open example in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/DocumentationExamples/Plugin.cs) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/01-Kernel-Intro) |

## The kernel is at the center of _everything_
Because the kernel has all of the services and plugins necessary to run both native code and AI services, it is used by nearly every component within the Semantic Kernel SDK. This means that if you run any prompt or code in Semantic Kernel, it will always go through a kernel.

This is extremely powerful, because it means you as a developer have a single place where you can configure, and most importantly monitor, your AI application. Take for example, when you invoke a prompt from the kernel. When you do so, the kernel will...
1. Select the best AI service to run the prompt.
2. Build the prompt using the provided prompt template.
3. Send the prompt to the AI service.
4. Receive and parse the response.
5. Before finally returning the response to your application.

Throughout this entire process, you can create events and middleware that are triggered at each of these steps. This means you can perform actions like logging, provide status updates to users, and most importantly responsible AI. All from a single place.

![The kernel is at the center of everything in Semantic Kernel](../../media/the-kernel-is-at-the-center-of-everything.png)

## Building a kernel
Before building a kernel, you should first understand the two types of components that exist within a kernel: services and plugins. Services consist of both AI services and other services that are necessary to run your application (e.g., logging, telemetry, etc.). Plugins, meanwhile, are _any_ code you want AI to call or leverage within a prompt.

In the following examples, you can see how to add a logger, chat completion service, and plugin to the kernel.

# [C#](#tab/Csharp)
With C#, Semantic Kernel natively supports dependency injection. This means you can add a kernel to your application's dependency injection container and use any of your application's services within the kernel by adding them as a service to the kernel.

Import the necessary packages:
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/UsingTheKernel.cs" id="NecessaryPackages":::

If you are using a Azure OpenAI, you can use the `AddAzureOpenAIChatCompletion` method.

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/UsingTheKernel.cs" id="KernelCreation":::

If you are using OpenAI, you can use the `AddOpenAIChatCompletionService` method instead.

# [Python](#tab/python)
Import the necessary packages:
:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="5-7,9,11":::

Create a kernel.

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/using_the_kernel.py" range="14" :::

If you are integrating Azure OpenAI services into your application, you should select the appropriate class based on the model you are utilizing:

**For Chat Models with Azure**: If you are utilizing the Chat model, employ the `AzureChatCompletion` class. This class is specifically designed for interacting with chat-based models, offering a structured way to manage chat sessions.

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="39-47" highlight="2":::

**For Completion Models with Azure**: If you are working with the Completion model, use the `AzureTextCompletion` class. This class is tailored for generating text completions, ideal for a variety of applications ranging from content creation to code generation.

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="48-56" highlight="2":::

Similarly, when utilizing OpenAI's API:

**For Chat Models with OpenAI**: When using chat models such as gpt-4, gpt-4-turbo-preview, or gpt-3.5-turbo, the `OpenAIChatCompletion` class should be your go-to. This class facilitates interaction with the AI in a conversational format, supporting nuanced dialogues and responses.

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="57-67" highlight="2":::

**For Completion Models with OpenAI**: For working with completion models like gpt-3.5-turbo-instruct, babbage-002, or davinci-002, opt for the `OpenAITextCompletion` class. This class is adept at generating text based on a prompt, suitable for diverse applications including automated responses and content generation.

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/service_configurator.py" range="68-76" highlight="2":::

For further understanding of the distinctions between Chat Completions and Completions models, please refer to: [Chat Completions vs. Completions](https://platform.openai.com/docs/guides/text-generation/completions-api).

---

## Invoking plugins from the kernel
Semantic Kernel makes it easy to run prompts alongside native code because they are both expressed as `KernelFunction`s. This means you can invoke them in exactly same way.

To run `KernelFunction`s, Semantic Kernel provides the `InvokeAsync` method. Simply pass in the function you want to run, its arguments, and the kernel will handle the rest.

# [C#](#tab/Csharp)
Run the `UtcNow` function from `TimePlugin`:
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/UsingTheKernel.cs" id="InvokeUtcNow":::

Run the `ShortPoem` function from `WriterPlugin` while using the current time as an argument:
:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/UsingTheKernel.cs" id="InvokeShortPoem":::

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

---


## Going further with the kernel
For more details on how to configure and leverage these properties, please refer to the following articles:

| Article | Description |
|---------|-------------|
| [Adding AI services](./adding-services.md) | Learn how to add additional AI services from OpenAI, Azure OpenAI, Hugging Face, and more to the kernel. |
| [Adding telemetry and logs](https://devblogs.microsoft.com/semantic-kernel/unlock-the-power-of-telemetry-in-semantic-kernel-sdk/) | Gain visibility into what Semantic Kernel is doing by adding telemetry to the kernel. |

## Next steps
Once you're done configuring the kernel, you can learn how to create prompts to run AI services from the kernel.

> [!div class="nextstepaction"]
> [Learn about plugins](../plugins/index.md)