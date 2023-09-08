// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using System.Reflection;

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
            .AddUserSecrets(Assembly.GetExecutingAssembly(), false)
            .AddJsonFile("appsettings.json", true)
            .Build()
            .Get<Configuration>()!;

        bool useContext = config.Application.UseContext;
        
        var aIServiceType = config.Service.Type;
        switch(aIServiceType)
        {
            case AIService.AzureOpenAI:

                var model = config.Service.AzureOpenAI!.ChatModelDeploymentName;
                var endpoint = config.Service.AzureOpenAI.Endpoint;
                var aPIKey = config.Service.AzureOpenAI.APIKey; // From UserSecrets

                // Build your semantic kernel.
                kernel = new KernelBuilder()
                    .WithAzureChatCompletionService(model, endpoint, aPIKey)
                    .Build();

                Console.WriteLine($"Service: {aIServiceType}");
                Console.WriteLine($"ChatModelDeploymentName: {model}");
                Console.WriteLine($"Endpoint: {endpoint}\r\n");
                break;
            case AIService.OpenAI:

                model = config.Service.OpenAI!.ChatModelName;
                aPIKey = config.Service.OpenAI.APIKey; // From UserSecrets

                // Build your semantic kernel.
                kernel = new KernelBuilder()
                    .WithOpenAIChatCompletionService(model, aPIKey)
                    .Build();

                Console.WriteLine($"Service: {aIServiceType}");
                Console.WriteLine($"ChatModelName: {model}\r\n");
                break;
            default:
                Console.WriteLine("Invalid AI Service provided.");
                return 1;
        }

        // Create an inline semantic function: context variables, prompt, prompt configuration.
        // (NOTE: This is not the standard approach. Used here for simplicity.)

        // Initialize the context variables that will be used.
        var chatFunctionVariables = new ContextVariables
        {
            [ContextVariableKeyHistory] = string.Empty,
            [ContextVariableKeyUserInput] = string.Empty,
        };

        // Initialize the prompt.
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

        // Register the semantic function with your semantic kernel.
        // (NOTE: This is not the standard approach. Used here for simplicity.)
        var chatPromptTemplate = new PromptTemplate(chatFunctionPrompt, chatFunctionPromptConfig, kernel);
        var chatFunctionConfig = new SemanticFunctionConfig(chatFunctionPromptConfig, chatPromptTemplate);
        var chatFunction = kernel.RegisterSemanticFunction(FunctionNameChat, chatFunctionConfig);

        // Chat!
        // Send initial prompt (run semantic function) using context variables (input) and receive chat completion (output).
        var chatCompletion = await kernel.RunAsync(chatFunction, chatFunctionVariables);
        Console.WriteLine("To finish the chat session, press only <Enter>.\r\n");
        Console.WriteLine(PromptStringChatBot + chatCompletion);
        Console.Write(PromptStringUser);

        string history = string.Empty;
        string userInput = string.Empty;

        // Important: As history context grows in size, so does the token count usage.
        //            Chat will not function correctly once token limit reached.
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
            //            Chat will not function correctly once token limit reached.
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
