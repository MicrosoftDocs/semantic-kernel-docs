---
title: Tutorials - Lesson 1
description: Learn how to proceed with Semantic Kernel tutorials for lesson 1.
author: matthewbolanos
ms.topic: tutorial
ms.author: sopand
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Semantic Kernel Tutorials - Lesson 1

Goal
To provide users with simple learning plans that allow you to quickly learn Semantic Kernel starting with the basic to advanced concepts
Use a lesson based approach for understanding of concepts that allow you to progressively learn advanced concepts
Allow much faster adoption of AI using by leveraging proven practices
Provide users with reusable building blocks resulting in ability to deliver solutions faster
Lessons
Lesson 1 - Why Semantic Kernel? - Building our first App?
In this lession we cover the fundimental concepts of Semantic Kernel and what is needed to build your first console app.

Solutions
Each lesson will have a separate project in the Semantic-Kernel-101.sln. If you need to see a working example for the lesson you can find them here. The idea is that you do the homework, put hands on the keyboard and you create the project that that covers the topics outlined in each lesson. If you need a little help you can use this solution to get you moving.



# Lesson 1 - Why Semantic Kernel? - Building our first App?

## What will you learn in this lession?
- Why is an SDK like Semantic Kernel Needed?
- What is required to build a basic Chat Bot with Semantic Kernel?
  
## Why is this important?
It's important to understand why a framework is needed for AI based solutions and what are the fundemental building blocks needed for a basic Semantic Kernel Chat Bot.  Once you understand this basics, you are start to leverage the more complex features.

## What is needed to build our first App?

### 5 Simple steps are needed
1. Create a Kernel Builder

   ~~~
         var builder = Kernel.CreateBuilder();
   ~~~

2. Load you AI Endpoint Details

   ~~~
        var openAiDeployment = ConfigurationManager.AppSettings.Get("AzureOpenAIModel");
        var openAiUri = ConfigurationManager.AppSettings.Get("AzureOpenAIEndpoint");
        var openAiApiKey = ConfigurationManager.AppSettings.Get("AzureOpenAIKey");
   ~~~

3. Add the Chat Completion Service

   ~~~
       builder.Services.AddAzureOpenAIChatCompletion(
            deploymentName: openAiDeployment!,
            endpoint: openAiUri!,
            apiKey: openAiApiKey!);
   ~~~

4. Construct the Kernel, ChatHistory, get instance of the ChatCompletion Service

   ~~~
        var kernel = builder.Build();
        ChatHistory history = [];
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
   ~~~

5. Send a prompt or ChatHistory and get a response from LLM

   ~~~
       var prompt = "Why is the Sky blue?";
       var result = await chatCompletionService.GetChatMessageContentAsync(prompt);
       Console.WriteLine(result);
   ~~~

## Let's build your first .NET Core SK Console App

1. Create a new .NET Core Console App in Visual Studio and name it SK-Lesson-1
2. Add the Semantic Kernel Package to the project.
   - Click on **Dependecies->Manage NuGet Packages** and inssue the package below 

    <details>
    <summary><u>Packages</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

        Microsoft.SemanticKernel 1.6.3 or better
    </details>

3. Come up with some way to store and read the Model Name, AI Key and AI Endpoint.  Here is an example of using the System.Configuration class and an App.Config file.
   - Create an App.Config file in the root of the project using the following format and replace the values to point to your AI Endpoint

   ~~~
      <?xml version="1.0" encoding="utf-8" ?>
      <configuration>
	      <appSettings>
		      <add key="AzureOpenAIEndpoint" value="AzureOpenAI-Endpoint-URI" />
	          <add key="AzureOpenAIKey" value="AzureOpenAI KEY" />  
	          <add key="AzureOpenAIModel" value="AzureOpenAI Model Name" />
	      </appSettings>
      </configuration>
   ~~~

   - Add a reference to the System.Configuration assembly to the program.cs file

4. Using the steps outlined in the **5 Simple steps** build and test your first app.
   - If needed you can take a look at the soltion to see a fishined project, but ideally you should put hands on keyboard and work through any issues you come across. Below is your homework for week 1.



# Homework - Lesson 1
Your homework for Lesson 1 is a follows:

- [Read the overview on Semantic Kernel and understand why a framework is needed](https://learn.microsoft.com/en-us/semantic-kernel/overview/) 

- In the latest version of Semantic Kernel is there such a thing as skills? Do some research to see if you can figuer out the answer before looking at the answer.
   <details>
    <summary><u>Answer</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

      
     No. [Skills have been replaced with Plugins](https://devblogs.microsoft.com/semantic-kernel/road-to-v1-0-for-the-python-semantic-kernel-sdk)
          
  </details>

- What are the fundimental steps needed for a Chat implementation using Semantic Kernel?
  <details>
    <summary><u>Answer</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->
      
     1. Create a Kernel Builder so you can construct Kernel instances
   
     2. Load the AI Endpoint values so you can access the REST endpoint
   
     3. Add the Chat Completion Service with the Endpoint details
   
     4. Construct the Kernel, Prompt / Chat History, get an instance to the Completion Service
   
     5. Send the Prompt / Chat History and get a response
  </details>

- Build your first Semantic Kernel Console App.
  <details>
    <summary><u>Tips</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->
      1. Follow the steps outlined in Lesson 1.
      
     If you need a shortcut you can take look at the Lesson 1 Project found in the [Semantic-Kernel-101.sln file](/solutions/Semantic-Kernel-101/README.md).
      
     **Hint:** Kernel.CreateBuilder, builder.Services.AddAzureOpenAIChatCompletion, builder.Build, kernel.GetRequiredService<IChatCompletionService>(), chatCompletionService.GetChatMessageContentAsync, history.AddAssistantMessage      
  </details>
 
- RAG (Retrieval Augmented Generation) and Generative AI

  The RAG pattern is one of the most important patterns in use today.
     Retrieval  = Retrieve inforamtion from a data source
     Augment    = Inject the retrieved data into the prompt
     Generation = Allow the LLM to generation responses based on the retrieved data
  <details>
    <summary><u>Read and understand RAG with AI Search</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

      
     [RAG with AI Search](https://learn.microsoft.com/en-us/azure/search/retrieval-augmented-generation-overview)
          
  </details>

- Prompt Template Language

  The Semantic Kernel prompt template language is a simple way to define and compose AI functions using plain text. You can use it to create natural language prompts, generate responses, 
  extract information, invoke other prompts or perform any other task that can be expressed with text.
  <details>
    <summary><u>Study the Syntax</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

      
     [Prompt Template Syntax](https://learn.microsoft.com/en-us/semantic-kernel/prompts/prompt-template-syntax)
          
  </details>

- Creating Functions from Prompts and adding variables

  The Semantic Kernel prompt template language is very powerful, you can create tokens that will automatically be replace with input parameters.  Read and study up on this topic as it will be used in Lesson 2!
  <details>
    <summary><u>Study the Syntax</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

      
     [Example of templatizing prompts and using variables](https://learn.microsoft.com/en-us/semantic-kernel/prompts/templatizing-prompts?tabs=Csharp))
          
  </details>
