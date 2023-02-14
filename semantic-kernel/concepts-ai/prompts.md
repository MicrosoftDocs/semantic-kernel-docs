---
title: LLM AI Prompts
description: LLM AI Prompts
author: johnmaeda
ms.topic: concepts
ms.author: johnmaeda
ms.date: 02/07/2023
ms.prod: semantic-kernel
---
# What are Prompts?

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

> Prompts are the way of communicating and directing an LLM AI model's behavior.

Prompts are the inputs or queries that a user or a program gives to an LLM AI, in order to elicit a specific response from the model. Prompts can be natural language sentences or questions, or code snippets or commands, or any combination of text or code, depending on the domain and the task. Prompts can also be nested or chained, meaning that the output of one prompt can be used as the input of another prompt, creating more complex and dynamic interactions with the model. 

## Subtleties of prompting

The art of creatively defining LLM AI prompts is an emerging field known as ["prompt design"](https://platform.openai.com/docs/guides/completion/prompt-design) and also ["prompt engineering"](https://microsoft.github.io/prompt-engineering/). It involves the process of rafting effective and efficient prompts that can elicit the desired response from the LLM AI model. Key challenges include choosing the right words, phrases, symbols, and formats that can guide the model to generate high-quality and relevant texts. One can also involves experimenting with different parameters and settings that can influence the behavior and performance of the model, such as temperature, top-k, top-p, frequency penalty, and presence penalty.

Three common considerations when designing or engineering prompts are:

* **Prompt chaining:** This is a way of extending and enhancing the dialogue with the model, by using the generated texts as the basis for the next prompts. Prompt chaining can allow the model to explore different topics, scenarios, roles, and formats, and to generate more coherent, creative, and engaging texts. Prompt chaining can also help the model to learn from the feedback and the corrections that the user provides, and to adjust its behavior and output accordingly. Prompt chaining can be done manually or automatically, by using different types of prompts, such as follow-up, continuation, clarification, elaboration, and redirection.
 * [**Prompt tuning:**](https://www.microsoft.com/en-us/research/video/research-talk-prompt-tuning-what-works-and-whats-next/) This is the process of adapting and optimizing the prompts for specific tasks or domains, by using smaller and more specialized datasets. Prompt tuning can improve the accuracy and the diversity of the generated texts, by reducing the noise and the bias that may exist in the general dataset. Prompt tuning can also increase the robustness and the consistency of the model, by making it more resistant to adversarial inputs or unexpected situations.
* **Prompt testing:** This is the process of measuring and comparing the quality and the usefulness of the prompts and the generated texts, by using various metrics and criteria. Prompt evaluation can involve both human and automated methods, such as rating, ranking, feedback, analysis, and testing. Prompt evaluation can help identify the strengths and the weaknesses of the prompts and the model, and provide suggestions for improvement and refinement.

SK has been created to empower developers to craft complex chains of LLM AI prompts that are both configurable and testable. Since the process of designing and engineering prompts is likely to change over the coming years as LLM AI models evolve, SK community members should expect a steady stream of updates to the GitHub repository for the foreseeable future.

## Take the next step

> [!div class="nextstepaction"]
> [Learn about tokens](tokens)

[!INCLUDE [footer.md](../includes/footer.md)]