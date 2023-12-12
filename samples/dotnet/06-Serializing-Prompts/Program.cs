// Copyright (c) Microsoft. All rights reserved.

using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;

using ILoggerFactory loggerFactory = LoggerFactory.Create(Builder =>
{
    Builder
        .SetMinimumLevel(0)
        .AddDebug();
});

// Create kernel
var builder = new KernelBuilder();
// Add a text or chat completion service using either:
// builder.Services.AddAzureOpenAIChatCompletion()
// builder.Services.AddAzureOpenAITextGeneration()
// builder.Services.AddOpenAIChatCompletion()
// builder.Services.AddOpenAITextGeneration()
builder.WithCompletionService();
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Information));
var kernel = builder.Build();

// Load prompts
var prompts = kernel.CreatePluginFromPromptDirectory("Prompts");

// Create history and choices
ChatHistory history = [];
string choices = string.Join(", ", ["Continue", "Stop"]);

// Start chat loop
while (true)
{
    // Get user input
    Console.Write("User > ");
    var request = Console.ReadLine();

    // Invoke prompt
    var intent = await kernel.InvokeAsync(
        prompts["getIntent"],
        new() {
            { "request", request },
            { "choices", choices },
            { "history", history.Select(x => x.Role + ": " + x.Content).ToList() }
        }
    );

    // End the chat if the intent is "Stop"
    if (intent.ToString() == "Stop")
    {
        break;
    }

    // Get chat response
    var chatResult = kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
        prompts["chat"],
        new() {
            { "request", request },
            { "history", history.Select(x => x.Role + ": " + x.Content).ToList() }
        }
    );

    string message = "";
    await foreach (var chunk in chatResult)
    {
        if (chunk.Role.HasValue) Console.Write(chunk.Role + " > ");
        message += chunk;
        Console.Write(chunk);
    }
    Console.WriteLine();

    // Append to history
    history.AddUserMessage(request!);
    history.AddAssistantMessage(message);
}

