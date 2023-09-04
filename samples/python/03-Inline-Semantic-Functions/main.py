import semantic_kernel as sk
import config.add_completion_service
from semantic_kernel import PromptTemplateConfig, PromptTemplate, SemanticFunctionConfig


async def main():
    # Create the prompt for the semantic function
    prompt = """Bot: How can I help you?
    User: {{$input}}

    ---------------------------------------------

    The intent of the user in 5 words or less: """

    # Create the configuration for the semantic function
    prompt_config = PromptTemplateConfig(
        description="Gets the intent of the user.",
        type="completion",
        completion=PromptTemplateConfig.CompletionConfig(0.0, 0.0, 0.0, 0.0, 500),
        input=PromptTemplateConfig.InputConfig(
            parameters=[
                PromptTemplateConfig.InputParameter(
                    name="input", description="The user's request.", default_value=""
                )
            ]
        ),
    )

    # Initialize the kernel
    kernel = sk.Kernel()
    # Add a text or chat completion service using either:
    # kernel.add_text_completion_service()
    # kernel.add_chat_service()
    kernel.add_completion_service()

    # Create the SemanticFunctionConfig object
    prompt_template = PromptTemplate(
        template=prompt,
        template_engine=kernel.prompt_template_engine,
        prompt_config=prompt_config,
    )
    function_config = SemanticFunctionConfig(prompt_config, prompt_template)

    # Register the GetIntent function with the Kernel
    get_intent = kernel.register_semantic_function(
        skill_name="OrchestratorPlugin",
        function_name="GetIntent",
        function_config=function_config,
    )

    # Run the GetIntent function
    result = await kernel.run_async(
        get_intent,
        input_str="I want to send an email to the marketing team celebrating their recent milestone.",
    )

    print(result)


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
