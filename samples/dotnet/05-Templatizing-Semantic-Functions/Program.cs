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

// Import the OrchestratorPlugin and SummarizePlugin from the plugins directory.
var orchestrationPlugin = kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "OrchestratorPlugin");

// Create a new collection of context variables and set the input, history, and options variables.
var variables = new ContextVariables
{
    ["input"] = "Yes",
    ["history"] = @"Bot: How can I help you?
User: My team just hit a major milestone and I would like to send them a message to congratulate them.
Bot:Would you like to send an email?",
    ["options"] = "SendEmail, ReadEmail, SendMeeting, RsvpToMeeting, SendChat"
};

// Run the GetIntent function with the variables.
var result = (await kernel.RunAsync(variables, orchestrationPlugin["GetIntent"])).Result;

Console.WriteLine(result);