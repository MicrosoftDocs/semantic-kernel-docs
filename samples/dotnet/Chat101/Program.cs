// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;

namespace Chat101;

class Program
{
    private const string ContextVariableKeyHistory = "history";
    private const string ContextVariableKeyUserInput = "userInput";
    private const string FunctionNameChat = "Chat";
    private const string PromptStringUser = "User: ";
    private const string PromptStringChatBot = "ChatBot: ";

    static async Task<int> Main(string[] args)
    {
        IKernel kernel;

        // Configure application.
        Configuration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .Build()
            .Get<Configuration>()!;

        bool useContext = config.Application.UseContext;
        Console.WriteLine($"useContext: {useContext}");
        
        // Build your semantic kernel.
        // (NOTE: `WithCompletionSerice()` is a custom function found in ./config/KernelBuilderExtensions.cs.)
        kernel = new KernelBuilder()
            .WithCompletionService()
            .Build();

        // Create an inline semantic function: prompt string + prompt configuration.
        // (NOTE: This is not the standard approach. Used here for simplicity.)

        // Initialize the prompt string.
        string chatFunctionPrompt = 
            @$"{{{{${ContextVariableKeyHistory}}}}}
            {PromptStringUser} {{{{${ContextVariableKeyUserInput}}}}}
            {PromptStringChatBot}";

        // Initialize the prompt configuration.
        var chatFunctionPromptConfig = new PromptTemplateConfig
        {
            Completion = 
            {
                MaxTokens = 2000,
                Temperature = 0.7,
                TopP = 0.5,
            }
        };

        // Initialize the context variables (semantic function input).
        var chatFunctionVariables = new ContextVariables
        {
            [ContextVariableKeyHistory] = string.Empty,
            [ContextVariableKeyUserInput] = string.Empty,
        };

        // Register the semantic function with your semantic kernel.
        // (NOTE: This is not the standard approach. Used here for simplicity.)
        var chatPromptTemplate = new PromptTemplate(chatFunctionPrompt, chatFunctionPromptConfig, kernel);
        var chatFunctionConfig = new SemanticFunctionConfig(chatFunctionPromptConfig, chatPromptTemplate);
        var chatFunction = kernel.RegisterSemanticFunction(FunctionNameChat, chatFunctionConfig);

        // Chat!
        // Send initial prompt (run semantic function) using context variables (input) and receive 
        // chat completion (output).
        var chatCompletion = await kernel.RunAsync(chatFunction, chatFunctionVariables);
        Console.WriteLine("To finish the chat session, press only <Enter>.\r\n");
        Console.WriteLine(PromptStringChatBot + chatCompletion);
        Console.Write(PromptStringUser);

        string history = string.Empty;
        string userInput = string.Empty;

        // Important: As history context grows in size, so does the token count usage.
        //            Chat will not function correctly once token limit is reached.
        //            By default, history context is sent in prompts.
        if (useContext) 
        {
            history += 
                @$"{PromptStringUser}{userInput}
                {PromptStringChatBot}{chatCompletion}
                ";
            chatFunctionVariables.Set(ContextVariableKeyHistory, history);
        }

        // Continue conversation until user presses only <Enter>. 
        while (!string.IsNullOrEmpty(userInput = Console.ReadLine()!))
        {
            // Include user input in prompts and receive model completions.
            chatFunctionVariables.Set(ContextVariableKeyUserInput, userInput);
            chatCompletion = await kernel.RunAsync(chatFunction, chatFunctionVariables);
            Console.WriteLine(PromptStringChatBot + chatCompletion);
            Console.Write(PromptStringUser);

            // Important: As history context grows in size, so does the token count usage.
            //            Chat will not function correctly once token limit is reached.
            //            By default, update history context and send in prompts.
            if (useContext)
            {
                history += 
                    @$"{PromptStringUser}{userInput}
                    {PromptStringChatBot}{chatCompletion}
                    ";
                chatFunctionVariables.Set(ContextVariableKeyHistory, history);
            }
        }
        return 0;
    }
}
