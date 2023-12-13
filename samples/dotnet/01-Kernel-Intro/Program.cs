// Copyright (c) Microsoft. All rights reserved.

// Create a default kernel
//////////////////////////////////////////////////////////////////////////////////
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Plugins;

var LlmService = Env.Var("Global:LlmService")!;

if (LlmService == "AzureOpenAI")
{
    // Create a kernel with a logger and Azure OpenAI chat completion service
    //////////////////////////////////////////////////////////////////////////////////
    var AzureOpenAIDeploymentName = Env.Var("AzureOpenAI:ChatCompletionDeploymentName")!;
    var AzureOpenAIModelId = Env.Var("AzureOpenAI:ChatCompletionModelId")!;
    var AzureOpenAIEndpoint = Env.Var("AzureOpenAI:Endpoint")!;
    var AzureOpenAIApiKey = Env.Var("AzureOpenAI:ApiKey")!;

    KernelBuilder builder = new();
    builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
    builder.Services.AddAzureOpenAIChatCompletion(
        AzureOpenAIDeploymentName,  // The name of your deployment (e.g., "gpt-35-turbo")
        AzureOpenAIModelId,         // The model ID of your Azure OpenAI service
        AzureOpenAIEndpoint,        // The endpoint of your Azure OpenAI service
        AzureOpenAIApiKey           // The API key of your Azure OpenAI service
    );
    builder.Plugins.AddFromType<TimePlugin>();
    builder.Plugins.AddFromPromptDirectory("./plugins/WriterPlugin");

    var kernel = builder.Build();

    // Get the current time
    var currentTime = await kernel.InvokeAsync("TimePlugin", "GetCurrentUtcTime");
    Console.WriteLine(currentTime);

    // Write a poem with the WriterPlugin.ShortPoem function using the current time as input
    var poemResult = await kernel.InvokeAsync("WriterPlugin", "ShortPoem", new() {
        { "input", currentTime }
    });
    Console.WriteLine(poemResult);
}
else
{
    // Create a kernel with a logger and OpenAI chat completion service
    //////////////////////////////////////////////////////////////////////////////////
    var OpenAIModelId = Env.Var("OpenAI:ChatCompletionModelId")!;
    var OpenAIApiKey = Env.Var("OpenAI:ApiKey")!;
    var OpenAIOrgId = Env.Var("OpenAI:OrgId")!;

    KernelBuilder builder = new();
    builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
    builder.Services.AddOpenAIChatCompletion(
        OpenAIModelId,       // The model ID of your OpenAI service
        OpenAIApiKey,        // The API key of your OpenAI service
        OpenAIOrgId          // The organization ID of your OpenAI service
    );
    builder.Plugins.AddFromType<TimePlugin>();
    builder.Plugins.AddFromPromptDirectory("./plugins/WriterPlugin");

    var kernel = builder.Build();

    // Get the current time
    var currentTime = await kernel.InvokeAsync("TimePlugin", "GetCurrentUtcTime");
    Console.WriteLine(currentTime);

    // Write a poem with the WriterPlugin.ShortPoem function using the current time as input
    var poem = await kernel.InvokeAsync("WriterPlugin", "ShortPoem", new() {
        { "input", currentTime }
    });
    Console.WriteLine(poem);
}

