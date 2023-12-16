---
title: Call nested functions within prompts
description: Learn how to call functions within a Semantic Kernel prompt.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Calling functions within a prompt
In the previous article we demonstrated how to [templatize a prompt](./templatizing-prompts.md) to make it more reusable. In this article, we'll show you how to call other functions _within_ a prompt to help break up the prompt into smaller pieces. This helps
keep LLMs focused on a single task, helps avoid hitting token limits, and allows you to add native code directly into your prompt.

If you want to see the final solution, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/04-Templatizing-Prompts) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/05-Nested-Functions-In-Prompts) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/04-Templatizing-Prompts) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/05-Nested-Functions-In-Prompts) |


## Calling a nested function
In the [previous example](./templatizing-prompts.md), we created a prompt that chats with the user. This function used the previous conversation history to determine what the agent should say next.

Putting the entire history into a single prompt, however, may result in using too many tokens. To avoid this, we can summarize the conversation history before asking for the intent. To do this, we can leverage the `ConversationSummaryPlugin` that's part of the [core plugins package](../agents/plugins/out-of-the-box-plugins.md).

Below, we show how we can update our original prompt to use the `SummarizeConversation` function in the `ConversationSummaryPlugin` to summarize the conversation history before asking for the intent.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/05-Nested-Functions-In-Prompts/program.cs" range="66-70" highlight="2":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/05-Nested-Functions-In-Prompts/main.py" range="22-24" highlight="1":::

---

## Testing the updated prompt
After adding the nested function, you must ensure that you load the plugin with the required function into the kernel.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/05-Nested-Functions-In-Prompts/program.cs" range="5-7, 9-16, 19-21":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/05-Nested-Functions-In-Prompts/main.py" range="2-3,5-16":::

---

Afterwards, we can test the prompt by creating a chat loop that makes the history progressively longer.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/05-Nested-Functions-In-Prompts/program.cs" range="23-24, 71-78, 95-118":::


# [Python](#tab/python)

:::code language="python" source="~/../samples/python/05-Nested-Functions-In-Prompts/main.py" range="26-44":::

---


## Calling nested functions in Handlebars
In the previous article, we showed how to use the Handlebars template engine to create the `getIntent` prompt. In this article, we'll show you how to update this prompt with the same nested function.

Similar to the previous example, we can use the `SummarizeConversation` function to summarize the conversation history before asking for the intent. The only difference is that we'll need to use the Handlebars syntax to call the function which requires us to use an `-` between the plugin name and function name instead of a `.`.

:::code language="csharp" source="~/../samples/dotnet/05-Nested-Functions-In-Prompts/program.cs" range="42-63" highlight="15":::

## Take the next step
Now that you can call nested functions, you can now learn how to [configure your prompts](./configure-prompts.md).

> [!div class="nextstepaction"]
> [Configure your prompts](./configure-prompts.md)
