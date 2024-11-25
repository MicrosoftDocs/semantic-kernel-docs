---
title: Give agents access to OpenAPI APIs
description: Learn how to add plugins from OpenAPI specifications to your agents in Semantic Kernel.
zone_pivot_groups: programming-languages
author: sophialagerkranspandey
ms.topic: conceptual
ms.author: sopand
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Add plugins from OpenAPI specifications

Often in an enterprise, you already have a set of APIs that perform real work.These could be used by other automation services or 
power front-end applications that humans interact with. In Semantic Kernel, you can add these exact same APIs as plugins so your agents can also use them.

## An example OpenAPI specification

Take for example an API that allows you to alter the state of light bulbs. The OpenAPI specification, known as Swagger Specification, or just Swagger, for this API might look like this:

```json
{
   "openapi": "3.0.1",
   "info": {
      "title": "Light API",
      "version": "v1"
   },
   "paths": {
      "/Light": {
         "get": {
            "summary": "Retrieves all lights in the system.",
            "operationId": "get_all_lights",
            "responses": {
               "200": {
                  "description": "Returns a list of lights with their current state",
                  "application/json": {
                     "schema": {
                        "type": "array",
                        "items": {
                              "$ref": "#/components/schemas/LightStateModel"
                        }
                     }
                  }
               }
            }
         }
      },
      "/Light/{id}": {
         "post": {
               "summary": "Changes the state of a light.",
               "operationId": "change_light_state",
               "parameters": [
                  {
                     "name": "id",
                     "in": "path",
                     "description": "The ID of the light to change.",
                     "required": true,
                     "style": "simple",
                     "schema": {
                           "type": "string"
                     }
                  }
               ],
               "requestBody": {
                  "description": "The new state of the light and change parameters.",
                  "content": {
                     "application/json": {
                           "schema": {
                              "$ref": "#/components/schemas/ChangeStateRequest"
                           }
                     }
                  }
               },
               "responses": {
                  "200": {
                     "description": "Returns the updated light state",
                     "content": {
                           "application/json": {
                              "schema": {
                                 "$ref": "#/components/schemas/LightStateModel"
                              }
                           }
                     }
                  },
                  "404": {
                     "description": "If the light is not found"
                  }
               }
         }
      }
   },
   "components": {
      "schemas": {
         "ChangeStateRequest": {
               "type": "object",
               "properties": {
                  "isOn": {
                     "type": "boolean",
                     "description": "Specifies whether the light is turned on or off.",
                     "nullable": true
                  },
                  "hexColor": {
                     "type": "string",
                     "description": "The hex color code for the light.",
                     "nullable": true
                  },
                  "brightness": {
                     "type": "integer",
                     "description": "The brightness level of the light.",
                     "format": "int32",
                     "nullable": true
                  },
                  "fadeDurationInMilliseconds": {
                     "type": "integer",
                     "description": "Duration for the light to fade to the new state, in milliseconds.",
                     "format": "int32",
                     "nullable": true
                  },
                  "scheduledTime": {
                     "type": "string",
                     "description": "Use ScheduledTime to synchronize lights. It's recommended that you asynchronously create tasks for each light that's scheduled to avoid blocking the main thread.",
                     "format": "date-time",
                     "nullable": true
                  }
               },
               "additionalProperties": false,
               "description": "Represents a request to change the state of the light."
         },
         "LightStateModel": {
               "type": "object",
               "properties": {
                  "id": {
                     "type": "string",
                     "nullable": true
                  },
                  "name": {
                     "type": "string",
                     "nullable": true
                  },
                  "on": {
                     "type": "boolean",
                     "nullable": true
                  },
                  "brightness": {
                     "type": "integer",
                     "format": "int32",
                     "nullable": true
                  },
                  "hexColor": {
                     "type": "string",
                     "nullable": true
                  }
               },
               "additionalProperties": false
         }
      }
   }
}
```

This specification provides everything needed by the AI to understand the API and how to interact with it. The API includes two endpoints: one to get all lights and another to change the state of a light. It also provides the following:
- Semantic descriptions for the endpoints and their parameters
- The types of the parameters
- The expected responses

Since the AI agent can understand this specification, you can add it as a plugin to the agent.

Semantic Kernel supports OpenAPI versions 2.0 and 3.0, and it aims to accommodate version 3.1 specifications by downgrading it to version 3.0.

> [!TIP]
> If you have existing OpenAPI specifications, you may need to make alterations to make them easier for an AI to understand them. For example, you may need to provide guidance in the descriptions. For more tips on how to make your OpenAPI specifications AI-friendly, see [Tips and tricks for adding OpenAPI plugins](#tips-and-tricks-for-adding-openapi-plugins).

## Adding the OpenAPI plugin

With a few lines of code, you can add the OpenAPI plugin to your agent. The following code snippet shows how to add the light plugin from the OpenAPI specification above:

::: zone pivot="programming-language-csharp"
```csharp
await kernel.ImportPluginFromOpenApiAsync(
   pluginName: "lights",
   uri: new Uri("https://example.com/v1/swagger.json"),
   executionParameters: new OpenApiFunctionExecutionParameters()
   {
      // Determines whether payload parameter names are augmented with namespaces.
      // Namespaces prevent naming conflicts by adding the parent parameter name
      // as a prefix, separated by dots
      EnablePayloadNamespacing = true
   }
);
```
With Semantic Kernel, you can add OpenAPI plugins from various sources, such as a URL, file path, or stream. 
Additionally, plugins can be created once and reused across multiple kernel instances or agents.
```csharp
// Create the OpenAPI plugin from a local file somewhere at the root of the application
KernelPlugin plugin = await OpenApiKernelPluginFactory.CreateFromOpenApiAsync(
    pluginName: "lights",
    filePath: "lights.json",
    executionParameters: new OpenApiFunctionExecutionParameters()
    {
        EnablePayloadNamespacing = true
    }
);

// Add the plugin to the kernel
Kernel kernel = new Kernel();
kernel.Plugins.Add(plugin);
```
::: zone-end

::: zone pivot="programming-language-python"
```python
await kernel.add_plugin_from_openapi(
   plugin_name="lights",
   openapi_document_path="https://example.com/v1/swagger.json",
   execution_settings=OpenAPIFunctionExecutionParameters(
         # Determines whether payload parameter names are augmented with namespaces.
         # Namespaces prevent naming conflicts by adding the parent parameter name
         # as a prefix, separated by dots
         enable_payload_namespacing=True,
   ),
)
```
::: zone-end

::: zone pivot="programming-language-java"
```java
String yaml = EmbeddedResourceLoader.readFile("petstore.yaml", ExamplePetstoreImporter.class);

KernelPlugin plugin = SemanticKernelOpenAPIImporter
   .builder()
   .withPluginName("petstore")
   .withSchema(yaml)
   .withServer("http://localhost:8090/api/v3")
   .build();

Kernel kernel = ExampleOpenAPIParent.kernelBuilder()
   .withPlugin(plugin)
   .build();
```
::: zone-end
Afterwards, you can use the plugin in your agent as if it were a native plugin.

::: zone pivot="programming-language-csharp"
## Handling OpenAPI plugin parameters

Semantic Kernel automatically extracts metadata - such as name, description, type, and schema for all parameters defined in OpenAPI documents. 
This metadata is stored in the `KernelFunction.Metadata.Parameters` property for each OpenAPI operation 
and is provided to the LLM along with the prompt to generate the correct arguments for function calls.

By default, the original parameter name is provided to the LLM and is used by Semantic Kernel to look up the corresponding 
argument in the list of arguments supplied by the LLM. However, there may be cases where the OpenAPI plugin has multiple parameters with the same name. 
Providing this parameter metadata to the LLM could create confusion, potentially preventing the LLM from generating the correct arguments for function calls.

Additionally, since a kernel function that does not allow for non-unique parameter names is created for each OpenAPI operation, 
adding such a plugin could result in some operations becoming unavailable for use. Specifically, operations with non-unique parameter names will be skipped, 
and a corresponding warning will be logged. Even if it were possible to include multiple parameters with the same name in the kernel function, 
this could lead to ambiguity in the argument selection process.

Considering all of this, Semantic Kernel offers a solution for managing plugins with non-unique parameter names. This solution is particularly useful when 
changing the API itself is not feasible, whether due to it being a third-party service or a legacy system.

The following code snippet demonstrates how to handle non-unique parameter names in an OpenAPI plugin. If the change_light_state operation had an additional 
parameter with the same name as the existing "id" parameter - specifically, to represent a session ID in addition to the current "id" that represents the ID of 
the light - it could be handled as shown below:
```csharp
OpenApiDocumentParser parser = new();

using FileStream stream = File.OpenRead("lights.json");

// Parse the OpenAPI document
RestApiSpecification specification = await parser.ParseAsync(stream);

// Get the change_light_state operation
RestApiOperation operation = specification.Operations.Single(o => o.Id == "change_light_state");

// Set the 'lightId' argument name to the 'id' path parameter that represents the ID of the light
RestApiParameter idPathParameter = operation.Parameters.Single(p => p.Location == RestApiParameterLocation.Path && p.Name == "id");
idPathParameter.ArgumentName = "lightId";

// Set the 'sessionId' argument name to the 'id' header parameter that represents the session ID
RestApiParameter idHeaderParameter = operation.Parameters.Single(p => p.Location == RestApiParameterLocation.Header && p.Name == "id");
idHeaderParameter.ArgumentName = "sessionId";

// Import the transformed OpenAPI plugin specification
kernel.ImportPluginFromOpenApi(pluginName: "lights", specification: specification);
```
This code snippet utilizes the `OpenApiDocumentParser` class to parse the OpenAPI document and access the `RestApiSpecification` model 
object that represents the document. It assigns argument names to the parameters and imports the transformed OpenAPI plugin 
specification into the kernel. Semantic Kernel provides the argument names to the LLM instead of the original names and uses them 
to look up the corresponding arguments in the list supplied by the LLM.

It is important to note that the argument names are not used in place of the original names when calling the OpenAPI operation. 
In the example above, the 'id' parameter in the path will be replaced by a value returned by the LLM for the 'lightId' argument. 
The same applies to the 'id' header parameter; the value returned by the LLM for the 'sessionId' argument will be used as the value for the header named 'id'.

## Handling OpenAPI plugins payload

OpenAPI plugins can modify the state of the system using POST, PUT, or PATCH operations. These operations often require a payload to be included with the request.

Semantic Kernel offers a few options for managing payload handling for OpenAPI plugins, depending on your specific scenario and API requirements.
   
### Dynamic payload construction  
   
Dynamic payload construction allows the payloads of OpenAPI operations to be created dynamically based on the payload schema and arguments provided by the LLM.
This feature is enabled by default but can be disabled by setting the `EnableDynamicPayload` property to `false` in the `OpenApiFunctionExecutionParameters` object when adding an OpenAPI plugin.
   
For example, consider the change_light_state operation, which requires a payload structured as follows:
```json
{
   "isOn": true,
   "hexColor": "#FF0000",
   "brightness": 100,
   "fadeDurationInMilliseconds": 500,
   "scheduledTime": "2023-07-12T12:00:00Z"
}
```

To change the state of the light and get values for the payload properties, Semantic Kernel provides the LLM with metadata for the operation so it can reason about it:
```json
{
    "name":"lights-change-light-state",
    "description": "Changes the state of a light.",
    "parameters":[
        { "name": "id", "schema": {"type":"string", "description": "The ID of the light to change.", "format":"uuid"}},
        { "name": "isOn", "schema": { "type": "boolean", "description": "Specifies whether the light is turned on or off."}},
        { "name": "hexColor", "schema": { "type": "string", "description": "Specifies whether the light is turned on or off."}},
        { "name": "brightness", "schema": { "type":"string", "description":"The brightness level of the light.", "enum":["Low","Medium","High"]}},
        { "name": "fadeDurationInMilliseconds", "schema": { "type":"integer", "description":"Duration for the light to fade to the new state, in milliseconds.", "format":"int32"}},
        { "name": "scheduledTime", "schema": {"type":"string", "description":"The time at which the change should occur.", "format":"date-time"}},
    ]
}
```

In addition to providing operation metadata to the LLM, Semantic Kernel will perform the following steps:
1. Handle the LLM call to the OpenAPI operation, constructing the payload based on the schema and provided by LLM property values.
2. Send the HTTP request with the payload to the API.
   
Dynamic payload construction is best suited for APIs with relatively simple payload structures that have unique property names.
If the payload has non-unique property names, consider the following alternatives:
1. Provide a unique argument name for each non-unique property, using a method similar to that described in the [Handling OpenAPI plugin parameters](./adding-openapi-plugins.md#handling-openapi-plugin-parameters) section.
2. Use namespaces to avoid naming conflicts, as outlined in the next section on [Payload namespacing](./adding-openapi-plugins.md#payload-namespacing).
3. Disable dynamic payload construction and allow the LLM to create the payload based on its schema, as explained in the [The payload parameter](./adding-openapi-plugins.md#the-payload-parameter) section.
   
### Payload namespacing

Payload namespacing helps prevent naming conflicts that can occur due to non-unique property names in OpenAPI plugin payloads.

When namespacing is enabled, Semantic Kernel provides the LLM with OpenAPI operation metadata that includes augmented property names.
These augmented names are created by adding the parent property name as a prefix, separated by a dot, to the child property names.

For example, if the change_light_state operation had included a nested `offTimer` object with a `scheduledTime` property:

```json
{
  "isOn": true,
  "hexColor": "#FF0000",
  "brightness": 100,
  "fadeDurationInMilliseconds": 500,
  "scheduledTime": "2023-07-12T12:00:00Z",
  "offTimer": {
      "scheduledTime": "2023-07-12T12:00:00Z"
  }
}
```

Semantic Kernel would have provided the LLM with metadata for the operation that includes the following property names:
```json
{
    "name":"lights-change-light-state",
    "description": "Changes the state of a light.",
    "parameters":[
        { "name": "id", "schema": {"type":"string", "description": "The ID of the light to change.", "format":"uuid"}},
        { "name": "isOn", "schema": { "type": "boolean", "description": "Specifies whether the light is turned on or off."}},
        { "name": "hexColor", "schema": { "type": "string", "description": "Specifies whether the light is turned on or off."}},
        { "name": "brightness", "schema": { "type":"string", "description":"The brightness level of the light.", "enum":["Low","Medium","High"]}},
        { "name": "fadeDurationInMilliseconds", "schema": { "type":"integer", "description":"Duration for the light to fade to the new state, in milliseconds.", "format":"int32"}},
        { "name": "scheduledTime", "schema": {"type":"string", "description":"The time at which the change should occur.", "format":"date-time"}},
        { "name": "offTimer.scheduledTime", "schema": {"type":"string", "description":"The time at which the device will be turned off.", "format":"date-time"}},
    ]
}
```

In addition to providing operation metadata with augmented property names to the LLM, Semantic Kernel performs the following steps:
1. Handle the LLM call to the OpenAPI operation and look up the corresponding arguments among those provided by the LLM for all the properties in the payload, using the augmented property names and falling back to the original property names if necessary.
2. Construct the payload using the original property names as keys and the resolved arguments as values.
3. Send the HTTP request with the constructed payload to the API.
   
By default, the payload namespacing option is disabled. It can be enabled by setting the `EnablePayloadNamespacing` property
to `true` in the `OpenApiFunctionExecutionParameters` object when adding an OpenAPI plugin:

```csharp
await kernel.ImportPluginFromOpenApiAsync(
    pluginName: "lights",
    uri: new Uri("https://example.com/v1/swagger.json"),
    executionParameters: new OpenApiFunctionExecutionParameters()
    {
        EnableDynamicPayload = true, // Enable dynamic payload construction. This is enabled by default.
        EnablePayloadNamespacing = true // Enable payload namespacing
    });
```

If enabled, the namespacing option only takes effect when dynamic payload construction is also enabled; otherwise, it has no effect.

### The payload parameter

Semantic Kernel can work with payloads created by the LLM using the payload parameter. This is useful when the payload schema is complex 
and contains non-unique property names, which makes it infeasible for Semantic Kernel to dynamically construct the payload.
In such cases, you will be relying on the LLM's ability to understand the schema and construct a valid payload. Recent models, such as `gpt-4o`
are effective at generating valid JSON payloads.

To enable the payload parameter, set the `EnableDynamicPayload` property to `false` in the `OpenApiFunctionExecutionParameters` object when adding an OpenAPI plugin:
```csharp
await kernel.ImportPluginFromOpenApiAsync(
    pluginName: "lights",
    uri: new Uri("https://example.com/v1/swagger.json"),
    executionParameters: new OpenApiFunctionExecutionParameters()
    {
        EnableDynamicPayload = false, // Disable dynamic payload construction
    });
```

When the payload parameter is enabled, Semantic Kernel provides the LLM with metadata for the operation that includes schemas for the payload and
content_type parameters, allowing the LLM to understand the payload structure and construct it accordingly:
```json
{
    "name": "payload",
    "schema":
    {
        "type": "object",
        "properties": {
            "isOn": {
                "type": "boolean",
                "description": "Specifies whether the light is turned on or off."
            },
            "hexColor": {
                "type": "string",
                "description": "The hex color code for the light.",
            },
            "brightness": {
                "enum": ["Low", "Medium", "High"],
                "type": "string",
                "description": "The brightness level of the light."
            },
            "fadeDurationInMilliseconds": {
                "type": "integer",
                "description": "Duration for the light to fade to the new state, in milliseconds.",
                "format": "int32"
            },
            "scheduledTime": {
                "type": "string",
                "description": "The time at which the change should occur.",
                "format": "date-time"
            }
        },
        "additionalProperties": false,
        "description": "Represents a request to change the state of the light."
    },
    {
        "name": "content_type",
        "schema":
        {
            "type": "string",
            "description": "Content type of REST API request body."
        }
    }
}
```

In addition to providing the operation metadata with the schema for payload and content type parameters to the LLM, Semantic Kernel performs the following steps:
1. Handle the LLM call to the OpenAPI operation and uses arguments provided by the LLM for the payload and content_type parameters.
2. Send the HTTP request to the API with provided payload and content type.

## Server base url

Semantic Kernel OpenAPI plugins require a base URL, which is used to prepend endpoint paths when making API requests. 
This base URL can be specified in the OpenAPI document, obtained implicitly by loading the document from a URL, or 
provided when adding the plugin to the kernel.

### Url specified in OpenAPI document
   
OpenAPI v2 documents define the server URL using the `schemes`, `host`, and `basePath` fields:

```json
{
   "swagger": "2.0",
   "host": "example.com",
   "basePath": "/v1",
   "schemes": ["https"]
   ...
}
```

Semantic Kernel will construct the server URL as `https://example.com/v1`.

In contrast, OpenAPI v3 documents define the server URL using the `servers` field:

```json
{
   "openapi": "3.0.1",
   "servers": [
      {
         "url": "https://example.com/v1"
      }
   ],
   ...
}
```

Semantic Kernel will use the first server URL specified in the document as the base URL: `https://example.com/v1`.

OpenAPI v3 also allows for parameterized server URLs using variables indicated by curly braces:

```json
{
   "openapi": "3.0.1",
   "servers": [
      {
         "url": "https://{environment}.example.com/v1",
         "variables": {
            "environment": {
               "default": "prod"
            }
         }
      }
   ],
   ...  
}
```

In this case, Semantic Kernel will replace the variable placeholder with either the value provided as an argument for the
variable or the default value if no argument is provided, resulting in the URL: `https://prod.example.com/v1`.

If the OpenAPI document specifies no server URL, Semantic Kernel will use the base URL of the server from which the OpenAPI document was loaded:

```csharp
await kernel.ImportPluginFromOpenApiAsync(pluginName: "lights", uri: new Uri("https://api-host.com/swagger.json"));
```
The base URL will be `https://api-host.com`.

### Overriding the Server URL  
   
In some instances, the server URL specified in the OpenAPI document or the server from which the document was loaded may not be suitable for use cases involving the OpenAPI plugin.   
  
The Semantic Kernel allows you to override the server URL by providing a custom base URL when adding the OpenAPI plugin to the kernel:
```csharp  
await kernel.ImportPluginFromOpenApiAsync(  
    pluginName: "lights",  
    uri: new Uri("https://example.com/v1/swagger.json"),  
    executionParameters: new OpenApiFunctionExecutionParameters()  
    {  
        ServerUrlOverride = new Uri("https://custom-server.com/v1")  
    });  
```  
   
In this example, the base URL will be `https://custom-server.com/v1`, overriding the server URL specified in the OpenAPI document and the server URL from which the document was loaded.  

## Authentication

Most REST APIs require authentication to access their resources. Semantic Kernel provides a mechanism that enables you to integrate 
a variety of authentication methods required by OpenAPI plugins.

This mechanism relies on an authentication callback function, which is invoked before each API request. This callback function has 
access to the HttpRequestMessage object, representing the HTTP request that will be sent to the API. You can use this object to add 
authentication credentials to the request. The credentials can be added as headers, query parameters, or in the request body, depending 
on the authentication method used by the API.

You need to register this callback function when adding the OpenAPI plugin to the kernel. The following code snippet demonstrates 
how to register it to authenticate requests:

```csharp
static Task AuthenticateRequestAsyncCallback(HttpRequestMessage request, CancellationToken cancellationToken = default)
{
    // Best Practices:  
    // * Store sensitive information securely, using environment variables or secure configuration management systems.  
    // * Avoid hardcoding sensitive information directly in your source code.  
    // * Regularly rotate tokens and API keys, and revoke any that are no longer in use.  
    // * Use HTTPS to encrypt the transmission of any sensitive information to prevent interception.  
  
    // Example of Bearer Token Authentication  
    // string token = "your_access_token";  
    // request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);  
  
    // Example of API Key Authentication  
    // string apiKey = "your_api_key";  
    // request.Headers.Add("X-API-Key", apiKey);    
  
    return Task.CompletedTask;  
}

await kernel.ImportPluginFromOpenApiAsync(  
    pluginName: "lights",  
    uri: new Uri("https://example.com/v1/swagger.json"),  
    executionParameters: new OpenApiFunctionExecutionParameters()  
    {  
        AuthCallback = AuthenticateRequestAsyncCallback
    });  
```

::: zone-end


## Tips and tricks for adding OpenAPI plugins

Since OpenAPI specifications are typically designed for humans, you may need to make some alterations to make them easier for an AI to understand. Here are some tips and tricks to help you do that:

| Recommendation | Description |
|----------------|-------------|
| **Version control your API specifications** | Instead of pointing to a live API specification, consider checking-in and versioning your Swagger file. This will allow your AI researchers to test (and alter) the API specification used by the AI agent without affecting the live API and vice versa. |
| **Limit the number of endpoints** | Try to limit the number of endpoints in your API. Consolidate similar functionalities into single endpoints with optional parameters to reduce complexity. |
| **Use descriptive names for endpoints and parameters** | Ensure that the names of your endpoints and parameters are descriptive and self-explanatory. This helps the AI understand their purpose without needing extensive explanations. |
| **Use consistent naming conventions** | Maintain consistent naming conventions throughout your API. This reduces confusion and helps the AI learn and predict the structure of your API more easily. |
| **Simplify your API specifications** | Often, OpenAPI specifications are very detailed and include a lot of information that isn't necessary for the AI agent to help a user. The simpler the API, the fewer tokens you need to spend to describe it, and the fewer tokens the AI needs to send requests to it. |
| **Avoid string parameters** | When possible, avoid using string parameters in your API. Instead, use more specific types like integers, booleans, or enums. This will help the AI understand the API better. |
| **Provide examples in descriptions** | When humans use Swagger files, they typically are able to test the API using the Swagger UI, which includes sample requests and responses. Since the AI agent can't do this, consider providing examples in the descriptions of the parameters. |
| **Reference other endpoints in descriptions** | Often, AIs will confuse similar endpoints. To help the AI differentiate between endpoints, consider referencing other endpoints in the descriptions. For example, you could say "This endpoint is similar to the `get_all_lights` endpoint, but it only returns a single light." |
| **Provide helpful error messages** | While not within the OpenAPI specification, consider providing error messages that help the AI self-correct. For example, if a user provides an invalid ID, consider providing an error message that suggests the AI agent get the correct ID from the `get_all_lights` endpoint. |

## Next steps
Now that you know how to create a plugin, you can now learn how to use them with your AI agent. Depending on the type of functions you've added to your plugins, there are different patterns you should follow. For retrieval functions, refer to the [using retrieval functions](./using-data-retrieval-functions-for-rag.md) article. For task automation functions, refer to the [using task automation functions](./using-task-automation-functions.md) article.

> [!div class="nextstepaction"]
> [Learn about using retrieval functions](./using-data-retrieval-functions-for-rag.md)