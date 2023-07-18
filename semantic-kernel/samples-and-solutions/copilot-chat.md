---
title: Get started using Chat Copilot
description: Learn how to use the Chat Copilot reference app to build your own integrated large language model chatbot.
author: smonroe
ms.topic: samples
ms.author: smonroe
ms.date: 04/07/2023
ms.service: mssearch
---
# Chat Copilot

[!INCLUDE [subheader.md](../includes/pat_large.md)]

The Chat Copilot reference app allows you to build your own integrated large language model chatbot.  This is an enriched intelligence app, with multiple dynamic components including command messages, user intent, and memories.  

The chat prompt and response will evolve as the conversation between the user and the application proceeds.  This chat experience uses a chat plugin containing multiple functions that work together to construct the final prompt for each exchange.


> [!IMPORTANT]
> Each function will call OpenAI which will use tokens that you will be billed for. 


## Requirements to run this app

> [!div class="checklist"]
> * [Visual Studio Code](https://code.visualstudio.com/Download)
> * [Git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)
> * [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
> * [Node.js](https://nodejs.org/en/download)
> * [Yarn](https://classic.yarnpkg.com/lang/en/docs/install)

## Running the app
The [Copilot Chat Sample App](https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/copilot-chat-app/README.md) is located in the Semantic Kernel GitHub repository.

1) Clone [Semantic Kernel](https://github.com/microsoft/semantic-kernel) locally.
2) To enable authentication, [register an Azure Application](/azure/active-directory/develop/quickstart-register-app). We recommend using the following properties:
    - Select Single-page application (SPA) as platform type, and set the Web redirect URI to http://localhost:3000
    - Select Accounts in any organizational directory and personal Microsoft Accounts as supported account types for this sample.
    
    > [!Note]
    > Make a note of the Application (client) ID from the Azure Portal, we will use it in step 4.

3) Install requirements

    # [Windows](#tab/Windows)

    ```powershell
    ./Install-Requirements.ps1
    ```

    # [Ubuntu/Debian Linux](#tab/Linux)

    ```bash
    ./Install-Requirements-UbuntuDebian.ps1
    ```
    # [Other](#tab/other)

    For all other operating systems, ensure NET 6.0 SDK (or newer), Node.js 14 (or newer), and Yarn classic ([v1.22.19](https://classic.yarnpkg.com/)) package manager are installed before proceeding.

    ---


4) Run the configuration script

    # [PowerShell](#tab/Powershell)

    ```powershell
    cd /samples/apps/copilot-chat-app/scripts
    ```

    If you are using Azure OpenAI, replace the `{AZURE_OPENAI_ENDPOINT}`, `{AZURE_OPENAI_API_KEY}`, and `{APPLICATION_CLIENT_ID}` values in the following command before running it:

    ```powershell
    ./Configure.ps1 -AzureOpenAI -Endpoint {AZURE_OPENAI_ENDPOINT} -ApiKey {AZURE_OPENAI_API_KEY} -ClientId {APPLICATION_CLIENT_ID}
    ```

    If you are using OpenAI, replace the  `{AZURE_OPENAI_API_KEY}` and `{APPLICATION_CLIENT_ID}` values in the following command before running it:

    ```powershell
    ./Configure.ps1 -openai -ApiKey {AZURE_OPENAI_API_KEY} -ClientId {APPLICATION_CLIENT_ID}
    ```

    # [Bash](#tab/Bash)

    If you are using Azure OpenAI, replace the `{AZURE_OPENAI_ENDPOINT}`, `{AZURE_OPENAI_API_KEY}`, and `{APPLICATION_CLIENT_ID}` values in the following command before running it:

    ```bash
    cd samples/apps/copilot-chat-app/scripts

    # Ensure the start scripts are executable
    chmod +x Configure.sh
    ```

    ```bash
    ./Configure.sh --azureopenai --endpoint {AZURE_OPENAI_ENDPOINT} --apikey {AZURE_OPENAI_API_KEY} --clientid {APPLICATION_CLIENT_ID}
    ```

     If you are using Azure OpenAI, replace the `{AZURE_OPENAI_ENDPOINT}`, `{AZURE_OPENAI_API_KEY}`, and `{APPLICATION_CLIENT_ID}` values in the following command before running it:

    ```bash
    ./Configure.sh --openai --apikey {AZURE_OPENAI_API_KEY} --clientid {APPLICATION_CLIENT_ID}
    ```
    ---

5) Run the start script
    
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
6) Congrats! A browser should automatically launch and navigate to https://localhost:3000 with the sample app running

## Exploring the app
With the Copilot Chat sample app running, you can start interacting with the chatbot.  The app will start with a default conversation thread where you can ask it questions.

![Copilot Chat Sample App](../media/copilot-chat.png)

| Feature | Name | Description |
|:-|:-|:-|
| **1** | Conversation Pane | The left portion of the screen shows different conversation threads the user is holding with the chatbot.  To start a new conversation, click the '+'Bot symbol. |
| **2** | Conversation Thread | Chatbot responses will appear in the main conversation thread, along with a history of your prompts.   Users can scroll up and down to review a complete conversation history. |
| **3** | Prompt Entry Box | The bottom of the screen contains the prompt entry box, where users can type their prompts, and click the "Send" icon to the right of the box when ready to send it to the bot. |

## Next step

If you've tried all the apps and are excited to see more, please star the GitHub repo and join the Semantic Kernel community!

> [!div class="nextstepaction"]
> [Star the Semantic Kernel repo](https://aka.ms/sk/repo)
