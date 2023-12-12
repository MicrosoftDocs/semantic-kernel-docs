---
title: Call nested functions in prompts
description: Learn how to call functions within a Semantic Kernel prompt.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Calling functions within a prompt
In the previous article we demonstrated how to [templatize a prompt](./templatizing-promtps.md) to make it more reusable. In this article, we'll show you how to call other functions _within_ a prompt to help break up the prompt into smaller pieces. This helps
keep LLMs focused on a single task, helps avoid hitting token limits, and allows you to add native code directly into your prompt.

If you want to see the final solution, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/04-Templatizing-Prompts) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/05-Nested-Functions-In-Prompts) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/04-Templatizing-Prompts) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/05-Nested-Functions-In-Prompts) |


## Calling a nested function
In the [previous example](./templatizing-prompts.md), we created a prompt that gets the user's intent. This function used the previous conversation history to determine the intent of the user.

Putting the entire history into a single prompt, however, may result in using too many tokens. To avoid this, we can summarize the conversation history before asking for the intent. To do this, we can leverage the `ConversationSummaryPlugin` that's part of the [core plugins package](../agents/plugins/using-plugins/out-of-the-box-plugins.md).

Below, we show how we can update our original prompt to use the `SummarizeConversation` function in the `ConversationSummaryPlugin` to summarize the conversation history before asking for the intent.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/05-Nested-Functions-In-Prompts/program.cs" range="34-50" highlight="14":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/05-Nested-Functions-In-Prompts/main.py" range="38-54" highlight="14":::

---

## Testing the updated prompt
After adding the nested function, you must ensure that you load the plugin with the required function into the kernel.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/05-Nested-Functions-In-Prompts/program.cs" range="3-14, 17-20" highlight="13":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/05-Nested-Functions-In-Prompts/main.py" range="8-17" highlight="8":::

---

Afterwards, we can test the prompt by populating the prompt template with the following variables. In this example, the chat history is relatively tame, but you could imagine it being too large for the context window.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/05-Nested-Functions-In-Prompts/program.cs" range="21-33, 52-66":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/05-Nested-Functions-In-Prompts/main.py" range="19-37, 56-63":::

---

After running the code, you should see the following output after inputting `Yes`:

```output
SendEmail
```

If you inspect the rendered prompt for the `GetIntent` function, you can see that the entire history is not sent to the LLM. Instead, the `SummarizeConversationAsync` function is called to summarize the conversation history before asking for the intent.

```output
Instructions: What is the intent of this request?
Choices: SendEmail, SendMessage, CompleteTask, CreateDocument.

Prior conversation summary: The marketing team needs an update on the new product.
AI response: What do you want to tell them?
User Input: Can you send a very quick approval to the marketing team?
Intent: SendMessage

Prior conversation summary: The AI offered to send an email to the marketing team.
AI response: Do you want me to send an email to the marketing team?
User Input: Yes, please.
Intent: SendEmail

Prior conversation summary: The user asked the AI about the weather in Seattle and was informed that it was 70 degrees and sunny. The user then asked the AI to remind them about their calendar and was informed that they had a meeting with their team at 2:00 PM. The user then decided to send an email to their team to congratulate them on hitting a major milestone.
AI response: Would you like to write one for you?
User Input: Yes
Intent: 
```

## Take the next step
Now that you can call nested functions, you can now learn how to [serialize your templates](./serializing-semantic-functions.md).

> [!div class="nextstepaction"]
> [Serialize your templates](./serializing-semantic-functions.md)