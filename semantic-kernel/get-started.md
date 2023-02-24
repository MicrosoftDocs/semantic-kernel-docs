---
title: Setting up Semantic Kernel
description: Setting up Semantic Kernel
author: johnmaeda
ms.topic: getting-started
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# Setting up Semantic Kernel

[!INCLUDE [pat_medium.md](includes/pat_medium.md)]

Getting started with Semantic Kernel (SK) is quick and simple by following these three steps:

1. Go to the SK GitHub repository ("repo")
2. Clone or fork the repo to your local machine
3. Run the Jupyter [notebooks](https://aka.ms/skjupyter) locally

## Requirements to run notebook samples

> [!div class="checklist"]
> * `git` or the [GitHub app](https://desktop.github.com/) 
> * [VSCode](https://code.visualstudio.com/Download) or [Visual Studio](https://visualstudio.microsoft.com/downloads/) 
> * An OpenAI key via either [Azure Open AI Service](/azure/cognitive-services/openai/quickstart?pivots=programming-language-studio) or [OpenAI](https://openai.com/api/)
> * [.Net 7 SDK](https://dotnet.microsoft.com/en-us/download) - for notebook samples
> * In VSCode or Visual Studio the [Polyglot Notebook](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode) - for notebook samples

## Step 1: Access the public SK repo

Use your web browser to visit [aka.ms/sk/repo](https://aka.ms/sk/repo) on GitHub and please give the repo a ⭐️ star to show your support.

![Starring the repo for SK to show support](/semantic-kernel/media/pleasestarrepo.png)

## Step 2: Clone or fork the repo

**New to GitHub?** If you are new to using GitHub and have never cloned a repo to your local machine, please review [this guide](https://docs.github.com/repositories/creating-and-managing-repositories/cloning-a-repository).

**New to contributing?** If you are a new contributor to open source, please [fork the repo](https://docs.github.com/en/get-started/quickstart/contributing-to-projects) to start your journey.

### Walkthrough video

> [!VIDEO https://aka.ms/SK-Local-Setup]

## Step 3: Run the Jupyter notebooks locally

> [!IMPORTANT]
> Make sure you have fulfilled the [requirements list](/semantic-kernel/getting-started/requirements) to run SK on your machine.

From your local machine:

1. While you have the repository open in VSCode or Visual Studio, go to the `notebooks` section
2. Activate each code snippet with the "play" button on the left hand side
3. Have your OpenAI or Azure OpenAI keys ready to enter when prompted

### Walkthrough video

> [!VIDEO https://aka.ms/SK-Getting-Started-Notebook] 

### For experienced C# .NET developers

Instructions on accessing the `Microsoft.SemanticKernel] nuget package is available [here](https://aka.ms/sk/nuget).

## Take the next step

For beginners who are just starting to learn LLM AI, you may want to start here:

> [!div class="nextstepaction"]
> [Learn LLM AI Concepts](/semantic-kernel/concepts-ai)

For people who are well versed in LLM AI, you can jump into SK immediately:

> [!div class="nextstepaction"]
> [Discover Semantic Kernel Concepts](/semantic-kernel/concepts-sk)

For people who are ready to run SK sample apps:

> [!div class="nextstepaction"]
> [Run Sample Apps](/semantic-kernel/samples)

[!INCLUDE [footer.md](includes/footer.md)]