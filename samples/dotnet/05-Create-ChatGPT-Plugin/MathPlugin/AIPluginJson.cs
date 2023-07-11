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
    ""name_for_human"": ""Simple calculator"",
    ""name_for_model"": ""calculator"",
    ""description_for_human"": ""This plugin performs basic math operations."",
    ""description_for_model"": ""Help the user perform math. You can add, subtract, multiple, divide, and perform square roots."",
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
