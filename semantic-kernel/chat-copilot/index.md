---
title: See Semantic Kernel working end-to-end with Chat Copilot
description: Use the Chat Copilot reference app to learn how to build a custom conversational agent and to test your plugins.
author: smonroe
ms.topic: samples
ms.author: smonroe
ms.date: 04/07/2023
ms.service: semantic-kernel
---
# See Semantic Kernel working end-to-end with Chat Copilot

[!INCLUDE [subheader.md](../includes/pat_large.md)]

Chat Copilot provides a reference application for building a chatbot experience using Semantic Kernel. The Semantic Kernel team built this application so that you could see how the different concepts of the platform come together to create a single conversational experience. These include leveraging [plugins](../ai-orchestration/plugins.md), [planners](../ai-orchestration/plugins.md), and AI [memories](../memories/index.md).

## Exploring the app
With Chat Copilot, you'll have access to an experience that is similar to the paid version of ChatGPT. You can create new conversations with a bot and ask it to perform requests using enabled [ChatGPT plugins](../ai-orchestration/chatgpt-plugins.md).

![Chat Copilot reference app](../media/copilot-chat.png)

| Feature | Name | Description |
|:-|:-|:-|
| **1** | Conversation Pane | The left portion of the screen shows different conversation threads the user is holding with the chatbot.  To start a new conversation, click the '+'Bot symbol. |
| **2** | Conversation Thread | Chatbot responses will appear in the main conversation thread, along with a history of your prompts.   Users can scroll up and down to review a complete conversation history. |
| **3** | Prompt Entry Box | The bottom of the screen contains the prompt entry box, where users can type their prompts, and click the "Send" icon to the right of the box when ready to send it to the bot. |

## Learning from the app
What's different about Chat Copilot is that it _also_ provides debugging capabilities that allow you to see how the bot is working behind the scenes. This includes the ability to see the results of the planner, the meta prompt that is used to generate a bot's response. With this information, you can see how the bot is working and debug any issues that you may encounter.

This makes Chat Copilot a great test bed for any plugins you create. By uploading your plugins to Chat Copilot, you can test them out and see how they work with the rest of the platform. 

## Next step

Now that you know what Chat Copilot is capable of, you can now follow the getting started guide to run the app locally.

> [!div class="nextstepaction"]
> [Getting started with Chat Copilot](./getting-started.md)

Once you've run the app locally, you can then learn how to accomplish other tasks with Chat Copilot:
| Goal | Link to documentation |
| --- | --- |
| Customize the app for your needs | [Link](./customizing-chat-copilot.md) |
| Testing your ChatGPT plugins | [Link](./testing-plugins-with-chat-copilot.md) |
| Deploy Chat Copilot to Azure | [Link](./deploying-to-azure.md) |
