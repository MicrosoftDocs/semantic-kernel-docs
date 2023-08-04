---
title: Get started using Chat Copilot locally
description: Run Chat Copilot locally to see how it works.
author: matthewbolanos, molliemunoz
ms.topic: samples
ms.author: mabolan, momuno
ms.date: 08/03/2023
ms.service: semantic-kernel
---
# Getting started with Chat Copilot

[!INCLUDE [subheader.md](../includes/pat_large.md)]

Chat Copilot is an AI chat application sample created for educational purposes. It is built on Microsoft Semantic Kernel and has two components:
- A frontend [React web app](https://github.com/microsoft/chat-copilot/tree/main/webapp) that provides a user interface for interacting with the Semantic Kernel.
- A backend [.NET web service](https://github.com/microsoft/chat-copilot/tree/main/webapi) that provides an API for the React web app to interact with the Semantic Kernel.

In this article, we'll walk through the steps to run these two components locally on your machine. The [Chat Copilot reference app](https://github.com/microsoft/chat-copilot/blob/main/README.md) is located in the Chat Copilot GitHub repository.

> [!IMPORTANT]
> Each chat interaction will call Azure OpenAI/OpenAI which will use tokens that you may be billed for.

## Requirements to run this app

**Environment:**
> [!div class="checklist"]
> * [Git](https://git-scm.com/book/v2/Getting-Started-Installing-Git)
> * [Azure account](https://azure.microsoft.com/free)
> * Installed by the Setup script:
    > * [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) 
    > * [Node.js](https://nodejs.org/en/download)
    > * [Yarn](https://classic.yarnpkg.com/docs/install)

**Frontend Web Application:**
> [!div class="checklist]
> * [Azure AD Tenant](/azure/active-directory/develop/quickstart-create-new-tenant)
> * [Registered application](/azure/active-directory/develop/quickstart-register-app#register-an-application)
    > * Under `Supported account types`: Select "_Accounts in any organizational directory (Any Azure AD directory - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)_" 
    > * Under `Redirect URI (optional)`: Select `Single-page application (SPA)` and set the URI to `http://localhost:3000`.
> * [Application (client) ID](/azure/active-directory/develop/quickstart-register-app#register-an-application)

**Backend AI Service (select one):**

| AI Service   | Item                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
| ------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Azure OpenAI | > [!div class="checklist] > * [Access](https://aka.ms/oai/access) > * [Resource](/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#create-a-resource) > * [Deployed models](/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#deploy-a-model) (`gpt-35-turbo` and `text-embedding-ada-002`) > * [Endpoint](/azure/ai-services/openai/tutorials/embeddings?tabs=command-line#retrieve-key-and-endpoint) > * [API key](/azure/ai-services/openai/tutorials/embeddings?tabs=command-line#retrieve-key-and-endpoint) |
| OpenAI       | > [!div class="checklist] > * [Account](https://platform.openai.com) > * [API key](https://platform.openai.com/account/api-keys)                    


## Instructions

1) Install .NET 7.0 SDK, Node.js, and Yarn on your machine.

    # [Windows](#tab/Windows)
    Open a PowerShell terminal as an administrator and navigate to the _\scripts_ directory in the Chat Copilot project.

    ```powershell
    cd .\scripts\
    ```
    
    Next, run the following command to install the required dependencies:
    ```powershell
    .\Install.ps1
    ```

    > [!NOTE]
    > This script will also install `Chocolatey`.

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

2) Configure Chat Copilot.

    # [PowerShell](#tab/Powershell)

    Replace the values in brackets below before running the command:

    ```powershell
    .\Configure.ps1 -AIService {AI_SERVICE} -APIKey {API_KEY} -Endpoint {AZURE_OPENAI_ENDPOINT} -ClientId {AZURE_APPLICATION_ID} 
    ```

    - `AI_SERVICE`: `AzureOpenAI` or `OpenAI`.
    - `API_KEY`: The `API key` for Azure OpenAI or for OpenAI.
    - `AZURE_OPENAI_ENDPOINT`: The Azure OpenAI resource `Endpoint` address. Omit `-Endpoint` if using OpenAI.
    - `AZURE_APPLICATION_ID`: The `Application (client) ID` associated with the registered application.

    > [!IMPORTANT]
    > If you deployed models `gpt-35-turbo` and `text-embedding-ada-002` with custom names (instead of each own's given name), also use the parameters:

    ```powershell
    -CompletionModel {DEPLOYMENT_NAME} -EmbeddingModel {DEPLOYMENT_NAME} -PlannerModel {DEPLOYMENT_NAME}
    ```

    > [!TIP]
    > To set a specific Tenant Id for the web application, use the optional parameter:

    ```powershell
    -TenantId {TENANT_ID}
    ```

    # [Bash](#tab/Bash)
    First, ensure the configuration script is executable:
    ```bash
    # Ensure the configure scripts are executable
    chmod +x Configure.sh
    ```
    
    Replace the values in brackets below before running the command:

    ```bash
    ./Configure.sh --aiservice {AI_SERVICE} --apikey {API_KEY} --endpoint {AZURE_OPENAI_ENDPOINT} --clientid {AZURE_APPLICATION_ID} 
    ```

    - `AI_SERVICE`: `AzureOpenAI` or `OpenAI`.
    - `API_KEY`: The `API key` for Azure OpenAI or for OpenAI.
    - `AZURE_OPENAI_ENDPOINT`: The Azure OpenAI resource `Endpoint` address. Omit `--endpoint` if using OpenAI.
    - `AZURE_APPLICATION_ID`: The `Application (client) ID` associated with the registered application.

    > [!IMPORTANT]
    > If you deployed models `gpt-35-turbo` and `text-embedding-ada-002` with custom names (instead of each own's given name), also use the parameters:

    ```bash
    --completionmodel {DEPLOYMENT_NAME} --embeddingmodel {DEPLOYMENT_NAME} --plannermodel {DEPLOYMENT_NAME}
    ```

    > [!TIP] 
    > To set a specific Tenant Id, use the optional parameter:

    ```bash
    --tenantid {TENANT_ID}
    ```
    ---

3) Run the start script.
    
    # [PowerShell](#tab/Powershell)

    ```powershell
    .\Start.ps1
    ```

    > [!TIP]
    > Confirm pop-ups are not bocked and you are logged in with the same account used to register the application. 

    > [!NOTE]
    > It may take a few minutes for Yarn packages to install on the first run.

    # [Bash](#tab/Bash)

    ```bash
    # Ensure the start scripts are executable
    chmod +x Start.sh
    chmod +x Start-Backend.sh
    chmod +x Start-Frontend.sh

    # Start CopilotChat 
    ./Start.sh
    ```

    > [!TIP]
    > Confirm pop-ups are not bocked and you are logged in with the same account used to register the application. 

    > [!NOTE]
    > It may take a few minutes for Yarn packages to install on the first run.
    ---
4) Congrats! A browser should automatically launch and navigate to _https://localhost:3000_ with the sample app running.

## Next step

Now that you've gotten Chat Copilot running locally, you can learn how to customize it to your needs.

> [!div class="nextstepaction"]
> [Customize Chat Copilot](./customizing-chat-copilot.md)
