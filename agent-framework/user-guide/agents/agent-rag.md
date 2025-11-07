---
title: Agent Retrieval Augmented Generation (RAG)
description: Learn how to use Retrieval Augmented Generation (RAG) with Agent Framework
zone_pivot_groups: programming-languages
author: westey-m
ms.topic: reference
ms.author: westey
ms.date: 11/06/2025
ms.service: agent-framework
---

# Agent Retrieval Augmented Generation (RAG)

Microsoft Agent Framework supports adding Retrieval Augmented Generation (RAG) capabilities to agents easily by adding AI Context Providers to the agent.

::: zone pivot="programming-language-csharp"

## Using TextSearchProvider

The `TextSearchProvider` class is an out-of-the-box implementation of a RAG context provider.

It can easily be attached to a `ChatClientAgent` using the `AIContextProviderFactory` option to provide RAG capabilities to the agent.

```csharp
// Create the AI agent with the TextSearchProvider as the AI context provider.
AIAgent agent = azureOpenAIClient
    .GetChatClient(deploymentName)
    .CreateAIAgent(new ChatClientAgentOptions
    {
        Instructions = "You are a helpful support specialist for Contoso Outdoors. Answer questions using the provided context and cite the source document when available.",
        AIContextProviderFactory = ctx => new TextSearchProvider(SearchAdapter, ctx.SerializedState, ctx.JsonSerializerOptions, textSearchOptions)
    });
```

The `TextSearchProvider` requires a function that provides the search results given a query. This can be implemented using any search technology, e.g. Azure AI Search, or a web search engine.

Here is an example of a mock search function that returns pre-defined results based on the query.
`SourceName` and `SourceLink` are optional, but if provided will be used by the agent to cite the source of the information when answering the user's question.

```csharp
static Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchAdapter(string query, CancellationToken cancellationToken)
{
    // The mock search inspects the user's question and returns pre-defined snippets
    // that resemble documents stored in an external knowledge source.
    List<TextSearchProvider.TextSearchResult> results = new();

    if (query.Contains("return", StringComparison.OrdinalIgnoreCase) || query.Contains("refund", StringComparison.OrdinalIgnoreCase))
    {
        results.Add(new()
        {
            SourceName = "Contoso Outdoors Return Policy",
            SourceLink = "https://contoso.com/policies/returns",
            Text = "Customers may return any item within 30 days of delivery. Items should be unused and include original packaging. Refunds are issued to the original payment method within 5 business days of inspection."
        });
    }

    return Task.FromResult<IEnumerable<TextSearchProvider.TextSearchResult>>(results);
}
```

### TextSearchProvider Options

The `TextSearchProvider` can be customized via the `TextSearchProviderOptions` class. Here is an example of creating options to run the search prior to every model invocation and keep a short rolling window of conversation context.

```csharp
TextSearchProviderOptions textSearchOptions = new()
{
    // Run the search prior to every model invocation and keep a short rolling window of conversation context.
    SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
    RecentMessageMemoryLimit = 6,
};
```

The `TextSearchProvider` class supports the following options via the `TextSearchProviderOptions` class.

| Option | Type | Description | Default |
|--------|------|-------------|---------|
| SearchTime | `TextSearchProviderOptions.TextSearchBehavior` | Indicates when the search should be executed. There are two options, each time the agent is invoked, or on-demand via function calling. | `TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke` |
| FunctionToolName | `string` | The name of the exposed search tool when operating in on-demand mode. | "Search" |
| FunctionToolDescription | `string` | The description of the exposed search tool when operating in on-demand mode. | "Allows searching for additional information to help answer the user question." |
| ContextPrompt | `string` | The context prompt prefixed to results when operating in `BeforeAIInvoke` mode. | "## Additional Context\nConsider the following information from source documents when responding to the user:" |
| CitationsPrompt | `string` | The instruction appended after results to request citations when operating in `BeforeAIInvoke` mode. | "Include citations to the source document with document name and link if document name and link is available." |
| ContextFormatter | `Func<IList<TextSearchProvider.TextSearchResult>, string>` | Optional delegate to fully customize formatting of the result list when operating in `BeforeAIInvoke` mode. If provided, `ContextPrompt` and `CitationsPrompt` are ignored. | `null` |
| RecentMessageMemoryLimit | `int` | The number of recent conversation messages (both user and assistant) to keep in memory and include when constructing the search input for `BeforeAIInvoke` searches. | `0` (disabled) |
| RecentMessageRolesIncluded | `List<ChatRole>` | The list of `ChatRole` types to filter recent messages to when deciding which recent messages to include when constructing the search input. | `ChatRole.User` |

::: zone-end
::: zone pivot="programming-language-python"

More info coming soon.

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Agent Memory](./agent-memory.md)
