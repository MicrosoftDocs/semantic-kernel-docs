// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

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
// builder.Services.AddAzureOpenAIChatCompletion()
// builder.Services.AddAzureOpenAIChatCompletion()
// builder.Services.AddAzureOpenAIChatCompletion()
builder.WithCompletionService();
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Information));

var kernel = builder.Build();

string history = @"User input: I hate sending emails, no one ever reads them.
AI response: I'm sorry to hear that. Messages may be a better way to communicate.";

string prompt = @"Instructions: What is the intent of this request?
If you don't know the intent, don't guess; instead respond with ""Unknown"".
Choices: {{$choices}}.

User Input: Can you send a very quick approval to the marketing team?
Intent: SendMessage

User Input: Can you send the full update to the marketing team?
Intent: SendEmail

{{$history}}
User Input: {{$request}}
Intent: ";

// Get user input
Console.Write("User > ");
var request = Console.ReadLine();

// Invoke prompt
var result = await kernel.InvokePromptAsync(
    prompt,
    new() {
        { "request", request },
        { "choices", new[] { "SendEmail", "SendMessage", "CompleteTask", "CreateDocument", "Unknown" } },
        { "history", history }
    }
);
Console.WriteLine(result);