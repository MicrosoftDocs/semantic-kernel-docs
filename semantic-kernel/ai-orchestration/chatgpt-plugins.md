---
title: Create and run a ChatGPT plugin with Semantic Kernel
description: Learn how to take a Semantic Kernel plugin and expose it to ChatGPT with Azure Functions.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/8/2023
ms.service: mssearch
---

# Create and run ChatGPT plugins using Semantic Kernel

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

In this article, we'll show you how to take a Semantic Kernel plugin and expose it to ChatGPT with Azure Functions. As an example, we'll demonstrate how to transform the `MathPlugin` we created in previous articles into a ChatGPT plugin.

At the [end of this article](./chatgpt-plugins.md#running-the-plugin-with-semantic-kernel), you'll also learn how to load a ChatGPT plugin into Semantic Kernel and use it with a planner.

Once we're done, you'll have an Azure Function that exposes each of your plugin's native functions as HTTP endpoints so they can be used by Semantic Kernel _or_ ChatGPT. If you want to see the final solution, you can check out the sample in the public documentation repository.


| Language  | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/05-Create-ChatGPT-Plugin) |
| Python | _Coming soon_ |

## Prerequisites
To complete this tutorial, you'll need the following:
- [Azure Functions Core Tools](https://github.com/Azure/azure-functions-core-tools) version 4.x.
- [.NET 6.0 SDK.](https://dotnet.microsoft.com/download)

To publish your plugin once you're complete, you'll also need an Azure account with an active subscription. [Create an account for free](https://azure.microsoft.com/free/?ref=microsoft.com&utm_source=microsoft.com&utm_medium=docs&utm_campaign=visualstudio) and one of the following tools for creating Azure resources:
- [Azure CLI](/cli/azure/install-azure-cli) [version 2.4](/cli/azure/release-notes-azure-cli#april-21-2020) or later.
- The [Azure Az PowerShell module](/powershell/azure/install-azure-powershell) version 5.9.0 or later.

You do **_not_** need to have access to OpenAI's plugin preview to complete this tutorial. If you do have access, however, you can upload your final plugin to OpenAI and use it in ChatGPT at the very end.


## What are ChatGPT plugins?
In the [plugin article](./plugins.md#what-is-a-plugin) we described how all plugins are moving towards the common standard defined by OpenAI. This standard, which is called a ChatGPT plugin in this article, uses a plugin manifest file that points to an accompanying [OpenAPI specification](https://swagger.io/resources/open-api/). Plugins defined in this way can then be used by any application that supports the OpenAI specification, including Semantic Kernel and ChatGPT.

> [!Important]
> OpenAPI is different than OpenAI. OpenAPI is a specification for describing REST APIs, while OpenAI is a company that develops AI models and APIs. While the two are not related, OpenAI has adopted the OpenAPI specification for describing plugin APIs.

### Transforming our `MathPlugin` into a ChatGPT plugin
So far, however, we've only shown how to create plugins that are _natively_ loaded into Semantic Kernel instead of being exposed through an OpenAPI specification. This has helped us demonstrate the core concepts of plugins without adding the additional complexity of standing up an HTTP endpoint. With minimal changes, however, we can take the plugins we've already created and expose them to ChatGPT.

![The Math plugin, before and after ](../media/plugin-before-and-after.png)

There are three steps we must take to turn our existing `MathPlugin` into a ChatGPT plugin:
1. Create HTTP endpoints for each native function.
2. Create an OpenAPI specification and plugin manifest file that describes our plugin.
3. Test the plugin in either Semantic Kernel or ChatGPT.

## Download the ChatGPT plugin starter
To make it easier to create ChatGPT plugins, we've created a [starter project]() that you can use as a template. The starter project includes the following features:
- An endpoint that serves up an ai-plugin.json file for ChatGPT to discover the plugin
- A generator that automatically converts prompts into semantic function endpoints
- The ability to add additional native functions as endpoints to the plugin

To easiest way to get started is to use the Semantic Kernel VS Code extension. Follow the steps to download the starter with VS Code:
1. If you don't have VS Code installed, you can download it [here](https://code.visualstudio.com/).
2. Afterwards, navigate to the __Extensions__ tab and search for "Semantic Kernel".
3. Click __Install__ to install the extension.
4. Once the extension is installed, you'll see a welcome message. Select __Create a new app__.
    > [!Note]
    > If you've already installed the extension, you can also create a new app by pressing __Ctrl+Shift+P__ and typing "Semantic Kernel: Create Project".
5. Finally, select __C# ChatGPT Plugin__ to create a new ChatGPT plugin project.

If you don't want to use the VS Code extension, you can also download the [starter project from GitHub]().

## Provide HTTP endpoints for each function
Before we can expose our plugin to other applications, we need to create an HTTP endpoint for each of our native functions. This will allow us to call our native functions from any other service. You can achieve this multiple ways, but in this article we'll use Azure Functions.

### Add the native functions to the Azure Function project
Now that you have your starter, it's time to add your native functions to the plugin. To do this, we'll use Azure Functions to create HTTP endpoints for each function.

1. Navigate into the _MathPlugin/azure-function_ directory:

    ```bash
    cd MathPlugin/azure-function
    ```
2. Run the following command in your terminal to create placeholder for the `Add` function:

    ```bash
    func new --name Add --template "HTTP trigger" --authlevel "anonymous"
    ```
3. Open the _Add.cs_ file.
4. Replace the `Run` function with the following code:
    ```csharp
    [Function("Add")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        bool result1 = double.TryParse(req.Query["number1"], out double number1);
        bool result2 = double.TryParse(req.Query["number2"], out double number2);

        if (result1 && result2)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain");
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
5. Repeat the previous steps to create HTTP endpoints for the `Subtract`, `Multiply`, `Divide`, and `Sqrt` functions. When replacing the `Run` function, be sure to update the function name and logic for each function accordingly.

### Validate the HTTP endpoints
At this point, you should have five HTTP endpoints in your Azure Function project. You can test them by following these steps:

1. Run the following command in your terminal:
    ```bash
    func start
    ```
2. Open a new terminal window and run the following commands:
    ```bash
    curl "http://localhost:7071/Add?number1=1&number2=2"
    curl "http://localhost:7071/Subtract?number1=1&number2=2"
    curl "http://localhost:7071/Multiply?number1=1&number2=2"
    curl "http://localhost:7071/Divide?number1=1&number2=2"
    curl "http://localhost:7071/Sqrt?number=9"
    ```

3. You should see the following responses:
    ```output
    3
    -1
    2
    0.5
    3
    ```

## Add a semantic function to the plugin

### Setup the kernel

### Create a semantic function

### Test the semantic function

## Create the manifest files
Now that we have HTTP endpoints for each of our native functions, we need to create the files that will tell ChatGPT and other applications how to call them. We'll do this by creating an OpenAPI specification and plugin manifest file.

### Add an OpenAPI spec to your Azure Function project
An OpenAPI specification describes the HTTP endpoints that are available in your plugin. Instead of manually creating an OpenAPI specification, you can use NuGet packages provided by Azure Functions to automatically create and serve up these files. To add an OpenAPI specification to your Azure Function project, follow these steps:

1. Run the following commands in your Azure Function project directory:
    ```bash
    dotnet add package Microsoft.Azure.WebJobs.Extensions.OpenApi --version 1.5.1
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
    [OpenApiParameter(name: "number1", Description = "The first number to add", Required = true, In = ParameterLocation.Query)]
    [OpenApiParameter(name: "number2", Description = "The second number to add", Required = true, In = ParameterLocation.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the sum of the two numbers.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns the error of the input.")]  
    ```
5. Repeat the previous steps for the `Subtract`, `Multiply`, `Divide`, and `Sqrt` functions. When adding the attributes, update the operation and parameter descriptions accordingly.

    > [!Important]
    > The `Description` fields for both the operation and the parameters are the most important attributes because they will be used by the planner to determine which function to call. We recommend reusing the same description values from the previous walkthroughs.

    | Function | Description | Number 1 | Number 2 |
    | --- | --- |
    | Add | Add two numbers. | The first number to add | The second number to add | 
    | Subtract | Subtract two numbers. | The first number to subtract from | The second number to subtract away |
    | Multiply | Multiply two numbers. When increasing by a percentage, don't forget to add 1 to the percentage. |
    | Divide | Divide two numbers. | The first number to divide from | The second number to divide by |
    | Sqrt | Take the square root of a number. | The number to calculate the square root of | N/A |

### Validate the OpenAPI spec
You can then test the OpenAPI document by following these steps:

1. Run the following command in your terminal:
    ```bash
    func start
    ```
2. Navigate to the following URL in your browser:
    ```bash
    http://localhost:7071/swagger/ui
    ```

3. You should see the following page:
    :::image type="content" source="../media/swagger-ui.png" alt-text="Swagger UI":::

4. Navigating to _http://localhost:7071/swagger.json_ will allow you to download the OpenAPI specification.

### Add the plugin manifest file
The last step is to serve up the plugin manifest file. Based on the OpenAI specification, the manifest file is always served up from the _/.well-known/ai-plugin.json_ file and contains the following information:

| Field | Type | Description |
| --- | --- | --- |
| schema_version | String | Manifest schema version |
| name_for_model | String | Name the model will use to target the plugin (no spaces allowed, only letters and numbers). 50 character max. |
| name_for_human | String | Human-readable name, such as the full company name. 20 character max. |
| description_for_model | String | Description better tailored to the model, such as token context length considerations or keyword usage for improved plugin prompting. 8,000 character max. |
| description_for_human | String | Human-readable description of the plugin. 100 character max. |
| auth | ManifestAuth | Authentication schema |
| api | Object | API specification |
| logo_url | String | URL used to fetch the logo. Suggested size: 512 x 512. Transparent backgrounds are supported. Must be an image, no GIFs are allowed. |
| contact_email | String | Email contact for safety/moderation |
| legal_info_url | String | Redirect URL for users to view plugin information | 

The starter already has an endpoint for this manifest file. To customize the output, follow these steps:

1. Run the following command in your terminal:
    ```bash
    func new --name AIPluginJson --template "HTTP trigger" --authlevel "anonymous"
    ```
2. Open the _appsettings.cs_ file.
3. Update the values in the `aiPlugin` object
    ```json
    "aiPlugin": {
        "schemaVersion": "v1",
        "nameForModel": "MathPlugin",
        "nameForHuman": "Math Plugin",
        "descriptionForModel": "Used to perform math operations (i.e., add, subtract, multiple, divide).",
        "descriptionForHuman": "Used to perform math operations.",
        "auth": {
            "type": "none"
        },
        "api": {
            "type": "openapi",
            "url": "{url}/swagger.json"
        },
        "logoUrl": "{url}/logo.png",
        "contactEmail": "support@example.com",
        "legalInfoUrl": "http://www.example.com/legal"
    }
    ```

### Validate the plugin manifest file
You can then test that the plugin manifest file is being served up by following these steps:

1. Run the following command in your terminal:
    ```bash
    func start
    ```
2. Navigate to the following URL in your browser:
    ```bash
    http://localhost:7071/.well-known/ai-plugin.json
    ```

3. You should now see the plugin manifest file.
    :::image type="content" source="../media/ai-plugin-json.png" alt-text="ai-plugin.json file":::

## Testing the plugin end-to-end
You now have a complete plugin that can be used in Semantic Kernel and ChatGPT. Since there is currently a waitlist for creating plugins for ChatGPT, we'll first demonstrate how you can test your plugin with Semantic Kernel.

### Running the plugin with Semantic Kernel
By testing your plugin in Semantic Kernel, you can ensure that it is working as expected before you get access to the plugin developer portal for ChatGPT. While testing in Semantic Kernel, we recommend using the Stepwise Planner to invoke your plugin since it is the only planner that supports JSON responses.

To test the plugin in Semantic Kernel, follow these steps:

1. Create a new C# project.
2. Add the necessary Semantic Kernel NuGet packages:
    ```bash
    dotnet add package Microsoft.SemanticKernel
    dotnet add package Microsoft.SemanticKernel.Planning.StepwisePlanner
    dotnet add package Microsoft.SemanticKernel.Skills.OpenAPI
    ```
3. Paste the following code into your _program.cs_ file:
    ```csharp
    using Microsoft.Extensions.Logging;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Planning;

    // ... create a new Semantic Kernel instance here

    // Add the math plugin using the plugin manifest URL
    const string pluginManifestUrl = "http://localhost:7071/.well-known/ai-plugin.json";
    var mathPlugin = await kernel.ImportChatGptPluginSkillFromUrlAsync("MathPlugin", new Uri(pluginManifestUrl));

    // Create a stepwise planner and invoke it
    var planner = new StepwisePlanner(kernel);
    var question = "I have $2130.23. How much would I have after it grew by 24% and after I spent $5 on a latte?";
    var plan = planner.CreatePlan(question);
    var result = await plan.InvokeAsync(kernel.CreateNewContext());

    // Print the results
    Console.WriteLine("Result: " + result);

    // Print details about the plan
    if (result.Variables.TryGetValue("stepCount", out string? stepCount))
    {
        Console.WriteLine("Steps Taken: " + stepCount);
    }
    if (result.Variables.TryGetValue("skillCount", out string? skillCount))
    {
        Console.WriteLine("Skills Used: " + skillCount);
    }
    ```

4. After running the code, you should see the following output:

    ```output
    Result: After the amount grew by 24% and $5 was spent on a latte, you would have $2636.4852 remaining.
    Steps Taken: 3
    Skills Used: 2 (MathPlugin.Multiply(1), MathPlugin.Subtract(1))
    ```

### Running the plugin in ChatGPT
If you would like to test your plugin in ChatGPT, you can do so by following these steps:
1. Request access to plugin development by filling out the [waitlist form](https://openai.com/waitlist/plugins).
2. Once you have access, follow the steps [provided by OpenAI](https://platform.openai.com/docs/plugins/getting-started/running-a-plugin) to register your plugin.

## Deploy the plugin to Azure
Once you have fully tested your plugin, you can deploy it to Azure Functions. To do so, follow these steps:

1. Create a new Azure Functions resource in the Azure portal.
2. Add your API key to the Azure Functions settings.
3. Navigate to the Azure extension in Visual Studio Code.
4. Select your Azure Function project.
5. Select the deploy command.


## Next steps
Congratulations! You have successfully created a plugin that can be used in Semantic Kernel and ChatGPT. Once you have fully tested your plugin, you can deploy it to Azure Functions and register it with OpenAI. For more information, see the following resources:
- [Deploying Azure Functions](/azure/azure-functions/functions-deployment-technologies)
- [Submit a plugin to the OpenAI plugin store](https://platform.openai.com/docs/plugins/review)