---
title: Call nested functions in prompts
description: Learn how to call functions within a Semantic Kernel prompt.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Calling functions within a semantic function
In the previous article we demonstrated how to [templatize a semantic function](./templatizing-semantic-functions.md) to make it more reusable. In this article, we'll show you how to call other functions _within_ a semantic function to help break up the prompt into smaller pieces. This helps
keep LLMs focused on a single task, helps avoid hitting token limits, and allows you to add native code directly into your prompt.

If you want to see the final solution, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

| Language  | Link to previous solution | Link to final solution |
| --- | --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/05-Templatizing-Semantic-Functions) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/06-Nested-Functions-In-Semantic-Functions) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/05-Templatizing-Semantic-Functions) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/06-Nested-Functions-In-Semantic-Functions) |


## Calling a nested function
In the [previous example](./templatizing-semantic-functions.md), we created a semantic function called `GetIntent`. This function uses the previous conversation history to determine the intent of the user.

Putting the entire history into a single prompt, however, may result in using too many tokens. To avoid this, we can summarize the conversation history before asking for the intent. To do this, we can leverage the `ConversationSummarySkill` plugin that's part of the [core plugins package](../out-of-the-box-plugins.md).

Below, we show how we can update our original prompt in the _skprompt.txt_ file to use the `SummarizeConversationAsync` function in the `ConversationSummarySkill` plugin to summarize the conversation history before asking for the intent.

:::code language="txt" source="~/../samples/dotnet/06-Calling-Nested-Functions-In-Semantic-Functions/plugins/OrchestratorPlugin/GetIntent/skprompt.txt" highlight="1":::

Since we're not changing the behavior of the `GetIntent` function or the required inputs, we don't need to update the _config.json_ file. For completeness, however, we've included the _config.json_ file below.

:::code language="json" source="~/../samples/dotnet/06-Calling-Nested-Functions-In-Semantic-Functions/plugins/OrchestratorPlugin/GetIntent/config.json":::


## Testing the updated semantic function
After adding the nested function, you must ensure that you load the plugin with the required function into the kernel.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../samples/dotnet/06-Calling-Nested-Functions-In-Semantic-Functions/program.cs" range="4-7,16-21,24-53" highlight="17":::

# [Python](#tab/python)

:::code language="python" source="~/../samples/python/06-Calling-Nested-Functions-In-Semantic-Functions/main.py" range="1-2,4-11,13-55" highlight="18-20":::

---

After running the code, you should see the following output:

```output
SendEmail
```

If you inspect the rendered prompt for the `GetIntent` function, you can see that the entire history is not sent to the LLM. Instead, the `SummarizeConversationAsync` function is called to summarize the conversation history before asking for the intent.

```output
The user asked about the weather in Seattle, and the bot informed them that
it was 70 degrees and sunny. The user also asked about their calendar, and
the bot reminded them of a 2:00 PM team meeting. The user mentioned their
team's recent milestone and considered sending a congratulatory email.
User: Yes

---------------------------------------------

Provide the intent of the user. The intent should be one of the following:
SendEmail, ReadEmail, SendMeeting, RsvpToMeeting, SendChat

INTENT: 
```

## Take the next step
Now that you can create a semantic function, you can now learn how to [create a native function](../native-functions/using-the-SKFunction-decorator.md).

> [!div class="nextstepaction"]
> [Create a native function](../native-functions/using-the-SKFunction-decorator.md)