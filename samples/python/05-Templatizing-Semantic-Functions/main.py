import semantic_kernel as sk
import config.add_completion_service


async def main():
    # Initialize the kernel
    kernel = sk.Kernel()
    # Add a text or chat completion service using either:
    # kernel.add_text_completion_service()
    # kernel.add_chat_service()
    kernel.add_completion_service()

    plugins_directory = "./plugins"

    # Import the OrchestratorPlugin from the plugins directory.
    orchestrator_plugin = kernel.import_semantic_skill_from_directory(
        plugins_directory, "OrchestratorPlugin"
    )

    # Create context variables and set the input, history, and options variables.
    variables = sk.ContextVariables()
    variables["input"] = "Yes"
    variables[
        "history"
    ] = """Bot: How can I help you?
    User: My team just hit a major milestone and I would like to send them a message to congratulate them.
    Bot:Would you like to send an email?"""
    variables["options"] = "SendEmail, ReadEmail, SendMeeting, RsvpToMeeting, SendChat"

    # Run the GetIntent function with the context.
    result = await kernel.run_async(
        orchestrator_plugin["GetIntent"],
        input_vars=variables,
    )

    print(result)


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
