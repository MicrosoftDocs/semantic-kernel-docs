---
title: integrations
description: Learn which features are available for C#, Python, and Java through integrations.
author: sophialagerkranspandey
ms.topic: reference
ms.author: sopand
ms.date: 07/11/2023
ms.service: semantic-kernel
---

# Integrations for Semantic Kernel 

Semantic Kernel provides a wide range of integrations to help you build powerful AI agents. These integrations include AI services, memory connectors. Additionally, Semantic Kernel integrates with other Microsoft services to provide additional functionality via plugins.

## Out-of-the-box integrations

With the available AI and memory connectors, developers can easily build AI agents with swappable components. This allows you to experiment with different AI services and memory connectors to find the best combination for your use case.

### AI Services

| Services                          |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| Text Generation                    | ✅ | ✅ | ✅ | Example: Text-Davinci-003 |
| Chat Completion                    | ✅ | ✅ | ✅ | Example: GPT4, Chat-GPT |
| Text Embeddings (Experimental)     | ✅ | ✅ | ✅ | Example: Text-Embeddings-Ada-002 |
| Text to Image (Experimental)       | ✅ | ❌ | ❌ | Example: Dall-E |
| Image to Text (Experimental)       | ✅ | ❌ | ❌ | Example: Pix2Struct |
| Text to Audio (Experimental)       | ✅ | ❌ | ❌ | Example: Text-to-speech |
| Audio to Text (Experimental)       | ✅ | ❌ | ❌ | Example: Whisper |


### Memory Connectors (Experimental)

> [!IMPORTANT]
> All of the existing memory connectors are currently experimental and are undergoing active development to improve the experience of using them. To provide feedback on the latest proposal, please refer to the active [Search](https://github.com/microsoft/semantic-kernel/pull/6012) and [Memory Connector](https://github.com/microsoft/semantic-kernel/pull/6364) ADRs.


| Memory Connectors        |  C#  | Python | Java | Notes |
|--------------------------|:----:|:------:|:----:|-------|
| Azure AI Search          | ✅ | ✅ | ✅ | |
| Chroma                   | ✅ | ✅ | ❌ | |
| DuckDB                   | ✅ | ❌ | ❌ | |
| Milvus                   | 🔄 | ✅ | ❌ | |
| Pinecone                 | ✅ | ✅ | ❌ | |
| Postgres                 | ✅ | ✅ | ❌ | |
| Qdrant                   | ✅ | 🔄 | ❌ | |
| Redis                    | ✅ | 🔄 | ❌ | |
| Sqlite                   | ✅ | ❌ | 🔄 | |
| Weaviate                 | ✅ | ✅ | ❌ | |


## Additional plugins

If you want to extend the functionality of your AI agent, you can use plugins to integrate with other Microsoft services. Here are some of the plugins that are available for Semantic Kernel:

| Plugin     | C#  | Python | Java | Description |
| ---------- | :-: | :----: | :--: | ----------- |
| Logic Apps | ✅  |   ✅   |  ✅  | Build workflows within Logic Apps using its available connectors and import them as plugins in Semantic Kernel. [Learn more](../concepts/plugins/adding-logic-apps-as-plugins.md). |
| Azure Container Apps Dynamic Sessions | ✅  |   ✅   |  ❌  | With dynamic sessions, you can recreate the Code Interpreter experience from the Assistants API by effortlessly spinning up Python containers where AI agents can execute Python code. [Learn more](https://learn.microsoft.com/en-us/azure/container-apps/sessions). |

## Next steps
Curious how to use these integrations in your AI agent? Check out the [use cases section](../use-cases/index.md) to see how other customers are leveraging Semantic Kernel to optimize their companies' workflows. 
