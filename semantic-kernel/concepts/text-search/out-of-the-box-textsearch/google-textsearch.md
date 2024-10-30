---
title: Using the Semantic Kernel Google text search (Preview)
description: Contains information on how to use a Semantic Kernel Text Search with Google.
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: conceptual
ms.author: markwallace
ms.date: 10/21/2024
ms.service: semantic-kernel
---
# Using the Google Text Search (Preview)

> [!WARNING]
> The Semantic Kernel Text Search functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

## Overview

The Google Text Search implementation uses [Google Custom Search](https://developers.google.com/custom-search) to retrieve search results. You must provide your own Google Search Api Key and Search Engine Id to use this component.

## Limitations

| Feature Area                      | Support                                                                               |
|-----------------------------------|---------------------------------------------------------------------------------------|
| Search API                        | [Google Custom Search API](https://developers.google.com/custom-search/v1/reference/rest/v1/cse) only. |
| Supported filter clauses          | Only "equal to" filter clauses are supported.                                         |
| Supported filter keys             | Following parameters are supported: "cr", "dateRestrict", "exactTerms", "excludeTerms", "filter", "gl", "hl", "linkSite", "lr", "orTerms", "rights", "siteSearch". For more information see [parameters](https://developers.google.com/custom-search/v1/reference/rest/v1/cse/list). |

> [!TIP]
> Follow this link for more information on how [search is performed](https://developers.google.com/custom-search/v1/reference/rest/v1/cse/list)

## Getting started

The sample below shows how to create a `GoogleTextSearch` and use it to perform a text search.

::: zone pivot="programming-language-csharp"

```csharp
using Google.Apis.Http;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Google;

// Create an ITextSearch instance using Google search
var textSearch = new GoogleTextSearch(
    initializer: new() { ApiKey = "<Your Google API Key>", HttpClientFactory = new CustomHttpClientFactory(this.Output) },
    searchEngineId: "<Your Google Search Engine Id>");

var query = "What is the Semantic Kernel?";

// Search and return results as string items
KernelSearchResults<string> stringResults = await textSearch.SearchAsync(query, new() { Top = 4, Skip = 0 });
Console.WriteLine("——— String Results ———\n");
await foreach (string result in stringResults.Results)
{
    Console.WriteLine(result);
}

// Search and return results as TextSearchResult items
KernelSearchResults<TextSearchResult> textResults = await textSearch.GetTextSearchResultsAsync(query, new() { Top = 4, Skip = 4 });
Console.WriteLine("\n——— Text Search Results ———\n");
await foreach (TextSearchResult result in textResults.Results)
{
    Console.WriteLine($"Name:  {result.Name}");
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Link:  {result.Link}");
}

// Search and return results as Google.Apis.CustomSearchAPI.v1.Data.Result items
KernelSearchResults<object> fullResults = await textSearch.GetSearchResultsAsync(query, new() { Top = 4, Skip = 8 });
Console.WriteLine("\n——— Google Web Page Results ———\n");
await foreach (Google.Apis.CustomSearchAPI.v1.Data.Result result in fullResults.Results)
{
    Console.WriteLine($"Title:       {result.Title}");
    Console.WriteLine($"Snippet:     {result.Snippet}");
    Console.WriteLine($"Link:        {result.Link}");
    Console.WriteLine($"DisplayLink: {result.DisplayLink}");
    Console.WriteLine($"Kind:        {result.Kind}");
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
