---
title: Get started using Chat Copilot locally
description: Run Chat Copilot locally to see how it works.
author: matthewbolanos
ms.topic: samples
ms.author: mabolan
ms.date: 04/07/2023
ms.service: semantic-kernel
---
# Getting started with Chat Copilot

[!INCLUDE [subheader.md](../includes/pat_large.md)]

Chat Copilot consists of two components:
- A [React web app](https://github.com/microsoft/chat-copilot/tree/main/webapp) that provides a user interface for interacting with the Semantic Kernel.
- And a [.NET web service](https://github.com/microsoft/chat-copilot/tree/main/webapi) that provides an API for the React web app to interact with the Semantic Kernel.

In this article, we'll walk through the steps you need to take to run these two components locally on your machine.

## Requirements to run this app

> [!div class="checklist"]
> * [Visual Studio Code](https://code.visualstudio.com/Download)
> * [Git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)
> * [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
> * [Node.js](https://nodejs.org/en/download)
> * [Yarn](https://classic.yarnpkg.com/lang/en/docs/install)

## Running the app
The [Chat Copilot reference app](https://github.com/microsoft/chat-copilot/blob/main/README.md) is located in the Semantic Kernel GitHub repository.

1) To enable authentication, [register an Azure Application](/azure/active-directory/develop/quickstart-register-app). We recommend using the following properties:
    - Select __Single-page application (SPA)__ as platform type, and set the Web redirect URI to _http://localhost:3000_
    - Select __Accounts in any organizational directory and personal Microsoft Accounts__ as supported account types for this sample.
    
    > [!Note]
    > Make a note of the Application (client) ID from the Azure Portal; we will use it in step 4.

2) Install requirements. The following scripts will install yarn, node, and .NET SDK on your machine.

    # [Windows](#tab/Windows)
    Open a PowerShell terminal as an administrator and navigate to the _/scripts_ directory in the Semantic Kernel project.

    ```powershell
    cd ./scripts
    ```
    
    Next, run the following command to install the required dependencies:
    ```powershell
    ./Install.ps1
    ```

    # [Ubuntu/Debian Linux](#tab/Linux)
    Open a bash terminal as an administrator and navigate to the _/scripts_ directory in the Semantic Kernel project:
    ```bash
    cd ./scripts

    # Ensure the install scripts are executable
    chmod +x Install-apt.sh
    ```

    Next, run the following command to install the required dependencies:
    ```bash
    ./Install-apt.sh
    ```
    # [MacOS](#tab/macos)

    Open a bash terminal as an administrator and navigate to the _/scripts_ directory in the Semantic Kernel project:
    ```bash
    cd ./scripts

    # Ensure the install scripts are executable
    chmod +x Install-brew.sh
    ```

    Next, run the following command to install the required dependencies. The MacOS install script uses [Homebrew](https://brew.sh/) to install dependencies:
    ```bash
    ./Install-brew.sh
    ```
    ---


3) Run the configuration script

    # [PowerShell](#tab/Powershell)
    If you are using Azure OpenAI, run the following command. Replace the `{AZURE_OPENAI_ENDPOINT}`, `{AZURE_OPENAI_API_KEY}`, and `{APPLICATION_CLIENT_ID}` values in the following command before running it:

    ```powershell
    ./Configure.ps1 -AIService AzureOpenAi -Endpoint {AZURE_OPENAI_ENDPOINT} -ApiKey {AZURE_OPENAI_API_KEY} -ClientId {APPLICATION_CLIENT_ID}
    ```

    If you are using OpenAI, run the following command. Replace the  `{OPENAI_API_KEY}` and `{APPLICATION_CLIENT_ID}` values in the following command before running it:

    ```powershell
    ./Configure.ps1 -AIService OpenAi -ApiKey {OPENAI_API_KEY} -ClientId {APPLICATION_CLIENT_ID}
    ```

    # [Bash](#tab/Bash)
    First, ensure the configuration script is executable:
    ```bash
    # Ensure the configure scripts are executable
    chmod +x Configure.sh
    ```
    
    If you are using Azure OpenAI, run the following command. Replace the `{AZURE_OPENAI_ENDPOINT}`, `{AZURE_OPENAI_API_KEY}`, and `{APPLICATION_CLIENT_ID}` values in the following command before running it:

    ```bash
    ./Configure.sh --aiservice azureopenai --endpoint {AZURE_OPENAI_ENDPOINT} --apikey {AZURE_OPENAI_API_KEY} --clientid {APPLICATION_CLIENT_ID}
    ```

     If you are using OpenAI, run the following command. Replace the `{OPENAI_API_KEY}` and `{APPLICATION_CLIENT_ID}` values in the following command before running it:

    ```bash
    ./Configure.sh --aiservice openai --apikey {OPENAI_API_KEY} --clientid {APPLICATION_CLIENT_ID}
    ```
    ---

4) Run the start script
    
    # [PowerShell](#tab/Powershell)

    ```powershell
    setx ASPNETCORE_ENVIRONMENT "Development"

    ./Start.ps1
    ```

    # [Bash](#tab/Bash)

    ```bash
    export ASPNETCORE_ENVIRONMENT=Development

    # Ensure the start scripts are executable
    chmod +x Start.sh
    chmod +x Start-Backend.sh
    chmod +x Start-Frontend.sh

    # Start CopilotChat 
    ./Start.sh
    ```
    ---
5) Congrats! A browser should automatically launch and navigate to _https://localhost:3000_ with the sample app running.

## Next step

Now that you've gotten Chat Copilot running locally, you can now learn how to customize it to your needs.

> [!div class="nextstepaction"]
> [Customize Chat Copilot](./customizing-chat-copilot.md)
