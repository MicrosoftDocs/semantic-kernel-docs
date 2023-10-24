// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(0)
        .AddDebug();
});

IKernel kernel = new KernelBuilder()
    // Add a text or chat completion service using either:
    // .WithAzureTextCompletionService()
    // .WithAzureChatCompletionService()
    // .WithOpenAITextCompletionService()
    // .WithOpenAIChatCompletionService()
    .WithCompletionService()
    .WithLoggerFactory(loggerFactory)
    .Build();

// Import the Math Plugin
var mathPlugin = kernel.ImportFunctions(new Plugins.MathPlugin.Math(), "MathPlugin");

// Create the context variables for the Multiply function
var contextVariables = new ContextVariables
{
    ["input"] = "12.34",
    ["number2"] = "56.78"
};

// Make a request that runs the Multiply function
var result = await kernel.RunAsync(contextVariables, mathPlugin["Multiply"]);
Console.WriteLine(result);
