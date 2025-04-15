---
title: Understanding the kernel in Semantic Kernel
description: Learn about the central component of Semantic Kernel and how it works
zone_pivot_groups: programming-languages
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Understanding the kernel

The kernel is the central component of Semantic Kernel. At its simplest, the kernel is a Dependency Injection container that manages all of the services and plugins necessary to run your AI application. If you provide all of your services and plugins to the kernel, they will then be seamlessly used by the AI as needed.

## The kernel is at the center

Because the kernel has all of the services and plugins necessary to run both native code and AI services, it is used by nearly every component within the Semantic Kernel SDK to power your agents. This means that if you run any prompt or code in Semantic Kernel, the kernel will always be available to retrieve the necessary services and plugins.

![The kernel is at the center of everything in Semantic Kernel](../media/the-kernel-is-at-the-center-of-everything.png)

This is extremely powerful, because it means you as a developer have a single place where you can configure, and most importantly monitor, your AI agents. Take for example, when you invoke a prompt from the kernel. When you do so, the kernel will...

1. Select the best AI service to run the prompt.
2. Build the prompt using the provided prompt template.
3. Send the prompt to the AI service.
4. Receive and parse the response.
5. And finally return the response from the LLM to your application.

Throughout this entire process, you can create events and middleware that are triggered at each of these steps. This means you can perform actions like logging, provide status updates to users, and most importantly responsible AI. All from a single place.

## Build a kernel with services and plugins

Before building a kernel, you should first understand the two types of components that exist:

| Component | Description |
|---|---|
| **Services** | These consist of both AI services (e.g., chat completion) and other services (e.g., logging and HTTP clients) that are necessary to run your application. This was modelled after the Service Provider pattern in .NET so that we could support dependency injection across all languages. |
| **Plugins** | These are the components that are used by your AI services and prompt templates to perform work. AI services, for example, can use plugins to retrieve data from a database or call an external API to perform actions. |

::: zone pivot="programming-language-csharp"
To start creating a kernel, import the necessary packages at the top of your file:

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/LearnResources/MicrosoftLearn/UsingTheKernel.cs" id="NecessaryPackages":::

Next, you can add services and plugins. Below is an example of how you can add an Azure OpenAI chat completion, a logger, and a time plugin.

```csharp
// Create a kernel with a logger and Azure OpenAI chat completion service
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
builder.Plugins.AddFromType<TimePlugin>();
Kernel kernel = builder.Build();
```

::: zone-end

::: zone pivot="programming-language-python"
Import the necessary packages:

```python
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.core_plugins.time_plugin import TimePlugin
```

Next, you can create a kernel.

```python
# Initialize the kernel
kernel = Kernel()
```

Finally, you can add the necessary services and plugins. Below is an example of how you can add an Azure OpenAI chat completion, a logger, and a time plugin.

```python
# Add the Azure OpenAI chat completion service
kernel.add_service(AzureChatCompletion(model_id, endpoint, api_key))

# Add a plugin
kernel.add_plugin(
    TimePlugin(),
    plugin_name="TimePlugin",
)
```

## Creating a MCP Server from your Kernel

We now support creating a MCP server from the function you have registered in your Semantic Kernel instance.

In order to do this, you create your kernel as you normally would, and then you can create a MCP server from it.

```python
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.functions import kernel_function
from semantic_kernel.prompt_template import InputVariable, PromptTemplateConfig

kernel = Kernel()

@kernel_function()
def echo_function(message: str, extra: str = "") -> str:
    """Echo a message as a function"""
    return f"Function echo: {message} {extra}"

kernel.add_service(OpenAIChatCompletion(service_id="default"))
kernel.add_function("echo", echo_function, "echo_function")
kernel.add_function(
    plugin_name="prompt",
    function_name="prompt",
    prompt_template_config=PromptTemplateConfig(
        name="prompt",
        description="This is a prompt",
        template="Please repeat this: {{$message}} and this: {{$extra}}",
        input_variables=[
            InputVariable(
                name="message",
                description="This is the message.",
                is_required=True,
                json_schema='{ "type": "string", "description": "This is the message."}',
            ),
            InputVariable(
                name="extra",
                description="This is extra.",
                default="default",
                is_required=False,
                json_schema='{ "type": "string", "description": "This is the message."}',
            ),
        ],
    ),
)
server = kernel.as_mcp_server(server_name="sk")

```

The `server` object created above comes from the mcp package, you can extend it even further for instance by adding resources, or other features to it. And then you can bring it online, for instance to be used with Stdio:

```python
import anyio
from mcp.server.stdio import stdio_server

async def handle_stdin(stdin: Any | None = None, stdout: Any | None = None) -> None:
    async with stdio_server() as (read_stream, write_stream):
        await server.run(read_stream, write_stream, server.create_initialization_options())

anyio.run(handle_stdin)
```

Or with SSE:

```python
import uvicorn
from mcp.server.sse import SseServerTransport
from starlette.applications import Starlette
from starlette.routing import Mount, Route

sse = SseServerTransport("/messages/")

async def handle_sse(request):
    async with sse.connect_sse(request.scope, request.receive, request._send) as (read_stream, write_stream):
        await server.run(read_stream, write_stream, server.create_initialization_options())

starlette_app = Starlette(
    debug=True,
    routes=[
        Route("/sse", endpoint=handle_sse),
        Mount("/messages/", app=sse.handle_post_message),
    ],
)

uvicorn.run(starlette_app, host="0.0.0.0", port=8000)
```

### Exposing prompt templates as MCP Prompt

You can also leverage the different Semantic Kernel prompt templates by exposing them as MCP Prompts, like this:

```python
from semantic_kernel.prompt_template import InputVariable, KernelPromptTemplate, PromptTemplateConfig

prompt = KernelPromptTemplate(
    prompt_template_config=PromptTemplateConfig(
        name="release_notes_prompt",
        description="This creates the prompts for a full set of release notes based on the PR messages given.",
        template=template,
        input_variables=[
            InputVariable(
                name="messages",
                description="These are the PR messages, they are a single string with new lines.",
                is_required=True,
                json_schema='{"type": "string"}',
            )
        ],
    )
)

server = kernel.as_mcp_server(server_name="sk_release_notes", prompts=[prompt])
```

::: zone-end

::: zone pivot="programming-language-java"

## Build a kernel

Kernels can be built using a `Kernel.builder()`. On this you can add required AI services and plugins.

```java
Kernel kernel = Kernel.builder()
    .withAIService(ChatCompletionService.class, chatCompletionService)
    .withPlugin(lightPlugin)
    .build();
```

::: zone-end

::: zone pivot="programming-language-csharp"

## Using Dependency Injection

In C#, you can use Dependency Injection to create a kernel. This is done by creating a `ServiceCollection` and adding services and plugins to it. Below is an example of how you can create a kernel using Dependency Injection.

> [!TIP]
> We recommend that you create a kernel as a transient service so that it is disposed of after each use because the plugin collection is mutable. The kernel is extremely lightweight (since it's just a container for services and plugins), so creating a new kernel for each use is not a performance concern.

```csharp
using Microsoft.SemanticKernel;

var builder = Host.CreateApplicationBuilder(args);

// Add the OpenAI chat completion service as a singleton
builder.Services.AddOpenAIChatCompletion(
    modelId: "gpt-4",
    apiKey: "YOUR_API_KEY",
    orgId: "YOUR_ORG_ID", // Optional; for OpenAI deployment
    serviceId: "YOUR_SERVICE_ID" // Optional; for targeting specific services within Semantic Kernel
);

// Create singletons of your plugins
builder.Services.AddSingleton(() => new LightsPlugin());
builder.Services.AddSingleton(() => new SpeakerPlugin());

// Create the plugin collection (using the KernelPluginFactory to create plugins from objects)
builder.Services.AddSingleton<KernelPluginCollection>((serviceProvider) => 
    [
        KernelPluginFactory.CreateFromObject(serviceProvider.GetRequiredService<LightsPlugin>()),
        KernelPluginFactory.CreateFromObject(serviceProvider.GetRequiredService<SpeakerPlugin>())
    ]
);

// Finally, create the Kernel service with the service provider and plugin collection
builder.Services.AddTransient((serviceProvider)=> {
    KernelPluginCollection pluginCollection = serviceProvider.GetRequiredService<KernelPluginCollection>();

    return new Kernel(serviceProvider, pluginCollection);
});
```

> [!TIP]
> For more samples on how to use dependency injection in C#, refer to the [concept samples](../get-started/detailed-samples.md).

::: zone-end

::: zone pivot="programming-language-java"

::: zone-end

## Next steps

Now that you understand the kernel, you can learn about all the different AI services that you can add to it.

> [!div class="nextstepaction"]
> [Learn about AI services](./ai-services/index.md)
