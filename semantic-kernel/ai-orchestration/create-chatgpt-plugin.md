---
title: How to create a plugin for ChatGPT with Semantic Kernel
description: Learn how to take a Semantic Kernel plugin and expose it to ChatGPT with Azure Functions.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/8/2023
ms.service: mssearch
---


# Expose your plugins to ChatGPT and Bing

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

So far, we've demonstrated how to create plugins that can be used natively in Semantic Kernel. This is great if you are building a custom application that only uses Semantic Kernel, but what if you want to use your plugins in applications that _don't_ use Semantic Kernel, like ChatGPT, or Bing?

In this article, we'll show you how to take a Semantic Kernel plugin and expose it to ChatGPT with Azure Functions. As an example, we'll demonstrate how to transform the `MathPlugin` we created in previous articles into a ChatGPT plugin.

Once we're done, you'll have an Azure Function that exposes each of your plugin's native functions as HTTP endpoints so they can be used by Semantic Kernel _or_ ChatGPT. If you want to see the final solution, you can check out the sample in the public documentation repository.


| Language  | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/05-Create-ChatGPT-Plugin) |
| Python | _Coming soon_ |

## Prerequisites
To complete this tutorial, you'll need the following:
- [Azure Functions Core Tools](https://github.com/Azure/azure-functions-core-tools)  version 4.x.
- [].NET 6.0 SDK.](https://dotnet.microsoft.com/download)
- One of the following tools for creating Azure resources:
  - [Azure CLI](/cli/azure/install-azure-cli) [version 2.4](/cli/azure/release-notes-azure-cli#april-21-2020) or later.
  - The [Azure Az PowerShell module](https://learn.microsoft.com/en-us/powershell/azure/install-azure-powershell) version 5.9.0 or later.

To publish your plugin, you'll also need an Azure account with an active subscription. [Create an account for free](https://azure.microsoft.com/free/?ref=microsoft.com&utm_source=microsoft.com&utm_medium=docs&utm_campaign=visualstudio).

You do _not_ need to have access to OpenAI's plugin preview to complete this tutorial. If you do have access, however, you can upload your final plugin to OpenAI and use it in ChatGPT.


## What are ChatGPT plugins?
In the [plugin article](./plugins#what-is-a-plugin) we described how all plugins are moving towards the common standard defined by OpenAI. This standard, which is called a ChatGPT plugin in this article, uses a plugin manifest file that points to an accompanying [OpenAPI specification](https://swagger.io/resources/open-api/). Plugins defined in this way can then be used by any application that supports the OpenAI specification, including Semantic Kernel and ChatGPT.

> [!Note]
> OpenAPI is different than OpenAI. OpenAPI is a specification for describing REST APIs, while OpenAI is a company that develops AI models and APIs. While the two are not related, OpenAI has adopted the OpenAPI specification for describing plugin APIs.

So far, however, we've only shown how to create plugins that are _natively_ loaded into Semantic Kernel instead of being exposed through an OpenAPI specification. This has helped us demonstrate the core concepts of plugins without adding the additional complexity of standing up an HTTP endpoint. With minimal changes, however, we can take the plugins we've already created and expose them to ChatGPT.

### Transforming our `MathPlugin` into a ChatGPT plugin

:::row:::
   :::column span="1":::
        There are three steps we must take to turn our existing `MathPlugin`` into a ChatGPT plugin:
        1. Create HTTP endpoints for each native function.
        2. Create an OpenAPI specification and plugin manifest file.
        3. Test the plugin by importing it into Semantic Kernel.
   :::column-end:::
   :::column span="2":::
      ![The Math plugin, before and after ](../media/plugin-before-and-after.png)
   :::column-end:::
:::row-end:::

## 1) Create HTTP endpoints for each native function
Before we can expose our plugin to other applications, we need to create an HTTP endpoint for each of our native functions. This will allow us to call our native functions from any other service. You can achieve this multiple ways, but in this article we'll use Azure Functions.

### Create a new Azure Function project
There are several ways to create an Azure Function, but in this article we'll use the Azure Functions Core Tools. If you are using Visual Studio or Visual Studio Code, you can also use the Azure Functions extension to create a new Azure Function project. For more information, see [Create your first function using Visual Studio](/azure/azure-functions/functions-create-your-first-function-visual-studio) or [Create your first function using Visual Studio Code](/azure/azure-functions/create-first-function-vs-code-csharp).

1. To create a new Azure Function project, run the following command in your terminal:

    ```bash
    func init MathPlugin --worker-runtime dotnet-isolated --target-framework net6.0
    ```

2. Navigate into the `MathPlugin` directory:

    ```bash
    cd MathPlugin
    ```
3. Update the `MathPlugin.csproj` file to include the following package references:

    ```xml
    <PackageReference Include="Microsoft.Azure.Functions.Worker" Version="1.17.0" />
    <PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.Http" Version="3.0.13" />
    <PackageReference Include="Microsoft.Azure.Functions.Worker.Sdk" Version="1.11.0" />
    ```
4. Run the following command to restore the packages:

    ```bash
    dotnet restore
    ```

### Add your native functions to the Azure Function project
We can now add our native functions to the Azure Function project.

1. Run the following command in your terminal to create placeholder for the `Add` function:

    ```bash
    func new --name Add --template "HTTP trigger" --authlevel "anonymous"
    ```
2. Open the _Add.cs_ file.
3. Replace the `Run` function with the following code:
    ```csharp
    [Function("Add")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        bool result1 = double.TryParse(req.Query["number1"], out double number1);
        bool result2 = double.TryParse(req.Query["number2"], out double number2);

        if (result1 && result2)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            double sum = number1 + number2;
            response.WriteString(sum.ToString());
            
            return response;
        }
        else
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString("Please pass two numbers on the query string or in the request body");

            return response;
        }
    }
    ```
4. Repeat the previous steps to create HTTP endpoints for the `Subtract`, `Multiply`, `Divide`, and `Sqrt` functions. When replacing the `Run` function, be sure to update the function name and logic for each function accordingly.

At this point, you should have five HTTP endpoints in your Azure Function project. You can test them by following these steps:

1. Run the following command in your terminal:
    ```bash
    func start
    ```
2. Open a new terminal window and run the following commands:
    ```bash
    curl "http://localhost:7071/api/Add?number1=1&number2=2"
    curl "http://localhost:7071/api/Subtract?number1=1&number2=2"
    curl "http://localhost:7071/api/Multiply?number1=1&number2=2"
    curl "http://localhost:7071/api/Divide?number1=1&number2=2"
    curl "http://localhost:7071/api/Sqrt?number=9"
    ```

You should see the following responses:
```bash
3
-1
2
0.5
3
```

### Add an OpenAPI document to your Azure Function project
Now that we have HTTP endpoints for each of our native functions, we need to create an OpenAPI specification that describes them. Thankfully, Azure Functions provides a NuGet package that makes this easy. To add an OpenAPI document to your Azure Function project, follow these steps:

1. Run the following commands in your terminal:
    ```bash
    dotnet add package Microsoft.Azure.WebJobs.Extensions.OpenApi --version 1.5.1
    ```
    ```bash
    dotnet add package Microsoft.Azure.Functions.Worker.Extensions.OpenApi --version 1.5.1
    ```
2. Open the _Add.cs_ file.
3. Add the following `using` statements:
    ```csharp
    using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
    using Microsoft.OpenApi.Models;
    ```
4. Add the following attributes to the `Run` function:
    ```csharp
    [OpenApiOperation(operationId: "Add", tags: new[] { "ExecuteFunction" }, Description = "Adds two numbers.")]
    [OpenApiParameter(name: "number1", Description = "The first number to add'", Required = true, In = ParameterLocation.Query)]
    [OpenApiParameter(name: "number2", Description = "The second number to add", Required = true, In = ParameterLocation.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "Returns the sum of the two numbers.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns the error of the input.")]  
    ```
5. Repeat the previous steps to add OpenAPI attributes to the `Subtract`, `Multiply`, `Divide`, and `Sqrt` functions. When adding the attributes, be sure to update the operation and parameter descriptions accordingly. The `Description` fields are the most important attributes because they will be used by the planner to determine which function to call. We recommend reusing the same description values from the previous walkthroughs.

    | Function | Description |
    | --- | --- |
    | Add | Add two numbers. |
    | Subtract | Subtract two numbers. |
    | Multiply | Multiply two numbers. When increasing by a percentage, don't forget to add 1 to the percentage. |
    | Divide | Divide two numbers. |
    | Sqrt | Take the square root of a number. |

You can then test the OpenAPI document by following these steps:

1. Run the following command in your terminal:
    ```bash
    func start
    ```
2. Navigate to the following URL in your browser:
    ```bash
    http://localhost:7071/api/swagger/ui
    ```

You should see the following page:
:::image type="content" source="../media/swagger-ui.png" alt-text="Swagger UI":::

Navigating to _http://localhost:7071/api/swagger.json_ will allow you to download the OpenAPI specification.

## 2) Create an OpenAPI specification and plugin manifest file

### Create the manifest file

### Expose the manifest from the Azure Function

## 3) Test the plugin by importing it into Semantic Kernel

dotnet add package Microsoft.SemanticKernel.Skills.OpenAPI --version 0.17.230704.3-preview

