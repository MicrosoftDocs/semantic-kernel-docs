import asyncio
from promptflow import tool

import semantic_kernel as sk
from semantic_kernel.planning.sequential_planner import SequentialPlanner
from plugins.MathPlugin.Math import Math as Math
from promptflow.connections import (
    AzureOpenAIConnection,
)

from semantic_kernel.connectors.ai.open_ai import (
    AzureChatCompletion,
    AzureTextCompletion,
)

@tool
def my_python_tool(
    input: str,
    deployment_type: str,
    deployment_name: str,
    AzureOpenAIConnection: AzureOpenAIConnection,
) -> str:
    # Initialize the kernel
    kernel = sk.Kernel(log=sk.NullLogger())

    # Add the chat service
    if deployment_type == "chat-completion":
        kernel.add_chat_service(
            "chat_completion",
            AzureChatCompletion(
                deployment_name,
                endpoint=AzureOpenAIConnection.api_base,
                api_key=AzureOpenAIConnection.api_key,
            ),
        )
    elif deployment_type == "text-completion":
        kernel.add_text_completion_service(
            "text_completion",
            AzureTextCompletion(
                deployment_name,
                endpoint=AzureOpenAIConnection.api_base,
                api_key=AzureOpenAIConnection.api_key,
            ),
        )

    planner = SequentialPlanner(kernel=kernel)

    # Import the native functions
    math_plugin = kernel.import_plugin(Math(), "MathPlugin")

    ask = "Use the available math functions to solve this word problem: " + input

    plan = asyncio.run(planner.create_plan_async(ask))

    # Execute the plan
    result = asyncio.run(kernel.run_async(plan)).result

    for index, step in enumerate(plan._steps):
        print("Function: " + step.plugin_name + "." + step._function.name)
        print("Input vars: " + str(step.parameters.variables))
        print("Output vars: " + str(step._outputs))
    print("Result: " + str(result))

    return str(result)
