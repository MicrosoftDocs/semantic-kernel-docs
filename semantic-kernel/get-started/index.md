---
title: Setting up Semantic Kernel
description: Setting up Semantic Kernel
author: johnmaeda
ms.topic: getting-started
ms.author: johnmaeda
ms.date: 05/04/2023
ms.service: mssearch
---
# Start learning how to use Semantic Kernel

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

In just a few steps, you can run the getting started guides for Semantic Kernel in either C# or Python. After completing the guides, you'll know how to...
- Configure your local machine to run Semantic Kernel
- Run prompts from the kernel
- Make prompts dynamic with variables
- Create prompt chains
- Automatically create new chains with the planner
- Store and retrieve memory with embeddings


If you are an experienced developer, you can skip the guides and directly access the packages.

# [C#](#tab/Csharp)

Instructions for accessing the `SemanticKernel` Nuget feed is available [here](https://aka.ms/sk/nuget). It's as easy as:

```Nuget
#r "nuget: Microsoft.SemanticKernel, *-*"
```

# [Python](#tab/python)

Instructions for accessing the `SemanticKernel` Python package is available [here](https://aka.ms/sk/pypi). It's as easy as:

```PyPI
pip install semantic-kernel
```

---



## Requirements to run the guides
Before running the guides in C#, make sure you have the following installed on your local machine. If you are using the Python guides, you just need `git` and `python`.

> [!div class="checklist"]
> * `git` or the [GitHub app](https://desktop.github.com/) 
> * [VSCode](https://code.visualstudio.com/Download) or [Visual Studio](https://visualstudio.microsoft.com/downloads/) 
> * An OpenAI key via either [Azure OpenAI Service](/azure/cognitive-services/openai/quickstart?pivots=programming-language-studio) or [OpenAI](https://openai.com/api/)
> * [.Net 7 SDK](https://dotnet.microsoft.com/download) - for C# notebook guides
> * In VS Code the [Polyglot Notebook](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode) - for notebook guides

## Download and run the guides
To setup the guides, follow the steps below.

> [!TIP]
> Have your OpenAI or Azure OpenAI keys ready to enter when prompted by the Jupyter notebook.


1. Use your web browser to visit [aka.ms/sk/repo](https://aka.ms/sk/repo) on GitHub. 

2. Clone or fork the repo to your local machine.

   > [!NOTE]
   > If you are new to using GitHub and have never cloned a repo to your local machine, please review [this guide](https://docs.github.com/repositories/creating-and-managing-repositories/cloning-a-repository).
   > [!NOTE]
   > If you are a new contributor to open source, please [fork the repo](https://docs.github.com/en/get-started/quickstart/contributing-to-projects) to start your journey.

   If you have trouble cloning or forking the repo, you can watch the video below.
   > [!VIDEO https://aka.ms/SK-Local-Setup]

3. While the repository is open in VS Code, navigate to the `semantic-kernel/samples/notebooks` folder.

4. Choose either the .Net or Python folder based on your preferred programming language.

5. Open the `00-getting-started.ipynb` notebook.
6. Activate each code snippet with the "play" button on the left hand side.

   If you need help running the `00-getting-started.ipynb` notebook, you can watch the video below.
   > [!VIDEO https://aka.ms/SK-Getting-Started-Notebook] 

7. Repeat for the remaining notebooks.
    

## Navigating the guides
The guides are designed to be run in order to build on the concepts learned in the previous notebook. If you are interested in learning a particular concept, however, you can jump to the notebook that covers that concept. Below are the available guides.

- `00-getting-started.ipynb` – Run your first prompt
- `01-basic-loading-the-kernel.ipynb` – Changing the configuration of the kernel
- `02-running-prompts-from-file.ipynb` – Learn how to run prompts from a file
- `03-semantic-function-inline.ipynb` – Configure and run prompts directly in code
- `04-context-variables-chat.ipynb` – Use variables to make prompts dynamic
- `05-using-the-planner.ipynb` – Dynamically create prompt chains with the planner
- `06-memory-and-embeddings.ipynb` – Store and retrieve memory with embeddings

## Like what you see?
If you are a fan of Semantic Kernel, please give the repo a ⭐️ star to show your support. 

:::image type="content" source="../media/pleasestarrepo.png" alt-text="Starring the repo for SK to show support":::

## Keep learning
The guides are an easy way run sample code and learn how to use Semantic Kernel. If you want to learn more about the concepts behind Semantic Kernel, keep reading the docs. Based on your experience level, you can jump to the section that best fits your needs.

| Experience level     | Next step     |
|--------------|-----------|
| For beginners who are just starting to learn about AI | [Learn prompt engineering](../prompt-engineering/index.md) |
| For people who are well versed in prompt engineering | [Orchestrate prompt chains](../create-chains/index.md) |
| For people familiar with chaining prompts |  [Store and retrieve memory ](../memories/index.md) |
| For those who want to see how it all works together |  [Run the sample apps](../samples-and-solutions/index.md) |