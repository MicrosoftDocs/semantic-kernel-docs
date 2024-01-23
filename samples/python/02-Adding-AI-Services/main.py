async def main():
    from dotenv import dotenv_values
    import semantic_kernel as sk
    from semantic_kernel.connectors.ai.open_ai import (
        OpenAIChatCompletion,
        AzureChatCompletion,
        OpenAITextCompletion,
        AzureTextCompletion,
    )

    config = dotenv_values(".env")
    llm_service = config.get("GLOBAL__LLM_SERVICE", None)

    # Configure AI service used by the kernel. Load settings from the .env file.
    if llm_service == "AzureOpenAI":
        deployment_type = config.get("AZURE_OPEN_AI__DEPLOYMENT_TYPE", None)

        if deployment_type == "chat-completion":
            kernel = sk.Kernel()
            kernel.add_chat_service(
                "chat_completion",
                AzureChatCompletion(
                    config.get("AZURE_OPEN_AI__CHAT_COMPLETION_DEPLOYMENT_NAME", None),
                    endpoint=config.get("AZURE_OPEN_AI__ENDPOINT", None),
                    api_key=config.get("AZURE_OPEN_AI__API_KEY", None),
                ),
            )
        else:
            kernel = sk.Kernel()
            kernel.add_text_completion_service(
                "text_completion",
                AzureTextCompletion(
                    config.get("AZURE_OPEN_AI__TEXT_COMPLETION_DEPLOYMENT_NAME", None),
                    endpoint=config.get("AZURE_OPEN_AI__ENDPOINT", None),
                    api_key=config.get("AZURE_OPEN_AI__API_KEY", None),
                ),
            )
    else:
        model_id = config.get("OPEN_AI__MODEL_TYPE", None)

        if model_id == "chat-completion":
            kernel = sk.Kernel()
            kernel.add_chat_service(
                "chat_completion",
                OpenAIChatCompletion(
                    config.get("OPEN_AI__CHAT_COMPLETION_MODEL_ID", None),
                    api_key=config.get("OPEN_AI__API_KEY", None),
                    org_id=config.get("OPEN_AI__ORG_ID", None),
                ),
            )
        else:
            kernel = sk.Kernel()
            kernel.add_text_completion_service(
                "text_completion",
                OpenAITextCompletion(
                    config.get("OPEN_AI__TEXT_COMPLETION_MODEL_ID", None),
                    api_key=config.get("OPEN_AI__API_KEY", None),
                    org_id=config.get("OPEN_AI__ORG_ID", None),
                ),
            )

    plugins_directory = "./plugins"

    # Import the WriterPlugin from the plugins directory.
    writer_plugin = kernel.import_semantic_plugin_from_directory(
        plugins_directory, "WriterPlugin"
    )

    # Run the ShortPoem function with the context.
    result = await kernel.run_async(writer_plugin["ShortPoem"], input_str="Hello world")

    print(result)


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
