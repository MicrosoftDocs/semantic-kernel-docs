---
title: Human-in-the-Loop with AG-UI
description: Learn how to implement approval workflows for tool execution using AG-UI protocol
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 11/07/2025
ms.service: agent-framework
---

# Human-in-the-Loop with AG-UI

::: zone pivot="programming-language-csharp"

This tutorial demonstrates how to implement human-in-the-loop approval workflows with AG-UI in .NET. The .NET implementation uses Microsoft.Extensions.AI's `ApprovalRequiredAIFunction` and translates approval requests into AG-UI "client tool calls" that the client handles and responds to.

## Overview

The C# AG-UI approval pattern works as follows:

1. **Server**: Wraps functions with `ApprovalRequiredAIFunction` to mark them as requiring approval
2. **Middleware**: Intercepts `FunctionApprovalRequestContent` from the agent and converts it to a client tool call
3. **Client**: Receives the tool call, displays approval UI, and sends the approval response as a tool result
4. **Middleware**: Unwraps the approval response and converts it to `FunctionApprovalResponseContent`
5. **Agent**: Continues execution with the user's approval decision

## Prerequisites

- Azure OpenAI resource with a deployed model
- Environment variables:
  - `AZURE_OPENAI_ENDPOINT`
  - `AZURE_OPENAI_DEPLOYMENT_NAME`
- Understanding of [Backend Tool Rendering](backend-tool-rendering.md)

## Server Implementation

### Define Approval-Required Tool

Create a function and wrap it with `ApprovalRequiredAIFunction`:

```csharp
using System.ComponentModel;
using Microsoft.Extensions.AI;

[Description("Send an email to a recipient.")]
static string SendEmail(
    [Description("The email address to send to")] string to,
    [Description("The subject line")] string subject,
    [Description("The email body")] string body)
{
    return $"Email sent to {to} with subject '{subject}'";
}

// Create approval-required tool
AITool[] tools = [new ApprovalRequiredAIFunction(AIFunctionFactory.Create(SendEmail))];
```

### Create Approval Models

Define models for the approval request and response:

```csharp
using System.Text.Json.Serialization;

public sealed class ApprovalRequest
{
    [JsonPropertyName("approval_id")]
    public required string ApprovalId { get; init; }

    [JsonPropertyName("function_name")]
    public required string FunctionName { get; init; }

    [JsonPropertyName("function_arguments")]
    public JsonElement? FunctionArguments { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }
}

public sealed class ApprovalResponse
{
    [JsonPropertyName("approval_id")]
    public required string ApprovalId { get; init; }

    [JsonPropertyName("approved")]
    public required bool Approved { get; init; }
}

[JsonSerializable(typeof(ApprovalRequest))]
[JsonSerializable(typeof(ApprovalResponse))]
[JsonSerializable(typeof(Dictionary<string, object?>))]
internal partial class ApprovalJsonContext : JsonSerializerContext
{
}
```

### Implement Approval Middleware

Create middleware that translates between Microsoft.Extensions.AI approval types and AG-UI protocol:

```csharp
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

// Get JsonSerializerOptions from the configured HTTP JSON options
var jsonOptions = app.Services.GetRequiredService<IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>>().Value;

var agent = baseAgent
    .AsBuilder()
    .Use(runFunc: null, runStreamingFunc: (messages, thread, options, innerAgent, cancellationToken) =>
        HandleApprovalRequestsMiddleware(
            messages,
            thread,
            options,
            innerAgent,
            jsonOptions.SerializerOptions,
            cancellationToken))
    .Build();

static async IAsyncEnumerable<AgentRunResponseUpdate> HandleApprovalRequestsMiddleware(
    IEnumerable<ChatMessage> messages,
    AgentThread? thread,
    AgentRunOptions? options,
    AIAgent innerAgent,
    JsonSerializerOptions jsonSerializerOptions,
    [EnumeratorCancellation] CancellationToken cancellationToken)
{
    // Process incoming approval responses from client
    var modifiedMessages = ProcessIncomingFunctionApprovals(messages, jsonSerializerOptions);

    // Run the agent and intercept any approval requests
    await foreach (var update in innerAgent.RunStreamingAsync(
        modifiedMessages, thread, options, cancellationToken))
    {
        // Check if we need to convert approval request to client tool call
        await foreach (var processedUpdate in ProcessFunctionApprovalRequests(update, jsonSerializerOptions))
        {
            yield return processedUpdate;
        }
    }

    // Local function to process incoming approval responses
    static IEnumerable<ChatMessage> ProcessIncomingFunctionApprovals(
        IEnumerable<ChatMessage> messages,
        JsonSerializerOptions jsonSerializerOptions)
    {
        // Look for "request_approval" tool calls and their matching results
        Dictionary<string, FunctionCallContent> approvalToolCalls = [];
        FunctionResultContent? approvalResult = null;

        foreach (var message in messages)
        {
            foreach (var content in message.Contents)
            {
                if (content is FunctionCallContent { Name: "request_approval" } toolCall)
                {
                    approvalToolCalls[toolCall.CallId] = toolCall;
                }
                else if (content is FunctionResultContent result && approvalToolCalls.ContainsKey(result.CallId))
                {
                    approvalResult = result;
                }
            }
        }

        // If no approval response found, return messages unchanged
        if (approvalResult == null)
        {
            return messages;
        }

        // Deserialize the approval response
        if ((approvalResult.Result as JsonElement?)?.Deserialize(jsonSerializerOptions.GetTypeInfo(typeof(ApprovalResponse))) is not ApprovalResponse response)
        {
            return messages;
        }

        // Extract the original function call details from the approval request
        var originalToolCall = approvalToolCalls[approvalResult.CallId];

        if (originalToolCall.Arguments?.TryGetValue("request", out JsonElement request) != true ||
            request.Deserialize(jsonSerializerOptions.GetTypeInfo(typeof(ApprovalRequest))) is not ApprovalRequest approvalRequest)
        {
            return messages;
        }

        // Deserialize the function arguments from JsonElement
        var functionArguments = approvalRequest.FunctionArguments is { } args
            ? (Dictionary<string, object?>?)args.Deserialize(
                jsonSerializerOptions.GetTypeInfo(typeof(Dictionary<string, object?>)))
            : null;

        var originalFunctionCall = new FunctionCallContent(
            callId: response.ApprovalId,
            name: approvalRequest.FunctionName,
            arguments: functionArguments);

        var functionApprovalResponse = new FunctionApprovalResponseContent(
            response.ApprovalId,
            response.Approved,
            originalFunctionCall);

        // Replace the tool result message with the approval response
        List<ChatMessage> newMessages = [];
        foreach (var message in messages)
        {
            var hasApprovalResult = false;
            foreach (var content in message.Contents)
            {
                if (content is FunctionResultContent { CallId: var callId } && callId == approvalResult.CallId)
                {
                    hasApprovalResult = true;
                    break;
                }
            }

            if (hasApprovalResult)
            {
                newMessages.Add(new ChatMessage(ChatRole.User, [functionApprovalResponse]));
            }
            else
            {
                newMessages.Add(message);
            }
        }

        return newMessages;
    }

    // Local function to process outgoing approval requests
    static async IAsyncEnumerable<AgentRunResponseUpdate> ProcessFunctionApprovalRequests(
        AgentRunResponseUpdate update,
        JsonSerializerOptions jsonSerializerOptions)
    {
        // Check if this update contains a FunctionApprovalRequestContent
        FunctionApprovalRequestContent? approvalRequestContent = null;
        foreach (var content in update.Contents)
        {
            if (content is FunctionApprovalRequestContent request)
            {
                approvalRequestContent = request;
                break;
            }
        }

        // If no approval request, yield the update unchanged
        if (approvalRequestContent == null)
        {
            yield return update;
            yield break;
        }

        // Convert the approval request to a "client tool call"
        var functionCall = approvalRequestContent.FunctionCall;
        var approvalId = approvalRequestContent.Id;

        // Serialize the function arguments as JsonElement
        var argsElement = functionCall.Arguments?.Count > 0
            ? JsonSerializer.SerializeToElement(functionCall.Arguments, jsonSerializerOptions.GetTypeInfo(typeof(IDictionary<string, object?>)))
            : (JsonElement?)null;

        var approvalData = new ApprovalRequest
        {
            ApprovalId = approvalId,
            FunctionName = functionCall.Name,
            FunctionArguments = argsElement,
            Message = $"Approve execution of '{functionCall.Name}'?"
        };

        var approvalJson = JsonSerializer.Serialize(approvalData, jsonSerializerOptions.GetTypeInfo(typeof(ApprovalRequest)));

        // Yield a tool call update that represents the approval request
        yield return new AgentRunResponseUpdate(ChatRole.Assistant, [
            new FunctionCallContent(
                callId: approvalId,
                name: "request_approval",
                arguments: new Dictionary<string, object?> { ["request"] = approvalJson })
        ]);
    }
}
```

## Client Implementation

## Client Implementation

### Implement Client-Side Middleware

The client wraps the agent with middleware that translates `"request_approval"` tool calls to `FunctionApprovalRequestContent`:

```csharp
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.AGUI;
using Microsoft.Extensions.AI;

// Get JsonSerializerOptions from the client
var jsonSerializerOptions = agent.ChatOptions?.JsonSerializerOptions ?? JsonSerializerOptions.Default;

// Wrap the agent with approval middleware
var wrappedAgent = agent
    .AsBuilder()
    .Use(runFunc: null, runStreamingFunc: (messages, thread, options, innerAgent, cancellationToken) =>
        HandleApprovalRequestsClientMiddleware(
            messages,
            thread,
            options,
            innerAgent,
            jsonSerializerOptions,
            cancellationToken))
    .Build();

static async IAsyncEnumerable<AgentRunResponseUpdate> HandleApprovalRequestsClientMiddleware(
    IEnumerable<ChatMessage> messages,
    AgentThread? thread,
    AgentRunOptions? options,
    AIAgent innerAgent,
    JsonSerializerOptions jsonSerializerOptions,
    [EnumeratorCancellation] CancellationToken cancellationToken)
{
    await foreach (var update in innerAgent.RunStreamingAsync(messages, thread, options, cancellationToken))
    {
        // Check if this update contains a "request_approval" tool call
        FunctionCallContent? approvalToolCall = null;
        foreach (var content in update.Contents)
        {
            if (content is FunctionCallContent { Name: "request_approval" } toolCall)
            {
                approvalToolCall = toolCall;
                break;
            }
        }

        // If no approval tool call, yield the update unchanged
        if (approvalToolCall == null)
        {
            yield return update;
            continue;
        }

        // Extract and parse the approval request
        if (approvalToolCall.Arguments?.TryGetValue("request", out JsonElement request) != true ||
            request.Deserialize(jsonSerializerOptions.GetTypeInfo(typeof(ApprovalRequest))) is not ApprovalRequest approvalRequest)
        {
            yield return update;
            continue;
        }

        // Deserialize the function arguments from JsonElement
        var functionArguments = approvalRequest.FunctionArguments is { } args
            ? (Dictionary<string, object?>?)args.Deserialize(
                jsonSerializerOptions.GetTypeInfo(typeof(Dictionary<string, object?>)))
            : null;

        var originalFunctionCall = new FunctionCallContent(
            callId: approvalRequest.ApprovalId,
            name: approvalRequest.FunctionName,
            arguments: functionArguments);

        // Convert to FunctionApprovalRequestContent
        yield return new AgentRunResponseUpdate(ChatRole.Assistant, [
            new FunctionApprovalRequestContent(
                approvalRequest.ApprovalId,
                originalFunctionCall)
        ]);
    }
}
```

### Handle Approval Requests and Send Responses

The consuming code processes approval requests and automatically continues until no more approvals are needed:

```csharp
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.AGUI;
using Microsoft.Extensions.AI;

List<AIContent> approvalResponses = [];

do
{
    approvalResponses.Clear();
    
    await foreach (AgentRunResponseUpdate update in wrappedAgent.RunStreamingAsync(
        messages, thread, cancellationToken: cancellationToken))
    {
        foreach (AIContent content in update.Contents)
        {
            if (content is FunctionApprovalRequestContent approvalRequest)
            {
                DisplayApprovalRequest(approvalRequest);
                
                // Get user approval immediately
                Console.Write($"\nApprove '{approvalRequest.FunctionCall.Name}'? (yes/no): ");
                string? userInput = Console.ReadLine();
                bool approved = userInput?.ToUpperInvariant() is "YES" or "Y";

                // Create and collect approval response
                var approvalResponse = approvalRequest.CreateResponse(approved);
                approvalResponses.Add(approvalResponse);
            }
            else if (content is TextContent textContent)
            {
                Console.Write(textContent.Text);
            }
        }
    }

    // If we collected approval responses, add them to messages for next iteration
    if (approvalResponses.Count > 0)
    {
        messages.Add(new ChatMessage(ChatRole.User, approvalResponses.ToArray()));
    }
}
while (approvalResponses.Count > 0);

static void DisplayApprovalRequest(FunctionApprovalRequestContent approvalRequest)
{
    Console.WriteLine();
    Console.WriteLine("============================================================");
    Console.WriteLine("APPROVAL REQUIRED");
    Console.WriteLine("============================================================");
    Console.WriteLine($"Function: {approvalRequest.FunctionCall.Name}");
    
    if (approvalRequest.FunctionCall.Arguments != null)
    {
        Console.WriteLine("Arguments:");
        foreach (var arg in approvalRequest.FunctionCall.Arguments)
        {
            Console.WriteLine($"  {arg.Key} = {arg.Value}");
        }
    }
    
    Console.WriteLine("============================================================");
}
```

## Example Interaction

```
User (:q or quit to exit): Send an email to user@example.com about the meeting

[Run Started - Thread: thread_abc123, Run: run_xyz789]

============================================================
APPROVAL REQUIRED
============================================================

Function: SendEmail
Arguments: {"to":"user@example.com","subject":"Meeting","body":"..."}
Message: Approve execution of 'SendEmail'?

============================================================

[Waiting for approval to execute SendEmail...]
[Run Finished - Thread: thread_abc123]

Approve this action? (yes/no): yes

[Sending approval response: APPROVED]

[Run Resumed - Thread: thread_abc123]
Email sent to user@example.com with subject 'Meeting'
[Run Finished]
```

## Key Concepts

### Client Tool Pattern

The C# implementation uses a "client tool call" pattern:

- **Approval Request** → Tool call named `"request_approval"` with approval details
- **Approval Response** → Tool result containing the user's decision
- **Middleware** → Translates between Microsoft.Extensions.AI types and AG-UI protocol

This allows the standard `ApprovalRequiredAIFunction` pattern to work across the HTTP+SSE boundary while maintaining consistency with the agent framework's approval model.

### Middleware Responsibilities

The middleware handles:

1. **Outbound**: Converting `FunctionApprovalRequestContent` to client tool calls named `"request_approval"`
2. **Inbound**: Tracking approval tool calls by CallId and converting their results to `FunctionApprovalResponseContent`
3. **Thread Management**: Correlating approval requests with responses via call IDs
4. **Function Call Reconstruction**: Capturing and restoring complete function call details (name, arguments) for approval flow

### Tool Call Matching

The middleware identifies approval-related messages by:

- **Tool Call Name**: Looking for tool calls with `Name == "request_approval"`
- **CallId Correlation**: Matching `FunctionResultContent` to `FunctionCallContent` using the same CallId
- **Argument Extraction**: Deserializing approval data from the tool call's `arguments["request"]` field

This approach avoids string prefix manipulation and properly tracks the relationship between approval requests and responses.

## Next Steps

- **[Learn State Management](state-management.md)**: Manage shared state with approval workflows
- **[Explore Function Tools](../../tutorials/agents/function-tools-approvals.md)**: Learn more about approval patterns in Agent Framework

::: zone-end

::: zone pivot="programming-language-python"

This tutorial shows you how to implement human-in-the-loop workflows with AG-UI, where users must approve tool executions before they are performed. This is essential for sensitive operations like financial transactions, data modifications, or actions that have significant consequences.

## Prerequisites

Before you begin, ensure you have completed the [Backend Tool Rendering](backend-tool-rendering.md) tutorial and understand:

- How to create function tools
- How AG-UI streams tool events
- Basic server and client setup

## What is Human-in-the-Loop?

Human-in-the-Loop (HITL) is a pattern where the agent requests user approval before executing certain operations. With AG-UI:

- The agent generates tool calls as usual
- Instead of executing immediately, the server sends approval requests to the client
- The client displays the request and prompts the user
- The user approves or rejects the action
- The server receives the response and proceeds accordingly

### Benefits

- **Safety**: Prevent unintended actions from being executed
- **Transparency**: Users see exactly what the agent wants to do
- **Control**: Users have final say over sensitive operations
- **Compliance**: Meet regulatory requirements for human oversight

## Marking Tools for Approval

To require approval for a tool, use the `approval_mode` parameter in the `@ai_function` decorator:

```python
from agent_framework import ai_function
from typing import Annotated
from pydantic import Field


@ai_function(approval_mode="always_require")
def send_email(
    to: Annotated[str, Field(description="Email recipient address")],
    subject: Annotated[str, Field(description="Email subject line")],
    body: Annotated[str, Field(description="Email body content")],
) -> str:
    """Send an email to the specified recipient."""
    # Send email logic here
    return f"Email sent to {to} with subject '{subject}'"


@ai_function(approval_mode="always_require")
def delete_file(
    filepath: Annotated[str, Field(description="Path to the file to delete")],
) -> str:
    """Delete a file from the filesystem."""
    # Delete file logic here
    return f"File {filepath} has been deleted"
```

### Approval Modes

- **`always_require`**: Always request approval before execution
- **`never_require`**: Never request approval (default behavior)
- **`conditional`**: Request approval based on certain conditions (custom logic)

## Creating a Server with Human-in-the-Loop

Here's a complete server implementation with approval-required tools:

```python
"""AG-UI server with human-in-the-loop."""

import os
from typing import Annotated

from agent_framework import ChatAgent, ai_function
from agent_framework.azure import AzureOpenAIChatClient
from agent_framework_ag_ui import AgentFrameworkAgent, add_agent_framework_fastapi_endpoint
from fastapi import FastAPI
from pydantic import Field


# Tools that require approval
@ai_function(approval_mode="always_require")
def transfer_money(
    from_account: Annotated[str, Field(description="Source account number")],
    to_account: Annotated[str, Field(description="Destination account number")],
    amount: Annotated[float, Field(description="Amount to transfer")],
    currency: Annotated[str, Field(description="Currency code")] = "USD",
) -> str:
    """Transfer money between accounts."""
    return f"Transferred {amount} {currency} from {from_account} to {to_account}"


@ai_function(approval_mode="always_require")
def cancel_subscription(
    subscription_id: Annotated[str, Field(description="Subscription identifier")],
) -> str:
    """Cancel a subscription."""
    return f"Subscription {subscription_id} has been cancelled"


# Regular tools (no approval required)
@ai_function
def check_balance(
    account: Annotated[str, Field(description="Account number")],
) -> str:
    """Check account balance."""
    # Simulated balance check
    return f"Account {account} balance: $5,432.10 USD"


# Read configuration
endpoint = os.environ.get("AZURE_OPENAI_ENDPOINT")
deployment_name = os.environ.get("AZURE_OPENAI_DEPLOYMENT_NAME")

if not endpoint or not deployment_name:
    raise ValueError("AZURE_OPENAI_ENDPOINT and AZURE_OPENAI_DEPLOYMENT_NAME are required")

# Create agent with tools
agent = ChatAgent(
    name="BankingAssistant",
    instructions="You are a banking assistant. Help users with their banking needs. Always confirm details before performing transfers.",
    chat_client=AzureOpenAIChatClient(
        endpoint=endpoint,
        deployment_name=deployment_name,
    ),
    tools=[transfer_money, cancel_subscription, check_balance],
)

# Wrap agent to enable human-in-the-loop
wrapped_agent = AgentFrameworkAgent(
    agent=agent,
    require_confirmation=True,  # Enable human-in-the-loop
)

# Create FastAPI app
app = FastAPI(title="AG-UI Banking Assistant")
add_agent_framework_fastapi_endpoint(app, wrapped_agent, "/")

if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="127.0.0.1", port=8888)
```

### Key Concepts

- **`AgentFrameworkAgent` wrapper**: Enables AG-UI protocol features like human-in-the-loop
- **`require_confirmation=True`**: Activates approval workflow for marked tools
- **Tool-level control**: Only tools marked with `approval_mode="always_require"` will request approval

## Understanding Approval Events

When a tool requires approval, the client receives these events:

### Approval Request Event

```python
{
    "type": "APPROVAL_REQUEST",
    "approvalId": "approval_abc123",
    "steps": [
        {
            "toolCallId": "call_xyz789",
            "toolCallName": "transfer_money",
            "arguments": {
                "from_account": "1234567890",
                "to_account": "0987654321",
                "amount": 500.00,
                "currency": "USD"
            }
        }
    ],
    "message": "Do you approve the following actions?"
}
```

### Approval Response Format

The client must send an approval response:

```python
# Approve
{
    "type": "APPROVAL_RESPONSE",
    "approvalId": "approval_abc123",
    "approved": True
}

# Reject
{
    "type": "APPROVAL_RESPONSE",
    "approvalId": "approval_abc123",
    "approved": False
}
```

## Client with Approval Support

Here's a client using `AGUIChatClient` that handles approval requests:

```python
"""AG-UI client with human-in-the-loop support."""

import asyncio
import os

from agent_framework import ChatAgent, ToolCallContent, ToolResultContent
from agent_framework_ag_ui import AGUIChatClient


def display_approval_request(update) -> None:
    """Display approval request details to the user."""
    print("\n\033[93m" + "=" * 60 + "\033[0m")
    print("\033[93mAPPROVAL REQUIRED\033[0m")
    print("\033[93m" + "=" * 60 + "\033[0m")
    
    # Display tool call details from update contents
    for i, content in enumerate(update.contents, 1):
        if isinstance(content, ToolCallContent):
            print(f"\nAction {i}:")
            print(f"  Tool: \033[95m{content.name}\033[0m")
            print(f"  Arguments:")
            for key, value in (content.arguments or {}).items():
                print(f"    {key}: {value}")
    
    print("\n\033[93m" + "=" * 60 + "\033[0m")


async def main():
    """Main client loop with approval handling."""
    server_url = os.environ.get("AGUI_SERVER_URL", "http://127.0.0.1:8888/")
    print(f"Connecting to AG-UI server at: {server_url}\n")

    # Create AG-UI chat client
    chat_client = AGUIChatClient(server_url=server_url)
    
    # Create agent with the chat client
    agent = ChatAgent(
        name="ClientAgent",
        chat_client=chat_client,
        instructions="You are a helpful assistant.",
    )

    # Get a thread for conversation continuity
    thread = agent.get_new_thread()

    try:
        while True:
            message = input("\nUser (:q or quit to exit): ")
            if not message.strip():
                continue

            if message.lower() in (":q", "quit"):
                break

            print("\nAssistant: ", end="", flush=True)
            pending_approval_update = None

            async for update in agent.run_stream(message, thread=thread):
                # Check if this is an approval request
                # (Approval requests are detected by specific metadata or content markers)
                if update.additional_properties and update.additional_properties.get("requires_approval"):
                    pending_approval_update = update
                    display_approval_request(update)
                    break  # Exit the loop to handle approval

                elif event_type == "RUN_FINISHED":
                    print(f"\n\033[92m[Run Finished]\033[0m")

                elif event_type == "RUN_ERROR":
                    error_msg = event.get("message", "Unknown error")
                    print(f"\n\033[91m[Error: {error_msg}]\033[0m")

            # Handle approval request
            if pending_approval:
                approval_id = pending_approval.get("approvalId")
                user_choice = input("\nApprove this action? (yes/no): ").strip().lower()
                approved = user_choice in ("yes", "y")

                print(f"\n\033[93m[Sending approval response: {approved}]\033[0m\n")

                async for event in client.send_approval_response(approval_id, approved):
                    event_type = event.get("type", "")

                    if event_type == "TEXT_MESSAGE_CONTENT":
                        print(f"\033[96m{event.get('delta', '')}\033[0m", end="", flush=True)

                    elif event_type == "TOOL_CALL_RESULT":
                        content = event.get("content", "")
                        print(f"\033[94m[Tool Result: {content}]\033[0m")

                    elif event_type == "RUN_FINISHED":
                        print(f"\n\033[92m[Run Finished]\033[0m")

                    elif event_type == "RUN_ERROR":
                        error_msg = event.get("message", "Unknown error")
                        print(f"\n\033[91m[Error: {error_msg}]\033[0m")

            print()

    except KeyboardInterrupt:
        print("\n\nExiting...")
    except Exception as e:
        print(f"\n\033[91mError: {e}\033[0m")


if __name__ == "__main__":
    asyncio.run(main())
```

## Example Interaction

With the server and client running:

```
User (:q or quit to exit): Transfer $500 from account 1234567890 to account 0987654321

[Run Started]
============================================================
APPROVAL REQUIRED
============================================================

Action 1:
  Tool: transfer_money
  Arguments:
    from_account: 1234567890
    to_account: 0987654321
    amount: 500.0
    currency: USD

============================================================

Approve this action? (yes/no): yes

[Sending approval response: True]

[Tool Result: Transferred 500.0 USD from 1234567890 to 0987654321]
The transfer of $500 from account 1234567890 to account 0987654321 has been completed successfully.
[Run Finished]
```

If the user rejects:

```
Approve this action? (yes/no): no

[Sending approval response: False]

I understand. The transfer has been cancelled and no money was moved.
[Run Finished]
```

## Custom Confirmation Messages

You can customize the approval messages by providing a custom confirmation strategy:

```python
from typing import Any
from agent_framework_ag_ui import AgentFrameworkAgent, ConfirmationStrategy


class BankingConfirmationStrategy(ConfirmationStrategy):
    """Custom confirmation messages for banking operations."""
    
    def on_approval_accepted(self, steps: list[dict[str, Any]]) -> str:
        """Message when user approves the action."""
        tool_name = steps[0].get("toolCallName", "action")
        return f"Thank you for confirming. Proceeding with {tool_name}..."
    
    def on_approval_rejected(self, steps: list[dict[str, Any]]) -> str:
        """Message when user rejects the action."""
        return "Action cancelled. No changes have been made to your account."
    
    def on_state_confirmed(self) -> str:
        """Message when state changes are confirmed."""
        return "Changes confirmed and applied."
    
    def on_state_rejected(self) -> str:
        """Message when state changes are rejected."""
        return "Changes discarded."


# Use custom strategy
wrapped_agent = AgentFrameworkAgent(
    agent=agent,
    require_confirmation=True,
    confirmation_strategy=BankingConfirmationStrategy(),
)
```

## Best Practices

### Clear Tool Descriptions

Provide detailed descriptions so users understand what they're approving:

```python
@ai_function(approval_mode="always_require")
def delete_database(
    database_name: Annotated[str, Field(description="Name of the database to permanently delete")],
) -> str:
    """
    Permanently delete a database and all its contents.
    
    WARNING: This action cannot be undone. All data in the database will be lost.
    Use with extreme caution.
    """
    # Implementation
    pass
```

### Granular Approval

Request approval for individual sensitive actions rather than batching:

```python
# Good: Individual approval per transfer
@ai_function(approval_mode="always_require")
def transfer_money(...): pass

# Avoid: Batching multiple sensitive operations
# Users should approve each operation separately
```

### Informative Arguments

Use descriptive parameter names and provide context:

```python
@ai_function(approval_mode="always_require")
def purchase_item(
    item_name: Annotated[str, Field(description="Name of the item to purchase")],
    quantity: Annotated[int, Field(description="Number of items to purchase")],
    price_per_item: Annotated[float, Field(description="Price per item in USD")],
    total_cost: Annotated[float, Field(description="Total cost including tax and shipping")],
) -> str:
    """Purchase items from the store."""
    pass
```

### Timeout Handling

Set appropriate timeouts for approval requests:

```python
# Client side
async with httpx.AsyncClient(timeout=120.0) as client:  # 2 minutes for user to respond
    # Handle approval
    pass
```

## Selective Approval

You can mix tools that require approval with those that don't:

```python
# No approval needed for read-only operations
@ai_function
def get_account_balance(...): pass

@ai_function
def list_transactions(...): pass

# Approval required for write operations
@ai_function(approval_mode="always_require")
def transfer_funds(...): pass

@ai_function(approval_mode="always_require")
def close_account(...): pass
```

## Next Steps

Now that you understand human-in-the-loop, you can:

- **[Learn State Management](state-management.md)**: Manage shared state with approval workflows
- **[Explore Advanced Patterns](../../tutorials/agents/function-tools-approvals.md)**: Learn more about approval patterns in Agent Framework

## Additional Resources

- [AG-UI Overview](index.md)
- [Backend Tool Rendering](backend-tool-rendering.md)
- [Function Tools with Approvals](../../tutorials/agents/function-tools-approvals.md)

::: zone-end
