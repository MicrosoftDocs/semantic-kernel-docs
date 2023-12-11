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

    kernel.import_skill(
        ConversationSummarySkill(kernel=kernel), skill_name="ConversationSummaryPlugin"
    )

    # Create a new context and set the input, history, and options variables.
    variables = sk.ContextVariables()
    variables["input"] = input("User > ")
    variables[
        "history"
    ] = """Bot: How can I help you?
    User: What's the weather like today?
    Bot: Where are you located?
    User: I'm in Seattle.
    Bot: It's 70 degrees and sunny in Seattle today.
    User: Thanks! I'll wear shorts.
    Bot: You're welcome.
    User: Could you remind me what I have on my calendar today?
    Bot: You have a meeting with your team at 2:00 PM.
    User: Oh right! My team just hit a major milestone; I should send them an email to congratulate them."""
    variables["lastMessage"] = "Bot: Would you like to write one for you?"
    variables[
        "options"
    ] = "SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown"

    prompt = """Instructions: What is the intent of this request?
    Choices: {{$choices}}.

    Prior conversation summary: The marketing team needs an update on the new product.
    AI response: What do you want to tell them?
    User Input: Can you send a very quick approval to the marketing team?
    Intent: SendMessage

    Prior conversation summary: The AI offered to send an email to the marketing team.
    AI response: Do you want me to send an email to the marketing team?
    User Input: Yes, please.
    Intent: SendEmail

    Prior conversation summary: {{ConversationSummaryPlugin.SummarizeConversation $history}}
    {{$lastMessage}}
    User Input: {{$request}}
    Intent: """

    # Run the GetIntent function with the context.
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
