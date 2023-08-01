---
title: Authentication and API calls sample app
description: Authentication and API calls sample app
author: evchaki
ms.topic: samples
ms.author: evchaki
ms.date: 02/07/2023
ms.service: mssearch
---
# Authentication and API calls sample app
The Authenticated API’s sample allows you to use authentication to connect to the Microsoft Graph using your personal account. If you don’t have a Microsoft account or do not want to connect to it, you can review the code to see the patterns needed to call out to APIs. The sample highlights connecting to Microsoft Graph and calling APIs for Outlook, OneDrive, and ToDo. Each function will call Microsoft Graph and/or OpenAI to perform the tasks.

> [!IMPORTANT]
> Each function will call OpenAI which will use tokens that you will be billed for. 

### Walkthrough video

> [!VIDEO https://aka.ms/SK-Samples-AuthAPI-Video]

## Requirements to run this app

> [!div class="checklist"]
> * [Local API service](/semantic-kernel/samples/localapiservice) is running
> * [Yarn](https://yarnpkg.com/getting-started/install) - used for installing the app's dependencies

## Running the app
The [Authentication and API sample app](https://github.com/microsoft/semantic-kernel/tree/main/samples/apps/auth-api-webapp-react) is located in the Semantic Kernel GitHub repository.

1) Follow the [Setup](/semantic-kernel/get-started) instructions if you do not already have a clone of Semantic Kernel locally.
2) Start the [local API service](/semantic-kernel/samples/localapiservice).
3) Open the ReadMe file in the Authentication and API sample folder.
4) You will need to register your application in the Azure Portal. Follow the steps to register your app [here](/azure/active-directory/develop/quickstart-register-app).
- Your Redirect URI will be <http://localhost:3000>
- It is recommended you use the `Personal Microsoft accounts` account type for this sample
5) Once registered, copy the Application (client) ID from the Azure Portal and paste in the GUID into the [env](https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/auth-api-webapp-react/.env.example) file next to `REACT_APP_GRAPH_CLIENT_ID=`
6) Open the Integrated Terminal window.
7) Run `yarn install` - if this is the first time you are running the sample.  Then run `yarn start`.
8) A browser will open with the sample app running

## Exploring the app

### Your Info Screen
You can sign in with your Microsoft account by clicking 'Sign in with Microsoft'.  This will give the sample app access to Microsoft Graph on your behalf and will be used for the functions to run on the Interact screen.

### Setup Screen
Start by entering in your [OpenAI key](https://openai.com/api/) or if you are using [Azure OpenAI Service](/azure/cognitive-services/openai/quickstart) the key and endpoint.  Then enter in the model you would like to use in this sample.

### Interact Screen
When you select each of the 3 actions, native functions will be called to preform actions through the Microsoft Graph API and connector.

The actions on this screen are:
1. Summarize and create a new Word document and save it to OneDrive
2. Get a shareable link and email the link to myself
3. Add a reminder to follow-up with the email sent above

## Take the next step

> [!div class="nextstepaction"]
> [Run the GitHub Repo Q&A Bot app](/semantic-kernel/samples/githubrepoqabot)
