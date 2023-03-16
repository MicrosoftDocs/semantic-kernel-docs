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

_Memories_ are a powerful way to provide broader context for your ASK. Historically, we've always called upon memory as a core component for how computers work: think the RAM in your laptop. For with just a CPU that can crunch numbers, the computer isn't that useful unless it knows what numbers you care about. Memories are what make computation relevant to the task at hand.

We access memories to be fed into SK in one of three ways — with the third way being the most interesting:

1. Conventional key-value pairs: Just like you would set an environment variable in your shell, the same can be done when using SK. The lookup is "conventional" because it's a one-to-one match between a key and your query. 

2. Conventional local-storage: When you save information to a file, it can be retrieved with its filename. When you have a lot of information to store in a key-value pair, you're best off keeping it on disk.

3. Semantic memory search: You can also store text information as a long vector of numbers, known as "embeddings." This lets you execute a "semantic" search that compares meaning-to-meaning with your query. 

## How does semantic memory work?

Embeddings are a way of representing words or other data as vectors in a high-dimensional space. Vectors are like arrows that have a direction and a length. High-dimensional means that the space has many dimensions, more than we can see or imagine. The idea is that similar words or data will have similar vectors, and different words or data will have different vectors. This helps us measure how related or unrelated they are, and also perform operations on them, such as adding, subtracting, multiplying, etc. Embeddings are useful for AI models because they can capture the meaning and context of words or data in a way that computers can understand and process.

So basically you take a sentence, paragraph, or entire page of text, and then generate the corresponding embedding vector. And when a query is performed, the query is transformed to its embedding representation, and then a search is performed through all the existing embedding vectors to find the most similar ones. This is similar to when you make a search query on Bing, and it gives you multiple results that are proximate to your query. Semantic memory is not likely to give you an exact match — but it will always give you a set of matches ranked in terms of how similar your query matches other pieces of text.

## Why are embeddings important with LLM AI?

Since a prompt is a text that we give as input to an AI model to generate a desired output or response, we need to consider the length of the input text based on the token limit of the model we choose to use. For example, GPT-4 can handle up to 8,192 tokens per input, while GPT-3 can only handle up to 4,096 tokens. This means that texts that are longer than the token limit of the model will not fit and may be cut off or ignored.

It would be nice if we could use an entire 10,000-page operating manual as context for our prompt, but because of the token limit constraint, that is impossible. Therefore, embeddings are useful for breaking down that large text into smaller pieces. We can do this by summarizing each page into a shorter paragraph and then generating an embedding vector for each summary. An embedding vector is like a compressed representation of the text that preserves its meaning and context. Then we can compare the embedding vectors of our summaries with the embedding vector of our prompt and select the most similar ones. We can then add those summaries to our input text as context for our prompt. This way, we can use embeddings to help us choose and fit large texts as context within the token limit of the model.

## Take the next step

Now that you know about the _kernel, planner, skills, memories,_ it's time for _connectors._

> [!div class="nextstepaction"]
> [Learn about Connectors](/semantic-kernel/concepts-sk/connectors)

[!INCLUDE [footer.md](../includes/footer.md)]
