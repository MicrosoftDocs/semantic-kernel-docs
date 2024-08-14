---
title: Supported languages for Semantic Kernel
description: Learn which features are available for C#, Python, and Java.
author: matthewbolanos
ms.topic: reference
ms.author: mabolan
ms.date: 07/11/2023
ms.service: semantic-kernel
---

# Supported Semantic Kernel languages

Semantic Kernel plans on providing support to the following languages:
> [!div class="checklist"]
> * C#
> * Python
> * Java

While the overall architecture of the kernel is consistent across all languages, we made sure the SDK for each language follows common paradigms and styles in each language to make it feel native and easy to use.

## Available SDK packages

### C# packages

In C#, there are several packages to help ensure that you only need to import the functionality that you need for your project. The following table shows the available packages in C#.

| Package name | Description | 
|--------------|-------------|
| `Microsoft.SemanticKernel` | The main package that includes everything to get started |
| `Microsoft.SemanticKernel.Core` | The core package that provides implementations for `Microsoft.SemanticKernel.Abstractions` |
| `Microsoft.SemanticKernel.Abstractions` | The base abstractions for Semantic Kernel |
| `Microsoft.SemanticKernel.Connectors.OpenAI` | The connector for OpenAI |
| `Microsoft.SemanticKernel.Connectors.HuggingFace` | The connector for Hugging Face models |
| `Microsoft.SemanticKernel.Connectors.Google` | The connector for Google models (e.g., Gemini) |
| `Microsoft.SemanticKernel.Connectors.MistralAI` | The connector for Mistral AI models |
| `Microsoft.SemanticKernel.Plugins.OpenApi` (Experimental) | Enables loading plugins from OpenAPI specifications |
| `Microsoft.SemanticKernel.PromptTemplates.Handlebars` | Enables the use of Handlebars templates for prompts |
| `Microsoft.SemanticKernel.Yaml` | Provides support for serializing prompts using YAML files |
| `Microsoft.SemanticKernel.Prompty` | Provides support for serializing prompts using Prompty files |
| `Microsoft.SemanticKernel.Agents.Abstractions` | Provides abstractions for creating agents |
| `Microsoft.SemanticKernel.Agents.OpenAI` | Provides support for Assistant API agents |

There are other packages available (e.g., the memory connectors), but they are still experimental and are not yet recommended for production use.

To install any of these packages, you can use the following command:

```bash
dotnet add package <package-name>
```

### Python packages

In Python, there's a single package that includes everything you need to get started with Semantic Kernel. To install the package, you can use the following command:

```bash
pip install semantic-kernel
```

### Java packages

For Java, Semantic Kernel has the following packages; all are under the group Id `com.microsoft.semantic-kernel`, and can be imported
from maven.

```xml
    <dependency>
        <groupId>com.microsoft.semantic-kernel</groupId>
        <artifactId>semantickernel-api</artifactId>
    </dependency>
```

A BOM is provided that can be used to define the versions of all Semantic Kernel packages.

```xml
    <dependencyManagement>
        <dependencies>
            <dependency>
                <groupId>com.microsoft.semantic-kernel</groupId>
                <artifactId>semantickernel-bom</artifactId>
                <version>${semantickernel.version}</version>
                <scope>import</scope>
                <type>pom</type>
            </dependency>
        </dependencies>
    </dependencyManagement>
```

- `semantickernel-bom` – A Maven project BOM that can be used to define the versions of all Semantic Kernel packages.
- `semantickernel-api` – Package that defines the core public API for the Semantic Kernel for a Maven project.
- `semantickernel-aiservices-openai` –Provides a connector that can be used to interact with the OpenAI API.

Below is an example POM XML for a simple project that uses OpenAI.

```xml
<project>
    <dependencyManagement>
        <dependencies>
            <dependency>
                <groupId>com.microsoft.semantic-kernel</groupId>
                <artifactId>semantickernel-bom</artifactId>
                <version>${semantickernel.version}</version>
                <scope>import</scope>
                <type>pom</type>
            </dependency>
        </dependencies>
    </dependencyManagement>
    <dependencies>
        <dependency>
            <groupId>com.microsoft.semantic-kernel</groupId>
            <artifactId>semantickernel-api</artifactId>
        </dependency>
        <dependency>
            <groupId>com.microsoft.semantic-kernel</groupId>
            <artifactId>semantickernel-connectors-ai-openai</artifactId>
        </dependency>
    </dependencies>
</project>
```

## Available features in each SDK

The following tables show which features are available in each language. The 🔄 symbol indicates that the feature is partially implemented, please see the associated note column for more details. The ❌ symbol indicates that the feature is not yet available in that language; if you would like to see a feature implemented in a language, please consider [contributing to the project](./contributing.md) or [opening an issue](./contributing.md#reporting-issues).


### Core capabilities

| Services                          |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| Prompts                           | ✅ | ✅ | ✅ | To see the full list of supported template and serialization formats, refer to the tables below |
| Native functions and plugins      | ✅ | ✅ | ✅ | |
| OpenAPI plugins                   | ✅ | ✅ | ✅ | Java has a sample demonstrating how to load OpenAPI plugins |
| Automatic function calling        | ✅ | ✅ | ✅ | |
| Open Telemetry logs               | ✅ | 🔄 | ❌ | |
| Hooks and filters                 | ✅ | ✅ | ✅ | |

### Prompt template formats

When authoring prompts, Semantic Kernel provides a variety of template languages that allow you to embed variables and invoke functions. The following table shows which template languages are supported in each language.

| Formats                          |  C#  | Python | Java | Notes |
| ---------------------------------|:----:|:------:|:----:|-------|
| Semantic Kernel template language | ✅ | ✅ | ✅ | |
| Handlebars                        | ✅ | ✅ | ✅ | |
| Liquid                            | ✅ | ❌ | ❌ | |
| Jinja2                            | ❌ | ✅ | ❌ | |

### Prompt serialization formats

Once you've created a prompt, you can serialize it so that it can be stored or shared across teams. The following table shows which serialization formats are supported in each language.

| Formats                          |  C#  | Python | Java | Notes |
| ---------------------------------|:----:|:------:|:----:|-------|
| YAML                             | ✅ | ✅ | ✅ | |
| Prompty                          | ❌ | ✅ | ❌ | |

### AI Services Modalities

| Services                          |  C#  | Python | Java | Notes |
|-----------------------------------|:----:|:------:|:----:|-------|
| Text Generation                    | ✅ | ✅ | ✅ | Example: Text-Davinci-003 |
| Chat Completion                    | ✅ | ✅ | ✅ | Example: GPT4, Chat-GPT |
| Text Embeddings (Experimental)     | ✅ | ✅ | ✅ | Example: Text-Embeddings-Ada-002 |
| Text to Image (Experimental)       | ✅ | ✅ | ❌ | Example: Dall-E |
| Image to Text (Experimental)       | ✅ | ✅  | ❌ | Example: Pix2Struct |
| Text to Audio (Experimental)       | ✅ | ❌ | ❌ | Example: Text-to-speech |
| Audio to Text (Experimental)       | ✅ | ❌ | ❌ | Example: Whisper |

### AI Service Connectors

| Endpoints                                 |  C#  | Python | Java | Notes |
|-------------------------------------------|:----:|:------:|:----:|-------|
| OpenAI                                    | ✅ | ✅ | ✅ | |
| Azure OpenAI                              | ✅ | ✅ | ✅ | |
| Other endpoints that suppoprt OpenAI APIs | ✅ | ✅ | ✅ | Includes Ollama, LLM Studio, Azure Model-as-a-service, etc. |
| Hugging Face Inference API                | 🔄 | ❌ | ❌ | Coming soon to Python, not all scenarios are covered for .NET |

### Memory Connectors (Experimental)

> [!IMPORTANT]
> All of the existing memory connectors are currently experimental and will be replaced by Vector Store connectors. These will provide more functionality via an updated abstraction layer.

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

### Vector Store Connectors (Experimental)

> [!IMPORTANT]
> All of the existing Vector Store connectors are currently experimental and are undergoing active development to improve the experience of using them. To provide feedback on the latest proposal, please refer to the active [Search](https://github.com/microsoft/semantic-kernel/pull/6012) and [Memory Connector](https://github.com/microsoft/semantic-kernel/pull/6364) ADRs.

For the list of out of the box vector store connectors and the language support for each, refer to [out of the box connectors](../concepts/vector-store-connectors/out-of-the-box-connectors/index.md).
