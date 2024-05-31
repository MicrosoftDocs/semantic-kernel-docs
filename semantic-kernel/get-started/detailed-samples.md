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


     ::: zone pivot="programming-language-csharp"
     > [!div class="nextstepaction"]
     > [View all C# concept samples on GitHub](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/Concepts)
     ::: end-zone

   :::column-end:::
   :::column span="2":::
        :::image type="content" source="../media/tutorials/kernel-syntax-examples.png" alt-text="Leverage the kernel syntax examples on GitHub":::
   :::column-end:::
:::row-end:::

::: zone pivot="programming-language-csharp"
| Category                       | Sample Name                                   | Link                                                                                                                                                   |
| ------------------------------ | --------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Agents**                         | ComplexChat_NestedShopper                     | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/ComplexChat_NestedShopper.cs)                             |
|                                | Legacy_AgentAuthoring                         | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_AgentAuthoring.cs)                                 |
|                                | Legacy_AgentCharts                            | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_AgentCharts.cs)                                    |
|                                | Legacy_AgentCollaboration                     | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_AgentCollaboration.cs)                             |
|                                | Legacy_AgentDelegation                        | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_AgentDelegation.cs)                                |
|                                | Legacy_AgentTools                             | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_AgentTools.cs)                                     |
|                                | Legacy_Agents                                 | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_Agents.cs)                                         |
|                                | Legacy_ChatCompletionAgent                    | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/Legacy_ChatCompletionAgent.cs)                            |
|                                | MixedChat_Agents                              | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/MixedChat_Agents.cs)                                      |
|                                | OpenAIAssistant_ChartMaker                    | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/OpenAIAssistant_ChartMaker.cs)                            |
|                                | OpenAIAssistant_CodeInterpreter               | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/OpenAIAssistant_CodeInterpreter.cs)                       |
|                                | OpenAIAssistant_Retrieval                     | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Agents/OpenAIAssistant_Retrieval.cs)                             |
| **Audio to text**                  | OpenAI_AudioToText                            | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/AudioToText/OpenAI_AudioToText.cs)                               |
| Automatic function calling     | Gemini_FunctionCalling                        | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/AutoFunctionCalling/Gemini_FunctionCalling.cs)                   |
|                                | OpenAI_FunctionCalling                        | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/AutoFunctionCalling/OpenAI_FunctionCalling.cs)                   |
| **Semantic caching**               | SemanticCachingWithFilters                    | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Caching/SemanticCachingWithFilters.cs)                           |
| **Chat completion**                | AzureOpenAIWithData_ChatCompletion            | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/AzureOpenAIWithData_ChatCompletion.cs)            |
|                                | ChatHistoryAuthorName                         | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/ChatHistoryAuthorName.cs)                         |
|                                | ChatHistorySerialization                      | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/ChatHistorySerialization.cs)                      |
|                                | Connectors_CustomHttpClient                   | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Connectors_CustomHttpClient.cs)                   |
|                                | Connectors_KernelStreaming                    | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Connectors_KernelStreaming.cs)                    |
|                                | Connectors_WithMultipleLLMs                   | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Connectors_WithMultipleLLMs.cs)                   |
|                                | Google_GeminiChatCompletion                   | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Google_GeminiChatCompletion.cs)                   |
|                                | Google_GeminiChatCompletionStreaming          | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Google_GeminiChatCompletionStreaming.cs)          |
|                                | Google_GeminiGetModelResult                   | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Google_GeminiGetModelResult.cs)                   |
|                                | Google_GeminiVision                           | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Google_GeminiVision.cs)                           |
|                                | OpenAI_ChatCompletion                         | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_ChatCompletion.cs)                         |
|                                | OpenAI_ChatCompletionMultipleChoices          | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_ChatCompletionMultipleChoices.cs)          |
|                                | OpenAI_ChatCompletionStreaming                | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_ChatCompletionStreaming.cs)                |
|                                | OpenAI_ChatCompletionStreamingMultipleChoices | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_ChatCompletionStreamingMultipleChoices.cs) |
|                                | OpenAI_ChatCompletionWithVision               | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_ChatCompletionWithVision.cs)               |
|                                | OpenAI_CustomAzureOpenAIClient                | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_CustomAzureOpenAIClient.cs)                |
|                                | OpenAI_UsingLogitBias                         | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_UsingLogitBias.cs)                         |
|                                | OpenAI_FunctionCalling                        | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_FunctionCalling.cs)                        |
|                                | MistralAI_ChatPrompt                          | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/MistralAI_ChatPrompt.cs)                          |
|                                | MistralAI_FunctionCalling                     | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/MistralAI_FunctionCalling.cs)                     |
|                                | MistralAI_StreamingFunctionCalling            | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/MistralAI_StreamingFunctionCalling.cs)            |
| **Dependency injection**           | HttpClient_Registration                       | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/DependencyInjection/HttpClient_Registration.cs)                  |
|                                | HttpClient_Resiliency                         | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/DependencyInjection/HttpClient_Resiliency.cs)                    |
|                                | Kernel_Building                               | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/DependencyInjection/Kernel_Building.cs)                          |
|                                | Kernel_Injecting                              | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/DependencyInjection/Kernel_Injecting.cs)                         |
| **Filtering**                      | AutoFunctionInvocationFiltering               | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/AutoFunctionInvocationFiltering.cs)                    |
|                                | FunctionInvocationFiltering                   | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/FunctionInvocationFiltering.cs)                        |
|                                | Legacy_KernelHooks                            | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/Legacy_KernelHooks.cs)                                 |
|                                | PromptRenderFiltering                         | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/PromptRenderFiltering.cs)                              |
|                                | RetryWithFilters                              | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/RetryWithFilters.cs)                                   |
|                                | PIIDetectionWithFilters                       | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Filtering/PIIDetectionWithFilters.cs)                            |
| **Plugin functions**               | Arguments                                     | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/Arguments.cs)                                          |
|                                | FunctionResult_Metadata                       | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/FunctionResult_Metadata.cs)                            |
|                                | FunctionResult_StronglyTyped                  | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/FunctionResult_StronglyTyped.cs)                       |
|                                | MethodFunctions                               | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/MethodFunctions.cs)                                    |
|                                | MethodFunctions_Advanced                      | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/MethodFunctions_Advanced.cs)                           |
|                                | MethodFunctions_Types                         | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/MethodFunctions_Types.cs)                              |
|                                | PromptFunctions_Inline                        | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/PromptFunctions_Inline.cs)                             |
|                                | PromptFunctions_MultipleArguments             | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Functions/PromptFunctions_MultipleArguments.cs)                  |
| **Image to text**                  | HuggingFace_ImageToText                       | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ImageToText/HuggingFace_ImageToText.cs)                          |
| **Local models**                   | HuggingFace_ChatCompletionWithTGI             | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/LocalModels/HuggingFace_ChatCompletionWithTGI.cs)                |
|                                | MultipleProviders_ChatCompletion              | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/LocalModels/MultipleProviders_ChatCompletion.cs)                 |
| **Memory**                         | HuggingFace_EmbeddingGeneration               | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/HuggingFace_EmbeddingGeneration.cs)                       |
|                                | MemoryStore_CustomReadOnly                    | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/MemoryStore_CustomReadOnly.cs)                            |
|                                | SemanticTextMemory_Building                   | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/SemanticTextMemory_Building.cs)                           |
|                                | TextChunkerUsage                              | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/TextChunkerUsage.cs)                                      |
|                                | TextChunkingAndEmbedding                      | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/TextChunkingAndEmbedding.cs)                              |
|                                | TextMemoryPlugin_GeminiEmbeddingGeneration    | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/TextMemoryPlugin_GeminiEmbeddingGeneration.cs)            |
|                                | TextMemoryPlugin_MultipleMemoryStore          | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Memory/TextMemoryPlugin_MultipleMemoryStore.cs)                  |
| **Planning**                       | FunctionCallStepwisePlanning                  | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Planners/FunctionCallStepwisePlanning.cs)                        |
|                                | HandlebarsPlanning                            | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Planners/HandlebarsPlanning.cs)                                  |
| **Plugins**                        | ApiManifestBasedPlugins                       | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/ApiManifestBasedPlugins.cs)                              |
|                                | ConversationSummaryPlugin                     | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/ConversationSummaryPlugin.cs)                            |
|                                | CreatePluginFromOpenAI_AzureKeyVault          | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/CreatePluginFromOpenAI_AzureKeyVault.cs)                 |
|                                | CreatePluginFromOpenApiSpec_Github            | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/CreatePluginFromOpenApiSpec_Github.cs)                   |
|                                | CreatePluginFromOpenApiSpec_Jira              | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/CreatePluginFromOpenApiSpec_Jira.cs)                     |
|                                | CustomMutablePlugin                           | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/CustomMutablePlugin.cs)                                  |
|                                | DescribeAllPluginsAndFunctions                | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/DescribeAllPluginsAndFunctions.cs)                       |
|                                | GroundednessChecks                            | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/GroundednessChecks.cs)                                   |
|                                | ImportPluginFromGrpc                          | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/ImportPluginFromGrpc.cs)                                 |
|                                | OpenAIPlugins                                 | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Plugins/OpenAIPlugins.cs)                                        |
| **Prompt templates**               | ChatCompletionPrompts                         | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/ChatCompletionPrompts.cs)                        |
|                                | ChatWithPrompts                               | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/ChatWithPrompts.cs)                              |
|                                | LiquidPrompts                                 | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/LiquidPrompts.cs)                                |
|                                | MultiplePromptTemplates                       | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/MultiplePromptTemplates.cs)                      |
|                                | PromptFunctionsWithChatGPT                    | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/PromptFunctionsWithChatGPT.cs)                   |
|                                | TemplateLanguage                              | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/TemplateLanguage.cs)                             |
|                                | PromptyFunction                               | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/PromptTemplates/PromptyFunction.cs)                              |
| **Retrieval augmented generation** | WithFunctionCallingStepwisePlanner            | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/RAG/WithFunctionCallingStepwisePlanner.cs)                       |
|                                | WithPlugins                                   | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/RAG/WithPlugins.cs)                                              |
| Search                         | BingAndGooglePlugins                          | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Search/BingAndGooglePlugins.cs)                                  |
|                                | MyAzureAISearchPlugin                         | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Search/MyAzureAISearchPlugin.cs)                                 |
|                                | WebSearchQueriesPlugin                        | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Search/WebSearchQueriesPlugin.cs)                                |
| **Text generation**                | Custom_TextGenerationService                  | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/TextGeneration/Custom_TextGenerationService.cs)                  |
|                                | HuggingFace_TextGeneration                    | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/TextGeneration/HuggingFace_TextGeneration.cs)                    |
|                                | OpenAI_TextGenerationStreaming                | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/TextGeneration/OpenAI_TextGenerationStreaming.cs)                |
| **Text to audio**                  | OpenAI_TextToAudio                            | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/TextToAudio/OpenAI_TextToAudio.cs)                               |
| **Text to image**                  | OpenAI_TextToImage                            | [Link](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/TextToImage/OpenAI_TextToImageDalle3.cs)                         |

::: zone-end