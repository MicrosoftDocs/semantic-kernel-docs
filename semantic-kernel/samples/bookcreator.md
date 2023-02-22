---
title: Book creator sample app
description: Book creator sample app
author: evchaki
ms.topic: samples
ms.author: evchaki
ms.date: 02/07/2023
ms.service: mssearch
---
# Book creator sample app

[!INCLUDE [subheader.md](../includes/pat_medium.md)]

The Book creator sample allows you to enter in a topic then the [Planner](/semantic-kernel/concepts-sk/planner) creates a plan for the functions to run based on the ask. You can see the plan along with the results. The  [Writer Skill](https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/WriterSkill) functions are chained togheter based on the asks.

> [!IMPORTANT]
> Each function will call Open AI which will use tokens that you will be billed for. 

### Walkthrough video

>[!Video https://aka.ms/SK-Samples-CreateBook-Video]

## Requirements to run this app

> [!div class="checklist"]
> * [Local API service](/semantic-kernel/samples/localapiservice) is running
> * [Yarn](https://yarnpkg.com/getting-started/install) - used for installing the app's dependencies

## Running the app
The [Book creator sample app](https://github.com/microsoft/semantic-kernel/tree/main/samples/starter-create-book-webapp-react) is located in the Semantic Kernel GitHub repository.

1) Follow the [Setup](/semantic-kernel/getting-started/setup) instructions if you do not already have a clone of Semantic Kernel locally.
2) Start the [local API service](/semantic-kernel/samples/localapiservice).
3) Open the ReadMe file in the Book creator sample folder.
4) Open the Integrated Terminal window.
5) Run `yarn install` - if this is the first time you are running the sample.  Then run `yarn start`.
6) A browser will open with the sample app running

## Exploring the app

### Setup Screen
Start by entering in your [Open AI key](https://openai.com/api/) or if you are using [Azure Open AI Service](https://learn.microsoft.com/azure/cognitive-services/openai/quickstart) the key and endpoint.  Then enter in the model you would like to use in this sample.

### Topics Screen
On this screen you can enter in a topic for the children's book that will be created for you.  This will use functions and AI to generate book ideas based on this topic.

### Book Screen
By clicking on the asks, multiple steps will be found from the Planner and the process will run to return results.

## Take the next step

> [!div class="nextstepaction"]
> [Run the auth and API app](/semantic-kernel/samples/authapi)

[!INCLUDE [footer.md](../includes/footer.md)]
