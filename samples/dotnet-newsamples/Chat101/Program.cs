// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.Orchestration;

namespace Chat101;

class Program
{
    private const string contextVariableKeyHistory = "history";
    private const string contextVariableKeyUserInput = "userInput";
    private const string functionNameChat = "Chat";
    private const string promptStringUser = "User: ";
    private const string promptStringChatBot = "ChatBot: ";

    static async Task<int> Main(string[] args)
    {
        // Configure application and build your semantic kernel to use a chat LLM:
        var (aiService, model, apiKey, endpoint, useContext) = Configure.LoadFromFile();

        var builder = new KernelBuilder();
        switch(aiService)
        {
            case Configure.AIService.AzureOpenAI:
                builder.WithAzureChatCompletionService(model, endpoint, apiKey);
                break;
            case Configure.AIService.OpenAI:
                builder.WithOpenAIChatCompletionService(model, apiKey);
                break;
            default:
                Console.WriteLine("Invalid AI Service provided.");
                return 1;
        }
        IKernel kernel = builder.Build();

        bool useHistoryContext = useContext;
        
        // Create an inline semantic function:
        // Initialize the context variables that will be used.
        var chatFunctionVariables = new ContextVariables
        {
            [contextVariableKeyHistory] = string.Empty,
            [contextVariableKeyUserInput] = string.Empty,
        };

        // Initialize the prompt.
        string chatFunctionPrompt = 
            @$"{{{{${contextVariableKeyHistory}}}}}
            {promptStringUser} {{{{${contextVariableKeyUserInput}}}}}
            {promptStringChatBot}";

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

        // Register the semantic function with your semantic kernel:
        var chatPromptTemplate = new PromptTemplate(chatFunctionPrompt, chatFunctionPromptConfig, kernel);
        var chatFunctionConfig = new SemanticFunctionConfig(chatFunctionPromptConfig, chatPromptTemplate);
        var chatFunction = kernel.RegisterSemanticFunction(functionNameChat, chatFunctionConfig);

        // Chat:
        // Send initial prompt (run semantic function) using context variables (input) and receive chat completion (output).
        var chatCompletion = await kernel.RunAsync(chatFunction, chatFunctionVariables);
        Console.WriteLine("To finish the chat session, press only <Enter>.\r\n");
        Console.WriteLine(promptStringChatBot + chatCompletion);
        Console.Write(promptStringUser);

        string history = string.Empty;
        string userInput = string.Empty;

        if (useHistoryContext) // Note: By default, history context is sent in prompts.
        {
            history += 
                @$"{promptStringUser}{userInput}
                {promptStringChatBot}{chatCompletion}
                ";
            chatFunctionVariables.Set(contextVariableKeyHistory, history);
        }

        // Continue conversation until user presses only <Enter> 
        while (!string.IsNullOrEmpty(userInput = Console.ReadLine()!))
        {
            // Include user input in prompts and receive model completions.
            chatFunctionVariables.Set(contextVariableKeyUserInput, userInput);
            chatCompletion = await kernel.RunAsync(chatFunction, chatFunctionVariables);
            Console.WriteLine(promptStringChatBot + chatCompletion);
            Console.Write(promptStringUser);

            if (useHistoryContext) // Note: By default, update history context.
            {
                history += 
                    @$"{promptStringUser}{userInput}
                    {promptStringChatBot}{chatCompletion}
                    ";
                chatFunctionVariables.Set(contextVariableKeyHistory, history);
            }
        }

        return 0;
    }
}
