---
title: Agent Skills
description: Learn how to extend agent capabilities with Agent Skills — portable packages of instructions, scripts, and resources that agents discover and load on demand.
zone_pivot_groups: programming-languages
author: SergeyMenshykh
ms.topic: conceptual
ms.author: semenshi
ms.date: 03/11/2026
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

## Providing skills to an agent

The Agent Framework includes a skills provider that discovers skills from filesystem directories and makes them available to agents as a context provider. It searches configured paths recursively (up to two levels deep) for `SKILL.md` files, validates their format and resources, and exposes tools to the agent: `load_skill`, `read_skill_resource`, and (when scripts are present) `run_skill_script`.

:::zone pivot="programming-language-csharp"

> [!NOTE]
> Script execution is not yet supported in C# and will be added in a future release.

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

Create a `SkillsProvider` pointing to a directory containing your skills, and add it to the agent's context providers:

```python
from pathlib import Path
from agent_framework import SkillsProvider
from agent_framework.azure import AzureOpenAIChatClient
from azure.identity.aio import AzureCliCredential

# Discover skills from the 'skills' directory
skills_provider = SkillsProvider(
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
skills_provider = SkillsProvider(
    skill_paths=[
        Path(__file__).parent / "company-skills",
        Path(__file__).parent / "team-skills",
    ]
)
```

:::zone-end

Each path can point to an individual skill folder (containing a `SKILL.md`) or a parent folder with skill subdirectories. The provider searches up to two levels deep.

## Custom system prompt

By default, the skills provider injects a system prompt that lists available skills and instructs the agent to use `load_skill` and `read_skill_resource`. You can customize this prompt:

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
skills_provider = SkillsProvider(
    skill_paths=Path(__file__).parent / "skills",
    instruction_template=(
        "You have skills available. Here they are:\n{skills}\n"
        "Use the `load_skill` function to get skill instructions.\n"
        "Use the `read_skill_resource` function to read skill files."
    ),
)
```

> [!NOTE]
> The custom template must contain a `{skills}` placeholder where the skill list is inserted and a `{runner_instructions}` placeholder where script-related instructions are inserted.

:::zone-end

:::zone pivot="programming-language-python"

## Code-defined skills

In addition to file-based skills discovered from `SKILL.md` files, you can define skills entirely in Python code. Code-defined skills are useful when:

- Skill content is generated dynamically (for example, reading from a database or environment).
- You want to keep skill definitions alongside the application code that uses them.
- You need resources that execute logic at read time rather than serving static files.

### Basic code skill

Create a `Skill` instance with a name, description, and instruction content. Optionally attach `SkillResource` instances with static content:

```python
from textwrap import dedent
from agent_framework import Skill, SkillResource, SkillsProvider

code_style_skill = Skill(
    name="code-style",
    description="Coding style guidelines and conventions for the team",
    content=dedent("""\
        Use this skill when answering questions about coding style,
        conventions, or best practices for the team.
    """),
    resources=[
        SkillResource(
            name="style-guide",
            content=dedent("""\
                # Team Coding Style Guide
                - Use 4-space indentation (no tabs)
                - Maximum line length: 120 characters
                - Use type annotations on all public functions
            """),
        ),
    ],
)

skills_provider = SkillsProvider(skills=[code_style_skill])
```

### Dynamic resources

Use the `@skill.resource` decorator to register a function as a resource. The function is called each time the agent reads the resource, so it can return up-to-date data. Both sync and async functions are supported:

```python
import os
from agent_framework import Skill

project_info_skill = Skill(
    name="project-info",
    description="Project status and configuration information",
    content="Use this skill for questions about the current project.",
)

@project_info_skill.resource
def environment() -> Any:
    """Get current environment configuration."""
    env = os.environ.get("APP_ENV", "development")
    region = os.environ.get("APP_REGION", "us-east-1")
    return f"Environment: {env}, Region: {region}"

@project_info_skill.resource(name="team-roster", description="Current team members")
def get_team_roster() -> Any:
    """Return the team roster."""
    return "Alice Chen (Tech Lead), Bob Smith (Backend Engineer)"
```

When the decorator is used without arguments (`@skill.resource`), the function name becomes the resource name and the docstring becomes the description. Use `@skill.resource(name="...", description="...")` to set them explicitly.

### Code-defined scripts

Use the `@skill.script` decorator to register a function as an executable script on a skill. Code-defined scripts run **in-process** and do not require a script executor. Both sync and async functions are supported:

```python
from agent_framework import Skill

unit_converter_skill = Skill(
    name="unit-converter",
    description="Convert between common units using a conversion factor",
    content="Use the convert script to perform unit conversions.",
)

@unit_converter_skill.script(name="convert", description="Convert a value: result = value × factor")
def convert_units(value: float, factor: float) -> str:
    """Convert a value using a multiplication factor."""
    import json
    result = round(value * factor, 4)
    return json.dumps({"value": value, "factor": factor, "result": result})
```

When the decorator is used without arguments (`@skill.script`), the function name becomes the script name and the docstring becomes the description. The function's typed parameters are automatically converted into a JSON Schema that the agent uses to pass arguments.

### Combining file-based and code-defined skills

Pass both `skill_paths` and `skills` to a single `SkillsProvider`. File-based skills are discovered first; if a code-defined skill has the same name as an existing file-based skill, the code-defined skill is skipped:

```python
from pathlib import Path
from agent_framework import Skill, SkillsProvider

my_skill = Skill(
    name="my-code-skill",
    description="A code-defined skill",
    content="Instructions for the skill.",
)

skills_provider = SkillsProvider(
    skill_paths=Path(__file__).parent / "skills",
    skills=[my_skill],
)
```

:::zone-end

:::zone pivot="programming-language-python"

## Script execution

Skills can include executable scripts that the agent runs via the `run_skill_script` tool. How a script runs depends on how it was defined:

- **Code-defined scripts** (registered via `@skill.script`) run **in-process** as direct function calls. No runner is needed.
- **File-based scripts** (`.py` files discovered in skill directories) require a `SkillScriptRunner` — any callable matching `(skill, script, args) -> Any` — that determines how the script is run (for example, as a local subprocess).

### File-based script execution

To enable execution of file-based scripts, pass a `script_runner` to `SkillsProvider`. Any sync or async callable that satisfies the `SkillScriptRunner` protocol can be used:

```python
from pathlib import Path
from agent_framework import Skill, SkillScript, SkillsProvider

def my_runner(skill: Skill, script: SkillScript, args: dict | None = None) -> str:
    """Run a file-based script as a subprocess."""
    import subprocess, sys
    cmd = [sys.executable, str(Path(skill.path) / script.path)]
    if args:
        for key, value in args.items():
            if value is not None:
                cmd.extend([f"--{key}", str(value)])
    result = subprocess.run(cmd, capture_output=True, text=True, timeout=30)
    return result.stdout.strip()

skills_provider = SkillsProvider(
    skill_paths=Path(__file__).parent / "skills",
    script_runner=my_runner,
)
```

The runner receives the resolved `Skill`, `SkillScript`, and an optional `args` dictionary. File-based scripts are automatically discovered from `.py` files in skill directories.

> [!WARNING]
> The runner above is provided for **demonstration purposes only**. For production use, consider adding:
>
> - Sandboxing (for example, containers, `seccomp`, or `firejail`)
> - Resource limits (CPU, memory, wall-clock timeout)
> - Input validation and allow-listing of executable scripts
> - Structured logging and audit trails

> [!NOTE]
> If file-based skills with scripts are provided but no `script_runner` is set, `SkillsProvider` raises a `ValueError`.

## Script approval

Use `require_script_approval=True` on `SkillsProvider` to gate all script execution behind human approval. Instead of executing immediately, the agent pauses and returns approval requests:

```python
from agent_framework import Agent, Skill, SkillsProvider

# Create provider with approval enabled
skills_provider = SkillsProvider(
    skills=[my_skill],
    require_script_approval=True,
)

# Run the agent — script calls pause for approval
result = await agent.run("Deploy version 2.5.0 to production", session=session)

# Handle approval requests
while result.user_input_requests:
    for request in result.user_input_requests:
        print(f"Script: {request.function_call.name}")
        print(f"Args: {request.function_call.arguments}")

        approval = request.to_function_approval_response(approved=True)
        result = await agent.run(approval, session=session)
```

When a script is rejected (`approved=False`), the agent is informed that the user declined and can respond accordingly.

:::zone-end

## Security best practices

Agent Skills should be treated like any third-party code you bring into your project. Because skill instructions are injected into the agent's context — and skills can include scripts — applying the same level of review and governance you would to an open-source dependency is essential.

- **Review before use** — Read all skill content (`SKILL.md`, scripts, and resources) before deploying. Verify that a script's actual behavior matches its stated intent. Check for adversarial instructions that attempt to bypass safety guidelines, exfiltrate data, or modify agent configuration files.
- **Source trust** — Only install skills from trusted authors or vetted internal contributors. Prefer skills with clear provenance, version control, and active maintenance. Watch for typosquatted skill names that mimic popular packages.
- **Sandboxing** — Run skills that include executable scripts in isolated environments. Limit filesystem, network, and system-level access to only what the skill requires. Require explicit user confirmation before executing potentially sensitive operations.
- **Audit and logging** — Record which skills are loaded, which resources are read, and which scripts are executed. This gives you an audit trail to trace agent behavior back to specific skill content if something goes wrong.

## When to use skills vs. workflows

Agent Skills and [Agent Framework Workflows](../workflows/index.md) both extend what agents can do, but they work in fundamentally different ways. Choose the approach that best matches your requirements:

- **Control** — With a skill, the AI decides how to execute the instructions. This is ideal when you want the agent to be creative or adaptive. With a workflow, you explicitly define the execution path. Use workflows when you need deterministic, predictable behavior.
- **Resilience** — A skill runs within a single agent turn. If something fails, the entire operation must be retried. Workflows support [checkpointing](../workflows/checkpoints.md), so they can resume from the last successful step after a failure. Choose workflows when the cost of re-executing the entire process is high.
- **Side effects** — Skills are suitable when operations are idempotent or low-risk. Prefer workflows when steps produce side effects (sending emails, charging payments) that should not be repeated on retry.
- **Complexity** — Skills are best for focused, single-domain tasks that one agent can handle. Workflows are better suited for multi-step business processes that coordinate multiple agents, human approvals, or external system integrations.

> [!TIP]
> As a rule of thumb: if you want the AI to figure out _how_ to accomplish a task, use a skill. If you need to guarantee _what_ steps execute and in what order, use a workflow.

## Next steps

> [!div class="nextstepaction"]
> [Agent Safety](./safety.md)

### Related content

- [Agent Skills specification](https://agentskills.io/)
- [Context Providers](./conversations/context-providers.md)
- [Tools Overview](./tools/index.md)
