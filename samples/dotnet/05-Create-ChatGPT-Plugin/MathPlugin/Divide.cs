using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace MathPlugin
{
    public class Divide
    {
        [OpenApiOperation(operationId: "Divide", tags: new[] { "ExecuteFunction" }, Description = "Divides two numbers.")]
        [OpenApiParameter(name: "dividend", Description = "The number to be divided", Required = true, In = ParameterLocation.Query)]
        [OpenApiParameter(name: "divisor", Description = "The number to divide by", Required = true, In = ParameterLocation.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "Returns the quotient of the division.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns the error of the input.")]
        [Function("Divide")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            bool result1 = double.TryParse(req.Query["number1"], out double dividend);
            bool result2 = double.TryParse(req.Query["number2"], out double divisor);

            if (result1 && result2 && divisor != 0)
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                double quotient = dividend / divisor;
                response.WriteString(quotient.ToString());

                return response;
            }
            else
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "application/json");
                response.WriteString("Please pass valid dividend and divisor (non-zero) numbers on the query string or in the request body");

                return response;
            }
        }
    }
}
