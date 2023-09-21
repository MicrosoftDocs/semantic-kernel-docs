import semantic_kernel as sk
from dotenv import dotenv_values
from semantic_kernel.connectors.ai.open_ai import (
    OpenAITextCompletion,
    AzureTextCompletion,
    OpenAIChatCompletion,
    AzureChatCompletion,
)
from semantic_kernel.kernel import Kernel


def add_completion_service(self):
    config = dotenv_values(".env")
    llm_service = config.get("GLOBAL__LLM_SERVICE", None)

    # Configure AI service used by the kernel. Load settings from the .env file.
    if llm_service == "AzureOpenAI":
        deployment_type = config.get("AZURE_OPEN_AI__DEPLOYMENT_TYPE", None)

        if deployment_type == "chat-completion":
            self.add_chat_service(
                "chat_completion",
                AzureChatCompletion(
                    config.get("AZURE_OPEN_AI__CHAT_COMPLETION_DEPLOYMENT_NAME", None),
                    config.get("AZURE_OPEN_AI__ENDPOINT", None),
                    config.get("AZURE_OPEN_AI__API_KEY", None),
                ),
            )
        else:
            self.add_text_completion_service(
                "text_completion",
                AzureTextCompletion(
                    config.get("AZURE_OPEN_AI__TEXT_COMPLETION_DEPLOYMENT_NAME", None),
                    config.get("AZURE_OPEN_AI__ENDPOINT", None),
                    config.get("AZURE_OPEN_AI__API_KEY", None),
                ),
            )
    else:
        model_id = config.get("OPEN_AI__MODEL_TYPE", None)

        if model_id == "chat-completion":
            self.add_chat_service(
                "chat_completion",
                OpenAIChatCompletion(
                    config.get("OPEN_AI__CHAT_COMPLETION_MODEL_ID", None),
                    config.get("OPEN_AI__API_KEY", None),
                    config.get("OPEN_AI__ORG_ID", None),
                ),
            )
        else:
            self.add_text_completion_service(
                "text_completion",
                OpenAITextCompletion(
                    config.get("OPEN_AI__TEXT_COMPLETION_MODEL_ID", None),
                    config.get("OPEN_AI__API_KEY", None),
                    config.get("OPEN_AI__ORG_ID", None),
                ),
            )

Kernel.add_completion_service = add_completion_service