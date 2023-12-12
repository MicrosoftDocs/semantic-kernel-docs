// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

// Create kernel
var builder = new KernelBuilder();
// Add a text or chat completion service using either:
// builder.Services.AddAzureOpenAIChatCompletion()
// builder.Services.AddAzureOpenAIChatCompletion()
// builder.Services.AddAzureOpenAIChatCompletion()
// builder.Services.AddAzureOpenAIChatCompletion()
builder.WithCompletionService();
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Information));
builder.Plugins.AddFromType<ConversationSummaryPlugin>();

var kernel = builder.Build();

string history = @"AI response: How can I help you?
User Input: What's the weather like today?
AI response: Where are you located?
User Input: I'm in Seattle.
AI response: It's 70 degrees and sunny in Seattle today.
User Input: Thanks! I'll wear shorts.
AI response: You're welcome.
User Input: Could you remind me what I have on my calendar today?
AI response: You have a meeting with your team at 2:00 PM.
User Input: Oh right! My team just hit a major milestone; I should send them an email to congratulate them.";

string lastMessage = "AI response: Would you like to write one for you?";

string prompt = @"Instructions: What is the intent of this request?
Choices: {{$choices}}.

Prior conversation summary: The marketing team needs an update on the new product.
AI response: What do you want to tell them?
User Input: Can you send a very quick approval to the marketing team?
Intent: SendMessage

Prior conversation summary: The AI offered to send an email to the marketing team.
AI response: Do you want me to send an email to the marketing team?
User Input: Yes, please.
Intent: SendEmail

Prior conversation summary: {{ConversationSummaryPlugin.SummarizeConversation $history}}
{{$lastMessage}}
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
        { "choices", "SendEmail, SendMessage, CompleteTask, CreateDocument" },
        { "history", history },
        { "lastMessage", lastMessage }
    }
);
Console.WriteLine(result);