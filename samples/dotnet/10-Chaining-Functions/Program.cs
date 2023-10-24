// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using Plugins.OrchestratorPlugin;

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(0)
        .AddDebug();
});

// Create kernel
IKernel kernel = new KernelBuilder()
    // Add a text or chat completion service using either:
    // .WithAzureTextCompletionService()
    // .WithAzureChatCompletionService()
    // .WithOpenAITextCompletionService()
    // .WithOpenAIChatCompletionService()
    .WithCompletionService()
    .WithLoggerFactory(loggerFactory)
    .Build();

var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins");

// Import the semantic functions
kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "OrchestratorPlugin");

// Import the native functions
var mathPlugin = kernel.ImportFunctions(new Plugins.MathPlugin.Math(), "MathPlugin");
var orchestratorPlugin = kernel.ImportFunctions(new Orchestrator(kernel), "OrchestratorPlugin");
var conversationSummaryPlugin = kernel.ImportFunctions(new ConversationSummaryPlugin(kernel), "ConversationSummaryPlugin");

// Make a request that runs the Sqrt function
var result1 = await kernel.RunAsync("What is the square root of 524?", orchestratorPlugin["RouteRequest"]);
Console.WriteLine(result1);

// Make a request that runs the Add function
var result2 = await kernel.RunAsync("How many square feet would the room be if its length was 12.25 feet and its width was 17.33 feet?", orchestratorPlugin["RouteRequest"]);
Console.WriteLine(result2);
