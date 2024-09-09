---
title: OpenAI Connector Migration Guide
description: Learn how to migrate .Net OpenAI Connectors after 1.18.0.
author: rogerbarreto
ms.topic: tutorial
ms.author: rbarreto
ms.date: 08/13/2024
ms.service: semantic-kernel
---

# OpenAI Connector Migration Guide

Coming as part of the new **1.18.0** version of Semantic Kernel we migrated our `OpenAI` and `AzureOpenAI` services to use the new `OpenAI SDK v2.0` and `Azure OpenAI SDK v2.0` SDKs. 

As those changes were major breaking changes when implementing ours we looked forward to break as minimal as possible the dev experience.

This guide prepares you for the migration that you may need to do to use our new OpenAI Connector is a complete rewrite of the existing OpenAI Connector and is designed to be more efficient, reliable, and scalable. This manual will guide you through the migration process and help you understand the changes that have been made to the OpenAI Connector.

Those changes are needed for anyone using `OpenAI` or `AzureOpenAI` connectors with Semantic Kernel version `1.18.0-rc` or above.

## 1. Package Setup when using Azure only services

If you are working with Azure services you will need to change the package from `Microsoft.SemanticKernel.Connectors.OpenAI` to `Microsoft.SemanticKernel.Connectors.AzureOpenAI`. This is necessary as we created two distinct connectors for each.

> [!IMPORTANT]
> The `Microsoft.SemanticKernel.Connectors.AzureOpenAI` package depends on the `Microsoft.SemanticKernel.Connectors.OpenAI` package so there's no need to add both to your project when using `OpenAI` related types.

```diff
Before
- using Microsoft.SemanticKernel.Connectors.OpenAI;
After
+ using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
```

### 1.1 AzureOpenAIClient

When using Azure with OpenAI, before where you were using `OpenAIClient` you will need to update your code to use the new `AzureOpenAIClient` type.

### 1.2 Services

All services below now belong to the `Microsoft.SemanticKernel.Connectors.AzureOpenAI` namespace.

- `AzureOpenAIAudioToTextService`
- `AzureOpenAIChatCompletionService`
- `AzureOpenAITextEmbeddingGenerationService`
- `AzureOpenAITextToAudioService`
- `AzureOpenAITextToImageService`

## 2. Text Generation Deprecation

The latest `OpenAI` SDK does not support text generation modality, when migrating to their underlying SDK we had to drop support as well and remove `TextGeneration` specific services.

If you were using OpenAI's `gpt-3.5-turbo-instruct` legacy model with any of the `OpenAITextGenerationService` or `AzureOpenAITextGenerationService` you will need to update your code to target a chat completion model instead, using `OpenAIChatCompletionService` or `AzureOpenAIChatCompletionService` instead.

> [!NOTE]
> OpenAI and AzureOpenAI `ChatCompletion` services also implement the `ITextGenerationService` interface and that may not require any changes to your code if you were targeting the `ITextGenerationService` interface.

tags: `AddOpenAITextGeneration`, `AddAzureOpenAITextGeneration`

## 3. ChatCompletion Multiple Choices Deprecated

The latest `OpenAI` SDK does not support multiple choices, when migrating to their underlying SDK we had to drop the support and remove `ResultsPerPrompt` also from the `OpenAIPromptExecutionSettings`.

tags: `results_per_prompt`

## 4. OpenAI File Service Deprecation

The `OpenAIFileService` was deprecated in the latest version of the OpenAI Connector. We strongly recommend to update your code to use the new `OpenAIClient.GetFileClient()` for file management operations.

## 5. SemanticKernel MetaPackage

To be retrocompatible with the new OpenAI and AzureOpenAI Connectors, our `Microsoft.SemanticKernel` meta package changed its dependency to use the new `Microsoft.SemanticKernel.Connectors.AzureOpenAI` package that depends on the `Microsoft.SemanticKernel.Connectors.OpenAI` package. This way if you are using the metapackage, no change is needed to get access to `Azure` related types.

## 6. Chat Message Content Changes

### 6.1 OpenAIChatMessageContent

- The `Tools` property type has changed from `IReadOnlyList<ChatCompletionsToolCall>` to `IReadOnlyList<ChatToolCall>`.

- Inner content type has changed from `ChatCompletionsFunctionToolCall` to `ChatToolCall`.

- Metadata type `FunctionToolCalls` has changed from `IEnumerable<ChatCompletionsFunctionToolCall>` to `IEnumerable<ChatToolCall>`.

### 6.2 OpenAIStreamingChatMessageContent

- The `FinishReason` property type has changed from `CompletionsFinishReason` to `FinishReason`.
- The `ToolCallUpdate` property has been renamed to `ToolCallUpdates` and its type has changed from `StreamingToolCallUpdate?` to `IReadOnlyList<StreamingToolCallUpdate>?`.
- The `AuthorName` property is not initialized because it's not provided by the underlying library anymore.

## 7. Metrics for AzureOpenAI Connector

The meter `s_meter = new("Microsoft.SemanticKernel.Connectors.OpenAI");` and the relevant counters still have old names that contain "openai" in them, such as:

- `semantic_kernel.connectors.openai.tokens.prompt`
- `semantic_kernel.connectors.openai.tokens.completion`
- `semantic_kernel.connectors.openai.tokens.total`

## 8. Using Azure with your data (Data Sources)

With the new `AzureOpenAIClient`, you can now specify your datasource thru the options and that requires a small change in your code to the new type.

Before

```csharp
var promptExecutionSettings = new OpenAIPromptExecutionSettings
{
    AzureChatExtensionsOptions = new AzureChatExtensionsOptions
    {
        Extensions = [ new AzureSearchChatExtensionConfiguration
        {
            SearchEndpoint = new Uri(TestConfiguration.AzureAISearch.Endpoint),
            Authentication = new OnYourDataApiKeyAuthenticationOptions(TestConfiguration.AzureAISearch.ApiKey),
            IndexName = TestConfiguration.AzureAISearch.IndexName
        }]
    };
};
```

After

```csharp
var promptExecutionSettings = new AzureOpenAIPromptExecutionSettings
{
    AzureChatDataSource = new AzureSearchChatDataSource
    {
         Endpoint = new Uri(TestConfiguration.AzureAISearch.Endpoint),
         Authentication = DataSourceAuthentication.FromApiKey(TestConfiguration.AzureAISearch.ApiKey),
         IndexName = TestConfiguration.AzureAISearch.IndexName
    }
};
```
Tags: `WithData`, `AzureOpenAIChatCompletionWithDataConfig`, `AzureOpenAIChatCompletionWithDataService`

## 9. Breaking glass scenarios

Breaking glass scenarios are scenarios where you may need to update your code to use the new OpenAI Connector. Below are some of the breaking changes that you may need to be aware of.

#### 9.1 KernelContent Metadata

Some of the keys in the content metadata dictionary have changed and removed.

- Changed: `Created` -> `CreatedAt`
- Changed: `LogProbabilityInfo` -> `ContentTokenLogProbabilities`
- Changed: `PromptFilterResults` -> `ContentFilterResultForPrompt`
- Changed: `ContentFilterResultsForPrompt` -> `ContentFilterResultForResponse`
- Removed: `FinishDetails`
- Removed: `Index`
- Removed: `Enhancements`

#### 9.2 Prompt Filter Results

The `PromptFilterResults` metadata type has changed from `IReadOnlyList<ContentFilterResultsForPrompt>` to `ContentFilterResultForPrompt`.

#### 9.3 Content Filter Results

The `ContentFilterResultsForPrompt` type has changed from `ContentFilterResultsForChoice` to `ContentFilterResultForResponse`.

#### 9.4 Finish Reason

The FinishReason metadata string value has changed from `stop` to `Stop`

#### 9.5 Tool Calls

The ToolCalls metadata string value has changed from `tool_calls` to `ToolCalls`

#### 9.6 LogProbs / Log Probability Info

The `LogProbabilityInfo` type has changed from `ChatChoiceLogProbabilityInfo` to `IReadOnlyList<ChatTokenLogProbabilityInfo>`.

#### 9.8 Token Usage

The Token usage naming convention from `OpenAI` changed from `Completion`, `Prompt` tokens to `Output` and `Input` respectively. You will need to update your code to use the new naming.

The type also changed from `CompletionsUsage` to `ChatTokenUsage`.

[Example of Token Usage Metadata Changes](https://github.com/microsoft/semantic-kernel/pull/7151/files#diff-a323107b9f8dc8559a83e50080c6e34551ddf6d9d770197a473f249589e8fb47)

```diff
Before
- var usage = FunctionResult.Metadata?["Usage"] as CompletionsUsage;
- var completionTokesn = usage?.CompletionTokens;
- var promptTokens = usage?.PromptTokens;

After
+ var usage = FunctionResult.Metadata?["Usage"] as ChatTokenUsage;
+ var promptTokens = usage?.InputTokens;
+ var completionTokens = completionTokens: usage?.OutputTokens;
```

#### 9.9 OpenAIClient

The `OpenAIClient` type previously was a Azure specific namespace type but now it is an `OpenAI` SDK namespace type, you will need to update your code to use the new `OpenAIClient` type.

When using Azure, you will need to update your code to use the new `AzureOpenAIClient` type.

#### 9.10 OpenAIClientOptions

The `OpenAIClientOptions` type previously was a Azure specific namespace type but now it is an `OpenAI` SDK namespace type, you will need to update your code to use the new `AzureOpenAIClientOptions` type if you are using the new `AzureOpenAIClient` with any of the specific options for the Azure client.

#### 9.11 Pipeline Configuration

The new `OpenAI` SDK uses a different pipeline configuration, and has a dependency on `System.ClientModel` package. You will need to update your code to use the new `HttpClientPipelineTransport` transport configuration where before you were using `HttpClientTransport` from `Azure.Core.Pipeline`.

[Example of Pipeline Configuration](https://github.com/microsoft/semantic-kernel/pull/7151/files#diff-fab02d9a75bf43cb57f71dddc920c3f72882acf83fb125d8cad963a643d26eb3)

```diff
var clientOptions = new OpenAIClientOptions
{
Before: From Azure.Core.Pipeline
-    Transport = new HttpClientTransport(httpClient),
-    RetryPolicy = new RetryPolicy(maxRetries: 0), // Disable Azure SDK retry policy if and only if a custom HttpClient is provided.
-    Retry = { NetworkTimeout = Timeout.InfiniteTimeSpan } // Disable Azure SDK default timeout
     

After: From OpenAI SDK -> System.ClientModel
+    Transport = new HttpClientPipelineTransport(httpClient),
+    RetryPolicy = new ClientRetryPolicy(maxRetries: 0); // Disable retry policy if and only if a custom HttpClient is provided.
+    NetworkTimeout = Timeout.InfiniteTimeSpan; // Disable default timeout
};
```