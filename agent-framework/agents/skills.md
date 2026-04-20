---
title: Agent Skills
description: Learn how to extend agent capabilities with Agent Skills — portable packages of instructions, scripts, and resources that agents discover and load on demand.
zone_pivot_groups: programming-languages
author: SergeyMenshykh
ms.topic: conceptual
ms.author: semenshi
ms.date: 04/10/2026
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

Agent Skills use a four-stage progressive disclosure pattern to minimize context usage:

1. **Advertise** (~100 tokens per skill) — Skill names and descriptions are injected into the system prompt at the start of each run, so the agent knows what skills are available.
2. **Load** (< 5000 tokens recommended) — When a task matches a skill's domain, the agent calls the `load_skill` tool to retrieve the full SKILL.md body with detailed instructions.
3. **Read resources** (as needed) — The agent calls the `read_skill_resource` tool to fetch supplementary files (references, templates, assets) only when required.
4. **Run scripts** (as needed) — The agent calls the `run_skill_script` tool to execute scripts bundled with a skill.

This pattern keeps the agent's context window lean while giving it access to deep domain knowledge on demand.

> [!NOTE]
> `load_skill` is always advertised. `read_skill_resource` is advertised only when at least one skill has resources. `run_skill_script` is advertised only when at least one skill has scripts.

## Providing skills to an agent

`AgentSkillsProvider` (C#) and `SkillsProvider` (Python) are context providers that make skills available to agents. They support three skill sources:

- **File-based** — skills discovered from `SKILL.md` files in filesystem directories
- **Code-defined** — skills defined inline in code using `AgentInlineSkill` (C#) or `Skill` (Python)
- **Class-based** — skills encapsulated in a C# class deriving from `AgentClassSkill<T>` (C# only)

For mixing multiple sources in one provider, use `AgentSkillsProviderBuilder` (C# only — see [Builder: advanced multi-source scenarios](#builder-advanced-multi-source-scenarios)).

:::zone pivot="programming-language-csharp"

## File-based skills

Create an `AgentSkillsProvider` pointing to a directory containing your skills, and add it to the agent's context providers. Pass a script runner to enable execution of file-based scripts found in skill directories:

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI.Responses;

string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!;
string deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

// Discover skills from the 'skills' directory
var skillsProvider = new AgentSkillsProvider(
    Path.Combine(AppContext.BaseDirectory, "skills"));

// Create an agent with the skills provider
AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
    .GetResponsesClient()
    .AsAIAgent(new ChatClientAgentOptions
    {
        Name = "SkillsAgent",
        ChatOptions = new()
        {
            Instructions = "You are a helpful assistant.",
        },
        AIContextProviders = [skillsProvider],
    },
    model: deploymentName);
```

> [!WARNING]
> `DefaultAzureCredential` is convenient for development but requires careful consideration in production. In production, consider using a specific credential (e.g., `ManagedIdentityCredential`) to avoid latency issues, unintended credential probing, and potential security risks from fallback mechanisms.

### Multiple skill directories

You can point the provider to a single parent directory — each subdirectory containing a `SKILL.md` is automatically discovered as a skill:

```csharp
var skillsProvider = new AgentSkillsProvider(
    Path.Combine(AppContext.BaseDirectory, "all-skills"));
```

Or pass a list of paths to search multiple root directories:

```csharp
var skillsProvider = new AgentSkillsProvider(
    [
        Path.Combine(AppContext.BaseDirectory, "company-skills"),
        Path.Combine(AppContext.BaseDirectory, "team-skills"),
    ]);
```

The provider searches up to two levels deep.

### Customizing resource discovery

By default, the provider recognizes resources with extensions `.md`, `.json`, `.yaml`, `.yml`, `.csv`, `.xml`, and `.txt` in `references` and `assets` subdirectories. Use `AgentFileSkillsSourceOptions` to change these defaults:

```csharp
var fileOptions = new AgentFileSkillsSourceOptions
{
    AllowedResourceExtensions = [".md", ".txt"],
    ResourceDirectories = ["docs", "templates"],
};

var skillsProvider = new AgentSkillsProvider(
    Path.Combine(AppContext.BaseDirectory, "skills"),
    fileOptions: fileOptions);
```

### Script execution

Pass `SubprocessScriptRunner.RunAsync` as the second argument to `AgentSkillsProvider` to enable execution of file-based scripts:

```csharp
var skillsProvider = new AgentSkillsProvider(
    Path.Combine(AppContext.BaseDirectory, "skills"),
    SubprocessScriptRunner.RunAsync);
```

`SubprocessScriptRunner.RunAsync` is roughly equivalent to the following:

```csharp
// Simplified equivalent of what SubprocessScriptRunner.RunAsync does internally
using System.Diagnostics;

static async Task<string> RunAsync(AgentSkill skill, AgentSkillScript script, IDictionary<string, object?>? args)
{
    var psi = new ProcessStartInfo("python3")
    {
        RedirectStandardOutput = true,
        UseShellExecute = false,
    };
    psi.ArgumentList.Add(Path.Combine(skill.Path, script.Path));
    if (args != null)
    {
        foreach (var (key, value) in args)
        {
            if (value is not null)
            {
                psi.ArgumentList.Add($"--{key}");
                psi.ArgumentList.Add(value.ToString()!);
            }
        }
    }
    using var process = Process.Start(psi)!;
    string output = await process.StandardOutput.ReadToEndAsync();
    await process.WaitForExitAsync();
    return output.Trim();
}
```

The runner runs each discovered script as a local subprocess, forwarding the JSON arguments provided by the agent as command-line flags.

> [!WARNING]
> `SubprocessScriptRunner` is provided for **demonstration purposes only**. For production use, consider adding:
>
> - Sandboxing (for example, containers or isolated execution environments)
> - Resource limits (CPU, memory, wall-clock timeout)
> - Input validation and allow-listing of executable scripts
> - Structured logging and audit trails

### Customizing script discovery

By default, the provider recognizes scripts with extensions `.py`, `.js`, `.sh`, `.ps1`, `.cs`, and `.csx` in the `scripts` subdirectory. Use `AgentFileSkillsSourceOptions` to change these defaults:

Pass `AgentFileSkillsSourceOptions` to the `AgentSkillsProvider` constructor or to `UseFileSkill` / `UseFileSkills` on the builder:

```csharp
var fileOptions = new AgentFileSkillsSourceOptions
{
    AllowedScriptExtensions = [".py"],
    ScriptDirectories = ["scripts", "tools"],
};

// Via constructor
var skillsProvider = new AgentSkillsProvider(
    Path.Combine(AppContext.BaseDirectory, "skills"),
    fileOptions: fileOptions);

// Via builder
var skillsProvider = new AgentSkillsProviderBuilder()
    .UseFileSkill(Path.Combine(AppContext.BaseDirectory, "skills"), options: fileOptions)
    .Build();
```

:::zone-end

:::zone pivot="programming-language-python"

## File-based skills

Create a `SkillsProvider` pointing to a directory containing your skills, and add it to the agent's context providers:

```python
import os
from pathlib import Path
from agent_framework import SkillsProvider
from agent_framework.openai import OpenAIChatCompletionClient
from azure.identity.aio import AzureCliCredential

# Discover skills from the 'skills' directory
skills_provider = SkillsProvider(
    skill_paths=Path(__file__).parent / "skills"
)

# Create an agent with the skills provider
agent = OpenAIChatCompletionClient(
    model=os.environ["AZURE_OPENAI_CHAT_COMPLETION_MODEL"],
    azure_endpoint=os.environ["AZURE_OPENAI_ENDPOINT"],
    api_version=os.getenv("AZURE_OPENAI_API_VERSION"),
    credential=AzureCliCredential(),
).as_agent(
    name="SkillsAgent",
    instructions="You are a helpful assistant.",
    context_providers=[skills_provider],
)
```

### Multiple skill directories

You can point the provider to a single parent folder — each subfolder containing a `SKILL.md` is automatically discovered as a skill:

```python
skills_provider = SkillsProvider(
    skill_paths=Path(__file__).parent / "all-skills"
)
```

Or pass a list of paths to search multiple root directories:

```python
skills_provider = SkillsProvider(
    skill_paths=[
        Path(__file__).parent / "company-skills",
        Path(__file__).parent / "team-skills",
    ]
)
```

The provider searches up to two levels deep.

### Customizing resource discovery

By default, `SkillsProvider` recognizes resources with extensions `.md`, `.json`, `.yaml`, `.yml`, `.csv`, `.xml`, and `.txt`. It scans all subdirectories within each skill folder. Pass `resource_extensions` to change the recognized file types:

```python
skills_provider = SkillsProvider(
    skill_paths=Path(__file__).parent / "skills",
    resource_extensions=(".md", ".txt"),
)
```

### Script execution

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

:::zone-end

## Code-defined skills

:::zone pivot="programming-language-csharp"

In addition to file-based skills discovered from `SKILL.md` files, you can define skills entirely in code using `AgentInlineSkill`. Code-defined skills are useful when:

- Skill content is generated dynamically (for example, reading from a database or environment).
- You want to keep skill definitions alongside the application code that uses them.
- You need resources that execute logic at read time rather than serving static files.

### Basic code skill

Create an `AgentInlineSkill` with a name, description, and instructions. Attach resources using `.AddResource()`:

```csharp
using Microsoft.Agents.AI;

var codeStyleSkill = new AgentInlineSkill(
    name: "code-style",
    description: "Coding style guidelines and conventions for the team",
    instructions: """
        Use this skill when answering questions about coding style, conventions, or best practices for the team.
        1. Read the style-guide resource for the full set of rules.
        2. Answer based on those rules, quoting the relevant guideline where helpful.
        """)
    .AddResource(
        "style-guide",
        """
        # Team Coding Style Guide
        - Use 4-space indentation (no tabs)
        - Maximum line length: 120 characters
        - Use type annotations on all public methods
        """);

var skillsProvider = new AgentSkillsProvider(codeStyleSkill);
```

### Dynamic resources

Pass a factory delegate to `.AddResource()` to compute the content at runtime. The delegate is invoked each time the agent reads the resource:

```csharp
var projectInfoSkill = new AgentInlineSkill(
    name: "project-info",
    description: "Project status and configuration information",
    instructions: """
        Use this skill for questions about the current project.
        1. Read the environment resource for deployment configuration details.
        2. Read the team-roster resource for information about team members.
        """)
    .AddResource("environment", () =>
    {
        string env = Environment.GetEnvironmentVariable("APP_ENV") ?? "development";
        string region = Environment.GetEnvironmentVariable("APP_REGION") ?? "us-east-1";
        return $"Environment: {env}, Region: {region}";
    })
    .AddResource(
        "team-roster",
        "Alice Chen (Tech Lead), Bob Smith (Backend Engineer)");
```

### Code-defined scripts

Use `.AddScript()` to register a delegate as an executable script. Code-defined scripts run **in-process** as direct delegate calls. No script runner is needed. The delegate's typed parameters are automatically converted into a JSON Schema that the agent uses to pass arguments:

```csharp
using System.Text.Json;

var unitConverterSkill = new AgentInlineSkill(
    name: "unit-converter",
    description: "Convert between common units using a conversion factor",
    instructions: """
        Use this skill when the user asks to convert between units.
        1. Review the conversion-table resource to find the correct factor.
        2. Use the convert script, passing the value and factor from the table.
        3. Present the result clearly with both units.
        """)
    .AddResource(
        "conversion-table",
        """
        # Conversion Tables
        Formula: **result = value × factor**
        | From       | To         | Factor   |
        |------------|------------|----------|
        | miles      | kilometers | 1.60934  |
        | kilometers | miles      | 0.621371 |
        | pounds     | kilograms  | 0.453592 |
        | kilograms  | pounds     | 2.20462  |
        """)
    .AddScript("convert", (double value, double factor) =>
    {
        double result = Math.Round(value * factor, 4);
        return JsonSerializer.Serialize(new { value, factor, result });
    });

var skillsProvider = new AgentSkillsProvider(unitConverterSkill);
```

> [!NOTE]
> To combine code-defined skills with file-based or class-based skills in a single provider, use `AgentSkillsProviderBuilder` — see [Builder: advanced multi-source scenarios](#builder-advanced-multi-source-scenarios).

:::zone-end

:::zone pivot="programming-language-python"

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

:::zone pivot="programming-language-csharp"

## Class-based skills

Class-based skills let you bundle all skill components — name, description, instructions, resources, and scripts — into a single C# class. Derive from `AgentClassSkill<T>` (where `T` is your class), then annotate properties with `[AgentSkillResource]` and methods with `[AgentSkillScript]` for automatic discovery:

```csharp
using System.ComponentModel;
using System.Text.Json;
using Microsoft.Agents.AI;

internal sealed class UnitConverterSkill : AgentClassSkill<UnitConverterSkill>
{
    public override AgentSkillFrontmatter Frontmatter { get; } = new(
        "unit-converter",
        "Convert between common units using a multiplication factor. Use when asked to convert miles, kilometers, pounds, or kilograms.");

    protected override string Instructions => """
        Use this skill when the user asks to convert between units.

        1. Review the conversion-table resource to find the correct factor.
        2. Use the convert script, passing the value and factor from the table.
        3. Present the result clearly with both units.
        """;

    [AgentSkillResource("conversion-table")]
    [Description("Lookup table of multiplication factors for common unit conversions.")]
    public string ConversionTable => """
        # Conversion Tables
        Formula: **result = value × factor**
        | From       | To         | Factor   |
        |------------|------------|----------|
        | miles      | kilometers | 1.60934  |
        | kilometers | miles      | 0.621371 |
        | pounds     | kilograms  | 0.453592 |
        | kilograms  | pounds     | 2.20462  |
        """;

    [AgentSkillScript("convert")]
    [Description("Multiplies a value by a conversion factor and returns the result as JSON.")]
    private static string ConvertUnits(double value, double factor)
    {
        double result = Math.Round(value * factor, 4);
        return JsonSerializer.Serialize(new { value, factor, result });
    }
}
```

Register the class-based skill with `AgentSkillsProvider`:

```csharp
var skill = new UnitConverterSkill();
var skillsProvider = new AgentSkillsProvider(skill);
```

When the `[AgentSkillResource]` attribute is applied to a property or method, its return value is used as the resource content when the agent reads the resource — use a method when the content needs to be computed at read time. When `[AgentSkillScript]` is applied to a method, the method is invoked when the agent calls the script. Use `[Description]` from `System.ComponentModel` to describe each resource and script for the agent.

> [!NOTE]
> `AgentClassSkill<T>` also supports overriding `Resources` and `Scripts` as collections for scenarios where attribute-based discovery does not fit.

:::zone-end

:::zone pivot="programming-language-csharp"

## Builder: advanced multi-source scenarios

For simple, single-source scenarios, use the `AgentSkillsProvider` constructors directly. Use `AgentSkillsProviderBuilder` when you need any of the following:

- **Mixed skill types** — combine file-based, code-defined (`AgentInlineSkill`), and class-based (`AgentClassSkill`) skills in a single provider.
- **Skill filtering** — include or exclude skills using a predicate.

### Mixed skill types

Combine all three skill types in one provider by chaining `UseFileSkill`, `UseSkill`, and `UseFileScriptRunner`:

```csharp
var skillsProvider = new AgentSkillsProviderBuilder()
    .UseFileSkill(Path.Combine(AppContext.BaseDirectory, "skills"))  // file-based skills
    .UseSkill(volumeConverterSkill)                                  // AgentInlineSkill
    .UseSkill(temperatureConverter)                                  // AgentClassSkill
    .UseFileScriptRunner(SubprocessScriptRunner.RunAsync)            // runner for file scripts
    .Build();
```

### Skill filtering

Use `UseFilter` to include only the skills that meet your criteria — for example, to load skills from a shared directory but exclude experimental ones:

```csharp
var approvedSkillNames = new HashSet<string> { "expense-report", "code-style" };

var skillsProvider = new AgentSkillsProviderBuilder()
    .UseFileSkill(Path.Combine(AppContext.BaseDirectory, "skills"))
    .UseFilter(skill => approvedSkillNames.Contains(skill.Frontmatter.Name))
    .Build();
```

:::zone-end

## Script approval

:::zone pivot="programming-language-csharp"

Use `AgentSkillsProviderOptions.ScriptApproval` to gate all script execution behind human approval. When enabled, the agent pauses and returns an approval request instead of executing immediately:

```csharp
var skillsProvider = new AgentSkillsProvider(
    skillPath: Path.Combine(AppContext.BaseDirectory, "skills"),
    options: new AgentSkillsProviderOptions
    {
        ScriptApproval = true,
    });
```

To enable script approval on a builder-configured provider, use `UseScriptApproval`:

```csharp
var skillsProvider = new AgentSkillsProviderBuilder()
    .UseFileSkill(Path.Combine(AppContext.BaseDirectory, "skills"))
    .UseScriptApproval(true)
    .Build();
```

:::zone-end

:::zone pivot="programming-language-python"

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

## Custom system prompt

By default, the skills provider injects a system prompt that lists available skills and instructs the agent to use `load_skill` and `read_skill_resource`. You can customize this prompt:

:::zone pivot="programming-language-csharp"

```csharp
var skillsProvider = new AgentSkillsProvider(
    skillPath: Path.Combine(AppContext.BaseDirectory, "skills"),
    options: new AgentSkillsProviderOptions
    {
        SkillsInstructionPrompt = """
            You have skills available. Here they are:
            {skills}
            {resource_instructions}
            {script_instructions}
            """
    });
```

> [!NOTE]
> The custom template must contain `{skills}` (skill list), `{resource_instructions}` (resource tool hint), and `{script_instructions}` (script tool hint) placeholders. Literal braces must be escaped as `{{` and `}}`.

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

:::zone pivot="programming-language-csharp"

## Caching behavior

By default, skill tools and instructions are cached after the first build. Set `DisableCaching = true` on `AgentSkillsProviderOptions` to force a rebuild on every invocation:

```csharp
var skillsProvider = new AgentSkillsProvider(
    Path.Combine(AppContext.BaseDirectory, "skills"),
    options: new AgentSkillsProviderOptions
    {
        DisableCaching = true,
    });
```

> [!NOTE]
> Disabling caching is useful during development when skill content changes frequently. In production, leave caching enabled (the default) for better performance.

:::zone-end

## Dependency injection

:::zone pivot="programming-language-csharp"

Skill resource and script delegates can declare an `IServiceProvider` parameter that the Agent Framework injects automatically. This lets skills resolve application services — such as database clients, configuration, or business logic — without hard-coding them into the skill definition.

### Setup

Register your application services and pass the built `IServiceProvider` to the agent via the `services` parameter:

```csharp
using Microsoft.Extensions.DependencyInjection;

// Register application services
ServiceCollection services = new();
services.AddSingleton<ConversionService>();
IServiceProvider serviceProvider = services.BuildServiceProvider();

// Create the agent and pass the service provider
AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
    .GetResponsesClient()
    .AsAIAgent(
        options: new ChatClientAgentOptions
        {
            Name = "ConverterAgent",
            ChatOptions = new() { Instructions = "You are a helpful assistant." },
            AIContextProviders = [skillsProvider],
        },
        model: deploymentName,
        services: serviceProvider);
```

### Code-defined skills with DI

Declare `IServiceProvider` as a parameter in `AddResource` or `AddScript` delegates — the framework resolves and injects it automatically when the agent reads a resource or runs a script:

```csharp
var distanceSkill = new AgentInlineSkill(
    name: "distance-converter",
    description: "Convert between distance units (miles and kilometers).",
    instructions: """
        Use this skill when the user asks to convert between miles and kilometers.
        1. Read the distance-table resource for conversion factors.
        2. Use the convert script to compute the result.
        """)
    .AddResource("distance-table", (IServiceProvider sp) =>
    {
        return sp.GetRequiredService<ConversionService>().GetDistanceTable();
    })
    .AddScript("convert", (double value, double factor, IServiceProvider sp) =>
    {
        return sp.GetRequiredService<ConversionService>().Convert(value, factor);
    });
```

### Class-based skills with DI

Annotate methods with `[AgentSkillResource]` or `[AgentSkillScript]` and declare an `IServiceProvider` parameter — the framework discovers these members via reflection and injects the service provider automatically:

```csharp
internal sealed class WeightConverterSkill : AgentClassSkill<WeightConverterSkill>
{
    public override AgentSkillFrontmatter Frontmatter { get; } = new(
        "weight-converter",
        "Convert between weight units (pounds and kilograms).");

    protected override string Instructions => """
        Use this skill when the user asks to convert between pounds and kilograms.
        1. Read the weight-table resource for conversion factors.
        2. Use the convert script to compute the result.
        """;

    [AgentSkillResource("weight-table")]
    [Description("Lookup table of multiplication factors for weight conversions.")]
    private static string GetWeightTable(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<ConversionService>().GetWeightTable();
    }

    [AgentSkillScript("convert")]
    [Description("Multiplies a value by a conversion factor and returns the result as JSON.")]
    private static string Convert(double value, double factor, IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<ConversionService>().Convert(value, factor);
    }
}
```

> [!TIP]
> Class-based skills can also resolve dependencies through their **constructor**. Register the skill class in the `ServiceCollection` and resolve it from the container instead of calling `new` directly:
>
> ```csharp
> services.AddSingleton<WeightConverterSkill>();
> var weightSkill = serviceProvider.GetRequiredService<WeightConverterSkill>();
> ```
>
> This is useful when the skill class itself needs injected services beyond what the resource and script delegates use.

:::zone-end

:::zone pivot="programming-language-python"

Resource and script functions that accept `**kwargs` automatically receive runtime keyword arguments passed to `agent.run()`. This lets skill functions access application context — such as configuration, user identity, or service clients — without hard-coding them into the skill definition.

### Passing runtime arguments

Pass `function_invocation_kwargs` to `agent.run()` to supply keyword arguments that the framework forwards to resource and script functions:

```python
response = await agent.run(
    "How many kilometers is 26.2 miles?",
    function_invocation_kwargs={"precision": 2, "user_id": "alice"},
)
```

### Resource functions with kwargs

When a resource function declares `**kwargs`, the framework forwards the runtime keyword arguments each time the agent reads the resource:

```python
from typing import Any
from agent_framework import Skill

project_info_skill = Skill(
    name="project-info",
    description="Project status and configuration information",
    content="Use this skill for questions about the current project.",
)

@project_info_skill.resource(name="environment", description="Current environment configuration")
def environment(**kwargs: Any) -> Any:
    """Return environment config, optionally scoped to a user."""
    user_id = kwargs.get("user_id", "anonymous")
    env = os.environ.get("APP_ENV", "development")
    return f"Environment: {env}, Caller: {user_id}"
```

Resource functions without `**kwargs` are called with no arguments and do not receive runtime context.

### Script functions with kwargs

When a script function declares `**kwargs`, the framework forwards the runtime keyword arguments alongside the `args` provided by the agent:

```python
import json
from typing import Any
from agent_framework import Skill

converter_skill = Skill(
    name="unit-converter",
    description="Convert between common units using a conversion factor",
    content="Use the convert script to perform unit conversions.",
)

@converter_skill.script(name="convert", description="Convert a value: result = value × factor")
def convert_units(value: float, factor: float, **kwargs: Any) -> str:
    """Convert a value using a multiplication factor.

    Args:
        value: The numeric value to convert (provided by the agent).
        factor: Conversion factor (provided by the agent).
        **kwargs: Runtime keyword arguments from agent.run().
    """
    precision = kwargs.get("precision", 4)
    result = round(value * factor, precision)
    return json.dumps({"value": value, "factor": factor, "result": result})
```

The agent provides `value` and `factor` through the tool call `args`; the application provides `precision` through `function_invocation_kwargs`. Script functions without `**kwargs` receive only the agent-provided arguments.

:::zone-end

## Security best practices

Agent Skills should be treated like any third-party code you bring into your project.Because skill instructions are injected into the agent's context — and skills can include scripts — applying the same level of review and governance you would to an open-source dependency is essential.

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
- [Running Agents](./running-agents.md)
- [Tools Overview](./tools/index.md)
