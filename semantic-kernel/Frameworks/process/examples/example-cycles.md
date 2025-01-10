---
title: How-To&colon; Using Cycles
description: A step-by-step walk-through for using Cycles
zone_pivot_groups: programming-languages
author: bentho
ms.topic: tutorial
ms.author: bentho
ms.date: 01/13/2025
ms.service: semantic-kernel
---
# How-To: Using Cycles

> [!WARNING]
> The _Semantic Kernel Process Framework_ is experimental, still in development and is subject to change.

## Overview

In the previous section we build a simple Process to help us automate the creation of documentation for our product. In this section we will improve on that process by adding a quality assurance step. This step will use and LLM to grade the generated documentation and provide recommended changes as well as a pass/fail grade. By taking advantage of the Process Frameworks' support for cycles, we can go one step further and automatically apply the recommended changes (if any) and then start the cycle over, repeating this until the content meets our quality bar. The updated process will look like this:

```mermaid
graph LR
    A[Request Feature Documentation] --> B[Ask LLM To Write Documentation] 
    B --> C[Proofread Documentation]
    C -->|Updates Needed| B
    C -->|Approved| D[Publish Documentation To Public]
```

## Updates to the process

We need to create our new proofreader step and also make a couple changes to our document generation step that will allow us to apply suggestions if needed.

### Step Updates

::: zone pivot="programming-language-csharp"

```csharp

// Updated process step to generate and edit documentation for a product
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

    override public ValueTask ActivateAsync(KernelProcessStepState<GeneratedDocumentationState> state)
    {
        this._state = state.State!;
        this._state.ChatHistory ??= new ChatHistory(systemPrompt);

        return base.ActivateAsync(state);
    }

    [KernelFunction]
    public async Task GenerateDocumentationAsync(Kernel kernel, KernelProcessStepContext context, string productInfo)
    {
        Console.WriteLine($"{nameof(GenerateDocumentationStep)}:\n\tGenerating documentation for provided productInfo...");

        // Add the new product info to the chat history
        this._state.ChatHistory!.AddUserMessage($"Product Info:\n\n{productInfo}");

        // Get a response from the LLM
        IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var generatedDocumentationResponse = await chatCompletionService.GetChatMessageContentAsync(this._state.ChatHistory!);

        await context.EmitEventAsync("DocumentationGenerated", generatedDocumentationResponse.Content!.ToString());
    }

    [KernelFunction]
    public async Task ApplySuggestionsAsync(Kernel kernel, KernelProcessStepContext context, string suggestions)
    {
        Console.WriteLine($"{nameof(GenerateDocumentationStep)}:\n\tRewriting documentation with provided suggestions...");

        // Add the new product info to the chat history
        this._state.ChatHistory!.AddUserMessage($"Rewrite the documentation with the following suggestions:\n\n{suggestions}");

        // Get a response from the LLM
        IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var generatedDocumentationResponse = await chatCompletionService.GetChatMessageContentAsync(this._state.ChatHistory!);

        await context.EmitEventAsync("DocumentationGenerated", generatedDocumentationResponse.Content!.ToString());
    }

    public class GeneratedDocumentationState
    {
        public ChatHistory? ChatHistory { get; set; }
    }
}

// A process step to proofread documentation
public class ProofreadStep : KernelProcessStep
{
    [KernelFunction]
    public async Task ProofreadDocumentationAsync(Kernel kernel, KernelProcessStepContext context, string documentation)
    {
        Console.WriteLine($"{nameof(ProofreadDocumentationAsync)}:\n\tProofreading documentation...");

        var systemPrompt =
            """
        Your job is to proofread customer facing documentation for a new product from Contoso. You will be provide with proposed documentation
        for a product and you must do the following things:

        1. Determine if the documentation is passes the following criteria:
            1. Documentation must use a professional tone.
            1. Documentation should be free of spelling or grammar mistakes.
            1. Documentation should be free of any offensive or inappropriate language.
            1. Documentation should be technically accurate.
        2. If the documentation does not pass 1, you must write detailed feedback of the changes that are needed to improve the documentation. 
        """;

        ChatHistory chatHistory = new ChatHistory(systemPrompt);
        chatHistory.AddUserMessage(documentation);

        // Use structured output to ensure the response format is easily parsable
        OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings();
        settings.ResponseFormat = typeof(ProofreadingResponse);

        IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var proofreadResponse = await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings: settings);
        var formattedResponse = JsonSerializer.Deserialize<ProofreadingResponse>(proofreadResponse.Content!.ToString());

        Console.WriteLine($"\n\tGrade: {(formattedResponse!.MeetsExpectations ? "Pass" : "Fail")}\n\tExplanation: {formattedResponse.Explanation}\n\tSuggestions: {string.Join("\n\t\t", formattedResponse.Suggestions)}");

        if (formattedResponse.MeetsExpectations)
        {
            await context.EmitEventAsync("DocumentationApproved", data: documentation);
        }
        else
        {
            await context.EmitEventAsync("DocumentationRejected", data: new { Explanation = formattedResponse.Explanation, Suggestions = formattedResponse.Suggestions});
        }
    }

    // A class 
    private class ProofreadingResponse
    {
        [Description("Specifies if the proposed documentation meets the expected standards for publishing.")]
        public bool MeetsExpectations { get; set; }

        [Description("An explanation of why the documentation does or does not meet expectations.")]
        public string Explanation { get; set; } = "";

        [Description("A lis of suggestions, may be empty if there no suggestions for improvement.")]
        public List<string> Suggestions { get; set; } = new();
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

The following changes have been made to the steps:

- Updates to `GenerateDocumentationStep`:
The `GenerateDocumentationStep` has been updated to include a new KernelFunction. The new function will be used to apply suggested changes to the documentation if our proofreading step requires them. Notice that both functions for generating or rewriting documentation emit the same event named `DocumentationGenerated` indicating that new documentation is available.
- A new step named `ProofreadStep` has been created. This step uses the LLM to grade the generated documentation as discussed above. Notice that this step conditionally emits either the `DocumentationApproved` event or the `DocumentationRejected` event based on the response from the LLM.

### Flow updates

::: zone pivot="programming-language-csharp"

```csharp
// Create the process builder
ProcessBuilder processBuilder = new("DocumentationGeneration");

// Add the steps
var infoGatheringStep = processBuilder.AddStepFromType<GatherProductInfoStep>();
var docsGenerationStep = processBuilder.AddStepFromType<GenerateDocumentationStepV2>();
var docsProofreadStep = processBuilder.AddStepFromType<ProofreadStep>();
var docsPublishStep = processBuilder.AddStepFromType<PublishDocumentationStep>();

// Orchestrate the events
processBuilder
    .OnInputEvent("Start")
    .SendEventTo(new(infoGatheringStep));

infoGatheringStep
    .OnFunctionResult()
    .SendEventTo(new(docsGenerationStep, functionName: "GenerateDocumentation"));

docsGenerationStep
    .OnEvent("DocumentationGenerated")
    .SendEventTo(new(docsProofreadStep));

docsProofreadStep
    .OnEvent("DocumentationRejected")
    .SendEventTo(new(docsGenerationStep, functionName: "ApplySuggestions"));

docsProofreadStep
    .OnEvent("DocumentationApproved")
    .SendEventTo(new(docsPublishStep));

var process = processBuilder.Build();
return process;
```

::: zone-end

::: zone pivot="programming-language-python"
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

Our updated process routing now does the following:
- When an external event with `id = Start` is sent to the process, this event and its associated data will be sent to the `infoGatheringStep` step.
- When the `infoGatheringStep` finishes running, send the returned object to the `docsGenerationStep` step.
- When the `docsGenerationStep` finishes running, send the generated docs to the `docsProofreadStep` step.
- When the `docsProofreadStep` rejects our documentation and provides suggestions, send the suggestions back to the `docsGenerationStep`.
- Finally, when the `docsProofreadStep` approves our documentation, send the returned object to the `docsPublishStep` step.

### Build and run the Process

Running our updated process shows the following output in the console:

```md
GatherProductInfoStep:
        Gathering product information for product named Contoso GlowBrew
GenerateDocumentationStep:
        Generating documentation for provided productInfo...
ProofreadDocumentationAsync:
        Proofreading documentation...

        Grade: Fail
        Explanation: The proposed documentation has an overly casual tone and uses informal expressions that might not suit all customers. Additionally, some phrases may detract from the professionalism expected in customer-facing documentation. There are minor areas that could benefit from clarity and conciseness.
        Suggestions: Adjust the tone to be more professional and less casual; phrases like 'dazzling light show' and 'coffee performing' could be simplified.
                Remove informal phrases such as 'who knew coffee could be so... illuminating?'
                Consider editing out overly whimsical phrases like 'it's like a warm hug for your nose!' for a more straightforward description.
                Clarify the troubleshooting section for better customer understanding; avoid metaphorical language like 'secure that coffee cup when you realize Monday is still a thing.'
GenerateDocumentationStep:
        Rewriting documentation with provided suggestions...
ProofreadDocumentationAsync:
        Proofreading documentation...

        Grade: Fail
        Explanation: The documentation generally maintains a professional tone but contains minor phrasing issues that could be improved. There are no spelling or grammar mistakes noted, and it excludes any offensive language. However, the content could be more concise, and some phrases can be streamlined for clarity. Additionally, technical accuracy regarding troubleshooting solutions may require more details for the user's understanding. For example, clarifying how to 'reset the lighting settings through the designated app' would enhance user experience.
        Suggestions: Rephrase 'Join us as we elevate your coffee experience to new heights!' to make it more straightforward, such as 'Experience an elevated coffee journey with us.'
                In the 'Solution' section for the LED lights malfunction, add specific instructions on how to find and use the 'designated app' for resetting the lighting settings.
                Consider simplifying sentences such as 'Meet your new personal barista!' to be more straightforward, for example, 'Introducing your personal barista.'
                Ensure clarity in troubleshooting steps by elaborating on what a 'factory reset' entails.
GenerateDocumentationStep:
        Rewriting documentation with provided suggestions...
ProofreadDocumentationAsync:
        Proofreading documentation...

        Grade: Pass
        Explanation: The documentation presents a professional tone, contains no spelling or grammar mistakes, is free of offensive language, and is technically accurate regarding the product's features and troubleshooting guidance.
        Suggestions:
PublishDocumentationStep:
        Publishing product documentation:

# GlowBrew User Documentation

## Product Overview
Introducing GlowBrew-your new partner in coffee brewing that brings together advanced technology and aesthetic appeal. This innovative AI-driven coffee machine not only brews your favorite coffee but also features the industry's leading number of customizable LEDs and programmable light shows.

## Key Features

1. **Luminous Brew Technology**: Transform your morning routine with our customizable LED lights that synchronize with your brewing process, creating the perfect ambiance to start your day.

2. **AI Taste Assistant**: Our intelligent system learns your preferences over time, recommending exciting new brew combinations tailored to your unique taste.

3. **Gourmet Aroma Diffusion**: Experience an enhanced aroma with built-in aroma diffusers that elevate your coffee's scent profile, invigorating your senses before that all-important first sip.

## Troubleshooting

### Issue: LED Lights Malfunctioning

**Solution**:
- Begin by resetting the lighting settings via the designated app. Open the app, navigate to the settings menu, and select "Reset LED Lights."
- Ensure that all LED connections inside the GlowBrew are secure and properly connected.
- If issues persist, you may consider performing a factory reset. To do this, hold down the reset button located on the machine's back panel for 10 seconds while the device is powered on.

We hope you enjoy your GlowBrew experience and that it brings a delightful blend of flavor and brightness to your coffee moments!
```


## What's Next?

Our process is now reliably generating documentation that meets our defined standards. This is great, but before we publish our documentation publicly we really should require a human to review and approve. Let's do that next.

- A proof reader agent that will grade the generated documentation and verify that it meets our standards of quality and accuracy.
- An approval process where the documentation is only published after a human approves it (human-in-the-loop).

[!div class="nextstepaction"]
[Human-in-the-loop](./example-human-in-loop.md)