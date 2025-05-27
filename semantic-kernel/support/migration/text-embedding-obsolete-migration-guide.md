---
title: Migrating from ITextEmbeddingGenerationService to IEmbeddingGenerator
description: Describes the steps for migrating from the obsolete ITextEmbeddingGenerationService to the new Microsoft.Extensions.AI IEmbeddingGenerator interface.
zone_pivot_groups: programming-languages
author: rogerbarreto
ms.topic: conceptual
ms.author: rbarreto
ms.date: 05/21/2025
ms.service: semantic-kernel
---

::: zone pivot="programming-language-csharp"

# Migrating from ITextEmbeddingGenerationService to IEmbeddingGenerator

As Semantic Kernel shifts its foundational abstractions to Microsoft.Extensions.AI, we are obsoleting and moving away from our experimental embeddings interfaces to the new standardized abstractions that provide a more consistent and powerful way to work with AI services across the .NET ecosystem.

This guide will help you migrate from the obsolete `ITextEmbeddingGenerationService` interface to the new `Microsoft.Extensions.AI.IEmbeddingGenerator<string, Embedding<float>>` interface.

## Why Make the Change?

The transition to `Microsoft.Extensions.AI.IEmbeddingGenerator` brings several benefits:

1. **Standardization**: Aligns with broader .NET ecosystem patterns and Microsoft.Extensions conventions
2. **Type Safety**: Stronger typing with the generic `Embedding<float>` return type
3. **Flexibility**: Support for different input types and embedding formats
4. **Consistency**: Uniform approach across different AI service providers
5. **Integration**: Seamless integration with other Microsoft.Extensions libraries

## Package Updates

Before migrating your code, ensure you have the latest Semantic Kernel packages that support the new `IEmbeddingGenerator` interface. The new interface is available in the same connector packages, so no additional package references are required.

## Service Registration Migration

### Before: Using ITextEmbeddingGenerationService

```csharp
using Microsoft.SemanticKernel;

// Create a kernel builder
var builder = Kernel.CreateBuilder();

// Add the OpenAI embedding service
#pragma warning disable SKEXP0010
builder.Services.AddOpenAITextEmbeddingGeneration(
    modelId: "text-embedding-ada-002",
    apiKey: "your-api-key");
#pragma warning restore SKEXP0010

// Build the kernel
var kernel = builder.Build();
```

### After: Using IEmbeddingGenerator

```csharp
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

// Create a kernel builder
var builder = Kernel.CreateBuilder();

// Add the OpenAI embedding generator
builder.Services.AddOpenAIEmbeddingGenerator(
    modelId: "text-embedding-ada-002",
    apiKey: "your-api-key");

// Build the kernel
var kernel = builder.Build();
```

## Interface Usage Migration

### Before: Using ITextEmbeddingGenerationService

```csharp
using Microsoft.SemanticKernel.Embeddings;

// Get the embedding service from the kernel
var embeddingService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

// Generate embeddings
var text = "Semantic Kernel is a lightweight SDK that integrates Large Language Models (LLMs) with conventional programming languages.";
var embedding = await embeddingService.GenerateEmbeddingAsync(text);

// Work with the embedding vector
Console.WriteLine($"Generated embedding with {embedding.Length} dimensions");
```

### After: Using IEmbeddingGenerator

```csharp
using Microsoft.Extensions.AI;

// Get the embedding generator from the kernel
var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

// Generate embeddings
var text = "Semantic Kernel is a lightweight SDK that integrates Large Language Models (LLMs) with conventional programming languages.";
var embedding = await embeddingGenerator.GenerateAsync(text);

// Work with the embedding vector
Console.WriteLine($"Generated embedding with {embedding.Vector.Length} dimensions");
```

## Key Differences

1. **Method Names**: `GenerateEmbeddingAsync` becomes `GenerateAsync`
2. **Return Type**: Instead of returning `ReadOnlyMemory<float>`, the new interface returns `GeneratedEmbeddings<Embedding<float>>`
3. **Vector Access**: Access the embedding vector through the `.Vector` property instead of directly
4. **Options**: The new interface accepts an optional `EmbeddingGenerationOptions` parameter for more control

## Multiple Embeddings Migration

### Before: Generating Multiple Embeddings

```csharp
using Microsoft.SemanticKernel.Embeddings;

var embeddingService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

var texts = new[]
{
    "First text to embed",
    "Second text to embed",
    "Third text to embed"
};

IList<ReadOnlyMemory<float>> embeddings = await embeddingService.GenerateEmbeddingsAsync(texts);

foreach (var embedding in embeddings)
{
    Console.WriteLine($"Generated embedding with {embedding.Length} dimensions");
}
```

### After: Generating Multiple Embeddings

```csharp
using Microsoft.Extensions.AI;

var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

var texts = new[]
{
    "First text to embed",
    "Second text to embed",
    "Third text to embed"
};

var embeddings = await embeddingGenerator.GenerateAsync(texts);

foreach (var embedding in embeddings)
{
    Console.WriteLine($"Generated embedding with {embedding.Vector.Length} dimensions");
}
```

## Transitional Support

To ease the transition, Semantic Kernel provides extension methods that allow you to convert between the old and new interfaces:

```csharp
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

// Create a kernel with the old embedding service
var builder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0010
builder.Services.AddOpenAITextEmbeddingGeneration(
    modelId: "text-embedding-ada-002",
    apiKey: "your-api-key");
#pragma warning restore SKEXP0010

var kernel = builder.Build();

// Get the old embedding service
var oldEmbeddingService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

// Convert from old to new using extension method
IEmbeddingGenerator<string, Embedding<float>> newGenerator =
    oldEmbeddingService.AsEmbeddingGenerator();

// Use the new generator
var newEmbedding = await newGenerator.GenerateAsync("Converting from old to new");
Console.WriteLine($"Generated embedding with {newEmbedding.Vector.Length} dimensions");
```

## Connector Support

All Semantic Kernel connectors have been updated to support the new interface:

- **OpenAI and Azure OpenAI**: Use `AddOpenAIEmbeddingGenerator` and `AddAzureOpenAIEmbeddingGenerator`
- **Google AI and Vertex AI**: Use `AddGoogleAIEmbeddingGenerator` and `AddVertexAIEmbeddingGenerator`
- **Amazon Bedrock**: Use `AddBedrockEmbeddingGenerator`
- **Hugging Face**: Use `AddHuggingFaceEmbeddingGenerator`
- **MistralAI**: Use `AddMistralEmbeddingGenerator`
- **Ollama**: Use `AddOllamaEmbeddingGenerator`
- **ONNX**: Use `AddBertOnnxEmbeddingGenerator`

Each connector now provides both the legacy service (marked as obsolete) and the new generator implementation.

## Azure OpenAI Example

### Before: Azure OpenAI with ITextEmbeddingGenerationService

```csharp
using Microsoft.SemanticKernel;

var builder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0010
builder.Services.AddAzureOpenAITextEmbeddingGeneration(
    deploymentName: "text-embedding-ada-002",
    endpoint: "https://myaiservice.openai.azure.com",
    apiKey: "your-api-key");
#pragma warning restore SKEXP0010

var kernel = builder.Build();
var embeddingService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
```

### After: Azure OpenAI with IEmbeddingGenerator

```csharp
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

var builder = Kernel.CreateBuilder();

builder.Services.AddAzureOpenAIEmbeddingGenerator(
    deploymentName: "text-embedding-ada-002",
    endpoint: "https://myaiservice.openai.azure.com",
    apiKey: "your-api-key");

var kernel = builder.Build();
var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
```

## Using Embedding Generation Options

The new interface supports additional options for more control over embedding generation:

### Before: Limited Options

```csharp
// The old interface had limited options, mostly configured during service registration
var embedding = await embeddingService.GenerateEmbeddingAsync(text);
```

### After: Rich Options Support

```csharp
using Microsoft.Extensions.AI;

var options = new EmbeddingGenerationOptions
{
    // Configure additional options as needed
    AdditionalProperties = new Dictionary<string, object>
    {
        ["user"] = "user-123"
    }
};

var embedding = await embeddingGenerator.GenerateAsync(text, options);
```

## Working with Vector Stores

The new `IEmbeddingGenerator` interface integrates seamlessly with Semantic Kernel's vector stores:

```csharp
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.InMemory;

// Create an embedding generator
var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

// Use with vector stores
var vectorStore = new InMemoryVectorStore(new() { EmbeddingGenerator = embeddingGenerator });
var collection = vectorStore.GetCollection<string, MyRecord>("myCollection");

// The vector store will automatically use the embedding generator for text properties
await collection.UpsertAsync(new MyRecord
{
    Id = "1",
    Text = "This text will be automatically embedded"
});
```

## Direct Service Instantiation

### Before: Direct Service Creation

```csharp
using Microsoft.SemanticKernel.Connectors.OpenAI;

#pragma warning disable SKEXP0010
var embeddingService = new OpenAITextEmbeddingGenerationService(
    modelId: "text-embedding-ada-002",
    apiKey: "your-api-key");
#pragma warning restore SKEXP0010
```

### After: Using Microsoft.Extensions.AI.OpenAI

```csharp
using Microsoft.Extensions.AI;
using OpenAI;

// Create using the OpenAI SDK directly
var openAIClient = new OpenAIClient("your-api-key");
var embeddingGenerator = openAIClient
    .GetEmbeddingClient("text-embedding-ada-002")
    .AsIEmbeddingGenerator();
```

## Next Steps

1. Update your package references to the latest Semantic Kernel versions
2. Replace `ITextEmbeddingGenerationService` with `IEmbeddingGenerator<string, Embedding<float>>`
3. Update service registration to use the new embedding generator methods (e.g., `AddOpenAIEmbeddingGenerator`)
4. Update method calls from `GenerateEmbeddingAsync`/`GenerateEmbeddingsAsync` to `GenerateAsync`
5. Update how you access the embedding vectors (now through the `.Vector` property)
6. Consider using the new options parameter for additional control
7. Test your application to ensure the migration is successful

The old interface will continue to work for now but is marked as obsolete and will be removed in a future release. We encourage all Semantic Kernel users to migrate to the new `IEmbeddingGenerator<string, Embedding<float>>` interface as soon as possible.

For more information about Microsoft.Extensions.AI, check out the [official announcement](https://devblogs.microsoft.com/semantic-kernel/microsoft-extensions-ai-simplifying-ai-integration-for-net-partners/).

::: zone-end

::: zone pivot="programming-language-python"

::: zone-end

::: zone pivot="programming-language-java"

::: zone-end
