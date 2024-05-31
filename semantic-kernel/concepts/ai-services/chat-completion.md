
# Chat completion
With chat completion, you can simulate a back-and-forth conversation with an AI. This is of course useful for creating chat bots, but it can also be used for automating business processes, creating code snippets, and more. As the primary model type provided by OpenAI, Google, Mistral, Facebook, and others, chat completion is the most common AI service that you will add to your Semantic Kernel project.

When picking out a chat completion model, you will need to consider the following:
- What modalities does the model support (e.g., text, image, audio, etc.)?
- Does it support function calling?
- How fast does it receive and generate tokens?
- How much does each token cost?

> [!IMPORTANT]
> Of all the above questions, the most important is whether the model supports function calling. If it does not, you will not be able to use the model to call your existing code. Most of the latest models from OpenAI, Google, Mistral, and Amazon all support function calling. Support from small language models, however, is still limited.

::: zone pivot="programming-language-csharp"

## Installing the necessary packages
Before adding chat completion to your kernel, you will need to install the necessary packages. Below are the packages you will need to install for each AI service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

# [OpenAI](#tab/OpenAI)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

# [Mistral](#tab/Mistral)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.MistralAI
```

# [Google](#tab/Google)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.Google
```

# [Hugging Face](#tab/HuggingFace)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.HuggingFace
```

# [Other](#tab/other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the OpenAI chat completion connector.

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

---

## Creating chat completion services

### Adding directly to the kernel
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

# [Azure OpenAI](#tab/AzureOpenAI)

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

# [Azure OpenAI](#tab/AzureOpenAI)

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

## Using the chat completion service

Once you've created an instance of the chat completion service, you can use it to generate the next response in a conversation. Before invoking the service, you will need to create a chat history object to store the conversation.

### Creating a chat history object

```csharp
```

Afterwards, you can send the chat history object to the LLM to generate the next response from the AI. When generating the next response, you have access to both non-streaming and streaming methods for generating responses from the `IChatCompletion` interface.

> [!TIP]
> We recommend that you always use the streaming methods when possible. This is because the streaming methods provide user's with a faster initial response time, which reduces the perceived latency of the AI.

### Retrieving the service from the kernel

To use either the non-streaming or streaming methods, you'll first need access to the actual service. If you've added the service to the kernel, you can retrieve it from the kernel's service provider.

```csharp
IChatCompletionService chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
```

### Non-streaming completion

```csharp
```

### Streaming completion

```csharp
```

## Function calling with chat completion

The most powerful feature of chat completion is the ability to call functions from the model. This allows you to create a chat bot that can interact with your existing code, making it possible to automate business processes, create code snippets, and more.

The following sample demonstrates how to add a plugin to the kernel that can be called by the chat completion service.

```csharp
```

For a more detailed explanation of how to use function calling with chat completion, refer to the [planning article](./planning.md).

## Multi-modal chat completion

Some chat completion models support modalities other than text. OpenAI's GPT-4o, for example, supports images as input within a chat message. This mimics how humans can attach images to their messages in chat apps.

With this additional input, the LLM can answer questions about the image or generate captions for the image. Below is an example of how to create a chat completion service that supports multi-modal input.

```csharp
```
