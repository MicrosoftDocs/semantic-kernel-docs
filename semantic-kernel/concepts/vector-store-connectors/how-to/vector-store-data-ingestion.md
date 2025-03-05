---
title: How to ingest data into a Semantic Kernel Vector Store (Preview)
description: Step by step instructions on how to ingest data into a Vector Store using Semantic Kernel
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: tutorial
ms.author: westey
ms.date: 07/08/2024
ms.service: semantic-kernel
---
# How to ingest data into a Vector Store using Semantic Kernel (Preview)

> [!WARNING]
> The Semantic Kernel Vector Store functionality is in preview, and improvements that require breaking changes may still occur in limited circumstances before release.

This article will demonstrate how to create an application to

1. Take text from each paragraph in a Microsoft Word document
2. Generate an embedding for each paragraph
3. Upsert the text, embedding and a reference to the original location into a Redis instance.

## Prerequisites

For this sample you will need

1. An embedding generation model hosted in Azure or another provider of your choice.
2. An instance of Redis or Docker Desktop so that you can run Redis locally.
3. A Word document to parse and load. Here is a zip containing a sample Word document you can download and use: [vector-store-data-ingestion-input.zip](../../../media/vector-store-data-ingestion-input.zip).

## Setup Redis

If you already have a Redis instance you can use that. If you prefer to test your project locally
you can easily start a Redis container using docker.

```
docker run -d --name redis-stack -p 6379:6379 -p 8001:8001 redis/redis-stack:latest
```

To verify that it is running successfully, visit [http://localhost:8001/redis-stack/browser](http://localhost:8001/redis-stack/browser) in your browser.

The rest of these instructions will assume that you are using this container using the above settings.

::: zone pivot="programming-language-csharp"

## Create your project

Create a new project and add nuget package references for the Redis connector from Semantic Kernel, the open xml package to read the word
document with and the OpenAI connector from Semantic Kernel for generating embeddings.

```dotnetcli
dotnet new console --framework net8.0 --name SKVectorIngest
cd SKVectorIngest
dotnet add package Microsoft.SemanticKernel.Connectors.AzureOpenAI
dotnet add package Microsoft.SemanticKernel.Connectors.Redis --prerelease
dotnet add package DocumentFormat.OpenXml
```

## Add a data model

To upload data we need to first describe what format the data should have in the database.
We can do this by creating a data model with attributes that describe the function of each property.

Add a new file to the project called `TextParagraph.cs` and add the following model to it.

```csharp
using Microsoft.Extensions.VectorData;

namespace SKVectorIngest;

internal class TextParagraph
{
    /// <summary>A unique key for the text paragraph.</summary>
    [VectorStoreRecordKey]
    public required string Key { get; init; }

    /// <summary>A uri that points at the original location of the document containing the text.</summary>
    [VectorStoreRecordData]
    public required string DocumentUri { get; init; }

    /// <summary>The id of the paragraph from the document containing the text.</summary>
    [VectorStoreRecordData]
    public required string ParagraphId { get; init; }

    /// <summary>The text of the paragraph.</summary>
    [VectorStoreRecordData]
    public required string Text { get; init; }

    /// <summary>The embedding generated from the Text.</summary>
    [VectorStoreRecordVector(1536)]
    public ReadOnlyMemory<float> TextEmbedding { get; set; }
}
```

Note that we are passing the value `1536` to the `VectorStoreRecordVectorAttribute`. This is the dimension size of the vector and has to match the size of vector that your chosen embedding generator produces.

> [!TIP]
> For more information on how to annotate your data model and what additional options are available for each attribute, refer to [definining your data model](../../../concepts/vector-store-connectors/defining-your-data-model.md).

## Read the paragraphs in the document

We need some code to read the word document and find the text of each paragraph in it.

Add a new file to the project called `DocumentReader.cs` and add the following class to read the paragraphs from a document.

```csharp
using System.Text;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;

namespace SKVectorIngest;

internal class DocumentReader
{
    public static IEnumerable<TextParagraph> ReadParagraphs(Stream documentContents, string documentUri)
    {
        // Open the document.
        using WordprocessingDocument wordDoc = WordprocessingDocument.Open(documentContents, false);
        if (wordDoc.MainDocumentPart == null)
        {
            yield break;
        }

        // Create an XmlDocument to hold the document contents and load the document contents into the XmlDocument.
        XmlDocument xmlDoc = new XmlDocument();
        XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
        nsManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        nsManager.AddNamespace("w14", "http://schemas.microsoft.com/office/word/2010/wordml");

        xmlDoc.Load(wordDoc.MainDocumentPart.GetStream());

        // Select all paragraphs in the document and break if none found.
        XmlNodeList? paragraphs = xmlDoc.SelectNodes("//w:p", nsManager);
        if (paragraphs == null)
        {
            yield break;
        }

        // Iterate over each paragraph.
        foreach (XmlNode paragraph in paragraphs)
        {
            // Select all text nodes in the paragraph and continue if none found.
            XmlNodeList? texts = paragraph.SelectNodes(".//w:t", nsManager);
            if (texts == null)
            {
                continue;
            }

            // Combine all non-empty text nodes into a single string.
            var textBuilder = new StringBuilder();
            foreach (XmlNode text in texts)
            {
                if (!string.IsNullOrWhiteSpace(text.InnerText))
                {
                    textBuilder.Append(text.InnerText);
                }
            }

            // Yield a new TextParagraph if the combined text is not empty.
            var combinedText = textBuilder.ToString();
            if (!string.IsNullOrWhiteSpace(combinedText))
            {
                Console.WriteLine("Found paragraph:");
                Console.WriteLine(combinedText);
                Console.WriteLine();

                yield return new TextParagraph
                {
                    Key = Guid.NewGuid().ToString(),
                    DocumentUri = documentUri,
                    ParagraphId = paragraph.Attributes?["w14:paraId"]?.Value ?? string.Empty,
                    Text = combinedText
                };
            }
        }
    }
}
```

## Generate embeddings and upload the data

We will need some code to generate embeddings and upload the paragraphs to Redis. Let's do this in a separate class.

Add a new file called `DataUploader.cs` and add the following class to it.

```csharp
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Embeddings;

namespace SKVectorIngest;

internal class DataUploader(IVectorStore vectorStore, ITextEmbeddingGenerationService textEmbeddingGenerationService)
{
    /// <summary>
    /// Generate an embedding for each text paragraph and upload it to the specified collection.
    /// </summary>
    /// <param name="collectionName">The name of the collection to upload the text paragraphs to.</param>
    /// <param name="textParagraphs">The text paragraphs to upload.</param>
    /// <returns>An async task.</returns>
    public async Task GenerateEmbeddingsAndUpload(string collectionName, IEnumerable<TextParagraph> textParagraphs)
    {
        var collection = vectorStore.GetCollection<string, TextParagraph>(collectionName);
        await collection.CreateCollectionIfNotExistsAsync();

        foreach (var paragraph in textParagraphs)
        {
            // Generate the text embedding.
            Console.WriteLine($"Generating embedding for paragraph: {paragraph.ParagraphId}");
            paragraph.TextEmbedding = await textEmbeddingGenerationService.GenerateEmbeddingAsync(paragraph.Text);

            // Upload the text paragraph.
            Console.WriteLine($"Upserting paragraph: {paragraph.ParagraphId}");
            await collection.UpsertAsync(paragraph);

            Console.WriteLine();
        }
    }
}
```

## Put it all together

Finally, we need to put together the different pieces.
In this example, we will use the Semantic Kernel dependency injection container but it is also possible to use any `IServiceCollection` based container.

Add the following code to your `Program.cs` file to create the container, register the Redis vector store and register the embedding service.
Make sure to replace the text embedding generation settings with your own values.

```csharp
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0020 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using SKVectorIngest;

// Replace with your values.
var deploymentName = "text-embedding-ada-002";
var endpoint = "https://sksample.openai.azure.com/";
var apiKey = "your-api-key";

// Register Azure OpenAI text embedding generation service and Redis vector store.
var builder = Kernel.CreateBuilder()
    .AddAzureOpenAITextEmbeddingGeneration(deploymentName, endpoint, apiKey)
    .AddRedisVectorStore("localhost:6379");

// Register the data uploader.
builder.Services.AddSingleton<DataUploader>();

// Build the kernel and get the data uploader.
var kernel = builder.Build();
var dataUploader = kernel.Services.GetRequiredService<DataUploader>();
```

As a last step, we want to read the paragraphs from our word document, and call the data uploader
to generate the embeddings and upload the paragraphs.

```csharp
// Load the data.
var textParagraphs = DocumentReader.ReadParagraphs(
    new FileStream(
        "vector-store-data-ingestion-input.docx",
        FileMode.Open),
    "file:///c:/vector-store-data-ingestion-input.docx");

await dataUploader.GenerateEmbeddingsAndUpload(
    "sk-documentation",
    textParagraphs);
```

## See your data in Redis

Navigate to the Redis stack browser, e.g. [http://localhost:8001/redis-stack/browser](http://localhost:8001/redis-stack/browser) where you should now be able to see
your uploaded paragraphs. Here is an example of what you should see for one of the uploaded paragraphs.

```json
{
    "DocumentUri" : "file:///c:/vector-store-data-ingestion-input.docx",
    "ParagraphId" : "14CA7304",
    "Text" : "Version 1.0+ support across C#, Python, and Java means it’s reliable, committed to non breaking changes. Any existing chat-based APIs are easily expanded to support additional modalities like voice and video.",
    "TextEmbedding" : [...]
}
```

::: zone-end
::: zone pivot="programming-language-python"

## Coming soon

Further instructions coming soon

::: zone-end
::: zone pivot="programming-language-java"

## Coming soon

Further instructions coming soon

::: zone-end
