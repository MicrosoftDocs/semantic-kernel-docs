
### Data retrieval

These functions are used to retrieve data from a database or external API so that an AI can gather additional context to generate a response. This is also known as Retrieval Augmented Generation (RAG). Examples include the following.

| Plugin | Description |
|--------|-------------|
| Web search | Allows an AI to search the web for current information that might not exist in its training data. |
| Time | Gives an AI the ability to see the current time so it can provide time-sensitive information. |
| CRM | Allows an AI to retrieve information about customers. |
| Inventory | Gives an AI the ability to see what is in stock so it can give recommendations to employees and customers alike. |
| Semantic search | Allows an AI to search for information within a specific domain (e.g., internal legal documents); typically powered by a vector DB like Azure AI Search. |

When developing plugins for Retrieval Augmented Generation (RAG), it’s important to note that you don't always need a vector DB. Often your existing APIs can be used by an AI to retrieve the necessary information. We recommend starting with your existing APIs and then moving to a vector DB for semantic search if necessary.
