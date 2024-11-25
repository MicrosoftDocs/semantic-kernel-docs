---
title: AI Integrations for Semantic Kernel 
description: Learn which features are available for C#, Python, and Java through integrations.
author: sophialagerkranspandey
ms.topic: reference
ms.author: sopand
ms.date: 07/11/2023
ms.service: semantic-kernel
---

# AI Integrations for Semantic Kernel

Semantic Kernel provides a wide range of AI service integrations to help you build powerful AI agents. Additionally, Semantic Kernel integrates with other Microsoft services to provide additional functionality via plugins.

## Out-of-the-box integrations

With the available AI connectors, developers can easily build AI agents with swappable components. This allows you to experiment with different AI services to find the best combination for your use case.

### AI Services

| Services                          |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| Text Generation                    | ✅ | ✅ | ✅ | Example: Text-Davinci-003 |
| Chat Completion                    | ✅ | ✅ | ✅ | Example: GPT4, Chat-GPT |
| Text Embeddings (Experimental)     | ✅ | ✅ | ✅ | Example: Text-Embeddings-Ada-002 |
| Text to Image (Experimental)       | ✅ | ✅ | ❌ | Example: Dall-E |
| Image to Text (Experimental)       | ✅ | ❌ | ❌ | Example: Pix2Struct |
| Text to Audio (Experimental)       | ✅ | ✅ | ❌ | Example: Text-to-speech |
| Audio to Text (Experimental)       | ✅ | ✅ | ❌ | Example: Whisper |

## Additional plugins

If you want to extend the functionality of your AI agent, you can use plugins to integrate with other Microsoft services. Here are some of the plugins that are available for Semantic Kernel:

| Plugin     | C#  | Python | Java | Description |
| ---------- | :-: | :----: | :--: | ----------- |
| Logic Apps | ✅  |   ✅   |  ✅  | Build workflows within Logic Apps using its available connectors and import them as plugins in Semantic Kernel. [Learn more](../plugins/adding-logic-apps-as-plugins.md). |
| Azure Container Apps Dynamic Sessions | ✅  |   ✅   |  ❌  | With dynamic sessions, you can recreate the Code Interpreter experience from the Assistants API by effortlessly spinning up Python containers where AI agents can execute Python code. [Learn more](/azure/container-apps/sessions). |
