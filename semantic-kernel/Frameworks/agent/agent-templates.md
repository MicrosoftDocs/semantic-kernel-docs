---
title: Create an Agent from a Semantic Kernel Template
description: Describes how to use a Semantic Kernel template to define an agent.
zone_pivot_groups: programming-languages
author: crickman
ms.topic: tutorial
ms.author: crickman
ms.date: 09/13/2024
ms.service: semantic-kernel
---

# Create an Agent from a Semantic Kernel Template

## Prompt Templates in Semantic Kernel

An agent's role is primarily shaped by the instructions it receives, which dictate its behavior and actions. Similar to invoking a `Kernel` [prompt](../../concepts/prompts/index.md), an agent's instructions can include templated parameters—both values and functions—that are dynamically substituted during execution. This enables flexible, context-aware responses, allowing the agent to adjust its output based on real-time input.

Additionally, an agent can be configured directly using a Prompt Template Configuration, providing developers with a structured and reusable way to define its behavior. This approach offers a powerful tool for standardizing and customizing agent instructions, ensuring consistency across various use cases while still maintaining dynamic adaptability.

::: zone pivot="programming-language-csharp"

> [!TIP]
> API reference:
>
> - [`PromptTemplateConfig`](/dotnet/api/microsoft.semantickernel.prompttemplateconfig)
> - [`KernelFunctionYaml.FromPromptYaml`](</dotnet/api/microsoft.semantickernel.kernelfunctionyaml.frompromptyaml#microsoft-semantickernel-kernelfunctionyaml-frompromptyaml(system-string-microsoft-semantickernel-iprompttemplatefactory-microsoft-extensions-logging-iloggerfactory)>)
> - [`IPromptTemplateFactory`](/dotnet/api/microsoft.semantickernel.iprompttemplatefactory)
> - [`KernelPromptTemplateFactory`](/dotnet/api/microsoft.semantickernel.kernelprompttemplatefactory)
> - [_Handlebars_](/dotnet/api/microsoft.semantickernel.prompttemplates.handlebars)
> - [_Prompty_](/dotnet/api/microsoft.semantickernel.prompty)
> - [_Liquid_](/dotnet/api/microsoft.semantickernel.prompttemplates.liquid)

::: zone-end

::: zone pivot="programming-language-python"

> [!TIP]
> API reference:
>
> - [`prompt_template_config`](/python/api/semantic-kernel/semantic_kernel.prompt_template.prompt_template_config)
> - [`kernel_prompt_template`](/python/api/semantic-kernel/semantic_kernel.prompt_template.kernel_prompt_template)
> - [`jinja2_prompt_template`](/python/api/semantic-kernel/semantic_kernel.prompt_template.jinja2_prompt_template)
> - [`handlebars_prompt_template`](/python/api/semantic-kernel/semantic_kernel.prompt_template.handlebars_prompt_template)

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Agent Instructions as a Template

Creating an agent with template parameters provides greater flexibility by allowing its instructions to be easily customized based on different scenarios or requirements. This approach enables the agent's behavior to be tailored by substituting specific values or functions into the template, making it adaptable to a variety of tasks or contexts. By leveraging template parameters, developers can design more versatile agents that can be configured to meet diverse use cases without needing to modify the core logic.

### Chat Completion Agent

::: zone pivot="programming-language-csharp"

```csharp
// Initialize a Kernel with a chat-completion service
Kernel kernel = ...;

var instructions = "Tell a story about {{$topic}} that is {{$length}} sentences long.";

ChatCompletionAgent agent =
    new(templateFactory: new KernelPromptTemplateFactory(),
        templateConfig: new(instructions) { TemplateFormat = PromptTemplateConfig.SemanticKernelTemplateFormat })
    {
        Kernel = kernel,
        Name = "StoryTeller",
        Arguments = new KernelArguments()
        {
            { "topic", "Dog" },
            { "length", "3" },
        }
    };
```

::: zone-end

::: zone pivot="programming-language-python"

```python
agent = ChatCompletionAgent(
    service=AzureChatCompletion(), # or other supported AI Services
    name="StoryTeller",
    instructions="Tell a story about {{$topic}} that is {{$length}} sentences long.",
    arguments=KernelArguments(topic="Dog", length="2"),
)
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

### OpenAI Assistant Agent

Templated instructions are especially powerful when working with an [`OpenAIAssistantAgent`](./agent-types/assistant-agent.md). With this approach, a single assistant definition can be created and reused multiple times, each time with different parameter values tailored to specific tasks or contexts. This enables a more efficient setup, allowing the same assistant framework to handle a wide range of scenarios while maintaining consistency in its core behavior.

::: zone pivot="programming-language-csharp"

```csharp
// Retrieve an existing assistant definition by identifier
AzureOpenAIClient client = OpenAIAssistantAgent.CreateAzureOpenAIClient(new AzureCliCredential(), new Uri("<your endpoint>"));
AssistantClient assistantClient = client.GetAssistantClient();
Assistant assistant = await client.GetAssistantAsync();
OpenAIAssistantAgent agent = new(assistant, assistantClient, new KernelPromptTemplateFactory(), PromptTemplateConfig.SemanticKernelTemplateFormat)
{
    Arguments = new KernelArguments()
    {
        { "topic", "Dog" },
        { "length", "3" },
    }
}
```

::: zone-end

::: zone pivot="programming-language-python"

```python
# Create the client using Azure OpenAI resources and configuration
client, model = AzureAssistantAgent.setup_resources()

# Retrieve the assistant definition from the server based on the assistant ID
definition = await client.beta.assistants.retrieve(
    assistant_id="your-assistant-id",
)

# Create the AzureAssistantAgent instance using the client and the assistant definition
agent = AzureAssistantAgent(
    client=client,
    definition=definition,
    arguments=KernelArguments(topic="Dog", length="3"),
)
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Agent Definition from a Prompt Template

The same Prompt Template Config used to create a Kernel Prompt Function can also be leveraged to define an agent. This allows for a unified approach in managing both prompts and agents, promoting consistency and reuse across different components. By externalizing agent definitions from the codebase, this method simplifies the management of multiple agents, making them easier to update and maintain without requiring changes to the underlying logic. This separation also enhances flexibility, enabling developers to modify agent behavior or introduce new agents by simply updating the configuration, rather than adjusting the code itself.

### YAML Template

```yaml
name: GenerateStory
template: |
  Tell a story about {{$topic}} that is {{$length}} sentences long.
template_format: semantic-kernel
description: A function that generates a story about a topic.
input_variables:
  - name: topic
    description: The topic of the story.
    is_required: true
  - name: length
    description: The number of sentences in the story.
    is_required: true
```

### Agent Initialization

::: zone pivot="programming-language-csharp"

```csharp
// Read YAML resource
string generateStoryYaml = File.ReadAllText("./GenerateStory.yaml");
// Convert to a prompt template config
PromptTemplateConfig templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(generateStoryYaml);

// Create agent with Instructions, Name and Description
// provided by the template config.
ChatCompletionAgent agent =
    new(templateConfig)
    {
        Kernel = this.CreateKernelWithChatCompletion(),
        // Provide default values for template parameters
        Arguments = new KernelArguments()
        {
            { "topic", "Dog" },
            { "length", "3" },
        }
    };
```

::: zone-end

::: zone pivot="programming-language-python"

```python
import yaml

from semantic_kernel.prompt_template import PromptTemplateConfig

# Read the YAML file
with open("./GenerateStory.yaml", "r", encoding="utf-8") as file:
    generate_story_yaml = file.read()

# Parse the YAML content
data = yaml.safe_load(generate_story_yaml)

# Use the parsed data to create a PromptTemplateConfig object
prompt_template_config = PromptTemplateConfig(**data)

agent = ChatCompletionAgent(
    service=AzureChatCompletion(), # or other supported AI services
    prompt_template_config=prompt_template_config,
    arguments=KernelArguments(topic="Dog", length="3"),
)
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## Overriding Template Values for Direct Invocation

When invoking an agent directly, the agent's parameters can be overridden as needed. This allows for greater control and customization of the agent's behavior during specific tasks, enabling you to modify its instructions or settings on the fly to suit particular requirements.

::: zone pivot="programming-language-csharp"

```csharp
// Initialize a Kernel with a chat-completion service
Kernel kernel = ...;

ChatCompletionAgent agent =
    new()
    {
        Kernel = kernel,
        Name = "StoryTeller",
        Instructions = "Tell a story about {{$topic}} that is {{$length}} sentences long.",
        Arguments = new KernelArguments()
        {
            { "topic", "Dog" },
            { "length", "3" },
        }
    };

KernelArguments overrideArguments =
    new()
    {
        { "topic", "Cat" },
        { "length", "3" },
    });

// Generate the agent response(s)
await foreach (ChatMessageContent response in agent.InvokeAsync([], options: new() { KernelArguments = overrideArguments }))
{
  // Process agent response(s)...
}
```

::: zone-end

::: zone pivot="programming-language-python"

```python
agent = ChatCompletionAgent(
    service=AzureChatCompletion(),
    name="StoryTeller",
    instructions="Tell a story about {{$topic}} that is {{$length}} sentences long.",
    arguments=KernelArguments(topic="Dog", length="2"),
)

# Create a thread to maintain the conversation state
# If no threaded is created, a thread will be returned
# with the initial response
thread = None

override_arguments = KernelArguments(topic="Cat", length="3")

# Two ways to get a response from the agent

# Get the response which returns a ChatMessageContent directly
response = await agent.get_response(messages="user input", arguments=override_arguments)
thread = response.thread

# or use the invoke method to return an AsyncIterable of ChatMessageContent
async for response in agent.invoke(messages="user input", arguments=override_arguments):
    # process agent response(s)...
    thread = response.thread
```

::: zone-end

::: zone pivot="programming-language-java"

> Feature currently unavailable in Java.

::: zone-end

## How-To

For an end-to-end example for creating an agent from a _prompt-template_, see:

- [How-To: `ChatCompletionAgent`](./examples/example-chat-agent.md)

## Next steps

> [!div class="nextstepaction"]
> [Agent orchestration](./agent-orchestration/index.md)
