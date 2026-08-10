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
├── concepts/                  # Fundamental mental models, semantics, and architecture
│   ├── index.md               # Concepts landing
│   ├── agents/
│   │   ├── index.md           # Agents landing
│   │   ├── conversations/
│   │   └── middleware/
│   ├── workflows/
│   │   ├── index.md           # Workflows landing
│   │   └── advanced/
│   └── harness.md             # Agent Harness composition and architecture
├── get-started/               # 7-step progressive tutorial
│   ├── index.md               # Tutorial landing page
│   ├── your-first-agent.md    # Step 1
│   ├── add-tools.md           # Step 2
│   ├── multi-turn.md          # Step 3
│   ├── memory.md              # Step 4
│   ├── workflows.md           # Step 5
│   ├── harness.md             # Step 6
│   └── hosting.md             # Step 7
├── agents/                    # Generic and built-in agent capability guides
│   ├── index.md               # Agent capabilities landing
│   ├── structured-outputs.md
│   ├── declarative.md
│   ├── observability.md
│   ├── rag.md
│   ├── multimodal.md
│   ├── background-responses.md
│   ├── background-agents.md
│   ├── looping.md
│   ├── planning-and-todos.md
│   ├── security.md
│   ├── tools/                 # 1 page per tool type
│   │   ├── index.md           # Tools overview & landing
│   │   └── ...
├── workflows/                 # Generic and built-in workflow capability guides
│   ├── index.md               # Workflow capabilities landing
│   ├── agents-in-workflows.md
│   ├── human-in-the-loop.md
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
├── integrations/              # Named external things; usually outside services
│   ├── index.md               # Integrations overview & landing
│   ├── by-provider/           # Cross-component provider ecosystem landing pages
│   │   ├── index.md
│   │   ├── microsoft-foundry.md
│   │   ├── microsoft-azure.md
│   │   └── ...
│   └── by-component/          # Canonical implementation guidance by framework surface
│       ├── index.md
│       ├── model-providers/   # Inference providers
│       │   ├── index.md
│       │   └── ...
│       ├── agent-services/    # Managed or protocol-backed remote agent runtimes
│       │   ├── index.md
│       │   ├── a2a.md
│       │   └── ...
│       ├── tools/             # Provider-managed and optional tool integrations
│       │   ├── index.md
│       │   ├── foundry-toolbox.md
│       │   └── shell-tools.md
│       ├── context-providers/ # External before-run/after-run providers
│       │   ├── index.md
│       │   └── ...            # One flat page per external provider
│       ├── middleware/        # External middleware integrations
│       │   └── ...
│       ├── evaluation/        # External evaluation services
│       │   └── ...
│       └── ui/                # Shared UI integrations
│           ├── ag-ui/
│           │   ├── index.md
│           │   └── ...
│           ├── chatkit.md     # Flat page (no subfolder)
│           └── devui/
│               ├── index.md
│               └── ...
├── hosting/                   # Hosting model selection and guides
│   ├── index.md               # Managed vs self-hosted overview
│   ├── azure-functions.md     # Azure Functions and Durable Extension
│   ├── foundry-hosted-agent.md
│   └── self-hosting/
│       ├── index.md           # Shared self-hosting state and protocol choices
│       ├── responses.md
│       ├── openai-endpoints.md
│       ├── telegram.md
│       ├── a2a/
│       │   ├── index.md
│       │   ├── server.md      # Multi-language A2A server guide
│       │   └── dotnet.md
│       └── mcp.md
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
   builds complexity step by step. Concepts explain foundational mental models
   and architecture; Agent Capabilities and Workflow Capabilities document
   generic or built-in opt-in framework features; integrations document named
   external things that normally require a service outside Agent Framework; and
   hosting documents deployment models.

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

## New content triage

**Before choosing a directory or writing a page, classify the primary lesson as
a fundamental concept, a capability, or an integration.** Package names, sample
folders, and implementation details do not determine placement.

| Classification | Primary test | Location | Examples |
|---|---|---|---|
| **Fundamental concept** | Does every reader need this mental model to understand how agents or workflows work, regardless of optional features or providers? Concepts explain core abstractions, semantics, runtime behavior, lifecycle, state, and architecture. | `concepts/agents/`, `concepts/workflows/`, or a standalone concept such as `concepts/harness.md` | Agent execution and pipeline, sessions, middleware scope, workflow APIs, executors, edges, events, and state |
| **Capability** | Is this a generic or built-in feature that extends what an agent or workflow can do? The guidance is provider-independent even when an integration can implement or enhance it. | `agents/` or `workflows/` | Tools, RAG, structured outputs, observability, evaluation, security, background agents, checkpoints, and human-in-the-loop |
| **Integration** | Is the lesson about configuring or using a named external service, provider, protocol, runtime, library, or tool? In almost all cases, an integration requires an outside service, endpoint, deployment, daemon, or separately operated system. | Canonical guidance under `integrations/by-component/`; provider navigation under `integrations/by-provider/` | OpenAI, Microsoft Foundry, Azure AI Search, Redis, Purview, A2A, AG-UI, and ChatKit |

Being built into Agent Framework does **not** automatically make something a
concept. A built-in but optional behavior is normally a capability. Likewise,
a provider-specific sample does **not** automatically make a generic framework
feature an integration.

When a topic has both generic framework behavior and provider-specific setup,
split the guidance:

1. Explain the provider-independent mental model under `concepts/`, or the
   generic or built-in feature under Agent Capabilities or Workflow
   Capabilities.
2. Add an integration page only when the named external system materially
   changes authentication, configuration, API shape, hosted features, runtime
   semantics, or operational behavior.
3. Cross-link the generic and provider-specific pages instead of duplicating
   the generic explanation.

This triage applies to conceptual and feature guidance. Use `hosting/` when the
primary lesson is deployment or protocol exposure, `get-started/` for the
progressive tutorial, and the dedicated migration and support areas for those
reader intents.

## Content placement rules

Classify content by the reader's learning intent before considering where the
implementation package lives.

| Rule | Decision | Examples |
|------|----------|----------|
| **R1: Learning intent first** | Put a page where readers look for the thing being taught, not where an incidental API or package lives. | Function tools → `agents/tools/`; Foundry evaluation service → `integrations/by-component/evaluation/microsoft-foundry.md` |
| **R2: Integrations name an external thing** | Use `integrations/` only when the primary lesson is a named external library, tool, protocol, runtime, or service. Most integrations require a service outside Agent Framework; do not put a generic or built-in capability here merely because a provider-specific sample exists. | Redis, Mem0, Azure AI Search, AG-UI, ChatKit, DevUI, Foundry, OpenAI |
| **R3: Component then external thing** | Canonical implementation pages use `integrations/by-component/<component>/<external-thing>`. Context providers use one flat page per external provider for all supported patterns. | `integrations/by-component/model-providers/openai.md`, `integrations/by-component/context-providers/redis.md` |
| **R4: Split concepts from capabilities** | Fundamental runtime, type, conversation, middleware, safety, workflow API, and execution-model guidance lives under `concepts/`. Generic or built-in opt-in features stay under Agent Capabilities or Workflow Capabilities. Built-in does not mean fundamental. Security remains an Agent Capability. Agent Harness is a standalone concept that links to its composed capabilities. | `concepts/agents/agent-pipeline.md`, `agents/security.md`, `concepts/harness.md` |
| **R5: Inference providers use `model-providers`** | Public inference-provider pages live under `integrations/by-component/model-providers/`, never a generic `providers/` or `chat-clients/` bucket. | OpenAI, Azure OpenAI, Anthropic, Ollama |
| **R6: Do not clone generic features** | Keep one generic framework page unless an external provider materially changes behavior, authentication, hosted tools, API shape, or runtime semantics. | Function tools stay generic; provider-hosted file search can be provider-specific |
| **R7: Distinguish memory from storage** | Long-term memory and exact conversation persistence remain distinct patterns even when one provider page documents both. Explain the difference before setup guidance. | `integrations/by-component/context-providers/redis.md`, `integrations/by-component/context-providers/azure-cosmos.md` |
| **R8: Use RAG for external retrieval** | Describe search-index grounding as the RAG pattern. External RAG providers live in the flat context-provider catalog; local file access remains a framework concept. | Azure AI Search → `integrations/by-component/context-providers/azure-ai-search.md`; local files → agent context management |
| **R9: Evaluation follows the external-service rule** | Generic or built-in agent/workflow evaluation remains a capability; managed evaluation services use integrations. | `agents/evaluation.md`, `integrations/by-component/evaluation/microsoft-foundry.md` |
| **R10: DevUI is a shared UI integration** | DevUI lives under `integrations/by-component/ui/devui/`, not under agents, workflows, or harness. | `integrations/by-component/ui/devui/index.md` |
| **R11: User-managed hosting uses `self-hosting`** | Local and user-managed hosting guides use `hosting/self-hosting/`. | `hosting/self-hosting/responses.md` |
| **R12: Apps are assembled applications** | Reserve a future `apps/` area for complete applications. External integrations remain in their component area even when their samples are end-to-end. | GitHub Copilot → `integrations/by-component/agent-services/`; Purview → `integrations/by-component/middleware/` |
| **R13: Move in phases** | Lock taxonomy and mapping, audit inbound links, move concept pages, move integrations, then remove old paths only after redirects and links are ready. | Preserve Learn, blog, and Foundry links during migration |

For context-provider integrations, use one flat page per external provider.
When a provider supports multiple patterns, add a short comparison before
separate sections for storage, memory, RAG, pre-processing, CodeAct, or other
behaviors. Keep the filename, page title, H1, and TOC label provider-focused.

Provider ecosystem pages under `integrations/by-provider/` aggregate links
across components for a named platform. Keep implementation guidance in the
canonical `by-component` pages and use provider pages only for navigation and
scenario selection. Provider pages can be added for ecosystems that readers
commonly select first, even when current coverage is limited to one component.

Model-provider pages should consistently document installation, verified
environment variables, explicit client and agent construction, supported tools,
provider-specific features, and runnable samples where they exist. In Python,
construct chat-client-backed agents with `Agent(client=client, ...)`, not
`client.as_agent(...)`. Direct agent types such as `FoundryAgent`,
`ClaudeAgent`, and workflow `.as_agent()` APIs are different patterns and
shouldn't be rewritten.

Protocol-backed remote agents belong under
`integrations/by-component/agent-services/`. Keep protocol exposure and server
setup under `hosting/`; for A2A, consumption lives at
`integrations/by-component/agent-services/a2a.md` and exposure lives under
`hosting/self-hosting/a2a/`.

### Move-only restructuring

When a PR only restructures documentation:

- Move existing pages without rewriting article prose so Microsoft Learn can
  preserve Platform IDs through content similarity.
- Update only affected doc-to-doc links, Next-step links, `TOC.yml`, `index.yml`,
  DocFX path metadata, and `.openpublishing.redirection.json`.
- Add a redirect for every moved page with `redirect_document_id: true`.
- Retarget older redirects directly to the final destination; do not introduce
  redirect chains. When multiple old paths target the same destination, keep
  `redirect_document_id: true` only on the directly moved page and set it to
  `false` on older legacy redirects.
- Do not update `:::code` paths or sample repository links until the matching
  sample restructuring is available.

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
- Top-level items: Agent Framework, Get Started, Concepts, Agent Capabilities,
  Workflow Capabilities, Integrations, Hosting, The Agent Development Journey,
  Migration Guide, API Reference, Support
- Each section uses `items:` for child pages
- `expanded: true` on Get Started and Concepts

## Index file convention

Use `index.md` (not `overview.md`) when a folder has a landing page. DocFX uses
`index.md` for URL routing — `/agents/` resolves to `/agents/index.md`.

Component folders that only group named leaf integrations may omit an index
temporarily; their TOC parent must be an expander without an `href`. Multi-page
external integrations such as `by-component/ui/ag-ui/`,
`by-component/ui/devui/`, and `by-component/agent-services/` use an `index.md`
landing page.

## Page → sample file mapping

Every docs page maps to sample files in both repos:

| Docs page | Python sample | .NET sample |
|-----------|--------------|-------------|
| `get-started/your-first-agent.md` | `01-get-started/01_hello_agent.py` | `01-get-started/01_hello_agent/Program.cs` |
| `get-started/add-tools.md` | `01-get-started/02_add_tools.py` | `01-get-started/02_add_tools/Program.cs` |
| `get-started/multi-turn.md` | `01-get-started/03_multi_turn.py` | `01-get-started/03_multi_turn/Program.cs` |
| `get-started/memory.md` | `01-get-started/04_memory.py` | `01-get-started/04_memory/Program.cs` |
| `get-started/workflows.md` | `01-get-started/07_first_graph_workflow.py` | `01-get-started/05_first_workflow/Program.cs` |
| `get-started/harness.md` | `02-agents/harness/` | `02-agents/Harness/` |
| `get-started/hosting.md` | `04-hosting/azure_functions/01_single_agent/function_app.py` | `01-get-started/06_host_your_agent/Program.cs` |
| `agents/tools/function-tools.md` | `02-agents/tools/function_tool_with_explicit_schema.py`, `02-agents/tools/function_tool_with_kwargs.py`, `02-agents/tools/tool_in_class.py` | N/A (no dedicated .NET sample; see `dotnet/samples` generally) |
| `agents/tools/web-search.md` | `02-agents/providers/openai/client_with_web_search.py` | `02-agents/AgentProviders/foundry/Agent_Step21_WebSearch/` |
| `agents/tools/file-search.md` | `02-agents/providers/openai/client_with_file_search.py` | `02-agents/AgentProviders/foundry/Agent_Step16_FileSearch/` |
| `agents/tools/code-interpreter.md` | `02-agents/providers/openai/client_with_code_interpreter.py` | `02-agents/AgentProviders/foundry/Agent_Step14_CodeInterpreter/` |
| `agents/tools/hosted-mcp-tools.md` | `02-agents/providers/openai/client_with_hosted_mcp.py`, `02-agents/providers/foundry/foundry_chat_client_with_hosted_mcp.py` | `02-agents/ModelContextProtocol/FoundryAgent_Hosted_MCP/` |
| `agents/tools/local-mcp-tools.md` | `02-agents/providers/openai/client_with_local_mcp.py`, `02-agents/providers/foundry/foundry_chat_client_with_local_mcp.py` | `02-agents/ModelContextProtocol/Agent_MCP_Server/` |
| `agents/tools/tool-approval.md` | `02-agents/tools/function_tool_with_approval.py`, `02-agents/tools/function_tool_with_approval_and_sessions.py` | `02-agents/Agents/Agent_Step01_UsingFunctionToolsWithApprovals/` |
| `agents/code_act.md` | `02-agents/context_providers/code_act/code_act.py` | `02-agents/AgentWithCodeAct/` |
| `concepts/harness.md` | `02-agents/harness/` | `02-agents/Harness/` |
| `agents/looping.md` | `02-agents/harness/` | `02-agents/Harness/` |
| `agents/background-agents.md` | `02-agents/harness/` | `02-agents/Harness/` |
| `agents/planning-and-todos.md` | `02-agents/harness/` | `02-agents/Harness/` |
| `concepts/agents/middleware/*.md` | `02-agents/middleware/` | `02-agents/Agents/Agent_Step11_Middleware/` |
| `concepts/agents/custom-agents.md` | `02-agents/providers/custom/custom_agent.py` | `02-agents/AgentProviders/custom/` |
| `concepts/agents/conversations/{session,storage}.md` | `02-agents/conversations/` | `02-agents/Agents/Agent_Step03_PersistedConversations/` |
| `concepts/agents/conversations/context-providers.md` | `02-agents/conversations/`, `02-agents/context_providers/file_memory_provider.py` | `02-agents/Agents/Agent_Step03_PersistedConversations/` |
| `concepts/agents/conversations/compaction.md` | `02-agents/compaction/` | `02-agents/Agents/Agent_Step18_CompactionPipeline/` |
| `concepts/agents/conversations/chat-history-memory-provider.md` | N/A | `02-agents/AgentWithMemory/AgentWithMemory_Step01_ChatHistoryMemory/` |
| `concepts/workflows/functional.md` | `03-workflows/functional/` | N/A (functional workflow API is Python-only) |
| `concepts/workflows/edges.md` | `03-workflows/control-flow/edge_condition.py`, `03-workflows/control-flow/switch_case_edge_group.py`, `03-workflows/control-flow/multi_selection_edge_group.py` | `03-workflows/ConditionalEdges/01_EdgeCondition/`, `03-workflows/ConditionalEdges/02_SwitchCase/`, `03-workflows/ConditionalEdges/03_MultiSelection/` |
| `concepts/workflows/advanced/agent-executor.md` | `03-workflows/orchestrations/sequential_chain_only_agent_responses.py` | N/A |
| `concepts/workflows/advanced/resettable-executors.md` | N/A | `03-workflows/Agents/WorkflowAsAnAgent/` |
| `concepts/workflows/{index,builder-and-execution,events,executors,state}.md`, `concepts/workflows/advanced/{execution-modes,sub-workflows}.md` | N/A (conceptual pages; no dedicated 1:1 sample) | N/A (conceptual pages; no dedicated 1:1 sample) |
| `workflows/<capability>.md` | `03-workflows/<matching>.py` | `03-workflows/<matching>.cs` |
| `integrations/by-component/model-providers/foundry-local.md` | `02-agents/providers/foundry/foundry_local_agent.py` | N/A |
| `integrations/by-component/model-providers/microsoft-foundry.md` | `02-agents/providers/foundry/` | `02-agents/AgentProviders/foundry/` |
| `integrations/by-component/model-providers/azure-openai.md` | `02-agents/providers/azure/` | `02-agents/AgentProviders/azure/` |
| `integrations/by-component/model-providers/{openai,anthropic,ollama}.md` | `02-agents/providers/<matching>/` | `02-agents/AgentProviders/<matching>/` |
| `integrations/by-component/model-providers/amazon-bedrock.md` | `02-agents/providers/amazon/bedrock_chat_client.py` | `02-agents/AgentWithMemory/AgentWithMemory_Step03_MemoryUsingValkey_Bedrock/` |
| `integrations/by-component/model-providers/google-gemini.md` | `02-agents/providers/gemini/` | `02-agents/AgentProviders/google-gemini/` |
| `integrations/by-component/model-providers/onnx.md` | N/A | `02-agents/AgentProviders/onnx/` |
| `integrations/by-component/model-providers/dapr.md` | N/A | `02-agents/AgentProviders/dapr/` |
| `integrations/by-component/model-providers/mistral.md` | `02-agents/providers/mistral/mistral_embeddings.py` | N/A |
| `integrations/by-component/agent-services/github-copilot.md` | `02-agents/providers/github_copilot/` | `02-agents/AgentProviders/github-copilot/` |
| `integrations/by-component/agent-services/copilot-studio.md` | `02-agents/providers/copilotstudio/` | N/A |
| `integrations/by-component/agent-services/foundry.md` | `02-agents/providers/foundry/` | `02-agents/AgentProviders/foundry/` |
| `integrations/by-component/agent-services/anthropic-claude.md` | `02-agents/providers/anthropic/anthropic_claude_*.py` | N/A |
| `integrations/by-component/middleware/purview.md` | `05-end-to-end/purview_agent/` | `05-end-to-end/AgentWithPurview/` |
| `integrations/by-component/agent-services/a2a.md` | `02-agents/a2a/` | `02-agents/A2A/` |
| `integrations/by-component/tools/foundry-toolbox.md` | `04-hosting/foundry-hosted-agents/responses/foundry_toolbox/main.py`, `04-hosting/foundry-hosted-agents/responses/foundry_toolbox_mcp_skills/main.py`, `02-agents/providers/foundry/foundry_chat_client_with_toolbox.py`, `02-agents/providers/foundry/foundry_chat_client_with_toolbox_skills.py`, `03-workflows/declarative/invoke_foundry_toolbox_mcp/` | `04-hosting/FoundryHostedAgents/responses/Hosted-Toolbox/`, `04-hosting/FoundryHostedAgents/responses/Hosted-ToolboxMcpSkills/`, `02-agents/AgentProviders/foundry/Agent_Step25_FoundryToolboxMcp/`, `02-agents/AgentProviders/foundry/Agent_Step26_FoundryToolboxMcpSkills/`, `03-workflows/Declarative/InvokeFoundryToolboxMcp/` |
| `integrations/by-component/tools/shell-tools.md` | `02-agents/providers/openai/client_with_local_shell.py`, `02-agents/tools/local_shell_with_allowlist.py`, `02-agents/tools/local_shell_with_environment_provider.py` | `02-agents/Agents/Agent_Step21_ShellWithEnvironment/` |
| `integrations/by-component/context-providers/azure-ai-search.md` | `02-agents/context_providers/azure_ai_search/` | `04-hosting/FoundryHostedAgents/responses/Hosted-AzureSearchRag/` |
| `integrations/by-component/context-providers/azure-content-understanding.md` | `02-agents/context_providers/azure_content_understanding/` | N/A |
| `integrations/by-component/context-providers/azure-cosmos.md` | `packages/azure-cosmos-memory/samples/`, `02-agents/conversations/cosmos_history_provider.py` | N/A (provider source only) |
| `integrations/by-component/context-providers/hyperlight.md` | `02-agents/context_providers/code_act/code_act.py` | `02-agents/AgentWithCodeAct/` |
| `integrations/by-component/context-providers/local.md` | N/A | `04-hosting/FoundryHostedAgents/responses/Hosted-LocalCodeAct/` |
| `integrations/by-component/context-providers/mem0.md` | `02-agents/context_providers/mem0/` | N/A (sample exists, but no supported .NET package is published) |
| `integrations/by-component/context-providers/microsoft-foundry.md` | `02-agents/providers/foundry/foundry_chat_client_with_file_search.py`, `02-agents/context_providers/azure_ai_foundry_memory.py` | `02-agents/AgentWithRAG/AgentWithRAG_Step04_FoundryServiceRAG/`, `02-agents/AgentWithMemory/AgentWithMemory_Step04_MemoryUsingFoundry/` |
| `integrations/by-component/context-providers/monty.md` | `02-agents/context_providers/code_act/monty_code_act.py` | N/A |
| `integrations/by-component/context-providers/neo4j.md` | `05-end-to-end/neo4j_graphrag/`, `02-agents/context_providers/neo4j_memory/` | `02-agents/AgentWithRAG/AgentWithRAG_Step05_Neo4jGraphRAG/`, `02-agents/AgentWithMemory/AgentWithMemory_Step06_MemoryUsingAgentMemory/` |
| `integrations/by-component/context-providers/redis.md` | `02-agents/context_providers/redis/`, `02-agents/conversations/redis_history_provider.py` | N/A |
| `integrations/by-component/context-providers/valkey.md` | N/A | `02-agents/AgentWithMemory/AgentWithMemory_Step03_MemoryUsingValkey/` |
| `integrations/by-component/ui/ag-ui/*.md` | `05-end-to-end/ag_ui_workflow_handoff/`, `packages/ag-ui/agent_framework_ag_ui_examples/` | `02-agents/AGUI/`, `05-end-to-end/AGUIClientServer/` |
| `integrations/by-component/ui/chatkit.md` | `05-end-to-end/chatkit-integration/` | N/A |
| `integrations/by-component/ui/devui/*.md` | `02-agents/devui/` | `02-agents/DevUI/`, `05-end-to-end/DevUIAspireIntegration/` |
| `integrations/by-component/evaluation/microsoft-foundry.md` | `05-end-to-end/evaluation/foundry_evals/` | `05-end-to-end/Evaluation/` |
| `hosting/azure-functions.md` | `04-hosting/azure_functions/`, `04-hosting/durabletask/` | `04-hosting/DurableAgents/`, `04-hosting/DurableWorkflows/` |
| `hosting/self-hosting/index.md` | `04-hosting/af-hosting/` | N/A |
| `hosting/self-hosting/responses.md` | `04-hosting/af-hosting/local_responses/`, `04-hosting/af-hosting/local_responses_workflow/` | N/A |
| `hosting/self-hosting/openai-endpoints.md` | `04-hosting/af-hosting/` | `04-hosting/af-hosting/` |
| `hosting/self-hosting/telegram.md` | `04-hosting/af-hosting/local_telegram/` | N/A |
| `hosting/self-hosting/a2a/index.md` | `04-hosting/a2a/` | `05-end-to-end/A2AClientServer/` |
| `hosting/self-hosting/a2a/server.md` | `04-hosting/a2a/` | `05-end-to-end/A2AClientServer/` |
| `hosting/self-hosting/a2a/dotnet.md` | N/A | `05-end-to-end/A2AClientServer/` |
| `hosting/self-hosting/mcp.md` | `04-hosting/mcp/` | N/A |

## When adding a new docs page

1. Triage the primary lesson as a fundamental concept, a generic or built-in
   capability, or an integration by using the decision table above.
2. If the topic mixes generic behavior with a named external system, keep the
   generic page canonical and add a separate integration page only when the
   external system materially changes setup or behavior.
3. Create the `.md` file with proper frontmatter (see template above).
4. Add zone pivots for C#, Python, and Go when the feature is supported in those SDKs.
5. Use `:::code` directives — never paste code inline.
6. Add the page to the root `TOC.yml` in the appropriate section.
7. Add a `## Next steps` section at the bottom with a `> [!div class="nextstepaction"]` link.
8. Add an `index.md` only when the folder needs a landing page; otherwise make
   the TOC parent an expander without an `href`.
9. Update the sample repos' `AGENTS.md` mapping tables if new sample files are involved.

## When a docs page is renamed or moved

You must update:

1. The root `TOC.yml`.
2. All internal doc-to-doc and Next-step links that point to the old path.
3. `.openpublishing.redirection.json`, using the old repository path as
   `source_path`, the final Learn URL as `redirect_url`, and
   `redirect_document_id: true`.
4. Any `index.yml` hub links and `docfx.json` path metadata affected by the move.
5. Existing redirect entries that target the old URL, so they point directly to
   the final URL.

For a move-only restructuring PR, do not change article prose, sample URLs, or
`:::code` source paths.

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
