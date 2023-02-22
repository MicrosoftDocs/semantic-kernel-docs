---
title: Troubleshooting Semantic Kernel
description: Troubleshooting Semantic Kernel
author: evchaki
ms.topic: troubleshooting
ms.author: evchaki
ms.date: 02/07/2023
ms.prod: semantic-kernel
---
# Troubleshooting Semantic Kernel


[!INCLUDE [subheader.md](../includes/pat_medium.md)]

We've worked hard to remove the kinks from getting going with Semantic Kernel, but we realize that every developer experience is different and challenging. 

> [!TIP]
> There are a variety of [support options](overview) available to you at any time to match how you may like to learn.

For the cases we've bumped into that you may be experiencing, here's our most current list to get your productivity game back up to speed.

## My OpenAI or Azure OpenAI key doesn't seem to be working all the time

Depending upon the model you are trying to access, there may be times when your key may not work because of high demand. Or, because your access to the model is limited by the plan you're currently signed up for — so-called "throttling". In general, however, your key will work according to the plan agreement with your LLM AI provider. 

## The Jupyter notebooks aren't coming up in my VSCode or Visual Studio

First of all, you'll need to be running locally on your own machine to interact with the Jupyter notebooks. If you've already cleared that hurdle, then all you need to do is to install the [Polyglot Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode) which requires .NET 7 to be installed. For complete information on the latest release of Polyglot Extension you can learn more [here](https://devblogs.microsoft.com/dotnet/polyglot-notebooks-december-2022-release/).

## I'm on an M-series Mac and am having problems running Azure Functions

If you're running our samples, you'll need to get a local server running on your machine. We've noticed that depending upon how your Mac may be configured, it's possible to run into some problems. There's a troubleshooting guide [here](mseriesmacbook) if that's the case.

## Take the next step

> [!div class="nextstepaction"]
> [Get more support](overview)

[!INCLUDE [footer.md](../includes/footer.md)]