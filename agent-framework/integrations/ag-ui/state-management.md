---
title: State Management with AG-UI
description: Learn how to synchronize state between client and server using AG-UI protocol
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 11/07/2025
ms.service: agent-framework
---

# State Management with AG-UI

This tutorial shows you how to implement state management with AG-UI, enabling bidirectional synchronization of state between the client and server. This is essential for building interactive applications like generative UI, real-time dashboards, or collaborative experiences.

## Prerequisites

Before you begin, ensure you understand:

- [Getting Started with AG-UI](getting-started.md)
- [Backend Tool Rendering](backend-tool-rendering.md)
- [Human-in-the-Loop](human-in-the-loop.md)

## What is State Management?

State management in AG-UI enables:

- **Shared State**: Both client and server maintain a synchronized view of application state
- **Bidirectional Sync**: State can be updated from either client or server
- **Real-time Updates**: Changes are streamed immediately using state events
- **Predictive Updates**: State updates stream as the LLM generates tool arguments (optimistic UI)
- **Structured Data**: State follows a JSON schema for validation

### Use Cases

State management is valuable for:

- **Generative UI**: Build UI components based on agent-controlled state
- **Form Building**: Agent populates form fields as it gathers information
- **Progress Tracking**: Show real-time progress of multi-step operations
- **Interactive Dashboards**: Display data that updates as the agent processes it
- **Collaborative Editing**: Multiple users see consistent state updates

::: zone pivot="programming-language-csharp"

## Creating State-Aware Agents in C#

### Define Your State Model

First, define classes for your state structure:

```csharp
using System.Text.Json.Serialization;

namespace RecipeAssistant;

// State response wrapper
internal sealed class RecipeResponse
{
    [JsonPropertyName("recipe")]
    public RecipeState Recipe { get; set; } = new();
}

// Recipe state model
internal sealed class RecipeState
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("cuisine")]
    public string Cuisine { get; set; } = string.Empty;

    [JsonPropertyName("ingredients")]
    public List<string> Ingredients { get; set; } = [];

    [JsonPropertyName("steps")]
    public List<string> Steps { get; set; } = [];

    [JsonPropertyName("prep_time_minutes")]
    public int PrepTimeMinutes { get; set; }

    [JsonPropertyName("cook_time_minutes")]
    public int CookTimeMinutes { get; set; }

    [JsonPropertyName("skill_level")]
    public string SkillLevel { get; set; } = string.Empty;
}

// JSON serialization context
[JsonSerializable(typeof(RecipeResponse))]
[JsonSerializable(typeof(RecipeState))]
internal sealed partial class RecipeSerializerContext : JsonSerializerContext;
```

### Configure Agent with JSON Schema

Use middleware to enhance JSON response format with schema definitions:

```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI;
using ChatResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat;

AIAgent CreateRecipeAgent()
{
    string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
        ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
    string deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME")
        ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME is not set.");

    AzureOpenAIClient azureClient = new AzureOpenAIClient(
        new Uri(endpoint),
        new DefaultAzureCredential());

    var chatClient = azureClient.GetChatClient(deploymentName);

    // Create base agent
    AIAgent baseAgent = chatClient.AsIChatClient().CreateAIAgent(
        name: "RecipeAgent",
        instructions: """
            You are a helpful recipe assistant. When users ask you to create or suggest a recipe,
            respond with a complete RecipeResponse JSON object that includes:
            - recipe.title: The recipe name
            - recipe.cuisine: Type of cuisine (e.g., Italian, Mexican, Japanese)
            - recipe.ingredients: Array of ingredient strings with quantities
            - recipe.steps: Array of cooking instruction strings
            - recipe.prep_time_minutes: Preparation time in minutes
            - recipe.cook_time_minutes: Cooking time in minutes
            - recipe.skill_level: One of "beginner", "intermediate", or "advanced"

            Always include all fields in the response. Be creative and helpful.
            """);

    // Wrap with middleware to configure JSON schema output for state management
    return baseAgent
        .AsBuilder()
        .Use(runFunc: null, runStreamingFunc: ConfigureStateMiddleware)
        .Build();
}

// Middleware that configures JSON schema response format for state
static IAsyncEnumerable<AgentRunResponseUpdate> ConfigureStateMiddleware(
    IEnumerable<Microsoft.Extensions.AI.ChatMessage> messages,
    AgentThread? thread,
    AgentRunOptions? options,
    AIAgent innerAgent,
    CancellationToken cancellationToken)
{
    // Configure structured JSON output with schema when JSON response format is requested
    if (options is ChatClientAgentRunOptions chatRunOptions &&
        chatRunOptions.ChatOptions?.ResponseFormat == ChatResponseFormat.Json)
    {
        // Set the JSON schema response format to ensure structured state output
        chatRunOptions.ChatOptions.ResponseFormat = ChatResponseFormat.ForJsonSchema<RecipeResponse>(
            schemaName: "RecipeResponse",
            schemaDescription: "A structured recipe with title, skill level, cooking time, ingredients, and instructions");
    }

    return innerAgent.RunStreamingAsync(messages, thread, options, cancellationToken);
}
```

> [!NOTE]
> The C# implementation uses a type alias to resolve ambiguity between `Microsoft.Extensions.AI.ChatResponseFormat` and `OpenAI.Chat.ChatResponseFormat`:
> ```csharp
> using ChatResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat;
> ```

### Map the Agent Endpoint

```csharp
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient().AddLogging();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.TypeInfoResolverChain.Add(RecipeSerializerContext.Default));
builder.Services.AddAGUI();

WebApplication app = builder.Build();

AIAgent recipeAgent = CreateRecipeAgent();
app.MapAGUI("/", recipeAgent);

await app.RunAsync();
```

### Key Concepts

- **Structured State Models**: Define C# classes for your state structure with JSON property names
- **JSON Schema Response Format**: Use `ChatResponseFormat.ForJsonSchema<T>()` to ensure structured output
- **Middleware Pattern**: Enhance JSON response format with schema when configured
- **STATE_SNAPSHOT Events**: Framework automatically emits state snapshots when structured output is returned
- **Response Format Configuration**: State management is controlled by setting `ChatOptions.ResponseFormat`

### How It Works

1. Client sends request with `ChatOptions.ResponseFormat = ChatResponseFormat.Json` configured
2. Middleware detects JSON response format and enhances it with the schema
3. Agent processes the request and returns structured JSON matching your state model
4. Framework automatically sends STATE_SNAPSHOT event with the returned JSON
5. Client receives and displays/updates state

> [!TIP]
> The middleware pattern allows you to conditionally apply schemas based on response format configuration rather than analyzing message content. This matches the Python implementation's approach where state management is declaratively configured through response format settings.

::: zone-end

::: zone pivot="programming-language-python"

## State Schema

Define a state schema to specify the structure and types of your state:

```python
state_schema = {
    "recipe": {
        "type": "object",
        "properties": {
            "name": {"type": "string"},
            "cuisine": {"type": "string"},
            "ingredients": {
                "type": "array",
                "items": {"type": "string"}
            },
            "steps": {
                "type": "array",
                "items": {"type": "string"}
            },
            "prep_time": {"type": "number"},
            "cook_time": {"type": "number"}
        }
    },
    "user_preferences": {
        "type": "object",
        "properties": {
            "dietary_restrictions": {
                "type": "array",
                "items": {"type": "string"}
            },
            "skill_level": {"type": "string"}
        }
    }
}
```

## Predictive State Updates

Predictive state updates stream tool arguments to the state as the LLM generates them, enabling optimistic UI updates:

```python
predict_state_config = {
    "recipe": {
        "tool": "create_recipe",
        "tool_argument": "recipe_data"
    },
    "user_preferences": {
        "tool": "update_preferences",
        "tool_argument": "preferences"
    }
}
```

This configuration maps state fields to tool arguments. When the agent calls a tool, the arguments stream to the corresponding state field in real-time.

## Creating a Server with State Management

Here's a complete server implementation with state management:

```python
"""AG-UI server with state management."""

import os
from typing import Annotated, Any

from agent_framework import ChatAgent, ai_function
from agent_framework.azure import AzureOpenAIChatClient
from agent_framework_ag_ui import AgentFrameworkAgent, add_agent_framework_fastapi_endpoint
from fastapi import FastAPI
from pydantic import Field


# Define tools that update state
@ai_function
def create_recipe(
    recipe_data: Annotated[
        dict[str, Any],
        Field(description="Recipe data including name, cuisine, ingredients, and steps")
    ],
) -> str:
    """Create a new recipe with the provided details."""
    name = recipe_data.get("name", "Untitled Recipe")
    return f"Recipe '{name}' created successfully!"


@ai_function
def update_preferences(
    preferences: Annotated[
        dict[str, Any],
        Field(description="User preferences including dietary restrictions and skill level")
    ],
) -> str:
    """Update user preferences."""
    restrictions = preferences.get("dietary_restrictions", [])
    return f"Preferences updated. Dietary restrictions: {', '.join(restrictions) if restrictions else 'none'}"


@ai_function
def search_recipes(
    cuisine: Annotated[str, Field(description="Type of cuisine to search for")],
    dietary_restrictions: Annotated[list[str], Field(description="Dietary restrictions to consider")] = None,
) -> list[dict[str, Any]]:
    """Search for recipes matching criteria."""
    # Simulated recipe search
    return [
        {
            "name": "Pasta Carbonara",
            "cuisine": "Italian",
            "prep_time": 10,
            "cook_time": 20
        },
        {
            "name": "Margherita Pizza",
            "cuisine": "Italian",
            "prep_time": 15,
            "cook_time": 12
        }
    ]


# Read configuration
endpoint = os.environ.get("AZURE_OPENAI_ENDPOINT")
deployment_name = os.environ.get("AZURE_OPENAI_CHAT_DEPLOYMENT_NAME")
# Note: a token may also be used in place of the api_key
api_key = os.environ.get("AZURE_OPENAI_API_KEY")

if not endpoint or not deployment_name or not api_key:
    raise ValueError("AZURE_OPENAI_ENDPOINT and AZURE_OPENAI_CHAT_DEPLOYMENT_NAME and AZURE_OPENAI_API_KEY are required")

# Create agent
agent = ChatAgent(
    name="RecipeAssistant",
    instructions="""You are a helpful recipe assistant. Help users find and create recipes.
    When creating recipes, always include name, cuisine, ingredients (as array), steps (as array), 
    prep_time (in minutes), and cook_time (in minutes).""",
    chat_client=AzureOpenAIChatClient(
        endpoint=endpoint,
        deployment_name=deployment_name,
        api_key=api_key,
    ),
    tools=[create_recipe, update_preferences, search_recipes],
)

# Define state schema
state_schema = {
    "recipe": {
        "type": "object",
        "properties": {
            "name": {"type": "string"},
            "cuisine": {"type": "string"},
            "ingredients": {"type": "array", "items": {"type": "string"}},
            "steps": {"type": "array", "items": {"type": "string"}},
            "prep_time": {"type": "number"},
            "cook_time": {"type": "number"}
        }
    },
    "user_preferences": {
        "type": "object",
        "properties": {
            "dietary_restrictions": {"type": "array", "items": {"type": "string"}},
            "skill_level": {"type": "string"}
        }
    }
}

# Configure predictive state updates
predict_state_config = {
    "recipe": {"tool": "create_recipe", "tool_argument": "recipe_data"},
    "user_preferences": {"tool": "update_preferences", "tool_argument": "preferences"}
}

# Wrap agent with state management
wrapped_agent = AgentFrameworkAgent(
    agent=agent,
    state_schema=state_schema,
    predict_state_config=predict_state_config,
    require_confirmation=False,  # Set to True to require approval for state changes
)

# Create FastAPI app
app = FastAPI(title="AG-UI Recipe Assistant")
add_agent_framework_fastapi_endpoint(app, wrapped_agent, "/")

if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="127.0.0.1", port=8888)
```

### Key Concepts

- **State Schema**: Defines the structure and types of state fields
- **Predictive State Config**: Maps state fields to tool arguments for streaming updates
- **State Injection**: State is automatically injected as system messages to provide context to the agent
- **Bidirectional Sync**: Client and server both maintain and update state

## Understanding State Events

### State Snapshot Event

A complete snapshot of the current state:

```python
{
    "type": "STATE_SNAPSHOT",
    "snapshot": {
        "recipe": {
            "name": "Pasta Carbonara",
            "cuisine": "Italian",
            "ingredients": ["pasta", "eggs", "bacon", "parmesan"],
            "steps": ["Boil pasta", "Cook bacon", "Mix eggs and cheese", "Combine"],
            "prep_time": 10,
            "cook_time": 20
        },
        "user_preferences": {
            "dietary_restrictions": ["gluten-free"],
            "skill_level": "beginner"
        }
    }
}
```

### State Delta Event

Incremental state updates using JSON Patch format:

```python
{
    "type": "STATE_DELTA",
    "delta": [
        {
            "op": "replace",
            "path": "/recipe/name",
            "value": "Spaghetti Carbonara"
        },
        {
            "op": "add",
            "path": "/recipe/ingredients/-",
            "value": "black pepper"
        }
    ]
}
```

## Client with State Management

Here's a client that handles state events:

```python
"""AG-UI client with state management."""

import asyncio
import json
import os
from typing import Any

import httpx
import jsonpatch


class AGUIClient:
    """AG-UI client with state management."""

    def __init__(self, server_url: str):
        self.server_url = server_url
        self.thread_id: str | None = None
        self.state: dict[str, Any] = {}

    def apply_state_snapshot(self, snapshot: dict[str, Any]) -> None:
        """Apply a complete state snapshot."""
        self.state = snapshot.copy()

    def apply_state_delta(self, delta: list[dict[str, Any]]) -> None:
        """Apply incremental state changes using JSON Patch."""
        patch = jsonpatch.JsonPatch(delta)
        self.state = patch.apply(self.state)

    async def send_message(self, message: str):
        """Send a message and handle state events."""
        request_data = {
            "messages": [
                {"role": "system", "content": "You are a helpful assistant."},
                {"role": "user", "content": message},
            ]
        }

        if self.thread_id:
            request_data["thread_id"] = self.thread_id

        # Include current state in request
        if self.state:
            request_data["state"] = self.state

        async with httpx.AsyncClient(timeout=60.0) as client:
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

                            # Handle state events
                            if event.get("type") == "STATE_SNAPSHOT":
                                self.apply_state_snapshot(event.get("snapshot", {}))

                            elif event.get("type") == "STATE_DELTA":
                                self.apply_state_delta(event.get("delta", []))

                            # Capture thread_id
                            if event.get("type") == "RUN_STARTED" and not self.thread_id:
                                self.thread_id = event.get("threadId")

                        except json.JSONDecodeError:
                            continue


def display_state(state: dict[str, Any]) -> None:
    """Display the current state."""
    if not state:
        return
    
    print("\n\033[94m" + "=" * 60 + "\033[0m")
    print("\033[94mCURRENT STATE\033[0m")
    print("\033[94m" + "=" * 60 + "\033[0m")
    
    for key, value in state.items():
        print(f"\n{key}:")
        if isinstance(value, dict):
            for k, v in value.items():
                print(f"  {k}: {v}")
        else:
            print(f"  {value}")
    
    print("\n\033[94m" + "=" * 60 + "\033[0m")


async def main():
    """Main client loop with state management."""
    server_url = os.environ.get("AGUI_SERVER_URL", "http://127.0.0.1:8888/")
    print(f"Connecting to AG-UI server at: {server_url}\n")

    client = AGUIClient(server_url)

    try:
        while True:
            message = input("\nUser (:q to quit, :state to show state): ")
            if not message.strip():
                continue

            if message.lower() in (":q", "quit"):
                break

            if message.lower() == ":state":
                display_state(client.state)
                continue

            print()
            async for event in client.send_message(message):
                event_type = event.get("type", "")

                if event_type == "RUN_STARTED":
                    print("\033[93m[Run Started]\033[0m")

                elif event_type == "TEXT_MESSAGE_CONTENT":
                    print(f"\033[96m{event.get('delta', '')}\033[0m", end="", flush=True)

                elif event_type == "STATE_SNAPSHOT":
                    print(f"\n\033[94m[State Snapshot Received]\033[0m")

                elif event_type == "STATE_DELTA":
                    print(f"\n\033[94m[State Updated]\033[0m")

                elif event_type == "TOOL_CALL_START":
                    tool_name = event.get("toolCallName", "unknown")
                    print(f"\n\033[95m[Calling Tool: {tool_name}]\033[0m")

                elif event_type == "TOOL_CALL_RESULT":
                    content = event.get("content", "")
                    print(f"\033[94m[Tool Result: {content}]\033[0m")

                elif event_type == "RUN_FINISHED":
                    print(f"\n\033[92m[Run Finished]\033[0m")
                    # Display final state
                    display_state(client.state)

                elif event_type == "RUN_ERROR":
                    error_msg = event.get("message", "Unknown error")
                    print(f"\n\033[91m[Error: {error_msg}]\033[0m")

            print()

    except KeyboardInterrupt:
        print("\n\nExiting...")
    except Exception as e:
        print(f"\n\033[91mError: {e}\033[0m")


if __name__ == "__main__":
    # Install jsonpatch: pip install jsonpatch
    asyncio.run(main())
```

## Example Interaction

With the server and client running:

```
User (:q to quit, :state to show state): I want to make an Italian recipe with pasta, eggs, bacon, and cheese

[Run Started]
[Calling Tool: create_recipe]
[State Updated]
[Tool Result: Recipe 'Pasta Carbonara' created successfully!]
I've created a Pasta Carbonara recipe for you! It's an Italian classic that takes 10 minutes 
to prep and 20 minutes to cook.
[Run Finished]

============================================================
CURRENT STATE
============================================================

recipe:
  name: Pasta Carbonara
  cuisine: Italian
  ingredients: ['pasta', 'eggs', 'bacon', 'parmesan', 'black pepper']
  steps: ['Boil pasta', 'Cook bacon', 'Mix eggs and cheese', 'Combine all ingredients']
  prep_time: 10
  cook_time: 20

============================================================

User (:q to quit, :state to show state): :state

============================================================
CURRENT STATE
============================================================

recipe:
  name: Pasta Carbonara
  cuisine: Italian
  ingredients: ['pasta', 'eggs', 'bacon', 'parmesan', 'black pepper']
  steps: ['Boil pasta', 'Cook bacon', 'Mix eggs and cheese', 'Combine all ingredients']
  prep_time: 10
  cook_time: 20

============================================================
```

## Predictive State Updates in Action

When using predictive state updates, the client receives `STATE_DELTA` events as the LLM generates tool arguments, before the tool executes:

```python
# Agent starts generating tool call for create_recipe
# Client receives STATE_DELTA events as arguments stream:

# First delta - recipe name appears
STATE_DELTA: [{"op": "replace", "path": "/recipe/name", "value": "Pasta"}]

# Second delta - full name
STATE_DELTA: [{"op": "replace", "path": "/recipe/name", "value": "Pasta Carbonara"}]

# Third delta - cuisine added
STATE_DELTA: [{"op": "replace", "path": "/recipe/cuisine", "value": "Italian"}]

# Fourth delta - first ingredient
STATE_DELTA: [{"op": "add", "path": "/recipe/ingredients", "value": ["pasta"]}]

# ... and so on as the LLM generates the full tool call
```

This enables the client to show optimistic UI updates in real-time as the agent is thinking.

## State with Human-in-the-Loop

You can combine state management with approval workflows:

```python
wrapped_agent = AgentFrameworkAgent(
    agent=agent,
    state_schema=state_schema,
    predict_state_config=predict_state_config,
    require_confirmation=True,  # Require approval for state changes
)
```

When enabled:

1. State updates stream as the agent generates tool arguments (predictive updates)
2. Agent requests approval before executing the tool
3. If approved, the tool executes and state is finalized
4. If rejected, the state changes are rolled back

## Advanced State Patterns

### Partial State Updates

Update only specific fields:

```python
@ai_function
def update_recipe_name(
    new_name: Annotated[str, Field(description="New recipe name")],
) -> str:
    """Update just the recipe name."""
    return f"Recipe name updated to '{new_name}'"

# Configure to update only the name field
predict_state_config = {
    "recipe.name": {"tool": "update_recipe_name", "tool_argument": "new_name"}
}
```

### Complex State Structures

Support nested objects and arrays:

```python
state_schema = {
    "project": {
        "type": "object",
        "properties": {
            "metadata": {
                "type": "object",
                "properties": {
                    "title": {"type": "string"},
                    "created_at": {"type": "string"},
                    "tags": {"type": "array", "items": {"type": "string"}}
                }
            },
            "tasks": {
                "type": "array",
                "items": {
                    "type": "object",
                    "properties": {
                        "id": {"type": "string"},
                        "title": {"type": "string"},
                        "status": {"type": "string"},
                        "assignee": {"type": "string"}
                    }
                }
            }
        }
    }
}
```

### Client-Initiated State Updates

Clients can send state updates to the server:

```python
# Include updated state in request
request_data = {
    "messages": [...],
    "state": {
        "user_preferences": {
            "dietary_restrictions": ["vegetarian"],
            "skill_level": "advanced"
        }
    }
}
```

## Best Practices

### Schema Design

- Keep state schemas focused and minimal
- Use descriptive property names
- Provide appropriate type constraints
- Consider validation requirements

### Predictive Updates

- Map state fields to corresponding tool arguments accurately
- Handle partial updates gracefully in the client UI
- Provide visual indicators for optimistic updates

### State Persistence

Consider persisting state for session continuity:

```python
# Server-side: Store state per thread_id
thread_states = {}

def get_thread_state(thread_id: str) -> dict:
    return thread_states.get(thread_id, {})

def update_thread_state(thread_id: str, state: dict):
    thread_states[thread_id] = state
```

### Error Handling

Handle state synchronization errors gracefully:

```python
try:
    client.apply_state_delta(delta)
except jsonpatch.JsonPatchException as e:
    print(f"Failed to apply state update: {e}")
    # Request full state snapshot
```

## Next Steps

You've now learned all the core AG-UI features! Next you can:

- Explore the [Agent Framework documentation](../../overview/agent-framework-overview.md)
- Build a complete application combining all AG-UI features
- Deploy your AG-UI service to production

## Additional Resources

- [AG-UI Overview](index.md)
- [Getting Started](getting-started.md)
- [Backend Tool Rendering](backend-tool-rendering.md)
- [Human-in-the-Loop](human-in-the-loop.md)

::: zone-end
