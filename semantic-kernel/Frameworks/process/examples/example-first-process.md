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
> The _Semantic Kernel Agent Framework_ is experimental, still in development and is subject to change.

## Overview

The Semantic Kernel Process Framework is a powerful orchestration SDK designed to simplify the development and execution of AI-integrated processes. Whether you are managing simple workflows or complex systems, this framework allows you to define a series of steps (or nodes) that can be executed in a structured manner, enhancing your application's capabilities with ease and flexibility.

Built for extensibility, the Process Framework supports diverse operational patterns such as sequential execution, parallel processing, fan-in and fan-out configurations, and even map-reduce strategies. This adaptability makes it suitable for a variety of real-world applications, particularly those that require intelligent decision-making and multi-step workflows.

## Getting Started

The Sematic Kernel Process Framework can be used to infuse AI into just about any business process you can think of. As an illustrative example to get started, let's look at building a process for generating documentation for a new product.

Before we get started, make sure you have the required Semantic Kernel packages installed:

::: zone pivot="programming-language-csharp"

```dotnetcli
dotnet add package Microsoft.SemanticKernel.Process.LocalRuntime --version 1.33.0-alpha
```

::: zone-end

::: zone pivot="programming-language-python"
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

### Illustrative Example: Generating Documentation for a New Product

In this example, we will utilize the Semantic Kernel Process Framework to develop an automated process for creating documentation for a new product. High quality documentation is critical for any product but it can be challenging and energy intensive to create and maintain. This process will start out simple and evolve as we go to cover more realistic scenarios.

We will start by modeling the documentation process with a very basic flow:

1. Gather information about the product.
1. Ask an LLM to generate documentation from the information gathered in step 1.
1. Publish the documentation.

```mermaid
graph LR
    A[Request Feature Documentation] --> B[Ask LLM To Write Documentation] --> C[Publish Documentation To Public]
```

So now that we understand our processes, let's write some code.

**Define the process steps:**

Each step of a Process is defined by a class that inherits from our base step class. For this process we have three steps, let's write the code to them.

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
        Console.WriteLine($"Gathering product information for product named {productName}");
    
        // For example purposes we just return some sample information.
        return
            """
        
        """;
    }
}

// A process step to generate documentation for a product
public class GenerateDocumentationStep : KernelProcessStep
{
    [KernelFunction]
    public async Task<string> GenerateDocumentationAsync(Kernel kernel, string productInfo)
    {
        Console.WriteLine("Generating documentation for provided productInfo");

        var systemPrompt = 
            """
            Your job is to write high quality customer facing documentation for a new product from Contoso. You will be provide with information
            about the product in the form of internal documentation, specs, and troubleshooting guides and you must use this information and
            nothing else to generate the documentation. 
            """;

        ChatHistory chatHistory = new ChatHistory(systemPrompt);
        chatHistory.AddUserMessage(productInfo);

        IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var generatedDocumentationResponse = await chatCompletionService.GetChatMessageContentAsync(chatHistory);

        return generatedDocumentationResponse.Content.ToString();
    }
}

// A process step to publish documentation
public class PublishDocumentationStep : KernelProcessStep
{
    [KernelFunction]
    public void PublishDocumentation(string docs)
    {
        // For example purposes we just write the generated docs to the console
        Console.WriteLine($"Publishing product documentation:\n\n{docs}");
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

The code above defines the three steps we need for our Process. In Semantic Kernel, a `KernelFunction` defines a block of code that is invocable by native code or by an LLM. In the case of the Process framework, `KernelFunction`s are the invocable members of a Step and each step requires at least one KernelFunction to be defined.

Now that we have our steps, let's orchestrate them into a Process.

**Define the process flow:**

::: zone pivot="programming-language-csharp"

```csharp
// Configure the kernel with your LLM connection details
Kernel kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion("myDeployment", "myEndpoint", "myApiKey")
    .Build();

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

::: zone-end

::: zone pivot="programming-language-python"
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

The code above creates and runs the processes. There is a lot going on here so let's break it down step by step.

1. Create the builder:
Processes use a builder pattern to simplify wiring everything up. The builder provides methods for managing the steps within a process and for managing the lifecycle of the process.

1. Add the steps:
This code adds our three steps to the process and creates a variable for each one. This gives us a handle to the unique instance of each step that we can use next to define the orchestration of events.

1. Orchestrate the events
This is where the routing of events from step to step are defined. In this case we have the following routes:
    - When an external event with Id `Start` is sent to the process, this event and its associated data will be sent to the `infoGatheringStep` step.
    - When the `infoGatheringStep` finishes running, send the object returned to the `docsGenerationStep` step.
    - Finally, when the `docsGenerationStep` finishes running, send the object it returned to the `docsPublishStep` step.

> **_Event Routing in Process Framework:_** You may be wondering how events that are sent to steps are routed to KernelFunctions within the step. In the code above, each step has only defined a single KernelFunction and each KernelFunction has only a single parameter (other than Kernel which is special, more on that later). When the event containing the generated documentation is sent to the `docsPublishStep` it will be passed to the `docs` parameter of the `PublishDocumentation` KernelFunction of the `docsGenerationStep` step because there is no other choice. However, steps can have multiple KernelFunctions and KernelFunctions can have multiple parameters in in these advanced scenarios you need to specify the target function and parameter. This is discussed in further detail [here](#illustrative-example-generating-documentation-for-a-new-product)

**Build and run the Process**:

::: zone pivot="programming-language-csharp"

```csharp
// Build and run the process
var process = processBuilder.Build();
await process.StartAsync(kernel, new KernelProcessEvent { Id = "Start", Data = "Contoso GlowBrew" });
```

::: zone-end

::: zone pivot="programming-language-python"
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Finally, we build the process and call `StartAsync` to run it. Our process is expecting an initial external event called `Start` to kick things off and so we provide that as well. Run this processes to see the generated documentation!

```md
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

### What's Next?

Our first draft of the documentation generation process is working but it leaves a lot to be desired. At a minimum, a production version would need:

- An editor agent that will grade the generated documentation and verify that it meets our standards of quality and accuracy.
- An approval process where the documentation is only published after a human approves it (human-in-the-loop).

Let's add these to our process next.
