---
title: How to make changes to the Semantic Kernel web app service
description: How-to guide for making changes to the Semantic Kernel web app service
author: nilesha
ms.topic: Azure
ms.author: nilesha
ms.date: 05/19/2023
ms.service: mssearch
---

# Learn how to make changes to the Semantic Kernel web app service
[!INCLUDE [subheader.md](../includes/pat_medium.md)]

This guide provides steps to make changes to the skills of a deployed instance of the Semantic Kernel web app. Currently, changing semantic skills can be done without redeploying the web app service but changes to native skills do require re-deployments. This document will guide you through the process of doing both.

## Prerequisites
1. An instance of the Semantic Kernel web app service deployed in your Azure subscription. You can follow the how-to guide [here](./deploy-to-azure.md) for details.
1. Have your web app's name handy. If you used the deployment templates provided with the Copilot Chat, you can find the web app's name by going to the [Azure Portal](https://portal.azure.com/) and selecting the resource group created for your Semantic Kernel web app service. Your web app's name is the one of the resource listed that ends with "skweb".
1. Locally tested [skills](../concepts-sk/skills.md) or [planner](../concepts-sk/planner.md) ready to be added to your Semantic Kernel web app service.

> Tip: You can find examples [skills](https://github.com/microsoft/semantic-kernel/tree/main/samples/skills) & [planners](https://github.com/microsoft/semantic-kernel/tree/main/samples/dotnet/kernel-syntax-examples) in the GitHub Semantic Kernel repo.

## How to publish changes to the Semantic Kernel web app service
There are two main ways to deploy changes to the Semantic Kernel web app service. If you have been working locally and are ready to deploy your changes to Azure as a new web app service, you can follow the steps in the first section. If you have already deployed your Semantic Kernel web app service and want to make changes to add Semantic skills, you can follow the steps in the second section.

### 1.Deploying your Copilot Chat App to Azure as a web application
After working locally, i.e. you cloned the code from the GitHub [repo](https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/copilot-chat-app/README.md) and have made changes to the code for your needs, you can deploy your changes to Azure as a web application.

You can use the standard methods available to [deploy an ASP.net web app](/azure/app-service/quickstart-dotnetcore?pivots=development-environment-vs&tabs=net70) in order to do so.

Alternatively, you can follow the steps below to manually build and upload your customized version of the Semantic Kernel service to Azure.

First, at the command line, go to the '../semantic-kernel/samples/apps/copilot-chat-app/webapi' directory and enter the following command:

```powershell
dotnet publish CopilotChatApi.csproj --configuration Release --arch x64 --os win
```

This will create a directory which contains all the files needed for a deployment:
<Br>
```cmd
../semantic-kernel/samples/apps/copilot-chat-app/webapi/bin/Release/net6.0/win-x64/publish'
```
</br>

Zip the contents of that directory and store the resulting zip file on cloud storage, e.g. Azure Blob Container. Put its URI in the "Package Uri" field in the web deployment page you access through the "Deploy to Azure" buttons or use its URI as the value for the PackageUri parameter of the deployment scripts found on this [page](./deploy-to-azure.md).

Your deployment will then use your customized deployment package. That package will be used to create a new Azure web app, which will be configured to run your customized version of the Semantic Kernel service.

### 2. Publish skills directly to the Semantic Kernel web app service
This method is useful for making changes when adding new semantic skills only.

#### How to add Semantic Skills
1. Go to <!-- markdown-link-check-disable -->https://YOUR_APP_NAME.scm.azurewebsites.net<!-- markdown-link-check-enable-->, replacing YOUR_APP_NAME in the URL with your app name found in Azure Portal. This will take you to the [Kudu](/azure/app-service/resources-kudu) console for your app hosting.
2. Click on Debug Console and select CMD.
3. Navigate to the 'site\wwwroot\Skills'
4. Create a new folder using the (+) sign at the top and give a folder name to store your Semantic Skills e.g. SemanticSkills.
5. Now you can drag and drop your Semantic Skills into this folder
6. Next navigate to 'site\wwwroot'
7. Click on the pencil icon to edit the appsettings.json file.
8. In the appsettings.json file, update the SemanticSkillDirectory with the location of the skills you have created.
```json
    "Service": {
    "SemanticSkillsDirectory": "/SemanticSkills",
    "KeyVaultUri": ""
  },
```
9. Click on "Save" to save the changes to the appsettings.json file.
10. Now your web app is configured to use your Semantic Skills.

## Take the next step
>To explore how you build a front-end web app explore the [Copilot Chat App](../samples/copilotchat.md) sample.

>If you have not already done so, please star the GitHub repo and join the Semantic Kernel community!
[Star the Semantic Kernel repo](https://aka.ms/sk/repo)

[!INCLUDE [footer.md](../includes/footer.md)]
