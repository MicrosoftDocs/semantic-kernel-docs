---
title: Building Agents with Semantic Kernel
description: Learn about agents and how to build them with Semantic Kernel.
author: sophialagerkranspandey
ms.topic: overview
ms.author: sopand
ms.date: 07/11/2023
ms.service: semantic-kernel
ms.custom: build-2023, build-2023-dataai
---

# What are agents?

An agent is an artificial intelligence that can answer questions and automate processes for users. There's a wide spectrum of agents that can be built, ranging from simple chat bots to fully automated AI assistants. With Semantic Kernel, we provide you with the tools to build increasingly more sophisticated agents that don't require you to be an AI expert.

![Types of agents](../media/types-of-agents.png)

A copilot is a special type of agent that is meant to work side-by-side with a user. Unlike an agent, a copilot is _not_ meant to be fully automated. Instead, it is meant to help a user complete a task by providing suggestions and recommendations. For example, a copilot could be used to help a user write an email by providing suggestions for what to write next. The user can then choose to accept or reject the suggestion.

As you build more sophisticated agents, you can turn your copilot into a fully automated agent. A fully automated agent is an agent that can respond to system events to perform actions without explicit user input. Even autonomous agents should be designed to work alongside users though to ensure that they safely and effectively complete tasks on behalf of the user.

> [!TIP] 
> When you start your journey with Semantic Kernel, we recommend building a copilot first. This is because copilots are easier to build and safer to deploy because the user is always in control. Once you have a copilot, you can then turn it into a fully automated agent by removing the need for user input.

## Building your first agent
An agent is made up of three core building blocks: plugins, planners, and its persona. These building blocks are what allow an agent to retrieve information from the user or other systems, plan how to use that information, and use that information to respond to a user or perform an action.

![Plugins, planners, and persona](../media/plugins-planners-personas.png)

### A real-world example: writing an email

Let's take a common scenario, building a copilot that helps a user write and send an email. After getting instructions from a user, the copilot would need to generate a plan using the available plugins to complete the task. This plan would include steps like...

| Step | Description                           |
|------|---------------------------------------|
| 1    | Get the user's email address and name |
| 2    | Get the email address of the recipient|
| 3    | Get the topic of the email            |
| 4    | Generate the subject and body of the email |
| 5    | Review the email with the user        |
| 6    | Send the email                        |

To enable this scenario, we would need to create a plugin that can send emails, a planner that can generate a plan to write an email, and a persona that can interact with the user to get the necessary information.

The following sections will walk you through the conceptual building blocks and how to put them together to build your first agent. Afterwards, you can refer to the specific guides for each building block to learn more about how to build them.
- [Plugins](./plugins/index.md)
- [Planners](./planning.md)
- [Personas](./personas.md)

### Plugins: giving your agent skills
To generate the above plan, the copilot first needs the capabilities necessary to perform these steps. This is where plugins come in. Plugins allow you to give your agent skills via code. For example, you could create a plugin that sends emails, retrieves information from a database, asks for help, or even saves and retrieves memories from previous conversations.

In our example, we can build a simple plugin that sends emails using [native code](./plugins/adding-native-plugins.md). Our plugin just has a single function, `send_email`, that takes in the email address, subject, and body of the email. It would then use this information to send the email.

```csharp
public class EmailPlugin
{
    [KernelFunction("send_email")]
    [Description("Sends an email to a recipient.")]
    public async Task SendEmailAsync(
        Kernel kernel,
        List<string> recipientEmails,
        string subject,
        string body
    )
    {
        // Add logic to send an email using the recipientEmails, subject, and body
        // For now, we'll just print out a success message to the console
        Console.WriteLine("Email sent!");
    }
}
```

There are other ways to create plugins. For example, if you have a RestAPI that can send emails, you can automatically create a plugin using its [OpenAPI specification](./plugins/adding-openapi-plugins.md). To learn more about other ways to author plugins, see the [plugins section](./plugins/index.md).

### Planners: giving guidance to your agent
To actually use this plugin (and to wire them up with other steps), the copilot would then need to generate a plan. This is where planning comes in. Planning comes from the built-in ability of LLMs to determine how to iteratively complete a task.

In the past, special prompts were created by AI app developers to guide the AI in generating a plan that could be consumed by an SDK like Semantic Kernel. However, with the advent of LLMs, AIs can now generate plans directly from a conversation with a user with the aid of function calling.

As a result, planning with AIs with Semantic Kernel is now as easy as invoking a chat completion service with auto function calling enabled.

// TODO: Add example of planning with LLMs

To learn more about planning with Semantic Kernel, see the [planning article](./planning.md).

### Personas: giving your agent a job description
In most cases, using the built-in planning capabilities of LLMs with plugins is sufficient to building an agent, but as you build more domain-specific agents, you may want to add a persona to your agent.

A persona is the instructions that you provide your agent so they can more effectively perform the role you want them to play. At its simplest, the persona could instruct the AI to be polite, ask for clarification when needed, or role-play as a specific job title (e.g., a customer service representative).

For enterprise scenarios, however, you may want to provide more detailed instructions to your agent. For example, you may want to instruct your agent to follow specific rules, ask for approval before taking actions, or escalate to a human if the AI is unable to complete a task.

To provide a persona to your agent, simply pre-pend a system message to the chat history that describes the persona. The AI will then use this persona to guide its interactions with the user.

// TODO: Add example of providing a persona to an agent

To learn more about authoring effective personas, refer to the [personas article](./personas.md).

## Putting the pieces together
Now that we understand the core building blocks of an agent, we can now combine them together to build our first agent. To do so, we'll initialize our `Kernel` object with our plugins, planners, and persona. Afterwards, we'll use the `Kernel` object to generate a plan and then execute that plan.

```csharp
// Create the kernel
var builder = Kernel.CreateBuilder();
builder.Services.AddLogging(c => c.SetMinimumLevel(LogLevel.Trace).AddDebug());
builder.Services.AddChatCompletionService(kernelSettings);
builder.Plugins.AddFromType<AuthorEmailPlanner>();
builder.Plugins.AddFromType<EmailPlugin>();
Kernel kernel = builder.Build();

// Retrieve the chat completion service from the kernel
IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Create the chat history
ChatHistory chatMessages = new ChatHistory("""
You are a friendly assistant who likes to follow the rules. You will complete required steps
and request approval before taking any consequential actions. If the user doesn't provide
enough information for you to complete a task, you will keep asking questions until you have
enough information to complete the task.
""");

// Start the conversation
while (true)
{
    // Get user input
    System.Console.Write("User > ");
    chatMessages.AddUserMessage(Console.ReadLine()!);

    // Get the chat completions
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        FunctionCallBehavior = FunctionCallBehavior.AutoInvokeKernelFunctions
    };
    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
        chatMessages,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Stream the results
    string fullMessage = "";
    await foreach (var content in result)
    {
        if (content.Role.HasValue)
        {
            System.Console.Write("Assistant > ");
        }
        System.Console.Write(content.Content);
        fullMessage += content.Content;
    }
    System.Console.WriteLine();

    // Add the message from the agent to the chat history
    chatMessages.AddAssistantMessage(fullMessage);
}
```

## Trying out your agent
Now that we've completed our program, we can give it a test run. Below is a sample conversation with our agent while using gpt-4 model.

| Role                          | Message                                                                                                                               |
| ----------------------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| 🔵&nbsp;**User**                      | Can you help me write an email for my boss?                                                                                           |
| 🔴&nbsp;**Assistant**                 | Of course, I'd be happy to help you write an email to your boss. Could you please provide me with more details? Specifically, I need to know: <br> 1. The topic or purpose of the email. <br> 2. Your boss's email address. |
| 🔵&nbsp;**User**                      | I want to give her an update on last months sales. We broke a bunch of records that I want to share with her, but we did have a challenge selling the X4321 model. |
| 🔴&nbsp;**Assistant**                 | That sounds like a great topic for an email. Could you please provide me with your boss's email address so I can assist you further?  |
| 🔵&nbsp;**User**                      | Sure! It's sarah@contoso.com                                                                                                          |
| 🔴&nbsp;**Assistant**                 | Great! Here's a plan for your email: <br> 1. **Start with a Clear Subject Line and Greeting**: The subject line should be concise and informative, such as "Update on Last Month's Sales & X4321 Model Challenges". Start the email with a professional greeting, like "Dear Sarah,". <br> 2. **Highlight Achievements and Challenges in Separate Paragraphs**: Begin with the positive news about the record-breaking achievements. Use specific data and examples to illustrate the success. Then, transition to the challenges with the X4321 model. Be honest and clear about the issues, but also suggest potential solutions or steps being taken to address these challenges. <br> 3. **End with a Call to Action and Professional Closing**: If you need Sarah to take any actions, state this clearly towards the end of the email. This could be asking for her input, a meeting to discuss further, or any other relevant action. Close the email professionally with a phrase like "Best regards" or "Sincerely", followed by your name. <br> Remember to proofread the email for any grammatical errors or unclear statements before sending it. This will help ensure the message is professional and easy to understand. <br> Would you like me to draft the email for you based on these steps? |
| 🔵&nbsp;**User**                      | Yes please!                                                                                                                          |
| 🔴&nbsp;**Assistant**                 | Sure, here's a draft based on the steps we discussed: <br> **Subject**: Update on Last Month's Sales & X4321 Model Challenges <br> **Dear Sarah,** <br> I hope this email finds you well. I am writing to update you on our sales performance for the past month. <br> I am pleased to report that we broke several sales records last month. Our team's hard work and dedication have paid off, resulting in significant growth in our overall sales figures. [Here, you can add specific data or examples of the records broken] <br> However, we did face some challenges with the X4321 model. Despite our best efforts, the sales for this model did not meet our expectations. [Here, you can explain more about the challenges faced] <br> We are currently working on strategies to improve the sales of the X4321 model. [Here, you can mention any specific steps or plans you have to address the issue] <br> I would appreciate your input on this matter. If you would like, we can schedule a meeting to discuss this further. <br> Thank you for your continued support and guidance. <br> **Best regards,** <br> [Your Name] <br> Please review this draft and let me know if you would like to make any changes before I send it. |
| 🔵&nbsp;**User**                      | Can you add that we increased our revenue by 20%? You can also share that the X4321 didn't sell as well since most customers upgraded to the Y5000 model, but this isn't too bad since it's what helped increase our revenue. |
| 🔴&nbsp;**Assistant**                 | Absolutely, here's the updated draft: <br> **Subject**: Update on Last Month's Sales & X4321 Model Challenges <br> **Dear Sarah,** <br> I hope this email finds you well. I am writing to update you on our sales performance for the past month. <br> I am pleased to report that we broke several sales records last month. Our team's hard work and dedication have paid off, resulting in a significant 20% increase in our overall revenue. This growth is a testament to our team's commitment and the quality of our products. <br> However, we did face some challenges with the X4321 model. Despite our best efforts, the sales for this model did not meet our expectations. Interestingly, most customers upgraded to the Y5000 model, which contributed significantly to our increased revenue. While we aim to improve the sales of all our models, this shift in customer preference is not entirely negative. <br> We are currently analyzing this trend and working on strategies to leverage it for future sales. Your insights and suggestions would be greatly appreciated. <br> Thank you for your continued support and guidance. <br> **Best regards,** <br> **Stephen** <br> Please review this updated draft and let me know if you would like to make any changes before I send it. |
| 🔵&nbsp;**User**                      | Please sign it with Stephen and then you can go ahead and send it to Sarah                                                          |
| 🟢&nbsp;**Tool**                      | Email sent!                                                                                                                          |
| 🔴&nbsp;**Assistant**                 | The email has been sent to Sarah. If there's anything else you need assistance with, feel free to ask!                               |


## Next steps
Now that you've built your first agent, you can now deep dive into the different components of Semantic Kernel.

| Section                                     | Description                                                |
| ------------------------------------------- | ---------------------------------------------------------- |
| [Kernel](./kernel.md)                       | Setting up and using the core Kernel object                |
| [AI Services](./ai-services/index.md)       | Integration and usage of various AI services               |
| [Plugins](./plugins/index.md)               | Integrate native code and APIs with Semantic Kernel        |
| [Planners](./planning.md)                   | Enabling your AI agents to complete multi-step tasks       |
| [Personas](./personas.md)                   | Customize your AI agents to better collaborate with others |
| [Telemetry](./telemetry.md)                 | Monitoring and logging for performance and usage metrics   |
| [Hooks and filters](./hooks-and-filters.md) | Middleware components for extending functionality          |
| [Prompts](./prompts.md)                     | Templates and examples for creating effective prompts      |
