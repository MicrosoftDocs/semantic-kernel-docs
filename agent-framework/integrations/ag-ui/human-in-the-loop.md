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

This tutorial demonstrates how to implement human-in-the-loop approval workflows with AG-UI in C#. The C# implementation uses Microsoft.Extensions.AI's `ApprovalRequiredAIFunction` and translates approval requests into AG-UI "client tool calls" that the client handles and responds to.

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

### Define Approval-Required Tools

Create functions and wrap them with `ApprovalRequiredAIFunction`:

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

[Description("Transfer money between accounts.")]
static string TransferMoney(
    [Description("Source account number")] string fromAccount,
    [Description("Destination account number")] string toAccount,
    [Description("Amount to transfer")] decimal amount)
{
    return $"Transferred ${amount:F2} from account {fromAccount} to account {toAccount}";
}

// Create approval-required tools
AITool[] tools =
[
    new ApprovalRequiredAIFunction(AIFunctionFactory.Create(SendEmail)),
    new ApprovalRequiredAIFunction(AIFunctionFactory.Create(TransferMoney)),
    AIFunctionFactory.Create(CheckBalance) // Regular tool, no approval needed
];
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
    public required string FunctionArguments { get; init; }

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
```

### Implement Approval Middleware

Create middleware that translates between Microsoft.Extensions.AI approval types and AG-UI protocol:

```csharp
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

AIAgent agent = baseAgent
    .AsBuilder()
    .Use(runFunc: null, runStreamingFunc: HandleApprovalRequestsMiddleware)
    .Build();

static async IAsyncEnumerable<AgentRunResponseUpdate> HandleApprovalRequestsMiddleware(
    IEnumerable<ChatMessage> messages,
    AgentThread? thread,
    AgentRunOptions? options,
    AIAgent innerAgent,
    [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
{
    // Check if we have an approval response from the client
    FunctionApprovalResponseContent? approvalResponse = messages
        .SelectMany(m => m.Contents)
        .OfType<FunctionResultContent>()
        .Where(frc => frc.CallId.StartsWith("approval_"))
        .Select(frc => DeserializeApprovalResponse(frc, messages))
        .FirstOrDefault(x => x != null);

    // If we have an approval response, inject it into the message stream
    IEnumerable<ChatMessage> modifiedMessages = messages;
    if (approvalResponse != null)
    {
        modifiedMessages = ReplaceToolResultWithApprovalResponse(messages, approvalResponse);
    }

    // Run the agent and intercept any approval requests
    await foreach (AgentRunResponseUpdate update in innerAgent.RunStreamingAsync(
        modifiedMessages, thread, options, cancellationToken))
    {
        // Check if this update contains a FunctionApprovalRequestContent
        FunctionApprovalRequestContent? approvalRequest = update.Contents
            .OfType<FunctionApprovalRequestContent>()
            .FirstOrDefault();

        if (approvalRequest != null)
        {
            // Convert the approval request to a "client tool call"
            FunctionCallContent functionCall = approvalRequest.FunctionCall;
            string approvalId = $"approval_{functionCall.CallId}";

            ApprovalRequest approvalData = new()
            {
                ApprovalId = approvalId,
                FunctionName = functionCall.Name,
                FunctionArguments = functionCall.Arguments?.ToString() ?? "{}",
                Message = $"Approve execution of '{functionCall.Name}'?"
            };

            string approvalJson = JsonSerializer.Serialize(approvalData);

            // Yield a tool call update that represents the approval request
            yield return new AgentRunResponseUpdate(ChatRole.Assistant, [
                new FunctionCallContent(
                    callId: approvalId,
                    name: "request_approval",
                    arguments: new Dictionary<string, object?> { ["request"] = approvalJson })
            ]);

            // Yield a message indicating we're waiting for approval
            yield return new AgentRunResponseUpdate(
                ChatRole.Assistant,
                $"\n\n[Waiting for approval to execute {functionCall.Name}...]");
        }
        else
        {
            yield return update;
        }
    }
}
```

## Client Implementation

### Handle Approval Requests

The client detects tool calls named `"request_approval"` and prompts the user:

```csharp
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.AGUI;
using Microsoft.Extensions.AI;

// Track pending approvals
Dictionary<string, (string callId, ApprovalRequest request)> pendingApprovals = [];

await foreach (AgentRunResponseUpdate update in agent.RunStreamingAsync(
    messages, thread, cancellationToken: cancellationToken))
{
    foreach (AIContent content in update.Contents)
    {
        if (content is FunctionCallContent toolCall && toolCall.Name == "request_approval")
        {
            // Extract and parse the approval request
            string argsJson = toolCall.Arguments?.TryGetValue("request", out object? requestObj) == true
                ? requestObj?.ToString() ?? "{}"
                : "{}";
                
            ApprovalRequest? approvalRequest = JsonSerializer.Deserialize<ApprovalRequest>(argsJson);

            if (approvalRequest != null)
            {
                pendingApprovals[toolCall.CallId] = (toolCall.CallId, approvalRequest);
                DisplayApprovalRequest(approvalRequest);
            }
        }
        else if (content is TextContent textContent)
        {
            Console.Write(textContent.Text);
        }
    }
}
```

### Send Approval Response

After getting user input, send the approval response as a tool result:

```csharp
foreach ((string callId, ApprovalRequest request) in pendingApprovals.Values)
{
    // Get user approval
    Console.Write("\nApprove this action? (yes/no): ");
    string? userInput = Console.ReadLine();
    bool approved = userInput?.ToUpperInvariant() is "YES" or "Y";

    // Create approval response
    ApprovalResponse response = new()
    {
        ApprovalId = request.ApprovalId,
        Approved = approved
    };

    string responseJson = JsonSerializer.Serialize(response);

    // Add the tool result to messages
    messages.Add(new ChatMessage(
        ChatRole.Tool,
        [new FunctionResultContent(callId, responseJson)]));

    // Continue the conversation with the approval response
    await foreach (AgentRunResponseUpdate update in agent.RunStreamingAsync(
        messages, thread, cancellationToken: cancellationToken))
    {
        // Process the continued response...
    }
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

## Complete Sample Code

For a complete working example, see:

- **Server**: [AGUI_Step05_ServerWithApprovals](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/GettingStarted/AGUI/AGUI_Step05_ServerWithApprovals)
- **Client**: [AGUI_Step05_ClientWithApprovals](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/GettingStarted/AGUI/AGUI_Step05_ClientWithApprovals)

## Key Concepts

### Client Tool Pattern

The C# implementation uses a "client tool call" pattern:

- **Approval Request** → Tool call named `"request_approval"` with approval details
- **Approval Response** → Tool result containing the user's decision
- **Middleware** → Translates between Microsoft.Extensions.AI types and AG-UI protocol

This allows the standard `ApprovalRequiredAIFunction` pattern to work across the HTTP+SSE boundary while maintaining consistency with the agent framework's approval model.

### Middleware Responsibilities

The middleware handles:

1. **Outbound**: Converting `FunctionApprovalRequestContent` to client tool calls
2. **Inbound**: Converting tool results back to `FunctionApprovalResponseContent`
3. **Thread Management**: Correlating approval requests with responses via approval IDs

### Approval ID Format

Approval IDs use the format `approval_{originalCallId}` to:

- Identify approval responses vs regular tool results
- Correlate approval responses with their original function calls
- Prevent ID collisions with regular tool calls

## Best Practices

1. **Descriptive Tools**: Provide clear descriptions so users understand what they're approving
2. **Granular Approval**: Request approval for individual sensitive actions
3. **Informative Arguments**: Use descriptive parameter names with good documentation
4. **Timeout Handling**: Set appropriate timeouts for approval requests (e.g., 120 seconds)
5. **Selective Approval**: Mix approval-required and non-approval tools appropriately

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

Here's a client that handles approval requests:

```python
"""AG-UI client with human-in-the-loop support."""

import asyncio
import json
import os

import httpx


class AGUIClient:
    """AG-UI client with approval handling."""

    def __init__(self, server_url: str):
        self.server_url = server_url
        self.thread_id: str | None = None

    async def send_message(self, message: str):
        """Send a message and handle responses including approval requests."""
        request_data = {
            "messages": [
                {"role": "system", "content": "You are a helpful assistant."},
                {"role": "user", "content": message},
            ]
        }

        if self.thread_id:
            request_data["thread_id"] = self.thread_id

        async with httpx.AsyncClient(timeout=120.0) as client:
            async with client.stream(
                "POST",
                self.server_url,
                json=request_data,
                headers={"Accept": "text/event-stream"},
            ) as response:
                response.raise_for_status()

                async for line in response.aiter_lines():
                    if line.startswith("data: "):
                        data = line[6:]
                        try:
                            event = json.loads(data)
                            yield event

                            if event.get("type") == "RUN_STARTED" and not self.thread_id:
                                self.thread_id = event.get("threadId")

                        except json.JSONDecodeError:
                            continue

    async def send_approval_response(self, approval_id: str, approved: bool):
        """Send an approval response to the server."""
        request_data = {
            "type": "APPROVAL_RESPONSE",
            "approvalId": approval_id,
            "approved": approved,
        }

        if self.thread_id:
            request_data["thread_id"] = self.thread_id

        async with httpx.AsyncClient(timeout=120.0) as client:
            async with client.stream(
                "POST",
                self.server_url,
                json=request_data,
                headers={"Accept": "text/event-stream"},
            ) as response:
                response.raise_for_status()

                async for line in response.aiter_lines():
                    if line.startswith("data: "):
                        data = line[6:]
                        try:
                            event = json.loads(data)
                            yield event
                        except json.JSONDecodeError:
                            continue


def display_approval_request(steps: list) -> None:
    """Display approval request details to the user."""
    print("\n\033[93m" + "=" * 60 + "\033[0m")
    print("\033[93mAPPROVAL REQUIRED\033[0m")
    print("\033[93m" + "=" * 60 + "\033[0m")
    
    for i, step in enumerate(steps, 1):
        print(f"\nAction {i}:")
        print(f"  Tool: \033[95m{step.get('toolCallName', 'unknown')}\033[0m")
        print(f"  Arguments:")
        for key, value in step.get("arguments", {}).items():
            print(f"    {key}: {value}")
    
    print("\n\033[93m" + "=" * 60 + "\033[0m")


async def main():
    """Main client loop with approval handling."""
    server_url = os.environ.get("AGUI_SERVER_URL", "http://127.0.0.1:8888/")
    print(f"Connecting to AG-UI server at: {server_url}\n")

    client = AGUIClient(server_url)

    try:
        while True:
            message = input("\nUser (:q or quit to exit): ")
            if not message.strip():
                continue

            if message.lower() in (":q", "quit"):
                break

            print()
            pending_approval = None

            async for event in client.send_message(message):
                event_type = event.get("type", "")

                if event_type == "RUN_STARTED":
                    print("\033[93m[Run Started]\033[0m")

                elif event_type == "TEXT_MESSAGE_CONTENT":
                    print(f"\033[96m{event.get('delta', '')}\033[0m", end="", flush=True)

                elif event_type == "TOOL_CALL_START":
                    tool_name = event.get("toolCallName", "unknown")
                    print(f"\n\033[95m[Calling Tool: {tool_name}]\033[0m")

                elif event_type == "TOOL_CALL_RESULT":
                    content = event.get("content", "")
                    print(f"\033[94m[Tool Result: {content}]\033[0m")

                elif event_type == "APPROVAL_REQUEST":
                    pending_approval = event
                    steps = event.get("steps", [])
                    display_approval_request(steps)
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
