---
title: Authentication and API calls sample app
description: Authentication and API calls sample app
author: evchaki
ms.topic: samples
ms.author: evchaki
ms.date: 02/07/2023
ms.prod: semantic-kernel
---
# Authentication and API calls sample app
The Authenticated API’s sample allows you to use authentication to connect to the Microsoft Graph using your personal account. If you don’t have a Microsoft account or do not want to connect to it, you can review the code to see the patterns needed to call out to APIs. The sample highlights connecting to Microsoft Graph and calling APIs for Outlook, OneDrive, and ToDo. Each function will call Microsoft Graph and/or Open AI to perform the tasks.

> [!IMPORTANT]
> Each function will call Open AI which will use tokens that you will be billed for. 

> [!VIDEO https://aka.ms/SK-Samples-SimChat-Video]

# Running the app
The [Authentication and API sample app](https://github.com/microsoft/semantic-kernel/tree/main/samples/starter-identity-webapp-react) is located in the Semantic Kernel GitHub repository.

1) Follow the [Setup](/semantic-kernel/getting-started/setup) instructions if you do not already have a clone of Semantic Kernel locally.
2) Start the [local API service](https://github.com/microsoft/semantic-kernel/tree/main/samples/starter-api-azure-function).
3) Open the ReadMe file in the Authentication and API sample folder.
4) You will need to registrer your application in the Azure Portal. Follow the steps to registrer your app [here](https://learn.microsoft.com/azure/active-directory/develop/quickstart-register-app).
- Your Redirect URI will be <http://localhost:3000>
- It is recommended you use the `Personal Microsoft accounts` account type for this sample
5) Once registrered, copy the Application (client) ID from the Azure Portal and paste in the GUID into the [env](https://github.com/microsoft/semantic-kernel/blob/main/samples/starter-identity-webapp-react/.env) file next to `REACT_APP_GRAPH_CLIENT_ID=`
6) Open the Integrated Terminal window.
7) Run 'yarn install' - if this is the first time you are running the sample.  Then run 'yarn start'.
8) A browser will open with the sample app running

# Exploring the app

## Your Info Screen
You can sign in with your Microsoft account by clicking 'Sign in with Microsoft'.  This will give the sample app access to Microsoft Graph on your behalf and will be used for the functions to run on the Interact screen.

## Setup Screen
Start by entering in your [Open AI key](https://openai.com/api/) or if you are using [Azure Open AI Service](https://learn.microsoft.com/azure/cognitive-services/openai/quickstart) the key and endpoint.  Then enter in the model you would like to use in this sample.

## Interact Screen
When you select each of the 3 options, native functions will be called to preform actions through the graph API.
