---
title: Book creator sample app
description: Book creator sample app
author: evchaki
ms.topic: samples
ms.author: evchaki
ms.date: 02/07/2023
ms.prod: semantic-kernel
---
# Local API service example
This service API is written in C# against Azure Function Runtime v4 and exposes some Semantic Kernel APIs that you can call via HTTP POST requests for the learning samples.

> [!IMPORTANT]
> Each function will call Open AI which will use tokens that you will be billed for. 

>[!Video https://aka.ms/SK-Local-API-Setup]

## Running the service API locally
**Run** `func start --csharp` from the command line. This will run the service API locally at `http://localhost:7071`.

Two endpoints will be exposed by the service API:

-   **InvokeFunction**: [POST] `http://localhost:7071/api/skills/{skillName}/invoke/{functionName}`
-   **Ping**: [GET] `http://localhost:7071/api/ping`

## Next steps

Now that your service API is running locally,
let's try it out in a sample app so you can learn core Semantic Kernel concepts!  
The service API will need to be run or running for each sample app you want to try.

Sample app learning examples:
## Take the next step



[!INCLUDE [footer.md](../includes/footer.md)]
