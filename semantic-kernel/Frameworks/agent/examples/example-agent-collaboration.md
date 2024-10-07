---
title: How-To&colon; Coordinate Agent Collaboration using Agent Group Chat (Experimental)
description: TBD $$$
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---
# How-To: Coordinate Agent Collaboration using Agent Group Chat (Experimental)

> [!WARNING]
> The _Semantic Kernel Agent Framework_ is experimental, still in development and is subject to change.

## Overview

In this sample, we will explore how to use _Agent Group Chat_ to coordinate collboration of three different agents working to author content in response to user direction.  Each agent is assigned a distinct role:

- **Planner**: Responsible for creating outline and providing strucural input
- **Reviewer**: Reviews and provides critical feedback for both _Planner_ and _Writer_ output.
- **Writer**: Authors content based on _Planner_ and _Reviewer_ input.

## Getting Started

Before proceeding with feature coding, make sure your development environment is fully set up and configured.

::: zone pivot="programming-language-csharp"

Start by creating a _Console_ project. Then, include the following package references to ensure all required dependencies are available.

```xml
  <ItemGroup>
    <PackageReference Include="Azure.Identity" Version="<stable>" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="<stable>" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="<stable>" />
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="<stable>" />
    <PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="<stable>" />
    <PackageReference Include="Microsoft.SemanticKernel.Agents.Core" Version="<latest>" />
    <PackageReference Include="Microsoft.SemanticKernel.Connectors.AzureOpenAI" Version="<latest>" />
  </ItemGroup>
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## Configuration

This sample requires configuration setting in order to connect to remote services.  You will need to define settings for either _Open AI_ or _Azure Open AI_.

::: zone pivot="programming-language-csharp"

```powershell
# Open AI
dotnet user-secrets set "OpenAISettings:ApiKey" "<api-key>"
dotnet user-secrets set "OpenAISettings:ChatModel" "gpt-4o"

# Azure Open AI
dotnet user-secrets set "AzureOpenAISettings:ApiKey" "<api-key>" # Not required if using token-credential
dotnet user-secrets set "AzureOpenAISettings:Endpoint" "<model-endpoint>"
dotnet user-secrets set "AzureOpenAISettings:ChatModelDeployment" "gpt-4o"
```

The following class is used in all of the Agent examples. Be sure to include it in your project to ensure proper functionality. This class serves as a foundational component for the examples that follow.

```c#
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace AgentsSample;

public class Settings
{
    private readonly IConfigurationRoot configRoot;

    private AzureOpenAISettings azureOpenAI;
    private OpenAISettings openAI;

    public AzureOpenAISettings AzureOpenAI => this.azureOpenAI ??= this.GetSettings<Settings.AzureOpenAISettings>();
    public OpenAISettings OpenAI => this.openAI ??= this.GetSettings<Settings.OpenAISettings>();

    public class OpenAISettings
    {
        public string ChatModel { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }

    public class AzureOpenAISettings
    {
        public string ChatModelDeployment { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }

    public TSettings GetSettings<TSettings>() =>
        this.configRoot.GetRequiredSection(typeof(TSettings).Name).Get<TSettings>()!;

    public Settings()
    {
        this.configRoot =
            new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
                .Build();
    }
}
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## Coding

The coding process for this sample involves:

1. [Setup](#setup) - Initializing settings and the plug-in.
2. [Agent Definition](#agent-definition) - Create the _Chat_Completion_Agent_ with templatized instructions and plug-in.
3. [The _Chat_ Loop](#the-chat-loop) - Write the loop that drives user / agent interaction.

The full example code is provided in the [Final](#final) section. Refer to that section for the complete implementation.

### Setup

::: zone pivot="programming-language-csharp"
```csharp
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

### Agent Definition

::: zone pivot="programming-language-csharp"
```csharp
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end

### The _Chat_ Loop

::: zone pivot="programming-language-csharp"
```csharp
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end


## Final

Bringing all the steps together, we have the final code for this example. The complete implementation is provided below.

::: zone pivot="programming-language-csharp"
```csharp
```
::: zone-end

::: zone pivot="programming-language-python"
```python
```
::: zone-end

::: zone pivot="programming-language-java"
::: zone-end