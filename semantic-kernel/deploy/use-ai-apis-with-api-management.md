---
title: Use your AI APIs with Azure API Management
description: How to use Azure API Management to protect your OpenAI and Azure OpenAI API keys.
author: matthewbolanos
ms.topic: Azure
ms.author: mabolanos
ms.date: 05/19/2023
ms.service: mssearch
---

# Protect your AI API keys with Azure API Management

[!INCLUDE [subheader.md](../includes/pat_large.md)]

With [Azure API Management](https://learn.microsoft.com/en-us/azure/api-management/api-management-key-concepts), you can protect your AI API keys and manage access to your AI APIs. This is helpful if you want to give your users or developers access to OpenAI or Azure OpenAPI APIs without giving them direct access to your keys or to manage access to your AI APIs directly from Azure.

By the end of this article, you'll have a working API Management instance along with sample code that shows you how to use Azure API Management with Semantic Kernel.

## Prerequisites
To complete this tutorial, you'll first need access to the following:
- Azure OpenAI API key or OpenAI API key
- API Management instance

To create a new API Management instance, see [Create an API Management instance](/azure/api-management/get-started-create-service-instance). You can use the default configuration when creating your instance.

## Setup Azure API Management instance with Azure OpenAI API
The following steps describe how you can setup your Azure OpenAI API with Azure API Management. You can follow a similar process for OpenAI APIs.

1. Start by [downloading the API definition for Azure OpenAI](https://raw.githubusercontent.com/Azure/azure-rest-api-specs/main/specification/cognitiveservices/data-plane/AzureOpenAI/inference/preview/2023-03-15-preview/inference.json) and saving them to your local machine.

    > [!div class="nextstepaction"]
    > [Download API reference](https://raw.githubusercontent.com/Azure/azure-rest-api-specs/main/specification/cognitiveservices/data-plane/AzureOpenAI/inference/preview/2023-03-15-preview/inference.json)

2. Open the file in a text editor and change the `servers` property so that the `url` and `endpoint` properties point to your API endpoint.

    For example, if your API endpoint is `https://contoso.openai.azure.com`, you would change the `servers` property to the following:

    ```json
    "servers": [
        {
        "url": "https://contoso.openai.azure.com/openai",
        "variables": {
            "endpoint": {
            "default": "contoso.openai.azure.com"
            }
        }
        }
    ],
    ```

3. You can now import the file from step 2 into Azure API Management.

    The full process is described in [Import and publish](/azure/api-management/import-and-publish) but the following steps will get you started:
    1. In the Azure portal, navigate to your API Management instance.
    2. Select **APIs** from the left-hand menu.
    3. Select **OpenAPI** tile.
    4. In the Create from OpenAPI specification window, select Full.
    5. Upload the API definition file you edited in step 2 by selecting **Select a file**.
    6. Populate the required **Title** and **Description** fields.
    7. Select **Create**.

4. Finally, [edit the inbound policy](/azure/api-management/set-edit-policies) so your API adds the `api-key` header to your request.

    To do this, follow these steps:
    1. Navigate to your API Management instance and select **APIs** from the left-hand menu.
    2. Select your API from the list of APIs.
    3. Select the **Design** tab.
    4. Select **Add policy** within the Inbound processing section.
    5. Select **Set headers**.
    6. In the **Set headers** page, enter `openai-key` for the **Name** field and your Azure OpenAI API key for the **Value** field; leave the action as `append`.
        :::image type="content" source="../media/api-management-define-header.png" alt-text="Set header in inbound processing":::
    7. Select **Save**.

    You can also use named values and key vault secrets to protect your Auzre OpenAI API key even more. To learn more, see [using named values in Azure API Management policies](azure/api-management/api-management-howto-properties). 

You've now created and initially setup your Azure OpenAI API with Azure API Management.  You can now test your API by selecting the **Test** tab in Azure API Management. For more information, see [Test an API](/azure/api-management/import-and-publish#test-the-new-api-in-the-azure-portal). Next, we'll configure authentication for your new API so only authorized users can access it.
    

## Configure authentication for your new API
By default, your new Azure API Management service uses subscriptions to control access to your APIs, but this would be functionally equivalent to giving your users direct access to your API keys. So instead, we'll demonstrate how to setup OAuth 2.0 to control access to your Azure OpenAI APIs.

1. First, turn off **Subscription required** for your API.

    To do this, navigate to your API Management instance and select **APIs** from the left-hand menu. Then select your API from the list of APIs. In the **Settings** tab, find the **Subscription required** checkbox and uncheck it.

    :::image type="content" source="../media/api-management-turn-off-subscription.png" alt-text="Turn off subscription required":::

2. Next, configure OAuth 2.0.

    To do this, follow the steps in [Protect an API in Azure API Management using OAuth 2.0 authorization with Azure Active Directory](/azure/api-management/api-management-howto-protect-backend-with-aad#register-an-application-in-azure-ad-to-represent-the-api). Once you're done with these steps you'll have an Azure AD application with the necessary scopes.

3. Lastly, [configure a JWT validation policy to pre-authorize requests](/azure/api-management/api-management-howto-protect-backend-with-aad#configure-a-jwt-validation-policy-to-pre-authorize-requests).
    This will ensure that only users with the correct permissions can access your API and underlying Azure OpenAI API. 

Congrats, you've now configured authentication for your Azure OpenAI API with Azure API Management. You can now provide users with access to your API by creating users in Azure AD and assigning them the correct permissions. For more information, see [Assign a user or group to an enterprise app in Azure Active Directory](/azure/active-directory/manage-apps/assign-user-or-group-access-portal).

## Access your API from Semantic Kernel

Once users have the correct permissions, they can access your API by using the ??? connector in Semantic Kernel.