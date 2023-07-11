using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace MathPlugin
{
    public class AiPluginJson
    {
        [Function("GetAiPluginJson")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = ".well-known/ai-plugin.json")] HttpRequestData req)
        {
            var currentDomain = $"{req.Url.Scheme}://{req.Url.Host}:{req.Url.Port}";

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            var json = $@"{{
    ""schema_version"": ""v1"",
    ""name_for_human"": ""TODO List"",
    ""name_for_model"": ""todo"",
    ""description_for_human"": ""Manage your TODO list. You can add, remove and view your TODOs."",
    ""description_for_model"": ""Help the user with managing a TODO list. You can add, remove and view your TODOs."",
    ""auth"": {{
        ""type"": ""none""
    }},
    ""api"": {{
        ""type"": ""openapi"",
        ""url"": ""{currentDomain}/api/swagger.json""
    }},
    ""logo_url"": ""{currentDomain}/logo.png"",
    ""contact_email"": ""support@example.com"",
    ""legal_info_url"": ""http://www.example.com/legal""
}}";

            response.WriteString(json);

            return response;
        }
    }
}
