---
name: sample-structure
description: >
  Conceptual organization of Agent Framework documentation and samples.
  For language-specific details (file naming, provider setup, code patterns),
  see each code repo's skills files.
---

# Skill: Agent Framework Conceptual Structure

## Purpose

This skill defines the **conceptual organization** of Agent Framework documentation
and how it maps to the learning journey. Language-specific implementation details
belong in each code repo's skills files — this file covers the shared conceptual
model only.

## Progressive Learning Model

The Agent Framework uses a 5-layer structure that progressively builds complexity.
Both docs and code samples mirror this structure so users can move fluidly between
reading and running code.

```
Layer 1: Get Started       → Linear tutorial, one concept per step
Layer 2: Agent Concepts    → Deep-dive reference, organized by topic
Layer 3: Workflows         → Multi-step orchestration patterns
Layer 4: Hosting           → Deployment, protocols, production infrastructure
Layer 5: End-to-End        → Complete applications combining all layers
```

### Layer Characteristics

| Layer | Purpose | Complexity | Audience |
|-------|---------|------------|----------|
| Get Started | "Follow these steps" — sequential, cumulative | Beginner | New users |
| Agent Concepts | "Go deeper on X" — one topic per page | Intermediate | Users building features |
| Workflows | "Orchestrate multiple steps" — pattern-based | Intermediate | Users composing agents |
| Hosting | "Deploy to production" — infrastructure-focused | Advanced | Users shipping |
| End-to-End | "See it all together" — reference applications | Advanced | Users architecting |

### Get Started Progression

Each step adds exactly one concept to the previous step:

1. **Hello Agent** — Create an agent, invoke it, stream the response
2. **Add Tools** — Give the agent a function tool it can call
3. **Multi-Turn** — Maintain conversation state with threads
4. **Memory** — Inject persistent context via context providers
5. **First Workflow** — Compose a multi-step workflow
6. **Host Your Agent** — Expose the agent via A2A protocol

### Agent Concepts — Topic Areas

Concepts are grouped by architectural concern:

- **Tools** — Function tools, hosted tools, MCP, approval workflows, code interpreter, file search, web search
- **Middleware** — Request interception at agent, chat, and function layers; termination, guardrails, shared state
- **Conversations** — Threads, persistent storage, suspend/resume, chat history
- **Providers** — Client setup for each supported model provider
- **Context Providers** — Memory injection, RAG patterns
- **Orchestrations** — Multi-agent patterns (handoff, sequential, concurrent)
- **Observability** — Tracing, OpenTelemetry, Foundry tracing
- **Declarative** — YAML/JSON-defined agents and workflows
- **Multimodal** — Image, audio, and file inputs

### Architecture: Tools & Middleware Layers

The Agent Framework uses a layered middleware architecture with three interception points:

1. **Agent Middleware** — Wraps the entire `Agent.run()` call; can modify messages, options, thread
2. **Chat Middleware** — Wraps calls to the underlying chat client; can modify messages, options
3. **Function Middleware** — Wraps individual tool invocations; can modify arguments, override results

Each layer has its own context object and supports `call_next()` for pipeline chaining.
Middleware can return normally (upstream post-processing runs), raise `MiddlewareTermination`
(skips all post-processing), or raise exceptions (propagated to caller).

## Docs ↔ Samples Alignment

Every docs page under `get-started/` maps 1:1 to a sample file in both Python and .NET.
Deep-dive docs pages link to corresponding concept samples.

| Docs Section | Code Repo Layer | Cross-link pattern |
|-------------|----------------|-------------------|
| `get-started/*.md` | `01-get-started/` | Each step → next step + lateral deep-dive |
| `agents/**/*.md` | `02-agents/` | Each topic → matching concept sample(s) |
| `workflows/*.md` | `03-workflows/` | Each pattern → matching workflow sample |
| `integrations/*.md` | `04-hosting/` | Each integration → matching hosting sample |

## Navigation Pattern

Docs pages follow consistent navigation:

- **Sequential**: Every page has a "Next step" link for linear flow
- **Lateral**: Every page has "Go deeper" links to related topics
- **Upward**: Every deep-dive page links back to its section overview

## When Adding New Content

1. Determine which layer (1–5) the content belongs to
2. For Get Started: only add a step if it introduces a truly foundational concept
3. For Agent Concepts: group under the appropriate topic area
4. For Workflows: coordinate with the workflow team
5. Ensure both Python and .NET samples exist before publishing a docs page
6. Update the corresponding code repo's skills file with language-specific details
