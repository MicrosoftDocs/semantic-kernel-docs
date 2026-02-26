---
title: Agent Skills
description: Learn how to extend agent capabilities with Agent Skills — portable packages of instructions, scripts, and resources that agents discover and load on demand.
zone_pivot_groups: programming-languages
author: SergeyMenshykh
ms.topic: conceptual
ms.author: semenshi
ms.date: 02/25/2026
ms.service: agent-framework
---

# Agent Skills

[Agent Skills](https://agentskills.io/) are portable packages of instructions, scripts, and resources that give agents specialized capabilities and domain expertise. Skills follow an open specification and implement a progressive disclosure pattern so agents load only the context they need, when they need it.

Use Agent Skills when you want to:

- **Package domain expertise** — Capture specialized knowledge (expense policies, legal workflows, data analysis pipelines) as reusable, portable packages.
- **Extend agent capabilities** — Give agents new abilities without changing their core instructions.
- **Ensure consistency** — Turn multi-step tasks into repeatable, auditable workflows.
- **Enable interoperability** — Reuse the same skill across different Agent Skills-compatible products.

## Skill structure

A skill is a directory containing a `SKILL.md` file with optional subdirectories for resources:

```
expense-report/
├── SKILL.md                          # Required — frontmatter + instructions
├── scripts/
│   └── validate.py                   # Executable code agents can run
├── references/
│   └── POLICY_FAQ.md                 # Reference documents loaded on demand
└── assets/
    └── expense-report-template.md    # Templates and static resources
```

### SKILL.md format

The `SKILL.md` file must contain YAML frontmatter followed by markdown content:

```yaml
---
name: expense-report
description: File and validate employee expense reports according to company policy. Use when asked about expense submissions, reimbursement rules, or spending limits.
license: Apache-2.0
compatibility: Requires python3
metadata:
  author: contoso-finance
  version: "2.1"
---
```

| Field | Required | Description |
|---|---|---|
| `name` | Yes | Max 64 characters. Lowercase letters, numbers, and hyphens only. Must not start or end with a hyphen or contain consecutive hyphens. Must match the parent directory name. |
| `description` | Yes | What the skill does and when to use it. Max 1024 characters. Should include keywords that help agents identify relevant tasks. |
| `license` | No | License name or reference to a bundled license file. |
| `compatibility` | No | Max 500 characters. Indicates environment requirements (intended product, system packages, network access, etc.). |
| `metadata` | No | Arbitrary key-value mapping for additional metadata. |
| `allowed-tools` | No | Space-delimited list of pre-approved tools the skill may use. Experimental — support may vary between agent implementations. |

The markdown body after the frontmatter contains the skill instructions — step-by-step guidance, examples of inputs and outputs, common edge cases, or any content that helps the agent perform the task. Keep `SKILL.md` under 500 lines and move detailed reference material to separate files.

## Progressive disclosure

Agent Skills use a three-stage progressive disclosure pattern to minimize context usage:

1. **Advertise** (~100 tokens per skill) — Skill names and descriptions are injected into the system prompt at the start of each run, so the agent knows what skills are available.
2. **Load** (< 5000 tokens recommended) — When a task matches a skill's domain, the agent calls the `load_skill` tool to retrieve the full SKILL.md body with detailed instructions.
3. **Read resources** (as needed) — The agent calls the `read_skill_resource` tool to fetch supplementary files (references, templates, assets) only when required.

This pattern keeps the agent's context window lean while giving it access to deep domain knowledge on demand.

## Using FileAgentSkillsProvider

The `FileAgentSkillsProvider` discovers skills from filesystem directories and makes them available to agents as a context provider. It searches configured paths recursively (up to two levels deep) for `SKILL.md` files, validates their format and resources, and exposes two tools to the agent: `load_skill` and `read_skill_resource`.

> [!NOTE]
> Script execution is not yet supported by `FileAgentSkillsProvider` and will be added in a future release.

:::zone pivot="programming-language-csharp"

### Basic setup

Create a `FileAgentSkillsProvider` pointing to a directory containing your skills, and add it to the agent's context providers:

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;

// Discover skills from the 'skills' directory
var skillsProvider = new FileAgentSkillsProvider(
    skillPath: Path.Combine(AppContext.BaseDirectory, "skills"));

// Create an agent with the skills provider
AIAgent agent = new AzureOpenAIClient(
    new Uri(endpoint), new DefaultAzureCredential())
    .GetResponsesClient(deploymentName)
    .AsAIAgent(new ChatClientAgentOptions
    {
        Name = "SkillsAgent",
        ChatOptions = new()
        {
            Instructions = "You are a helpful assistant.",
        },
        AIContextProviders = [skillsProvider],
    });
```

### Invoking the agent

Once configured, the agent automatically discovers available skills and uses them when a task matches:

```csharp
// The agent loads the expense-report skill and reads the FAQ resource
AgentResponse response = await agent.RunAsync(
    "Are tips reimbursable? I left a 25% tip on a taxi ride.");
Console.WriteLine(response.Text);
```

:::zone-end

:::zone pivot="programming-language-python"

### Basic setup

Create a `FileAgentSkillsProvider` pointing to a directory containing your skills, and add it to the agent's context providers:

```python
from pathlib import Path
from agent_framework import FileAgentSkillsProvider
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity.aio import AzureCliCredential

# Discover skills from the 'skills' directory
skills_provider = FileAgentSkillsProvider(
    skill_paths=Path(__file__).parent / "skills"
)

# Create an agent with the skills provider
agent = AzureOpenAIChatClient(credential=AzureCliCredential()).as_agent(
    name="SkillsAgent",
    instructions="You are a helpful assistant.",
    context_providers=[skills_provider],
)
```

### Invoking the agent

Once configured, the agent automatically discovers available skills and uses them when a task matches:

```python
# The agent loads the expense-report skill and reads the FAQ resource
response = await agent.run(
    "Are tips reimbursable? I left a 25% tip on a taxi ride."
)
print(response.text)
```

:::zone-end

## Multiple skill directories

You can search multiple directories by passing a list of paths:

:::zone pivot="programming-language-csharp"

```csharp
var skillsProvider = new FileAgentSkillsProvider(
    skillPaths: [
        Path.Combine(AppContext.BaseDirectory, "company-skills"),
        Path.Combine(AppContext.BaseDirectory, "team-skills"),
    ]);
```

:::zone-end

:::zone pivot="programming-language-python"

```python
skills_provider = FileAgentSkillsProvider(
    skill_paths=[
        Path(__file__).parent / "company-skills",
        Path(__file__).parent / "team-skills",
    ]
)
```

:::zone-end

Each path can point to an individual skill folder (containing a `SKILL.md`) or a parent folder with skill subdirectories. The provider searches up to two levels deep.

## Custom system prompt

By default, `FileAgentSkillsProvider` injects a system prompt that lists available skills and instructs the agent to use `load_skill` and `read_skill_resource`. You can customize this prompt:

:::zone pivot="programming-language-csharp"

```csharp
var skillsProvider = new FileAgentSkillsProvider(
    skillPath: Path.Combine(AppContext.BaseDirectory, "skills"),
    options: new FileAgentSkillsProviderOptions
    {
        SkillsInstructionPrompt = """
            You have skills available. Here they are:
            {0}
            Use the `load_skill` function to get skill instructions.
            Use the `read_skill_resource` function to read skill files.
            """
    });
```

> [!NOTE]
> The custom template must contain a `{0}` placeholder where the skill list is inserted. Literal braces must be escaped as `{{` and `}}`.

:::zone-end

:::zone pivot="programming-language-python"

```python
skills_provider = FileAgentSkillsProvider(
    skill_paths=Path(__file__).parent / "skills",
    skills_instruction_prompt=(
        "You have skills available. Here they are:\n{0}\n"
        "Use the `load_skill` function to get skill instructions.\n"
        "Use the `read_skill_resource` function to read skill files."
    ),
)
```

> [!NOTE]
> The custom template must contain a `{0}` placeholder where the skill list is inserted.

:::zone-end

## Security considerations

`FileAgentSkillsProvider` reads only static content from the filesystem and includes the following security measures:

- **XML-escaping** — Skill metadata (names and descriptions) is XML-escaped before being injected into the system prompt, preventing prompt injection through skill frontmatter.
- **Path traversal protection** — Resource reads validate that the resolved file path remains within the skill directory, blocking `../` escape attempts.
- **Symlink guards** — Each path segment is checked for symlinks that could resolve outside the skill directory.
- **Validation at discovery** — All referenced resources are validated when skills are loaded. Skills with missing or invalid resources are excluded and logged.

> [!WARNING]
> Only use skills from trusted sources. Skill instructions are injected into the agent's context and can influence agent behavior.

## Next steps

> [!div class="nextstepaction"]
> [Context Providers](./conversations/context-providers.md)

### Related content

- [Agent Skills specification](https://agentskills.io/)
- [Context Providers](./conversations/context-providers.md)
- [Tools Overview](./tools/index.md)
