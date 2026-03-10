---
name: per-concept-documentation
description: Required when reviewing, updating, auditing, or creating any concept documentation page. Covers code accuracy checks against source repos, language parity across zone pivots, parity table comments, and structural consistency.
---

To create or update documentation for a specific concept in the project, follow this process:

1. Identify the concept for which the user is seeking new or updated documentation. This could be a feature, function, class, or any other relevant topic within the project. The project can be either Agent Framework or Semantic Kernel.
2. Gather information about the concept. Ask the following questions if they are not already answered:
   - What is the concept?
   - What language does it pertain to (e.g., Python, .NET, or both)?
   - The code path to the concept (this is usually a link to the source code in GitHub).
   - Samples of how to use the concept, if applicable.
3. If the concept is already documented, review the existing documentation to identify any gaps or areas that need updating. If the concept is not yet documented, proceed to create new documentation (ask the location where the documentation should be created if not already defined).
4. Documentation follows the format of the existing documentation in the project. Ensure that the new or updated documentation is consistent with the style and structure of the existing documentation.
5. If code snippets are needed, refer to the "code-snippets" skill for guidance on how to create code snippets in the documentation.
6. **Verify code accuracy against source.** For each language zone, compare the documented APIs (class names, method signatures, imports, parameters) against the current source code in the corresponding GitHub repository. Use GitHub search and file retrieval tools to check the actual source files, not just samples.
7. **Check language parity.** If the documentation covers multiple languages via zone pivots, compare the sections side by side to ensure each language covers the same set of concepts. Build a parity table listing each section and whether it is present for each language. Identify:
   - Sections present in one language but missing in another.
   - Language-specific features that are intentionally only in one zone (mark these explicitly).
   - Differences in depth or structure for the same concept across languages.
8. **Add a parity comment.** Insert an HTML comment immediately after the YAML frontmatter containing the language parity table. This table should be maintained as sections are added or removed. Use ✅ for included concepts, ❌ for excluded concepts, and add a notes column for explanations (e.g., "Python-specific"). Example:

   ```html
   <!--
     Language parity table – keep in sync when adding/removing sections.

     | Section                    | C# | Python | Notes           |
     |----------------------------|:--:|:------:|-----------------|
     | Basic Structure            | ✅ |   ✅   |                 |
     | Function-Based             | ✅ |   ✅   |                 |
     | Explicit Type Parameters   | ❌ |   ✅   | Python-specific |
     | The Context Object         | ✅ |   ✅   |                 |
   -->
   ```

9. **Resolve parity gaps.** For each missing section identified in the parity table, determine whether it should be added (the concept exists in the other language's SDK) or marked as language-specific. Add equivalent sections where appropriate, mirroring the structure and depth of the existing language zone.
10. **Treat each language zone as an isolated document.** Do not compare or contrast one language's implementation with another within the zone content itself. For example, avoid phrases like "Unlike C#, Python does not use a RequestPort" or "the same guessing game workflow." Each zone pivot should read as a standalone document that makes sense without any knowledge of the other language's zone.
11. **Update the date.** After making changes, update the `ms.date` field in the YAML frontmatter to the current date.
