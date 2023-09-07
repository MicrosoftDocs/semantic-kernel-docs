// Copyright (c) Microsoft. All rights reserved.
using static System.Net.Mime.MediaTypeNames;

namespace Chat101;

internal record Configuration
{
    public Configuration(Service? service, Application? application)
    {
        Service = service ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(service)}'");
        Application = application ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(application)}'"); ;
    }

    public Service Service { get; }

    public Application Application { get; }
}

public enum AIService
{
    Undefined,
    AzureOpenAI,
    OpenAI
}

internal record Service
{
    public Service(AIService? aIService, string? chatModelName, string? aPIKey, string? endpoint)
    {
        AIService = aIService ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(aIService)}'");

        if (string.IsNullOrWhiteSpace( chatModelName ) )
        {
            throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(chatModelName)}'");
        }

        if (string.IsNullOrWhiteSpace(aPIKey))
        {
            throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(aPIKey)}'");
        }

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(endpoint)}'");
        }

        ChatModelName = chatModelName;
        APIKey = aPIKey;
        AzureOpenAIEndpoint = endpoint;
    }

    public AIService AIService { get; }

    public string ChatModelName { get; }

    public string APIKey { get; }

    public string AzureOpenAIEndpoint { get; }
}

internal record Application
{
    public Application(bool? useContext)
    {
        UseContext = useContext ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(useContext)}'"); ;
    }

    public bool UseContext { get; set; }
}

