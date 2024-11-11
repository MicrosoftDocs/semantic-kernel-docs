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

On [PyPI](https://pypi.org/project/semantic-kernel/) under `Provides-Extra` the additional extras you can install are also listed and when used that will install the packages needed for using SK with that specific connector or service, you can install those with the square bracket syntax for instance:
```bash
pip install semantic-kernel[azure]
```
This will install Semantic Kernel, as well as specific tested versions of: `azure-ai-inference`, `azure-search-documents`, `azure-core`, `azure-identity`, `azure-cosmos` and `msgraph-sdk` (and any dependencies of those packages). Similarly the extra `hugging_face` will install `transformers` and `sentence-transformers`.

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
| Amazon Bedrock                            | ✅ | ✅ | ❌ | |
| Anthropic                                 | ✅ | ✅ | ❌ | |
| Azure AI Inference                        | ✅ | ✅ | ❌ | |
| Azure OpenAI                              | ✅ | ✅ | ✅ | |
| Google                                    | ✅ | ✅ | ✅ | |
| Hugging Face Inference API                | ✅ | ✅ | ❌ | |
| Mistral                                   | ✅ | ✅ | ❌ | |
| Ollama                                    | ✅ | ✅ | ❌ | |
| ONNX                                      | ✅ | ✅ | ❌ | |
| OpenAI                                    | ✅ | ✅ | ✅ | |
| Other endpoints that suppoprt OpenAI APIs | ✅ | ✅ | ✅ | Includes LLM Studio, Azure Model-as-a-service, etc. |

### Vector Store Connectors (Experimental)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

For the list of out of the box vector store connectors and the language support for each, refer to [out of the box connectors](../concepts/vector-store-connectors/out-of-the-box-connectors/index.md).

### Memory Store Connectors (Legacy)

> [!IMPORTANT]
> Memory Store connectors are legacy and have been replaced by Vector Store connectors. For more information see [Legacy Memory Stores](../concepts/vector-store-connectors/memory-stores.md).

| Memory Connectors        |  C#  | Python | Java | Notes |
|--------------------------|:----:|:------:|:----:|-------|
| Azure AI Search          | ✅ | ✅ | ✅ | |
| Chroma                   | ✅ | ✅ | ❌ | |
| DuckDB                   | ✅ | ❌ | ❌ | |
| Milvus                   | ✅ | ✅ | ❌ | |
| Pinecone                 | ✅ | ✅ | ❌ | |
| Postgres                 | ✅ | ✅ | ❌ | |
| Qdrant                   | ✅ | 🔄 | ❌ | |
| Redis                    | ✅ | 🔄 | ❌ | |
| Sqlite                   | ✅ | ❌ | 🔄 | |
| Weaviate                 | ✅ | ✅ | ❌ | |
