---
title: Agent Framework Release Candidate Migration Guide
description: Describes the steps for developers to update their Agent Framework code to the latest abstractions.
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: conceptual
ms.author: evmattso
ms.date: 02/26/2025
ms.service: semantic-kernel
---

# Agent Framework Release Candidate Migration Guide

As we transition some agents from the experimental stage to the release candidate stage, we have updated the APIs to simplify and streamline their use. Refer to the specific scenario guide to learn how to update your existing code to work with the latest available APIs.

::: zone pivot="programming-language-csharp"

##  OpenAIAssistantAgent C# Migration Guide

We recently applied a significant shift around the [`OpenAIAssistantAgent`](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Agents/OpenAI/OpenAIAssistantAgent.cs) in the _Semantic Kernel Agent Framework_.

These changes were applied in:

- [PR #10583](https://github.com/microsoft/semantic-kernel/pull/10583)
- [PR #10616](https://github.com/microsoft/semantic-kernel/pull/10616)
- [PR #10633](https://github.com/microsoft/semantic-kernel/pull/10633)

These changes are intended to:

- Align with the pattern for using for our [`AzureAIAgent`](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Agents/AzureAI/AzureAIAgent.cs).
- Fix bugs around static initialization pattern.
- Avoid limiting features based on our abstraction of the underlying SDK.

This guide provides step-by-step instructions for migrating your C# code from the old implementation to the new one. Changes include updates for creating assistants, managing the assistant lifecycle, handling threads, files, and vector stores.

## 1. Client Instantiation

Previously, `OpenAIClientProvider` was required for creating any `OpenAIAssistantAgent`. This dependency has been simplified.

#### **New Way**
```csharp
OpenAIClient client = OpenAIAssistantAgent.CreateAzureOpenAIClient(new AzureCliCredential(), new Uri(endpointUrl));
AssistantClient assistantClient = client.GetAssistantClient();
```

#### **Old Way (Deprecated)**
```csharp
var clientProvider = new OpenAIClientProvider(...);
```

## 2. Assistant Lifecycle

### **Creating an Assistant**
You may now directly instantiate an `OpenAIAssistantAgent` using an existing or new Assistant definition from `AssistantClient`.

##### **New Way**
```csharp
Assistant definition = await assistantClient.GetAssistantAsync(assistantId);
OpenAIAssistantAgent agent = new(definition, client);
```

Plugins can be directly included during initialization:
```csharp
KernelPlugin plugin = KernelPluginFactory.CreateFromType<YourPlugin>();
Assistant definition = await assistantClient.GetAssistantAsync(assistantId);
OpenAIAssistantAgent agent = new(definition, client, [plugin]);
```

Creating a new assistant definition using an extension method:
```csharp
Assistant assistant = await assistantClient.CreateAssistantAsync(
    model,
    name,
    instructions: instructions,
    enableCodeInterpreter: true);
```

##### **Old Way (Deprecated)**
Previously, assistant definitions were managed indirectly.

## 3. Invoking the Agent

You may specify `RunCreationOptions` directly, enabling full access to underlying SDK capabilities.

#### **New Way**
```csharp
RunCreationOptions options = new(); // configure as needed
var result = await agent.InvokeAsync(options);
```

#### **Old Way (Deprecated)**
```csharp
var options = new OpenAIAssistantInvocationOptions();
```

## 4. Assistant Deletion

You can directly manage assistant deletion with `AssistantClient`.

```csharp
await assistantClient.DeleteAssistantAsync(agent.Id);
```

## 5. Thread Lifecycle

### **Creating a Thread**
Threads are now created directly using `AssistantClient`.

##### **New Way**
```csharp
AssistantThread thread = await assistantClient.CreateThreadAsync();
```

Using a convenience extension method:
```csharp
string threadId = await assistantClient.CreateThreadAsync(messages: [new ChatMessageContent(AuthorRole.User, "<message content>")]);
```

##### **Old Way (Deprecated)**
Previously, thread management was indirect or agent-bound.

### **Thread Deletion**
```csharp
await assistantClient.DeleteThreadAsync(thread.Id);
```

## 6. File Lifecycle

File creation and deletion now utilize `OpenAIFileClient`.

### **File Upload**
```csharp
string fileId = await client.UploadAssistantFileAsync(stream, "<filename>");
```

### **File Deletion**
```csharp
await client.DeleteFileAsync(fileId);
```

## 7. Vector Store Lifecycle

Vector stores are managed directly via `VectorStoreClient` with convenient extension methods.

### **Vector Store Creation**
```csharp
string vectorStoreId = await client.CreateVectorStoreAsync([fileId1, fileId2], waitUntilCompleted: true);
```

### **Vector Store Deletion**
```csharp
await client.DeleteVectorStoreAsync(vectorStoreId);
```

## Backwards Compatibility

Deprecated patterns are marked with `[Obsolete]`. To suppress obsolete warnings (`CS0618`), update your project file as follows:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);CS0618</NoWarn>
</PropertyGroup>
```

This migration guide helps you transition smoothly to the new implementation, simplifying client initialization, resource management, and integration with the **Semantic Kernel .NET SDK**.

::: zone-end
::: zone pivot="programming-language-python"

For developers upgrading to Semantic Kernel Python 1.22.0 or later, the ChatCompletionAgent and OpenAI Assistant abstractions have been updated.

These changes were applied in:

- [PR #10666](https://github.com/microsoft/semantic-kernel/pull/10666)
- [PR #10667](https://github.com/microsoft/semantic-kernel/pull/10667)
- [PR #10701](https://github.com/microsoft/semantic-kernel/pull/10701)
- [PR #10707](https://github.com/microsoft/semantic-kernel/pull/10707)

This guide provides step-by-step instructions for migrating your Python code from the old implementation to the new implementation.

## `ChatCompletionAgent`

The `ChatCompletionAgent` has been updated to simplify service configuration, plugin handling, and function calling behaviors. Below are the key changes you should consider when migrating.

### 1. Specifying the Service

You can now specify the service directly as part of the agent constructor:

#### New Way

```python
agent = ChatCompletionAgent(
    service=AzureChatCompletion(),
    name="<name>",
    instructions="<instructions>",
)
```

Note: If both a kernel and a service are provided, the service will take precedence if it shares the same service_id or ai_model_id. Otherwise, if they are separate, the first AI service registered on the kernel will be used.

#### Old Way (Still Valid)

Previously, you would first add a service to a kernel and then pass the kernel to the agent:

```python
kernel = Kernel()
kernel.add_service(AzureChatCompletion())

agent = ChatCompletionAgent(
    kernel=kernel,
    name="<name>",
    instructions="<instructions>",
)
```

### 2. Adding Plugins

Plugins can now be supplied directly through the constructor:

#### New Way

```python
agent = ChatCompletionAgent(
    service=AzureChatCompletion(),
    name="<name>",
    instructions="<instructions>",
    plugins=[SamplePlugin()],
)
```

#### Old Way (Still Valid)

Plugins previously had to be added to the kernel separately:

```python
kernel = Kernel()
kernel.add_plugin(SamplePlugin())

agent = ChatCompletionAgent(
    kernel=kernel,
    name="<name>",
    instructions="<instructions>",
)
```

Note: Both approaches are valid, but directly specifying plugins simplifies initialization.

### 3. Invoking the Agent

You now have two ways to invoke the agent. The new method directly retrieves a single response, while the old method supports streaming.

#### New Way (Single Response)

```python
chat_history = ChatHistory()
chat_history.add_user_message("<user_input>")
response = await agent.get_response(chat_history)
# response is of type ChatMessageContent
```

#### Old Way (Still Valid)

```python
chat_history = ChatHistory()
chat_history.add_user_message("<user_input>")
async for response in agent.invoke(chat_history):
    # handle response
```

### 4. Controlling Function Calling

Function calling behavior can now be controlled directly when specifying the service within the agent constructor:

```python
agent = ChatCompletionAgent(
    service=AzureChatCompletion(),
    name="<name>",
    instructions="<instructions>",
    plugins=[MenuPlugin()],
    function_choice_behavior=FunctionChoiceBehavior.Auto(
        filters={"included_functions": ["get_specials", "get_item_price"]}
    ),
)
```

Note: Previously, function calling configuration required separate setup on the kernel or service object. If execution settings specify the same `service_id` or `ai_model_id` as the AI service configuration, the function calling behavior defined in the execution settings (via `KernelArguments`) will take precedence over the function choice behavior set in the constructor.

These updates enhance simplicity and configurability, making the ChatCompletionAgent easier to integrate and maintain.

## `OpenAIAssistantAgent`

The `AzureAssistantAgent` and `OpenAIAssistantAgent` changes include updates for creating assistants, creating threads, handling plugins, using the code interpreter tool, working with the file search tool, and adding chat messages to a thread.

## Setting up Resources

### Old Way

The `AsyncAzureOpenAI` client was created as part of creating the Agent object.

```python
agent = await AzureAssistantAgent.create(
    deployment_name="optional-deployment-name",
    api_key="optional-api-key",
    endpoint="optional-endpoint",
    ad_token="optional-ad-token",
    ad_token_provider=optional_callable,
    default_headers={"optional_header": "optional-header-value"},
    env_file_path="optional-env-file-path",
    env_file_encoding="optional-env-file-encoding",
    ...,
)
```

### New Way

The agent provides a static method to create the required client for the specified resources, where method-level keyword arguments take precedence over environment variables and values in an existing `.env` file.

```python
client, model = AzureAssistantAgent.setup_resources(
    ad_token="optional-ad-token",
    ad_token_provider=optional_callable,
    api_key="optional-api-key",
    api_version="optional-api-version",
    base_url="optional-base-url",
    default_headers="optional-default-headers",
    deployment_name="optional-deployment-name",
    endpoint="optional-endpoint",
    env_file_path="optional-env-file-path",
    env_file_encoding="optional-env-file-encoding",
    token_scope="optional-token-scope",
)
```

## 1. Creating an Assistant

### Old Way
```python
agent = await AzureAssistantAgent.create(
    kernel=kernel,
    service_id=service_id,
    name=AGENT_NAME,
    instructions=AGENT_INSTRUCTIONS,
    enable_code_interpreter=True,
)
```
or
```python
agent = await OpenAIAssistantAgent.create(
    kernel=kernel,
    service_id=service_id,
    name=<name>,
    instructions=<instructions>,
    enable_code_interpreter=True,
)
```

### New Way
```python
# Azure AssistantAgent 

# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    instructions="<instructions>",
    name="<name>",
)

# Create the agent using the client and the assistant definition
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
)
```
or
```python
# OpenAI Assistant Agent

# Create the client using OpenAI resources and configuration
client, model = OpenAIAssistantAgent.setup_resources()

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    instructions="<instructions>",
    name="<name>",
)

# Create the agent using the client and the assistant definition
agent = OpenAIAssistantAgent(
    client=client,
    definition=definition,
)
```

## 2. Creating a Thread

### Old Way
```python
thread_id = await agent.create_thread()
```

### New Way
```python
thread = await agent.client.beta.threads.create()
# Use thread.id for the thread_id string
```

## 3. Handling Plugins

### Old Way
```python
# Create the instance of the Kernel
kernel = Kernel()

# Add the sample plugin to the kernel
kernel.add_plugin(plugin=MenuPlugin(), plugin_name="menu")

agent = await AzureAssistantAgent.create(
    kernel=kernel, 
    name="<name>", 
    instructions="<instructions>"
)
```
*Note: It is still possible to manage plugins via the kernel. If you do not supply a kernel, a kernel is automatically created at agent creation time and the plugins will be added to that instance.*

### New Way
```python
# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    instructions="<instructions>",
    name="<name>",
)

# Create the agent with plugins passed in as a list
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
    plugins=[MenuPlugin()],
)
```

Refer to the [sample implementation](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/getting_started_with_agents/openai_assistant/step2_plugins.py) for full details.

## 4. Using the Code Interpreter Tool

### Old Way
```python
csv_file_path = ...

agent = await AzureAssistantAgent.create(
    kernel=kernel,
    name="<name>",
    instructions="<instructions>",
    enable_code_interpreter=True,
    code_interpreter_filenames=[csv_file_path],
)
```

### New Way
```python
# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

csv_file_path = ...

# Load the CSV file as a FileObject
with open(csv_file_path, "rb") as file:
    file = await client.files.create(file=file, purpose="assistants")

# Get the code interpreter tool and resources
code_interpreter_tool, code_interpreter_tool_resource = AzureAssistantAgent.configure_code_interpreter_tool(file.id)

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    name="<name>",
    instructions="<instructions>.",
    tools=code_interpreter_tool,
    tool_resources=code_interpreter_tool_resource,
)

# Create the AzureAssistantAgent instance using the client and the assistant definition
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
)
```

Refer to the [sample implementation](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/openai_assistant/openai_assistant_file_manipulation.py) for full details.

## 5. Working with the File Search Tool

### Old Way
```python
pdf_file_path = ...

agent = await AzureAssistantAgent.create(
    kernel=kernel,
    service_id=service_id,
    name=AGENT_NAME,
    instructions=AGENT_INSTRUCTIONS,
    enable_file_search=True,
    vector_store_filenames=[pdf_file_path],
)
```

### New Way

```python
# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

pdf_file_path = ...

# Load the employees PDF file as a FileObject
with open(pdf_file_path, "rb") as file:
    file = await client.files.create(file=file, purpose="assistants")

# Create a vector store specifying the file ID to be used for file search
vector_store = await client.beta.vector_stores.create(
    name="step4_assistant_file_search",
    file_ids=[file.id],
)

file_search_tool, file_search_tool_resources = AzureAssistantAgent.configure_file_search_tool(vector_store.id)

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    instructions="Find answers to the user's questions in the provided file.",
    name="FileSearch",
    tools=file_search_tool,
    tool_resources=file_search_tool_resources,
)

# Create the AzureAssistantAgent instance using the client and the assistant definition
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
)
```

Refer to the [sample implementation](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/getting_started_with_agents/openai_assistant/step4_assistant_tool_file_search.py) for full details.

## 6. Adding Chat Messages to a Thread

### Old Way
```python
await agent.add_chat_message(
    thread_id=thread_id, 
    message=ChatMessageContent(role=AuthorRole.USER, content=user_input)
)
```

### New Way
*Note: The old method still works if you pass in a `ChatMessageContent`, but you can now also pass a simple string.*
```python
await agent.add_chat_message(
    thread_id=thread_id, 
    message=user_input,
)
```

## 7. Cleaning Up Resources

### Old Way
```python
await agent.delete_file(file_id)
await agent.delete_thread(thread_id)
await agent.delete()
```

### New Way
```python
await client.files.delete(file_id)
await client.beta.threads.delete(thread.id)
await client.beta.assistants.delete(agent.id)
```

## Handling Structured Outputs

### Old Way
*Unavailable in the old way*

### New Way
```python
# Define a Pydantic model that represents the structured output from the OpenAI service
class ResponseModel(BaseModel):
    response: str
    items: list[str]

# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

# Create the assistant definition
definition = await client.beta.assistants.create(
    model=model,
    name="<name>",
    instructions="<instructions>",
    response_format=AzureAssistantAgent.configure_response_format(ResponseModel),
)

# Create the AzureAssistantAgent instance using the client and the assistant definition
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
)
```
Refer to the [sample implementation](https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/agents/openai_assistant/openai_assistant_structured_outputs.py) for full details.

This migration guide should help you update your code to the new implementation, leveraging client-based configuration and enhanced features.

::: zone-end
::: zone pivot="programming-language-java"
> Agents are unavailable in Java.
::: zone-end
