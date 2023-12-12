// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel;
using System.Text.Json.Nodes;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
namespace Plugins.OrchestratorPlugin;

public class Orchestrator
{
    private readonly IKernel _kernel;

    public Orchestrator(IKernel kernel)
    {
        _kernel = kernel;
    }

    [SKFunction]
    public async Task<string> RouteRequestAsync(
        [Description("The user request")] string input
    )
    {
        // Save the original user request
        string request = input;

        // Retrieve the intent from the user request
        var getIntent = this._kernel.Functions.GetFunction("OrchestratorPlugin", "getIntent");
        var getIntentVariables = new ContextVariables
        {
            ["input"] = input,
            ["options"] = "Sqrt, Multiply"
        };
        string intent = (await this._kernel.RunAsync(getIntentVariables, getIntent)).GetValue<string>()!.Trim();

        // Retrieve the numbers from the user request
        var getNumbers = this._kernel.Functions.GetFunction("OrchestratorPlugin", "GetNumbers");
        string numbersJson = (await this._kernel.RunAsync(request, getNumbers)).GetValue<string>()!;
        JsonObject numbers = JsonObject.Parse(numbersJson)!.AsObject();

        // Call the appropriate function
        KernelResult result;
        switch (intent)
        {
            case "Sqrt":
                // Call the Sqrt function with the first number
                var sqrt = _kernel.Functions.GetFunction("MathPlugin", "Sqrt");
                result = await _kernel.RunAsync(numbers["number1"]!.ToString(), sqrt);
                return result.GetValue<string>()!.ToString();
            case "Multiply":
                // Call the Multiply function with both numbers
                var multiply = _kernel.Functions.GetFunction("MathPlugin", "Multiply");
                var multiplyVariables = new ContextVariables
                {
                    ["input"] = numbers["number1"]!.ToString(),
                    ["number2"] = numbers["number2"]!.ToString()
                };
                result = await _kernel.RunAsync(multiplyVariables, multiply);
                return result.GetValue<double>()!.ToString();
            default:
                return "I'm sorry, I don't understand.";
        }
    }
}
