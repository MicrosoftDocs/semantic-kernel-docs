using System;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace MathPlugin
{
    public class Sqrt
    {
        [OpenApiOperation(operationId: "Sqrt", tags: new[] { "ExecuteFunction" }, Description = "Take the square root of a number")]
        [OpenApiParameter(name: "number", Description = "The number to calculate the square root of", Required = true, In = ParameterLocation.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "Returns the square root of the number.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns an error message.")]
        [Function("Sqrt")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            bool result = double.TryParse(req.Query["number"], out double number);

            if (result && number >= 0)
            {
                double sqrt = Math.Sqrt(number);
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                response.WriteString(sqrt.ToString());

                return response;
            }
            else
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "application/json");
                response.WriteString("Please pass a non-negative number on the query string or in the request body");

                return response;
            }
        }
    }
}
