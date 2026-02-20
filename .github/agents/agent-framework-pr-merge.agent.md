# Agent Framework PR Merge Documentation Update

This instruction is triggered when a PR is merged in the agent-framework repository. Your job is to update documentation accordingly and track Python breaking changes.

## Context

- **Source repo:** [microsoft/agent-framework](https://github.com/microsoft/agent-framework) - The PR being merged is from this repo
- **Docs repo:** This repository (`semantic-kernel-pr`) contains the documentation to update
- **Docset location:** `agent-framework/`
- **Significant changes file:** `agent-framework/support/upgrade/python-2026-significant-changes.md`
- **Languages supported:** C# and Python (no Java)

## Workflow

### Step 1: Analyze the Merged PR

Review the PR to understand:
1. **Check PR title for `[BREAKING]` prefix** - This is the primary indicator of a breaking change
2. What changed (API, behavior, configuration, etc.)
3. Which language(s) are affected (C#, Python, or both)
4. Whether it's a breaking change (even if not tagged - contributors sometimes forget the prefix)
5. Whether it's a version update

### Step 2: Update Relevant Documentation

For any PR that affects user-facing behavior or APIs:

1. **Identify affected docs** - Search `agent-framework/` for mentions of changed APIs, classes, methods, or concepts
2. **Update code samples** - Ensure all code examples reflect the new API/behavior
3. **Update conceptual docs** - Revise explanations if behavior changed
4. **Update TOC.yml** - If adding new pages or restructuring

Follow these conventions:
- Use zone pivots for language-specific content (C# and Python only)
- Update `ms.date` in frontmatter of modified files
- Maintain consistency with existing documentation style

### Step 3: Handle Python Breaking Changes

If the PR introduces a **Python breaking change**, add an entry to `agent-framework/support/upgrade/python-2026-significant-changes.md`:

#### Format for Breaking Change Entry

Add under the appropriate version header (create new header if needed):

```markdown
## python-{version} ({Month} {Day}, {Year})

### {Brief description of breaking change}

**PR:** [#{pr_number}](https://github.com/microsoft/agent-framework/pull/{pr_number})

{One or two sentence explanation of what changed and why.}

**Before:**
```python
# Old code that no longer works
```

**After:**
```python
# New code showing the correct approach
```

---
```

#### Unreleased Breaking Changes

If a breaking change is merged but **not yet released**, add it under an "Unreleased" section at the top of the file:

```markdown
## Unreleased (Coming Soon)

> [!NOTE]
> These changes have been merged but are not yet available in a published release.

### {Brief description of breaking change}

**PR:** [#{pr_number}](https://github.com/microsoft/agent-framework/pull/{pr_number})

{Explanation of what changed.}

**Before:**
```python
# Old code
```

**After:**
```python
# New code
```

---
```

When the version is released, rename the header from `## Unreleased (Coming Soon)` to `## python-{version} ({Month} {Day}, {Year})` and remove the note about being unreleased.

#### Ordering Rules

1. **Version headers** - Newest versions at the top, oldest at the bottom
2. **Changes within a version** - Order by significance/impact
3. **Summary table** - Add a row to the summary table at the bottom of the file

#### Summary Table Entry

Add a row to the summary table:

```markdown
| {version} | {Brief change description} | [#{pr_number}](https://github.com/microsoft/agent-framework/pull/{pr_number}) |
```

### Step 4: Handle Version Updates

If the PR is a **version update** (e.g., bumping package version):

1. Check if there is an `## Unreleased (Coming Soon)` section in `python-2026-significant-changes.md`
2. If so, rename the header to `## python-{version} ({Month} {Day}, {Year})` with the new version
3. Remove the `> [!NOTE]` block about changes being unreleased
4. Add any entries from the unreleased section to the summary table at the bottom
5. Ensure the version format matches the pattern: `python-{version}` (e.g., `python-1.0.0b260128`)

## Breaking Change Criteria

### How to Identify Breaking Changes

**Check both of the following:**
1. **PR labels** — Look for a `breaking-change` label
2. **PR title** — Look for a `[BREAKING]` prefix

Either indicator means the PR contains breaking changes. If neither is present, review the PR content. A change is considered **breaking** if it:
- Renames a class, method, function, or parameter
- Removes a public API
- Changes method signatures (parameters, return types)
- Changes default behavior that could break existing code
- Requires code changes to upgrade
- Changes exception types or error handling behavior

A change is **NOT breaking** if it:
- Adds new optional parameters with defaults
- Adds new classes or methods
- Fixes bugs without changing expected behavior
- Improves performance without API changes
- Updates internal/private implementation

### Significant (Non-Breaking) Changes

Some changes are important to document even if not breaking. Consider adding entries for:
- New generic type parameters that improve type inference
- Major new features or capabilities
- Changes to recommended patterns or best practices

## Example Workflow

**Scenario:** PR #3413 renames `AIFunction` to `FunctionTool` and `@ai_function` to `@tool` in Python

1. **Analyze:** Breaking change affecting Python API (check for `breaking-change` label)
2. **Update docs:** Search for `AIFunction` and `@ai_function` in `agent-framework/` and update all occurrences
3. **Add breaking change entry:**

```markdown
## python-1.0.0b260128 (January 28, 2026)

### 🔴 `AIFunction` renamed to `FunctionTool` and `@ai_function` renamed to `@tool`

**PR:** [#3413](https://github.com/microsoft/agent-framework/pull/3413)

The class and decorator have been renamed for clarity and consistency with industry terminology.

**Before:**
```python
from agent_framework.core import ai_function, AIFunction

@ai_function
def get_weather(city: str) -> str:
    return f"Weather in {city}: Sunny"
```

**After:**
```python
from agent_framework.core import tool, FunctionTool

@tool
def get_weather(city: str) -> str:
    return f"Weather in {city}: Sunny"
```

---
```

4. **Update summary table:**

```markdown
| 1.0.0b260128 | `AIFunction` → `FunctionTool`, `@ai_function` → `@tool` | [#3413](https://github.com/microsoft/agent-framework/pull/3413) |
```
