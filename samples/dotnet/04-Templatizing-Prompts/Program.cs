// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.PromptTemplate.Handlebars;

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

// 1.0 Using semantic-kernel template engine
//////////////////////////////////////////////////////////////////////////////////
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
        { "choices", "SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown" },
        { "history", history }
    }
);
Console.WriteLine(result);

// 2.0 Using Handlebars template engine
//////////////////////////////////////////////////////////////////////////////////

// Create chat history
ChatHistory chatHistory = [
    new ChatMessageContent(AuthorRole.User, "I hate sending emails, no one ever reads them."),
    new ChatMessageContent(AuthorRole.Assistant, "I'm sorry to hear that. Messages may be a better way to communicate.")
];

// Create few-shot examples
List<ChatHistory> fewShotExamples = [
    [
        new ChatMessageContent(AuthorRole.User, "Can you send a very quick approval to the marketing team?"),
        new ChatMessageContent(AuthorRole.System, "Intent:"),
        new ChatMessageContent(AuthorRole.Assistant, "SendMessage")
    ],
    [
        new ChatMessageContent(AuthorRole.User, "Can you send the full update to the marketing team?"),
        new ChatMessageContent(AuthorRole.System, "Intent:"),
        new ChatMessageContent(AuthorRole.Assistant, "SendEmail")
    ]
];

// Create prompt
var promptConfig = new PromptTemplateConfig()
{
    Template = @"
        <message role=""system"">Instructions: What is the intent of this request?
        If you don't know the intent, don't guess; instead respond with ""Unknown"".
        Choices: {{choices}}.</message>

        {{#each fewShotExamples}}
            {{#each this}}
                <message role=""{{role}}"">{{content}}</message>
            {{/each}}
        {{/each}}

        {{#each chatHistory}}
            <message role=""{{role}}"">{{content}}</message>
        {{/each}}

        <message role=""user"">{{request}}</message>
        <message role=""system"">Intent:</message>",
    TemplateFormat = "handlebars"
};

var promptFunction = kernel.CreateFunctionFromPrompt(
    promptConfig,
    new HandlebarsPromptTemplateFactory()
);

// Invoke prompt
var handleBarsResult = await kernel.InvokeAsync(
    promptFunction,
    new() {
        { "request", request },
        { "choices", "SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown" },
        { "history", history },
        { "fewShotExamples", fewShotExamples }
    }
);
Console.WriteLine(result);