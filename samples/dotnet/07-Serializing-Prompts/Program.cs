// Copyright (c) Microsoft. All rights reserved.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.PromptTemplate.Handlebars;

// Create kernel
var builder = Kernel.CreateBuilder();
// Add a text or chat completion service using either:
// builder.Services.AddAzureOpenAIChatCompletion()
// builder.Services.AddAzureOpenAITextGeneration()
// builder.Services.AddOpenAIChatCompletion()
// builder.Services.AddOpenAITextGeneration()
builder.WithCompletionService();
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
builder.Plugins.AddFromType<ConversationSummaryPlugin>();
var kernel = builder.Build();

// Load prompts
var prompts = kernel.CreatePluginFromPromptDirectory("Prompts");

// Load prompt from YAML
using StreamReader reader = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("prompts.getIntent.prompt.yaml")!);
KernelFunction getIntent = kernel.CreateFunctionFromPromptYaml(
    reader.ReadToEnd(),
    promptTemplateFactory: new HandlebarsPromptTemplateFactory()
);

// Create chat history
ChatHistory history = [];

// Create choices
List<string> choices = ["ContinueConversation", "EndConversation"];

// Create few-shot examples
List<ChatHistory> fewShotExamples = [
    [
        new ChatMessageContent(AuthorRole.User, "Can you send a very quick approval to the marketing team?"),
        new ChatMessageContent(AuthorRole.System, "Intent:"),
        new ChatMessageContent(AuthorRole.Assistant, "ContinueConversation")
    ],
    [
        new ChatMessageContent(AuthorRole.User, "Can you send the full update to the marketing team?"),
        new ChatMessageContent(AuthorRole.System, "Intent:"),
        new ChatMessageContent(AuthorRole.Assistant, "EndConversation")
    ]
];

// Start the chat loop
while (true)
{
    // Get user input
    Console.Write("User > ");
    var request = Console.ReadLine();

    // Invoke handlebars prompt
    var intent = await kernel.InvokeAsync(
        getIntent,
        new() {
            { "request", request },
            { "choices", choices },
            { "history", history },
            { "fewShotExamples", fewShotExamples }
        }
    );

    // End the chat if the intent is "Stop"
    if (intent.ToString() == "EndConversation")
    {
        break;
    }

    // Get chat response
    var chatResult = kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
        prompts["chat"],
        new() {
            { "request", request },
            { "history", string.Join("\n", history.Select(x => x.Role + ": " + x.Content)) }
        }
    );

    // Stream the response
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

