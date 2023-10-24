// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

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

// Import the Math Plugin
var mathPlugin = kernel.ImportFunctions(new Plugins.MathPlugin.Math(), "MathPlugin");

// Make a request that runs the Sqrt function
var result = await kernel.RunAsync("12", mathPlugin["Sqrt"]);
Console.WriteLine(result.GetValue<double>());
