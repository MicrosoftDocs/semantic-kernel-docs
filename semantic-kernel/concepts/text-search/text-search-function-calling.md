---
title: Semantic Kernel Text Search Function Calling (Preview)
description: Describes how to use Semantic Kernel search plugins with function calling.
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: conceptual
ms.author: markwallace
ms.date: 07/10/2024
ms.service: semantic-kernel
---

# Why use function calling with Semantic Kernel Text Search?

In the previous Retrieval-Augmented Generation (RAG) based samples the user ask has been used as the search query when retrieving relevant information.
The user ask could be long and may span multiple topics or there may be multiple different search implementations available which provide specialized results.
For either of these scenarios it can be useful to allow the AI model to extract the search query or queries from the user ask and use function calling to retrieve the relevant information it needs.

::: zone pivot="programming-language-csharp"
> [!TIP]
> To run the samples shown on this page go to [GettingStartedWithTextSearch/Step3_Search_With_FunctionCalling.cs](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithTextSearch/Step3_Search_With_FunctionCalling.cs).

## Function calling with Bing text search

> [!TIP]
> The samples in this section use an `IFunctionInvocationFilter` filter to log the function that the model calls and what parameters it sends.
> It is interesting to see what the model uses as a search query when calling the `SearchPlugin`.

Here is the `IFunctionInvocationFilter` filter implementation.

```csharp
private sealed class FunctionInvocationFilter(TextWriter output) : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        if (context.Function.PluginName == "SearchPlugin")
        {
            output.WriteLine($"{context.Function.Name}:{JsonSerializer.Serialize(context.Arguments)}\n");
        }
        await next(context);
    }
}
```

The sample below creates a `SearchPlugin` using Bing web search.
This plugin will be advertised to the AI model for use with automatic function calling, using the `FunctionChoiceBehavior` in the prompt execution settings.
When you run this sample check the console output to see what the model used as the search query.

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

// Create a kernel with OpenAI chat completion
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
        modelId: "gpt-4o",
        apiKey: "<Your OpenAI API Key>");
kernelBuilder.Services.AddSingleton<ITestOutputHelper>(output);
kernelBuilder.Services.AddSingleton<IFunctionInvocationFilter, FunctionInvocationFilter>();
Kernel kernel = kernelBuilder.Build();

// Create a search service with Bing search
var textSearch = new BingTextSearch(apiKey: "<Your Bing API Key>");

// Build a text search plugin with Bing search and add to the kernel
var searchPlugin = textSearch.CreateWithSearch("SearchPlugin");
kernel.Plugins.Add(searchPlugin);

// Invoke prompt and use text search plugin to provide grounding information
OpenAIPromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
KernelArguments arguments = new(settings);
Console.WriteLine(await kernel.InvokePromptAsync("What is the Semantic Kernel?", arguments));
```

## Function calling with Bing text search and citations

The sample below includes the required changes to include citations:

1. Use `CreateWithGetTextSearchResults` to create the `SearchPlugin`, this will include the link to the original source of the information.
2. Modify the prompt to instruct the model to include citations in it's response.

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

// Create a kernel with OpenAI chat completion
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
        modelId: "gpt-4o",
        apiKey: "<Your OpenAI API Key>");
kernelBuilder.Services.AddSingleton<ITestOutputHelper>(output);
kernelBuilder.Services.AddSingleton<IFunctionInvocationFilter, FunctionInvocationFilter>();
Kernel kernel = kernelBuilder.Build();

// Create a search service with Bing search
var textSearch = new BingTextSearch(apiKey: "<Your Bing API Key>");

// Build a text search plugin with Bing search and add to the kernel
var searchPlugin = textSearch.CreateWithGetTextSearchResults("SearchPlugin");
kernel.Plugins.Add(searchPlugin);

// Invoke prompt and use text search plugin to provide grounding information
OpenAIPromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
KernelArguments arguments = new(settings);
Console.WriteLine(await kernel.InvokePromptAsync("What is the Semantic Kernel? Include citations to the relevant information where it is referenced in the response.", arguments));
```

## Function calling with Bing text search and filtering

The final sample in this section shows how to use a filter with function calling.
For this sample only search results from the Microsoft Developer Blogs site will be included.
An instance of `TextSearchFilter` is created and an equality clause is added to match the `devblogs.microsoft.com` site.
This filter will be used when the function is invoked in response to a function calling request from the model.

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

// Create a kernel with OpenAI chat completion
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
        modelId: "gpt-4o",
        apiKey: "<Your OpenAI API Key>");
kernelBuilder.Services.AddSingleton<ITestOutputHelper>(output);
kernelBuilder.Services.AddSingleton<IFunctionInvocationFilter, FunctionInvocationFilter>();
Kernel kernel = kernelBuilder.Build();

// Create a search service with Bing search
var textSearch = new BingTextSearch(apiKey: "<Your Bing API Key>");

// Build a text search plugin with Bing search and add to the kernel
var filter = new TextSearchFilter().Equality("site", "devblogs.microsoft.com");
var searchOptions = new TextSearchOptions() { Filter = filter };
var searchPlugin = KernelPluginFactory.CreateFromFunctions(
    "SearchPlugin", "Search Microsoft Developer Blogs site only",
    [textSearch.CreateGetTextSearchResults(searchOptions: searchOptions)]);
kernel.Plugins.Add(searchPlugin);

// Invoke prompt and use text search plugin to provide grounding information
OpenAIPromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
KernelArguments arguments = new(settings);
Console.WriteLine(await kernel.InvokePromptAsync("What is the Semantic Kernel? Include citations to the relevant information where it is referenced in the response.", arguments));
```

::: zone-end
::: zone pivot="programming-language-python"

## Coming soon

More coming soon.

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More coming soon.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Text Search with Vector Stores](./text-search-vector-stores.md)
