// Copyright (c) Microsoft. All rights reserved.
using static System.Net.Mime.MediaTypeNames;

namespace Chat101;

internal record Configuration
{
    public Configuration(Service? service = null, Application? application = null)
    {
        Service = service ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(Service)}'");
        Application = application ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(Application)}'");
    }

    public Service Service { get; }

    public Application Application { get; }
}

public enum AIService
{
    AzureOpenAI,
    OpenAI
}

internal record Service
{
    public Service(AIService? type, AzureOpenAI? azureOpenAI = null, OpenAI? openAI = null)
    {
        Type = type ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(Type)}'");

        if (Type == AIService.AzureOpenAI)
        {
            AzureOpenAI = azureOpenAI ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(AzureOpenAI)}'");
        }
        else if (Type == AIService.OpenAI)
        {
            OpenAI = openAI ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(OpenAI)}'");
        }
        // Configuration Binder will throw an exception if Type is anything else.
    }

    public AIService Type { get; }

    public AzureOpenAI? AzureOpenAI { get; }

    public OpenAI? OpenAI { get; }
}

internal record AzureOpenAI
{
    public AzureOpenAI(string? chatModelDeploymentName = null, string? endpoint = null, string? aPIKey= null)
    {
        if (string.IsNullOrWhiteSpace(chatModelDeploymentName))
        {
            throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(AzureOpenAI)}:{nameof(ChatModelDeploymentName)}'");
        }
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(AzureOpenAI)}:{nameof(Endpoint)}'");
        }
        if (string.IsNullOrWhiteSpace(aPIKey))
        {
            throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(AzureOpenAI)}:{nameof(APIKey)}'");
        }

        ChatModelDeploymentName = chatModelDeploymentName;
        Endpoint = endpoint;
        APIKey = aPIKey;
    }

    public string ChatModelDeploymentName { get; }

    public string Endpoint { get; }

    public string APIKey { get; }
}

internal record OpenAI
{
    public OpenAI(string? chatModelName = null, string? aPIKey = null)
    {
        if (string.IsNullOrWhiteSpace(chatModelName))
        {
            throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(OpenAI)}:{nameof(chatModelName)}'");
        }
        if (string.IsNullOrWhiteSpace(aPIKey))
        {
            throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(OpenAI)}:{nameof(APIKey)}'");
        }

        ChatModelName = chatModelName;
        APIKey = aPIKey;
    }

    public string ChatModelName { get; }

    public string APIKey { get; }
}

internal record Application
{
    public Application(bool? useContext)
    {
        UseContext = useContext ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(useContext)}'"); ;
    }

    public bool UseContext { get; set; }
}
