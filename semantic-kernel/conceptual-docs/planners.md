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
Early on, Semantic Kernel introduced the concept of planners that used prompts to request the AI to choose which functions to invoke. Since Semantic Kernel was introduced, however, OpenAI introduced a native way for the model to invoke or “call” a function: function calling. Other AI models like Gemini, Claude, and Mistral have other adopted function calling as a core capability, making it a cross-model supported feature.
In Semantic Kernel, we make it easy to use function calling by automating the loop necessary for your AI application to 1) choose a function, 2) invoke the function with the right parameters, and 3) return the results of the function so that the AI can determine what it should do next.
Automatic function calling planning in action
> Note: Function calling is only available in OpenAI models that are 0613 or newer. If you use an older model (e.g., 0314), this functionality will return an error.
Take, for example, a user that wants an agent to “toggle” a light bulb. To support this scenario, the developer would first need to provide the agent with a lightbulb plugin that can retrieve the current state of the light bulb and change the state of the light bulb.

// Insert code for light plugin here

Afterwards, the developer can import the light plugin into their kernel.

// Show code adding the light plugin to the kernel

The magic then comes when the developer makes a chat completion request. All the developer needs to do is set configuration to use automatic function calling and then the AI will automatically call all the necessary functions it needs in order until the user’s request is complete.

// Show code for making a chat completion request

As part of this process, the underlying chat history will be manipulated by the AI to include the function calls from the AI and the function results from the kernel so that the AI has a full record of everything that has happened. For the request to “toggle the lightbulb” for example, the AI would first need to check the lightbulb (to see if it’s on or off) before finally changing the state to the opposite value.
If the lightbulb was originally off, the chat history would look something like this after the automatic function calling loop has completed.

// Show a JSON representation of the automatic function call loop

Parallel function calling
> Note: parallel function calling is only supported on OpenAI models that are 1106 or newer.
One of the downsides of automatic function calling is that it requires a call to the AI every time a new function needs to be picked and invoked. This can add latency and waste tokens. With the introduction of parallel function calling, OpenAI provided a way for a model to choose multiple function at the same time. Semantic Kernel will then invoke all of them at the same time before return the results back to the model.
For example, if we added multiple light plugins, we could ask the AI to turn them all on or off and it would perform the operations using a single LLM call.

// Show adding multiple light plugins

The final chat history object would look something like the following

// Show JSON representation of the chat history object

Controlling automatic function calling with hooks and filters
Using the Python plugin for more advanced planning
