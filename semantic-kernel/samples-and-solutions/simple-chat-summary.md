---
title: Simple chat summary sample app
description: Simple chat summary sample app
author: evchaki
ms.topic: samples
ms.author: evchaki
ms.date: 02/07/2023
ms.service: mssearch
---
# Simple chat summary sample app

[!INCLUDE [subheader.md](../includes/pat_medium.md)]

The Simple Chat Summary sample allows you to see the power of [functions](/semantic-kernel/skills/skfunctions) used in a chat sample app.  The sample highlights the [Summarize](https://aka.ms/sk/repo/summarize), [Topics](https://aka.ms/sk/repo/topics) and [Action Items](https://aka.ms/sk/repo/actionitems) functions in the [Summarize Skill](https://aka.ms/sk/repo/summarizeskill).  Each function calls OpenAI to review the information in the chat window and produces insights.   

> [!IMPORTANT]
> Each function will call OpenAI which will use tokens that you will be billed for. 

### Walkthrough video
> [!VIDEO https://aka.ms/SK-Samples-SimChat-Video]

## Requirements to run this app

> [!div class="checklist"]
> * [Local API service](/semantic-kernel/samples/localapiservice) is running
> * [Yarn](https://yarnpkg.com/getting-started/install) - used for installing the app's dependencies

## Running the app
The [Simple chat summary sample app](https://aka.ms/sk/repo/samples/starter-chat) is located in the Semantic Kernel GitHub repository.

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
A preloaded chat conversation is avaialble.  You can add additional items in the chat or modify the [chat thread](https://aka.ms/sk/repo/samples/starter-chat/chat-thread) before running the sample. 

### AI Summaries Screen
Three semantic functions are called on this screen
1) [Summarize](https://aka.ms/sk/repo/summarize)
2) [Topics](https://aka.ms/sk/repo/topics) 
3) [Action Items](https://aka.ms/sk/repo/actionitems) 

## Next step

> [!div class="nextstepaction"]
> [Run the book creator app](/semantic-kernel/samples/bookcreator)

[!INCLUDE [footer.md](../includes/footer.md)]
