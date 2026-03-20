---
name: cross-repository-references
description: >
  How to configure cross-repository references (CRR) and create API class links
  for Microsoft Learn docs in this repository.
---

# Skill: Cross-Repository References (CRR) and API Class Links

## Purpose

Use this skill when you need to:

- reference files from another repository during Docs build (CRR)
- add stable links to Python API classes in Microsoft Learn docs pages

CRR replaces Git submodule-based references for docs build scenarios and is easier to reason about for doc authors.

## Where CRR is configured

CRR is configured in:

- `.openpublishing.publish.config.json`

The key is:

- `dependent_repositories`

## CRR schema

```json
{
  "dependent_repositories": [
    {
      "path_to_root": "<relative path>",
      "url": "<repository URL>",
      "branch": "<default branch>",
      "branch_mapping": {
        "<source branch>": "<dependency branch>"
      }
    }
  ]
}
```

### Fields

| Field | Required | Meaning |
|---|---|---|
| `dependent_repositories` | Yes | List of CRR entries |
| `path_to_root` | Yes | Where the dependency is mounted in the build workspace |
| `url` | Yes | Dependency repository URL |
| `branch` | Yes | Default dependency branch |
| `branch_mapping` | No | Optional source->dependency branch override map |

## Current repo baseline

This repository already uses CRR in `.openpublishing.publish.config.json` for:

- `_themes`
- `_themes.pdf`
- `semantic-kernel-samples`
- `semantic-kernel-samples-java`

Prefer extending the existing `dependent_repositories` list instead of introducing alternate mechanisms.

## Path semantics (`~`, `..`) for references

- `~` resolves to the docset root (`build_source_folder`)
- in this repo, for Agent Framework docs, docset root is `agent-framework/`
- use `~/..` to move from docset root to repository root

Example (from Agent Framework docset to CRR sample repo):

```text
~/../semantic-kernel-samples/path/to/file.py
```

## Adding a new CRR dependency

1. Add a `dependent_repositories` entry in `.openpublishing.publish.config.json`
2. Use dependency paths from docs content (`[!INCLUDE]`, `:::code`, or static asset references)
3. If needed, add exclusion rules in `docfx.json` so referenced source trees are not published as content
4. Add dependency mount folders to `.gitignore` if local build artifacts create noise

## API class links (Python)

For Agent Framework Python API links, use Learn URLs with explicit package + module/class paths.

### Preferred link forms

- Module landing:
  - `/python/api/agent-framework-core/agent_framework?view=agent-framework-python-latest`
- Class page:
  - `/python/api/agent-framework-core/agent_framework.baseagent?view=agent-framework-python-latest`
  - `/python/api/agent-framework-core/agent_framework.agent?view=agent-framework-python-latest`
  - `/python/api/agent-framework-core/agent_framework.supportschatgetresponse?view=agent-framework-python-latest`
  - `/python/api/agent-framework-core/agent_framework.supportsagentrun?view=agent-framework-python-latest`
  - `/python/api/agent-framework-core/agent_framework.agentresponseupdate?view=agent-framework-python-latest`

### Markdown pattern

```markdown
[`BaseAgent`](/python/api/agent-framework-core/agent_framework.baseagent?view=agent-framework-python-latest)
```

### Practical guidance

- prefer class-level links when the class page exists
- if a class page does not exist, link to the module landing page
- when code and API reference differ, prefer code-leading class names and matching URL slug
- keep link text aligned with surrounding docs terminology
- use relative Learn paths (`/python/api/...`) instead of hardcoded absolute hostnames

## Validation checklist

- link resolves to an existing Learn page
- `view=agent-framework-python-latest` is present
- no broken relative paths (`./`, `../`) introduced
- doc wording stays accurate after class/protocol name linking

## References

- [Python API docs: agent-framework-core](https://learn.microsoft.com/en-us/python/api/agent-framework-core/agent_framework?view=agent-framework-python-latest)
