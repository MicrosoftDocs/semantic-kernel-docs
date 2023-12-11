// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using AIPlugins.AzureFunctions.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Models;

const string DefaultSemanticFunctionsFolder = "Prompts";
string semanticFunctionsFolder = Environment.GetEnvironmentVariable("SEMANTIC_SKILLS_FOLDER") ?? DefaultSemanticFunctionsFolder;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(configuration =>
    {
        var config = configuration.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        var builtConfig = config.Build();
    })
    .ConfigureServices(services =>
    {
        services.Configure<JsonSerializerOptions>(options =>
        {
            // `ConfigureFunctionsWorkerDefaults` sets the default to ignore casing already.
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
        {
            var options = new OpenApiConfigurationOptions()
            {
                Info = new OpenApiInfo()
                {
                    Version = "1.0.0",
                    Title = "My Plugin",
                    Description = "This plugin does..."
                },
                Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
                OpenApiVersion = OpenApiVersionType.V3,
                //IncludeRequestingHostName = true,
                ForceHttps = false,
                ForceHttp = false,
            };

            return options;
        });
        services
        .AddScoped<Kernel>(serviceProvider =>
        {
            // This will be called each time a new Kernel is needed

            // Get a logger instance
            ILogger<Kernel> logger = serviceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger<Kernel>();

            // Register your AI Providers...
            var appSettings = AppSettings.LoadSettings();
            Kernel kernel = new KernelBuilder()
                .WithChatCompletionService(appSettings.Kernel)
                // .Services.AddLogging(logger)
                .Build();

            // Load your semantic functions...
            kernel.ImportPromptsFromDirectory(appSettings.AIPlugin.NameForModel, semanticFunctionsFolder);

            return kernel;
        })
        .AddScoped<IAIPluginRunner, AIPluginRunner>();
    })
    .Build();

host.Run();
