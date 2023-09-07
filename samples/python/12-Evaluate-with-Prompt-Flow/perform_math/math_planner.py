import asyncio
from promptflow import tool

import semantic_kernel as sk
from semantic_kernel.planning.sequential_planner import SequentialPlanner
from plugins.MathPlugin.Math import Math as Math

import config.add_completion_service


@tool
def my_python_tool(input1: str) -> str:
    # Initialize the kernel
    kernel = sk.Kernel()
    # Add a text or chat completion service using either:
    # kernel.add_text_completion_service()
    # kernel.add_chat_service()
    kernel.add_completion_service()

    planner = SequentialPlanner(kernel=kernel)

    # Import the native functions
    math_plugin = kernel.import_skill(Math(), "MathPlugin")

    ask = "Use the available math functions to solve this word problem: " + input1

    plan = asyncio.run(planner.create_plan_async(ask))

    # Execute the plan
    result = asyncio.run(kernel.run_async(plan)).result

    for index, step in enumerate(plan._steps):
        print("Function: " + step.skill_name + "." + step._function.name)
        print("Input vars: " + str(step._parameters._variables))
        print("Output vars: " + str(step._outputs))
    print("Result: " + str(result))

    return str(result)
