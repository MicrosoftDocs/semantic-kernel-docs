---
name: toc
description: >
  How to create, structure, and configure table of contents (TOC) files
  for Microsoft Learn documentation using toc.yml and docfx.json.
---

# Skill: Table of Contents (TOC) on Microsoft Learn

## Purpose

This skill describes how to create, structure, and configure table of contents
files (`toc.yml`) for documentation published on Microsoft Learn. The TOC defines
the left-hand navigation structure of a docset.

## Goals for TOCs

- Present a useful amount of content while staying scannable
- Match the customer's likely use cases for a product or technology
- Allow rapid zooming in and out among topics
- Help users form mental models of how a product is organized

## YAML TOC Format

TOCs must be YAML (not Markdown). Create a file named `toc.yml` (always lowercase).

### Basic Structure

```yaml
items:
- name: Tutorial
  items:
  - name: Introduction
    href: tutorial.md
  - name: Step 1
    href: step-1.md
  - name: Step 2
    href: step-2.md
```

Parent nodes contain an `items` list of children. If you add an `href` to a
parent node, the build system automatically creates a duplicate child node with
the parent's name and link — the parent itself becomes an expander only.

### Node Properties

| Property | Required | Description |
|----------|----------|-------------|
| `name` | Yes | Display text for the TOC node. Cannot include a colon (`:`). |
| `href` | No | Path the node leads to. Omit for parent-only nodes. |
| `displayName` | No | Additional search terms for TOC filtering (comma-separated). Not displayed to users. |
| `uid` | No | Identifier for reference documentation (e.g., `System.String`). |
| `items` | No | Child nodes, each with the same available properties. |
| `expanded` | No | Set to `true` to expand this node by default on page load. Only **one** root-level node can be expanded. |

> [!WARNING]
> Do not use `maintainContext`. It is no longer supported. Use contextual TOC
> links instead (see [Contextual TOCs](#contextual-tocs)).

### Advanced Example

```yaml
- name: Dev sandbox
  href: index.md
  displayName: Home
- name: Conceptual pages
  expanded: true
  items:
  - name: Overview
    href: ./conceptual/index.md
  - name: Code samples
    href: ./conceptual/code.md
- name: Reference Pages
  items:
  - name: IDictionary
    href: ./reference/System.Collection.IDictionary.yml
  - name: String
    href: ./reference/System.String.yml
```

> [!TIP]
> Validate your YAML with [YAML Lint](https://www.yamllint.com/). Add `items:`
> as the first line to avoid parser errors.

## docfx.json Configuration

Ensure `"**/**.yml"` is listed as a content file type so the build picks up TOC files:

```json
"build": {
  "content": [
    {
      "files": ["**/*.md", "**/**.yml"],
      "exclude": ["**/obj/**"]
    }
  ]
}
```

## Single vs. Multiple TOCs

### Single TOC

Best for most products and services. One `toc.yml` plus one landing page.
Top-level (L1) nodes represent the main content categories.

### Multiple TOCs

For large products with diverse content areas, connect multiple TOCs via a
central **hub page**. The hub page itself has no TOC — it links to landing
pages, each with its own focused TOC.

**Decision factors:**

- Product complexity and breadth of content
- Number of files — each TOC should cover a meaningful end-to-end task area
- Shared content — more sharing favors a single TOC

## TOC and Product Directory

The TOC structure should relate to the product directory (the landing/hub page
that showcases the product's content areas) but does **not** need to be a 1:1
match. The TOC may group or split content differently than the product directory
to best serve navigation within the docset.

> [!TIP]
> For reference images showing product directory layouts, see the
> [Microsoft Learn static media index](https://learn.microsoft.com/static/media/index.html).

## Nested TOCs

Nest one TOC inside another by pointing `href` to a child `toc.yml`:

```yaml
items:
- name: Azure overview
  href: azure-overview.md
- name: Extensibility
  href: extensibility/toc.yml
- name: Reference
  href: azure-reference.md
```

Users stay in the containing TOC when selecting nested links. If the node
linked to `extensibility/overview.md` instead of `extensibility/toc.yml`,
selecting it would navigate the user to a different TOC.

> [!NOTE]
> If a user arrives at a nested TOC article via search, the full parent TOC
> is displayed, not just the nested portion.

## TOC Best Practices

### Content and Context

- All articles in a TOC should display the **same TOC** — don't surprise
  users by landing them in a different TOC
- All links should go to articles, not to other TOCs (use hub pages for that)
- Selecting the rightmost breadcrumb should always return to the current TOC

### Size Guidelines

- Keep **3–12 items** per section. Fewer than 3 may not warrant a section;
  more than 12 becomes hard to scan.
- Avoid going more than **3 levels deep**.

### Labels and Links

- TOC labels should be short but match the article's H1
- Parent nodes should expand, not be links (the build duplicates linked parents)
- External links must live in a **Resources** node; all other nodes should
  link to Microsoft Learn content

### Common Mistakes to Avoid

| ❌ Don't | ✅ Do |
|----------|-------|
| Link to the same file more than once | Use a single link per article |
| Link to a different TOC from within a TOC | Use a contextual TOC instead |
| Duplicate info from hub/landing pages | Provide complementary structure |
| Include root folder in href (`/docs/marketplace/overview.md`) | Use relative path (`marketplace/overview.md`) |
| Begin href with a slash (`/collaborate/overview.md`) | Omit leading slash (`collaborate/overview.md`) |

## Contextual TOCs

When your TOC links to articles in another folder or repo, the user's
navigation context (TOC, breadcrumbs, header) normally changes. A
**contextual TOC** preserves your docset's context instead.

### Three Files Involved

| File | Purpose |
|------|---------|
| `toc.yml` | Links to external articles with forced context parameters |
| `breadcrumb/toc.yml` | Maps external article URLs to your breadcrumb hierarchy |
| `context/context.yml` | (Optional) Bundles brand, breadcrumb, and TOC references |

> [!IMPORTANT]
> Publish breadcrumb and context file changes **before** publishing TOC changes.
> Contextual links can't be previewed until these files are live.

### Forced TOC and Breadcrumb Paths

Append query parameters to the `href` in your TOC:

```yaml
# Forced TOC only
- name: Secure SQL data
  href: ../sql-database/sql-database-always-encrypted.md?toc=/azure/key-vault/general/toc.json

# Forced TOC + breadcrumb
- name: Secure SQL data
  href: ../sql-database/sql-database-always-encrypted.md?toc=/azure/key-vault/general/toc.json&bc=/azure/key-vault/general/breadcrumb/toc.json
```

### Context Files (Optional Shorthand)

A context file combines brand, breadcrumb, and TOC into a single reference:

```yaml
### YamlMime: ContextObject
brand: azure
breadcrumb_path: ../breadcrumb/toc.yml
toc_rel: ../toc.yml
```

Then reference it in your TOC with a single parameter:

```yaml
- name: SCCM Run Scripts
  href: /sccm/apps/deploy-use/create-deploy-scripts?context=/azure/key-vault/context/kv-context
```

> [!NOTE]
> Context files have limitations with moniker views (e.g., `?view=azurermps-6.2.0`).
> In those cases, use explicit `?toc=` and `&bc=` parameters instead.

## Relationship Between TOC and Breadcrumbs

The TOC and breadcrumb are separate navigation systems with different purposes:

- The **TOC** (`toc.yml`) provides left-hand navigation within a docset —
  showing where the user is and what content is available nearby.
- The **breadcrumb** (`breadcrumb/toc.yml`) provides top-of-page links showing
  where the docset sits in the overall site hierarchy.

The breadcrumb's `tocHref` property maps URL paths to breadcrumb entries. When
using contextual TOCs, the breadcrumb file must also be updated so that
externally linked articles show your product's breadcrumb rather than their own.

For breadcrumb file creation and configuration details, see the
[breadcrumbs skill](..\breadcrumbs\SKILL.md).

## When to Apply This Skill

- When creating a new docset and its initial navigation structure
- When reorganizing content into new sections or sub-products
- When linking to articles outside your docset while preserving context
- When deciding between single vs. multiple TOCs for a large product

## Reference

- [Overview of TOCs](https://learn.microsoft.com/help/platform/toc-overview)
- [How to set up a contextual TOC](https://learn.microsoft.com/help/platform/contextual-toc)
