# Create a Prompt flow with Semantic Kernel

This console application demonstrates the final solution to the [creating prompt flow](https://learn.microsoft.com/en-us/semantic-kernel/ai-orchestration/planners/evaluate-and-deploy-planners/create-a-prompt-flow-with-semantic-kernel) doc article.

## Prerequisites

- [Python](https://www.python.org/downloads/) 3.8 and above
  - [Poetry](https://python-poetry.org/) is used for packaging and dependency management
  - [Semantic Kernel Tools](https://marketplace.visualstudio.com/items?itemName=ms-semantic-kernel.semantic-kernel)

## Configuring the sample

The sample can be configured with a `.env` file which holds api keys and other secrets and configurations.

Make sure you have an
[Open AI API Key](https://openai.com/api/) or
[Azure Open AI service key](https://learn.microsoft.com/azure/cognitive-services/openai/quickstart?pivots=rest-api)

Copy the `.env.example` file as a new file named `.env`. Then, copy those keys into the `.env` file:

```
GLOBAL__LLM_SERVICE="OpenAI"

AZURE_OPEN_AI__DEPLOYMENT_TYPE="chat-completion"
AZURE_OPEN_AI__CHAT_COMPLETION_DEPLOYMENT_NAME="gpt-35-turbo"
AZURE_OPEN_AI__TEXT_COMPLETION_DEPLOYMENT_NAME="text-davinci-003"
AZURE_OPEN_AI__ENDPOINT=""
AZURE_OPEN_AI__API_KEY=""
```

## Running the sample

To run the console application within Visual Studio Code, just hit `F5`.
As configured in `launch.json` and `tasks.json`, Visual Studio Code will run `poetry install` followed by `python hello_world/main.py`

To build and run the console application from the terminal use the following commands:

```powershell
poetry install
poetry run python main.py
```
