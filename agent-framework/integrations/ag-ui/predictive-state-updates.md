---
title: Predictive State Updates with AG-UI
description: Stream a tool's arguments as optimistic state snapshots so the UI updates before the tool call completes
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 07/10/2026
ms.service: agent-framework
---

# Predictive State Updates with AG-UI

Predictive state updates let the UI react to a tool call *while its arguments are still being generated*, instead of waiting for the tool to finish. As the model streams the arguments for a tool, the server converts those partial arguments into state snapshots and sends them to the client. The client renders each snapshot immediately, giving the user an optimistic, live preview of the result.

A common example is a document editor: as the model writes the document, the client shows the text appearing in real time, then asks the user to confirm the change once the model is done.

::: zone pivot="programming-language-csharp"

## Prerequisites

Before you begin, ensure you have completed the [Getting Started](getting-started.md) tutorial and read [State Management](state-management.md), since predictive state updates build on the same `AGUIStreamOptions` state-event mechanism.

You need:

- .NET 10.0 or later
- `Microsoft.Agents.AI.Hosting.AGUI.AspNetCore` and `Microsoft.Agents.AI.OpenAI` packages
- An Azure OpenAI endpoint and deployment

## How It Works

Unlike the [shared-state](state-management.md) scenario — where a tool *runs* and its result becomes a snapshot — the predictive scenario intercepts the tool **call** before it executes and streams the argument into state:

1. The agent declares a `write_document_local` tool. The model calls it with the full document text as its `document` argument.
2. The tool is **not** executed server-side. Instead, an <xref:AGUI.Server.AGUIStreamOptions> `MapCall` mapping intercepts the call.
3. The mapping emits a series of `STATE_SNAPSHOT` events, each carrying a progressively longer prefix of the document, so the client sees the text stream in.
4. It then completes the tool call with a `TOOL_CALL_RESULT` event and injects a client-side `confirm_changes` tool call so the client can prompt the user to approve.
5. The client renders each snapshot and shows the confirm/reject prompt.

Because the mapping produces the tool's result itself, the document tool is declared but never invoked — the chat client is built **without** function invocation.

## Define the State Model

The state model describes the shape the client renders. Use `JsonPropertyName` so the property names match what the client expects:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class DocumentState
{
    [JsonPropertyName("document")]
    public string Document { get; set; } = string.Empty;
}

[JsonSerializable(typeof(DocumentState))]
[JsonSerializable(typeof(JsonElement))]
internal sealed partial class DocumentSerializerContext : JsonSerializerContext;
```

## Declare the Document Tool

The tool signature is what the model fills in. It is declared so the model calls it, but its result is produced by the stream mapping, not by executing the method body:

```csharp
using System.ComponentModel;
using Microsoft.Extensions.AI;

[Description("Write a document in markdown format.")]
static string WriteDocument(
    [Description("The document content to write.")] string document) => "Document written successfully";

AITool writeDocument = AIFunctionFactory.Create(
    WriteDocument,
    name: "write_document_local",
    description: "Write a document. Use markdown formatting to format the document.");
```

## Configure the Predictive Stream Mapping

Register a `MapCall` mapping for the tool. When the model calls `write_document_local`, the mapping reads the streamed `document` argument, emits progressive `StateSnapshotEvent` snapshots, completes the tool call, and injects a `confirm_changes` client-side tool call:

```csharp
using System.Text.Json;
using AGUI.Abstractions;
using AGUI.Server;
using Microsoft.Extensions.AI;

static AGUIStreamOptions CreatePredictiveStreamOptions(JsonSerializerOptions jsonSerializerOptions)
{
    string? lastEmittedDocument = null;

    return new AGUIStreamOptions().MapCall("write_document_local", fcc =>
    {
        string? document = fcc.Arguments?.TryGetValue("document", out var value) == true
            ? value?.ToString()
            : null;

        if (document is null || document == lastEmittedDocument)
        {
            return [];
        }

        var events = new List<BaseEvent>();

        // Only stream the newly added portion if the document grew.
        int startIndex = lastEmittedDocument is not null &&
            document.StartsWith(lastEmittedDocument, StringComparison.Ordinal)
                ? lastEmittedDocument.Length
                : 0;

        const int chunkSize = 10;
        for (int i = startIndex; i < document.Length; i += chunkSize)
        {
            int length = Math.Min(chunkSize, document.Length - i);
            var snapshot = new DocumentState { Document = document[..(i + length)] };
            JsonElement snapshotJson = JsonSerializer.SerializeToElement(snapshot, jsonSerializerOptions);

            events.Add(new StateSnapshotEvent { Snapshot = snapshotJson });
        }

        // Complete the write_document_local call (its document is now reflected in state) so the
        // only tool call the client sees pending is confirm_changes.
        events.Add(new ToolCallResultEvent
        {
            MessageId = Guid.NewGuid().ToString("N"),
            ToolCallId = fcc.CallId,
            Content = "Document written.",
            Role = "tool",
        });

        // Inject a client-side confirm_changes tool call so the approval modal renders.
        string confirmCallId = Guid.NewGuid().ToString("N");
        string confirmMessageId = Guid.NewGuid().ToString("N");
        events.Add(new ToolCallStartEvent { ToolCallId = confirmCallId, ToolCallName = "confirm_changes", ParentMessageId = confirmMessageId });
        events.Add(new ToolCallArgsEvent { ToolCallId = confirmCallId, Delta = "{}" });
        events.Add(new ToolCallEndEvent { ToolCallId = confirmCallId });

        lastEmittedDocument = document;
        return events;
    });
}
```

> [!NOTE]
> Each snapshot contains the full document up to that point, so the client always renders a consistent view even if it misses an intermediate update.

## Map the Endpoint

Because the mapping produces the tool result itself, build the chat client **without** function invocation and stream through the AG-UI pipeline directly: adapt the incoming `RunAgentInput` with `ToChatRequestContext`, call `GetStreamingResponseAsync`, and convert the updates with `AsAGUIEventStreamAsync`.

```csharp
using AGUI.Abstractions;
using AGUI.Server;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

const string PredictiveSystemPrompt =
    """
    You are a document editor assistant. When asked to write or edit content:
    - Use the `write_document_local` tool with the full document text in Markdown format.
    - You MUST write the full document, even when changing only a few words.
    - When making edits, keep them minimal. Do not change every word.
    After writing the document, briefly summarize the changes you made in at most two sentences.
    """;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.TypeInfoResolverChain.Add(DocumentSerializerContext.Default));
builder.Services.AddAGUIServer();

string endpoint = builder.Configuration["AZURE_OPENAI_ENDPOINT"]
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
string deploymentName = builder.Configuration["AZURE_OPENAI_DEPLOYMENT_NAME"]
    ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME is not set.");

// No UseFunctionInvocation: the call is intercepted by the stream mapping, not executed.
IChatClient chatClient = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();

WebApplication app = builder.Build();

JsonSerializerOptions jsonSerializerOptions = app.Services
    .GetRequiredService<IOptions<JsonOptions>>()
    .Value.SerializerOptions;

app.MapPost("/", (
    [FromBody] RunAgentInput input,
    HttpContext httpContext,
    CancellationToken cancellationToken) =>
{
    AGUIStreamOptions streamOptions = CreatePredictiveStreamOptions(jsonSerializerOptions);

    ChatRequestContext ctx = input.ToChatRequestContext(jsonSerializerOptions, streamOptions);
    ctx.Messages.Insert(0, new ChatMessage(ChatRole.System, PredictiveSystemPrompt));
    (ctx.ChatOptions.Tools ??= []).Add(writeDocument);

    var updates = chatClient.GetStreamingResponseAsync(ctx.Messages, ctx.ChatOptions, cancellationToken);
    IAsyncEnumerable<BaseEvent> events = updates.AsAGUIEventStreamAsync(ctx, cancellationToken);

    return TypedResults.ServerSentEvents(events);
});

await app.RunAsync();
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

> [!NOTE]
> `confirm_changes` is a *client-side* tool. The stream mapping requests it, and the client renders the approval prompt. See [Human-in-the-Loop](human-in-the-loop.md) for the client-side tool pattern.

### Key Concepts

- **`AGUIStreamOptions.MapCall`**: Intercepts a tool *call* (before execution) and returns the AG-UI events to emit for it.
- **`FunctionCallContent.Arguments`**: The streamed tool arguments; read `Arguments["document"]` to get the text as the model produces it.
- **`StateSnapshotEvent`**: Each snapshot holds the full document prefix so far, producing the optimistic streaming effect.
- **`ToChatRequestContext` / `AsAGUIEventStreamAsync`**: The AG-UI streaming pipeline that adapts a `RunAgentInput` to a chat request and converts the response updates back into AG-UI events.
- **`confirm_changes`**: A client-side tool call injected after the document is written, so the user can approve the result.

## Rendering on the Client

A UI toolkit such as [CopilotKit](https://copilotkit.ai/) subscribes to the state snapshots and re-renders the document on each one, then shows the confirm or reject prompt when the `confirm_changes` tool call arrives. You can see this scenario running in the [AG-UI Dojo](https://dojo.ag-ui.com/microsoft-agent-framework-dotnet).

## Next Steps

- **[Manage State](state-management.md)**: Learn about state snapshots, deltas, and shared state.
- **[Human-in-the-Loop](human-in-the-loop.md)**: Add approval workflows for tool calls.
- **[Test with Dojo](testing-with-dojo.md)**: Run this scenario against the AG-UI Dojo app.

## Additional Resources

- [AG-UI Overview](index.md)
- [State Management](state-management.md)
- [Agent Framework Documentation](../../overview/index.md)

::: zone-end

::: zone pivot="programming-language-python"

Predictive state updates for Python are covered in the [State Management](state-management.md) tutorial, which shows how to configure predicted state with `predict_state_config` and stream tool arguments as optimistic state updates.

::: zone-end
