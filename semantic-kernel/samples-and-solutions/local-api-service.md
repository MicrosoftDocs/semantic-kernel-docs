---
title: Local API Service for samples
description: Local API Service app samples
author: evchaki
ms.topic: conceptual
ms.author: evchaki
ms.date: 02/07/2023
ms.service: semantic-kernel
---
# Local API service for app samples

[!INCLUDE [subheader.md](../includes/pat_large.md)]

This service API is written in C# against Azure Function Runtime v4 and exposes some Semantic Kernel APIs that you can call via HTTP POST requests for the [learning samples](/semantic-kernel/samples).

> [!IMPORTANT]
> Each function will call OpenAI which will use tokens that you will be billed for. 

### Walkthrough video

>[!Video https://learn-video.azurefd.net/vod/player?id=a7be2947-cc94-4a2d-9932-162977ed35ce]

## Requirements to run the local service

> [!div class="checklist"]
> * [Azure Functions Core Tools](/azure/azure-functions/functions-run-local) - used for running the kernel as a local API

## Running the service API locally

The [local API service](https://github.com/microsoft/semantic-kernel/tree/main/samples/dotnet/KernelHttpServer) is located in the Semantic Kernel GitHub repository.

**Run** `func start --csharp` from the command line. This will run the service API locally at `http://localhost:7071`.

Two endpoints will be exposed by the service API:

-   **InvokeFunction**: [POST] `http://localhost:7071/api/skills/{skillName}/invoke/{functionName}`
-   **Ping**: [GET] `http://localhost:7071/api/ping`

### Take the next step

Now that your service API is running locally, try out some of the sample apps so you can learn core Semantic Kernel concepts!  

> [!div class="nextstepaction"]
> [Jump into all the sample apps](/semantic-kernel/samples)
