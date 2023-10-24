// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using ILoggerFactory loggerFactory = LoggerFactory.Create(Builder =>
{
    Builder
        .SetMinimumLevel(0)
        .AddDebug();
});

// Create kernel
IKernel Kernel = new KernelBuilder()
    // Add a text or chat completion service using either:
    // .WithAzureTextCompletionService()
    // .WithAzureChatCompletionService()
    // .WithOpenAITextCompletionService()
    // .WithOpenAIChatCompletionService()
    .WithCompletionService()
    .WithLoggerFactory(loggerFactory)
    .Build();

var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins");

// Import the OrchestratorPlugin from the plugins directory.
var orchestratorPlugin = Kernel
     .ImportSemanticFunctionsFromDirectory(pluginsDirectory, "OrchestratorPlugin");

// Get the GetIntent function from the OrchestratorPlugin and run it
var result = await Kernel.RunAsync(
    "I want to send an email to the marketing team celebrating their recent milestone.",
    orchestratorPlugin["GetIntent"]
);

Console.WriteLine(result);