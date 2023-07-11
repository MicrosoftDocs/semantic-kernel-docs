using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;

// Create a logger
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(0)
        .AddDebug();
});
var logger = loggerFactory.CreateLogger<Kernel>();

// Create a kernel
var kernelSettings = KernelSettings.LoadSettings();
IKernel kernel = new KernelBuilder()
    .WithCompletionService(kernelSettings)
    .WithLogger(logger)
    .Build();

// Add the math plugin using the plugin manifest URL
const string pluginManifestUrl = "http://localhost:7071/api/ai-plugin.json";
var mathPlugin = await kernel.ImportChatGptPluginSkillFromUrlAsync("MathPlugin", new Uri(pluginManifestUrl));

// Create a stepwise planner and invoke it
var planner = new StepwisePlanner(kernel);
var question = "I have $2130.23. How much would I have after it grew by 24% and after I spent $5 on a latte?";
var plan = planner.CreatePlan(question);
var result = await plan.InvokeAsync(kernel.CreateNewContext());

// Print the results
Console.WriteLine("Result: " + result);
if (result.Variables.TryGetValue("stepCount", out string? stepCount))
{
    Console.WriteLine("Steps Taken: " + stepCount);
}
if (result.Variables.TryGetValue("skillCount", out string? skillCount))
{
    Console.WriteLine("Skills Used: " + skillCount);
}