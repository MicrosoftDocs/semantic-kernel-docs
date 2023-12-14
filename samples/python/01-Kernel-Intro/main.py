async def main():
    from dotenv import dotenv_values
    import semantic_kernel as sk
    from semantic_kernel.core_skills import TimeSkill
    from semantic_kernel.connectors.ai.open_ai import (
        OpenAIChatCompletion,
        AzureChatCompletion,
    )

    # Initialize the kernel
    kernel = sk.Kernel()

    config = dotenv_values(".env")
    llm_service = config.get("GLOBAL__LLM_SERVICE", None)

    # Configure AI service used by the kernel. Load settings from the .env file.
    if llm_service == "AzureOpenAI":
        kernel.add_chat_service(
            "chat_completion",
            AzureChatCompletion(
                config.get("AZURE_OPEN_AI__CHAT_COMPLETION_DEPLOYMENT_NAME", None),
                config.get("AZURE_OPEN_AI__ENDPOINT", None),
                config.get("AZURE_OPEN_AI__API_KEY", None),
            ),
        )
    else:
        kernel.add_chat_service(
            "chat_completion",
            OpenAIChatCompletion(
                config.get("OPEN_AI__CHAT_COMPLETION_MODEL_ID", None),
                config.get("OPEN_AI__API_KEY", None),
                config.get("OPEN_AI__ORG_ID", None),
            ),
        )

    # Import the TimeSkill
    time = kernel.import_skill(TimeSkill())

    # Import the WriterPlugin from the plugins directory.
    plugins_directory = "./plugins"
    writer_plugin = kernel.import_semantic_skill_from_directory(
        plugins_directory, "WriterPlugin"
    )

    # Run the current time function
    currentTime = await kernel.run_async(time["today"])
    print(currentTime)

    # Run the short poem function
    poemResult = await kernel.run_async(
        writer_plugin["ShortPoem"], input_str=str(currentTime)
    )
    print(poemResult)


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
