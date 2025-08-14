---
title: Exploring the Semantic Kernel Amazon Bedrock Agent
description: An exploration of the definition, behaviors, and usage patterns for a BedrockAgent in Semantic Kernel
zone_pivot_groups: programming-languages
author: moonbox3
ms.topic: tutorial
ms.author: evmattso
ms.date: 05/29/2025
ms.service: semantic-kernel
---

# Exploring the Semantic Kernel `BedrockAgent`

> [!IMPORTANT]
> Single-agent features, such as `BedrockAgent`, are currently in the experimental stage. These features are under active development and may change before reaching general availability.

Detailed API documentation related to this discussion is available at:

::: zone pivot="programming-language-csharp"
**BedrockAgent API documentation coming soon.**
::: zone-end

::: zone pivot="programming-language-python"
**BedrockAgent API documentation coming soon.**
::: zone-end

::: zone pivot="programming-language-java"
**Feature currently unavailable in Java.**
::: zone-end

## What is a `BedrockAgent`?

The Bedrock Agent is a specialized AI agent within Semantic Kernel designed to integrate with Amazon Bedrock’s Agent service. Like the OpenAI and Azure AI agents, a Bedrock Agent enables advanced multi-turn conversational capabilities with seamless tool (action) integration, but it operates entirely in the AWS ecosystem. It automates function/tool invocation (called action groups in Bedrock), so you don’t have to manually parse and execute actions, and it securely manages conversation state on AWS via sessions, reducing the need to maintain chat history in your application.

A Bedrock Agent differs from other agent types in a few key ways:

- **AWS Managed Execution:** Unlike the OpenAI Assistant which uses OpenAI’s cloud or the Azure AI Agent which uses Azure’s Foundry service, the Bedrock Agent runs on Amazon Bedrock. You must have an AWS account with access to Bedrock (and appropriate IAM permissions) to use it. The agent’s lifecycle (creation, sessions, deletion) and certain tool executions are managed by AWS services, while function-calling tools execute locally within your environment.

- **Foundation Model Selection:** When creating a Bedrock Agent, you specify which foundation model (e.g. an Amazon Titan or partner model) it should use. Only models you have been granted access to can be used. This is different from Chat Completion agents (which you instantiate with a direct model endpoint) – with Bedrock, the model is chosen at agent creation time as the agent’s default capability.

- **IAM Role Requirement:** Bedrock Agents require an IAM role ARN to be provided at creation. This role must have permissions to invoke the chosen model (and any integrated tools) on your behalf. This ensures the agent has the necessary privileges to perform its actions (for example, running code or accessing other AWS services) under your AWS account.

- **Built-in Tools (Action Groups):** Bedrock supports built-in “action groups” (tools) that can be attached to an agent. For example, you can enable a Code Interpreter action group to allow the agent to execute Python code, or a User Input action group to allow the agent to prompt for clarification. These capabilities are analogous to OpenAI’s Code Interpreter plugin or function calling, but in AWS they are configured explicitly on the agent. A Bedrock Agent can also be extended with custom Semantic Kernel plugins (functions) for domain-specific tools, similar to other agents.

- **Session-based Threads:** Conversations with a Bedrock Agent occur in threads tied to Bedrock sessions on AWS. Each thread (session) is identified by a unique ID provided by the Bedrock service, and the conversation history is stored by the service rather than in-process. This means multi-turn dialogues persist on AWS, and you retrieve context via the session ID. The Semantic Kernel `BedrockAgentThread` class abstracts this detail – when you use it, it creates or continues a Bedrock session behind the scenes for the agent.

In summary, `BedrockAgent` allows you to leverage Amazon Bedrock’s powerful agent-and-tools framework through Semantic Kernel, providing goal-directed dialogue with AWS-hosted models and tools. It automates the intricacies of Bedrock’s Agent API (agent creation, session management, tool invocation) so you can interact with it in a high-level, cross-language SK interface.

## Preparing Your Development Environment

To start developing with a `BedrockAgent`, set up your environment with the appropriate Semantic Kernel packages and ensure AWS prerequisites are met.

> [!TIP]
> Check out the [AWS documentation](https://boto3.amazonaws.com/v1/documentation/api/latest/guide/quickstart.html#configuration) on configuring your environment to use the Bedrock API.

::: zone pivot="programming-language-csharp"

Add the Semantic Kernel Bedrock Agents package to your .NET project:

```pwsh
dotnet add package Microsoft.SemanticKernel.Agents.Bedrock --prerelease
```

This will bring in the Semantic Kernel SDK support for Bedrock, including dependencies on the AWS SDK for Bedrock. You may also need to configure AWS credentials (e.g. via environment variables or the default AWS config). The AWS SDK will use your configured credentials; make sure you have your `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, and default region set in your environment or AWS profile. (See AWS’s documentation on credential configuration for more details.)

::: zone-end

::: zone pivot="programming-language-python"

Install the Semantic Kernel package with the AWS extras:

```bash
pip install semantic-kernel[aws]
```

This ensures that the necessary AWS libraries (e.g. boto3) are installed alongside Semantic Kernel. Before using a Bedrock Agent in Python, ensure your AWS credentials and region are properly configured (for example, by setting environment variables or using the AWS CLI). You should have `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, and `AWS_DEFAULT_REGION` (or AWS profile) set so that boto3 can authenticate.

::: zone-end

::: zone pivot="programming-language-java"
**Feature currently unavailable in Java.**
::: zone-end

## Creating a `BedrockAgent`

Creating a Bedrock Agent involves two steps: first, defining the agent with Amazon Bedrock (including selecting a model and providing initial instructions), and then instantiating the Semantic Kernel agent object to interact with it. When you create the agent on AWS, it starts in a non-prepared state, so an additional “prepare” operation is performed to ready it for use.

::: zone pivot="programming-language-csharp"

```csharp
using Amazon.Bedrock;
using Amazon.Bedrock.Model;
using Amazon.BedrockRuntime;
using Microsoft.SemanticKernel.Agents.Bedrock;

// 1. Define a new agent on the Amazon Bedrock service
IAmazonBedrock bedrockClient = new AmazonBedrockClient();  // uses default AWS credentials & region
var createRequest = new CreateAgentRequest 
{
    AgentName = "<foundation model ID>",          // e.g., "anthropic.claude-v2" or other model
    FoundationModel = "<foundation model ID>",    // the same model, or leave null if AgentName is the model
    AgentResourceArn = "<agent role ARN>",        // IAM role ARN with Bedrock permissions
    Instruction = "<agent instructions>"
};
CreateAgentResponse createResponse = await bedrockClient.CreateAgentAsync(createRequest);

// (Optional) Provide a description as needed:
// createRequest.Description = "<agent description>";

// After creation, the agent is in a "NOT_PREPARED" state.
// Prepare the agent to load tools and finalize setup:
await bedrockClient.PrepareAgentAsync(new PrepareAgentRequest 
{
    AgentId = createResponse.Agent.AgentId
});

// 2. Create a Semantic Kernel agent instance from the Bedrock agent definition
IAmazonBedrockRuntime runtimeClient = new AmazonBedrockRuntimeClient();
BedrockAgent agent = new BedrockAgent(createResponse.Agent, bedrockClient, runtimeClient);
```

In the code above, we first use the AWS SDK (`AmazonBedrockClient`) to create an agent on Bedrock, specifying the foundation model, a name, the instructions, and the ARN of the IAM role the agent should assume. The Bedrock service responds with an agent definition (including a unique AgentId). We then call `PrepareAgentAsync` to transition the agent into a ready state (the agent will move from a CREATING status to NOT_PREPARED, then to PREPARED once ready). Finally, we construct a `BedrockAgent` object using the returned definition and the AWS clients. This `BedrockAgent` instance is what we’ll use to send messages and receive responses.

::: zone-end

::: zone pivot="programming-language-python"

```python
import boto3
from semantic_kernel.agents import BedrockAgent

# 1. Define and prepare a new agent on Amazon Bedrock
agent = await BedrockAgent.create_and_prepare_agent(
    name="<agent name>", 
    instructions="<agent instructions>",
    foundation_model="<foundation model ID>",
    agent_resource_role_arn="<agent role ARN>"
)
```

In the example above, `BedrockAgent.create_and_prepare_agent` handles the full creation flow: it uses your AWS configuration (via boto3) to create an agent on Bedrock with the given name, foundation model, and instructions, then automatically waits for the agent to reach a ready state (performing the prepare step internally). The result is a `BedrockAgent` instance ready to use. Under the hood, this method is creating AWS clients (for Bedrock and Bedrock Runtime) using your default credentials, so ensure your AWS environment is set up. If you need custom configuration, you can also manually construct the AWS clients and pass them as parameters (e.g. `client= boto3.client("bedrock")` and `runtime_client= boto3.client("bedrock-runtime")`) to the `create_and_prepare_agent` call.

::: zone-end

::: zone pivot="programming-language-java"
**Feature currently unavailable in Java.**
::: zone-end

## Retrieving an existing `BedrockAgent`

Once an agent has been created on Bedrock, its unique identifier (Agent ID) can be used to retrieve it later. This allows you to re-instantiate a `BedrockAgent` in Semantic Kernel without recreating it from scratch.

::: zone pivot="programming-language-csharp"

For .NET, the Bedrock agent’s identifier is a string accessible via `agent.Id`. To retrieve an existing agent by ID, use the AWS Bedrock client and then construct a new `BedrockAgent`:

```csharp
string existingAgentId = "<your agent ID>";
var getResponse = await bedrockClient.GetAgentAsync(new GetAgentRequest { AgentId = existingAgentId });
BedrockAgent agent = new BedrockAgent(getResponse.Agent, bedrockClient, runtimeClient);
```

Here we call `GetAgentAsync` on the `IAmazonBedrock` client with the known ID, which returns the agent’s definition (name, model, instructions, etc.). We then initialize a new `BedrockAgent` with that definition and the same clients. This agent instance will be linked to the existing Bedrock agent.

::: zone-end

::: zone pivot="programming-language-python"

In Python, you can similarly retrieve an agent by ID using the AWS Bedrock boto3 client, then wrap it in a `BedrockAgent`:

```python
import asyncio, boto3
from semantic_kernel.agents import BedrockAgent

agent_id = "<your agent ID>"
bedrock_client = boto3.client("bedrock")  # Bedrock service client
# Fetch the agent's definition from AWS
agent_info = await asyncio.to_thread(bedrock_client.get_agent, AgentId=agent_id)
# Create the BedrockAgent instance from the retrieved definition
agent = BedrockAgent(agent_model=agent_info["agent"])
```

In this snippet, we use boto3 to call `get_agent` on the Bedrock service (running it in a thread via `asyncio.to_thread` since boto3 is blocking). The returned `agent_info["agent"]` contains the agent’s details (id, name, status, etc.), which we pass into the BedrockAgent constructor. Because we did not explicitly supply AWS clients to `BedrockAgent`, it will internally create new clients with default settings. (Optionally, you could provide `client=` and `runtime_client=` to reuse clients if you have them.)

::: zone-end

::: zone pivot="programming-language-java"
**Feature currently unavailable in Java.**
::: zone-end

## Interacting with a BedrockAgent

Once you have a BedrockAgent instance, interacting with it (sending user messages and receiving AI responses) is straightforward. The agent uses threads to manage conversation context. For a Bedrock Agent, a thread corresponds to an AWS Bedrock session. The Semantic Kernel `BedrockAgentThread` class handles session creation and tracking: when you start a new conversation, a new Bedrock session is started, and as you send messages, Bedrock maintains the alternating user/assistant message history. (Bedrock requires that chat history alternates between user and assistant messages; Semantic Kernel’s channel logic will insert placeholders if necessary to enforce this pattern.) You can invoke the agent without specifying a thread (in which case SK will create a new `BedrockAgentThread` automatically), or you can explicitly create/maintain a thread if you want to continue a conversation across multiple calls. Each invocation returns one or more responses, and you can manage the thread lifetime (e.g., deleting it when done to end the AWS session).

::: zone pivot="programming-language-csharp"

The specifics of the Bedrock agent thread are abstracted by the `BedrockAgentThread` class (which implements the common `AgentThread` interface). The `BedrockAgent` currently only supports threads of type `BedrockAgentThread`.

```csharp
BedrockAgent agent = /* (your BedrockAgent instance, as created above) */;

// Start a new conversation thread for the agent
AgentThread agentThread = new BedrockAgentThread(runtimeClient);
try
{
    // Send a user message and iterate over the response(s)
    var userMessage = new ChatMessageContent(AuthorRole.User, "<your user input>");
    await foreach (ChatMessageContent response in agent.InvokeAsync(userMessage, agentThread))
    {
        Console.WriteLine(response.Content);
    }
}
finally
{
    // Clean up the thread and (optionally) the agent when done
    await agentThread.DeleteAsync();
    await agent.Client.DeleteAgentAsync(new DeleteAgentRequest { AgentId = agent.Id });
}
```

In this example, we explicitly create a `BedrockAgentThread` (passing in the `runtimeClient`, which it uses to communicate with the Bedrock runtime service). We then call `agent.InvokeAsync(...)` with a `ChatMessageContent` representing a user’s message. `InvokeAsync` returns an async stream of responses – in practice, a Bedrock Agent typically returns one final response per invocation (since intermediate tool actions are handled separately), so you’ll usually get a single `ChatMessageContent` from the loop. We print out the assistant’s reply (`response.Content`). In the finally block, we delete the thread, which ends the Bedrock session on AWS. We also delete the agent itself in this case (since we created it just for this example) – this step is optional and only needed if you do not intend to reuse the agent again (see Deleting a BedrockAgent below).

You can continue an existing conversation by reusing the same `agentThread` for subsequent calls. For example, you might loop reading user input and calling `InvokeAsync` each time with the same thread to carry on a multi-turn dialogue. You can also create a BedrockAgentThread with a known session ID to resume a conversation that was saved previously:

```csharp
string sessionId = "<existing Bedrock session ID>";
AgentThread thread = new BedrockAgentThread(runtimeClient, sessionId);
// Now `InvokeAsync` using this thread will continue the conversation from that session
```

::: zone-end

::: zone pivot="programming-language-python"

Using a Bedrock Agent in Python is similar, with the `BedrockAgentThread` managing the session. You can start a new thread or pass an existing one to continue a conversation:

```python
from semantic_kernel.agents import BedrockAgentThread

# Assume `agent` is your BedrockAgent instance
USER_INPUTS = ["Hello", "What's your name?"]

thread = BedrockAgentThread()  # start a new conversation thread (session)
try:
    for user_input in USER_INPUTS:
        response = await agent.get_response(messages=user_input, thread=thread)
        print(response)  # print the assistant's reply
        thread = response.thread  # update thread (BedrockAgentThread) for next turn
finally:
    await thread.delete() if thread else None
```

In this code, we loop through a couple of user inputs. On each iteration, we call `agent.get_response(...)` with the user message and the current thread. The first call starts the Bedrock session and returns an `AgentResponseItem` (or `ChatMessageContent`) containing the assistant’s answer. We print the response, then grab the `response.thread` – which is the same `BedrockAgentThread` updated with the new message context – to use for the next turn. After the conversation (in this example, two turns), we delete the thread to end the session on AWS.

If you omit the `thread` parameter in the call, `agent.get_response` or `agent.invoke` will automatically create a new thread for that invocation and include it in the response.

Optionally, you can also send a batch of messages at once by passing a list of messages to `get_response` or using the asynchronous streaming invocation. For example, to stream the assistant’s response (token by token) for a single prompt:

```python
# Streaming a single response from the Bedrock agent
async for partial in agent.invoke_stream(messages="Tell me a joke.", thread=thread):
    print(partial.content, end="")
```

The `invoke_stream(...)` method yields `ChatMessageContent` objects as the response is generated. By iterating over it, you can output the assistant’s answer incrementally (here we print characters without a newline to form the full response).

::: zone-end

::: zone pivot="programming-language-java"
**Feature currently unavailable in Java.**
::: zone-end

## Deleting a `BedrockAgent`

Bedrock Agents are persistent resources in your AWS account – they will remain (and potentially incur costs or count against service limits) until deleted. If you no longer need an agent you’ve created, you should delete it via the Bedrock service API.

::: zone pivot="programming-language-csharp"

Use the Bedrock client to delete by agent ID. For example:

```csharp
await bedrockAgent.Client.DeleteAgentAsync(new() { AgentId = bedrockAgent.Id });
```

After this call, the agent’s status will change and it will no longer be usable. (Attempting to invoke a deleted agent will result in an error.)

::: zone-end

::: zone pivot="programming-language-python"

Call the agent’s deletion method. For instance:

```python
await agent.delete_agent()
```

This will call the Bedrock service to delete the agent (and internally mark the `BedrockAgent` object as deleted). You can verify by checking `agent.id` or a flag if provided (e.g., `_is_deleted`).

::: zone-end

> **Note:** Deleting a Bedrock agent does not automatically terminate its ongoing sessions. If you have long-running sessions (threads), you should end those by deleting the threads (which calls Bedrock’s EndSession and DeleteSession under the hood). In practice, deleting a thread (as shown in the examples above) ends the session.

::: zone pivot="programming-language-java"
**Feature currently unavailable in Java.**
::: zone-end

## Handling Intermediate Messages with a `BedrockAgent`

When a Bedrock Agent invokes tools (action groups) to arrive at an answer, those intermediate steps (function calls and results) are by default handled internally. The agent’s final answer will reference the outcome of those tools but will not automatically include verbose step-by-step details. However, Semantic Kernel allows you to tap into those intermediate messages for logging or custom handling by providing a callback.

During `agent.invoke(...)` or `agent.invoke_stream(...)`, you can supply an `on_intermediate_message` callback function. This callback will be invoked for each intermediate message generated in the process of formulating the final response. Intermediate messages may include `FunctionCallContent` (when the agent decides to call a function/tool) and `FunctionResultContent` (when a tool returns a result).

For example, suppose our Bedrock Agent has access to a simple plugin (or built-in tool) for menu information, similar to the examples used with OpenAI Assistant:

::: zone pivot="programming-language-python"

```python
from semantic_kernel.contents import ChatMessageContent, FunctionCallContent, FunctionResultContent
from semantic_kernel.functions import kernel_function

# Define a sample plugin with two functions
class MenuPlugin:
    @kernel_function(description="Provides a list of specials from the menu.")
    def get_specials(self) -> str:
        return "Soup: Clam Chowder; Salad: Cobb Salad; Drink: Chai Tea"

    @kernel_function(description="Provides the price of a menu item.")
    def get_item_price(self, menu_item: str) -> str:
        return "$9.99"

# Callback to handle intermediate messages
async def handle_intermediate_steps(message: ChatMessageContent) -> None:
    for item in (message.items or []):
        if isinstance(item, FunctionCallContent):
            print(f"Function Call:> {item.name} with arguments: {item.arguments}")
        elif isinstance(item, FunctionResultContent):
            print(f"Function Result:> {item.result} for function: {item.name}")
        else:
            print(f"[Intermediate] {item}")

# Create the BedrockAgent with the plugin (assuming agent is not yet created above)
agent = await BedrockAgent.create_and_prepare_agent(
    name="MenuAgent",
    instructions="You are a restaurant assistant.",
    foundation_model="<model ID>",
    agent_resource_role_arn="<role ARN>",
    plugins=[MenuPlugin()]  # include our custom plugin
)

# Start a conversation with intermediate callback
thread = BedrockAgentThread()
user_queries = [
    "Hello!",
    "What are the specials today?",
    "What is the special drink?",
    "How much is that?"
]
try:
    for query in user_queries:
        print(f"# User: {query}")
        async for response in agent.invoke(messages=query, thread=thread, on_intermediate_message=handle_intermediate_steps):
            print(f"# Assistant: {response}")
            thread = response.thread
finally:
    await thread.delete() if thread else None
    await agent.delete_agent()
```

In this code, whenever the agent needs to call a function from `MenuPlugin` (for example, `get_specials` or `get_item_price`), the `handle_intermediate_steps` callback will print out a line for the function call and another for the function result. The final assistant response for each user query is then printed as normal. By observing the intermediate content, you can trace how the agent arrived at its answer (which tool was used, what it returned, etc.).

For instance, the output might look like:

```bash
# User: Hello!
# Assistant: Hello! How can I assist you today?
# User: What are the specials today?
Function Call:> MenuPlugin-get_specials with arguments: {}
Function Result:> Soup: Clam Chowder; Salad: Cobb Salad; Drink: Chai Tea for function: MenuPlugin-get_specials
# Assistant: The specials today include Clam Chowder for the soup, Cobb Salad, and Chai Tea as a special drink.
# User: What is the special drink?
# Assistant: The special drink is Chai Tea.
# User: How much is that?
Function Call:> MenuPlugin-get_item_price with arguments: {"menu_item": "Chai Tea"}
Function Result:> $9.99 for function: MenuPlugin-get_item_price
# Assistant: The special drink (Chai Tea) costs $9.99.
```

In the above interaction, the intermediate prints show that the agent successfully called `MenuPlugin.get_specials` and `MenuPlugin.get_item_price` at the appropriate times, and used their results to answer the user. These intermediate details can be logged or used in your application logic as needed (for example, to display the steps the agent took).

::: zone-end

::: zone pivot="programming-language-csharp"

Callback support for intermediate messages in BedrockAgent (C#) follows a similar pattern, but the exact API is under development. (Future releases will enable registering a delegate to handle `FunctionCallContent` and `FunctionResultContent` during `InvokeAsync`.)

::: zone-end

::: zone pivot="programming-language-java"
**Feature currently unavailable in Java.**
::: zone-end

## Using Declarative YAML to Define a Bedrock Agent

Semantic Kernel’s agent framework supports a declarative schema for defining agents via YAML (or JSON). This allows you to specify an agent’s configuration – its type, models, tools, etc. – in a file and then load that agent definition at runtime without writing imperative code to construct it.

> **Note:** YAML-based agent definitions are an emerging feature and may be experimental. Ensure you are using a Semantic Kernel version that supports YAML agent loading, and refer to the latest docs for any format changes.

::: zone pivot="programming-language-csharp"

Using a declarative spec can simplify configuration, especially if you want to easily switch agent setups or use a configuration file approach. For a Bedrock Agent, a YAML definition might look like:

```yaml
type: bedrock_agent
name: MenuAgent
description: Agent that answers questions about a restaurant menu
instructions: You are a restaurant assistant that provides daily specials and prices.
model:
  id: anthropic.claude-v2
agent_resource_role_arn: arn:aws:iam::123456789012:role/BedrockAgentRole
tools:
  - type: code_interpreter
  - type: user_input
  - name: MenuPlugin
    type: kernel_function
```

In this (hypothetical) YAML, we define an agent of type `bedrock_agent`, give it a name and instructions, specify the foundation model by ID, and provide the ARN of the role it should use. We also declare a couple of tools: one enabling the built-in Code Interpreter, another enabling the built-in User Input tool, and a custom MenuPlugin (which would be defined separately in code and registered as a kernel function). Such a file encapsulates the agent’s setup in a human-readable form.

To instantiate an agent from YAML, use the static loader with an appropriate factory. For example:

```csharp
string yamlText = File.ReadAllText("bedrock-agent.yaml");
var factory = new BedrockAgentFactory();  // or an AggregatorAgentFactory if multiple types are used
Agent myAgent = await KernelAgentYaml.FromAgentYamlAsync(kernel, yamlText, factory);
```

This will parse the YAML and produce a `BedrockAgent` instance (or other type based on the `type` field) using the provided kernel and factory.
::: zone-end

::: zone pivot="programming-language-python"
**BedrockAgent Declarative Spec handling is coming soon.**
::: zone-end

::: zone pivot="programming-language-java"
**Feature currently unavailable in Java.**
::: zone-end

Using a declarative schema can be particularly powerful for scenario configuration and testing, as you can swap out models or instructions by editing a config file rather than changing code. Keep an eye on Semantic Kernel’s documentation and samples for more on YAML agent definitions as the feature evolves.



## Further Resources

- **AWS Bedrock Documentation**: To learn more about Amazon Bedrock’s agent capabilities, see *Amazon Bedrock Agents* in the [AWS documentation](https://docs.aws.amazon.com/bedrock/) (e.g., how to configure foundation model access and IAM roles). Understanding the underlying service will help in setting correct permissions and making the most of built-in tools.
- **Semantic Kernel Samples**: The Semantic Kernel repository contains [concept samples](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/concepts/agents/bedrock_agent) for Bedrock Agents. For example, the **Bedrock Agent basic chat sample** in the Python samples demonstrates simple Q&A with a `BedrockAgent`, and the **Bedrock Agent with Code Interpreter sample** shows how to enable and use the Code Interpreter tool. These samples can be a great starting point to see `BedrockAgent` in action.

With the Amazon Bedrock Agent integrated, Semantic Kernel enables truly multi-platform AI solutions – whether you use OpenAI, Azure OpenAI, or AWS Bedrock, you can build rich conversational applications with tool integration using a consistent framework. The `BedrockAgent` opens the door to leveraging AWS’s latest foundation models and secure, extensible agent paradigm within your Semantic Kernel projects.

## Next Steps

> [!div class="nextstepaction"]
> [Explore the Chat Completion Agent](./chat-completion-agent.md)