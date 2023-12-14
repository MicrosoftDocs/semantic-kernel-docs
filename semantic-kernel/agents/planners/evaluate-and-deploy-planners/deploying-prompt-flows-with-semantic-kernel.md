---
title: Deploy Prompt flows with Semantic Kernel to Azure
description: Learn how to add Semantic Kernel to your Prompt flow runtime for deployments.
author: matthewbolanos
ms.topic: tutorial
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Deploy Prompt flows with Semantic Kernel to Azure AI Studio


Once you've created a Prompt flow, you can deploy it to Azure ML. This has several benefits:
> [!div class="checklist"]
> * Access to an endpoint that can be called from anywhere
> * Active evaluations of your Prompt flow while running in production
> * Ability to share your Prompt flow with others (including non-developers)

If you would like to deploy your Prompt flow to another service, you can refer to the [open source deployment guide](https://microsoft.github.io/promptflow/how-to-guides/deploy-a-flow/index.html) for Prompt flow.

## Prerequisites
Before beginning, you must first have an Azure AI studio project. If you do not have one, you can create one by following the [Azure AI studio guide](/azure/ai-studio/how-to/create-projects).

## Adding your Prompt flow to Azure AI Studio
Adding your Prompt flow to Azure AI Studio is a simple process. You can either upload your Prompt flow to your workspace or you can create a new Prompt flow and copy and paste your Prompt flow's JSON into the editor. Afterwards, your Prompt flow will be editable in both Azure AI Studio and VS Code for Web. You can even version your Prompt flow using Git with the VS Code for Web integration.

In the following sections, we will walk through the process of adding your Prompt flow to Azure AI Studio using the upload method.

To learn more about Prompt flow in Azure AI Studio, refer to the [Prompt flow in Azure AI Studio guide](/azure/ai-studio/how-to/prompt-flow).

### Upload your Prompt flow to your workspace
To add your Prompt flow to Azure AI Studio, complete the following steps:
1. Open your AI Studio project.
2. Select the **Prompt flow** navigation item on the left.
3. Select the **Create** button to add a new Prompt flow.
    :::image type="content" source="../../../media/create-flow-from-ai-studio.png" alt-text="Create Prompt flow":::

4. Scroll to the bottom of the new Prompt flow panel and select **Import from local**.
5. Select the folder containing your Prompt flow by selecting the **Browse** button.
6. Give your Prompt flow a name and configure it's type.
7. Select **Import**.
    :::image type="content" source="../../../media/import-flow-into-ai-studio.png" alt-text="Import Prompt flow":::

### Update any broken connections
If you have any broken connections in your Prompt flow, you will need to update them. To do so, complete the following steps:
1. Scroll to any nodes with broken connections (e.g., the `math_planner`` node)
2. Select the broken connection and swap it with your preferred connection with the dropdown.
    :::image type="content" source="../../../media/fix-broken-connector-in-ai-studio.png" alt-text="Update broken connections":::

## Running your Prompt flow
Now that you've added your Prompt flow to Azure AI Studio, you can start running it in the cloud. In the following sections, we'll walk through the process of creating a runtime, testing your Prompt flow, and deploying it to an endpoint.

### Create a runtime
If you don't already have a runtime for your Prompt flow, complete the following steps:
1. Select the **+** button next to the **Runtime** dropdown.
2. Give your runtime a name.
3. Select your compute instance; if you don't have one, you can create one by selecting the **Create Azure ML compute instance** link.
4. Select **Use default environment**. The default environment comes pre-installed with the Semantic Kernel package.
5. Select **Create**.
    :::image type="content" source="../../../media/create-runtime-in-ai-studio.png" alt-text="Create runtime":::

### Test your Prompt flow
Once you've created a runtime, you can test your Prompt flow. To do so, complete the following steps:
1. Select your newly created runtime from the **Runtime** dropdown.
2. Select the **Run** button.
    :::image type="content" source="../../../media/run-from-ai-studio.png" alt-text="Run Prompt flow":::
3. Select **View outputs** to view the outputs of your Prompt flow.

Once your flow is in AI Studio, you can also [run batch runs](/azure/ai-studio/how-to/flow-bulk-test-evaluation) and [evaluations](/azure/ai-studio/how-to/flow-develop-evaluation).

### Deploy your Prompt flow
1. Select the **Deploy** button.
2. Define your basic settings.
3. Select **Review + Create**.
4. Review your deployment and select **Create**.

You can now access your Prompt flow's deployment endpoint by selecting the **Deployments** navigation item on the left and selecting your deployment.

To use your deployment, navigate to the **Consume** tab. From there, you can get sample code in a variety of languages to call your Prompt flow's endpoint.

To learn more about deploying Prompt flows refer to the [Deploy a flow for real-time inference](/azure/ai-studio/how-to/flow-deploy?tabs=azure-studio) article.

