---
# These are optional elements. Feel free to remove any of them.
status: accepted
contact: sergeymenshykh
date: 2025-05-13
deciders: markwallace, rbarreto, dmytrostruk, westey-m
consulted: 
informed:
---

## Context and Problem Statement

Currently, Semantic Kernel (SK) advertises **all** functions to the AI model, regardless of their source, whether they are from all registered plugins or provided directly when configuring function choice behavior. This approach works perfectly for most scenarios where there are not too many functions, and the AI model can easily choose the right one.

However, when there are many functions available, AI models may struggle to select the appropriate function, leading to confusion and suboptimal performance. This can result in the AI model calling functions that are not relevant to the current context or conversation, potentially causing the entire scenario to fail.

This ADR consider different options to provide context-based function selection and advertisement mechanism to such components as SK agents, chat completion services, and M.E.AI chat clients.

## Decision Drivers
- It should be possible to advertise functions dynamically based on the context of the conversation.
- It should seamlessly integrate with SK and M.E.AI AI connectors and SK agents.
- It should have access to context and functions without the need for complex plumbing.