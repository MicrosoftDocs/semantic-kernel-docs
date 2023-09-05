import semantic_kernel as sk
from plugins.MathPlugin.Math import Math
from plugins.OrchestratorPlugin.Orchestrator import Orchestrator
from semantic_kernel.core_skills import ConversationSummarySkill
import config.add_completion_service


async def main():
    # Initialize the kernel
    kernel = sk.Kernel()
    # Add a text or chat completion service using either:
    # kernel.add_text_completion_service()
    # kernel.add_chat_service()
    kernel.add_completion_service()

    plugins_directory = "./plugins"

    # Import the semantic functions
    kernel.import_semantic_skill_from_directory(plugins_directory, "OrchestratorPlugin")
    kernel.import_skill(
        ConversationSummarySkill(kernel=kernel), skill_name="ConversationSummarySkill"
    )

    # Import the native functions.
    math_plugin = kernel.import_skill(Math(), skill_name="MathPlugin")
    orchestrator_plugin = kernel.import_skill(
        Orchestrator(kernel), skill_name="OrchestratorPlugin"
    )

    # Make a request that runs the Sqrt function
    result1 = (
        await kernel.run_async(
            orchestrator_plugin["route_request"],
            input_str="What is the square root of 524?",
        )
    ).result
    print(result1)

    # Make a request that runs the Add function
    result2 = (
        await kernel.run_async(
            orchestrator_plugin["route_request"],
            input_str="How many square feet would the room be if its length was 12.25 feet and its width was 17.33 feet?",
        )
    ).result
    print(result2["input"])


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
