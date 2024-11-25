---
title: YAML Schema Reference for Prompts
description: Detailed YAML schema reference for Semantic Kernel prompts
author: markwallace
ms.topic: conceptual
ms.author: markwallace
ms.date: 09/27/2024
ms.service: semantic-kernel
---

# YAML schema reference for Semantic Kernel prompts

The YAML schema reference for Semantic Kernel prompts is a detailed reference for YAML prompts that lists all supported YAML syntax and their available options.

## Definitions

### name

The function name to use by default when creating prompt functions using this configuration.
If the name is null or empty, a random name will be generated dynamically when creating a function.

### description

The function description to use by default when creating prompt functions using this configuration.

### template_format

The identifier of the Semantic Kernel template format. Semantic Kernel provides support for the following template formats:

1. [semantic-kernel](./prompt-template-syntax.md) - Built-in Semantic Kernel format.
2. [handlebars](./handlebars-prompt-templates.md) - Handlebars template format.
3. [liquid](./liquid-prompt-templates.md) - Liquid template format

### template

The prompt template string that defines the prompt.

### input_variables

The collection of input variables used by the prompt template.
Each input variable has the following properties:

1. `name` - The name of the variable.
2. `description` - The description of the variable.
3. `default` - An optional default value for the variable.
4. `is_required` - Whether the variable is considered required (rather than optional).
5. `json_schema` - The JSON Schema describing this variable.
6. `allow_dangerously_set_content` - A boolean value indicating whether to handle the variable value as potential dangerous content.

> [!TIP]
> The default for `allow_dangerously_set_content` is false.
> When set to true the value of the input variable is treated as safe content.
> For prompts which are being used with a chat completion service this should be set to false to protect against prompt injection attacks.
> When using other AI services e.g. Text-To-Image this can be set to true to allow for more complex prompts.

### output_variable

The output variable used by the prompt template.
The output variable has the following properties:

1. `description` - The description of the variable.
2. `json_schema` - The JSON Schema describing this variable.

### execution_settings

The collection of execution settings used by the prompt template.
The settings dictionary is keyed by the service ID, or `default` for the default execution settings.
When setting, the service id of each [PromptExecutionSettings](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/SemanticKernel.Abstractions/AI/PromptExecutionSettings.cs) must match the key in the dictionary.

Each entry has the following properties:

1. `service_id` - This identifies the service these settings are configured for e.g., azure_openai_eastus, openai, ollama, huggingface, etc.
2. `model_id` - This identifies the AI model these settings are configured for e.g., gpt-4, gpt-3.5-turbo.
3. `function_choice_behavior` - The behavior defining the way functions are chosen by LLM and how they are invoked by AI connectors.

> [!TIP]
> If provided, the service identifier will be the key in a dictionary collection of execution settings.
> If not provided the service identifier will be set to `default`.

#### Function Choice Behavior

To disable function calling, and have the model only generate a user-facing message, set the property to null (the default).

- `auto` - To allow the model to decide whether to call the functions and, if so, which ones to call.
- `required` - To force the model to always call one or more functions.
- `none` - To instruct the model to not call any functions and only generate a user-facing message.

### allow_dangerously_set_content

A boolean value indicating whether to allow potentially dangerous content to be inserted into the prompt from functions.
**The default is false.**
When set to true the return values from functions only are treated as safe content.
For prompts which are being used with a chat completion service this should be set to false to protect against prompt injection attacks.
When using other AI services e.g. Text-To-Image this can be set to true to allow for more complex prompts.

## Next steps

> [!div class="nextstepaction"]
> [Handlebars Prompt Templates](./handlebars-prompt-templates.md)
> [Liquid Prompt Templates](./liquid-prompt-templates.md)
