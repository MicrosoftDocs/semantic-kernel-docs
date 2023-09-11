// Copyright (c) Microsoft. All rights reserved.

namespace Chat101;

internal record Configuration
{
    public Configuration(Application? application = null)
    {
        Application = application ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(Application)}'");
    }
    public Application Application { get; }
}

internal record Application
{
    public Application(bool? useContext)
    {
        UseContext = useContext ?? throw new ArgumentOutOfRangeException($"The configuration is missing required values for section: '{nameof(useContext)}'"); ;
    }

    public bool UseContext { get; set; }
}