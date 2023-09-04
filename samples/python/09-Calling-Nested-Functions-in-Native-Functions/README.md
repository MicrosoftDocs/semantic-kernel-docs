# Creating native functions

This console application demonstrates the final solution to the [Calling functions within a native function](https://learn.microsoft.com/en-us/semantic-kernel/ai-orchestration/plugins/native-functions/calling-nested-functions) doc article.

## Prerequisites

- [Python](https://www.python.org/downloads/) 3.8 and above
  - [Poetry](https://python-poetry.org/) is used for packaging and dependency management
  - [Semantic Kernel Tools](https://marketplace.visualstudio.com/items?itemName=ms-semantic-kernel.semantic-kernel)

## Configuring the sample

The sample can be configured with a `.env` file in the project which holds api keys and other secrets and configurations.

Make sure you have an
[Open AI API Key](https://openai.com/api/) or
[Azure Open AI service key](https://learn.microsoft.com/azure/cognitive-services/openai/quickstart?pivots=rest-api)

Copy the `.env.example` file to a new file named `.env`. Then, copy those keys into the `.env` file:

```
GLOBAL__LLM_SERVICE="OpenAI"

AZURE_OPEN_AI__DEPLOYMENT_TYPE="chat-completion"
AZURE_OPEN_AI__CHAT_COMPLETION_DEPLOYMENT_NAME="gpt-35-turbo"
AZURE_OPEN_AI__TEXT_COMPLETION_DEPLOYMENT_NAME="text-davinci-003"
AZURE_OPEN_AI__ENDPOINT=""
AZURE_OPEN_AI__API_KEY=""

OPEN_AI__MODEL_TYPE="chat-completion"
OPEN_AI__CHAT_COMPLETION_MODEL_ID="gpt-3.5-turbo"
OPEN_AI__TEXT_COMPLETION_MODEL_ID="text-davinci-003"
OPEN_AI__API_KEY=""
OPEN_AI__ORG_ID=""
```

This sample has been tested with the following models:

| Service      | Model type      | Model            | Model version | Supported |
| ------------ | --------------- | ---------------- | ------------: | --------- |
| OpenAI       | Text Completion | text-davinci-003 |             1 | ✅        |
| OpenAI       | Chat Completion | gpt-3.5-turbo    |             1 | ✅        |
| OpenAI       | Chat Completion | gpt-3.5-turbo    |          0301 | ✅        |
| OpenAI       | Chat Completion | gpt-4            |             1 | ✅        |
| OpenAI       | Chat Completion | gpt-4            |          0314 | ✅        |
| Azure OpenAI | Text Completion | text-davinci-003 |             1 | ✅        |
| Azure OpenAI | Chat Completion | gpt-3.5-turbo    |          0301 | ✅        |
| Azure OpenAI | Chat Completion | gpt-4       |          0314 | ✅        |

## Running the sample

To run the console application within Visual Studio Code, just hit `F5`.
As configured in `launch.json` and `tasks.json`, Visual Studio Code will run `poetry install` followed by `python hello_world/main.py`

To build and run the console application from the terminal use the following commands:

```powershell
poetry install
poetry run python main.py
```
