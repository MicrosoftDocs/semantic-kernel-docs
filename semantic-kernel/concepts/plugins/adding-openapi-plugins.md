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

Often in an enterprise, you already have a set of APIs that perform real work. These could be used by other automation services or power front-end applications that humans interact with. In Semantic Kernel, you can add these exact same APIs as plugins so your agents can also use them.

## An example OpenAPI specification

Take for example an API that allows you to alter the state of light bulbs. The OpenAPI specification for this API might look like this:

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
            "tags": [
               "Light"
            ],
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
               "tags": [
                  "Light"
               ],
               "summary": "Changes the state of a light.",
               "operationId": "change_light_state",
               "parameters": [
                  {
                     "name": "id",
                     "in": "path",
                     "description": "The ID of the light to change from the get_all_lights tool.",
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

Afterwards, you can use the plugin in your agent as if it were a native plugin.

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