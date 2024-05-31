---
title: Provide native code to your agents
description: Learn how to add and invoke native code as plugins in Semantic Kernel.
author: sophialagerkranspandey
ms.topic: conceptual
ms.author: sopand
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Add native code as a plugin

The easiest way to provide an AI agent with capabilities that are not natively supported is to wrap native code into a plugin. This allows you to leverage your existing skills as an app developer to extend the capabilities of your AI agents.

Behind the scenes, Semantic Kernel will then use the descriptions you provide, along with reflection, to semantically describe the plugin to the AI agent. This allows the AI agent to understand the capabilities of the plugin and how to interact with it.

## Providing the LLM with the right information

When authoring a plugin, you need to provide the AI agent with the right information to understand the capabilities of the plugin and its functions. This includes:
- The name of the plugin
- The names of the functions
- The descriptions of the functions
- The parameters of the functions
- The schema of the parameters

The value of Semantic Kernel is that it can automatically generate most of this information from the code itself. As a developer, this just means that you must provide the semantic descriptions of the functions and parameters so the AI agent can understand them. If you properly comment and annotate your code, however, you likely already have this information on hand.

## Authoring a native plugin

Below, we'll walk through the different ways of providing your AI agent with native code.

1. 

### Defining a plugin using a class

The easiest way to create a native plugin is to start with a class and then add methods annotated with the `PluginFunction` attribute.

```csharp
```

Notice above that we've provided a constructor for out plugin class. This is one of the main benefits of the plugin abstraction in Semantic Kernel. By providing a constructor, you can inject dependencies into your plugin, making it easier to reuse common services and components across your plugin functions.

Once you're done authoring your plugin class, you can add it to the kernel using the `AddFromType<>` or `AddFromObject` methods.

#### Adding a plugin using the `AddFromType<>` method

When using the `AddFromType<>` method, the kernel will automatically use dependency injection to create an instance of the plugin class and add it to the plugin collection.

```csharp
```

#### Adding a plugin using the `AddFromObject` method

The `AddFromObject` method allows you to add an instance of the plugin class directly to the plugin collection in case you want to directly control how the plugin is constructed.

```csharp
```

## Defining a plugin using a collection of functions

Less common but still useful is defining a plugin using a collection of functions. This is particularly useful if you need to dynamically create a plugin from a set of functions at runtime.

Using this process requires you to use the function factory to create individual functions before adding them to the plugin.

```csharp
```

## Additional strategies for adding native code with Dependency Injection

If you're working with Dependency Injection, there are additional ways to create and add plugins to the kernel. Below are some examples of how you can add a plugin using Dependency Injection.

### Inject a plugin collection

> [!TIP]
> We recommend making your plugin collection a transient service so that it is disposed of after each use since the plugin collection is mutable. Creating a new plugin collection for each use is cheap, so it should not be a performance concern.

```csharp
```

### Generate your plugins as singletons

Plugins are not mutable, so its typically safe to create them as singletons. This can be done by using the plugin factory and adding the resulting plugin to your service collection.

```csharp
```

## Next steps
Now that you know how to create a plugin, you can now learn how to use them with your AI agent. Depending on the type of functions you've added to your plugins, there are different patterns you should follow. For retrieval functions, refer to the [using retrieval functions](./using-data-retrieval-functions-for-rag.md) article. For task automation functions, refer to the [using task automation functions](./using-task-automation-functions.md) article.