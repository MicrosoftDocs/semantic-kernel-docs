---
title: GitHub Repo Q&A Bot sample app
description: GitHub Repo Q&A Bot sample app
author: evchaki
ms.topic: samples
ms.author: evchaki
ms.date: 02/07/2023
ms.service: mssearch
---
# GitHub Repo Q&A Bot sample app

[!INCLUDE [subheader.md](../includes/pat_medium.md)]

The GitHub Repo Q&A Bot sample allows you to enter in a GitHub repo then those files are created as [embeddings](/semantic-kernel/concepts-ai/embeddings). You can then question the stored files from the embedding local storage.

> [!IMPORTANT]
> Each function will call OpenAI which will use tokens that you will be billed for. 

### Walkthrough video

>[!Video https://aka.ms/SK-GitHub-QA-Bot-Video]


## Requirements to run this app

> [!div class="checklist"]
> * [Local API service](/semantic-kernel/samples/localapiservice) is running
> * [Yarn](https://yarnpkg.com/getting-started/install) - used for installing the app's dependencies

## Running the app
The [GitHub Repo Q&A Bot sample app](https://aka.ms/sk/repo/samples/github-repo-qa-bot) is located in the Semantic Kernel GitHub repository.

1) Follow the [Setup](/semantic-kernel/get-started) instructions if you do not already have a clone of Semantic Kernel locally.
2) Start the [local API service](/semantic-kernel/samples/localapiservice).
3) Open the ReadMe file in the GitHub Repo Q&A Bot sample folder.
4) Open the Integrated Terminal window.
5) Run `yarn install` - if this is the first time you are running the sample.  Then run `yarn start`.
6) A browser will open with the sample app running

## Exploring the app

### Setup Screen
Start by entering in your [OpenAI key](https://openai.com/api/) or if you are using [Azure OpenAI Service](/azure/cognitive-services/openai/quickstart) the key and endpoint.  Then enter in the model for completion and embeddings you would like to use in this sample.

### GitHub Repository Screen
On this screen you can enter in public GitHub repo and the sample will download the repo using a function and add the files as embeddings.

### Q&A Screen
By default the Markdown files are stored as embeddings.  You can ask questions in the chat and get answers based on the embeddings.

## Next step

If you've tried all the apps and are excited to see more, please star the GitHub repo and join the SK community!

> [!div class="nextstepaction"]
> [Star the SK repo](https://aka.ms/sk/repo)

[!INCLUDE [footer.md](../includes/footer.md)]
