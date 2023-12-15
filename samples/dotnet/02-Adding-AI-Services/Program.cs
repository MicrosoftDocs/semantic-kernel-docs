// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;

var LlmService = Env.Var("Global:LlmService")!;

if (LlmService == "AzureOpenAI")
{
    var AzureOpenAIDeploymentType = Env.Var("AzureOpenAI:DeploymentType")!;

    if (AzureOpenAIDeploymentType == "chat-completion")
    {
        // Create a kernel with an Azure OpenAI chat completion service
        //////////////////////////////////////////////////////////////////////////////////
        var AzureOpenAIDeploymentName = Env.Var("AzureOpenAI:ChatCompletionDeploymentName")!;
        var AzureOpenAIModelId = Env.Var("AzureOpenAI:ChatCompletionModelId")!;
        var AzureOpenAIEndpoint = Env.Var("AzureOpenAI:Endpoint")!;
        var AzureOpenAIApiKey = Env.Var("AzureOpenAI:ApiKey")!;
        var pluginDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins", "WriterPlugin");

        var builder = Kernel.CreateBuilder();
        builder.Services.AddAzureOpenAIChatCompletion(
            deploymentName: AzureOpenAIDeploymentName,  // The name of your deployment (e.g., "gpt-35-turbo")
            endpoint: AzureOpenAIEndpoint,              // The endpoint of your Azure OpenAI service
            apiKey: AzureOpenAIApiKey,                  // The API key of your Azure OpenAI service
            modelId: AzureOpenAIModelId                 // The model ID of your Azure OpenAI service
        );
        builder.Plugins.AddFromPromptDirectory(pluginDirectory);
        var kernel = builder.Build();

        var result = await kernel.InvokeAsync("WriterPlugin", "ShortPoem", new() {
            { "input", "Hello world" }
        });
        Console.WriteLine(result);
    }
    else
    {
        // Create a kernel with an Azure OpenAI text generation service
        //////////////////////////////////////////////////////////////////////////////////
        var AzureOpenAIDeploymentName = Env.Var("AzureOpenAI:TextCompletionDeploymentName")!;
        var AzureOpenAIModelId = Env.Var("AzureOpenAI:ChatCompletionModelId")!;
        var AzureOpenAIEndpoint = Env.Var("AzureOpenAI:Endpoint")!;
        var AzureOpenAIApiKey = Env.Var("AzureOpenAI:ApiKey")!;
        var pluginDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins", "WriterPlugin");

        var builder = Kernel.CreateBuilder();
        builder.Services.AddAzureOpenAITextGeneration(
            deploymentName: AzureOpenAIDeploymentName,  // The name of your deployment (e.g., "text-davinci-003")
            endpoint: AzureOpenAIEndpoint,              // The endpoint of your Azure OpenAI service
            apiKey: AzureOpenAIApiKey,                  // The API key of your Azure OpenAI service
            modelId: AzureOpenAIModelId                 // The model ID of your Azure OpenAI service
        );
        builder.Plugins.AddFromPromptDirectory(pluginDirectory);
        var kernel = builder.Build();

        var result = await kernel.InvokeAsync("WriterPlugin", "ShortPoem", new() {
            { "input", "Hello world" }
        });
        Console.WriteLine(result);
    }
}
else
{
    var OpenAIModelType = Env.Var("OpenAI:ModelType")!;

    if (OpenAIModelType == "chat-completion")
    {
        // Create a kernel with an OpenAI chat completion service
        //////////////////////////////////////////////////////////////////////////////////
        var OpenAIModelId = Env.Var("OpenAI:ChatCompletionModelId")!;
        var OpenAIApiKey = Env.Var("OpenAI:ApiKey")!;
        var OpenAIOrgId = Env.Var("OpenAI:OrgId")!;
        var pluginDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins", "WriterPlugin");

        var builder = Kernel.CreateBuilder();
        builder.Services.AddOpenAIChatCompletion(
            modelId: OpenAIModelId,              // The name of your deployment (e.g., "gpt-3.5-turbo")
            apiKey: OpenAIApiKey,                // The API key of your Azure OpenAI service
            orgId: OpenAIOrgId                   // The endpoint of your Azure OpenAI service
        );
        builder.Plugins.AddFromPromptDirectory(pluginDirectory);
        var kernel = builder.Build();

        var result = await kernel.InvokeAsync("WriterPlugin", "ShortPoem", new() {
            { "input", "Hello world" }
        });
        Console.WriteLine(result);
    }
    else
    {
        // Create a kernel with an OpenAI text generation service
        //////////////////////////////////////////////////////////////////////////////////
        var OpenAIModelId = Env.Var("OpenAI:TextCompletionModelId")!;
        var OpenAIApiKey = Env.Var("OpenAI:ApiKey")!;
        var OpenAIOrgId = Env.Var("OpenAI:OrgId")!;
        var pluginDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins", "WriterPlugin");

        var builder = Kernel.CreateBuilder();
        builder.Services.AddOpenAITextGeneration(
            modelId: OpenAIModelId,              // The name of your deployment (e.g., "gpt-3.5-turbo")
            apiKey: OpenAIApiKey,                // The API key of your Azure OpenAI service
            orgId: OpenAIOrgId                   // The endpoint of your Azure OpenAI service
        );
        builder.Plugins.AddFromPromptDirectory(pluginDirectory);
        var kernel = builder.Build();

        var result = await kernel.InvokeAsync("WriterPlugin", "ShortPoem", new() {
            { "input", "Hello world" }
        });
        Console.WriteLine(result);
    }
}