---
title: Agent Background Responses
description: Learn how to handle long-running operations with background responses in Agent Framework
zone_pivot_groups: programming-languages
author: sergeymenshykh
ms.topic: reference
ms.author: semenshi
ms.date: 10/16/2025
ms.service: agent-framework
---

# Agent Background Responses

The Microsoft Agent Framework supports background responses for handling long-running operations that may take time to complete. This feature enables agents to start processing a request and return a continuation token that can be used to poll for results or resume interrupted streams.

## When to Use Background Responses

Background responses are particularly useful for:
- Complex reasoning tasks that require significant processing time
- Operations that may be interrupted by network issues or client timeouts
- Scenarios where you want to start a long-running task and check back later for results

## How Background Responses Work

Background responses use a **continuation token** mechanism to handle long-running operations. When you send a request to an agent with background responses enabled, one of two things happens:

1. **Immediate completion**: The agent completes the task quickly and returns the final response without a continuation token
2. **Background processing**: The agent starts processing in the background and returns a continuation token instead of the final result

The continuation token contains all necessary information to either poll for completion using the non-streaming agent API or resume an interrupted stream with streaming agent API. When the continuation token is `null`, the operation is complete - this happens when a background response has completed, failed, or cannot proceed further (for example, when user input is required).

::: zone pivot="programming-language-csharp"

## Enabling Background Responses

To enable background responses, set the `AllowBackgroundResponses` property to `true` in the `AgentRunOptions`:

```csharp
AgentRunOptions options = new()
{
    AllowBackgroundResponses = true
};
```

> [!NOTE]
> Currently, only agents that use the OpenAI Responses API support background responses: [OpenAI Responses Agent](agent-types/openai-responses-agent.md) and [Azure OpenAI Responses Agent](agent-types/azure-openai-responses-agent.md).

Some agents may not allow explicit control over background responses. These agents can decide autonomously whether to initiate a background response based on the complexity of the operation, regardless of the `AllowBackgroundResponses` setting.

## Non-Streaming Background Responses

For non-streaming scenarios, when you initially run an agent, it may or may not return a continuation token. If no continuation token is returned, it means the operation has completed. If a continuation token is returned, it indicates that the agent has initiated a background response that is still processing and will require polling to retrieve the final result:

```csharp
AgentRunOptions options = new()
{
    AllowBackgroundResponses = true
};

AgentThread thread = agent.GetNewThread();

// Get initial response - may return with or without a continuation token
AgentRunResponse response = await agent.RunAsync("What is the weather like in Amsterdam?", thread, options);

// Continue to poll until the final response is received
while (response.ContinuationToken is not null)
{
        // Wait before polling again.
    await Task.Delay(TimeSpan.FromSeconds(2));

    options.ContinuationToken = response.ContinuationToken;
    response = await agent.RunAsync(thread, options);
}

Console.WriteLine(response.Text);
```

### Key Points:

- The initial call may complete immediately (no continuation token) or start a background operation (with continuation token)
- If no continuation token is returned, the operation is complete and the response contains the final result
- If a continuation token is returned, the agent has started a background process that requires polling
- Use the continuation token from the previous response in subsequent polling calls
- When `ContinuationToken` is `null`, the operation is complete

## Streaming Background Responses

In streaming scenarios, background responses work much like regular streaming responses - the agent streams all updates back to consumers in real-time. However, the key difference is that if the original stream gets interrupted, agents support stream resumption through continuation tokens. Each update includes a continuation token that captures the current state, allowing the stream to be resumed from exactly where it left off by passing this token to subsequent streaming API calls:

```csharp
AgentRunOptions options = new()
{
    AllowBackgroundResponses = true
};

AgentThread thread = agent.GetNewThread();

AgentRunResponseUpdate? latestReceivedUpdate = null;

await foreach (var update in agent.RunStreamingAsync("Tell me a joke about a pirate.", thread, options))
{
    Console.Write(update.Text);
    
    latestReceivedUpdate = update;
    
    // Simulate an interruption
    break;
}

// Resume from interruption point captured by the continuation token
options.ContinuationToken = latestReceivedUpdate?.ContinuationToken;
await foreach (var update in agent.RunStreamingAsync(thread, options))
{
    Console.Write(update.Text);
}
```

### Key Points:

- Each `AgentRunResponseUpdate` contains a continuation token that can be used for resumption
- Store the continuation token from the last received update before interruption
- Use the stored continuation token to resume the stream from the interruption point

::: zone-end

::: zone pivot="programming-language-python"

> [!NOTE]
> Background responses support in Python is coming soon. This feature is currently available in the .NET implementation of Agent Framework.

::: zone-end

## Best Practices

When working with background responses, consider the following best practices:

- **Implement appropriate polling intervals** to avoid overwhelming the service
- **Use exponential backoff** for polling intervals if the operation is taking longer than expected
- **Always check for `null` continuation tokens** to determine when processing is complete
- **Consider storing continuation tokens persistently** for operations that may span user sessions

## Limitations and Considerations

- Background responses are dependent on the underlying AI service supporting long-running operations
- Not all agent types may support background responses
- Network interruptions or client restarts may require special handling to persist continuation tokens

## Next steps

> [!div class="nextstepaction"]
> [Using MCP Tools](../model-context-protocol/using-mcp-tools.md)