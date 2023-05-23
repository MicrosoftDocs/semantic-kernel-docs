---
title: How to make changes to Semantic Kernel web app service
description: How to guide for making changes to the Semantic Kernel web app service
author: nilesha
ms.topic: Azure
ms.author: nilesha
ms.date: 05/19/2023
ms.service: mssearch
---

# Learn how make changes to Semantic Kernel web app service
[!INCLUDE [subheader.md](../includes/pat_medium.md)]

In this how to guide we will provide steps to make changes to your deployed Semantic Kernel web App to update  skills. Currently you can only make changes to Semantic Skills without redeploying a new web app service. Don't worry though, we will guide you through the process in order you want to make changes beyond Semantic Skills. 

## Prerequisites
1. Semantic Kernel web app service must deployed in your Azure subscription. You can follow the how to guide [here](./deploy-to-azure.md) for details.
1. Have your Web App Name handy. To find the Web App Name, goto [Azure Portal](https://portal.azure.com/). Within the resource group created for your Semantic Kernel Web App Service. Find the 'App Service' resource and click on it. You will see the App Name in the Web App Section next to the name.
1. Locally tested [skills](../concepts-sk/skills.md) or [planner](../concepts-sk/planner.md) ready to be added to your Semantic Kernel web app service.

> Tip: You can find examples [skills](https://github.com/microsoft/semantic-kernel/tree/main/samples/skills) & [planners](https://github.com/microsoft/semantic-kernel/tree/main/samples/dotnet/kernel-syntax-examples) in the GitHub Semantic Kernel repo.

## How to make changes to Semantic Kernel web app service 
There are two main ways to deploy changes to the Semantic Kernel web app service. If you have been working locally and are ready to deploy your changes to Azure as a new web app service, you can follow the steps in the first section. If you have already deployed your Semantic Kernel web app service and want to make changes to add Semantic skills, you can follow the steps in the second section. 

### 1.Deploying your Copilot Chat App to Azure as a web application 
After working localley, i.e. you cloned the code from the GitHub [repo](https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/copilot-chat-app/README.md) and have made changes to the code for your needs, you can deploy your changes to Azure as a new web application. 

To do this you will need to build and upload your customized version of the Semantic Kernel service to Azure. Once you are ready, using Powershell or commandline tool go to '../semantic-kernel/samples/apps/copilot-chat-app/webapi' directory and enter the following command:

```powershell
dotnet publish CopilotChatApi.csproj --configuration Release --arch x64 --os win
```

This will create a directory which contains all the files needed for a deployment:
<Br>
```cmd
../semantic-kernel/samples/apps/copilot-chat-app/webapi/bin/Release/net6.0/win-x64/publish'
```
</br>

Zip the contents of that directory and store the resulting zip file on cloud storage e.g. Azure Blob Container. Put its URI in the "Package Uri" field in the web deployment page you access through the "Deploy to Azure" buttons or use its URI as the value for the PackageUri parameter of the Powershell scripts found on this [page](./deploy-to-azure.md).

Your deployment will then use your customized deployment package. That package will be used to create a new Azure Web App, which will be configured to run your customized version of the Semantic Kernel service. 
Now you have a new web app service with your customized version of Semantic Kernel! 

### 2. Publish changes directly to the Semantic Kernel web app service without creating a new web app service.
Changes via this method are limited since the Web App does not contain binaries for Semantic Kernel. This method is useful for making changes when adding new Semantic skills only.

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
9. Click on Save to save the changes to the appsettings.json file.
10. Now your web app is configured to use your Semantic Skills.

## Take the next step
>To explore how you build a front end web app explore the [Copilot Chat App](../samples/copilotchat.md) sample.

>If you have not already done so please star the GitHub repo and join the Semantic Kernel community! 
[Star the Semantic Kernel repo](https://aka.ms/sk/repo)

[!INCLUDE [footer.md](../includes/footer.md)]
