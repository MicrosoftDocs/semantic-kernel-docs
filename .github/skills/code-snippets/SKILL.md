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
:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/01_hello_agent.py" id="create_agent" highlight="1-4":::
```

### Parameters

| Parameter | Required | Description |
|-----------|----------|-------------|
| `language` | Yes | `"python"` or `"csharp"` |
| `source` | Yes | Snippet file path using docset-relative syntax (for example `~/...` or `~/../<dependent-repo>/...`) |
| `id` | No | Matches a snippet tag in source (`# <name>` / `# </name>` for Python, `// <name>` / `// </name>` for C#) |
| `range` | No | Line range (e.g. `"2-24,26"`). **Cannot coexist with `id`** |
| `highlight` | No | Lines to highlight, **relative to the displayed snippet** (not the file) |

### Source Path Conventions

- In-repo snippets: `~/agent-framework/<path-to-file>`
- Out-of-repo snippets (dependent repositories): `~/../<path_to_root>/<path-to-file>`
- Agent Framework code samples in this repo: `~/../agent-framework-code/python/samples/<section>/<file>.py`

## Out-of-repo snippet references

If the code file you want to reference is in a different repository, set up that code repository as a dependent repository in `.openpublishing.publish.config.json`. The `path_to_root` you assign acts like a folder name for snippet source paths.

### Dependent repositories metadata

The `dependent_repositories` list is required in `.openpublishing.publish.config.json` for cross-repository references (CRR):

```json
{
  "dependent_repositories": [
    {
      "path_to_root": "<relative path to repository root>",
      "url": "<referenced repository url>",
      "branch": "<branch name of referenced repository>",
      "branch_mapping": {
        "<source repository branch>": "<referenced repository branch>",
        "<source repository branch>": "<referenced repository branch>"
      }
    },
    {
      "path_to_root": "token",
      "url": "https://github.com/Microsoft/token",
      "branch": "main",
      "branch_mapping": {
        "main": "main",
        "develop": "test"
      }
    }
  ]
}
```

| Metadata | Meaning | Required |
|----------|---------|----------|
| `dependent_repositories` | The CRR relationship list name | Yes |
| `path_to_root` | Relative folder path to the repository root (folder can be virtual) | Yes |
| `url` | URL of the reference repository this repo depends on | Yes |
| `branch` | Default branch of the reference repository for builds | Yes |
| `branch_mapping` | Optional source-branch to reference-branch map | No |

### Snippet reference syntax

Use the same `:::code` syntax for CRR snippets as in-repo snippets. Only the `source` path changes.

- Start paths with `~` for docset-root-relative references.
- Use `..` segments to move up directories when needed (for example `../../`).
- Prefer readable paths; deeply nested `..` chains are harder to maintain.

Example CRR source path:

`~/../xamarin-forms-samples/WebServices/TodoREST/TodoAPI/TodoAPI/Startup.cs`

> [!NOTE]
> The dependent repository alias is rooted at repo root, but `~` is rooted at the docset's `build_source_folder`. In this repo, Agent Framework docs use `"build_source_folder": "agent-framework"`, so dependent repositories are referenced like `~/../agent-framework-code/...`.

> [!IMPORTANT]
> Updating an external code snippet does not automatically trigger a content build. Trigger a build by changing doc content or starting a build manually.

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
:::code language="csharp" source="~/../agent-framework-code/dotnet/samples/..." id="...":::
:::zone-end

:::zone pivot="programming-language-python"
:::code language="python" source="~/../agent-framework-code/python/samples/..." id="...":::
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
