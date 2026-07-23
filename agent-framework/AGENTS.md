# Docs Structure & Design Choices — Agent Framework

> This file documents the structure and conventions of the Agent Framework
> documentation so that agents (AI or human) can maintain it without
> rediscovering decisions.

## Directory layout

```
agent-framework/
├── TOC.yml                    # Single flat table of contents (no nested sub-TOCs)
├── index.yml                  # Landing page (hub page)
├── zone-pivot-groups.yml      # Language pivot definitions
├── docfx.json                 # Build configuration
├── breadcrumb/agent-framework/toc.yml  # Breadcrumb navigation
├── overview/
│   ├── index.md               # "What is Agent Framework" landing
│   └── index.md
├── get-started/               # 7-step progressive tutorial
│   ├── index.md               # Tutorial landing page
│   ├── your-first-agent.md    # Step 1
│   ├── add-tools.md           # Step 2
│   ├── multi-turn.md          # Step 3
│   ├── memory.md              # Step 4
│   ├── workflows.md           # Step 5
│   ├── harness.md             # Step 6
│   └── hosting.md             # Step 7
├── agents/                    # Deep-dive reference
│   ├── index.md               # Agents overview & landing
│   ├── running-agents.md
│   ├── structured-outputs.md
│   ├── declarative.md
│   ├── observability.md
│   ├── rag.md
│   ├── multimodal.md
│   ├── background-responses.md
│   ├── tools/                 # 1 page per tool type
│   │   ├── index.md           # Tools overview & landing
│   │   └── ...
│   ├── middleware/             # 1 page per middleware concept
│   │   ├── index.md           # Middleware overview & landing
│   │   └── ...
│   ├── conversations/         # Threads, history, storage
│   │   ├── index.md           # Conversations landing
│   │   └── ...
│   └── providers/             # 1 page per provider
│       ├── index.md           # Providers overview & landing
│       └── ...
├── workflows/                 # Workflow patterns
│   ├── index.md               # Workflows overview & landing
│   ├── edges.md
│   ├── agents-in-workflows.md
│   ├── human-in-the-loop.md
│   ├── state.md
│   ├── checkpoints.md
│   ├── declarative.md
│   ├── visualization.md
│   ├── observability.md
│   ├── as-agents.md
│   └── orchestrations/       # Multi-agent orchestration patterns
│       ├── index.md           # Orchestrations landing
│       ├── sequential.md
│       ├── concurrent.md
│       ├── handoff.md
│       ├── group-chat.md
│       └── magentic.md
├── integrations/              # Hosting & deployment
│   ├── index.md               # Integrations overview & landing
│   ├── a2a.md
│   ├── ag-ui/                 # AG-UI Protocol (multi-page)
│   │   ├── index.md
│   │   └── ...
│   ├── durable-extension.md
│   ├── openai-endpoints.md
│   ├── m365.md
│   └── purview.md
├── hosting/                   # Hosting model selection and guides
│   ├── index.md               # Managed vs self-hosted overview
│   ├── foundry-hosted-agent.md
│   └── self-hosting/
│       ├── index.md           # Shared self-hosting state and protocol choices
│       ├── responses.md
│       ├── telegram.md
│       └── a2a.md
├── devui/                     # DevUI reference (top-level)
│   ├── index.md
│   └── ...
├── migration-guide/           # SK & AutoGen migration
│   ├── index.md
│   ├── from-autogen/
│   └── from-semantic-kernel/
├── api-docs/                  # API reference (external links)
└── support/                   # FAQ, troubleshooting, upgrade guides
    ├── index.md
    ├── faq.md
    ├── troubleshooting.md
    └── upgrade/
        ├── index.md
        └── ...
```

## Design principles

1. **Progressive then deep**: Get-started (01→07) is a linear tutorial that
   builds complexity step by step. Agents/workflows/integrations are reference
   docs organized by topic — users land here from "Go deeper" links.

2. **Zone pivots for languages**: Use
   `zone_pivot_groups: programming-languages` and matching
   `:::zone pivot="..."` sections only when a page presents code in multiple
   supported SDKs. You can also use a non-empty "coming soon" zone for an SDK
   that is planned but not yet supported. When you use a pivot group, include a
   non-empty zone for every pivot ID it defines; otherwise the validator reports
   blank language tabs. Do not declare a multi-language pivot group on a
   language-specific page.

3. **Code snippets as source of truth**: Prefer `:::code` directives that point
   to sample files in the code repo, so docs stay synced with runnable samples.
   Inline code blocks are temporary and should be replaced when snippet tags are
   available in the source sample.

4. **Navigation**: Each page has a "Next steps" section with:
   - A `> [!div class="nextstepaction"]` button pointing to the sequential next page
   - A "Go deeper" section with lateral links to related reference pages

## :::code directive syntax

```markdown
:::code language="python" source="~/../agent-framework-code/python/samples/01-get-started/01_hello_agent.py" id="create_agent" highlight="8-11":::
```

| Parameter | Description |
|-----------|-------------|
| `language` | `"python"`, `"csharp"`, or `"go"` |
| `source` | Snippet source path using docset-relative syntax (for example, `~/...` or `~/../<dependent-repo>/...`) |
| `id` | Matches a snippet tag in the source file (`# <name>` / `# </name>` for Python, `// <name>` / `// </name>` for C#) |
| `range` | Line range (e.g. `"2-24,26"`). **Cannot coexist with `id`** |
| `highlight` | Lines to highlight, **relative to the displayed snippet** |

### Source path conventions

- Python samples: `~/../agent-framework-code/python/samples/<section>/<file>.py`
- .NET samples: `~/../agent-framework-code/dotnet/samples/<section>/<project folder>/<file>.cs`
- Go samples: `~/../agent-framework-go/examples/<section>/<file>.go`

The dependent repository alias (`agent-framework-code`) is configured in
`.openpublishing.publish.config.json` under `dependent_repositories`.

## Zone pivot syntax

```markdown
:::zone pivot="programming-language-csharp"

C# content here

:::zone-end

:::zone pivot="programming-language-python"

Python content here

:::zone-end

:::zone pivot="programming-language-go"

Go content here

:::zone-end
```

Available pivots are defined in `zone-pivot-groups.yml`:
- `programming-language-csharp`
- `programming-language-python`
- `programming-language-go`

## Frontmatter template

Every `.md` page must have this frontmatter:

```yaml
---
title: "Page Title"
description: "One-line description for SEO"
zone_pivot_groups: programming-languages
author: eavanvalkenburg
ms.topic: article           # Use overview, tutorial, how-to, reference, or upgrade-and-migration-article when appropriate
ms.date: MM/DD/YYYY
ms.service: agent-framework
---
```

Use `article` by default. Do not use `conceptual`; it is not a supported
topic value. Use `overview` for section landing pages and choose `tutorial`,
`how-to`, `reference`, or `upgrade-and-migration-article` only when they
match the page's purpose.

## TOC.yml conventions

- **Single flat TOC**: All entries are in the root `TOC.yml` — no nested sub-TOC
  files (`href: .../TOC.yml`). This avoids breadcrumb compatibility issues and
  keeps navigation in a single source of truth.
- Top-level items: Agent Framework, Get Started, Agents, Workflows, Integrations,
  Hosting, DevUI, Migration Guide, API Reference, Support
- Each section uses `items:` for child pages
- `expanded: true` only on Get Started (first section users see)

## Index file convention

Every content folder must have an `index.md` file (not `overview.md`). DocFX
uses `index.md` for URL routing — `/agents/` resolves to `/agents/index.md`.
When creating a new folder, always create an `index.md` as the section landing
page.

## Page → sample file mapping

Every docs page maps to sample files in both repos:

| Docs page | Python sample | .NET sample |
|-----------|--------------|-------------|
| `get-started/your-first-agent.md` | `01-get-started/01_hello_agent.py` | `01-get-started/01_HelloAgent.cs` |
| `get-started/add-tools.md` | `01-get-started/02_add_tools.py` | `01-get-started/02_AddTools.cs` |
| `get-started/multi-turn.md` | `01-get-started/03_multi_turn.py` | `01-get-started/03_MultiTurn.cs` |
| `get-started/memory.md` | `01-get-started/04_memory.py` | `01-get-started/04_Memory.cs` |
| `get-started/workflows.md` | `01-get-started/07_first_graph_workflow.py` | `01-get-started/05_FirstWorkflow.cs` |
| `get-started/harness.md` | `02-agents/harness/` | `02-agents/Harness/` |
| `get-started/hosting.md` | `04-hosting/azure_functions/01_single_agent/function_app.py` | `01-get-started/06_HostYourAgent.cs` |
| `agents/tools/function-tools.md` | `02-agents/tools/function_tools.py` | `02-agents/tools/FunctionTools.cs` |
| `agents/tools/web-search.md` | `02-agents/tools/web_search.py` | `02-agents/tools/WebSearch.cs` |
| `agents/tools/file-search.md` | `02-agents/tools/file_search.py` | `02-agents/tools/FileSearch.cs` |
| `agents/tools/code-interpreter.md` | `02-agents/tools/code_interpreter.py` | `02-agents/tools/CodeInterpreter.cs` |
| `agents/tools/hosted-mcp-tools.md` | `02-agents/tools/hosted_mcp_tools.py` | `02-agents/tools/HostedMcpTools.cs` |
| `agents/tools/local-mcp-tools.md` | `02-agents/tools/local_mcp_tools.py` | `02-agents/tools/LocalMcpTools.cs` |
| `agents/tools/tool-approval.md` | `02-agents/tools/tool_approval.py` | `02-agents/tools/ToolApproval.cs` |
| `agents/code_act.md` | `02-agents/context_providers/code_act/code_act.py` | `02-agents/AgentWithCodeAct/` |
| `agents/harness.md` | `02-agents/harness/` | `02-agents/Harness/` |
| `agents/middleware/*.md` | `02-agents/middleware/<matching>.py` | `02-agents/middleware/<matching>.cs` |
| `agents/providers/foundry-local.md` | `02-agents/providers/foundry/foundry_local_agent.py` | N/A |
| `agents/providers/*.md` | `02-agents/providers/<matching>.py` | `02-agents/providers/<matching>.cs` |
| `agents/conversations/*.md` | `02-agents/conversations/<matching>.py` | `02-agents/conversations/<matching>.cs` |
| `workflows/<pattern>.md` | `03-workflows/<pattern>/<matching>.py` | `03-workflows/<pattern>/<matching>.cs` |
| `integrations/hyperlight.md` | `02-agents/context_providers/code_act/code_act.py` | `02-agents/AgentWithCodeAct/` |
| `integrations/a2a.md` | `04-hosting/a2a/` | `04-hosting/a2a/` |
| `integrations/durable-extension.md` | `04-hosting/azure_functions/`, `04-hosting/durabletask/` | `04-hosting/DurableAgents/`, `04-hosting/DurableWorkflows/` |
| `hosting/self-hosting/index.md` | `04-hosting/af-hosting/` | N/A |
| `hosting/self-hosting/responses.md` | `04-hosting/af-hosting/local_responses/`, `04-hosting/af-hosting/local_responses_workflow/` | N/A |
| `hosting/self-hosting/telegram.md` | `04-hosting/af-hosting/local_telegram/` | N/A |
| `hosting/self-hosting/a2a.md` | `04-hosting/a2a/` | N/A |
| `hosting/self-hosting/mcp.md` | `04-hosting/mcp/` | N/A |

## When adding a new docs page

1. Create the `.md` file with proper frontmatter (see template above)
2. Add zone pivots for C#, Python, and Go when the feature is supported in those SDKs
3. Use `:::code` directives — never paste code inline
4. Add the page to the root `TOC.yml` in the appropriate section
5. Add a `## Next steps` section at the bottom with a `> [!div class="nextstepaction"]` link
6. If creating a new folder, ensure it has an `index.md` landing page
7. Update the sample repos' `AGENTS.md` mapping tables if new sample files are involved

## When a sample file is renamed or moved

You must update:
1. The `:::code source=` path in the docs `.md` file that references it
2. The mapping table in the sample repo's `AGENTS.md`
3. The mapping table in this file (above)

## Language-specific pages

Some concepts exist in only one language:
- `response_stream.py`, `typed_options.py` — Python only samples (under `02-agents/`)

Use zone pivots to show language-specific content. Add a note in another
language's zone if the feature is not yet supported.
