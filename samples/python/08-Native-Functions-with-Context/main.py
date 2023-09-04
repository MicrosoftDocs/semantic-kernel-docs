import semantic_kernel as sk
from plugins.MathPlugin.Math import Math
import config.add_completion_service


async def main():
    # Initialize the kernel
    kernel = sk.Kernel()
    # Add a text or chat completion service using either:
    # kernel.add_text_completion_service()
    # kernel.add_chat_service()
    kernel.add_completion_service()

    # Import the MathPlugin.
    math_plugin = kernel.import_skill(Math(), skill_name="MathPlugin")

    context = kernel.create_new_context()
    context["input"] = "12.34"
    context["number2"] = "56.78"

    # Run the Multiply function with the context.
    result = await kernel.run_async(
        math_plugin["Multiply"],
        input_context=context,
    )

    print(result)


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
