---
title: Local API Service for samples
description: Local API Service app samples
author: evchaki
ms.topic: samples
ms.author: evchaki
ms.date: 02/07/2023
ms.service: mssearch
---
# Local API service for app samples

[!INCLUDE [subheader.md](../includes/pat_medium.md)]

This service API is written in C# against Azure Function Runtime v4 and exposes some Semantic Kernel APIs that you can call via HTTP POST requests for the [learning samples](/semantic-kernel/samples).

> [!IMPORTANT]
> Each function will call OpenAI which will use tokens that you will be billed for. 

### Walkthrough video

>[!Video https://aka.ms/SK-Local-API-Setup]

## Requirements to run the local service

> [!div class="checklist"]
> * [Azure Functions Core Tools](/azure/azure-functions/functions-run-local) - used for running the kernel as a local API

## Running the service API locally

The [local API service](https://aka.ms/sk/repo/api-azure-function) is located in the Semantic Kernel GitHub repository.

**Run** `func start --csharp` from the command line. This will run the service API locally at `http://localhost:7071`.

Two endpoints will be exposed by the service API:

-   **InvokeFunction**: [POST] `http://localhost:7071/api/skills/{skillName}/invoke/{functionName}`
-   **Ping**: [GET] `http://localhost:7071/api/ping`

### Take the next step

Now that your service API is running locally, try out some of the sample apps so you can learn core Semantic Kernel concepts!  

> [!div class="nextstepaction"]
> [Jump into all the sample apps](/semantic-kernel/samples)

[!INCLUDE [footer.md](../includes/footer.md)]
