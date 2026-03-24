---
name: redirection
description: >
  How to delete, rename, or move articles on Microsoft Learn while preserving
  Platform IDs, preventing broken links, and managing redirects.
---

# Skill: Redirection on Microsoft Learn

## Purpose

This skill describes how to properly redirect content when articles are deleted,
renamed, or moved on Microsoft Learn. Proper redirection prevents broken links,
avoids 404 errors, and ensures the Platform ID algorithm correctly associates
the new location with the original content.

## When Redirection Is Required

Redirection is **always** required for:

- **Renaming files** (not just article titles)
- **Moving a file** to a different folder or directory
- **Migrating content** to a new repository
- **Merging files**

These operations are treated as "remove and replace" by the platform, not as
in-place updates.

## The Redirection File

Redirects are managed in a JSON file at the repo root, typically named
`.openpublishing.redirection.json`. Each entry maps an old source path to a
new target URL.

### Entry Format

```json
{
  "redirections": [
    {
      "source_path_from_root": "/articles/ai-services/content-safety/concepts/custom-categories-rapid.md",
      "redirect_url": "/azure/ai-services/content-safety/concepts/custom-categories",
      "redirect_document_id": true
    }
  ]
}
```

### Properties

| Property | Required | Description |
|----------|----------|-------------|
| `source_path_from_root` | Yes | Absolute path to the old file in the repo. Must start with `/`. |
| `redirect_url` | Yes | Target URL — can be a relative path, a site-relative path on Microsoft Learn, or an external URL. |
| `redirect_document_id` | No | Set to `true` to transfer the document ID (and reporting data like page views and rankings) from the old article to the target. Defaults to `false` if omitted. |

## Platform ID

The Platform ID is a unique identifier for Learn documents, designed to replace
the Document ID system. Unlike Document ID, the Platform ID is **automatically
generated** based on content similarity and URL redirection relationships.

### Best Practices for Platform ID Continuity

1. **Don't change content during file operations.** The Platform ID relies on
   a 90% content similarity threshold. If similarity drops below 90%, the
   platform treats the documents as distinct entities with different IDs.
   Rename or move first, then make content changes in a separate PR.

2. **Eliminate duplicate content:**
   - **Same repo:** Delete the original and set up redirection in the same PR.
   - **Cross-repo migration:** Publish in the target repo and create the
     redirect in a first PR, then delete the old document within 24 hours
     in a second PR. After 24 hours the Platform ID is finalized.

> [!WARNING]
> Not following redirection policies risks losing historical BI data. The
> Platform ID is also used by recommendations and search services — changes
> to the ID can make content inaccessible or degrade search results.

## Redirection Workflows

### Deprecating Content (Keep Published, Remove from Search)

Add the `ROBOTS: NOINDEX` metadata as the last entry in the article's YAML
frontmatter, then republish:

```yaml
ROBOTS: NOINDEX
```

This tells search engines not to index the page while still allowing crawlers
to follow links on it to other pages.

### Deleting an Article

1. Ensure you're listed as `author` in the article's metadata (update if not).
2. Delete the article file.
3. Add a redirect entry in `.openpublishing.redirection.json` pointing to an
   appropriate replacement page.
4. Include both the deletion and the redirect in the **same PR**.

### Renaming a File

1. Create a copy of the article with the new file name.
2. Delete the old file.
3. Add a redirect from the old path to the new path in the redirection file.
4. Update the TOC entry for the article.

### Moving a File

Same process as renaming — create the file in the new location, delete the
original, add the redirect, and update the TOC.

## Updating Cross-References

Don't rely on redirects alone for internal links. After any file operation,
scan for and update cross-references:

```powershell
# Find all files linking to the old article
Get-ChildItem -Recurse -Include *.md |
    Select-String '<old-article-name>' |
    Group-Object Path |
    Select-Object Name
```

Update or remove these cross-links in the same or a follow-up PR.

## Post-Merge Checklist

After your PR merges, complete these steps:

| Step | Details |
|------|---------|
| **Test redirects in staging** | Verify the old URL redirects to the correct target before signing off |
| **Update FWLinks** | Check the FWLink tool for any links pointing to the old article and update them |
| **Manage inbound links** | Identify high-traffic non-Microsoft inbound links (blogs, forums) and coordinate updates if possible |
| **Remove search cache** (rare) | Only if content must be removed urgently for legal or severe customer issues — use [Bing](https://www.bing.com/webmasters/tools/contentremoval) and [Google](https://search.google.com/search-console/removals) removal tools |
| **Review old redirects** | Periodically review and clean up redirect entries based on your org's retention policy |
| **Check Platform ID Dashboard** | Verify that file URL changes follow best practices |

## Relationship with TOC and Breadcrumbs

When you delete, rename, or move an article, you must also update the
corresponding [TOC entry](..\toc\SKILL.md) to reflect the new file path.
If the move changes the article's URL structure, verify that the
[breadcrumb](..\breadcrumbs\SKILL.md) `tocHref` mappings still apply correctly.

## Reference

- [Redirect articles](https://learn.microsoft.com/help/platform/redirect-articles)
- [Moving or refactoring files in a repository](https://learn.microsoft.com/help/platform/move-refactor-files)
- [Platform ID Dashboard](https://learn.microsoft.com/help/platform/platform-id-dashboard)
