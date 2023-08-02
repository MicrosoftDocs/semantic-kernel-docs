---
title: How to add an LLM prompt in Semantic Kernel
description: Learn how to add semantic functions to a plugin in Semantic Kernel.
author: matthewbolanos
ms.topic: conceptual
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---


# Creating semantic functions

[!INCLUDE [pat_large.md](../includes/pat_large.md)]


In previous articles, we demonstrated [how to load a semantic function](./index.md#importing-and-registering-plugins). We also showed how to run the function either [by itself](./index.md#running-a-function-from-a-plugin) or [in a chain](./index.md#chaining-functions-within-plugins). In both cases, we used out-of-the-box sample functions that are included with Semantic Kernel to demonstrate the process.

In this article, we'll demonstrate how to actually _create_ a semantic function so you can easily import them into Semantic Kernel. As an example in this article, we will demonstrate how to create a semantic function that gathers the intent of the user. This semantic function will be called `GetIntent` and will be part of a plugin called `OrchestratorPlugin`.

By following this example, you'll learn how to create a semantic function that can use multiple context variables and functions to elicit an AI response. If you want to see the final solution, you can check out the following samples in the public documentation repository.

| Language  | Link to final solution |
| --- | --- |
| C# | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/01-Semantic-Functions) |
| Python | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/01-Semantic-Functions) |


> [!Note]
> Skills are currently being renamed to plugins. This article has been updated to reflect the latest terminology, but some images and code samples may still refer to skills.


> [!TIP]
> We recommend using the [Semantic Kernel Tools](../vs-code-tools/index.md) extension for Visual Studio Code to help you create semantic functions. This extension provides an easy way to create and test functions directly from within VS Code.
> [:::image type="content" source="../media/semantic-kernel-tools-install.png" alt-text="Semantic Kernel Tools Extension":::](https://marketplace.visualstudio.com/items?itemName=ms-semantic-kernel.semantic-kernel)

## Creating a home for your semantic functions
Before creating the `OrchestratorPlugin` or the `GetIntent` function, you must first define a folder that will hold all of your plugins. This will make it easier to import them into Semantic Kernel later. We recommend putting this folder at the root of your project and calling it _plugins_.

Within your _plugins_ folder, you can then create a folder called _OrchestratorPlugin_ for your plugin and a nested folder called _GetIntent_ for your function.

```directory
Plugins
│
└─── OrchestratorPlugin
     |
     └─── GetIntent
```

To see a more complete example of a plugins directory, check out the [Semantic Kernel sample plugins](https://github.com/microsoft/semantic-kernel/tree/main/samples/skills) folder in the GitHub repository.

## Creating the files for your semantic function
Once inside of a semantic functions folder, you'll need to create two files: _skprompt.txt_ and _config.json_. The _skprompt.txt_ file contains the prompt that will be sent to the AI service and the _config.json_ file contains the configuration along with semantic descriptions used by planner.

Go ahead and create these two files in the _GetIntent_ folder. In the following sections, we'll walk through how to configure them.

```directory
Plugins
│
└─── OrchestratorPlugin
     |
     └─── GetIntent
          |
          └─── config.json
          └─── skprompt.txt
```

### Writing a prompt in the _skprompt.txt_ file
The _skprompt.txt_ file contains the request that will be sent to the AI service. The [prompt engineering](../prompt-engineering/index.md) section of the documentation provides a detailed overview of how to write prompts, but at a high level, prompts are requests written in natural language that are sent to an AI service.

In most cases, you'll send your prompt to a text or chat completion service which will return back a response that attempts to complete the prompt. For example, if you send the prompt `I want to go to the `, the AI service might return back `beach`. This is a very simple example, but it demonstrates the basic idea of how prompts work.

In the case of the `GetIntent` function, we want to create a prompt that asks the AI service what the intent of a user is. The following prompt will do just that:

```txt
Bot: How can I help you?
User: {{$input}}

---------------------------------------------

The intent of the user in 5 words or less: 
```

Notice that we're using a variable called `$input` in the prompt. This variable is later defined in the _config.json_ file and is used to pass the user's input to the AI service.

Go ahead and copy the prompt above and save it in the _skprompt.txt_ file.

### Configuring the function in the _config.json_ file
Next, we need to define the configuration for the `GetIntent` function. The [configuring prompts](../prompt-engineering/configure-prompts.md) article provides a detailed overview of how to configure prompts, but at a high level, prompts are configured using a JSON file that contains the following properties:

- `type` – The type of prompt. In this case, we're using the `completion` type.
- `description` – A description of what the prompt does. This is used by planner to automatically orchestrate plans with the function.
- `completion` – The settings for completion models. For OpenAI models, this includes the `max_tokens` and `temperature` properties.
- `input` – Defines the variables that are used inside of the prompt (e.g., `$input`).

For the `GetIntent` function, we'll use the following configuration:

```json
{
     "schema": 1,
     "type": "completion",
     "description": "Gets the intent of the user.",
     "completion": {
          "max_tokens": 500,
          "temperature": 0.0,
          "top_p": 0.0,
          "presence_penalty": 0.0,
          "frequency_penalty": 0.0
     },
     "input": {
          "parameters": [
               {
                    "name": "input",
                    "description": "The user's request.",
                    "defaultValue": ""
               }
          ]
     }
}
```

Copy the configuration above and save it in the _config.json_ file.

### Testing your semantic function
At this point, you can import and test your function with the kernel by using the following code.

# [C#](#tab/Csharp)

```csharp
var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "path", "to", "your", "plugins", "folder");

// Import the OrchestratorPlugin from the plugins directory.
var orchestratorPlugin = kernel
     .ImportSemanticSkillFromDirectory(pluginsDirectory, "OrchestratorPlugin");

// Get the GetIntent function from the OrchestratorPlugin and run it
var result = await orchestratorPlugin["GetIntent"]
     .InvokeAsync("I want to send an email to the marketing team celebrating their recent milestone.");

Console.WriteLine(result);
```

# [Python](#tab/python)

```python
plugins_directory = "<path to your plugins folder>"

# Import the OrchestratorPlugin from the plugins directory.
orchestrator_plugin = kernel.import_semantic_skill_from_directory(plugins_directory, "OrchestratorPlugin")

# Get the GetIntent function from the OrchestratorPlugin.
get_intent_function = orchestrator_plugin["GetIntent"]

# Run the GetIntent function.
result = await get_intent_function.invoke_async("I want to send an email to the marketing team celebrating their recent milestone.")

print(result)
```

---

You should get an output that looks like the following:

```output
Send congratulatory email.
```

## Making your semantic function more robust
While our function works, it's not very useful when combined with native code. For example, if we had several native functions available to run based on an intent, it would be difficult to use the output of the `GetIntent` function to choose which native function to actually run.

We need to find a way to constrain the output of our function so that we can use the output in a switch statement.

### Templatizing a semantic function
One way to constrain the output of a semantic function is to provide a list of options for it to choose from. A naive approach would be to hard code these options into the prompt, but this would be difficult to maintain and would not scale well. Instead, we can use Semantic Kernel's templating language to dynamically generate the prompt.

The [prompt template syntax](../prompt-engineering/prompt-template-syntax.md) article in the [prompt engineering](../prompt-engineering/index.md) section of the documentation provides a detailed overview of how to use the templating language. In this article, we'll show you just enough to get started.

:::row:::
   :::column span="2":::
      The following prompt uses the `{{$options}}` variable to provide a list of options for the LLM to choose from. We've also added a `{{$history}}` variable to the prompt so that the previous conversation is included.
      
      By including these variables, we are able to help the LLM choose the correct intent by allowing it to leverage variables within the Semantic Kernel context object.
   :::column-end:::
   :::column span="3":::
      ![Consuming context within a semantic function](../media/using-context-in-templates.png)
   :::column-end:::
:::row-end:::


```txt
{{$history}}
User: {{$input}}

---------------------------------------------

Provide the intent of the user. The intent should be one of the following: {{$options}}

INTENT: 
```

When you add a new variable to the prompt, you also should update the _config.json_ file to include the new variable. While these properties aren't used now, it's good to get into the practice of adding them so that they can be used by the [planner](./planner.md) later. The following configuration adds the `$options` and `$history` variable to the `input` section of the configuration.

```json
{
     "schema": 1,
     "type": "completion",
     "description": "Gets the intent of the user.",
     "completion": {
          "max_tokens": 500,
          "temperature": 0.0,
          "top_p": 0.0,
          "presence_penalty": 0.0,
          "frequency_penalty": 0.0
     },
     "input": {
          "parameters": [
               {
                    "name": "input",
                    "description": "The user's request.",
                    "defaultValue": ""
               },
               {
                    "name": "history",
                    "description": "The history of the conversation.",
                    "defaultValue": ""
               },
               {
                    "name": "options",
                    "description": "The options to choose from.",
                    "defaultValue": ""
               }
          ]
     }
}
```

You can now update your code to provide a list of options to the `GetIntent` function by using context.

# [C#](#tab/Csharp)

```csharp
// Import the OrchestratorPlugin from the plugins directory.
var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "path", "to", "your", "plugins", "folder");
var orchestrationPlugin = kernel
     .ImportSemanticSkillFromDirectory(pluginsDirectory, "OrchestratorPlugin");

// Create a new context and set the input, history, and options variables.
var context = kernel.CreateNewContext();
context["input"] = "Yes";
context["history"] = @"Bot: How can I help you?
User: My team just hit a major milestone and I would like to send them a message to congratulate them.
Bot:Would you like to send an email?";
context["options"] = "SendEmail, ReadEmail, SendMeeting, RsvpToMeeting, SendChat";

// Run the GetIntent function with the context.
var result = await orchestrationPlugin["GetIntent"].InvokeAsync(context);

Console.WriteLine(result);
```

# [Python](#tab/python)

```python
plugins_directory = "<path to your plugins folder>"

# Import the OrchestratorPlugin from the plugins directory.
orchestrator_plugin = kernel.import_semantic_skill_from_directory(plugins_directory, "OrchestratorPlugin")
get_intent_function = orchestrator_plugin["GetIntent"]

# Create a new context and set the input, history, and options variables.
context = kernel.create_new_context()
context["input"] = "Yes"
context["history"] = """Bot: How can I help you?
User: My team just hit a major milestone and I would like to send them a message to congratulate them.
Bot:Would you like to send an email?"""
context["options"] = "SendEmail, ReadEmail, SendMeeting, RsvpToMeeting, SendChat"

# Run the GetIntent function with the context.
result = get_intent_function(context=context)

print(result)
```

---

Now, instead of getting an output like `Send congratulatory email.`, we'll get an output like `SendEmail`. This output could then be used within a switch statement in native code to run the correct function.

### Calling functions _within_ a semantic function
We now have a more useful semantic function, but you might run into token limits if you had a long list of options and a long conversation history. To get around this, we can call other functions within our semantic function to help break up the prompt into smaller pieces.

To learn more about calling functions within a semantic function, see the [calling functions within a semantic function](../prompt-engineering/prompt-template-syntax.md#function-calls) section in the [prompt engineering](../prompt-engineering/index.md) section of the documentation.

The following prompt uses the `Summarize` function in the [`SummarizeSkill` plugin](https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/SummarizeSkill) to summarize the conversation history before asking for the intent.

```txt
{{SummarizeSkill.Summarize $history}}
User: {{$input}}

---------------------------------------------

Provide the intent of the user. The intent should be one of the following: {{$options}}

INTENT: 
```

You can now update your code to load the `SummarizeSkill` plugin so the kernel can find the `Summarize` function.

# [C#](#tab/Csharp)

```csharp
var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "path", "to", "your", "plugins", "folder");

// Import the OrchestratorPlugin and SummarizeSkill from the plugins directory.
var orchestrationPlugin = kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "OrchestratorPlugin");
var summarizationPlugin = kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "SummarizeSkill");

// Create a new context and set the input, history, and options variables.
var context = kernel.CreateNewContext();
context["input"] = "Yes";
context["history"] = @"Bot: How can I help you?
User: My team just hit a major milestone and I would like to send them a message to congratulate them.
Bot:Would you like to send an email?";
context["options"] = "SendEmail, ReadEmail, SendMeeting, RsvpToMeeting, SendChat";

// Run the Summarize function with the context.
var result = await orchestrationPlugin["GetIntent"].InvokeAsync(context);

Console.WriteLine(result);
```

# [Python](#tab/python)

```python
plugins_directory = "<path to your plugins folder>"

# Import the OrchestratorPlugin and SummarizeSkill from the plugins directory.
orchestrator_plugin = kernel.import_semantic_skill_from_directory(plugins_directory, "OrchestratorPlugin")
summarization_plugin = kernel.import_semantic_skill_from_directory(plugins_directory, "SummarizeSkill")
get_intent_function = orchestrator_plugin["GetIntent"]

# Create a new context and set the input, history, and options variables.
context = kernel.create_new_context()
context["input"] = "Yes"
context["history"] = """Bot: How can I help you?
User: My team just hit a major milestone and I would like to send them a message to congratulate them.
Bot:Would you like to send an email?"""
context["options"] = "SendEmail, ReadEmail, SendMeeting, RsvpToMeeting, SendChat"

# Run the Summarize function with the context.
result = get_intent_function(context=context)

print(result)
```

---

## Take the next step
Now that you can create a semantic function, you can now learn how to [create a native function](./native-functions.md).

> [!div class="nextstepaction"]
> [Create a native function](./native-functions.md)