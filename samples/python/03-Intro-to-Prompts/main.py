async def main():
    import semantic_kernel as sk
    import config.add_completion_service

    # Initialize the kernel
    kernel = sk.Kernel()
    # Add a text or chat completion service using either:
    # kernel.add_text_completion_service()
    # kernel.add_chat_service()
    kernel.add_completion_service()

    request = input("Your request: ")

    # 0.0 Initial prompt
    prompt = f"What is the intent of this request? {request}"
    print("0.0 Initial prompt")
    semantic_function = kernel.create_semantic_function(prompt)
    print(await kernel.run_async(semantic_function))

    # 1.0 Make the prompt more specific
    prompt = f"""What is the intent of this request? {request}
        You can choose between SendEmail, SendMessage, CompleteTask, CreateDocument."""
    print("1.0 Make the prompt more specific")
    semantic_function = kernel.create_semantic_function(prompt)
    print(await kernel.run_async(semantic_function))

    # 2.0 Add structure to the output with formatting
    prompt = f"""Instructions: What is the intent of this request?
        Choices: SendEmail, SendMessage, CompleteTask, CreateDocument.
        User Input: {request}
        Intent: """
    print("2.0 Add structure to the output with formatting")
    semantic_function = kernel.create_semantic_function(prompt)
    print(await kernel.run_async(semantic_function))

    # 2.1 Add structure to the output with formatting (using Markdown and JSON)
    prompt = f"""## Instructions
        Provide the intent of the request using the following format:
        ```json
        {{
            "intent": {{intent}}
        }}
        ```

        ## Choices
        You can choose between the following intents:
        ```json
        ["SendEmail", "SendMessage", "CompleteTask", "CreateDocument"]
        ```

        ## User Input
        The user input is:
        ```json
        {{
            "request": "{request}"\n'
        }}
        ```

        ## Intent"""
    print("2.1 Add structure to the output with formatting (using Markdown and JSON)")
    semantic_function = kernel.create_semantic_function(prompt)
    print(await kernel.run_async(semantic_function))

    # 3.0 Provide examples with few-shot prompting
    prompt = f"""Instructions: What is the intent of this request?
        Choices: SendEmail, SendMessage, CompleteTask, CreateDocument.

        User Input: Can you send a very quick approval to the marketing team?
        Intent: SendMessage

        User Input: Can you send the full update to the marketing team?
        Intent: SendEmail
        
        User Input: {request}
        Intent: """
    print("3.0 Provide examples with few-shot prompting")
    semantic_function = kernel.create_semantic_function(prompt)
    print(await kernel.run_async(semantic_function))

    # 4.0 Tell the AI what to do to avoid doing something wrong
    prompt = f"""Instructions: What is the intent of this request?
        If you don't know the intent, don't guess; instead respond with "Unknown".
        Choices: SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown.

        User Input: Can you send a very quick approval to the marketing team?
        Intent: SendMessage

        User Input: Can you send the full update to the marketing team?
        Intent: SendEmail
        
        User Input: {request}
        Intent: """
    print("4.0 Tell the AI what to do to avoid doing something wrong")
    semantic_function = kernel.create_semantic_function(prompt)
    print(await kernel.run_async(semantic_function))

    # 5.0 Provide context to the AI
    history = (
        "User input: I hate sending emails, no one ever reads them.\n"
        "AI response: I'm sorry to hear that. Messages may be a better way to communicate."
    )
    prompt = f"""Instructions: What is the intent of this request?\n"
        If you don't know the intent, don't guess; instead respond with "Unknown".
        Choices: SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown.

        User Input: Can you send a very quick approval to the marketing team?
        Intent: SendMessage

        User Input: Can you send the full update to the marketing team?
        Intent: SendEmail

        {history}
        User Input: {request}
        Intent: """
    print("5.0 Provide context to the AI")
    semantic_function = kernel.create_semantic_function(prompt)
    print(await kernel.run_async(semantic_function))


# Run the main function
if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
