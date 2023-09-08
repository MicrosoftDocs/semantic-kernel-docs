// Copyright (c) Microsoft. All rights reserved.
using static System.Net.Mime.MediaTypeNames;

namespace Chat101;

internal record Configuration
{
    public Configuration(Service? service = null, Application? application = null)
    {
        Service = service ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(service)}'");
        Application = application ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(application)}'");
    }

    public Service Service { get; }

    public Application Application { get; }

    public string? AzureOpenAI_APIKey { get; set; } // From user secrets

    public string? OpenAI_APIKey { get; set; } // From user secrets
    
}

public enum AIService
{
    Undefined,
    AzureOpenAI,
    OpenAI
}

internal record Service
{
    public Service(AIService? type, AzureOpenAI? azureOpenAI = null, OpenAI? openAI = null)
    {
        Type = type ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(type)}'");
        AzureOpenAI = azureOpenAI ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(azureOpenAI)}'");
        OpenAI = openAI ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(openAI)}'");
    }

    public AIService Type { get; }

    public AzureOpenAI AzureOpenAI { get; }
    
    public OpenAI OpenAI { get; }
}

internal record AzureOpenAI
{
    public AzureOpenAI(string? chatModelDeploymentName, string? endpoint)
    {
        if (string.IsNullOrWhiteSpace(chatModelDeploymentName))
        {
            throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(chatModelDeploymentName)}'");
        }

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(endpoint)}'");
        }

        ChatModelDeploymentName = chatModelDeploymentName;
        Endpoint = endpoint;
    }

    public string ChatModelDeploymentName { get; }

    public string Endpoint { get; }

    public string? APIKey { get; }
}

internal record OpenAI
{
    public OpenAI(string? chatModelName)
    {
        if (string.IsNullOrWhiteSpace(chatModelName))
        {
            throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(chatModelName)}'");
        }

        ChatModelName = chatModelName;
    }

    public string ChatModelName { get; }

    public string? APIKey { get; }
}




internal record Application
{
    public Application(bool? useContext)
    {
        UseContext = useContext ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(useContext)}'"); ;
    }

    public bool UseContext { get; set; }
}

