---
title: How to quickly start with Semantic Kernel
description: Follow along with Semantic Kernel's guides to quickly learn how to use the SDK.
author: matthewbolanos
ms.topic: quickstart
ms.author: mabolan
ms.date: 07/11/2023
ms.service: semantic-kernel
---
# Start learning how to use Semantic Kernel

[!INCLUDE [pat_large.md](../../includes/pat_large.md)]

In just a few steps, you can start running  the getting started guides for Semantic Kernel in either C# or Python. After completing the guides, you'll know how to...
- Configure your local machine to run Semantic Kernel
- Run AI prompts from the kernel
- Make AI prompts dynamic with variables
- Create a simple AI agent
- Automatically combine functions together with planners
- Store and retrieve memory with embeddings


If you are an experienced developer, you can skip the guides and directly access the packages from the Nuget feed or PyPI.

# [C#](#tab/Csharp)

Instructions for accessing the `SemanticKernel` Nuget feed is available [here](https://www.nuget.org/packages/Microsoft.SemanticKernel/). It's as easy as:

```Nuget
#r "nuget: Microsoft.SemanticKernel, *-*"
```

# [Python](#tab/python)

Instructions for accessing the `SemanticKernel` Python package is available [here](https://pypi.org/project/semantic-kernel/). It's as easy as:

```PyPI
pip install semantic-kernel
```

---



## Requirements to run the guides
Before running the guides in C#, make sure you have the following installed on your local machine.

> [!div class="checklist"]
> * `git` or the [GitHub app](https://desktop.github.com/) 
> * [VSCode](https://code.visualstudio.com/Download) or [Visual Studio](https://visualstudio.microsoft.com/downloads/) 
> * An OpenAI key via either [Azure OpenAI Service](/azure/cognitive-services/openai/quickstart?pivots=programming-language-studio) or [OpenAI](https://openai.com/api/)
> * [.Net 7 SDK](https://dotnet.microsoft.com/download) - for C# notebook guides
> * In VS Code the [Polyglot Notebook](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode) - for notebook guides

If you are using the Python guides, you just need `git` and `python`. These guides have been tested on python versions 3.8-3.11.

## Download and run the guides
To setup the guides, follow the steps below.

> [!TIP]
> Have your OpenAI or Azure OpenAI keys ready to enter when prompted by the Jupyter notebook.


1. Use your web browser to visit [aka.ms/sk/repo](https://github.com/microsoft/semantic-kernel) on GitHub. 

2. Clone or fork the repo to your local machine.

   > [!NOTE]
   > If you are new to using GitHub and have never cloned a repo to your local machine, please review [this guide](https://docs.github.com/repositories/creating-and-managing-repositories/cloning-a-repository).
   > [!NOTE]
   > If you are a new contributor to open source, please [fork the repo](https://docs.github.com/en/get-started/quickstart/contributing-to-projects) to start your journey.

   If you have trouble cloning or forking the repo, you can watch the video below.
   > [!VIDEO https://learn-video.azurefd.net/vod/player?id=5a410eae-b131-4227-a8e5-8e24e0cefd8e]

3. While the repository is open in VS Code, navigate to the `/dotnet/notebooks` or `/python/notebooks` folder.

4. Choose either the `dotnet` or `python` folder based on your preferred programming language.

5. Open the _00-getting-started.ipynb_ notebook.
6. Activate each code snippet with the "play" button on the left hand side.

   If you need help running the _00-getting-started.ipynb_ notebook, you can watch the video below.
   > [!VIDEO https://learn-video.azurefd.net/vod/player?id=fc3c792e-3b4d-4009-900c-588ee35ee426] 

7. Repeat for the remaining notebooks.
    

## Navigating the guides
The guides are designed to be run in order to build on the concepts learned in the previous notebook. If you are interested in learning a particular concept, however, you can jump to the notebook that covers that concept. Below are the available guides; each one can also be opened within the docs website by clicking on the **Open guide** link.


| File | Link | Description |
| --- | --- | --- |
| _00-getting-started.ipynb_| [Open guide](./getting-started.md) | Run your first prompt  |
| _01-basic-loading-the-kernel.ipynb_ | [Open guide](./loading-the-kernel.md) | Changing the configuration of the kernel |
| _02-running-prompts-from-file.ipynb_ |  [Open guide](./running-prompts-from-files.md) | Learn how to run prompts from a file |
| _03-semantic-function-inline.ipynb_ | [Open guide](./semantic-function-inline.md) | Configure and run prompts directly in code | 
| _04-context-variables-chat.ipynb_ | [Open guide](./context-variables-chat.md) | Use variables to make prompts dynamic |
| _05-using-the-planner.ipynb_ | [Open guide](./using-the-planner.md) | Dynamically create prompt chains with planners |
| _06-memory-and-embeddings.ipynb_ | [Open guide](./memory-and-embeddings.md) | Store and retrieve memory with embeddings |


> [!div class="nextstepaction"]
> [Start the first guide](./getting-started.md)


## Like what you see?
If you are a fan of Semantic Kernel, please give the repo a ⭐️ star to show your support. 

:::image type="content" source="../../media/pleasestarrepo.png" alt-text="Starring the repo for SK to show support":::

## Keep learning
The guides are an easy way run sample code and learn how to use Semantic Kernel. If you want to learn more about the concepts behind Semantic Kernel, keep reading the docs. Based on your experience level, you can jump to the section that best fits your needs.

| Experience level     | Next step     |
|--------------|-----------|
| For beginners who are just starting to learn about AI | [Learn prompt engineering](../../prompt-engineering/index.md) |
| For people who are well versed in prompt engineering | [Orchestrate AI plugins](../../ai-orchestration/index.md) |
| For people familiar with using AI plugins |  [Store and retrieve memory ](../../memories/index.md) |
| For those who want to see how it all works together |  [Run the sample apps](../../samples-and-solutions/index.md) |


> [!div class="nextstepaction"]
> [Learn how to Orchestrate AI](../../ai-orchestration/index.md)
