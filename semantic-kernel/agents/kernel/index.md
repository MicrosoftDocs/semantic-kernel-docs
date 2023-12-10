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
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/01-Kernel-Intro) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/01-Kernel-Intro) |

## The kernel is at center of _everything_
Because the kernel has all of the services and plugins necessary to run both native code and AI services, it is at the center of everything in Semantic Kernel. This means that if run any prompt or code in Semantic Kernel, it will always go through a kernel.

This is extremely powerful, because it means you as a developer have a single place where you can configure, and most importantly monitor, your AI application. Take for example, when you invoke a prompt from the kernel. When you do so, the kernel will 1) select the best AI service, 2) build your prompt template, 3) send the prompt to the AI service, 4) receive the response, and 5) return the response to you.

![The kernel is at the center of everything in Semantic Kernel](../media/the-kernel-is-at-the-center-of-everything.png)

Throughout this entire process, you can create events and middleware that are triggered at each of these steps. This means you can perform actions like logging, provide status updates to users, and most importantly responsible AI. All from a single place.

## Building a kernel
Before building a kernel, you should first understand the two types of components that exist within a kernel: services and plugins. Services consist of both AI services and other services that are necessary to run your application (e.g., logging, telemetry, etc.). Plugins, meanwhile, are _any_ code you want AI to call or leverage within a prompt.

### Configuring the kernel
Depending on your language of choice, you can configure the kernel in different ways. For example, in C#, you can use the `KernelBuilder` class to create a kernel using dependency injection. Whereas with Python, you can iteratively add properties to the `Kernel` object directly.

In the following examples, you can see how to add a chat completion service, logger, and plugin to the kernel.

# [C#](#tab/Csharp)
With C#, Semantic Kernel natively supports dependency injection. This means you can add a kernel to your application's dependency injection container and use any of your application's services within the kernel by adding them as a service to the kernel.

Import the necessary packages:
:::code language="csharp" source="~/../samples/dotnet/01-Kernel-Intro/Program.cs" range="1-5":::

If you are using a Azure OpenAI, you can use the `AddAzureOpenAIChatCompletion` method.

:::code language="csharp" source="~/../samples/dotnet/01-Kernel-Intro/Program.cs" range="21-32" highlight="3":::

If you are using OpenAI, you can use the `AddOpenAIChatCompletionService` method.

:::code language="csharp" source="~/../samples/dotnet/01-Kernel-Intro/Program.cs" range="49-59" highlight="3":::

# [Python](#tab/python)
If you are using a Azure OpenAI, you can use the `AzureChatCompletion` class.

:::code language="csharp" source="~/../samples/python/01-Kernel-Intro/main.py" range="24-32" highlight="4":::

If you are using OpenAI, you can use the `OpenAIChatCompletion` class.

:::code language="csharp" source="~/../samples/python/01-Kernel-Intro/main.py" range="34-42" highlight="4":::

---

## Invoking native code and prompts from the kernel
Semantic Kernel makes it easy to run prompts alongside native code because they are both expressed as `KernelFunction` objects. This means you can invoke them in  exactly same way.

To run `KernelFunction` objects, Semantic Kernel provides the `InvokeAsync` method. Simply pass in the function you want to run, its arguments, and the kernel will handle the rest.

# [C#](#tab/Csharp)
Run the `GetCurrentUtcTime` function from `TimePlugin`:
:::code language="csharp" source="~/../samples/dotnet/01-Kernel-Intro/Program.cs" range="35":::

Run the `ShortPoem` function from `WriterPlugin` while using the current time as an argument:
:::code language="csharp" source="~/../samples/dotnet/01-Kernel-Intro/Program.cs" range="38":::


# [Python](#tab/python)
Import the necessary packages:
:::code language="python" source="~/../samples/python/01-Kernel-Intro/main.py" range="2-3":::

Run the today function from the time plugin:
:::code language="python" source="~/../samples/python/01-Kernel-Intro/main.py" range="12-17" highlight="4":::

---


### Going further with the kernel
For more details on how to configure and leverage these properties, please refer to the following articles:

| Article | Description |
|---------|-------------|
| [Adding AI services](./adding-services.md) | Learn how to add additional AI services from OpenAI, Azure OpenAI, Hugging Face, and more to the kernel. |
| [Adding telemetry and logs](https://devblogs.microsoft.com/semantic-kernel/unlock-the-power-of-telemetry-in-semantic-kernel-sdk/) | Gain visibility into what Semantic Kernel is doing by adding telemetry to the kernel. |

## Next steps
Once you're done configuring the kernel, you can learn how to create prompts to run AI services from the kernel.

> [!div class="nextstepaction"]
> [Learn how to create prompt templates](../prompts/index.md)