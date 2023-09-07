# Chat101: A begginer's chat app

This sample runs a simple chat experience with a large language model (LLM). The code demonstrates basic usage of the Semantic Kernel SDK and the importance of context in prompts.

![A chat application experience with an LLM](Chat101-context.gif)

> **IMPORTANT:** This sample is for educational purposes only and is not recommended for production deployments.

> **IMPORTANT:** Each chat interaction will call Azure OpenAI/OpenAI which will use tokens that you may be billed for.

# Requirements

You will need the following items to run the sample:

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- AI Service: See requirements below.

| AI Service   | Requirement                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| ------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Azure OpenAI | - [Access](https://aka.ms/oai/access)<br>- [Resource](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#create-a-resource)<br>- [Deployed chat model](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#deploy-a-model)<br>- [API key](https://learn.microsoft.com/azure/ai-services/openai/tutorials/embeddings?tabs=command-line#retrieve-key-and-endpoint)<br>- [Endpoint](https://learn.microsoft.com/azure/ai-services/openai/tutorials/embeddings?tabs=command-line#retrieve-key-and-endpoint) |
| OpenAI       | - [Account](https://platform.openai.com)<br>- [API key](https://platform.openai.com/account/api-keys)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |

# Instructions

1. Configure the application.

   - **Option 1: .NET Secret Manager (default)**
  
        This is the default option to set AI Service settings. If you want to customize the settings for individual samples, use the **`appsettings.json`** option below.

        <details><summary><i>Instructions</i></summary>
        <p>

        Run the following commands for your AI Service:

        - Azure OpenAI: 

            ```powershell
            dotnet user-secrets set "Global:LlmService" "AzureOpenAI"
            dotnet user-secrets set "AzureOpenAI:DeploymentType" "chat-completion"
            dotnet user-secrets set "AzureOpenAI:ChatCompletionDeploymentName" "... your chat model's deployment name ..."
            dotnet user-secrets set "AzureOpenAI:Endpoint" "... your Azure OpenAI endpoint ..."
            dotnet user-secrets set "AzureOpenAI:ApiKey" "... your Azure OpenAI API key ..."
            ```

        - OpenAI:
            
            ```powershell
            dotnet user-secrets set "Global:LlmService" "OpenAI"
            dotnet user-secrets set "OpenAI:ModelType" "chat-completion"
            dotnet user-secrets set "OpenAI:ChatCompletionModelId" "gpt-3.5-turbo"
            dotnet user-secrets set "OpenAI:ApiKey" "... your OpenAI API key ..."
            dotnet user-secrets set "OpenAI:OrgId" "... your org ID ..."
            ```

        See .NET [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for more information.
        </p>
        </details>

    - **Option 2: `appsettings.json`**

        This option is available to customize the AI Service settings per sample. If you want to use the same settings for all samples, use the **.NET Secret Manager** default above.

        <details><summary><i>Instructions</i></summary>
        <p>

        1. Copy `appsettings.Development.json`.

            **Windows**

            ```powershell
            cd <path to \Chat101\>
            cp .\appsettings.Development.json .\appsettings.json
            ```

            **Linux/macOS**

            ```bash
            cd <path to /Chat101/>
            cp appsettings.Development.json appsettings.json
            ```

        2. Open  `appsettings.json` and update the `Service` fields.

            ```json
            "Service":
            {
                // To use instead of `dotnet secrets`, uncomment below and fill-in.
                "AIService": "AzureOpenAI | OpenAI",
                "ChatModelName": "<AzureOpenAI-deployment-name> | <OpenAI-model-name>",
                "APIKey": "<API-key>", // It is recommended to use .NET Secret Manager instead of hard-coding.
                "AzureOpenAIEndpoint": "<https:// ...fill-in... .openai.azure.com/>"
            },
            ```
        </p>
        </details>

2. Run the application.

   - Run the default application (with context).
        - **Visual Studio:** 
          - Open `Chat101.sln`. 
          - Press `F5`.
        - **VS Code:** 
          - Open `Chat101/` as its own workspace. 
          - Press `F5`.
        - **Command line:** 
          - Enter `Chat101/`. 
          - Run `dotnet run`.

   - Run the application without context.
   
       1. Copy `appsettings.Development.json` (if not done previously).

           **Windows**

           ```powershell
           cd <path to \Chat101\>
           cp .\appsettings.Development.json .\appsettings.json
           ```

           **Linux/macOS**

           ```bash
           cd <path to /Chat101/>
           cp appsettings.Development.json appsettings.json
           ```

       2. Open `appsettings.json`.
      
            - Set `UseContext` to `false`.

                ```json
                "Application":
                {
                    "UseContext": false
                }
                ```
            - Run the application.

# Check out our other repos!

If you would like to learn more about Semantic Kernel and AI, you may also be interested in other repos the Semantic Kernel team supports:

| Repo                                                                              | Description                                                                                      |
| --------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------ |
| [Semantic Kernel](https://github.com/microsoft/semantic-kernel)                   | A lightweight SDK that integrates cutting-edge LLM technology quickly and easily into your apps. |
| [Semantic Kernel Docs](https://github.com/MicrosoftDocs/semantic-kernel-docs)     | The home for Semantic Kernel documentation that appears on the Microsoft learn site.             |
| [Semantic Kernel Starters](https://github.com/microsoft/semantic-kernel-starters) | Starter projects for Semantic Kernel to make it easier to get started.                           |
| [Semantic Memory](https://github.com/microsoft/semantic-memory)                   | A service that allows you to create pipelines for ingesting, storing, and querying knowledge.    |

## Join the community

We welcome your contributions! One of the easiest ways to participate is to engage in discussions in the GitHub repository.
Bug reports and fixes are welcome!

To learn more and get started:

- Read the [documentation](https://learn.microsoft.com/semantic-kernel/)
- Join the [Discord community](https://aka.ms/SKDiscord)
- [Contribute](CONTRIBUTING.md) to the project
- Follow the team on our [blog](https://aka.ms/sk/blog)

## Code of Conduct

This project has adopted the
[Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the
[Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com)
with any additional questions or comments.

## License

Copyright (c) Microsoft Corporation. All rights reserved.
