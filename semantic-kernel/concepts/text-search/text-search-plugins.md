---
title: Semantic Kernel Text Search Plugins (Preview)
description: Describes how to create and use Semantic Kernel text search plugins. Includes samples showing how to use filters and how to create a custom search plugin.
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: conceptual
ms.author: markwallace
ms.date: 07/10/2024
ms.service: semantic-kernel
---

# What are Semantic Kernel Text Search plugins?

Semantic Kernel uses [Plugins](../plugins/index.md) to connect existing APIs with AI.
These Plugins have functions that can be used to add relevant data or examples to prompts, or to allow the AI to perform actions automatically.

To integrate Text Search with Semantic Kernel, we need to turn it into a Plugin.
Once we have a Text Search plugin, we can use it to add relevant information to prompts or to retrieve information as needed.
Creating a plugin from Text Search is a simple process, which we will explain below.

::: zone pivot="programming-language-csharp"

> [!TIP]
> To run the samples shown on this page go to [GettingStartedWithTextSearch/Step2_Search_For_RAG.cs](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithTextSearch/Step2_Search_For_RAG.cs).

## Basic search plugin

Semantic Kernel provides a default template implementation that supports variable substitution and function calling.
By including an expression such as `{{MyPlugin.Function $arg1}}` in a prompt template, the specified function i.e., `MyPlugin.Function` will be invoked with the provided argument `arg1` (which is resolved from `KernelArguments`).
The return value from the function invocation is inserted into the prompt. This technique can be used to inject relevant information into a prompt.

The sample below shows how to create a plugin named `SearchPlugin` from an instance of `BingTextSearch`.
Using `CreateWithSearch` creates a new plugin with a single `Search` function that calls the underlying text search implementation.
The `SearchPlugin` is added to the `Kernel` which makes it available to be called during prompt rendering.
The prompt template includes a call to `{{SearchPlugin.Search $query}}` which will invoke the `SearchPlugin` to retrieve results related to the current query.
The results are then inserted into the rendered prompt before it is sent to the AI model.

```csharp
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

// Create a kernel with OpenAI chat completion
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
        modelId: "gpt-4o",
        apiKey: "<Your OpenAI API Key>");
Kernel kernel = kernelBuilder.Build();

// Create a text search using Bing search
var textSearch = new BingTextSearch(apiKey: "<Your Bing API Key>");

// Build a text search plugin with Bing search and add to the kernel
var searchPlugin = textSearch.CreateWithSearch("SearchPlugin");
kernel.Plugins.Add(searchPlugin);

// Invoke prompt and use text search plugin to provide grounding information
var query = "What is the Semantic Kernel?";
var prompt = "{{SearchPlugin.Search $query}}. {{$query}}";
KernelArguments arguments = new() { { "query", query } };
Console.WriteLine(await kernel.InvokePromptAsync(prompt, arguments));
```

## Search plugin with citations

The sample below repeats the pattern described in the previous section with a few notable changes:

1. `CreateWithGetTextSearchResults` is used to create a `SearchPlugin` which calls the `GetTextSearchResults` method from the underlying text search implementation.
2. The prompt template uses Handlebars syntax. This allows the template to iterate over the search results and render the name, value and link for each result.
3. The prompt includes an instruction to include citations, so the AI model will do the work of adding citations to the response.

```csharp
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

// Create a kernel with OpenAI chat completion
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
        modelId: "gpt-4o",
        apiKey: "<Your OpenAI API Key>");
Kernel kernel = kernelBuilder.Build();

// Create a text search using Bing search
var textSearch = new BingTextSearch(apiKey: "<Your Bing API Key>");

// Build a text search plugin with Bing search and add to the kernel
var searchPlugin = textSearch.CreateWithGetTextSearchResults("SearchPlugin");
kernel.Plugins.Add(searchPlugin);

// Invoke prompt and use text search plugin to provide grounding information
var query = "What is the Semantic Kernel?";
string promptTemplate = """
{{#with (SearchPlugin-GetTextSearchResults query)}}  
    {{#each this}}  
    Name: {{Name}}
    Value: {{Value}}
    Link: {{Link}}
    -----------------
    {{/each}}  
{{/with}}  

{{query}}

Include citations to the relevant information where it is referenced in the response.
""";
KernelArguments arguments = new() { { "query", query } };
HandlebarsPromptTemplateFactory promptTemplateFactory = new();
Console.WriteLine(await kernel.InvokePromptAsync(
    promptTemplate,
    arguments,
    templateFormat: HandlebarsPromptTemplateFactory.HandlebarsTemplateFormat,
    promptTemplateFactory: promptTemplateFactory
));
```

## Search plugin with a filter

The samples shown so far will use the top ranked web search results to provide the grounding data. To provide more reliability in the data the web search can be restricted to only return results from a specified site.

The sample below builds on the previous one to add filtering of the search results.
A `TextSearchFilter` with an equality clause is used to specify that only results from the Microsoft Developer Blogs site (`site == 'devblogs.microsoft.com'`) are to be included in the search results.

The sample uses `KernelPluginFactory.CreateFromFunctions` to create the `SearchPlugin`.
A custom description is provided for the plugin.
The `ITextSearch.CreateGetTextSearchResults` extension method is used to create the `KernelFunction` which invokes the text search service.

> [!TIP]
> The `site` filter is Bing specific. For Google web search use `siteSearch`.

```csharp
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

// Create a kernel with OpenAI chat completion
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
        modelId: "gpt-4o",
        apiKey: "<Your OpenAI API Key>");
Kernel kernel = kernelBuilder.Build();

// Create a text search using Bing search
var textSearch = new BingTextSearch(apiKey: "<Your Bing API Key>");

// Create a filter to search only the Microsoft Developer Blogs site
var filter = new TextSearchFilter().Equality("site", "devblogs.microsoft.com");
var searchOptions = new TextSearchOptions() { Filter = filter };

// Build a text search plugin with Bing search and add to the kernel
var searchPlugin = KernelPluginFactory.CreateFromFunctions(
    "SearchPlugin", "Search Microsoft Developer Blogs site only",
    [textSearch.CreateGetTextSearchResults(searchOptions: searchOptions)]);
kernel.Plugins.Add(searchPlugin);

// Invoke prompt and use text search plugin to provide grounding information
var query = "What is the Semantic Kernel?";
string promptTemplate = """
{{#with (SearchPlugin-GetTextSearchResults query)}}  
    {{#each this}}  
    Name: {{Name}}
    Value: {{Value}}
    Link: {{Link}}
    -----------------
    {{/each}}  
{{/with}}  

{{query}}

Include citations to the relevant information where it is referenced in the response.
""";
KernelArguments arguments = new() { { "query", query } };
HandlebarsPromptTemplateFactory promptTemplateFactory = new();
Console.WriteLine(await kernel.InvokePromptAsync(
    promptTemplate,
    arguments,
    templateFormat: HandlebarsPromptTemplateFactory.HandlebarsTemplateFormat,
    promptTemplateFactory: promptTemplateFactory
));
```

> [!TIP]
> Follow the link for more information on how to [filter the answers that Bing returns](/bing/search-apis/bing-web-search/filter-answers#getting-results-from-a-specific-site).

## Custom search plugin

In the previous sample a static site filter was applied to the search operations.
What if you need this filter to be dynamic?

The next sample shows how you can perform more customization of the `SearchPlugin` so that the filter value can be dynamic.
The sample uses `KernelFunctionFromMethodOptions` to specify the following for the `SearchPlugin`:

- `FunctionName`: The search function is named `GetSiteResults` because it will apply a site filter if the query includes a domain.
- `Description`: The description describes how this specialized search function works.
- `Parameters`: The parameters include an additional optional parameter for the `site` so the domain can be specified.

Customizing the search function is required if you want to provide multiple specialized search functions.
In prompts you can use the function names to make the template more readable.
If you use function calling then the model will use the function name and description to select the best search function to invoke.

When this sample is executed, the response will use [techcommunity.microsoft.com](https://techcommunity.microsoft.com/) as the source for relevant data.

```csharp
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

// Create a kernel with OpenAI chat completion
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
        modelId: "gpt-4o",
        apiKey: "<Your OpenAI API Key>");
Kernel kernel = kernelBuilder.Build();

// Create a text search using Bing search
var textSearch = new BingTextSearch(apiKey: "<Your Bing API Key>");

// Build a text search plugin with Bing search and add to the kernel
var options = new KernelFunctionFromMethodOptions()
{
    FunctionName = "GetSiteResults",
    Description = "Perform a search for content related to the specified query and optionally from the specified domain.",
    Parameters =
    [
        new KernelParameterMetadata("query") { Description = "What to search for", IsRequired = true },
        new KernelParameterMetadata("top") { Description = "Number of results", IsRequired = false, DefaultValue = 5 },
        new KernelParameterMetadata("skip") { Description = "Number of results to skip", IsRequired = false, DefaultValue = 0 },
        new KernelParameterMetadata("site") { Description = "Only return results from this domain", IsRequired = false },
    ],
    ReturnParameter = new() { ParameterType = typeof(KernelSearchResults<string>) },
};
var searchPlugin = KernelPluginFactory.CreateFromFunctions("SearchPlugin", "Search specified site", [textSearch.CreateGetTextSearchResults(options)]);
kernel.Plugins.Add(searchPlugin);

// Invoke prompt and use text search plugin to provide grounding information
var query = "What is the Semantic Kernel?";
string promptTemplate = """
    {{#with (SearchPlugin-GetSiteResults query)}}  
        {{#each this}}  
        Name: {{Name}}
        Value: {{Value}}
        Link: {{Link}}
        -----------------
        {{/each}}  
    {{/with}}  

    {{query}}

    Only include results from techcommunity.microsoft.com. 
    Include citations to the relevant information where it is referenced in the response.
    """;
KernelArguments arguments = new() { { "query", query } };
HandlebarsPromptTemplateFactory promptTemplateFactory = new();
Console.WriteLine(await kernel.InvokePromptAsync(
    promptTemplate,
    arguments,
    templateFormat: HandlebarsPromptTemplateFactory.HandlebarsTemplateFormat,
    promptTemplateFactory: promptTemplateFactory
));
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
> [Text Search Function Calling](./text-search-function-calling.md)
> [Text Search with Vector Stores](./text-search-vector-stores.md)