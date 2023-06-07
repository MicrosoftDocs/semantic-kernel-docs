---
title: Use your AI APIs with Azure API Management
description: How to use Azure API Management to protect your OpenAI and Azure OpenAI API keys.
author: matthewbolanos
ms.topic: Azure
ms.author: mabolanos
ms.date: 05/19/2023
ms.service: mssearch
---

# Protect your Azure OpenAI API keys with Azure API Management

[!INCLUDE [subheader.md](../includes/pat_large.md)]

With [Azure API Management](/azure/api-management/api-management-key-concepts), you can protect your AI API keys and manage access to your AI APIs. This is helpful if you want to give your users or developers access to  Azure OpenAPI APIs without giving them direct access to your keys or to manage access to your AI APIs directly from Azure.

By the end of this article, you'll have a working API Management instance along with sample code that shows you how to use Azure API Management with Semantic Kernel.

## Prerequisites
To complete this tutorial, you'll first need access to the following:
- Azure OpenAI API key
- API Management instance

To create a new API Management instance, see [Create an API Management instance](/azure/api-management/get-started-create-service-instance). You can use the default configuration when creating your instance.

## Setup Azure API Management instance with Azure OpenAI API
The following steps describe how you can setup your Azure OpenAI API with Azure API Management.

1. Start by [downloading the API definition for Azure OpenAI](https://raw.githubusercontent.com/Azure/azure-rest-api-specs/main/specification/cognitiveservices/data-plane/AzureOpenAI/inference/preview/2023-03-15-preview/inference.json) and saving it to your local machine.

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

3. You can now import the file from step 2 into Azure API Management. The full process is described in the [import and publish](/azure/api-management/import-and-publish) article.

4. Now create a named value for your Azure OpenAI API key.

    This will allow you to better protect your Azure OpenAI API key. To create a named value, see [using named values in Azure API Management policies](/azure/api-management/api-management-howto-properties). Take note of the **Display name** you give your named value as you'll need it in the next step.

    :::image type="content" source="../media/azure-named-key.png" alt-text="Named value for Azure OpenAI key":::

5. Finally, [edit the inbound policy](/azure/api-management/set-edit-policies) so your API adds the `api-key` header to your request.
    1. Navigate to your API Management instance and select **APIs** from the left-hand menu.
    2. Select your API from the list of APIs.
    3. Select the **Design** tab.
    4. Select **Add policy** within the Inbound processing section.
    5. Select **Set headers**.
    6. In the **Set headers** page, enter `openai-key` for the **Name** field
    7. For the **Value** field, take the display name of your named value in the previous step and wrap it in double curly braces. For example, if the display name of your named value was `azure-openai-key`, you would enter `{{azure-openai-key}}`.
    8. Leave the action as `append`.
        :::image type="content" source="../media/api-management-define-header.png" alt-text="Set header in inbound processing":::
    7. Select **Save**.

    You can also use key vault secrets to protect your Auzre OpenAI API key even more. To learn more, see [using named values in Azure API Management policies](/azure/api-management/api-management-howto-properties). 

You've now created and initially setup your Azure OpenAI API with Azure API Management.  You can now test your API by selecting the **Test** tab in Azure API Management. For more information, see [Test an API](/azure/api-management/import-and-publish#test-the-new-api-in-the-azure-portal). Next, we'll configure authentication for your new API so only authorized users can access it.
    

## Configure authentication for your new API
By default, your new Azure API Management service uses subscriptions to control access to your APIs, but this would be functionally equivalent to giving your users direct access to your API keys. So instead, we'll demonstrate how to setup OAuth 2.0 to control access to your Azure OpenAI APIs.

2. Configure OAuth 2.0 by folloing the steps in the [protect an API in Azure API Management using OAuth 2.0 authorization with Azure Active Directory](/azure/api-management/api-management-howto-protect-backend-with-aad#register-an-application-in-azure-ad-to-represent-the-api) article. Once you're done with these steps you'll have an Azure AD application with the necessary scopes.

3. Next, [configure a JWT validation policy to pre-authorize requests](/azure/api-management/api-management-howto-protect-backend-with-aad#configure-a-jwt-validation-policy-to-pre-authorize-requests). This will ensure that only users with the correct permissions can access your API and underlying Azure OpenAI API. 

1. Lastly, turn off **Subscription required** for your API.

    Navigate to your API Management instance and select **APIs** from the left-hand menu. Then select your API from the list of APIs. In the **Settings** tab, find the **Subscription required** checkbox and uncheck it. Only complete this step after configuring OAuth 2.0 and JWT validation to ensure that only authorized users can access your API.

    :::image type="content" source="../media/api-management-turn-off-subscription.png" alt-text="Turn off subscription required":::


Congrats, you've now configured authentication for your Azure OpenAI API with Azure API Management. You can now provide users with access to your API by creating users in Azure AD and assigning them the correct permissions. For more information, see [Assign a user or group to an enterprise app in Azure Active Directory](/azure/active-directory/manage-apps/assign-user-or-group-access-portal).

## Access your API from Semantic Kernel

Once users have the correct permissions, they can access your API from within Semantic Kernel. There are a few steps to complete to test this connection within a console application.

> [!IMPORTANT]
> The following code is for illustrative purposes. 

1. First, create a new class that implements the `TokenCredential` class. This class will be used to provide Semantic Kernel with the user authentication token for your service.

    ```csharp
    using Azure.Core;

    public class BearerTokenCredential : TokenCredential
    {
        private readonly AccessToken _accessToken;

        // Constructor that takes a Bearer token string and its expiration date
        public BearerTokenCredential(AccessToken accessToken)
        {
            _accessToken = accessToken;
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return _accessToken;
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return new ValueTask<AccessToken>(_accessToken);
        }
    }
    ```

2. Next, within your console application, you'll create an interactive logon to get the user's token.

    ```csharp
    string[] scopes = new string[] { "https://cognitiveservices.azure.com/.default" };
    var credential = new InteractiveBrowserCredential();
    var requestContext = new TokenRequestContext(scopes);
    var accessToken = await credential.GetTokenAsync(requestContext);
    ```

3. Finally, you can create a new instance of Kernel and pass in the `BearerTokenCredential` class you created in step 1 along with the access token you retrieved in step 2.

    ```csharp
    IKernel kernel = new KernelBuilder()
    .WithAzureTextCompletionService(
        "text-davinci-003",
        "https://apim...api.net/",
        new BearerTokenCredential(accessToken)
    )
    .Build();
    ```