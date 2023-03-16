---
title: Memories in Semantic Kernel
description: What are Memories in Semantic Kernel
author: johnmaeda
ms.topic: concepts
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---

# What are Memories?

| ASK⇾ | [Kernel](/semantic-kernel/concepts-sk/kernel) | [Planner](/semantic-kernel/concepts-sk/planner) | [Skills](/semantic-kernel/concepts-sk/skills)|   [Memories](/semantic-kernel/concepts-sk/memories) |[Connectors](/semantic-kernel/concepts-sk/Connectors) | >>>|  ⇾GET | 
|---|---|---|---|---|---|---|---|

[!INCLUDE [fullview.md](../includes/fullview.md)]

_Memories_ are a powerful way to provide broader context for your ASK. We've always called upon memory as a core component for how computers work: think the RAM in your laptop. For with just a CPU that can crunch numbers, the computer isn't that useful unless it knows what numbers you care about. Memories are what make computation relevant to the task at hand.

In SK, memories currently come in three variations:

* Conventional key-value pairs: Just like you would set an environment variable in your shell, the same can be done when using SK. The lookup is "conventional" because it's a one-to-one match between a key and your query. 

* Conventional local-storage: When you save information to a file, it can be retrieved with its filename. When you have a lot of information to store in a key-value pair, you're best off keeping it on disk.

* Semantic memory search: You can also store text information as a long vector of numbers, known as "embeddings." This lets you execute a "semantic" search that compares meaning-to-meaning with your query. 

Of course there are many other ways to store information locally and remotely which you're free to use with SK. These variations are what are available out-of-the-box.



## Take the next step

Now that you know about the _kernel, planner, skills, memories,_ it's time for _connectors._

> [!div class="nextstepaction"]
> [Learn about Connectors](/semantic-kernel/concepts-sk/connectors)

[!INCLUDE [footer.md](../includes/footer.md)]
