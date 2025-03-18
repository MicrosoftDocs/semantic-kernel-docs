---
title: Text Search with Semantic Kernel (Preview)
description: Describes what a Semantic Kernel Text Search is, an provides a basic example of how to use one and how to get started.
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: conceptual
ms.author: markwallace
ms.date: 07/10/2024
ms.service: semantic-kernel
---

# What is Semantic Kernel Text Search?

> [!WARNING]
> The Semantic Kernel Text Search functionality is preview, and improvements that require breaking changes may still occur in limited circumstances before release.

Semantic Kernel provides capabilities that allow developers to integrate search when calling a Large Language Model (LLM).
This is important because LLM's are trained on fixed data sets and may need access to additional data to accurately respond to a user ask.

The process of providing additional context when prompting a LLM is called Retrieval-Augmented Generation (RAG).
RAG typically involves retrieving additional data that is relevant to the current user ask and augmenting the prompt sent to the LLM with this data.
The LLM can use its training plus the additional context to provide a more accurate response.

A simple example of when this becomes important is when the user's ask is related to up-to-date information not included in the LLM's training data set.
By performing an appropriate text search and including the results with the user's ask, more accurate responses will be achieved.

Semantic Kernel provides a set of Text Search capabilities that allow developers to perform searches using Web Search or Vector Databases and easily add RAG to their applications.

## How does text search differ from vector search?

Semantic Kernel provides APIs to perform data retrieval at different levels of abstraction.

Text search allows search at a high level in the stack, where the input is text with support for basic filtering.
The text search interface supports various types of output, including support for just returning a simple string.
This allows text search to support many implementations, including web search engines and vector stores.
The main goal for text search is to provide a simple interface that can be exposed as a plugin to chat completion.

> [!TIP]
> For all out-of-the-box text search implementations see [Out-of-the-box Text Search](./out-of-the-box-textsearch/index.md).

Vector search sits at a lower level in the stack, where the input is a vector. It also supports basic filtering,
plus choosing a vector from the data store to compare the input vector with. It returns a data model containing
the data from the data store.

When you want to do RAG with Vector stores, it makes sense to use text search and vector search together.
The way to to do this, is by wrapping a vector store collection, which supports vector search, with text
search and then exposing the text search as a plugin to chat completion. Semantic Kernel provides the
ability to do this easily out of the box. See the following tips for more information on how to do this.

> [!TIP]
> To see how to expose vector search to chat completion see [How to use Vector Stores with Semantic Kernel Text Search](./text-search-vector-stores.md).
> [!TIP]
> For more information on vector stores and vector search see [What are Semantic Kernel Vector Store connectors?](../vector-store-connectors/index.md).

::: zone pivot="programming-language-csharp"

## Implementing RAG using web text search

In the following sample code you can choose between using Bing or Google to perform web search operations.

> [!TIP]
> To run the samples shown on this page go to [GettingStartedWithTextSearch/Step1_Web_Search.cs](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithTextSearch/Step1_Web_Search.cs).

### Create text search instance

Each sample creates a text search instance and then performs a search operation to get results for the provided query.
The search results will contain a snippet of text from the webpage that describes its contents.
This provides only a limited context i.e., a subset of the web page contents and no link to the source of the information.
Later samples show how to address these limitations.

> [!TIP]
> The following sample code uses the Semantic Kernel OpenAI connector and Web plugins, install using the following commands:
>
> `dotnet add package Microsoft.SemanticKernel`<br>
> `dotnet add package Microsoft.SemanticKernel.Plugins.Web`

#### Bing web search

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
```

#### Google web search

```csharp
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Google;

// Create an ITextSearch instance using Google search
var textSearch = new GoogleTextSearch(
    searchEngineId: "<Your Google Search Engine Id>",
    apiKey: "<Your Google API Key>");

var query = "What is the Semantic Kernel?";

// Search and return results
KernelSearchResults<string> searchResults = await textSearch.SearchAsync(query, new() { Top = 4 });
await foreach (string result in searchResults.Results)
{
    Console.WriteLine(result);
}
```

> [!TIP]
> For more information on what types of search results can be retrieved, refer to [the documentation on Text Search Plugins](./text-search-plugins.md).

### Use text search results to augment a prompt

Next steps are to create a Plugin from the web text search and invoke the Plugin to add the search results to the prompt.

The sample code below shows how to achieve this:

1. Create a `Kernel` that has an OpenAI service registered. This will be used to call the `gpt-4o` model with the prompt.
2. Create a text search instance.
3. Create a Search Plugin from the text search instance.
4. Create a prompt template that will invoke the Search Plugin with the query and include search results in the prompt along with the original query.
5. Invoke the prompt and display the response.

The model will provide a response that is grounded in the latest information available from a web search.

#### Bing web search

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

#### Google web search

```csharp
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Google;

// Create a kernel with OpenAI chat completion
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
        modelId: "gpt-4o",
        apiKey: "<Your OpenAI API Key>");
Kernel kernel = kernelBuilder.Build();

// Create an ITextSearch instance using Google search
var textSearch = new GoogleTextSearch(
    searchEngineId: "<Your Google Search Engine Id>",
    apiKey: "<Your Google API Key>");

// Build a text search plugin with Google search and add to the kernel
var searchPlugin = textSearch.CreateWithSearch("SearchPlugin");
kernel.Plugins.Add(searchPlugin);

// Invoke prompt and use text search plugin to provide grounding information
var query = "What is the Semantic Kernel?";
var prompt = "{{SearchPlugin.Search $query}}. {{$query}}";
KernelArguments arguments = new() { { "query", query } };
Console.WriteLine(await kernel.InvokePromptAsync(prompt, arguments));
```

There are a number of issues with the above sample:

1. The response does not include citations showing the web pages that were used to provide grounding context.
2. The response will include data from any web site, it would be better to limit this to trusted sites.
3. Only a snippet of each web page is being used to provide grounding context to the model, the snippet may not contain the data required to provide an accurate response.

See the page which describes [Text Search Plugins](./text-search-plugins.md) for solutions to these issues.

Next we recommend looking at [Text Search Abstractions](./text-search-abstractions.md).

::: zone-end
::: zone pivot="programming-language-python"

## Implementing RAG using web text search

In the following sample code, you can choose between using Bing or Google to perform web search operations.

> [!TIP]
> Install the required packages using:
>
> `pip install semantic-kernel`
> `pip install semantic-kernel-plugins-web`

### Create text search instance

Each sample creates a text search instance and then performs a search operation to get results for the provided query.

#### Bing web search

```python
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.plugins.web import BingTextSearch
from semantic_kernel.kernel_arguments import KernelArguments

# Create a kernel with OpenAI chat completion
kernel = Kernel()
kernel.add_service(
    OpenAIChatCompletion(
        model_id="gpt-4o",
        api_key="<Your OpenAI API Key>"
    )
)

# Create a text search using Bing search
text_search = BingTextSearch(api_key="<Your Bing API Key>")

# Build a text search plugin with Bing search and add to the kernel
search_plugin = text_search.create_with_search("SearchPlugin")
kernel.plugins.add(search_plugin)

# Invoke prompt and use text search plugin to provide grounding information
query = "What is Semantic Kernel?"
prompt = "{{SearchPlugin.Search $query}}. {{$query}}"
arguments = KernelArguments({"query": query})

response = kernel.invoke_prompt(prompt, arguments)
print(response)
```

#### Google web search 

```python
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.plugins.web import GoogleTextSearch
from semantic_kernel.kernel_arguments import KernelArguments

# Create a kernel with OpenAI chat completion
kernel = Kernel()
kernel.add_service(
    OpenAIChatCompletion(
        model_id="gpt-4o",
        api_key="<Your OpenAI API Key>"
    )
)

# Create an ITextSearch instance using Google search
text_search = GoogleTextSearch(
    search_engine_id="<Your Google Search Engine Id>",
    api_key="<Your Google API Key>"
)

# Build a text search plugin with Google search and add to the kernel
search_plugin = text_search.create_with_search("SearchPlugin")
kernel.plugins.add(search_plugin)

# Invoke prompt and use text search plugin to provide grounding information
query = "What is the Semantic Kernel?"
prompt = "{{SearchPlugin.Search $query}}. {{$query}}"
arguments = KernelArguments({"query": query})

response = kernel.invoke_prompt(prompt, arguments)
print(response)
```

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

More coming soon.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Text Search Abstractions](./text-search-abstractions.md)
> [Text Search Plugins](./text-search-plugins.md)
> [Text Search Function Calling](./text-search-function-calling.md)
> [Text Search with Vector Stores](./text-search-vector-stores.md)
