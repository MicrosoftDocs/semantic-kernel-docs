import semantic_kernel as sk
from semantic_kernel.core_skills import ConversationSummarySkill
import config.add_completion_service


async def main():
    # Initialize the kernel
    kernel = sk.Kernel()
    # Add a text or chat completion service using either:
    # kernel.add_text_completion_service()
    # kernel.add_chat_service()
    kernel.add_completion_service()

    # Import the ConversationSummaryPlugin
    kernel.import_skill(
        ConversationSummarySkill(kernel=kernel), skill_name="ConversationSummaryPlugin"
    )

    # Create the history
    history = []

    # Create the prompt with the ConversationSummaryPlugin
    prompt = """{{ConversationSummaryPlugin.SummarizeConversation $history}}
    User: {{$request}}
    Assistant:  """

    while True:
        request = input("User > ")

        variables = sk.ContextVariables()
        variables["request"] = request
        variables["history"] = "\n".join(history)

        # Run the prompt
        semantic_function = kernel.create_semantic_function(prompt)
        result = await kernel.run_async(
            semantic_function,
            input_vars=variables,
        )

        # Add the request to the history
        history.append("User: " + request)
        history.append("Assistant" + result.result)

        print("Assistant > " + result.result)


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
