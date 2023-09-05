// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Chat101;

public class Configure
{
    private const string configFile = "appsettings.json";
    private const string configService = "Service";
    private const string configAIService = "AIService";
    private const string configModel = "ChatModelName";
    private const string configAPIKey = "APIKey";
    private const string configEndpoint = "AzureOpenAIEndpoint";
    private const string configApplication = "Application";
    private const string configUseContext = "UseContext";

    public enum AIService
    {
        Undefined,
        AzureOpenAI,
        OpenAI
    }

    public static (AIService aiService, string model, string apiKey, string endpoint, bool useContext) LoadFromFile()
    {
        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFile)
                .Build();
            
            if (config is not null)
            {
                AIService aiService = AIService.Undefined;
                var serviceSection = config.GetSection(configService);
                var aiServiceValue = serviceSection.GetValue<string>(configAIService);
                 
                switch(aiServiceValue)
                {
                    case "AzureOpenAI":
                        aiService = AIService.AzureOpenAI;
                        break;
                    case "OpenAI":
                        aiService = AIService.OpenAI;
                        break;
                    default:
                        Console.WriteLine("AI service is not supported.");
                        return (AIService.Undefined, "", "", "", true);
                }

                var model = serviceSection.GetValue<string>(configModel);
                var endpoint = serviceSection.GetValue<string>(configEndpoint);
                var apiKey = serviceSection.GetValue<string>(configAPIKey);

                var applicationSection = config.GetSection(configApplication);
                var useContext = applicationSection.GetValue<bool>(configUseContext);
            
                return (aiService, model, apiKey, endpoint, useContext);  
            }
            else
            {
                Console.WriteLine("Configuration is null.");
                return (AIService.Undefined, "", "", "", true);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
            return (AIService.Undefined, "", "", "", true);
        }
    }
}
