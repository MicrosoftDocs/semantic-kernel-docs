---
title: Connectors in Semantic Kernel
description: Connectors in Semantic Kernel
author: johnmaeda
ms.topic: creating-chains
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---

# What are Connectors?

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

_Connectors_ let you reach outside of the plugins universe to external APIs and whatever else you can imagine. By combining your custom plugins with a custom set of connectors, you can build LLM AI app features that fully leverage realtime data into fully reusable "AI ready" components to add to all of your projects. 

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

The set of example connectors provided in the [repo](https://aka.ms/sk/repo) have designed to start you on your path to building your own external interfaces to Semantic Kernel.

## Take the next step

> [!div class="nextstepaction"]
> [Automatically create chains with planner](./planner.md)
