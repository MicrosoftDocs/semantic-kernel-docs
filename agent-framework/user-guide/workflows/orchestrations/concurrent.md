---
title: Microsoft Agent Framework Workflows Orchestrations - Concurrent
description: In-depth look at Concurrent Orchestrations in Microsoft Agent Framework Workflows.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/12/2025
ms.service: agent-framework
---


# Microsoft Agent Framework Workflows Orchestrations - Concurrent

Concurrent orchestration enables multiple agents to work on the same task in parallel. Each agent processes the input independently, and their results are collected and aggregated. This approach is well-suited for scenarios where diverse perspectives or solutions are valuable, such as brainstorming, ensemble reasoning, or voting systems.

![Concurrent Orchestration](../resources/images/orchestration-concurrent.png)

## What You'll Learn

- How to define multiple agents with different expertise
- How to orchestrate these agents to work concurrently on a single task
- How to collect and process the results

## Define Your Agents

::: zone pivot="programming-language-csharp"

```csharp
// Define the agents
ChatClientAgent physicist =
    this.CreateAgent(
        instructions: "You are an expert in physics. You answer questions from a physics perspective.",
        description: "An expert in physics");
ChatClientAgent chemist =
    this.CreateAgent(
        instructions: "You are an expert in chemistry. You answer questions from a chemistry perspective.",
        description: "An expert in chemistry");
```

More coming soon...

::: zone-end

::: zone pivot="programming-language-python"

Agents are specialized entities that can process tasks. Here, we define three agents: a research expert, a marketing expert, and a legal expert.

```python
from agent_framework.azure import AzureChatClient

# 1) Create three domain agents using AzureChatClient
chat_client = AzureChatClient(credential=AzureCliCredential())

researcher = chat_client.create_agent(
    instructions=(
        "You're an expert market and product researcher. Given a prompt, provide concise, factual insights,"
        " opportunities, and risks."
    ),
    name="researcher",
)

marketer = chat_client.create_agent(
    instructions=(
        "You're a creative marketing strategist. Craft compelling value propositions and target messaging"
        " aligned to the prompt."
    ),
    name="marketer",
)

legal = chat_client.create_agent(
    instructions=(
        "You're a cautious legal/compliance reviewer. Highlight constraints, disclaimers, and policy concerns"
        " based on the prompt."
    ),
    name="legal",
)
```

## Set Up the Concurrent Orchestration

The `ConcurrentBuilder` class allows you to construct a workflow to run multiple agents in parallel. You pass the list of agents as participants.

```python
from agent_framework import ConcurrentBuilder

# 2) Build a concurrent workflow
# Participants are either Agents (type of AgentProtocol) or Executors
workflow = ConcurrentBuilder().participants([researcher, marketer, legal]).build()
```

## Run the Concurrent Workflow and Collect the Results

```python
from agent_framework import ChatMessage, WorkflowCompletedEvent

# 3) Run with a single prompt, stream progress, and pretty-print the final combined messages
completion: WorkflowCompletedEvent | None = None
async for event in workflow.run_stream("We are launching a new budget-friendly electric bike for urban commuters."):
    if isinstance(event, WorkflowCompletedEvent):
        completion = event

if completion:
    print("===== Final Aggregated Conversation (messages) =====")
    messages: list[ChatMessage] | Any = completion.data
    for i, msg in enumerate(messages, start=1):
        name = msg.author_name if msg.author_name else "user"
        print(f"{'-' * 60}\n\n{i:02d} [{name}]:\n{msg.text}")
```

## Sample Output

```plaintext
Sample Output:

    ===== Final Aggregated Conversation (messages) =====
    ------------------------------------------------------------

    01 [user]:
    We are launching a new budget-friendly electric bike for urban commuters.
    ------------------------------------------------------------

    02 [researcher]:
    **Insights:**

    - **Target Demographic:** Urban commuters seeking affordable, eco-friendly transport;
        likely to include students, young professionals, and price-sensitive urban residents.
    - **Market Trends:** E-bike sales are growing globally, with increasing urbanization,
        higher fuel costs, and sustainability concerns driving adoption.
    - **Competitive Landscape:** Key competitors include brands like Rad Power Bikes, Aventon,
        Lectric, and domestic budget-focused manufacturers in North America, Europe, and Asia.
    - **Feature Expectations:** Customers expect reliability, ease-of-use, theft protection,
        lightweight design, sufficient battery range for daily city commutes (typically 25-40 miles),
        and low-maintenance components.

    **Opportunities:**

    - **First-time Buyers:** Capture newcomers to e-biking by emphasizing affordability, ease of
        operation, and cost savings vs. public transit/car ownership.
    ...
    ------------------------------------------------------------

    03 [marketer]:
    **Value Proposition:**
    "Empowering your city commute: Our new electric bike combines affordability, reliability, and
        sustainable design—helping you conquer urban journeys without breaking the bank."

    **Target Messaging:**

    *For Young Professionals:*
    ...
    ------------------------------------------------------------

    04 [legal]:
    **Constraints, Disclaimers, & Policy Concerns for Launching a Budget-Friendly Electric Bike for Urban Commuters:**

    **1. Regulatory Compliance**
    - Verify that the electric bike meets all applicable federal, state, and local regulations
        regarding e-bike classification, speed limits, power output, and safety features.
    - Ensure necessary certifications (e.g., UL certification for batteries, CE markings if sold internationally) are obtained.

    **2. Product Safety**
    - Include consumer safety warnings regarding use, battery handling, charging protocols, and age restrictions.
```

::: zone-end

## Next steps

> [!div class="nextstepaction"]
> [Sequential Orchestration](./sequential.md)
