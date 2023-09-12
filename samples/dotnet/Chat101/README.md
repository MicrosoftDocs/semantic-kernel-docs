# Chat101: A begginer's chatbot

This sample runs a 101 level chat application with a large language model (LLM). The code demonstrates basic usage of the Semantic Kernel SDK and the importance of context in prompts.

It is referenced by the [Introduction to Context](tbd) article on Microsoft Learn.

# Requirements

You will need the following items to run the sample:

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- AI Service:

    | AI Service   | Requirement |
    | ------------ | ------------------------------------------------------------------------------ |
    | Azure OpenAI | - [Access](https://aka.ms/oai/access)<br>- [Resource](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#create-a-resource)<br>- [Endpoint](https://learn.microsoft.com/azure/ai-services/openai/tutorials/embeddings?tabs=command-line#retrieve-key-and-endpoint)<br>- [API key](https://learn.microsoft.com/azure/ai-services/openai/tutorials/embeddings?tabs=command-line#retrieve-key-and-endpoint)<br>- [Deployment name](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#deploy-a-model) of a [supported chat model](#supported-models)<br> |
    | OpenAI       | - [Account](https://platform.openai.com)<br>- [API key](https://platform.openai.com/account/api-keys)<br>- Model name of a [supported chat model](#supported-models)<br> |

 ## Supported Models
 This sample has been tested for the following models:

<details><summary><i>Azure OpenAI Chat Models: </i></summary>
<p>

| Service      | Model type      | Model name             | Model             | Model version | Supported |
| ------------ | --------------- | ---------------------- | ----------------- | ------------- | --------- |
| Azure OpenAI | Chat Completion | gpt-35-turbo           | gpt-3.5-turbo     |          0301 | ✅        |
| Azure OpenAI | Chat Completion | gpt-35-turbo           | gpt-3.5-turbo     |          0613 | ✅        |
| Azure OpenAI | Chat Completion | gpt-35-turbo-16k       | gpt-3.5-turbo-16k |          0613 | ✅        |

</p>
</details>

<details><summary><i>OpenAI Chat Models: </i></summary>
<p>

| Service | Model type      | Model name             | Model             | Model version | Supported |
| ------- | --------------- | ---------------------- | ----------------- | ------------- | --------- |
| OpenAI  | Chat Completion | gpt-3.5-turbo          | gpt-3.5-turbo     |             1 | ❌        |
| OpenAI  | Chat Completion | gpt-3.5-turbo-0301     | gpt-3.5-turbo     |          0301 | ✅        |
| OpenAI  | Chat Completion | gpt-3.5-turbo-0613     | gpt-3.5-turbo     |          0613 | ✅        |
| OpenAI  | Chat Completion | gpt-3.5-turbo-16k      | gpt-3.5-turbo-16k |             1 | ✅        |
| OpenAI  | Chat Completion | gpt-3.5-turbo-16k-0613 | gpt-3.5-turbo-16k |          0613 | ✅        |
| OpenAI  | Chat Completion | gpt-4                  | gpt-4             |             1 | ✅        |
| OpenAI  | Chat Completion | gpt-4-0314             | gpt-4             |          0314 | ✅        |
| OpenAI  | Chat Completion | gpt-4-0613             | gpt-4             |          0613 | ❌        |

</p>
</details>

> **IMPORTANT:** This sample is for educational purposes only and is not recommended for production deployments.

> **IMPORTANT:** Each chat interaction will call Azure OpenAI/OpenAI which will use tokens that you may be billed for.

# Instructions

## Configure the application

Use .NET [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to set the service variables.

   - **Azure OpenAI**

       ```powershell
       cd <path to Chat101/>

       dotnet user-secrets set "Global:LlmService" "AzureOpenAI"

       dotnet user-secrets set "AzureOpenAI:DeploymentType" "chat-completion"
       dotnet user-secrets set "AzureOpenAI:ChatCompletionDeploymentName" "... your chat model deployment name ..."
       dotnet user-secrets set "AzureOpenAI:Endpoint" "... your Azure OpenAI endpoint ..."
       dotnet user-secrets set "AzureOpenAI:ApiKey" "... your Azure OpenAI key ..."
       ```

   - **OpenAI**

       ```powershell
       cd <path to Chat101/>

       dotnet user-secrets set "Global:LlmService" "OpenAI"

       dotnet user-secrets set "OpenAI:ModelType" "chat-completion"
       dotnet user-secrets set "OpenAI:ChatCompletionModelId" "gpt-3.5-turbo-0301"
       dotnet user-secrets set "OpenAI:ApiKey" "... your OpenAI key ..."
       dotnet user-secrets set "OpenAI:OrgId" "... your org ID ..."
       ```

## Run the application

- Run the default application (with context).
   
   - **Visual Studio:** 
     1. Open `Chat101.csproj`. 
     2. Set `Chat101` as the startup project.
     3. Press `F5`.
   - **VS Code:** 
     1. Open `Chat101/` as its own workspace. 
     2. Press `F5`.
     3. Open the Terminal window.
   - **Command line:** 
     1. Navigate to `Chat101/`. 
     2. Run `dotnet run`.
     
- Run the application without context.
  
  1. Open `appsettings.json`.
  2. Set `UseContext` to `false`:

        ```json
        "Application":
        {
            "UseContext": false
        }
        ```

  3. Run the application (see above).

# Check out our other repos!

If you would like to learn more about Semantic Kernel and AI, you may also be interested in other repos the Semantic Kernel team supports:

| Repo                                                                              | Description                                                                                      |
| --------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------ |
| [Chat Copilot](https://github.com/microsoft/chat-copilot)                         | A reference application that demonstrates how to build a chatbot with Semantic Kernel.        |
| [Semantic Kernel](https://github.com/microsoft/semantic-kernel)                   | A lightweight SDK that integrates cutting-edge LLM technology quickly and easily into your apps. |
| [Semantic Kernel Docs](https://github.com/MicrosoftDocs/semantic-kernel-docs)     | The home for Semantic Kernel documentation that appears on the Microsoft learn site.             |
| [Semantic Kernel Starters](https://github.com/microsoft/semantic-kernel-starters) | Starter projects for Semantic Kernel to make it easier to get started.                           |
| [Semantic Memory](https://github.com/microsoft/semantic-memory)                   | A service that allows you to create pipelines for ingesting, storing, and querying knowledge.    |
