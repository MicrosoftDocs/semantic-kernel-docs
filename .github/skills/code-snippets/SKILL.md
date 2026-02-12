---
name: code-snippets
description: >
  How to reference code from sample repos in Agent Framework docs pages
  using :::code directives, snippet tags, zone pivots, and highlight attributes.
---

# Skill: Code Snippets in Docs

## Purpose
This skill describes how to reference code from sample repos in Agent Framework docs pages,
eliminating inline code duplication.

## :::code Directive Syntax

```markdown
:::code language="python" source="https://github.com/microsoft/agent-framework/python/samples/01-get-started/01_hello_agent.py" id="create_agent" highlight="1-4":::
```

### Parameters

| Parameter | Required | Description |
|-----------|----------|-------------|
| `language` | Yes | `"python"` or `"csharp"` |
| `source` | Yes | Path from docset root. Always starts with `~/agent-framework/` |
| `id` | No | Matches a snippet tag in source (`# <name>` / `# </name>` for Python, `// <name>` / `// </name>` for C#) |
| `range` | No | Line range (e.g. `"2-24,26"`). **Cannot coexist with `id`** |
| `highlight` | No | Lines to highlight, **relative to the displayed snippet** (not the file) |

### Source Path Conventions

- Python: `https://github.com/microsoft/agent-framework/python/samples/<section>/<file>.py`
- .NET: `https://github.com/microsoft/agent-framework/dotnet/samples/<section>/<file>.cs`

The `~/` prefix maps to the docset root configured in `docfx.json`.

## Snippet Tags in Source Files

### Python
```python
# <create_agent>
client = OpenAIResponsesClient(...)
agent = client.as_agent(name="...", instructions="...")
# </create_agent>
```

### C#
```csharp
// <create_agent>
var agent = await client.CreateAIAgentAsync(...);
// </create_agent>
```

### Rules
- Tag names use `snake_case`
- Tags must be unique within a file
- Tags cannot overlap or nest
- Keep snippet regions small and focused (5-20 lines ideal)
- Include all necessary imports within the snippet OR document them separately

## Zone Pivots

Every page showing code for both languages uses zone pivots:

```markdown
:::zone pivot="programming-language-csharp"
:::code language="csharp" source="https://github.com/microsoft/agent-framework/dotnet/samples/..." id="...":::
:::zone-end

:::zone pivot="programming-language-python"
:::code language="python" source="https://github.com/microsoft/agent-framework/python/samples/..." id="...":::
:::zone-end
```

## When to Apply This Skill

Apply :::code references when:
1. The sample repo structure is stable (samples merged to main)
2. The sample file has proper snippet tags
3. You want to eliminate inline code duplication

Until then, use inline code blocks (```python / ```csharp) as a temporary measure.

## Common Snippet IDs

| ID | Purpose | Typical file |
|----|---------|-------------|
| `create_agent` | Agent instantiation | `01_hello_agent.py` |
| `run_agent` | Non-streaming run | `01_hello_agent.py` |
| `run_agent_streaming` | Streaming run | `01_hello_agent.py` |
| `define_tool` | Tool definition | `02_add_tools.py` |
| `create_agent_with_tools` | Agent + tools | `02_add_tools.py` |
| `multi_turn` | Thread-based conversation | `03_multi_turn.py` |
| `context_provider` | Memory/context setup | `04_memory.py` |
| `create_workflow` | Workflow builder | `05_first_workflow.py` |

## Reference
- [MS Learn code-in-docs guide](https://learn.microsoft.com/help/platform/code-in-docs)
