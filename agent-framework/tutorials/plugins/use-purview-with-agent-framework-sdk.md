---
title: Use Microsoft Purview SDK with Agent Framework
description: Learn how to integrate Microsoft Purview SDK for data security and governance in your Agent Framework project
author: reezaali149
ms.topic: conceptual
ms.author: v-reezaali
ms.date: 10/28/2025
ms.service: purview
---

# Use Microsoft Purview SDK with Agent Framework

Microsoft Purview provides enterprise-grade data security, compliance, and governance capabilities for AI applications. By integrating Purview APIs within the Agent Framework SDK, developers can build intelligent agents that are secure by design, while ensuring sensitive data in prompts and responses are protected and compliant with organizational policies.

## Why integrate Purview with Agent Framework?

- **Prevent sensitive data leaks**: Inline blocking of sensitive content based on Data Loss Prevention (DLP) policies.
- **Enable governance**: Log AI interactions in Purview for Audit, Communication Compliance, Insider Risk Management, eDiscovery, and Data Lifecycle Management.
- **Accelerate adoption**: Enterprise customers require compliance for AI apps. Purview integration unblocks deployment.

## Prerequisites

Before you begin, ensure you have:

- Microsoft Azure subscription with Microsoft Purview configured.
- Microsoft 365 subscription with an E5 license and pay-as-you-go billing setup.
  - For testing, you can use a Microsoft 365 Developer Program tenant. For more information, see [Join the Microsoft 365 Developer Program](https://developer.microsoft.com/en-us/microsoft-365/dev-program).
- Agent Framework SDK: To install the Agent Framework SDK:
  - Python: Run `pip install agent-framework`.
  - .NET: Install from NuGet.

## How to integrate Microsoft Purview into your agent

In your agent's workflow middleware pipeline, you can add Microsoft Purview policy middleware to intercept prompts and responses to determine if they meet the policies set up in Microsoft Purview. The Agent Framework SDK is capable of intercepting agent-to-agent or end-user chat client prompts and responses.

The following code sample demonstrates how to add the Microsoft Purview policy middleware to your agent code. If you're new to Agent Framework, see [Create and run an agent with Agent Framework](/agent-framework/tutorials/agents/run-agent?pivots=programming-language-python).

### [C#](#tab/csharp)

```csharp
using AgentFramework;
using AgentFramework.Azure;
using AgentFramework.Microsoft;
using Azure.Identity;

var chatClient = new AzureOpenAIChatClient();
var purviewMiddleware = new PurviewPolicyMiddleware(
    new InteractiveBrowserCredential(),
    new PurviewSettings { AppName = "My Secure Agent" }
);

var agent = new ChatAgent(chatClient, "You are a secure assistant.");
agent.AddMiddleware(purviewMiddleware);

var response = await agent.RunAsync(new ChatMessage(Role.User, "Summarize zero trust in one sentence."));
Console.WriteLine(response);
```

### [Python](#tab/python)

```python
import asyncio
from agent_framework import ChatAgent, ChatMessage, Role
from agent_framework.azure import AzureOpenAIChatClient
from agent_framework.microsoft import PurviewPolicyMiddleware, PurviewSettings
from azure.identity import InteractiveBrowserCredential

async def main():
    chat_client = AzureOpenAIChatClient()
    purview_middleware = PurviewPolicyMiddleware(
        credential=InteractiveBrowserCredential(),
        settings=PurviewSettings(app_name="My Secure Agent")
    )
    agent = ChatAgent(
        chat_client=chat_client,
        instructions="You are a secure assistant.",
        middleware=[purview_middleware]
    )
    response = await agent.run(ChatMessage(role=Role.USER, text="Summarize zero trust in one sentence."))
    print(response)

asyncio.run(main())
```

---

## Next steps

Now that you added the above code to your agent, perform the following steps to test the integration of Microsoft Purview into your code:

1. **Entra registration**: Register your agent and add the required Microsoft Graph permissions (`dataSecurityAndGovernance`) to the Service Principal. For more information, see [Register an application in Microsoft Entra ID](/entra/identity-platform/quickstart-register-app) and [dataSecurityAndGovernance resource type](/graph/api/resources/datasecurityandgovernance). You'll need the Microsoft Entra app ID in the next step.
1. **Purview policies**: Configure Purview policies using the Microsoft Entra app ID to enable agent communications data to flow into Purview. For more information, see [Configure Microsoft Purview](https://learn.microsoft.com/purview/developer/configurepurview).

## Resources

- [PyPI Package: Microsoft Agent Framework - Purview Integration (Python)](https://pypi.org/project/agent-framework-purview/).
- [GitHub: Microsoft Agent Framework – Purview Integration (Python) source code](https://github.com/microsoft/agent-framework/tree/main/python/packages/purview).
- [Code Sample: Purview Policy Enforcement Sample (Python)](https://github.com/microsoft/agent-framework/tree/main/python/samples/getting_started/purview_agent).