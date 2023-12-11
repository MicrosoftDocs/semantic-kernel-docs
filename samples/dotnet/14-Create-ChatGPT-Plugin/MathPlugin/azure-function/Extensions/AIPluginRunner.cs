// Copyright (c) Microsoft. All rights reserved.

using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Models;

namespace AIPlugins.AzureFunctions.Extensions;

public class AIPluginRunner : IAIPluginRunner
{
    private readonly ILogger<AIPluginRunner> _logger;
    private readonly Kernel _kernel;

    public AIPluginRunner(Kernel kernel, ILoggerFactory loggerFactory)
    {
        this._kernel = kernel;
        this._logger = loggerFactory.CreateLogger<AIPluginRunner>();
    }


    /// <summary>
    /// Runs a semantic function using the operationID and returns back an HTTP response.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="operationId"></param>
    public async Task<HttpResponseData> RunAIPluginOperationAsync(HttpRequestData req, string operationId)
    {
        KernelArguments contextVariables = LoadContextVariablesFromRequest(req);

        var appSettings = AppSettings.LoadSettings();
        var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(),"Prompts");
        var func = this._kernel.ImportPromptsFromDirectory(appSettings.AIPlugin.NameForModel, pluginsDirectory);

        var result = await this._kernel.InvokeAsync(func[operationId], contextVariables);
        if (result == null)
        {
            HttpResponseData errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            string? message = result?.ToString();
            if (message != null)
            {
                await errorResponse.WriteStringAsync(message);  
            }
            return errorResponse;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain;charset=utf-8");
        await response.WriteStringAsync(result.GetValue<string>());
        return response;
    }

    /// <summary>
    /// Grabs the context variables to send to the semantic function from the original HTTP request.
    /// </summary>
    /// <param name="req"></param>
    protected static KernelArguments LoadContextVariablesFromRequest(HttpRequestData req)
    {
        KernelArguments contextVariables = new KernelArguments();
        foreach (string? key in req.Query.AllKeys)
        {
            if (!string.IsNullOrEmpty(key))
            {
                contextVariables.Add(key, req.Query[key]);
            }
        }

        // If "input" was not specified in the query string, then check the body
        if (string.IsNullOrEmpty(req.Query.Get("input")))
        {
            // Load the input from the body
            string? body = req.ReadAsString();
            if (!string.IsNullOrEmpty(body))
            {
                contextVariables.Add("input", body);
            }
        }

        return contextVariables;
    }
}