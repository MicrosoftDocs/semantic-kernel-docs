---
name: breadcrumbs
description: >
  How breadcrumbs work on Microsoft Learn: structure, creation, configuration
  in docfx.json, and best practices for documentation repos.
---

# Skill: Breadcrumbs on Microsoft Learn

## Purpose

This skill describes how breadcrumbs are structured, created, and configured
for documentation published on Microsoft Learn. Breadcrumbs are navigation
links at the top of a page that convey an article's position in the site
hierarchy.

## Overview

Breadcrumbs allow users to navigate through the site hierarchy back to the
home page, one level at a time. Most pages should have breadcrumbs.

### Pages Without Breadcrumbs

- **L1 pages**: Microsoft Learn home, Training home, Q&A home, Code Samples home
- **L2 pages**: Hub pages, training browse, certification browse, certification overview, Learn Events
- **Search results pages**

## Breadcrumb Structure

Breadcrumbs on Learn are intentionally simple and flat. The general pattern is:

```
Learn / [Offering name]
```

When an offering belongs to a branded hierarchical family:

```
Learn / [Offering name] / [Sub-offering name]
```

Each breadcrumb links to a hub page or, if unavailable, a landing page in a TOC.

> [!NOTE]
> The breadcrumb shows where the TOC sits relative to the rest of the site,
> **not** where the user is within the TOC. This is an intentional shift to
> keep breadcrumbs simpler, shorter, and more coherent.

### By Content Type

| Content Type | Structure | Example |
|-------------|-----------|---------|
| Documentation | `Learn / Product family (if applicable) / Offering / Sub-offering (if applicable)` | `Learn / Azure / Cognitive Services / Custom Vision Service` |
| Training paths & modules | `Learn / Training / Learning paths & modules` | — |
| Training units | `Learn / Training / Learning paths & modules / Learning path name` | `Learn / Training / Learning paths & modules / Introduction to Azure fundamentals` |
| Certifications | `Learn / Certifications` | — |
| Shows | `Learn / Shows` | — |
| Show episodes | `Learn / Shows / Show name` | `Learn / Shows / AI Show` |
| Events | `Learn / Events` | — |
| Event sessions | `Learn / Events / Event name` | `Learn / Events / Azure Static Web Apps Anniversary` |
| Code samples | `Learn / Code samples` | — |

## Creating a Breadcrumb File

### Step 1: Locate the Breadcrumb Folder

Look for a `breadcrumb/` folder in the root of your docset containing a `toc.yml` file.
This folder is created during repo provisioning. In some repos the folder is called `bread/`.

### Step 2: Determine Your Structure

Follow the patterns above. You can map breadcrumbs for all docsets in one file,
or create multiple breadcrumb files for special cases.

### Step 3: Find Your Index Files

Locate the `index.yml` or `index.md` files for your product or service.
Their URLs are used as breadcrumb link targets.

### Step 4: Add Breadcrumb Entries

Use this YAML syntax in `/breadcrumb/toc.yml`. The "Learn" crumb is added
automatically at build time — do not include it.

```yaml
items:
- name: Azure
  tocHref: /azure/
  topicHref: /azure/index
  items:
  - name: Azure DevOps
    tocHref: /azure/devops/
    topicHref: /azure/devops/index
    items:
    - name: Azure Boards
      tocHref: /azure/devops/boards/
      topicHref: /azure/devops/boards/index
```

### Required Properties

| Property | Description |
|----------|-------------|
| `name` | Display text in the breadcrumb |
| `tocHref` | URL path corresponding to the name. Must start and end with `/`. Articles whose URL contains this value are mapped to this breadcrumb entry. Can be a relative path to the TOC folder or an absolute path from the site domain. |
| `topicHref` | URL that opens when a user selects the breadcrumb link. Must start with `/`, be an absolute path from the site domain, and **not** include a file extension. |

> [!IMPORTANT]
> Breadcrumbs do not work with nested TOCs.

> [!TIP]
> Validate your YAML syntax with a tool like [YAML Lint](https://www.yamllint.com/).

## Configuring docfx.json

### 1. Include YAML Files in Content

Ensure `"**/**.yml"` is in the `files` array of the `content` section:

```json
"content": [
  {
    "files": ["**/**.md", "**/**.yml"],
    "exclude": ["**/obj/**"]
  }
]
```

### 2. Set the Breadcrumb Path

Add `breadcrumb_path` to `globalMetadata`:

```json
"globalMetadata": {
  "breadcrumb_path": "/<docset-base-url>/breadcrumb/toc.json"
}
```

The path uses the docset's base URL plus the breadcrumb folder location.
The file type must be `.json` (not `.yml`).

### 3. Remove extendBreadcrumb

If `"extendBreadcrumb": true` exists in `globalMetadata`, remove it or set it
to `false`. Breadcrumbs should not duplicate the TOC.

## Multiple Breadcrumb Files

Use multiple files only for special cases. To set them up:

1. Create subfolders under `breadcrumb/` with additional `toc.yml` files.
2. Use `fileMetadata` in `docfx.json` to map folders to breadcrumb files:

```json
"fileMetadata": {
  "breadcrumb_path": {
    "folder-a/**/*.md": "/docset/breadcrumb/subfolder-a/toc.json",
    "folder-b/**/*.md": "/docset/breadcrumb/subfolder-b/toc.json"
  }
}
```

## Relationship with TOC

The breadcrumb and TOC are separate navigation systems:

- The **TOC** (`toc.yml`) controls the left-hand sidebar — showing where the
  user is within a docset and what sibling/child content is available.
- The **breadcrumb** (`breadcrumb/toc.yml`) controls the top-of-page links —
  showing where the docset sits in the overall site hierarchy.

The breadcrumb's `tocHref` maps URL paths to breadcrumb entries. When using
**contextual TOCs** (linking to articles in other folders/repos while preserving
your docset's navigation), both the TOC and breadcrumb files must be updated
together. The breadcrumb file needs a mapping so that externally linked articles
show your product's breadcrumb rather than their own.

> [!IMPORTANT]
> Breadcrumbs do not work with nested TOCs. If you nest one `toc.yml` inside
> another, the nested portion will not generate its own breadcrumb entries.

For TOC creation and contextual TOC details, see the
[TOC skill](..\toc\SKILL.md).

## When to Apply This Skill

- When onboarding a new docset and setting up initial breadcrumbs
- When restructuring an existing docset's navigation hierarchy
- When troubleshooting missing or incorrect breadcrumb display
- When adding a new sub-offering under an existing product family

## Reference

- [Overview of breadcrumbs](https://learn.microsoft.com/help/platform/breadcrumbs-overview)
- [How to create or update a breadcrumb file](https://learn.microsoft.com/help/platform/breadcrumb-create-update)