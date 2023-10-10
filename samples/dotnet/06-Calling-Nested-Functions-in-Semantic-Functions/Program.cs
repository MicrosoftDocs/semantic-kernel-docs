// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Plugins.Core;

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

// Import the OrchestratorPlugin and ConversationSummarySkill into the kernel.
var orchestrationPlugin = kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "OrchestratorPlugin");
var conversationSummaryPlugin = kernel.ImportFunctions(new ConversationSummarySkill(kernel), "ConversationSummarySkill");

// Create a new context and set the input, history, and options variables.
var variables = new ContextVariables
{
    ["input"] = "Yes",
    ["history"] = @"Bot: How can I help you?
User: What's the weather like today?
Bot: Where are you located?
User: I'm in Seattle.
Bot: It's 70 degrees and sunny in Seattle today.
User: Thanks! I'll wear shorts.
Bot: You're welcome.
User: Could you remind me what I have on my calendar today?
Bot: You have a meeting with your team at 2:00 PM.
User: Oh right! My team just hit a major milestone; I should send them an email to congratulate them.
Bot: Would you like to write one for you?",
    ["options"] = "SendEmail, ReadEmail, SendMeeting, RsvpToMeeting, SendChat"
};

// Run the GetIntent function with the context.
var result = (await kernel.RunAsync(variables, orchestrationPlugin["GetIntent"])).Result;

Console.WriteLine(result.GetValue<string>());
