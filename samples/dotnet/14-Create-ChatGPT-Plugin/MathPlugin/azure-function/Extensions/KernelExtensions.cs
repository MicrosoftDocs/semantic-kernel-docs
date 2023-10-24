// Copyright (c) Microsoft. All rights reserved.

using System.Diagnostics.Contracts;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TemplateEngine;

namespace Extensions;

public static class KernelExtensions
{
    public static IDictionary<string, ISKFunction> ImportPromptsFromDirectory(
        this IKernel kernel, string pluginName, string promptDirectory)
    {
        Contract.Requires(kernel != null);

        const string CONFIG_FILE = "config.json";
        const string PROMPT_FILE = "skprompt.txt";

        var plugin = new Dictionary<string, ISKFunction>();

        string[] directories = Directory.GetDirectories(promptDirectory);
        foreach (string dir in directories)
        {
            var functionName = Path.GetFileName(dir);

            // Continue only if prompt template exists
            var promptPath = Path.Combine(dir, PROMPT_FILE);
            if (!File.Exists(promptPath)) { continue; }

            // Load prompt configuration. Note: the configuration is optional.
            var config = new PromptTemplateConfig();
            var configPath = Path.Combine(dir, CONFIG_FILE);
            if (File.Exists(configPath))
            {
                config = PromptTemplateConfig.FromJson(File.ReadAllText(configPath));
            }

            // Load prompt template
            var template = new PromptTemplate(File.ReadAllText(promptPath), config, kernel!.PromptTemplateEngine);

            plugin[functionName] = kernel.RegisterSemanticFunction(pluginName, functionName, config, template);
        }

        return plugin;
    }
}
