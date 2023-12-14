async def main():
    import semantic_kernel as sk
    from plugins.MathPlugin.Math import Math
    from semantic_kernel.planning.sequential_planner import SequentialPlanner
    import config.add_completion_service

    # Initialize the kernel
    kernel = sk.Kernel()
    # Add a text or chat completion service using either:
    # kernel.add_text_completion_service()
    # kernel.add_chat_service()
    kernel.add_completion_service()

    # Import the native functions
    math_plugin = kernel.import_skill(Math(), "MathPlugin")

    planner = SequentialPlanner(kernel)

    ask = "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent $5 on a latte?"

    # Create a plan
    plan = await planner.create_plan_async(ask)

    # Execute the plan
    result = await plan.invoke_async()
    print("Plan results:")
    print(result)


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
