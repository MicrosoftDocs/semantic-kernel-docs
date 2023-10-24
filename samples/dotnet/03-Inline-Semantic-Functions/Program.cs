// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

// Create the prompt for the semantic function
string prompt = @"Bot: How can I help you?
User: {{$input}}

---------------------------------------------

The intent of the user in 5 words or less: ";

// Create request settings
OpenAIRequestSettings requestSettings = new()
{
    ExtensionData = {
                {"MaxTokens", 500},
                {"Temperature", 0.0},
                {"TopP", 0.0},
                {"PresencePenalty", 0.0},
                {"FrequencyPenalty", 0.0}
            }
};

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(0)
        .AddDebug();
});

// Create the Kernel
IKernel kernel = new KernelBuilder()
    // Add a text or chat completion service using either:
    // .WithAzureTextCompletionService()
    // .WithAzureChatCompletionService()
    // .WithOpenAITextCompletionService()
    // .WithOpenAIChatCompletionService()
    .WithCompletionService()
    .WithLoggerFactory(loggerFactory)
    .Build();

// Register the GetIntent function with the Kernel
var getIntentFunction = kernel.CreateSemanticFunction(prompt, requestSettings, "GetIntent");

// Run the GetIntent function
var result = await kernel.RunAsync(
    "I want to send an email to the marketing team celebrating their recent milestone.",
    getIntentFunction
);

Console.WriteLine(result);