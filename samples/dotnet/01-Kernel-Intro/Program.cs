// Copyright (c) Microsoft. All rights reserved.

// Create a default kernel
//////////////////////////////////////////////////////////////////////////////////
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Skills.Core;

var kernel = Kernel.Builder
    .Build();
var time = kernel.ImportSkill(new TimeSkill());
var result = await kernel.RunAsync(time["Today"]);

Console.WriteLine(result);


var LlmService = Env.Var("Global:LlmService")!;

if (LlmService == "AzureOpenAI")
{
    // Create a kernel with a logger and Azure OpenAI chat completion service
    //////////////////////////////////////////////////////////////////////////////////
    var AzureOpenAIDeploymentName = Env.Var("AzureOpenAI:ChatCompletionDeploymentName")!;
    var AzureOpenAIEndpoint = Env.Var("AzureOpenAI:Endpoint")!;
    var AzureOpenAIApiKey = Env.Var("AzureOpenAI:ApiKey")!;

    using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    {
        builder
            .SetMinimumLevel(0)
            .AddDebug();
    });

    var kernelWithConfiguration = Kernel.Builder
        .WithLoggerFactory(loggerFactory)
        .WithAzureChatCompletionService(
            AzureOpenAIDeploymentName,  // The name of your deployment (e.g., "gpt-35-turbo")
            AzureOpenAIEndpoint,        // The endpoint of your Azure OpenAI service
            AzureOpenAIApiKey           // The API key of your Azure OpenAI service
        )
        .Build();

    var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins");
    var writerPlugin = kernelWithConfiguration
         .ImportSemanticSkillFromDirectory(pluginsDirectory, "WriterPlugin");

    result = await kernelWithConfiguration.RunAsync("Hello world", writerPlugin["ShortPoem"]);
    Console.WriteLine(result);
}
else
{
    // Create a kernel with a logger and OpenAI chat completion service
    //////////////////////////////////////////////////////////////////////////////////
    var OpenAIModelId = Env.Var("OpenAI:ChatCompletionModelId")!;
    var OpenAIApiKey = Env.Var("OpenAI:ApiKey")!;
    var OpenAIOrgId = Env.Var("OpenAI:OrgId")!;

    using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    {
        builder
            .SetMinimumLevel(0)
            .AddDebug();
    });

    var kernelWithConfiguration = Kernel.Builder
        .WithLoggerFactory(loggerFactory)
        .WithOpenAIChatCompletionService(
            OpenAIModelId,              // The name of your deployment (e.g., "gpt-35-turbo")
            OpenAIApiKey,               // The API key of your Azure OpenAI service
            OpenAIOrgId                 // The endpoint of your Azure OpenAI service
        )
        .Build();

    var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins");
    var writerPlugin = kernelWithConfiguration
         .ImportSemanticSkillFromDirectory(pluginsDirectory, "WriterPlugin");

    result = await kernelWithConfiguration.RunAsync("Hello world", writerPlugin["ShortPoem"]);
    Console.WriteLine(result);
}



