---
title: Add AI services to Semantic Kernel
description: Learn how to bring multiple AI services to your Semantic Kernel project.
zone_pivot_groups: programming-languages
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Adding AI services to Semantic Kernel

One of the main features of Semantic Kernel is its ability to add different AI services to the kernel. This allows you to easily swap out different AI services to compare their performance and to leverage the best model for your needs. In this article, we will provide sample code for adding different AI services to the kernel.

::: zone pivot="programming-language-csharp"
Within Semantic Kernel, there are interfaces for the most popular AI tasks. These include:
- [Chat completion](#chat-completion)
- [Text generation](#text-generation)
- [Text-to-image](#text-to-image) (Experimental)
- [Image-to-text](#image-to-text) (Experimental)
- [Text-to-audio](#text-to-audio) (Experimental)
- [Audio-to-text](#audio-to-text) (Experimental)
- [Embedding generation](#embedding-generation) (Experimental)

::: zone-end

::: zone pivot="programming-language-python"
Within Semantic Kernel, there are interfaces for the most popular AI tasks. These include:
- [Chat completion](#chat-completion)
- [Text generation](#text-generation)
- [Embedding generation](#embedding-generation) (Experimental)

::: zone-end

::: zone pivot="programming-language-java"
Within Semantic Kernel, there are interfaces for the most popular AI tasks. These include:
- [Chat completion](#chat-completion)
- [Text generation](#text-generation)

::: zone-end

In most scenarios, you will only need to add chat completion to your kernel, but to support multi-modal AI, you can add any of the above services to your kernel.

The rest of this article will describe what each interface does and how to add them from different AI providers.

## Chat completion
With chat completion, you can simulate a back-and-forth conversation with an AI. This is of course useful for creating chat bots, but it can also be used for automating business processes, creating code snippets, and more. As the primary model type provided by OpenAI, Google, Mistral, Facebook, and others, chat completion is the most common AI service that you will add to your Semantic Kernel project.

When picking out a chat completion model, you will need to consider the following:
- What modalities does the model support (e.g., text, image, audio, etc.)?
- Does it support function calling?
- How fast does it receive and generate tokens?
- How much does each token cost?

> [!IMPORTANT]
> Of all the above questions, the most important is whether the model supports function calling. If it does not, you will not be able to use the model to call your existing code. Most of the latest models from OpenAI, Google, Mistral, and Amazon all support function calling. Support from small language models, however, is still limited.

::: zone pivot="programming-language-csharp"
To add a chat completion service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

```csharp
using Microsoft.SemanticKernel;

IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddAzureOpenAIChatCompletion(
    deploymentName: "NAME_OF_YOUR_DEPLOYMENT",
    apiKey: "YOUR_API_KEY",
    endpoint: "YOUR_AZURE_ENDPOINT",
    modelId: "gpt-4", // Optional name of the underlying model if the deployment name doesn't match the model name
    serviceId: "YOUR_SERVICE_ID", // Optional; for targeting specific services within Semantic Kernel
    httpClient: new HttpClient() // Optional; if not provided, the HttpClient from the kernel will be used
);
Kernel kernel = kernelBuilder.Build();
```

# [OpenAI](#tab/OpenAI)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

```csharp
using Microsoft.SemanticKernel;

IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
    modelId: "gpt-4",
    apiKey: "YOUR_API_KEY",
    orgId: "YOUR_ORG_ID", // Optional
    serviceId: "YOUR_SERVICE_ID", // Optional; for targeting specific services within Semantic Kernel
    httpClient: new HttpClient() // Optional; if not provided, the HttpClient from the kernel will be used
);
Kernel kernel = kernelBuilder.Build();
```

# [Mistral](#tab/Mistral)

> [!IMPORTANT]
> The Mistral chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.MistralAI
```

```csharp
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0070
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddMistralChatCompletion(
    modelId: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    endpoint: new Uri("YOUR_ENDPOINT"), // Optional
    serviceId: "SERVICE_ID", // Optional; for targeting specific services within Semantic Kernel
    httpClient: new HttpClient() // Optional; for customizing HTTP client
);
Kernel kernel = kernelBuilder.Build();
```

# [Google](#tab/Google)

> [!IMPORTANT]
> The Google chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.Google
```

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

#pragma warning disable SKEXP0070
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddGoogleAIGeminiChatCompletion(
    modelId: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    apiVersion: GoogleAIVersion.V1, // Optional
    serviceId: "SERVICE_ID", // Optional; for targeting specific services within Semantic Kernel
    httpClient: new HttpClient() // Optional; for customizing HTTP client
);
Kernel kernel = kernelBuilder.Build();
```

# [Hugging Face](#tab/HuggingFace)

> [!IMPORTANT]
> The Hugging Face chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.HuggingFace
```

```csharp
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0070
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddHuggingFaceChatCompletion(
    model: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    endpoint: new Uri("YOUR_ENDPOINT"), // Optional
    serviceId: "SERVICE_ID", // Optional; for targeting specific services within Semantic Kernel
    httpClient: new HttpClient() // Optional; for customizing HTTP client
);
Kernel kernel = kernelBuilder.Build();
```

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

> [!IMPORTANT]
> Using custom endpoints with the OpenAI connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0010`.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

```csharp
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0010
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
    modelId: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    endpoint: new Uri("YOUR_ENDPOINT"), // Used to point to your service
    serviceId: "SERVICE_ID", // Optional; for targeting specific services within Semantic Kernel
    httpClient: new HttpClient() // Optional; for customizing HTTP client
);
Kernel kernel = kernelBuilder.Build();
```

---

If you're using dependency injection, you'll likely want to add your AI services directly to the service provider. This is helpful if you want to create singletons of your AI services and reuse them in transient kernels.

# [Azure OpenAI](#tab/AzureOpenAI)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

```csharp
using Microsoft.SemanticKernel;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddAzureOpenAIChatCompletion(
    deploymentName: "NAME_OF_YOUR_DEPLOYMENT",
    apiKey: "YOUR_API_KEY",
    endpoint: "YOUR_AZURE_ENDPOINT",
    modelId: "gpt-4", // Optional name of the underlying model if the deployment name doesn't match the model name
    serviceId: "YOUR_SERVICE_ID" // Optional; for targeting specific services within Semantic Kernel
);

builder.Services.AddTransient((serviceProvider)=> {
    return new Kernel(serviceProvider);
});
```

# [OpenAI](#tab/OpenAI)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

```csharp
using Microsoft.SemanticKernel;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOpenAIChatCompletion(
    modelId: "gpt-4",
    apiKey: "YOUR_API_KEY",
    orgId: "YOUR_ORG_ID", // Optional; for OpenAI deployment
    serviceId: "YOUR_SERVICE_ID" // Optional; for targeting specific services within Semantic Kernel
);

builder.Services.AddTransient((serviceProvider)=> {
    return new Kernel(serviceProvider);
});
```

# [Mistral](#tab/Mistral)

> [!IMPORTANT]
> The Mistral chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.MistralAI
```

```csharp
using Microsoft.SemanticKernel;

var builder = Host.CreateApplicationBuilder(args);

#pragma warning disable SKEXP0070
builder.Services.AddMistralChatCompletion(
    modelId: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    endpoint: new Uri("YOUR_ENDPOINT"), // Optional
    serviceId: "SERVICE_ID" // Optional; for targeting specific services within Semantic Kernel
);

builder.Services.AddTransient((serviceProvider)=> {
    return new Kernel(serviceProvider);
});
```

# [Google](#tab/Google)

> [!IMPORTANT]
> The Google chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.Google
```

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

var builder = Host.CreateApplicationBuilder(args);

#pragma warning disable SKEXP0070
builder.Services.AddGoogleAIGeminiChatCompletion(
    modelId: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    apiVersion: GoogleAIVersion.V1, // Optional
    serviceId: "SERVICE_ID" // Optional; for targeting specific services within Semantic Kernel
);

builder.Services.AddTransient((serviceProvider)=> {
    return new Kernel(serviceProvider);
});
```

# [Hugging Face](#tab/HuggingFace)

> [!IMPORTANT]
> The Hugging Face chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.HuggingFace
```

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

var builder = Host.CreateApplicationBuilder(args);

#pragma warning disable SKEXP0070
builder.Services.AddHuggingFaceChatCompletion(
    model: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    endpoint: new Uri("YOUR_ENDPOINT"), // Optional
    serviceId: "SERVICE_ID" // Optional; for targeting specific services within Semantic Kernel
);

builder.Services.AddTransient((serviceProvider)=> {
    return new Kernel(serviceProvider);
});
```

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

> [!IMPORTANT]
> Using custom endpoints with the OpenAI connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0010`.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

```csharp
using Microsoft.SemanticKernel;

var builder = Host.CreateApplicationBuilder(args);

#pragma warning disable SKEXP0010
builder.Services.AddOpenAIChatCompletion(
    modelId: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    endpoint: new Uri("YOUR_ENDPOINT"), // Used to point to your service
    serviceId: "SERVICE_ID", // Optional; for targeting specific services within Semantic Kernel
    httpClient: new HttpClient() // Optional; for customizing HTTP client
);

builder.Services.AddTransient((serviceProvider)=> {
    return new Kernel(serviceProvider);
});
```

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without ever injecting them into the kernel or in a service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

```csharp
using Microsoft.SemanticKernel.Connectors.OpenAI;

AzureOpenAIChatCompletionService chatCompletionService = new (
    deploymentName: "NAME_OF_YOUR_DEPLOYMENT",
    apiKey: "YOUR_API_KEY",
    endpoint: "YOUR_AZURE_ENDPOINT",
    modelId: "gpt-4", // Optional name of the underlying model if the deployment name doesn't match the model name
    httpClient: new HttpClient() // Optional; if not provided, the HttpClient from the kernel will be used
);
```

# [OpenAI](#tab/OpenAI)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

```csharp
using Microsoft.SemanticKernel.Connectors.OpenAI;

OpenAIChatCompletionService chatCompletionService = new (
    modelId: "gpt-4",
    apiKey: "YOUR_API_KEY",
    organization: "YOUR_ORG_ID", // Optional
    httpClient: new HttpClient() // Optional; if not provided, the HttpClient from the kernel will be used
);
```

# [Mistral](#tab/Mistral)

> [!IMPORTANT]
> The Mistral chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.MistralAI
```

```csharp
using Microsoft.SemanticKernel.Connectors.MistralAI;

#pragma warning disable SKEXP0070
MistralAIChatCompletionService chatCompletionService = new (
    modelId: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    endpoint: new Uri("YOUR_ENDPOINT"), // Optional
    httpClient: new HttpClient() // Optional; for customizing HTTP client
);
```

# [Google](#tab/Google)

> [!IMPORTANT]
> The Google chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.Google
```

```csharp
using Microsoft.SemanticKernel.Connectors.Google;

#pragma warning disable SKEXP0070
GoogleAIGeminiChatCompletionService chatCompletionService = new (
    modelId: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    apiVersion: GoogleAIVersion.V1, // Optional
    httpClient: new HttpClient() // Optional; for customizing HTTP client
);
```

# [Hugging Face](#tab/HuggingFace)

> [!IMPORTANT]
> The Hugging Face chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.HuggingFace
```

```csharp
using Microsoft.SemanticKernel.Connectors.OpenAI;

OpenAIChatCompletionService chatCompletionService = new (
    modelId: "gpt-4",
    apiKey: "YOUR_API_KEY",
    organization: "YOUR_ORG_ID", // Optional
    httpClient: new HttpClient() // Optional; if not provided, the HttpClient from the kernel will be used
);
```

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

> [!IMPORTANT]
> Using custom endpoints with the OpenAI connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0010`.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

```csharp
using Microsoft.SemanticKernel.Connectors.OpenAI;

#pragma warning disable SKEXP0010
OpenAIChatCompletionService chatCompletionService = new (
    modelId: "gpt-4",
    apiKey: "YOUR_API_KEY",
    organization: "YOUR_ORG_ID", // Optional
    endpoint: new Uri("YOUR_ENDPOINT"), // Used to point to your service
    httpClient: new HttpClient() // Optional; if not provided, the HttpClient from the kernel will be used
);
```

---

::: zone-end

::: zone pivot="programming-language-python"
To add a chat completion service, you can use the following code to add it to the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.


---

You can also create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

---

::: zone-end

::: zone pivot="programming-language-java"

To add a chat completion service, you can use the following code to add it to the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

::: zone-end

## Text generation

Text generation is the process of predicting the next set of words from an initial sentence.

> [!TIP]
> In _most_ cases, you will likely use a chat completion models instead of a text generation model because they're better suited to generating discrete responses. If you have autocomplete scenarios, however, text generation models can be useful.

::: zone pivot="programming-language-csharp"
To add a text generation service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Mistral](#tab/Mistral)

# [Google](#tab/Mistral)

# [Hugging Face](#tab/HuggingFace)

# [Ollama](#tab/Ollama)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Mistral](#tab/Mistral)

# [Google](#tab/Mistral)

# [Hugging Face](#tab/HuggingFace)

# [Ollama](#tab/Ollama)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Mistral](#tab/Mistral)

# [Google](#tab/Mistral)

# [Hugging Face](#tab/HuggingFace)

# [Ollama](#tab/Ollama)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

---

::: zone-end

::: zone pivot="programming-language-python"
To add a chat completion service, you can use the following code to add it to the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

---

You can also create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

---

::: zone-end

::: zone pivot="programming-language-java"

To add a chat completion service, you can use the following code to add it to the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

::: zone-end

::: zone pivot="programming-language-csharp"
## Text-to-image

Text-to-image is the process of generating an image from a text prompt. This is useful for generating images for chat bots, creating images for reports, and more. Today's chat completion models currently do not support text-to-image. To recreate the experience in ChatGPT, you can wrap a text-to-image model in a plugin so that the chat completion model can call it.


To add a text-to-image service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

::: zone-end

::: zone pivot="programming-language-csharp"
## Image-to-text

Image-to-text is the process of generating text from an image. This is useful for extracting text from images, creating captions for images, and more. Some chat completion models support images as input, so you can use them to generate text from images. Often, however, using a dedicated image-to-text can provide cheaper and more targeted results.

Text-to-image is the process of generating an image from a text prompt. This is useful for generating images for chat bots, creating images for reports, and more. Today's chat completion models currently do not support text-to-image. To recreate the experience in ChatGPT, you can wrap a text-to-image model in a plugin so that the chat completion model can call it.

To add an image-to-text service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

::: zone-end

::: zone pivot="programming-language-csharp"
## Text-to-audio

Text-to-audio is the process of generating audio from a text prompt. Today's chat completion models currently do not support text-to-audio. To recreate the experience in ChatGPT, you can post-process any of the messages generated by the model into audio.

> [!NOTE] There are several different types of text-to-audio models, including those that generate speech, music, and more. In future versions of Semantic Kernel, we plan on making this interface more specific to the type of audio you want to generate to make it easier to request the right model through dependency injection.


To add a text-to-audio service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

::: zone-end

::: zone pivot="programming-language-csharp"

## Audio-to-text

Audio-to-text is the process of generating text from an audio prompt. Today's chat completion models currently do not support audio-to-text. To recreate the experience in ChatGPT, you can generate text from a user's audio input using a dedicated audio-to-text model and then pass that text to the chat completion model.

> [!NOTE] There are several different types of audio-to-text models, including those that transcribe speech, music, and more. In future versions of Semantic Kernel, we plan on making this interface more specific to the type of audio you want to transcribe to make it easier to request the right model through dependency injection.

To add an audio-to-text service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

::: zone-end

::: zone pivot="programming-language-csharp,programming-language-python"

## Embedding generation

Embedding generation is the process of generating a vector representation of a text prompt. This is useful for retrieving text from memory stores.

> [!NOTE] As part of the enhancements coming to Semantic Kernel's memory connectors, we plan on making the embedding interface more generic to support additional embedding models.

::: zone-end

::: zone pivot="programming-language-csharp"

To add an embedding generation service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

::: zone-end

::: zone pivot="programming-language-python"

::: zone-end

## Next steps
Now that you know how to add different AI services to your Semantic Kernel project, you can learn now to add plugins to your project so that you can start automating tasks with AI.

> [!div class="nextstepaction"]
> [Learn about plugins in Semantic Kernel](./plugins.md)

