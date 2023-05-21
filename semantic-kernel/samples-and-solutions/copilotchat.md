---
title: Copilot Chat Sample App
description: Copilot Chat sample app
author: smonroe
ms.topic: samples
ms.author: smonroe
ms.date: 04/07/2023
ms.service: mssearch
---
# Copilot Chat Sample App

[!INCLUDE [subheader.md](../includes/pat_medium.md)]

The Copilot Chat sample allows you to build your own integrated large language model chatbot.  This is an enriched intelligence app, with multiple dynamic components including command messages, user intent, and memories.  

The chat prompt and response will evolve as the conversation between the user and the application proceeds.  This chat experience is a chat skill containing multiple functions that work together to construct the final prompt for each exchange.


> [!IMPORTANT]
> Each function will call OpenAI which will use tokens that you will be billed for. 


## Requirements to run this app

> [!div class="checklist"]
> * [Visual Studio Code](https://code.visualstudio.com/Download)
> * [Git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)
> * [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
> * [Node.js](https://nodejs.org/en/download)
> * [Yarn](https://classic.yarnpkg.com/lang/en/docs/install)

## Running the app
The [Copilot Chat Sample App](https://aka.ms/sk/repo/samples/copilot-chat-app) is located in the Semantic Kernel GitHub repository.

1) Follow the [Setup](/semantic-kernel/get-started) instructions if you do not already have a clone of Semantic Kernel locally.
2) Follow the instructions to [Register an Azure Application](/azure/active-directory/develop/quickstart-register-app)
3) Open the ReadMe file in the Copilot Chat sample folder.
4) Follow the ReadMe instructions to configure, start, and test the Backend API Server
5) Follow the ReadMe instructions to start the front end WebApp
5) A browser should automatically launch and navigate to https://localhost:3000. with the sample app running

## Exploring the app

### Conversation Title
This will default to date and time, but can easily be edited by clicking the pencil icon.


### Conversation Panel
The left portion of the screen shows different conversation threads the user is holding with the chatbot.  To start a new conversation, click the '+'Bot symbol.

### Conversation Thread
Chatbot responses will appear in the main conversation thread, along with a history of your prompts.   Users can scroll up and down to review a complete conversation history.

### Prompt Entry Box
The bottom of the screen contains the prompt entry box, where users can type their prompts, and click the "Send" icon to the right of the box when ready to send it to the bot.

### Deploying your Copilot Chat App to Azure as a web application 
You can build and upload a customized version of the Semantic Kernel service to Azure.

Once you have cloned the code from the GitHub [repo](https://aka.ms/sk/repo/samples/copilot-chat-app),you can choose to modify for your needs (for example, by adding your own skills) or leave as is. Once you are ready, go into the ../semantic-kernel/samples/apps/copilot-chat-app/webapi
directory and enter the following command:
```powershell
dotnet publish CopilotChatApi.csproj --configuration Release --arch x64 --os win
```

This will create a directory which contains all the files needed for a deployment:
../semantic-kernel/samples/apps/copilot-chat-app/webapi/bin/Release/net6.0/win-x64/publish

Zip the contents of that directory and store the resulting zip file on cloud storage.

Put its URI in the "Package Uri" field in the web deployment page you access through the "Deploy to Azure" buttons above, or use its URI as the value for the PackageUri parameter of the Powershell scripts above.

Your deployment will then use your customized deployment package. That package will be used to create a new Azure Web App, which will be configured to run your customized version of the Semantic Kernel service.

## Next step

If you've tried all the apps and are excited to see more, please star the GitHub repo and join the SK community!

> [!div class="nextstepaction"]
> [Star the SK repo](https://aka.ms/sk/repo)

[!INCLUDE [footer.md](../includes/footer.md)]
