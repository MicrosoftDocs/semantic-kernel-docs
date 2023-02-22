---
title: Connectors in Semantic Kernel
description: Connectors in Semantic Kernel
author: johnmaeda
ms.topic: concepts
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---

# What are Connectors?

| ASK⇾ | [Kernel](kernel) | [Planner](planner) | [Skills](skills)| | Connectors | >>>|  ⇾GET | 
|---|---|---|---|---|---|---|---|

[!INCLUDE [fullview.md](../includes/fullview.md)]

_Connectors_ let you reach outside of the skills universe to external APIs and whatever else you can imagine. By combining your custom skills with a custom set of connectors, you can build LLM AI app features that fully leverage realtime data into fully reusable "AI ready" components to add to all of your projects. 

## What's the MS Graph Connector Kit?

The MS Graph Connector Kit lets you fluidly connect with useful data that's only available **to you** when securely logged-in. We currently support your ability to:

* Add an event to your calendar
* Send an email for you
* Add a file to your OneDrive
* Create a share link to a file in your OneDrive
* Query your organization hierarchy
* Manage your MS To Do list

## More out-of-the-box connectors

* Issue a Bing search query
* Read OpenXML streams (e.g. Word docs)
* Use SQLite as a lightweight database

The set of example connectors provided in the [repo](https://aka.ms/sk/repo) have designed to start you on your path to building your own external interfaces to SK.

## Take the next step

> [!TIP]
> Try the [Authentication and API sample app](/semantic-kernel/samples/authapi) to see a _Connector_ in action.

Now that you know about the _kernel_, _planner_, _skills_, _connectors_ then you're ready for all the sample apps.

> [!div class="nextstepaction"]
> [Run Sample Apps](/semantic-kernel/samples/overview)

[!INCLUDE [footer.md](../includes/footer.md)]
