---
title: Simple chat summary sample app
description: Simple chat summary sample app
author: evchaki
ms.topic: samples
ms.author: evchaki
ms.date: 02/07/2023
ms.service: semantic-kernel
---
# Simple chat summary sample app

[!INCLUDE [subheader.md](../includes/pat_large.md)]

The Simple Chat Summary sample allows you to see the power of [functions](../prompt-engineering/your-first-prompt.md) used in a chat sample app.  The sample highlights the [Summarize](https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/SummarizeSkill/Summarize), [Topics](https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/SummarizeSkill/Topics) and [Action Items](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Skills/Skills.Core/ConversationSummarySkill.cs) functions in the [Summarize Plugin](https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/SummarizeSkill).  Each function calls OpenAI to review the information in the chat window and produces insights.   

> [!IMPORTANT]
> Each function will call OpenAI which will use tokens that you will be billed for. 

### Walkthrough video
> [!VIDEO https://aka.ms/SK-Samples-SimChat-Video]

## Requirements to run this app

> [!div class="checklist"]
> * [Local API service](/semantic-kernel/samples/localapiservice) is running
> * [Yarn](https://yarnpkg.com/getting-started/install) - used for installing the app's dependencies

## Running the app
The [Simple chat summary sample app](https://github.com/microsoft/semantic-kernel/tree/main/samples/apps/chat-summary-webapp-react) is located in the Semantic Kernel GitHub repository.

1) Follow the [Setup](/semantic-kernel/get-started) instructions if you do not already have a clone of Semantic Kernel locally.
2) Start the [local API service](/semantic-kernel/samples/localapiservice).
3) Open the ReadMe file in the Simple Chat Summary sample folder.
4) Open the Integrated Terminal window.
5) Run `yarn install` - if this is the first time you are running the sample.  Then run `yarn start`.
6) A browser will open with the sample app running

## Exploring the app

### Setup Screen
Start by entering in your [OpenAI key](https://openai.com/api/) or if you are using [Azure OpenAI Service](/azure/cognitive-services/openai/quickstart) the key and endpoint.  Then enter in the model you would like to use in this sample.

### Interact Screen
A preloaded chat conversation is avaialble.  You can add additional items in the chat or modify the [chat thread](https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/chat-summary-webapp-react/src/components/chat/ChatThread.ts) before running the sample. 

### AI Summaries Screen
Three semantic functions are called on this screen
1) [Summarize](https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/SummarizeSkill/Summarize)
2) [Topics](https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/SummarizeSkill/Topics) 
3) [Action Items](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Skills/Skills.Core/ConversationSummarySkill.cs) 

## Next step

> [!div class="nextstepaction"]
> [Run the book creator app](/semantic-kernel/samples/bookcreator)
