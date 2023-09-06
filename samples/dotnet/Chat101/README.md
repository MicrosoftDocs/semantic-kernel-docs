# Chat101: A console chat application 

This sample runs a simple chat experience with a large language model (LLM). The code demonstrates basic usage of the Semantic Kernel SDK and the importance of context in prompts.

> **IMPORTANT:** This sample is for educational purposes only and is not recommended for production deployments.

> **IMPORTANT:** Each chat interaction will call Azure OpenAI/OpenAI which will use tokens that you may be billed for.

![A chat application experience with an LLM](Chat101-context.gif)

# Requirements

You will need the following items to run the sample:

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- AI Service: See requirements below.

| AI Service   | Requirement                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| ------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Azure OpenAI | - [Access](https://aka.ms/oai/access)<br>- [Resource](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#create-a-resource)<br>- [Deployed chat model](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#deploy-a-model) (`gpt-35-turbo`)<br>- [API key](https://learn.microsoft.com/azure/ai-services/openai/tutorials/embeddings?tabs=command-line#retrieve-key-and-endpoint)<br>- [Endpoint](https://learn.microsoft.com/azure/ai-services/openai/tutorials/embeddings?tabs=command-line#retrieve-key-and-endpoint) |
| OpenAI       | - [Account](https://platform.openai.com)<br>- [API key](https://platform.openai.com/account/api-keys)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |

# Instructions

## Configure the application

1. Copy the development settings file:

    **Windows:**

    ```powershell
    cd <path to \Chat101\>
    cp .\appsettings.Development.json .\appsettings.json
    ```

    **Linux/macOS:**

    ```bash
    cd <path to /Chat101/>
    cp appsettings.Development.json appsettings.json
    ```

2. Open  `appsettings.json` and update the service fields:  
   
    ```json
    {
        "Service":
        {
            "AIService": "<AzureOpenAI | OpenAI>",
            "ChatModelName": "<AzureOpenAI deployment name | OpenAI model name>",
            "APIKey": "<AzureOpenAI API key | OpenAI API key>",
            "AzureOpenAIEndpoint": "<AzureOpenAI ONLY: https:// ...fill-in... .openai.azure.com/>"
        },
        ...
    }
    ```

3. Run the application.

    - **Visual Studio:** 
      - Open `Chat101.sln`. 
      - Press `F5`.
    - **VS Code:** 
      - Open `Chat101/` as its own workspace. 
      - Press `F5`.
    - **Command line:** 
      - Enter `Chat101/`. 
      - Run `dotnet run`.

4. Interact with the chatbot!

5. Try a chat experience with no context. What happens?

   - Open `appsettings.json`.
   - Set `UseContext` to `false`.
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

- Read the [documentation](https://learn.microsoft.com/en-us/semantic-kernel/)
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
