// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Newtonsoft.Json.Linq;

namespace Plugins.OrchestratorPlugin;

public class Orchestrator
{
    private readonly IKernel _kernel;

    public Orchestrator(IKernel kernel)
    {
        _kernel = kernel;
    }

    [SKFunction, Description("Routes the request to the appropriate function.")]
    public async Task<string> RouteRequestAsync(
        [Description("The user request")] string input
    )
    {
        // Save the original user request
        string request = input;

        // Retrieve the intent from the user request
        var getIntent = _kernel.Functions.GetFunction("OrchestratorPlugin", "getIntent");
        var getIntentVariables = new ContextVariables
        {
            ["input"] = input,
            ["options"] = "Sqrt, Multiply"
        };
        string intent = (await _kernel.RunAsync(getIntentVariables, getIntent)).GetValue<string>()!.Trim();

        // Call the appropriate function
        ISKFunction MathFunction;
        switch (intent)
        {
            case "Sqrt":
                MathFunction = this._kernel.Functions.GetFunction("MathPlugin", "Sqrt");
                break;
            case "Multiply":
                MathFunction = this._kernel.Functions.GetFunction("MathPlugin", "Multiply");
                break;
            default:
                return "I'm sorry, I don't understand.";
        }

        // Get remaining functions
        var createResponse = this._kernel.Functions.GetFunction("OrchestratorPlugin", "CreateResponse");
        var getNumbers = this._kernel.Functions.GetFunction("OrchestratorPlugin", "GetNumbers");
        var extractNumbersFromJson = this._kernel.Functions.GetFunction("OrchestratorPlugin", "ExtractNumbersFromJson");

        // Run the pipeline
        var output = await this._kernel.RunAsync(
            request,
            getNumbers,
            extractNumbersFromJson,
            MathFunction,
            createResponse
        );

        // Create a new context with the original request
        var pipelineContext = new ContextVariables(request);
        pipelineContext["original_request"] = request;

        // Run the pipeline with create response
        output = await this._kernel.RunAsync(
            pipelineContext,
            getNumbers,
            extractNumbersFromJson,
            MathFunction,
            createResponse
        );

        return output.GetValue<string>()!;
    }

    [SKFunction, Description("Extracts numbers from JSON")]
    public static SKContext ExtractNumbersFromJson(SKContext context)
    {
        JObject numbers = JObject.Parse(context.Variables["input"]);

        // Loop through numbers and add them to the context
        foreach (var number in numbers)
        {
            if (number.Key == "number1")
            {
                context.Variables["input"] = number.Value!.ToString();
                continue;
            }

            context.Variables[number.Key] = number.Value!.ToString();
        }
        return context;
    }
}
