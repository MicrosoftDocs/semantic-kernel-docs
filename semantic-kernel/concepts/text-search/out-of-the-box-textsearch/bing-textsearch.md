---
title: Using the Semantic Kernel Bing text search (Preview)
description: Contains information on how to use a Semantic Kernel Text Search with Bing.
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: conceptual
ms.author: markwallace
ms.date: 10/21/2024
ms.service: semantic-kernel
---
# Using the Bing Text Search (Preview)

> [!WARNING]
> The Semantic Kernel Text Search functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

## Overview

The Bing Text Search implementation uses the [Bing Web Search API](https://www.microsoft.com/bing/apis/bing-web-search-api) to retrieve search results. You must provide your own Bing Search Api Key to use this component.

## Limitations

| Feature Area                      | Support                                                                               |
|-----------------------------------|---------------------------------------------------------------------------------------|
| Search API                        | [Bing Web Search API](https://www.microsoft.com/bing/apis/bing-web-search-api) only.  |
| Supported filter clauses          | Only "equal to" filter clauses are supported.                                         |
| Supported filter keys             | The [responseFilter](/bing/search-apis/bing-web-search/reference/query-parameters#responsefilter) query parameter and [advanced search keywords](https://support.microsoft.com/topic/advanced-search-keywords-ea595928-5d63-4a0b-9c6b-0b769865e78a) are supported. |

> [!TIP]
> Follow this link for more information on how to [filter the answers that Bing returns](/bing/search-apis/bing-web-search/filter-answers#getting-results-from-a-specific-site).
> Follow this link for more information on using [advanced search keywords](https://support.microsoft.com/topic/advanced-search-keywords-ea595928-5d63-4a0b-9c6b-0b769865e78a)

## Getting started

The sample below shows how to create a `BingTextSearch` and use it to perform a text search.

::: zone pivot="programming-language-csharp"

```csharp
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

// Create an ITextSearch instance using Bing search
var textSearch = new BingTextSearch(apiKey: "<Your Bing API Key>");

var query = "What is the Semantic Kernel?";

// Search and return results as a string items
KernelSearchResults<string> stringResults = await textSearch.SearchAsync(query, new() { Top = 4, Skip = 0 });
Console.WriteLine("--- String Results ---\n");
await foreach (string result in stringResults.Results)
{
    Console.WriteLine(result);
}

// Search and return results as TextSearchResult items
KernelSearchResults<TextSearchResult> textResults = await textSearch.GetTextSearchResultsAsync(query, new() { Top = 4, Skip = 4 });
Console.WriteLine("\n--- Text Search Results ---\n");
await foreach (TextSearchResult result in textResults.Results)
{
    Console.WriteLine($"Name:  {result.Name}");
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Link:  {result.Link}");
}

// Search and return s results as BingWebPage items
KernelSearchResults<object> fullResults = await textSearch.GetSearchResultsAsync(query, new() { Top = 4, Skip = 8 });
Console.WriteLine("\n--- Bing Web Page Results ---\n");
await foreach (BingWebPage result in fullResults.Results)
{
    Console.WriteLine($"Name:            {result.Name}");
    Console.WriteLine($"Snippet:         {result.Snippet}");
    Console.WriteLine($"Url:             {result.Url}");
    Console.WriteLine($"DisplayUrl:      {result.DisplayUrl}");
    Console.WriteLine($"DateLastCrawled: {result.DateLastCrawled}");
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
The following sections of the documentation show you how to:

1. Create a [plugin](../text-search-plugins.md) and use it for Retrieval Augmented Generation (RAG).
2. Use text search together with [function calling](../text-search-function-calling.md).
3. Learn more about using [vector stores](../text-search-vector-stores.md) for text search.

> [!div class="nextstepaction"]
> [Text Search Abstractions](../text-search-abstractions.md)
> [Text Search Plugins](../text-search-plugins.md)
> [Text Search Function Calling](../text-search-function-calling.md)
> [Text Search with Vector Stores](../text-search-vector-stores.md)
