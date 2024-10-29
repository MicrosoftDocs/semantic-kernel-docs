---
title: Semantic Kernel Text Search Abstractions (Preview)
description: Provides a detailed look at the Semantic Kernel Text Search abstractions.
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: conceptual
ms.author: markwallace
ms.date: 07/10/2024
ms.service: semantic-kernel
---

# Why are Text Search abstractions needed?

When dealing with text prompts or text content in chat history a common requirement is to provide additional relevant information related to this text.
This provides the AI model with relevant context which helps it to provide more accurate responses.
To meet this requirement the Semantic Kernel provides a Text Search abstraction which allows using text inputs from various sources, e.g. Web search engines, vector stores, etc., and provide results in a few standardized formats.

> [!NOTE]
> Search for image content or audio content is not currently supported.

::: zone pivot="programming-language-csharp"
## Text search abstraction

The Semantic Kernel text search abstractions provides three methods:

1. `Search`
2. `GetSearchResults`
3. `GetTextSearchResults`

### `Search`
Performs a search for content related to the specified query and returns string values representing the search results. `Search` can be used in the most basic use cases e.g., when augmenting a `semantic-kernel` format prompt template with search results. `Search` always returns just a single string value per search result so is not suitable if citations are required.

### `GetSearchResults`
Performs a search for content related to the specified query and returns search results in the format defined by the implementation. `GetSearchResults` returns the full search result as defined by the underlying search service. This provides the most versatility at the cost of tying your code to a specific search service implementation.

### `GetTextSearchResults`
Performs a search for content related to the specified query and returns a normalized data model representing the search results. This normalized data model includes a string value and optionally a name and link. `GetTextSearchResults` allows your code to be isolated from the a specific search service implementation, so the same prompt can be used with multiple different search services.

> [!TIP]
> To run the samples shown on this page go to [GettingStartedWithTextSearch/Step1_Web_Search.cs](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithTextSearch/Step1_Web_Search.cs).

The sample code below shows each of the text search methods in action.

```csharp
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

// Create an ITextSearch instance using Bing search
var textSearch = new BingTextSearch(apiKey: "<Your Bing API Key>");

var query = "What is the Semantic Kernel?";

// Search and return results
KernelSearchResults<string> searchResults = await textSearch.SearchAsync(query, new() { Top = 4 });
await foreach (string result in searchResults.Results)
{
    Console.WriteLine(result);
}

// Search and return results as BingWebPage items
KernelSearchResults<object> webPages = await textSearch.GetSearchResultsAsync(query, new() { Top = 4 });
await foreach (BingWebPage webPage in webPages.Results)
{
    Console.WriteLine($"Name:            {webPage.Name}");
    Console.WriteLine($"Snippet:         {webPage.Snippet}");
    Console.WriteLine($"Url:             {webPage.Url}");
    Console.WriteLine($"DisplayUrl:      {webPage.DisplayUrl}");
    Console.WriteLine($"DateLastCrawled: {webPage.DateLastCrawled}");
}

// Search and return results as TextSearchResult items
KernelSearchResults<TextSearchResult> textResults = await textSearch.GetTextSearchResultsAsync(query, new() { Top = 4 });
await foreach (TextSearchResult result in textResults.Results)
{
    Console.WriteLine($"Name:  {result.Name}");
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Link:  {result.Link}");
}
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
> [Text Search Plugins](./text-search-plugins.md)
> [Text Search Function Calling](./text-search-function-calling.md)
> [Text Search with Vector Stores](./text-search-vector-stores.md)
