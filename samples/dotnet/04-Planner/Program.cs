using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Plugins;

var kernelSettings = KernelSettings.LoadSettings();
IKernel kernel = new KernelBuilder()
    .WithCompletionService(kernelSettings)
    .Build();

var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins");
kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "OrchestratorPlugin");
kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "SummarizeSkill");

// Add the math plugin
var mathPlugin = kernel.ImportSkill(new MathPlugin(), "MathPlugin");

// Create a planner
var planner = new SequentialPlanner(kernel);

var ask = "I have $2130.23. How much would I have after it grew by 24% and after I spent $5 on a latte?";
var plan = await planner.CreatePlanAsync(ask);

// Console.WriteLine("Plan:\n");
// Console.WriteLine(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true }));

var result = await plan.InvokeAsync();

Console.WriteLine("Plan results:");
Console.WriteLine(result.Result.Trim());