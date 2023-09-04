// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;

var LlmService = Env.Var("Global:LlmService")!;

if (LlmService == "AzureOpenAI")
{
    var AzureOpenAIDeploymentType = Env.Var("AzureOpenAI:DeploymentType")!;

    if (AzureOpenAIDeploymentType == "chat-completion")
    {
        // Create a kernel with a logger and Azure OpenAI chat completion service
        //////////////////////////////////////////////////////////////////////////////////
        var AzureOpenAIDeploymentName = Env.Var("AzureOpenAI:ChatCompletionDeploymentName")!;
        var AzureOpenAIEndpoint = Env.Var("AzureOpenAI:Endpoint")!;
        var AzureOpenAIApiKey = Env.Var("AzureOpenAI:ApiKey")!;

        var kernelWithConfiguration = Kernel.Builder
            .WithAzureChatCompletionService(
                AzureOpenAIDeploymentName,  // The name of your deployment (e.g., "gpt-35-turbo")
                AzureOpenAIEndpoint,        // The endpoint of your Azure OpenAI service
                AzureOpenAIApiKey           // The API key of your Azure OpenAI service
            )
            .Build();

        var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins");
        var writerPlugin = kernelWithConfiguration
             .ImportSemanticSkillFromDirectory(pluginsDirectory, "WriterPlugin");

        var result = await kernelWithConfiguration.RunAsync("Hello world", writerPlugin["ShortPoem"]);
        Console.WriteLine(result);
    }
    else
    {
        // Create a kernel with a logger and Azure OpenAI text completion service
        //////////////////////////////////////////////////////////////////////////////////
        var AzureOpenAIDeploymentName = Env.Var("AzureOpenAI:TextCompletionDeploymentName")!;
        var AzureOpenAIEndpoint = Env.Var("AzureOpenAI:Endpoint")!;
        var AzureOpenAIApiKey = Env.Var("AzureOpenAI:ApiKey")!;

        var kernelWithConfiguration = Kernel.Builder
            .WithAzureTextCompletionService(
                AzureOpenAIDeploymentName,  // The name of your deployment (e.g., "text-davinci-003")
                AzureOpenAIEndpoint,        // The endpoint of your Azure OpenAI service
                AzureOpenAIApiKey           // The API key of your Azure OpenAI service
            )
            .Build();

        var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins");
        var writerPlugin = kernelWithConfiguration
             .ImportSemanticSkillFromDirectory(pluginsDirectory, "WriterPlugin");

        var result = await kernelWithConfiguration.RunAsync("Hello world", writerPlugin["ShortPoem"]);
        Console.WriteLine(result);
    }
}
else
{
    var OpenAIModelType = Env.Var("OpenAI:ModelType")!;

    if (OpenAIModelType == "chat-completion")
    {
        // Create a kernel with a logger and OpenAI chat completion service
        //////////////////////////////////////////////////////////////////////////////////
        var OpenAIModelId = Env.Var("OpenAI:ChatCompletionModelId")!;
        var OpenAIApiKey = Env.Var("OpenAI:ApiKey")!;
        var OpenAIOrgId = Env.Var("OpenAI:OrgId")!;

        var kernelWithConfiguration = Kernel.Builder
            .WithOpenAIChatCompletionService(
                OpenAIModelId,              // The name of your deployment (e.g., "gpt-3.5-turbo")
                OpenAIApiKey,               // The API key of your Azure OpenAI service
                OpenAIOrgId                 // The endpoint of your Azure OpenAI service
            )
            .Build();

        var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins");
        var writerPlugin = kernelWithConfiguration
            .ImportSemanticSkillFromDirectory(pluginsDirectory, "WriterPlugin");

        var result = await kernelWithConfiguration.RunAsync("Hello world", writerPlugin["ShortPoem"]);
        Console.WriteLine(result);
    }
    else
    {
        // Create a kernel with a logger and OpenAI chat completion service
        //////////////////////////////////////////////////////////////////////////////////
        var OpenAIModelId = Env.Var("OpenAI:TextCompletionModelId")!;
        var OpenAIApiKey = Env.Var("OpenAI:ApiKey")!;
        var OpenAIOrgId = Env.Var("OpenAI:OrgId")!;

        var kernelWithConfiguration = Kernel.Builder
            .WithOpenAITextCompletionService(
                OpenAIModelId,              // The name of your deployment (e.g., "text-davinci-003")
                OpenAIApiKey,               // The API key of your Azure OpenAI service
                OpenAIOrgId                 // The endpoint of your Azure OpenAI service
            )
            .Build();

        var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins");
        var writerPlugin = kernelWithConfiguration
            .ImportSemanticSkillFromDirectory(pluginsDirectory, "WriterPlugin");

        var result = await kernelWithConfiguration.RunAsync("Hello world", writerPlugin["ShortPoem"]);
        Console.WriteLine(result);
    }
}