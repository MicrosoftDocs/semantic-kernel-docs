---
title: How to contribute to Semantic Kernel
description: Help improve Semantic Kernel by contributing to the project and docs. 
author: matthewbolanos
ms.topic: contributor-guide
ms.author: mabolan
ms.date: 02/07/2023
ms.service: semantic-kernel
---

# Contributing to Semantic Kernel

You can contribute to Semantic Kernel by submitting issues, starting discussions, and submitting pull requests (PRs). Contributing code is greatly appreciated, but simply filing issues for problems you encounter is also a great way to contribute since it helps us focus our efforts. 

## Reporting issues and feedback
We always welcome bug reports, API proposals, and overall feedback. Since we use GitHub, you can use the [Issues](https://github.com/microsoft/semantic-kernel/issues) and [Discussions](https://github.com/microsoft/semantic-kernel/discussions) tabs to start a conversation with the team. Below are a few tips when submitting issues and feedback so we can respond to your feedback as quickly as possible.

### Reporting issues
New issues for the SDK can be reported in our [list of issues](https://github.com/microsoft/semantic-kernel/issues), but before you file a new issue, please search the list of issues to make sure it does not already exist. If you have issues with the Semantic Kernel documentation (this site), please file an issue in the [Semantic Kernel documentation repository](https://github.com/MicrosoftDocs/semantic-kernel-docs/issues).

If you _do_ find an existing issue for what you wanted to report, please include your own feedback in the discussion. We also highly recommend up-voting (👍 reaction) the original post, as this helps us prioritize popular issues in our backlog. 

#### Writing a Good Bug Report
Good bug reports make it easier for maintainers to verify and root cause the underlying problem. The better a bug report, the faster the problem can be resolved. Ideally, a bug report should contain the following information:

- A high-level description of the problem.
- A _minimal reproduction_, i.e. the smallest size of code/configuration required
  to reproduce the wrong behavior.
- A description of the _expected behavior_, contrasted with the _actual behavior_ observed.
- Information on the environment: OS/distribution, CPU architecture, SDK version, etc.
- Additional information, e.g. Is it a regression from previous versions? Are there any known workarounds?

> [!div class="nextstepaction"]
> [Create issue](https://github.com/microsoft/semantic-kernel/issues)

### Submitting feedback
If you have general feedback on Semantic Kernel or ideas on how to make it better, please share it on our [discussions board](https://github.com/microsoft/semantic-kernel/discussions). Before starting a new discussion, please search the list of discussions to make sure it does not already exist.

We recommend using the [ideas category](https://github.com/microsoft/semantic-kernel/discussions/categories/ideas) if you have a specific idea you would like to share and the [Q&A category](https://github.com/microsoft/semantic-kernel/discussions/categories/q-a) if you have a question about Semantic Kernel.

You can also start discussions (and share any feedback you've created) in the Discord community by joining the [Semantic Kernel Discord server](https://aka.ms/sk/discord).

> [!div class="nextstepaction"]
> [Start a discussion](https://github.com/microsoft/semantic-kernel/discussions)

### Help us prioritize feedback
We currently use up-votes to help us prioritize issues and features in our backlog, so please up-vote any issues or discussions that you would like to see addressed.

If you think others would benefit from a feature, we also encourage you to ask others to up-vote the issue. This helps us prioritize issues that are impacting the most users. You can ask colleagues, friends, or the [community on Discord](https://aka.ms/sk/discord) to up-vote an issue by sharing the link to the issue or discussion.

## Submitting pull requests
We welcome contributions to Semantic Kernel. If you have a bug fix or new feature that you would like to contribute, please follow the steps below to submit a pull request (PR). Afterwards, project maintainers will review code changes and merge them once they've been accepted.

### Recommended contribution workflow

We recommend using the following workflow to contribute to Semantic Kernel (this is the same workflow used by the Semantic Kernel team):

1. Create an issue for your work.
   - You can skip this step for trivial changes.
   - Reuse an existing issue on the topic, if there is one.
   - Get agreement from the team and the community that your proposed change is
     a good one by using the discussion in the issue.
   - Clearly state in the issue that you will take on implementation. This allows us to assign the issue to you and ensures that someone else does not accidentally works on it.
2. Create a personal fork of the repository on GitHub (if you don't already have one).
3. In your fork, create a branch off of main (`git checkout -b mybranch`).
   - Name the branch so that it clearly communicates your intentions, such as
     "issue-123" or "githubhandle-issue".
4. Make and commit your changes to your branch.
5. Add new tests corresponding to your change, if applicable.
6. Build the repository with your changes.
   - Make sure that the builds are clean.
   - Make sure that the tests are all passing, including your new tests.
7. Create a PR against the repository's **main** branch.
   - State in the description what issue or improvement your change is addressing.
   - Verify that all the Continuous Integration checks are passing.
8. Wait for feedback or approval of your changes from the code maintainers.
9. When area owners have signed off, and all checks are green, your PR will be merged.

### Dos and Don'ts while contributing
The following is a list of Dos and Don'ts that we recommend when contributing to Semantic Kernel to help us review and merge your changes as quickly as possible.

#### Do's:
- **Do** follow the standard
  [.NET coding style](/dotnet/csharp/fundamentals/coding-style/coding-conventions)
  and [Python code style](https://pypi.org/project/black/)
- **Do** give priority to the current style of the project or file you're changing
  if it diverges from the general guidelines.
- **Do** include tests when adding new features. When fixing bugs, start with
  adding a test that highlights how the current behavior is broken.
- **Do** keep the discussions focused. When a new or related topic comes up
  it's often better to create new issue than to side track the discussion.
- **Do** clearly state on an issue that you are going to take on implementing it.
- **Do** blog and/or tweet about your contributions!

#### Don'ts:
- **Don't** surprise the team with big pull requests. We want to support contributors, so we recommend filing an issue and starting a discussion so we can agree on a direction before you invest a large amount of time.
- **Don't** commit code that you didn't write. If you find code that you think is a good fit to add to Semantic Kernel, file an issue and start a discussion before proceeding.
- **Don't** submit PRs that alter licensing related files or headers. If you believe there's a problem with them, file an issue and we'll be happy to discuss it.
- **Don't** make new APIs without filing an issue and discussing with the team first. Adding new public surface area to a library is a big deal and we want to make sure we get it right.

### Breaking Changes
Contributions must maintain API signature and behavioral compatibility. If you want to make a change that will break existing code, please file an issue to discuss your idea or change if you believe that a breaking change is warranted. Otherwise, contributions that include breaking changes will be rejected.

### The continuous integration (CI) process
The continuous integration (CI) system will automatically perform the required builds and run tests (including the ones you should also run locally) for PRs. Builds and test runs must be clean before a PR can be merged. 

If the CI build fails for any reason, the PR issue will be updated with a link that can be used to determine the cause of the failure so that it can be addressed.

### Contributing to documentation
We also accept contributions to the [Semantic Kernel documentation repository](https://github.com/MicrosoftDocs/semantic-kernel-docs/issues). To learn how to make contributions, please start with the Microsoft [docs contributor guide](/contribute).