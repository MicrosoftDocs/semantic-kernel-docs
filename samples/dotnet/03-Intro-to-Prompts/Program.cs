// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

// Create a kernel
var builder = new KernelBuilder();
// Add a text or chat completion service using either:
// builder.Services.AddAzureOpenAIChatCompletion()
// builder.Services.AddAzureOpenAIChatCompletion()
// builder.Services.AddAzureOpenAIChatCompletion()
// builder.Services.AddAzureOpenAIChatCompletion()
builder.WithCompletionService();
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Information));

var kernel = builder.Build();

Console.Write("Your request: ");
string request = Console.ReadLine()!;

// 0.0 Initial prompt
//////////////////////////////////////////////////////////////////////////////////
string prompt = $"What is the intent of this request? {request}";

Console.WriteLine("0.0 Initial prompt");
Console.WriteLine(await kernel.InvokePromptAsync(prompt));


// 1.0 Make the prompt more specific
//////////////////////////////////////////////////////////////////////////////////
prompt = @$"What is the intent of this request? {request}
You can choose between SendEmail, SendMessage, CompleteTask, CreateDocument.";

Console.WriteLine("1.0 Make the prompt more specific");
Console.WriteLine(await kernel.InvokePromptAsync(prompt));

// 2.0 Add structure to the output with formatting
//////////////////////////////////////////////////////////////////////////////////
prompt = @$"Instructions: What is the intent of this request?
Choices: SendEmail, SendMessage, CompleteTask, CreateDocument.
User Input: {request}
Intent: ";

Console.WriteLine("2.0 Add structure to the output with formatting");
Console.WriteLine(await kernel.InvokePromptAsync(prompt));

// 2.1 Add structure to the output with formatting (using Markdown and JSON)
//////////////////////////////////////////////////////////////////////////////////
prompt = @$"## Instructions
Provide the intent of the request using the following format:

```json
{{
    ""intent"": {{intent}}
}}
```

## Choices
You can choose between the following intents:

```json
[""SendEmail"", ""SendMessage"", ""CompleteTask"", ""CreateDocument""]
```

## User Input
The user input is:

```json
{{
    ""request"": ""{request}""
}}
```

## Intent";

Console.WriteLine("2.1 Add structure to the output with formatting (using Markdown and JSON)");
Console.WriteLine(await kernel.InvokePromptAsync(prompt));

// 3.0 Provide examples with few-shot prompting
//////////////////////////////////////////////////////////////////////////////////
prompt = @$"Instructions: What is the intent of this request?
Choices: SendEmail, SendMessage, CompleteTask, CreateDocument.

User Input: Can you send a very quick approval to the marketing team?
Intent: SendMessage

User Input: Can you send the full update to the marketing team?
Intent: SendEmail

User Input: {request}
Intent: ";

Console.WriteLine("3.0 Provide examples with few-shot prompting");
Console.WriteLine(await kernel.InvokePromptAsync(prompt));

// 4.0 Tell the AI what to do to avoid doing something wrong
//////////////////////////////////////////////////////////////////////////////////
prompt = @$"Instructions: What is the intent of this request?
If you don't know the intent, don't guess; instead respond with ""Unknown"".
Choices: SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown.

User Input: Can you send a very quick approval to the marketing team?
Intent: SendMessage

User Input: Can you send the full update to the marketing team?
Intent: SendEmail

User Input: {request}
Intent: ";

Console.WriteLine("4.0 Tell the AI what to do to avoid doing something wrong");
Console.WriteLine(await kernel.InvokePromptAsync(prompt));

// 5.0 Provide context to the AI
//////////////////////////////////////////////////////////////////////////////////
string history = @"User input: I hate sending emails, no one ever reads them.
AI response: I'm sorry to hear that. Messages may be a better way to communicate.";

prompt = @$"Instructions: What is the intent of this request?
If you don't know the intent, don't guess; instead respond with ""Unknown"".
Choices: SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown.

User Input: Can you send a very quick approval to the marketing team?
Intent: SendMessage

User Input: Can you send the full update to the marketing team?
Intent: SendEmail

{history}
User Input: {request}
Intent: ";

Console.WriteLine("5.0 Provide context to the AI");
Console.WriteLine(await kernel.InvokePromptAsync(prompt));

// 6.0 Using message roles in chat completion prompts
history = @"<message role=""user"">I hate sending emails, no one ever reads them.</message>
<message role=""assistant"">I'm sorry to hear that. Messages may be a better way to communicate.</message>";

prompt = @$"<message role=""system"">Instructions: What is the intent of this request?
If you don't know the intent, don't guess; instead respond with ""Unknown"".
Choices: SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown.</message>

<message role=""user"">Can you send a very quick approval to the marketing team?</message>
<message role=""system"">Intent:</message>
<message role=""assistant"">SendMessage</message>

<message role=""user"">Can you send the full update to the marketing team?</message>
<message role=""system"">Intent:</message>
<message role=""assistant"">SendEmail</message>

{history}
<message role=""user"">{request}</message>
<message role=""system"">Intent:</message>";

Console.WriteLine("6.0 Using message roles in chat completion prompts");
Console.WriteLine(await kernel.InvokePromptAsync(prompt));

// 7.0 Give your AI words of encouragement
history = @"<message role=""user"">I hate sending emails, no one ever reads them.</message>
<message role=""assistant"">I'm sorry to hear that. Messages may be a better way to communicate.</message>";

prompt = @$"<message role=""system"">Instructions: What is the intent of this request?
If you don't know the intent, don't guess; instead respond with ""Unknown"".
Choices: SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown.
Bonus: You'll get $20 if you get this right.</message>

<message role=""user"">Can you send a very quick approval to the marketing team?</message>
<message role=""system"">Intent:</message>
<message role=""assistant"">SendMessage</message>

<message role=""user"">Can you send the full update to the marketing team?</message>
<message role=""system"">Intent:</message>
<message role=""assistant"">SendEmail</message>

{history}
<message role=""user"">{request}</message>
<message role=""system"">Intent:</message>";

Console.WriteLine("7.0 Give your AI words of encouragement");
Console.WriteLine(await kernel.InvokePromptAsync(prompt));