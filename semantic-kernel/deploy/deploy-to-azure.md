---
title: Deploy Semantic Kernel to Azure as a web app service
description: How-to guide on how to deploy Semantic Kernel to Azure in a web app service
author: nilesha
ms.topic: Azure
ms.author: nilesha
ms.date: 05/19/2023
ms.service: mssearch
---

# Learn how to deploy Semantic Kernel to Azure as a web app service
[!INCLUDE [subheader.md](../includes/pat_large.md)]

In this how-to guide we will provide steps to deploy Semantic Kernel to Azure as a web app service. 
Deploying Semantic Kernel as web service to Azure provides a great pathway for developers to take advantage of Azure compute and other services such as Azure Cognitive Services for responsible AI and vectorized databases.  

You can use one of the deployment options to deploy based on your use case and preference.

## Considerations
1. Azure currently limits the number of Azure OpenAI resources per region per subscription to 3. Azure OpenAI is not available in every region.
(Refer to this [availability map](https://azure.microsoft.com/explore/global-infrastructure/products-by-region/?products=cognitive-services)) Bearing this in mind, you might want to use the same Azure OpenAI instance for multiple deployments of Semantic Kernel to Azure.

1. F1 and D1 App Service SKU's (the Free and Shared ones) are not supported for this deployment.

1. Ensure you have sufficient permissions to create resources in the target subscription.

1. Using web frontends to access your deployment: make sure to include your frontend's URL as an allowed origin in your deployment's CORS settings. Otherwise, web browsers will refuse to let JavaScript make calls to your deployment.
<Br></br>

| Use Case     | Deployment Option     |
|--------------|-----------|
| <u>Use existing: Azure OpenAI Resources</u><p>Use this option to use an existing Azure OpenAI instance and connect the Semantic Kernel web API to it.| [![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://aka.ms/sk-deploy-existing-azureopenai-portal)<br>[PowerShell File](https://aka.ms/sk-deploy-existing-azureopenai-powershell)<br>[Bash File](https://aka.ms/sk-deploy-existing-azureopenai-bash)
|<u>Create new: Azure OpenAI Resources</u> <p>Use this option to deploy Semantic Kernel in a web app service and have it use a new instance of Azure OpenAI.<p>Note: access to new [Azure OpenAI](/azure/cognitive-services/openai/overview) resources is currently limited due to high demand. | [![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://aka.ms/sk-deploy-new-azureopenai-portal) <br> [PowerShell File ](https://aka.ms/sk-deploy-new-azureopenai-powershell) <br> [Bash File](https://aka.ms/sk-deploy-new-azureopenai-bash)
|<u>Use existing: OpenAI Resources</u><p>Use this option to use your OpenAI account and connect the Semantic Kernel web API to it. | [![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://aka.ms/sk-deploy-openai-portal)<br>[PowerShell File](https://aka.ms/sk-deploy-openai-powershell)<br>[Bash File](https://aka.ms/sk-deploy-openai-bash) |

 
## Script Parameters
Below are examples on how to run the PowerShell and bash scripts. Refer to each of the script files for the complete list of available parameters and usage. 
### <b>PowerShell</b>

* Creating new Azure OpenAI Resources
```powershell
.\DeploySK.ps1 -DeploymentName YOUR_DEPLOYMENT_NAME -Subscription YOUR_SUBSCRIPTION_ID
```
* Using existing Azure OpenAI Resources

After entering the command below, you will be prompted to enter your Azure OpenAI API key. (You can also pass in the API key using the -ApiKey parameter)

```powershell
.\DeploySK-Existing-AzureOpenAI.ps1 -DeploymentName YOUR_DEPLOYMENT_NAME -Subscription YOUR_SUBSCRIPTION_ID -Endpoint "YOUR_AZURE_OPENAI_ENDPOINT"
```

* Using existing OpenAI Resources

After entering the command below, you will be prompted to enter your OpenAI API key. (You can also pass in the API key using the -ApiKey parameter)

```powershell
.\DeploySK-Existing-OpenAI.ps1 -DeploymentName YOUR_DEPLOYMENT_NAME -Subscription YOUR_SUBSCRIPTION_ID
```

### <b>Bash</b>
* Creating new Azure OpenAI Resources
```bash
./DeploySK.sh -d DEPLOYMENT_NAME -s SUBSCRIPTION_ID
```
* Using existing Azure OpenAI Resources
```bash
./DeploySK-Existing-AzureOpenAI.sh -d YOUR_DEPLOYMENT_NAME -s YOUR_SUBSCRIPTION_ID -e YOUR_AZURE_OPENAI_ENDPOINT -o YOUR_AZURE_OPENAI_API_KEY
```
* Using existing OpenAI Resources
```bash
 ./DeploySK-Existing-AI.sh -d YOUR_DEPLOYMENT_NAME -s YOUR_SUBSCRIPTION_ID -o YOUR_OPENAI_API_KEY
```
### Azure Portal Template
If you choose to use Azure Portal as your deployment method, you will need to review and update the template form to create the resources. Below is a list of items you will need to review and update.
1. Subscription: decide which Azure subscription you want to use. This will house the resource group for the Semantic Kernel web application.
1. Resource Group: the resource group in which your deployment will go. Creating a new resource group helps isolate resources, especially if you are still in active development.
1. Region: select the geo-region for deployment. Note: Azure OpenAI is not available in all regions and is currently to three instances per region per subscription.
1. Name: used to identify the app.
App Service SKU: select the pricing tier based on your usage. Click [here](https://azure.microsoft.com/en-ca/pricing/details/app-service/windows/) to learn more about Azure App Service plans.
1. Package URI: there is no need to change this unless you want to deploy a customized version of Semantic Kernel. (See [this page](https://learn.microsoft.com/en-us/semantic-kernel/deploy/publish-changes-to-azure) for more information on publishing your own version of the Semantic Kernel web app service)
1. Completion, Embedding and Planner Models: these are by default using the appropriate models based on the current use case - that is Azure OpenAI or OpenAI. You can update these based on your needs.
1. Endpoint: this is only applicable if using Azure OpenAI and is the Azure OpenAI endpoint to use.
1. API Key: enter the API key for the instance of Azure OpenAI or OpenAI to use.
1. Semantic Kernel API Key: the default value of "\[newGuid()\]" in this field will create an API key to protect you Semantic Kernel endpoint. You can change this by providing your own API key. If you do not want to use API authorization, you can make this field blank.
1. CosmosDB: whether to deploy a CosmosDB resource to store chats. Otherwise, volatile memory will be used.
1. Qdrant: whether to deploy a Qdrant database to store embeddings. Otherwise, volatile memory will be used.
1. Speech Services: whether to deploy an instance of the Azure Speech service to provide speech-to-text for input.

## What resources are deployed?
Below is a list of the key resources created within the resource group when you deploy Semantic Kernel to Azure as a web app service.
1. Azure web app service: hosts Semantic Kernel
1. Application Insights: application logs and debugging 
1. Azure Cosmos DB: used for chat storage (optional)
1. Qdrant vector database (within a container): used for embeddings storage (optional)
1. Azure Speech service: used for speech-to-text (optional)

## Verifying the deployment

To make sure your web app service is running, go to <!-- markdown-link-check-disable -->https://YOUR_INSTANCE_NAME.azurewebsites.net/probe<!-- markdown-link-check-enable-->

To get your instance's URL, go to your deployment's resource group (by clicking on the "Go to resource group" button seen at the conclusion of your deployment if you use the "Deploy to Azure" button). Then click on the resource whose name ends with "-skweb".

This will bring you to the Overview page on your web service. Your instance's URL is the value that appears next to the "Default domain" field.

## Changing your configuration, monitoring your deployment and troubleshooting

After your deployment is complete, you can change your configuration in the Azure Portal by clicking on the "Configuration" item in the "Settings" section of the left pane found in the Semantic Kernel web app service page.

Scrolling down in that same pane to the "Monitoring" section gives you access to a multitude of ways to monitor your deployment.

In addition to this, the "Diagnose and solve problems" item near the top of the pane can yield crucial insight into some problems your deployment may be experiencing.

If the service itself is functioning properly but you keep getting errors (perhaps reported as 400 HTTP errors) when making calls to the Semantic Kernel, 
check that you have correctly entered the values for the following settings:
- AIService:AzureOpenAI
- AIService:Endpoint
- AIService:Models:Completion
- AIService:Models:Embedding
- AIService:Models:Planner

AIService:Models:Completion is ignored for OpenAI instances from [openai.com](https://openai.com) but MUST be properly populated when using Azure OpenAI instances.


## How to clean up resources
When you want to clean up the resources from this deployment, use the Azure portal or run the following [Azure CLI](/cli/azure/) command:
```powershell
az group delete --name YOUR_RESOURCE_GROUP
```

## Take the next step
>Learn [how to make changes to your Semantic Kernel web app](./publish-changes-to-azure.md), such as adding new skills.

>If you have not already done so, please star the GitHub repo and join the Semantic Kernel community! 
[Star the Semantic Kernel repo](https://aka.ms/sk/repo)

[!INCLUDE [footer.md](../includes/footer.md)]
