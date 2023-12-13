// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Plugins;

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(0)
        .AddDebug();
});

// Create kernel
var builder = Kernel.CreateBuilder();
// Add a text or chat completion service using either:
// builder.Services.AddAzureOpenAIChatCompletion()
// builder.Services.AddAzureOpenAITextGeneration()
// builder.Services.AddOpenAIChatCompletion()
// builder.Services.AddOpenAITextGeneration()
builder.WithCompletionService();
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
builder.Plugins.AddFromType<MathSolver>();
var kernel = builder.Build();

// Create chat history
ChatHistory history = [];

// Get chat completion service
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Start the conversation
while (true)
{
    // Get user input
    Console.Write("User > ");
    history.AddUserMessage(Console.ReadLine()!);

    // Enable auto function calling
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    // Get the response from the AI
    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Stream the results
    string fullMessage = "";
    var first = true;
    await foreach (var content in result)
    {
        if (content.Role.HasValue && first)
        {
            Console.Write("Assistant > ");
            first = false;
        }
        Console.Write(content.Content);
        fullMessage += content.Content;
    }
    Console.WriteLine();

    // Add the message from the agent to the chat history
    history.AddAssistantMessage(fullMessage);
}