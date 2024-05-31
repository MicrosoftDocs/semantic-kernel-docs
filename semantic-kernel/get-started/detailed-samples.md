---
title: In-depth Semantic Kernel Demos
description: Go deeper with additional Demos to learn how to use Semantic Kernel. 
zone_pivot_groups: programming-languages
author: sophialagerkranspandey          
ms.topic: tutorial
ms.author: sopand
ms.date: 07/11/2023
ms.service: semantic-kernel
---

# Deep dive into Semantic Kernel

If you want to dive into deeper into Semantic Kernel and learn how to use more advanced functionality not explicitly covered in our Learn documentation, we recommend that you check out our concepts samples that individually demonstrate how to use specific features within the SDK.


:::row:::

   :::column span="3":::
        Each of the SDKs (Python, C#, and Java) have their own set of samples that walk through the SDK. Each sample is modelled as a test case within our main repo, so you're always guaranteed that the sample will work with the latest nightly version of the SDK! Below are most of the samples you'll find in our concepts project.
   :::column-end:::
   :::column span="2":::
        :::image type="content" source="../media/tutorials/kernel-syntax-examples.png" alt-text="Leverage the kernel syntax examples on GitHub":::
   :::column-end:::
:::row-end:::

::: zone pivot="programming-language-csharp"

## Agents

- [ComplexChat_NestedShopper](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/ComplexChat_NestedShopper.cs)
- [Legacy_AgentAuthoring](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_AgentAuthoring.cs)
- [Legacy_AgentCharts](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_AgentCharts.cs)
- [Legacy_AgentCollaboration](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_AgentCollaboration.cs)
- [Legacy_AgentDelegation](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_AgentDelegation.cs)
- [Legacy_AgentTools](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_AgentTools.cs)
- [Legacy_Agents](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_Agents.cs)
- [Legacy_ChatCompletionAgent](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_ChatCompletionAgent.cs)
- [MixedChat_Agents](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/MixedChat_Agents.cs)
- [OpenAIAssistant_ChartMaker](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/OpenAIAssistant_ChartMaker.cs)
- [OpenAIAssistant_CodeInterpreter](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/OpenAIAssistant_CodeInterpreter.cs)
- [OpenAIAssistant_Retrieval](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/OpenAIAssistant_Retrieval.cs)

## Audio to text

- [OpenAI_AudioToText](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/AudioToText/OpenAI_AudioToText.cs)

## Automatic function calling

- [Gemini_FunctionCalling](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/AutoFunctionCalling/Gemini_FunctionCalling.cs)
- [OpenAI_FunctionCalling](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/AutoFunctionCalling/OpenAI_FunctionCalling.cs)

## Semantic caching

- [SemanticCachingWithFilters](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Caching/SemanticCachingWithFilters.cs)

## Chat completion

- [AzureOpenAIWithData_ChatCompletion](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/AzureOpenAIWithData_ChatCompletion.cs)
- [ChatHistoryAuthorName](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/ChatHistoryAuthorName.cs)
- [ChatHistorySerialization](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/ChatHistorySerialization.cs)
- [Connectors_CustomHttpClient](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Connectors_CustomHttpClient.cs)
- [Connectors_KernelStreaming](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Connectors_KernelStreaming.cs)
- [Connectors_WithMultipleLLMs](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Connectors_WithMultipleLLMs.cs)
- [Google_GeminiChatCompletion](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Google_GeminiChatCompletion.cs)
- [Google_GeminiChatCompletionStreaming](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Google_GeminiChatCompletionStreaming.cs)
- [Google_GeminiGetModelResult](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Google_GeminiGetModelResult.cs)
- [Google_GeminiVision](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Google_GeminiVision.cs)
- [OpenAI_ChatCompletion](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_ChatCompletion.cs)
- [OpenAI_ChatCompletionMultipleChoices](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_ChatCompletionMultipleChoices.cs)
- [OpenAI_ChatCompletionStreaming](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_ChatCompletionStreaming.cs)
- [OpenAI_ChatCompletionStreamingMultipleChoices](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_ChatCompletionStreamingMultipleChoices.cs)
- [OpenAI_ChatCompletionWithVision](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_ChatCompletionWithVision.cs)
- [OpenAI_CustomAzureOpenAIClient](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_CustomAzureOpenAIClient.cs)
- [OpenAI_UsingLogitBias](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_UsingLogitBias.cs)
- [OpenAI_FunctionCalling](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_FunctionCalling.cs)
- [MistralAI_ChatPrompt](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/MistralAI_ChatPrompt.cs)
- [MistralAI_FunctionCalling](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/MistralAI_FunctionCalling.cs)
- [MistralAI_StreamingFunctionCalling](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/MistralAI_StreamingFunctionCalling.cs)

## Dependency injection

- [HttpClient_Registration](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/DependencyInjection/HttpClient_Registration.cs)
- [HttpClient_Resiliency](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/DependencyInjection/HttpClient_Resiliency.cs)
- [Kernel_Building](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/DependencyInjection/Kernel_Building.cs)
- [Kernel_Injecting](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/DependencyInjection/Kernel_Injecting.cs)

## Filtering

- [AutoFunctionInvocationFiltering](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/AutoFunctionInvocationFiltering.cs)
- [FunctionInvocationFiltering](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/FunctionInvocationFiltering.cs)
- [Legacy_KernelHooks](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/Legacy_KernelHooks.cs)
- [PromptRenderFiltering](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/PromptRenderFiltering.cs)
- [RetryWithFilters](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/RetryWithFilters.cs)
- [PIIDetectionWithFilters](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/PIIDetectionWithFilters.cs)

## Plugin functions

- [Arguments](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/Arguments.cs)
- [FunctionResult_Metadata](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/FunctionResult_Metadata.cs)
- [FunctionResult_StronglyTyped](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/FunctionResult_StronglyTyped.cs)
- [MethodFunctions](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/MethodFunctions.cs)
- [MethodFunctions_Advanced](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/MethodFunctions_Advanced.cs)
- [MethodFunctions_Types](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/MethodFunctions_Types.cs)
- [PromptFunctions_Inline](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/PromptFunctions_Inline.cs)
- [PromptFunctions_MultipleArguments](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/PromptFunctions_MultipleArguments.cs)

## Image to text

- [HuggingFace_ImageToText](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ImageToText/HuggingFace_ImageToText.cs)

## Local models

- [HuggingFace_ChatCompletionWithTGI](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/LocalModels/HuggingFace_ChatCompletionWithTGI.cs)
- [MultipleProviders_ChatCompletion](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/LocalModels/MultipleProviders_ChatCompletion.cs)

## Memory

- [HuggingFace_EmbeddingGeneration](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/HuggingFace_EmbeddingGeneration.cs)
- [MemoryStore_CustomReadOnly](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/MemoryStore_CustomReadOnly.cs)
- [SemanticTextMemory_Building](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/SemanticTextMemory_Building.cs)
- [TextChunkerUsage](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/TextChunkerUsage.cs)
- [TextChunkingAndEmbedding](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/TextChunkingAndEmbedding.cs)
- [TextMemoryPlugin_GeminiEmbeddingGeneration](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/TextMemoryPlugin_GeminiEmbeddingGeneration.cs)
- [TextMemoryPlugin_MultipleMemoryStore](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/TextMemoryPlugin_MultipleMemoryStore.cs)

## Planning

- [FunctionCallStepwisePlanning](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Planners/FunctionCallStepwisePlanning.cs)
- [HandlebarsPlanning](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Planners/HandlebarsPlanning.cs)

## Plugins

- [ApiManifestBasedPlugins](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/ApiManifestBasedPlugins.cs)
- [ConversationSummaryPlugin](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/ConversationSummaryPlugin.cs)
- [CreatePluginFromOpenAI_AzureKeyVault](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/CreatePluginFromOpenAI_AzureKeyVault.cs)
- [CreatePluginFromOpenApiSpec_Github](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/CreatePluginFromOpenApiSpec_Github.cs)
- [CreatePluginFromOpenApiSpec_Jira](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/CreatePluginFromOpenApiSpec_Jira.cs)
- [CustomMutablePlugin](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/CustomMutablePlugin.cs)
- [DescribeAllPluginsAndFunctions](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/DescribeAllPluginsAndFunctions.cs)
- [GroundednessChecks](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/GroundednessChecks.cs)
- [ImportPluginFromGrpc](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/ImportPluginFromGrpc.cs)
- [OpenAIPlugins](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/OpenAIPlugins.cs)

## Prompt templates

- [ChatCompletionPrompts](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/ChatCompletionPrompts.cs)
- [ChatWithPrompts](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/ChatWithPrompts.cs)
- [LiquidPrompts](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/LiquidPrompts.cs)
- [MultiplePromptTemplates](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/MultiplePromptTemplates.cs)
- [PromptFunctionsWithChatGPT](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/PromptFunctionsWithChatGPT.cs)
- [TemplateLanguage](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/TemplateLanguage.cs)
- [PromptyFunction](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptYemplates/PromptyFunction.cs)

## Retrieval augmented generation (RAG)

- [WithFunctionCallingStepwisePlanner](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/RAG/WithFunctionCallingStepwisePlanner.cs)
- [WithPlugins](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/RAG/WithPlugins.cs)

## Search

- [BingAndGooglePlugins](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Search/BingAndGooglePlugins.cs)
- [MyAzureAISearchPlugin](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Search/MyAzureAISearchPlugin.cs)
- [WebSearchQueriesPlugin](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Search/WebSearchQueriesPlugin.cs)

## Text generation

- [Custom_TextGenerationService](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/TextGeneration/Custom_TextGenerationService.cs)
- [HuggingFace_TextGeneration](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/TextGeneration/HuggingFace_TextGeneration.cs)
- [OpenAI_TextGenerationStreaming](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/TextGeneration/OpenAI_TextGenerationStreaming.cs)

## Text to audio

- [OpenAI_TextToAudio](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/TextToAudio/OpenAI_TextToAudio.cs)

## Text to image

- [OpenAI_TextToImage](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/TextToImage/OpenAI_TextToImageDalle3.cs)

::: zone-end