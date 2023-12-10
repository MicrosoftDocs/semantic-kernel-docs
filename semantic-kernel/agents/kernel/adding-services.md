---
title: Adding AI services to Semantic Kernel
description: Create a new Semantic Kernel project and initialize the kernel.
author: matthewbolanos
ms.topic: tutorial
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Adding AI services to Semantic Kernel



One of the main features of Semantic Kernel is its ability to add different AI services to the kernel. This allows you to easily swap out different AI services to compare their performance and to leverage the best model for your needs. In this article, we will provide sample code for adding different AI services to the kernel.

If you want to see any of these samples in a complete solution, you can check them out in the public documentation repository.

| Language  | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/02-Adding-AI-Services) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/02-Adding-AI-Services) |

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

### Text generation deployments

# [C#](#tab/Csharp)
To add an Azure OpenAI text generation service to your Semantic Kernel project, you will need to use the [WithAzureTextCompletionService](/dotnet/api/microsoft.semantickernel.openaikernelbuilderextensions.withazuretextcompletionservice) method.

:::code language="csharp" source="~/../samples/dotnet/02-Adding-AI-Services/Program.cs" range="46-54" highlight="2":::
# [Python](#tab/python)
To add an Azure OpenAI text generation service to your Semantic Kernel project, you will need to use the `AzureTextCompletion` class.

:::code language="python" source="~/../samples/python/02-Adding-AI-Services/main.py" range="30-38" highlight="4":::

---

### Chat completion deployments

# [C#](#tab/Csharp)
To add an Azure OpenAI chat completion service to your Semantic Kernel project, you will need to use the [WithAzureChatCompletionService](/dotnet/api/microsoft.semantickernel.openaikernelbuilderextensions.withazurechatcompletionservice) method.

:::code language="csharp" source="~/../samples/dotnet/02-Adding-AI-Services/Program.cs" range="21-29" highlight="2":::
# [Python](#tab/python)
To add an Azure OpenAI chat completion service to your Semantic Kernel project, you will need to use the `AzureChatCompletion` class.

:::code language="python" source="~/../samples/python/02-Adding-AI-Services/main.py" range="20-28" highlight="4":::

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

### Text generation models

# [C#](#tab/Csharp)
To add an OpenAI text generation service to your Semantic Kernel project, you will need to use the [WithOpenAITextCompletionService](/dotnet/api/microsoft.semantickernel.openaikernelbuilderextensions.withopenaitextcompletionservice) method.

:::code language="csharp" source="~/../samples/dotnet/02-Adding-AI-Services/Program.cs" range="98-105" highlight="2":::
# [Python](#tab/python)
To add an OpenAI text generation service to your Semantic Kernel project, you will need to use the `OpenAITextCompletion` class.

:::code language="python" source="~/../samples/python/02-Adding-AI-Services/main.py" range="53-61" highlight="4":::

---

### Chat completion models

# [C#](#tab/Csharp)
To add an OpenAI text generation service to your Semantic Kernel project, you will need to use the [WithOpenAIChatCompletionService](/dotnet/api/microsoft.semantickernel.openaikernelbuilderextensions.withopenaichatcompletionservice) method.

:::code language="csharp" source="~/../samples/dotnet/02-Adding-AI-Services/Program.cs" range="75-82" highlight="2":::
# [Python](#tab/python)
To add an OpenAI text generation service to your Semantic Kernel project, you will need to use the `OpenAIChatCompletion` class.

:::code language="python" source="~/../samples/python/02-Adding-AI-Services/main.py" range="43-51" highlight="4":::

---

## Next steps
Now that you know how to add different AI services to your Semantic Kernel project, you can learn now to add telemetry and logging to the kernel.

> [!div class="nextstepaction"]
> [Add telemetry and logging](https://devblogs.microsoft.com/semantic-kernel/unlock-the-power-of-telemetry-in-semantic-kernel-sdk)