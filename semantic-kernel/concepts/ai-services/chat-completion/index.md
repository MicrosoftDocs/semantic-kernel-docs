---
title: Add chat completion services to Semantic Kernel
description: Learn how to add gpt-4, Mistral, Google, and other chat completion services to your Semantic Kernel project.
zone_pivot_groups: programming-languages
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---


# Chat completion

With chat completion, you can simulate a back-and-forth conversation with an AI agent. This is of course useful for creating chat bots, but it can also be used for creating autonomous agents that can complete business processes, generate code, and more. As the primary model type provided by OpenAI, Google, Mistral, Facebook, and others, chat completion is the most common AI service that you will add to your Semantic Kernel project.

When picking out a chat completion model, you will need to consider the following:

- What modalities does the model support (e.g., text, image, audio, etc.)?
- Does it support function calling?
- How fast does it receive and generate tokens?
- How much does each token cost?

> [!IMPORTANT]
> Of all the above questions, the most important is whether the model supports function calling. If it does not, you will not be able to use the model to call your existing code. Most of the latest models from OpenAI, Google, Mistral, and Amazon all support function calling. Support from small language models, however, is still limited.

::: zone pivot="programming-language-csharp"

## Setting up your local environment

Some of the AI Services can be hosted locally and may require some setup. Below are instructions for those that support this.

# [Azure OpenAI](#tab/csharp-AzureOpenAI)

No local setup.

# [OpenAI](#tab/csharp-OpenAI)

No local setup.

# [Mistral](#tab/csharp-Mistral)

No local setup.

# [Google](#tab/csharp-Google)

No local setup.

# [Hugging Face](#tab/csharp-HuggingFace)

No local setup.

# [Azure AI Inference](#tab/csharp-AzureAIInference)

No local setup.

# [Ollama](#tab/csharp-Ollama)

To run Ollama locally using docker, use the following command to start a container using the CPU.

```bash
docker run -d -v "c:\temp\ollama:/root/.ollama" -p 11434:11434 --name ollama ollama/ollama
```

To run Ollama locally using docker, use the following command to start a container using GPUs.

```bash
docker run -d --gpus=all -v "c:\temp\ollama:/root/.ollama" -p 11434:11434 --name ollama ollama/ollama
```

After the container has started, launch a Terminal window for the docker container, e.g. if using
docker desktop, choose `Open in Terminal` from actions.

From this terminal download the required models, e.g. here we are downloading the phi3 model.

```bash
ollama pull phi3
```

# [Anthropic](#tab/csharp-Anthropic)

No local setup.

# [Amazon Bedrock](#tab/csharp-AmazonBedrock)

No local setup.

# [ONNX](#tab/csharp-ONNX)

Clone the repository containing the ONNX model you would like to use.

```bash
git clone https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx
```

# [Other](#tab/csharp-other)

No local setup.

---

## Installing the necessary packages

Before adding chat completion to your kernel, you will need to install the necessary packages. Below are the packages you will need to install for each AI service provider.

# [Azure OpenAI](#tab/csharp-AzureOpenAI)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.AzureOpenAI
```

# [OpenAI](#tab/csharp-OpenAI)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

# [Mistral](#tab/csharp-Mistral)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.MistralAI --prerelease
```

# [Google](#tab/csharp-Google)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.Google --prerelease
```

# [Hugging Face](#tab/csharp-HuggingFace)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.HuggingFace --prerelease
```

# [Azure AI Inference](#tab/csharp-AzureAIInference)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.AzureAIInference --prerelease
```

# [Ollama](#tab/csharp-Ollama)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.Ollama --prerelease
```

# [Anthropic](#tab/csharp-Anthropic)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.Amazon --prerelease
```

# [Amazon Bedrock](#tab/csharp-AmazonBedrock)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.Amazon --prerelease
```

# [ONNX](#tab/csharp-ONNX)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.Onnx --prerelease
```

# [Other](#tab/csharp-other)

For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the OpenAI chat completion connector.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

---

## Creating chat completion services

Now that you've installed the necessary packages, you can create chat completion services. Below are the several ways you can create chat completion services using Semantic Kernel.

### Adding directly to the kernel

To add a chat completion service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/csharp-AzureOpenAI)

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

# [OpenAI](#tab/csharp-OpenAI)

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

# [Mistral](#tab/csharp-Mistral)

> [!IMPORTANT]
> The Mistral chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

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

# [Google](#tab/csharp-Google)

> [!IMPORTANT]
> The Google chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

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

# [Hugging Face](#tab/csharp-HuggingFace)

> [!IMPORTANT]
> The Hugging Face chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

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

# [Azure AI Inference](#tab/csharp-AzureAIInference)

> [!IMPORTANT]
> The Azure AI Inference chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0070
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddAzureAIInferenceChatCompletion(
    modelId: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    endpoint: new Uri("YOUR_ENDPOINT"), // Optional
    serviceId: "SERVICE_ID", // Optional; for targeting specific services within Semantic Kernel
    httpClient: new HttpClient() // Optional; for customizing HTTP client
);
Kernel kernel = kernelBuilder.Build();
```

# [Ollama](#tab/csharp-Ollama)

> [!IMPORTANT]
> The Ollama chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0070
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOllamaChatCompletion(
    modelId: "NAME_OF_MODEL",           // E.g. "phi3" if phi3 was downloaded as described above.
    endpoint: new Uri("YOUR_ENDPOINT"), // E.g. "http://localhost:11434" if Ollama has been started in docker as described above.
    serviceId: "SERVICE_ID"             // Optional; for targeting specific services within Semantic Kernel
);
Kernel kernel = kernelBuilder.Build();
```

# [Anthropic](#tab/csharp-Anthropic)

> [!IMPORTANT]
> The Bedrock chat completion connector which is required for Anthropic is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0070
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddBedrockChatCompletionService(
    modelId: "NAME_OF_MODEL",
    bedrockRuntime: amazonBedrockRuntime, // Optional; An instance of IAmazonBedrockRuntime, used to communicate with Azure Bedrock.
    serviceId: "SERVICE_ID"               // Optional; for targeting specific services within Semantic Kernel
);
Kernel kernel = kernelBuilder.Build();
```

# [Amazon Bedrock](#tab/csharp-AmazonBedrock)

> [!IMPORTANT]
> The Bedrock chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0070
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddBedrockChatCompletionService(
    modelId: "NAME_OF_MODEL",
    bedrockRuntime: amazonBedrockRuntime, // Optional; An instance of IAmazonBedrockRuntime, used to communicate with Azure Bedrock.
    serviceId: "SERVICE_ID"               // Optional; for targeting specific services within Semantic Kernel
);
Kernel kernel = kernelBuilder.Build();
```

# [ONNX](#tab/csharp-ONNX)

> [!IMPORTANT]
> The ONNX chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0070
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOnnxRuntimeGenAIChatCompletion(
    modelId: "NAME_OF_MODEL",  // E.g. phi-3
    modelPath: "PATH_ON_DISK", // Path to the model on disk e.g. C:\Repos\huggingface\microsoft\Phi-3-mini-4k-instruct-onnx\cpu_and_mobile\cpu-int4-rtn-block-32
    serviceId: "SERVICE_ID",                            // Optional; for targeting specific services within Semantic Kernel
    jsonSerializerOptions: customJsonSerializerOptions  // Optional; for providing custom serialization settings for e.g. function argument / result serialization and parsing.
);
Kernel kernel = kernelBuilder.Build();
```

# [Other](#tab/csharp-other)

For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

> [!IMPORTANT]
> Using custom endpoints with the OpenAI connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0010`.

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

### Using dependency injection

If you're using dependency injection, you'll likely want to add your AI services directly to the service provider. This is helpful if you want to create singletons of your AI services and reuse them in transient kernels.

# [Azure OpenAI](#tab/csharp-AzureOpenAI)

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

# [OpenAI](#tab/csharp-OpenAI)

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

# [Mistral](#tab/csharp-Mistral)

> [!IMPORTANT]
> The Mistral chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

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

# [Google](#tab/csharp-Google)

> [!IMPORTANT]
> The Google chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

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

# [Hugging Face](#tab/csharp-HuggingFace)

> [!IMPORTANT]
> The Hugging Face chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel;

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

# [Azure AI Inference](#tab/csharp-AzureAIInference)

> [!IMPORTANT]
> The Azure AI Inference chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel;

var builder = Host.CreateApplicationBuilder(args);

#pragma warning disable SKEXP0070
builder.Services.AddAzureAIInferenceChatCompletion(
    modelId: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    endpoint: new Uri("YOUR_ENDPOINT"), // Optional
    serviceId: "SERVICE_ID" // Optional; for targeting specific services within Semantic Kernel
);

builder.Services.AddTransient((serviceProvider)=> {
    return new Kernel(serviceProvider);
});
```

# [Ollama](#tab/csharp-Ollama)

> [!IMPORTANT]
> The Ollama chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel;

var builder = Host.CreateApplicationBuilder(args);

#pragma warning disable SKEXP0070
builder.Services.AddOllamaChatCompletion(
    modelId: "NAME_OF_MODEL",           // E.g. "phi3" if phi3 was downloaded as described above.
    endpoint: new Uri("YOUR_ENDPOINT"), // E.g. "http://localhost:11434" if Ollama has been started in docker as described above.
    serviceId: "SERVICE_ID"             // Optional; for targeting specific services within Semantic Kernel
);

builder.Services.AddTransient((serviceProvider)=> {
    return new Kernel(serviceProvider);
});
```

# [Anthropic](#tab/csharp-Anthropic)

> [!IMPORTANT]
> The Bedrock chat completion connector which is required for Anthropic is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel;

var builder = Host.CreateApplicationBuilder(args);

#pragma warning disable SKEXP0070
builder.Services.AddBedrockChatCompletionService(
    modelId: "NAME_OF_MODEL",
    bedrockRuntime: amazonBedrockRuntime, // Optional; An instance of IAmazonBedrockRuntime, used to communicate with Azure Bedrock.
    serviceId: "SERVICE_ID"               // Optional; for targeting specific services within Semantic Kernel
);

builder.Services.AddTransient((serviceProvider)=> {
    return new Kernel(serviceProvider);
});
```

# [Amazon Bedrock](#tab/csharp-AmazonBedrock)

> [!IMPORTANT]
> The Bedrock chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel;

var builder = Host.CreateApplicationBuilder(args);

#pragma warning disable SKEXP0070
builder.Services.AddBedrockChatCompletionService(
    modelId: "NAME_OF_MODEL",
    bedrockRuntime: amazonBedrockRuntime, // Optional; An instance of IAmazonBedrockRuntime, used to communicate with Azure Bedrock.
    serviceId: "SERVICE_ID"               // Optional; for targeting specific services within Semantic Kernel
);

builder.Services.AddTransient((serviceProvider)=> {
    return new Kernel(serviceProvider);
});
```

# [ONNX](#tab/csharp-ONNX)

> [!IMPORTANT]
> The ONNX chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel;

var builder = Host.CreateApplicationBuilder(args);

#pragma warning disable SKEXP0070
builder.Services.AddOnnxRuntimeGenAIChatCompletion(
    modelId: "NAME_OF_MODEL",  // E.g. phi-3
    modelPath: "PATH_ON_DISK", // Path to the model on disk e.g. C:\Repos\huggingface\microsoft\Phi-3-mini-4k-instruct-onnx\cpu_and_mobile\cpu-int4-rtn-block-32
    serviceId: "SERVICE_ID",                            // Optional; for targeting specific services within Semantic Kernel
    jsonSerializerOptions: customJsonSerializerOptions  // Optional; for providing custom serialization settings for e.g. function argument / result serialization and parsing.
);

builder.Services.AddTransient((serviceProvider)=> {
    return new Kernel(serviceProvider);
});
```

# [Other](#tab/csharp-other)

For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

> [!IMPORTANT]
> Using custom endpoints with the OpenAI connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0010`.

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

### Creating standalone instances

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without ever injecting them into the kernel or in a service provider.

# [Azure OpenAI](#tab/csharp-AzureOpenAI)

```csharp
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

AzureOpenAIChatCompletionService chatCompletionService = new (
    deploymentName: "NAME_OF_YOUR_DEPLOYMENT",
    apiKey: "YOUR_API_KEY",
    endpoint: "YOUR_AZURE_ENDPOINT",
    modelId: "gpt-4", // Optional name of the underlying model if the deployment name doesn't match the model name
    httpClient: new HttpClient() // Optional; if not provided, the HttpClient from the kernel will be used
);
```

# [OpenAI](#tab/csharp-OpenAI)

```csharp
using Microsoft.SemanticKernel.Connectors.OpenAI;

OpenAIChatCompletionService chatCompletionService = new (
    modelId: "gpt-4",
    apiKey: "YOUR_API_KEY",
    organization: "YOUR_ORG_ID", // Optional
    httpClient: new HttpClient() // Optional; if not provided, the HttpClient from the kernel will be used
);
```

# [Mistral](#tab/csharp-Mistral)

> [!IMPORTANT]
> The Mistral chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

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

# [Google](#tab/csharp-Google)

> [!IMPORTANT]
> The Google chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

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

# [Hugging Face](#tab/csharp-HuggingFace)

> [!IMPORTANT]
> The Hugging Face chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel.Connectors.HuggingFace;

#pragma warning disable SKEXP0070
HuggingFaceChatCompletionService chatCompletionService = new (
    model: "NAME_OF_MODEL",
    apiKey: "API_KEY",
    endpoint: new Uri("YOUR_ENDPOINT") // Optional
);
```

# [Azure AI Inference](#tab/csharp-AzureAIInference)

> [!IMPORTANT]
> The Azure AI Inference chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel.Connectors.AzureAIInference;

#pragma warning disable SKEXP0070
AzureAIInferenceChatCompletionService chatCompletionService = new (
    modelId: "YOUR_MODEL_ID",
    apiKey: "YOUR_API_KEY",
    endpoint: new Uri("YOUR_ENDPOINT"), // Used to point to your service
    httpClient: new HttpClient() // Optional; if not provided, the HttpClient from the kernel will be used
);
```

# [Ollama](#tab/csharp-Ollama)

> [!IMPORTANT]
> The Ollama chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;

#pragma warning disable SKEXP0070
using var ollamaClient = new OllamaApiClient(
    uriString: "YOUR_ENDPOINT"    // E.g. "http://localhost:11434" if Ollama has been started in docker as described above.
    defaultModel: "NAME_OF_MODEL" // E.g. "phi3" if phi3 was downloaded as described above.
);

IChatCompletionService chatCompletionService = ollamaClient.AsChatCompletionService();
```

# [Anthropic](#tab/csharp-Anthropic)

> [!IMPORTANT]
> The Bedrock chat completion connector which is required for Anthropic is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel.Connectors.Amazon;

#pragma warning disable SKEXP0070
BedrockChatCompletionService chatCompletionService = new BedrockChatCompletionService(
    modelId: "NAME_OF_MODEL",
    bedrockRuntime: amazonBedrockRuntime // Optional; An instance of IAmazonBedrockRuntime, used to communicate with Azure Bedrock.
);
```

# [Amazon Bedrock](#tab/csharp-AmazonBedrock)

> [!IMPORTANT]
> The Bedrock chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel.Connectors.Amazon;

#pragma warning disable SKEXP0070
BedrockChatCompletionService chatCompletionService = new BedrockChatCompletionService(
    modelId: "NAME_OF_MODEL",
    bedrockRuntime: amazonBedrockRuntime // Optional; An instance of IAmazonBedrockRuntime, used to communicate with Azure Bedrock.
);
```

# [ONNX](#tab/csharp-ONNX)

> [!IMPORTANT]
> The ONNX chat completion connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0070`.

```csharp
using Microsoft.SemanticKernel.Connectors.Onnx;

#pragma warning disable SKEXP0070
OnnxRuntimeGenAIChatCompletionService chatCompletionService = new OnnxRuntimeGenAIChatCompletionService(
    modelId: "NAME_OF_MODEL",  // E.g. phi-3
    modelPath: "PATH_ON_DISK", // Path to the model on disk e.g. C:\Repos\huggingface\microsoft\Phi-3-mini-4k-instruct-onnx\cpu_and_mobile\cpu-int4-rtn-block-32
    jsonSerializerOptions: customJsonSerializerOptions  // Optional; for providing custom serialization settings for e.g. function argument / result serialization and parsing.
);
```

# [Other](#tab/csharp-other)

For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

> [!IMPORTANT]
> Using custom endpoints with the OpenAI connector is currently experimental. To use it, you will need to add `#pragma warning disable SKEXP0010`.

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
To create a chat completion service, you need to import the necessary modules and create an instance of the service. Below are the steps to create a chat completion service for each AI service provider.

> [!TIP]
> There are three methods to supply the required information to AI services. You may either provide the information directly through the constructor, set the necessary environment variables, or create a .env file within your project directory containing the environment variables. You can visit this page to find all the required environment variables for each AI service provider: <https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/setup/ALL_SETTINGS.md>

# [Azure OpenAI](#tab/python-AzureOpenAI)

```python
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion

chat_completion_service = AzureChatCompletion(
    deployment_name="my-deployment",  
    api_key="my-api-key",
    endpoint="my-api-endpoint", # Used to point to your service
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)

# You can do the following if you have set the necessary environment variables or created a .env file
chat_completion_service = AzureChatCompletion(service_id="my-service-id")
```

> [!NOTE]
> The `AzureChatCompletion` service also supports [Microsoft Entra](https://learn.microsoft.com/en-us/entra/identity/authentication/overview-authentication) authentication. If you don't provide an API key, the service will attempt to authenticate using the Entra token.

# [OpenAI](#tab/python-OpenAI)

```python
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion

chat_completion_service = OpenAIChatCompletion(
    ai_model_id="my-deployment",
    api_key="my-api-key",
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)

# You can do the following if you have set the necessary environment variables or created a .env file
chat_completion_service = OpenAIChatCompletion(service_id="my-service-id")
```

# [Azure AI Inference](#tab/python-AzureAIInference)

```python
from semantic_kernel.connectors.ai.azure_ai_inference import AzureAIInferenceChatCompletion

chat_completion_service = AzureAIInferenceChatCompletion(
    ai_model_id="my-deployment",
    api_key="my-api-key",
    endpoint="my-api-endpoint", # Used to point to your service
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)

# You can do the following if you have set the necessary environment variables or created a .env file
chat_completion_service = AzureAIInferenceChatCompletion(ai_model_id="my-deployment", service_id="my-service-id")

# You can also use an Azure OpenAI deployment with the Azure AI Inference service
from azure.ai.inference.aio import ChatCompletionsClient
from azure.identity.aio import DefaultAzureCredential

chat_completion_service = AzureAIInferenceChatCompletion(
    ai_model_id="my-deployment",
    client=ChatCompletionsClient(
        endpoint=f"{str(endpoint).strip('/')}/openai/deployments/{deployment_name}",
        credential=DefaultAzureCredential(),
        credential_scopes=["https://cognitiveservices.azure.com/.default"],
    ),
)
```

> [!NOTE]
> The `AzureAIInferenceChatCompletion` service also supports [Microsoft Entra](https://learn.microsoft.com/en-us/entra/identity/authentication/overview-authentication) authentication. If you don't provide an API key, the service will attempt to authenticate using the Entra token.

# [Anthropic](#tab/python-Anthropic)

```python
from semantic_kernel.connectors.ai.anthropic import AnthropicChatCompletion

chat_completion_service = AnthropicChatCompletion(
    chat_model_id="model-id",
    api_key="my-api-key",
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)
```

# [Amazon Bedrock](#tab/python-AmazonBedrock)

```python
from semantic_kernel.connectors.ai.bedrock import BedrockChatCompletion

chat_completion_service = BedrockChatCompletion(
    model_id="model-id",
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)
```

> [!NOTE]
> Amazon Bedrock does not accept an API key. Follow this [guide](https://github.com/microsoft/semantic-kernel/blob/main/python/semantic_kernel/connectors/ai/bedrock/README.md) to configure your environment.

# [Google AI](#tab/python-Google)

```python
from semantic_kernel.connectors.ai.google.google_ai import GoogleAIChatCompletion

chat_completion_service = GoogleAIChatCompletion(
    gemini_model_id="model-id",
    api_key="my-api-key",
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)

```

> [!TIP]
> Users can access Google's Gemini models via Google AI Studio or Google Vertex platform. Follow this [guide](https://github.com/microsoft/semantic-kernel/blob/main/python/semantic_kernel/connectors/ai/google/README.md) to configure your environment.

# [Vertex AI](#tab/python-VertexAI)

```python
from semantic_kernel.connectors.ai.google.vertex_ai import VertexAIChatCompletion

chat_completion_service = VertexAIChatCompletion(
    project_id="my-project-id",
    gemini_model_id="model-id",
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)
```

> [!TIP]
> Users can access Google's Gemini models via Google AI Studio or Google Vertex platform. Follow this [guide](https://github.com/microsoft/semantic-kernel/blob/main/python/semantic_kernel/connectors/ai/google/README.md) to configure your environment.

# [Mistral AI](#tab/python-MistralAI)

```python
from semantic_kernel.connectors.ai.mistral_ai import MistralAIChatCompletion

chat_completion_service = MistralAIChatCompletion(
    ai_model_id="model-id",
    api_key="my-api-key",
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)
```

# [Ollama](#tab/python-Ollama)

```python
from semantic_kernel.connectors.ai.ollama import OllamaChatCompletion

chat_completion_service = OllamaChatCompletion(
    ai_model_id="model-id",
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)
```

> [!TIP]
> Learn more about Ollama and download the necessary software from [here](https://github.com/ollama/ollama).

# [ONNX](#tab/python-ONNX)

```python
from semantic_kernel.connectors.ai.onnx import OnnxGenAIChatCompletion

chat_completion_service = OnnxGenAIChatCompletion(
    template="phi3v",
    ai_model_path="model-path",
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)
```

---

You can start using the completion service right away or add the chat completion service to a kernel. You can use the following code to add a service to the kernel.

```python
from semantic_kernel import Kernel

# Initialize the kernel
kernel = Kernel()

# Add the chat completion service created above to the kernel
kernel.add_service(chat_completion_service)
```

::: zone-end

::: zone pivot="programming-language-java"
You can create instances of the chat completion service directly and either add them to
a kernel or use them directly in your code without injecting them into the kernel. The
following code shows how to create a a chat completion service and add it to the kernel.

# [Azure OpenAI](#tab/java-AzureOpenAI)

```java
import com.azure.ai.openai.OpenAIAsyncClient;
import com.azure.ai.openai.OpenAIClientBuilder;
import com.microsoft.semantickernel.Kernel;
import com.microsoft.semantickernel.services.chatcompletion.ChatCompletionService;

// Create the client
OpenAIAsyncClient client = new OpenAIClientBuilder()
    .credential(azureOpenAIClientCredentials)
    .endpoint(azureOpenAIClientEndpoint)
    .buildAsyncClient();

// Create the chat completion service
ChatCompletionService openAIChatCompletion = OpenAIChatCompletion.builder()
    .withOpenAIAsyncClient(client)
    .withModelId(modelId)
    .build();

// Initialize the kernel
Kernel kernel = Kernel.builder()
    .withAIService(ChatCompletionService.class, openAIChatCompletion)
    .build();
```

# [OpenAI](#tab/java-OpenAI)

```java
import com.azure.ai.openai.OpenAIAsyncClient;
import com.azure.ai.openai.OpenAIClientBuilder;
import com.microsoft.semantickernel.Kernel;
import com.microsoft.semantickernel.services.chatcompletion.ChatCompletionService;

// Create the client
OpenAIAsyncClient client = new OpenAIClientBuilder()
    .credential(openAIClientCredentials)
    .buildAsyncClient();

// Create the chat completion service
ChatCompletionService openAIChatCompletion = OpenAIChatCompletion.builder()
    .withOpenAIAsyncClient(client)
    .withModelId(modelId)
    .build();

// Initialize the kernel
Kernel kernel = Kernel.builder()
    .withAIService(ChatCompletionService.class, openAIChatCompletion)
    .build();
```

---

::: zone-end

## Retrieving chat completion services

Once you've added chat completion services to your kernel, you can retrieve them using the get service method. Below is an example of how you can retrieve a chat completion service from the kernel.

::: zone pivot="programming-language-csharp"

```csharp
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
```

::: zone-end

::: zone pivot="programming-language-python"

```python
from semantic_kernel.connectors.ai.chat_completion_client_base import ChatCompletionClientBase

# Retrieve the chat completion service by type
chat_completion_service = kernel.get_service(type=ChatCompletionClientBase)

# Retrieve the chat completion service by id
chat_completion_service = kernel.get_service(service_id="my-service-id")

# Retrieve the default inference settings
execution_settings = kernel.get_prompt_execution_settings_from_service_id("my-service-id")
```

::: zone-end

::: zone pivot="programming-language-java"

```java
ChatCompletionService chatCompletionService = kernel.getService(ChatCompletionService.class);
```

::: zone-end

> [!TIP]
> Adding the chat completion service to the kernel is not required if you don't need to use other services in the kernel. You can use the chat completion service directly in your code.

## Using chat completion services

Now that you have a chat completion service, you can use it to generate responses from an AI agent. There are two main ways to use a chat completion service:

- **Non-streaming**: You wait for the service to generate an entire response before returning it to the user.
- **Streaming**: Individual chunks of the response are generated and returned to the user as they are created.

::: zone pivot="programming-language-python"

Before getting started, you will need to manually create an execution settings instance to use the chat completion service if you did not register the service with the kernel.

# [Azure OpenAI](#tab/python-AzureOpenAI)

```python
from semantic_kernel.connectors.ai.open_ai import OpenAIChatPromptExecutionSettings

execution_settings = OpenAIChatPromptExecutionSettings()
```

# [OpenAI](#tab/python-OpenAI)

```python
from semantic_kernel.connectors.ai.open_ai import OpenAIChatPromptExecutionSettings

execution_settings = OpenAIChatPromptExecutionSettings()
```

# [Azure AI Inference](#tab/python-AzureAIInference)

```python
from semantic_kernel.connectors.ai.azure_ai_inference import AzureAIInferenceChatPromptExecutionSettings

execution_settings = AzureAIInferenceChatPromptExecutionSettings()
```

# [Anthropic](#tab/python-Anthropic)

```python
from semantic_kernel.connectors.ai.anthropic import AnthropicChatPromptExecutionSettings

execution_settings = AnthropicChatPromptExecutionSettings()
```

# [Amazon Bedrock](#tab/python-AmazonBedrock)

```python
from semantic_kernel.connectors.ai.bedrock import BedrockChatPromptExecutionSettings

execution_settings = BedrockChatPromptExecutionSettings()
```

# [Google AI](#tab/python-Google)

```python
from semantic_kernel.connectors.ai.google.google_ai import GoogleAIChatPromptExecutionSettings

execution_settings = GoogleAIChatPromptExecutionSettings()
```

# [Vertex AI](#tab/python-VertexAI)

```python
from semantic_kernel.connectors.ai.google.vertex_ai import VertexAIChatPromptExecutionSettings

execution_settings = VertexAIChatPromptExecutionSettings()
```

# [Mistral AI](#tab/python-MistralAI)

```python
from semantic_kernel.connectors.ai.mistral_ai import MistralAIChatPromptExecutionSettings

execution_settings = MistralAIChatPromptExecutionSettings()
```

# [Ollama](#tab/python-Ollama)

```python
from semantic_kernel.connectors.ai.ollama import OllamaChatPromptExecutionSettings

execution_settings = OllamaChatPromptExecutionSettings()
```

# [ONNX](#tab/python-ONNX)

```python
from semantic_kernel.connectors.ai.onnx import OnnxGenAIPromptExecutionSettings

execution_settings = OnnxGenAIPromptExecutionSettings()
```

---

> [!TIP]
> To see what you can configure in the execution settings, you can check the class definition in the [source code](https://github.com/microsoft/semantic-kernel/tree/main/python/semantic_kernel/connectors/ai) or check out the [API documentation](https://learn.microsoft.com/en-us/python/api/semantic-kernel/semantic_kernel.connectors.ai?view=semantic-kernel-python).

::: zone-end

Below are the two ways you can use a chat completion service to generate responses.

### Non-streaming chat completion

To use non-streaming chat completion, you can use the following code to generate a response from the AI agent.

::: zone pivot="programming-language-csharp"

```csharp
ChatHistory history = [];
history.AddUserMessage("Hello, how are you?");

var response = await chatCompletionService.GetChatMessageContentAsync(
    history,
    kernel: kernel
);
```

::: zone-end

::: zone pivot="programming-language-python"

```python
chat_history = ChatHistory()
chat_history.add_user_message("Hello, how are you?")

response = await chat_completion.get_chat_message_content(
    chat_history=history,
    settings=execution_settings,
)
```

::: zone-end

::: zone pivot="programming-language-java"

```java
ChatHistory history = new ChatHistory();
history.addUserMessage("Hello, how are you?");

InvocationContext optionalInvocationContext = null;

List<ChatMessageContent<?>> response = chatCompletionService.getChatMessageContentsAsync(
    history,
    kernel,
    optionalInvocationContext
);
```

::: zone-end

### Streaming chat completion

To use streaming chat completion, you can use the following code to generate a response from the AI agent.

::: zone pivot="programming-language-csharp"

```csharp
ChatHistory history = [];
history.AddUserMessage("Hello, how are you?");

var response = chatCompletionService.GetStreamingChatMessageContentsAsync(
    chatHistory: history,
    kernel: kernel
);

await foreach (var chunk in response)
{
    Console.Write(chunk);
}
```

::: zone-end

::: zone pivot="programming-language-python"

```python
chat_history = ChatHistory()
chat_history.add_user_message("Hello, how are you?")

response = chat_completion.get_streaming_chat_message_content(
    chat_history=history,
    settings=execution_settings,
)

async for chunk in response:
    print(chunk, end="")
```

::: zone-end

::: zone pivot="programming-language-java"

> [!NOTE]
> Semantic Kernel for Java does not support the streaming response model.

::: zone-end

## Next steps

Now that you've added chat completion services to your Semantic Kernel project, you can start creating conversations with your AI agent. To learn more about using a chat completion service, check out the following articles:

> [!div class="nextstepaction"]
> [Using the chat history object](./chat-history.md)

> [!div class="nextstepaction"]
> [Optimizing function calling with chat completion](./function-calling.md)
