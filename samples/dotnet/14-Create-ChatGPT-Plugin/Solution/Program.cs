// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Functions.OpenAPI.Extensions;
using Microsoft.SemanticKernel.Planners;

// Create a logger
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(0)
        .AddDebug();
});

// Create a kernel
IKernel kernel = new KernelBuilder()
    .WithCompletionService()
    .WithLoggerFactory(loggerFactory)
    .Build();

// Add the math plugin using the plugin manifest URL
const string pluginManifestUrl = "http://localhost:7071/.well-known/ai-plugin.json";
var mathPlugin = await kernel.ImportPluginFunctionsAsync("MathPlugin", new Uri(pluginManifestUrl));

// Create a stepwise planner and invoke it
var planner = new StepwisePlanner(kernel);
var ask = "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent $5 on a latte?";
var plan = planner.CreatePlan(ask);
var result = await kernel.RunAsync(plan);

// Print the results
Console.WriteLine("Result: " + result);

// Print details about the plan
if (result.FunctionResults.First().TryGetMetadataValue("stepCount", out string? stepCount))
{
    Console.WriteLine("Steps Taken: " + stepCount);
}
if (result.FunctionResults.First().TryGetMetadataValue("functionCount", out string? functionCount))
{
    Console.WriteLine("Functions Used: " + functionCount);
}
if (result.FunctionResults.First().TryGetMetadataValue("iterations", out string? iterations))
{
    Console.WriteLine("Iterations: " + iterations);
}
