---
title: State Management with AG-UI
description: Learn how to synchronize state between client and server using AG-UI protocol
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 08/11/2026
ms.service: agent-framework
---

<!--
  Language parity table – keep in sync when adding/removing sections.

  | Section                    | C# | Python | Go | Notes |
  |----------------------------|:--:|:------:|:--:|-------|
  | Read client state          | ✅ |   ✅   | ❌ | Not documented for Go |
  | State snapshots and deltas | ✅ |   ✅   | ❌ | Go zone demonstrates generic data middleware |
  | Predictive state           | ✅ |   ✅   | ❌ | Explicit SDK-specific mappings |
-->

# State Management with AG-UI

AG-UI defines state events and request fields for sharing application state between a client and an agent endpoint. The implementation and supported state patterns vary by MAF SDK.

## Prerequisites

Before you begin, ensure you understand:

- [Getting Started with AG-UI](getting-started.md)
- [Backend Tool Rendering](backend-tool-rendering.md)
<!-- - [Human-in-the-Loop](human-in-the-loop.md) -->

## What is State Management?

AG-UI state can provide:

- **Shared State**: Both client and server maintain a synchronized view of application state
- **Client and server updates**: Applications can send state in requests and emit state events
- **Real-time Updates**: Changes are streamed immediately using state events
- **Predictive Updates**: An SDK can map tool-call progress to optimistic UI state
- **Structured Data**: State follows a JSON schema for validation

### Use Cases

State management is valuable for:

- **Generative UI**: Build UI components based on agent-controlled state
- **Form Building**: Agent populates form fields as it gathers information
- **Progress Tracking**: Show real-time progress of multi-step operations
- **Interactive Dashboards**: Display data that updates as the agent processes it
- **Collaborative Editing**: Multiple users see consistent state updates

::: zone pivot="programming-language-csharp"

AG-UI state is client-visible JSON associated with a run. In .NET, the integration provides two explicit mechanisms:

- Read state supplied by the client from the originating `RunAgentInput`.
- Map selected tool calls or results to AG-UI state events with `AGUIStreamOptions`.

State mapping is opt-in. Arbitrary tool results don't automatically become shared state.

## Read client state

`MapAGUIServer` stores the originating `RunAgentInput` on `ChatOptions`. A delegating agent or chat-client middleware can recover it with `TryGetRunAgentInput`:

```csharp
using System.Text.Json;
using AGUI.Abstractions;
using AGUI.Server;
using Microsoft.Extensions.AI;

static bool TryGetClientState(ChatOptions options, out JsonElement state)
{
    if (options.TryGetRunAgentInput(out RunAgentInput? input) &&
        input.State is { ValueKind: not JsonValueKind.Undefined } value)
    {
        state = value;
        return true;
    }

    state = default;
    return false;
}
```

Client state is request input. Validate its shape and values before using it in prompts, routing, or privileged operations.

## Emit a state snapshot

Map a tool result to `STATE_SNAPSHOT` when the tool returns the complete state:

```csharp
using AGUI.Server;

AGUIStreamOptions streamOptions = new AGUIStreamOptions()
    .MapResultAsStateSnapshot("generate_recipe");

app.MapAGUIServer("/", agent).WithMetadata(streamOptions);
```

`MapResultAsStateSnapshot` requires the `FunctionResultContent.Result` value to be a `JsonElement`. Serialize a POCO, dictionary, or collection to `JsonElement` in the tool before returning it. The result of `generate_recipe` then becomes the snapshot and replaces the client's current shared state.

For other result types, use `MapResult` with a custom mapper that constructs the `StateSnapshotEvent`.

## Emit state deltas

Map a tool result to `STATE_DELTA` when it returns an [RFC 6902 JSON Patch](https://datatracker.ietf.org/doc/html/rfc6902):

```csharp
AGUIStreamOptions streamOptions = new AGUIStreamOptions()
    .MapResultAsStateSnapshot("create_plan")
    .MapResultAsStateDelta("update_plan_step");

app.MapAGUIServer("/", agent).WithMetadata(streamOptions);
```

Use a snapshot to initialize or replace state and deltas for incremental changes.

`MapResultAsStateDelta` also requires a `JsonElement` result. The element must contain an [RFC 6902 JSON Patch](https://datatracker.ietf.org/doc/html/rfc6902) array. Use `MapResult` with a custom mapper if the tool returns another representation.

## Map tool calls to state

`AGUIStreamOptions.MapCall` maps a selected `FunctionCallContent` to additional AG-UI events emitted after the normal tool-call events. Use it when state derives from tool arguments rather than the tool result:

```csharp
AGUIStreamOptions streamOptions = new AGUIStreamOptions()
    .MapCall("write_document", call =>
    {
        if (call.Arguments?.TryGetValue("document", out object? document) is not true)
        {
            return [];
        }

        JsonElement snapshot = JsonSerializer.SerializeToElement(new { document });
        return [new StateSnapshotEvent { Snapshot = snapshot }];
    });

app.MapAGUIServer("/", agent).WithMetadata(streamOptions);
```

The application owns the mapping and the state shape. `MapCall` doesn't infer state from arbitrary tool arguments or suppress normal tool execution. Incremental updates require the underlying model client to expose streamed tool-call arguments and the application to configure the corresponding argument extraction.

## Receive state in a .NET client

The AG-UI .NET client surfaces state protocol events through `ChatResponseUpdate.RawRepresentation`:

```csharp
await foreach (AgentResponseUpdate update in agent.RunStreamingAsync(messages, session))
{
    if (update.AsChatResponseUpdate().RawRepresentation is StateSnapshotEvent snapshot)
    {
        JsonElement state = snapshot.Snapshot;
    }
    else if (update.AsChatResponseUpdate().RawRepresentation is StateDeltaEvent delta)
    {
        JsonElement changes = delta.Delta;
    }
}
```

The client is responsible for retaining and applying shared state, then sending the current state on later requests when the application requires it.

## Next steps

> [!div class="nextstepaction"]
> [Review workflow support with AG-UI](./workflows.md)

::: zone-end

::: zone pivot="programming-language-python"

## Define State Models

First, define Pydantic models for your state structure. This ensures type safety and validation:

```python
from enum import Enum
from pydantic import BaseModel, Field


class SkillLevel(str, Enum):
    """The skill level required for the recipe."""
    BEGINNER = "Beginner"
    INTERMEDIATE = "Intermediate"
    ADVANCED = "Advanced"


class CookingTime(str, Enum):
    """The cooking time of the recipe."""
    FIVE_MIN = "5 min"
    FIFTEEN_MIN = "15 min"
    THIRTY_MIN = "30 min"
    FORTY_FIVE_MIN = "45 min"
    SIXTY_PLUS_MIN = "60+ min"


class Ingredient(BaseModel):
    """An ingredient with its details."""
    icon: str = Field(..., description="Emoji icon representing the ingredient (e.g., 🥕)")
    name: str = Field(..., description="Name of the ingredient")
    amount: str = Field(..., description="Amount or quantity of the ingredient")


class Recipe(BaseModel):
    """A complete recipe."""
    title: str = Field(..., description="The title of the recipe")
    skill_level: SkillLevel = Field(..., description="The skill level required")
    special_preferences: list[str] = Field(
        default_factory=list, description="Dietary preferences (e.g., Vegetarian, Gluten-free)"
    )
    cooking_time: CookingTime = Field(..., description="The estimated cooking time")
    ingredients: list[Ingredient] = Field(..., description="Complete list of ingredients")
    instructions: list[str] = Field(..., description="Step-by-step cooking instructions")
```

## State Schema

Define a state schema to specify the structure and types of your state:

```python
state_schema = {
    "recipe": {"type": "object", "description": "The current recipe"},
}
```

> [!NOTE]
> The state schema uses a simple format with `type` and optional `description`. The actual structure is defined by your Pydantic models.

## Predictive State Updates

Predictive state updates stream tool arguments to the state as the LLM generates them, enabling optimistic UI updates:

```python
predict_state_config = {
    "recipe": {"tool": "update_recipe", "tool_argument": "recipe"},
}
```

This configuration maps the `recipe` state field to the `recipe` argument of the `update_recipe` tool. When the agent calls the tool, the arguments stream to the state in real-time as the LLM generates them.

## Define State Update Tool

Create a tool function that accepts your Pydantic model:

```python
from agent_framework import tool


@tool
def update_recipe(recipe: Recipe) -> str:
    """Update the recipe with new or modified content.

    You MUST write the complete recipe with ALL fields, even when changing only a few items.
    When modifying an existing recipe, include ALL existing ingredients and instructions plus your changes.
    NEVER delete existing data - only add or modify.

    Args:
        recipe: The complete recipe object with all details

    Returns:
        Confirmation that the recipe was updated
    """
    return "Recipe updated."
```

> [!IMPORTANT]
> The tool function's parameter name (`recipe`) must match the `tool_argument` in your `predict_state_config`.

## Create the Agent with State Management

Here's a complete server implementation with state management:

```python
"""AG-UI server with state management."""

from agent_framework import Agent
from agent_framework.openai import OpenAIChatCompletionClient
from agent_framework_ag_ui import (
    AgentFrameworkAgent,
    add_agent_framework_fastapi_endpoint,
)
from azure.identity import AzureCliCredential
from fastapi import FastAPI

# Create the chat agent with tools
agent = Agent(
    name="recipe_agent",
    instructions="""You are a helpful recipe assistant that creates and modifies recipes.

    CRITICAL RULES:
    1. You will receive the current recipe state in the system context
    2. To update the recipe, you MUST use the update_recipe tool
    3. When modifying a recipe, ALWAYS include ALL existing data plus your changes in the tool call
    4. NEVER delete existing ingredients or instructions - only add or modify
    5. After calling the tool, provide a brief conversational message (1-2 sentences)

    When creating a NEW recipe:
    - Provide all required fields: title, skill_level, cooking_time, ingredients, instructions
    - Use actual emojis for ingredient icons (🥕 🧄 🧅 🍅 🌿 🍗 🥩 🧀)
    - Leave special_preferences empty unless specified
    - Message: "Here's your recipe!" or similar

    When MODIFYING or IMPROVING an existing recipe:
    - Include ALL existing ingredients + any new ones
    - Include ALL existing instructions + any new/modified ones
    - Update other fields as needed
    - Message: Explain what you improved (e.g., "I upgraded the ingredients to premium quality")
    - When asked to "improve", enhance with:
      * Better ingredients (upgrade quality, add complementary flavors)
      * More detailed instructions
      * Professional techniques
      * Adjust skill_level if complexity changes
      * Add relevant special_preferences

    Example improvements:
    - Upgrade "chicken" → "organic free-range chicken breast"
    - Add herbs: basil, oregano, thyme
    - Add aromatics: garlic, shallots
    - Add finishing touches: lemon zest, fresh parsley
    - Make instructions more detailed and professional
    """,
    client=OpenAIChatCompletionClient(
        model=deployment_name,
        azure_endpoint=endpoint,
        api_version=os.getenv("AZURE_OPENAI_API_VERSION"),
        credential=AzureCliCredential(),
    ),
    tools=[update_recipe],
)

# Wrap agent with state management
recipe_agent = AgentFrameworkAgent(
    agent=agent,
    name="RecipeAgent",
    description="Creates and modifies recipes with streaming state updates",
    state_schema={
        "recipe": {"type": "object", "description": "The current recipe"},
    },
    predict_state_config={
        "recipe": {"tool": "update_recipe", "tool_argument": "recipe"},
    },
)

# Create FastAPI app
app = FastAPI(title="AG-UI Recipe Assistant")
add_agent_framework_fastapi_endpoint(app, recipe_agent, "/")

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="127.0.0.1", port=8888)
```

### Key Concepts

- **Pydantic Models**: Define structured state with type safety and validation
- **State Schema**: Simple format specifying state field types
- **Predictive State Config**: Maps state fields to tool arguments for streaming updates
- **State Injection**: Current state is automatically injected as system messages to provide context
- **Complete Updates**: Tools must write the complete state, not just deltas
- **Confirmation Strategy**: Customize approval messages for your domain (recipe, document, task planning, etc.)

## Understanding State Events

### State Snapshot Event

A complete snapshot of the current state, emitted when the tool completes:

```json
{
    "type": "STATE_SNAPSHOT",
    "snapshot": {
        "recipe": {
            "title": "Classic Pasta Carbonara",
            "skill_level": "Intermediate",
            "special_preferences": ["Authentic Italian"],
            "cooking_time": "30 min",
            "ingredients": [
                {"icon": "🍝", "name": "Spaghetti", "amount": "400g"},
                {"icon": "🥓", "name": "Guanciale or bacon", "amount": "200g"},
                {"icon": "🥚", "name": "Egg yolks", "amount": "4"},
                {"icon": "🧀", "name": "Pecorino Romano", "amount": "100g grated"},
                {"icon": "🧂", "name": "Black pepper", "amount": "To taste"}
            ],
            "instructions": [
                "Bring a large pot of salted water to boil",
                "Cut guanciale into small strips and fry until crispy",
                "Beat egg yolks with grated Pecorino and black pepper",
                "Cook spaghetti until al dente",
                "Reserve 1 cup pasta water, then drain pasta",
                "Remove pan from heat, add hot pasta to guanciale",
                "Quickly stir in egg mixture, adding pasta water to create creamy sauce",
                "Serve immediately with extra Pecorino and black pepper"
            ]
        }
    }
}
```

### State Delta Event

Incremental state updates using JSON Patch format, emitted as the LLM streams tool arguments:

```json
{
    "type": "STATE_DELTA",
    "delta": [
        {
            "op": "replace",
            "path": "/recipe",
            "value": {
                "title": "Classic Pasta Carbonara",
                "skill_level": "Intermediate",
                "cooking_time": "30 min",
                "ingredients": [
                    {"icon": "🍝", "name": "Spaghetti", "amount": "400g"}
                ],
                "instructions": ["Bring a large pot of salted water to boil"]
            }
        }
    ]
}
```

> [!NOTE]
> State delta events stream in real-time as the LLM generates the tool arguments, providing optimistic UI updates. The final state snapshot is emitted when the tool completes execution.

## Client Implementation

The `agent_framework_ag_ui` package provides `AGUIChatClient` for connecting to AG-UI servers, bringing Python client experience to parity with .NET:

```python
"""AG-UI client with state management."""

import asyncio
import json
import os
from typing import Any

from agent_framework import Agent, Message, Role
from agent_framework_ag_ui import AGUIChatClient


async def main():
    """Example client with state tracking."""
    server_url = os.environ.get("AGUI_SERVER_URL", "http://127.0.0.1:8888/")
    print(f"Connecting to AG-UI server at: {server_url}\n")

    # Create AG-UI chat client
    chat_client = AGUIChatClient(endpoint=server_url)

    # Wrap with Agent for convenient API
    agent = Agent(
        name="ClientAgent",
        client=chat_client,
        instructions="You are a helpful assistant.",
    )

    # Get a thread for conversation continuity
    thread = agent.create_session()

    # Track state locally
    state: dict[str, Any] = {}

    try:
        while True:
            message = input("\nUser (:q to quit, :state to show state): ")
            if not message.strip():
                continue

            if message.lower() in (":q", "quit"):
                break

            if message.lower() == ":state":
                print(f"\nCurrent state: {json.dumps(state, indent=2)}")
                continue

            print()
            # Stream the agent response with state
            async for update in agent.run(message, session=thread, stream=True):
                # Handle text content
                if update.text:
                    print(update.text, end="", flush=True)

                # Handle state updates surfaced through AG-UI events.
                for content in update.contents:
                    if content.type == "data" and getattr(content, "media_type", None) == "application/json":
                        print("\n[JSON state payload received]")

            print(f"\n\nCurrent state: {json.dumps(state, indent=2)}")
            print()

    except KeyboardInterrupt:
        print("\n\nExiting...")


if __name__ == "__main__":
    # Install dependencies: pip install agent-framework-ag-ui --pre
    asyncio.run(main())
```

### Key Benefits

The `AGUIChatClient` provides:

- **Simplified Connection**: Automatic handling of HTTP/SSE communication
- **Thread Management**: Built-in thread ID tracking for conversation continuity
- **Agent Integration**: Works seamlessly with `Agent` for familiar API
- **State Handling**: Automatic parsing of state events from the server
- **Parity with .NET**: Consistent experience across languages

> [!TIP]
> Use `AGUIChatClient` with `Agent` to get the full benefit of the agent framework's features like conversation history, tool execution, and middleware support.

## Confirming predicted state

Set `require_confirmation=True` on `AgentFrameworkAgent` when predicted state changes should wait for client confirmation before being applied:

```python
recipe_agent = AgentFrameworkAgent(
    agent=agent,
    state_schema={"recipe": {"type": "object", "description": "The current recipe"}},
    predict_state_config={"recipe": {"tool": "update_recipe", "tool_argument": "recipe"}},
    require_confirmation=True,
)
```

Customize confirmation copy in your AG-UI client UI when rendering the confirmation event.

## Example Interaction

With the server and client running:

```
User (:q to quit, :state to show state): I want to make a classic Italian pasta carbonara

[Run Started]
[Calling Tool: update_recipe]
[State Updated]
[State Updated]
[State Updated]
[Tool Result: Recipe updated.]
Here's your recipe!
[Run Finished]

============================================================
CURRENT STATE
============================================================

recipe:
  title: Classic Pasta Carbonara
  skill_level: Intermediate
  special_preferences: ['Authentic Italian']
  cooking_time: 30 min
  ingredients:
    - 🍝 Spaghetti: 400g
    - 🥓 Guanciale or bacon: 200g
    - 🥚 Egg yolks: 4
    - 🧀 Pecorino Romano: 100g grated
    - 🧂 Black pepper: To taste
  instructions:
    1. Bring a large pot of salted water to boil
    2. Cut guanciale into small strips and fry until crispy
    3. Beat egg yolks with grated Pecorino and black pepper
    4. Cook spaghetti until al dente
    5. Reserve 1 cup pasta water, then drain pasta
    6. Remove pan from heat, add hot pasta to guanciale
    7. Quickly stir in egg mixture, adding pasta water to create creamy sauce
    8. Serve immediately with extra Pecorino and black pepper

============================================================
```

> [!TIP]
> Use the `:state` command to view the current state at any time during the conversation.

## Predictive State Updates in Action

When using predictive state updates with `predict_state_config`, the client receives `STATE_DELTA` events as the LLM generates tool arguments in real-time, before the tool executes:

```json
// Agent starts generating tool call for update_recipe
// Client receives STATE_DELTA events as the recipe argument streams:

// First delta - partial recipe with title
{
  "type": "STATE_DELTA",
  "delta": [{"op": "replace", "path": "/recipe", "value": {"title": "Classic Pasta"}}]
}

// Second delta - title complete with more fields
{
  "type": "STATE_DELTA",
  "delta": [{"op": "replace", "path": "/recipe", "value": {
    "title": "Classic Pasta Carbonara",
    "skill_level": "Intermediate"
  }}]
}

// Third delta - ingredients starting to appear
{
  "type": "STATE_DELTA",
  "delta": [{"op": "replace", "path": "/recipe", "value": {
    "title": "Classic Pasta Carbonara",
    "skill_level": "Intermediate",
    "cooking_time": "30 min",
    "ingredients": [
      {"icon": "🍝", "name": "Spaghetti", "amount": "400g"}
    ]
  }}]
}

// ... more deltas as the LLM generates the complete recipe
```

This enables the client to show optimistic UI updates in real-time as the agent is thinking, providing immediate feedback to users.

## State with Human-in-the-Loop

You can combine state management with approval workflows by setting `require_confirmation=True`:

```python
recipe_agent = AgentFrameworkAgent(
    agent=agent,
    state_schema={"recipe": {"type": "object", "description": "The current recipe"}},
    predict_state_config={"recipe": {"tool": "update_recipe", "tool_argument": "recipe"}},
    require_confirmation=True,  # Require approval for state changes
)
```

When enabled:

1. State updates stream as the agent generates tool arguments (predictive updates via `STATE_DELTA` events)
2. Agent pauses before executing the tool with a `tool_call` interrupt in `RUN_FINISHED.outcome.interrupts`
3. If approved, the tool executes and final state is emitted (via `STATE_SNAPSHOT` event)
4. If rejected, the predictive state changes are discarded

## Advanced State Patterns

### Complex State with Multiple Fields

You can manage multiple state fields with different tools:

```python
from pydantic import BaseModel


class TaskStep(BaseModel):
    """A single task step."""
    description: str
    status: str = "pending"
    estimated_duration: str = "5 min"


@tool
def generate_task_steps(steps: list[TaskStep]) -> str:
    """Generate task steps for a given task."""
    return f"Generated {len(steps)} steps."


@tool
def update_preferences(preferences: dict[str, Any]) -> str:
    """Update user preferences."""
    return "Preferences updated."


# Configure with multiple state fields
agent_with_multiple_state = AgentFrameworkAgent(
    agent=agent,
    state_schema={
        "steps": {"type": "array", "description": "List of task steps"},
        "preferences": {"type": "object", "description": "User preferences"},
    },
    predict_state_config={
        "steps": {"tool": "generate_task_steps", "tool_argument": "steps"},
        "preferences": {"tool": "update_preferences", "tool_argument": "preferences"},
    },
)
```

### Using Wildcard Tool Arguments

When a tool returns complex nested data, use `"*"` to map all tool arguments to state:

```python
@tool
def create_document(title: str, content: str, metadata: dict[str, Any]) -> str:
    """Create a document with title, content, and metadata."""
    return "Document created."


# Map all tool arguments to document state
predict_state_config = {
    "document": {"tool": "create_document", "tool_argument": "*"}
}
```

This maps the entire tool call (all arguments) to the `document` state field.

## Best Practices

### Use Pydantic Models

Define structured models for type safety:

```python
class Recipe(BaseModel):
    """Use Pydantic models for structured, validated state."""
    title: str
    skill_level: SkillLevel
    ingredients: list[Ingredient]
    instructions: list[str]
```

Benefits:
- **Type Safety**: Automatic validation of data types
- **Documentation**: Field descriptions serve as documentation
- **IDE Support**: Auto-completion and type checking
- **Serialization**: Automatic JSON conversion

### Complete State Updates

Always write the complete state, not just deltas:

```python
@tool
def update_recipe(recipe: Recipe) -> str:
    """
    You MUST write the complete recipe with ALL fields.
    When modifying a recipe, include ALL existing ingredients and
    instructions plus your changes. NEVER delete existing data.
    """
    return "Recipe updated."
```

This ensures state consistency and proper predictive updates.

### Match Parameter Names

Ensure tool parameter names match `tool_argument` configuration:

```python
# Tool parameter name
def update_recipe(recipe: Recipe) -> str:  # Parameter name: 'recipe'
    ...

# Must match in predict_state_config
predict_state_config = {
    "recipe": {"tool": "update_recipe", "tool_argument": "recipe"}  # Same name
}
```

### Provide Context in Instructions

Include clear instructions about state management:

```python
agent = Agent(
    instructions="""
    CRITICAL RULES:
    1. You will receive the current recipe state in the system context
    2. To update the recipe, you MUST use the update_recipe tool
    3. When modifying a recipe, ALWAYS include ALL existing data plus your changes
    4. NEVER delete existing ingredients or instructions - only add or modify
    """,
    ...
)
```

### Customize confirmation UI

Customize approval and state-confirmation messages in your AG-UI client when rendering confirmation events from the server.

## Next Steps

You've now learned all the core AG-UI features! Next you can:

- Explore the [Agent Framework documentation](../../../../overview/index.md)
- Build a complete application combining all AG-UI features
- Deploy your AG-UI service to production

## Additional Resources

- [AG-UI Overview](index.md)
- [Getting Started](getting-started.md)
- [Backend Tool Rendering](backend-tool-rendering.md)
<!-- - [Human-in-the-Loop](human-in-the-loop.md) -->

::: zone-end

::: zone pivot="programming-language-go"

Go AG-UI state management can be implemented with middleware that emits structured `message.DataContent` updates alongside normal text updates.

```go
stateSnapshotMiddleware := agent.MiddlewareFunc(func(next agent.RunFunc, ctx context.Context, messages []*message.Message, opts ...agent.Option) iter.Seq2[*agent.ResponseUpdate, error] {
    return func(yield func(*agent.ResponseUpdate, error) bool) {
        for update, err := range next(ctx, messages, opts...) {
            if err != nil {
                yield(nil, err)
                return
            }
            if update != nil {
                // Inspect update contents and yield DataContent snapshots as needed.
            }
            if !yield(update, nil) {
                return
            }
        }
    }
})

a := foundryprovider.NewAgent(endpoint, token, foundryprovider.ModelDeployment(model), foundryprovider.AgentConfig{
    Config: agent.Config{
        Middlewares: []agent.Middleware{stateSnapshotMiddleware},
    },
})
```

> [!TIP]
> See the [AG-UI state management sample](https://github.com/microsoft/agent-framework-go/blob/main/examples/02-agents/agui/step05_state_management/server/main.go) for a complete runnable example.

::: zone-end
