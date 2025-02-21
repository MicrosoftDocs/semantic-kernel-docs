---
title: Understanding the Experimental Attribute in Semantic Kernel
description: Learn about experimental attribute in Semantic Kernel, why they exist, and how to use them.
zone_pivot_groups: programming-languages
author: shethaadit
ms.topic: conceptual
ms.author: shethaadit
ms.date: 02/21/2025
ms.service: semantic-kernel
---

# Experimental Features in Semantic Kernel

Semantic Kernel introduces experimental features to provide early access to new, evolving capabilities. These features allow users to explore cutting-edge functionality, but they are not yet stable and may be modified, deprecated, or removed in future releases.

## Purpose of Experimental Features

The `Experimental` attribute serves several key purposes:

- **Signals Instability** – Indicates that a feature is still evolving and not yet production-ready.
- **Encourages Early Feedback** – Allows developers to test and provide input before a feature is fully stabilized.
- **Manages Expectations** – Ensures users understand that experimental features may have limited support or documentation.
- **Facilitates Rapid Iteration** – Enables the team to refine and improve features based on real-world usage.
- **Guides Contributors** – Helps maintainers and contributors recognize that the feature is subject to significant changes.

## Implications for Users

Using experimental features comes with certain considerations:

- **Potential Breaking Changes** – APIs, behavior, or entire features may change without prior notice.
- **Limited Support** – The Semantic Kernel team may provide limited or no support for experimental features.
- **Stability Concerns** – Features may be less stable and prone to unexpected behavior or performance issues.
- **Incomplete Documentation** – Experimental features may have incomplete or outdated documentation.

### Suppressing Experimental Feature Warnings in .NET

In the .NET SDK, experimental features generate compiler warnings. To suppress these warnings in your project, add the relevant diagnostic IDs to your `.csproj` file:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);SKEXP0001,SKEXP0010</NoWarn>
</PropertyGroup>
```

Each experimental feature has a unique diagnostic code (`SKEXPXXXX`). The full list can be found in **[EXPERIMENTS.md](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/docs/EXPERIMENTS.md)**.

## Using Experimental Features in .NET

In .NET, experimental features are marked using the `[Experimental]` attribute:

```csharp
using System;
using System.Diagnostics.CodeAnalysis;

[Experimental("SKEXP0101", "FeatureCategory")]
public class NewFeature 
{
    public void ExperimentalMethod() 
    {
        Console.WriteLine("This is an experimental feature.");
    }
}
```

### Experimental Feature Support in Other SDKs

- **Python and Java** do not have a built-in experimental feature system like .NET.
- Experimental features in **Python** may be marked using warnings (e.g., `warnings.warn`).
- In **Java**, developers typically use custom annotations to indicate experimental features.

## Developing and Contributing to Experimental Features

### Marking a Feature as Experimental

- Apply the `Experimental` attribute to classes, methods, or properties:

```csharp
[Experimental("SKEXP0101", "FeatureCategory")]
public class NewFeature { }
```

- Include a brief description explaining why the feature is experimental.
- Use meaningful tags as the second argument to categorize and track experimental features.

### Coding and Documentation Best Practices

- **Follow Coding Standards** – Maintain Semantic Kernel's coding conventions.
- **Write Unit Tests** – Ensure basic functionality and prevent regressions.
- **Document All Changes** – Update relevant documentation, including `EXPERIMENTS.md`.
- **Use GitHub for Discussions** – Open issues or discussions to gather feedback.
- **Consider Feature Flags** – Where appropriate, use feature flags to allow opt-in/opt-out.

### Communicating Changes

- Clearly document updates, fixes, or breaking changes.
- Provide migration guidance if the feature is evolving.
- Tag the relevant GitHub issues for tracking progress.

## Future of Experimental Features

Experimental features follow one of three paths:

1. **Graduation to Stable** – If a feature is well-received and technically sound, it may be promoted to stable.
2. **Deprecation & Removal** – Features that do not align with long-term goals may be removed.
3. **Continuous Experimentation** – Some features may remain experimental indefinitely while being iterated upon.

The Semantic Kernel team strives to communicate experimental feature updates through release notes and documentation updates.

## Getting Involved

The community plays a crucial role in shaping the future of experimental features. Provide feedback via:

- **GitHub Issues** – Report bugs, request improvements, or share concerns.
- **Discussions & PRs** – Engage in discussions and contribute directly to the codebase.

## Summary

- **Experimental features** allow users to test and provide feedback on new capabilities in Semantic Kernel.
- **They may change frequently**, have limited support, and require caution when used in production.
- **Contributors should follow best practices**, use `[Experimental]` correctly, and document changes properly.
- **Users can suppress warnings** for experimental features but should stay updated on their evolution.

For the latest details, check **[EXPERIMENTS.md](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/docs/EXPERIMENTS.md)**.
