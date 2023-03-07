---
title: Semantic Kernel and Responsible AI
description: Semantic Kernel and Responsible AI
author: johnmaeda
ms.topic: responsibleai
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# Responsible AI and Semantic Kernel

[!INCLUDE [pat_small.md](includes/pat_small.md)]

## What is a Transparency Note?

An AI system includes not only the technology, but also the people who will use it, the people who will be affected by it, and the environment in which it is deployed. Creating a system that is fit for its intended purpose requires an understanding of how the technology works, what its capabilities and limitations are, and how to achieve the best performance. Microsoft’s Transparency Notes are intended to help you understand how our AI technology works, the choices system owners can make that influence system performance and behavior, and the importance of thinking about the whole system, including the technology, the people, and the environment. You can use Transparency Notes when developing or deploying your own system, or share them with the people who will use or be affected by your system.

Microsoft’s Transparency Notes are part of a broader effort at Microsoft to put our AI Principles into practice. To find out more, see the [Microsoft AI principles](https://www.microsoft.com/en-us/ai/responsible-ai).

## Introduction to Semantic Kernel

Semantic Kernel (SK) is a lightweight SDK that lets you easily mix conventional programming languages with the latest in Large Language Model (LLM) AI "prompts" with templating, chaining, and planning capabilities out-of-the-box.

## The basics of Semantic Kernel

Semantic Kernel (SK) builds upon the following five concepts:

| Concept | Short Description |
|---|---|
| Kernel | The kernel orchestrates a user's ASK expressed as a goal |
| Planner | The planner breaks it down into steps based upon resources that are available |
| Skills | Skills are customizable resources built from LLM AI prompts and native code |
| Memories | Memories are customizable resources that manage contextual information |
| Connectors | Connectors are customizable resources that enable external data access |

## Use cases for LLM AI

## Intended uses

The general intended uses include:

* Chat and conversation interaction: Users can interact with a conversational agent that responds with responses drawn from trusted documents such as internal company documentation or tech support documentation; conversations must be limited to answering scoped questions.
* Chat and conversation creation: Users can create a conversational agent that responds with responses drawn from trusted documents such as internal company documentation or tech support documentation; conversations must be limited to answering scoped questions.
* Code generation or transformation scenarios: For example, converting one programming language to another, generating docstrings for functions, converting natural language to SQL.
* Journalistic content: For use to create new journalistic content or to rewrite journalistic content submitted by the user as a writing aid for pre-defined topics. Users cannot use the application as a general content creation tool for all topics. May not be used to generate content for political campaigns.
* Question-answering: Users can ask questions and receive answers from trusted source documents such as internal company documentation. The application does not generate answers ungrounded in trusted source documentation.
* Reason over structured and unstructured data: Users can analyze inputs using classification, sentiment analysis of text, or entity extraction. Examples include analyzing product feedback sentiment, analyzing support calls and transcripts, and refining text-based search with embeddings.
* Search: Users can search trusted source documents such as internal company documentation. The application does not generate results ungrounded in trusted source documentation.
* Summarization: Users can submit content to be summarized for pre-defined topics built into the application and cannot use the application as an open-ended summarizer. Examples include summarization of internal company documentation, call center transcripts, technical reports, and product reviews.
* Writing assistance on specific topics: Users can create new content or rewrite content submitted by the user as a writing aid for business content or pre-defined topics. Users can only rewrite or create content for specific business purposes or pre-defined topics and cannot use the application as a general content creation tool for all topics. Examples of business content include proposals and reports. For journalistic use, see above Journalistic content use case.

## Considerations when choosing a use case for LLM AI

There are some considerations:

* Not suitable for open-ended, unconstrained content generation. Scenarios where users can generate content on any topic are more likely to produce offensive or harmful text. The same is true of longer generations.
* Not suitable for scenarios where up-to-date, factually accurate information is crucial unless you have human reviewers or are using the models to search your own documents and have verified suitability for your scenario. The service does not have information about events that occur after its training date, likely has missing knowledge about some topics, and may not always produce factually accurate information.
* Avoid scenarios where use or misuse of the system could result in significant physical or psychological injury to an individual. For example, scenarios that diagnose patients or prescribe medications have the potential to cause significant harm.
* Avoid scenarios where use or misuse of the system could have a consequential impact on life opportunities or legal status. Examples include scenarios where the AI system could affect an individual's legal status, legal rights, or their access to credit, education, employment, healthcare, housing, insurance, social welfare benefits, services, opportunities, or the terms on which they are provided.
* Avoid high stakes scenarios that could lead to harm. Each LLM AI model reflects certain societal views, biases and other undesirable content present in the training data or the examples provided in the prompt. As a result, we caution against using the models in high-stakes scenarios where unfair, unreliable, or offensive behavior might be extremely costly or lead to harm.
* Carefully consider use cases in high stakes domains or industry: Examples include but are not limited to healthcare, medicine, finance or legal.
* Carefully consider well-scoped chatbot scenarios. Limiting the use of the service in chatbots to a narrow domain reduces the risk of generating unintended or undesirable responses.
* Carefully consider all generative use cases. Content generation scenarios may be more likely to produce unintended outputs and these scenarios require careful consideration and mitigations.

## Characteristics and limitations of LLM AI

When it LLM AI models, there are particular fairness and responsible AI issues to consider. People use language to describe the world and to express their beliefs, assumptions, attitudes, and values. As a result, publicly available text data typically used to train large-scale natural language processing models contains societal biases relating to race, gender, religion, age, and other groups of people, as well as other undesirable content. These societal biases are reflected in the distributions of words, phrases, and syntactic structures.

## Evaluating and integrating Semantic Kernel for your use

When getting ready to deploy any AI-powered products or features, the following activities help to set you up for success:

* Understand what it can do: Fully assess the capabilities of any AI system you are using to understand its capabilities and limitations. Understand how it will perform in your particular scenario and context by thoroughly testing it with real life conditions and data.

* Respect an individual's right to privacy: Only collect data and information from individuals for lawful and justifiable purposes. Only use data and information that you have consent to use for this purpose.

* Legal review: Obtain appropriate legal advice to review your solution, particularly if you will use it in sensitive or high-risk applications. Understand what restrictions you might need to work within and your responsibility to resolve any issues that might come up in the future. Do not provide any legal advice or guidance.

* Human-in-the-loop: Keep a human-in-the-loop and include human oversight as a consistent pattern area to explore. This means ensuring constant human oversight of the AI-powered product or feature, and maintaining the role of humans in decision making. Ensure you can have real-time human intervention in the solution to prevent harm. This enables you to manage situations when the AI model does not perform as required.

* Security: Ensure your solution is secure and has adequate controls to preserve the integrity of your content and prevent unauthorized access.

* Build trust with affected stakeholders: Communicate the expected benefits and potential risks to affected stakeholders. Help people understand why the data is needed and how the use of the data will lead to their benefit. Describe data handling in an understandable way.

* Customer feedback loop: Provide a feedback channel that allows users and individuals to report issues with the service once it's been deployed. Once you've deployed an AI-powered product or feature it requires ongoing monitoring and improvement -- be ready to implement any feedback and suggestions for improvement. Establish channels to collect questions and concerns from affected stakeholders (people who may be directly or indirectly impacted by the system, including employees, visitors, and the general public). Examples of such channels are:

* Feedback features built into app experiences,
An easy-to-remember email address for feedback,
Anonymous feedback boxes placed in semi-private spaces, and
Knowledgeable representatives on site.
Feedback: Seek out feedback from a diverse sampling of the community during the development and evaluation process (for example, historically marginalized groups, people with disabilities, and service workers). See: [Community Jury](https://learn.microsoft.com/en-us/azure/architecture/guide/responsible-innovation/community-jury/).

* User Study: Any consent or disclosure recommendations should be framed in a user study. Evaluate the first and continuous-use experience with a representative sample of the community to validate that the design choices lead to effective disclosure. Conduct user research with 10-20 community members (affected stakeholders) to evaluate their comprehension of the information and to determine if their expectations are met.

## Learn more about responsible AI

* [Microsoft responsible AI resources]()
* [Microsoft Azure Learning course on responsible AI](https://learn.microsoft.com/en-us/training/paths/responsible-ai-business-principles/)

[!INCLUDE [footer.md](includes/footer.md)]