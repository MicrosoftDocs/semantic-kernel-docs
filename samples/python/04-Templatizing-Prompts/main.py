import semantic_kernel as sk
import config.add_completion_service


async def main():
    # Initialize the kernel
    kernel = sk.Kernel()
    # Add a text or chat completion service using either:
    # kernel.add_text_completion_service()
    # kernel.add_chat_service()
    kernel.add_completion_service()

    history = (
        "User input: I hate sending emails, no one ever reads them.\n"
        "AI response: I'm sorry to hear that. Messages may be a better way to communicate."
    )
    prompt = """Instructions: What is the intent of this request?
        If you don't know the intent, don't guess; instead respond with "Unknown".
        Choices: {{$options}}

        User Input: Can you send a very quick approval to the marketing team?
        Intent: SendMessage

        User Input: Can you send the full update to the marketing team?
        Intent: SendEmail
        
        {{$history}}
        User Input: {{$request}}
        Intent: """

    request = input("Your request: ")

    variables = sk.ContextVariables()
    variables["request"] = request
    variables[
        "options"
    ] = "SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown"
    variables["history"] = history

    # Run the prompt
    semantic_function = kernel.create_semantic_function(prompt)
    result = await kernel.run_async(
        semantic_function,
        input_vars=variables,
    )

    print(result)


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
