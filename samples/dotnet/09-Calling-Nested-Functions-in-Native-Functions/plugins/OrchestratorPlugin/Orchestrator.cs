// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Nodes;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Plugins.OrchestratorPlugin;

public class Orchestrator
{
    private readonly IKernel _kernel;

    public Orchestrator(IKernel kernel)
    {
        _kernel = kernel;
    }

    [SKFunction]
    public async Task<string> RouteRequestAsync(SKContext context)
    {
        // Save the original user request
        string request = context.Variables["input"];

        // Retrieve the intent from the user request
        var getIntent = this._kernel.Skills.GetFunction("OrchestratorPlugin", "GetIntent");
        var getIntentVariables = new ContextVariables
        {
            ["input"] = context.Variables["input"],
            ["options"] = "Sqrt, Multiply"
        };
        string intent = (await this._kernel.RunAsync(getIntentVariables, getIntent)).Result.Trim();

        // Retrieve the numbers from the user request
        var getNumbers = this._kernel.Skills.GetFunction("OrchestratorPlugin", "GetNumbers");
        string numbersJson = (await this._kernel.RunAsync(request, getNumbers)).Result;
        JsonObject numbers = JsonObject.Parse(numbersJson)!.AsObject();

        // Call the appropriate function
        switch (intent)
        {
            case "Sqrt":
                // Call the Sqrt function with the first number
                var sqrt = _kernel.Skills.GetFunction("MathPlugin", "Sqrt");
                return (await _kernel.RunAsync(numbers["number1"]!.ToString(), sqrt)).Result;
            case "Multiply":
                // Call the Multiply function with both numbers
                var multiply = _kernel.Skills.GetFunction("MathPlugin", "Multiply");
                var multiplyVariables = new ContextVariables
                {
                    ["input"] = numbers["number1"]!.ToString(),
                    ["number2"] = numbers["number2"]!.ToString()
                };
                return (await _kernel.RunAsync(multiplyVariables, multiply)).Result;
            default:
                return "I'm sorry, I don't understand.";
        }
    }
}
