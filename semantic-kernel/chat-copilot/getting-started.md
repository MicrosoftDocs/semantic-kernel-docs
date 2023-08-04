---
title: Get started using Chat Copilot locally
description: Run Chat Copilot locally to see how it works.
author: matthewbolanos & molliemunoz
ms.topic: samples
ms.author: mabolan & momuno
ms.date: 08/03/2023
ms.service: semantic-kernel
---
# Getting started with Chat Copilot

[!INCLUDE [subheader.md](../includes/pat_large.md)]

Chat Copilot consists of two components:
- A [React web app](https://github.com/microsoft/chat-copilot/tree/main/webapp) that provides a user interface for interacting with the Semantic Kernel.
- And a [.NET web service](https://github.com/microsoft/chat-copilot/tree/main/webapi) that provides an API for the React web app to interact with the Semantic Kernel.

In this article, we'll walk through the steps you need to take to run these two components locally on your machine. The [Chat Copilot reference app](https://github.com/microsoft/chat-copilot/blob/main/README.md) is located in the Chat Copilot GitHub repository.


## Requirements to run this app

**Environment:**
> [!div class="checklist"]
> * [Visual Studio Code](https://code.visualstudio.com/Download)
> * [Git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)
> * [Azure account](https://azure.microsoft.com/free)
> * [.NET 7.0](https://dotnet.microsoft.com/download/dotnet/7.0) - Installed by script below
> * [Node.js](https://nodejs.org/en/download) - Installed by script below
> * [Yarn](https://classic.yarnpkg.com/lang/docs/install) - Installed by script below

**AI Service (select one):**

Azure OpenAI: 
> [!div class="checklist"]
> * [Access](https://aka.ms/oai/access)
> * [Resource](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#create-a-resource)
> * [Deployed models](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#deploy-a-model) (`gpt-35-turbo` and `text-embedding-ada-002`)
> * [Endpoint](https://learn.microsoft.com/azure/ai-services/openai/tutorials/embeddings?tabs=command-line#retrieve-key-and-endpoint) (e.g., `http://contoso.openai.azure.com`)
> * [API key](https://learn.microsoft.com/azure/ai-services/openai/tutorials/embeddings?tabs=command-line#retrieve-key-and-endpoint)

OpenAI:
> [!div class="checklist"]
> * [Account](https://platform.openai.com)
> * [API key](https://platform.openai.com/account/api-keys)                           

**Web Application:**
> [!div class="checklist]
> * [Azure AD Tenant](https://learn.microsoft.com/azure/active-directory/develop/quickstart-create-new-tenant)
> * [Registered application](https://learn.microsoft.com/azure/active-directory/develop/quickstart-register-app#register-an-application) - See [Registering your web application](#registering-your-web-application)
> * [Application (client) ID](https://learn.microsoft.com/azure/active-directory/develop/quickstart-register-app#register-an-application) - See [Registering your web application](#registering-your-web-application)

## Registering your web application
When [registering your application](https://learn.microsoft.com/azure/active-directory/develop/quickstart-register-app#register-an-application), we recommend using the following properties:

- Under `Supported account types`: Select "_Accounts in any organizational directory (Any Azure AD directory - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)_" 
- Under `Redirect URI (optional)`: Select `Single-page application (SPA)` and set the URI to `http://localhost:3000`.

> [!Note]
> Make note of the `Application (client) ID` from the Azure Portal; we will use it to configure the sample.

## Running the app

1) Install Yarn, Node.js, and .NET 7.0 SDK on your machine.

    # [Windows](#tab/Windows)
    Open a PowerShell terminal as an administrator and navigate to the _\scripts_ directory in the Chat Copilot project.

    ```powershell
    cd .\scripts\
    ```
    
    Next, run the following command to install the required dependencies:
    ```powershell
    .\Install.ps1
    ```

    # [Debian/Ubuntu Linux](#tab/Linux)
    Open a Bash terminal as an administrator and navigate to the _/scripts_ directory in the Chat Copilot project:
    ```bash
    cd ./scripts/

    # Ensure the install scripts are executable
    chmod +x Install-apt.sh
    ```

    Next, run the following command to install the required dependencies:
    ```bash
    ./Install-apt.sh
    ```
    # [macOS](#tab/macos)

    Open a Bash terminal as an administrator and navigate to the _/scripts_ directory in the Semantic Kernel project:
    ```bash
    cd ./scripts/

    # Ensure the install scripts are executable
    chmod +x Install-brew.sh
    ```

    Next, run the following command to install the required dependencies. The MacOS install script uses [Homebrew](https://brew.sh/) to install dependencies:
    ```bash
    ./Install-brew.sh
    ```
    ---

2) Run the configuration script.

    # [PowerShell](#tab/Powershell)

    If using Azure OpenAI, replace the values in brackets below before running the command:

    ```powershell
    .\Configure.ps1 -AIService AzureOpenAI -APIKey {API_KEY} -Endpoint {AZURE_OPENAI_ENDPOINT} -ClientId {AZURE_APPLICATION_ID} 
    ```

    - `API_KEY`: The `API key` for Azure OpenAI.
    - `AZURE_OPENAI_ENDPOINT`: The Azure OpenAI resource `Endpoint` address.
    - `AZURE_APPLICATION_ID`: The `Application (client) ID` associated with the registered application.

    > [!Important] If you deployed models `gpt-35-turbo` and `text-embedding-ada-002` with custom names (instead of each own's given name), also use the parameters:

    ```powershell
    -CompletionModel {DEPLOYMENT_NAME} -EmbeddingModel {DEPLOYMENT_NAME} -PlannerModel {DEPLOYMENT_NAME}
    ```

    If using OpenAI, replace the values in brackets below before running the command:

    ```powershell
    .\Configure.ps1 -AIService OpenAI -APIKey {API_KEY} -ClientId {AZURE_APPLICATION_ID} 
    ```

    - `API_KEY`: The `API key` for OpenAI.
    - `AZURE_APPLICATION_ID`: The `Application (client) ID` associated with the registered application.

    > [!Optional]: To set a specific Tenant Id for the web application, use the parameter:

        ```powershell
        -TenantId {TENANT_ID}
        ```

    # [Bash](#tab/Bash)
    First, ensure the configuration script is executable:
    ```bash
    # Ensure the configure scripts are executable
    chmod +x Configure.sh
    ```
    
    If you are using Azure OpenAI, replace the values in brackets below before running the command:

    ```bash
    ./Configure.sh --aiservce AzureOpenAI --apikey {API_KEY} --endpoint {AZURE_OPENAI_ENDPOINT} --clientid {AZURE_APPLICATION_ID}
    ```
    
    - `API_KEY`: The `API key` for Azure OpenAI.
    - `AZURE_OPENAI_ENDPOINT`: The Azure OpenAI resource `Endpoint` address.
    - `AZURE_APPLICATION_ID`: The `Application (client) ID` associated with the registered application.
  
    > [!Important] If you deployed models `gpt-35-turbo` and `text-embedding-ada-002` with custom names (instead of each own's given name), also use the parameters:

    ```bash
    --completionmodel {DEPLOYMENT_NAME} --embeddingmodel {DEPLOYMENT_NAME} --plannermodel {DEPLOYMENT_NAME}
    ```

    If using OpenAI, replace the values in brackets below before running the command:

    ```bash
    ./Configure.sh --aiservice OpenAI --apikey {API_KEY} --clientid {AZURE_APPLICATION_ID}
    ```

    - `API_KEY`: The `API key` for OpenAI.
    - `AZURE_APPLICATION_ID`: The `Application (client) ID` associated with the registered application.
  
    > [!Optional] To set a specific Tenant Id, use the parameter:

    ```bash
    --tenantid {TENANT_ID}
    ```
    ---

3) Run the start script.
    
    Confirm pop-ups are not bocked and you are logged in with the same account used to register the application. It may take a few minutes for Yarn packages to install on the first run.
    
    # [PowerShell](#tab/Powershell)

    ```powershell
    .\Start.ps1
    ```

    # [Bash](#tab/Bash)

    ```bash
    # Ensure the start scripts are executable
    chmod +x Start.sh
    chmod +x Start-Backend.sh
    chmod +x Start-Frontend.sh

    # Start CopilotChat 
    ./Start.sh
    ```
    ---
4) Congrats! A browser should automatically launch and navigate to _https://localhost:3000_ with the sample app running.

## Next step

Now that you've gotten Chat Copilot running locally, you can learn how to customize it to your needs.

> [!div class="nextstepaction"]
> [Customize Chat Copilot](./customizing-chat-copilot.md)
