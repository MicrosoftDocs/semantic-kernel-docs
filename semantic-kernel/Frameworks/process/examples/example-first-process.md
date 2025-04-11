---
title: How-To&colon; Create your first Process
description: A step-by-step walk-through for creating a basic Process
zone_pivot_groups: programming-languages
author: bentho
ms.topic: tutorial
ms.author: bentho
ms.date: 01/13/2025
ms.service: semantic-kernel
---
# How-To: Create your first Process

> [!WARNING]
> The _Semantic Kernel Process Framework_ is experimental, still in development and is subject to change.

## Overview

The Semantic Kernel Process Framework is a powerful orchestration SDK designed to simplify the development and execution of AI-integrated processes. Whether you are managing simple workflows or complex systems, this framework allows you to define a series of steps that can be executed in a structured manner, enhancing your application's capabilities with ease and flexibility.

Built for extensibility, the Process Framework supports diverse operational patterns such as sequential execution, parallel processing, fan-in and fan-out configurations, and even map-reduce strategies. This adaptability makes it suitable for a variety of real-world applications, particularly those that require intelligent decision-making and multi-step workflows.

## Getting Started

The Sematic Kernel Process Framework can be used to infuse AI into just about any business process you can think of. As an illustrative example to get started, let's look at building a process for generating documentation for a new product.

Before we get started, make sure you have the required Semantic Kernel packages installed:

::: zone pivot="programming-language-csharp"

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Process.LocalRuntime --version 1.46.0-alpha
```

::: zone-end

::: zone pivot="programming-language-python"
pip install semantic-kernel==1.20.0
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

## Illustrative Example: Generating Documentation for a New Product

In this example, we will utilize the Semantic Kernel Process Framework to develop an automated process for creating documentation for a new product. This process will start out simple and evolve as we go to cover more realistic scenarios.

We will start by modeling the documentation process with a very basic flow:

1. `GatherProductInfoStep`: Gather information about the product.
1. `GenerateDocumentationStep`: Ask an LLM to generate documentation from the information gathered in step 1.
1. `PublishDocumentationStep`: Publish the documentation.

![Flow diagram of our first process: A[Request Feature Documentation] --> B[Ask LLM To Write Documentation] --> C[Publish Documentation To Public]](../../../media/first-process-flow.png)

Now that we understand our processes, let's build it.

### Define the process steps

Each step of a Process is defined by a class that inherits from our base step class. For this process we have three steps:

::: zone pivot="programming-language-csharp"

```csharp
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;

// A process step to gather information about a product
public class GatherProductInfoStep: KernelProcessStep
{
    [KernelFunction]
    public string GatherProductInformation(string productName)
    {
        Console.WriteLine($"{nameof(GatherProductInfoStep)}:\n\tGathering product information for product named {productName}");
    
        // For example purposes we just return some fictional information.
        return
            """
            Product Description:
            GlowBrew is a revolutionary AI driven coffee machine with industry leading number of LEDs and programmable light shows. The machine is also capable of brewing coffee and has a built in grinder.
            
            Product Features:
            1. **Luminous Brew Technology**: Customize your morning ambiance with programmable LED lights that sync with your brewing process.
            2. **AI Taste Assistant**: Learns your taste preferences over time and suggests new brew combinations to explore.
            3. **Gourmet Aroma Diffusion**: Built-in aroma diffusers enhance your coffee's scent profile, energizing your senses before the first sip.
            
            Troubleshooting:
            - **Issue**: LED Lights Malfunctioning
                - **Solution**: Reset the lighting settings via the app. Ensure the LED connections inside the GlowBrew are secure. Perform a factory reset if necessary.
            """;
    }
}

// A process step to generate documentation for a product
public class GenerateDocumentationStep : KernelProcessStep<GeneratedDocumentationState>
{
    private GeneratedDocumentationState _state = new();

    private string systemPrompt =
            """
            Your job is to write high quality and engaging customer facing documentation for a new product from Contoso. You will be provide with information
            about the product in the form of internal documentation, specs, and troubleshooting guides and you must use this information and
            nothing else to generate the documentation. If suggestions are provided on the documentation you create, take the suggestions into account and
            rewrite the documentation. Make sure the product sounds amazing.
            """;

    // Called by the process runtime when the step instance is activated. Use this to load state that may be persisted from previous activations.
    override public ValueTask ActivateAsync(KernelProcessStepState<GeneratedDocumentationState> state)
    {
        this._state = state.State!;
        this._state.ChatHistory ??= new ChatHistory(systemPrompt);

        return base.ActivateAsync(state);
    }

    [KernelFunction]
    public async Task GenerateDocumentationAsync(Kernel kernel, KernelProcessStepContext context, string productInfo)
    {
        Console.WriteLine($"[{nameof(GenerateDocumentationStep)}]:\tGenerating documentation for provided productInfo...");

        // Add the new product info to the chat history
        this._state.ChatHistory!.AddUserMessage($"Product Info:\n{productInfo.Title} - {productInfo.Content}");

        // Get a response from the LLM
        IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var generatedDocumentationResponse = await chatCompletionService.GetChatMessageContentAsync(this._state.ChatHistory!);

        DocumentInfo generatedContent = new()
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Generated document - {productInfo.Title}",
            Content = generatedDocumentationResponse.Content!,
        };

        this._state!.LastGeneratedDocument = generatedContent;

        await context.EmitEventAsync("DocumentationGenerated", generatedContent);
    }

    public class GeneratedDocumentationState
    {
        public DocumentInfo LastGeneratedDocument { get; set; } = new();
        public ChatHistory? ChatHistory { get; set; }
    }
}

// A process step to publish documentation
public class PublishDocumentationStep : KernelProcessStep
{
    [KernelFunction]
    public DocumentInfo PublishDocumentation(DocumentInfo document)
    {
        // For example purposes we just write the generated docs to the console
        Console.WriteLine($"[{nameof(PublishDocumentationStep)}]:\tPublishing product documentation approved by user: \n{document.Title}\n{document.Content}");
        return document;
    }
}

// Custom classes must be serializable
public class DocumentInfo
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

```

The code above defines the three steps we need for our Process. There are a few points to call out here:
- In Semantic Kernel, a `KernelFunction` defines a block of code that is invocable by native code or by an LLM. In the case of the Process framework, `KernelFunction`s are the invocable members of a Step and each step requires at least one KernelFunction to be defined.
- The Process Framework has support for stateless and stateful steps. Stateful steps automatically checkpoint their progress and maintain state over multiple invocations. The `GenerateDocumentationStep` provides an example of this where the `GeneratedDocumentationState` class is used to persist the `ChatHistory` and `LastGeneratedDocument` object.
- Steps can manually emit events by calling `EmitEventAsync` on the `KernelProcessStepContext` object. To get an instance of `KernelProcessStepContext` just add it as a parameter on your KernelFunction and the framework will automatically inject it.
::: zone-end

::: zone pivot="programming-language-python"
```python
import asyncio
from typing import ClassVar

from pydantic import BaseModel, Field

from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.chat_completion_client_base import ChatCompletionClientBase
from semantic_kernel.connectors.ai.open_ai import AzureChatCompletion
from semantic_kernel.contents import ChatHistory
from semantic_kernel.functions import kernel_function
from semantic_kernel.processes import ProcessBuilder
from semantic_kernel.processes.kernel_process import KernelProcessStep, KernelProcessStepContext, KernelProcessStepState
from semantic_kernel.processes.local_runtime import KernelProcessEvent, start


# A process step to gather information about a product
class GatherProductInfoStep(KernelProcessStep):
    @kernel_function
    def gather_product_information(self, product_name: str) -> str:
        print(f"{GatherProductInfoStep.__name__}\n\t Gathering product information for Product Name: {product_name}")

        return """
Product Description:

GlowBrew is a revolutionary AI driven coffee machine with industry leading number of LEDs and 
programmable light shows. The machine is also capable of brewing coffee and has a built in grinder.

Product Features:
1. **Luminous Brew Technology**: Customize your morning ambiance with programmable LED lights that sync 
    with your brewing process.
2. **AI Taste Assistant**: Learns your taste preferences over time and suggests new brew combinations 
    to explore.
3. **Gourmet Aroma Diffusion**: Built-in aroma diffusers enhance your coffee's scent profile, energizing 
    your senses before the first sip.

Troubleshooting:
- **Issue**: LED Lights Malfunctioning
    - **Solution**: Reset the lighting settings via the app. Ensure the LED connections inside the 
        GlowBrew are secure. Perform a factory reset if necessary.
        """


# A sample step state model for the GenerateDocumentationStep
class GeneratedDocumentationState(BaseModel):
    """State for the GenerateDocumentationStep."""

    chat_history: ChatHistory | None = None


# A process step to generate documentation for a product
class GenerateDocumentationStep(KernelProcessStep[GeneratedDocumentationState]):
    state: GeneratedDocumentationState = Field(default_factory=GeneratedDocumentationState)

    system_prompt: ClassVar[str] = """
Your job is to write high quality and engaging customer facing documentation for a new product from Contoso. You will 
be provided with information about the product in the form of internal documentation, specs, and troubleshooting guides 
and you must use this information and nothing else to generate the documentation. If suggestions are provided on the 
documentation you create, take the suggestions into account and rewrite the documentation. Make sure the product 
sounds amazing.
"""

    async def activate(self, state: KernelProcessStepState[GeneratedDocumentationState]):
        self.state = state.state
        if self.state.chat_history is None:
            self.state.chat_history = ChatHistory(system_message=self.system_prompt)
        self.state.chat_history

    @kernel_function
    async def generate_documentation(
        self, context: KernelProcessStepContext, product_info: str, kernel: Kernel
    ) -> None:
        print(f"{GenerateDocumentationStep.__name__}\n\t Generating documentation for provided product_info...")

        self.state.chat_history.add_user_message(f"Product Information:\n{product_info}")

        chat_service, settings = kernel.select_ai_service(type=ChatCompletionClientBase)
        assert isinstance(chat_service, ChatCompletionClientBase)  # nosec

        response = await chat_service.get_chat_message_content(chat_history=self.state.chat_history, settings=settings)

        await context.emit_event(process_event="documentation_generated", data=str(response))


# A process step to publish documentation
class PublishDocumentationStep(KernelProcessStep):
    @kernel_function
    async def publish_documentation(self, docs: str) -> None:
        print(f"{PublishDocumentationStep.__name__}\n\t Publishing product documentation:\n\n{docs}")
```

The code above defines the three steps we need for our Process. There are a few points to call out here:
- In Semantic Kernel, a `KernelFunction` defines a block of code that is invocable by native code or by an LLM. In the case of the Process framework, `KernelFunction`s are the invocable members of a Step and each step requires at least one KernelFunction to be defined.
- The Process Framework has support for stateless and stateful steps. Stateful steps automatically checkpoint their progress and maintain state over multiple invocations. The `GenerateDocumentationStep` provides an example of this where the `GeneratedDocumentationState` class is used to persist the `ChatHistory` object.
- Steps can manually emit events by calling `emit_event` on the `KernelProcessStepContext` object. To get an instance of `KernelProcessStepContext` just add it as a parameter on your KernelFunction and the framework will automatically inject it.
::: zone-end

### Define the process flow

::: zone pivot="programming-language-csharp"

```csharp
// Create the process builder
ProcessBuilder processBuilder = new("DocumentationGeneration");

// Add the steps
var infoGatheringStep = processBuilder.AddStepFromType<GatherProductInfoStep>();
var docsGenerationStep = processBuilder.AddStepFromType<GenerateDocumentationStep>();
var docsPublishStep = processBuilder.AddStepFromType<PublishDocumentationStep>();

// Orchestrate the events
processBuilder
    .OnInputEvent("Start")
    .SendEventTo(new(infoGatheringStep));

infoGatheringStep
    .OnFunctionResult()
    .SendEventTo(new(docsGenerationStep));

docsGenerationStep
    .OnFunctionResult()
    .SendEventTo(new(docsPublishStep));
```

There are a few things going on here so let's break it down step by step.

1. Create the builder:
Processes use a builder pattern to simplify wiring everything up. The builder provides methods for managing the steps within a process and for managing the lifecycle of the process.

1. Add the steps:
Steps are added to the process by calling the `AddStepFromType` method of the builder. This allows the Process Framework to manage the lifecycle of steps by instantiating instances as needed. In this case we've added three steps to the process and created a variable for each one. These variables give us a handle to the unique instance of each step that we can use next to define the orchestration of events.

1. Orchestrate the events:
This is where the routing of events from step to step are defined. In this case we have the following routes:
    - When an external event with `id = Start` is sent to the process, this event and its associated data will be sent to the `infoGatheringStep` step.
    - When the `infoGatheringStep` finishes running, send the returned object to the `docsGenerationStep` step.
    - Finally, when the `docsGenerationStep` finishes running, send the returned object to the `docsPublishStep` step.

> [!TIP]
> **_Event Routing in Process Framework:_** You may be wondering how events that are sent to steps are routed to KernelFunctions within the step. In the code above, each step has only defined a single KernelFunction and each KernelFunction has only a single parameter (other than Kernel and the step context which are special, more on that later). When the event containing the generated documentation is sent to the `docsPublishStep` it will be passed to the `document` parameter of the `PublishDocumentation` KernelFunction of the `docsGenerationStep` step because there is no other choice. However, steps can have multiple KernelFunctions and KernelFunctions can have multiple parameters, in these advanced scenarios you need to specify the target function and parameter.

::: zone-end

::: zone pivot="programming-language-python"
```python
# Create the process builder
process_builder = ProcessBuilder(name="DocumentationGeneration")

# Add the steps
info_gathering_step = process_builder.add_step(GatherProductInfoStep)
docs_generation_step = process_builder.add_step(GenerateDocumentationStep)
docs_publish_step = process_builder.add_step(PublishDocumentationStep)

# Orchestrate the events
process_builder.on_input_event("Start").send_event_to(target=info_gathering_step)

info_gathering_step.on_function_result().send_event_to(
    target=docs_generation_step, function_name="generate_documentation", parameter_name="product_info"
)

docs_generation_step.on_event("documentation_generated").send_event_to(target=docs_publish_step)

# Configure the kernel with an AI Service and connection details, if necessary
kernel = Kernel()
kernel.add_service(AzureChatCompletion())

# Build the process
kernel_process = process_builder.build()
```

There are a few things going on here so let's break it down step by step.

1. Create the builder:
Processes use a builder pattern to simplify wiring everything up. The builder provides methods for managing the steps within a process and for managing the lifecycle of the process.

1. Add the steps:
Steps are added to the process by calling the `add_step` method of the builder, which adds the step type to the builder. This allows the Process Framework to manage the lifecycle of steps by instantiating instances as needed. In this case we've added three steps to the process and created a variable for each one. These variables give us a handle to the unique instance of each step that we can use next to define the orchestration of events.

1. Orchestrate the events:
This is where the routing of events from step to step are defined. In this case we have the following routes:
    - When an external event with `id = Start` is sent to the process, this event and its associated data will be sent to the `info_gathering_step`.
    - When the `info_gathering_step` finishes running, send the returned object to the `docs_generation_step`.
    - Finally, when the `docs_generation_step` finishes running, send the returned object to the `docs_publish_step`.

> [!TIP]
> **_Event Routing in Process Framework:_** You may be wondering how events that are sent to steps are routed to KernelFunctions within the step. In the code above, each step has only defined a single KernelFunction and each KernelFunction has only a single parameter (other than Kernel and the step context which are special, more on that later). When the event containing the generated documentation is sent to the `docs_publish_step` it will be passed to the `docs` parameter of the `publish_documentation` KernelFunction of the `docs_generation_step` because there is no other choice. However, steps can have multiple KernelFunctions and KernelFunctions can have multiple parameters, in these advanced scenarios you need to specify the target function and parameter.

::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

### Build and run the Process

::: zone pivot="programming-language-csharp"

```csharp
// Configure the kernel with your LLM connection details
Kernel kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion("myDeployment", "myEndpoint", "myApiKey")
    .Build();

// Build and run the process
var process = processBuilder.Build();
await process.StartAsync(kernel, new KernelProcessEvent { Id = "Start", Data = "Contoso GlowBrew" });
```

We build the process and call `StartAsync` to run it. Our process is expecting an initial external event called `Start` to kick things off and so we provide that as well. Running this process shows the following output in the Console:

```
GatherProductInfoStep: Gathering product information for product named Contoso GlowBrew
GenerateDocumentationStep: Generating documentation for provided productInfo
PublishDocumentationStep: Publishing product documentation:

# GlowBrew: Your Ultimate Coffee Experience Awaits!

Welcome to the world of GlowBrew, where coffee brewing meets remarkable technology! At Contoso, we believe that your morning ritual shouldn't just include the perfect cup of coffee but also a stunning visual experience that invigorates your senses. Our revolutionary AI-driven coffee machine is designed to transform your kitchen routine into a delightful ceremony.

## Unleash the Power of GlowBrew

### Key Features

- **Luminous Brew Technology**
  - Elevate your coffee experience with our cutting-edge programmable LED lighting. GlowBrew allows you to customize your morning ambiance, creating a symphony of colors that sync seamlessly with your brewing process. Whether you need a vibrant wake-up call or a soothing glow, you can set the mood for any moment!

- **AI Taste Assistant**
  - Your taste buds deserve the best! With the GlowBrew built-in AI taste assistant, the machine learns your unique preferences over time and curates personalized brew suggestions just for you. Expand your coffee horizons and explore delightful new combinations that fit your palate perfectly.

- **Gourmet Aroma Diffusion**
  - Awaken your senses even before that first sip! The GlowBrew comes equipped with gourmet aroma diffusers that enhance the scent profile of your coffee, diffusing rich aromas that fill your kitchen with the warm, inviting essence of freshly-brewed bliss.

### Not Just Coffee - An Experience

With GlowBrew, it's more than just making coffee-it's about creating an experience that invigorates the mind and pleases the senses. The glow of the lights, the aroma wafting through your space, and the exceptional taste meld into a delightful ritual that prepares you for whatever lies ahead.

## Troubleshooting Made Easy

While GlowBrew is designed to provide a seamless experience, we understand that technology can sometimes be tricky. If you encounter issues with the LED lights, we've got you covered:

- **LED Lights Malfunctioning?**
  - If your LED lights aren't working as expected, don't worry! Follow these steps to restore the glow:
    1. **Reset the Lighting Settings**: Use the GlowBrew app to reset the lighting settings.
    2. **Check Connections**: Ensure that the LED connections inside the GlowBrew are secure.
    3. **Factory Reset**: If you're still facing issues, perform a factory reset to rejuvenate your machine.

With GlowBrew, you not only brew the perfect coffee but do so with an ambiance that excites the senses. Your mornings will never be the same!

## Embrace the Future of Coffee

Join the growing community of GlowBrew enthusiasts today, and redefine how you experience coffee. With stunning visual effects, customized brewing suggestions, and aromatic enhancements, it's time to indulge in the delightful world of GlowBrew-where every cup is an adventure!

### Conclusion

Ready to embark on an extraordinary coffee journey? Discover the perfect blend of technology and flavor with Contoso's GlowBrew. Your coffee awaits!
```
::: zone-end

::: zone pivot="programming-language-python"

```python
# Configure the kernel with an AI Service and connection details, if necessary
kernel = Kernel()
kernel.add_service(AzureChatCompletion())

# Build the process
kernel_process = process_builder.build()

# Start the process
async with await start(
    process=kernel_process,
    kernel=kernel,
    initial_event=KernelProcessEvent(id="Start", data="Contoso GlowBrew"),
) as process_context:
    _ = await process_context.get_state()
```

We build the process and call `start` with the asynchronous context manager to run it. Our process is expecting an initial external event called `Start` to kick things off and so we provide that as well. Running this process shows the following output in the Console:

```
GatherProductInfoStep
         Gathering product information for Product Name: Contoso GlowBrew
GenerateDocumentationStep
         Generating documentation for provided product_info...
PublishDocumentationStep
         Publishing product documentation:

# GlowBrew AI-Driven Coffee Machine: Elevate Your Coffee Experience

Welcome to the future of coffee enjoyment with GlowBrew, the AI-driven coffee machine that not only crafts the perfect cup but does so with a light show that brightens your day. Designed for coffee enthusiasts and tech aficionados alike, GlowBrew combines cutting-edge brewing technology with an immersive lighting experience to start every day on a bright note.

## Unleash the Power of Luminous Brew Technology

With GlowBrew, your mornings will never be dull. The industry-leading number of programmable LEDs offers endless possibilities for customizing your coffee-making ritual. Sync the light show with the brewing process to create a visually stimulating ambiance that transforms your kitchen into a vibrant café each morning.

## Discover New Flavor Dimensions with the AI Taste Assistant

Leave the traditional coffee routines behind and say hello to personalization sophistication. The AI Taste Assistant learns and adapts to your unique preferences over time. Whether you prefer a strong espresso or a light latte, the assistant suggests new brew combinations tailored to your palate, inviting you to explore a world of flavors you never knew existed.

## Heighten Your Senses with Gourmet Aroma Diffusion

The moment you step into the room, let the GlowBrew’s built-in aroma diffusers captivate your senses. This feature is designed to enrich your coffee’s scent profile, ensuring every cup you brew is a multi-sensory delight. Let the burgeoning aroma energize you before the very first sip.

## Troubleshooting Guide: LED Lights Malfunctioning

Occasionally, you might encounter an issue with the LED lights not functioning as intended. Here’s how to resolve it efficiently:

- **Reset Lighting Settings**: Start by using the GlowBrew app to reset the lighting configurations to their default state.
- **Check Connections**: Ensure that all LED connections inside your GlowBrew machine are secure and properly connected.
- **Perform a Factory Reset**: If the problem persists, perform a factory reset on your GlowBrew to restore all settings to their original state.

Experience the art of coffee making like never before with the GlowBrew AI-driven coffee machine. From captivating light shows to aromatic sensations, every feature is engineered to enhance your daily brew. Brew, savor, and glow with GlowBrew.
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

## What's Next?

Our first draft of the documentation generation process is working but it leaves a lot to be desired. At a minimum, a production version would need:

- A proof reader agent that will grade the generated documentation and verify that it meets our standards of quality and accuracy.
- An approval process where the documentation is only published after a human approves it (human-in-the-loop).

> [!div class="nextstepaction"]
> [Add a proof reader agent to our process...](./example-cycles.md)
