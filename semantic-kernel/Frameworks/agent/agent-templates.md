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

> [!IMPORTANT]
> This feature is in the release candidate stage. Features at this stage are nearly complete and generally stable, though they may undergo minor refinements or optimizations before reaching full general availability.

## Prompt Templates in Semantic Kernel

An agent's role is primarily shaped by the instructions it receives, which dictate its behavior and actions. Similar to invoking a `Kernel` [prompt](../../concepts/prompts/index.md), an agent's instructions can include templated parameters—both values and functions—that are dynamically substituted during execution. This enables flexible, context-aware responses, allowing the agent to adjust its output based on real-time input.

Additionally, an agent can be configured directly using a _Prompt Template Configuration_, providing developers with a structured and reusable way to define its behavior. This approach offers a powerful tool for standardizing and customizing agent instructions, ensuring consistency across various use cases while still maintaining dynamic adaptability.

#### Related API's:

::: zone pivot="programming-language-csharp"

- [`PromptTemplateConfig`](/dotnet/api/microsoft.semantickernel.prompttemplateconfig)
- [`KernelFunctionYaml.FromPromptYaml`](/dotnet/api/microsoft.semantickernel.kernelfunctionyaml.frompromptyaml#microsoft-semantickernel-kernelfunctionyaml-frompromptyaml(system-string-microsoft-semantickernel-iprompttemplatefactory-microsoft-extensions-logging-iloggerfactory))
- [`IPromptTemplateFactory`](/dotnet/api/microsoft.semantickernel.iprompttemplatefactory)
- [`KernelPromptTemplateFactory`](/dotnet/api/microsoft.semantickernel.kernelprompttemplatefactory)
- [_Handlebars_](/dotnet/api/microsoft.semantickernel.prompttemplates.handlebars)
- [_Prompty_](/dotnet/api/microsoft.semantickernel.prompty)
- [_Liquid_](/dotnet/api/microsoft.semantickernel.prompttemplates.liquid)

::: zone-end

::: zone pivot="programming-language-python"

- [`prompt_template_config`](/python/api/semantic-kernel/semantic_kernel.prompt_template.prompt_template_config)
- [`kernel_prompt_template`](/python/api/semantic-kernel/semantic_kernel.prompt_template.kernel_prompt_template)
- [`jinja2_prompt_template`](/python/api/semantic-kernel/semantic_kernel.prompt_template.jinja2_prompt_template)
- [`handlebars_prompt_template`](/python/api/semantic-kernel/semantic_kernel.prompt_template.handlebars_prompt_template)

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Agent Instructions as a Template

Creating an agent with template parameters provides greater flexibility by allowing its instructions to be easily customized based on different scenarios or requirements. This approach enables the agent's behavior to be tailored by substituting specific values or functions into the template, making it adaptable to a variety of tasks or contexts. By leveraging template parameters, developers can design more versatile agents that can be configured to meet diverse use cases without needing to modify the core logic.

#### Chat Completion Agent
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
```
::: zone-end

::: zone pivot="programming-language-python"
```python
kernel = Kernel()

agent = ChatCompletionAgent(
    kernel=kernel,
    name="StoryTeller",
    instructions="Tell a story about {{$topic}} that is {{$length}} sentences long.",
    arguments=KernelArguments(topic="Dog", length="2"),
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end

#### OpenAI Assistant Agent

Templated instructions are especially powerful when working with an [`OpenAIAssistantAgent`](./assistant-agent.md). With this approach, a single assistant definition can be created and reused multiple times, each time with different parameter values tailored to specific tasks or contexts. This enables a more efficient setup, allowing the same assistant framework to handle a wide range of scenarios while maintaining consistency in its core behavior. 

::: zone pivot="programming-language-csharp"
```csharp
// Retrieve an existing assistant definition by identifier
OpenAIAssistantAgent agent = 
    await OpenAIAssistantAgent.RetrieveAsync(
        this.GetClientProvider(),
        "<stored agent-identifier>",
        new Kernel(),
        new KernelArguments()
        {
            { "topic", "Dog" },
            { "length", "3" },
        });
```
::: zone-end

::: zone pivot="programming-language-python"
```python
agent = await OpenAIAssistantAgent.retrieve(
    id=<assistant_id>,
    kernel=Kernel(),
    arguments=KernelArguments(topic="Dog", length="3"),
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## Agent Definition from a _Prompt Template_

The same _Prompt Template Config_ used to create a _Kernel Prompt Function_ can also be leveraged to define an agent. This allows for a unified approach in managing both prompts and agents, promoting consistency and reuse across different components. By externalizing agent definitions from the codebase, this method simplifies the management of multiple agents, making them easier to update and maintain without requiring changes to the underlying logic. This separation also enhances flexibility, enabling developers to modify agent behavior or introduce new agents by simply updating the configuration, rather than adjusting the code itself.

#### YAML Template

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

#### Agent Initialization
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
    kernel=_create_kernel_with_chat_completion(),
    prompt_template_config=prompt_template_config,
    arguments=KernelArguments(topic="Dog", length="3"),
)
```
::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


### Overriding Template Values for Direct Invocation

When invoking an agent directly, without using [`AgentChat`](./agent-chat.md), the agent's parameters can be overridden as needed. This allows for greater control and customization of the agent's behavior during specific tasks, enabling you to modify its instructions or settings on the fly to suit particular requirements.

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

// Create a ChatHistory object to maintain the conversation state.
ChatHistory chat = [];

KernelArguments overrideArguments =
    new()
    {
        { "topic", "Cat" },
        { "length", "3" },
    });

// Generate the agent response(s)
await foreach (ChatMessageContent response in agent.InvokeAsync(chat, overrideArguments))
{
  // Process agent response(s)...
}
```
::: zone-end

::: zone pivot="programming-language-python"

```python
kernel = Kernel()

agent = ChatCompletionAgent(
    kernel=kernel,
    name="StoryTeller",
    instructions="Tell a story about {{$topic}} that is {{$length}} sentences long.",
    arguments=KernelArguments(topic="Dog", length="2"),
)

# Create a chat history to maintain the conversation state
chat = ChatHistory()

override_arguments = KernelArguments(topic="Cat", length="3")

# Two ways to get a response from the agent

# Get the response which returns a ChatMessageContent directly
response = await agent.get_response(chat, arguments=override_arguments)

# or use the invoke method to return an AsyncIterable of ChatMessageContent
async for response in agent.invoke(chat, arguments=override_arguments):
    # process agent response(s)...
```

::: zone-end

::: zone pivot="programming-language-java"

> Agents are currently unavailable in Java.

::: zone-end


## How-To

For an end-to-end example for creating an agent from a _prompt-template_, see:

- [How-To: `ChatCompletionAgent`](./examples/example-chat-agent.md)


> [!div class="nextstepaction"]
> [Configuring Agents with Plugins](./agent-functions.md)

