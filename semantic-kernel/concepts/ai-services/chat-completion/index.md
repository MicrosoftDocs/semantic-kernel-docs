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

## Installing the necessary packages
Before adding chat completion to your kernel, you will need to install the necessary packages. Below are the packages you will need to install for each AI service provider.

# [Azure OpenAI](#tab/csharp-AzureOpenAI)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

# [OpenAI](#tab/csharp-OpenAI)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

# [Mistral](#tab/csharp-Mistral)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.MistralAI
```

# [Google](#tab/csharp-Google)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.Google
```

# [Hugging Face](#tab/csharp-HuggingFace)

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.HuggingFace
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
using Microsoft.SemanticKernel.Connectors.OpenAI;

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
using Microsoft.SemanticKernel.Connectors.OpenAI;

OpenAIChatCompletionService chatCompletionService = new (
    modelId: "gpt-4",
    apiKey: "YOUR_API_KEY",
    organization: "YOUR_ORG_ID", // Optional
    httpClient: new HttpClient() // Optional; if not provided, the HttpClient from the kernel will be used
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
To add a chat completion service, you can use the following code to add it to the kernel.

# [Azure OpenAI](#tab/python-AzureOpenAI)

```python
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion

# Initialize the kernel
kernel = Kernel()

# Add the Azure OpenAI chat completion service
kernel.add_service(AzureChatCompletion(
    deployment_name="my-deployment",
    api_key="my-api-key",
    base_url="https://my-deployment.azurewebsites.net", # Used to point to your service
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
))
```

# [OpenAI](#tab/python-OpenAI)

```python
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion

# Initialize the kernel
kernel = Kernel()

# Add the Azure OpenAI chat completion service
kernel.add_service(OpenAIChatCompletion(
    ai_model_id="my-deployment",
    api_key="my-api-key",
    org_id="my-org-id", # Optional
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
))
```

# [Other](#tab/python-other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

```python
from semantic_kernel import Kernel

# Initialize the kernel
kernel = Kernel()

# Add the Azure OpenAI chat completion service
kernel.add_service(OpenAIChatCompletion(
    ai_model_id="my-deployment",
    api_key="my-api-key",
    org_id="my-org-id", # Optional
    base_url="https://my-custom-deployment.net", # Used to point to your service
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
))
```
---

You can also create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/python-AzureOpenAI)
```python
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion

chat_completion_service = AzureChatCompletion(
    deployment_name="my-deployment",
    api_key="my-api-key",
    base_url="https://my-deployment.azurewebsites.net", # Used to point to your service
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)
```

# [OpenAI](#tab/python-OpenAI)

```python
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion

chat_completion_service = OpenAIChatCompletion(
    ai_model_id="my-deployment",
    api_key="my-api-key",
    org_id="my-org-id", # Optional
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)
```

# [Other](#tab/python-other)
For other AI service providers that support the OpenAI chat completion API (e.g., LLM Studio), you can use the following code to reuse the existing OpenAI chat completion connector.

```python
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion

chat_completion_service = OpenAIChatCompletion(
    ai_model_id="my-deployment",
    api_key="my-api-key",
    org_id="my-org-id", # Optional
    base_url="https://my-custom-deployment.net", # Used to point to your service
    service_id="my-service-id", # Optional; for targeting specific services within Semantic Kernel
)
```

---

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

chat_completion_service = kernel.get_service(type=ChatCompletionClientBase)
```
::: zone-end

::: zone pivot="programming-language-java"

```java
ChatCompletionService chatCompletionService = kernel.getService(ChatCompletionService.class);
```

::: zone-end

## Using chat completion services

Now that you have a chat completion service, you can use it to generate responses from an AI agent. There are two main ways to use a chat completion service:
- Non-streaming: You wait for the service to generate an entire response before returning it to the user.
- Streaming: Individual chunks of the response are generated and returned to the user as they are created.

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

response = (await chat_completion.get_chat_message_contents(
    chat_history=history,
    kernel=kernel,
))[0]
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

response = chat_completion.get_streaming_chat_message_contents(
    chat_history=history,
    kernel=kernel,
)

async for chunk in response:
    print(chunk)
```
::: zone-end

::: zone pivot="programming-language-java"

Semantic Kernel for Java does not support the streaming response model.

::: zone-end

## Next steps

Now that you've added chat completion services to your Semantic Kernel project, you can start creating conversations with your AI agent. To learn more about using a chat completion service, check out the following articles:

- [Using the chat history object](./chat-history.md)
- [Optimizing function calling with chat completion](./function-calling.md)