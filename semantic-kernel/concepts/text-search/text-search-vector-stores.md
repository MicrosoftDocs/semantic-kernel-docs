---
title: Semantic Kernel Text Search with Vector Stores (Preview)
description: Describes how to use a Vector Store with Semantic Text search. Includes samples show how to create a search plugin which searches a Vector Store.
zone_pivot_groups: programming-languages
author: markwallace
ms.topic: conceptual
ms.author: markwallace
ms.date: 07/10/2024
ms.service: semantic-kernel
---

# How to use Vector Stores with Semantic Kernel Text Search

All of the Vector Store [connectors](../vector-store-connectors/out-of-the-box-connectors/index.md) can be used for text search.

1. Use the Vector Store connector to retrieve the record collection you want to search.
2. Wrap the record collection with `VectorStoreTextSearch`.
3. Convert to a plugin for use in RAG and/or function calling scenarios.

It's very likely that you will want to customize the plugin search function so that its description reflects the type of data available in the record collection.
For example if the record collection contains information about hotels the plugin search function description should mention this.
This will allow you to register multiple plugins e.g., one to search for hotels, another for restaurants and another for things to do.

The [text search abstractions](./text-search-abstractions.md) include a function to return a normalized search result i.e., an instance of `TextSearchResult`.
This normalized search result contains a value and optionally a name and link.
The [text search abstractions](./text-search-abstractions.md) include a function to return a string value e.g., one of the data model properties will be returned as the search result.
For text search to work correctly you need to provide a way to map from the Vector Store data model to an instance of `TextSearchResult`.
The next section describes the two options you can use to perform this mapping.

::: zone pivot="programming-language-csharp"

> [!TIP]
> To run the samples shown on this page go to [GettingStartedWithTextSearch/Step4_Search_With_VectorStore.cs](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithTextSearch/Step4_Search_With_VectorStore.cs).

## Using a vector store model with text search

The mapping from a Vector Store data model to a `TextSearchResult` can be done declaratively using attributes.

1. `[TextSearchResultValue]` - Add this attribute to the property of the data model which will be the value of the `TextSearchResult`, e.g. the textual data that the AI model will use to answer questions.
2. `[TextSearchResultName]` - Add this attribute to the property of the data model which will be the name of the `TextSearchResult`.
3. `[TextSearchResultLink]` - Add this attribute to the property of the data model which will be the link to the `TextSearchResult`.

The following sample shows an data model which has the text search result attributes applied.

```csharp
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;

public sealed class DataModel
{
    [VectorStoreRecordKey]
    [TextSearchResultName]
    public Guid Key { get; init; }

    [VectorStoreRecordData]
    [TextSearchResultValue]
    public string Text { get; init; }

    [VectorStoreRecordData]
    [TextSearchResultLink]
    public string Link { get; init; }

    [VectorStoreRecordData(IsFilterable = true)]
    public required string Tag { get; init; }

    [VectorStoreRecordVector(1536)]
    public ReadOnlyMemory<float> Embedding { get; init; }
}
```

The mapping from a Vector Store data model to a `string` or a `TextSearchResult` can also be done by providing implementations of `ITextSearchStringMapper` and `ITextSearchResultMapper` respectively.

You may decide to create custom mappers for the following scenarios:

1. Multiple properties from the data model need to be combined together e.g., if multiple properties need to be combined to provide the value.
2. Additional logic is required to generate one of the properties e.g., if the link property needs to be computed from the data model properties.

The following sample shows a data model and two example mapper implementations that can be used with the data model.

```csharp
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;

protected sealed class DataModel
{
    [VectorStoreRecordKey]
    public Guid Key { get; init; }

    [VectorStoreRecordData]
    public required string Text { get; init; }

    [VectorStoreRecordData]
    public required string Link { get; init; }

    [VectorStoreRecordData(IsFilterable = true)]
    public required string Tag { get; init; }

    [VectorStoreRecordVector(1536)]
    public ReadOnlyMemory<float> Embedding { get; init; }
}

/// <summary>
/// String mapper which converts a DataModel to a string.
/// </summary>
protected sealed class DataModelTextSearchStringMapper : ITextSearchStringMapper
{
    /// <inheritdoc />
    public string MapFromResultToString(object result)
    {
        if (result is DataModel dataModel)
        {
            return dataModel.Text;
        }
        throw new ArgumentException("Invalid result type.");
    }
}

/// <summary>
/// Result mapper which converts a DataModel to a TextSearchResult.
/// </summary>
protected sealed class DataModelTextSearchResultMapper : ITextSearchResultMapper
{
    /// <inheritdoc />
    public TextSearchResult MapFromResultToTextSearchResult(object result)
    {
        if (result is DataModel dataModel)
        {
            return new TextSearchResult(value: dataModel.Text) { Name = dataModel.Key.ToString(), Link = dataModel.Link };
        }
        throw new ArgumentException("Invalid result type.");
    }
}
```

The mapper implementations can be provided as parameters when creating the `VectorStoreTextSearch` as shown below:

```csharp
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;

// Create custom mapper to map a <see cref="DataModel"/> to a <see cref="string"/>
var stringMapper = new DataModelTextSearchStringMapper();

// Create custom mapper to map a <see cref="DataModel"/> to a <see cref="TextSearchResult"/>
var resultMapper = new DataModelTextSearchResultMapper();

// Add code to create instances of IVectorStoreRecordCollection and ITextEmbeddingGenerationService 

// Create a text search instance using the vector store record collection.
var result = new VectorStoreTextSearch<DataModel>(vectorStoreRecordCollection, textEmbeddingGeneration, stringMapper, resultMapper);
```

## Using a vector store with text search

The sample below shows how to create an instance of `VectorStoreTextSearch` using a Vector Store record collection.

> [!TIP]
> The following samples require instances of `IVectorStoreRecordCollection` and `ITextEmbeddingGenerationService`.
> To create an instance of `IVectorStoreRecordCollection` refer to [the documentation for each connector](../vector-store-connectors/out-of-the-box-connectors/index.md).
> To create an instance of `ITextEmbeddingGenerationService` select the service you wish to use e.g., Azure OpenAI, OpenAI, ... or use a local model ONNX, Ollama, ... and create an instance of the corresponding `ITextEmbeddingGenerationService` implementation.

> [!TIP]
> A `VectorStoreTextSearch` can also be constructed from an instance of `IVectorizableTextSearch`. In this case no `ITextEmbeddingGenerationService` is needed.

```csharp
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

// Add code to create instances of IVectorStoreRecordCollection and ITextEmbeddingGenerationService 

// Create a text search instance using the vector store record collection.
var textSearch = new VectorStoreTextSearch<DataModel>(vectorStoreRecordCollection, textEmbeddingGeneration);

// Search and return results as TextSearchResult items
var query = "What is the Semantic Kernel?";
KernelSearchResults<TextSearchResult> textResults = await textSearch.GetTextSearchResultsAsync(query, new() { Top = 2, Skip = 0 });
Console.WriteLine("\n--- Text Search Results ---\n");
await foreach (TextSearchResult result in textResults.Results)
{
    Console.WriteLine($"Name:  {result.Name}");
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Link:  {result.Link}");
}
```

## Creating a search plugin from a vector store

The sample below shows how to create a plugin named `SearchPlugin` from an instance of `VectorStoreTextSearch`.
Using `CreateWithGetTextSearchResults` creates a new plugin with a single `GetTextSearchResults` function that calls the underlying Vector Store record collection search implementation.
The `SearchPlugin` is added to the `Kernel` which makes it available to be called during prompt rendering.
The prompt template includes a call to `{{SearchPlugin.Search $query}}` which will invoke the `SearchPlugin` to retrieve results related to the current query.
The results are then inserted into the rendered prompt before it is sent to the model.

```csharp
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

// Create a kernel with OpenAI chat completion
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
        modelId: TestConfiguration.OpenAI.ChatModelId,
        apiKey: TestConfiguration.OpenAI.ApiKey);
Kernel kernel = kernelBuilder.Build();

// Add code to create instances of IVectorStoreRecordCollection and ITextEmbeddingGenerationService

// Create a text search instance using the vector store record collection.
var textSearch = new VectorStoreTextSearch<DataModel>(vectorStoreRecordCollection, textEmbeddingGeneration);

// Build a text search plugin with vector store search and add to the kernel
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

## Using a vector store with function calling

The sample below also creates a `SearchPlugin` from an instance of `VectorStoreTextSearch`.
This plugin will be advertised to the model for use with automatic function calling using the `FunctionChoiceBehavior` in the prompt execution settings.
When you run this sample the model will invoke the search function to retrieve additional information to respond to the question.
It will likely just search for "Semantic Kernel" rather than the entire query.

```csharp
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

// Create a kernel with OpenAI chat completion
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
        modelId: TestConfiguration.OpenAI.ChatModelId,
        apiKey: TestConfiguration.OpenAI.ApiKey);
Kernel kernel = kernelBuilder.Build();

// Add code to create instances of IVectorStoreRecordCollection and ITextEmbeddingGenerationService

// Create a text search instance using the vector store record collection.
var textSearch = new VectorStoreTextSearch<DataModel>(vectorStoreRecordCollection, textEmbeddingGeneration);

// Build a text search plugin with vector store search and add to the kernel
var searchPlugin = textSearch.CreateWithGetTextSearchResults("SearchPlugin");
kernel.Plugins.Add(searchPlugin);

// Invoke prompt and use text search plugin to provide grounding information
OpenAIPromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
KernelArguments arguments = new(settings);
Console.WriteLine(await kernel.InvokePromptAsync("What is the Semantic Kernel?", arguments));
```

## Customizing the search function

The sample below how to customize the description of the search function that is added to the `SearchPlugin`.
Some things you might want to do are:

1. Change the name of the search function to reflect what is in the associated record collection e.g., you might want to name the function `SearchForHotels` if the record collection contains hotel information.
2. Change the description of the function. An accurate function description helps the AI model to select the best function to call. This is especially important if you are adding multiple search functions.
3. Add an additional parameter to the search function. If the record collection contain hotel information and one of the properties is the city name you could add a property to the search function to specify the city. A filter will be automatically added and it will filter search results by city.

> [!TIP]
> The sample below uses the default implementation of search. You can opt to provide your own implementation which calls the underlying Vector Store record collection with additional options to fine tune your searches.

```csharp
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

// Create a kernel with OpenAI chat completion
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
        modelId: TestConfiguration.OpenAI.ChatModelId,
        apiKey: TestConfiguration.OpenAI.ApiKey);
Kernel kernel = kernelBuilder.Build();

// Add code to create instances of IVectorStoreRecordCollection and ITextEmbeddingGenerationService

// Create a text search instance using the vector store record collection.
var textSearch = new VectorStoreTextSearch<DataModel>(vectorStoreRecordCollection, textEmbeddingGeneration);

// Create options to describe the function I want to register.
var options = new KernelFunctionFromMethodOptions()
{
    FunctionName = "Search",
    Description = "Perform a search for content related to the specified query from a record collection.",
    Parameters =
    [
        new KernelParameterMetadata("query") { Description = "What to search for", IsRequired = true },
        new KernelParameterMetadata("top") { Description = "Number of results", IsRequired = false, DefaultValue = 2 },
        new KernelParameterMetadata("skip") { Description = "Number of results to skip", IsRequired = false, DefaultValue = 0 },
    ],
    ReturnParameter = new() { ParameterType = typeof(KernelSearchResults<string>) },
};

// Build a text search plugin with vector store search and add to the kernel
var searchPlugin = textSearch.CreateWithGetTextSearchResults("SearchPlugin", "Search a record collection", [textSearch.CreateSearch(options)]);
kernel.Plugins.Add(searchPlugin);

// Invoke prompt and use text search plugin to provide grounding information
OpenAIPromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
KernelArguments arguments = new(settings);
Console.WriteLine(await kernel.InvokePromptAsync("What is the Semantic Kernel?", arguments));
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
