---
title: AzureAIAgent Foundry GA Migration Guide
description: Describes the steps for developers to update their usage of AzureAIAgent for a GA Foundry Project.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: conceptual
ms.author: crickman
ms.date: 05/16/2025
ms.service: semantic-kernel
---

# `AzureAIAgent` Foundry GA Migration Guide

::: zone pivot="programming-language-csharp"

In Semantic Kernel .NET 1.53.1+, .NET and Python developers using `AzureAIAgent` must to update the patterns they use to interact with the Azure AI Foundry in response to its move to GA.

### GA Foundry Project

- Must be created on or after May 19th, 2025
- Connect programatically using the _Foundry Project's_ endpoint url.
- Requires Semantic Kernel version 1.53.1 and above.
- Based on package [Azure.AI.Agents.Persistent](https://www.nuget.org/packages/Azure.AI.Agents.Persistent/1.0.0)

### Pre-GA Foundry Project

- Was created prior to May 19th, 2025
- Connect programatically using the _Foundry Project's_ connection string.
- Continue to use Semantic Kernel versions below version 1.53.*
- Based on package [Azure.AI.Projects version 1.0.0-beta.8](https://www.nuget.org/packages/Azure.AI.Projects/1.0.0-beta.8)


## Creating an Client

### Old Way

```c#
AIProjectClient client = AzureAIAgent.CreateAzureAIClient("<connection string>", new AzureCliCredential());
AgentsClient agentsClient = client.GetAgentsClient();
```

### New Way

``` c#
PersistentAgentsClient agentsClient = AzureAIAgent.CreateAgentsClient("<endpoint>", new AzureCliCredential());```
```

## Creating an Agent

### Old Way

```c#
Agent agent = await agentsClient.CreateAgentAsync(...);
```

### New Way

``` c#
PersistentAgent agent = await agentsClient.Administration.CreateAgentAsync(
```

## Deleting an Agent

### Old Way

```c#
await agentsClient.DeleteAgentAsync("<agent id>");
```

### New Way

``` c#
await agentsClient.Administration.DeleteAgentAsync("<agent id>");
```

## Uploading Files

### Old Way

```c#
AgentFile fileInfo = await agentsClient.UploadFileAsync(stream, AgentFilePurpose.Agents, "<file name>");
```

### New Way

``` c#
PersistentAgentFileInfo fileInfo = await agentsClient.Files.UploadFileAsync(stream, PersistentAgentFilePurpose.Agents, "<file name>");
```

## Deleting Files

### Old Way

```c#
await agentsClient.DeleteFileAsync("<file id>");
```

### New Way

``` c#
await agentsClient.Files.DeleteFileAsync("<file id>");
```

## Creating a VectorStore

### Old Way

```c#
VectorStore fileStore = await agentsClient.CreateVectorStoreAsync(...);
```

### New Way

``` c#
PersistentAgentsVectorStore fileStore = await agentsClient.VectorStores.CreateVectorStoreAsync(...);
```

## Deleting a VectorStore

### Old Way

```c#
await agentsClient.DeleteVectorStoreAsync("<store id>");
```

### New Way

``` c#
await agentsClient.VectorStores.DeleteVectorStoreAsync("<store id>");
```

::: zone-end


::: zone pivot="programming-language-python"

## `AzureAIAgent`

In Semantic Kernel Python 1.31.0 and above, `AzureAIAgent` support has been updated to align with Azure AI Foundry's general availability. This update requires several changes for developers integrating with Foundry projects.

The `project_connection_string` and its environment variable `AZURE_AI_AGENT_PROJECT_CONNECTION_STRING` are no longer supported. Instead, you must configure an `endpoint` using the `AZURE_AI_AGENT_ENDPOINT` environment variable. The endpoint is found on your Azure Foundry project page and follows this format: `https://<resource>.services.ai.azure.com/api/projects/<project-name>`

Tool-related imports have also changed. Tools that were previously imported from `azure.ai.projects.models` must now be imported from `azure.ai.agents.models`. Refer to the updated [AzureAIAgent getting started samples](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/getting_started_with_agents/azure_ai_agent) or [AzureAIAgent concept samples](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/concepts/agents/azure_ai_agent) for current patterns.

Follow the setup instructions in the official documentation: [Azure AI Agents Quickstart](/azure/ai-services/agents/quickstart?pivots=ai-foundry-portal).

### GA Foundry Project

- Must be created on or after May 19, 2025.
- Connect programmatically using the Foundry project's endpoint URL.
- Requires Semantic Kernel version 1.31.0 or higher.
- Uses packages `azure-ai-projects` version 1.0.0b11 or higher and `azure-ai-agents` version 1.0.0 or higher, installed via `pip install semantic-kernel[azure]`.

### Pre-GA Foundry Project

- Was created before May 19, 2025.
- Connect programmatically using the Foundry project's connection string.
- Requires Semantic Kernel versions below 1.31.0.
- Uses package `azure-ai-projects` version 1.0.0b10 or lower.

Updated Imports for Tools:

### Old Way

```python
from azure.ai.projects.models import CodeInterpreterTool, FileSearchTool, OpenApiAnonymousAuthDetails, OpenApiTool
```

##### New Way

```python
from azure.ai.agents.models import CodeInterpreterTool, FileSearchTool, OpenApiAnonymousAuthDetails, OpenApiTool
```

Uploading Files:

### Old Way

```python
from azure.ai.projects.models import FilePurpose

file = await client.agents.upload_file_and_poll(file_path="<file-path>", purpose=FilePurpose.AGENTS)
```

##### New Way

```python
from azure.ai.agents.models import FilePurpose

file = await client.agents.files.upload_and_poll(file_path="<file-path>", purpose=FilePurpose.AGENTS)
```

Deleting Files:

### Old Way

```python
await client.agents.delete_file(file.id)
```

##### New Way

```python
await client.agents.files.delete(file.id)
```

Creating Vector Stores:

### Old Way

```python
from azure.ai.projects.models import VectorStore

vector_store: VectorStore = await client.agents.create_vector_store_and_poll(
    file_ids=[file.id], name="<vector-store-name>"
)
```

##### New Way

```python
from azure.ai.agents.models import VectorStore

vector_store: VectorStore = await client.agents.vector_stores.create_and_poll(
    file_ids=[file.id], name="<vector-store-name>"
)
```

Deleting Vector Stores:

### Old Way

```python
from azure.ai.projects.models import VectorStore

await client.agents.delete_vector_store(vector_store.id)
```

##### New Way

```python
from azure.ai.agents.models import VectorStore

await client.agents.vector_stores.delete(vector_store.id)
```

::: zone-end

::: zone pivot="programming-language-java"

> AzureAIAgent is currently unavailable in Java.

::: zone-end
