---
title: Deploy Semantic Kernel to Azure as a web app service
description: How to guide on how to deploy Semantic Kernel to Azure in a web app service
author: nilesha
ms.topic: Azure
ms.author: nilesha
ms.date: 05/19/2023
ms.service: mssearch
---

# Learn how to deploy Semantic Kernel to Azure as a web app service
[!INCLUDE [subheader.md](../includes/pat_large.md)]

In this how to guide we will provide steps to deploy Semantic Kernel to Azure as a web app service. 
Deploying Semantic Kernel as Web Service to Azure provides a great pathway for developers to take advantage of Azure compute and other services such as Azure Cognitive Services for Responsible AI and Vectorized databases among services.  

You can use one of the deployment options to deploy based on your use case and preference.

## Considerations
1. Azure currently limits the number of Azure OpenAI resources per region per subscription to 3. Azure OpenAI is not available in every region.
(Refer to this [availability map](https://azure.microsoft.com/explore/global-infrastructure/products-by-region/?products=cognitive-services)) Bearing this in mind, you might want to use the same Azure OpenAI instance for multiple deployments of Semantic Kernel to Azure.

2. F1 and D1 App Service SKU's (the Free and Shared) are not supported for this deployment.

3. Ensure you have sufficient permissions to create resources in the target subscription.

4. Using web frontends to access your deployment: Make sure to include your frontend's URL as an allowed origin in your deployment's CORS settings. Otherwise, web browsers will refuse to let JavaScript make calls to your deployment.
<Br></br>

| Use Case     | Deployment Option     |
|--------------|-----------|
| <u>Use existing : Azure OpenAI Resources</u><p>Use this option to use an existing Azure OpenAI instance and connect the Semantic Kernel web API to it.| [![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://aka.ms/sk-deploy-existing-azureopenai-portal)<br>[PowerShell File](https://aka.ms/sk-deploy-existing-azureopenai-powershell)<br>[Bash File](https://aka.ms/sk-deploy-existing-azureopenai-bash)
|<u>Create new : Azure OpenAI Resources</u> <p>Use this option to deploy Semantic Kernel in a web app service and have it use a new instance of Azure OpenAI.<p>Note: Getting access to New [Azure OpenAI](/azure/cognitive-services/openai/overview) Resources is limited due to high demand. | [![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://aka.ms/sk-deploy-new-azureopenai-portal) <br> [PowerShell File ](https://aka.ms/sk-deploy-new-azureopenai-powershell) <br> [Bash File](https://aka.ms/sk-deploy-new-azureopenai-bash)
|<u>Use existing: OpenAI Resources</u><p>Use this option to use your OpenAI account and connect the Semantic Kernel web API to it. | [![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://aka.ms/sk-deploy-openai-portal)<br>[PowerShell File](https://aka.ms/sk-deploy-openai-powershell)<br>[Bash File](https://aka.ms/sk-deploy-openai-bash) |

 
## Script Parameters
If you choose to use Powershell or Bash scripts, below are examples on how to run these scripts. Refer to each of the script files for the complete list of available parameters and usage. 
### <b>Powershell</b>

* Creating new Azure OpenAI Resources
```powershell
.\DeploySK.ps1 -DeploymentName YOUR_DEPLOYMENT_NAME -Subscription YOUR_SUBSCRIPTION_ID
```
* Use existing Azure OpenAI Resources

After entering the command below, you will be prompted to enter your Azure OpenAI API key. (You can also pass in the API key using the -ApiKey parameter)

```powershell
.\DeploySK-Existing-AzureOpenAI.ps1 -DeploymentName YOUR_DEPLOYMENT_NAME -Subscription YOUR_SUBSCRIPTION_ID -Endpoint "YOUR_AZURE_OPENAI_ENDPOINT"
```

* Use existing OpenAI Resources

After entering the command below, you will be prompted to enter your OpenAI API key. (You can also pass in the API key using the -ApiKey parameter)

```powershell
.\DeploySK-Existing-OpenAI.ps1 -DeploymentName YOUR_DEPLOYMENT_NAME -Subscription YOUR_SUBSCRIPTION_ID
```

### <b>Bash</b>
* Creating new Azure OpenAI Resources
```bash
./DeploySK.sh YOUR_DEPLOYMENT_NAME YOUR_SUBSCRIPTION_ID
```
* Use existing Azure OpenAI Resources
```bash
./DeploySK-Existing-AzureOpenAI.sh YOUR_DEPLOYMENT_NAME YOUR_API_KEY "YOUR_AZURE_OPENAI_ENDPOINT" YOUR_SUBSCRIPTION_ID
```
* Use existing OpenAI Resources
```bash
 ./DeploySK-Existing-AI.sh YOUR_DEPLOYMENT_NAME YOUR_API_KEY YOUR_SUBSCRIPTION_ID
```
### Azure Portal Template
If you choose to use Azure Portal as your deployment method, you will need to review and update the template form to create the resources. Below is a list of items you will need to review and update.
1. Subscription: Decide which subscription you want to use, you may see 1 or many depending on your role's permissions. This will house the resource group for Semantic Kernel Web Application.
1. Resource Group: you can use an existing resource group or create a new resource group. I recommend creating a new resource group to help isolate resources, especially if you still in active development.
1. Region: Select the geo-region for deployment. Note: OpenAI is not available in all regions, and Azure OpenAI currently limits 3 resources groups per region per subscription. Refer to this availability map for more details.
1. Name: This be used as a pre-fix to name the app.
App Service SKU: Select the pricing tier based on your usage. Click here to learn more about Azure App Service Plan
1. Package URI: There is no need to change this. Should you in future want to deploy a customized version of Semantic Kernel you can update to use your package URI.
1. Completion, Embedding and Planner Model: These are by default using the appropriate models based on the current use case e.g. Azure OpenAI or OpenAI. You can update these based on your preference for available models by each provider.
1. End point: If using Azure OpenAI enter the Model Endpoint, this can be found for the model you are using in Azure OpenAI Studio.
1. API Key: Enter the API key for either Azure OpenAI or OpenAI
Semantic Kernel API Key: This is created automatically via the template for authentication. If you do not want to use any authentication you can remove the value in this field.
1. CosmosDB: Deploy this resource if you want to use store chats, this is optional.
1. Qdrant: This is used for memory and embeddings, this is optional. 
1. Speech Services: Deploy this resource if you want to use to speech-to-text for input, this is optional. 

## What resources are deployed?
Below is a list of the key resources created within the Resource Group when you deploy Semantic Kernel to Azure as a web app service.
1. Azure Web App Service : hosts back-end services
1. Application Insights : application logs and debugging 
1. Azure Cosmos DB : used for chat storage (optional)
1. Qdrant VectorDB in Azure: used for embedding & memory storage (optional)
1. Azure Speech Services : used for speech to text (optional)

## Verifying the deployment

To make sure your web app service is running, go to <!-- markdown-link-check-disable -->https://YOUR_INSTANCE_NAME.azurewebsites.net/probe<!-- markdown-link-check-enable-->

To get your instance's URL, click on the "Go to resource group" button you see at the end of your deployment. Then click on the resource whose name starts with "app-".

This will bring you to the Overview page on your web service. Your instance's URL is the value that appears next to the "Default domain" field.

## Changing your configuration, monitoring your deployment and troubleshooting

After your deployement is complete, in Azure Portal, you can change your configuration by clicking on the "Configuration" item in the "Settings" section of the left pane found in the main Resource Group page.

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


## How to clean up resources
To save costs you may want to clean up resources from this deployment. You can do this by deleting the resource group created via Azure portal or run the following in [Azure CLI](/cli/azure/):
```powershell
az group delete --name YOUR_RESOURCE_GROUP
```

## Take the next step
>Learn how to make changes to your web app, such as adding new skills. To learn more click [here](./publish-changes-to-azure.md).

>If you have not already done so please star the GitHub repo and join the Semantic Kernel community! 
[Star the Semantic Kernel repo](https://aka.ms/sk/repo)

[!INCLUDE [footer.md](../includes/footer.md)]
