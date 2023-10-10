// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;
using static Microsoft.SemanticKernel.SemanticFunctions.PromptTemplateConfig;

// Create the prompt for the semantic function
string prompt = @"Bot: How can I help you?
User: {{$input}}

---------------------------------------------

The intent of the user in 5 words or less: ";

// Create the configuration for the semantic function
var promptConfig = new PromptTemplateConfig
{
    Schema = 1,
    Type = "completion",
    Description = "Gets the intent of the user.",
    Completion =
    {
        MaxTokens = 500,
        Temperature = 0.0,
        TopP = 0.0,
        PresencePenalty = 0.0,
        FrequencyPenalty = 0.0
     },
    Input =
     {
        Parameters = new List<InputParameter>
        {
            new InputParameter
            {
                Name = "input",
                Description = "The user's request.",
                DefaultValue = ""
            }
        }
    }
};

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(0)
        .AddDebug();
});

// Create the Kernel
IKernel kernel = new KernelBuilder()
    // Add a text or chat completion service using either:
    // .WithAzureTextCompletionService()
    // .WithAzureChatCompletionService()
    // .WithOpenAITextCompletionService()
    // .WithOpenAIChatCompletionService()
    .WithCompletionService()
    .WithLoggerFactory(loggerFactory)
    .Build();

// Create the SemanticFunctionConfig object
var promptTemplate = new PromptTemplate(
    prompt,
    promptConfig,
    kernel
);
var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

// Register the GetIntent function with the Kernel
var getIntentFunction = kernel.RegisterSemanticFunction("OrchestratorPlugin", "GetIntent", functionConfig);

// Run the GetIntent function
var result = await kernel.RunAsync(
    "I want to send an email to the marketing team celebrating their recent milestone.",
    getIntentFunction
);

Console.WriteLine(result.GetValue<string>());