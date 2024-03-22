---
title: Quickly create your first prompt in Semantic Kernel
description: Learn how to create and run prompts using Semantic Kernel.
author: johnmaeda
ms.topic: creating-chains
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: semantic-kernel
---

# Prompting AI models with Semantic Kernel

Prompts are core to getting the correct results from AI models. In this article, we'll demonstrate how to use common prompt engineering techniques while using Semantic Kernel.

If you want to see the final solution to this tutorial, you can check out the following samples in the public documentation repository.

| Language  | Link to final solution |
| --- | --- |
| C# | [Open example in GitHub](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/DocumentationExamples/Prompts.cs) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/03-Intro-to-Prompts) |
| Java | [Open example in GitHub](https://github.com/microsoft/semantic-kernel/blob/java-v1/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Prompts.java) |

## Creating a prompt that detects the intent of a user
If you've ever used ChatGPT or Microsoft Copilot, you're already familiar with prompting. Given a request, an LLM will attempt to predict the most likely response. For example, if you sent the prompt `"I want to go to the "`, an AI service might return back `"beach"` to complete the sentence. This is a very simple example, but it demonstrates the basic idea of how text generation prompts work.

With the Semantic Kernel SDK, you can easily run prompts from your own applications. This allows you to leverage the power of AI models in your own applications.

One common scenario is to detect the intent of a user so  you could run some automation afterwards, so in this article, we'll show how you can create a prompt that detects a user's intent. Additionally, we'll demonstrate how to progressively improve the prompt by using prompt engineering techniques.

> [!Tip]
> Many of the recommendations in this article are based on the [Prompt Engineering Guide](https://www.promptingguide.ai/introduction/basics). If you want to become an expert at writing prompts, we highly recommend reading it and leveraging their prompt engineering techniques.

## Running your first prompt with Semantic Kernel
If we wanted an AI to detect the intent of a user's input, we could simply _ask_ what the intent is. In Semantic Kernel, we could create a string that does just that with the following code:

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" id="InitialPrompt":::

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/prompts.py" range="18,21":::

# [Java](#tab/java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Prompts.java" id="InitialPrompt":::

---

To run this prompt, we now need to create a kernel with an AI service.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" id="KernelCreation":::

# [Python](#tab/python)

1. Import Semantic Kernel.

    :::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/prompts.py" range="7":::

2. Create the kernel.

    :::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/prompts.py" range="12":::

3. Add the service to the kernel.

# [Java](#tab/java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Prompts.java" id="KernelCreation":::

---

Finally, we can invoke our prompt using our new kernel.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" id="InvokeInitialPrompt":::

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/prompts.py" range="24-28":::

# [Java](#tab/java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Prompts.java" id="InvokeInitialPrompt":::

---

If we run this code with the input "I want to send an email to the marketing team celebrating their recent milestone.", we should get an output that looks like the following:

```
The intent of this request is to seek guidance or clarification on how to effectively compose an email to the marketing team in order to celebrate their recent milestone.
```

## Improving the prompt with prompt engineering
While this prompt "works", it's not very usable since you cannot use the result to predictably trigger automation. Every time you run the prompt, you may get a very different response.

To make the result more predictable, we can perform the following improvements:
1. [Make the prompt more specific](#1-make-the-prompt-more-specific)
2. [Add structure to the output with formatting](#2-add-structure-to-the-output-with-formatting)
3. [Provide examples with few-shot prompting](#3-provide-examples-with-few-shot-prompting)
4. [Tell the AI what to do to avoid doing something wrong](#4-tell-the-ai-what-to-do-to-avoid-doing-something-wrong)
5. [Provide context to the AI](#5-provide-context-to-the-ai)
6. [Using message roles in chat completion prompts](#6-using-message-roles-in-chat-completion-prompts)
7. [Give your AI words of encouragement](#7-give-your-ai-words-of-encouragement)

### 1) Make the prompt more specific
The first thing we can do is be more specific with our prompt. Instead of just asking "What is the intent of this request?", we can provide the AI with a list of intents to choose from. This will make the prompt more predictable since the AI will only be able to choose from the list of intents we provide.


# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" id="MoreSpecificPrompt":::

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/prompts.py" range="32-33" highlight="2":::

# [Java](#tab/java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Prompts.java" id="MoreSpecificPrompt":::

---

Now when you run the prompt with the same input, you should get a more usable result, but it's still not perfect since the AI responds with additional information.

```
The intent of the request is to send an email. Therefore, the appropriate action would be to use the SendEmail function.
```

### 2) Add structure to the output with formatting
While the result is more predictable, there's a chance that the LLM responds in such a way that you cannot easily parse the result. For example, if the LLM responded with "The intent is SendEmail", you may have a hard time extracting the intent since it's not in a predictable location.

To make the result more predictable, we can add structure to the prompt by using formatting. In this case, we can define the different parts of our prompt like so:

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" id="StructuredPrompt":::

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/prompts.py" range="44-47":::

# [Java](#tab/java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Prompts.java" id="StructuredPrompt":::

---

By using this formatting, the AI is less likely to respond with a result that is more than just the intent.

In other prompts, you may also want to experiment with using Markdown, XML, JSON, YAML or other formats to add structure to your prompts and their outputs. Since LLMs have a tendency to generate text that looks like the prompt, it's recommended that you use the same format for both the prompt and the output.

For example, if you wanted the LLM to generate a JSON object, you could use the following prompt:

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" id="FormattedPrompt":::

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/prompts.py" range="58-64":::

# [Java](#tab/java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Prompts.java" id="FormattedPrompt":::

---

This would result in the following output:

```json
{
    "intent": "SendEmail"
}
```

### 3) Provide examples with few-shot prompting
So far, we've been using zero-shot prompting, which means we're not providing any examples to the AI. While this is ok for getting started, it's not recommended for more complex scenarios since the AI may not have enough training data to generate the correct result.

To add examples, we can use few-shot prompting. With few-shot prompting, we provide the AI with a few examples of what we want it to do.  For example, we could provide the following examples to help the AI distinguish between sending an email and sending an instant message.


# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" id="FewShotPrompt":::

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/prompts.py" range="91-101" highlight="4-5,7-8":::

# [Java](#tab/java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Prompts.java" id="FewShotPrompt":::

---

### 4) Tell the AI what to do to avoid doing something wrong
Often when an AI starts responding incorrectly, it's tempting to simply tell the AI to stop doing something. Unfortunately, this can often lead to the AI doing something even worse. For example, if you told the AI to stop returning back a hallucinated intent, it may start returning back an intent that is completely unrelated to the user's request.

Instead, it's recommended that you tell the AI what it should do _instead_. For example, if you wanted to tell the AI to stop returning back a hallucinated intent, you might write the following prompt.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" id="AvoidPrompt":::

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/prompts.py" range="112-123" highlight="2":::

# [Java](#tab/java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Prompts.java" id="AvoidPrompt":::

---

### 5) Provide context to the AI
In some cases, you may want to provide the AI with context so it can better understand the user's request. This is particularly important for long running chat scenarios where the intent of the user may require context from previous messages.

Take for example, the following conversation:
    
```
User: I hate sending emails, no one ever reads them.
AI: I'm sorry to hear that. Messages may be a better way to communicate.
User: I agree, can you send the full status update to the marketing team that way?
```

If the AI was only given the last message, it may incorrectly respond with "SendEmail" instead of "SendMessage". However, if the AI was given the entire conversation, it may be able to understand the intent of the user.

To provide this context, we can simply add the previous messages to the prompt. For example, we could update our prompt to look like the following:

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" id="ContextPrompt":::

# [Python](#tab/python)

:::code language="python" source="~/../semantic-kernel-samples/python/samples/documentation_examples/prompts.py" range="134-150" highlight="1-4,15":::

# [Java](#tab/java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Prompts.java" id="ContextPrompt":::

---

### 6) Using message roles in chat completion prompts
As your prompts become more complex, you may want to use message roles to help the AI differentiate between system instructions, user input, and AI responses. This is particularly important as we start to add the chat history to the prompt. The AI should know that some of the previous messages were sent by itself and not the user.

In Semantic Kernel, a special syntax is used to define message roles. To define a message role, you simply wrap the message in `<message>` tag with the role name as an attribute. This is currently only available in the C# and Java SDKs.

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" id="RolePrompt":::

# [Java](#tab/java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Prompts.java" id="RolePrompt":::

---

### 7) Give your AI words of encouragement
Finally, research has shown that giving your AI words of encouragement can help it perform better. For example, offering bonuses or rewards for good results can yield better results. 

# [C#](#tab/Csharp)

:::code language="csharp" source="~/../semantic-kernel-samples/dotnet/samples/DocumentationExamples/Prompts.cs" id="BonusPrompt":::

# [Java](#tab/java)

:::code language="java" source="~/../semantic-kernel-samples-java/java/samples/sample-code/src/main/java/com/microsoft/semantickernel/samples/documentationexamples/Prompts.java" id="BonusPrompt":::

---


## Next steps
Now that you know how to write prompts, you can learn how to templatize them to make them more flexible and powerful.

> [!div class="nextstepaction"]
> [Learn how to templatize your prompts](./templatizing-prompts.md)

