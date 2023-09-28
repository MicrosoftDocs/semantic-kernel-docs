import semantic_kernel as sk
from plugins.MathPlugin.Math import Math
from semantic_kernel.planning.basic_planner import BasicPlanner
import config.add_completion_service


async def main():
    # Initialize the kernel
    kernel = sk.Kernel()
    # Add a text or chat completion service using either:
    # kernel.add_text_completion_service()
    # kernel.add_chat_service()
    kernel.add_completion_service()

    planner = BasicPlanner()

    # Import the native functions
    math_plugin = kernel.import_skill(Math(), "MathPlugin")

    ask = "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent $5 on a latte?"
    plan = await planner.create_plan_async(ask, kernel)

    # Execute the plan
    result = await planner.execute_plan_async(plan, kernel)

    print("Plan results:")
    print(result)


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
