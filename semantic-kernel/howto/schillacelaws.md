---
title: Schillace Laws of Semantic AI
description: Schillace Laws of Semantic AI
author: johnmaeda
ms.topic: advanced
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# Schillace Laws of Semantic AI
[!INCLUDE [pat_large.md](../includes/pat_small.md)]

## Consider the future of this decidedly "semantic" AI

The "Schillace Laws" were formulated after working with a variety of Large Language Model (LLM) AI systems to date. Knowing them will accelerate your journey into this exciting space of reimagining the future of software engineering. Welcome!

1. **Don’t write code if the model can do it; the model will get better, but the code won't.** The overall goal of the system is to build very high leverage programs using the LLM's capacity to plan and understand intent. It's very easy to slide back into a more imperative mode of thinking and write code for aspects of a program. Resist this temptation – to the degree that you can get the model to do something reliably now, it will be that much better and more robust as the model develops.  

2. **Trade leverage for precision; use interaction to mitigate.** Related to the above, the right mindset when coding with an LLM is not "let's see what we can get the dancing bear to do," it's to get as much leverage from the system as possible. For example, it's possible to build very general patterns, like "build a report from a database" or "teach a year of a subject" that can be parameterized with plain text prompts to produce enormously valuable and differentiated results easily.  

3. **Code is for syntax and process; models are for semantics and intent.** There are lots of different ways to say this, but fundamentally, the models are stronger when they are being asked to reason about meaning and goals, and weaker when they are being asked to perform specific calculations and processes. For example, it's easy for advanced models to write code to solve a sudoku generally, but hard for them to solve a sudoku themselves. Each kind of code has different strengths and it's important to use the right kind of code for the right kind of problem. The boundaries between syntax and semantics are the hard parts of these programs.  

4. **The system will be as brittle as its most brittle part.** This goes for either kind of code. Because we are striving for flexibility and high leverage, it’s important to not hard code anything unnecessarily. Put as much reasoning and flexibility into the prompts and use imperative code minimally to enable the LLM.

5. **Ask Smart to Get Smart.** Emerging LLM AI models are incredibly capable and "well educated" but they lacks context and initiative. If you ask them a simple or open-ended question, you will get a simple or generic answer back. If you want more detail and refinement, the question has to be more intelligent. This is an echo of "Garbage in, Garbage out" for the AI age.  

6. **Uncertainty is an exception throw.** Because we are trading precision for leverage, we need to lean on interaction with the user when the model is uncertain about intent. Thus, when we have a nested set of prompts in a program, and one of them is uncertain in its result ("One possible way...") the correct thing to do is the equivalent of an "exception throw" - propagate that uncertainty up the stack until a level that can either clarify or interact with the user.  

7. **Text is the universal wire protocol.** Since the LLMs are adept at parsing natural language and intent as well as semantics, text is a natural format for passing instructions between prompts, modules and LLM based services. Natural language is less precise for some uses, and it is possible to use structured language like XML sparingly, but generally speaking, passing natural language between prompts works very well, and is less fragile than more structured language for most uses. Over time, as these model-based programs proliferate, this is a natural "future proofing" that will make disparate prompts able to understand each other, the same way humans do.  

8. **Hard for you is hard for the model.** One common pattern when giving the model a challenging task is that it needs to "reason out loud." This is fun to watch and very interesting, but it's problematic when using a prompt as part of a program, where all that is needed is the result of the reasoning. However, using a "meta" prompt that is given the question and the verbose answer and asked to extract just the answer works quite well. This is a cognitive task that would be easier for a person (it's easy to imagine being able to give someone the general task of "read this and pull out whatever the answer is" and have that work across many domains where the user had no expertise, just because natural language is so powerful). _So, when writing programs, remember that something that would be hard for a person is likely to be hard for the model, and breaking patterns down into easier steps often gives a more stable result._

9. **​​​​​​​Beware "pareidolia of consciousness"; the model can be used against itself."** It is very easy to imagine a "mind" inside an LLM. But there are meaningful differences between human thinking and the model. An important one that can be exploited is that the models currently don't remember interactions from one minute to the next. So, while we would never ask a human to look for bugs or malicious code in something they had just personally written, we can do that for the model. It might make the same kind of mistake in both places, but it's not capable of "lying" to us because it doesn't know where the code came from to begin with.  ​​​​​​_This means we can "use the model against itself" in some places – it can be used as a safety monitor for code, a component of the testing strategy, a content filter on generated content, etc._

## Take the next step

If you're interested in LLM AI models and feel inspired by the Schillace Laws, be sure to visit the Semantic Kernel GitHub repository and add a star to show your support!

> [!div class="nextstepaction"]
> [Go to the SK GitHub repository](https://aka.ms/sk/repo)

[!INCLUDE [footer.md](../includes/footer.md)]