---
title: Deploy Semantic Kernel to Azure as a web app service
description: How to guide on how to deploy Semantic Kernel to Azure in a web app service
author: nilesha
ms.topic: Azure
ms.author: nilesha
ms.date: 05/10/2023
ms.service: mssearch
---

# Learn how to deploy Semantic Kernel to Azure as a web app service
[!INCLUDE [subheader.md](../includes/pat_medium.md)]

You can use one of the following methods to deploy Semantic Kernel to Azure in a web app service based on your use case.  

| Use Case     | Deployment Option     |
|--------------|-----------|
| <u>Create new : Azure Open AI Resources</u> <p>Use this option to create a new instance Azure Open AI instance and deploy Semantic Kernel to it.<br>You will deploy an instance of Semantic Kernel in a web app service within a resource group that bears the name [YOUR_DEPLOYMENT_NAME] preceded by the "rg-" prefix.| [![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fmicrosoft%2Fsemantic-kernel%2Fmain%2Fsamples%2Fapps%2Fcopilot-chat-app%2Fwebapi%2FDeploymentTemplates%2Fsk.json) <br> [Powershell File ](https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/copilot-chat-app/webapi/DeploymentTemplates/DeploySK.ps1) <br> [Bash File](https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/copilot-chat-app/webapi/DeploymentTemplates/DeploySK.sh)|
| <u>Use existing : Azure Open AI Resources</u> <p>Will use existing Azure Open AI deployed resources in Subscription for Semantic Kernel for app web service.  | [![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fmicrosoft%2Fsemantic-kernel%2Fmain%2Fsamples%2Fapps%2Fcopilot-chat-app%2Fwebapi%2FDeploymentTemplates%2Fsk-existing-azureopenai.json)<br>[PowerShell File](https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/copilot-chat-app/webapi/DeploymentTemplates/DeploySK-Existing-AzureOpenAI.ps1)<br>[Bash File](https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/copilot-chat-app/webapi/DeploymentTemplates/DeploySK-Existing-AzureOpenAI.sh)
 <u>Use existing: Open AI Keys</u><p> Will leverage Open AI models for Semantic Kernel Web App Service   | [![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fmicrosoft%2Fsemantic-kernel%2Fmain%2Fsamples%2Fapps%2Fcopilot-chat-app%2Fwebapi%2FDeploymentTemplates%2Fsk-existing-openai.json)<br>[PowerShell File](https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/copilot-chat-app/webapi/DeploymentTemplates/DeploySK-Existing-OpenAI.ps1)<br>[Bash File](https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/copilot-chat-app/webapi/DeploymentTemplates/DeploySK-Existing-OpenAI.sh) |

 
## Script Parameters
Below are a list of required parameters each use case if you are using Powershell or Bash. You can find additional parameters options in each file. 
### <b>Powershell</b>

* Creating new Azure Open AI Resources
```powershell
.\DeploySK.ps1 -DeploymentName YOUR_DEPLOYMENT_NAME -Subscription YOUR_SUBSCRIPTION_ID
```
* Use existing Azure Open AI Resources

After entering the command below, you will be prompted to enter your Azure OpenAI API key. (You can also pass in the API key using the -ApiKey parameter)

```powershell
.\DeploySK-Existing-AzureOpenAI.ps1 -DeploymentName YOUR_DEPLOYMENT_NAME -Subscription YOUR_SUBSCRIPTION_ID -Endpoint "YOUR_AZURE_OPENAI_ENDPOINT"
```

* Use existing Open AI Keys

After entering the command below, you will be prompted to enter your OpenAI API key. (You can also pass in the API key using the -ApiKey parameter)

```powershell
.\DeploySK-Existing-OpenAI.ps1 -DeploymentName YOUR_DEPLOYMENT_NAME -Subscription YOUR_SUBSCRIPTION_ID
```

### <b>Bash</b>
* Creating new Azure Open AI Resources
```bash
./DeploySK.sh YOUR_DEPLOYMENT_NAME YOUR_SUBSCRIPTION_ID
```
* Use existing Azure Open AI Resources
```bash
./DeploySK-Existing-AzureOpenAI.sh YOUR_DEPLOYMENT_NAME YOUR_API_KEY "YOUR_AZURE_OPENAI_ENDPOINT" YOUR_SUBSCRIPTION_ID
```
* Use existing Open AI Keys
```bash
 ./DeploySK-Existing-AI.sh YOUR_DEPLOYMENT_NAME YOUR_API_KEY YOUR_SUBSCRIPTION_ID
```

## Considerations

1. Azure currently limits the number of Azure OpenAI resources per region per subscription to 3. Azure OpenAI is not available in every region.
(Refer to this [availability map](https://azure.microsoft.com/en-us/explore/global-infrastructure/products-by-region/?products=cognitive-services))
Bearing this in mind, you might want to use the same Azure OpenAI instance for multiple deployments of Semantic Kernel to Azure.

2. F1 and D1 App Service SKU's (the Free and Shared ones) are not supported for this deployment.

3. Ensure you have sufficient permissions to create resources in the target subscription.

4. Using web frontends to access your deployment: Make sure to include your frontend's URL as an allowed origin in your deployment's CORS settings. Otherwise, web browsers will refuse to let JavaScript make calls to your deployment.
<Br></br>

## Verifying the deployment

To make sure your web app service is running, go to <!-- markdown-link-check-disable -->https://YOUR_INSTANCE_NAME.azurewebsites.net/probe<!-- markdown-link-check-enable-->

To get your instance's URL, click on the "Go to resource group" button you see at the end of your deployment. Then click on the resource whose name starts with "app-".

This will bring you to the Overview page on your web service. Your instance's URL is the value that appears next to the "Default domain" field.
<Br></br>

## Changing your configuration, monitoring your deployment and troubleshooting

After your deployement is complete, in Azure Portal, you can change your configuration by clicking on the "Configuration" item in the "Settings" section of the left pane.

Scrolling down in that same pane to the "Monitoring" section gives you access to a multitude of ways to monitor your deployment.

In addition to this, the "Diagnose and "solve problems" item near the top of the pane can yield crucial insight into some problems your deployment may be experiencing.

If the service itself if functioning properly but you keep getting errors (perhaps reported as 400 HTTP errors) when making calls to the Semantic Kernel,
check that you have correctly entered the values for the following settings:
- Completion:AzureOpenAI
- Completion:DeploymentOrModelId
- Completion:Endpoint
- Completion:Label
- Embedding:AzureOpenAI
- Embedding:DeploymentOrModelId
- Embedding:Endpoint
- Embedding:Label

Both Completion:Endpoint and Embedding:Endpoint are ignored for OpenAI instances from [openai.com](https://openai.com) but MUST be properly populated when using Azure OpenAI instances.
<br></br>
## How to clean up resources

To save costs you may want to clean up resources from this deployment. You can do this by deleting the resource group that contains you created via Azure portal or run the following in [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/):
```powershell
az group delete --name YOUR_RESOURCE_GROUP
```

## Take the next step
Try deploying a custom Co-pilot to your SK instance. Follow the steps [here](../samples/copilotchat.md)

If you have not already done so please star the GitHub repo and join the SK community! 
[Star the SK repo](https://aka.ms/sk/repo)
