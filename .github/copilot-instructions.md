# Copilot Instructions for Semantic Kernel Documentation

This repository contains documentation for two separate Microsoft products, published via Microsoft Docs (DocFX):

| Docset | Languages | Source Code | Published URL |
|--------|-----------|-------------|---------------|
| **Semantic Kernel** (`semantic-kernel/`) | C#, Python, Java | [microsoft/semantic-kernel](https://github.com/microsoft/semantic-kernel) | [/semantic-kernel](https://learn.microsoft.com/semantic-kernel) |
| **Agent Framework** (`agent-framework/`) | C#, Python, Go | [microsoft/agent-framework](https://github.com/microsoft/agent-framework), [microsoft/agent-framework-go](https://github.com/microsoft/agent-framework-go) | [/agent-framework](https://learn.microsoft.com/agent-framework) |

Each docset is independent with its own `docfx.json`, `TOC.yml`, and `zone-pivot-groups.yml`.

## Documentation Conventions

### YAML Frontmatter

Every markdown file requires this frontmatter:

```yaml
---
title: Page title
description: Brief description
zone_pivot_groups: programming-languages  # If showing code in multiple languages
author: github-username
ms.topic: conceptual
ms.author: microsoft-alias
ms.date: MM/DD/YYYY
ms.service: semantic-kernel
---
```

### Zone Pivots for Multi-Language Content

Zone pivots show language-specific content. **Note the different language support per docset:**

**Semantic Kernel** (C#, Python, Java):
```markdown
::: zone pivot="programming-language-csharp"
C# content here
::: zone-end

::: zone pivot="programming-language-python"
Python content here
::: zone-end

::: zone pivot="programming-language-java"
Java content here
::: zone-end
```

**Agent Framework** (C#, Python, Go):
```markdown
::: zone pivot="programming-language-csharp"
C# content here
::: zone-end

::: zone pivot="programming-language-python"
Python content here
::: zone-end

::: zone pivot="programming-language-go"
Go content here
::: zone-end
```

### Code Samples

Reference code from external sample repositories using DocFX syntax:

```markdown
:::code language="csharp" source="~/../semantic-kernel-samples/path/to/file.cs" id="SnippetId":::
```

Sample repositories (configured in `.openpublishing.publish.config.json`):
- `semantic-kernel-samples` → [microsoft/semantic-kernel](https://github.com/microsoft/semantic-kernel) (main branch)
- `semantic-kernel-samples-java` → [microsoft/semantic-kernel-java](https://github.com/microsoft/semantic-kernel-java)
- `agent-framework-code` → [microsoft/agent-framework](https://github.com/microsoft/agent-framework) (main branch)
- `agent-framework-go` → [microsoft/agent-framework-go](https://github.com/microsoft/agent-framework-go) (main branch)

### Table of Contents

Each section has a `TOC.yml` file. When adding new pages, update the appropriate TOC:

```yaml
- name: Page Title
  href: page-file.md
```

### Links

Use docs-relative paths:
- Cross-docset: `/semantic-kernel/concepts/kernel` or `/agent-framework/overview/`
- Same docset: `./sibling-page.md` or `../parent/page.md`

### Media Files

Place images in `media/` folders within each section. Use SVG for diagrams when possible.

## Key Patterns

- **Tip boxes**: `> [!TIP]` for helpful hints
- **Notes**: `> [!NOTE]` for important information
- **Warnings**: `> [!WARNING]` for cautions
- **Next steps buttons**: `> [!div class="nextstepaction"]` followed by a link

## What NOT to Include

- Do not add build/test commands (this is a pure documentation repo)
- Do not reference `_themes/` or `_themes.pdf/` directories (managed externally)
- Files in `**/includes/**` and `**/obj/**` are excluded from builds
