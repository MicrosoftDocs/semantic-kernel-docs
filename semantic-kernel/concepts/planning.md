---
title: What are Planners in Semantic Kernel
description: Learn what a planner is in Semantic Kernel.
author: sophialagerkranspandey
ms.topic: conceptual
ms.author: sopand
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# What is a Planner?

Once you have multiple plugins, you then need a way for your AI agent to use them together to solve a user’s need. This is where planning comes in.

Early on, Semantic Kernel introduced the concept of planners that used prompts to request the AI to choose which functions to invoke. Since Semantic Kernel was introduced, however, OpenAI introduced a native way for the model to invoke or “call” a function: [function calling](../concepts/ai-services/chat-completion/function-calling.md). Other AI models like Gemini, Claude, and Mistral have since adopted function calling as a core capability, making it a cross-model supported feature.

Because of these advancements, Semantic Kernel has evolved to use function calling as the primary way to plan and execute tasks.

> [!IMPORTANT]
> Function calling is only available in OpenAI models that are 0613 or newer. If you use an older model (e.g., 0314), this functionality will return an error. We recommend using the latest OpenAI models to take advantage of this feature.


## How does function calling create a "plan"?

At its simplest, function calling is merely a way for an AI to invoke a function with the right parameters. Take for example a user wants to turn on a light bulb. Assuming the AI has the right plugin, it can call the function to turn on the light.

| Role | Message |
| --- | --- |
| 🔵&nbsp;**User** | Please turn on light #1 |
| 🔴&nbsp;**Assistant&nbsp;(function&nbsp;call)** | `Lights.change_state(1,  { "isOn": true })` |
| 🟢&nbsp;**Tool** | `{ "id": 1, "name": "Table Lamp", "isOn": true, "brightness": 100, "hex": "FF0000" }` |
| 🔴&nbsp;**Assistant** | The lamp is now on |

But what if the user doesn't know the ID of the light? Or what if the user wants to turn on all the lights? This is where planning comes in. Today's LLM models are capable of iteratively calling functions to solve a user's need. This is accomplished by creating a feedback loop where the AI can call a function, check the result, and then decide what to do next.

For example, a user may ask the AI to "toggle" a light bulb. The AI would first need to check the state of the light bulb before deciding whether to turn it on or off.

| Role | Message |
| --- | --- |
| 🔵&nbsp;**User** | Please toggle all the lights |
| 🔴&nbsp;**Assistant&nbsp;(function&nbsp;call)** | `Lights.get_lights()` |
| 🟢&nbsp;**Tool** | `{ "lights": [ { "id": 1, "name": "Table Lamp", "isOn": true, "brightness": 100, "hex": "FF0000" }, { "id": 2, "name": "Ceiling Light", "isOn": false, "brightness": 0, "hex": "FFFFFF" } ] }` |
| 🔴&nbsp;**Assistant&nbsp;(function&nbsp;call)** | `Lights.change_state(1, { "isOn": false})` `Lights.change_state(2,  { "isOn": true} |
| 🟢&nbsp;**Tool** | `{ "id": 1, "name": "Table Lamp", "isOn": false, "brightness": 0, "hex": "FFFFFF" }` |
| 🟢&nbsp;**Tool** | `{ "id": 2, "name": "Ceiling Light", "isOn": true, "brightness": 100, "hex": "FF0000" }` |
| 🔴&nbsp;**Assistant** | The lights have been toggled |

> [!NOTE]
> In this example, you also saw parallel function calling. This is where the AI can call multiple functions at the same time. This is a powerful feature that can help the AI solve complex tasks more quickly. It was added to the OpenAI models in 1106.

## The automatic planning loop

Supporting function calling without Semantic Kernel is relatively complex. You would need to write a loop that would accomplish the following:

1. Create JSON schemas for each of your functions
2. Provide the LLM with the previous chat history and function schemas
3. Parse the LLM's response to determine if it wants to reply with a message or call a function
4. If the LLM wants to call a function, you would need to parse the function name and parameters from the LLM's response
5. Invoke the function with the right parameters
6. Return the results of the function so that the LLM can determine what it should do next
7. Repeat steps 2-6 until the LLM decides it has completed the task or needs help from the user

In Semantic Kernel, we make it easy to use function calling by automating this loop for you. This allows you to focus on building the plugins needed to solve your user's needs.

## Using automatic function calling in Semantic Kernel

To use automatic function calling in Semantic Kernel, you need to do the following:
1. Register the plugin with the kernel
2. Create an execution settings object that tells the AI to automatically call functions
3. Invoke the chat completion service with the chat history and the kernel

```csharp
using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// 1. Create the kernel with the Lights plugin
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
builder.Plugins.AddFromType<LightsPlugin>();
Kernel kernel = builder.Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// 2. Enable automatic function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

var history = new ChatHistory();

string? userInput;
do {
    // Collect user input
    Console.Write("User > ");
    userInput = Console.ReadLine();

    // Add user input
    history.AddUserMessage(userInput);

    // 3. Get the response from the AI with automatic function calling
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Print the results
    Console.WriteLine("Assistant > " + result);

    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not null)
```

When you use automatic function calling, all of the steps in the automatic planning loop are handled for you and added to the `ChatHistory` object. After the function calling loop is complete, you can inspect the `ChatHistory` object to see all of the function calls made and results provided by Semantic Kernel.

## What about the Stepwise and Handlebars planners?

The Stepwise and Handlebars planners are still available in Semantic Kernel. However, we recommend using function calling for most tasks as it is more powerful and easier to use. Both the Stepwise and Handlebars planners will be deprecated in a future release of Semantic Kernel.

Before we deprecate these planners, we will provide guidance on how to migrate your existing planners to function calling. If you have any questions about this process, please reach out to us on the [discussions board](https://github.com/microsoft/semantic-kernel/discussions) in the Semantic Kernel GitHub repository.

## Next steps
Now that you understand how planners work in Semantic Kernel, you can learn more about how influence your AI agent so that they best plan and execute tasks on behalf of your users.

> [!div class="nextstepaction"]
> [Learn about personas](./personas.md)